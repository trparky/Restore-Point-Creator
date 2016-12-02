Imports System.Text.RegularExpressions
Imports Microsoft.Win32.TaskScheduler
Imports Microsoft.Win32

Namespace My
    ' The following events are available for MyApplication:
    '
    ' Startup: Raised when the application starts, before the startup form is created.
    ' Shutdown: Raised after all application forms are closed.  This event is not raised if the application terminates abnormally.
    ' UnhandledException: Raised if the application encounters an unhandled exception.
    ' StartupNextInstance: Raised when launching a single-instance application and the application is already active.
    ' NetworkAvailabilityChanged: Raised when the network connection is connected or disconnected.
    Partial Friend Class MyApplication
        Private pleaseWaitInstance As Please_Wait

        Private Sub MyApplication_Startup(sender As Object, e As ApplicationServices.StartupEventArgs) Handles Me.Startup
            exceptionHandler.loadExceptionHandler()
            Functions.startupFunctions.validateSettings()

            Dim commandLineArgument As String
            Dim registryKey As RegistryKey
            Dim boolNoTask As Boolean = False ' Create a Boolean data type variable.

            If Environment.OSVersion.ToString.Contains("5.1") = True Or Environment.OSVersion.ToString.Contains("5.2") = True Then
                Functions.startupFunctions.downloadWindowsXPVersion()
            End If

            If Functions.osVersionInfo.isThisAServerOS() = True Then
                MsgBox("You are running a Server edition of Microsoft Windows. System Restore Point Creator doesn't function on server operating systems." & vbCrLf & vbCrLf & "This application will now close.", MsgBoxStyle.Critical, "System Restore Point Creator -- Application Error")
                Process.GetCurrentProcess.Kill()
            End If

            If IO.File.Exists("portable.mode") = True Or IO.File.Exists("portablemode.txt") = True Then
                globalVariables.boolPortableMode = True
                boolNoTask = True
            End If

            ' This is to test the crash submission code. Make sure this block is commented out before compiling for public release.
            'Try
            '    Throw New IO.FileLoadException()
            'Catch ex As Exception
            '    exceptionHandler.manuallyLoadCrashWindow(ex)
            'End Try
            ' This is to test the crash submission code. Make sure this block is commented out before compiling for public release.

            ' We're going to store the result of the Functions.areWeAnAdministrator() call in this Boolean variable for later use in this code block.
            Dim boolAreWeAnAdministrator As Boolean = Functions.privilegeChecks.areWeAnAdministrator()

            ' We're going to store the result of the Functions.support.areWeInSafeMode() call in this Boolean variable for later use in this code block.
            Dim boolAreWeInSafeMode As Boolean = Functions.support.areWeInSafeMode()

            ' This checks to see if our Registry Key doesn't exist. If it doesn't exist, we need to create it.
            If Registry.LocalMachine.OpenSubKey(globalVariables.registryValues.strKey) Is Nothing Then
                ' OK, it doesn't exist so we have to create it.

                ' But first, we need to check if we are running with an Administrator user rights token.
                If boolAreWeAnAdministrator = True Then
                    ' OK, we're running as an Administrator so we can continue to create our Registry Key.

                    Try
                        ' First we open the SOFTWARE key in HKEY_LOCAL_MACHINE and then create a subkey in the SOFTWARE key.
                        Registry.LocalMachine.OpenSubKey("SOFTWARE", True).CreateSubKey(globalVariables.registryValues.strKeyInsideSoftware)

                        ' Now we check to make sure that the our Registry key has been created.
                        If Registry.LocalMachine.OpenSubKey(globalVariables.registryValues.strKey) Is Nothing Then
                            ' It hasn't so we give the user a message stating that something went wrong.
                            Functions.startupFunctions.giveMessageToUserAboutNotBeingAbleToCreateRegistrySubKey()

                            ' And now we close the program.
                            e.Cancel = True
                            Exit Sub
                        End If
                    Catch ex As Exception
                        ' Something really went wrong so we give the user a message stating that something went wrong.
                        Functions.startupFunctions.giveMessageToUserAboutNotBeingAbleToCreateRegistrySubKey()

                        ' And now we close the program.
                        e.Cancel = True
                        Exit Sub
                    End Try
                Else
                    ' We're not running as an Administrator so we have to relaunch this program while triggering a UAC prompt. The next
                    ' time this program runs we will be running as an Administrator so the above code will run instead of this branch.
                    Functions.support.reRunWithAdminUserRights()
                End If
            End If

            ' This code is for running parts of the program that don't necessarily need Admin privs.
            If My.Application.CommandLineArgs.Count >= 1 Then
                commandLineArgument = My.Application.CommandLineArgs(0).Trim

                If commandLineArgument.stringCompare(globalVariables.commandLineSwitches.viewChangeLog) Then
                    Dim changeLog As New Change_Log
                    changeLog.ShowDialog()
                    e.Cancel = True
                    Exit Sub
                ElseIf commandLineArgument.stringCompare(globalVariables.commandLineSwitches.viewEventLog) Then
                    Dim eventLogForm As New eventLogForm
                    eventLogForm.ShowDialog()
                    e.Cancel = True
                    Exit Sub
                ElseIf commandLineArgument.stringCompare(globalVariables.commandLineSwitches.createRestorePoint) Then
                    If My.Application.CommandLineArgs.Count = 2 Then
                        If My.Application.CommandLineArgs(1).Trim.StartsWith("-name", StringComparison.OrdinalIgnoreCase) Then
                            My.Settings.savedRestorePointFromCommandLine = My.Application.CommandLineArgs(1).Trim.caseInsensitiveReplace("-name=", "")
                            My.Settings.Save()
                        End If
                    End If

                    Functions.taskStuff.runProgramUsingTaskWrapper()
                End If
            End If

            ' This code checks to see if the current version is a beta or Release Candidate and if the user's update channel is already set to beta mode.
            ' If the user's update channel isn't set to beta mode we then set it for the user here.
            If (globalVariables.version.boolBeta = True Or globalVariables.version.boolReleaseCandidate = True) And My.Settings.updateChannel <> globalVariables.updateChannels.beta Then
                My.Settings.updateChannel = globalVariables.updateChannels.beta ' Changes the update channel to beta.
            End If

            ' Checks to see if the update channel is set to stable, if a debug symbols file exists, and we are an Admin.
            If My.Settings.updateChannel = globalVariables.updateChannels.stable And IO.File.Exists(globalVariables.pdbFileNameInZIP) = True And boolAreWeAnAdministrator = True Then
                Functions.support.deleteFileWithNoException(globalVariables.pdbFileNameInZIP)
            End If

            Dim executablePathPathInfo As New IO.FileInfo(Windows.Forms.Application.ExecutablePath)

