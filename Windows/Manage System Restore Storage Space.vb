Imports System.Text.RegularExpressions

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

    Function getSizeOfDrive(strDriveLetter As String) As ULong
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
        If Not String.IsNullOrEmpty(strDriveLetterWeAreWorkingWith) Then
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
        Me.Location = My.Settings.ManageSystemRestoreStorageSpaceWindowLocation
        Control.CheckForIllegalCrossThreadCalls = False
        percentageIndicator.ProgressBarColor = My.Settings.barColor

        loadDriveData(globalVariables.systemDriveLetter)
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
        Dim strHumanSize As String = Nothing

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
                strHumanSize = Functions.support.bytesToHumanSize(shadowStorageStatistics.MaxSpace)

                Dim percentage As Double = Functions.support.calculatePercentageValue(shadowStorageStatistics.UsedSpace, shadowStorageStatistics.MaxSpace)
                percentageIndicator.Value = Math.Round(percentage, 0)

                If percentage > globalVariables.warningPercentage Then
                    percentageIndicator.ProgressBarColor = Color.Red
                Else
                    percentageIndicator.ProgressBarColor = My.Settings.barColor
                End If

                lblUsedShadowStorageSpace.Text = String.Format("Used Shadow Storage Space: {0} of {1} ({2}% Used)", Functions.support.bytesToHumanSize(shadowStorageStatistics.UsedSpace), strHumanSize, percentage)

                If strHumanSize.Contains("Bytes") Then
                    listSizeType.SelectedIndex = 0
                ElseIf strHumanSize.Contains("KBs") Then
                    listSizeType.SelectedIndex = 1
                ElseIf strHumanSize.Contains("MBs") Then
                    listSizeType.SelectedIndex = 2
                ElseIf strHumanSize.Contains("GBs") Then
                    listSizeType.SelectedIndex = 3
                ElseIf strHumanSize.Contains("TBs") Then
                    listSizeType.SelectedIndex = 4
                End If

                txtSize.Text = Regex.Replace(strHumanSize, "(?:Bytes|KBs|MBs|GBs|TBs|PBs)", "", RegexOptions.IgnoreCase).Trim
            Else
                ' If it errors out, do this.
                lblUsedShadowStorageSpace.Text = "Used Shadow Storage Space: (None Allocated)"
                percentageIndicator.Value = 0
            End If
        Catch ex As Exception
            ' If it errors out, do this.
            lblUsedShadowStorageSpace.Text = "Used Shadow Storage Space: (None Allocated)"
            percentageIndicator.Value = 0
        End Try
    End Sub

    Sub setSize(ByVal dblSize As Double)
        Dim newSize As Long

        If listSizeType.Text = "Kilobytes" Then
            newSize = dblSize * kilobytesInBytes
        ElseIf listSizeType.Text = "Megabytes" Then
            newSize = dblSize * megabytesInBytes
        ElseIf listSizeType.Text = "Gigabytes" Then
            newSize = dblSize * gigabytesInBytes
        ElseIf listSizeType.Text = "Terabytes" Then
            newSize = dblSize * terabytesInBytes
        ElseIf listSizeType.Text = "% (Percentage)" Then
            dblSize = dblSize / 100
            newSize = getSizeOfDrive(strDriveLetterWeAreWorkingWith) * dblSize
        End If

        Dim size As ULong = Functions.vss.getMaxSize(strDriveLetterWeAreWorkingWith)

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

            Threading.ThreadPool.QueueUserWorkItem(Sub() setSize(dblSize))
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
        Me.Size = New Size(469, 217)

        drivePanel.Visible = True
        settingsPanel.Location = New Point(75, 5)
    End Sub

    Private Sub hideDrivesPanel()
        Me.Size = New Size(400, 217)

        drivePanel.Visible = False
        settingsPanel.Location = New Point(5, 5)
    End Sub
End Class