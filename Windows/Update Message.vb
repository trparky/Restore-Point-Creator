﻿Public Class Update_Message
#Region "--== Public Data Types ==--"
    Public Enum versionUpdateType
        standardVersionUpdate = 1
        betaVersionUpdate = 2
        releaseCandidateVersionUpdate = 3
        totallyNewVersionUpdate = 4
    End Enum

    Public Enum userResponse
        doTheUpdate = 1
        dontDoTheUpdate = 2
    End Enum
#End Region

#Region "--== Public Variables ==--"
    ' dialogResponse is the Public Variable that exposes the user's response outside of this form.
    Public dialogResponse As userResponse = userResponse.dontDoTheUpdate

    Public remoteVersion, remoteBuild, remoteBetaRCVersion As String

    ' versionUpdate is the Public Variable that is used by code outside of this form to write to it when a totally new version is released.
    ' It's only used when versionUpdate is equal to versionUpdateType.totallyNewVersionUpdate.
    Public versionUpdate As versionUpdateType
#End Region

    Private shortCountDown As Short = 30
    Private currentScreen As Screen = Screen.FromControl(Me)

    Sub loadChangeLogData()
        Try
            Dim strChangeLog As String = Nothing
            Dim urlToLoadDataFrom As String = Nothing
            Dim versionStringToFindAndReplace As String
            Dim httpHelper As httpHelper = Functions.http.createNewHTTPHelperObject()

            If versionUpdate = versionUpdateType.standardVersionUpdate Or versionUpdate = versionUpdateType.totallyNewVersionUpdate Then
                urlToLoadDataFrom = globalVariables.webURLs.core.strRecentChangesLog
            ElseIf versionUpdate = versionUpdateType.betaVersionUpdate Or versionUpdate = versionUpdateType.releaseCandidateVersionUpdate Then
                urlToLoadDataFrom = globalVariables.webURLs.core.strBetaDetails

                If My.Settings.showPartialBetaChangeLogs Then
                    If globalVariables.version.boolBeta Then
                        httpHelper.addPOSTData("beta", globalVariables.version.shortBetaVersion)
                    ElseIf globalVariables.version.boolReleaseCandidate Then
                        httpHelper.addPOSTData("rc", globalVariables.version.shortReleaseCandidateVersion)
                    End If
                End If
            End If

            httpHelper.addPOSTData("programversion", globalVariables.version.strFullVersionString)
            httpHelper.addPOSTData("major", globalVariables.version.shortMajor)
            httpHelper.addPOSTData("minor", globalVariables.version.shortMinor)
            httpHelper.addPOSTData("build", globalVariables.version.shortBuild)

            Try
                If httpHelper.getWebData(urlToLoadDataFrom, strChangeLog) Then
                    If versionUpdate = versionUpdateType.betaVersionUpdate Or versionUpdate = versionUpdateType.releaseCandidateVersionUpdate Then
                        If globalVariables.version.boolBeta Then
                            versionStringToFindAndReplace = String.Format("Public Beta {0} Changes", globalVariables.version.shortBetaVersion)
                            strChangeLog = strChangeLog.Replace(versionStringToFindAndReplace, versionStringToFindAndReplace & " (Currently Installed Version)")
                        ElseIf globalVariables.version.boolReleaseCandidate Then
                            versionStringToFindAndReplace = String.Format("Release Candidate {0} Changes", globalVariables.version.shortReleaseCandidateVersion)
                            strChangeLog = strChangeLog.Replace(versionStringToFindAndReplace, versionStringToFindAndReplace & " (Currently Installed Version)")
                        End If
                    End If

                    txtChanges.Text = strChangeLog
                Else
                    txtChanges.Text = "Error Loading Change Log... Please Try Again."
                End If
            Catch ex As Exception
                txtChanges.Text = "Error Loading Change Log... Please Try Again."
            End Try

            Try
                Me.Focus()
            Catch ex As Exception
            End Try
        Catch ex As Exception
            Functions.eventLogFunctions.writeCrashToApplicationLogFile(ex)
        End Try
    End Sub

    Private Sub Update_Message_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        Me.MaximumSize = New Size(788, Screen.FromControl(Me).Bounds.Height)
        PictureBox1.Image = SystemIcons.Information.ToBitmap()
        Control.CheckForIllegalCrossThreadCalls = False
        chkShowPartialBetaChangeLogs.Checked = My.Settings.showPartialBetaChangeLogs

        If My.Settings.useSSL Then
            ToolTip1.SetToolTip(btnYes, "Download Secured by SSL.")
            imgSSL.Visible = True
            lblTheUpdateWillDownload.Location = New Point(33, lblTheUpdateWillDownload.Location.Y)
        Else
            lblTheUpdateWillDownload.Location = New Point(9, lblTheUpdateWillDownload.Location.Y)
        End If

        If versionUpdate = versionUpdateType.betaVersionUpdate Or versionUpdate = versionUpdateType.releaseCandidateVersionUpdate Then
            chkShowPartialBetaChangeLogs.Visible = True
        Else
            chkShowPartialBetaChangeLogs.Visible = False
        End If

        Threading.ThreadPool.QueueUserWorkItem(AddressOf loadChangeLogData)

        Me.Icon = My.Resources.RestorePoint_noBackground_2
        Media.SystemSounds.Asterisk.Play()

        If My.Settings.askToUpgrade Then
            lblTheUpdateWillDownload.Text = "Do you want to upgrade to this new version?"
            TableLayoutPanel1.Visible = True
        Else
            lblTheUpdateWillDownload.Text = "The update will now be downloaded and installed on your system."
            TableLayoutPanel1.Visible = False
        End If

        If versionUpdate = versionUpdateType.standardVersionUpdate Then
            lblTopUpdateMessage.Text = String.Format("There is an updated version of System Restore Point Creator. Version {0} Build {1}.", globalVariables.version.versionStringWithoutBuild, remoteBuild)
        ElseIf versionUpdate = versionUpdateType.totallyNewVersionUpdate Then
            lblTopUpdateMessage.Text = String.Format("Restore Point Creator version {0} is no longer supported and has been replaced by version {1}. Completely new versions are more important than{2}simple new builds of an existing version.", globalVariables.version.versionStringWithoutBuild, remoteBuild, vbCrLf)
        ElseIf versionUpdate = versionUpdateType.betaVersionUpdate Then
            lblTopUpdateMessage.Text = String.Format("There is an updated Public Beta version of System Restore Point Creator. Version {0} Build {1} {2}.", globalVariables.version.versionStringWithoutBuild, remoteBuild, remoteBetaRCVersion)
        ElseIf versionUpdate = versionUpdateType.releaseCandidateVersionUpdate Then
            lblTopUpdateMessage.Text = String.Format("There is an updated Release Candidate version of System Restore Point Creator. Version {0} Build {1} {2}.", globalVariables.version.versionStringWithoutBuild, remoteBuild, remoteBetaRCVersion)
        End If

        lblCurrentVersion.Text = String.Format("Current Version: {0}", globalVariables.version.strFullVersionString)

        If My.Settings.updateNotificationWindowLocation.X <> 0 And My.Settings.updateNotificationWindowLocation.Y <> 0 Then
            Me.Location = Functions.support.verifyWindowLocation(My.Settings.updateNotificationWindowLocation)
        End If

        Me.Size = My.Settings.updateMessageDialogSize
        timerCountdown.Enabled = True
    End Sub

    Private Sub txtChanges_GotFocus(sender As Object, e As EventArgs) Handles txtChanges.GotFocus
        txtChanges.SelectionStart = 0
    End Sub

    Private Sub btnOKButton_Click(sender As Object, e As EventArgs) Handles btnOK.Click
        dialogResponse = userResponse.doTheUpdate
        Me.Close()
    End Sub

    Private Sub btnYes_Click(sender As Object, e As EventArgs) Handles btnYes.Click
        dialogResponse = userResponse.doTheUpdate
        Me.Close()
    End Sub

    Private Sub btnNo_Click(sender As Object, e As EventArgs) Handles btnNo.Click
        If MsgBox(String.Format("Do you want to ignore this version ({0} Build {1})?", remoteVersion, remoteBuild), MsgBoxStyle.YesNo + MsgBoxStyle.Question, "Ignore this version?") = MsgBoxResult.Yes Then
            My.Settings.ignoreThisVersion = String.Format("{0} Build {1}", remoteVersion, remoteBuild)
        End If

        dialogResponse = userResponse.dontDoTheUpdate
        Me.Close()
    End Sub

    Private Sub Update_Message_FormClosing(sender As Object, e As FormClosingEventArgs) Handles Me.FormClosing
        My.Settings.updateNotificationWindowLocation = Me.Location
        My.Settings.updateMessageDialogSize = Me.Size
    End Sub

    Private Sub btnReloadChangeLog_Click(sender As Object, e As EventArgs) Handles btnReloadChangeLog.Click
        disableCountdown()
        txtChanges.Text = "Loading Change Log Data... Please Wait."
        Threading.ThreadPool.QueueUserWorkItem(AddressOf loadChangeLogData)
    End Sub

    Private Sub chkShowPartialBetaChangeLogs_Click(sender As Object, e As EventArgs) Handles chkShowPartialBetaChangeLogs.Click
        disableCountdown()
        txtChanges.Text = "Loading Change Log Data... Please Wait."
        My.Settings.showPartialBetaChangeLogs = chkShowPartialBetaChangeLogs.Checked
        Threading.ThreadPool.QueueUserWorkItem(AddressOf loadChangeLogData)
    End Sub

    Private Sub timerCountdown_Tick(sender As Object, e As EventArgs) Handles timerCountdown.Tick
        If shortCountDown = 0 Then
            timerCountdown.Enabled = False
            btnYes.PerformClick()
        Else
            shortCountDown -= 1
            lblCountdown.Text = shortCountDown
            ToolTip1.SetToolTip(lblCountdown, If(shortCountDown = 1, "1 second until automatic Yes to update. Click to disable countdown.", shortCountDown & " seconds until automatic Yes to update. Click to disable countdown."))
        End If
    End Sub

    Sub disableCountdown()
        If timerCountdown.Enabled Then
            timerCountdown.Enabled = False
            lblCountdown.Text = "  "
            ToolTip1.SetToolTip(lblCountdown, "Update countdown disabled.")
        End If
    End Sub

    Private Sub Update_Message_MouseMove(sender As Object, e As MouseEventArgs) Handles Me.MouseMove
        disableCountdown()
    End Sub

    Private Sub txtChanges_MouseMove(sender As Object, e As MouseEventArgs) Handles txtChanges.MouseMove
        disableCountdown()
    End Sub

    Private Sub Update_Message_Shown(sender As Object, e As EventArgs) Handles Me.Shown
        Me.BringToFront()
    End Sub

    Private Sub txtChanges_Click(sender As Object, e As EventArgs) Handles txtChanges.Click
        Me.BringToFront()
    End Sub

    Private Sub Update_Message_LocationChanged(sender As Object, e As EventArgs) Handles Me.LocationChanged
        Try
            If currentScreen IsNot Nothing Then
                If Not currentScreen.Equals(Screen.FromControl(Me)) Then
                    currentScreen = Screen.FromControl(Me)
                    Me.MaximumSize = New Size(788, Screen.FromControl(Me).Bounds.Height)
                End If
            End If
        Catch ex As Exception
            ' Does nothing.
        End Try
    End Sub
End Class