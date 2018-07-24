Imports System.Text

Namespace exceptionHandler
    Module exeptionHandler
        Public Sub loadExceptionHandler()
            ' Subscribe to thread (unhandled) exception events
            Dim handler As ThreadExceptionHandler = New ThreadExceptionHandler()
            AddHandler Application.ThreadException, AddressOf handler.Application_ThreadException
            AddHandler AppDomain.CurrentDomain.UnhandledException, AddressOf handler.Application_UnhandledException
        End Sub

        ''' <summary>This function tells the program if the program should handle the exception silently or if it should show the crash message.</summary>
        ''' <param name="exceptionType">The type of the exception.</param>
        ''' <param name="exceptionObject">The exception object from the TRY block.</param>
        ''' <returns>Return a TRUE value to have the program show the crash window. Return a FALSE value to have the program NOT show the crash window.</returns>
        Public Function handleCrashWithAnErrorOrRedirectUserInstead(exceptionType As Type, exceptionObject As Exception) As Boolean
            ' There are three options in the code of this function...
            ' 1. Return a TRUE value to have the program SHOW the crash window to the user.
            ' 2. Return a FALSE value to have the program NOT SHOW the crash window to the user.
            ' 3. Execute a Process.GetCurrentProcess.Kill() to end program execution immediately.

            Dim exceptionMessage As String = exceptionObject.Message

            If exceptionType = GetType(IO.FileLoadException) And exceptionMessage.regExSearch("(?:restoreToSystemRestorePoint|createRestorePoint)") = True Then
                MsgBox("There has been an error while loading a required system library for System Restore. Please reboot your computer and try again.", MsgBoxStyle.Critical + MsgBoxStyle.ApplicationModal, "System Restore Point Creator")
                Process.GetCurrentProcess.Kill()
            ElseIf exceptionType = GetType(Configuration.ConfigurationErrorsException) Or exceptionType = GetType(Configuration.ConfigurationException) Then
                MsgBox("There has been an error in loading the program's user configuration data. Please relaunch the program.", MsgBoxStyle.Critical + MsgBoxStyle.ApplicationModal, "System Restore Point Creator")
                Process.GetCurrentProcess.Kill()
            ElseIf exceptionType = GetType(OutOfMemoryException) Then
                MsgBox("Your system appears to have run out of operating memory. Please reboot your computer and try again." & vbCrLf & vbCrLf & "This program will now close.", MsgBoxStyle.Information, "System Restore Point Creator")
                Process.GetCurrentProcess.Kill()
            ElseIf exceptionType = GetType(UnauthorizedAccessException) Then
                Functions.eventLogFunctions.writeCrashToApplicationLogFile(exceptionObject)

                MsgBox("It appears that you don't have the necessary user permissions to do what you are attempting to do. Please contact your system administrator for guidance.", MsgBoxStyle.Exclamation, "System Restore Point Creator")

                Return False
            ElseIf exceptionType = GetType(BadImageFormatException) Then
                Functions.eventLogFunctions.writeCrashToApplicationLogFile(exceptionObject)

                MsgBox("There was an error while loading a required system library. Please check the Event Log Viewer for more information.", MsgBoxStyle.Exclamation, "System Restore Point Creator")

                Return False
            ElseIf exceptionType = GetType(ObjectDisposedException) And exceptionMessage.regExSearch("(?:Please_Wait|SmoothProgressBar|pleaseWaitlblLabel)") = True Then
                ' This is to hopefully catch an annoying crash that I've not been able to track down
                ' so we're going to simply handle it silently with no notification to the user.
                Functions.eventLogFunctions.writeCrashToApplicationLogFile(exceptionObject)
                Return False
            ElseIf exceptionType = GetType(Runtime.InteropServices.COMException) Then
                Functions.eventLogFunctions.writeCrashToApplicationLogFile(exceptionObject)

                MsgBox("System-level APIs and functions that this program requires to operate appear to be damaged or disabled on your system. Your web browser will be opened to a web page with information on how to repair your system." & vbCrLf & vbCrLf & "This program will now close.", MsgBoxStyle.Information, "System Restore Point Creator")

                Functions.support.launchURLInWebBrowser(globalVariables.webURLs.webPages.strCOMExceptionCrash)
                Process.GetCurrentProcess.Kill()
            ElseIf exceptionType = GetType(Management.ManagementException) Then
                Functions.eventLogFunctions.writeCrashToApplicationLogFile(exceptionObject)

                MsgBox("System-level APIs and functions that this program requires to operate appear to be damaged or disabled on your system. Your web browser will be opened to a web page with information on how to repair your system." & vbCrLf & vbCrLf & "This program will now close.", MsgBoxStyle.Information, "System Restore Point Creator")

                Functions.support.launchURLInWebBrowser(globalVariables.webURLs.webPages.strCOMExceptionCrash)
                Process.GetCurrentProcess.Kill()
            ElseIf exceptionType = GetType(TypeInitializationException) And exceptionObject.Message.caseInsensitiveContains("Microsoft.Win32.TaskScheduler.TaskService") Then
                ' Fixes an issue that causes the Crash Handling Window to appear during updates on systems with Avast antivirus installed on them.
                Return False
            End If

            Return True ' This function returns a TRUE statement by default which will cause the program to show the crash window.
        End Function

        ''' <summary>This function tells the program if the program should handle the exception silently or if it should show the crash message.</summary>
        ''' <param name="exceptionObject">The exception object from the TRY block.</param>
        ''' <returns>Return a TRUE value to have the program show the crash window. Return a FALSE value to have the program NOT show the crash window.</returns>
        Public Function handleCrashWithAnErrorOrRedirectUserInstead(exceptionObject As Exception) As Boolean
            Return handleCrashWithAnErrorOrRedirectUserInstead(exceptionObject.GetType, exceptionObject)
        End Function

        Public Sub manuallyLoadCrashWindow(exceptionObject As Exception)
            manuallyLoadCrashWindow(exceptionObject, Functions.support.removeSourceCodePathInfo(exceptionObject.Message), Functions.support.removeSourceCodePathInfo(exceptionObject.StackTrace), exceptionObject.GetType)
        End Sub

        Public Sub manuallyLoadCrashWindow(exceptionObject As Exception, exceptionMessage As String, exceptionStackTrace As String, exceptionType As Type)
            exceptionMessage = Functions.support.removeSourceCodePathInfo(exceptionMessage).Trim
            exceptionStackTrace = Functions.support.removeSourceCodePathInfo(exceptionStackTrace).Trim

            If handleCrashWithAnErrorOrRedirectUserInstead(exceptionType, exceptionObject) = True Then
                Try
                    Functions.miniDump.MiniDump.MiniDumpToFile(globalVariables.strDumpFilePath)
                Catch Ex As Exception
                    ' Does nothing
                End Try

                Dim crashWindow As New frmCrash
                crashWindow.Text = "Critical Application Error Detected!"
                crashWindow.lblHeader.Text = "Critical Application Error Detected!"
                crashWindow.ShowDialog()

                Process.GetCurrentProcess.Kill()
            End If
        End Sub
    End Module
End Namespace

Friend Class ThreadExceptionHandler
    '''
    ''' Handles the thread exception.
    '''
    Public Sub Application_ThreadException(ByVal sender As Object, ByVal exceptionObject As Threading.ThreadExceptionEventArgs)
        Try
            writeCrashToEventLogAndGiveErrorMessage(exceptionObject.Exception)
        Catch ex As Exception
        End Try
    End Sub

    '''
    ''' Handles the thread exception.
    '''
    Public Sub Application_UnhandledException(sender As Object, exceptionObject As UnhandledExceptionEventArgs)
        Try
            writeCrashToEventLogAndGiveErrorMessage(exceptionObject.ExceptionObject)
        Catch ex As Exception
        End Try
    End Sub

    Private Sub writeCrashToEventLogAndGiveErrorMessage(exceptionObject As Exception)
        Try
            Functions.eventLogFunctions.writeCrashToApplicationLogFile(exceptionObject)
            MsgBox("A fatal error occurred and the program will now close. Please check the application event log for more details.", MsgBoxStyle.Critical + MsgBoxStyle.ApplicationModal, "Restore Point Creator Fatal Program Crash")
            Process.GetCurrentProcess.Kill()
        Catch ex As Exception
        End Try
    End Sub
End Class