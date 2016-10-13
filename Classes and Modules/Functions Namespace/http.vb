Namespace Functions.http
    Module http
        Public Function checkForInternetConnection() As Boolean
            If My.Computer.Network.IsAvailable = True Then
                Return True
            Else
                Return False
            End If
        End Function

        ''' <summary>Creates a User Agent String for this program to be used in HTTP requests.</summary>
        ''' <returns>String type.</returns>
        Private Function createHTTPUserAgentHeaderString() As String
            If globalVariables.version.boolBeta = True Then
                Return String.Format("{0} version {1} Public Beta {2} on {3}", globalVariables.programName, globalVariables.version.strFullVersionString, globalVariables.version.shortBetaVersion, osVersionInfo.getFullOSVersionString())
            ElseIf globalVariables.version.boolReleaseCandidate = True Then
                Return String.Format("{0} version {1} Release Candidate {2} on {3}", globalVariables.programName, globalVariables.version.strFullVersionString, globalVariables.version.shortReleaseCandidateVersion, osVersionInfo.getFullOSVersionString())
            Else
                Return String.Format("{0} version {1} on {2}", globalVariables.programName, globalVariables.version.strFullVersionString, osVersionInfo.getFullOSVersionString())
            End If
        End Function

        Public Function downloadFile(ByVal urlToDownloadFrom As String, ByRef memStream As IO.MemoryStream) As Boolean
            Try
                Dim httpHelper As httpHelper = createNewHTTPHelperObject()
                Return httpHelper.getDownloadDataStream(urlToDownloadFrom, memStream, False)
            Catch ex As Exception
                Return False
            End Try
        End Function

        Public Function downloadFile(urlToDownloadFrom As String, localFileName As String) As Boolean
            Try
                Dim httpHelper As httpHelper = createNewHTTPHelperObject()
                Return httpHelper.downloadFile(urlToDownloadFrom, localFileName, False)
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
                    If ex2.Message.Contains("(500)") = True Then
                        eventLogFunctions.writeToSystemEventLog(String.Format("The server responded with an HTTP Protocol Error (Server 500 Error) while accessing the URL {0}{1}{0}.", Chr(34), lastAccessedURL), EventLogEntryType.Error)
                    Else
                        eventLogFunctions.writeToSystemEventLog(String.Format("The server responded with an HTTP Protocol Error while accessing the URL {0}{1}{0}.", Chr(34), lastAccessedURL), EventLogEntryType.Error)
                    End If
                ElseIf ex2.Status = Net.WebExceptionStatus.TrustFailure Then
                    eventLogFunctions.writeToSystemEventLog(String.Format("There was an error establishing an SSL connection while accessing the URL {0}{1}{0}.", Chr(34), lastAccessedURL), EventLogEntryType.Error)
                Else
                    eventLogFunctions.writeToSystemEventLog(String.Format("The server responded with an HTTP Protocol Error while accessing the URL {0}{1}{0}.", Chr(34), lastAccessedURL), EventLogEntryType.Error)
                End If
            ElseIf TypeOf ex Is httpProtocolException Then
                eventLogFunctions.writeToSystemEventLog(String.Format("The server responded with an HTTP error while accessing the URL {0}{1}{0}. This may be because the web site is down or some other kind of issue. Please check back at at later time.", Chr(34), lastAccessedURL), EventLogEntryType.Warning)
            ElseIf TypeOf ex Is sslErrorException Then
                eventLogFunctions.writeToSystemEventLog("An HTTP SSL error occurred.", EventLogEntryType.Error)
            End If
        End Sub

        Public Function createNewHTTPHelperObject() As httpHelper
            Dim httpHelper As New httpHelper
            httpHelper.setUserAgent(createHTTPUserAgentHeaderString())
            httpHelper.addHTTPHeader("PROGRAM_NAME", globalVariables.programName)
            httpHelper.addHTTPHeader("PROGRAM_VERSION", globalVariables.version.strFullVersionString)
            httpHelper.addHTTPHeader("OPERATING_SYSTEM", osVersionInfo.getFullOSVersionString())
            httpHelper.useHTTPCompression(True)
            httpHelper.setProxyMode(My.Settings.useHTTPProxy)
            httpHelper.setHTTPTimeout(30)

            If IO.File.Exists("tom") = True Then
                httpHelper.addPOSTData("dontcount", "True")
            End If

            httpHelper.setURLPreProcessor(Function(ByVal strURLInput As String) As String
                                              Try
                                                  If strURLInput.Trim.StartsWith("http", StringComparison.OrdinalIgnoreCase) = False Then
                                                      If My.Settings.useSSL = True Then
                                                          Debug.WriteLine("The setURLPreProcessor code transformed """ & strURLInput & """ to ""https://" & strURLInput & """.")
                                                          Return "https://" & strURLInput
                                                      Else
                                                          Debug.WriteLine("The setURLPreProcessor code transformed """ & strURLInput & """ to ""http://" & strURLInput & """.")
                                                          Return "http://" & strURLInput
                                                      End If
                                                  Else
                                                      Debug.WriteLine("The setURLPreProcessor code didn't have to do anything to the input """ & strURLInput & """.")
                                                      Return strURLInput
                                                  End If
                                              Catch ex As Exception
                                                  Debug.WriteLine("The setURLPreProcessor code errored out with an input of """ & strURLInput & """.")
                                                  Return strURLInput
                                              End Try
                                          End Function)

            httpHelper.setCustomErrorHandler(Function(ex As Exception, classInstance As httpHelper) As Boolean
                                                 customHTTPHelperErrorHandler(ex, classInstance)
                                                 Return True
                                             End Function)

            Return httpHelper
        End Function
    End Module
End Namespace