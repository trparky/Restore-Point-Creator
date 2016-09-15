﻿Imports Tom

Public Class Disk_Space_Usage
    Private pleaseWaitInstance As Please_Wait
    Private formLoadDiskDataAttempts As Short = 0
    Private boldFont As New Font("Microsoft Sans Serif", 8.25, FontStyle.Bold)
    Private boolLoadingDiskData As Boolean = False

    Sub manualFixSub(Optional drive As String = "C:")
        'debug.writeline("drive = " & drive)
        If (globalVariables.windows.frmManageSystemRestoreStorageSpace Is Nothing) Then
            Dim screen As Screen = Screen.FromControl(Me)

            globalVariables.windows.frmManageSystemRestoreStorageSpace = New frmManageSystemRestoreStorageSpace()
            globalVariables.windows.frmManageSystemRestoreStorageSpace.StartPosition = FormStartPosition.CenterParent
            globalVariables.windows.frmManageSystemRestoreStorageSpace.preSelectedDriveLetter = drive
            globalVariables.windows.frmManageSystemRestoreStorageSpace.Show()

            globalVariables.windows.frmManageSystemRestoreStorageSpace.Location = screen.Bounds.Location + New Point(100, 100)
        Else
            globalVariables.windows.frmManageSystemRestoreStorageSpace.BringToFront()
        End If
    End Sub

    'Sub addObjectToForm(ByVal objectToAdd As Object)
    '    Me.Invoke(Sub()
    '                  GroupBox1.Controls.Add(objectToAdd)
    '                  objectToAdd = Nothing
    '              End Sub)
    'End Sub

    ''' <summary>This function creates labels on the GUI so that the messy code doesn't have to be in the main code branches.</summary>
    ''' <param name="strLabelText">The text you want the label to have.</param>
    ''' <param name="xPosition">Obviously.</param>
    ''' <param name="yPosition">Obviously.</param>
    ''' <param name="boolBold">Obviously.</param>
    ''' <returns>Returns the next xPosition for the next lable to be created at.</returns>
    Function createLabel(ByVal strLabelText As String, ByVal xPosition As Integer, ByVal yPosition As Integer, ByVal boolBold As Boolean) As Integer
        Dim lblLabel As New Label
        lblLabel.Location = New Point(xPosition, yPosition)
        lblLabel.Show()
        lblLabel.AutoSize = True
        lblLabel.Text = strLabelText
        If boolBold = True Then lblLabel.Font = boldFont

        'GroupBox1.Controls.Add(lblLabel)
        GroupBox1.Invoke(Sub() GroupBox1.Controls.Add(lblLabel))

        xPosition += lblLabel.Width
        lblLabel = Nothing

        Return xPosition
    End Function

    Sub createLinkLabel(ByVal strLabelText As String, ByVal xPosition As Integer, ByVal yPosition As Integer, ByVal currentDriveLetter As String)
        Dim lblManageLink As New LinkLabel
        lblManageLink.Location = New Point(xPosition, yPosition)
        lblManageLink.Text = strLabelText
        lblManageLink.AutoSize = True

        AddHandler lblManageLink.Click, Sub() manualFixSub(currentDriveLetter)

        'GroupBox1.Controls.Add(lblManageLink)
        GroupBox1.Invoke(Sub() GroupBox1.Controls.Add(lblManageLink))

        lblManageLink = Nothing
    End Sub

    Sub createColoredBar(ByVal usedSpacePercentage As Double, ByVal freeSpacePercentage As Double, ByVal xPosition As Integer, ByVal yPosition As Integer)
        Dim diskUsageBar As New SmoothProgressBar

        If usedSpacePercentage > globalVariables.warningPercentage And My.Settings.ShowFullDisksAsRed = True Then
            diskUsageBar.ProgressBarColor = Color.Red
        Else
            diskUsageBar.ProgressBarColor = My.Settings.barColor
        End If

        diskUsageBar.Width = Me.GroupBox1.Width - 30
        diskUsageBar.Location = New Point(xPosition, yPosition)

        diskUsageBar.Height = 20
        diskUsageBar.Maximum = 100
        diskUsageBar.Value = usedSpacePercentage
        diskUsageBar.Show()

        ToolTip.SetToolTip(diskUsageBar, String.Format("{0}% Used ({1}% Free)", usedSpacePercentage, freeSpacePercentage))

        'GroupBox1.Controls.Add(diskUsageBar)
        GroupBox1.Invoke(Sub() GroupBox1.Controls.Add(diskUsageBar))

        diskUsageBar = Nothing
    End Sub

    Sub loadDiskSpaceUsageData()
        Try
            Dim currentDriveLetter As String

            Dim theFreeSpace, theTotalSpace, usedSpace, shadowStorageFreeSpace As ULong
            Dim usedSpacePercentage, freeSpacePercentage, shadowStorageUsedPercentage, shadowStorageFreeSpacePercentage As Double
            Dim yPosition As Integer = 10
            Dim xPosition As Integer = 12

            For Each currentDrive As IO.DriveInfo In My.Computer.FileSystem.Drives
                xPosition = 12 ' Resets the X position back to the beginning of the line.

                Try
                    If currentDrive.IsReady = True And (currentDrive.DriveType = IO.DriveType.Fixed Or currentDrive.DriveType = IO.DriveType.Removable) Then
                        currentDriveLetter = currentDrive.RootDirectory.ToString.Replace("\", "")

                        ' Some math.  YAY MATH!
                        theFreeSpace = currentDrive.TotalFreeSpace
                        theTotalSpace = currentDrive.TotalSize
                        usedSpace = theTotalSpace - theFreeSpace

                        usedSpacePercentage = Functions.support.calculatePercentageValue(usedSpace, theTotalSpace)
                        freeSpacePercentage = 100 - usedSpacePercentage




                        ' Creates the "Drive:" or "Mount Point:" header.
                        If currentDrive.RootDirectory.ToString.Length = 3 Then
                            xPosition = createLabel("Drive:", xPosition, yPosition, True)
                        Else
                            xPosition = createLabel("Mount Point:", xPosition, yPosition, True)
                        End If
                        ' Creates the "Drive:" or "Mount Point:" header.

                        ' Creates the Drive or Mount Point data label.
                        If currentDrive.RootDirectory.ToString.Length = 3 Then
                            xPosition = createLabel(currentDrive.RootDirectory.ToString.Replace("\", ""), xPosition, yPosition, False)
                        Else
                            xPosition = createLabel(currentDrive.RootDirectory.ToString, xPosition, yPosition, False)
                        End If
                        ' Creates the Drive or Mount Point data label.

                        ' If the drive has no drive label, why display one?
                        If currentDrive.VolumeLabel <> Nothing Then
                            ' Creates the "Drive Label:" header.
                            xPosition = createLabel("Drive Label:", xPosition + 10, yPosition, True)

                            ' Creates the Drive Label data label.
                            xPosition = createLabel(currentDrive.VolumeLabel, xPosition - 2, yPosition, False)
                        End If

                        ' Creates the "File System:" header.
                        xPosition = createLabel("File System:", xPosition + 10, yPosition, True)

                        ' Creates the File System data label.
                        xPosition = createLabel(currentDrive.DriveFormat, xPosition - 4, yPosition, False)

                        ' Creates "Drive Type:" header.
                        xPosition = createLabel("Drive Type:", xPosition + 10, yPosition, True)

                        ' Creates the Drive Type data label.
                        xPosition = createLabel(currentDrive.DriveType.ToString, xPosition - 2, yPosition, False)

                        ' Creates the "Drive Capacity:" header.
                        xPosition = createLabel("Drive Capacity:", xPosition + 10, yPosition, True)

                        ' Creates the Drive Capacity data label.
                        xPosition = createLabel(Functions.support.bytesToHumanSize(theTotalSpace), xPosition - 2, yPosition, False)




                        yPosition += 15 ' Advances the Y position down a bit so that we make room for the next object.
                        xPosition = 12 ' Resets the X position back to the beginning of the line.




                        ' Creates the "Disk Space Usage:" header.
                        xPosition = createLabel("Disk Space Usage:", xPosition, yPosition, True)

                        ' Creates the Disk Space Usage data label.
                        xPosition = createLabel(String.Format("{0} ({1}%)", Functions.support.bytesToHumanSize(usedSpace), usedSpacePercentage), xPosition - 2, yPosition, False)

                        ' Creates the "Free Space:" header.
                        xPosition = createLabel("Free Space:", xPosition + 10, yPosition, True)

                        ' Creates the Free Space data label.
                        xPosition = createLabel(String.Format("{0} ({1}%)", Functions.support.bytesToHumanSize(theFreeSpace), freeSpacePercentage), xPosition - 2, yPosition, False)




                        yPosition += 15 ' Advances the Y position down a bit so that we make room for the next object.




                        ' Handles the creation of the Free Disk Space progress bar.
                        createColoredBar(usedSpacePercentage, freeSpacePercentage, 12, yPosition)

                        If currentDriveLetter = globalVariables.systemDriveLetter Then
                            yPosition += 22 ' Advances the Y position down a bit so that we make room for the next object.
                            xPosition = 12 ' Resets the X position back to the beginning of the line.

                            Dim boolGetVSSDataResult As Boolean
                            Dim shadowStorageData As Functions.supportClasses.ShadowStorageData = Functions.vss.getData(globalVariables.systemDriveLetter, boolGetVSSDataResult)

                            If shadowStorageData IsNot Nothing And boolGetVSSDataResult = True Then
                                shadowStorageFreeSpace = shadowStorageData.MaxSpace - shadowStorageData.UsedSpace

                                shadowStorageUsedPercentage = Functions.support.calculatePercentageValue(shadowStorageData.UsedSpace, shadowStorageData.MaxSpace)
                                shadowStorageFreeSpacePercentage = 100 - shadowStorageUsedPercentage




                                ' Creates "System Restore Capacity:" header.
                                xPosition = createLabel("System Restore Capacity:", xPosition, yPosition, True)

                                ' Creates System Restore Capacity data label.
                                xPosition = createLabel(Functions.support.bytesToHumanSize(shadowStorageData.MaxSpace), xPosition - 2, yPosition, False)

                                ' Creates the "Used Space:" header.
                                xPosition = createLabel("Used Space:", xPosition + 10, yPosition, True)

                                ' Creates the Used Space data label.
                                xPosition = createLabel(String.Format("{0} ({1}%)", Functions.support.bytesToHumanSize(shadowStorageData.UsedSpace), shadowStorageUsedPercentage), xPosition - 2, yPosition, False)

                                ' Creates the "Free Space:" header.
                                xPosition = createLabel("Free Space:", xPosition + 10, yPosition, True)

                                ' Creates the Free Space data label.
                                xPosition = createLabel(String.Format("{0} ({1}%)", Functions.support.bytesToHumanSize(shadowStorageFreeSpace), shadowStorageFreeSpacePercentage), xPosition - 2, yPosition, False)

                                ' Handles the creation of the Manage Disk Space control lable.
                                createLinkLabel("Manage System Restore Space", xPosition + 10, yPosition, globalVariables.systemDriveLetter)




                                yPosition += 15 ' Advances the Y position down a bit so that we make room for the next object.




                                ' Handles the creation of the Shadow Copy Disk Free Disk Space progress bar.
                                createColoredBar(shadowStorageUsedPercentage, shadowStorageFreeSpacePercentage, 12, yPosition)

                                shadowStorageData = Nothing
                            End If
                        End If

                        yPosition += 50 ' Advances the Y position down for the display of data for the next system drive.
                    End If
                Catch ex As IO.IOException

                End Try
            Next
        Catch ex As Exception
            Threading.Thread.CurrentThread.CurrentUICulture = New Globalization.CultureInfo("en-US")
            exceptionHandler.manuallyLoadCrashWindow(ex, ex.Message, ex.StackTrace, ex.GetType)
        Finally
            boolLoadingDiskData = False
            Functions.wait.closePleaseWaitWindow()
            doTheResizingOfTheBars()
            GC.Collect()
        End Try
    End Sub

    'Public Shared Sub SetProgressNoAnimation(pb As ProgressBar)
    '    Dim value As Short = pb.Value

    '    ' To get around this animation, we need to move the progress bar backwards.
    '    If value = pb.Maximum Then
    '        ' Special case (can't set value > Maximum).
    '        pb.Value = value
    '        ' Set the value
    '        ' Move it backwards
    '        pb.Value = value - 1
    '    Else
    '        ' Move past
    '        pb.Value = value + 1
    '    End If

    '    pb.Value = value
    '    ' Move to correct value
    'End Sub

    Private Sub Disk_Space_Usage_Activated(sender As Object, e As EventArgs) Handles Me.Activated
        GroupBox1.Focus()
    End Sub

    Private Sub Disk_Space_Usage_FormClosing(sender As Object, e As FormClosingEventArgs) Handles Me.FormClosing
        My.Settings.DiskSpaceUsageWindowLocation = Me.Location
        destroyAllObjectsInGroupBox()
        globalVariables.windows.frmDiskSpaceUsageWindow.Dispose()
        globalVariables.windows.frmDiskSpaceUsageWindow = Nothing
    End Sub

    Private Sub Disk_Space_Usage_KeyUp(sender As Object, e As KeyEventArgs) Handles Me.KeyUp
        If e.KeyCode = Keys.F5 Then
            btnRefresh.PerformClick()
        End If
    End Sub

    Sub formLoadDiskData()
        Try
            If Me.IsHandleCreated = True Then
                If GroupBox1.IsHandleCreated = True Then
                    Functions.wait.createPleaseWaitWindow("Loading Disk Space Usage Information... Please Wait.")

                    Dim loadDiskSpaceUsageDataThread As New Threading.Thread(AddressOf loadDiskSpaceUsageData)
                    loadDiskSpaceUsageDataThread.Start()

                    Functions.wait.openPleaseWaitWindow(Me)
                End If
            Else
                Threading.Thread.Sleep(100)

                If formLoadDiskDataAttempts = 5 Then
                    Exit Sub
                End If

                formLoadDiskDataAttempts += 1
                formLoadDiskData()
            End If
        Catch ex As Exception

        End Try
    End Sub

    Private Sub Disk_Space_Usage_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        If Me.IsHandleCreated = False Then Me.CreateHandle()

        Try
            Control.CheckForIllegalCrossThreadCalls = False
            Me.Size = My.Settings.diskUsageWindowSize
            chkShowFullDisksAsRed.Checked = My.Settings.ShowFullDisksAsRed

            If Me.Size.Width <> 859 Then
                Me.Size = New Size(859, Me.Size.Height)
                My.Settings.diskUsageWindowSize = Me.Size
            End If
        Catch ex As Exception
            Threading.Thread.CurrentThread.CurrentUICulture = New Globalization.CultureInfo("en-US")
            exceptionHandler.manuallyLoadCrashWindow(ex, ex.Message, ex.StackTrace, ex.GetType)
        End Try
    End Sub

    Sub destroyAllObjectsInGroupBox()
        Dim controlObject As Control

        For i As Integer = (GroupBox1.Controls.Count - 1) To 0 Step -1
            controlObject = GroupBox1.Controls(i)
            controlObject.Dispose()

            Functions.PSLib.cEventHelper.RemoveAllEventHandlers(controlObject)
            'GroupBox1.Controls.Remove(controlObject)
            controlObject = Nothing
        Next i
    End Sub

    Private Sub btnRefresh_Click(sender As Object, e As EventArgs) Handles btnRefresh.Click
        Try
            If boolLoadingDiskData = True Then
                MsgBox("Please wait until the loading of the disk data is complete before you try again.", MsgBoxStyle.Information, Me.Text)
                Exit Sub
            End If

            destroyAllObjectsInGroupBox()
            GC.Collect()

            boolLoadingDiskData = True

            Functions.wait.createPleaseWaitWindow("Loading Disk Space Usage Information... Please Wait.")

            Dim loadDiskSpaceUsageDataThread As New Threading.Thread(AddressOf loadDiskSpaceUsageData)
            loadDiskSpaceUsageDataThread.Start()

            Functions.wait.openPleaseWaitWindow(Me)
        Catch ex As Exception
            Threading.Thread.CurrentThread.CurrentUICulture = New Globalization.CultureInfo("en-US")
            exceptionHandler.manuallyLoadCrashWindow(ex, ex.Message, ex.StackTrace, ex.GetType)
        End Try
    End Sub

    Sub doTheResizingOfTheBars()
        Dim controlObject As Control
        Dim smoothProgressBarObject As SmoothProgressBar

        If GroupBox1.VerticalScroll.Visible = True Then
            For i As Integer = (GroupBox1.Controls.Count - 1) To 0 Step -1
                controlObject = GroupBox1.Controls(i)

                If controlObject.GetType.ToString = "Tom.SmoothProgressBar" Then
                    smoothProgressBarObject = DirectCast(controlObject, SmoothProgressBar)

                    smoothProgressBarObject.Width = GroupBox1.Width - 48
                End If
            Next i
        Else
            For i As Integer = (GroupBox1.Controls.Count - 1) To 0 Step -1
                controlObject = GroupBox1.Controls(i)

                If controlObject.GetType.ToString = "Tom.SmoothProgressBar" Then
                    smoothProgressBarObject = DirectCast(controlObject, SmoothProgressBar)

                    smoothProgressBarObject.Width = GroupBox1.Width - 30
                End If
            Next i
        End If
    End Sub

    Private Sub Disk_Space_Usage_Resize(sender As Object, e As EventArgs) Handles Me.Resize
        If Me.Width <> 859 Then Me.Width = 859
        doTheResizingOfTheBars()
    End Sub

    Private Sub Disk_Space_Usage_ResizeEnd(sender As Object, e As EventArgs) Handles Me.ResizeEnd
        My.Settings.diskUsageWindowSize = Me.Size
        My.Settings.Save()
    End Sub

    Private Sub btnManageSystemRestoreStorageSize_Click(sender As Object, e As EventArgs) Handles btnManageSystemRestoreStorageSize.Click
        If (globalVariables.windows.frmManageSystemRestoreStorageSpace Is Nothing) Then
            globalVariables.windows.frmManageSystemRestoreStorageSpace = New frmManageSystemRestoreStorageSpace
            globalVariables.windows.frmManageSystemRestoreStorageSpace.Show()
        Else
            globalVariables.windows.frmManageSystemRestoreStorageSpace.BringToFront()
        End If
    End Sub

    Private Sub GroupBox1_Click(sender As Object, e As EventArgs) Handles GroupBox1.Click
        'GroupBox1.Focus()
    End Sub

    Private Sub btnSetBarColor_Click(sender As Object, e As EventArgs) Handles btnSetBarColor.Click
        Try
            If My.Settings.customColors2 IsNot Nothing Then
                Dim integerArray As Integer()
                ReDim integerArray(My.Settings.customColors2.Count - 1)

                For i = 0 To My.Settings.customColors2.Count - 1
                    integerArray(i) = Integer.Parse(My.Settings.customColors2(i))
                Next

                ColorDialog.CustomColors = integerArray
                integerArray = Nothing
            End If

            ColorDialog.Color = My.Settings.barColor
            ColorDialog.ShowDialog()
            My.Settings.barColor = ColorDialog.Color

            Dim temp As New Specialized.StringCollection
            For Each entry As String In ColorDialog.CustomColors
                temp.Add(entry)
            Next
            My.Settings.customColors2 = temp
            My.Settings.Save()
            temp = Nothing

            Dim controlObject As Control
            Dim smoothProgressBarObject As SmoothProgressBar

            For i As Integer = (GroupBox1.Controls.Count - 1) To 0 Step -1
                controlObject = GroupBox1.Controls(i)

                If controlObject.GetType.ToString = "Tom.SmoothProgressBar" Then
                    smoothProgressBarObject = DirectCast(controlObject, SmoothProgressBar)

                    If smoothProgressBarObject.Value > globalVariables.warningPercentage And My.Settings.ShowFullDisksAsRed = True Then
                        smoothProgressBarObject.ProgressBarColor = Color.Red
                    Else
                        smoothProgressBarObject.ProgressBarColor = My.Settings.barColor
                    End If
                End If
            Next i
        Catch ex As Exception
            Threading.Thread.CurrentThread.CurrentUICulture = New Globalization.CultureInfo("en-US")
            exceptionHandler.manuallyLoadCrashWindow(ex, ex.Message, ex.StackTrace, ex.GetType)
        End Try
    End Sub

    Private Sub Disk_Space_Usage_Shown(sender As Object, e As EventArgs) Handles Me.Shown
        If Me.IsHandleCreated = False Then Me.CreateHandle()
        formLoadDiskData()
    End Sub

    Private Sub chkShowFullDisksAsRed_Click(sender As Object, e As EventArgs) Handles chkShowFullDisksAsRed.Click
        My.Settings.ShowFullDisksAsRed = chkShowFullDisksAsRed.Checked

        Dim controlObject As Control
        Dim smoothProgressBarObject As SmoothProgressBar

        For i As Integer = (GroupBox1.Controls.Count - 1) To 0 Step -1
            controlObject = GroupBox1.Controls(i)

            If controlObject.GetType.ToString = "Tom.SmoothProgressBar" Then
                smoothProgressBarObject = DirectCast(controlObject, SmoothProgressBar)

                If smoothProgressBarObject.Value > globalVariables.warningPercentage And My.Settings.ShowFullDisksAsRed = True Then
                    smoothProgressBarObject.ProgressBarColor = Color.Red
                Else
                    smoothProgressBarObject.ProgressBarColor = My.Settings.barColor
                End If
            End If
        Next i
    End Sub
End Class