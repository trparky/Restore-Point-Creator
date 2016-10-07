Public Class Update_Message
#Region "--== Public Data Types ==--"
    Public Enum versionUpdateType
        standardVersionUpdate = 1
        betaVersionUpdate = 2
        releaseCandidateVersionUpdate = 3
        totallyNewVersionUpdate = 4
        minorUpdate = 5
    End Enum

    Public Enum userResponse
        doTheUpdate = 1
        dontDoTheUpdate = 2
        nullSetting = 3
    End Enum
#End Region

#Region "--== Public Variables ==--"
    ' dialogResponse is the Public Variable that exposes the user's response outside of this form.
    Public dialogResponse As userResponse = userResponse.nullSetting

    ' versionUpdate is the Public Variable that is used by code outside of this form to write to it when a totally new version is released.
    ' It's only used when versionUpdate is equal to versionUpdateType.totallyNewVersionUpdate.
    Public versionUpdate As versionUpdateType
#End Region

    Public newVersionString As String
    Private boolButtonPushed As Boolean = False
    Private shortCountDown As Short = 30

    Sub loadChangeLogData()
        Try
            Dim strChangeLog As String = Nothing, urlToLoadDataFrom As String = Nothing

            Dim httpHelper As httpHelper = Functions.http.createNewHTTPHelperObject()

            If versionUpdate = versionUpdateType.standardVersionUpdate Or versionUpdate = versionUpdateType.totallyNewVersionUpdate Or versionUpdate = versionUpdateType.minorUpdate Then
                urlToLoadDataFrom = globalVariables.webURLs.core.strRecentChangesLog
            ElseIf versionUpdate = versionUpdateType.betaVersionUpdate Or versionUpdate = versionUpdateType.releaseCandidateVersionUpdate Then
                urlToLoadDataFrom = globalVariables.webURLs.core.strBetaDetails

                If My.Settings.showPartialBetaChangeLogs = True Then
                    If globalVariables.version.boolBeta = True Then
                        httpHelper.addPOSTData("beta", globalVariables.version.shortBetaVersion)
                    ElseIf globalVariables.version.boolReleaseCandidate = True Then
                        httpHelper.addPOSTData("rc", globalVariables.version.shortReleaseCandidateVersion)
                    End If
                End If
            End If

            httpHelper.addPOSTData("programversion", globalVariables.version.strFullVersionString)
            httpHelper.addPOSTData("major", globalVariables.version.shortMajor)
            httpHelper.addPOSTData("minor", globalVariables.version.shortMinor)
            httpHelper.addPOSTData("build", globalVariables.version.shortBuild)

            Try
                If httpHelper.getWebData(urlToLoadDataFrom, strChangeLog) = True Then
                    If versionUpdate = versionUpdateType.betaVersionUpdate Or versionUpdate = versionUpdateType.releaseCandidateVersionUpdate Then
                        If globalVariables.version.boolBeta = True Then
                            Dim versionStringToFindAndReplace As String = String.Format("Public Beta {0} Changes", globalVariables.version.shortBetaVersion)
                            strChangeLog = strChangeLog.Replace(versionStringToFindAndReplace, versionStringToFindAndReplace & " (Currently Installed Version)")
                        ElseIf globalVariables.version.boolReleaseCandidate = True Then
                            Dim versionStringToFindAndReplace As String = String.Format("Release Candidate {0} Changes", globalVariables.version.shortReleaseCandidateVersion)
                            strChangeLog = strChangeLog.Replace(versionStringToFindAndReplace, versionStringToFindAndReplace & " (Currently Installed Version)")
                        End If
                    End If

                    txtChanges.Text = strChangeLog
                Else
                    txtChanges.Text = "Error Loading Change Log... Please Try Again."
                End If
            Catch ex As Exception
                If TypeOf ex Is Net.WebException Or TypeOf ex Is httpProtocolException Then
                    Functions.eventLogFunctions.writeToSystemEventLog("The server responded with an HTTP error. This may be because the web site is down or some other kind of issue. Please check back at at later time.", EventLogEntryType.Warning)
                    MsgBox("The server responded with an HTTP error. This may be because the web site is down or some other kind of issue. Please check back at at later time.", MsgBoxStyle.Exclamation, "Restore Point Creator")
                ElseIf TypeOf ex Is sslErrorException Then
                    Functions.eventLogFunctions.writeToSystemEventLog("An HTTP SSL error occurred.", EventLogEntryType.Error)
                End If

                Functions.eventLogFunctions.writeCrashToEventLog(ex)

                txtChanges.Text = "Error Loading Change Log... Please Try Again."
            End Try

            Me.Focus()
        Catch ex As Exception
            Functions.eventLogFunctions.writeCrashToEventLog(ex)
        End Try
    End Sub

    Private Sub Update_Message_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        PictureBox1.Image = SystemIcons.Information.ToBitmap()
        Control.CheckForIllegalCrossThreadCalls = False
        Me.Size = My.Settings.updateMessageDialogSize
        chkShowPartialBetaChangeLogs.Checked = My.Settings.showPartialBetaChangeLogs

        If My.Settings.useSSL = True Then
            ToolTip1.SetToolTip(btnYes, "Download Secured by SSL.")
            imgSSL.Visible = True
        End If

        If versionUpdate = versionUpdateType.betaVersionUpdate Or versionUpdate = versionUpdateType.releaseCandidateVersionUpdate Then
            chkShowPartialBetaChangeLogs.Visible = True
        Else
            chkShowPartialBetaChangeLogs.Visible = False
        End If

        Dim loadDetailsThread As New Threading.Thread(AddressOf loadChangeLogData)
        loadDetailsThread.Name = "Change Log Data Loading Thread"
        loadDetailsThread.Priority = Threading.ThreadPriority.Lowest
        loadDetailsThread.Start()

        Me.Icon = My.Resources.RestorePoint_noBackground_2
        Media.SystemSounds.Asterisk.Play()

        If My.Settings.askToUpgrade = True Then
            Label2.Text = "Do you want to upgrade to this new version?"
            TableLayoutPanel1.Visible = True
        Else
            Label2.Text = "The update will now be downloaded and installed on your system."
            TableLayoutPanel1.Visible = False
        End If

        If versionUpdate = versionUpdateType.standardVersionUpdate Then
            lblStandardUpdateNotice.Visible = True
        ElseIf versionUpdate = versionUpdateType.totallyNewVersionUpdate Then
            lblTotallyNewVersion.Visible = True
            lblTotallyNewVersion.Text = String.Format(lblTotallyNewVersion.Text, globalVariables.version.versionStringWithoutBuild, newVersionString)
        ElseIf versionUpdate = versionUpdateType.betaVersionUpdate Then
            lblBetaNotice.Visible = True
        ElseIf versionUpdate = versionUpdateType.releaseCandidateVersionUpdate Then
            lblReleaseCandidateNotice.Visible = True
        ElseIf versionUpdate = versionUpdateType.minorUpdate Then
            lblMinorUpdateNotice.Visible = True
        End If

        If globalVariables.version.boolBeta = True Then
            lblCurrentVersion.Text = String.Format("Current Version: {0} Public Beta {1}", globalVariables.version.strFullVersionString, globalVariables.version.shortBetaVersion)
        ElseIf globalVariables.version.boolReleaseCandidate = True Then
            lblCurrentVersion.Text = String.Format("Current Version: {0} Release Candidate {1}", globalVariables.version.strFullVersionString, globalVariables.version.shortReleaseCandidateVersion)
        Else
            lblCurrentVersion.Text = String.Format("Current Version: {0}", globalVariables.version.strFullVersionString)
        End If

        timerCountdown.Enabled = True
    End Sub

    Private Sub txtChanges_GotFocus(sender As Object, e As EventArgs) Handles txtChanges.GotFocus
        txtChanges.SelectionStart = 0
    End Sub

    Private Sub btnOKButton_Click(sender As Object, e As EventArgs) Handles btnOK.Click
        boolButtonPushed = True
        dialogResponse = userResponse.doTheUpdate
        Me.Close()
    End Sub

    Private Sub btnYes_Click(sender As Object, e As EventArgs) Handles btnYes.Click
        boolButtonPushed = True
        dialogResponse = userResponse.doTheUpdate
        Me.Close()
    End Sub

    Private Sub btnNo_Click(sender As Object, e As EventArgs) Handles btnNo.Click
        boolButtonPushed = True
        dialogResponse = userResponse.dontDoTheUpdate
        Me.Close()
    End Sub

    Private Sub Update_Message_FormClosing(sender As Object, e As FormClosingEventArgs) Handles Me.FormClosing
        If boolButtonPushed = False Then dialogResponse = userResponse.dontDoTheUpdate
    End Sub

    Private Sub btnReloadChangeLog_Click(sender As Object, e As EventArgs) Handles btnReloadChangeLog.Click
        disableCountdown()
        txtChanges.Text = "Loading Change Log Data... Please Wait."
        Dim loadDetailsThread As New Threading.Thread(AddressOf loadChangeLogData)
        loadDetailsThread.Name = "Change Log Data Loading Thread"
        loadDetailsThread.Priority = Threading.ThreadPriority.Lowest
        loadDetailsThread.Start()
    End Sub

    Private Sub Update_Message_Resize(sender As Object, e As EventArgs) Handles Me.Resize
        If Me.Width <> 788 Then Me.Width = 788
    End Sub

    Private Sub Update_Message_ResizeEnd(sender As Object, e As EventArgs) Handles Me.ResizeEnd
        My.Settings.updateMessageDialogSize = Me.Size
    End Sub

    Private Sub chkShowPartialBetaChangeLogs_Click(sender As Object, e As EventArgs) Handles chkShowPartialBetaChangeLogs.Click
        disableCountdown()
        txtChanges.Text = "Loading Change Log Data... Please Wait."
        My.Settings.showPartialBetaChangeLogs = chkShowPartialBetaChangeLogs.Checked

        Dim loadDetailsThread As New Threading.Thread(AddressOf loadChangeLogData)
        loadDetailsThread.Name = "Change Log Data Loading Thread"
        loadDetailsThread.Priority = Threading.ThreadPriority.Lowest
        loadDetailsThread.Start()
    End Sub

    Private Sub timerCountdown_Tick(sender As Object, e As EventArgs) Handles timerCountdown.Tick
        If shortCountDown = 0 Then
            timerCountdown.Enabled = False
            btnYes.PerformClick()
        Else
            shortCountDown -= 1
            lblCountdown.Text = shortCountDown

            If shortCountDown = 1 Then
                ToolTip1.SetToolTip(lblCountdown, "1 second until automatic Yes to update. Click to disable countdown.")
            Else
                ToolTip1.SetToolTip(lblCountdown, shortCountDown & " seconds until automatic Yes to update. Click to disable countdown.")
            End If
        End If
    End Sub

    Sub disableCountdown()
        If timerCountdown.Enabled = True Then
            timerCountdown.Enabled = False
            lblCountdown.Text = "  "
            ToolTip1.SetToolTip(lblCountdown, "Update countdown disabled.")
        End If
    End Sub

    Private Sub txtChanges_Click(sender As Object, e As EventArgs) Handles txtChanges.Click
        disableCountdown()
    End Sub

    Private Sub lblCountdown_Click(sender As Object, e As EventArgs) Handles lblCountdown.Click
        disableCountdown()
    End Sub

    Private Sub Update_Message_Click(sender As Object, e As EventArgs) Handles Me.Click
        disableCountdown()
    End Sub

    Private Sub txtChanges_MouseWheel(sender As Object, e As MouseEventArgs) Handles txtChanges.MouseWheel
        disableCountdown()
    End Sub

    Private Sub imgSSL_Click(sender As Object, e As EventArgs) Handles imgSSL.Click
        disableCountdown()
    End Sub

    Private Sub Label2_Click(sender As Object, e As EventArgs) Handles Label2.Click
        disableCountdown()
    End Sub

    Private Sub lblBetaNotice_Click(sender As Object, e As EventArgs) Handles lblBetaNotice.Click
        disableCountdown()
    End Sub

    Private Sub lblChangesAsFollows_Click(sender As Object, e As EventArgs) Handles lblChangesAsFollows.Click
        disableCountdown()
    End Sub

    Private Sub lblCurrentVersion_Click(sender As Object, e As EventArgs) Handles lblCurrentVersion.Click
        disableCountdown()
    End Sub

    Private Sub lblMinorUpdateNotice_Click(sender As Object, e As EventArgs) Handles lblMinorUpdateNotice.Click
        disableCountdown()
    End Sub

    Private Sub lblReleaseCandidateNotice_Click(sender As Object, e As EventArgs) Handles lblReleaseCandidateNotice.Click
        disableCountdown()
    End Sub

    Private Sub lblStandardUpdateNotice_Click(sender As Object, e As EventArgs) Handles lblStandardUpdateNotice.Click
        disableCountdown()
    End Sub

    Private Sub lblTotallyNewVersion_Click(sender As Object, e As EventArgs) Handles lblTotallyNewVersion.Click
        disableCountdown()
    End Sub

    Private Sub PictureBox1_Click(sender As Object, e As EventArgs) Handles PictureBox1.Click
        disableCountdown()
    End Sub

    Private Sub TableLayoutPanel1_Click(sender As Object, e As EventArgs) Handles TableLayoutPanel1.Click
        disableCountdown()
    End Sub
End Class