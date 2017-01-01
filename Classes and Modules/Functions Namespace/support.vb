Imports System.Text.RegularExpressions
Imports ICSharpCode.SharpZipLib.Zip

Namespace Functions.support
    Module support
        Public Sub addExtendedCrashData(ByRef stringBuilder As System.Text.StringBuilder, rawExceptionObject As Exception)
            Dim jsonTemp As String

            Try
                If rawExceptionObject.GetType.Equals(GetType(IO.FileNotFoundException)) Then
                    stringBuilder.AppendLine()
                    stringBuilder.AppendLine("Additional IO.FileNotFoundException Data")

                    Dim FileNotFoundExceptionObject As IO.FileNotFoundException = DirectCast(rawExceptionObject, IO.FileNotFoundException)
                    stringBuilder.AppendLine("Name of File: " & FileNotFoundExceptionObject.FileName)

                    If Not String.IsNullOrEmpty(FileNotFoundExceptionObject.FusionLog) Then
                        stringBuilder.AppendLine("Reason: " & FileNotFoundExceptionObject.FusionLog)
                    End If

                    Try
                        jsonTemp = jsonObject(FileNotFoundExceptionObject.Data)

                        If Not jsonTemp.Equals("{}") Then
                            stringBuilder.AppendLine("Additional FileLoadException Data: " & jsonTemp)
                        End If
                    Catch ex As Exception
                    End Try

                    stringBuilder.AppendLine()
                ElseIf rawExceptionObject.GetType.Equals(GetType(IO.FileLoadException)) Then
                    stringBuilder.AppendLine()
                    stringBuilder.AppendLine("Additional IO.FileLoadException Data")

                    Dim FileLoadExceptionObject As IO.FileLoadException = DirectCast(rawExceptionObject, IO.FileLoadException)
                    stringBuilder.AppendLine("Unable to Load Assembly File: " & FileLoadExceptionObject.FileName)

                    If Not String.IsNullOrEmpty(FileLoadExceptionObject.FusionLog) Then
                        stringBuilder.AppendLine("Reason why assembly couldn't be loaded: " & FileLoadExceptionObject.FusionLog)
                    End If

                    Try
                        jsonTemp = jsonObject(FileLoadExceptionObject.Data)

                        If Not jsonTemp.Equals("{}") Then
                            stringBuilder.AppendLine("Additional FileLoadException Data: " & jsonTemp)
                        End If
                    Catch ex As Exception
                    End Try

                    stringBuilder.AppendLine()
                ElseIf rawExceptionObject.GetType.Equals(GetType(Runtime.InteropServices.COMException)) Then
                    stringBuilder.AppendLine()
                    stringBuilder.AppendLine("Additional Runtime.InteropServices.COMException Data")

                    Dim COMExceptionObject As Runtime.InteropServices.COMException = DirectCast(rawExceptionObject, Runtime.InteropServices.COMException)
                    stringBuilder.AppendLine("Source: " & COMExceptionObject.Source)
                    stringBuilder.AppendLine("Error Code: " & COMExceptionObject.ErrorCode)

                    Try
                        jsonTemp = jsonObject(COMExceptionObject.Data)

                        If Not jsonTemp.Equals("{}") Then
                            stringBuilder.AppendLine("Additional FileLoadException Data: " & jsonTemp)
                        End If
                    Catch ex As Exception
                    End Try

                    stringBuilder.AppendLine()
                End If
            Catch ex As Exception
            End Try
        End Sub

        Public Function jsonObject(input As Object) As String
            Try
                Dim jsonEngine As New Web.Script.Serialization.JavaScriptSerializer
                Return jsonEngine.Serialize(input)
            Catch ex As Exception
                Return "{}"
            End Try
        End Function

        Public Function copyTextToWindowsClipboard(strTextToBeCopiedToClipboard As String) As Boolean
            Try
                Clipboard.SetDataObject(strTextToBeCopiedToClipboard, True, 5, 200)
                Return True
            Catch ex As Exception
                MsgBox("Unable to open Windows Clipboard to copy text to it.", MsgBoxStyle.Critical, "Restore Point Creator")
                Return False
            End Try
        End Function

        ''' <summary>Gets a setting from the application's Registry key.</summary>
        ''' <param name="registryObject">This is Registry Key Object that will be used in this function to pull the Registry value from.</param>
        ''' <param name="valueToGetFromRegistry">The name of the Registry Value we will be pulling from.</param>
        ''' <param name="boolDefaultValue">If the Registry Value isn't found or the value is malformed, this will be the Boolean value that this function will return.</param>
        ''' <returns>A Boolean value.</returns>
        Public Function getBooleanValueFromRegistry(ByRef registryObject As Microsoft.Win32.RegistryKey, ByVal valueToGetFromRegistry As String, ByVal boolDefaultValue As Boolean) As Boolean
            Try
                Dim boolTemp As Boolean, strDefaultValue As String

                If boolDefaultValue = True Then
                    strDefaultValue = globalVariables.booleans.strTrue
                Else
                    strDefaultValue = globalVariables.booleans.strFalse
                End If

                If Boolean.TryParse(registryObject.GetValue(valueToGetFromRegistry, strDefaultValue), boolTemp) = False Then
                    boolTemp = boolDefaultValue
                End If

                Return boolTemp
            Catch ex As Exception
                Return boolDefaultValue
            End Try
        End Function

        ''' <summary>Checks to see if a Process ID or PID exists on the system.</summary>
        ''' <param name="PID">The PID of the process you are checking the existance of.</param>
        ''' <param name="processObject">If the PID does exist, the function writes back to this argument in a ByRef way a Process Object that can be interacted with outside of this function.</param>
        ''' <returns>Return a Boolean value. If the PID exists, it return a True value. If the PID doesn't exist, it returns a False value.</returns>
        Public Function doesProcessIDExist(ByVal PID As Integer, ByRef processObject As Process) As Boolean
            Try
                processObject = Process.GetProcessById(PID)
                Return True
            Catch ex As Exception
                Return False
            End Try
        End Function

        Function getProcesses() As Dictionary(Of Integer, String)
            Dim processesOnSystem As New Dictionary(Of Integer, String)
            Dim exePath As String

            For Each process As Process In Process.GetProcesses()
                exePath = getProcessExecutablePath(process.Id)
                If exePath IsNot Nothing Then processesOnSystem.Add(process.Id, exePath)
            Next

            Return processesOnSystem
        End Function

        Function getProcessExecutablePath(processID As Integer) As String
            Dim memoryBuffer = New Text.StringBuilder(1024)
            Dim processHandle As IntPtr = APIs.OpenProcess(APIs.ProcessAccessFlags.PROCESS_QUERY_LIMITED_INFORMATION, False, processID)

            If processHandle <> IntPtr.Zero Then
                Try
                    Dim memoryBufferSize As Integer = memoryBuffer.Capacity

                    If APIs.QueryFullProcessImageName(processHandle, 0, memoryBuffer, memoryBufferSize) Then
                        Return memoryBuffer.ToString()
                    End If
                Finally
                    APIs.CloseHandle(processHandle)
                End Try
            End If

            Return Nothing
        End Function

        Public Sub searchForProcessAndKillIt(strFileName As String, boolFullFilePathPassed As Boolean)
            Dim processExecutablePath As String
            Dim processExecutablePathFileInfo As IO.FileInfo

            For Each process As Process In Process.GetProcesses()
                processExecutablePath = getProcessExecutablePath(process.Id)

                If processExecutablePath IsNot Nothing Then
                    Try
                        processExecutablePathFileInfo = New IO.FileInfo(processExecutablePath)

                        If boolFullFilePathPassed = True Then
                            If stringCompare(strFileName, processExecutablePathFileInfo.FullName) = True Then
                                killProcess(process.Id, True)
                            End If
                        ElseIf boolFullFilePathPassed = False Then
                            If stringCompare(strFileName, processExecutablePathFileInfo.Name) = True Then
                                killProcess(process.Id, True)
                            End If
                        End If

                        processExecutablePathFileInfo = Nothing
                    Catch ex As ArgumentException
                    End Try
                End If

                processExecutablePath = Nothing
            Next
        End Sub

        ''' <summary>Extracts a file from a ZIP file.</summary>
        ''' <param name="zipFileObject">This is a ICSharpCode.SharpZipLib.Zip.ZipFile Object referencing the ZIP file that you want to extract data from. To use this create a new ZipFile Object using Dim zipFileObject As New ZipFile("C:\Path\To\Your\Zip\File.zip") or by creating a new ZipFile Object by passing it an IO.MemoryStream() and then pass that Object to this function.</param>
        ''' <param name="fileToExtract">The name of the file you want to extract from the ZIP file.</param>
        ''' <param name="extractionTarget">The path to which you want to extract the file to.</param>
        ''' <returns>A Boolean Value. If the function was able to sucessfully extract the requested file from the ZIP file, it will return a True value. However, if for whatever reason something goes wrong anywhere in this function, it will return a False value.</returns>
        Public Function extractUpdatedFileFromZIPPackage(ByRef zipFileObject As ZipFile, ByVal fileToExtract As String, ByVal extractionTarget As String, Optional boolDeleteTargetIfExists As Boolean = True) As Boolean
            Try
                Dim extractionTargetFileInfo As New IO.FileInfo(extractionTarget)

                If globalVariables.boolExtendedLoggingDuringUpdating = True Then
                    eventLogFunctions.writeToSystemEventLog(String.Format("Beginning extraction of {0}{1}{0} to {0}{2}{0}.", Chr(34), fileToExtract, extractionTargetFileInfo.Name), EventLogEntryType.Information)
                End If

                ' This gets the ZipEntry Object for the file we are trying to extract from the ZIP file.
                Dim zipFileEntryObject As ZipEntry = zipFileObject.GetEntry(fileToExtract)

                ' This checks to see if the file that we're trying to extract from the ZIP file exists in the ZIP file.
                If zipFileEntryObject Is Nothing Then
                    eventLogFunctions.writeToSystemEventLog(String.Format("Unable to find {0}{1}{0} to extract from ZIP file.", Chr(34), fileToExtract), EventLogEntryType.Error)
                    Return False ' Nope, the file doesn't exist in the ZIP file so we exit out of the routine by returning a False value for the function.
                Else
                    ' This checks to see if the file we are trying to extract to from the ZIP file exists or not.
                    If IO.File.Exists(extractionTarget) = True Then
                        If globalVariables.boolExtendedLoggingDuringUpdating = True Then
                            eventLogFunctions.writeToSystemEventLog(String.Format("The file named {0}{1}{0} already exists, attempting to delete it.", Chr(34), fileToExtract), EventLogEntryType.Information)
                        End If
                        ' OK, so it does exist, let's do something about it.

                        ' This checks to see if the programmer wants to delete the file if it exists in this function.
                        If boolDeleteTargetIfExists = True Then
                            If stringCompare(extractionTargetFileInfo.Extension, ".exe") Then
                                ' If the file is an EXE file, let's try and kill any parent processes first.
                                If globalVariables.boolExtendedLoggingDuringUpdating = True Then
                                    eventLogFunctions.writeToSystemEventLog(String.Format("Since this file is an EXE file we need to check for any processes that have this file as the parent executable file.{2}{2}Killing any possible processes that have a parent executable of {0}{1}{0}.", Chr(34), extractionTargetFileInfo.FullName, vbCrLf), EventLogEntryType.Information)
                                End If

                                searchForProcessAndKillIt(extractionTargetFileInfo.FullName, True)
                            End If

                            Try
                                IO.File.Delete(extractionTarget) ' And now let's try and delete the file.

                                If globalVariables.boolExtendedLoggingDuringUpdating = True Then
                                    eventLogFunctions.writeToSystemEventLog(String.Format("The file named {0}{1}{0} has been successfully deleted.", Chr(34), extractionTargetFileInfo.FullName), EventLogEntryType.Information)
                                End If
                            Catch ex As Exception
                                eventLogFunctions.writeToSystemEventLog(String.Format("Unable to delete {0}{1}{0}.", Chr(34), extractionTargetFileInfo.Name), EventLogEntryType.Error)
                                ' Something went wrong while trying to delete the file so we return a False value for the function.
                                Return False
                            End Try
                        Else
                            ' The programmer doesn't want the file deleted if it existed so we return a False value for the function.
                            Return False
                        End If
                    Else
                        If globalVariables.boolExtendedLoggingDuringUpdating = True Then
                            eventLogFunctions.writeToSystemEventLog(String.Format("The file named {0}{1}{0} does not exist, this is a good thing; we can continue with the update process.", Chr(34), extractionTargetFileInfo.Name), EventLogEntryType.Information)
                        End If
                    End If

                    If globalVariables.boolExtendedLoggingDuringUpdating = True Then
                        eventLogFunctions.writeToSystemEventLog(String.Format("Creating new IO.FileStream to write {0}{1}{0} as {0}{2}{0} to disk.", Chr(34), fileToExtract, extractionTargetFileInfo.Name), EventLogEntryType.Information)
                    End If

                    ' Create a new FileStream Object to write out our extracted file.
                    Dim fileStream As New IO.FileStream(extractionTarget, IO.FileMode.Create)

                    If globalVariables.boolExtendedLoggingDuringUpdating = True Then
                        eventLogFunctions.writeToSystemEventLog(String.Format("IO.FileStream created successfully. Commencing the process of writing file {0}{1}{0} as {0}{2}{0} to disk.", Chr(34), fileToExtract, extractionTargetFileInfo.Name), EventLogEntryType.Information)
                    End If

                    ' This copies the data out of the ZIP File Data Stream to our FileStream Object that was created above.
                    zipFileObject.GetInputStream(zipFileEntryObject).CopyTo(fileStream)

                    If globalVariables.boolExtendedLoggingDuringUpdating = True Then
                        eventLogFunctions.writeToSystemEventLog("File write operation complete. Closing out file and disposing of the IO.FileStream.", EventLogEntryType.Information)
                    End If

                    fileStream.Close() ' This closes our FileStream Object.
                    fileStream.Dispose() ' And this Disposes of our FileStream Object.

                    If globalVariables.boolExtendedLoggingDuringUpdating = True Then
                        eventLogFunctions.writeToSystemEventLog(String.Format("Extraction of {0}{1}{0} was successful.", Chr(34), fileToExtract), EventLogEntryType.Information)
                    End If

                    extractionTargetFileInfo = Nothing

                    If globalVariables.boolExtendedLoggingDuringUpdating = True Then
                        eventLogFunctions.writeToSystemEventLog("Returning a True value for the extractUpdatedFileFromZIPPackage() function.", EventLogEntryType.Information)
                    End If

                    Return True ' Yes, the file did exist in the ZIP file and we were able to successfully extract it so we return a True value for the function.
                End If
            Catch ex As Exception
                ' Something went wrong so we write out our crash stack trace data to the application event log.
                eventLogFunctions.writeCrashToEventLog(ex)
                eventLogFunctions.writeToSystemEventLog("Returning a False value for the extractUpdatedFileFromZIPPackage() function.", EventLogEntryType.Error)
                Return False ' And we return a False value for the function.
            End Try
        End Function

        ''' <summary>Delete a file with no Exception. It first checks to see if the file exists and if it does it attempts to delete it. If it fails to do so then it will fail silently without an Exception.</summary>
        Public Sub deleteFileWithNoException(ByVal pathToFile As String)
            Try
                If IO.File.Exists(pathToFile) Then IO.File.Delete(pathToFile)
            Catch ex As Exception
            End Try
        End Sub

        ''' <summary>ZIPs the exported log file.</summary>
        ''' <param name="pathToZIPFile">The path the ZIP file we will be working with.</param>
        ''' <param name="fileToAddToZIPFile">The path to the file we will be adding to the ZIP file.</param>
        ''' <returns>Returns a Boolean value.</returns>
        Public Function addFileToZipFile(pathToZIPFile As String, fileToAddToZIPFile As String) As Boolean
            Try
                Dim zipFileObject As ZipFile ' Creates a ZIPFile Object.

                ' This checks to see if the ZIP file we want to work with already exists.
                If IO.File.Exists(pathToZIPFile) = True Then
                    ' OK, the ZIP file already exists so we create a new ZIPFile Object by opening the existing ZIP file.
                    zipFileObject = New ZipFile(pathToZIPFile)
                Else
                    ' No, the ZIP file doesn't exist so we tells the ZIPFile Library to create a new ZIPFile Object with no previous file.
                    zipFileObject = ZipFile.Create(pathToZIPFile)
                End If

                zipFileObject.BeginUpdate() ' We need to open the ZIP file for writing.
                zipFileObject.Add(fileToAddToZIPFile, New IO.FileInfo(fileToAddToZIPFile).Name) ' Adds the file to the ZIP file.
                zipFileObject.CommitUpdate() ' Commits the added file to the ZIP file.
                zipFileObject.Close() ' Closes the ZIPFile Object.

                Return True
            Catch ex As Exception
                Return False
            End Try
        End Function

        <Runtime.InteropServices.DllImport("Srclient.dll")> Public Function SRRemoveRestorePoint(index As Integer) As Integer
        End Function

        ''' <summary>Converts the number of Bytes to a nice way of saying it, like MBs, GBs, etc.</summary>
        ''' <param name="size">The amount of data in Bytes.</param>
        ''' <param name="roundToNearestWholeNumber">Tells the function if it should round the number to the nearest whole number.</param>
        ''' <returns>A String value such as 100 MBs or 100 GBs.</returns>
        Public Function bytesToHumanSize(ByVal size As ULong, Optional roundToNearestWholeNumber As Boolean = False) As String
            Dim result As String

            If size <= (2 ^ 10) Then
                result = size & " Bytes"
            ElseIf size > (2 ^ 10) And size <= (2 ^ 20) Then
                If roundToNearestWholeNumber = True Then
                    result = Math.Round(size / (2 ^ 10), 0) & " KBs"
                Else
                    result = Math.Round(size / (2 ^ 10), 2) & " KBs"
                End If
            ElseIf size > (2 ^ 20) And size <= (2 ^ 30) Then
                If roundToNearestWholeNumber = True Then
                    result = Math.Round(size / (2 ^ 20), 0) & " MBs"
                Else
                    result = Math.Round(size / (2 ^ 20), 2) & " MBs"
                End If
            ElseIf size > (2 ^ 30) And size <= (2 ^ 40) Then
                If roundToNearestWholeNumber = True Then
                    result = Math.Round(size / (2 ^ 30), 0) & " GBs"
                Else
                    result = Math.Round(size / (2 ^ 30), 2) & " GBs"
                End If
            ElseIf size > (2 ^ 40) And size <= (2 ^ 50) Then
                If roundToNearestWholeNumber = True Then
                    result = Math.Round(size / (2 ^ 40), 0) & " TBs"
                Else
                    result = Math.Round(size / (2 ^ 40), 2) & " TBs"
                End If
            ElseIf size > (2 ^ 50) And size <= (2 ^ 60) Then
                If roundToNearestWholeNumber = True Then
                    result = Math.Round(size / (2 ^ 50), 0) & " PBs"
                Else
                    result = Math.Round(size / (2 ^ 50), 2) & " PBs"
                End If
            Else
                result = "(None)"
            End If

            Return result
        End Function

        ''' <summary>Executes a command at the command line and returns the output in the commandLineOutput ByRef variable.</summary>
        ''' <param name="commandLineOutput">The output of the command that was ran by this command. This is a ByRef variable.</param>
        ''' <param name="commandToExecute">The command to be run by this function.</param>
        ''' <param name="commandLineArgument">The arguments you want passed to the command to be run by this function.</param>
        ''' <returns>A Boolean value. If True the command was successful, if it returns a False value the command failed.</returns>
        Public Function executeShellCommandAndGetOutput(ByRef commandLineOutput As String, ByVal commandToExecute As String, Optional ByVal commandLineArgument As String = Nothing) As Boolean
            Try
                Dim processObject As New Process()
                processObject.StartInfo.FileName = commandToExecute

                If commandLineArgument <> Nothing Then
                    processObject.StartInfo.Arguments = commandLineArgument
                End If

                processObject.StartInfo.CreateNoWindow = True
                processObject.StartInfo.UseShellExecute = False
                processObject.StartInfo.RedirectStandardOutput = True
                processObject.Start()

                commandLineOutput = processObject.StandardOutput.ReadToEnd()
                Return True
            Catch ex As Exception
                Return False
            End Try
        End Function

        Private Function tryToParseShort(value As String) As Boolean
            Dim number As Short
            Dim result As Boolean = Short.TryParse(value, number)

            If result Then
                Return True
            Else
                Return False
            End If
        End Function

        ''' <summary>Tests to see if the RegEx pattern is valid or not.</summary>
        ''' <param name="strPattern">A RegEx pattern.</param>
        ''' <returns>A Boolean value. If True, the RegEx pattern is valid. If False, the RegEx pattern is not valid.</returns>
        Public Function boolTestRegExPattern(strPattern As String) As Boolean
            Try
                Dim testRegExPattern As New Regex(strPattern)
                Return True
            Catch ex As Exception
                Return False
            End Try
        End Function

        Public Sub setSafeModeBoot()
            Try
                Dim bcdEditor As New BCD.bcdEditor()
                bcdEditor.setSafeModeBootFlag()
                bcdEditor.dispose()
                bcdEditor = Nothing

                eventLogFunctions.writeToSystemEventLog("Successfully set Safe Mode Boot flag.", EventLogEntryType.Information)
            Catch ex As Exception
                eventLogFunctions.writeCrashToEventLog(ex)
                eventLogFunctions.writeToSystemEventLog("Unable to set Safe Mode Boot flag.", EventLogEntryType.Error)
            End Try
        End Sub

        Public Sub removeSafeModeBoot()
            Try
                Dim bcdEditor As New BCD.bcdEditor()

                If bcdEditor.getSafeModeBootStatus() Then
                    bcdEditor.removeSafeModeBootFlag()
                    eventLogFunctions.writeToSystemEventLog("Successfully removed Safe Mode Boot flag.", EventLogEntryType.Information)
                End If

                bcdEditor.dispose()
                bcdEditor = Nothing
            Catch ex As Exception
                eventLogFunctions.writeCrashToEventLog(ex)
                eventLogFunctions.writeToSystemEventLog("Unable to remove Safe Mode Boot flag.", EventLogEntryType.Error)
            End Try
        End Sub

        Public Sub executeCommand(pathToExecutable As String, arguments As String, Optional runAsAdmin As Boolean = False)
            If IO.File.Exists(pathToExecutable) = True Then
                Dim processStartInfo As New ProcessStartInfo()
                processStartInfo.FileName = pathToExecutable
                processStartInfo.WindowStyle = ProcessWindowStyle.Hidden
                processStartInfo.Arguments = arguments
                processStartInfo.CreateNoWindow = True

                If runAsAdmin = True Then processStartInfo.Verb = "runas"

                Process.Start(processStartInfo)
            End If
        End Sub

        Public Sub executeCommand(pathToExecutable As String, Optional runAsAdmin As Boolean = False)
            If IO.File.Exists(pathToExecutable) = True Then
                Dim processStartInfo As New ProcessStartInfo()
                processStartInfo.FileName = pathToExecutable
                processStartInfo.WindowStyle = ProcessWindowStyle.Hidden
                processStartInfo.CreateNoWindow = True

                If runAsAdmin = True Then processStartInfo.Verb = "runas"

                Process.Start(processStartInfo)
            End If
        End Sub

        Public Sub executeCommandWithWait(pathToExecutable As String, arguments As String, Optional runAsAdmin As Boolean = False)
            If IO.File.Exists(pathToExecutable) = True Then
                Dim processStartInfo As New ProcessStartInfo()
                processStartInfo.FileName = pathToExecutable
                processStartInfo.WindowStyle = ProcessWindowStyle.Hidden
                processStartInfo.Arguments = arguments
                processStartInfo.CreateNoWindow = True

                If runAsAdmin = True Then processStartInfo.Verb = "runas"

                Process.Start(processStartInfo).WaitForExit()
            End If
        End Sub

        Public Sub executeCommandWithWait(pathToExecutable As String, Optional runAsAdmin As Boolean = False)
            If IO.File.Exists(pathToExecutable) = True Then
                Dim processStartInfo As New ProcessStartInfo()
                processStartInfo.FileName = pathToExecutable
                processStartInfo.WindowStyle = ProcessWindowStyle.Hidden
                processStartInfo.CreateNoWindow = True

                If runAsAdmin = True Then processStartInfo.Verb = "runas"

                Process.Start(processStartInfo).WaitForExit()
            End If
        End Sub

        Public Sub rebootSystem()
            Dim strPathToShutDown As String = IO.Path.Combine(globalVariables.strPathToSystemFolder, "shutdown.exe")

            If IO.File.Exists(strPathToShutDown) = True Then
                executeCommand(strPathToShutDown, "-r -t 0", True)
            Else
                MsgBox("Unable to find the Windows command line reboot tool to trigger a reboot. You will have to manually trigger a reboot yourself.", MsgBoxStyle.Exclamation, "Restore Point Creator")
            End If
        End Sub

        Public Sub createScheduledSystemRestorePoint(strRestorePointDescription As String)
            Try
                Dim integerNewRestorePointID As Integer
                wmi.createRestorePoint(strRestorePointDescription, RestoreType.WindowsType, integerNewRestorePointID)
            Catch ex As Exception
                eventLogFunctions.writeCrashToEventLog(ex)
                Process.GetCurrentProcess.Kill()
            End Try
        End Sub

        Public Sub writeSystemRestorePointsToApplicationLogs()
            Dim restorePoints As New Text.StringBuilder
            Dim systemRestorePointsManagementObjectSearcher As New Management.ManagementObjectSearcher("root\DEFAULT", "SELECT * FROM SystemRestore")
            Dim restorePointCreationDate As Date

            If systemRestorePointsManagementObjectSearcher IsNot Nothing Then
                Dim restorePointsOnSystemManagementObjectCollection As Management.ManagementObjectCollection = systemRestorePointsManagementObjectSearcher.Get()

                If restorePointsOnSystemManagementObjectCollection IsNot Nothing Then
                    If (restorePointsOnSystemManagementObjectCollection.Count = 0) = False Then
                        restorePoints.AppendLine("Number of Restore Points: " & restorePointsOnSystemManagementObjectCollection.Count)
                        restorePoints.AppendLine("=========================")
                        For Each restorePointDetails As Management.ManagementObject In restorePointsOnSystemManagementObjectCollection
                            If (restorePointDetails("SequenceNumber") IsNot Nothing) And (restorePointDetails("CreationTime") IsNot Nothing) And (restorePointDetails("Description") IsNot Nothing) And (restorePointDetails("RestorePointType") IsNot Nothing) Then
                                restorePoints.Append(restorePointDetails("SequenceNumber").ToString & " | " & restorePointDetails("Description").ToString & " | ")

                                If String.IsNullOrEmpty(restorePointDetails("CreationTime").ToString.Trim) Then
                                    restorePoints.Append("Error Parsing Date")
                                Else
                                    restorePointCreationDate = parseSystemRestorePointCreationDate(restorePointDetails("CreationTime").ToString)
                                    restorePoints.Append(String.Format("{0} {1}", restorePointCreationDate.ToShortDateString, restorePointCreationDate.ToLongTimeString))
                                End If

                                restorePoints.Append(" | " & whatTypeOfRestorePointIsIt(Integer.Parse(restorePointDetails("RestorePointType").ToString)))
                                restorePoints.AppendLine()
                            End If
                        Next
                    End If
                End If
            End If

            systemRestorePointsManagementObjectSearcher.Dispose()
            systemRestorePointsManagementObjectSearcher = Nothing

            restorePoints.AppendLine("=========================")
            eventLogFunctions.writeToSystemEventLog(restorePoints.ToString.Trim, EventLogEntryType.Information)
        End Sub

        ''' <summary>Converts an Integer representing days to English. Like "1 day" or "2 days".</summary>
        ''' <param name="input">Days.</param>
        ''' <returns>A String.</returns>
        Private Function daysToString(input As Integer) As String
            If input = 1 Then
                Return "1 day"
            Else
                Return input & " days"
            End If
        End Function

        ''' <summary>Creates a Window Shortcut.</summary>
        ''' <param name="locationOfShortcut">The location where we want to create the Windows shortcut.</param>
        ''' <param name="pathToExecutable">The path to the executable that the shortcut will launch.</param>
        ''' <param name="iconPath">The path to the icon that the new shortcut will use.</param>
        ''' <param name="Title">The name of the new shortcut.</param>
        ''' <param name="arguments">Any and all command line arguments that the invoked executable will be launched with.</param>
        Public Sub createShortcut(ByVal locationOfShortcut As String, pathToExecutable As String, iconPath As String, ByVal Title As String, Optional arguments As String = Nothing)
            Try
                Dim WshShell As New IWshRuntimeLibrary.WshShell
                ' short cut files have a .lnk extension
                Dim shortCut As IWshRuntimeLibrary.IWshShortcut = DirectCast(WshShell.CreateShortcut(locationOfShortcut), IWshRuntimeLibrary.IWshShortcut)

                ' set the shortcut properties
                With shortCut
                    .TargetPath = pathToExecutable

                    If (arguments = Nothing) = False Then
                        .Arguments = arguments
                    End If

                    .WindowStyle = 1I
                    .Description = Title
                    ' the next line gets the first Icon from the executing program
                    .IconLocation = iconPath & ", 0"
                    .Save() ' save the shortcut file
                End With
            Catch ex As Exception
            End Try
        End Sub

        Public Sub launchRuntimeTaskFixRoutine()
            Try
                Dim startInfo As New ProcessStartInfo
                startInfo.FileName = New IO.FileInfo(Application.ExecutablePath).FullName
                startInfo.Arguments = "-fixruntimetasks"

                If privilegeChecks.areWeAnAdministrator() = False Then startInfo.Verb = "runas"

                Process.Start(startInfo)
                Process.GetCurrentProcess.Kill()
            Catch ex As ComponentModel.Win32Exception
                eventLogFunctions.writeCrashToEventLog(ex)
                MsgBox("There was an error while attempting to elevate the process, please make sure that when the Windows UAC prompt appears asking you to run the program with elevated privileges that you say ""Yes"" to the UAC prompt." & vbCrLf & vbCrLf & "The program will now terminate.", MsgBoxStyle.Critical, globalVariables.programName)
                Process.GetCurrentProcess.Kill()
            End Try
        End Sub

        Public Sub reRunWithAdminUserRights()
            Try
                Dim startInfo As New ProcessStartInfo
                startInfo.FileName = New IO.FileInfo(Application.ExecutablePath).FullName

                If Environment.GetCommandLineArgs.Count <> 1 Then
                    startInfo.Arguments = Environment.GetCommandLineArgs(1)
                End If

                If privilegeChecks.areWeAnAdministrator() = False Then startInfo.Verb = "runas"

                Process.Start(startInfo)
                Process.GetCurrentProcess.Kill()
            Catch ex As ComponentModel.Win32Exception
                eventLogFunctions.writeCrashToEventLog(ex)
                MsgBox("There was an error while attempting to elevate the process, please make sure that when the Windows UAC prompt appears asking you to run the program with elevated privileges that you say ""Yes"" to the UAC prompt." & vbCrLf & vbCrLf & "The program will now terminate.", MsgBoxStyle.Critical, globalVariables.programName)
                Process.GetCurrentProcess.Kill()
            End Try
        End Sub

        ''' <summary>Returns the creation date of the System Restore Point ID you pass to the function.</summary>
        ''' <param name="restorePointID">Data type, Short. The ID of the System Restore Point you want to return the creation date of.</param>
        ''' <returns>A Date Object.</returns>
        Private Function getSystemRestorePointDate(restorePointID As Short) As Date
            Try
                Dim newestSystemRestoreID As Integer = 0 ' Resets the newest System Restore ID to 0.

                ' Get all System Restore Points from the Windows Management System and puts then in the systemRestorePoints variable.
                Dim systemRestorePoints As New Management.ManagementObjectSearcher("root\DEFAULT", "SELECT * FROM SystemRestore WHERE SequenceNumber = " & restorePointID)

                If systemRestorePoints.Get().Count <> 0 Then
                    Return parseSystemRestorePointCreationDate(systemRestorePoints.Get(0)("CreationTime").ToString, True)
                Else
                    Return Nothing
                End If
            Catch ex As Exception
                Threading.Thread.CurrentThread.CurrentUICulture = New Globalization.CultureInfo("en-US")
                exceptionHandler.manuallyLoadCrashWindow(ex, ex.Message, ex.StackTrace, ex.GetType)
                Return Nothing
            End Try
        End Function

        Public Function parseSystemRestorePointCreationDate(strDate As String, Optional boolFullDateParsing As Boolean = True) As Date
            Dim regexMatches As Match = globalVariables.regexRestorePointCreationTimeParser.Match(strDate)
            Dim year, month, day, second, minute, hour As Integer

            ' Gets the values out of the Regular Expression Matches object.
            With regexMatches
                year = Integer.Parse(.Groups("year").Value)
                month = Integer.Parse(.Groups("month").Value)
                day = Integer.Parse(.Groups("day").Value)
                second = Integer.Parse(.Groups("second").Value)
                minute = Integer.Parse(.Groups("minute").Value)
                hour = Integer.Parse(.Groups("hour").Value)
            End With

            If boolFullDateParsing = True Then
                Return New DateTime(year, month, day, hour, minute, second, DateTimeKind.Utc).ToLocalTime
            Else
                Return New Date(year, month, day)
            End If
        End Function

        Public Function areWeInSafeMode() As Boolean
            If SystemInformation.BootMode = BootMode.Normal Then
                Return False
            Else
                Return True
            End If
        End Function

        Public Enum RestoreType
            ApplicationInstall = 0 ' Installing a new application
            ApplicationUninstall = 1 ' An application has been uninstalled
            ModifySettings = 12 ' An application has had features added or removed
            CancelledOperation = 13 ' An application needs to delete the restore point it created
            Restore = 6 ' System Restore
            Checkpoint = 7 ' Checkpoint
            DeviceDriverInstall = 10 ' Device driver has been installed
            FirstRun = 11 ' Program used for 1st time
            BackupRecovery = 14 ' Restoring a backup
            WindowsType = 16 ' The type of restore point that Windows makes.
        End Enum

        ''' <summary>A function that returns the type of restore point depending upon the Integer that comes from the WMI.</summary>
        ''' <param name="type">Integer.</param>
        ''' <returns>Returns a String value containing the type of Restore Point in English.</returns>
        Public Function whatTypeOfRestorePointIsIt(type As Integer) As String
            If type = RestoreType.ApplicationInstall Then
                Return "Application Install"
            ElseIf type = RestoreType.ApplicationUninstall Then
                Return "Application Removal"
            ElseIf type = RestoreType.BackupRecovery Then
                Return "Backup Recovery"
            ElseIf type = RestoreType.CancelledOperation Then
                Return "Cancelled Operation"
            ElseIf type = RestoreType.Checkpoint Then
                Return "System Checkpoint"
            ElseIf type = RestoreType.DeviceDriverInstall Then
                Return "Device Driver Install"
            ElseIf type = RestoreType.FirstRun Then
                Return "First Run"
            ElseIf type = RestoreType.ModifySettings Then
                Return "Settings Modified"
            ElseIf type = RestoreType.Restore Then
                Return "Restore"
            ElseIf type = RestoreType.WindowsType Then
                Return "System Restore Point"
            Else
                Return "Unknown Type"
            End If
        End Function

        Public Sub killProcess(processID As Integer, Optional boolLogToEventLog As Boolean = False)
            If boolLogToEventLog = True Then
                eventLogFunctions.writeToSystemEventLog("Killing process with PID of " & processID & ".", EventLogEntryType.Information)
            End If

            Dim processObject As Process = Nothing

            ' First we are going to check if the Process ID exists.
            If doesProcessIDExist(processID, processObject) = True Then
                Try
                    processObject.Kill() ' Yes, it does so let's kill it.
                Catch ex As Exception
                    ' Wow, it seems that even with double-checking if a process exists by it's PID number things can still go wrong.
                    ' So this Try-Catch block is here to trap any possible errors when trying to kill a process by it's PID number.
                End Try
            End If

            processObject = Nothing
            Threading.Thread.Sleep(250) ' We're going to sleep to give the system some time to kill the process.

            '' Now we are going to check again if the Process ID exists and if it does, we're going to attempt to kill it again.
            If doesProcessIDExist(processID, processObject) = True Then
                Try
                    processObject.Kill()
                Catch ex As Exception
                    ' Wow, it seems that even with double-checking if a process exists by it's PID number things can still go wrong.
                    ' So this Try-Catch block is here to trap any possible errors when trying to kill a process by it's PID number.
                End Try
            End If

            processObject = Nothing
            Threading.Thread.Sleep(250) ' We're going to sleep (again) to give the system some time to kill the process.
        End Sub

        Public Function calculatePercentageValue(longSmallerNumber As ULong, longLargerNumber As ULong) As Double
            Dim result As Double = Math.Round(((longSmallerNumber / longLargerNumber) * 100), 2)

            If result > 100 Then
                Return 100
            ElseIf result < 0 Then
                Return 0
            Else
                Return result
            End If
        End Function

        Public Sub launchURLInWebBrowser(url As String, Optional errorMessage As String = "An error occurred when trying the URL In your Default browser. The URL has been copied to your Windows Clipboard for you to paste into the address bar in the web browser of your choice.")
            If url.Trim.StartsWith("http", StringComparison.OrdinalIgnoreCase) = False Then
                If My.Settings.useSSL = True Then
                    url = "https://" & url
                Else
                    url = "http://" & url
                End If
            End If

            Try
                Dim associatedApplication As String = Nothing

                If registryStuff.getFileAssociation(".html", associatedApplication) = False Then
                    Process.Start(url)
                Else
                    If IO.File.Exists(associatedApplication) = True Then
                        Process.Start(associatedApplication, Chr(34) & url & Chr(34))
                    Else
                        Process.Start(url)
                    End If
                End If
            Catch ex2 As ComponentModel.Win32Exception
                eventLogFunctions.writeCrashToEventLog(ex2)
                MsgBox("There was an error attempting to launch your web browser. Perhaps rebooting your system will correct this issue.", MsgBoxStyle.Information, "Restore Point Creator")
            Catch ex As Exception
                copyTextToWindowsClipboard(url)
                MsgBox(errorMessage, MsgBoxStyle.Information, "Restore Point Creator")
            End Try
        End Sub

        Public Function removeSourceCodePathInfo(strInput As String) As String
            If strInput.regExSearch("(?:Google Drive|OneDrive)") = True Then
                Return Regex.Replace(strInput, "C:\\Users\\Tom\\(?:Google Drive|OneDrive)\\My Visual Studio Projects\\Projects\\", "", RegexOptions.IgnoreCase)
            Else
                Return strInput
            End If
        End Function

        Private Function convertToJSON(input As Object) As String
            Return (New Web.Script.Serialization.JavaScriptSerializer).Serialize(input)
        End Function

        Public Function getDateInShortForm() As String
            Dim year, month, day As String
            year = Now.Year
            month = Now.Month
            day = Now.Day

            If month.Length = 1 Then month = "0" & month
            If day.Length = 1 Then day = "0" & day

            Return year & month & day
        End Function

        Public Function convertToBase64(input As String) As String
            Return Convert.ToBase64String(Text.Encoding.UTF8.GetBytes(input))
        End Function

        Public Function convertFromBase64(input As String) As String
            Return Text.Encoding.UTF8.GetString(Convert.FromBase64String(input))
        End Function

        Public Function isNumeric(input As String) As Boolean
            Return Regex.IsMatch(input.Trim, "\A-{0,1}[0-9.]*\Z")
        End Function

        Public Function randomStringGenerator(length As Integer)
            Dim random As Random = New Random()
            Dim builder As New Text.StringBuilder()
            Dim ch As Char
            Dim legalCharacters As String = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz1234567890"

            For cntr As Integer = 0 To length
                ch = legalCharacters.Substring(random.Next(0, legalCharacters.Length), 1)
                builder.Append(ch)
            Next

            Return builder.ToString()
        End Function
    End Module
End Namespace