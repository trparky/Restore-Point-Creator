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

        For Each control As Control In restorePointGroup.Controls
            If control.GetType = GetType(myCheckbox) Then
                If Not DirectCast(control, myCheckbox).Checked Then numberOfUncheckedCheckboxes += 1
            End If
        Next

        If numberOfUncheckedCheckboxes = numberOfCheckboxes Then
            btnConfirm.Enabled = False
            MsgBox("You have unchecked all of the checkboxes, you have none of your previously selected restore points queued for deletion.", MsgBoxStyle.Information, Me.Text)
        Else
            btnConfirm.Enabled = True
        End If
    End Sub

    Sub createCheckbox(restorePointInfo As KeyValuePair(Of String, restorePointInfo), xPosition As Integer, yPosition As Integer)
        Dim chkBox As New myCheckbox
        chkBox.restorePointID = restorePointInfo.Key
        chkBox.Name = "confirm_delete_" & restorePointInfo.Key
        chkBox.Location = New Point(xPosition, yPosition)

        chkBox.Text = restorePointInfo.Value.strName & String.Format(" (ID: {0})", restorePointInfo.Key)
        chkBox.Text &= vbCrLf & String.Format("Created On: {0}", restorePointInfo.Value.strCreatedDate)
        chkBox.Text &= vbCrLf & String.Format("Type: {0}", restorePointInfo.Value.strRestorePointType)

        chkBox.AutoSize = True
        chkBox.Checked = True

        AddHandler chkBox.Click, Sub() scanCheckBoxesForUncheckedBoxes()

        restorePointGroup.Invoke(Sub() restorePointGroup.Controls.Add(chkBox))

        chkBox = Nothing
    End Sub

    Private Sub Confirm_Restore_Point_Deletions_Form_Shown(sender As Object, e As EventArgs) Handles Me.Shown
        Dim yPosition As Integer = 5
        Dim xPosition As Integer = 5

        For Each item As KeyValuePair(Of String, restorePointInfo) In restorePointIDsToBeDeleted
            createCheckbox(item, xPosition, yPosition)
            yPosition += 50
        Next
    End Sub

    Private Sub Confirm_Restore_Point_Deletions_Form_Resize(sender As Object, e As EventArgs) Handles Me.Resize
        If Me.Width <> 739 Then Me.Width = 739
    End Sub

    Private Sub Confirm_Restore_Point_Deletions_Form_ResizeEnd(sender As Object, e As EventArgs) Handles Me.ResizeEnd
        My.Settings.batchConfirmWindowSize = Me.Size
    End Sub

    Private Sub Confirm_Restore_Point_Deletions_Form_Load(sender As Object, e As EventArgs) Handles Me.Load
        Media.SystemSounds.Beep.Play()
        Me.Size = My.Settings.batchConfirmWindowSize
    End Sub

    Private Sub btnCancel_Click(sender As Object, e As EventArgs) Handles btnCancel.Click
        Me.Close()
    End Sub

    Private Sub btnConfirm_Click(sender As Object, e As EventArgs) Handles btnConfirm.Click
        userResponse = userResponseENum.yes
        Dim checkBoxObject As myCheckbox

        For Each control As Control In restorePointGroup.Controls
            If control.GetType = GetType(myCheckbox) Then
                checkBoxObject = DirectCast(control, myCheckbox)

                If Not checkBoxObject.Checked Then
                    If restorePointIDsToBeDeleted.Keys.Contains(checkBoxObject.restorePointID) Then restorePointIDsToBeDeleted.Remove(checkBoxObject.restorePointID)
                End If
            End If
        Next

        Me.Close()
    End Sub

    Private Sub btnCheckAll_Click(sender As Object, e As EventArgs) Handles btnCheckAll.Click
        For Each control As Control In restorePointGroup.Controls
            If control.GetType = GetType(myCheckbox) Then
                DirectCast(control, myCheckbox).Checked = True
            End If
        Next

        btnConfirm.Enabled = True
    End Sub

    Private Sub btnUncheckAll_Click(sender As Object, e As EventArgs) Handles btnUncheckAll.Click
        For Each control As Control In restorePointGroup.Controls
            If control.GetType = GetType(myCheckbox) Then
                DirectCast(control, myCheckbox).Checked = False
            End If
        Next

        btnConfirm.Enabled = False
        MsgBox("You have unchecked all of the checkboxes, you have none of your previously selected restore points queued for deletion.", MsgBoxStyle.Information, Me.Text)
    End Sub
End Class

' This class extends the checkbox so that I can add more properties to it for my purposes.
Class myCheckbox
    Inherits CheckBox
    Private strRestorePointID As String

    Public Property restorePointID() As String
        Get
            Return strRestorePointID
        End Get
        Set(value As String)
            strRestorePointID = value
        End Set
    End Property
End Class