Imports System.IO

Namespace Szunyi
    Namespace Protein
        Namespace Membrane_Topology
            Public Enum Predictor
                TOPCONS = 1
                OCTOPUS = 2
                Philius = 3
                PolyPhobius = 4
                SCAMPI = 5
                SPOCTOPUS = 6
                TMHMM = 7
            End Enum
            Public Class C
                Public Const Membrane = "Membrane"
                Public Const Inside = "Inside"
                Public Const OutSide = "Outside"
                Public Const Signal = "Signal"
            End Class

            Public Class Membrane_Topotolgy_Result
                Public Property Seq As Bio.Sequence
                Public Property Motifs As New List(Of Bio.SequenceRange)
                Public Property Insides As New List(Of Bio.SequenceRange)
                Public Property OutSides As New List(Of Bio.SequenceRange)
                Public Property TMHELIX As New List(Of Bio.SequenceRange)
                Public Property Signals As New List(Of Bio.SequenceRange)
                Public Property Predictor As Membrane_Topology.Predictor
                Public Sub New(Seq As Bio.Sequence)
                    Me.Seq = Seq
                End Sub
                Public Sub New(Seq As Bio.Sequence, s As String, Predicor As Membrane_Topology.Predictor)
                    Me.Seq = Seq
                    Me.Predictor = Predicor
                    Me.Motifs = TMHMM_Manipulation.GetRanges(s)
                    For Each Motif In Motifs
                        Select Case Motif.ID
                            Case Membrane_Topology.C.Signal
                                Me.Signals.Add(Motif)
                            Case Membrane_Topology.C.Inside
                                Me.Insides.Add(Motif)
                            Case Membrane_Topology.C.OutSide
                                Me.OutSides.Add(Motif)
                            Case Membrane_Topology.C.Membrane
                                Me.TMHELIX.Add(Motif)
                        End Select
                    Next
                End Sub
                Public Sub Add(x As Bio.SequenceRange)
                    Motifs.Add(x)
                    Select Case x.ID
                        Case "outside"
                            Me.OutSides.Add(x)
                        Case "TMhelix"
                            Me.TMHELIX.Add(x)
                        Case "inside"
                            Me.Insides.Add(x)
                    End Select
                End Sub

            End Class

            Public Class TopScon
                Public Property Files As List(Of FileInfo)
                Public Property Seqs As New List(Of Bio.Sequence)
                Public Property Topologies As New List(Of Membrane_Topotolgy_Result)
                Public Sub New(Files As List(Of FileInfo))
                    Me.Files = Files
                    DoIt()
                End Sub
                Public Sub DoIt()
                    For Each File In Files
                        For Each Lines In Szunyi.IO.Import.Text.Read_Lines_by_Group(File, "Sequence name")
                            If Lines(0).StartsWith("Sequence name: ") Then
                                Topologies.AddRange(Get_TopologiesOfSeq(Lines))
                            End If
                        Next
                    Next

                End Sub
                Private Function Get_TopologiesOfSeq(lines As List(Of String)) As List(Of Membrane_Topotolgy_Result)
                    Dim out As New List(Of Membrane_Topotolgy_Result)
                    Dim Seq = New Bio.Sequence(Bio.Alphabets.AmbiguousProtein, lines(3))
                    Seq.ID = lines(0).Split(":").Last.Trim

                    For i1 = 5 To lines.Count - 1
                        Select Case lines(i1)
                            Case "TOPCONS predicted topology:"
                                Dim x As New Membrane_Topotolgy_Result(Seq, lines(i1 + 1), Membrane_Topology.Predictor.TOPCONS)
                                out.Add(x)
                            Case "OCTOPUS predicted topology:"
                                Dim x As New Membrane_Topotolgy_Result(Seq, lines(i1 + 1), Membrane_Topology.Predictor.OCTOPUS)
                                out.Add(x)
                            Case "Philius predicted topology:"
                                Dim x As New Membrane_Topotolgy_Result(Seq, lines(i1 + 1), Membrane_Topology.Predictor.Philius)
                                out.Add(x)
                            Case "PolyPhobius predicted topology:"
                                Dim x As New Membrane_Topotolgy_Result(Seq, lines(i1 + 1), Membrane_Topology.Predictor.PolyPhobius)
                                out.Add(x)
                            Case "SCAMPI predicted topology:"
                                Dim x As New Membrane_Topotolgy_Result(Seq, lines(i1 + 1), Membrane_Topology.Predictor.SCAMPI)
                                out.Add(x)
                            Case "SPOCTOPUS predicted topology:"
                                Dim x As New Membrane_Topotolgy_Result(Seq, lines(i1 + 1), Membrane_Topology.Predictor.SPOCTOPUS)
                                out.Add(x)
                            Case "Homology:"
                                Exit For
                        End Select
                    Next
                    Return out
                End Function
            End Class

            Public Class TMHMM
                Public Property Files As List(Of FileInfo)
                Public Property Seqs As New List(Of Bio.Sequence)
                Public Property Topologies As New List(Of Membrane_Topotolgy_Result)
                Public Sub New(Files As List(Of FileInfo), Seqs As List(Of Bio.Sequence))
                    Me.Files = Files
                    Me.Seqs = Seqs
                    Dim tmpSeq As New Bio.Sequence(Bio.Alphabets.DNA, "C")
                    For Each File In Files
                        Dim AllText = Szunyi.IO.Import.Text.ReadToEnd(File)
                        Dim s1() = Split(AllText, "------------------------------------------------------------------------")
                        For i1 = 1 To s1.Count - 2
                            Dim s2() = Split(s1(i1), vbCrLf)
                            Dim ImportantLines As List(Of String) = Szunyi.Text.Lists.Ignore_Empty_Or_StartWiths(s2.ToList, "#")
                            Dim SeqID = ImportantLines.First.Split(vbTab).First
                            tmpSeq.ID = SeqID
                            Dim Index = Seqs.BinarySearch(tmpSeq, Szunyi.Comparares.AllComparares.BySeqID)
                            If Index > -1 Then
                                Dim TheSeq = Seqs(Index)
                                Dim x As New Membrane_Topology.Membrane_Topotolgy_Result(TheSeq)
                                For Each Line In ImportantLines
                                    Dim Vals = Split(Line, vbTab)
                                    Dim SE = Split(Vals(3), " ")
                                    Dim IS1 = Szunyi.Text.Lists.Ignore_Empty_Or_StartWiths(SE.ToList, "p")
                                    Dim sp As New Bio.SequenceRange(Vals(2), IS1.First - 1, IS1.Last - 1)
                                    Select Case (Vals(2))
                                        Case "inside"
                                            sp.ID = Membrane_Topology.C.Inside
                                        Case "TMhelix"
                                            sp.ID = Membrane_Topology.C.Membrane
                                        Case "outside"
                                            sp.ID = Membrane_Topology.C.OutSide
                                    End Select
                                    x.Add(sp)
                                Next
                                Me.Topologies.Add(x)
                            End If
                        Next
                    Next
                    '  RR_Analysis(TMHMM_Results)

                End Sub

            End Class
        End Namespace

        Public Class TMHMM_Manipulation
            Public Property Files As List(Of FileInfo)
            Public Property Seqs As List(Of Bio.Sequence)
            Public Property TMHMM_Results As New List(Of Membrane_Topology.Membrane_Topotolgy_Result)
            Public Shared Function GetRanges(s As String) As List(Of Bio.SequenceRange)
                Dim out As New List(Of Bio.SequenceRange)
                Dim currChar = s.Substring(0, 1)
                Dim currStartPos As Integer = 0
                For i1 = 1 To s.Count - 1
                    If currChar <> s.Substring(i1, 1) Then

                        out.Add(New Bio.SequenceRange(GetMotifType(currChar), currStartPos, i1 - 1))
                        currStartPos = i1
                        currChar = s.Substring(i1, 1)
                    End If
                Next
                out.Add(New Bio.SequenceRange(GetMotifType(currChar), currStartPos, s.Count - 1))
                Return out
            End Function
            Private Shared Function GetMotifType(s As String) As String
                Select Case s
                    Case "S"
                        Return Membrane_Topology.C.Signal
                    Case "i"
                        Return Membrane_Topology.C.Inside
                    Case "o"
                        Return Membrane_Topology.C.OutSide
                    Case "M"
                        Return Membrane_Topology.C.Membrane
                End Select
                Return String.Empty
            End Function


            Public Shared Function HelixBrokensIn_Out_Helixes(TM_Results As List(Of Membrane_Topology.Membrane_Topotolgy_Result))
                Dim Has_TM_Helix = Get_Has_Helix(TM_Results)

                Dim Helix_Brokeners As New List(Of String)
                Helix_Brokeners.Add("C")
                Helix_Brokeners.Add("S")
                Helix_Brokeners.Add("N")
                Helix_Brokeners.Add("G")
                Dim Has_Brokeners_In_TM_in_out As New List(Of Bio.Sequence)
                For Each Seq In Has_TM_Helix
                    Dim HasBrokeners As Boolean = False
                    For Each TmSeq As Bio.Sequence In Get_Inside_OutSide_TMs(Seq)
                        Dim s = TmSeq.ConvertToString
                        For Each HB In Helix_Brokeners
                            If s.Contains(HB) Then
                                HasBrokeners = True
                                Exit For
                            End If
                        Next
                        If HasBrokeners = True Then Exit For

                    Next
                    If HasBrokeners = True Then
                        Has_Brokeners_In_TM_in_out.Add(Seq.Seq)
                    End If
                Next
                '  Szunyi.IO.Export.SaveSequencesToSingleFasta(Has_Brokeners_In_TM_in_out)
                Return Has_Brokeners_In_TM_in_out

            End Function
            Public Sub RR_Analysis(TM_Results As List(Of Membrane_Topology.Membrane_Topotolgy_Result))
                Dim Has_TM_Helix = Get_Has_Helix(Me.TMHMM_Results)
                Dim NTerms_Inside = Get_First_Insides(Has_TM_Helix)
                Dim NtermSeqs = Get_First_Motifs_Sequences(NTerms_Inside)
                Dim NtermsSegsFirst25 = Szunyi.Sequences.SequenceManipulation.GetSequences.ByStartAndLength(NtermSeqs, 0, 24)

                Dim IWPs As New Szunyi.Text.TableManipulation.Items_With_Properties(NtermSeqs)
                Dim IWPs25bp As New Szunyi.Text.TableManipulation.Items_With_Properties(NtermsSegsFirst25)
                Dim Motifs As New List(Of String)
                Motifs.Add("RR")
                Motifs.Add("R[A-Z]{1,2}R")
                Motifs.Add("KK")
                Motifs.Add("K[A-Z]{1}K")
                AddMotifsCOunts(Motifs, IWPs)
                AddMotifsCOunts(Motifs, IWPs25bp)
                IWPs.Merge(IWPs25bp)
                IWPs.Save_With_ID()
            End Sub
            Private Sub AddMotifsCOunts(Motifs As List(Of String), ByRef IWPs As Szunyi.Text.TableManipulation.Items_With_Properties)
                Dim TotalMotifsCounts(IWPs.Items.Count) As Double
                For Each Motif In Motifs
                    Dim Values As New List(Of Double)
                    For i1 = 0 To IWPs.Items.Count - 1
                        Dim Item = IWPs.Items(i1)

                        Dim tmp = Szunyi.Text.Regexp.Find_Motifs(Item.Properties(1), Motif)
                        If IsNothing(tmp) = True Then
                            Values.Add(0)
                        Else
                            Values.Add(tmp.Count)
                        End If
                        TotalMotifsCounts(i1) += Values.Last
                    Next
                    IWPs.Add_Values_WithOut_Keys(Motif, Values)
                Next
                IWPs.Add_Values_WithOut_Keys("Total", TotalMotifsCounts)
            End Sub
            Private Shared Function Get_Inside_OutSide_TMs(x As Membrane_Topology.Membrane_Topotolgy_Result) As List(Of Bio.Sequence)
                Dim out As New List(Of Bio.Sequence)
                If x.Motifs.First.ID = Membrane_Topology.C.Inside Then
                    For i1 = 0 To x.TMHELIX.Count - 1 Step 2
                        Dim nS = x.Seq.GetSubSequenceByStartAndEnd(x.TMHELIX(i1).Start, x.TMHELIX(i1).End)
                        out.Add(nS)
                    Next
                Else
                    For i1 = 1 To x.TMHELIX.Count - 1 Step 2
                        Dim nS = x.Seq.GetSubSequenceByStartAndEnd(x.TMHELIX(i1).Start, x.TMHELIX(i1).End)
                        out.Add(nS)
                    Next
                End If
                Return out
            End Function
            Public Shared Function Get_By_Nof_Tm_Motifs(Results As List(Of Membrane_Topology.Membrane_Topotolgy_Result), Min_Nof As Integer, Max_Nof As Integer) As List(Of Membrane_Topology.Membrane_Topotolgy_Result)
                Dim res = From x In Results Where x.Motifs.Count >= Min_Nof AndAlso x.Motifs.Count <= Max_Nof

                If res.Count > 0 Then Return res.ToList

                Return New List(Of Membrane_Topology.Membrane_Topotolgy_Result)
            End Function


            Public Shared Function Get_First_Motifs_Sequences(Results As List(Of Membrane_Topology.Membrane_Topotolgy_Result)) As List(Of Bio.ISequence)
                Dim out As New List(Of Bio.ISequence)
                For Each Item In Results
                    Dim tmpSeq As Bio.Sequence = Item.Seq.GetSubSequence(0, Item.Motifs.First.End)
                    Dim x As New Bio.Sequence(Bio.Alphabets.AmbiguousProtein, tmpSeq.ConvertToString)
                    x.ID = Item.Seq.ID
                    out.Add(x)
                Next
                Return out
            End Function

            Public Shared Function Get_Has_Helix(Results As List(Of Membrane_Topology.Membrane_Topotolgy_Result)) As List(Of Membrane_Topology.Membrane_Topotolgy_Result)
                Dim res = From x In Results Where x.TMHELIX.Count > 0

                If res.Count > 0 Then Return res.ToList

                Return New List(Of Membrane_Topology.Membrane_Topotolgy_Result)

            End Function
            Public Shared Function Get_First_Insides(Results As List(Of Membrane_Topology.Membrane_Topotolgy_Result)) As List(Of Membrane_Topology.Membrane_Topotolgy_Result)
                Dim res = From x In Results Where x.Motifs.First.ID = Membrane_Topology.C.Inside

                If res.Count > 0 Then Return res.ToList

                Return New List(Of Membrane_Topology.Membrane_Topotolgy_Result)

            End Function
            Public Shared Function Get_First_OutSide(Results As List(Of Membrane_Topology.Membrane_Topotolgy_Result)) As List(Of Membrane_Topology.Membrane_Topotolgy_Result)
                Dim res = From x In Results Where x.Motifs.First.ID = Membrane_Topology.C.OutSide

                If res.Count > 0 Then Return res.ToList

                Return New List(Of Membrane_Topology.Membrane_Topotolgy_Result)

            End Function
        End Class

    End Namespace
End Namespace



