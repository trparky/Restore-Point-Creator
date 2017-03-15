Imports System.Text.RegularExpressions

Public Class frmDeleteOldSystemRestorePoints
    Private Sub btnOK_Click(sender As System.Object, e As System.EventArgs) Handles btnOK.Click
        Try
            Dim shortMaxAge As Short
            txtMaxAge.Text = txtMaxAge.Text.Trim

            ' This checks to see if the user inputted an Integer and not a String.
            If Short.TryParse(txtMaxAge.Text, shortMaxAge) = True Then
                If shortMaxAge < 5 Then
                    Dim msgBoxResult As MsgBoxResult

                    Dim messageBoxText As New Text.StringBuilder
                    messageBoxText.AppendLine("It's a recommended idea to keep at least 5 days of System Restore Points for the safety of your computer.")
                    messageBoxText.AppendLine()

                    If shortMaxAge = 1 Then
                        messageBoxText.Append("Are you sure you want to delete System Restore Points older than 1 day?")
                    Else
                        messageBoxText.AppendFormat("Are you sure you want to delete System Restore Points older than {0} days?", txtMaxAge.Text)
                    End If

                    msgBoxResult = MsgBox(messageBoxText.ToString.Trim, MsgBoxStyle.Question + MsgBoxStyle.YesNo, "Are you sure?")
                    messageBoxText = Nothing

                    If msgBoxResult = MsgBoxResult.No Then
                        Exit Sub
                    End If
                End If

                If chkRememberThisSetting.Checked Then My.Settings.maxDaysManualDelete = shortMaxAge

                If My.Settings.maxDaysManualDelete = 1 Then
                    Form1.toolStripDeleteOldRestorePoints.Text = "Delete Restore Points older than 1 Day"
                Else
                    Form1.toolStripDeleteOldRestorePoints.Text = "Delete Restore Points older than " & My.Settings.maxDaysManualDelete & " Days"
                End If

                Form1.doDeleteOldSystemRestorePoint(shortMaxAge)
                Me.Close()
            Else
                txtMaxAge.Text = "10" ' Nope, so we replace it with valid data.
                Exit Sub ' And exit the sub.
            End If
        Catch ex As OverflowException
            MsgBox("You have entered in a number that's too large. Please try again.", MsgBoxStyle.Critical, Me.Text)
        End Try
    End Sub

    Private Sub btnCancel_Click(sender As System.Object, e As System.EventArgs) Handles btnCancel.Click
        Me.Close()
    End Sub

    Private Sub txtMaxAge_KeyUp(sender As Object, e As System.Windows.Forms.KeyEventArgs) Handles txtMaxAge.KeyUp
        If e.KeyCode = Keys.Enter Then
            btnOK.PerformClick()
        End If
    End Sub

    Private Sub frmDeleteOldSystemRestorePoints_Load(sender As System.Object, e As System.EventArgs) Handles MyBase.Load
        Me.Location = Functions.support.verifyWindowLocation(My.Settings.DeleteOldSystemRestorePointsWindowLocation)
        txtMaxAge.Select()
    End Sub

    Private Sub frmDeleteOldSystemRestorePoints_FormClosing(sender As Object, e As FormClosingEventArgs) Handles Me.FormClosing
        My.Settings.DeleteOldSystemRestorePointsWindowLocation = Me.Location
    End Sub
End Class