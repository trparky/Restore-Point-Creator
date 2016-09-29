Imports System.Text.RegularExpressions
Imports Microsoft.Win32

Namespace Functions.power
    Module power
        Private Function getActivePowerPlanGUID() As String
            Try
                Dim powerPlanSeacher As New Management.ManagementObjectSearcher("root\CIMV2\power", "SELECT * FROM Win32_PowerPlan WHERE IsActive = True")

                If powerPlanSeacher.Get().Count = 0 Then
                    eventLogFunctions.writeToSystemEventLog("WMI returned 0 results from Win32_PowerPlan.", EventLogEntryType.Error)
                    Return "INVALID"
                Else
                    Dim powerPlanDetails As Management.ManagementObject = powerPlanSeacher.Get()(0)
                    Dim powerGUIDFromWMI As String = powerPlanDetails("InstanceID")

                    If Regex.IsMatch(powerGUIDFromWMI, "(\{[0-9A-F]{8}-[0-9A-F]{4}-[0-9A-F]{4}-[0-9A-F]{4}-[0-9A-F]{12}\})", RegexOptions.IgnoreCase) Then
                        Return Regex.Match(powerGUIDFromWMI, "(\{[0-9A-F]{8}-[0-9A-F]{4}-[0-9A-F]{4}-[0-9A-F]{4}-[0-9A-F]{12}\})", RegexOptions.IgnoreCase).Groups(1).Value.Replace("{", "").Replace("}", "").Trim.ToLower
                    Else
                        eventLogFunctions.writeToSystemEventLog("Unable to parse out GUID from WMI output.", EventLogEntryType.Error)
                        Return "INVALID"
                    End If
                End If
            Catch ex As Exception
                Dim registryKey As RegistryKey = Registry.LocalMachine.OpenSubKey("SYSTEM\CurrentControlSet\Control\Power\User\PowerSchemes", False)

                If registryKey Is Nothing Then
                    Return executePowerCFGToGetActivePowerPlanGUID()
                Else
                    Dim powerGUID As String = registryKey.GetValue("ActivePowerScheme", globalVariables.invalidGUID)
                    registryKey.Close()
                    registryKey.Dispose()

                    If powerGUID = globalVariables.invalidGUID Then
                        Return executePowerCFGToGetActivePowerPlanGUID()
                    Else
                        Return powerGUID.Trim.ToLower
                    End If
                End If
            End Try
        End Function

        Private Function executePowerCFGToGetActivePowerPlanGUID() As String
            Dim commandLineOutput As String = Nothing
            Dim boolResult As Boolean = support.executeShellCommandAndGetOutput(commandLineOutput, "powercfg.exe", "/GETACTIVESCHEME")

            If boolResult = True Then
                commandLineOutput = commandLineOutput.Trim

                If Regex.IsMatch(commandLineOutput, "([0-9A-F]{8}-[0-9A-F]{4}-[0-9A-F]{4}-[0-9A-F]{4}-[0-9A-F]{12})", RegexOptions.IgnoreCase) Then
                    Return Regex.Match(commandLineOutput, "([0-9A-F]{8}-[0-9A-F]{4}-[0-9A-F]{4}-[0-9A-F]{4}-[0-9A-F]{12})", RegexOptions.IgnoreCase).Groups(1).Value.Trim.ToLower
                Else
                    eventLogFunctions.writeToSystemEventLog("Unable to parse out GUID from powercfg.exe output.", EventLogEntryType.Error)

                    Return "INVALID"
                End If
            Else
                Return "INVALID"
            End If
        End Function

        Private Function doWeHaveAValidActivePowerPlan(ByRef activePowerPlanGUID As String) As Boolean
            activePowerPlanGUID = getActivePowerPlanGUID()

            If activePowerPlanGUID = "INVALID" Then
                Return False
            Else
                Return True
            End If
        End Function

        Public Sub disablePowerPlanWakeFromSleep()
            Try
                Dim activePowerPlanGUID As String = Nothing
                Dim strPathToPowerCFG As String = IO.Path.Combine(globalVariables.strPathToSystemFolder, "powercfg.exe")

                If doWeHaveAValidActivePowerPlan(activePowerPlanGUID) = True Then
                    Dim registryKey As RegistryKey = Registry.LocalMachine.OpenSubKey("SYSTEM\CurrentControlSet\Control\Power\User\PowerSchemes\" & activePowerPlanGUID & "\238c9fa8-0aad-41ed-83f4-97be242c8f20\bd3b718a-0680-4d9d-8ab2-e1d2b4ac806d", False)

                    Dim boolDidWeChangeAnything As Boolean = False

                    If registryKey Is Nothing Then
                        boolDidWeChangeAnything = True

                        support.executeCommand(strPathToPowerCFG, "-SETACVALUEINDEX " & activePowerPlanGUID & " SUB_SLEEP bd3b718a-0680-4d9d-8ab2-e1d2b4ac806d 0")
                        support.executeCommand(strPathToPowerCFG, "-SETDCVALUEINDEX " & activePowerPlanGUID & " SUB_SLEEP bd3b718a-0680-4d9d-8ab2-e1d2b4ac806d 0")
                    Else
                        If Short.Parse(registryKey.GetValue("ACSettingIndex", "0").ToString) = 1 Then
                            boolDidWeChangeAnything = True
                            support.executeCommand(strPathToPowerCFG, "-SETACVALUEINDEX " & activePowerPlanGUID & " SUB_SLEEP bd3b718a-0680-4d9d-8ab2-e1d2b4ac806d 0")
                        End If

                        If Short.Parse(registryKey.GetValue("DCSettingIndex", "0").ToString) = 1 Then
                            boolDidWeChangeAnything = True
                            support.executeCommand(strPathToPowerCFG, "-SETDCVALUEINDEX " & activePowerPlanGUID & " SUB_SLEEP bd3b718a-0680-4d9d-8ab2-e1d2b4ac806d 0")
                        End If

                        registryKey.Close()
                        registryKey.Dispose()
                    End If

                    If boolDidWeChangeAnything = True Then
                        eventLogFunctions.writeToSystemEventLog("System Restore Point Creator has set your Windows Power Plan back to default settings for wake timers.", EventLogEntryType.Information)

                        MsgBox("System Restore Point Creator has set your Windows Power Plan back to default settings for wake timers. Your system should no longer wake up for scheduled tasks.", MsgBoxStyle.Information, "System Restore Point Creator")
                    End If
                Else
                    MsgBox("This system doesn't appear to have a valid active power plan in place.", MsgBoxStyle.Critical, "System Restore Point Creator")
                End If
            Catch ex As Exception
                eventLogFunctions.writeCrashToEventLog(ex)
            End Try
        End Sub

        Public Sub checkIfActivePowerPlanIsSetProperlyForWakingFromSleep(Optional boolShowNoChangesNeededMessage As Boolean = False)
            Try
                Dim activePowerPlanGUID As String = Nothing
                Dim strPathToPowerCFG As String = IO.Path.Combine(globalVariables.strPathToSystemFolder, "powercfg.exe")

                If doWeHaveAValidActivePowerPlan(activePowerPlanGUID) = True Then
                    Dim registryKey As RegistryKey = Registry.LocalMachine.OpenSubKey("SYSTEM\CurrentControlSet\Control\Power\User\PowerSchemes\" & activePowerPlanGUID & "\238c9fa8-0aad-41ed-83f4-97be242c8f20\bd3b718a-0680-4d9d-8ab2-e1d2b4ac806d", False)

                    Dim boolDidWeChangeAnything As Boolean = False

                    If registryKey Is Nothing Then
                        boolDidWeChangeAnything = True

                        support.executeCommand(strPathToPowerCFG, "-SETACVALUEINDEX " & activePowerPlanGUID & " SUB_SLEEP bd3b718a-0680-4d9d-8ab2-e1d2b4ac806d 1")
                        support.executeCommand(strPathToPowerCFG, "-SETDCVALUEINDEX " & activePowerPlanGUID & " SUB_SLEEP bd3b718a-0680-4d9d-8ab2-e1d2b4ac806d 1")
                    Else
                        If Short.Parse(registryKey.GetValue("ACSettingIndex", "0").ToString) <> 1 Then
                            boolDidWeChangeAnything = True
                            support.executeCommand(strPathToPowerCFG, "-SETACVALUEINDEX " & activePowerPlanGUID & " SUB_SLEEP bd3b718a-0680-4d9d-8ab2-e1d2b4ac806d 1")
                        End If

                        If Short.Parse(registryKey.GetValue("DCSettingIndex", "0").ToString) <> 1 Then
                            boolDidWeChangeAnything = True
                            support.executeCommand(strPathToPowerCFG, "-SETDCVALUEINDEX " & activePowerPlanGUID & " SUB_SLEEP bd3b718a-0680-4d9d-8ab2-e1d2b4ac806d 1")
                        End If

                        registryKey.Close()
                        registryKey.Dispose()
                    End If

                    If boolDidWeChangeAnything = True Then
                        eventLogFunctions.writeToSystemEventLog("System Restore Point Creator has set your Windows Power Plan up to properly support waking up from Sleep Mode.", EventLogEntryType.Information)

                        MsgBox("System Restore Point Creator has set your Windows Power Plan up to properly support waking up from Sleep Mode." & vbCrLf & vbCrLf & "NOTE!" & vbCrLf & "This does not guarantee that your system will wake from sleep, your system's hardware must be able to support this functionality. Support for this functionality depends upon your system's motherboard and system drivers.", MsgBoxStyle.Information, "System Restore Point Creator")
                    Else
                        If boolShowNoChangesNeededMessage = True Then MsgBox("No changes to power plan settings were required.", MsgBoxStyle.Information, "System Restore Point Creator")
                    End If
                Else
                    MsgBox("This system doesn't appear to have a valid active power plan in place.", MsgBoxStyle.Critical, "System Restore Point Creator")
                End If
            Catch ex As Exception
                eventLogFunctions.writeCrashToEventLog(ex)
            End Try
        End Sub
    End Module
End Namespace