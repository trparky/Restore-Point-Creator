Namespace Functions.supportClasses
    Public Class ShadowStorageData
        Public AllocatedSpace As ULong = 0
        Public DiffVolume As String = Nothing
        Public MaxSpace As ULong = 0
        Public UsedSpace As ULong = 0
        Public Volume As String = Nothing
        Public UsedSpaceHuman As String = Nothing
        Public MaxSpaceHuman As String = Nothing
        Public AllocatedSpaceHuman As String = Nothing
    End Class

    Public Class processorInfoClass
        Public strProcessor As String = "unknown"
        Public strNumberOfCores As String = "unknown"
    End Class
End Namespace