Public Class NotSupportedWindow
    Private Sub btnClose_Click(sender As Object, e As EventArgs) Handles btnClose.Click
        Me.Close()
    End Sub

    Private Sub NotSupported_FormClosing(sender As Object, e As FormClosingEventArgs) Handles Me.FormClosing
        My.Settings.boolDontBugMeAboutNoSupport = chkNoSupport.Checked
    End Sub
End Class