Public Enum settingType
    settings = 0
    registry = 1
End Enum

Public Class exportedSettings
    Public settingType As settingType
    Public strName As String, value As Object
    Public type As String
End Class

Public Class exportedSettingsFile
    Public xmlPayload, randomString, checksum As String
End Class