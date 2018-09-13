Imports System.Text
Imports Bio
Imports ClassLibrary1.Szunyi.Protein

Namespace Szunyi
    Namespace Sequence_Analysis
        Public Class Sequence_Ranges_With_Values

            Public Sub New(Motifs As List(Of SequenceRange),
                           Parameter As Object,
                           Threshold As Double, Values As List(Of Double))
                Me.Motifs = Motifs
                Me.Paramaters = Parameter
                Me.Threshold = Threshold
                Me.Values = Values
            End Sub

            Public Property Paramaters As Object
            Public Property Motifs As List(Of Bio.SequenceRange)
            Public Property Values As List(Of Double)
            Public Property Threshold As Double
        End Class
        Public Class Sequence_Ranges
            Public Property Seq As Bio.Sequence
            Public Property Analysis As New Dictionary(Of String, Sequence_Ranges_With_Values)
            Public Sub New(Seq As Bio.Sequence)
                Me.Seq = Seq
            End Sub
        End Class
        Public Class Sequences_Ranges
            Public Property Seqs As List(Of Bio.ISequence)
            Public Property Ranges As new List(Of Sequence_Ranges)
            Public Property Title As String
            Public Property UniqueID As Integer
            Public Sub New(Seqs As Szunyi.ListOf.SequenceList, Type As String)
                Me.Title = "SA & " & Type & " " & Seqs.ShortFileName
                Me.Seqs = Seqs.Sequences
                Me.Seqs.Sort(Szunyi.Comparares.AllComparares.BySeqIDAndLength)
                For Each Seq In Me.Seqs
                    Me.Ranges.Add(New Sequence_Ranges(Seq))
                Next
            End Sub

            Public Function Get_Slices_Details(Slices As List(Of SequenceRange)) As String
                Dim str As New StringBuilder
                Dim str2 As New System.Text.StringBuilder
                For Each Slice In Slices
                    str.Append(Slice.Start).Append(vbTab).Append(Slice.End).Append(vbTab).Append(Slice.End - Slice.Start + 1)
                    str2.Append(Slice.Start + 1).Append("..").Append(Slice.End).Append(",")
                Next
                If str2.Length > 0 Then str.Length -= 1
                str.Append(vbTab)
                str.Append("(").Append(str2.ToString).Append(")")
                Return str.ToString
            End Function

            Public Shared Sub Save(SsRs As Sequences_Ranges)
                Dim I_W_Ps As New Szunyi.Text.TableManipulation.Items_With_Properties(SsRs.Seqs)
                Dim x As New Szunyi.Constants.Sequence_Analysis
                For Each AnalysisName In x.All
                    I_W_Ps.ItemHeaders.Add(AnalysisName & " Nof Disorders")
                    I_W_Ps.ItemHeaders.Add(AnalysisName & " AAs Counts in Disorders")
                    I_W_Ps.ItemHeaders.Add(AnalysisName & " AAs Percents in Disorders")
                    I_W_Ps.ItemHeaders.Add(AnalysisName & " Disorders Positions")
                    I_W_Ps.ItemHeaders.Add(AnalysisName & " Value")
                    For Each SRs In SsRs.Ranges
                        Dim Index = I_W_Ps.Get_Index(SRs.Seq.ID, Szunyi.Constants.TextMatch.Exact)
                        If SRs.Analysis.ContainsKey(AnalysisName) = True Then
                            Dim Anal = SRs.Analysis(AnalysisName)
                            I_W_Ps.Items(Index).Properties.Add(Anal.Motifs.Count)
                            Dim NofAA_In_Motifs = Get_Nof_AAs_In_MOtifs(Anal.Motifs)
                            I_W_Ps.Items(Index).Properties.Add(NofAA_In_Motifs)
                            I_W_Ps.Items(Index).Properties.Add((NofAA_In_Motifs / Szunyi.Sequences.SequenceManipulation.Counts.Get_Sequence_Length_Wo_Ter_And_Gap_Symbols(SRs.Seq)) * 100)
                            I_W_Ps.Items(Index).Properties.Add(Get_Positions_From_Motifs(Anal.Motifs))
                            I_W_Ps.Items(Index).Properties.Add(Get_Values_From_Motifs(Anal))
                        Else
                            I_W_Ps.Items(Index).Properties.Add("")
                            I_W_Ps.Items(Index).Properties.Add("")
                            I_W_Ps.Items(Index).Properties.Add("")
                            I_W_Ps.Items(Index).Properties.Add("")
                            I_W_Ps.Items(Index).Properties.Add("")
                        End If

                    Next
                Next
                I_W_Ps.Save_With_ID()
                Dim str As New System.Text.StringBuilder


                For Each Seq In SsRs.Ranges
                    str.Append(Seq.Seq.ID).AppendLine()
                    str.AppendLine(Seq.Seq.ConvertToString).AppendLine()
                    For Each Motif In Seq.Analysis
                        str.Append(Get_Motifs_As_String(Motif.Value)).AppendLine()
                    Next

                Next
                Szunyi.IO.Export.SaveText(str.ToString, "Save as Alignment File")
            End Sub
            Private Shared Function Get_Motifs_As_String(x As Sequence_Ranges_With_Values) As String
                Dim str As New System.Text.StringBuilder
                Dim s = Szunyi.Text.General.Multiply(" ", x.Values.Count).ToCharArray
                For Each motif In x.Motifs
                    For i1 = motif.Start To motif.End
                        s(i1) = "X"
                    Next
                Next
                Return New String(s)
            End Function
            Private Shared Function Get_Nof_AAs_In_Motifs(Motifs As List(Of SequenceRange)) As Integer
                Dim Nof_AA As Integer = 0
                For Each Motif In Motifs
                    Nof_AA += Motif.Length + 1
                Next
                Return Nof_AA
            End Function
            Private Shared Function Get_Positions_From_Motifs(Motifs As List(Of SequenceRange)) As String
                Dim str As New System.Text.StringBuilder

                For Each Slice In Motifs
                    str.Append(Slice.Start + 1).Append("..").Append(Slice.End + 1).Append(",")
                Next
                If str.Length > 0 Then str.Length -= 1
                Return str.ToString
            End Function
            Private Shared Function Get_Values_From_Motifs(SR_w_Values As Sequence_Ranges_With_Values) As Double
                Dim d As Double = 0
                For Each Motif In SR_w_Values.Motifs
                    For i1 = Motif.Start To Motif.End
                        d += SR_w_Values.Values(i1) '- SR_w_Values.Threshold
                    Next
                Next
                Return d
            End Function

        End Class

        Public Class Slices
