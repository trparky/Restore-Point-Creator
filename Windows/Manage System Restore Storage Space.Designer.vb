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
        Me.Label2 = New System.Windows.Forms.Label()
        Me.txtSize = New System.Windows.Forms.TextBox()
        Me.listSizeType = New System.Windows.Forms.ComboBox()
        Me.btnSetSize = New System.Windows.Forms.Button()
        Me.lblDriveSize = New System.Windows.Forms.Label()
        Me.lblUsedShadowStorageSpace = New System.Windows.Forms.Label()
        Me.lblDriveLabel = New System.Windows.Forms.Label()
        Me.percentageIndicator = New Tom.SmoothProgressBar()
        Me.SuspendLayout()
        '
        'Label2
        '
        Me.Label2.AutoSize = True
        Me.Label2.Location = New System.Drawing.Point(13, 88)
        Me.Label2.Name = "Label2"
        Me.Label2.Size = New System.Drawing.Size(174, 13)
        Me.Label2.TabIndex = 2
        Me.Label2.Text = "System Restore Point Storage Size:"
        '
        'txtSize
        '
        Me.txtSize.Location = New System.Drawing.Point(190, 85)
        Me.txtSize.Name = "txtSize"
        Me.txtSize.Size = New System.Drawing.Size(87, 20)
        Me.txtSize.TabIndex = 3
        '
        'listSizeType
        '
        Me.listSizeType.FormattingEnabled = True
        Me.listSizeType.Items.AddRange(New Object() {"Bytes", "Kilobytes", "Megabytes", "Gigabytes", "Terabytes", "% (Percentage)"})
        Me.listSizeType.Location = New System.Drawing.Point(283, 84)
        Me.listSizeType.Name = "listSizeType"
        Me.listSizeType.Size = New System.Drawing.Size(95, 21)
        Me.listSizeType.TabIndex = 4
        '
        'btnSetSize
        '
        Me.btnSetSize.Enabled = False
        Me.btnSetSize.Image = Global.Restore_Point_Creator.My.Resources.Resources.save
        Me.btnSetSize.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft
        Me.btnSetSize.Location = New System.Drawing.Point(12, 112)
        Me.btnSetSize.Name = "btnSetSize"
        Me.btnSetSize.Size = New System.Drawing.Size(366, 23)
        Me.btnSetSize.TabIndex = 5
        Me.btnSetSize.UseVisualStyleBackColor = True
        '
        'lblDriveSize
        '
        Me.lblDriveSize.AutoSize = True
        Me.lblDriveSize.Location = New System.Drawing.Point(9, 10)
        Me.lblDriveSize.Name = "lblDriveSize"
        Me.lblDriveSize.Size = New System.Drawing.Size(97, 13)
        Me.lblDriveSize.TabIndex = 6
        Me.lblDriveSize.Text = "Total Size of Drive:"
        '
        'lblUsedShadowStorageSpace
        '
        Me.lblUsedShadowStorageSpace.AutoSize = True
        Me.lblUsedShadowStorageSpace.Location = New System.Drawing.Point(9, 42)
        Me.lblUsedShadowStorageSpace.Name = "lblUsedShadowStorageSpace"
        Me.lblUsedShadowStorageSpace.Size = New System.Drawing.Size(151, 13)
        Me.lblUsedShadowStorageSpace.TabIndex = 8
        Me.lblUsedShadowStorageSpace.Text = "Used Shadow Storage Space:"
        '
        'lblDriveLabel
        '
        Me.lblDriveLabel.AutoSize = True
        Me.lblDriveLabel.Location = New System.Drawing.Point(9, 26)
        Me.lblDriveLabel.Name = "lblDriveLabel"
        Me.lblDriveLabel.Size = New System.Drawing.Size(64, 13)
        Me.lblDriveLabel.TabIndex = 13
        Me.lblDriveLabel.Text = "Drive Label:"
        '
        'percentageIndicator
        '
        Me.percentageIndicator.Location = New System.Drawing.Point(12, 58)
        Me.percentageIndicator.Maximum = 100
        Me.percentageIndicator.Minimum = 0
        Me.percentageIndicator.Name = "percentageIndicator"
        Me.percentageIndicator.ProgressBarColor = System.Drawing.Color.Blue
        Me.percentageIndicator.Size = New System.Drawing.Size(366, 19)
        Me.percentageIndicator.TabIndex = 14
        Me.percentageIndicator.Value = 0
        '
        'frmManageSystemRestoreStorageSpace
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(6.0!, 13.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi
        Me.ClientSize = New System.Drawing.Size(385, 145)
        Me.Controls.Add(Me.percentageIndicator)
        Me.Controls.Add(Me.lblDriveLabel)
        Me.Controls.Add(Me.lblUsedShadowStorageSpace)
        Me.Controls.Add(Me.lblDriveSize)
        Me.Controls.Add(Me.btnSetSize)
        Me.Controls.Add(Me.listSizeType)
        Me.Controls.Add(Me.txtSize)
        Me.Controls.Add(Me.Label2)
        Me.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle
        Me.Icon = Global.Restore_Point_Creator.My.Resources.Resources.RestorePoint_noBackground_2
        Me.MaximizeBox = False
        Me.Name = "frmManageSystemRestoreStorageSpace"
        Me.Text = "Manage System Restore Storage Space"
        Me.ResumeLayout(False)
        Me.PerformLayout()

    End Sub
    Friend WithEvents Label2 As System.Windows.Forms.Label
    Friend WithEvents txtSize As System.Windows.Forms.TextBox
    Friend WithEvents listSizeType As System.Windows.Forms.ComboBox
    Friend WithEvents btnSetSize As System.Windows.Forms.Button
    Friend WithEvents lblDriveSize As System.Windows.Forms.Label
    Friend WithEvents lblUsedShadowStorageSpace As System.Windows.Forms.Label
    Friend WithEvents lblDriveLabel As System.Windows.Forms.Label
    Friend WithEvents percentageIndicator As Tom.SmoothProgressBar
End Class
