<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Class Setup_Auto_Crash_Submission
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
        Dim resources As System.ComponentModel.ComponentResourceManager = New System.ComponentModel.ComponentResourceManager(GetType(Setup_Auto_Crash_Submission))
        Me.Label1 = New System.Windows.Forms.Label()
        Me.Label2 = New System.Windows.Forms.Label()
        Me.Label3 = New System.Windows.Forms.Label()
        Me.txtName = New System.Windows.Forms.TextBox()
        Me.txtEmail = New System.Windows.Forms.TextBox()
        Me.Label4 = New System.Windows.Forms.Label()
        Me.btnEnableAutoCrashSubmission = New System.Windows.Forms.Button()
        Me.TableLayoutPanel1 = New System.Windows.Forms.TableLayoutPanel()
        Me.btnDisableAutoCrashSubmission = New System.Windows.Forms.Button()
        Me.TableLayoutPanel1.SuspendLayout()
        Me.SuspendLayout()
        '
        'Label1
        '
        Me.Label1.Location = New System.Drawing.Point(12, 9)
        Me.Label1.Name = "Label1"
        Me.Label1.Size = New System.Drawing.Size(592, 110)
        Me.Label1.TabIndex = 0
        Me.Label1.Text = resources.GetString("Label1.Text")
        '
        'Label2
        '
        Me.Label2.AutoSize = True
        Me.Label2.Location = New System.Drawing.Point(12, 125)
        Me.Label2.Name = "Label2"
        Me.Label2.Size = New System.Drawing.Size(60, 13)
        Me.Label2.TabIndex = 1
        Me.Label2.Text = "Your Name"
        '
        'Label3
        '
        Me.Label3.AutoSize = True
        Me.Label3.Location = New System.Drawing.Point(12, 151)
        Me.Label3.Name = "Label3"
        Me.Label3.Size = New System.Drawing.Size(98, 13)
        Me.Label3.TabIndex = 2
        Me.Label3.Text = "Your Email Address"
        '
        'txtName
        '
        Me.txtName.Location = New System.Drawing.Point(78, 122)
        Me.txtName.Name = "txtName"
        Me.txtName.Size = New System.Drawing.Size(526, 20)
        Me.txtName.TabIndex = 3
        '
        'txtEmail
        '
        Me.txtEmail.Location = New System.Drawing.Point(116, 148)
        Me.txtEmail.Name = "txtEmail"
        Me.txtEmail.Size = New System.Drawing.Size(488, 20)
        Me.txtEmail.TabIndex = 4
        '
        'Label4
        '
        Me.Label4.Location = New System.Drawing.Point(12, 171)
        Me.Label4.Name = "Label4"
        Me.Label4.Size = New System.Drawing.Size(592, 15)
        Me.Label4.TabIndex = 5
        Me.Label4.Text = "All information you send and is sent by this program will be kept STRICTLY confid" &
    "ential. It will not be sold to third-parties."
        '
        'btnEnableAutoCrashSubmission
        '
        Me.btnEnableAutoCrashSubmission.Anchor = CType((((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Bottom) _
            Or System.Windows.Forms.AnchorStyles.Left) _
            Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.btnEnableAutoCrashSubmission.Location = New System.Drawing.Point(3, 3)
        Me.btnEnableAutoCrashSubmission.Name = "btnEnableAutoCrashSubmission"
        Me.btnEnableAutoCrashSubmission.Size = New System.Drawing.Size(288, 23)
        Me.btnEnableAutoCrashSubmission.TabIndex = 6
        Me.btnEnableAutoCrashSubmission.Text = "Enable Auto Crash Submission"
        Me.btnEnableAutoCrashSubmission.UseVisualStyleBackColor = True
        '
        'TableLayoutPanel1
        '
        Me.TableLayoutPanel1.ColumnCount = 2
        Me.TableLayoutPanel1.ColumnStyles.Add(New System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50.0!))
        Me.TableLayoutPanel1.ColumnStyles.Add(New System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50.0!))
        Me.TableLayoutPanel1.Controls.Add(Me.btnDisableAutoCrashSubmission, 0, 0)
        Me.TableLayoutPanel1.Controls.Add(Me.btnEnableAutoCrashSubmission, 0, 0)
        Me.TableLayoutPanel1.Location = New System.Drawing.Point(15, 189)
        Me.TableLayoutPanel1.Name = "TableLayoutPanel1"
        Me.TableLayoutPanel1.RowCount = 1
        Me.TableLayoutPanel1.RowStyles.Add(New System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 50.0!))
        Me.TableLayoutPanel1.Size = New System.Drawing.Size(589, 29)
        Me.TableLayoutPanel1.TabIndex = 7
        '
        'btnDisableAutoCrashSubmission
        '
        Me.btnDisableAutoCrashSubmission.Anchor = CType((((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Bottom) _
            Or System.Windows.Forms.AnchorStyles.Left) _
            Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.btnDisableAutoCrashSubmission.Location = New System.Drawing.Point(297, 3)
        Me.btnDisableAutoCrashSubmission.Name = "btnDisableAutoCrashSubmission"
        Me.btnDisableAutoCrashSubmission.Size = New System.Drawing.Size(289, 23)
        Me.btnDisableAutoCrashSubmission.TabIndex = 7
        Me.btnDisableAutoCrashSubmission.Text = "Disable Auto Crash Submission"
        Me.btnDisableAutoCrashSubmission.UseVisualStyleBackColor = True
        '
        'Setup_Auto_Crash_Submission
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(6.0!, 13.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.ClientSize = New System.Drawing.Size(616, 226)
        Me.Controls.Add(Me.TableLayoutPanel1)
        Me.Controls.Add(Me.Label4)
        Me.Controls.Add(Me.txtEmail)
        Me.Controls.Add(Me.txtName)
        Me.Controls.Add(Me.Label3)
        Me.Controls.Add(Me.Label2)
        Me.Controls.Add(Me.Label1)
        Me.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle
        Me.Icon = Global.Restore_Point_Creator.My.Resources.Resources.RestorePoint_noBackground_2
        Me.MaximizeBox = False
        Me.MinimizeBox = False
        Me.Name = "Setup_Auto_Crash_Submission"
        Me.Text = "Setup Auto Crash Submission"
        Me.TableLayoutPanel1.ResumeLayout(False)
        Me.ResumeLayout(False)
        Me.PerformLayout()

    End Sub

    Friend WithEvents Label1 As Label
    Friend WithEvents Label2 As Label
    Friend WithEvents Label3 As Label
    Friend WithEvents txtName As TextBox
    Friend WithEvents txtEmail As TextBox
    Friend WithEvents Label4 As Label
    Friend WithEvents btnEnableAutoCrashSubmission As Button
    Friend WithEvents TableLayoutPanel1 As TableLayoutPanel
    Friend WithEvents btnDisableAutoCrashSubmission As Button
End Class
