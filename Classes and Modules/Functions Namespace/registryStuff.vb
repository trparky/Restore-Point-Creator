Imports System.Text.RegularExpressions
Imports Microsoft.Win32

Namespace Functions.registryStuff
    Module registryStuff
        Private Function getFileTypeHandler(fileType As String) As String
            Try
                Dim registryKey As RegistryKey = Registry.ClassesRoot.OpenSubKey(fileType, False)

                If registryKey IsNot Nothing Then
                    Dim fileTypeNameInRegistry As String = registryKey.GetValue("")
                    Dim registryKey2 As RegistryKey = Registry.ClassesRoot.OpenSubKey(fileTypeNameInRegistry & "\shell\open\command", False)

                    If registryKey2 IsNot Nothing Then
                        Return registryKey2.GetValue("").ToString.Replace("""", "").Replace("%1", "").Trim
                    End If
                End If

                Return ""
            Catch ex As Exception
                Return ""
            End Try
        End Function

        Public Sub removeSafeModeBoot(Optional boolBypassRegCheck As Boolean = False)
            If boolBypassRegCheck = True Then
                support.doTheActualSafeModeBootRemoval()
            Else
                If Registry.LocalMachine.OpenSubKey(globalVariables.registryValues.strKey) IsNot Nothing Then
                    Dim strRegValue As String = Registry.LocalMachine.OpenSubKey(globalVariables.registryValues.strKey).GetValue(globalVariables.registryValues.strSafeModeValue, "False")
                    Dim boolRegValue As Boolean

                    If Boolean.TryParse(strRegValue, boolRegValue) = True Then
                        If boolRegValue = True Then
                            support.doTheActualSafeModeBootRemoval()
                        End If
                    End If
                End If
            End If
        End Sub

        Public Function getFileAssociation(ByVal fileExtension As String, ByRef associatedApplication As String) As Boolean
            Try
                fileExtension = fileExtension.ToLower.Trim
                If fileExtension.StartsWith(".") = False Then
                    fileExtension = "." & fileExtension
                End If

                Dim subPath As String = Registry.ClassesRoot.OpenSubKey(fileExtension, False).GetValue(vbNullString)
                Dim rawExecutablePath As String = Registry.ClassesRoot.OpenSubKey(subPath & "\shell\open\command", False).GetValue(vbNullString)

                ' We use this to parse out the executable path out of the regular junk in the string.
                Dim matches As Match = Regex.Match(rawExecutablePath, "(""{0, 1}[A-Za-z]:  \\.*\.(?:bat|bin|cmd|com|cpl|exe|gadget|inf1|ins|inx|isu|job|jse|lnk|msc|msi|msp|mst|paf|pif|ps1|reg|rgs|sct|shb|shs|u3p|vb|vbe|vbs|vbscript|ws|wsf)""{0,1})", RegexOptions.IgnoreCase)

                associatedApplication = matches.Groups(1).Value.Trim ' And return the value.
                Return True
            Catch ex As Exception
                Return False
            End Try
        End Function

        Public Sub updateRestorePointCreatorUninstallationInfo()
            Try
                Dim displayName As String
                Dim installerRegistryPath As RegistryKey = Registry.LocalMachine.OpenSubKey("SOFTWARE\Microsoft\Windows\CurrentVersion\Uninstall\{CC48DE1C-8EC2-43BC-9201-29701CD9AE13}_is1", True)

                If installerRegistryPath IsNot Nothing Then
                    displayName = installerRegistryPath.GetValue("DisplayName", "")

                    If displayName.caseInsensitiveContains("restore point creator") = True Then
                        installerRegistryPath.SetValue("DisplayName", String.Format("Restore Point Creator version {0}", globalVariables.version.strFullVersionString), RegistryValueKind.String)
                        installerRegistryPath.SetValue("DisplayVersion", globalVariables.version.versionInfo(enums.versionPieces.major) & "." & globalVariables.version.versionInfo(enums.versionPieces.minor), RegistryValueKind.String)
                        installerRegistryPath.SetValue("DisplayIcon", Application.ExecutablePath & ",0", RegistryValueKind.String)

                        installerRegistryPath.SetValue("Publisher", "Tom Parkison")
                        installerRegistryPath.SetValue("MajorVersion", globalVariables.version.versionInfo(enums.versionPieces.major), RegistryValueKind.DWord)
                        installerRegistryPath.SetValue("MinorVersion", globalVariables.version.versionInfo(enums.versionPieces.minor), RegistryValueKind.DWord)
                        installerRegistryPath.SetValue("Build", globalVariables.version.versionInfo(enums.versionPieces.build), RegistryValueKind.DWord)
                        installerRegistryPath.SetValue("InstallDate", support.getDateInShortForm())

                        eventLogFunctions.writeToSystemEventLog("Updated uninstall information in system Registry.", EventLogEntryType.Information)
                    End If

                    installerRegistryPath.Close()
                    installerRegistryPath.Dispose()
                End If
            Catch ex As Exception
                eventLogFunctions.writeToSystemEventLog("Unable to update uninstall information in system Registry.", EventLogEntryType.Error)
                eventLogFunctions.writeCrashToEventLog(ex)
            End Try
        End Sub

        ''' <summary>A depracated function that's used only in certain circumstances. It renames a Registry key by copying data from one key to another and deleting the original key.</summary>
        ''' <param name="parentKey">The parent Registry tree. Ex. HKEY_LOCAL_MACHINE.</param>
        ''' <param name="oldSubKeyName">Old key name.</param>
        ''' <param name="newSubKeyName">New key name.</param>
        Public Sub renameRegistrySubKey(parentKey As RegistryKey, oldSubKeyName As String, newSubKeyName As String)
            copyRegistryKey(parentKey, oldSubKeyName, newSubKeyName)
            parentKey.DeleteSubKeyTree(oldSubKeyName)
            parentKey.Close()
            parentKey.Dispose()
        End Sub

        Private Sub copyRegistryKey(parentKey As RegistryKey, oldSubKeyName As String, newSubKeyName As String)
            recurseCopyRegistryKey(parentKey.OpenSubKey(oldSubKeyName), parentKey.CreateSubKey(newSubKeyName))
        End Sub

        ''' <summary>A depracated function that's used only in certain circumstances. It copies the contents of one Registry key to another.</summary>
        ''' <param name="sourceKey">The source.</param>
        ''' <param name="destinationKey">The destination or new Registry key.</param>
        Private Sub recurseCopyRegistryKey(sourceKey As RegistryKey, destinationKey As RegistryKey)
            For Each valueName As String In sourceKey.GetValueNames()
                destinationKey.SetValue(valueName, sourceKey.GetValue(valueName), sourceKey.GetValueKind(valueName))
            Next

            For Each sourceSubKeyName As String In sourceKey.GetSubKeyNames()
                recurseCopyRegistryKey(sourceKey.OpenSubKey(sourceSubKeyName), destinationKey.CreateSubKey(sourceSubKeyName))
            Next
        End Sub
    End Module
End Namespace