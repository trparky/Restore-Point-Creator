<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()>
Partial Class frmCrash
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
        Dim resources As System.ComponentModel.ComponentResourceManager = New System.ComponentModel.ComponentResourceManager(GetType(frmCrash))
        Me.txtStackTrace = New System.Windows.Forms.TextBox()
        Me.Label1 = New System.Windows.Forms.Label()
        Me.lblHeader = New System.Windows.Forms.Label()
        Me.Timer1 = New System.Windows.Forms.Timer(Me.components)
        Me.PictureBox1 = New System.Windows.Forms.PictureBox()
        Me.txtDoing = New System.Windows.Forms.TextBox()
        Me.Label5 = New System.Windows.Forms.Label()
        Me.Label2 = New System.Windows.Forms.Label()
        Me.chkReproducable = New System.Windows.Forms.CheckBox()
        Me.Label6 = New System.Windows.Forms.Label()
        Me.Label7 = New System.Windows.Forms.Label()
        Me.txtEmail = New System.Windows.Forms.TextBox()
        Me.txtName = New System.Windows.Forms.TextBox()
        Me.Label8 = New System.Windows.Forms.Label()
        Me.btnSubmitData = New System.Windows.Forms.Button()
        Me.btnClose = New System.Windows.Forms.Button()
        Me.ToolTip = New System.Windows.Forms.ToolTip(Me.components)
        Me.TableLayoutPanel1 = New System.Windows.Forms.TableLayoutPanel()
        Me.chkSendLogs = New System.Windows.Forms.CheckBox()
        Me.pleaseWaitMessageChanger = New System.Windows.Forms.Timer(Me.components)
        Me.pleaseWaitProgressBarChanger = New System.Windows.Forms.Timer(Me.components)
        Me.pleaseWaitPanel = New System.Windows.Forms.Panel()
        Me.pleaseWaitBorderText = New System.Windows.Forms.Label()
        Me.pleaseWaitlblLabel = New System.Windows.Forms.Label()
        Me.pleaseWaitProgressBar = New SmoothProgressBar()
        CType(Me.PictureBox1, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.TableLayoutPanel1.SuspendLayout()
        Me.pleaseWaitPanel.SuspendLayout()
        Me.SuspendLayout()
        '
        'txtStackTrace
        '
        Me.txtStackTrace.BackColor = System.Drawing.SystemColors.Window
        Me.txtStackTrace.Location = New System.Drawing.Point(12, 55)
        Me.txtStackTrace.Multiline = True
        Me.txtStackTrace.Name = "txtStackTrace"
        Me.txtStackTrace.ReadOnly = True
        Me.txtStackTrace.ScrollBars = System.Windows.Forms.ScrollBars.Vertical
        Me.txtStackTrace.Size = New System.Drawing.Size(511, 450)
        Me.txtStackTrace.TabIndex = 2
        '
        'Label1
        '
        Me.Label1.AutoSize = True
        Me.Label1.Location = New System.Drawing.Point(12, 39)
        Me.Label1.Name = "Label1"
        Me.Label1.Size = New System.Drawing.Size(102, 13)
        Me.Label1.TabIndex = 4
        Me.Label1.Text = "Detailed Crash Data"
        '
        'lblHeader
        '
        Me.lblHeader.Font = New System.Drawing.Font("Microsoft Sans Serif", 15.75!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.lblHeader.ForeColor = System.Drawing.Color.Red
        Me.lblHeader.Location = New System.Drawing.Point(51, 9)
        Me.lblHeader.Name = "lblHeader"
        Me.lblHeader.Size = New System.Drawing.Size(472, 23)
        Me.lblHeader.TabIndex = 10
        Me.lblHeader.Text = "Unhandled Application Error Detected!"
        Me.lblHeader.TextAlign = System.Drawing.ContentAlignment.MiddleCenter
        '
        'Timer1
        '
        Me.Timer1.Enabled = True
        Me.Timer1.Interval = 750
        '
        'PictureBox1
        '
        Me.PictureBox1.Image = Global.Restore_Point_Creator.My.Resources.Resources.removeBig
        Me.PictureBox1.Location = New System.Drawing.Point(12, 5)
        Me.PictureBox1.Name = "PictureBox1"
        Me.PictureBox1.Size = New System.Drawing.Size(33, 31)
        Me.PictureBox1.TabIndex = 11
        Me.PictureBox1.TabStop = False
        '
        'txtDoing
        '
        Me.txtDoing.Location = New System.Drawing.Point(532, 366)
        Me.txtDoing.Multiline = True
        Me.txtDoing.Name = "txtDoing"
        Me.txtDoing.Size = New System.Drawing.Size(464, 81)
        Me.txtDoing.TabIndex = 24
        '
        'Label5
        '
        Me.Label5.AutoSize = True
        Me.Label5.Location = New System.Drawing.Point(529, 350)
        Me.Label5.Name = "Label5"
        Me.Label5.Size = New System.Drawing.Size(266, 13)
        Me.Label5.TabIndex = 28
        Me.Label5.Text = "What were you doing at the time of the program crash?"
        '
        'Label2
        '
        Me.Label2.AutoSize = True
        Me.Label2.Font = New System.Drawing.Font("Microsoft Sans Serif", 8.25!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.Label2.Location = New System.Drawing.Point(529, 4)
        Me.Label2.Name = "Label2"
        Me.Label2.Size = New System.Drawing.Size(168, 13)
        Me.Label2.TabIndex = 27
        Me.Label2.Text = "Crash Data Submission Form"
        '
        'chkReproducable
        '
        Me.chkReproducable.AutoSize = True
        Me.chkReproducable.Location = New System.Drawing.Point(532, 310)
        Me.chkReproducable.Name = "chkReproducable"
        Me.chkReproducable.Size = New System.Drawing.Size(304, 30)
        Me.chkReproducable.TabIndex = 23
        Me.chkReproducable.Text = "Is this crash reproducable?" & Global.Microsoft.VisualBasic.ChrW(13) & Global.Microsoft.VisualBasic.ChrW(10) & "Meaning, does this issue happen all the time or some" &
    "times?"
        Me.chkReproducable.UseVisualStyleBackColor = True
        '
        'Label6
        '
        Me.Label6.AutoSize = True
        Me.Label6.Location = New System.Drawing.Point(529, 70)
        Me.Label6.Name = "Label6"
        Me.Label6.Size = New System.Drawing.Size(479, 234)
        Me.Label6.TabIndex = 26
        Me.Label6.Text = resources.GetString("Label6.Text")
        '
        'Label7
        '
        Me.Label7.AutoSize = True
        Me.Label7.Location = New System.Drawing.Point(529, 50)
        Me.Label7.Name = "Label7"
        Me.Label7.Size = New System.Drawing.Size(101, 13)
        Me.Label7.TabIndex = 25
        Me.Label7.Text = "Your Email Address:"
        '
        'txtEmail
        '
        Me.txtEmail.Location = New System.Drawing.Point(636, 47)
        Me.txtEmail.Name = "txtEmail"
        Me.txtEmail.Size = New System.Drawing.Size(360, 20)
        Me.txtEmail.TabIndex = 22
        '
        'txtName
        '
        Me.txtName.Location = New System.Drawing.Point(598, 20)
        Me.txtName.Name = "txtName"
        Me.txtName.Size = New System.Drawing.Size(398, 20)
        Me.txtName.TabIndex = 20
        '
        'Label8
        '
        Me.Label8.AutoSize = True
        Me.Label8.Location = New System.Drawing.Point(529, 23)
        Me.Label8.Name = "Label8"
        Me.Label8.Size = New System.Drawing.Size(63, 13)
        Me.Label8.TabIndex = 21
        Me.Label8.Text = "Your Name:"
        '
        'btnSubmitData
        '
        Me.btnSubmitData.Anchor = CType((((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Bottom) _
            Or System.Windows.Forms.AnchorStyles.Left) _
            Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.btnSubmitData.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft
        Me.btnSubmitData.Location = New System.Drawing.Point(3, 3)
        Me.btnSubmitData.Name = "btnSubmitData"
        Me.btnSubmitData.Size = New System.Drawing.Size(226, 23)
        Me.btnSubmitData.TabIndex = 29
        Me.btnSubmitData.Text = "Submit Crash Data"
        Me.btnSubmitData.UseVisualStyleBackColor = True
        '
        'btnClose
        '
        Me.btnClose.Anchor = CType((((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Bottom) _
            Or System.Windows.Forms.AnchorStyles.Left) _
            Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.btnClose.Location = New System.Drawing.Point(235, 3)
        Me.btnClose.Name = "btnClose"
        Me.btnClose.Size = New System.Drawing.Size(226, 23)
        Me.btnClose.TabIndex = 30
        Me.btnClose.Text = "Close"
        Me.btnClose.UseVisualStyleBackColor = True
        '
        'TableLayoutPanel1
        '
        Me.TableLayoutPanel1.ColumnCount = 2
        Me.TableLayoutPanel1.ColumnStyles.Add(New System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50.0!))
        Me.TableLayoutPanel1.ColumnStyles.Add(New System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50.0!))
        Me.TableLayoutPanel1.Controls.Add(Me.btnClose, 1, 0)
        Me.TableLayoutPanel1.Controls.Add(Me.btnSubmitData, 0, 0)
        Me.TableLayoutPanel1.Location = New System.Drawing.Point(532, 476)
        Me.TableLayoutPanel1.Name = "TableLayoutPanel1"
        Me.TableLayoutPanel1.RowCount = 1
        Me.TableLayoutPanel1.RowStyles.Add(New System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 50.0!))
        Me.TableLayoutPanel1.Size = New System.Drawing.Size(464, 29)
        Me.TableLayoutPanel1.TabIndex = 31
        '
        'chkSendLogs
        '
        Me.chkSendLogs.AutoSize = True
        Me.chkSendLogs.Checked = True
        Me.chkSendLogs.CheckState = System.Windows.Forms.CheckState.Checked
        Me.chkSendLogs.Location = New System.Drawing.Point(535, 453)
        Me.chkSendLogs.Name = "chkSendLogs"
        Me.chkSendLogs.Size = New System.Drawing.Size(301, 17)
        Me.chkSendLogs.TabIndex = 46
        Me.chkSendLogs.Text = "Send program logs as part of crash report (Recommended)"
        Me.chkSendLogs.UseVisualStyleBackColor = True
        '
        'pleaseWaitMessageChanger
        '
        Me.pleaseWaitMessageChanger.Interval = 250
        '
        'pleaseWaitProgressBarChanger
        '
        Me.pleaseWaitProgressBarChanger.Interval = 25
        '
        'pleaseWaitPanel
        '
        Me.pleaseWaitPanel.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle
        Me.pleaseWaitPanel.Controls.Add(Me.pleaseWaitBorderText)
        Me.pleaseWaitPanel.Controls.Add(Me.pleaseWaitlblLabel)
        Me.pleaseWaitPanel.Controls.Add(Me.pleaseWaitProgressBar)
        Me.pleaseWaitPanel.Location = New System.Drawing.Point(358, 213)
        Me.pleaseWaitPanel.Name = "pleaseWaitPanel"
        Me.pleaseWaitPanel.Size = New System.Drawing.Size(293, 86)
        Me.pleaseWaitPanel.TabIndex = 48
        Me.pleaseWaitPanel.Visible = False
        '
        'pleaseWaitBorderText
        '
        Me.pleaseWaitBorderText.BackColor = System.Drawing.Color.SkyBlue
        Me.pleaseWaitBorderText.Location = New System.Drawing.Point(0, 0)
        Me.pleaseWaitBorderText.Name = "pleaseWaitBorderText"
        Me.pleaseWaitBorderText.Padding = New System.Windows.Forms.Padding(3, 0, 0, 0)
        Me.pleaseWaitBorderText.Size = New System.Drawing.Size(292, 23)
        Me.pleaseWaitBorderText.TabIndex = 4
        Me.pleaseWaitBorderText.Text = "Please Wait..."
        Me.pleaseWaitBorderText.TextAlign = System.Drawing.ContentAlignment.MiddleLeft
        '
        'pleaseWaitlblLabel
        '
        Me.pleaseWaitlblLabel.AutoSize = True
        Me.pleaseWaitlblLabel.Location = New System.Drawing.Point(3, 31)
        Me.pleaseWaitlblLabel.Name = "pleaseWaitlblLabel"
        Me.pleaseWaitlblLabel.Size = New System.Drawing.Size(39, 13)
        Me.pleaseWaitlblLabel.TabIndex = 3
        Me.pleaseWaitlblLabel.Text = "Label1"
        '
        'pleaseWaitProgressBar
        '
        Me.pleaseWaitProgressBar.Location = New System.Drawing.Point(6, 56)
        Me.pleaseWaitProgressBar.Maximum = 100
        Me.pleaseWaitProgressBar.Minimum = 0
        Me.pleaseWaitProgressBar.Name = "pleaseWaitProgressBar"
        Me.pleaseWaitProgressBar.ProgressBarColor = System.Drawing.Color.Blue
        Me.pleaseWaitProgressBar.Size = New System.Drawing.Size(280, 19)
        Me.pleaseWaitProgressBar.TabIndex = 2
        Me.pleaseWaitProgressBar.Value = 0
        '
        'frmCrash
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(96.0!, 96.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi
        Me.ClientSize = New System.Drawing.Size(1008, 513)
        Me.Controls.Add(Me.pleaseWaitPanel)
        Me.Controls.Add(Me.chkSendLogs)
        Me.Controls.Add(Me.TableLayoutPanel1)
        Me.Controls.Add(Me.txtDoing)
        Me.Controls.Add(Me.Label5)
        Me.Controls.Add(Me.Label2)
        Me.Controls.Add(Me.chkReproducable)
        Me.Controls.Add(Me.Label6)
        Me.Controls.Add(Me.Label7)
        Me.Controls.Add(Me.txtEmail)
        Me.Controls.Add(Me.txtName)
        Me.Controls.Add(Me.Label8)
        Me.Controls.Add(Me.PictureBox1)
        Me.Controls.Add(Me.lblHeader)
        Me.Controls.Add(Me.Label1)
        Me.Controls.Add(Me.txtStackTrace)
        Me.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle
        Me.Icon = Global.Restore_Point_Creator.My.Resources.Resources.RestorePoint_noBackground_2
        Me.MaximizeBox = False
        Me.MinimizeBox = False
        Me.Name = "frmCrash"
        Me.Text = "Unhandled Application Error Detected!"
        CType(Me.PictureBox1, System.ComponentModel.ISupportInitialize).EndInit()
        Me.TableLayoutPanel1.ResumeLayout(False)
        Me.pleaseWaitPanel.ResumeLayout(False)
        Me.pleaseWaitPanel.PerformLayout()
        Me.ResumeLayout(False)
        Me.PerformLayout()

    End Sub
    Friend WithEvents txtStackTrace As System.Windows.Forms.TextBox
    Friend WithEvents Label1 As System.Windows.Forms.Label
    Friend WithEvents lblHeader As System.Windows.Forms.Label
    Friend WithEvents Timer1 As System.Windows.Forms.Timer
    Friend WithEvents PictureBox1 As System.Windows.Forms.PictureBox
    Friend WithEvents txtDoing As TextBox
    Friend WithEvents Label5 As Label
    Friend WithEvents Label2 As Label
    Friend WithEvents chkReproducable As CheckBox
    Friend WithEvents Label6 As Label
    Friend WithEvents Label7 As Label
    Friend WithEvents txtEmail As TextBox
    Friend WithEvents txtName As TextBox
    Friend WithEvents Label8 As Label
    Friend WithEvents btnSubmitData As Button
    Friend WithEvents btnClose As Button
    Friend WithEvents ToolTip As ToolTip
    Friend WithEvents TableLayoutPanel1 As TableLayoutPanel
    Friend WithEvents chkSendLogs As CheckBox
    Friend WithEvents pleaseWaitMessageChanger As Timer
    Friend WithEvents pleaseWaitProgressBarChanger As Timer
    Friend WithEvents pleaseWaitPanel As Panel
    Friend WithEvents pleaseWaitBorderText As Label
    Friend WithEvents pleaseWaitlblLabel As Label
    Friend WithEvents pleaseWaitProgressBar As SmoothProgressBar
End Class
