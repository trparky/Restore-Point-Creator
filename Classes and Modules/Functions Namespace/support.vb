﻿Imports System.Text.RegularExpressions
Imports System.Xml

Namespace Functions.support
    Public Enum updateType
        null = 0
        release = 1
        candidate = 2
        beta = 3
        buildLessThanError = 4
    End Enum

    Module support
        Public Sub enableControlsOnForm(ByRef formToWorkOn As Form)
            For Each control As Control In formToWorkOn.Controls
                If Not control.GetType.Equals(GetType(Timer)) And Not control.Name.caseInsensitiveContains("pleasewait") Then control.Enabled = True
            Next
        End Sub

        Public Sub disableControlsOnForm(ByRef formToWorkOn As Form)
            For Each control As Control In formToWorkOn.Controls
                If Not control.GetType.Equals(GetType(Timer)) And Not control.Name.caseInsensitiveContains("pleasewait") Then control.Enabled = False
            Next
        End Sub

        Public Function randomStringGenerator(length As Integer) As String
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

        Public Function getDWMGlassColor() As Color
            Try
                ' This checks to see if we are running on Windows 8 or Windows 10 since the API we're accessing
                ' here really only works correctly on those versions of Windows; not anything older than that.
                If osVersionInfo.isThisWindows8x() Or osVersionInfo.isThisWindows10() Then
                    ' Yep, we're on Windows 8 or Windows 10 so lets go ahead and use that special API.
                    Dim color As UInteger, blend As Boolean
                    NativeMethod.NativeMethods.DwmGetColorizationColor(color, blend)
                    Return ColorTranslator.FromHtml("#" & color.ToString("x"))
                Else
                    ' This is for Windows Vista and Windows 7 machines.
                    Return My.Settings.pleaseWaitBorderColor
                End If
            Catch ex As Exception
                ' This is if something goes wrong while trying to parse what Windows gave us as the window's border color.
                eventLogFunctions.writeCrashToApplicationLogFile(ex)
                Return My.Settings.pleaseWaitBorderColor
            End Try
        End Function

        Public Function loadCustomColors() As Integer()
            If Not String.IsNullOrEmpty(My.Settings.customColors3) Then
                Return (New Web.Script.Serialization.JavaScriptSerializer).Deserialize(Of Integer())(My.Settings.customColors3)
            Else
                Return Nothing
            End If
        End Function

        Public Sub saveCustomColors(input As Integer())
            My.Settings.customColors3 = (New Web.Script.Serialization.JavaScriptSerializer).Serialize(input)
        End Sub

        Public Function getGoodTextColorBasedUponBackgroundColor(input As Color) As Color
            Dim intCombinedTotal As Short = Integer.Parse(input.R.ToString) + Integer.Parse(input.G.ToString) + Integer.Parse(input.B.ToString)
            Return If((intCombinedTotal / 3) < 128, Color.White, Color.Black)
        End Function

        Public Function verifyWindowLocation(point As Point) As Point
            Return If(point.X < 0 Or point.Y < 0, New Point(0, 0), point)
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
                    If strRemoteType.Equals("release", StringComparison.OrdinalIgnoreCase) Then
                        updateType = updateType.release
                    ElseIf strRemoteType.Equals("candidate", StringComparison.OrdinalIgnoreCase) Then
                        updateType = updateType.candidate
                        strRemoteBetaRCVersion = xmlNode.SelectSingleNode("betaRCVersion").InnerText.Trim
                    ElseIf strRemoteType.Equals("beta", StringComparison.OrdinalIgnoreCase) Then
                        updateType = updateType.beta
                        strRemoteBetaRCVersion = xmlNode.SelectSingleNode("betaRCVersion").InnerText.Trim
                    End If

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
                    If My.Settings.updateChannel.Equals(globalVariables.updateChannels.beta, StringComparison.OrdinalIgnoreCase) Or My.Settings.updateChannel.Equals(globalVariables.updateChannels.tom, StringComparison.OrdinalIgnoreCase) Then
                        ' Yes, it is set to the "beta" or "tom" channel.

                        ' Now let's check to see if the user wants any betas if the current new version is a beta.
                        If strRemoteType.Equals("beta", StringComparison.OrdinalIgnoreCase) And My.Settings.onlyGiveMeRCs Then
                            updateType = updateType.beta
                            strRemoteBetaRCVersion = xmlNode.SelectSingleNode("betaRCVersion").InnerText.Trim

                            ' Nope, the user doesn't want betas versions, they only want release candidates, so we return
                            ' a False value indicating that there is no new version to download and update to.
                            Return False
                        End If

                        ' Checks to see if the remote build type is not "release" since only the "beta" and "candidate" build types have a betaRCVersion node.
                        If Not strRemoteType.Equals("release", StringComparison.OrdinalIgnoreCase) Then
                            ' Let's try and get the "betaRCVersion" from the XML document. We don't want to crash so we check to see if the object is null first.
                            If xmlNode.SelectSingleNode("betaRCVersion") IsNot Nothing Then
                                ' Get the "strRemoteBetaRCVersion" version string from the XML data.
                                strRemoteBetaRCVersion = xmlNode.SelectSingleNode("betaRCVersion").InnerText.Trim

                                ' This checks to see if the "betaRCVersion" node doesn't contain either "Public Beta" or "Release Candidate".
                                ' If it doesn't then it prepends the appropriate string based upon the value of the "type" node.
                                If (Not strRemoteBetaRCVersion.caseInsensitiveContains("Public Beta")) And (Not strRemoteBetaRCVersion.caseInsensitiveContains("Release Candidate")) Then
                                    If strRemoteType.Equals("beta", StringComparison.OrdinalIgnoreCase) Then
                                        strRemoteBetaRCVersion = "Public Beta " & strRemoteBetaRCVersion
                                    ElseIf strRemoteType.Equals("candidate", StringComparison.OrdinalIgnoreCase) Then
                                        strRemoteBetaRCVersion = "Release Candidate " & strRemoteBetaRCVersion
                                    End If
                                End If
                            End If
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
                eventLogFunctions.writeToApplicationLogFile("There was an error while parsing the XML document. The contents of the XML document are below..." & vbCrLf & vbCrLf & xmlData, EventLogEntryType.Error, False)
                eventLogFunctions.writeCrashToApplicationLogFile(ex)

                ' Something went wrong so we return a False value.
                Return False
            Catch ex As XmlException
                eventLogFunctions.writeToApplicationLogFile("There was an error while parsing the XML document. The contents of the XML document are below..." & vbCrLf & vbCrLf & xmlData, EventLogEntryType.Error, False)
                eventLogFunctions.writeCrashToApplicationLogFile(ex)

                ' Something went wrong so we return a False value.
                Return False
            Catch ex As Exception
                ' Something went wrong so we return a False value.
                Return False
            End Try
        End Function

        Public Function convertErrorCodeToHex(input As Long) As String
            Try
                Dim strHexValue As String = input.ToString("x").ToUpper.caseInsensitiveReplace("ffffffff", "0x").ToString
                If Not strHexValue.StartsWith("0x", StringComparison.OrdinalIgnoreCase) Then strHexValue = "0x" & strHexValue
                Return strHexValue
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

                Dim listExceptionTypes As New List(Of Type) From {
                    GetType(IO.FileNotFoundException),
                    GetType(IO.FileLoadException),
                    GetType(Runtime.InteropServices.COMException),
                    GetType(IO.IOException),
                    GetType(ArgumentOutOfRangeException),
                    GetType(FormatException),
                    GetType(ComponentModel.Win32Exception),
                    GetType(XPath.XPathException),
                    GetType(XmlException),
                    GetType(InvalidOperationException),
                    GetType(myExceptions.integerTryParseException),
                    GetType(IO.DirectoryNotFoundException),
                    GetType(Management.ManagementException),
                    GetType(ObjectDisposedException),
                    GetType(StackOverflowException),
                    GetType(PlatformNotSupportedException),
                    GetType(NotSupportedException),
                    GetType(UnauthorizedAccessException),
                    GetType(myExceptions.unableToGetLockOnLogFile)
                }

                If listExceptionTypes.Contains(exceptionType) Then
                    stringBuilder.AppendLine()
                    stringBuilder.AppendLine("Additional " & rawExceptionObject.GetType.ToString & " Data")

                    If exceptionType.Equals(GetType(IO.FileNotFoundException)) Then
                        Dim FileNotFoundExceptionObject As IO.FileNotFoundException = DirectCast(rawExceptionObject, IO.FileNotFoundException)
                        stringBuilder.AppendLine("Name of File: " & FileNotFoundExceptionObject.FileName)

                        If Not String.IsNullOrEmpty(FileNotFoundExceptionObject.FusionLog) Then
                            stringBuilder.AppendLine("Reason: " & FileNotFoundExceptionObject.FusionLog)
                        End If
                    ElseIf exceptionType.Equals(GetType(UnauthorizedAccessException)) Then
                        Dim intErrorCode As Integer = DirectCast(rawExceptionObject, UnauthorizedAccessException).HResult
                        stringBuilder.AppendLine(String.Format("Error Code: {0} ({1})", intErrorCode, convertErrorCodeToHex(intErrorCode)))
                    ElseIf exceptionType.Equals(GetType(PlatformNotSupportedException)) Then
                        stringBuilder.AppendLine("Source: " & DirectCast(rawExceptionObject, PlatformNotSupportedException).Source)
                    ElseIf exceptionType.Equals(GetType(NotSupportedException)) Then
                        stringBuilder.AppendLine("Source: " & DirectCast(rawExceptionObject, NotSupportedException).Source)
                    ElseIf exceptionType.Equals(GetType(Management.ManagementException)) Then
                        Dim ManagementExceptionObject As Management.ManagementException = DirectCast(rawExceptionObject, Management.ManagementException)
                        stringBuilder.AppendLine("Source: " & ManagementExceptionObject.Source)

                        Try
                            Dim intErrorCode As Integer = ManagementExceptionObject.ErrorCode
                            stringBuilder.AppendLine(String.Format("Error Code: {0} (0x{1})", intErrorCode.ToString(), intErrorCode.ToString("X")))
                        Catch ex As Exception
                        End Try

                        addJSONedExtendedExceptionDataPackage(ManagementExceptionObject, stringBuilder)
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
                        stringBuilder.AppendLine("Error Code: " & convertErrorCodeToHex(Runtime.InteropServices.Marshal.GetExceptionCode()))

                        If Not String.IsNullOrEmpty(FileLoadExceptionObject.FusionLog) Then
                            stringBuilder.AppendLine("Reason why assembly couldn't be loaded: " & FileLoadExceptionObject.FusionLog)
                        End If
                    ElseIf exceptionType.Equals(GetType(Runtime.InteropServices.COMException)) Then
                        Dim COMExceptionObject As Runtime.InteropServices.COMException = DirectCast(rawExceptionObject, Runtime.InteropServices.COMException)
                        stringBuilder.AppendLine("Source: " & COMExceptionObject.Source)
                        stringBuilder.AppendLine("Error Code: " & convertErrorCodeToHex(COMExceptionObject.ErrorCode))
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
                    ElseIf exceptionType.Equals(GetType(StackOverflowException)) Then
                        Dim StackOverflowExceptionObject As StackOverflowException = DirectCast(rawExceptionObject, StackOverflowException)
                        stringBuilder.AppendLine(String.Format("Source: {0}", StackOverflowExceptionObject.Source))
                        addJSONedExtendedExceptionDataPackage(StackOverflowExceptionObject, stringBuilder)
                    ElseIf exceptionType.Equals(GetType(myExceptions.unableToGetLockOnLogFile)) Then
                        stringBuilder.AppendLine("Mutex Last Acquired Where: " & If(String.IsNullOrWhiteSpace(eventLogFunctions.strMutexAcquiredWhere), "Null", eventLogFunctions.strMutexAcquiredWhere))
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

            For Each process As Process In process.GetProcesses()
                exePath = getProcessExecutablePath(process.Id)
                If exePath IsNot Nothing Then processesOnSystem.Add(process.Id, exePath)
            Next

            Return processesOnSystem
        End Function

        Function getProcessExecutablePath(processID As Integer) As String
            Dim memoryBuffer = New Text.StringBuilder(1024)
            Dim processHandle As IntPtr = APIs.NativeMethods.OpenProcess(APIs.ProcessAccessFlags.PROCESS_QUERY_LIMITED_INFORMATION, False, processID)

            If processHandle <> IntPtr.Zero Then
                Try
                    Dim memoryBufferSize As Integer = memoryBuffer.Capacity

                    If APIs.NativeMethods.QueryFullProcessImageName(processHandle, 0, memoryBuffer, memoryBufferSize) Then
                        Return memoryBuffer.ToString()
                    End If
                Finally
                    APIs.NativeMethods.CloseHandle(processHandle)
                End Try
            End If

            APIs.NativeMethods.CloseHandle(processHandle)
            Return Nothing
        End Function

        Public Sub searchForProcessAndKillIt(strFileName As String, boolFullFilePathPassed As Boolean)
            Dim processExecutablePath As String
            Dim processExecutablePathFileInfo As IO.FileInfo

            For Each process As Process In process.GetProcesses()
                processExecutablePath = getProcessExecutablePath(process.Id)

                If processExecutablePath IsNot Nothing Then
                    Try
                        processExecutablePathFileInfo = New IO.FileInfo(processExecutablePath)

                        If boolFullFilePathPassed = True Then
                            If strFileName.Equals(processExecutablePathFileInfo.FullName, StringComparison.OrdinalIgnoreCase) = True Then
                                killProcess(process.Id, True)
                            End If
                        ElseIf boolFullFilePathPassed = False Then
                            If strFileName.Equals(processExecutablePathFileInfo.Name, StringComparison.OrdinalIgnoreCase) = True Then
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
        Public Function extractUpdatedFileFromZIPPackage(ByRef zipFileObject As IO.Compression.ZipArchive, ByVal fileToExtract As String, ByVal extractionTarget As String, Optional boolDeleteTargetIfExists As Boolean = True) As Boolean
            Try
                Dim extractionTargetFileInfo As New IO.FileInfo(extractionTarget)

                If globalVariables.boolExtendedLoggingDuringUpdating = True Then
                    eventLogFunctions.writeToApplicationLogFile(String.Format("Beginning extraction of {0}{1}{0} to {0}{2}{0}.", Chr(34), fileToExtract, extractionTargetFileInfo.Name), EventLogEntryType.Information, False)
                End If

                ' This gets the ZipEntry Object for the file we are trying to extract from the ZIP file.
                Dim zipFileEntryObject As IO.Compression.ZipArchiveEntry = zipFileObject.GetEntry(fileToExtract)

                ' This checks to see if the file that we're trying to extract from the ZIP file exists in the ZIP file.
                If zipFileEntryObject Is Nothing Then
                    eventLogFunctions.writeToApplicationLogFile(String.Format("Unable to find {0}{1}{0} to extract from ZIP file.", Chr(34), fileToExtract), EventLogEntryType.Error, False)
                    Return False ' Nope, the file doesn't exist in the ZIP file so we exit out of the routine by returning a False value for the function.
                Else
                    ' This checks to see if the file we are trying to extract to from the ZIP file exists or not.
                    If IO.File.Exists(extractionTarget) = True Then
                        If globalVariables.boolExtendedLoggingDuringUpdating = True Then
                            eventLogFunctions.writeToApplicationLogFile(String.Format("The file named {0}{1}{0} already exists, attempting to delete it.", Chr(34), fileToExtract), EventLogEntryType.Information, False)
                        End If
                        ' OK, so it does exist, let's do something about it.

                        ' This checks to see if the programmer wants to delete the file if it exists in this function.
                        If boolDeleteTargetIfExists = True Then
                            If extractionTargetFileInfo.Extension.Equals(".exe", StringComparison.OrdinalIgnoreCase) Then
                                ' If the file is an EXE file, let's try and kill any parent processes first.
                                If globalVariables.boolExtendedLoggingDuringUpdating = True Then
                                    eventLogFunctions.writeToApplicationLogFile(String.Format("Since this file is an EXE file we need to check for any processes that have this file as the parent executable file.{2}{2}Killing any possible processes that have a parent executable of {0}{1}{0}.", Chr(34), extractionTargetFileInfo.FullName, vbCrLf), EventLogEntryType.Information, False)
                                End If

                                searchForProcessAndKillIt(extractionTargetFileInfo.FullName, True)
                            End If

                            Try
                                IO.File.Delete(extractionTarget) ' And now let's try and delete the file.

                                If globalVariables.boolExtendedLoggingDuringUpdating = True Then
                                    eventLogFunctions.writeToApplicationLogFile(String.Format("The file named {0}{1}{0} has been successfully deleted.", Chr(34), extractionTargetFileInfo.FullName), EventLogEntryType.Information, False)
                                End If
                            Catch ex As Exception
                                eventLogFunctions.writeToApplicationLogFile(String.Format("Unable to delete {0}{1}{0}.", Chr(34), extractionTargetFileInfo.Name), EventLogEntryType.Error, False)
                                ' Something went wrong while trying to delete the file so we return a False value for the function.
                                Return False
                            End Try
                        Else
                            ' The programmer doesn't want the file deleted if it existed so we return a False value for the function.
                            Return False
                        End If
                    Else
                        If globalVariables.boolExtendedLoggingDuringUpdating = True Then
                            eventLogFunctions.writeToApplicationLogFile(String.Format("The file named {0}{1}{0} does not exist, this is a good thing; we can continue with the update process.", Chr(34), extractionTargetFileInfo.Name), EventLogEntryType.Information, False)
                        End If
                    End If

                    If globalVariables.boolExtendedLoggingDuringUpdating = True Then
                        eventLogFunctions.writeToApplicationLogFile(String.Format("Creating new IO.FileStream to write {0}{1}{0} as {0}{2}{0} to disk.", Chr(34), fileToExtract, extractionTargetFileInfo.Name), EventLogEntryType.Information, False)
                    End If

                    ' Create a new FileStream Object to write out our extracted file.
                    Dim fileStream As New IO.FileStream(extractionTarget, IO.FileMode.Create)

                    If globalVariables.boolExtendedLoggingDuringUpdating = True Then
                        eventLogFunctions.writeToApplicationLogFile(String.Format("IO.FileStream created successfully. Commencing the process of writing file {0}{1}{0} as {0}{2}{0} to disk.", Chr(34), fileToExtract, extractionTargetFileInfo.Name), EventLogEntryType.Information, False)
                        eventLogFunctions.writeToApplicationLogFile("Opening IO.Stream from ZIP File Object.", EventLogEntryType.Information, False)
                    End If

                    ' This copies the data out of the ZIP File Data Stream to our FileStream Object that was created above.
                    Using zipFileEntryObjectIOStream As IO.Stream = zipFileEntryObject.Open()
                        If globalVariables.boolExtendedLoggingDuringUpdating = True Then
                            eventLogFunctions.writeToApplicationLogFile("ZIP File Object IO.Stream opened. Copying data from ZIP File Object IO.Stream to IO.FileStream.", EventLogEntryType.Information, False)
                        End If

                        zipFileEntryObjectIOStream.CopyTo(fileStream)

                        If globalVariables.boolExtendedLoggingDuringUpdating = True Then
                            eventLogFunctions.writeToApplicationLogFile("Data copying complete. Closing out ZIP File Object IO.Stream.", EventLogEntryType.Information, False)
                        End If
                    End Using

                    If globalVariables.boolExtendedLoggingDuringUpdating = True Then
                        eventLogFunctions.writeToApplicationLogFile("File write operation complete. Closing out file and disposing of the IO.FileStream.", EventLogEntryType.Information, False)
                    End If

                    fileStream.Close() ' This closes our FileStream Object.

                    If globalVariables.boolExtendedLoggingDuringUpdating = True Then
                        eventLogFunctions.writeToApplicationLogFile(String.Format("Extraction of {0}{1}{0} was successful.", Chr(34), fileToExtract), EventLogEntryType.Information, False)
                    End If

                    extractionTargetFileInfo = Nothing

                    If globalVariables.boolExtendedLoggingDuringUpdating = True Then
                        eventLogFunctions.writeToApplicationLogFile("Returning a True value for the extractUpdatedFileFromZIPPackage() function.", EventLogEntryType.Information, False)
                    End If

                    Return True ' Yes, the file did exist in the ZIP file and we were able to successfully extract it so we return a True value for the function.
                End If
            Catch ex As Exception
                ' Something went wrong so we write out our crash stack trace data to the application event log.
                eventLogFunctions.writeCrashToApplicationLogFile(ex)
                eventLogFunctions.writeToApplicationLogFile("Returning a False value for the extractUpdatedFileFromZIPPackage() function.", EventLogEntryType.Error, False)
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

        ''' <summary>Adds a file to the chosen ZIP file.</summary>
        ''' <param name="zipFileObject">A IO.Compression.ZipArchive Object.</param>
        ''' <param name="strFileToBeAdded">The path to the file we will be adding to the ZIP file.</param>
        ''' <returns>Returns a Boolean value.</returns>
        Public Function addFileToZipFile(ByRef zipFileObject As IO.Compression.ZipArchive, strFileToBeAdded As String) As Boolean
            Try
                If IO.File.Exists(strFileToBeAdded) Then
                    Dim newZipFileEntryObject As IO.Compression.ZipArchiveEntry = zipFileObject.CreateEntry(New IO.FileInfo(strFileToBeAdded).Name, IO.Compression.CompressionLevel.Optimal)

                    Using localFileStreamReader As New IO.FileStream(strFileToBeAdded, IO.FileMode.Open)
                        Using zipFileEntryIOStream As IO.Stream = newZipFileEntryObject.Open()
                            localFileStreamReader.CopyTo(zipFileEntryIOStream)
                        End Using
                    End Using
                End If

                Return True
            Catch ex As Exception
                eventLogFunctions.writeCrashToApplicationLogFile(ex)
                Return False
            End Try
        End Function

        ''' <summary>Adds a file to the chosen ZIP file.</summary>
        ''' <param name="pathToZIPFile">The path the ZIP file we will be working with.</param>
        ''' <param name="strFileToBeAdded">The path to the file we will be adding to the ZIP file.</param>
        ''' <returns>Returns a Boolean value.</returns>
        Public Function addFileToZipFile(pathToZIPFile As String, strFileToBeAdded As String) As Boolean
            Try
                Dim zipArchiveMode As IO.Compression.ZipArchiveMode = If(IO.File.Exists(pathToZIPFile), IO.Compression.ZipArchiveMode.Update, IO.Compression.ZipArchiveMode.Create)

                Using zipFileObject As IO.Compression.ZipArchive = IO.Compression.ZipFile.Open(pathToZIPFile, zipArchiveMode)
                    addFileToZipFile(zipFileObject, strFileToBeAdded)
                End Using

                Return True
            Catch ex As Exception
                eventLogFunctions.writeCrashToApplicationLogFile(ex)
                Return False
            End Try
        End Function

        ''' <summary>Converts the number of Bytes to a nice way of saying it, like MBs, GBs, etc.</summary>
        ''' <param name="size">The amount of data in Bytes.</param>
        ''' <param name="roundToNearestWholeNumber">Tells the function if it should round the number to the nearest whole number.</param>
        ''' <returns>A String value such as 100 MBs or 100 GBs.</returns>
        Public Function bytesToHumanSize(ByVal size As Long, Optional roundToNearestWholeNumber As Boolean = False) As String
            Dim result As String
            Dim shortRoundNumber As Short = If(roundToNearestWholeNumber, 0, 2)

            If size <= (2 ^ 10) Then
                result = size & " Bytes"
            ElseIf size > (2 ^ 10) And size <= (2 ^ 20) Then
                result = Math.Round(size / (2 ^ 10), shortRoundNumber) & " KBs"
            ElseIf size > (2 ^ 20) And size <= (2 ^ 30) Then
                result = Math.Round(size / (2 ^ 20), shortRoundNumber) & " MBs"
            ElseIf size > (2 ^ 30) And size <= (2 ^ 40) Then
                result = Math.Round(size / (2 ^ 30), shortRoundNumber) & " GBs"
            ElseIf size > (2 ^ 40) And size <= (2 ^ 50) Then
                result = Math.Round(size / (2 ^ 40), shortRoundNumber) & " TBs"
            ElseIf size > (2 ^ 50) And size <= (2 ^ 60) Then
                result = Math.Round(size / (2 ^ 50), shortRoundNumber) & " PBs"
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

                If commandLineArgument IsNot Nothing Then
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

                eventLogFunctions.writeToApplicationLogFile("Successfully set Safe Mode Boot flag.", EventLogEntryType.Information, False)
            Catch ex As Exception
                eventLogFunctions.writeCrashToApplicationLogFile(ex)
                eventLogFunctions.writeToApplicationLogFile("Unable to set Safe Mode Boot flag.", EventLogEntryType.Error, False)
            End Try
        End Sub

        Public Sub removeSafeModeBoot()
            Try
                Dim bcdEditor As New BCD.bcdEditor()

                If bcdEditor.getSafeModeBootStatus() Then
                    eventLogFunctions.writeToApplicationLogFile("The Safe Mode Boot flag has been detected, now attempting to remove it so the system can boot into normal mode.", EventLogEntryType.Information, False)
                    bcdEditor.removeSafeModeBootFlag()

                    If bcdEditor.getSafeModeBootStatus() Then
                        eventLogFunctions.writeToApplicationLogFile("Something went wrong, the Safe Mode Boot flag still exists.", EventLogEntryType.Error, False)
                    Else
                        eventLogFunctions.writeToApplicationLogFile("The Safe Mode Boot flag has been successfully removed.", EventLogEntryType.Information, False)
                    End If
                Else
                    If My.Settings.debug Then eventLogFunctions.writeToApplicationLogFile("No Safe Mode Boot flag has been detected.", EventLogEntryType.Information, False)
                End If

                bcdEditor.dispose()
                bcdEditor = Nothing
            Catch ex As Exception
                eventLogFunctions.writeCrashToApplicationLogFile(ex)
                eventLogFunctions.writeToApplicationLogFile("Unable to remove Safe Mode Boot flag.", EventLogEntryType.Error, False)
            End Try
        End Sub

        Public Sub executeCommand(pathToExecutable As String, strArguments As String, Optional runAsAdmin As Boolean = False)
            If IO.File.Exists(pathToExecutable) = True Then
                Dim processStartInfo As New ProcessStartInfo() With {
                    .FileName = pathToExecutable,
                    .WindowStyle = ProcessWindowStyle.Hidden,
                    .Arguments = strArguments,
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

        Public Sub executeCommandWithWait(pathToExecutable As String, strArguments As String, Optional runAsAdmin As Boolean = False)
            If IO.File.Exists(pathToExecutable) = True Then
                Dim processStartInfo As New ProcessStartInfo() With {
                    .FileName = pathToExecutable,
                    .WindowStyle = ProcessWindowStyle.Hidden,
                    .Arguments = strArguments,
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

                    If Not String.IsNullOrEmpty(arguments) Then
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
                eventLogFunctions.writeCrashToApplicationLogFile(ex)
                MsgBox("There was an error while attempting to elevate the process, please make sure that when the Windows UAC prompt appears asking you to run the program with elevated privileges that you say ""Yes"" to the UAC prompt." & vbCrLf & vbCrLf & "The program will now terminate.", MsgBoxStyle.Critical, globalVariables.programName)
                Process.GetCurrentProcess.Kill()
            End Try
        End Sub

        Public Sub reRunWithAdminUserRights()
            Try
                Dim startInfo As New ProcessStartInfo With {.FileName = New IO.FileInfo(Application.ExecutablePath).FullName}
                If Environment.GetCommandLineArgs.Count <> 1 Then startInfo.Arguments = Environment.GetCommandLineArgs(1)
                If Not privilegeChecks.areWeAnAdministrator() Then startInfo.Verb = "runas"

                Process.Start(startInfo)
                Process.GetCurrentProcess.Kill()
            Catch ex As ComponentModel.Win32Exception
                eventLogFunctions.writeCrashToApplicationLogFile(ex)
                MsgBox("There was an error while attempting to elevate the process, please make sure that when the Windows UAC prompt appears asking you to run the program with elevated privileges that you say ""Yes"" to the UAC prompt." & vbCrLf & vbCrLf & "The program crash notification window will now appear to allow you to send the crash data to me, the developer.", MsgBoxStyle.Critical + MsgBoxStyle.SystemModal, globalVariables.programName)
                If Not ex.ErrorCode.Equals(-2147467259) Then exceptionHandler.manuallyLoadCrashWindow(ex)
                Process.GetCurrentProcess.Kill()
            End Try
        End Sub

        Public Function areWeInSafeMode() As Boolean
            If SystemInformation.BootMode = BootMode.Normal Then
                Return False
            Else
                Return True
            End If
        End Function

        Public Sub killProcess(processID As Integer, Optional boolLogToEventLog As Boolean = False)
            If boolLogToEventLog = True Then
                eventLogFunctions.writeToApplicationLogFile("Killing process with PID of " & processID & ".", EventLogEntryType.Information, False)
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

        Public Function calculatePercentageValue(longSmallerNumber As Long, longLargerNumber As Long) As Double
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
            If Not url.Trim.StartsWith("http", StringComparison.OrdinalIgnoreCase) Then url = If(My.Settings.useSSL, "https://" & url, "http://" & url)

            Try
                Dim associatedApplication As String = Nothing

                If Not registryStuff.getFileAssociation(".html", associatedApplication) Then
                    Process.Start(url)
                Else
                    If IO.File.Exists(associatedApplication) Then
                        Process.Start(associatedApplication, Chr(34) & url & Chr(34))
                    Else
                        Process.Start(url)
                    End If
                End If
            Catch ex2 As ComponentModel.Win32Exception
                eventLogFunctions.writeCrashToApplicationLogFile(ex2)
                MsgBox("There was an error attempting to launch your web browser. Perhaps rebooting your system will correct this issue.", MsgBoxStyle.Information, "Restore Point Creator")
            Catch ex As Exception
                copyTextToWindowsClipboard(url)
                MsgBox(errorMessage, MsgBoxStyle.Information, "Restore Point Creator")
            End Try
        End Sub

        Public Function removeSourceCodePathInfo(strInput As String) As String
            If strInput.regExSearch("(?:Google Drive|OneDrive|AppData)") Then
                strInput = Regex.Replace(strInput, "C:\\Users\\Tom\\(?:Google Drive|OneDrive)\\My Visual Studio Projects\\Projects\\", "", RegexOptions.IgnoreCase)
                strInput = strInput.caseInsensitiveReplace("C:\Users\Tom\AppData\Local\Temp\", "")
                Return strInput
            Else
                Return strInput
            End If
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

        Public Function getSystemRAM() As String
            Try
                Return bytesToHumanSize((New Devices.ComputerInfo()).TotalPhysicalMemory, True)
            Catch ex As Exception
                Return "Unknown amount of System RAM"
            End Try
        End Function
    End Module
End Namespace