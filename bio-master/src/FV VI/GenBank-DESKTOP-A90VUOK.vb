Imports System.IO
Imports Bio
Imports Bio.IO.GenBank

Namespace Szunyi
	Module GenBank


        Public Class FeatureManipulation
            Dim tmpExtFeature As New ExtFeature("")
            Dim LocusTagComparer As New Comparares.ExtFeatureLocusTagComparer
#Region "MergeFeatures"
            ''' <summary>
            ''' Merge the Annotaion if different, but not The LocusTag
            ''' </summary>
            ''' <param name="Feat1"></param>
            ''' <param name="Feat2"></param>
            ''' <returns></returns>
            Public Function Merge2Features(Feat1 As FeatureItem, Feat2 As FeatureItem) As FeatureItem
                Dim Qulifiers = StandardQualifierNames.All
                For Each Qual In StandardQualifierNames.All
                    If Qual <> StandardQualifierNames.LocusTag Then
                        If Feat1.Qualifiers.ContainsKey(Qual) = False AndAlso Feat2.Qualifiers.ContainsKey(Qual) = True Then
                            Feat1.Qualifiers(Qual) = Feat2.Qualifiers(Qual)
                        ElseIf Feat1.Qualifiers.ContainsKey(Qual) AndAlso Feat2.Qualifiers.ContainsKey(Qual) = True
                            Dim sFeat1 = Szunyi.Text.GetText(Feat1.Qualifiers(Qual), " ")
                            Dim sFeat2 = Szunyi.Text.GetText(Feat2.Qualifiers(Qual), " ")
                            If sFeat1 <> sFeat2 Then
                                Dim x As New List(Of String)
                                x.Add(sFeat1)
                                x.Add(sFeat2)
                                Feat1.Qualifiers(Qual) = x
                            End If
                        End If
                    End If

                Next
                Return Feat1
            End Function

            ''' <summary>
            ''' Find the Merge the Annotaion if different, but not The LocusTag
            ''' </summary>
            ''' <param name="FeatureTo"></param>
            ''' <param name="FeatList"></param>
            ''' <returns></returns>
            Public Function FindAndMerge2Features(FeatureTo As ExtFeature,
                                                  FeatList As Szunyi.ListOf.ExtFeatureList) As FeatureItem
                Dim LocusTags = GetLocusTags(FeatureTo)
                Dim FeaturesFrom As List(Of ExtFeature) = GetExtFeatures(LocusTags, FeatList)

                Dim Qulifiers = StandardQualifierNames.All
                For Each CorrespondingExtFeature In FeaturesFrom
                    For Each Qual In StandardQualifierNames.All
                        If Qual <> StandardQualifierNames.LocusTag Then
                            If FeatureTo.Feature.Qualifiers.ContainsKey(Qual) = False AndAlso
                                CorrespondingExtFeature.Feature.Qualifiers.ContainsKey(Qual) = True Then
                                FeatureTo.Feature.Qualifiers(Qual) = CorrespondingExtFeature.Feature.Qualifiers(Qual)
                            ElseIf FeatureTo.Feature.Qualifiers.ContainsKey(Qual) AndAlso
                                CorrespondingExtFeature.Feature.Qualifiers.ContainsKey(Qual) = True
                                Dim sFeat1 = FeatureTo.Feature.Qualifiers(Qual).ToList
                                Dim SFeat2 As New List(Of String)
                                For Each Item In sFeat1
                                    SFeat2.AddRange(Split(Item, vbCrLf))
                                Next
                                For Each Line In CorrespondingExtFeature.Feature.Qualifiers(Qual)
                                    If SFeat2.Contains(Line) = False Then
                                        SFeat2.Add(Line)
                                    Else
                                        Dim alf As Int16 = 43
                                    End If
                                Next
                                FeatureTo.Feature.Qualifiers(Qual) = SFeat2
                            End If
                        End If

                    Next
                Next

                Return FeatureTo.Feature
            End Function

#End Region

