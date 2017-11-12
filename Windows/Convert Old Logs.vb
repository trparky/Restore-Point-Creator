Public Class convertOldLogs
    Private _userResponse As userResponseENum
    Private boolUserResponded As Boolean = False

    Public Enum userResponseENum As Short
        yes
        no
        dontConvertThemEver
    End Enum

    Public ReadOnly Property userResponse As userResponseENum
        Get
            Return _userResponse
        End Get
    End Property

    Private Sub Convert_Old_Logs_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        PictureBox1.Image = SystemIcons.Question.ToBitmap()
        Media.SystemSounds.Exclamation.Play()
    End Sub

    Private Sub Convert_Old_Logs_FormClosing(sender As Object, e As FormClosingEventArgs) Handles Me.FormClosing
        If boolUserResponded = False Then _userResponse = userResponseENum.no
    End Sub

    Private Sub btnConvertNow_Click(sender As Object, e As EventArgs) Handles btnConvertNow.Click
        boolUserResponded = True
        _userResponse = userResponseENum.yes
        Me.Close()
    End Sub

    Private Sub btnConvertThemLater_Click(sender As Object, e As EventArgs) Handles btnConvertThemLater.Click
        boolUserResponded = True
        _userResponse = userResponseENum.no
        Me.Close()
    End Sub

    Private Sub btnDontConvertThemEver_Click(sender As Object, e As EventArgs) Handles btnDontConvertThemEver.Click
        boolUserResponded = True
        _userResponse = userResponseENum.dontConvertThemEver
        Me.Close()
    End Sub
End Class