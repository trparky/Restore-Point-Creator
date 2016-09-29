Imports System.IO

Namespace Functions.miniDump
    Friend Class MiniDump
        'Code converted from C# code found here: http://social.msdn.microsoft.com/Forums/en-US/clr/thread/6c8d3529-a493-49b9-93d7-07a3a2d715dc

        Private Enum MINIDUMP_TYPE
            MiniDumpNormal = 0
            MiniDumpWithDataSegs = 1
            MiniDumpWithFullMemory = 2
            MiniDumpWithHandleData = 4
            MiniDumpFilterMemory = 8
            MiniDumpScanMemory = 10
            MiniDumpWithUnloadedModules = 20
            MiniDumpWithIndirectlyReferencedMemory = 40
            MiniDumpFilterModulePaths = 80
            MiniDumpWithProcessThreadData = 100
            MiniDumpWithPrivateReadWriteMemory = 200
            MiniDumpWithoutOptionalData = 400
            MiniDumpWithFullMemoryInfo = 800
            MiniDumpWithThreadInfo = 1000
            MiniDumpWithCodeSegs = 2000
        End Enum

        <Runtime.InteropServices.DllImport("dbghelp.dll")>
        Private Shared Function MiniDumpWriteDump(
             ByVal hProcess As IntPtr,
             ByVal ProcessId As Int32,
            ByVal hFile As IntPtr,
             ByVal DumpType As MINIDUMP_TYPE,
            ByVal ExceptionParam As IntPtr,
             ByVal UserStreamParam As IntPtr,
            ByVal CallackParam As IntPtr) As Boolean
        End Function

        Friend Shared Sub MiniDumpToFile(ByVal fileToDump As String)
            Dim fsToDump As FileStream = Nothing

            If (File.Exists(fileToDump)) Then
                fsToDump = File.Open(fileToDump, FileMode.Append)
            Else
                fsToDump = File.Create(fileToDump)
            End If

            Dim thisProcess As Process = Process.GetCurrentProcess()
            MiniDumpWriteDump(thisProcess.Handle,
                              thisProcess.Id,
                              fsToDump.SafeFileHandle.DangerousGetHandle(),
                              MINIDUMP_TYPE.MiniDumpNormal,
                              IntPtr.Zero,
                              IntPtr.Zero,
                              IntPtr.Zero)
            fsToDump.Close()
        End Sub
    End Class
End Namespace