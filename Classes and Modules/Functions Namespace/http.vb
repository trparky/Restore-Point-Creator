Namespace Functions.http
    Module http
        Public Function checkForInternetConnection() As Boolean
            Return My.Computer.Network.IsAvailable
        End Function

        ''' <summary>Creates a User Agent String for this program to be used in HTTP requests.</summary>
        ''' <returns>String type.</returns>
        Private Function createHTTPUserAgentHeaderString() As String
            If globalVariables.version.boolBeta Then
                Return String.Format("{0} version {1} Public Beta {2} on {3}", globalVariables.programName, globalVariables.version.strFullVersionString, globalVariables.version.shortBetaVersion, osVersionInfo.getFullOSVersionString())
            ElseIf globalVariables.version.boolReleaseCandidate Then
                Return String.Format("{0} version {1} Release Candidate {2} on {3}", globalVariables.programName, globalVariables.version.strFullVersionString, globalVariables.version.shortReleaseCandidateVersion, osVersionInfo.getFullOSVersionString())
            Else
                Return String.Format("{0} version {1} on {2}", globalVariables.programName, globalVariables.version.strFullVersionString, osVersionInfo.getFullOSVersionString())
            End If
        End Function

        Public Function downloadFile(ByVal urlToDownloadFrom As String, ByRef memStream As IO.MemoryStream) As Boolean
            Try
                Dim httpHelper As httpHelper = createNewHTTPHelperObject()
                Dim downloadResult As Boolean = httpHelper.downloadFile(urlToDownloadFrom, memStream, False)
                If Not downloadResult Then eventLogFunctions.writeCrashToEventLog(httpHelper.getLastException)
                Return downloadResult
            Catch ex As Exception
                Return False
            End Try
        End Function

        Private Sub customHTTPHelperErrorHandler(ex As Exception, classInstance As httpHelper)
            eventLogFunctions.writeCrashToEventLog(ex, EventLogEntryType.Warning)
            Dim lastAccessedURL As String = classInstance.getLastAccessedURL()

            If TypeOf ex Is Net.WebException Then
                Dim ex2 As Net.WebException = DirectCast(ex, Net.WebException)

                If ex2.Status = Net.WebExceptionStatus.ProtocolError Then
                    Dim httpErrorResponse As Net.HttpWebResponse = TryCast(ex2.Response, Net.HttpWebResponse)

                    If httpErrorResponse IsNot Nothing Then
                        If httpErrorResponse.StatusCode = Net.HttpStatusCode.InternalServerError Then
                            eventLogFunctions.writeToSystemEventLog(String.Format("The server responded with an HTTP Protocol Error (Server 500 Error) while accessing the URL {0}{1}{0}.", Chr(34), lastAccessedURL), EventLogEntryType.Warning)
                        ElseIf httpErrorResponse.StatusCode = Net.HttpStatusCode.NotFound Then
                            eventLogFunctions.writeToSystemEventLog(String.Format("The server responded with an HTTP Protocol Error (HTTP 404 File Not Found) while accessing the URL {0}{1}{0}.", Chr(34), lastAccessedURL), EventLogEntryType.Warning)
                        ElseIf httpErrorResponse.StatusCode = Net.HttpStatusCode.Unauthorized Then
                            eventLogFunctions.writeToSystemEventLog(String.Format("The server responded with an HTTP Protocol Error (HTTP 401 Unauthorized) while accessing the URL {0}{1}{0}.", Chr(34), lastAccessedURL), EventLogEntryType.Warning)
                        ElseIf httpErrorResponse.StatusCode = Net.HttpStatusCode.ServiceUnavailable Then
                            eventLogFunctions.writeToSystemEventLog(String.Format("The server responded with an HTTP Protocol Error (HTTP 503 Service Unavailable) while accessing the URL {0}{1}{0}.", Chr(34), lastAccessedURL), EventLogEntryType.Warning)
                        ElseIf httpErrorResponse.StatusCode = Net.HttpStatusCode.Forbidden Then
                            eventLogFunctions.writeToSystemEventLog(String.Format("The server responded with an HTTP Protocol Error (HTTP 403 Forbidden) while accessing the URL {0}{1}{0}.", Chr(34), lastAccessedURL), EventLogEntryType.Warning)
                        Else
                            eventLogFunctions.writeToSystemEventLog(String.Format("The server responded with an HTTP Protocol Error while accessing the URL {0}{1}{0}.", Chr(34), lastAccessedURL), EventLogEntryType.Warning)
                        End If
                    Else
                        eventLogFunctions.writeToSystemEventLog(String.Format("The server responded with an HTTP Protocol Error while accessing the URL {0}{1}{0}.", Chr(34), lastAccessedURL), EventLogEntryType.Warning)
                    End If
                ElseIf ex2.Status = Net.WebExceptionStatus.TrustFailure Then
                    eventLogFunctions.writeToSystemEventLog(String.Format("There was an error establishing an SSL connection while accessing the URL {0}{1}{0}.", Chr(34), lastAccessedURL), EventLogEntryType.Warning)
                ElseIf ex2.Status = Net.WebExceptionStatus.NameResolutionFailure Then
                    eventLogFunctions.writeToSystemEventLog(String.Format("There was an error while resolving the domain name when attempting to request the URL {0}{1}{0}.", Chr(34), lastAccessedURL), EventLogEntryType.Warning)
                Else
                    eventLogFunctions.writeToSystemEventLog(String.Format("The server responded with an HTTP Protocol Error while accessing the URL {0}{1}{0}.", Chr(34), lastAccessedURL), EventLogEntryType.Warning)
                End If
            ElseIf TypeOf ex Is httpProtocolException Then
                eventLogFunctions.writeToSystemEventLog(String.Format("The server responded with an HTTP error while accessing the URL {0}{1}{0}. This may be because the web site is down or some other kind of issue. Please check back at at later time.", Chr(34), lastAccessedURL), EventLogEntryType.Warning)
            ElseIf TypeOf ex Is sslErrorException Then
                eventLogFunctions.writeToSystemEventLog("An HTTP SSL error occurred.", EventLogEntryType.Warning)
            End If
        End Sub

        Public Function createNewHTTPHelperObject() As httpHelper
            Dim httpHelper As New httpHelper
            httpHelper.setUserAgent = createHTTPUserAgentHeaderString()
            httpHelper.addHTTPHeader("PROGRAM_NAME", globalVariables.programName)
            httpHelper.addHTTPHeader("PROGRAM_VERSION", globalVariables.version.strFullVersionString)
            httpHelper.addHTTPHeader("OPERATING_SYSTEM", osVersionInfo.getFullOSVersionString())
            httpHelper.useHTTPCompression = True
            httpHelper.setProxyMode = My.Settings.useHTTPProxy
            httpHelper.setHTTPTimeout = My.Settings.httpTimeout

            If My.Settings.useSystemProxyConfig Then
                httpHelper.useSystemProxy = True
            Else
                httpHelper.useSystemProxy = False

                If String.IsNullOrEmpty(My.Settings.proxyUser) Then
                    httpHelper.setProxy(My.Settings.proxyAddress, My.Settings.proxyPort, True)
                Else
                    httpHelper.setProxy(My.Settings.proxyAddress, My.Settings.proxyPort, My.Settings.proxyUser, support.convertFromBase64(My.Settings.proxyPass), True)
                End If
            End If

            If IO.File.Exists("tom") = True Then
                httpHelper.addPOSTData("dontcount", "True")
            End If

            httpHelper.setURLPreProcessor = Function(ByVal strURLInput As String) As String
                                                Try
                                                    If strURLInput.Trim.StartsWith("http", StringComparison.OrdinalIgnoreCase) Then
                                                        Return strURLInput
                                                    Else
                                                        If My.Settings.useSSL Then
                                                            Return "https://" & strURLInput
                                                        Else
                                                            Return "http://" & strURLInput
                                                        End If
                                                    End If
                                                Catch ex As Exception
                                                    Return strURLInput
                                                End Try
                                            End Function

            httpHelper.setCustomErrorHandler = Sub(ex As Exception, classInstance As httpHelper)
                                                   customHTTPHelperErrorHandler(ex, classInstance)
                                               End Sub

            Return httpHelper
        End Function
    End Module
End Namespace