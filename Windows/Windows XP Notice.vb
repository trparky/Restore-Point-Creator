﻿Public Class Windows_XP_Notice
    Private Sub Windows_XP_Notice_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        PictureBox1.Image = SystemIcons.Information.ToBitmap()
        Media.SystemSounds.Asterisk.Play()
    End Sub

    Private Sub btnOK_Click(sender As Object, e As EventArgs) Handles btnOK.Click
        Me.Close()
    End Sub
End Class