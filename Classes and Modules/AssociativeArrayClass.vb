'***************************************************************************** 
'
'  VB.NET Associative Array Class
'  Copyright (C) 2009 Max Vergelli ( http://maxvergelli.wordpress.com )
'
'  Description: 
'  A VB.NET class to
'  - create easly associative arrays like in PHP;
'  - manage programmatically any structured data;
'  - export associative arrays to XML.
'
'  Version Id: AssociativeArrayClass.vb 2009-10-12 19:27:00
'  Language:   VB.NET / ASP.NET 2.0
'  Requirements: .NET Framework 2.0
'
'  Download the latest version of VB.NET Associative Array Class at
'  https://sourceforge.net/projects/vbnetassocarray/
'
'  For support and tutorials visit
'  http://maxvergelli.wordpress.com/
'
'  License: GNU Lesser General Public License (LGPL) 
'           at http://opensource.org/licenses/lgpl-2.1.php
'
'  This library is free software; you can redistribute it  and/or  modify  it 
'  under the terms of the GNU Lesser General Public License as  published  by 
'  the Free Software Foundation;  either version 2.1 of the License,  or  (at 
'  your option) any later version.
'  This library is distributed in the  hope  that  it  will  be  useful,  but 
'  WITHOUT ANY WARRANTY; without even the implied warranty of MERCHANTABILITY 
'  or FITNESS FOR A PARTICULAR PURPOSE.  See the  GNU Lesser  General  Public 
'  License for more details.
'  You should have received a copy of  the  GNU Lesser General Public License 
'  along with this library;  if not,  write to the  Free Software Foundation, 
'  Inc., 59 Temple Place, Suite 330, Boston, MA 02111-1307 USA 
'
'  This notice must be retained in the code as is!
'
'***************************************************************************** 

Imports System
Imports System.Collections
Imports System.Collections.Generic


