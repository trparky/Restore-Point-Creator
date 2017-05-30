Imports Microsoft.Win32.TaskScheduler

Public Class Create_Restore_Point_at_User_Logon
    Private boolThingsChanged As Boolean = False
    Private strTaskName As String = "Create a Restore Point at User Logon (" & Environment.UserName & ")"

    Private Sub chkDelayed_Click(sender As Object, e As EventArgs) Handles chkDelayed.Click
        If chkDelayed.Checked Then
            lblDelayed.Visible = True
            txtDelayed.Visible = True
        Else
            lblDelayed.Visible = False
            txtDelayed.Visible = False
        End If
    End Sub

    Private Sub renameOldTask()
        Try
            Using taskServiceObject As TaskService = New TaskService() ' Creates a new instance of the TaskService.
                Dim oldTaskObject As Task = taskServiceObject.GetTask(globalVariables.taskFolder & "\Create a Restore Point at User Logon") ' Gets the task.

                ' Makes sure that the task exists and we don't get a Null Reference Exception.
                If oldTaskObject IsNot Nothing Then
                    Dim newTask As TaskDefinition = taskServiceObject.NewTask

                    Dim logonTriggerDefinition As LogonTrigger = New LogonTrigger
                    logonTriggerDefinition.UserId = Security.Principal.WindowsIdentity.GetCurrent().Name

                    Dim triggerDelayMinutes As Integer = DirectCast(oldTaskObject.Definition.Triggers.Item(0), ITriggerDelay).Delay.Minutes
                    If triggerDelayMinutes <> 0 Then
                        logonTriggerDefinition.Delay = TimeSpan.FromMinutes(triggerDelayMinutes)
                    End If
                    triggerDelayMinutes = 0

                    newTask.Triggers.Add(logonTriggerDefinition)

                    Functions.taskStuff.deleteTask(oldTaskObject)

                    newTask.RegistrationInfo.Description = "Creates a Restore Point at User Logon for the user " & Environment.UserName & "."
                    newTask.Principal.RunLevel = TaskRunLevel.Highest
                    newTask.Principal.LogonType = TaskLogonType.InteractiveToken

                    Dim exePathInfo As New IO.FileInfo(Application.ExecutablePath)
                    newTask.Actions.Add(New ExecAction(exePathInfo.FullName, globalVariables.commandLineSwitches.scheduledRestorePoint, exePathInfo.DirectoryName))
                    exePathInfo = Nothing

                    With newTask.Settings
                        .Compatibility = TaskCompatibility.V2
                        .AllowDemandStart = True
                        .DisallowStartIfOnBatteries = False
                        .RunOnlyIfIdle = False
                        .StopIfGoingOnBatteries = False
                        .AllowHardTerminate = False
                        .ExecutionTimeLimit = Nothing
                    End With

                    If newTask.Validate() Then
                        Dim taskFolderObject As TaskFolder = Functions.taskStuff.getOurTaskFolder(taskServiceObject)
                        Functions.eventLogFunctions.writeToSystemEventLog(String.Format("Creating scheduled task ""{0}"" in ""{1}"".", strTaskName, taskFolderObject.Name))
                        taskFolderObject.RegisterTaskDefinition(strTaskName, newTask)

                        taskFolderObject.Dispose()
                        newTask.Dispose()
                        logonTriggerDefinition.Dispose()

                        taskFolderObject = Nothing
                        newTask = Nothing
                        logonTriggerDefinition = Nothing
                    End If
                End If
            End Using
        Catch ex As Exception
        End Try
    End Sub

    Private Function doesAtUserLoginTaskExist(ByRef delayedTime As Short) As Boolean
        Try
            Dim taskObject As Task

            Using taskServiceObject As TaskService = New TaskService() ' Creates a new instance of the TaskService.
                taskObject = taskServiceObject.GetTask(globalVariables.taskFolder & "\" & strTaskName) ' Gets the task.

                ' Makes sure that the task exists and we don't get a Null Reference Exception.
                If taskObject IsNot Nothing Then
                    ' Makes sure that we have some triggers to actually work with.
                    If taskObject.Definition.Triggers.Count > 0 Then
                        ' Good, we have some triggers to work with.
                        Dim trigger As Trigger = taskObject.Definition.Triggers.Item(0) ' Stores the trigger object in a Trigger type variable.

                        ' Checks to see if the trigger type is a user logon type.
                        If trigger.TriggerType = TaskTriggerType.Logon Then
                            ' Yes, it is. So we go on.

                            ' Checks to see if the trigger is a delayed trigger.
                            If (TypeOf trigger Is ITriggerDelay) Then
                                ' Yes, it is. So we go on.

                                ' Gets the delayed time and stores it in the ByRef delayedTime which is a pointer to a variable that's passed to this function.
                                delayedTime = DirectCast(taskObject.Definition.Triggers.Item(0), ITriggerDelay).Delay.Minutes
                            Else
                                ' No, it's not a delayed trigger so we have to give the delayedTime pointer variable as value of 0.
                                delayedTime = 0
                            End If

                            ' OK, so we know that the trigger is a At User Logon type so this is a valid task so we return True.
                            Return True
                        Else
                            ' The trigger type doesn't match what we are looking for so this is an invalid task and so we give the delayedTime pointer variable a value of 0 and return False.
                            delayedTime = 0
                            Return False
                        End If
                    Else
                        ' We don't even have a trigger so this is an invalid task and so we give the delayedTime pointer variable a value of 0 and return False.
                        delayedTime = 0
                        Return False
                    End If
                Else
                    ' The task doesn't exist so we give the delayedTime pointer variable a value of 0 and return False.
                    delayedTime = 0
                    Return False
                End If
            End Using
        Catch ex As Exception
            ' Something we went wrong so we give the delayedTime pointer variable a value of 0 and return False.
            delayedTime = 0
            Return False
        End Try
    End Function

    Private Sub deleteAtUserLogonTask()
        Try
            Dim taskService As New TaskService
            Dim taskFolderObject As TaskFolder = Functions.taskStuff.getOurTaskFolder(taskService)

            taskFolderObject.DeleteTask(strTaskName, False)

            taskFolderObject.Dispose()
            taskService.Dispose()

            taskFolderObject = Nothing
            taskService = Nothing
        Catch ex As Exception
            Threading.Thread.CurrentThread.CurrentUICulture = New Globalization.CultureInfo("en-US")
            exceptionHandler.manuallyLoadCrashWindow(ex, ex.Message, ex.StackTrace, ex.GetType)
        End Try
    End Sub

    Private Sub btnCreateTask_Click(sender As Object, e As EventArgs) Handles btnCreateTask.Click
        Dim delayedTime As Short = 0

        If doesAtUserLoginTaskExist(delayedTime) = True Then
            deleteAtUserLogonTask()
        End If

        If chkDelayed.Checked Then
            If Short.TryParse(txtDelayed.Text, delayedTime) Then
                addAtUserLoginTask(chkDelayed.Checked, delayedTime)
            Else
                addAtUserLoginTask()
            End If
        Else
            addAtUserLoginTask()
        End If

        btnDeleteTask.Enabled = True
        boolThingsChanged = False

        lblIndication.Text = "Create Restore Point at User Login: Enabled"

        MsgBox("The task to create Restore Points at user logon has been created.", MsgBoxStyle.Information, Me.Text)
    End Sub

    Private Sub addAtUserLoginTask(Optional delayed As Boolean = False, Optional delayTimeInMinutes As Short = 10)
        Dim taskService As New TaskService()
        Dim newTask As TaskDefinition = taskService.NewTask
        Dim logonTriggerDefinition As LogonTrigger = New LogonTrigger

        logonTriggerDefinition.UserId = Security.Principal.WindowsIdentity.GetCurrent().Name

        If delayed = True Then
            logonTriggerDefinition.Delay = TimeSpan.FromMinutes(delayTimeInMinutes)
        End If

        newTask.Triggers.Add(logonTriggerDefinition)
        newTask.RegistrationInfo.Description = "Creates a Restore Point at User Logon for the user " & Environment.UserName & "."
        newTask.Principal.RunLevel = TaskRunLevel.Highest
        newTask.Principal.LogonType = TaskLogonType.InteractiveToken

        Dim exePathInfo As New IO.FileInfo(Application.ExecutablePath)
        newTask.Actions.Add(New ExecAction(exePathInfo.FullName, globalVariables.commandLineSwitches.scheduledRestorePoint, exePathInfo.DirectoryName))
        exePathInfo = Nothing

        With newTask.Settings
            .Compatibility = TaskCompatibility.V2
            .AllowDemandStart = True
            .DisallowStartIfOnBatteries = False
            .RunOnlyIfIdle = False
            .StopIfGoingOnBatteries = False
            .AllowHardTerminate = False
            .ExecutionTimeLimit = Nothing
        End With

        Try
            newTask.Validate(True)
        Catch ex As InvalidOperationException
            newTask.Dispose()
            taskService.Dispose()

            Functions.eventLogFunctions.writeToSystemEventLog("There was an error while validating the task definition settings.", EventLogEntryType.Error)
            Functions.eventLogFunctions.writeCrashToEventLog(ex)

            MsgBox("There was an error while validating the task definition settings. Please see the Application Event Log for more details.", MsgBoxStyle.Critical, Me.Text)
            Exit Sub
        End Try

        Dim taskFolderObject As TaskFolder = Functions.taskStuff.getOurTaskFolder(taskService)
        Functions.eventLogFunctions.writeToSystemEventLog(String.Format("Creating scheduled task ""{0}"" in ""{1}"".", strTaskName, taskFolderObject.Name))
        taskFolderObject.RegisterTaskDefinition(strTaskName, newTask)

        taskFolderObject.Dispose()
        newTask.Dispose()
        taskService.Dispose()

        taskFolderObject = Nothing
        newTask = Nothing
        taskService = Nothing
    End Sub

    Private Sub Create_Restore_Point_at_User_Logon_FormClosing(sender As Object, e As FormClosingEventArgs) Handles Me.FormClosing
        If boolThingsChanged = True Then
            Dim msgBoxResult As MsgBoxResult = MsgBox("It seems that you have changed some things. Remember, you have to click the ""Make Restore Points at User Logon"" button after changing settings." & vbCrLf & vbCrLf & "Are you sure you want to abandon these changes?", MsgBoxStyle.Question + MsgBoxStyle.YesNo, Me.Text)

            If msgBoxResult = Microsoft.VisualBasic.MsgBoxResult.No Then
                e.Cancel = True
                Exit Sub
            End If
        End If

        globalVariables.windows.frmCreateRestorePointAtUserLogon.Dispose()
        globalVariables.windows.frmCreateRestorePointAtUserLogon = Nothing
    End Sub

    Private Sub Create_Restore_Point_at_User_Logon_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        Try
            renameOldTask()

            Dim delayedTime As Short = 0
            If doesAtUserLoginTaskExist(delayedTime) = True Then
                btnDeleteTask.Enabled = True

                If delayedTime <> 0 Then
                    txtDelayed.Text = delayedTime
                    txtDelayed.Visible = True
                    lblDelayed.Visible = True
                    chkDelayed.Checked = True
                End If

                lblIndication.Text = "Create Restore Point at User Login: Enabled"
            Else
                lblIndication.Text = "Create Restore Point at User Login: Disabled"
                btnDeleteTask.Enabled = False
            End If

            boolThingsChanged = False
        Catch ex As Exception
            ' Does nothing
        End Try
    End Sub

    Private Sub btnDeleteTask_Click(sender As Object, e As EventArgs) Handles btnDeleteTask.Click
        Dim delayedTime As Short = 0

        If doesAtUserLoginTaskExist(delayedTime) = True Then
            deleteAtUserLogonTask()
            btnDeleteTask.Enabled = False
            boolThingsChanged = False
            lblIndication.Text = "Create Restore Point at User Login: Disabled"
            MsgBox("Restore Point Creator will now no longer be making restore points as user logon.", MsgBoxStyle.Information, Me.Text)
        End If
    End Sub

    Private Sub txtDelayed_TextChanged(sender As Object, e As EventArgs) Handles txtDelayed.TextChanged
        boolThingsChanged = True
    End Sub

    Private Sub chkDelayed_CheckedChanged(sender As Object, e As EventArgs) Handles chkDelayed.CheckedChanged
        boolThingsChanged = True
    End Sub
End Class