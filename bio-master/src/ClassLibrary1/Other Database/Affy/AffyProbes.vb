Imports System.IO
Imports System.Threading
Imports System.Threading.Thread
Imports ClassLibrary1.Szunyi
Imports ClassLibrary1.Szunyi.Comparares

Namespace Szunyi
    Namespace Other_Database
        Namespace Affy
            Public Enum ExportFormat
                BySeqID_SingleLine
                BySeqID_MultipleLines
                ByAffyID_SingleLine
                ByAffyID_MultipleLines
                OnlyGeneIDs
            End Enum

            Public Class ParseAffy

                Public Property Type As String = Szunyi.Constants.BackGroundWork.Mapping
                Public Property SubType As String = Szunyi.Constants.BackGroundWork.AffyMapping
                Public Property Seqs As List(Of Bio.ISequence)
                Public Property AffyProbes As AffyProbes
                Public Property MappigResultByGene As New List(Of AffyParsingResultBySeq)
                Public Property MappigResultByAffyProbeID As New List(Of AffyParsingResultByAffyID)
                Public Property UniqueID As Integer
                Public Property ShortFileName As String
                Private UsedAffyIds As New List(Of String)
                Public Sub New(Seqs As List(Of Bio.ISequence), AffyProbes As AffyProbes)
                    Me.Seqs = Seqs
                    Me.AffyProbes = AffyProbes
                End Sub
                Public Sub New()

                End Sub
                Public Sub New(Files As List(Of FileInfo))
                    Dim ByAffyFiles = From x In Files Where x.Name.Contains("ByAffy SL5")

                    Dim BySeqFiles = From x In Files Where x.Name.Contains("BySeq SL5")

                    If ByAffyFiles.Count > 0 AndAlso BySeqFiles.Count > 0 Then
                        ImportByAffyIDs(ByAffyFiles.ToList)
                        ImportBySeqIDs(BySeqFiles.ToList)
                    End If
                    Me.ShortFileName = InputBox("Enter The Name Of Mapping")

                End Sub
                Private Sub ImportBySeqIDs(ByAffyFiles As List(Of FileInfo))
                    For Each File In ByAffyFiles
                        For Each Line In Szunyi.IO.Import.Text.Parse(File)
                            Dim s1() = Split(Line, vbTab)
                            Dim t = Split(s1.First, "|").First
                            Dim x As New AffyParsingResultBySeq(New Bio.Sequence(Bio.Alphabets.AmbiguousDNA, ""))
                            x.Seq.ID = t

                            For i1 = 1 To s1.Count - 1 Step 2

                                Dim y As New AffyIDwCount(s1(i1), s1(i1 + 1))
                                x.AffyIDswCounts.Add(y)
                            Next
                            Me.MappigResultByGene.Add(x)
                        Next
                    Next

                End Sub
                Private Sub ImportByAffyIDs(ByAffyFiles As List(Of FileInfo))
                    For Each File In ByAffyFiles
                        For Each Line In Szunyi.IO.Import.Text.Parse(File)
                            Dim s1() = Split(Line, vbTab)
                            Dim x As New AffyParsingResultByAffyID(s1.First)
                            For i1 = 1 To s1.Count - 1 Step 2
                                Dim t = Split(s1(i1), "|").First
                                Dim y As New AffyIDwCount(t, s1(i1 + 1))
                                x.AffyIDswCounts.Add(y)
                            Next
                            Me.MappigResultByAffyProbeID.Add(x)
                        Next
                    Next

                End Sub
                Public Sub DoIt()
                    Parallel.ForEach(Seqs, Sub(Seq As Bio.Sequence)
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
                                           End Sub)





                    Dim dUsedAffys = Szunyi.Text.General.GetDictinctList(UsedAffyIds)

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
                    For Each Item In Me.MappigResultByAffyProbeID
                        Item.AffyIDswCounts.Sort(AllComparares.Affy.ByCount)
                    Next
                    For Each Item In Me.MappigResultByGene
                        Item.AffyIDswCounts.Sort(AllComparares.Affy.ByCount)
                    Next
                End Sub


                ''' <summary>
                ''' Return The List Of Results, Also Where There is No Hit 
                ''' </summary>
                ''' <param name="affyIDs"></param>
                ''' <returns></returns>
                Public Function GetAffyIdsAndGenes(affyIDs As List(Of String)) As List(Of AffyParsingResultByAffyID)
                    Dim Res As New List(Of AffyParsingResultByAffyID)
                    For Each AffyID In affyIDs
                        Dim x As New AffyParsingResultByAffyID(AffyID)
                        Dim Index = Me.MappigResultByAffyProbeID.BinarySearch(x, AllComparares.Affy.AffyProbesByID)
                        If Index > -1 Then
                            Res.Add(Me.MappigResultByAffyProbeID(Index))
                        Else
                            Res.Add(x)
                        End If
                    Next
                    Return Res
                End Function
#Region "Export"

                ''' <summary>
                ''' Return TDT Format Of Affy Mapping By AffyID
                ''' 1 Affy ID could Hit Multiple Genes
                ''' All Of Them are in New Line
                ''' </summary>
                ''' <param name="MinNofPerfectMatch"></param>
                ''' <param name="ResultByAffyID"></param>
                ''' <returns></returns>
                Private Shared Function ExportByAffyIDMultipleLine(MinNofPerfectMatch As Integer,
                                                ResultByAffyID As List(Of AffyParsingResultByAffyID)) As String

                    Dim str As New System.Text.StringBuilder
                    For Each Item In ResultByAffyID
                        For Each Count In Item.AffyIDswCounts
                            If MinNofPerfectMatch = -1 Or Count.Count >= MinNofPerfectMatch Then
                                str.Append(Item.AffyID).Append(vbTab)
                                str.Append(Count.AffyID).Append(vbTab)
                                str.Append(Count.Count).AppendLine()
                            End If
                        Next
                    Next
                    If str.Length >= vbCrLf.Length Then str.Length -= vbCrLf.Length
                    Return str.ToString
                End Function

                ''' <summary>
                ''' Return TDT Format Of Affy Mapping By AffyID
                ''' 1 AffyID could Hit Multiple Genes 
                ''' All Of Them in Single Line
                ''' </summary>
                ''' <param name="MinNofPerfectMatch"></param>
                ''' <param name="ResultByAffyID"></param>
                ''' <returns></returns>
                Private Shared Function ExportByAffyIDSingleLine(MinNofPerfectMatch As Integer,
                                                ResultByAffyID As List(Of AffyParsingResultByAffyID)) As String
                    Dim str As New System.Text.StringBuilder
                    Try

                        For Each Item In ResultByAffyID
                            str.Append(Item.AffyID)

                            For Each Count In Item.AffyIDswCounts
                                If MinNofPerfectMatch = -1 Or Count.Count >= MinNofPerfectMatch Then
                                    str.Append(vbTab).Append(Count.AffyID).Append(vbTab)
                                    str.Append(Count.Count)

                                End If
                            Next

                            str.AppendLine()

                        Next
                    Catch ex As Exception
                        Dim alf As Int16 = 54
                    End Try

                    If str.Length >= vbCrLf.Length Then str.Length -= vbCrLf.Length
                    Return str.ToString
                End Function

                ''' <summary>
                ''' Return TDT Format Of Affy Mapping By SeqID
                ''' 1 Gene ID could Hit Multiple AffyIDs
                ''' All Of Them are in New Line
                ''' </summary>
                ''' <param name="MinNofPerfectMatch"></param>
                ''' <param name="ResultBySeqID"></param>
                ''' <returns></returns>
                Private Shared Function ExportBySeqIDMultipleLine(MinNofPerfectMatch As Integer,
                                                ResultBySeqID As List(Of AffyParsingResultBySeq)) As String

                    Dim str As New System.Text.StringBuilder
                    For Each Item In ResultBySeqID
                        For Each Count In Item.AffyIDswCounts
                            If MinNofPerfectMatch = -1 Or Count.Count >= MinNofPerfectMatch Then
                                str.Append(Item.Seq.ID).Append(vbTab)
                                str.Append(Count.AffyID).Append(vbTab)
                                str.Append(Count.Count).AppendLine()
                            End If
                        Next
                    Next
                    If str.Length >= vbCrLf.Length Then str.Length -= vbCrLf.Length
                    Return str.ToString
                End Function

                ''' <summary>
                ''' Return TDT Format Of Affy Mapping By SeqID
                ''' 1 Gene ID could Hit Multiple AffyIDs
                ''' All Of Them are in Single Line
                ''' </summary>
                ''' <param name="MinNofPerfectMatch"></param>
                ''' <param name="ResultBySeqID"></param>
                ''' <returns></returns>
                Private Shared Function ExportBySeqIDSingleine(MinNofPerfectMatch As Integer,
                                                ResultBySeqID As List(Of AffyParsingResultBySeq)) As String

                    Dim str As New System.Text.StringBuilder
                    For Each Item In ResultBySeqID
                        str.Append(Item.Seq.ID)
                        Dim Filtered As Boolean = True
                        For Each Count In Item.AffyIDswCounts
                            If MinNofPerfectMatch = -1 Or Count.Count >= MinNofPerfectMatch Then
                                str.Append(vbTab)
                                str.Append(Count.AffyID).Append(vbTab)
                                str.Append(Count.Count)
                                Filtered = False
                            End If
                        Next
                        If Filtered = True Then
                            str.Length = str.Length - Item.Seq.ID.Length
                        Else
                            str.AppendLine()
                        End If
                    Next
                    If str.Length >= vbCrLf.Length Then str.Length -= vbCrLf.Length
                    Return str.ToString
                End Function

                ''' <summary>
                ''' Format of Export, Optionally Subset Of 
                ''' </summary>
                ''' <returns></returns>
                Public Function Export(Format As ExportFormat,
                                       Optional ResultByGene As List(Of AffyParsingResultBySeq) = Nothing,
                                       Optional ResultByAffyID As List(Of AffyParsingResultByAffyID) = Nothing,
                                       Optional MinNofPerfectMatch As Integer = -1) As String
                    If IsNothing(ResultByAffyID) = True Then ResultByAffyID = Me.MappigResultByAffyProbeID
                    If IsNothing(ResultByGene) = True Then ResultByGene = Me.MappigResultByGene
                    Select Case Format
                        Case ExportFormat.ByAffyID_MultipleLines
                            Return ExportByAffyIDMultipleLine(MinNofPerfectMatch, ResultByAffyID)
                        Case ExportFormat.ByAffyID_SingleLine
                            Return ExportByAffyIDSingleLine(MinNofPerfectMatch, ResultByAffyID)
                        Case ExportFormat.BySeqID_MultipleLines
                            Return ExportBySeqIDMultipleLine(MinNofPerfectMatch, ResultByGene)
                        Case ExportFormat.BySeqID_SingleLine
                            Return ExportBySeqIDSingleine(MinNofPerfectMatch, ResultByGene)
                        Case ExportFormat.OnlyGeneIDs
                            Return ExportOnlyGeneIDs(MinNofPerfectMatch, ResultByGene)
                    End Select
                    Return String.Empty
                End Function

                Private Function ExportOnlyGeneIDs(minNofPerfectMatch As Integer, resultByGene As List(Of AffyParsingResultBySeq)) As String
                    Dim out As New List(Of String)
                    For Each Item In resultByGene
                        For Each SubItem In Item.AffyIDswCounts
                            If SubItem.Count >= minNofPerfectMatch Then
                                Dim t = Szunyi.Text.General.GetTextFromSplitted(Item.Seq.ID, " ", 1)
                                t = Szunyi.Text.General.GetTextFromSplitted(t, ".", 1)
                                If out.Contains(t) = False Then out.Add(t)
                                Exit For
                            End If
                        Next
                    Next
                    Return Szunyi.Text.General.GetText(out)

                End Function
#End Region
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

