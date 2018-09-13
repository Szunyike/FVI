Imports System.IO
Imports Bio
Imports Bio.IO.GenBank

Namespace Szunyi
	Module GenBank
		Public Class Features
			Public LociBuilder As New Bio.IO.GenBank.LocationBuilder
			Public Property SearchSetting As SettingForSearchInFeaturesAndQulifiers
			Public Property SequenceLists As List(Of Szunyi.ListOf.SequenceList)
			Public ReadOnly Property Type = "Features"
			Public Sub New(SearchSetting As SettingForSearchInFeaturesAndQulifiers, SequenceLists As List(Of Szunyi.ListOf.SequenceList))
				Me.SearchSetting = SearchSetting
				Me.SequenceLists = SequenceLists
			End Sub
			Public Sub New()

			End Sub
			Public Function GetExtFeatures() As Szunyi.ListOf.ExtFeatureList
				Dim out As New Szunyi.ListOf.ExtFeatureList
				For Each SeqList In SequenceLists
					out.Features.AddRange(GetExtFeaturesWithOutSorting(SearchSetting, SeqList).Features)
					out.Seqs.AddRange(SeqList.Sequences)
				Next
				out.Seqs.Sort(New Comparares.SequenceIDComparer)

				out.ShortFileName = Szunyi.Text.GetText(SearchSetting.SelectedFeatures, " ")
				Dim t = (From x In Me.SequenceLists Select x.ShortFileName).ToList
				out.ShortFileName = out.ShortFileName & " " & Szunyi.Text.GetText(t, " ")
				out.SetInvestigateItems()
				Return out
			End Function
			Public Function GetExtFeatures(searchSetting As SettingForSearchInFeaturesAndQulifiers, SequenceLists As List(Of Szunyi.ListOf.SequenceList)) _
				As Szunyi.ListOf.ExtFeatureList
				Dim out As New Szunyi.ListOf.ExtFeatureList
				For Each SeqList In SequenceLists
					out.Features.AddRange(GetExtFeatures(searchSetting, SeqList).Features)
				Next

				out.SetInvestigateItems()
				Return out

			End Function

			Public Function GetExtFeatures(searchSetting As SettingForSearchInFeaturesAndQulifiers, seqs As Szunyi.ListOf.SequenceList)	_
				As Szunyi.ListOf.ExtFeatureList
				Dim out = GetExtFeaturesWithOutSorting(searchSetting, seqs)
				out.SetInvestigateItems()
				Return out

			End Function
			Private Function GetExtFeaturesWithOutSorting(searchSetting As SettingForSearchInFeaturesAndQulifiers,
					 seqs As Szunyi.ListOf.SequenceList) _
					As Szunyi.ListOf.ExtFeatureList
				Dim out As New Szunyi.ListOf.ExtFeatureList
				For Each Seq In seqs.Sequences
					If Seq.Metadata.ContainsKey(Bio.Util.Helper.GenBankMetadataKey) Then
						Dim x As New List(Of FeatureItem)
						Dim md As Bio.IO.GenBank.GenBankMetadata = Seq.Metadata(Bio.Util.Helper.GenBankMetadataKey)

						For Each FeatureType In searchSetting.SelectedFeatures
							Dim t = GetFeatureByType(FeatureType, md).ToList
							If searchSetting.SearchStringInQulifiers.Count > 0 Then
								If searchSetting.IsPerfectMatch = True Then
									x.AddRange(GetPerfectMathces(t, searchSetting))
								Else
									x.AddRange(GettMathcesContains(t, searchSetting))
								End If
							Else
								x.AddRange(t)
							End If
						Next
						For Each Item In x
							out.Features.Add(New ExtFeature(Item, Seq.ID))
						Next
					End If
				Next
				Return out
			End Function
			Private Shared Function GetPerfectMathces(t As List(Of FeatureItem), searchSetting As SettingForSearchInFeaturesAndQulifiers)
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
			Private Shared Function GettMathcesContains(t As List(Of FeatureItem), searchSetting As SettingForSearchInFeaturesAndQulifiers)
				Dim out As New List(Of FeatureItem)
				For Each Qual In searchSetting.SelectedQualifiers
					For Each s In searchSetting.SearchStringInQulifiers
						Dim g = From x In t Where x.Qualifiers.ContainsKey(Qual) AndAlso
												x.Qualifiers(Qual).First.IndexOf(s) > 0
						If g.Count > 0 Then out.AddRange(g.ToList)
					Next
				Next

				Return out
			End Function
#Region "GetAnnotation"
			Shared Function GetFeatureByType(Type As String, md As Bio.IO.GenBank.GenBankMetadata) As IEnumerable(Of FeatureItem)
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
				Return New List(Of FeatureItem)
			End Function
#Region "GetAnnotation"
			Public Function GetTextFromExtFeaturesLists(FeatureLists As List(Of ListOf.ExtFeatureList),
														Qualifiers As List(Of String), Separator As String, Optional ToRemove As String = "") As String
				Dim str As New System.Text.StringBuilder
				Dim h As New Features(Nothing, Nothing)
				For Each FeatureList In FeatureLists
					str.Append(GetTextFromExtFeatureList(FeatureList, Qualifiers, Separator, ToRemove))
				Next
				str.Length -= 1
				Return str.ToString
			End Function

			Public Function GetTextFromExtFeatureList(FeatureList As ListOf.ExtFeatureList,
													  Qulifiers As List(Of String), Separator As String, Optional ToRemove As String = "") As String
				Dim str As New System.Text.StringBuilder
				Dim h As New Features(Nothing, Nothing)

				For Each Feature In FeatureList.Features
					str.Append(GetTextFromExtFeature(Feature.Feature, Qulifiers, Separator, ToRemove))
					str.AppendLine()
				Next
				str.Length -= 1
				Return str.ToString
			End Function

			Public Function GetTextFromExtFeature(Feature As FeatureItem,
												  Qulifiers As List(Of String),
												  Separator As String, Optional ToRemove As String = "") As String
				Dim str As New System.Text.StringBuilder
				For Each Qulifier In Qulifiers
					If Qulifier = MyConstants.Locations Then
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


#End Region




#End Region
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

