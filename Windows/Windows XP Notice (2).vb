Public Class Windows_XP_Notice
    Private Sub Windows_XP_Notice_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        PictureBox1.Image = System.Drawing.SystemIcons.Information.ToBitmap()
    End Sub

    Private Sub btnOK_Click(sender As Object, e As EventArgs) Handles btnOK.Click
        Me.Close()
    End Sub

    Private Sub btnStopBuggingMe_Click(sender As Object, e As EventArgs) Handles btnStopBuggingMe.Click
        My.Settings.bugUserAboutStillBeingOnAncientWindowsXP = False
        Me.Close()
    End Sub
End Class