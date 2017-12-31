Imports System.Text.RegularExpressions
Imports Microsoft.Win32

Namespace Functions.registryStuff
    Module registryStuff
        Public Function getFileAssociation(ByVal fileExtension As String, ByRef associatedApplication As String) As Boolean
            Try
                fileExtension = fileExtension.ToLower.Trim
                If Not fileExtension.StartsWith(".") Then
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

                    If displayName.caseInsensitiveContains("restore point creator") Then
                        If globalVariables.version.boolBeta Then
                            installerRegistryPath.SetValue("DisplayName", String.Format("Restore Point Creator version {0} Public Beta {1}", globalVariables.version.strFullVersionString, globalVariables.version.shortBetaVersion), RegistryValueKind.String)
                        ElseIf globalVariables.version.boolReleaseCandidate Then
                            installerRegistryPath.SetValue("DisplayName", String.Format("Restore Point Creator version {0} Release Candidate {1}", globalVariables.version.strFullVersionString, globalVariables.version.shortReleaseCandidateVersion), RegistryValueKind.String)
                        Else
                            installerRegistryPath.SetValue("DisplayName", String.Format("Restore Point Creator version {0}", globalVariables.version.strFullVersionString), RegistryValueKind.String)
                        End If

                        installerRegistryPath.SetValue("DisplayVersion", globalVariables.version.versionInfo(enums.versionPieces.major) & "." & globalVariables.version.versionInfo(enums.versionPieces.minor), RegistryValueKind.String)
                        installerRegistryPath.SetValue("DisplayIcon", Application.ExecutablePath.caseInsensitiveReplace(".new.exe", "") & ",0", RegistryValueKind.String)

                        installerRegistryPath.SetValue("Publisher", "Tom Parkison")
                        installerRegistryPath.SetValue("MajorVersion", globalVariables.version.versionInfo(enums.versionPieces.major), RegistryValueKind.DWord)
                        installerRegistryPath.SetValue("MinorVersion", globalVariables.version.versionInfo(enums.versionPieces.minor), RegistryValueKind.DWord)
                        installerRegistryPath.SetValue("Build", globalVariables.version.versionInfo(enums.versionPieces.build), RegistryValueKind.DWord)
                        installerRegistryPath.SetValue("InstallDate", support.getDateInShortForm())

                        eventLogFunctions.writeToApplicationLogFile("Updated uninstall information in system Registry.", EventLogEntryType.Information)
                    End If

                    installerRegistryPath.Close()
                End If
            Catch ex As Exception
                eventLogFunctions.writeToApplicationLogFile("Unable to update uninstall information in system Registry.", EventLogEntryType.Error)
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

        ''' <summary>Creates a Registry value in the application's Registry key.</summary>
        ''' <param name="strValueNameToSetInRegistry">The name of the Registry value you want to create.</param>
        ''' <param name="strValueToSet">The data for the Registry value you want to create.</param>
        Public Sub setValueInRegistry(ByVal strValueNameToSetInRegistry As String, ByVal strValueToSet As String)
            Dim registryKey As RegistryKey = Registry.LocalMachine.OpenSubKey(globalVariables.registryValues.strKey, True)
            setValueInRegistry(registryKey, strValueNameToSetInRegistry, strValueToSet, True)
        End Sub

        ''' <summary>Creates a Registry value in the application's Registry key.</summary>
        ''' <param name="strValueNameToSetInRegistry">The name of the Registry value you want to create.</param>
        ''' <param name="strValueToSet">The data for the Registry value you want to create.</param>
        ''' <param name="registryKey">The Registry Object you want to act on.</param>
        ''' <param name="boolCloseAfterSettingValue">This is an optional Boolean value, this is normally set to False.</param>
        Public Sub setValueInRegistry(ByRef registryKey As RegistryKey, ByVal strValueNameToSetInRegistry As String, ByVal strValueToSet As String, Optional boolCloseAfterSettingValue As Boolean = False)
            If registryKey Is Nothing Then
                registryKey = Registry.LocalMachine.OpenSubKey(globalVariables.registryValues.strKey, True)
            End If

            registryKey.SetValue(strValueNameToSetInRegistry, strValueToSet, RegistryValueKind.String)

            If boolCloseAfterSettingValue Then
                registryKey.Close()
            End If
        End Sub

        ''' <summary>Creates a Boolean value Registry value in the application's Registry key.</summary>
        ''' <param name="strValueNameToSetInRegistry">The name of the Registry value you want to create.</param>
        ''' <param name="boolValueToSet">The Boolean value for the Registry value you want to create.</param>
        Public Sub setBooleanValueInRegistry(ByVal strValueNameToSetInRegistry As String, ByVal boolValueToSet As Boolean)
            setValueInRegistry(strValueNameToSetInRegistry, boolValueToSet.ToString)
        End Sub

        ''' <summary>Creates a Boolean value Registry value in the application's Registry key.</summary>
        ''' <param name="strValueNameToSetInRegistry">The name of the Registry value you want to create.</param>
        ''' <param name="boolValueToSet">The Boolean value for the Registry value you want to create.</param>
        ''' <param name="registryKey">The Registry Object you want to act on.</param>
        ''' <param name="boolCloseAfterSettingValue">This is an optional Boolean value, this is normally set to False.</param>
        Public Sub setBooleanValueInRegistry(ByRef registryKey As RegistryKey, ByVal strValueNameToSetInRegistry As String, ByVal boolValueToSet As Boolean, Optional boolCloseAfterSettingValue As Boolean = False)
            setValueInRegistry(registryKey, strValueNameToSetInRegistry, boolValueToSet.ToString, boolCloseAfterSettingValue)
        End Sub

        ''' <summary>Gets a setting from the application's Registry key.</summary>
        ''' <param name="registryObject">This is Registry Key Object that will be used in this function to pull the Registry value from.</param>
        ''' <param name="valueToGetFromRegistry">The name of the Registry Value we will be pulling from.</param>
        ''' <param name="boolDefaultValue">If the Registry Value isn't found or the value is malformed, this will be the Boolean value that this function will return.</param>
        ''' <returns>A Boolean value.</returns>
        Public Function getBooleanValueFromRegistry(ByRef registryObject As RegistryKey, ByVal valueToGetFromRegistry As String, ByVal boolDefaultValue As Boolean) As Boolean
            Try
                Dim boolTemp As Boolean
                Dim strDefaultValue As String = If(boolDefaultValue, globalVariables.booleans.strTrue, globalVariables.booleans.strFalse)

                If Not Boolean.TryParse(registryObject.GetValue(valueToGetFromRegistry, strDefaultValue), boolTemp) Then boolTemp = boolDefaultValue

                Return boolTemp
            Catch ex As Exception
                Return boolDefaultValue
            End Try
        End Function

        ''' <summary>Gets a setting from the application's Registry key.</summary>
        ''' <param name="valueToGetFromRegistry">The name of the Registry Value we will be pulling from.</param>
        ''' <param name="boolDefaultValue">If the Registry Value isn't found or the value is malformed, this will be the Boolean value that this function will return.</param>
        ''' <returns>A Boolean value.</returns>
        Public Function getBooleanValueFromRegistry(ByVal valueToGetFromRegistry As String, ByVal boolDefaultValue As Boolean) As Boolean
            Using registryObject As RegistryKey = Registry.LocalMachine.OpenSubKey(globalVariables.registryValues.strKey, False)
                If registryObject Is Nothing Then
                    Return boolDefaultValue
                Else
                    Return getBooleanValueFromRegistry(registryObject, valueToGetFromRegistry, boolDefaultValue)
                End If
            End Using
        End Function
    End Module
End Namespace