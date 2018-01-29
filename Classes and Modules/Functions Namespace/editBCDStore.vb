Option Strict On
Imports System.Management

Namespace Functions.BCD
    Public Class bcdEditorStatusException
        Inherits Exception

        Public Sub New()
        End Sub

        Public Sub New(message As String)
            MyBase.New(message)
        End Sub

        Public Sub New(message As String, inner As Exception)
            MyBase.New(message, inner)
        End Sub
    End Class

    Public Class bcdEditorSetException
        Inherits Exception

        Public Sub New()
        End Sub

        Public Sub New(message As String)
            MyBase.New(message)
        End Sub

        Public Sub New(message As String, inner As Exception)
            MyBase.New(message, inner)
        End Sub
    End Class

    Public Class bcdEditorConnectionException
        Inherits Exception

        Public Sub New()
        End Sub

        Public Sub New(message As String)
            MyBase.New(message)
        End Sub

        Public Sub New(message As String, inner As Exception)
            MyBase.New(message, inner)
        End Sub
    End Class

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
        Private currentBootloader As ManagementObject

        Public Sub dispose()
            currentBootloader.Dispose()
            currentBootloader = Nothing
            managementPath = Nothing
            managementScope = Nothing
            connectionOptions = Nothing
        End Sub

        ''' <summary>Connects to the Windows BCDStore.</summary>
        ''' <exception cref="bcdEditorConnectionException">This exception is thrown if the program can't connect to the Windows BCDStore for whatever reason.</exception>
        Public Sub New()
            Try
                connectionOptions = New ConnectionOptions() With {
                  .Impersonation = ImpersonationLevel.Impersonate,
                  .EnablePrivileges = True
                }

                managementScope = New ManagementScope("root\WMI", connectionOptions)
                managementPath = New ManagementPath("root\WMI:BcdObject.Id=""{fa926493-6f1c-4193-a414-58f0b2456d1e}"",StoreFilePath=""""")
                currentBootloader = New ManagementObject(managementScope, managementPath, Nothing)
            Catch ex As Exception
                Throw New bcdEditorConnectionException("Unable to connect to BCDStore.", ex)
            End Try
        End Sub

        ''' <summary>Adds the Safe Mode Boot flag.</summary>
        ''' <param name="boolBypassCheck">This tells the function if it should bypass a check to see if the Safe Mode Boot flag has already been set. It is recommended not to set this flag to True and to keep it as the default value of False.</param>
        ''' <exception cref="bcdEditorSetException">This exception is thrown if the program can't set the Safe Mode Boot Flag.</exception>
        Public Sub setSafeModeBootFlag(Optional boolBypassCheck As Boolean = False)
            If boolBypassCheck Then
                Try
                    currentBootloader.InvokeMethod("SetIntegerElement", New Object() {BcdOSLoaderInteger_SafeBoot, BcdLibrary_SafeBoot.SafemodeMinimal})
                Catch ex As Exception
                    Throw New bcdEditorSetException("Unable to set Safe Mode Boot Flag.", ex)
                End Try
            Else
                Dim safeModeBootType As BcdLibrary_SafeBoot
                If Not getSafeModeBootStatus(safeModeBootType) Then
                    Try
                        currentBootloader.InvokeMethod("SetIntegerElement", New Object() {BcdOSLoaderInteger_SafeBoot, BcdLibrary_SafeBoot.SafemodeMinimal})
                    Catch ex As Exception
                        Throw New bcdEditorSetException("Unable to set Safe Mode Boot Flag.", ex)
                    End Try
                End If
            End If
        End Sub

        ''' <summary>Removes the Safe Mode Boot flag.</summary>
        ''' <param name="boolBypassCheck">This tells the function if it should bypass a check to see if the Safe Mode Boot flag has already been set. It is recommended not to set this flag to True and to keep it as the default value of False.</param>
        ''' <exception cref="bcdEditorSetException">This exception is thrown if the program can't remove the Safe Mode Boot Flag.</exception>
        Public Sub removeSafeModeBootFlag(Optional boolBypassCheck As Boolean = False)
            If boolBypassCheck Then
                Try
                    currentBootloader.InvokeMethod("DeleteElement", New Object() {BcdOSLoaderInteger_SafeBoot})
                Catch ex As Exception
                    Throw New bcdEditorSetException("Unable to remove Safe Mode Boot Flag.", ex)
                End Try
            Else
                Dim safeModeBootType As BcdLibrary_SafeBoot
                If getSafeModeBootStatus(safeModeBootType) Then
                    Try
                        currentBootloader.InvokeMethod("DeleteElement", New Object() {BcdOSLoaderInteger_SafeBoot})
                    Catch ex As Exception
                        Throw New bcdEditorSetException("Unable to remove Safe Mode Boot Flag.", ex)
                    End Try
                End If
            End If
        End Sub

        Public Function getSafeModeBootStatus() As Boolean
            Dim safeModeBootType As BcdLibrary_SafeBoot
            Return getSafeModeBootStatus(safeModeBootType)
        End Function

        ''' <summary>This function checks the status of the Safe Mode Boot flag and if it exists it returns a True value, if not False.</summary>
        ''' <exception cref="bcdEditorStatusException">This exception is thrown if this function can't retrieve the status of the Safe Mode Boot Flag.</exception>
        Private Function getSafeModeBootStatus(ByRef safeModeBootType As BcdLibrary_SafeBoot) As Boolean
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
                    Dim shortReturnValue As Short
                    If Short.TryParse(getElementInvokationResults.Properties("ReturnValue").Value.ToString, shortReturnValue) Then
                        safeModeBootType = DirectCast(shortReturnValue, BcdLibrary_SafeBoot)
                    Else
                        boolResult = False
                    End If
                End If

                ' We then finally return the value of the Boolean value that was created above.
                Return boolResult
            Catch ex As Exception
                Throw New bcdEditorStatusException("Unable to retrieve Safe Mode Boot Flag status.", ex)
                Return False ' If anything goes wrong we then return a False value.
            End Try
        End Function
    End Class
End Namespace