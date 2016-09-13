Imports System.Runtime.CompilerServices
Imports System.Text.RegularExpressions

Module DateExtensions
    ''' <summary>Converts .NET Date Objects to a standard 64-bit Integer-based UNIX Timestamp.</summary>
    ''' <param name="inputDate">A .NET Date Object.</param>
    ''' <returns>A 64-but Integer representing a standard UNIX timestamp.</returns>
    <Extension()>
    Public Function toUNIXTimestamp(ByVal inputDate As Date) As ULong
        If inputDate.IsDaylightSavingTime = True Then
            inputDate = DateAdd(DateInterval.Hour, -1, inputDate)
        End If

        'Return DateDiff(DateInterval.Second, #1/1/1970#, inputDate)
        Return DateDiff(DateInterval.Second, New DateTime(1970, 1, 1, 0, 0, 0, 0), inputDate)
    End Function
End Module

Module StringExtensions
    ' PHP like addSlashes and stripSlashes. Call using String.addSlashes() and String.stripSlashes().
    <Extension()>
    Public Function addSlashes(unsafeString As String) As String
        Return Regex.Replace(unsafeString, "([\000\010\011\012\015\032\042\047\134\140])", "\$1")
    End Function

    ' Un-quote string quoted with addslashes()
    <Extension()>
    Public Function stripSlashes(safeString As String) As String
        Return Regex.Replace(safeString, "\\([\000\010\011\012\015\032\042\047\134\140])", "$1")
    End Function

    ''' <summary>This function operates a lot like Contains() but is case-InSeNsItIvE.</summary>
    ''' <param name="needle">The String containing what you want to search for.</param>
    ''' <return>Returns a Boolean value.</return>
    <Extension()>
    Public Function caseInsensitiveContains(haystack As String, needle As String) As Boolean
        Dim index As Integer = haystack.IndexOf(needle, StringComparison.OrdinalIgnoreCase)
        Return (index >= 0)

        'Try
        '    If boolDoEscaping = True Then needle = Regex.Escape(needle)
        '    Return Regex.IsMatch(haystack, needle, RegexOptions.IgnoreCase)
        'Catch ex As Exception
        '    Return False
        'End Try
    End Function

    <Extension()>
    Public Function stringCompare(str1 As String, str2 As String, Optional boolCaseInsensitive As Boolean = True)
        If boolCaseInsensitive = True Then
            Return str1.Trim.Equals(str2.Trim, StringComparison.OrdinalIgnoreCase)
        Else
            Return str1.Trim.Equals(str2.Trim, StringComparison.Ordinal)
        End If
    End Function

    ''' <summary>This function performs RegEx search on a String. This function operates a lot like Contains().</summary>
    ''' <param name="needle">The String containing what you want to search for.</param>
    ''' <param name="boolDoEscaping">A Boolean value. Normally you would pass a correct RegEx pattern to this function so normally this value would be False.</param>
    ''' <return>Returns a Boolean value.</return>
    <Extension()>
    Public Function regExSearch(haystack As String, needle As String, Optional boolDoEscaping As Boolean = False) As Boolean
        Try
            If boolDoEscaping = True Then needle = Regex.Escape(needle)
            Return Regex.IsMatch(haystack, needle, RegexOptions.IgnoreCase)
        Catch ex As Exception
            Return False
        End Try
    End Function
End Module

Module RtfExtensions
    <Extension()>
    Public Function ToRtf(s As String) As String
        Return "{\rtf1\ansi" + s + "}"
    End Function

    <Extension()>
    Public Function ToBold(s As String) As String
        Return String.Format("\b {0}\b0 ", s)
    End Function
End Module