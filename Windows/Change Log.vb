﻿Public Class Change_Log
    Private workingThread As Threading.Thread

    Private Sub Change_Log_FormClosing(sender As Object, e As FormClosingEventArgs) Handles MyBase.FormClosing
        If workingThread IsNot Nothing Then workingThread.Abort()

        My.Settings.changeLogWindowSize = Me.Size
        globalVariables.windows.frmChangeLog.Dispose()
        globalVariables.windows.frmChangeLog = Nothing
    End Sub

    Sub loadChangelog()
        If workingThread Is Nothing Then
            openPleaseWaitPanel("Loading Official Change Log... Please Wait.")

            workingThread = New Threading.Thread(AddressOf loadChangelogSub)
            workingThread.Name = "Change Log Loading Thread"
            workingThread.IsBackground = True
            workingThread.Start()
        End If
    End Sub

    Private Sub Change_Log_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        Me.Size = My.Settings.changeLogWindowSize
    End Sub

    Sub loadChangelogSub()
        Try
            Me.Invoke(Sub()
                          RichTextBox1.Text = "Loading Change Log from web site..."
                          AbortToolStripMenuItem.Visible = True
                      End Sub)

            Dim changeLogData As String = Nothing
            Dim httpHelper As httpHelper = Functions.http.createNewHTTPHelperObject()
            Dim stopWatch As Stopwatch = Stopwatch.StartNew()

            Try
                If httpHelper.getWebData(globalVariables.webURLs.core.strFullChangeLog, changeLogData) = True Then
                    Dim stringWeAreLookingFor As String = String.Format("Version {0}.{1} Build {2}", globalVariables.version.shortMajor, globalVariables.version.shortMinor, globalVariables.version.shortBuild)
                    changeLogData = changeLogData.Replace(stringWeAreLookingFor, stringWeAreLookingFor & " (Currently Installed Version)")

                    Me.Invoke(Sub() RichTextBox1.Rtf = changeLogData)
                    changeLogData = Nothing

                    stopWatch.Stop()
                    If stopWatch.Elapsed.Milliseconds < 1000 Then Threading.Thread.Sleep(1000 - stopWatch.Elapsed.Milliseconds)

                    Me.Invoke(Sub()
                                  closePleaseWaitPanel()
                                  workingThread = Nothing
                                  AbortToolStripMenuItem.Visible = False
                              End Sub)
                Else
                    Me.Invoke(Sub()
                                  RichTextBox1.Text = "There was an error loading the official changelog."
                                  closePleaseWaitPanel()
                                  workingThread = Nothing
                                  AbortToolStripMenuItem.Visible = False
                              End Sub)
                    Exit Sub
                End If
            Catch ex As Exception
                Exit Sub
            End Try
        Catch ex2 As Threading.ThreadAbortException
        Catch ex As Exception
            Me.Invoke(Sub()
                          RichTextBox1.Text = "Error loading change log data."
                          closePleaseWaitPanel()
                          workingThread = Nothing
                          AbortToolStripMenuItem.Visible = False
                      End Sub)
        Finally
            workingThread = Nothing
        End Try
    End Sub

    Private Sub AbortToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles AbortToolStripMenuItem.Click
        workingThread.Abort()
    End Sub

    Private Sub ReloadToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles ReloadToolStripMenuItem.Click
        loadChangelog()
    End Sub

    Private Sub Change_Log_Shown(sender As Object, e As EventArgs) Handles MyBase.Shown
        loadChangelog()
    End Sub

    Private Sub Change_Log_KeyUp(sender As Object, e As KeyEventArgs) Handles MyBase.KeyUp
        If e.KeyCode = Keys.F5 Then loadChangelog()
    End Sub

#Region "--== Please Wait Panel Code ==--"
    Private strPleaseWaitLabelText As String

    Private Sub centerPleaseWaitPanel()
        pleaseWaitPanel.Location = New Point(
            (Me.ClientSize.Width / 2) - (pleaseWaitPanel.Size.Width / 2),
            (Me.ClientSize.Height / 2) - (pleaseWaitPanel.Size.Height / 2))
        pleaseWaitPanel.Anchor = AnchorStyles.None
    End Sub

    Private Sub openPleaseWaitPanel(strInputPleaseWaitLabelText As String)
        Functions.support.disableControlsOnForm(Me)

        strPleaseWaitLabelText = strInputPleaseWaitLabelText
        pleaseWaitProgressBar.ProgressBarColor = My.Settings.barColor
        pleaseWaitlblLabel.Text = strInputPleaseWaitLabelText
        centerPleaseWaitPanel()
        pleaseWaitPanel.Visible = True
        pleaseWaitProgressBar.Value = 0
        pleaseWaitProgressBarChanger.Enabled = True
        pleaseWaitMessageChanger.Enabled = True
        pleaseWaitBorderText.BackColor = globalVariables.pleaseWaitPanelColor
        pleaseWaitBorderText.ForeColor = globalVariables.pleaseWaitPanelFontColor
    End Sub

    Private Sub closePleaseWaitPanel()
        Functions.support.enableControlsOnForm(Me)

        pleaseWaitPanel.Visible = False
        pleaseWaitProgressBarChanger.Enabled = False
        pleaseWaitMessageChanger.Enabled = False
        pleaseWaitProgressBar.Value = 0
    End Sub

    Private Sub pleaseWaitProgressBarChanger_Tick(sender As Object, e As EventArgs) Handles pleaseWaitProgressBarChanger.Tick
        pleaseWaitProgressBar.Value = If(pleaseWaitProgressBar.Value < 100, pleaseWaitProgressBar.Value + 1, 0)
    End Sub

    Private Sub pleaseWaitMessageChanger_Tick(sender As Object, e As EventArgs) Handles pleaseWaitMessageChanger.Tick
        If pleaseWaitBorderText.Text = "Please Wait..." Then
            pleaseWaitBorderText.Text = "Please Wait"
            pleaseWaitlblLabel.Text = strPleaseWaitLabelText
        ElseIf pleaseWaitBorderText.Text = "Please Wait" Then
            pleaseWaitBorderText.Text = "Please Wait."
            pleaseWaitlblLabel.Text = strPleaseWaitLabelText & "."
        ElseIf pleaseWaitBorderText.Text = "Please Wait." Then
            pleaseWaitBorderText.Text = "Please Wait.."
            pleaseWaitlblLabel.Text = strPleaseWaitLabelText & ".."
        ElseIf pleaseWaitBorderText.Text = "Please Wait.." Then
            pleaseWaitBorderText.Text = "Please Wait..."
            pleaseWaitlblLabel.Text = strPleaseWaitLabelText & "..."
        End If
    End Sub
#End Region
End Class