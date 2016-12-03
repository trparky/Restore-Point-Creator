Public Class eventLogForm
    Private m_SortingColumn As ColumnHeader
    Private eventLogLoadingThread As Threading.Thread
    Private boolDoneLoading As Boolean = False
    Private oldSplitterDifference As Integer

    Private cachedEventLogApplication As New Dictionary(Of Long, String)
    Private cachedEventLogCustom As New Dictionary(Of Long, String)

    Private rawSearchTerms As String = Nothing
    Private previousSearchType As Search_Event_Log.searceType

    Private selectedIndex As Long
    Private boolDidSortingChange As Boolean = False
    Private boolDidWeDoASearch As Boolean = False
    Private longEntriesFound As Long = 0

    Sub loadEventLogData(ByVal strEventLog As String, ByRef itemsToPutInToList As List(Of ListViewItem), ByRef cache As Dictionary(Of Long, String))
        Dim itemAdd As myListViewItemTypes.eventLogListEntry
        Dim eventLogQuery As Eventing.Reader.EventLogQuery
        Dim logReader As Eventing.Reader.EventLogReader
        Dim eventInstance As Eventing.Reader.EventRecord

        Try
            If EventLog.Exists(strEventLog) Then
                eventLogQuery = New Eventing.Reader.EventLogQuery(strEventLog, Eventing.Reader.PathType.LogName)
                logReader = New Eventing.Reader.EventLogReader(eventLogQuery)

                eventInstance = logReader.ReadEvent()

                While eventInstance IsNot Nothing
                    If eventInstance.ProviderName.stringCompare(globalVariables.eventLog.strSystemRestorePointCreator) Or eventInstance.ProviderName.caseInsensitiveContains(globalVariables.eventLog.strSystemRestorePointCreator) Then
                        cache.Add(eventInstance.RecordId, eventInstance.FormatDescription)

                        itemAdd = New myListViewItemTypes.eventLogListEntry()
                        Try
                            itemAdd.Text = eventInstance.LevelDisplayName
                        Catch ex As Eventing.Reader.EventLogNotFoundException
                            itemAdd.Text = "Unknown"
                        End Try

                        ' 0 = "Error"
                        ' 1 = "Information"
                        ' 2 = "Warning"
                        Select Case eventInstance.Level
                            Case Eventing.Reader.StandardEventLevel.Error ' Error
                                itemAdd.ImageIndex = 0
                            Case Eventing.Reader.StandardEventLevel.Informational ' Information
                                itemAdd.ImageIndex = 1
                            Case Eventing.Reader.StandardEventLevel.Warning ' Warning
                                itemAdd.ImageIndex = 2
                        End Select

                        itemAdd.SubItems.Add(eventInstance.TimeCreated.Value.ToLocalTime.ToString)
                        itemAdd.SubItems.Add(Long.Parse(eventInstance.RecordId).ToString("N0"))
                        itemAdd.SubItems.Add(strEventLog)
                        itemAdd.SubItems.Add("")

                        itemAdd.eventLogEntryID = Long.Parse(eventInstance.RecordId)
                        itemAdd.eventLogSource = strEventLog
                        itemAdd.eventLogLevel = eventInstance.Level

                        itemsToPutInToList.Add(itemAdd)
                        itemAdd = Nothing
                    End If

                    eventInstance.Dispose()
                    eventInstance = Nothing
                    eventInstance = logReader.ReadEvent()
                End While

                logReader.Dispose()
                logReader = Nothing
                eventLogQuery = Nothing
            End If
        Catch ex As Exception
            MsgBox(ex.Message)
            Functions.eventLogFunctions.writeCrashToEventLog(ex)
        End Try
    End Sub

    Sub loadEventLog()
        Me.Cursor = Cursors.WaitCursor
        Dim itemsToPutInToList As New List(Of ListViewItem)

        ' Cleans any possible existing data.
        cachedEventLogApplication.Clear()
        cachedEventLogCustom.Clear()

        Dim timeStamp As New Stopwatch
        timeStamp.Start()

        loadEventLogData(globalVariables.eventLog.strApplication, itemsToPutInToList, cachedEventLogApplication)
        loadEventLogData(globalVariables.eventLog.strSystemRestorePointCreator, itemsToPutInToList, cachedEventLogCustom)

        lblLogEntryCount.Text = "Entries in Event Log: " & itemsToPutInToList.Count.ToString("N0")
        eventLogList.Items.Clear()
        eventLogList.Items.AddRange(itemsToPutInToList.ToArray())
        eventLogList.Sort()

        Me.Cursor = Cursors.Default
        boolDoneLoading = True
        eventLogLoadingThread = Nothing

        Functions.wait.closePleaseWaitWindow()
        timeStamp.Stop()
        lblProcessedIn.Text = String.Format("Event Log Loaded and Processed in {0}ms ({1} seconds).", timeStamp.ElapsedMilliseconds.ToString("N0"), Math.Round(timeStamp.Elapsed.TotalSeconds, 2))
    End Sub

    Private Sub eventLogForm_FormClosing(sender As Object, e As FormClosingEventArgs) Handles Me.FormClosing
        My.Settings.eventLogFormWindowLocation = Me.Location

        ' Cleans any possible existing data.
        cachedEventLogApplication.Clear()
        cachedEventLogApplication = Nothing
        cachedEventLogCustom.Clear()
        cachedEventLogCustom = Nothing

        If globalVariables.windows.eventLogForm IsNot Nothing Then
            globalVariables.windows.eventLogForm.Dispose()
            globalVariables.windows.eventLogForm = Nothing
        End If

        If eventLogLoadingThread IsNot Nothing Then
            eventLogLoadingThread.Abort()
        End If
    End Sub

    Private Sub eventLogForm_KeyUp(sender As Object, e As KeyEventArgs) Handles Me.KeyUp
        If e.KeyCode = Keys.F5 Then
            If eventLogLoadingThread Is Nothing Then
                Functions.wait.createPleaseWaitWindow("Loading Event Log Data... Please Wait.", False, enums.howToCenterWindow.parent, False)
                Threading.ThreadPool.QueueUserWorkItem(AddressOf loadEventLog)
                Functions.wait.openPleaseWaitWindow()
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
        chkAskMeToSubmitIfViewingAnExceptionEntry.Checked = My.Settings.boolAskMeToSubmitIfViewingAnExceptionEntry
        Control.CheckForIllegalCrossThreadCalls = False
        applySavedSorting()
    End Sub

    Function getEventLogEntryDetails(id As Long, source As String) As String
        source = source.Trim

        If source = globalVariables.eventLog.strRestorePointCreator Then
            If cachedEventLogCustom.Keys.Contains(id) Then
                Return cachedEventLogCustom(id)
            Else
                Return Nothing
            End If
        ElseIf source = globalVariables.eventLog.strApplication Then
            If cachedEventLogApplication.Keys.Contains(id) Then
                Return cachedEventLogApplication(id)
            Else
                Return Nothing
            End If
        End If

        Return Nothing
    End Function

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
            If ColumnHeader5.Width <> 18 Then ColumnHeader5.Width = 18

            My.Settings.eventLogColumn1Size = ColumnHeader1.Width
            My.Settings.eventLogColumn2Size = ColumnHeader2.Width
            My.Settings.eventLogColumn3Size = ColumnHeader3.Width
            My.Settings.eventLogColumn4Size = ColumnHeader4.Width
            My.Settings.Save()
        End If
    End Sub

    Private Sub btnRefreshEvents_Click(sender As Object, e As EventArgs) Handles btnRefreshEvents.Click
        If eventLogLoadingThread Is Nothing Then
            Functions.wait.createPleaseWaitWindow("Loading Event Log Data... Please Wait.", False, enums.howToCenterWindow.parent, False)
            Threading.ThreadPool.QueueUserWorkItem(AddressOf loadEventLog)
            Functions.wait.openPleaseWaitWindow()
        End If
    End Sub

    Private Sub eventLogForm_ResizeEnd(sender As Object, e As EventArgs) Handles Me.ResizeEnd
        My.Settings.eventLogWindowSize = Me.Size
    End Sub

    Private Sub btnOpenEventLog_Click(sender As Object, e As EventArgs) Handles btnOpenEventLog.Click
        Process.Start("eventvwr")
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
        ColumnHeader4.Width = My.Settings.eventLogColumn4Size

        boolDoneLoading = True

        Functions.wait.createPleaseWaitWindow("Loading Event Log Data... Please Wait.", False, enums.howToCenterWindow.parent, False)
        Threading.ThreadPool.QueueUserWorkItem(AddressOf loadEventLog)
        Functions.wait.openPleaseWaitWindow()
    End Sub

    Private Sub chkAskMeToSubmitIfViewingAnExceptionEntry_Click(sender As Object, e As EventArgs) Handles chkAskMeToSubmitIfViewingAnExceptionEntry.Click
        My.Settings.boolAskMeToSubmitIfViewingAnExceptionEntry = chkAskMeToSubmitIfViewingAnExceptionEntry.Checked
    End Sub

    Private Sub eventLogList_SelectedIndexChanged(sender As Object, e As EventArgs) Handles eventLogList.SelectedIndexChanged
        Try
            If eventLogList.SelectedItems.Count <> 0 Then
                Dim eventID As Long = DirectCast(eventLogList.SelectedItems(0), myListViewItemTypes.eventLogListEntry).eventLogEntryID
                Dim source As String = DirectCast(eventLogList.SelectedItems(0), myListViewItemTypes.eventLogListEntry).eventLogSource

                eventLogText.Text = Functions.support.removeSourceCodePathInfo(getEventLogEntryDetails(eventID, source))

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

    Sub highlightItemInList(ByRef item As ListViewItem, ByRef longEntriesFound As Long)
        item.SubItems(4).Text = "*"
        item.BackColor = Color.LightBlue
        longEntriesFound += 1
    End Sub

    Private Sub btnSearch_Click(sender As Object, e As EventArgs) Handles btnSearch.Click
        clearSearchResults()

        If boolDidWeDoASearch Then
            eventLogList.BeginUpdate()

            For Each item As ListViewItem In eventLogList.Items
                If item.SubItems(4).Text.Equals("*") Then
                    item.SubItems(4).Text = ""
                    item.BackColor = eventLogList.BackColor
                End If
            Next

            eventLogList.EndUpdate()
            boolDidWeDoASearch = False
        End If

        Dim searchWindow As New Search_Event_Log
        searchWindow.StartPosition = FormStartPosition.CenterParent
        searchWindow.txtSearchTerms.Text = rawSearchTerms
        searchWindow.previousSearchType = previousSearchType
        searchWindow.ShowDialog(Me)

        If searchWindow.dialogResponse = Search_Event_Log.userResponse.doSearch Then
            Dim eventText As String
            Dim searchTerms As String = searchWindow.searchTerms

            rawSearchTerms = searchWindow.searchTerms
            previousSearchType = searchWindow.searchType

            Dim boolCaseInsensitive As Boolean = searchWindow.boolCaseInsensitive
            Dim boolUseRegEx As Boolean = searchWindow.boolUseRegEx
            Dim searchType As Search_Event_Log.searceType = searchWindow.searchType
            Dim strEventType As String

            If boolUseRegEx = True And Functions.support.boolTestRegExPattern(searchTerms) = False Then
                MsgBox("Invalid RegEx Pattern.", MsgBoxStyle.Critical, Me.Text)
                Exit Sub
            End If

            searchWindow.Dispose()
            searchWindow = Nothing

            longEntriesFound = 0
            boolDidWeDoASearch = True

            For Each item As ListViewItem In eventLogList.Items
                strEventType = item.SubItems.Item(0).Text.Trim
                eventText = getEventLogEntryDetails(Long.Parse(item.SubItems.Item(2).Text.Replace(",", "").Trim), item.SubItems.Item(3).Text.Trim)

                If boolUseRegEx = True Then
                    If searchType = Search_Event_Log.searceType.typeAny And eventText.regExSearch(searchTerms) Then
                        highlightItemInList(item, longEntriesFound)
                    ElseIf searchType = Search_Event_Log.searceType.typeError And strEventType = EventLogEntryType.Error.ToString And eventText.regExSearch(searchTerms) Then
                        highlightItemInList(item, longEntriesFound)
                    ElseIf searchType = Search_Event_Log.searceType.typeInfo And strEventType = EventLogEntryType.Information.ToString And eventText.regExSearch(searchTerms) Then
                        highlightItemInList(item, longEntriesFound)
                    End If
                ElseIf boolCaseInsensitive = True Then
                    If searchType = Search_Event_Log.searceType.typeAny And eventText.caseInsensitiveContains(searchTerms) Then
                        highlightItemInList(item, longEntriesFound)
                    ElseIf searchType = Search_Event_Log.searceType.typeError And strEventType = EventLogEntryType.Error.ToString And eventText.caseInsensitiveContains(searchTerms) Then
                        highlightItemInList(item, longEntriesFound)
                    ElseIf searchType = Search_Event_Log.searceType.typeInfo And strEventType = EventLogEntryType.Information.ToString And eventText.caseInsensitiveContains(searchTerms) Then
                        highlightItemInList(item, longEntriesFound)
                    End If
                Else
                    If searchType = Search_Event_Log.searceType.typeAny And eventText.Contains(searchTerms) Then
                        highlightItemInList(item, longEntriesFound)
                    ElseIf searchType = Search_Event_Log.searceType.typeError And strEventType = EventLogEntryType.Error.ToString And eventText.Contains(searchTerms) Then
                        highlightItemInList(item, longEntriesFound)
                    ElseIf searchType = Search_Event_Log.searceType.typeInfo And strEventType = EventLogEntryType.Information.ToString And eventText.Contains(searchTerms) Then
                        highlightItemInList(item, longEntriesFound)
                    End If
                End If
            Next

            If longEntriesFound <> 0 Then
                eventLogList.ListViewItemSorter = New Functions.listViewSorter.ListViewComparer(4, SortOrder.Descending)
                eventLogList.Sort()
                eventLogList.EnsureVisible(0)
                boolDidSortingChange = True

                Dim strEntriesFound As String
                If longEntriesFound = 1 Then
                    strEntriesFound = "1 log entry was found."
                Else
                    strEntriesFound = longEntriesFound & " log entries were found."
                End If

                MsgBox("Search complete. " & strEntriesFound & " The event log entries that contain your search terms have been highlighted in blue.", MsgBoxStyle.Information, Me.Text)
            Else
                MsgBox("Search complete. No results found.", MsgBoxStyle.Information, Me.Text)
            End If
        End If
    End Sub

    Private Sub btnExportLogs_Click(sender As Object, e As EventArgs) Handles btnExportLogs.Click
        Try
            SaveFileDialog1.Title = "Export Application Event Logs to File"
            SaveFileDialog1.FileName = Nothing
            SaveFileDialog1.Filter = "Restore Point Creator Exported Log File|*.reslog"
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
        clearSearchResults()
    End Sub

    Sub clearSearchResults()
        If boolDidSortingChange Then
            rawSearchTerms = Nothing

            For Each item As ListViewItem In eventLogList.Items
                item.SubItems(4).Text = ""
                item.BackColor = eventLogList.BackColor
            Next

            eventLogList.ListViewItemSorter = New Functions.listViewSorter.ListViewComparer(1, SortOrder.Descending)
            eventLogList.Sort()

            boolDidSortingChange = False
        End If
    End Sub
End Class