#Region "GenBankMetaData"
            ''' <summary>
            ''' Return the List of GenBankMetadatas or Empty List
            ''' </summary>
            ''' <param name="Seqs"></param>
            ''' <returns></returns>
            Public Shared Function GetGenBankMetaDatas(Seqs As List(Of Bio.Sequence)) As List(Of Bio.IO.GenBank.GenBankMetadata)
                Dim Out As New List(Of Bio.IO.GenBank.GenBankMetadata)
                If IsNothing(Seqs) = True Then Return Out
                For Each Seq In Seqs
                    If Seq.Metadata.ContainsKey(Bio.Util.Helper.GenBankMetadataKey) Then
                        Out.Add(Seq.Metadata(Bio.Util.Helper.GenBankMetadataKey))
                    End If
                Next
                Return Out
            End Function
            ''' <summary>
            ''' Return The GenBankMetaData or Nothing
            ''' </summary>
            ''' <param name="Seq"></param>
            ''' <returns></returns>
            Public Function GetGenBankMetaData(Seq As Bio.Sequence) As Bio.IO.GenBank.GenBankMetadata

                If Seq.Metadata.ContainsKey(Bio.Util.Helper.GenBankMetadataKey) Then
                    Return (Seq.Metadata(Bio.Util.Helper.GenBankMetadataKey))
                End If
                Return Nothing
            End Function
#End Region

