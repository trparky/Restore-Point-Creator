Namespace Functions.oldEventLogFunctions
    Module oldEventLogFunctions
        Public boolShowErrorMessage As Boolean = False

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

                    If Not EventLog.SourceExists(logSource, host) Then
                        EventLog.CreateEventSource(New EventSourceCreationData(logSource, logName))
                    End If

                    Dim eventLogObject As New EventLog(logName, host, logSource)
                    eventLogObject.WriteEntry(logMessage, logType, 234, CType(3, Short))

                    If boolShowErrorMessage Then MsgBox("The program was unable to write the regular application log file, the event data was written to the Windows Event Log as a backup. Better to have it written somewhere then to have the data go nowhere." & vbCrLf & vbCrLf & "Please see the Windows Event Log for more details.", MsgBoxStyle.Critical, "Restore Point Creator -- Unable to write to log file")

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
                writeToSystemEventLog(eventLogFunctions.assembleCrashData(exceptionObject, errorType), errorType)
            Catch ex2 As Exception
                ' Does nothing
            End Try
        End Sub
    End Module
End Namespace