Imports System.Management

Public Class Mount_Volume_Shadow_Copy
    Private shadowCopyCache As New Dictionary(Of Integer, String)

    Private Sub Mount_Volume_Shadow_Copy_FormClosing(sender As Object, e As FormClosingEventArgs) Handles Me.FormClosing
        My.Settings.mountVolumeShadowCopyWindowPosition = Me.Location
        If IO.Directory.Exists(globalVariables.shadowCopyMountFolder) Then IO.Directory.Delete(globalVariables.shadowCopyMountFolder)

        globalVariables.windows.mountVolumeShadowCopy.Dispose()
        globalVariables.windows.mountVolumeShadowCopy = Nothing
    End Sub

    Sub loadSnapshots()
        Dim searcher As New ManagementObjectSearcher("root\CIMV2", "SELECT * FROM Win32_ShadowCopy")

        listShadowCopyIDs.Items.Clear()
        shadowCopyCache.Clear()

        Dim timeCreated As Date, index As Integer

        For Each queryObj As ManagementObject In searcher.Get()
            timeCreated = Functions.support.parseSystemRestorePointCreationDate(queryObj("InstallDate").ToString).ToUniversalTime

            index = listShadowCopyIDs.Items.Add((timeCreated.ToLongDateString & " at " & timeCreated.ToLongTimeString).Trim)
            shadowCopyCache.Add(index, queryObj("DeviceObject").ToString)

            Debug.WriteLine("shadow copy info | " & index & " | " & queryObj("DeviceObject").ToString)

            timeCreated = Nothing
        Next

        searcher.Dispose()
        searcher = Nothing
    End Sub

    Private Sub Mount_Volume_Shadow_Copy_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        lblMainLabel.Text = String.Format(lblMainLabel.Text, Environment.GetFolderPath(Environment.SpecialFolder.Windows).Substring(0, 3).ToUpper)
        loadSnapshots()
    End Sub

    Private Sub listShadowCopyIDs_SelectedIndexChanged(sender As Object, e As EventArgs) Handles listShadowCopyIDs.SelectedIndexChanged
        If IO.Directory.Exists(globalVariables.shadowCopyMountFolder) Then IO.Directory.Delete(globalVariables.shadowCopyMountFolder)

        btnUnmount.Enabled = False
        btnMount.Enabled = True

        Debug.WriteLine("shadow copy info | " & listShadowCopyIDs.SelectedIndex & " | " & shadowCopyCache(listShadowCopyIDs.SelectedIndex))
    End Sub

    Private Sub btnMount_Click(sender As Object, e As EventArgs) Handles btnMount.Click
        Try
            If IO.Directory.Exists(globalVariables.shadowCopyMountFolder) Then IO.Directory.Delete(globalVariables.shadowCopyMountFolder)

            If shadowCopyCache.ContainsKey(listShadowCopyIDs.SelectedIndex) = True Then
                Dim deviceID As String = shadowCopyCache(listShadowCopyIDs.SelectedIndex) & "\"

                Dim mountProcessInfo As New ProcessStartInfo
                mountProcessInfo.FileName = IO.Path.Combine(GlobalVariables.strPathToSystemFolder, "cmd.exe")
                mountProcessInfo.Arguments = String.Format("/c mklink /d {0}{1}{0} {0}{2}{0}", Chr(34), globalVariables.shadowCopyMountFolder, deviceID) '"/c mklink /d C:\shadowcopy """ & deviceID & """"
                mountProcessInfo.Verb = "runas"
                mountProcessInfo.CreateNoWindow = True
                mountProcessInfo.WindowStyle = ProcessWindowStyle.Hidden

                Dim mountProcess As Process = Process.Start(mountProcessInfo)
                mountProcess.WaitForExit()
                mountProcessInfo = Nothing

                Process.Start(IO.Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Windows), "explorer.exe"), globalVariables.shadowCopyMountFolder)
                btnUnmount.Enabled = True
            Else
                MsgBox("Something went wrong, unable to find entry in shadowCopyCache Object.", MsgBoxStyle.Critical, Me.Text)
            End If
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
End Class