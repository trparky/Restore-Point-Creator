<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Class Set_Default_Custom_Restore_Point_Name
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
        Dim resources As System.ComponentModel.ComponentResourceManager = New System.ComponentModel.ComponentResourceManager(GetType(Set_Default_Custom_Restore_Point_Name))
        Me.btnCancel = New System.Windows.Forms.Button()
        Me.btnOK = New System.Windows.Forms.Button()
        Me.txtName = New System.Windows.Forms.TextBox()
        Me.Label1 = New System.Windows.Forms.Label()
        Me.btnDelete = New System.Windows.Forms.Button()
        Me.SuspendLayout()
        '
        'btnCancel
        '
        Me.btnCancel.Image = Global.Restore_Point_Creator.My.Resources.Resources.removeSmall
        Me.btnCancel.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft
        Me.btnCancel.Location = New System.Drawing.Point(264, 68)
        Me.btnCancel.Name = "btnCancel"
        Me.btnCancel.Size = New System.Drawing.Size(84, 23)
        Me.btnCancel.TabIndex = 16
        Me.btnCancel.Text = "Cancel"
        Me.btnCancel.UseVisualStyleBackColor = True
        '
        'btnOK
        '
        Me.btnOK.Image = Global.Restore_Point_Creator.My.Resources.Resources.nextArrow
        Me.btnOK.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft
        Me.btnOK.Location = New System.Drawing.Point(264, 10)
        Me.btnOK.Name = "btnOK"
        Me.btnOK.Size = New System.Drawing.Size(84, 23)
        Me.btnOK.TabIndex = 15
        Me.btnOK.Text = "OK"
        Me.btnOK.UseVisualStyleBackColor = True
        '
        'txtName
        '
        Me.txtName.Location = New System.Drawing.Point(10, 151)
        Me.txtName.Name = "txtName"
        Me.txtName.Size = New System.Drawing.Size(332, 20)
        Me.txtName.TabIndex = 13
        '
        'Label1
        '
        Me.Label1.Location = New System.Drawing.Point(7, 10)
        Me.Label1.Name = "Label1"
        Me.Label1.Size = New System.Drawing.Size(251, 138)
        Me.Label1.TabIndex = 14
        Me.Label1.Text = resources.GetString("Label1.Text")
        '
        'btnDelete
        '
        Me.btnDelete.Image = Global.Restore_Point_Creator.My.Resources.Resources.removeSmall
        Me.btnDelete.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft
        Me.btnDelete.Location = New System.Drawing.Point(264, 39)
        Me.btnDelete.Name = "btnDelete"
        Me.btnDelete.Size = New System.Drawing.Size(84, 23)
        Me.btnDelete.TabIndex = 17
        Me.btnDelete.Text = "Delete"
        Me.btnDelete.UseVisualStyleBackColor = True
        '
        'Set_Default_Custom_Restore_Point_Name
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(6.0!, 13.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.ClientSize = New System.Drawing.Size(354, 183)
        Me.Controls.Add(Me.btnDelete)
        Me.Controls.Add(Me.btnCancel)
        Me.Controls.Add(Me.btnOK)
        Me.Controls.Add(Me.txtName)
        Me.Controls.Add(Me.Label1)
        Me.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog
        Me.MaximizeBox = False
        Me.MinimizeBox = False
        Me.Name = "Set_Default_Custom_Restore_Point_Name"
        Me.Text = "Set Default Custom Restore Point Name"
        Me.Icon = Global.Restore_Point_Creator.My.Resources.RestorePoint_noBackground_2
        Me.ResumeLayout(False)
        Me.PerformLayout()

    End Sub
    Friend WithEvents btnCancel As System.Windows.Forms.Button
    Friend WithEvents btnOK As System.Windows.Forms.Button
    Friend WithEvents txtName As System.Windows.Forms.TextBox
    Friend WithEvents Label1 As System.Windows.Forms.Label
    Friend WithEvents btnDelete As System.Windows.Forms.Button
End Class
