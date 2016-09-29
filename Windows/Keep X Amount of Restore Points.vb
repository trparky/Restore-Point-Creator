Imports Microsoft.Win32

Public Class createRestorePointAtUserLogon
    Public parentFormG As Form1

    Private Sub btnOK_Click(sender As Object, e As EventArgs) Handles btnOK.Click
        Try
            Dim shortMaxNumber As Short

            If Short.TryParse(txtMaxNumber.Text.Trim, shortMaxNumber) = True Then
                Registry.LocalMachine.OpenSubKey(globalVariables.registryValues.strKey, True).SetValue("Keep X Amount of Restore Points Value", shortMaxNumber.ToString, RegistryValueKind.String)
                Registry.LocalMachine.OpenSubKey(globalVariables.registryValues.strKey, True).SetValue("Keep X Amount of Restore Points", "True", RegistryValueKind.String)

                globalVariables.KeepXAmountOfRestorePoints = True
                globalVariables.KeepXAmountofRestorePointsValue = shortMaxNumber

                parentFormG.KeepXAmountOfRestorePointsToolStripMenuItem.Text = String.Format("Keep X Amount of Restore Points ({0})", shortMaxNumber.ToString)
                Me.Close()
            End If
        Catch ex As OverflowException
            MsgBox("You have entered in a number that's too large. Please try again.", MsgBoxStyle.Critical, Me.Text)
        End Try
    End Sub

    Private Sub txtMaxNumber_KeyUp(sender As Object, e As KeyEventArgs) Handles txtMaxNumber.KeyUp
        If e.KeyCode = Keys.Enter Then
            btnOK.PerformClick()
        End If
    End Sub

    Private Sub btnCancel_Click(sender As Object, e As EventArgs) Handles btnCancel.Click
        parentFormG.KeepXAmountOfRestorePointsToolStripMenuItem.Checked = False
        Me.Close()
    End Sub

    Private Sub Keep_X_Amount_of_Restore_Points_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        txtMaxNumber.Text = Microsoft.Win32.Registry.LocalMachine.OpenSubKey(globalVariables.registryValues.strKey, False).GetValue("Keep X Amount of Restore Points Value", Nothing)
    End Sub
End Class