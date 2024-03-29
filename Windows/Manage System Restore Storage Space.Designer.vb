﻿<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
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
        Me.listDrives = New System.Windows.Forms.ListView()
        Me.Label1 = New System.Windows.Forms.Label()
        Me.lblDriveSize = New System.Windows.Forms.Label()
        Me.Label2 = New System.Windows.Forms.Label()
        Me.percentageIndicator = New SmoothProgressBar()
        Me.txtSize = New System.Windows.Forms.TextBox()
        Me.lblDriveLabel = New System.Windows.Forms.Label()
        Me.listSizeType = New System.Windows.Forms.ComboBox()
        Me.lblUsedShadowStorageSpace = New System.Windows.Forms.Label()
        Me.btnSetSize = New System.Windows.Forms.Button()
        Me.chkConfirmNewSmallerSize = New System.Windows.Forms.CheckBox()
        Me.chkAdvancedMode = New System.Windows.Forms.CheckBox()
        Me.drivePanel = New System.Windows.Forms.Panel()
        Me.settingsPanel = New System.Windows.Forms.Panel()
        Me.drivePanel.SuspendLayout()
        Me.settingsPanel.SuspendLayout()
        Me.SuspendLayout()
        '
        'listDrives
        '
        Me.listDrives.Location = New System.Drawing.Point(4, 15)
        Me.listDrives.Name = "listDrives"
        Me.listDrives.Size = New System.Drawing.Size(64, 110)
        Me.listDrives.TabIndex = 2
        Me.listDrives.UseCompatibleStateImageBehavior = False
        Me.listDrives.View = System.Windows.Forms.View.List
        '
        'Label1
        '
        Me.Label1.AutoSize = True
        Me.Label1.Location = New System.Drawing.Point(3, 0)
        Me.Label1.Name = "Label1"
        Me.Label1.Size = New System.Drawing.Size(32, 13)
        Me.Label1.TabIndex = 0
        Me.Label1.Text = "Drive"
        '
        'lblDriveSize
        '
        Me.lblDriveSize.AutoSize = True
        Me.lblDriveSize.Location = New System.Drawing.Point(0, 0)
        Me.lblDriveSize.Name = "lblDriveSize"
        Me.lblDriveSize.Size = New System.Drawing.Size(97, 13)
        Me.lblDriveSize.TabIndex = 6
        Me.lblDriveSize.Text = "Total Size of Drive:"
        '
        'Label2
        '
        Me.Label2.AutoSize = True
        Me.Label2.Location = New System.Drawing.Point(4, 78)
        Me.Label2.Name = "Label2"
        Me.Label2.Size = New System.Drawing.Size(174, 13)
        Me.Label2.TabIndex = 2
        Me.Label2.Text = "System Restore Point Storage Size:"
        '
        'percentageIndicator
        '
        Me.percentageIndicator.Location = New System.Drawing.Point(3, 48)
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
        Me.txtSize.Location = New System.Drawing.Point(181, 75)
        Me.txtSize.Name = "txtSize"
        Me.txtSize.Size = New System.Drawing.Size(87, 20)
        Me.txtSize.TabIndex = 3
        '
        'lblDriveLabel
        '
        Me.lblDriveLabel.AutoSize = True
        Me.lblDriveLabel.Location = New System.Drawing.Point(0, 16)
        Me.lblDriveLabel.Name = "lblDriveLabel"
        Me.lblDriveLabel.Size = New System.Drawing.Size(64, 13)
        Me.lblDriveLabel.TabIndex = 13
        Me.lblDriveLabel.Text = "Drive Label:"
        '
        'listSizeType
        '
        Me.listSizeType.FormattingEnabled = True
        Me.listSizeType.Items.AddRange(New Object() {"Bytes", "Kilobytes", "Megabytes", "Gigabytes", "Terabytes", "% (Percentage)"})
        Me.listSizeType.Location = New System.Drawing.Point(274, 74)
        Me.listSizeType.Name = "listSizeType"
        Me.listSizeType.Size = New System.Drawing.Size(95, 21)
        Me.listSizeType.TabIndex = 4
        '
        'lblUsedShadowStorageSpace
        '
        Me.lblUsedShadowStorageSpace.AutoSize = True
        Me.lblUsedShadowStorageSpace.Location = New System.Drawing.Point(0, 32)
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
        Me.btnSetSize.Location = New System.Drawing.Point(3, 102)
        Me.btnSetSize.Name = "btnSetSize"
        Me.btnSetSize.Size = New System.Drawing.Size(366, 23)
        Me.btnSetSize.TabIndex = 5
        Me.btnSetSize.UseVisualStyleBackColor = True
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
        'chkAdvancedMode
        '
        Me.chkAdvancedMode.AutoSize = True
        Me.chkAdvancedMode.Location = New System.Drawing.Point(10, 158)
        Me.chkAdvancedMode.Name = "chkAdvancedMode"
        Me.chkAdvancedMode.Size = New System.Drawing.Size(191, 17)
        Me.chkAdvancedMode.TabIndex = 17
        Me.chkAdvancedMode.Text = "Advanced Mode (For Experts Only)"
        Me.chkAdvancedMode.UseVisualStyleBackColor = True
        '
        'drivePanel
        '
        Me.drivePanel.Controls.Add(Me.listDrives)
        Me.drivePanel.Controls.Add(Me.Label1)
        Me.drivePanel.Location = New System.Drawing.Point(5, 5)
        Me.drivePanel.Name = "drivePanel"
        Me.drivePanel.Size = New System.Drawing.Size(71, 128)
        Me.drivePanel.TabIndex = 18
        '
        'settingsPanel
        '
        Me.settingsPanel.Controls.Add(Me.lblDriveSize)
        Me.settingsPanel.Controls.Add(Me.Label2)
        Me.settingsPanel.Controls.Add(Me.btnSetSize)
        Me.settingsPanel.Controls.Add(Me.percentageIndicator)
        Me.settingsPanel.Controls.Add(Me.lblUsedShadowStorageSpace)
        Me.settingsPanel.Controls.Add(Me.txtSize)
        Me.settingsPanel.Controls.Add(Me.listSizeType)
        Me.settingsPanel.Controls.Add(Me.lblDriveLabel)
        Me.settingsPanel.Location = New System.Drawing.Point(75, 5)
        Me.settingsPanel.Name = "settingsPanel"
        Me.settingsPanel.Size = New System.Drawing.Size(372, 128)
        Me.settingsPanel.TabIndex = 19
        '
        'frmManageSystemRestoreStorageSpace
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(96.0!, 96.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi
        Me.ClientSize = New System.Drawing.Size(453, 178)
        Me.Controls.Add(Me.settingsPanel)
        Me.Controls.Add(Me.drivePanel)
        Me.Controls.Add(Me.chkAdvancedMode)
        Me.Controls.Add(Me.chkConfirmNewSmallerSize)
        Me.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle
        Me.Icon = Global.Restore_Point_Creator.My.Resources.Resources.RestorePoint_noBackground_2
        Me.MaximizeBox = False
        Me.Name = "frmManageSystemRestoreStorageSpace"
        Me.Text = "Manage System Restore Storage Space"
        Me.drivePanel.ResumeLayout(False)
        Me.drivePanel.PerformLayout()
        Me.settingsPanel.ResumeLayout(False)
        Me.settingsPanel.PerformLayout()
        Me.ResumeLayout(False)
        Me.PerformLayout()

    End Sub
    Friend WithEvents lblDriveSize As Label
    Friend WithEvents chkConfirmNewSmallerSize As CheckBox
    Friend WithEvents Label2 As Label
    Friend WithEvents percentageIndicator As SmoothProgressBar
    Friend WithEvents txtSize As TextBox
    Friend WithEvents lblDriveLabel As Label
    Friend WithEvents listSizeType As ComboBox
    Friend WithEvents lblUsedShadowStorageSpace As Label
    Friend WithEvents btnSetSize As Button
    Friend WithEvents chkAdvancedMode As CheckBox
    Friend WithEvents Label1 As Label
    Friend WithEvents listDrives As ListView
    Friend WithEvents drivePanel As Panel
    Friend WithEvents settingsPanel As Panel
End Class
