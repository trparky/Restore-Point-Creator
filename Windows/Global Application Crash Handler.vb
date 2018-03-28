Imports System.Text

Public Class frmCrash
    Property exceptionMessage As String
    Property exceptionStackTrace As String
    Property exceptionType As String
    Property rawExceptionObject As Exception

    Private strFileToHaveDataExportedTo As String = IO.Path.Combine(IO.Path.GetTempPath(), "event log entries.reslogx")
    Private strTempZIPFile As String = IO.Path.Combine(IO.Path.GetTempPath(), "attachments.zip")

    Sub deleteTempFiles()
        If boolDoWeHaveAttachments = True Then
            Functions.support.deleteFileWithNoException(strTempZIPFile)
            Functions.support.deleteFileWithNoException(strFileToHaveDataExportedTo)
            Functions.support.deleteFileWithNoException(globalVariables.strDumpFilePath)
        End If
    End Sub

    Private Sub frmCrash_FormClosing(sender As Object, e As FormClosingEventArgs) Handles Me.FormClosing
        Try
            If boolSubmitted = False Then
                If MsgBox("Are you sure you want to close this window? You have not submitted the crash data yet.", MsgBoxStyle.Question + MsgBoxStyle.YesNo, Me.Text) = MsgBoxResult.No Then
                    e.Cancel = True
                    Exit Sub
                End If
            End If
        Catch ex As Exception
            Functions.eventLogFunctions.writeToApplicationLogFile("A crash occurred in the FormClosing event of the Global Application Crash Handler.", EventLogEntryType.Error, False)
            Functions.eventLogFunctions.writeCrashToApplicationLogFile(ex)
        End Try

        Process.GetCurrentProcess.Kill()
    End Sub

    Private Sub frmCrash_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        Control.CheckForIllegalCrossThreadCalls = False
        Media.SystemSounds.Hand.Play()

        If My.Settings.useSSL = True Then
            btnSubmitData.Image = My.Resources.lock
            ToolTip.SetToolTip(btnSubmitData, "Secured by SSL.")
        End If

        Dim strCrashData As String = Functions.eventLogFunctions.assembleCrashData(rawExceptionObject, EventLogEntryType.Error)
        txtStackTrace.Text = strCrashData
        Functions.eventLogFunctions.writeToApplicationLogFile(strCrashData, EventLogEntryType.Error, True)

        Dim stopBitmapIcon As New Bitmap(My.Resources.removeSmall)
        Dim stopIcon As Icon = Icon.FromHandle(stopBitmapIcon.GetHicon())
        Me.Icon = stopIcon

        stopBitmapIcon.Dispose()
        stopIcon.Dispose()
        stopIcon = Nothing
        stopBitmapIcon = Nothing

        If My.Settings.boolSaveInfo Then
            chkSaveInfo.Checked = True
            txtName.Text = My.Settings.usersName
            txtEmail.Text = My.Settings.usersEmail
        End If
    End Sub

    Private Sub Timer1_Tick(sender As Object, e As EventArgs) Handles Timer1.Tick
        If lblHeader.Visible = True Then
            lblHeader.Visible = False
        Else
            lblHeader.Visible = True
        End If
    End Sub

    Sub copyTextToWindowsClipboard(text As String)
        If Functions.support.copyTextToWindowsClipboard(text) Then MsgBox("Copied to Windows Clipboard.", MsgBoxStyle.Information, Me.Text)
    End Sub

    Private boolSubmitted As Boolean = False
    Private boolDoWeHaveAttachments As Boolean = False

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
        httpHelper.addPOSTData("manually", "false")
        httpHelper.addPOSTData("autosubmitted", "false")

        If chkReproducable.Checked = True Then
            httpHelper.addPOSTData("reproducable", "Yes")
        Else
            httpHelper.addPOSTData("reproducable", "No")
        End If

        httpHelper.addPOSTData("crashdata", txtStackTrace.Text)

        If Not String.IsNullOrWhiteSpace(txtDoing.Text.Trim) Then
            httpHelper.addPOSTData("doing", txtDoing.Text)
        End If

        Try
            If chkSendLogs.Checked = True Then
                Dim logCount As ULong = 0

                If Functions.eventLogFunctions.exportLogsToFile(strFileToHaveDataExportedTo, logCount) = True Then
                    If Functions.support.addFileToZipFile(strTempZIPFile, strFileToHaveDataExportedTo) = True Then
                        If IO.File.Exists(strTempZIPFile) = True Then
                            boolDoWeHaveAttachments = True
                            httpHelper.addFileUpload("attachment", strTempZIPFile, Nothing, "application/zip")
                        End If
                    End If
                End If
            End If

            If IO.File.Exists(globalVariables.strDumpFilePath) Then
                If Functions.support.addFileToZipFile(strTempZIPFile, globalVariables.strDumpFilePath) = True Then
                    boolDoWeHaveAttachments = True
                    httpHelper.addFileUpload("attachment", strTempZIPFile, Nothing, "application/zip")
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
                If boolDoWeHaveAttachments = True Then closePleaseWaitPanel()
                deleteTempFiles()

                If strHTTPResponse.Equals("ok", StringComparison.OrdinalIgnoreCase) Then
                    boolSubmitted = True
                    Functions.eventLogFunctions.markLastExceptionLogAsSubmitted()
                    MsgBox("Crash data has been submitted. The program will now close.", MsgBoxStyle.Information + MsgBoxStyle.ApplicationModal, "Restore Point Creator Crash Reporter")
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
                    Debug.WriteLine("HTTP Response = " & strHTTPResponse)
                End If
            Else
                If boolDoWeHaveAttachments = True Then closePleaseWaitPanel()
                deleteTempFiles()

                boolSubmitted = False
                Me.btnSubmitData.Enabled = True
                Me.btnClose.Enabled = True
                MsgBox("Something went wrong while submitting data. Please try again.", MsgBoxStyle.Critical + MsgBoxStyle.ApplicationModal, "Restore Point Creator Crash Reporter")
            End If
        Catch ex As Exception
            If boolDoWeHaveAttachments = True Then closePleaseWaitPanel()
            deleteTempFiles()
            Functions.eventLogFunctions.writeCrashToApplicationLogFile(ex)
        End Try
    End Sub

    Private Sub btnSubmitData_Click(sender As Object, e As EventArgs) Handles btnSubmitData.Click
        If String.IsNullOrWhiteSpace(txtEmail.Text.Trim) Then
            MsgBox("You must provide your email address.", MsgBoxStyle.Critical + MsgBoxStyle.ApplicationModal, Me.Text)
            Exit Sub
        End If

        Dim boolDoWeHaveAttachmentsInLine As Boolean = False

        If IO.File.Exists(globalVariables.strDumpFilePath) Then
            boolDoWeHaveAttachmentsInLine = True
        End If
        If chkSendLogs.Checked = True Then
            boolDoWeHaveAttachmentsInLine = True
        End If

        If boolDoWeHaveAttachmentsInLine = True Then
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

    Private Sub btnClose_Click(sender As Object, e As EventArgs) Handles btnClose.Click
        If boolSubmitted = False Then
            If MsgBox("Are you sure you want to close this window? You have not submitted the crash data yet.", MsgBoxStyle.Question + MsgBoxStyle.YesNo, Me.Text) = MsgBoxResult.Yes Then
                boolSubmitted = True
                Functions.support.deleteFileWithNoException(globalVariables.strDumpFilePath)
                Me.Close()
            End If
        End If
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

