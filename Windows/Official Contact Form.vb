Imports System.ComponentModel
Imports ICSharpCode.SharpZipLib.Zip
Imports System.Text
Imports System.IO

Public Class Official_Contact_Form
    Protected Const apiAccessCode As String = "YWiIMIyGVVFEunRpDF5PNIF2yzcADdBxneRmWDlLpMTCoVFEunRWiIMIyRmWnRpDF"
    Private strFileToHaveDataExportedTo As String = Path.Combine(Path.GetTempPath(), "event log entries.reslog")

    Private Sub btnClose_Click(sender As Object, e As EventArgs) Handles btnClose.Click
        Me.Close()
    End Sub

    Private Sub Official_Contact_Form_FormClosing(sender As Object, e As FormClosingEventArgs) Handles Me.FormClosing
        If File.Exists(strFileToHaveDataExportedTo) = True Then
            Functions.support.deleteFileWithNoException(strFileToHaveDataExportedTo)
        End If

        If globalVariables.windows.officialContactForm IsNot Nothing Then
            globalVariables.windows.officialContactForm.Dispose()
            globalVariables.windows.officialContactForm = Nothing
        End If
    End Sub

    Sub dataSubmitThread()
        Dim zipFilePath As String = Path.Combine(Path.GetTempPath(), "attachments.zip")
        If File.Exists(zipFilePath) Then File.Delete(zipFilePath)

        Dim boolDoWeHaveAttachments As Boolean = False
        If listAttachedFiles.Items.Count <> 0 Then boolDoWeHaveAttachments = True

        If boolDoWeHaveAttachments = True Then
            Try
                Dim zipFileObject As ZipFile = ZipFile.Create(zipFilePath) ' Creates a new ZIP file.
                zipFileObject.BeginUpdate() ' We need to open the ZIP file for writing.

                Dim fileToAdd As String

                For i = 0 To listAttachedFiles.Items.Count - 1
                    fileToAdd = listAttachedFiles.Items.Item(i).ToString
                    zipFileObject.Add(fileToAdd, New FileInfo(fileToAdd).Name) ' Adds the file to the ZIP file.
                Next

                zipFileObject.CommitUpdate() ' Commits the added file(s) to the ZIP file.
                zipFileObject.Close() ' Closes the ZIPFile Object.
            Catch ex As Exception
                enableFormElements()
                Functions.eventLogFunctions.writeCrashToEventLog(ex)
                MsgBox("There was an error while preparing your file attachments for submission. Please see the Event Log for more details.", MsgBoxStyle.Critical, Me.Text)
                Exit Sub
            End Try
        End If

        Dim httpHelper As httpHelper = Functions.http.createNewHTTPHelperObject()

        httpHelper.addPOSTData("email", txtEmail.Text)
        httpHelper.addPOSTData("name", txtName.Text)
        httpHelper.addPOSTData("message", txtMessage.Text)
        httpHelper.addPOSTData("apiaccesscode", apiAccessCode)
        httpHelper.addPOSTData("submissionversion", "2")

        If boolDoWeHaveAttachments = True Then
            Try
                httpHelper.addFileUpload("attachment", zipFilePath, Nothing, "application/zip")
            Catch ex As FileNotFoundException
                MsgBox("The file attachment you have chosen doesn't exist.", MsgBoxStyle.Critical, Me.Text)
                enableFormElements()
                Exit Sub
            End Try
        End If

        Try
            Dim strHTTPResponse As String = Nothing
            Dim boolHTTPResponseResult As Boolean

            If boolDoWeHaveAttachments = True Then
                ' If we have a file to upload we need to use the most complicated uploadData() function.
                boolHTTPResponseResult = httpHelper.uploadData(globalVariables.webURLs.dataProcessors.strContactForm, strHTTPResponse)
            Else
                ' If we don't have a file to upload we can use the much simpler getWebData() function instead.
                boolHTTPResponseResult = httpHelper.getWebData(globalVariables.webURLs.dataProcessors.strContactForm, strHTTPResponse)
            End If

            If boolHTTPResponseResult = True Then
                If boolDoWeHaveAttachments = True Then Functions.wait.closePleaseWaitWindow()

                Debug.WriteLine("strHTTPResponse = " & strHTTPResponse)

                Select Case strHTTPResponse
                    Case "ok"
                        listAttachedFiles.Items.Clear()
                        If File.Exists(zipFilePath) Then File.Delete(zipFilePath)

                        MsgBox("Your email to the developer has been sent. This window will now close.", MsgBoxStyle.Information, Me.Text)
                        Me.Close()
                    Case "error-invalid-email"
                        MsgBox("Invalid email address. Please try again.", MsgBoxStyle.Critical, Me.Text)
                    Case "email-server-said-user-doesnt-exist"
                        MsgBox("The remote email server said that the email address doesn't exist. Please try again.", MsgBoxStyle.Critical, Me.Text)
                    Case "dns-error"
                        MsgBox("The domain name doesn't exist. Please try again.", MsgBoxStyle.Critical, Me.Text)
                    Case "server-connect-error"
                        MsgBox("Unable to contact mail server, more than likely your email address is invalid. Please try again.", MsgBoxStyle.Critical, Me.Text)
                    Case "invalid-email-syntax"
                        MsgBox("The email address didn't pass syntax validation. Please try again.", MsgBoxStyle.Critical, Me.Text)
                    Case "no-email-servers-contactable"
                        MsgBox("No mail servers found, more than likely your email address is invalid. Please try again.", MsgBoxStyle.Critical, Me.Text)
                    Case "no-access-allowed"
                        MsgBox("Error accessing server side script.", MsgBoxStyle.Critical, Me.Text)
                    Case "error-no-message-found"
                        MsgBox("No message found. Please try again.", MsgBoxStyle.Critical, Me.Text)
                    Case "file_attachment_failure"
                        MsgBox("File attachment failure. Please try again.", MsgBoxStyle.Critical, Me.Text)
                End Select

                btnSubmit.Enabled = True
            Else
                MsgBox("Error accessing server side script.", MsgBoxStyle.Critical, Me.Text)
            End If
        Catch ex As Exception
            Functions.eventLogFunctions.writeCrashToEventLog(ex)

            If TypeOf ex Is Net.WebException Or TypeOf ex Is httpProtocolException Then
                Functions.eventLogFunctions.writeToSystemEventLog("The server responded with an HTTP error.", EventLogEntryType.Error)
                btnSubmit.Enabled = True
                MsgBox("The server responded with an HTTP error.", MsgBoxStyle.Critical, Me.Text)
            ElseIf TypeOf ex Is sslErrorException Then
                Functions.eventLogFunctions.writeToSystemEventLog("An HTTP SSL error occurred.", EventLogEntryType.Error)
                MsgBox("An HTTP SSL error occurred.", MsgBoxStyle.Critical, Me.Text)
            ElseIf TypeOf ex Is Threading.ThreadAbortException Then
                ' We don't do anything here.
            Else
                MsgBox("A general error occured, please check the Event Log.", MsgBoxStyle.Critical, Me.Text)
            End If
        Finally
            enableFormElements()
        End Try
    End Sub

    Sub disableFormElements()
        listAttachedFiles.Enabled = False
        btnBrowse.Enabled = False
        btnDeleteAttachment.Enabled = False
        btnClear.Enabled = False
        btnSubmit.Enabled = False
    End Sub

    Sub enableFormElements()
        listAttachedFiles.Enabled = True
        btnBrowse.Enabled = True
        btnDeleteAttachment.Enabled = True
        btnClear.Enabled = True
    End Sub

    Private Sub btnSubmit_Click(sender As Object, e As EventArgs) Handles btnSubmit.Click
        disableFormElements()

        If listAttachedFiles.Items.Count <> 0 Then
            Functions.wait.createPleaseWaitWindow("Compressing and Sending Data... Please Wait.")
        End If

        Dim sendingThread As New Threading.Thread(AddressOf dataSubmitThread)
        sendingThread.Name = "Contact Form Data Sending Thread"
        sendingThread.Start()

        If listAttachedFiles.Items.Count <> 0 Then
            Functions.wait.openPleaseWaitWindow(Me)
        End If
    End Sub

    Private Sub Official_Contact_Form_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        If My.Settings.useSSL = True Then
            btnSubmit.Image = My.Resources.lock
            ToolTip.SetToolTip(btnSubmit, "Secured by SSL.")
        End If
    End Sub

    Private Sub btnBrowseForAttachment_Click(sender As Object, e As EventArgs) Handles btnBrowse.Click
        OpenFileDialog1.FileName = Nothing
        OpenFileDialog1.Title = "Browse for Attachment"
        OpenFileDialog1.Filter = "All Accepted File Types|*.png;*.jpg;*.jpeg;*.txt;*.log;*.reslog|Image Files (JPEG, PNG)|*.png;*.jpg;*.jpeg|Text Files|*.txt;*.log;*.reslog"

        If OpenFileDialog1.ShowDialog() = DialogResult.OK Then
            Dim fileInfo As New FileInfo(OpenFileDialog1.FileName)

            If fileInfo.Extension.ToLower = ".png" Or fileInfo.Extension.ToLower = ".jpg" Or fileInfo.Extension.ToLower = ".jpeg" Or fileInfo.Extension.ToLower = ".txt" Or fileInfo.Extension.ToLower = ".log" Or fileInfo.Extension.ToLower = ".reslog" Then
                If doesFileExistInList(OpenFileDialog1.FileName.ToString) = True Then
                    MsgBox("A file by the name of " & Chr(34) & New FileInfo(OpenFileDialog1.FileName.ToString).Name & Chr(34) & " already exists in the list of attached files.", MsgBoxStyle.Information, Me.Text)
                Else
                    listAttachedFiles.Items.Add(OpenFileDialog1.FileName.ToString)
                End If
            Else
                MsgBox("Invalid file attachment.", MsgBoxStyle.Information, Me.Text)
            End If
        End If
    End Sub

    Private Sub btnDeleteAttachment_Click(sender As Object, e As EventArgs) Handles btnDeleteAttachment.Click
        Dim strFileToBeRemoved As String = listAttachedFiles.Text
        If listAttachedFiles.SelectedItems.Count <> 0 Then listAttachedFiles.Items.Remove(listAttachedFiles.SelectedItems(0))

        If strFileToBeRemoved.ToLower.EndsWith(".reslog") = True Then
            Try
                File.Delete(strFileToBeRemoved)
            Catch ex As Exception
            End Try
        End If
    End Sub

    Function doesFileExistInList(strFileToCheckForExistanceOf As String) As Boolean
        If listAttachedFiles.Items.Count = 0 Then
            Return False
        Else
            strFileToCheckForExistanceOf = New FileInfo(strFileToCheckForExistanceOf).Name.ToLower.Trim

            For Each item As String In listAttachedFiles.Items
                item = New FileInfo(item).Name.ToLower.Trim
                If item = strFileToCheckForExistanceOf Then Return True
            Next

            Return False
        End If
    End Function

    Private Sub btnClear_Click(sender As Object, e As EventArgs) Handles btnClear.Click
        listAttachedFiles.Items.Clear()
    End Sub

    Private Sub btnAttachEventLogs_Click(sender As Object, e As EventArgs) Handles btnAttachEventLogs.Click
        Try
            Dim jsonEngine As New Web.Script.Serialization.JavaScriptSerializer

            Dim logCount As ULong = 0
            Dim timeStamp As New Stopwatch
            timeStamp.Start()

            Functions.eventLogFunctions.exportLogsToFile(strFileToHaveDataExportedTo, logCount)

            timeStamp.Stop()

            btnAttachEventLogs.Enabled = False
            listAttachedFiles.Items.Add(strFileToHaveDataExportedTo)

            MsgBox(String.Format("{0} log entries have been successfully exported and added to the list of attached files.{1}{1}Application Event Log exported in {2}ms ({3} seconds).", logCount, vbCrLf, timeStamp.ElapsedMilliseconds, Math.Round(timeStamp.Elapsed.TotalSeconds, 3)), MsgBoxStyle.Information, Me.Text)
        Catch ex As Exception
            Functions.eventLogFunctions.writeCrashToEventLog(ex)
            Functions.eventLogFunctions.writeToSystemEventLog("There was an error while attempting to export the program's event log entries.", EventLogEntryType.Error)

            MsgBox("There was an error while exporting the log data. Please see the Event Log for more details.", MsgBoxStyle.Critical, Me.Text)
        End Try
    End Sub

    Private Sub Official_Contact_Form_Closing(sender As Object, e As CancelEventArgs) Handles Me.Closing
        If File.Exists(strFileToHaveDataExportedTo) = True Then
            Try
                File.Delete(strFileToHaveDataExportedTo)
            Catch ex As Exception
            End Try
        End If
    End Sub
End Class