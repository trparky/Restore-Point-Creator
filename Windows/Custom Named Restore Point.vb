Public Class Custom_Named_Restore_Point
    Public restorePointName As String
    Public createRestorePoint As Boolean

    Private Sub btnOK_Click(sender As Object, e As EventArgs) Handles btnOK.Click
        If txtRestorePointName.Text.Trim = Nothing Then
            MsgBox("You did not input a name for your Restore Point.", MsgBoxStyle.Information, "Restore Point Creator")
        Else
            restorePointName = txtRestorePointName.Text.Trim
            createRestorePoint = True
            Me.Close()
        End If
    End Sub

    Private Sub btnCancel_Click(sender As Object, e As EventArgs) Handles btnCancel.Click
        createRestorePoint = False
        Me.Close()
    End Sub

    Private Sub txtRestorePointName_KeyUp(sender As Object, e As KeyEventArgs) Handles txtRestorePointName.KeyUp
        If e.KeyCode = Keys.Enter Then
            btnOK.PerformClick()
        End If
    End Sub

    Private Sub Custom_Named_Restore_Point_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        txtRestorePointName.Text = Microsoft.Win32.Registry.LocalMachine.OpenSubKey(globalVariables.registryValues.strKey).GetValue("Default Custom Restore Point Name", "").ToString.Trim
    End Sub

    Private Sub Timer1_Tick(sender As Object, e As EventArgs) Handles Timer1.Tick
        Try
            AppActivate(Me.Text)
            AppActivate(Process.GetCurrentProcess.Id)
            Me.BringToFront()
            Me.Focus()
            Timer1.Enabled = False
        Catch ex As Exception
            ' Does nothing
        End Try
    End Sub
End Class