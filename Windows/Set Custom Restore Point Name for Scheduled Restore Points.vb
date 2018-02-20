Public Class Set_Custom_Restore_Point_Name_for_Scheduled_Restore_Points
    Private Sub btnOK_Click(sender As Object, e As EventArgs) Handles btnOK.Click
        If String.IsNullOrWhiteSpace(txtRestorePointName.Text.Trim) = False Then
            Microsoft.Win32.Registry.LocalMachine.OpenSubKey(globalVariables.registryValues.strKey, True).SetValue("Custom Name for Scheduled Restore Points", txtRestorePointName.Text, Microsoft.Win32.RegistryValueKind.String)
        End If

        Me.Close()
    End Sub

    Private Sub Set_Custom_Restore_Point_Name_for_Scheduled_Restore_Points_Load(sender As Object, e As EventArgs) Handles Me.Load
        txtRestorePointName.Text = Microsoft.Win32.Registry.LocalMachine.OpenSubKey(globalVariables.registryValues.strKey, True).GetValue("Custom Name for Scheduled Restore Points", "")
    End Sub

    Private Sub btnCancel_Click(sender As Object, e As EventArgs) Handles btnCancel.Click
        Me.Close()
    End Sub

    Private Sub btnDelete_Click(sender As Object, e As EventArgs) Handles btnDelete.Click
        Microsoft.Win32.Registry.LocalMachine.OpenSubKey(globalVariables.registryValues.strKey, True).DeleteValue("Custom Name for Scheduled Restore Points", False)
        Me.Close()
    End Sub

    Private Sub txtRestorePointName_KeyUp(sender As Object, e As KeyEventArgs) Handles txtRestorePointName.KeyUp
        If e.KeyCode = Keys.Enter Then btnOK.PerformClick()
    End Sub
End Class