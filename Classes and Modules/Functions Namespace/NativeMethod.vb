Imports System.Runtime.InteropServices

Namespace Functions.NativeMethod
    Friend Class NativeMethod
        Public Const SWP_NOMOVE = 2
        Public Const SWP_NOSIZE = 1
        Public Const HWND_TOPMOST = -1
        Public Const HWND_NOTOPMOST = -2
        Public Const TOPMOST_FLAGS = SWP_NOMOVE Or SWP_NOSIZE

        Public Enum PROCESS_INFORMATION_CLASS As Integer
            ProcessBasicInformation = 0
            ProcessQuotaLimits
            ProcessIoCounters
            ProcessVmCounters
            ProcessTimes
            ProcessBasePriority
            ProcessRaisePriority
            ProcessDebugPort
            ProcessExceptionPort
            ProcessAccessToken
            ProcessLdtInformation
            ProcessLdtSize
            ProcessDefaultHardErrorMode
            ProcessIoPortHandlers
            ProcessPooledUsageAndLimits
            ProcessWorkingSetWatch
            ProcessUserModeIOPL
            ProcessEnableAlignmentFaultFixup
            ProcessPriorityClass
            ProcessWx86Information
            ProcessHandleCount
            ProcessAffinityMask
            ProcessPriorityBoost
            ProcessDeviceMap
            ProcessSessionInformation
            ProcessForegroundInformation
            ProcessWow64Information
            ProcessImageFileName
            ProcessLUIDDeviceMapsEnabled
            ProcessBreakOnTermination
            ProcessDebugObjectHandle
            ProcessDebugFlags
            ProcessHandleTracing
            ProcessIoPriority
            ProcessExecuteFlags
            ProcessResourceManagement
            ProcessCookie
            ProcessImageInformation
            ProcessCycleTime
            ProcessPagePriority
            ProcessInstrumentationCallback
            ProcessThreadStackAllocation
            ProcessWorkingSetWatchEx
            ProcessImageFileNameWin32
            ProcessImageFileMapping
            ProcessAffinityUpdateMode
            ProcessMemoryAllocationMode
            MaxProcessInfoClass
        End Enum

        <Flags>
        Public Enum SetWindowPosFlags As UInteger
            ' ReSharper disable InconsistentNaming

            ''' <summary>
            '''     If the calling thread and the thread that owns the window are attached to different input queues, the system posts the request to the thread that owns the window. This prevents the calling thread from blocking its execution while other threads process the request.
            ''' </summary>
            SWP_ASYNCWINDOWPOS = &H4000

            ''' <summary>
            '''     Prevents generation of the WM_SYNCPAINT message.
            ''' </summary>
            SWP_DEFERERASE = &H2000

            ''' <summary>
            '''     Draws a frame (defined in the window's class description) around the window.
            ''' </summary>
            SWP_DRAWFRAME = &H20

            ''' <summary>
            '''     Applies new frame styles set using the SetWindowLong function. Sends a WM_NCCALCSIZE message to the window, even if the window's size is not being changed. If this flag is not specified, WM_NCCALCSIZE is sent only when the window's size is being changed.
            ''' </summary>
            SWP_FRAMECHANGED = &H20

            ''' <summary>
            '''     Hides the window.
            ''' </summary>
            SWP_HIDEWINDOW = &H80

            ''' <summary>
            '''     Does not activate the window. If this flag is not set, the window is activated and moved to the top of either the topmost or non-topmost group (depending on the setting of the hWndInsertAfter parameter).
            ''' </summary>
            SWP_NOACTIVATE = &H10

            ''' <summary>
            '''     Discards the entire contents of the client area. If this flag is not specified, the valid contents of the client area are saved and copied back into the client area after the window is sized or repositioned.
            ''' </summary>
            SWP_NOCOPYBITS = &H100

            ''' <summary>
            '''     Retains the current position (ignores X and Y parameters).
            ''' </summary>
            SWP_NOMOVE = &H2

            ''' <summary>
            '''     Does not change the owner window's position in the Z order.
            ''' </summary>
            SWP_NOOWNERZORDER = &H200

            ''' <summary>
            '''     Does not redraw changes. If this flag is set, no repainting of any kind occurs. This applies to the client area, the nonclient area (including the title bar and scroll bars), and any part of the parent window uncovered as a result of the window being moved. When this flag is set, the application must explicitly invalidate or redraw any parts of the window and parent window that need redrawing.
            ''' </summary>
            SWP_NOREDRAW = &H8

            ''' <summary>
            '''     Same as the SWP_NOOWNERZORDER flag.
            ''' </summary>
            SWP_NOREPOSITION = &H200

            ''' <summary>
            '''     Prevents the window from receiving the WM_WINDOWPOSCHANGING message.
            ''' </summary>
            SWP_NOSENDCHANGING = &H400

            ''' <summary>
            '''     Retains the current size (ignores the cx and cy parameters).
            ''' </summary>
            SWP_NOSIZE = &H1

            ''' <summary>
            '''     Retains the current Z order (ignores the hWndInsertAfter parameter).
            ''' </summary>
            SWP_NOZORDER = &H4

            ''' <summary>
            '''     Displays the window.
            ''' </summary>
            SWP_SHOWWINDOW = &H40

            ' ReSharper restore InconsistentNaming
        End Enum

        <DllImport("user32.dll", SetLastError:=True)>
        Public Shared Function SetWindowPos(ByVal hWnd As IntPtr, ByVal hWndInsertAfter As IntPtr, ByVal X As Integer, ByVal Y As Integer, ByVal cx As Integer, ByVal cy As Integer, ByVal uFlags As SetWindowPosFlags) As Boolean
        End Function

        <DllImport("dwmapi.dll")>
        Public Shared Sub DwmGetColorizationColor(ByRef ColorizationColor As UInteger, ByRef ColorizationOpaqueBlend As Boolean)
        End Sub

        <DllImport("kernel32.dll", SetLastError:=True)>
        Public Shared Function GetCurrentProcess() As IntPtr
        End Function

        <DllImport("ntdll.dll", SetLastError:=True)>
        Public Shared Function NtSetInformationProcess(processHandle As IntPtr, processInformationClass As PROCESS_INFORMATION_CLASS, ByRef processInformation As IntPtr, processInformationLength As UInteger) As Integer
        End Function
    End Class
End Namespace