Imports System.ComponentModel
Imports System.Reflection

Namespace Functions.PSLib
    '--------------------------------------------------------------------------------
    Public NotInheritable Class cEventHelper
        Private Sub New()
        End Sub
        Shared dicEventFieldInfos As New Dictionary(Of Type, List(Of FieldInfo))()

        Private Shared ReadOnly Property AllBindings() As BindingFlags
            Get
                Return BindingFlags.IgnoreCase Or BindingFlags.[Public] Or BindingFlags.NonPublic Or BindingFlags.Instance Or BindingFlags.[Static]
            End Get
        End Property

        '--------------------------------------------------------------------------------
        Private Shared Function GetTypeEventFields(t As Type) As List(Of FieldInfo)
            If dicEventFieldInfos.ContainsKey(t) Then
                Return dicEventFieldInfos(t)
            End If

            Dim lst As New List(Of FieldInfo)()
            BuildEventFields(t, lst)
            dicEventFieldInfos.Add(t, lst)
            Return lst
        End Function

        '--------------------------------------------------------------------------------
        Private Shared Sub BuildEventFields(t As Type, lst As List(Of FieldInfo))
            ' Type.GetEvent(s) gets all Events for the type AND it's ancestors
            ' Type.GetField(s) gets only Fields for the exact type.
            '  (BindingFlags.FlattenHierarchy only works on PROTECTED & PUBLIC
            '   doesn't work because Fieds are PRIVATE)

            ' NEW version of this routine uses .GetEvents and then uses .DeclaringType
            ' to get the correct ancestor type so that we can get the FieldInfo.
            For Each ei As EventInfo In t.GetEvents(AllBindings)
                Dim dt As Type = ei.DeclaringType
                Dim fi As FieldInfo = dt.GetField(ei.Name, AllBindings)
                If fi IsNot Nothing Then
                    lst.Add(fi)
                End If
            Next
        End Sub

        '--------------------------------------------------------------------------------
        Private Shared Function GetStaticEventHandlerList(t As Type, obj As Object) As EventHandlerList
            Dim mi As MethodInfo = t.GetMethod("get_Events", AllBindings)
            Return DirectCast(mi.Invoke(obj, New Object() {}), EventHandlerList)
        End Function

        '--------------------------------------------------------------------------------
        Public Shared Sub RemoveAllEventHandlers(obj As Object)
            RemoveEventHandler(obj, "")
        End Sub

        '--------------------------------------------------------------------------------
        Public Shared Sub RemoveEventHandler(obj As Object, EventName As String)
            If obj Is Nothing Then
                Return
            End If

            Dim t As Type = obj.[GetType]()
            Dim event_fields As List(Of FieldInfo) = GetTypeEventFields(t)
            Dim static_event_handlers As EventHandlerList = Nothing

            For Each fi As FieldInfo In event_fields
                If String.IsNullOrEmpty(EventName) = False AndAlso String.Compare(EventName, fi.Name, True) <> 0 Then
                    Continue For
                End If

                ' After hours and hours of research and trial and error, it turns out that
                ' STATIC Events have to be treated differently from INSTANCE Events...
                If fi.IsStatic Then
                    ' STATIC EVENT
                    If static_event_handlers Is Nothing Then
                        static_event_handlers = GetStaticEventHandlerList(t, obj)
                    End If

                    Dim idx As Object = fi.GetValue(obj)
                    Dim eh As [Delegate] = static_event_handlers(idx)
                    If eh Is Nothing Then
                        Continue For
                    End If

                    Dim dels As [Delegate]() = eh.GetInvocationList()
                    If dels Is Nothing Then
                        Continue For
                    End If

                    Dim ei As EventInfo = t.GetEvent(fi.Name, AllBindings)
                    For Each del As [Delegate] In dels
                        ei.RemoveEventHandler(obj, del)
                    Next
                Else
                    ' INSTANCE EVENT
                    Dim ei As EventInfo = t.GetEvent(fi.Name, AllBindings)
                    If ei IsNot Nothing Then
                        Dim val As Object = fi.GetValue(obj)
                        Dim mdel As [Delegate] = TryCast(val, [Delegate])
                        If mdel IsNot Nothing Then
                            For Each del As [Delegate] In mdel.GetInvocationList()
                                ei.RemoveEventHandler(obj, del)
                            Next
                        End If
                    End If
                End If
            Next
        End Sub

        '--------------------------------------------------------------------------------
    End Class
End Namespace