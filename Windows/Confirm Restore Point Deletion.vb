Public Class frmConfirmDelete
    Public userResponse As userResponseENum
    Private boolUserResponded As Boolean = False

    Public Enum userResponseENum
        yes
        no
        cancel
        yesAndDontAskAgain
    End Enum

    Private Sub btnNo_Click(sender As Object, e As EventArgs) Handles btnNo.Click
        boolUserResponded = True
        userResponse = userResponseENum.no
        Me.Close()
    End Sub

    Private Sub btnYes_Click(sender As Object, e As EventArgs) Handles btnYes.Click
        boolUserResponded = True

        If chkDontAskAgain.Checked Then
            userResponse = userResponseENum.yesAndDontAskAgain
        Else
            userResponse = userResponseENum.yes
        End If

        Me.Close()
    End Sub

    Private Sub frmConfirmDelete_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        PictureBox1.Image = SystemIcons.Question.ToBitmap()

        Try
            Me.Size = New Size(lblRestorePointName.Width + 100, Me.Height)
            Me.BringToFront()
            Me.Focus()
        Catch ex As Exception
        End Try

        Media.SystemSounds.Exclamation.Play()
    End Sub

    Private Sub frmConfirmDelete_FormClosing(sender As Object, e As FormClosingEventArgs) Handles Me.FormClosing
        If boolUserResponded = False Then userResponse = userResponseENum.no
    End Sub

    Private Sub btnCancelDeletion_Click(sender As Object, e As EventArgs) Handles btnCancelDeletion.Click
        boolUserResponded = True
        userResponse = userResponseENum.cancel
        Me.Close()
    End Sub
End Class