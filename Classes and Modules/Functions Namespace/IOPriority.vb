Namespace Functions
    Class IOPriority
        Enum IOPrioritySetting As UInteger
            VeryLow = 0
            Low = 1
            Normal = 2
        End Enum

        Public Shared Function SetIOPriority(newPrio As IOPrioritySetting) As Integer
            Dim ioPrio As IntPtr = New IntPtr(newPrio)
            Dim lret As Integer = NativeMethod.NativeMethods.NtSetInformationProcess(NativeMethod.NativeMethods.GetCurrentProcess(), NativeMethod.NativeMethods.PROCESS_INFORMATION_CLASS.ProcessIoPriority, ioPrio, 4)
            Return lret
        End Function
    End Class
End Namespace