Namespace Functions.eventLogFunctions
    Module eventLogFunctions
        Private Const strSystemRestorePointCreator As String = "System Restore Point Creator"

        ''' <summary>Exports the application logs a file.</summary>
        ''' <param name="strLogFile">The path to the file we will be exporting the data to.</param>
        ''' <returns>Returns a Boolean value.</returns>
        Public Function exportLogsToFile(ByVal strLogFile As String, ByRef logCount As ULong) As Boolean
            Try
                Dim jsonEngine As New Web.Script.Serialization.JavaScriptSerializer

                If IO.File.Exists(strLogFile) Then IO.File.Delete(strLogFile)
                Dim fileHandle As New IO.StreamWriter(strLogFile, False, Text.Encoding.UTF8)

                fileHandle.WriteLine("// Export Data Version: 3")
                fileHandle.WriteLine("// Program Version: " & globalVariables.version.strFullVersionString)
                fileHandle.WriteLine("// Operating System: " & osVersionInfo.getFullOSVersionString)
                fileHandle.WriteLine("// --== End Header Information ==--")
                fileHandle.WriteLine("//")

                exportApplicationEventLogEntriesToFile(globalVariables.eventLog.strApplication, fileHandle, jsonEngine, logCount)
                exportApplicationEventLogEntriesToFile(globalVariables.eventLog.strSystemRestorePointCreator, fileHandle, jsonEngine, logCount)

                jsonEngine = Nothing
                fileHandle.Close()
                fileHandle.Dispose()

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

        ''' <summary>Writes the IO.FileLoadException event to the System Log File. This exception logger is different than the universal exception logger in the sense that it's written specifically to handle the Runtime.InteropServices.COMException type.</summary>
        ''' <param name="exceptionObject">The exception object.</param>
        ''' <example>functions.eventLogFunctions.writeCrashToEventLog(ex)</example>
        Public Sub writeCrashToEventLog(exceptionObject As Runtime.InteropServices.COMException)
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

                stringBuilder.AppendLine("Running As: " & Environment.UserName)
                stringBuilder.AppendLine("Exception Type: " & exceptionObject.GetType.ToString)

                Try
                    stringBuilder.AppendLine("Source: " & exceptionObject.Source)
                    stringBuilder.AppendLine("Error Code: " & exceptionObject.ErrorCode)

                    Try
                        stringBuilder.AppendLine("Additional COMException Data: " & support.jsonObject(exceptionObject.Data))
                    Catch ex As Exception ' We do nothing here.
                    End Try
                Catch ex As Exception
                End Try

                stringBuilder.AppendLine()
                stringBuilder.AppendLine("Message: " & support.removeSourceCodePathInfo(exceptionObject.Message))

                stringBuilder.AppendLine()

                stringBuilder.Append("The exception occurred ")

                stringBuilder.AppendLine(support.removeSourceCodePathInfo(exceptionObject.StackTrace.Trim))

                writeToSystemEventLog(stringBuilder.ToString.Trim, EventLogEntryType.Error)
                stringBuilder = Nothing
            Catch ex2 As Exception
                ' Does nothing
            End Try
        End Sub

        ''' <summary>Writes the IO.FileLoadException event to the System Log File. This exception logger is different than the universal exception logger in the sense that it's written specifically to handle the IO.FileNotFoundException type.</summary>
        ''' <param name="exceptionObject">The exception object.</param>
        ''' <example>functions.eventLogFunctions.writeCrashToEventLog(ex)</example>
        Public Sub writeCrashToEventLog(exceptionObject As IO.FileNotFoundException)
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

                stringBuilder.AppendLine("Running As: " & Environment.UserName)
                stringBuilder.AppendLine("Exception Type: " & exceptionObject.GetType.ToString)

                Try
                    stringBuilder.AppendLine("File that was not found: " & exceptionObject.FileName)
                    stringBuilder.AppendLine("Fusion Log: " & exceptionObject.FusionLog)

                    Try
                        stringBuilder.AppendLine("Additional FileNotFoundException Data: " & support.jsonObject(exceptionObject.Data))
                    Catch ex As Exception ' We do nothing here.
                    End Try
                Catch ex As Exception
                End Try

                stringBuilder.AppendLine()
                stringBuilder.AppendLine("Message: " & support.removeSourceCodePathInfo(exceptionObject.Message))

                stringBuilder.AppendLine()

                stringBuilder.Append("The exception occurred ")

                stringBuilder.AppendLine(support.removeSourceCodePathInfo(exceptionObject.StackTrace.Trim))

                writeToSystemEventLog(stringBuilder.ToString.Trim, EventLogEntryType.Error)
                stringBuilder = Nothing
            Catch ex2 As Exception
                ' Does nothing
            End Try
        End Sub

        ''' <summary>Writes the IO.FileLoadException event to the System Log File. This exception logger is different than the universal exception logger in the sense that it's written specifically to handle the IO.FileLoadException type.</summary>
        ''' <param name="exceptionObject">The exception object.</param>
        ''' <example>functions.eventLogFunctions.writeCrashToEventLog(ex)</example>
        Public Sub writeCrashToEventLog(exceptionObject As IO.FileLoadException)
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

                stringBuilder.AppendLine("Running As: " & Environment.UserName)
                stringBuilder.AppendLine("Exception Type: " & exceptionObject.GetType.ToString)

                Try
                    stringBuilder.AppendLine("Unable to Load Assembly File: " & exceptionObject.FileName)
                    stringBuilder.AppendLine("Fusion Log: " & exceptionObject.FusionLog)

                    Try
                        stringBuilder.AppendLine("Additional FileLoadException Data: " & support.jsonObject(exceptionObject.Data))
                    Catch ex As Exception ' We do nothing here.
                    End Try
                Catch ex As Exception
                End Try

                stringBuilder.AppendLine()
                stringBuilder.AppendLine("Message: " & support.removeSourceCodePathInfo(exceptionObject.Message))

                stringBuilder.AppendLine()

                stringBuilder.Append("The exception occurred ")

                stringBuilder.AppendLine(support.removeSourceCodePathInfo(exceptionObject.StackTrace.Trim))

                writeToSystemEventLog(stringBuilder.ToString.Trim, EventLogEntryType.Error)
                stringBuilder = Nothing
            Catch ex2 As Exception
                ' Does nothing
            End Try
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

        Private Sub exportApplicationEventLogEntriesToFile(ByVal strEventLog As String, ByRef fileHandle As IO.StreamWriter, ByRef jsonEngine As Web.Script.Serialization.JavaScriptSerializer, ByRef logCount As ULong)
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
                            fileHandle.WriteLine(jsonEngine.Serialize(logClass))
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