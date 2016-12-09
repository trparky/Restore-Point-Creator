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
        Private longEventLogEntryID As Long, strEventLogSource, strEventLogText, _strLevel As String, levelType As Short

        Public Sub New(strInput As String)
            Me.Text = strInput
            Me.strEventLogLevel = strInput
        End Sub

        Public Property strEventLogLevel() As String
            Get
                Return _strLevel
            End Get
            Set(value As String)
                _strLevel = value
            End Set
        End Property

        Public Property eventLogText() As String
            Get
                Return strEventLogText
            End Get
            Set(value As String)
                strEventLogText = value
            End Set
        End Property

        Public Property eventLogEntryID() As Long
            Get
                Return longEventLogEntryID
            End Get
            Set(value As Long)
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

        Public Property eventLogLevel() As Short
            Get
                Return levelType
            End Get
            Set(value As Short)
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