#Region "Slices"
            Public Shared Function Get_Slices(dN_Datas As List(Of Double), fold As Double, join_frame As Integer,
                                        peak_frame As Integer, expect_val As Double) As List(Of Bio.SequenceRange)

                Dim Basic_Slices = Get_Basic_Slices(dN_Datas, fold, expect_val)

                Dim Joined_slices = Get_Joined_Slices(Basic_Slices, join_frame)

                Dim Good_Lengths_Slices = From x In Joined_slices Where x.End - x.Start >= peak_frame - 1
                If Good_Lengths_Slices.Count > 0 Then
                    Return Good_Lengths_Slices.ToList
                End If


                Return New List(Of Bio.SequenceRange)
            End Function

            Friend Shared Function Get_Slices(dN_Datas As List(Of Double),
                                              Parameter As DisEMBL_Parameters) As List(Of SequenceRange)
                Dim Basic_Slices = Get_Basic_Slices(dN_Datas, Parameter.fold, Parameter.Expect_Value)

                Dim Joined_slices = Get_Joined_Slices(Basic_Slices, Parameter.join_frame)

                Dim Good_Lengths_Slices = From x In Joined_slices Where x.End - x.Start >= Parameter.peak_frame - 1
                If Good_Lengths_Slices.Count > 0 Then
                    Return Good_Lengths_Slices.ToList
                End If


                Return New List(Of Bio.SequenceRange)
            End Function

            Public Shared Function Get_Slices(dN_Datas As List(Of Double), ThresHold As Double, join_frame As Integer,
peak_frame As Integer, op As String) As List(Of Bio.SequenceRange)

                Dim Basic_Slices = Get_Basic_Slices(dN_Datas, ThresHold, op)

                Dim Joined_slices = Get_Joined_Slices(Basic_Slices, join_frame)

                Dim Good_Lengths_Slices = From x In Joined_slices Where x.End - x.Start >= peak_frame - 1
                If Good_Lengths_Slices.Count > 0 Then
                    Return Good_Lengths_Slices.ToList
                End If


                Return New List(Of Bio.SequenceRange)
            End Function
            Public Shared Function Get_Slices_Above_Threshold(dN_Datas As List(Of Double), ThresHold As Double, join_frame As Integer,
                                        peak_frame As Integer) As List(Of Bio.SequenceRange)

                Dim Basic_Slices = Get_Basic_Slices_Above_Threshold(dN_Datas, ThresHold)

                Dim Joined_slices = Get_Joined_Slices(Basic_Slices, join_frame)

                Dim Good_Lengths_Slices = From x In Joined_slices Where x.End - x.Start >= peak_frame - 1
                If Good_Lengths_Slices.Count > 0 Then
                    Return Good_Lengths_Slices.ToList
                End If
                
                Return New List(Of Bio.SequenceRange)
            End Function

            Public Shared Function Get_Slices_Below_Threshold(dN_Datas As List(Of Double), ThresHold As Double, join_frame As Integer,
                                        peak_frame As Integer) As List(Of Bio.SequenceRange)

                Dim Basic_Slices = Get_Basic_Slices_Below_Threshold(dN_Datas, ThresHold)

                Dim Joined_slices = Get_Joined_Slices(Basic_Slices, join_frame)

                Dim Good_Lengths_Slices = From x In Joined_slices Where x.End - x.Start >= peak_frame - 1
                If Good_Lengths_Slices.Count > 0 Then
                    Return Good_Lengths_Slices.ToList
                End If

                Return New List(Of Bio.SequenceRange)
            End Function
            Public Shared Function Get_Joined_Slices(basic_Slices As List(Of SequenceRange),
                                               join_frame As Integer) As List(Of Bio.SequenceRange)
                Dim Slices As New List(Of Bio.SequenceRange)
                Dim Good_Slices As New List(Of Bio.SequenceRange)
                For i1 = 0 To basic_Slices.Count - 1
                    If i1 + 1 <> basic_Slices.Count AndAlso basic_Slices(i1 + 1).Start - basic_Slices(i1).End <= join_frame Then
                        Good_Slices.Add(New Bio.SequenceRange("a", basic_Slices(i1).Start, basic_Slices(i1 + 1).End))
                        If i1 + 1 = basic_Slices.Count - 1 Then
                            Exit For
                        End If
                        i1 += 1
                    Else
                        Good_Slices.Add(basic_Slices(i1))
                    End If
                Next
                If basic_Slices.Count = Good_Slices.Count Then
                    Return Good_Slices
                Else
                    Good_Slices = Get_Joined_Slices(Good_Slices, join_frame)
                End If
                Return Good_Slices
            End Function

            Public Shared Function Get_Basic_Slices(dN_Datas As List(Of Double),
                                              fold As Double,
                                              expect_val As Double) As List(Of Bio.SequenceRange)
                Dim BeginSlice As Integer = 0
                Dim EndSlice As Integer = 0
                Dim maxSlice As Double = 0
                Dim inSlice As Boolean = False
                Dim Slices As New List(Of Bio.SequenceRange)
                For i1 = 0 To dN_Datas.Count - 1
                    If inSlice = True Then
                        If dN_Datas(i1) < expect_val Then
                            If maxSlice >= fold * expect_val Then
                                Slices.Add(New Bio.SequenceRange("a", BeginSlice, EndSlice))

                            End If
                            inSlice = False
                        Else
                            EndSlice += 1
                            If dN_Datas(i1) > maxSlice Then
                                maxSlice = dN_Datas(i1)
                            End If
                        End If

                    ElseIf dN_Datas(i1) >= expect_val Then
                        BeginSlice = i1
                        EndSlice = i1
                        inSlice = True
                        maxSlice = dN_Datas(i1)

                    End If

                Next
                If inSlice = True And maxSlice >= fold * expect_val Then
                    Slices.Add(New Bio.SequenceRange("A", BeginSlice, EndSlice))
                End If
                Return Slices
            End Function

            Public Shared Function Get_Basic_Slices_Above_Threshold(dN_Datas As List(Of Double),
                                              ThresHold As Double) As List(Of Bio.SequenceRange)
                Dim BeginSlice As Integer = 0
                Dim EndSlice As Integer = 0
                Dim inSlice As Boolean = False
                Dim Slices As New List(Of Bio.SequenceRange)
                For i1 = 0 To dN_Datas.Count - 1
                    If inSlice = True Then
                        If dN_Datas(i1) < ThresHold Then
                            Slices.Add(New Bio.SequenceRange("a", BeginSlice, EndSlice))
                            inSlice = False
                        Else
                            EndSlice += 1
                        End If

                    Else

                        If dN_Datas(i1) > ThresHold Then
                            BeginSlice = i1
                            EndSlice = i1
                            inSlice = True
                        End If

                    End If
                Next
                If inSlice = True Then
                    If dN_Datas.Last > ThresHold Then
                        Slices.Add(New Bio.SequenceRange("a", BeginSlice, EndSlice))
                    End If
                End If

                Return Slices
            End Function

            Public Shared Function Get_Basic_Slices_Below_Threshold(dN_Datas As List(Of Double),
                                              ThresHold As Double) As List(Of Bio.SequenceRange)
                Dim BeginSlice As Integer = 0
                Dim EndSlice As Integer = 0
                Dim inSlice As Boolean = False
                Dim Slices As New List(Of Bio.SequenceRange)
                For i1 = 0 To dN_Datas.Count - 1
                    If inSlice = True Then
                        If dN_Datas(i1) > ThresHold Then
                            Slices.Add(New Bio.SequenceRange("a", BeginSlice, EndSlice))
                            inSlice = False
                        Else
                            EndSlice += 1
                        End If
                    Else
                        If dN_Datas(i1) < ThresHold Then
                            BeginSlice = i1
                            EndSlice = i1
                            inSlice = True
                        End If

                    End If
                Next
                If inSlice = True Then
                    If dN_Datas.Last > ThresHold Then
                        Slices.Add(New Bio.SequenceRange("a", BeginSlice, EndSlice))
                    End If
                End If

                Return Slices
            End Function

            Public Shared Function Get_NofAA_In_Slices(Slices As List(Of Bio.SequenceRange)) As Integer
                Dim out As Integer = 0
                For Each Slice In Slices
                    out += Slice.End - Slice.Start
                Next
                Return out
            End Function
#End Region
        End Class
    End Namespace
End Namespace