Namespace exceptionHandler
    Module exeptionHandler
        Public Sub loadExceptionHandler()
            ' Subscribe to thread (unhandled) exception events
            Dim handler As ThreadExceptionHandler = New ThreadExceptionHandler()
            AddHandler Application.ThreadException, AddressOf handler.Application_ThreadException
            AddHandler AppDomain.CurrentDomain.UnhandledException, AddressOf handler.Application_UnhandledException
        End Sub

        ''' <summary>This function tells the program if the program should handle the exception silently or if it should show the crash message.</summary>
        ''' <param name="exceptionType">The type of the exception.</param>
        ''' <param name="exceptionObject">The exception object from the TRY block.</param>
        ''' <returns>Return a TRUE value to have the program show the crash window. Return a FALSE value to have the program NOT show the crash window.</returns>
        Public Function handleCrashWithAnErrorOrRedirectUserInstead(exceptionType As Type, exceptionObject As Exception) As Boolean
            ' There are three options in the code of this function...
            ' 1. Return a TRUE value to have the program SHOW the crash window to the user.
            ' 2. Return a FALSE value to have the program NOT SHOW the crash window to the user.
            ' 3. Execute a Process.GetCurrentProcess.Kill() to end program execution immediately.

            Dim exceptionMessage As String = exceptionObject.Message

            If exceptionType = GetType(IO.FileLoadException) And exceptionMessage.regExSearch("(?:restoreToSystemRestorePoint|createRestorePoint)") = True Then
                MsgBox("There has been an error while loading a required system library for System Restore. Please reboot your computer and try again.", MsgBoxStyle.Critical + MsgBoxStyle.ApplicationModal, "System Restore Point Creator")
                Process.GetCurrentProcess.Kill()
            ElseIf exceptionType = GetType(Configuration.ConfigurationErrorsException) Or exceptionType = GetType(Configuration.ConfigurationException) Then
                MsgBox("There has been an error in loading the program's user configuration data. Please relaunch the program.", MsgBoxStyle.Critical + MsgBoxStyle.ApplicationModal, "System Restore Point Creator")
                Process.GetCurrentProcess.Kill()
            ElseIf exceptionType = GetType(OutOfMemoryException) Then
                MsgBox("Your system appears to have run out of operating memory. Please reboot your computer and try again." & vbCrLf & vbCrLf & "This program will now close.", MsgBoxStyle.Information, "System Restore Point Creator")
                Process.GetCurrentProcess.Kill()
            ElseIf exceptionType = GetType(UnauthorizedAccessException) Then
                Functions.eventLogFunctions.writeCrashToApplicationLogFile(exceptionObject)

                MsgBox("It appears that you don't have the necessary user permissions to do what you are attempting to do. Please contact your system administrator for guidance.", MsgBoxStyle.Exclamation, "System Restore Point Creator")

                Return False
            ElseIf exceptionType = GetType(BadImageFormatException) Then
                Functions.eventLogFunctions.writeCrashToApplicationLogFile(exceptionObject)

                MsgBox("There was an error while loading a required system library. Please check the Event Log Viewer for more information.", MsgBoxStyle.Exclamation, "System Restore Point Creator")

                Return False
            ElseIf exceptionType = GetType(ObjectDisposedException) And exceptionMessage.regExSearch("(?:Please_Wait|SmoothProgressBar|pleaseWaitlblLabel)") = True Then
                ' This is to hopefully catch an annoying crash that I've not been able to track down
                ' so we're going to simply handle it silently with no notification to the user.
                Functions.eventLogFunctions.writeCrashToApplicationLogFile(exceptionObject)
                Return False
            ElseIf exceptionType = GetType(Runtime.InteropServices.COMException) Then
                Functions.eventLogFunctions.writeCrashToApplicationLogFile(exceptionObject)

                MsgBox("System-level APIs and functions that this program requires to operate appear to be damaged or disabled on your system. Your web browser will be opened to a web page with information on how to repair your system." & vbCrLf & vbCrLf & "This program will now close.", MsgBoxStyle.Information, "System Restore Point Creator")

                Functions.support.launchURLInWebBrowser(globalVariables.webURLs.webPages.strCOMExceptionCrash)
                Process.GetCurrentProcess.Kill()
            ElseIf exceptionType = GetType(Management.ManagementException) Then
                Functions.eventLogFunctions.writeCrashToApplicationLogFile(exceptionObject)

                MsgBox("System-level APIs and functions that this program requires to operate appear to be damaged or disabled on your system. Your web browser will be opened to a web page with information on how to repair your system." & vbCrLf & vbCrLf & "This program will now close.", MsgBoxStyle.Information, "System Restore Point Creator")

                Functions.support.launchURLInWebBrowser(globalVariables.webURLs.webPages.strCOMExceptionCrash)
                Process.GetCurrentProcess.Kill()
            ElseIf exceptionType = GetType(TypeInitializationException) And exceptionObject.Message.caseInsensitiveContains("Microsoft.Win32.TaskScheduler.TaskService") Then
                ' Fixes an issue that causes the Crash Handling Window to appear during updates on systems with Avast antivirus installed on them.
                Return False
            End If

            Return True ' This function returns a TRUE statement by default which will cause the program to show the crash window.
        End Function

        ''' <summary>This function tells the program if the program should handle the exception silently or if it should show the crash message.</summary>
        ''' <param name="exceptionObject">The exception object from the TRY block.</param>
        ''' <returns>Return a TRUE value to have the program show the crash window. Return a FALSE value to have the program NOT show the crash window.</returns>
        Public Function handleCrashWithAnErrorOrRedirectUserInstead(exceptionObject As Exception) As Boolean
            Return handleCrashWithAnErrorOrRedirectUserInstead(exceptionObject.GetType, exceptionObject)
        End Function

        Public Sub manuallyLoadCrashWindow(exceptionObject As Exception)
            manuallyLoadCrashWindow(exceptionObject, Functions.support.removeSourceCodePathInfo(exceptionObject.Message), Functions.support.removeSourceCodePathInfo(exceptionObject.StackTrace), exceptionObject.GetType)
        End Sub

        Public Sub manuallyLoadCrashWindow(exceptionObject As Exception, exceptionMessage As String, exceptionStackTrace As String, exceptionType As Type)
            exceptionMessage = Functions.support.removeSourceCodePathInfo(exceptionMessage).Trim
            exceptionStackTrace = Functions.support.removeSourceCodePathInfo(exceptionStackTrace).Trim

            If handleCrashWithAnErrorOrRedirectUserInstead(exceptionType, exceptionObject) = True Then
                Try
                    Functions.miniDump.MiniDump.MiniDumpToFile(globalVariables.strDumpFilePath)
                Catch Ex As Exception
                    ' Does nothing
                End Try

                Dim crashWindow As New frmCrash
                crashWindow.Text = "Critical Application Error Detected!"
                crashWindow.lblHeader.Text = "Critical Application Error Detected!"
                crashWindow.exceptionStackTrace = exceptionStackTrace
                crashWindow.exceptionMessage = exceptionMessage
                crashWindow.exceptionType = exceptionType.ToString
                crashWindow.rawExceptionObject = exceptionObject
                crashWindow.ShowDialog()

                Process.GetCurrentProcess.Kill()
            End If
        End Sub
    End Module
End Namespace

Friend Class ThreadExceptionHandler
    '''
    ''' Handles the thread exception.
    '''
    Public Sub Application_ThreadException(ByVal sender As Object, ByVal exceptionObject As Threading.ThreadExceptionEventArgs)
        Try
            writeCrashToEventLogAndGiveErrorMessage(exceptionObject.Exception)
        Catch ex As Exception
        End Try
    End Sub

    '''
    ''' Handles the thread exception.
    '''
    Public Sub Application_UnhandledException(sender As Object, exceptionObject As UnhandledExceptionEventArgs)
        Try
            writeCrashToEventLogAndGiveErrorMessage(exceptionObject.ExceptionObject)
        Catch ex As Exception
        End Try
    End Sub

    Private Sub writeCrashToEventLogAndGiveErrorMessage(exceptionObject As Exception)
        Try
            Functions.eventLogFunctions.writeCrashToApplicationLogFile(exceptionObject)
            MsgBox("A fatal error occurred and the program will now close. Please check the application event log for more details.", MsgBoxStyle.Critical + MsgBoxStyle.ApplicationModal, "Restore Point Creator Fatal Program Crash")
            Process.GetCurrentProcess.Kill()
        Catch ex As Exception
        End Try
    End Sub
End Class