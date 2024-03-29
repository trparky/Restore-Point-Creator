﻿Imports System.ComponentModel

Public Class Official_Contact_Form
    Protected Const apiAccessCode As String = "YWiIMIyGVVFEunRpDF5PNIF2yzcADdBxneRmWDlLpMTCoVFEunRWiIMIyRmWnRpDF"
    Private strFileToHaveDataExportedTo As String = IO.Path.Combine(IO.Path.GetTempPath(), "event log entries.reslogx")
    Private Const intMaxSize As Integer = 6291456

    Private Sub btnClose_Click(sender As Object, e As EventArgs) Handles btnClose.Click
        Me.Close()
    End Sub

    Private Sub Official_Contact_Form_FormClosing(sender As Object, e As FormClosingEventArgs) Handles Me.FormClosing
        If IO.File.Exists(strFileToHaveDataExportedTo) = True Then
            Functions.support.deleteFileWithNoException(strFileToHaveDataExportedTo)
        End If

        If globalVariables.windows.officialContactForm IsNot Nothing Then
            globalVariables.windows.officialContactForm.Dispose()
            globalVariables.windows.officialContactForm = Nothing
        End If
    End Sub

    Sub dataSubmitThread()
        Dim zipFilePath As String = IO.Path.Combine(IO.Path.GetTempPath(), "attachments.zip")
        If IO.File.Exists(zipFilePath) Then IO.File.Delete(zipFilePath)

        Dim boolDoWeHaveAttachments As Boolean = False
        If listAttachedFiles.Items.Count <> 0 Then boolDoWeHaveAttachments = True

        If boolDoWeHaveAttachments = True Then
            Try
                Using zipFileObject As IO.Compression.ZipArchive = IO.Compression.ZipFile.Open(zipFilePath, IO.Compression.ZipArchiveMode.Create)
                    If zipFileObject Is Nothing Then
                        MsgBox("There was an error while creating the ZIP file to contain your attached files, submission process aborted.", MsgBoxStyle.Critical + MsgBoxStyle.ApplicationModal, Me.Text)
                        Exit Sub
                    Else
                        For Each item As myListViewItemTypes.contactFormFileListItem In listAttachedFiles.Items
                            If Not Functions.support.addFileToZipFile(zipFileObject, item.strFileName) Then
                                MsgBox("There was an error while writing the attached files to the ZIP file, submission process aborted.", MsgBoxStyle.Critical + MsgBoxStyle.ApplicationModal, Me.Text)
                                Exit Sub
                            End If
                        Next
                    End If
                End Using
            Catch ex As Exception
                enableFormElements()
                Functions.eventLogFunctions.writeCrashToApplicationLogFile(ex)
                closePleaseWaitPanel()
                MsgBox("There was an error while preparing your file attachments for submission. Please see the Event Log for more details.", MsgBoxStyle.Critical + MsgBoxStyle.ApplicationModal, Me.Text)
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
            Catch ex As IO.FileNotFoundException
                closePleaseWaitPanel()
                MsgBox("The file attachment you have chosen doesn't exist.", MsgBoxStyle.Critical + MsgBoxStyle.ApplicationModal, Me.Text)
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
                If boolDoWeHaveAttachments = True Then closePleaseWaitPanel()

                If strHTTPResponse.Equals("ok", StringComparison.OrdinalIgnoreCase) Then
                    listAttachedFiles.Items.Clear()
                    If IO.File.Exists(zipFilePath) Then IO.File.Delete(zipFilePath)

                    MsgBox("Your email to the developer has been sent. This window will now close.", MsgBoxStyle.Information + MsgBoxStyle.ApplicationModal, Me.Text)
                    Me.Close()
                ElseIf strHTTPResponse.Equals("error-invalid-email", StringComparison.OrdinalIgnoreCase) Then
                    MsgBox("Invalid email address. Please try again.", MsgBoxStyle.Critical + MsgBoxStyle.ApplicationModal, Me.Text)
                ElseIf strHTTPResponse.Equals("email-server-said-user-doesnt-exist", StringComparison.OrdinalIgnoreCase) Then
                    MsgBox("The remote email server said that the email address doesn't exist. Please try again.", MsgBoxStyle.Critical + MsgBoxStyle.ApplicationModal, Me.Text)
                ElseIf strHTTPResponse.Equals("dns-error", StringComparison.OrdinalIgnoreCase) Then
                    MsgBox("The domain name doesn't exist. Please try again.", MsgBoxStyle.Critical + MsgBoxStyle.ApplicationModal, Me.Text)
                ElseIf strHTTPResponse.Equals("server-connect-error", StringComparison.OrdinalIgnoreCase) Then
                    MsgBox("Unable to contact mail server, more than likely your email address is invalid. Please try again.", MsgBoxStyle.Critical + MsgBoxStyle.ApplicationModal, Me.Text)
                ElseIf strHTTPResponse.Equals("invalid-email-syntax", StringComparison.OrdinalIgnoreCase) Then
                    MsgBox("The email address didn't pass syntax validation. Please try again.", MsgBoxStyle.Critical + MsgBoxStyle.ApplicationModal, Me.Text)
                ElseIf strHTTPResponse.Equals("no-email-servers-contactable", StringComparison.OrdinalIgnoreCase) Then
                    MsgBox("No mail servers found, more than likely your email address is invalid. Please try again.", MsgBoxStyle.Critical + MsgBoxStyle.ApplicationModal, Me.Text)
                ElseIf strHTTPResponse.Equals("no-access-allowed", StringComparison.OrdinalIgnoreCase) Then
                    MsgBox("Error accessing server side script.", MsgBoxStyle.Critical + MsgBoxStyle.ApplicationModal, Me.Text)
                ElseIf strHTTPResponse.Equals("error-no-message-found", StringComparison.OrdinalIgnoreCase) Then
                    MsgBox("No message found. Please try again.", MsgBoxStyle.Critical + MsgBoxStyle.ApplicationModal, Me.Text)
                ElseIf strHTTPResponse.Equals("file_attachment_failure", StringComparison.OrdinalIgnoreCase) Then
                    MsgBox("File attachment failure. Please try again.", MsgBoxStyle.Critical + MsgBoxStyle.ApplicationModal, Me.Text)
                End If

                btnSubmit.Enabled = True
            Else
                closePleaseWaitPanel()
                MsgBox("Error accessing server side script.", MsgBoxStyle.Critical + MsgBoxStyle.ApplicationModal, Me.Text)
            End If
        Catch ex As Exception
        Finally
            closePleaseWaitPanel()
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
                My.Settings.boolAutoCrashSubmissionEnabled = False
                chkSaveInfo.Checked = False
            Else
                chkSaveInfo.Checked = True
            End If
        End If

        My.Settings.boolSaveInfo = chkSaveInfo.Checked
    End Sub

    Private Sub Official_Contact_Form_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        maxSize.ProgressBarColor = My.Settings.barColor

        If My.Settings.useSSL = True Then
            btnSubmit.Image = My.Resources.lock
            ToolTip.SetToolTip(btnSubmit, "Secured by SSL.")
        End If

        If My.Settings.boolSaveInfo Then
            chkSaveInfo.Checked = True
            txtName.Text = My.Settings.usersName
            txtEmail.Text = My.Settings.usersEmail
        End If
    End Sub

    Private Sub addFileToList(strInputFileName As String)
        Dim listViewItem As New myListViewItemTypes.contactFormFileListItem With {
            .Text = strInputFileName,
            .strFileName = strInputFileName,
            .longFileSize = New IO.FileInfo(strInputFileName).Length
        }
        listViewItem.SubItems.Add(Functions.support.bytesToHumanSize(listViewItem.longFileSize))
        listAttachedFiles.Items.Add(listViewItem)
    End Sub

    Private Sub btnBrowseForAttachment_Click(sender As Object, e As EventArgs) Handles btnBrowse.Click
        OpenFileDialog1.FileName = Nothing
        OpenFileDialog1.Title = "Browse for Attachment"
        OpenFileDialog1.Filter = "All Accepted File Types|*.png;*.jpg;*.jpeg;*.txt;*.log;*.reslog|Image Files (JPEG, PNG)|*.png;*.jpg;*.jpeg|Text Files|*.txt;*.log;*.reslog"

        If OpenFileDialog1.ShowDialog() = DialogResult.OK Then
            Dim fileInfo As New IO.FileInfo(OpenFileDialog1.FileName)

            If fileInfo.Extension.Equals(".png", StringComparison.OrdinalIgnoreCase) Or fileInfo.Extension.Equals(".jpg", StringComparison.OrdinalIgnoreCase) Or fileInfo.Extension.Equals(".jpeg", StringComparison.OrdinalIgnoreCase) Or fileInfo.Extension.Equals(".txt", StringComparison.OrdinalIgnoreCase) Or fileInfo.Extension.Equals(".log", StringComparison.OrdinalIgnoreCase) Or fileInfo.Extension.Equals(".reslog", StringComparison.OrdinalIgnoreCase) Then
                If doesFileExistInList(OpenFileDialog1.FileName.ToString) = True Then
                    MsgBox("A file by the name of " & Chr(34) & New IO.FileInfo(OpenFileDialog1.FileName.ToString).Name & Chr(34) & " already exists in the list of attached files.", MsgBoxStyle.Information, Me.Text)
                Else
                    addFileToList(OpenFileDialog1.FileName.ToString)
                    calculateTotalSize()
                End If
            Else
                MsgBox("Invalid file attachment.", MsgBoxStyle.Information, Me.Text)
            End If
        End If
    End Sub

    Private Sub btnDeleteAttachment_Click(sender As Object, e As EventArgs) Handles btnDeleteAttachment.Click
        If listAttachedFiles.SelectedItems.Count > 0 Then
            For Each item As myListViewItemTypes.contactFormFileListItem In listAttachedFiles.SelectedItems
                If item.strFileName.EndsWith(".reslog", StringComparison.OrdinalIgnoreCase) Or item.strFileName.EndsWith(".reslogx", StringComparison.OrdinalIgnoreCase) Then
                    Functions.support.deleteFileWithNoException(item.strFileName)
                End If
                item.Remove()
            Next
        End If

        calculateTotalSize()
    End Sub

    Function doesFileExistInList(strFileToCheckForExistanceOf As String) As Boolean
        If listAttachedFiles.Items.Count = 0 Then
            Return False
        Else
            strFileToCheckForExistanceOf = New IO.FileInfo(strFileToCheckForExistanceOf).Name

            For Each item As myListViewItemTypes.contactFormFileListItem In listAttachedFiles.Items
                If New IO.FileInfo(item.strFileName).Name.Equals(strFileToCheckForExistanceOf, StringComparison.OrdinalIgnoreCase) Then Return True
            Next

            Return False
        End If
    End Function

    Private Sub btnClear_Click(sender As Object, e As EventArgs) Handles btnClear.Click
        listAttachedFiles.Items.Clear()
        maxSize.Value = 0
    End Sub

    Private Sub btnAttachEventLogs_Click(sender As Object, e As EventArgs) Handles btnAttachEventLogs.Click
        Try
            Dim logCount As ULong = 0
            Dim timeStamp As Stopwatch = Stopwatch.StartNew()

            If Functions.eventLogFunctions.exportLogsToFile(strFileToHaveDataExportedTo, logCount) Then
                timeStamp.Stop()

                btnAttachEventLogs.Enabled = False
                addFileToList(strFileToHaveDataExportedTo)
                calculateTotalSize()

                MsgBox(String.Format("{0} log entries have been successfully exported and added to the list of attached files.{1}{1}Application Event Log exported in {2}ms ({3} seconds).", logCount, vbCrLf, timeStamp.ElapsedMilliseconds, Math.Round(timeStamp.Elapsed.TotalSeconds, 3)), MsgBoxStyle.Information, Me.Text)
            Else
                MsgBox("There was an error while attempting to export the program's event log entries.", MsgBoxStyle.Critical + MsgBoxStyle.ApplicationModal, Me.Text)
            End If
        Catch ex As Exception
            Functions.eventLogFunctions.writeCrashToApplicationLogFile(ex)
            Functions.eventLogFunctions.writeToApplicationLogFile("There was an error while attempting to export the program's event log entries.", EventLogEntryType.Error, False)

            MsgBox("There was an error while exporting the log data. Please see the Event Log for more details.", MsgBoxStyle.Critical + MsgBoxStyle.ApplicationModal, Me.Text)
        End Try
    End Sub

    Private Sub Official_Contact_Form_Closing(sender As Object, e As CancelEventArgs) Handles Me.Closing
        If IO.File.Exists(strFileToHaveDataExportedTo) = True Then
            Try
                IO.File.Delete(strFileToHaveDataExportedTo)
            Catch ex As Exception
            End Try
        End If
    End Sub

    Sub calculateTotalSize()
        btnSubmit.Enabled = True
        Dim size As Integer

        For Each item As myListViewItemTypes.contactFormFileListItem In listAttachedFiles.Items
            size += item.longFileSize
        Next

        lblTotalFileSize.Text = String.Format("You have attached {0} of the maximum {1} allowed.", Functions.support.bytesToHumanSize(size), Functions.support.bytesToHumanSize(intMaxSize))
        maxSize.Value = Math.Round((size / intMaxSize) * 100, 0)

        If size > intMaxSize Then
            btnSubmit.Enabled = False
            MsgBox("You have exceeded the maximum amount of data that is allowed by this form, please reduce the amount of files you have attached." & vbCrLf & vbCrLf & "You are limited to 2 MBs of data.", MsgBoxStyle.Information, Me.Text)
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