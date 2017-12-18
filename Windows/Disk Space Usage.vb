Imports Tom

Public Class Disk_Space_Usage
    Private pleaseWaitInstance As Please_Wait
    Private formLoadDiskDataAttempts As Short = 0
    Private boldFont As New Font("Microsoft Sans Serif", 8.25, FontStyle.Bold)
    Private currentScreen As Screen = Screen.FromControl(Me)
    Private workingThread As Threading.Thread

    Sub manualFixSub(Optional drive As String = "C:")
        If (globalVariables.windows.frmManageSystemRestoreStorageSpace Is Nothing) Then
            Dim screen As Screen = Screen.FromControl(Me)

            globalVariables.windows.frmManageSystemRestoreStorageSpace = New frmManageSystemRestoreStorageSpace()
            globalVariables.windows.frmManageSystemRestoreStorageSpace.StartPosition = FormStartPosition.CenterParent
            globalVariables.windows.frmManageSystemRestoreStorageSpace.strDriveLetterWeAreWorkingWith = drive
            globalVariables.windows.frmManageSystemRestoreStorageSpace.Location = My.Settings.ManageSystemRestoreStorageSpaceWindowLocation
            globalVariables.windows.frmManageSystemRestoreStorageSpace.Show()
        Else
            globalVariables.windows.frmManageSystemRestoreStorageSpace.BringToFront()
        End If
    End Sub

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
            Dim stopWatch As Stopwatch = Stopwatch.StartNew()

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
                        If currentDrive.VolumeLabel IsNot Nothing Then
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

                Threading.Thread.Sleep(100)
            Next

            stopWatch.Stop()
            If stopWatch.Elapsed.Milliseconds < 1000 Then Threading.Thread.Sleep(1000 - stopWatch.Elapsed.Milliseconds)

            btnRefresh.Invoke(Sub() btnRefresh.Enabled = True)
            closePleaseWaitPanel()
            doTheResizingOfTheBars()
            GC.Collect()
        Catch ex As Threading.ThreadAbortException
        Catch ex As Exception
            Threading.Thread.CurrentThread.CurrentUICulture = New Globalization.CultureInfo("en-US")
            exceptionHandler.manuallyLoadCrashWindow(ex, ex.Message, ex.StackTrace, ex.GetType)

            btnRefresh.Invoke(Sub() btnRefresh.Enabled = True)
            closePleaseWaitPanel()
            doTheResizingOfTheBars()
            GC.Collect()
        Finally
            workingThread = Nothing
        End Try
    End Sub

    Private Sub Disk_Space_Usage_Activated(sender As Object, e As EventArgs) Handles Me.Activated
        Try
            GroupBox1.Focus()
        Catch ex As Exception
        End Try
    End Sub

    Private Sub Disk_Space_Usage_FormClosing(sender As Object, e As FormClosingEventArgs) Handles Me.FormClosing
        If workingThread IsNot Nothing Then workingThread.Abort()

        My.Settings.DiskSpaceUsageWindowLocation = Me.Location
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
                    openPleaseWaitPanel("Loading Disk Space Usage Information... Please Wait.")

                    workingThread = New Threading.Thread(AddressOf loadDiskSpaceUsageData)
                    workingThread.Name = "Disk Space Usage Data Loading Thread"
                    workingThread.IsBackground = True
                    workingThread.Start()
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

    Private Sub btnRefresh_Click(sender As Object, e As EventArgs) Handles btnRefresh.Click
        btnRefresh.Enabled = False

        Try
            GroupBox1.Controls.Clear()

            openPleaseWaitPanel("Loading Disk Space Usage Information... Please Wait.")

            workingThread = New Threading.Thread(AddressOf loadDiskSpaceUsageData)
            workingThread.Name = "Disk Space Usage Data Loading Thread"
            workingThread.IsBackground = True
            workingThread.Start()
        Catch ex As Exception
            Threading.Thread.CurrentThread.CurrentUICulture = New Globalization.CultureInfo("en-US")
            exceptionHandler.manuallyLoadCrashWindow(ex, ex.Message, ex.StackTrace, ex.GetType)
        End Try
    End Sub

    Sub doTheResizingOfTheBars()
        If GroupBox1 IsNot Nothing Then
            If GroupBox1.VerticalScroll.Visible Then
                For Each controlObject As Control In GroupBox1.Controls
                    If controlObject IsNot Nothing AndAlso controlObject.GetType = GetType(SmoothProgressBar) Then
                        If controlObject IsNot Nothing Then DirectCast(controlObject, SmoothProgressBar).Width = GroupBox1.Width - 48
                    End If
                Next
            Else
                For Each controlObject As Control In GroupBox1.Controls
                    If controlObject IsNot Nothing AndAlso controlObject.GetType = GetType(SmoothProgressBar) Then
                        If controlObject IsNot Nothing Then DirectCast(controlObject, SmoothProgressBar).Width = GroupBox1.Width - 30
                    End If
                Next
            End If
        End If
    End Sub

    Private Sub Disk_Space_Usage_Resize(sender As Object, e As EventArgs) Handles Me.Resize
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

    Private Sub btnSetBarColor_Click(sender As Object, e As EventArgs) Handles btnSetBarColor.Click
        Try
            ColorDialog.CustomColors = Functions.support.loadCustomColors()

            ColorDialog.Color = My.Settings.barColor
            ColorDialog.ShowDialog()
            My.Settings.barColor = ColorDialog.Color

            Functions.support.saveCustomColors(ColorDialog.CustomColors)

            Dim smoothProgressBarObject As SmoothProgressBar

            For Each control As Control In GroupBox1.Controls
                If control.GetType = GetType(SmoothProgressBar) Then
                    smoothProgressBarObject = DirectCast(control, SmoothProgressBar)

                    If smoothProgressBarObject.Value > globalVariables.warningPercentage And My.Settings.ShowFullDisksAsRed = True Then
                        smoothProgressBarObject.ProgressBarColor = Color.Red
                    Else
                        smoothProgressBarObject.ProgressBarColor = My.Settings.barColor
                    End If
                End If
            Next
        Catch ex As Exception
            Threading.Thread.CurrentThread.CurrentUICulture = New Globalization.CultureInfo("en-US")
            exceptionHandler.manuallyLoadCrashWindow(ex, ex.Message, ex.StackTrace, ex.GetType)
        End Try
    End Sub

    Private Sub Disk_Space_Usage_Shown(sender As Object, e As EventArgs) Handles Me.Shown
        If Me.IsHandleCreated = False Then Me.CreateHandle()

        Try
            Me.Size = My.Settings.diskUsageWindowSize
            chkShowFullDisksAsRed.Checked = My.Settings.ShowFullDisksAsRed
        Catch ex As Exception
            Threading.Thread.CurrentThread.CurrentUICulture = New Globalization.CultureInfo("en-US")
            exceptionHandler.manuallyLoadCrashWindow(ex, ex.Message, ex.StackTrace, ex.GetType)
        End Try

        btnRefresh.Enabled = False
        formLoadDiskData()
    End Sub

    Private Sub chkShowFullDisksAsRed_Click(sender As Object, e As EventArgs) Handles chkShowFullDisksAsRed.Click
        My.Settings.ShowFullDisksAsRed = chkShowFullDisksAsRed.Checked

        Dim smoothProgressBarObject As SmoothProgressBar

        For Each control As Control In GroupBox1.Controls
            If control.GetType = GetType(SmoothProgressBar) Then
                smoothProgressBarObject = DirectCast(control, SmoothProgressBar)

                If smoothProgressBarObject.Value > globalVariables.warningPercentage And My.Settings.ShowFullDisksAsRed = True Then
                    smoothProgressBarObject.ProgressBarColor = Color.Red
                Else
                    smoothProgressBarObject.ProgressBarColor = My.Settings.barColor
                End If
            End If
        Next
    End Sub

    Private Sub Disk_Space_Usage_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        Me.MaximumSize = New Size(859, Screen.FromControl(Me).Bounds.Height)
        Me.Location = Functions.support.verifyWindowLocation(My.Settings.DiskSpaceUsageWindowLocation)
        currentScreen = Screen.FromControl(Me)
    End Sub

    Private Sub Disk_Space_Usage_LocationChanged(sender As Object, e As EventArgs) Handles Me.LocationChanged
        Try
            If currentScreen IsNot Nothing Then
                If Not currentScreen.Equals(Screen.FromControl(Me)) Then
                    currentScreen = Screen.FromControl(Me)
                    Me.MaximumSize = New Size(859, Screen.FromControl(Me).Bounds.Height)
                End If
            End If
        Catch ex As Exception
            ' Does nothing.
        End Try
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