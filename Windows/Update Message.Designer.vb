<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()>
Partial Class Update_Message
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
        Me.txtChanges = New System.Windows.Forms.TextBox()
        Me.Label2 = New System.Windows.Forms.Label()
        Me.PictureBox1 = New System.Windows.Forms.PictureBox()
        Me.btnOK = New System.Windows.Forms.Button()
        Me.TableLayoutPanel1 = New System.Windows.Forms.TableLayoutPanel()
        Me.btnYes = New System.Windows.Forms.Button()
        Me.btnNo = New System.Windows.Forms.Button()
        Me.lblCountdown = New System.Windows.Forms.Label()
        Me.btnReloadChangeLog = New System.Windows.Forms.Button()
        Me.lblChangesAsFollows = New System.Windows.Forms.Label()
        Me.lblCurrentVersion = New System.Windows.Forms.Label()
        Me.chkShowPartialBetaChangeLogs = New System.Windows.Forms.CheckBox()
        Me.ToolTip1 = New System.Windows.Forms.ToolTip(Me.components)
        Me.imgSSL = New System.Windows.Forms.PictureBox()
        Me.timerCountdown = New System.Windows.Forms.Timer(Me.components)
        Me.lblTopUpdateMessage = New System.Windows.Forms.Label()
        CType(Me.PictureBox1, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.TableLayoutPanel1.SuspendLayout()
        CType(Me.imgSSL, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.SuspendLayout()
        '
        'txtChanges
        '
        Me.txtChanges.Anchor = CType((((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Bottom) _
            Or System.Windows.Forms.AnchorStyles.Left) _
            Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.txtChanges.BackColor = System.Drawing.SystemColors.Window
        Me.txtChanges.Location = New System.Drawing.Point(9, 68)
        Me.txtChanges.Multiline = True
        Me.txtChanges.Name = "txtChanges"
        Me.txtChanges.ReadOnly = True
        Me.txtChanges.ScrollBars = System.Windows.Forms.ScrollBars.Vertical
        Me.txtChanges.Size = New System.Drawing.Size(751, 160)
        Me.txtChanges.TabIndex = 1
        Me.txtChanges.Text = "Loading Change Log Data... Please Wait."
        '
        'Label2
        '
        Me.Label2.Anchor = CType(((System.Windows.Forms.AnchorStyles.Bottom Or System.Windows.Forms.AnchorStyles.Left) _
            Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.Label2.AutoSize = True
        Me.Label2.Font = New System.Drawing.Font("Microsoft Sans Serif", 8.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.Label2.Location = New System.Drawing.Point(9, 238)
        Me.Label2.Name = "Label2"
        Me.Label2.Size = New System.Drawing.Size(316, 13)
        Me.Label2.TabIndex = 2
        Me.Label2.Text = "The update will now be downloaded and installed on your system."
        '
        'PictureBox1
        '
        Me.PictureBox1.Location = New System.Drawing.Point(9, 12)
        Me.PictureBox1.Name = "PictureBox1"
        Me.PictureBox1.Size = New System.Drawing.Size(32, 35)
        Me.PictureBox1.TabIndex = 3
        Me.PictureBox1.TabStop = False
        '
        'btnOK
        '
        Me.btnOK.Anchor = CType(((System.Windows.Forms.AnchorStyles.Bottom Or System.Windows.Forms.AnchorStyles.Left) _
            Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.btnOK.Location = New System.Drawing.Point(12, 258)
        Me.btnOK.Name = "btnOK"
        Me.btnOK.Size = New System.Drawing.Size(748, 23)
        Me.btnOK.TabIndex = 4
        Me.btnOK.Text = "OK"
        Me.btnOK.UseVisualStyleBackColor = True
        '
        'TableLayoutPanel1
        '
        Me.TableLayoutPanel1.Anchor = CType(((System.Windows.Forms.AnchorStyles.Bottom Or System.Windows.Forms.AnchorStyles.Left) _
            Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.TableLayoutPanel1.ColumnCount = 3
        Me.TableLayoutPanel1.ColumnStyles.Add(New System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50.0!))
        Me.TableLayoutPanel1.ColumnStyles.Add(New System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 25.0!))
        Me.TableLayoutPanel1.ColumnStyles.Add(New System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50.0!))
        Me.TableLayoutPanel1.Controls.Add(Me.btnYes, 0, 0)
        Me.TableLayoutPanel1.Controls.Add(Me.btnNo, 2, 0)
        Me.TableLayoutPanel1.Controls.Add(Me.lblCountdown, 1, 0)
        Me.TableLayoutPanel1.Location = New System.Drawing.Point(12, 258)
        Me.TableLayoutPanel1.Name = "TableLayoutPanel1"
        Me.TableLayoutPanel1.RowCount = 1
        Me.TableLayoutPanel1.RowStyles.Add(New System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100.0!))
        Me.TableLayoutPanel1.Size = New System.Drawing.Size(748, 29)
        Me.TableLayoutPanel1.TabIndex = 5
        '
        'btnYes
        '
        Me.btnYes.Anchor = CType((((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Bottom) _
            Or System.Windows.Forms.AnchorStyles.Left) _
            Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.btnYes.Image = Global.Restore_Point_Creator.My.Resources.Resources.down
        Me.btnYes.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft
        Me.btnYes.Location = New System.Drawing.Point(3, 3)
        Me.btnYes.Name = "btnYes"
        Me.btnYes.Size = New System.Drawing.Size(355, 23)
        Me.btnYes.TabIndex = 6
        Me.btnYes.Text = "&Yes (Recommended)"
        Me.btnYes.UseVisualStyleBackColor = True
        '
        'btnNo
        '
        Me.btnNo.Anchor = CType((((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Bottom) _
            Or System.Windows.Forms.AnchorStyles.Left) _
            Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.btnNo.Image = Global.Restore_Point_Creator.My.Resources.Resources.removeSmall
        Me.btnNo.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft
        Me.btnNo.Location = New System.Drawing.Point(389, 3)
        Me.btnNo.Name = "btnNo"
        Me.btnNo.Size = New System.Drawing.Size(356, 23)
        Me.btnNo.TabIndex = 6
        Me.btnNo.Text = "&No"
        Me.btnNo.UseVisualStyleBackColor = True
        '
        'lblCountdown
        '
        Me.lblCountdown.AutoSize = True
        Me.lblCountdown.Location = New System.Drawing.Point(364, 8)
        Me.lblCountdown.Margin = New System.Windows.Forms.Padding(3, 8, 3, 0)
        Me.lblCountdown.Name = "lblCountdown"
        Me.lblCountdown.Size = New System.Drawing.Size(19, 13)
        Me.lblCountdown.TabIndex = 7
        Me.lblCountdown.Text = "30"
        Me.lblCountdown.TextAlign = System.Drawing.ContentAlignment.MiddleCenter
        '
        'btnReloadChangeLog
        '
        Me.btnReloadChangeLog.Image = Global.Restore_Point_Creator.My.Resources.Resources.refresh
        Me.btnReloadChangeLog.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft
        Me.btnReloadChangeLog.Location = New System.Drawing.Point(635, 39)
        Me.btnReloadChangeLog.Name = "btnReloadChangeLog"
        Me.btnReloadChangeLog.Size = New System.Drawing.Size(125, 23)
        Me.btnReloadChangeLog.TabIndex = 6
        Me.btnReloadChangeLog.Text = "Reload Change Log"
        Me.btnReloadChangeLog.TextAlign = System.Drawing.ContentAlignment.MiddleRight
        Me.btnReloadChangeLog.UseVisualStyleBackColor = True
        '
        'lblChangesAsFollows
        '
        Me.lblChangesAsFollows.AutoSize = True
        Me.lblChangesAsFollows.Location = New System.Drawing.Point(47, 49)
        Me.lblChangesAsFollows.Name = "lblChangesAsFollows"
        Me.lblChangesAsFollows.Size = New System.Drawing.Size(146, 13)
        Me.lblChangesAsFollows.TabIndex = 7
        Me.lblChangesAsFollows.Text = "The changes are as follows..."
        '
        'lblCurrentVersion
        '
        Me.lblCurrentVersion.AutoSize = True
        Me.lblCurrentVersion.Location = New System.Drawing.Point(245, 49)
        Me.lblCurrentVersion.Name = "lblCurrentVersion"
        Me.lblCurrentVersion.Size = New System.Drawing.Size(39, 13)
        Me.lblCurrentVersion.TabIndex = 13
        Me.lblCurrentVersion.Text = "Label1"
        '
        'chkShowPartialBetaChangeLogs
        '
        Me.chkShowPartialBetaChangeLogs.Anchor = CType((System.Windows.Forms.AnchorStyles.Bottom Or System.Windows.Forms.AnchorStyles.Left), System.Windows.Forms.AnchorStyles)
        Me.chkShowPartialBetaChangeLogs.AutoSize = True
        Me.chkShowPartialBetaChangeLogs.Location = New System.Drawing.Point(586, 238)
        Me.chkShowPartialBetaChangeLogs.Name = "chkShowPartialBetaChangeLogs"
        Me.chkShowPartialBetaChangeLogs.Size = New System.Drawing.Size(176, 17)
        Me.chkShowPartialBetaChangeLogs.TabIndex = 14
        Me.chkShowPartialBetaChangeLogs.Text = "Show Partial Beta Change Logs"
        Me.chkShowPartialBetaChangeLogs.UseVisualStyleBackColor = True
        '
        'imgSSL
        '
        Me.imgSSL.Anchor = CType((System.Windows.Forms.AnchorStyles.Bottom Or System.Windows.Forms.AnchorStyles.Left), System.Windows.Forms.AnchorStyles)
        Me.imgSSL.Image = Global.Restore_Point_Creator.My.Resources.Resources.lock
        Me.imgSSL.Location = New System.Drawing.Point(562, 238)
        Me.imgSSL.Name = "imgSSL"
        Me.imgSSL.Size = New System.Drawing.Size(18, 18)
        Me.imgSSL.TabIndex = 15
        Me.imgSSL.TabStop = False
        Me.ToolTip1.SetToolTip(Me.imgSSL, "Download Secured by SSL.")
        Me.imgSSL.Visible = False
        '
        'timerCountdown
        '
        Me.timerCountdown.Interval = 1000
        '
        'lblTopUpdateMessage
        '
        Me.lblTopUpdateMessage.AutoSize = True
        Me.lblTopUpdateMessage.Location = New System.Drawing.Point(47, 10)
        Me.lblTopUpdateMessage.Name = "lblTopUpdateMessage"
        Me.lblTopUpdateMessage.Size = New System.Drawing.Size(388, 13)
        Me.lblTopUpdateMessage.TabIndex = 9
        Me.lblTopUpdateMessage.Text = "There is an updated Release Candidate version of System Restore Point Creator."
        '
        'Update_Message
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(96.0!, 96.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi
        Me.ClientSize = New System.Drawing.Size(772, 295)
        Me.Controls.Add(Me.imgSSL)
        Me.Controls.Add(Me.chkShowPartialBetaChangeLogs)
        Me.Controls.Add(Me.lblCurrentVersion)
        Me.Controls.Add(Me.lblTopUpdateMessage)
        Me.Controls.Add(Me.lblChangesAsFollows)
        Me.Controls.Add(Me.btnReloadChangeLog)
        Me.Controls.Add(Me.TableLayoutPanel1)
        Me.Controls.Add(Me.btnOK)
        Me.Controls.Add(Me.PictureBox1)
        Me.Controls.Add(Me.Label2)
        Me.Controls.Add(Me.txtChanges)
        Me.KeyPreview = True
        Me.MaximizeBox = False
        Me.MinimizeBox = False
        Me.MinimumSize = New System.Drawing.Size(788, 334)
        Me.Name = "Update_Message"
        Me.Text = "System Restore Point Creator Version Checker"
        CType(Me.PictureBox1, System.ComponentModel.ISupportInitialize).EndInit()
        Me.TableLayoutPanel1.ResumeLayout(False)
        Me.TableLayoutPanel1.PerformLayout()
        CType(Me.imgSSL, System.ComponentModel.ISupportInitialize).EndInit()
        Me.ResumeLayout(False)
        Me.PerformLayout()

    End Sub
    Friend WithEvents txtChanges As TextBox
    Friend WithEvents Label2 As Label
    Friend WithEvents PictureBox1 As PictureBox
    Friend WithEvents btnOK As Button
    Friend WithEvents TableLayoutPanel1 As TableLayoutPanel
    Friend WithEvents btnNo As Button
    Friend WithEvents btnYes As Button
    Friend WithEvents btnReloadChangeLog As Button
    Friend WithEvents lblChangesAsFollows As Label
    Friend WithEvents lblCurrentVersion As Label
    Friend WithEvents chkShowPartialBetaChangeLogs As CheckBox
    Friend WithEvents ToolTip1 As ToolTip
    Friend WithEvents imgSSL As PictureBox
    Friend WithEvents timerCountdown As Timer
    Friend WithEvents lblCountdown As Label
    Friend WithEvents lblTopUpdateMessage As Label
End Class
