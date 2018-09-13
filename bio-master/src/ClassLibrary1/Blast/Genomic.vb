Imports System.Drawing
Imports System.IO
Imports Bio.IO.GenBank
Imports Bio.Web.Blast
Imports ClassLibrary1.Szunyi.Sequences.SequenceManipulation

Namespace Szunyi
    Namespace Blast
        Public Class ReturnPointForBlast
            Public RealQueryStart As Integer
            Public MdQueryStart As Integer
            Public Length As Integer
            Public RealQueryEnd As Integer
            Public MdQueryEnd As Integer
            Public RealHitStart As Integer
            Public MdHitStart As Integer
            Public RealHitEnd As Integer
            Public MdHitEnd As Integer
            Public IsReverse As Boolean = False

            Public Status As String
            Public QueryDef As String
            Public Sub New(extenededHsp As ExtenededHsp, QueryDef As String)
                Me.QueryDef = QueryDef
                With extenededHsp
                    Me.RealQueryStart = .QueryStart
                    Me.RealQueryEnd = .QueryEnd
                    Me.RealHitStart = .HitStart
                    Me.RealHitEnd = .HitEnd
                    Dim NNPNucleotidePositionByQuery = GetNextNotPerfectNucleotidePosition(extenededHsp)
                    Me.Length = NNPNucleotidePositionByQuery
                    Me.RealQueryStart = .QueryStart
                    Me.RealQueryEnd = .QueryEnd
                    Me.MdQueryEnd = .QueryStart + NNPNucleotidePositionByQuery - 1
                    Me.MdQueryStart = .QueryStart
                    Me.MdHitStart = .HitStart
                    If .HitStart < .HitEnd Then
                        ' fw
                        Me.MdHitEnd = .HitStart + NNPNucleotidePositionByQuery - 1
                    Else
                        'rev
                        Me.MdHitEnd = .HitStart - NNPNucleotidePositionByQuery + 1
                        Me.IsReverse = True
                    End If
                End With

            End Sub
            Public Sub New(Before As ReturnPointForBlast, extenededHsp As ExtenededHsp)

                If Before.IsReverse = False Then
                    Me.IsReverse = False
                    DoFwExtension(Before, extenededHsp)
                Else
                    DoRevExtension(Before, extenededHsp)
                    Me.IsReverse = True
                End If
            End Sub
            Private Sub DoRevExtension(Before As ReturnPointForBlast, extenededHsp As ExtenededHsp)
                With extenededHsp
                    DoBasic(extenededHsp)
                    Dim FirstBaseToInvestigate = GetFirstBaseToInvestigate(Before.MdQueryEnd, Me.RealQueryStart, extenededHsp)
                    Dim NNPNucleotidePositionByQuery = GetNextNotPerfectNucleotidePosition(extenededHsp, FirstBaseToInvestigate)
                    If Before.MdQueryEnd >= extenededHsp.QueryStart Then

                        Me.MdQueryStart = Before.MdQueryEnd + 1
                        Me.MdHitStart = .HitStart - FirstBaseToInvestigate - GetModifier(extenededHsp, FirstBaseToInvestigate)
                        Dim LastNotPerfectNA = GetLastNotPerfectNucleotidePosition(extenededHsp)
                        If FirstBaseToInvestigate >= LastNotPerfectNA Then
                            Me.MdHitEnd = Me.MdHitStart - NNPNucleotidePositionByQuery + FirstBaseToInvestigate + 1
                            Me.MdQueryEnd = Me.MdQueryStart + NNPNucleotidePositionByQuery - FirstBaseToInvestigate - 1
                        Else
                            Me.Status = "First Base Bigger"
                        End If

                        Dim j As Int16 = 54
                    ElseIf Before.MdQueryEnd = .QueryStart - 1 Then
                        DoPerfect(extenededHsp)
                    Else
                        Me.Status = "Query End Bigger than QueryStart"
                    End If

                    Me.Length = NNPNucleotidePositionByQuery
                End With

            End Sub
            Private Sub DoBasic(extendedHSP As ExtenededHsp)
                With extendedHSP
                    Me.RealQueryStart = .QueryStart
                    Me.RealQueryEnd = .QueryEnd
                    Me.RealHitStart = .HitStart
                    Me.RealHitEnd = .HitEnd
                End With
            End Sub
            Private Sub DoPerfect(extenededHsp As ExtenededHsp)
                ' Perfect
                With extenededHsp
                    Me.MdQueryStart = .QueryStart
                    Me.MdQueryEnd = .QueryEnd
                    Me.MdHitStart = .HitStart
                    Me.MdHitEnd = .HitEnd
                End With
            End Sub
            Private Sub DoFwExtension(Before As ReturnPointForBlast, extenededHsp As ExtenededHsp)
                With extenededHsp
                    DoBasic(extenededHsp)
                    Dim FirstBaseToInvestigate = GetFirstBaseToInvestigate(Before.MdQueryEnd, Me.RealQueryStart, extenededHsp)
                    Dim NNPNucleotidePositionByQuery = GetNextNotPerfectNucleotidePosition(extenededHsp, FirstBaseToInvestigate)
                    If Before.MdQueryEnd >= extenededHsp.QueryStart Then

                        Me.MdQueryStart = Before.MdQueryEnd + 1
                        Me.MdHitStart = .HitStart + FirstBaseToInvestigate + GetModifier(extenededHsp, FirstBaseToInvestigate)
                        Dim LastNotPerfectNA = GetLastNotPerfectNucleotidePosition(extenededHsp)
                        If FirstBaseToInvestigate >= LastNotPerfectNA Then
                            Me.MdHitEnd = Me.MdHitStart + NNPNucleotidePositionByQuery - FirstBaseToInvestigate - 1
                            Me.MdQueryEnd = Me.MdQueryStart + NNPNucleotidePositionByQuery - FirstBaseToInvestigate - 1
                        Else
                            Me.Status = "First Base Bigger"
                        End If


                    ElseIf Before.MdQueryEnd = .QueryStart - 1 Then
                        DoPerfect(extenededHsp)
                    Else
                        Me.Status = "Query End Bigger than QueryStart"
                    End If

                    Me.Length = NNPNucleotidePositionByQuery


                End With
            End Sub

            Private Function GetNextNotPerfectNucleotidePosition(extenededHsp As ExtenededHsp, Optional StartBp As Integer = 0) As Integer
                Dim tmp = Split(extenededHsp.Fullseq, vbCrLf)
                Dim pt As New Point
                Dim Chars = tmp(1).ToCharArray

                For i1 = StartBp To Chars.Count - 1
                    If Chars(i1) = " " Then
                        Return i1
                    End If
                Next
                Return Chars.Count
            End Function

            Private Function GetLastNotPerfectNucleotidePosition(extenededHsp As ExtenededHsp, Optional StartBp As Integer = 0) As Integer
                Dim tmp = Split(extenededHsp.Fullseq, vbCrLf)
                Dim pt As New Point
                Dim Chars = tmp(1).ToCharArray
                For i1 = Chars.Count To 1 Step -1
                    If Mid(tmp(0), i1, 1) <> Mid(tmp(2), i1, 1) Then
                        Return i1
                    End If
                Next
                Return 0
            End Function

            Private Function GetFirstBaseToInvestigate(MdQueryEnd As Integer, RealQueryStart As Integer, extenededHsp As ExtenededHsp) As Integer
                Dim tmp = Split(extenededHsp.Fullseq, vbCrLf)
                Dim pt As New Point
                Dim Chars = tmp(1).ToCharArray

                For i1 = 1 To Chars.Count
                    If Mid(tmp(0), i1, 1) <> "-" Then
                        If RealQueryStart + i1 <= MdQueryEnd Then
                            ' Continue iteration
                        Else
                            For i2 = i1 + 1 To Chars.Count
                                If Mid(tmp(0), i2, 1) <> "-" Then Return i2 - 1
                            Next
                            Return i1
                        End If
                    End If
                Next
                Return 0
            End Function
            Private Function GetModifier(extendHSp As ExtenededHsp, FIrstNucleotide As Integer) As Integer

                Dim Modifier As Integer = 0
                Dim tmp = Split(extendHSp.Fullseq, vbCrLf)
                Dim pt As New Point
                Dim Chars = tmp(1).ToCharArray

                For i1 = 1 To FIrstNucleotide

                    If Mid(tmp(2), i1, 1) = "-" Then Modifier -= 1
                Next
                Return Modifier
            End Function
        End Class
        Public Class Genomic
            Public Property FileNames As List(Of String)
            Public Property BlastResults As List(Of BlastSearchRecord)
            Public Property QuerySeqs As List(Of Bio.ISequence)
            Public Property GenomicSeqs As List(Of Bio.ISequence)
            Public Property log As New System.Text.StringBuilder
            Public Sub New(X As List(Of BlastSearchRecord), FIles As List(Of FileInfo))
                Me.BlastResults = X
                Me.FileNames = (From x1 In FIles Select x1.FullName).ToList

            End Sub
            Public Sub DoIt()

                Me.GenomicSeqs = Szunyi.IO.Import.Sequence.FromUserSelection(Nothing, Szunyi.Constants.Files.GenBank)
                Dim MDs = Szunyi.Features.FeatureManipulation.GenBankMetaDataManipulation.GetGenBankMetaDatasFromSeqs(Me.GenomicSeqs)

                Dim cSearchSetting As New SettingForSearchInFeaturesAndQulifiers(Szunyi.Constants.Features.CDSs_mRNAs_Genes)
                '  Dim ExtFeatureList = Szunyi.Features.Features.GetExtFeatures(cSearchSetting, Me.GenomicSeqs)
                Me.QuerySeqs = Szunyi.Blast.BlastBasicIO.GetQuerySeqs(FileNames, Me.BlastResults, log)
                Dim NOfs As New Dictionary(Of Integer, List(Of BlastSearchRecord))

                For Each BlastResult In Me.BlastResults
                    '
                    Dim HitsBySeqID As Dictionary(Of String, List(Of Hit)) = GetGroups(BlastResult)
                    Dim HitsBySeqIDByCloseEachOther = GetSubGroups(HitsBySeqID, BlastResult, 2500)
                    Dim QuerySeq = Szunyi.Sequences.SequenceManipulation.GetSequences.ByID(QuerySeqs, BlastResult.IterationQueryDefinition)

                    If NOfs.ContainsKey(HitsBySeqIDByCloseEachOther.Count) = False Then NOfs.Add(HitsBySeqIDByCloseEachOther.Count, New List(Of BlastSearchRecord))
                    NOfs(HitsBySeqIDByCloseEachOther.Count).Add(BlastResult)
                    Dim HasFound As Boolean = False
                    For Each PotentialGene In HitsBySeqIDByCloseEachOther
                        Dim Key As String = Split(HitsBySeqIDByCloseEachOther.First.Key, vbTab).First
                        Dim Seq = GetSequences.ByID(Me.GenomicSeqs, Key)
                        If IsNothing(Seq) = False Then
                            Dim GB As Bio.IO.GenBank.GenBankMetadata = Seq.Metadata(Bio.Util.Helper.GenBankMetadataKey)

                            Dim Locis = GetLocation(HitsBySeqIDByCloseEachOther.First.Value,
                                                    BlastResult, New Bio.Web.Blast.Hit) 'Gene, mRNA,CDS

                            Dim NextLocusTag As Integer = 1


                            ' New Gene
                            Dim TheGene = GetFeatureItem(StandardFeatureKeys.Gene, Locis.First, BlastResult.IterationQueryDefinition)
                            GB.Features.All.Add(TheGene)



                            Dim mRNAs = GetFeatureItem(StandardFeatureKeys.MessengerRna, Locis.Last, BlastResult.IterationQueryDefinition)

                            Dim TheCDS = GetFeatureItem(StandardFeatureKeys.CodingSequence, Locis.Last, BlastResult.IterationQueryDefinition)

                            If TestLocactionAndTranslation(TheCDS, Seq,
                            QuerySeq, Locis.Last, log) = True Then


                                '   Dim Insertionindex = GB.Features.All.IndexOf(Features(BlastResult.IterationQueryDefinition).Last)

                                GB.Features.All.Add(mRNAs)
                                GB.Features.All.Add(TheCDS)

                                HasFound = True
                                log.Append(BlastResult.IterationQueryDefinition & vbTab & "Good" & vbCrLf)

                            Else
                                log.Append(BlastResult.IterationQueryDefinition & vbTab & "Bad Translation" & vbCrLf)
                            End If
                        Else
                            log.Append(BlastResult.IterationQueryDefinition & "nOT fOUND" & vbCrLf)
                        End If

                    Next ' Next Potetial Gene
                    If HasFound = False Then log.Append(BlastResult.IterationQueryDefinition).AppendLine()
                Next ' Next BlastSearchRecord
                Szunyi.IO.Export.SaveSequencesToSingleGenBank(Me.GenomicSeqs)

            End Sub
            Private Function TestLocactionAndTranslation(TheCDS As FeatureItem,
                                                 GenomeicDNA As Bio.Sequence,
                                                 OriginalCDSSeq As Bio.Sequence,
                                                 Loci As Bio.IO.GenBank.Location,
                                                 ByRef log As System.Text.StringBuilder) As Boolean
                Dim LociBuilder As New LocationBuilder
                Dim f = LociBuilder.GetLocationString(Loci)
                Dim LengthOFLog As Integer = log.Length
                If f.Contains("0..0") Then
                    log.Append(OriginalCDSSeq.ID).Append(vbTab)
                    log.Append("0..0").Append(vbTab)
                    log.Append(f).AppendLine()
                End If

                Try
                    Dim Translation = Szunyi.DNA.Translate.TranaslateToString(TheCDS, GenomeicDNA)
                    Dim OriTranslation = Szunyi.Translate.TranaslateIntoString(OriginalCDSSeq, 1)
                    If Translation <> OriTranslation Then
                        log.Append(OriginalCDSSeq.ID).Append(vbTab)
                        log.Append("Mis translation").Append(vbTab)
                        log.Append(f).AppendLine()
                    End If
                Catch ex As Exception
                    log.Append(OriginalCDSSeq.ID).Append(vbTab)
                    log.Append("Error in software").Append(vbTab)
                    log.Append(f).AppendLine()
                End Try
                If log.Length = LengthOFLog Then Return True ' No error
                Return False
            End Function
