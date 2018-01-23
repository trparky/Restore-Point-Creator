' DON'T TOUCH THIS!  SERIOUSLY... DON'T FUCKING TOUCH THIS SHIT!!!!
' DIDN'T I TELL YOU NOT TO TOUCH THIS FUCKING SHIT!  AHH CRAP, YOU DID TOUCH THIS FUCKING SHIT!  STOP FUCKING TOUCHING THIS!
Public Class MyApp
    Inherits System.Windows.Application
End Class

Module keepThisVeryImportantStuff
    ''' <summary>Creates a Windows Jumplist Task Object.</summary>
    ''' <param name="jumpList">This is a ByRef argument. We pass the JumpList Object to this subroutine as a pointer to the original JumpList Object created in the createJumpListTaskItems() subroutine.</param>
    ''' <param name="name">The name you want to give to your new Jumplist item.</param>
    ''' <param name="category">The category that you want the new Jumplist item to be in.</param>
    ''' <param name="cmdLineArgument">Optional parameter. Used to set a command line argument if needed.</param>
    Sub createJumpListItem(ByRef jumpList As Windows.Shell.JumpList, ByVal name As String, ByVal category As String, ByVal Optional cmdLineArgument As String = Nothing)
        Try
            Dim jumpTask As New Windows.Shell.JumpTask With {
                .ApplicationPath = Application.ExecutablePath,
                .IconResourcePath = Application.ExecutablePath,
                .IconResourceIndex = 0,
                .Title = name,
                .CustomCategory = category
            }

            If Not String.IsNullOrEmpty(cmdLineArgument) Then jumpTask.Arguments = cmdLineArgument

            jumpList.JumpItems.Add(jumpTask)
        Catch ex As Exception
            Functions.eventLogFunctions.writeCrashToApplicationLogFile(ex)
        End Try
    End Sub

    Public Sub createJumpListTaskItems()
        Try
            Dim jumpList As New Windows.Shell.JumpList

            createJumpListItem(jumpList, "Create System Checkpoint", "Create System Restore Point", globalVariables.commandLineSwitches.createRestorePoint)
            createJumpListItem(jumpList, "Create Custom Named", "Create System Restore Point", globalVariables.commandLineSwitches.createCustomRestorePoint)
            createJumpListItem(jumpList, "Restore Point Creator Event Log", "Utilities", globalVariables.commandLineSwitches.viewEventLog)
            createJumpListItem(jumpList, "Launch with forced UAC prompt", "Utilities", globalVariables.commandLineSwitches.forceUAC)

            Dim app As New MyApp

            Windows.Shell.JumpList.SetJumpList(Windows.Application.Current, jumpList)
        Catch ex As Exception
            Functions.eventLogFunctions.writeCrashToApplicationLogFile(ex)
        End Try
    End Sub
End Module
' DON'T TOUCH THIS!  SERIOUSLY... DON'T FUCKING TOUCH THIS SHIT!!!!
' DIDN'T I TELL YOU NOT TO TOUCH THIS FUCKING SHIT!  AHH CRAP, YOU DID TOUCH THIS FUCKING SHIT!  STOP FUCKING TOUCHING THIS!