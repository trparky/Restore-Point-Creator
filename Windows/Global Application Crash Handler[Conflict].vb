Public Class frmCrash
    Property exceptionMessage As String
    Property exceptionStackTrace As String
    Property exceptionType As String

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

        If My.Settings.useSSL = True And GlobalVariables.boolWinXP = False Then
            btnSubmitData.Image = My.Resources.lock
            ToolTip.SetToolTip(btnSubmitData, "Secured by SSL.")
        End If

        Dim stringBuilder As New Text.StringBuilder

        stringBuilder.AppendLine("System Information")
        stringBuilder.AppendLine("Time of Crash: " & Now.ToString)
        stringBuilder.AppendLine("Operating System: " & Functions.getFullOSVersionString())
        stringBuilder.AppendLine("System RAM: " & Functions.getSystemRAM())

        Dim cpuInfo As Dictionary(Of String, String) = Functions.getSystemProcessor()
        stringBuilder.AppendLine("CPU: " & cpuInfo("cpuname"))
        stringBuilder.AppendLine("Number of Cores: " & cpuInfo("numberofcores"))

        stringBuilder.AppendLine()

        If GlobalVariables.boolBeta = True Then
            stringBuilder.AppendLine("Program Version: " & String.Format("{0} Public Beta {1}", GlobalVariables.versionString, GlobalVariables.shortBetaVersion))
        ElseIf GlobalVariables.boolReleaseCandidate = True Then
            stringBuilder.AppendLine("Program Version: " & String.Format("{0} Release Candidate {1}", GlobalVariables.versionString, GlobalVariables.shortReleaseCandidateVersion))
        Else
            stringBuilder.AppendLine("Program Version: " & GlobalVariables.versionString)
        End If

        If GlobalVariables.boolDebugBuild = True Then
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
        Functions.writeToSystemEventLog(stringBuilder.ToString.Trim, EventLogEntryType.Error)
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

    Private Sub LinkLabel2_LinkClicked(sender As Object, e As LinkLabelLinkClickedEventArgs) Handles LinkLabel2.LinkClicked
        Functions.launchURLInWebBrowser("http://bit.ly/winrepairaio")
    End Sub

    Sub copyTextToWindowsClipboard(text As String)
        Clipboard.SetText(text)
        MsgBox("Copied to Windows Clipboard.", MsgBoxStyle.Information, Me.Text)
    End Sub

    Private Sub Button1_Click(sender As Object, e As EventArgs) Handles Button1.Click
        Dim stringBuilder As New Text.StringBuilder
        stringBuilder.AppendLine("If you receive a program crash that looks similar to the following...")
        stringBuilder.AppendLine()
        stringBuilder.AppendLine("Message: Exception Type: System.Runtime.InteropServices.COMException The exception occurred at System.Runtime.InteropServices.Marshal.ThrowExceptionForHRInternal(Int32 errorCode, IntPtr errorInfo) at System.Runtime.InteropServices.Marshal.ThrowExceptionForHR(Int32 errorCode) at System.Management.ManagementObject.InvokeMethod(String methodName, ManagementBaseObject inParameters, InvokeMethodOptions options) at Restore_Point_Creator.Functions.Functions.createRestorePoint(String strDescription, RestoreType rt, Int64& lSeqNum)")
        stringBuilder.AppendLine()
        stringBuilder.AppendLine("Your system's Windows operating system is in need of repair.  Please go to http://bit.ly/winrepairaio and download the Windows Repair (All In One) tool and run the tool to repair your system.  Once you run the program, run the following repairs...")
        stringBuilder.AppendLine()
        stringBuilder.AppendLine("01 – Reset Registry Permissions")
        stringBuilder.AppendLine("03 – Register System Files")
        stringBuilder.AppendLine("04 – Repair WMI")
        stringBuilder.AppendLine("18 – Repair Volume Shadow Copy Service")
        stringBuilder.AppendLine("25 – Restore Important Windows Services")
        stringBuilder.AppendLine("26 – Set Windows Services to Default Startup")
        stringBuilder.AppendLine()
        stringBuilder.AppendLine("If this doesn’t work for you then your system will need to be re-installed from your Windows installation media.")

        copyTextToWindowsClipboard(stringBuilder.ToString.Trim)
    End Sub

    'Private Sub btnSubmitAutomatically_Click(sender As Object, e As EventArgs) Handles btnSubmitAutomatically.Click
    '    Dim frmSubmitCrashDataInstance As New frmSubmitCrashData
    '    frmSubmitCrashDataInstance.Icon = My.Resources.RestorePoint_noBackground_2
    '    frmSubmitCrashDataInstance.crashData = txtStackTrace.Text
    '    frmSubmitCrashDataInstance.StartPosition = FormStartPosition.CenterParent
    '    frmSubmitCrashDataInstance.ShowDialog()
    'End Sub

    Private boolSubmitted As Boolean = False

    Private Sub btnSubmitData_Click(sender As Object, e As EventArgs) Handles btnSubmitData.Click
        Dim submitThread As New Threading.Thread(Sub()
                                                     txtEmail.Text = txtEmail.Text.Trim
                                                     txtName.Text = txtName.Text.Trim

                                                     If txtEmail.Text = Nothing Then
                                                         MsgBox("You must provide your email address.", MsgBoxStyle.Critical, Me.Text)
                                                         Exit Sub
                                                     End If

                                                     Me.btnSubmitData.Enabled = False
                                                     Me.btnClose.Enabled = False

                                                     Dim httpPOSTData As New Dictionary(Of String, String)
                                                     httpPOSTData.Add("name", txtName.Text)
                                                     httpPOSTData.Add("email", txtEmail.Text)
                                                     httpPOSTData.Add("program", GlobalVariables.programName)
                                                     httpPOSTData.Add("submissionversion", "3")

                                                     If chkReproducable.Checked = True Then
                                                         httpPOSTData.Add("reproducable", "Yes")
                                                     Else
                                                         httpPOSTData.Add("reproducable", "No")
                                                     End If

                                                     httpPOSTData.Add("crashdata", txtStackTrace.Text)

                                                     If txtDoing.Text.Trim <> Nothing Then
                                                         httpPOSTData.Add("doing", txtDoing.Text)
                                                     End If

                                                     Dim strHTTPResponse As String = Functions.postDataToWebPage(GlobalVariables.crashFromURL, httpPOSTData).Trim

                                                     Select Case strHTTPResponse
                                                         Case "ok"
                                                             boolSubmitted = True
                                                             MsgBox("Crash data has been submitted. The program will now close.", MsgBoxStyle.Information, "Restore Point Creator Crash Reporter")
                                                             Process.GetCurrentProcess.Kill()
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
                                                         Case Else
                                                             boolSubmitted = True
                                                             Me.btnSubmitData.Enabled = True
                                                             Me.btnClose.Enabled = True
                                                             Debug.WriteLine("HTTP Response = " & strHTTPResponse)
                                                     End Select
                                                 End Sub)

        submitThread.Name = "Crash Data Submission Thread"
        submitThread.Start()
    End Sub

    Private Sub btnClose_Click(sender As Object, e As EventArgs) Handles btnClose.Click
        If boolSubmitted = False Then
            If MsgBox("Are you sure you want to close this window? You have not submitted the crash data yet.", MsgBoxStyle.Question + MsgBoxStyle.YesNo, Me.Text) = MsgBoxResult.Yes Then
                boolSubmitted = True
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
            'AddHandler AppDomain.CurrentDomain.UnhandledException, AddressOf handler.Application_UnhandledException
            AddHandler Application.ThreadException, AddressOf handler.Application_ThreadException
        End Sub

        'Public Sub manuallyLoadCrashWindow(ex As Exception)
        '    Dim crashWindow As New frmCrash
        '    crashWindow.crashData = ex
        '    crashWindow.Text = "Critical Application Error Detected!"
        '    crashWindow.lblHeader.Text = "Critical Application Error Detected!"
        '    crashWindow.ShowDialog()
        '    Dim currentProcess As Process = Process.GetCurrentProcess()
        '    currentProcess.Kill()
        'End Sub

        Public Function handleCrashWithAnErrorInstead(exceptionType As String, exceptionObject As Exception) As Boolean
            ' Return a TRUE value to have the program show the crash window.
            ' Return a FALSE value to have the program NOT show the crash window.

            If exceptionType.caseInsensitiveContains("ConfigurationErrorsException") = True Then
                MsgBox("There has been an error in loading the program's user configuration data. Please relaunch the program.", MsgBoxStyle.Critical, "System Restore Point Creator")
                Process.GetCurrentProcess.Kill()
            ElseIf exceptionType.caseInsensitiveContains("UnauthorizedAccessException") = True Then
                Functions.writeCrashToEventLog(exceptionObject)

                MsgBox("It appears that you don't have the necessary user permissions to do what you are attempting to do. Please contact your system administrator for guidance.", MsgBoxStyle.Exclamation, "System Restore Point Creator")

                Return False
            ElseIf exceptionType.caseInsensitiveContains("BadImageFormatException") = True Then
                Functions.writeCrashToEventLog(exceptionObject)

                MsgBox("There was an error while loading a required system library. Please check the Event Log Viewer for more information.", MsgBoxStyle.Exclamation, "System Restore Point Creator")

                Return False
            ElseIf exceptionType.caseInsensitiveContains("ObjectDisposedException") = True And exceptionObject.Message.caseInsensitiveContains("Please_Wait") = True Then
                ' This is to hopefully catch an annoying crash that I've not been able to track down
                ' so we're going to simply handle it silently with no notification to the user.
                Return False
            End If

            Return True ' This function returns a TRUE statement by default which will cause the program to show the crash window.
        End Function

        Public Function handleCrashWithAnErrorInstead(exceptionObject As Exception) As Boolean
            Return handleCrashWithAnErrorInstead(exceptionObject.GetType.ToString.Trim, exceptionObject)
        End Function

        Public Sub manuallyLoadCrashWindow(exceptionObject As Exception)
            manuallyLoadCrashWindow(exceptionObject, Functions.removeSourceCodePathInfo(exceptionObject.Message), Functions.removeSourceCodePathInfo(exceptionObject.StackTrace), exceptionObject.GetType.ToString)
        End Sub

        Public Sub manuallyLoadCrashWindow(exceptionObject As Exception, exceptionMessage As String, exceptionStackTrace As String, exceptionType As Type)
            manuallyLoadCrashWindow(exceptionObject, Functions.removeSourceCodePathInfo(exceptionMessage), Functions.removeSourceCodePathInfo(exceptionStackTrace), exceptionType.ToString)
        End Sub

        Public Sub manuallyLoadCrashWindow(exceptionObject As Exception, exceptionMessage As String, exceptionStackTrace As String, exceptionType As String)
            exceptionMessage = Functions.removeSourceCodePathInfo(exceptionMessage)
            exceptionStackTrace = Functions.removeSourceCodePathInfo(exceptionStackTrace)

            Dim handleCrashWithAnErrorInsteadResult As Boolean = handleCrashWithAnErrorInstead(exceptionType, exceptionObject)

            If handleCrashWithAnErrorInsteadResult = True Then
                Dim crashWindow As New frmCrash
                crashWindow.Text = "Critical Application Error Detected!"
                crashWindow.lblHeader.Text = "Critical Application Error Detected!"
                crashWindow.exceptionStackTrace = exceptionStackTrace
                crashWindow.exceptionMessage = exceptionMessage
                crashWindow.exceptionType = exceptionType
                crashWindow.ShowDialog()

                Process.GetCurrentProcess.Kill()
            End If
        End Sub

        'Public Sub manuallyLoadCrashWindow(ex As Exception, message As String, stackTrace As String, crashType As System.Type)
        '    Dim handleCrashWithAnErrorInsteadResult As Boolean = handleCrashWithAnErrorInstead(crashType.ToString, ex)

        '    If handleCrashWithAnErrorInsteadResult = True Then
        '        Dim crashWindow As New frmCrash
        '        crashWindow.Text = "Critical Application Error Detected!"
        '        crashWindow.lblHeader.Text = "Critical Application Error Detected!"
        '        crashWindow.crashStackTrace = stackTrace
        '        crashWindow.crashMessage = message
        '        crashWindow.crashType = crashType.ToString
        '        crashWindow.ShowDialog()

        '        Process.GetCurrentProcess.Kill()
        '    End If
        'End Sub
    End Module
