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

    Private checkForAndEnableSystemRestoreIfNeeded, checkRestorePointSpaceThread, startSystemRestorePointListLoadThread As Threading.Thread
    Private formLoadCheckForUpdatesRoutineThread, userInitiatedCheckForUpdatesThread, deleteThread, updateRestorePointListThread As Threading.Thread
    Private deleteAllRestorePointsThreadInstance As Threading.Thread

    Private restorePointDateData As New Dictionary(Of String, String)
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

    Private Sub startSystemRestorePointListLoadThreadKiller_Tick(sender As Object, e As EventArgs) Handles startSystemRestorePointListLoadThreadKiller.Tick
        Try
            If startSystemRestorePointListLoadThread IsNot Nothing Then
                If startSystemRestorePointListLoadThread.IsAlive = True Then
                    startSystemRestorePointListLoadThread.Abort()
                End If
            End If
        Catch ex As Exception
        Finally
            startSystemRestorePointListLoadThreadKiller.Enabled = False
            startSystemRestorePointListLoadThread = Nothing
        End Try
    End Sub

    Private Sub checkRestorePointSpaceThreadThreadKiller_Tick(sender As Object, e As EventArgs) Handles checkRestorePointSpaceThreadThreadKiller.Tick
        Try
            If checkRestorePointSpaceThread IsNot Nothing Then
                If checkRestorePointSpaceThread.IsAlive = True Then
                    checkRestorePointSpaceThread.Abort()
                End If
            End If
        Catch ex As Exception
        Finally
            checkRestorePointSpaceThreadThreadKiller.Enabled = False
            checkRestorePointSpaceThread = Nothing
        End Try
    End Sub

    Private Sub checkForAndEnableSystemRestoreIfNeededThreadKiller_Tick(sender As Object, e As EventArgs) Handles checkForAndEnableSystemRestoreIfNeededThreadKiller.Tick
        Try
            If checkForAndEnableSystemRestoreIfNeeded IsNot Nothing Then
                If checkForAndEnableSystemRestoreIfNeeded.IsAlive = True Then
                    checkForAndEnableSystemRestoreIfNeeded.Abort()
                End If
            End If
        Catch ex As Exception
        Finally
            checkForAndEnableSystemRestoreIfNeededThreadKiller.Enabled = False
            checkForAndEnableSystemRestoreIfNeeded = Nothing
        End Try
    End Sub
#End Region

