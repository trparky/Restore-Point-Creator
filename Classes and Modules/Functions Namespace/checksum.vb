Imports System.Text.RegularExpressions

Namespace Functions.checksum
    Module checksum
        Private Function SHA256ChecksumFile(ByVal filename As String) As String
            Dim SHA1Engine As New Security.Cryptography.SHA256CryptoServiceProvider

            Dim FileStream As New IO.FileStream(filename, IO.FileMode.Open, IO.FileAccess.Read, IO.FileShare.Read, 10 * 1048576, IO.FileOptions.SequentialScan)
            Dim Output As Byte() = SHA1Engine.ComputeHash(FileStream)
            FileStream.Close()

            Return BitConverter.ToString(Output).ToLower().Replace("-", "").Trim
        End Function

        Private Function SHA256ChecksumStream(ByRef stream As IO.Stream) As String
            Dim SHA1Engine As New Security.Cryptography.SHA256CryptoServiceProvider

            Dim Output As Byte() = SHA1Engine.ComputeHash(stream)
            Return BitConverter.ToString(Output).ToLower().Replace("-", "").Trim
        End Function

        Public Function verifyChecksum(urlOfChecksumFile As String, memStream As IO.MemoryStream, boolGiveUserAnErrorMessage As Boolean) As Boolean
            Dim checksumFromWeb As String = Nothing
            memStream.Position = 0

            Dim httpHelper As httpHelper = http.createNewHTTPHelperObject()

            Try
                If httpHelper.getWebData(urlOfChecksumFile, checksumFromWeb) = True Then
                    ' Checks to see if we have a valid SHA1 file.
                    If Regex.IsMatch(checksumFromWeb, "([a-zA-Z0-9]{64})") = True Then
                        ' Now that we have a valid SHA256 file we need to parse out what we want.
                        checksumFromWeb = Regex.Match(checksumFromWeb, "([a-zA-Z0-9]{64})").Groups(1).Value.Trim

                        ' Now we do the actual checksum verification by passing the name of the file to the SHA256() function
                        ' which calculates the checksum of the file on disk. We then compare it to the checksum from the web.
                        If SHA256ChecksumStream(memStream).Equals(checksumFromWeb, StringComparison.OrdinalIgnoreCase) Then
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
                            MsgBox("Invalid SHA2 file detected. Update process aborted.", MsgBoxStyle.Critical, "Restore Point Creator")
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
                eventLogFunctions.writeCrashToEventLog(ex)

                If boolGiveUserAnErrorMessage = True Then
                    MsgBox("There was an error downloading the checksum verification file. Update process aborted.", MsgBoxStyle.Critical, "Restore Point Creator")
                End If

                Return False
            End Try
        End Function

        Public Function verifyChecksum(urlOfChecksumFile As String, fileToVerify As String, boolGiveUserAnErrorMessage As Boolean) As Boolean
            Dim checksumFromWeb As String = Nothing

            Dim httpHelper As httpHelper = http.createNewHTTPHelperObject()

            Try
                If httpHelper.getWebData(urlOfChecksumFile, checksumFromWeb) = True Then
                    ' Checks to see if we have a valid SHA1 file.
                    If Regex.IsMatch(checksumFromWeb, "([a-zA-Z0-9]{64})") = True Then
                        ' Now that we have a valid SHA256 file we need to parse out what we want.
                        checksumFromWeb = Regex.Match(checksumFromWeb, "([a-zA-Z0-9]{64})").Groups(1).Value.Trim

                        ' Now we do the actual checksum verification by passing the name of the file to the SHA256() function
                        ' which calculates the checksum of the file on disk. We then compare it to the checksum from the web.
                        If SHA256ChecksumFile(fileToVerify).Equals(checksumFromWeb, StringComparison.OrdinalIgnoreCase) Then
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
                            MsgBox("Invalid SHA2 file detected. Update process aborted.", MsgBoxStyle.Critical, "Restore Point Creator")
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
                eventLogFunctions.writeCrashToEventLog(ex)

                If boolGiveUserAnErrorMessage = True Then
                    MsgBox("There was an error downloading the checksum verification file. Update process aborted.", MsgBoxStyle.Critical, "Restore Point Creator")
                End If

                Return False
            End Try
        End Function
    End Module
End Namespace