Namespace Functions.wait
    Module wait
        Private Function loadPleaseWaitIcon() As Icon
            Dim bitMap As New Bitmap(My.Resources.chronometer)
            Dim icon As Icon = Icon.FromHandle(bitMap.GetHicon)
            bitMap.Dispose()
            Return icon
        End Function

        Public Sub openPleaseWaitWindow(Optional ByRef parentForm As Form = Nothing)
            Try
                If globalVariables.windows.frmPleaseWait IsNot Nothing Then
                    If parentForm Is Nothing Then
                        If globalVariables.windows.frmPleaseWait IsNot Nothing Then globalVariables.windows.frmPleaseWait.ShowDialog()
                    Else
                        If globalVariables.windows.frmPleaseWait IsNot Nothing Then
                            Dim _parentForm As Form = parentForm
                            parentForm.Invoke(Sub() globalVariables.windows.frmPleaseWait.ShowDialog(_parentForm))
                        End If
                    End If
                End If
            Catch ex As Exception
            End Try
        End Sub

        ''' <summary>Creates a Please Wait window.</summary>
        ''' <param name="message">Sets the message for the Please Wait window.</param>
        ''' <param name="howToCenter">Tells the window how to center itself when it appears.</param>
        ''' <param name="openDialogInNewThread">This is an optional setting, normally it's False. But if set to True, the function will show the newly created Please Wait window in a new thread. Normally, this function doesn't do that.</param>
        Public Sub createPleaseWaitWindow(message As String, boolShowOnTaskbar As Boolean, howToCenter As enums.howToCenterWindow, openDialogInNewThread As Boolean, Optional boolSystemModal As Boolean = False)
            Try
                globalVariables.windows.frmPleaseWait = New Please_Wait
                globalVariables.windows.frmPleaseWait.StartPosition = FormStartPosition.CenterParent
                globalVariables.windows.frmPleaseWait.pleaseWaitlblLabel.Text = message
                globalVariables.windows.frmPleaseWait.lblLabelText = message
                globalVariables.windows.frmPleaseWait.Icon = loadPleaseWaitIcon()
                globalVariables.windows.frmPleaseWait.TopMost = True
                globalVariables.windows.frmPleaseWait.howToCenter = howToCenter
                globalVariables.windows.frmPleaseWait.ShowInTaskbar = boolShowOnTaskbar
                globalVariables.windows.frmPleaseWait.systemModal = boolSystemModal

                If openDialogInNewThread = True Then Threading.ThreadPool.QueueUserWorkItem(Sub() openPleaseWaitWindow())
            Catch ex As Exception
            End Try
        End Sub

        Public Sub closePleaseWaitWindow()
            Try
                If globalVariables.windows.frmPleaseWait IsNot Nothing Then
                    globalVariables.windows.frmPleaseWait.allowClose = True
                    globalVariables.windows.frmPleaseWait.Close()
                    globalVariables.windows.frmPleaseWait.Dispose()
                    globalVariables.windows.frmPleaseWait = Nothing

                    If globalVariables.pleaseWaitWindowThread IsNot Nothing Then
                        globalVariables.pleaseWaitWindowThread.Abort()
                        globalVariables.pleaseWaitWindowThread = Nothing
                    End If
                End If
            Catch ex As Exception
            End Try
        End Sub
    End Module
End Namespace