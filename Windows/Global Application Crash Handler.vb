Imports System.Text

Public Class frmCrash
    Property exceptionMessage As String
    Property exceptionStackTrace As String
    Property exceptionType As String

    Private strFileToHaveDataExportedTo As String = IO.Path.Combine(IO.Path.GetTempPath(), "event log entries.reslog")
    Private strTempZIPFile As String = IO.Path.Combine(IO.Path.GetTempPath(), "attachments.zip")

    Sub deleteFileWithCrashPrevention(strPathToFile As String)
        Try
            If IO.File.Exists(strPathToFile) = True Then IO.File.Delete(strPathToFile)
        Catch ex As Exception
        End Try
    End Sub

    Sub deleteTempFiles()
        If boolDoWeHaveAttachments = True Then
            deleteFileWithCrashPrevention(strTempZIPFile)
            deleteFileWithCrashPrevention(strFileToHaveDataExportedTo)
            deleteFileWithCrashPrevention(globalVariables.strDumpFilePath)
        End If
    End Sub

    Private Sub frmCrash_FormClosing(sender As Object, e As FormClosingEventArgs) Handles Me.FormClosing
        If boolSubmitted = False Then
            If MsgBox("Are you sure you want to close this window? You have not submitted the crash data yet.", MsgBoxStyle.Question + MsgBoxStyle.YesNo, Me.Text) = MsgBoxResult.No Then
                e.Cancel = True
                Exit Sub
            End If
        End If

        Process.GetCurrentProcess.Kill()
    End Sub

    Private Sub frmCrash_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        Control.CheckForIllegalCrossThreadCalls = False
        Media.SystemSounds.Hand.Play()

        If My.Settings.useSSL = True Then
            btnSubmitData.Image = My.Resources.lock
            ToolTip.SetToolTip(btnSubmitData, "Secured by SSL.")
        End If

        Dim stringBuilder As New StringBuilder

        stringBuilder.AppendLine("System Information")
        stringBuilder.AppendLine("Time of Crash: " & Now.ToString)
        stringBuilder.AppendLine("Operating System: " & Functions.osVersionInfo.getFullOSVersionString())
        stringBuilder.AppendLine("System RAM: " & Functions.wmi.getSystemRAM())

        Dim processorInfo As Functions.supportClasses.processorInfoClass = Functions.wmi.getSystemProcessor()
        stringBuilder.AppendLine("CPU: " & processorInfo.strProcessor)
        stringBuilder.AppendLine("Number of Cores: " & processorInfo.strNumberOfCores.ToString)

        stringBuilder.AppendLine()

        If globalVariables.version.boolBeta = True Then
            stringBuilder.AppendLine("Program Version: " & String.Format("{0} Public Beta {1}", globalVariables.version.strFullVersionString, globalVariables.version.shortBetaVersion))
        ElseIf globalVariables.version.boolReleaseCandidate = True Then
            stringBuilder.AppendLine("Program Version: " & String.Format("{0} Release Candidate {1}", globalVariables.version.strFullVersionString, globalVariables.version.shortReleaseCandidateVersion))
        Else
            stringBuilder.AppendLine("Program Version: " & globalVariables.version.strFullVersionString)
        End If

        If globalVariables.version.boolDebugBuild = True Then
            stringBuilder.AppendLine("Debug Build: Yes")
        Else
            stringBuilder.AppendLine("Debug Build: No")
        End If

        stringBuilder.AppendLine("Running As: " & Environment.UserName)
        stringBuilder.AppendLine("Exception Type: " & exceptionType)
        stringBuilder.AppendLine("Message: " & exceptionMessage)

        stringBuilder.AppendLine()

        stringBuilder.Append("The exception occurred ")

        stringBuilder.AppendLine(exceptionStackTrace.Trim)

        txtStackTrace.Text = stringBuilder.ToString.Trim
        Functions.eventLogFunctions.writeToSystemEventLog(stringBuilder.ToString.Trim, EventLogEntryType.Error)
        stringBuilder = Nothing

        Dim stopBitmapIcon As New Bitmap(My.Resources.removeSmall)
        Dim stopIcon As Icon = Icon.FromHandle(stopBitmapIcon.GetHicon())
        Me.Icon = stopIcon

        stopBitmapIcon.Dispose()
        stopIcon.Dispose()
        stopIcon = Nothing
        stopBitmapIcon = Nothing
    End Sub

    'Private Sub btnSubmitCrashData_Click(sender As System.Object, e As System.EventArgs) Handles btnSubmitCrashData.Click

    '    Dim submitThread As New Threading.Thread(Sub()
    '                                                 Dim msgBoxResult As MsgBoxResult

    '                                                 If txtEmailAddress.Text.Trim = Nothing Then
    '                                                     msgBoxResult = MsgBox("You did not enter an email address." & vbCrLf & vbCrLf & "If you don't want to provide an email address, press the OK button.  Press the Cancel button to try again.", MsgBoxStyle.Information + MsgBoxStyle.OkCancel, Me.Text)

    '                                                     If msgBoxResult = Microsoft.VisualBasic.MsgBoxResult.Cancel Then Exit Sub
    '                                                 End If

    '                                                 msgBoxResult = Microsoft.VisualBasic.MsgBoxResult.Retry

    '                                                 If txtEmailAddress.Text.Trim <> Nothing And System.Text.RegularExpressions.Regex.IsMatch(txtEmailAddress.Text.Trim, "[a-z0-9!#$%&'*+/=?^_`{|}~-]+(?:\.[a-z0-9!#$%&'*+/=?^_`{|}~-]+)*@(?:[a-z0-9](?:[a-z0-9-]*[a-z0-9])?\.)+[a-z0-9](?:[a-z0-9-]*[a-z0-9])?") = False Then
    '                                                     msgBoxResult = MsgBox("Invalid Email Address Format." & vbCrLf & vbCrLf & "If you don't want to provide an email address, press the OK button.  Press the Cancel button to try again.", MsgBoxStyle.Information + MsgBoxStyle.OkCancel, Me.Text)

    '                                                     If msgBoxResult = Microsoft.VisualBasic.MsgBoxResult.Cancel Then
    '                                                         Exit Sub
    '                                                     ElseIf msgBoxResult = Microsoft.VisualBasic.MsgBoxResult.Ok Then
    '                                                         txtEmailAddress.Text = ""
    '                                                     End If
    '                                                 End If

    '                                                 Dim stringWhatWereYouDoing As String = txtWhatWereYouDoing.Text
    '                                                 If stringWhatWereYouDoing.Trim = Nothing Then
    '                                                     stringWhatWereYouDoing = "(Nothing Provided)"
    '                                                 End If

    '                                                 Dim dataToBeSent As String = String.Format("doing={0}&program={1}&error={2}", UrlEncode(stringWhatWereYouDoing), UrlEncode(Application.ProductName), UrlEncode(txtStackTrace.Text))

    '                                                 If (txtEmailAddress.Text.Trim = Nothing) = False Then
    '                                                     dataToBeSent &= "&email=" & UrlEncode(txtEmailAddress.Text)
    '                                                 End If

    '                                                 If dataToBeSent.Trim = Nothing Then
    '                                                     MsgBox("Something went wrong, post data is invalid.", MsgBoxStyle.Information, Me.Text)
    '                                                     Exit Sub
    '                                                 End If

    '                                                 Dim webPageResults As String = postData(dataToBeSent, "http://www.toms-world.org/crashReporter")
    '                                                 boolSubmittedCrashData = True
    '                                                 dataToBeSent = Nothing
    '                                                 stringWhatWereYouDoing = Nothing

    '                                                 If webPageResults = "ok" Then
    '                                                     MsgBox("Bug report submitted successfully.", MsgBoxStyle.Information, Me.Text)
    '                                                 Else
    '                                                     MsgBox("Bug report submission error." & vbCrLf & vbCrLf & "Error: " & webPageResults, MsgBoxStyle.Information, Me.Text)
    '                                                 End If

    '                                                 Process.GetCurrentProcess.Kill()
    '                                             End Sub)
    '    submitThread.Name = "Data Submission Thread"
    '    submitThread.Start()
    'End Sub

    Private Sub Timer1_Tick(sender As Object, e As EventArgs) Handles Timer1.Tick
        If lblHeader.Visible = True Then
            lblHeader.Visible = False
        Else
            lblHeader.Visible = True
        End If
    End Sub

    Sub copyTextToWindowsClipboard(text As String)
        Clipboard.SetText(text)
        MsgBox("Copied to Windows Clipboard.", MsgBoxStyle.Information, Me.Text)
    End Sub

    'Private Sub btnSubmitAutomatically_Click(sender As Object, e As EventArgs) Handles btnSubmitAutomatically.Click
    '    Dim frmSubmitCrashDataInstance As New frmSubmitCrashData
    '    frmSubmitCrashDataInstance.Icon = My.Resources.RestorePoint_noBackground_2
    '    frmSubmitCrashDataInstance.crashData = txtStackTrace.Text
    '    frmSubmitCrashDataInstance.StartPosition = FormStartPosition.CenterParent
    '    frmSubmitCrashDataInstance.ShowDialog()
    'End Sub

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

        If chkReproducable.Checked = True Then
            httpHelper.addPOSTData("reproducable", "Yes")
        Else
            httpHelper.addPOSTData("reproducable", "No")
        End If

        httpHelper.addPOSTData("crashdata", txtStackTrace.Text)

        If txtDoing.Text.Trim <> Nothing Then
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
                If boolDoWeHaveAttachments = True Then Functions.wait.closePleaseWaitWindow()
                deleteTempFiles()

                Select Case strHTTPResponse
                    Case "ok"
                        boolSubmitted = True
                        MsgBox("Crash data has been submitted. The program will now close.", MsgBoxStyle.Information, "Restore Point Creator Crash Reporter")
                        Me.Close()
                    Case "error"
                        boolSubmitted = False
                        Me.btnSubmitData.Enabled = True
                        Me.btnClose.Enabled = True
                        MsgBox("There was an error in submission. Please try again.", MsgBoxStyle.Critical, "Restore Point Creator Crash Reporter")
                    Case "error-invalid-email"
                        boolSubmitted = False
                        Me.btnSubmitData.Enabled = True
                        Me.btnClose.Enabled = True
                        MsgBox("Invalid email address. Please try again.", MsgBoxStyle.Critical, "Restore Point Creator Crash Reporter")
                    Case "email-server-said-user-doesnt-exist"
                        boolSubmitted = False
                        Me.btnSubmitData.Enabled = True
                        Me.btnClose.Enabled = True
                        MsgBox("The remote email server said that the email address doesn't exist. Please try again.", MsgBoxStyle.Critical, "Restore Point Creator Crash Reporter")
                    Case "no-email-servers-contactable"
                        boolSubmitted = False
                        Me.btnSubmitData.Enabled = True
                        Me.btnClose.Enabled = True
                        MsgBox("No mail servers found, more than likely your email address is invalid. Please try again.", MsgBoxStyle.Critical, "Restore Point Creator Crash Reporter")
                    Case "dns-error"
                        boolSubmitted = False
                        Me.btnSubmitData.Enabled = True
                        Me.btnClose.Enabled = True
                        MsgBox("The domain name doesn't exist. Please try again.", MsgBoxStyle.Critical, "Restore Point Creator Crash Reporter")
                    Case "server-connect-error"
                        boolSubmitted = False
                        Me.btnSubmitData.Enabled = True
                        Me.btnClose.Enabled = True
                        MsgBox("Unable to contact mail server, more than likely your email address is invalid. Please try again.", MsgBoxStyle.Critical, "Restore Point Creator Crash Reporter")
                    Case "error-no-crash-data-found"
                        boolSubmitted = False
                        Me.btnSubmitData.Enabled = True
                        Me.btnClose.Enabled = True
                        MsgBox("Something went wrong, no crash data found in submission data. Please try again.", MsgBoxStyle.Critical, "Restore Point Creator Crash Reporter")
                    Case "error-no-program-code-found"
                        boolSubmitted = False
                        Me.btnSubmitData.Enabled = True
                        Me.btnClose.Enabled = True
                        MsgBox("Something went wrong, no program code found in submission data. Please try again.", MsgBoxStyle.Critical, "Restore Point Creator Crash Reporter")
                    Case "error-no-email-address-found"
                        boolSubmitted = False
                        Me.btnSubmitData.Enabled = True
                        Me.btnClose.Enabled = True
                        MsgBox("Something went wrong, no email address found in submission data. Please try again.", MsgBoxStyle.Critical, "Restore Point Creator Crash Reporter")
                    Case "error-no-name-found"
                        boolSubmitted = False
                        Me.btnSubmitData.Enabled = True
                        Me.btnClose.Enabled = True
                        MsgBox("Something went wrong, no name found in submission data. Please try again.", MsgBoxStyle.Critical, "Restore Point Creator Crash Reporter")
                    Case "invalid-email-syntax"
                        boolSubmitted = False
                        Me.btnSubmitData.Enabled = True
                        Me.btnClose.Enabled = True
                        MsgBox("The email address didn't pass syntax validation. Please try again.", MsgBoxStyle.Critical, "Restore Point Creator Crash Reporter")
                    Case Else
                        boolSubmitted = True
                        Me.btnSubmitData.Enabled = True
                        Me.btnClose.Enabled = True
                        Debug.WriteLine("HTTP Response = " & strHTTPResponse)
                End Select
            Else
                If boolDoWeHaveAttachments = True Then Functions.wait.closePleaseWaitWindow()
                deleteTempFiles()

                boolSubmitted = False
                Me.btnSubmitData.Enabled = True
                Me.btnClose.Enabled = True
                MsgBox("Something went wrong while submitting data. Please try again.", MsgBoxStyle.Critical, "Restore Point Creator Crash Reporter")
            End If
        Catch ex As Exception
            If boolDoWeHaveAttachments = True Then Functions.wait.closePleaseWaitWindow()
            deleteTempFiles()
            Functions.eventLogFunctions.writeCrashToEventLog(ex)
        End Try
    End Sub

    Private Sub btnSubmitData_Click(sender As Object, e As EventArgs) Handles btnSubmitData.Click
        If txtEmail.Text.Trim = Nothing Then
            MsgBox("You must provide your email address.", MsgBoxStyle.Critical, Me.Text)
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
            Functions.wait.createPleaseWaitWindow("Compressing and Sending Data... Please Wait.")
        End If

        Dim submitThread As New Threading.Thread(AddressOf dataSubmitThread)
        submitThread.Name = "Crash Data Submission Thread"
        submitThread.Start()

        If boolDoWeHaveAttachmentsInLine = True Then
            Functions.wait.openPleaseWaitWindow(Me)
        End If
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
                MsgBox("There has been an error while loading a required system library for System Restore. Please reboot your computer and try again.", MsgBoxStyle.Critical, "System Restore Point Creator")
                Process.GetCurrentProcess.Kill()
            ElseIf exceptionType = GetType(Configuration.ConfigurationErrorsException) Or exceptionType = GetType(Configuration.ConfigurationException) Then
                MsgBox("There has been an error in loading the program's user configuration data. Please relaunch the program.", MsgBoxStyle.Critical, "System Restore Point Creator")
                Process.GetCurrentProcess.Kill()
            ElseIf exceptionType = GetType(OutOfMemoryException) Then
                MsgBox("Your system appears to have run out of operating memory. Please reboot your computer and try again." & vbCrLf & vbCrLf & "This program will now close.", MsgBoxStyle.Information, "System Restore Point Creator")
                Process.GetCurrentProcess.Kill()
            ElseIf exceptionType = GetType(UnauthorizedAccessException) Then
                Functions.eventLogFunctions.writeCrashToEventLog(exceptionObject)

                MsgBox("It appears that you don't have the necessary user permissions to do what you are attempting to do. Please contact your system administrator for guidance.", MsgBoxStyle.Exclamation, "System Restore Point Creator")

                Return False
            ElseIf exceptionType = GetType(BadImageFormatException) Then
                Functions.eventLogFunctions.writeCrashToEventLog(exceptionObject)

                MsgBox("There was an error while loading a required system library. Please check the Event Log Viewer for more information.", MsgBoxStyle.Exclamation, "System Restore Point Creator")

                Return False
            ElseIf exceptionType = GetType(ObjectDisposedException) And exceptionMessage.regExSearch("(?:Please_Wait|SmoothProgressBar|pleaseWaitlblLabel)") = True Then
                ' This is to hopefully catch an annoying crash that I've not been able to track down
                ' so we're going to simply handle it silently with no notification to the user.
                Functions.eventLogFunctions.writeCrashToEventLog(exceptionObject)
                Return False
            ElseIf exceptionType = GetType(Runtime.InteropServices.COMException) Then
                Functions.eventLogFunctions.writeCrashToEventLog(exceptionObject)

                MsgBox("System-level APIs and functions that this program requires to operate appear to be damaged or disabled on your system. Your web browser will be opened to a web page with information on how to repair your system." & vbCrLf & vbCrLf & "This program will now close.", MsgBoxStyle.Information, "System Restore Point Creator")

                Functions.support.launchURLInWebBrowser(globalVariables.webURLs.webPages.strCOMExceptionCrash)
                Process.GetCurrentProcess.Kill()
                ' ElseIf exceptionType = GetType(Management.ManagementException) And (exceptionMessage.caseInsensitiveContains("createRestorePoint") = True Or exceptionMessage.caseInsensitiveContains("restoreToSystemRestorePoint") = True) Then
            ElseIf exceptionType = GetType(Management.ManagementException) Then
                Functions.eventLogFunctions.writeCrashToEventLog(exceptionObject)

                MsgBox("System-level APIs and functions that this program requires to operate appear to be damaged or disabled on your system. Your web browser will be opened to a web page with information on how to repair your system." & vbCrLf & vbCrLf & "This program will now close.", MsgBoxStyle.Information, "System Restore Point Creator")

                Functions.support.launchURLInWebBrowser(globalVariables.webURLs.webPages.strCOMExceptionCrash)
                Process.GetCurrentProcess.Kill()
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
            exceptionMessage = Functions.support.removeSourceCodePathInfo(exceptionMessage)
            exceptionStackTrace = Functions.support.removeSourceCodePathInfo(exceptionStackTrace)

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
            If exceptionHandler.handleCrashWithAnErrorOrRedirectUserInstead(exceptionObject.Exception) = True Then
            	Try
                	Functions.miniDump.MiniDump.MiniDumpToFile(globalVariables.strDumpFilePath)
            	Catch Ex As Exception
            		' Does nothing
            	End Try

                Dim crashWindow As New frmCrash
                Threading.Thread.CurrentThread.CurrentUICulture = New Globalization.CultureInfo("en-US")
                crashWindow.exceptionMessage = exceptionObject.Exception.Message
                crashWindow.exceptionStackTrace = exceptionObject.Exception.StackTrace
                crashWindow.exceptionType = exceptionObject.Exception.GetType.ToString
                crashWindow.ShowDialog()

                Dim currentProcess As Process = Process.GetCurrentProcess()
                currentProcess.Kill()
            End If
        Catch
            ' Fatal error, terminate program
            Try
                MessageBox.Show("Fatal Error", "Fatal Error", MessageBoxButtons.OK, MessageBoxIcon.Stop)
            Finally
                Dim currentProcess As Process = Process.GetCurrentProcess()
                currentProcess.Kill()
            End Try
        End Try
    End Sub

    '''
    ''' Handles the thread exception.
    '''
    Public Sub Application_UnhandledException(sender As Object, exceptionObject As UnhandledExceptionEventArgs)
        Functions.eventLogFunctions.writeCrashToEventLog(exceptionObject.ExceptionObject)

        Dim crashWindow As New frmCrash
        Threading.Thread.CurrentThread.CurrentUICulture = New Globalization.CultureInfo("en-US")
        crashWindow.exceptionMessage = exceptionObject.ExceptionObject.Exception.Message
        crashWindow.exceptionStackTrace = exceptionObject.ExceptionObject.Exception.StackTrace
        crashWindow.exceptionType = exceptionObject.ExceptionObject.Exception.GetType.ToString
        crashWindow.ShowDialog()

        Dim currentProcess As Process = Process.GetCurrentProcess()
        Environment.Exit(1)

        'Try
        '    If exceptionHandler.handleCrashWithAnErrorOrRedirectUserInstead(exceptionObject.Exception) = True Then
        '        Dim crashWindow As New frmCrash
        '        Threading.Thread.CurrentThread.CurrentUICulture = New Globalization.CultureInfo("en-US")
        '        crashWindow.exceptionMessage = exceptionObject.Exception.Message
        '        crashWindow.exceptionStackTrace = exceptionObject.Exception.StackTrace
        '        crashWindow.exceptionType = exceptionObject.Exception.GetType.ToString
        '        crashWindow.ShowDialog()

        '        Dim currentProcess As Process = Process.GetCurrentProcess()
        '        currentProcess.Kill()
        '    End If
        'Catch
        '    ' Fatal error, terminate program
        '    Try
        '        MessageBox.Show("Fatal Error", "Fatal Error", MessageBoxButtons.OK, MessageBoxIcon.Stop)
        '    Finally
        '        Dim currentProcess As Process = Process.GetCurrentProcess()
        '        currentProcess.Kill()
        '    End Try
        'End Try
    End Sub
End Class