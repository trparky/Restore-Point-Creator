Imports Microsoft.Win32

Public Class Set_Default_Custom_Restore_Point_Name
    Public parentFormG As Form1

    Private Sub Set_Default_Custom_Restore_Point_Name_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        txtName.Text = Microsoft.Win32.Registry.LocalMachine.OpenSubKey(globalVariables.registryValues.strKey).GetValue("Default Custom Restore Point Name", "").ToString.Trim
    End Sub

    Private Sub txtName_KeyUp(sender As Object, e As KeyEventArgs) Handles txtName.KeyUp
        If e.KeyCode = Keys.Enter Then btnOK.PerformClick()
    End Sub

    Private Sub btnOK_Click(sender As Object, e As EventArgs) Handles btnOK.Click
        Microsoft.Win32.Registry.LocalMachine.OpenSubKey(globalVariables.registryValues.strKey, True).SetValue("Default Custom Restore Point Name", txtName.Text, RegistryValueKind.String)
        parentFormG.defaultCustomRestorePointName = txtName.Text
        parentFormG.btnCreateRestorePointNameWithDefaultName.Visible = True
        Me.Close()
    End Sub

    Private Sub btnCancel_Click(sender As Object, e As EventArgs) Handles btnCancel.Click
        Me.Close()
    End Sub

    Private Sub btnDelete_Click(sender As Object, e As EventArgs) Handles btnDelete.Click
        Try
            Microsoft.Win32.Registry.LocalMachine.OpenSubKey(globalVariables.registryValues.strKey, True).DeleteValue("Default Custom Restore Point Name", False)
        Catch ex As Exception
        End Try

        parentFormG.defaultCustomRestorePointName = Nothing
        parentFormG.btnCreateRestorePointNameWithDefaultName.Visible = False
        parentFormG.txtRestorePointDescription.Text = Nothing
        parentFormG.doTheGrayingOfTheRestorePointNameTextBox()

        Me.Close()
    End Sub
End Class