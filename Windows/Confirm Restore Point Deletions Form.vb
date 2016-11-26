Public Class Confirm_Restore_Point_Deletions_Form
    Public Enum userResponseENum
        yes
        cancel
    End Enum

    Public restorePointIDsToBeDeleted As New Dictionary(Of String, restorePointInfo)
    Public userResponse As userResponseENum = userResponseENum.cancel

    Sub scanCheckBoxesForUncheckedBoxes()
        Dim numberOfCheckboxes As Short = restorePointGroup.Controls.Count
        Dim numberOfUncheckedCheckboxes As Short = 0

        For index = (restorePointGroup.Controls.Count - 1) To 0 Step -1
            If restorePointGroup.Controls(index).GetType = GetType(CheckBox) Then
                If Not DirectCast(restorePointGroup.Controls(index), CheckBox).Checked Then numberOfUncheckedCheckboxes += 1
            End If
        Next index

        If numberOfUncheckedCheckboxes = numberOfCheckboxes Then
            btnConfirm.Enabled = False
            MsgBox("You have unchecked all of the checkboxes, you have none of your previously selected restore points queued for deletion.", MsgBoxStyle.Information, Me.Text)
        Else
            btnConfirm.Enabled = True
        End If
    End Sub

    Sub createCheckbox(restorePointInfo As KeyValuePair(Of String, restorePointInfo), xPosition As Integer, yPosition As Integer)
        Dim chkBox As New CheckBox
        chkBox.Name = "confirm_delete_" & restorePointInfo.Key
        chkBox.Location = New Point(xPosition, yPosition)
        chkBox.Text = restorePointInfo.Value.strName & " (ID: " & restorePointInfo.Key & ")" & vbCrLf & "Created On: " & restorePointInfo.Value.strCreatedDate
        chkBox.AutoSize = True
        chkBox.Checked = True

        AddHandler chkBox.Click, Sub() scanCheckBoxesForUncheckedBoxes()

        restorePointGroup.Invoke(Sub() restorePointGroup.Controls.Add(chkBox))

        chkBox = Nothing
    End Sub

    Private Sub Confirm_Restore_Point_Deletions_Form_Shown(sender As Object, e As EventArgs) Handles Me.Shown
        Dim yPosition As Integer = 10
        Dim xPosition As Integer = 12

        For Each item As KeyValuePair(Of String, restorePointInfo) In restorePointIDsToBeDeleted
            createCheckbox(item, xPosition, yPosition)
            yPosition += 40
        Next
    End Sub

    Private Sub Confirm_Restore_Point_Deletions_Form_Resize(sender As Object, e As EventArgs) Handles Me.Resize
        If Me.Width <> 739 Then Me.Width = 739
    End Sub

    Private Sub Confirm_Restore_Point_Deletions_Form_ResizeEnd(sender As Object, e As EventArgs) Handles Me.ResizeEnd
        My.Settings.batchConfirmWindowSize = Me.Size
    End Sub

    Private Sub Confirm_Restore_Point_Deletions_Form_Load(sender As Object, e As EventArgs) Handles Me.Load
        Me.Size = My.Settings.batchConfirmWindowSize
    End Sub

    Private Sub btnCancel_Click(sender As Object, e As EventArgs) Handles btnCancel.Click
        Me.Close()
    End Sub

    Private Sub btnConfirm_Click(sender As Object, e As EventArgs) Handles btnConfirm.Click
        userResponse = userResponseENum.yes
        Dim checkBoxObject As CheckBox, strCheckBoxID As String

        For index = (restorePointGroup.Controls.Count - 1) To 0 Step -1
            If restorePointGroup.Controls(index).GetType = GetType(CheckBox) Then
                checkBoxObject = DirectCast(restorePointGroup.Controls(index), CheckBox)

                If Not checkBoxObject.Checked Then
                    strCheckBoxID = checkBoxObject.Name.Replace("confirm_delete_", "").Trim
                    If restorePointIDsToBeDeleted.Keys.Contains(strCheckBoxID) Then restorePointIDsToBeDeleted.Remove(strCheckBoxID)
                End If
            End If
        Next index

        Me.Close()
    End Sub
End Class