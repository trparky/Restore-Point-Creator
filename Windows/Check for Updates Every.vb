Public Class checkForUpdatesEvery
    Public parentFormG As Form1

    Private Sub btnOK_Click(sender As Object, e As EventArgs) Handles btnOK.Click
        Try
            Dim shortDays As Short
            txtDays.Text = txtDays.Text.Trim

            If Short.TryParse(txtDays.Text, shortDays) = True Then
                My.Settings.checkForUpdatesEveryInDays = shortDays

                If shortDays = 1 Then
                    parentFormG.toolStripCheckCustom.Text = "Custom Time Interval (Check for updates every day)"
                Else
                    parentFormG.toolStripCheckCustom.Text = "Custom Time Interval (Check for updates every " & shortDays & " days)"
                End If

                Me.Close()
            Else
                MsgBox("You must enter a number.", MsgBoxStyle.Information, Me.Text)
                Exit Sub
            End If
        Catch ex As OverflowException
            MsgBox("You have entered in a number that's too large. Please try again.", MsgBoxStyle.Critical, Me.Text)
        End Try
    End Sub

    Private Sub btnCancel_Click(sender As Object, e As EventArgs) Handles btnCancel.Click
        parentFormG.toolStripCheckCustom.Checked = False

        If My.Settings.checkForUpdatesEveryInDays = 7 Then
            parentFormG.toolStripCheckEveryWeek.Checked = True
            parentFormG.toolStripCheckCustom.Text = "Custom"
        ElseIf My.Settings.checkForUpdatesEveryInDays = 14 Then
            parentFormG.toolStripCheckEveryTwoWeeks.Checked = True
            parentFormG.toolStripCheckCustom.Text = "Custom"
        End If

        Me.Close()
    End Sub

    Private Sub txtDays_KeyUp(sender As Object, e As KeyEventArgs) Handles txtDays.KeyUp
        If e.KeyCode = Keys.Enter Then
            btnOK.PerformClick()
        End If
    End Sub
End Class