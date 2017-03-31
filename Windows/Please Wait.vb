Public Class Please_Wait
    Public allowClose As Boolean = False
    Public lblLabelText As String
    Public myParentForm As Form
    Public howToCenter As Short = enums.howToCenterWindow.parent

    Private Sub Please_Wait_FormClosing(sender As Object, e As FormClosingEventArgs) Handles Me.FormClosing
        If allowClose = False And e.CloseReason <> CloseReason.UserClosing Then
            e.Cancel = True
            Exit Sub
        End If
    End Sub

    Private Sub Please_Wait_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        If howToCenter = enums.howToCenterWindow.parent Then
            Me.CenterToParent()
        ElseIf howToCenter = enums.howToCenterWindow.screen Then
            Me.CenterToScreen()
        End If

        Try
            Me.BringToFront()
            Me.Focus()
        Catch ex As Exception
        End Try

        Control.CheckForIllegalCrossThreadCalls = False
        SmoothProgressBar1.ProgressBarColor = My.Settings.barColor
        progressBarChanger.Enabled = True
        pleaseWaitMessageChanger.Enabled = True
        autoWindowCloser.Enabled = True
    End Sub

    Private Sub progressBarChanger_Tick(sender As Object, e As EventArgs) Handles progressBarChanger.Tick
        If SmoothProgressBar1.Value < 100 Then
            SmoothProgressBar1.Value += 1
        Else
            SmoothProgressBar1.Value = 0
        End If
    End Sub

    Private Sub pleaseWaitMessageChanger_Tick(sender As Object, e As EventArgs) Handles pleaseWaitMessageChanger.Tick
        If Me.Text = "Please Wait..." Then
            Me.Text = "Please Wait"
            pleaseWaitlblLabel.Text = lblLabelText
        ElseIf Me.Text = "Please Wait" Then
            Me.Text = "Please Wait."
            pleaseWaitlblLabel.Text = lblLabelText & "."
        ElseIf Me.Text = "Please Wait." Then
            Me.Text = "Please Wait.."
            pleaseWaitlblLabel.Text = lblLabelText & ".."
        ElseIf Me.Text = "Please Wait.." Then
            Me.Text = "Please Wait..."
            pleaseWaitlblLabel.Text = lblLabelText & "..."
        End If
    End Sub

    Private Sub autoWindowCloser_Tick(sender As Object, e As EventArgs) Handles autoWindowCloser.Tick
        MsgBox("Something went wrong, this window should have been closed by the processing thread on the main window. If this window closes in this way, something seriously went wrong.", MsgBoxStyle.Critical, "Something went wrong...")
        allowClose = True
        Me.Close()
    End Sub
End Class