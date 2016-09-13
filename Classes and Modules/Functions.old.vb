Imports System
Imports System.Management
Imports Microsoft.Win32.TaskScheduler
Imports System.ComponentModel
Imports System.Globalization
Imports System.IO
Imports System.Text.RegularExpressions
Imports System.Text
Imports Microsoft.Win32
Imports System.Runtime.InteropServices
Imports System.Security.Principal
Imports Microsoft.Win32.SafeHandles

Namespace Functions
    Module Functions
        <DllImport("Srclient.dll")> Public Function SRRemoveRestorePoint(index As Integer) As Integer
        End Function

        Public Const ERROR_SUCCESS As Integer = 0
        Public Const ERROR_BAD_ENVIRONMENT As Integer = 10
        Public Const ERROR_DISK_FULL As Integer = 112
        Public Const ERROR_INTERNAL_ERROR As Integer = 1359
        Public Const ERROR_INVALID_DATA As Integer = 13
        Public Const ERROR_SERVICE_DISABLED As Integer = 1058
        Public Const ERROR_TIMEOUT As Integer = 1460
        Public Const ERROR_WEBREQUEST As String = "WebRequest Error"

        Public Sub giveComExceptionCrashMessage()
            MsgBox("A program crash that should not happen on properly working Windows installations has occurred.  Your windows installation will need to be repaired before this program will function properly." & vbCrLf & vbCrLf & "Your web browser will now open with instructions on how to repair your Windows installation.", MsgBoxStyle.Critical, "Critical Windows Component Failure Detected")
            Process.Start("http://www.toms-world.org/blog/restore_point_creator/comexception-crash")
            Process.GetCurrentProcess.Kill()
        End Sub

        Public Function IsUserInAdminGroup() As Boolean
            Dim fInAdminGroup As Boolean = False
            Dim hToken As SafeTokenHandle = Nothing
            Dim hTokenToCheck As SafeTokenHandle = Nothing
            Dim pElevationType As IntPtr = IntPtr.Zero
            Dim pLinkedToken As IntPtr = IntPtr.Zero
            Dim cbSize As Integer = 0

            Try
                ' Open the access token of the current process for query and duplicate.
                If (Not NativeMethod.OpenProcessToken(Process.GetCurrentProcess.Handle, NativeMethod.TOKEN_QUERY Or NativeMethod.TOKEN_DUPLICATE, hToken)) Then
                    Throw New Win32Exception(Marshal.GetLastWin32Error)
                End If

                ' Determine whether system is running Windows Vista or later operating 
                ' systems (major version >= 6) because they support linked tokens, but 
                ' previous versions (major version < 6) do not.
                If (Environment.OSVersion.Version.Major >= 6) Then
                    ' Running Windows Vista or later (major version >= 6). 
                    ' Determine token type: limited, elevated, or default. 

                    ' Allocate a buffer for the elevation type information.
                    cbSize = 4  ' Size of TOKEN_ELEVATION_TYPE
                    pElevationType = Marshal.AllocHGlobal(cbSize)
                    If (pElevationType = IntPtr.Zero) Then
                        Throw New Win32Exception(Marshal.GetLastWin32Error)
                    End If

                    ' Retrieve token elevation type information.
                    If (Not NativeMethod.GetTokenInformation(hToken, TOKEN_INFORMATION_CLASS.TokenElevationType, pElevationType, cbSize, cbSize)) Then
                        Throw New Win32Exception(Marshal.GetLastWin32Error)
                    End If

                    ' Marshal the TOKEN_ELEVATION_TYPE enum from native to .NET.
                    Dim elevType As TOKEN_ELEVATION_TYPE = Marshal.ReadInt32(pElevationType)

                    ' If limited, get the linked elevated token for further check.
                    If (elevType = TOKEN_ELEVATION_TYPE.TokenElevationTypeLimited) Then
                        ' Allocate a buffer for the linked token.
                        cbSize = IntPtr.Size
                        pLinkedToken = Marshal.AllocHGlobal(cbSize)
                        If (pLinkedToken = IntPtr.Zero) Then
                            Throw New Win32Exception(Marshal.GetLastWin32Error)
                        End If

                        ' Get the linked token.
                        If (Not NativeMethod.GetTokenInformation(hToken, TOKEN_INFORMATION_CLASS.TokenLinkedToken, pLinkedToken, cbSize, cbSize)) Then
                            Throw New Win32Exception(Marshal.GetLastWin32Error)
                        End If

                        ' Marshal the linked token value from native to .NET.
                        Dim hLinkedToken As IntPtr = Marshal.ReadIntPtr(pLinkedToken)
                        hTokenToCheck = New SafeTokenHandle(hLinkedToken)
                    End If
                End If

                ' CheckTokenMembership requires an impersonation token. If we just got a 
                ' linked token, it already is an impersonation token.  If we did not get 
                ' a linked token, duplicate the original into an impersonation token for 
                ' CheckTokenMembership.
                If (hTokenToCheck Is Nothing) Then
                    If (Not NativeMethod.DuplicateToken(hToken, SECURITY_IMPERSONATION_LEVEL.SecurityIdentification, hTokenToCheck)) Then
                        Throw New Win32Exception(Marshal.GetLastWin32Error)
                    End If
                End If

                ' Check if the token to be checked contains admin SID.
                Dim id As New WindowsIdentity(hTokenToCheck.DangerousGetHandle)
                Dim principal As New WindowsPrincipal(id)
                fInAdminGroup = principal.IsInRole(WindowsBuiltInRole.Administrator)
            Finally
                ' Centralized cleanup for all allocated resources. 
                If (Not hToken Is Nothing) Then
                    hToken.Close()
                    hToken = Nothing
                End If
                If (Not hTokenToCheck Is Nothing) Then
                    hTokenToCheck.Close()
                    hTokenToCheck = Nothing
                End If
                If (pElevationType <> IntPtr.Zero) Then
                    Marshal.FreeHGlobal(pElevationType)
                    pElevationType = IntPtr.Zero
                End If
                If (pLinkedToken <> IntPtr.Zero) Then
                    Marshal.FreeHGlobal(pLinkedToken)
                    pLinkedToken = IntPtr.Zero
                End If
            End Try

            Return fInAdminGroup
        End Function

        Private Declare Function InternetGetConnectedState Lib "wininet.dll" (ByRef dwflags As Long, ByVal dwReserved As Long) As Long

        Public Function checkForInternetConnection() As Boolean
            Dim LFlags As Long
            Return InternetGetConnectedState(LFlags, 0&)
        End Function

        Public Function getWebPageData(url As String) As String
            Try
                Dim inStream As System.IO.StreamReader
                Dim webRequest As Net.WebRequest
                Dim webresponse As Net.WebResponse

                webRequest = Net.WebRequest.Create(url)
                webresponse = webRequest.GetResponse()
                inStream = New System.IO.StreamReader(webresponse.GetResponseStream())
                Return inStream.ReadToEnd.Trim().Replace(vbLf, vbCrLf)
            Catch ex As Exception
                Return ERROR_WEBREQUEST
            End Try
        End Function

        Public Function doesTaskExist(ByVal nameOfTask As String, ByRef taskObject As Task) As Boolean
            Using taskServiceObject As TaskService = New TaskService()
                taskObject = taskServiceObject.GetTask(nameOfTask)

                If taskObject Is Nothing Then
                    Return False
                Else
                    Return True
                End If
            End Using
        End Function

        Public Sub renameRegistrySubKey(parentKey As RegistryKey, oldSubKeyName As String, newSubKeyName As String)
            copyRegistryKey(parentKey, oldSubKeyName, newSubKeyName)
            parentKey.DeleteSubKeyTree(oldSubKeyName)
            parentKey.Close()
            parentKey.Dispose()
        End Sub

        Private Sub copyRegistryKey(parentKey As RegistryKey, oldSubKeyName As String, newSubKeyName As String)
            recurseCopyRegistryKey(parentKey.OpenSubKey(oldSubKeyName), parentKey.CreateSubKey(newSubKeyName))
        End Sub

        Private Sub recurseCopyRegistryKey(sourceKey As RegistryKey, destinationKey As RegistryKey)
            For Each valueName As String In sourceKey.GetValueNames()
                destinationKey.SetValue(valueName, sourceKey.GetValue(valueName), sourceKey.GetValueKind(valueName))
            Next

            For Each sourceSubKeyName As String In sourceKey.GetSubKeyNames()
                recurseCopyRegistryKey(sourceKey.OpenSubKey(sourceSubKeyName), destinationKey.CreateSubKey(sourceSubKeyName))
            Next
        End Sub

        Public Function whatTypeIsIt(type As Integer) As String
            If type = RestoreType.ApplicationInstall Then
                Return "Application Install"
            ElseIf type = RestoreType.ApplicationUninstall Then
                Return "Application Removal"
            ElseIf type = RestoreType.BackupRecovery Then
                Return "Backup Recovery"
            ElseIf type = RestoreType.CancelledOperation Then
                Return "Cancelled Operation"
            ElseIf type = RestoreType.Checkpoint Then
                Return "System Checkpoint"
            ElseIf type = RestoreType.DeviceDriverInstall Then
                Return "Device Driver Install"
            ElseIf type = RestoreType.FirstRun Then
                Return "First Run"
            ElseIf type = RestoreType.ModifySettings Then
                Return "Settings Modified"
            ElseIf type = RestoreType.Restore Then
                Return "Restore"
            ElseIf type = RestoreType.WindowsType Then
                Return "System Restore Point"
            Else
                Return "Unknown Type"
            End If
        End Function

        Public Sub doDeletingOfXNumberOfRestorePoints(ByVal shortHowManyToKeep As Short)
            Dim systemRestorePoints As New ManagementObjectSearcher("root\DEFAULT", "SELECT * FROM SystemRestore")

            If systemRestorePoints.Get.Count <> 0 Then
                Dim numberOfRestorePointsToBeDeleted As Short = systemRestorePoints.Get.Count - shortHowManyToKeep

                If numberOfRestorePointsToBeDeleted < 0 Or numberOfRestorePointsToBeDeleted = 0 Then
                    systemRestorePoints.Dispose()
                    systemRestorePoints = Nothing
                    Exit Sub
                Else
                    For Each systemRestorePoint As ManagementObject In systemRestorePoints.Get()
                        If numberOfRestorePointsToBeDeleted = 0 Then
                            Exit For
                        Else
                            numberOfRestorePointsToBeDeleted -= 1
                            Functions.SRRemoveRestorePoint(Integer.Parse(systemRestorePoint("SequenceNumber")))
                        End If
                    Next
                End If
            End If

            systemRestorePoints.Dispose()
            systemRestorePoints = Nothing
        End Sub

        Private Function doesPIDExist(PID As Integer) As Boolean
            Try
                Dim searcher As New ManagementObjectSearcher("root\CIMV2", String.Format("SELECT * FROM Win32_Process WHERE ProcessId={0}", PID))

                If searcher.Get.Count = 0 Then
                    searcher.Dispose()
                    Return False
                Else
                    searcher.Dispose()
                    Return True
                End If
            Catch ex3 As System.Runtime.InteropServices.COMException
                Functions.giveComExceptionCrashMessage()
                Return False
            Catch ex As Exception
                Return False
            End Try
        End Function

        Private Sub killProcess(PID As Integer)
            Dim processDetail As Process

            Debug.Write(String.Format("Killing PID {0}...", PID))

            processDetail = Process.GetProcessById(PID)
            processDetail.Kill()

            Threading.Thread.Sleep(100)

            If doesPIDExist(PID) Then
                'debug.writeline(" Process still running.  Attempting to kill process again.")
                killProcess(PID)
            Else
                'debug.writeline(" Process Killed.")
            End If
        End Sub

        Public Sub searchForProcessAndKillIt(fileName As String)
            Dim fullFileName As String = New FileInfo(fileName).FullName
            'Dim PID As Integer

            'debug.writeline("Killing all processes that belong to parent executable file.  Please Wait.")
            'Console.WriteLine(String.Format("SELECT * FROM Win32_Process WHERE ExecutablePath = '{0}'", fullFileName.Replace("\", "\\")))

            Dim searcher As New ManagementObjectSearcher("root\CIMV2", "SELECT * FROM Win32_Process")

            Try
                For Each queryObj As ManagementObject In searcher.Get()
                    If queryObj("ExecutablePath") IsNot Nothing Then
                        If queryObj("ExecutablePath").ToString = fullFileName Then
                            killProcess(Integer.Parse(queryObj("ProcessId").ToString))
                        End If
                    End If
                Next

                'debug.writeline("All processes killed... Update process can continue.")
            Catch ex3 As System.Runtime.InteropServices.COMException
                Functions.giveComExceptionCrashMessage()
            Catch err As ManagementException
                ' Does nothing
            End Try
        End Sub

        Public Enum RestoreType
            ApplicationInstall = 0 ' Installing a new application
            ApplicationUninstall = 1 ' An application has been uninstalled
            ModifySettings = 12 ' An application has had features added or removed
            CancelledOperation = 13 ' An application needs to delete the restore point it created
            Restore = 6 ' System Restore
            Checkpoint = 7 ' Checkpoint
            DeviceDriverInstall = 10 ' Device driver has been installed
            FirstRun = 11 ' Program used for 1st time 
            BackupRecovery = 14 ' Restoring a backup
            WindowsType = 16 ' The type of restore point that Windows makes.
        End Enum

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

        Sub writeCrashToEventLog(ex As Exception)
            System.Media.SystemSounds.Hand.Play()

            Dim stringBuilder As New System.Text.StringBuilder
            stringBuilder.AppendLine("Running As: " & Environment.UserName)
            stringBuilder.AppendLine("Message: " & ex.Message)
            stringBuilder.AppendLine("Exception Type: " & ex.GetType.ToString)

            stringBuilder.AppendLine()

            stringBuilder.Append("The exception occurred ")

            For Each lineInStackTrace As String In ex.StackTrace.Split(vbCrLf)
                stringBuilder.AppendLine(lineInStackTrace.Trim)
            Next

            writeToSystemEventLog(stringBuilder.ToString.Trim, EventLogEntryType.Error)
            stringBuilder = Nothing
        End Sub

        ' Call like this... enableSystemRestoreOnDrive("C:").  It accepts a drive letter.
        Public Function enableSystemRestoreOnDriveWMI(driveLetter As String) As Short
            Try
                Dim managementScopeObject As New ManagementScope("\\localhost\root\default")
                Dim managementPathObject As New ManagementPath("SystemRestore")
                Dim managementObjectOptions As New ObjectGetOptions()
                Dim managementClassObject As New ManagementClass(managementScopeObject, managementPathObject, managementObjectOptions)

                Dim managementBaseObjectParameters As ManagementBaseObject = managementClassObject.GetMethodParameters("Enable")
                managementBaseObjectParameters("Drive") = driveLetter
                managementBaseObjectParameters("WaitTillEnabled") = True

                Dim oOutParams As ManagementBaseObject = managementClassObject.InvokeMethod("Enable", managementBaseObjectParameters, Nothing)

                writeToSystemEventLog("Enabled System Restore on drive " & driveLetter.Substring(0, 1).ToUpper & ".", EventLogEntryType.Information)

                'MsgBox(oOutParams.Properties.Count)
                'For Each t In oOutParams.Properties
                '    MsgBox(t.Name)
                'Next

                Return oOutParams("ReturnValue")
            Catch ex3 As System.Runtime.InteropServices.COMException
                Functions.giveComExceptionCrashMessage()
                Return 1
            Catch ex As Exception
                Return 1
            End Try
        End Function

        Private Sub enableSystemRestoreOnSystemDrive()
            Dim driveLetter As String = Environment.SystemDirectory.Substring(0, 2)
            'debug.writeline("Repairing System Restore on System Drive " & driveLetter)
            enableSystemRestoreOnDrive(driveLetter)
        End Sub

        'Private Declare Function GetSystemMetrics Lib "user32" (ByVal nIndex As Long) As Long

        Public Function areWeInSafeMode() As Boolean
            If System.Windows.Forms.SystemInformation.BootMode = BootMode.Normal Then
                Return False
            Else
                Return True
            End If
            'Select Case GetSystemMetrics(67)
            '    Case 1 : Return True
            '    Case 2 : Return True
            '    Case Else : Return False
            'End Select
        End Function

        'Private Sub enableSystemRestoreOnAllSystemDrives()
        '    Dim boolDisableRepairOfSystemRestoreOnSystemDrives As Boolean

        '    If Boolean.TryParse(Microsoft.Win32.Registry.LocalMachine.OpenSubKey(GlobalVariables.strProgramRegistryKey).GetValue("Disable Repair of System Restore on System Drives", "False"), boolDisableRepairOfSystemRestoreOnSystemDrives) Then
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

        Public Sub deleteAtUserLogonTask()
            Try
                Dim taskService As New TaskService

                For Each task As Task In taskService.RootFolder.SubFolders(GlobalVariables.taskFolder).Tasks
                    If task.Name = "Create a Restore Point at User Logon" Then
                        taskService.RootFolder.SubFolders(GlobalVariables.taskFolder).DeleteTask("Create a Restore Point at User Logon")
                    End If

                    task.Dispose()
                    task = Nothing
                Next

                taskService.Dispose()
                taskService = Nothing
            Catch ex As Exception
                Threading.Thread.CurrentThread.CurrentUICulture = New System.Globalization.CultureInfo("en-US")
                manuallyLoadCrashWindow(ex.Message, ex.StackTrace, ex.GetType)
            End Try
        End Sub

        Public Function parseSystemRestorePointCreationDate(strDate As String, Optional boolFullDateParsing As Boolean = True) As Date
            ' Parses out the date.  This is the old way of doing the below code with the use of a Regular Expression.
            'year = Mid$(strDate, 1, 4)
            'month = Mid$(strDate, 5, 2)
            'day = Mid$(strDate, 7, 2)

            Dim regexMatches As RegularExpressions.Match = GlobalVariables.regexRestorePointCreationTimeParser.Match(strDate)
            Dim year, month, day, second, minute, hour As Integer

            ' Gets the values out of the Regular Expression Matches object.
            With regexMatches
                year = Integer.Parse(.Groups("year").Value)
                month = Integer.Parse(.Groups("month").Value)
                day = Integer.Parse(.Groups("day").Value)
                second = Integer.Parse(.Groups("second").Value)
                minute = Integer.Parse(.Groups("minute").Value)
                hour = Integer.Parse(.Groups("hour").Value)
            End With

            If boolFullDateParsing = True Then
                Return New DateTime(year, month, day, hour, minute, second, DateTimeKind.Utc).ToLocalTime
            Else
                Return New Date(year, month, day)
            End If
        End Function

        Public Function doesAtUserLoginTaskExist(ByRef delayedTime As Short) As Boolean
            Try
                Dim nameOfTask As String = GlobalVariables.taskFolder & "\Create a Restore Point at User Logon"
                Dim taskObject As Microsoft.Win32.TaskScheduler.Task

                Using taskServiceObject As TaskService = New TaskService()
                    taskObject = taskServiceObject.GetTask(nameOfTask)

                    ' Makes sure that the task exists and we don't get a Null Reference Exception.
                    If taskObject IsNot Nothing Then
                        ' Makes sure that we have some triggers to actually work with.
                        If taskObject.Definition.Triggers.Count > 0 Then
                            Dim trigger As Trigger = taskObject.Definition.Triggers.Item(0)

                            delayedTime = CType(trigger, ITriggerDelay).Delay.Minutes

                            Return True
                        End If
                    End If
                End Using

                Return False
            Catch ex As Exception
                Return False
            End Try
        End Function

        Public Sub writeToSystemEventLog(message As String, type As System.Diagnostics.EventLogEntryType)
            If GlobalVariables.boolLogToSystemLog = True Then
                Try
                    Dim sSource As String = "System Restore Point Creator"
                    Dim sLog As String = "Application"
                    Dim sMachine As String = "."

                    If Microsoft.Win32.Registry.LocalMachine.OpenSubKey("SYSTEM\CurrentControlSet\services\eventlog\Application\" & sSource) Is Nothing Then
                        Microsoft.Win32.Registry.LocalMachine.OpenSubKey("SYSTEM\CurrentControlSet\services\eventlog\Application", True).CreateSubKey(sSource)
                        Microsoft.Win32.Registry.LocalMachine.OpenSubKey("SYSTEM\CurrentControlSet\services\eventlog\Application\" & sSource, True).SetValue("EventMessageFile", IO.Path.Combine(System.Runtime.InteropServices.RuntimeEnvironment.GetRuntimeDirectory(), "EventLogMessages.dll"), RegistryValueKind.String)
                    End If

                    If Not EventLog.SourceExists(sSource, sMachine) Then
                        EventLog.CreateEventSource(sSource, sLog, sMachine)
                    End If

                    Dim eventLogObject As New EventLog(sLog, sMachine, sSource)
                    eventLogObject.WriteEntry(message, type, 234, CType(3, Short))

                    eventLogObject.Dispose()
                    eventLogObject = Nothing
                Catch ex As Exception
                    ' Does nothing
                End Try
            End If
        End Sub

        Public Sub addAtUserLoginTask(Optional delayed As Boolean = False, Optional delayTimeInMinutes As Short = 10)
            Dim taskService As TaskService = New TaskService()
            Dim newTask As TaskDefinition = taskService.NewTask

            newTask.RegistrationInfo.Description = "Creates a Restore Point at User Logon"

            Dim logonTriggerDefinition As Microsoft.Win32.TaskScheduler.LogonTrigger = New Microsoft.Win32.TaskScheduler.LogonTrigger

            If delayed = True Then
                logonTriggerDefinition.Delay = New TimeSpan(0, 0, delayTimeInMinutes, 0, 0)
            End If

            newTask.Triggers.Add(logonTriggerDefinition)

            Dim exePathInfo As New System.IO.FileInfo(Application.ExecutablePath)
            newTask.Actions.Add(New ExecAction(Chr(34) & exePathInfo.FullName & Chr(34), "-createscheduledrestorepoint", exePathInfo.DirectoryName))
            exePathInfo = Nothing

            'If parameters = Nothing Then
            '    newTask.Actions.Add(New ExecAction(Chr(34) & txtEXEPath.Text & Chr(34), Nothing, exeFileInfo.DirectoryName))
            'Else
            '    newTask.Actions.Add(New ExecAction(Chr(34) & txtEXEPath.Text & Chr(34), parameters, exeFileInfo.DirectoryName))
            'End If

            newTask.Principal.RunLevel = TaskRunLevel.Highest
            newTask.Settings.Compatibility = TaskCompatibility.V2_1
            newTask.Settings.AllowDemandStart = True
            newTask.Settings.DisallowStartIfOnBatteries = False
            newTask.Settings.RunOnlyIfIdle = False
            newTask.Settings.StopIfGoingOnBatteries = False
            newTask.Settings.AllowHardTerminate = False
            newTask.Settings.UseUnifiedSchedulingEngine = True
            newTask.Settings.ExecutionTimeLimit = Nothing
            newTask.Principal.LogonType = TaskLogonType.InteractiveToken

            Dim taskName As String = "Create a Restore Point at User Logon"
            taskService.RootFolder.SubFolders(GlobalVariables.taskFolder).RegisterTaskDefinition(taskName, newTask)

            newTask.Dispose()
            taskService.Dispose()
            newTask = Nothing
            taskService = Nothing
        End Sub

        Function checkToSeeIfSystemRestoreIsEnabledOnSystemDrive() As Boolean
            Dim systemRestorePoints As New ManagementObjectSearcher("root\DEFAULT", "SELECT * FROM SystemRestore")

            If systemRestorePoints.Get().Count = 0 Then
                Return False
            Else
                Return True
            End If
        End Function

        Public Function createRestorePoint(strDescription As String, rt As RestoreType, ByRef lSeqNum As Long) As Integer
            Try
                ' This is such a damn hack but I have no idea how to code around it. ARG!
                enableSystemRestoreOnDriveWMI(Environment.SystemDirectory.Substring(0, 2))

                Dim shadowStorageDetails As ShadowStorageData = getShadowStorageData(getDeviceIDFromDriveLetter(Environment.SystemDirectory.Substring(0, 2)))

                Try
                    If shadowStorageDetails IsNot Nothing Or checkToSeeIfSystemRestoreIsEnabledOnSystemDrive() = False Then
                        If shadowStorageDetails.MaxSpace = 0 Then
                            If GlobalVariables.didWeFixSystemRestoreForTheSystemDrivesThisTime = False Then
                                'enableSystemRestoreOnAllSystemDrives()
                                enableSystemRestoreOnSystemDrive()
                                GlobalVariables.didWeFixSystemRestoreForTheSystemDrivesThisTime = True
                            End If
                        End If
                    Else
                        If GlobalVariables.didWeFixSystemRestoreForTheSystemDrivesThisTime = False Then
                            ' Something isn't right with the system, we need to try and fix it.
                            enableSystemRestoreOnSystemDrive()
                            GlobalVariables.didWeFixSystemRestoreForTheSystemDrivesThisTime = True
                        End If
                    End If
                Catch ex As NullReferenceException
                    enableSystemRestoreOnSystemDrive()
                    GlobalVariables.didWeFixSystemRestoreForTheSystemDrivesThisTime = True
                End Try

                Dim managementScopeObject As New ManagementScope("\\localhost\root\default")
                Dim managementPathObject As New ManagementPath("SystemRestore")
                Dim managementObjectOptions As New ObjectGetOptions()
                Dim managementClassObject As New ManagementClass(managementScopeObject, managementPathObject, managementObjectOptions)

                Dim managementBaseObjectParameters As ManagementBaseObject = managementClassObject.GetMethodParameters("CreateRestorePoint")
                managementBaseObjectParameters("Description") = strDescription
                managementBaseObjectParameters("RestorePointType") = rt
                managementBaseObjectParameters("EventType") = 100

                writeToSystemEventLog("Created System Restore Point (" & strDescription & ").", EventLogEntryType.Information)
                Dim oOutParams As ManagementBaseObject = managementClassObject.InvokeMethod("CreateRestorePoint", managementBaseObjectParameters, Nothing)

                'MsgBox(oOutParams.Properties.Count)
                'For Each t In oOutParams.Properties
                '    MsgBox(t.Name)
                'Next

                Return oOutParams("ReturnValue")

                'Dim restorePointClassObject As Object
                'restorePointClassObject = GetObject("winmgmts:\\.\root\default:SystemRestore")
                'Dim result As Short = restorePointClassObject.CreateRestorePoint(strDescription, rt, 100)
                'lSeqNum = Functions.getNewestSystemRestorePointID
                'Return result
            Catch ex3 As System.Runtime.InteropServices.COMException
                writeCrashToEventLog(ex3)
                Functions.giveComExceptionCrashMessage()
                Return 0
            Catch ex As Exception
                Threading.Thread.CurrentThread.CurrentUICulture = New System.Globalization.CultureInfo("en-US")
                manuallyLoadCrashWindow(ex.Message, ex.StackTrace, ex.GetType)
                Return 0
            End Try
        End Function

        Public Sub restoreToSystemRestorePoint(point As Long)
            Try
                writeToSystemEventLog("Restoring system back to System Restore Point ID " & point & " (" & getRestorePointName(point) & ").", EventLogEntryType.Information)

                Dim managementScopeObject As New ManagementScope("\\localhost\root\default")
                Dim managementPathObject As New ManagementPath("SystemRestore")
                Dim managementObjectOptions As New ObjectGetOptions()
                Dim managementClassObject As New ManagementClass(managementScopeObject, managementPathObject, managementObjectOptions)

                Dim managementBaseObjectParameters As ManagementBaseObject = managementClassObject.GetMethodParameters("Restore")
                managementBaseObjectParameters("SequenceNumber") = point

                Dim oOutParams As ManagementBaseObject = managementClassObject.InvokeMethod("Restore", managementBaseObjectParameters, Nothing)

                'Dim managementObject As New ManagementObject("root\DEFAULT", "SystemRestore.ReplaceKeyPropery='ReplaceKeyPropertyValue'", Nothing)
                'Dim inParams As ManagementBaseObject = managementObject.GetMethodParameters("Restore")
                'inParams("SequenceNumber") = point
                'Dim outParams As ManagementBaseObject = managementObject.InvokeMethod("Restore", inParams, Nothing)
                'rebootSystem()

                'Dim systemRestorePointObject As Object = GetObject("winmgmts:\\.\root\Default:SystemRestore")
                'systemRestorePointObject.Restore(point)

                rebootSystem()
                'System.Diagnostics.Process.Start("shutdown.exe", "-r -t 0")
            Catch ex3 As System.Runtime.InteropServices.COMException
                Functions.giveComExceptionCrashMessage()
            Catch ex As Exception
                Threading.Thread.CurrentThread.CurrentUICulture = New System.Globalization.CultureInfo("en-US")
                manuallyLoadCrashWindow(ex.Message, ex.StackTrace, ex.GetType)
            End Try
        End Sub

        Private Function executeAddSafeModeCommand() As String ' Adds a new OS boot entry to the Windows BCD and returns the GUID of the new entry.
            Try
                Dim oProcess As New Process()
                Dim oStartInfo As New ProcessStartInfo("bcdedit", "/copy {current} /d ""Windows Safe Mode Boot""")
                oStartInfo.UseShellExecute = False
                oStartInfo.RedirectStandardOutput = True
                oProcess.StartInfo = oStartInfo
                oProcess.Start()

                Dim sOutput As String
                Using oStreamReader As System.IO.StreamReader = oProcess.StandardOutput
                    sOutput = oStreamReader.ReadToEnd()
                End Using

                If System.Text.RegularExpressions.Regex.IsMatch(sOutput, "(\{[a-zA-Z0-9-]*\})") Then
                    Return System.Text.RegularExpressions.Regex.Match(sOutput, "(\{[a-zA-Z0-9-]*\})").Groups(1).Value
                Else
                    Return "failed"
                End If
            Catch ex As Exception
                Return "failed"
            End Try
        End Function

        Public Sub setSafeModeBoot(Optional saveSettingAsNewBootEntry As Boolean = True)
            If saveSettingAsNewBootEntry = True Then
                Dim strGUIDForWindowsSafeModeBootEntry As String = executeAddSafeModeCommand()

                ' We need this later to remove the Safe Mode Boot Entry from the Windows BCD Store so we save the value in a Registry Value for later use.
                Registry.LocalMachine.OpenSubKey(GlobalVariables.strProgramRegistryKey, True).SetValue("Safe Mode Boot GUID", strGUIDForWindowsSafeModeBootEntry, RegistryValueKind.String)

                Shell("bcdedit /set " & strGUIDForWindowsSafeModeBootEntry & " safeboot minimal", AppWinStyle.Hide)
                Threading.Thread.Sleep(100)
                'Shell("bcdedit /default " & strGUIDForWindowsSafeModeBootEntry, AppWinStyle.Hide)
                'Threading.Thread.Sleep(100)
                Shell("bootcfg /timeout 5", AppWinStyle.Hide)
                Threading.Thread.Sleep(100)
            Else
                Shell("bcdedit /set {default} safeboot minimal", AppWinStyle.Hide)
            End If
        End Sub

        Public Sub removeSafeModeBoot(Optional bcdBootGUIDToDelete As String = "{none}")
            If bcdBootGUIDToDelete = "{none}" Then
                Shell("bcdedit /deletevalue {default} safeboot", AppWinStyle.Hide)
            Else
                Shell("bcdedit /delete " & bcdBootGUIDToDelete, AppWinStyle.Hide)
            End If
        End Sub

        Public Sub rebootSystem()
            Shell("shutdown.exe -r -t 0", AppWinStyle.Hide)

            'Dim t As Single, objWMIService, objComputer As Object
            'objWMIService = GetObject("Winmgmts:{impersonationLevel=impersonate,(Debug,Shutdown)}")

            'For Each objComputer In objWMIService.InstancesOf("Win32_OperatingSystem")
            '    t = objComputer.Win32Shutdown(2 + 4, 0)
            '    If t <> 0 Then
            '        MsgBox("Error occurred!!!")
            '    Else
            '        'LogOff your system
            '    End If
            'Next
        End Sub

        Function getRestorePointName(id As Long) As String
            Try
                Dim systemRestorePoints As New ManagementObjectSearcher("root\DEFAULT", "SELECT * FROM SystemRestore")

                If (systemRestorePoints.Get().Count = 0) = False Then
                    For Each systemRestorePoint As ManagementObject In systemRestorePoints.Get()
                        If Long.Parse(systemRestorePoint("SequenceNumber").ToString) = id Then
                            Return systemRestorePoint("Description").ToString.ToString
                        End If
                    Next
                End If

                writeToSystemEventLog("Unable to find description for restore point ID " & id & ".", EventLogEntryType.Error)
                Return "ERROR_NO_DESCRIPTION"
            Catch ex As Exception
                writeToSystemEventLog("Unable to find description for restore point ID " & id & ".", EventLogEntryType.Error)
                Return "ERROR_NO_DESCRIPTION"
            End Try
        End Function

        Function getNewestSystemRestorePointID() As Integer
            Try
                Dim newestSystemRestoreID As Integer = 0 ' Resets the newest System Restore ID to 0.

                ' Get all System Restore Points from the Windows Management System and puts then in the systemRestorePoints variable.
                Dim systemRestorePoints As New ManagementObjectSearcher("root\DEFAULT", "SELECT * FROM SystemRestore")

                Dim systemRestoreIDs As New ArrayList

                ' Checks to see if there are any System Restore Points to be listed.
                If (systemRestorePoints.Get().Count = 0) = False Then
                    ' Loops through systemRestorePoints.
                    For Each systemRestorePoint As ManagementObject In systemRestorePoints.Get()
                        systemRestoreIDs.Add(systemRestorePoint("SequenceNumber").ToString)
                    Next

                    newestSystemRestoreID = Integer.Parse(systemRestoreIDs.Item(systemRestoreIDs.Count - 1))
                Else
                    newestSystemRestoreID = 0
                End If

                Return newestSystemRestoreID
            Catch ex2 As ManagementException
                Return 0
            Catch ex3 As System.Runtime.InteropServices.COMException
                Functions.giveComExceptionCrashMessage()
                Return 0
            End Try
        End Function

        'Public Sub changeLanguage(ByVal lang As String, windowObject As Object)
        '    For Each c As Control In windowObject.Controls
        '        Dim resources As ComponentResourceManager = New ComponentResourceManager(windowObject.GetType)
        '        resources.ApplyResources(c, c.Name, New CultureInfo(lang))
        '    Next c
        'End Sub

        Public Function tryToParseShort(value As String) As Boolean
            Dim number As Short
            Dim result As Boolean = Short.TryParse(value, number)

            If result Then
                Return True
            Else
                Return False
            End If
        End Function

        Function getFileTypeHandler(fileType As String) As String
            Try
                Dim registryKey As Microsoft.Win32.RegistryKey = Microsoft.Win32.Registry.ClassesRoot.OpenSubKey(fileType, False)

                ' If registryKey Is Nothing = False Then
                If registryKey IsNot Nothing Then
                    Dim fileTypeNameInRegistry As String = registryKey.GetValue("")
                    Dim registryKey2 As Microsoft.Win32.RegistryKey = Microsoft.Win32.Registry.ClassesRoot.OpenSubKey(fileTypeNameInRegistry & "\shell\open\command", False)

                    ' If registryKey2 Is Nothing = False Then
                    If registryKey2 IsNot Nothing Then
                        Return registryKey2.GetValue("").ToString.Replace("""", "").Replace("%1", "").Trim
                    End If
                End If

                Return ""
            Catch ex As Exception
                Return ""
            End Try
        End Function

        'Sub loadIcon(objectGettingTheIcon As Object, image As Bitmap)
        '    objectGettingTheIcon.Image = image
        '    objectGettingTheIcon.ImageAlign = ContentAlignment.MiddleLeft
        'End Sub

        Public Function ResizeImage(SourceImage As Drawing.Image, TargetWidth As Int32, TargetHeight As Int32) As Drawing.Bitmap
            Dim bmSource = New Drawing.Bitmap(SourceImage)
            Return ResizeImage(bmSource, TargetWidth, TargetHeight)
        End Function

        Public Function ResizeImage(bmSource As Drawing.Bitmap, TargetWidth As Int32, TargetHeight As Int32) As Drawing.Bitmap
            Dim bmDest As New Drawing.Bitmap(TargetWidth, TargetHeight, Drawing.Imaging.PixelFormat.Format32bppArgb)

            Dim nSourceAspectRatio = bmSource.Width / bmSource.Height
            Dim nDestAspectRatio = bmDest.Width / bmDest.Height

            Dim NewX = 0
            Dim NewY = 0
            Dim NewWidth = bmDest.Width
            Dim NewHeight = bmDest.Height

            If nDestAspectRatio = nSourceAspectRatio Then
                'same ratio
            ElseIf nDestAspectRatio > nSourceAspectRatio Then
                'Source is taller
                NewWidth = Convert.ToInt32(Math.Floor(nSourceAspectRatio * NewHeight))
                NewX = Convert.ToInt32(Math.Floor((bmDest.Width - NewWidth) / 2))
            Else
                'Source is wider
                NewHeight = Convert.ToInt32(Math.Floor((1 / nSourceAspectRatio) * NewWidth))
                NewY = Convert.ToInt32(Math.Floor((bmDest.Height - NewHeight) / 2))
            End If

            Using grDest = Drawing.Graphics.FromImage(bmDest)
                With grDest
                    .CompositingQuality = Drawing.Drawing2D.CompositingQuality.HighQuality
                    .InterpolationMode = Drawing.Drawing2D.InterpolationMode.HighQualityBicubic
                    .PixelOffsetMode = Drawing.Drawing2D.PixelOffsetMode.HighQuality
                    .SmoothingMode = Drawing.Drawing2D.SmoothingMode.AntiAlias
                    .CompositingMode = Drawing.Drawing2D.CompositingMode.SourceOver

                    .DrawImage(bmSource, NewX, NewY, NewWidth, NewHeight)
                End With
            End Using

            Return bmDest
        End Function

        Sub cleanLogFile(pathToFile As String)
            Try
                If System.IO.File.Exists(pathToFile) = False Then
                    System.IO.File.Create(pathToFile)
                End If

                Dim streamWriter As New System.IO.StreamWriter(pathToFile)
                streamWriter.WriteLine("======================================================")
                streamWriter.WriteLine("== Restore Point Creator Restore Point Deletion Log ==")
                streamWriter.WriteLine("======================================================")
                streamWriter.Close()
                streamWriter.Dispose()
                streamWriter = Nothing
            Catch ex As Exception
            End Try
        End Sub

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

        ' driveLetter is just that, "C:"
        Function getShadowStorageSize(driveLetter As String) As Long
            Try
                driveLetter = driveLetter.ToUpper
                Dim volumeID As String = getDeviceIDFromDriveLetter(driveLetter)
                Dim returnedValue As String

                Dim searcher As New ManagementObjectSearcher("root\CIMV2", "SELECT * FROM Win32_ShadowStorage")

                For Each queryObj As ManagementObject In searcher.[Get]()
                    If queryObj("Volume").ToString.Contains(volumeID.Replace("\\?\Volume", "")) Then
                        If queryObj("MaxSpace") = Nothing Then
                            Return 0
                        Else
                            returnedValue = queryObj("MaxSpace")
                            searcher.Dispose()
                            queryObj.Dispose()
                            queryObj = Nothing
                            searcher = Nothing

                            Return returnedValue
                        End If

                    End If
                Next

                searcher.Dispose()

                Return 0
            Catch ex As Exception
                If Regex.IsMatch(ex.Message, "provider failure", RegexOptions.IgnoreCase) Then
                    giveMessageAboutShadowCopyServiceBeingBroken()
                    Return 0
                Else
                    Return 0
                End If
            End Try
        End Function

        Public Sub giveMessageAboutShadowCopyServiceBeingBroken()
            Dim pathToDefaultWebBrowser As String = Microsoft.Win32.Registry.ClassesRoot.OpenSubKey("http\shell\open\command").GetValue("")
            Dim matches As RegularExpressions.Match = Regex.Match(pathToDefaultWebBrowser, "((?:""|'){0,1}[A-Za-z]:\\.*\.(?:bat|bin|cmd|com|cpl|exe|gadget|inf1|ins|inx|isu|job|jse|lnk|msc|msi|msp|mst|paf|pif|ps1|reg|rgs|sct|shb|shs|u3p|vb|vbe|vbs|vbscript|ws|wsf)(?:""|'){0,1} {0,1})(.*)", RegexOptions.IgnoreCase)
            ' Dim matches As RegularExpressions.Match = Regex.Match(pathToDefaultWebBrowser, "(""{0,1}[A-Za-z]:\\.*\.(?:bat|bin|cmd|com|cpl|exe|gadget|inf1|ins|inx|isu|job|jse|lnk|msc|msi|msp|mst|paf|pif|ps1|reg|rgs|sct|shb|shs|u3p|vb|vbe|vbs|vbscript|ws|wsf)""{0,1} )(.*)", RegexOptions.IgnoreCase)
            pathToDefaultWebBrowser = matches.Groups(1).Value.Replace(Chr(34), "").Trim

            MsgBox("Shadow Copy Service Provider Error Detected!" & vbCrLf & vbCrLf & "Please reference the following Microsoft Support Knowledge Base Article..." & vbCrLf & "http://support.microsoft.com/kb/2738812" & vbCrLf & vbCrLf & "Your default web browser will now open to load the Microsoft Knowledgebase Article mentioned above.", MsgBoxStyle.Critical, "Restore Point Creator")

            Process.Start(pathToDefaultWebBrowser, "http://support.microsoft.com/kb/2738812")
        End Sub

        ' driveLetter is just that, "C:"
        Function getDriveLabel(driveLetter As String) As String
            driveLetter = driveLetter.ToUpper
            Dim volumeID As String = getDeviceIDFromDriveLetter(driveLetter)
            Dim returnedValue As String

            Dim searcher As New ManagementObjectSearcher("root\CIMV2", "SELECT * FROM Win32_LogicalDisk")

            For Each queryObj As ManagementObject In searcher.[Get]()
                If queryObj("Name").ToString = driveLetter Then
                    If queryObj("VolumeName") = Nothing Then
                        Return Nothing
                    Else
                        returnedValue = queryObj("VolumeName")
                        searcher.Dispose()
                        queryObj.Dispose()
                        queryObj = Nothing
                        searcher = Nothing

                        Return returnedValue
                    End If
                End If
            Next

            searcher.Dispose()

            Return Nothing
        End Function

        ' driveLetter is just that, "C:"
        Sub setShadowStorageSize(driveLetter As String, size As Long)
            Try
                driveLetter = driveLetter.ToUpper
                Dim volumeID As String = addSlashes(getDeviceIDFromDriveLetter(driveLetter))

                Dim searcher As New ManagementObjectSearcher("root\CIMV2", "SELECT * FROM Win32_ShadowStorage")

                For Each queryObj As ManagementObject In searcher.Get
                    If queryObj("Volume").ToString.Contains(volumeID) Then
                        queryObj("MaxSpace") = size
                        queryObj.Put()

                        queryObj.Dispose()
                        searcher.Dispose()
                        queryObj = Nothing
                        searcher = Nothing
                        Exit Sub
                    End If
                Next

                searcher.Dispose()
                searcher = Nothing
            Catch ex As Exception
                If Regex.IsMatch(ex.Message.ToString(New System.Globalization.CultureInfo("en-US")), "provider failure", RegexOptions.IgnoreCase) Then
                    giveMessageAboutShadowCopyServiceBeingBroken()
                Else
                    Threading.Thread.CurrentThread.CurrentUICulture = New System.Globalization.CultureInfo("en-US")
                    manuallyLoadCrashWindow(ex.Message, ex.StackTrace, ex.GetType)
                End If
            End Try
        End Sub

        ' driveLetter is just that, "C:"
        Public Function getDeviceIDFromDriveLetter(driveLetter As String) As String
            Dim objMOC As Management.ManagementObjectCollection
            Dim objMOS As Management.ManagementObjectSearcher = New Management.ManagementObjectSearcher("SELECT * FROM Win32_Volume")
            Dim objMO As Management.ManagementObject
            Dim returnedValue As String

            objMOC = objMOS.Get()

            For Each objMO In objMOC
                If driveLetter = objMO("DriveLetter") Then
                    If objMO("DeviceID") = Nothing Then
                        Return 0
                    Else
                        returnedValue = objMO("DeviceID")

                        objMOC.Dispose()
                        objMOS.Dispose()
                        objMO.Dispose()
                        objMOC = Nothing
                        objMOS = Nothing
                        objMO = Nothing

                        Return returnedValue
                    End If
                End If
            Next

            objMOC.Dispose()
            objMOS.Dispose()

            Return 0
        End Function

        Public Function areWeRunningAsSystemUser() As Boolean
            'Dim identity As System.Security.Principal.WindowsIdentity = System.Security.Principal.WindowsIdentity.GetCurrent()
            'Return identity.IsSystem
            Return System.Security.Principal.WindowsIdentity.GetCurrent().IsSystem
        End Function

        Public Function areWeAnAdministrator() As Boolean
            Try
                Dim identity As System.Security.Principal.WindowsIdentity = System.Security.Principal.WindowsIdentity.GetCurrent()
                Dim principal As System.Security.Principal.WindowsPrincipal = New System.Security.Principal.WindowsPrincipal(identity)

                If principal.IsInRole(System.Security.Principal.WindowsBuiltInRole.Administrator) = True Then
                    identity.Dispose()
                    Return True
                Else
                    identity.Dispose()
                    Return False
                End If
            Catch ex As Exception
                Return False
            End Try
        End Function

        Public Function fileSizeToHumanReadableFormat(ByVal size As Long) As String
            Dim result As String

            If size <= (2 ^ 10) Then
                result = size & " Bytes"
            ElseIf size > (2 ^ 10) And size <= (2 ^ 20) Then
                result = System.Math.Round(size / (2 ^ 10), 2) & " KBs"
            ElseIf size > (2 ^ 20) And size <= (2 ^ 30) Then
                result = System.Math.Round(size / (2 ^ 20), 2) & " MBs"
            ElseIf size > (2 ^ 30) And size <= (2 ^ 40) Then
                result = System.Math.Round(size / (2 ^ 30), 2) & " GBs"
            ElseIf size > (2 ^ 40) And size <= (2 ^ 50) Then
                result = System.Math.Round(size / (2 ^ 40), 2) & " TBs"
            ElseIf size > (2 ^ 50) And size <= (2 ^ 60) Then
                result = System.Math.Round(size / (2 ^ 50), 2) & " PBs"
            Else
                result = "(None)"
            End If

            Return result
        End Function

        Public Sub enableSystemRestoreOnDrive(driveLetter)
            If GlobalVariables.boolWinXP = False Then
                Dim runningProcess As Process
                'runningProcess = Process.Start("vssadmin", String.Format("Resize ShadowStorage /For={0} /On={0} /MaxSize=1%", driveLetter))
                'Else
                Dim startInfo As New ProcessStartInfo
                startInfo.FileName = "vssadmin"
                startInfo.Arguments = String.Format("Resize ShadowStorage /For={0} /On={0} /MaxSize=5%", driveLetter)
                startInfo.Verb = "runas"
                startInfo.UseShellExecute = False
                startInfo.CreateNoWindow = True
                runningProcess = Process.Start(startInfo)
                runningProcess.WaitForExit()

                writeToSystemEventLog("No system restore point storage space was assigned for the system drive, this issue has been corrected.", EventLogEntryType.Information)
                enableSystemRestoreOnDriveWMI(driveLetter)
            End If
        End Sub

        Function getShadowStorageData(volumeID As String) As ShadowStorageData ' Accepts both a Volume ID and a drive letter.
            If volumeID.Length = 2 Then
                volumeID = getDeviceIDFromDriveLetter(volumeID)
            End If

            Try
                Dim searcher As New ManagementObjectSearcher("root\CIMV2", "SELECT * FROM Win32_ShadowStorage")

                For Each queryObj As ManagementObject In searcher.[Get]()
                    If queryObj("Volume").ToString.Contains(volumeID.Replace("\\?\Volume", "")) Then
                        Dim shadowStorageDataInstance As New ShadowStorageData

                        ' This is all in an effort to try and prevent Null Reference Exceptions.
                        If queryObj("AllocatedSpace") Is Nothing Then
                            shadowStorageDataInstance.AllocatedSpace = 0
                        Else
                            If Long.TryParse(queryObj("AllocatedSpace"), shadowStorageDataInstance.AllocatedSpace) = False Then
                                shadowStorageDataInstance.AllocatedSpace = 0
                            End If
                        End If

                        If queryObj("DiffVolume") Is Nothing Then
                            shadowStorageDataInstance.DiffVolume = Nothing
                        Else
                            shadowStorageDataInstance.DiffVolume = queryObj("DiffVolume")
                        End If

                        If queryObj("MaxSpace") Is Nothing Then
                            shadowStorageDataInstance.MaxSpace = 0
                        Else
                            If Long.TryParse(queryObj("MaxSpace"), shadowStorageDataInstance.MaxSpace) = False Then
                                shadowStorageDataInstance.MaxSpace = 0
                            End If
                        End If

                        If queryObj("UsedSpace") Is Nothing Then
                            shadowStorageDataInstance.UsedSpace = 0
                        Else
                            If Long.TryParse(queryObj("UsedSpace"), shadowStorageDataInstance.UsedSpace) = False Then
                                shadowStorageDataInstance.UsedSpace = 0
                            End If
                        End If

                        If queryObj("Volume") Is Nothing Then
                            shadowStorageDataInstance.Volume = Nothing
                        Else
                            shadowStorageDataInstance.Volume = queryObj("Volume")
                        End If

                        searcher.Dispose()
                        queryObj.Dispose()
                        searcher = Nothing
                        queryObj = Nothing

                        Return shadowStorageDataInstance
                    End If
                Next

                searcher.Dispose()

                Return Nothing
            Catch ex As Exception
                Return New ShadowStorageData
            End Try
        End Function

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
    End Module

    Public Class ShadowStorageData
        Public AllocatedSpace As Long = 0
        Public DiffVolume As String = Nothing
        Public MaxSpace As Long = 0
        Public UsedSpace As Long = 0
        Public Volume As String = Nothing
    End Class
End Namespace