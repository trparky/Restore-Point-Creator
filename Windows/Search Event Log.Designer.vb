<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()>
Partial Class Search_Event_Log
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
        Me.btnSearch = New System.Windows.Forms.Button()
        Me.txtSearchTerms = New System.Windows.Forms.TextBox()
        Me.Label1 = New System.Windows.Forms.Label()
        Me.chkCaseInsensitive = New System.Windows.Forms.CheckBox()
        Me.chkRegEx = New System.Windows.Forms.CheckBox()
        Me.btnClear = New System.Windows.Forms.Button()
        Me.radAny = New System.Windows.Forms.RadioButton()
        Me.radInfo = New System.Windows.Forms.RadioButton()
        Me.Label2 = New System.Windows.Forms.Label()
        Me.radError = New System.Windows.Forms.RadioButton()
        Me.SuspendLayout()
        '
        'btnSearch
        '
        Me.btnSearch.Enabled = False
        Me.btnSearch.Image = Global.Restore_Point_Creator.My.Resources.Resources.view
        Me.btnSearch.Location = New System.Drawing.Point(333, 3)
        Me.btnSearch.Name = "btnSearch"
        Me.btnSearch.Size = New System.Drawing.Size(26, 23)
        Me.btnSearch.TabIndex = 1
        Me.btnSearch.UseVisualStyleBackColor = True
        '
        'txtSearchTerms
        '
        Me.txtSearchTerms.Location = New System.Drawing.Point(94, 6)
        Me.txtSearchTerms.Name = "txtSearchTerms"
        Me.txtSearchTerms.Size = New System.Drawing.Size(233, 20)
        Me.txtSearchTerms.TabIndex = 0
        '
        'Label1
        '
        Me.Label1.AutoSize = True
        Me.Label1.Location = New System.Drawing.Point(12, 9)
        Me.Label1.Name = "Label1"
        Me.Label1.Size = New System.Drawing.Size(76, 13)
        Me.Label1.TabIndex = 2
        Me.Label1.Text = "Search Terms:"
        '
        'chkCaseInsensitive
        '
        Me.chkCaseInsensitive.AutoSize = True
        Me.chkCaseInsensitive.Checked = True
        Me.chkCaseInsensitive.CheckState = System.Windows.Forms.CheckState.Checked
        Me.chkCaseInsensitive.Location = New System.Drawing.Point(12, 32)
        Me.chkCaseInsensitive.Name = "chkCaseInsensitive"
        Me.chkCaseInsensitive.Size = New System.Drawing.Size(140, 17)
        Me.chkCaseInsensitive.TabIndex = 2
        Me.chkCaseInsensitive.Text = "Case Insensitive Search"
        Me.chkCaseInsensitive.UseVisualStyleBackColor = True
        '
        'chkRegEx
        '
        Me.chkRegEx.AutoSize = True
        Me.chkRegEx.Location = New System.Drawing.Point(158, 32)
        Me.chkRegEx.Name = "chkRegEx"
        Me.chkRegEx.Size = New System.Drawing.Size(240, 17)
        Me.chkRegEx.TabIndex = 3
        Me.chkRegEx.Text = "Use Regular Expressions (Power Users Only!)"
        Me.chkRegEx.UseVisualStyleBackColor = True
        '
        'btnClear
        '
        Me.btnClear.Enabled = False
        Me.btnClear.Image = Global.Restore_Point_Creator.My.Resources.Resources.edit_clear
        Me.btnClear.Location = New System.Drawing.Point(365, 3)
        Me.btnClear.Name = "btnClear"
        Me.btnClear.Size = New System.Drawing.Size(26, 23)
        Me.btnClear.TabIndex = 4
        Me.btnClear.UseVisualStyleBackColor = True
        '
        'radAny
        '
        Me.radAny.AutoSize = True
        Me.radAny.Checked = True
        Me.radAny.Location = New System.Drawing.Point(71, 54)
        Me.radAny.Name = "radAny"
        Me.radAny.Size = New System.Drawing.Size(43, 17)
        Me.radAny.TabIndex = 5
        Me.radAny.TabStop = True
        Me.radAny.Text = "Any"
        Me.radAny.UseVisualStyleBackColor = True
        '
        'radInfo
        '
        Me.radInfo.AutoSize = True
        Me.radInfo.Location = New System.Drawing.Point(120, 54)
        Me.radInfo.Name = "radInfo"
        Me.radInfo.Size = New System.Drawing.Size(77, 17)
        Me.radInfo.TabIndex = 6
        Me.radInfo.Text = "Information"
        Me.radInfo.UseVisualStyleBackColor = True
        '
        'Label2
        '
        Me.Label2.AutoSize = True
        Me.Label2.Location = New System.Drawing.Point(12, 56)
        Me.Label2.Name = "Label2"
        Me.Label2.Size = New System.Drawing.Size(52, 13)
        Me.Label2.TabIndex = 7
        Me.Label2.Text = "Log Type"
        '
        'radError
        '
        Me.radError.AutoSize = True
        Me.radError.Location = New System.Drawing.Point(203, 54)
        Me.radError.Name = "radError"
        Me.radError.Size = New System.Drawing.Size(47, 17)
        Me.radError.TabIndex = 8
        Me.radError.Text = "Error"
        Me.radError.UseVisualStyleBackColor = True
        '
        'Search_Event_Log
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(6.0!, 13.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.ClientSize = New System.Drawing.Size(399, 75)
        Me.Controls.Add(Me.radError)
        Me.Controls.Add(Me.Label2)
        Me.Controls.Add(Me.radInfo)
        Me.Controls.Add(Me.radAny)
        Me.Controls.Add(Me.btnClear)
        Me.Controls.Add(Me.chkRegEx)
        Me.Controls.Add(Me.chkCaseInsensitive)
        Me.Controls.Add(Me.Label1)
        Me.Controls.Add(Me.txtSearchTerms)
        Me.Controls.Add(Me.btnSearch)
        Me.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle
        Me.Icon = Global.Restore_Point_Creator.My.Resources.Resources.RestorePoint_noBackground_2
        Me.KeyPreview = True
        Me.MaximizeBox = False
        Me.MinimizeBox = False
        Me.Name = "Search_Event_Log"
        Me.Text = "Search Event Log"
        Me.ResumeLayout(False)
        Me.PerformLayout()

    End Sub

    Friend WithEvents btnSearch As Button
    Friend WithEvents txtSearchTerms As TextBox
    Friend WithEvents Label1 As Label
    Friend WithEvents chkCaseInsensitive As CheckBox
    Friend WithEvents chkRegEx As CheckBox
    Friend WithEvents btnClear As Button
    Friend WithEvents radAny As RadioButton
    Friend WithEvents radInfo As RadioButton
    Friend WithEvents Label2 As Label
    Friend WithEvents radError As RadioButton
End Class
