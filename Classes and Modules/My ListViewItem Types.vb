Namespace myListViewItemTypes
    ' This class extends the ListViewItem so that I can add more properties to it for my purposes.
    Public Class volumeShadowCopyListItem
        Inherits ListViewItem
        Private strDeviceID As String

        Public Property deviceID() As String
            Get
                Return strDeviceID
            End Get
            Set(value As String)
                strDeviceID = value
            End Set
        End Property
    End Class

    ' This class extends the ListViewItem so that I can add more properties to it for my purposes.
    Public Class eventLogListEntry
        Inherits ListViewItem
        Private longEventLogEntryID As Long, strEventLogSource As String, levelType As Byte

        Public Property eventLogEntryID() As String
            Get
                Return longEventLogEntryID
            End Get
            Set(value As String)
                longEventLogEntryID = value
            End Set
        End Property

        Public Property eventLogSource() As String
            Get
                Return strEventLogSource
            End Get
            Set(value As String)
                strEventLogSource = value
            End Set
        End Property

        Public Property eventLogLevel() As String
            Get
                Return levelType
            End Get
            Set(value As String)
                levelType = value
            End Set
        End Property
    End Class

    ' This class extends the ListViewItem so that I can add more properties to it for my purposes.
    Public Class restorePointEntryItem
        Inherits ListViewItem
        Private strRestorePointID, strRestorePointName, strRestorePointDate, strRestorePointType, strRestorePointAge As String
        Private dateRestorePointDate As Date

        Public Property restorePointID() As String
            Get
                Return strRestorePointID
            End Get
            Set(value As String)
                strRestorePointID = value
            End Set
        End Property

        Public Property restorePointAge() As String
            Get
                Return strRestorePointAge
            End Get
            Set(value As String)
                strRestorePointAge = value
            End Set
        End Property

        Public Property restorePointName() As String
            Get
                Return strRestorePointName
            End Get
            Set(value As String)
                strRestorePointName = value
            End Set
        End Property

        Public Property restorePointDate() As String
            Get
                Return strRestorePointDate
            End Get
            Set(value As String)
                strRestorePointDate = value
            End Set
        End Property

        Public Property restorePointType() As String
            Get
                Return strRestorePointType
            End Get
            Set(value As String)
                strRestorePointType = value
            End Set
        End Property

        Public Property rawRestorePointDate() As Date
            Get
                Return dateRestorePointDate
            End Get
            Set(value As Date)
                dateRestorePointDate = value
            End Set
        End Property
    End Class
End Namespace