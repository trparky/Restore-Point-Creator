Imports System.Collections.Generic
Imports System.Text
Imports System.Runtime.InteropServices

Namespace SystemRestorePointCreator.Classes
    Public Class SystemRestore
        <DllImport("srclient.dll")> _
        Friend Shared Function SRSetRestorePointW(ByRef pRestorePtSpec As RestorePointInfo, ByRef pSMgrStatus As STATEMGRSTATUS) As <MarshalAs(UnmanagedType.Bool)> Boolean
        End Function

        <DllImport("kernel32.dll", SetLastError:=True, CharSet:=CharSet.Auto)> _
        Friend Shared Function SearchPath(lpPath As String, lpFileName As String, lpExtension As String, nBufferLength As Integer, <MarshalAs(UnmanagedType.LPTStr)> lpBuffer As StringBuilder, lpFilePart As String) As UInteger
        End Function

        <DllImport("Srclient.dll")> _
        Public Shared Function SRRemoveRestorePoint(index As Integer) As Integer
        End Function

        'Public Shared newestRestorePointID As Integer

        ''' <summary>
        ''' Contains information used by the SRSetRestorePoint function
        ''' </summary>
        <StructLayout(LayoutKind.Sequential, CharSet:=CharSet.Unicode)> _
        Friend Structure RestorePointInfo
            Public dwEventType As Integer
            ' The type of event
            Public dwRestorePtType As Integer
            ' The type of restore point
            Public llSequenceNumber As Int64
            ' The sequence number of the restore point
            <MarshalAs(UnmanagedType.ByValTStr, SizeConst:=MaxDescW + 1)> _
            Public szDescription As String
            ' The description to be displayed so the user can easily identify a restore point
        End Structure

        ''' <summary>
        ''' Contains status information used by the SRSetRestorePoint function
        ''' </summary>
        <StructLayout(LayoutKind.Sequential)> _
        Friend Structure STATEMGRSTATUS
            Public nStatus As Integer
            ' The status code
            Public llSequenceNumber As Int64
            ' The sequence number of the restore point
        End Structure

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

        ' Type of restorations
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

        ' Constants
        Friend Const BeginSystemChange As Int16 = 100
        ' Start of operation 
        Friend Const EndSystemChange As Int16 = 101
        ' End of operation
        ' Windows XP only - used to prevent the restore points intertwined
        Friend Const BeginNestedSystemChange As Int16 = 102
        Friend Const EndNestedSystemChange As Int16 = 103

        Friend Const DesktopSetting As Int16 = 2
        ' not implemented 
        Friend Const AccessibilitySetting As Int16 = 3
        ' not implemented 
        Friend Const OeSetting As Int16 = 4
        ' not implemented 
        Friend Const ApplicationRun As Int16 = 5
        ' not implemented 
        Friend Const WindowsShutdown As Int16 = 8
        ' not implemented 
        Friend Const WindowsBoot As Int16 = 9
        ' not implemented 
        Friend Const MaxDesc As Int16 = 64
        Friend Const MaxDescW As Int16 = 256

        Public Const ERROR_SUCCESS As Integer = 0
        Public Const ERROR_BAD_ENVIRONMENT As Integer = 10
        Public Const ERROR_DISK_FULL As Integer = 112
        Public Const ERROR_INTERNAL_ERROR As Integer = 1359
        Public Const ERROR_INVALID_DATA As Integer = 13
        Public Const ERROR_SERVICE_DISABLED As Integer = 1058
        Public Const ERROR_TIMEOUT As Integer = 1460

        ''' <summary>
        ''' Verifies that the OS can do system restores
        ''' </summary>
        ''' <returns>True if OS is either ME,XP,Vista,7,8</returns>
        Public Shared Function SysRestoreAvailable() As Boolean
            Dim majorVersion As Integer = Environment.OSVersion.Version.Major
            Dim minorVersion As Integer = Environment.OSVersion.Version.Minor

            Dim sbPath As New StringBuilder(260)

            ' See if DLL exists
            If SearchPath(Nothing, "srclient.dll", Nothing, 260, sbPath, Nothing) <> 0 Then
                Return True
            End If

            ' Windows ME
            If majorVersion = 4 AndAlso minorVersion = 90 Then
                Return True
            End If

            ' Windows XP
            If majorVersion = 5 AndAlso minorVersion = 1 Then
                Return True
            End If

            ' Windows Vista
            If majorVersion = 6 AndAlso minorVersion = 0 Then
                Return True
            End If

            ' Windows Seven
            If majorVersion = 6 AndAlso minorVersion = 1 Then
                Return True
            End If

            ' Windows 8
            If majorVersion = 6 AndAlso minorVersion = 2 Then
                Return True
            End If

            ' Windows 8.1
            If majorVersion = 6 AndAlso minorVersion = 3 Then
                Return True
            End If

            ' All others : Win 95, 98, 2000, Server
            Return False
        End Function

        ''' <summary>
        ''' Starts system restore
        ''' </summary>
        ''' <param name="strDescription">The description of the restore</param>
        ''' <param name="rt">The type of restore point</param>
        ''' <param name="lSeqNum">Returns the sequence number</param>
        ''' <returns>The status of call</returns>
        ''' <seealso href="Use EndRestore() or CancelRestore() to end the system restore" />
        Public Shared Function StartRestore(strDescription As String, rt As RestoreType, ByRef lSeqNum As Long) As Integer
            Dim rpInfo As New RestorePointInfo()
            Dim rpStatus As New STATEMGRSTATUS()

            If Not SysRestoreAvailable() Then
                lSeqNum = 0
                Return -1
            End If

            Try
                ' Prepare Restore Point
                rpInfo.dwEventType = BeginSystemChange
                ' By default we create a verification system
                rpInfo.dwRestorePtType = CInt(rt)
                rpInfo.llSequenceNumber = 0
                rpInfo.szDescription = strDescription

                SRSetRestorePointW(rpInfo, rpStatus)
            Catch generatedExceptionName As DllNotFoundException
                lSeqNum = 0
                Return -1
            End Try

            lSeqNum = rpStatus.llSequenceNumber
            'newestRestorePointID = rpStatus.llSequenceNumber

            Return rpStatus.nStatus

            'Dim restorePointClassObject As Object = GetObject("winmgmts:\\.\root\default:SystemRestore")
            'If restorePointClassObject IsNot Nothing Then
            '    If restorePointClassObject.CreateRestorePoint("test restore point", 0, 100) = 0 Then
            '        MsgBox("Restore Point created successfully")
            '    Else
            '        MsgBox("Could not create restore point!")
            '    End If
            'End If
        End Function

        ''' <summary>
        ''' Ends system restore call
        ''' </summary>
        ''' <param name="lSeqNum">The restore sequence number</param>
        ''' <returns>The status of call</returns>
        Public Shared Function EndRestore(lSeqNum As Long) As Integer
            Dim rpInfo As New RestorePointInfo()
            Dim rpStatus As New STATEMGRSTATUS()

            If Not SysRestoreAvailable() Then
                Return -1
            End If

            Try
                rpInfo.dwEventType = EndSystemChange
                rpInfo.llSequenceNumber = lSeqNum

                SRSetRestorePointW(rpInfo, rpStatus)
            Catch generatedExceptionName As DllNotFoundException
                Return -1
            End Try

            Return rpStatus.nStatus
        End Function

        ''' <summary>
        ''' Cancels restore call
        ''' </summary>
        ''' <param name="lSeqNum">The restore sequence number</param>
        ''' <returns>The status of call</returns>
        Public Shared Function CancelRestore(lSeqNum As Long) As Integer
            Dim rpInfo As New RestorePointInfo()
            Dim rpStatus As New STATEMGRSTATUS()

            If Not SysRestoreAvailable() Then
                Return -1
            End If

            Try
                rpInfo.dwEventType = EndSystemChange
                rpInfo.dwRestorePtType = CInt(RestoreType.CancelledOperation)
                rpInfo.llSequenceNumber = lSeqNum

                SRSetRestorePointW(rpInfo, rpStatus)
            Catch generatedExceptionName As DllNotFoundException
                Return -1
            End Try

            Return rpStatus.nStatus
        End Function

        Public Sub restoreToSystemRestorePoint(point As Integer)
            Dim systemRestorePointObject As Object = GetObject("winmgmts:\\.\root\Default:SystemRestore")
            systemRestorePointObject.Restore(point)

            rebootSystem()
            'System.Diagnostics.Process.Start("shutdown.exe", "-r -t 0")
        End Sub

        Public Sub rebootSystem()
            Dim t As Single, objWMIService, objComputer As Object
            objWMIService = GetObject("Winmgmts:{impersonationLevel=impersonate,(Debug,Shutdown)}")

            For Each objComputer In objWMIService.InstancesOf("Win32_OperatingSystem")
                t = objComputer.Win32Shutdown(2 + 4, 0)
                If t <> 0 Then
                    MsgBox("Error occurred!!!")
                Else
                    'LogOff your system
                End If
            Next
        End Sub
    End Class
End Namespace