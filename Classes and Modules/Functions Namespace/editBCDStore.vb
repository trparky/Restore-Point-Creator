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
    End Class
End Namespace