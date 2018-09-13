Imports System.Text
Imports Bio.IO.GenBank
Imports ClassLibrary1.Szunyi.Comparares
Imports ClassLibrary1.Szunyi.Features
Imports ClassLibrary1.Szunyi.Features.FeatureManipulation

Namespace Szunyi
    Namespace Sequences
        Namespace SequenceManipulation
            Public Class Reapair
                Public Shared Function ReapirSeqs(Seqs As List(Of Bio.ISequence)) As List(Of Bio.ISequence)
                    '   Dim nSeqs = Szunyi.Sequences.SequenceManinpulation.CloneSeqs(Seqs)
                    Dim Log As New System.Text.StringBuilder
                    ' Create ShortLocusTag For gene (splitby . Get First Add " at the end

                    Seqs = UniqueDistinct.Get1CopyBySeqAndID(Seqs)

                    CheckAndRepair.CorrectLocusTagsbySeqs(Seqs, Log)
                    '

                    CheckAndRepair.CorrectFeaturesStartAndEndBySeqs(Seqs, Log)
                    '

                    CheckAndRepair.CorrectSinglePosition(Seqs, Log)
                    '
                    CheckAndRepair.CorrectFeaturesQulifiers_Chr34(Seqs, Log)
                    '

                    CheckAndRepair.DeleteDuplicateFeatures(Seqs, Szunyi.Constants.Features.CDSs_mRNAs_Genes, Log)
                    '
                    CheckAndRepair.InsertMissingGenes(Seqs, Log)




                    Return Seqs
                End Function


            End Class

            Public Class CheckAndRepair
                Public Shared Sub InsertMissingGenes(ByRef Seqs As List(Of Bio.ISequence), ByRef log As StringBuilder)

                End Sub

                Public Shared Sub DeleteDuplicateFeatures(ByRef Seqs As List(Of Bio.ISequence),
                                                           FeatureTypes As List(Of String),
                                                           ByRef log As StringBuilder)
                    Dim str As New System.Text.StringBuilder
                    Dim Count As Integer = 0
                    For Each Seq In Seqs
                        Dim Md = GenBankMetaDataManipulation.GetGenBankMetaDataFromSeq(Seq)
                        For Each FeatureType In FeatureTypes
                            Dim Features = GetFeaturesByType.GetFeturesByTypeFromSeq(Seq, FeatureType)
                            If FeatureType = StandardFeatureKeys.Gene Then
                                For Each MultipliedFeature In UniqueDistict.GetDuplicatedFeaturesByLocusTag(Features)
                                    Dim NewGene = Szunyi.Features.FeatureManipulation.GetLocations.GetBiggest(MultipliedFeature, FeatureType)
                                    Md.Features.All.Add(NewGene)
                                    For Each Feat In MultipliedFeature
                                        Md.Features.All.Remove(Feat)
                                    Next
                                Next
                            Else
                                For Each MultipliedFeature In UniqueDistict.GetDuplicatedFeaturesByLocusTagAndLocation(Features)
                                    Dim NewGene = Szunyi.Features.FeatureManipulation.GetLocations.GetBiggest(MultipliedFeature, FeatureType)
                                    Md.Features.All.Add(NewGene)
                                    For Each Feat In MultipliedFeature
                                        Md.Features.All.Remove(Feat)
                                    Next
                                Next
                            End If


                        Next
                    Next

                End Sub

                Public Shared Sub CorrectSinglePosition(ByRef Seqs As List(Of Bio.ISequence), log As StringBuilder)
                    For Each Seq In Seqs
                        CorrectSinglePosition(Seq, log)
                    Next
                End Sub
                Public Shared Sub CorrectSinglePosition(ByRef Seq As Bio.ISequence, log As StringBuilder)
                    Dim Md = GenBankMetaDataManipulation.GetGenBankMetaDataFromSeq(Seq)
                    Dim str As New StringBuilder
                    Dim Count As Integer = 0
                    If IsNothing(Md) = False AndAlso IsNothing(Md.Features) = False Then
                        Dim Features = Md.Features.All

                        Dim LociBuilder As New LocationBuilder
                        For Each Feature In Features

                            Dim loc = LociBuilder.GetLocationString(Feature.Location)

                            Dim ToRemove = Split("complement_join_)_(", "_").ToList
                            Dim sLocs() = Split(Szunyi.Text.General.RemoveFromString(loc, ToRemove), ",")
                            For Each sLoc In sLocs
                                Dim s1() = Split(sLoc, "..")
                                If s1.Count <> 2 Then

                                    str.Append(Feature.Qualifiers(StandardQualifierNames.LocusTag).First).Append(vbTab)
                                    str.Append(loc).AppendLine()
                                    Count += 1
                                    loc = loc.Replace(s1.First, s1.First & ".." & s1.First)

                                    Feature.Location = LociBuilder.GetLocation(loc)
                                End If
                            Next

                        Next
                    End If
                    If Count > 0 Then
                        log.AppendLine("Single Position:" & Count & vbCrLf)
                        log.AppendLine(str.ToString)
                    End If
                End Sub

                ''' <summary>
                ''' Correct missing " and Replace "" with Sort The Feature By Start"
                ''' After End Rverse After Gene, mRNA, CDS
                ''' </summary>
                ''' <param name="Seqs"></param>
                Public Shared Sub CorrectFeaturesQulifiers_Chr34(ByRef Seqs As List(Of Bio.ISequence), log As StringBuilder)
                    For Each Seq In Seqs
                        If Seq.Metadata.ContainsKey(Bio.Util.Helper.GenBankMetadataKey) Then
                            Dim Md As Bio.IO.GenBank.GenBankMetadata = Seq.Metadata(Bio.Util.Helper.GenBankMetadataKey)
                            If IsNothing(Md) = False AndAlso IsNothing(Md.Features) = False Then
                                For Each Feat In Md.Features.All
                                    For Each Qual In Bio.IO.GenBank.StandardQualifierNames.All
                                        If Feat.Qualifiers.ContainsKey(Qual) Then
                                            Dim s As List(Of String) = Feat.Qualifiers(Qual).ToList
                                            If s.Count > 0 Then
                                                If s.First.StartsWith(Chr(34)) = False Then
                                                    s(0) = Chr(34) & s.First
                                                    Feat.Qualifiers(Qual) = s
                                                End If
                                                If s.Last.EndsWith(Chr(34)) = False Then
                                                    s(s.Count - 1) = s.Last & Chr(34)
                                                    Feat.Qualifiers(Qual) = s
                                                End If
                                            Else
                                                Feat.Qualifiers.Remove(Qual)
                                            End If

                                        End If
                                    Next ' Qulifier
                                Next ' Features
                                Md.Features.All.Sort(Comparares.AllComparares.ByFeauterItemLocation)
                            End If
                        End If

                    Next ' Seq
                End Sub

                ''' <summary>
                ''' Set Correct Start and End position for CDS,mRNA and Gene
                ''' </summary>
                ''' <param name="Seqs"></param>
                ''' <param name="log"></param>
                Public Shared Sub CorrectFeaturesStartAndEndBySeqs(ByRef Seqs As List(Of Bio.ISequence), log As StringBuilder)
                    For Each Seq In Seqs
                        CorrectFeaturesStartAndEndBySeq(Seq, log)
                    Next
                End Sub


                Public Shared Sub CorrectFeaturesStartAndEndBySeq(ByRef Seq As Bio.ISequence, log As StringBuilder)
                    Dim LociBuilder As New Bio.IO.GenBank.LocationBuilder
                    Dim CDSs = Szunyi.Features.FeatureManipulation.GetFeaturesByType.GetFeturesByTypeFromSeq(Seq, StandardFeatureKeys.CodingSequence)
                    Dim mRNAs = Szunyi.Features.FeatureManipulation.GetFeaturesByType.GetFeturesByTypeFromSeq(Seq, StandardFeatureKeys.MessengerRna)
                    Dim Genes = FeatureManipulation.GetFeaturesByType.GetFeturesByTypeFromSeq(Seq, StandardFeatureKeys.Gene)
                    Dim Md = FeatureManipulation.GenBankMetaDataManipulation.GetGenBankMetaDataFromSeq(Seq)

                    If IsNothing(mRNAs) = False Then mRNAs.Sort(AllComparares.ByLocusTag)
                    If IsNothing(Genes) = False Then Genes.Sort(AllComparares.ByLocusTag)
                    If IsNothing(CDSs) = False Then CDSs.Sort(AllComparares.ByLocusTag)
                    CorrectFeaturesStartAndEnd(CDSs, mRNAs, True, StandardFeatureKeys.MessengerRna, log, Md, LociBuilder)
                    CorrectFeaturesStartAndEnd(CDSs, Genes, False, StandardFeatureKeys.Gene, log, Md, LociBuilder)
                    CorrectFeaturesStartAndEnd(mRNAs, Genes, False, StandardFeatureKeys.Gene, log, Md, LociBuilder)
                    '    CorrectFeaturesStartAndEnd(Genes, mRNAs, False, StandardFeatureKeys.Gene, log, Md, LociBuilder)
                    Seq.Metadata.Clear()
                    Seq.Metadata.Add(Bio.Util.Helper.GenBankMetadataKey, Md)


                End Sub
                Public Shared Sub CorrectFeaturesStartAndEnd(ByRef FeatureFrom As List(Of FeatureItem),
                                                             ByRef FeatureTo As List(Of FeatureItem),
                                                             IsFullLocusTag As Boolean,
                                                             TypeOfFeatureTo As String,
                                                            ByRef log As StringBuilder,
                                                            ByRef Md As Bio.IO.GenBank.GenBankMetadata,
                                                            ByRef LociBuilder As Bio.IO.GenBank.LocationBuilder)

                    If IsNothing(FeatureFrom) = True Then Exit Sub
                    If IsNothing(FeatureTo) = True Then FeatureTo = New List(Of FeatureItem)
                    Dim tmp As New FeatureItem(StandardFeatureKeys.CodingSequence, "1..1")
                    Dim IndexOfFeatureTo As Integer
                    For Each Feat In FeatureFrom

                        If IsFullLocusTag = True Then
                            IndexOfFeatureTo = FeatureTo.BinarySearch(Feat, AllComparares.ByLocusTag)
                        Else
                            tmp.Qualifiers.Remove(StandardQualifierNames.LocusTag)
                            Dim Ltags As New List(Of String)
                            Ltags.Add(Szunyi.Features.FeatureManipulation.Common.Get_ShortLocusTag(Feat))
                            tmp.Qualifiers.Add(StandardQualifierNames.LocusTag, Ltags)

                            IndexOfFeatureTo = FeatureTo.BinarySearch(tmp, AllComparares.ByLocusTag)
                        End If

                        If IndexOfFeatureTo > -1 Then
                            CorrectStartAndEndmRNA(Feat, FeatureTo(IndexOfFeatureTo), LociBuilder, log)
                        Else
                            Dim x As FeatureItem

                            If TypeOfFeatureTo = StandardFeatureKeys.Gene Then
                                x = New FeatureItem(TypeOfFeatureTo,
                                                       Szunyi.Features.FeatureManipulation.GetLocations.GetGeneLocationFromCDS(Feat.Location))
                                Dim x1 As New Gene(x.Location)

                            Else
                                x = New FeatureItem(TypeOfFeatureTo, Feat.Location)
                            End If
                            log.Append("Inserted: " & TypeOfFeatureTo & " :" & Feat.Qualifiers(StandardQualifierNames.LocusTag).First).AppendLine()
                            x = FeatureManipulation.MergeFeatures.Merge2Features(Feat, x, True)
                            ' If the new is gene then LocusTag is set to ShortLocusTag
                            Dim Ltags As New List(Of String)
                            Ltags.Add(Szunyi.Features.FeatureManipulation.Common.Get_ShortLocusTag(Feat))
                            '  tmp.Qualifiers.Add(StandardQualifierNames.LocusTag, Ltags)
                            If TypeOfFeatureTo = StandardFeatureKeys.Gene Then x.Qualifiers(StandardQualifierNames.LocusTag) = Ltags
                            Szunyi.Features.FeatureManipulation.GenBankMetaDataManipulation.AddFeature(Md, x, TypeOfFeatureTo)


                        End If


                    Next

                End Sub

                Public Shared Sub CorrectStartAndEndGene(ByRef CDS As FeatureItem,
                                                         ByRef FeatureToCheck As FeatureItem,
                                                         ByRef LociBuilder As Bio.IO.GenBank.LocationBuilder,
                                                         ByRef log As StringBuilder)
                    Dim sLoc = LociBuilder.GetLocationString(FeatureToCheck.Location)
                    Dim sLoctmp = LociBuilder.GetLocationString(CDS.Location)
                    Dim s1() = Split(sLoc, "..")
                    If FeatureToCheck.Location.LocationStart > CDS.Location.LocationStart Then
                        s1(0) = s1(0).Replace(FeatureToCheck.Location.LocationStart, CDS.Location.LocationStart)

                    End If
                    If FeatureToCheck.Location.LocationEnd < CDS.Location.LocationEnd Then
                        s1(s1.Count - 1) = s1.Last.Replace(FeatureToCheck.Location.LocationEnd, CDS.Location.LocationEnd)
                    End If
                    Dim sLoc2 = Szunyi.Text.General.GetText(s1, "..")
                    If sLoc <> sLoc2 Then
                        log.Append(FeatureToCheck.Qualifiers(StandardQualifierNames.LocusTag).First)
                    End If
                    FeatureToCheck.Location = LociBuilder.GetLocation(sLoc2)
                End Sub
                Public Shared Sub CorrectStartAndEndmRNA(ByRef CDS As FeatureItem,
                                                         ByRef FeatureToCheck As FeatureItem,
                                                        ByRef LociBuilder As Bio.IO.GenBank.LocationBuilder,
                                                        ByRef log As StringBuilder)
                    Dim sLoc = LociBuilder.GetLocationString(FeatureToCheck.Location)

                    Dim s1() = Split(sLoc, "..")
                    If FeatureToCheck.Location.LocationStart > CDS.Location.LocationStart Then
                        s1(0) = s1(0).Replace(FeatureToCheck.Location.LocationStart, CDS.Location.LocationStart)

                    End If
                    If FeatureToCheck.Location.LocationEnd < CDS.Location.LocationEnd Then
                        s1(s1.Count - 1) = s1.Last.Replace(FeatureToCheck.Location.LocationEnd, CDS.Location.LocationEnd)
                    End If

                    Dim sLoc2 = Szunyi.Text.General.GetText(s1, "..")
                    If sLoc <> sLoc2 Then
                        log.AppendLine(CDS.Key).Append(vbTab).Append(FeatureToCheck.Key).AppendLine()
                        log.Append(FeatureToCheck.Qualifiers(StandardQualifierNames.LocusTag).First).Append(vbTab).Append(sLoc).Append("<>").Append(sLoc2).AppendLine()
                        FeatureToCheck.Location = LociBuilder.GetLocation(sLoc2)
                    End If

                End Sub
                ''' <summary>
                ''' Replace LocusTag with ShortLocusTag and Set Chr(34) Exactly One 
                ''' </summary>
                ''' <param name="Seqs"></param>
                ''' <param name="log"></param>
                Public Shared Sub CorrectLocusTagsbySeqs(ByRef Seqs As List(Of Bio.ISequence), ByRef log As StringBuilder)
                    For Each Seq In Seqs
                        CorrectLocusTagsbySeq(Seq, log)
                    Next
                End Sub
                ''' <summary>
                ''' Replace LocusTag with ShortLocusTag in Gene And Correct Chr(34) Issues
                ''' </summary>
                ''' <param name="Seq"></param>
                ''' <param name="log"></param>
                Public Shared Sub CorrectLocusTagsbySeq(ByRef Seq As Bio.ISequence, log As StringBuilder)
                    Dim Features = Szunyi.Features.FeatureManipulation.GetFeaturesByType.GetFeturesByTypesFromSeq(Seq, StandardFeatureKeys.All)
                    If IsNothing(Features) = False Then
                        For Each Feature In Features
                            If Feature.Key = StandardFeatureKeys.Gene Then

                                Dim t = Szunyi.Features.FeatureManipulation.Common.Get_ShortLocusTag(Feature)
                                If t <> Feature.Qualifiers(StandardQualifierNames.LocusTag).First Then
                                    Szunyi.Features.FeatureManipulation.Common.Correct_LocusTag(Feature)
                                    log.Append("Correct LocusTag: " & t.First)
                                End If
                            End If

                        Next
                    End If

                End Sub
            End Class

        End Namespace
    End Namespace
End Namespace