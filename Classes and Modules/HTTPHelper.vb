Imports System.IO
Imports System.Security.Cryptography

Public Class FormFile
    Private m_Name, m_ContentType, m_FilePath, m_uploadedFileName As String

    ''' <summary>This is the name for the form entry.</summary>
    Public Property formName() As String
        Get
            Return m_Name
        End Get
        Set(value As String)
            m_Name = value
        End Set
    End Property

    ''' <summary>This is the content type or MIME type.</summary>
    Public Property contentType() As String
        Get
            Return m_ContentType
        End Get
        Set(value As String)
            m_ContentType = value
        End Set
    End Property

    ''' <summary>This is the path to the file to be uploaded on the local file system.</summary>
    Public Property localFilePath() As String
        Get
            Return m_FilePath
        End Get
        Set(value As String)
            m_FilePath = value
        End Set
    End Property

    ''' <summary>This sets the name that the uploaded file will be called on the remote server.</summary>
    Public Property remoteFileName() As String
        Get
            Return m_uploadedFileName
        End Get
        Set(value As String)
            m_uploadedFileName = value
        End Set
    End Property
End Class

Public Class noMimeTypeFoundException
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

Public Class localFileAlreadyExistsException
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

Public Class dataMissingException
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

Public Class dataAlreadyExistsException
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

Public Class proxyConfigurationErrorException
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

Public Class dnsLookupError
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

Public Class noHTTPServerResponseHeadersFoundException
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

Public Class credentialsAlreadySet
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

Public Class httpProtocolException
    Inherits Exception
    Private _httpStatusCode As Net.HttpStatusCode = Net.HttpStatusCode.NoContent

    Public Sub New()
    End Sub

    Public Sub New(message As String)
        MyBase.New(message)
    End Sub

    Public Sub New(message As String, inner As Exception)
        MyBase.New(message, inner)
    End Sub

    Public Property httpStatusCode As Net.HttpStatusCode
        Get
            Return _httpStatusCode
        End Get
        Set(value As Net.HttpStatusCode)
            _httpStatusCode = value
        End Set
    End Property
End Class

Public Class noSSLCertificateFoundException
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

Class cookieDetails
    Public cookieData As String, cookieDomain As String
    Public cookiePath As String = "/"
End Class

Public Class downloadStatusDetails
    Public remoteFileSize As ULong, localFileSize As ULong, percentageDownloaded As Short
End Class

Class credentials
    Public strUser, strPassword As String
End Class

