Imports System.Security.AccessControl
Imports System.Security.Claims
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
            If WindowsIdentity.GetCurrent().IsSystem Then Return True

            Try
                Dim dsDirectoryACLs As DirectorySecurity = IO.Directory.GetAccessControl(folderPath)
                Dim strCurrentUserSDDL As String = WindowsIdentity.GetCurrent.User.Value
                Dim ircCurrentUserGroups As IdentityReferenceCollection = WindowsIdentity.GetCurrent.Groups

                Dim arcAuthorizationRules As AuthorizationRuleCollection = dsDirectoryACLs.GetAccessRules(True, True, GetType(SecurityIdentifier))
                Dim fsarDirectoryAccessRights As FileSystemAccessRule

                For Each arAccessRule As AuthorizationRule In arcAuthorizationRules
                    If arAccessRule.IdentityReference.Value.Equals(strCurrentUserSDDL, StringComparison.OrdinalIgnoreCase) Or ircCurrentUserGroups.Contains(arAccessRule.IdentityReference) Then
                        fsarDirectoryAccessRights = DirectCast(arAccessRule, FileSystemAccessRule)

                        If fsarDirectoryAccessRights.AccessControlType = AccessControlType.Allow Then
                            If fsarDirectoryAccessRights.FileSystemRights = FileSystemRights.Modify Or fsarDirectoryAccessRights.FileSystemRights = FileSystemRights.WriteData Or fsarDirectoryAccessRights.FileSystemRights = FileSystemRights.FullControl Then
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
                Dim principal As WindowsPrincipal = New WindowsPrincipal(WindowsIdentity.GetCurrent())
                Return principal.IsInRole(WindowsBuiltInRole.Administrator)
            Catch ex As Exception
                Return False
            End Try
        End Function

        Public Function IsUserInAdminGroup() As Boolean
            Dim identity As WindowsIdentity = WindowsIdentity.GetCurrent()
            Dim principal As New WindowsPrincipal(identity)

            If principal.IsInRole(WindowsBuiltInRole.Administrator) Then Return True ' Elevated
            Return identity.FindAll(ClaimTypes.DenyOnlySid).Any(Function(userClaimObject) userClaimObject.Value.Trim.Equals("S-1-5-114", StringComparison.OrdinalIgnoreCase))
        End Function
    End Module
End Namespace