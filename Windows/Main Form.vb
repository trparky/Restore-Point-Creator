#Region "--== Imported Namespaces ==--"
Imports Microsoft.Win32
Imports System.Text.RegularExpressions
Imports System.Text
Imports System.Runtime.InteropServices
Imports System.Management
Imports ICSharpCode.SharpZipLib.Zip
#End Region

Public Class Form1
#Region "--== Program-Wide Variables ==--"
    Private boolDoneLoading As Boolean = False
    Private boolShowDonationMessage As Boolean = True

    Private newestSystemRestoreID As Integer
    Private m_SortingColumn As ColumnHeader

    Private Const strTypeYourRestorePointName As String = "Type in a name for your custom-named System Restore Point and press Enter..."
    Private Const strMessageBoxTitle As String = "System Restore Point Creator"
    Public defaultCustomRestorePointName As String
    Private boolHaveWeTriedToCreateTheRestorePointAgain As Boolean = False

    'Private restorePointDateData As New Dictionary(Of String, String)
#End Region

#Region "--== Timers ==--"
    Private Sub Timer1_Tick(sender As Object, e As EventArgs) Handles Timer1.Tick
        Try
            'AppActivate(Me.Text)
            AppActivate(Process.GetCurrentProcess.Id)
            Me.BringToFront()
            Me.Focus()
            Timer1.Enabled = False
        Catch ex As Exception
            ' Does nothing
        End Try
    End Sub
#End Region

