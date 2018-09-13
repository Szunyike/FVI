Imports ClassLibrary1.Szunyi.Other_Database.CrossRefs

Namespace Szunyi
    Namespace Math
        Public Class Combinatory
            Public Shared Function GetBinaryPermutation(Nof As Integer) As List(Of Boolean())
                Dim x(getFactorial(Nof - 1), Nof - 1) As Boolean
                Dim h As New List(Of Boolean())
                Dim i1(Nof - 1) As Boolean
                Dim i2(Nof - 1) As Boolean
                h.Add(i1)
                h.Add(i2)
                h(0)(0) = True
                h(1)(0) = False
                For i = 1 To Nof - 1
                    h = GetT(h, i)
                Next
                Return h


            End Function
            Private Shared Function GetT(t As List(Of Boolean()), Index As Integer) As List(Of Boolean())
                Dim out As New List(Of Boolean())
                For Each Item In t
                    Dim h1(Item.Count - 1) As Boolean
                    Item.CopyTo(h1, 0)
                    h1(Index) = True
                    Dim h2(Item.Count - 1) As Boolean
                    Item.CopyTo(h2, 0)
                    h2(Index) = False
                    out.Add(h1)
                    out.Add(h2)
                Next
                Return out
            End Function
            Public Shared Function getFactorial(ByVal Nof As Integer) As Integer
                Dim fact As Integer = 1
                Dim i As Integer = Nof

                While i > 1
                    fact = fact * i

                    i -= 1
                End While

                Return fact
            End Function

            Public Shared Function GetCounts(permutation As List(Of Boolean()), ls As List(Of CrossRefOneToMany)) _
                As List(Of CrossRefOneToMany)
                Dim out As New List(Of CrossRefOneToMany)
                If IsNothing(ls) = True Then Return Nothing
                If ls.Count < 4 Then Return Nothing
                Try


                    For Each Item In permutation
                        Dim started As Boolean = False
                        Dim reg As New List(Of String)
                        Dim Name As String = ""
                        For i1 = 0 To Item.Count - 1
                            If started = False And Item(i1) = True Then
                                reg = ls(i1).Many
                                Name = ls(i1).One
                                For i2 = 0 To i1 - 1
                                    reg.Except(ls(i2).Many)
                                Next

                                started = True
                            ElseIf started = True Then
                                Dim reg2 = ls(i1).Many
                                If Item(i1) = True Then ' Common
                                    Dim l = (reg.Intersect(reg2)).ToList
                                    reg = l
                                    Name = Name & ls(i1).One
                                Else
                                    Dim l = (reg.Except(reg2)).ToList
                                    reg = l
                                End If

                            End If
                        Next
                        out.Add(New CrossRefOneToMany(Name, reg.ToArray))
                    Next
                Catch ex As Exception
                    Dim alf As Integer = 665
                End Try
                Return out
            End Function

            Public Shared Function GetCounts(permutation As List(Of Boolean()), ls() As List(Of String)) As List(Of Integer)
                Dim out As New List(Of Integer)
                If IsNothing(ls) = True Then Return Nothing
                If ls.Count < 4 Then Return Nothing
                Try


                    For Each Item In permutation
                        Dim started As Boolean = False
                        Dim reg As New List(Of String)
                        For i1 = 0 To Item.Count - 1
                            If started = False And Item(i1) = True Then
                                reg = ls(i1)

                                For i2 = 0 To i1 - 1
                                    reg.Except(ls(i2))
                                Next

                                started = True
                            ElseIf started = True Then
                                Dim reg2 = ls(i1)
                                If Item(i1) = True Then ' Common
                                    Dim l = (reg.Intersect(reg2)).ToList
                                    reg = l
                                Else
                                    Dim l = (reg.Except(reg2)).ToList
                                    reg = l
                                End If

                            End If
                        Next
                        out.Add(reg.Count)
                    Next
                Catch ex As Exception
                    Dim alf As Integer = 665
                End Try
                Return out
            End Function
        End Class
    End Namespace

End Namespace

