Imports System.Management

Namespace Functions.BCD
    Public Class bcdEditor
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
            connectionOptions = New ConnectionOptions() With {
              .Impersonation = ImpersonationLevel.Impersonate,
              .EnablePrivileges = True
            }

            managementScope = New ManagementScope("root\WMI", connectionOptions)

            managementPath = New ManagementPath("root\WMI:BcdObject.Id=""{fa926493-6f1c-4193-a414-58f0b2456d1e}"",StoreFilePath=""""")
        End Sub

        ''' <summary>Adds the Safe Mode Boot flag.</summary>
        Public Sub setSafeModeBootFlag()
            Dim currentBootloader As New ManagementObject(managementScope, managementPath, Nothing)

            If Not getSafeModeBootStatus(currentBootloader) Then
                currentBootloader.InvokeMethod("SetIntegerElement", New Object() {BcdOSLoaderInteger_SafeBoot, BcdLibrary_SafeBoot.SafemodeMinimal})
            End If

            currentBootloader.Dispose()
        End Sub

        ''' <summary>Removes the Safe Mode Boot flag.</summary>
        Public Sub removeSafeModeBootFlag()
            Dim currentBootloader As New ManagementObject(managementScope, managementPath, Nothing)

            If getSafeModeBootStatus(currentBootloader) Then
                currentBootloader.InvokeMethod("DeleteElement", New Object() {BcdOSLoaderInteger_SafeBoot})
            End If

            currentBootloader.Dispose()
        End Sub

        ''' <summary>This function checks the status of the Safe Mode Boot flag and if it exists it returns a True value, if not False.</summary>
        Private Function getSafeModeBootStatus(ByRef currentBootloader As ManagementObject) As Boolean
            Try
                Return currentBootloader.InvokeMethod("GetElement", (New Object() {BcdOSLoaderInteger_SafeBoot}))
            Catch ex As Exception
                Return False
            End Try
        End Function
    End Class
End Namespace