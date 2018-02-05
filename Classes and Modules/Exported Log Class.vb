Public Class restorePointCreatorExportedLog
    Public logType As Short
    Public logID As Long
    Public unixTime As ULong
    Public logData, logSource As String
    Public dateObject As Date
    Public boolException As Boolean
    Public boolSubmitted As Boolean
End Class

Public Class exportedLogFile
    Public programVersion, operatingSystem As String, version As Short
    Public logsEntries As List(Of restorePointCreatorExportedLog)
End Class