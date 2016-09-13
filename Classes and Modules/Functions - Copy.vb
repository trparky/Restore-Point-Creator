Imports Microsoft.Win32.TaskScheduler
Imports System.ComponentModel
Imports System.IO
Imports System.Text.RegularExpressions
Imports System.Text
Imports Microsoft.Win32
Imports System.Runtime.InteropServices
Imports System.Security.Principal
Imports IWshRuntimeLibrary
Imports System.Management

Namespace Functions
    Namespace exceptions
        Public Class httpProtocolException
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

        Public Class sslErrorException
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
    End Namespace

    Module Functions
        Public Function removeSourceCodePathInfo(strInput As String) As String
            If strInput.ToLower.caseInsensitiveContains("Google Drive") = True Then
                Return Regex.Replace(strInput, Regex.Escape("C:\Users\Tom\Google Drive\My Visual Studio Projects\Projects\"), "", RegexOptions.IgnoreCase)
            Else
                Return strInput
            End If
        End Function

        Public Function returnPreferedPlaceOnScreen(formObject As Form) As Point
            Return (Screen.FromControl(formObject)).Bounds.Location + New Point(100, 100)
        End Function

        Private Function getDateInShortForm() As String
            Dim year, month, day As String
            year = Now.Year
            month = Now.Month
            day = Now.Day

            If month.Length = 1 Then month = "0" & month
            If day.Length = 1 Then day = "0" & day

            Return year & month & day
        End Function

        Public Function convertToBase64(input As String) As String
            Return Convert.ToBase64String(Encoding.UTF8.GetBytes(input))
        End Function

        Public Function convertFromBase64(input As String) As String
            Return Encoding.UTF8.GetString(Convert.FromBase64String(input))
        End Function

        Public Function isNumeric(input As String) As Boolean
            'Return Regex.IsMatch(input.Trim, "\A[0-9]*\Z")
            Return Regex.IsMatch(input.Trim, "\A-{0,1}[0-9.]*\Z")
        End Function

        Function parseExecutableOutOfScheduledTaskAction(input As String) As String
            Dim matches As Match = Regex.Match(input, "((?:""|'){0,1}[A-Za-z]:\\.*\.(?:bat|bin|cmd|com|cpl|exe|gadget|inf1|ins|inx|isu|job|jse|lnk|msc|msi|msp|mst|paf|pif|ps1|reg|rgs|sct|shb|shs|u3p|vb|vbe|vbs|vbscript|ws|wsf)(?:""|'){0,1} {0,1})(.*)", RegexOptions.IgnoreCase)

            If matches.Success = True Then
                Return matches.Groups(1).Value.Replace(Chr(34), "").Trim
            Else
                Return Nothing
            End If
        End Function

        Function getProxyConfiguration() As Net.WebProxy
            If My.Settings.useSystemProxyConfig = True Then
                Return Net.WebProxy.GetDefaultProxy
                'Return Net.WebRequest.DefaultWebProxy
                'Return Net.WebRequest.GetSystemWebProxy()
            Else
                Dim proxyConfig As New Net.WebProxy(My.Settings.proxyAddress, My.Settings.proxyPort)

                If My.Settings.proxyUser <> Nothing And My.Settings.proxyPass <> Nothing Then
                    proxyConfig.Credentials = New Net.NetworkCredential(My.Settings.proxyUser, convertFromBase64(My.Settings.proxyPass))
                End If

                Return proxyConfig
            End If
        End Function

        Public Sub updateRestorePointCreatorUninstallationInfo()
            Try
                Dim displayName As String
                Dim installerRegistryPath As RegistryKey = Registry.LocalMachine.OpenSubKey("SOFTWARE\Microsoft\Windows\CurrentVersion\Uninstall\{CC48DE1C-8EC2-43BC-9201-29701CD9AE13}_is1", True)

                If installerRegistryPath IsNot Nothing Then
                    displayName = installerRegistryPath.GetValue("DisplayName", "")

                    If displayName.ToLower.Contains("restore point creator") = True Then
                        installerRegistryPath.SetValue("DisplayName", String.Format("Restore Point Creator version {0}", globalVariables.versionString), RegistryValueKind.String)
                        installerRegistryPath.SetValue("DisplayVersion", globalVariables.versionInfo(globalVariables.versionPieces.major) & "." & globalVariables.versionInfo(globalVariables.versionPieces.minor), RegistryValueKind.String)
                        installerRegistryPath.SetValue("DisplayIcon", Application.ExecutablePath & ",0", RegistryValueKind.String)

                        installerRegistryPath.SetValue("Publisher", "Tom Parkison")
                        installerRegistryPath.SetValue("MajorVersion", globalVariables.versionInfo(globalVariables.versionPieces.major), RegistryValueKind.DWord)
                        installerRegistryPath.SetValue("MinorVersion", globalVariables.versionInfo(globalVariables.versionPieces.minor), RegistryValueKind.DWord)
                        installerRegistryPath.SetValue("Build", globalVariables.versionInfo(globalVariables.versionPieces.build), RegistryValueKind.DWord)
                        installerRegistryPath.SetValue("InstallDate", getDateInShortForm())

                        writeToSystemEventLog("Updated uninstall information in system Registry.", EventLogEntryType.Information)
                    End If

                    installerRegistryPath.Close()
                    installerRegistryPath.Dispose()
                End If
            Catch ex As Exception
                writeToSystemEventLog("Unable to update uninstall information in system Registry.", EventLogEntryType.Error)
                writeCrashToEventLog(ex)
            End Try
        End Sub

        Public Sub disableFocusingOnPleaseWaitWindow()
            Try
                If windowInstances.frmPleaseWait IsNot Nothing Then
                    windowInstances.frmPleaseWait.allowAutoFocus = False
                End If
            Catch ex As Exception
            End Try
        End Sub

        Public Sub enableFocusingOnPleaseWaitWindow()
            Try
                If windowInstances.frmPleaseWait IsNot Nothing Then windowInstances.frmPleaseWait.allowAutoFocus = True
            Catch ex As Exception
            End Try
        End Sub

        Public Sub openPleaseWaitWindow()
            Try
                If windowInstances.frmPleaseWait IsNot Nothing Then windowInstances.frmPleaseWait.ShowDialog()
            Catch ex As Exception
            End Try
        End Sub

        ''' <summary>Creates a Please Wait window.</summary>
        ''' <param name="message">Sets the message for the Please Wait window.</param>
        ''' <param name="openDialog">This is an optional setting, normally it's False. But if set to True, the function will show the newly created Please Wait window in a new thread. Normally, this function doesn't do that.</param>
        Public Sub createPleaseWaitWindow(message As String, Optional ByVal openDialog As Boolean = False)
            Try
                windowInstances.frmPleaseWait = New Please_Wait
                windowInstances.frmPleaseWait.StartPosition = FormStartPosition.CenterParent
                windowInstances.frmPleaseWait.pleaseWaitlblLabel.Text = message
                windowInstances.frmPleaseWait.lblLabelText = message
                windowInstances.frmPleaseWait.Icon = loadPleaseWaitIcon()
                windowInstances.frmPleaseWait.TopMost = True

                If openDialog = True Then
                    globalVariables.pleaseWaitWindowThread = New Threading.Thread(AddressOf openPleaseWaitWindow)
                    globalVariables.pleaseWaitWindowThread.Start()
                End If
            Catch ex As Exception
            End Try
        End Sub

        Public Sub closePleaseWaitWindow()
            Try
                If windowInstances.frmPleaseWait IsNot Nothing Then
                    windowInstances.frmPleaseWait.allowClose = True
                    windowInstances.frmPleaseWait.Close()
                    windowInstances.frmPleaseWait.Dispose()
                    windowInstances.frmPleaseWait = Nothing

                    If globalVariables.pleaseWaitWindowThread IsNot Nothing Then
                        globalVariables.pleaseWaitWindowThread.Abort()
                        globalVariables.pleaseWaitWindowThread = Nothing
                    End If
                End If
            Catch ex As Exception
            End Try
        End Sub

        <DllImport("Srclient.dll")> Public Function SRRemoveRestorePoint(index As Integer) As Integer
        End Function

        Public Sub launchURLInWebBrowser(url As String, Optional errorMessage As String = "An error occurred when trying the URL In your Default browser. The URL has been copied to your Windows Clipboard for you to paste into the address bar in the web browser of your choice.")
            Try
                Dim associatedApplication As String = Nothing

                If getFileAssociation(".html", associatedApplication) = False Then
                    Process.Start(url)
                Else
                    If IO.File.Exists(associatedApplication) = True Then
                        Process.Start(associatedApplication, Chr(34) & url & Chr(34))
                    Else
                        Process.Start(url)
                    End If
                End If
            Catch ex2 As Win32Exception
                writeCrashToEventLog(ex2)
                MsgBox("There was an error attempting to launch your web browser. Perhaps rebooting your system will correct this issue.", MsgBoxStyle.Information, "Restore Point Creator")
            Catch ex As Exception
                Clipboard.SetText(url)
                MsgBox(errorMessage, MsgBoxStyle.Information, "Restore Point Creator")
            End Try
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

        Public Function randomStringGenerator(length As Integer)
            Dim random As Random = New Random()
            Dim builder As New StringBuilder()
            Dim ch As Char
            Dim legalCharacters As String = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz1234567890"

            For cntr As Integer = 0 To length
                ch = legalCharacters.Substring(random.Next(0, legalCharacters.Length), 1)
                builder.Append(ch)
            Next

            Return builder.ToString()
        End Function

        Public Function SHA1ChecksumString(input As String) As String
            Dim SHA1Engine As New Security.Cryptography.SHA1CryptoServiceProvider
            Dim inputAsByteArray As Byte() = Encoding.ASCII.GetBytes(input)
            Dim hashedByteArray As Byte() = SHA1Engine.ComputeHash(inputAsByteArray)
            Return BitConverter.ToString(hashedByteArray).ToLower().Replace("-", "").Trim
        End Function

        Private Function SHA160ChecksumFile(ByVal filename As String) As String
            Dim SHA1Engine As New Security.Cryptography.SHA1CryptoServiceProvider

            Dim FileStream As New IO.FileStream(filename, IO.FileMode.Open, IO.FileAccess.Read, IO.FileShare.Read, 10 * 1048576, IO.FileOptions.SequentialScan)
            Dim Output As Byte() = SHA1Engine.ComputeHash(FileStream)
            FileStream.Close()
            FileStream.Dispose()

            Return BitConverter.ToString(Output).ToLower().Replace("-", "").Trim
        End Function

        Private Function SHA160ChecksumFile(ByRef stream As IO.Stream) As String
            Dim SHA1Engine As New Security.Cryptography.SHA1CryptoServiceProvider

            Dim Output As Byte() = SHA1Engine.ComputeHash(stream)
            Return BitConverter.ToString(Output).ToLower().Replace("-", "").Trim
        End Function

        Public Function verifyChecksum(urlOfChecksumFile As String, fileToVerify As String, boolGiveUserAnErrorMessage As Boolean) As Boolean
            Dim checksumFromWeb As String = Nothing

            Try
                If getWebURLData(urlOfChecksumFile, checksumFromWeb) = True Then
                    ' Checks to see if we have a valid SHA1 file.
                    If Regex.IsMatch(checksumFromWeb, "([a-zA-Z0-9]{40})") = True Then
                        ' Now that we have a valid SHA1 file we need to parse out what we want.
                        checksumFromWeb = Regex.Match(checksumFromWeb, "([a-zA-Z0-9]{40})").Groups(1).Value().ToLower.Trim()

                        ' Now we do the actual checksum verification by passing the name of the file to the SHA160() function
                        ' which calculates the checksum of the file on disk. We then compare it to the checksum from the web.
                        If SHA160ChecksumFile(fileToVerify) = checksumFromWeb Then
                            Return True ' OK, things are good; the file passed checksum verification so we return True.
                        Else
                            ' The checksums don't match. Oops.
                            If boolGiveUserAnErrorMessage = True Then
                                MsgBox("There was an error in the download, checksums don't match. Update process aborted.", MsgBoxStyle.Critical, "Restore Point Creator")
                            End If

                            Return False
                        End If
                    Else
                        If boolGiveUserAnErrorMessage = True Then
                            MsgBox("Invalid SHA1 file detected. Update process aborted.", MsgBoxStyle.Critical, "Restore Point Creator")
                        End If

                        Return False
                    End If
                Else
                    If boolGiveUserAnErrorMessage = True Then
                        MsgBox("There was an error downloading the checksum verification file. Update process aborted.", MsgBoxStyle.Critical, "Restore Point Creator")
                    End If

                    Return False
                End If
            Catch ex As Exception
                writeCrashToEventLog(ex)

                If boolGiveUserAnErrorMessage = True Then
                    MsgBox("There was an error downloading the checksum verification file. Update process aborted.", MsgBoxStyle.Critical, "Restore Point Creator")
                End If

                Return False
            End Try
        End Function

        Public Function isThisAServerOS() As Boolean
            Dim strOSProductType As String = Registry.LocalMachine.OpenSubKey("SYSTEM\CurrentControlSet\Control\ProductOptions", False).GetValue("ProductType", Nothing)

            If strOSProductType.caseInsensitiveContains("ServerNT") = True Then
                Return True
            Else
                Return False
            End If
        End Function

        <DllImport("kernel32.dll", SetLastError:=False)>
        Private Function GetProductInfo(intOSMajorVersion As Integer, intOSMinorVersion As Integer, intSPMajorVersion As Integer, intSPMinorVersion As Integer, ByRef intProductType As Integer) As Boolean
        End Function

        Public Function getFullOSVersionString() As String
            Try
                Dim intOSMajorVersion As Integer = Environment.OSVersion.Version.Major
                Dim intOSMinorVersion As Integer = Environment.OSVersion.Version.Minor
                Dim dblDOTNETVersion As Double = Double.Parse(Environment.Version.Major & "." & Environment.Version.Minor)
                Dim strOSName As String

                If intOSMajorVersion = 5 And intOSMinorVersion = 0 Then
                    strOSName = "Windows 2000"
                ElseIf intOSMajorVersion = 5 And intOSMinorVersion = 1 Then
                    strOSName = "Windows XP"
                ElseIf intOSMajorVersion = 6 And intOSMinorVersion = 0 Then
                    strOSName = "Windows Vista"
                ElseIf intOSMajorVersion = 6 And intOSMinorVersion = 1 Then
                    strOSName = "Windows 7"
                ElseIf intOSMajorVersion = 6 And intOSMinorVersion = 2 Then
                    strOSName = "Windows 8"
                ElseIf intOSMajorVersion = 6 And intOSMinorVersion = 3 Then
                    strOSName = "Windows 8.1"
                ElseIf intOSMajorVersion = 10 Then
                    strOSName = "Windows 10"
                Else
                    strOSName = String.Format("Windows NT {0}.{1}", intOSMajorVersion, intOSMinorVersion)
                End If

                If Environment.Is64BitOperatingSystem Then
                    Return String.Format("{0} 64-bit (Microsoft .NET {1})", strOSName, dblDOTNETVersion)
                Else
                    Return String.Format("{0} 32-bit (Microsoft .NET {1})", strOSName, dblDOTNETVersion)
                End If
            Catch ex As Exception
                Try
                    Return "Unknown Windows Operating System (" & Environment.OSVersion.VersionString & ")"
                Catch ex2 As Exception
                    Return "Unknown Windows Operating System"
                End Try
            End Try
        End Function

        Public Function getSystemRAM() As String
            Dim computerInfo As New Devices.ComputerInfo()
            Return fileSizeToHumanReadableFormat(computerInfo.TotalPhysicalMemory, True)
        End Function

        Public Function getSystemProcessor() As processorInfoClass
            Try
                Dim searcher As New ManagementObjectSearcher("root\CIMV2", "Select * FROM Win32_Processor")
                Dim queryObj As ManagementObjectCollection = searcher.Get()

                Dim rawProcessorName As String = queryObj(0)("Name").ToString.Trim
                rawProcessorName = Regex.Replace(rawProcessorName, "\(R\)", "", RegexOptions.IgnoreCase)
                rawProcessorName = Regex.Replace(rawProcessorName, "\(TM\)", "", RegexOptions.IgnoreCase)

                Dim processorInfo As New processorInfoClass
                processorInfo.strProcessor = rawProcessorName
                processorInfo.strNumberOfCores = queryObj(0)("NumberOfCores").ToString
                Return processorInfo
            Catch ex As Exception
                Dim processorInfo As New processorInfoClass
                processorInfo.strProcessor = "unknown"
                processorInfo.strNumberOfCores = "unknown"
                Return processorInfo
            End Try

        End Function

        'Debug.WriteLine("gdiHandles = {0}", Functions.GetGuiResources(Process.GetCurrentProcess().Handle, GlobalVariables.gdiHandles))
        'Debug.WriteLine("userHandles = {0}", Functions.GetGuiResources(Process.GetCurrentProcess().Handle, GlobalVariables.userHandles))
        <DllImport("User32")> Public Function GetGuiResources(hProcess As IntPtr, uiFlags As Integer) As Integer
        End Function

        Public Const ERROR_SUCCESS As Integer = 0
        Public Const ERROR_BAD_ENVIRONMENT As Integer = 10
        Public Const ERROR_DISK_FULL As Integer = 112
        Public Const ERROR_INTERNAL_ERROR As Integer = 1359
        Public Const ERROR_INVALID_DATA As Integer = 13
        Public Const ERROR_SERVICE_DISABLED As Integer = 1058
        Public Const ERROR_TIMEOUT As Integer = 1460

        Public Const constStringRoot As String = "root"

        Public Sub giveComExceptionCrashMessage()
            MsgBox("A program crash that should Not happen On properly working Windows installations has occurred.  Your windows installation will need To be repaired before this program will Function properly." & vbCrLf & vbCrLf & "Your web browser will now open With instructions On how To repair your Windows installation.", MsgBoxStyle.Critical, "Critical Windows Component Failure Detected")
            launchURLInWebBrowser(globalVariables.comExceptionCrashURL, "An Error occurred When trying To launch a URL. The URL has been copied To your Windows Clipboard For you To paste into the address bar In the browser Of your choice.")
            Process.GetCurrentProcess.Kill()
        End Sub

        Public Function IsUserInAdminGroup() As Boolean
            Dim fInAdminGroup As Boolean = False
            Dim hToken As SafeTokenHandle = Nothing
            Dim hTokenToCheck As SafeTokenHandle = Nothing
            Dim pElevationType As IntPtr = IntPtr.Zero
            Dim pLinkedToken As IntPtr = IntPtr.Zero
            Dim cbSize As Integer = 0

            Try
                ' Open the access token of the current process for query and duplicate.
                If (Not NativeMethod.OpenProcessToken(Process.GetCurrentProcess.Handle, NativeMethod.TOKEN_QUERY Or NativeMethod.TOKEN_DUPLICATE, hToken)) Then
                    Throw New Win32Exception(Marshal.GetLastWin32Error)
                End If

                ' Determine whether system is running Windows Vista or later operating
                ' systems (major version >= 6) because they support linked tokens, but
                ' previous versions (major version < 6) do not.
                If (Environment.OSVersion.Version.Major >= 6) Then
                    ' Running Windows Vista or later (major version >= 6).
                    ' Determine token type: limited, elevated, or default.

                    ' Allocate a buffer for the elevation type information.
                    cbSize = 4  ' Size of TOKEN_ELEVATION_TYPE
                    pElevationType = Marshal.AllocHGlobal(cbSize)
                    If (pElevationType = IntPtr.Zero) Then
                        Throw New Win32Exception(Marshal.GetLastWin32Error)
                    End If

                    ' Retrieve token elevation type information.
                    If (Not NativeMethod.GetTokenInformation(hToken, TOKEN_INFORMATION_CLASS.TokenElevationType, pElevationType, cbSize, cbSize)) Then
                        Throw New Win32Exception(Marshal.GetLastWin32Error)
                    End If

                    ' Marshal the TOKEN_ELEVATION_TYPE enum from native to .NET.
                    Dim elevType As TOKEN_ELEVATION_TYPE = Marshal.ReadInt32(pElevationType)

                    ' If limited, get the linked elevated token for further check.
                    If (elevType = TOKEN_ELEVATION_TYPE.TokenElevationTypeLimited) Then
                        ' Allocate a buffer for the linked token.
                        cbSize = IntPtr.Size
                        pLinkedToken = Marshal.AllocHGlobal(cbSize)
                        If (pLinkedToken = IntPtr.Zero) Then
                            Throw New Win32Exception(Marshal.GetLastWin32Error)
                        End If

                        ' Get the linked token.
                        If (Not NativeMethod.GetTokenInformation(hToken, TOKEN_INFORMATION_CLASS.TokenLinkedToken, pLinkedToken, cbSize, cbSize)) Then
                            Throw New Win32Exception(Marshal.GetLastWin32Error)
                        End If

                        ' Marshal the linked token value from native to .NET.
                        Dim hLinkedToken As IntPtr = Marshal.ReadIntPtr(pLinkedToken)
                        hTokenToCheck = New SafeTokenHandle(hLinkedToken)
                    End If
                End If

                ' CheckTokenMembership requires an impersonation token. If we just got a
                ' linked token, it already is an impersonation token.  If we did not get
                ' a linked token, duplicate the original into an impersonation token for
                ' CheckTokenMembership.
                If (hTokenToCheck Is Nothing) Then
                    If (Not NativeMethod.DuplicateToken(hToken, SECURITY_IMPERSONATION_LEVEL.SecurityIdentification, hTokenToCheck)) Then
                        Throw New Win32Exception(Marshal.GetLastWin32Error)
                    End If
                End If

                ' Check if the token to be checked contains admin SID.
                Dim id As New WindowsIdentity(hTokenToCheck.DangerousGetHandle)
                Dim principal As New WindowsPrincipal(id)
                fInAdminGroup = principal.IsInRole(WindowsBuiltInRole.Administrator)
            Catch ex As Exception
                ' Something went wrong here so we are going to assume that the user isn't part of the Administrator user group.
                fInAdminGroup = False
                Functions.writeCrashToEventLog(ex) ' Write exception data to the Windows Event Log.
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
                    Marshal.FreeHGlobal(pElevationType)
                    pElevationType = IntPtr.Zero
                End If
                If (pLinkedToken <> IntPtr.Zero) Then
                    Marshal.FreeHGlobal(pLinkedToken)
                    pLinkedToken = IntPtr.Zero
                End If
            End Try

            Return fInAdminGroup
        End Function

        Public Function checkForInternetConnection() As Boolean
            If My.Computer.Network.IsAvailable = True Then
                Return True
            Else
                Return False
            End If
        End Function

        Function convertLineFeeds(input As String) As String
            ' Checks to see if the file is in Windows linefeed format or UNIX linefeed format.
            If input.Contains(vbCrLf) Then
                Return input ' It's in Windows linefeed format so we return the output as is.
            Else
                Return input.Replace(vbLf, vbCrLf) ' It's in UNIX linefeed format so we have to convert it to Windows before we return the output.
            End If
        End Function

        Public Function downloadFile(urlToDownloadFrom As String, localFileName As String) As Boolean
            Try
                urlToDownloadFrom = globalVariables.transformURL(urlToDownloadFrom)

                Dim httpWebRequest As New Net.WebClient
                If My.Settings.useHTTPProxy = True Then httpWebRequest.Proxy = getProxyConfiguration()

                httpWebRequest.Headers("PROGRAM_NAME") = globalVariables.programName
                httpWebRequest.Headers("PROGRAM_VERSION") = globalVariables.versionString
                httpWebRequest.Headers("OPERATING_SYSTEM") = getFullOSVersionString()
                httpWebRequest.Headers.Set(Net.HttpRequestHeader.UserAgent, createHTTPUserAgentHeaderString())

                'Debug.WriteLine("headers = " & httpWebRequest.Headers.ToString())

                httpWebRequest.DownloadFile(urlToDownloadFrom, localFileName)
                httpWebRequest.Dispose()

                Return True
            Catch ex As Exception
                writeToSystemEventLog(String.Format("Unable To contact web server For file download. The URL was {0}{1}{0}.", Chr(34), urlToDownloadFrom), EventLogEntryType.Error)
                writeCrashToEventLog(ex)

                Return False
            End Try
        End Function

        ''' <summary>Sends data to a URL of your choosing.</summary>
        ''' <param name="url">This is the URL that the program will send to the web server in the POST data stream.</param>
        ''' <param name="postData">A Dictionary(Of String, String) containing the data needed to be POSTed to the web server.</param>
        ''' <param name="httpResponseText">This is a ByRef variable so declare it before passing it to this function, think of this as a pointer. The HTML/text content that the web server on the other end responds with is put into this variable and passed back in a ByRef function.</param>
        ''' <returns>A Boolean value. If the HTTP operation was successful it returns a TRUE value, if not FALSE.</returns>
        ''' <exception cref="exceptions.httpProtocolException">This exception is thrown if the server responds with an HTTP Error.</exception>
        ''' <exception cref="Net.WebException">If this function throws a Net.WebException then a general web exception occurred.</exception>
        ''' <exception cref="exceptions.sslErrorException">If this function throws an sslErrorException, an error occurred while negotiating an SSL connection.</exception>
        Public Function postDataToWebURLAndGetData(ByVal url As String, ByVal postData As Dictionary(Of String, String), ByRef httpResponseText As String) As Boolean
            Try
                url = globalVariables.transformURL(url)
                Dim postDataString As String = getPostDataString(postData)
                Debug.WriteLine("postDataString = " & postDataString)

                'Dim httpWebRequest As Net.WebRequest = Net.WebRequest.Create(url) ' OLD WAY
                Dim httpWebRequest As Net.HttpWebRequest = DirectCast(Net.WebRequest.Create(url), Net.HttpWebRequest)
                If My.Settings.useHTTPProxy = True Then httpWebRequest.Proxy = getProxyConfiguration()

                httpWebRequest.UserAgent = createHTTPUserAgentHeaderString()
                httpWebRequest.Method = "POST"
                httpWebRequest.ContentType = "application/x-www-form-urlencoded"
                httpWebRequest.ContentLength = postDataString.Length

                httpWebRequest.Headers("PROGRAM_NAME") = globalVariables.programName
                httpWebRequest.Headers("PROGRAM_VERSION") = globalVariables.versionString
                httpWebRequest.Headers("OPERATING_SYSTEM") = getFullOSVersionString()

                'Debug.WriteLine("headers = " & httpWebRequest.Headers.ToString())

                Dim httpRequestWriter = New StreamWriter(httpWebRequest.GetRequestStream())
                httpRequestWriter.Write(postDataString)
                httpRequestWriter.Close()
                httpRequestWriter.Dispose()
                httpRequestWriter = Nothing

                Dim httpWebResponse As Net.WebResponse = httpWebRequest.GetResponse()
                Dim httpInStream As New StreamReader(httpWebResponse.GetResponseStream())
                Dim httpTextOutput As String = httpInStream.ReadToEnd.Trim()

                httpInStream.Close()
                httpInStream.Dispose()
                httpWebRequest = Nothing

                httpResponseText = convertLineFeeds(httpTextOutput).Trim()
                Return True
            Catch ex As Net.WebException
                If ex.Status = Net.WebExceptionStatus.ProtocolError Then
                    If ex.Message.Contains("(500)") = True Then
                        Throw New exceptions.httpProtocolException("HTTP Protocol Error (Server 500 Error)", ex)
                    Else
                        Throw New exceptions.httpProtocolException("HTTP Protocol Error ", ex)
                    End If

                    Return False
                ElseIf ex.Status = Net.WebExceptionStatus.TrustFailure Then
                    Throw New exceptions.sslErrorException("There was an error establishing an SSL connection.", ex)
                    Return False
                End If

                Throw New Net.WebException(ex.Message, ex)
                Return False
            Catch ex As Exception
                writeToSystemEventLog(String.Format("Unable To contact web server. The URL was {0}{1}{0}.", Chr(34), url), EventLogEntryType.Error)
                writeCrashToEventLog(ex)
                Return False
            End Try
        End Function

        ''' <summary>Pulls down the data from a web URL of your choosing.</summary>
        ''' <param name="url">This is the URL that the program will send a request to.</param>
        ''' <param name="httpResponseText">This is a ByRef variable so declare it before passing it to this function, think of this as a pointer. The HTML/text content that the web server on the other end responds with is put into this variable and passed back in a ByRef function.</param>
        ''' <returns>A Boolean value. If the HTTP operation was successful it returns a TRUE value, if not FALSE.</returns>
        ''' <exception cref="exceptions.httpProtocolException">This exception is thrown if the server responds with an HTTP Error.</exception>
        ''' <exception cref="Net.WebException">If this function throws a Net.WebException then a general web exception occurred.</exception>
        ''' <exception cref="exceptions.sslErrorException">If this function throws an sslErrorException, an error occurred while negotiating an SSL connection.</exception>
        Public Function getWebURLData(ByVal url As String, ByRef httpResponseText As String) As Boolean
            Try
                url = globalVariables.transformURL(url)
                Debug.WriteLine("url = " & url)

                'Dim httpWebRequest As Net.WebRequest = Net.WebRequest.Create(url) ' OLD WAY
                Dim httpWebRequest As Net.HttpWebRequest = DirectCast(Net.WebRequest.Create(url), Net.HttpWebRequest)

                If My.Settings.useHTTPProxy = True Then httpWebRequest.Proxy = getProxyConfiguration()

                httpWebRequest.UserAgent = createHTTPUserAgentHeaderString()
                httpWebRequest.Headers("PROGRAM_NAME") = globalVariables.programName
                httpWebRequest.Headers("PROGRAM_VERSION") = globalVariables.versionString
                httpWebRequest.Headers("OPERATING_SYSTEM") = getFullOSVersionString()

                'Debug.WriteLine("headers = " & httpWebRequest.Headers.ToString())

                Dim httpWebResponse As Net.WebResponse = httpWebRequest.GetResponse()

                Dim httpInStream As New StreamReader(httpWebResponse.GetResponseStream())
                Dim httpTextOutput As String = httpInStream.ReadToEnd.Trim()

                httpInStream.Close()
                httpInStream.Dispose()
                httpInStream = Nothing

                httpResponseText = convertLineFeeds(httpTextOutput).Trim
                Return True
            Catch ex As Net.WebException
                If ex.Status = Net.WebExceptionStatus.ProtocolError Then
                    If ex.Message.Contains("(500)") = True Then
                        Throw New exceptions.httpProtocolException("HTTP Protocol Error (Server 500 Error)", ex)
                    Else
                        Throw New exceptions.httpProtocolException("HTTP Protocol Error ", ex)
                    End If

                    Return False
                ElseIf ex.Status = Net.WebExceptionStatus.TrustFailure Then
                    Throw New exceptions.sslErrorException("There was an error establishing an SSL connection.", ex)
                    Return False
                End If

                Throw New Net.WebException(ex.Message, ex)
                Return False
            Catch ex As Exception
                writeToSystemEventLog(String.Format("Unable To contact web server. The URL was {0}{1}{0}.", Chr(34), url), EventLogEntryType.Error)
                writeCrashToEventLog(ex)
                Return False
            End Try
        End Function

        ''' <summary>Converts a Dictionary of Strings into a String ready to be POSTed to a URL.</summary>
        ''' <param name="postData">A Dictionary(Of String, String) containing the data needed to by POSTed to a web server.</param>
        ''' <returns>Returns a String value containing the POST data.</returns>
        Private Function getPostDataString(postData As Dictionary(Of String, String)) As String
            Dim postDataString As String = ""
            For Each entry As KeyValuePair(Of String, String) In postData
                postDataString &= entry.Key.Trim & "=" & Web.HttpUtility.UrlEncode(entry.Value.Trim) & "&"
            Next

            If postDataString.EndsWith("&") Then
                postDataString = postDataString.Substring(0, postDataString.Length - 1)
            End If

            Return postDataString
        End Function

        ''' <summary>Creates a User Agent String for this program to be used in HTTP requests.</summary>
        ''' <returns>String type.</returns>
        Public Function createHTTPUserAgentHeaderString() As String
            If globalVariables.boolBeta = True Then
                Return String.Format("{0} version {1} Public Beta {2} on {3}", globalVariables.programName, globalVariables.versionString, globalVariables.shortBetaVersion, getFullOSVersionString())
            ElseIf globalVariables.boolReleaseCandidate = True Then
                Return String.Format("{0} version {1} Release Candidate {2} on {3}", globalVariables.programName, globalVariables.versionString, globalVariables.shortReleaseCandidateVersion, getFullOSVersionString())
            Else
                Return String.Format("{0} version {1} on {2}", globalVariables.programName, globalVariables.versionString, getFullOSVersionString())
            End If
        End Function

        Public Function doesTaskExist(ByVal nameOfTask As String, ByRef taskObject As Task) As Boolean
            Using taskServiceObject As TaskService = New TaskService()
                taskObject = taskServiceObject.GetTask(nameOfTask)

                If taskObject Is Nothing Then
                    Return False
                Else
                    Return True
                End If
            End Using
        End Function

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

        Public Sub doDeletingOfXNumberOfRestorePoints(ByVal shortHowManyToKeep As Short)
            Dim systemRestorePoints As New ManagementObjectSearcher("root\Default", "Select * FROM SystemRestore")

            If systemRestorePoints.Get.Count <> 0 Then
                Dim numberOfRestorePointsToBeDeleted As Short = systemRestorePoints.Get.Count - shortHowManyToKeep

                If numberOfRestorePointsToBeDeleted < 0 Or numberOfRestorePointsToBeDeleted = 0 Then
                    systemRestorePoints.Dispose()
                    systemRestorePoints = Nothing
                    Exit Sub
                Else
                    For Each systemRestorePoint As ManagementObject In systemRestorePoints.Get()
                        If numberOfRestorePointsToBeDeleted = 0 Then
                            Exit For
                        Else
                            numberOfRestorePointsToBeDeleted -= 1
                            SRRemoveRestorePoint(Integer.Parse(systemRestorePoint("SequenceNumber")))
                        End If
                    Next
                End If
            End If

            systemRestorePoints.Dispose()
            systemRestorePoints = Nothing
        End Sub

        Private Function doesPIDExist(PID As Integer) As Boolean
            Try
                Dim searcher As New ManagementObjectSearcher("root\CIMV2", String.Format("Select * FROM Win32_Process WHERE ProcessId={0}", PID))

                If searcher.Get.Count = 0 Then
                    searcher.Dispose()
                    Return False
                Else
                    searcher.Dispose()
                    Return True
                End If
            Catch ex3 As COMException
                giveComExceptionCrashMessage()
                Return False
            Catch ex As Exception
                Return False
            End Try
        End Function

        Private Sub killProcess(PID As Integer)
            Debug.Write(String.Format("Killing PID {0}...", PID))

            If doesPIDExist(PID) Then
                Process.GetProcessById(PID).Kill()
            End If

            If doesPIDExist(PID) Then
                killProcess(PID)
                'Else
                'debug.writeline(" Process Killed.")
            End If
        End Sub

        Public Sub searchForProcessAndKillIt(strFileName As String, boolFullFilePathPassed As Boolean)
            Dim fullFileName As String

            If boolFullFilePathPassed = True Then
                fullFileName = strFileName
            Else
                fullFileName = New FileInfo(strFileName).FullName
            End If

            Dim wmiQuery As String = String.Format("Select ExecutablePath, ProcessId FROM Win32_Process WHERE ExecutablePath = '{0}'", fullFileName.addSlashes())
            Dim searcher As New ManagementObjectSearcher("root\CIMV2", wmiQuery)

            Try
                For Each queryObj As ManagementObject In searcher.Get()
                    killProcess(Integer.Parse(queryObj("ProcessId").ToString))
                Next

                'debug.writeline("All processes killed... Update process can continue.")
            Catch ex3 As COMException
                giveComExceptionCrashMessage()
            Catch err As ManagementException
                ' Does nothing
            End Try
        End Sub

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

        Function isThisWindows8x() As Boolean
            If Environment.OSVersion.ToString.Contains("6.2") = True Or Environment.OSVersion.ToString.Contains("6.3") = True Then
                Return True
            Else
                Return False
            End If
        End Function

        Function isThisWindows10() As Boolean
            If Environment.OSVersion.Version.Major = 10 Then
                Return True
            Else
                Return False
            End If
        End Function

        ''' <summary>Write the exception event to the System Log File.</summary>
        ''' <param name="exceptionObject">The exception object.</param>
        ''' <example>functions.writeCrashToEventLog(ex)</example>
        Sub writeCrashToEventLog(exceptionObject As Exception)
            Try
                Dim stringBuilder As New StringBuilder

                stringBuilder.AppendLine("System Information")
                stringBuilder.AppendLine("Time of Crash: " & Now.ToString)
                stringBuilder.AppendLine("Operating System: " & getFullOSVersionString())
                stringBuilder.AppendLine("System RAM: " & getSystemRAM())

                Dim processorInfo As processorInfoClass = getSystemProcessor()
                stringBuilder.AppendLine("CPU: " & processorInfo.strProcessor)
                stringBuilder.AppendLine("Number of Cores: " & processorInfo.strNumberOfCores)

                stringBuilder.AppendLine()

                If globalVariables.boolBeta = True Then
                    stringBuilder.AppendLine("Program Version: " & String.Format("{0} Public Beta {1}", globalVariables.versionString, globalVariables.shortBetaVersion))
                ElseIf globalVariables.boolReleaseCandidate = True Then
                    stringBuilder.AppendLine("Program Version: " & String.Format("{0} Release Candidate {1}", globalVariables.versionString, globalVariables.shortReleaseCandidateVersion))
                Else
                    stringBuilder.AppendLine("Program Version: " & globalVariables.versionString)
                End If

                stringBuilder.AppendLine("Running As: " & Environment.UserName)
                stringBuilder.AppendLine("Exception Type: " & exceptionObject.GetType.ToString)
                stringBuilder.AppendLine("Message: " & removeSourceCodePathInfo(exceptionObject.Message))

                stringBuilder.AppendLine()

                stringBuilder.Append("The exception occurred ")

                stringBuilder.AppendLine(exceptionObject.StackTrace.Trim)

                writeToSystemEventLog(stringBuilder.ToString.Trim, EventLogEntryType.Error)
                stringBuilder = Nothing
            Catch ex2 As Exception
                ' Does nothing
            End Try
        End Sub

        Public Sub checkSystemDrivesForFullShadowStorage()
            Try
                Dim shadowStorageStatistics As ShadowStorageData = getShadowStorageData(globalVariables.systemDriveLetter)
                Dim shadowStorageUsePercentage As Short
                Dim shortUpperPercentLimit As Short = 80

                If shadowStorageStatistics IsNot Nothing Then
                    If (shadowStorageStatistics.UsedSpace = Nothing) = False And (shadowStorageStatistics.MaxSpace = Nothing) = False Then
                        shadowStorageUsePercentage = Math.Round((shadowStorageStatistics.UsedSpace / shadowStorageStatistics.MaxSpace) * 100, 2)

                        If shadowStorageUsePercentage > shortUpperPercentLimit Then
                            Dim fullSystemDriveDialog As New Reserved_Space_for_System_Drive()
                            fullSystemDriveDialog.StartPosition = FormStartPosition.CenterParent
                            fullSystemDriveDialog.ShowDialog()
                        End If
                    End If

                    shadowStorageStatistics = Nothing
                End If
                'globalVariables.systemDriveLetter

                'Dim driveLetter, deviceID As String

                'Dim fullDrives As New Specialized.StringCollection

                'For Each currentDrive As DriveInfo In My.Computer.FileSystem.Drives
                '    If currentDrive.DriveType = IO.DriveType.Fixed Or currentDrive.DriveType = IO.DriveType.Removable Then
                '        If (currentDrive.TotalFreeSpace = Nothing) = False And (currentDrive.TotalSize = Nothing) = False Then
                '            driveLetter = currentDrive.RootDirectory.ToString.Replace("\", "")
                '            deviceID = getDeviceIDFromDriveLetter(driveLetter)
                '            shadowStorageStatistics = getShadowStorageData(deviceID)

                '            If shadowStorageStatistics IsNot Nothing Then
                '                If (shadowStorageStatistics.UsedSpace = Nothing) = False And (shadowStorageStatistics.MaxSpace = Nothing) = False Then
                '                    shadowStorageUsePercentage = Math.Round((shadowStorageStatistics.UsedSpace / shadowStorageStatistics.MaxSpace) * 100, 2)

                '                    If shadowStorageUsePercentage > shortUpperPercentLimit Then
                '                        fullDrives.Add(driveLetter)
                '                    End If
                '                End If
                '            End If
                '        End If
                '    End If
                'Next

                'If fullDrives.Count > 0 Then
                '    Dim g As New Drives_with_Full_Restore_Point_Reserved_Space
                '    g.drives = fullDrives
                '    g.Icon = My.Resources.RestorePoint_noBackground_2
                '    g.ShowDialog()
                'End If

                'shadowStorageUsePercentage = Nothing
                'driveLetter = Nothing
                'deviceID = Nothing
                'shadowStorageStatistics = Nothing
            Catch ex As Exception
                ' We don't care if we crash here.
            End Try
        End Sub

        Public Sub checkForAndEnableSystemRestoreIfNeeded()
            Try
                If checkToSeeIfSystemRestoreIsEnabledOnSystemDrive() = False Then
                    writeToSystemEventLog("System Restore appears to not be enabled on this system.", EventLogEntryType.Information)

                    Dim shadowStorageDetails As ShadowStorageData = getShadowStorageData(globalVariables.systemDriveLetter)

                    If shadowStorageDetails IsNot Nothing Then
                        If shadowStorageDetails.MaxSpace = 0 Then
                            enableSystemRestoreOnDriveVSSAdmin(globalVariables.systemDriveLetter)
                        End If
                    End If

                    enableSystemRestoreOnDriveWMI(globalVariables.systemDriveLetter)
                End If
            Catch ex As Exception
                writeCrashToEventLog(ex)
                writeToSystemEventLog("System Restore appears to not be enabled on this system.", EventLogEntryType.Information)

                enableSystemRestoreOnDriveVSSAdmin(globalVariables.systemDriveLetter)
                enableSystemRestoreOnDriveWMI(globalVariables.systemDriveLetter)
            End Try
        End Sub

        ''' <summary>Activates System Restore on a given system drive.</summary>
        ''' <param name="driveLetter">The system drive letter you want to enable System Restore on.</param>
        ''' <example>functions.enableSystemRestoreOnDriveWMI("C:")</example>
        Public Function enableSystemRestoreOnDriveWMI(driveLetter As String) As Short
            Try
                Dim managementScopeObject As New ManagementScope("\\localhost\root\default")
                Dim managementPathObject As New ManagementPath("SystemRestore")
                Dim managementObjectOptions As New ObjectGetOptions()
                Dim managementClassObject As New ManagementClass(managementScopeObject, managementPathObject, managementObjectOptions)

                Dim managementBaseObjectParameters As ManagementBaseObject = managementClassObject.GetMethodParameters("Enable")
                managementBaseObjectParameters("Drive") = driveLetter
                managementBaseObjectParameters("WaitTillEnabled") = True

                Dim oOutParams As ManagementBaseObject = managementClassObject.InvokeMethod("Enable", managementBaseObjectParameters, Nothing)

                writeToSystemEventLog("Enabled System Restore on drive " & driveLetter.Substring(0, 1).ToUpper & ".", EventLogEntryType.Information)

                Return oOutParams("ReturnValue")
            Catch ex3 As COMException
                giveComExceptionCrashMessage()
                Return 1
            Catch ex As Exception
                Return 1
            End Try
        End Function

        'Private Sub enableSystemRestoreOnSystemDrive()
        '    enableSystemRestoreOnDriveVSSAdmin(Environment.SystemDirectory.Substring(0, 2))
        'End Sub

        'Private Declare Function GetSystemMetrics Lib "user32" (ByVal nIndex As Long) As Long

        Public Function areWeInSafeMode() As Boolean
            If SystemInformation.BootMode = BootMode.Normal Then
                Return False
            Else
                Return True
            End If
            'Select Case GetSystemMetrics(67)
            '    Case 1 : Return True
            '    Case 2 : Return True
            '    Case Else : Return False
            'End Select
        End Function

        Public Sub deleteAtUserLogonTask()
            Try
                Dim taskService As New TaskService

                If (taskService.GetTask(globalVariables.taskFolder & "\Create a Restore Point at User Logon") IsNot Nothing) Then
                    taskService.RootFolder.SubFolders(globalVariables.taskFolder).DeleteTask("Create a Restore Point at User Logon")
                End If

                taskService.Dispose()
                taskService = Nothing
            Catch ex As Exception
                Threading.Thread.CurrentThread.CurrentUICulture = New Globalization.CultureInfo("en-US")
                exceptionHandler.manuallyLoadCrashWindow(ex, ex.Message, ex.StackTrace, ex.GetType)
            End Try
        End Sub

        Public Function parseSystemRestorePointCreationDate(strDate As String, Optional boolFullDateParsing As Boolean = True) As Date
            ' Parses out the date.  This is the old way of doing the below code with the use of a Regular Expression.
            'year = Mid$(strDate, 1, 4)
            'month = Mid$(strDate, 5, 2)
            'day = Mid$(strDate, 7, 2)

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

        Public Function doesAtUserLoginTaskExist(ByRef delayedTime As Short) As Boolean
            Try
                Dim nameOfTask As String = globalVariables.taskFolder & "\Create a Restore Point at User Logon"
                Dim taskObject As Task

                Using taskServiceObject As TaskService = New TaskService() ' Creates a new instance of the TaskService.
                    taskObject = taskServiceObject.GetTask(nameOfTask) ' Gets the task.

                    ' Makes sure that the task exists and we don't get a Null Reference Exception.
                    If taskObject IsNot Nothing Then
                        ' Makes sure that we have some triggers to actually work with.
                        If taskObject.Definition.Triggers.Count > 0 Then
                            ' Good, we have some triggers to work with.
                            Dim trigger As Trigger = taskObject.Definition.Triggers.Item(0) ' Stores the trigger object in a Trigger type variable.

                            ' Checks to see if the trigger type is a user logon type.
                            If trigger.TriggerType = TaskTriggerType.Logon Then
                                ' Yes, it is. So we go on.

                                ' Checks to see if the trigger is a delayed trigger.
                                If (TypeOf trigger Is ITriggerDelay) Then
                                    ' Yes, it is. So we go on.

                                    ' Gets the delayed time and stores it in the ByRef delayedTime which is a pointer to a variable that's passed to this function.
                                    delayedTime = DirectCast(taskObject.Definition.Triggers.Item(0), ITriggerDelay).Delay.Minutes
                                Else
                                    ' No, it's not a delayed trigger so we have to give the delayedTime pointer variable as value of 0.
                                    delayedTime = 0
                                End If

                                ' OK, so we know that the trigger is a At User Logon type so this is a valid task so we return True.
                                Return True
                            Else
                                ' The trigger type doesn't match what we are looking for so this is an invalid task and so we give the delayedTime pointer variable a value of 0 and return False.
                                delayedTime = 0
                                Return False
                            End If
                        Else
                            ' We don't even have a trigger so this is an invalid task and so we give the delayedTime pointer variable a value of 0 and return False.
                            delayedTime = 0
                            Return False
                        End If
                    Else
                        ' The task doesn't exist so we give the delayedTime pointer variable a value of 0 and return False.
                        delayedTime = 0
                        Return False
                    End If
                End Using
            Catch ex As Exception
                ' Something we went wrong so we give the delayedTime pointer variable a value of 0 and return False.
                delayedTime = 0
                Return False
            End Try
        End Function

        ' This function returns a Date Object, you would call this function with the unique numerical ID of a Restore Point.
        ' Dim dateOfRestorePoint As Date = getNewestSystemRestorePointDate(2)

        ''' <summary>Returns the creation date of the System Restore Point ID you pass to the function.</summary>
        ''' <param name="restorePointID">Data type, Short. The ID of the System Restore Point you want to return the creation date of.</param>
        ''' <returns>A Date Object.</returns>
        Public Function getSystemRestorePointDate(restorePointID As Short) As Date
            Try
                Dim newestSystemRestoreID As Integer = 0 ' Resets the newest System Restore ID to 0.

                ' Get all System Restore Points from the Windows Management System and puts then in the systemRestorePoints variable.
                Dim systemRestorePoints As New ManagementObjectSearcher("root\DEFAULT", "SELECT * FROM SystemRestore WHERE SequenceNumber = " & restorePointID)

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

        ''' <summary>Adds a scheduled task to launch this program with elevated user rights without a UAC prompt.</summary>
        ''' <param name="taskName">The name of the task.</param>
        ''' <param name="taskDescription">The description of the task.</param>
        ''' <param name="taskEXEPath">The path the executable that this task will invoke.</param>
        ''' <param name="taskParameters">Any and all command line arguments that the invoked executable will be launched with.</param>
        Private Sub addRunTimeTask(taskName As String, taskDescription As String, taskEXEPath As String, taskParameters As String)
            ' Apparently the SYSTEM user can't create scheduled tasks, so to prevent crashes we check to
            ' see if we are running in the SYSTEM user context and if we are we abort this subroutine.
            If Functions.areWeRunningAsSystemUser() = True Then
                Exit Sub
            End If

            Try
                taskName = taskName.Trim & " (For User " & Environment.UserName & ")"
                taskDescription = taskDescription.Trim
                taskEXEPath = taskEXEPath.Trim
                taskParameters = taskParameters.Trim

                writeToSystemEventLog("Creating task """ & taskName & """", EventLogEntryType.Information)

                If IO.File.Exists(taskEXEPath) = False Then
                    MsgBox("Executable path not found.", MsgBoxStyle.Critical, "Restore Point Creator")
                    Exit Sub
                End If

                Dim taskService As TaskService = New TaskService()
                Dim newTask As TaskDefinition = taskService.NewTask

                newTask.RegistrationInfo.Description = taskDescription

                Dim exeFileInfo As New FileInfo(taskEXEPath)

                newTask.Actions.Add(New ExecAction(Chr(34) & taskEXEPath & Chr(34), taskParameters, exeFileInfo.DirectoryName))

                'If parameters = Nothing Then
                '    newTask.Actions.Add(New ExecAction(Chr(34) & txtEXEPath.Text & Chr(34), Nothing, exeFileInfo.DirectoryName))
                'Else
                '    newTask.Actions.Add(New ExecAction(Chr(34) & txtEXEPath.Text & Chr(34), parameters, exeFileInfo.DirectoryName))
                'End If

                newTask.Principal.RunLevel = TaskRunLevel.Highest

                With newTask.Settings
                    .Compatibility = TaskCompatibility.V2
                    .AllowDemandStart = True
                    .DisallowStartIfOnBatteries = False
                    .RunOnlyIfIdle = False
                    .StopIfGoingOnBatteries = False
                    .AllowHardTerminate = False
                    '.UseUnifiedSchedulingEngine = False
                    .ExecutionTimeLimit = Nothing
                End With

                newTask.Principal.LogonType = TaskLogonType.InteractiveToken

                If doesTaskFolderExist() = False Then
                    taskService.RootFolder.CreateFolder(globalVariables.taskFolder)
                End If

                taskService.RootFolder.SubFolders(globalVariables.taskFolder).RegisterTaskDefinition(taskName, newTask)

                newTask.Dispose()
                taskService.Dispose()
                newTask = Nothing
                taskService = Nothing
            Catch ex As Exception
                Functions.writeCrashToEventLog(ex)
                'Microsoft.Win32.Registry.LocalMachine.OpenSubKey(GlobalVariables.strProgramRegistryKey, True).SetValue("No Task", "True", Microsoft.Win32.RegistryValueKind.String)
                'reRunAsAdmin()
            End Try
        End Sub

        ''' <summary>Deletes a scheduled task.</summary>
        ''' <param name="taskName">The task name to be deleted.</param>
        ''' <param name="taskFolder">The task folder in which the task can be found. The default input is "root".</param>
        Private Sub deleteTask(taskName As String, Optional taskFolder As String = constStringRoot)
            Try
                Using taskServiceObject As TaskService = New TaskService()
                    If taskFolder = constStringRoot Then
                        taskServiceObject.RootFolder.DeleteTask(taskName)
                    Else
                        taskServiceObject.RootFolder.SubFolders(taskFolder).DeleteTask(taskName)
                    End If
                End Using
            Catch ex As Exception
            End Try
        End Sub

        ''' <summary>Enables the built-in System Restore Point task that appears to be the culprit in the disappearance of restore points on Windows 8.x and Windows 10.</summary>
        Public Sub enableBuiltInRestorePointTask()
            Try
                Dim taskService As New TaskService()
                Dim taskObject As Task = taskService.GetTask("Microsoft\Windows\SystemRestore\SR")

                If taskObject IsNot Nothing Then
                    If taskObject.Enabled = False Then
                        taskObject.Definition.Settings.Enabled = True
                        taskObject.RegisterChanges()
                        writeToSystemEventLog("Enabled built-in Windows System Restore Task.", EventLogEntryType.Information)
                    End If

                    taskObject.Dispose()
                End If

                taskService.Dispose()
            Catch ex As Exception
                writeToSystemEventLog("Unable to disable built-in Windows System Restore Task.", EventLogEntryType.Error)
                writeCrashToEventLog(ex)
            End Try
        End Sub

        ''' <summary>Disables the built-in System Restore Point task that appears to be the culprit in the disappearance of restore points on Windows 8.x and Windows 10.</summary>
        Public Sub disableBuiltInRestorePointTask()
            Try
                Dim taskService As New TaskService()
                Dim taskObject As Task = taskService.GetTask("Microsoft\Windows\SystemRestore\SR")

                If taskObject IsNot Nothing Then
                    If taskObject.Enabled = True Then
                        taskObject.Definition.Settings.Enabled = False
                        taskObject.RegisterChanges()
                        writeToSystemEventLog("Disabled built-in Windows System Restore Task.", EventLogEntryType.Information)
                    End If

                    taskObject.Dispose()
                End If

                taskService.Dispose()
            Catch ex As Exception
                writeToSystemEventLog("Unable to disable built-in Windows System Restore Task.", EventLogEntryType.Error)
                writeCrashToEventLog(ex)
            End Try
        End Sub

        ''' <summary>Checks to see if a run time task to launch this program with elevated user rights without a UAC prompt exists.</summary>
        ''' <param name="nameOfTask">The name of the task to check if it exists.</param>
        ''' <param name="taskObject">This is a ByRef argument that passes back the Task Object to be used by the calling function.</param>
        ''' <returns>Returns a Boolean value. If the task exists it returns TRUE that you can then use the ByRef Task Object to start.</returns>
        Private Function doesRunTimeTaskExist(ByVal nameOfTask As String, ByRef taskObject As Task) As Boolean
            nameOfTask = globalVariables.taskFolder & "\" & nameOfTask & " (For User " & Environment.UserName & ")"

            Try
                Using taskServiceObject As TaskService = New TaskService()
                    taskObject = taskServiceObject.GetTask(nameOfTask)

                    If taskObject Is Nothing Then
                        Return False
                    Else
                        If taskObject.Definition.Actions.Count = 1 Then
                            Dim exePathInTask As String = Regex.Match(taskObject.Definition.Actions(0).ToString, "((?:""|'){0,1}[A-Za-z]:\\.*\.(?:bat|bin|cmd|com|cpl|exe|gadget|inf1|ins|inx|isu|job|jse|lnk|msc|msi|msp|mst|paf|pif|ps1|reg|rgs|sct|shb|shs|u3p|vb|vbe|vbs|vbscript|ws|wsf)(?:""|'){0,1} {0,1})(.*)", RegexOptions.IgnoreCase).Groups(1).Value.Replace(Chr(34), "").Trim

                            If System.IO.File.Exists(exePathInTask) = True Then
                                Return True
                            Else
                                deleteTask(nameOfTask & " (For User " & Environment.UserName & ")", globalVariables.taskFolder)
                                Return False
                            End If
                        ElseIf taskObject.Definition.Actions.Count = 0 Then
                            deleteTask(nameOfTask & " (For User " & Environment.UserName & ")", globalVariables.taskFolder)
                            Return False
                        Else
                            deleteTask(nameOfTask & " (For User " & Environment.UserName & ")", globalVariables.taskFolder)
                            Return False
                        End If
                    End If
                End Using
            Catch ex As Exception
                Return False
            End Try
        End Function

        Public Function doesTaskFolderExist() As Boolean
            ' Apparently the SYSTEM user can't create scheduled tasks, so to prevent crashes we check to see if
            ' we are running in the SYSTEM user context and if we are we abort this subroutine by returning a
            ' True value even though the Task Folder may not actually exist.
            If areWeRunningAsSystemUser() = True Then
                Return True
            End If

            Try
                Return TaskService.Instance.RootFolder.SubFolders.Exists(globalVariables.taskFolder)
            Catch ex As Exception
                Return False
            End Try
        End Function

        ''' <summary>Creates the run time tasks to launch this program with elevated user rights without a UAC prompt.</summary>
        Public Sub createRunTimeTasksSubRoutine()
            ' Apparently the SYSTEM user can't create scheduled tasks, so to prevent crashes we check to
            ' see if we are running in the SYSTEM user context and if we are we abort this subroutine.
            If areWeRunningAsSystemUser() = True Then
                Exit Sub
            End If

            Try
                Dim task As Task = Nothing

                If doesRunTimeTaskExist("Restore Point Creator -- Run with no UAC (Create Restore Point)", task) = False Then
                    addRunTimeTask("Restore Point Creator -- Run with no UAC (Create Restore Point)", "Runs Restore Point Creator with no UAC prompt.", Application.ExecutablePath, "-createrestorepoint")
                End If
                If doesRunTimeTaskExist("Restore Point Creator -- Run with no UAC (Create Custom Restore Point)", task) = False Then
                    addRunTimeTask("Restore Point Creator -- Run with no UAC (Create Custom Restore Point)", "Runs Restore Point Creator with no UAC prompt.", Application.ExecutablePath, "-createrestorepointcustomname")
                End If
                If doesRunTimeTaskExist("Restore Point Creator -- Run with no UAC", task) = False Then
                    addRunTimeTask("Restore Point Creator -- Run with no UAC", "Runs Restore Point Creator with no UAC prompt.", Application.ExecutablePath, "")
                End If
            Catch ex As Exception
                writeCrashToEventLog(ex)
                ' Silently handle the exception.
            End Try
        End Sub

        ''' <summary>Creates a Window Shortcut.</summary>
        ''' <param name="locationOfShortcut">The location where we want to create the Windows shortcut.</param>
        ''' <param name="pathToExecutable">The path to the executable that the shortcut will launch.</param>
        ''' <param name="iconPath">The path to the icon that the new shortcut will use.</param>
        ''' <param name="Title">The name of the new shortcut.</param>
        ''' <param name="arguments">Any and all command line arguments that the invoked executable will be launched with.</param>
        Public Sub createShortcut(ByVal locationOfShortcut As String, pathToExecutable As String, iconPath As String, ByVal Title As String, Optional arguments As String = Nothing)
            Try
                Dim WshShell As New WshShell
                ' short cut files have a .lnk extension
                Dim shortCut As IWshShortcut = DirectCast(WshShell.CreateShortcut(locationOfShortcut), IWshShortcut)

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

        ''' <summary>Runs this program with elevated user rights without a UAC prompt by running the appropriate scheduled task.</summary>
        Public Sub runAsTask()
            ' We're going to store the result of the areWeAnAdministrator() call in this Boolean variable for later use in this code block.
            Dim boolAreWeRunningAsAdministrator As Boolean = areWeAnAdministrator()

            ' To make sure that nothing goes wrong... we wrap this code in a TRY block.
            Try
                If Debugger.IsAttached = False Then ' Checks to see if a debugger is attached to the current process.
                    Dim boolNoTask As Boolean ' Create a Boolean data type variable.

                    ' Checks to see if our Registry key exists to make sure we don't get any Null Reference Exceptions.
                    If Registry.LocalMachine.OpenSubKey(globalVariables.strProgramRegistryKey) IsNot Nothing Then
                        ' Reads a special Registry entry from the Registry that instructs the program to not run with the Task Wrapper.
                        If Boolean.TryParse(Registry.LocalMachine.OpenSubKey(globalVariables.strProgramRegistryKey).GetValue("No Task", "False"), boolNoTask) Then
                            ' Checks to see if the Registry value was True and if we aren't an Admin.
                            If boolNoTask = True And boolAreWeRunningAsAdministrator = False Then
                                reRunWithAdminUserRights() ' OK, we relaunch the process with full Administrator privileges with a UAC prompt.
                            End If
                        End If
                    End If

                    ' Checks to see if this application's executable is in a safe place, in this case... Program Files.
                    If Application.ExecutablePath.ToLower.Contains("program files") = True Then
                        ' Yes, it is... so let's continue.

                        ' OK, we need to make sure that we are currently running with Administrator privileges.
                        If boolAreWeRunningAsAdministrator = True Then
                            ' Yes, we are.

                            ' Let's check to see if the tasks exist or not.  If they don't, let's create them.  We have a total of three tasks we need to create.
                            createRunTimeTasksSubRoutine()
                            ' Done creating the tasks.  So from this point on we keep on running as normal.
                        ElseIf boolAreWeRunningAsAdministrator = False Then
                            ' No we are not.  So we are either going to relaunch the program as a task or prompt the user with a UAC prompt.

                            Dim task As Task = Nothing ' Creates a task object.

                            ' Checks to see if the user is running one of the application shortcuts for this program.
                            If My.Application.CommandLineArgs.Count = 1 Then
                                ' Yes, one of the application shortcuts for this program was used.  Let's find out which one, shall we.

                                Dim commandLineArgument As String = My.Application.CommandLineArgs(0).ToLower.Trim ' Stores the contents of the command line argument for later use in this code.

                                ' Handles the creation of a restore point from the command line or shortcut.
                                If commandLineArgument = "-createrestorepoint" Then
                                    ' Checks to see if the task exists and returns both a Boolean value and a task object.
                                    If doesRunTimeTaskExist("Restore Point Creator -- Run with no UAC (Create Restore Point)", task) = True Then
                                        task.Run() ' Yep, the task exists so we run it.
                                        Process.GetCurrentProcess.Kill() ' We kill the current un-elevated process.
                                    Else
                                        reRunWithAdminUserRights() ' OK, we relaunch the process with full Administrator privileges with a UAC prompt.
                                    End If
                                ElseIf commandLineArgument = "-createrestorepointcustomname" Then
                                    ' Checks to see if the task exists and returns both a Boolean value and a task object.
                                    If doesRunTimeTaskExist("Restore Point Creator -- Run with no UAC (Create Custom Restore Point)", task) = True Then
                                        task.Run() ' Yep, the task exists so we run it.
                                        Process.GetCurrentProcess.Kill() ' We kill the current un-elevated process.
                                    Else
                                        reRunWithAdminUserRights() ' OK, we relaunch the process with full Administrator privileges with a UAC prompt.
                                    End If
                                ElseIf commandLineArgument = "-forceuac" And boolAreWeRunningAsAdministrator = False Then
                                    reRunWithAdminUserRights() ' OK, we relaunch the process with full Administrator privileges with a UAC prompt.
                                End If
                            Else
                                ' Nope, the user is running it normally without a command line argument.

                                ' Checks to see if we have our task in the system.
                                If doesRunTimeTaskExist("Restore Point Creator -- Run with no UAC", task) = True Then
                                    task.Run() ' Yep, the task exists so we run it.
                                    Process.GetCurrentProcess.Kill() ' We kill the current un-elevated process.
                                Else
                                    reRunWithAdminUserRights() ' OK, we relaunch the process with full Administrator privileges with a UAC prompt.
                                End If
                            End If
                        End If
                    Else
                        ' No, so let's use the UAC prompt because we can't tell if the user is going to move this executable or not.  So to be on the safe side, we don't create the no UAC tasks.
                        If boolAreWeRunningAsAdministrator = False Then reRunWithAdminUserRights()
                    End If
                End If
            Catch ex As Exception
                writeCrashToEventLog(ex)
                ' Something went very wrong, so let's just try and re-launch this program with standard Administrator privileges with a UAC prompt.
                If boolAreWeRunningAsAdministrator = False Then reRunWithAdminUserRights()
            End Try
        End Sub

        Public Sub reRunWithAdminUserRights()
            Try
                ' Old Code, kept in case the new code doesn't work out.
                ' Dim matches As Match = Regex.Match(Environment.CommandLine, "(""{0,1}[A-Za-z]:\\.*\.(?:bat|bin|cmd|com|cpl|exe|gadget|inf1|ins|inx|isu|job|jse|lnk|msc|msi|msp|mst|paf|pif|ps1|reg|rgs|sct|shb|shs|u3p|vb|vbe|vbs|vbscript|ws|wsf)""{0,1} )(.*)", RegexOptions.IgnoreCase)

                Dim startInfo As New ProcessStartInfo

                Dim executablePathPathInfo As New FileInfo(Application.ExecutablePath)
                startInfo.FileName = executablePathPathInfo.FullName
                executablePathPathInfo = Nothing

                ' Old Code, kept in case the new code doesn't work out.
                ' If matches.Groups(2).Value.Trim <> Nothing Then
                '     startInfo.Arguments = matches.Groups(2).Value.Trim
                ' End If

                ' New Code
                If Environment.GetCommandLineArgs.Count <> 1 Then
                    startInfo.Arguments = Environment.GetCommandLineArgs(1)
                End If
                ' New Code

                If areWeAnAdministrator() = False Then startInfo.Verb = "runas"

                Process.Start(startInfo)
                Process.GetCurrentProcess.Kill()
            Catch ex As Win32Exception
                writeCrashToEventLog(ex)
                MsgBox("There was an error while attempting to elevate the process, please make sure that when the Windows UAC prompt appears asking you to run the program with elevated privileges that you say ""Yes"" to the UAC prompt." & vbCrLf & vbCrLf & "The program will now terminate.", MsgBoxStyle.Critical, globalVariables.programName)
                Process.GetCurrentProcess.Kill()
            End Try
        End Sub

        Public Sub createScheduledSystemRestorePoint(strRestorePointDescription As String)
            Try
                Dim integerNewRestorePointID As Integer

                createRestorePoint(strRestorePointDescription, RestoreType.WindowsType, integerNewRestorePointID)
            Catch ex As Exception
                writeCrashToEventLog(ex)
                Process.GetCurrentProcess.Kill()
            End Try
        End Sub

        Private Function getNewestSystemRestorePointIDMadeByRestorePointCreator(strRestorePointDescription As String) As Integer
            Try
                Dim newestSystemRestoreID As Integer = 0 ' Resets the newest System Restore ID to 0.

                ' Get all System Restore Points from the Windows Management System and puts then in the systemRestorePoints variable.
                Dim systemRestorePoints As New ManagementObjectSearcher("root\DEFAULT", "SELECT * FROM SystemRestore")

                Dim systemRestoreIDs As New ArrayList
                Dim restorePointNameOnSystem As String

                ' Checks to see if there are any System Restore Points to be listed.
                If systemRestorePoints.Get().Count <> 0 Then
                    ' Loops through systemRestorePoints.
                    For Each systemRestorePoint As ManagementObject In systemRestorePoints.Get()
                        restorePointNameOnSystem = systemRestorePoint("Description").ToString.Trim

                        ' We need only the Restore Point IDs made by this program, nothing else.
                        If restorePointNameOnSystem = globalVariables.strDefaultNameForScheduledRestorePoint Or restorePointNameOnSystem = strRestorePointDescription Then
                            systemRestoreIDs.Add(systemRestorePoint("SequenceNumber").ToString)
                        End If
                    Next

                    If systemRestoreIDs.Count = 0 Then
                        Return 0
                    Else
                        systemRestoreIDs.Sort()
                        Return Integer.Parse(systemRestoreIDs.Item(systemRestoreIDs.Count - 1))
                    End If
                Else
                    Return 0
                End If

                'Return newestSystemRestoreID
            Catch ex As Exception
                Return 0
            End Try
        End Function

        Public Sub updateScheduledRestorePointCreationTaskWithEverySetting()
            Try
                ' First we get the Every setting from the Registry. If the value in the Registry is missing we populate the value for it in the program with "nothing".
                Dim strEvery As String = Registry.LocalMachine.OpenSubKey(globalVariables.strProgramRegistryKey, False).GetValue("Every", Nothing)

                ' Checks to see if the value in the Registry existed, if it exists then it shouldn't equal "nothing".
                If strEvery <> Nothing Then
                    ' Good, the Every setting existed in the Registry. So let's continue the work here.

                    Dim shortEvery As Short ' Creates a Short variable with a name of everyShort.

                    ' Now we try and parse the setting from the Registry from a String to a Short.
                    If Short.TryParse(strEvery, shortEvery) = True Then
                        ' Good, the Short parsing worked.

                        Dim taskService As New TaskService ' Creates a new TaskService Object.
                        Dim taskWeAreWorkingWith As Task = taskService.GetTask("\System Restore Checkpoint by System Restore Point Creator") ' Gets our Task.

                        ' First we need to make sure if the Task even exists, we do this by doing an IsNot Nothing check to make sure we don't have a Null Reference Exception.
                        If taskWeAreWorkingWith IsNot Nothing Then
                            If taskWeAreWorkingWith.Definition.Triggers.Count <> 0 Then ' Now we make sure our task has triggers to work with.
                                ' Checks to see if the Trigger is a daily Trigger.
                                If taskWeAreWorkingWith.Definition.Triggers(0).TriggerType = TaskTriggerType.Daily Then
                                    Dim taskTime As Date = taskWeAreWorkingWith.Definition.Triggers(0).StartBoundary ' Saves the task start time for later use.
                                    taskWeAreWorkingWith.Definition.Triggers.Remove(taskWeAreWorkingWith.Definition.Triggers(0)) ' Removes the Trigger.
                                    taskWeAreWorkingWith.Definition.Triggers.Add(New DailyTrigger(shortEvery) With {.StartBoundary = taskTime}) ' Creates a new Trigger but with a Every setting.
                                    taskWeAreWorkingWith.RegisterChanges() ' Saves the changes to the task.

                                    Registry.LocalMachine.OpenSubKey(globalVariables.strProgramRegistryKey, True).DeleteValue("Every", False) ' Deletes the Every setting from the Registry.

                                    ' Add an Event Log Entry stating the scheduled task was upgraded successfully.
                                    writeToSystemEventLog("The upgrade of the scheduled task was successful.", EventLogEntryType.Information)
                                End If
                            End If

                            taskWeAreWorkingWith.Dispose() ' Disposes of our Task Object.
                            taskWeAreWorkingWith = Nothing
                        End If

                        taskService.Dispose() ' Disposes of our TaskService Object.
                        taskService = Nothing
                    End If
                End If
            Catch ex As Exception
                ' OK, we have some errors so lets add them to the Event Log.
                writeToSystemEventLog("An error occurred while upgrading scheduled task.", EventLogEntryType.Error)
                writeCrashToEventLog(ex)
            End Try
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

        ''' <summary>Writes a log entry to the System Event Log.</summary>
        ''' <param name="logMessage">The text you want to have in your new System Event Log entry.</param>
        ''' <param name="logType">The type of log that you want your entry to be. The three major options are Error, Information, and Warning.</param>
        ''' <example>functions.writeToSystemEventLog("My Event Log Entry", EventLogEntryType.Information)</example>
        Public Sub writeToSystemEventLog(logMessage As String, Optional logType As EventLogEntryType = EventLogEntryType.Information)
            If globalVariables.boolLogToSystemLog = True Then
                Try
                    Dim logSource As String = "System Restore Point Creator"
                    Dim logName As String = "Application"
                    Dim host As String = "."

                    'Dim registryKey As Microsoft.Win32.RegistryKey = Microsoft.Win32.Registry.LocalMachine.OpenSubKey("SYSTEM\CurrentControlSet\services\eventlog\Application", True)

                    'If registryKey.OpenSubKey(sSource) Is Nothing Then
                    '    registryKey.CreateSubKey(sSource)
                    '    registryKey.OpenSubKey(sSource, True).SetValue("EventMessageFile", IO.Path.Combine(System.Runtime.InteropServices.RuntimeEnvironment.GetRuntimeDirectory(), "EventLogMessages.dll"), Microsoft.Win32.RegistryValueKind.String)
                    'End If

                    'registryKey.Close()
                    'registryKey.Dispose()
                    'registryKey = Nothing

                    If Registry.LocalMachine.OpenSubKey("SYSTEM\CurrentControlSet\services\eventlog\Application\" & logSource) Is Nothing Then
                        Registry.LocalMachine.OpenSubKey("SYSTEM\CurrentControlSet\services\eventlog\Application", True).CreateSubKey(logSource)
                        Registry.LocalMachine.OpenSubKey("SYSTEM\CurrentControlSet\services\eventlog\Application\" & logSource, True).SetValue("EventMessageFile", IO.Path.Combine(RuntimeEnvironment.GetRuntimeDirectory(), "EventLogMessages.dll"), RegistryValueKind.String)
                    Else
                        If Registry.LocalMachine.OpenSubKey("SYSTEM\CurrentControlSet\services\eventlog\Application\" & logSource, False).GetValue("EventMessageFile", Nothing) Is Nothing Then
                            Registry.LocalMachine.OpenSubKey("SYSTEM\CurrentControlSet\services\eventlog\Application\" & logSource, True).SetValue("EventMessageFile", IO.Path.Combine(RuntimeEnvironment.GetRuntimeDirectory(), "EventLogMessages.dll"), RegistryValueKind.String)
                        End If
                    End If

                    If Not EventLog.SourceExists(logSource, host) Then
                        EventLog.CreateEventSource(logSource, logName, host)
                    End If

                    Dim eventLogObject As New EventLog(logName, host, logSource)
                    eventLogObject.WriteEntry(logMessage, logType, 234, CType(3, Short))

                    eventLogObject.Dispose()
                    eventLogObject = Nothing
                Catch ex As Exception
                    ' Does nothing
                End Try
            End If
        End Sub

        Public Sub addAtUserLoginTask(Optional delayed As Boolean = False, Optional delayTimeInMinutes As Short = 10)
            Dim taskService As New TaskService()
            Dim newTask As TaskDefinition = taskService.NewTask

            newTask.RegistrationInfo.Description = "Creates a Restore Point at User Logon"

            Dim logonTriggerDefinition As LogonTrigger = New LogonTrigger

            If delayed = True Then
                logonTriggerDefinition.Delay = New TimeSpan(0, 0, delayTimeInMinutes, 0, 0)
            End If

            newTask.Triggers.Add(logonTriggerDefinition)

            Dim exePathInfo As New FileInfo(Application.ExecutablePath)
            newTask.Actions.Add(New ExecAction(Chr(34) & exePathInfo.FullName & Chr(34), "-createscheduledrestorepoint", exePathInfo.DirectoryName))
            exePathInfo = Nothing

            newTask.Principal.RunLevel = TaskRunLevel.Highest

            With newTask.Settings
                .Compatibility = TaskCompatibility.V2
                .AllowDemandStart = True
                .DisallowStartIfOnBatteries = False
                .RunOnlyIfIdle = False
                .StopIfGoingOnBatteries = False
                .AllowHardTerminate = False
                .ExecutionTimeLimit = Nothing
            End With

            newTask.Principal.LogonType = TaskLogonType.InteractiveToken

            If doesTaskFolderExist() = False Then
                taskService.RootFolder.CreateFolder(globalVariables.taskFolder)
            End If

            Dim taskName As String = "Create a Restore Point at User Logon"
            taskService.RootFolder.SubFolders(globalVariables.taskFolder).RegisterTaskDefinition(taskName, newTask)

            newTask.Dispose()
            taskService.Dispose()
            newTask = Nothing
            taskService = Nothing
        End Sub

        Public Function checkToSeeIfSystemRestoreIsEnabledOnSystemDrive() As Boolean
            Try
                Dim systemRestorePoints As New ManagementObjectSearcher("root\DEFAULT", "SELECT * FROM SystemRestore")

                If systemRestorePoints.Get().Count = 0 Then
                    Return False
                Else
                    Return True
                End If
            Catch ex As Exception
                Return False
            End Try
        End Function

        Public Function createRestorePoint(restorePointName As String, restorePointType As RestoreType, ByRef restorePointID As Long) As Integer
            Try
                If globalVariables.systemDriveLetter = Nothing Then
                    writeToSystemEventLog("Unable to determine system drive letter.", EventLogEntryType.Error)
                    MsgBox("Unable to determine system drive letter.", MsgBoxStyle.Critical, "Error Creating System Restore Point")
                    Return Functions.ERROR_INTERNAL_ERROR
                End If

                Dim shadowStorageDetails As ShadowStorageData = getShadowStorageData(globalVariables.systemDriveLetter)

                Try
                    ' This checks if the system has any existing restore points on the system, if the system doesn't then this functioon returns False since
                    ' more than likely if it doesn't have any restore points then System Restore is somehow broken on this machine and we have to fix it.
                    If checkToSeeIfSystemRestoreIsEnabledOnSystemDrive() = False Then
                        ' OK, it doesn't so now we go and check some other things.

                        ' Checks to see if we have a Null Object.
                        If shadowStorageDetails IsNot Nothing Then
                            ' Nope, so let's do some checking.

                            ' Checks to see if the system drive has any shadow storage space assigned to it.
                            If shadowStorageDetails.MaxSpace = 0 Then
                                ' Nope, so we assign some via the VSSAdmin command.
                                enableSystemRestoreOnDriveVSSAdmin(globalVariables.systemDriveLetter)
                            End If
                        End If

                        ' Now we simply just re-enable System Restore on the main system drive.
                        enableSystemRestoreOnDriveWMI(globalVariables.systemDriveLetter)
                    End If
                Catch ex As NullReferenceException
                    enableSystemRestoreOnDriveVSSAdmin(globalVariables.systemDriveLetter)
                    enableSystemRestoreOnDriveWMI(globalVariables.systemDriveLetter)
                End Try

                Dim managementScopeObject As New ManagementScope("\\localhost\root\default")
                Dim managementPathObject As New ManagementPath("SystemRestore")
                Dim managementObjectOptions As New ObjectGetOptions()
                Dim managementClassObject As New ManagementClass(managementScopeObject, managementPathObject, managementObjectOptions)

                Dim managementBaseObjectParameters As ManagementBaseObject = managementClassObject.GetMethodParameters("CreateRestorePoint")
                managementBaseObjectParameters("Description") = restorePointName
                managementBaseObjectParameters("RestorePointType") = restorePointType
                managementBaseObjectParameters("EventType") = 100

                Dim oOutParams As ManagementBaseObject = managementClassObject.InvokeMethod("CreateRestorePoint", managementBaseObjectParameters, Nothing)
                writeToSystemEventLog("Created System Restore Point (" & restorePointName & ").", EventLogEntryType.Information)
                Return oOutParams("ReturnValue")
            Catch ex4 As UnauthorizedAccessException
                writeCrashToEventLog(ex4)
                writeToSystemEventLog("Unable to create system restore point. System permissions seem to not allow it.", EventLogEntryType.Error)
                MsgBox("Unable to create system restore point. System permissions seem to not allow it.", MsgBoxStyle.Critical, "Error Creating System Restore Point")
                Return 0
            Catch ex3 As COMException
                writeCrashToEventLog(ex3)
                giveComExceptionCrashMessage()
                Return 0
            Catch ex As Exception
                Threading.Thread.CurrentThread.CurrentUICulture = New Globalization.CultureInfo("en-US")
                exceptionHandler.manuallyLoadCrashWindow(ex, ex.Message, ex.StackTrace, ex.GetType)
                Return 0
            End Try
        End Function

        Public Sub restoreToSystemRestorePoint(point As Long)
            Try
                writeToSystemEventLog("Restoring system back to System Restore Point ID " & point & " (" & getRestorePointName(point) & ").", EventLogEntryType.Information)

                Dim managementScopeObject As New ManagementScope("\\localhost\root\default")
                Dim managementPathObject As New ManagementPath("SystemRestore")
                Dim managementObjectOptions As New ObjectGetOptions()
                Dim managementClassObject As New ManagementClass(managementScopeObject, managementPathObject, managementObjectOptions)

                Dim managementBaseObjectParameters As ManagementBaseObject = managementClassObject.GetMethodParameters("Restore")
                managementBaseObjectParameters("SequenceNumber") = point

                Dim oOutParams As ManagementBaseObject = managementClassObject.InvokeMethod("Restore", managementBaseObjectParameters, Nothing)

                'Dim managementObject As New ManagementObject("root\DEFAULT", "SystemRestore.ReplaceKeyPropery='ReplaceKeyPropertyValue'", Nothing)
                'Dim inParams As ManagementBaseObject = managementObject.GetMethodParameters("Restore")
                'inParams("SequenceNumber") = point
                'Dim outParams As ManagementBaseObject = managementObject.InvokeMethod("Restore", inParams, Nothing)
                'rebootSystem()

                'Dim systemRestorePointObject As Object = GetObject("winmgmts:\\.\root\Default:SystemRestore")
                'systemRestorePointObject.Restore(point)

                rebootSystem()
                'System.Diagnostics.Process.Start("shutdown.exe", "-r -t 0")
            Catch ex4 As ArgumentException
                writeCrashToEventLog(ex4)
                MsgBox("Unable to restore system to selected restore point, a COM Exception has occurred. Restore process aborted.", MsgBoxStyle.Critical, "Restore Point Creator")
            Catch ex3 As COMException
                giveComExceptionCrashMessage()
            Catch ex As Exception
                Threading.Thread.CurrentThread.CurrentUICulture = New System.Globalization.CultureInfo("en-US")
                exceptionHandler.manuallyLoadCrashWindow(ex, ex.Message, ex.StackTrace, ex.GetType)
            End Try
        End Sub

        Private Function executeCommandLineAndGetOutput(processExecutable As String, Optional commandLineArguments As String = Nothing) As String
            Dim processObject As New Process()
            Dim processStartSettings As ProcessStartInfo

            If commandLineArguments = Nothing Then
                processStartSettings = New ProcessStartInfo(processExecutable)
            Else
                processStartSettings = New ProcessStartInfo(processExecutable, commandLineArguments)
            End If

            processStartSettings.UseShellExecute = False
            processStartSettings.RedirectStandardOutput = True
            processStartSettings.CreateNoWindow = True

            processObject.StartInfo = processStartSettings
            processObject.Start()

            Return processObject.StandardOutput.ReadToEnd
        End Function

        Public Sub setSafeModeBoot()
            Dim bcdEditor As New editBCDStore.editBCDStore
            bcdEditor.SetSafeboot()
            bcdEditor = Nothing
        End Sub

        Private Sub doTheActualSafeModeBootRemoval()
            Try
                Dim bcdEditor As New editBCDStore.editBCDStore
                bcdEditor.RemoveSafeboot()
                bcdEditor = Nothing
            Catch ex As Exception
                ' We don't care if it crashes here, just let it do it silently.
            End Try
        End Sub

        Public Sub removeSafeModeBoot(Optional boolBypassRegCheck As Boolean = False)
            If boolBypassRegCheck = True Then
                doTheActualSafeModeBootRemoval()
            Else
                If Registry.LocalMachine.OpenSubKey(globalVariables.strProgramRegistryKey) IsNot Nothing Then
                    Dim strRegValue As String = Registry.LocalMachine.OpenSubKey(globalVariables.strProgramRegistryKey).GetValue(globalVariables.strSafeModeBootRegistryValue, "False")
                    Dim boolRegValue As Boolean

                    If Boolean.TryParse(strRegValue, boolRegValue) = True Then
                        If boolRegValue = True Then
                            doTheActualSafeModeBootRemoval()
                        End If
                    End If
                End If
            End If
        End Sub

        Public Sub rebootSystem()
            Dim strPathToShutDown As String = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.System), "shutdown.exe")

            If IO.File.Exists(strPathToShutDown) = True Then
                Shell(strPathToShutDown & " -r -t 0", AppWinStyle.Hide)
            Else
                MsgBox("Unable to find the Windows command line reboot tool to trigger a reboot. You will have to manually trigger a reboot yourself.", MsgBoxStyle.Exclamation, "Restore Point Creator")
            End If

            'Shell("shutdown.exe -r -t 0", AppWinStyle.Hide)

            'Dim t As Single, objWMIService, objComputer As Object
            'objWMIService = GetObject("Winmgmts:{impersonationLevel=impersonate,(Debug,Shutdown)}")

            'For Each objComputer In objWMIService.InstancesOf("Win32_OperatingSystem")
            '    t = objComputer.Win32Shutdown(2 + 4, 0)
            '    If t <> 0 Then
            '        MsgBox("Error occurred!!!")
            '    Else
            '        'LogOff your system
            '    End If
            'Next
        End Sub

        Function getRestorePointName(id As Long) As String
            Try
                Dim systemRestorePoints As New ManagementObjectSearcher("root\DEFAULT", "SELECT * FROM SystemRestore")

                If (systemRestorePoints.Get().Count = 0) = False Then
                    For Each systemRestorePoint As ManagementObject In systemRestorePoints.Get()
                        If Long.Parse(systemRestorePoint("SequenceNumber").ToString) = id Then
                            Return systemRestorePoint("Description").ToString.ToString
                        End If
                    Next
                End If

                writeToSystemEventLog("Unable to find description for restore point ID " & id & ".", EventLogEntryType.Error)
                Return "ERROR_NO_DESCRIPTION"
            Catch ex As Exception
                writeToSystemEventLog("Unable to find description for restore point ID " & id & ".", EventLogEntryType.Error)
                Return "ERROR_NO_DESCRIPTION"
            End Try
        End Function

        Function getNewestSystemRestorePointID() As Integer
            Try
                Dim newestSystemRestoreID As Integer = 0 ' Resets the newest System Restore ID to 0.

                ' Get all System Restore Points from the Windows Management System and puts then in the systemRestorePoints variable.
                Dim systemRestorePoints As New ManagementObjectSearcher("root\DEFAULT", "SELECT * FROM SystemRestore")

                Dim systemRestoreIDs As New ArrayList

                ' Checks to see if there are any System Restore Points to be listed.
                If (systemRestorePoints.Get().Count = 0) = False Then
                    ' Loops through systemRestorePoints.
                    For Each systemRestorePoint As ManagementObject In systemRestorePoints.Get()
                        systemRestoreIDs.Add(systemRestorePoint("SequenceNumber").ToString)
                    Next

                    If systemRestoreIDs.Count <> 0 Then
                        newestSystemRestoreID = Integer.Parse(systemRestoreIDs.Item(systemRestoreIDs.Count - 1))
                    Else
                        newestSystemRestoreID = 0
                    End If
                Else
                    newestSystemRestoreID = 0
                End If

                Return newestSystemRestoreID
            Catch ex3 As UnauthorizedAccessException
                Return 0
            Catch ex2 As ManagementException
                Return 0
            Catch ex4 As ArgumentOutOfRangeException
                Return 0
            Catch ex3 As COMException
                Functions.giveComExceptionCrashMessage()
                Return 0
            End Try
        End Function

        'Public Sub changeLanguage(ByVal lang As String, windowObject As Object)
        '    For Each c As Control In windowObject.Controls
        '        Dim resources As ComponentResourceManager = New ComponentResourceManager(windowObject.GetType)
        '        resources.ApplyResources(c, c.Name, New CultureInfo(lang))
        '    Next c
        'End Sub

        Public Function tryToParseShort(value As String) As Boolean
            Dim number As Short
            Dim result As Boolean = Short.TryParse(value, number)

            If result Then
                Return True
            Else
                Return False
            End If
        End Function

        Function getFileTypeHandler(fileType As String) As String
            Try
                Dim registryKey As RegistryKey = Registry.ClassesRoot.OpenSubKey(fileType, False)

                ' If registryKey Is Nothing = False Then
                If registryKey IsNot Nothing Then
                    Dim fileTypeNameInRegistry As String = registryKey.GetValue("")
                    Dim registryKey2 As RegistryKey = Registry.ClassesRoot.OpenSubKey(fileTypeNameInRegistry & "\shell\open\command", False)

                    ' If registryKey2 Is Nothing = False Then
                    If registryKey2 IsNot Nothing Then
                        Return registryKey2.GetValue("").ToString.Replace("""", "").Replace("%1", "").Trim
                    End If
                End If

                Return ""
            Catch ex As Exception
                Return ""
            End Try
        End Function

        Public Function ResizeImage(SourceImage As Image, TargetWidth As Integer, TargetHeight As Integer) As Bitmap
            Dim bmSource = New Bitmap(SourceImage)
            Return ResizeImage(bmSource, TargetWidth, TargetHeight)
        End Function

        Public Function ResizeImage(bmSource As Bitmap, TargetWidth As Integer, TargetHeight As Integer) As Bitmap
            Dim bmDest As New Bitmap(TargetWidth, TargetHeight, Imaging.PixelFormat.Format32bppArgb)

            Dim nSourceAspectRatio = bmSource.Width / bmSource.Height
            Dim nDestAspectRatio = bmDest.Width / bmDest.Height

            Dim NewX = 0
            Dim NewY = 0
            Dim NewWidth = bmDest.Width
            Dim NewHeight = bmDest.Height

            If nDestAspectRatio = nSourceAspectRatio Then
                'same ratio
            ElseIf nDestAspectRatio > nSourceAspectRatio Then
                'Source is taller
                NewWidth = Convert.ToInt32(Math.Floor(nSourceAspectRatio * NewHeight))
                NewX = Convert.ToInt32(Math.Floor((bmDest.Width - NewWidth) / 2))
            Else
                'Source is wider
                NewHeight = Convert.ToInt32(Math.Floor((1 / nSourceAspectRatio) * NewWidth))
                NewY = Convert.ToInt32(Math.Floor((bmDest.Height - NewHeight) / 2))
            End If

            Using grDest = Graphics.FromImage(bmDest)
                With grDest
                    .CompositingQuality = Drawing2D.CompositingQuality.HighQuality
                    .InterpolationMode = Drawing2D.InterpolationMode.HighQualityBicubic
                    .PixelOffsetMode = Drawing2D.PixelOffsetMode.HighQuality
                    .SmoothingMode = Drawing2D.SmoothingMode.AntiAlias
                    .CompositingMode = Drawing2D.CompositingMode.SourceOver

                    .DrawImage(bmSource, NewX, NewY, NewWidth, NewHeight)
                End With
            End Using

            Return bmDest
        End Function

        ' driveLetter is just that, "C:"
        Function getShadowStorageSize(driveLetter As String) As Long
            Try
                driveLetter = driveLetter.ToUpper
                Dim volumeID As String = getDeviceIDFromDriveLetter(driveLetter)

                Dim wmiQuery As String = String.Format("SELECT * FROM Win32_ShadowStorage WHERE Volume='Win32_Volume.DeviceID=\'{0}\''", volumeID.addSlashes())
                Dim searcher As New ManagementObjectSearcher("root\CIMV2", wmiQuery)
                'Dim searcher As New ManagementObjectSearcher("root\CIMV2", "SELECT * FROM Win32_ShadowStorage")

                If searcher.Get().Count <> 0 Then
                    Return searcher.Get(0)("MaxSpace").ToString
                Else
                    Return 0
                End If
            Catch ex As Exception
                If ex.Message.caseInsensitiveContains("provider failure", False) Then
                    writeCrashToEventLog(ex)
                    giveMessageAboutShadowCopyServiceBeingBroken()
                    Return 0
                Else
                    Return 0
                End If
            End Try
        End Function

        Public Sub giveMessageAboutShadowCopyServiceBeingBroken()
            MsgBox("Shadow Copy Service Provider Error Detected!" & vbCrLf & vbCrLf & "Please reference the following Microsoft Support Knowledge Base Article..." & vbCrLf & globalVariables.providerFailureMicrosoftKnowledgebaseURL & vbCrLf & vbCrLf & "Your default web browser will now open to load the Microsoft Knowledgebase Article mentioned above.", MsgBoxStyle.Critical, "Restore Point Creator")

            launchURLInWebBrowser(globalVariables.providerFailureMicrosoftKnowledgebaseURL)
        End Sub

        ' driveLetter is just that, "C:"
        Function getDriveLabel(driveLetter As String) As String
            Dim wmiQueryObject As New ObjectQuery(String.Format("SELECT * FROM Win32_LogicalDisk Where DeviceID='{0}'", driveLetter.ToUpper.Trim))
            Dim managementObjectSearcher As New ManagementObjectSearcher(wmiQueryObject)

            If managementObjectSearcher.Get.Count = 0 Then
                managementObjectSearcher.Dispose()
                Return Nothing
            Else
                'Debug.WriteLine("managementObjectSearcher.Get.Count = " & managementObjectSearcher.Get.Count)
                Dim managementObjectCollection As ManagementObjectCollection = managementObjectSearcher.Get()

                Dim volumeName As String = managementObjectCollection(0)("VolumeName").ToString

                managementObjectSearcher.Dispose()
                managementObjectCollection.Dispose()

                Return volumeName
            End If
        End Function

        ' driveLetter is just that, "C:"
        Sub setShadowStorageSize(driveLetter As String, size As Long)
            Try
                driveLetter = driveLetter.ToUpper
                Dim volumeID As String = getDeviceIDFromDriveLetter(driveLetter)

                Dim wmiQuery As String = String.Format("SELECT * FROM Win32_ShadowStorage WHERE Volume='Win32_Volume.DeviceID=\'{0}\''", volumeID.addSlashes())
                Dim searcher As New ManagementObjectSearcher("root\CIMV2", wmiQuery)

                If searcher.Get.Count <> 0 Then
                    Dim queryObj As ManagementObject = searcher.Get(0)

                    queryObj("MaxSpace") = size
                    queryObj.Put()
                    queryObj.Dispose()
                    queryObj = Nothing
                End If

                searcher.Dispose()
                searcher = Nothing
            Catch ex2 As ManagementException
                writeCrashToEventLog(ex2)

                Dim msgBoxResult As MsgBoxResult = MsgBox("There was an error attempting to apply your drive space settings. Would you like to try again?", MsgBoxStyle.Question + MsgBoxStyle.YesNo, "Restore Point Creator")

                If msgBoxResult = MsgBoxResult.Yes Then
                    runVSSAdminCommand(driveLetter)
                    setShadowStorageSize(driveLetter, size)
                End If
            Catch ex As Exception
                If ex.Message.caseInsensitiveContains("provider failure", False) = True Then
                    writeCrashToEventLog(ex)
                    giveMessageAboutShadowCopyServiceBeingBroken()
                Else
                    Threading.Thread.CurrentThread.CurrentUICulture = New Globalization.CultureInfo("en-US")
                    exceptionHandler.manuallyLoadCrashWindow(ex, ex.Message, ex.StackTrace, ex.GetType)
                End If
            End Try
        End Sub

        Public Function boolTestRegExPattern(strPattern As String) As Boolean
            Try
                Dim testRegExPattern As New Regex(strPattern)
                Return True
            Catch ex As Exception
                Return False
            End Try
        End Function

        ' driveLetter is just that, "C:"
        Public Function getDeviceIDFromDriveLetter(driveLetter As String) As String
            Try
                Dim wmiQueryObject As New ObjectQuery(String.Format("SELECT * FROM Win32_Volume Where DriveLetter='{0}'", driveLetter.ToUpper.Trim))
                Dim managementObjectSearcher As New ManagementObjectSearcher(wmiQueryObject)

                If managementObjectSearcher.Get.Count = 0 Then
                    managementObjectSearcher.Dispose()
                    Return 0
                Else
                    'Debug.WriteLine("managementObjectSearcher.Get.Count = " & managementObjectSearcher.Get.Count)
                    Dim managementObjectCollection As ManagementObjectCollection = managementObjectSearcher.Get()

                    Dim deviceID As String = managementObjectCollection(0)("DeviceID").ToString

                    managementObjectSearcher.Dispose()
                    managementObjectCollection.Dispose()

                    Return deviceID
                End If
            Catch ex As ManagementException
                writeCrashToEventLog(ex)
                writeToSystemEventLog("Unable to retrieve volumeID from WMI for system drive " & driveLetter & ".", EventLogEntryType.Error)
                MsgBox("Unable to retrieve volumeID from WMI for system drive " & driveLetter & "." & vbCrLf & vbCrLf & "The program will now terminate.", MsgBoxStyle.Critical, "Restore Point Creator")
                Process.GetCurrentProcess.Kill()
                Return 0
            End Try
        End Function

        Public Function areWeRunningAsSystemUser() As Boolean
            'Dim identity As System.Security.Principal.WindowsIdentity = System.Security.Principal.WindowsIdentity.GetCurrent()
            'Return identity.IsSystem
            Return Security.Principal.WindowsIdentity.GetCurrent().IsSystem
        End Function

        Public Function areWeAnAdministrator() As Boolean
            Try
                Dim principal As WindowsPrincipal = New WindowsPrincipal(WindowsIdentity.GetCurrent())

                If principal.IsInRole(WindowsBuiltInRole.Administrator) = True Then
                    Return True
                Else
                    Return False
                End If
            Catch ex As Exception
                Return False
            End Try
        End Function

        Public Function fileSizeToHumanReadableFormat(ByVal size As Long, Optional roundToNearestWholeNumber As Boolean = False) As String
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

        Sub runVSSAdminCommand(driveLetter As String)
            If IO.File.Exists(Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.System), "vssadmin.exe")) = True Then
                Dim runningProcess As Process
                Dim startInfo As New ProcessStartInfo
                startInfo.FileName = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.System), "vssadmin.exe")
                startInfo.Arguments = String.Format("Resize ShadowStorage /For={0} /On={0} /MaxSize=20%", driveLetter)
                startInfo.Verb = "runas"
                startInfo.UseShellExecute = False
                startInfo.CreateNoWindow = True
                runningProcess = Process.Start(startInfo)
                runningProcess.WaitForExit()
            Else
                MsgBox("Unable to find the VSSAdmin utility.", MsgBoxStyle.Critical, "Restore Point Creator")
            End If
        End Sub

        Public Sub enableSystemRestoreOnDriveVSSAdmin(driveLetter As String)
            If IO.File.Exists(Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.System), "vssadmin.exe")) = True Then
                Dim runningProcess As Process
                'runningProcess = Process.Start("vssadmin", String.Format("Resize ShadowStorage /For={0} /On={0} /MaxSize=1%", driveLetter))
                'Else
                Dim startInfo As New ProcessStartInfo
                startInfo.FileName = IO.Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.System), "vssadmin.exe")
                startInfo.Arguments = String.Format("Resize ShadowStorage /For={0} /On={0} /MaxSize=20%", driveLetter)
                startInfo.Verb = "runas"
                startInfo.UseShellExecute = False
                startInfo.CreateNoWindow = True
                runningProcess = Process.Start(startInfo)
                runningProcess.WaitForExit()

                writeToSystemEventLog("No system restore point storage space was assigned for the system drive, this issue has been corrected.", EventLogEntryType.Information)
            Else
                MsgBox("Unable to find the VSSAdmin utility.", MsgBoxStyle.Critical, "Restore Point Creator")
            End If
        End Sub

        Public Function loadPleaseWaitIcon() As Icon
            Dim bitMap As New Bitmap(My.Resources.chronometer)
            Dim icon As Icon = Icon.FromHandle(bitMap.GetHicon)
            bitMap.Dispose()
            Return icon
        End Function

        ''' <summary>Gets the Shadow Storage Information for a particular storage device in the system.</summary>
        ''' <param name="volumeID">This input can accept either a storage device GUID or a drive letter such as "C:".</param>
        ''' <returns>ShadowStorageData Class</returns>
        Function getShadowStorageData(volumeID As String) As ShadowStorageData ' Accepts both a Volume ID and a drive letter such as "C:".
            If volumeID.Length = 2 Then
                volumeID = getDeviceIDFromDriveLetter(volumeID)
            End If

            Try
                Dim wmiQuery As String = String.Format("SELECT * FROM Win32_ShadowStorage WHERE Volume='Win32_Volume.DeviceID=\'{0}\''", volumeID.addSlashes())
                Dim searcher As New ManagementObjectSearcher("root\CIMV2", wmiQuery)

                If searcher.Get.Count <> 0 Then
                    Dim queryObj As ManagementObject = searcher.Get(0)
                    Dim shadowStorageDataInstance As New ShadowStorageData

                    ' This is all in an effort to try and prevent Null Reference Exceptions.
                    If queryObj("AllocatedSpace") Is Nothing Then
                        shadowStorageDataInstance.AllocatedSpace = 0
                    Else
                        If Long.TryParse(queryObj("AllocatedSpace"), shadowStorageDataInstance.AllocatedSpace) = False Then
                            shadowStorageDataInstance.AllocatedSpace = 0
                        End If
                    End If

                    ' This is all in an effort to try and prevent Null Reference Exceptions.
                    If queryObj("DiffVolume") Is Nothing Then
                        shadowStorageDataInstance.DiffVolume = Nothing
                    Else
                        shadowStorageDataInstance.DiffVolume = queryObj("DiffVolume")
                    End If

                    ' This is all in an effort to try and prevent Null Reference Exceptions.
                    If queryObj("MaxSpace") Is Nothing Then
                        shadowStorageDataInstance.MaxSpace = 0
                    Else
                        If Long.TryParse(queryObj("MaxSpace"), shadowStorageDataInstance.MaxSpace) = False Then
                            shadowStorageDataInstance.MaxSpace = 0
                        End If
                    End If

                    ' This is all in an effort to try and prevent Null Reference Exceptions.
                    If queryObj("UsedSpace") Is Nothing Then
                        shadowStorageDataInstance.UsedSpace = 0
                    Else
                        If Long.TryParse(queryObj("UsedSpace"), shadowStorageDataInstance.UsedSpace) = False Then
                            shadowStorageDataInstance.UsedSpace = 0
                        End If
                    End If

                    ' This is all in an effort to try and prevent Null Reference Exceptions.
                    If queryObj("Volume") Is Nothing Then
                        shadowStorageDataInstance.Volume = Nothing
                    Else
                        shadowStorageDataInstance.Volume = queryObj("Volume")
                    End If

                    searcher.Dispose()
                    queryObj.Dispose()
                    searcher = Nothing
                    queryObj = Nothing

                    Return shadowStorageDataInstance
                End If

                searcher.Dispose()
                searcher = Nothing

                Return Nothing
            Catch ex As Exception
                Return New ShadowStorageData
            End Try
        End Function

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

        Function getActivePowerPlanGUID() As String
            Try
                Dim powerPlanSeacher As New ManagementObjectSearcher("root\CIMV2\power", "SELECT * FROM Win32_PowerPlan WHERE IsActive = True")

                If powerPlanSeacher.Get().Count = 0 Then
                    writeToSystemEventLog("WMI returned 0 results from Win32_PowerPlan.", EventLogEntryType.Error)
                    Return "INVALID"
                Else
                    Dim powerPlanDetails As ManagementObject = powerPlanSeacher.Get()(0)
                    Dim powerGUIDFromWMI As String = powerPlanDetails("InstanceID")

                    If Regex.IsMatch(powerGUIDFromWMI, "(\{[0-9A-F]{8}-[0-9A-F]{4}-[0-9A-F]{4}-[0-9A-F]{4}-[0-9A-F]{12}\})", RegexOptions.IgnoreCase) Then
                        Return Regex.Match(powerGUIDFromWMI, "(\{[0-9A-F]{8}-[0-9A-F]{4}-[0-9A-F]{4}-[0-9A-F]{4}-[0-9A-F]{12}\})", RegexOptions.IgnoreCase).Groups(1).Value.Replace("{", "").Replace("}", "").Trim.ToLower
                    Else
                        writeToSystemEventLog("Unable to parse out GUID from WMI output.", EventLogEntryType.Error)
                        Return "INVALID"
                    End If
                End If
            Catch ex As Exception
                Dim registryKey As RegistryKey = Registry.LocalMachine.OpenSubKey("SYSTEM\CurrentControlSet\Control\Power\User\PowerSchemes", False)

                If registryKey Is Nothing Then
                    Return executePowerCFGToGetActivePowerPlanGUID()
                Else
                    Dim powerGUID As String = registryKey.GetValue("ActivePowerScheme", globalVariables.invalidGUID)
                    registryKey.Close()
                    registryKey.Dispose()

                    If powerGUID = globalVariables.invalidGUID Then
                        Return executePowerCFGToGetActivePowerPlanGUID()
                    Else
                        Return powerGUID.Trim.ToLower
                    End If
                End If
            End Try
        End Function

        Function executePowerCFGToGetActivePowerPlanGUID() As String
            Dim output As String = executeCommandLineAndGetOutput("powercfg.exe", "/GETACTIVESCHEME")

            If Regex.IsMatch(output, "([0-9A-F]{8}-[0-9A-F]{4}-[0-9A-F]{4}-[0-9A-F]{4}-[0-9A-F]{12})", RegexOptions.IgnoreCase) Then
                Return Regex.Match(output, "([0-9A-F]{8}-[0-9A-F]{4}-[0-9A-F]{4}-[0-9A-F]{4}-[0-9A-F]{12})", RegexOptions.IgnoreCase).Groups(1).Value.Trim.ToLower
            Else
                writeToSystemEventLog("Unable to parse out GUID from powercfg.exe output.", EventLogEntryType.Error)
                Return "INVALID"
            End If
        End Function

        Function doWeHaveAValidActivePowerPlan(ByRef activePowerPlanGUID As String) As Boolean
            activePowerPlanGUID = getActivePowerPlanGUID()

            If activePowerPlanGUID = "INVALID" Then
                Return False
            Else
                Return True
            End If
        End Function

        Sub disablePowerPlanWakeFromSleep()
            Try
                Dim activePowerPlanGUID As String = Nothing

                If doWeHaveAValidActivePowerPlan(activePowerPlanGUID) = True Then
                    Dim registryKey As RegistryKey = Registry.LocalMachine.OpenSubKey("SYSTEM\CurrentControlSet\Control\Power\User\PowerSchemes\" & activePowerPlanGUID & "\238c9fa8-0aad-41ed-83f4-97be242c8f20\bd3b718a-0680-4d9d-8ab2-e1d2b4ac806d", False)

                    Dim boolDidWeChangeAnything As Boolean = False

                    If registryKey Is Nothing Then
                        boolDidWeChangeAnything = True
                        Shell("powercfg.exe -SETACVALUEINDEX " & activePowerPlanGUID & " SUB_SLEEP bd3b718a-0680-4d9d-8ab2-e1d2b4ac806d 0", AppWinStyle.Hide)
                        Shell("powercfg.exe -SETDCVALUEINDEX " & activePowerPlanGUID & " SUB_SLEEP bd3b718a-0680-4d9d-8ab2-e1d2b4ac806d 0", AppWinStyle.Hide)
                    Else
                        If Short.Parse(registryKey.GetValue("ACSettingIndex", "0").ToString) = 1 Then
                            boolDidWeChangeAnything = True
                            Shell("powercfg.exe -SETACVALUEINDEX " & activePowerPlanGUID & " SUB_SLEEP bd3b718a-0680-4d9d-8ab2-e1d2b4ac806d 0", AppWinStyle.Hide)
                        End If

                        If Short.Parse(registryKey.GetValue("DCSettingIndex", "0").ToString) = 1 Then
                            boolDidWeChangeAnything = True
                            Shell("powercfg.exe -SETDCVALUEINDEX " & activePowerPlanGUID & " SUB_SLEEP bd3b718a-0680-4d9d-8ab2-e1d2b4ac806d 0", AppWinStyle.Hide)
                        End If

                        registryKey.Close()
                        registryKey.Dispose()
                    End If

                    If boolDidWeChangeAnything = True Then
                        writeToSystemEventLog("System Restore Point Creator has set your Windows Power Plan back to default settings for wake timers.", EventLogEntryType.Information)

                        MsgBox("System Restore Point Creator has set your Windows Power Plan back to default settings for wake timers. Your system should no longer wake up for scheduled tasks.", MsgBoxStyle.Information, "System Restore Point Creator")
                    End If
                Else
                    MsgBox("This system doesn't appear to have a valid active power plan in place.", MsgBoxStyle.Critical, "System Restore Point Creator")
                End If
            Catch ex As Exception
                writeCrashToEventLog(ex)
            End Try
        End Sub

        Sub checkIfActivePowerPlanIsSetProperlyForWakingFromSleep(Optional boolShowNoChangesNeededMessage As Boolean = False)
            Try
                Dim activePowerPlanGUID As String = Nothing

                If doWeHaveAValidActivePowerPlan(activePowerPlanGUID) = True Then
                    Dim registryKey As RegistryKey = Registry.LocalMachine.OpenSubKey("SYSTEM\CurrentControlSet\Control\Power\User\PowerSchemes\" & activePowerPlanGUID & "\238c9fa8-0aad-41ed-83f4-97be242c8f20\bd3b718a-0680-4d9d-8ab2-e1d2b4ac806d", False)

                    Dim boolDidWeChangeAnything As Boolean = False

                    If registryKey Is Nothing Then
                        boolDidWeChangeAnything = True
                        Shell("powercfg.exe -SETACVALUEINDEX " & activePowerPlanGUID & " SUB_SLEEP bd3b718a-0680-4d9d-8ab2-e1d2b4ac806d 1", AppWinStyle.Hide)
                        Shell("powercfg.exe -SETDCVALUEINDEX " & activePowerPlanGUID & " SUB_SLEEP bd3b718a-0680-4d9d-8ab2-e1d2b4ac806d 1", AppWinStyle.Hide)
                    Else
                        If Short.Parse(registryKey.GetValue("ACSettingIndex", "0").ToString) <> 1 Then
                            boolDidWeChangeAnything = True
                            Shell("powercfg.exe -SETACVALUEINDEX " & activePowerPlanGUID & " SUB_SLEEP bd3b718a-0680-4d9d-8ab2-e1d2b4ac806d 1", AppWinStyle.Hide)
                        End If

                        If Short.Parse(registryKey.GetValue("DCSettingIndex", "0").ToString) <> 1 Then
                            boolDidWeChangeAnything = True
                            Shell("powercfg.exe -SETDCVALUEINDEX " & activePowerPlanGUID & " SUB_SLEEP bd3b718a-0680-4d9d-8ab2-e1d2b4ac806d 1", AppWinStyle.Hide)
                        End If

                        registryKey.Close()
                        registryKey.Dispose()
                    End If

                    If boolDidWeChangeAnything = True Then
                        writeToSystemEventLog("System Restore Point Creator has set your Windows Power Plan up to properly support waking up from Sleep Mode.", EventLogEntryType.Information)

                        MsgBox("System Restore Point Creator has set your Windows Power Plan up to properly support waking up from Sleep Mode." & vbCrLf & vbCrLf & "NOTE!" & vbCrLf & "This does not guarantee that your system will wake from sleep, your system's hardware must be able to support this functionality. Support for this functionality depends upon your system's motherboard and system drivers.", MsgBoxStyle.Information, "System Restore Point Creator")
                    Else
                        If boolShowNoChangesNeededMessage = True Then MsgBox("No changes to power plan settings were required.", MsgBoxStyle.Information, "System Restore Point Creator")
                    End If
                Else
                    MsgBox("This system doesn't appear to have a valid active power plan in place.", MsgBoxStyle.Critical, "System Restore Point Creator")
                End If
            Catch ex As Exception
                writeCrashToEventLog(ex)
            End Try
        End Sub

        'Public Function getDriveLetterFromDeviceID(deviceID As String) As String
        '    Dim objMOC As Management.ManagementObjectCollection
        '    Dim objMOS As Management.ManagementObjectSearcher = New Management.ManagementObjectSearcher("SELECT * FROM Win32_Volume")
        '    Dim objMO As Management.ManagementObject

        '    objMOC = objMOS.Get()

        '    For Each objMO In objMOC
        '        If deviceID = objMO("DeviceID") Then
        '            Return objMO("DriveLetter")
        '        End If
        '    Next

        '    Return Nothing
        'End Function

        'Private Sub enableSystemRestoreOnAllSystemDrives()
        '    Dim boolDisableRepairOfSystemRestoreOnSystemDrives As Boolean

        '    If Boolean.TryParse(Microsoft.Win32.Registry.LocalMachine.OpenSubKey(GlobalVariables.strProgramRegistryKey).GetValue("Disable Repair of System Restore on System Drives", "False"), boolDisableRepairOfSystemRestoreOnSystemDrives) Then
        '        If boolDisableRepairOfSystemRestoreOnSystemDrives = False Then
        '            Dim driveLetter As String

        '            For Each currentDrive As IO.DriveInfo In My.Computer.FileSystem.Drives
        '                If currentDrive.DriveType = IO.DriveType.Fixed Then
        '                    driveLetter = currentDrive.RootDirectory.ToString.Replace("\", "")
        '                    'debug.writeline("Repairing System Restore on System Drive " & driveLetter)
        '                    enableSystemRestoreOnDrive(driveLetter)
        '                End If
        '            Next
        '        Else
        '            'debug.writeline("System Restore on System Drive Repair disabled")
        '        End If
        '    End If
        'End Sub

        '' Call like this... enableSystemRestoreOnDrive("C:").  It accepts a drive letter.
        'Public Function disableSystemRestoreOnDrive(driveLetter As String) As Short
        '    Try
        '        Dim managementScopeObject As New ManagementScope("\\localhost\root\default")
        '        Dim managementPathObject As New ManagementPath("SystemRestore")
        '        Dim managementObjectOptions As New ObjectGetOptions()
        '        Dim managementClassObject As New ManagementClass(managementScopeObject, managementPathObject, managementObjectOptions)

        '        Dim managementBaseObjectParameters As ManagementBaseObject = managementClassObject.GetMethodParameters("Disable")
        '        managementBaseObjectParameters("Drive") = driveLetter
        '        'managementBaseObjectParameters("WaitTillEnabled") = True

        '        Dim oOutParams As ManagementBaseObject = managementClassObject.InvokeMethod("Disable", managementBaseObjectParameters, Nothing)

        '        'MsgBox(oOutParams.Properties.Count)
        '        'For Each t In oOutParams.Properties
        '        '    MsgBox(t.Name)
        '        'Next

        '        Return oOutParams("ReturnValue")
        '    Catch ex3 As System.Runtime.InteropServices.COMException
        '        Functions.giveComExceptionCrashMessage()
        '        Return 1
        '    Catch ex As Exception
        '        Return 1
        '    End Try
        'End Function

        'Public Function GetDriveInfo() As String
        '    Dim objMOC As Management.ManagementObjectCollection
        '    Dim objMOS As Management.ManagementObjectSearcher = New Management.ManagementObjectSearcher("SELECT * FROM Win32_Volume")
        '    Dim objMO As Management.ManagementObject

        '    objMOC = objMOS.Get()

        '    For Each objMO In objMOC
        '        Dim objDriveLetter As String = objMO("DeviceID")
        '        Dim objVolumeLabel = objMO("DriveLetter")

        '        'debug.writeline(objDriveLetter & ": " & objVolumeLabel)
        '    Next
        '    Return Nothing
        'End Function

        ' This is the old function kept here in case the patched function above is broken.
        'Public Function doesAtUserLoginTaskExist(ByRef delayedTime As Short) As Boolean
        '    Try
        '        Dim nameOfTask As String = GlobalVariables.taskFolder & "\Create a Restore Point at User Logon"
        '        Dim taskObject As Microsoft.Win32.TaskScheduler.Task

        '        Using taskServiceObject As TaskService = New TaskService()
        '            taskObject = taskServiceObject.GetTask(nameOfTask)

        '            ' Makes sure that the task exists and we don't get a Null Reference Exception.
        '            If taskObject IsNot Nothing Then
        '                ' Makes sure that we have some triggers to actually work with.
        '                If taskObject.Definition.Triggers.Count > 0 Then
        '                    Dim trigger As Trigger = taskObject.Definition.Triggers.Item(0)

        '                    delayedTime = CType(trigger, ITriggerDelay).Delay.Minutes

        '                    Return True
        '                End If
        '            End If
        '        End Using

        '        Return False
        '    Catch ex As Exception
        '        Return False
        '    End Try
        'End Function

        'Public Sub ScaleForm(WindowsForm As System.Windows.Forms.Form)
        '    Try
        '        Using g As System.Drawing.Graphics = WindowsForm.CreateGraphics
        '            Dim sngScaleFactor As Single = 1
        '            Dim sngFontFactor As Single = 1
        '            If g.DpiX > 96 Then
        '                sngScaleFactor = g.DpiX / 96
        '                'sngFontFactor = 96 / g.DpiY
        '            End If
        '            If WindowsForm.AutoScaleDimensions = WindowsForm.CurrentAutoScaleDimensions Then
        '                'ucWindowsFormHost.ScaleControl(WindowsForm, sngFontFactor)
        '                WindowsForm.Scale(sngScaleFactor)
        '            End If
        '        End Using
        '    Catch ex As Exception
        '    End Try
        'End Sub

        'Private Function doWeNeedToMakeAScheduledRestorePoint(strRestorePointDescription As String, shortEvery As Short) As Boolean
        '    Dim id As Long = getNewestSystemRestorePointIDMadeByRestorePointCreator(strRestorePointDescription)

        '    If id = 0 Then
        '        Return True
        '    End If

        '    Dim creationDate As Date = getSystemRestorePointDate(id)

        '    If shortEvery <> 0 And (creationDate = Nothing) = False Then
        '        'Math.Round(creationDate.Subtract(Now).TotalDays)
        '        'Dim dateDiffValue As Integer = Math.Abs(DateDiff(DateInterval.Day, creationDate, Now)) + 1
        '        Dim doubleDateDiffValue As Double = Math.Round(Now.Subtract(creationDate).TotalDays)
        '        Dim strDateDiffValue As String = daysToString(doubleDateDiffValue)

        '        If doubleDateDiffValue >= shortEvery Then
        '            writeToSystemEventLog(String.Format("The last restore point created by System Restore Point Creator was {0} ago.", strDateDiffValue), EventLogEntryType.Information)

        '            Return True
        '        Else
        '            Dim strEveryDayString As String = daysToString(shortEvery)

        '            writeToSystemEventLog(String.Format("A system restore point isn't necessary according to user preferences. User has instructed the program to only create restore points every {0}, the last restore point created by System Restore Point Creator was {1} ago.", strEveryDayString, strDateDiffValue), EventLogEntryType.Information)

        '            Return False
        '        End If
        '    Else
        '        ' If the value of Every is 0 that means that the user wants to have unlimited restore points created.
        '        Return True
        '    End If
        'End Function

        'Private Function executeAddSafeModeCommand() As String ' Adds a new OS boot entry to the Windows BCD and returns the GUID of the new entry.
        '    Try

        '        Dim sOutput As String = executeCommandLineAndGetOutput("bcdedit", "/copy {current} /d ""Windows Safe Mode Boot""")

        '        If Regex.IsMatch(sOutput, "(\{[0-9A-F]{8}-[0-9A-F]{4}-[0-9A-F]{4}-[0-9A-F]{4}-[0-9A-F]{12}\})", RegexOptions.IgnoreCase) Then
        '            Return Regex.Match(sOutput, "(\{[0-9A-F]{8}-[0-9A-F]{4}-[0-9A-F]{4}-[0-9A-F]{4}-[0-9A-F]{12}\})", RegexOptions.IgnoreCase).Groups(1).Value.Trim
        '        Else
        '            Return "failed"
        '        End If
        '    Catch ex As Exception
        '        Return "failed"
        '    End Try
        'End Function

        'Private Function checkUACStatus() As Boolean
        '    Dim registryKeyToRead As RegistryKey = Registry.LocalMachine.OpenSubKey("SOFTWARE\Microsoft\Windows\CurrentVersion\Policies\System", False)

        '    If registryKeyToRead Is Nothing Then
        '        Return True
        '    Else
        '        Dim enableLUA As String = registryKeyToRead.GetValue("EnableLUA", 1)
        '        registryKeyToRead.Close()
        '        registryKeyToRead.Dispose()

        '        Dim shortEnableLUA As Short
        '        If Short.TryParse(enableLUA, shortEnableLUA) Then
        '            If shortEnableLUA <> 0 Then
        '                Return True
        '            Else
        '                Return False
        '            End If
        '        Else
        '            Return True
        '        End If
        '    End If
        'End Function

        'Public Function setSafeModeBoot(Optional saveSettingAsNewBootEntry As Boolean = True) As Boolean
        '    If saveSettingAsNewBootEntry = True Then
        '        Dim strGUIDForWindowsSafeModeBootEntry As String = executeAddSafeModeCommand()

        '        If strGUIDForWindowsSafeModeBootEntry = "failed" Then
        '            Return False
        '        End If

        '        ' We need this later to remove the Safe Mode Boot Entry from the Windows BCD Store so we save the value in a Registry Value for later use.
        '        Registry.LocalMachine.OpenSubKey(GlobalVariables.strProgramRegistryKey, True).SetValue("Safe Mode Boot GUID", strGUIDForWindowsSafeModeBootEntry, RegistryValueKind.String)

        '        Shell("bcdedit /set " & strGUIDForWindowsSafeModeBootEntry & " safeboot minimal", AppWinStyle.Hide)
        '        Threading.Thread.Sleep(100)
        '        'Shell("bcdedit /default " & strGUIDForWindowsSafeModeBootEntry, AppWinStyle.Hide)
        '        'Threading.Thread.Sleep(100)
        '        Shell("bootcfg /timeout 5", AppWinStyle.Hide)
        '        Threading.Thread.Sleep(100)

        '        Return True
        '    Else
        '        Shell("bcdedit /set {default} safeboot minimal", AppWinStyle.Hide)
        '        Return True
        '    End If
        'End Function

        'Public Sub removeSafeModeBoot(Optional bcdBootGUIDToDelete As String = "{none}")
        '    If bcdBootGUIDToDelete = "{none}" Then
        '        Shell("bcdedit /deletevalue {default} safeboot", AppWinStyle.Hide)
        '    Else
        '        Shell("bcdedit /delete " & bcdBootGUIDToDelete, AppWinStyle.Hide)
        '    End If
        'End Sub

        'Sub loadIcon(objectGettingTheIcon As Object, image As Bitmap)
        '    objectGettingTheIcon.Image = image
        '    objectGettingTheIcon.ImageAlign = ContentAlignment.MiddleLeft
        'End Sub
    End Module

    Public Class ShadowStorageData
        Public AllocatedSpace As Long = 0
        Public DiffVolume As String = Nothing
        Public MaxSpace As Long = 0
        Public UsedSpace As Long = 0
        Public Volume As String = Nothing
    End Class

    Public Class processorInfoClass
        Public strProcessor As String = "unknown"
        Public strNumberOfCores As Short = "unknown"
    End Class
End Namespace