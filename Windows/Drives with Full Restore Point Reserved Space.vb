Imports System.Text

Public Class Drives_with_Full_Restore_Point_Reserved_Space
    Public Property drives As Specialized.StringCollection
    Private Const gigabytesInBytes As Long = 1073741824

    Private Sub Drives_with_Full_Restore_Point_Reserved_Space_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        Try
            'drives = New Specialized.StringCollection
            'drives.Add("C:")
            'drives.Add("S:")
            'drives.Add("T:")

            If drives IsNot Nothing Then
                Dim label1 As Label
                Dim autoFixButton, manualFixButton As Button
                Dim yPosition As Integer = 10
                Dim shadowStorageData As Functions.ShadowStorageData
                Dim shadowStorageUsePercentage, shadowStorageFreeSpacePercentage As Double
                Dim shadowStorageFreeSpace As Long
                Dim deviceID As String

                For Each drive As String In drives
                    'debug.writeline(drive)
                    deviceID = Functions.getDeviceIDFromDriveLetter(drive)
                    shadowStorageData = Functions.getShadowStorageData(deviceID)

                    If shadowStorageData IsNot Nothing Then
                        shadowStorageUsePercentage = Math.Round((shadowStorageData.UsedSpace / shadowStorageData.MaxSpace) * 100, 2)
                        shadowStorageFreeSpace = shadowStorageData.MaxSpace - shadowStorageData.UsedSpace
                        shadowStorageFreeSpacePercentage = 100 - shadowStorageUsePercentage

                        label1 = New Label
                        label1.Location = New System.Drawing.Point(1, yPosition)
                        label1.Font = New Font(label1.Font, FontStyle.Bold)
                        label1.Show()
                        label1.AutoSize = True
                        label1.Text = String.Format("Drive {0} ({1}): {2} of which {3} ({4}%) is used.  Available Space: {5} ({6}%)", drive.Replace(":", ""), getDriveLabelFromDriveLetter(drive), Functions.fileSizeToHumanReadableFormat(shadowStorageData.MaxSpace), Functions.fileSizeToHumanReadableFormat(shadowStorageData.UsedSpace), Math.Round(shadowStorageUsePercentage, 2), Functions.fileSizeToHumanReadableFormat(shadowStorageFreeSpace), Math.Round(shadowStorageFreeSpacePercentage, 2))
                        Me.GroupBox1.Controls.Add(label1)

                        autoFixButton = New Button
                        autoFixButton.Location = New System.Drawing.Point(label1.Width + 10, yPosition)
                        autoFixButton.Text = "Auto Fix"
                        autoFixButton.AutoSize = True
                        autoFixButton.Show()
                        Me.GroupBox1.Controls.Add(autoFixButton)
                        AddHandler autoFixButton.Click, Sub() autoFixSub(drive)

                        manualFixButton = New Button
                        manualFixButton.Location = New System.Drawing.Point(label1.Width + 90, yPosition)
                        manualFixButton.Text = "Fix Manually"
                        manualFixButton.AutoSize = True
                        manualFixButton.Show()
                        Me.GroupBox1.Controls.Add(manualFixButton)
                        AddHandler manualFixButton.Click, Sub() manualFixSub(drive)

                        yPosition += 40
                    End If
                Next
            Else
                Me.Close()
            End If
        Catch ex As Exception
            Threading.Thread.CurrentThread.CurrentUICulture = New System.Globalization.CultureInfo("en-US")
            exceptionHandler.manuallyLoadCrashWindow(ex, ex.Message, ex.StackTrace, ex.GetType)
        End Try
    End Sub

    Sub autoFixSub(drive As String)
        Try
            Dim deviceID As String = Functions.getDeviceIDFromDriveLetter(drive)
            Dim shadowStorageData As Functions.ShadowStorageData = Functions.getShadowStorageData(deviceID)
            Dim totalDriveSize As Long = getTotalDriveSize(drive)

            Dim oldShadowStorageSize As Long = shadowStorageData.MaxSpace
            Dim newShadowStorageSize As Long
            Dim stringBuilder As New StringBuilder

            If (oldShadowStorageSize + (gigabytesInBytes * 20)) < totalDriveSize Then
                newShadowStorageSize = oldShadowStorageSize + (gigabytesInBytes * 20)
                Functions.setShadowStorageSize(drive, newShadowStorageSize)

                stringBuilder.AppendLine("Auto Fixing of Reserved Restore Point Disk Space Usage complete.")
                stringBuilder.AppendLine()
                stringBuilder.AppendLine("Old Size: " & Functions.fileSizeToHumanReadableFormat(oldShadowStorageSize))
                stringBuilder.AppendLine("New Size: " & Functions.fileSizeToHumanReadableFormat(newShadowStorageSize))
                stringBuilder.AppendLine("Size Increased By: 20 GBs")

                MsgBox(stringBuilder.ToString.Trim, MsgBoxStyle.Information, Me.Text)

                Exit Sub
            ElseIf (oldShadowStorageSize + (gigabytesInBytes * 15)) < totalDriveSize Then
                newShadowStorageSize = oldShadowStorageSize + (gigabytesInBytes * 15)
                Functions.setShadowStorageSize(drive, newShadowStorageSize)

                stringBuilder.AppendLine("Auto Fixing of Reserved Restore Point Disk Space Usage complete.")
                stringBuilder.AppendLine()
                stringBuilder.AppendLine("Old Size: " & Functions.fileSizeToHumanReadableFormat(oldShadowStorageSize))
                stringBuilder.AppendLine("New Size: " & Functions.fileSizeToHumanReadableFormat(newShadowStorageSize))
                stringBuilder.AppendLine("Size Increased By: 15 GBs")

                MsgBox(stringBuilder.ToString.Trim, MsgBoxStyle.Information, Me.Text)

                Exit Sub
            ElseIf (oldShadowStorageSize + (gigabytesInBytes * 10)) < totalDriveSize Then
                newShadowStorageSize = oldShadowStorageSize + (gigabytesInBytes * 10)
                Functions.setShadowStorageSize(drive, newShadowStorageSize)

                stringBuilder.AppendLine("Auto Fixing of Reserved Restore Point Disk Space Usage complete.")
                stringBuilder.AppendLine()
                stringBuilder.AppendLine("Old Size: " & Functions.fileSizeToHumanReadableFormat(oldShadowStorageSize))
                stringBuilder.AppendLine("New Size: " & Functions.fileSizeToHumanReadableFormat(newShadowStorageSize))
                stringBuilder.AppendLine("Size Increased By: 10 GBs")

                MsgBox(stringBuilder.ToString.Trim, MsgBoxStyle.Information, Me.Text)

                Exit Sub
            ElseIf (oldShadowStorageSize + (gigabytesInBytes * 5)) < totalDriveSize Then
                newShadowStorageSize = oldShadowStorageSize + (gigabytesInBytes * 5)
                Functions.setShadowStorageSize(drive, newShadowStorageSize)

                stringBuilder.AppendLine("Auto Fixing of Reserved Restore Point Disk Space Usage complete.")
                stringBuilder.AppendLine()
                stringBuilder.AppendLine("Old Size: " & Functions.fileSizeToHumanReadableFormat(oldShadowStorageSize))
                stringBuilder.AppendLine("New Size: " & Functions.fileSizeToHumanReadableFormat(newShadowStorageSize))
                stringBuilder.AppendLine("Size Increased By: 5 GBs")

                MsgBox(stringBuilder.ToString.Trim, MsgBoxStyle.Information, Me.Text)

                Exit Sub
            Else
                MsgBox("Auto Fixing of Reserved Restore Point Disk Space Usage can't be performed, please do so manually.", MsgBoxStyle.Information, Me.Text)
                manualFixSub(drive)
            End If
        Catch ex As Exception
            MsgBox("Auto Fixing of Reserved Restore Point Disk Space Usage can't be performed, please do so manually.", MsgBoxStyle.Information, Me.Text)
            manualFixSub(drive)
        End Try
    End Sub

    Function getTotalDriveSize(drive As String) As Long
        For Each currentDrive As IO.DriveInfo In My.Computer.FileSystem.Drives
            If currentDrive.RootDirectory.ToString.Replace("\", "") = drive Then
                Return currentDrive.TotalSize
            End If
        Next

        Return 0
    End Function

    Sub manualFixSub(drive As String)
        If (windowInstances.frmManageSystemRestoreStorageSpace Is Nothing) Then
            windowInstances.frmManageSystemRestoreStorageSpace = New frmManageSystemRestoreStorageSpace
            'windowInstances.frmManageSystemRestoreStorageSpace.Icon = Me.Icon
            windowInstances.frmManageSystemRestoreStorageSpace.preSelectedDriveLetter = drive
            windowInstances.frmManageSystemRestoreStorageSpace.Show()
        Else
            windowInstances.frmManageSystemRestoreStorageSpace.selectDriveLetterInList(drive)
            windowInstances.frmManageSystemRestoreStorageSpace.BringToFront()
        End If
    End Sub

    Function getDriveLabelFromDriveLetter(letter As String) As String
        For Each driveInfo In My.Computer.FileSystem.Drives
            If driveInfo.RootDirectory.ToString.Replace("\", "") = letter Then
                Return driveInfo.VolumeLabel
            End If
        Next

        Return ""
    End Function

    Private Sub Drives_with_Full_Restore_Point_Reserved_Space_Resize(sender As Object, e As EventArgs) Handles Me.Resize
        If Me.Width <> 900 Then Me.Width = 900
        ''debug.writeline("Me.Width = " & Me.Width)
    End Sub
End Class