Imports Microsoft.Win32.TaskScheduler
Imports System.Management
Imports System.Globalization

Public Class frmTaskScheduler
    Private boolDoneLoading As Boolean = False
    Private boolThingsChanged As Boolean = False

    Private Const strCheckPointTaskName As String = "System Restore Checkpoint by System Restore Point Creator"
    Private Const strDeleteTaskName As String = "Delete Old Restore Points"

    Private Sub frmTaskScheduler_FormClosing(sender As Object, e As FormClosingEventArgs) Handles Me.FormClosing
        If boolThingsChanged = True Then
            Dim msgBoxResponse As MsgBoxResult = MsgBox("You have changed some task settings but have yet to save them. Closing this window before saving your task settings will result in the loss of all recent changes." & vbCrLf & vbCrLf & "Remember, you must click the ""Save Task"" button to save your task settings." & vbCrLf & vbCrLf & "Are you sure you want close this window?", MsgBoxStyle.Question + MsgBoxStyle.YesNo, Me.Text)

            If msgBoxResponse = MsgBoxResult.No Then
                e.Cancel = True
                Exit Sub
            End If
        End If

        My.Settings.TaskSchedulerWindowLocation = Me.Location
        globalVariables.windows.frmTaskScheduler.Dispose()
        globalVariables.windows.frmTaskScheduler = Nothing
    End Sub

    Public Function boolDoesTaskExist(taskName As String) As Boolean
        Dim taskObject As Task = (New TaskService).GetTask(taskName)

        If taskObject Is Nothing Then
            Return False
        Else
            Return True
        End If
    End Function

    Sub checkWindowsTaskScheduler()
        Try
            Dim serviceController As ServiceProcess.ServiceController

            Dim searcher As New ManagementObjectSearcher("root\CIMV2", "SELECT * FROM Win32_Service")
            Dim inParams, outParams As ManagementBaseObject
            Dim Result As Integer

            For Each queryObj As ManagementObject In searcher.Get()
                If queryObj("Name") = "Schedule" Then
                    If (queryObj("StartMode").ToString = "Auto") = False Then
                        MsgBox("Restore Point Creator has determined that the Windows Schedule Service has been disabled, Restore Point Creator will now attempt to repair this.", MsgBoxStyle.Information, Me.Text)

                        inParams = queryObj.GetMethodParameters("ChangeStartMode")
                        inParams("StartMode") = "Automatic"
                        outParams = queryObj.InvokeMethod("ChangeStartMode", inParams, Nothing)

                        Result = Convert.ToInt32(outParams("returnValue"))

                        If Result = 0 Then
                            MsgBox("Restore Point Creator has been able to repair the Windows Schedule Service's startup type, now attempting to start the service.", MsgBoxStyle.Information, Me.Text)

                            serviceController = New ServiceProcess.ServiceController("Schedule")

                            serviceController.Start()
                            serviceController.WaitForStatus(ServiceProcess.ServiceControllerStatus.Running, New TimeSpan(0, 0, 0, 2))

                            If serviceController.Status = ServiceProcess.ServiceControllerStatus.Running Then
                                MsgBox("The Windows Scheduler Service has been successfully started.", MsgBoxStyle.Information, Me.Text)
                            Else
                                MsgBox("Restore Point Creator has attempted to start the Windows Scheduler Service but has failed to do so.  This may indicate that the service has been broken in some way." & vbCrLf & vbCrLf & "Scheduled restore points will not function until this is corrected.", MsgBoxStyle.Information, Me.Text)
                            End If

                            serviceController.Close()
                            serviceController = Nothing

                            outParams.Dispose()
                            outParams = Nothing

                            Me.Close()
                        Else
                            MsgBox("Restore Point Creator has been unable to repair the Windows Schedule Service's startup type.  All scheduled restore points will not function until the Windows Schedule Service is repaired.", MsgBoxStyle.Information, Me.Text)
                            outParams.Dispose()
                            outParams = Nothing

                            Me.Close()
                        End If
                    End If
                End If
            Next

            searcher.Dispose()
            searcher = Nothing

            serviceController = New ServiceProcess.ServiceController("Schedule")

            If serviceController IsNot Nothing Then
                If (serviceController.Status = ServiceProcess.ServiceControllerStatus.Running) = False Then
                    MsgBox("The Windows Scheduler Service is currently not running, Restore Point Creator will attempt to start it.", MsgBoxStyle.Information, Me.Text)
                    serviceController.Start()
                    serviceController.WaitForStatus(ServiceProcess.ServiceControllerStatus.Running, New TimeSpan(0, 0, 0, 2))

                    If serviceController.Status = ServiceProcess.ServiceControllerStatus.Running Then
                        MsgBox("The Windows Scheduler Service has been successfully started.", MsgBoxStyle.Information, Me.Text)
                    Else
                        MsgBox("Restore Point Creator has attempted to start the Windows Scheduler Service but has failed to do so.  This may indicate that the service has been broken in some way." & vbCrLf & vbCrLf & "Scheduled restore points will not function until this is corrected.", MsgBoxStyle.Information, Me.Text)
                    End If
                End If
            End If

            serviceController.Close()
            serviceController = Nothing
        Catch ex As Exception
            ' If the above stuff crashes, we don't care.  Windows is broken at this point and we can't fix it.
        End Try
    End Sub

    Private Sub frmTaskScheduler_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        Me.Location = Functions.support.verifyWindowLocation(My.Settings.TaskSchedulerWindowLocation)
        checkWindowsTaskScheduler()

        Using regKey As Microsoft.Win32.RegistryKey = Microsoft.Win32.Registry.LocalMachine.OpenSubKey(globalVariables.registryValues.strKey, False)
            chkDeleteOldRestorePoints.Checked = Functions.registryStuff.getBooleanValueFromRegistry(regKey, "Delete Old Restore Points", False)
        End Using

        txtDays.Text = Microsoft.Win32.Registry.LocalMachine.OpenSubKey(globalVariables.registryValues.strKey, False).GetValue("MaxDays", 15).ToString
        txtDaysDelete.Text = Microsoft.Win32.Registry.LocalMachine.OpenSubKey(globalVariables.registryValues.strKey, False).GetValue("MaxDays", 15).ToString

        ' Checks to see if the Registry Value exists.
        If (Microsoft.Win32.Registry.LocalMachine.OpenSubKey(globalVariables.registryValues.strKey, True).GetValue("Every", Nothing) IsNot Nothing) Then
            txtEveryDay.Text = Microsoft.Win32.Registry.LocalMachine.OpenSubKey(globalVariables.registryValues.strKey, True).GetValue("Every")

            If Functions.support.isNumeric(txtEveryDay.Text) = False Then
                txtEveryDay.Text = Nothing
                lblDays.Visible = False
                lblEvery.Visible = False
                txtEveryDay.Visible = False
            End If
        End If

        If chkDeleteOldRestorePoints.Checked Then
            txtDays.Enabled = True
            btnSet.Enabled = True
            deleteOldRestorePointsTaskSettings.Enabled = False
        Else
            txtDays.Enabled = False
            btnSet.Enabled = False
        End If

        radDaily.Checked = False
        radWeekly.Checked = False

        If My.Settings.dontBugTheUserAboutPuttingTheEXEFileSomewhereSafe = False Then
            If Environment.Is64BitOperatingSystem Then
                MsgBox("In order to use this option of the program, you must put this program in a safe place.  For instance, C:\Program Files (x86)." & vbCrLf & vbCrLf & "The reason why is that this portion of the program creates a Windows Scheduled Task that runs this program so if the scheduled task can't find the program's EXE file, the task won't run.", MsgBoxStyle.Information, Me.Text)
            Else
                MsgBox("In order to use this option of the program, you must put this program in a safe place.  For instance, C:\Program Files." & vbCrLf & vbCrLf & "The reason why is that this portion of the program creates a Windows Scheduled Task that runs this program so if the scheduled task can't find the program's EXE file, the task won't run.", MsgBoxStyle.Information, Me.Text)
            End If

            My.Settings.dontBugTheUserAboutPuttingTheEXEFileSomewhereSafe = True
            My.Settings.Save()
        End If

        Dim taskService As New TaskService
        Dim taskTriggers As TriggerCollection
        Dim boolDoesTaskExist As Boolean = False
        Dim taskObject As Task = Nothing

        Try
            If Functions.taskStuff.doesTaskExist(strDeleteTaskName, taskObject) = True Then
                lblRunTimesDelete.Text = "Next Run Time: " & taskObject.NextRunTime.ToString & vbCrLf & "Last Run Time: " & taskObject.LastRunTime.ToString

                taskTriggers = taskObject.Definition.Triggers
                chkWakeDelete.Checked = taskObject.Definition.Settings.WakeToRun

                chkRunMissedTaskDelete.Checked = taskObject.Definition.Settings.StartWhenAvailable

                With taskTriggers.Item(0)
                    If .TriggerType = TaskTriggerType.Daily Then
                        radDailyDelete.Checked = True
                    ElseIf .TriggerType = TaskTriggerType.Weekly Then
                        radWeeklyDelete.Checked = True
                        grpDaysOfTheWeekDelete.Enabled = True

                        ' I hate this hack, I really fucking hate this fucking hack! But I know of no other fucking way to do this. Did I mention that I fucking hate this fucking hack? Yes, I think I fucking did.
                        If .ToString.caseInsensitiveContains(DateTimeFormatInfo.CurrentInfo.GetDayName(DayOfWeek.Sunday)) Then chkSundayDelete.Checked = True
                        If .ToString.caseInsensitiveContains(DateTimeFormatInfo.CurrentInfo.GetDayName(DayOfWeek.Monday)) Then chkMondayDelete.Checked = True
                        If .ToString.caseInsensitiveContains(DateTimeFormatInfo.CurrentInfo.GetDayName(DayOfWeek.Tuesday)) Then chkTuesdayDelete.Checked = True
                        If .ToString.caseInsensitiveContains(DateTimeFormatInfo.CurrentInfo.GetDayName(DayOfWeek.Wednesday)) Then chkWednesdayDelete.Checked = True
                        If .ToString.caseInsensitiveContains(DateTimeFormatInfo.CurrentInfo.GetDayName(DayOfWeek.Thursday)) Then chkThursdayDelete.Checked = True
                        If .ToString.caseInsensitiveContains(DateTimeFormatInfo.CurrentInfo.GetDayName(DayOfWeek.Friday)) Then chkFridayDelete.Checked = True
                        If .ToString.caseInsensitiveContains(DateTimeFormatInfo.CurrentInfo.GetDayName(DayOfWeek.Saturday)) Then chkSaturdayDelete.Checked = True
                    End If

                    Try
                        timePickerDelete.Value = .StartBoundary
                    Catch ex As ArgumentOutOfRangeException
                        Functions.eventLogFunctions.writeToSystemEventLog("There was an error loading the scheduled task's start time. The value that was returned was """ & .StartBoundary.ToString & """ which is invalid.", EventLogEntryType.Warning)
                        timePickerDelete.Value = Now
                    End Try
                End With

                taskTriggers.Dispose()
                taskTriggers = Nothing

                btnDeleteTaskDelete.Enabled = True
            End If
        Catch ex As FormatException
            Functions.eventLogFunctions.writeCrashToEventLog(ex)
            Functions.eventLogFunctions.writeToSystemEventLog("There was an error loading the saved schedule time from the """ & strDeleteTaskName & """ task.", EventLogEntryType.Error)
        End Try

        Try
            If Functions.taskStuff.doesTaskExist(strCheckPointTaskName, taskObject) = True Then
                lblRunTimes.Text = "Next Run Time: " & taskObject.NextRunTime.ToString & vbCrLf & "Last Run Time: " & taskObject.LastRunTime.ToString

                boolDoesTaskExist = True
                taskTriggers = taskObject.Definition.Triggers
                chkWake.Checked = taskObject.Definition.Settings.WakeToRun

                chkRunMissedTask.Checked = taskObject.Definition.Settings.StartWhenAvailable

                With taskTriggers.Item(0)
                    If .TriggerType = TaskTriggerType.Daily Then
                        ' We have to DirectCast the Trigger to a DailyTrigger first and then we'll get access to the DaysInterval value. I have no idea why we have to do this but we have to. ARG! It took me forever to figure this one out.
                        Dim shortEvery As Short = DirectCast(taskTriggers.Item(0), DailyTrigger).DaysInterval

                        If shortEvery = 1 Then
                            radDaily.Checked = True
                            txtEveryDay.Text = Nothing
                            lblDays.Visible = False
                            lblEvery.Visible = False
                            txtEveryDay.Visible = False
                        Else
                            radEvery.Checked = True
                            txtEveryDay.Text = shortEvery.ToString
                            lblDays.Visible = True
                            lblEvery.Visible = True
                            txtEveryDay.Visible = True
                        End If
                    ElseIf .TriggerType = TaskTriggerType.Weekly Then
                        radWeekly.Checked = True
                        grpDaysOfTheWeek.Enabled = True

                        ' I hate this hack, I really fucking hate this fucking hack! But I know of no other fucking way to do this. Did I mention that I fucking hate this fucking hack? Yes, I think I fucking did.
                        If .ToString.caseInsensitiveContains(DateTimeFormatInfo.CurrentInfo.GetDayName(DayOfWeek.Sunday)) Then chkSunday.Checked = True
                        If .ToString.caseInsensitiveContains(DateTimeFormatInfo.CurrentInfo.GetDayName(DayOfWeek.Monday)) Then chkMonday.Checked = True
                        If .ToString.caseInsensitiveContains(DateTimeFormatInfo.CurrentInfo.GetDayName(DayOfWeek.Tuesday)) Then chkTuesday.Checked = True
                        If .ToString.caseInsensitiveContains(DateTimeFormatInfo.CurrentInfo.GetDayName(DayOfWeek.Wednesday)) Then chkWednesday.Checked = True
                        If .ToString.caseInsensitiveContains(DateTimeFormatInfo.CurrentInfo.GetDayName(DayOfWeek.Thursday)) Then chkThursday.Checked = True
                        If .ToString.caseInsensitiveContains(DateTimeFormatInfo.CurrentInfo.GetDayName(DayOfWeek.Friday)) Then chkFriday.Checked = True
                        If .ToString.caseInsensitiveContains(DateTimeFormatInfo.CurrentInfo.GetDayName(DayOfWeek.Saturday)) Then chkSaturday.Checked = True
                    End If

                    Try
                        timePicker.Value = .StartBoundary
                    Catch ex As ArgumentOutOfRangeException
                        Functions.eventLogFunctions.writeToSystemEventLog("There was an error loading the scheduled task's start time. The value that was returned was """ & .StartBoundary.ToString & """ which is invalid.", EventLogEntryType.Warning)
                        timePicker.Value = Now
                    End Try
                End With

                taskTriggers.Dispose()
                taskTriggers = Nothing

                btnDeleteTask.Enabled = True
            End If
        Catch ex As FormatException
            Functions.eventLogFunctions.writeCrashToEventLog(ex)
            Functions.eventLogFunctions.writeToSystemEventLog("There was an error loading the saved schedule time from the """ & strCheckPointTaskName & """ task.", EventLogEntryType.Error)
        End Try

        taskService.Dispose()
        taskService = Nothing

        boolDoneLoading = True
    End Sub

    Private Sub radDaily_CheckedChanged(sender As Object, e As EventArgs) Handles radDaily.CheckedChanged
        grpDaysOfTheWeek.Enabled = False
        lblDays.Visible = False
        lblEvery.Visible = False
        txtEveryDay.Visible = False
    End Sub

    Private Sub radWeekly_CheckedChanged(sender As Object, e As EventArgs) Handles radWeekly.CheckedChanged
        grpDaysOfTheWeek.Enabled = True
        lblDays.Visible = False
        lblEvery.Visible = False
        txtEveryDay.Visible = False
    End Sub

    Private Sub radEvery_CheckedChanged(sender As Object, e As EventArgs) Handles radEvery.CheckedChanged
        grpDaysOfTheWeek.Enabled = False
        lblDays.Visible = True
        lblEvery.Visible = True
        txtEveryDay.Visible = True

        If String.IsNullOrEmpty(txtEveryDay.Text.Trim) Then
            txtEveryDay.Text = 2
        End If
    End Sub

    Private Sub btnSaveTask_Click(sender As Object, e As EventArgs) Handles btnSaveTask.Click
        Microsoft.Win32.Registry.LocalMachine.OpenSubKey(globalVariables.registryValues.strKey, True).DeleteValue("Every", False)
        Dim shortEvery As Short = Nothing

        If radEvery.Checked = True And Short.TryParse(txtEveryDay.Text.Trim, shortEvery) = False Then
            MsgBox("Invalid input for Every setting.", MsgBoxStyle.Critical, Me.Text)
            Exit Sub
        End If

        Dim taskService As New TaskService

        Try
            If radDaily.Checked = False And radWeekly.Checked = False And radEvery.Checked = False Then
                MsgBox("You must select a schedule type.", MsgBoxStyle.Information, Me.Text)
                Exit Sub
            End If

            If radWeekly.Checked = True And chkSunday.Checked = False And chkMonday.Checked = False And chkTuesday.Checked = False And chkWednesday.Checked = False And chkThursday.Checked = False And chkFriday.Checked = False And chkSaturday.Checked = False Then
                MsgBox("You must select days of the week for your weekly schedule.", MsgBoxStyle.Information, Me.Text)
                Exit Sub
            End If

            Dim newTask As TaskDefinition = taskService.NewTask
            newTask.RegistrationInfo.Description = strCheckPointTaskName

            Dim exePathInfo As New IO.FileInfo(Application.ExecutablePath)
            newTask.Actions.Add(New ExecAction(Chr(34) & exePathInfo.FullName & Chr(34), globalVariables.commandLineSwitches.scheduledRestorePoint, exePathInfo.DirectoryName))
            exePathInfo = Nothing

            If radDaily.Checked Then
                Dim trigger As Trigger = newTask.Triggers.Add(New DailyTrigger(1))
                trigger.StartBoundary = timePicker.Value
            ElseIf radEvery.Checked Then
                Dim trigger As Trigger = newTask.Triggers.Add(New DailyTrigger(shortEvery))
                trigger.StartBoundary = timePicker.Value
            ElseIf radWeekly.Checked Then
                Dim daysOfWeekSetting As DaysOfTheWeek

                If chkSunday.Checked Then
                    daysOfWeekSetting += DaysOfTheWeek.Sunday
                End If
                If chkMonday.Checked Then
                    daysOfWeekSetting += DaysOfTheWeek.Monday
                End If
                If chkTuesday.Checked Then
                    daysOfWeekSetting += DaysOfTheWeek.Tuesday
                End If
                If chkWednesday.Checked Then
                    daysOfWeekSetting += DaysOfTheWeek.Wednesday
                End If
                If chkThursday.Checked Then
                    daysOfWeekSetting += DaysOfTheWeek.Thursday
                End If
                If chkFriday.Checked Then
                    daysOfWeekSetting += DaysOfTheWeek.Friday
                End If
                If chkSaturday.Checked Then
                    daysOfWeekSetting += DaysOfTheWeek.Saturday
                End If

                newTask.Triggers.Add(New WeeklyTrigger() With {.StartBoundary = timePicker.Value, .DaysOfWeek = daysOfWeekSetting})
            End If

            newTask.Principal.UserId = "NT AUTHORITY\System"

            newTask.Principal.RunLevel = TaskRunLevel.Highest
            newTask.Settings.Compatibility = TaskCompatibility.V2
            newTask.Settings.AllowDemandStart = True
            newTask.Settings.AllowHardTerminate = False
            newTask.Settings.StartWhenAvailable = chkRunMissedTask.Checked

            newTask.Settings.DisallowStartIfOnBatteries = False
            newTask.Settings.RunOnlyIfIdle = False
            newTask.Settings.StopIfGoingOnBatteries = False
            newTask.Settings.ExecutionTimeLimit = Nothing
            newTask.Settings.IdleSettings.StopOnIdleEnd = False
            newTask.Settings.WakeToRun = chkWake.Checked

            If chkWake.Checked = True Then
                Functions.power.checkIfActivePowerPlanIsSetProperlyForWakingFromSleep()
            End If

            Functions.eventLogFunctions.writeToSystemEventLog(String.Format("Creating scheduled task ""{0}"".", strCheckPointTaskName))
            taskService.RootFolder.RegisterTaskDefinition(strCheckPointTaskName, newTask, TaskCreation.CreateOrUpdate, "SYSTEM", vbNullString, TaskLogonType.ServiceAccount)

            newTask.Dispose()
            newTask = Nothing

            btnDeleteTask.Enabled = True

            If chkDeleteOldRestorePoints.Checked Then
                ' This checks to see if the user inputted an Integer and not a String.
                If Functions.support.isNumeric(txtDays.Text.Trim) = False Then
                    txtDays.Text = 10
                End If

                Dim regKey As Microsoft.Win32.RegistryKey = Microsoft.Win32.Registry.LocalMachine.OpenSubKey(globalVariables.registryValues.strKey, True)
                regKey.SetValue("MaxDays", Short.Parse(txtDays.Text.Trim), Microsoft.Win32.RegistryValueKind.String)
                regKey.SetValue(strDeleteTaskName, "True", Microsoft.Win32.RegistryValueKind.String)
                regKey.Close()
            Else
                Microsoft.Win32.Registry.LocalMachine.OpenSubKey(globalVariables.registryValues.strKey, True).SetValue(strDeleteTaskName, "False", Microsoft.Win32.RegistryValueKind.String)
            End If

            boolThingsChanged = False
            MsgBox("System Restore Point Creation Task Created.", MsgBoxStyle.Information, Me.Text)
        Catch ex2 As IO.FileNotFoundException
            Try
                If boolDoesTaskExist(strCheckPointTaskName) Then
                    taskService.RootFolder.DeleteTask(strCheckPointTaskName)
                End If
            Catch ex As Exception
            End Try

            MsgBox("There was an error creating/updating your task, please try again.", MsgBoxStyle.Critical, Me.Text)
        Catch ex As Exception
            Threading.Thread.CurrentThread.CurrentUICulture = New System.Globalization.CultureInfo("en-US")
            exceptionHandler.manuallyLoadCrashWindow(ex, ex.Message, ex.StackTrace, ex.GetType)
        Finally
            taskService.Dispose()
            taskService = Nothing
        End Try
    End Sub

    Private Sub btnDeleteTask_Click(sender As Object, e As EventArgs) Handles btnDeleteTask.Click
        Try
            Dim taskService As New TaskService

            For Each task As Task In taskService.RootFolder.Tasks
                If task.Name = strCheckPointTaskName Then
                    taskService.RootFolder.DeleteTask(strCheckPointTaskName)
                End If

                task.Dispose()
                task = Nothing
            Next

            taskService.Dispose()
            taskService = Nothing

            radDaily.Checked = False
            radWeekly.Checked = False
            grpDaysOfTheWeek.Enabled = False
            btnDeleteTask.Enabled = False

            boolThingsChanged = False
            MsgBox("Task deleted.", MsgBoxStyle.Information, Me.Text)
        Catch ex As Exception
            Threading.Thread.CurrentThread.CurrentUICulture = New Globalization.CultureInfo("en-US")
            exceptionHandler.manuallyLoadCrashWindow(ex, ex.Message, ex.StackTrace, ex.GetType)
        End Try
    End Sub

    Private Sub chkDeleteOldRestorePoints_Click(sender As Object, e As EventArgs) Handles chkDeleteOldRestorePoints.Click
        Try
            tellTheProgramThingsChanged()

            If chkDeleteOldRestorePoints.Checked Then
                txtDays.Text = Microsoft.Win32.Registry.LocalMachine.OpenSubKey(globalVariables.registryValues.strKey, False).GetValue("MaxDays", 15).ToString

                ' Checks for valid data.
                If Functions.support.isNumeric(txtDays.Text) = False Then
                    txtDays.Text = 15 ' Nope, so we replace it with valid data.
                End If

                txtDays.Enabled = True
                btnSet.Enabled = True
                deleteOldRestorePointsTaskSettings.Enabled = False

                Dim taskService As New TaskService
                For Each task As Task In taskService.RootFolder.Tasks
                    If task.Name = strDeleteTaskName Then
                        taskService.RootFolder.DeleteTask(strDeleteTaskName)
                    End If
                Next
                taskService.Dispose()
                taskService = Nothing

                radDailyDelete.Checked = False
                radWeeklyDelete.Checked = False

                chkFridayDelete.Checked = False
                chkMondayDelete.Checked = False
                chkSaturdayDelete.Checked = False
                chkSundayDelete.Checked = False
                chkThursdayDelete.Checked = False
                chkTuesdayDelete.Checked = False
                chkWednesdayDelete.Checked = False
            Else
                txtDays.Enabled = False
                btnSet.Enabled = False
                deleteOldRestorePointsTaskSettings.Enabled = True
            End If

            Microsoft.Win32.Registry.LocalMachine.OpenSubKey(globalVariables.registryValues.strKey, True).SetValue(strDeleteTaskName, chkDeleteOldRestorePoints.Checked.ToString, Microsoft.Win32.RegistryValueKind.String)

            My.Settings.deleteOldRestorePoints2 = chkDeleteOldRestorePoints.Checked
            My.Settings.Save()
        Catch ex As Exception
            Threading.Thread.CurrentThread.CurrentUICulture = New System.Globalization.CultureInfo("en-US")
            exceptionHandler.manuallyLoadCrashWindow(ex, ex.Message, ex.StackTrace, ex.GetType)
        End Try
    End Sub

    Private Sub txtDays_KeyUp(sender As Object, e As KeyEventArgs) Handles txtDays.KeyUp
        If e.KeyCode = Keys.Enter Then
            btnSet.PerformClick()
        End If
    End Sub

    Private Sub txtDays_Leave(sender As Object, e As EventArgs) Handles txtDays.Leave
        ' This checks to see if the user inputted an Integer and not a String.
        If Functions.support.isNumeric(txtDays.Text) = False Then
            txtDays.Text = 10
        End If
    End Sub

    Private Sub btnSet_Click(sender As Object, e As EventArgs) Handles btnSet.Click
        Dim shortDays As Short
        If Short.TryParse(txtDays.Text, shortDays) = True Then
            Microsoft.Win32.Registry.LocalMachine.OpenSubKey(globalVariables.registryValues.strKey, True).SetValue("MaxDays", shortDays.ToString, Microsoft.Win32.RegistryValueKind.String)

            MsgBox("Max Age Setting Saved.", MsgBoxStyle.Information, "Setting Saved.")
        Else
            MsgBox("Invalid input for the days setting.", MsgBoxStyle.Critical, Me.Text)
        End If
    End Sub

    Private Sub btnSaveTaskDelete_Click(sender As Object, e As EventArgs) Handles btnSaveTaskDelete.Click
        Dim taskService As New TaskService

        Try
            ' This checks to see if the user inputted an Integer and not a String.
            If Functions.support.isNumeric(txtDaysDelete.Text.Trim) = False Then
                txtDaysDelete.Text = 10
            End If

            If radWeeklyDelete.Checked = True And chkSundayDelete.Checked = False And chkMondayDelete.Checked = False And chkTuesdayDelete.Checked = False And chkWednesdayDelete.Checked = False And chkThursdayDelete.Checked = False And chkFridayDelete.Checked = False And chkSaturdayDelete.Checked = False Then
                MsgBox("You must select days of the week for your weekly schedule.", MsgBoxStyle.Information, Me.Text)
                Exit Sub
            End If

            Microsoft.Win32.Registry.LocalMachine.OpenSubKey(globalVariables.registryValues.strKey, True).SetValue("MaxDays", Short.Parse(txtDays.Text.Trim), Microsoft.Win32.RegistryValueKind.String)

            If radDailyDelete.Checked = False And radWeeklyDelete.Checked = False Then
                MsgBox("You must select a schedule type.", MsgBoxStyle.Information, Me.Text)
                Exit Sub
            End If

            Dim newTask As TaskDefinition = taskService.NewTask
            newTask.RegistrationInfo.Description = strDeleteTaskName

            Dim exePathInfo As New IO.FileInfo(Application.ExecutablePath)
            newTask.Actions.Add(New ExecAction(Chr(34) & exePathInfo.FullName & Chr(34), "-deleteoldrestorepoints", exePathInfo.DirectoryName))
            exePathInfo = Nothing

            Try
                If radDailyDelete.Checked Then
                    Dim trigger As Trigger = newTask.Triggers.Add(New DailyTrigger(1))
                    trigger.StartBoundary = timePicker.Value
                ElseIf radWeeklyDelete.Checked Then
                    Dim daysOfWeekSetting As DaysOfTheWeek

                    If chkSundayDelete.Checked Then
                        daysOfWeekSetting += DaysOfTheWeek.Sunday
                    End If
                    If chkMondayDelete.Checked Then
                        daysOfWeekSetting += DaysOfTheWeek.Monday
                    End If
                    If chkTuesdayDelete.Checked Then
                        daysOfWeekSetting += DaysOfTheWeek.Tuesday
                    End If
                    If chkWednesdayDelete.Checked Then
                        daysOfWeekSetting += DaysOfTheWeek.Wednesday
                    End If
                    If chkThursdayDelete.Checked Then
                        daysOfWeekSetting += DaysOfTheWeek.Thursday
                    End If
                    If chkFridayDelete.Checked Then
                        daysOfWeekSetting += DaysOfTheWeek.Friday
                    End If
                    If chkSaturdayDelete.Checked Then
                        daysOfWeekSetting += DaysOfTheWeek.Saturday
                    End If

                    newTask.Triggers.Add(New WeeklyTrigger() With {.StartBoundary = timePickerDelete.Value, .DaysOfWeek = daysOfWeekSetting})
                End If
            Catch ex As Exception
                Threading.Thread.CurrentThread.CurrentUICulture = New Globalization.CultureInfo("en-US")
                exceptionHandler.manuallyLoadCrashWindow(ex, ex.Message, ex.StackTrace, ex.GetType)
            End Try

            newTask.Principal.UserId = "NT AUTHORITY\System"

            newTask.Principal.RunLevel = TaskRunLevel.Highest
            newTask.Settings.Compatibility = TaskCompatibility.V2
            newTask.Settings.AllowDemandStart = True
            newTask.Settings.AllowHardTerminate = False
            newTask.Settings.StartWhenAvailable = chkRunMissedTaskDelete.Checked

            newTask.Settings.DisallowStartIfOnBatteries = False
            newTask.Settings.RunOnlyIfIdle = False
            newTask.Settings.StopIfGoingOnBatteries = False
            newTask.Settings.ExecutionTimeLimit = Nothing
            newTask.Settings.IdleSettings.StopOnIdleEnd = False
            newTask.Settings.WakeToRun = chkWakeDelete.Checked

            If chkWakeDelete.Checked = True Then
                Functions.power.checkIfActivePowerPlanIsSetProperlyForWakingFromSleep()
            End If

            Functions.eventLogFunctions.writeToSystemEventLog(String.Format("Creating scheduled task ""{0}"".", strDeleteTaskName))
            taskService.RootFolder.RegisterTaskDefinition(strDeleteTaskName, newTask, TaskCreation.CreateOrUpdate, "SYSTEM", vbNullString, TaskLogonType.ServiceAccount)

            newTask.Dispose()
            newTask = Nothing

            btnDeleteTaskDelete.Enabled = True

            boolThingsChanged = False
            MsgBox("Delete Old System Restore Points Task Created.", MsgBoxStyle.Information, Me.Text)
        Catch ex2 As IO.FileNotFoundException
            Try
                If boolDoesTaskExist(strDeleteTaskName) Then
                    taskService.RootFolder.DeleteTask(strDeleteTaskName)
                End If
            Catch ex As Exception
            End Try

            MsgBox("There was an error creating/updating your task, please try again.", MsgBoxStyle.Critical, Me.Text)
        Catch ex As Exception
            Threading.Thread.CurrentThread.CurrentUICulture = New Globalization.CultureInfo("en-US")
            exceptionHandler.manuallyLoadCrashWindow(ex, ex.Message, ex.StackTrace, ex.GetType)
        Finally
            taskService.Dispose()
            taskService = Nothing
        End Try
    End Sub

    Private Sub btnDeleteTaskDelete_Click(sender As Object, e As EventArgs) Handles btnDeleteTaskDelete.Click
        Try
            Dim taskService As New TaskService

            For Each task As Task In taskService.RootFolder.Tasks
                If task.Name = strDeleteTaskName Then
                    taskService.RootFolder.DeleteTask(strDeleteTaskName)
                End If

                task.Dispose()
                task = Nothing
            Next

            taskService.Dispose()
            taskService = Nothing

            radDailyDelete.Checked = False
            radWeeklyDelete.Checked = False
            grpDaysOfTheWeekDelete.Enabled = False
            btnDeleteTaskDelete.Enabled = False

            boolThingsChanged = False
            MsgBox("Task deleted.", MsgBoxStyle.Information, Me.Text)
        Catch ex As Exception
            Threading.Thread.CurrentThread.CurrentUICulture = New Globalization.CultureInfo("en-US")
            exceptionHandler.manuallyLoadCrashWindow(ex, ex.Message, ex.StackTrace, ex.GetType)
        End Try
    End Sub

    Private Sub radDailyDelete_CheckedChanged(sender As Object, e As EventArgs) Handles radDailyDelete.CheckedChanged
        grpDaysOfTheWeekDelete.Enabled = False
    End Sub

    Private Sub radWeeklyDelete_CheckedChanged(sender As Object, e As EventArgs) Handles radWeeklyDelete.CheckedChanged
        grpDaysOfTheWeekDelete.Enabled = True
    End Sub

    Private Sub btnSetCustomName_Click(sender As Object, e As EventArgs) Handles btnSetCustomName.Click
        Dim setCustomRestorePointNameForScheduledRestorePointsInstance As New Set_Custom_Restore_Point_Name_for_Scheduled_Restore_Points With {
            .StartPosition = FormStartPosition.CenterParent
        }
        setCustomRestorePointNameForScheduledRestorePointsInstance.ShowDialog()
    End Sub

    Private Sub radDaily_Click(sender As Object, e As EventArgs) Handles radDaily.Click
        tellTheProgramThingsChanged()
    End Sub

    Private Sub radDailyDelete_Click(sender As Object, e As EventArgs) Handles radDailyDelete.Click
        tellTheProgramThingsChanged()
    End Sub

    Private Sub radEvery_Click(sender As Object, e As EventArgs) Handles radEvery.Click
        tellTheProgramThingsChanged()
    End Sub

    Private Sub radWeekly_Click(sender As Object, e As EventArgs) Handles radWeekly.Click
        tellTheProgramThingsChanged()
    End Sub

    Private Sub radWeeklyDelete_Click(sender As Object, e As EventArgs) Handles radWeeklyDelete.Click
        tellTheProgramThingsChanged()
    End Sub

    Private Sub chkFriday_Click(sender As Object, e As EventArgs) Handles chkFriday.Click
        tellTheProgramThingsChanged()
    End Sub

    Private Sub chkFridayDelete_Click(sender As Object, e As EventArgs) Handles chkFridayDelete.Click
        tellTheProgramThingsChanged()
    End Sub

    Private Sub chkMonday_Click(sender As Object, e As EventArgs) Handles chkMonday.Click
        tellTheProgramThingsChanged()
    End Sub

    Private Sub chkMondayDelete_Click(sender As Object, e As EventArgs) Handles chkMondayDelete.Click
        tellTheProgramThingsChanged()
    End Sub

    Private Sub chkRunMissedTask_Click(sender As Object, e As EventArgs) Handles chkRunMissedTask.Click
        tellTheProgramThingsChanged()
    End Sub

    Private Sub chkRunMissedTaskDelete_Click(sender As Object, e As EventArgs) Handles chkRunMissedTaskDelete.Click
        tellTheProgramThingsChanged()
    End Sub

    Private Sub chkSaturday_Click(sender As Object, e As EventArgs) Handles chkSaturday.Click
        tellTheProgramThingsChanged()
    End Sub

    Private Sub chkSaturdayDelete_Click(sender As Object, e As EventArgs) Handles chkSaturdayDelete.Click
        tellTheProgramThingsChanged()
    End Sub

    Private Sub chkSunday_Click(sender As Object, e As EventArgs) Handles chkSunday.Click
        tellTheProgramThingsChanged()
    End Sub

    Private Sub chkSundayDelete_Click(sender As Object, e As EventArgs) Handles chkSundayDelete.Click
        tellTheProgramThingsChanged()
    End Sub

    Private Sub chkThursday_Click(sender As Object, e As EventArgs) Handles chkThursday.Click
        tellTheProgramThingsChanged()
    End Sub

    Private Sub chkThursdayDelete_Click(sender As Object, e As EventArgs) Handles chkThursdayDelete.Click
        tellTheProgramThingsChanged()
    End Sub

    Private Sub chkTuesday_Click(sender As Object, e As EventArgs) Handles chkTuesday.Click
        tellTheProgramThingsChanged()
    End Sub

    Private Sub chkTuesdayDelete_Click(sender As Object, e As EventArgs) Handles chkTuesdayDelete.Click
        tellTheProgramThingsChanged()
    End Sub

    Private Sub chkWake_Click(sender As Object, e As EventArgs) Handles chkWake.Click
        tellTheProgramThingsChanged()
    End Sub

    Private Sub chkWakeDelete_Click(sender As Object, e As EventArgs) Handles chkWakeDelete.Click
        tellTheProgramThingsChanged()
    End Sub

    Private Sub chkWednesday_Click(sender As Object, e As EventArgs) Handles chkWednesday.Click
        tellTheProgramThingsChanged()
    End Sub

    Private Sub chkWednesdayDelete_Click(sender As Object, e As EventArgs) Handles chkWednesdayDelete.Click
        tellTheProgramThingsChanged()
    End Sub

    Private Sub timePicker_ValueChanged(sender As Object, e As EventArgs) Handles timePicker.ValueChanged
        tellTheProgramThingsChanged()
    End Sub

    Private Sub timePickerDelete_ValueChanged(sender As Object, e As EventArgs) Handles timePickerDelete.ValueChanged
        tellTheProgramThingsChanged()
    End Sub

    Sub tellTheProgramThingsChanged()
        If boolDoneLoading = True Then boolThingsChanged = True
    End Sub
End Class