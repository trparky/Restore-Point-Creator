Public Class Log_File_Bigger_than_1_MB
    Public Enum userResponseEnum
        yes
        no
        dontAskAgain
        null
    End Enum

    Private _userResponse As userResponseEnum = userResponseEnum.null
    Private boolUserResponded As Boolean = False

    Public ReadOnly Property userResponse As userResponseEnum
        Get
            Return _userResponse
        End Get
    End Property

    Private Sub Log_File_Bigger_than_1_MB_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        PictureBox1.Image = SystemIcons.Question.ToBitmap()
        Media.SystemSounds.Exclamation.Play()
    End Sub

    Private Sub Log_File_Bigger_than_1_MB_FormClosing(sender As Object, e As FormClosingEventArgs) Handles Me.FormClosing
        If Not boolUserResponded Then _userResponse = userResponseEnum.no
    End Sub

    Private Sub btnDontAskAgain_Click(sender As Object, e As EventArgs) Handles btnDontAskAgain.Click
        boolUserResponded = True
        _userResponse = userResponseEnum.dontAskAgain
        Me.Close()
    End Sub

    Private Sub btnNo_Click(sender As Object, e As EventArgs) Handles btnNo.Click
        boolUserResponded = True
        _userResponse = userResponseEnum.no
        Me.Close()
    End Sub

    Private Sub btnYes_Click(sender As Object, e As EventArgs) Handles btnYes.Click
        boolUserResponded = True
        _userResponse = userResponseEnum.yes
        Me.Close()
    End Sub
End Class