#If DEBUG Then
            If Debugger.IsAttached = True And boolAreWeAnAdministrator = False Then
                MsgBox("You must restart Microsoft Visual Studio with Administrator privileges. Debugging will now stop.", MsgBoxStyle.Critical, "Debug Mode Enabled")
                e.Cancel = True
                Exit Sub
            End If
#End If
            ' Reads a special Registry entry from the Registry that instructs the program to not run with the Task Wrapper.
            If Boolean.TryParse(Registry.LocalMachine.OpenSubKey(globalVariables.registryValues.strKey).GetValue("No Task", "False"), boolNoTask) Then
                ' Checks to see if the Registry value was True and if we aren't an Admin.
                If boolNoTask = True And boolAreWeAnAdministrator = False Then
                    Functions.support.reRunWithAdminUserRights() ' OK, we relaunch the process with full Administrator privileges with a UAC prompt.
                End If
            Else
                ' This is in case we couldn't parse what came from the Registry.
                boolNoTask = False
            End If

            ' Checks to see if we are in Safe Mode and if the No Task setting is set to False.  Both conditions have to be False for this code block to run.
            If boolAreWeInSafeMode = False And boolNoTask = False Then
                If Functions.privilegeChecks.IsUserInAdminGroup() = True Then
                    ' This code creates the subfolder in the task scheduler for our runtime tasks.

                    ' Checks to see if the Task Folder doesn't exist.
                    If Functions.taskStuff.doesTaskFolderExist() = False Then
                        ' If we aren't an Administrator, we relaunch this program as an admin.
                        If boolAreWeAnAdministrator = False Then
                            Functions.support.reRunWithAdminUserRights()
                        ElseIf boolAreWeAnAdministrator = True Then ' Yes, we are an Admin, so we go ahead and create the Task folder.
                            Try
                                Dim taskService As New TaskService
                                taskService.RootFolder.CreateFolder(globalVariables.taskFolder)
                                taskService.Dispose()
                                taskService = Nothing
                            Catch ex As UnauthorizedAccessException
                                Functions.support.reRunWithAdminUserRights()
                            Catch ex2 As Runtime.InteropServices.COMException
                                ' If something goes wrong, we're just going to bypass the whole task engine entirely on this system.
                                Registry.LocalMachine.OpenSubKey(globalVariables.registryValues.strKey, True).SetValue("No Task", "True", RegistryValueKind.String)
                                Functions.support.reRunWithAdminUserRights()
                            End Try
                        End If
                    End If

                    If boolAreWeAnAdministrator = True And My.Application.CommandLineArgs.Count = 1 Then
                        commandLineArgument = My.Application.CommandLineArgs(0)

                        If commandLineArgument.stringCompare("-createtasks") Then
                            Functions.eventLogFunctions.writeToSystemEventLog("The program was called with an obsolete command line argument, specifically ""-createtasks"". The program has ignored the command and exited.", EventLogEntryType.Information)
                            Process.GetCurrentProcess.Kill()
                        ElseIf commandLineArgument.stringCompare("-update") Or commandLineArgument.stringCompare("-updatewithoutuninstallinfoupdate") Then
                            Functions.startupFunctions.performApplicationUpdate(commandLineArgument)
                            e.Cancel = True
                            Exit Sub
                        ElseIf commandLineArgument.stringCompare("-fixruntimetasks") Then
                            Functions.startupFunctions.repairRuntimeTasks()

                            e.Cancel = True
                            Exit Sub
                        ElseIf commandLineArgument.stringCompare("-deletealltasks") Then
                            Functions.startupFunctions.deleteAllTasks()
                        End If
                    End If

                    Functions.taskStuff.runProgramUsingTaskWrapper()
                Else
                    Functions.support.reRunWithAdminUserRights()
                End If
            End If

            Try
                ' Ordinarily on Windows Vista and newer this code should not be needed but we have it in there to check if we do indeed have Administrator user rights.
                If boolAreWeAnAdministrator = False Then
                    Functions.support.reRunWithAdminUserRights()
                End If

                registryKey = Registry.LocalMachine.OpenSubKey(globalVariables.registryValues.strKey, False)

                If registryKey IsNot Nothing Then
                    Boolean.TryParse(registryKey.GetValue("Keep X Amount of Restore Points", "False"), globalVariables.KeepXAmountOfRestorePoints)

                    If globalVariables.KeepXAmountOfRestorePoints = True Then
                        Short.TryParse(registryKey.GetValue("Keep X Amount of Restore Points Value", "-10"), globalVariables.KeepXAmountofRestorePointsValue)
                    End If

                    If Boolean.TryParse(registryKey.GetValue("Enable System Logging", "True"), globalVariables.boolLogToSystemLog) = False Then
                        globalVariables.boolLogToSystemLog = True
                    End If

                    If Boolean.TryParse(registryKey.GetValue("Log Program Loads and Exits to Event Log", "True"), globalVariables.boolLogLoadsAndExits) = False Then
                        globalVariables.boolLogLoadsAndExits = True
                    End If

                    registryKey.Close()
                    registryKey.Dispose()
                    registryKey = Nothing
                End If

                If My.Settings.UpdateRequired = True Then
                    My.Settings.Upgrade()
                    My.Settings.UpdateRequired = False
                End If

                If My.Application.CommandLineArgs IsNot Nothing Then
                    If My.Application.CommandLineArgs.Count >= 1 Then
                        commandLineArgument = My.Application.CommandLineArgs(0)

                        If commandLineArgument.stringCompare(globalVariables.commandLineSwitches.createRestorePoint) Then
                            Functions.startupFunctions.giveSafeModeErrorMessage(boolAreWeInSafeMode)
                            Functions.eventLogFunctions.writeToSystemEventLog("Activated JumpList Task.", EventLogEntryType.Information)

                            Dim strRestorePointName As String = "System Checkpoint made by System Restore Point Creator"

                            If My.Application.CommandLineArgs.Count = 2 Then
                                If My.Application.CommandLineArgs(1).Trim.StartsWith("-name=", StringComparison.OrdinalIgnoreCase) Then
                                    strRestorePointName = Regex.Replace(My.Application.CommandLineArgs(1).Trim, "-name=", "", RegexOptions.IgnoreCase)
                                End If
                            End If

                            If String.IsNullOrEmpty(My.Settings.savedRestorePointFromCommandLine) = False Then
                                strRestorePointName = My.Settings.savedRestorePointFromCommandLine
                                My.Settings.savedRestorePointFromCommandLine = Nothing
                                My.Settings.Save()
                            End If

                            Functions.wait.createPleaseWaitWindow("Creating Restore Point... Please Wait.", True, enums.howToCenterWindow.screen, True)

                            ' This is a special way of making a thread in which you can pass parameters to the sub-routine that you're running as a separate thread.
                            Dim creatingThread As New Threading.Thread(Sub()
                                                                           Functions.startupFunctions.createSystemRestorePoint(True, strRestorePointName)
                                                                       End Sub)
                            creatingThread.Name = "Restore Point Creating Thread"
                            creatingThread.Start()

                            e.Cancel = True
                            Exit Sub
                        ElseIf commandLineArgument.stringCompare(globalVariables.commandLineSwitches.createCustomRestorePoint) Then
                            Functions.startupFunctions.giveSafeModeErrorMessage(boolAreWeInSafeMode)
                            Functions.eventLogFunctions.writeToSystemEventLog("Activated JumpList Task.", EventLogEntryType.Information)

                            Dim Custom_Named_Restore_Point_Instance As Custom_Named_Restore_Point
                            Custom_Named_Restore_Point_Instance = New Custom_Named_Restore_Point
                            Custom_Named_Restore_Point_Instance.StartPosition = FormStartPosition.CenterScreen
                            Custom_Named_Restore_Point_Instance.ShowDialog()

                            Dim restorePointName As String

                            If Custom_Named_Restore_Point_Instance.createRestorePoint = False Then
                                MsgBox("Restore Point not created.", MsgBoxStyle.Information, "Restore Point Creator") ' Gives the user some feedback.
                                e.Cancel = True
                                Exit Sub
                            Else
                                restorePointName = Custom_Named_Restore_Point_Instance.restorePointName
                            End If

                            Functions.wait.createPleaseWaitWindow("Creating Restore Point... Please Wait.", True, enums.howToCenterWindow.screen, True)

                            ' This is a special way of making a thread in which you can pass parameters to the sub-routine that you're running as a separate thread.
                            Dim creatingThread As New Threading.Thread(Sub()
                                                                           Functions.startupFunctions.createSystemRestorePoint(True, restorePointName)
                                                                       End Sub)
                            creatingThread.Name = "Restore Point Creating Thread"
                            creatingThread.Start()

                            e.Cancel = True
                            Exit Sub
                        ElseIf commandLineArgument.stringCompare(globalVariables.commandLineSwitches.scheduledRestorePoint) Then
                            Dim restorePointNameForScheduledTasks As String = globalVariables.strDefaultNameForScheduledTasks
                            Dim boolExtendedLoggingForScheduledTasks As Boolean = True
                            Dim oldNewestRestorePointID As Integer

                            registryKey = Registry.LocalMachine.OpenSubKey(globalVariables.registryValues.strKey, False)

                            If registryKey IsNot Nothing Then
                                restorePointNameForScheduledTasks = registryKey.GetValue("Custom Name for Scheduled Restore Points", globalVariables.strDefaultNameForScheduledTasks)

                                If Boolean.TryParse(registryKey.GetValue("Extended Logging For Scheduled Tasks", "True"), boolExtendedLoggingForScheduledTasks) Then
                                    boolExtendedLoggingForScheduledTasks = boolExtendedLoggingForScheduledTasks
                                Else
                                    boolExtendedLoggingForScheduledTasks = True
                                End If

                                registryKey.Close()
                                registryKey.Dispose()
                                registryKey = Nothing
                            End If

                            If boolExtendedLoggingForScheduledTasks = True Then
                                Functions.eventLogFunctions.writeToSystemEventLog(String.Format("Starting scheduled restore point job. Task running as user {0}. There are currently {1} system restore point(s) on this system.", Environment.UserName, Functions.wmi.getNumberOfRestorePoints()), EventLogEntryType.Information)
                            Else
                                Functions.eventLogFunctions.writeToSystemEventLog(String.Format("Starting scheduled restore point job. Task running as user {0}.", Environment.UserName), EventLogEntryType.Information)
                            End If

                            Functions.startupFunctions.writeLastRunFile()

                            If boolExtendedLoggingForScheduledTasks = True Then oldNewestRestorePointID = Functions.wmi.getNewestSystemRestorePointID()

                            Functions.support.createScheduledSystemRestorePoint(restorePointNameForScheduledTasks)

                            If boolExtendedLoggingForScheduledTasks = True Then
                                ' We wait here with this loop until the system's has the restore point created.
                                While oldNewestRestorePointID = Functions.wmi.getNewestSystemRestorePointID()
                                    ' Does nothing, just loops and sleeps for half a second.
                                    Threading.Thread.Sleep(500)
                                End While
                            End If

                            If boolExtendedLoggingForScheduledTasks = True Then Functions.support.writeSystemRestorePointsToApplicationLogs()

                            Dim boolValueFromRegistryAsString As String = Registry.LocalMachine.OpenSubKey(globalVariables.registryValues.strKey, False).GetValue("Delete Old Restore Points", "False").Trim
                            Dim boolValueFromRegistry As Boolean

                            ' This checks if we have valid data from the Registry value.  It attempts to parse it and passes the value of the parses data to boolValueFromRegistry.
                            If Boolean.TryParse(boolValueFromRegistryAsString, boolValueFromRegistry) = True Then
                                ' OK, we do have valid data, let's continue.
                                If boolValueFromRegistryAsString = True Then
                                    Functions.startupFunctions.deleteOldRestorePoints()
                                End If
                            End If

                            If globalVariables.KeepXAmountOfRestorePoints = True Then
                                Functions.wmi.doDeletingOfXNumberOfRestorePoints(globalVariables.KeepXAmountofRestorePointsValue)
                            End If

                            Functions.eventLogFunctions.writeToSystemEventLog("Scheduled restore point job complete.", EventLogEntryType.Information)

                            e.Cancel = True
                            Exit Sub
                        ElseIf commandLineArgument.stringCompare("-restoretopoint") Then
                            ' The first thing we do is disable Safe Mode boot so the user doesn't get trapped in Safe Mode.
                            Functions.registryStuff.removeSafeModeBoot(True)

                            ' Now let's delete that Registry setting that tells the program that a Safe Mode boot was set up.
                            If Registry.LocalMachine.OpenSubKey(globalVariables.registryValues.strKey) IsNot Nothing Then
                                Registry.LocalMachine.OpenSubKey(globalVariables.registryValues.strKey, True).DeleteValue("Safe Mode Boot Set", False)
                            End If

                            ' We try and parse the value in the Registry.
                            If Integer.TryParse(Registry.LocalMachine.OpenSubKey(globalVariables.registryValues.strKey, False).GetValue("Preselected Restore Point for Restore in Safe Mode", 0), Functions.startupFunctions.preSelectedRestorePointID) Then
                                ' We need to remove the registry keys from the registry, we no longer need them.
                                Registry.LocalMachine.OpenSubKey(globalVariables.registryValues.strKey, True).DeleteValue("Preselected Restore Point for Restore in Safe Mode", False)
                                Registry.LocalMachine.OpenSubKey("Software\Microsoft\Windows\CurrentVersion\RunOnce", True).DeleteValue("*Restore To Restore Point", False)

                                ' OK, what we have is an Interger. Great. Let's do some work with it.

                                If Functions.startupFunctions.preSelectedRestorePointID = 0 Then
                                    MsgBox("Something went wrong, we don't have a valid restore point to restore to. The program will execute as normal from now on.", MsgBoxStyle.Information, "Restore Point Creator")
                                Else
                                    Functions.wait.createPleaseWaitWindow("Beginning the Restore Process... Please Wait.", True, enums.howToCenterWindow.screen, True)

                                    Functions.startupFunctions.isMyRestoreThreadRunning = True

                                    Dim thread As New Threading.Thread(AddressOf Functions.startupFunctions.restoreSystemRestorePoint)
                                    thread.Name = "Restore System Restore Point Thread"
                                    thread.Priority = Threading.ThreadPriority.Normal
                                    thread.Start()

                                    While Functions.startupFunctions.isMyRestoreThreadRunning = True
                                        Threading.Thread.Sleep(1000)
                                    End While
                                End If
                            Else
                                ' We need to remove the registry keys from the registry, we no longer need them.
                                Registry.LocalMachine.OpenSubKey(globalVariables.registryValues.strKey, True).DeleteValue("Preselected Restore Point for Restore in Safe Mode", False)
                                Registry.LocalMachine.OpenSubKey("Software\Microsoft\Windows\CurrentVersion\RunOnce", True).DeleteValue("*Restore To Restore Point", False)

                                ' Nope, we don't have an Integer. Let's stop things right here.
                                MsgBox("Something went wrong, we don't have a valid restore point to restore to. The program will execute as normal from now on.", MsgBoxStyle.Information, "Restore Point Creator")
                            End If

                            e.Cancel = True
                            Exit Sub
                        ElseIf commandLineArgument.stringCompare("-deleteoldrestorepoints") Then
                            Functions.startupFunctions.deleteOldRestorePoints()
                            e.Cancel = True
                            Exit Sub
                        ElseIf commandLineArgument.stringCompare("-prefscleanup") Then
                            Functions.startupFunctions.prefsCleanup()
                        End If
                    Else
                        Functions.registryStuff.removeSafeModeBoot()
                    End If
                Else
                    Functions.registryStuff.removeSafeModeBoot()
                End If

                If (Functions.osVersionInfo.isThisWindows10() = True Or Functions.osVersionInfo.isThisWindows8x() = True) And boolAreWeAnAdministrator = True And Functions.support.areWeInSafeMode() = False Then
                    Functions.taskStuff.disableBuiltInRestorePointTask()
                End If
            Catch ex As Exception
                Threading.Thread.CurrentThread.CurrentUICulture = New Globalization.CultureInfo("en-US")
                exceptionHandler.manuallyLoadCrashWindow(ex, "Application Startup Routine" & vbCrLf & vbCrLf & ex.Message, ex.StackTrace, ex.GetType)
            End Try

            If boolAreWeAnAdministrator = True Then
                Functions.taskStuff.updateScheduledRestorePointCreationTaskWithEverySetting()
                Functions.taskStuff.setMultiRunForTask()
            End If
        End Sub

        Private Sub MyApplication_UnhandledException(sender As Object, e As ApplicationServices.UnhandledExceptionEventArgs) Handles Me.UnhandledException
            Threading.Thread.CurrentThread.CurrentUICulture = New Globalization.CultureInfo("en-US")

            Dim result As Boolean = exceptionHandler.handleCrashWithAnErrorOrRedirectUserInstead(e.Exception)
            If result = True Then exceptionHandler.manuallyLoadCrashWindow(e.Exception, e.Exception.Message, e.Exception.StackTrace, e.Exception.GetType)
        End Sub

        Protected Overrides Sub Finalize()
            MyBase.Finalize()
        End Sub

        Private Sub MyApplication_Shutdown(sender As Object, e As EventArgs) Handles Me.Shutdown
            If IO.Directory.Exists(globalVariables.shadowCopyMountFolder) Then IO.Directory.Delete(globalVariables.shadowCopyMountFolder)
        End Sub
    End Class
End Namespace