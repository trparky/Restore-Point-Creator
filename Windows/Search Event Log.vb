Public Class Search_Event_Log
    Public Enum userResponse
        abortSearch = 0
        doSearch = 1
    End Enum

    Public Enum searceType
        typeAny = 0
        typeInfo = 1
        typeError = 2
    End Enum

    Public searchType As searceType
    Public searchTerms As String = ""
    Public boolCaseInsensitive As Boolean = False
    Public boolUseRegEx As Boolean = False
    Private boolButtonPushed As Boolean = False
    Public dialogResponse As userResponse = userResponse.abortSearch
    Public previousSearchType As searceType

    Private Sub btnSearch_Click(sender As Object, e As EventArgs) Handles btnSearch.Click
        If chkRegEx.Checked = True And Functions.support.boolTestRegExPattern(txtSearchTerms.Text.Trim) = False Then
            MsgBox("There was an error detected in your Regular Expression pattern. Please try again.", MsgBoxStyle.Information, Me.Text)
            Exit Sub
        End If

        searchTerms = txtSearchTerms.Text.Trim
        boolCaseInsensitive = chkCaseInsensitive.Checked
        boolUseRegEx = chkRegEx.Checked

        If radAny.Checked = True Then
            searchType = searceType.typeAny
        ElseIf radError.Checked = True Then
            searchType = searceType.typeError
        ElseIf radInfo.Checked = True Then
            searchType = searceType.typeInfo
        End If

        dialogResponse = userResponse.doSearch
        boolButtonPushed = True
        Me.Close()
    End Sub

    Private Sub Search_Event_Log_FormClosing(sender As Object, e As FormClosingEventArgs) Handles Me.FormClosing
        If boolButtonPushed = False Then dialogResponse = userResponse.abortSearch
    End Sub

    Private Sub txtSearchTerms_KeyUp(sender As Object, e As KeyEventArgs) Handles txtSearchTerms.KeyUp
        If txtSearchTerms.Text.Trim IsNot Nothing And e.KeyCode = Keys.Enter Then btnSearch.PerformClick()
    End Sub

    Private Sub btnClear_Click(sender As Object, e As EventArgs) Handles btnClear.Click
        txtSearchTerms.Text = Nothing
    End Sub

    Private Sub txtSearchTerms_TextChanged(sender As Object, e As EventArgs) Handles txtSearchTerms.TextChanged
        If String.IsNullOrWhiteSpace(txtSearchTerms.Text) Then
            btnSearch.Enabled = False
            btnClear.Enabled = False
        Else
            btnSearch.Enabled = True
            btnClear.Enabled = True
        End If
    End Sub

    Private Sub Search_Event_Log_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        If previousSearchType = searceType.typeAny Then
            radAny.Checked = True
        ElseIf previousSearchType = searceType.typeError Then
            radError.Checked = True
        ElseIf previousSearchType = searceType.typeInfo Then
            radInfo.Checked = True
        End If

        radError.Text = EventLogEntryType.Error.ToString
        radInfo.Text = EventLogEntryType.Information.ToString
    End Sub
End Class