#Region "Create Groups"
            Private Function GetGroups(Item As BlastSearchRecord) As Dictionary(Of String, List(Of Hit))
                Dim Groups As New Dictionary(Of String, List(Of Hit))
                For Each Hit In Item.Hits
                    If Groups.ContainsKey(Hit.Def) = False Then Groups.Add(Hit.Def, New List(Of Hit))
                    Groups(Hit.Def).Add(Hit)
                Next
                Return Groups
            End Function
            Private Function GetSubGroups(Groups As Dictionary(Of String, List(Of Hit)),
                                Item As BlastSearchRecord, MaxDiffereBetweenExons As Integer) As Dictionary(Of String, List(Of Hsp))


                Dim SubGroups As New Dictionary(Of String, List(Of Hsp))

                For Each Group In Groups
                    Dim Hsps = Group.Value.First.Hsps

                    Dim OrderedHsps = (From x In Hsps Order By x.HitStart).ToList

                    If OrderedHsps.Count = 1 Then
                        SubGroups.Add(Group.Key & vbTab & SubGroups.Count + 1, New List(Of Hsp))
                        SubGroups.Last.Value.Add(OrderedHsps.First)
                    Else
                        Dim Finished As Boolean = False
                        For i1 = 0 To OrderedHsps.Count - 2
                            If Finished = True Then
                                Exit For
                            End If
                            For i2 = i1 + 1 To OrderedHsps.Count - 1
                                If OrderedHsps(i1).HitStart + MaxDiffereBetweenExons > OrderedHsps(i2).HitStart Then
                                    If i1 = 0 And i2 = 1 Then
                                        SubGroups.Add(Group.Key & vbTab & SubGroups.Count + 1, New List(Of Hsp))
                                        SubGroups.Last.Value.Add(OrderedHsps(i1))
                                        SubGroups.Last.Value.Add(OrderedHsps(i2))
                                    Else
                                        SubGroups.Last.Value.Add(OrderedHsps(i2))
                                    End If
                                Else ' ha nagy a difi
                                    If i1 = 0 And SubGroups.Count = 0 Then
                                        SubGroups.Add(Group.Key & vbTab & SubGroups.Count + 1, New List(Of Hsp))
                                        SubGroups.Last.Value.Add(OrderedHsps(i1))
                                    End If
                                    SubGroups.Add(Group.Key & vbTab & SubGroups.Count + 1, New List(Of Hsp))
                                    SubGroups.Last.Value.Add(OrderedHsps(i2))


                                    i1 = i2
                                End If
                                If i2 = OrderedHsps.Count - 1 Then
                                    Finished = True
                                End If
                            Next
                        Next
                    End If
                Next
                Dim res As New Dictionary(Of String, List(Of Hsp))
                For Each SubGroup In SubGroups
                    Dim IdentityCount As Integer = 0
                    For Each Hsp In SubGroup.Value
                        IdentityCount += Hsp.IdentitiesCount
                    Next
                    If IdentityCount >= Item.IterationQueryLength Then
                        res.Add(SubGroup.Key, SubGroup.Value)
                    Else
                        Dim alf As Int16 = 54
                    End If

                Next
                Return res
            End Function

