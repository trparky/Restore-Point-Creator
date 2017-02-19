<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Class frmManageSystemRestoreStorageSpace
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
        Me.SplitContainer1 = New System.Windows.Forms.SplitContainer()
        Me.listDrives = New System.Windows.Forms.ListView()
        Me.Label1 = New System.Windows.Forms.Label()
        Me.lblDriveSize = New System.Windows.Forms.Label()
        Me.chkConfirmNewSmallerSize = New System.Windows.Forms.CheckBox()
        Me.Label2 = New System.Windows.Forms.Label()
        Me.percentageIndicator = New Tom.SmoothProgressBar()
        Me.txtSize = New System.Windows.Forms.TextBox()
        Me.lblDriveLabel = New System.Windows.Forms.Label()
        Me.listSizeType = New System.Windows.Forms.ComboBox()
        Me.lblUsedShadowStorageSpace = New System.Windows.Forms.Label()
        Me.btnSetSize = New System.Windows.Forms.Button()
        Me.chkAdvancedMode = New System.Windows.Forms.CheckBox()
        CType(Me.SplitContainer1, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.SplitContainer1.Panel1.SuspendLayout()
        Me.SplitContainer1.Panel2.SuspendLayout()
        Me.SplitContainer1.SuspendLayout()
        Me.SuspendLayout()
        '
        'SplitContainer1
        '
        Me.SplitContainer1.FixedPanel = System.Windows.Forms.FixedPanel.Panel2
        Me.SplitContainer1.Location = New System.Drawing.Point(0, 0)
        Me.SplitContainer1.Name = "SplitContainer1"
        '
        'SplitContainer1.Panel1
        '
        Me.SplitContainer1.Panel1.Controls.Add(Me.listDrives)
        Me.SplitContainer1.Panel1.Controls.Add(Me.Label1)
        Me.SplitContainer1.Panel1MinSize = 0
        '
        'SplitContainer1.Panel2
        '
        Me.SplitContainer1.Panel2.Controls.Add(Me.lblDriveSize)
        Me.SplitContainer1.Panel2.Controls.Add(Me.Label2)
        Me.SplitContainer1.Panel2.Controls.Add(Me.percentageIndicator)
        Me.SplitContainer1.Panel2.Controls.Add(Me.txtSize)
        Me.SplitContainer1.Panel2.Controls.Add(Me.lblDriveLabel)
        Me.SplitContainer1.Panel2.Controls.Add(Me.listSizeType)
        Me.SplitContainer1.Panel2.Controls.Add(Me.lblUsedShadowStorageSpace)
        Me.SplitContainer1.Panel2.Controls.Add(Me.btnSetSize)
        Me.SplitContainer1.Panel2MinSize = 385
        Me.SplitContainer1.Size = New System.Drawing.Size(463, 139)
        Me.SplitContainer1.SplitterDistance = 74
        Me.SplitContainer1.TabIndex = 16
        '
        'listDrives
        '
        Me.listDrives.Location = New System.Drawing.Point(10, 24)
        Me.listDrives.Name = "listDrives"
        Me.listDrives.Size = New System.Drawing.Size(52, 109)
        Me.listDrives.TabIndex = 2
        Me.listDrives.UseCompatibleStateImageBehavior = False
        Me.listDrives.View = System.Windows.Forms.View.List
        '
        'Label1
        '
        Me.Label1.AutoSize = True
        Me.Label1.Location = New System.Drawing.Point(9, 9)
        Me.Label1.Name = "Label1"
        Me.Label1.Size = New System.Drawing.Size(32, 13)
        Me.Label1.TabIndex = 0
        Me.Label1.Text = "Drive"
        '
        'lblDriveSize
        '
        Me.lblDriveSize.AutoSize = True
        Me.lblDriveSize.Location = New System.Drawing.Point(3, 8)
        Me.lblDriveSize.Name = "lblDriveSize"
        Me.lblDriveSize.Size = New System.Drawing.Size(97, 13)
        Me.lblDriveSize.TabIndex = 6
        Me.lblDriveSize.Text = "Total Size of Drive:"
        '
        'chkConfirmNewSmallerSize
        '
        Me.chkConfirmNewSmallerSize.AutoSize = True
        Me.chkConfirmNewSmallerSize.Location = New System.Drawing.Point(10, 139)
        Me.chkConfirmNewSmallerSize.Name = "chkConfirmNewSmallerSize"
        Me.chkConfirmNewSmallerSize.Size = New System.Drawing.Size(220, 17)
        Me.chkConfirmNewSmallerSize.TabIndex = 15
        Me.chkConfirmNewSmallerSize.Text = "Confirm if new size is smaller than old size"
        Me.chkConfirmNewSmallerSize.UseVisualStyleBackColor = True
        '
        'Label2
        '
        Me.Label2.AutoSize = True
        Me.Label2.Location = New System.Drawing.Point(7, 86)
        Me.Label2.Name = "Label2"
        Me.Label2.Size = New System.Drawing.Size(174, 13)
        Me.Label2.TabIndex = 2
        Me.Label2.Text = "System Restore Point Storage Size:"
        '
        'percentageIndicator
        '
        Me.percentageIndicator.Location = New System.Drawing.Point(6, 56)
        Me.percentageIndicator.Maximum = 100
        Me.percentageIndicator.Minimum = 0
        Me.percentageIndicator.Name = "percentageIndicator"
        Me.percentageIndicator.ProgressBarColor = System.Drawing.Color.Blue
        Me.percentageIndicator.Size = New System.Drawing.Size(366, 19)
        Me.percentageIndicator.TabIndex = 14
        Me.percentageIndicator.Value = 0
        '
        'txtSize
        '
        Me.txtSize.Location = New System.Drawing.Point(184, 83)
        Me.txtSize.Name = "txtSize"
        Me.txtSize.Size = New System.Drawing.Size(87, 20)
        Me.txtSize.TabIndex = 3
        '
        'lblDriveLabel
        '
        Me.lblDriveLabel.AutoSize = True
        Me.lblDriveLabel.Location = New System.Drawing.Point(3, 24)
        Me.lblDriveLabel.Name = "lblDriveLabel"
        Me.lblDriveLabel.Size = New System.Drawing.Size(64, 13)
        Me.lblDriveLabel.TabIndex = 13
        Me.lblDriveLabel.Text = "Drive Label:"
        '
        'listSizeType
        '
        Me.listSizeType.FormattingEnabled = True
        Me.listSizeType.Items.AddRange(New Object() {"Bytes", "Kilobytes", "Megabytes", "Gigabytes", "Terabytes", "% (Percentage)"})
        Me.listSizeType.Location = New System.Drawing.Point(277, 82)
        Me.listSizeType.Name = "listSizeType"
        Me.listSizeType.Size = New System.Drawing.Size(95, 21)
        Me.listSizeType.TabIndex = 4
        '
        'lblUsedShadowStorageSpace
        '
        Me.lblUsedShadowStorageSpace.AutoSize = True
        Me.lblUsedShadowStorageSpace.Location = New System.Drawing.Point(3, 40)
        Me.lblUsedShadowStorageSpace.Name = "lblUsedShadowStorageSpace"
        Me.lblUsedShadowStorageSpace.Size = New System.Drawing.Size(151, 13)
        Me.lblUsedShadowStorageSpace.TabIndex = 8
        Me.lblUsedShadowStorageSpace.Text = "Used Shadow Storage Space:"
        '
        'btnSetSize
        '
        Me.btnSetSize.Enabled = False
        Me.btnSetSize.Image = Global.Restore_Point_Creator.My.Resources.Resources.save
        Me.btnSetSize.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft
        Me.btnSetSize.Location = New System.Drawing.Point(6, 110)
        Me.btnSetSize.Name = "btnSetSize"
        Me.btnSetSize.Size = New System.Drawing.Size(366, 23)
        Me.btnSetSize.TabIndex = 5
        Me.btnSetSize.UseVisualStyleBackColor = True
        '
        'chkAdvancedMode
        '
        Me.chkAdvancedMode.AutoSize = True
        Me.chkAdvancedMode.Location = New System.Drawing.Point(236, 139)
        Me.chkAdvancedMode.Name = "chkAdvancedMode"
        Me.chkAdvancedMode.Size = New System.Drawing.Size(109, 30)
        Me.chkAdvancedMode.TabIndex = 17
        Me.chkAdvancedMode.Text = "Advanced Mode" & Global.Microsoft.VisualBasic.ChrW(13) & Global.Microsoft.VisualBasic.ChrW(10) & "(For Experts Only)"
        Me.chkAdvancedMode.UseVisualStyleBackColor = True
        '
        'frmManageSystemRestoreStorageSpace
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(96.0!, 96.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi
        Me.ClientSize = New System.Drawing.Size(463, 170)
        Me.Controls.Add(Me.chkAdvancedMode)
        Me.Controls.Add(Me.chkConfirmNewSmallerSize)
        Me.Controls.Add(Me.SplitContainer1)
        Me.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle
        Me.Icon = Global.Restore_Point_Creator.My.Resources.Resources.RestorePoint_noBackground_2
        Me.MaximizeBox = False
        Me.MinimumSize = New System.Drawing.Size(405, 209)
        Me.Name = "frmManageSystemRestoreStorageSpace"
        Me.Text = "Manage System Restore Storage Space"
        Me.SplitContainer1.Panel1.ResumeLayout(False)
        Me.SplitContainer1.Panel1.PerformLayout()
        Me.SplitContainer1.Panel2.ResumeLayout(False)
        Me.SplitContainer1.Panel2.PerformLayout()
        CType(Me.SplitContainer1, System.ComponentModel.ISupportInitialize).EndInit()
        Me.SplitContainer1.ResumeLayout(False)
        Me.ResumeLayout(False)
        Me.PerformLayout()

    End Sub

    Friend WithEvents SplitContainer1 As SplitContainer
    Friend WithEvents lblDriveSize As Label
    Friend WithEvents chkConfirmNewSmallerSize As CheckBox
    Friend WithEvents Label2 As Label
    Friend WithEvents percentageIndicator As Tom.SmoothProgressBar
    Friend WithEvents txtSize As TextBox
    Friend WithEvents lblDriveLabel As Label
    Friend WithEvents listSizeType As ComboBox
    Friend WithEvents lblUsedShadowStorageSpace As Label
    Friend WithEvents btnSetSize As Button
    Friend WithEvents chkAdvancedMode As CheckBox
    Friend WithEvents Label1 As Label
    Friend WithEvents listDrives As ListView
End Class
