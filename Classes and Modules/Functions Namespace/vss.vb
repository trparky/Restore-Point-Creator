Namespace Functions.vss
    Module vss
        ''' <summary>Gets the Shadow Storage Information for a particular storage device in the system.</summary>
        ''' <param name="volumeID">This input can accept either a storage device GUID or a drive letter such as "C:".</param>
        ''' <returns>A ShadowStorageData Class instance.</returns>
        Public Function getData(ByVal volumeID As String, ByRef boolResult As Boolean) As supportClasses.ShadowStorageData ' Accepts both a Volume ID and a drive letter such as "C:".
            Dim boolDeviceIDRetrievalResult As Boolean = True

            If volumeID.Length = 2 Then
                volumeID = wmi.getDeviceIDFromDriveLetter(volumeID, boolDeviceIDRetrievalResult)
            End If

            Try
                If boolDeviceIDRetrievalResult = True Then
                    Dim wmiQuery As String = String.Format("SELECT * FROM Win32_ShadowStorage WHERE Volume='Win32_Volume.DeviceID=\'{0}\''", volumeID.addSlashes())
                    Dim searcher As New Management.ManagementObjectSearcher("root\CIMV2", wmiQuery)

                    If searcher.Get.Count <> 0 Then
                        Dim queryObj As Management.ManagementObject = searcher.Get(0)
                        Dim shadowStorageDataClassInstance As New supportClasses.ShadowStorageData

                        ' This is all in an effort to try and prevent Null Reference Exceptions.
                        If queryObj("AllocatedSpace") Is Nothing Then
                            shadowStorageDataClassInstance.AllocatedSpace = 0
                        Else
                            If ULong.TryParse(queryObj("AllocatedSpace"), shadowStorageDataClassInstance.AllocatedSpace) = False Then
                                shadowStorageDataClassInstance.AllocatedSpace = 0
                            End If
                        End If

                        ' This is all in an effort to try and prevent Null Reference Exceptions.
                        If queryObj("MaxSpace") Is Nothing Then
                            shadowStorageDataClassInstance.MaxSpace = 0
                        Else
                            If ULong.TryParse(queryObj("MaxSpace"), shadowStorageDataClassInstance.MaxSpace) = False Then
                                shadowStorageDataClassInstance.MaxSpace = 0
                            End If
                        End If

                        ' This is all in an effort to try and prevent Null Reference Exceptions.
                        If queryObj("UsedSpace") Is Nothing Then
                            shadowStorageDataClassInstance.UsedSpace = 0
                        Else
                            If ULong.TryParse(queryObj("UsedSpace"), shadowStorageDataClassInstance.UsedSpace) = False Then
                                shadowStorageDataClassInstance.UsedSpace = 0
                            End If
                        End If

                        ' This is all in an effort to try and prevent Null Reference Exceptions.
                        If queryObj("DiffVolume") Is Nothing Then
                            shadowStorageDataClassInstance.DiffVolume = Nothing
                        Else
                            shadowStorageDataClassInstance.DiffVolume = queryObj("DiffVolume")
                        End If

                        ' This is all in an effort to try and prevent Null Reference Exceptions.
                        If queryObj("Volume") Is Nothing Then
                            shadowStorageDataClassInstance.Volume = Nothing
                        Else
                            shadowStorageDataClassInstance.Volume = queryObj("Volume")
                        End If

                        searcher.Dispose()
                        queryObj.Dispose()
                        searcher = Nothing
                        queryObj = Nothing

                        boolResult = True
                        Return shadowStorageDataClassInstance
                    End If

                    searcher.Dispose()
                    searcher = Nothing

                    boolResult = False
                    Return Nothing
                Else
                    boolResult = False
                    Return Nothing
                End If
            Catch ex As Exception
                boolResult = False
                Return Nothing
            End Try
        End Function

        ''' <summary>Gets the Shadow Storage Information for a particular storage device in the system.</summary>
        ''' <param name="driveLetter">This input can accept either a storage device GUID or a drive letter such as "C:".</param>
        ''' <returns>A 64-bit Unsigned (ULong) Integer.</returns>
        Public Function getMaxSize(ByVal driveLetter As String) As ULong
            Dim boolResult As Boolean
            Dim vssData As supportClasses.ShadowStorageData = getData(driveLetter, boolResult)

            If boolResult = True Then
                Return vssData.MaxSpace
            Else
                Return 0
            End If
        End Function

        Private Sub giveMessageAboutShadowCopyServiceBeingBroken()
            MsgBox("Shadow Copy Service Provider Error Detected!" & vbCrLf & vbCrLf & "Please reference the following Microsoft Support Knowledge Base Article..." & vbCrLf & globalVariables.webURLs.webPages.strProviderFailureMicrosoftKB & vbCrLf & vbCrLf & "Your default web browser will now open to load the Microsoft Knowledgebase Article mentioned above.", MsgBoxStyle.Critical, "Restore Point Creator")

            support.launchURLInWebBrowser(globalVariables.webURLs.webPages.strProviderFailureMicrosoftKB)
        End Sub

        Public Sub executeVSSAdminCommand(driveLetter As String)
            Try
                If IO.File.Exists(IO.Path.Combine(globalVariables.strPathToSystemFolder, "vssadmin.exe")) = True Then
                    Dim startInfo As New ProcessStartInfo
                    startInfo.FileName = IO.Path.Combine(globalVariables.strPathToSystemFolder, "vssadmin.exe")
                    startInfo.Arguments = String.Format("Resize ShadowStorage /For={0} /On={0} /MaxSize=20%", driveLetter)
                    startInfo.Verb = "runas"
                    startInfo.UseShellExecute = False
                    startInfo.CreateNoWindow = True

                    Dim runningProcess As Process = Process.Start(startInfo)
                    runningProcess.WaitForExit()

                    eventLogFunctions.writeToSystemEventLog("No system restore point storage space was assigned for the system drive, this issue has been corrected.", EventLogEntryType.Information)
                Else
                    MsgBox("Unable to find the VSSAdmin utility.", MsgBoxStyle.Critical, "Restore Point Creator")
                End If
            Catch ex2 As ComponentModel.Win32Exception
                eventLogFunctions.writeToSystemEventLog("An issue came up while attempting to run the vssadmin.exe command, perhaps due to Group Policy Restrictions. Falling back to WMI mode.", EventLogEntryType.Error)

                Dim uLongDriveCapacity As ULong = wmi.getDriveSize(driveLetter)
                Dim newSize As Long = uLongDriveCapacity * 0.2

                setShadowStorageSize(driveLetter, newSize)
            Catch ex As Exception
                eventLogFunctions.writeCrashToEventLog(ex)
            End Try
        End Sub

        Public Sub checkSystemDrivesForFullShadowStorage(parentForm As Form)
            Try
                Dim boolGetVSSDataResult As Boolean
                Dim shadowStorageStatistics As supportClasses.ShadowStorageData = getData(globalVariables.systemDriveLetter, boolGetVSSDataResult)
                Dim shadowStorageUsePercentage As Double

                If shadowStorageStatistics IsNot Nothing And boolGetVSSDataResult = True Then
                    If (shadowStorageStatistics.UsedSpace = Nothing) = False And (shadowStorageStatistics.MaxSpace = Nothing) = False Then
                        shadowStorageUsePercentage = support.calculatePercentageValue(shadowStorageStatistics.UsedSpace, shadowStorageStatistics.MaxSpace)

                        If shadowStorageUsePercentage > globalVariables.warningPercentage Then
                            Dim fullSystemDriveDialog As New Reserved_Space_for_System_Drive()
                            fullSystemDriveDialog.StartPosition = FormStartPosition.CenterParent
                            fullSystemDriveDialog.ShowDialog(parentForm)
                        End If
                    End If

                    shadowStorageStatistics = Nothing
                End If
            Catch ex As Exception
                ' We don't care if we crash here.
            End Try
        End Sub

        Public Sub checkForAndEnableSystemRestoreIfNeeded()
            Try
                If wmi.checkToSeeIfSystemRestoreIsEnabledOnSystemDrive() = False Then
                    eventLogFunctions.writeToSystemEventLog("System Restore appears to not be enabled on this system.", EventLogEntryType.Information)

                    Dim boolGetVSSDataResult As Boolean
                    Dim shadowStorageDetails As supportClasses.ShadowStorageData = getData(globalVariables.systemDriveLetter, boolGetVSSDataResult)

                    If boolGetVSSDataResult = True Then
                    	eventLogFunctions.writeToSystemEventLog(String.Format("The old max space assigned for System Restore Points was {0}.", support.bytesToHumanSize(shadowStorageDetails.MaxSpace)), EventLogEntryType.Information)

                        executeVSSAdminCommand(globalVariables.systemDriveLetter)
                        enableSystemRestoreOnDriveWMI(globalVariables.systemDriveLetter)
                    End If
                End If
            Catch ex As Threading.ThreadAbortException
                ' Does nothing
            Catch ex As Exception
                eventLogFunctions.writeCrashToEventLog(ex)
                eventLogFunctions.writeToSystemEventLog("System Restore appears to not be enabled on this system.", EventLogEntryType.Information)

                executeVSSAdminCommand(globalVariables.systemDriveLetter)
                enableSystemRestoreOnDriveWMI(globalVariables.systemDriveLetter)
            End Try
        End Sub

        ''' <summary>Activates System Restore on a given system drive.</summary>
        ''' <param name="driveLetter">The system drive letter you want to enable System Restore on.</param>
        ''' <example>functions.enableSystemRestoreOnDriveWMI("C:")</example>
        Public Function enableSystemRestoreOnDriveWMI(driveLetter As String) As Short
            Try
                Dim managementScopeObject As New Management.ManagementScope("\\localhost\root\default")
                Dim managementPathObject As New Management.ManagementPath("SystemRestore")
                Dim managementObjectOptions As New Management.ObjectGetOptions()
                Dim managementClassObject As New Management.ManagementClass(managementScopeObject, managementPathObject, managementObjectOptions)

                Dim managementBaseObjectParameters As Management.ManagementBaseObject = managementClassObject.GetMethodParameters("Enable")
                managementBaseObjectParameters("Drive") = driveLetter
                managementBaseObjectParameters("WaitTillEnabled") = True

                Dim oOutParams As Management.ManagementBaseObject = managementClassObject.InvokeMethod("Enable", managementBaseObjectParameters, Nothing)

                eventLogFunctions.writeToSystemEventLog("Enabled System Restore on drive " & driveLetter.Substring(0, 1).ToUpper & ".", EventLogEntryType.Information)

                Return oOutParams("ReturnValue")
            Catch ex3 As Runtime.InteropServices.COMException
                wmi.giveComExceptionCrashMessage()
                Return 1
            Catch ex As Exception
                Return 1
            End Try
        End Function

        ' driveLetter is just that, "C:"
        Public Sub setShadowStorageSize(driveLetter As String, size As Long)
            Try
                driveLetter = driveLetter.ToUpper

                Dim boolResult As Boolean
                Dim volumeID As String = wmi.getDeviceIDFromDriveLetter(driveLetter, boolResult)

                If boolResult = True Then
                    Dim wmiQuery As String = String.Format("SELECT * FROM Win32_ShadowStorage WHERE Volume='Win32_Volume.DeviceID=\'{0}\''", volumeID.addSlashes())
                    Dim searcher As New Management.ManagementObjectSearcher("root\CIMV2", wmiQuery)

                    If searcher.Get.Count <> 0 Then
                        Dim queryObj As Management.ManagementObject = searcher.Get(0)

                        queryObj("MaxSpace") = size
                        queryObj.Put()
                        queryObj.Dispose()
                        queryObj = Nothing
                    End If

                    searcher.Dispose()
                    searcher = Nothing
                End If
            Catch ex2 As Management.ManagementException
                eventLogFunctions.writeCrashToEventLog(ex2)

                Dim msgBoxResult As MsgBoxResult = MsgBox("There was an error attempting to apply your drive space settings. Would you like to try again?", MsgBoxStyle.Question + MsgBoxStyle.YesNo, "Restore Point Creator")

                If msgBoxResult = MsgBoxResult.Yes Then
                    executeVSSAdminCommand(driveLetter)
                    setShadowStorageSize(driveLetter, size)
                End If
            Catch ex As Exception
                If ex.Message.caseInsensitiveContains("provider failure") = True Then
                    eventLogFunctions.writeCrashToEventLog(ex)
                    giveMessageAboutShadowCopyServiceBeingBroken()
                Else
                    Threading.Thread.CurrentThread.CurrentUICulture = New Globalization.CultureInfo("en-US")
                    exceptionHandler.manuallyLoadCrashWindow(ex, ex.Message, ex.StackTrace, ex.GetType)
                End If
            End Try
        End Sub
    End Module
End Namespace