Public Class AssociativeArray

    Private d As New Dictionary(Of String, Object)
    Private c As Integer = 0
    Private l As New AssociativeArray_Item("", "")

    Public Enum CompareAs As Integer
        Binary = 0
        Text = 1
    End Enum

    Public Sub New()
        c = CompareAs.Binary
    End Sub

    Protected Overrides Sub Finalize()
        If IsReference(d) Then
            d.Clear()
            d = Nothing
        End If
    End Sub

    Public WriteOnly Property CompareMode() As CompareAs
        Set(ByVal CompareModeValue As CompareAs)
            c = CompareModeValue
        End Set
    End Property

    Public ReadOnly Property Items() As AssociativeArray_Item()
        Get
            Dim a(d.Count - 1) As AssociativeArray_Item
            Dim n As Integer = 0
            For Each k As KeyValuePair(Of String, Object) In d
                a(n) = New AssociativeArray_Item(k.Key, k.Value)
                n = n + 1
            Next
            Return a
        End Get
    End Property

    Default Public Property Item(ByVal Key As String) As Object
        Get
            If Not Key Is Nothing And Key <> "" Then
                If c = CompareAs.Binary Then
                    If d.ContainsKey(Key) Then
                        Return d.Item(Key)
                    Else
                        Return Nothing
                    End If
                Else
                    Dim pk As String = ""
                    Dim pv As Object = ""
                    If CaseInsensitiveExists(Key, pk, pv) = True Then
                        Return pv
                    Else
                        Return Nothing
                    End If
                End If
            Else
                Return Nothing
            End If
        End Get
        Set(ByVal Value As Object)
            If Not Key Is Nothing And Key <> "" Then
                If c = CompareAs.Binary Then
                    If d.ContainsKey(Key) Then
                        d.Item(Key) = Value
                        If d.ElementAt(d.Count - 1).Key = Key Then l = New AssociativeArray_Item(Key, Value)
                    Else
                        d.Add(Key, Value)
                        l = New AssociativeArray_Item(Key, Value)
                    End If
                Else
                    Dim pk As String = ""
                    Dim pv As Object = ""
                    If CaseInsensitiveExists(Key, pk, pv) = True Then
                        d.Item(pk) = Value
                        If d.ElementAt(d.Count - 1).Key = pk Then l = New AssociativeArray_Item(pk, Value)
                    Else
                        d.Add(Key, Value)
                        l = New AssociativeArray_Item(Key, Value)
                    End If
                End If
            End If
        End Set
    End Property

    Public ReadOnly Property LastItem() As AssociativeArray_Item
        Get
            If l.Key <> "" Then
                Return l
            Else
                Return Nothing
            End If
        End Get
    End Property

    Public Function Exists(ByVal Key As String) As Boolean
        If c = CompareAs.Binary Then
            If d.ContainsKey(Key) Then
                Return True
            Else
                Return False
            End If
        Else
            Dim pk As String = ""
            Dim pv As Object = ""
            If CaseInsensitiveExists(Key, pk, pv) = True Then
                Return True
            Else
                Return False
            End If
        End If
    End Function

    Public Function Count() As Integer
        Return d.Count
    End Function

    Public Sub Remove(ByVal Key As String)
        If d.ContainsKey(Key) Then
            d.Remove(Key)
        End If
    End Sub

    Public Sub RemoveAll()
        d.Clear()
    End Sub

    Public Function ToXML() As String
        'Return a String formatted as XML 1.0 W3C Standard at http://www.w3.org/TR/REC-xml/
        '<?xml version="1.0" encoding="utf-8"?>
        '<array>
        '<key>
        '	<name><![CDATA[Key Name 1]]></name>
        '	<value><![CDATA[Test Key Value]]></value>
        '</key>
        '<key>
        '	<name><![CDATA[Key Name 2]]></name>
        '	<value>
        '		<key>
        '			<name><![CDATA[SubKey Name 3]]></name>
        '			<value><![CDATA[Test Subkey Value]]></value>
        '		</key>
        '	</value>
        '</key>
        '[...]
        '</array>
        Dim xml As String = "<?xml version=""1.0"" encoding=""utf-8""?>" & vbCrLf & _
                              "<array>" & vbCrLf & _
                              Enumerate(Me) & _
                              "</array>" & vbCrLf
        Return xml
    End Function

    Private Function Enumerate(ByVal o As Object) As String
        Dim xml As String = ""
        If TypeOf o Is AssociativeArray Then
            For Each e As Object In o.Items
                xml = xml & "<key>" & vbCrLf & _
                    "<name><![CDATA[" & e.Key & "]]></name>" & vbCrLf & _
                    "<value>"
                If TypeOf e.Value Is AssociativeArray Then
                    xml = xml & vbCrLf & Enumerate(e.Value) & vbCrLf
                Else
                    xml = xml & "<![CDATA[" & e.Value.ToString & "]]>" & vbCrLf
                End If
                xml = xml & "</value>" & vbCrLf & "</key>" & vbCrLf
            Next
        End If
        Return xml
    End Function

    Private Function CaseInsensitiveExists(ByVal Key As String, _
                                           ByRef refKey As String, _
                                           ByRef refValue As Object) As Boolean
        Dim r As Boolean = False
        For Each i As KeyValuePair(Of String, Object) In d
            If String.Compare(Key, i.Key, True) = 0 Then
                refKey = i.Key
                refValue = i.Value
                r = True
                Exit For
            End If
        Next
        Return r
    End Function

End Class

Public Class AssociativeArray_Item

    Private k As String
    Private v As Object

    Public Sub New(ByVal Key As String, ByVal Value As Object)
        k = Key
        v = Value
    End Sub

    Public Property Key() As String
        Get
            Return k
        End Get
        Set(ByVal sValue As String)
            k = sValue
        End Set
    End Property

    Public Property Value() As Object
        Get
            Return v
        End Get
        Set(ByVal oValue As Object)
            v = oValue
        End Set
    End Property

End Class