#End Region
            Private Function GetFeatureItem(FeatureType As String, Loci As Bio.IO.GenBank.Location, LocusTag As String) _
                As FeatureItem
                Dim FeatureItem =
                    New FeatureItem(FeatureType, Loci)
                Dim LocusTags As New List(Of String)
                LocusTags.Add(LocusTag)
                FeatureItem.Qualifiers(StandardQualifierNames.LocusTag) = LocusTags
                Return FeatureItem

            End Function
            Private Function GetLocation(SelectedHSps As List(Of Hsp),
                                BRecord As BlastSearchRecord, Hit As Bio.Web.Blast.Hit) As List(Of Bio.IO.GenBank.Location)

                Dim Locis As New List(Of Bio.IO.GenBank.Location)
                Dim r As New List(Of ExtenededHsp)
                For Each item In SelectedHSps
                    r.Add(New ExtenededHsp(item, BRecord, Hit))
                Next
                Dim OrderedHsps As List(Of ExtenededHsp) = (From x In r Order By x.QueryStart Ascending).ToList


                Dim t As New List(Of ReturnPointForBlast)
                t.Add(New ReturnPointForBlast(OrderedHsps.First, BRecord.IterationQueryDefinition))
                For i1 = 1 To OrderedHsps.Count - 1
                    Dim x1 As New ReturnPointForBlast(t.Last, OrderedHsps(i1))
                    t.Add(x1)
                Next

                Dim LocBuilder As New Bio.IO.GenBank.LocationBuilder

                ' Gene
                Dim GeneLocation As New List(Of ReturnPointForBlast)
                GeneLocation.Add(New ReturnPointForBlast(OrderedHsps.First, BRecord.IterationQueryDefinition))
                GeneLocation.First.MdHitEnd = t.Last.MdHitEnd
                Locis.Add(LocBuilder.GetLocation(GetLOcationString(GeneLocation)))
                Dim s As String = GetLOcationString(t)
                Dim i = s.LastIndexOf("(")
                If i <> -1 Then
                    Dim s1 = s.Insert(i + 1, "<")
                    i = s1.LastIndexOf(".")
                    s1 = s1.Insert(i + 1, ">")
                    Locis.Add(LocBuilder.GetLocation(s1))
                Else
                    Dim alf As Int16 = 54
                End If

                'CDS
                Dim Loci = LocBuilder.GetLocation(s)
                Locis.Add(Loci)
                Return Locis
            End Function
            Private Function GetLOcationString(t As List(Of ReturnPointForBlast)) As String
                Dim str As New System.Text.StringBuilder
                If t.First.IsReverse = True Then str.Append("complement(")
                If t.Count > 1 Then str.Append("join(")
                Dim tf = From x In t Order By x.MdHitStart Ascending


                For Each item In tf
                    If item.MdHitStart < item.MdHitEnd Then
                        str.Append(item.MdHitStart).Append("..").Append(item.MdHitEnd).Append(",")
                    Else
                        str.Append(item.MdHitEnd).Append("..").Append(item.MdHitStart).Append(",")
                    End If

                Next
                str.Length -= 1
                If t.Count > 1 Then str.Append(")")
                If t.First.IsReverse = True Then str.Append(")")
                Return str.ToString

            End Function

        End Class

    End Namespace
End Namespace
