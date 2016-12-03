<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Class Create_Restore_Point_at_User_Logon
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
        Dim resources As System.ComponentModel.ComponentResourceManager = New System.ComponentModel.ComponentResourceManager(GetType(Create_Restore_Point_at_User_Logon))
        Me.Label1 = New System.Windows.Forms.Label()
        Me.chkDelayed = New System.Windows.Forms.CheckBox()
        Me.txtDelayed = New System.Windows.Forms.TextBox()
        Me.lblDelayed = New System.Windows.Forms.Label()
        Me.btnCreateTask = New System.Windows.Forms.Button()
        Me.btnDeleteTask = New System.Windows.Forms.Button()
        Me.lblIndication = New System.Windows.Forms.Label()
        Me.SuspendLayout()
        '
        'Label1
        '
        Me.Label1.AutoSize = True
        Me.Label1.Location = New System.Drawing.Point(12, 9)
        Me.Label1.Name = "Label1"
        Me.Label1.Size = New System.Drawing.Size(426, 91)
        Me.Label1.TabIndex = 0
        Me.Label1.Text = resources.GetString("Label1.Text")
        '
        'chkDelayed
        '
        Me.chkDelayed.AutoSize = True
        Me.chkDelayed.Location = New System.Drawing.Point(15, 145)
        Me.chkDelayed.Name = "chkDelayed"
        Me.chkDelayed.Size = New System.Drawing.Size(178, 17)
        Me.chkDelayed.TabIndex = 1
        Me.chkDelayed.Text = "Delayed by X number of minutes"
        Me.chkDelayed.UseVisualStyleBackColor = True
        '
        'txtDelayed
        '
        Me.txtDelayed.Location = New System.Drawing.Point(316, 143)
        Me.txtDelayed.Name = "txtDelayed"
        Me.txtDelayed.Size = New System.Drawing.Size(46, 20)
        Me.txtDelayed.TabIndex = 2
        Me.txtDelayed.Visible = False
        '
        'lblDelayed
        '
        Me.lblDelayed.AutoSize = True
        Me.lblDelayed.Location = New System.Drawing.Point(208, 146)
        Me.lblDelayed.Name = "lblDelayed"
        Me.lblDelayed.Size = New System.Drawing.Size(102, 13)
        Me.lblDelayed.TabIndex = 3
        Me.lblDelayed.Text = "How many minutes?"
        Me.lblDelayed.Visible = False
        '
        'btnCreateTask
        '
        Me.btnCreateTask.Location = New System.Drawing.Point(9, 167)
        Me.btnCreateTask.Name = "btnCreateTask"
        Me.btnCreateTask.Size = New System.Drawing.Size(207, 26)
        Me.btnCreateTask.TabIndex = 4
        Me.btnCreateTask.Text = "Make Restore Points at User Logon"
        Me.btnCreateTask.UseVisualStyleBackColor = True
        '
        'btnDeleteTask
        '
        Me.btnDeleteTask.Location = New System.Drawing.Point(222, 167)
        Me.btnDeleteTask.Name = "btnDeleteTask"
        Me.btnDeleteTask.Size = New System.Drawing.Size(207, 26)
        Me.btnDeleteTask.TabIndex = 5
        Me.btnDeleteTask.Text = "Stop Making Restore Points"
        Me.btnDeleteTask.UseVisualStyleBackColor = True
        '
        'lblIndication
        '
        Me.lblIndication.AutoSize = True
        Me.lblIndication.Location = New System.Drawing.Point(12, 111)
        Me.lblIndication.Name = "lblIndication"
        Me.lblIndication.Size = New System.Drawing.Size(174, 13)
        Me.lblIndication.TabIndex = 6
        Me.lblIndication.Text = "Create Restore Point at User Login:"
        '
        'Create_Restore_Point_at_User_Logon
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(96.0!, 96.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi
        Me.ClientSize = New System.Drawing.Size(441, 205)
        Me.Controls.Add(Me.lblIndication)
        Me.Controls.Add(Me.btnDeleteTask)
        Me.Controls.Add(Me.btnCreateTask)
        Me.Controls.Add(Me.lblDelayed)
        Me.Controls.Add(Me.txtDelayed)
        Me.Controls.Add(Me.chkDelayed)
        Me.Controls.Add(Me.Label1)
        Me.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog
        Me.Icon = Global.Restore_Point_Creator.My.Resources.Resources.RestorePoint_noBackground_2
        Me.MaximizeBox = False
        Me.MinimizeBox = False
        Me.Name = "Create_Restore_Point_at_User_Logon"
        Me.Text = "Create Restore Point at User Logon"
        Me.ResumeLayout(False)
        Me.PerformLayout()

    End Sub
    Friend WithEvents Label1 As System.Windows.Forms.Label
    Friend WithEvents chkDelayed As System.Windows.Forms.CheckBox
    Friend WithEvents txtDelayed As System.Windows.Forms.TextBox
    Friend WithEvents lblDelayed As System.Windows.Forms.Label
    Friend WithEvents btnCreateTask As System.Windows.Forms.Button
    Friend WithEvents btnDeleteTask As System.Windows.Forms.Button
    Friend WithEvents lblIndication As System.Windows.Forms.Label
End Class
