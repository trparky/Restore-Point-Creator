Public Class frmManuallySubmitCrashData
    Private boolSubmitted As Boolean = False
    Private boolDoWeHaveAttachments As Boolean = False
    Public crashData As String

    Private strLogFile As String = IO.Path.Combine(IO.Path.GetTempPath(), "event log entries.reslog")
    Private strZIPFile As String = IO.Path.Combine(IO.Path.GetTempPath(), "attachments.zip")

    Private Sub Manually_Submit_Crash_Details_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        If My.Settings.useSSL = True Then
            btnSubmitData.Image = My.Resources.lock
            ToolTip.SetToolTip(btnSubmitData, "Secured by SSL.")
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

        If txtDoing.Text.Trim <> Nothing Then
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
                Functions.wait.closePleaseWaitWindow()
                Debug.WriteLine(httpHelper.getHTTPResponseHeaders.ToString)
                deleteTempFiles()

                If strHTTPResponse.stringCompare("ok") Then
                    boolSubmitted = True
                    MsgBox("Crash data has been submitted. This window will now close.", MsgBoxStyle.Information, "Restore Point Creator Crash Reporter")
                    Me.Close()
                ElseIf strHTTPResponse.stringCompare("error") Then
                    boolSubmitted = False
                    Me.btnSubmitData.Enabled = True
                    Me.btnClose.Enabled = True
                    MsgBox("There was an error in submission. Please try again.", MsgBoxStyle.Critical, "Restore Point Creator Crash Reporter")
                ElseIf strHTTPResponse.stringCompare("error-invalid-email") Then
                    boolSubmitted = False
                    Me.btnSubmitData.Enabled = True
                    Me.btnClose.Enabled = True
                    MsgBox("Invalid email address. Please try again.", MsgBoxStyle.Critical, "Restore Point Creator Crash Reporter")
                ElseIf strHTTPResponse.stringCompare("email-server-said-user-doesnt-exist") Then
                    boolSubmitted = False
                    Me.btnSubmitData.Enabled = True
                    Me.btnClose.Enabled = True
                    MsgBox("The remote email server said that the email address doesn't exist. Please try again.", MsgBoxStyle.Critical, "Restore Point Creator Crash Reporter")
                ElseIf strHTTPResponse.stringCompare("no-email-servers-contactable") Then
                    boolSubmitted = False
                    Me.btnSubmitData.Enabled = True
                    Me.btnClose.Enabled = True
                    MsgBox("No mail servers found, more than likely your email address is invalid. Please try again.", MsgBoxStyle.Critical, "Restore Point Creator Crash Reporter")
                ElseIf strHTTPResponse.stringCompare("dns-error") Then
                    boolSubmitted = False
                    Me.btnSubmitData.Enabled = True
                    Me.btnClose.Enabled = True
                    MsgBox("The domain name doesn't exist. Please try again.", MsgBoxStyle.Critical, "Restore Point Creator Crash Reporter")
                ElseIf strHTTPResponse.stringCompare("server-connect-error") Then
                    boolSubmitted = False
                    Me.btnSubmitData.Enabled = True
                    Me.btnClose.Enabled = True
                    MsgBox("Unable to contact mail server, more than likely your email address is invalid. Please try again.", MsgBoxStyle.Critical, "Restore Point Creator Crash Reporter")
                ElseIf strHTTPResponse.stringCompare("error-no-crash-data-found") Then
                    boolSubmitted = False
                    Me.btnSubmitData.Enabled = True
                    Me.btnClose.Enabled = True
                    MsgBox("Something went wrong, no crash data found in submission data. Please try again.", MsgBoxStyle.Critical, "Restore Point Creator Crash Reporter")
                ElseIf strHTTPResponse.stringCompare("error-no-program-code-found") Then
                    boolSubmitted = False
                    Me.btnSubmitData.Enabled = True
                    Me.btnClose.Enabled = True
                    MsgBox("Something went wrong, no program code found in submission data. Please try again.", MsgBoxStyle.Critical, "Restore Point Creator Crash Reporter")
                ElseIf strHTTPResponse.stringCompare("error-no-email-address-found") Then
                    boolSubmitted = False
                    Me.btnSubmitData.Enabled = True
                    Me.btnClose.Enabled = True
                    MsgBox("Something went wrong, no email address found in submission data. Please try again.", MsgBoxStyle.Critical, "Restore Point Creator Crash Reporter")
                ElseIf strHTTPResponse.stringCompare("error-no-name-found") Then
                    boolSubmitted = False
                    Me.btnSubmitData.Enabled = True
                    Me.btnClose.Enabled = True
                    MsgBox("Something went wrong, no name found in submission data. Please try again.", MsgBoxStyle.Critical, "Restore Point Creator Crash Reporter")
                ElseIf strHTTPResponse.stringCompare("invalid-email-syntax") Then
                    boolSubmitted = False
                    Me.btnSubmitData.Enabled = True
                    Me.btnClose.Enabled = True
                    MsgBox("The email address didn't pass syntax validation. Please try again.", MsgBoxStyle.Critical, "Restore Point Creator Crash Reporter")
                Else
                    boolSubmitted = True
                    Me.btnSubmitData.Enabled = True
                    Me.btnClose.Enabled = True
                End If
            Else
                Functions.wait.closePleaseWaitWindow()
                deleteTempFiles()

                boolSubmitted = False
                Me.btnSubmitData.Enabled = True
                Me.btnClose.Enabled = True
                MsgBox("Something went wrong while submitting data. Please try again.", MsgBoxStyle.Critical, "Restore Point Creator Crash Reporter")
            End If
        Catch ex As Exception
            Functions.wait.closePleaseWaitWindow()
            deleteTempFiles()
            Functions.eventLogFunctions.writeCrashToEventLog(ex)

            If TypeOf ex Is Net.WebException Or TypeOf ex Is httpProtocolException Then
                Functions.eventLogFunctions.writeToSystemEventLog("The server responded with an HTTP error. This may be because the web site is down or some other kind of issue. Please check back at at later time.", EventLogEntryType.Warning)
                MsgBox("The server responded with an HTTP error. This may be because the web site is down or some other kind of issue. Please check back at at later time.", MsgBoxStyle.Exclamation, Me.Text)
            ElseIf TypeOf ex Is sslErrorException Then
                Functions.eventLogFunctions.writeToSystemEventLog("An HTTP SSL error occurred.", EventLogEntryType.Error)
                MsgBox("An HTTP SSL error occurred.", MsgBoxStyle.Critical, Me.Text)
            ElseIf TypeOf ex Is Threading.ThreadAbortException Then
                ' We don't do anything here.
            Else
                MsgBox("A general error occured, please check the Event Log.", MsgBoxStyle.Critical, Me.Text)
            End If
        End Try
    End Sub

    Private Sub btnSubmitData_Click(sender As Object, e As EventArgs) Handles btnSubmitData.Click
        If txtEmail.Text.Trim = Nothing Then
            MsgBox("You must provide your email address.", MsgBoxStyle.Critical, Me.Text)
            Exit Sub
        End If

        If chkSendLogs.Checked = True Then
            Functions.wait.createPleaseWaitWindow("Compressing and Sending Data... Please Wait.")
        End If

        Dim submitThread As New Threading.Thread(AddressOf dataSubmitThread)
        submitThread.Name = "Crash Data Submission Thread"
        submitThread.Start()

        If chkSendLogs.Checked = True Then
            Functions.wait.openPleaseWaitWindow(Me)
        End If
    End Sub
End Class