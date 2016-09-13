'Imports System.Runtime.InteropServices
'Imports System.ComponentModel
'Imports System.ServiceProcess

'Public NotInheritable Class ServiceHelper
'    Private Sub New()
'    End Sub

'    <DllImport("advapi32.dll", CharSet:=CharSet.Unicode, SetLastError:=True)> _
'    Public Shared Function ChangeServiceConfig(hService As IntPtr, nServiceType As UInt32, nStartType As UInt32, nErrorControl As UInt32, lpBinaryPathName As [String], lpLoadOrderGroup As [String], _
'        lpdwTagId As IntPtr, <[In]> lpDependencies As Char(), lpServiceStartName As [String], lpPassword As [String], lpDisplayName As [String]) As [Boolean]
'    End Function

'    <DllImport("advapi32.dll", SetLastError:=True, CharSet:=CharSet.Auto)> _
'    Private Shared Function OpenService(hSCManager As IntPtr, lpServiceName As String, dwDesiredAccess As UInteger) As IntPtr
'    End Function

'    Public Declare Unicode Function OpenSCManager Lib "advapi32.dll" Alias "OpenSCManagerW" (machineName As String, databaseName As String, dwAccess As UInteger) As IntPtr

'    <DllImport("advapi32.dll", EntryPoint:="CloseServiceHandle")> _
'    Public Shared Function CloseServiceHandle(hSCObject As IntPtr) As Integer
'    End Function

'    Private Const SERVICE_NO_CHANGE As UInteger = &HFFFFFFFFUI
'    Private Const SERVICE_QUERY_CONFIG As UInteger = &H1
'    Private Const SERVICE_CHANGE_CONFIG As UInteger = &H2
'    Private Const SC_MANAGER_ALL_ACCESS As UInteger = &HF003F

'    Public Shared Sub changeStartMode(svc As ServiceController, mode As ServiceStartMode)
'        Dim scManagerHandle = OpenSCManager(Nothing, Nothing, SC_MANAGER_ALL_ACCESS)
'        If scManagerHandle = IntPtr.Zero Then
'            Throw New ExternalException("Open Service Manager Error")
'        End If

'        Dim serviceHandle = OpenService(scManagerHandle, svc.ServiceName, SERVICE_QUERY_CONFIG Or SERVICE_CHANGE_CONFIG)

'        If serviceHandle = IntPtr.Zero Then
'            Throw New ExternalException("Open Service Error")
'        End If

'        Dim result = ChangeServiceConfig(serviceHandle, SERVICE_NO_CHANGE, CUInt(mode), SERVICE_NO_CHANGE, Nothing, Nothing, _
'            IntPtr.Zero, Nothing, Nothing, Nothing, Nothing)

'        If result = False Then
'            Dim nError As Integer = Marshal.GetLastWin32Error()
'            Dim win32Exception = New Win32Exception(nError)
'            Throw New ExternalException("Could not change service start type: " & Convert.ToString(win32Exception.Message))
'        End If

'        CloseServiceHandle(serviceHandle)
'        CloseServiceHandle(scManagerHandle)
'    End Sub
'End Class