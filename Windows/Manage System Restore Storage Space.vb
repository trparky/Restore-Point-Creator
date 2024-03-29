﻿Imports System.Text.RegularExpressions

Public Class frmManageSystemRestoreStorageSpace
    Private Const kilobytesInBytes As Long = 1024
    Private Const megabytesInBytes As Long = 1048576
    Private Const gigabytesInBytes As Long = 1073741824
    Private Const terabytesInBytes As Long = 1099511627776

    Public strDriveLetterWeAreWorkingWith As String

    Private Sub frmManageSystemRestoreStorageSpace_FormClosing(sender As Object, e As FormClosingEventArgs) Handles Me.FormClosing
        My.Settings.ManageSystemRestoreStorageSpaceWindowLocation = Me.Location
        globalVariables.windows.frmManageSystemRestoreStorageSpace.Dispose()
        globalVariables.windows.frmManageSystemRestoreStorageSpace = Nothing
    End Sub

    Function getSizeOfDrive(strDriveLetter As String) As Long
        Dim strDriveLetterInLoop As String

        For Each currentDrive As IO.DriveInfo In My.Computer.FileSystem.Drives
            Try
                strDriveLetterInLoop = currentDrive.RootDirectory.ToString.Replace("\", "")

                If strDriveLetter.ToUpper.Trim = strDriveLetterInLoop.ToUpper.Trim Then
                    Return currentDrive.TotalSize
                End If
            Catch ex As Exception
                Return 0
            End Try
        Next

        Return 0
    End Function

    Sub loadDriveData(strDrive As String)
        percentageIndicator.Value = 0
        getSize(strDrive)

        lblDriveSize.Text = String.Format("Total Size of Drive {0} {1}", strDrive, Functions.support.bytesToHumanSize(getSizeOfDrive(strDrive)))

        btnSetSize.Text = String.Format("Set Size for System Drive ({0})", strDrive)
        btnSetSize.Enabled = True

        strDriveLetterWeAreWorkingWith = strDrive
    End Sub

    Private Sub frmManageSystemRestoreStorageSpace_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        Dim strDriveWeAreLoadingDataFor As String = globalVariables.systemDriveLetter

        If Not String.IsNullOrEmpty(strDriveLetterWeAreWorkingWith) Then
            strDriveWeAreLoadingDataFor = strDriveLetterWeAreWorkingWith
            showDrivesPanel()
            chkAdvancedMode.Checked = True
        Else
            hideDrivesPanel()
        End If

        listDrives.Items.Clear()

        For Each drive As IO.DriveInfo In My.Computer.FileSystem.Drives
            If drive.IsReady And drive.DriveType = IO.DriveType.Fixed Then
                listDrives.Items.Add(drive.RootDirectory.FullName.Substring(0, 2))
            End If
        Next

        If chkAdvancedMode.Checked Then
            For Each item As ListViewItem In listDrives.Items
                If item.Text.Equals(strDriveLetterWeAreWorkingWith, StringComparison.OrdinalIgnoreCase) Then
                    listDrives.Items(item.Index).Selected = True
                    Exit For
                End If
            Next
        End If

        chkConfirmNewSmallerSize.Checked = My.Settings.confirmSmallerRestorePointSpaceSetting
        Me.Location = Functions.support.verifyWindowLocation(My.Settings.ManageSystemRestoreStorageSpaceWindowLocation)
        percentageIndicator.ProgressBarColor = My.Settings.barColor

        loadDriveData(strDriveWeAreLoadingDataFor)

        calculateWindowSize()
    End Sub

    ' driveLetter is just that, "C:"
    Private Function getDriveLabel(driveLetter As String) As String
        Dim wmiQueryObject As New Management.ObjectQuery(String.Format("SELECT * FROM Win32_LogicalDisk Where DeviceID='{0}'", driveLetter.ToUpper.Trim))
        Dim managementObjectSearcher As New Management.ManagementObjectSearcher(wmiQueryObject)

        If managementObjectSearcher.Get.Count = 0 Then
            managementObjectSearcher.Dispose()
            Return Nothing
        Else
            Dim managementObjectCollection As Management.ManagementObjectCollection = managementObjectSearcher.Get()

            Dim volumeName As String = managementObjectCollection(0)("VolumeName").ToString

            managementObjectSearcher.Dispose()
            managementObjectCollection.Dispose()

            Return volumeName
        End If
    End Function

    Private Sub getSize(driveLetter As String)
        Dim strDriveLabel As String = getDriveLabel(driveLetter)

        If strDriveLabel Is Nothing Then
            lblDriveLabel.Text = "Drive Label: (None Specified)"
        Else
            lblDriveLabel.Text = "Drive Label: " & strDriveLabel
        End If

        Dim boolGetVSSDataResult As Boolean
        Dim shadowStorageStatistics As Functions.supportClasses.ShadowStorageData = Nothing

        Try
            shadowStorageStatistics = Functions.vss.getData(driveLetter, boolGetVSSDataResult)
        Catch ex As Exception
            shadowStorageStatistics = Nothing
        End Try

        Try
            ' This makes sure we don't have a Null Reference Exception.
            If shadowStorageStatistics IsNot Nothing And boolGetVSSDataResult = True Then
                Dim percentage As Double = Functions.support.calculatePercentageValue(shadowStorageStatistics.UsedSpace, shadowStorageStatistics.MaxSpace)
                percentageIndicator.Value = Math.Round(percentage, 0)

                If percentage > globalVariables.warningPercentage Then
                    percentageIndicator.ProgressBarColor = Color.Red
                Else
                    percentageIndicator.ProgressBarColor = My.Settings.barColor
                End If

                lblUsedShadowStorageSpace.Invoke(Sub() lblUsedShadowStorageSpace.Text = String.Format("Used Shadow Storage Space: {0} of {1} ({2}% Used)", shadowStorageStatistics.UsedSpaceHuman, shadowStorageStatistics.MaxSpaceHuman, percentage))

                If shadowStorageStatistics.MaxSpaceHuman.Contains("Bytes") Then
                    listSizeType.SelectedIndex = 0
                ElseIf shadowStorageStatistics.MaxSpaceHuman.Contains("KBs") Then
                    listSizeType.SelectedIndex = 1
                ElseIf shadowStorageStatistics.MaxSpaceHuman.Contains("MBs") Then
                    listSizeType.SelectedIndex = 2
                ElseIf shadowStorageStatistics.MaxSpaceHuman.Contains("GBs") Then
                    listSizeType.SelectedIndex = 3
                ElseIf shadowStorageStatistics.MaxSpaceHuman.Contains("TBs") Then
                    listSizeType.SelectedIndex = 4
                End If

                txtSize.Text = Regex.Replace(shadowStorageStatistics.MaxSpaceHuman, "(?:Bytes|KBs|MBs|GBs|TBs|PBs)", "", RegexOptions.IgnoreCase).Trim
                shadowStorageStatistics = Nothing
            Else
                ' If it errors out, do this.
                lblUsedShadowStorageSpace.Invoke(Sub() lblUsedShadowStorageSpace.Text = "Used Shadow Storage Space: (None Allocated)")
                percentageIndicator.Value = 0
            End If
        Catch ex As Exception
            ' If it errors out, do this.
            lblUsedShadowStorageSpace.Invoke(Sub() lblUsedShadowStorageSpace.Text = "Used Shadow Storage Space: (None Allocated)")
            percentageIndicator.Value = 0
        End Try
    End Sub

    Sub setSize(ByVal dblSize As Double, listSizeType As String)
        Dim newSize As Long

        If listSizeType = "Kilobytes" Then
            newSize = dblSize * kilobytesInBytes
        ElseIf listSizeType = "Megabytes" Then
            newSize = dblSize * megabytesInBytes
        ElseIf listSizeType = "Gigabytes" Then
            newSize = dblSize * gigabytesInBytes
        ElseIf listSizeType = "Terabytes" Then
            newSize = dblSize * terabytesInBytes
        ElseIf listSizeType = "% (Percentage)" Then
            dblSize = dblSize / 100
            newSize = getSizeOfDrive(strDriveLetterWeAreWorkingWith) * dblSize
        End If

        Dim size As Long = Functions.vss.getMaxSize(strDriveLetterWeAreWorkingWith)

        If size = newSize Then
            MsgBox("You didn't change the size.", MsgBoxStyle.Information, Me.Text)
            Exit Sub
        End If

        If newSize < size And My.Settings.confirmSmallerRestorePointSpaceSetting Then
            If MsgBox("Your new size (" & Functions.support.bytesToHumanSize(newSize) & ") is smaller than your old specified (" & Functions.support.bytesToHumanSize(size) & ") size. This may cause your system to lose restore points." & vbCrLf & vbCrLf & "Are you sure you want to do this?", MsgBoxStyle.Question + MsgBoxStyle.YesNo, "Set New Size") = MsgBoxResult.No Then Exit Sub
        End If

        If size = 0 Then
            Functions.vss.executeVSSAdminCommand(strDriveLetterWeAreWorkingWith)
            Functions.vss.setShadowStorageSize(strDriveLetterWeAreWorkingWith, newSize)
            Functions.vss.enableSystemRestoreOnDriveWMI(strDriveLetterWeAreWorkingWith)
        Else
            Functions.vss.setShadowStorageSize(strDriveLetterWeAreWorkingWith, newSize)
        End If

        getSize(strDriveLetterWeAreWorkingWith)

        MsgBox("New System Restore Point Storage Space Size has been set.", MsgBoxStyle.Information, Me.Text)
    End Sub

    Private Sub btnSetSize_Click(sender As Object, e As EventArgs) Handles btnSetSize.Click
        Dim dblSize As Double

        If Double.TryParse(txtSize.Text, dblSize) = True Then
            If dblSize = 0 Then
                MsgBox("Zero is an invalid input.", MsgBoxStyle.Information, Me.Text)
                Exit Sub
            End If

            Dim strListSizeType As String = listSizeType.Text
            Threading.ThreadPool.QueueUserWorkItem(Sub() setSize(dblSize, strListSizeType))
        Else
            MsgBox("Invalid numerical input.", MsgBoxStyle.Critical, Me.Text)
        End If
    End Sub

    Private Sub chkConfirmNewSmallerSize_Click(sender As Object, e As EventArgs) Handles chkConfirmNewSmallerSize.Click
        My.Settings.confirmSmallerRestorePointSpaceSetting = chkConfirmNewSmallerSize.Checked
        My.Settings.Save()
    End Sub

    Private Sub chkAdvancedMode_Click(sender As Object, e As EventArgs) Handles chkAdvancedMode.Click
        If chkAdvancedMode.Checked Then
            showDrivesPanel()

            lblDriveSize.Text = "Total Size of Drive:"
            lblDriveLabel.Text = "Drive Label:"
            lblUsedShadowStorageSpace.Text = "Used Shadow Storage Space:"
            percentageIndicator.Value = 0
            txtSize.Text = Nothing
            listSizeType.SelectedIndex = -1
            btnSetSize.Text = Nothing
            btnSetSize.Enabled = False
        Else
            hideDrivesPanel()
            loadDriveData(globalVariables.systemDriveLetter)
        End If
    End Sub

    Private Sub listDrives_Click(sender As Object, e As EventArgs) Handles listDrives.Click
        If listDrives.SelectedItems.Count > 0 Then loadDriveData(listDrives.SelectedItems(0).Text)
    End Sub

    Private Sub showDrivesPanel()
        drivePanel.Visible = True
        settingsPanel.Location = New Point(75, 5)

        calculateWindowSize()
    End Sub

    Private Sub hideDrivesPanel()
        drivePanel.Visible = False
        settingsPanel.Location = New Point(5, 5)

        calculateWindowSize()
    End Sub

    Private Sub calculateWindowSize()
        Dim shortHeight As Short = chkAdvancedMode.Location.Y + chkAdvancedMode.Size.Height + 35
        Dim shortWidth As Short = settingsPanel.Location.X + settingsPanel.Size.Width + 15
        Me.Size = New Size(shortWidth, shortHeight)
    End Sub
End Class