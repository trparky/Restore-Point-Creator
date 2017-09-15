﻿Public Class eventLogForm
    Private m_SortingColumn As ColumnHeader
    Private boolDoneLoading As Boolean = False
    Private oldSplitterDifference As Integer
    Private boolAreWeLoadingTheEventLogData As Boolean = False

    Private rawSearchTerms As String = Nothing
    Private previousSearchType As Search_Event_Log.searceType

    Private selectedIndex As Long
    Private eventLogContents As New List(Of myListViewItemTypes.eventLogListEntry)
    Private workingThread As Threading.Thread

    Private Function convertToEventLogType(input As Short) As EventLogEntryType
        If input = EventLogEntryType.Error Then
            Return EventLogEntryType.Error
        ElseIf input = EventLogEntryType.FailureAudit Then
            Return EventLogEntryType.FailureAudit
        ElseIf input = EventLogEntryType.Information Then
            Return EventLogEntryType.Information
        ElseIf input = EventLogEntryType.SuccessAudit Then
            Return EventLogEntryType.SuccessAudit
        ElseIf input = EventLogEntryType.Warning Then
            Return EventLogEntryType.Warning
        Else
            Return EventLogEntryType.Information
        End If
    End Function

    Private Function UNIXTimestampToDate(ByVal strUnixTime As ULong) As Date
        Return DateAdd(DateInterval.Second, strUnixTime, New DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc)).ToLocalTime
    End Function

    Private Function convertLineFeeds(input As String) As String
        ' Checks to see if the file is in Windows linefeed format or UNIX linefeed format.
        If input.Contains(vbCrLf) Then
            Return input ' It's in Windows linefeed format so we return the output as is.
        Else
            Return input.Replace(vbLf, vbCrLf) ' It's in UNIX linefeed format so we have to convert it to Windows before we return the output.
        End If
    End Function

    Sub loadEventLogData(ByRef itemsToPutInToList As List(Of myListViewItemTypes.eventLogListEntry))
        Dim itemAdd As myListViewItemTypes.eventLogListEntry
        Dim eventLogType As EventLogEntryType

        Try
            For Each logEntry As restorePointCreatorExportedLog In Functions.eventLogFunctions.applicationLog
                eventLogType = convertToEventLogType(logEntry.logType)
                itemAdd = New myListViewItemTypes.eventLogListEntry(eventLogType.ToString)

                With itemAdd
                    ' 0 = "Error"
                    ' 1 = "Information"
                    ' 2 = "Warning"
                    Select Case eventLogType
                        Case EventLogEntryType.Error ' Error
                            .ImageIndex = 0
                        Case EventLogEntryType.Information ' Information
                            .ImageIndex = 1
                        Case EventLogEntryType.Warning ' Warning
                            .ImageIndex = 2
                    End Select

                    .SubItems.Add(UNIXTimestampToDate(logEntry.unixTime).ToString)
                    .SubItems.Add(logEntry.logID.ToString("N0"))

                    .strEventLogText = Functions.support.removeSourceCodePathInfo(convertLineFeeds(logEntry.logData))
                    .longEventLogEntryID = logEntry.logID
                    .strEventLogSource = logEntry.logSource
                    .shortLevelType = logEntry.logType
                End With

                itemsToPutInToList.Add(itemAdd)
                itemAdd = Nothing
            Next
        Catch ex As Threading.ThreadAbortException
        Catch ex As Exception
            MsgBox(ex.Message)
            Functions.eventLogFunctions.writeCrashToEventLog(ex)
        End Try
    End Sub

    Sub loadEventLog()
        Try
            boolAreWeLoadingTheEventLogData = True
            Invoke(Sub() Me.Cursor = Cursors.WaitCursor)
            eventLogContents.Clear() ' Cleans our cached log entries in memory.

            Dim timeStamp As Stopwatch = Stopwatch.StartNew()
            loadEventLogData(eventLogContents)

            Dim longElapsedMilisecond As Long = timeStamp.ElapsedMilliseconds
            Dim dblElapsedSeconds As Double = timeStamp.Elapsed.TotalSeconds

            If timeStamp.Elapsed.Milliseconds < 1000 Then Threading.Thread.Sleep(1000 - timeStamp.Elapsed.Milliseconds)

            Me.Invoke(Sub()
                          lblLogEntryCount.Text = "Entries in Event Log: " & eventLogContents.Count.ToString("N0")
                          loadEventLogContentsIntoList()

                          Me.Cursor = Cursors.Default
                          boolDoneLoading = True
                          boolAreWeLoadingTheEventLogData = False

                          closePleaseWaitPanel()
                          timeStamp.Stop()
                          lblProcessedIn.Text = String.Format("Event Log Loaded and Processed in {0}ms ({1} seconds).", longElapsedMilisecond.ToString("N0"), Math.Round(dblElapsedSeconds, 2))
                      End Sub)
        Catch ex As Threading.ThreadAbortException
        Finally
            workingThread = Nothing
        End Try
    End Sub

    Private Sub eventLogForm_FormClosing(sender As Object, e As FormClosingEventArgs) Handles Me.FormClosing
        If workingThread IsNot Nothing Then workingThread.Abort()

        My.Settings.eventLogFormWindowLocation = Me.Location

        If globalVariables.windows.eventLogForm IsNot Nothing Then
            globalVariables.windows.eventLogForm.Dispose()
            globalVariables.windows.eventLogForm = Nothing
        End If
    End Sub

    Private Sub eventLogForm_KeyUp(sender As Object, e As KeyEventArgs) Handles Me.KeyUp
        If e.KeyCode = Keys.F5 Then
            If Not boolAreWeLoadingTheEventLogData Then
                openPleaseWaitPanel("Loading Event Log Data... Please Wait.")

                workingThread = New Threading.Thread(AddressOf loadEventLog)
                workingThread.Name = "Event Log Data Loading Thread"
                workingThread.IsBackground = True
                workingThread.Start()
            End If
        End If
    End Sub

    Sub applySavedSorting()
        ' Some data validation.
        If My.Settings.eventLogSortingColumn < 0 Or My.Settings.eventLogSortingColumn > 4 Then
            My.Settings.eventLogSortingColumn = 0
        End If

        If My.Settings.eventLogSortingOrder <> 1 And My.Settings.eventLogSortingOrder <> 2 Then
            My.Settings.eventLogSortingOrder = 2
        End If
        ' Some data validation.

        ' Get the new sorting column.
        If My.Settings.eventLogSortingColumn > 2 Then My.Settings.eventLogSortingColumn = 2
        Dim new_sorting_column As ColumnHeader = eventLogList.Columns(My.Settings.eventLogSortingColumn)
        Dim sort_order As SortOrder = My.Settings.eventLogSortingOrder

        ' Figure out the new sorting order.
        If (m_SortingColumn IsNot Nothing) Then
            ' See if this is the same column.
            If new_sorting_column.Equals(m_SortingColumn) Then
                ' Same column. Switch the sort order.
                If m_SortingColumn.Text.StartsWith("> ") Then
                    sort_order = SortOrder.Descending
                    My.Settings.eventLogSortingOrder = SortOrder.Descending
                Else
                    sort_order = SortOrder.Ascending
                    My.Settings.eventLogSortingOrder = SortOrder.Ascending
                End If
            Else
                ' New column. Sort ascending.
                sort_order = SortOrder.Ascending
                My.Settings.eventLogSortingOrder = SortOrder.Ascending
            End If

            ' Remove the old sort indicator.
            m_SortingColumn.Text = m_SortingColumn.Text.Substring(2)
        End If

        ' Display the new sort order.
        m_SortingColumn = new_sorting_column
        If sort_order = SortOrder.Ascending Then
            m_SortingColumn.Text = "> " & m_SortingColumn.Text
        Else
            m_SortingColumn.Text = "< " & m_SortingColumn.Text
        End If

        ' Create a comparer.
        eventLogList.ListViewItemSorter = New Functions.listViewSorter.ListViewComparer(My.Settings.eventLogSortingColumn, sort_order)

        ' Sort.
        eventLogList.Sort()
    End Sub

    Private Sub eventLog_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        Dim logFileInfo As New IO.FileInfo(Functions.eventLogFunctions.strLogFile)
        logFileWatcher.Path = logFileInfo.DirectoryName
        logFileWatcher.Filter = logFileInfo.Name
        logFileInfo = Nothing

        If IO.File.Exists(Functions.eventLogFunctions.strLogFile) Then
            lblLogFileSize.Text = "Log File Size: " & Functions.support.bytesToHumanSize(New IO.FileInfo(Functions.eventLogFunctions.strLogFile).Length)
        Else
            lblLogFileSize.Text = "Log File Size: (File Doesn't Exist)"
        End If

        Me.Location = Functions.support.verifyWindowLocation(My.Settings.eventLogFormWindowLocation)
        chkAskMeToSubmitIfViewingAnExceptionEntry.Checked = My.Settings.boolAskMeToSubmitIfViewingAnExceptionEntry
        applySavedSorting()

        If Not Functions.privilegeChecks.areWeAnAdministrator() Then
            btnCleanLogFile.Enabled = False
            btnCleanLogFile.Text &= " (Disabled)"
        End If
    End Sub

    Private Sub eventLogList_ColumnClick(sender As Object, e As ColumnClickEventArgs) Handles eventLogList.ColumnClick
        ' Get the new sorting column.
        Dim new_sorting_column As ColumnHeader = eventLogList.Columns(e.Column)
        My.Settings.eventLogSortingColumn = e.Column

        ' Figure out the new sorting order.
        Dim sort_order As SortOrder
        If (m_SortingColumn Is Nothing) Then
            ' New column. Sort ascending.
            sort_order = SortOrder.Ascending
            My.Settings.eventLogSortingOrder = SortOrder.Ascending
        Else
            ' See if this is the same column.
            If new_sorting_column.Equals(m_SortingColumn) Then
                ' Same column. Switch the sort order.
                If m_SortingColumn.Text.StartsWith("> ") Then
                    sort_order = SortOrder.Descending
                    My.Settings.eventLogSortingOrder = SortOrder.Descending
                Else
                    sort_order = SortOrder.Ascending
                    My.Settings.eventLogSortingOrder = SortOrder.Ascending
                End If
            Else
                ' New column. Sort ascending.
                sort_order = SortOrder.Ascending
                My.Settings.eventLogSortingOrder = SortOrder.Ascending
            End If

            ' Remove the old sort indicator.
            m_SortingColumn.Text = m_SortingColumn.Text.Substring(2)
        End If

        ' Display the new sort order.
        m_SortingColumn = new_sorting_column
        If sort_order = SortOrder.Ascending Then
            m_SortingColumn.Text = "> " & m_SortingColumn.Text
        Else
            m_SortingColumn.Text = "< " & m_SortingColumn.Text
        End If

        ' Create a comparer.
        eventLogList.ListViewItemSorter = New Functions.listViewSorter.ListViewComparer(e.Column, sort_order)

        ' Sort.
        eventLogList.Sort()
    End Sub

    Private Sub eventLogList_ColumnWidthChanged(sender As Object, e As ColumnWidthChangedEventArgs) Handles eventLogList.ColumnWidthChanged
        If boolDoneLoading = True Then
            My.Settings.eventLogColumn1Size = ColumnHeader1.Width
            My.Settings.eventLogColumn2Size = ColumnHeader2.Width
            My.Settings.eventLogColumn3Size = ColumnHeader3.Width
            My.Settings.Save()
        End If
    End Sub

    Private Sub btnRefreshEvents_Click(sender As Object, e As EventArgs) Handles btnRefreshEvents.Click
        If Not boolAreWeLoadingTheEventLogData Then
            openPleaseWaitPanel("Loading Event Log Data... Please Wait.")

            workingThread = New Threading.Thread(AddressOf loadEventLog)
            workingThread.Name = "Event Log Data Loading Thread"
            workingThread.IsBackground = True
            workingThread.Start()
        End If
    End Sub

    Private Sub eventLogForm_ResizeEnd(sender As Object, e As EventArgs) Handles Me.ResizeEnd
        My.Settings.eventLogWindowSize = Me.Size
    End Sub

    Private Sub SplitContainer1_SplitterMoved(sender As Object, e As SplitterEventArgs) Handles SplitContainer1.SplitterMoved
        If SplitContainer1.SplitterDistance < 418 Then
            SplitContainer1.SplitterDistance = 418
            Exit Sub
        End If

        Try
            If boolDoneLoading = True Then My.Settings.eventLogSplitDistance = SplitContainer1.SplitterDistance
        Catch ex As Exception
        End Try
    End Sub

    Private Sub eventLogForm_ResizeBegin(sender As Object, e As EventArgs) Handles Me.ResizeBegin
        oldSplitterDifference = SplitContainer1.SplitterDistance
    End Sub

    Private Sub eventLogForm_Resize(sender As Object, e As EventArgs) Handles Me.Resize
        Try
            SplitContainer1.SplitterDistance = oldSplitterDifference
        Catch ex As Exception
        End Try
    End Sub

    Private Sub eventLogForm_Shown(sender As Object, e As EventArgs) Handles Me.Shown
        Me.Icon = My.Resources.RestorePoint_noBackground_2

        Me.Size = My.Settings.eventLogWindowSize
        SplitContainer1.SplitterDistance = My.Settings.eventLogSplitDistance

        imageList.Images.Add("Error", My.Resources.errorIcon)
        imageList.Images.Add("Information", My.Resources.informationIcon)
        imageList.Images.Add("Warning", My.Resources.warningIcon)

        ColumnHeader1.Width = My.Settings.eventLogColumn1Size
        ColumnHeader2.Width = My.Settings.eventLogColumn2Size
        ColumnHeader3.Width = My.Settings.eventLogColumn3Size

        boolDoneLoading = True

        openPleaseWaitPanel("Loading Event Log Data... Please Wait.")

        workingThread = New Threading.Thread(AddressOf loadEventLog)
        workingThread.Name = "Event Log Data Loading Thread"
        workingThread.IsBackground = True
        workingThread.Start()
    End Sub

    Private Sub chkAskMeToSubmitIfViewingAnExceptionEntry_Click(sender As Object, e As EventArgs) Handles chkAskMeToSubmitIfViewingAnExceptionEntry.Click
        My.Settings.boolAskMeToSubmitIfViewingAnExceptionEntry = chkAskMeToSubmitIfViewingAnExceptionEntry.Checked
    End Sub

    Private Sub eventLogList_SelectedIndexChanged(sender As Object, e As EventArgs) Handles eventLogList.SelectedIndexChanged
        Try
            If eventLogList.SelectedItems.Count <> 0 Then
                eventLogText.Text = DirectCast(eventLogList.SelectedItems(0), myListViewItemTypes.eventLogListEntry).strEventLogText

                If eventLogText.Text.caseInsensitiveContains("exception") And chkAskMeToSubmitIfViewingAnExceptionEntry.Checked And selectedIndex <> eventLogList.SelectedIndices(0) Then
                    If MsgBox("The log entry that you're looking at appears to be a program exception and stack trace. Would you like to submit it?", MsgBoxStyle.Question + MsgBoxStyle.YesNo + MsgBoxStyle.DefaultButton2, Me.Text) = MsgBoxResult.Yes Then
                        If (globalVariables.windows.frmManuallySubmitCrashDataInstance Is Nothing) Then
                            globalVariables.windows.frmManuallySubmitCrashDataInstance = New frmManuallySubmitCrashData
                            globalVariables.windows.frmManuallySubmitCrashDataInstance.StartPosition = FormStartPosition.CenterParent
                            globalVariables.windows.frmManuallySubmitCrashDataInstance.crashData = eventLogText.Text
                            globalVariables.windows.frmManuallySubmitCrashDataInstance.Show()

                            globalVariables.windows.frmManuallySubmitCrashDataInstance.Location = My.Settings.ManuallySubmitCrashDataInstanceLocation
                        Else
                            globalVariables.windows.frmManuallySubmitCrashDataInstance.BringToFront()
                            globalVariables.windows.frmManuallySubmitCrashDataInstance.crashData = eventLogText.Text
                        End If
                    End If
                End If

                selectedIndex = eventLogList.SelectedIndices(0)
            End If
        Catch ex As Exception
        End Try
    End Sub

    Private Sub btnSearch_Click(sender As Object, e As EventArgs) Handles btnSearch.Click
        Dim searchWindow As New Search_Event_Log
        searchWindow.StartPosition = FormStartPosition.CenterParent
        searchWindow.txtSearchTerms.Text = rawSearchTerms
        searchWindow.previousSearchType = previousSearchType
        searchWindow.ShowDialog(Me)

        If searchWindow.dialogResponse = Search_Event_Log.userResponse.doSearch Then
            Dim searchTerms As String = searchWindow.searchTerms

            rawSearchTerms = searchWindow.searchTerms
            previousSearchType = searchWindow.searchType

            Dim boolCaseInsensitive As Boolean = searchWindow.boolCaseInsensitive
            Dim boolUseRegEx As Boolean = searchWindow.boolUseRegEx
            Dim searchType As Search_Event_Log.searceType = searchWindow.searchType

            If boolUseRegEx = True And Functions.support.boolTestRegExPattern(searchTerms) = False Then
                MsgBox("Invalid RegEx Pattern.", MsgBoxStyle.Critical, Me.Text)
                Exit Sub
            End If

            searchWindow.Dispose()
            searchWindow = Nothing

            eventLogList.Items.Clear()

            For Each item As myListViewItemTypes.eventLogListEntry In eventLogContents
                With item
                    If boolUseRegEx Then
                        If searchType = Search_Event_Log.searceType.typeAny And .strEventLogText.regExSearch(searchTerms) Then
                            eventLogList.Items.Add(item)
                        ElseIf searchType = Search_Event_Log.searceType.typeError And .shortLevelType = EventLogEntryType.Error And .strEventLogText.regExSearch(searchTerms) Then
                            eventLogList.Items.Add(item)
                        ElseIf searchType = Search_Event_Log.searceType.typeInfo And .shortLevelType = EventLogEntryType.Information And .strEventLogText.regExSearch(searchTerms) Then
                            eventLogList.Items.Add(item)
                        End If
                    ElseIf boolCaseInsensitive Then
                        If searchType = Search_Event_Log.searceType.typeAny And .strEventLogText.caseInsensitiveContains(searchTerms) Then
                            eventLogList.Items.Add(item)
                        ElseIf searchType = Search_Event_Log.searceType.typeError And .shortLevelType = EventLogEntryType.Error And .strEventLogText.caseInsensitiveContains(searchTerms) Then
                            eventLogList.Items.Add(item)
                        ElseIf searchType = Search_Event_Log.searceType.typeInfo And .shortLevelType = EventLogEntryType.Information And .strEventLogText.caseInsensitiveContains(searchTerms) Then
                            eventLogList.Items.Add(item)
                        End If
                    Else
                        If searchType = Search_Event_Log.searceType.typeAny And .strEventLogText.Contains(searchTerms) Then
                            eventLogList.Items.Add(item)
                        ElseIf searchType = Search_Event_Log.searceType.typeError And .shortLevelType = EventLogEntryType.Error And .strEventLogText.Contains(searchTerms) Then
                            eventLogList.Items.Add(item)
                        ElseIf searchType = Search_Event_Log.searceType.typeInfo And .shortLevelType = EventLogEntryType.Information And .strEventLogText.Contains(searchTerms) Then
                            eventLogList.Items.Add(item)
                        End If
                    End If
                End With
            Next

            If eventLogList.Items.Count <> 0 Then
                Dim strEntriesFound As String
                If eventLogList.Items.Count = 1 Then
                    strEntriesFound = "1 log entry was found."
                Else
                    strEntriesFound = eventLogList.Items.Count.ToString & " log entries were found."
                End If

                MsgBox("Search complete. " & strEntriesFound, MsgBoxStyle.Information, Me.Text)
            Else
                MsgBox("Search complete. No results found.", MsgBoxStyle.Information, Me.Text)
            End If
        End If
    End Sub

    Private Sub btnExportLogs_Click(sender As Object, e As EventArgs) Handles btnExportLogs.Click
        Try
            SaveFileDialog1.Title = "Export Application Event Logs to File"
            SaveFileDialog1.FileName = Nothing
            SaveFileDialog1.Filter = "Restore Point Creator Exported XML Log File|*.reslogx"
            SaveFileDialog1.OverwritePrompt = True

            If SaveFileDialog1.ShowDialog = DialogResult.OK Then
                Dim jsonEngine As New Web.Script.Serialization.JavaScriptSerializer

                Dim logCount As ULong = 0
                Dim timeStamp As New Stopwatch
                timeStamp.Start()

                Functions.eventLogFunctions.exportLogsToFile(SaveFileDialog1.FileName, logCount)

                timeStamp.Stop()

                MsgBox(String.Format("{0} log entries have been successfully exported.{1}{1}Application Event Log exported in {2}ms ({3} seconds).", logCount, vbCrLf, timeStamp.ElapsedMilliseconds, Math.Round(timeStamp.Elapsed.TotalSeconds, 3)), MsgBoxStyle.Information, Me.Text)
            End If
        Catch ex As Exception
            Functions.eventLogFunctions.writeCrashToEventLog(ex)
            Functions.eventLogFunctions.writeToSystemEventLog("There was an error while attempting to export the program's event log entries.", EventLogEntryType.Error)

            MsgBox("There was an error while exporting the log data.", MsgBoxStyle.Critical, Me.Text)
            btnRefreshEvents.PerformClick()
        End Try
    End Sub

    Private Sub btnClear_Click(sender As Object, e As EventArgs) Handles btnClear.Click
        loadEventLogContentsIntoList()
    End Sub

    Sub loadEventLogContentsIntoList()
        rawSearchTerms = Nothing
        eventLogList.Items.Clear()
        eventLogList.Items.AddRange(eventLogContents.ToArray())
        eventLogList.Sort()
    End Sub

    Private Sub btnCleanLogFile_Click(sender As Object, e As EventArgs) Handles btnCleanLogFile.Click
        IO.File.Delete(Functions.eventLogFunctions.strLogFile)
        Functions.eventLogFunctions.writeToSystemEventLog(String.Format("Log file cleaned by user {0}.", Environment.UserName), EventLogEntryType.Information)
        loadEventLog()
    End Sub

    Private Sub logFileWatcher_Changed(sender As Object, e As IO.FileSystemEventArgs) Handles logFileWatcher.Changed
        If IO.File.Exists(Functions.eventLogFunctions.strLogFile) Then
            lblLogFileSize.Text = "Log File Size: " & Functions.support.bytesToHumanSize(New IO.FileInfo(Functions.eventLogFunctions.strLogFile).Length)
        Else
            lblLogFileSize.Text = "Log File Size: (File Doesn't Exist)"
        End If
    End Sub