#Region "GetFeaturesByType"
            Public Shared Function GetAllFetureByType(Seqs As List(Of Bio.Sequence),
                                               SelectedFeatureTypes As List(Of String)) As List(Of FeatureItem)

                Dim MDs = GetGenBankMetaDatas(Seqs)
                Return GetAllFetureByType(MDs, SelectedFeatureTypes)

            End Function
            Public Shared Function GetAllFetureByType(MDs As List(Of Bio.IO.GenBank.GenBankMetadata),
                                               SelectedFeatureTypes As List(Of String)) As List(Of FeatureItem)

                Dim Out As New List(Of FeatureItem)
                For Each Md In MDs
                    For Each SelectedFeature In SelectedFeatureTypes
                        Dim t = GetFeatureByType(SelectedFeature, Md)
                        If IsNothing(t) = False Then Out.AddRange(t)
                    Next
                Next
                Return Out
            End Function
            ''' <summary>
            ''' Return Ienumarable of FeatureItem or Nothing
            ''' </summary>
            ''' <param name="Type"></param>
            ''' <param name="md"></param>
            ''' <returns></returns>
            Shared Function GetFeatureByType(Type As String, md As Bio.IO.GenBank.GenBankMetadata) As IEnumerable(Of FeatureItem)
                If IsNothing(md) = True Then Return Nothing
                If IsNothing(md.Features) = True Then Return Nothing

                Select Case Type
                    Case = StandardFeatureKeys.Attenuator
                        If md.Features.Attenuators.Count > 0 Then Return md.Features.Attenuators

                    Case = StandardFeatureKeys.CaaTSignal
                        If md.Features.CAATSignals.Count > 0 Then Return md.Features.CAATSignals

                    Case = StandardFeatureKeys.CodingSequence
                        If md.Features.CodingSequences.Count > 0 Then Return md.Features.CodingSequences

                    Case = StandardFeatureKeys.DisplacementLoop
                        If md.Features.DisplacementLoops.Count > 0 Then Return md.Features.DisplacementLoops

                    Case = StandardFeatureKeys.Enhancer
                        If md.Features.Enhancers.Count > 0 Then Return md.Features.Enhancers

                    Case = StandardFeatureKeys.Exon
                        If md.Features.Exons.Count > 0 Then Return md.Features.Exons

                    Case = StandardFeatureKeys.FivePrimeUtr
                        If md.Features.FivePrimeUTRs.Count > 0 Then Return md.Features.FivePrimeUTRs

                    Case = StandardFeatureKeys.GcSingal
                        If md.Features.GCSignals.Count > 0 Then Return md.Features.GCSignals

                    Case = StandardFeatureKeys.Gene
                        If md.Features.Genes.Count > 0 Then Return md.Features.Genes

                    Case = StandardFeatureKeys.InterveningDna
                        If md.Features.InterveningDNAs.Count > 0 Then Return md.Features.InterveningDNAs

                    Case = StandardFeatureKeys.Intron
                        If md.Features.Introns.Count > 0 Then Return md.Features.Introns

                    Case = StandardFeatureKeys.LongTerminalRepeat
                        If md.Features.LongTerminalRepeats.Count > 0 Then Return md.Features.LongTerminalRepeats

                    Case = StandardFeatureKeys.MaturePeptide
                        If md.Features.MaturePeptides.Count > 0 Then Return md.Features.MaturePeptides

                    Case = StandardFeatureKeys.MessengerRna
                        If md.Features.MessengerRNAs.Count > 0 Then Return md.Features.MessengerRNAs

                    Case = StandardFeatureKeys.Minus10Signal
                        If md.Features.Minus10Signals.Count > 0 Then Return md.Features.Minus10Signals

                    Case = StandardFeatureKeys.Minus35Signal
                        If md.Features.Minus35Signals.Count > 0 Then Return md.Features.Minus35Signals

                    Case = StandardFeatureKeys.MiscBinding
                        If md.Features.MiscBindings.Count > 0 Then Return md.Features.MiscBindings

                    Case = StandardFeatureKeys.MiscDifference
                        If md.Features.MiscDifferences.Count > 0 Then Return md.Features.MiscDifferences

                    Case = StandardFeatureKeys.MiscFeature
                        If md.Features.MiscFeatures.Count > 0 Then Return md.Features.MiscFeatures

                    Case = StandardFeatureKeys.MiscRecombination
                        If md.Features.MiscRecombinations.Count > 0 Then Return md.Features.MiscRecombinations

                    Case = StandardFeatureKeys.MiscRna
                        If md.Features.MiscRNAs.Count > 0 Then Return md.Features.MiscRNAs

                    Case = StandardFeatureKeys.MiscSignal
                        If md.Features.MiscSignals.Count > 0 Then Return md.Features.MiscSignals

                    Case = StandardFeatureKeys.MiscStructure
                        If md.Features.MiscStructures.Count > 0 Then Return md.Features.MiscStructures

                    Case = StandardFeatureKeys.ModifiedBase
                        If md.Features.ModifiedBases.Count > 0 Then Return md.Features.ModifiedBases

                    Case = StandardFeatureKeys.NonCodingRna
                        If md.Features.NonCodingRNAs.Count > 0 Then Return md.Features.NonCodingRNAs

                    Case = StandardFeatureKeys.OperonRegion
                        If md.Features.OperonRegions.Count > 0 Then Return md.Features.OperonRegions

                    Case = StandardFeatureKeys.PolyASignal
                        If md.Features.PolyASignals.Count > 0 Then Return md.Features.PolyASignals

                    Case = StandardFeatureKeys.PolyASite
                        If md.Features.PolyASites.Count > 0 Then Return md.Features.PolyASites

                    Case = StandardFeatureKeys.PrecursorRna
                        If md.Features.PrecursorRNAs.Count > 0 Then Return md.Features.PrecursorRNAs

                    Case = StandardFeatureKeys.Promoter
                        If md.Features.Promoters.Count > 0 Then Return md.Features.Promoters

                    Case = StandardFeatureKeys.ProteinBindingSite
                        If md.Features.ProteinBindingSites.Count > 0 Then Return md.Features.ProteinBindingSites

                    Case = StandardFeatureKeys.RepeatRegion
                        If md.Features.RepeatRegions.Count > 0 Then Return md.Features.RepeatRegions

                    Case = StandardFeatureKeys.ReplicationOrigin
                        If md.Features.ReplicationOrigins.Count > 0 Then Return md.Features.ReplicationOrigins

                    Case = StandardFeatureKeys.RibosomalRna
                        If md.Features.RibosomalRNAs.Count > 0 Then Return md.Features.RibosomalRNAs

                    Case = StandardFeatureKeys.RibosomeBindingSite
                        If md.Features.RibosomeBindingSites.Count > 0 Then Return md.Features.RibosomeBindingSites

                    Case = StandardFeatureKeys.SignalPeptide
                        If md.Features.SignalPeptides.Count > 0 Then Return md.Features.SignalPeptides

                    Case = StandardFeatureKeys.StemLoop
                        If md.Features.StemLoops.Count > 0 Then Return md.Features.StemLoops

                    Case = StandardFeatureKeys.TataSignal
                        If md.Features.TATASignals.Count > 0 Then Return md.Features.TATASignals

                    Case = StandardFeatureKeys.Terminator
                        If md.Features.Terminators.Count > 0 Then Return md.Features.Terminators

                    Case = StandardFeatureKeys.ThreePrimeUtr
                        If md.Features.ThreePrimeUTRs.Count > 0 Then Return md.Features.ThreePrimeUTRs

                    Case = StandardFeatureKeys.TransferMessengerRna
                        If md.Features.TransferMessengerRNAs.Count > 0 Then Return md.Features.TransferMessengerRNAs

                    Case = StandardFeatureKeys.TransferRna
                        If md.Features.TransferRNAs.Count > 0 Then Return md.Features.TransferRNAs

                    Case = StandardFeatureKeys.TransitPeptide
                        If md.Features.TransitPeptides.Count > 0 Then Return md.Features.TransitPeptides

                    Case = StandardFeatureKeys.UnsureSequenceRegion
                        If md.Features.UnsureSequenceRegions.Count > 0 Then Return md.Features.UnsureSequenceRegions

                    Case = StandardFeatureKeys.Variation
                        If md.Features.Variations.Count > 0 Then Return md.Features.Variations

                End Select
                Return Nothing
            End Function
