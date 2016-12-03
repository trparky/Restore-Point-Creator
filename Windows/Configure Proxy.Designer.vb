<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Class Configure_Proxy
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
        Me.chkUseSystemProxy = New System.Windows.Forms.CheckBox()
        Me.chkUseProxy = New System.Windows.Forms.CheckBox()
        Me.Label1 = New System.Windows.Forms.Label()
        Me.Label2 = New System.Windows.Forms.Label()
        Me.txtProxyAddress = New System.Windows.Forms.TextBox()
        Me.txtPort = New System.Windows.Forms.TextBox()
        Me.Label3 = New System.Windows.Forms.Label()
        Me.txtUser = New System.Windows.Forms.TextBox()
        Me.txtPass = New System.Windows.Forms.TextBox()
        Me.Label4 = New System.Windows.Forms.Label()
        Me.Label5 = New System.Windows.Forms.Label()
        Me.btnSave = New System.Windows.Forms.Button()
        Me.SuspendLayout()
        '
        'chkUseSystemProxy
        '
        Me.chkUseSystemProxy.AutoSize = True
        Me.chkUseSystemProxy.Location = New System.Drawing.Point(12, 35)
        Me.chkUseSystemProxy.Name = "chkUseSystemProxy"
        Me.chkUseSystemProxy.Size = New System.Drawing.Size(257, 17)
        Me.chkUseSystemProxy.TabIndex = 0
        Me.chkUseSystemProxy.Text = "Use System Proxy Configuration (Recommended)"
        Me.chkUseSystemProxy.UseVisualStyleBackColor = True
        '
        'chkUseProxy
        '
        Me.chkUseProxy.AutoSize = True
        Me.chkUseProxy.Location = New System.Drawing.Point(12, 12)
        Me.chkUseProxy.Name = "chkUseProxy"
        Me.chkUseProxy.Size = New System.Drawing.Size(327, 17)
        Me.chkUseProxy.TabIndex = 1
        Me.chkUseProxy.Text = "Use Proxy (More often than not you will not have to use a proxy)"
        Me.chkUseProxy.UseVisualStyleBackColor = True
        '
        'Label1
        '
        Me.Label1.AutoSize = True
        Me.Label1.Location = New System.Drawing.Point(9, 70)
        Me.Label1.Name = "Label1"
        Me.Label1.Size = New System.Drawing.Size(77, 13)
        Me.Label1.TabIndex = 2
        Me.Label1.Text = "Proxy Address:"
        '
        'Label2
        '
        Me.Label2.AutoSize = True
        Me.Label2.Location = New System.Drawing.Point(275, 73)
        Me.Label2.Name = "Label2"
        Me.Label2.Size = New System.Drawing.Size(29, 13)
        Me.Label2.TabIndex = 3
        Me.Label2.Text = "Port:"
        '
        'txtProxyAddress
        '
        Me.txtProxyAddress.Location = New System.Drawing.Point(92, 70)
        Me.txtProxyAddress.Name = "txtProxyAddress"
        Me.txtProxyAddress.Size = New System.Drawing.Size(177, 20)
        Me.txtProxyAddress.TabIndex = 4
        '
        'txtPort
        '
        Me.txtPort.Location = New System.Drawing.Point(307, 70)
        Me.txtPort.Name = "txtPort"
        Me.txtPort.Size = New System.Drawing.Size(64, 20)
        Me.txtPort.TabIndex = 5
        '
        'Label3
        '
        Me.Label3.AutoSize = True
        Me.Label3.Location = New System.Drawing.Point(9, 99)
        Me.Label3.Name = "Label3"
        Me.Label3.Size = New System.Drawing.Size(87, 13)
        Me.Label3.TabIndex = 6
        Me.Label3.Text = "Proxy Username:"
        '
        'txtUser
        '
        Me.txtUser.Location = New System.Drawing.Point(99, 96)
        Me.txtUser.Name = "txtUser"
        Me.txtUser.Size = New System.Drawing.Size(170, 20)
        Me.txtUser.TabIndex = 7
        '
        'txtPass
        '
        Me.txtPass.Location = New System.Drawing.Point(100, 122)
        Me.txtPass.Name = "txtPass"
        Me.txtPass.PasswordChar = Global.Microsoft.VisualBasic.ChrW(8226)
        Me.txtPass.Size = New System.Drawing.Size(169, 20)
        Me.txtPass.TabIndex = 9
        '
        'Label4
        '
        Me.Label4.AutoSize = True
        Me.Label4.Location = New System.Drawing.Point(9, 125)
        Me.Label4.Name = "Label4"
        Me.Label4.Size = New System.Drawing.Size(85, 13)
        Me.Label4.TabIndex = 8
        Me.Label4.Text = "Proxy Password:"
        '
        'Label5
        '
        Me.Label5.AutoSize = True
        Me.Label5.Location = New System.Drawing.Point(9, 145)
        Me.Label5.Name = "Label5"
        Me.Label5.Size = New System.Drawing.Size(332, 13)
        Me.Label5.TabIndex = 10
        Me.Label5.Text = "Leave Username and Password blank if no authentication is needed."
        '
        'btnSave
        '
        Me.btnSave.Location = New System.Drawing.Point(11, 165)
        Me.btnSave.Name = "btnSave"
        Me.btnSave.Size = New System.Drawing.Size(363, 23)
        Me.btnSave.TabIndex = 11
        Me.btnSave.Text = "Save Proxy Settings"
        Me.btnSave.UseVisualStyleBackColor = True
        '
        'Configure_Proxy
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(6.0!, 13.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi
        Me.ClientSize = New System.Drawing.Size(383, 196)
        Me.Controls.Add(Me.btnSave)
        Me.Controls.Add(Me.Label5)
        Me.Controls.Add(Me.txtPass)
        Me.Controls.Add(Me.Label4)
        Me.Controls.Add(Me.txtUser)
        Me.Controls.Add(Me.Label3)
        Me.Controls.Add(Me.txtPort)
        Me.Controls.Add(Me.txtProxyAddress)
        Me.Controls.Add(Me.Label2)
        Me.Controls.Add(Me.Label1)
        Me.Controls.Add(Me.chkUseProxy)
        Me.Controls.Add(Me.chkUseSystemProxy)
        Me.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle
        Me.MaximizeBox = False
        Me.Name = "Configure_Proxy"
        Me.Text = "Configure HTTP Proxy Settings"
        Me.Icon = Global.Restore_Point_Creator.My.Resources.RestorePoint_noBackground_2
        Me.ResumeLayout(False)
        Me.PerformLayout()

    End Sub

    Friend WithEvents chkUseSystemProxy As CheckBox
    Friend WithEvents chkUseProxy As CheckBox
    Friend WithEvents Label1 As Label
    Friend WithEvents Label2 As Label
    Friend WithEvents txtProxyAddress As TextBox
    Friend WithEvents txtPort As TextBox
    Friend WithEvents Label3 As Label
    Friend WithEvents txtUser As TextBox
    Friend WithEvents txtPass As TextBox
    Friend WithEvents Label4 As Label
    Friend WithEvents Label5 As Label
    Friend WithEvents btnSave As Button
End Class
