Imports Microsoft.Win32.TaskScheduler

Public Class Create_Restore_Point_at_User_Logon
    Private boolThingsChanged As Boolean = False

    Private Sub chkDelayed_Click(sender As Object, e As EventArgs) Handles chkDelayed.Click
        If chkDelayed.Checked Then
            lblDelayed.Visible = True
            txtDelayed.Visible = True
        Else
            lblDelayed.Visible = False
            txtDelayed.Visible = False
        End If
    End Sub

    Private Sub btnCreateTask_Click(sender As Object, e As EventArgs) Handles btnCreateTask.Click
        Dim delayedTime As Short = 0

        If Functions.taskStuff.doesAtUserLoginTaskExist(delayedTime) = True Then
            Functions.taskStuff.deleteAtUserLogonTask()
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
        newTask.RegistrationInfo.Description = "Create a Restore Point at User Logon (" & Environment.UserName & ")"
        newTask.Principal.RunLevel = TaskRunLevel.Highest
        newTask.Principal.LogonType = TaskLogonType.InteractiveToken

        Dim exePathInfo As New IO.FileInfo(Application.ExecutablePath)
        newTask.Actions.Add(New ExecAction(exePathInfo.FullName, "-createscheduledrestorepoint", exePathInfo.DirectoryName))
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
        taskFolderObject.RegisterTaskDefinition("Create a Restore Point at User Logon (" & Environment.UserName & ")", newTask)

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
            Dim delayedTime As Short = 0

            If Functions.taskStuff.doesAtUserLoginTaskExist(delayedTime) = True Then
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

        If Functions.taskStuff.doesAtUserLoginTaskExist(delayedTime) = True Then
            Functions.taskStuff.deleteAtUserLogonTask()
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