#End Region

#Region "GetFeatureByQulifier"
            Public Shared Function GetFeaturesByQulifiersPerfect(t As List(Of FeatureItem), searchSetting As SettingForSearchInFeaturesAndQulifiers)
                Dim out As New List(Of FeatureItem)
                For Each Qual In searchSetting.SelectedQualifiers
                    For Each s In searchSetting.SearchStringInQulifiers
                        Dim g = From x In t Where x.Qualifiers.ContainsKey(Qual) AndAlso
                                                String.Compare(x.Qualifiers(Qual).First, s, StringComparison.InvariantCultureIgnoreCase)
                        If g.Count > 0 Then out.AddRange(g.ToList)
                    Next
                Next

                Return out
            End Function
            Public Shared Function GetFeaturesByQulifiersContains(t As List(Of FeatureItem), searchSetting As SettingForSearchInFeaturesAndQulifiers)
                Dim out As New List(Of FeatureItem)
                For Each Qual In searchSetting.SelectedQualifiers
                    For Each s In searchSetting.SearchStringInQulifiers
                        Dim g = From x In t Where x.Qualifiers.ContainsKey(Qual) AndAlso
                                                x.Qualifiers(Qual).First.IndexOf(s) >= 0
                        If g.Count > 0 Then out.AddRange(g.ToList)
                    Next
                Next

                Return out
            End Function


