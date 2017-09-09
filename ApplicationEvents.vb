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

        Private Sub giveNoCountGivenError()
            Console.WriteLine("ERROR: You must include a ""-count=(0-9)"" to your invokation of this program with this command line argument. For instance... ""-count=9"".")
            Windows.Forms.Application.Exit()
        End Sub

        Private Sub setProcessPriorities()
            If Functions.privilegeChecks.areWeAnAdministrator() Then
                If areWeRunningAsATask() Then
                    Try
                        Process.GetCurrentProcess.PriorityClass = ProcessPriorityClass.Normal
                    Catch ex As Exception
                    End Try

                    Try
                        Functions.IOPriority.SetIOPriority(Functions.IOPriority.IOPrioritySetting.Normal)
                    Catch ex As Exception
                    End Try
                End If
            End If
        End Sub

        Private Function areWeRunningAsATask() As Boolean
            Try
                Return Process.GetCurrentProcess.Parent.ProcessName.caseInsensitiveContains("svchost")
            Catch ex As Exception
                Return False
            End Try
        End Function

        Private Sub MyApplication_Startup(sender As Object, e As ApplicationServices.StartupEventArgs) Handles Me.Startup
            Functions.eventLogFunctions.getOldLogsFromWindowsEventLog()
            setProcessPriorities()
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
                Windows.Forms.Application.Exit()
            End If

            If IO.File.Exists("portable.mode") = True Or IO.File.Exists("portablemode.txt") = True Then
                globalVariables.boolPortableMode = True
                boolNoTask = True
            End If

            If Debugger.IsAttached Then
                Debug.WriteLine("debugger is attached")
                ' This is to test the crash submission code. Make sure this block is commented out before compiling for public release.
                'Try
                '    Throw New IO.FileLoadException()
                'Catch ex As Exception
                '    exceptionHandler.manuallyLoadCrashWindow(ex)
                'End Try
                ' This is to test the crash submission code. Make sure this block is commented out before compiling for public release.
            End If

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

                If commandLineArgument.Equals(globalVariables.commandLineSwitches.viewChangeLog, StringComparison.OrdinalIgnoreCase) Then
                    Dim changeLog As New Change_Log
                    changeLog.ShowDialog()
                    e.Cancel = True
                    Exit Sub
                ElseIf commandLineArgument.Equals(globalVariables.commandLineSwitches.viewEventLog, StringComparison.OrdinalIgnoreCase) Then
                    Dim eventLogForm As New eventLogForm
                    eventLogForm.ShowDialog()
                    e.Cancel = True
                    Exit Sub
                ElseIf commandLineArgument.Equals(globalVariables.commandLineSwitches.createRestorePoint, StringComparison.OrdinalIgnoreCase) Then
                    If My.Application.CommandLineArgs.Count = 2 Then
                        If My.Application.CommandLineArgs(1).Trim.StartsWith("-name", StringComparison.OrdinalIgnoreCase) Then
                            My.Settings.savedRestorePointFromCommandLine = My.Application.CommandLineArgs(1).Trim.caseInsensitiveReplace("-name=", "")
                            My.Settings.Save()
                        End If
                    End If

                    Functions.taskStuff.runProgramUsingTaskWrapper()
                ElseIf commandLineArgument.Equals(globalVariables.commandLineSwitches.deleteOldRestorePoints, StringComparison.OrdinalIgnoreCase) Then
                    If Not Functions.privilegeChecks.areWeAnAdministrator() Then
                        If My.Application.CommandLineArgs.Count = 2 Then
                            ' OK, the user provided a second command line argument so let's check it out.
                            If My.Application.CommandLineArgs(1).Trim.StartsWith("-maxdays", StringComparison.OrdinalIgnoreCase) Then
                                ' Let's try and parse the value that the user gave us. If it parses then what's inside the IF statement will not execute and this is just fine; things are OK so we can continue as normal.
                                If Not Short.TryParse(My.Application.CommandLineArgs(1).Trim.caseInsensitiveReplace("-maxdays=", "").Trim, My.Settings.maxDaysAtRelaunch) Then
                                    ' We tried to parse it and we failed so we give the user an error message.
                                    Console.WriteLine("ERROR: You have provided an invalid numeric input, please try again.")
                                    Windows.Forms.Application.Exit()
                                End If

                                My.Settings.Save()
                            End If
                        End If

                        Functions.taskStuff.runProgramUsingTaskWrapper()
                        e.Cancel = True
                        Exit Sub
                    End If
                ElseIf commandLineArgument.Equals(globalVariables.commandLineSwitches.keepXNumberOfRestorePoints, StringComparison.OrdinalIgnoreCase) And Not boolAreWeAnAdministrator Then
                    ' Let's put the setting back to the default value of 0.
                    My.Settings.deleteOldRestorePointCommandLineCount = 0
                    My.Settings.Save()

                    Dim local_KeepXAmountofRestorePointsValue As Short

                    ' This checks if the user provided a "-count" argument.
                    If My.Application.CommandLineArgs.Count <> 2 Then
                        ' No, the user didn't so let's do some stuff in the Registry. We open the program's Registry Key here.
                        registryKey = Registry.LocalMachine.OpenSubKey(globalVariables.registryValues.strKey, False)

                        ' This checks to see if the Registry Key exists by doing a Null check.
                        If registryKey IsNot Nothing Then
                            ' The key does exist so let's pull our value from the Registry.
                            Dim stringlocal_KeepXAmountofRestorePointsValue As String = registryKey.GetValue("Keep X Amount of Restore Points Value", "")

                            ' And let's close the Registry Object.
                            registryKey.Close()
                            registryKey.Dispose()

                            ' OK, let's check to see if the value we pulled from the Registry is null.
                            If String.IsNullOrEmpty(stringlocal_KeepXAmountofRestorePointsValue) Then
                                giveNoCountGivenError() ' OK, it is so we give the user an error message.
                            Else
                                ' OK, the value we got from the Registry isn't null so we try to parse it now.
                                If Short.TryParse(stringlocal_KeepXAmountofRestorePointsValue, local_KeepXAmountofRestorePointsValue) Then
                                    ' OK, we were able to parse it so we save it to the application's settings to use it in the relaunch.
                                    My.Settings.deleteOldRestorePointCommandLineCount = local_KeepXAmountofRestorePointsValue
                                    My.Settings.Save()
                                Else
                                    giveNoCountGivenError() ' We tried to parse it and we failed so we give the user an error message.
                                End If
                            End If
                        Else
                            ' OK, the registry key doesn't exist so we give the user an error message.
                            giveNoCountGivenError()
                        End If
                    Else
                        ' OK, the user provided a second command line argument so let's check it out.
                        If My.Application.CommandLineArgs(1).Trim.StartsWith("-count", StringComparison.OrdinalIgnoreCase) Then
                            Dim deleteOldRestorePointCommandLineCount As Short

                            ' Let's try and parse the value that the user gave us.
                            If Short.TryParse(My.Application.CommandLineArgs(1).Trim.caseInsensitiveReplace("-count=", "").Trim, deleteOldRestorePointCommandLineCount) Then
                                ' Good, we could parse it so let's save it in the application's settings to use it in the relaunch.
                                My.Settings.deleteOldRestorePointCommandLineCount = deleteOldRestorePointCommandLineCount
                                My.Settings.Save()
                            Else
                                Console.WriteLine("ERROR: You have provided an invalid numeric input, please try again.")
                                Windows.Forms.Application.Exit()
                            End If
                        Else
                            ' No, the second command line argument isn't a "-count" like we are expecting so give the user an error message.
                            giveNoCountGivenError()
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
            boolNoTask = Functions.registryStuff.getBooleanValueFromRegistry(Registry.LocalMachine.OpenSubKey(globalVariables.registryValues.strKey), "No Task", False)

            If boolNoTask And Not boolAreWeAnAdministrator Then
                Functions.support.reRunWithAdminUserRights() ' OK, we relaunch the process with full Administrator privileges with a UAC prompt.
            End If

            If Not boolAreWeInSafeMode And boolAreWeAnAdministrator And My.Application.CommandLineArgs.Count > 0 Then
                commandLineArgument = My.Application.CommandLineArgs(0)

                If commandLineArgument.Equals("-update", StringComparison.OrdinalIgnoreCase) Or commandLineArgument.Equals("-updatewithoutuninstallinfoupdate", StringComparison.OrdinalIgnoreCase) Then
                    Functions.startupFunctions.performApplicationUpdate(commandLineArgument)
                    e.Cancel = True
                    Exit Sub
                End If
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

                        If commandLineArgument.Equals("-createtasks", StringComparison.OrdinalIgnoreCase) Then
                            Functions.eventLogFunctions.writeToSystemEventLog("The program was called with an obsolete command line argument, specifically ""-createtasks"". The program has ignored the command and exited.", EventLogEntryType.Information)
                            Windows.Forms.Application.Exit()
                        ElseIf commandLineArgument.Equals("-fixruntimetasks", StringComparison.OrdinalIgnoreCase) Then
                            Functions.startupFunctions.repairRuntimeTasks()

                            Functions.eventLogFunctions.saveLogFileToDisk()
                            e.Cancel = True
                            Exit Sub
                        ElseIf commandLineArgument.Equals("-deletealltasks", StringComparison.OrdinalIgnoreCase) Then
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
                    globalVariables.KeepXAmountOfRestorePoints = Functions.registryStuff.getBooleanValueFromRegistry(registryKey, "Keep X Amount of Restore Points", False)

                    If globalVariables.KeepXAmountOfRestorePoints = True Then
                        Short.TryParse(registryKey.GetValue("Keep X Amount of Restore Points Value", "-10"), globalVariables.KeepXAmountofRestorePointsValue)
                    End If

                    globalVariables.boolLogToSystemLog = Functions.registryStuff.getBooleanValueFromRegistry(registryKey, "Enable System Logging", True)
                    globalVariables.boolLogLoadsAndExits = Functions.registryStuff.getBooleanValueFromRegistry(registryKey, "Log Program Loads and Exits to Event Log", True)

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

                        If commandLineArgument.Equals(globalVariables.commandLineSwitches.createRestorePoint, StringComparison.OrdinalIgnoreCase) Then
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
                        ElseIf commandLineArgument.Equals(globalVariables.commandLineSwitches.createCustomRestorePoint, StringComparison.OrdinalIgnoreCase) Then
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
                        ElseIf commandLineArgument.Equals(globalVariables.commandLineSwitches.scheduledRestorePoint, StringComparison.OrdinalIgnoreCase) Then
                            Dim restorePointNameForScheduledTasks As String = globalVariables.strDefaultNameForScheduledTasks
                            Dim boolExtendedLoggingForScheduledTasks As Boolean = True
                            Dim oldNewestRestorePointID As Integer

                            registryKey = Registry.LocalMachine.OpenSubKey(globalVariables.registryValues.strKey, False)

                            If registryKey IsNot Nothing Then
                                restorePointNameForScheduledTasks = registryKey.GetValue("Custom Name for Scheduled Restore Points", globalVariables.strDefaultNameForScheduledTasks)

                                boolExtendedLoggingForScheduledTasks = Functions.registryStuff.getBooleanValueFromRegistry(registryKey, "Extended Logging For Scheduled Tasks", True)

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

                            Functions.restorePointStuff.createScheduledSystemRestorePoint(restorePointNameForScheduledTasks)

                            If boolExtendedLoggingForScheduledTasks = True Then
                                ' We wait here with this loop until the system's has the restore point created.
                                While oldNewestRestorePointID = Functions.wmi.getNewestSystemRestorePointID()
                                    ' Does nothing, just loops and sleeps for half a second.
                                    Threading.Thread.Sleep(500)
                                End While
                            End If

                            If boolExtendedLoggingForScheduledTasks = True Then Functions.restorePointStuff.writeSystemRestorePointsToApplicationLogs()

                            If Functions.registryStuff.getBooleanValueFromRegistry(Registry.LocalMachine.OpenSubKey(globalVariables.registryValues.strKey, False), "Delete Old Restore Points", False) Then
                                Functions.startupFunctions.deleteOldRestorePoints()
                            End If

                            If globalVariables.KeepXAmountOfRestorePoints = True Then
                                Functions.wmi.doDeletingOfXNumberOfRestorePoints(globalVariables.KeepXAmountofRestorePointsValue)
                            End If

                            Functions.eventLogFunctions.writeToSystemEventLog("Scheduled restore point job complete.", EventLogEntryType.Information)

                            e.Cancel = True
                            Exit Sub
                        ElseIf commandLineArgument.Equals("-restoretopoint", StringComparison.OrdinalIgnoreCase) Then
                            ' The first thing we do is disable Safe Mode boot so the user doesn't get trapped in Safe Mode.
                            Functions.support.removeSafeModeBoot()

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
                        ElseIf commandLineArgument.Equals(globalVariables.commandLineSwitches.deleteOldRestorePoints, StringComparison.OrdinalIgnoreCase) Then
                            Functions.startupFunctions.deleteOldRestorePoints()
                            e.Cancel = True
                            Exit Sub
                        ElseIf commandLineArgument.Equals(globalVariables.commandLineSwitches.keepXNumberOfRestorePoints, StringComparison.OrdinalIgnoreCase) Then
                            Dim deleteOldRestorePointCommandLineCount As Short

                            ' This checks if the user provided a "-count" argument.
                            If My.Application.CommandLineArgs.Count <> 2 Then
                                ' No, the user didn't provide a "-count" command line argument.

                                ' First, let's see if the user provided a setting in an un-elevated execution of this program.
                                If My.Settings.deleteOldRestorePointCommandLineCount = 0 Then
                                    ' The user didn't so let's see if there's a setting in the Registry. We open the program's Registry Key here.
                                    registryKey = Registry.LocalMachine.OpenSubKey(globalVariables.registryValues.strKey, False)

                                    ' This checks to see if the Registry Key exists by doing a Null check.
                                    If registryKey IsNot Nothing Then
                                        ' The key does exist so let's pull our value from the Registry.
                                        Dim stringlocal_KeepXAmountofRestorePointsValue As String = registryKey.GetValue("Keep X Amount of Restore Points Value", "")

                                        ' And let's close the Registry Object.
                                        registryKey.Close()
                                        registryKey.Dispose()

                                        ' OK, let's check to see if the value we pulled from the Registry is null.
                                        If String.IsNullOrEmpty(stringlocal_KeepXAmountofRestorePointsValue) Then
                                            giveNoCountGivenError() ' OK, it is so we give the user an error message.
                                        Else
                                            ' OK, the value we got from the Registry isn't null so we try to parse it. If it parses then what's inside the IF statement will not execute and this is just fine; things are OK so we can continue as normal.
                                            If Not Short.TryParse(stringlocal_KeepXAmountofRestorePointsValue, deleteOldRestorePointCommandLineCount) Then
                                                giveNoCountGivenError() ' We tried to parse it and we failed so we give the user an error message.
                                            End If
                                        End If
                                    Else
                                        ' OK, the registry key doesn't exist so we give the user an error message.
                                        giveNoCountGivenError()
                                    End If
                                Else
                                    ' Yes, the user did so let's work with it.
                                    deleteOldRestorePointCommandLineCount = My.Settings.deleteOldRestorePointCommandLineCount

                                    ' Now let's reset the saved setting back to the default value of 0.
                                    My.Settings.deleteOldRestorePointCommandLineCount = 0
                                    My.Settings.Save()
                                End If
                            Else
                                ' OK, the user provided a second command line argument so let's check it out.
                                If My.Application.CommandLineArgs(1).Trim.StartsWith("-count", StringComparison.OrdinalIgnoreCase) Then
                                    ' Let's try and parse the value that the user gave us. If it parses then what's inside the IF statement will not execute and this is just fine; things are OK so we can continue as normal.
                                    If Not Short.TryParse(My.Application.CommandLineArgs(1).Trim.caseInsensitiveReplace("-count=", "").Trim, deleteOldRestorePointCommandLineCount) Then
                                        ' We tried to parse it and we failed so we give the user an error message.
                                        Console.WriteLine("ERROR: You have provided an invalid numeric input, please try again.")
                                        Windows.Forms.Application.Exit()
                                    End If
                                Else
                                    ' No, the second command line argument isn't a "-count" like we are expecting so give the user an error message.
                                    giveNoCountGivenError()
                                End If
                            End If

                            ' If all things passed the checks above this code will now execute.

                            If deleteOldRestorePointCommandLineCount <> 0 Then
                                Dim numberOfRestorePoints As Integer = Functions.wmi.getNumberOfRestorePoints()

                                If deleteOldRestorePointCommandLineCount < numberOfRestorePoints Then
                                    Functions.restorePointStuff.writeSystemRestorePointsToApplicationLogs()

                                    Functions.wmi.doDeletingOfXNumberOfRestorePoints(deleteOldRestorePointCommandLineCount)

                                    While numberOfRestorePoints = Functions.wmi.getNumberOfRestorePoints()
                                        Threading.Thread.Sleep(500)
                                    End While

                                    Functions.restorePointStuff.writeSystemRestorePointsToApplicationLogs()
                                End If
                            End If

                            My.Settings.deleteOldRestorePointCommandLineCount = 0
                            My.Settings.Save()

                            e.Cancel = True
                            Exit Sub
                        ElseIf commandLineArgument.Equals("-prefscleanup", StringComparison.OrdinalIgnoreCase) Then
                            Functions.startupFunctions.prefsCleanup()
                        End If
                    Else
                        Functions.support.removeSafeModeBoot()
                    End If
                Else
                    Functions.support.removeSafeModeBoot()
                End If

                If (Functions.osVersionInfo.isThisWindows10() = True Or Functions.osVersionInfo.isThisWindows8x() = True) And boolAreWeAnAdministrator = True And boolAreWeInSafeMode = False Then
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
            Functions.eventLogFunctions.saveLogFileToDisk()
        End Sub
    End Class
End Namespace