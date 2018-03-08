Imports System.Text.RegularExpressions

Namespace Functions.restorePointStuff
    Module restorePointStuff
        Public Enum RestoreType
            ApplicationInstall = 0 ' Installing a new application
            ApplicationUninstall = 1 ' An application has been uninstalled
            ModifySettings = 12 ' An application has had features added or removed
            CancelledOperation = 13 ' An application needs to delete the restore point it created
            Restore = 6 ' System Restore
            Checkpoint = 7 ' Checkpoint
            DeviceDriverInstall = 10 ' Device driver has been installed
            FirstRun = 11 ' Program used for 1st time
            BackupRecovery = 14 ' Restoring a backup
            WindowsType = 16 ' The type of restore point that Windows makes.
        End Enum

        ''' <summary>A function that returns the type of restore point depending upon the Integer that comes from the WMI.</summary>
        ''' <param name="strType">String.</param>
        ''' <returns>Returns a String value containing the type of Restore Point in English.</returns>
        Public Function whatTypeOfRestorePointIsIt(strType As Integer) As String
            Dim intType As Integer
            If Integer.TryParse(strType, intType) Then
                If intType = RestoreType.ApplicationInstall Then
                    Return "Application Install"
                ElseIf intType = RestoreType.ApplicationUninstall Then
                    Return "Application Removal"
                ElseIf intType = RestoreType.BackupRecovery Then
                    Return "Backup Recovery"
                ElseIf intType = RestoreType.CancelledOperation Then
                    Return "Cancelled Operation"
                ElseIf intType = RestoreType.Checkpoint Then
                    Return "System Checkpoint"
                ElseIf intType = RestoreType.DeviceDriverInstall Then
                    Return "Device Driver Install"
                ElseIf intType = RestoreType.FirstRun Then
                    Return "First Run"
                ElseIf intType = RestoreType.ModifySettings Then
                    Return "Settings Modified"
                ElseIf intType = RestoreType.Restore Then
                    Return "Restore"
                ElseIf intType = RestoreType.WindowsType Then
                    Return "System Restore Point"
                Else
                    Return "Unknown Type"
                End If
            Else
                Return "Unknown Type"
            End If
        End Function

        Public Function parseSystemRestorePointCreationDate(strDate As String, Optional boolFullDateParsing As Boolean = True) As Date
            Dim regexMatches As Match = globalVariables.regexRestorePointCreationTimeParser.Match(strDate)
            Dim year, month, day, second, minute, hour As Integer

            ' Gets the values out of the Regular Expression Matches object.
            With regexMatches
                year = Integer.Parse(.Groups("year").Value)
                month = Integer.Parse(.Groups("month").Value)
                day = Integer.Parse(.Groups("day").Value)
                second = Integer.Parse(.Groups("second").Value)
                minute = Integer.Parse(.Groups("minute").Value)
                hour = Integer.Parse(.Groups("hour").Value)
            End With

            If boolFullDateParsing Then
                Return New DateTime(year, month, day, hour, minute, second, DateTimeKind.Utc).ToLocalTime
            Else
                Return New Date(year, month, day)
            End If
        End Function

        ''' <summary>Returns the creation date of the System Restore Point ID you pass to the function.</summary>
        ''' <param name="restorePointID">Data type, Short. The ID of the System Restore Point you want to return the creation date of.</param>
        ''' <returns>A Date Object.</returns>
        Private Function getSystemRestorePointDate(restorePointID As Short) As Date
            Try
                Dim newestSystemRestoreID As Integer = 0 ' Resets the newest System Restore ID to 0.

                ' Get all System Restore Points from the Windows Management System and puts then in the systemRestorePoints variable.
                Dim systemRestorePoints As New Management.ManagementObjectSearcher("root\DEFAULT", "SELECT * FROM SystemRestore WHERE SequenceNumber = " & restorePointID)

                If systemRestorePoints.Get().Count = 0 Then
                    Return Nothing
                Else
                    Return parseSystemRestorePointCreationDate(systemRestorePoints.Get(0)("CreationTime").ToString, True)
                End If
            Catch ex As Exception
                Threading.Thread.CurrentThread.CurrentUICulture = New Globalization.CultureInfo("en-US")
                exceptionHandler.manuallyLoadCrashWindow(ex, ex.Message, ex.StackTrace, ex.GetType)
                Return Nothing
            End Try
        End Function

        Public Sub createScheduledSystemRestorePoint(strRestorePointDescription As String)
            Try
                Dim integerNewRestorePointID As Integer
                wmi.createRestorePoint(strRestorePointDescription, RestoreType.WindowsType, integerNewRestorePointID)
            Catch ex As Exception
                eventLogFunctions.writeCrashToApplicationLogFile(ex)
                Process.GetCurrentProcess.Kill()
            End Try
        End Sub

        Public Sub writeSystemRestorePointsToApplicationLogs()
            Dim restorePoints As New Text.StringBuilder
            Dim systemRestorePointsManagementObjectSearcher As New Management.ManagementObjectSearcher("root\DEFAULT", "SELECT * FROM SystemRestore")
            Dim restorePointCreationDate As Date

            If systemRestorePointsManagementObjectSearcher IsNot Nothing Then
                Dim restorePointsOnSystemManagementObjectCollection As Management.ManagementObjectCollection = systemRestorePointsManagementObjectSearcher.Get()

                If restorePointsOnSystemManagementObjectCollection IsNot Nothing Then
                    If restorePointsOnSystemManagementObjectCollection.Count <> 0 Then
                        restorePoints.AppendLine("Number of Restore Points: " & restorePointsOnSystemManagementObjectCollection.Count)
                        restorePoints.AppendLine("=========================")
                        For Each restorePointDetails As Management.ManagementObject In restorePointsOnSystemManagementObjectCollection
                            If (restorePointDetails("SequenceNumber") IsNot Nothing) And (restorePointDetails("CreationTime") IsNot Nothing) And (restorePointDetails("Description") IsNot Nothing) And (restorePointDetails("RestorePointType") IsNot Nothing) Then
                                restorePoints.Append(restorePointDetails("SequenceNumber").ToString & " | " & restorePointDetails("Description").ToString & " | ")

                                If String.IsNullOrEmpty(restorePointDetails("CreationTime").ToString.Trim) Then
                                    restorePoints.Append("Error Parsing Date")
                                Else
                                    restorePointCreationDate = parseSystemRestorePointCreationDate(restorePointDetails("CreationTime").ToString)
                                    restorePoints.Append(String.Format("{0} {1}", restorePointCreationDate.ToShortDateString, restorePointCreationDate.ToLongTimeString))
                                End If

                                restorePoints.Append(" | " & whatTypeOfRestorePointIsIt(Integer.Parse(restorePointDetails("RestorePointType").ToString)))
                                restorePoints.AppendLine()
                            End If
                        Next
                    End If
                End If
            End If

            systemRestorePointsManagementObjectSearcher.Dispose()
            systemRestorePointsManagementObjectSearcher = Nothing

            restorePoints.AppendLine("=========================")
            eventLogFunctions.writeToApplicationLogFile(restorePoints.ToString.Trim, EventLogEntryType.Information, False, False)
        End Sub
    End Module
End Namespace