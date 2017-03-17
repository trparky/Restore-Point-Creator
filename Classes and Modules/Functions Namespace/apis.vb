Imports System.Runtime.InteropServices
Imports System.Text

Namespace Functions.APIs
    Friend NotInheritable Class NativeMethods
        Private Sub New()
        End Sub

        ''' <summary>
        ''' Contains status information used by the SRSetRestorePoint function
        ''' </summary>
        <StructLayout(LayoutKind.Sequential)>
        Friend Structure STATEMGRSTATUS
            Public nStatus As Integer
            ' The status code
            Public llSequenceNumber As Int64
            ' The sequence number of the restore point
        End Structure

        ''' <summary>
        ''' Contains information used by the SRSetRestorePoint function
        ''' </summary>
        <StructLayout(LayoutKind.Sequential, CharSet:=CharSet.Unicode)>
        Friend Structure RestorePointInfo
            Public dwEventType As Integer
            ' The type of event
            Public dwRestorePtType As Integer
            ' The type of restore point
            Public llSequenceNumber As Int64
            ' The sequence number of the restore point
            <MarshalAs(UnmanagedType.ByValTStr, SizeConst:=systemRestore.MaxDescW + 1)>
            Public szDescription As String
            ' The description to be displayed so the user can easily identify a restore point
        End Structure

        <DllImport("srclient.dll", CharSet:=CharSet.Unicode)>
        Friend Shared Function SRSetRestorePointW(ByRef pRestorePtSpec As restorePointInfo, ByRef pSMgrStatus As STATEMGRSTATUS) As <MarshalAs(UnmanagedType.Bool)> Boolean
        End Function

        <DllImport("kernel32.dll", SetLastError:=True, CharSet:=CharSet.Unicode, ThrowOnUnmappableChar:=True, BestFitMapping:=False)>
        Friend Shared Function SearchPath(lpPath As String, lpFileName As String, lpExtension As String, nBufferLength As Integer, <MarshalAs(UnmanagedType.LPTStr)> lpBuffer As StringBuilder, lpFilePart As String) As UInteger
        End Function

        <DllImport("kernel32.dll", CharSet:=CharSet.Unicode)>
        Friend Shared Function QueryFullProcessImageName(hprocess As IntPtr, dwFlags As Integer, lpExeName As Text.StringBuilder, ByRef size As Integer) As Boolean
        End Function

        <DllImport("kernel32.dll")>
        Friend Shared Function OpenProcess(dwDesiredAccess As ProcessAccessFlags, bInheritHandle As Boolean, dwProcessId As Integer) As IntPtr
        End Function

        <DllImport("kernel32.dll", SetLastError:=True)>
        Friend Shared Function CloseHandle(hHandle As IntPtr) As Boolean
        End Function
    End Class

    Public Class systemRestore
        ' Type of restorations
        Public Enum RestoreType
            ApplicationInstall = 0
            ' Installing a new application
            ApplicationUninstall = 1
            ' An application has been uninstalled
            ModifySettings = 12
            ' An application has had features added or removed
            CancelledOperation = 13
            ' An application needs to delete the restore point it created
            Restore = 6
            ' System Restore
            Checkpoint = 7
            ' Checkpoint
            DeviceDriverInstall = 10
            ' Device driver has been installed
            FirstRun = 11
            ' Program used for 1st time 
            BackupRecovery = 14
            ' Restoring a backup
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

        ''' <summary>
        ''' Verifies that the OS can do system restores
        ''' </summary>
        ''' <returns>True if OS is either ME,XP,Vista,7</returns>
        Public Shared Function SysRestoreAvailable() As Boolean
            Dim majorVersion As Integer = Environment.OSVersion.Version.Major
            Dim minorVersion As Integer = Environment.OSVersion.Version.Minor

            Dim sbPath As New StringBuilder(260)

            ' See if DLL exists
            If NativeMethods.SearchPath(Nothing, "srclient.dll", Nothing, 260, sbPath, Nothing) <> 0 Then
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

            ' Windows Se7en
            If majorVersion = 6 AndAlso minorVersion = 1 Then
                Return True
            End If

            ' Windows 10
            If majorVersion = 10 Then
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
        ''' <remarks>Use EndRestore() or CancelRestore() to end the system restore</remarks>
        Public Shared Function StartRestore(strDescription As String, rt As RestoreType, ByRef lSeqNum As Long) As Integer
            Dim rpInfo As New NativeMethods.RestorePointInfo()
            Dim rpStatus As New NativeMethods.STATEMGRSTATUS()

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

                NativeMethods.SRSetRestorePointW(rpInfo, rpStatus)
            Catch generatedExceptionName As DllNotFoundException
                lSeqNum = 0
                Return -1
            End Try

            lSeqNum = rpStatus.llSequenceNumber

            Return rpStatus.nStatus
        End Function

        ''' <summary>
        ''' Ends system restore call
        ''' </summary>
        ''' <param name="lSeqNum">The restore sequence number</param>
        ''' <returns>The status of call</returns>
        Public Shared Function EndRestore(lSeqNum As Long) As Integer
            Dim rpInfo As New NativeMethods.RestorePointInfo()
            Dim rpStatus As New NativeMethods.STATEMGRSTATUS()

            If Not SysRestoreAvailable() Then
                Return -1
            End If

            Try
                rpInfo.dwEventType = EndSystemChange
                rpInfo.llSequenceNumber = lSeqNum

                NativeMethods.SRSetRestorePointW(rpInfo, rpStatus)
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
            Dim rpInfo As New NativeMethods.RestorePointInfo()
            Dim rpStatus As New NativeMethods.STATEMGRSTATUS()

            If Not SysRestoreAvailable() Then
                Return -1
            End If

            Try
                rpInfo.dwEventType = EndSystemChange
                rpInfo.dwRestorePtType = CInt(RestoreType.CancelledOperation)
                rpInfo.llSequenceNumber = lSeqNum

                NativeMethods.SRSetRestorePointW(rpInfo, rpStatus)
            Catch generatedExceptionName As DllNotFoundException
                Return -1
            End Try

            Return rpStatus.nStatus
        End Function
    End Class

    Module APIs
        <Flags>
        Public Enum ProcessAccessFlags As UInteger
            PROCESS_QUERY_LIMITED_INFORMATION = &H1000
            All = &H1F0FFF
            Terminate = &H1
            CreateThread = &H2
            VirtualMemoryOperation = &H8
            VirtualMemoryRead = &H10
            VirtualMemoryWrite = &H20
            DuplicateHandle = &H40
            CreateProcess = &H80
            SetQuota = &H100
            SetInformation = &H200
            QueryInformation = &H400
            QueryLimitedInformation = &H1000
            Synchronize = &H100000
        End Enum

        Public Enum errorCodes As Integer
            ERROR_SUCCESS = 0
            ERROR_ACCESS_DENIED = 5
            ERROR_BAD_ENVIRONMENT = 10
            ERROR_INVALID_DATA = 13
            ERROR_DISK_FULL = 112
            ERROR_SERVICE_DISABLED = 1058
            ERROR_INTERNAL_ERROR = 1359
            ERROR_TIMEOUT = 1460
        End Enum
    End Module
End Namespace