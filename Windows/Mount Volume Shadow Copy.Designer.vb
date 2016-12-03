<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()>
Partial Class Mount_Volume_Shadow_Copy
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
        Dim resources As System.ComponentModel.ComponentResourceManager = New System.ComponentModel.ComponentResourceManager(GetType(Mount_Volume_Shadow_Copy))
        Me.TableLayoutPanel1 = New System.Windows.Forms.TableLayoutPanel()
        Me.btnUnmount = New System.Windows.Forms.Button()
        Me.btnMount = New System.Windows.Forms.Button()
        Me.btnRefreshList = New System.Windows.Forms.Button()
        Me.lblMainLabel = New System.Windows.Forms.Label()
        Me.listShadowCopyIDs = New System.Windows.Forms.ListView()
        Me.ColumnHeader1 = CType(New System.Windows.Forms.ColumnHeader(), System.Windows.Forms.ColumnHeader)
        Me.TableLayoutPanel1.SuspendLayout()
        Me.SuspendLayout()
        '
        'TableLayoutPanel1
        '
        Me.TableLayoutPanel1.ColumnCount = 2
        Me.TableLayoutPanel1.ColumnStyles.Add(New System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50.0!))
        Me.TableLayoutPanel1.ColumnStyles.Add(New System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50.0!))
        Me.TableLayoutPanel1.Controls.Add(Me.btnUnmount, 1, 0)
        Me.TableLayoutPanel1.Controls.Add(Me.btnMount, 0, 0)
        Me.TableLayoutPanel1.Controls.Add(Me.btnRefreshList, 0, 1)
        Me.TableLayoutPanel1.Location = New System.Drawing.Point(12, 324)
        Me.TableLayoutPanel1.Name = "TableLayoutPanel1"
        Me.TableLayoutPanel1.RowCount = 2
        Me.TableLayoutPanel1.RowStyles.Add(New System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 50.0!))
        Me.TableLayoutPanel1.RowStyles.Add(New System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 50.0!))
        Me.TableLayoutPanel1.RowStyles.Add(New System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 20.0!))
        Me.TableLayoutPanel1.Size = New System.Drawing.Size(413, 60)
        Me.TableLayoutPanel1.TabIndex = 2
        '
        'btnUnmount
        '
        Me.btnUnmount.Enabled = False
        Me.btnUnmount.Location = New System.Drawing.Point(209, 3)
        Me.btnUnmount.Name = "btnUnmount"
        Me.btnUnmount.Size = New System.Drawing.Size(200, 23)
        Me.btnUnmount.TabIndex = 0
        Me.btnUnmount.Text = "Unmount"
        Me.btnUnmount.UseVisualStyleBackColor = True
        '
        'btnMount
        '
        Me.btnMount.Enabled = False
        Me.btnMount.Location = New System.Drawing.Point(3, 3)
        Me.btnMount.Name = "btnMount"
        Me.btnMount.Size = New System.Drawing.Size(200, 23)
        Me.btnMount.TabIndex = 1
        Me.btnMount.Text = "Mount"
        Me.btnMount.UseVisualStyleBackColor = True
        '
        'btnRefreshList
        '
        Me.TableLayoutPanel1.SetColumnSpan(Me.btnRefreshList, 2)
        Me.btnRefreshList.Location = New System.Drawing.Point(3, 33)
        Me.btnRefreshList.Name = "btnRefreshList"
        Me.btnRefreshList.Size = New System.Drawing.Size(406, 23)
        Me.btnRefreshList.TabIndex = 2
        Me.btnRefreshList.Text = "Refresh Shadow Copy Snapshot List"
        Me.btnRefreshList.UseVisualStyleBackColor = True
        '
        'lblMainLabel
        '
        Me.lblMainLabel.AutoSize = True
        Me.lblMainLabel.Location = New System.Drawing.Point(13, 9)
        Me.lblMainLabel.Name = "lblMainLabel"
        Me.lblMainLabel.Size = New System.Drawing.Size(419, 169)
        Me.lblMainLabel.TabIndex = 3
        Me.lblMainLabel.Text = resources.GetString("lblMainLabel.Text")
        '
        'listShadowCopyIDs
        '
        Me.listShadowCopyIDs.Columns.AddRange(New System.Windows.Forms.ColumnHeader() {Me.ColumnHeader1})
        Me.listShadowCopyIDs.Location = New System.Drawing.Point(16, 181)
        Me.listShadowCopyIDs.Name = "listShadowCopyIDs"
        Me.listShadowCopyIDs.Size = New System.Drawing.Size(409, 137)
        Me.listShadowCopyIDs.TabIndex = 4
        Me.listShadowCopyIDs.UseCompatibleStateImageBehavior = False
        Me.listShadowCopyIDs.View = System.Windows.Forms.View.Details
        '
        'ColumnHeader1
        '
        Me.ColumnHeader1.Text = "Snapshot Time"
        Me.ColumnHeader1.Width = 382
        '
        'Mount_Volume_Shadow_Copy
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(96.0!, 96.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi
        Me.ClientSize = New System.Drawing.Size(435, 396)
        Me.Controls.Add(Me.listShadowCopyIDs)
        Me.Controls.Add(Me.lblMainLabel)
        Me.Controls.Add(Me.TableLayoutPanel1)
        Me.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle
        Me.Icon = Global.Restore_Point_Creator.My.Resources.Resources.RestorePoint_noBackground_2
        Me.Name = "Mount_Volume_Shadow_Copy"
        Me.Text = "Mount Volume Shadow Copy"
        Me.TableLayoutPanel1.ResumeLayout(False)
        Me.ResumeLayout(False)
        Me.PerformLayout()

    End Sub
    Friend WithEvents TableLayoutPanel1 As TableLayoutPanel
    Friend WithEvents btnUnmount As Button
    Friend WithEvents btnMount As Button
    Friend WithEvents lblMainLabel As Label
    Friend WithEvents btnRefreshList As Button
    Friend WithEvents listShadowCopyIDs As ListView
    Friend WithEvents ColumnHeader1 As ColumnHeader
End Class
