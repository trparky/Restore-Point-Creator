<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Class Configure_HTTP_Timeout
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
        Me.timeout = New System.Windows.Forms.NumericUpDown()
        Me.Label1 = New System.Windows.Forms.Label()
        Me.btnSave = New System.Windows.Forms.Button()
        CType(Me.timeout, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.SuspendLayout()
        '
        'timeout
        '
        Me.timeout.Location = New System.Drawing.Point(151, 10)
        Me.timeout.Maximum = New Decimal(New Integer() {120, 0, 0, 0})
        Me.timeout.Name = "timeout"
        Me.timeout.Size = New System.Drawing.Size(120, 20)
        Me.timeout.TabIndex = 0
        '
        'Label1
        '
        Me.Label1.AutoSize = True
        Me.Label1.Location = New System.Drawing.Point(12, 12)
        Me.Label1.Name = "Label1"
        Me.Label1.Size = New System.Drawing.Size(133, 13)
        Me.Label1.TabIndex = 1
        Me.Label1.Text = "HTTP Timeout in Seconds"
        '
        'btnSave
        '
        Me.btnSave.Image = Global.Restore_Point_Creator.My.Resources.Resources.save
        Me.btnSave.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft
        Me.btnSave.Location = New System.Drawing.Point(12, 36)
        Me.btnSave.Name = "btnSave"
        Me.btnSave.Size = New System.Drawing.Size(259, 23)
        Me.btnSave.TabIndex = 2
        Me.btnSave.Text = "Save"
        Me.btnSave.UseVisualStyleBackColor = True
        '
        'Configure_HTTP_Timeout
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(6.0!, 13.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.ClientSize = New System.Drawing.Size(283, 69)
        Me.Controls.Add(Me.btnSave)
        Me.Controls.Add(Me.Label1)
        Me.Controls.Add(Me.timeout)
        Me.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle
        Me.Icon = Global.Restore_Point_Creator.My.Resources.Resources.RestorePoint_noBackground_2
        Me.MaximizeBox = False
        Me.MinimizeBox = False
        Me.Name = "Configure_HTTP_Timeout"
        Me.Text = "Configure HTTP Timeout"
        CType(Me.timeout, System.ComponentModel.ISupportInitialize).EndInit()
        Me.ResumeLayout(False)
        Me.PerformLayout()

    End Sub

    Friend WithEvents timeout As NumericUpDown
    Friend WithEvents Label1 As Label
    Friend WithEvents btnSave As Button
End Class
