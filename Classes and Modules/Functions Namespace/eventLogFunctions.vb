Imports System.Runtime.InteropServices
Imports Microsoft.Win32

Namespace Functions.eventLogFunctions
    Module eventLogFunctions
        Private Const strSystemRestorePointCreator As String = "System Restore Point Creator"
        Private Const strRegistryApplicationPath As String = "SYSTEM\CurrentControlSet\services\eventlog\Application"

        Public strLogFile As String = IO.Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData), "Restore Point Creator.log")
        Private boolCachedCanIWriteThereResults As Boolean = privilegeChecks.canIWriteThere(Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData))
        Private shortNumberOfRecalledGetFileStreamFunction As Short = 0

        ''' <summary>Opens and returns a IO.FileStream.</summary>
        ''' <param name="strFileToOpen">Path to the file you want to open.</param>
        ''' <param name="accessMethod">The way you want to access the file.</param>
        ''' <param name="fileMode">The way you want to access the file.</param>
        ''' <returns>A IO.FileStream Object.</returns>
        ''' <exception cref="myExceptions.unableToGetLockOnLogFile"></exception>
        Private Function getFileStreamWithWaiting(strFileToOpen As String, accessMethod As IO.FileAccess, Optional fileMode As IO.FileMode = IO.FileMode.Open) As IO.FileStream
            Try
                Return New IO.FileStream(strFileToOpen, fileMode, accessMethod, IO.FileShare.None)
            Catch ex As StackOverflowException
                Throw New myExceptions.unableToGetLockOnLogFile() With {.innerIOException = ex}
            Catch ex As IO.IOException
                If shortNumberOfRecalledGetFileStreamFunction.Equals(20) Then
                    Throw New myExceptions.unableToGetLockOnLogFile() With {.innerIOException = ex}
                End If

                Debug.WriteLine("File is locked, waiting 100ms to get a lock on the file.")
                Threading.Thread.Sleep(100)
                shortNumberOfRecalledGetFileStreamFunction += 1
                Return getFileStreamWithWaiting(strFileToOpen, accessMethod)
            End Try
        End Function

        ''' <summary>Exports the application logs to a file.</summary>
        ''' <param name="strFileToBeExportedTo">The path to the file we will be exporting the data to.</param>
        ''' <param name="logCount">This is a ByRef argument which passes back the number of logs that this function exported.</param>
        ''' <returns>Returns a Boolean value. If True the logs were successfully exported, if False then something went wrong.</returns>
        Public Function exportLogsToFile(ByVal strFileToBeExportedTo As String, ByRef logCount As ULong) As Boolean
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
                        Dim xmlSerializerObject As New Xml.Serialization.XmlSerializer(logObject.GetType)
                        xmlSerializerObject.Serialize(streamWriter, logObject)
                    End Using
                Catch ex As Exception
                    writeCrashToEventLog(ex)
                End Try

                Return True
            Catch ex As Exception
                writeCrashToEventLog(ex)
                writeToSystemEventLog("There was an error while attempting to export the program's event log entries.", EventLogEntryType.Error)

                Return False
            End Try
        End Function

        Public Function getLogObject() As List(Of restorePointCreatorExportedLog)
            Dim internalApplicationLog As New List(Of restorePointCreatorExportedLog)

            If IO.File.Exists(strLogFile) Then
                Try
                    shortNumberOfRecalledGetFileStreamFunction = 0
                    Using fileStream As IO.FileStream = getFileStreamWithWaiting(strLogFile, IO.FileAccess.Read)
                        Using streamReader As New IO.StreamReader(fileStream)
                            Dim xmlSerializerObject As New Xml.Serialization.XmlSerializer(internalApplicationLog.GetType)
                            internalApplicationLog = xmlSerializerObject.Deserialize(streamReader)
                        End Using
                    End Using
                Catch ex As myExceptions.unableToGetLockOnLogFile
                    oldEventLogFunctions.boolShowErrorMessage = True
                    oldEventLogFunctions.writeCrashToEventLog(ex.innerIOException)
                End Try
            End If

            Return internalApplicationLog
        End Function

        Public Sub getOldLogsFromWindowsEventLog()
            Try
                If Not IO.File.Exists(strLogFile) Then createLogFile()
                writeToSystemEventLog("Starting log conversion process.", EventLogEntryType.Information)

                Dim applicationLog As New List(Of restorePointCreatorExportedLog)
                Dim xmlSerializerObject As New Xml.Serialization.XmlSerializer(applicationLog.GetType)
                Dim logCount, longOldLogCount As ULong
                Dim stopwatch As Stopwatch = Stopwatch.StartNew

                Try
                    shortNumberOfRecalledGetFileStreamFunction = 0
                    Using fileStream As IO.FileStream = getFileStreamWithWaiting(strLogFile, IO.FileAccess.ReadWrite)
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
                    oldEventLogFunctions.boolShowErrorMessage = True
                    oldEventLogFunctions.writeCrashToEventLog(ex.innerIOException)
                    Exit Sub
                End Try

                Dim longNumberOfImportedLogs As ULong = applicationLog.Count - longOldLogCount

                writeToSystemEventLog("Log conversion process complete.", EventLogEntryType.Information)

                If longNumberOfImportedLogs = 1 Then
                    writeToSystemEventLog(String.Format("Converted log data to new log file format in {0}ms. 1 log entry was imported.", stopwatch.ElapsedMilliseconds.ToString), EventLogEntryType.Information)
                ElseIf longNumberOfImportedLogs > 1 Then
                    writeToSystemEventLog(String.Format("Converted log data to new log file format in {0}ms. {1} log entries were imported.", stopwatch.ElapsedMilliseconds.ToString, longNumberOfImportedLogs.ToString("N0")), EventLogEntryType.Information)
                Else
                    writeToSystemEventLog(String.Format("Converted log data to new log file format in {0}ms. No old log entries were detected.", stopwatch.ElapsedMilliseconds.ToString), EventLogEntryType.Information)
                End If
            Catch ex As Exception
            Finally
                registryStuff.setBooleanValueInRegistry("Exported Old Logs", True)
            End Try
        End Sub

        Private Sub createLogFile()
            Try
                Dim applicationLog As New List(Of restorePointCreatorExportedLog)
                Dim xmlSerializerObject As New Xml.Serialization.XmlSerializer(applicationLog.GetType)

                applicationLog.Add(New restorePointCreatorExportedLog With {
                    .logData = "Log file initialized.",
                    .logType = EventLogEntryType.Information,
                    .unixTime = 0,
                    .logSource = "Restore Point Creator",
                    .logID = applicationLog.Count,
                    .dateObject = Now.ToUniversalTime
                })

                Try
                    shortNumberOfRecalledGetFileStreamFunction = 0
                    Using fileStream As IO.FileStream = getFileStreamWithWaiting(strLogFile, IO.FileAccess.Write, IO.FileMode.Create)
                        Using streamWriter As New IO.StreamWriter(fileStream)
                            xmlSerializerObject.Serialize(streamWriter, applicationLog)
                        End Using
                    End Using
                Catch ex As myExceptions.unableToGetLockOnLogFile
                    oldEventLogFunctions.boolShowErrorMessage = True
                    oldEventLogFunctions.writeCrashToEventLog(ex.innerIOException)
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

        ''' <summary>Deletes a list of individual log entries from the log.</summary>
        Public Sub deleteEntryFromLog(idsOfLogsToBeDeleted As List(Of Long))
            If globalVariables.boolLogToSystemLog = True Then
                Try
                    Dim applicationLog As New List(Of restorePointCreatorExportedLog)
                    Dim xmlSerializerObject As New Xml.Serialization.XmlSerializer(applicationLog.GetType)

                    If Not IO.File.Exists(strLogFile) Then createLogFile()

                    shortNumberOfRecalledGetFileStreamFunction = 0
                    Using fileStream As IO.FileStream = getFileStreamWithWaiting(strLogFile, IO.FileAccess.ReadWrite)
                        Dim streamReader As New IO.StreamReader(fileStream)
                        applicationLog = xmlSerializerObject.Deserialize(streamReader)

                        For Each longIDToBeDeleted As Long In idsOfLogsToBeDeleted
                            For Each item As restorePointCreatorExportedLog In applicationLog.Where(Function(logObject As restorePointCreatorExportedLog) (logObject.logID = longIDToBeDeleted)).ToList()
                                applicationLog.Remove(item)
                            Next
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
                    ' Does nothing
                End Try
            End If
        End Sub

        ''' <summary>Deletes an individual log entry from the log.</summary>
        Public Sub deleteEntryFromLog(longIDToBeDeleted As Long)
            If globalVariables.boolLogToSystemLog = True Then
                Try
                    Dim applicationLog As New List(Of restorePointCreatorExportedLog)
                    Dim xmlSerializerObject As New Xml.Serialization.XmlSerializer(applicationLog.GetType)

                    If Not IO.File.Exists(strLogFile) Then createLogFile()

                    shortNumberOfRecalledGetFileStreamFunction = 0
                    Using fileStream As IO.FileStream = getFileStreamWithWaiting(strLogFile, IO.FileAccess.ReadWrite)
                        Dim streamReader As New IO.StreamReader(fileStream)
                        applicationLog = xmlSerializerObject.Deserialize(streamReader)

                        For Each item As restorePointCreatorExportedLog In applicationLog.Where(Function(logObject As restorePointCreatorExportedLog) (logObject.logID = longIDToBeDeleted)).ToList()
                            applicationLog.Remove(item)
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
                    ' Does nothing
                End Try
            End If
        End Sub

        ''' <summary>Writes a log entry to the System Event Log.</summary>
        ''' <param name="logMessage">The text you want to have in your new System Event Log entry.</param>
        ''' <param name="logType">The type of log that you want your entry to be. The three major options are Error, Information, and Warning.</param>
        ''' <example>functions.eventLogFunctions.writeToSystemEventLog("My Event Log Entry", EventLogEntryType.Information)</example>
        Public Sub writeToSystemEventLog(logMessage As String, Optional logType As EventLogEntryType = EventLogEntryType.Information)
            If globalVariables.boolLogToSystemLog = True Then
                Try
                    Dim applicationLog As New List(Of restorePointCreatorExportedLog)
                    Dim xmlSerializerObject As New Xml.Serialization.XmlSerializer(applicationLog.GetType)

                    If Not IO.File.Exists(strLogFile) Then createLogFile()

                    shortNumberOfRecalledGetFileStreamFunction = 0
                    Using fileStream As IO.FileStream = getFileStreamWithWaiting(strLogFile, IO.FileAccess.ReadWrite)
                        Dim streamReader As New IO.StreamReader(fileStream)
                        applicationLog = xmlSerializerObject.Deserialize(streamReader)

                        applicationLog.Add(New restorePointCreatorExportedLog With {
                            .logData = logMessage,
                            .logType = logType,
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
                    oldEventLogFunctions.writeToSystemEventLog(logMessage, logType)
                    oldEventLogFunctions.boolShowErrorMessage = True
                    oldEventLogFunctions.writeCrashToEventLog(ex.innerIOException)
                Catch ex As Exception
                    ' Does nothing
                End Try
            End If
        End Sub

        ''' <summary>Writes the exception event to the System Log File. This is a universal exception logging function that's built to handle various forms of exceptions and not not any particular type.</summary>
        ''' <param name="exceptionObject">The exception object.</param>
        ''' <param name="errorType">The type of Event Log you want the Exception Event to be recorded to the Application Event Log as.</param>
        ''' <example>functions.eventLogFunctions.writeCrashToEventLog(ex)</example>
        Public Sub writeCrashToEventLog(exceptionObject As Exception, Optional errorType As EventLogEntryType = EventLogEntryType.Error)
            Try
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

                stringBuilder.AppendLine("Running As: " & Environment.UserName)
                stringBuilder.AppendLine("Exception Type: " & exceptionObject.GetType.ToString)
                stringBuilder.AppendLine("Message: " & support.removeSourceCodePathInfo(exceptionObject.Message))

                stringBuilder.AppendLine()

                stringBuilder.Append("The exception occurred ")

                stringBuilder.AppendLine(support.removeSourceCodePathInfo(exceptionObject.StackTrace.Trim))

                writeToSystemEventLog(stringBuilder.ToString.Trim, errorType)

                stringBuilder = Nothing
            Catch ex2 As Exception
                ' Does nothing
            End Try
        End Sub

        ''' <summary>This sub-routine is called by the exportLogsToFile() sub-routine, this is not intended to be called outside of this module.</summary>
        ''' <param name="logCount">This is a ByRef argument which passes back the number of logs that this function exported.</param>
        ''' <param name="strEventLog">The log that we're exporting.</param>
        Private Sub exportApplicationEventLogEntriesToFile(ByVal strEventLog As String, ByRef logEntries As List(Of restorePointCreatorExportedLog), ByRef logCount As ULong)
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
                writeCrashToEventLog(ex)
            End Try
        End Sub
    End Module
End Namespace