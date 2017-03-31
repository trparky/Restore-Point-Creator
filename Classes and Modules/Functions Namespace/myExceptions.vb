Namespace Functions.myExceptions
    Module myExceptions
        Public Class integerTryParseException
            Inherits Exception
            Private _strThatCouldNotBeParsedIntoAnInteger As String

            Public Property strThatCouldNotBeParsedIntoAnInteger As String
                Get
                    Return _strThatCouldNotBeParsedIntoAnInteger
                End Get
                Set(value As String)
                    _strThatCouldNotBeParsedIntoAnInteger = value
                End Set
            End Property

            Public Sub New()
            End Sub

            Public Sub New(message As String)
                MyBase.New(message)
            End Sub

            Public Sub New(message As String, inner As Exception)
                MyBase.New(message, inner)
            End Sub
        End Class
    End Module
End Namespace