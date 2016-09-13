Module oldFunctions
    'Public Function executeCommandLineAndGetOutput(processExecutable As String, Optional commandLineArguments As String = Nothing) As String
    '    Dim processObject As New Process()
    '    Dim processStartSettings As ProcessStartInfo

    '    If commandLineArguments = Nothing Then
    '        processStartSettings = New ProcessStartInfo(processExecutable)
    '    Else
    '        processStartSettings = New ProcessStartInfo(processExecutable, commandLineArguments)
    '    End If

    '    processStartSettings.UseShellExecute = False
    '    processStartSettings.RedirectStandardOutput = True
    '    processStartSettings.CreateNoWindow = True

    '    processObject.StartInfo = processStartSettings
    '    processObject.Start()

    '    Return processObject.StandardOutput.ReadToEnd
    'End Function

    'Private Function ResizeImage(SourceImage As Image, TargetWidth As Integer, TargetHeight As Integer) As Bitmap
    '    Dim bmSource = New Bitmap(SourceImage)
    '    Return ResizeImage(bmSource, TargetWidth, TargetHeight)
    'End Function

    'Private Function ResizeImage(bmSource As Bitmap, TargetWidth As Integer, TargetHeight As Integer) As Bitmap
    '    Dim bmDest As New Bitmap(TargetWidth, TargetHeight, Imaging.PixelFormat.Format32bppArgb)

    '    Dim nSourceAspectRatio = bmSource.Width / bmSource.Height
    '    Dim nDestAspectRatio = bmDest.Width / bmDest.Height

    '    Dim NewX = 0
    '    Dim NewY = 0
    '    Dim NewWidth = bmDest.Width
    '    Dim NewHeight = bmDest.Height

    '    If nDestAspectRatio = nSourceAspectRatio Then
    '        'same ratio
    '    ElseIf nDestAspectRatio > nSourceAspectRatio Then
    '        'Source is taller
    '        NewWidth = Convert.ToInt32(Math.Floor(nSourceAspectRatio * NewHeight))
    '        NewX = Convert.ToInt32(Math.Floor((bmDest.Width - NewWidth) / 2))
    '    Else
    '        'Source is wider
    '        NewHeight = Convert.ToInt32(Math.Floor((1 / nSourceAspectRatio) * NewWidth))
    '        NewY = Convert.ToInt32(Math.Floor((bmDest.Height - NewHeight) / 2))
    '    End If

    '    Using grDest = Graphics.FromImage(bmDest)
    '        With grDest
    '            .CompositingQuality = Drawing2D.CompositingQuality.HighQuality
    '            .InterpolationMode = Drawing2D.InterpolationMode.HighQualityBicubic
    '            .PixelOffsetMode = Drawing2D.PixelOffsetMode.HighQuality
    '            .SmoothingMode = Drawing2D.SmoothingMode.AntiAlias
    '            .CompositingMode = Drawing2D.CompositingMode.SourceOver

    '            .DrawImage(bmSource, NewX, NewY, NewWidth, NewHeight)
    '        End With
    '    End Using

    '    Return bmDest
    'End Function
            
    'Module Functions
    '<DllImport("kernel32.dll", SetLastError:=False)>
    'Private Function GetProductInfo(intOSMajorVersion As Integer, intOSMinorVersion As Integer, intSPMajorVersion As Integer, intSPMinorVersion As Integer, ByRef intProductType As Integer) As Boolean
    'End Function

    'Debug.WriteLine("gdiHandles = {0}", Functions.GetGuiResources(Process.GetCurrentProcess().Handle, GlobalVariables.gdiHandles))
    'Debug.WriteLine("userHandles = {0}", Functions.GetGuiResources(Process.GetCurrentProcess().Handle, GlobalVariables.userHandles))
    '<DllImport("User32")> Public Function GetGuiResources(hProcess As IntPtr, uiFlags As Integer) As Integer
    'End Function
    'End Module

    'Private Sub enableSystemRestoreOnSystemDrive()
    '    enableSystemRestoreOnDriveVSSAdmin(Environment.SystemDirectory.Substring(0, 2))
    'End Sub

    ' This function returns a Date Object, you would call this function with the unique numerical ID of a Restore Point.
    ' Dim dateOfRestorePoint As Date = getNewestSystemRestorePointDate(2)

    'Public Sub changeLanguage(ByVal lang As String, windowObject As Object)
    '    For Each c As Control In windowObject.Controls
    '        Dim resources As ComponentResourceManager = New ComponentResourceManager(windowObject.GetType)
    '        resources.ApplyResources(c, c.Name, New CultureInfo(lang))
    '    Next c
    'End Sub

    'Public Function getDriveLetterFromDeviceID(deviceID As String) As String
    '    Dim objMOC As Management.ManagementObjectCollection
    '    Dim objMOS As Management.ManagementObjectSearcher = New Management.ManagementObjectSearcher("SELECT * FROM Win32_Volume")
    '    Dim objMO As Management.ManagementObject

    '    objMOC = objMOS.Get()

    '    For Each objMO In objMOC
    '        If deviceID = objMO("DeviceID") Then
    '            Return objMO("DriveLetter")
    '        End If
    '    Next

    '    Return Nothing
    'End Function

    'Private Sub enableSystemRestoreOnAllSystemDrives()
    '    Dim boolDisableRepairOfSystemRestoreOnSystemDrives As Boolean

    '    If Boolean.TryParse(Microsoft.Win32.Registry.LocalMachine.OpenSubKey(globalVariables.registry.strProgramRegistryKey).GetValue("Disable Repair of System Restore on System Drives", "False"), boolDisableRepairOfSystemRestoreOnSystemDrives) Then
    '        If boolDisableRepairOfSystemRestoreOnSystemDrives = False Then
    '            Dim driveLetter As String

    '            For Each currentDrive As IO.DriveInfo In My.Computer.FileSystem.Drives
    '                If currentDrive.DriveType = IO.DriveType.Fixed Then
    '                    driveLetter = currentDrive.RootDirectory.ToString.Replace("\", "")
    '                    'debug.writeline("Repairing System Restore on System Drive " & driveLetter)
    '                    enableSystemRestoreOnDrive(driveLetter)
    '                End If
    '            Next
    '        Else
    '            'debug.writeline("System Restore on System Drive Repair disabled")
    '        End If
    '    End If
    'End Sub

    '' Call like this... enableSystemRestoreOnDrive("C:").  It accepts a drive letter.
    'Public Function disableSystemRestoreOnDrive(driveLetter As String) As Short
    '    Try
    '        Dim managementScopeObject As New ManagementScope("\\localhost\root\default")
    '        Dim managementPathObject As New ManagementPath("SystemRestore")
    '        Dim managementObjectOptions As New ObjectGetOptions()
    '        Dim managementClassObject As New ManagementClass(managementScopeObject, managementPathObject, managementObjectOptions)

    '        Dim managementBaseObjectParameters As ManagementBaseObject = managementClassObject.GetMethodParameters("Disable")
    '        managementBaseObjectParameters("Drive") = driveLetter
    '        'managementBaseObjectParameters("WaitTillEnabled") = True

    '        Dim oOutParams As ManagementBaseObject = managementClassObject.InvokeMethod("Disable", managementBaseObjectParameters, Nothing)

    '        'MsgBox(oOutParams.Properties.Count)
    '        'For Each t In oOutParams.Properties
    '        '    MsgBox(t.Name)
    '        'Next

    '        Return oOutParams("ReturnValue")
    '    Catch ex3 As System.Runtime.InteropServices.COMException
    '        Functions.giveComExceptionCrashMessage()
    '        Return 1
    '    Catch ex As Exception
    '        Return 1
    '    End Try
    'End Function

    'Public Function GetDriveInfo() As String
    '    Dim objMOC As Management.ManagementObjectCollection
    '    Dim objMOS As Management.ManagementObjectSearcher = New Management.ManagementObjectSearcher("SELECT * FROM Win32_Volume")
    '    Dim objMO As Management.ManagementObject

    '    objMOC = objMOS.Get()

    '    For Each objMO In objMOC
    '        Dim objDriveLetter As String = objMO("DeviceID")
    '        Dim objVolumeLabel = objMO("DriveLetter")

    '        'debug.writeline(objDriveLetter & ": " & objVolumeLabel)
    '    Next
    '    Return Nothing
    'End Function

    ' This is the old function kept here in case the patched function above is broken.
    'Public Function doesAtUserLoginTaskExist(ByRef delayedTime As Short) As Boolean
    '    Try
    '        Dim nameOfTask As String = GlobalVariables.taskFolder & "\Create a Restore Point at User Logon"
    '        Dim taskObject As Microsoft.Win32.TaskScheduler.Task

    '        Using taskServiceObject As TaskService = New TaskService()
    '            taskObject = taskServiceObject.GetTask(nameOfTask)

    '            ' Makes sure that the task exists and we don't get a Null Reference Exception.
    '            If taskObject IsNot Nothing Then
    '                ' Makes sure that we have some triggers to actually work with.
    '                If taskObject.Definition.Triggers.Count > 0 Then
    '                    Dim trigger As Trigger = taskObject.Definition.Triggers.Item(0)

    '                    delayedTime = CType(trigger, ITriggerDelay).Delay.Minutes

    '                    Return True
    '                End If
    '            End If
    '        End Using

    '        Return False
    '    Catch ex As Exception
    '        Return False
    '    End Try
    'End Function

    'Public Sub ScaleForm(WindowsForm As System.Windows.Forms.Form)
    '    Try
    '        Using g As System.Drawing.Graphics = WindowsForm.CreateGraphics
    '            Dim sngScaleFactor As Single = 1
    '            Dim sngFontFactor As Single = 1
    '            If g.DpiX > 96 Then
    '                sngScaleFactor = g.DpiX / 96
    '                'sngFontFactor = 96 / g.DpiY
    '            End If
    '            If WindowsForm.AutoScaleDimensions = WindowsForm.CurrentAutoScaleDimensions Then
    '                'ucWindowsFormHost.ScaleControl(WindowsForm, sngFontFactor)
    '                WindowsForm.Scale(sngScaleFactor)
    '            End If
    '        End Using
    '    Catch ex As Exception
    '    End Try
    'End Sub

    'Private Function doWeNeedToMakeAScheduledRestorePoint(strRestorePointDescription As String, shortEvery As Short) As Boolean
    '    Dim id As Long = getNewestSystemRestorePointIDMadeByRestorePointCreator(strRestorePointDescription)

    '    If id = 0 Then
    '        Return True
    '    End If

    '    Dim creationDate As Date = getSystemRestorePointDate(id)

    '    If shortEvery <> 0 And (creationDate = Nothing) = False Then
    '        'Math.Round(creationDate.Subtract(Now).TotalDays)
    '        'Dim dateDiffValue As Integer = Math.Abs(DateDiff(DateInterval.Day, creationDate, Now)) + 1
    '        Dim doubleDateDiffValue As Double = Math.Round(Now.Subtract(creationDate).TotalDays)
    '        Dim strDateDiffValue As String = daysToString(doubleDateDiffValue)

    '        If doubleDateDiffValue >= shortEvery Then
    '            eventLogFunctions.writeToSystemEventLog(String.Format("The last restore point created by System Restore Point Creator was {0} ago.", strDateDiffValue), EventLogEntryType.Information)

    '            Return True
    '        Else
    '            Dim strEveryDayString As String = daysToString(shortEvery)

    '            eventLogFunctions.writeToSystemEventLog(String.Format("A system restore point isn't necessary according to user preferences. User has instructed the program to only create restore points every {0}, the last restore point created by System Restore Point Creator was {1} ago.", strEveryDayString, strDateDiffValue), EventLogEntryType.Information)

    '            Return False
    '        End If
    '    Else
    '        ' If the value of Every is 0 that means that the user wants to have unlimited restore points created.
    '        Return True
    '    End If
    'End Function

    'Private Function executeAddSafeModeCommand() As String ' Adds a new OS boot entry to the Windows BCD and returns the GUID of the new entry.
    '    Try

    '        Dim sOutput As String = executeCommandLineAndGetOutput("bcdedit", "/copy {current} /d ""Windows Safe Mode Boot""")

    '        If Regex.IsMatch(sOutput, "(\{[0-9A-F]{8}-[0-9A-F]{4}-[0-9A-F]{4}-[0-9A-F]{4}-[0-9A-F]{12}\})", RegexOptions.IgnoreCase) Then
    '            Return Regex.Match(sOutput, "(\{[0-9A-F]{8}-[0-9A-F]{4}-[0-9A-F]{4}-[0-9A-F]{4}-[0-9A-F]{12}\})", RegexOptions.IgnoreCase).Groups(1).Value.Trim
    '        Else
    '            Return "failed"
    '        End If
    '    Catch ex As Exception
    '        Return "failed"
    '    End Try
    'End Function

    'Private Function checkUACStatus() As Boolean
    '    Dim registryKeyToRead As RegistryKey = Registry.LocalMachine.OpenSubKey("SOFTWARE\Microsoft\Windows\CurrentVersion\Policies\System", False)

    '    If registryKeyToRead Is Nothing Then
    '        Return True
    '    Else
    '        Dim enableLUA As String = registryKeyToRead.GetValue("EnableLUA", 1)
    '        registryKeyToRead.Close()
    '        registryKeyToRead.Dispose()

    '        Dim shortEnableLUA As Short
    '        If Short.TryParse(enableLUA, shortEnableLUA) Then
    '            If shortEnableLUA <> 0 Then
    '                Return True
    '            Else
    '                Return False
    '            End If
    '        Else
    '            Return True
    '        End If
    '    End If
    'End Function

    'Public Function setSafeModeBoot(Optional saveSettingAsNewBootEntry As Boolean = True) As Boolean
    '    If saveSettingAsNewBootEntry = True Then
    '        Dim strGUIDForWindowsSafeModeBootEntry As String = executeAddSafeModeCommand()

    '        If strGUIDForWindowsSafeModeBootEntry = "failed" Then
    '            Return False
    '        End If

    '        ' We need this later to remove the Safe Mode Boot Entry from the Windows BCD Store so we save the value in a Registry Value for later use.
    '        Registry.LocalMachine.OpenSubKey(globalVariables.registry.strProgramRegistryKey, True).SetValue("Safe Mode Boot GUID", strGUIDForWindowsSafeModeBootEntry, RegistryValueKind.String)

    '        Shell("bcdedit /set " & strGUIDForWindowsSafeModeBootEntry & " safeboot minimal", AppWinStyle.Hide)
    '        Threading.Thread.Sleep(100)
    '        'Shell("bcdedit /default " & strGUIDForWindowsSafeModeBootEntry, AppWinStyle.Hide)
    '        'Threading.Thread.Sleep(100)
    '        Shell("bootcfg /timeout 5", AppWinStyle.Hide)
    '        Threading.Thread.Sleep(100)

    '        Return True
    '    Else
    '        Shell("bcdedit /set {default} safeboot minimal", AppWinStyle.Hide)
    '        Return True
    '    End If
    'End Function

    'Function getProxyConfiguration() As Net.WebProxy
    '    If My.Settings.useSystemProxyConfig = True Then
    '        Return Net.WebProxy.GetDefaultProxy
    '        'Return Net.WebRequest.DefaultWebProxy
    '        'Return Net.WebRequest.GetSystemWebProxy()
    '    Else
    '        Dim proxyConfig As New Net.WebProxy(My.Settings.proxyAddress, My.Settings.proxyPort)

    '        If My.Settings.proxyUser <> Nothing And My.Settings.proxyPass <> Nothing Then
    '            proxyConfig.Credentials = New Net.NetworkCredential(My.Settings.proxyUser, utilities.convertFromBase64(My.Settings.proxyPass))
    '        End If

    '        Return proxyConfig
    '    End If
    'End Function

    'Public Sub removeSafeModeBoot(Optional bcdBootGUIDToDelete As String = "{none}")
    '    If bcdBootGUIDToDelete = "{none}" Then
    '        Shell("bcdedit /deletevalue {default} safeboot", AppWinStyle.Hide)
    '    Else
    '        Shell("bcdedit /delete " & bcdBootGUIDToDelete, AppWinStyle.Hide)
    '    End If
    'End Sub

    'Sub loadIcon(objectGettingTheIcon As Object, image As Bitmap)
    '    objectGettingTheIcon.Image = image
    '    objectGettingTheIcon.ImageAlign = ContentAlignment.MiddleLeft
    'End Sub

    'Private Function SHA1ChecksumString(input As String) As String
    '    Dim SHA1Engine As New Security.Cryptography.SHA1CryptoServiceProvider
    '    Dim inputAsByteArray As Byte() = Encoding.ASCII.GetBytes(input)
    '    Dim hashedByteArray As Byte() = SHA1Engine.ComputeHash(inputAsByteArray)
    '    Return BitConverter.ToString(hashedByteArray).ToLower().Replace("-", "").Trim
    'End Function

    'Private Function SHA160ChecksumFile(ByVal filename As String) As String
    '    Dim SHA1Engine As New Security.Cryptography.SHA1CryptoServiceProvider

    '    Dim FileStream As New FileStream(filename, FileMode.Open, FileAccess.Read, FileShare.Read, 10 * 1048576, FileOptions.SequentialScan)
    '    Dim Output As Byte() = SHA1Engine.ComputeHash(FileStream)
    '    FileStream.Close()
    '    FileStream.Dispose()

    '    Return BitConverter.ToString(Output).ToLower().Replace("-", "").Trim
    'End Function

    'Private Function SHA160ChecksumFile(ByRef stream As Stream) As String
    '    Dim SHA1Engine As New Security.Cryptography.SHA1CryptoServiceProvider

    '    Dim Output As Byte() = SHA1Engine.ComputeHash(stream)
    '    Return BitConverter.ToString(Output).ToLower().Replace("-", "").Trim
    'End Function
End Module