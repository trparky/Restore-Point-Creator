Imports Microsoft.Win32

Namespace Functions.taskStuff
    Module taskStuff
        Public Function getOurTaskFolder(ByRef taskService As TaskScheduler.TaskService) As TaskScheduler.TaskFolder
            Return If(taskService.GetFolder(globalVariables.taskFolder), taskService.RootFolder.CreateFolder(globalVariables.taskFolder))
        End Function

        Public Function doesTaskFolderExist() As Boolean
            ' Apparently the SYSTEM user can't create scheduled tasks, so to prevent crashes we check to see if
            ' we are running in the SYSTEM user context and if we are we abort this subroutine by returning a
            ' True value even though the Task Folder may not actually exist.
            If privilegeChecks.areWeRunningAsSystemUser() = True Then
                Return True
            End If

            Try
                Return TaskScheduler.TaskService.Instance.RootFolder.SubFolders.Exists(globalVariables.taskFolder)
            Catch ex As Exception
                Return False
            End Try
        End Function

        Public Sub deleteAtUserLogonTask()
            Try
                Dim taskService As New TaskScheduler.TaskService
                Dim taskFolderObject As TaskScheduler.TaskFolder = getOurTaskFolder(taskService)

                taskFolderObject.DeleteTask("Create a Restore Point at User Logon", False)

                taskFolderObject.Dispose()
                taskService.Dispose()

                taskFolderObject = Nothing
                taskService = Nothing
            Catch ex As Exception
                Threading.Thread.CurrentThread.CurrentUICulture = New Globalization.CultureInfo("en-US")
                exceptionHandler.manuallyLoadCrashWindow(ex, ex.Message, ex.StackTrace, ex.GetType)
            End Try
        End Sub

        Public Function doesTaskExist(ByVal nameOfTask As String, ByRef taskObject As TaskScheduler.Task) As Boolean
            Try
                Using taskServiceObject As TaskScheduler.TaskService = New TaskScheduler.TaskService()
                    taskObject = taskServiceObject.GetTask(nameOfTask)

                    If taskObject Is Nothing Then
                        Return False
                    Else
                        Return True
                    End If
                End Using
            Catch ex As Exception
                Return False
            End Try
        End Function

        ''' <summary>Enables the built-in System Restore Point task that appears to be the culprit in the disappearance of restore points on Windows 8.x and Windows 10.</summary>
        Public Sub enableBuiltInRestorePointTask()
            Try
                Dim taskService As New TaskScheduler.TaskService()
                Dim taskObject As TaskScheduler.Task = taskService.GetTask("Microsoft\Windows\SystemRestore\SR")

                If taskObject IsNot Nothing Then
                    If taskObject.Enabled = False Then
                        taskObject.Definition.Settings.Enabled = True
                        taskObject.RegisterChanges()
                        eventLogFunctions.writeToSystemEventLog("Enabled built-in Windows System Restore Task.", EventLogEntryType.Information)
                    End If

                    taskObject.Dispose()
                End If

                taskService.Dispose()
            Catch ex As Exception
                eventLogFunctions.writeToSystemEventLog("Unable to disable built-in Windows System Restore Task.", EventLogEntryType.Error)
                eventLogFunctions.writeCrashToEventLog(ex)
            End Try
        End Sub

        ''' <summary>Disables the built-in System Restore Point task that appears to be the culprit in the disappearance of restore points on Windows 8.x and Windows 10.</summary>
        Public Sub disableBuiltInRestorePointTask()
            Try
                Dim taskService As New TaskScheduler.TaskService()
                Dim taskObject As TaskScheduler.Task = taskService.GetTask("Microsoft\Windows\SystemRestore\SR")

                If taskObject IsNot Nothing Then
                    If taskObject.Enabled = True Then
                        taskObject.Definition.Settings.Enabled = False
                        taskObject.RegisterChanges()
                        eventLogFunctions.writeToSystemEventLog("Disabled built-in Windows System Restore Task.", EventLogEntryType.Information)
                    End If

                    taskObject.Dispose()
                End If

                taskService.Dispose()
            Catch ex As Exception
                eventLogFunctions.writeToSystemEventLog("Unable to disable built-in Windows System Restore Task.", EventLogEntryType.Error)
                eventLogFunctions.writeCrashToEventLog(ex)
            End Try
        End Sub

        ''' <summary>Adds a scheduled task to launch this program with elevated user rights without a UAC prompt.</summary>
        ''' <param name="taskName">The name of the task.</param>
        ''' <param name="taskDescription">The description of the task.</param>
        ''' <param name="taskEXEPath">The path the executable that this task will invoke.</param>
        ''' <param name="taskParameters">Any and all command line arguments that the invoked executable will be launched with.</param>
        Public Sub addRunTimeTask(taskName As String, taskDescription As String, taskEXEPath As String, taskParameters As String, Optional boolAllowParallelRunMode As Boolean = False)
            ' Apparently the SYSTEM user can't create scheduled tasks, so to prevent crashes we check to
            ' see if we are running in the SYSTEM user context and if we are we abort this subroutine.
            If privilegeChecks.areWeRunningAsSystemUser() = True Then
                Exit Sub
            End If

            Try
                taskName = taskName.Trim & " (For User " & Environment.UserName & ")"
                taskDescription = taskDescription.Trim
                taskEXEPath = taskEXEPath.Trim
                taskParameters = taskParameters.Trim

                eventLogFunctions.writeToSystemEventLog("Creating task """ & taskName & """", EventLogEntryType.Information)

                If IO.File.Exists(taskEXEPath) = False Then
                    MsgBox("Executable path not found.", MsgBoxStyle.Critical, "Restore Point Creator")
                    Exit Sub
                End If

                Dim taskService As TaskScheduler.TaskService = New TaskScheduler.TaskService()
                Dim newTask As TaskScheduler.TaskDefinition = taskService.NewTask

                newTask.RegistrationInfo.Description = taskDescription

                Dim exeFileInfo As New IO.FileInfo(taskEXEPath)

                newTask.Actions.Add(New TaskScheduler.ExecAction(taskEXEPath, taskParameters, exeFileInfo.DirectoryName))

                newTask.Principal.RunLevel = TaskScheduler.TaskRunLevel.Highest

                With newTask.Settings
                    .Compatibility = TaskScheduler.TaskCompatibility.V2
                    .AllowDemandStart = True
                    .DisallowStartIfOnBatteries = False
                    .RunOnlyIfIdle = False
                    .StopIfGoingOnBatteries = False
                    .AllowHardTerminate = False
                    .ExecutionTimeLimit = Nothing
                End With

                If boolAllowParallelRunMode = True Then
                    newTask.Settings.MultipleInstances = TaskScheduler.TaskInstancesPolicy.Parallel
                End If

                newTask.Principal.LogonType = TaskScheduler.TaskLogonType.InteractiveToken

                Try
                    newTask.Validate(True)
                Catch ex As Exception
                    newTask.Dispose()
                    taskService.Dispose()

                    eventLogFunctions.writeToSystemEventLog("There was an error while validating the task definition settings.", EventLogEntryType.Error)
                    eventLogFunctions.writeCrashToEventLog(ex)

                    Exit Sub
                End Try

                Dim taskFolderObject As TaskScheduler.TaskFolder = getOurTaskFolder(taskService)
                taskFolderObject.RegisterTaskDefinition(taskName, newTask)

                taskFolderObject.Dispose()
                newTask.Dispose()
                taskService.Dispose()

                taskFolderObject = Nothing
                newTask = Nothing
                taskService = Nothing
            Catch ex As Exception
                eventLogFunctions.writeCrashToEventLog(ex)
            End Try
        End Sub

        ''' <summary>Creates the run time tasks to launch this program with elevated user rights without a UAC prompt.</summary>
        Public Sub createRunTimeTasksSubRoutine()
            ' Apparently the SYSTEM user can't create scheduled tasks, so to prevent crashes we check to
            ' see if we are running in the SYSTEM user context and if we are we abort this subroutine.
            If privilegeChecks.areWeRunningAsSystemUser() = True Then
                Exit Sub
            End If

            Try
                Dim task As TaskScheduler.Task = Nothing

                If doesRunTimeTaskExist("Restore Point Creator -- Run with no UAC (Create Restore Point)", task) = False Then
                    addRunTimeTask("Restore Point Creator -- Run with no UAC (Create Restore Point)", "Runs Restore Point Creator with no UAC prompt.", Application.ExecutablePath, globalVariables.commandLineSwitches.createRestorePoint)
                End If
                If doesRunTimeTaskExist("Restore Point Creator -- Run with no UAC (Create Custom Restore Point)", task) = False Then
                    addRunTimeTask("Restore Point Creator -- Run with no UAC (Create Custom Restore Point)", "Runs Restore Point Creator with no UAC prompt.", Application.ExecutablePath, globalVariables.commandLineSwitches.createCustomRestorePoint)
                End If
                If doesRunTimeTaskExist("Restore Point Creator -- Run with no UAC", task) = False Then
                    addRunTimeTask("Restore Point Creator -- Run with no UAC", "Runs Restore Point Creator with no UAC prompt.", Application.ExecutablePath, "", True)
                End If
                If doesRunTimeTaskExist("Restore Point Creator -- Run with no UAC (Delete old Restore Points)", task) = False Then
                    addRunTimeTask("Restore Point Creator -- Run with no UAC (Delete old Restore Points)", "Runs Restore Point Creator with no UAC prompt.", Application.ExecutablePath, globalVariables.commandLineSwitches.deleteOldRestorePoints)
                End If
            Catch ex As Exception
                eventLogFunctions.writeCrashToEventLog(ex)
                ' Silently handle the exception.
            End Try
        End Sub

        Public Sub updateScheduledRestorePointCreationTaskWithEverySetting()
            Try
                ' First we get the Every setting from the Registry. If the value in the Registry is missing we populate the value for it in the program with "nothing".
                Dim strEvery As String = Registry.LocalMachine.OpenSubKey(globalVariables.registryValues.strKey, False).GetValue("Every", Nothing)

                ' Checks to see if the value in the Registry existed, if it exists then it shouldn't equal "nothing".
                If strEvery <> Nothing Then
                    ' Good, the Every setting existed in the Registry. So let's continue the work here.

                    Dim shortEvery As Short ' Creates a Short variable with a name of everyShort.

                    ' Now we try and parse the setting from the Registry from a String to a Short.
                    If Short.TryParse(strEvery, shortEvery) = True Then
                        ' Good, the Short parsing worked.

                        Dim taskService As New TaskScheduler.TaskService ' Creates a new TaskService Object.
                        Dim taskWeAreWorkingWith As TaskScheduler.Task = taskService.GetTask("\System Restore Checkpoint by System Restore Point Creator") ' Gets our Task.

                        ' First we need to make sure if the Task even exists, we do this by doing an IsNot Nothing check to make sure we don't have a Null Reference Exception.
                        If taskWeAreWorkingWith IsNot Nothing Then
                            If taskWeAreWorkingWith.Definition.Triggers.Count <> 0 Then ' Now we make sure our task has triggers to work with.
                                ' Checks to see if the Trigger is a daily Trigger.
                                If taskWeAreWorkingWith.Definition.Triggers(0).TriggerType = TaskScheduler.TaskTriggerType.Daily Then
                                    Dim taskTime As Date = taskWeAreWorkingWith.Definition.Triggers(0).StartBoundary ' Saves the task start time for later use.
                                    taskWeAreWorkingWith.Definition.Triggers.Remove(taskWeAreWorkingWith.Definition.Triggers(0)) ' Removes the Trigger.
                                    taskWeAreWorkingWith.Definition.Triggers.Add(New TaskScheduler.DailyTrigger(shortEvery) With {.StartBoundary = taskTime}) ' Creates a new Trigger but with a Every setting.
                                    taskWeAreWorkingWith.RegisterChanges() ' Saves the changes to the task.

                                    Registry.LocalMachine.OpenSubKey(globalVariables.registryValues.strKey, True).DeleteValue("Every", False) ' Deletes the Every setting from the Registry.

                                    ' Add an Event Log Entry stating the scheduled task was upgraded successfully.
                                    eventLogFunctions.writeToSystemEventLog("The upgrade of the scheduled task was successful.", EventLogEntryType.Information)
                                End If
                            End If

                            taskWeAreWorkingWith.Dispose() ' Disposes of our Task Object.
                            taskWeAreWorkingWith = Nothing
                        End If

                        taskService.Dispose() ' Disposes of our TaskService Object.
                        taskService = Nothing
                    End If
                End If
            Catch ex As Exception
                ' OK, we have some errors so lets add them to the Event Log.
                eventLogFunctions.writeToSystemEventLog("An error occurred while upgrading scheduled task.", EventLogEntryType.Error)
                eventLogFunctions.writeCrashToEventLog(ex)
            End Try
        End Sub

        ''' <summary>Deletes a scheduled task.</summary>
        ''' <param name="task">The Task Object representing the task that needs to be deleted.</param>
        Public Sub deleteTask(ByRef task As TaskScheduler.Task)
            Try
                task.Folder.DeleteTask(task.Name)
            Catch ex As Exception
            End Try
        End Sub

        ''' <summary>Deletes a scheduled task.</summary>
        ''' <param name="taskName">The task name to be deleted.</param>
        ''' <param name="taskFolder">The task folder in which the task can be found. The default input is "root".</param>
        Private Sub deleteTask(taskName As String, Optional taskFolder As String = globalVariables.constStringRoot)
            Try
                Using taskServiceObject As TaskScheduler.TaskService = New TaskScheduler.TaskService()
                    If taskFolder = globalVariables.constStringRoot Then
                        taskServiceObject.RootFolder.DeleteTask(taskName, False)
                    Else
                        taskServiceObject.RootFolder.SubFolders(taskFolder).DeleteTask(taskName, False)
                    End If
                End Using
            Catch ex As Exception
            End Try
        End Sub

        Public Sub setMultiRunForTask()
            Dim task As TaskScheduler.Task = Nothing

            If doesRunTimeTaskExist("Restore Point Creator -- Run with no UAC", task) = True Then
                If task.Definition.Settings.MultipleInstances <> TaskScheduler.TaskInstancesPolicy.Parallel Then
                    deleteTask(task)
                    task.Dispose()

                    eventLogFunctions.writeToSystemEventLog("Re-Created RunTime Task with MultiRun mode enabled.", EventLogEntryType.Information)
                    addRunTimeTask("Restore Point Creator -- Run with no UAC", "Runs Restore Point Creator with no UAC prompt.", Application.ExecutablePath, "", True)
                End If
            End If
        End Sub

        ''' <summary>Checks to see if a run time task to launch this program with elevated user rights without a UAC prompt exists.</summary>
        ''' <param name="nameOfTask">The name of the task to check if it exists.</param>
        ''' <param name="taskObject">This is a ByRef argument that passes back the Task Object to be used by the calling function.</param>
        ''' <returns>Returns a Boolean value. If the task exists it returns TRUE that you can then use the ByRef Task Object to start.</returns>
        Public Function doesRunTimeTaskExist(ByVal nameOfTask As String, ByRef taskObject As TaskScheduler.Task) As Boolean
            nameOfTask = globalVariables.taskFolder & "\" & nameOfTask & " (For User " & Environment.UserName & ")"

            Dim execActionPath As String
            Dim taskServiceObject As New TaskScheduler.TaskService()

            Try
                taskObject = taskServiceObject.GetTask(nameOfTask)

                If taskObject Is Nothing Then
                    taskServiceObject.Dispose()
                    Return False
                Else
                    If taskObject.Definition.Actions.Count = 1 Then
                        execActionPath = DirectCast(taskObject.Definition.Actions(0), TaskScheduler.ExecAction).Path

                        If execActionPath.Contains(Chr(34)) = True Then execActionPath = execActionPath.Replace(Chr(34), "")

                        If IO.File.Exists(execActionPath) = True Then
                            Return True
                        Else
                            deleteTask(taskObject) ' This is an invalid RunTime task so we need to delete it.

                            taskServiceObject.Dispose()
                            Return False
                        End If
                    ElseIf taskObject.Definition.Actions.Count = 0 Then
                        deleteTask(taskObject) ' This is an invalid RunTime task so we need to delete it.

                        taskServiceObject.Dispose()
                        Return False
                    Else
                        deleteTask(taskObject) ' This is an invalid RunTime task so we need to delete it.

                        taskServiceObject.Dispose()
                        Return False
                    End If
                End If
            Catch ex As Exception
                Return False
            End Try
        End Function

        Private Sub runTheTask(ByRef task As TaskScheduler.Task)
            ' This checks to see if the task we are trying to run is allowed to be run on demand.
            If task.Definition.Settings.AllowDemandStart = True Then
                ' Yes, it's allowed so let's continue.
                Try
                    task.Run() ' Runs the task.
                    Process.GetCurrentProcess.Kill() ' We kill the current un-elevated process.
                Catch ex As Exception
                    ' OK, something went wrong so now let's just abandon the idea of running the task and just re-run the program with a UAC prompt instead.
                    support.reRunWithAdminUserRights() ' OK, we relaunch the process with full Administrator privileges with a UAC prompt.
                End Try
            Else
                ' OK, the task somehow got the demand start flag disabled; we need to repair this.
                support.launchRuntimeTaskFixRoutine() ' This launches the program with a command line argument to tell the program to fix the runtime tasks.
                Process.GetCurrentProcess.Kill() ' We now kill the current un-elevated process.
            End If
        End Sub

        ''' <summary>Runs this program with elevated user rights without a UAC prompt by running the appropriate scheduled task.</summary>
        ''' This code is highly commented so as to make it so that in the future I know what the
        ''' hell I'm doing in it. This is very complicated code with a lot of logic in it.
        Public Sub runProgramUsingTaskWrapper()
            ' We're going to store the result of the areWeAnAdministrator() call in this Boolean variable for later use in this code block.
            Dim boolAreWeRunningAsAdministrator As Boolean = privilegeChecks.areWeAnAdministrator()

            ' To make sure that nothing goes wrong... we wrap this code in a TRY block.
            Try
                If Debugger.IsAttached = False Then ' Checks to see if a debugger is attached to the current process.
                    Dim boolNoTask As Boolean ' Create a Boolean data type variable.

                    ' We open our program's Registry key.
                    Dim registryKey As RegistryKey = Registry.LocalMachine.OpenSubKey(globalVariables.registryValues.strKey, False)

                    ' We then check to see if our Registry key exists to make sure we don't get any Null Reference Exceptions.
                    If registryKey IsNot Nothing Then
                        ' This not only reads the entry from the Registry but also tries to parse it as a Boolean value. If the
                        ' value from the Registry cannot be parsed, we give the boolNoTask variable a default value of False.
                        ' If the value is False, we use the Task Wrapper to launch the program. If the value is True, we prompt
                        ' the user for a UAC prompt instead.
                        If Boolean.TryParse(registryKey.GetValue("No Task", "False"), boolNoTask) = False Then
                            ' We weren't able to parse the value from the Registry so let's
                            ' give the boolNoTask variable a default value of False.
                            boolNoTask = False
                        End If

                        ' And let's close the Registry key.
                        registryKey.Close()
                        registryKey.Dispose()
                        registryKey = Nothing

                        ' Checks to see if the Registry value was True and if we aren't an Admin. If boolNoTask
                        ' is True, then we aren't going to be using the Task Wrapper to launch the program.
                        If boolNoTask = True And boolAreWeRunningAsAdministrator = False Then
                            ' OK, we relaunch the process with full Administrator privileges with a UAC prompt.
                            support.reRunWithAdminUserRights()
                        End If
                    End If

                    ' Checks to see if this application's executable is in a safe place, in this case... Program Files.
                    If Application.ExecutablePath.caseInsensitiveContains("program files") = True Then
                        ' Yes, it is... so let's continue.

                        ' OK, we need to make sure that we are currently running with Administrator privileges.
                        If boolAreWeRunningAsAdministrator = True Then
                            ' Yes, we are.

                            ' Let's check to see if the tasks exist or not.  If they don't, let's create them.  We have a total of three tasks we need to create.
                            createRunTimeTasksSubRoutine()
                            ' Done creating the tasks.  So from this point on we keep on running as normal.
                        ElseIf boolAreWeRunningAsAdministrator = False Then
                            ' No we are not.  So we are either going to relaunch the program with Administrator
                            ' user rights using the Task Wrapper or prompt the user with a UAC prompt.

                            Dim task As TaskScheduler.Task = Nothing ' Creates a task object.

                            ' Checks to see if the user is running one of the application shortcuts for this program.
                            If My.Application.CommandLineArgs.Count >= 1 Then
                                ' Yes, one of the application shortcuts for this program was used.  Let's find out which one, shall we.

                                ' Stores the contents of the command line argument for later use in this code.
                                Dim commandLineArgument As String = My.Application.CommandLineArgs(0).Trim

                                ' Handles the creation of a restore point from the command line or shortcut.
                                If commandLineArgument.stringCompare(globalVariables.commandLineSwitches.createRestorePoint) Then
                                    ' Checks to see if the Task Wrapper task exists and returns both a Boolean value and a task object.
                                    If doesRunTimeTaskExist("Restore Point Creator -- Run with no UAC (Create Restore Point)", task) = True Then
                                        ' Yes, the Task Wrapper task exists so we are going to run that task.
                                        runTheTask(task)
                                    Else
                                        ' OK, we relaunch the process with full Administrator privileges with a UAC prompt.
                                        support.reRunWithAdminUserRights()
                                    End If
                                ElseIf commandLineArgument.stringCompare(globalVariables.commandLineSwitches.createCustomRestorePoint) Then
                                    ' Checks to see if the Task Wrapper task exists and returns both a Boolean value and a task object.
                                    If doesRunTimeTaskExist("Restore Point Creator -- Run with no UAC (Create Custom Restore Point)", task) = True Then
                                        ' Yes, the Task Wrapper task exists so we are going to run that task.
                                        runTheTask(task)
                                    Else
                                        ' OK, we relaunch the process with full Administrator privileges with a UAC prompt.
                                        support.reRunWithAdminUserRights()
                                    End If
                                ElseIf commandLineArgument.stringCompare(globalVariables.commandLineSwitches.deleteOldRestorePoints) Then
                                    ' Checks to see if the Task Wrapper task exists and returns both a Boolean value and a task object.
                                    If doesRunTimeTaskExist("Restore Point Creator -- Run with no UAC (Delete old Restore Points)", task) = True Then
                                        ' Yes, the Task Wrapper task exists so we are going to run that task.
                                        runTheTask(task)
                                    Else
                                        ' OK, we relaunch the process with full Administrator privileges with a UAC prompt.
                                        support.reRunWithAdminUserRights()
                                    End If
                                ElseIf commandLineArgument.stringCompare(globalVariables.commandLineSwitches.forceUAC) And boolAreWeRunningAsAdministrator = False Then
                                    ' OK, we relaunch the process with full Administrator privileges with a UAC prompt.
                                    support.reRunWithAdminUserRights()
                                End If
                            Else
                                ' Nope, the user is running it normally without a command line argument.

                                ' Checks to see if the Task Wrapper task exists and returns both a Boolean value and a task object.
                                If doesRunTimeTaskExist("Restore Point Creator -- Run with no UAC", task) = True Then
                                    ' Yes, the Task Wrapper task exists so we are going to run that task.
                                    runTheTask(task)
                                Else
                                    ' OK, we relaunch the process with full Administrator privileges with a UAC prompt.
                                    support.reRunWithAdminUserRights()
                                End If
                            End If
                        End If
                    Else
                        ' No, so let's use the UAC prompt because we can't tell if the user is going to move
                        ' this executable or not. So to be on the safe side, we don't use the Task Wrapper.
                        If boolAreWeRunningAsAdministrator = False Then support.reRunWithAdminUserRights()
                    End If
                End If
            Catch ex As Exception
                eventLogFunctions.writeCrashToEventLog(ex)
                ' Something went very wrong, so let's just try and re-launch this program with standard Administrator privileges with a UAC prompt.
                If boolAreWeRunningAsAdministrator = False Then support.reRunWithAdminUserRights()
            End Try
        End Sub
    End Module
End Namespace