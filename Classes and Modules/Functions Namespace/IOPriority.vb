Imports System.Runtime.InteropServices

Namespace Functions
    Class IOPriority
        <DllImport("kernel32.dll", SetLastError:=True)>
        Private Shared Function GetCurrentProcess() As IntPtr
        End Function

        <DllImport("ntdll.dll", SetLastError:=True)>
        Public Shared Function NtSetInformationProcess(processHandle As IntPtr, processInformationClass As PROCESS_INFORMATION_CLASS, ByRef processInformation As IntPtr, processInformationLength As UInteger) As Integer
        End Function

        Enum IOPrioritySetting As UInteger
            VeryLow = 0
            Low = 1
            Normal = 2
        End Enum

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

        Public Shared Function SetIOPriority(newPrio As IOPrioritySetting) As Integer
            Dim ioPrio As IntPtr = New IntPtr(newPrio)
            Dim lret As Integer = NtSetInformationProcess(GetCurrentProcess(), PROCESS_INFORMATION_CLASS.ProcessIoPriority, ioPrio, 4)
            Return lret
        End Function
    End Class
End Namespace