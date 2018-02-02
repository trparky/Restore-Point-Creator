Namespace Functions.eventLogFunctions
    Module eventLogFunctions
        Private Const strSystemRestorePointCreator As String = "System Restore Point Creator"
        Private Const strRegistryApplicationPath As String = "SYSTEM\CurrentControlSet\services\eventlog\Application"

        Public strProgramDataDirectory As String = Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData)
        Public strLogFile As String = IO.Path.Combine(strProgramDataDirectory, "Restore Point Creator.log")
        Public strLogLockFile As String = IO.Path.Combine(strProgramDataDirectory, "Restore Point Creator.log.lock")
        Public strCorruptedLockFile As String = IO.Path.Combine(strProgramDataDirectory, "corruptedlog.lock")

        Private boolCachedCanIWriteThereResults As Boolean = privilegeChecks.canIWriteThere(strProgramDataDirectory)
        Private spinLockThread As Threading.Thread

        Private ReadOnly xmlSerializerObject As New Xml.Serialization.XmlSerializer((New List(Of restorePointCreatorExportedLog)).GetType)

        ''' <summary>Opens and returns a IO.FileStream.</summary>
        ''' <param name="strFileToOpen">Path to the file you want to open.</param>
        ''' <param name="strLockFile">Path to the lock file that indicates that the file declared using the "strFileToOpen" variable is in use.</param>
        ''' <param name="accessMethod">The way you want to access the file.</param>
        ''' <param name="fileMode">The way you want to access the file.</param>
        ''' <returns>A IO.FileStream Object.</returns>
        ''' <exception cref="myExceptions.unableToGetLockOnLogFile"></exception>
        Private Function getLogFileIOFileStream(strFileToOpen As String, strLockFile As String, accessMethod As IO.FileAccess, Optional fileMode As IO.FileMode = IO.FileMode.Open, Optional boolUseLockFile As Boolean = True) As IO.FileStream
            If boolUseLockFile Then
                If IO.File.Exists(strLockFile) Then
                    spinLockThread = New Threading.Thread(Sub()
                                                              Threading.Thread.Sleep(5000) ' Sleeps for 5 seconds.
                                                              support.deleteFileWithNoException(strLockFile)
                                                              Debug.WriteLine("Forcefully removed log lock file.")
                                                              spinLockThread = Nothing
                                                          End Sub)
                    spinLockThread.Name = "Log File Lock File Watcher"
                    spinLockThread.Priority = Threading.ThreadPriority.Normal
                    spinLockThread.IsBackground = True
                    spinLockThread.Start()
                End If

                While IO.File.Exists(strLockFile)
                    ' Spin locks.
                End While

                IO.File.Create(strLockFile).Dispose()
            End If

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
                Dim jsonEngine As New Web.Script.Serialization.JavaScriptSerializer

                Dim logObject As New exportedLogFile With {
                    .operatingSystem = osVersionInfo.getFullOSVersionString,
                    .programVersion = globalVariables.version.strFullVersionString,
                    .version = 5,
                    .logsEntries = getLogObject()
                }
                logCount = logObject.logsEntries.Count

                If IO.File.Exists(strFileToBeExportedTo) Then IO.File.Delete(strFileToBeExportedTo)

                Try
                    Using streamWriter As New IO.StreamWriter(strFileToBeExportedTo)
                        Dim resLogXSerializer As New Xml.Serialization.XmlSerializer(logObject.GetType)
                        resLogXSerializer.Serialize(streamWriter, logObject)
                    End Using
                Catch ex As Exception
                    writeCrashToEventLog(ex)
                End Try

                Return True
            Catch ex As Exception
                writeCrashToEventLog(ex)
                writeToApplicationLogFile("There was an error while attempting to export the program's event log entries.", EventLogEntryType.Error)

                Return False
            End Try
        End Function

        Public Function getLogObject() As List(Of restorePointCreatorExportedLog)
            Dim internalApplicationLog As New List(Of restorePointCreatorExportedLog)

            If IO.File.Exists(strLogFile) Then
                Try
                    Dim boolDidWeHaveACorruptedLogFile As Boolean = False

                    Using fileStream As IO.FileStream = getLogFileIOFileStream(strLogFile, strLogLockFile, IO.FileAccess.Read)
                        Using streamReader As New IO.StreamReader(fileStream)

                            If (New IO.FileInfo(strLogFile)).Length = 0 Then
                                internalApplicationLog.Add(New restorePointCreatorExportedLog With {
                                    .logData = "The log file was found to be empty.",
                                    .logType = EventLogEntryType.Error,
                                    .unixTime = 0,
                                    .logSource = "Restore Point Creator",
                                    .logID = internalApplicationLog.Count,
                                    .dateObject = Now.ToUniversalTime
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
                        Using fileStream As IO.FileStream = getLogFileIOFileStream(strLogFile, strLogLockFile, IO.FileAccess.Write, IO.FileMode.Create, False)
                            xmlSerializerObject.Serialize(fileStream, internalApplicationLog)
                        End Using
                    End If
                Catch ex As myExceptions.unableToGetLockOnLogFile
                    oldEventLogFunctions.boolShowErrorMessage = True
                    oldEventLogFunctions.writeCrashToEventLog(ex.innerIOException)
                Finally
                    support.deleteFileWithNoException(strLogLockFile)
                End Try
            End If

            Return internalApplicationLog
        End Function

        Public Sub getOldLogsFromWindowsEventLog()
            Try
                If Not IO.File.Exists(strLogFile) Then createLogFile()
                writeToApplicationLogFile("Starting log conversion process.", EventLogEntryType.Information)

                Dim applicationLog As New List(Of restorePointCreatorExportedLog)
                Dim logCount, longOldLogCount As Long
                Dim stopwatch As Stopwatch = Stopwatch.StartNew

                Try
                    Using fileStream As IO.FileStream = getLogFileIOFileStream(strLogFile, strLogLockFile, IO.FileAccess.ReadWrite)
                        Dim streamReader As New IO.StreamReader(fileStream)
                        applicationLog = xmlSerializerObject.Deserialize(streamReader)

                        logCount = applicationLog.Count
                        longOldLogCount = logCount

                        exportApplicationEventLogEntriesToFile(globalVariables.eventLog.strApplication, applicationLog, logCount)
                        exportApplicationEventLogEntriesToFile(globalVariables.eventLog.strSystemRestorePointCreator, applicationLog, logCount)

                        fileStream.Position = 0
                        fileStream.SetLength(0)

                        Dim streamWriter As New IO.StreamWriter(fileStream)
                        xmlSerializerObject.Serialize(streamWriter, applicationLog)

                        streamReader.Dispose()
                    End Using
                Catch ex As myExceptions.unableToGetLockOnLogFile
                    support.deleteFileWithNoException(strLogLockFile)
                    oldEventLogFunctions.boolShowErrorMessage = True
                    oldEventLogFunctions.writeCrashToEventLog(ex.innerIOException)
                    Exit Sub
                Finally
                    support.deleteFileWithNoException(strLogLockFile)
                End Try

                Dim longNumberOfImportedLogs As Long = applicationLog.Count - longOldLogCount

                writeToApplicationLogFile("Log conversion process complete.", EventLogEntryType.Information)

                If longNumberOfImportedLogs = 1 Then
                    writeToApplicationLogFile(String.Format("Converted log data to new log file format in {0}ms. 1 log entry was imported.", stopwatch.ElapsedMilliseconds.ToString), EventLogEntryType.Information)
                ElseIf longNumberOfImportedLogs > 1 Then
                    writeToApplicationLogFile(String.Format("Converted log data to new log file format in {0}ms. {1} log entries were imported.", stopwatch.ElapsedMilliseconds.ToString, longNumberOfImportedLogs.ToString("N0")), EventLogEntryType.Information)
                Else
                    writeToApplicationLogFile(String.Format("Converted log data to new log file format in {0}ms. No old log entries were detected.", stopwatch.ElapsedMilliseconds.ToString), EventLogEntryType.Information)
                End If
            Catch ex As Exception
                writeCrashToEventLog(ex)
            End Try
        End Sub

        Private Sub createLogFile()
            Try
                Dim applicationLog As New List(Of restorePointCreatorExportedLog)

                applicationLog.Add(New restorePointCreatorExportedLog With {
                    .logData = "Log file initialized.",
                    .logType = EventLogEntryType.Information,
                    .unixTime = 0,
                    .logSource = "Restore Point Creator",
                    .logID = applicationLog.Count,
                    .dateObject = Now.ToUniversalTime
                })

                Try
                    Using fileStream As IO.FileStream = getLogFileIOFileStream(strLogFile, strLogLockFile, IO.FileAccess.Write, IO.FileMode.Create)
                        Using streamWriter As New IO.StreamWriter(fileStream)
                            xmlSerializerObject.Serialize(streamWriter, applicationLog)
                        End Using
                    End Using
                Catch ex As myExceptions.unableToGetLockOnLogFile
                    support.deleteFileWithNoException(strLogLockFile)
                    oldEventLogFunctions.boolShowErrorMessage = True
                    oldEventLogFunctions.writeCrashToEventLog(ex.innerIOException)
                Finally
                    support.deleteFileWithNoException(strLogLockFile)
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
        Public Sub deleteEntryFromLog(idsOfLogsToBeDeleted As List(Of Long))
            Try
                Dim applicationLog As New List(Of restorePointCreatorExportedLog)

                If Not IO.File.Exists(strLogFile) Then createLogFile()

                Using fileStream As IO.FileStream = getLogFileIOFileStream(strLogFile, strLogLockFile, IO.FileAccess.ReadWrite)
                    Dim streamReader As New IO.StreamReader(fileStream)
                    applicationLog = xmlSerializerObject.Deserialize(streamReader)

                    For Each longIDToBeDeleted As Long In idsOfLogsToBeDeleted
                        deleteEntryFromLogSubRoutine(applicationLog, longIDToBeDeleted)
                    Next

                    applicationLog = reIDTheLogEntries(applicationLog)

                    fileStream.Position = 0
                    fileStream.SetLength(0)

                    Dim streamWriter As New IO.StreamWriter(fileStream)
                    xmlSerializerObject.Serialize(streamWriter, applicationLog)

                    streamReader.Dispose()
                End Using
            Catch ex As myExceptions.unableToGetLockOnLogFile
                oldEventLogFunctions.boolShowErrorMessage = True
                oldEventLogFunctions.writeCrashToEventLog(ex.innerIOException)
            Catch ex As Exception
                ' Does nothing.
            Finally
                support.deleteFileWithNoException(strLogLockFile)
            End Try
        End Sub

        ''' <summary>Deletes an individual log entry from the log.</summary>
        Public Sub deleteEntryFromLog(longIDToBeDeleted As Long)
            Try
                Dim applicationLog As New List(Of restorePointCreatorExportedLog)

                If Not IO.File.Exists(strLogFile) Then createLogFile()

                Using fileStream As IO.FileStream = getLogFileIOFileStream(strLogFile, strLogLockFile, IO.FileAccess.ReadWrite)
                    Dim streamReader As New IO.StreamReader(fileStream)
                    applicationLog = xmlSerializerObject.Deserialize(streamReader)

                    deleteEntryFromLogSubRoutine(applicationLog, longIDToBeDeleted)
                    applicationLog = reIDTheLogEntries(applicationLog)

                    fileStream.Position = 0
                    fileStream.SetLength(0)

                    Dim streamWriter As New IO.StreamWriter(fileStream)
                    xmlSerializerObject.Serialize(streamWriter, applicationLog)

                    streamReader.Dispose()
                End Using
            Catch ex As myExceptions.unableToGetLockOnLogFile
                oldEventLogFunctions.boolShowErrorMessage = True
                oldEventLogFunctions.writeCrashToEventLog(ex.innerIOException)
            Catch ex As Exception
                ' Does nothing.
            Finally
                support.deleteFileWithNoException(strLogLockFile)
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
                .dateObject = Now.ToUniversalTime
            })
        End Sub

        ''' <summary>Writes a log entry to the System Event Log.</summary>
        ''' <param name="logMessage">The text you want to have in your new System Event Log entry.</param>
        ''' <param name="eventLogType">The type of log that you want your entry to be. The three major options are Error, Information, and Warning.</param>
        ''' <example>functions.eventLogFunctions.writeToSystemEventLog("My Event Log Entry", EventLogEntryType.Information)</example>
        Public Sub writeToApplicationLogFile(logMessage As String, Optional eventLogType As EventLogEntryType = EventLogEntryType.Information)
            Try
                Dim applicationLog As New List(Of restorePointCreatorExportedLog)

                If Not IO.File.Exists(strLogFile) Then createLogFile()

                Using fileStream As IO.FileStream = getLogFileIOFileStream(strLogFile, strLogLockFile, IO.FileAccess.ReadWrite)
                    Dim streamReader As New IO.StreamReader(fileStream)

                    If (New IO.FileInfo(strLogFile)).Length = 0 Then
                        applicationLog.Add(New restorePointCreatorExportedLog With {
                            .logData = "The log file was found to be empty.",
                            .logType = EventLogEntryType.Error,
                            .unixTime = 0,
                            .logSource = "Restore Point Creator",
                            .logID = applicationLog.Count,
                            .dateObject = Now.ToUniversalTime
                        })
                    Else
                        Try
                            applicationLog = xmlSerializerObject.Deserialize(streamReader)
                        Catch ex As InvalidOperationException
                            handleCorruptedXMLLogFile(applicationLog, fileStream)
                        End Try
                    End If

                    applicationLog.Add(New restorePointCreatorExportedLog With {
                        .logData = logMessage,
                        .logType = eventLogType,
                        .unixTime = 0,
                        .logSource = "Restore Point Creator",
                        .logID = applicationLog.Count,
                        .dateObject = Now.ToUniversalTime
                    })

                    fileStream.Position = 0
                    fileStream.SetLength(0)

                    Dim streamWriter As New IO.StreamWriter(fileStream)
                    xmlSerializerObject.Serialize(streamWriter, applicationLog)

                    streamReader.Dispose()
                End Using
            Catch ex As myExceptions.unableToGetLockOnLogFile
                oldEventLogFunctions.writeToSystemEventLog(logMessage, eventLogType)
                oldEventLogFunctions.boolShowErrorMessage = True
                oldEventLogFunctions.writeCrashToEventLog(ex.innerIOException)
            Catch ex As Exception
                oldEventLogFunctions.writeCrashToEventLog(ex)
            Finally
                support.deleteFileWithNoException(strLogLockFile)
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
        Public Sub writeCrashToEventLog(exceptionObject As Exception, Optional errorType As EventLogEntryType = EventLogEntryType.Error)
            Try
                writeToApplicationLogFile(assembleCrashData(exceptionObject, errorType), errorType)
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
                                .dateObject = eventInstance.TimeCreated.Value.ToUniversalTime
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
                    .dateObject = Now.ToUniversalTime
                })
            End Try
        End Sub
    End Module
End Namespace