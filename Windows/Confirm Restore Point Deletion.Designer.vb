﻿<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Class frmConfirmDelete
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
        Me.btnYes = New System.Windows.Forms.Button()
        Me.btnNo = New System.Windows.Forms.Button()
        Me.Label1 = New System.Windows.Forms.Label()
        Me.lblRestorePointName = New System.Windows.Forms.Label()
        Me.lblCreatedOn = New System.Windows.Forms.Label()
        Me.PictureBox1 = New System.Windows.Forms.PictureBox()
        Me.btnCancelDeletion = New System.Windows.Forms.Button()
        Me.chkDontAskAgain = New System.Windows.Forms.CheckBox()
        Me.lblType = New System.Windows.Forms.Label()
        Me.ToolTip = New System.Windows.Forms.ToolTip(Me.components)
        CType(Me.PictureBox1, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.SuspendLayout()
        '
        'btnYes
        '
        Me.btnYes.Anchor = CType((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.btnYes.Font = New System.Drawing.Font("Microsoft Sans Serif", 8.25!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.btnYes.Image = Global.Restore_Point_Creator.My.Resources.Resources.yes
        Me.btnYes.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft
        Me.btnYes.Location = New System.Drawing.Point(53, 104)
        Me.btnYes.Name = "btnYes"
        Me.btnYes.Size = New System.Drawing.Size(75, 24)
        Me.btnYes.TabIndex = 0
        Me.btnYes.Text = "&Yes"
        Me.btnYes.UseVisualStyleBackColor = True
        '
        'btnNo
        '
        Me.btnNo.Anchor = CType((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.btnNo.Image = Global.Restore_Point_Creator.My.Resources.Resources.removeSmall
        Me.btnNo.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft
        Me.btnNo.Location = New System.Drawing.Point(134, 104)
        Me.btnNo.Name = "btnNo"
        Me.btnNo.Size = New System.Drawing.Size(75, 24)
        Me.btnNo.TabIndex = 1
        Me.btnNo.Text = "&No"
        Me.btnNo.UseVisualStyleBackColor = True
        '
        'Label1
        '
        Me.Label1.AutoSize = True
        Me.Label1.Location = New System.Drawing.Point(50, 88)
        Me.Label1.Name = "Label1"
        Me.Label1.Size = New System.Drawing.Size(242, 13)
        Me.Label1.TabIndex = 5
        Me.Label1.Text = "Are you sure you want to delete this restore point?"
        '
        'lblRestorePointName
        '
        Me.lblRestorePointName.AutoSize = True
        Me.lblRestorePointName.Font = New System.Drawing.Font("Microsoft Sans Serif", 8.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.lblRestorePointName.Location = New System.Drawing.Point(50, 12)
        Me.lblRestorePointName.Name = "lblRestorePointName"
        Me.lblRestorePointName.Size = New System.Drawing.Size(105, 13)
        Me.lblRestorePointName.TabIndex = 6
        Me.lblRestorePointName.Text = "Restore Point Name:"
        '
        'lblCreatedOn
        '
        Me.lblCreatedOn.AutoSize = True
        Me.lblCreatedOn.Font = New System.Drawing.Font("Microsoft Sans Serif", 8.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.lblCreatedOn.Location = New System.Drawing.Point(50, 34)
        Me.lblCreatedOn.Name = "lblCreatedOn"
        Me.lblCreatedOn.Size = New System.Drawing.Size(64, 13)
        Me.lblCreatedOn.TabIndex = 7
        Me.lblCreatedOn.Text = "Created On:"
        '
        'PictureBox1
        '
        Me.PictureBox1.Location = New System.Drawing.Point(12, 12)
        Me.PictureBox1.Name = "PictureBox1"
        Me.PictureBox1.Size = New System.Drawing.Size(32, 35)
        Me.PictureBox1.TabIndex = 8
        Me.PictureBox1.TabStop = False
        '
        'btnCancelDeletion
        '
        Me.btnCancelDeletion.Anchor = CType((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.btnCancelDeletion.Location = New System.Drawing.Point(53, 169)
        Me.btnCancelDeletion.Name = "btnCancelDeletion"
        Me.btnCancelDeletion.Size = New System.Drawing.Size(235, 23)
        Me.btnCancelDeletion.TabIndex = 9
        Me.btnCancelDeletion.Text = "&Cancel Deletion of Selected Restore Points"
        Me.ToolTip.SetToolTip(Me.btnCancelDeletion, "Clicking here will abort the operation of deleting all selected restore points. T" &
        "hink of this as saying ""No"" to delete for every selected restore point at once.")
        Me.btnCancelDeletion.UseVisualStyleBackColor = True
        '
        'chkDontAskAgain
        '
        Me.chkDontAskAgain.Anchor = CType((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.chkDontAskAgain.AutoSize = True
        Me.chkDontAskAgain.Location = New System.Drawing.Point(53, 133)
        Me.chkDontAskAgain.Name = "chkDontAskAgain"
        Me.chkDontAskAgain.Size = New System.Drawing.Size(395, 30)
        Me.chkDontAskAgain.TabIndex = 10
        Me.chkDontAskAgain.Text = "&Don't ask me again during this selected restore point deletion session" & Global.Microsoft.VisualBasic.ChrW(13) & Global.Microsoft.VisualBasic.ChrW(10) & "(The pro" &
    "gram will ask again if you select another set of restore points to delete)"
        Me.chkDontAskAgain.UseVisualStyleBackColor = True
        '
        'lblType
        '
        Me.lblType.AutoSize = True
        Me.lblType.Font = New System.Drawing.Font("Microsoft Sans Serif", 8.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.lblType.Location = New System.Drawing.Point(50, 56)
        Me.lblType.Name = "lblType"
        Me.lblType.Size = New System.Drawing.Size(34, 13)
        Me.lblType.TabIndex = 11
        Me.lblType.Text = "Type:"
        '
        'frmConfirmDelete
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(96.0!, 96.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi
        Me.ClientSize = New System.Drawing.Size(456, 199)
        Me.Controls.Add(Me.lblType)
        Me.Controls.Add(Me.chkDontAskAgain)
        Me.Controls.Add(Me.btnCancelDeletion)
        Me.Controls.Add(Me.PictureBox1)
        Me.Controls.Add(Me.lblCreatedOn)
        Me.Controls.Add(Me.lblRestorePointName)
        Me.Controls.Add(Me.btnNo)
        Me.Controls.Add(Me.Label1)
        Me.Controls.Add(Me.btnYes)
        Me.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle
        Me.Icon = Global.Restore_Point_Creator.My.Resources.Resources.RestorePoint_noBackground_2
        Me.MaximizeBox = False
        Me.MinimizeBox = False
        Me.MinimumSize = New System.Drawing.Size(472, 166)
        Me.Name = "frmConfirmDelete"
        Me.Text = "Confirm Restore Point Deletion"
        CType(Me.PictureBox1, System.ComponentModel.ISupportInitialize).EndInit()
        Me.ResumeLayout(False)
        Me.PerformLayout()

    End Sub
    Friend WithEvents btnYes As Button
    Friend WithEvents btnNo As Button
    Friend WithEvents Label1 As Label
    Friend WithEvents lblRestorePointName As Label
    Friend WithEvents lblCreatedOn As Label
    Friend WithEvents PictureBox1 As PictureBox
    Friend WithEvents btnCancelDeletion As Button
    Friend WithEvents chkDontAskAgain As CheckBox
    Friend WithEvents lblType As Label
    Friend WithEvents ToolTip As ToolTip
End Class
