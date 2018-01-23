Public Class Round_Restore_Point_Age_in_Days
    Public Enum userResponse
        cancel = 0
        ok = 1
        nullSetting = 3
    End Enum

    Public dialogResponse As userResponse = userResponse.nullSetting
    Private boolButtonPushed As Boolean = False

    Private Sub btnOK_Click(sender As Object, e As EventArgs) Handles btnOK.Click
        Dim shortRound As Short

        If Short.TryParse(txtRound.Text, shortRound) Then
            If shortRound >= 5 Then
                MsgBox("Inputting a number greater than or equal to five is not a recommended setting.", MsgBoxStyle.Information, "Restore Point Creator")
                Exit Sub
            End If

            My.Settings.roundRestorePointAgeNumber = shortRound
            MsgBox("Setting saved.", MsgBoxStyle.Information, "Restore Point Creator")

            boolButtonPushed = True
            dialogResponse = userResponse.ok
            Me.Close()
        Else
            MsgBox("Invalid input. Your input must be a number.", MsgBoxStyle.Critical, "Restore Point Creator")
        End If
    End Sub

    Private Sub Round_Restore_Point_Age_in_Days_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        txtRound.Text = My.Settings.roundRestorePointAgeNumber.ToString
    End Sub

    Private Sub btnCancel_Click(sender As Object, e As EventArgs) Handles btnCancel.Click
        boolButtonPushed = True
        dialogResponse = userResponse.cancel
        Me.Close()
    End Sub

    Private Sub Round_Restore_Point_Age_in_Days_FormClosing(sender As Object, e As FormClosingEventArgs) Handles Me.FormClosing
        If Not boolButtonPushed Then dialogResponse = userResponse.cancel
    End Sub

    Private Sub txtRound_KeyUp(sender As Object, e As KeyEventArgs) Handles txtRound.KeyUp
        If e.KeyCode = Keys.Enter Then btnOK.PerformClick()
    End Sub
End Class