''' <summary>Allows you to easily POST and upload files to a remote HTTP server without you, the programmer, knowing anything about how it all works. This class does it all for you. It handles adding a User Agent String, additional HTTP Request Headers, string data to your HTTP POST data, and files to be uploaded in the HTTP POST data.</summary>
Public Class httpHelper
    Private Const classVersion As String = "1.300"

    Private strUserAgentString As String = Nothing
    Private boolUseProxy As Boolean = False
    Private boolUseSystemProxy As Boolean = True
    Private customProxy As Net.IWebProxy = Nothing
    Private httpResponseHeaders As Net.WebHeaderCollection = Nothing
    Private httpDownloadProgressPercentage As Short = 0
    Private remoteFileSize, currentFileSize As ULong
    Private httpTimeOut As Long = 5000
    Private boolUseHTTPCompression As Boolean = True
    Private lastAccessedURL As String = Nothing
    Private lastException As Exception = Nothing
    Private boolRunDownloadStatusUpdatePluginInSeparateThread As Boolean = True
    Private downloadStatusUpdaterThread As Threading.Thread = Nothing
    Private _intDownloadThreadSleepTime As Integer = 1000
    Private intDownloadBufferSize As Integer = 8191 ' The default is 8192 bytes or 8 KBs.
    Private strLastHTTPServerResponse As String

    Private additionalHTTPHeaders As New Dictionary(Of String, String)
    Private httpCookies As New Dictionary(Of String, cookieDetails)
    Private postData As New Dictionary(Of String, Object)
    Private getData As New Dictionary(Of String, String)
    Private downloadStatusDetails As downloadStatusDetails
    Private credentials As credentials

    Private sslCertificate As X509Certificates.X509Certificate2
    Private urlPreProcessor As Func(Of String, String)
    Private customErrorHandler As [Delegate]
    Private downloadStatusUpdater As [Delegate]

    ''' <summary>Retrieves the downloadStatusDetails data from within the Class instance.</summary>
    ''' <returns>A downloadStatusDetails Object.</returns>
    Public ReadOnly Property getDownloadStatusDetails As downloadStatusDetails
        Get
            Return downloadStatusDetails
        End Get
    End Property

    ''' <summary>Sets the size of the download buffer to hold data in memory during the downloading of a file. The default is 8192 bytes or 8 KBs.</summary>
    Public WriteOnly Property setDownloadBufferSize As Integer
        Set(value As Integer)
            intDownloadBufferSize = value - 1
        End Set
    End Property

    ''' <summary>This allows you to inject your own error handler for HTTP exceptions into the Class instance.</summary>
    ''' <value>A Lambda</value>
    ''' <example>
    ''' A VB.NET Example...
    ''' httpHelper.setCustomErrorHandler(Function(ByVal ex As Exception, classInstance As httpHelper)
    ''' End Function)
    ''' OR A C# Example...
    ''' httpHelper.setCustomErrorHandler((Exception ex, httpHelper classInstance) => { }
    ''' </example>
    Public WriteOnly Property setCustomErrorHandler As [Delegate]
        Set(value As [Delegate])
            customErrorHandler = value
        End Set
    End Property

    ''' <summary>Adds HTTP Authentication headers to your HTTP Request in this HTTPHelper instance.</summary>
    ''' <param name="strUsername">The username you want to pass to the server.</param>
    ''' <param name="strPassword">The password you want to pass to the server.</param>
    ''' <param name="throwExceptionIfAlreadySet">A Boolean value. This tells the function if it should throw an exception if HTTP Authentication settings have already been set.</param>
    Public Sub setHTTPCredentials(strUsername As String, strPassword As String, Optional throwExceptionIfAlreadySet As Boolean = True)
        If credentials Is Nothing Then
            credentials = New credentials() With {.strUser = strUsername, .strPassword = strPassword}
        Else
            If throwExceptionIfAlreadySet Then Throw New credentialsAlreadySet("HTTP Authentication Credentials have already been set for this HTTPHelper Class Instance.")
        End If
    End Sub

    ''' <summary>Sets up a custom proxy configuration for this class instance.</summary>
    ''' <param name="strUsername">The username you want to pass to the server.</param>
    ''' <param name="strPassword">The password you want to pass to the server.</param>
    ''' <param name="strServer">The proxy server's address, usually an IP address.</param>
    ''' <param name="intPort">The proxy port.</param>
    ''' <param name="boolByPassOnLocal">This tells the class instance if it should bypass the proxy for local servers. This is an optional value, by default it is True.</param>
    ''' <exception cref="proxyConfigurationErrorException">If this function throws a proxyConfigurationError, it means that something went wrong while setting up the proxy configuration for this class instance.</exception>
    Public Sub setProxy(strServer As String, intPort As Integer, strUsername As String, strPassword As String, Optional boolByPassOnLocal As Boolean = True)
        Try
            customProxy = New Net.WebProxy(String.Format("{0}:{1}", strServer, intPort.ToString), boolByPassOnLocal) With {.Credentials = New Net.NetworkCredential(strUsername, strPassword)}
        Catch ex As UriFormatException
            Throw New proxyConfigurationErrorException("There was an error setting up the proxy for this class instance.", ex)
        End Try
    End Sub

    ''' <summary>Sets up a custom proxy configuration for this class instance.</summary>
    ''' <param name="boolByPassOnLocal">This tells the class instance if it should bypass the proxy for local servers. This is an optional value, by default it is True.</param>
    ''' <param name="strServer">The proxy server's address, usually an IP address.</param>
    ''' <param name="intPort">The proxy port.</param>
    ''' <exception cref="proxyConfigurationErrorException">If this function throws a proxyConfigurationError, it means that something went wrong while setting up the proxy configuration for this class instance.</exception>
    Public Sub setProxy(strServer As String, intPort As Integer, Optional boolByPassOnLocal As Boolean = True)
        Try
            customProxy = New Net.WebProxy(String.Format("{0}:{1}", strServer, intPort.ToString), boolByPassOnLocal)
        Catch ex As UriFormatException
            Throw New proxyConfigurationErrorException("There was an error setting up the proxy for this class instance.", ex)
        End Try
    End Sub

    ''' <summary>Sets up a custom proxy configuration for this class instance.</summary>
    ''' <param name="boolByPassOnLocal">This tells the class instance if it should bypass the proxy for local servers. This is an optional value, by default it is True.</param>
    ''' <param name="strServer">The proxy server's address, usually an IP address followed up by a ":" followed up by a port number.</param>
    ''' <exception cref="proxyConfigurationErrorException">If this function throws a proxyConfigurationError, it means that something went wrong while setting up the proxy configuration for this class instance.</exception>
    Public Sub setProxy(strServer As String, Optional boolByPassOnLocal As Boolean = True)
        Try
            customProxy = New Net.WebProxy(strServer, boolByPassOnLocal)
        Catch ex As UriFormatException
            Throw New proxyConfigurationErrorException("There was an error setting up the proxy for this class instance.", ex)
        End Try
    End Sub

    ''' <summary>Sets up a custom proxy configuration for this class instance.</summary>
    ''' <param name="boolByPassOnLocal">This tells the class instance if it should bypass the proxy for local servers. This is an optional value, by default it is True.</param>
    ''' <param name="strServer">The proxy server's address, usually an IP address followed up by a ":" followed up by a port number.</param>
    ''' <param name="strUsername">The username you want to pass to the server.</param>
    ''' <param name="strPassword">The password you want to pass to the server.</param>
    ''' <exception cref="proxyConfigurationErrorException">If this function throws a proxyConfigurationError, it means that something went wrong while setting up the proxy configuration for this class instance.</exception>
    Public Sub setProxy(strServer As String, strUsername As String, strPassword As String, Optional boolByPassOnLocal As Boolean = True)
        Try
            customProxy = New Net.WebProxy(strServer, boolByPassOnLocal) With {.Credentials = New Net.NetworkCredential(strUsername, strPassword)}
        Catch ex As UriFormatException
            Throw New proxyConfigurationErrorException("There was an error setting up the proxy for this class instance.", ex)
        End Try
    End Sub

    ''' <summary>Returns the last Exception that occurred within this Class instance.</summary>
    ''' <returns>An Exception Object.</returns>
    Public ReadOnly Property getLastException As Exception
        Get
            Return lastException
        End Get
    End Property

    ''' <summary>This allows you to set up a function to be run while your HTTP download is being processed. This function can be used to update things on the GUI during a download.</summary>
    ''' <value>A Lambda</value>
    ''' <example>
    ''' A VB.NET Example...
    ''' httpHelper.setDownloadStatusUpdateRoutine(Function(ByVal downloadStatusDetails As downloadStatusDetails)
    ''' End Function)
    ''' OR A C# Example...
    ''' httpHelper.setDownloadStatusUpdateRoutine((downloadStatusDetails downloadStatusDetails) => { })
    ''' </example>
    Public WriteOnly Property setDownloadStatusUpdateRoutine As [Delegate]
        Set(value As [Delegate])
            downloadStatusUpdater = value
        End Set
    End Property

    ''' <summary>This allows you to set up a Pre-Processor of sorts for URLs in case you need to add things to the beginning or end of URLs.</summary>
    ''' <value>A Lambda</value>
    ''' <example>
    ''' httpHelper.setURLPreProcessor(Function(ByVal strURLInput As String) As String
    '''   If strURLInput.ToLower.StartsWith("http://") = False Then
    '''     strURLInput = "http://" + strURLInput
    '''   End If
    '''   Return strURLInput
    ''' End Function)
    ''' </example>
    Public WriteOnly Property setURLPreProcessor As Func(Of String, String)
        Set(value As Func(Of String, String))
            urlPreProcessor = value
        End Set
    End Property

    ''' <summary>This wipes out most of the data in this Class instance. Once you have called this function it's recommended to set the name of your class instance to Nothing. For example... httpHelper = Nothing</summary>
    Public Sub dispose()
        additionalHTTPHeaders.Clear()
        httpCookies.Clear()
        postData.Clear()
        getData.Clear()

        remoteFileSize = 0
        currentFileSize = 0

        sslCertificate = Nothing
        urlPreProcessor = Nothing
        customErrorHandler = Nothing
        downloadStatusUpdater = Nothing
        httpResponseHeaders = Nothing
        strLastHTTPServerResponse = Nothing
    End Sub

    ''' <summary>Returns the last accessed URL by this Class instance.</summary>
    ''' <returns>A String.</returns>
    Public ReadOnly Property getLastAccessedURL As String
        Get
            Return lastAccessedURL
        End Get
    End Property

    ''' <summary>Tells the Class instance if it should use the system proxy.</summary>
    Public WriteOnly Property useSystemProxy As Boolean
        Set(value As Boolean)
            boolUseSystemProxy = value
        End Set
    End Property

    ''' <summary>This function allows you to get a peek inside the Class object instance. It returns many of the things that make up the Class instance like POST and GET data, cookies, additional HTTP headers, if proxy mode and HTTP compression mode is enabled, the user agent string, etc.</summary>
    ''' <returns>A String.</returns>
    Public Overrides Function toString() As String
        Dim stringBuilder As New Text.StringBuilder()
        stringBuilder.AppendLine("--== HTTPHelper Class Object ==--")
        stringBuilder.AppendLine("--== Version: " & classVersion & " ==--")
        stringBuilder.AppendLine()
        stringBuilder.AppendLine("Last Accessed URL: " & lastAccessedURL)
        stringBuilder.AppendLine()

        If getData.Count <> 0 Then
            For Each item In getData
                stringBuilder.AppendLine("GET Data | " & item.Key & "=" & item.Value)
            Next
        End If

        If postData.Count <> 0 Then
            For Each item In postData
                stringBuilder.AppendLine("POST Data | " & item.Key & "=" & item.Value.ToString())
            Next
        End If

        If httpCookies.Count <> 0 Then
            For Each item In httpCookies
                stringBuilder.AppendLine("COOKIES | " & item.Key & "=" & item.Value.cookieData)
            Next
        End If

        If additionalHTTPHeaders.Count <> 0 Then
            For Each item In additionalHTTPHeaders
                stringBuilder.AppendLine("Additional HTTP Header | " & item.Key & "=" & item.Value)
            Next
        End If

        stringBuilder.AppendLine()

        stringBuilder.AppendLine("User Agent String: " & strUserAgentString)
        stringBuilder.AppendLine("Use HTTP Compression: " & boolUseHTTPCompression.ToString)
        stringBuilder.AppendLine("HTTP Time Out: " & httpTimeOut)
        stringBuilder.AppendLine("Use Proxy: " & boolUseProxy.ToString)

        If credentials Is Nothing Then
            stringBuilder.AppendLine("HTTP Authentication Enabled: False")
        Else
            stringBuilder.AppendLine("HTTP Authentication Enabled: True")
            stringBuilder.AppendLine("HTTP Authentication Details: " & credentials.strUser & "|" & credentials.strPassword)
        End If

        If lastException IsNot Nothing Then
            stringBuilder.AppendLine()
            stringBuilder.AppendLine("--== Raw Exception Data ==--")
            stringBuilder.AppendLine(lastException.ToString)

            If TypeOf lastException Is Net.WebException Then
                stringBuilder.AppendLine("Raw Exception Status Code: " & DirectCast(lastException, Net.WebException).Status.ToString)
            End If
        End If

        Return stringBuilder.ToString.Trim
    End Function

    ''' <summary>Gets the remote file size.</summary>
    ''' <param name="boolHumanReadable">Optional setting, normally set to True. Tells the function if it should transform the Integer representing the file size into a human readable format.</param>
    ''' <returns>Either a String or a Long containing the remote file size.</returns>
    Public Function getHTTPDownloadRemoteFileSize(Optional boolHumanReadable As Boolean = True) As Object
        If boolHumanReadable Then
            Return fileSizeToHumanReadableFormat(remoteFileSize)
        Else
            Return remoteFileSize
        End If
    End Function

    ''' <summary>This returns the SSL certificate details for the last HTTP request made by this Class instance.</summary>
    ''' <returns>System.Security.Cryptography.X509Certificates.X509Certificate2</returns>
    ''' <exception cref="noSSLCertificateFoundException">If this function throws a noSSLCertificateFoundException it means that the Class doesn't have an SSL certificate in the memory space of the Class instance. Perhaps the last HTTP request wasn't an HTTPS request.</exception>
    ''' <param name="boolThrowException">An optional parameter that tells the function if it should throw an exception if an SSL certificate isn't found in the memory space of this Class instance.</param>
    Public Function getCertificateDetails(Optional boolThrowException As Boolean = True) As X509Certificates.X509Certificate2
        If sslCertificate Is Nothing Then
            If boolThrowException Then
                lastException = New noSSLCertificateFoundException("No valid SSL certificate found for the last HTTP request. Perhaps the last HTTP request wasn't an HTTPS request.")
                Throw lastException
            End If
            Return Nothing
        Else
            Return sslCertificate
        End If
    End Function

    ''' <summary>Gets the current local file's size.</summary>
    ''' <param name="boolHumanReadable">Optional setting, normally set to True. Tells the function if it should transform the Integer representing the file size into a human readable format.</param>
    ''' <returns>Either a String or a Long containing the current local file's size.</returns>
    Public Function getHTTPDownloadLocalFileSize(Optional boolHumanReadable As Boolean = True) As Object
        If boolHumanReadable Then
            Return fileSizeToHumanReadableFormat(currentFileSize)
        Else
            Return currentFileSize
        End If
    End Function

    ''' <summary>Creates a new instance of the HTTPPost Class. You will need to set things up for the Class instance using the setProxyMode() and setUserAgent() routines.</summary>
    ''' <example>Dim httpPostObject As New Tom.HTTPPost()</example>
    Public Sub New()
    End Sub

    ''' <summary>Creates a new instance of the HTTPPost Class with some required parameters.</summary>
    ''' <param name="strUserAgentStringIN">This set the User Agent String for the HTTP Request.</param>
    ''' <example>Dim httpPostObject As New Tom.HTTPPost("Microsoft .NET")</example>
    Public Sub New(strUserAgentStringIN As String)
        strUserAgentString = strUserAgentStringIN
    End Sub

    ''' <summary>Creates a new instance of the HTTPPost Class with some required parameters.</summary>
    ''' <param name="strUserAgentStringIN">This set the User Agent String for the HTTP Request.</param>
    ''' <param name="boolUseProxyIN">This tells the Class if you're going to be using a Proxy or not.</param>
    ''' <example>Dim httpPostObject As New Tom.HTTPPost("Microsoft .NET", True)</example>
    Public Sub New(strUserAgentStringIN As String, boolUseProxyIN As Boolean)
        strUserAgentString = strUserAgentStringIN
        boolUseProxy = boolUseProxyIN
    End Sub

    ''' <summary>Tells the HTTPPost Class if you want to use a Proxy or not.</summary>
    Public WriteOnly Property setProxyMode As Boolean
        Set(value As Boolean)
            boolUseProxy = value
        End Set
    End Property

    ''' <summary>Sets a timeout for any HTTP requests in this Class. Normally it's set for 5 seconds. The input is the amount of time in seconds (NOT milliseconds) that you want your HTTP requests to timeout in. The class will translate the seconds to milliseconds for you.</summary>
    ''' <value>The amount of time in seconds (NOT milliseconds) that you want your HTTP requests to timeout in. This function will translate the seconds to milliseconds for you.</value>
    Public WriteOnly Property setHTTPTimeout As Short
        Set(value As Short)
            httpTimeOut = value * 1000
        End Set
    End Property

    ''' <summary>Tells this Class instance if it should use HTTP compression for transport. Using HTTP Compression can save bandwidth. Normally the Class is setup to use HTTP Compression by default.</summary>
    ''' <value>Boolean value.</value>
    Public WriteOnly Property useHTTPCompression As Boolean
        Set(value As Boolean)
            boolUseHTTPCompression = value
        End Set
    End Property

    ''' <summary>Sets the User Agent String to be used by the HTTPPost Class.</summary>
    ''' <value>Your User Agent String.</value>
    Public WriteOnly Property setUserAgent As String
        Set(value As String)
            strUserAgentString = value
        End Set
    End Property

    ''' <summary>This adds a String variable to your POST data.</summary>
    ''' <param name="strName">The form name of the data to post.</param>
    ''' <param name="strValue">The value of the data to post.</param>
    ''' <param name="throwExceptionIfDataAlreadyExists">This tells the function if it should throw an exception if the data already exists in the POST data.</param>
    ''' <exception cref="dataAlreadyExistsException">If this function throws a dataAlreadyExistsException, you forgot to add some data for your POST variable.</exception>
    Public Sub addPOSTData(strName As String, strValue As String, Optional throwExceptionIfDataAlreadyExists As Boolean = False)
        If String.IsNullOrEmpty(strValue.Trim) Then
            lastException = New dataMissingException(String.Format("Data was missing for the {0}{1}{0} POST variable.", Chr(34), strName))
            Throw lastException
        End If

        If postData.ContainsKey(strName) And throwExceptionIfDataAlreadyExists Then
            lastException = New dataAlreadyExistsException(String.Format("The POST data key named {0}{1}{0} already exists in the POST data.", Chr(34), strName))
            Throw lastException
        Else
            postData.Remove(strName)
            postData.Add(strName, strValue)
        End If
    End Sub

    ''' <summary>This adds a String variable to your GET data.</summary>
    ''' <param name="strName">The form name of the data to post.</param>
    ''' <param name="strValue">The value of the data to post.</param>
    ''' <exception cref="dataAlreadyExistsException">If this function throws a dataAlreadyExistsException, you forgot to add some data for your POST variable.</exception>
    Public Sub addGETData(strName As String, strValue As String, Optional throwExceptionIfDataAlreadyExists As Boolean = False)
        If String.IsNullOrEmpty(strValue.Trim) Then
            lastException = New dataMissingException(String.Format("Data was missing for the {0}{1}{0} GET variable.", Chr(34), strName))
            Throw lastException
        End If

        If getData.ContainsKey(strName) And throwExceptionIfDataAlreadyExists Then
            lastException = New dataAlreadyExistsException(String.Format("The GET data key named {0}{1}{0} already exists in the GET data.", Chr(34), strName))
            Throw lastException
        Else
            getData.Remove(strName)
            getData.Add(strName, strValue)
        End If
    End Sub

    ''' <summary>Allows you to add additional headers to your HTTP Request Headers.</summary>
    ''' <param name="strHeaderName">The name of your new HTTP Request Header.</param>
    ''' <param name="strHeaderContents">The contents of your new HTTP Request Header. Be careful with adding data here, invalid data can cause your HTTP Request to fail thus throwing an httpPostException.</param>
    ''' <param name="urlEncodeHeaderContent">Optional setting, normally set to False. Tells the function if it should URLEncode the HTTP Header Contents before setting it.</param>
    ''' <example>httpPostObject.addHTTPHeader("myheader", "my header value")</example>
    ''' <exception cref="dataAlreadyExistsException">If this function throws an dataAlreadyExistsException, it means that this Class instance already has an Additional HTTP Header of that name in the Class instance.</exception>
    Public Sub addHTTPHeader(strHeaderName As String, strHeaderContents As String, Optional urlEncodeHeaderContent As Boolean = False)
        If Not doesAdditionalHeaderExist(strHeaderName) Then
            If urlEncodeHeaderContent Then
                additionalHTTPHeaders.Add(strHeaderName.ToLower, Web.HttpUtility.UrlEncode(strHeaderContents))
            Else
                additionalHTTPHeaders.Add(strHeaderName.ToLower, strHeaderContents)
            End If
        Else
            lastException = New dataAlreadyExistsException(String.Format("The additional HTTP Header named {0}{1}{0} already exists in the Additional HTTP Headers settings for this Class instance.", Chr(34), strHeaderName))
            Throw lastException
        End If
    End Sub

    ''' <summary>Allows you to add HTTP cookies to your HTTP Request with a specific path for the cookie.</summary>
    ''' <param name="strCookieName">The name of your cookie.</param>
    ''' <param name="strCookieValue">The value for your cookie.</param>
    ''' <param name="strCookiePath">The path for the cookie.</param>
    ''' <param name="strDomainDomain">The domain for the cookie.</param>
    ''' <param name="urlEncodeHeaderContent">Optional setting, normally set to False. Tells the function if it should URLEncode the cookie contents before setting it.</param>
    ''' <exception cref="dataAlreadyExistsException">If this function throws a dataAlreadyExistsException, it means that the cookie already exists in this Class instance.</exception>
    Public Sub addHTTPCookie(strCookieName As String, strCookieValue As String, strDomainDomain As String, strCookiePath As String, Optional urlEncodeHeaderContent As Boolean = False)
        If Not doesCookieExist(strCookieName) Then
            Dim cookieDetails As New cookieDetails() With {.cookieDomain = strDomainDomain, .cookiePath = strCookiePath}

            If urlEncodeHeaderContent Then
                cookieDetails.cookieData = Web.HttpUtility.UrlEncode(strCookieValue)
            Else
                cookieDetails.cookieData = strCookieValue
            End If

            httpCookies.Add(strCookieName.ToLower, cookieDetails)
        Else
            lastException = New dataAlreadyExistsException(String.Format("The HTTP Cookie named {0}{1}{0} already exists in the settings for this Class instance.", Chr(34), strCookieName))
            Throw lastException
        End If
    End Sub

    ''' <summary>Allows you to add HTTP cookies to your HTTP Request with a default path of "/".</summary>
    ''' <param name="strCookieName">The name of your cookie.</param>
    ''' <param name="strCookieValue">The value for your cookie.</param>
    ''' <param name="strCookieDomain">The domain for the cookie.</param>
    ''' <param name="urlEncodeHeaderContent">Optional setting, normally set to False. Tells the function if it should URLEncode the cookie contents before setting it.</param>
    ''' <exception cref="dataAlreadyExistsException">If this function throws a dataAlreadyExistsException, it means that the cookie already exists in this Class instance.</exception>
    Public Sub addHTTPCookie(strCookieName As String, strCookieValue As String, strCookieDomain As String, Optional urlEncodeHeaderContent As Boolean = False)
        If Not doesCookieExist(strCookieName) Then
            Dim cookieDetails As New cookieDetails() With {.cookieDomain = strCookieDomain, .cookiePath = "/"}

            If urlEncodeHeaderContent Then
                cookieDetails.cookieData = Web.HttpUtility.UrlEncode(strCookieValue)
            Else
                cookieDetails.cookieData = strCookieValue
            End If

            httpCookies.Add(strCookieName.ToLower, cookieDetails)
        Else
            lastException = New dataAlreadyExistsException(String.Format("The HTTP Cookie named {0}{1}{0} already exists in the settings for this Class instance.", Chr(34), strCookieName))
            Throw lastException
        End If
    End Sub

    ''' <summary>Checks to see if the GET data key exists in this GET data.</summary>
    ''' <param name="strName">The name of the GET data variable you are checking the existance of.</param>
    ''' <returns></returns>
    Public Function doesGETDataExist(strName As String) As Boolean
        Return getData.ContainsKey(strName)
    End Function

    ''' <summary>Checks to see if the POST data key exists in this POST data.</summary>
    ''' <param name="strName">The name of the POST data variable you are checking the existance of.</param>
    ''' <returns></returns>
    Public Function doesPOSTDataExist(strName As String) As Boolean
        Return postData.ContainsKey(strName)
    End Function

    ''' <summary>Checks to see if an additional HTTP Request Header has been added to the Class.</summary>
    ''' <param name="strHeaderName">The name of the HTTP Request Header to check the existance of.</param>
    ''' <returns>Boolean value; True if found, False if not found.</returns>
    Public Function doesAdditionalHeaderExist(strHeaderName As String) As Boolean
        Return additionalHTTPHeaders.ContainsKey(strHeaderName.ToLower)
    End Function

    ''' <summary>Checks to see if a cookie has been added to the Class.</summary>
    ''' <param name="strCookieName">The name of the cookie to check the existance of.</param>
    ''' <returns>Boolean value; True if found, False if not found.</returns>
    Public Function doesCookieExist(strCookieName As String) As Boolean
        Return httpCookies.ContainsKey(strCookieName.ToLower)
    End Function

    ''' <summary>This adds a file to be uploaded to your POST data.</summary>
    ''' <param name="strFormName">The form name of the data to post.</param>
    ''' <param name="strLocalFilePath">The path to the file you want to upload.</param>
    ''' <param name="strRemoteFileName">This is the name that the uploaded file will be called on the remote server. If set to Nothing the program will fill the name in.</param>
    ''' <param name="strContentType">The Content Type of the file you want to upload. You can leave it blank (or set to Nothing) and the program will try and determine what the MIME type of the file you're attaching is.</param>
    ''' <exception cref="FileNotFoundException">If this function throws a FileNotFoundException, the Class wasn't able to find the file that you're trying to attach to the POST data on the local file system.</exception>
    ''' <exception cref="noMimeTypeFoundException">If this function throws a noMimeTypeFoundException, the Class wasn't able to automatically determine the MIME type of the file you're trying to attach to the POST data.</exception>
    ''' <example>httpPostObject.addFileToUpload("file", "C:\My File.txt", "My File.txt", Nothing)</example>
    ''' <example>httpPostObject.addFileToUpload("file", "C:\My File.txt", "My File.txt", "text/plain")</example>
    ''' <example>httpPostObject.addFileToUpload("file", "C:\My File.txt", Nothing, Nothing)</example>
    ''' <example>httpPostObject.addFileToUpload("file", "C:\My File.txt", Nothing, "text/plain")</example>
    Public Sub addFileUpload(strFormName As String, strLocalFilePath As String, strRemoteFileName As String, strContentType As String, Optional throwExceptionIfItemAlreadyExists As Boolean = False)
        Dim fileInfo As New FileInfo(strLocalFilePath)

        If Not fileInfo.Exists Then
            lastException = New FileNotFoundException("Local file not found.", strLocalFilePath)
            Throw lastException
        ElseIf postData.ContainsKey(strFormName) Then
            If throwExceptionIfItemAlreadyExists Then
                lastException = New dataAlreadyExistsException(String.Format("The POST data key named {0}{1}{0} already exists in the POST data.", Chr(34), strFormName))
                Throw lastException
            Else
                Exit Sub
            End If
        Else
            Dim formFileInstance As New FormFile With {
                .formName = strFormName,
                .localFilePath = strLocalFilePath,
                .remoteFileName = strRemoteFileName
            }

            If String.IsNullOrEmpty(strContentType) Then
                Dim contentType As String
                Dim regPath As Microsoft.Win32.RegistryKey = Microsoft.Win32.Registry.ClassesRoot.OpenSubKey(fileInfo.Extension.ToLower, False)

                If regPath Is Nothing Then
                    lastException = New noMimeTypeFoundException("No MIME Type found for " & fileInfo.Extension.ToLower)
                    Throw lastException
                Else
                    contentType = regPath.GetValue("Content Type", Nothing).ToString
                End If

                If String.IsNullOrEmpty(contentType) Then
                    lastException = New noMimeTypeFoundException("No MIME Type found for " & fileInfo.Extension.ToLower)
                    Throw lastException
                Else
                    formFileInstance.contentType = contentType
                End If
            Else
                formFileInstance.contentType = strContentType
            End If

            postData.Add(strFormName, formFileInstance)
        End If
    End Sub

    ''' <summary>Gets the HTTP Response Headers that were returned by the HTTP Server after the HTTP request.</summary>
    ''' <param name="throwExceptionIfNoHeaders">Optional setting, normally set to False. Tells the function if it should throw an exception if no HTTP Response Headers are contained in this Class instance.</param>
    ''' <returns>A collection of HTTP Response Headers in a Net.WebHeaderCollection object.</returns>
    ''' <exception cref="noHTTPServerResponseHeadersFoundException">If this function throws a noHTTPServerResponseHeadersFoundException, there are no HTTP Response Headers in this Class instance.</exception>
    Public Function getHTTPResponseHeaders(Optional throwExceptionIfNoHeaders As Boolean = False) As Net.WebHeaderCollection
        If httpResponseHeaders Is Nothing Then
            If throwExceptionIfNoHeaders Then
                lastException = New noHTTPServerResponseHeadersFoundException("No HTTP Server Response Headers found.")
                Throw lastException
            Else
                Return Nothing
            End If
        Else
            Return httpResponseHeaders
        End If
    End Function

    ''' <summary>Gets the percentage of the file that's being downloaded from the HTTP Server.</summary>
    ''' <returns>Returns a Short Integer value.</returns>
    Public ReadOnly Property getHTTPDownloadProgressPercentage As Short
        Get
            Return httpDownloadProgressPercentage
        End Get
    End Property

    ''' <summary>This tells the current HTTPHelper Class Instance if it should run the download update status routine in a separate thread. By default this is enabled.</summary>
    Public Property enableMultiThreadedDownloadStatusUpdates As Boolean
        Get
            Return boolRunDownloadStatusUpdatePluginInSeparateThread
        End Get
        Set(value As Boolean)
            boolRunDownloadStatusUpdatePluginInSeparateThread = value
        End Set
    End Property

    ''' <summary>Sets the amount of time in miliseconds that the download status updating thread sleeps. The default is 1000 ms or 1 second, perfect for calculating the amount of data downloaded per second.</summary>
    Public WriteOnly Property intDownloadThreadSleepTime As Integer
        Set(value As Integer)
            _intDownloadThreadSleepTime = value
        End Set
    End Property

    Private Sub downloadStatusUpdaterThreadSubroutine()
        Try
beginAgain:
            downloadStatusUpdater.DynamicInvoke(downloadStatusDetails)
            Threading.Thread.Sleep(_intDownloadThreadSleepTime)
            GoTo beginAgain
        Catch ex As Threading.ThreadAbortException
            ' Does nothing
        Catch ex2 As Reflection.TargetInvocationException
            ' Does nothing
        End Try
    End Sub

    ''' <summary>This subroutine is used by the downloadFile function to update the download status of the file that's being downloaded by the class instance.</summary>
    Private Sub downloadStatusUpdateInvoker()
        downloadStatusDetails = New downloadStatusDetails With {.remoteFileSize = remoteFileSize, .percentageDownloaded = httpDownloadProgressPercentage, .localFileSize = currentFileSize} ' Update the downloadStatusDetails.

        ' Checks to see if we have a status update routine to invoke.
        If downloadStatusUpdater IsNot Nothing Then
            ' We invoke the status update routine if we have one to invoke. This is usually injected
            ' into the class instance by the programmer who's using this class in his/her program.
            If boolRunDownloadStatusUpdatePluginInSeparateThread Then
                If downloadStatusUpdaterThread Is Nothing Then
                    downloadStatusUpdaterThread = New Threading.Thread(AddressOf downloadStatusUpdaterThreadSubroutine) With {
                        .IsBackground = True,
                        .Priority = Threading.ThreadPriority.Lowest,
                        .Name = "HTTPHelper Class Download Status Updating Thread"
                    }
                    downloadStatusUpdaterThread.Start()
                End If
            Else
                downloadStatusUpdater.DynamicInvoke(downloadStatusDetails)
            End If
        End If
    End Sub

    Private Sub abortDownloadStatusUpdaterThread()
        Try
            If downloadStatusUpdaterThread IsNot Nothing And boolRunDownloadStatusUpdatePluginInSeparateThread Then
                downloadStatusUpdaterThread.Abort()
                downloadStatusUpdaterThread = Nothing
            End If
        Catch ex As Exception
            ' Does nothing
        End Try
    End Sub

    ''' <summary>Downloads a file from a web server while feeding back the status of the download. You can find the percentage of the download in the httpDownloadProgressPercentage variable. This function gives you the programmer more control over how HTTP downloads are done. For instance, if you don't want to write the data directly out to disk until the download is complete, this function gives you that ability whereas the downloadFile() function writes the downloaded data directly to disk bypassing system RAM. This is good for those cases you may be writing the data to an SSD in which you only want to write the data to the SSD until the download is known to be successful.</summary>
    ''' <param name="fileDownloadURL">The HTTP Path to a file on a remote server to download.</param>
    ''' <param name="memStream">This is a IO.MemoryStream, it is passed as a ByRef so that the function will be able to act on the IO.MemoryStream() Object you pass to it. At the end of the download, if it is successful, the function will reset the position back to 0 for writing to whatever stream you choose.</param>
    ''' <param name="throwExceptionIfError">Normally True. If True this function will throw an exception if an error occurs. If set to False, the function simply returns False if an error occurs; this is a much more simpler way to handle errors.</param>
    ''' <exception cref="Net.WebException">If this function throws a Net.WebException then something failed during the HTTP request.</exception>
    ''' <exception cref="localFileAlreadyExistsException">If this function throws a localFileAlreadyExistsException, the path in the local file system already exists.</exception>
    ''' <exception cref="Exception">If this function throws a general Exception, something really went wrong; something that the function normally doesn't handle.</exception>
    ''' <exception cref="httpProtocolException">This exception is thrown if the server responds with an HTTP Error.</exception>
    ''' <exception cref="sslErrorException">If this function throws an sslErrorException, an error occurred while negotiating an SSL connection.</exception>
    ''' <exception cref="dnsLookupError">If this function throws a dnsLookupError exception it means that the domain name wasn't able to be resolved properly.</exception>
    Public Function downloadFile(ByVal fileDownloadURL As String, ByRef memStream As MemoryStream, Optional ByVal throwExceptionIfError As Boolean = True) As Boolean
        Dim httpWebRequest As Net.HttpWebRequest = Nothing
        currentFileSize = 0
        Dim amountDownloaded As Double

        Try
            If urlPreProcessor IsNot Nothing Then
                fileDownloadURL = urlPreProcessor(fileDownloadURL)
            End If
            lastAccessedURL = fileDownloadURL

            ' We create a new data buffer to hold the stream of data from the web server.
            Dim dataBuffer As Byte() = New Byte(intDownloadBufferSize) {}

            httpWebRequest = DirectCast(Net.WebRequest.Create(fileDownloadURL), Net.HttpWebRequest)

            configureProxy(httpWebRequest)
            addParametersToWebRequest(httpWebRequest)

            Dim webResponse As Net.WebResponse = httpWebRequest.GetResponse() ' We now get the web response.
            captureSSLInfo(fileDownloadURL, httpWebRequest)

            ' Gets the size of the remote file on the web server.
            remoteFileSize = CType(webResponse.ContentLength, ULong)

            Dim responseStream As Stream = webResponse.GetResponseStream() ' Gets the response stream.

            Dim lngBytesReadFromInternet As ULong = responseStream.Read(dataBuffer, 0, dataBuffer.Length) ' Reads some data from the HTTP stream into our data buffer.

            ' We keep looping until all of the data has been downloaded.
            While lngBytesReadFromInternet <> 0
                ' We calculate the current file size by adding the amount of data that we've so far
                ' downloaded from the server repeatedly to a variable called "currentFileSize".
                currentFileSize += lngBytesReadFromInternet

                memStream.Write(dataBuffer, 0, lngBytesReadFromInternet) ' Writes the data directly to disk.

                amountDownloaded = (currentFileSize / remoteFileSize) * 100
                httpDownloadProgressPercentage = CType(Math.Round(amountDownloaded, 0), Short) ' Update the download percentage value.
                downloadStatusUpdateInvoker()

                lngBytesReadFromInternet = CType(responseStream.Read(dataBuffer, 0, dataBuffer.Length), ULong) ' Reads more data into our data buffer.
            End While

            ' Before we return the MemoryStream to the user we have to reset the position back to the beginning of the Stream. This is so that when the
            ' user processes the IO.MemoryStream that's returned as part of this function the IO.MemoryStream will be ready to write the data out of
            ' memory and into whatever stream the user wants to write the data out to. If this isn't done and the user executes the CopyTo() function
            ' on the IO.MemoryStream Object the user will have nothing written out because the IO.MemoryStream will be at the end of the stream.
            memStream.Position = 0

            abortDownloadStatusUpdaterThread()

            Return True
        Catch ex As Threading.ThreadAbortException
            abortDownloadStatusUpdaterThread()

            If httpWebRequest IsNot Nothing Then httpWebRequest.Abort()

            If memStream IsNot Nothing Then
                memStream.Close() ' Closes the file stream.
                memStream.Dispose() ' Disposes the file stream.
            End If

            Return False
        Catch ex As Exception
            abortDownloadStatusUpdaterThread()

            lastException = ex
            If memStream IsNot Nothing Then
                memStream.Close() ' Closes the file stream.
                memStream.Dispose() ' Disposes the file stream.
            End If

            If Not throwExceptionIfError Then Return False

            If customErrorHandler IsNot Nothing Then
                customErrorHandler.DynamicInvoke(ex, Me)
                ' Since we handled the exception with an injected custom error handler, we can now exit the function with the return of a False value.
                Return False
            End If

            If TypeOf ex Is Net.WebException Then
                Dim ex2 As Net.WebException = DirectCast(ex, Net.WebException)

                If ex2.Status = Net.WebExceptionStatus.ProtocolError Then
                    Throw handleWebExceptionProtocolError(fileDownloadURL, ex2)
                    Return False
                ElseIf ex2.Status = Net.WebExceptionStatus.TrustFailure Then
                    lastException = New sslErrorException("There was an error establishing an SSL connection.", ex2)
                    Throw lastException
                    Return False
                ElseIf ex2.Status = Net.WebExceptionStatus.NameResolutionFailure Then
                    Dim strDomainName As String = Text.RegularExpressions.Regex.Match(lastAccessedURL, "(?:http(?:s){0,1}://){0,1}(.*)/", Text.RegularExpressions.RegexOptions.Singleline).Groups(1).Value
                    lastException = New dnsLookupError(String.Format("There was an error while looking up the DNS records for the domain name {0}{1}{0}.", Chr(34), strDomainName), ex2)
                    Throw lastException
                    Return False
                End If

                lastException = New Net.WebException(ex.Message, ex2)
                Throw lastException
                Return False
            End If

            Return False
        End Try
    End Function

    ''' <summary>Downloads a file from a web server while feeding back the status of the download. You can find the percentage of the download in the httpDownloadProgressPercentage variable.</summary>
    ''' <param name="fileDownloadURL">The HTTP Path to a file on a remote server to download.</param>
    ''' <param name="localFileName">The path in the local file system to which you are saving the file that's being downloaded.</param>
    ''' <param name="throwExceptionIfLocalFileExists">This tells the function if it should throw an Exception if the local file already exists. If set the False the function will delete the local file if it exists before the download starts.</param>
    ''' <param name="throwExceptionIfError">Normally True. If True this function will throw an exception if an error occurs. If set to False, the function simply returns False if an error occurs; this is a much more simpler way to handle errors.</param>
    ''' <exception cref="Net.WebException">If this function throws a Net.WebException then something failed during the HTTP request.</exception>
    ''' <exception cref="localFileAlreadyExistsException">If this function throws a localFileAlreadyExistsException, the path in the local file system already exists.</exception>
    ''' <exception cref="Exception">If this function throws a general Exception, something really went wrong; something that the function normally doesn't handle.</exception>
    ''' <exception cref="httpProtocolException">This exception is thrown if the server responds with an HTTP Error.</exception>
    ''' <exception cref="sslErrorException">If this function throws an sslErrorException, an error occurred while negotiating an SSL connection.</exception>
    ''' <exception cref="dnsLookupError">If this function throws a dnsLookupError exception it means that the domain name wasn't able to be resolved properly.</exception>
    Public Function downloadFile(fileDownloadURL As String, localFileName As String, throwExceptionIfLocalFileExists As Boolean, Optional throwExceptionIfError As Boolean = True) As Boolean
        Dim fileWriteStream As FileStream = Nothing
        Dim httpWebRequest As Net.HttpWebRequest = Nothing
        currentFileSize = 0
        Dim amountDownloaded As Double

        Try
            If urlPreProcessor IsNot Nothing Then
                fileDownloadURL = urlPreProcessor(fileDownloadURL)
            End If
            lastAccessedURL = fileDownloadURL

            If File.Exists(localFileName) Then
                If throwExceptionIfLocalFileExists Then
                    lastException = New localFileAlreadyExistsException(String.Format("The local file found at {0}{1}{0} already exists.", Chr(34), localFileName))
                    Throw lastException
                Else
                    File.Delete(localFileName)
                End If
            End If

            ' We create a new data buffer to hold the stream of data from the web server.
            Dim dataBuffer As Byte() = New Byte(intDownloadBufferSize) {}

            httpWebRequest = DirectCast(Net.WebRequest.Create(fileDownloadURL), Net.HttpWebRequest)

            configureProxy(httpWebRequest)
            addParametersToWebRequest(httpWebRequest)

            Dim webResponse As Net.WebResponse = httpWebRequest.GetResponse() ' We now get the web response.
            captureSSLInfo(fileDownloadURL, httpWebRequest)

            ' Gets the size of the remote file on the web server.
            remoteFileSize = CType(webResponse.ContentLength, ULong)

            Dim responseStream As Stream = webResponse.GetResponseStream() ' Gets the response stream.
            fileWriteStream = New FileStream(localFileName, FileMode.Create) ' Creates a file write stream.

            Dim lngBytesReadFromInternet As ULong = responseStream.Read(dataBuffer, 0, dataBuffer.Length) ' Reads some data from the HTTP stream into our data buffer.

            ' We keep looping until all of the data has been downloaded.
            While lngBytesReadFromInternet <> 0
                ' We calculate the current file size by adding the amount of data that we've so far
                ' downloaded from the server repeatedly to a variable called "currentFileSize".
                currentFileSize += lngBytesReadFromInternet

                fileWriteStream.Write(dataBuffer, 0, lngBytesReadFromInternet) ' Writes the data directly to disk.

                amountDownloaded = (currentFileSize / remoteFileSize) * 100
                httpDownloadProgressPercentage = CType(Math.Round(amountDownloaded, 0), Short) ' Update the download percentage value.
                downloadStatusUpdateInvoker()

                lngBytesReadFromInternet = CType(responseStream.Read(dataBuffer, 0, dataBuffer.Length), ULong) ' Reads more data into our data buffer.
            End While

            fileWriteStream.Close() ' Closes the file stream.
            fileWriteStream.Dispose() ' Disposes the file stream.

            If downloadStatusUpdaterThread IsNot Nothing And boolRunDownloadStatusUpdatePluginInSeparateThread Then
                downloadStatusUpdaterThread.Abort()
                downloadStatusUpdaterThread = Nothing
            End If

            abortDownloadStatusUpdaterThread()

            Return True
        Catch ex As Threading.ThreadAbortException
            abortDownloadStatusUpdaterThread()

            If httpWebRequest IsNot Nothing Then httpWebRequest.Abort()

            If fileWriteStream IsNot Nothing Then
                fileWriteStream.Close() ' Closes the file stream.
                fileWriteStream.Dispose() ' Disposes the file stream.
            End If

            Return False
        Catch ex As Exception
            abortDownloadStatusUpdaterThread()

            lastException = ex
            If fileWriteStream IsNot Nothing Then
                fileWriteStream.Close() ' Closes the file stream.
                fileWriteStream.Dispose() ' Disposes the file stream.
            End If

            If Not throwExceptionIfError Then Return False

            If customErrorHandler IsNot Nothing Then
                customErrorHandler.DynamicInvoke(ex, Me)
                ' Since we handled the exception with an injected custom error handler, we can now exit the function with the return of a False value.
                Return False
            End If

            If TypeOf ex Is Net.WebException Then
                Dim ex2 As Net.WebException = DirectCast(ex, Net.WebException)

                If ex2.Status = Net.WebExceptionStatus.ProtocolError Then
                    Throw handleWebExceptionProtocolError(fileDownloadURL, ex2)
                    Return False
                ElseIf ex2.Status = Net.WebExceptionStatus.TrustFailure Then
                    lastException = New sslErrorException("There was an error establishing an SSL connection.", ex2)
                    Throw lastException
                    Return False
                ElseIf ex2.Status = Net.WebExceptionStatus.NameResolutionFailure Then
                    Dim strDomainName As String = Text.RegularExpressions.Regex.Match(lastAccessedURL, "(?:http(?:s){0,1}://){0,1}(.*)/", Text.RegularExpressions.RegexOptions.Singleline).Groups(1).Value
                    lastException = New dnsLookupError(String.Format("There was an error while looking up the DNS records for the domain name {0}{1}{0}.", Chr(34), strDomainName), ex2)
                    Throw lastException
                    Return False
                End If

                lastException = New Net.WebException(ex.Message, ex2)
                Throw lastException
                Return False
            End If

            Return False
        End Try
    End Function

    ''' <summary>Performs an HTTP Request for data from a web server.</summary>
    ''' <param name="url">This is the URL that the program will send to the web server in the HTTP request. Do not include any GET variables in the URL, use the addGETData() function before calling this function.</param>
    ''' <param name="httpResponseText">This is a ByRef variable so declare it before passing it to this function, think of this as a pointer. The HTML/text content that the web server on the other end responds with is put into this variable and passed back in a ByRef function.</param>
    ''' <returns>A Boolean value. If the HTTP operation was successful it returns a TRUE value, if not FALSE.</returns>
    ''' <exception cref="Net.WebException">If this function throws a Net.WebException then something failed during the HTTP request.</exception>
    ''' <exception cref="Exception">If this function throws a general Exception, something really went wrong; something that the function normally doesn't handle.</exception>
    ''' <exception cref="httpProtocolException">This exception is thrown if the server responds with an HTTP Error.</exception>
    ''' <exception cref="sslErrorException">If this function throws an sslErrorException, an error occurred while negotiating an SSL connection.</exception>
    ''' <exception cref="dnsLookupError">If this function throws a dnsLookupError exception it means that the domain name wasn't able to be resolved properly.</exception>
    ''' <example>httpPostObject.getWebData("http://www.myserver.com/mywebpage", httpResponseText)</example>
    ''' <param name="throwExceptionIfError">Normally True. If True this function will throw an exception if an error occurs. If set to False, the function simply returns False if an error occurs; this is a much more simpler way to handle errors.</param>
    ''' <param name="shortRangeTo">This controls how much data is downloaded from the server.</param>
    ''' <param name="shortRangeFrom">This controls how much data is downloaded from the server.</param>
    Public Function getWebData(ByVal url As String, ByRef httpResponseText As String, shortRangeFrom As Short, shortRangeTo As Short, Optional throwExceptionIfError As Boolean = True) As Boolean
        Dim httpWebRequest As Net.HttpWebRequest = Nothing

        Try
            If urlPreProcessor IsNot Nothing Then
                url = urlPreProcessor(url)
            End If
            lastAccessedURL = url

            If getData.Count <> 0 Then url &= "?" & Me.getGETDataString

            httpWebRequest = DirectCast(Net.WebRequest.Create(url), Net.HttpWebRequest)
            httpWebRequest.AddRange(shortRangeFrom, shortRangeTo)

            configureProxy(httpWebRequest)
            addParametersToWebRequest(httpWebRequest)
            addPostDataToWebRequest(httpWebRequest)

            Dim httpWebResponse As Net.WebResponse = httpWebRequest.GetResponse()
            captureSSLInfo(url, httpWebRequest)

            Dim httpInStream As New StreamReader(httpWebResponse.GetResponseStream())
            Dim httpTextOutput As String = httpInStream.ReadToEnd.Trim()
            httpResponseHeaders = httpWebResponse.Headers

            httpInStream.Close()
            httpInStream.Dispose()

            httpWebResponse.Close()
            httpWebResponse = Nothing
            httpWebRequest = Nothing

            httpResponseText = convertLineFeeds(httpTextOutput).Trim()
            strLastHTTPServerResponse = httpResponseText

            Return True
        Catch ex As Exception
            If TypeOf ex Is Threading.ThreadAbortException Then
                If httpWebRequest IsNot Nothing Then httpWebRequest.Abort()
                Return False
            End If

            lastException = ex
            If Not throwExceptionIfError Then Return False

            If customErrorHandler IsNot Nothing Then
                customErrorHandler.DynamicInvoke(ex, Me)
                ' Since we handled the exception with an injected custom error handler, we can now exit the function with the return of a False value.
                Return False
            End If

            If TypeOf ex Is Net.WebException Then
                Dim ex2 As Net.WebException = DirectCast(ex, Net.WebException)

                If ex2.Status = Net.WebExceptionStatus.ProtocolError Then
                    Throw handleWebExceptionProtocolError(url, ex2)
                    Return False
                ElseIf ex2.Status = Net.WebExceptionStatus.TrustFailure Then
                    lastException = New sslErrorException("There was an error establishing an SSL connection.", ex2)
                    Throw lastException
                    Return False
                ElseIf ex2.Status = Net.WebExceptionStatus.NameResolutionFailure Then
                    Dim strDomainName As String = Text.RegularExpressions.Regex.Match(lastAccessedURL, "(?:http(?:s){0,1}://){0,1}(.*)/", Text.RegularExpressions.RegexOptions.Singleline).Groups(1).Value
                    lastException = New dnsLookupError(String.Format("There was an error while looking up the DNS records for the domain name {0}{1}{0}.", Chr(34), strDomainName), ex2)
                    Throw lastException
                    Return False
                End If

                lastException = New Net.WebException(ex.Message, ex2)
                Throw lastException
                Return False
            End If

            lastException = New Exception(ex.Message, ex)
            Throw lastException
            Return False
        End Try
    End Function

    ''' <summary>Performs an HTTP Request for data from a web server.</summary>
    ''' <param name="url">This is the URL that the program will send to the web server in the HTTP request. Do not include any GET variables in the URL, use the addGETData() function before calling this function.</param>
    ''' <param name="httpResponseText">This is a ByRef variable so declare it before passing it to this function, think of this as a pointer. The HTML/text content that the web server on the other end responds with is put into this variable and passed back in a ByRef function.</param>
    ''' <returns>A Boolean value. If the HTTP operation was successful it returns a TRUE value, if not FALSE.</returns>
    ''' <exception cref="Net.WebException">If this function throws a Net.WebException then something failed during the HTTP request.</exception>
    ''' <exception cref="Exception">If this function throws a general Exception, something really went wrong; something that the function normally doesn't handle.</exception>
    ''' <exception cref="httpProtocolException">This exception is thrown if the server responds with an HTTP Error.</exception>
    ''' <exception cref="sslErrorException">If this function throws an sslErrorException, an error occurred while negotiating an SSL connection.</exception>
    ''' <exception cref="dnsLookupError">If this function throws a dnsLookupError exception it means that the domain name wasn't able to be resolved properly.</exception>
    ''' <example>httpPostObject.getWebData("http://www.myserver.com/mywebpage", httpResponseText)</example>
    ''' <param name="throwExceptionIfError">Normally True. If True this function will throw an exception if an error occurs. If set to False, the function simply returns False if an error occurs; this is a much more simpler way to handle errors.</param>
    Public Function getWebData(ByVal url As String, ByRef httpResponseText As String, Optional throwExceptionIfError As Boolean = True) As Boolean
        Dim httpWebRequest As Net.HttpWebRequest = Nothing

        Try
            If urlPreProcessor IsNot Nothing Then
                url = urlPreProcessor(url)
            End If
            lastAccessedURL = url

            If getData.Count <> 0 Then url &= "?" & getGETDataString()

            httpWebRequest = DirectCast(Net.WebRequest.Create(url), Net.HttpWebRequest)

            configureProxy(httpWebRequest)
            addParametersToWebRequest(httpWebRequest)
            addPostDataToWebRequest(httpWebRequest)

            Dim httpWebResponse As Net.WebResponse = httpWebRequest.GetResponse()
            captureSSLInfo(url, httpWebRequest)

            Dim httpInStream As New StreamReader(httpWebResponse.GetResponseStream())
            Dim httpTextOutput As String = httpInStream.ReadToEnd.Trim()
            httpResponseHeaders = httpWebResponse.Headers

            httpInStream.Close()
            httpInStream.Dispose()

            httpWebResponse.Close()
            httpWebResponse = Nothing
            httpWebRequest = Nothing

            httpResponseText = convertLineFeeds(httpTextOutput).Trim()
            strLastHTTPServerResponse = httpResponseText

            Return True
        Catch ex As Exception
            If TypeOf ex Is Threading.ThreadAbortException Then
                If httpWebRequest IsNot Nothing Then httpWebRequest.Abort()
                Return False
            End If

            lastException = ex
            If Not throwExceptionIfError Then Return False

            If customErrorHandler IsNot Nothing Then
                customErrorHandler.DynamicInvoke(ex, Me)
                ' Since we handled the exception with an injected custom error handler, we can now exit the function with the return of a False value.
                Return False
            End If

            If TypeOf ex Is Net.WebException Then
                Dim ex2 As Net.WebException = DirectCast(ex, Net.WebException)

                If ex2.Status = Net.WebExceptionStatus.ProtocolError Then
                    Throw handleWebExceptionProtocolError(url, ex2)
                    Return False
                ElseIf ex2.Status = Net.WebExceptionStatus.TrustFailure Then
                    lastException = New sslErrorException("There was an error establishing an SSL connection.", ex2)
                    Throw lastException
                    Return False
                ElseIf ex2.Status = Net.WebExceptionStatus.NameResolutionFailure Then
                    Dim strDomainName As String = Text.RegularExpressions.Regex.Match(lastAccessedURL, "(?:http(?:s){0,1}://){0,1}(.*)/", Text.RegularExpressions.RegexOptions.Singleline).Groups(1).Value
                    lastException = New dnsLookupError(String.Format("There was an error while looking up the DNS records for the domain name {0}{1}{0}.", Chr(34), strDomainName), ex2)
                    Throw lastException
                    Return False
                End If

                lastException = New Net.WebException(ex.Message, ex2)
                Throw lastException
                Return False
            End If

            lastException = New Exception(ex.Message, ex)
            Throw lastException
            Return False
        End Try
    End Function

    ''' <summary>Sends data to a URL of your choosing.</summary>
    ''' <param name="url">This is the URL that the program will send to the web server in the HTTP request. Do not include any GET variables in the URL, use the addGETData() function before calling this function.</param>
    ''' <param name="httpResponseText">This is a ByRef variable so declare it before passing it to this function, think of this as a pointer. The HTML/text content that the web server on the other end responds with is put into this variable and passed back in a ByRef function.</param>
    ''' <returns>A Boolean value. If the HTTP operation was successful it returns a TRUE value, if not FALSE.</returns>
    ''' <exception cref="Net.WebException">If this function throws a Net.WebException then something failed during the HTTP request.</exception>
    ''' <exception cref="dataMissingException">If this function throws an postDataMissingException, the Class has nothing to upload so why continue?</exception>
    ''' <exception cref="Exception">If this function throws a general Exception, something really went wrong; something that the function normally doesn't handle.</exception>
    ''' <exception cref="httpProtocolException">This exception is thrown if the server responds with an HTTP Error.</exception>
    ''' <exception cref="sslErrorException">If this function throws an sslErrorException, an error occurred while negotiating an SSL connection.</exception>
    ''' <exception cref="dnsLookupError">If this function throws a dnsLookupError exception it means that the domain name wasn't able to be resolved properly.</exception>
    ''' <example>httpPostObject.uploadData("http://www.myserver.com/myscript", httpResponseText)</example>
    ''' <param name="throwExceptionIfError">Normally True. If True this function will throw an exception if an error occurs. If set to False, the function simply returns False if an error occurs; this is a much more simpler way to handle errors.</param>
    Public Function uploadData(ByVal url As String, ByRef httpResponseText As String, Optional throwExceptionIfError As Boolean = False) As Boolean
        Dim httpWebRequest As Net.HttpWebRequest = Nothing

        Try
            If urlPreProcessor IsNot Nothing Then
                url = urlPreProcessor(url)
            End If
            lastAccessedURL = url

            If postData.Count = 0 Then
                lastException = New dataMissingException("Your HTTP Request contains no POST data. Please add some data to POST before calling this function.")
                Throw lastException
            End If
            If getData.Count <> 0 Then url &= "?" & Me.getGETDataString()

            Dim boundary As String = "---------------------------" & Now.Ticks.ToString("x")
            Dim boundaryBytes As Byte() = Text.Encoding.ASCII.GetBytes((Convert.ToString(vbCr & vbLf & "--") & boundary) & vbCr & vbLf)

            httpWebRequest = DirectCast(Net.WebRequest.Create(url), Net.HttpWebRequest)

            configureProxy(httpWebRequest)
            addParametersToWebRequest(httpWebRequest)

            httpWebRequest.KeepAlive = True
            httpWebRequest.ContentType = "multipart/form-data; boundary=" & boundary
            httpWebRequest.Method = "POST"

            If postData.Count <> 0 Then
                Dim httpRequestWriter As Stream = httpWebRequest.GetRequestStream()
                Dim header As String, fileInfo As FileInfo, formFileObjectInstance As FormFile
                Dim bytes As Byte(), buffer As Byte(), fileStream As FileStream, data As String

                For Each entry As KeyValuePair(Of String, Object) In postData
                    httpRequestWriter.Write(boundaryBytes, 0, boundaryBytes.Length)

                    If TypeOf entry.Value Is FormFile Then
                        formFileObjectInstance = DirectCast(entry.Value, FormFile)

                        If String.IsNullOrEmpty(formFileObjectInstance.remoteFileName) Then
                            fileInfo = New FileInfo(formFileObjectInstance.localFilePath)

                            header = String.Format("Content-Disposition: form-data; name={0}{1}{0}; filename={0}{2}{0}", Chr(34), entry.Key, fileInfo.Name)
                            header &= vbCrLf & "Content-Type: " & formFileObjectInstance.contentType & vbCrLf & vbCrLf
                        Else
                            header = String.Format("Content-Disposition: form-data; name={0}{1}{0}; filename={0}{2}{0}", Chr(34), entry.Key, formFileObjectInstance.remoteFileName)
                            header &= vbCrLf & "Content-Type: " & formFileObjectInstance.contentType & vbCrLf & vbCrLf
                        End If

                        bytes = Text.Encoding.UTF8.GetBytes(header)
                        httpRequestWriter.Write(bytes, 0, bytes.Length)

                        fileStream = New FileStream(formFileObjectInstance.localFilePath, FileMode.Open)
                        buffer = New Byte(32768) {}

                        While fileStream.Read(buffer, 0, buffer.Length) <> 0
                            httpRequestWriter.Write(buffer, 0, buffer.Length)
                        End While

                        fileStream.Close()
                        fileStream.Dispose()
                        fileStream = Nothing
                    Else
                        data = String.Format("Content-Disposition: form-data; name={0}{1}{0}{2}{2}{3}", Chr(34), entry.Key, vbCrLf, entry.Value)
                        bytes = Text.Encoding.UTF8.GetBytes(data)
                        httpRequestWriter.Write(bytes, 0, bytes.Length)
                    End If
                Next

                Dim trailer As Byte() = Text.Encoding.ASCII.GetBytes(vbCrLf & "--" & boundary & "--" & vbCrLf)
                httpRequestWriter.Write(trailer, 0, trailer.Length)
                httpRequestWriter.Close()
            End If

            Dim httpWebResponse As Net.WebResponse = httpWebRequest.GetResponse()
            captureSSLInfo(url, httpWebRequest)

            Dim httpInStream As New StreamReader(httpWebResponse.GetResponseStream())
            Dim httpTextOutput As String = httpInStream.ReadToEnd.Trim()
            httpResponseHeaders = httpWebResponse.Headers

            httpInStream.Close()
            httpInStream.Dispose()

            httpWebResponse.Close()
            httpWebResponse = Nothing
            httpWebRequest = Nothing

            httpResponseText = convertLineFeeds(httpTextOutput).Trim()
            strLastHTTPServerResponse = httpResponseText

            Return True
        Catch ex As Exception
            If TypeOf ex Is Threading.ThreadAbortException Then
                If httpWebRequest IsNot Nothing Then httpWebRequest.Abort()
            End If

            lastException = ex
            If Not throwExceptionIfError Then Return False

            If customErrorHandler IsNot Nothing Then
                customErrorHandler.DynamicInvoke(ex, Me)
                ' Since we handled the exception with an injected custom error handler, we can now exit the function with the return of a False value.
                Return False
            End If

            If TypeOf ex Is Net.WebException Then
                Dim ex2 As Net.WebException = DirectCast(ex, Net.WebException)

                If ex2.Status = Net.WebExceptionStatus.ProtocolError Then
                    Throw handleWebExceptionProtocolError(url, ex2)
                    Return False
                ElseIf ex2.Status = Net.WebExceptionStatus.TrustFailure Then
                    lastException = New sslErrorException("There was an error establishing an SSL connection.", ex2)
                    Throw lastException
                    Return False
                ElseIf ex2.Status = Net.WebExceptionStatus.NameResolutionFailure Then
                    Dim strDomainName As String = Text.RegularExpressions.Regex.Match(lastAccessedURL, "(?:http(?:s){0,1}://){0,1}(.*)/", Text.RegularExpressions.RegexOptions.Singleline).Groups(1).Value
                    lastException = New dnsLookupError(String.Format("There was an error while looking up the DNS records for the domain name {0}{1}{0}.", Chr(34), strDomainName), ex2)
                    Throw lastException
                    Return False
                End If

                lastException = New Net.WebException(ex.Message, ex2)
                Throw lastException
                Return False
            End If

            lastException = New Exception(ex.Message, ex)
            Throw lastException
            Return False
        End Try
    End Function

    Private Sub captureSSLInfo(ByVal url As String, ByRef httpWebRequest As Net.HttpWebRequest)
        If url.StartsWith("https://", StringComparison.OrdinalIgnoreCase) Then
            sslCertificate = New X509Certificates.X509Certificate2(httpWebRequest.ServicePoint.Certificate)
        Else
            sslCertificate = Nothing
        End If
    End Sub

    Private Sub addPostDataToWebRequest(ByRef httpWebRequest As Net.HttpWebRequest)
        If postData.Count = 0 Then
            httpWebRequest.Method = "GET"
        Else
            httpWebRequest.Method = "POST"
            Dim postDataString As String = getPOSTDataString()
            httpWebRequest.ContentType = "application/x-www-form-urlencoded"
            httpWebRequest.ContentLength = postDataString.Length

            Dim httpRequestWriter = New StreamWriter(httpWebRequest.GetRequestStream())
            httpRequestWriter.Write(postDataString)
            httpRequestWriter.Close()
            httpRequestWriter.Dispose()
            httpRequestWriter = Nothing
        End If
    End Sub

    Private Sub addParametersToWebRequest(ByRef httpWebRequest As Net.HttpWebRequest)
        If credentials IsNot Nothing Then
            httpWebRequest.PreAuthenticate = True
            addHTTPHeader("Authorization", "Basic " & Convert.ToBase64String(Text.Encoding.Default.GetBytes(credentials.strUser & ":" & credentials.strPassword)))
        End If

        If strUserAgentString IsNot Nothing Then httpWebRequest.UserAgent = strUserAgentString
        If httpCookies.Count <> 0 Then getCookies(httpWebRequest)
        If additionalHTTPHeaders.Count <> 0 Then getHeaders(httpWebRequest)

        If boolUseHTTPCompression Then
            ' We tell the web server that we can accept a GZIP and Deflate compressed data stream.
            httpWebRequest.Accept = "gzip, deflate"
            httpWebRequest.Headers.Add(Net.HttpRequestHeader.AcceptEncoding, "gzip, deflate")
            httpWebRequest.AutomaticDecompression = Net.DecompressionMethods.GZip Or Net.DecompressionMethods.Deflate
        End If

        httpWebRequest.Timeout = CType(httpTimeOut, Integer)
        httpWebRequest.KeepAlive = True
    End Sub

    Private Sub getCookies(ByRef httpWebRequest As Net.HttpWebRequest)
        Dim cookieContainer As New Net.CookieContainer
        For Each entry As KeyValuePair(Of String, cookieDetails) In httpCookies
            cookieContainer.Add(New Net.Cookie(entry.Key, entry.Value.cookieData, entry.Value.cookiePath, entry.Value.cookieDomain))
        Next
        httpWebRequest.CookieContainer = cookieContainer
    End Sub

    Private Sub getHeaders(ByRef httpWebRequest As Net.HttpWebRequest)
        For Each entry As KeyValuePair(Of String, String) In additionalHTTPHeaders
            httpWebRequest.Headers(entry.Key) = entry.Value
        Next
    End Sub

    Private Sub configureProxy(ByRef httpWebRequest As Net.HttpWebRequest)
        If boolUseProxy Then
            If boolUseSystemProxy Then
                httpWebRequest.Proxy = Net.WebRequest.GetSystemWebProxy()
            Else
                If customProxy Is Nothing Then
                    httpWebRequest.Proxy = Net.WebRequest.GetSystemWebProxy()
                Else
                    httpWebRequest.Proxy = customProxy
                End If
            End If
        End If
    End Sub

    Private Function convertLineFeeds(input As String) As String
        ' Checks to see if the file is in Windows linefeed format or UNIX linefeed format.
        If input.Contains(vbCrLf) Then
            Return input ' It's in Windows linefeed format so we return the output as is.
        Else
            Return input.Replace(vbLf, vbCrLf) ' It's in UNIX linefeed format so we have to convert it to Windows before we return the output.
        End If
    End Function

    Private Function getPOSTDataString() As String
        Dim postDataString As String = ""
        For Each entry As KeyValuePair(Of String, Object) In postData
            If Not entry.Value.GetType.Equals(GetType(FormFile)) Then
                postDataString &= entry.Key.Trim & "=" & Web.HttpUtility.UrlEncode(entry.Value.ToString.Trim) & "&"
            End If
        Next

        If postDataString.EndsWith("&") Then
            postDataString = postDataString.Substring(0, postDataString.Length - 1)
        End If

        Return postDataString
    End Function

    Private Function getGETDataString() As String
        Dim getDataString As String = ""
        For Each entry As KeyValuePair(Of String, String) In getData
            getDataString &= entry.Key.Trim & "=" & Web.HttpUtility.UrlEncode(entry.Value.Trim) & "&"
        Next

        If getDataString.EndsWith("&") Then
            getDataString = getDataString.Substring(0, getDataString.Length - 1)
        End If

        Return getDataString
    End Function

    Private Function handleWebExceptionProtocolError(url As String, ex As Net.WebException) As httpProtocolException
        Dim httpErrorResponse As Net.HttpWebResponse = TryCast(ex.Response, Net.HttpWebResponse)

        If httpErrorResponse IsNot Nothing Then
            If httpErrorResponse.StatusCode = Net.HttpStatusCode.InternalServerError Then
                lastException = New httpProtocolException("HTTP Protocol Error (Server 500 Error) while accessing " & url, ex) With {.httpStatusCode = httpErrorResponse.StatusCode}
            ElseIf httpErrorResponse.StatusCode = Net.HttpStatusCode.NotFound Then
                lastException = New httpProtocolException("HTTP Protocol Error (404 File Not Found) while accessing " & url, ex) With {.httpStatusCode = httpErrorResponse.StatusCode}
            ElseIf httpErrorResponse.StatusCode = Net.HttpStatusCode.Unauthorized Then
                lastException = New httpProtocolException("HTTP Protocol Error (401 Unauthorized) while accessing " & url, ex) With {.httpStatusCode = httpErrorResponse.StatusCode}
            ElseIf httpErrorResponse.StatusCode = Net.HttpStatusCode.ServiceUnavailable Then
                lastException = New httpProtocolException("HTTP Protocol Error (503 Service Unavailable) while accessing " & url, ex) With {.httpStatusCode = httpErrorResponse.StatusCode}
            ElseIf httpErrorResponse.StatusCode = Net.HttpStatusCode.Forbidden Then
                lastException = New httpProtocolException("HTTP Protocol Error (403 Forbidden) while accessing " & url, ex) With {.httpStatusCode = httpErrorResponse.StatusCode}
            Else
                lastException = New httpProtocolException("HTTP Protocol Error while accessing " & url, ex) With {.httpStatusCode = httpErrorResponse.StatusCode}
            End If
        Else
            lastException = New httpProtocolException("HTTP Protocol Error while accessing " & url, ex)
        End If

        Return CType(lastException, httpProtocolException)
    End Function

    Public Function fileSizeToHumanReadableFormat(ByVal size As ULong, Optional roundToNearestWholeNumber As Boolean = False) As String
        Dim result As String

        If size <= (2 ^ 10) Then
            result = size & " Bytes"
        ElseIf size > (2 ^ 10) And size <= (2 ^ 20) Then
            If roundToNearestWholeNumber Then
                result = Math.Round(size / (2 ^ 10), 0) & " KBs"
            Else
                result = Math.Round(size / (2 ^ 10), 2) & " KBs"
            End If
        ElseIf size > (2 ^ 20) And size <= (2 ^ 30) Then
            If roundToNearestWholeNumber Then
                result = Math.Round(size / (2 ^ 20), 0) & " MBs"
            Else
                result = Math.Round(size / (2 ^ 20), 2) & " MBs"
            End If
        ElseIf size > (2 ^ 30) And size <= (2 ^ 40) Then
            If roundToNearestWholeNumber Then
                result = Math.Round(size / (2 ^ 30), 0) & " GBs"
            Else
                result = Math.Round(size / (2 ^ 30), 2) & " GBs"
            End If
        ElseIf size > (2 ^ 40) And size <= (2 ^ 50) Then
            If roundToNearestWholeNumber Then
                result = Math.Round(size / (2 ^ 40), 0) & " TBs"
            Else
                result = Math.Round(size / (2 ^ 40), 2) & " TBs"
            End If
        ElseIf size > (2 ^ 50) And size <= (2 ^ 60) Then
            If roundToNearestWholeNumber Then
                result = Math.Round(size / (2 ^ 50), 0) & " PBs"
            Else
                result = Math.Round(size / (2 ^ 50), 2) & " PBs"
            End If
        Else
            result = "(None)"
        End If

        Return result
    End Function

    Private Function doWeHaveAnInternetConnection() As Boolean
        Try
            Dim ping As New Net.NetworkInformation.Ping()

            If ping.Send("8.8.8.8").Status = Net.NetworkInformation.IPStatus.Success Then
                Return True
            Else
                Return False
            End If
        Catch ex As Exception
            Return False
        End Try
    End Function
End Class