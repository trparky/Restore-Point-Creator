Public Class Restore_Point_Creator_Command_Line_Arguments
    Private Sub btnClose_Click(sender As Object, e As EventArgs) Handles btnClose.Click
        Me.Close()
    End Sub

    Private Sub btnCopyToClipboard_Click(sender As Object, e As EventArgs) Handles btnCopyToClipboard.Click
        Clipboard.SetText(Label1.Text)
        MsgBox("The above text has been copied to the Windows Clipboard.", MsgBoxStyle.Information, Me.Text)
    End Sub
End Class