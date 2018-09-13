Imports System.IO

Namespace Szunyi
    Namespace Other_Database
        Namespace Affy
            Public Class ParseAffy
                Public Property Type As String = MyConstants.BackGroundWork.AffyMapping
                Public Property Seqs As List(Of Bio.Sequence)
                Public Property AffyProbes As AffyProbes
                Public Property MappigResultByGene As New List(Of AffyParsingResultBySeq)
                Public Property MappigResultByAffyProbeID As New List(Of AffyParsingResultByAffyID)
                Private UsedAffyIds As New List(Of String)
                Public Sub New(Seqs As List(Of Bio.Sequence), AffyProbes As AffyProbes)
                    Me.Seqs = Seqs
                    Me.AffyProbes = AffyProbes
                End Sub
                Public Sub DoIt()
                    For Each Seq In Seqs
                        Dim Kmers = Szunyi.Sequences.KmerManipulation.GetAllKmer(Seq, 25)
                        Dim Res As New AffyParsingResultBySeq(Seq)
                        For Each Kmer In Kmers
                            Dim Index = AffyProbes.AffyProbesBySeq.BinarySearch(New AffyProbeBySeq(Kmer), AllComparares.Affy.AffyProbesBySeq)
                            If Index > -1 Then
                                Res.AddAffyIDs(AffyProbes.AffyProbesBySeq(Index).AffyIDs)
                                UsedAffyIds.AddRange(AffyProbes.AffyProbesBySeq(Index).AffyIDs)
                            End If
                        Next
                        If Res.AffyIDswCounts.Count > 0 Then MappigResultByGene.Add(Res)
                    Next

                    Dim dUsedAffys = Szunyi.Text.GetDictinctList(UsedAffyIds)

                    For Each ID In dUsedAffys
                        Me.MappigResultByAffyProbeID.Add(New AffyParsingResultByAffyID(ID))
                    Next

                    For Each AffyParsingResultByGene In Me.MappigResultByGene
                        For Each ByGeneAffyIDwCounts In AffyParsingResultByGene.AffyIDswCounts
                            Dim Index = Me.MappigResultByAffyProbeID.BinarySearch(New AffyParsingResultByAffyID(ByGeneAffyIDwCounts.AffyID),
                                        AllComparares.Affy.AffyProbesByID)

                            Dim Has As Boolean = False
                            For Each AffyIDwCount In Me.MappigResultByAffyProbeID(Index).AffyIDswCounts
                                If AffyIDwCount.AffyID = AffyParsingResultByGene.Seq.ID Then
                                    AffyIDwCount.Count += 1
                                    Has = True
                                    Exit For
                                End If
                            Next
                            If Has = False Then
                                Me.MappigResultByAffyProbeID(Index).AffyIDswCounts.Add(
                                   New AffyIDwCount(AffyParsingResultByGene.Seq.ID, ByGeneAffyIDwCounts.Count))

                            End If

                        Next
                    Next
                End Sub
                Public Function GetResultBySeqID(Optional MinNofProbes As Integer = 5)
                    Dim str As New System.Text.StringBuilder
                    For Each Item In MappigResultByGene
                        For Each AffyID In Item.AffyIDswCounts
                            If AffyID.Count >= MinNofProbes Then
                                str.Append(Item.Seq.ID).Append(vbTab)
                                str.Append(AffyID.AffyID).Append(vbTab).Append(AffyID.Count).AppendLine()
                            End If

                        Next
                    Next
                    If str.Length >= vbCrLf.Length Then str.Length -= vbCrLf.Length
                    Return str.ToString
                End Function
                Public Function GetResultByAffyID(Optional MinNofProbes As Integer = 5)
                    Dim str As New System.Text.StringBuilder
                    For Each Item In Me.MappigResultByAffyProbeID
                        For Each AffyID In Item.AffyIDswCounts
                            If AffyID.Count >= MinNofProbes Then
                                str.Append(Item.AffyID).Append(vbTab)
                                str.Append(AffyID.AffyID).Append(vbTab).Append(AffyID.Count).AppendLine()
                            End If

                        Next
                    Next
                    If str.Length >= vbCrLf.Length Then str.Length -= vbCrLf.Length
                    Return str.ToString
                End Function
            End Class
            Public Class AffyParsingResultBySeq
                Public Property Seq As Bio.Sequence
                Public Property AffyIDswCounts As New List(Of AffyIDwCount)
                Public Sub New(Seq As Bio.Sequence)
                    Me.Seq = Seq
                End Sub
                Public Sub AddAffyIDs(AffyIDs As List(Of String))

                    For Each AffyID In AffyIDs
                        Dim Has As Boolean = False
                        For Each Item In Me.AffyIDswCounts
                            If Item.AffyID = AffyID Then
                                Item.Count += 1
                                Has = True
                            End If
                        Next
                        If Has = False Then Me.AffyIDswCounts.Add(New AffyIDwCount(AffyID))
                    Next

                End Sub
            End Class
            Public Class AffyParsingResultByAffyID
                Public Property AffyID As String
                Public Property AffyIDswCounts As New List(Of AffyIDwCount)
                Public Sub New(Id As String)
                    Me.AffyID = Id
                End Sub
                Public Sub AddAffyIDs(AffyIDs As List(Of String))

                    For Each ID In AffyIDs
                        Dim Has As Boolean = False
                        For Each Item In Me.AffyIDswCounts
                            If Item.AffyID = ID Then
                                Item.Count += 1
                                Has = True
                            End If
                        Next
                        If Has = False Then Me.AffyIDswCounts.Add(New AffyIDwCount(ID))
                    Next

                End Sub
            End Class
            Public Class AffyIDwCount
                Public Property AffyID As String
                Public Property Count As Integer
                Public Sub New(AffyID)
                    Me.AffyID = AffyID
                    Me.Count = 1
                End Sub
                Public Sub New(AffyID As String, Count As Integer)
                    Me.AffyID = AffyID
                    Me.Count = Count
                End Sub
            End Class
            Public Class AffyProbes
                Private AffyProbes As New List(Of AffyProbe)
                Public Property AffyProbesByIDs As New List(Of AffyProbeByID)
                Public Property AffyProbesBySeq As New List(Of AffyProbeBySeq)
                Public Sub New(File As FileInfo, FirstLine As Integer, IdCol As Integer, SeqIDCol As Integer)
                    Dim Lines = Szunyi.IO.Import.Text.ReadLines(File, Nothing)
                    For i1 = 1 To Lines.Count - 1
                        AffyProbes.Add(New AffyProbe(Lines(i1), IdCol, SeqIDCol))
                    Next
                    Build()

                End Sub

                Private Sub Build()
                    AffyProbes.Sort(AllComparares.Affy.AffyByID)
                    Dim currAffyID = AffyProbes.First.ID
                    Me.AffyProbesByIDs.Add(New AffyProbeByID(currAffyID))
                    For Each AffyProbe In AffyProbes
                        If AffyProbe.ID <> currAffyID Then _
                            Me.AffyProbesByIDs.Add(New AffyProbeByID(AffyProbe.ID))
                        currAffyID = AffyProbe.ID
                        Me.AffyProbesByIDs.Last.Seqs.Add(AffyProbe.Seq)
                    Next

                    AffyProbes.Sort(AllComparares.Affy.AffyBySeq)
                    Dim CurrAffySeq = AffyProbes.First.Seq
                    Me.AffyProbesBySeq.Add(New AffyProbeBySeq(CurrAffySeq))
                    For Each AffyProbe In AffyProbes
                        If AffyProbe.Seq <> CurrAffySeq Then _
                            Me.AffyProbesBySeq.Add(New AffyProbeBySeq(AffyProbe.Seq))

                        CurrAffySeq = AffyProbe.Seq
                        Me.AffyProbesBySeq.Last.AffyIDs.Add(AffyProbe.ID)
                    Next
                End Sub

                ''' <summary>
                ''' Import FastaFile
                ''' </summary>
                ''' <param name="File"></param>
                Public Sub New(File As FileInfo)

                End Sub
            End Class
            ''' <summary>
            ''' This Class Contains IDs And Sequnces
            ''' </summary>
            Public Class AffyProbe
                Public Property ID As String
                Public Property Seq As String
                Public Sub New(Line As String, IdCol As Integer, SeqIDCOl As Integer)
                    Dim s1() As String = Split(Line, vbTab)
                    Me.ID = s1(IdCol)
                    Me.Seq = s1(SeqIDCOl).ToUpper
                End Sub
            End Class

            ''' <summary>
            ''' This Class Contains AffyTarget And Corresponding Sequneces
            ''' </summary>
            Public Class AffyProbeByID
                Public Property ID As String
                Public Property Seqs As New List(Of String)
                Public Sub New(ID As String)
                    Me.ID = ID
                End Sub
            End Class

            ''' <summary>
            ''' Thic Class Contains Sequence and Corresponding AffyIDs
            ''' </summary>
            Public Class AffyProbeBySeq
                Public Property Seq As String
                Public Property AffyIDs As New List(Of String)
                Public Sub New(Seq As String)
                    Me.Seq = Seq
                End Sub
            End Class
        End Namespace
    End Namespace
End Namespace