#Region "--== Form Load Event Sub-Routines ==--"
    Sub checkForMyComputerRightClickOption()
        Try
            If Functions.osVersionInfo.isThisWindows10() = True Then
                ' Apparently Microsoft doesn't want anyone adding anything to the My Computer context menu since they have completely made it impossible
                ' to do so via Registry Access Permission settings. So, we disable this part of the program and hide it from users.
                toolStripMyComputer.Visible = False
                toolStripMyComputer.Enabled = False
                Exit Sub
            End If

            Dim valueInRegistry As String
            Dim iconPath As String
            Dim registryKey As RegistryKey
            Dim matches As Match

            ' This checks to see if we need to rename the Registry key for the "My Computer" right-click context menu.
            ' First we check to see if "Create Custom Named System Restore" exists, if it does then we go onto checking if
            ' "Create Custom Named System Restore Point" doesn't exist.  If both "Create Custom Named System Restore" exists
            ' AND "Create Custom Named System Restore Point" doesn't exist, then we know that we have to rename the
            ' "Create Custom Named System Restore" to "Create Custom Named System Restore Point".
            Try
                If (Registry.ClassesRoot.OpenSubKey("CLSID\{20D04FE0-3AEA-1069-A2D8-08002B30309D}\Shell\Create Custom Named System Restore", False) Is Nothing) = False And (Registry.ClassesRoot.OpenSubKey("CLSID\{20D04FE0-3AEA-1069-A2D8-08002B30309D}\Shell\Create Custom Named System Restore Point", False) Is Nothing) = True Then
                    'debug.writeline("renaming registry key")
                    Functions.registryStuff.renameRegistrySubKey(Registry.ClassesRoot, "CLSID\{20D04FE0-3AEA-1069-A2D8-08002B30309D}\Shell\Create Custom Named System Restore", "CLSID\{20D04FE0-3AEA-1069-A2D8-08002B30309D}\Shell\Create Custom Named System Restore Point")
                End If
            Catch ex As Exception
            End Try

            If Registry.ClassesRoot.OpenSubKey("CLSID\{20D04FE0-3AEA-1069-A2D8-08002B30309D}\Shell\Create System Restore Checkpoint", False) IsNot Nothing Then
                toolStripMyComputer.Checked = True

                If Registry.ClassesRoot.OpenSubKey("CLSID\{20D04FE0-3AEA-1069-A2D8-08002B30309D}\Shell\Create System Restore Checkpoint\command", False) IsNot Nothing Then
                    valueInRegistry = Registry.ClassesRoot.OpenSubKey("CLSID\{20D04FE0-3AEA-1069-A2D8-08002B30309D}\Shell\Create System Restore Checkpoint\command", False).GetValue(vbNullString, Nothing)

                    If valueInRegistry <> Nothing Then
                        ' We check if the current Registry path is different than the current process's EXE path.
                        If valueInRegistry.Contains(Application.ExecutablePath.ToLower) = False Then
                            ' OK, it doesn't match the current process's EXE path.

                            ' We parse out the EXE's path out of the combined path with the argument.
                            matches = Regex.Match(valueInRegistry, "((?:""|'){0,1}[A-Za-z]:\\.*\.(?:bat|bin|cmd|com|cpl|exe|gadget|inf1|ins|inx|isu|job|jse|lnk|msc|msi|msp|mst|paf|pif|ps1|reg|rgs|sct|shb|shs|u3p|vb|vbe|vbs|vbscript|ws|wsf)(?:""|'){0,1} {0,1})(.*)", RegexOptions.IgnoreCase)

                            If matches IsNot Nothing Then
                                ' Now we make sure that the file exists.
                                If IO.File.Exists(matches.Groups(1).Value.Replace(Chr(34), "").Trim) = False Then
                                    '  OK, it doesn't.  The entries in the Registry are invalid, now let's fix them.
                                    registryKey = Registry.ClassesRoot.OpenSubKey("CLSID\{20D04FE0-3AEA-1069-A2D8-08002B30309D}\Shell\Create System Restore Checkpoint\Command", True)

                                    If registryKey IsNot Nothing Then ' This should prevent Null Reference Exceptions.
                                        registryKey.SetValue(vbNullString, String.Format("{0}{1}{0} -createrestorepoint", Chr(34), Application.ExecutablePath))
                                        registryKey.SetValue("icon", String.Format("{0}{1}{0}", Chr(34), Application.ExecutablePath), RegistryValueKind.String)
                                        registryKey.Close()
                                        registryKey.Dispose()
                                    End If
                                End If
                            End If

                            matches = Nothing
                        End If
                    End If
                End If

                matches = Nothing
                valueInRegistry = Nothing

                If Registry.ClassesRoot.OpenSubKey("CLSID\{20D04FE0-3AEA-1069-A2D8-08002B30309D}\Shell\Create Custom Named System Restore Point\command", False) IsNot Nothing Then
                    valueInRegistry = Registry.ClassesRoot.OpenSubKey("CLSID\{20D04FE0-3AEA-1069-A2D8-08002B30309D}\Shell\Create Custom Named System Restore Point\command", False).GetValue(vbNullString, Nothing)

                    If valueInRegistry <> Nothing Then
                        ' We check if the current Registry path is different than the current process's EXE path.
                        If valueInRegistry.Contains(Application.ExecutablePath.ToLower) = False Then
                            ' OK, it doesn't match the current process's EXE path.

                            ' We parse out the EXE's path out of the combined path with the argument.
                            matches = Regex.Match(valueInRegistry, "((?:""|'){0,1}[A-Za-z]:\\.*\.(?:bat|bin|cmd|com|cpl|exe|gadget|inf1|ins|inx|isu|job|jse|lnk|msc|msi|msp|mst|paf|pif|ps1|reg|rgs|sct|shb|shs|u3p|vb|vbe|vbs|vbscript|ws|wsf)(?:""|'){0,1} {0,1})(.*)", RegexOptions.IgnoreCase)

                            If matches IsNot Nothing Then
                                ' Now we make sure that the file exists.
                                If IO.File.Exists(matches.Groups(1).Value.Replace(Chr(34), "").Trim) = False Then
                                    ' OK, it doesn't.  The entries in the Registry are invalid, now let's fix them.
                                    registryKey = Registry.ClassesRoot.OpenSubKey("CLSID\{20D04FE0-3AEA-1069-A2D8-08002B30309D}\Shell\Create Custom Named System Restore Point\Command", True)

                                    If registryKey IsNot Nothing Then ' This should prevent Null Reference Exceptions.
                                        registryKey.SetValue(vbNullString, String.Format("{0}{1}{0} -createrestorepointcustomname", Chr(34), Application.ExecutablePath))
                                        registryKey.SetValue("icon", String.Format("{0}{1}{0}", Chr(34), Application.ExecutablePath), RegistryValueKind.String)
                                        registryKey.Close()
                                        registryKey.Dispose()
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
                If Registry.ClassesRoot.OpenSubKey("CLSID\{20D04FE0-3AEA-1069-A2D8-08002B30309D}\Shell\Launch Restore Point Creator\command", False) IsNot Nothing Then
                    ' OK, it does exist so let's go on with the process of checking for the validity of the entries.
                    valueInRegistry = Registry.ClassesRoot.OpenSubKey("CLSID\{20D04FE0-3AEA-1069-A2D8-08002B30309D}\Shell\Launch Restore Point Creator\command", False).GetValue(vbNullString, Nothing)

                    If valueInRegistry <> Nothing Then
                        ' We check if the current Registry path is different than the current process's EXE path.
                        If valueInRegistry.Contains(Application.ExecutablePath.ToLower) = False Then
                            ' OK, it doesn't match the current process's EXE path.

                            ' We parse out the EXE's path out of the combined path with the argument.
                            matches = Regex.Match(valueInRegistry, "((?:""|'){0,1}[A-Za-z]:\\.*\.(?:bat|bin|cmd|com|cpl|exe|gadget|inf1|ins|inx|isu|job|jse|lnk|msc|msi|msp|mst|paf|pif|ps1|reg|rgs|sct|shb|shs|u3p|vb|vbe|vbs|vbscript|ws|wsf)(?:""|'){0,1} {0,1})(.*)", RegexOptions.IgnoreCase)

                            If matches IsNot Nothing Then
                                ' Now we make sure that the file exists.
                                If IO.File.Exists(matches.Groups(1).Value.Replace(Chr(34), "").Trim) = False Then
                                    ' OK, it doesn't.  The entries in the Registry are invalid, now let's fix them.
                                    registryKey = Registry.ClassesRoot.OpenSubKey("CLSID\{20D04FE0-3AEA-1069-A2D8-08002B30309D}\Shell\Launch Restore Point Creator\Command", True)

                                    If registryKey IsNot Nothing Then ' This should prevent Null Reference Exceptions.
                                        registryKey.SetValue(vbNullString, String.Format("{0}{1}{0}", Chr(34), Application.ExecutablePath))
                                        registryKey.SetValue("icon", String.Format("{0}{1}{0}", Chr(34), Application.ExecutablePath), RegistryValueKind.String)
                                        registryKey.Close()
                                        registryKey.Dispose()
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

                    If Registry.ClassesRoot.OpenSubKey("CLSID\{20D04FE0-3AEA-1069-A2D8-08002B30309D}\Shell") IsNot Nothing Then
                        Registry.ClassesRoot.OpenSubKey("CLSID\{20D04FE0-3AEA-1069-A2D8-08002B30309D}\Shell", True).CreateSubKey("Launch Restore Point Creator")
                        Registry.ClassesRoot.OpenSubKey("CLSID\{20D04FE0-3AEA-1069-A2D8-08002B30309D}\Shell\Launch Restore Point Creator", True).CreateSubKey("Command")

                        registryKey = Registry.ClassesRoot.OpenSubKey("CLSID\{20D04FE0-3AEA-1069-A2D8-08002B30309D}\Shell\Launch Restore Point Creator", True)
                        registryKey.SetValue("HasLUAShield", "", RegistryValueKind.String)
                        registryKey.SetValue("icon", String.Format("{0}{1}{0}", Chr(34), Application.ExecutablePath))
                        registryKey.SetValue("SuppressionPolicy", 1073741884, RegistryValueKind.DWord)
                        registryKey.Close()
                        registryKey.Dispose()

                        registryKey = Registry.ClassesRoot.OpenSubKey("CLSID\{20D04FE0-3AEA-1069-A2D8-08002B30309D}\Shell\Launch Restore Point Creator\Command", True)
                        registryKey.SetValue(vbNullString, String.Format("{0}{1}{0}", Chr(34), Application.ExecutablePath))
                        registryKey.Close()
                        registryKey.Dispose()
                    End If
                End If

                matches = Nothing

                If Registry.ClassesRoot.OpenSubKey("CLSID\{20D04FE0-3AEA-1069-A2D8-08002B30309D}\Shell\Create System Restore Checkpoint") IsNot Nothing Then
                    iconPath = Registry.ClassesRoot.OpenSubKey("CLSID\{20D04FE0-3AEA-1069-A2D8-08002B30309D}\Shell\Create System Restore Checkpoint").GetValue("icon", "nothing")
                    If iconPath.ToLower.Contains(Application.ExecutablePath.ToLower) = False Then
                        Registry.ClassesRoot.OpenSubKey("CLSID\{20D04FE0-3AEA-1069-A2D8-08002B30309D}\Shell\Create System Restore Checkpoint", True).SetValue("icon", String.Format("{0}{1}{0}", Chr(34), Application.ExecutablePath))
                    End If
                End If

                If Registry.ClassesRoot.OpenSubKey("CLSID\{20D04FE0-3AEA-1069-A2D8-08002B30309D}\Shell\Create Custom Named System Restore Point") IsNot Nothing Then
                    iconPath = Registry.ClassesRoot.OpenSubKey("CLSID\{20D04FE0-3AEA-1069-A2D8-08002B30309D}\Shell\Create Custom Named System Restore Point").GetValue("icon", "nothing")
                    If iconPath.ToLower.Contains(Application.ExecutablePath.ToLower) = False Then
                        Registry.ClassesRoot.OpenSubKey("CLSID\{20D04FE0-3AEA-1069-A2D8-08002B30309D}\Shell\Create Custom Named System Restore Point", True).SetValue("icon", String.Format("{0}{1}{0}", Chr(34), Application.ExecutablePath))
                    End If
                End If

                If Registry.ClassesRoot.OpenSubKey("CLSID\{20D04FE0-3AEA-1069-A2D8-08002B30309D}\Shell\Launch Restore Point Creator") IsNot Nothing Then
                    iconPath = Registry.ClassesRoot.OpenSubKey("CLSID\{20D04FE0-3AEA-1069-A2D8-08002B30309D}\Shell\Launch Restore Point Creator").GetValue("icon", "nothing")
                    If iconPath.ToLower.Contains(Application.ExecutablePath.ToLower) = False Then
                        Registry.ClassesRoot.OpenSubKey("CLSID\{20D04FE0-3AEA-1069-A2D8-08002B30309D}\Shell\Launch Restore Point Creator", True).SetValue("icon", String.Format("{0}{1}{0}", Chr(34), Application.ExecutablePath))
                    End If
                End If
            End If
        Catch ex As Exception
            Threading.Thread.CurrentThread.CurrentUICulture = New Globalization.CultureInfo("en-US")
            exceptionHandler.manuallyLoadCrashWindow(ex, ex.Message, ex.StackTrace, ex.GetType)
        End Try
    End Sub

    Sub checkScheduledTaskEXEPathsSubRoutine(ByRef taskService As TaskScheduler.TaskService, ByRef task As TaskScheduler.Task, commandLineArgument As String)
        If Functions.support.areWeInSafeMode() = True Then Exit Sub

        Try
            Dim actions As TaskScheduler.ActionCollection = task.Definition.Actions

            For Each action As TaskScheduler.Action In actions
                If action.ActionType = TaskScheduler.TaskActionType.Execute Then
                    Dim execActionPath As String = DirectCast(action, TaskScheduler.ExecAction).Path.ToLower.Replace("""", "")

                    ' We check if the current task ExecAction path is different than the current process's EXE path.
                    If execActionPath <> Application.ExecutablePath.ToLower Then
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

    Sub checkScheduledTaskEXEPaths()
        If Functions.support.areWeInSafeMode() = True Then Exit Sub
        If Debugger.IsAttached = True Then Exit Sub

        Try
            Dim taskService As New TaskScheduler.TaskService

            For Each task As TaskScheduler.Task In taskService.RootFolder.Tasks
                If task.Name = "System Restore Checkpoint by System Restore Point Creator" Then
                    checkScheduledTaskEXEPathsSubRoutine(taskService, task, "-createscheduledrestorepoint")
                ElseIf task.Name = "Delete Old Restore Points" Then
                    checkScheduledTaskEXEPathsSubRoutine(taskService, task, "-deleteoldrestorepoints")
                ElseIf task.Name = "Create a Restore Point at User Logon" Then
                    checkScheduledTaskEXEPathsSubRoutine(taskService, task, "-createscheduledrestorepoint")
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

    Sub loadPreferences()
        Try
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

            If My.Settings.updateChannel = globalVariables.updateChannels.stable Then
                toolStripStableChannel.Checked = True
                lineUnderRC.Visible = False
                OnlyGiveMeReleaseCandidates.Visible = False
            ElseIf My.Settings.updateChannel = globalVariables.updateChannels.beta Then
                toolStripBetaChannel.Checked = True
                lineUnderRC.Visible = True
                OnlyGiveMeReleaseCandidates.Visible = True
                OnlyGiveMeReleaseCandidates.Checked = My.Settings.onlyGiveMeRCs
            ElseIf My.Settings.updateChannel = globalVariables.updateChannels.tom Then
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

            If My.Settings.notificationType = globalVariables.notificationTypeBalloon Then
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
            LogProgramLoadsAndExitsToEventLogToolStripMenuItem.Checked = globalVariables.boolLogLoadsAndExitsToEventLog
            UseSSLToolStripMenuItem.Checked = My.Settings.useSSL
            AskBeforeUpgradingUpdatingToolStripMenuItem.Checked = My.Settings.askToUpgrade
            Me.Location = My.Settings.mainWindowPosition
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
                boolUpdateAtNextRunTime = Functions.support.getBooleanValueFromRegistry(registryObject, "UpdateAtNextRunTime", False)
                registryObject.DeleteValue("UpdateAtNextRunTime", False)

                boolShowDonationMessage = Functions.support.getBooleanValueFromRegistry(registryObject, "Show Donation Message", True)

                ' Converts some settings over to Registry-based Settings.
                If registryObject.GetValue("Log Restore Point Deletions", Nothing) = Nothing Then
                    registryObject.SetValue("Log Restore Point Deletions", My.Settings.boolLogDeletedRestorePoints.ToString)
                End If

                If registryObject.GetValue("Delete Old Restore Points", Nothing) = Nothing Then
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
                boolNoTask = Functions.support.getBooleanValueFromRegistry(registryObject, "No Task", False)
                BypassNoUACLauncherToolStripMenuItem.Checked = boolNoTask

                boolExtendedLoggingForScheduledTasks = Functions.support.getBooleanValueFromRegistry(registryObject, "Extended Logging For Scheduled Tasks", True)
                ExtendedLoggingForScheduledTasks.Checked = boolExtendedLoggingForScheduledTasks

                boolLogRestorePointDeletions = Functions.support.getBooleanValueFromRegistry(registryObject, "Log Restore Point Deletions", True)
                toolStripLogRestorePointDeletions.Checked = boolLogRestorePointDeletions

                globalVariables.boolExtendedLoggingDuringUpdating = Functions.support.getBooleanValueFromRegistry(registryObject, "Enable Extended Logging During Updating", "True")
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

                formLoadCheckForUpdatesRoutineThread = New Threading.Thread(Sub() formLoadCheckForUpdatesRoutine(True))
                formLoadCheckForUpdatesRoutineThread.Name = "Form Load Check For Updates Routine Thread"
                formLoadCheckForUpdatesRoutineThread.Start()
            End If

            If My.Settings.CheckForUpdates = True And boolDidWeAlreadyLaunchTheCheckForUpdatesRoutine = False Then
                toolStripAutomaticallyCheckForUpdates.Checked = True

                formLoadCheckForUpdatesRoutineThread = New Threading.Thread(Sub() formLoadCheckForUpdatesRoutine())
                formLoadCheckForUpdatesRoutineThread.Name = "Form Load Check For Updates Routine Thread"
                formLoadCheckForUpdatesRoutineThread.Start()
            End If
        Catch ex As Exception
            Threading.Thread.CurrentThread.CurrentUICulture = New Globalization.CultureInfo("en-US")
            exceptionHandler.manuallyLoadCrashWindow(ex, ex.Message, ex.StackTrace, ex.GetType)
        End Try
    End Sub

    Sub addSpecialRegistryKeysToWindows8ToFixWindows8SystemRestorePoint()
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
    Public Enum userFeedbackType As Short
        typeError = 0
        typeInfo = 1
        typeWarning = 2
    End Enum

    Sub giveFeedbackToUser(feedBackMessage As String, Optional feedbackType As userFeedbackType = userFeedbackType.typeInfo)
        If My.Settings.notificationType = globalVariables.notificationTypeMessageBox Then
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

    Sub savePreferenceToRegistry(variableName As String, variableValue As String)
        Dim registryKeyObject As RegistryKey = Registry.LocalMachine.OpenSubKey(globalVariables.registryValues.strKey, True)

        If registryKeyObject IsNot Nothing Then
            registryKeyObject.SetValue(variableName, variableValue, RegistryValueKind.String)

            registryKeyObject.Close()
            registryKeyObject.Dispose()
        End If
    End Sub

    Sub applySavedSorting()
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

    Sub startSystemRestorePointListLoadThreadSub()
        Try
            startSystemRestorePointListLoadThreadKiller.Enabled = True
            loadRestorePointsFromSystemIntoList()
            startSystemRestorePointListLoadThread = Nothing
            startSystemRestorePointListLoadThreadKiller.Enabled = False
        Catch ex As Threading.ThreadAbortException
        End Try
    End Sub

    Sub interfaceTooSmallSettingCheckFormLoadSubRoutine()
        Try
            Dim registryKeyWeAreWorkingWith As RegistryKey = Registry.LocalMachine.OpenSubKey("SOFTWARE\Microsoft\Windows NT\CurrentVersion\AppCompatFlags\Layers", False)

            ' We don't want a Null Reference Exception so we check to see if the Registry Object we just created in Null.
            If registryKeyWeAreWorkingWith IsNot Nothing Then
                ' OK, it isn't so let's continue with the checking.

                ' We now get the current value of our special Registry value. If the value doesn't exist then the default value that the .NET Framework should return is "Nothing".
                Dim valueInRegistry As String = registryKeyWeAreWorkingWith.GetValue(Process.GetCurrentProcess.MainModule.FileName.ToLower, "Nothing")

                ' Now we check to see if the value isn't equal to "Nothing" and if it contains the word "HIGHDPIAWARE".
                If valueInRegistry.Equals("Nothing") = False And valueInRegistry.caseInsensitiveContains("HIGHDPIAWARE") = True Then
                    InterfaceTooBigToolStripMenuItem.Checked = True
                End If

                ' Now we dispose of the Registry Access Objects.
                registryKeyWeAreWorkingWith.Close()
                registryKeyWeAreWorkingWith.Dispose()
            End If
        Catch ex As Exception
            ' Does nothing
        End Try

        'End If
    End Sub

    Function openUpdateDialog(versionUpdateType As Update_Message.versionUpdateType, Optional newVersionString As String = "") As Update_Message.userResponse
        Dim updateMessageDialog As New Update_Message

        updateMessageDialog.StartPosition = FormStartPosition.CenterScreen
        updateMessageDialog.versionUpdate = versionUpdateType
        updateMessageDialog.newVersionString = newVersionString

        updateMessageDialog.ShowDialog()

        Dim response As Update_Message.userResponse = updateMessageDialog.dialogResponse

        updateMessageDialog.Dispose()
        updateMessageDialog = Nothing

        Return response
    End Function

    Sub addRPSessionInterval()
        Try
            If Registry.LocalMachine.OpenSubKey("SOFTWARE\Microsoft\Windows NT\CurrentVersion\SystemRestore") IsNot Nothing Then
                Dim regKey As RegistryKey = Registry.LocalMachine.OpenSubKey("SOFTWARE\Microsoft\Windows NT\CurrentVersion\SystemRestore", True)

                Dim value As Object = regKey.GetValue("RPSessionInterval", Nothing)

                If value = Nothing Then
                    regKey.SetValue("RPSessionInterval", 1, RegistryValueKind.DWord)
                ElseIf value <> 1 Then
                    regKey.SetValue("RPSessionInterval", 1, RegistryValueKind.DWord)
                End If

                regKey.Close()
                regKey.Dispose()
            End If
        Catch ex As Exception
            Functions.eventLogFunctions.writeCrashToEventLog(ex)
        End Try
    End Sub

    Sub addRPGlobalInterval()
        Try
            If Registry.LocalMachine.OpenSubKey("SOFTWARE\Microsoft\Windows NT\CurrentVersion\SystemRestore") IsNot Nothing Then
                Dim regKey As RegistryKey = Registry.LocalMachine.OpenSubKey("SOFTWARE\Microsoft\Windows NT\CurrentVersion\SystemRestore", True)

                Dim value As Object = regKey.GetValue("RPGlobalInterval", Nothing)

                If value = Nothing Then
                    regKey.SetValue("RPGlobalInterval", 1, RegistryValueKind.DWord)
                ElseIf value <> 1 Then
                    regKey.SetValue("RPGlobalInterval", 1, RegistryValueKind.DWord)
                End If

                regKey.Close()
                regKey.Dispose()
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
                    Functions.support.createShortcut(pathOfShortcutWeAreGoingToMake, Application.ExecutablePath, Application.ExecutablePath, "Restore Point Creator Event Log Viewer", "-eventlog")
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
                registryKey.Dispose()
            End If
        Else
            registryKey = RegistryKey.OpenBaseKey(RegistryHive.LocalMachine, RegistryView.Registry64).OpenSubKey("SOFTWARE\Microsoft\Windows NT\CurrentVersion\SystemRestore", True)

            If registryKey IsNot Nothing Then
                Try
                    registryKey.DeleteValue("RPLifeInterval", False)
                Catch ex As Exception
                End Try

                registryKey.Close()
                registryKey.Dispose()
            End If

            registryKey = Registry.LocalMachine.OpenSubKey("SOFTWARE\Microsoft\Windows NT\CurrentVersion\SystemRestore", True)

            If registryKey IsNot Nothing Then
                Try
                    registryKey.DeleteValue("RPLifeInterval", False)
                Catch ex As Exception
                End Try

                registryKey.Close()
                registryKey.Dispose()
            End If
        End If
    End Sub

    Private Sub newFileDeleterThreadSub()
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
            Functions.APIs.MoveFileEx(Application.ExecutablePath & ".new.exe", vbNullString, 4)

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
        Catch ex As Threading.ThreadAbortException
        Catch ex As Exception
        End Try
    End Sub

    Private Sub checkRestorePointSpaceThreadSub()
        Try
            checkRestorePointSpaceThreadThreadKiller.Enabled = True
            Functions.vss.checkSystemDrivesForFullShadowStorage()
            checkRestorePointSpaceThread = Nothing
            checkRestorePointSpaceThreadThreadKiller.Enabled = False
        Catch ex As Threading.ThreadAbortException
        Catch ex As Exception
        End Try
    End Sub

    Private Sub checkForAndEnableSystemRestoreIfNeededSub()
        Try
            checkForAndEnableSystemRestoreIfNeededThreadKiller.Enabled = True
            Functions.vss.checkForAndEnableSystemRestoreIfNeeded()
            checkForAndEnableSystemRestoreIfNeeded = Nothing
            checkForAndEnableSystemRestoreIfNeededThreadKiller.Enabled = False
        Catch ex As Threading.ThreadAbortException
        Catch ex As Exception
        End Try
    End Sub

    Sub showDonationNotice()
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

    Sub launchDonationURL()
        Functions.support.launchURLInWebBrowser(globalVariables.webURLs.webPages.strPayPal, "An error occurred when trying to launch the donation URL in your default browser. The donation URL has been copied to your Windows Clipboard for you to paste into the address bar in the browser of your choice.")
    End Sub

    Sub killThread(ByRef threadToKill As Threading.Thread)
        Try
            If threadToKill IsNot Nothing Then
                threadToKill.Abort()
            End If
        Catch ex As Exception
        End Try
    End Sub

    Private Function calculateConfigBackupDataPayloadChecksum(strInput As String, strSaltedString As String) As String
        Dim string1, string2, string3, string4 As String
        string1 = strInput.Substring(0, 30)
        string2 = strInput.Substring(29, 30)
        string3 = strInput.Substring(59, 30)
        string4 = strInput.Substring(89, 30)

        Dim compoundString As String = strInput & string4 & strSaltedString & string1 & strInput & strSaltedString & string4 & string3 & strInput & strSaltedString & string2 & string1 & strInput & strSaltedString

        Return SHA512ChecksumString(compoundString)
    End Function

    Private Function SHA512ChecksumString(input As String) As String
        Dim SHA1Engine As New Security.Cryptography.SHA512CryptoServiceProvider
        Dim inputAsByteArray As Byte() = Encoding.ASCII.GetBytes(input)
        Dim hashedByteArray As Byte() = SHA1Engine.ComputeHash(inputAsByteArray)
        Return BitConverter.ToString(hashedByteArray).ToLower().Replace("-", "").Trim
    End Function

    Private Sub switchToDebugBuildDownloadThreadSub()
        Try
            Dim memoryStream As New IO.MemoryStream()

            If Functions.http.downloadFile(globalVariables.webURLs.updateBranch.debug.strProgramZIP, memoryStream) = False Then
                Functions.wait.closePleaseWaitWindow()
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

    Sub doTheGrayingOfTheRestorePointNameTextBox()
        If txtRestorePointDescription.Text.caseInsensitiveContains(strTypeYourRestorePointName) = True Then
            txtRestorePointDescription.ForeColor = Color.DimGray
            btnCreate.Enabled = False
        ElseIf txtRestorePointDescription.Text.Trim = Nothing Then
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

            My.Settings.restorePointListColumnOrder = New Specialized.StringCollection
            My.Settings.restorePointListColumnOrder.AddRange(columnIndexes)
        Catch ex As Exception
            Functions.eventLogFunctions.writeToSystemEventLog("Error saving restore point list column ordering.", EventLogEntryType.Error)
            Functions.eventLogFunctions.writeCrashToEventLog(ex)
        End Try
    End Sub

    Private Sub loadRestorePointListColumnOrder()
        Try
            If My.Settings.restorePointListColumnOrder IsNot Nothing Then
                Dim columnIndexes As Specialized.StringCollection = My.Settings.restorePointListColumnOrder
                Dim displayIndex, index As Integer

                For displayIndex = 0 To columnIndexes.Count - 1
                    index = Integer.Parse(columnIndexes(displayIndex))
                    systemRestorePointsList.Columns(index).DisplayIndex = displayIndex
                Next
            End If
        Catch ex As Exception
            My.Settings.restorePointListColumnOrder = Nothing
            Functions.eventLogFunctions.writeToSystemEventLog("Error loading saved restore point list column ordering.", EventLogEntryType.Error)
            Functions.eventLogFunctions.writeCrashToEventLog(ex)
        End Try
    End Sub

    Sub deleteAllRestorePointsThread()
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
                                dateTime = Functions.support.parseSystemRestorePointCreationDate(systemRestorePoint("CreationTime"))

                                If toolStripLogRestorePointDeletions.Checked Then
                                    Functions.eventLogFunctions.writeToSystemEventLog(String.Format("Deleted Restore Point named ""{0}"" which was created on {1} at {2}.", systemRestorePoint("Description").ToString, dateTime.ToShortDateString, dateTime.ToLongTimeString), EventLogEntryType.Information)
                                End If

                                dateTime = Nothing
                            End If
                        End If

                        Functions.support.SRRemoveRestorePoint(Integer.Parse(systemRestorePoint("SequenceNumber").ToString))
                    End If

                    systemRestorePoint.Dispose()
                    systemRestorePoint = Nothing
                Next

                While oldNumberOfRestorePoints = systemRestorePoints.Get().Count
                    ' Does nothing, just loops.
                    Threading.Thread.Sleep(500)
                End While

                Functions.wait.closePleaseWaitWindow()

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

        Functions.wait.createPleaseWaitWindow("Deleting Restore Points... Please Wait.")

        deleteThread = New Threading.Thread(Sub()
                                                deleteOldRestorePoints(maxAgeInput)
                                            End Sub)
        deleteThread.Name = "Delete Old Restore Points Thread"
        deleteThread.Priority = Threading.ThreadPriority.Lowest
        deleteThread.Start()

        Functions.wait.openPleaseWaitWindow()
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

    Sub giveFeedbackAfterCreatingRestorePoint(result As Integer)
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

            result = Functions.wmi.createRestorePoint(stringRestorePointName, Functions.support.RestoreType.WindowsType, sequenceNumber)

            If result <> Functions.APIs.errorCodes.ERROR_SUCCESS Then
                MsgBox(String.Format("There was an error while attempting to creating the restore point. The error code returned from the system was {0}{1}{0}.", Chr(34), result), MsgBoxStyle.Critical, strMessageBoxTitle)
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

            Functions.wait.closePleaseWaitWindow()
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

    Sub giveDownloadErrorMessage()
        Functions.wait.closePleaseWaitWindow()
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

        If boolOverrideUserUpdateChannelPreferences = True Then
            updateChannel = globalVariables.updateChannels.stable
        End If

        If updateChannel = globalVariables.updateChannels.stable Then
            If globalVariables.boolExtendedLoggingDuringUpdating = True Then
                Functions.eventLogFunctions.writeToSystemEventLog("Downloading compressed application ZIP package into system RAM.", EventLogEntryType.Information)
            End If

            If Functions.http.downloadFile(globalVariables.webURLs.updateBranch.main.strProgramZIP, memoryStream) = False Then
                memoryStream.Close()
                memoryStream.Dispose()
                memoryStream = Nothing

                giveDownloadErrorMessage()
                Exit Sub
            End If

            If globalVariables.boolExtendedLoggingDuringUpdating = True Then
                Functions.eventLogFunctions.writeToSystemEventLog("Compressed application ZIP package download complete. Now verifying compressed application ZIP package integrity.", EventLogEntryType.Information)
            End If

            If Functions.checksum.verifyChecksum(globalVariables.webURLs.updateBranch.main.strProgramZIPSHA2, memoryStream, True) = False Then
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

            If globalVariables.boolExtendedLoggingDuringUpdating = True Then
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

    Sub disableAutomaticUpdatesAndNotifyUser()
        Dim msgBoxResult As MsgBoxResult = MsgBox("Since you have told the program that you didn't want to update to the newest supported version, do you want to also disable Automatic Update Checking?" & vbCrLf & vbCrLf & "By disabling Automatic Update Checking you will no longer be notified about new versions of this program, that is, unless you manually check for updates." & vbCrLf & vbCrLf & "Do you want to disable Automatic Checking for Updates?", MsgBoxStyle.Question + MsgBoxStyle.YesNo, strMessageBoxTitle)

        If msgBoxResult = MsgBoxResult.Yes Then
            My.Settings.CheckForUpdates = False
            ConfigureAutomaticUpdatesToolStripMenuItem.Visible = False
            toolStripAutomaticallyCheckForUpdates.Checked = False
        End If
    End Sub

    Sub openThePleaseWaitWindowAndStartTheDownloadThread(Optional boolOverrideUserUpdateChannelPreferences As Boolean = False)
        Functions.wait.createPleaseWaitWindow("Downloading update... Please Wait.")

        Dim downloadThread As New Threading.Thread(Sub()
                                                       Try
                                                           downloadAndDoTheUpdate(boolOverrideUserUpdateChannelPreferences)
                                                       Catch ex As Threading.ThreadAbortException
                                                       End Try
                                                   End Sub)
        downloadThread.Name = "Update Download Thread"
        downloadThread.Start()

        Functions.wait.openPleaseWaitWindow(Me)
    End Sub

    Sub giveUserNoticeAboutTotallyNewVersion(ByVal strRemoteBuild As String, ByVal boolDisableAutoUpdateIfUserSaysNo As Boolean)
        ' Example: newversion-1.2
        Dim strRemoteBuildParts As String() = strRemoteBuild.Split("-")

        If openUpdateDialog(Update_Message.versionUpdateType.totallyNewVersionUpdate, strRemoteBuildParts(1).Trim) = Update_Message.userResponse.doTheUpdate Then
            openThePleaseWaitWindowAndStartTheDownloadThread(True)
        Else
            If boolDisableAutoUpdateIfUserSaysNo = True Then disableAutomaticUpdatesAndNotifyUser()
        End If
    End Sub

    Sub userInitiatedCheckForUpdates()
        My.Settings.ProgramExecutionsSinceLastUpdateCheck = 0
        My.Settings.Save()

        If Functions.http.checkForInternetConnection() = False Then
            toolStripCheckForUpdates.Enabled = True
            userInitiatedCheckForUpdatesThread = Nothing
            MsgBox("No Internet connection detected.", MsgBoxStyle.Information, strMessageBoxTitle)
        Else
            Try
                Dim strRemoteBuild As String = Nothing
                Dim shortRemoteBuild As Short, changeLog As String = Nothing

                Dim httpHelper As httpHelper = Functions.http.createNewHTTPHelperObject()
                httpHelper.addPOSTData("version", globalVariables.version.versionStringWithoutBuild)

                If My.Settings.updateChannel = globalVariables.updateChannels.beta Then
                    httpHelper.addPOSTData("program", globalVariables.programCodeNames.beta)
                ElseIf My.Settings.updateChannel = globalVariables.updateChannels.stable Then
                    httpHelper.addPOSTData("program", globalVariables.programCodeNames.main)
                ElseIf My.Settings.updateChannel = globalVariables.updateChannels.tom Then
                    httpHelper.addPOSTData("program", globalVariables.programCodeNames.tom)
                End If

                Try
                    If httpHelper.getWebData(globalVariables.webURLs.core.strProgramUpdateChecker, strRemoteBuild) = False Then
                        MsgBox("There was an error checking for a software update; update check aborted.", MsgBoxStyle.Information, strMessageBoxTitle)
                        userInitiatedCheckForUpdatesThread = Nothing
                        Exit Sub
                    End If
                Catch ex As Exception
                    userInitiatedCheckForUpdatesThread = Nothing
                    Exit Sub
                End Try

                If strRemoteBuild.caseInsensitiveContains("unknown") = True Then
                    MsgBox("There was an error checking for updates." & vbCrLf & vbCrLf & "HTTP Response: " & strRemoteBuild, MsgBoxStyle.Critical, strMessageBoxTitle)
                    userInitiatedCheckForUpdatesThread = Nothing
                    Exit Sub
                ElseIf strRemoteBuild.caseInsensitiveContains("newversion") = True Then
                    ' This handles entirely new versions, not just new builds.
                    giveUserNoticeAboutTotallyNewVersion(strRemoteBuild, False)
                    userInitiatedCheckForUpdatesThread = Nothing
                    Exit Sub
                ElseIf strRemoteBuild.caseInsensitiveContains("beta") = True Or strRemoteBuild.caseInsensitiveContains("rc") = True Then
                    Dim strRemoteBuildParts As String() = strRemoteBuild.Split("-")

                    If Short.TryParse(strRemoteBuildParts(1).Trim, shortRemoteBuild) = True Then
                        If shortRemoteBuild > globalVariables.version.shortBuild Then
                            If strRemoteBuild.caseInsensitiveContains("beta") = True And My.Settings.onlyGiveMeRCs = True Then
                                giveFeedbackToUser("You already have the latest version.")

                                userInitiatedCheckForUpdatesThread = Nothing
                                Exit Sub
                            End If

                            Dim updateDialogResponse As Update_Message.userResponse

                            If strRemoteBuild.caseInsensitiveContains("beta") = True Then
                                updateDialogResponse = openUpdateDialog(Update_Message.versionUpdateType.betaVersionUpdate)
                            ElseIf strRemoteBuild.caseInsensitiveContains("rc") = True Then
                                updateDialogResponse = openUpdateDialog(Update_Message.versionUpdateType.releaseCandidateVersionUpdate)
                            End If

                            If updateDialogResponse = Update_Message.userResponse.doTheUpdate Then
                                openThePleaseWaitWindowAndStartTheDownloadThread()
                            End If

                            userInitiatedCheckForUpdatesThread = Nothing
                            Exit Sub
                        ElseIf shortRemoteBuild < globalVariables.version.shortBuild Then
                            giveFeedbackToUser("Somehow you have a version that is newer than is listed on the product web site, weird.")
                        ElseIf shortRemoteBuild = globalVariables.version.shortBuild Then
                            giveFeedbackToUser("You already have the latest version.")
                        End If
                    Else
                        Functions.eventLogFunctions.writeToSystemEventLog("Error parsing server output. The output that the server gave was """ & strRemoteBuild & """.", EventLogEntryType.Error)

                        userInitiatedCheckForUpdatesThread = Nothing
                        MsgBox("There was an error parsing server output. Please see the Event Log for more details.", MsgBoxStyle.Information, strMessageBoxTitle)
                        Exit Sub
                    End If
                ElseIf strRemoteBuild.caseInsensitiveContains("minor") = True Then
                    Dim strRemoteBuildParts As String() = strRemoteBuild.Split("-")

                    Dim minorBuildApplicables As New Specialized.StringCollection

                    If strRemoteBuildParts(2).Contains(",") Then
                        minorBuildApplicables.AddRange(strRemoteBuildParts(2).ToString.Trim.Split(","))
                    Else
                        minorBuildApplicables.Add(strRemoteBuildParts(2).ToString.Trim)
                    End If

                    If Short.TryParse(strRemoteBuildParts(1).Trim, shortRemoteBuild) = True Then
                        ' If the current build is found in the minorBuildApplicables it means that the new update is a minor update to the current build that the user has installed. If the current build is NOT found then that means that the update is mandatory for the build that the user has installed.
                        If shortRemoteBuild > globalVariables.version.shortBuild And minorBuildApplicables.Contains(globalVariables.version.shortBuild) = True Then
                            If openUpdateDialog(Update_Message.versionUpdateType.minorUpdate) = Update_Message.userResponse.doTheUpdate Then
                                openThePleaseWaitWindowAndStartTheDownloadThread()
                            End If

                            userInitiatedCheckForUpdatesThread = Nothing
                            Exit Sub
                        ElseIf shortRemoteBuild > globalVariables.version.shortBuild And minorBuildApplicables.Contains(globalVariables.version.shortBuild) = False Then
                            If openUpdateDialog(Update_Message.versionUpdateType.standardVersionUpdate) = Update_Message.userResponse.doTheUpdate Then
                                openThePleaseWaitWindowAndStartTheDownloadThread()
                            End If

                            userInitiatedCheckForUpdatesThread = Nothing
                            Exit Sub
                        ElseIf shortRemoteBuild = globalVariables.version.shortBuild Then
                            giveFeedbackToUser("You already have the latest version.")
                        End If
                    Else
                        Functions.eventLogFunctions.writeToSystemEventLog("Error parsing server output. The output that the server gave was """ & strRemoteBuild & """.", EventLogEntryType.Error)

                        userInitiatedCheckForUpdatesThread = Nothing
                        MsgBox("There was an error parsing server output. Please see the Event Log for more details.", MsgBoxStyle.Information, strMessageBoxTitle)
                        Exit Sub
                    End If

                    minorBuildApplicables.Clear()
                    minorBuildApplicables = Nothing
                Else
                    If Short.TryParse(strRemoteBuild, shortRemoteBuild) = True Then
                        If shortRemoteBuild < globalVariables.version.shortBuild Then
                            giveFeedbackToUser("Somehow you have a version that is newer than is listed on the product web site, weird.")
                        ElseIf shortRemoteBuild = globalVariables.version.shortBuild Then
                            giveFeedbackToUser("You already have the latest version.")
                        ElseIf shortRemoteBuild > globalVariables.version.shortBuild Then
                            If openUpdateDialog(Update_Message.versionUpdateType.standardVersionUpdate) = Update_Message.userResponse.doTheUpdate Then
                                openThePleaseWaitWindowAndStartTheDownloadThread()
                            End If

                            userInitiatedCheckForUpdatesThread = Nothing
                            Exit Sub
                        End If
                    Else
                        Functions.eventLogFunctions.writeToSystemEventLog("Error parsing server output. The output that the server gave was """ & strRemoteBuild & """.", EventLogEntryType.Error)

                        userInitiatedCheckForUpdatesThread = Nothing
                        MsgBox("There was an error parsing server output. Please see the Event Log for more details.", MsgBoxStyle.Information, strMessageBoxTitle)
                        Exit Sub
                    End If
                End If
            Catch ex As Exception
                Functions.eventLogFunctions.writeCrashToEventLog(ex)
            Finally
                userInitiatedCheckForUpdatesThread = Nothing
                toolStripCheckForUpdates.Enabled = True
            End Try
        End If

        userInitiatedCheckForUpdatesThread = Nothing
    End Sub

    Private Sub formLoadCheckForUpdatesRoutine(Optional forceRunOfUpdate As Boolean = False)
        If My.Settings.CheckForUpdates = True Or forceRunOfUpdate = True Then
            toolStripAutomaticallyCheckForUpdates.Checked = True
            Dim longDateDiff As Long = Math.Abs(DateDiff(DateInterval.Day, Now, My.Settings.lastUpdateTime))

            If My.Settings.ProgramExecutionsSinceLastUpdateCheck >= 50 Or longDateDiff >= My.Settings.checkForUpdatesEveryInDays Or forceRunOfUpdate = True Then
                My.Settings.ProgramExecutionsSinceLastUpdateCheck = 0
                My.Settings.Save()

                If Functions.http.checkForInternetConnection() = False Then
                    MsgBox("No Internet connection detected.", MsgBoxStyle.Information, strMessageBoxTitle)
                    formLoadCheckForUpdatesRoutineThread = Nothing
                    Exit Sub
                Else
                    My.Settings.lastUpdateTime = Now
                    My.Settings.Save()

                    Try
                        Dim strRemoteBuild As String = Nothing, changeLog As String = Nothing
                        Dim shortRemoteBuild, shortRemoteParts As Short

                        Dim httpHelper As httpHelper = Functions.http.createNewHTTPHelperObject()
                        httpHelper.addPOSTData("version", globalVariables.version.versionStringWithoutBuild)

                        If My.Settings.updateChannel = globalVariables.updateChannels.beta Then
                            httpHelper.addPOSTData("program", globalVariables.programCodeNames.beta)
                        ElseIf My.Settings.updateChannel = globalVariables.updateChannels.stable Then
                            httpHelper.addPOSTData("program", globalVariables.programCodeNames.main)
                        ElseIf My.Settings.updateChannel = globalVariables.updateChannels.tom Then
                            httpHelper.addPOSTData("program", globalVariables.programCodeNames.tom)
                        End If

                        Try
                            If httpHelper.getWebData(globalVariables.webURLs.core.strProgramUpdateChecker, strRemoteBuild) = False Then
                                MsgBox("There was an error checking for a software update; update check aborted. Please see the Event Log for more information regarding this error message.", MsgBoxStyle.Information, strMessageBoxTitle)

                                formLoadCheckForUpdatesRoutineThread = Nothing
                                Exit Sub
                            End If
                        Catch ex As Exception
                            formLoadCheckForUpdatesRoutineThread = Nothing
                            Exit Sub
                        End Try

                        If strRemoteBuild.caseInsensitiveContains("unknown") = True Then
                            MsgBox("There was an error checking for updates." & vbCrLf & vbCrLf & "HTTP Response: " & strRemoteBuild, MsgBoxStyle.Critical, strMessageBoxTitle)

                            formLoadCheckForUpdatesRoutineThread = Nothing
                            Exit Sub
                        ElseIf strRemoteBuild.caseInsensitiveContains("newversion") = True Then
                            ' This handles entirely new versions, not just new builds.
                            giveUserNoticeAboutTotallyNewVersion(strRemoteBuild, True)

                            formLoadCheckForUpdatesRoutineThread = Nothing
                            Exit Sub
                        ElseIf strRemoteBuild.caseInsensitiveContains("beta") = True Or strRemoteBuild.caseInsensitiveContains("rc") = True Then
                            Dim strRemoteBuildParts As String() = strRemoteBuild.Split("-")

                            If Short.TryParse(strRemoteBuildParts(1).Trim, shortRemoteParts) Then
                                If shortRemoteParts > globalVariables.version.shortBuild Then
                                    If strRemoteBuild.caseInsensitiveContains("beta") = True And My.Settings.onlyGiveMeRCs = True Then
                                        My.Settings.ProgramExecutionsSinceLastUpdateCheck += 1
                                        My.Settings.Save()

                                        formLoadCheckForUpdatesRoutineThread = Nothing
                                        Exit Sub
                                    End If

                                    Dim updateDialogResponse As Update_Message.userResponse

                                    If strRemoteBuild.caseInsensitiveContains("beta") = True Then
                                        updateDialogResponse = openUpdateDialog(Update_Message.versionUpdateType.betaVersionUpdate)
                                    ElseIf strRemoteBuild.caseInsensitiveContains("rc") = True Then
                                        updateDialogResponse = openUpdateDialog(Update_Message.versionUpdateType.releaseCandidateVersionUpdate)
                                    End If

                                    If updateDialogResponse = Update_Message.userResponse.doTheUpdate Then
                                        openThePleaseWaitWindowAndStartTheDownloadThread()
                                    Else
                                        disableAutomaticUpdatesAndNotifyUser()
                                    End If

                                    formLoadCheckForUpdatesRoutineThread.Abort()
                                    Exit Sub
                                End If
                            Else
                                Functions.eventLogFunctions.writeToSystemEventLog("Error parsing server output. The output that the server gave was """ & strRemoteBuild & """.", EventLogEntryType.Error)

                                userInitiatedCheckForUpdatesThread = Nothing
                                MsgBox("There was an error parsing server output. Please see the Event Log for more details.", MsgBoxStyle.Information, strMessageBoxTitle)
                                Exit Sub
                            End If
                        ElseIf strRemoteBuild.caseInsensitiveContains("minor") = True Then
                            Dim strRemoteBuildParts As String() = strRemoteBuild.Split("-")

                            Dim minorBuildApplicables As New Specialized.StringCollection

                            If strRemoteBuildParts(2).Contains(",") Then
                                minorBuildApplicables.AddRange(strRemoteBuildParts(2).ToString.Trim.Split(","))
                            Else
                                minorBuildApplicables.Add(strRemoteBuildParts(2).ToString.Trim)
                            End If

                            If Short.TryParse(strRemoteBuildParts(1).Trim, shortRemoteParts) Then
                                ' If the current build is found in the minorBuildApplicables it means that the new update is a minor update to the current build that the user has installed. If the current build is NOT found then that means that the update is mandatory for the build that the user has installed.
                                If shortRemoteParts > globalVariables.version.shortBuild And minorBuildApplicables.Contains(globalVariables.version.shortBuild) = False Then
                                    If openUpdateDialog(Update_Message.versionUpdateType.standardVersionUpdate) = Update_Message.userResponse.doTheUpdate Then
                                        openThePleaseWaitWindowAndStartTheDownloadThread()
                                    End If

                                    formLoadCheckForUpdatesRoutineThread = Nothing
                                    Exit Sub
                                End If
                            Else
                                Functions.eventLogFunctions.writeToSystemEventLog("Error parsing server output. The output that the server gave was """ & strRemoteBuild & """.", EventLogEntryType.Error)

                                userInitiatedCheckForUpdatesThread = Nothing
                                MsgBox("There was an error parsing server output. Please see the Event Log for more details.", MsgBoxStyle.Information, strMessageBoxTitle)
                                Exit Sub
                            End If

                            minorBuildApplicables.Clear()
                            minorBuildApplicables = Nothing
                        Else
                            If Short.TryParse(strRemoteBuild, shortRemoteBuild) = True Then
                                If shortRemoteBuild > globalVariables.version.shortBuild Then
                                    If openUpdateDialog(Update_Message.versionUpdateType.standardVersionUpdate) = Update_Message.userResponse.doTheUpdate Then
                                        openThePleaseWaitWindowAndStartTheDownloadThread()
                                    Else
                                        disableAutomaticUpdatesAndNotifyUser()
                                    End If

                                    formLoadCheckForUpdatesRoutineThread = Nothing
                                    Exit Sub
                                End If
                            Else
                                Functions.eventLogFunctions.writeToSystemEventLog("Error parsing server output. The output that the server gave was """ & strRemoteBuild & """.", EventLogEntryType.Error)

                                userInitiatedCheckForUpdatesThread = Nothing
                                MsgBox("There was an error parsing server output. Please see the Event Log for more details.", MsgBoxStyle.Information, strMessageBoxTitle)
                                Exit Sub
                            End If
                        End If
                    Catch ex As Exception
                        Functions.eventLogFunctions.writeCrashToEventLog(ex)
                    End Try
                End If
            Else
                My.Settings.ProgramExecutionsSinceLastUpdateCheck += 1
                My.Settings.Save()
            End If
        Else
            toolStripAutomaticallyCheckForUpdates.Checked = False
        End If

        formLoadCheckForUpdatesRoutineThread = Nothing
    End Sub

    Private Function calculateRestorePointAge(creationDate As Date) As Double
        Return Math.Round(Math.Abs(Now.Subtract(creationDate).TotalDays), My.Settings.roundRestorePointAgeNumber)
    End Function

    Private Sub loadRestorePointsFromSystemIntoList() 'Adds all Restore Points to a ListView
        ' Declares some variables.
        Dim systemRestoreIDs As New ArrayList ' Creates an ArrayList for us to put our System Restore IDs into for later checking for the newest System Restore Point ID.
        Dim systemRestorePointsManagementObjectSearcher As ManagementObjectSearcher
        Dim listViewItem As ListViewItem
        Dim listOfRestorePoints As New List(Of ListViewItem)
        Dim restorePointCreationDate As Date
        Dim restorePointAge As Double

        ' We need to check if we have
        If restorePointDateData.Count() <> 0 Then
            restorePointDateData.Clear()
        End If

        Try
            btnRefreshRestorePoints.Text = "Abort Refreshing System Restore Points"

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
                                listViewItem = New ListViewItem(restorePointDetails("SequenceNumber").ToString)

                                If restorePointDateData.Keys.Contains(restorePointDetails("SequenceNumber")) = False Then
                                    restorePointDateData.Add(restorePointDetails("SequenceNumber"), restorePointDetails("CreationTime"))
                                    'debug.writeline("Added restore point id " & systemRestorePoint("SequenceNumber").ToString)
                                End If

                                ' Adds the System Restore Point ID to our list of System Restore Point IDs to calculate the newest System Restore Point.
                                systemRestoreIDs.Add(Integer.Parse(restorePointDetails("SequenceNumber")))

                                listViewItem.SubItems.Add(restorePointDetails("Description").ToString)

                                If String.IsNullOrEmpty(restorePointDetails("CreationTime").ToString.Trim) = False Then
                                    restorePointCreationDate = Functions.support.parseSystemRestorePointCreationDate(restorePointDetails("CreationTime"))
                                    listViewItem.SubItems.Add(String.Format("{0} {1}", restorePointCreationDate.ToShortDateString, restorePointCreationDate.ToLongTimeString))
                                    restorePointAge = calculateRestorePointAge(restorePointCreationDate)
                                    restorePointCreationDate = Nothing
                                Else
                                    restorePointAge = 0
                                    listViewItem.SubItems.Add("(Error Parsing Date)")
                                End If

                                If restorePointDetails("Description").ToString.ToLower.Contains("windows update") Then
                                    If My.Settings.debug = True Then
                                        listViewItem.SubItems.Add("Windows Update" & " (" & restorePointDetails("RestorePointType").ToString & ")")
                                    Else
                                        listViewItem.SubItems.Add("Windows Update")
                                    End If
                                ElseIf restorePointDetails("Description").ToString.ToLower.Contains("system checkpoint") Then
                                    If My.Settings.debug = True Then
                                        listViewItem.SubItems.Add("System Checkpoint" & " (" & restorePointDetails("RestorePointType").ToString & ")")
                                    Else
                                        listViewItem.SubItems.Add("System Checkpoint")
                                    End If
                                Else
                                    If My.Settings.debug = True Then
                                        listViewItem.SubItems.Add(Functions.support.whatTypeOfRestorePointIsIt(Integer.Parse(restorePointDetails("RestorePointType").ToString)) & " (" & restorePointDetails("RestorePointType").ToString & ")")
                                    Else
                                        listViewItem.SubItems.Add(Functions.support.whatTypeOfRestorePointIsIt(Integer.Parse(restorePointDetails("RestorePointType").ToString)))
                                        'MsgBox(systemRestorePoint("Description") & " -- " & systemRestorePoint("EventType"))
                                    End If
                                End If

                                listViewItem.SubItems.Add(restorePointAge.ToString)

                                ' Adds the item to the list.
                                listOfRestorePoints.Add(listViewItem)
                                listViewItem = Nothing
                            End If
                        Next

                        systemRestorePointsManagementObjectSearcher.Dispose()
                        systemRestorePointsManagementObjectSearcher = Nothing

                        ' Adds the list of System Restore Points that we created earlier in this routine to the System Restore Points list on the GUI.
                        systemRestorePointsList.Items.AddRange(listOfRestorePoints.ToArray())

                        ' Does some sorting on the System Restore Points list on the GUI.
                        systemRestorePointsList.Sort()

                        If systemRestoreIDs.Count <> 0 Then
                            ' First, we convert the ArrayList into an Integer then calculate the Max value of all of the Integers in the Integer Array.
                            ' This gets the latest System Restore Point ID for later checking to see if the user is deleting the newest System Restore Point.
                            newestSystemRestoreID = DirectCast(systemRestoreIDs.ToArray(GetType(Integer)), Integer()).Max
                        End If

                        For i = 0 To systemRestorePointsList.Items.Count - 1
                            If Integer.Parse(systemRestorePointsList.Items.Item(i).SubItems(0).Text) = newestSystemRestoreID Then
                                systemRestorePointsList.Items.Item(i).Font = New Font(btnCreate.Font.FontFamily, btnCreate.Font.SizeInPoints, FontStyle.Bold)
                            End If
                        Next
                    Else
                        newestSystemRestoreID = 0
                    End If

                    restorePointsOnSystemManagementObjectCollection.Dispose()
                    restorePointsOnSystemManagementObjectCollection = Nothing
                Else
                    newestSystemRestoreID = 0
                End If
            End If
        Catch ex6 As ObjectDisposedException
            Functions.eventLogFunctions.writeCrashToEventLog(ex6)

            Functions.wait.closePleaseWaitWindow()

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
            Functions.wait.closePleaseWaitWindow()

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

    Sub restoreSystemRestorePoint()
        Try
            disableFormElements()

            Dim systemRestorePointIndex As Integer = Integer.Parse(systemRestorePointsList.SelectedItems(0).SubItems(enums.restorePointListSubItems.restorePointID).Text)
            'systemRestorePointClass = New SystemRestorePointCreator.Classes.SystemRestore
            Functions.wmi.restoreToSystemRestorePoint(systemRestorePointIndex)

            Functions.wait.closePleaseWaitWindow()
        Catch ex2 As Threading.ThreadAbortException
            ' Does nothing.
        Catch ex As Exception
            MsgBox("Unable to restore system back to user defined System Restore Point." & vbCrLf & vbCrLf & ex.Message, MsgBoxStyle.Critical, "Critical Error")
        Finally
            enableFormElements()
        End Try
    End Sub

    Private Sub deleteSystemRestorePoint()
        Try
            ' Now we declare some variables.
            'Dim systemRestore As New SystemRestorePointCreator.Classes.SystemRestore ' Creates an instance of the SystemRestore class.
            Dim dateData As Date
            Dim deletionConfirmationWindow As frmConfirmDelete
            Dim boolUserWantsToDeleteTheRestorePoint As Boolean
            Dim shortNumberOfRestorePointsDeleted As Short = 0
            Dim intNumberOfRestorePoints As Integer = Functions.wmi.getNumberOfRestorePoints()

            systemRestorePointsList.Enabled = False
            btnCreateSystemCheckpoint.Enabled = False
            txtRestorePointDescription.Enabled = False
            btnCreate.Enabled = False
            toolStripAutomaticallyCheckForUpdates.Enabled = False
            toolStripConfirmDeletions.Enabled = False
            toolStripCloseAfterRestorePointIsCreated.Enabled = False
            btnRefreshRestorePoints.Enabled = False
            btnDeleteRestorePoint.Enabled = False

            stripRestore.Enabled = False
            stripRestoreSafeMode.Enabled = False
            btnRestoreToRestorePoint.Enabled = False
            btnRestoreToRestorePointSafeMode.Enabled = False

            toolStripAbout.Enabled = False
            toolStripCheckForUpdates.Enabled = False

            Dim boolMultipleRestorePointsDeleted As Boolean = False
            If systemRestorePointsList.SelectedItems.Count > 1 Then
                boolMultipleRestorePointsDeleted = True
            End If

            ' Get all System Restore Points from the Windows Management System and puts then in the systemRestorePoints variable.
            Dim systemRestorePoints As New ManagementObjectSearcher("root\DEFAULT", "SELECT * FROM SystemRestore")
            Dim oldNumberOfRestorePoints As Integer = systemRestorePoints.Get().Count

            If systemRestorePointsList.SelectedItems.Count <> 0 Then
                For Each item As ListViewItem In systemRestorePointsList.SelectedItems
                    If AllowForDeletionOfAllSystemRestorePointsToolStripMenuItem.Checked = False Then
                        ' Checks to see if the user is trying to delete the newest System Restore Point based upon ID.
                        If Integer.Parse(item.SubItems(enums.restorePointListSubItems.restorePointID).Text) = newestSystemRestoreID Then
                            ' Yep, the user is trying to do that.  Stupid user, now we give that stupid user a message to prevent his/her stupidity.
                            MsgBox("You can't delete the most recent System Restore Point.  This is for your own protection.", MsgBoxStyle.Information, strMessageBoxTitle)
                            Exit Sub
                        End If
                    End If

                    boolUserWantsToDeleteTheRestorePoint = False

                    ' Checks to see if the user wants to confirm deletions.
                    If toolStripConfirmDeletions.Checked Then
                        If globalVariables.windows.frmPleaseWait IsNot Nothing Then
                            globalVariables.windows.frmPleaseWait.TopMost = False
                            Functions.wait.enableFocusingOnPleaseWaitWindow()
                        End If

                        ' Yep, so ask the user.
                        deletionConfirmationWindow = New frmConfirmDelete
                        deletionConfirmationWindow.lblCreatedOn.Text &= " " & item.SubItems(enums.restorePointListSubItems.restorePointDate).Text
                        deletionConfirmationWindow.lblRestorePointName.Text &= " " & item.SubItems(enums.restorePointListSubItems.restorePointName).Text
                        deletionConfirmationWindow.StartPosition = FormStartPosition.CenterParent
                        deletionConfirmationWindow.ShowDialog(Me)

                        If deletionConfirmationWindow.userResponse = frmConfirmDelete.userResponseENum.yes Then
                            boolUserWantsToDeleteTheRestorePoint = True
                        Else
                            boolUserWantsToDeleteTheRestorePoint = False
                        End If

                        deletionConfirmationWindow.Dispose()
                        deletionConfirmationWindow = Nothing

                        If globalVariables.windows.frmPleaseWait IsNot Nothing Then
                            globalVariables.windows.frmPleaseWait.TopMost = True
                            Functions.wait.enableFocusingOnPleaseWaitWindow()
                        End If
                    Else
                        ' No, so we give the variable a value of Yes without asking the user.
                        boolUserWantsToDeleteTheRestorePoint = True
                    End If

                    If boolUserWantsToDeleteTheRestorePoint = True Then
                        shortNumberOfRestorePointsDeleted += 1

                        If toolStripLogRestorePointDeletions.Checked Then
                            If String.IsNullOrEmpty(restorePointDateData(item.SubItems(0).Text).Trim) = False And toolStripLogRestorePointDeletions.Checked Then
                                dateData = Functions.support.parseSystemRestorePointCreationDate(restorePointDateData(item.SubItems(0).Text))
                                Functions.eventLogFunctions.writeToSystemEventLog(String.Format("The user {3}/{4} deleted the restore point named ""{0}"" which was created on {1} at {2}.", item.SubItems(1).Text, dateData.ToShortDateString, dateData.ToShortTimeString, Environment.MachineName, Environment.UserName), EventLogEntryType.Information)
                                dateData = Nothing
                            End If
                        End If

                        intNumberOfRestorePoints -= 1
                        Functions.support.SRRemoveRestorePoint(Integer.Parse(item.SubItems(0).Text)) ' Deletes the Restore Point.

                        While intNumberOfRestorePoints <> Functions.wmi.getNumberOfRestorePoints()
                            Threading.Thread.Sleep(500)
                        End While

                        systemRestorePointsList.Items.Remove(item)
                    End If
                Next
            End If

            Functions.wait.closePleaseWaitWindow()

            If ShowMessageBoxAfterSuccessfulDeletionOfRestorePointsToolStripMenuItem.Checked = True Then
                If shortNumberOfRestorePointsDeleted = 0 Then
                    If globalVariables.windows.frmPleaseWait IsNot Nothing Then globalVariables.windows.frmPleaseWait.TopMost = False
                    giveFeedbackToUser("No System Restore Points were deleted.")
                    If globalVariables.windows.frmPleaseWait IsNot Nothing Then globalVariables.windows.frmPleaseWait.TopMost = True
                ElseIf shortNumberOfRestorePointsDeleted > 0 Then
                    If globalVariables.windows.frmPleaseWait IsNot Nothing Then globalVariables.windows.frmPleaseWait.TopMost = False

                    ' Gives some feedback.
                    If boolMultipleRestorePointsDeleted = True Then
                        giveFeedbackToUser(shortNumberOfRestorePointsDeleted.ToString & " System Restore Points were deleted.")
                    Else
                        giveFeedbackToUser("One System Restore Point was deleted.")
                    End If

                    If globalVariables.windows.frmPleaseWait IsNot Nothing Then globalVariables.windows.frmPleaseWait.TopMost = True
                End If
            End If
        Catch ex As Threading.ThreadAbortException
            MsgBox("System Restore Point Deletion Process Aborted.", MsgBoxStyle.Information, strMessageBoxTitle)
        Catch ex3 As ArgumentOutOfRangeException
            Functions.eventLogFunctions.writeCrashToEventLog(ex3)
        Catch ex2 As Exception
            Threading.Thread.CurrentThread.CurrentUICulture = New Globalization.CultureInfo("en-US")
            exceptionHandler.manuallyLoadCrashWindow(ex2, ex2.Message, ex2.StackTrace, ex2.GetType)
        Finally
            systemRestorePointsList.Enabled = True

            If Functions.support.areWeInSafeMode() = False Then
                btnCreateSystemCheckpoint.Enabled = True
                btnCreate.Enabled = True
                txtRestorePointDescription.Enabled = True
            End If

            toolStripAutomaticallyCheckForUpdates.Enabled = True
            toolStripConfirmDeletions.Enabled = True
            toolStripCloseAfterRestorePointIsCreated.Enabled = True
            btnRefreshRestorePoints.Enabled = True
            btnDeleteRestorePoint.Enabled = True

            If Functions.support.areWeInSafeMode = False Then
                stripRestoreSafeMode.Enabled = True
                btnRestoreToRestorePointSafeMode.Enabled = True
            End If

            btnRestoreToRestorePoint.Enabled = True
            stripRestore.Enabled = True

            btnRestoreToRestorePointSafeMode.Enabled = True
            stripRestoreSafeMode.Enabled = True

            toolStripAbout.Enabled = True
            toolStripCheckForUpdates.Enabled = True

            loadRestorePointsFromSystemIntoList() ' Calls our central sub-routine to update the List of System Restore Points list on the GUI.

            Functions.wait.closePleaseWaitWindow()

            Threading.Thread.Sleep(4000)
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
                Functions.wait.closePleaseWaitWindow()
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
                    systemRestorePointCreationDate = Functions.support.parseSystemRestorePointCreationDate(systemRestorePoint("CreationTime"), True)

                    dateDiffResults = Math.Abs(DateDiff(DateInterval.Day, systemRestorePointCreationDate, Date.Now))

                    If dateDiffResults >= maxAgeInput Then
                        Functions.support.SRRemoveRestorePoint(Integer.Parse(systemRestorePoint("SequenceNumber").ToString)) ' Deletes the Restore Point.

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
            Functions.wait.closePleaseWaitWindow()
        End Try
    End Sub
#End Region

#Region "--== Misc. Event Code ==--"
    Private Sub txtRestorePointDescription_Click(sender As Object, e As EventArgs) Handles txtRestorePointDescription.Click
        txtRestorePointDescription.ForeColor = Color.Black
        txtRestorePointDescription.Text = ""
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

    Private Sub txtRestorePointDescription_Leave(sender As Object, e As EventArgs) Handles txtRestorePointDescription.Leave
        doTheGrayingOfTheRestorePointNameTextBox()
    End Sub

    Private Sub txtRestorePointDescription_LostFocus(sender As Object, e As EventArgs) Handles txtRestorePointDescription.LostFocus
        doTheGrayingOfTheRestorePointNameTextBox()
    End Sub

    Private Sub txtRestorePointDescription_TextChanged(sender As Object, e As EventArgs) Handles txtRestorePointDescription.TextChanged
        If txtRestorePointDescription.Text.caseInsensitiveContains(strTypeYourRestorePointName) = False And txtRestorePointDescription.Text.Trim <> Nothing And Functions.support.areWeInSafeMode() = False Then
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
            For i = 0 To systemRestorePointsList.SelectedItems.Count - 1
                If Integer.Parse(systemRestorePointsList.SelectedItems(i).SubItems(enums.restorePointListSubItems.restorePointID).Text) = newestSystemRestoreID Then
                    btnDeleteRestorePoint.Enabled = False
                    stripDelete.Enabled = False
                    ToolTip.SetToolTip(btnDeleteRestorePoint, "Disabled because you have the latest System Restore Point selected as part of the group of selected System Restore Points.")
                    Exit Sub
                End If
            Next
        End If
    End Sub

    Private Sub Form1_FormClosing(sender As Object, e As FormClosingEventArgs) Handles Me.FormClosing
        killThread(formLoadCheckForUpdatesRoutineThread)
        killThread(userInitiatedCheckForUpdatesThread)

        My.Settings.mainWindowPosition = Me.Location
        saveRestorePointListColumnOrder()

        If updateRestorePointListThread IsNot Nothing Then
            updateRestorePointListThread.Abort()
        End If

        If globalVariables.boolLogLoadsAndExitsToEventLog = True Then Functions.eventLogFunctions.writeToSystemEventLog("The user " & Environment.UserName & " closed the program.", EventLogEntryType.Information)
    End Sub

    Private Sub Form1_ResizeEnd(sender As Object, e As EventArgs) Handles Me.ResizeEnd
        My.Settings.windowSize = Me.Size
        My.Settings.Save()
    End Sub

    Private Sub Form1_KeyUp(sender As Object, e As KeyEventArgs) Handles Me.KeyUp
        If e.KeyCode = Keys.F5 Then
            Functions.wait.createPleaseWaitWindow("Loading Restore Points... Please Wait.")

            updateRestorePointListThread = New Threading.Thread(AddressOf loadRestorePointsFromSystemIntoList)
            updateRestorePointListThread.Name = "System Restore Point List Updating Thread"
            updateRestorePointListThread.Priority = Threading.ThreadPriority.Normal
            updateRestorePointListThread.Start()

            Functions.wait.openPleaseWaitWindow()
        End If
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
        Functions.wait.createPleaseWaitWindow("Loading Restore Points... Please Wait.")

        startSystemRestorePointListLoadThread = New Threading.Thread(AddressOf startSystemRestorePointListLoadThreadSub)
        startSystemRestorePointListLoadThread.Name = "Start System Restore Point List Load Thread Thread"
        startSystemRestorePointListLoadThread.Start()

        Functions.wait.openPleaseWaitWindow(Me)
    End Sub

    Private Sub Form1_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        Control.CheckForIllegalCrossThreadCalls = False

        If IO.File.Exists("tom") Then
            ToolStripMenuItemPrivateForTom.Visible = True
        End If

        Try
            If globalVariables.boolLogLoadsAndExitsToEventLog = True Then Functions.eventLogFunctions.writeToSystemEventLog("The user " & Environment.UserName & " started the program.", EventLogEntryType.Information)

            If IO.File.Exists("updater.exe") = True Then
                Dim updaterDeleterThread As New Threading.Thread(AddressOf updaterDeleterThreadSub)
                updaterDeleterThread.Name = "Legacy Updater File Deletion Thread"
                updaterDeleterThread.Start()
            End If

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

            addShortCutForEventLogToUsersStartMenu()
            loadPreferences()
            checkForMyComputerRightClickOption()
            checkScheduledTaskEXEPaths()
            showDonationNotice()
            loadRestorePointListColumnOrder()
            applySavedSorting()

            If IO.File.Exists(Application.ExecutablePath & ".new.exe") = True Then
                Dim newFileDeleterThread As New Threading.Thread(AddressOf newFileDeleterThreadSub)
                newFileDeleterThread.Name = "New Application File Deletion Thread"
                newFileDeleterThread.Start()
            End If

            If Functions.support.areWeInSafeMode() = True Then
                toolStripScheduleRestorePoints.Enabled = False
                btnRestoreToRestorePointSafeMode.Enabled = False
                stripRestoreSafeMode.Enabled = False
                RemoveSafeModeBootOptionToolStripMenuItem.Visible = True
            Else
                RemoveSafeModeBootOptionToolStripMenuItem.Visible = False
            End If

            If My.Settings.checkSystemDrivesForFullShadowStorage = True Then
                checkRestorePointSpaceThread = New Threading.Thread(AddressOf checkRestorePointSpaceThreadSub)
                checkRestorePointSpaceThread.Name = "Restore Point Storage Status Checking Thread"
                checkRestorePointSpaceThread.Priority = Threading.ThreadPriority.Lowest
                checkRestorePointSpaceThread.Start()
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

            checkForAndEnableSystemRestoreIfNeeded = New Threading.Thread(AddressOf checkForAndEnableSystemRestoreIfNeededSub)
            checkForAndEnableSystemRestoreIfNeeded.Name = "Check For and Enable System Restore if Needed Thread"
            checkForAndEnableSystemRestoreIfNeeded.Start()
            checkForAndEnableSystemRestoreIfNeededThreadKiller.Enabled = True

            boolDoneLoading = True
            systemRestorePointsList.Select()

            My.Settings.boolFirstRun = False
        Catch ex As Exception
            Threading.Thread.CurrentThread.CurrentUICulture = New Globalization.CultureInfo("en-US")
            exceptionHandler.manuallyLoadCrashWindow(ex, "Main Form Load" & vbCrLf & vbCrLf & ex.Message, ex.StackTrace, ex.GetType)
        End Try
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

            Functions.taskStuff.addRunTimeTask("Restore Point Creator -- Run with no UAC (Create Restore Point)", "Runs Restore Point Creator with no UAC prompt.", Application.ExecutablePath, "-createrestorepoint")
        End If

        If Functions.taskStuff.doesRunTimeTaskExist("Restore Point Creator -- Run with no UAC (Create Custom Restore Point)", task) = True Then
            Functions.taskStuff.deleteTask(task)
            task.Dispose()

            Functions.taskStuff.addRunTimeTask("Restore Point Creator -- Run with no UAC (Create Custom Restore Point)", "Runs Restore Point Creator with no UAC prompt.", Application.ExecutablePath, "-createrestorepointcustomname")
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
        exportBackupDialog.Filter = "Config Backup File|*.resbak|INI File|*.ini"
        exportBackupDialog.FileName = Nothing

        If exportBackupDialog.ShowDialog() = DialogResult.OK Then
            If IO.File.Exists(exportBackupDialog.FileName) Then
                IO.File.Delete(exportBackupDialog.FileName)
            End If

            Dim iniFile As New IniFile
            Dim settingName, settingType As String, settingValue As Object
            Dim stringCollection As Specialized.StringCollection
            Dim point As Point

            For Each settingProperty As Object In My.Settings.PropertyValues
                If settingProperty.propertyvalue IsNot Nothing Then
                    settingName = settingProperty.name
                    settingType = settingProperty.propertyvalue.GetType.ToString

                    If settingType.stringCompare("System.Drawing.Color") Then
                        settingValue = DirectCast(settingProperty.propertyvalue, Color).ToArgb
                    Else
                        settingValue = settingProperty.propertyvalue.ToString
                    End If

                    If settingType.stringCompare("System.Int16") Or settingType.stringCompare("System.Int32") Or settingType.stringCompare("System.Boolean") Or settingType.stringCompare("System.String") Or settingType.stringCompare("System.Drawing.Size") Or settingType.stringCompare("System.Drawing.Color") Then
                        If settingType.stringCompare("System.String") Then
                            If String.IsNullOrEmpty(DirectCast(settingValue, String).Trim) = False Then
                                iniFile.SetKeyValue("Settings", settingName, settingType & "," & settingValue)
                            End If
                        Else
                            iniFile.SetKeyValue("Settings", settingName, settingType & "," & settingValue)
                        End If
                    ElseIf settingType.stringCompare("System.Collections.Specialized.StringCollection") Then
                        stringCollection = DirectCast(settingProperty.propertyvalue, Specialized.StringCollection)

                        Dim tempArray(stringCollection.Count - 1) As String
                        stringCollection.CopyTo(tempArray, 0)
                        iniFile.SetKeyValue("Settings", settingName, settingType & "," & (New Web.Script.Serialization.JavaScriptSerializer).Serialize(tempArray))
                    ElseIf settingType.stringCompare("System.Drawing.Point") Then
                        point = DirectCast(settingProperty.propertyvalue, Point)
                        iniFile.SetKeyValue("Settings", settingName, settingType & "," & point.X & "|" & point.Y)
                    End If
                End If
            Next

            settingName = Nothing
            settingType = Nothing
            settingValue = Nothing

            Dim registryKeyWeAreWorkingWith As RegistryKey = Registry.LocalMachine.OpenSubKey(globalVariables.registryValues.strKey, False)
            Dim registryValue As String

            For Each registryValueName As String In registryKeyWeAreWorkingWith.GetValueNames
                registryValue = registryKeyWeAreWorkingWith.GetValue(registryValueName, Nothing)
                If registryValue IsNot Nothing Then iniFile.SetKeyValue("Registry", registryValueName.Replace(" ", "_"), registryValue)
            Next

            registryKeyWeAreWorkingWith.Close()
            registryKeyWeAreWorkingWith.Dispose()

            Dim fileInfo As New IO.FileInfo(exportBackupDialog.FileName)
            Debug.WriteLine(fileInfo.Extension)

            If fileInfo.Extension.stringCompare(".ini") Then
                iniFile.Save(exportBackupDialog.FileName)
            Else
                Dim iniFileTextBase64ed As String = Functions.support.convertToBase64(iniFile.getINIText())
                Dim randomString As String = Functions.support.randomStringGenerator((New Random).Next(100, 300))
                Dim checksum As String = calculateConfigBackupDataPayloadChecksum(iniFileTextBase64ed, randomString)

                Dim streamWriter As New IO.StreamWriter(exportBackupDialog.FileName)
                streamWriter.WriteLine("Payload: " & iniFileTextBase64ed)
                streamWriter.WriteLine("Random String: " & randomString)
                streamWriter.Write("Checksum: " & checksum)

                streamWriter.Close()
                streamWriter.Dispose()
            End If

            iniFile = Nothing

            MsgBox("Backup complete.", MsgBoxStyle.Information, strMessageBoxTitle)
        End If
    End Sub

    Private Sub RestoreToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles RestoreToolStripMenuItem.Click
        importBackupDialog.Title = "Locate your backup file..."
        importBackupDialog.Filter = "Config Backup File|*.resbak|INI File|*.ini"
        importBackupDialog.FileName = Nothing

        If importBackupDialog.ShowDialog() = DialogResult.OK Then
            If IO.File.Exists(importBackupDialog.FileName) = True Then
                Dim fileInfo As New IO.FileInfo(importBackupDialog.FileName)
                Dim iniFile As New IniFile

                If fileInfo.Extension.stringCompare(".ini") Then
                    iniFile.loadINIFileFromFile(importBackupDialog.FileName)
                Else
                    Dim streamReader As New IO.StreamReader(importBackupDialog.FileName)
                    Dim strDataPayload = Nothing, strRandomString = Nothing, strChecksum = Nothing, strTemp As String

                    While streamReader.EndOfStream = False
                        strTemp = streamReader.ReadLine

                        If strTemp.StartsWith("Payload: ", StringComparison.OrdinalIgnoreCase) = True Then
                            strDataPayload = strTemp.caseInsensitiveReplace("Payload: ", "").Trim
                        ElseIf strTemp.StartsWith("Random String: ", StringComparison.OrdinalIgnoreCase) = True Then
                            strRandomString = strTemp.caseInsensitiveReplace("Random String: ", "").Trim
                        ElseIf strTemp.StartsWith("Checksum: ", StringComparison.OrdinalIgnoreCase) = True Then
                            strChecksum = strTemp.caseInsensitiveReplace("Checksum: ", "").Trim
                        End If
                    End While

                    streamReader.Close()
                    streamReader.Dispose()

                    If calculateConfigBackupDataPayloadChecksum(strDataPayload, strRandomString) = strChecksum Then
                        iniFile.loadINIFileFromText(Functions.support.convertFromBase64(strDataPayload))
                    Else
                        MsgBox("Invalid backup file.", MsgBoxStyle.Critical, strMessageBoxTitle)
                        Exit Sub
                    End If
                End If

                Dim iniFileValue, iniFileKeyName As String
                Dim tempShort As Short, tempInteger As Integer, tempBoolean As Boolean
                Dim tempWidth, tempHeight As Integer
                Dim regExMatches As Match

                Dim systemDrawingSizeRegexObject As New Regex("\{Width=(?<width>[0-9]{1,4}), Height=(?<height>[0-9]{1,4})\}", RegexOptions.Compiled)

                For Each valueObject As Object In iniFile.GetSection("Settings").Keys
                    Try
                        iniFileKeyName = valueObject.name
                        iniFileValue = valueObject.value

                        If iniFileValue.StartsWith("System.Int16") Then
                            iniFileValue = iniFileValue.Replace("System.Int16,", "")

                            If Short.TryParse(iniFileValue, tempShort) Then
                                My.Settings(iniFileKeyName) = tempShort
                            End If
                        ElseIf iniFileValue.StartsWith("System.Int32") Then
                            iniFileValue = iniFileValue.Replace("System.Int32,", "")

                            If Integer.TryParse(iniFileValue, tempInteger) Then
                                My.Settings(iniFileKeyName) = tempInteger
                            End If
                        ElseIf iniFileValue.StartsWith("System.Boolean") Then
                            iniFileValue = iniFileValue.Replace("System.Boolean,", "")

                            If Boolean.TryParse(iniFileValue, tempBoolean) Then
                                My.Settings(iniFileKeyName) = tempBoolean
                            End If
                        ElseIf iniFileValue.StartsWith("System.String") Then
                            iniFileValue = iniFileValue.Replace("System.String,", "")
                            My.Settings(iniFileKeyName) = iniFileValue
                        ElseIf iniFileValue.StartsWith("System.Drawing.Size") Then
                            iniFileValue = iniFileValue.Replace("System.Drawing.Size,", "")

                            If systemDrawingSizeRegexObject.IsMatch(iniFileValue) = True Then
                                regExMatches = systemDrawingSizeRegexObject.Match(iniFileValue)

                                tempWidth = Integer.Parse(regExMatches.Groups("width").Value)
                                tempHeight = Integer.Parse(regExMatches.Groups("height").Value)

                                regExMatches = Nothing
                                My.Settings(iniFileKeyName) = New Size(tempWidth, tempHeight)
                            End If
                        ElseIf iniFileValue.StartsWith("System.Drawing.Color") Then
                            iniFileValue = iniFileValue.Replace("System.Drawing.Color,", "")

                            If Integer.TryParse(iniFileValue, tempInteger) Then
                                My.Settings(iniFileKeyName) = Color.FromArgb(tempInteger)
                            End If
                        ElseIf iniFileValue.StartsWith("System.Collections.Specialized.StringCollection") Then
                            iniFileValue = iniFileValue.Replace("System.Collections.Specialized.StringCollection,", "")
                            Dim deJSONedObject As String() = (New Web.Script.Serialization.JavaScriptSerializer).Deserialize(iniFileValue, GetType(String()))

                            Dim tempStringCollection As New Specialized.StringCollection
                            For i = 0 To deJSONedObject.Count - 1
                                tempStringCollection.Add(deJSONedObject(i))
                            Next
                            My.Settings(iniFileKeyName) = tempStringCollection
                        ElseIf iniFileValue.StartsWith("System.Drawing.Point") Then
                            iniFileValue = iniFileValue.Replace("System.Drawing.Point,", "").Trim
                            Dim pointParts() As String = iniFileValue.Split("|")
                            My.Settings(iniFileKeyName) = New Point(pointParts(0), pointParts(1))
                        End If
                    Catch ex As Configuration.SettingsPropertyNotFoundException
                        ' Does nothing
                    End Try
                Next

                My.Settings.Save()

                systemDrawingSizeRegexObject = Nothing

                Dim registryKeyWeAreWorkingWith As RegistryKey = Registry.LocalMachine.OpenSubKey(globalVariables.registryValues.strKey, True)

                For Each valueObject As Object In iniFile.GetSection("Registry").Keys
                    iniFileKeyName = valueObject.name.ToString.Replace("_", " ")
                    iniFileValue = valueObject.value.ToString

                    registryKeyWeAreWorkingWith.SetValue(iniFileKeyName, iniFileValue, RegistryValueKind.String)
                Next

                registryKeyWeAreWorkingWith.Close()
                registryKeyWeAreWorkingWith.Dispose()

                iniFile = Nothing

                Functions.eventLogFunctions.writeToSystemEventLog(String.Format("A configuration backup has been restored from {0}{1}{0} by user {0}{2}{0}.", Chr(34), importBackupDialog.FileName, Environment.UserName), EventLogEntryType.Information)

                loadPreferences()
                loadRestorePointListColumnOrder()

                MsgBox("Backup configuration file restoration complete.", MsgBoxStyle.Information, strMessageBoxTitle)
            End If
        End If
    End Sub

    Private Sub MountVolumeShadowCopyToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles MountVolumeShadowCopyToolStripMenuItem.Click
        If (globalVariables.windows.mountVolumeShadowCopy Is Nothing) Then
            globalVariables.windows.mountVolumeShadowCopy = New Mount_Volume_Shadow_Copy
            globalVariables.windows.mountVolumeShadowCopy.StartPosition = FormStartPosition.CenterScreen
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
            globalVariables.windows.configureProxy = New Configure_Proxy
            globalVariables.windows.configureProxy.StartPosition = FormStartPosition.CenterParent
            globalVariables.windows.configureProxy.Show()
        Else
            globalVariables.windows.configureProxy.BringToFront()
        End If
    End Sub

    Private Sub AskBeforeUpgradingUpdatingToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles AskBeforeUpgradingUpdatingToolStripMenuItem.Click
        My.Settings.askToUpgrade = AskBeforeUpgradingUpdatingToolStripMenuItem.Checked
    End Sub

    Private Sub RemoveSafeModeBootOptionToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles RemoveSafeModeBootOptionToolStripMenuItem.Click
        Functions.registryStuff.removeSafeModeBoot(True)
        MsgBox("The setting to boot your system into Safe Mode has been removed. Your system will now reboot.", MsgBoxStyle.Information, strMessageBoxTitle)
        Functions.support.rebootSystem()
    End Sub

    Private Sub FrequentlyAskedQuestionsToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles FrequentlyAskedQuestionsToolStripMenuItem.Click
        Functions.support.launchURLInWebBrowser(globalVariables.webURLs.webPages.strFAQ)
    End Sub

    Private Sub LogProgramLoadsAndExitsToEventLogToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles LogProgramLoadsAndExitsToEventLogToolStripMenuItem.Click
        globalVariables.boolLogLoadsAndExitsToEventLog = LogProgramLoadsAndExitsToEventLogToolStripMenuItem.Checked

        If globalVariables.boolLogLoadsAndExitsToEventLog = True Then
            savePreferenceToRegistry("Log Program Loads And Exits to Event Log", "True")
        Else
            savePreferenceToRegistry("Log Program Loads And Exits to Event Log", "False")
        End If
    End Sub

    Private Sub ContactTheDeveloperToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles ContactTheDeveloperToolStripMenuItem.Click
        If (globalVariables.windows.officialContactForm Is Nothing) Then
            globalVariables.windows.officialContactForm = New Official_Contact_Form
            globalVariables.windows.officialContactForm.StartPosition = FormStartPosition.CenterParent
            globalVariables.windows.officialContactForm.Show()
        Else
            globalVariables.windows.officialContactForm.BringToFront()
        End If
    End Sub

    Private Sub RoundTheAgeOfRestorePointInDaysToHowManyDecimalsToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles RoundTheAgeOfRestorePointInDaysToHowManyDecimalsToolStripMenuItem.Click
        Dim roundWindow As New Round_Restore_Point_Age_in_Days
        roundWindow.StartPosition = FormStartPosition.CenterParent
        roundWindow.ShowDialog()

        Dim response As Round_Restore_Point_Age_in_Days.userResponse = roundWindow.dialogResponse
        If response = Round_Restore_Point_Age_in_Days.userResponse.ok Then btnRefreshRestorePoints.PerformClick()
    End Sub

    Private Sub CommandLineArgumentsToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles CommandLineArgumentsToolStripMenuItem.Click
        Dim argumentsWindows As New Restore_Point_Creator_Command_Line_Arguments
        argumentsWindows.StartPosition = FormStartPosition.CenterParent
        argumentsWindows.ShowDialog()
    End Sub

    Private Sub stripRestoreSafeMode_Click(sender As Object, e As EventArgs) Handles stripRestoreSafeMode.Click
        btnRestoreToRestorePointSafeMode.PerformClick()
    End Sub

    Private Sub CreateRestorePointAtUserLogonToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles CreateRestorePointAtUserLogonToolStripMenuItem.Click
        If (globalVariables.windows.frmCreateRestorePointAtUserLogon Is Nothing) Then
            globalVariables.windows.frmCreateRestorePointAtUserLogon = New Create_Restore_Point_at_User_Logon
            globalVariables.windows.frmCreateRestorePointAtUserLogon.StartPosition = FormStartPosition.CenterParent
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
            globalVariables.windows.eventLogForm = New eventLogForm
            globalVariables.windows.eventLogForm.StartPosition = FormStartPosition.CenterScreen
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
            Functions.wait.createPleaseWaitWindow("Downloading Debug Build... Please Wait.", True)

            Dim downloadThread As New Threading.Thread(AddressOf switchToDebugBuildDownloadThreadSub)
            downloadThread.Name = "Debug Build Download Thread"
            downloadThread.Start()
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

        If InterfaceTooBigToolStripMenuItem.Checked = True Then
            Dim registryKeyWeAreWorkingWith As RegistryKey = Registry.LocalMachine.OpenSubKey("SOFTWARE\Microsoft\Windows NT\CurrentVersion\AppCompatFlags\Layers", True)
            registryKeyWeAreWorkingWith.SetValue(Process.GetCurrentProcess.MainModule.FileName.ToLower, "~ HIGHDPIAWARE", RegistryValueKind.String)
            registryKeyWeAreWorkingWith.Close()
            registryKeyWeAreWorkingWith.Dispose()

            MsgBox("The compatibility flag has been set for the program. The program will relaunch for the changes to take effect.", MsgBoxStyle.Information, strMessageBoxTitle)
            Functions.support.reRunWithAdminUserRights()
        Else
            Dim registryKeyWeAreWorkingWith As RegistryKey = Registry.LocalMachine.OpenSubKey("SOFTWARE\Microsoft\Windows NT\CurrentVersion\AppCompatFlags\Layers", True)
            registryKeyWeAreWorkingWith.DeleteValue(Process.GetCurrentProcess.MainModule.FileName.ToLower, False)
            registryKeyWeAreWorkingWith.Close()
            registryKeyWeAreWorkingWith.Dispose()
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
        My.Settings.notificationType = globalVariables.notificationTypeMessageBox
        My.Settings.Save()
        'changeMessageTypeMenuItems()
    End Sub

    Private Sub BalloonToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles BalloonToolStripMenuItem.Click
        MessageBoxToolStripMenuItem.Checked = False
        My.Settings.notificationType = globalVariables.notificationTypeBalloon
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
            globalVariables.windows.frmChangeLog = New Change_Log
            globalVariables.windows.frmChangeLog.StartPosition = FormStartPosition.CenterParent
            globalVariables.windows.frmChangeLog.Show()
        Else
            globalVariables.windows.frmChangeLog.BringToFront()
        End If
    End Sub

    Private Sub toolStripViewDiskSpaceUsage_Click(sender As Object, e As EventArgs) Handles toolStripViewDiskSpaceUsage.Click
        If (globalVariables.windows.frmDiskSpaceUsageWindow Is Nothing) Then
            globalVariables.windows.frmDiskSpaceUsageWindow = New Disk_Space_Usage
            globalVariables.windows.frmDiskSpaceUsageWindow.StartPosition = FormStartPosition.CenterScreen
            globalVariables.windows.frmDiskSpaceUsageWindow.Show()
            globalVariables.windows.frmDiskSpaceUsageWindow.Location = My.Settings.DiskSpaceUsageWindowLocation
        Else
            globalVariables.windows.frmDiskSpaceUsageWindow.BringToFront()
        End If
    End Sub

    Private Sub toolStripManageSystemRestoreStorageSize_Click(sender As Object, e As EventArgs) Handles toolStripManageSystemRestoreStorageSize.Click
        If (globalVariables.windows.frmManageSystemRestoreStorageSpace Is Nothing) Then
            globalVariables.windows.frmManageSystemRestoreStorageSpace = New frmManageSystemRestoreStorageSpace
            globalVariables.windows.frmManageSystemRestoreStorageSpace.StartPosition = FormStartPosition.CenterScreen
            globalVariables.windows.frmManageSystemRestoreStorageSpace.Show()
            globalVariables.windows.frmManageSystemRestoreStorageSpace.Location = My.Settings.ManageSystemRestoreStorageSpaceWindowLocation
        Else
            globalVariables.windows.frmManageSystemRestoreStorageSpace.BringToFront()
        End If
    End Sub

    Private Sub toolStripScheduleRestorePoints_Click(sender As Object, e As EventArgs) Handles toolStripScheduleRestorePoints.Click
        If (globalVariables.windows.frmTaskScheduler Is Nothing) Then
            globalVariables.windows.frmTaskScheduler = New frmTaskScheduler
            globalVariables.windows.frmTaskScheduler.StartPosition = FormStartPosition.CenterScreen
            globalVariables.windows.frmTaskScheduler.Show()
            globalVariables.windows.frmTaskScheduler.Location = My.Settings.TaskSchedulerWindowLocation
        Else
            globalVariables.windows.frmTaskScheduler.BringToFront()
        End If
    End Sub

    Private Sub toolStripMyComputer_Click(sender As Object, e As EventArgs) Handles toolStripMyComputer.Click
        If Functions.osVersionInfo.isThisWindows10() = True Then
            Exit Sub
        End If

        If toolStripMyComputer.Checked Then
            Try
                ' Attempts to fix something that should never have been broken to begin with.  Why this would be broken, who the fuck knows.
                If (Registry.ClassesRoot.OpenSubKey("CLSID\{20D04FE0-3AEA-1069-A2D8-08002B30309D}", False) Is Nothing) Then
                    Registry.ClassesRoot.OpenSubKey("CLSID", True).CreateSubKey("{20D04FE0-3AEA-1069-A2D8-08002B30309D}")
                    Registry.ClassesRoot.OpenSubKey("CLSID\{20D04FE0-3AEA-1069-A2D8-08002B30309D}", True).CreateSubKey("Shell")
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
                If Registry.ClassesRoot.OpenSubKey("CLSID\{20D04FE0-3AEA-1069-A2D8-08002B30309D}\Shell\Create System Restore Checkpoint") IsNot Nothing Then
                    Registry.ClassesRoot.OpenSubKey("CLSID\{20D04FE0-3AEA-1069-A2D8-08002B30309D}\Shell", True).DeleteSubKeyTree("Create System Restore Checkpoint")
                End If

                Registry.ClassesRoot.OpenSubKey("CLSID\{20D04FE0-3AEA-1069-A2D8-08002B30309D}\Shell", True).CreateSubKey("Create System Restore Checkpoint")
                Registry.ClassesRoot.OpenSubKey("CLSID\{20D04FE0-3AEA-1069-A2D8-08002B30309D}\Shell\Create System Restore Checkpoint", True).CreateSubKey("Command")

                registryKey = Registry.ClassesRoot.OpenSubKey("CLSID\{20D04FE0-3AEA-1069-A2D8-08002B30309D}\Shell\Create System Restore Checkpoint", True)
                If boolIsThisVistaOrNewer = True Then
                    registryKey.SetValue("HasLUAShield", "", RegistryValueKind.String)
                End If
                registryKey.SetValue("icon", String.Format("{0}{1}{0}", Chr(34), Application.ExecutablePath), RegistryValueKind.String)
                registryKey.SetValue("SuppressionPolicy", 1073741884, RegistryValueKind.DWord)
                registryKey.Close()
                registryKey.Dispose()

                registryKey = Registry.ClassesRoot.OpenSubKey("CLSID\{20D04FE0-3AEA-1069-A2D8-08002B30309D}\Shell\Create System Restore Checkpoint\Command", True)
                registryKey.SetValue(vbNullString, String.Format("{0}{1}{0} -createrestorepoint", Chr(34), Application.ExecutablePath))
                registryKey.Close()
                registryKey.Dispose()

                ' ===============================================================
                ' == Makes the "Create Custom Named System Restore Point" Item ==
                ' ===============================================================

                ' Checks if an existing entry exists and deletes it.
                If Registry.ClassesRoot.OpenSubKey("CLSID\{20D04FE0-3AEA-1069-A2D8-08002B30309D}\Shell\Create Custom Named System Restore Point") IsNot Nothing Then
                    Registry.ClassesRoot.OpenSubKey("CLSID\{20D04FE0-3AEA-1069-A2D8-08002B30309D}\Shell", True).DeleteSubKeyTree("Create Custom Named System Restore Point")
                End If

                Registry.ClassesRoot.OpenSubKey("CLSID\{20D04FE0-3AEA-1069-A2D8-08002B30309D}\Shell", True).CreateSubKey("Create Custom Named System Restore Point")
                Registry.ClassesRoot.OpenSubKey("CLSID\{20D04FE0-3AEA-1069-A2D8-08002B30309D}\Shell\Create Custom Named System Restore Point", True).CreateSubKey("Command")

                registryKey = Registry.ClassesRoot.OpenSubKey("CLSID\{20D04FE0-3AEA-1069-A2D8-08002B30309D}\Shell\Create Custom Named System Restore Point", True)
                If boolIsThisVistaOrNewer = True Then
                    registryKey.SetValue("HasLUAShield", "", RegistryValueKind.String)
                End If
                registryKey.SetValue("icon", String.Format("{0}{1}{0}", Chr(34), Application.ExecutablePath), RegistryValueKind.String)
                registryKey.SetValue("SuppressionPolicy", 1073741884, RegistryValueKind.DWord)
                registryKey.Close()
                registryKey.Dispose()

                registryKey = Registry.ClassesRoot.OpenSubKey("CLSID\{20D04FE0-3AEA-1069-A2D8-08002B30309D}\Shell\Create Custom Named System Restore Point\Command", True)
                registryKey.SetValue(vbNullString, String.Format("{0}{1}{0} -createrestorepointcustomname", Chr(34), Application.ExecutablePath))
                registryKey.Close()
                registryKey.Dispose()

                ' ===================================================
                ' == Makes the "Launch Restore Point Creator" Item ==
                ' ===================================================

                ' Checks if an existing entry exists and deletes it.
                If Registry.ClassesRoot.OpenSubKey("CLSID\{20D04FE0-3AEA-1069-A2D8-08002B30309D}\Shell\Launch Restore Point Creator") IsNot Nothing Then
                    Registry.ClassesRoot.OpenSubKey("CLSID\{20D04FE0-3AEA-1069-A2D8-08002B30309D}\Shell", True).DeleteSubKeyTree("Launch Restore Point Creator")
                End If

                Registry.ClassesRoot.OpenSubKey("CLSID\{20D04FE0-3AEA-1069-A2D8-08002B30309D}\Shell", True).CreateSubKey("Launch Restore Point Creator")
                Registry.ClassesRoot.OpenSubKey("CLSID\{20D04FE0-3AEA-1069-A2D8-08002B30309D}\Shell\Launch Restore Point Creator", True).CreateSubKey("Command")

                registryKey = Registry.ClassesRoot.OpenSubKey("CLSID\{20D04FE0-3AEA-1069-A2D8-08002B30309D}\Shell\Launch Restore Point Creator", True)
                If boolIsThisVistaOrNewer = True Then
                    registryKey.SetValue("HasLUAShield", "", RegistryValueKind.String)
                End If
                registryKey.SetValue("icon", String.Format("{0}{1}{0}", Chr(34), Application.ExecutablePath), RegistryValueKind.String)
                registryKey.SetValue("SuppressionPolicy", 1073741884, RegistryValueKind.DWord)
                registryKey.Close()
                registryKey.Dispose()

                registryKey = Registry.ClassesRoot.OpenSubKey("CLSID\{20D04FE0-3AEA-1069-A2D8-08002B30309D}\Shell\Launch Restore Point Creator\Command", True)
                registryKey.SetValue(vbNullString, String.Format("{0}{1}{0}", Chr(34), Application.ExecutablePath))
                registryKey.Close()
                registryKey.Dispose()
            Catch ex As Security.SecurityException
                toolStripMyComputer.Checked = False
                Functions.eventLogFunctions.writeCrashToEventLog(ex)
                MsgBox("Unable to modify the My Computer right-click context menu. A security violation has occurred. Please contact your system administrator for assistance.", MsgBoxStyle.Critical, strMessageBoxTitle)
            End Try
        Else
            Try
                ' This code removes the options from the "My Computer" right-click context menu.

                ' All of this prevents a rare Null Reference Exception.
                If Registry.ClassesRoot.OpenSubKey("CLSID\{20D04FE0-3AEA-1069-A2D8-08002B30309D}\Shell\Create System Restore Checkpoint", False) IsNot Nothing Then
                    Registry.ClassesRoot.OpenSubKey("CLSID\{20D04FE0-3AEA-1069-A2D8-08002B30309D}\Shell", True).DeleteSubKeyTree("Create System Restore Checkpoint")
                End If

                If Registry.ClassesRoot.OpenSubKey("CLSID\{20D04FE0-3AEA-1069-A2D8-08002B30309D}\Shell\Create Custom Named System Restore", False) IsNot Nothing Then
                    Registry.ClassesRoot.OpenSubKey("CLSID\{20D04FE0-3AEA-1069-A2D8-08002B30309D}\Shell", True).DeleteSubKeyTree("Create Custom Named System Restore")
                End If

                If Registry.ClassesRoot.OpenSubKey("CLSID\{20D04FE0-3AEA-1069-A2D8-08002B30309D}\Shell\Create Custom Named System Restore Point", False) IsNot Nothing Then
                    Registry.ClassesRoot.OpenSubKey("CLSID\{20D04FE0-3AEA-1069-A2D8-08002B30309D}\Shell", True).DeleteSubKeyTree("Create Custom Named System Restore Point")
                End If

                If Registry.ClassesRoot.OpenSubKey("CLSID\{20D04FE0-3AEA-1069-A2D8-08002B30309D}\Shell\Launch Restore Point Creator", False) IsNot Nothing Then
                    Registry.ClassesRoot.OpenSubKey("CLSID\{20D04FE0-3AEA-1069-A2D8-08002B30309D}\Shell", True).DeleteSubKeyTree("Launch Restore Point Creator")
                End If
                ' All of this prevents a rare Null Reference Exception.
            Catch ex As Security.SecurityException
                Functions.eventLogFunctions.writeCrashToEventLog(ex)
                MsgBox("Unable to modify the My Computer right-click context menu. A security violation has occurred. Please contact your system administrator for assistance.", MsgBoxStyle.Critical, strMessageBoxTitle)
            End Try
        End If
    End Sub

    Private Sub toolStripLogRestorePointDeletions_Click(sender As Object, e As EventArgs) Handles toolStripLogRestorePointDeletions.Click
        savePreferenceToRegistry("Log Restore Point Deletions", toolStripLogRestorePointDeletions.Checked.ToString)
    End Sub

    Private Sub toolStripCheckForUpdates_Click(sender As Object, e As EventArgs) Handles toolStripCheckForUpdates.Click
        userInitiatedCheckForUpdatesThread = New Threading.Thread(AddressOf userInitiatedCheckForUpdates)
        userInitiatedCheckForUpdatesThread.Name = "User Initiated Check For Updates Thread"
        userInitiatedCheckForUpdatesThread.Priority = Threading.ThreadPriority.Lowest
        userInitiatedCheckForUpdatesThread.Start()
        toolStripCheckForUpdates.Enabled = False
    End Sub

    Private Sub ProductWebSiteToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles ProductWebSiteToolStripMenuItem.Click
        Functions.support.launchURLInWebBrowser(globalVariables.webURLs.core.strWebSite, "An error occurred when trying to launch the product's web site URL in your default browser. The URL has been copied to your Windows Clipboard for you to paste into the address bar in the browser of your choice.")
    End Sub

    Private Sub AboutThisProgramToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles AboutThisProgramToolStripMenuItem.Click
        Dim stringBuilder As New StringBuilder

        stringBuilder.AppendLine("Restore Point Creator")
        stringBuilder.AppendLine("Written By Tom Parkison")
        stringBuilder.AppendLine("Copyright Thomas Parkison 2012-2017.")
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

        If registryShowDonationMessageValue.ToLower = "true" Or registryShowDonationMessageValue.ToLower = "false" Then
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
            Dim frmDeleteOldSystemRestorePointsInstance = New frmDeleteOldSystemRestorePoints
            frmDeleteOldSystemRestorePointsInstance.StartPosition = FormStartPosition.CenterScreen
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
            Functions.wait.createPleaseWaitWindow("Deleting Restore Points... Please Wait.")

            toolStripDeleteAllRestorePoints.Enabled = False
            deleteAllRestorePointsThreadInstance = New Threading.Thread(AddressOf deleteAllRestorePointsThread)
            deleteAllRestorePointsThreadInstance.Name = "Delete All Restore Points Thread"
            deleteAllRestorePointsThreadInstance.Priority = Threading.ThreadPriority.Lowest
            deleteAllRestorePointsThreadInstance.Start()

            Functions.wait.openPleaseWaitWindow()
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
                Functions.APIs.MoveFileEx(globalVariables.pdbFileNameInZIP, vbNullString, 4)
            End Try
        End If
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
            Dim Keep_X_Amount_of_Restore_PointsInstance = New createRestorePointAtUserLogon
            Keep_X_Amount_of_Restore_PointsInstance.parentFormG = Me
            Keep_X_Amount_of_Restore_PointsInstance.StartPosition = FormStartPosition.CenterParent
            Keep_X_Amount_of_Restore_PointsInstance.ShowDialog()

            Keep_X_Amount_of_Restore_PointsInstance.Dispose()
            Keep_X_Amount_of_Restore_PointsInstance = Nothing
        Else
            Dim registryKey As RegistryKey = Registry.LocalMachine.OpenSubKey(globalVariables.registryValues.strKey, True)

            registryKey.SetValue("Keep X Amount of Restore Points", "False", RegistryValueKind.String)
            registryKey.DeleteValue("Keep X Amount of Restore Points Value", False)

            registryKey.Close()
            registryKey.Dispose()

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

        Dim checkForUpdatesEveryInstance As New checkForUpdatesEvery
        checkForUpdatesEveryInstance.parentFormG = Me
        checkForUpdatesEveryInstance.StartPosition = FormStartPosition.CenterParent
        checkForUpdatesEveryInstance.ShowDialog()
    End Sub

    Private Sub HelpToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles HelpToolStripMenuItem.Click
        Functions.support.launchURLInWebBrowser(globalVariables.webURLs.webPages.strHelpVideos, "An error occurred when trying to launch the Help Videos URL in your default browser. The URL has been copied to your Windows Clipboard for you to paste into the address bar in the browser of your choice.")
    End Sub

    Private Sub SetBarColorToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles SetBarColorToolStripMenuItem.Click
        If My.Settings.customColors2 IsNot Nothing Then
            Dim integerArray(My.Settings.customColors2.Count - 1) As Integer

            For i = 0 To My.Settings.customColors2.Count - 1
                integerArray(i) = Integer.Parse(My.Settings.customColors2(i))
            Next

            ColorDialog.CustomColors = integerArray
            integerArray = Nothing
        End If

        ColorDialog.Color = My.Settings.barColor

        If ColorDialog.ShowDialog() = DialogResult.OK Then
            My.Settings.barColor = ColorDialog.Color

            Dim temp As New Specialized.StringCollection
            For Each entry As String In ColorDialog.CustomColors
                temp.Add(entry)
            Next
            My.Settings.customColors2 = temp
            My.Settings.Save()
            temp = Nothing

            MsgBox("Color Preference Saved.", MsgBoxStyle.Information, "Setting Saved")
        End If
    End Sub

    Private Sub DefaultCustomRestorePointNameToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles DefaultCustomRestorePointNameToolStripMenuItem.Click
        Dim setDefaultCustomRestorePointNameWindowInstance As New Set_Default_Custom_Restore_Point_Name
        setDefaultCustomRestorePointNameWindowInstance.StartPosition = FormStartPosition.CenterParent
        setDefaultCustomRestorePointNameWindowInstance.parentFormG = Me
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

        Dim selectedRestorePoint As ListViewItem = systemRestorePointsList.SelectedItems(0)

        Dim strDescription As String = selectedRestorePoint.SubItems(enums.restorePointListSubItems.restorePointName).Text
        Dim strDate As String = selectedRestorePoint.SubItems(enums.restorePointListSubItems.restorePointDate).Text
        Dim strType As String = selectedRestorePoint.SubItems(enums.restorePointListSubItems.restorePointType).Text

        Dim msgboxResult As MsgBoxResult = MsgBox(String.Format("Are you sure you want to restore your system back to the selected System Restore Point?  Your system will reboot into Safe Mode and perform the restore process there and reboot after the process is complete.{0}{0}Description: {1}{0}Created On: {2}{0}Type: {3}", vbCrLf, strDescription, strDate, strType), MsgBoxStyle.YesNo + MsgBoxStyle.Question, "Are you sure?")

        If msgboxResult = MsgBoxResult.Yes Then
            savePreferenceToRegistry(globalVariables.registryValues.strSafeModeValue, "True")
            Functions.support.setSafeModeBoot() ' Set the system up for Safe Mode Boot.

            ' Set the restore point that we're going to restore back to.
            savePreferenceToRegistry("Preselected Restore Point for Restore in Safe Mode", Integer.Parse(systemRestorePointsList.SelectedItems(0).SubItems(0).Text))

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

        Functions.wait.createPleaseWaitWindow("Creating Restore Point... Please Wait.")

        Dim createSystemRestorePointThread As New Threading.Thread(Sub() unifiedCreateSystemRestorePoint(defaultCustomRestorePointName))
        createSystemRestorePointThread.Name = "Create System Restore Point Thread"
        createSystemRestorePointThread.Priority = Threading.ThreadPriority.Normal
        createSystemRestorePointThread.Start()
        createSystemRestorePointThread = Nothing

        Functions.wait.openPleaseWaitWindow()
    End Sub

    Private Sub btnCreate_Click(ByVal sender As Object, ByVal e As EventArgs) Handles btnCreate.Click
        If Functions.support.areWeInSafeMode() = True Then
            MsgBox("You are in Safe Mode, it's not recommended to make restore points in Safe Mode.", MsgBoxStyle.Information, strMessageBoxTitle)
            Exit Sub
        End If

        If txtRestorePointDescription.Text.caseInsensitiveContains(strTypeYourRestorePointName) = True Then
            Exit Sub
        End If

        If txtRestorePointDescription.Text = Nothing Then Exit Sub

        txtRestorePointDescription.Text = txtRestorePointDescription.Text.Trim

        Dim msgBoxResult As MsgBoxResult = MsgBoxResult.Yes

        If My.Settings.askBeforeCreatingRestorePoint = True Then
            msgBoxResult = MsgBox(String.Format("Are you sure you want to create a new system restore point with the name of {0}{1}{0}?", Chr(34), txtRestorePointDescription.Text), MsgBoxStyle.Question + vbYesNo, "Restore Point Creator")
        End If

        If msgBoxResult = MsgBoxResult.Yes Then
            Functions.wait.createPleaseWaitWindow("Creating Restore Point... Please Wait.")

            Dim createSystemRestorePointThread As New Threading.Thread(Sub() unifiedCreateSystemRestorePoint(txtRestorePointDescription.Text))
            createSystemRestorePointThread.Name = "Create System Restore Point Thread"
            createSystemRestorePointThread.Priority = Threading.ThreadPriority.Normal
            createSystemRestorePointThread.Start()
            createSystemRestorePointThread = Nothing

            Functions.wait.openPleaseWaitWindow()
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
            Functions.wait.createPleaseWaitWindow("Creating Restore Point... Please Wait.")

            Dim threadCreateSystemCheckpoint As New Threading.Thread(Sub() unifiedCreateSystemRestorePoint())
            threadCreateSystemCheckpoint.Name = "Create System Checkpoint Thread"
            threadCreateSystemCheckpoint.Priority = Threading.ThreadPriority.Normal
            threadCreateSystemCheckpoint.Start()
            threadCreateSystemCheckpoint = Nothing

            Functions.wait.openPleaseWaitWindow()
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

        Dim selectedRestorePoint As ListViewItem = systemRestorePointsList.SelectedItems(0)

        Dim strDescription As String = selectedRestorePoint.SubItems(enums.restorePointListSubItems.restorePointName).Text
        Dim strDate As String = selectedRestorePoint.SubItems(enums.restorePointListSubItems.restorePointDate).Text
        Dim strType As String = selectedRestorePoint.SubItems(enums.restorePointListSubItems.restorePointType).Text

        Dim msgboxResult As MsgBoxResult = MsgBox(String.Format("Are you sure you want to restore your system back to the selected System Restore Point?  Your system will reboot after the restoration process is complete.{0}{0}Description: {1}{0}Created On: {2}{0}Type: {3}", vbCrLf, strDescription, strDate, strType), MsgBoxStyle.YesNo + MsgBoxStyle.Question, "Are you sure?")

        If msgboxResult = MsgBoxResult.Yes Then
            Functions.wait.createPleaseWaitWindow("Beginning the Restore Process... Please wait.")

            Dim thread As New Threading.Thread(AddressOf restoreSystemRestorePoint)
            thread.Name = "Restore System Restore Point Thread"
            thread.Priority = Threading.ThreadPriority.Normal
            thread.Start()

            Functions.wait.openPleaseWaitWindow()
        Else
            MsgBox("Your system has NOT been restored to the selected System Restore Point.", MsgBoxStyle.Information, strMessageBoxTitle)
        End If
    End Sub

    Private Sub btnClose_Click(sender As Object, e As EventArgs)
        Me.Close()
    End Sub

    Private Sub btnRefreshRestorePoints_Click(ByVal sender As Object, ByVal e As EventArgs) Handles btnRefreshRestorePoints.Click
        If btnRefreshRestorePoints.Text = "Refresh List of System Restore Points" Then
            Functions.wait.createPleaseWaitWindow("Loading Restore Points... Please Wait.")

            updateRestorePointListThread = New Threading.Thread(AddressOf loadRestorePointsFromSystemIntoList)
            updateRestorePointListThread.Name = "System Restore Point List Updating Thread"
            updateRestorePointListThread.Priority = Threading.ThreadPriority.Normal
            updateRestorePointListThread.Start()

            Functions.wait.openPleaseWaitWindow()
        Else
            If updateRestorePointListThread IsNot Nothing Then
                updateRestorePointListThread.Abort()
                btnRefreshRestorePoints.Text = "Refresh List of System Restore Points"
            End If
        End If
    End Sub

    Private Sub btnDeleteRestorePoint_Click(sender As Object, e As EventArgs) Handles btnDeleteRestorePoint.Click
        If systemRestorePointsList.SelectedItems.Count = 0 Then
            MsgBox("You must select one Or more System Restore Points to delete.", MsgBoxStyle.Information, strMessageBoxTitle)
            Exit Sub
        End If

        Functions.wait.createPleaseWaitWindow("Deleting Restore Points... Please Wait.")

        deleteThread = New Threading.Thread(AddressOf deleteSystemRestorePoint)
        deleteThread.Name = "System Restore Point Deletion Thread"
        deleteThread.Priority = Threading.ThreadPriority.Normal
        deleteThread.Start()

        Functions.wait.openPleaseWaitWindow()
    End Sub
#End Region

#Region "--== DON'T TOUCH THIS ==--"
    Public Sub New()
        ' This call is required by the designer.
        InitializeComponent()

        ' Add any initialization after the InitializeComponent() call.
    End Sub
#End Region
End Class