Imports Microsoft.Win32

Namespace Functions
    ''' <summary>This class manages the pending operations at system reboot.</summary>
    Public Class deleteAtReboot
        Private pendingOperations As List(Of deleteAtRebootClass)
        Private boolThingsChanged As Boolean = False

        ''' <summary>This class manages the pending operations at system reboot.</summary>
        Public Sub New()
            pendingOperations = loadStagedPendingOperations()
        End Sub

        ''' <summary>This saves any and all changes to the system Registry.</summary>
        Public Sub save()
            If boolThingsChanged Then
                saveStagedPendingOperations()
            End If
        End Sub

        ''' <summary>This closes out this class instance.</summary>
        ''' <param name="boolSaveChanges">This is an optional parameter. Set to True if you want to save any and all changes back to the system Registry.</param>
        Public Sub dispose(Optional boolSaveChanges As Boolean = False)
            If boolSaveChanges And boolThingsChanged Then
                saveStagedPendingOperations()
            End If

            pendingOperations.Clear()
        End Sub

        ''' <summary>This function removes an item from the list of pending file operations that are to occur at the next system reboot.</summary>
        ''' <param name="strFileToBeRemoved">The name of the file that's in the operations queue that needs to be removed.</param>
        ''' <param name="boolExactMatch">This is an optional parameter. If True then the function will do an exact match check, if False the function will simply remove an item from the operations queue if the file to be worked on contains the value of the strFileToBeRemoved input parameter.</param>
        Public Sub removeItem(strFileToBeRemoved As String, Optional boolExactMatch As Boolean = False)
            If pendingOperations.Count <> 0 Then
                For Each item As deleteAtRebootClass In pendingOperations
                    If boolExactMatch Then
                        If item.strFileToBeWorkedOn.Equals(strFileToBeRemoved, StringComparison.OrdinalIgnoreCase) Then
                            pendingOperations.Remove(item)
                            boolThingsChanged = True
                            Exit For ' Now that we removed the item we have to escape out of the loop.
                        End If
                    Else
                        If item.strFileToBeWorkedOn.caseInsensitiveContains(strFileToBeRemoved) Then
                            pendingOperations.Remove(item)
                            boolThingsChanged = True
                            Exit For ' Now that we removed the item we have to escape out of the loop.
                        End If
                    End If
                Next
            End If
        End Sub

        ''' <summary>This adds an item to be deleted to the list of operations.</summary>
        ''' <param name="strFileToBeDeleted">The file to be deleted.</param>
        Public Sub addItem(strFileToBeDeleted As String)
            pendingOperations.Add(New deleteAtRebootClass(strFileToBeDeleted))
            boolThingsChanged = True
        End Sub

        ''' <summary>This adds an item to be renamed to the list of operations.</summary>
        ''' <param name="strFileToBeRenamed">The name of the file to be renamed.</param>
        ''' <param name="strNewName">The new name of the file.</param>
        Public Sub addItem(strFileToBeRenamed As String, strNewName As String)
            pendingOperations.Add(New deleteAtRebootClass(strFileToBeRenamed, strNewName))
            boolThingsChanged = True
        End Sub

        ''' <summary>Returns the list of pending operations.</summary>
        ''' <returns>A list of pending operations.</returns>
        Public ReadOnly Property currentPendingOperations As List(Of deleteAtRebootClass)
            Get
                Return pendingOperations
            End Get
        End Property

        ''' <summary>This function loads the list of pending operations at system reboot.</summary>
        ''' <returns>A list of pending operations.</returns>
        Private Function loadStagedPendingOperations() As List(Of deleteAtRebootClass)
            Dim _pendingOperations As New List(Of deleteAtRebootClass)

            Try
                Dim registryKey As RegistryKey = Registry.LocalMachine.OpenSubKey("SYSTEM\CurrentControlSet\Control\Session Manager", False)
                Dim pendingOperations As String() = registryKey.GetValue("PendingFileRenameOperations")
                registryKey.Close()

                Dim strFileToBeWorkedOn, strFileToBeRenamedTo As String

                If pendingOperations IsNot Nothing Then
                    For i = 0 To pendingOperations.Count - 1 Step 2
                        strFileToBeWorkedOn = pendingOperations(i).Replace("\??\", "")
                        strFileToBeRenamedTo = pendingOperations(i + 1).Replace("\??\", "")

                        If pendingOperations(i + 1).Trim = "" Then
                            _pendingOperations.Add(New deleteAtRebootClass(strFileToBeWorkedOn))
                        Else
                            _pendingOperations.Add(New deleteAtRebootClass(strFileToBeWorkedOn, strFileToBeRenamedTo))
                        End If
                    Next
                End If

                Return _pendingOperations
            Catch ex As Exception
                Return _pendingOperations
            End Try
        End Function

        ''' <summary>This function saves any and all pending operations within this class instance back to the system Registry.</summary>
        Private Sub saveStagedPendingOperations()
            Dim registryKey As RegistryKey = Registry.LocalMachine.OpenSubKey("SYSTEM\CurrentControlSet\Control\Session Manager", True)

            If pendingOperations.Count = 0 Then
                registryKey.DeleteValue("PendingFileRenameOperations")
            Else
                Dim itemsToBeSavedToTheRegistry As New Specialized.StringCollection()

                For Each pendingOperation As deleteAtRebootClass In pendingOperations
                    If pendingOperation.boolDelete Then
                        itemsToBeSavedToTheRegistry.Add("\??\" & pendingOperation.strFileToBeWorkedOn)
                        itemsToBeSavedToTheRegistry.Add("")
                    Else
                        itemsToBeSavedToTheRegistry.Add("\??\" & pendingOperation.strFileToBeWorkedOn)
                        itemsToBeSavedToTheRegistry.Add("\??\" & pendingOperation.strToBeRenamedTo)
                    End If
                Next

                Dim valuesToBeSavedToTheRegistry(itemsToBeSavedToTheRegistry.Count - 1) As String
                itemsToBeSavedToTheRegistry.CopyTo(valuesToBeSavedToTheRegistry, 0)

                registryKey.SetValue("PendingFileRenameOperations", valuesToBeSavedToTheRegistry, RegistryValueKind.MultiString)
            End If

            registryKey.Close()
        End Sub
    End Class

    Public Class deleteAtRebootClass
        Private _strFileToBeWorkedOn, _strToBeRenamedTo As String
        Private _boolDelete As Boolean

        Public Sub New()
        End Sub

        ''' <summary>This adds an item to be renamed.</summary>
        ''' <param name="strFileToBeWorkedOn">The file to be renamed.</param>
        ''' <param name="strToBeRenamedTo">The new name of the file to be renamed.</param>
        Public Sub New(strFileToBeWorkedOn As String, strToBeRenamedTo As String)
            _strFileToBeWorkedOn = strFileToBeWorkedOn
            _strToBeRenamedTo = strToBeRenamedTo
            _boolDelete = False
        End Sub

        ''' <summary>This adds an item to be deleted.</summary>
        ''' <param name="strFileToDelete">The file to be deleted from the pending operations.</param>
        Public Sub New(strFileToDelete As String)
            _strFileToBeWorkedOn = strFileToDelete
            _strToBeRenamedTo = Nothing
            _boolDelete = True
        End Sub

        Public Property boolDelete As Boolean
            Get
                Return _boolDelete
            End Get
            Set(value As Boolean)
                _boolDelete = value
            End Set
        End Property

        Public Property strFileToBeWorkedOn As String
            Get
                Return _strFileToBeWorkedOn
            End Get
            Set(value As String)
                _strFileToBeWorkedOn = value
            End Set
        End Property

        Public Property strToBeRenamedTo As String
            Get
                Return _strToBeRenamedTo
            End Get
            Set(value As String)
                _strToBeRenamedTo = value
            End Set
        End Property
    End Class
End Namespace