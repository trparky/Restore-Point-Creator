Imports System.Runtime.InteropServices
Imports System.Text

Namespace Functions.APIs
    Public Class systemRestore
        <DllImport("srclient.dll")>
        Friend Shared Function SRSetRestorePointW(ByRef pRestorePtSpec As RestorePointInfo, ByRef pSMgrStatus As STATEMGRSTATUS) As <MarshalAs(UnmanagedType.Bool)> Boolean
        End Function

        <DllImport("kernel32.dll", SetLastError:=True, CharSet:=CharSet.Auto)>
        Friend Shared Function SearchPath(lpPath As String, lpFileName As String, lpExtension As String, nBufferLength As Integer, <MarshalAs(UnmanagedType.LPTStr)> lpBuffer As StringBuilder, lpFilePart As String) As UInteger
        End Function

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
            <MarshalAs(UnmanagedType.ByValTStr, SizeConst:=MaxDescW + 1)>
            Public szDescription As String
            ' The description to be displayed so the user can easily identify a restore point
        End Structure

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

            ' Windows Se7en
            If majorVersion = 6 AndAlso minorVersion = 1 Then
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
        ''' <seealso cref="Use EndRestore() or CancelRestore() to end the system restore"/>
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

            Return rpStatus.nStatus
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
            ERROR_INVALID_FUNCTION = 1
            ERROR_FILE_NOT_FOUND = 2
            ERROR_PATH_NOT_FOUND = 3
            ERROR_TOO_MANY_OPEN_FILES = 4
            ERROR_ACCESS_DENIED = 5
            ERROR_INVALID_HANDLE = 6
            ERROR_ARENA_TRASHED = 7
            ERROR_NOT_ENOUGH_MEMORY = 8
            ERROR_INVALID_BLOCK = 9
            ERROR_BAD_ENVIRONMENT = 10
            ERROR_BAD_FORMAT = 11
            ERROR_INVALID_ACCESS = 12
            ERROR_INVALID_DATA = 13
            ERROR_OUTOFMEMORY = 14
            ERROR_INVALID_DRIVE = 15
            ERROR_CURRENT_DIRECTORY = 16
            ERROR_NOT_SAME_DEVICE = 17
            ERROR_NO_MORE_FILES = 18
            ERROR_WRITE_PROTECT = 19
            ERROR_BAD_UNIT = 20
            ERROR_NOT_READY = 21
            ERROR_BAD_COMMAND = 22
            ERROR_CRC = 23
            ERROR_BAD_LENGTH = 24
            ERROR_SEEK = 25
            ERROR_NOT_DOS_DISK = 26
            ERROR_SECTOR_NOT_FOUND = 27
            ERROR_OUT_OF_PAPER = 28
            ERROR_WRITE_FAULT = 29
            ERROR_READ_FAULT = 30
            ERROR_GEN_FAILURE = 31
            ERROR_SHARING_VIOLATION = 32
            ERROR_LOCK_VIOLATION = 33
            ERROR_WRONG_DISK = 34
            ERROR_SHARING_BUFFER_EXCEEDED = 36
            ERROR_HANDLE_EOF = 38
            ERROR_HANDLE_DISK_FULL = 39
            ERROR_NOT_SUPPORTED = 50
            ERROR_REM_NOT_LIST = 51
            ERROR_DUP_NAME = 52
            ERROR_BAD_NETPATH = 53
            ERROR_NETWORK_BUSY = 54
            ERROR_DEV_NOT_EXIST = 55
            ERROR_TOO_MANY_CMDS = 56
            ERROR_ADAP_HDW_ERR = 57
            ERROR_BAD_NET_RESP = 58
            ERROR_UNEXP_NET_ERR = 59
            ERROR_BAD_REM_ADAP = 60
            ERROR_PRINTQ_FULL = 61
            ERROR_NO_SPOOL_SPACE = 62
            ERROR_PRINT_CANCELLED = 63
            ERROR_NETNAME_DELETED = 64
            ERROR_NETWORK_ACCESS_DENIED = 65
            ERROR_BAD_DEV_TYPE = 66
            ERROR_BAD_NET_NAME = 67
            ERROR_TOO_MANY_NAMES = 68
            ERROR_TOO_MANY_SESS = 69
            ERROR_SHARING_PAUSED = 70
            ERROR_REQ_NOT_ACCEP = 71
            ERROR_REDIR_PAUSED = 72
            ERROR_FILE_EXISTS = 80
            ERROR_CANNOT_MAKE = 82
            ERROR_FAIL_I24 = 83
            ERROR_OUT_OF_STRUCTURES = 84
            ERROR_ALREADY_ASSIGNED = 85
            ERROR_INVALID_PASSWORD = 86
            ERROR_INVALID_PARAMETER = 87
            ERROR_NET_WRITE_FAULT = 88
            ERROR_NO_PROC_SLOTS = 89
            ERROR_TOO_MANY_SEMAPHORES = 100
            ERROR_EXCL_SEM_ALREADY_OWNED = 101
            ERROR_SEM_IS_SET = 102
            ERROR_TOO_MANY_SEM_REQUESTS = 103
            ERROR_INVALID_AT_INTERRUPT_TIME = 104
            ERROR_SEM_OWNER_DIED = 105
            ERROR_SEM_USER_LIMIT = 106
            ERROR_DISK_CHANGE = 107
            ERROR_DRIVE_LOCKED = 108
            ERROR_BROKEN_PIPE = 109
            ERROR_OPEN_FAILED = 110
            ERROR_BUFFER_OVERFLOW = 111
            ERROR_DISK_FULL = 112
            ERROR_NO_MORE_SEARCH_HANDLES = 113
            ERROR_INVALID_TARGET_HANDLE = 114
            ERROR_INVALID_CATEGORY = 117
            ERROR_INVALID_VERIFY_SWITCH = 118
            ERROR_BAD_DRIVER_LEVEL = 119
            ERROR_CALL_NOT_IMPLEMENTED = 120
            ERROR_SEM_TIMEOUT = 121
            ERROR_INSUFFICIENT_BUFFER = 122
            ERROR_INVALID_NAME = 123
            ERROR_INVALID_LEVEL = 124
            ERROR_NO_VOLUME_LABEL = 125
            ERROR_MOD_NOT_FOUND = 126
            ERROR_PROC_NOT_FOUND = 127
            ERROR_WAIT_NO_CHILDREN = 128
            ERROR_CHILD_NOT_COMPLETE = 129
            ERROR_DIRECT_ACCESS_HANDLE = 130
            ERROR_NEGATIVE_SEEK = 131
            ERROR_SEEK_ON_DEVICE = 132
            ERROR_IS_JOIN_TARGET = 133
            ERROR_IS_JOINED = 134
            ERROR_IS_SUBSTED = 135
            ERROR_NOT_JOINED = 136
            ERROR_NOT_SUBSTED = 137
            ERROR_JOIN_TO_JOIN = 138
            ERROR_SUBST_TO_SUBST = 139
            ERROR_JOIN_TO_SUBST = 140
            ERROR_SUBST_TO_JOIN = 141
            ERROR_BUSY_DRIVE = 142
            ERROR_SAME_DRIVE = 143
            ERROR_DIR_NOT_ROOT = 144
            ERROR_DIR_NOT_EMPTY = 145
            ERROR_IS_SUBST_PATH = 146
            ERROR_IS_JOIN_PATH = 147
            ERROR_PATH_BUSY = 148
            ERROR_IS_SUBST_TARGET = 149
            ERROR_SYSTEM_TRACE = 150
            ERROR_INVALID_EVENT_COUNT = 151
            ERROR_TOO_MANY_MUXWAITERS = 152
            ERROR_INVALID_LIST_FORMAT = 153
            ERROR_LABEL_TOO_LONG = 154
            ERROR_TOO_MANY_TCBS = 155
            ERROR_SIGNAL_REFUSED = 156
            ERROR_DISCARDED = 157
            ERROR_NOT_LOCKED = 158
            ERROR_BAD_THREADID_ADDR = 159
            ERROR_BAD_ARGUMENTS = 160
            ERROR_BAD_PATHNAME = 161
            ERROR_SIGNAL_PENDING = 162
            ERROR_MAX_THRDS_REACHED = 164
            ERROR_LOCK_FAILED = 167
            ERROR_BUSY = 170
            ERROR_DEVICE_SUPPORT_IN_PROGRESS = 171
            ERROR_CANCEL_VIOLATION = 173
            ERROR_ATOMIC_LOCKS_NOT_SUPPORTED = 174
            ERROR_INVALID_SEGMENT_NUMBER = 180
            ERROR_INVALID_ORDINAL = 182
            ERROR_ALREADY_EXISTS = 183
            ERROR_INVALID_FLAG_NUMBER = 186
            ERROR_SEM_NOT_FOUND = 187
            ERROR_INVALID_STARTING_CODESEG = 188
            ERROR_INVALID_STACKSEG = 189
            ERROR_INVALID_MODULETYPE = 190
            ERROR_INVALID_EXE_SIGNATURE = 191
            ERROR_EXE_MARKED_INVALID = 192
            ERROR_BAD_EXE_FORMAT = 193
            ERROR_ITERATED_DATA_EXCEEDS_64k = 194
            ERROR_INVALID_MINALLOCSIZE = 195
            ERROR_DYNLINK_FROM_INVALID_RING = 196
            ERROR_IOPL_NOT_ENABLED = 197
            ERROR_INVALID_SEGDPL = 198
            ERROR_AUTODATASEG_EXCEEDS_64k = 199
            ERROR_RING2SEG_MUST_BE_MOVABLE = 200
            ERROR_RELOC_CHAIN_XEEDS_SEGLIM = 201
            ERROR_INFLOOP_IN_RELOC_CHAIN = 202
            ERROR_ENVVAR_NOT_FOUND = 203
            ERROR_NO_SIGNAL_SENT = 205
            ERROR_FILENAME_EXCED_RANGE = 206
            ERROR_RING2_STACK_IN_USE = 207
            ERROR_META_EXPANSION_TOO_LONG = 208
            ERROR_INVALID_SIGNAL_NUMBER = 209
            ERROR_THREAD_1_INACTIVE = 210
            ERROR_LOCKED = 212
            ERROR_TOO_MANY_MODULES = 214
            ERROR_NESTING_NOT_ALLOWED = 215
            ERROR_EXE_MACHINE_TYPE_MISMATCH = 216
            ERROR_EXE_CANNOT_MODIFY_SIGNED_BINARY = 217
            ERROR_EXE_CANNOT_MODIFY_STRONG_SIGNED_BINARY = 218
            ERROR_FILE_CHECKED_OUT = 220
            ERROR_CHECKOUT_REQUIRED = 221
            ERROR_BAD_FILE_TYPE = 222
            ERROR_FILE_TOO_LARGE = 223
            ERROR_FORMS_AUTH_REQUIRED = 224
            ERROR_VIRUS_INFECTED = 225
            ERROR_VIRUS_DELETED = 226
            ERROR_PIPE_LOCAL = 229
            ERROR_BAD_PIPE = 230
            ERROR_PIPE_BUSY = 231
            ERROR_NO_DATA = 232
            ERROR_PIPE_NOT_CONNECTED = 233
            ERROR_MORE_DATA = 234
            ERROR_VC_DISCONNECTED = 240
            ERROR_INVALID_EA_NAME = 254
            ERROR_EA_LIST_INCONSISTENT = 255
            WAIT_TIMEOUT = 258
            ERROR_NO_MORE_ITEMS = 259
            ERROR_CANNOT_COPY = 266
            ERROR_DIRECTORY = 267
            ERROR_EAS_DIDNT_FIT = 275
            ERROR_EA_FILE_CORRUPT = 276
            ERROR_EA_TABLE_FULL = 277
            ERROR_INVALID_EA_HANDLE = 278
            ERROR_EAS_NOT_SUPPORTED = 282
            ERROR_NOT_OWNER = 288
            ERROR_TOO_MANY_POSTS = 298
            ERROR_PARTIAL_COPY = 299
            ERROR_OPLOCK_NOT_GRANTED = 300
            ERROR_INVALID_OPLOCK_PROTOCOL = 301
            ERROR_DISK_TOO_FRAGMENTED = 302
            ERROR_DELETE_PENDING = 303
            ERROR_INCOMPATIBLE_WITH_GLOBAL_SHORT_NAME_REGISTRY_SETTING = 304
            ERROR_SHORT_NAMES_NOT_ENABLED_ON_VOLUME = 305
            ERROR_SECURITY_STREAM_IS_INCONSISTENT = 306
            ERROR_INVALID_LOCK_RANGE = 307
            ERROR_IMAGE_SUBSYSTEM_NOT_PRESENT = 308
            ERROR_NOTIFICATION_GUID_ALREADY_DEFINED = 309
            ERROR_INVALID_EXCEPTION_HANDLER = 310
            ERROR_DUPLICATE_PRIVILEGES = 311
            ERROR_NO_RANGES_PROCESSED = 312
            ERROR_NOT_ALLOWED_ON_SYSTEM_FILE = 313
            ERROR_DISK_RESOURCES_EXHAUSTED = 314
            ERROR_INVALID_TOKEN = 315
            ERROR_DEVICE_FEATURE_NOT_SUPPORTED = 316
            ERROR_MR_MID_NOT_FOUND = 317
            ERROR_SCOPE_NOT_FOUND = 318
            ERROR_UNDEFINED_SCOPE = 319
            ERROR_INVALID_CAP = 320
            ERROR_DEVICE_UNREACHABLE = 321
            ERROR_DEVICE_NO_RESOURCES = 322
            ERROR_DATA_CHECKSUM_ERROR = 323
            ERROR_INTERMIXED_KERNEL_EA_OPERATION = 324
            ERROR_FILE_LEVEL_TRIM_NOT_SUPPORTED = 326
            ERROR_OFFSET_ALIGNMENT_VIOLATION = 327
            ERROR_INVALID_FIELD_IN_PARAMETER_LIST = 328
            ERROR_OPERATION_IN_PROGRESS = 329
            ERROR_BAD_DEVICE_PATH = 330
            ERROR_TOO_MANY_DESCRIPTORS = 331
            ERROR_SCRUB_DATA_DISABLED = 332
            ERROR_NOT_REDUNDANT_STORAGE = 333
            ERROR_RESIDENT_FILE_NOT_SUPPORTED = 334
            ERROR_COMPRESSED_FILE_NOT_SUPPORTED = 335
            ERROR_DIRECTORY_NOT_SUPPORTED = 336
            ERROR_NOT_READ_FROM_COPY = 337
            ERROR_FAIL_NOACTION_REBOOT = 350
            ERROR_FAIL_SHUTDOWN = 351
            ERROR_FAIL_RESTART = 352
            ERROR_MAX_SESSIONS_REACHED = 353
            ERROR_THREAD_MODE_ALREADY_BACKGROUND = 400
            ERROR_THREAD_MODE_NOT_BACKGROUND = 401
            ERROR_PROCESS_MODE_ALREADY_BACKGROUND = 402
            ERROR_PROCESS_MODE_NOT_BACKGROUND = 403
            ERROR_INVALID_ADDRESS = 487
        End Enum

        <DllImport("kernel32.dll")>
        Public Function QueryFullProcessImageName(hprocess As IntPtr, dwFlags As Integer, lpExeName As Text.StringBuilder, ByRef size As Integer) As Boolean
        End Function

        <DllImport("kernel32.dll")>
        Public Function OpenProcess(dwDesiredAccess As ProcessAccessFlags, bInheritHandle As Boolean, dwProcessId As Integer) As IntPtr
        End Function

        <DllImport("kernel32.dll", SetLastError:=True)>
        Public Function CloseHandle(hHandle As IntPtr) As Boolean
        End Function

        <DllImport("kernel32.dll")>
        Public Function MoveFileEx(ByVal lpExistingFileName As String, ByVal lpNewFileName As String, ByVal dwFlags As Int32) As Boolean
        End Function
    End Module
End Namespace