<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()>
Partial Class Official_Contact_Form
    Inherits System.Windows.Forms.Form

    'Form overrides dispose to clean up the component list.
    <System.Diagnostics.DebuggerNonUserCode()>
    Protected Overrides Sub Dispose(ByVal disposing As Boolean)
        Try
            If disposing AndAlso components IsNot Nothing Then
                components.Dispose()
            End If
        Finally
            MyBase.Dispose(disposing)
        End Try
    End Sub

    'Required by the Windows Form Designer
    Private components As System.ComponentModel.IContainer

    'NOTE: The following procedure is required by the Windows Form Designer
    'It can be modified using the Windows Form Designer.  
    'Do not modify it using the code editor.
    <System.Diagnostics.DebuggerStepThrough()>
    Private Sub InitializeComponent()
        Me.components = New System.ComponentModel.Container()
        Me.Label1 = New System.Windows.Forms.Label()
        Me.Label2 = New System.Windows.Forms.Label()
        Me.txtName = New System.Windows.Forms.TextBox()
        Me.txtEmail = New System.Windows.Forms.TextBox()
        Me.Label3 = New System.Windows.Forms.Label()
        Me.txtMessage = New System.Windows.Forms.TextBox()
        Me.TableLayoutPanel1 = New System.Windows.Forms.TableLayoutPanel()
        Me.btnClose = New System.Windows.Forms.Button()
        Me.btnSubmit = New System.Windows.Forms.Button()
        Me.ToolTip = New System.Windows.Forms.ToolTip(Me.components)
        Me.btnBrowse = New System.Windows.Forms.Button()
        Me.OpenFileDialog1 = New System.Windows.Forms.OpenFileDialog()
        Me.Label4 = New System.Windows.Forms.Label()
        Me.btnClear = New System.Windows.Forms.Button()
        Me.btnDeleteAttachment = New System.Windows.Forms.Button()
        Me.listAttachedFiles = New System.Windows.Forms.ListBox()
        Me.btnAttachEventLogs = New System.Windows.Forms.Button()
        Me.TableLayoutPanel1.SuspendLayout()
        Me.SuspendLayout()
        '
        'Label1
        '
        Me.Label1.AutoSize = True
        Me.Label1.Location = New System.Drawing.Point(12, 9)
        Me.Label1.Name = "Label1"
        Me.Label1.Size = New System.Drawing.Size(60, 13)
        Me.Label1.TabIndex = 0
        Me.Label1.Text = "Your Name"
        '
        'Label2
        '
        Me.Label2.AutoSize = True
        Me.Label2.Location = New System.Drawing.Point(12, 35)
        Me.Label2.Name = "Label2"
        Me.Label2.Size = New System.Drawing.Size(98, 13)
        Me.Label2.TabIndex = 1
        Me.Label2.Text = "Your Email Address"
        '
        'txtName
        '
        Me.txtName.Location = New System.Drawing.Point(78, 6)
        Me.txtName.Name = "txtName"
        Me.txtName.Size = New System.Drawing.Size(423, 20)
        Me.txtName.TabIndex = 1
        '
        'txtEmail
        '
        Me.txtEmail.Location = New System.Drawing.Point(116, 32)
        Me.txtEmail.Name = "txtEmail"
        Me.txtEmail.Size = New System.Drawing.Size(385, 20)
        Me.txtEmail.TabIndex = 2
        '
        'Label3
        '
        Me.Label3.AutoSize = True
        Me.Label3.Location = New System.Drawing.Point(12, 55)
        Me.Label3.Name = "Label3"
        Me.Label3.Size = New System.Drawing.Size(414, 52)
        Me.Label3.TabIndex = 4
        Me.Label3.Text = "Your email address will be kept strictly confidential and will NOT be sold to thi" &
    "rd-parties." & Global.Microsoft.VisualBasic.ChrW(13) & Global.Microsoft.VisualBasic.ChrW(10) & "Believe me, I hate spam just as much as you do." & Global.Microsoft.VisualBasic.ChrW(13) & Global.Microsoft.VisualBasic.ChrW(10) & Global.Microsoft.VisualBasic.ChrW(13) & Global.Microsoft.VisualBasic.ChrW(10) & "Your Message"
        '
        'txtMessage
        '
        Me.txtMessage.Location = New System.Drawing.Point(10, 110)
        Me.txtMessage.Multiline = True
        Me.txtMessage.Name = "txtMessage"
        Me.txtMessage.Size = New System.Drawing.Size(491, 200)
        Me.txtMessage.TabIndex = 3
        '
        'TableLayoutPanel1
        '
        Me.TableLayoutPanel1.ColumnCount = 2
        Me.TableLayoutPanel1.ColumnStyles.Add(New System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50.0!))
        Me.TableLayoutPanel1.ColumnStyles.Add(New System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50.0!))
        Me.TableLayoutPanel1.Controls.Add(Me.btnClose, 0, 0)
        Me.TableLayoutPanel1.Controls.Add(Me.btnSubmit, 0, 0)
        Me.TableLayoutPanel1.Location = New System.Drawing.Point(10, 525)
        Me.TableLayoutPanel1.Name = "TableLayoutPanel1"
        Me.TableLayoutPanel1.RowCount = 1
        Me.TableLayoutPanel1.RowStyles.Add(New System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 50.0!))
        Me.TableLayoutPanel1.Size = New System.Drawing.Size(491, 29)
        Me.TableLayoutPanel1.TabIndex = 7
        '
        'btnClose
        '
        Me.btnClose.Anchor = CType((((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Bottom) _
            Or System.Windows.Forms.AnchorStyles.Left) _
            Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.btnClose.Location = New System.Drawing.Point(248, 3)
        Me.btnClose.Name = "btnClose"
        Me.btnClose.Size = New System.Drawing.Size(240, 23)
        Me.btnClose.TabIndex = 9
        Me.btnClose.Text = "Close Window"
        Me.btnClose.UseVisualStyleBackColor = True
        '
        'btnSubmit
        '
        Me.btnSubmit.Anchor = CType((((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Bottom) _
            Or System.Windows.Forms.AnchorStyles.Left) _
            Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.btnSubmit.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft
        Me.btnSubmit.Location = New System.Drawing.Point(3, 3)
        Me.btnSubmit.Name = "btnSubmit"
        Me.btnSubmit.Size = New System.Drawing.Size(239, 23)
        Me.btnSubmit.TabIndex = 8
        Me.btnSubmit.Text = "Submit Message"
        Me.btnSubmit.UseVisualStyleBackColor = True
        '
        'btnBrowse
        '
        Me.btnBrowse.Image = Global.Restore_Point_Creator.My.Resources.Resources.folder_explore
        Me.btnBrowse.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft
        Me.btnBrowse.Location = New System.Drawing.Point(394, 346)
        Me.btnBrowse.Name = "btnBrowse"
        Me.btnBrowse.Size = New System.Drawing.Size(107, 35)
        Me.btnBrowse.TabIndex = 4
        Me.btnBrowse.Text = "Browse to add attachment"
        Me.btnBrowse.TextAlign = System.Drawing.ContentAlignment.MiddleRight
        Me.ToolTip.SetToolTip(Me.btnBrowse, "Browse to add attachment")
        Me.btnBrowse.UseVisualStyleBackColor = True
        '
        'OpenFileDialog1
        '
        Me.OpenFileDialog1.FileName = "OpenFileDialog1"
        '
        'Label4
        '
        Me.Label4.AutoSize = True
        Me.Label4.Location = New System.Drawing.Point(15, 317)
        Me.Label4.Name = "Label4"
        Me.Label4.Size = New System.Drawing.Size(486, 26)
        Me.Label4.TabIndex = 12
        Me.Label4.Text = "Attached Files (This is optional. Perfect for if you need to send screenshots to " &
    "me.)" & Global.Microsoft.VisualBasic.ChrW(13) & Global.Microsoft.VisualBasic.ChrW(10) & "Please don't attach any executable files (EXE, BAT, DLL, etc.), they will " &
    "be blocked by my mail server."
        '
        'btnClear
        '
        Me.btnClear.Image = Global.Restore_Point_Creator.My.Resources.Resources.edit_clear
        Me.btnClear.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft
        Me.btnClear.Location = New System.Drawing.Point(394, 477)
        Me.btnClear.Name = "btnClear"
        Me.btnClear.Size = New System.Drawing.Size(107, 40)
        Me.btnClear.TabIndex = 6
        Me.btnClear.Text = "Clear" & Global.Microsoft.VisualBasic.ChrW(13) & Global.Microsoft.VisualBasic.ChrW(10) & "Attachments"
        Me.btnClear.TextAlign = System.Drawing.ContentAlignment.MiddleRight
        Me.btnClear.UseVisualStyleBackColor = True
        '
        'btnDeleteAttachment
        '
        Me.btnDeleteAttachment.Image = Global.Restore_Point_Creator.My.Resources.Resources.delete
        Me.btnDeleteAttachment.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft
        Me.btnDeleteAttachment.Location = New System.Drawing.Point(394, 387)
        Me.btnDeleteAttachment.Name = "btnDeleteAttachment"
        Me.btnDeleteAttachment.Size = New System.Drawing.Size(107, 40)
        Me.btnDeleteAttachment.TabIndex = 5
        Me.btnDeleteAttachment.Text = "Delete" & Global.Microsoft.VisualBasic.ChrW(13) & Global.Microsoft.VisualBasic.ChrW(10) & "Attachment"
        Me.btnDeleteAttachment.TextAlign = System.Drawing.ContentAlignment.MiddleRight
        Me.btnDeleteAttachment.UseVisualStyleBackColor = True
        '
        'listAttachedFiles
        '
        Me.listAttachedFiles.FormattingEnabled = True
        Me.listAttachedFiles.Location = New System.Drawing.Point(10, 346)
        Me.listAttachedFiles.Name = "listAttachedFiles"
        Me.listAttachedFiles.Size = New System.Drawing.Size(378, 173)
        Me.listAttachedFiles.TabIndex = 13
        '
        'btnAttachEventLogs
        '
        Me.btnAttachEventLogs.Image = Global.Restore_Point_Creator.My.Resources.Resources.textBlock
        Me.btnAttachEventLogs.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft
        Me.btnAttachEventLogs.Location = New System.Drawing.Point(394, 433)
        Me.btnAttachEventLogs.Name = "btnAttachEventLogs"
        Me.btnAttachEventLogs.Size = New System.Drawing.Size(107, 38)
        Me.btnAttachEventLogs.TabIndex = 14
        Me.btnAttachEventLogs.Text = "Attach Program Event Logs"
        Me.btnAttachEventLogs.TextAlign = System.Drawing.ContentAlignment.MiddleRight
        Me.btnAttachEventLogs.UseVisualStyleBackColor = True
        '
        'Official_Contact_Form
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(6.0!, 13.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi
        Me.ClientSize = New System.Drawing.Size(509, 561)
        Me.Controls.Add(Me.btnAttachEventLogs)
        Me.Controls.Add(Me.listAttachedFiles)
        Me.Controls.Add(Me.btnDeleteAttachment)
        Me.Controls.Add(Me.btnClear)
        Me.Controls.Add(Me.Label4)
        Me.Controls.Add(Me.btnBrowse)
        Me.Controls.Add(Me.TableLayoutPanel1)
        Me.Controls.Add(Me.txtMessage)
        Me.Controls.Add(Me.Label3)
        Me.Controls.Add(Me.txtEmail)
        Me.Controls.Add(Me.txtName)
        Me.Controls.Add(Me.Label2)
        Me.Controls.Add(Me.Label1)
        Me.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle
        Me.Icon = Global.Restore_Point_Creator.My.Resources.Resources.RestorePoint_noBackground_2
        Me.MaximizeBox = False
        Me.Name = "Official_Contact_Form"
        Me.Text = "Official Contact Form"
        Me.TableLayoutPanel1.ResumeLayout(False)
        Me.ResumeLayout(False)
        Me.PerformLayout()

    End Sub

    Friend WithEvents Label1 As Label
    Friend WithEvents Label2 As Label
    Friend WithEvents txtName As TextBox
    Friend WithEvents txtEmail As TextBox
    Friend WithEvents Label3 As Label
    Friend WithEvents txtMessage As TextBox
    Friend WithEvents TableLayoutPanel1 As TableLayoutPanel
    Friend WithEvents btnClose As Button
    Friend WithEvents btnSubmit As Button
    Friend WithEvents ToolTip As ToolTip
    Friend WithEvents btnBrowse As Button
    Friend WithEvents OpenFileDialog1 As OpenFileDialog
    Friend WithEvents Label4 As Label
    Friend WithEvents btnClear As Button
    Friend WithEvents btnDeleteAttachment As Button
    Friend WithEvents listAttachedFiles As ListBox
    Friend WithEvents btnAttachEventLogs As Button
End Class
