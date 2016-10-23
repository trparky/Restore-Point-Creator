<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Class Please_Wait
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
        Me.progressBarChanger = New System.Windows.Forms.Timer(Me.components)
        Me.pleaseWaitlblLabel = New System.Windows.Forms.Label()
        Me.pleaseWaitMessageChanger = New System.Windows.Forms.Timer(Me.components)
        Me.autoWindowCloser = New System.Windows.Forms.Timer(Me.components)
        Me.SmoothProgressBar1 = New Tom.SmoothProgressBar()
        Me.SuspendLayout()
        '
        'progressBarChanger
        '
        Me.progressBarChanger.Interval = 25
        '
        'pleaseWaitlblLabel
        '
        Me.pleaseWaitlblLabel.AutoSize = True
        Me.pleaseWaitlblLabel.Location = New System.Drawing.Point(9, 9)
        Me.pleaseWaitlblLabel.Name = "pleaseWaitlblLabel"
        Me.pleaseWaitlblLabel.Size = New System.Drawing.Size(39, 13)
        Me.pleaseWaitlblLabel.TabIndex = 1
        Me.pleaseWaitlblLabel.Text = "Label1"
        '
        'pleaseWaitMessageChanger
        '
        Me.pleaseWaitMessageChanger.Interval = 250
        '
        'autoWindowCloser
        '
        Me.autoWindowCloser.Interval = 240000
        '
        'SmoothProgressBar1
        '
        Me.SmoothProgressBar1.Location = New System.Drawing.Point(12, 34)
        Me.SmoothProgressBar1.Maximum = 100
        Me.SmoothProgressBar1.Minimum = 0
        Me.SmoothProgressBar1.Name = "SmoothProgressBar1"
        Me.SmoothProgressBar1.ProgressBarColor = System.Drawing.Color.Blue
        Me.SmoothProgressBar1.Size = New System.Drawing.Size(268, 19)
        Me.SmoothProgressBar1.TabIndex = 0
        Me.SmoothProgressBar1.Value = 0
        '
        'Please_Wait
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(6.0!, 13.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.ClientSize = New System.Drawing.Size(292, 65)
        Me.Controls.Add(Me.pleaseWaitlblLabel)
        Me.Controls.Add(Me.SmoothProgressBar1)
        Me.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle
        Me.MaximizeBox = False
        Me.MinimizeBox = False
        Me.Name = "Please_Wait"
        Me.ShowInTaskbar = False
        Me.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen
        Me.Text = "Please Wait..."
        Me.ResumeLayout(False)
        Me.PerformLayout()

    End Sub
    Friend WithEvents progressBarChanger As System.Windows.Forms.Timer
    Friend WithEvents pleaseWaitlblLabel As System.Windows.Forms.Label
    Friend WithEvents pleaseWaitMessageChanger As System.Windows.Forms.Timer
    Friend WithEvents autoWindowCloser As System.Windows.Forms.Timer
    Friend WithEvents SmoothProgressBar1 As Tom.SmoothProgressBar
End Class
