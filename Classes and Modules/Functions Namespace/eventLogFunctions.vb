Namespace Functions.eventLogFunctions
    Module eventLogFunctions
        Private Const strSystemRestorePointCreator As String = "System Restore Point Creator"
        Private Const strRegistryApplicationPath As String = "SYSTEM\CurrentControlSet\services\eventlog\Application"

        Public ReadOnly strProgramDataDirectory, strLogFile, strCorruptedLockFile As String
        Private ReadOnly boolCachedCanIWriteThereResults As Boolean
        Private ReadOnly xmlSerializerObject As Xml.Serialization.XmlSerializer
        Public myLogFileLockingMutex As New Threading.Mutex(False, "restorepointcreatorlogfilemutex")

        ' All operations on the log file are done using atomic file transactions. This means that until we have verified that
        ' all data has been written to disk after changing the log file the original file remains intact because the original
        ' log file isn't the actual file that has been operated on; only a temporary file has been worked on.
        '
        ' These are the steps that the program executes when updating or writing the log file.
        ' 1. Acquire a mutex lock. If another part of the program or another instance of the program has a mutex lock then
        '    we wait until the mutex lock has been released and then acquire it after the lock has been released.
        ' 2. Read the data in from the log file and de-serialize it.
        ' 3. Add to or modify the resulting log object in memory.
        ' 4. Serialize the log object and write the new data to a MemoryStream.
        ' 5. Write all data that's in the MemoryStream to a temporary file on disk.
        ' 6. Verify that all data has been written to disk by comparing what's in the MemoryStream to what's been written to disk.
        ' 7. Once we have verified that all data has been successfully written to disk we then delete the original log file
        '    and rename the temporary file to same name of the original file.
        ' 8. And now our atomic file transaction is complete.
        ' 9. Release the mutex lock.
        '
        ' Yes, this process requires more code and time to complete but it ensures that the integrity of the log file is maintained
        ' at all times and as much as humanly possible. All of this is to ensure that if the program crashes during a log file
        ' write operation the log file will maintain its integrity.

        ' This is the sub-routine that loads first when this module of code is called in the program. We can initialize variables in this sub-routine.
        Sub New()
            strProgramDataDirectory = If(globalVariables.boolPortableMode, (New IO.FileInfo(Application.ExecutablePath)).DirectoryName, Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData))
            boolCachedCanIWriteThereResults = privilegeChecks.canIWriteThere(strProgramDataDirectory)

            strLogFile = IO.Path.Combine(strProgramDataDirectory, "Restore Point Creator.log")
            strCorruptedLockFile = IO.Path.Combine(strProgramDataDirectory, "corruptedlog.lock")

            xmlSerializerObject = New Xml.Serialization.XmlSerializer((New List(Of restorePointCreatorExportedLog)).GetType)
        End Sub

        ''' <summary>Opens and returns a IO.FileStream.</summary>
        ''' <param name="strFileToOpen">Path to the file you want to open.</param>
        ''' <param name="accessMethod">The way you want to access the file.</param>
        ''' <param name="fileMode">The way you want to access the file.</param>
        ''' <returns>A IO.FileStream Object.</returns>
        ''' <exception cref="myExceptions.unableToGetLockOnLogFile"></exception>
        Private Function getLogFileIOFileStream(strFileToOpen As String, accessMethod As IO.FileAccess, Optional fileMode As IO.FileMode = IO.FileMode.Open, Optional boolUseLockFile As Boolean = True) As IO.FileStream
            Try
                Return New IO.FileStream(strFileToOpen, fileMode, accessMethod, IO.FileShare.None)
            Catch ex As IO.IOException
                Throw New myExceptions.unableToGetLockOnLogFile() With {.innerIOException = ex}
            End Try
        End Function

        ''' <summary>Exports the application logs to a file.</summary>
        ''' <param name="strFileToBeExportedTo">The path to the file we will be exporting the data to.</param>
        ''' <param name="logCount">This is a ByRef argument which passes back the number of logs that this function exported.</param>
        ''' <returns>Returns a Boolean value. If True the logs were successfully exported, if False then something went wrong.</returns>
        Public Function exportLogsToFile(ByVal strFileToBeExportedTo As String, ByRef logCount As Long) As Boolean
            Try
                ' We don't get a mutex lock here because the getLogObject() function has mutex lock code
                ' in it already. If we get a mutex lock here then we would never get the logObject.
                Dim logObject As New exportedLogFile With {
                    .operatingSystem = osVersionInfo.getFullOSVersionString,
                    .programVersion = globalVariables.version.strFullVersionString,
                    .version = 5,
                    .logsEntries = getLogObject()
                }
                logCount = logObject.logsEntries.Count

                If IO.File.Exists(strFileToBeExportedTo) Then support.deleteFileWithNoException(strFileToBeExportedTo)

                Try
                    ' Create a MemoryStream to temporarily write our serialized data to.
                    Using memoryStream As New IO.MemoryStream
                        ' Create an XMLSerializer to write our serialized data to our MemoryStream.
                        Dim resLogXSerializer As New Xml.Serialization.XmlSerializer(logObject.GetType)
                        ' And serialize it.
                        resLogXSerializer.Serialize(memoryStream, logObject)

                        ' Write all data in our MemoryStream to disk.
                        IO.File.WriteAllBytes(strFileToBeExportedTo, memoryStream.ToArray())

                        ' And now verify it.
                        If verifyDataOnDisk(memoryStream, strFileToBeExportedTo) Then
                            ' If the verification passed, return True.
                            Return True
                        Else
                            ' If the verification DIDN'T pass, delete the file.
                            support.deleteFileWithNoException(strFileToBeExportedTo)
                            ' And return False.
                            Return False
                        End If
                    End Using
                Catch ex As Exception
                    writeCrashToApplicationLogFile(ex)
                    Return False
                End Try
            Catch ex As Exception
                writeCrashToApplicationLogFile(ex)
                writeToApplicationLogFile("There was an error while attempting to export the program's event log entries.", EventLogEntryType.Error, False)

                Return False
            End Try
        End Function

        Public Function getLogObject() As List(Of restorePointCreatorExportedLog)
            myLogFileLockingMutex.WaitOne() ' We wait here until any other code that's working with the log file is finished executing.
            Dim internalApplicationLog As New List(Of restorePointCreatorExportedLog)

            If IO.File.Exists(strLogFile) Then
                Try
                    Dim boolDidWeHaveACorruptedLogFile As Boolean = False

                    Using fileStream As IO.FileStream = getLogFileIOFileStream(strLogFile, IO.FileAccess.Read)
                        Using streamReader As New IO.StreamReader(fileStream)
                            If (New IO.FileInfo(strLogFile)).Length = 0 Then
                                internalApplicationLog.Add(New restorePointCreatorExportedLog With {
                                    .logData = "The log file was found to be empty.",
                                    .logType = EventLogEntryType.Error,
                                    .unixTime = 0,
                                    .logSource = "Restore Point Creator",
                                    .logID = internalApplicationLog.Count,
                                    .dateObject = Now.ToUniversalTime,
                                    .boolException = False
                                })

                                boolDidWeHaveACorruptedLogFile = True
                            Else
                                Try
                                    internalApplicationLog = xmlSerializerObject.Deserialize(streamReader)
                                Catch ex As Exception
                                    boolDidWeHaveACorruptedLogFile = True
                                    handleCorruptedXMLLogFile(internalApplicationLog, fileStream)
                                End Try
                            End If
                        End Using
                    End Using

                    If boolDidWeHaveACorruptedLogFile Then
                        Using fileStream As IO.FileStream = getLogFileIOFileStream(strLogFile, IO.FileAccess.Write, IO.FileMode.Create, False)
                            xmlSerializerObject.Serialize(fileStream, internalApplicationLog)
                        End Using
                    End If
                Catch ex As myExceptions.unableToGetLockOnLogFile
                    oldEventLogFunctions.boolShowErrorMessage = True
                    oldEventLogFunctions.writeCrashToEventLog(ex.innerIOException)
                End Try
            End If

            myLogFileLockingMutex.ReleaseMutex() ' Release the mutex so that other code can work with the log file.
            Return internalApplicationLog
        End Function

        ''' <summary>Writes the contents of a MemoryStream to disk and verifies the contents of both. It returns a Boolean value indicating the results of the file operation.</summary>
        ''' <param name="memoryStream">The MemoryStream Object that you want to write to disk.</param>
        ''' <param name="strPathOfFileToVerify">The path of the file to which you want to write the contents of the MemoryStream to.</param>
        ''' <returns>A Boolean value indicating the results of the file operation.</returns>
        Private Function verifyDataOnDisk(ByRef memoryStream As IO.MemoryStream, ByVal strPathOfFileToVerify As String) As Boolean
            Dim tempFileByteArray As Byte() = IO.File.ReadAllBytes(strPathOfFileToVerify)
            Return If(tempFileByteArray.Length = memoryStream.Length AndAlso tempFileByteArray.SequenceEqual(memoryStream.ToArray), True, False)
        End Function

        ''' <summary>Writes the contents of a "List(Of restorePointCreatorExportedLog)" Object to disk and verifies it. It returns a Boolean value indicating the results of the file operation.</summary>
        ''' <param name="applicationLog">A Log Object.</param>
        ''' <returns>A Boolean value indicating the results of the file operation.</returns>
        Private Function writeDataToDiskAndVerifyIt(ByRef applicationLog As List(Of restorePointCreatorExportedLog)) As Boolean
            Dim boolSuccessfulWriteToDisk As Boolean = False ' Declare a variable to store our results of the write operation.

            ' Create a MemoryStream to temporarily write our serialized data to.
            Using memoryStream As New IO.MemoryStream()
                ' Create an XMLSerialized to write our serialized data to our MemoryStream.
                xmlSerializerObject.Serialize(memoryStream, applicationLog)

                ' Write all data in our MemoryStream to disk.
                IO.File.WriteAllBytes(strLogFile & ".temp", memoryStream.ToArray())

                ' This validates the data that has been written to disk in the form of a temporary file.
                boolSuccessfulWriteToDisk = verifyDataOnDisk(memoryStream, strLogFile & ".temp")
            End Using

            If boolSuccessfulWriteToDisk Then
                ' Delete the current log file.
                support.deleteFileWithNoException(strLogFile)

                ' And now our atomic file transaction is complete. The data that has been written to disk
                ' has been validated so we can move the new log file into place of the old log file.
                IO.File.Move(strLogFile & ".temp", strLogFile)

                Return True ' OK, things went good so we return a True value.
            Else
                support.deleteFileWithNoException(strLogFile & ".temp")
                Return False ' Oops, something went wrong; return a False value.
            End If
        End Function

        ''' <exception cref="myExceptions.logFileWriteToDiskFailureException" />
        Public Sub getOldLogsFromWindowsEventLog()
            Try
                If Not IO.File.Exists(strLogFile) Then createLogFile()
                writeToApplicationLogFile("Starting log conversion process.", EventLogEntryType.Information, False)

                myLogFileLockingMutex.WaitOne() ' We wait here until any other code that's working with the log file is finished executing.

                Dim applicationLog As New List(Of restorePointCreatorExportedLog)
                Dim logCount, longOldLogCount As Long
                Dim stopwatch As Stopwatch = Stopwatch.StartNew

                Try
                    Using fileStream As IO.FileStream = getLogFileIOFileStream(strLogFile, IO.FileAccess.Read)
                        Using streamReader As New IO.StreamReader(fileStream)
                            applicationLog = xmlSerializerObject.Deserialize(streamReader)
                        End Using
                    End Using

                    logCount = applicationLog.Count
                    longOldLogCount = logCount

                    exportApplicationEventLogEntriesToFile(globalVariables.eventLog.strApplication, applicationLog, logCount)
                    exportApplicationEventLogEntriesToFile(globalVariables.eventLog.strSystemRestorePointCreator, applicationLog, logCount)

                    If Not writeDataToDiskAndVerifyIt(applicationLog) Then Throw New myExceptions.logFileWriteToDiskFailureException()
                Catch ex As myExceptions.unableToGetLockOnLogFile
                    oldEventLogFunctions.boolShowErrorMessage = True
                    oldEventLogFunctions.writeCrashToEventLog(ex.innerIOException)
                    Exit Sub
                End Try

                Dim longNumberOfImportedLogs As Long = applicationLog.Count - longOldLogCount

                myLogFileLockingMutex.ReleaseMutex() ' Release the mutex so that other code can work with the log file.

                writeToApplicationLogFile("Log conversion process complete.", EventLogEntryType.Information, False)

                If longNumberOfImportedLogs = 1 Then
                    writeToApplicationLogFile(String.Format("Converted log data to new log file format in {0}ms. 1 log entry was imported.", stopwatch.ElapsedMilliseconds.ToString), EventLogEntryType.Information, False)
                ElseIf longNumberOfImportedLogs > 1 Then
                    writeToApplicationLogFile(String.Format("Converted log data to new log file format in {0}ms. {1} log entries were imported.", stopwatch.ElapsedMilliseconds.ToString, longNumberOfImportedLogs.ToString("N0")), EventLogEntryType.Information, False)
                Else
                    writeToApplicationLogFile(String.Format("Converted log data to new log file format in {0}ms. No old log entries were detected.", stopwatch.ElapsedMilliseconds.ToString), EventLogEntryType.Information, False)
                End If
            Catch ex As Exception
                myLogFileLockingMutex.ReleaseMutex() ' Release the mutex so that other code can work with the log file.
                writeCrashToApplicationLogFile(ex)
            End Try
        End Sub

        Private Sub createLogFile()
            Try
                myLogFileLockingMutex.WaitOne() ' We wait here until any other code that's working with the log file is finished executing.
                Dim applicationLog As New List(Of restorePointCreatorExportedLog)

                applicationLog.Add(New restorePointCreatorExportedLog With {
                    .logData = "Log file initialized.",
                    .logType = EventLogEntryType.Information,
                    .unixTime = 0,
                    .logSource = "Restore Point Creator",
                    .logID = applicationLog.Count,
                    .dateObject = Now.ToUniversalTime,
                    .boolException = False
                })

                Try
                    Dim boolSuccessfulWriteToDisk As Boolean = False ' Declare a variable to store our results of the write operation.

                    Using memoryStream As New IO.MemoryStream()
                        xmlSerializerObject.Serialize(memoryStream, applicationLog)
                        IO.File.WriteAllBytes(strLogFile & ".temp", memoryStream.ToArray())

                        boolSuccessfulWriteToDisk = verifyDataOnDisk(memoryStream, strLogFile & ".temp")
                    End Using

                    If boolSuccessfulWriteToDisk Then
                        support.deleteFileWithNoException(strLogFile)
                        IO.File.Move(strLogFile & ".temp", strLogFile)
                    Else
                        support.deleteFileWithNoException(strLogFile & ".temp")
                    End If
                Catch ex As myExceptions.unableToGetLockOnLogFile
                    oldEventLogFunctions.boolShowErrorMessage = True
                    oldEventLogFunctions.writeCrashToEventLog(ex.innerIOException)
                Finally
                    myLogFileLockingMutex.ReleaseMutex() ' Release the mutex so that other code can work with the log file.
                End Try
            Catch ex As Exception
                MsgBox(ex.Message)
            End Try
        End Sub

        ''' <summary>A function that re-IDs all of the log entries.</summary>
        ''' <param name="oldLogEntryList">A List of restorePointCreatorExportedLog Objects.</param>
        ''' <returns>A List of restorePointCreatorExportedLog Objects.</returns>
        Private Function reIDTheLogEntries(oldLogEntryList As List(Of restorePointCreatorExportedLog)) As List(Of restorePointCreatorExportedLog)
            ' We sort the List of log entries first.
            oldLogEntryList = oldLogEntryList.OrderBy(Function(logEntry As restorePointCreatorExportedLog) logEntry.logID).ToList()

            Dim newLogID As Long = 0
            Dim newLogEntryList As New List(Of restorePointCreatorExportedLog)

            For Each logEntry As restorePointCreatorExportedLog In oldLogEntryList
                logEntry.logID = newLogID
                newLogEntryList.Add(logEntry)
                newLogID += 1
            Next

            Return newLogEntryList
        End Function

        Private Sub deleteEntryFromLogSubRoutine(ByRef applicationLog As List(Of restorePointCreatorExportedLog), longIDToBeDeleted As Long)
            Dim itemToBeRemoved As restorePointCreatorExportedLog = applicationLog.Where(Function(logObject As restorePointCreatorExportedLog) (logObject.logID = longIDToBeDeleted))(0)
            applicationLog.Remove(itemToBeRemoved)
        End Sub

        ''' <summary>Deletes a list of individual log entries from the log.</summary>
        ''' <exception cref="myExceptions.logFileWriteToDiskFailureException" />
        Public Sub deleteEntryFromLog(idsOfLogsToBeDeleted As List(Of Long))
            Try
                myLogFileLockingMutex.WaitOne() ' We wait here until any other code that's working with the log file is finished executing.
                Dim applicationLog As New List(Of restorePointCreatorExportedLog)

                Using fileStream As IO.FileStream = getLogFileIOFileStream(strLogFile, IO.FileAccess.Read)
                    Using streamReader As New IO.StreamReader(fileStream)
                        applicationLog = xmlSerializerObject.Deserialize(streamReader)
                    End Using
                End Using

                For Each longIDToBeDeleted As Long In idsOfLogsToBeDeleted
                    deleteEntryFromLogSubRoutine(applicationLog, longIDToBeDeleted)
                Next

                applicationLog = reIDTheLogEntries(applicationLog)

                If Not writeDataToDiskAndVerifyIt(applicationLog) Then Throw New myExceptions.logFileWriteToDiskFailureException()
            Catch ex As myExceptions.unableToGetLockOnLogFile
                oldEventLogFunctions.boolShowErrorMessage = True
                oldEventLogFunctions.writeCrashToEventLog(ex.innerIOException)
            Catch ex As Exception
                ' Does nothing.
            Finally
                myLogFileLockingMutex.ReleaseMutex() ' Release the mutex so that other code can work with the log file.
            End Try
        End Sub

        ''' <summary>Deletes an individual log entry from the log.</summary>
        ''' <exception cref="myExceptions.logFileWriteToDiskFailureException" />
        Public Sub deleteEntryFromLog(longIDToBeDeleted As Long)
            Try
                myLogFileLockingMutex.WaitOne() ' We wait here until any other code that's working with the log file is finished executing.
                Dim applicationLog As New List(Of restorePointCreatorExportedLog)

                Using fileStream As IO.FileStream = getLogFileIOFileStream(strLogFile, IO.FileAccess.Read)
                    Using streamReader As New IO.StreamReader(fileStream)
                        applicationLog = xmlSerializerObject.Deserialize(streamReader)
                    End Using
                End Using

                deleteEntryFromLogSubRoutine(applicationLog, longIDToBeDeleted)
                applicationLog = reIDTheLogEntries(applicationLog)

                If Not writeDataToDiskAndVerifyIt(applicationLog) Then Throw New myExceptions.logFileWriteToDiskFailureException()
            Catch ex As myExceptions.unableToGetLockOnLogFile
                oldEventLogFunctions.boolShowErrorMessage = True
                oldEventLogFunctions.writeCrashToEventLog(ex.innerIOException)
            Catch ex As Exception
                ' Does nothing.
            Finally
                myLogFileLockingMutex.ReleaseMutex() ' Release the mutex so that other code can work with the log file.
            End Try
        End Sub

        Private Function incrementFileName(input As String) As String
            If IO.File.Exists(input) Then
                Dim i As Short = 1
                While IO.File.Exists(input & i.ToString)
                    i += 1
                End While
                Return input & i.ToString
            Else
                Return input
            End If
        End Function

        Private Sub handleCorruptedXMLLogFile(ByRef applicationLog As List(Of restorePointCreatorExportedLog), ByRef fileStream As IO.FileStream)
            Using newCorruptedLogFileStream As IO.FileStream = getLogFileIOFileStream(incrementFileName(strLogFile & ".corrupted"), strCorruptedLockFile, IO.FileAccess.Write, IO.FileMode.CreateNew)
                fileStream.Position = 0
                fileStream.CopyTo(newCorruptedLogFileStream)
            End Using

            support.deleteFileWithNoException(strCorruptedLockFile)

            applicationLog.Add(New restorePointCreatorExportedLog With {
                .logData = "A corrupted XML log file was detected, the corrupted log file was backed up and a new log file has been created.",
                .logType = EventLogEntryType.Error,
                .unixTime = 0,
                .logSource = "Restore Point Creator",
                .logID = applicationLog.Count,
                .dateObject = Now.ToUniversalTime,
                .boolException = False
            })
        End Sub

        Public Sub markLastExceptionLogAsSubmitted()
            Try
                myLogFileLockingMutex.WaitOne() ' We wait here until any other code that's working with the log file is finished executing.
                Dim applicationLog As New List(Of restorePointCreatorExportedLog)

                Using fileStream As IO.FileStream = getLogFileIOFileStream(strLogFile, IO.FileAccess.Read)
                    Using streamReader As New IO.StreamReader(fileStream)
                        Try
                            applicationLog = xmlSerializerObject.Deserialize(streamReader)
                        Catch ex As InvalidOperationException
                            ' OK, at this point we have nothing to mark in the log file so we just exit this sub-routine.
                            handleCorruptedXMLLogFile(applicationLog, fileStream)
                            Exit Sub
                        End Try
                    End Using
                End Using

                ' Gets the latest log entry in the log file.
                Dim longNewestLogEntry As Long = applicationLog.Max(Function(logEntryObject As restorePointCreatorExportedLog) logEntryObject.logID And logEntryObject.boolException)

                ' Sets the "submitted" bit to True.
                applicationLog.First(Function(logEntryObject As restorePointCreatorExportedLog) logEntryObject.logID = longNewestLogEntry).boolSubmitted = True

                writeDataToDiskAndVerifyIt(applicationLog)
            Catch ex As Exception
            Finally
                myLogFileLockingMutex.ReleaseMutex() ' Release the mutex so that other code can work with the log file.
            End Try
        End Sub

        Public Sub markLogEntryAsSubmitted(inputLogID As Long)
            Try
                myLogFileLockingMutex.WaitOne() ' We wait here until any other code that's working with the log file is finished executing.
                Dim applicationLog As New List(Of restorePointCreatorExportedLog)

                Using fileStream As IO.FileStream = getLogFileIOFileStream(strLogFile, IO.FileAccess.Read)
                    Using streamReader As New IO.StreamReader(fileStream)
                        Try
                            applicationLog = xmlSerializerObject.Deserialize(streamReader)
                        Catch ex As InvalidOperationException
                            ' OK, at this point we have nothing to mark in the log file so we just exit this sub-routine.
                            handleCorruptedXMLLogFile(applicationLog, fileStream)
                            Exit Sub
                        End Try
                    End Using
                End Using

                ' Sets the "submitted" bit to True.
                applicationLog.First(Function(logEntryObject As restorePointCreatorExportedLog) logEntryObject.logID = inputLogID).boolSubmitted = True

                writeDataToDiskAndVerifyIt(applicationLog)
            Catch ex As Exception
            Finally
                myLogFileLockingMutex.ReleaseMutex() ' Release the mutex so that other code can work with the log file.
            End Try
        End Sub

        ''' <summary>Writes a log entry to the System Event Log.</summary>
        ''' <param name="logMessage">The text you want to have in your new System Event Log entry.</param>
        ''' <param name="eventLogType">The type of log that you want your entry to be. The three major options are Error, Information, and Warning.</param>
        ''' <example>functions.eventLogFunctions.writeToSystemEventLog("My Event Log Entry", EventLogEntryType.Information, False)</example>
        Public Sub writeToApplicationLogFile(logMessage As String, eventLogType As EventLogEntryType, boolExceptionInput As Boolean)
            Try
                myLogFileLockingMutex.WaitOne() ' We wait here until any other code that's working with the log file is finished executing.
                Dim applicationLog As New List(Of restorePointCreatorExportedLog)

                If Not IO.File.Exists(strLogFile) Then createLogFile()

                Using fileStream As IO.FileStream = getLogFileIOFileStream(strLogFile, IO.FileAccess.Read)
                    Using streamReader As New IO.StreamReader(fileStream)
                        If (New IO.FileInfo(strLogFile)).Length = 0 Then
                            applicationLog.Add(New restorePointCreatorExportedLog With {
                                .logData = "The log file was found to be empty.",
                                .logType = EventLogEntryType.Error,
                                .unixTime = 0,
                                .logSource = "Restore Point Creator",
                                .logID = applicationLog.Count,
                                .dateObject = Now.ToUniversalTime,
                                .boolException = boolExceptionInput
                            })
                        Else
                            Try
                                applicationLog = xmlSerializerObject.Deserialize(streamReader)
                            Catch ex As InvalidOperationException
                                handleCorruptedXMLLogFile(applicationLog, fileStream)
                            End Try
                        End If
                    End Using
                End Using

                applicationLog.Add(New restorePointCreatorExportedLog With {
                    .logData = logMessage,
                    .logType = eventLogType,
                    .unixTime = 0,
                    .logSource = "Restore Point Creator",
                    .logID = applicationLog.Count,
                    .dateObject = Now.ToUniversalTime,
                    .boolException = boolExceptionInput
                })

                writeDataToDiskAndVerifyIt(applicationLog)
            Catch ex As myExceptions.unableToGetLockOnLogFile
                oldEventLogFunctions.writeToSystemEventLog(logMessage, eventLogType)
                oldEventLogFunctions.boolShowErrorMessage = True
                oldEventLogFunctions.writeCrashToEventLog(ex.innerIOException)
            Catch ex As Exception
                oldEventLogFunctions.writeCrashToEventLog(ex)
            Finally
                myLogFileLockingMutex.ReleaseMutex() ' Release the mutex so that other code can work with the log file.
            End Try
        End Sub

        Public Function convertLogTypeToText(errorType As EventLogEntryType) As String
            If errorType = EventLogEntryType.Error Then
                Return "Error"
            ElseIf errorType = EventLogEntryType.Information Then
                Return "Information"
            ElseIf errorType = EventLogEntryType.Warning Then
                Return "Warning"
            ElseIf errorType = EventLogEntryType.FailureAudit Then
                Return "FailureAudit"
            ElseIf errorType = EventLogEntryType.SuccessAudit Then
                Return "SuccessAudit"
            Else
                Return "Information"
            End If
        End Function

        ''' <summary>A function that is called in two different functions to assemble the crash data that's being written to the log file.</summary>
        ''' <param name="exceptionObject">The Exception Object.</param>
        ''' <param name="errorType">The ErrorType.</param>
        ''' <returns>A String value.</returns>
        Public Function assembleCrashData(exceptionObject As Exception, errorType As EventLogEntryType) As String
            Dim stringBuilder As New Text.StringBuilder

            stringBuilder.AppendLine("System Information")
            stringBuilder.AppendLine("Time of Crash: " & Now.ToString)
            stringBuilder.AppendLine("Operating System: " & osVersionInfo.getFullOSVersionString())
            stringBuilder.AppendLine("System RAM: " & support.getSystemRAM())

            If Debugger.IsAttached Then
                stringBuilder.AppendLine("Debug Mode: True")
            Else
                stringBuilder.AppendLine("Debug Mode: False")
            End If

            If globalVariables.version.boolDebugBuild Then
                stringBuilder.AppendLine("Debug Build: True")
            Else
                stringBuilder.AppendLine("Debug Build: False")
            End If

            Dim processorInfo As supportClasses.processorInfoClass = wmi.getSystemProcessor()
            stringBuilder.AppendLine("CPU: " & processorInfo.strProcessor)
            stringBuilder.AppendLine("Number of Cores: " & processorInfo.strNumberOfCores.ToString)

            stringBuilder.AppendLine()

            If globalVariables.version.boolBeta = True Then
                stringBuilder.AppendLine("Program Version: " & String.Format("{0} Public Beta {1}", globalVariables.version.strFullVersionString, globalVariables.version.shortBetaVersion))
            ElseIf globalVariables.version.boolReleaseCandidate = True Then
                stringBuilder.AppendLine("Program Version: " & String.Format("{0} Release Candidate {1}", globalVariables.version.strFullVersionString, globalVariables.version.shortReleaseCandidateVersion))
            Else
                stringBuilder.AppendLine("Program Version: " & globalVariables.version.strFullVersionString)
            End If

            support.addExtendedCrashData(stringBuilder, exceptionObject)

            stringBuilder.AppendLine("Log Type: " & convertLogTypeToText(errorType))
            stringBuilder.AppendLine("Running As: " & Environment.UserName)
            stringBuilder.AppendLine("Exception Type: " & exceptionObject.GetType.ToString)
            stringBuilder.AppendLine("Message: " & support.removeSourceCodePathInfo(exceptionObject.Message))

            stringBuilder.AppendLine()

            stringBuilder.Append("The exception occurred ")

            stringBuilder.AppendLine(support.removeSourceCodePathInfo(exceptionObject.StackTrace.Trim))

            If exceptionObject.InnerException IsNot Nothing Then
                Try
                    stringBuilder.AppendLine()
                    stringBuilder.AppendLine()
                    stringBuilder.AppendLine()
                    stringBuilder.AppendLine("An inner exception was found, below is the data from it...")
                    stringBuilder.AppendLine()
                    stringBuilder.AppendLine(exceptionObject.InnerException.GetType.ToString & " Inner Exception Data")
                    stringBuilder.AppendLine(assembleInnerExceptionCrashData(exceptionObject.InnerException).Trim)
                Catch ex As Exception
                End Try
            End If

            Return stringBuilder.ToString.Trim
        End Function

        Private Function assembleInnerExceptionCrashData(exceptionObject As Exception)
            Dim stringBuilder As New Text.StringBuilder
            stringBuilder.AppendLine("Exception Type: " & exceptionObject.GetType.ToString)
            stringBuilder.AppendLine("Message: " & support.removeSourceCodePathInfo(exceptionObject.Message))
            stringBuilder.AppendLine()
            stringBuilder.Append("The exception occurred ")
            stringBuilder.AppendLine(support.removeSourceCodePathInfo(exceptionObject.StackTrace.Trim))

            Return stringBuilder.ToString.Trim
        End Function

        ''' <summary>Writes the exception event to the System Log File. This is a universal exception logging function that's built to handle various forms of exceptions and not not any particular type.</summary>
        ''' <param name="exceptionObject">The exception object.</param>
        ''' <param name="errorType">The type of Event Log you want the Exception Event to be recorded to the Application Event Log as.</param>
        ''' <example>functions.eventLogFunctions.writeCrashToEventLog(ex)</example>
        Public Sub writeCrashToApplicationLogFile(exceptionObject As Exception, Optional errorType As EventLogEntryType = EventLogEntryType.Error)
            Try
                writeToApplicationLogFile(assembleCrashData(exceptionObject, errorType), errorType, True)
            Catch ex2 As Exception
                oldEventLogFunctions.writeCrashToEventLog(ex2)
            End Try
        End Sub

        ''' <summary>This sub-routine is called by the exportLogsToFile() sub-routine, this is not intended to be called outside of this module.</summary>
        ''' <param name="logCount">This is a ByRef argument which passes back the number of logs that this function exported.</param>
        ''' <param name="strEventLog">The log that we're exporting.</param>
        Private Sub exportApplicationEventLogEntriesToFile(ByVal strEventLog As String, ByRef logEntries As List(Of restorePointCreatorExportedLog), ByRef logCount As Long)
            Dim eventLogQuery As Eventing.Reader.EventLogQuery
            Dim logReader As Eventing.Reader.EventLogReader
            Dim eventInstance As Eventing.Reader.EventRecord
            Dim logClass As restorePointCreatorExportedLog

            Try
                If EventLog.Exists(strEventLog) Then
                    eventLogQuery = New Eventing.Reader.EventLogQuery(strEventLog, Eventing.Reader.PathType.LogName)
                    logReader = New Eventing.Reader.EventLogReader(eventLogQuery)

                    eventInstance = logReader.ReadEvent()

                    While eventInstance IsNot Nothing
                        If eventInstance.ProviderName.Equals(strSystemRestorePointCreator, StringComparison.OrdinalIgnoreCase) Or eventInstance.ProviderName.caseInsensitiveContains(strSystemRestorePointCreator) = True Then
                            logClass = New restorePointCreatorExportedLog With {
                                .logData = "--== Imported Log ==--" & vbCrLf & vbCrLf & eventInstance.FormatDescription,
                                .unixTime = 0,
                                .logType = eventInstance.Level,
                                .logSource = strEventLog,
                                .logID = logCount,
                                .dateObject = eventInstance.TimeCreated.Value.ToUniversalTime,
                                .boolException = False
                            }

                            logCount += 1
                            logEntries.Add(logClass)
                            logClass = Nothing
                        End If

                        eventInstance.Dispose()
                        eventInstance = Nothing
                        eventInstance = logReader.ReadEvent()
                    End While

                    logReader.Dispose()
                    logReader = Nothing
                    eventLogQuery = Nothing
                End If
            Catch ex As Exception
                logEntries.Add(New restorePointCreatorExportedLog With {
                    .logData = assembleCrashData(ex, EventLogEntryType.Error),
                    .logType = EventLogEntryType.Error,
                    .unixTime = 0,
                    .logSource = "Restore Point Creator",
                    .logID = logEntries.Count,
                    .dateObject = Now.ToUniversalTime,
                    .boolException = True
                })
            End Try
        End Sub
    End Module
End Namespace