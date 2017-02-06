Namespace Functions.eventLogFunctions
    Module eventLogFunctions
        Private Const strSystemRestorePointCreator As String = "System Restore Point Creator"

        ''' <summary>Exports the application logs to a file.</summary>
        ''' <param name="strLogFile">The path to the file we will be exporting the data to.</param>
        ''' <param name="logCount">This is a ByRef argument which passes back the number of logs that this function exported.</param>
        ''' <returns>Returns a Boolean value. If True the logs were successfully exported, if False then something went wrong.</returns>
        Public Function exportLogsToFile(ByVal strLogFile As String, ByRef logCount As ULong) As Boolean
            Try
                Dim jsonEngine As New Web.Script.Serialization.JavaScriptSerializer

                Dim logObject As New exportedLogFile
                logObject.operatingSystem = osVersionInfo.getFullOSVersionString
                logObject.programVersion = globalVariables.version.strFullVersionString
                logObject.version = 4

                Dim logsEntries As New List(Of restorePointCreatorExportedLog)()

                If IO.File.Exists(strLogFile) Then IO.File.Delete(strLogFile)

                exportApplicationEventLogEntriesToFile(globalVariables.eventLog.strApplication, logsEntries, logCount)
                exportApplicationEventLogEntriesToFile(globalVariables.eventLog.strSystemRestorePointCreator, logsEntries, logCount)

                logObject.logsEntries = logsEntries

                Try
                    Dim streamWriter As New IO.StreamWriter(strLogFile)
                    Dim xmlSerializerObject As New Xml.Serialization.XmlSerializer(logObject.GetType)
                    xmlSerializerObject.Serialize(streamWriter, logObject)
                    streamWriter.Close()
                    streamWriter.Dispose()
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

        ''' <summary>Writes a log entry to the System Event Log.</summary>
        ''' <param name="logMessage">The text you want to have in your new System Event Log entry.</param>
        ''' <param name="logType">The type of log that you want your entry to be. The three major options are Error, Information, and Warning.</param>
        ''' <example>functions.eventLogFunctions.writeToSystemEventLog("My Event Log Entry", EventLogEntryType.Information)</example>
        Public Sub writeToSystemEventLog(logMessage As String, Optional logType As EventLogEntryType = EventLogEntryType.Information)
            If globalVariables.boolLogToSystemLog = True Then
                Try
                    Dim logSource As String = "System Restore Point Creator"
                    Dim logName As String = "Application"
                    Dim host As String = "."

                    If Microsoft.Win32.Registry.LocalMachine.OpenSubKey("SYSTEM\CurrentControlSet\services\eventlog\Application\" & logSource) Is Nothing Then
                        Microsoft.Win32.Registry.LocalMachine.OpenSubKey("SYSTEM\CurrentControlSet\services\eventlog\Application", True).CreateSubKey(logSource)
                        Microsoft.Win32.Registry.LocalMachine.OpenSubKey("SYSTEM\CurrentControlSet\services\eventlog\Application\" & logSource, True).SetValue("EventMessageFile", IO.Path.Combine(Runtime.InteropServices.RuntimeEnvironment.GetRuntimeDirectory(), "EventLogMessages.dll"), Microsoft.Win32.RegistryValueKind.String)
                    Else
                        If Microsoft.Win32.Registry.LocalMachine.OpenSubKey("SYSTEM\CurrentControlSet\services\eventlog\Application\" & logSource, False).GetValue("EventMessageFile", Nothing) Is Nothing Then
                            Microsoft.Win32.Registry.LocalMachine.OpenSubKey("SYSTEM\CurrentControlSet\services\eventlog\Application\" & logSource, True).SetValue("EventMessageFile", IO.Path.Combine(Runtime.InteropServices.RuntimeEnvironment.GetRuntimeDirectory(), "EventLogMessages.dll"), Microsoft.Win32.RegistryValueKind.String)
                        End If
                    End If

                    If Not EventLog.SourceExists(logSource, host) Then
                        EventLog.CreateEventSource(New EventSourceCreationData(logSource, logName))
                    End If

                    Dim eventLogObject As New EventLog(logName, host, logSource)
                    eventLogObject.WriteEntry(logMessage, logType, 234, CType(3, Short))

                    eventLogObject.Dispose()
                    eventLogObject = Nothing
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
                stringBuilder.AppendLine("System RAM: " & wmi.getSystemRAM())

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

            Try
                If EventLog.Exists(strEventLog) Then
                    eventLogQuery = New Eventing.Reader.EventLogQuery(strEventLog, Eventing.Reader.PathType.LogName)
                    logReader = New Eventing.Reader.EventLogReader(eventLogQuery)

                    eventInstance = logReader.ReadEvent()

                    While eventInstance IsNot Nothing
                        If eventInstance.ProviderName = strSystemRestorePointCreator Or eventInstance.ProviderName.caseInsensitiveContains(strSystemRestorePointCreator) = True Then
                            Dim logClass As New restorePointCreatorExportedLog
                            logClass.logData = eventInstance.FormatDescription
                            logClass.unixTime = eventInstance.TimeCreated.Value.ToUniversalTime.toUNIXTimestamp()
                            logClass.logType = eventInstance.Level
                            logClass.logSource = strEventLog
                            logClass.logID = eventInstance.RecordId

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