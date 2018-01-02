<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Class Disk_Space_Usage
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
        Me.ToolTip = New System.Windows.Forms.ToolTip(Me.components)
        Me.btnRefresh = New System.Windows.Forms.Button()
        Me.GroupBox1 = New System.Windows.Forms.Panel()
        Me.btnManageSystemRestoreStorageSize = New System.Windows.Forms.Button()
        Me.ColorDialog = New System.Windows.Forms.ColorDialog()
        Me.btnSetBarColor = New System.Windows.Forms.Button()
        Me.chkShowFullDisksAsRed = New System.Windows.Forms.CheckBox()
        Me.pleaseWaitPanel = New System.Windows.Forms.Panel()
        Me.pleaseWaitlblLabel = New System.Windows.Forms.Label()
        Me.pleaseWaitProgressBar = New SmoothProgressBar()
        Me.pleaseWaitBorderText = New System.Windows.Forms.Label()
        Me.pleaseWaitProgressBarChanger = New System.Windows.Forms.Timer(Me.components)
        Me.pleaseWaitMessageChanger = New System.Windows.Forms.Timer(Me.components)
        Me.chkShowSystemRestoreSpaceForAllDrives = New System.Windows.Forms.CheckBox()
        Me.pleaseWaitPanel.SuspendLayout()
        Me.SuspendLayout()
        '
        'btnRefresh
        '
        Me.btnRefresh.Image = Global.Restore_Point_Creator.My.Resources.Resources.refresh
        Me.btnRefresh.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft
        Me.btnRefresh.Location = New System.Drawing.Point(12, 12)
        Me.btnRefresh.Name = "btnRefresh"
        Me.btnRefresh.Size = New System.Drawing.Size(82, 23)
        Me.btnRefresh.TabIndex = 0
        Me.btnRefresh.Text = "Refresh"
        Me.btnRefresh.UseVisualStyleBackColor = True
        '
        'GroupBox1
        '
        Me.GroupBox1.Anchor = CType((((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Bottom) _
            Or System.Windows.Forms.AnchorStyles.Left) _
            Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.GroupBox1.AutoScroll = True
        Me.GroupBox1.Location = New System.Drawing.Point(2, 41)
        Me.GroupBox1.Name = "GroupBox1"
        Me.GroupBox1.Size = New System.Drawing.Size(830, 140)
        Me.GroupBox1.TabIndex = 1
        '
        'btnManageSystemRestoreStorageSize
        '
        Me.btnManageSystemRestoreStorageSize.Image = Global.Restore_Point_Creator.My.Resources.Resources.settings
        Me.btnManageSystemRestoreStorageSize.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft
        Me.btnManageSystemRestoreStorageSize.Location = New System.Drawing.Point(100, 12)
        Me.btnManageSystemRestoreStorageSize.Name = "btnManageSystemRestoreStorageSize"
        Me.btnManageSystemRestoreStorageSize.Size = New System.Drawing.Size(241, 23)
        Me.btnManageSystemRestoreStorageSize.TabIndex = 30
        Me.btnManageSystemRestoreStorageSize.Text = "Manage System Restore Storage Space"
        Me.btnManageSystemRestoreStorageSize.UseVisualStyleBackColor = True
        '
        'btnSetBarColor
        '
        Me.btnSetBarColor.Image = Global.Restore_Point_Creator.My.Resources.Resources.color_wheel
        Me.btnSetBarColor.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft
        Me.btnSetBarColor.Location = New System.Drawing.Point(347, 12)
        Me.btnSetBarColor.Name = "btnSetBarColor"
        Me.btnSetBarColor.Size = New System.Drawing.Size(114, 23)
        Me.btnSetBarColor.TabIndex = 31
        Me.btnSetBarColor.Text = "Set Bar Color"
        Me.btnSetBarColor.UseVisualStyleBackColor = True
        '
        'chkShowFullDisksAsRed
        '
        Me.chkShowFullDisksAsRed.AutoSize = True
        Me.chkShowFullDisksAsRed.Location = New System.Drawing.Point(467, 16)
        Me.chkShowFullDisksAsRed.Name = "chkShowFullDisksAsRed"
        Me.chkShowFullDisksAsRed.Size = New System.Drawing.Size(206, 17)
        Me.chkShowFullDisksAsRed.TabIndex = 32
        Me.chkShowFullDisksAsRed.Text = "Show disks with near-full usage as red"
        Me.chkShowFullDisksAsRed.UseVisualStyleBackColor = True
        '
        'pleaseWaitPanel
        '
        Me.pleaseWaitPanel.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle
        Me.pleaseWaitPanel.Controls.Add(Me.pleaseWaitlblLabel)
        Me.pleaseWaitPanel.Controls.Add(Me.pleaseWaitProgressBar)
        Me.pleaseWaitPanel.Controls.Add(Me.pleaseWaitBorderText)
        Me.pleaseWaitPanel.Location = New System.Drawing.Point(275, 63)
        Me.pleaseWaitPanel.Name = "pleaseWaitPanel"
        Me.pleaseWaitPanel.Size = New System.Drawing.Size(293, 86)
        Me.pleaseWaitPanel.TabIndex = 43
        Me.pleaseWaitPanel.Visible = False
        '
        'pleaseWaitlblLabel
        '
        Me.pleaseWaitlblLabel.AutoSize = True
        Me.pleaseWaitlblLabel.Location = New System.Drawing.Point(3, 32)
        Me.pleaseWaitlblLabel.Name = "pleaseWaitlblLabel"
        Me.pleaseWaitlblLabel.Size = New System.Drawing.Size(39, 13)
        Me.pleaseWaitlblLabel.TabIndex = 7
        Me.pleaseWaitlblLabel.Text = "Label1"
        '
        'pleaseWaitProgressBar
        '
        Me.pleaseWaitProgressBar.Location = New System.Drawing.Point(6, 57)
        Me.pleaseWaitProgressBar.Maximum = 100
        Me.pleaseWaitProgressBar.Minimum = 0
        Me.pleaseWaitProgressBar.Name = "pleaseWaitProgressBar"
        Me.pleaseWaitProgressBar.ProgressBarColor = System.Drawing.Color.Blue
        Me.pleaseWaitProgressBar.Size = New System.Drawing.Size(268, 19)
        Me.pleaseWaitProgressBar.TabIndex = 6
        Me.pleaseWaitProgressBar.Value = 0
        '
        'pleaseWaitBorderText
        '
        Me.pleaseWaitBorderText.BackColor = System.Drawing.Color.SkyBlue
        Me.pleaseWaitBorderText.Location = New System.Drawing.Point(0, 0)
        Me.pleaseWaitBorderText.Name = "pleaseWaitBorderText"
        Me.pleaseWaitBorderText.Padding = New System.Windows.Forms.Padding(3, 0, 0, 0)
        Me.pleaseWaitBorderText.Size = New System.Drawing.Size(292, 23)
        Me.pleaseWaitBorderText.TabIndex = 5
        Me.pleaseWaitBorderText.Text = "Please Wait..."
        Me.pleaseWaitBorderText.TextAlign = System.Drawing.ContentAlignment.MiddleLeft
        '
        'pleaseWaitProgressBarChanger
        '
        Me.pleaseWaitProgressBarChanger.Interval = 25
        '
        'pleaseWaitMessageChanger
        '
        Me.pleaseWaitMessageChanger.Interval = 250
        '
        'chkShowSystemRestoreSpaceForAllDrives
        '
        Me.chkShowSystemRestoreSpaceForAllDrives.AutoSize = True
        Me.chkShowSystemRestoreSpaceForAllDrives.Location = New System.Drawing.Point(679, 9)
        Me.chkShowSystemRestoreSpaceForAllDrives.Name = "chkShowSystemRestoreSpaceForAllDrives"
        Me.chkShowSystemRestoreSpaceForAllDrives.Size = New System.Drawing.Size(130, 30)
        Me.chkShowSystemRestoreSpaceForAllDrives.TabIndex = 44
        Me.chkShowSystemRestoreSpaceForAllDrives.Text = "Show System Restore" & Global.Microsoft.VisualBasic.ChrW(13) & Global.Microsoft.VisualBasic.ChrW(10) & "Space for all drives"
        Me.chkShowSystemRestoreSpaceForAllDrives.UseVisualStyleBackColor = True
        '
        'Disk_Space_Usage
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(96.0!, 96.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi
        Me.ClientSize = New System.Drawing.Size(843, 191)
        Me.Controls.Add(Me.chkShowSystemRestoreSpaceForAllDrives)
        Me.Controls.Add(Me.pleaseWaitPanel)
        Me.Controls.Add(Me.chkShowFullDisksAsRed)
        Me.Controls.Add(Me.btnSetBarColor)
        Me.Controls.Add(Me.btnManageSystemRestoreStorageSize)
        Me.Controls.Add(Me.GroupBox1)
        Me.Controls.Add(Me.btnRefresh)
        Me.Icon = Global.Restore_Point_Creator.My.Resources.Resources.RestorePoint_noBackground_2
        Me.KeyPreview = True
        Me.MaximizeBox = False
        Me.MinimumSize = New System.Drawing.Size(859, 229)
        Me.Name = "Disk_Space_Usage"
        Me.Text = "Disk Space Usage"
        Me.pleaseWaitPanel.ResumeLayout(False)
        Me.pleaseWaitPanel.PerformLayout()
        Me.ResumeLayout(False)
        Me.PerformLayout()

    End Sub
    Friend WithEvents ToolTip As System.Windows.Forms.ToolTip
    Friend WithEvents btnRefresh As System.Windows.Forms.Button
    Friend WithEvents GroupBox1 As System.Windows.Forms.Panel
    Friend WithEvents btnManageSystemRestoreStorageSize As System.Windows.Forms.Button
    Friend WithEvents ColorDialog As System.Windows.Forms.ColorDialog
    Friend WithEvents btnSetBarColor As System.Windows.Forms.Button
    Friend WithEvents chkShowFullDisksAsRed As CheckBox
    Friend WithEvents pleaseWaitPanel As Panel
    Friend WithEvents pleaseWaitProgressBarChanger As Timer
    Friend WithEvents pleaseWaitBorderText As Label
    Friend WithEvents pleaseWaitlblLabel As Label
    Friend WithEvents pleaseWaitProgressBar As SmoothProgressBar
    Friend WithEvents pleaseWaitMessageChanger As Timer
    Friend WithEvents chkShowSystemRestoreSpaceForAllDrives As CheckBox
End Class
