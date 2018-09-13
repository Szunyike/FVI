Imports Bio
Imports ClassLibrary1.Szunyi.Sequence_Analysis

Namespace Szunyi
    Namespace Protein
        Public Class AGGRESCAN
            Public Property AGGRESCAN_Values As New Dictionary(Of Byte, Double)
            Public HST = -0.02
            Public Sub New()
                Dim x As New Szunyi.Protein.AAs_Properties
                AGGRESCAN_Values = x.AA_Values(Szunyi.Constants.Protein.Hidrophobicity_Indexes.AggreScan).Values_By_AA
            End Sub
            Public Function Get_AGGRESCAN_Results(Seqs As List(Of Bio.Sequence)) As List(Of AGGRESCAN_Result)
                Dim out As New List(Of AGGRESCAN_Result)
                For Each Seq In Seqs
                    out.Add(Get_AGGRESCAN_Result(Seq))
                Next
                Return out
            End Function
            Public Function Get_Results(S_With_Motifs As Sequences_Ranges) As Sequences_Ranges

                For Each Item In S_With_Motifs.Ranges
                    Dim Anal As Szunyi.Sequence_Analysis.Sequence_Ranges_With_Values = Get_Analysis(Item)
                    Item.Analysis.Add(Szunyi.Constants.Sequence_Analysis.Disordered_by_AggreScan, Anal)

                Next
                Return S_With_Motifs

            End Function

            Private Function Get_Analysis(Item As Sequence_Ranges) As Sequence_Ranges_With_Values

                Dim Values = Get_A4v(Item.Seq).ToList
                Dim Motifs = Get_Hot_Spots(Item.Seq, Values)
                Dim x As New Szunyi.Sequence_Analysis.Sequence_Ranges_With_Values(Motifs, Nothing, -0.02, Values)
                Return x
            End Function

            Public Function Get_txt(AS_Results As List(Of AGGRESCAN_Result)) As String
                Dim str As New System.Text.StringBuilder
                str.Append("Seq.ID").Append(vbTab)
                str.Append("nof HSPs").Append(vbTab)
                str.Append("Normalised nof HSPs").Append(vbTab)
                str.Append("Total Aree of HSPs").Append(vbTab)

                str.Append("THSA Ar").AppendLine()

                For Each As_Result In AS_Results
                    str.Append(As_Result.Seq.ID).Append(vbTab)
                    str.Append(As_Result.Number_of_Hot_Spots).Append(vbTab)
                    str.Append(As_Result.Normalized_nHS_for_100_residues).Append(vbTab)
                    str.Append(As_Result.Total_Hot_Spot_Area).Append(vbTab)
                    str.Append(As_Result.THSA_per_residue).AppendLine()
                Next
                str.Length -= 2
                Return str.ToString

            End Function
            Public Function Get_AGGRESCAN_Result(seq As Sequence) As AGGRESCAN_Result
                Dim x As New AGGRESCAN_Result
                x.Seq = seq
                If seq.Count < 6 Then Return x

                x.a4v = Get_A4v(seq)
                Dim Hot_Spots = Get_Hot_Spots(seq, x.a4v)
                x.Area_of_the_profile_Above_Threshold = Szunyi.Math.Sum_Avarage.Get_Trapezoidal_Integration(x.a4v)
                Dim l = Szunyi.Math.Sum_Avarage.Get_Sum_Above_ThresHold(x.a4v, -0.02)

                x.a3v_Sequence_Average = Szunyi.Math.Sum_Avarage.Get_Sum(x.a4v) / x.a4v.Count + 0.02
                x.Number_of_Hot_Spots = Hot_Spots.Count
                x.Normalized_nHS_for_100_residues = (100 / x.a4v.Count) * x.Number_of_Hot_Spots
                Dim AAT = Szunyi.Math.Sum_Avarage.Get_Trapezoidal_Integration_ThresHold(x.a4v, -0.02)
                Dim TA = Szunyi.Math.Sum_Avarage.Get_Trapezoidal_Integration(x.a4v) + x.a4v.Count * 0.02

                x.Total_Hot_Spot_Area = Get_HSAs(Hot_Spots)
                Dim Length_of_Hot_Spots = Get_Length_of_Hot_Spots(Hot_Spots)
                x.THSA_per_residue = x.Total_Hot_Spot_Area / x.a4v.Count
                x.AAT_per_residue = AAT / x.a4v.Count
                Return x
            End Function
            Private Function Get_Length_of_Hot_Spots(Hot_Spots As List(Of Hot_Spot)) As Double
                Dim d As Double
                For Each Hot_Spot In Hot_Spots
                    d += Hot_Spot.Length
                Next
                Return d
            End Function
            Private Function Get_HSAs(Hot_Spots As List(Of Hot_Spot)) As Double
                Dim d As Double
                For Each Hot_Spot In Hot_Spots
                    d += Hot_Spot.HSA
                Next
                Return d
            End Function
            Private Function Get_Hot_Spots(seq As Sequence, a4v As List(Of Double)) As List(Of Bio.SequenceRange)
                Dim Out As New List(Of Bio.SequenceRange)
                For i1 = 0 To a4v.Count - 4
                    For i2 = i1 To a4v.Count - 1
                        If seq(i2) = AscW("P") Or a4v(i2) < Me.HST Then ' Not Accepted
                            If i2 - i1 > 4 Then

                                Out.Add(New SequenceRange(seq.ID, i1, i2 - 1))
                                i1 = i2
                                Exit For
                            End If
                            Exit For
                        ElseIf i2 = a4v.Count - 1 Then
                            If i2 - i1 > 3 Then
                                Out.Add(New SequenceRange(seq.ID, i1, i2))
                            End If
                            i1 = i2
                            Exit For
                        End If

                    Next
                Next
                Return Out
            End Function
            Private Function Get_Hot_Spots(seq As Sequence, a4v() As Double) As List(Of Hot_Spot)
                Dim Out As New List(Of Hot_Spot)
                For i1 = 0 To a4v.Count - 4
                    For i2 = i1 To a4v.Count - 1
                        If seq(i2) = AscW("P") Or a4v(i2) < Me.HST Then ' Not Accepted
                            If i2 - i1 > 4 Then
                                Out.Add(New Hot_Spot(i1, i2 - 1, a4v))
                                i1 = i2
                                Exit For
                            End If
                            Exit For
                        ElseIf i2 = a4v.Count - 1 Then
                            If i2 - i1 > 3 Then
                                Out.Add(New Hot_Spot(i1, i2, a4v))
                            End If
                            i1 = i2
                            Exit For
                        End If

                    Next
                Next
                Return Out
            End Function

            Private Function Get_A4v(seq As Sequence) As Double()
                Dim x As New List(Of Double)
                Dim Window As Integer = 0
                Select Case seq.Count
                    Case < 5
                        Return Nothing
                    Case < 75
                        Window = 5
                    Case < 175
                        Window = 7
                    Case < 300
                        Window = 9
                    Case Else
                        Window = 11
                End Select
                For i1 = 0 To seq.Count - 1
                    If Me.AGGRESCAN_Values.ContainsKey(seq(i1)) Then
                        x.Add(Me.AGGRESCAN_Values(seq(i1)))
                    End If
                Next


                x.Insert(0, -1.624)
                x.Add(-1.0855)
                Dim res As New List(Of Double)
                For i1 = 0 To x.Count - Window
                    Dim d As Double = 0
                    For i2 = i1 To i1 + Window - 1
                        d += x(i2)
                    Next
                    res.Add(d / Window)
                Next
                For i1 = 1 To (Window - 3) / 2
                    res.Insert(0, res(0))
                    res.Add(res.Last)
                Next
                Return res.ToArray
            End Function


        End Class
        Public Class AGGRESCAN_Result
            Public Property Seq As Bio.Sequence
            Public Property a3v_Sequence_Average As Double
            Public Property Number_of_Hot_Spots As Integer
            Public Property Normalized_nHS_for_100_residues As Double
            Public Property Area_of_the_profile_Above_Threshold As Double
            Public Property Total_Hot_Spot_Area As Double
            Public Property Total_Area As Double
            Public Property AAT_per_residue As Double
            Public Property THSA_per_residue As Double
            Public Property Normalized_a4v_Sequence_Sum_For_100_residue As Double

            Public Property a4v As Double()
            Public Property HSA As Double()
            Public Property NHSA() As Double()
            Public Property a4vAHS As Double()
            Public Property AATr As Double



        End Class

        Public Class Hot_Spot
            Public Property Start As Integer
            Public Property [End] As Integer
            Public Property Length As Integer
            Public Property HSA As Double
            Public Property a4vAHS As Double
            Public Property NHSA As Double
            Public Sub New(Start As Integer, [End] As Integer, a4v As Double())

                Me.Start = Start
                Me.End = [End]
                Me.Length = Me.End - Me.Start + 1
                Me.a4vAHS = Szunyi.Math.Sum_Avarage.Get_Sum(a4v, Start, [End]) / Me.Length
                Me.HSA = Szunyi.Math.Sum_Avarage.Get_Sum(a4v, Start, [End]) + Me.Length * 0.02
                Dim hsa2 = Szunyi.Math.Sum_Avarage.Get_Trapezoidal_Integration(a4v, Start, [End]) + Me.Length * 0.032
                Dim TR = Szunyi.Math.Sum_Avarage.Get_Trapezoidal_Integration(a4v, Start, [End])
                Me.NHSA = Me.HSA / (Me.Length)
                Dim str As New System.Text.StringBuilder
                For i1 = 0 To a4v.Count - 1
                    str.Append(i1).Append(vbTab)
                    str.Append(a4v(i1)).AppendLine()
                Next
            End Sub

        End Class
    End Namespace

End Namespace

