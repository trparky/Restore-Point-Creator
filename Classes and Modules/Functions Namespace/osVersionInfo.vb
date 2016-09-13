Namespace Functions.osVersionInfo
    Module osVersionInfo
        Public Function isThisAServerOS() As Boolean
            Dim strOSProductType As String = Microsoft.Win32.Registry.LocalMachine.OpenSubKey("SYSTEM\CurrentControlSet\Control\ProductOptions", False).GetValue("ProductType", Nothing)

            If strOSProductType.caseInsensitiveContains("ServerNT") = True Then
                Return True
            Else
                Return False
            End If
        End Function

        Public Function isWindowsVista() As Boolean
            If Environment.OSVersion.Version.Major = 6 And Environment.OSVersion.Version.Minor = 0 Then
                Return True
            Else
                Return False
            End If
        End Function

        Public Function isThisWindows8x() As Boolean
            If Environment.OSVersion.ToString.Contains("6.2") = True Or Environment.OSVersion.ToString.Contains("6.3") = True Then
                Return True
            Else
                Return False
            End If
        End Function

        Public Function isThisWindows10() As Boolean
            If Environment.OSVersion.Version.Major = 10 Then
                Return True
            Else
                Return False
            End If
        End Function

        Public Function getFullOSVersionString() As String
            Dim strOSName As String

            Try
                Dim intOSMajorVersion As Integer = Environment.OSVersion.Version.Major
                Dim intOSMinorVersion As Integer = Environment.OSVersion.Version.Minor

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
                    strOSName &= " 64-bit"
                Else
                    strOSName &= " 32-bit"
                End If
            Catch ex As Exception
                Try
                    Return "Unknown Windows Operating System (" & Environment.OSVersion.VersionString & ")"
                Catch ex2 As Exception
                    Return "Unknown Windows Operating System"
                End Try
            End Try

            Return strOSName

            'Try
            '    strOSName &= String.Format(" (Microsoft .NET {0}.{1})", Environment.Version.Major, Environment.Version.Minor)
            'Catch ex As Exception
            '    strOSName &= " (Unknown Microsoft .NET Version)"
            'End Try
        End Function
    End Module
End Namespace