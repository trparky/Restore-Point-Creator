Imports System.Text.RegularExpressions
Imports ICSharpCode.SharpZipLib.Zip
Imports System.Xml
Imports Microsoft.Win32

Namespace Functions.support
    Public Enum updateType
        null = 0
        release = 1
        candidate = 2
        beta = 3
        buildLessThanError = 4
    End Enum

    Module support
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

        Public Function verifyWindowLocation(point As Point) As Point
            If point.X < 0 Or point.Y < 0 Then
                Return New Point(0, 0)
            Else
                Return point
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
            streamReader.Dispose()
            xmlSerializerObject = Nothing

            With exportedSettingsFile
                If Not calculateConfigBackupDataPayloadChecksum(.xmlPayload, .randomString).Equals(.checksum) Then
                    MsgBox("Checksum validation failed.", MsgBoxStyle.Critical, strMessageBoxTitle)
                End If

                bytes = Text.Encoding.UTF8.GetBytes(convertFromBase64(.xmlPayload))
            End With

            Dim memStream As New IO.MemoryStream(bytes)
            bytes = Nothing
            Dim exportedSettingsArray As New List(Of exportedSettings)
            xmlSerializerObject = New Serialization.XmlSerializer(exportedSettingsArray.GetType)
            exportedSettingsArray = xmlSerializerObject.Deserialize(memStream)
            memStream.Close()
            memStream.Dispose()
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
                            My.Settings(item.strName) = Color.FromArgb(item.value)
                        ElseIf type = GetType(Point) Then
                            splitArray = item.value.split("|")
                            My.Settings(item.strName) = New Point() With {.X = splitArray(0), .Y = splitArray(1)}
                            splitArray = Nothing
                        ElseIf type = GetType(Size) Then
                            splitArray = item.value.split("|")
                            My.Settings(item.strName) = New Size() With {.Height = splitArray(0), .Width = splitArray(1)}
                            splitArray = Nothing
                        ElseIf type = GetType(Specialized.StringCollection) Then
                            Dim deJSONedObject As String() = (New Web.Script.Serialization.JavaScriptSerializer).Deserialize(item.value, GetType(String()))

                            Dim tempStringCollection As New Specialized.StringCollection
                            For i = 0 To deJSONedObject.Count - 1
                                tempStringCollection.Add(deJSONedObject(i))
                            Next

                            My.Settings(item.strName) = tempStringCollection
                        ElseIf type = GetType(Date) Then
                            My.Settings(item.strName) = DirectCast(item.value, Date).ToLocalTime
                        Else
                            My.Settings(item.strName) = item.value
                        End If
                    End If
                End If
            Next

            registryKeyWeAreWorkingWith.Close()
            registryKeyWeAreWorkingWith.Dispose()
        End Sub

        Public Sub importSettingsFromLegacyBackupFile(strPathToFile As String, strMessageBoxTitle As String)
            Dim fileInfo As New IO.FileInfo(strPathToFile)
            Dim iniFile As New IniFile

            If fileInfo.Extension.stringCompare(".ini") Then
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
                streamReader.Dispose()

                If calculateConfigBackupDataPayloadChecksum(strDataPayload, strRandomString) = strChecksum Then
                    iniFile.loadINIFileFromText(convertFromBase64(strDataPayload))
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
                    ElseIf iniFileValue.StartsWith("System.Collections.Specialized.StringCollection") Then
                        iniFileValue = iniFileValue.caseInsensitiveReplace("System.Collections.Specialized.StringCollection,", "")
                        Dim deJSONedObject As String() = (New Web.Script.Serialization.JavaScriptSerializer).Deserialize(iniFileValue, GetType(String()))

                        Dim tempStringCollection As New Specialized.StringCollection
                        For i = 0 To deJSONedObject.Count - 1
                            tempStringCollection.Add(deJSONedObject(i))
                        Next
                        My.Settings(iniFileKeyName) = tempStringCollection
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
            registryKeyWeAreWorkingWith.Dispose()

            iniFile = Nothing
        End Sub

        Public Sub exportSettingsToXMLFile(strPathToXMLFile As String)
            Dim exportedSettingsArray As New List(Of exportedSettings)
            Dim exportedSettingsObject As exportedSettings
            Dim settingType As Type
            Dim stringCollection As Specialized.StringCollection
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
                    ElseIf settingType = GetType(Specialized.StringCollection) Then
                        stringCollection = DirectCast(settingProperty.PropertyValue, Specialized.StringCollection)

                        Dim tempArray(stringCollection.Count - 1) As String
                        stringCollection.CopyTo(tempArray, 0)

                        exportedSettingsObject.value = (New Web.Script.Serialization.JavaScriptSerializer).Serialize(tempArray)

                        stringCollection = Nothing
                        tempArray = Nothing
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
            registryKeyWeAreWorkingWith.Dispose()

            Try
                Dim xmlSerializerObject As Serialization.XmlSerializer

                Dim memStream As New IO.MemoryStream
                xmlSerializerObject = New Serialization.XmlSerializer(exportedSettingsArray.GetType)
                xmlSerializerObject.Serialize(memStream, exportedSettingsArray)
                memStream.Position = 0

                Dim streamReader As New IO.StreamReader(memStream)
                Dim strXMLData As String = streamReader.ReadToEnd

                streamReader.Close()
                streamReader.Dispose()
                memStream.Close()
                memStream.Dispose()
                xmlSerializerObject = Nothing

                Dim exportedSettingsFile As New exportedSettingsFile()
                With exportedSettingsFile
                    .xmlPayload = convertToBase64(strXMLData)
                    .randomString = randomStringGenerator((New Random).Next(100, 300))
                    .checksum = calculateConfigBackupDataPayloadChecksum(.xmlPayload, .randomString)
                End With

                Dim streamWriter As New IO.StreamWriter(strPathToXMLFile)
                xmlSerializerObject = New Serialization.XmlSerializer(exportedSettingsFile.GetType)
                xmlSerializerObject.Serialize(streamWriter, exportedSettingsFile)
                streamWriter.Close()
                streamWriter.Dispose()
            Catch ex As Exception
                eventLogFunctions.writeCrashToEventLog(ex)
                exceptionHandler.manuallyLoadCrashWindow(ex, ex.Message, ex.StackTrace, ex.GetType)
            End Try
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

        Private Function SHA512ChecksumString(input As String) As String
            Dim SHA1Engine As New Security.Cryptography.SHA512CryptoServiceProvider
            Dim inputAsByteArray As Byte() = Text.Encoding.ASCII.GetBytes(input)
            Dim hashedByteArray As Byte() = SHA1Engine.ComputeHash(inputAsByteArray)
            Return BitConverter.ToString(hashedByteArray).ToLower().Replace("-", "").Trim
        End Function

        Public Function processUpdateXMLData(ByVal xmlData As String, ByRef updateType As updateType, ByRef remoteVersion As String, ByRef remoteBuild As String, ByRef strRemoteBetaRCVersion As String) As Boolean
            Try
                Dim strRemoteType As String
                Dim shortRemoteBuild As Short

                updateType = updateType.null ' Give it a default value of null.
                strRemoteBetaRCVersion = Nothing ' Give it a default value of Nothing.

                Dim xmlDocument As New XmlDocument() ' First we create an XML Document Object.
                xmlDocument.Load(New IO.StringReader(xmlData)) ' Now we try and parse the XML data.

                Dim xmlNode As XmlNode = xmlDocument.SelectSingleNode(String.Format("/xmlroot/{0}", My.Settings.updateChannel))

                remoteVersion = xmlNode.SelectSingleNode("version").InnerText.Trim
                remoteBuild = xmlNode.SelectSingleNode("build").InnerText.Trim
                strRemoteType = xmlNode.SelectSingleNode("type").InnerText.Trim

                ' This checks to see if current version and the current build matches that of the remote values in the XML document.
                If remoteVersion.Equals(globalVariables.version.versionStringWithoutBuild) And remoteBuild.Equals(globalVariables.version.shortBuild.ToString) Then
                    ' OK, they match so there's no update to download and update to therefore we return a False value.
                    Return False
                Else
                    ' Nope, they don't match so let's do some additional checks before we just assume that there's an update.

                    If Short.TryParse(remoteBuild, shortRemoteBuild) Then
                        If shortRemoteBuild < globalVariables.version.shortBuild And remoteVersion.Equals(globalVariables.version.versionStringWithoutBuild) Then
                            updateType = updateType.buildLessThanError

                            ' This is weird, the remote build is less than the current build. Something went wrong. So to be safe we're going to return a False value indicating that there is no update to download. Better to be safe.
                            Return False
                        End If
                    End If

                    ' Let's check to see if the user's update channel preference is set to "beta" or "tom".
                    If My.Settings.updateChannel.Equals(globalVariables.updateChannels.beta) Or My.Settings.updateChannel.Equals(globalVariables.updateChannels.tom) Then
                        ' Yes, it is set to the "beta" or "tom" channel.

                        ' Now let's check to see if the user wants any betas if the current new version is a beta.
                        If strRemoteType.Equals("beta", StringComparison.OrdinalIgnoreCase) And My.Settings.onlyGiveMeRCs Then
                            ' Nope, the user doesn't want betas versions, they only want release candidates, so we return
                            ' a False value indicating that there is no new version to download and update to.
                            Return False
                        End If

                        ' Let's try and get the "betaRCVersion" from the XML document. We don't want to crash so we check to see if the object is null first.
                        If xmlNode.SelectSingleNode("betaRCVersion") IsNot Nothing Then
                            ' Get the "strRemoteBetaRCVersion" version string from the XML data.
                            strRemoteBetaRCVersion = xmlNode.SelectSingleNode("betaRCVersion").InnerText.Trim
                        End If

                        ' Now we need to check the remote update type.
                        If strRemoteType.Equals("release", StringComparison.OrdinalIgnoreCase) Then
                            ' It's a release type so we set the update type to "release".
                            updateType = updateType.release
                        ElseIf strRemoteType.Equals("candidate", StringComparison.OrdinalIgnoreCase) Then
                            ' It's a "rc" or candidate so we set the update type to "candidate".
                            updateType = updateType.candidate
                        ElseIf strRemoteType.Equals("beta", StringComparison.OrdinalIgnoreCase) Then
                            ' It's a beta so we set the update type to "beta".
                            updateType = updateType.beta
                        End If
                    Else
                        ' The user's update channel is set to stable so we set the update type to "release".
                        updateType = updateType.release
                    End If

                    Return True ' And now we return a True value indicating that there is a new version to download and install.
                End If
            Catch ex As XPath.XPathException
                eventLogFunctions.writeToSystemEventLog("There was an error while parsing the XML document. The contents of the XML document are below..." & vbCrLf & vbCrLf & xmlData, EventLogEntryType.Error)
                eventLogFunctions.writeCrashToEventLog(ex)

                ' Something went wrong so we return a False value.
                Return False
            Catch ex As XmlException
                eventLogFunctions.writeToSystemEventLog("There was an error while parsing the XML document. The contents of the XML document are below..." & vbCrLf & vbCrLf & xmlData, EventLogEntryType.Error)
                eventLogFunctions.writeCrashToEventLog(ex)

                ' Something went wrong so we return a False value.
                Return False
            Catch ex As Exception
                ' Something went wrong so we return a False value.
                Return False
            End Try
        End Function

        Public Function convertErrorCodeToHex(input As Long) As String
            Try
                Return input.ToString("x").caseInsensitiveReplace("ffffffff", "0x").ToString.ToUpper
            Catch ex As Exception
                Return Nothing
            End Try
        End Function

        Private Sub addJSONedExtendedExceptionDataPackage(ByVal exceptionObject As Exception, ByRef stringBuilder As System.Text.StringBuilder)
            Try
                Dim jsonTemp As String = jsonObject(exceptionObject.Data)

                If Not jsonTemp.Equals("{}") Then
                    stringBuilder.AppendLine(String.Format("Additional {0} Data: {1}", exceptionObject.GetType.ToString, jsonTemp))
                End If
            Catch ex As Exception
            End Try
        End Sub

        Public Sub addExtendedCrashData(ByRef stringBuilder As System.Text.StringBuilder, rawExceptionObject As Exception)
            Try
                Dim exceptionType As Type = rawExceptionObject.GetType

                If exceptionType.Equals(GetType(IO.FileNotFoundException)) Or
                    exceptionType.Equals(GetType(IO.FileLoadException)) Or
                    exceptionType.Equals(GetType(Runtime.InteropServices.COMException)) Or
                    exceptionType.Equals(GetType(IO.IOException)) Or
                    exceptionType.Equals(GetType(ArgumentOutOfRangeException)) Or
                    exceptionType.Equals(GetType(FormatException)) Or
                    exceptionType.Equals(GetType(ComponentModel.Win32Exception)) Or
                    exceptionType.Equals(GetType(XPath.XPathException)) Or
                    exceptionType.Equals(GetType(XmlException)) Or
                    exceptionType.Equals(GetType(InvalidOperationException)) Or
                    exceptionType.Equals(GetType(myExceptions.integerTryParseException)) Or
                    exceptionType.Equals(GetType(IO.DirectoryNotFoundException)) Or
                    exceptionType.Equals(GetType(ObjectDisposedException)) Then

                    stringBuilder.AppendLine()
                    stringBuilder.AppendLine("Additional " & rawExceptionObject.GetType.ToString & " Data")

                    If exceptionType.Equals(GetType(IO.FileNotFoundException)) Then
                        Dim FileNotFoundExceptionObject As IO.FileNotFoundException = DirectCast(rawExceptionObject, IO.FileNotFoundException)
                        stringBuilder.AppendLine("Name of File: " & FileNotFoundExceptionObject.FileName)

                        If Not String.IsNullOrEmpty(FileNotFoundExceptionObject.FusionLog) Then
                            stringBuilder.AppendLine("Reason: " & FileNotFoundExceptionObject.FusionLog)
                        End If
                    ElseIf exceptionType.Equals(GetType(IO.DirectoryNotFoundException)) Then
                        Dim DirectoryNotFoundExceptionObject As IO.DirectoryNotFoundException = DirectCast(rawExceptionObject, IO.DirectoryNotFoundException)
                        stringBuilder.AppendLine("Source: " & DirectoryNotFoundExceptionObject.Source)
                        addJSONedExtendedExceptionDataPackage(DirectoryNotFoundExceptionObject, stringBuilder)
                    ElseIf exceptionType.Equals(GetType(myExceptions.integerTryParseException)) Then
                        stringBuilder.AppendLine("String that could not be parsed into an Integer: " & DirectCast(rawExceptionObject, myExceptions.integerTryParseException).strThatCouldNotBeParsedIntoAnInteger)
                    ElseIf exceptionType.Equals(GetType(XmlException)) Then
                        Dim XmlExceptionObject As XmlException = DirectCast(rawExceptionObject, XmlException)
                        stringBuilder.AppendLine("Line Number: " & XmlExceptionObject.LineNumber)
                        stringBuilder.AppendLine("Line Position: " & XmlExceptionObject.LinePosition)

                        addJSONedExtendedExceptionDataPackage(XmlExceptionObject, stringBuilder)
                    ElseIf exceptionType.Equals(GetType(XPath.XPathException)) Then
                        Dim XPathExceptionExceptionObject As XPath.XPathException = DirectCast(rawExceptionObject, XPath.XPathException)
                        addJSONedExtendedExceptionDataPackage(XPathExceptionExceptionObject, stringBuilder)
                    ElseIf exceptionType.Equals(GetType(IO.FileLoadException)) Then
                        Dim FileLoadExceptionObject As IO.FileLoadException = DirectCast(rawExceptionObject, IO.FileLoadException)
                        stringBuilder.AppendLine("Unable to Load Assembly File: " & FileLoadExceptionObject.FileName)

                        If Not String.IsNullOrEmpty(FileLoadExceptionObject.FusionLog) Then
                            stringBuilder.AppendLine("Reason why assembly couldn't be loaded: " & FileLoadExceptionObject.FusionLog)
                        End If
                    ElseIf exceptionType.Equals(GetType(Runtime.InteropServices.COMException)) Then
                        Dim COMExceptionObject As Runtime.InteropServices.COMException = DirectCast(rawExceptionObject, Runtime.InteropServices.COMException)
                        stringBuilder.AppendLine("Source: " & COMExceptionObject.Source)
                        stringBuilder.AppendLine("Error Code: " & COMExceptionObject.ErrorCode)
                    ElseIf exceptionType.Equals(GetType(ObjectDisposedException)) Then
                        Dim ObjectDisposedExceptionObject As ObjectDisposedException = DirectCast(rawExceptionObject, ObjectDisposedException)
                        stringBuilder.AppendLine("Source: " & ObjectDisposedExceptionObject.Source)
                        stringBuilder.AppendLine("Object Name: " & ObjectDisposedExceptionObject.ObjectName)
                    ElseIf exceptionType.Equals(GetType(IO.IOException)) Then
                        stringBuilder.AppendLine("Source: " & DirectCast(rawExceptionObject, IO.IOException).Source)
                    ElseIf exceptionType.Equals(GetType(ArgumentOutOfRangeException)) Then
                        Dim ArgumentOutOfRangeExceptionObject As ArgumentOutOfRangeException = DirectCast(rawExceptionObject, ArgumentOutOfRangeException)
                        stringBuilder.AppendLine("Parameter Name: " & ArgumentOutOfRangeExceptionObject.ParamName)
                        stringBuilder.AppendLine("Parameter Value: " & ArgumentOutOfRangeExceptionObject.ActualValue)
                    ElseIf exceptionType.Equals(GetType(ArgumentException)) Then
                        stringBuilder.AppendLine("Parameter Name: " & DirectCast(rawExceptionObject, ArgumentException).ParamName)
                    ElseIf exceptionType.Equals(GetType(ComponentModel.Win32Exception)) Then
                        Dim Win32ExceptionObject As ComponentModel.Win32Exception = DirectCast(rawExceptionObject, ComponentModel.Win32Exception)
                        stringBuilder.AppendLine(String.Format("Error Code: {0} ({1})", Win32ExceptionObject.ErrorCode, convertErrorCodeToHex(Win32ExceptionObject.ErrorCode)))
                        stringBuilder.AppendLine(String.Format("Native Error Code: {0} ({1})", Win32ExceptionObject.NativeErrorCode, convertErrorCodeToHex(Win32ExceptionObject.NativeErrorCode)))
                    End If

                    addJSONedExtendedExceptionDataPackage(rawExceptionObject, stringBuilder)
                    stringBuilder.AppendLine()
                End If
            Catch ex As Exception
            End Try
        End Sub

        Public Function jsonObject(input As Object) As String
            Try
                Dim jsonEngine As New Web.Script.Serialization.JavaScriptSerializer
                Return jsonEngine.Serialize(input)
            Catch ex As Exception
                Return "{}"
            End Try
        End Function

        Public Function copyTextToWindowsClipboard(strTextToBeCopiedToClipboard As String) As Boolean
            Try
                Clipboard.SetDataObject(strTextToBeCopiedToClipboard, True, 5, 200)
                Return True
            Catch ex As Exception
                MsgBox("Unable to open Windows Clipboard to copy text to it.", MsgBoxStyle.Critical, "Restore Point Creator")
                Return False
            End Try
        End Function

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
                registryKey.Dispose()
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
            setValueInRegistry(registryKey, strValueNameToSetInRegistry, boolValueToSet, boolCloseAfterSettingValue)
        End Sub

        ''' <summary>Gets a setting from the application's Registry key.</summary>
        ''' <param name="registryObject">This is Registry Key Object that will be used in this function to pull the Registry value from.</param>
        ''' <param name="valueToGetFromRegistry">The name of the Registry Value we will be pulling from.</param>
        ''' <param name="boolDefaultValue">If the Registry Value isn't found or the value is malformed, this will be the Boolean value that this function will return.</param>
        ''' <returns>A Boolean value.</returns>
        Public Function getBooleanValueFromRegistry(ByRef registryObject As RegistryKey, ByVal valueToGetFromRegistry As String, ByVal boolDefaultValue As Boolean) As Boolean
            Try
                Dim boolTemp As Boolean, strDefaultValue As String

                If boolDefaultValue = True Then
                    strDefaultValue = globalVariables.booleans.strTrue
                Else
                    strDefaultValue = globalVariables.booleans.strFalse
                End If

                If Boolean.TryParse(registryObject.GetValue(valueToGetFromRegistry, strDefaultValue), boolTemp) = False Then
                    boolTemp = boolDefaultValue
                End If

                Return boolTemp
            Catch ex As Exception
                Return boolDefaultValue
            End Try
        End Function

        ''' <summary>Checks to see if a Process ID or PID exists on the system.</summary>
        ''' <param name="PID">The PID of the process you are checking the existance of.</param>
        ''' <param name="processObject">If the PID does exist, the function writes back to this argument in a ByRef way a Process Object that can be interacted with outside of this function.</param>
        ''' <returns>Return a Boolean value. If the PID exists, it return a True value. If the PID doesn't exist, it returns a False value.</returns>
        Public Function doesProcessIDExist(ByVal PID As Integer, ByRef processObject As Process) As Boolean
            Try
                processObject = Process.GetProcessById(PID)
                Return True
            Catch ex As Exception
                Return False
            End Try
        End Function

        Function getProcesses() As Dictionary(Of Integer, String)
            Dim processesOnSystem As New Dictionary(Of Integer, String)
            Dim exePath As String

            For Each process As Process In Process.GetProcesses()
                exePath = getProcessExecutablePath(process.Id)
                If exePath IsNot Nothing Then processesOnSystem.Add(process.Id, exePath)
            Next

            Return processesOnSystem
        End Function

        Function getProcessExecutablePath(processID As Integer) As String
            Dim memoryBuffer = New Text.StringBuilder(1024)
            Dim processHandle As IntPtr = APIs.OpenProcess(APIs.ProcessAccessFlags.PROCESS_QUERY_LIMITED_INFORMATION, False, processID)

            If processHandle <> IntPtr.Zero Then
                Try
                    Dim memoryBufferSize As Integer = memoryBuffer.Capacity

                    If APIs.QueryFullProcessImageName(processHandle, 0, memoryBuffer, memoryBufferSize) Then
                        Return memoryBuffer.ToString()
                    End If
                Finally
                    APIs.CloseHandle(processHandle)
                End Try
            End If

            Return Nothing
        End Function

        Public Sub searchForProcessAndKillIt(strFileName As String, boolFullFilePathPassed As Boolean)
            Dim processExecutablePath As String
            Dim processExecutablePathFileInfo As IO.FileInfo

            For Each process As Process In Process.GetProcesses()
                processExecutablePath = getProcessExecutablePath(process.Id)

                If processExecutablePath IsNot Nothing Then
                    Try
                        processExecutablePathFileInfo = New IO.FileInfo(processExecutablePath)

                        If boolFullFilePathPassed = True Then
                            If stringCompare(strFileName, processExecutablePathFileInfo.FullName) = True Then
                                killProcess(process.Id, True)
                            End If
                        ElseIf boolFullFilePathPassed = False Then
                            If stringCompare(strFileName, processExecutablePathFileInfo.Name) = True Then
                                killProcess(process.Id, True)
                            End If
                        End If

                        processExecutablePathFileInfo = Nothing
                    Catch ex As ArgumentException
                    End Try
                End If

                processExecutablePath = Nothing
            Next
        End Sub

        ''' <summary>Extracts a file from a ZIP file.</summary>
        ''' <param name="zipFileObject">This is a ICSharpCode.SharpZipLib.Zip.ZipFile Object referencing the ZIP file that you want to extract data from. To use this create a new ZipFile Object using Dim zipFileObject As New ZipFile("C:\Path\To\Your\Zip\File.zip") or by creating a new ZipFile Object by passing it an IO.MemoryStream() and then pass that Object to this function.</param>
        ''' <param name="fileToExtract">The name of the file you want to extract from the ZIP file.</param>
        ''' <param name="extractionTarget">The path to which you want to extract the file to.</param>
        ''' <returns>A Boolean Value. If the function was able to sucessfully extract the requested file from the ZIP file, it will return a True value. However, if for whatever reason something goes wrong anywhere in this function, it will return a False value.</returns>
        Public Function extractUpdatedFileFromZIPPackage(ByRef zipFileObject As ZipFile, ByVal fileToExtract As String, ByVal extractionTarget As String, Optional boolDeleteTargetIfExists As Boolean = True) As Boolean
            Try
                Dim extractionTargetFileInfo As New IO.FileInfo(extractionTarget)

                If globalVariables.boolExtendedLoggingDuringUpdating = True Then
                    eventLogFunctions.writeToSystemEventLog(String.Format("Beginning extraction of {0}{1}{0} to {0}{2}{0}.", Chr(34), fileToExtract, extractionTargetFileInfo.Name), EventLogEntryType.Information)
                End If

                ' This gets the ZipEntry Object for the file we are trying to extract from the ZIP file.
                Dim zipFileEntryObject As ZipEntry = zipFileObject.GetEntry(fileToExtract)

                ' This checks to see if the file that we're trying to extract from the ZIP file exists in the ZIP file.
                If zipFileEntryObject Is Nothing Then
                    eventLogFunctions.writeToSystemEventLog(String.Format("Unable to find {0}{1}{0} to extract from ZIP file.", Chr(34), fileToExtract), EventLogEntryType.Error)
                    Return False ' Nope, the file doesn't exist in the ZIP file so we exit out of the routine by returning a False value for the function.
                Else
                    ' This checks to see if the file we are trying to extract to from the ZIP file exists or not.
                    If IO.File.Exists(extractionTarget) = True Then
                        If globalVariables.boolExtendedLoggingDuringUpdating = True Then
                            eventLogFunctions.writeToSystemEventLog(String.Format("The file named {0}{1}{0} already exists, attempting to delete it.", Chr(34), fileToExtract), EventLogEntryType.Information)
                        End If
                        ' OK, so it does exist, let's do something about it.

                        ' This checks to see if the programmer wants to delete the file if it exists in this function.
                        If boolDeleteTargetIfExists = True Then
                            If stringCompare(extractionTargetFileInfo.Extension, ".exe") Then
                                ' If the file is an EXE file, let's try and kill any parent processes first.
                                If globalVariables.boolExtendedLoggingDuringUpdating = True Then
                                    eventLogFunctions.writeToSystemEventLog(String.Format("Since this file is an EXE file we need to check for any processes that have this file as the parent executable file.{2}{2}Killing any possible processes that have a parent executable of {0}{1}{0}.", Chr(34), extractionTargetFileInfo.FullName, vbCrLf), EventLogEntryType.Information)
                                End If

                                searchForProcessAndKillIt(extractionTargetFileInfo.FullName, True)
                            End If

                            Try
                                IO.File.Delete(extractionTarget) ' And now let's try and delete the file.

                                If globalVariables.boolExtendedLoggingDuringUpdating = True Then
                                    eventLogFunctions.writeToSystemEventLog(String.Format("The file named {0}{1}{0} has been successfully deleted.", Chr(34), extractionTargetFileInfo.FullName), EventLogEntryType.Information)
                                End If
                            Catch ex As Exception
                                eventLogFunctions.writeToSystemEventLog(String.Format("Unable to delete {0}{1}{0}.", Chr(34), extractionTargetFileInfo.Name), EventLogEntryType.Error)
                                ' Something went wrong while trying to delete the file so we return a False value for the function.
                                Return False
                            End Try
                        Else
                            ' The programmer doesn't want the file deleted if it existed so we return a False value for the function.
                            Return False
                        End If
                    Else
                        If globalVariables.boolExtendedLoggingDuringUpdating = True Then
                            eventLogFunctions.writeToSystemEventLog(String.Format("The file named {0}{1}{0} does not exist, this is a good thing; we can continue with the update process.", Chr(34), extractionTargetFileInfo.Name), EventLogEntryType.Information)
                        End If
                    End If

                    If globalVariables.boolExtendedLoggingDuringUpdating = True Then
                        eventLogFunctions.writeToSystemEventLog(String.Format("Creating new IO.FileStream to write {0}{1}{0} as {0}{2}{0} to disk.", Chr(34), fileToExtract, extractionTargetFileInfo.Name), EventLogEntryType.Information)
                    End If

                    ' Create a new FileStream Object to write out our extracted file.
                    Dim fileStream As New IO.FileStream(extractionTarget, IO.FileMode.Create)

                    If globalVariables.boolExtendedLoggingDuringUpdating = True Then
                        eventLogFunctions.writeToSystemEventLog(String.Format("IO.FileStream created successfully. Commencing the process of writing file {0}{1}{0} as {0}{2}{0} to disk.", Chr(34), fileToExtract, extractionTargetFileInfo.Name), EventLogEntryType.Information)
                    End If

                    ' This copies the data out of the ZIP File Data Stream to our FileStream Object that was created above.
                    zipFileObject.GetInputStream(zipFileEntryObject).CopyTo(fileStream)

                    If globalVariables.boolExtendedLoggingDuringUpdating = True Then
                        eventLogFunctions.writeToSystemEventLog("File write operation complete. Closing out file and disposing of the IO.FileStream.", EventLogEntryType.Information)
                    End If

                    fileStream.Close() ' This closes our FileStream Object.
                    fileStream.Dispose() ' And this Disposes of our FileStream Object.

                    If globalVariables.boolExtendedLoggingDuringUpdating = True Then
                        eventLogFunctions.writeToSystemEventLog(String.Format("Extraction of {0}{1}{0} was successful.", Chr(34), fileToExtract), EventLogEntryType.Information)
                    End If

                    extractionTargetFileInfo = Nothing

                    If globalVariables.boolExtendedLoggingDuringUpdating = True Then
                        eventLogFunctions.writeToSystemEventLog("Returning a True value for the extractUpdatedFileFromZIPPackage() function.", EventLogEntryType.Information)
                    End If

                    Return True ' Yes, the file did exist in the ZIP file and we were able to successfully extract it so we return a True value for the function.
                End If
            Catch ex As Exception
                ' Something went wrong so we write out our crash stack trace data to the application event log.
                eventLogFunctions.writeCrashToEventLog(ex)
                eventLogFunctions.writeToSystemEventLog("Returning a False value for the extractUpdatedFileFromZIPPackage() function.", EventLogEntryType.Error)
                Return False ' And we return a False value for the function.
            End Try
        End Function

        ''' <summary>Delete a file with no Exception. It first checks to see if the file exists and if it does it attempts to delete it. If it fails to do so then it will fail silently without an Exception.</summary>
        Public Sub deleteFileWithNoException(ByVal pathToFile As String)
            Try
                If IO.File.Exists(pathToFile) Then IO.File.Delete(pathToFile)
            Catch ex As Exception
            End Try
        End Sub

        ''' <summary>ZIPs the exported log file.</summary>
        ''' <param name="pathToZIPFile">The path the ZIP file we will be working with.</param>
        ''' <param name="fileToAddToZIPFile">The path to the file we will be adding to the ZIP file.</param>
        ''' <returns>Returns a Boolean value.</returns>
        Public Function addFileToZipFile(pathToZIPFile As String, fileToAddToZIPFile As String) As Boolean
            Try
                Dim zipFileObject As ZipFile ' Creates a ZIPFile Object.

                ' This checks to see if the ZIP file we want to work with already exists.
                If IO.File.Exists(pathToZIPFile) = True Then
                    ' OK, the ZIP file already exists so we create a new ZIPFile Object by opening the existing ZIP file.
                    zipFileObject = New ZipFile(pathToZIPFile)
                Else
                    ' No, the ZIP file doesn't exist so we tells the ZIPFile Library to create a new ZIPFile Object with no previous file.
                    zipFileObject = ZipFile.Create(pathToZIPFile)
                End If

                zipFileObject.BeginUpdate() ' We need to open the ZIP file for writing.
                zipFileObject.Add(fileToAddToZIPFile, New IO.FileInfo(fileToAddToZIPFile).Name) ' Adds the file to the ZIP file.
                zipFileObject.CommitUpdate() ' Commits the added file to the ZIP file.
                zipFileObject.Close() ' Closes the ZIPFile Object.

                Return True
            Catch ex As Exception
                Return False
            End Try
        End Function

        <Runtime.InteropServices.DllImport("Srclient.dll")> Public Function SRRemoveRestorePoint(index As Integer) As Integer
        End Function

        ''' <summary>Converts the number of Bytes to a nice way of saying it, like MBs, GBs, etc.</summary>
        ''' <param name="size">The amount of data in Bytes.</param>
        ''' <param name="roundToNearestWholeNumber">Tells the function if it should round the number to the nearest whole number.</param>
        ''' <returns>A String value such as 100 MBs or 100 GBs.</returns>
        Public Function bytesToHumanSize(ByVal size As ULong, Optional roundToNearestWholeNumber As Boolean = False) As String
            Dim result As String

            If size <= (2 ^ 10) Then
                result = size & " Bytes"
            ElseIf size > (2 ^ 10) And size <= (2 ^ 20) Then
                If roundToNearestWholeNumber = True Then
                    result = Math.Round(size / (2 ^ 10), 0) & " KBs"
                Else
                    result = Math.Round(size / (2 ^ 10), 2) & " KBs"
                End If
            ElseIf size > (2 ^ 20) And size <= (2 ^ 30) Then
                If roundToNearestWholeNumber = True Then
                    result = Math.Round(size / (2 ^ 20), 0) & " MBs"
                Else
                    result = Math.Round(size / (2 ^ 20), 2) & " MBs"
                End If
            ElseIf size > (2 ^ 30) And size <= (2 ^ 40) Then
                If roundToNearestWholeNumber = True Then
                    result = Math.Round(size / (2 ^ 30), 0) & " GBs"
                Else
                    result = Math.Round(size / (2 ^ 30), 2) & " GBs"
                End If
            ElseIf size > (2 ^ 40) And size <= (2 ^ 50) Then
                If roundToNearestWholeNumber = True Then
                    result = Math.Round(size / (2 ^ 40), 0) & " TBs"
                Else
                    result = Math.Round(size / (2 ^ 40), 2) & " TBs"
                End If
            ElseIf size > (2 ^ 50) And size <= (2 ^ 60) Then
                If roundToNearestWholeNumber = True Then
                    result = Math.Round(size / (2 ^ 50), 0) & " PBs"
                Else
                    result = Math.Round(size / (2 ^ 50), 2) & " PBs"
                End If
            Else
                result = "(None)"
            End If

            Return result
        End Function

        ''' <summary>Executes a command at the command line and returns the output in the commandLineOutput ByRef variable.</summary>
        ''' <param name="commandLineOutput">The output of the command that was ran by this command. This is a ByRef variable.</param>
        ''' <param name="commandToExecute">The command to be run by this function.</param>
        ''' <param name="commandLineArgument">The arguments you want passed to the command to be run by this function.</param>
        ''' <returns>A Boolean value. If True the command was successful, if it returns a False value the command failed.</returns>
        Public Function executeShellCommandAndGetOutput(ByRef commandLineOutput As String, ByVal commandToExecute As String, Optional ByVal commandLineArgument As String = Nothing) As Boolean
            Try
                Dim processObject As New Process()
                processObject.StartInfo.FileName = commandToExecute

                If commandLineArgument <> Nothing Then
                    processObject.StartInfo.Arguments = commandLineArgument
                End If

                processObject.StartInfo.CreateNoWindow = True
                processObject.StartInfo.UseShellExecute = False
                processObject.StartInfo.RedirectStandardOutput = True
                processObject.Start()

                commandLineOutput = processObject.StandardOutput.ReadToEnd()
                Return True
            Catch ex As Exception
                Return False
            End Try
        End Function

        Private Function tryToParseShort(value As String) As Boolean
            Dim number As Short
            Dim result As Boolean = Short.TryParse(value, number)

            If result Then
                Return True
            Else
                Return False
            End If
        End Function

        ''' <summary>Tests to see if the RegEx pattern is valid or not.</summary>
        ''' <param name="strPattern">A RegEx pattern.</param>
        ''' <returns>A Boolean value. If True, the RegEx pattern is valid. If False, the RegEx pattern is not valid.</returns>
        Public Function boolTestRegExPattern(strPattern As String) As Boolean
            Try
                Dim testRegExPattern As New Regex(strPattern)
                Return True
            Catch ex As Exception
                Return False
            End Try
        End Function

        Public Sub setSafeModeBoot()
            Try
                Dim bcdEditor As New BCD.bcdEditor()
                bcdEditor.setSafeModeBootFlag()
                bcdEditor.dispose()
                bcdEditor = Nothing

                eventLogFunctions.writeToSystemEventLog("Successfully set Safe Mode Boot flag.", EventLogEntryType.Information)
            Catch ex As Exception
                eventLogFunctions.writeCrashToEventLog(ex)
                eventLogFunctions.writeToSystemEventLog("Unable to set Safe Mode Boot flag.", EventLogEntryType.Error)
            End Try
        End Sub

        Public Sub removeSafeModeBoot()
            Try
                Dim bcdEditor As New BCD.bcdEditor()

                If bcdEditor.getSafeModeBootStatus() Then
                    bcdEditor.removeSafeModeBootFlag()
                    eventLogFunctions.writeToSystemEventLog("Successfully removed Safe Mode Boot flag.", EventLogEntryType.Information)
                End If

                bcdEditor.dispose()
                bcdEditor = Nothing
            Catch ex As Exception
                eventLogFunctions.writeCrashToEventLog(ex)
                eventLogFunctions.writeToSystemEventLog("Unable to remove Safe Mode Boot flag.", EventLogEntryType.Error)
            End Try
        End Sub

        Public Sub executeCommand(pathToExecutable As String, arguments As String, Optional runAsAdmin As Boolean = False)
            If IO.File.Exists(pathToExecutable) = True Then
                Dim processStartInfo As New ProcessStartInfo() With {
                    .FileName = pathToExecutable,
                    .WindowStyle = ProcessWindowStyle.Hidden,
                    .Arguments = arguments,
                    .CreateNoWindow = True
                }

                If runAsAdmin = True Then processStartInfo.Verb = "runas"

                Process.Start(processStartInfo)
            End If
        End Sub

        Public Sub executeCommand(pathToExecutable As String, Optional runAsAdmin As Boolean = False)
            If IO.File.Exists(pathToExecutable) = True Then
                Dim processStartInfo As New ProcessStartInfo() With {
                    .FileName = pathToExecutable,
                    .WindowStyle = ProcessWindowStyle.Hidden,
                    .CreateNoWindow = True
                }

                If runAsAdmin = True Then processStartInfo.Verb = "runas"

                Process.Start(processStartInfo)
            End If
        End Sub

        Public Sub executeCommandWithWait(pathToExecutable As String, arguments As String, Optional runAsAdmin As Boolean = False)
            If IO.File.Exists(pathToExecutable) = True Then
                Dim processStartInfo As New ProcessStartInfo() With {
                    .FileName = pathToExecutable,
                    .WindowStyle = ProcessWindowStyle.Hidden,
                    .Arguments = arguments,
                    .CreateNoWindow = True
                }

                If runAsAdmin = True Then processStartInfo.Verb = "runas"

                Process.Start(processStartInfo).WaitForExit()
            End If
        End Sub

        Public Sub executeCommandWithWait(pathToExecutable As String, Optional runAsAdmin As Boolean = False)
            If IO.File.Exists(pathToExecutable) = True Then
                Dim processStartInfo As New ProcessStartInfo() With {
                    .FileName = pathToExecutable,
                    .WindowStyle = ProcessWindowStyle.Hidden,
                    .CreateNoWindow = True
                }

                If runAsAdmin = True Then processStartInfo.Verb = "runas"

                Process.Start(processStartInfo).WaitForExit()
            End If
        End Sub

        Public Sub rebootSystem()
            Dim strPathToShutDown As String = IO.Path.Combine(globalVariables.strPathToSystemFolder, "shutdown.exe")

            If IO.File.Exists(strPathToShutDown) = True Then
                executeCommand(strPathToShutDown, "-r -t 0", True)
            Else
                MsgBox("Unable to find the Windows command line reboot tool to trigger a reboot. You will have to manually trigger a reboot yourself.", MsgBoxStyle.Exclamation, "Restore Point Creator")
            End If
        End Sub

        Public Sub createScheduledSystemRestorePoint(strRestorePointDescription As String)
            Try
                Dim integerNewRestorePointID As Integer
                wmi.createRestorePoint(strRestorePointDescription, RestoreType.WindowsType, integerNewRestorePointID)
            Catch ex As Exception
                eventLogFunctions.writeCrashToEventLog(ex)
                Process.GetCurrentProcess.Kill()
            End Try
        End Sub

        Public Sub writeSystemRestorePointsToApplicationLogs()
            Dim restorePoints As New Text.StringBuilder
            Dim systemRestorePointsManagementObjectSearcher As New Management.ManagementObjectSearcher("root\DEFAULT", "SELECT * FROM SystemRestore")
            Dim restorePointCreationDate As Date

            If systemRestorePointsManagementObjectSearcher IsNot Nothing Then
                Dim restorePointsOnSystemManagementObjectCollection As Management.ManagementObjectCollection = systemRestorePointsManagementObjectSearcher.Get()

                If restorePointsOnSystemManagementObjectCollection IsNot Nothing Then
                    If (restorePointsOnSystemManagementObjectCollection.Count = 0) = False Then
                        restorePoints.AppendLine("Number of Restore Points: " & restorePointsOnSystemManagementObjectCollection.Count)
                        restorePoints.AppendLine("=========================")
                        For Each restorePointDetails As Management.ManagementObject In restorePointsOnSystemManagementObjectCollection
                            If (restorePointDetails("SequenceNumber") IsNot Nothing) And (restorePointDetails("CreationTime") IsNot Nothing) And (restorePointDetails("Description") IsNot Nothing) And (restorePointDetails("RestorePointType") IsNot Nothing) Then
                                restorePoints.Append(restorePointDetails("SequenceNumber").ToString & " | " & restorePointDetails("Description").ToString & " | ")

                                If String.IsNullOrEmpty(restorePointDetails("CreationTime").ToString.Trim) Then
                                    restorePoints.Append("Error Parsing Date")
                                Else
                                    restorePointCreationDate = parseSystemRestorePointCreationDate(restorePointDetails("CreationTime").ToString)
                                    restorePoints.Append(String.Format("{0} {1}", restorePointCreationDate.ToShortDateString, restorePointCreationDate.ToLongTimeString))
                                End If

                                restorePoints.Append(" | " & whatTypeOfRestorePointIsIt(Integer.Parse(restorePointDetails("RestorePointType").ToString)))
                                restorePoints.AppendLine()
                            End If
                        Next
                    End If
                End If
            End If

            systemRestorePointsManagementObjectSearcher.Dispose()
            systemRestorePointsManagementObjectSearcher = Nothing

            restorePoints.AppendLine("=========================")
            eventLogFunctions.writeToSystemEventLog(restorePoints.ToString.Trim, EventLogEntryType.Information)
        End Sub

        ''' <summary>Converts an Integer representing days to English. Like "1 day" or "2 days".</summary>
        ''' <param name="input">Days.</param>
        ''' <returns>A String.</returns>
        Private Function daysToString(input As Integer) As String
            If input = 1 Then
                Return "1 day"
            Else
                Return input & " days"
            End If
        End Function

        ''' <summary>Creates a Window Shortcut.</summary>
        ''' <param name="locationOfShortcut">The location where we want to create the Windows shortcut.</param>
        ''' <param name="pathToExecutable">The path to the executable that the shortcut will launch.</param>
        ''' <param name="iconPath">The path to the icon that the new shortcut will use.</param>
        ''' <param name="Title">The name of the new shortcut.</param>
        ''' <param name="arguments">Any and all command line arguments that the invoked executable will be launched with.</param>
        Public Sub createShortcut(ByVal locationOfShortcut As String, pathToExecutable As String, iconPath As String, ByVal Title As String, Optional arguments As String = Nothing)
            Try
                Dim WshShell As New IWshRuntimeLibrary.WshShell
                ' short cut files have a .lnk extension
                Dim shortCut As IWshRuntimeLibrary.IWshShortcut = DirectCast(WshShell.CreateShortcut(locationOfShortcut), IWshRuntimeLibrary.IWshShortcut)

                ' set the shortcut properties
                With shortCut
                    .TargetPath = pathToExecutable

                    If (arguments = Nothing) = False Then
                        .Arguments = arguments
                    End If

                    .WindowStyle = 1I
                    .Description = Title
                    ' the next line gets the first Icon from the executing program
                    .IconLocation = iconPath & ", 0"
                    .Save() ' save the shortcut file
                End With
            Catch ex As Exception
            End Try
        End Sub

        Public Sub launchRuntimeTaskFixRoutine()
            Try
                Dim startInfo As New ProcessStartInfo With {
                    .FileName = New IO.FileInfo(Application.ExecutablePath).FullName,
                    .Arguments = "-fixruntimetasks"
                }

                If privilegeChecks.areWeAnAdministrator() = False Then startInfo.Verb = "runas"

                Process.Start(startInfo)
                Process.GetCurrentProcess.Kill()
            Catch ex As ComponentModel.Win32Exception
                eventLogFunctions.writeCrashToEventLog(ex)
                MsgBox("There was an error while attempting to elevate the process, please make sure that when the Windows UAC prompt appears asking you to run the program with elevated privileges that you say ""Yes"" to the UAC prompt." & vbCrLf & vbCrLf & "The program will now terminate.", MsgBoxStyle.Critical, globalVariables.programName)
                Process.GetCurrentProcess.Kill()
            End Try
        End Sub

        Public Sub reRunWithAdminUserRights()
            Try
                Dim startInfo As New ProcessStartInfo With {
                    .FileName = New IO.FileInfo(Application.ExecutablePath).FullName
                }

                If Environment.GetCommandLineArgs.Count <> 1 Then
                    startInfo.Arguments = Environment.GetCommandLineArgs(1)
                End If

                If privilegeChecks.areWeAnAdministrator() = False Then startInfo.Verb = "runas"

                Process.Start(startInfo)
                Process.GetCurrentProcess.Kill()
            Catch ex As ComponentModel.Win32Exception
                eventLogFunctions.writeCrashToEventLog(ex)
                MsgBox("There was an error while attempting to elevate the process, please make sure that when the Windows UAC prompt appears asking you to run the program with elevated privileges that you say ""Yes"" to the UAC prompt." & vbCrLf & vbCrLf & "The program will now terminate.", MsgBoxStyle.Critical + MsgBoxStyle.SystemModal, globalVariables.programName)
                Process.GetCurrentProcess.Kill()
            End Try
        End Sub

        ''' <summary>Returns the creation date of the System Restore Point ID you pass to the function.</summary>
        ''' <param name="restorePointID">Data type, Short. The ID of the System Restore Point you want to return the creation date of.</param>
        ''' <returns>A Date Object.</returns>
        Private Function getSystemRestorePointDate(restorePointID As Short) As Date
            Try
                Dim newestSystemRestoreID As Integer = 0 ' Resets the newest System Restore ID to 0.

                ' Get all System Restore Points from the Windows Management System and puts then in the systemRestorePoints variable.
                Dim systemRestorePoints As New Management.ManagementObjectSearcher("root\DEFAULT", "SELECT * FROM SystemRestore WHERE SequenceNumber = " & restorePointID)

                If systemRestorePoints.Get().Count <> 0 Then
                    Return parseSystemRestorePointCreationDate(systemRestorePoints.Get(0)("CreationTime").ToString, True)
                Else
                    Return Nothing
                End If
            Catch ex As Exception
                Threading.Thread.CurrentThread.CurrentUICulture = New Globalization.CultureInfo("en-US")
                exceptionHandler.manuallyLoadCrashWindow(ex, ex.Message, ex.StackTrace, ex.GetType)
                Return Nothing
            End Try
        End Function

        Public Function parseSystemRestorePointCreationDate(strDate As String, Optional boolFullDateParsing As Boolean = True) As Date
            Dim regexMatches As Match = globalVariables.regexRestorePointCreationTimeParser.Match(strDate)
            Dim year, month, day, second, minute, hour As Integer

            ' Gets the values out of the Regular Expression Matches object.
            With regexMatches
                year = Integer.Parse(.Groups("year").Value)
                month = Integer.Parse(.Groups("month").Value)
                day = Integer.Parse(.Groups("day").Value)
                second = Integer.Parse(.Groups("second").Value)
                minute = Integer.Parse(.Groups("minute").Value)
                hour = Integer.Parse(.Groups("hour").Value)
            End With

            If boolFullDateParsing = True Then
                Return New DateTime(year, month, day, hour, minute, second, DateTimeKind.Utc).ToLocalTime
            Else
                Return New Date(year, month, day)
            End If
        End Function

        Public Function areWeInSafeMode() As Boolean
            If SystemInformation.BootMode = BootMode.Normal Then
                Return False
            Else
                Return True
            End If
        End Function

        Public Enum RestoreType
            ApplicationInstall = 0 ' Installing a new application
            ApplicationUninstall = 1 ' An application has been uninstalled
            ModifySettings = 12 ' An application has had features added or removed
            CancelledOperation = 13 ' An application needs to delete the restore point it created
            Restore = 6 ' System Restore
            Checkpoint = 7 ' Checkpoint
            DeviceDriverInstall = 10 ' Device driver has been installed
            FirstRun = 11 ' Program used for 1st time
            BackupRecovery = 14 ' Restoring a backup
            WindowsType = 16 ' The type of restore point that Windows makes.
        End Enum

        ''' <summary>A function that returns the type of restore point depending upon the Integer that comes from the WMI.</summary>
        ''' <param name="type">Integer.</param>
        ''' <returns>Returns a String value containing the type of Restore Point in English.</returns>
        Public Function whatTypeOfRestorePointIsIt(type As Integer) As String
            If type = RestoreType.ApplicationInstall Then
                Return "Application Install"
            ElseIf type = RestoreType.ApplicationUninstall Then
                Return "Application Removal"
            ElseIf type = RestoreType.BackupRecovery Then
                Return "Backup Recovery"
            ElseIf type = RestoreType.CancelledOperation Then
                Return "Cancelled Operation"
            ElseIf type = RestoreType.Checkpoint Then
                Return "System Checkpoint"
            ElseIf type = RestoreType.DeviceDriverInstall Then
                Return "Device Driver Install"
            ElseIf type = RestoreType.FirstRun Then
                Return "First Run"
            ElseIf type = RestoreType.ModifySettings Then
                Return "Settings Modified"
            ElseIf type = RestoreType.Restore Then
                Return "Restore"
            ElseIf type = RestoreType.WindowsType Then
                Return "System Restore Point"
            Else
                Return "Unknown Type"
            End If
        End Function

        Public Sub killProcess(processID As Integer, Optional boolLogToEventLog As Boolean = False)
            If boolLogToEventLog = True Then
                eventLogFunctions.writeToSystemEventLog("Killing process with PID of " & processID & ".", EventLogEntryType.Information)
            End If

            Dim processObject As Process = Nothing

            ' First we are going to check if the Process ID exists.
            If doesProcessIDExist(processID, processObject) = True Then
                Try
                    processObject.Kill() ' Yes, it does so let's kill it.
                Catch ex As Exception
                    ' Wow, it seems that even with double-checking if a process exists by it's PID number things can still go wrong.
                    ' So this Try-Catch block is here to trap any possible errors when trying to kill a process by it's PID number.
                End Try
            End If

            processObject = Nothing
            Threading.Thread.Sleep(250) ' We're going to sleep to give the system some time to kill the process.

            '' Now we are going to check again if the Process ID exists and if it does, we're going to attempt to kill it again.
            If doesProcessIDExist(processID, processObject) = True Then
                Try
                    processObject.Kill()
                Catch ex As Exception
                    ' Wow, it seems that even with double-checking if a process exists by it's PID number things can still go wrong.
                    ' So this Try-Catch block is here to trap any possible errors when trying to kill a process by it's PID number.
                End Try
            End If

            processObject = Nothing
            Threading.Thread.Sleep(250) ' We're going to sleep (again) to give the system some time to kill the process.
        End Sub

        Public Function calculatePercentageValue(longSmallerNumber As ULong, longLargerNumber As ULong) As Double
            Dim result As Double = Math.Round(((longSmallerNumber / longLargerNumber) * 100), 2)

            If result > 100 Then
                Return 100
            ElseIf result < 0 Then
                Return 0
            Else
                Return result
            End If
        End Function

        Public Sub launchURLInWebBrowser(url As String, Optional errorMessage As String = "An error occurred when trying the URL In your Default browser. The URL has been copied to your Windows Clipboard for you to paste into the address bar in the web browser of your choice.")
            If url.Trim.StartsWith("http", StringComparison.OrdinalIgnoreCase) = False Then
                If My.Settings.useSSL = True Then
                    url = "https://" & url
                Else
                    url = "http://" & url
                End If
            End If

            Try
                Dim associatedApplication As String = Nothing

                If registryStuff.getFileAssociation(".html", associatedApplication) = False Then
                    Process.Start(url)
                Else
                    If IO.File.Exists(associatedApplication) = True Then
                        Process.Start(associatedApplication, Chr(34) & url & Chr(34))
                    Else
                        Process.Start(url)
                    End If
                End If
            Catch ex2 As ComponentModel.Win32Exception
                eventLogFunctions.writeCrashToEventLog(ex2)
                MsgBox("There was an error attempting to launch your web browser. Perhaps rebooting your system will correct this issue.", MsgBoxStyle.Information, "Restore Point Creator")
            Catch ex As Exception
                copyTextToWindowsClipboard(url)
                MsgBox(errorMessage, MsgBoxStyle.Information, "Restore Point Creator")
            End Try
        End Sub

        Public Function removeSourceCodePathInfo(strInput As String) As String
            If strInput.regExSearch("(?:Google Drive|OneDrive)") = True Then
                Return Regex.Replace(strInput, "C:\\Users\\Tom\\(?:Google Drive|OneDrive)\\My Visual Studio Projects\\Projects\\", "", RegexOptions.IgnoreCase)
            Else
                Return strInput
            End If
        End Function

        Private Function convertToJSON(input As Object) As String
            Return (New Web.Script.Serialization.JavaScriptSerializer).Serialize(input)
        End Function

        Public Function getDateInShortForm() As String
            Dim year, month, day As String
            year = Now.Year
            month = Now.Month
            day = Now.Day

            If month.Length = 1 Then month = "0" & month
            If day.Length = 1 Then day = "0" & day

            Return year & month & day
        End Function

        Public Function convertToBase64(input As String) As String
            Return Convert.ToBase64String(Text.Encoding.UTF8.GetBytes(input))
        End Function

        Public Function convertFromBase64(input As String) As String
            Return Text.Encoding.UTF8.GetString(Convert.FromBase64String(input))
        End Function

        Public Function isNumeric(input As String) As Boolean
            Return Regex.IsMatch(input.Trim, "\A-{0,1}[0-9.]*\Z")
        End Function

        Public Function randomStringGenerator(length As Integer)
            Dim random As Random = New Random()
            Dim builder As New Text.StringBuilder()
            Dim ch As Char
            Dim legalCharacters As String = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz1234567890"

            For cntr As Integer = 0 To length
                ch = legalCharacters.Substring(random.Next(0, legalCharacters.Length), 1)
                builder.Append(ch)
            Next

            Return builder.ToString()
        End Function
    End Module
End Namespace