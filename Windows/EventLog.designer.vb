﻿<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()>
Partial Class eventLogForm
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
        Me.components = New System.ComponentModel.Container()
        Me.eventLogList = New System.Windows.Forms.ListView()
        Me.ColumnHeader1 = CType(New System.Windows.Forms.ColumnHeader(), System.Windows.Forms.ColumnHeader)
        Me.ColumnHeader2 = CType(New System.Windows.Forms.ColumnHeader(), System.Windows.Forms.ColumnHeader)
        Me.ColumnHeader3 = CType(New System.Windows.Forms.ColumnHeader(), System.Windows.Forms.ColumnHeader)
        Me.ColumnHeader4 = CType(New System.Windows.Forms.ColumnHeader(), System.Windows.Forms.ColumnHeader)
        Me.ColumnHeader5 = CType(New System.Windows.Forms.ColumnHeader(), System.Windows.Forms.ColumnHeader)
        Me.imageList = New System.Windows.Forms.ImageList(Me.components)
        Me.eventLogText = New System.Windows.Forms.TextBox()
        Me.btnRefreshEvents = New System.Windows.Forms.Button()
        Me.StatusStrip1 = New System.Windows.Forms.StatusStrip()
        Me.lblLogEntryCount = New System.Windows.Forms.ToolStripStatusLabel()
        Me.lblProcessedIn = New System.Windows.Forms.ToolStripStatusLabel()
        Me.progressBar = New System.Windows.Forms.ToolStripProgressBar()
        Me.btnOpenEventLog = New System.Windows.Forms.Button()
        Me.SplitContainer1 = New System.Windows.Forms.SplitContainer()
        Me.TableLayoutPanel2 = New System.Windows.Forms.TableLayoutPanel()
        Me.btnExportLogs = New System.Windows.Forms.Button()
        Me.btnClear = New System.Windows.Forms.Button()
        Me.btnSearch = New System.Windows.Forms.Button()
        Me.chkAskMeToSubmitIfViewingAnExceptionEntry = New System.Windows.Forms.CheckBox()
        Me.SaveFileDialog1 = New System.Windows.Forms.SaveFileDialog()
        Me.StatusStrip1.SuspendLayout()
        CType(Me.SplitContainer1, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.SplitContainer1.Panel1.SuspendLayout()
        Me.SplitContainer1.Panel2.SuspendLayout()
        Me.SplitContainer1.SuspendLayout()
        Me.TableLayoutPanel2.SuspendLayout()
        Me.SuspendLayout()
        '
        'eventLogList
        '
        Me.eventLogList.Anchor = CType((((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Bottom) _
            Or System.Windows.Forms.AnchorStyles.Left) _
            Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.eventLogList.Columns.AddRange(New System.Windows.Forms.ColumnHeader() {Me.ColumnHeader1, Me.ColumnHeader2, Me.ColumnHeader3, Me.ColumnHeader4, Me.ColumnHeader5})
        Me.eventLogList.FullRowSelect = True
        Me.eventLogList.Location = New System.Drawing.Point(3, 3)
        Me.eventLogList.MultiSelect = False
        Me.eventLogList.Name = "eventLogList"
        Me.eventLogList.Size = New System.Drawing.Size(410, 172)
        Me.eventLogList.SmallImageList = Me.imageList
        Me.eventLogList.TabIndex = 0
        Me.eventLogList.UseCompatibleStateImageBehavior = False
        Me.eventLogList.View = System.Windows.Forms.View.Details
        '
        'ColumnHeader1
        '
        Me.ColumnHeader1.Text = "Event Type"
        Me.ColumnHeader1.Width = 100
        '
        'ColumnHeader2
        '
        Me.ColumnHeader2.Text = "Date & Time"
        Me.ColumnHeader2.Width = 100
        '
        'ColumnHeader3
        '
        Me.ColumnHeader3.Text = "Event ID"
        Me.ColumnHeader3.TextAlign = System.Windows.Forms.HorizontalAlignment.Center
        '
        'ColumnHeader4
        '
        Me.ColumnHeader4.Text = "Log Source"
        Me.ColumnHeader4.Width = 95
        '
        'ColumnHeader5
        '
        Me.ColumnHeader5.Text = "*"
        Me.ColumnHeader5.Width = 18
        '
        'imageList
        '
        Me.imageList.ColorDepth = System.Windows.Forms.ColorDepth.Depth32Bit
        Me.imageList.ImageSize = New System.Drawing.Size(16, 16)
        Me.imageList.TransparentColor = System.Drawing.Color.Transparent
        '
        'eventLogText
        '
        Me.eventLogText.Anchor = CType((((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Bottom) _
            Or System.Windows.Forms.AnchorStyles.Left) _
            Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.eventLogText.BackColor = System.Drawing.SystemColors.Window
        Me.eventLogText.Location = New System.Drawing.Point(3, 3)
        Me.eventLogText.Multiline = True
        Me.eventLogText.Name = "eventLogText"
        Me.eventLogText.ReadOnly = True
        Me.eventLogText.Size = New System.Drawing.Size(405, 263)
        Me.eventLogText.TabIndex = 1
        '
        'btnRefreshEvents
        '
        Me.btnRefreshEvents.Anchor = CType((((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Bottom) _
            Or System.Windows.Forms.AnchorStyles.Left) _
            Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.btnRefreshEvents.Image = Global.Restore_Point_Creator.My.Resources.Resources.refresh
        Me.btnRefreshEvents.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft
        Me.btnRefreshEvents.Location = New System.Drawing.Point(3, 3)
        Me.btnRefreshEvents.Name = "btnRefreshEvents"
        Me.btnRefreshEvents.Size = New System.Drawing.Size(203, 23)
        Me.btnRefreshEvents.TabIndex = 2
        Me.btnRefreshEvents.Text = "Refresh Events (F5)"
        Me.btnRefreshEvents.UseVisualStyleBackColor = True
        '
        'StatusStrip1
        '
        Me.StatusStrip1.Items.AddRange(New System.Windows.Forms.ToolStripItem() {Me.lblLogEntryCount, Me.lblProcessedIn, Me.progressBar})
        Me.StatusStrip1.Location = New System.Drawing.Point(0, 310)
        Me.StatusStrip1.Name = "StatusStrip1"
        Me.StatusStrip1.Size = New System.Drawing.Size(860, 22)
        Me.StatusStrip1.TabIndex = 3
        Me.StatusStrip1.Text = "StatusStrip1"
        '
        'lblLogEntryCount
        '
        Me.lblLogEntryCount.Name = "lblLogEntryCount"
        Me.lblLogEntryCount.Size = New System.Drawing.Size(122, 17)
        Me.lblLogEntryCount.Text = "Entries in Event Log: 0"
        '
        'lblProcessedIn
        '
        Me.lblProcessedIn.Name = "lblProcessedIn"
        Me.lblProcessedIn.Padding = New System.Windows.Forms.Padding(30, 0, 0, 0)
        Me.lblProcessedIn.Size = New System.Drawing.Size(112, 17)
        Me.lblProcessedIn.Text = "Processed in..."
        '
        'progressBar
        '
        Me.progressBar.Name = "progressBar"
        Me.progressBar.Size = New System.Drawing.Size(200, 16)
        Me.progressBar.Visible = False
        '
        'btnOpenEventLog
        '
        Me.btnOpenEventLog.Anchor = CType(((System.Windows.Forms.AnchorStyles.Bottom Or System.Windows.Forms.AnchorStyles.Left) _
            Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.btnOpenEventLog.Image = Global.Restore_Point_Creator.My.Resources.Resources.textBlock
        Me.btnOpenEventLog.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft
        Me.btnOpenEventLog.Location = New System.Drawing.Point(212, 3)
        Me.btnOpenEventLog.Name = "btnOpenEventLog"
        Me.btnOpenEventLog.Size = New System.Drawing.Size(204, 23)
        Me.btnOpenEventLog.TabIndex = 3
        Me.btnOpenEventLog.Text = "Open System Event Log"
        Me.btnOpenEventLog.UseVisualStyleBackColor = True
        '
        'SplitContainer1
        '
        Me.SplitContainer1.Anchor = CType((((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Bottom) _
            Or System.Windows.Forms.AnchorStyles.Left) _
            Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.SplitContainer1.Location = New System.Drawing.Point(12, 12)
        Me.SplitContainer1.Name = "SplitContainer1"
        '
        'SplitContainer1.Panel1
        '
        Me.SplitContainer1.Panel1.Controls.Add(Me.TableLayoutPanel2)
        Me.SplitContainer1.Panel1.Controls.Add(Me.eventLogList)
        '
        'SplitContainer1.Panel2
        '
        Me.SplitContainer1.Panel2.Controls.Add(Me.eventLogText)
        Me.SplitContainer1.Size = New System.Drawing.Size(836, 272)
        Me.SplitContainer1.SplitterDistance = 418
        Me.SplitContainer1.TabIndex = 5
        '
        'TableLayoutPanel2
        '
        Me.TableLayoutPanel2.Anchor = CType(((System.Windows.Forms.AnchorStyles.Bottom Or System.Windows.Forms.AnchorStyles.Left) _
            Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.TableLayoutPanel2.ColumnCount = 2
        Me.TableLayoutPanel2.ColumnStyles.Add(New System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50.0!))
        Me.TableLayoutPanel2.ColumnStyles.Add(New System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50.0!))
        Me.TableLayoutPanel2.Controls.Add(Me.btnExportLogs, 0, 2)
        Me.TableLayoutPanel2.Controls.Add(Me.btnClear, 0, 1)
        Me.TableLayoutPanel2.Controls.Add(Me.btnOpenEventLog, 1, 0)
        Me.TableLayoutPanel2.Controls.Add(Me.btnRefreshEvents, 0, 0)
        Me.TableLayoutPanel2.Controls.Add(Me.btnSearch, 0, 1)
        Me.TableLayoutPanel2.Location = New System.Drawing.Point(0, 178)
        Me.TableLayoutPanel2.Name = "TableLayoutPanel2"
        Me.TableLayoutPanel2.RowCount = 3
        Me.TableLayoutPanel2.RowStyles.Add(New System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 33.0!))
        Me.TableLayoutPanel2.RowStyles.Add(New System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 33.0!))
        Me.TableLayoutPanel2.RowStyles.Add(New System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 34.0!))
        Me.TableLayoutPanel2.Size = New System.Drawing.Size(419, 90)
        Me.TableLayoutPanel2.TabIndex = 1
        '
        'btnExportLogs
        '
        Me.btnExportLogs.Anchor = CType((((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Bottom) _
            Or System.Windows.Forms.AnchorStyles.Left) _
            Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.TableLayoutPanel2.SetColumnSpan(Me.btnExportLogs, 2)
        Me.btnExportLogs.Image = Global.Restore_Point_Creator.My.Resources.Resources.save
        Me.btnExportLogs.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft
        Me.btnExportLogs.Location = New System.Drawing.Point(3, 61)
        Me.btnExportLogs.Name = "btnExportLogs"
        Me.btnExportLogs.Size = New System.Drawing.Size(413, 26)
        Me.btnExportLogs.TabIndex = 9
        Me.btnExportLogs.Text = "Export Application Event Logs to File"
        Me.btnExportLogs.UseVisualStyleBackColor = True
        '
        'btnClear
        '
        Me.btnClear.Anchor = CType((((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Bottom) _
            Or System.Windows.Forms.AnchorStyles.Left) _
            Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.btnClear.Image = Global.Restore_Point_Creator.My.Resources.Resources.edit_clear
        Me.btnClear.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft
        Me.btnClear.Location = New System.Drawing.Point(212, 32)
        Me.btnClear.Name = "btnClear"
        Me.btnClear.Size = New System.Drawing.Size(204, 23)
        Me.btnClear.TabIndex = 8
        Me.btnClear.Text = "Clear Search Results"
        Me.btnClear.UseVisualStyleBackColor = True
        '
        'btnSearch
        '
        Me.btnSearch.Anchor = CType((((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Bottom) _
            Or System.Windows.Forms.AnchorStyles.Left) _
            Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.btnSearch.Image = Global.Restore_Point_Creator.My.Resources.Resources.view
        Me.btnSearch.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft
        Me.btnSearch.Location = New System.Drawing.Point(3, 32)
        Me.btnSearch.Name = "btnSearch"
        Me.btnSearch.Size = New System.Drawing.Size(203, 23)
        Me.btnSearch.TabIndex = 7
        Me.btnSearch.Text = "Search Event Log"
        Me.btnSearch.UseVisualStyleBackColor = True
        '
        'chkAskMeToSubmitIfViewingAnExceptionEntry
        '
        Me.chkAskMeToSubmitIfViewingAnExceptionEntry.Anchor = CType((System.Windows.Forms.AnchorStyles.Bottom Or System.Windows.Forms.AnchorStyles.Left), System.Windows.Forms.AnchorStyles)
        Me.chkAskMeToSubmitIfViewingAnExceptionEntry.AutoSize = True
        Me.chkAskMeToSubmitIfViewingAnExceptionEntry.Location = New System.Drawing.Point(12, 290)
        Me.chkAskMeToSubmitIfViewingAnExceptionEntry.Name = "chkAskMeToSubmitIfViewingAnExceptionEntry"
        Me.chkAskMeToSubmitIfViewingAnExceptionEntry.Size = New System.Drawing.Size(485, 17)
        Me.chkAskMeToSubmitIfViewingAnExceptionEntry.TabIndex = 6
        Me.chkAskMeToSubmitIfViewingAnExceptionEntry.Text = "Ask me if I want to submit a crash report if I view an Event Log Entry that conta" &
    "ins Exception data"
        Me.chkAskMeToSubmitIfViewingAnExceptionEntry.UseVisualStyleBackColor = True
        '
        'eventLogForm
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(6.0!, 13.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.ClientSize = New System.Drawing.Size(860, 332)
        Me.Controls.Add(Me.chkAskMeToSubmitIfViewingAnExceptionEntry)
        Me.Controls.Add(Me.SplitContainer1)
        Me.Controls.Add(Me.StatusStrip1)
        Me.Icon = Global.Restore_Point_Creator.My.Resources.Resources.RestorePoint_noBackground_2
        Me.KeyPreview = True
        Me.MinimumSize = New System.Drawing.Size(876, 371)
        Me.Name = "eventLogForm"
        Me.Text = "Restore Point Creator Application Event Log"
        Me.StatusStrip1.ResumeLayout(False)
        Me.StatusStrip1.PerformLayout()
        Me.SplitContainer1.Panel1.ResumeLayout(False)
        Me.SplitContainer1.Panel2.ResumeLayout(False)
        Me.SplitContainer1.Panel2.PerformLayout()
        CType(Me.SplitContainer1, System.ComponentModel.ISupportInitialize).EndInit()
        Me.SplitContainer1.ResumeLayout(False)
        Me.TableLayoutPanel2.ResumeLayout(False)
        Me.ResumeLayout(False)
        Me.PerformLayout()

    End Sub
    Friend WithEvents eventLogList As System.Windows.Forms.ListView
    Friend WithEvents ColumnHeader1 As System.Windows.Forms.ColumnHeader
    Friend WithEvents ColumnHeader2 As System.Windows.Forms.ColumnHeader
    Friend WithEvents ColumnHeader3 As System.Windows.Forms.ColumnHeader
    Friend WithEvents eventLogText As System.Windows.Forms.TextBox
    Friend WithEvents btnRefreshEvents As System.Windows.Forms.Button
    Friend WithEvents StatusStrip1 As System.Windows.Forms.StatusStrip
    Friend WithEvents imageList As System.Windows.Forms.ImageList
    Friend WithEvents lblLogEntryCount As System.Windows.Forms.ToolStripStatusLabel
    Friend WithEvents ColumnHeader4 As System.Windows.Forms.ColumnHeader
    Friend WithEvents lblProcessedIn As System.Windows.Forms.ToolStripStatusLabel
    Friend WithEvents btnOpenEventLog As System.Windows.Forms.Button
    Friend WithEvents SplitContainer1 As SplitContainer
    Friend WithEvents TableLayoutPanel2 As TableLayoutPanel
    Friend WithEvents progressBar As ToolStripProgressBar
    Friend WithEvents chkAskMeToSubmitIfViewingAnExceptionEntry As CheckBox
    Friend WithEvents btnSearch As Button
    Friend WithEvents btnClear As Button
    Friend WithEvents ColumnHeader5 As ColumnHeader
    Friend WithEvents btnExportLogs As Button
    Friend WithEvents SaveFileDialog1 As SaveFileDialog
End Class
