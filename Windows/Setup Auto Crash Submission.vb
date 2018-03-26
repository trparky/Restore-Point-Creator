Public Class Setup_Auto_Crash_Submission
    Private Sub btnEnableAutoCrashSubmission_Click(sender As Object, e As EventArgs) Handles btnEnableAutoCrashSubmission.Click
        My.Settings.boolUserSetAutoCrashSubmission = True
        My.Settings.boolAutoCrashSubmissionEnabled = True
        My.Settings.usersName = txtName.Text
        My.Settings.usersEmail = txtEmail.Text
        My.Settings.boolSaveInfo = True
        Me.Close()
    End Sub

    Private Sub btnDisableAutoCrashSubmission_Click(sender As Object, e As EventArgs) Handles btnDisableAutoCrashSubmission.Click
        My.Settings.boolUserSetAutoCrashSubmission = True
        My.Settings.boolAutoCrashSubmissionEnabled = False
        Me.Close()
    End Sub

    Private Sub Setup_Auto_Crash_Submission_FormClosed(sender As Object, e As FormClosedEventArgs) Handles Me.FormClosed
        If e.CloseReason = CloseReason.UserClosing And Not My.Settings.boolUserSetAutoCrashSubmission Then
            My.Settings.boolUserSetAutoCrashSubmission = True
            My.Settings.boolAutoCrashSubmissionEnabled = False
            Me.Close()
        End If
    End Sub

    Private Sub Setup_Auto_Crash_Submission_Load(sender As Object, e As EventArgs) Handles Me.Load
        If My.Settings.boolSaveInfo Then
            txtName.Text = My.Settings.usersName
            txtEmail.Text = My.Settings.usersEmail
        End If
    End Sub
End Class