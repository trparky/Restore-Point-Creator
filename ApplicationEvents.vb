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

        Private Sub setProcessPriorities()
            If Functions.privilegeChecks.areWeAnAdministrator() AndAlso areWeRunningAsATask() Then
                Try
                    Process.GetCurrentProcess.PriorityClass = ProcessPriorityClass.Normal
                Catch ex As Exception
                End Try

                Try
                    Functions.IOPriority.SetIOPriority(Functions.IOPriority.IOPrioritySetting.Normal)
                Catch ex As Exception
                End Try
            End If
        End Sub

        Private Function areWeRunningAsATask() As Boolean
            If boolAreWeInUpdateOrNoParentProcessCheckMode() Then Return False

            Try
                Dim strParentProcess As String = Process.GetCurrentProcess.Parent.ProcessName
                Return If(strParentProcess.caseInsensitiveContains("svchost") Or strParentProcess.caseInsensitiveContains("taskeng"), True, False)
            Catch ex2 As Functions.myExceptions.unableToGetParentProcessException
                If My.Settings.debug Then Functions.eventLogFunctions.writeCrashToApplicationLogFile(ex2, EventLogEntryType.Warning)
                Return False
            Catch ex3 As Functions.myExceptions.integerTryParseException
                If My.Settings.debug Then Functions.eventLogFunctions.writeCrashToApplicationLogFile(ex3, EventLogEntryType.Warning)
                Return False
            Catch ex As Exception
                Return False
            End Try
        End Function

        Private Function boolAreWeInUpdateOrNoParentProcessCheckMode() As Boolean
            If My.Application.CommandLineArgs.Count >= 1 Then
                Dim commandLineArgument As String = My.Application.CommandLineArgs(0)
                Return If(commandLineArgument.Equals("-update", StringComparison.OrdinalIgnoreCase) Or commandLineArgument.Equals("-updatewithoutuninstallinfoupdate", StringComparison.OrdinalIgnoreCase) Or commandLineArgument.Equals("-noparentprocesscheck", StringComparison.OrdinalIgnoreCase), True, False)
            Else
                Return False
            End If
        End Function

        Private Sub MyApplication_Startup(sender As Object, e As ApplicationServices.StartupEventArgs) Handles Me.Startup
            If Functions.osVersionInfo.isThisWindowsXP() Then
                MsgBox("System Restore Point Creator does not support Windows XP. This program will now terminate.", MsgBoxStyle.Critical, "System Restore Point Creator")
                Process.GetCurrentProcess.Kill()
            End If

            Functions.startupFunctions.validateSettings()
            If Not globalVariables.version.boolDebugBuild Then My.Settings.debug = False
            setProcessPriorities()
            exceptionHandler.loadExceptionHandler()

            Dim commandLineArgument As String
            Dim registryKey As RegistryKey
            Dim boolNoTask As Boolean = False ' Create a Boolean data type variable.

            If Functions.osVersionInfo.isThisAServerOS() = True Then
                MsgBox("You are running a Server edition of Microsoft Windows. System Restore Point Creator doesn't function on server operating systems." & vbCrLf & vbCrLf & "This application will now close.", MsgBoxStyle.Critical, "System Restore Point Creator -- Application Error")
                Process.GetCurrentProcess.Kill()
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

            If boolAreWeAnAdministrator Then
                ' First we need to make sure that there are no other instances of this program running since we don't want to
                ' delete the lock files while there's another instance that's possibly working on the log file. This is to
                ' make sure that the log files aren't clobbered by multiple instances trying to write to the log file.
                If Not Functions.startupFunctions.isThereOtherInstancesOfMeRunning() Then
                    ' This is needed to make sure that no residual log lock files exist when starting the program.
                    Functions.support.deleteFileWithNoException(Functions.eventLogFunctions.strLogFile & ".temp")
                    Functions.support.deleteFileWithNoException(Functions.eventLogFunctions.strCorruptedLockFile)
                End If
            End If

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
                                    Process.GetCurrentProcess.Kill()
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
                                Functions.startupFunctions.giveNoCountGivenError() ' OK, it is so we give the user an error message.
                            Else
                                ' OK, the value we got from the Registry isn't null so we try to parse it now.
                                If Short.TryParse(stringlocal_KeepXAmountofRestorePointsValue, local_KeepXAmountofRestorePointsValue) Then
                                    ' OK, we were able to parse it so we save it to the application's settings to use it in the relaunch.
                                    My.Settings.deleteOldRestorePointCommandLineCount = local_KeepXAmountofRestorePointsValue
                                    My.Settings.Save()
                                Else
                                    Functions.startupFunctions.giveNoCountGivenError() ' We tried to parse it and we failed so we give the user an error message.
                                End If
                            End If
                        Else
                            ' OK, the registry key doesn't exist so we give the user an error message.
                            Functions.startupFunctions.giveNoCountGivenError()
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
                                Process.GetCurrentProcess.Kill()
                            End If
                        Else
                            ' No, the second command line argument isn't a "-count" like we are expecting so give the user an error message.
                            Functions.startupFunctions.giveNoCountGivenError()
                        End If
                    End If

                    Functions.taskStuff.runProgramUsingTaskWrapper()
                End If
            End If

            ' This code checks to see if the current version is a beta or Release Candidate and if the user's update channel is already set to beta mode.
            ' If the user's update channel isn't set to beta mode we then set it for the user here.
            If (globalVariables.version.boolBeta Or globalVariables.version.boolReleaseCandidate) And Not My.Settings.updateChannel.Equals(globalVariables.updateChannels.beta, StringComparison.OrdinalIgnoreCase) Then
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
                            Functions.eventLogFunctions.writeToApplicationLogFile("The program was called with an obsolete command line argument, specifically ""-createtasks"". The program has ignored the command and exited.", EventLogEntryType.Information, False)
                            Process.GetCurrentProcess.Kill()
                        ElseIf commandLineArgument.Equals("-fixruntimetasks", StringComparison.OrdinalIgnoreCase) Then
                            Functions.startupFunctions.repairRuntimeTasks()
                            Process.GetCurrentProcess.Kill()
                        ElseIf commandLineArgument.Equals("-deletealltasks", StringComparison.OrdinalIgnoreCase) Then
                            Functions.startupFunctions.deleteAllTasks()
                            Process.GetCurrentProcess.Kill()
                        End If
                    End If

                    Functions.taskStuff.runProgramUsingTaskWrapper()
                Else
                    Functions.support.reRunWithAdminUserRights()
                End If
            End If

            Try
                ' Ordinarily on Windows Vista and newer this code should not be needed but we have it in there to check if we do indeed have Administrator user rights.
                If Not boolAreWeAnAdministrator Then Functions.support.reRunWithAdminUserRights()

                registryKey = Registry.LocalMachine.OpenSubKey(globalVariables.registryValues.strKey, False)

                If registryKey IsNot Nothing Then
                    globalVariables.KeepXAmountOfRestorePoints = Functions.registryStuff.getBooleanValueFromRegistry(registryKey, "Keep X Amount of Restore Points", False)

                    If globalVariables.KeepXAmountOfRestorePoints Then
                        Short.TryParse(registryKey.GetValue("Keep X Amount of Restore Points Value", "-10"), globalVariables.KeepXAmountofRestorePointsValue)
                    End If

                    globalVariables.boolLogLoadsAndExits = Functions.registryStuff.getBooleanValueFromRegistry(registryKey, "Log Program Loads and Exits to Event Log", True)

                    registryKey.Close()
                    registryKey.Dispose()
                    registryKey = Nothing
                End If

                If My.Settings.UpdateRequired Then
                    My.Settings.Upgrade()
                    My.Settings.UpdateRequired = False
                End If

                If My.Application.CommandLineArgs IsNot Nothing Then
                    If My.Application.CommandLineArgs.Count >= 1 Then
                        commandLineArgument = My.Application.CommandLineArgs(0)

                        If commandLineArgument.Equals(globalVariables.commandLineSwitches.createRestorePoint, StringComparison.OrdinalIgnoreCase) Then
                            Functions.startupFunctions.doCreateRestorePointRoutine(boolAreWeInSafeMode)
                            e.Cancel = True
                            Exit Sub
                        ElseIf commandLineArgument.Equals(globalVariables.commandLineSwitches.createCustomRestorePoint, StringComparison.OrdinalIgnoreCase) Then
                            Functions.startupFunctions.doCreateCustomRestorePointRoutine(boolAreWeInSafeMode)
                            e.Cancel = True
                            Exit Sub
                        ElseIf commandLineArgument.Equals(globalVariables.commandLineSwitches.testlogwrite, StringComparison.OrdinalIgnoreCase) Then
                            Functions.eventLogFunctions.writeToApplicationLogFile("Written to application log file successfully.", EventLogEntryType.Information, False)
                            e.Cancel = True
                            Exit Sub
                        ElseIf commandLineArgument.Equals(globalVariables.commandLineSwitches.scheduledRestorePoint, StringComparison.OrdinalIgnoreCase) Then
                            Functions.startupFunctions.doScheduledRestorePointRoutine(registryKey, boolAreWeAnAdministrator)
                            e.Cancel = True
                            Exit Sub
                        ElseIf commandLineArgument.Equals("-restoretopoint", StringComparison.OrdinalIgnoreCase) Then
                            Functions.startupFunctions.doRestoreToPointRoutine()
                            e.Cancel = True
                            Exit Sub
                        ElseIf commandLineArgument.Equals(globalVariables.commandLineSwitches.deleteOldRestorePoints, StringComparison.OrdinalIgnoreCase) Then
                            Functions.startupFunctions.deleteOldRestorePoints()
                            e.Cancel = True
                            Exit Sub
                        ElseIf commandLineArgument.Equals(globalVariables.commandLineSwitches.keepXNumberOfRestorePoints, StringComparison.OrdinalIgnoreCase) Then
                            Functions.startupFunctions.doKeepXNumberOfRestorePointsRoutine(registryKey)
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
                If Not Functions.registryStuff.getBooleanValueFromRegistry("Updated Scheduled Tasks with Every Setting", False) Then Functions.taskStuff.updateScheduledRestorePointCreationTaskWithEverySetting()
                If Not Functions.registryStuff.getBooleanValueFromRegistry("Added MultiRun For Runtime Task", False) Then Functions.taskStuff.setMultiRunForTask()
                If Not Functions.registryStuff.getBooleanValueFromRegistry("Added Priority Settings to Tasks", False) Then Functions.taskStuff.addPrioritySettings()
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

            Try
                Functions.eventLogFunctions.myLogFileLockingMutex.ReleaseMutex()
            Catch ex As Exception
            End Try
        End Sub
    End Class
End Namespace