#Region "--== Form Load Event Sub-Routines ==--"
    Private Sub verifyUpdateChannel()
        If Not My.Settings.updateChannel.Equals(globalVariables.updateChannels.stable) And
           Not My.Settings.updateChannel.Equals(globalVariables.updateChannels.beta) And
           Not My.Settings.updateChannel.Equals(globalVariables.updateChannels.tom) Then
            Functions.eventLogFunctions.writeToSystemEventLog(String.Format("An invalid update channel was detected. The update channel was previously set to ""{0}"". This has been auto-corrected by setting it back to the ""stable"" update channel.", My.Settings.updateChannel), EventLogEntryType.Warning)
            My.Settings.updateChannel = globalVariables.updateChannels.stable
        End If
    End Sub

    Private Sub checkForMyComputerRightClickOption()
        Try
            Dim valueInRegistry As String
            Dim iconPath As String
            Dim registryKey As RegistryKey
            Dim matches As Match
            Dim registryRootKeyWeAreWorkingWith As RegistryKey

            If Functions.osVersionInfo.isThisWindows10() Then
                ' In Windows 10 we can only write to the CLSID Classes Root that is a subkey of the current user's registry hive.
                registryRootKeyWeAreWorkingWith = Registry.CurrentUser.OpenSubKey("SOFTWARE\Classes", True)
            Else
                registryRootKeyWeAreWorkingWith = Registry.ClassesRoot
            End If

            ' This checks to see if we need to rename the Registry key for the "My Computer" right-click context menu.
            ' First we check to see if "Create Custom Named System Restore" exists, if it does then we go onto checking if
            ' "Create Custom Named System Restore Point" doesn't exist.  If both "Create Custom Named System Restore" exists
            ' AND "Create Custom Named System Restore Point" doesn't exist, then we know that we have to rename the
            ' "Create Custom Named System Restore" to "Create Custom Named System Restore Point".
            Try
                If (registryRootKeyWeAreWorkingWith.OpenSubKey("CLSID\{20D04FE0-3AEA-1069-A2D8-08002B30309D}\Shell\Create Custom Named System Restore", False) Is Nothing) = False And (registryRootKeyWeAreWorkingWith.OpenSubKey("CLSID\{20D04FE0-3AEA-1069-A2D8-08002B30309D}\Shell\Create Custom Named System Restore Point", False) Is Nothing) = True Then
                    'debug.writeline("renaming registry key")
                    Functions.registryStuff.renameRegistrySubKey(registryRootKeyWeAreWorkingWith, "CLSID\{20D04FE0-3AEA-1069-A2D8-08002B30309D}\Shell\Create Custom Named System Restore", "CLSID\{20D04FE0-3AEA-1069-A2D8-08002B30309D}\Shell\Create Custom Named System Restore Point")
                End If
            Catch ex As Exception
            End Try

            If registryRootKeyWeAreWorkingWith.OpenSubKey("CLSID\{20D04FE0-3AEA-1069-A2D8-08002B30309D}\Shell\Create System Restore Checkpoint", False) IsNot Nothing Then
                toolStripMyComputer.Checked = True

                If registryRootKeyWeAreWorkingWith.OpenSubKey("CLSID\{20D04FE0-3AEA-1069-A2D8-08002B30309D}\Shell\Create System Restore Checkpoint\command", False) IsNot Nothing Then
                    valueInRegistry = registryRootKeyWeAreWorkingWith.OpenSubKey("CLSID\{20D04FE0-3AEA-1069-A2D8-08002B30309D}\Shell\Create System Restore Checkpoint\command", False).GetValue(vbNullString, Nothing)

                    If valueInRegistry IsNot Nothing Then
                        ' We check if the current Registry path is different than the current process's EXE path.
                        If valueInRegistry.caseInsensitiveContains(Application.ExecutablePath) = False Then
                            ' OK, it doesn't match the current process's EXE path.

                            ' We parse out the EXE's path out of the combined path with the argument.
                            matches = Regex.Match(valueInRegistry, "((?:""|'){0,1}[A-Za-z]:\\.*\.(?:bat|bin|cmd|com|cpl|exe|gadget|inf1|ins|inx|isu|job|jse|lnk|msc|msi|msp|mst|paf|pif|ps1|reg|rgs|sct|shb|shs|u3p|vb|vbe|vbs|vbscript|ws|wsf)(?:""|'){0,1} {0,1})(.*)", RegexOptions.IgnoreCase)

                            If matches IsNot Nothing Then
                                ' Now we make sure that the file exists.
                                If IO.File.Exists(matches.Groups(1).Value.Replace(Chr(34), "").Trim) = False Then
                                    '  OK, it doesn't.  The entries in the Registry are invalid, now let's fix them.
                                    registryKey = registryRootKeyWeAreWorkingWith.OpenSubKey("CLSID\{20D04FE0-3AEA-1069-A2D8-08002B30309D}\Shell\Create System Restore Checkpoint\Command", True)

                                    If registryKey IsNot Nothing Then ' This should prevent Null Reference Exceptions.
                                        registryKey.SetValue(vbNullString, String.Format("{0}{1}{0} -createrestorepoint", Chr(34), Application.ExecutablePath))
                                        registryKey.SetValue("icon", String.Format("{0}{1}{0}", Chr(34), Application.ExecutablePath), RegistryValueKind.String)
                                        registryKey.Close()
                                        registryKey = Nothing
                                    End If
                                End If
                            End If

                            matches = Nothing
                        End If
                    End If
                End If

                matches = Nothing
                valueInRegistry = Nothing

                If registryRootKeyWeAreWorkingWith.OpenSubKey("CLSID\{20D04FE0-3AEA-1069-A2D8-08002B30309D}\Shell\Create Custom Named System Restore Point\command", False) IsNot Nothing Then
                    valueInRegistry = registryRootKeyWeAreWorkingWith.OpenSubKey("CLSID\{20D04FE0-3AEA-1069-A2D8-08002B30309D}\Shell\Create Custom Named System Restore Point\command", False).GetValue(vbNullString, Nothing)

                    If valueInRegistry IsNot Nothing Then
                        ' We check if the current Registry path is different than the current process's EXE path.
                        If valueInRegistry.caseInsensitiveContains(Application.ExecutablePath) = False Then
                            ' OK, it doesn't match the current process's EXE path.

                            ' We parse out the EXE's path out of the combined path with the argument.
                            matches = Regex.Match(valueInRegistry, "((?:""|'){0,1}[A-Za-z]:\\.*\.(?:bat|bin|cmd|com|cpl|exe|gadget|inf1|ins|inx|isu|job|jse|lnk|msc|msi|msp|mst|paf|pif|ps1|reg|rgs|sct|shb|shs|u3p|vb|vbe|vbs|vbscript|ws|wsf)(?:""|'){0,1} {0,1})(.*)", RegexOptions.IgnoreCase)

                            If matches IsNot Nothing Then
                                ' Now we make sure that the file exists.
                                If IO.File.Exists(matches.Groups(1).Value.Replace(Chr(34), "").Trim) = False Then
                                    ' OK, it doesn't.  The entries in the Registry are invalid, now let's fix them.
                                    registryKey = registryRootKeyWeAreWorkingWith.OpenSubKey("CLSID\{20D04FE0-3AEA-1069-A2D8-08002B30309D}\Shell\Create Custom Named System Restore Point\Command", True)

                                    If registryKey IsNot Nothing Then ' This should prevent Null Reference Exceptions.
                                        registryKey.SetValue(vbNullString, String.Format("{0}{1}{0} -createrestorepointcustomname", Chr(34), Application.ExecutablePath))
                                        registryKey.SetValue("icon", String.Format("{0}{1}{0}", Chr(34), Application.ExecutablePath), RegistryValueKind.String)
                                        registryKey.Close()
                                        registryKey = Nothing
                                    End If
                                End If
                            End If

                            matches = Nothing
                        End If
                    End If
                End If

                matches = Nothing
                valueInRegistry = Nothing

                ' This checks to see if the entry exists...
                If registryRootKeyWeAreWorkingWith.OpenSubKey("CLSID\{20D04FE0-3AEA-1069-A2D8-08002B30309D}\Shell\Launch Restore Point Creator\command", False) IsNot Nothing Then
                    ' OK, it does exist so let's go on with the process of checking for the validity of the entries.
                    valueInRegistry = registryRootKeyWeAreWorkingWith.OpenSubKey("CLSID\{20D04FE0-3AEA-1069-A2D8-08002B30309D}\Shell\Launch Restore Point Creator\command", False).GetValue(vbNullString, Nothing)

                    If valueInRegistry IsNot Nothing Then
                        ' We check if the current Registry path is different than the current process's EXE path.
                        If valueInRegistry.caseInsensitiveContains(Application.ExecutablePath) = False Then
                            ' OK, it doesn't match the current process's EXE path.

                            ' We parse out the EXE's path out of the combined path with the argument.
                            matches = Regex.Match(valueInRegistry, "((?:""|'){0,1}[A-Za-z]:\\.*\.(?:bat|bin|cmd|com|cpl|exe|gadget|inf1|ins|inx|isu|job|jse|lnk|msc|msi|msp|mst|paf|pif|ps1|reg|rgs|sct|shb|shs|u3p|vb|vbe|vbs|vbscript|ws|wsf)(?:""|'){0,1} {0,1})(.*)", RegexOptions.IgnoreCase)

                            If matches IsNot Nothing Then
                                ' Now we make sure that the file exists.
                                If IO.File.Exists(matches.Groups(1).Value.Replace(Chr(34), "").Trim) = False Then
                                    ' OK, it doesn't.  The entries in the Registry are invalid, now let's fix them.
                                    registryKey = registryRootKeyWeAreWorkingWith.OpenSubKey("CLSID\{20D04FE0-3AEA-1069-A2D8-08002B30309D}\Shell\Launch Restore Point Creator\Command", True)

                                    If registryKey IsNot Nothing Then ' This should prevent Null Reference Exceptions.
                                        registryKey.SetValue(vbNullString, String.Format("{0}{1}{0}", Chr(34), Application.ExecutablePath))
                                        registryKey.SetValue("icon", String.Format("{0}{1}{0}", Chr(34), Application.ExecutablePath), RegistryValueKind.String)
                                        registryKey.Close()
                                        registryKey = Nothing
                                    End If
                                End If
                            End If

                            matches = Nothing
                        End If
                    End If
                Else
                    ' Nope, this user must have been using an older version of this program, one that didn't have this option.  So let's create it.

                    ' ===================================================
                    ' == Makes the "Launch Restore Point Creator" Item ==
                    ' ===================================================

                    If registryRootKeyWeAreWorkingWith.OpenSubKey("CLSID\{20D04FE0-3AEA-1069-A2D8-08002B30309D}\Shell") IsNot Nothing Then
                        registryRootKeyWeAreWorkingWith.OpenSubKey("CLSID\{20D04FE0-3AEA-1069-A2D8-08002B30309D}\Shell", True).CreateSubKey("Launch Restore Point Creator")
                        registryRootKeyWeAreWorkingWith.OpenSubKey("CLSID\{20D04FE0-3AEA-1069-A2D8-08002B30309D}\Shell\Launch Restore Point Creator", True).CreateSubKey("Command")

                        registryKey = registryRootKeyWeAreWorkingWith.OpenSubKey("CLSID\{20D04FE0-3AEA-1069-A2D8-08002B30309D}\Shell\Launch Restore Point Creator", True)
                        registryKey.SetValue("HasLUAShield", "", RegistryValueKind.String)
                        registryKey.SetValue("icon", String.Format("{0}{1}{0}", Chr(34), Application.ExecutablePath))
                        registryKey.SetValue("SuppressionPolicy", 1073741884, RegistryValueKind.DWord)
                        registryKey.Close()

                        registryKey = registryRootKeyWeAreWorkingWith.OpenSubKey("CLSID\{20D04FE0-3AEA-1069-A2D8-08002B30309D}\Shell\Launch Restore Point Creator\Command", True)
                        registryKey.SetValue(vbNullString, String.Format("{0}{1}{0}", Chr(34), Application.ExecutablePath))
                        registryKey.Close()
                    End If
                End If

                matches = Nothing

                If registryRootKeyWeAreWorkingWith.OpenSubKey("CLSID\{20D04FE0-3AEA-1069-A2D8-08002B30309D}\Shell\Create System Restore Checkpoint") IsNot Nothing Then
                    iconPath = registryRootKeyWeAreWorkingWith.OpenSubKey("CLSID\{20D04FE0-3AEA-1069-A2D8-08002B30309D}\Shell\Create System Restore Checkpoint").GetValue("icon", "nothing")
                    If iconPath.caseInsensitiveContains(Application.ExecutablePath) = False Then
                        registryRootKeyWeAreWorkingWith.OpenSubKey("CLSID\{20D04FE0-3AEA-1069-A2D8-08002B30309D}\Shell\Create System Restore Checkpoint", True).SetValue("icon", String.Format("{0}{1}{0}", Chr(34), Application.ExecutablePath))
                    End If
                End If

                If registryRootKeyWeAreWorkingWith.OpenSubKey("CLSID\{20D04FE0-3AEA-1069-A2D8-08002B30309D}\Shell\Create Custom Named System Restore Point") IsNot Nothing Then
                    iconPath = registryRootKeyWeAreWorkingWith.OpenSubKey("CLSID\{20D04FE0-3AEA-1069-A2D8-08002B30309D}\Shell\Create Custom Named System Restore Point").GetValue("icon", "nothing")
                    If iconPath.caseInsensitiveContains(Application.ExecutablePath) = False Then
                        registryRootKeyWeAreWorkingWith.OpenSubKey("CLSID\{20D04FE0-3AEA-1069-A2D8-08002B30309D}\Shell\Create Custom Named System Restore Point", True).SetValue("icon", String.Format("{0}{1}{0}", Chr(34), Application.ExecutablePath))
                    End If
                End If

                If registryRootKeyWeAreWorkingWith.OpenSubKey("CLSID\{20D04FE0-3AEA-1069-A2D8-08002B30309D}\Shell\Launch Restore Point Creator") IsNot Nothing Then
                    iconPath = registryRootKeyWeAreWorkingWith.OpenSubKey("CLSID\{20D04FE0-3AEA-1069-A2D8-08002B30309D}\Shell\Launch Restore Point Creator").GetValue("icon", "nothing")
                    If iconPath.caseInsensitiveContains(Application.ExecutablePath) = False Then
                        registryRootKeyWeAreWorkingWith.OpenSubKey("CLSID\{20D04FE0-3AEA-1069-A2D8-08002B30309D}\Shell\Launch Restore Point Creator", True).SetValue("icon", String.Format("{0}{1}{0}", Chr(34), Application.ExecutablePath))
                    End If
                End If
            End If

            registryRootKeyWeAreWorkingWith.Close()
        Catch ex As Exception
            Threading.Thread.CurrentThread.CurrentUICulture = New Globalization.CultureInfo("en-US")
            exceptionHandler.manuallyLoadCrashWindow(ex, ex.Message, ex.StackTrace, ex.GetType)
        End Try
    End Sub

    Private Sub checkScheduledTaskEXEPathsSubRoutine(ByRef taskService As TaskScheduler.TaskService, ByRef task As TaskScheduler.Task, commandLineArgument As String)
        If Functions.support.areWeInSafeMode() = True Then Exit Sub

        Try
            Dim actions As TaskScheduler.ActionCollection = task.Definition.Actions
            Dim execActionPath As String

            For Each action As TaskScheduler.Action In actions
                If action.ActionType = TaskScheduler.TaskActionType.Execute Then
                    execActionPath = DirectCast(action, TaskScheduler.ExecAction).Path.Replace("""", "")

                    ' We check if the current task ExecAction path is different than the current process's EXE path.
                    If execActionPath.Equals(Application.ExecutablePath, StringComparison.OrdinalIgnoreCase) = False Then
                        task.Definition.Actions.Remove(action)
                        actions.Add(New TaskScheduler.ExecAction(Application.ExecutablePath, commandLineArgument))
                        task.RegisterChanges()
                    End If
                End If
            Next
        Catch ex3 As COMException
            taskService.RootFolder.DeleteTask(task.Name)
            Functions.eventLogFunctions.writeCrashToEventLog(ex3)
            Functions.eventLogFunctions.writeToSystemEventLog("Invalid XML data has been detected as part of the task validation routine. The task named """ & task.Name & """ has been deleted.", EventLogEntryType.Error)
            MsgBox("One or more of your scheduled Restore Point Creator tasks have been found to be corrupted. Please check your scheduled tasks to see if any of them have been deleted.", MsgBoxStyle.Information, strMessageBoxTitle)
        Catch ex2 As IO.FileNotFoundException
            Functions.eventLogFunctions.writeCrashToEventLog(ex2)
        Catch ex As Exception
            Threading.Thread.CurrentThread.CurrentUICulture = New Globalization.CultureInfo("en-US")
            exceptionHandler.manuallyLoadCrashWindow(ex, ex.Message, ex.StackTrace, ex.GetType)
        End Try
    End Sub

    Private Sub checkScheduledTaskEXEPaths()
        If Functions.support.areWeInSafeMode() = True Then Exit Sub
        If Debugger.IsAttached = True Then Exit Sub

        Try
            Dim taskService As New TaskScheduler.TaskService

            For Each task As TaskScheduler.Task In taskService.RootFolder.Tasks
                If task.Name = "System Restore Checkpoint by System Restore Point Creator" Then
                    checkScheduledTaskEXEPathsSubRoutine(taskService, task, globalVariables.commandLineSwitches.scheduledRestorePoint)
                ElseIf task.Name = "Delete Old Restore Points" Then
                    checkScheduledTaskEXEPathsSubRoutine(taskService, task, "-deleteoldrestorepoints")
                ElseIf task.Name = "Create a Restore Point at User Logon" Then
                    checkScheduledTaskEXEPathsSubRoutine(taskService, task, globalVariables.commandLineSwitches.scheduledRestorePoint)
                End If
            Next

            taskService.Dispose()
            taskService = Nothing
        Catch ex As IO.FileNotFoundException
            Functions.eventLogFunctions.writeCrashToEventLog(ex)
        Catch ex As Exception
            Threading.Thread.CurrentThread.CurrentUICulture = New Globalization.CultureInfo("en-US")
            exceptionHandler.manuallyLoadCrashWindow(ex, ex.Message, ex.StackTrace, ex.GetType)
        End Try
    End Sub

    Private Sub loadPreferences()
        Try
            ConfigureHTTPTimeoutToolStripMenuItem.Text = String.Format("Configure HTTP Timeout ({0} Seconds)", My.Settings.httpTimeout)
            Dim registryObject As RegistryKey
            My.Settings.firstRun = False
            My.Settings.Save()
            Dim boolUpdateAtNextRunTime As Boolean

            If Registry.LocalMachine.OpenSubKey(globalVariables.registryValues.strKey) Is Nothing Then
                defaultCustomRestorePointName = ""
            Else
                defaultCustomRestorePointName = Registry.LocalMachine.OpenSubKey(globalVariables.registryValues.strKey).GetValue("Default Custom Restore Point Name", "").ToString.Trim
            End If

            If String.IsNullOrEmpty(defaultCustomRestorePointName.Trim) = False And Functions.support.areWeInSafeMode() = False Then btnCreateRestorePointNameWithDefaultName.Visible = True

            If My.Settings.updateChannel.Equals(globalVariables.updateChannels.stable, StringComparison.OrdinalIgnoreCase) Then
                toolStripStableChannel.Checked = True
                lineUnderRC.Visible = False
                OnlyGiveMeReleaseCandidates.Visible = False

                If IO.File.Exists(globalVariables.pdbFileNameInZIP) Then
                    Try
                        IO.File.Delete(globalVariables.pdbFileNameInZIP)
                    Catch ex As Exception
                        Dim deleteAtReboot As New Functions.deleteAtReboot()
                        deleteAtReboot.addItem(globalVariables.pdbFileNameInZIP)
                        deleteAtReboot.dispose(True)
                    End Try
                End If
            ElseIf My.Settings.updateChannel.Equals(globalVariables.updateChannels.beta, StringComparison.OrdinalIgnoreCase) Then
                toolStripBetaChannel.Checked = True
                lineUnderRC.Visible = True
                OnlyGiveMeReleaseCandidates.Visible = True
                OnlyGiveMeReleaseCandidates.Checked = My.Settings.onlyGiveMeRCs
            ElseIf My.Settings.updateChannel.Equals(globalVariables.updateChannels.tom, StringComparison.OrdinalIgnoreCase) Then
                ToolStripMenuItemPrivateForTom.Checked = True
                lineUnderRC.Visible = True
                OnlyGiveMeReleaseCandidates.Visible = True
                OnlyGiveMeReleaseCandidates.Checked = My.Settings.onlyGiveMeRCs
            End If

            If globalVariables.version.boolDebugBuild = True Then
                SwitchToDebugBuildToolStripMenuItem.Visible = False
                lineBeforeDebugMenuItem.Visible = False
            End If

            toolStripCloseAfterRestorePointIsCreated.Checked = My.Settings.closeAfterCreatingRestorePoint
            AllowForDeletionOfAllSystemRestorePointsToolStripMenuItem.Checked = My.Settings.allowDeleteOfAllRestorePoints
            ConfirmRestorePointDeletionsInBatchesToolStripMenuItem.Checked = My.Settings.multiConfirmRestorePointDeletions

            ' This code converts the old way of saving the user feedback type preference to the new way of saving the user feedback preference.
            If String.IsNullOrEmpty(My.Settings.notificationType) = False Then
                If My.Settings.notificationType = globalVariables.notificationTypeBalloon Then
                    My.Settings.notificationType2 = enums.userFeedbackType.balloon
                ElseIf My.Settings.notificationType = globalVariables.notificationTypeMessageBox Then
                    My.Settings.notificationType2 = enums.userFeedbackType.msgbox
                End If

                My.Settings.notificationType = Nothing
            End If
            ' This code converts the old way of saving the user feedback type preference to the new way of saving the user feedback preference.

            If My.Settings.notificationType2 = enums.userFeedbackType.balloon Then
                BalloonToolStripMenuItem.Checked = True
                MessageBoxToolStripMenuItem.Checked = False
            Else
                BalloonToolStripMenuItem.Checked = False
                MessageBoxToolStripMenuItem.Checked = True
            End If

            Me.Size = My.Settings.windowSize

            ColumnHeader1.Width = My.Settings.column1Size
            ColumnHeader2.Width = My.Settings.column2Size
            ColumnHeader3.Width = My.Settings.column3Size
            ColumnHeader4.Width = My.Settings.column4Size
            ColumnHeader5.Width = My.Settings.column5Size

            ShowMessageBoxAfterSuccessfulCreationOfRestorePointToolStripMenuItem.Checked = My.Settings.ShowMessageBoxAfterSuccessfulCreationOfRestorePoint
            ShowMessageBoxAfterSuccessfulDeletionOfRestorePointsToolStripMenuItem.Checked = My.Settings.ShowMessageBoxAfterSuccessfulDeletionOfRestorePoints
            CheckSystemDrivesForFullShadowStorageToolStripMenuItem.Checked = My.Settings.checkSystemDrivesForFullShadowStorage
            EnableSystemEventLoggingToolStripMenuItem.Checked = globalVariables.boolLogToSystemLog
            LogProgramLoadsAndExitsToEventLogToolStripMenuItem.Checked = globalVariables.boolLogLoadsAndExits
            UseSSLToolStripMenuItem.Checked = My.Settings.useSSL
            AskBeforeUpgradingUpdatingToolStripMenuItem.Checked = My.Settings.askToUpgrade
            Me.Location = Functions.support.verifyWindowLocation(My.Settings.mainWindowPosition)
            AskBeforeCreatingRestorePointToolStripMenuItem.Checked = My.Settings.askBeforeCreatingRestorePoint

            If My.Settings.CheckForUpdates = True Then
                If My.Settings.checkForUpdatesEveryInDays = 7 Then
                    toolStripCheckEveryWeek.Checked = True
                ElseIf My.Settings.checkForUpdatesEveryInDays = 14 Then
                    toolStripCheckEveryTwoWeeks.Checked = True
                Else
                    toolStripCheckCustom.Checked = True

                    If My.Settings.checkForUpdatesEveryInDays = 1 Then
                        toolStripCheckCustom.Text = "Custom Time Interval (Check for updates every day)"
                    Else
                        toolStripCheckCustom.Text = "Custom Time Interval (Check for updates every " & My.Settings.checkForUpdatesEveryInDays & " days)"
                    End If
                End If
            Else
                ConfigureAutomaticUpdatesToolStripMenuItem.Visible = False
            End If

            If globalVariables.KeepXAmountOfRestorePoints = True Then
                KeepXAmountOfRestorePointsToolStripMenuItem.Checked = True
                KeepXAmountOfRestorePointsToolStripMenuItem.Text &= " (" & globalVariables.KeepXAmountofRestorePointsValue & ")"
            End If

            registryObject = Registry.LocalMachine.OpenSubKey(globalVariables.registryValues.strKey, True)

            If registryObject IsNot Nothing Then
                boolUpdateAtNextRunTime = Functions.registryStuff.getBooleanValueFromRegistry(registryObject, "UpdateAtNextRunTime", False)
                registryObject.DeleteValue("UpdateAtNextRunTime", False)

                boolShowDonationMessage = Functions.registryStuff.getBooleanValueFromRegistry(registryObject, "Show Donation Message", True)

                ' Converts some settings over to Registry-based Settings.
                If registryObject.GetValue("Log Restore Point Deletions", Nothing) Is Nothing Then
                    registryObject.SetValue("Log Restore Point Deletions", My.Settings.boolLogDeletedRestorePoints.ToString)
                End If

                If registryObject.GetValue("Delete Old Restore Points", Nothing) Is Nothing Then
                    registryObject.SetValue("Delete Old Restore Points", My.Settings.deleteOldRestorePoints2)
                End If
                ' Converts some settings over to Registry-based Settings.

                registryObject.Close()
                registryObject.Dispose()
            End If

            If My.Settings.maxDaysManualDelete = -1 = False Then
                If My.Settings.maxDaysManualDelete = 1 Then
                    toolStripDeleteOldRestorePoints.Text = "Delete Restore Points older than 1 Day"
                Else
                    toolStripDeleteOldRestorePoints.Text = "Delete Restore Points older than " & My.Settings.maxDaysManualDelete & " Days"
                End If
            End If

            toolStripConfirmDeletions.Checked = My.Settings.confirmRestorePointDeletions

            registryObject = Registry.LocalMachine.OpenSubKey(globalVariables.registryValues.strKey, False)
            Dim boolNoTask, boolLogRestorePointDeletions, boolExtendedLoggingForScheduledTasks As Boolean

            If registryObject IsNot Nothing Then
                boolNoTask = Functions.registryStuff.getBooleanValueFromRegistry(registryObject, "No Task", False)
                BypassNoUACLauncherToolStripMenuItem.Checked = boolNoTask

                boolExtendedLoggingForScheduledTasks = Functions.registryStuff.getBooleanValueFromRegistry(registryObject, "Extended Logging For Scheduled Tasks", True)
                ExtendedLoggingForScheduledTasks.Checked = boolExtendedLoggingForScheduledTasks

                boolLogRestorePointDeletions = Functions.registryStuff.getBooleanValueFromRegistry(registryObject, "Log Restore Point Deletions", True)
                toolStripLogRestorePointDeletions.Checked = boolLogRestorePointDeletions

                globalVariables.boolExtendedLoggingDuringUpdating = Functions.registryStuff.getBooleanValueFromRegistry(registryObject, "Enable Extended Logging During Updating", True)
                EnableExtendedLoggingToolStripMenuItem.Checked = globalVariables.boolExtendedLoggingDuringUpdating

                registryObject.Close()
                registryObject.Dispose()
            Else
                BypassNoUACLauncherToolStripMenuItem.Checked = False
                boolNoTask = False

                toolStripLogRestorePointDeletions.Checked = False
                boolLogRestorePointDeletions = False

                boolExtendedLoggingForScheduledTasks = True
                toolStripLogRestorePointDeletions.Checked = True

                globalVariables.boolExtendedLoggingDuringUpdating = True
                EnableExtendedLoggingToolStripMenuItem.Checked = True
            End If

            Dim boolDidWeAlreadyLaunchTheCheckForUpdatesRoutine As Boolean = False

            If boolUpdateAtNextRunTime = True Or My.Settings.boolFirstRun = True Then
                boolDidWeAlreadyLaunchTheCheckForUpdatesRoutine = True
                Threading.ThreadPool.QueueUserWorkItem(Sub() formLoadCheckForUpdatesRoutine(True))
            End If

            If My.Settings.CheckForUpdates = True And boolDidWeAlreadyLaunchTheCheckForUpdatesRoutine = False Then
                toolStripAutomaticallyCheckForUpdates.Checked = True
                Threading.ThreadPool.QueueUserWorkItem(Sub() formLoadCheckForUpdatesRoutine())
            End If
        Catch ex2 As IO.IOException
            handleConfigFileAccessViolation(ex2)
        Catch ex As Exception
            Threading.Thread.CurrentThread.CurrentUICulture = New Globalization.CultureInfo("en-US")
            exceptionHandler.manuallyLoadCrashWindow(ex, ex.Message, ex.StackTrace, ex.GetType)
        End Try
    End Sub

    Private Sub addSpecialRegistryKeysToWindows8ToFixWindows8SystemRestorePoint()
        'Debug.WriteLine("We are running on Windows 8.x or Windows 10")
        ' Yep, we are.  Let the changes commence.

        If Environment.Is64BitOperatingSystem = False Then
            ' Checks to see if the Registry Subkey exists, that way we don't have a Null Reference Exception.
            If Registry.LocalMachine.OpenSubKey("SOFTWARE\Microsoft\Windows NT\CurrentVersion\SystemRestore") IsNot Nothing Then
                ' OK, the Registry Subkey exists, now to create the Registry value that we need to create.
                'debug.writeline("Setting special registry key for 32-bit")
                Registry.LocalMachine.OpenSubKey("SOFTWARE\Microsoft\Windows NT\CurrentVersion\SystemRestore", True).SetValue("SystemRestorePointCreationFrequency", 0, RegistryValueKind.DWord)
            Else
                ' No, the Registry Subkey doesn't exist, so we have to create the Registry Subkey first then set the Registry value.
                Registry.LocalMachine.OpenSubKey("SOFTWARE\Microsoft\Windows NT\CurrentVersion", True).CreateSubKey("SystemRestore")
                Registry.LocalMachine.OpenSubKey("SOFTWARE\Microsoft\Windows NT\CurrentVersion\SystemRestore", True).SetValue("SystemRestorePointCreationFrequency", 0, RegistryValueKind.DWord)
            End If
        ElseIf Environment.Is64BitOperatingSystem = True Then
            ' Checks to see if the Registry Subkey exists, that way we don't have a Null Reference Exception.
            If RegistryKey.OpenBaseKey(RegistryHive.LocalMachine, RegistryView.Registry64).OpenSubKey("SOFTWARE\Microsoft\Windows NT\CurrentVersion\SystemRestore") IsNot Nothing Then
                ' OK, it exists.  Now let's set the setting.
                'debug.writeline("Setting special registry key for 64-bit")
                RegistryKey.OpenBaseKey(RegistryHive.LocalMachine, RegistryView.Registry64).OpenSubKey("SOFTWARE\Microsoft\Windows NT\CurrentVersion\SystemRestore", True).SetValue("SystemRestorePointCreationFrequency", 0, RegistryValueKind.DWord)
            Else
                ' No, the Registry Subkey doesn't exist, so we have to create the Registry Subkey first then set the Registry value.
                'debug.writeline("Creating Registry subkey and setting special registry key for 64-bit")

                RegistryKey.OpenBaseKey(RegistryHive.LocalMachine, RegistryView.Registry64).OpenSubKey("SOFTWARE\Microsoft\Windows NT\CurrentVersion", True).CreateSubKey("SystemRestore")
                RegistryKey.OpenBaseKey(RegistryHive.LocalMachine, RegistryView.Registry64).OpenSubKey("SOFTWARE\Microsoft\Windows NT\CurrentVersion\SystemRestore", True).SetValue("SystemRestorePointCreationFrequency", 0, RegistryValueKind.DWord)
            End If

            ' This is commented out because it appears that this second registry key value isn't needed.  But, I'm not removing it since it may be needed later if time proves it's needed after all.
            ' Checks to see if the Registry Subkey exists, that way we don't have a Null Reference Exception.
            If Registry.LocalMachine.OpenSubKey("SOFTWARE\Microsoft\Windows NT\CurrentVersion\SystemRestore") IsNot Nothing Then
                ' OK, the Registry Subkey exists, now to create the Registry value that we need to create.
                Registry.LocalMachine.OpenSubKey("SOFTWARE\Microsoft\Windows NT\CurrentVersion\SystemRestore", True).SetValue("SystemRestorePointCreationFrequency", 0, RegistryValueKind.DWord)
            Else
                ' No, the Registry Subkey doesn't exist, so we have to create the Registry Subkey first then set the Registry value.

                Registry.LocalMachine.OpenSubKey("SOFTWARE\Microsoft\Windows NT\CurrentVersion", True).CreateSubKey("SystemRestore")
                Registry.LocalMachine.OpenSubKey("SOFTWARE\Microsoft\Windows NT\CurrentVersion\SystemRestore", True).SetValue("SystemRestorePointCreationFrequency", 0, RegistryValueKind.DWord)
            End If
        End If
    End Sub
#End Region

#Region "--== Functions and Sub-Routines ==--"
    Private Sub startCheckForUpdatesThread()
        Threading.ThreadPool.QueueUserWorkItem(AddressOf userInitiatedCheckForUpdates)
        toolStripCheckForUpdates.Enabled = False
    End Sub

    Public Enum userFeedbackType As Short
        typeError = 0
        typeInfo = 1
        typeWarning = 2
    End Enum

    Private Sub giveFeedbackToUser(feedBackMessage As String, Optional feedbackType As userFeedbackType = userFeedbackType.typeInfo)
        If My.Settings.notificationType2 = enums.userFeedbackType.msgbox Then
            If feedbackType = userFeedbackType.typeInfo Then
                MsgBox(feedBackMessage, MsgBoxStyle.Information, strMessageBoxTitle)
            ElseIf feedbackType = userFeedbackType.typeError Then
                MsgBox(feedBackMessage, MsgBoxStyle.Critical, strMessageBoxTitle)
            ElseIf feedbackType = userFeedbackType.typeWarning Then
                MsgBox(feedBackMessage, MsgBoxStyle.Exclamation, strMessageBoxTitle)
            End If
        Else
            If feedbackType = userFeedbackType.typeInfo Then
                NotifyIcon1.ShowBalloonTip(5000, strMessageBoxTitle, feedBackMessage, ToolTipIcon.Info)
            ElseIf feedbackType = userFeedbackType.typeError Then
                NotifyIcon1.ShowBalloonTip(5000, strMessageBoxTitle, feedBackMessage, ToolTipIcon.Error)
            ElseIf feedbackType = userFeedbackType.typeWarning Then
                NotifyIcon1.ShowBalloonTip(5000, strMessageBoxTitle, feedBackMessage, ToolTipIcon.Warning)
            End If
        End If
    End Sub

    Private Sub savePreferenceToRegistry(variableName As String, variableValue As String)
        Dim registryKeyObject As RegistryKey = Registry.LocalMachine.OpenSubKey(globalVariables.registryValues.strKey, True)

        If registryKeyObject IsNot Nothing Then
            registryKeyObject.SetValue(variableName, variableValue, RegistryValueKind.String)

            registryKeyObject.Close()
        End If
    End Sub

    Private Sub applySavedSorting()
        ' Some data validation.
        If My.Settings.sortingColumn < 0 Or My.Settings.sortingColumn > 4 Then
            My.Settings.sortingColumn = 0
        End If

        If My.Settings.sortingOrder <> 1 And My.Settings.sortingOrder <> 2 Then
            My.Settings.sortingOrder = 2
        End If
        ' Some data validation.

        ' Get the new sorting column.
        Dim new_sorting_column As ColumnHeader = systemRestorePointsList.Columns(My.Settings.sortingColumn)
        Dim sort_order As SortOrder = My.Settings.sortingOrder

        ' Figure out the new sorting order.
        If (m_SortingColumn IsNot Nothing) Then
            ' See if this is the same column.
            If new_sorting_column.Equals(m_SortingColumn) Then
                ' Same column. Switch the sort order.
                If m_SortingColumn.Text.StartsWith("> ") Then
                    sort_order = SortOrder.Descending
                    My.Settings.sortingOrder = SortOrder.Descending
                Else
                    sort_order = SortOrder.Ascending
                    My.Settings.sortingOrder = SortOrder.Ascending
                End If
            Else
                ' New column. Sort ascending.
                sort_order = SortOrder.Ascending
                My.Settings.sortingOrder = SortOrder.Ascending
            End If

            ' Remove the old sort indicator.
            m_SortingColumn.Text = m_SortingColumn.Text.Substring(2)
        End If

        ' Display the new sort order.
        m_SortingColumn = new_sorting_column
        If sort_order = SortOrder.Ascending Then
            m_SortingColumn.Text = "> " & m_SortingColumn.Text
        Else
            m_SortingColumn.Text = "< " & m_SortingColumn.Text
        End If

        ' Create a comparer.
        systemRestorePointsList.ListViewItemSorter = New Functions.listViewSorter.ListViewComparer(My.Settings.sortingColumn, sort_order)

        ' Sort.
        systemRestorePointsList.Sort()
    End Sub

    Private Sub startSystemRestorePointListLoadThreadSub()
        Try
            loadRestorePointsFromSystemIntoList()
        Catch ex As Threading.ThreadAbortException
        End Try
    End Sub

    Private Sub interfaceTooSmallSettingCheckFormLoadSubRoutine()
        Try
            Dim registryKey As RegistryKey = Registry.LocalMachine.OpenSubKey("SOFTWARE\Microsoft\Windows NT\CurrentVersion\AppCompatFlags\Layers", False)

            ' We don't want a Null Reference Exception so we check to see if the Registry Object we just created in Null.
            If registryKey IsNot Nothing Then
                ' OK, it isn't so let's continue with the checking.

                ' We now get the current value of our special Registry value. If the value doesn't exist then the default value that the .NET Framework should return is "Nothing".
                Dim valueInRegistry As String = registryKey.GetValue(Application.ExecutablePath.ToLower, "Nothing")

                ' Now we check to see if the value isn't equal to "Nothing" and if it contains the word "HIGHDPIAWARE".
                If valueInRegistry.Equals("Nothing") = False And valueInRegistry.caseInsensitiveContains("HIGHDPIAWARE") = True Then
                    InterfaceTooBigToolStripMenuItem.Checked = True
                End If

                ' Now we dispose of the Registry Access Objects.
                registryKey.Close()
            End If
        Catch ex As Exception
            ' Does nothing
        End Try

        'End If
    End Sub

    Private Function openUpdateDialog(versionUpdateType As Update_Message.versionUpdateType, remoteVersion As String, remoteBuild As String, strRemoteBetaRCVersion As String) As Update_Message.userResponse
        Dim updateMessageDialog As New Update_Message With {
            .StartPosition = FormStartPosition.CenterScreen,
            .versionUpdate = versionUpdateType,
            .remoteVersion = remoteVersion,
            .remoteBuild = remoteBuild,
            .strRemoteBetaRCVersion = strRemoteBetaRCVersion,
            .TopMost = True
        }

        updateMessageDialog.ShowDialog()

        Dim response As Update_Message.userResponse = updateMessageDialog.dialogResponse

        updateMessageDialog.Dispose()
        updateMessageDialog = Nothing

        Return response
    End Function

    Private Sub addRPSessionInterval()
        Try
            If Registry.LocalMachine.OpenSubKey("SOFTWARE\Microsoft\Windows NT\CurrentVersion\SystemRestore") IsNot Nothing Then
                Dim regKey As RegistryKey = Registry.LocalMachine.OpenSubKey("SOFTWARE\Microsoft\Windows NT\CurrentVersion\SystemRestore", True)

                Dim value As Object = regKey.GetValue("RPSessionInterval", Nothing)

                If value Is Nothing Then
                    regKey.SetValue("RPSessionInterval", 1, RegistryValueKind.DWord)
                Else
                    If value <> 1 Then
                        regKey.SetValue("RPSessionInterval", 1, RegistryValueKind.DWord)
                    End If
                End If

                regKey.Close()
            End If
        Catch ex As Exception
            Functions.eventLogFunctions.writeCrashToEventLog(ex)
        End Try
    End Sub

    Private Sub addRPGlobalInterval()
        Try
            If Registry.LocalMachine.OpenSubKey("SOFTWARE\Microsoft\Windows NT\CurrentVersion\SystemRestore") IsNot Nothing Then
                Dim regKey As RegistryKey = Registry.LocalMachine.OpenSubKey("SOFTWARE\Microsoft\Windows NT\CurrentVersion\SystemRestore", True)

                Dim value As Object = regKey.GetValue("RPGlobalInterval", Nothing)

                If value Is Nothing Then
                    regKey.SetValue("RPGlobalInterval", 1, RegistryValueKind.DWord)
                Else
                    If value <> 1 Then
                        regKey.SetValue("RPGlobalInterval", 1, RegistryValueKind.DWord)
                    End If
                End If

                regKey.Close()
            End If
        Catch ex As Exception
            Functions.eventLogFunctions.writeCrashToEventLog(ex)
        End Try
    End Sub

    Private Sub addShortCutForEventLogToUsersStartMenu()
        Dim pathInStartMenu As String = IO.Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.CommonStartMenu), "Programs\Restore Point Creator")

        ' Checks to see if this application's executable is in a safe place, in this case... Program Files.
        If Application.ExecutablePath.caseInsensitiveContains("program files") = True Then
            ' Checks to see if a program folder exists.
            If IO.Directory.Exists(pathInStartMenu) = True Then
                Dim pathOfShortcutWeAreGoingToMake As String = IO.Path.Combine(pathInStartMenu, "Restore Point Creator Event Log Viewer.lnk")

                If IO.File.Exists(pathOfShortcutWeAreGoingToMake) = False Then
                    Functions.support.createShortcut(pathOfShortcutWeAreGoingToMake, Application.ExecutablePath, Application.ExecutablePath, "Restore Point Creator Event Log Viewer", globalVariables.commandLineSwitches.viewEventLog)
                End If
            End If
        End If
    End Sub

    Private Sub deleteRPLifeIntervalValue()
        Dim registryKey As RegistryKey

        If Environment.Is64BitOperatingSystem = False Then
            registryKey = Registry.LocalMachine.OpenSubKey("SOFTWARE\Microsoft\Windows NT\CurrentVersion\SystemRestore", True)

            If registryKey IsNot Nothing Then
                Try
                    registryKey.DeleteValue("RPLifeInterval", False)
                Catch ex As Exception
                End Try

                registryKey.Close()
                registryKey = Nothing
            End If
        Else
            registryKey = RegistryKey.OpenBaseKey(RegistryHive.LocalMachine, RegistryView.Registry64).OpenSubKey("SOFTWARE\Microsoft\Windows NT\CurrentVersion\SystemRestore", True)

            If registryKey IsNot Nothing Then
                Try
                    registryKey.DeleteValue("RPLifeInterval", False)
                Catch ex As Exception
                End Try

                registryKey.Close()
                registryKey = Nothing
            End If

            registryKey = Registry.LocalMachine.OpenSubKey("SOFTWARE\Microsoft\Windows NT\CurrentVersion\SystemRestore", True)

            If registryKey IsNot Nothing Then
                Try
                    registryKey.DeleteValue("RPLifeInterval", False)
                Catch ex As Exception
                End Try

                registryKey.Close()
                registryKey = Nothing
            End If
        End If
    End Sub

    Private Sub newFileDeleterThreadSub()
        If globalVariables.boolExtendedLoggingDuringUpdating = True Then
            Functions.eventLogFunctions.writeToSystemEventLog("New File Deleter thread sleeping for 5 seconds for processes to close out before continuing.", EventLogEntryType.Information)
        End If

        Threading.Thread.Sleep(5000)
        Dim strFoundFile As String = New IO.FileInfo(Application.ExecutablePath & ".new.exe").Name

        If globalVariables.boolExtendedLoggingDuringUpdating = True Then
            Functions.eventLogFunctions.writeToSystemEventLog(String.Format("Found file named {0}{1}{0}. Now searching for any processes that have parent executables files of {0}{1}{0}.", Chr(34), strFoundFile))
        End If

        Functions.support.searchForProcessAndKillIt(Application.ExecutablePath & ".new.exe", True)

        If globalVariables.boolExtendedLoggingDuringUpdating = True Then
            Functions.eventLogFunctions.writeToSystemEventLog(String.Format("Now attempting to delete {0}{1}{0}.", Chr(34), strFoundFile))
        End If

        Try
            IO.File.Delete(Application.ExecutablePath & ".new.exe")

            If globalVariables.boolExtendedLoggingDuringUpdating = True Then
                Functions.eventLogFunctions.writeToSystemEventLog(String.Format("Deletion of {0}{1}{0} was successful.", Chr(34), strFoundFile))
            End If
        Catch ex As Exception
            Dim deleteAtReboot As New Functions.deleteAtReboot()
            deleteAtReboot.addItem(Application.ExecutablePath & ".new.exe")
            deleteAtReboot.dispose(True)

            If globalVariables.boolExtendedLoggingDuringUpdating = True Then
                Functions.eventLogFunctions.writeToSystemEventLog(String.Format("Deletion of {0}{1}{0} was unsuccessful, scheduling it to be deleted at next system reboot.", Chr(34), strFoundFile))
            End If
        End Try
    End Sub

    Private Sub updaterDeleterThreadSub()
        Try
            Functions.support.searchForProcessAndKillIt("updater.exe", False)
            Functions.support.searchForProcessAndKillIt("updater.exe", False)
            Threading.Thread.Sleep(250) ' We're going to sleep to give the system some time to kill the process.
            Functions.support.deleteFileWithNoException("updater.exe")
        Catch ex As Exception
        End Try
    End Sub

    Private Sub checkRestorePointSpaceThreadSub()
        Try
            Functions.vss.checkSystemDrivesForFullShadowStorage()
        Catch ex As Exception
        End Try
    End Sub

    Private Sub showDonationNotice()
        Try
            If boolShowDonationMessage = True Then
                Dim randomNumberGenerator As New Random()

                If randomNumberGenerator.Next(0, 5) = randomNumberGenerator.Next(0, 5) Then
                    Dim result As MsgBoxResult = MsgBox("Though this is free software, donations are welcome." & vbCrLf & vbCrLf & "Remember... donations are optional, there is no requirement to donate to use this software." & vbCrLf & vbCrLf & "Do you want to donate today?  If not, click ""No"" and you won't be asked like this again.", MsgBoxStyle.YesNo + MsgBoxStyle.Question, strMessageBoxTitle)

                    If result = MsgBoxResult.Yes Then
                        launchDonationURL()
                    Else
                        savePreferenceToRegistry("Show Donation Message", "False")
                    End If
                End If
            End If
        Catch ex As Exception
            Functions.eventLogFunctions.writeCrashToEventLog(ex)
        End Try
    End Sub

    Private Sub launchDonationURL()
        Functions.support.launchURLInWebBrowser(globalVariables.webURLs.webPages.strPayPal, "An error occurred when trying to launch the donation URL in your default browser. The donation URL has been copied to your Windows Clipboard for you to paste into the address bar in the browser of your choice.")
    End Sub

    Private Sub switchToDebugBuildDownloadThreadSub()
        Try
            Dim memoryStream As New IO.MemoryStream()

            If Functions.http.downloadFile(globalVariables.webURLs.updateBranch.debug.strProgramZIP, memoryStream) = False Then
                closePleaseWaitPanel()
                MsgBox("There was an error while downloading required files, please check the Event Log for more details.", MsgBoxStyle.Critical, strMessageBoxTitle)

                memoryStream.Close()
                memoryStream.Dispose()
                memoryStream = Nothing

                Exit Sub
            End If

            If Functions.checksum.verifyChecksum(globalVariables.webURLs.updateBranch.debug.strProgramZIPSHA2, memoryStream, True) = False Then
                memoryStream.Close()
                memoryStream.Dispose()
                memoryStream = Nothing

                Exit Sub
            End If

            Dim strNewApplicationFileNameFullName As String = New IO.FileInfo(Application.ExecutablePath).FullName & ".new.exe"

            If IO.File.Exists(strNewApplicationFileNameFullName) Then
                Try
                    IO.File.Delete(strNewApplicationFileNameFullName)
                Catch ex As Exception
                    Me.Invoke(Sub() closePleaseWaitPanel())
                    MsgBox("An existing new program executable file has been found, we tried to delete it but we couldn't. Please see the Application Event Log for more details.", MsgBoxStyle.Critical, strMessageBoxTitle)
                    Functions.eventLogFunctions.writeToSystemEventLog("An existing new program executable file has been found, we tried to delete it but we couldn't.", EventLogEntryType.Error)
                    Functions.eventLogFunctions.writeCrashToEventLog(ex)
                    Exit Sub
                End Try
            End If

            memoryStream.Position = 0
            Dim zipFileObject As New ZipFile(memoryStream)

            If Functions.support.extractUpdatedFileFromZIPPackage(zipFileObject, globalVariables.programFileNameInZIP, strNewApplicationFileNameFullName) = False Then
                MsgBox("There was an issue extracting data from the downloaded ZIP file.", MsgBoxStyle.Critical, strMessageBoxTitle)

                zipFileObject.Close()
                memoryStream.Close()
                memoryStream.Dispose()
                memoryStream = Nothing
                Exit Sub
            End If

            If Functions.support.extractUpdatedFileFromZIPPackage(zipFileObject, globalVariables.pdbFileNameInZIP, globalVariables.pdbFileNameInZIP & ".new") = False Then
                MsgBox("There was an issue extracting data from the downloaded ZIP file.", MsgBoxStyle.Critical, strMessageBoxTitle)

                zipFileObject.Close()
                memoryStream.Close()
                memoryStream.Dispose()
                memoryStream = Nothing
                Exit Sub
            End If

            zipFileObject.Close()
            memoryStream.Close()
            memoryStream.Dispose()
            memoryStream = Nothing

            If IO.File.Exists(strNewApplicationFileNameFullName) = True Then
                Process.Start(New ProcessStartInfo With {.FileName = strNewApplicationFileNameFullName, .Arguments = "-updatewithoutuninstallinfoupdate", .Verb = "runas"})
                Process.GetCurrentProcess.Kill()
            Else
                MsgBox("Something went wrong during the download, update process aborted.", MsgBoxStyle.Critical, strMessageBoxTitle)
            End If
        Catch ex As Exception
            exceptionHandler.manuallyLoadCrashWindow(ex, ex.Message, ex.StackTrace, ex.GetType)
        End Try
    End Sub

    Public Sub doTheGrayingOfTheRestorePointNameTextBox()
        If txtRestorePointDescription.Text.caseInsensitiveContains(strTypeYourRestorePointName) = True Then
            txtRestorePointDescription.ForeColor = Color.DimGray
            btnCreate.Enabled = False
        ElseIf String.IsNullOrEmpty(txtRestorePointDescription.Text.Trim) Then
            txtRestorePointDescription.Text = strTypeYourRestorePointName
            txtRestorePointDescription.ForeColor = Color.DimGray
            btnCreate.Enabled = False
        End If
    End Sub

    Private Sub saveRestorePointListColumnOrder()
        Try
            Dim columnCount As Integer = systemRestorePointsList.Columns.Count - 1
            Dim columnIndexes(columnCount) As String
            Dim column As ColumnHeader, index As Integer

            For index = 0 To columnCount
                column = systemRestorePointsList.Columns.Item(index)
                columnIndexes(column.DisplayIndex) = index.ToString()
            Next

            My.Settings.restorePointListColumnOrder2 = (New Web.Script.Serialization.JavaScriptSerializer).Serialize(columnIndexes)
        Catch ex As Exception
            Functions.eventLogFunctions.writeToSystemEventLog("Error saving restore point list column ordering.", EventLogEntryType.Error)
            Functions.eventLogFunctions.writeCrashToEventLog(ex)
        End Try
    End Sub

    Private Sub loadRestorePointListColumnOrder()
        Try
            If Not String.IsNullOrEmpty(My.Settings.restorePointListColumnOrder2) Then
                Dim columnIndexes As String() = (New Web.Script.Serialization.JavaScriptSerializer).Deserialize(Of String())(My.Settings.restorePointListColumnOrder2)
                Dim displayIndex, index As Integer

                For displayIndex = 0 To columnIndexes.Count - 1
                    index = Integer.Parse(columnIndexes(displayIndex))
                    systemRestorePointsList.Columns(index).DisplayIndex = displayIndex
                Next
            End If
        Catch ex As Exception
            My.Settings.restorePointListColumnOrder2 = Nothing
            Functions.eventLogFunctions.writeToSystemEventLog("Error loading saved restore point list column ordering.", EventLogEntryType.Error)
            Functions.eventLogFunctions.writeCrashToEventLog(ex)
        End Try
    End Sub

    Private Sub deleteAllRestorePointsThread()
        ' Declares some variables.
        Dim systemRestorePoints As ManagementObjectSearcher
        Dim numberOfOldRestorePointsDeleted As Short = 0
        Dim dateTime As DateTime

        Try
            If toolStripLogRestorePointDeletions.Checked Then
                Functions.eventLogFunctions.writeToSystemEventLog(String.Format("Began deleting all System Restore Points by {0}\{1}.", Environment.MachineName, Environment.UserName), EventLogEntryType.Information)
            End If

            btnCreate.Enabled = False
            btnCreateSystemCheckpoint.Enabled = False
            txtRestorePointDescription.Enabled = False

            btnRefreshRestorePoints.Enabled = False
            systemRestorePointsList.Enabled = False
            systemRestorePointsList.Items.Clear() ' Clears the System Restore Points list on the GUI of existing items.

            ' Get all System Restore Points from the Windows Management System and puts then in the systemRestorePoints variable.
            systemRestorePoints = New ManagementObjectSearcher("root\DEFAULT", "SELECT * FROM SystemRestore")
            Dim oldNumberOfRestorePoints As Integer = systemRestorePoints.Get().Count

            If AllowForDeletionOfAllSystemRestorePointsToolStripMenuItem.Checked Then
                newestSystemRestoreID = 0
            End If

            ' Checks to see if there are any System Restore Points to be listed.
            If systemRestorePoints.Get().Count <> 0 Then
                ' Loops through systemRestorePoints.
                For Each systemRestorePoint As ManagementObject In systemRestorePoints.Get()
                    If newestSystemRestoreID <> Integer.Parse(systemRestorePoint("SequenceNumber").ToString) Then
                        'debug.writeline("Deleting Restore Point ID " & systemRestorePoint("SequenceNumber").ToString)
                        Threading.Thread.Sleep(500)

                        If toolStripLogRestorePointDeletions.Checked Then
                            numberOfOldRestorePointsDeleted += 1

                            If String.IsNullOrEmpty(systemRestorePoint("CreationTime").ToString.Trim) = False Then
                                dateTime = Functions.restorePointStuff.parseSystemRestorePointCreationDate(systemRestorePoint("CreationTime").ToString)

                                If toolStripLogRestorePointDeletions.Checked Then
                                    Functions.eventLogFunctions.writeToSystemEventLog(String.Format("Deleted Restore Point named ""{0}"" which was created on {1} at {2}.", systemRestorePoint("Description").ToString, dateTime.ToShortDateString, dateTime.ToLongTimeString), EventLogEntryType.Information)
                                End If

                                dateTime = Nothing
                            End If
                        End If

                        Functions.APIs.NativeMethods.SRRemoveRestorePoint(Integer.Parse(systemRestorePoint("SequenceNumber").ToString))
                    End If

                    systemRestorePoint.Dispose()
                    systemRestorePoint = Nothing
                Next

                While oldNumberOfRestorePoints = systemRestorePoints.Get().Count
                    ' Does nothing, just loops.
                    Threading.Thread.Sleep(500)
                End While

                lblCurrentRestorePointsLabel.Text = String.Format("Current Restore Points ({0})", systemRestorePointsList.Items.Count)

                closePleaseWaitPanel()

                systemRestorePoints.Dispose()
                systemRestorePoints = Nothing
            Else
                newestSystemRestoreID = 0
            End If
        Catch ex As Threading.ThreadAbortException
            ' Does nothing
        Catch ex2 As Exception
            Threading.Thread.CurrentThread.CurrentUICulture = New Globalization.CultureInfo("en-US")
            exceptionHandler.manuallyLoadCrashWindow(ex2, ex2.Message, ex2.StackTrace, ex2.GetType)
        Finally
            If toolStripLogRestorePointDeletions.Checked Then
                If numberOfOldRestorePointsDeleted = 0 Then
                    Functions.eventLogFunctions.writeToSystemEventLog("End of processing old System Restore Points. No old System Restore Point were deleted.", EventLogEntryType.Information)
                ElseIf numberOfOldRestorePointsDeleted = 1 Then
                    Functions.eventLogFunctions.writeToSystemEventLog("End of processing old System Restore Points. 1 old System Restore Point was deleted.", EventLogEntryType.Information)
                Else
                    Functions.eventLogFunctions.writeToSystemEventLog(String.Format("End of processing old System Restore Points. {0} old System Restore Points were deleted.", numberOfOldRestorePointsDeleted), EventLogEntryType.Information)
                End If
            End If

            If Functions.support.areWeInSafeMode() = False Then
                btnCreateSystemCheckpoint.Enabled = True
                btnCreate.Enabled = True
                txtRestorePointDescription.Enabled = True
            End If

            loadRestorePointsFromSystemIntoList()
            systemRestorePointsList.Enabled = True
            toolStripDeleteAllRestorePoints.Enabled = True
            enableFormElements()
        End Try
    End Sub

    Public Sub doDeleteOldSystemRestorePoint(maxAgeInput As Short)
        toolStripDeleteOldRestorePoints.Enabled = False
        btnDeleteRestorePoint.Enabled = False
        stripDelete.Enabled = False

        openPleaseWaitPanel("Deleting Restore Points... Please Wait.")
        Threading.ThreadPool.QueueUserWorkItem(Sub() deleteOldRestorePoints(maxAgeInput))
    End Sub

    Private Sub disableFormElements()
        txtRestorePointDescription.Enabled = False
        toolStripCloseAfterRestorePointIsCreated.Enabled = False
        toolStripAbout.Enabled = False
        toolStripCheckForUpdates.Enabled = False
        toolStripAutomaticallyCheckForUpdates.Enabled = False
        toolStripConfirmDeletions.Enabled = False
        systemRestorePointsList.Enabled = False
        btnRefreshRestorePoints.Enabled = False
        btnRestoreToRestorePoint.Enabled = False
        stripRestoreSafeMode.Enabled = False
        btnRestoreToRestorePointSafeMode.Enabled = False
        btnCreateSystemCheckpoint.Enabled = False
        stripRefresh.Enabled = False
        restorePointListContextMenu.Enabled = False
        toolStripManageSystemRestoreStorageSize.Enabled = False
        toolStripLogRestorePointDeletions.Enabled = False
    End Sub

    Private Sub enableFormElements()
        txtRestorePointDescription.Enabled = True
        toolStripCloseAfterRestorePointIsCreated.Enabled = True
        toolStripAbout.Enabled = True
        toolStripCheckForUpdates.Enabled = True
        toolStripAutomaticallyCheckForUpdates.Enabled = True
        toolStripConfirmDeletions.Enabled = True
        systemRestorePointsList.Enabled = True

        btnDeleteRestorePoint.Enabled = True
        toolStripManageSystemRestoreStorageSize.Enabled = True
        toolStripLogRestorePointDeletions.Enabled = True

        btnRefreshRestorePoints.Enabled = True
        If Functions.support.areWeInSafeMode() = False Then btnCreateSystemCheckpoint.Enabled = True

        stripRefresh.Enabled = True
        restorePointListContextMenu.Enabled = True
    End Sub

    Private Sub giveFeedbackAfterCreatingRestorePoint(result As Integer)
        If result = Functions.APIs.errorCodes.ERROR_SUCCESS Then
            If Me.ShowMessageBoxAfterSuccessfulCreationOfRestorePointToolStripMenuItem.Checked = True Then
                giveFeedbackToUser("System Restore Point Created Successfully.", userFeedbackType.typeInfo)
            End If
        ElseIf result = Functions.APIs.errorCodes.ERROR_DISK_FULL Then
            giveFeedbackToUser("System Restore Point Creation Failed.  Disk Full." & vbCrLf & vbCrLf & "Internal Windows Error Code: ERROR_DISK_FULL (112)", userFeedbackType.typeError)
        ElseIf result = Functions.APIs.errorCodes.ERROR_ACCESS_DENIED Then
            giveFeedbackToUser("System Restore Point Creation Failed.  Access Denied." & vbCrLf & vbCrLf & "Internal Windows Error Code: ERROR_ACCESS_DENIED (5)", userFeedbackType.typeError)
        ElseIf result = Functions.APIs.errorCodes.ERROR_INTERNAL_ERROR Then
            giveFeedbackToUser("System Restore Point Creation Failed.  Internal Error." & vbCrLf & vbCrLf & "Internal Windows Error Code: ERROR_INTERNAL_ERROR (1359)", userFeedbackType.typeError)
        ElseIf result = Functions.APIs.errorCodes.ERROR_INVALID_DATA Then
            giveFeedbackToUser("System Restore Point Creation Failed.  Invalid Data." & vbCrLf & vbCrLf & "Internal Windows Error Code: ERROR_INVALID_DATA (13)", userFeedbackType.typeError)
        ElseIf result = Functions.APIs.errorCodes.ERROR_TIMEOUT Then
            giveFeedbackToUser("System Restore Point Creation Failed.  Invalid Data." & vbCrLf & vbCrLf & "Internal Windows Error Code: ERROR_TIMEOUT (1460)", userFeedbackType.typeError)
        ElseIf result = Functions.APIs.errorCodes.ERROR_SERVICE_DISABLED Then
            giveFeedbackToUser("System Restore Point Creation Failed.  Invalid Data." & vbCrLf & vbCrLf & "Internal Windows Error Code: ERROR_SERVICE_DISABLED (1058)", userFeedbackType.typeError)
        ElseIf result = Functions.APIs.errorCodes.ERROR_BAD_ENVIRONMENT Then
            giveFeedbackToUser("System Restore Point Creation Failed.  Invalid Data." & vbCrLf & vbCrLf & "Internal Windows Error Code: ERROR_BAD_ENVIRONMENT (10)", userFeedbackType.typeError)
        Else
            giveFeedbackToUser("System Restore Point Creation Failed." & vbCrLf & vbCrLf & "Internal Windows Error Code: UNKNOWN_ERROR (9999)", userFeedbackType.typeError)
        End If
    End Sub

    Private Sub unifiedCreateSystemRestorePoint(Optional ByVal stringRestorePointName As String = "System Checkpoint made by System Restore Point Creator")
        Try
            If String.IsNullOrEmpty(stringRestorePointName.Trim) Then
                MsgBox("You must enter a description for your System Restore Point.", MsgBoxStyle.Critical, strMessageBoxTitle)
                Exit Sub
            End If

            disableFormElements()

            'Dim systemRestore As New SystemRestorePointCreator.Classes.SystemRestore
            Dim sequenceNumber As Long = newestSystemRestoreID + 1
            Dim result As Integer

            ' Get all System Restore Points from the Windows Management System and puts then in the systemRestorePoints variable.
            Dim systemRestorePoints As New ManagementObjectSearcher("root\DEFAULT", "SELECT * FROM SystemRestore")
            Dim oldNewestRestorePointID As Integer = Functions.wmi.getNewestSystemRestorePointID()

            result = Functions.wmi.createRestorePoint(stringRestorePointName, Functions.restorePointStuff.RestoreType.WindowsType, sequenceNumber)

            If result = Functions.APIs.errorCodes.ERROR_SERVICE_DISABLED Then
                Dim reservedSpaceSize As ULong = Functions.vss.getMaxSize(globalVariables.systemDriveLetter)

                Functions.eventLogFunctions.writeToSystemEventLog("The system restore point API returned error code 1058 (ERROR_SERVICE_DISABLED). Attempting to auto-correct this issue.", EventLogEntryType.Warning)

                If reservedSpaceSize = 0 Then
                    If boolHaveWeTriedToCreateTheRestorePointAgain Then
                        Functions.eventLogFunctions.writeToSystemEventLog("Auto-corrections to system configuration has failed. Unable to create restore point.", EventLogEntryType.Error)
                        Exit Sub
                    Else
                        Dim gigabytesInBytes As Long = 1073741824
                        Dim newSize As Long = gigabytesInBytes * 20 ' Sets the size to 20 GBs.

                        Functions.eventLogFunctions.writeToSystemEventLog("The system returned error code 1058 (ERROR_SERVICE_DISABLED). Attempting to correct it by setting up reserved system restore point space and enabling system restore on the system drive.")

                        If MsgBox("The system has returned error code 1058 (ERROR_SERVICE_DISABLED). System Restore Point Creator can go about fixing this issue but it could have unintended consequences." & vbCrLf & vbCrLf & "Do you want System Restore Point Creator to attempt repairs to your system?", MsgBoxStyle.Question + MsgBoxStyle.YesNo + MsgBoxStyle.ApplicationModal, "System Error 1058 (ERROR_SERVICE_DISABLED) -- System Restore Point Creator") = MsgBoxResult.Yes Then
                            Functions.vss.executeVSSAdminCommand(globalVariables.systemDriveLetter)
                            Functions.vss.setShadowStorageSize(globalVariables.systemDriveLetter, newSize)
                            Functions.vss.enableSystemRestoreOnDriveWMI(globalVariables.systemDriveLetter)
                            boolHaveWeTriedToCreateTheRestorePointAgain = True
                        Else
                            MsgBox("You have chosen not to repair your system. System Restore Point creation has failed.", MsgBoxStyle.Exclamation, "System Restore Point Creator")
                            Exit Sub
                        End If

                        Functions.eventLogFunctions.writeToSystemEventLog("Attempting to create the restore point after system configuration corrections.")
                        unifiedCreateSystemRestorePoint(stringRestorePointName)
                        Exit Sub
                    End If
                Else
                    Dim msgBoxAndEventLogText As String = "The reserved space for restore points on the system drive appears to be set correctly, something else appears to be wrong. Auto-correction of system configurations may cause unintended side-effects. The auto-correction routine has halted."

                    Functions.eventLogFunctions.writeToSystemEventLog(msgBoxAndEventLogText, EventLogEntryType.Error)

                    enableFormElements()

                    closePleaseWaitPanel()
                    systemRestorePoints.Dispose()
                    giveFeedbackAfterCreatingRestorePoint(result)
                    loadRestorePointsFromSystemIntoList()

                    txtRestorePointDescription.Text = Nothing
                    doTheGrayingOfTheRestorePointNameTextBox()

                    MsgBox(msgBoxAndEventLogText & vbCrLf & vbCrLf & "If you want to try and correct it, go to the Utilities menu and click on ""Manually Fix System Restore"".", MsgBoxStyle.Exclamation, strMessageBoxTitle)

                    Exit Sub
                End If
            End If

            If result = Functions.APIs.errorCodes.ERROR_SUCCESS Then
                boolHaveWeTriedToCreateTheRestorePointAgain = False
            End If

            If result <> Functions.APIs.errorCodes.ERROR_SUCCESS Then
                If result <> Functions.APIs.errorCodes.ERROR_SERVICE_DISABLED Then
                    Functions.eventLogFunctions.writeToSystemEventLog("The system restore point API returned an error code (" & result & ").", EventLogEntryType.Warning)
                End If

                closePleaseWaitPanel()

                MsgBox(String.Format("There was an error while attempting to creating the restore point. The error code returned from the system was ""{0}"" ({1}).", result, Functions.support.convertErrorCodeToHex(result)), MsgBoxStyle.Critical, strMessageBoxTitle)
                Exit Sub
            End If

            ' We wait here with this loop until the system's has the restore point created.
            While oldNewestRestorePointID = Functions.wmi.getNewestSystemRestorePointID()
                ' Does nothing, just loops and sleeps for half a second.
                Threading.Thread.Sleep(500)
            End While

            If globalVariables.KeepXAmountOfRestorePoints = True Then
                Functions.wmi.doDeletingOfXNumberOfRestorePoints(globalVariables.KeepXAmountofRestorePointsValue)
            End If

            enableFormElements()

            closePleaseWaitPanel()
            systemRestorePoints.Dispose()
            giveFeedbackAfterCreatingRestorePoint(result)
            loadRestorePointsFromSystemIntoList()

            txtRestorePointDescription.Text = Nothing
            doTheGrayingOfTheRestorePointNameTextBox()

            If toolStripCloseAfterRestorePointIsCreated.Checked Then Me.Close()
        Catch ex2 As Exception
            Threading.Thread.CurrentThread.CurrentUICulture = New Globalization.CultureInfo("en-US")
            exceptionHandler.manuallyLoadCrashWindow(ex2, ex2.Message, ex2.StackTrace, ex2.GetType)
        End Try
    End Sub

    Private Sub giveDownloadErrorMessage()
        closePleaseWaitPanel()
        MsgBox("There was an error while downloading required files, please check the Event Log for more details.", MsgBoxStyle.Critical, strMessageBoxTitle)
    End Sub

    Private Sub downloadAndDoTheUpdate(Optional boolOverrideUserUpdateChannelPreferences As Boolean = False)
        Functions.eventLogFunctions.writeToSystemEventLog("Beginning Application Update Procedure.", EventLogEntryType.Information)

        If globalVariables.boolExtendedLoggingDuringUpdating = True Then
            Functions.eventLogFunctions.writeToSystemEventLog("Extended logging enabled for application update procedure.", EventLogEntryType.Information)
        End If

        Dim updateChannel As String = My.Settings.updateChannel
        Dim extractPDB As Boolean = False
        Dim boolRebootNeeded As Boolean = False
        Dim memoryStream As New IO.MemoryStream()
        Dim overrideURLPaths As Boolean = False

        If boolOverrideUserUpdateChannelPreferences And Not updateChannel.Equals(globalVariables.updateChannels.stable) Then
            If globalVariables.boolExtendedLoggingDuringUpdating = True Then
                Functions.eventLogFunctions.writeToSystemEventLog("Forcing the update channel to the stable channel for this update session.", EventLogEntryType.Information)
            End If

            updateChannel = globalVariables.updateChannels.stable

            If globalVariables.boolExtendedLoggingDuringUpdating = True Then
                Functions.eventLogFunctions.writeToSystemEventLog("Setting extractPDB flag to True.", EventLogEntryType.Information)
            End If

            extractPDB = True

            If globalVariables.boolExtendedLoggingDuringUpdating = True Then
                Functions.eventLogFunctions.writeToSystemEventLog("Setting overrideURLPaths flag to True.", EventLogEntryType.Information)
            End If

            overrideURLPaths = True
        End If

        If updateChannel = globalVariables.updateChannels.stable Then
            If globalVariables.boolExtendedLoggingDuringUpdating = True Then
                Functions.eventLogFunctions.writeToSystemEventLog("Downloading compressed application ZIP package into system RAM.", EventLogEntryType.Information)
            End If

            Dim urlToZipFile As String = globalVariables.webURLs.updateBranch.main.strProgramZIP
            Dim urlToZipFileSHA2 As String = globalVariables.webURLs.updateBranch.main.strProgramZIPSHA2

            If boolOverrideUserUpdateChannelPreferences And overrideURLPaths Then
                If globalVariables.boolExtendedLoggingDuringUpdating = True Then
                    Functions.eventLogFunctions.writeToSystemEventLog("Overriding URL paths for download in this update session.", EventLogEntryType.Information)
                End If

                urlToZipFile = globalVariables.webURLs.updateBranch.debug.strProgramZIP
                urlToZipFileSHA2 = globalVariables.webURLs.updateBranch.debug.strProgramZIPSHA2
            End If

            If Functions.http.downloadFile(urlToZipFile, memoryStream) = False Then
                memoryStream.Close()
                memoryStream.Dispose()
                memoryStream = Nothing

                giveDownloadErrorMessage()
                Exit Sub
            End If

            If globalVariables.boolExtendedLoggingDuringUpdating = True Then
                Functions.eventLogFunctions.writeToSystemEventLog("Compressed application ZIP package download complete. Now verifying compressed application ZIP package integrity.", EventLogEntryType.Information)
            End If

            If Functions.checksum.verifyChecksum(urlToZipFileSHA2, memoryStream, True) = False Then
                Functions.eventLogFunctions.writeToSystemEventLog("There was an error in the download of the program's ZIP file, checksums don't match. Update process aborted.", EventLogEntryType.Error)

                memoryStream.Close()
                memoryStream.Dispose()
                memoryStream = Nothing

                giveDownloadErrorMessage()
                Exit Sub
            End If

            If globalVariables.boolExtendedLoggingDuringUpdating = True Then
                Functions.eventLogFunctions.writeToSystemEventLog("Compressed application ZIP package integrity check passed.", EventLogEntryType.Information)
            End If
        ElseIf updateChannel = globalVariables.updateChannels.beta Or updateChannel = globalVariables.updateChannels.tom Then
            If globalVariables.boolExtendedLoggingDuringUpdating = True Then
                Functions.eventLogFunctions.writeToSystemEventLog("Downloading compressed application ZIP package into system RAM.", EventLogEntryType.Information)
            End If

            If Functions.http.downloadFile(globalVariables.webURLs.updateBranch.beta.strProgramZIP, memoryStream) = False Then
                memoryStream.Close()
                memoryStream.Dispose()
                memoryStream = Nothing

                giveDownloadErrorMessage()
                Exit Sub
            End If

            If globalVariables.boolExtendedLoggingDuringUpdating = True Then
                Functions.eventLogFunctions.writeToSystemEventLog("Compressed application ZIP package download complete. Now verifying compressed application ZIP package integrity.", EventLogEntryType.Information)
            End If

            If Functions.checksum.verifyChecksum(globalVariables.webURLs.updateBranch.beta.strProgramZIPSHA2, memoryStream, True) = False Then
                Functions.eventLogFunctions.writeToSystemEventLog("There was an error in the download of the program's ZIP file, checksums don't match. Update process aborted.", EventLogEntryType.Error)

                memoryStream.Close()
                memoryStream.Dispose()
                memoryStream = Nothing

                giveDownloadErrorMessage()
                Exit Sub
            End If

            If globalVariables.boolExtendedLoggingDuringUpdating = True Then
                Functions.eventLogFunctions.writeToSystemEventLog("Compressed application ZIP package integrity check passed.", EventLogEntryType.Information)
                Functions.eventLogFunctions.writeToSystemEventLog("Setting extractPDB flag to True.", EventLogEntryType.Information)
            End If

            extractPDB = True
        End If

        If globalVariables.boolExtendedLoggingDuringUpdating = True Then
            Functions.eventLogFunctions.writeToSystemEventLog("Download and verification of files required for update complete. Starting application update and extraction process.", EventLogEntryType.Information)
        End If

        Dim strNewApplicationFileNameFullName As String = New IO.FileInfo(Application.ExecutablePath).FullName & ".new.exe"

        If IO.File.Exists(strNewApplicationFileNameFullName) Then
            Try
                IO.File.Delete(strNewApplicationFileNameFullName)
            Catch ex As Exception
                Me.Invoke(Sub() closePleaseWaitPanel())
                MsgBox("An existing new program executable file has been found, we tried to delete it but we couldn't. Please see the Application Event Log for more details.", MsgBoxStyle.Critical, strMessageBoxTitle)
                Functions.eventLogFunctions.writeToSystemEventLog("An existing new program executable file has been found, we tried to delete it but we couldn't.", EventLogEntryType.Error)
                Functions.eventLogFunctions.writeCrashToEventLog(ex)
                Exit Sub
            End Try
        End If

        If globalVariables.boolExtendedLoggingDuringUpdating = True Then
            Functions.eventLogFunctions.writeToSystemEventLog("Setting position for the IO.MemoryStream() back to the beginning of the stream to ready it for file extraction.", EventLogEntryType.Information)
        End If

        memoryStream.Position = 0

        If globalVariables.boolExtendedLoggingDuringUpdating = True Then
            Functions.eventLogFunctions.writeToSystemEventLog("Opening ZIP file package in system RAM for file extractions.", EventLogEntryType.Information)
        End If

        Dim zipFileObject As New ZipFile(memoryStream) ' Create a new ZIPFile Object.

        If extractPDB = True Then
            If Functions.support.extractUpdatedFileFromZIPPackage(zipFileObject, globalVariables.pdbFileNameInZIP, globalVariables.pdbFileNameInZIP & ".new") = False Then
                ' This code executes only if the file extraction from the ZIP file fails.
                Functions.eventLogFunctions.writeToSystemEventLog("There was an issue extracting data from the downloaded ZIP file.", EventLogEntryType.Error)
                MsgBox("There was an issue extracting data from the downloaded ZIP file.", MsgBoxStyle.Critical, strMessageBoxTitle) ' Gives some feedback.

                If globalVariables.boolExtendedLoggingDuringUpdating = True Then
                    Functions.eventLogFunctions.writeToSystemEventLog("Closing out ZIP File Object and freeing up memory.", EventLogEntryType.Information)
                End If

                zipFileObject.Close() ' This closes our ZIPFile Object.
                memoryStream.Close()
                memoryStream.Dispose()
                memoryStream = Nothing

                Functions.eventLogFunctions.writeToSystemEventLog("Update proceess failed, aborting update procedure.", EventLogEntryType.Error)

                Exit Sub ' Exits the routine
            End If
        End If

        If Functions.support.extractUpdatedFileFromZIPPackage(zipFileObject, globalVariables.programFileNameInZIP, strNewApplicationFileNameFullName) = False Then
            ' This code executes only if the file extraction from the ZIP file fails.
            Functions.eventLogFunctions.writeToSystemEventLog("There was an issue extracting data from the downloaded ZIP file.", EventLogEntryType.Error)
            MsgBox("There was an issue extracting data from the downloaded ZIP file.", MsgBoxStyle.Critical, strMessageBoxTitle) ' Gives some feedback.

            If globalVariables.boolExtendedLoggingDuringUpdating = True Then
                Functions.eventLogFunctions.writeToSystemEventLog("Closing out ZIP File Object and freeing up memory.", EventLogEntryType.Information)
            End If

            zipFileObject.Close() ' This closes our ZIPFile Object.
            memoryStream.Close()
            memoryStream.Dispose()
            memoryStream = Nothing

            Functions.eventLogFunctions.writeToSystemEventLog("Update proceess failed, aborting update procedure.", EventLogEntryType.Error)

            Exit Sub ' Exits the routine
        End If

        If globalVariables.boolExtendedLoggingDuringUpdating = True Then
            Functions.eventLogFunctions.writeToSystemEventLog("Closing out ZIP File Object and freeing up memory.", EventLogEntryType.Information)
        End If

        zipFileObject.Close() ' This closes our ZIPFile Object.
        memoryStream.Close()
        memoryStream.Dispose()
        memoryStream = Nothing

        If globalVariables.boolExtendedLoggingDuringUpdating = True Then
            Functions.eventLogFunctions.writeToSystemEventLog("Making sure new executable file exists.", EventLogEntryType.Information)
        End If

        If IO.File.Exists(strNewApplicationFileNameFullName) = True Then
            If globalVariables.boolExtendedLoggingDuringUpdating = True Then
                Functions.eventLogFunctions.writeToSystemEventLog("New executable exists, executing it in update mode.", EventLogEntryType.Information)
            End If

            Process.Start(New ProcessStartInfo With {.FileName = strNewApplicationFileNameFullName, .Arguments = "-update", .Verb = "runas"})
            Process.GetCurrentProcess.Kill()
        Else
            Functions.eventLogFunctions.writeToSystemEventLog("New executable doesn't exists, update process aborted.", EventLogEntryType.Error)
            MsgBox("Something went wrong during the download, update process aborted.", MsgBoxStyle.Critical, strMessageBoxTitle)
        End If
    End Sub

    Private Sub disableAutomaticUpdatesAndNotifyUser()
        Dim msgBoxResult As MsgBoxResult = MsgBox("Since you have told the program that you didn't want to update to the newest supported version, do you want to also disable Automatic Update Checking?" & vbCrLf & vbCrLf & "By disabling Automatic Update Checking you will no longer be notified about new versions of this program, that is, unless you manually check for updates." & vbCrLf & vbCrLf & "Do you want to disable Automatic Checking for Updates?", MsgBoxStyle.Question + MsgBoxStyle.YesNo, strMessageBoxTitle)

        If msgBoxResult = MsgBoxResult.Yes Then
            My.Settings.CheckForUpdates = False
            ConfigureAutomaticUpdatesToolStripMenuItem.Visible = False
            toolStripAutomaticallyCheckForUpdates.Checked = False
        End If
    End Sub

    Private Sub openThePleaseWaitWindowAndStartTheDownloadThread(Optional boolOverrideUserUpdateChannelPreferences As Boolean = False)
        Me.Invoke(Sub() openPleaseWaitPanel("Downloading update... Please Wait."))

        Try
            downloadAndDoTheUpdate(boolOverrideUserUpdateChannelPreferences)
        Catch ex As Threading.ThreadAbortException
        End Try
    End Sub

    Private Sub userInitiatedCheckForUpdates()
        My.Settings.ProgramExecutionsSinceLastUpdateCheck = 0 ' We reset the number of program executions since last update to 0.
        My.Settings.Save() ' And save the settings to disk.

        ' First we check to see if we have an Internet connection because if we don't then why even bother trying to check for updates.
        If Functions.http.checkForInternetConnection() = False Then
            toolStripCheckForUpdates.Enabled = True ' Re-Enable the toolStripCheckForUpdates button.
            MsgBox("No Internet connection detected.", MsgBoxStyle.Information, strMessageBoxTitle) ' Tell the user that we don't have an Internet connection.
        Else
            Try
                checkForUpdatesSubRoutine()
            Catch ex As Exception
                Functions.eventLogFunctions.writeCrashToEventLog(ex) ' If anything goes wrong during this routine, handle it as an exception.
            Finally
                toolStripCheckForUpdates.Enabled = True
            End Try
        End If
    End Sub

    Private Sub checkForUpdatesSubRoutine()
        ' Create some variables.
        Dim xmlData As String = Nothing
        Dim remoteVersion As String = Nothing
        Dim remoteBuild As String = Nothing
        Dim changeLog As String = Nothing
        Dim strRemoteBetaRCVersion As String = Nothing
        Dim updateType As Functions.support.updateType = Functions.support.updateType.null
        Dim boolOverrideUserUpdateChannelPreferences As Boolean = False
        Dim boolSetTriggerUpdateAtNextRuntimeSetting As Boolean = False

        ' Now let's set up an httpHelper Class Instance to check for updates with, we will use this Class instance for all HTTP operations in this routine
        Dim httpHelper As httpHelper = Functions.http.createNewHTTPHelperObject()

        ' And let's get the info from my web site.
        Try
            If httpHelper.getWebData(globalVariables.webURLs.core.strProgramUpdateChecker, xmlData) = False Then
                ' Something went wrong, checking for updates failed. Let's tell the user that and abort the routine.
                MsgBox("There was an error checking for a software update; update check aborted.", MsgBoxStyle.Information, strMessageBoxTitle)
                Exit Sub
            End If
        Catch ex As Exception
            Exit Sub
        End Try

        If Functions.support.processUpdateXMLData(xmlData, updateType, remoteVersion, remoteBuild, strRemoteBetaRCVersion) Then
            If My.Settings.ignoreThisVersion = String.Format("{0} Build {1}", remoteVersion, remoteBuild) Then
                Functions.eventLogFunctions.writeToSystemEventLog(String.Format("There was an update but you have chosen to ignore it ({0}).", My.Settings.ignoreThisVersion), EventLogEntryType.Information)
                Exit Sub
            End If

            ' Creates a variable to store the user's response to the update notification window.
            Dim updateDialogResponse As Update_Message.userResponse

            ' This checks to see two of the following conditions are met...
            ' 1. If the update channel is set to beta or tom.
            ' 2. If the remote version (5.8, 5.9, etc.) does not equal the current version of the program (5.8, 5.9, etc.).
            ' If both conditions are met then that means that a new .1 version has been released (5.8, 5.9, etc.) and that
            ' we need to upgrade the user to the latest release branch version first.
            If (My.Settings.updateChannel.Equals(globalVariables.updateChannels.beta, StringComparison.OrdinalIgnoreCase) Or My.Settings.updateChannel.Equals(globalVariables.updateChannels.tom, StringComparison.OrdinalIgnoreCase)) And remoteVersion <> globalVariables.version.versionStringWithoutBuild Then
                ' OK, both conditions were met so we need to set some stuff up for later use in this function.

                boolSetTriggerUpdateAtNextRuntimeSetting = True ' We need to tell this sub-routine to set a trigger in the Registry that triggers an update check at the next program launch.
                boolOverrideUserUpdateChannelPreferences = True ' We need to override the update channel that the user has so we set this value to True.

                updateType = Functions.support.updateType.release ' We override the update type to release.
            End If

            ' At this point, if the conditional statement above didn't work out and the code inside the
            ' statement didn't execute then we simply go onto the rest of the update code that is below.

            If updateType = Functions.support.updateType.beta Then
                updateDialogResponse = openUpdateDialog(Update_Message.versionUpdateType.betaVersionUpdate, remoteVersion, remoteBuild, strRemoteBetaRCVersion)
            ElseIf updateType = Functions.support.updateType.candidate Then
                updateDialogResponse = openUpdateDialog(Update_Message.versionUpdateType.releaseCandidateVersionUpdate, remoteVersion, remoteBuild, strRemoteBetaRCVersion)
            ElseIf updateType = Functions.support.updateType.release Then
                If remoteVersion = globalVariables.version.versionStringWithoutBuild Then
                    updateDialogResponse = openUpdateDialog(Update_Message.versionUpdateType.standardVersionUpdate, remoteVersion, remoteBuild, strRemoteBetaRCVersion)
                Else
                    updateDialogResponse = openUpdateDialog(Update_Message.versionUpdateType.totallyNewVersionUpdate, remoteVersion, remoteVersion, strRemoteBetaRCVersion)
                End If
            End If

            ' Checks to see if the user said yes to update.
            If updateDialogResponse = Update_Message.userResponse.doTheUpdate Then
                If boolSetTriggerUpdateAtNextRuntimeSetting Then
                    Functions.registryStuff.setBooleanValueInRegistry("UpdateAtNextRunTime", True)
                End If

                ' The user said yes.
                openThePleaseWaitWindowAndStartTheDownloadThread(boolOverrideUserUpdateChannelPreferences) ' Starts the download and update thread.
            ElseIf updateDialogResponse = Update_Message.userResponse.dontDoTheUpdate Then
                Functions.eventLogFunctions.writeToSystemEventLog("There was an update but you have chosen not download it.", EventLogEntryType.Information)
                giveFeedbackToUser("The update will not be downloaded.")
            End If

            Exit Sub ' Exit the routine.
        Else
            If updateType = Functions.support.updateType.buildLessThanError Then
                Functions.eventLogFunctions.writeToSystemEventLog("A software update check was performed and something weird happened. Your current version is newer than what is listed on the web site.") ' Log it.
                giveFeedbackToUser("Something weird happened. Your current version is newer than what is listed on the web site.") ' Gives feedback.
            Else
                Functions.eventLogFunctions.writeToSystemEventLog("A software update check was performed and it's been determined that you already have the latest version.") ' Log it.
                giveFeedbackToUser("You already have the latest version.") ' Gives feedback.
            End If

            Exit Sub ' Exits the routine.
        End If
    End Sub

    ''' <summary>This function runs at window load time to check for updates.</summary>
    ''' <param name="forceRunOfUpdate">This tells the routine if it should run the routine even if the user has automatic checking for updates disabled.</param>
    Private Sub formLoadCheckForUpdatesRoutine(Optional forceRunOfUpdate As Boolean = False)
        ' This checks to see if automatic checking for updates is enabled or not, if so this routine runs. There's also
        ' a way to force this routine to run and that is by passing a True value for the forceRunOfUpdate variable.
        If My.Settings.CheckForUpdates = True Or forceRunOfUpdate = True Then
            toolStripAutomaticallyCheckForUpdates.Checked = True ' This puts a checkbox for the toolStripAutomaticallyCheckForUpdates checkbox.
            Dim longDateDiff As Long = Math.Abs(DateDiff(DateInterval.Day, Now, My.Settings.lastUpdateTime)) ' This determinds the amount of days since the last update check.

            ' This checks to see if the number of executions of the program has exceeded 50 times or if the days since last update is greater than the user specified days interval for update checking. It also has the ability to bypass this check by setting the forceRunOfUpdate variable to True.
            If My.Settings.ProgramExecutionsSinceLastUpdateCheck >= 50 Or longDateDiff >= My.Settings.checkForUpdatesEveryInDays Or forceRunOfUpdate = True Then
                My.Settings.ProgramExecutionsSinceLastUpdateCheck = 0 ' We reset the number of program executions since last update to 0.
                My.Settings.Save() ' And save the settings to disk.

                ' First we check to see if we have an Internet connection because if we don't then why even bother trying to check for updates.
                If Functions.http.checkForInternetConnection() = False Then
                    MsgBox("No Internet connection detected.", MsgBoxStyle.Information, strMessageBoxTitle) ' Tell the user that we don't have an Internet connection.
                    Exit Sub ' And exit this routine.
                Else
                    My.Settings.lastUpdateTime = Now ' Set the last update time to the current time.
                    My.Settings.Save() ' And save the settings to disk.

                    Try
                        checkForUpdatesSubRoutine()
                    Catch ex As Exception
                        Functions.eventLogFunctions.writeCrashToEventLog(ex) ' If anything goes wrong during this routine, handle it as an exception.
                    End Try
                End If
            Else
                My.Settings.ProgramExecutionsSinceLastUpdateCheck += 1 ' Increments the ProgramExecutionsSinceLastUpdateCheck counter.
                My.Settings.Save() ' Saves to disk.
            End If
        Else
            toolStripAutomaticallyCheckForUpdates.Checked = False
        End If
    End Sub

    Private Function calculateRestorePointAge(creationDate As Date) As Double
        Return Math.Round(Math.Abs(Now.Subtract(creationDate).TotalDays), My.Settings.roundRestorePointAgeNumber)
    End Function

    Private Sub loadRestorePointsFromSystemIntoList() 'Adds all Restore Points to a ListView
        ' Declares some variables.
        Dim systemRestoreIDs As New ArrayList ' Creates an ArrayList for us to put our System Restore IDs into for later checking for the newest System Restore Point ID.
        Dim systemRestorePointsManagementObjectSearcher As ManagementObjectSearcher
        Dim listViewItem As myListViewItemTypes.restorePointEntryItem
        Dim listOfRestorePoints As New List(Of myListViewItemTypes.restorePointEntryItem)
        Dim restorePointAge As Double
        Dim stopWatch As Stopwatch = Stopwatch.StartNew()

        Try
            systemRestorePointsList.Enabled = False
            systemRestorePointsList.Items.Clear() ' Clears the System Restore Points list on the GUI of existing items.

            newestSystemRestoreID = 0 ' Resets the newest System Restore ID to 0.

            ' Get all System Restore Points from the Windows Management System and puts then in the systemRestorePoints variable.
            systemRestorePointsManagementObjectSearcher = New ManagementObjectSearcher("root\DEFAULT", "SELECT * FROM SystemRestore")

            If systemRestorePointsManagementObjectSearcher IsNot Nothing Then
                Dim restorePointsOnSystemManagementObjectCollection As ManagementObjectCollection = systemRestorePointsManagementObjectSearcher.Get()

                If restorePointsOnSystemManagementObjectCollection IsNot Nothing Then
                    ' Checks to see if there are any System Restore Points to be listed.
                    If (restorePointsOnSystemManagementObjectCollection.Count = 0) = False Then
                        'Dim index As Integer = 0

                        ' Loops through systemRestorePoints.
                        For Each restorePointDetails As ManagementObject In restorePointsOnSystemManagementObjectCollection
                            If (restorePointDetails("SequenceNumber") IsNot Nothing) And (restorePointDetails("CreationTime") IsNot Nothing) And (restorePointDetails("Description") IsNot Nothing) And (restorePointDetails("RestorePointType") IsNot Nothing) Then
                                'index += 1

                                ' Adds a System Restore Point to a list of System Restore Points with the Restore Point ID as a Key
                                listViewItem = New myListViewItemTypes.restorePointEntryItem() With {
                                    .Text = restorePointDetails("SequenceNumber").ToString,
                                    .strRestorePointID = restorePointDetails("SequenceNumber").ToString.Trim
                                }

                                If Not Integer.TryParse(listViewItem.strRestorePointID, listViewItem.intRestorePointID) Then
                                    Throw New Functions.myExceptions.integerTryParseException() With {.strThatCouldNotBeParsedIntoAnInteger = listViewItem.strRestorePointID}
                                End If

                                ' Adds the System Restore Point ID to our list of System Restore Point IDs to calculate the newest System Restore Point.
                                systemRestoreIDs.Add(Integer.Parse(restorePointDetails("SequenceNumber")))

                                listViewItem.SubItems.Add(restorePointDetails("Description").ToString)
                                listViewItem.strRestorePointName = restorePointDetails("Description").ToString

                                If String.IsNullOrEmpty(restorePointDetails("CreationTime").ToString.Trim) = False Then
                                    listViewItem.dateRestorePointDate = Functions.restorePointStuff.parseSystemRestorePointCreationDate(restorePointDetails("CreationTime"))
                                    listViewItem.strRestorePointDate = String.Format("{0} {1}", listViewItem.dateRestorePointDate.ToShortDateString, listViewItem.dateRestorePointDate.ToLongTimeString)

                                    listViewItem.SubItems.Add(listViewItem.strRestorePointDate)

                                    restorePointAge = calculateRestorePointAge(listViewItem.dateRestorePointDate)
                                Else
                                    restorePointAge = 0
                                    listViewItem.SubItems.Add("(Error Parsing Date)")
                                End If

                                If restorePointDetails("Description").ToString.caseInsensitiveContains("windows update") Then
                                    If My.Settings.debug = True Then
                                        listViewItem.strRestorePointType = "Windows Update" & " (" & restorePointDetails("RestorePointType").ToString & ")"
                                    Else
                                        listViewItem.strRestorePointType = "Windows Update"
                                    End If
                                ElseIf restorePointDetails("Description").ToString.caseInsensitiveContains("system checkpoint") Then
                                    If My.Settings.debug = True Then
                                        listViewItem.strRestorePointType = "System Checkpoint" & " (" & restorePointDetails("RestorePointType").ToString & ")"
                                    Else
                                        listViewItem.strRestorePointType = "System Checkpoint"
                                    End If
                                Else
                                    If My.Settings.debug = True Then
                                        listViewItem.strRestorePointType = Functions.restorePointStuff.whatTypeOfRestorePointIsIt(Integer.Parse(restorePointDetails("RestorePointType").ToString)) & " (" & restorePointDetails("RestorePointType").ToString & ")"
                                    Else
                                        listViewItem.strRestorePointType = Functions.restorePointStuff.whatTypeOfRestorePointIsIt(Integer.Parse(restorePointDetails("RestorePointType").ToString))
                                    End If
                                End If

                                listViewItem.SubItems.Add(listViewItem.strRestorePointType)
                                listViewItem.SubItems.Add(restorePointAge.ToString)
                                listViewItem.strRestorePointAge = restorePointAge.ToString

                                ' Adds the item to the list.
                                listOfRestorePoints.Add(listViewItem)
                                listViewItem = Nothing
                            End If
                        Next

                        systemRestorePointsManagementObjectSearcher.Dispose()
                        systemRestorePointsManagementObjectSearcher = Nothing

                        If listOfRestorePoints.Count = 0 Then
                            newestSystemRestoreID = 0
                        Else
                            stopWatch.Stop()
                            If stopWatch.Elapsed.Milliseconds < 1000 Then Threading.Thread.Sleep(1000 - stopWatch.Elapsed.Milliseconds)

                            ' Adds the list of System Restore Points that we created earlier in this routine to the System Restore Points list on the GUI.
                            systemRestorePointsList.Items.AddRange(listOfRestorePoints.ToArray())

                            ' Does some sorting on the System Restore Points list on the GUI.
                            systemRestorePointsList.Sort()

                            If systemRestoreIDs.Count <> 0 Then
                                ' First, we convert the ArrayList into an Integer then calculate the Max value of all of the Integers in the Integer Array.
                                ' This gets the latest System Restore Point ID for later checking to see if the user is deleting the newest System Restore Point.
                                newestSystemRestoreID = DirectCast(systemRestoreIDs.ToArray(GetType(Integer)), Integer()).Max
                            End If

                            For Each itemInList As myListViewItemTypes.restorePointEntryItem In systemRestorePointsList.Items
                                If itemInList.intRestorePointID = newestSystemRestoreID Then
                                    itemInList.Font = New Font(btnCreate.Font.FontFamily, btnCreate.Font.SizeInPoints, FontStyle.Bold)
                                End If
                            Next
                        End If
                    Else
                        newestSystemRestoreID = 0
                    End If

                    restorePointsOnSystemManagementObjectCollection.Dispose()
                    restorePointsOnSystemManagementObjectCollection = Nothing
                Else
                    newestSystemRestoreID = 0
                End If
            End If
        Catch ex8 As IO.FileNotFoundException
            closePleaseWaitPanel()
            MsgBox("There has been an error while trying to load the System Restore Points on your system." & vbCrLf & vbCrLf & "Please go to System Restore Point Utilities and click on Manually Fix System Restore and follow the prompts. It will ask you reboot your computer so please save all work before you do so.", MsgBoxStyle.Critical, strMessageBoxTitle)
        Catch ex7 As Functions.myExceptions.integerTryParseException
            closePleaseWaitPanel()
            Functions.eventLogFunctions.writeCrashToEventLog(ex7)
            MsgBox("There was a serious error while loading the restore points on your system. The restore point IDs appear to have been mangled and can't be parsed into an Integer like it should be. The loading of restore points has been halted." & vbCrLf & vbCrLf & "The value that was returned by the system was... " & ex7.strThatCouldNotBeParsedIntoAnInteger, MsgBoxStyle.Critical, strMessageBoxTitle)
        Catch ex6 As ObjectDisposedException
            Functions.eventLogFunctions.writeCrashToEventLog(ex6)

            closePleaseWaitPanel()

            Exit Sub
        Catch ex5 As UnauthorizedAccessException
            Functions.eventLogFunctions.writeCrashToEventLog(ex5)
        Catch ex4 As ManagementException
            Functions.eventLogFunctions.writeCrashToEventLog(ex4)
        Catch ex As Threading.ThreadAbortException
            ' Does nothing
        Catch ex3 As COMException
            Functions.wmi.giveComExceptionCrashMessage()
        Catch ex2 As Exception
            Threading.Thread.CurrentThread.CurrentUICulture = New Globalization.CultureInfo("en-US")
            exceptionHandler.manuallyLoadCrashWindow(ex2, ex2.Message, ex2.StackTrace, ex2.GetType)
        Finally
            closePleaseWaitPanel()

            systemRestorePointsList.Enabled = True

            ' Cleans up some memory.
            systemRestorePointsManagementObjectSearcher = Nothing
            listOfRestorePoints = Nothing
            listViewItem = Nothing

            lblCurrentRestorePointsLabel.Text = String.Format("Current Restore Points ({0})", systemRestorePointsList.Items.Count)

            btnRefreshRestorePoints.Text = "Refresh List of System Restore Points"

            btnRestoreToRestorePoint.Enabled = False
            stripRestore.Enabled = False

            stripRestoreSafeMode.Enabled = False
            btnRestoreToRestorePointSafeMode.Enabled = False

            btnDeleteRestorePoint.Enabled = False
            stripDelete.Enabled = False
        End Try
    End Sub

    Private Sub restoreSystemRestorePoint(systemRestorePointIndex As Integer)
        Try
            disableFormElements()
            Functions.wmi.restoreToSystemRestorePoint(systemRestorePointIndex)
            closePleaseWaitPanel()
        Catch ex2 As Threading.ThreadAbortException
            ' Does nothing.
        Catch ex As Exception
            MsgBox("Unable to restore system back to user defined System Restore Point." & vbCrLf & vbCrLf & ex.Message, MsgBoxStyle.Critical, "Critical Error")
        Finally
            enableFormElements()
        End Try
    End Sub

    Private Sub afterDeleteSelectedRestorePoints(restorePointsToBeDeleted As Dictionary(Of String, restorePointInfo))
        closePleaseWaitPanel()

        Dim boolMultiMode As Boolean = False

        If restorePointsToBeDeleted.Count > 1 Then boolMultiMode = True

        For Each itemInList As myListViewItemTypes.restorePointEntryItem In systemRestorePointsList.Items
            If restorePointsToBeDeleted.ContainsKey(itemInList.strRestorePointID) Then systemRestorePointsList.Items.Remove(itemInList)
        Next

        systemRestorePointsList.Enabled = True
        lblCurrentRestorePointsLabel.Text = String.Format("Current Restore Points ({0})", systemRestorePointsList.Items.Count)

        If ShowMessageBoxAfterSuccessfulDeletionOfRestorePointsToolStripMenuItem.Checked Then
            If boolMultiMode Then
                giveFeedbackToUser(restorePointsToBeDeleted.Count & " System Restore Points were deleted.")
            Else
                giveFeedbackToUser("One System Restore Point was deleted.")
            End If
        End If
    End Sub

    Private Sub deleteSelectedRestorePoints(restorePointsToBeDeleted As Dictionary(Of String, restorePointInfo), boolEnableLogging As Boolean)
        Try
            Dim intRestorePointID As Integer
            Dim restorePointCreationDate As Date
            Dim intOldNumberOfRestorePoints As Integer = Functions.wmi.getNumberOfRestorePoints()
            Dim boolMultiMode As Boolean = False

            If restorePointsToBeDeleted.Count > 1 Then boolMultiMode = True

            For Each restorePointInfo As KeyValuePair(Of String, restorePointInfo) In restorePointsToBeDeleted
                If Integer.TryParse(restorePointInfo.Key, intRestorePointID) Then
                    If boolEnableLogging Then
                        restorePointCreationDate = restorePointInfo.Value.dateCreated

                        Functions.eventLogFunctions.writeToSystemEventLog(String.Format("The user {3}/{4} deleted the restore point named ""{0}"" which was created on {1} at {2}.", restorePointInfo.Value.strName, restorePointCreationDate.ToShortDateString, restorePointCreationDate.ToShortTimeString, Environment.MachineName, Environment.UserName), EventLogEntryType.Information)
                    End If

                    intOldNumberOfRestorePoints -= 1
                    Functions.APIs.NativeMethods.SRRemoveRestorePoint(intRestorePointID) ' Deletes the Restore Point.
                End If
            Next

            While intOldNumberOfRestorePoints <> Functions.wmi.getNumberOfRestorePoints()
                Threading.Thread.Sleep(500)
            End While

            Invoke(Sub() afterDeleteSelectedRestorePoints(restorePointsToBeDeleted))
        Catch ex1 As Threading.ThreadAbortException
            Invoke(Sub() MsgBox("System Restore Point Deletion Process Aborted.", MsgBoxStyle.Information, strMessageBoxTitle))
        Catch ex2 As ArgumentOutOfRangeException
            Functions.eventLogFunctions.writeCrashToEventLog(ex2)
        Catch ex3 As Exception
            Threading.Thread.CurrentThread.CurrentUICulture = New Globalization.CultureInfo("en-US")
            exceptionHandler.manuallyLoadCrashWindow(ex3, ex3.Message, ex3.StackTrace, ex3.GetType)
        End Try
    End Sub

    Private Sub deleteOldRestorePoints(maxAgeInput As Short)
        Try
            ' Get all System Restore Points from the Windows Management System and puts then in the systemRestorePoints variable.
            Dim systemRestorePoints As ManagementObjectSearcher = New ManagementObjectSearcher("root\DEFAULT", "SELECT * FROM SystemRestore")
            Dim oldNumberOfRestorePoints As Integer = systemRestorePoints.Get().Count
            Dim systemRestorePointCreationDate As Date
            Dim dateDiffResults As Short
            Dim numberOfOldRestorePointsDeleted As Short = 0

            If oldNumberOfRestorePoints = 0 Then
                toolStripDeleteOldRestorePoints.Enabled = True
                btnDeleteRestorePoint.Enabled = True
                stripDelete.Enabled = True
                closePleaseWaitPanel()
                Exit Sub
            End If

            If toolStripLogRestorePointDeletions.Checked Then
                If maxAgeInput = 1 Then
                    Functions.eventLogFunctions.writeToSystemEventLog(String.Format("The user {0}\{1} began batch processing and deletion of System Restore Points older than 1 day.", Environment.MachineName, Environment.UserName), EventLogEntryType.Information)
                Else
                    Functions.eventLogFunctions.writeToSystemEventLog(String.Format("The user {0}\{1} began batch processing and deletion of System Restore Points older than {2} days.", Environment.MachineName, Environment.UserName, maxAgeInput), EventLogEntryType.Information)
                End If
            End If

            ' Loops through systemRestorePoints.
            For Each systemRestorePoint As ManagementObject In systemRestorePoints.Get()
                If systemRestorePoint("CreationTime") IsNot Nothing Then
                    systemRestorePointCreationDate = Functions.restorePointStuff.parseSystemRestorePointCreationDate(systemRestorePoint("CreationTime"), True)

                    dateDiffResults = Math.Abs(DateDiff(DateInterval.Day, systemRestorePointCreationDate, Date.Now))

                    If dateDiffResults >= maxAgeInput Then
                        Functions.APIs.NativeMethods.SRRemoveRestorePoint(Integer.Parse(systemRestorePoint("SequenceNumber").ToString)) ' Deletes the Restore Point.

                        If toolStripLogRestorePointDeletions.Checked Then
                            numberOfOldRestorePointsDeleted += 1
                            Functions.eventLogFunctions.writeToSystemEventLog(String.Format("Deleted Restore Point named ""{0}"" which was created on {1} at {2}.", systemRestorePoint("Description"), systemRestorePointCreationDate.ToLongDateString, systemRestorePointCreationDate.ToShortTimeString), EventLogEntryType.Information)
                        End If
                    End If

                    systemRestorePointCreationDate = Nothing
                End If

                systemRestorePoint.Dispose()
                systemRestorePoint = Nothing
            Next

            lblCurrentRestorePointsLabel.Text = String.Format("Current Restore Points ({0})", systemRestorePointsList.Items.Count)

            If toolStripLogRestorePointDeletions.Checked Then
                If numberOfOldRestorePointsDeleted = 0 Then
                    Functions.eventLogFunctions.writeToSystemEventLog("End of processing old System Restore Points. No old System Restore Point were deleted.", EventLogEntryType.Information)
                ElseIf numberOfOldRestorePointsDeleted = 1 Then
                    Functions.eventLogFunctions.writeToSystemEventLog("End of processing old System Restore Points. 1 old System Restore Point was deleted.", EventLogEntryType.Information)
                Else
                    Functions.eventLogFunctions.writeToSystemEventLog(String.Format("End of processing old System Restore Points. {0} old System Restore Points were deleted.", numberOfOldRestorePointsDeleted), EventLogEntryType.Information)
                End If
            End If

            systemRestorePoints.Dispose()
            systemRestorePoints = Nothing

            loadRestorePointsFromSystemIntoList()
        Catch ex As Threading.ThreadAbortException
            ' Does nothing
        Catch ex2 As Exception
            Threading.Thread.CurrentThread.CurrentUICulture = New Globalization.CultureInfo("en-US")
            exceptionHandler.manuallyLoadCrashWindow(ex2, ex2.Message, ex2.StackTrace, ex2.GetType)
        Finally
            toolStripDeleteOldRestorePoints.Enabled = True
            btnDeleteRestorePoint.Enabled = True
            stripDelete.Enabled = True
            closePleaseWaitPanel()
        End Try
    End Sub
#End Region

#Region "--== Misc. Event Code ==--"
    Private Sub txtRestorePointDescription_Click(sender As Object, e As EventArgs) Handles txtRestorePointDescription.Click
        txtRestorePointDescription.ForeColor = Color.Black
        txtRestorePointDescription.Text = ""
    End Sub

    Private Sub txtRestorePointDescription_Leave(sender As Object, e As EventArgs) Handles txtRestorePointDescription.Leave
        doTheGrayingOfTheRestorePointNameTextBox()
    End Sub

    Private Sub txtRestorePointDescription_LostFocus(sender As Object, e As EventArgs) Handles txtRestorePointDescription.LostFocus
        doTheGrayingOfTheRestorePointNameTextBox()
    End Sub

    Private Sub txtRestorePointDescription_TextChanged(sender As Object, e As EventArgs) Handles txtRestorePointDescription.TextChanged
        If txtRestorePointDescription.Text.caseInsensitiveContains(strTypeYourRestorePointName) = False And txtRestorePointDescription.Text.Trim IsNot Nothing And Functions.support.areWeInSafeMode() = False Then
            btnCreate.Enabled = True
        ElseIf txtRestorePointDescription.Text.caseInsensitiveContains(strTypeYourRestorePointName) = True Then
            doTheGrayingOfTheRestorePointNameTextBox()
        End If
    End Sub

    Private Sub txtRestorePointDescription_KeyUp(sender As Object, e As KeyEventArgs) Handles txtRestorePointDescription.KeyUp
        If e.KeyCode = Keys.Enter And btnCreate.Enabled = True And txtRestorePointDescription.Text.Trim IsNot Nothing And Functions.support.areWeInSafeMode() = False Then
            btnCreate.PerformClick()
        End If
    End Sub

    Private Sub TableLayoutPanel2_Click(sender As Object, e As EventArgs) Handles buttonTableLayout.Click
        doTheGrayingOfTheRestorePointNameTextBox()
    End Sub

    Private Sub lblCurrentRestorePointsLabel_Click(sender As Object, e As EventArgs) Handles lblCurrentRestorePointsLabel.Click
        doTheGrayingOfTheRestorePointNameTextBox()
    End Sub

    Private Sub StatusStrip1_Click(sender As Object, e As EventArgs)
        doTheGrayingOfTheRestorePointNameTextBox()
    End Sub

    Private Sub NotifyIcon1_DoubleClick(sender As Object, e As EventArgs) Handles NotifyIcon1.DoubleClick
        Try
            Me.WindowState = FormWindowState.Normal
            Me.BringToFront()
            Me.BringToFront()

            'AppActivate(Me.Text)
            AppActivate(Process.GetCurrentProcess.Id)
        Catch ex As Exception
            ' Does nothing
        End Try
    End Sub

    Private Sub Form1_Click(sender As Object, e As EventArgs) Handles Me.Click
        txtRestorePointDescription.ForeColor = Color.DimGray
        txtRestorePointDescription.Text = strTypeYourRestorePointName
        btnCreate.Enabled = False
    End Sub

    Private Sub systemRestorePointsList_SelectedIndexChanged(sender As Object, e As EventArgs) Handles systemRestorePointsList.SelectedIndexChanged
        If systemRestorePointsList.SelectedItems.Count > 1 Then
            btnRestoreToRestorePoint.Enabled = False
            stripRestore.Enabled = False

            stripRestoreSafeMode.Enabled = False
            btnRestoreToRestorePointSafeMode.Enabled = False

            ToolTip.SetToolTip(btnRestoreToRestorePoint, "Disabled because you can't have multiple Restore Points to use the Restore function.")
        Else
            btnRestoreToRestorePoint.Enabled = True
            stripRestore.Enabled = True

            If Functions.support.areWeInSafeMode = False Then
                stripRestoreSafeMode.Enabled = True
                btnRestoreToRestorePointSafeMode.Enabled = True
            End If

            ToolTip.SetToolTip(btnRestoreToRestorePoint, "")
        End If

        If systemRestorePointsList.SelectedItems.Count = 1 Then
            btnDeleteRestorePoint.Text = "Delete Selected Restore Point"
            stripDelete.Text = "&Delete Selected Restore Point"
        Else
            btnDeleteRestorePoint.Text = "Delete Selected Restore Points"

            stripDelete.Text = "&Delete Selected Restore Points"
        End If

        btnDeleteRestorePoint.Enabled = True
        stripDelete.Enabled = True
        ToolTip.SetToolTip(btnDeleteRestorePoint, "")

        If systemRestorePointsList.SelectedItems.Count = 1 Then
            btnRestoreToRestorePoint.Enabled = True
            stripRestore.Enabled = True

            If Functions.support.areWeInSafeMode = False Then
                stripRestoreSafeMode.Enabled = True
                btnRestoreToRestorePointSafeMode.Enabled = True
            End If

            btnDeleteRestorePoint.Enabled = True
            stripDelete.Enabled = True
        ElseIf systemRestorePointsList.SelectedItems.Count = 0 Then
            btnRestoreToRestorePoint.Enabled = False
            stripRestore.Enabled = False
            stripRestoreSafeMode.Enabled = False
            btnRestoreToRestorePointSafeMode.Enabled = False

            btnDeleteRestorePoint.Enabled = False
            stripDelete.Enabled = False
        End If

        If AllowForDeletionOfAllSystemRestorePointsToolStripMenuItem.Checked = False Then
            For Each iteminlist As myListViewItemTypes.restorePointEntryItem In systemRestorePointsList.SelectedItems
                If iteminlist.intRestorePointID = newestSystemRestoreID Then
                    btnDeleteRestorePoint.Enabled = False
                    stripDelete.Enabled = False
                    ToolTip.SetToolTip(btnDeleteRestorePoint, "Disabled because you have the latest System Restore Point selected as part of the group of selected System Restore Points.")
                    Exit Sub
                End If
            Next
        End If
    End Sub

    Private Sub Form1_FormClosing(sender As Object, e As FormClosingEventArgs) Handles Me.FormClosing
        My.Settings.mainWindowPosition = Me.Location
        saveRestorePointListColumnOrder()

        If globalVariables.boolLogLoadsAndExits = True Then Functions.eventLogFunctions.writeToSystemEventLog("The user " & Environment.UserName & " closed the program.", EventLogEntryType.Information)
    End Sub

    Private Sub Form1_ResizeEnd(sender As Object, e As EventArgs) Handles Me.ResizeEnd
        My.Settings.windowSize = Me.Size
        My.Settings.Save()
    End Sub

    Private Sub Form1_KeyUp(sender As Object, e As KeyEventArgs) Handles Me.KeyUp
        If e.KeyCode = Keys.F5 Then btnRefreshRestorePoints.PerformClick()
    End Sub

    Private Sub Form1_Shown(sender As Object, e As EventArgs) Handles Me.Shown
        'AppActivate(Me.Text)
        Try
            Me.BringToFront()
        Catch ex As Exception
        End Try

        Try
            AppActivate(Process.GetCurrentProcess.Id)
        Catch ex As Exception
        End Try

        Control.CheckForIllegalCrossThreadCalls = False

        Threading.Thread.Sleep(750)

        doOldLogFileConversionRoutineAndStartLoadingRestorePoints()
    End Sub

    Private Sub doOldLogFileConversionRoutineAndStartLoadingRestorePoints()
        ' This code here checks to see if the log conversion has already taken place.
        Dim boolExportedOldLogs As Boolean = False
        Using registryKey As RegistryKey = Registry.LocalMachine.OpenSubKey(globalVariables.registryValues.strKey, False)
            ' And this parses the value from the Registry into a Boolean value.
            Boolean.TryParse(registryKey.GetValue("Exported Old Logs", "False"), boolExportedOldLogs)
        End Using

        ' This checks to see if we have converted the logs by checking the Boolean value we parsed above.
        If boolExportedOldLogs Then
            ' OK, so the log have already been converted so we just load the restore points.
            openPleaseWaitPanel("Loading Restore Points... Please Wait.")
            Threading.ThreadPool.QueueUserWorkItem(AddressOf startSystemRestorePointListLoadThreadSub)
        Else
            ' The log hasn't been converted yet so let's do it now.

            ' Open a Please Wait panel to tell the user that we have to do some work.
            openPleaseWaitPanel("Converting old application logs... Please wait.")

            ' Start a background thread to do the work.
            Threading.ThreadPool.QueueUserWorkItem(Sub()
                                                       ' This calls the function that converts the logs.
                                                       Functions.eventLogFunctions.getOldLogsFromWindowsEventLog()

                                                       ' Now we need to do some work on the main thread.
                                                       Me.Invoke(Sub()
                                                                     closePleaseWaitPanel() ' First we close the Please Wait panel.
                                                                     openPleaseWaitPanel("Loading Restore Points... Please Wait.") ' And open a new one.
                                                                 End Sub)

                                                       ' Now we load the restore points.
                                                       startSystemRestorePointListLoadThreadSub()
                                                   End Sub)
        End If
    End Sub

    Private Sub Form1_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        ' This code checks to see if the current version is a beta or Release Candidate and if the user's update channel is already set to beta mode.
        ' If the user's update channel isn't set to beta mode we then set it for the user here.
        If (globalVariables.version.boolBeta Or globalVariables.version.boolReleaseCandidate) And Not My.Settings.updateChannel.Equals(globalVariables.updateChannels.beta, StringComparison.OrdinalIgnoreCase) Then
            My.Settings.updateChannel = globalVariables.updateChannels.beta ' Changes the update channel to beta.
        End If

        If globalVariables.version.boolBeta Or globalVariables.version.boolReleaseCandidate Then
            toolStripStableChannel.Enabled = False
            toolStripStableChannel.Text &= " (Disabled)"
        End If

        Control.CheckForIllegalCrossThreadCalls = False
        verifyUpdateChannel()

        If IO.File.Exists("tom") Then
            ToolStripMenuItemPrivateForTom.Visible = True
        End If

        Try
            If globalVariables.boolLogLoadsAndExits = True Then Functions.eventLogFunctions.writeToSystemEventLog("The user " & Environment.UserName & " started the program.", EventLogEntryType.Information)

            If IO.File.Exists("updater.exe") = True Then Threading.ThreadPool.QueueUserWorkItem(AddressOf updaterDeleterThreadSub)

            interfaceTooSmallSettingCheckFormLoadSubRoutine()

            toolStripHeader.Image = My.Resources.RestorePoint_noBackground_2.ToBitmap
            NotifyIcon1.Icon = My.Resources.RestorePoint_noBackground_2

            Try
                ' Checks to see if the user is running Windows 7, Windows 8, Windows 8.1, or Windows 10 and then proceeds to add the Jumplist Tasks.
                ' Earlier versions of Windows don't support this option so there's a check for what OS the user is running before adding them.
                If Environment.OSVersion.ToString.Contains("10") Or Environment.OSVersion.ToString.Contains("6.4") Or Environment.OSVersion.ToString.Contains("6.3") Or Environment.OSVersion.ToString.Contains("6.2") Or Environment.OSVersion.ToString.Contains("6.1") Then
                    createJumpListTaskItems()
                End If
            Catch ex As Exception
                Functions.eventLogFunctions.writeCrashToEventLog(ex)
            End Try

            If Functions.osVersionInfo.isThisWindows10() = True Then
                BalloonToolStripMenuItem.Text = "Balloon/Action Center Notification"
            End If

            deleteRPLifeIntervalValue()

            If Functions.osVersionInfo.isThisWindows10 = True Or Functions.osVersionInfo.isThisWindows8x() = True Then
                addRPGlobalInterval()
                addRPSessionInterval()
            End If

            If (Environment.OSVersion.ToString.Contains("6.2") Or Environment.OSVersion.ToString.Contains("6.3") Or Environment.OSVersion.ToString.Contains("6.4") Or Environment.OSVersion.ToString.Contains("10")) Then ' This checks to see if we are running Windows 8, 8.1, or 10.
                addSpecialRegistryKeysToWindows8ToFixWindows8SystemRestorePoint()
            End If

            ' This hides the "Set Please Wait Border Color" options for Windows 8 and Windows 10 machines.
            If Functions.osVersionInfo.isThisWindows8x() Or Functions.osVersionInfo.isThisWindows10() Then
                SetPleaseWaitBorderColorToolStripMenuItem.Visible = False
                barBelowColorSettings.Visible = False
            End If

            addShortCutForEventLogToUsersStartMenu()
            loadPreferences()
            checkForMyComputerRightClickOption()
            checkScheduledTaskEXEPaths()
            showDonationNotice()
            loadRestorePointListColumnOrder()
            applySavedSorting()

            If IO.File.Exists(Application.ExecutablePath & ".new.exe") = True Then Threading.ThreadPool.QueueUserWorkItem(AddressOf newFileDeleterThreadSub)

            If Functions.support.areWeInSafeMode() = True Then
                toolStripScheduleRestorePoints.Enabled = False
                btnRestoreToRestorePointSafeMode.Enabled = False
                stripRestoreSafeMode.Enabled = False
                RemoveSafeModeBootOptionToolStripMenuItem.Visible = True
            Else
                RemoveSafeModeBootOptionToolStripMenuItem.Visible = False
            End If

            If My.Settings.checkSystemDrivesForFullShadowStorage = True Then
                Threading.ThreadPool.QueueUserWorkItem(AddressOf checkRestorePointSpaceThreadSub)
            End If

            chkShowVersionInTitleBarToolStripMenuItem.Checked = My.Settings.boolShowVersionInWindowTitle

            If My.Settings.boolShowVersionInWindowTitle = True Then
                If globalVariables.version.boolBeta Then
                    Me.Text = String.Format("Restore Point Creator ({0} Public Beta {1})", globalVariables.version.strFullVersionString, globalVariables.version.shortBetaVersion)
                ElseIf globalVariables.version.boolReleaseCandidate Then
                    Me.Text = String.Format("Restore Point Creator ({0} Release Candidate {1})", globalVariables.version.strFullVersionString, globalVariables.version.shortReleaseCandidateVersion)
                Else
                    Me.Text = "Restore Point Creator (" & globalVariables.version.strFullVersionString & ")"
                End If
            End If

            boolDoneLoading = True
            systemRestorePointsList.Select()

            My.Settings.boolFirstRun = False
        Catch ex2 As IO.IOException
            handleConfigFileAccessViolation(ex2)
        Catch ex As Exception
            Threading.Thread.CurrentThread.CurrentUICulture = New Globalization.CultureInfo("en-US")
            exceptionHandler.manuallyLoadCrashWindow(ex, "Main Form Load" & vbCrLf & vbCrLf & ex.Message, ex.StackTrace, ex.GetType)
        End Try
    End Sub

    Private Sub handleConfigFileAccessViolation(ex As IO.IOException)
        If ex.Message.caseInsensitiveContains("user.config") Then
            Functions.eventLogFunctions.writeToSystemEventLog("Unable to open application settings file, it appears to be locked by another process.", EventLogEntryType.Error)
            Functions.eventLogFunctions.writeCrashToEventLog(ex)
            MsgBox("Unable to open application settings file, it appears to be locked by another process." & vbCrLf & vbCrLf & "The program will now close.", MsgBoxStyle.Critical, "Restore Point Creator")

            Process.GetCurrentProcess.Kill()
        End If
    End Sub

    Private Sub systemRestorePointsList_ColumnClick(sender As Object, e As ColumnClickEventArgs) Handles systemRestorePointsList.ColumnClick
        ' Get the new sorting column.
        Dim new_sorting_column As ColumnHeader = systemRestorePointsList.Columns(e.Column)
        My.Settings.sortingColumn = e.Column

        ' Figure out the new sorting order.
        Dim sort_order As SortOrder
        If (m_SortingColumn Is Nothing) Then
            ' New column. Sort ascending.
            sort_order = SortOrder.Ascending
            My.Settings.sortingOrder = SortOrder.Ascending
        Else
            ' See if this is the same column.
            If new_sorting_column.Equals(m_SortingColumn) Then
                ' Same column. Switch the sort order.
                If m_SortingColumn.Text.StartsWith("> ") Then
                    sort_order = SortOrder.Descending
                    My.Settings.sortingOrder = SortOrder.Descending
                Else
                    sort_order = SortOrder.Ascending
                    My.Settings.sortingOrder = SortOrder.Ascending
                End If
            Else
                ' New column. Sort ascending.
                sort_order = SortOrder.Ascending
                My.Settings.sortingOrder = SortOrder.Ascending
            End If

            ' Remove the old sort indicator.
            m_SortingColumn.Text = m_SortingColumn.Text.Substring(2)
        End If

        ' Display the new sort order.
        m_SortingColumn = new_sorting_column
        If sort_order = SortOrder.Ascending Then
            m_SortingColumn.Text = "> " & m_SortingColumn.Text
        Else
            m_SortingColumn.Text = "< " & m_SortingColumn.Text
        End If

        ' Create a comparer.
        systemRestorePointsList.ListViewItemSorter = New Functions.listViewSorter.ListViewComparer(e.Column, sort_order)

        ' Sort.
        systemRestorePointsList.Sort()
    End Sub

    Private Sub systemRestorePointsList_ColumnWidthChanged(sender As Object, e As ColumnWidthChangedEventArgs) Handles systemRestorePointsList.ColumnWidthChanged
        If boolDoneLoading = True Then
            My.Settings.column1Size = ColumnHeader1.Width
            My.Settings.column2Size = ColumnHeader2.Width
            My.Settings.column3Size = ColumnHeader3.Width
            My.Settings.column4Size = ColumnHeader4.Width
            My.Settings.column5Size = ColumnHeader5.Width
            My.Settings.Save()
        End If
    End Sub

    Private Sub ListView2_KeyUp(sender As Object, e As KeyEventArgs) Handles systemRestorePointsList.KeyUp
        If e.KeyCode = Keys.Delete Then
            btnDeleteRestorePoint.PerformClick()
        End If
    End Sub
#End Region

#Region "--== ToolStrip Click Events ==--"
    Private Sub ManuallyFixSystemRestoreToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles ManuallyFixSystemRestoreToolStripMenuItem.Click
        If MsgBox("You are about to forcefully fix System Restore on your system by enabling System Restore on the system drive. Use this tool only if you have received errors from the program such as Error 1058." & vbCrLf & vbCrLf & "WARNING! This tool may have unintended consequences such as lost restore points. By using this tool you agree that the developer of System Restore Point Creator is not liable for any lost restore points." & vbCrLf & vbCrLf & "Are you sure you want to do this?", MsgBoxStyle.Question + MsgBoxStyle.YesNo, strMessageBoxTitle) = MsgBoxResult.Yes Then
            Functions.eventLogFunctions.writeToSystemEventLog("The Manual System Restore Fix Tool has been engaged.", EventLogEntryType.Information)

            Dim gigabytesInBytes As Long = 1073741824
            Dim newSize As Long = gigabytesInBytes * 20 ' Sets the size to 20 GBs.

            Functions.vss.executeVSSAdminCommand(globalVariables.systemDriveLetter)
            Functions.vss.setShadowStorageSize(globalVariables.systemDriveLetter, newSize)
            Functions.vss.enableSystemRestoreOnDriveWMI(globalVariables.systemDriveLetter)

            Functions.eventLogFunctions.writeToSystemEventLog("The Manual System Restore Fix Tool completed it's work. The system will now reboot.", EventLogEntryType.Information)

            If MsgBox("The Manual System Restore Fix Tool completed it's work. Your system needs to be rebooted." & vbCrLf & vbCrLf & "Do you want to reboot your computer now?", MsgBoxStyle.Information + MsgBoxStyle.YesNo, strMessageBoxTitle) = MsgBoxResult.Yes Then
                Functions.support.rebootSystem()
            End If
        Else
            MsgBox("You have chosen to not use this tool therefore nothing has been done to your system.", MsgBoxStyle.Information, strMessageBoxTitle)
        End If
    End Sub

    Private Sub ConfirmRestorePointDeletionsInBatchesToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles ConfirmRestorePointDeletionsInBatchesToolStripMenuItem.Click
        My.Settings.multiConfirmRestorePointDeletions = ConfirmRestorePointDeletionsInBatchesToolStripMenuItem.Checked
    End Sub

    Private Sub chkShowVersionInTitleBarToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles chkShowVersionInTitleBarToolStripMenuItem.Click
        My.Settings.boolShowVersionInWindowTitle = chkShowVersionInTitleBarToolStripMenuItem.Checked

        If My.Settings.boolShowVersionInWindowTitle = True Then
            If globalVariables.version.boolBeta Then
                Me.Text = String.Format("Restore Point Creator ({0} Public Beta {1})", globalVariables.version.strFullVersionString, globalVariables.version.shortBetaVersion)
            ElseIf globalVariables.version.boolReleaseCandidate Then
                Me.Text = String.Format("Restore Point Creator ({0} Release Candidate {1})", globalVariables.version.strFullVersionString, globalVariables.version.shortReleaseCandidateVersion)
            Else
                Me.Text = "Restore Point Creator (" & globalVariables.version.strFullVersionString & ")"
            End If
        Else
            Me.Text = "Restore Point Creator"
        End If
    End Sub

    Private Sub ConfigureHTTPTimeoutToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles ConfigureHTTPTimeoutToolStripMenuItem.Click
        Dim frmConfigureHTTPTimeout As New Configure_HTTP_Timeout With {
            .StartPosition = FormStartPosition.CenterParent
        }
        frmConfigureHTTPTimeout.ShowDialog(Me)
        ConfigureHTTPTimeoutToolStripMenuItem.Text = String.Format("Configure HTTP Timeout ({0} Seconds)", My.Settings.httpTimeout)
    End Sub

    Private Sub ExtendedLoggingForScheduledTasks_Click(sender As Object, e As EventArgs) Handles ExtendedLoggingForScheduledTasks.Click
        If ExtendedLoggingForScheduledTasks.Checked = True Then
            savePreferenceToRegistry("Extended Logging For Scheduled Tasks", "True")
        Else
            savePreferenceToRegistry("Extended Logging For Scheduled Tasks", "False")
        End If
    End Sub

    Private Sub FixRuntimeTasksToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles FixRuntimeTasksToolStripMenuItem.Click
        Dim task As TaskScheduler.Task = Nothing

        If Functions.taskStuff.doesRunTimeTaskExist("Restore Point Creator -- Run with no UAC (Create Restore Point)", task) = True Then
            Functions.taskStuff.deleteTask(task)
            task.Dispose()

            Functions.taskStuff.addRunTimeTask("Restore Point Creator -- Run with no UAC (Create Restore Point)", "Runs Restore Point Creator with no UAC prompt.", Application.ExecutablePath, globalVariables.commandLineSwitches.createRestorePoint)
        End If

        If Functions.taskStuff.doesRunTimeTaskExist("Restore Point Creator -- Run with no UAC (Create Custom Restore Point)", task) = True Then
            Functions.taskStuff.deleteTask(task)
            task.Dispose()

            Functions.taskStuff.addRunTimeTask("Restore Point Creator -- Run with no UAC (Create Custom Restore Point)", "Runs Restore Point Creator with no UAC prompt.", Application.ExecutablePath, globalVariables.commandLineSwitches.createCustomRestorePoint)
        End If

        If Functions.taskStuff.doesRunTimeTaskExist("Restore Point Creator -- Run with no UAC", task) = True Then
            Functions.taskStuff.deleteTask(task)
            task.Dispose()

            Functions.taskStuff.addRunTimeTask("Restore Point Creator -- Run with no UAC", "Runs Restore Point Creator with no UAC prompt.", Application.ExecutablePath, "", True)
        End If

        MsgBox("The Runtime tasks have been repaired.", MsgBoxStyle.Information, strMessageBoxTitle)
    End Sub

    Private Sub AskBeforeCreatingRestorePointToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles AskBeforeCreatingRestorePointToolStripMenuItem.Click
        My.Settings.askBeforeCreatingRestorePoint = AskBeforeCreatingRestorePointToolStripMenuItem.Checked
    End Sub

    Private Sub RebootSystemToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles RebootSystemToolStripMenuItem.Click
        If MsgBox("Are you sure you want to reboot your system?", MsgBoxStyle.Question + MsgBoxStyle.YesNo, "Reboot System?") = MsgBoxResult.Yes Then
            Functions.support.rebootSystem()
        End If
    End Sub

    Private Sub EnableExtendedLoggingToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles EnableExtendedLoggingToolStripMenuItem.Click
        globalVariables.boolExtendedLoggingDuringUpdating = EnableExtendedLoggingToolStripMenuItem.Checked

        If EnableExtendedLoggingToolStripMenuItem.Checked = True Then
            savePreferenceToRegistry("Enable Extended Logging During Updating", "True")
        Else
            savePreferenceToRegistry("Enable Extended Logging During Updating", "False")
        End If
    End Sub

    Private Sub OnlyGiveMeReleaseCandidates_Click(sender As Object, e As EventArgs) Handles OnlyGiveMeReleaseCandidates.Click
        My.Settings.onlyGiveMeRCs = OnlyGiveMeReleaseCandidates.Checked
    End Sub

    Private Sub stripDelete_Click(sender As Object, e As EventArgs) Handles stripDelete.Click
        btnDeleteRestorePoint.PerformClick()
    End Sub

    Private Sub stripRefresh_Click(sender As Object, e As EventArgs) Handles stripRefresh.Click
        btnRefreshRestorePoints.PerformClick()
    End Sub

    Private Sub stripRestore_Click(sender As Object, e As EventArgs) Handles stripRestore.Click
        btnRestoreToRestorePoint.PerformClick()
    End Sub

    Private Sub BackupToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles BackupToolStripMenuItem.Click
        exportBackupDialog.Title = "Backup your settings where?"
        exportBackupDialog.Filter = "Config Backup File|*.resbakx"
        exportBackupDialog.FileName = Nothing

        If exportBackupDialog.ShowDialog() = DialogResult.OK Then
            If Not IO.Directory.Exists(New IO.FileInfo(exportBackupDialog.FileName).DirectoryName) Then
                MsgBox("The directory that you are trying to write the file to doesn't exist, please double check where you are saving the file to.", MsgBoxStyle.Critical, strMessageBoxTitle)
                Exit Sub
            End If

            If IO.File.Exists(exportBackupDialog.FileName) Then
                IO.File.Delete(exportBackupDialog.FileName)
            End If

            Functions.importExportSettings.exportSettingsToXMLFile(exportBackupDialog.FileName)

            MsgBox("Backup complete.", MsgBoxStyle.Information, strMessageBoxTitle)
        End If
    End Sub

    Private Sub RestoreToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles RestoreToolStripMenuItem.Click
        importBackupDialog.Title = "Locate your backup file..."
        importBackupDialog.Filter = "XML Config Backup File|*.resbakx|Config Backup File|*.resbak|INI File|*.ini"
        importBackupDialog.FileName = Nothing

        If importBackupDialog.ShowDialog() = DialogResult.OK Then
            If IO.File.Exists(importBackupDialog.FileName) = True Then
                If New IO.FileInfo(importBackupDialog.FileName).Extension.Equals(".resbakx", StringComparison.OrdinalIgnoreCase) Then
                    Functions.importExportSettings.importSettingsFromXMLFile(importBackupDialog.FileName, strMessageBoxTitle)
                Else
                    Functions.importExportSettings.importSettingsFromLegacyBackupFile(importBackupDialog.FileName, strMessageBoxTitle)
                End If

                Functions.eventLogFunctions.writeToSystemEventLog(String.Format("A configuration backup has been restored from {0}{1}{0} by user {0}{2}{0}.", Chr(34), importBackupDialog.FileName, Environment.UserName), EventLogEntryType.Information)

                loadPreferences()
                loadRestorePointListColumnOrder()

                MsgBox("Backup configuration file restoration complete.", MsgBoxStyle.Information, strMessageBoxTitle)
            End If
        End If
    End Sub

    Private Sub MountVolumeShadowCopyToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles MountVolumeShadowCopyToolStripMenuItem.Click
        If (globalVariables.windows.mountVolumeShadowCopy Is Nothing) Then
            globalVariables.windows.mountVolumeShadowCopy = New Mount_Volume_Shadow_Copy With {
                .StartPosition = FormStartPosition.CenterScreen
            }
            globalVariables.windows.mountVolumeShadowCopy.Show()
            globalVariables.windows.mountVolumeShadowCopy.Location = My.Settings.mountVolumeShadowCopyWindowPosition
        Else
            globalVariables.windows.mountVolumeShadowCopy.BringToFront()
        End If
    End Sub

    Private Sub SetWindowsActivePowerPlanSettingsForWakeTimersBackToDefaultToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles SetWindowsActivePowerPlanSettingsForWakeTimersBackToDefaultToolStripMenuItem.Click
        Dim msgBoxResult As MsgBoxResult = MsgBox("Setting the Windows Active Power Plan Wake Times feature back to default (Disabled) will prevent System Restore from creating restore points while your system is asleep." & vbCrLf & vbCrLf & "Are you sure you want to do this?", MsgBoxStyle.Question + MsgBoxStyle.YesNo, "Are you sure?")

        If msgBoxResult = MsgBoxResult.Yes Then
            Functions.power.disablePowerPlanWakeFromSleep()
        End If
    End Sub

    Private Sub UseSSLToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles UseSSLToolStripMenuItem.Click
        If UseSSLToolStripMenuItem.Checked = False Then
            Dim msgBoxResult As MsgBoxResult = MsgBox("It's recommended to have SSL enabled so that downloads and data that's sent to my web site is secure. Please reconsider this." & vbCrLf & vbCrLf & "Are you sure you want to disable SSL?", MsgBoxStyle.Question + vbYesNo, "Are you sure?")

            If msgBoxResult = MsgBoxResult.Yes Then
                My.Settings.useSSL = UseSSLToolStripMenuItem.Checked
            Else
                UseSSLToolStripMenuItem.Checked = True
            End If
        Else
            My.Settings.useSSL = UseSSLToolStripMenuItem.Checked
        End If
    End Sub

    Private Sub ConfigureProxyToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles ConfigureProxyToolStripMenuItem.Click
        If (globalVariables.windows.configureProxy Is Nothing) Then
            globalVariables.windows.configureProxy = New Configure_Proxy With {
                .StartPosition = FormStartPosition.CenterParent
            }
            globalVariables.windows.configureProxy.Show()
        Else
            globalVariables.windows.configureProxy.BringToFront()
        End If
    End Sub

    Private Sub AskBeforeUpgradingUpdatingToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles AskBeforeUpgradingUpdatingToolStripMenuItem.Click
        My.Settings.askToUpgrade = AskBeforeUpgradingUpdatingToolStripMenuItem.Checked
    End Sub

    Private Sub RemoveSafeModeBootOptionToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles RemoveSafeModeBootOptionToolStripMenuItem.Click
        Functions.support.removeSafeModeBoot()
        MsgBox("The setting to boot your system into Safe Mode has been removed. Your system will now reboot.", MsgBoxStyle.Information, strMessageBoxTitle)
        Functions.support.rebootSystem()
    End Sub

    Private Sub FrequentlyAskedQuestionsToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles FrequentlyAskedQuestionsToolStripMenuItem.Click
        Functions.support.launchURLInWebBrowser(globalVariables.webURLs.webPages.strFAQ)
    End Sub

    Private Sub LogProgramLoadsAndExitsToEventLogToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles LogProgramLoadsAndExitsToEventLogToolStripMenuItem.Click
        globalVariables.boolLogLoadsAndExits = LogProgramLoadsAndExitsToEventLogToolStripMenuItem.Checked

        If globalVariables.boolLogLoadsAndExits = True Then
            savePreferenceToRegistry("Log Program Loads And Exits to Event Log", "True")
        Else
            savePreferenceToRegistry("Log Program Loads And Exits to Event Log", "False")
        End If
    End Sub

    Private Sub ContactTheDeveloperToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles ContactTheDeveloperToolStripMenuItem.Click
        If (globalVariables.windows.officialContactForm Is Nothing) Then
            globalVariables.windows.officialContactForm = New Official_Contact_Form With {
                .StartPosition = FormStartPosition.CenterParent
            }
            globalVariables.windows.officialContactForm.Show()
        Else
            globalVariables.windows.officialContactForm.BringToFront()
        End If
    End Sub

    Private Sub RoundTheAgeOfRestorePointInDaysToHowManyDecimalsToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles RoundTheAgeOfRestorePointInDaysToHowManyDecimalsToolStripMenuItem.Click
        Dim roundWindow As New Round_Restore_Point_Age_in_Days With {
            .StartPosition = FormStartPosition.CenterParent
        }
        roundWindow.ShowDialog()

        Dim response As Round_Restore_Point_Age_in_Days.userResponse = roundWindow.dialogResponse
        If response = Round_Restore_Point_Age_in_Days.userResponse.ok Then btnRefreshRestorePoints.PerformClick()
    End Sub

    Private Sub CommandLineArgumentsToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles CommandLineArgumentsToolStripMenuItem.Click
        Dim argumentsWindows As New Restore_Point_Creator_Command_Line_Arguments With {
            .StartPosition = FormStartPosition.CenterParent
        }
        argumentsWindows.ShowDialog()
    End Sub

    Private Sub stripRestoreSafeMode_Click(sender As Object, e As EventArgs) Handles stripRestoreSafeMode.Click
        btnRestoreToRestorePointSafeMode.PerformClick()
    End Sub

    Private Sub CreateRestorePointAtUserLogonToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles CreateRestorePointAtUserLogonToolStripMenuItem.Click
        If (globalVariables.windows.frmCreateRestorePointAtUserLogon Is Nothing) Then
            globalVariables.windows.frmCreateRestorePointAtUserLogon = New Create_Restore_Point_at_User_Logon With {
                .StartPosition = FormStartPosition.CenterParent
            }
            globalVariables.windows.frmCreateRestorePointAtUserLogon.Show()
        Else
            globalVariables.windows.frmCreateRestorePointAtUserLogon.BringToFront()
        End If
    End Sub

    Private Sub EnableSystemEventLoggingToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles EnableSystemEventLoggingToolStripMenuItem.Click
        If EnableSystemEventLoggingToolStripMenuItem.Checked = True Then
            savePreferenceToRegistry("Enable System Logging", "True")
            globalVariables.boolLogToSystemLog = True
        Else
            savePreferenceToRegistry("Enable System Logging", "False")
            globalVariables.boolLogToSystemLog = False
        End If
    End Sub

    Private Sub ProgramEventLogToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles ProgramEventLogToolStripMenuItem.Click
        If (globalVariables.windows.eventLogForm Is Nothing) Then
            globalVariables.windows.eventLogForm = New eventLogForm With {
                .StartPosition = FormStartPosition.CenterScreen
            }
            globalVariables.windows.eventLogForm.Show()
            globalVariables.windows.eventLogForm.Location = My.Settings.eventLogFormWindowLocation
        Else
            globalVariables.windows.eventLogForm.BringToFront()
        End If
    End Sub

    Private Sub SwitchToDebugBuildToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles SwitchToDebugBuildToolStripMenuItem.Click
        If globalVariables.version.boolDebugBuild = True Then
            Exit Sub
        End If

        Dim msgBoxResult As MsgBoxResult = MsgBox("The debug build is a build that's not optimized for normal use but may help in the process of debugging crashes and other issues that you may have with the program. The debug build outputs far more crash data than the release type build." & vbCrLf & vbCrLf & "Are you sure you want to switch to the debug build?", MsgBoxStyle.Question + MsgBoxStyle.YesNo)

        If msgBoxResult = MsgBoxResult.Yes Then
            openPleaseWaitPanel("Downloading Debug Build... Please Wait.")
            Threading.ThreadPool.QueueUserWorkItem(AddressOf switchToDebugBuildDownloadThreadSub)
        End If
    End Sub

    Private Sub InterfaceTooSmallToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles InterfaceTooBigToolStripMenuItem.Click
        If Registry.LocalMachine.OpenSubKey("SOFTWARE\Microsoft\Windows NT\CurrentVersion") Is Nothing Then
            Registry.LocalMachine.OpenSubKey("SOFTWARE\Microsoft\Windows NT", True).CreateSubKey("CurrentVersion")
            Registry.LocalMachine.OpenSubKey("SOFTWARE\Microsoft\Windows NT\CurrentVersion", True).CreateSubKey("AppCompatFlags")
            Registry.LocalMachine.OpenSubKey("SOFTWARE\Microsoft\Windows NT\CurrentVersion\AppCompatFlags", True).CreateSubKey("Layers")
        End If

        If Registry.LocalMachine.OpenSubKey("SOFTWARE\Microsoft\Windows NT\CurrentVersion\AppCompatFlags") Is Nothing Then
            Registry.LocalMachine.OpenSubKey("SOFTWARE\Microsoft\Windows NT\CurrentVersion", True).CreateSubKey("AppCompatFlags")
            Registry.LocalMachine.OpenSubKey("SOFTWARE\Microsoft\Windows NT\CurrentVersion\AppCompatFlags", True).CreateSubKey("Layers")
        End If

        If Registry.LocalMachine.OpenSubKey("SOFTWARE\Microsoft\Windows NT\CurrentVersion\AppCompatFlags\Layers") Is Nothing Then
            Registry.LocalMachine.OpenSubKey("SOFTWARE\Microsoft\Windows NT\CurrentVersion\AppCompatFlags", True).CreateSubKey("Layers")
        End If

        Dim registryKey As RegistryKey = Registry.LocalMachine.OpenSubKey("SOFTWARE\Microsoft\Windows NT\CurrentVersion\AppCompatFlags\Layers", True)

        If InterfaceTooBigToolStripMenuItem.Checked = True Then
            registryKey.SetValue(Process.GetCurrentProcess.MainModule.FileName.ToLower, "~ HIGHDPIAWARE", RegistryValueKind.String)
            registryKey.Close()
            registryKey = Nothing

            MsgBox("The compatibility flag has been set for the program. The program will relaunch for the changes to take effect.", MsgBoxStyle.Information, strMessageBoxTitle)
            Functions.support.reRunWithAdminUserRights()
        Else
            registryKey.DeleteValue(Process.GetCurrentProcess.MainModule.FileName.ToLower, False)
            registryKey.Close()
            registryKey = Nothing
        End If
    End Sub

    Private Sub CheckWindowsPowerPlanSettingsToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles CheckWindowsPowerPlanSettingsToolStripMenuItem.Click
        Functions.power.checkIfActivePowerPlanIsSetProperlyForWakingFromSleep(True)
    End Sub

    Private Sub AllowForDeletionOfAllSystemRestorePointsToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles AllowForDeletionOfAllSystemRestorePointsToolStripMenuItem.Click
        If AllowForDeletionOfAllSystemRestorePointsToolStripMenuItem.Checked Then
            AllowForDeletionOfAllSystemRestorePointsToolStripMenuItem.Checked = False
        Else
            Dim msgBoxResult As MsgBoxResult = MsgBox("It isn't recommended to allow System Restore Point Creator to delete all System Restore Points that exist on this system, including the most recent System Restore Point." & vbCrLf & vbCrLf & "Are you sure you want to do this?", MsgBoxStyle.Question + MsgBoxStyle.YesNo, strMessageBoxTitle)

            If msgBoxResult = Microsoft.VisualBasic.MsgBoxResult.Yes Then
                AllowForDeletionOfAllSystemRestorePointsToolStripMenuItem.Checked = True
            End If
        End If

        My.Settings.allowDeleteOfAllRestorePoints = AllowForDeletionOfAllSystemRestorePointsToolStripMenuItem.Checked
        My.Settings.Save()
    End Sub

    Private Sub MenuStrip1_Click(sender As Object, e As EventArgs) Handles MenuStrip1.Click
        doTheGrayingOfTheRestorePointNameTextBox()
    End Sub

    Private Sub MessageBoxToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles MessageBoxToolStripMenuItem.Click
        BalloonToolStripMenuItem.Checked = False
        My.Settings.notificationType2 = enums.userFeedbackType.balloon
        My.Settings.Save()
        'changeMessageTypeMenuItems()
    End Sub

    Private Sub BalloonToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles BalloonToolStripMenuItem.Click
        MessageBoxToolStripMenuItem.Checked = False
        My.Settings.notificationType2 = enums.userFeedbackType.balloon
        My.Settings.Save()
        'changeMessageTypeMenuItems()
    End Sub

    Private Sub BypassNoUACLauncherToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles BypassNoUACLauncherToolStripMenuItem.Click
        If BypassNoUACLauncherToolStripMenuItem.Checked Then
            savePreferenceToRegistry("No Task", "True")
        Else
            savePreferenceToRegistry("No Task", "False")
        End If
    End Sub

    Private Sub ViewOfficialChangeLogToolStripMenuItem1_Click(sender As Object, e As EventArgs) Handles ViewOfficialChangeLogToolStripMenuItem1.Click
        If (globalVariables.windows.frmChangeLog Is Nothing) Then
            globalVariables.windows.frmChangeLog = New Change_Log With {
                .StartPosition = FormStartPosition.CenterParent
            }
            globalVariables.windows.frmChangeLog.Show()
        Else
            globalVariables.windows.frmChangeLog.BringToFront()
        End If
    End Sub

    Private Sub toolStripViewDiskSpaceUsage_Click(sender As Object, e As EventArgs) Handles toolStripViewDiskSpaceUsage.Click
        If (globalVariables.windows.frmDiskSpaceUsageWindow Is Nothing) Then
            globalVariables.windows.frmDiskSpaceUsageWindow = New Disk_Space_Usage With {
                .StartPosition = FormStartPosition.CenterScreen
            }
            globalVariables.windows.frmDiskSpaceUsageWindow.Show()
            globalVariables.windows.frmDiskSpaceUsageWindow.Location = My.Settings.DiskSpaceUsageWindowLocation
        Else
            globalVariables.windows.frmDiskSpaceUsageWindow.BringToFront()
        End If
    End Sub

    Private Sub toolStripManageSystemRestoreStorageSize_Click(sender As Object, e As EventArgs) Handles toolStripManageSystemRestoreStorageSize.Click
        If (globalVariables.windows.frmManageSystemRestoreStorageSpace Is Nothing) Then
            globalVariables.windows.frmManageSystemRestoreStorageSpace = New frmManageSystemRestoreStorageSpace With {
                .StartPosition = FormStartPosition.CenterScreen
            }
            globalVariables.windows.frmManageSystemRestoreStorageSpace.Show()
            globalVariables.windows.frmManageSystemRestoreStorageSpace.Location = My.Settings.ManageSystemRestoreStorageSpaceWindowLocation
        Else
            globalVariables.windows.frmManageSystemRestoreStorageSpace.BringToFront()
        End If
    End Sub

    Private Sub toolStripScheduleRestorePoints_Click(sender As Object, e As EventArgs) Handles toolStripScheduleRestorePoints.Click
        If (globalVariables.windows.frmTaskScheduler Is Nothing) Then
            globalVariables.windows.frmTaskScheduler = New frmTaskScheduler With {
                .StartPosition = FormStartPosition.CenterScreen
            }
            globalVariables.windows.frmTaskScheduler.Show()
            globalVariables.windows.frmTaskScheduler.Location = My.Settings.TaskSchedulerWindowLocation
        Else
            globalVariables.windows.frmTaskScheduler.BringToFront()
        End If
    End Sub

    Private Sub toolStripMyComputer_Click(sender As Object, e As EventArgs) Handles toolStripMyComputer.Click
        Dim registryRootKeyWeAreWorkingWith As RegistryKey

        If Functions.osVersionInfo.isThisWindows10() Then
            ' In Windows 10 we can only write to the CLSID Classes Root that is a subkey of the current user's registry hive.
            registryRootKeyWeAreWorkingWith = Registry.CurrentUser.OpenSubKey("SOFTWARE\Classes", True)
        Else
            registryRootKeyWeAreWorkingWith = Registry.ClassesRoot
        End If

        If toolStripMyComputer.Checked Then
            Try
                ' Attempts to fix something that should never have been broken to begin with.  Why this would be broken, who the fuck knows.
                If (registryRootKeyWeAreWorkingWith.OpenSubKey("CLSID\{20D04FE0-3AEA-1069-A2D8-08002B30309D}", False) Is Nothing) Then
                    registryRootKeyWeAreWorkingWith.OpenSubKey("CLSID", True).CreateSubKey("{20D04FE0-3AEA-1069-A2D8-08002B30309D}")
                    registryRootKeyWeAreWorkingWith.OpenSubKey("CLSID\{20D04FE0-3AEA-1069-A2D8-08002B30309D}", True).CreateSubKey("Shell")
                End If
            Catch ex As Exception
                ' Does nothing
            End Try

            If Application.ExecutablePath.caseInsensitiveContains("program files") = False Then
                If Environment.Is64BitOperatingSystem Then
                    MsgBox("It is HIGHLY recommended to place this program's executable in C:\Program Files (x86)." & vbCrLf & vbCrLf & "The reason why is that this portion of the program creates a My Computer right-click option so if Windows can't find the executable in the specified path, the right-click option will be invalid.", MsgBoxStyle.Information, strMessageBoxTitle)
                Else
                    MsgBox("It is HIGHLY recommended to place this program's executable in C:\Program Files." & vbCrLf & vbCrLf & "The reason why is that this portion of the program creates a My Computer right-click option so if Windows can't find the executable in the specified path, the right-click option will be invalid.", MsgBoxStyle.Information, strMessageBoxTitle)
                End If

                toolStripMyComputer.Checked = False
                Exit Sub
            End If

            Dim boolIsThisVistaOrNewer As Boolean = False
            If Environment.OSVersion.Version.Major >= 6 Then
                boolIsThisVistaOrNewer = True
            End If

            Try
                Dim registryKey As RegistryKey

                ' =======================================================
                ' == Makes the "Create System Restore Checkpoint" Item ==
                ' =======================================================

                ' Checks if an existing entry exists and deletes it.
                If registryRootKeyWeAreWorkingWith.OpenSubKey("CLSID\{20D04FE0-3AEA-1069-A2D8-08002B30309D}\Shell\Create System Restore Checkpoint") IsNot Nothing Then
                    registryRootKeyWeAreWorkingWith.OpenSubKey("CLSID\{20D04FE0-3AEA-1069-A2D8-08002B30309D}\Shell", True).DeleteSubKeyTree("Create System Restore Checkpoint")
                End If

                registryRootKeyWeAreWorkingWith.OpenSubKey("CLSID\{20D04FE0-3AEA-1069-A2D8-08002B30309D}\Shell", True).CreateSubKey("Create System Restore Checkpoint")
                registryRootKeyWeAreWorkingWith.OpenSubKey("CLSID\{20D04FE0-3AEA-1069-A2D8-08002B30309D}\Shell\Create System Restore Checkpoint", True).CreateSubKey("Command")

                registryKey = registryRootKeyWeAreWorkingWith.OpenSubKey("CLSID\{20D04FE0-3AEA-1069-A2D8-08002B30309D}\Shell\Create System Restore Checkpoint", True)
                If boolIsThisVistaOrNewer = True Then
                    registryKey.SetValue("HasLUAShield", "", RegistryValueKind.String)
                End If
                registryKey.SetValue("icon", String.Format("{0}{1}{0}", Chr(34), Application.ExecutablePath), RegistryValueKind.String)
                registryKey.SetValue("SuppressionPolicy", 1073741884, RegistryValueKind.DWord)
                registryKey.Close()

                registryKey = registryRootKeyWeAreWorkingWith.OpenSubKey("CLSID\{20D04FE0-3AEA-1069-A2D8-08002B30309D}\Shell\Create System Restore Checkpoint\Command", True)
                registryKey.SetValue(vbNullString, String.Format("{0}{1}{0} -createrestorepoint", Chr(34), Application.ExecutablePath))
                registryKey.Close()

                ' ===============================================================
                ' == Makes the "Create Custom Named System Restore Point" Item ==
                ' ===============================================================

                ' Checks if an existing entry exists and deletes it.
                If registryRootKeyWeAreWorkingWith.OpenSubKey("CLSID\{20D04FE0-3AEA-1069-A2D8-08002B30309D}\Shell\Create Custom Named System Restore Point") IsNot Nothing Then
                    registryRootKeyWeAreWorkingWith.OpenSubKey("CLSID\{20D04FE0-3AEA-1069-A2D8-08002B30309D}\Shell", True).DeleteSubKeyTree("Create Custom Named System Restore Point")
                End If

                registryRootKeyWeAreWorkingWith.OpenSubKey("CLSID\{20D04FE0-3AEA-1069-A2D8-08002B30309D}\Shell", True).CreateSubKey("Create Custom Named System Restore Point")
                registryRootKeyWeAreWorkingWith.OpenSubKey("CLSID\{20D04FE0-3AEA-1069-A2D8-08002B30309D}\Shell\Create Custom Named System Restore Point", True).CreateSubKey("Command")

                registryKey = registryRootKeyWeAreWorkingWith.OpenSubKey("CLSID\{20D04FE0-3AEA-1069-A2D8-08002B30309D}\Shell\Create Custom Named System Restore Point", True)
                If boolIsThisVistaOrNewer = True Then
                    registryKey.SetValue("HasLUAShield", "", RegistryValueKind.String)
                End If
                registryKey.SetValue("icon", String.Format("{0}{1}{0}", Chr(34), Application.ExecutablePath), RegistryValueKind.String)
                registryKey.SetValue("SuppressionPolicy", 1073741884, RegistryValueKind.DWord)
                registryKey.Close()

                registryKey = registryRootKeyWeAreWorkingWith.OpenSubKey("CLSID\{20D04FE0-3AEA-1069-A2D8-08002B30309D}\Shell\Create Custom Named System Restore Point\Command", True)
                registryKey.SetValue(vbNullString, String.Format("{0}{1}{0} -createrestorepointcustomname", Chr(34), Application.ExecutablePath))
                registryKey.Close()

                ' ===================================================
                ' == Makes the "Launch Restore Point Creator" Item ==
                ' ===================================================

                ' Checks if an existing entry exists and deletes it.
                If registryRootKeyWeAreWorkingWith.OpenSubKey("CLSID\{20D04FE0-3AEA-1069-A2D8-08002B30309D}\Shell\Launch Restore Point Creator") IsNot Nothing Then
                    registryRootKeyWeAreWorkingWith.OpenSubKey("CLSID\{20D04FE0-3AEA-1069-A2D8-08002B30309D}\Shell", True).DeleteSubKeyTree("Launch Restore Point Creator")
                End If

                registryRootKeyWeAreWorkingWith.OpenSubKey("CLSID\{20D04FE0-3AEA-1069-A2D8-08002B30309D}\Shell", True).CreateSubKey("Launch Restore Point Creator")
                registryRootKeyWeAreWorkingWith.OpenSubKey("CLSID\{20D04FE0-3AEA-1069-A2D8-08002B30309D}\Shell\Launch Restore Point Creator", True).CreateSubKey("Command")

                registryKey = registryRootKeyWeAreWorkingWith.OpenSubKey("CLSID\{20D04FE0-3AEA-1069-A2D8-08002B30309D}\Shell\Launch Restore Point Creator", True)
                If boolIsThisVistaOrNewer = True Then
                    registryKey.SetValue("HasLUAShield", "", RegistryValueKind.String)
                End If
                registryKey.SetValue("icon", String.Format("{0}{1}{0}", Chr(34), Application.ExecutablePath), RegistryValueKind.String)
                registryKey.SetValue("SuppressionPolicy", 1073741884, RegistryValueKind.DWord)
                registryKey.Close()

                registryKey = registryRootKeyWeAreWorkingWith.OpenSubKey("CLSID\{20D04FE0-3AEA-1069-A2D8-08002B30309D}\Shell\Launch Restore Point Creator\Command", True)
                registryKey.SetValue(vbNullString, String.Format("{0}{1}{0}", Chr(34), Application.ExecutablePath))
                registryKey.Close()
            Catch ex As Security.SecurityException
                toolStripMyComputer.Checked = False
                Functions.eventLogFunctions.writeCrashToEventLog(ex)
                MsgBox("Unable to modify the My Computer right-click context menu. A security violation has occurred. Please contact your system administrator for assistance.", MsgBoxStyle.Critical, strMessageBoxTitle)
            End Try
        Else
            Try
                ' This code removes the options from the "My Computer" right-click context menu.

                ' All of this prevents a rare Null Reference Exception.
                If registryRootKeyWeAreWorkingWith.OpenSubKey("CLSID\{20D04FE0-3AEA-1069-A2D8-08002B30309D}\Shell\Create System Restore Checkpoint", False) IsNot Nothing Then
                    registryRootKeyWeAreWorkingWith.OpenSubKey("CLSID\{20D04FE0-3AEA-1069-A2D8-08002B30309D}\Shell", True).DeleteSubKeyTree("Create System Restore Checkpoint")
                End If

                If registryRootKeyWeAreWorkingWith.OpenSubKey("CLSID\{20D04FE0-3AEA-1069-A2D8-08002B30309D}\Shell\Create Custom Named System Restore", False) IsNot Nothing Then
                    registryRootKeyWeAreWorkingWith.OpenSubKey("CLSID\{20D04FE0-3AEA-1069-A2D8-08002B30309D}\Shell", True).DeleteSubKeyTree("Create Custom Named System Restore")
                End If

                If registryRootKeyWeAreWorkingWith.OpenSubKey("CLSID\{20D04FE0-3AEA-1069-A2D8-08002B30309D}\Shell\Create Custom Named System Restore Point", False) IsNot Nothing Then
                    registryRootKeyWeAreWorkingWith.OpenSubKey("CLSID\{20D04FE0-3AEA-1069-A2D8-08002B30309D}\Shell", True).DeleteSubKeyTree("Create Custom Named System Restore Point")
                End If

                If registryRootKeyWeAreWorkingWith.OpenSubKey("CLSID\{20D04FE0-3AEA-1069-A2D8-08002B30309D}\Shell\Launch Restore Point Creator", False) IsNot Nothing Then
                    registryRootKeyWeAreWorkingWith.OpenSubKey("CLSID\{20D04FE0-3AEA-1069-A2D8-08002B30309D}\Shell", True).DeleteSubKeyTree("Launch Restore Point Creator")
                End If
                ' All of this prevents a rare Null Reference Exception.
            Catch ex As Security.SecurityException
                Functions.eventLogFunctions.writeCrashToEventLog(ex)
                MsgBox("Unable to modify the My Computer right-click context menu. A security violation has occurred. Please contact your system administrator for assistance.", MsgBoxStyle.Critical, strMessageBoxTitle)
            End Try
        End If

        registryRootKeyWeAreWorkingWith.Close()
    End Sub

    Private Sub toolStripLogRestorePointDeletions_Click(sender As Object, e As EventArgs) Handles toolStripLogRestorePointDeletions.Click
        savePreferenceToRegistry("Log Restore Point Deletions", toolStripLogRestorePointDeletions.Checked.ToString)
    End Sub

    Private Sub toolStripCheckForUpdates_Click(sender As Object, e As EventArgs) Handles toolStripCheckForUpdates.Click
        startCheckForUpdatesThread()
    End Sub

    Private Sub ProductWebSiteToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles ProductWebSiteToolStripMenuItem.Click
        Functions.support.launchURLInWebBrowser(globalVariables.webURLs.core.strWebSite, "An error occurred when trying to launch the product's web site URL in your default browser. The URL has been copied to your Windows Clipboard for you to paste into the address bar in the browser of your choice.")
    End Sub

    Private Sub AboutThisProgramToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles AboutThisProgramToolStripMenuItem.Click
        Dim stringBuilder As New StringBuilder

        stringBuilder.AppendLine("Restore Point Creator")
        stringBuilder.AppendLine("Written By Tom Parkison")
        stringBuilder.AppendLine("Copyright Thomas Parkison 2012-2018.")
        stringBuilder.AppendLine()
        stringBuilder.AppendLine("This program uses the Microsoft.Win32.TaskScheduler library version 2.5.23 to interface with the Windows Task Scheduler, copyright David Hall.")
        stringBuilder.AppendLine()

        If globalVariables.version.boolBeta Then
            stringBuilder.AppendFormat("Version {0} Public Beta {1}", globalVariables.version.strFullVersionString, globalVariables.version.shortBetaVersion)
        ElseIf globalVariables.version.boolReleaseCandidate Then
            stringBuilder.AppendFormat("Version {0} Release Candidate {1}", globalVariables.version.strFullVersionString, globalVariables.version.shortReleaseCandidateVersion)
        Else
            stringBuilder.AppendFormat("Version {0}", globalVariables.version.strFullVersionString)
        End If

        If globalVariables.version.boolDebugBuild = True Then
            stringBuilder.Append(" (Debug Build)")
            stringBuilder.AppendLine()
        Else
            stringBuilder.AppendLine()
        End If

        stringBuilder.AppendLine()
        stringBuilder.AppendLine("All operations that have to do with Microsoft Windows System Restore are processed by approved Microsoft APIs, System DLLs, and Microsoft Windows Management Instrumentation APIs.")
        stringBuilder.AppendLine()
        stringBuilder.AppendLine("Windows and Windows System Restore are registered trademarks of Microsoft Corporation in the United States and other countries.")
        'stringBuilder.AppendLine()
        'stringBuilder.AppendLine("The icons and images that are used in this program are property of FindIcons.com and are used with permission of FindIcons.com.")

        MsgBox(stringBuilder.ToString.Trim, MsgBoxStyle.Information, "About")

        stringBuilder = Nothing
    End Sub

    Private Sub toolStripDonate_Click(sender As Object, e As EventArgs) Handles toolStripDonate.Click
        doTheGrayingOfTheRestorePointNameTextBox()
        Dim registryShowDonationMessageValue As String = Registry.LocalMachine.OpenSubKey(globalVariables.registryValues.strKey).GetValue("Show Donation Message", "True").ToString.Trim
        Dim boolRegistryShowDonationMessageValue As Boolean

        If registryShowDonationMessageValue.Equals("true", StringComparison.OrdinalIgnoreCase) Or registryShowDonationMessageValue.Equals("false", StringComparison.OrdinalIgnoreCase) Then
            boolRegistryShowDonationMessageValue = Boolean.Parse(registryShowDonationMessageValue)
        Else
            boolRegistryShowDonationMessageValue = True
        End If

        If boolRegistryShowDonationMessageValue = False Then
            MsgBox("It appears that you have already donated to the developer of this program.  You don't have to donate again but if you want to, I won't argue with you.", MsgBoxStyle.Information, strMessageBoxTitle)
        End If

        launchDonationURL()
    End Sub

    Private Sub toolStripCloseAfterRestorePointIsCreated_Click(sender As Object, e As EventArgs) Handles toolStripCloseAfterRestorePointIsCreated.Click
        If toolStripCloseAfterRestorePointIsCreated.Checked = True Then
            My.Settings.closeAfterCreatingRestorePoint = True
        Else
            My.Settings.closeAfterCreatingRestorePoint = False
        End If
        My.Settings.Save()
    End Sub

    Private Sub OldToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles toolStripDeleteOldRestorePoints.Click
        If My.Settings.maxDaysManualDelete = -1 Then
            Dim frmDeleteOldSystemRestorePointsInstance = New frmDeleteOldSystemRestorePoints With {
                .StartPosition = FormStartPosition.CenterScreen
            }
            frmDeleteOldSystemRestorePointsInstance.ShowDialog()
            frmDeleteOldSystemRestorePointsInstance.Location = My.Settings.DeleteOldSystemRestorePointsWindowLocation

            frmDeleteOldSystemRestorePointsInstance.Dispose()
            frmDeleteOldSystemRestorePointsInstance = Nothing
        Else
            doDeleteOldSystemRestorePoint(My.Settings.maxDaysManualDelete)
        End If
    End Sub

    Private Sub AllToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles toolStripDeleteAllRestorePoints.Click
        Dim result As MsgBoxResult = MsgBox("Are you sure you want to delete all restore points (except the newest one) on this system?" & vbCrLf & vbCrLf & "Remember, this is a one-way action.  There is no way back once you delete them.", MsgBoxStyle.YesNo + MsgBoxStyle.Question, "Are you sure?")

        If result = MsgBoxResult.Yes Then
            disableFormElements()
            openPleaseWaitPanel("Deleting Restore Points... Please Wait.")
            toolStripDeleteAllRestorePoints.Enabled = False
            Threading.ThreadPool.QueueUserWorkItem(AddressOf deleteAllRestorePointsThread)
        Else
            MsgBox("Restore points not deleted.", MsgBoxStyle.Information, strMessageBoxTitle)
        End If
    End Sub

    Private Sub PrivateForTomToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles ToolStripMenuItemPrivateForTom.Click
        toolStripBetaChannel.Checked = False
        toolStripStableChannel.Checked = False

        My.Settings.updateChannel = globalVariables.updateChannels.tom
        My.Settings.Save()
    End Sub

    Private Sub toolStripStableChannel_Click(sender As Object, e As EventArgs) Handles toolStripStableChannel.Click
        toolStripBetaChannel.Checked = False
        ToolStripMenuItemPrivateForTom.Checked = False
        lineUnderRC.Visible = False
        OnlyGiveMeReleaseCandidates.Visible = False

        My.Settings.updateChannel = globalVariables.updateChannels.stable
        My.Settings.Save()

        If IO.File.Exists(globalVariables.pdbFileNameInZIP) = True Then
            Try
                IO.File.Delete(globalVariables.pdbFileNameInZIP)
            Catch ex As Exception
                Dim deleteAtReboot As New Functions.deleteAtReboot()
                deleteAtReboot.addItem(globalVariables.pdbFileNameInZIP)
                deleteAtReboot.dispose(True)
            End Try
        End If

        startCheckForUpdatesThread()
    End Sub

    Private Sub toolStripBetaChannel_Click(sender As Object, e As EventArgs) Handles toolStripBetaChannel.Click
        Dim messageBoxResult As MsgBoxResult = MsgBox("You are activating the Public Beta Update Channel." & vbCrLf & vbCrLf & "This update channel is used for public beta versions in which new features and bug fixes are tested.  These are meant for people who know what they are doing and would like to help in the public testing of new versions." & vbCrLf & vbCrLf & "Are you sure you want to enable the beta update channel?", MsgBoxStyle.Question + MsgBoxStyle.YesNo, "Are you sure?")

        If messageBoxResult = MsgBoxResult.Yes Then
            toolStripStableChannel.Checked = False
            ToolStripMenuItemPrivateForTom.Checked = False
            lineUnderRC.Visible = True
            OnlyGiveMeReleaseCandidates.Visible = True

            My.Settings.updateChannel = globalVariables.updateChannels.beta
            My.Settings.Save()

            startCheckForUpdatesThread()
        Else
            toolStripBetaChannel.Checked = False
            toolStripStableChannel.Checked = True
            ToolStripMenuItemPrivateForTom.Checked = False
            lineUnderRC.Visible = False
            OnlyGiveMeReleaseCandidates.Visible = False

            My.Settings.updateChannel = globalVariables.updateChannels.stable
            My.Settings.Save()
        End If
    End Sub

    Private Sub KeepXAmountOfRestorePointsToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles KeepXAmountOfRestorePointsToolStripMenuItem.Click
        If KeepXAmountOfRestorePointsToolStripMenuItem.Checked = True Then
            Dim Keep_X_Amount_of_Restore_PointsInstance = New createRestorePointAtUserLogon With {
                .parentFormG = Me,
                .StartPosition = FormStartPosition.CenterParent
            }
            Keep_X_Amount_of_Restore_PointsInstance.ShowDialog()

            Keep_X_Amount_of_Restore_PointsInstance.Dispose()
            Keep_X_Amount_of_Restore_PointsInstance = Nothing
        Else
            Dim registryKey As RegistryKey = Registry.LocalMachine.OpenSubKey(globalVariables.registryValues.strKey, True)

            registryKey.SetValue("Keep X Amount of Restore Points", "False", RegistryValueKind.String)
            registryKey.DeleteValue("Keep X Amount of Restore Points Value", False)

            registryKey.Close()

            KeepXAmountOfRestorePointsToolStripMenuItem.Text = "Keep X Amount of Restore Points"
        End If
    End Sub

    Private Sub toolStripCheckEveryWeek_Click(sender As Object, e As EventArgs) Handles toolStripCheckEveryWeek.Click
        toolStripCheckEveryTwoWeeks.Checked = False
        toolStripCheckCustom.Checked = False
        My.Settings.checkForUpdatesEveryInDays = 7
        toolStripCheckCustom.Text = "Custom"
    End Sub

    Private Sub toolStripCheckEveryTwoWeeks_Click(sender As Object, e As EventArgs) Handles toolStripCheckEveryTwoWeeks.Click
        toolStripCheckEveryWeek.Checked = False
        toolStripCheckCustom.Checked = False
        My.Settings.checkForUpdatesEveryInDays = 14
        toolStripCheckCustom.Text = "Custom"
    End Sub

    Private Sub toolStripCheckCustom_Click(sender As Object, e As EventArgs) Handles toolStripCheckCustom.Click
        toolStripCheckEveryWeek.Checked = False
        toolStripCheckEveryTwoWeeks.Checked = False

        Dim checkForUpdatesEveryInstance As New checkForUpdatesEvery With {
            .parentFormG = Me,
            .StartPosition = FormStartPosition.CenterParent
        }
        checkForUpdatesEveryInstance.ShowDialog()
    End Sub

    Private Sub HelpToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles HelpToolStripMenuItem.Click
        Functions.support.launchURLInWebBrowser(globalVariables.webURLs.webPages.strHelpVideos, "An error occurred when trying to launch the Help Videos URL in your default browser. The URL has been copied to your Windows Clipboard for you to paste into the address bar in the browser of your choice.")
    End Sub

    Private Sub SetBarColorToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles SetBarColorToolStripMenuItem.Click
        ColorDialog.CustomColors = Functions.support.loadCustomColors()
        ColorDialog.Color = My.Settings.barColor

        If ColorDialog.ShowDialog() = DialogResult.OK Then
            My.Settings.barColor = ColorDialog.Color
            Functions.support.saveCustomColors(ColorDialog.CustomColors)
            MsgBox("Color Preference Saved.", MsgBoxStyle.Information, "Setting Saved")
        End If
    End Sub

    Private Sub SetPleaseWaitBorderColorToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles SetPleaseWaitBorderColorToolStripMenuItem.Click
        ColorDialog.CustomColors = Functions.support.loadCustomColors()
        ColorDialog.Color = My.Settings.pleaseWaitBorderColor

        If ColorDialog.ShowDialog() = DialogResult.OK Then
            My.Settings.pleaseWaitBorderColor = ColorDialog.Color

            globalVariables.pleaseWaitPanelColor = ColorDialog.Color
            globalVariables.pleaseWaitPanelFontColor = Functions.support.getGoodTextColorBasedUponBackgroundColor(My.Settings.pleaseWaitBorderColor)

            pleaseWaitBorderText.BackColor = globalVariables.pleaseWaitPanelColor
            pleaseWaitBorderText.ForeColor = globalVariables.pleaseWaitPanelFontColor

            Functions.support.saveCustomColors(ColorDialog.CustomColors)
            MsgBox("Color Preference Saved.", MsgBoxStyle.Information, "Setting Saved")
        End If
    End Sub

    Private Sub DefaultCustomRestorePointNameToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles DefaultCustomRestorePointNameToolStripMenuItem.Click
        Dim setDefaultCustomRestorePointNameWindowInstance As New Set_Default_Custom_Restore_Point_Name With {
            .StartPosition = FormStartPosition.CenterParent,
            .parentFormG = Me
        }
        setDefaultCustomRestorePointNameWindowInstance.ShowDialog()
    End Sub

    Private Sub ShowMessageBoxAfterSuccessfulCreationOfRestorePointToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles ShowMessageBoxAfterSuccessfulCreationOfRestorePointToolStripMenuItem.Click
        My.Settings.ShowMessageBoxAfterSuccessfulCreationOfRestorePoint = ShowMessageBoxAfterSuccessfulCreationOfRestorePointToolStripMenuItem.Checked
        My.Settings.Save()
    End Sub

    Private Sub ShowMessageBoxAfterSuccessfulDeletionOfRestorePointsToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles ShowMessageBoxAfterSuccessfulDeletionOfRestorePointsToolStripMenuItem.Click
        My.Settings.ShowMessageBoxAfterSuccessfulDeletionOfRestorePoints = ShowMessageBoxAfterSuccessfulDeletionOfRestorePointsToolStripMenuItem.Checked
        My.Settings.Save()
    End Sub

    Private Sub CheckSystemDrivesForFullShadowStorageToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles CheckSystemDrivesForFullShadowStorageToolStripMenuItem.Click
        My.Settings.checkSystemDrivesForFullShadowStorage = CheckSystemDrivesForFullShadowStorageToolStripMenuItem.Checked
        My.Settings.Save()
    End Sub

    Private Sub toolStripAutomaticallyCheckForUpdates_Click(sender As Object, e As EventArgs) Handles toolStripAutomaticallyCheckForUpdates.Click
        My.Settings.CheckForUpdates = toolStripAutomaticallyCheckForUpdates.Checked
        My.Settings.Save()

        If My.Settings.CheckForUpdates = True Then
            ConfigureAutomaticUpdatesToolStripMenuItem.Visible = True
        Else
            ConfigureAutomaticUpdatesToolStripMenuItem.Visible = False
        End If
    End Sub

    Private Sub toolStripConfirmDeletions_Click(sender As Object, e As EventArgs) Handles toolStripConfirmDeletions.Click
        My.Settings.confirmRestorePointDeletions = toolStripConfirmDeletions.Checked
        My.Settings.Save()
    End Sub

    Private Sub toolStripClear_Click(sender As Object, e As EventArgs) Handles toolStripClear.Click
        toolStripDeleteOldRestorePoints.Text = "Delete Old Restore Points"
        My.Settings.maxDaysManualDelete = -1
        My.Settings.Save()
    End Sub
#End Region

#Region "--== Button Click Event Code ==--"
    Private Sub btnRestoreToRestorePointSafeMode_Click(sender As Object, e As EventArgs) Handles btnRestoreToRestorePointSafeMode.Click
        If systemRestorePointsList.SelectedItems.Count > 1 Then
            MsgBox("You can't have multiple System Restore Points selected for this function to work, you must only select one.", MsgBoxStyle.Information, strMessageBoxTitle)
            Exit Sub
        ElseIf systemRestorePointsList.SelectedItems.Count = 0 Then
            MsgBox("You must select a System Restore Point to restore your system to.", MsgBoxStyle.Information, strMessageBoxTitle)
            Exit Sub
        End If

        Dim selectedRestorePoint As myListViewItemTypes.restorePointEntryItem = DirectCast(systemRestorePointsList.SelectedItems(0), myListViewItemTypes.restorePointEntryItem)

        Dim msgboxResult As MsgBoxResult = MsgBox(String.Format("Are you sure you want to restore your system back to the selected System Restore Point?  Your system will reboot into Safe Mode and perform the restore process there and reboot after the process is complete.{0}{0}Description: {1}{0}Created On: {2}{0}Type: {3}", vbCrLf, selectedRestorePoint.strRestorePointName, selectedRestorePoint.strRestorePointDate, selectedRestorePoint.strRestorePointType), MsgBoxStyle.YesNo + MsgBoxStyle.Question, "Are you sure?")

        If msgboxResult = MsgBoxResult.Yes Then
            Functions.support.setSafeModeBoot() ' Set the system up for Safe Mode Boot.

            ' Set the restore point that we're going to restore back to.
            savePreferenceToRegistry("Preselected Restore Point for Restore in Safe Mode", selectedRestorePoint.strRestorePointID.Trim)

            ' Set this program up to launch at user logon.
            Registry.LocalMachine.OpenSubKey("Software\Microsoft\Windows\CurrentVersion\RunOnce", True).SetValue("*Restore To Restore Point", String.Format("{0}{1}{0} -restoretopoint", Chr(34), Application.ExecutablePath), RegistryValueKind.String)

            ' Then finally reboot the system.
            Functions.support.rebootSystem()
        Else
            MsgBox("Your system has NOT been restored to the selected System Restore Point.", MsgBoxStyle.Information, strMessageBoxTitle)
        End If
    End Sub

    Private Sub btnCreateRestorePointNameWithDefaultName_Click(sender As Object, e As EventArgs) Handles btnCreateRestorePointNameWithDefaultName.Click
        If Functions.support.areWeInSafeMode() = True Then
            MsgBox("You are in Safe Mode, it's not recommended to make restore points in Safe Mode.", MsgBoxStyle.Information, strMessageBoxTitle)
            Exit Sub
        End If

        openPleaseWaitPanel("Creating Restore Point... Please Wait.")
        Threading.ThreadPool.QueueUserWorkItem(Sub() unifiedCreateSystemRestorePoint(defaultCustomRestorePointName))
    End Sub

    Private Sub btnCreate_Click(ByVal sender As Object, ByVal e As EventArgs) Handles btnCreate.Click
        If Functions.support.areWeInSafeMode() = True Then
            MsgBox("You are in Safe Mode, it's not recommended to make restore points in Safe Mode.", MsgBoxStyle.Information, strMessageBoxTitle)
            Exit Sub
        End If

        If txtRestorePointDescription.Text.caseInsensitiveContains(strTypeYourRestorePointName) = True Then
            Exit Sub
        End If

        If String.IsNullOrEmpty(txtRestorePointDescription.Text) Then Exit Sub

        txtRestorePointDescription.Text = txtRestorePointDescription.Text.Trim

        Dim msgBoxResult As MsgBoxResult = MsgBoxResult.Yes

        If My.Settings.askBeforeCreatingRestorePoint = True Then
            msgBoxResult = MsgBox(String.Format("Are you sure you want to create a new system restore point with the name of {0}{1}{0}?", Chr(34), txtRestorePointDescription.Text), MsgBoxStyle.Question + vbYesNo, "Restore Point Creator")
        End If

        If msgBoxResult = MsgBoxResult.Yes Then
            openPleaseWaitPanel("Creating Restore Point... Please Wait.")
            Threading.ThreadPool.QueueUserWorkItem(Sub() unifiedCreateSystemRestorePoint(txtRestorePointDescription.Text))
        End If
    End Sub

    Private Sub btnCreateSystemCheckpoint_Click(ByVal sender As Object, ByVal e As EventArgs) Handles btnCreateSystemCheckpoint.Click
        If Functions.support.areWeInSafeMode() = True Then
            MsgBox("You are in Safe Mode, it's not recommended to make restore points in Safe Mode.", MsgBoxStyle.Information, strMessageBoxTitle)
            Exit Sub
        End If

        Dim msgBoxResult As MsgBoxResult = MsgBoxResult.Yes

        If My.Settings.askBeforeCreatingRestorePoint = True Then
            msgBoxResult = MsgBox("Are you sure you want to create a new system restore point?", MsgBoxStyle.Question + vbYesNo, "Restore Point Creator")
        End If

        If msgBoxResult = MsgBoxResult.Yes Then
            openPleaseWaitPanel("Creating Restore Point... Please Wait.")
            Threading.ThreadPool.QueueUserWorkItem(Sub() unifiedCreateSystemRestorePoint())
        End If
    End Sub

    Private Sub btnRestoreToRestorePoint_Click(sender As Object, e As EventArgs) Handles btnRestoreToRestorePoint.Click
        If systemRestorePointsList.SelectedItems.Count > 1 Then
            MsgBox("You can't have multiple System Restore Points selected for this function to work, you must only select one.", MsgBoxStyle.Information, strMessageBoxTitle)
            Exit Sub
        ElseIf systemRestorePointsList.SelectedItems.Count = 0 Then
            MsgBox("You must select a System Restore Point to restore your system to.", MsgBoxStyle.Information, strMessageBoxTitle)
            Exit Sub
        End If

        Dim selectedRestorePoint As myListViewItemTypes.restorePointEntryItem = DirectCast(systemRestorePointsList.SelectedItems(0), myListViewItemTypes.restorePointEntryItem)

        Dim msgboxResult As MsgBoxResult = MsgBox(String.Format("Are you sure you want to restore your system back to the selected System Restore Point?  Your system will reboot after the restoration process is complete.{0}{0}Description: {1}{0}Created On: {2}{0}Type: {3}", vbCrLf, selectedRestorePoint.strRestorePointName, selectedRestorePoint.strRestorePointDate, selectedRestorePoint.strRestorePointType), MsgBoxStyle.YesNo + MsgBoxStyle.Question, "Are you sure?")

        If msgboxResult = MsgBoxResult.Yes Then
            openPleaseWaitPanel("Beginning the Restore Process... Please Wait.")
            Threading.ThreadPool.QueueUserWorkItem(Sub() restoreSystemRestorePoint(selectedRestorePoint.intRestorePointID))
        Else
            MsgBox("Your system has NOT been restored to the selected System Restore Point.", MsgBoxStyle.Information, strMessageBoxTitle)
        End If
    End Sub

    Private Sub btnRefreshRestorePoints_Click(ByVal sender As Object, ByVal e As EventArgs) Handles btnRefreshRestorePoints.Click
        If btnRefreshRestorePoints.Text.Equals("Refresh List of System Restore Points", StringComparison.OrdinalIgnoreCase) Then
            openPleaseWaitPanel("Loading Restore Points... Please Wait.")
            Threading.ThreadPool.QueueUserWorkItem(AddressOf loadRestorePointsFromSystemIntoList)
        End If
    End Sub

    Private Sub btnDeleteRestorePoint_Click(sender As Object, e As EventArgs) Handles btnDeleteRestorePoint.Click
        If systemRestorePointsList.SelectedItems.Count = 0 Then
            MsgBox("You must select one or more System Restore Points to delete.", MsgBoxStyle.Information, strMessageBoxTitle)
            Exit Sub
        End If

        systemRestorePointsList.Enabled = False

        Dim boolMultipleRestorePointsDeleted As Boolean = False
        If systemRestorePointsList.SelectedItems.Count > 1 Then boolMultipleRestorePointsDeleted = True

        Dim boolUserWantsToDeleteTheRestorePoint As Boolean
        Dim deletionConfirmationWindow As frmConfirmDelete
        Dim restorePointIDsToBeDeleted As New Dictionary(Of String, restorePointInfo)
        Dim strRestorePointName, strRestorePointDate, strRestorePointID, strRestorePointType As String
        Dim boolConfirmDeletions As Boolean = toolStripConfirmDeletions.Checked
        Dim dateRestorePointCreated As Date

        For Each restorePointEntryItem As myListViewItemTypes.restorePointEntryItem In systemRestorePointsList.SelectedItems
            If AllowForDeletionOfAllSystemRestorePointsToolStripMenuItem.Checked = False Then
                ' Checks to see if the user is trying to delete the newest System Restore Point based upon ID.
                If restorePointEntryItem.intRestorePointID = newestSystemRestoreID Then
                    systemRestorePointsList.Enabled = True

                    ' Yep, the user is trying to do that.  Stupid user, now we give that stupid user a message to prevent his/her stupidity.
                    MsgBox("You can't delete the most recent System Restore Point.  This is for your own protection.", MsgBoxStyle.Information, strMessageBoxTitle)
                    Exit Sub
                End If
            End If

            boolUserWantsToDeleteTheRestorePoint = False

            strRestorePointName = restorePointEntryItem.strRestorePointName
            strRestorePointDate = restorePointEntryItem.strRestorePointDate
            strRestorePointID = restorePointEntryItem.strRestorePointID
            strRestorePointType = restorePointEntryItem.strRestorePointType
            dateRestorePointCreated = restorePointEntryItem.dateRestorePointDate

            ' This check is required to make sure that we don't have duplicate key IDs going into the restorePointIDsToBeDeleted Dictionary.
            ' This fixes a bug that was reported by "George".
            If Not restorePointIDsToBeDeleted.ContainsKey(strRestorePointID) Then
                If boolConfirmDeletions And My.Settings.multiConfirmRestorePointDeletions And systemRestorePointsList.SelectedItems.Count > 1 Then
                    restorePointIDsToBeDeleted.Add(strRestorePointID, New restorePointInfo With {.strName = strRestorePointName, .strCreatedDate = strRestorePointDate, .strRestorePointType = strRestorePointType, .dateCreated = dateRestorePointCreated})
                ElseIf boolConfirmDeletions And (Not My.Settings.multiConfirmRestorePointDeletions Or systemRestorePointsList.SelectedItems.Count = 1) Then
                    deletionConfirmationWindow = New frmConfirmDelete

                    deletionConfirmationWindow.lblCreatedOn.Text &= " " & strRestorePointDate
                    deletionConfirmationWindow.lblRestorePointName.Text &= " " & strRestorePointName
                    deletionConfirmationWindow.lblType.Text &= " " & strRestorePointType

                    deletionConfirmationWindow.StartPosition = FormStartPosition.CenterParent
                    deletionConfirmationWindow.ShowDialog(ParentForm)

                    If deletionConfirmationWindow.userResponse = frmConfirmDelete.userResponseENum.yes Then
                        boolUserWantsToDeleteTheRestorePoint = True
                    ElseIf deletionConfirmationWindow.userResponse = frmConfirmDelete.userResponseENum.yesAndDontAskAgain Then
                        boolConfirmDeletions = False
                        boolUserWantsToDeleteTheRestorePoint = True
                    ElseIf deletionConfirmationWindow.userResponse = frmConfirmDelete.userResponseENum.cancel Then
                        systemRestorePointsList.Enabled = True
                        giveFeedbackToUser("Deletion of selected restore points canceled.")
                        Exit Sub
                    Else
                        boolUserWantsToDeleteTheRestorePoint = False
                    End If

                    deletionConfirmationWindow.Dispose()
                    deletionConfirmationWindow = Nothing

                    If boolUserWantsToDeleteTheRestorePoint Then restorePointIDsToBeDeleted.Add(strRestorePointID, New restorePointInfo With {.strName = strRestorePointName, .strCreatedDate = strRestorePointDate, .strRestorePointType = strRestorePointType, .dateCreated = dateRestorePointCreated})
                Else
                    restorePointIDsToBeDeleted.Add(strRestorePointID, New restorePointInfo With {.strName = strRestorePointName, .strCreatedDate = strRestorePointDate, .strRestorePointType = strRestorePointType, .dateCreated = dateRestorePointCreated})
                End If
            End If

            strRestorePointName = Nothing
            strRestorePointDate = Nothing
            strRestorePointID = Nothing
        Next

        If boolConfirmDeletions And My.Settings.multiConfirmRestorePointDeletions And systemRestorePointsList.SelectedItems.Count > 1 Then
            Dim batchConfirmWindow As New Confirm_Restore_Point_Deletions_Form With {
                .restorePointIDsToBeDeleted = restorePointIDsToBeDeleted,
                .StartPosition = FormStartPosition.CenterParent
            }
            batchConfirmWindow.ShowDialog(Me)

            If batchConfirmWindow.userResponse = Confirm_Restore_Point_Deletions_Form.userResponseENum.cancel Then
                systemRestorePointsList.Enabled = True
                giveFeedbackToUser("Deletion of selected restore points canceled.")
                Exit Sub
            End If

            restorePointIDsToBeDeleted = batchConfirmWindow.restorePointIDsToBeDeleted

            batchConfirmWindow.Dispose()
            batchConfirmWindow = Nothing
        End If

        If restorePointIDsToBeDeleted.Count > 0 Then
            openPleaseWaitPanel("Deleting Restore Points... Please Wait.")
            Threading.ThreadPool.QueueUserWorkItem(Sub() deleteSelectedRestorePoints(restorePointIDsToBeDeleted, toolStripLogRestorePointDeletions.Checked))
        Else
            systemRestorePointsList.Enabled = True
            giveFeedbackToUser("No System Restore Points were deleted.")
        End If
    End Sub
#End Region

#Region "--== DON'T TOUCH THIS ==--"
    Public Sub New()
        ' This call is required by the designer.
        InitializeComponent()

        ' Add any initialization after the InitializeComponent() call.
    End Sub
#End Region

#Region "--== Please Wait Panel Code ==--"
    Private strPleaseWaitLabelText As String

    Private Sub centerPleaseWaitPanel()
        pleaseWaitPanel.Location = New Point(
            (Me.ClientSize.Width / 2) - (pleaseWaitPanel.Size.Width / 2),
            (Me.ClientSize.Height / 2) - (pleaseWaitPanel.Size.Height / 2))
        pleaseWaitPanel.Anchor = AnchorStyles.None
    End Sub

    Private Sub openPleaseWaitPanel(strInputPleaseWaitLabelText As String)
        Functions.support.disableControlsOnForm(Me)

        strPleaseWaitLabelText = strInputPleaseWaitLabelText
        pleaseWaitProgressBar.ProgressBarColor = My.Settings.barColor
        pleaseWaitlblLabel.Text = strInputPleaseWaitLabelText
        centerPleaseWaitPanel()
        pleaseWaitPanel.Visible = True
        pleaseWaitProgressBar.Value = 0
        pleaseWaitProgressBarChanger.Enabled = True
        pleaseWaitMessageChanger.Enabled = True
        pleaseWaitBorderText.BackColor = globalVariables.pleaseWaitPanelColor
        pleaseWaitBorderText.ForeColor = globalVariables.pleaseWaitPanelFontColor
    End Sub

    Private Sub closePleaseWaitPanel()
        Functions.support.enableControlsOnForm(Me)

        pleaseWaitPanel.Visible = False
        pleaseWaitProgressBarChanger.Enabled = False
        pleaseWaitMessageChanger.Enabled = False
        pleaseWaitProgressBar.Value = 0
    End Sub

    Private Sub pleaseWaitProgressBarChanger_Tick(sender As Object, e As EventArgs) Handles pleaseWaitProgressBarChanger.Tick
        If pleaseWaitProgressBar.Value < 100 Then
            pleaseWaitProgressBar.Value += 1
        Else
            pleaseWaitProgressBar.Value = 0
        End If
    End Sub

    Private Sub pleaseWaitMessageChanger_Tick(sender As Object, e As EventArgs) Handles pleaseWaitMessageChanger.Tick
        If pleaseWaitBorderText.Text = "Please Wait..." Then
            pleaseWaitBorderText.Text = "Please Wait"
            pleaseWaitlblLabel.Text = strPleaseWaitLabelText
        ElseIf pleaseWaitBorderText.Text = "Please Wait" Then
            pleaseWaitBorderText.Text = "Please Wait."
            pleaseWaitlblLabel.Text = strPleaseWaitLabelText & "."
        ElseIf pleaseWaitBorderText.Text = "Please Wait." Then
            pleaseWaitBorderText.Text = "Please Wait.."
            pleaseWaitlblLabel.Text = strPleaseWaitLabelText & ".."
        ElseIf pleaseWaitBorderText.Text = "Please Wait.." Then
            pleaseWaitBorderText.Text = "Please Wait..."
            pleaseWaitlblLabel.Text = strPleaseWaitLabelText & "..."
        End If
    End Sub
#End Region
End Class