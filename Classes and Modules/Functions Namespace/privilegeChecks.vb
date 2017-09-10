Imports System.Security.AccessControl
Imports System.Security.Principal

Namespace Functions.privilegeChecks
    Module privilegeChecks
        Public Function areWeRunningAsSystemUser() As Boolean
            Return WindowsIdentity.GetCurrent().IsSystem
        End Function

        Public Function canIWriteToTheCurrentDirectory() As Boolean
            Return canIWriteThere(New IO.FileInfo(Application.ExecutablePath).DirectoryName)
        End Function

        Public Function canIWriteThere(folderPath As String) As Boolean
            ' We make sure we get valid folder path by taking off the leading slash.
            If folderPath.EndsWith("\") Then folderPath = folderPath.Substring(0, folderPath.Length - 1)
            If String.IsNullOrEmpty(folderPath) Or Not IO.Directory.Exists(folderPath) Then Return False

            If checkByFolderACLs(folderPath) Then
                Try
                    Dim strOurTestFileName As String = IO.Path.Combine(folderPath, "test" & support.randomStringGenerator(10) & ".txt")

                    IO.File.Create(strOurTestFileName, 1, IO.FileOptions.DeleteOnClose).Close()
                    If IO.File.Exists(strOurTestFileName) Then IO.File.Delete(strOurTestFileName)

                    Return True
                Catch ex As Exception
                    Return False
                End Try
            Else
                Return False
            End If
        End Function

        Private Function checkByFolderACLs(folderPath As String) As Boolean
            Try
                Dim directoryACLs As DirectorySecurity = IO.Directory.GetAccessControl(folderPath)
                Dim directoryUsers As String = WindowsIdentity.GetCurrent.User.Value
                Dim userGroups As IdentityReferenceCollection = WindowsIdentity.GetCurrent.Groups
                Dim authorizationRules As AuthorizationRuleCollection = directoryACLs.GetAccessRules(True, True, GetType(SecurityIdentifier))
                Dim directoryAccessRights As FileSystemAccessRule
                Dim fileSystemRights As FileSystemRights

                For Each rule As AuthorizationRule In authorizationRules
                    If rule.IdentityReference.Value.Equals(directoryUsers, StringComparison.OrdinalIgnoreCase) Or userGroups.Contains(rule.IdentityReference) Then
                        directoryAccessRights = DirectCast(rule, FileSystemAccessRule)

                        If directoryAccessRights.AccessControlType = Security.AccessControl.AccessControlType.Allow Then
                            fileSystemRights = directoryAccessRights.FileSystemRights

                            If fileSystemRights = (FileSystemRights.Read Or FileSystemRights.Modify Or FileSystemRights.Write Or FileSystemRights.FullControl) Then
                                Return True
                            End If
                        End If
                    End If
                Next

                Return False
            Catch ex As Exception
                Return False
            End Try
        End Function

        Public Function areWeAnAdministrator() As Boolean
            Try
                Dim principal As Security.Principal.WindowsPrincipal = New Security.Principal.WindowsPrincipal(Security.Principal.WindowsIdentity.GetCurrent())

                If principal.IsInRole(Security.Principal.WindowsBuiltInRole.Administrator) = True Then
                    Return True
                Else
                    Return False
                End If
            Catch ex As Exception
                Return False
            End Try
        End Function

        Public Function IsUserInAdminGroup() As Boolean
            Dim fInAdminGroup As Boolean = False
            Dim hToken As NativeMethod.SafeTokenHandle = Nothing
            Dim hTokenToCheck As NativeMethod.SafeTokenHandle = Nothing
            Dim pElevationType As IntPtr = IntPtr.Zero
            Dim pLinkedToken As IntPtr = IntPtr.Zero
            Dim cbSize As Integer = 0

            Try
                ' Open the access token of the current process for query and duplicate.
                If (Not NativeMethod.NativeMethod.OpenProcessToken(Process.GetCurrentProcess.Handle, NativeMethod.NativeMethod.TOKEN_QUERY Or NativeMethod.NativeMethod.TOKEN_DUPLICATE, hToken)) Then
                    Throw New ComponentModel.Win32Exception(Runtime.InteropServices.Marshal.GetLastWin32Error)
                End If

                ' Determine whether system is running Windows Vista or later operating
                ' systems (major version >= 6) because they support linked tokens, but
                ' previous versions (major version < 6) do not.
                If (Environment.OSVersion.Version.Major >= 6) Then
                    ' Running Windows Vista or later (major version >= 6).
                    ' Determine token type: limited, elevated, or default.

                    ' Allocate a buffer for the elevation type information.
                    cbSize = 4  ' Size of TOKEN_ELEVATION_TYPE
                    pElevationType = Runtime.InteropServices.Marshal.AllocHGlobal(cbSize)
                    If (pElevationType = IntPtr.Zero) Then
                        Throw New ComponentModel.Win32Exception(Runtime.InteropServices.Marshal.GetLastWin32Error)
                    End If

                    ' Retrieve token elevation type information.
                    If (Not NativeMethod.NativeMethod.GetTokenInformation(hToken, NativeMethod.TOKEN_INFORMATION_CLASS.TokenElevationType, pElevationType, cbSize, cbSize)) Then
                        Throw New ComponentModel.Win32Exception(Runtime.InteropServices.Marshal.GetLastWin32Error)
                    End If

                    ' Marshal the TOKEN_ELEVATION_TYPE enum from native to .NET.
                    Dim elevType As NativeMethod.TOKEN_ELEVATION_TYPE = Runtime.InteropServices.Marshal.ReadInt32(pElevationType)

                    ' If limited, get the linked elevated token for further check.
                    If (elevType = Functions.NativeMethod.TOKEN_ELEVATION_TYPE.TokenElevationTypeLimited) Then
                        ' Allocate a buffer for the linked token.
                        cbSize = IntPtr.Size
                        pLinkedToken = Runtime.InteropServices.Marshal.AllocHGlobal(cbSize)
                        If (pLinkedToken = IntPtr.Zero) Then
                            Throw New ComponentModel.Win32Exception(Runtime.InteropServices.Marshal.GetLastWin32Error)
                        End If

                        ' Get the linked token.
                        If (Not NativeMethod.NativeMethod.GetTokenInformation(hToken, NativeMethod.TOKEN_INFORMATION_CLASS.TokenLinkedToken, pLinkedToken, cbSize, cbSize)) Then
                            Throw New ComponentModel.Win32Exception(Runtime.InteropServices.Marshal.GetLastWin32Error)
                        End If

                        ' Marshal the linked token value from native to .NET.
                        Dim hLinkedToken As IntPtr = Runtime.InteropServices.Marshal.ReadIntPtr(pLinkedToken)
                        hTokenToCheck = New NativeMethod.SafeTokenHandle(hLinkedToken)
                    End If
                End If

                ' CheckTokenMembership requires an impersonation token. If we just got a
                ' linked token, it already is an impersonation token.  If we did not get
                ' a linked token, duplicate the original into an impersonation token for
                ' CheckTokenMembership.
                If (hTokenToCheck Is Nothing) Then
                    If (Not NativeMethod.NativeMethod.DuplicateToken(hToken, NativeMethod.SECURITY_IMPERSONATION_LEVEL.SecurityIdentification, hTokenToCheck)) Then
                        Throw New ComponentModel.Win32Exception(Runtime.InteropServices.Marshal.GetLastWin32Error)
                    End If
                End If

                ' Check if the token to be checked contains admin SID.
                Dim id As New Security.Principal.WindowsIdentity(hTokenToCheck.DangerousGetHandle)
                Dim principal As New Security.Principal.WindowsPrincipal(id)
                fInAdminGroup = principal.IsInRole(Security.Principal.WindowsBuiltInRole.Administrator)
            Catch ex As Exception
                ' Something went wrong here so we are going to assume that the user isn't part of the Administrator user group.
                fInAdminGroup = False
                eventLogFunctions.writeCrashToEventLog(ex) ' Write exception data to the Windows Event Log.
            Finally
                ' Centralized cleanup for all allocated resources.
                If (Not hToken Is Nothing) Then
                    hToken.Close()
                    hToken = Nothing
                End If
                If (Not hTokenToCheck Is Nothing) Then
                    hTokenToCheck.Close()
                    hTokenToCheck = Nothing
                End If
                If (pElevationType <> IntPtr.Zero) Then
                    Runtime.InteropServices.Marshal.FreeHGlobal(pElevationType)
                    pElevationType = IntPtr.Zero
                End If
                If (pLinkedToken <> IntPtr.Zero) Then
                    Runtime.InteropServices.Marshal.FreeHGlobal(pLinkedToken)
                    pLinkedToken = IntPtr.Zero
                End If
            End Try

            Return fInAdminGroup
        End Function
    End Module
End Namespace