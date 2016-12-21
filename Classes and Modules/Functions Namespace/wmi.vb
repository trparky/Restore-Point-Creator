Imports System.Text.RegularExpressions

Namespace Functions.wmi
    Module wmi
        ''' <summary>Gets the total capcity of a system drive.</summary>
        ''' <param name="driveLetter">C: or D:</param>
        ''' <returns></returns>
        Public Function getDriveSize(driveLetter As String) As ULong
            driveLetter = driveLetter.Trim
            If driveLetter.Length = 1 Then driveLetter &= ":"
            driveLetter = driveLetter.ToUpper

            Try
                Dim managementObjectSearcher As New Management.ManagementObjectSearcher("root\CIMV2", String.Format("SELECT Capacity FROM Win32_Volume WHERE DriveLetter = '{0}'", driveLetter))

                If managementObjectSearcher.Get.Count = 0 Then
                    Return 0
                Else
                    Dim managementObjectCollection As Management.ManagementObjectCollection = managementObjectSearcher.Get()
                    Dim capacityString As String = managementObjectCollection(0)("Capacity").ToString
                    Dim uLongCapacity As ULong

                    If ULong.TryParse(capacityString, uLongCapacity) Then
                        Return uLongCapacity
                    Else
                        Return 0
                    End If
                End If
            Catch err As Management.ManagementException
                Return 0
            End Try
        End Function

        ' driveLetter is just that, "C:"
        ''' <summary>Returns the DeviceID for a system drive letter.</summary>
        ''' <param name="driveLetter">A drive letter such as C, D, or E. It's optional if it ends with a ":", the function will fill in a ":" if it's not passed as part of the value passed to the function.</param>
        ''' <param name="boolResult">A ByRef Boolean value. If this function returns a True value, then the lookup was successful. If it fails, it returns a False value.</param>
        ''' <returns>A DeviceID GUID in String format.</returns>
        Public Function getDeviceIDFromDriveLetter(ByVal driveLetter As String, ByRef boolResult As Boolean) As String
            Try
                driveLetter = driveLetter.Trim.ToUpper
                If driveLetter.EndsWith(":") = False Then driveLetter &= ":"

                Dim wmiQueryObject As New Management.ObjectQuery(String.Format("SELECT * FROM Win32_Volume Where DriveLetter='{0}'", driveLetter))
                Dim managementObjectSearcher As New Management.ManagementObjectSearcher(wmiQueryObject)

                If managementObjectSearcher.Get.Count = 0 Then
                    managementObjectSearcher.Dispose()

                    boolResult = False
                    Return Nothing
                Else
                    Dim managementObjectCollection As Management.ManagementObjectCollection = managementObjectSearcher.Get()

                    Dim deviceID As String = managementObjectCollection(0)("DeviceID").ToString

                    managementObjectSearcher.Dispose()
                    managementObjectCollection.Dispose()

                    boolResult = True
                    Return deviceID
                End If
            Catch ex As Management.ManagementException
                eventLogFunctions.writeCrashToEventLog(ex)
                eventLogFunctions.writeToSystemEventLog("Unable to retrieve volumeID from WMI for system drive " & driveLetter & ".", EventLogEntryType.Error)
                MsgBox("Unable to retrieve volumeID from WMI for system drive " & driveLetter & "." & vbCrLf & vbCrLf & "The program will now terminate.", MsgBoxStyle.Critical, "Restore Point Creator")
                Process.GetCurrentProcess.Kill()

                boolResult = False
                Return Nothing
            End Try
        End Function

        Public Sub giveComExceptionCrashMessage()
            MsgBox("A program crash that should not happen on properly working Windows installations has occurred.  Your windows installation will need to be repaired before this program will function properly." & vbCrLf & vbCrLf & "Your web browser will now open with instructions on how to repair your Windows installation.", MsgBoxStyle.Critical, "Critical Windows Component Failure Detected")

            support.launchURLInWebBrowser(globalVariables.webURLs.webPages.strCOMExceptionCrash, "An error occurred when trying to launch a URL. The URL has been copied to your Windows Clipboard for you to paste into the address bar in the browser of your choice.")

            Process.GetCurrentProcess.Kill()
        End Sub

        Public Sub doDeletingOfXNumberOfRestorePoints(ByVal shortHowManyToKeep As Short)
            Dim systemRestorePoints As New Management.ManagementObjectSearcher("root\Default", "Select * FROM SystemRestore")

            If systemRestorePoints.Get.Count <> 0 Then
                Dim numberOfRestorePointsToBeDeleted As Short = systemRestorePoints.Get.Count - shortHowManyToKeep

                If numberOfRestorePointsToBeDeleted < 0 Or numberOfRestorePointsToBeDeleted = 0 Then
                    systemRestorePoints.Dispose()
                    systemRestorePoints = Nothing
                    Exit Sub
                Else
                    For Each systemRestorePoint As Management.ManagementObject In systemRestorePoints.Get()
                        If numberOfRestorePointsToBeDeleted = 0 Then
                            Exit For
                        Else
                            numberOfRestorePointsToBeDeleted -= 1

                            Dim restorePointCreationDate As Date = support.parseSystemRestorePointCreationDate(systemRestorePoint("CreationTime").ToString)
                            eventLogFunctions.writeToSystemEventLog(String.Format("The user {3}/{4} deleted the restore point named ""{0}"" which was created on {1} at {2}.", systemRestorePoint("Description").ToString, restorePointCreationDate.ToShortDateString, restorePointCreationDate.ToShortTimeString, Environment.MachineName, Environment.UserName), EventLogEntryType.Information)

                            support.SRRemoveRestorePoint(Integer.Parse(systemRestorePoint("SequenceNumber").ToString))
                        End If
                    Next
                End If
            End If

            systemRestorePoints.Dispose()
            systemRestorePoints = Nothing
        End Sub

        Public Function checkToSeeIfSystemRestoreIsEnabledOnSystemDrive() As Boolean
            Try
                Dim systemRestorePoints As New Management.ManagementObjectSearcher("root\DEFAULT", "SELECT * FROM SystemRestore")

                If systemRestorePoints.Get().Count = 0 Then
                    Return False
                Else
                    Return True
                End If
            Catch ex As Exception
                Return False
            End Try
        End Function

        Public Function getNumberOfRestorePoints() As Integer
            Try
                Dim systemRestorePointsManagementObjectSearcher As New Management.ManagementObjectSearcher("root\DEFAULT", "SELECT * FROM SystemRestore")

                If systemRestorePointsManagementObjectSearcher IsNot Nothing Then
                    Dim restorePointsOnSystemManagementObjectCollection As Management.ManagementObjectCollection = systemRestorePointsManagementObjectSearcher.Get()

                    If restorePointsOnSystemManagementObjectCollection IsNot Nothing Then
                        Return restorePointsOnSystemManagementObjectCollection.Count
                    End If
                End If

                Return 0
            Catch ex As Exception
                Return 0
            End Try
        End Function

        Public Function createRestorePoint(restorePointName As String, restorePointType As support.RestoreType, ByRef restorePointID As Long) As Integer
            Try
                vss.checkForAndEnableSystemRestoreIfNeeded()

                Dim managementScope As New Management.ManagementScope("\\localhost\root\default")
                Dim managementPath As New Management.ManagementPath("SystemRestore")
                Dim managementOptions As New Management.ObjectGetOptions()
                Dim managementClass As New Management.ManagementClass(managementScope, managementPath, managementOptions)

                Dim managementParameters As Management.ManagementBaseObject = managementClass.GetMethodParameters("CreateRestorePoint")
                managementParameters("Description") = restorePointName
                managementParameters("RestorePointType") = restorePointType
                managementParameters("EventType") = 100

                Dim oOutParams As Management.ManagementBaseObject = managementClass.InvokeMethod("CreateRestorePoint", managementParameters, Nothing)

                eventLogFunctions.writeToSystemEventLog("Created System Restore Point (" & restorePointName & ").", EventLogEntryType.Information)

                Return oOutParams("ReturnValue")
            Catch ex4 As UnauthorizedAccessException
                Try
                    eventLogFunctions.writeToSystemEventLog("Falling back to core Windows APIs to create restore point.", EventLogEntryType.Warning)
                    Return APIs.systemRestore.StartRestore(restorePointName, restorePointType, restorePointID)
                Catch ex6 As Exception
                    eventLogFunctions.writeCrashToEventLog(ex6)
                    eventLogFunctions.writeToSystemEventLog("Unable to create system restore point. System permissions seem to not allow it.", EventLogEntryType.Error)
                    MsgBox("Unable to create system restore point. System permissions seem to not allow it.", MsgBoxStyle.Critical, "Error Creating System Restore Point")

                    Return APIs.errorCodes.ERROR_ACCESS_DENIED
                End Try
            Catch ex3 As Runtime.InteropServices.COMException
                Try
                    eventLogFunctions.writeToSystemEventLog("Falling back to core Windows APIs to create restore point.", EventLogEntryType.Warning)
                    Return APIs.systemRestore.StartRestore(restorePointName, restorePointType, restorePointID)
                Catch ex5 As Exception
                    eventLogFunctions.writeCrashToEventLog(ex5)
                    giveComExceptionCrashMessage()

                    Return APIs.errorCodes.ERROR_INTERNAL_ERROR
                End Try
            Catch ex As Exception
                Threading.Thread.CurrentThread.CurrentUICulture = New Globalization.CultureInfo("en-US")

                If privilegeChecks.areWeRunningAsSystemUser() = False Then
                    exceptionHandler.manuallyLoadCrashWindow(ex)
                End If

                eventLogFunctions.writeCrashToEventLog(ex)

                Return APIs.errorCodes.ERROR_INTERNAL_ERROR
            End Try
        End Function

        Public Sub restoreToSystemRestorePoint(point As Long)
            Try
                eventLogFunctions.writeToSystemEventLog("Restoring system back to System Restore Point ID " & point & " (" & wmi.getRestorePointName(point) & ").", EventLogEntryType.Information)

                Dim managementScope As New Management.ManagementScope("\\localhost\root\default")
                Dim managementPath As New Management.ManagementPath("SystemRestore")
                Dim managementOptions As New Management.ObjectGetOptions()
                Dim managementClass As New Management.ManagementClass(managementScope, managementPath, managementOptions)

                Dim managementParameters As Management.ManagementBaseObject = managementClass.GetMethodParameters("Restore")
                managementParameters("SequenceNumber") = point

                Dim oOutParams As Management.ManagementBaseObject = managementClass.InvokeMethod("Restore", managementParameters, Nothing)

                support.rebootSystem()
            Catch ex4 As ArgumentException
                eventLogFunctions.writeCrashToEventLog(ex4)
                MsgBox("Unable to restore system to selected restore point, a COM Exception has occurred. Restore process aborted.", MsgBoxStyle.Critical, "Restore Point Creator")
            Catch ex3 As Runtime.InteropServices.COMException
                giveComExceptionCrashMessage()
            Catch ex2 As IO.FileLoadException
                eventLogFunctions.writeCrashToEventLog(ex2)
                MsgBox("Unable to load required system assemblies to perform system operation. Please refer to the Application Event Log for more details and to submit the crash event to me." & vbCrLf & vbCrLf & "This program will now close.", MsgBoxStyle.Critical, "Restore Point Creator FileLoadException Handler")
                Process.GetCurrentProcess.Kill()
            Catch ex As Exception
                Threading.Thread.CurrentThread.CurrentUICulture = New System.Globalization.CultureInfo("en-US")
                exceptionHandler.manuallyLoadCrashWindow(ex, ex.Message, ex.StackTrace, ex.GetType)
            End Try
        End Sub

        Private Function getRestorePointName(id As Long) As String
            Try
                Dim systemRestorePoints As New Management.ManagementObjectSearcher("root\DEFAULT", "SELECT * FROM SystemRestore")

                If (systemRestorePoints.Get().Count = 0) = False Then
                    For Each systemRestorePoint As Management.ManagementObject In systemRestorePoints.Get()
                        If Long.Parse(systemRestorePoint("SequenceNumber").ToString) = id Then
                            Return systemRestorePoint("Description").ToString.ToString
                        End If
                    Next
                End If

                eventLogFunctions.writeToSystemEventLog("Unable to find description for restore point ID " & id & ".", EventLogEntryType.Error)
                Return "ERROR_NO_DESCRIPTION"
            Catch ex As Exception
                eventLogFunctions.writeToSystemEventLog("Unable to find description for restore point ID " & id & ".", EventLogEntryType.Error)
                Return "ERROR_NO_DESCRIPTION"
            End Try
        End Function

        Public Function getNewestSystemRestorePointID() As Integer
            Try
                Dim newestSystemRestoreID As Integer = 0 ' Resets the newest System Restore ID to 0.

                ' Get all System Restore Points from the Windows Management System and puts then in the systemRestorePoints variable.
                Dim systemRestorePoints As New Management.ManagementObjectSearcher("root\DEFAULT", "SELECT * FROM SystemRestore")

                Dim systemRestoreIDs As New ArrayList

                ' Checks to see if there are any System Restore Points to be listed.
                If (systemRestorePoints.Get().Count = 0) = False Then
                    ' Loops through systemRestorePoints.
                    For Each systemRestorePoint As Management.ManagementObject In systemRestorePoints.Get()
                        systemRestoreIDs.Add(systemRestorePoint("SequenceNumber").ToString)
                    Next

                    If systemRestoreIDs.Count = 0 Then
                        newestSystemRestoreID = 0
                    Else
                        newestSystemRestoreID = Integer.Parse(systemRestoreIDs.Item(systemRestoreIDs.Count - 1))
                    End If
                Else
                    newestSystemRestoreID = 0
                End If

                Return newestSystemRestoreID
            Catch ex3 As UnauthorizedAccessException
                Return 0
            Catch ex2 As Management.ManagementException
                Return 0
            Catch ex4 As ArgumentOutOfRangeException
                Return 0
            Catch ex3 As Runtime.InteropServices.COMException
                giveComExceptionCrashMessage()
                Return 0
            End Try
        End Function

        Private Function getNewestSystemRestorePointIDMadeByRestorePointCreator(strRestorePointDescription As String) As Integer
            Try
                Dim newestSystemRestoreID As Integer = 0 ' Resets the newest System Restore ID to 0.

                ' Get all System Restore Points from the Windows Management System and puts then in the systemRestorePoints variable.
                Dim systemRestorePoints As New Management.ManagementObjectSearcher("root\DEFAULT", "SELECT * FROM SystemRestore")

                Dim systemRestoreIDs As New ArrayList
                Dim restorePointNameOnSystem As String

                ' Checks to see if there are any System Restore Points to be listed.
                If systemRestorePoints.Get().Count <> 0 Then
                    ' Loops through systemRestorePoints.
                    For Each systemRestorePoint As Management.ManagementObject In systemRestorePoints.Get()
                        restorePointNameOnSystem = systemRestorePoint("Description").ToString.Trim

                        ' We need only the Restore Point IDs made by this program, nothing else.
                        If restorePointNameOnSystem = globalVariables.strDefaultNameForScheduledTasks Or restorePointNameOnSystem = strRestorePointDescription Then
                            systemRestoreIDs.Add(systemRestorePoint("SequenceNumber").ToString)
                        End If
                    Next

                    If systemRestoreIDs.Count = 0 Then
                        Return 0
                    Else
                        systemRestoreIDs.Sort()
                        Return Integer.Parse(systemRestoreIDs.Item(systemRestoreIDs.Count - 1))
                    End If
                Else
                    Return 0
                End If

                'Return newestSystemRestoreID
            Catch ex As Exception
                Return 0
            End Try
        End Function

        Public Function getSystemRAM() As String
            Try
                Dim computerInfo As New Devices.ComputerInfo()
                Return support.bytesToHumanSize(computerInfo.TotalPhysicalMemory, True)
            Catch ex As Exception
                Return "Unknown amount of System RAM"
            End Try
        End Function

        Public Function getSystemProcessor() As supportClasses.processorInfoClass
            Try
                Dim searcher As New Management.ManagementObjectSearcher("root\CIMV2", "Select * FROM Win32_Processor")
                Dim queryObj As Management.ManagementObjectCollection = searcher.Get()

                Dim rawProcessorName As String = queryObj(0)("Name").ToString.Trim
                rawProcessorName = Regex.Replace(rawProcessorName, "\(R\)", "", RegexOptions.IgnoreCase)
                rawProcessorName = Regex.Replace(rawProcessorName, "\(TM\)", "", RegexOptions.IgnoreCase)

                Dim processorInfo As New supportClasses.processorInfoClass
                processorInfo.strProcessor = rawProcessorName
                processorInfo.strNumberOfCores = queryObj(0)("NumberOfCores").ToString
                Return processorInfo
            Catch ex As Exception
                Dim processorInfo As New supportClasses.processorInfoClass
                processorInfo.strProcessor = "unknown"
                processorInfo.strNumberOfCores = "unknown"
                Return processorInfo
            End Try
        End Function
    End Module
End Namespace