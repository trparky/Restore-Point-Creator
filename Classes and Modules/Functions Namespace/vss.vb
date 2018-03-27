Namespace Functions.vss
    Module vss
        Public Sub checkVSSServiceStatus()
            Try
                Dim strResult As String = wmi.getServiceStartType("VSS")

                If String.IsNullOrEmpty(strResult) Then
                    eventLogFunctions.writeToApplicationLogFile("The WMI system returned an invalid response for the Startup type parameter.", EventLogEntryType.Error, False)
                Else
                    eventLogFunctions.writeToApplicationLogFile(String.Format("VSS System Service Startup Type is set to {0}{1}{0}.", Chr(34), strResult), EventLogEntryType.Information, False)

                    If Not strResult.Equals("manual", StringComparison.OrdinalIgnoreCase) Then
                        eventLogFunctions.writeToApplicationLogFile("The VSS System Service Startup Type was detected to be not setup correctly, this has been corrected.", EventLogEntryType.Warning, False)
                        wmi.setServiceStartMode("VSS")
                    End If

                    eventLogFunctions.writeToApplicationLogFile("Attempting to start the VSS System Service.", EventLogEntryType.Information, False)

                    Try
                        Using systemServiceController As New ServiceProcess.ServiceController("VSS")
                            systemServiceController.Start()
                            eventLogFunctions.writeToApplicationLogFile("The VSS System Service has been successfully started.", EventLogEntryType.Information, False)
                        End Using
                    Catch ex As Exception
                        eventLogFunctions.writeToApplicationLogFile("The VSS System Service has failed to start.", EventLogEntryType.Error, False)
                        eventLogFunctions.writeCrashToApplicationLogFile(ex, EventLogEntryType.Error)
                    End Try
                End If
            Catch ex As Exception
                eventLogFunctions.writeToApplicationLogFile("Unable to find the VSS System Service.", EventLogEntryType.Error, False)
            End Try
        End Sub

        ''' <summary>Gets the Shadow Storage Information for a particular storage device in the system.</summary>
        ''' <param name="volumeID">This input can accept either a storage device GUID or a drive letter such as "C:".</param>
        ''' <returns>A ShadowStorageData Class instance.</returns>
        Public Function getData(ByVal volumeID As String, ByRef boolResult As Boolean) As supportClasses.ShadowStorageData ' Accepts both a Volume ID and a drive letter such as "C:".
            Dim boolDeviceIDRetrievalResult As Boolean = True

            If volumeID.Length = 2 Then volumeID = wmi.getDeviceIDFromDriveLetter(volumeID, boolDeviceIDRetrievalResult)

            Try
                If boolDeviceIDRetrievalResult Then
                    Dim wmiQuery As String = String.Format("SELECT * FROM Win32_ShadowStorage WHERE Volume='Win32_Volume.DeviceID=\'{0}\''", volumeID.addSlashes())

                    Using searcher As New Management.ManagementObjectSearcher("root\CIMV2", wmiQuery)
                        If searcher.Get.Count <> 0 Then
                            Using queryObj As Management.ManagementObject = searcher.Get(0)
                                Dim shadowStorageDataClassInstance As New supportClasses.ShadowStorageData

                                ' This is all in an effort to try and prevent Null Reference Exceptions.
                                If queryObj("AllocatedSpace") Is Nothing Then
                                    shadowStorageDataClassInstance.AllocatedSpace = 0
                                Else
                                    If Long.TryParse(queryObj("AllocatedSpace").ToString, shadowStorageDataClassInstance.AllocatedSpace) Then
                                        shadowStorageDataClassInstance.AllocatedSpaceHuman = support.bytesToHumanSize(shadowStorageDataClassInstance.AllocatedSpace)
                                    Else
                                        shadowStorageDataClassInstance.AllocatedSpace = 0
                                        shadowStorageDataClassInstance.AllocatedSpaceHuman = "0 Bytes"
                                    End If
                                End If

                                ' This is all in an effort to try and prevent Null Reference Exceptions.
                                If queryObj("MaxSpace") Is Nothing Then
                                    shadowStorageDataClassInstance.MaxSpace = 0
                                Else
                                    If Long.TryParse(queryObj("MaxSpace").ToString, shadowStorageDataClassInstance.MaxSpace) Then
                                        shadowStorageDataClassInstance.MaxSpaceHuman = support.bytesToHumanSize(shadowStorageDataClassInstance.MaxSpace)
                                    Else
                                        shadowStorageDataClassInstance.MaxSpace = 0
                                        shadowStorageDataClassInstance.MaxSpaceHuman = "0 Bytes"
                                    End If
                                End If

                                ' This is all in an effort to try and prevent Null Reference Exceptions.
                                If queryObj("UsedSpace") Is Nothing Then : shadowStorageDataClassInstance.UsedSpace = 0
                                Else
                                    If Long.TryParse(queryObj("UsedSpace").ToString, shadowStorageDataClassInstance.UsedSpace) Then
                                        shadowStorageDataClassInstance.UsedSpaceHuman = support.bytesToHumanSize(shadowStorageDataClassInstance.UsedSpace)
                                    Else
                                        shadowStorageDataClassInstance.UsedSpace = 0
                                        shadowStorageDataClassInstance.UsedSpaceHuman = "0 Bytes"
                                    End If
                                End If

                                ' This is all in an effort to try and prevent Null Reference Exceptions.
                                shadowStorageDataClassInstance.DiffVolume = queryObj("DiffVolume")?.ToString
                                shadowStorageDataClassInstance.Volume = queryObj("Volume")?.ToString

                                boolResult = True
                                Return shadowStorageDataClassInstance
                            End Using
                        End If
                    End Using

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
        ''' <returns>A 64-bit Signed (Long) Integer.</returns>
        Public Function getMaxSize(ByVal driveLetter As String) As Long
            Dim boolResult As Boolean
            Dim vssData As supportClasses.ShadowStorageData = getData(driveLetter, boolResult)
            Return If(boolResult, vssData.MaxSpace, 0)
        End Function

        Private Sub giveMessageAboutShadowCopyServiceBeingBroken()
            MsgBox("Shadow Copy Service Provider Error Detected!" & vbCrLf & vbCrLf & "Please reference the following Microsoft Support Knowledge Base Article..." & vbCrLf & globalVariables.webURLs.webPages.strProviderFailureMicrosoftKB & vbCrLf & vbCrLf & "Your default web browser will now open to load the Microsoft Knowledgebase Article mentioned above.", MsgBoxStyle.Critical, "Restore Point Creator")

            support.launchURLInWebBrowser(globalVariables.webURLs.webPages.strProviderFailureMicrosoftKB)
        End Sub

        Public Sub executeVSSAdminCommand(driveLetter As String)
            Try
                If IO.File.Exists(IO.Path.Combine(globalVariables.strPathToSystemFolder, "vssadmin.exe")) Then
                    Dim startInfo As New ProcessStartInfo
                    startInfo.FileName = IO.Path.Combine(globalVariables.strPathToSystemFolder, "vssadmin.exe")
                    startInfo.Arguments = String.Format("Resize ShadowStorage /For={0} /On={0} /MaxSize=20%", driveLetter)
                    startInfo.Verb = "runas"
                    startInfo.UseShellExecute = False
                    startInfo.CreateNoWindow = True

                    Dim runningProcess As Process = Process.Start(startInfo)
                    runningProcess.WaitForExit()

                    eventLogFunctions.writeToApplicationLogFile("No system restore point storage space was assigned for the system drive, this issue has been corrected.", EventLogEntryType.Information, False)
                Else
                    MsgBox("Unable to find the VSSAdmin utility.", MsgBoxStyle.Critical, "Restore Point Creator")
                End If
            Catch ex2 As ComponentModel.Win32Exception
                eventLogFunctions.writeToApplicationLogFile("An issue came up while attempting to run the vssadmin.exe command, perhaps due to Group Policy Restrictions. Falling back to WMI mode.", EventLogEntryType.Error, False)

                Dim longDriveCapacity As Long = wmi.getDriveSize(driveLetter)
                Dim newSize As Long = longDriveCapacity * 0.2

                setShadowStorageSize(driveLetter, newSize)
            Catch ex As Exception
                eventLogFunctions.writeCrashToApplicationLogFile(ex)
            End Try
        End Sub

        Public Sub checkSystemDrivesForFullShadowStorage(parentForm As Form1)
            Try
                Dim boolGetVSSDataResult As Boolean
                Dim shadowStorageStatistics As supportClasses.ShadowStorageData = getData(globalVariables.systemDriveLetter, boolGetVSSDataResult)
                Dim shadowStorageUsePercentage As Double

                If shadowStorageStatistics IsNot Nothing And boolGetVSSDataResult Then
                    If Not shadowStorageStatistics.UsedSpace.Equals(Nothing) And Not shadowStorageStatistics.MaxSpace.Equals(Nothing) Then
                        shadowStorageUsePercentage = support.calculatePercentageValue(shadowStorageStatistics.UsedSpace, shadowStorageStatistics.MaxSpace)

                        If shadowStorageUsePercentage > globalVariables.warningPercentage Then
                            parentForm.Invoke(Sub()
                                                  Dim fullSystemDriveDialog As New Reserved_Space_for_System_Drive()
                                                  fullSystemDriveDialog.StartPosition = FormStartPosition.CenterParent
                                                  fullSystemDriveDialog.ShowDialog()
                                              End Sub)
                        End If
                    End If

                    shadowStorageStatistics = Nothing
                End If
            Catch ex As Exception
                ' We don't care if we crash here.
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

                Using managementClassObject As New Management.ManagementClass(managementScopeObject, managementPathObject, managementObjectOptions)
                    Using managementBaseObjectParameters As Management.ManagementBaseObject = managementClassObject.GetMethodParameters("Enable")
                        managementBaseObjectParameters("Drive") = driveLetter
                        managementBaseObjectParameters("WaitTillEnabled") = True

                        Dim oOutParams As Management.ManagementBaseObject = managementClassObject.InvokeMethod("Enable", managementBaseObjectParameters, Nothing)

                        eventLogFunctions.writeToApplicationLogFile("Enabled System Restore on drive " & driveLetter.Substring(0, 1).ToUpper & ".", EventLogEntryType.Information, False)

                        Return oOutParams("ReturnValue")
                    End Using
                End Using
            Catch ex3 As Runtime.InteropServices.COMException
                wmi.giveComExceptionCrashMessage()
                Return 1
            Catch ex As Exception
                Return 1
            End Try
        End Function

        ' driveLetter is just that, "C:"
        Public Sub setShadowStorageSize(driveLetter As String, size As Long)
            eventLogFunctions.writeToApplicationLogFile(String.Format("Setting reserved system restore space for drive {0} to {1}.", driveLetter, support.bytesToHumanSize(size)), EventLogEntryType.Information, False)

            Try
                driveLetter = driveLetter.ToUpper

                Dim boolResult As Boolean
                Dim volumeID As String = wmi.getDeviceIDFromDriveLetter(driveLetter, boolResult)

                If boolResult Then
                    Dim wmiQuery As String = String.Format("SELECT * FROM Win32_ShadowStorage WHERE Volume='Win32_Volume.DeviceID=\'{0}\''", volumeID.addSlashes())

                    Using searcher As New Management.ManagementObjectSearcher("root\CIMV2", wmiQuery)
                        If searcher.Get.Count <> 0 Then
                            Using queryObj As Management.ManagementObject = searcher.Get(0)
                                queryObj("MaxSpace") = size
                                queryObj.Put()
                            End Using
                        End If
                    End Using
                End If
            Catch ex2 As Management.ManagementException
                eventLogFunctions.writeCrashToApplicationLogFile(ex2)

                Dim msgBoxResult As MsgBoxResult = MsgBox("There was an error attempting to apply your drive space settings. Would you like to try again?", MsgBoxStyle.Question + MsgBoxStyle.YesNo, "Restore Point Creator")

                If msgBoxResult = MsgBoxResult.Yes Then
                    executeVSSAdminCommand(driveLetter)
                    setShadowStorageSize(driveLetter, size)
                End If
            Catch ex As Exception
                If ex.Message.caseInsensitiveContains("provider failure") Then
                    eventLogFunctions.writeCrashToApplicationLogFile(ex)
                    giveMessageAboutShadowCopyServiceBeingBroken()
                Else
                    Threading.Thread.CurrentThread.CurrentUICulture = New Globalization.CultureInfo("en-US")
                    exceptionHandler.manuallyLoadCrashWindow(ex, ex.Message, ex.StackTrace, ex.GetType)
                End If
            End Try
        End Sub
    End Module
End Namespace