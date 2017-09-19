Imports System.Management

Public Class Mount_Volume_Shadow_Copy
    Private m_SortingColumn As ColumnHeader

    Private Sub Mount_Volume_Shadow_Copy_FormClosing(sender As Object, e As FormClosingEventArgs) Handles Me.FormClosing
        My.Settings.mountVolumeShadowCopyWindowPosition = Me.Location
        If IO.Directory.Exists(globalVariables.shadowCopyMountFolder) Then IO.Directory.Delete(globalVariables.shadowCopyMountFolder)

        globalVariables.windows.mountVolumeShadowCopy.Dispose()
        globalVariables.windows.mountVolumeShadowCopy = Nothing
    End Sub

    Sub loadSnapshots()
        Dim searcher As New ManagementObjectSearcher("root\CIMV2", "SELECT * FROM Win32_ShadowCopy")

        listShadowCopyIDs.Items.Clear()

        Dim timeCreated As Date

        For Each queryObj As ManagementObject In searcher.Get()
            timeCreated = Functions.restorePointStuff.parseSystemRestorePointCreationDate(queryObj("InstallDate").ToString).ToUniversalTime

            listShadowCopyIDs.Items.Add(New myListViewItemTypes.volumeShadowCopyListItem() With {
                                            .Text = (timeCreated.ToLongDateString & " at " & timeCreated.ToLongTimeString).Trim,
                                            .deviceID = queryObj("DeviceObject").ToString,
                                            .dateCreated = timeCreated
                                        })

            timeCreated = Nothing
        Next

        searcher.Dispose()
        searcher = Nothing
    End Sub

    Private Sub Mount_Volume_Shadow_Copy_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        Me.Location = Functions.support.verifyWindowLocation(My.Settings.mountVolumeShadowCopyWindowPosition)
        lblMainLabel.Text = String.Format(lblMainLabel.Text, Environment.GetFolderPath(Environment.SpecialFolder.Windows).Substring(0, 3).ToUpper)
        loadSnapshots()
    End Sub

    Private Sub listShadowCopyIDs_SelectedIndexChanged(sender As Object, e As EventArgs) Handles listShadowCopyIDs.SelectedIndexChanged
        If IO.Directory.Exists(globalVariables.shadowCopyMountFolder) Then IO.Directory.Delete(globalVariables.shadowCopyMountFolder)

        If listShadowCopyIDs.SelectedItems.Count = 0 Then
            btnMount.Enabled = False
        Else
            btnMount.Enabled = True
        End If

        btnUnmount.Enabled = False
    End Sub

    Private Sub btnMount_Click(sender As Object, e As EventArgs) Handles btnMount.Click
        Try
            If IO.Directory.Exists(globalVariables.shadowCopyMountFolder) Then IO.Directory.Delete(globalVariables.shadowCopyMountFolder)

            Dim deviceID As String = DirectCast(listShadowCopyIDs.SelectedItems(0), myListViewItemTypes.volumeShadowCopyListItem).deviceID & "\"

            Dim mountProcessInfo As New ProcessStartInfo
            mountProcessInfo.FileName = IO.Path.Combine(globalVariables.strPathToSystemFolder, "cmd.exe")
            mountProcessInfo.Arguments = String.Format("/c mklink /d {0}{1}{0} {0}{2}{0}", Chr(34), globalVariables.shadowCopyMountFolder, deviceID) '"/c mklink /d C:\shadowcopy """ & deviceID & """"
            mountProcessInfo.Verb = "runas"
            mountProcessInfo.CreateNoWindow = True
            mountProcessInfo.WindowStyle = ProcessWindowStyle.Hidden

            Dim mountProcess As Process = Process.Start(mountProcessInfo)
            mountProcess.WaitForExit()
            mountProcessInfo = Nothing

            Process.Start(IO.Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Windows), "explorer.exe"), globalVariables.shadowCopyMountFolder)
            btnUnmount.Enabled = True
        Catch ex As NullReferenceException
            Functions.eventLogFunctions.writeCrashToEventLog(ex)
            Functions.eventLogFunctions.writeToSystemEventLog("Something went wrong, unable to find entry in shadowCopyCache Object.", EventLogEntryType.Error)

            MsgBox("Something went wrong, unable to find entry in shadowCopyCache Object.", MsgBoxStyle.Critical, Me.Text)
        End Try
    End Sub

    Private Sub btnUnmount_Click(sender As Object, e As EventArgs) Handles btnUnmount.Click
        If IO.Directory.Exists(globalVariables.shadowCopyMountFolder) Then IO.Directory.Delete(globalVariables.shadowCopyMountFolder)
        btnUnmount.Enabled = False
    End Sub

    Private Sub btnRefreshList_Click(sender As Object, e As EventArgs) Handles btnRefreshList.Click
        loadSnapshots()
    End Sub

    Private Sub listShadowCopyIDs_ColumnClick(sender As Object, e As ColumnClickEventArgs) Handles listShadowCopyIDs.ColumnClick
        ' Get the new sorting column.
        Dim new_sorting_column As ColumnHeader = listShadowCopyIDs.Columns(e.Column)
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
        listShadowCopyIDs.ListViewItemSorter = New Functions.listViewSorter.ListViewComparer(e.Column, sort_order)

        ' Sort.
        listShadowCopyIDs.Sort()
    End Sub
End Class