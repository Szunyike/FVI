Imports System.IO

Namespace Szunyi
    Namespace DNA
        Public Class Weiht_Matrix
            Public Property Name As String
            Public ID As String
            Public Property Consensus As Bio.Sequence
            Public Property Counts As New Dictionary(Of Byte, Integer())
            Public Property Percents As New Dictionary(Of Byte, Double())
            Public Property Regexp_Motif As String
        End Class
        Public Class Weiht_Matrix_Manipulation

            Public Function Get_From_File(File As FileInfo) As List(Of Weiht_Matrix)
                Dim res As New List(Of Weiht_Matrix)
                For Each LInes25 In Szunyi.IO.Import.Text.Read_Lines_by_Group(File, 22)
                    Dim x As New Weiht_Matrix
                    Dim s = Split(LInes25(3), " ")
                    x.Name = s.First
                    x.ID = s(1)
                    x.Counts.Add(AscW("A"c), Szunyi.Text.Lists.Get_Integers(LInes25(14), " "))
                    x.Counts.Add(AscW("C"c), Szunyi.Text.Lists.Get_Integers(LInes25(15), " "))
                    x.Counts.Add(AscW("G"c), Szunyi.Text.Lists.Get_Integers(LInes25(16), " "))
                    x.Counts.Add(AscW("T"c), Szunyi.Text.Lists.Get_Integers(LInes25(17), " "))

                    x.Percents.Add(AscW("A"c), Szunyi.Text.Lists.Get_Doubles(LInes25(14), " "))
                    x.Percents.Add(AscW("C"c), Szunyi.Text.Lists.Get_Doubles(LInes25(15), " "))
                    x.Percents.Add(AscW("G"c), Szunyi.Text.Lists.Get_Doubles(LInes25(16), " "))
                    x.Percents.Add(AscW("T"c), Szunyi.Text.Lists.Get_Doubles(LInes25(17), " "))
                    x.Regexp_Motif = Get_RegExp_Motif(x)
                    Dim o = LInes25(20)
                    Dim p = o.Replace(" ", "")
                    x.Consensus = New Bio.Sequence(Bio.Alphabets.DNA, p)
                    res.Add(x)

                    Dim alf As Int16 = 54
                Next
                Return res

            End Function

            Private Function Get_RegExp_Motif(x As Weiht_Matrix) As String
                Dim SubMotifs As New List(Of String)
                Dim Counts = x.Counts
                For i1 = 0 To counts.First.Value.Count - 1
                    Dim s As String = ""
                    Dim TotalCount As Integer = 0
                    For Each Item In counts
                        If Item.Value(i1) > 0 Then s = s & ChrW(Item.Key)
                        TotalCount += Item.Value(i1)
                    Next
                    SubMotifs.Add(s)
                    For Each Item In Counts
                        x.Percents(Item.Key)(i1) = Item.Value(i1) / TotalCount
                    Next
                Next
                Return Get_RegExp_Motif_From_SubMotifs(SubMotifs)
            End Function

            Private Function Get_RegExp_Motif_From_SubMotifs(subMotifs As List(Of String)) As String
                Dim out As New System.Text.StringBuilder
                Dim SingleMatch As New List(Of Integer)

                For i1 = 0 To subMotifs.Count - 2
                    If subMotifs(i1).Count = 1 Then
                        For i2 = i1 + 1 To subMotifs.Count - 1
                            If subMotifs(i2).Count = 1 Then
                                subMotifs(i2) = subMotifs(i2 - 1) & subMotifs(i2)
                                subMotifs(i2 - 1) = ""
                                SingleMatch.Add(i2)
                            End If
                        Next
                    End If
                Next
                For i1 = 0 To subMotifs.Count - 1
                    Dim s = subMotifs(i1)
                    If s.Length <> 0 Then
                        If SingleMatch.Contains(i1) = False Then
                            out.Append("[").Append(s).Append("]")
                        Else
                            out.Append(s)
                        End If
                    End If
                Next
                Return out.ToString


            End Function
        End Class
    End Namespace
End Namespace

