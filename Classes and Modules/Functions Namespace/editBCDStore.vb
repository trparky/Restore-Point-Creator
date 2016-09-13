Imports System.Management

Namespace Functions.editBCDStore
    Public Class editBCDStore
        Public Const BcdOSLoaderInteger_SafeBoot As Integer = &H25000080

        Public Enum BcdLibrary_SafeBoot
            SafemodeMinimal = 0
            SafemodeNetwork = 1
            SafemodeDsRepair = 2
        End Enum

        Private connectionOptions As ConnectionOptions
        Private managementScope As ManagementScope
        Private managementPath As ManagementPath

        Public Sub New()
            connectionOptions = New ConnectionOptions()
            connectionOptions.Impersonation = ImpersonationLevel.Impersonate
            connectionOptions.EnablePrivileges = True

            managementScope = New ManagementScope("root\WMI", connectionOptions)

            managementPath = New ManagementPath("root\WMI:BcdObject.Id=""{fa926493-6f1c-4193-a414-58f0b2456d1e}"",StoreFilePath=""""")
        End Sub

        Public Sub SetSafeboot()
            Dim currentBootloader As New ManagementObject(managementScope, managementPath, Nothing)
            currentBootloader.InvokeMethod("SetIntegerElement", New Object() {BcdOSLoaderInteger_SafeBoot, BcdLibrary_SafeBoot.SafemodeMinimal})
        End Sub

        Public Sub RemoveSafeboot()
            Dim currentBootloader As New ManagementObject(managementScope, managementPath, Nothing)
            currentBootloader.InvokeMethod("DeleteElement", New Object() {BcdOSLoaderInteger_SafeBoot})
        End Sub

        'Function isSafeModeBootEnabled() As Boolean
        '    Dim bcdEditOutput As String = Nothing
        '    If Functions.executeShellCommandAndGetOutput(bcdEditOutput, "bcdedit") = True Then
        '        If bcdEditOutput.caseInsensitiveContains("safeboot", False) = True Then
        '            Return True
        '        Else
        '            Return False
        '        End If
        '    Else
        '        Return True
        '    End If
        'End Function

        'Public Sub RemoveSafeboot()
        '    Dim removeSafeModeBootThread As New Threading.Thread(Sub()
        '                                                             Try
        '                                                                 If isSafeModeBootEnabled() = True Then
        '                                                                     Dim currentBootloader As New ManagementObject(managementScope, managementPath, Nothing)
        '                                                                     currentBootloader.InvokeMethod("DeleteElement", New Object() {BcdOSLoaderInteger_SafeBoot})
        '                                                                 End If
        '                                                             Catch ex As Runtime.InteropServices.COMException
        '                                                                 Debug.WriteLine("A COMException occurred here but we don't care")
        '                                                             End Try
        '                                                         End Sub)
        '    removeSafeModeBootThread.Name = "Remove Safe Mode Boot Setting Thread"
        '    removeSafeModeBootThread.Start()
        'End Sub
    End Class
End Namespace