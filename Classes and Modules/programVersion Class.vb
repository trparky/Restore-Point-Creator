Public Class programVersion
    Public major As Short = 0
    Public minor As Short = 0
    Public build As Short = 0
    Public revision As Short = 0

    Public Function toOutputString(Optional ByVal includeRevision As Boolean = False) As String
        If includeRevision = True Then
            Return String.Format("{0}.{1}.{2}.{3}", major, minor, build, revision)
        Else
            Return String.Format("{0}.{1}.{2}", major, minor, build)
        End If
    End Function
End Class