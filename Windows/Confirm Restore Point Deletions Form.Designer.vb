<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()>
Partial Class Confirm_Restore_Point_Deletions_Form
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
        Dim resources As System.ComponentModel.ComponentResourceManager = New System.ComponentModel.ComponentResourceManager(GetType(Confirm_Restore_Point_Deletions_Form))
        Me.Label1 = New System.Windows.Forms.Label()
        Me.restorePointGroup = New System.Windows.Forms.Panel()
        Me.btnCancel = New System.Windows.Forms.Button()
        Me.btnConfirm = New System.Windows.Forms.Button()
        Me.btnCheckAll = New System.Windows.Forms.Button()
        Me.btnUncheckAll = New System.Windows.Forms.Button()
        Me.SuspendLayout()
        '
        'Label1
        '
        Me.Label1.AutoSize = True
        Me.Label1.Location = New System.Drawing.Point(12, 9)
        Me.Label1.Name = "Label1"
        Me.Label1.Size = New System.Drawing.Size(614, 26)
        Me.Label1.TabIndex = 0
        Me.Label1.Text = resources.GetString("Label1.Text")
        '
        'restorePointGroup
        '
        Me.restorePointGroup.Anchor = CType((((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Bottom) _
            Or System.Windows.Forms.AnchorStyles.Left) _
            Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.restorePointGroup.AutoScroll = True
        Me.restorePointGroup.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle
        Me.restorePointGroup.Location = New System.Drawing.Point(15, 38)
        Me.restorePointGroup.Name = "restorePointGroup"
        Me.restorePointGroup.Size = New System.Drawing.Size(696, 161)
        Me.restorePointGroup.TabIndex = 1
        '
        'btnCancel
        '
        Me.btnCancel.Anchor = CType((System.Windows.Forms.AnchorStyles.Bottom Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.btnCancel.Location = New System.Drawing.Point(338, 204)
        Me.btnCancel.Name = "btnCancel"
        Me.btnCancel.Size = New System.Drawing.Size(247, 23)
        Me.btnCancel.TabIndex = 2
        Me.btnCancel.Text = "&Cancel Deletion of All Selected Restore Points"
        Me.btnCancel.UseVisualStyleBackColor = True
        '
        'btnConfirm
        '
        Me.btnConfirm.Anchor = CType((System.Windows.Forms.AnchorStyles.Bottom Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.btnConfirm.Location = New System.Drawing.Point(591, 204)
        Me.btnConfirm.Name = "btnConfirm"
        Me.btnConfirm.Size = New System.Drawing.Size(120, 23)
        Me.btnConfirm.TabIndex = 3
        Me.btnConfirm.Text = "Confirm &Deletions"
        Me.btnConfirm.UseVisualStyleBackColor = True
        '
        'btnCheckAll
        '
        Me.btnCheckAll.Anchor = CType((System.Windows.Forms.AnchorStyles.Bottom Or System.Windows.Forms.AnchorStyles.Left), System.Windows.Forms.AnchorStyles)
        Me.btnCheckAll.Image = Global.Restore_Point_Creator.My.Resources.Resources.checkbox_yes
        Me.btnCheckAll.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft
        Me.btnCheckAll.Location = New System.Drawing.Point(15, 205)
        Me.btnCheckAll.Name = "btnCheckAll"
        Me.btnCheckAll.Size = New System.Drawing.Size(75, 23)
        Me.btnCheckAll.TabIndex = 4
        Me.btnCheckAll.Text = "Check All"
        Me.btnCheckAll.TextAlign = System.Drawing.ContentAlignment.MiddleRight
        Me.btnCheckAll.UseVisualStyleBackColor = True
        '
        'btnUncheckAll
        '
        Me.btnUncheckAll.Anchor = CType((System.Windows.Forms.AnchorStyles.Bottom Or System.Windows.Forms.AnchorStyles.Left), System.Windows.Forms.AnchorStyles)
        Me.btnUncheckAll.Image = Global.Restore_Point_Creator.My.Resources.Resources.checkbox_no
        Me.btnUncheckAll.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft
        Me.btnUncheckAll.Location = New System.Drawing.Point(96, 205)
        Me.btnUncheckAll.Name = "btnUncheckAll"
        Me.btnUncheckAll.Size = New System.Drawing.Size(87, 23)
        Me.btnUncheckAll.TabIndex = 5
        Me.btnUncheckAll.Text = "Uncheck All"
        Me.btnUncheckAll.TextAlign = System.Drawing.ContentAlignment.MiddleRight
        Me.btnUncheckAll.UseVisualStyleBackColor = True
        '
        'Confirm_Restore_Point_Deletions_Form
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(6.0!, 13.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.ClientSize = New System.Drawing.Size(723, 234)
        Me.Controls.Add(Me.btnUncheckAll)
        Me.Controls.Add(Me.btnCheckAll)
        Me.Controls.Add(Me.btnConfirm)
        Me.Controls.Add(Me.btnCancel)
        Me.Controls.Add(Me.restorePointGroup)
        Me.Controls.Add(Me.Label1)
        Me.Icon = Global.Restore_Point_Creator.My.Resources.Resources.RestorePoint_noBackground_2
        Me.MaximizeBox = False
        Me.MinimizeBox = False
        Me.MinimumSize = New System.Drawing.Size(739, 273)
        Me.Name = "Confirm_Restore_Point_Deletions_Form"
        Me.Text = "Confirm Restore Point Deletions"
        Me.ResumeLayout(False)
        Me.PerformLayout()

    End Sub

    Friend WithEvents Label1 As Label
    Friend WithEvents restorePointGroup As Panel
    Friend WithEvents btnCancel As Button
    Friend WithEvents btnConfirm As Button
    Friend WithEvents btnCheckAll As Button
    Friend WithEvents btnUncheckAll As Button
End Class
