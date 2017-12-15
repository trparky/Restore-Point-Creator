<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()>
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
        Me.imageList = New System.Windows.Forms.ImageList(Me.components)
        Me.eventLogText = New System.Windows.Forms.TextBox()
        Me.btnRefreshEvents = New System.Windows.Forms.Button()
        Me.StatusStrip1 = New System.Windows.Forms.StatusStrip()
        Me.lblLogEntryCount = New System.Windows.Forms.ToolStripStatusLabel()
        Me.lblProcessedIn = New System.Windows.Forms.ToolStripStatusLabel()
        Me.lblLogFileSize = New System.Windows.Forms.ToolStripStatusLabel()
        Me.btnCleanLogFile = New System.Windows.Forms.Button()
        Me.SplitContainer1 = New System.Windows.Forms.SplitContainer()
        Me.chkMultiSelectMode = New System.Windows.Forms.CheckBox()
        Me.TableLayoutPanel2 = New System.Windows.Forms.TableLayoutPanel()
        Me.btnExportLogs = New System.Windows.Forms.Button()
        Me.btnDeleteIndividualLogEntry = New System.Windows.Forms.Button()
        Me.btnClear = New System.Windows.Forms.Button()
        Me.btnSearch = New System.Windows.Forms.Button()
        Me.chkAskMeToSubmitIfViewingAnExceptionEntry = New System.Windows.Forms.CheckBox()
        Me.SaveFileDialog1 = New System.Windows.Forms.SaveFileDialog()
        Me.pleaseWaitProgressBarChanger = New System.Windows.Forms.Timer(Me.components)
        Me.pleaseWaitPanel = New System.Windows.Forms.Panel()
        Me.pleaseWaitBorderText = New System.Windows.Forms.Label()
        Me.pleaseWaitlblLabel = New System.Windows.Forms.Label()
        Me.pleaseWaitProgressBar = New Tom.SmoothProgressBar()
        Me.pleaseWaitMessageChanger = New System.Windows.Forms.Timer(Me.components)
        Me.logFileWatcher = New System.IO.FileSystemWatcher()
        Me.lblLastModified = New System.Windows.Forms.ToolStripStatusLabel()
        Me.StatusStrip1.SuspendLayout()
        CType(Me.SplitContainer1, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.SplitContainer1.Panel1.SuspendLayout()
        Me.SplitContainer1.Panel2.SuspendLayout()
        Me.SplitContainer1.SuspendLayout()
        Me.TableLayoutPanel2.SuspendLayout()
        Me.pleaseWaitPanel.SuspendLayout()
        CType(Me.logFileWatcher, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.SuspendLayout()
        '
        'eventLogList
        '
        Me.eventLogList.Anchor = CType((((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Bottom) _
            Or System.Windows.Forms.AnchorStyles.Left) _
            Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.eventLogList.Columns.AddRange(New System.Windows.Forms.ColumnHeader() {Me.ColumnHeader1, Me.ColumnHeader2, Me.ColumnHeader3})
        Me.eventLogList.FullRowSelect = True
        Me.eventLogList.Location = New System.Drawing.Point(3, 3)
        Me.eventLogList.MultiSelect = False
        Me.eventLogList.Name = "eventLogList"
        Me.eventLogList.Size = New System.Drawing.Size(410, 153)
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
        Me.StatusStrip1.Items.AddRange(New System.Windows.Forms.ToolStripItem() {Me.lblLogEntryCount, Me.lblProcessedIn, Me.lblLogFileSize, Me.lblLastModified})
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
        'lblLogFileSize
        '
        Me.lblLogFileSize.Name = "lblLogFileSize"
        Me.lblLogFileSize.Size = New System.Drawing.Size(100, 17)
        Me.lblLogFileSize.Text = "Log File Size: 0 KB"
        '
        'btnCleanLogFile
        '
        Me.btnCleanLogFile.Anchor = CType(((System.Windows.Forms.AnchorStyles.Bottom Or System.Windows.Forms.AnchorStyles.Left) _
            Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.btnCleanLogFile.Image = Global.Restore_Point_Creator.My.Resources.Resources.textBlock
        Me.btnCleanLogFile.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft
        Me.btnCleanLogFile.Location = New System.Drawing.Point(212, 3)
        Me.btnCleanLogFile.Name = "btnCleanLogFile"
        Me.btnCleanLogFile.Size = New System.Drawing.Size(204, 23)
        Me.btnCleanLogFile.TabIndex = 3
        Me.btnCleanLogFile.Text = "Clean Application Log"
        Me.btnCleanLogFile.UseVisualStyleBackColor = True
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
        Me.SplitContainer1.Panel1.Controls.Add(Me.chkMultiSelectMode)
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
        'chkMultiSelectMode
        '
        Me.chkMultiSelectMode.Anchor = CType((System.Windows.Forms.AnchorStyles.Bottom Or System.Windows.Forms.AnchorStyles.Left), System.Windows.Forms.AnchorStyles)
        Me.chkMultiSelectMode.AutoSize = True
        Me.chkMultiSelectMode.Location = New System.Drawing.Point(4, 158)
        Me.chkMultiSelectMode.Name = "chkMultiSelectMode"
        Me.chkMultiSelectMode.Size = New System.Drawing.Size(289, 17)
        Me.chkMultiSelectMode.TabIndex = 2
        Me.chkMultiSelectMode.Text = "Multi-Select Mode (Used for deleting multiple log entries)"
        Me.chkMultiSelectMode.UseVisualStyleBackColor = True
        '
        'TableLayoutPanel2
        '
        Me.TableLayoutPanel2.Anchor = CType(((System.Windows.Forms.AnchorStyles.Bottom Or System.Windows.Forms.AnchorStyles.Left) _
            Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.TableLayoutPanel2.ColumnCount = 2
        Me.TableLayoutPanel2.ColumnStyles.Add(New System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50.0!))
        Me.TableLayoutPanel2.ColumnStyles.Add(New System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50.0!))
        Me.TableLayoutPanel2.Controls.Add(Me.btnExportLogs, 0, 1)
        Me.TableLayoutPanel2.Controls.Add(Me.btnCleanLogFile, 1, 0)
        Me.TableLayoutPanel2.Controls.Add(Me.btnRefreshEvents, 0, 0)
        Me.TableLayoutPanel2.Controls.Add(Me.btnDeleteIndividualLogEntry, 1, 1)
        Me.TableLayoutPanel2.Controls.Add(Me.btnClear, 1, 2)
        Me.TableLayoutPanel2.Controls.Add(Me.btnSearch, 0, 2)
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
        Me.btnExportLogs.Image = Global.Restore_Point_Creator.My.Resources.Resources.save
        Me.btnExportLogs.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft
        Me.btnExportLogs.Location = New System.Drawing.Point(3, 32)
        Me.btnExportLogs.Name = "btnExportLogs"
        Me.btnExportLogs.Size = New System.Drawing.Size(203, 23)
        Me.btnExportLogs.TabIndex = 9
        Me.btnExportLogs.Text = "Export Application Event Logs"
        Me.btnExportLogs.UseVisualStyleBackColor = True
        '
        'btnDeleteIndividualLogEntry
        '
        Me.btnDeleteIndividualLogEntry.Anchor = CType((((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Bottom) _
            Or System.Windows.Forms.AnchorStyles.Left) _
            Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.btnDeleteIndividualLogEntry.Enabled = False
        Me.btnDeleteIndividualLogEntry.Image = Global.Restore_Point_Creator.My.Resources.Resources.delete
        Me.btnDeleteIndividualLogEntry.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft
        Me.btnDeleteIndividualLogEntry.Location = New System.Drawing.Point(212, 32)
        Me.btnDeleteIndividualLogEntry.Name = "btnDeleteIndividualLogEntry"
        Me.btnDeleteIndividualLogEntry.Size = New System.Drawing.Size(204, 23)
        Me.btnDeleteIndividualLogEntry.TabIndex = 10
        Me.btnDeleteIndividualLogEntry.Text = "Delete Individual Log Entry"
        Me.btnDeleteIndividualLogEntry.UseVisualStyleBackColor = True
        '
        'btnClear
        '
        Me.btnClear.Anchor = CType((((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Bottom) _
            Or System.Windows.Forms.AnchorStyles.Left) _
            Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.btnClear.Enabled = False
        Me.btnClear.Image = Global.Restore_Point_Creator.My.Resources.Resources.edit_clear
        Me.btnClear.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft
        Me.btnClear.Location = New System.Drawing.Point(212, 61)
        Me.btnClear.Name = "btnClear"
        Me.btnClear.Size = New System.Drawing.Size(204, 26)
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
        Me.btnSearch.Location = New System.Drawing.Point(3, 61)
        Me.btnSearch.Name = "btnSearch"
        Me.btnSearch.Size = New System.Drawing.Size(203, 26)
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
        'pleaseWaitProgressBarChanger
        '
        Me.pleaseWaitProgressBarChanger.Interval = 25
        '
        'pleaseWaitPanel
        '
        Me.pleaseWaitPanel.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle
        Me.pleaseWaitPanel.Controls.Add(Me.pleaseWaitBorderText)
        Me.pleaseWaitPanel.Controls.Add(Me.pleaseWaitlblLabel)
        Me.pleaseWaitPanel.Controls.Add(Me.pleaseWaitProgressBar)
        Me.pleaseWaitPanel.Location = New System.Drawing.Point(284, 123)
        Me.pleaseWaitPanel.Name = "pleaseWaitPanel"
        Me.pleaseWaitPanel.Size = New System.Drawing.Size(293, 86)
        Me.pleaseWaitPanel.TabIndex = 41
        Me.pleaseWaitPanel.Visible = False
        '
        'pleaseWaitBorderText
        '
        Me.pleaseWaitBorderText.BackColor = System.Drawing.Color.SkyBlue
        Me.pleaseWaitBorderText.Location = New System.Drawing.Point(0, 0)
        Me.pleaseWaitBorderText.Name = "pleaseWaitBorderText"
        Me.pleaseWaitBorderText.Padding = New System.Windows.Forms.Padding(3, 0, 0, 0)
        Me.pleaseWaitBorderText.Size = New System.Drawing.Size(292, 23)
        Me.pleaseWaitBorderText.TabIndex = 4
        Me.pleaseWaitBorderText.Text = "Please Wait..."
        Me.pleaseWaitBorderText.TextAlign = System.Drawing.ContentAlignment.MiddleLeft
        '
        'pleaseWaitlblLabel
        '
        Me.pleaseWaitlblLabel.AutoSize = True
        Me.pleaseWaitlblLabel.Location = New System.Drawing.Point(3, 31)
        Me.pleaseWaitlblLabel.Name = "pleaseWaitlblLabel"
        Me.pleaseWaitlblLabel.Size = New System.Drawing.Size(39, 13)
        Me.pleaseWaitlblLabel.TabIndex = 3
        Me.pleaseWaitlblLabel.Text = "Label1"
        '
        'pleaseWaitProgressBar
        '
        Me.pleaseWaitProgressBar.Location = New System.Drawing.Point(6, 56)
        Me.pleaseWaitProgressBar.Maximum = 100
        Me.pleaseWaitProgressBar.Minimum = 0
        Me.pleaseWaitProgressBar.Name = "pleaseWaitProgressBar"
        Me.pleaseWaitProgressBar.ProgressBarColor = System.Drawing.Color.Blue
        Me.pleaseWaitProgressBar.Size = New System.Drawing.Size(268, 19)
        Me.pleaseWaitProgressBar.TabIndex = 2
        Me.pleaseWaitProgressBar.Value = 0
        '
        'pleaseWaitMessageChanger
        '
        Me.pleaseWaitMessageChanger.Interval = 250
        '
        'logFileWatcher
        '
        Me.logFileWatcher.EnableRaisingEvents = True
        Me.logFileWatcher.SynchronizingObject = Me
        '
        'lblLastModified
        '
        Me.lblLastModified.Name = "lblLastModified"
        Me.lblLastModified.Padding = New System.Windows.Forms.Padding(30, 0, 0, 0)
        Me.lblLastModified.Size = New System.Drawing.Size(150, 17)
        Me.lblLastModified.Text = "ToolStripStatusLabel1"
        '
        'eventLogForm
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(96.0!, 96.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi
        Me.ClientSize = New System.Drawing.Size(860, 332)
        Me.Controls.Add(Me.pleaseWaitPanel)
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
        Me.SplitContainer1.Panel1.PerformLayout()
        Me.SplitContainer1.Panel2.ResumeLayout(False)
        Me.SplitContainer1.Panel2.PerformLayout()
        CType(Me.SplitContainer1, System.ComponentModel.ISupportInitialize).EndInit()
        Me.SplitContainer1.ResumeLayout(False)
        Me.TableLayoutPanel2.ResumeLayout(False)
        Me.pleaseWaitPanel.ResumeLayout(False)
        Me.pleaseWaitPanel.PerformLayout()
        CType(Me.logFileWatcher, System.ComponentModel.ISupportInitialize).EndInit()
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
    Friend WithEvents lblProcessedIn As System.Windows.Forms.ToolStripStatusLabel
    Friend WithEvents btnCleanLogFile As System.Windows.Forms.Button
    Friend WithEvents SplitContainer1 As SplitContainer
    Friend WithEvents TableLayoutPanel2 As TableLayoutPanel
    Friend WithEvents chkAskMeToSubmitIfViewingAnExceptionEntry As CheckBox
    Friend WithEvents btnSearch As Button
    Friend WithEvents btnClear As Button
    Friend WithEvents btnExportLogs As Button
    Friend WithEvents SaveFileDialog1 As SaveFileDialog
    Friend WithEvents pleaseWaitProgressBarChanger As Timer
    Friend WithEvents pleaseWaitPanel As Panel
    Friend WithEvents pleaseWaitBorderText As Label
    Friend WithEvents pleaseWaitlblLabel As Label
    Friend WithEvents pleaseWaitProgressBar As Tom.SmoothProgressBar
    Friend WithEvents pleaseWaitMessageChanger As Timer
    Friend WithEvents lblLogFileSize As ToolStripStatusLabel
    Friend WithEvents logFileWatcher As IO.FileSystemWatcher
    Friend WithEvents btnDeleteIndividualLogEntry As Button
    Friend WithEvents chkMultiSelectMode As CheckBox
    Friend WithEvents lblLastModified As ToolStripStatusLabel
End Class
