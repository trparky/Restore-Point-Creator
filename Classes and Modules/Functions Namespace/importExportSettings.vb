Imports System.Text.RegularExpressions
Imports System.Xml
Imports Microsoft.Win32

Namespace Functions.importExportSettings
    Module importExportSettings
        Public Sub importSettingsFromLegacyBackupFile(strPathToFile As String, strMessageBoxTitle As String)
            Dim fileInfo As New IO.FileInfo(strPathToFile)
            Dim iniFile As New IniFile

            If fileInfo.Extension.Equals(".ini", StringComparison.OrdinalIgnoreCase) Then
                iniFile.loadINIFileFromFile(strPathToFile)
            Else
                Dim streamReader As New IO.StreamReader(strPathToFile)
                Dim strDataPayload = Nothing, strRandomString = Nothing, strChecksum = Nothing, strTemp As String

                While streamReader.EndOfStream = False
                    strTemp = streamReader.ReadLine

                    If strTemp.StartsWith("Payload: ", StringComparison.OrdinalIgnoreCase) = True Then
                        strDataPayload = strTemp.caseInsensitiveReplace("Payload: ", "").Trim
                    ElseIf strTemp.StartsWith("Random String: ", StringComparison.OrdinalIgnoreCase) = True Then
                        strRandomString = strTemp.caseInsensitiveReplace("Random String: ", "").Trim
                    ElseIf strTemp.StartsWith("Checksum: ", StringComparison.OrdinalIgnoreCase) = True Then
                        strChecksum = strTemp.caseInsensitiveReplace("Checksum: ", "").Trim
                    End If
                End While

                streamReader.Close()

                If calculateConfigBackupDataPayloadChecksum(strDataPayload, strRandomString) = strChecksum Then
                    iniFile.loadINIFileFromText(support.convertFromBase64(strDataPayload))
                Else
                    MsgBox("Invalid backup file.", MsgBoxStyle.Critical, strMessageBoxTitle)
                    Exit Sub
                End If
            End If

            Dim iniFileValue, iniFileKeyName As String
            Dim tempShort As Short, tempInteger As Integer, tempBoolean As Boolean
            Dim tempWidth, tempHeight As Integer
            Dim regExMatches As Match

            Dim systemDrawingSizeRegexObject As New Regex("\{Width=(?<width>[0-9]{1,4}), Height=(?<height>[0-9]{1,4})\}", RegexOptions.Compiled)

            For Each valueObject As Object In iniFile.GetSection("Settings").Keys
                Try
                    iniFileKeyName = valueObject.name
                    iniFileValue = valueObject.value

                    If iniFileValue.StartsWith("System.Int16") Then
                        iniFileValue = iniFileValue.caseInsensitiveReplace("System.Int16,", "")

                        If Short.TryParse(iniFileValue, tempShort) Then
                            My.Settings(iniFileKeyName) = tempShort
                        End If
                    ElseIf iniFileValue.StartsWith("System.Int32") Then
                        iniFileValue = iniFileValue.caseInsensitiveReplace("System.Int32,", "")

                        If Integer.TryParse(iniFileValue, tempInteger) Then
                            My.Settings(iniFileKeyName) = tempInteger
                        End If
                    ElseIf iniFileValue.StartsWith("System.Boolean") Then
                        iniFileValue = iniFileValue.caseInsensitiveReplace("System.Boolean,", "")

                        If Boolean.TryParse(iniFileValue, tempBoolean) Then
                            My.Settings(iniFileKeyName) = tempBoolean
                        End If
                    ElseIf iniFileValue.StartsWith("System.String") Then
                        My.Settings(iniFileKeyName) = iniFileValue.caseInsensitiveReplace("System.String,", "")
                    ElseIf iniFileValue.StartsWith("System.Drawing.Size") Then
                        iniFileValue = iniFileValue.caseInsensitiveReplace("System.Drawing.Size,", "")

                        If systemDrawingSizeRegexObject.IsMatch(iniFileValue) = True Then
                            regExMatches = systemDrawingSizeRegexObject.Match(iniFileValue)

                            tempWidth = Integer.Parse(regExMatches.Groups("width").Value)
                            tempHeight = Integer.Parse(regExMatches.Groups("height").Value)

                            regExMatches = Nothing
                            My.Settings(iniFileKeyName) = New Size(tempWidth, tempHeight)
                        End If
                    ElseIf iniFileValue.StartsWith("System.Drawing.Color") Then
                        iniFileValue = iniFileValue.caseInsensitiveReplace("System.Drawing.Color,", "")

                        If Integer.TryParse(iniFileValue, tempInteger) Then
                            My.Settings(iniFileKeyName) = Color.FromArgb(tempInteger)
                        End If
                    ElseIf iniFileValue.StartsWith("System.Drawing.Point") Then
                        iniFileValue = iniFileValue.caseInsensitiveReplace("System.Drawing.Point,", "").Trim
                        Dim pointParts() As String = iniFileValue.Split("|")
                        My.Settings(iniFileKeyName) = New Point(pointParts(0), pointParts(1))
                    End If
                Catch ex As Configuration.SettingsPropertyNotFoundException
                    ' Does nothing
                End Try
            Next

            My.Settings.Save()

            systemDrawingSizeRegexObject = Nothing

            Dim registryKeyWeAreWorkingWith As RegistryKey = Registry.LocalMachine.OpenSubKey(globalVariables.registryValues.strKey, True)

            For Each valueObject As Object In iniFile.GetSection("Registry").Keys
                iniFileKeyName = valueObject.name.ToString.Replace("_", " ")
                iniFileValue = valueObject.value.ToString

                registryKeyWeAreWorkingWith.SetValue(iniFileKeyName, iniFileValue, RegistryValueKind.String)
            Next

            registryKeyWeAreWorkingWith.Close()

            iniFile = Nothing
        End Sub

        Public Sub exportSettingsToXMLFile(strPathToXMLFile As String)
            Dim exportedSettingsArray As New List(Of exportedSettings)
            Dim exportedSettingsObject As exportedSettings
            Dim settingType As Type
            Dim point As Point, size As Size

            For Each settingProperty As Configuration.SettingsPropertyValue In My.Settings.PropertyValues
                If settingProperty.PropertyValue IsNot Nothing Then
                    settingType = settingProperty.PropertyValue.GetType
                    exportedSettingsObject = New exportedSettings With {.settingType = Restore_Point_Creator.settingType.settings}

                    exportedSettingsObject.strName = settingProperty.Name
                    exportedSettingsObject.type = settingProperty.PropertyValue.GetType.ToString

                    If settingType = GetType(Point) Then
                        point = DirectCast(settingProperty.PropertyValue, Point)
                        exportedSettingsObject.value = point.X & "|" & point.Y
                        point = Nothing
                    ElseIf settingType = GetType(Color) Then
                        exportedSettingsObject.value = DirectCast(settingProperty.PropertyValue, Color).ToArgb
                    ElseIf settingType = GetType(Size) Then
                        size = DirectCast(settingProperty.PropertyValue, Size)
                        exportedSettingsObject.value = size.Height & "|" & size.Width
                        size = Nothing
                    ElseIf settingType = GetType(Date) Or settingType = GetType(DateTime) Then
                        exportedSettingsObject.value = DirectCast(settingProperty.PropertyValue, Date).ToUniversalTime
                    Else
                        exportedSettingsObject.value = settingProperty.PropertyValue
                    End If

                    exportedSettingsArray.Add(exportedSettingsObject)
                    exportedSettingsObject = Nothing
                End If
            Next

            Dim registryKeyWeAreWorkingWith As RegistryKey = Registry.LocalMachine.OpenSubKey(globalVariables.registryValues.strKey, False)
            Dim registryValue As String

            For Each registryValueName As String In registryKeyWeAreWorkingWith.GetValueNames
                registryValue = registryKeyWeAreWorkingWith.GetValue(registryValueName, Nothing)

                If registryValue IsNot Nothing Then
                    exportedSettingsObject = New exportedSettings With {.settingType = Restore_Point_Creator.settingType.registry, .type = GetType(String).ToString}

                    exportedSettingsObject.strName = registryValueName
                    exportedSettingsObject.value = registryValue

                    exportedSettingsArray.Add(exportedSettingsObject)
                    exportedSettingsObject = Nothing
                    registryValue = Nothing
                End If
            Next

            registryKeyWeAreWorkingWith.Close()

            Try
                Dim xmlSerializerObject As Serialization.XmlSerializer

                Dim memStream As New IO.MemoryStream
                xmlSerializerObject = New Serialization.XmlSerializer(exportedSettingsArray.GetType)
                xmlSerializerObject.Serialize(memStream, exportedSettingsArray)
                memStream.Position = 0

                Dim streamReader As New IO.StreamReader(memStream)
                Dim strXMLData As String = streamReader.ReadToEnd

                streamReader.Close()
                memStream.Close()
                xmlSerializerObject = Nothing

                Dim exportedSettingsFile As New exportedSettingsFile()
                With exportedSettingsFile
                    .xmlPayload = support.convertToBase64(strXMLData)
                    .randomString = support.randomStringGenerator((New Random).Next(100, 300))
                    .checksum = calculateConfigBackupDataPayloadChecksum(.xmlPayload, .randomString)
                End With

                Dim streamWriter As New IO.StreamWriter(strPathToXMLFile)
                xmlSerializerObject = New Serialization.XmlSerializer(exportedSettingsFile.GetType)
                xmlSerializerObject.Serialize(streamWriter, exportedSettingsFile)
                streamWriter.Close()
            Catch ex As Exception
                eventLogFunctions.writeCrashToEventLog(ex)
                exceptionHandler.manuallyLoadCrashWindow(ex, ex.Message, ex.StackTrace, ex.GetType)
            End Try
        End Sub

        Private Function doesThisApplicationSettingExist(strSettingName As String) As Boolean
            If My.Settings.Properties.Cast(Of Configuration.SettingsProperty).ToList().Where(Function(setting As Configuration.SettingsProperty) setting.Name.Equals(strSettingName, StringComparison.OrdinalIgnoreCase)).Count > 0 Then
                Return True
            Else
                Return False
            End If
        End Function

        Public Sub importSettingsFromXMLFile(strPathToFile As String, strMessageBoxTitle As String)
            Dim xmlSerializerObject As Serialization.XmlSerializer
            Dim bytes() As Byte

            'exportedSettingsFile
            Dim streamReader As New IO.StreamReader(strPathToFile)
            Dim exportedSettingsFile As New exportedSettingsFile()
            xmlSerializerObject = New Serialization.XmlSerializer(exportedSettingsFile.GetType)
            exportedSettingsFile = xmlSerializerObject.Deserialize(streamReader)
            streamReader.Close()
            xmlSerializerObject = Nothing

            With exportedSettingsFile
                If Not calculateConfigBackupDataPayloadChecksum(.xmlPayload, .randomString).Equals(.checksum) Then
                    MsgBox("Checksum validation failed.", MsgBoxStyle.Critical, strMessageBoxTitle)
                End If

                bytes = Text.Encoding.UTF8.GetBytes(support.convertFromBase64(.xmlPayload))
            End With

            Dim memStream As New IO.MemoryStream(bytes)
            bytes = Nothing
            Dim exportedSettingsArray As New List(Of exportedSettings)
            xmlSerializerObject = New Serialization.XmlSerializer(exportedSettingsArray.GetType)
            exportedSettingsArray = xmlSerializerObject.Deserialize(memStream)
            memStream.Close()
            xmlSerializerObject = Nothing

            Dim registryKeyWeAreWorkingWith As RegistryKey = Registry.LocalMachine.OpenSubKey(globalVariables.registryValues.strKey, True)
            Dim type As Type
            Dim splitArray As String()
            Dim boolParseSuccessful As Boolean

            For Each item As exportedSettings In exportedSettingsArray
                If item.settingType = settingType.registry Then
                    registryKeyWeAreWorkingWith.SetValue(item.strName, item.value.ToString, RegistryValueKind.String)
                ElseIf item.settingType = settingType.settings Then
                    type = parseTypeString(item.type, boolParseSuccessful)

                    If boolParseSuccessful Then
                        If type = GetType(Color) Then
                            If doesThisApplicationSettingExist(item.strName) Then My.Settings(item.strName) = Color.FromArgb(item.value)
                        ElseIf type = GetType(Point) Then
                            splitArray = item.value.split("|")
                            If doesThisApplicationSettingExist(item.strName) Then My.Settings(item.strName) = New Point() With {.X = splitArray(0), .Y = splitArray(1)}
                            splitArray = Nothing
                        ElseIf type = GetType(Size) Then
                            splitArray = item.value.split("|")
                            If doesThisApplicationSettingExist(item.strName) Then My.Settings(item.strName) = New Size() With {.Height = splitArray(0), .Width = splitArray(1)}
                            splitArray = Nothing
                        ElseIf type = GetType(Date) Then
                            If doesThisApplicationSettingExist(item.strName) Then My.Settings(item.strName) = DirectCast(item.value, Date).ToLocalTime
                        Else
                            If doesThisApplicationSettingExist(item.strName) Then My.Settings(item.strName) = item.value
                        End If
                    End If
                End If
            Next

            registryKeyWeAreWorkingWith.Close()
        End Sub

        Private Function calculateConfigBackupDataPayloadChecksum(strInput As String, strSaltedString As String) As String
            Dim string1, string2, string3, string4 As String
            string1 = strInput.Substring(0, 30)
            string2 = strInput.Substring(29, 30)
            string3 = strInput.Substring(59, 30)
            string4 = strInput.Substring(89, 30)

            Dim compoundString As String = strInput & string4 & strSaltedString & string1 & strInput & strSaltedString & string4 & string3 & strInput & strSaltedString & string2 & string1 & strInput & strSaltedString

            Return SHA512ChecksumString(compoundString)
        End Function

        Private Function parseTypeString(input As String, ByRef boolParseSuccessful As Boolean) As Type
            Try
                Dim type As Type = Type.GetType(input.Trim, True, True)
                boolParseSuccessful = True
                Return type
            Catch ex As Exception
                boolParseSuccessful = False
                Return Nothing
            End Try
        End Function

        Private Function SHA512ChecksumString(input As String) As String
            Dim SHA1Engine As New Security.Cryptography.SHA512CryptoServiceProvider
            Dim inputAsByteArray As Byte() = Text.Encoding.ASCII.GetBytes(input)
            Dim hashedByteArray As Byte() = SHA1Engine.ComputeHash(inputAsByteArray)
            Return BitConverter.ToString(hashedByteArray).ToLower().Replace("-", "").Trim
        End Function
    End Module
End Namespace