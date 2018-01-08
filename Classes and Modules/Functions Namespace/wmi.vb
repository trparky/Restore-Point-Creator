Imports System.Text.RegularExpressions

Namespace Functions.wmi
    Module wmi
        ''' <summary>Gets the startup type for a system service.</summary>
        ''' <param name="strServiceName">The name of the System Service you are querying the status of.</param>
        ''' <returns>A String Value.</returns>
        ''' <exception cref="ArgumentException">If this function returns an ArgumentException exception it means that the program could not find the system service that you passed to it.</exception>
        Public Function getServiceStartType(strServiceName As String) As String
            Dim startMode As String = ""

            Using managementObjectSearcher As Management.ManagementObjectSearcher = New Management.ManagementObjectSearcher(String.Format("SELECT StartMode FROM Win32_Service WHERE Name = '{0}'", strServiceName))
                If managementObjectSearcher Is Nothing Then
                    Throw New ArgumentException("Unable to find the System Service named " & strServiceName & ".")
                Else
                    Try
                        Dim managementObject As Management.ManagementObject = managementObjectSearcher.Get()(0)
                        startMode = managementObject.GetPropertyValue("StartMode").ToString()
                    Catch ex As Exception
                        eventLogFunctions.writeCrashToEventLog(ex)
                        eventLogFunctions.writeToApplicationLogFile("Unable to get Startup Type for the " & strServiceName & " System Service.", EventLogEntryType.Error)
                    End Try
                End If
            End Using

            Return startMode
        End Function

        Public Sub setServiceStartMode(strServiceName As String, Optional strStartType As String = "Manual")
            Try
                Dim managementObject As New Management.ManagementObject("root\CIMV2", "Win32_Service.Name='" & strServiceName & "'", Nothing)
                Dim managementParameters As Management.ManagementBaseObject = managementObject.GetMethodParameters("ChangeStartMode")
                managementParameters("StartMode") = strStartType
                managementObject.InvokeMethod("ChangeStartMode", managementParameters, Nothing)
            Catch ex As Exception
                eventLogFunctions.writeCrashToEventLog(ex)
                eventLogFunctions.writeToApplicationLogFile("Unable to set Startup Type for the " & strServiceName & " System Service.", EventLogEntryType.Error)
            End Try
        End Sub

        ''' <summary>Gets the total capcity of a system drive.</summary>
        ''' <param name="driveLetter">C: or D:</param>
        ''' <returns></returns>
        Public Function getDriveSize(driveLetter As String) As ULong
            driveLetter = driveLetter.Trim
            If driveLetter.Length = 1 Then driveLetter &= ":"
            driveLetter = driveLetter.ToUpper

            Try
                Dim managementObjectSearcher As New Management.ManagementObjectSearcher("root\CIMV2", String.Format("SELECT Capacity FROM Win32_Volume WHERE DriveLetter = '{0}'", driveLetter))

                If managementObjectSearcher IsNot Nothing Then
                    Dim managementObjectCollection As Management.ManagementObjectCollection = managementObjectSearcher.Get()

                    If managementObjectCollection IsNot Nothing Then
                        If managementObjectCollection.Count = 0 Then
                            Return 0
                        Else
                            Dim capacityString As String = managementObjectCollection(0)("Capacity").ToString
                            Dim uLongCapacity As ULong

                            If ULong.TryParse(capacityString, uLongCapacity) Then
                                Return uLongCapacity
                            Else
                                Return 0
                            End If
                        End If
                    Else
                        Return 0
                    End If
                Else
                    Return 0
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

                If managementObjectSearcher IsNot Nothing Then
                    Dim managementObjectCollection As Management.ManagementObjectCollection = managementObjectSearcher.Get()

                    If managementObjectCollection IsNot Nothing Then
                        If managementObjectCollection.Count = 0 Then
                            managementObjectSearcher.Dispose()

                            boolResult = False
                            Return Nothing
                        Else
                            Dim deviceID As String = managementObjectCollection(0)("DeviceID").ToString

                            managementObjectSearcher.Dispose()
                            managementObjectCollection.Dispose()

                            boolResult = True
                            Return deviceID
                        End If
                    Else
                        boolResult = False
                        Return Nothing
                    End If
                Else
                    boolResult = False
                    Return Nothing
                End If
            Catch ex As Management.ManagementException
                eventLogFunctions.writeToApplicationLogFile("Unable to retrieve volumeID from WMI for system drive " & driveLetter & ".", EventLogEntryType.Error)
                eventLogFunctions.writeCrashToEventLog(ex)
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
            Try
                Dim managementObjectSearcher As New Management.ManagementObjectSearcher("root\Default", "Select * FROM SystemRestore")

                If managementObjectSearcher IsNot Nothing Then
                    Dim managementObjectCollection As Management.ManagementObjectCollection = managementObjectSearcher.Get()

                    If managementObjectCollection IsNot Nothing Then
                        If managementObjectCollection.Count <> 0 Then
                            Dim numberOfRestorePointsToBeDeleted As Short = managementObjectCollection.Count - shortHowManyToKeep

                            If numberOfRestorePointsToBeDeleted < 0 Or numberOfRestorePointsToBeDeleted = 0 Then
                                managementObjectSearcher.Dispose()
                                managementObjectSearcher = Nothing
                                Exit Sub
                            Else
                                If numberOfRestorePointsToBeDeleted = 1 Then
                                    eventLogFunctions.writeToApplicationLogFile("Preparing to delete 1 restore point.")
                                Else
                                    eventLogFunctions.writeToApplicationLogFile("Preparing to delete " & numberOfRestorePointsToBeDeleted & " restore points.")
                                End If

                                For Each managementObject As Management.ManagementObject In managementObjectCollection
                                    If numberOfRestorePointsToBeDeleted = 0 Then
                                        Exit For
                                    Else
                                        numberOfRestorePointsToBeDeleted -= 1

                                        Dim restorePointCreationDate As Date = restorePointStuff.parseSystemRestorePointCreationDate(managementObject("CreationTime").ToString)
                                        eventLogFunctions.writeToApplicationLogFile(String.Format("The user {3}/{4} deleted the restore point named ""{0}"" which was created on {1} at {2}.", managementObject("Description").ToString, restorePointCreationDate.ToShortDateString, restorePointCreationDate.ToShortTimeString, Environment.MachineName, Environment.UserName), EventLogEntryType.Information)

                                        APIs.NativeMethods.SRRemoveRestorePoint(Integer.Parse(managementObject("SequenceNumber").ToString))
                                    End If
                                Next
                            End If
                        End If

                        managementObjectSearcher.Dispose()
                        managementObjectSearcher = Nothing
                    End If
                End If
            Catch ex As Exception
                eventLogFunctions.writeCrashToEventLog(ex)
            End Try
        End Sub

        Public Function checkToSeeIfSystemRestoreIsEnabledOnSystemDrive() As Boolean
            If getNumberOfRestorePoints() = 0 Then
                Return False
            Else
                Return True
            End If
        End Function

        Public Function getNumberOfRestorePoints() As Integer
            Try
                Dim managementObjectSearcher As New Management.ManagementObjectSearcher("root\DEFAULT", "SELECT * FROM SystemRestore")

                If managementObjectSearcher IsNot Nothing Then
                    Dim managementObjectCollection As Management.ManagementObjectCollection = managementObjectSearcher.Get()

                    If managementObjectCollection IsNot Nothing Then
                        managementObjectSearcher.Dispose()
                        Return managementObjectCollection.Count
                    Else
                        Return 0
                    End If
                End If

                Return 0
            Catch ex As Exception
                Return 0
            End Try
        End Function

        Public Function createRestorePoint(restorePointName As String, restorePointType As restorePointStuff.RestoreType, ByRef restorePointID As Long) As Integer
            Try
                Dim managementScope As New Management.ManagementScope("\\localhost\root\default")
                Dim managementPath As New Management.ManagementPath("SystemRestore")
                Dim managementOptions As New Management.ObjectGetOptions()
                Dim managementClass As New Management.ManagementClass(managementScope, managementPath, managementOptions)

                Dim managementParameters As Management.ManagementBaseObject = managementClass.GetMethodParameters("CreateRestorePoint")
                managementParameters("Description") = restorePointName
                managementParameters("RestorePointType") = restorePointType
                managementParameters("EventType") = 100

                Dim oOutParams As Management.ManagementBaseObject = managementClass.InvokeMethod("CreateRestorePoint", managementParameters, Nothing)

                eventLogFunctions.writeToApplicationLogFile("Created System Restore Point (" & restorePointName & ").", EventLogEntryType.Information)

                Return oOutParams("ReturnValue")
            Catch ex4 As UnauthorizedAccessException
                eventLogFunctions.writeCrashToEventLog(ex4)
                eventLogFunctions.writeToApplicationLogFile("Falling back to core Windows APIs to create restore point.", EventLogEntryType.Warning)

                Try
                    Return APIs.systemRestore.StartRestore(restorePointName, restorePointType, restorePointID)
                Catch ex6 As Exception
                    eventLogFunctions.writeToApplicationLogFile("Unable to create system restore point. System permissions seem to not allow it.", EventLogEntryType.Error)
                    eventLogFunctions.writeCrashToEventLog(ex6)
                    MsgBox("Unable to create system restore point. System permissions seem to not allow it.", MsgBoxStyle.Critical, "Error Creating System Restore Point")

                    Return APIs.errorCodes.ERROR_ACCESS_DENIED
                End Try
            Catch ex3 As Runtime.InteropServices.COMException
                eventLogFunctions.writeCrashToEventLog(ex3)
                eventLogFunctions.writeToApplicationLogFile("Falling back to core Windows APIs to create restore point.", EventLogEntryType.Warning)

                Try
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
                Dim boolResult As Boolean
                Dim strRestorePointName As String = getRestorePointName(point, boolResult)

                If boolResult Then
                    eventLogFunctions.writeToApplicationLogFile("Restoring system back to System Restore Point ID " & point & " (" & strRestorePointName & ").", EventLogEntryType.Information)

                    Dim managementScope As New Management.ManagementScope("\\localhost\root\default")
                    Dim managementPath As New Management.ManagementPath("SystemRestore")
                    Dim managementOptions As New Management.ObjectGetOptions()
                    Dim managementClass As New Management.ManagementClass(managementScope, managementPath, managementOptions)

                    Dim managementParameters As Management.ManagementBaseObject = managementClass.GetMethodParameters("Restore")
                    managementParameters("SequenceNumber") = point

                    Dim oOutParams As Management.ManagementBaseObject = managementClass.InvokeMethod("Restore", managementParameters, Nothing)

                    support.rebootSystem()
                Else
                    MsgBox("Unable to retrieve restore point name from system, System Restore Restoration Process halted.", MsgBoxStyle.Critical, "Restore Point Creator")
                End If
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
                Threading.Thread.CurrentThread.CurrentUICulture = New Globalization.CultureInfo("en-US")
                exceptionHandler.manuallyLoadCrashWindow(ex, ex.Message, ex.StackTrace, ex.GetType)
            End Try
        End Sub

        Private Function getRestorePointName(id As Long, ByRef boolResult As Boolean) As String
            Try
                Dim managementObjectSearcher As New Management.ManagementObjectSearcher("root\DEFAULT", "SELECT * FROM SystemRestore WHERE SequenceNumber = " & id.ToString)

                If managementObjectSearcher IsNot Nothing Then
                    Dim managementObjectCollection As Management.ManagementObjectCollection = managementObjectSearcher.Get()

                    If managementObjectCollection IsNot Nothing Then
                        If managementObjectCollection.Count <> 0 Then
                            boolResult = True
                            Return managementObjectCollection(0)("Description").ToString
                        Else
                            eventLogFunctions.writeToApplicationLogFile("Unable to find description for restore point ID " & id & ".", EventLogEntryType.Error)
                            boolResult = False
                            Return "ERROR_NO_DESCRIPTION"
                        End If
                    End If
                End If

                eventLogFunctions.writeToApplicationLogFile("Unable to find description for restore point ID " & id & ".", EventLogEntryType.Error)
                boolResult = False
                Return "ERROR_NO_DESCRIPTION"
            Catch ex As Exception
                eventLogFunctions.writeToApplicationLogFile("Unable to find description for restore point ID " & id & ".", EventLogEntryType.Error)
                boolResult = False
                Return "ERROR_NO_DESCRIPTION"
            End Try
        End Function

        Public Function getNewestSystemRestorePointID() As Integer
            Try
                ' Get all System Restore Points from the Windows Management System and puts then in the systemRestorePoints variable.
                Dim systemRestorePoints As New Management.ManagementObjectSearcher("root\DEFAULT", "SELECT * FROM SystemRestore")

                If systemRestorePoints IsNot Nothing Then
                    Dim managementObjectCollection As Management.ManagementObjectCollection = systemRestorePoints.Get()

                    If managementObjectCollection IsNot Nothing Then
                        ' Checks to see if there are any System Restore Points to be listed.
                        If managementObjectCollection.Count <> 0 Then
                            Return managementObjectCollection.Cast(Of Management.ManagementObject).ToList().Max(Function(managementObject As Management.ManagementObject) Integer.Parse(managementObject("SequenceNumber").ToString))
                        Else
                            Return 0
                        End If
                    Else
                        Return 0
                    End If
                Else
                    Return 0
                End If
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

        Public Function getSystemProcessor() As supportClasses.processorInfoClass
            Try
                Dim managementObjectSearcher As New Management.ManagementObjectSearcher("root\CIMV2", "Select * FROM Win32_Processor")

                If managementObjectSearcher IsNot Nothing Then
                    Dim managementObjectCollection As Management.ManagementObjectCollection = managementObjectSearcher.Get()

                    If managementObjectCollection IsNot Nothing Then
                        Dim rawProcessorName As String = managementObjectCollection(0)("Name").ToString.Trim.caseInsensitiveReplace("\((?:R|TM)\)", "", False)
                        Return createProcessorInfoClassObject(rawProcessorName, managementObjectCollection(0)("NumberOfCores").ToString)
                    Else
                        Return createProcessorInfoClassObject()
                    End If
                Else
                    Return createProcessorInfoClassObject()
                End If
            Catch ex As Exception
                Return createProcessorInfoClassObject()
            End Try
        End Function

        Private Function createProcessorInfoClassObject() As supportClasses.processorInfoClass
            Dim processorInfoClassObject As New supportClasses.processorInfoClass()
            processorInfoClassObject.strProcessor = "unknown"
            processorInfoClassObject.strNumberOfCores = "unknown"

            Return processorInfoClassObject
        End Function

        Private Function createProcessorInfoClassObject(strProcessorName As String, strNumberOfCores As String) As supportClasses.processorInfoClass
            Dim processorInfoClassObject As New supportClasses.processorInfoClass()
            processorInfoClassObject.strProcessor = strProcessorName
            processorInfoClassObject.strNumberOfCores = strNumberOfCores

            Return processorInfoClassObject
        End Function
    End Module
End Namespace