#End Region
            Private Function GetExtFeatures(locusTags As List(Of String), featList As ListOf.ExtFeatureList) As List(Of ExtFeature)
                Dim x As New List(Of String)
                Dim Out As New List(Of ExtFeature)
                For Each LocusTag In locusTags

                    tmpExtFeature.LocusTag = LocusTag
                    Dim i1 = featList.FetauresByLocustag.BinarySearch(tmpExtFeature, LocusTagComparer)
                    If i1 >= 0 Then
                        Out.Add(featList.FetauresByLocustag(i1))
                        For i2 = i1 - 1 To 0 Step -1
                            If featList.FetauresByLocustag(i2).LocusTag = LocusTag Then
                                Out.Add(featList.FetauresByLocustag(i2))
                            Else
                                Exit For
                            End If
                        Next
                        For i2 = i1 + 1 To featList.FetauresByLocustag.Count - 1
                            If featList.FetauresByLocustag(i2).LocusTag = LocusTag Then
                                Out.Add(featList.FetauresByLocustag(i2))
                            Else
                                Exit For
                            End If
                        Next
                    End If
                Next
                Return Out
            End Function

            Private Function GetLocusTags(ExtFeat As ExtFeature) As List(Of String)
                Dim LocusTags As New List(Of String) ' First is full, Second is Short LocusTag
                LocusTags.Add(ExtFeat.LocusTag)
                Dim tmp = Split(ExtFeat.LocusTag, ".").First
                If LocusTags.First <> tmp Then LocusTags.Add(tmp)
                Return LocusTags
            End Function

            ''' <summary>
            ''' Correct missing " and Replace "" with Sort The Feature By Start"
            ''' After End Rverse After Gene, mRNA, CDS
            ''' </summary>
            ''' <param name="Seqs"></param>
            Public Sub CorrectFeaturesStartAndEnd(ByRef Seqs As List(Of Bio.Sequence))
                For Each Seq In Seqs
                    If Seq.Metadata.ContainsKey(Bio.Util.Helper.GenBankMetadataKey) Then
                        Dim Md As Bio.IO.GenBank.GenBankMetadata = Seq.Metadata(Bio.Util.Helper.GenBankMetadataKey)
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
                Next ' Seq
            End Sub

            Friend Class MergeFeatureAnnotation
                Private featuresFrom As List(Of String)
                Private featuresTo As List(Of String)
                Public Property Type As String = MyConstants.BackGroundWork.ModyfiedSequence
                Public Property Result As Szunyi.ListOf.SequenceList
                Public Property SeqLists As List(Of Szunyi.ListOf.SequenceList)

                Public Sub New(mergedSeqList As List(Of Szunyi.ListOf.SequenceList),
                               featuresTo As List(Of String),
                               featuresFrom As List(Of String))
                    Me.SeqLists = mergedSeqList
                    Me.featuresTo = featuresTo
                    Me.featuresFrom = featuresFrom
                End Sub
                Public Sub DoIt()
                    ' Get the two ExtFeatureList

                    Dim cSearchSetting = New SettingForSearchInFeaturesAndQulifiers(featuresTo)

                    Dim t As New Szunyi.ListOf.ExtFeatureList(cSearchSetting, Me.SeqLists)
                    t.DoIt()

                    Dim cSearchSetting2 = New SettingForSearchInFeaturesAndQulifiers(featuresFrom)
                    Dim t2 As New Szunyi.ListOf.ExtFeatureList(cSearchSetting2, Me.SeqLists)
                    t2.DoIt()

                    Dim x As New Szunyi.GenBank.FeatureManipulation
                    For Each Feat In t.FetauresByLocustag
                        x.FindAndMerge2Features(Feat, t2)
                    Next

                End Sub
            End Class

            Friend Class Rename
                Private SubType As String
                Private mergedSeqs As List(Of Sequence)
                Private selectedFeatures As List(Of String)
                Private selectedQualifiers As List(Of String)
                Private separator As String
                Private Name As String
                Public Property Type As String = MyConstants.BackGroundWork.ModyfiedSequence
                Public Property SeqList As Szunyi.ListOf.SequenceList

                Public Sub New(mergedSeqs As List(Of Sequence),
                               selectedFeatures As List(Of String),
                               selectedQualifiers As List(Of String),
                               separator As String,
                               SubType As String,
                               Name As String)
                    Me.mergedSeqs = mergedSeqs
                    Me.selectedFeatures = selectedFeatures
                    Me.selectedQualifiers = selectedQualifiers
                    Me.separator = separator
                    Me.SubType = SubType
                    Me.SeqList = New Szunyi.ListOf.SequenceList(mergedSeqs, "RN Features: " & Name)

                End Sub
                Public Sub DoIt()
                    Dim Features = Szunyi.GenBank.FeatureManipulation.GetAllFetureByType(mergedSeqs, selectedFeatures)
                    Select Case Me.SubType
                        Case MyConstants.StringRename.FirstAfterSplit
                            For Each Feat In Features
                                For Each Qual In selectedQualifiers
                                    If Feat.Qualifiers.ContainsKey(Qual) AndAlso Feat.Qualifiers(Qual).Count > 0 Then
                                        Dim x As New List(Of String)
                                        x.Add(Split(Feat.Qualifiers(Qual).First, Me.separator).First)
                                        Feat.Qualifiers(Qual) = x
                                    End If
                                Next
                            Next
                        Case MyConstants.StringRename.LastAfterSplit
                            For Each Feat In Features
                                For Each Qual In selectedQualifiers
                                    If Feat.Qualifiers.ContainsKey(Qual) AndAlso Feat.Qualifiers(Qual).Count > 0 Then
                                        Dim x As New List(Of String)
                                        x.Add(Split(Feat.Qualifiers(Qual).First, Me.separator).Last)
                                        Feat.Qualifiers(Qual) = x
                                    End If
                                Next
                            Next
                    End Select

                End Sub
            End Class

        End Class

        Public Class ExtFeatureManipulation
            Dim LociBuilder As New LocationBuilder
            Public Shared Function GetExtFeatures(searchSetting As SettingForSearchInFeaturesAndQulifiers,
         seqs As List(Of Bio.Sequence)) _
        As List(Of ExtFeature)
                Dim out As New List(Of ExtFeature)
                Dim Features As New List(Of FeatureItem)
                For Each Seq In seqs
                    Try
                        If Seq.Metadata.ContainsKey(Bio.Util.Helper.GenBankMetadataKey) Then
                            Dim x As New List(Of FeatureItem)
                            Dim md As Bio.IO.GenBank.GenBankMetadata = Seq.Metadata(Bio.Util.Helper.GenBankMetadataKey)

                            For Each FeatureType In searchSetting.SelectedFeatures

                                Dim t1 = Szunyi.GenBank.FeatureManipulation.GetFeatureByType(FeatureType, md)
                                If IsNothing(t1) = False Then
                                    Dim t = t1.ToList
                                    If searchSetting.SearchStringInQulifiers.Count > 0 Then
                                        If searchSetting.IsPerfectMatch = True Then
                                            x.AddRange(Szunyi.GenBank.FeatureManipulation.GetFeaturesByQulifiersPerfect(t, searchSetting))
                                        Else
                                            x.AddRange(Szunyi.GenBank.FeatureManipulation.GetFeaturesByQulifiersContains(t, searchSetting))
                                        End If
                                    Else
                                        x.AddRange(t)
                                    End If
                                End If

                            Next
                            For Each Item In x
                                out.Add(New ExtFeature(Item, Seq.ID))
                            Next
                        End If
                    Catch ex As Exception
                        Dim alf As Int16 = 54
                    End Try
                Next
                Return out
            End Function

            ''' <summary>
            ''' Return the String Reprasantioan of Annotation of Features
            ''' Optionally Remove special Strings
            ''' Annotation are Separated by VbCrlf
            ''' </summary>
            ''' <param name="FeatureList"></param>
            ''' <param name="Qulifiers"></param>
            ''' <param name="Separator"></param>
            ''' <param name="ToRemove"></param>
            ''' <returns></returns>
            Public Function GetTextFromExtFeatureList(FeatureList As ListOf.ExtFeatureList,
                                                      Qulifiers As List(Of String), Separator As String, Optional ToRemove As String = "") As String
                Dim str As New System.Text.StringBuilder

                For Each Feature In FeatureList.Features
                    str.Append(GetTextFromExtFeature(Feature.Feature, Qulifiers, Separator, ToRemove))
                    str.AppendLine()
                Next
                str.Length -= 1
                Return str.ToString
            End Function
            ''' <summary>
            ''' Return the String Representation of Selected Qulifiers From Feature
            ''' Optionally Remove Special Strings
            ''' </summary>
            ''' <param name="Feature"></param>
            ''' <param name="Qulifiers"></param>
            ''' <param name="Separator"></param>
            ''' <param name="ToRemove"></param>
            ''' <returns></returns>
            Public Function GetTextFromExtFeature(Feature As FeatureItem,
                                                  Qulifiers As List(Of String),
                                                  Separator As String, Optional ToRemove As String = "") As String
                Dim str As New System.Text.StringBuilder
                For Each Qulifier In Qulifiers
                    If Qulifier = MyConstants.BackGroundWork.Locations Then
                        str.Append(LociBuilder.GetLocationString(Feature.Location)).Append(Separator)
                    Else
                        If Feature.Qualifiers.ContainsKey(Qulifier) = True Then
                            str.Append(Szunyi.Text.GetText(Feature.Qualifiers(Qulifier), " ")).Append(Separator)
                        Else
                            str.Append(vbTab)
                        End If
                    End If
                Next
                If ToRemove = "" Then Return str.ToString
                Return str.ToString.Replace(ToRemove, "")
            End Function

        End Class
    End Module

	Public Class TranscriptDiscovery
		Private BamFile As FileInfo
		Private toList1 As List(Of ListOf.ExtFeatureList)
		Private toList2 As List(Of ListOf.LocationList)

		Public Sub New()

		End Sub

		Public Sub New(toList1 As List(Of ListOf.ExtFeatureList), toList2 As List(Of ListOf.LocationList), BamfIle As FileInfo)
			Me.toList1 = toList1
			Me.toList2 = toList2
			Me.BamFile = BamfIle
		End Sub
		Public Sub DoIt()

		End Sub
	End Class
End Namespace

