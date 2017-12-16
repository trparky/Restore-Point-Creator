Imports System.Runtime.CompilerServices
Imports System.Text.RegularExpressions

Module ProcessExtensions
    ''' <summary>Gets the parent process of the current process.</summary>
    ''' <param name="process">Pass it Process.GetCurrentProcess.</param>
    ''' <returns>A Process Object.</returns>
    ''' <exception cref="Functions.myExceptions.unableToGetParentProcessException">If this function throws this exception it means that it wasn't able to get the parent process.</exception>
    ''' <exception cref="Functions.myExceptions.integerTryParseException">If this function throws this exception it means that it wasn't able to parse what the Windows WMI returned to it.</exception>
    <Extension()>
    Public Function Parent(process As Process) As Process
        Try
            Using managementObjectSearcher As New Management.ManagementObjectSearcher("root\CIMV2", String.Format("SELECT ParentProcessId FROM Win32_Process WHERE ProcessId = {0}", process.Id))
                If managementObjectSearcher Is Nothing Then
                    Throw New Functions.myExceptions.unableToGetParentProcessException("Management Object Searcher Object was null.")
                Else
                    Dim managementObjectCollection As Management.ManagementObjectCollection = managementObjectSearcher.Get()

                    If managementObjectCollection Is Nothing Then
                        Throw New Functions.myExceptions.unableToGetParentProcessException("Management Object Collection Object was null.")
                    Else
                        If managementObjectCollection.Count = 0 Then
                            Throw New Functions.myExceptions.unableToGetParentProcessException("Management Object Collection Object count was null.")
                        Else
                            If managementObjectCollection(0)("ParentProcessId") Is Nothing Then
                                Throw New Functions.myExceptions.unableToGetParentProcessException("ParentProcessId Object was null.")
                            Else
                                Dim strParentProcessID As String = managementObjectCollection(0)("ParentProcessId").ToString()
                                Dim intParentProcessID As Integer

                                If Integer.TryParse(strParentProcessID, intParentProcessID) Then
                                    Try
                                        Return Process.GetProcessById(intParentProcessID)
                                    Catch ex As Exception
                                        Throw New Functions.myExceptions.unableToGetParentProcessException("Unable to get Process Object by ID.")
                                    End Try
                                Else
                                    Throw New Functions.myExceptions.integerTryParseException("Unable to parse Parent Process ID into an Integer.") With {.strThatCouldNotBeParsedIntoAnInteger = strParentProcessID}
                                End If
                            End If
                        End If
                    End If
                End If
            End Using
        Catch ex As Exception
            Functions.eventLogFunctions.writeCrashToEventLog(ex, EventLogEntryType.Warning)
            Throw New Functions.myExceptions.unableToGetParentProcessException("General error.")
        End Try
    End Function
End Module

Module StringExtensions
    ''' <summary>This function operates a lot like Replace() but is case-InSeNsItIvE.</summary>
    ''' <param name="source">The source String, aka the String where the data will be replaced in.</param>
    ''' <param name="replace">What you want to replace in the String.</param>
    ''' <param name="replaceWith">What you want to replace with in the String.</param>
    ''' <param name="boolEscape">This is an optional parameter, the default is True. This parameter gives you far more control over how the function works. With this parameter set to True the function automatically properly escapes the "replace" parameter for use in the RegEx replace function that operates inside this function. If this parameter is set to False it is up to you, the programmer, to properly escape the value of the "replace" parameter or this function will throw an exception.</param>
    ''' <return>Returns a String value.</return>
    <Extension()>
    Public Function caseInsensitiveReplace(source As String, replace As String, replaceWith As String, Optional boolEscape As Boolean = True) As String
        If boolEscape Then
            Return Regex.Replace(source, Regex.Escape(replace), replaceWith, RegexOptions.IgnoreCase)
        Else
            Return Regex.Replace(source, replace, replaceWith, RegexOptions.IgnoreCase)
        End If
    End Function

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