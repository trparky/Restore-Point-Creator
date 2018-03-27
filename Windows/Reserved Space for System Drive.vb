Public Class Reserved_Space_for_System_Drive
    Private Const kilobytesInBytes As Long = 1024
    Private Const megabytesInBytes As Long = 1048576
    Private Const gigabytesInBytes As Long = 1073741824
    Private Const terabytesInBytes As Long = 1099511627776

    Function getTotalDriveSize(drive As String) As Long
        For Each currentDrive As IO.DriveInfo In My.Computer.FileSystem.Drives
            If currentDrive.RootDirectory.ToString.Replace("\", "").Equals(drive, StringComparison.OrdinalIgnoreCase) Then Return currentDrive.TotalSize
        Next

        Return 0
    End Function

    Private Sub btnAutoFix_Click(sender As Object, e As EventArgs) Handles btnAutoFix.Click
        Try
            Dim stringBuilder As New Text.StringBuilder
            Dim boolResult As Boolean
            Dim deviceID As String = Functions.wmi.getDeviceIDFromDriveLetter(globalVariables.systemDriveLetter, boolResult)

            If boolResult = True Then
                If My.Settings.debug Then Functions.eventLogFunctions.writeToApplicationLogFile(String.Format("EXTENDED DEBUG MESSAGE{0}DeviceID for boot drive has been detected as {1}.", vbCrLf, deviceID), EventLogEntryType.Information, False)

                Dim totalDriveSize As Long = getTotalDriveSize(globalVariables.systemDriveLetter)
                Dim oldShadowStorageSize As Long = Functions.vss.getMaxSize(globalVariables.systemDriveLetter)
                Dim newShadowStorageSize As Long

                If My.Settings.debug Then Functions.eventLogFunctions.writeToApplicationLogFile(String.Format("EXTENDED DEBUG MESSAGE{0}Total system drive size was detected as {1} and the old reserved space was detected as {2}.", vbCrLf, Functions.support.bytesToHumanSize(totalDriveSize), Functions.support.bytesToHumanSize(oldShadowStorageSize)), EventLogEntryType.Information, False)

                If (oldShadowStorageSize + (gigabytesInBytes * 20)) < totalDriveSize Then
                    newShadowStorageSize = oldShadowStorageSize + (gigabytesInBytes * 20)
                    Functions.vss.setShadowStorageSize(globalVariables.systemDriveLetter, newShadowStorageSize)

                    stringBuilder.AppendLine("Auto Fixing of Reserved Restore Point Disk Space Usage complete.")
                    stringBuilder.AppendLine()
                    stringBuilder.AppendLine("Old Size: " & Functions.support.bytesToHumanSize(oldShadowStorageSize))
                    stringBuilder.AppendLine("New Size: " & Functions.support.bytesToHumanSize(newShadowStorageSize))
                    stringBuilder.AppendLine("Size Increased By: 20 GBs")

                    MsgBox(stringBuilder.ToString.Trim, MsgBoxStyle.Information, Me.Text)

                    Me.Close()
                    Exit Sub
                ElseIf (oldShadowStorageSize + (gigabytesInBytes * 15)) < totalDriveSize Then
                    newShadowStorageSize = oldShadowStorageSize + (gigabytesInBytes * 15)
                    Functions.vss.setShadowStorageSize(globalVariables.systemDriveLetter, newShadowStorageSize)

                    stringBuilder.AppendLine("Auto Fixing of Reserved Restore Point Disk Space Usage complete.")
                    stringBuilder.AppendLine()
                    stringBuilder.AppendLine("Old Size: " & Functions.support.bytesToHumanSize(oldShadowStorageSize))
                    stringBuilder.AppendLine("New Size: " & Functions.support.bytesToHumanSize(newShadowStorageSize))
                    stringBuilder.AppendLine("Size Increased By: 15 GBs")

                    MsgBox(stringBuilder.ToString.Trim, MsgBoxStyle.Information, Me.Text)

                    Me.Close()
                    Exit Sub
                ElseIf (oldShadowStorageSize + (gigabytesInBytes * 10)) < totalDriveSize Then
                    newShadowStorageSize = oldShadowStorageSize + (gigabytesInBytes * 10)
                    Functions.vss.setShadowStorageSize(globalVariables.systemDriveLetter, newShadowStorageSize)

                    stringBuilder.AppendLine("Auto Fixing of Reserved Restore Point Disk Space Usage complete.")
                    stringBuilder.AppendLine()
                    stringBuilder.AppendLine("Old Size: " & Functions.support.bytesToHumanSize(oldShadowStorageSize))
                    stringBuilder.AppendLine("New Size: " & Functions.support.bytesToHumanSize(newShadowStorageSize))
                    stringBuilder.AppendLine("Size Increased By: 10 GBs")

                    MsgBox(stringBuilder.ToString.Trim, MsgBoxStyle.Information, Me.Text)

                    Me.Close()
                    Exit Sub
                ElseIf (oldShadowStorageSize + (gigabytesInBytes * 5)) < totalDriveSize Then
                    newShadowStorageSize = oldShadowStorageSize + (gigabytesInBytes * 5)
                    Functions.vss.setShadowStorageSize(globalVariables.systemDriveLetter, newShadowStorageSize)

                    stringBuilder.AppendLine("Auto Fixing of Reserved Restore Point Disk Space Usage complete.")
                    stringBuilder.AppendLine()
                    stringBuilder.AppendLine("Old Size: " & Functions.support.bytesToHumanSize(oldShadowStorageSize))
                    stringBuilder.AppendLine("New Size: " & Functions.support.bytesToHumanSize(newShadowStorageSize))
                    stringBuilder.AppendLine("Size Increased By: 5 GBs")

                    MsgBox(stringBuilder.ToString.Trim, MsgBoxStyle.Information, Me.Text)

                    Me.Close()
                    Exit Sub
                Else
                    MsgBox("Auto Fixing of Reserved Restore Point Disk Space Usage can't be performed, please do so manually.", MsgBoxStyle.Information, Me.Text)
                    manualFixSub(globalVariables.systemDriveLetter)
                End If
            Else
                MsgBox("Unable to retrieve DeviceID for system drive.", MsgBoxStyle.Critical, Me.Text)
            End If
        Catch ex As Exception
            MsgBox("Auto Fixing of Reserved Restore Point Disk Space Usage can't be performed, please do so manually.", MsgBoxStyle.Information, Me.Text)
            manualFixSub(globalVariables.systemDriveLetter)
        End Try
    End Sub

    Sub manualFixSub(drive As String)
        If (globalVariables.windows.frmManageSystemRestoreStorageSpace Is Nothing) Then
            globalVariables.windows.frmManageSystemRestoreStorageSpace = New frmManageSystemRestoreStorageSpace
            globalVariables.windows.frmManageSystemRestoreStorageSpace.strDriveLetterWeAreWorkingWith = drive
            globalVariables.windows.frmManageSystemRestoreStorageSpace.Show()
        Else
            globalVariables.windows.frmManageSystemRestoreStorageSpace.BringToFront()
        End If
    End Sub

    Private Sub btnManuallyFix_Click(sender As Object, e As EventArgs) Handles btnManuallyFix.Click
        manualFixSub(globalVariables.systemDriveLetter)
        Me.Close()
    End Sub

    Private Sub btnClose_Click(sender As Object, e As EventArgs) Handles btnClose.Click
        Me.Close()
    End Sub

    Private Sub Reserved_Space_for_System_Drive_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        Me.BringToFront()
        Label1.Text = String.Format(Label1.Text, globalVariables.systemDriveLetter)
    End Sub
End Class