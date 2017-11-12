<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Class convertOldLogs
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
        Me.Label1 = New System.Windows.Forms.Label()
        Me.PictureBox1 = New System.Windows.Forms.PictureBox()
        Me.btnConvertNow = New System.Windows.Forms.Button()
        Me.btnConvertThemLater = New System.Windows.Forms.Button()
        Me.btnDontConvertThemEver = New System.Windows.Forms.Button()
        Me.TableLayoutPanel1 = New System.Windows.Forms.TableLayoutPanel()
        CType(Me.PictureBox1, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.TableLayoutPanel1.SuspendLayout()
        Me.SuspendLayout()
        '
        'Label1
        '
        Me.Label1.AutoSize = True
        Me.Label1.Location = New System.Drawing.Point(50, 12)
        Me.Label1.Name = "Label1"
        Me.Label1.Size = New System.Drawing.Size(382, 26)
        Me.Label1.TabIndex = 0
        Me.Label1.Text = "System Restore Point Creator has detected that you have not yet converted the" & Global.Microsoft.VisualBasic.ChrW(13) & Global.Microsoft.VisualBasic.ChrW(10) & "ol" &
    "d application event logs to the new format. Would you like to do that now?"
        '
        'PictureBox1
        '
        Me.PictureBox1.Location = New System.Drawing.Point(12, 12)
        Me.PictureBox1.Name = "PictureBox1"
        Me.PictureBox1.Size = New System.Drawing.Size(32, 35)
        Me.PictureBox1.TabIndex = 9
        Me.PictureBox1.TabStop = False
        '
        'btnConvertNow
        '
        Me.btnConvertNow.Anchor = CType((((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Bottom) _
            Or System.Windows.Forms.AnchorStyles.Left) _
            Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.btnConvertNow.Location = New System.Drawing.Point(3, 3)
        Me.btnConvertNow.Name = "btnConvertNow"
        Me.btnConvertNow.Size = New System.Drawing.Size(131, 23)
        Me.btnConvertNow.TabIndex = 10
        Me.btnConvertNow.Text = "Convert &Now"
        Me.btnConvertNow.UseVisualStyleBackColor = True
        '
        'btnConvertThemLater
        '
        Me.btnConvertThemLater.Anchor = CType((((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Bottom) _
            Or System.Windows.Forms.AnchorStyles.Left) _
            Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.btnConvertThemLater.Location = New System.Drawing.Point(140, 3)
        Me.btnConvertThemLater.Name = "btnConvertThemLater"
        Me.btnConvertThemLater.Size = New System.Drawing.Size(119, 23)
        Me.btnConvertThemLater.TabIndex = 11
        Me.btnConvertThemLater.Text = "Convert Them &Later"
        Me.btnConvertThemLater.UseVisualStyleBackColor = True
        '
        'btnDontConvertThemEver
        '
        Me.btnDontConvertThemEver.Anchor = CType((((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Bottom) _
            Or System.Windows.Forms.AnchorStyles.Left) _
            Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.btnDontConvertThemEver.Location = New System.Drawing.Point(265, 3)
        Me.btnDontConvertThemEver.Name = "btnDontConvertThemEver"
        Me.btnDontConvertThemEver.Size = New System.Drawing.Size(145, 23)
        Me.btnDontConvertThemEver.TabIndex = 12
        Me.btnDontConvertThemEver.Text = "&Don't Convert Them Ever"
        Me.btnDontConvertThemEver.UseVisualStyleBackColor = True
        '
        'TableLayoutPanel1
        '
        Me.TableLayoutPanel1.ColumnCount = 3
        Me.TableLayoutPanel1.ColumnStyles.Add(New System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 33.33333!))
        Me.TableLayoutPanel1.ColumnStyles.Add(New System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 30.50847!))
        Me.TableLayoutPanel1.ColumnStyles.Add(New System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 36.31961!))
        Me.TableLayoutPanel1.Controls.Add(Me.btnConvertNow, 0, 0)
        Me.TableLayoutPanel1.Controls.Add(Me.btnDontConvertThemEver, 2, 0)
        Me.TableLayoutPanel1.Controls.Add(Me.btnConvertThemLater, 1, 0)
        Me.TableLayoutPanel1.Location = New System.Drawing.Point(12, 53)
        Me.TableLayoutPanel1.Name = "TableLayoutPanel1"
        Me.TableLayoutPanel1.RowCount = 1
        Me.TableLayoutPanel1.RowStyles.Add(New System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100.0!))
        Me.TableLayoutPanel1.Size = New System.Drawing.Size(413, 29)
        Me.TableLayoutPanel1.TabIndex = 13
        '
        'convertOldLogs
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(6.0!, 13.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.ClientSize = New System.Drawing.Size(437, 90)
        Me.Controls.Add(Me.TableLayoutPanel1)
        Me.Controls.Add(Me.PictureBox1)
        Me.Controls.Add(Me.Label1)
        Me.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle
        Me.Icon = Global.Restore_Point_Creator.My.Resources.Resources.RestorePoint_noBackground_2
        Me.MaximizeBox = False
        Me.MinimizeBox = False
        Me.Name = "convertOldLogs"
        Me.Text = "Convert Old Application Logs?"
        CType(Me.PictureBox1, System.ComponentModel.ISupportInitialize).EndInit()
        Me.TableLayoutPanel1.ResumeLayout(False)
        Me.ResumeLayout(False)
        Me.PerformLayout()

    End Sub

    Friend WithEvents Label1 As Label
    Friend WithEvents PictureBox1 As PictureBox
    Friend WithEvents btnConvertNow As Button
    Friend WithEvents btnConvertThemLater As Button
    Friend WithEvents btnDontConvertThemEver As Button
    Friend WithEvents TableLayoutPanel1 As TableLayoutPanel
End Class
