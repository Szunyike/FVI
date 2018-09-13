
Namespace Szunyi
    Namespace Text
        Public Class UniqueDistinct
            Public Shared Function GetDuplicated(Words As List(Of String)) As List(Of String)
                Dim out As New List(Of String)
                For Each group In GetGroups_Iterator(Words)
                    If group.Count > 1 Then
                        out.AddRange(group)
                    End If
                Next
                Return out
            End Function

            Public Shared Function GetDuplicatedToUnique(Words As List(Of String)) As List(Of String)
                Dim out As New List(Of String)
                For Each group In GetGroups_Iterator(Words)
                    If group.Count > 1 Then
                        out.Add(group.First)
                    End If
                Next
                Return out
            End Function

            Public Shared Function GetUniques(Words As List(Of String)) As List(Of String)
                Dim out As New List(Of String)
                For Each group In GetGroups_Iterator(Words)
                    If group.Count = 1 Then
                        out.Add(group.First)
                    End If
                Next
                Return out
            End Function
            Public Shared Function Get_One_Copy(Words As List(Of String)) As List(Of String)
                Dim out As New List(Of String)
                For Each group In GetGroups_Iterator(Words)
                    out.Add(group.First)
                Next
                Return out
            End Function
            Private Shared Iterator Function GetGroups_Iterator(Words As List(Of String)) As IEnumerable(Of IEnumerable(Of String))
                Dim x = From t In Words Group By t Into Group
                For Each t In x
                    Yield t.Group
                Next

            End Function

        End Class
    End Namespace
End Namespace