#Region "--== Please Wait Panel Code ==--"
    Private strPleaseWaitLabelText As String

    Private Sub centerPleaseWaitPanel()
        pleaseWaitPanel.Location = New Point(
            (Me.ClientSize.Width / 2) - (pleaseWaitPanel.Size.Width / 2),
            (Me.ClientSize.Height / 2) - (pleaseWaitPanel.Size.Height / 2))
        pleaseWaitPanel.Anchor = AnchorStyles.None
    End Sub

    Private Sub openPleaseWaitPanel(strInputPleaseWaitLabelText As String)
        Functions.support.disableControlsOnForm(Me)

        pleaseWaitProgressBar.ProgressBarColor = My.Settings.barColor
        strPleaseWaitLabelText = strInputPleaseWaitLabelText
        pleaseWaitlblLabel.Text = strInputPleaseWaitLabelText
        centerPleaseWaitPanel()
        pleaseWaitPanel.Visible = True
        pleaseWaitProgressBar.Value = 0
        pleaseWaitProgressBarChanger.Enabled = True
        pleaseWaitMessageChanger.Enabled = True
        pleaseWaitBorderText.BackColor = globalVariables.pleaseWaitPanelColor
        pleaseWaitBorderText.ForeColor = globalVariables.pleaseWaitPanelFontColor
    End Sub

    Private Sub closePleaseWaitPanel()
        Functions.support.enableControlsOnForm(Me)

        If Not Functions.privilegeChecks.areWeAnAdministrator() Then btnCleanLogFile.Enabled = False

        pleaseWaitPanel.Visible = False
        pleaseWaitProgressBarChanger.Enabled = False
        pleaseWaitMessageChanger.Enabled = False
        pleaseWaitProgressBar.Value = 0
    End Sub

    Private Sub pleaseWaitProgressBarChanger_Tick(sender As Object, e As EventArgs) Handles pleaseWaitProgressBarChanger.Tick
        If pleaseWaitProgressBar.Value < 100 Then
            pleaseWaitProgressBar.Value += 1
        Else
            pleaseWaitProgressBar.Value = 0
        End If
    End Sub

    Private Sub pleaseWaitMessageChanger_Tick(sender As Object, e As EventArgs) Handles pleaseWaitMessageChanger.Tick
        If pleaseWaitBorderText.Text = "Please Wait..." Then
            pleaseWaitBorderText.Text = "Please Wait"
            pleaseWaitlblLabel.Text = strPleaseWaitLabelText
        ElseIf pleaseWaitBorderText.Text = "Please Wait" Then
            pleaseWaitBorderText.Text = "Please Wait."
            pleaseWaitlblLabel.Text = strPleaseWaitLabelText & "."
        ElseIf pleaseWaitBorderText.Text = "Please Wait." Then
            pleaseWaitBorderText.Text = "Please Wait.."
            pleaseWaitlblLabel.Text = strPleaseWaitLabelText & ".."
        ElseIf pleaseWaitBorderText.Text = "Please Wait.." Then
            pleaseWaitBorderText.Text = "Please Wait..."
            pleaseWaitlblLabel.Text = strPleaseWaitLabelText & "..."
        End If
    End Sub
#End Region
End Class