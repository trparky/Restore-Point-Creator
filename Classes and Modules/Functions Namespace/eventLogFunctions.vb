Imports System.Runtime.InteropServices
Imports Microsoft.Win32

Namespace Functions.eventLogFunctions
    Module eventLogFunctions
        Private Const strSystemRestorePointCreator As String = "System Restore Point Creator"
        Private Const strRegistryApplicationPath As String = "SYSTEM\CurrentControlSet\services\eventlog\Application"

        Public strLogFile As String = IO.Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData), "Restore Point Creator.log")
        Private boolCachedCanIWriteThereResults As Boolean = privilegeChecks.canIWriteThere(Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData))
        Private applicationLog As List(Of restorePointCreatorExportedLog) = getLogObject()

        ''' <summary>Exports the application logs to a file.</summary>
        ''' <param name="strLogFile">The path to the file we will be exporting the data to.</param>
        ''' <param name="logCount">This is a ByRef argument which passes back the number of logs that this function exported.</param>
        ''' <returns>Returns a Boolean value. If True the logs were successfully exported, if False then something went wrong.</returns>
        Public Function exportLogsToFile(ByVal strLogFile As String, ByRef logCount As ULong) As Boolean
            Try
                Dim jsonEngine As New Web.Script.Serialization.JavaScriptSerializer

                Dim logObject As New exportedLogFile With {
                    .operatingSystem = osVersionInfo.getFullOSVersionString,
                    .programVersion = globalVariables.version.strFullVersionString,
                    .version = 4,
                    .logsEntries = getLogObject()
                }
                logCount = logObject.logsEntries.Count

                If IO.File.Exists(strLogFile) Then IO.File.Delete(strLogFile)

                Try
                    Using streamWriter As New IO.StreamWriter(strLogFile)
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
            Dim applicationLog As New List(Of restorePointCreatorExportedLog)
            Dim xmlSerializerObject As New Xml.Serialization.XmlSerializer(applicationLog.GetType)

            If IO.File.Exists(strLogFile) Then
                Using streamReader As New IO.StreamReader(strLogFile)
                    applicationLog = xmlSerializerObject.Deserialize(streamReader)
                End Using
            End If

            Return applicationLog
        End Function

        Public Sub getOldLogsFromWindowsEventLog()
            Dim registryKey As RegistryKey = Registry.LocalMachine.OpenSubKey(globalVariables.registryValues.strKey, True)
            Dim boolExportedOldLogs As Boolean = False
            Boolean.TryParse(registryKey.GetValue("Exported Old Logs", "False"), boolExportedOldLogs)

            If Not boolExportedOldLogs Then
                Try
                    writeToSystemEventLog("Starting log conversion process.", EventLogEntryType.Information)

                    Dim stopwatch As Stopwatch = Stopwatch.StartNew
                    Dim logCount As ULong = 0
                    exportApplicationEventLogEntriesToFile(globalVariables.eventLog.strApplication, applicationLog, logCount)
                    exportApplicationEventLogEntriesToFile(globalVariables.eventLog.strSystemRestorePointCreator, applicationLog, logCount)

                    Using streamWriter As New IO.StreamWriter(strLogFile)
                        Dim xmlSerializerObject As New Xml.Serialization.XmlSerializer(applicationLog.GetType)
                        xmlSerializerObject.Serialize(streamWriter, applicationLog)
                    End Using

                    writeToSystemEventLog("Log conversion process complete.", EventLogEntryType.Information)
                    writeToSystemEventLog(String.Format("Converted log data to new log file format in {0}ms.", stopwatch.ElapsedMilliseconds.ToString), EventLogEntryType.Information)
                Catch ex As Exception
                End Try

                registryKey.SetValue("Exported Old Logs", "True", RegistryValueKind.String)
            End If

            registryKey.Close()
            registryKey.Dispose()
        End Sub

        Public Sub saveLogFileToDisk()
            If boolCachedCanIWriteThereResults Then
                Using streamWriter As New IO.StreamWriter(strLogFile)
                    Dim xmlSerializerObject As New Xml.Serialization.XmlSerializer(applicationLog.GetType)
                    xmlSerializerObject.Serialize(streamWriter, applicationLog)
                End Using
            End If
        End Sub

        ''' <summary>Writes a log entry to the System Event Log.</summary>
        ''' <param name="logMessage">The text you want to have in your new System Event Log entry.</param>
        ''' <param name="logType">The type of log that you want your entry to be. The three major options are Error, Information, and Warning.</param>
        ''' <example>functions.eventLogFunctions.writeToSystemEventLog("My Event Log Entry", EventLogEntryType.Information)</example>
        Public Sub writeToSystemEventLog(logMessage As String, Optional logType As EventLogEntryType = EventLogEntryType.Information)
            If globalVariables.boolLogToSystemLog = True Then
                Try
                    applicationLog.Add(New restorePointCreatorExportedLog With {
                                       .logData = logMessage,
                                       .logType = logType,
                                       .unixTime = Now.ToUniversalTime.toUNIXTimestamp,
                                       .logSource = "Restore Point Creator",
                                       .logID = applicationLog.Count
                    })

                    saveLogFileToDisk()
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

                applicationLog.Add(New restorePointCreatorExportedLog With {
                                   .logData = stringBuilder.ToString.Trim,
                                   .logType = errorType,
                                   .unixTime = Now.ToUniversalTime.toUNIXTimestamp,
                                   .logSource = "Restore Point Creator",
                                   .logID = applicationLog.Count
                })
                saveLogFileToDisk()

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
                                .logData = eventInstance.FormatDescription,
                                .unixTime = eventInstance.TimeCreated.Value.ToUniversalTime.toUNIXTimestamp(),
                                .logType = eventInstance.Level,
                                .logSource = strEventLog,
                                .logID = logCount
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