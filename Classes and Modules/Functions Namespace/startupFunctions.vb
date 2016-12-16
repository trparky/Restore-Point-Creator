Imports Microsoft.Win32.TaskScheduler
Imports System.IO
Imports Microsoft.Win32
Imports System.Management
Imports System.Text.RegularExpressions
Imports System.Configuration

Namespace Functions.startupFunctions
    Module startupFunctions
        Public Sub downloadWindowsXPVersion()
            Dim windowsXPVersionEXEURL As String = "http://www.toms-world.org/download/Restore Point Creator (Windows XP).exe"
            Dim windowsXPVersionSHA1URL As String = "http://www.toms-world.org/download/Restore Point Creator (Windows XP).exe.sha1"

            If MsgBox("This version of System Restore Point Creator is NOT compatible with Windows XP and will NOT run on Windows XP." & vbCrLf & vbCrLf & "Would you like to go back to the last version that supported Windows XP?", MsgBoxStyle.Question + MsgBoxStyle.YesNo, "System Restore Point Creator Windows XP Support") = MsgBoxResult.Yes Then
                wait.createPleaseWaitWindow("Downloading File... Please Wait.", True, enums.howToCenterWindow.screen, True)

                If http.downloadFile("http://www.toms-world.org/download/updater.exe", "updater.exe") = False Then
                    wait.closePleaseWaitWindow()
                    MsgBox("There was an error while downloading required files, please check the Event Log for more details.", MsgBoxStyle.Critical, "Restore Point Creator")

                    Process.GetCurrentProcess.Kill()
                End If

                If File.Exists("updater.exe") = True Then
                    If checksum.verifyChecksum("http://www.toms-world.org/download/updater.exe.sha1", "updater.exe", True) = False Then
                        wait.closePleaseWaitWindow()
                        MsgBox("There was an error while downloading required files, please check the Event Log for more details.", MsgBoxStyle.Critical, "Restore Point Creator")

                        Process.GetCurrentProcess.Kill()
                    End If
                End If

                Dim fileInfo As New FileInfo(Application.ExecutablePath)

                If http.downloadFile(windowsXPVersionEXEURL, fileInfo.Name & ".New") = False Then
                    wait.closePleaseWaitWindow()
                    MsgBox("There was an error while downloading required files, please check the Event Log for more details.", MsgBoxStyle.Critical, "Restore Point Creator")

                    Process.GetCurrentProcess.Kill()
                End If

                If File.Exists(fileInfo.Name & ".new") = True Then
                    If checksum.verifyChecksum(windowsXPVersionSHA1URL, fileInfo.Name & ".new", True) = False Then
                        wait.closePleaseWaitWindow()
                        MsgBox("There was an error while downloading required files, please check the Event Log for more details.", MsgBoxStyle.Critical, "Restore Point Creator")

                        Process.GetCurrentProcess.Kill()
                    End If
                End If

                fileInfo = Nothing

                Process.Start(globalVariables.updaterFileName, String.Format("--file={0}{1}{0}", Chr(34), Application.ExecutablePath))

                wait.closePleaseWaitWindow()
                Application.Exit()
            End If

            Process.GetCurrentProcess.Kill()
        End Sub

        Public isMyRestoreThreadRunning As Boolean = False
        Public preSelectedRestorePointID As Integer

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
                vss.checkForAndEnableSystemRestoreIfNeeded()

                Dim newRestorePointID As Integer
                Dim result As Integer

                Dim oldNewestRestorePointID As Integer = wmi.getNewestSystemRestorePointID()
                result = wmi.createRestorePoint(strRestorePointDescription, support.RestoreType.WindowsType, newRestorePointID)
                wait.closePleaseWaitWindow()

                If displayMessage = True Then
                    Dim msgBoxTitle As String = "Restore Point Creator"

                    If result = APIs.errorCodes.ERROR_SUCCESS Then
                        ' Does nothing.
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
                eventLogFunctions.writeCrashToEventLog(ex)
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
                ' Get all System Restore Points from the Windows Management System and puts then in the systemRestorePoints variable.
                Dim systemRestorePoints As ManagementObjectSearcher = New ManagementObjectSearcher("root\DEFAULT", "SELECT * FROM SystemRestore")
                Dim systemRestorePointCreationDate As Date
                Dim dateDiffResults As Short
                Dim numberOfOldRestorePointsDeleted As Short = 0

                ' First we read it as a String and store it in memory as a String.
                Dim boolValueFromRegistryAsString As String = Registry.LocalMachine.OpenSubKey(globalVariables.registryValues.strKey).GetValue("Log Restore Point Deletions", "False")
                boolValueFromRegistryAsString = boolValueFromRegistryAsString.Trim ' Then we trim it.
                Dim boolLogDeletedRestorePoints As Boolean ' We now make a Boolean variable.

                ' First we read it as a String and store it in memory as a String.
                Dim maxDaysFromRegistryAsString As String = Registry.LocalMachine.OpenSubKey(globalVariables.registryValues.strKey, False).GetValue("MaxDays", 15)
                maxDaysFromRegistryAsString = maxDaysFromRegistryAsString.Trim ' Then we trim it.
                Dim maxDays As Short ' We now make a Short variable.

                ' Check to see if the String is a valid Short value.
                If Short.TryParse(maxDaysFromRegistryAsString, maxDays) = False Then
                    ' Nope, now we exit this subroutine since we have no valid data now.
                    Exit Sub
                End If

                ' Check to see if the String is a valid Boolean String value.
                If Boolean.TryParse(boolValueFromRegistryAsString.Trim, boolLogDeletedRestorePoints) = False Then
                    ' Oops, we have invalid data so at this point we are going to ignore the setting in the Registry and just assume a True value.
                    boolLogDeletedRestorePoints = True
                End If

                If boolLogDeletedRestorePoints = True Then
                    eventLogFunctions.writeToSystemEventLog("Began processing of old System Restore Points.", EventLogEntryType.Information)
                End If

                ' Loops through systemRestorePoints.
                For Each systemRestorePoint As ManagementObject In systemRestorePoints.Get()
                    If String.IsNullOrEmpty(systemRestorePoint("CreationTime").ToString.Trim) = False Then
                        systemRestorePointCreationDate = support.parseSystemRestorePointCreationDate(systemRestorePoint("CreationTime"))

                        dateDiffResults = Math.Abs(DateDiff(DateInterval.Day, Date.Now, systemRestorePointCreationDate))

                        If dateDiffResults >= maxDays Then
                            support.SRRemoveRestorePoint(Integer.Parse(systemRestorePoint("SequenceNumber"))) ' Deletes the Restore Point.

                            If boolLogDeletedRestorePoints Then
                                numberOfOldRestorePointsDeleted += 1

                                If boolLogDeletedRestorePoints = True Then
                                    eventLogFunctions.writeToSystemEventLog(String.Format("Deleted Restore Point named ""{0}"" which was created on {1} at {2}.", systemRestorePoint("Description"), systemRestorePointCreationDate.ToLongDateString, systemRestorePointCreationDate.ToShortTimeString), EventLogEntryType.Information)
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
                        eventLogFunctions.writeToSystemEventLog("End of processing old System Restore Points.  No old System Restore Point were deleted.", EventLogEntryType.Information)
                    ElseIf numberOfOldRestorePointsDeleted = 1 Then
                        eventLogFunctions.writeToSystemEventLog("End of processing old System Restore Points.  1 old System Restore Point was deleted.", EventLogEntryType.Information)
                    Else
                        eventLogFunctions.writeToSystemEventLog(String.Format("End of processing old System Restore Points.  {0} old System Restore Points were deleted.", numberOfOldRestorePointsDeleted), EventLogEntryType.Information)
                    End If
                End If

                systemRestorePoints.Dispose()
                systemRestorePoints = Nothing
            Catch ex As Exception
                eventLogFunctions.writeCrashToEventLog(ex)
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
                Dim fileStream As New StreamWriter("lastrun.txt")
                fileStream.Write(Now.ToString)
                fileStream.Close()
                fileStream.Dispose()
            Catch ex As Exception
                eventLogFunctions.writeCrashToEventLog(ex)
                ' Does nothing.
            End Try
        End Sub

        Public Sub giveMessageToUserAboutNotBeingAbleToCreateRegistrySubKey()
            globalVariables.boolLogToSystemLog = True
            eventLogFunctions.writeToSystemEventLog("Unable to create Registry Key for program in HKEY_LOCAL_MACHINE\SOFTWARE. Restore Point Creator can't continue. Please check with your System Administrator to see if you have access rights to HKEY_LOCAL_MACHINE.", EventLogEntryType.Error)
            MsgBox("Unable to create Registry Key for program in HKEY_LOCAL_MACHINE\SOFTWARE. Restore Point Creator can't continue. Please check with your System Administrator to see if you have access rights to HKEY_LOCAL_MACHINE.", MsgBoxStyle.Critical, "Restore Point Creator")
        End Sub

        Private Sub handleLockedSettingsFile(ex As IOException)
            eventLogFunctions.writeCrashToEventLog(ex)
            eventLogFunctions.writeToSystemEventLog("Unable to open application settings file, it appears to be locked by another process.", EventLogEntryType.Error)
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
        End Sub

        Sub writeKeyToRegistryToForceUpdateAtNextRun()
            Try
                Dim registryObject As RegistryKey = Registry.LocalMachine.OpenSubKey(globalVariables.registryValues.strKey, True)
                registryObject.SetValue("UpdateAtNextRunTime", "True", RegistryValueKind.String)
                registryObject.Close()
                registryObject.Dispose()
            Catch ex As Exception
            End Try
        End Sub

        Public Sub performApplicationUpdate(commandLineArgument As String)
            Dim boolExtendedLoggingForUpdating As Boolean = True

            Dim registryObject As RegistryKey = Registry.LocalMachine.OpenSubKey(globalVariables.registryValues.strKey, False)
            If registryObject IsNot Nothing Then
                If Boolean.TryParse(registryObject.GetValue("Enable Extended Logging During Updating", "True"), boolExtendedLoggingForUpdating) = False Then
                    ' This is in case we can't parse the value from the Registry.
                    boolExtendedLoggingForUpdating = True
                End If

                registryObject.Close()
                registryObject.Dispose()
            End If

            wait.createPleaseWaitWindow("Updating Restore Point Creator... Please Wait.", True, enums.howToCenterWindow.screen, True)

            If boolExtendedLoggingForUpdating = True Then
                eventLogFunctions.writeToSystemEventLog("Update thread sleeping for 5 seconds for processes to close out before continuing with update procedure.", EventLogEntryType.Information)
            End If

            Threading.Thread.Sleep(5000)

            Dim boolNeedsReboot As Boolean = False
            Dim currentProcessFileName As String = New FileInfo(Application.ExecutablePath).Name

            If boolExtendedLoggingForUpdating = True Then
                eventLogFunctions.writeToSystemEventLog("Beginning second phase of application update process. Verifying environment for updating.", EventLogEntryType.Information)
            End If

            If currentProcessFileName.caseInsensitiveContains(".new.exe") Then
                If boolExtendedLoggingForUpdating = True Then
                    eventLogFunctions.writeToSystemEventLog("The environment is ready for updating.", EventLogEntryType.Information)
                End If

                Dim restorePointCreatorMainEXEName As String = Regex.Replace(currentProcessFileName, Regex.Escape(".new.exe"), "", RegexOptions.IgnoreCase)

                If commandLineArgument.stringCompare("-update") Then
                    registryStuff.updateRestorePointCreatorUninstallationInfo()

                    If globalVariables.version.boolBeta = True Then
                        eventLogFunctions.writeToSystemEventLog(String.Format("Updated program to version {0} Public Beta {1}.", globalVariables.version.strFullVersionString, globalVariables.version.shortBetaVersion), EventLogEntryType.Information)
                    ElseIf globalVariables.version.boolReleaseCandidate = True Then
                        eventLogFunctions.writeToSystemEventLog(String.Format("Updated program to version {0} Release Candidate {1}.", globalVariables.version.strFullVersionString, globalVariables.version.shortReleaseCandidateVersion), EventLogEntryType.Information)
                    Else
                        eventLogFunctions.writeToSystemEventLog(String.Format("Updated program to version {0}.", globalVariables.version.strFullVersionString), EventLogEntryType.Information)
                    End If
                End If

                If File.Exists(globalVariables.pdbFileNameInZIP & ".new") = True Then
                    If boolExtendedLoggingForUpdating = True Then
                        eventLogFunctions.writeToSystemEventLog("PDB file found. Starting the updating of the PDF file.", EventLogEntryType.Information)
                    End If

                    Try
                        File.Delete(globalVariables.pdbFileNameInZIP)
                        File.Move(globalVariables.pdbFileNameInZIP & ".new", globalVariables.pdbFileNameInZIP)

                        If boolExtendedLoggingForUpdating = True Then
                            eventLogFunctions.writeToSystemEventLog("Update of the PDB file complete.", EventLogEntryType.Information)
                        End If
                    Catch ex As Exception
                        Dim pdbFullPath As String = New FileInfo(globalVariables.pdbFileNameInZIP).FullName
                        APIs.MoveFileEx(pdbFullPath, vbNullString, 4)
                        APIs.MoveFileEx(pdbFullPath & ".new", pdbFullPath, 4)
                        boolNeedsReboot = True

                        eventLogFunctions.writeToSystemEventLog("Something went wrong with the updating of the PDB file, scheduling it for update at system reboot.", EventLogEntryType.Error)
                    End Try
                Else
                    If boolExtendedLoggingForUpdating = True Then
                        eventLogFunctions.writeToSystemEventLog("No PDB file found, skipping PDB file update.", EventLogEntryType.Information)
                    End If
                End If

                If boolExtendedLoggingForUpdating = True Then
                    eventLogFunctions.writeToSystemEventLog(String.Format("Killing process with parent executable of {0}{1}{0}.", Chr(34), restorePointCreatorMainEXEName), EventLogEntryType.Information)
                End If

                support.searchForProcessAndKillIt(restorePointCreatorMainEXEName, False)

                If boolExtendedLoggingForUpdating = True Then
                    eventLogFunctions.writeToSystemEventLog("Starting the updating of the core executable file.", EventLogEntryType.Information)
                End If

                Try
                    File.Delete(restorePointCreatorMainEXEName)
                    File.Copy(currentProcessFileName, restorePointCreatorMainEXEName)

                    If boolExtendedLoggingForUpdating = True Then
                        eventLogFunctions.writeToSystemEventLog("Update of the core executable file complete.", EventLogEntryType.Information)
                    End If
                Catch ex As Exception
                    APIs.MoveFileEx(New FileInfo(restorePointCreatorMainEXEName).FullName, vbNullString, 4)
                    APIs.MoveFileEx(New FileInfo(currentProcessFileName).FullName, New FileInfo(restorePointCreatorMainEXEName).FullName, 4)
                    boolNeedsReboot = True

                    eventLogFunctions.writeToSystemEventLog("Something went wrong with the updating of the core executable file, scheduling it for update at system reboot.", EventLogEntryType.Error)
                End Try

                If boolNeedsReboot = True Then
                    writeKeyToRegistryToForceUpdateAtNextRun()

                    Dim msgBoxResult As MsgBoxResult = MsgBox("A system restart will need to be done in order to finish the update. System Restore Point Creator will not function properly until you restart your system." & vbCrLf & vbCrLf & "Would you like to restart your system now?", MsgBoxStyle.Question + MsgBoxStyle.YesNo, "Restart?")

                    If msgBoxResult = MsgBoxResult.Yes Then
                        If boolExtendedLoggingForUpdating = True Then
                            eventLogFunctions.writeToSystemEventLog("Rebooting system.", EventLogEntryType.Information)
                        End If

                        support.rebootSystem()
                        Process.GetCurrentProcess.Kill()
                    Else
                        If boolExtendedLoggingForUpdating = True Then
                            eventLogFunctions.writeToSystemEventLog("User chose not to reboot the system. Application updating is scheduled for the next system reboot.", EventLogEntryType.Information)
                        End If

                        MsgBox("System Restore Point Creator will not function properly until your system is rebooted.", MsgBoxStyle.Information, "System Restore Point Creator -- Application Update")
                    End If
                Else
                    If boolExtendedLoggingForUpdating = True Then
                        eventLogFunctions.writeToSystemEventLog("Starting the third and final phase of application update procedure; verification of update.", EventLogEntryType.Information)
                    End If

                    If File.Exists(restorePointCreatorMainEXEName) = True Then
                        If boolExtendedLoggingForUpdating = True Then
                            eventLogFunctions.writeToSystemEventLog("Final verification of update complete, things all look good. Starting newly updated program.", EventLogEntryType.Information)
                        End If

                        eventLogFunctions.writeToSystemEventLog("Application Update Procedure Complete.", EventLogEntryType.Information)

                        Process.Start(New ProcessStartInfo With {.FileName = restorePointCreatorMainEXEName, .Verb = "runas"})
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
        End Sub
    End Module
End Namespace