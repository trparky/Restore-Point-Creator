Option Strict On
Imports System.Management

Namespace Functions.BCD
    Public Class bcdEditor
        Public Const BcdOSLoaderInteger_SafeBoot As Integer = &H25000080

        Public Enum BcdLibrary_SafeBoot As Short
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
        Public Sub setSafeModeBootFlag(Optional boolBypassCheck As Boolean = False)
            Dim currentBootloader As New ManagementObject(managementScope, managementPath, Nothing)

            If boolBypassCheck Then
                currentBootloader.InvokeMethod("SetIntegerElement", New Object() {BcdOSLoaderInteger_SafeBoot, BcdLibrary_SafeBoot.SafemodeMinimal})
            Else
                Dim safeModeBootType As BcdLibrary_SafeBoot
                If Not getSafeModeBootStatus(currentBootloader, safeModeBootType) Then
                    currentBootloader.InvokeMethod("SetIntegerElement", New Object() {BcdOSLoaderInteger_SafeBoot, BcdLibrary_SafeBoot.SafemodeMinimal})
                End If
            End If

            currentBootloader.Dispose()
        End Sub

        ''' <summary>Removes the Safe Mode Boot flag.</summary>
        Public Sub removeSafeModeBootFlag()
            Dim currentBootloader As New ManagementObject(managementScope, managementPath, Nothing)

            Dim safeModeBootType As BcdLibrary_SafeBoot
            If getSafeModeBootStatus(currentBootloader, safeModeBootType) Then
                currentBootloader.InvokeMethod("DeleteElement", New Object() {BcdOSLoaderInteger_SafeBoot})
            End If

            currentBootloader.Dispose()
        End Sub

        ''' <summary>This function checks the status of the Safe Mode Boot flag and if it exists it returns a True value, if not False.</summary>
        Private Function getSafeModeBootStatus(ByRef currentBootloader As ManagementObject, ByRef safeModeBootType As BcdLibrary_SafeBoot) As Boolean
            Try
                ' This line queries the boot flags that have been set for the current boot loader.
                Dim bcdBootSettings As ManagementBaseObject = currentBootloader.InvokeMethod("EnumerateElementTypes", Nothing, Nothing)

                ' This line gets the Property Values from the ManagementBaseObject, converts it into an Unsigned Integer Array, and
                ' then checks for the existance of the BcdOSLoaderInteger_SafeBoot Integer and stores the result in a Boolean value.
                Dim boolResult As Boolean = DirectCast(bcdBootSettings.Properties("Types").Value, UInteger()).Contains(BcdOSLoaderInteger_SafeBoot)

                ' OK, we then check the result of the Boolean value that was created above.
                If boolResult Then
                    ' This routine only runs if the Safe Mode Boot Flag exists for the current boot loader.

                    ' First we need to create a set of parameters that we're going to pass to the "GetElement" API.
                    Dim getElementParameters As ManagementBaseObject = currentBootloader.GetMethodParameters("GetElement")
                    getElementParameters("Type") = BcdOSLoaderInteger_SafeBoot ' This sets the setting we want to query, namely the Safe Mode Boot Flag.

                    ' This calls the "GetElement" API and stores the result as a ManagementBaseObject.
                    Dim getElementInvokationResults As ManagementBaseObject = currentBootloader.InvokeMethod("GetElement", getElementParameters, Nothing)

                    ' The returned value is an Object but we know that it's really a Short (or 16-bit Integer) so we convert it to a Short then
                    ' DirectCast it to a BcdLibrary_SafeBoot Type to get the Safe Mode Boot Flag value (Minimal or with Networking).
                    safeModeBootType = DirectCast(Convert.ToInt16(getElementInvokationResults.Properties("ReturnValue").Value), BcdLibrary_SafeBoot)
                End If

                ' We then finally return the value of the Boolean value that was created above.
                Return boolResult
            Catch ex As Exception
                Return False ' If anything goes wrong we then return a False value.
            End Try
        End Function
    End Class
End Namespace