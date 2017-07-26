Imports System.Text.RegularExpressions

Namespace Functions.listViewSorter
    ' Implements a comparer for ListView columns.
    Class ListViewComparer
        Implements IComparer

        Private intColumnNumber As Integer
        Private soSortOrder As SortOrder
        Private regExNumberParser As New Regex("\A[,0-9]+\Z", RegexOptions.Compiled) '

        Public Sub New(ByVal intInputColumnNumber As Integer, ByVal soInputSortOrder As SortOrder)
            intColumnNumber = intInputColumnNumber
            soSortOrder = soInputSortOrder
        End Sub

        ' Compare the items in the appropriate column
        ' for objects x and y.
        Public Function Compare(ByVal lvInputFirstListView As Object, ByVal lvInputSecondListView As Object) As Integer Implements IComparer.Compare
            Dim dbl1, dbl2 As Double
            Dim long1, long2 As Long
            Dim date1, date2 As Date
            Dim strFirstString, strSecondString As String
            Dim lvFirstListView As ListViewItem = lvInputFirstListView
            Dim lvSecondListView As ListViewItem = lvInputSecondListView

            ' Get the sub-item values.
            If lvFirstListView.SubItems.Count <= intColumnNumber Then
                strFirstString = ""
            Else
                strFirstString = lvFirstListView.SubItems(intColumnNumber).Text
            End If

            If lvSecondListView.SubItems.Count <= intColumnNumber Then
                strSecondString = ""
            Else
                strSecondString = lvSecondListView.SubItems(intColumnNumber).Text
            End If

            If regExNumberParser.IsMatch(strFirstString) Then
                strFirstString = strFirstString.Replace(",", "")
            End If
            If regExNumberParser.IsMatch(strSecondString) Then
                strSecondString = strSecondString.Replace(",", "")
            End If

            ' Compare them.
            If soSortOrder = SortOrder.Ascending Then
                If Double.TryParse(strFirstString, dbl1) And Double.TryParse(strSecondString, dbl2) Then
                    Return dbl1.CompareTo(dbl2)
                ElseIf Date.TryParse(strFirstString, date1) And Date.TryParse(strSecondString, date2) Then
                    Return date1.CompareTo(date2)
                ElseIf Long.TryParse(strFirstString, long1) And Long.TryParse(strSecondString, long2) Then
                    Return long1.CompareTo(long2)
                Else
                    Return String.Compare(strFirstString, strSecondString)
                End If
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
        End Function
    End Class
End Namespace