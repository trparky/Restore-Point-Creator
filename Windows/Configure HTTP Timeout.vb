Public Class Configure_HTTP_Timeout
    Private Sub Configure_HTTP_Timeout_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        Me.timeout.Value = Decimal.Parse(My.Settings.httpTimeout)
    End Sub

    Private Sub btnSave_Click(sender As Object, e As EventArgs) Handles btnSave.Click
        My.Settings.httpTimeout = Short.Parse(Math.Round(Me.timeout.Value))
        Me.Close()
    End Sub

    Private Sub timeout_KeyUp(sender As Object, e As KeyEventArgs) Handles timeout.KeyUp
        If e.KeyCode = Keys.Enter Then btnSave.PerformClick()
    End Sub
End Class