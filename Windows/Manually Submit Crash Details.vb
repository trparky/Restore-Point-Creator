Public Class frmManuallySubmitCrashData
    Private boolSubmitted As Boolean = False
    Private boolDoWeHaveAttachments As Boolean = False
    Public crashData As String
    Private _logID As Long

    Private strLogFile As String = IO.Path.Combine(IO.Path.GetTempPath(), "event log entries.reslogx")
    Private strZIPFile As String = IO.Path.Combine(IO.Path.GetTempPath(), "attachments.zip")

    Public WriteOnly Property logID As Long
        Set(value As Long)
            _logID = value
        End Set
    End Property

    Private Sub Manually_Submit_Crash_Details_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        Me.Location = Functions.support.verifyWindowLocation(My.Settings.ManuallySubmitCrashDataInstanceLocation)
        If My.Settings.useSSL = True Then
            btnSubmitData.Image = My.Resources.lock
            ToolTip.SetToolTip(btnSubmitData, "Secured by SSL.")
        End If

        If My.Settings.boolSaveInfo Then
            chkSaveInfo.Checked = True
            txtName.Text = My.Settings.usersName
            txtEmail.Text = My.Settings.usersEmail
        End If
    End Sub

    Private Sub btnClose_Click(sender As Object, e As EventArgs) Handles btnClose.Click
        If boolSubmitted = False Then
            If MsgBox("Are you sure you want to close this window? You have not submitted the crash data yet.", MsgBoxStyle.Question + MsgBoxStyle.YesNo, Me.Text) = MsgBoxResult.Yes Then
                boolSubmitted = True
                Me.Close()
            End If
        End If
    End Sub

    Private Sub Manually_Submit_Crash_Details_FormClosing(sender As Object, e As FormClosingEventArgs) Handles Me.FormClosing
        If boolSubmitted = False Then
            If MsgBox("Are you sure you want to close this window? You have not submitted the crash data yet.", MsgBoxStyle.Question + MsgBoxStyle.YesNo, Me.Text) = MsgBoxResult.No Then
                e.Cancel = True
                Exit Sub
            End If
        End If

        My.Settings.ManuallySubmitCrashDataInstanceLocation = Me.Location
        globalVariables.windows.frmManuallySubmitCrashDataInstance.Dispose()
        globalVariables.windows.frmManuallySubmitCrashDataInstance = Nothing
    End Sub

    Sub deleteFileWithCrashPrevention(strPathToFile As String)
        Try
            If IO.File.Exists(strPathToFile) = True Then IO.File.Delete(strPathToFile)
        Catch ex As Exception
        End Try
    End Sub

    Sub deleteTempFiles()
        If boolDoWeHaveAttachments = True Then
            deleteFileWithCrashPrevention(strZIPFile)
            deleteFileWithCrashPrevention(strLogFile)
        End If
    End Sub

    Sub dataSubmitThread()
        txtEmail.Text = txtEmail.Text.Trim
        txtName.Text = txtName.Text.Trim

        btnSubmitData.Enabled = False
        btnClose.Enabled = False

        Dim httpHelper As httpHelper = Functions.http.createNewHTTPHelperObject()

        httpHelper.addPOSTData("name", txtName.Text)
        httpHelper.addPOSTData("email", txtEmail.Text)
        httpHelper.addPOSTData("program", globalVariables.programName)
        httpHelper.addPOSTData("submissionversion", "4")
        httpHelper.addPOSTData("manually", "true")

        If chkReproducable.Checked = True Then
            httpHelper.addPOSTData("reproducable", "Yes")
        Else
            httpHelper.addPOSTData("reproducable", "No")
        End If

        httpHelper.addPOSTData("crashdata", crashData)

        If Not String.IsNullOrWhiteSpace(txtDoing.Text.Trim) Then
            httpHelper.addPOSTData("doing", txtDoing.Text)
        End If

        Try
            If chkSendLogs.Checked = True Then
                Dim logCount As ULong = 0

                If Functions.eventLogFunctions.exportLogsToFile(strLogFile, logCount) = True Then
                    If Functions.support.addFileToZipFile(strZIPFile, strLogFile) = True Then
                        If IO.File.Exists(strZIPFile) = True Then
                            boolDoWeHaveAttachments = True
                            httpHelper.addFileUpload("attachment", strZIPFile, Nothing, "application/zip")
                        End If
                    End If
                End If
            End If

            Dim strHTTPResponse As String = Nothing
            Dim boolHTTPResult As Boolean

            If boolDoWeHaveAttachments = True Then
                boolHTTPResult = httpHelper.uploadData(globalVariables.webURLs.dataProcessors.strCrashReporter, strHTTPResponse)
            Else
                boolHTTPResult = httpHelper.getWebData(globalVariables.webURLs.dataProcessors.strCrashReporter, strHTTPResponse)
            End If

            If boolHTTPResult = True Then
                closePleaseWaitPanel()
                Debug.WriteLine(httpHelper.getHTTPResponseHeaders.ToString)
                deleteTempFiles()

                If strHTTPResponse.Equals("ok", StringComparison.OrdinalIgnoreCase) Then
                    boolSubmitted = True
                    Functions.eventLogFunctions.markLogEntryAsSubmitted(_logID)
                    MsgBox("Crash data has been submitted. This window will now close.", MsgBoxStyle.Information + MsgBoxStyle.ApplicationModal, "Restore Point Creator Crash Reporter")
                    Me.Close()
                ElseIf strHTTPResponse.Equals("error", StringComparison.OrdinalIgnoreCase) Then
                    boolSubmitted = False
                    Me.btnSubmitData.Enabled = True
                    Me.btnClose.Enabled = True
                    MsgBox("There was an error in submission. Please try again.", MsgBoxStyle.Critical + MsgBoxStyle.ApplicationModal, "Restore Point Creator Crash Reporter")
                ElseIf strHTTPResponse.Equals("error-invalid-email", StringComparison.OrdinalIgnoreCase) Then
                    boolSubmitted = False
                    Me.btnSubmitData.Enabled = True
                    Me.btnClose.Enabled = True
                    MsgBox("Invalid email address. Please try again.", MsgBoxStyle.Critical + MsgBoxStyle.ApplicationModal, "Restore Point Creator Crash Reporter")
                ElseIf strHTTPResponse.Equals("email-server-said-user-doesnt-exist", StringComparison.OrdinalIgnoreCase) Then
                    boolSubmitted = False
                    Me.btnSubmitData.Enabled = True
                    Me.btnClose.Enabled = True
                    MsgBox("The remote email server said that the email address doesn't exist. Please try again.", MsgBoxStyle.Critical + MsgBoxStyle.ApplicationModal, "Restore Point Creator Crash Reporter")
                ElseIf strHTTPResponse.Equals("no-email-servers-contactable", StringComparison.OrdinalIgnoreCase) Then
                    boolSubmitted = False
                    Me.btnSubmitData.Enabled = True
                    Me.btnClose.Enabled = True
                    MsgBox("No mail servers found, more than likely your email address is invalid. Please try again.", MsgBoxStyle.Critical + MsgBoxStyle.ApplicationModal, "Restore Point Creator Crash Reporter")
                ElseIf strHTTPResponse.Equals("dns-error", StringComparison.OrdinalIgnoreCase) Then
                    boolSubmitted = False
                    Me.btnSubmitData.Enabled = True
                    Me.btnClose.Enabled = True
                    MsgBox("The domain name doesn't exist. Please try again.", MsgBoxStyle.Critical + MsgBoxStyle.ApplicationModal, "Restore Point Creator Crash Reporter")
                ElseIf strHTTPResponse.Equals("server-connect-error", StringComparison.OrdinalIgnoreCase) Then
                    boolSubmitted = False
                    Me.btnSubmitData.Enabled = True
                    Me.btnClose.Enabled = True
                    MsgBox("Unable to contact mail server, more than likely your email address is invalid. Please try again.", MsgBoxStyle.Critical + MsgBoxStyle.ApplicationModal, "Restore Point Creator Crash Reporter")
                ElseIf strHTTPResponse.Equals("error-no-crash-data-found", StringComparison.OrdinalIgnoreCase) Then
                    boolSubmitted = False
                    Me.btnSubmitData.Enabled = True
                    Me.btnClose.Enabled = True
                    MsgBox("Something went wrong, no crash data found in submission data. Please try again.", MsgBoxStyle.Critical + MsgBoxStyle.ApplicationModal, "Restore Point Creator Crash Reporter")
                ElseIf strHTTPResponse.Equals("error-no-program-code-found", StringComparison.OrdinalIgnoreCase) Then
                    boolSubmitted = False
                    Me.btnSubmitData.Enabled = True
                    Me.btnClose.Enabled = True
                    MsgBox("Something went wrong, no program code found in submission data. Please try again.", MsgBoxStyle.Critical + MsgBoxStyle.ApplicationModal, "Restore Point Creator Crash Reporter")
                ElseIf strHTTPResponse.Equals("error-no-email-address-found", StringComparison.OrdinalIgnoreCase) Then
                    boolSubmitted = False
                    Me.btnSubmitData.Enabled = True
                    Me.btnClose.Enabled = True
                    MsgBox("Something went wrong, no email address found in submission data. Please try again.", MsgBoxStyle.Critical + MsgBoxStyle.ApplicationModal, "Restore Point Creator Crash Reporter")
                ElseIf strHTTPResponse.Equals("error-no-name-found", StringComparison.OrdinalIgnoreCase) Then
                    boolSubmitted = False
                    Me.btnSubmitData.Enabled = True
                    Me.btnClose.Enabled = True
                    MsgBox("Something went wrong, no name found in submission data. Please try again.", MsgBoxStyle.Critical + MsgBoxStyle.ApplicationModal, "Restore Point Creator Crash Reporter")
                ElseIf strHTTPResponse.Equals("invalid-email-syntax", StringComparison.OrdinalIgnoreCase) Then
                    boolSubmitted = False
                    Me.btnSubmitData.Enabled = True
                    Me.btnClose.Enabled = True
                    MsgBox("The email address didn't pass syntax validation. Please try again.", MsgBoxStyle.Critical + MsgBoxStyle.ApplicationModal, "Restore Point Creator Crash Reporter")
                Else
                    boolSubmitted = True
                    Me.btnSubmitData.Enabled = True
                    Me.btnClose.Enabled = True
                End If
            Else
                closePleaseWaitPanel()
                deleteTempFiles()

                boolSubmitted = False
                Me.btnSubmitData.Enabled = True
                Me.btnClose.Enabled = True
                MsgBox("Something went wrong while submitting data. Please try again.", MsgBoxStyle.Critical + MsgBoxStyle.ApplicationModal, "Restore Point Creator Crash Reporter")
            End If
        Catch ex As Exception
            closePleaseWaitPanel()
            deleteTempFiles()
        End Try
    End Sub

    Private Sub btnSubmitData_Click(sender As Object, e As EventArgs) Handles btnSubmitData.Click
        If String.IsNullOrWhiteSpace(txtEmail.Text.Trim) Then
            MsgBox("You must provide your email address.", MsgBoxStyle.Critical + MsgBoxStyle.ApplicationModal, Me.Text)
            Exit Sub
        End If

        If chkSendLogs.Checked = True Then
            openPleaseWaitPanel("Compressing and Sending Data... Please Wait.")
        End If

        If chkSaveInfo.Checked Then
            My.Settings.usersName = txtName.Text
            My.Settings.usersEmail = txtEmail.Text
        End If

        Threading.ThreadPool.QueueUserWorkItem(AddressOf dataSubmitThread)
    End Sub

    Private Sub chkSaveInfo_Click(sender As Object, e As EventArgs) Handles chkSaveInfo.Click
        If chkSaveInfo.Checked Then
            If MsgBox("Are you sure you want to save your info in the program's settings?" & vbCrLf & vbCrLf & "This is COMPLETELY optional, it is only for the purpose of not burdening you with having to refill your info every time you use either the Official Contact Form or the Crash Submission Form.", MsgBoxStyle.Question + MsgBoxStyle.YesNo, "Are you sure?") = MsgBoxResult.No Then
                chkSaveInfo.Checked = False
            End If
        Else
            If MsgBox("Unchecking this checkbox will delete your saved name and email from the program settings." & vbCrLf & vbCrLf & "Are you sure you want to do this?", MsgBoxStyle.Question + MsgBoxStyle.YesNo, "Are you sure?") = MsgBoxResult.Yes Then
                My.Settings.usersName = Nothing
                My.Settings.usersEmail = Nothing
                chkSaveInfo.Checked = False
            Else
                chkSaveInfo.Checked = True
            End If
        End If

        My.Settings.boolSaveInfo = chkSaveInfo.Checked
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