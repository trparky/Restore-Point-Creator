Namespace Functions.osVersionInfo
    Module osVersionInfo
        Public Function isThisAServerOS() As Boolean
            Dim strOSProductType As String = Microsoft.Win32.Registry.LocalMachine.OpenSubKey("SYSTEM\CurrentControlSet\Control\ProductOptions", False).GetValue("ProductType", Nothing)
            Return If(strOSProductType.caseInsensitiveContains("ServerNT"), True, False)
        End Function

        Public Function isThisWindowsXP() As Boolean
            Return If(Environment.OSVersion.Version.Major = 5 And (Environment.OSVersion.Version.Minor = 1 Or 2), True, False)
        End Function

        Public Function isThisWindows7() As Boolean
            Return If(Environment.OSVersion.Version.Major = 6 And Environment.OSVersion.Version.Minor = 1, True, False)
        End Function

        Public Function isThisWindowsVista() As Boolean
            Return If(Environment.OSVersion.Version.Major = 6 And Environment.OSVersion.Version.Minor = 0, True, False)
        End Function

        Public Function isThisWindows8x() As Boolean
            Return If(Environment.OSVersion.Version.Major = 6 And (Environment.OSVersion.Version.Minor = 2 Or 3), True, False)
        End Function

        Public Function isThisWindows10() As Boolean
            Return If(Environment.OSVersion.Version.Major = 10, True, False)
        End Function

        Public Function getFullOSVersionString() As String
            Dim strOSName As String

            Try
                If isThisWindowsXP() Then
                    strOSName = "Windows XP"
                ElseIf isThisWindowsVista() Then
                    strOSName = "Windows Vista"
                ElseIf isThisWindows7() Then
                    strOSName = "Windows 7"
                ElseIf isThisWindows8x() Then
                    strOSName = "Windows 8.x"
                ElseIf isThisWindows10() Then
                    strOSName = "Windows 10"
                Else
                    strOSName = String.Format("Windows NT {0}.{1}", Environment.OSVersion.Version.Major, Environment.OSVersion.Version.Minor)
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
        End Function
    End Module
End Namespace