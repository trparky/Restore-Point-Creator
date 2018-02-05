Namespace myListViewItemTypes
    ' This class extends the ListViewItem so that I can add more properties to it for my purposes.
    Public Class volumeShadowCopyListItem
        Inherits ListViewItem
        Private strDeviceID As String
        Private _dateCreated As Date

        Public Property deviceID() As String
            Get
                Return strDeviceID
            End Get
            Set(value As String)
                strDeviceID = value
            End Set
        End Property

        Public Property dateCreated() As Date
            Get
                Return _dateCreated
            End Get
            Set(value As Date)
                _dateCreated = value
            End Set
        End Property
    End Class

    ' This class extends the ListViewItem so that I can add more properties to it for my purposes.
    Public Class eventLogListEntry
        Inherits ListViewItem
        Private _longEventLogEntryID As Long, _strEventLogSource, _strEventLogText, _strLevel As String, _shortLevelType As Short
        Private _logDate As Date, _eventLogType As EventLogEntryType
        Private _boolException, _boolSubmitted As Boolean

        Public Sub New(strInput As String)
            Me.Text = strInput
            Me.strEventLogLevel = strInput
        End Sub

        Public Property boolSubmitted() As Boolean
            Get
                Return _boolSubmitted
            End Get
            Set(value As Boolean)
                _boolSubmitted = value
            End Set
        End Property

        Public Property boolException() As Boolean
            Get
                Return _boolException
            End Get
            Set(value As Boolean)
                _boolException = value
            End Set
        End Property

        Public Property eventLogType() As EventLogEntryType
            Get
                Return _eventLogType
            End Get
            Set(value As EventLogEntryType)
                _eventLogType = value
            End Set
        End Property

        Public Property logDate() As Date
            Get
                Return _logDate
            End Get
            Set(value As Date)
                _logDate = value
            End Set
        End Property

        Public Property strEventLogLevel() As String
            Get
                Return _strLevel
            End Get
            Set(value As String)
                _strLevel = value
            End Set
        End Property

        Public Property strEventLogText() As String
            Get
                Return _strEventLogText
            End Get
            Set(value As String)
                _strEventLogText = value
            End Set
        End Property

        Public Property longEventLogEntryID() As Long
            Get
                Return _longEventLogEntryID
            End Get
            Set(value As Long)
                _longEventLogEntryID = value
            End Set
        End Property

        Public Property strEventLogSource() As String
            Get
                Return _strEventLogSource
            End Get
            Set(value As String)
                _strEventLogSource = value
            End Set
        End Property

        Public Property shortLevelType() As Short
            Get
                Return _shortLevelType
            End Get
            Set(value As Short)
                _shortLevelType = value
            End Set
        End Property
    End Class

    ' This class extends the ListViewItem so that I can add more properties to it for my purposes.
    Public Class restorePointEntryItem
        Inherits ListViewItem
        Private _strRestorePointID, _strRestorePointName, _strRestorePointDate, _strRestorePointType, _strRestorePointAge As String
        Private _intRestorePointID As Integer
        Private _dateRestorePointDate As Date

        Public Property intID() As Integer
            Get
                Return _intRestorePointID
            End Get
            Set(value As Integer)
                _intRestorePointID = value
            End Set
        End Property

        Public Property strID() As String
            Get
                Return _strRestorePointID
            End Get
            Set(value As String)
                _strRestorePointID = value
            End Set
        End Property

        Public Property strAge() As String
            Get
                Return _strRestorePointAge
            End Get
            Set(value As String)
                _strRestorePointAge = value
            End Set
        End Property

        Public Property strName() As String
            Get
                Return _strRestorePointName
            End Get
            Set(value As String)
                _strRestorePointName = value
            End Set
        End Property

        Public Property strCreatedDate() As String
            Get
                Return _strRestorePointDate
            End Get
            Set(value As String)
                _strRestorePointDate = value
            End Set
        End Property

        Public Property strRestorePointType() As String
            Get
                Return _strRestorePointType
            End Get
            Set(value As String)
                _strRestorePointType = value
            End Set
        End Property

        Public Property dateCreated() As Date
            Get
                Return _dateRestorePointDate
            End Get
            Set(value As Date)
                _dateRestorePointDate = value
            End Set
        End Property
    End Class

    ' This class extends the ListViewItem so that I can add more properties to it for my purposes.
    Public Class contactFormFileListItem
        Inherits ListViewItem
        Private _strFileName As String, _longFileSize As Long

        Public Property strFileName() As String
            Get
                Return _strFileName
            End Get
            Set(value As String)
                _strFileName = value
            End Set
        End Property

        Public Property longFileSize() As Long
            Get
                Return _longFileSize
            End Get
            Set(value As Long)
                _longFileSize = value
            End Set
        End Property
    End Class
End Namespace