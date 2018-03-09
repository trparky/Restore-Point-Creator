Imports Microsoft.Win32.TaskScheduler
Imports System.IO
Imports Microsoft.Win32
Imports System.Management
Imports System.Configuration

Namespace Functions.startupFunctions
    Module startupFunctions
        Public isMyRestoreThreadRunning As Boolean = False
        Public preSelectedRestorePointID As Integer

        Public Sub giveNoCountGivenError()
            Console.WriteLine("ERROR: You must include a ""-count=(0-9)"" to your invokation of this program with this command line argument. For instance... ""-count=9"".")
            Process.GetCurrentProcess.Kill()
        End Sub

        Public Sub doRestoreToPointRoutine()
            ' The first thing we do is disable Safe Mode boot so the user doesn't get trapped in Safe Mode.
            support.removeSafeModeBoot()

            ' Now let's delete that Registry setting that tells the program that a Safe Mode boot was set up.
            If Registry.LocalMachine.OpenSubKey(globalVariables.registryValues.strKey) IsNot Nothing Then
                Registry.LocalMachine.OpenSubKey(globalVariables.registryValues.strKey, True).DeleteValue("Safe Mode Boot Set", False)
            End If

            ' We try and parse the value in the Registry.
            If Integer.TryParse(Registry.LocalMachine.OpenSubKey(globalVariables.registryValues.strKey, False).GetValue("Preselected Restore Point For Restore In Safe Mode", 0), Functions.startupFunctions.preSelectedRestorePointID) Then
                ' We need to remove the registry keys from the registry, we no longer need them.
                Registry.LocalMachine.OpenSubKey(globalVariables.registryValues.strKey, True).DeleteValue("Preselected Restore Point For Restore In Safe Mode", False)
                Registry.LocalMachine.OpenSubKey("Software\Microsoft\Windows\CurrentVersion\RunOnce", True).DeleteValue("*Restore To Restore Point", False)

                ' OK, what we have is an Interger. Great. Let's do some work with it.

                If preSelectedRestorePointID = 0 Then
                    MsgBox("Something went wrong, we don't have a valid restore point to restore to. The program will execute as normal from now on.", MsgBoxStyle.Information, "Restore Point Creator")
                Else
                    wait.createPleaseWaitWindow("Beginning the Restore Process... Please Wait.", True, enums.howToCenterWindow.screen, True)

                    isMyRestoreThreadRunning = True

                    Dim thread As New Threading.Thread(AddressOf restoreSystemRestorePoint)
                    thread.Name = "Restore System Restore Point Thread"
                    thread.Priority = Threading.ThreadPriority.Normal
                    thread.Start()

                    While isMyRestoreThreadRunning
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
        End Sub

        Public Sub doKeepXNumberOfRestorePointsRoutine()
            Dim deleteOldRestorePointCommandLineCount As Short

            ' This checks if the user provided a "-count" argument.
            If My.Application.CommandLineArgs.Count <> 2 Then
                ' No, the user didn't provide a "-count" command line argument.

                ' First, let's see if the user provided a setting in an un-elevated execution of this program.
                If My.Settings.deleteOldRestorePointCommandLineCount = 0 Then
                    Using registryKey As RegistryKey = Registry.LocalMachine.OpenSubKey(globalVariables.registryValues.strKey, False)
                        ' This checks to see if the Registry Key exists by doing a Null check.
                        If registryKey IsNot Nothing Then
                            ' The key does exist so let's pull our value from the Registry.
                            Dim stringlocal_KeepXAmountofRestorePointsValue As String = registryKey.GetValue("Keep X Amount of Restore Points Value", "")

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
                    End Using
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
                        Process.GetCurrentProcess.Kill()
                    End If
                Else
                    ' No, the second command line argument isn't a "-count" like we are expecting so give the user an error message.
                    giveNoCountGivenError()
                End If
            End If

            ' If all things passed the checks above this code will now execute.

            If deleteOldRestorePointCommandLineCount <> 0 Then
                Dim numberOfRestorePoints As Integer = wmi.getNumberOfRestorePoints()

                If deleteOldRestorePointCommandLineCount < numberOfRestorePoints AndAlso eventLogFunctions.myLogFileLockingMutex.WaitOne(500) Then
                    eventLogFunctions.strMutexAcquiredWhere = "Mutex acquired in doKeepXNumberOfRestorePointsRoutine()."
                    restorePointStuff.writeSystemRestorePointsToApplicationLogs()

                    wmi.doDeletingOfXNumberOfRestorePoints(deleteOldRestorePointCommandLineCount)

                    While numberOfRestorePoints = wmi.getNumberOfRestorePoints()
                        Threading.Thread.Sleep(500)
                    End While

                    restorePointStuff.writeSystemRestorePointsToApplicationLogs()
                    eventLogFunctions.writeToApplicationLogFile("Deletion of X number of restore points complete.", EventLogEntryType.Information, False, False)

                    eventLogFunctions.myLogFileLockingMutex.ReleaseMutex()
                    eventLogFunctions.strMutexAcquiredWhere = Nothing
                End If
            End If

            My.Settings.deleteOldRestorePointCommandLineCount = 0
            My.Settings.Save()
        End Sub

        Public Sub doScheduledRestorePointRoutine(ByVal boolAreWeAnAdministrator As Boolean)
            If eventLogFunctions.myLogFileLockingMutex.WaitOne(500) Then
                eventLogFunctions.strMutexAcquiredWhere = "Mutex acquired in doScheduledRestorePointRoutine()."

                Dim restorePointNameForScheduledTasks As String = globalVariables.strDefaultNameForScheduledTasks
                Dim boolExtendedLoggingForScheduledTasks As Boolean = True
                Dim boolWriteRestorePointListToLog As Boolean = True
                Dim oldNewestRestorePointID As Integer

                Using registryKey As RegistryKey = Registry.LocalMachine.OpenSubKey(globalVariables.registryValues.strKey, False)
                    If registryKey IsNot Nothing Then
                        restorePointNameForScheduledTasks = registryKey.GetValue("Custom Name for Scheduled Restore Points", globalVariables.strDefaultNameForScheduledTasks)

                        boolExtendedLoggingForScheduledTasks = registryStuff.getBooleanValueFromRegistry(registryKey, "Extended Logging For Scheduled Tasks", True)
                        boolWriteRestorePointListToLog = registryStuff.getBooleanValueFromRegistry(registryKey, globalVariables.strWriteRestorePointListToApplicationLogRegistryValue, True)
                    End If
                End Using

                If boolExtendedLoggingForScheduledTasks = True Then
                    eventLogFunctions.writeToApplicationLogFile(String.Format("Starting scheduled restore point job. Task running as user {0}. There are currently {1} system restore point(s) on this system.", Environment.UserName, wmi.getNumberOfRestorePoints()), EventLogEntryType.Information, False, False)
                Else
                    eventLogFunctions.writeToApplicationLogFile(String.Format("Starting scheduled restore point job. Task running As user {0}.", Environment.UserName), EventLogEntryType.Information, False, False)
                End If

                If boolAreWeAnAdministrator Then writeLastRunFile()

                If boolExtendedLoggingForScheduledTasks = True Then oldNewestRestorePointID = wmi.getNewestSystemRestorePointID()

                restorePointStuff.createScheduledSystemRestorePoint(restorePointNameForScheduledTasks)

                If boolExtendedLoggingForScheduledTasks = True Then
                    ' We wait here with this loop until the system's has the restore point created.
                    While oldNewestRestorePointID = wmi.getNewestSystemRestorePointID()
                        ' Does nothing, just loops and sleeps for half a second.
                        Threading.Thread.Sleep(500)
                    End While
                End If

                If boolExtendedLoggingForScheduledTasks And boolWriteRestorePointListToLog Then restorePointStuff.writeSystemRestorePointsToApplicationLogs()

                If registryStuff.getBooleanValueFromRegistry(Registry.LocalMachine.OpenSubKey(globalVariables.registryValues.strKey, False), "Delete Old Restore Points", False) Then
                    deleteOldRestorePoints()
                End If

                If globalVariables.KeepXAmountOfRestorePoints = True Then
                    wmi.doDeletingOfXNumberOfRestorePoints(globalVariables.KeepXAmountofRestorePointsValue)
                End If

                eventLogFunctions.writeToApplicationLogFile("Scheduled restore point job complete.", EventLogEntryType.Information, False, False)

                eventLogFunctions.myLogFileLockingMutex.ReleaseMutex()
                eventLogFunctions.strMutexAcquiredWhere = Nothing
            End If
        End Sub

        Public Sub doCreateCustomRestorePointRoutine(boolAreWeInSafeMode As Boolean)
            giveSafeModeErrorMessage(boolAreWeInSafeMode)
            eventLogFunctions.writeToApplicationLogFile("Activated JumpList Task.", EventLogEntryType.Information, False)

            Dim Custom_Named_Restore_Point_Instance As Custom_Named_Restore_Point
            Custom_Named_Restore_Point_Instance = New Custom_Named_Restore_Point
            Custom_Named_Restore_Point_Instance.StartPosition = FormStartPosition.CenterScreen
            Custom_Named_Restore_Point_Instance.ShowDialog()

            Dim restorePointName As String

            If Custom_Named_Restore_Point_Instance.createRestorePoint = False Then
                MsgBox("Restore Point not created.", MsgBoxStyle.Information, "Restore Point Creator") ' Gives the user some feedback.
                Exit Sub
            Else
                restorePointName = Custom_Named_Restore_Point_Instance.restorePointName
            End If

            wait.createPleaseWaitWindow("Creating Restore Point... Please Wait.", True, enums.howToCenterWindow.screen, True)

            ' This is a special way of making a thread in which you can pass parameters to the sub-routine that you're running as a separate thread.
            Dim creatingThread As New Threading.Thread(Sub()
                                                           createSystemRestorePoint(True, restorePointName)
                                                       End Sub)
            creatingThread.Name = "Restore Point Creating Thread"
            creatingThread.Start()
        End Sub

        Public Sub doCreateRestorePointRoutine(boolAreWeInSafeMode As Boolean)
            giveSafeModeErrorMessage(boolAreWeInSafeMode)
            eventLogFunctions.writeToApplicationLogFile("Activated JumpList Task.", EventLogEntryType.Information, False)

            Dim strRestorePointName As String = "System Checkpoint made by System Restore Point Creator"

            If My.Application.CommandLineArgs.Count = 2 Then
                If My.Application.CommandLineArgs(1).Trim.StartsWith("-name=", StringComparison.OrdinalIgnoreCase) Then
                    strRestorePointName = Text.RegularExpressions.Regex.Replace(My.Application.CommandLineArgs(1).Trim, "-name=", "", Text.RegularExpressions.RegexOptions.IgnoreCase)
                End If
            End If

            If String.IsNullOrEmpty(My.Settings.savedRestorePointFromCommandLine) = False Then
                strRestorePointName = My.Settings.savedRestorePointFromCommandLine
                My.Settings.savedRestorePointFromCommandLine = Nothing
                My.Settings.Save()
            End If

            wait.createPleaseWaitWindow("Creating Restore Point... Please Wait.", True, enums.howToCenterWindow.screen, True)

            ' This is a special way of making a thread in which you can pass parameters to the sub-routine that you're running as a separate thread.
            Dim creatingThread As New Threading.Thread(Sub()
                                                           createSystemRestorePoint(True, strRestorePointName)
                                                       End Sub)
            creatingThread.Name = "Restore Point Creating Thread"
            creatingThread.Start()
        End Sub

        Public Function isThereOtherInstancesOfMeRunning() As Boolean
            Try
                Using searcher As New ManagementObjectSearcher("root\CIMV2", String.Format("SELECT * FROM Win32_Process WHERE Name = '{0}' AND ProcessID != {1}", (New FileInfo(Application.ExecutablePath)).Name, Process.GetCurrentProcess.Id))
                    Return If(searcher IsNot Nothing, (searcher.Get().Count() > 0), False)
                End Using
            Catch ex As Exception
                Return False
            End Try
        End Function

        Public Sub repairRuntimeTasks()
            Dim task As Task = Nothing

            If taskStuff.doesRunTimeTaskExist("Restore Point Creator -- Run with no UAC (Create Restore Point)", task) = True Then
                taskStuff.deleteTask(task)
                task.Dispose()

                taskStuff.addRunTimeTask("Restore Point Creator -- Run with no UAC (Create Restore Point)", "Runs Restore Point Creator with no UAC prompt.", Application.ExecutablePath, globalVariables.commandLineSwitches.createRestorePoint)
            End If

            If taskStuff.doesRunTimeTaskExist("Restore Point Creator -- Run with no UAC (Create Custom Restore Point)", task) = True Then
                taskStuff.deleteTask(task)
                task.Dispose()

                taskStuff.addRunTimeTask("Restore Point Creator -- Run with no UAC (Create Custom Restore Point)", "Runs Restore Point Creator with no UAC prompt.", Application.ExecutablePath, globalVariables.commandLineSwitches.createCustomRestorePoint)
            End If

            If taskStuff.doesRunTimeTaskExist("Restore Point Creator -- Run with no UAC", task) = True Then
                taskStuff.deleteTask(task)
                task.Dispose()

                taskStuff.addRunTimeTask("Restore Point Creator -- Run with no UAC", "Runs Restore Point Creator with no UAC prompt.", Application.ExecutablePath, "", True)
            End If

            If taskStuff.doesRunTimeTaskExist("Restore Point Creator -- Run with no UAC (Delete old Restore Points)", task) = True Then
                taskStuff.deleteTask(task)
                task.Dispose()

                taskStuff.addRunTimeTask("Restore Point Creator -- Run with no UAC (Delete old Restore Points)", "Runs Restore Point Creator with no UAC prompt.", Application.ExecutablePath, globalVariables.commandLineSwitches.deleteOldRestorePoints)
            End If

            If taskStuff.doesRunTimeTaskExist("Restore Point Creator -- Run with no UAC (Keep X Number of Restore Points)", task) = True Then
                taskStuff.deleteTask(task)
                task.Dispose()

                taskStuff.addRunTimeTask("Restore Point Creator -- Run with no UAC (Delete old Restore Points)", "Runs Restore Point Creator with no UAC prompt.", Application.ExecutablePath, globalVariables.commandLineSwitches.keepXNumberOfRestorePoints)
            End If

            MsgBox("Some required system configurations for normal operation of the program needed to be repaired. Those repairs have been completed." & vbCrLf & vbCrLf & "Please relaunch the program as normal.", MsgBoxStyle.Information, "Restore Point Creator")
        End Sub

        Public Sub createSystemRestorePoint(Optional displayMessage As Boolean = True, Optional strRestorePointDescription As String = "System Checkpoint made by System Restore Point Creator")
            Try
                Dim newRestorePointID As Integer
                Dim result As Integer

                Dim oldNewestRestorePointID As Integer = wmi.getNewestSystemRestorePointID()
                result = wmi.createRestorePoint(strRestorePointDescription, restorePointStuff.RestoreType.WindowsType, newRestorePointID)

                If displayMessage = True Then
                    Dim msgBoxTitle As String = "Restore Point Creator"

                    If result = APIs.errorCodes.ERROR_SUCCESS Then
                        While wmi.getNewestSystemRestorePointID() = oldNewestRestorePointID
                            Threading.Thread.Sleep(500)
                        End While

                        wait.closePleaseWaitWindow()
                    ElseIf result = APIs.errorCodes.ERROR_DISK_FULL Then
                        MsgBox("System Restore Point Creation Failed.  Disk Full." & vbCrLf & vbCrLf & "Internal Windows Error Code: ERROR_DISK_FULL (112)", MsgBoxStyle.Critical, msgBoxTitle)
                    ElseIf result = APIs.errorCodes.ERROR_ACCESS_DENIED Then
                        MsgBox("System Restore Point Creation Failed.  Access Denied." & vbCrLf & vbCrLf & "Internal Windows Error Code: ERROR_ACCESS_DENIED (5)", MsgBoxStyle.Critical, msgBoxTitle)
                    ElseIf result = APIs.errorCodes.ERROR_INTERNAL_ERROR Then
                        MsgBox("System Restore Point Creation Failed.  Internal Error." & vbCrLf & vbCrLf & "Internal Windows Error Code: ERROR_INTERNAL_ERROR (1359)", MsgBoxStyle.Critical, msgBoxTitle)
                    ElseIf result = APIs.errorCodes.ERROR_INVALID_DATA Then
                        MsgBox("System Restore Point Creation Failed.  Invalid Data." & vbCrLf & vbCrLf & "Internal Windows Error Code: ERROR_INVALID_DATA (13)", MsgBoxStyle.Critical, msgBoxTitle)
                    ElseIf result = APIs.errorCodes.ERROR_TIMEOUT Then
                        MsgBox("System Restore Point Creation Failed.  Invalid Data." & vbCrLf & vbCrLf & "Internal Windows Error Code: ERROR_TIMEOUT (1460)", MsgBoxStyle.Critical, msgBoxTitle)
                    ElseIf result = APIs.errorCodes.ERROR_SERVICE_DISABLED Then
                        MsgBox("System Restore Point Creation Failed.  Invalid Data." & vbCrLf & vbCrLf & "Internal Windows Error Code: ERROR_SERVICE_DISABLED (1058)", MsgBoxStyle.Critical, msgBoxTitle)
                    ElseIf result = APIs.errorCodes.ERROR_BAD_ENVIRONMENT Then
                        MsgBox("System Restore Point Creation Failed.  Invalid Data." & vbCrLf & vbCrLf & "Internal Windows Error Code: ERROR_BAD_ENVIRONMENT (10)", MsgBoxStyle.Critical, msgBoxTitle)
                    Else
                        MsgBox("System Restore Point Creation Failed." & vbCrLf & vbCrLf & "Internal Windows Error Code: UNKNOWN_ERROR (9999)", MsgBoxStyle.Critical, msgBoxTitle)
                    End If

                    msgBoxTitle = Nothing
                End If
            Catch ex As Exception
                Threading.Thread.CurrentThread.CurrentUICulture = New Globalization.CultureInfo("en-US")
                exceptionHandler.manuallyLoadCrashWindow(ex, ex.Message, ex.StackTrace, ex.GetType)
            End Try
        End Sub

        Public Sub deleteAllTasks(Optional boolKillProcessAfterRun As Boolean = True)
            Try
                Dim taskServiceObject As New TaskService
                taskServiceObject.RootFolder.DeleteTask("System Restore Checkpoint by System Restore Point Creator", False)
                Dim taskFolderObject As TaskFolder = taskStuff.getOurTaskFolder(taskServiceObject)

                For Each task As Task In taskFolderObject.GetTasks()
                    taskFolderObject.DeleteTask(task.Name)
                Next

                taskServiceObject.RootFolder.DeleteFolder(taskFolderObject.Name)

                taskServiceObject.Dispose()
                taskFolderObject.Dispose()
                taskFolderObject = Nothing
                taskServiceObject = Nothing

                If boolKillProcessAfterRun = True Then Process.GetCurrentProcess.Kill()
            Catch ex As Exception
                eventLogFunctions.writeCrashToApplicationLogFile(ex)
            End Try
        End Sub

        Public Sub prefsCleanup()
            If Registry.LocalMachine.OpenSubKey(globalVariables.registryValues.strKey, False) IsNot Nothing Then
                Registry.LocalMachine.OpenSubKey("SOFTWARE", True).DeleteSubKeyTree(globalVariables.registryValues.strKeyInsideSoftware, False)
            End If

            deleteAllTasks(False)

            Dim strPathToPrefsDataDirectory As String = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "Restore_Point_Creator")

            If Directory.Exists(strPathToPrefsDataDirectory) = True Then
                My.Settings.Reset()
                My.Settings.Save()

                Try
                    Directory.Delete(strPathToPrefsDataDirectory, True)
                Catch ex As Exception
                End Try
            End If

            taskStuff.enableBuiltInRestorePointTask()
            Process.GetCurrentProcess.Kill()
        End Sub

        Public Sub deleteOldRestorePoints()
            Try
                ' First we read it as a String and store it in memory as a String.
                Dim maxDaysFromRegistryAsString As String = Registry.LocalMachine.OpenSubKey(globalVariables.registryValues.strKey, False).GetValue("MaxDays", 15)
                maxDaysFromRegistryAsString = maxDaysFromRegistryAsString.Trim ' Then we trim it.
                Dim maxDays As Short ' We now make a Short variable.

                ' Check to see if the String is a valid Short value.
                If Short.TryParse(maxDaysFromRegistryAsString, maxDays) = False And My.Application.CommandLineArgs.Count <> 2 Then
                    Exit Sub
                End If

                If My.Settings.maxDaysAtRelaunch <> -1 Then
                    maxDays = My.Settings.maxDaysAtRelaunch
                    My.Settings.maxDaysAtRelaunch = -1
                    My.Settings.Save()
                End If

                If My.Application.CommandLineArgs.Count = 2 Then
                    ' OK, the user provided a second command line argument so let's check it out.
                    If My.Application.CommandLineArgs(1).Trim.StartsWith("-maxdays", StringComparison.OrdinalIgnoreCase) Then
                        ' Let's try and parse the value that the user gave us. If it parses then what's inside the IF statement will not execute and this is just fine; things are OK so we can continue as normal.
                        If Not Short.TryParse(My.Application.CommandLineArgs(1).Trim.caseInsensitiveReplace("-maxdays=", "").Trim, maxDays) Then
                            ' We tried to parse it and we failed so we give the user an error message.
                            Console.WriteLine("ERROR: You have provided an invalid numeric input, please try again.")
                            Process.GetCurrentProcess.Kill()
                        End If
                    End If
                End If

                ' Get all System Restore Points from the Windows Management System and puts then in the systemRestorePoints variable.
                Dim systemRestorePoints As ManagementObjectSearcher = New ManagementObjectSearcher("root\DEFAULT", "SELECT * FROM SystemRestore")
                Dim systemRestorePointCreationDate As Date
                Dim dateDiffResults As Short
                Dim numberOfOldRestorePointsDeleted As Short = 0

                Dim boolLogDeletedRestorePoints As Boolean = registryStuff.getBooleanValueFromRegistry(Registry.LocalMachine.OpenSubKey(globalVariables.registryValues.strKey), "Log Restore Point Deletions", True)

                If boolLogDeletedRestorePoints Then
                    eventLogFunctions.writeToApplicationLogFile("Began processing of old System Restore Points.", EventLogEntryType.Information, False, False)
                End If

                ' Loops through systemRestorePoints.
                For Each systemRestorePoint As ManagementObject In systemRestorePoints.Get()
                    If String.IsNullOrEmpty(systemRestorePoint("CreationTime").ToString.Trim) = False Then
                        systemRestorePointCreationDate = restorePointStuff.parseSystemRestorePointCreationDate(systemRestorePoint("CreationTime"))

                        dateDiffResults = Math.Abs(DateDiff(DateInterval.Day, Date.Now, systemRestorePointCreationDate))

                        If dateDiffResults >= maxDays Then
                            APIs.NativeMethods.SRRemoveRestorePoint(Integer.Parse(systemRestorePoint("SequenceNumber"))) ' Deletes the Restore Point.

                            If boolLogDeletedRestorePoints Then
                                numberOfOldRestorePointsDeleted += 1

                                If boolLogDeletedRestorePoints = True Then
                                    eventLogFunctions.writeToApplicationLogFile(String.Format("Deleted Restore Point named ""{0}"" which was created on {1} at {2}.", systemRestorePoint("Description"), systemRestorePointCreationDate.ToLongDateString, systemRestorePointCreationDate.ToShortTimeString), EventLogEntryType.Information, False, False)
                                End If
                            End If
                        End If

                        systemRestorePointCreationDate = Nothing
                    End If

                    systemRestorePoint.Dispose()
                    systemRestorePoint = Nothing
                Next

                If boolLogDeletedRestorePoints = True Then
                    If numberOfOldRestorePointsDeleted = 0 Then
                        eventLogFunctions.writeToApplicationLogFile("End of processing old System Restore Points.  No old System Restore Point were deleted.", EventLogEntryType.Information, False, False)
                    ElseIf numberOfOldRestorePointsDeleted = 1 Then
                        eventLogFunctions.writeToApplicationLogFile("End of processing old System Restore Points.  1 old System Restore Point was deleted.", EventLogEntryType.Information, False, False)
                    Else
                        eventLogFunctions.writeToApplicationLogFile(String.Format("End of processing old System Restore Points.  {0} old System Restore Points were deleted.", numberOfOldRestorePointsDeleted), EventLogEntryType.Information, False, False)
                    End If
                End If

                systemRestorePoints.Dispose()
                systemRestorePoints = Nothing
            Catch ex As Exception
                eventLogFunctions.writeCrashToApplicationLogFile(ex)
            End Try
        End Sub

        Public Sub restoreSystemRestorePoint()
            Try
                Try
                    Dim systemRestorePointIndex As Integer = Integer.Parse(preSelectedRestorePointID)
                    wmi.restoreToSystemRestorePoint(systemRestorePointIndex)
                    wait.closePleaseWaitWindow()
                    isMyRestoreThreadRunning = False
                Catch ex As Exception
                    MsgBox("Unable to restore system back to user defined System Restore Point." & vbCrLf & vbCrLf & ex.Message, MsgBoxStyle.Critical, "Critical Error")
                End Try
            Catch ex As Threading.ThreadAbortException
                ' Does nothing.
            Finally
                ' Does nothing.
            End Try
        End Sub

        Public Sub giveSafeModeErrorMessage(boolAreWeInSafeMode As Boolean)
            If boolAreWeInSafeMode = True Then
                MsgBox("You are in Safe Mode, it's not recommended to make restore points in Safe Mode.", MsgBoxStyle.Information, "Restore Point Creator")
                Process.GetCurrentProcess.Kill()
            End If
        End Sub

        Public Sub writeLastRunFile()
            Try
                Using fileStream As New StreamWriter("lastrun.txt")
                    fileStream.Write(Now.ToString)
                End Using
            Catch ex As Exception
                eventLogFunctions.writeCrashToApplicationLogFile(ex, EventLogEntryType.Warning)
                ' Does nothing.
            End Try
        End Sub

        Public Sub giveMessageToUserAboutNotBeingAbleToCreateRegistrySubKey()
            eventLogFunctions.writeToApplicationLogFile("Unable to create Registry Key for program in HKEY_LOCAL_MACHINE\SOFTWARE. Restore Point Creator can't continue. Please check with your System Administrator to see if you have access rights to HKEY_LOCAL_MACHINE.", EventLogEntryType.Error, False)
            MsgBox("Unable to create Registry Key for program in HKEY_LOCAL_MACHINE\SOFTWARE. Restore Point Creator can't continue. Please check with your System Administrator to see if you have access rights to HKEY_LOCAL_MACHINE.", MsgBoxStyle.Critical, "Restore Point Creator")
        End Sub

        Private Sub handleLockedSettingsFile(ex As Exception)
            eventLogFunctions.writeToApplicationLogFile("Unable to open application settings file, it appears to be locked by another process.", EventLogEntryType.Error, False)
            eventLogFunctions.writeCrashToApplicationLogFile(ex)
            MsgBox("Unable to open application settings file, it appears to be locked by another process." & vbCrLf & vbCrLf & "The program will now close.", MsgBoxStyle.Critical, "Restore Point Creator")
            Process.GetCurrentProcess.Kill()
        End Sub

        Public Sub validateSettings()
            Try
                ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.PerUserRoamingAndLocal)
            Catch ex As ConfigurationErrorsException
                support.deleteFileWithNoException(ex.Filename)
            Catch ex2 As IOException
                handleLockedSettingsFile(ex2)
            End Try

            Try
                ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.PerUserRoaming)
            Catch ex As ConfigurationErrorsException
                support.deleteFileWithNoException(ex.Filename)
            Catch ex2 As IOException
                handleLockedSettingsFile(ex2)
            End Try

            Try
                ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None)
            Catch ex As ConfigurationErrorsException
                support.deleteFileWithNoException(ex.Filename)
            Catch ex2 As IOException
                handleLockedSettingsFile(ex2)
            End Try

            Try
                testSettingsAccess(My.Settings.boolFirstRun)
                My.Settings.boolFirstRun = My.Settings.boolFirstRun
                My.Settings.Save()
            Catch ex As Exception
                handleLockedSettingsFile(ex)
            End Try
        End Sub

        Private Sub testSettingsAccess(boolData As Boolean)
            ' Does nothing. All this sub-routine does is accept an input and does nothing with it.
        End Sub

        Sub writeKeyToRegistryToForceUpdateAtNextRun()
            Try
                Dim registryObject As RegistryKey = Registry.LocalMachine.OpenSubKey(globalVariables.registryValues.strKey, True)
                registryObject.SetValue("UpdateAtNextRunTime", "True", RegistryValueKind.String)
                registryObject.Close()
            Catch ex As Exception
            End Try
        End Sub

        Public Sub performApplicationUpdate(commandLineArgument As String)
            Try
                Dim boolExtendedLoggingForUpdating As Boolean = True

                Dim registryObject As RegistryKey = Registry.LocalMachine.OpenSubKey(globalVariables.registryValues.strKey, False)
                If registryObject IsNot Nothing Then
                    boolExtendedLoggingForUpdating = registryStuff.getBooleanValueFromRegistry(registryObject, "Enable Extended Logging During Updating", True)
                    registryObject.Close()
                End If

                wait.createPleaseWaitWindow("Updating Restore Point Creator... Please Wait.", True, enums.howToCenterWindow.screen, True, True)

                If boolExtendedLoggingForUpdating = True Then
                    eventLogFunctions.writeToApplicationLogFile("Update thread sleeping for 5 seconds for processes to close out before continuing with update procedure.", EventLogEntryType.Information, False)
                End If

                Threading.Thread.Sleep(5000)

                Dim boolNeedsReboot As Boolean = False
                Dim currentProcessFileName As String = New FileInfo(Application.ExecutablePath).Name

                If boolExtendedLoggingForUpdating = True Then
                    eventLogFunctions.writeToApplicationLogFile("Beginning second phase of application update process. Verifying environment for updating.", EventLogEntryType.Information, False)
                End If

                If currentProcessFileName.caseInsensitiveContains(".new.exe") Then
                    If boolExtendedLoggingForUpdating = True Then
                        eventLogFunctions.writeToApplicationLogFile("The environment is ready for updating.", EventLogEntryType.Information, False)
                    End If

                    Dim restorePointCreatorMainEXEName As String = currentProcessFileName.caseInsensitiveReplace(".new.exe", "")

                    If commandLineArgument.Equals("-update", StringComparison.OrdinalIgnoreCase) Then
                        registryStuff.updateRestorePointCreatorUninstallationInfo()
                        eventLogFunctions.writeToApplicationLogFile(String.Format("Updated program to version {0}.", globalVariables.version.strFullVersionString), EventLogEntryType.Information, False)
                    End If

                    If File.Exists(globalVariables.pdbFileNameInZIP & ".new") = True Then
                        If boolExtendedLoggingForUpdating = True Then
                            eventLogFunctions.writeToApplicationLogFile("PDB file found. Starting the updating of the PDF file.", EventLogEntryType.Information, False)
                        End If

                        Try
                            File.Delete(globalVariables.pdbFileNameInZIP)
                            File.Move(globalVariables.pdbFileNameInZIP & ".new", globalVariables.pdbFileNameInZIP)

                            Dim deleteAtReboot As New deleteAtReboot()
                            deleteAtReboot.removeItem(globalVariables.pdbFileNameInZIP)
                            deleteAtReboot.removeItem(globalVariables.pdbFileNameInZIP & ".new")
                            deleteAtReboot.dispose(True)
                            deleteAtReboot = Nothing

                            If boolExtendedLoggingForUpdating = True Then
                                eventLogFunctions.writeToApplicationLogFile("Update of the PDB file complete.", EventLogEntryType.Information, False)
                            End If
                        Catch ex As Exception
                            Dim pdbFullPath As String = New FileInfo(globalVariables.pdbFileNameInZIP).FullName

                            Dim deleteAtReboot As New deleteAtReboot()
                            deleteAtReboot.addItem(pdbFullPath)
                            deleteAtReboot.addItem(pdbFullPath & ".new", pdbFullPath)
                            deleteAtReboot.dispose(True)
                            deleteAtReboot = Nothing

                            boolNeedsReboot = True

                            eventLogFunctions.writeToApplicationLogFile("Something went wrong with the updating of the PDB file, scheduling it for update at system reboot.", EventLogEntryType.Error, False)
                        End Try
                    Else
                        If boolExtendedLoggingForUpdating = True Then
                            eventLogFunctions.writeToApplicationLogFile("No PDB file found, skipping PDB file update.", EventLogEntryType.Information, False)
                        End If
                    End If

                    If boolExtendedLoggingForUpdating = True Then
                        eventLogFunctions.writeToApplicationLogFile(String.Format("Killing process with parent executable of {0}{1}{0}.", Chr(34), restorePointCreatorMainEXEName), EventLogEntryType.Information, False)
                    End If

                    support.searchForProcessAndKillIt(restorePointCreatorMainEXEName, False)

                    If boolExtendedLoggingForUpdating = True Then
                        eventLogFunctions.writeToApplicationLogFile("Starting the updating of the core executable file.", EventLogEntryType.Information, False)
                    End If

                    Try
                        File.Delete(restorePointCreatorMainEXEName)
                        File.Copy(currentProcessFileName, restorePointCreatorMainEXEName)

                        Dim deleteAtReboot As New deleteAtReboot()
                        deleteAtReboot.removeItem(restorePointCreatorMainEXEName)
                        deleteAtReboot.removeItem(currentProcessFileName)
                        deleteAtReboot.dispose(True)
                        deleteAtReboot = Nothing

                        If boolExtendedLoggingForUpdating = True Then
                            eventLogFunctions.writeToApplicationLogFile("Update of the core executable file complete.", EventLogEntryType.Information, False)
                        End If
                    Catch ex As Exception
                        Dim deleteAtReboot As New deleteAtReboot()
                        deleteAtReboot.addItem(New FileInfo(restorePointCreatorMainEXEName).FullName)
                        deleteAtReboot.addItem(New FileInfo(currentProcessFileName).FullName, New FileInfo(restorePointCreatorMainEXEName).FullName)
                        deleteAtReboot.dispose(True)
                        deleteAtReboot = Nothing

                        boolNeedsReboot = True

                        eventLogFunctions.writeToApplicationLogFile("Something went wrong with the updating of the core executable file, scheduling it for update at system reboot.", EventLogEntryType.Error, False)
                    End Try

                    If boolNeedsReboot = True Then
                        wait.closePleaseWaitWindow()
                        writeKeyToRegistryToForceUpdateAtNextRun()

                        Dim msgBoxResult As MsgBoxResult = MsgBox("A system restart will need to be done in order to finish the update. System Restore Point Creator will not function properly until you restart your system." & vbCrLf & vbCrLf & "Would you like to restart your system now?", MsgBoxStyle.Question + MsgBoxStyle.YesNo, "Restart?")

                        If msgBoxResult = MsgBoxResult.Yes Then
                            If boolExtendedLoggingForUpdating = True Then
                                eventLogFunctions.writeToApplicationLogFile("Rebooting system.", EventLogEntryType.Information, False)
                            End If

                            support.rebootSystem()
                            Process.GetCurrentProcess.Kill()
                        Else
                            If boolExtendedLoggingForUpdating = True Then
                                eventLogFunctions.writeToApplicationLogFile("User chose not to reboot the system. Application updating is scheduled for the next system reboot.", EventLogEntryType.Information, False)
                            End If

                            MsgBox("System Restore Point Creator will not function properly until your system is rebooted.", MsgBoxStyle.Information, "System Restore Point Creator -- Application Update")
                        End If
                    Else
                        If boolExtendedLoggingForUpdating = True Then
                            eventLogFunctions.writeToApplicationLogFile("Starting the third and final phase of application update procedure; verification of update.", EventLogEntryType.Information, False)
                        End If

                        If File.Exists(restorePointCreatorMainEXEName) = True Then
                            If boolExtendedLoggingForUpdating = True Then
                                eventLogFunctions.writeToApplicationLogFile("Final verification of update complete, things all look good. Starting newly updated program.", EventLogEntryType.Information, False)
                            End If

                            eventLogFunctions.writeToApplicationLogFile("Application Update Procedure Complete.", EventLogEntryType.Information, False)

                            Process.Start(New ProcessStartInfo With {.FileName = restorePointCreatorMainEXEName, .Verb = "runas", .Arguments = "-noparentprocesscheck"})
                        Else
                            MsgBox("Something went wrong during the update process.", MsgBoxStyle.Critical, "Restore Point Creator Update Procedure")
                            Process.Start(Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Windows), "explorer.exe"), New FileInfo(Application.ExecutablePath).DirectoryName)
                        End If

                        Process.GetCurrentProcess.Kill()
                    End If
                Else
                    MsgBox("The environment is not ready for an update. This process will now terminate.", MsgBoxStyle.Critical, "Restore Point Creator")
                    Process.GetCurrentProcess.Kill()
                End If
            Catch ex As Exception
                eventLogFunctions.writeCrashToApplicationLogFile(ex)
                MsgBox("Something went wrong during the application update procedure, please see the Application Event Log for more details.", MsgBoxStyle.Critical, "Restore Point Creator -- Error")
                Process.GetCurrentProcess.Kill()
            End Try
        End Sub
    End Module
End Namespace