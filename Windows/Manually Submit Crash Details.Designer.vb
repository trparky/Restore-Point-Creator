<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Class frmManuallySubmitCrashData
    Inherits System.Windows.Forms.Form

    'Form overrides dispose to clean up the component list.
    <System.Diagnostics.DebuggerNonUserCode()> _
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
    <System.Diagnostics.DebuggerStepThrough()> _
    Private Sub InitializeComponent()
        Me.components = New System.ComponentModel.Container()
        Dim resources As System.ComponentModel.ComponentResourceManager = New System.ComponentModel.ComponentResourceManager(GetType(frmManuallySubmitCrashData))
        Me.btnClose = New System.Windows.Forms.Button()
        Me.btnSubmitData = New System.Windows.Forms.Button()
        Me.txtDoing = New System.Windows.Forms.TextBox()
        Me.Label5 = New System.Windows.Forms.Label()
        Me.Label2 = New System.Windows.Forms.Label()
        Me.chkReproducable = New System.Windows.Forms.CheckBox()
        Me.Label7 = New System.Windows.Forms.Label()
        Me.txtEmail = New System.Windows.Forms.TextBox()
        Me.txtName = New System.Windows.Forms.TextBox()
        Me.Label8 = New System.Windows.Forms.Label()
        Me.ToolTip = New System.Windows.Forms.ToolTip(Me.components)
        Me.TableLayoutPanel1 = New System.Windows.Forms.TableLayoutPanel()
        Me.chkSendLogs = New System.Windows.Forms.CheckBox()
        Me.Label6 = New System.Windows.Forms.Label()
        Me.TableLayoutPanel1.SuspendLayout()
        Me.SuspendLayout()
        '
        'btnClose
        '
        Me.btnClose.Anchor = CType((((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Bottom) _
            Or System.Windows.Forms.AnchorStyles.Left) _
            Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.btnClose.Location = New System.Drawing.Point(241, 3)
        Me.btnClose.Name = "btnClose"
        Me.btnClose.Size = New System.Drawing.Size(232, 23)
        Me.btnClose.TabIndex = 42
        Me.btnClose.Text = "Close"
        Me.btnClose.UseVisualStyleBackColor = True
        '
        'btnSubmitData
        '
        Me.btnSubmitData.Anchor = CType((((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Bottom) _
            Or System.Windows.Forms.AnchorStyles.Left) _
            Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.btnSubmitData.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft
        Me.btnSubmitData.Location = New System.Drawing.Point(3, 3)
        Me.btnSubmitData.Name = "btnSubmitData"
        Me.btnSubmitData.Size = New System.Drawing.Size(232, 23)
        Me.btnSubmitData.TabIndex = 41
        Me.btnSubmitData.Text = "Submit Crash Data"
        Me.btnSubmitData.UseVisualStyleBackColor = True
        '
        'txtDoing
        '
        Me.txtDoing.Location = New System.Drawing.Point(10, 373)
        Me.txtDoing.Multiline = True
        Me.txtDoing.Name = "txtDoing"
        Me.txtDoing.Size = New System.Drawing.Size(476, 69)
        Me.txtDoing.TabIndex = 36
        '
        'Label5
        '
        Me.Label5.AutoSize = True
        Me.Label5.Location = New System.Drawing.Point(9, 357)
        Me.Label5.Name = "Label5"
        Me.Label5.Size = New System.Drawing.Size(266, 13)
        Me.Label5.TabIndex = 40
        Me.Label5.Text = "What were you doing at the time of the program crash?"
        '
        'Label2
        '
        Me.Label2.AutoSize = True
        Me.Label2.Font = New System.Drawing.Font("Microsoft Sans Serif", 8.25!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.Label2.Location = New System.Drawing.Point(12, 12)
        Me.Label2.Name = "Label2"
        Me.Label2.Size = New System.Drawing.Size(168, 13)
        Me.Label2.TabIndex = 39
        Me.Label2.Text = "Crash Data Submission Form"
        '
        'chkReproducable
        '
        Me.chkReproducable.AutoSize = True
        Me.chkReproducable.Location = New System.Drawing.Point(12, 320)
        Me.chkReproducable.Name = "chkReproducable"
        Me.chkReproducable.Size = New System.Drawing.Size(304, 30)
        Me.chkReproducable.TabIndex = 35
        Me.chkReproducable.Text = "Is this crash reproducable?" & Global.Microsoft.VisualBasic.ChrW(13) & Global.Microsoft.VisualBasic.ChrW(10) & "Meaning, does this issue happen all the time or some" &
    "times?"
        Me.chkReproducable.UseVisualStyleBackColor = True
        '
        'Label7
        '
        Me.Label7.AutoSize = True
        Me.Label7.Location = New System.Drawing.Point(12, 58)
        Me.Label7.Name = "Label7"
        Me.Label7.Size = New System.Drawing.Size(101, 13)
        Me.Label7.TabIndex = 37
        Me.Label7.Text = "Your Email Address:"
        '
        'txtEmail
        '
        Me.txtEmail.Location = New System.Drawing.Point(119, 55)
        Me.txtEmail.Name = "txtEmail"
        Me.txtEmail.Size = New System.Drawing.Size(367, 20)
        Me.txtEmail.TabIndex = 34
        '
        'txtName
        '
        Me.txtName.Location = New System.Drawing.Point(81, 28)
        Me.txtName.Name = "txtName"
        Me.txtName.Size = New System.Drawing.Size(405, 20)
        Me.txtName.TabIndex = 32
        '
        'Label8
        '
        Me.Label8.AutoSize = True
        Me.Label8.Location = New System.Drawing.Point(12, 31)
        Me.Label8.Name = "Label8"
        Me.Label8.Size = New System.Drawing.Size(63, 13)
        Me.Label8.TabIndex = 33
        Me.Label8.Text = "Your Name:"
        '
        'TableLayoutPanel1
        '
        Me.TableLayoutPanel1.ColumnCount = 2
        Me.TableLayoutPanel1.ColumnStyles.Add(New System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50.0!))
        Me.TableLayoutPanel1.ColumnStyles.Add(New System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50.0!))
        Me.TableLayoutPanel1.Controls.Add(Me.btnClose, 1, 0)
        Me.TableLayoutPanel1.Controls.Add(Me.btnSubmitData, 0, 0)
        Me.TableLayoutPanel1.Location = New System.Drawing.Point(10, 471)
        Me.TableLayoutPanel1.Name = "TableLayoutPanel1"
        Me.TableLayoutPanel1.RowCount = 1
        Me.TableLayoutPanel1.RowStyles.Add(New System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 50.0!))
        Me.TableLayoutPanel1.Size = New System.Drawing.Size(476, 29)
        Me.TableLayoutPanel1.TabIndex = 44
        '
        'chkSendLogs
        '
        Me.chkSendLogs.AutoSize = True
        Me.chkSendLogs.Checked = True
        Me.chkSendLogs.CheckState = System.Windows.Forms.CheckState.Checked
        Me.chkSendLogs.Location = New System.Drawing.Point(10, 448)
        Me.chkSendLogs.Name = "chkSendLogs"
        Me.chkSendLogs.Size = New System.Drawing.Size(301, 17)
        Me.chkSendLogs.TabIndex = 45
        Me.chkSendLogs.Text = "Send program logs as part of crash report (Recommended)"
        Me.chkSendLogs.UseVisualStyleBackColor = True
        '
        'Label6
        '
        Me.Label6.AutoSize = True
        Me.Label6.Location = New System.Drawing.Point(12, 78)
        Me.Label6.Name = "Label6"
        Me.Label6.Size = New System.Drawing.Size(479, 234)
        Me.Label6.TabIndex = 46
        Me.Label6.Text = resources.GetString("Label6.Text")
        '
        'frmManuallySubmitCrashData
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(6.0!, 13.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi
        Me.ClientSize = New System.Drawing.Size(498, 509)
        Me.Controls.Add(Me.Label6)
        Me.Controls.Add(Me.chkSendLogs)
        Me.Controls.Add(Me.TableLayoutPanel1)
        Me.Controls.Add(Me.txtDoing)
        Me.Controls.Add(Me.Label5)
        Me.Controls.Add(Me.Label2)
        Me.Controls.Add(Me.chkReproducable)
        Me.Controls.Add(Me.Label7)
        Me.Controls.Add(Me.txtEmail)
        Me.Controls.Add(Me.txtName)
        Me.Controls.Add(Me.Label8)
        Me.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle
        Me.Icon = Global.Restore_Point_Creator.My.Resources.Resources.RestorePoint_noBackground_2
        Me.MaximizeBox = False
        Me.Name = "frmManuallySubmitCrashData"
        Me.Text = "Manually Submit Crash Details"
        Me.TableLayoutPanel1.ResumeLayout(False)
        Me.ResumeLayout(False)
        Me.PerformLayout()

    End Sub

    Friend WithEvents btnClose As Button
    Friend WithEvents btnSubmitData As Button
    Friend WithEvents txtDoing As TextBox
    Friend WithEvents Label5 As Label
    Friend WithEvents Label2 As Label
    Friend WithEvents chkReproducable As CheckBox
    Friend WithEvents Label7 As Label
    Friend WithEvents txtEmail As TextBox
    Friend WithEvents txtName As TextBox
    Friend WithEvents Label8 As Label
    Friend WithEvents ToolTip As ToolTip
    Friend WithEvents TableLayoutPanel1 As TableLayoutPanel
    Friend WithEvents chkSendLogs As CheckBox
    Friend WithEvents Label6 As Label
End Class