End Namespace

Friend Class ThreadExceptionHandler
    '''
    ''' Handles the thread exception.
    '''
    Public Sub Application_ThreadException(ByVal sender As Object, ByVal exceptionObject As Threading.ThreadExceptionEventArgs)
        Try
            Dim handleCrashWithAnErrorInsteadResult As Boolean = exceptionHandler.handleCrashWithAnErrorInstead(exceptionObject.Exception)

            If handleCrashWithAnErrorInsteadResult = True Then
                Dim crashWindow As New frmCrash
                Threading.Thread.CurrentThread.CurrentUICulture = New Globalization.CultureInfo("en-US")
                crashWindow.exceptionMessage = exceptionObject.Exception.Message
                crashWindow.exceptionStackTrace = exceptionObject.Exception.StackTrace
                crashWindow.exceptionType = exceptionObject.Exception.GetType.ToString
                crashWindow.ShowDialog()

                Dim currentProcess As Process = Process.GetCurrentProcess()
                currentProcess.Kill()

                '' Exit the program if the user clicks Abort.
                'Dim result As DialogResult = ShowThreadExceptionDialog(e.Exception)

                'If (result = DialogResult.Abort) Then
                '    Application.Exit()
                'End If
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

    'Public Sub Application_UnhandledException(ByVal sender As System.Object, ByVal e As UnhandledExceptionEventArgs)
    '    Try
    '        Dim crashWindow As New frmCrash
    '        crashWindow.crashData = e.ExceptionObject
    '        crashWindow.ShowDialog()
    '        Dim currentProcess As Process = Process.GetCurrentProcess()
    '        currentProcess.Kill()

    '        '' Exit the program if the user clicks Abort.
    '        'Dim result As DialogResult = ShowThreadExceptionDialog(e.Exception)

    '        'If (result = DialogResult.Abort) Then
    '        '    Application.Exit()
    '        'End If
    '    Catch
    '        ' Fatal error, terminate program
    '        Try
    '            MessageBox.Show("Fatal Error", "Fatal Error", MessageBoxButtons.OK, MessageBoxIcon.Stop)
    '        Finally
    '            Dim currentProcess As Process = Process.GetCurrentProcess()
    '            currentProcess.Kill()
    '        End Try
    '    End Try
    'End Sub
End Class