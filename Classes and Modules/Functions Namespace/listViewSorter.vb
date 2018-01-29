Namespace Functions.listViewSorter
    ' Implements a comparer for ListView columns.
    Class ListViewComparer
        Implements IComparer

        Private intColumnNumber As Integer
        Private soSortOrder As SortOrder

        Public Sub New(ByVal intInputColumnNumber As Integer, ByVal soInputSortOrder As SortOrder)
            intColumnNumber = intInputColumnNumber
            soSortOrder = soInputSortOrder
        End Sub

        ' Compare the items in the appropriate column
        ' for objects x and y.
        Public Function Compare(ByVal lvInputFirstListView As Object, ByVal lvInputSecondListView As Object) As Integer Implements IComparer.Compare
            Dim dbl1, dbl2 As Double
            Dim int1, int2 As Integer
            Dim long1, long2 As Long
            Dim date1, date2 As Date
            Dim strFirstString, strSecondString As String
            Dim lvFirstListView As ListViewItem = lvInputFirstListView
            Dim lvSecondListView As ListViewItem = lvInputSecondListView
            Dim lvFirstListViewType As Type = lvFirstListView.GetType
            Dim lvSecondListViewType As Type = lvSecondListView.GetType

            ' Get the sub-item values.
            strFirstString = If(lvFirstListView.SubItems.Count <= intColumnNumber, "", lvFirstListView.SubItems(intColumnNumber).Text)
            strSecondString = If(lvSecondListView.SubItems.Count <= intColumnNumber, "", lvSecondListView.SubItems(intColumnNumber).Text)

            ' Compare them.
            If soSortOrder = SortOrder.Ascending Then
                If lvFirstListViewType.Equals(GetType(myListViewItemTypes.volumeShadowCopyListItem)) And lvSecondListViewType.Equals(GetType(myListViewItemTypes.volumeShadowCopyListItem)) Then
                    Dim vol1 As myListViewItemTypes.volumeShadowCopyListItem = DirectCast(lvFirstListView, myListViewItemTypes.volumeShadowCopyListItem)
                    Dim vol2 As myListViewItemTypes.volumeShadowCopyListItem = DirectCast(lvSecondListView, myListViewItemTypes.volumeShadowCopyListItem)

                    Return vol1.dateCreated.CompareTo(vol2.dateCreated)
                ElseIf lvFirstListViewType.Equals(GetType(myListViewItemTypes.eventLogListEntry)) And lvSecondListViewType.Equals(GetType(myListViewItemTypes.eventLogListEntry)) And intColumnNumber = 1 Then
                    date1 = DirectCast(lvFirstListView, myListViewItemTypes.eventLogListEntry).logDate
                    date2 = DirectCast(lvSecondListView, myListViewItemTypes.eventLogListEntry).logDate

                    Return date1.CompareTo(date2)
                ElseIf lvFirstListViewType.Equals(GetType(myListViewItemTypes.eventLogListEntry)) And lvSecondListViewType.Equals(GetType(myListViewItemTypes.eventLogListEntry)) And intColumnNumber = 2 Then
                    long1 = DirectCast(lvFirstListView, myListViewItemTypes.eventLogListEntry).longEventLogEntryID
                    long2 = DirectCast(lvSecondListView, myListViewItemTypes.eventLogListEntry).longEventLogEntryID

                    Return long1.CompareTo(long2)
                ElseIf lvFirstListViewType.Equals(GetType(myListViewItemTypes.restorePointEntryItem)) And lvSecondListViewType.Equals(GetType(myListViewItemTypes.restorePointEntryItem)) And intColumnNumber = 0 Then
                    int1 = DirectCast(lvFirstListView, myListViewItemTypes.restorePointEntryItem).intID
                    int2 = DirectCast(lvSecondListView, myListViewItemTypes.restorePointEntryItem).intID

                    Return int1.CompareTo(int2)
                Else
                    If Double.TryParse(strFirstString, dbl1) And Double.TryParse(strSecondString, dbl2) Then
                        Return dbl1.CompareTo(dbl2)
                    ElseIf Date.TryParse(strFirstString, date1) And Date.TryParse(strSecondString, date2) Then
                        Return date1.CompareTo(date2)
                    ElseIf Long.TryParse(strFirstString, long1) And Long.TryParse(strSecondString, long2) Then
                        Return long1.CompareTo(long2)
                    Else
                        Return String.Compare(strFirstString, strSecondString)
                    End If
                End If
            Else
                If lvFirstListViewType.Equals(GetType(myListViewItemTypes.volumeShadowCopyListItem)) And lvSecondListViewType.Equals(GetType(myListViewItemTypes.volumeShadowCopyListItem)) Then
                    Dim vol1 As myListViewItemTypes.volumeShadowCopyListItem = DirectCast(lvFirstListView, myListViewItemTypes.volumeShadowCopyListItem)
                    Dim vol2 As myListViewItemTypes.volumeShadowCopyListItem = DirectCast(lvSecondListView, myListViewItemTypes.volumeShadowCopyListItem)

                    Return vol2.dateCreated.CompareTo(vol1.dateCreated)
                ElseIf lvFirstListViewType.Equals(GetType(myListViewItemTypes.eventLogListEntry)) And lvSecondListViewType.Equals(GetType(myListViewItemTypes.eventLogListEntry)) And intColumnNumber = 1 Then
                    date1 = DirectCast(lvFirstListView, myListViewItemTypes.eventLogListEntry).logDate
                    date2 = DirectCast(lvSecondListView, myListViewItemTypes.eventLogListEntry).logDate

                    Return date2.CompareTo(date1)
                ElseIf lvFirstListViewType.Equals(GetType(myListViewItemTypes.eventLogListEntry)) And lvSecondListViewType.Equals(GetType(myListViewItemTypes.eventLogListEntry)) And intColumnNumber = 2 Then
                    long1 = DirectCast(lvFirstListView, myListViewItemTypes.eventLogListEntry).longEventLogEntryID
                    long2 = DirectCast(lvSecondListView, myListViewItemTypes.eventLogListEntry).longEventLogEntryID

                    Return long2.CompareTo(long1)
                ElseIf lvFirstListViewType.Equals(GetType(myListViewItemTypes.restorePointEntryItem)) And lvSecondListViewType.Equals(GetType(myListViewItemTypes.restorePointEntryItem)) And intColumnNumber = 0 Then
                    int1 = DirectCast(lvFirstListView, myListViewItemTypes.restorePointEntryItem).intID
                    int2 = DirectCast(lvSecondListView, myListViewItemTypes.restorePointEntryItem).intID

                    Return int2.CompareTo(int1)
                Else
                    If Double.TryParse(strFirstString, dbl1) And Double.TryParse(strSecondString, dbl2) Then
                        Return dbl2.CompareTo(dbl1)
                    ElseIf Date.TryParse(strFirstString, date1) And Date.TryParse(strSecondString, date2) Then
                        Return date2.CompareTo(date1)
                    ElseIf Long.TryParse(strFirstString, long1) And Long.TryParse(strSecondString, long2) Then
                        Return long2.CompareTo(long1)
                    Else
                        Return String.Compare(strSecondString, strFirstString)
                    End If
                End If
            End If
        End Function
    End Class
End Namespace