Imports Microsoft.Win32

Public Class Set_Custom_Restore_Point_Name_for_Scheduled_Restore_Points
    Private Sub btnOK_Click(sender As Object, e As EventArgs) Handles btnOK.Click
        If txtRestorePointName.Text.Trim <> "" Then
            Registry.LocalMachine.OpenSubKey(globalVariables.registryValues.strKey, True).SetValue("Custom Name for Scheduled Restore Points", txtRestorePointName.Text, RegistryValueKind.String)
        End If

        Me.Close()
    End Sub

    Private Sub Set_Custom_Restore_Point_Name_for_Scheduled_Restore_Points_Load(sender As Object, e As EventArgs) Handles Me.Load
        txtRestorePointName.Text = Registry.LocalMachine.OpenSubKey(globalVariables.registryValues.strKey, True).GetValue("Custom Name for Scheduled Restore Points", "")
    End Sub

    Private Sub btnCancel_Click(sender As Object, e As EventArgs) Handles btnCancel.Click
        Me.Close()
    End Sub

    Private Sub btnDelete_Click(sender As Object, e As EventArgs) Handles btnDelete.Click
        Registry.LocalMachine.OpenSubKey(globalVariables.registryValues.strKey, True).DeleteValue("Custom Name for Scheduled Restore Points", False)
        Me.Close()
    End Sub

    Private Sub txtRestorePointName_KeyUp(sender As Object, e As KeyEventArgs) Handles txtRestorePointName.KeyUp
        If e.KeyCode = Keys.Enter Then
            btnOK.PerformClick()
        End If
    End Sub
End Class