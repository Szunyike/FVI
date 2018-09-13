Imports ClassLibrary1.Szunyi.mRNA.Transcript
Namespace Szunyi
    Public Class Util_Helpers
        Public Shared Function Get_All_Enum_Names_Values(Of t)(ByVal currentlySelectedEnum As Object) As List(Of String)
            Dim out As New List(Of String)
            Dim enumList As Type = GetType(t)
            If Not enumList.IsEnum Then Throw New InvalidOperationException("Object is not an Enum.")

            Dim values() As Integer = CType([Enum].GetValues(GetType(t)), Integer())
            Dim Names() = CType([Enum].GetNames(GetType(t)), String())


            For i1 = 0 To values.Count - 1
                out.Add(Names(i1) & ":" & values(i1))
            Next
            Return out


        End Function

        Public Shared Function Get_All_Enum_Names(Of t)(ByVal currentlySelectedEnum As Object) As List(Of String)
            Dim out As New List(Of String)
            Dim enumList As Type = GetType(t)
            If Not enumList.IsEnum Then Throw New InvalidOperationException("Object is not an Enum.")

            Dim values() As Integer = CType([Enum].GetValues(GetType(t)), Integer())
            Dim Names() = CType([Enum].GetNames(GetType(t)), String())

                Return Names.ToList


        End Function
        Public Shared Function Get_Enum_Name(Of T)(first As String) As String
            Dim enumList As Type = GetType(T)
            Dim x = CType([Enum].Parse(GetType(T), first), T)
            Dim values() As Integer = CType([Enum].GetValues(GetType(T)), Integer())
            Dim Names() = CType([Enum].GetNames(GetType(T)), String())

            For i1 = 0 To Names.Count - 1
                If values(i1) = first Then
                    Return Names(i1)
                End If
            Next
            Return -1
        End Function
        Public Shared Function Get_Enum_Value(Of T)(first As String) As Integer
            Dim enumList As Type = GetType(T)
            Dim x = CType([Enum].Parse(GetType(T), first), T)
            Dim values() As Integer = CType([Enum].GetValues(GetType(T)), Integer())
            Dim Names() = CType([Enum].GetNames(GetType(T)), String())

            For i1 = 0 To Names.Count - 1
                If Names(i1) = first Then
                    Return values(i1)
                End If
            Next
            Return -1
        End Function
        Public Shared Function Get_Enum_Value(Of T)(Items As List(Of String)) As List(Of Integer)
            Dim enumList As Type = GetType(T)
            Dim x = CType([Enum].Parse(GetType(T), Items.First), T)
            Dim values() As Integer = CType([Enum].GetValues(GetType(T)), Integer())
            Dim Names() = CType([Enum].GetNames(GetType(T)), String())
            Dim out As New List(Of Integer)
            For Each Item In Items
                For i1 = 0 To Names.Count - 1
                    If Names(i1) = Item Then
                        out.Add(values(i1))
                    End If
                Next
            Next

            Return out
        End Function

        Public Shared Function Get_Property_Value(src As Object, Property_Name As String)
            Return src.GetType().GetProperty(Property_Name).GetValue(src)
        End Function

    End Class
End Namespace

