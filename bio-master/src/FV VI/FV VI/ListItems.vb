Imports System.IO
Imports Bio.Core
Imports Bio
Imports Bio.IO.GenBank

Namespace Szunyi
	Public Class ListOf

		Public Class SequenceList
			Public Property Files As New List(Of FileInfo)
			Public Property UniqueID As Integer
			Public Property Sequences As New List(Of Sequence)
			Public Property ShortFileName As String
			Public Property Log As String

			Public Property Type As String = MyConstants.Sequences
			Public Sub New(file As FileInfo, ID As Integer)
				Files.Add(file)
				Me.UniqueID = ID
				Me.ShortFileName = Me.Files.First.Name.Replace(Me.Files.First.Extension, "")
			End Sub
			Public Sub New(files As List(Of FileInfo), ID As Integer)
				Me.Files.AddRange(files)
				Me.UniqueID = ID
				Me.ShortFileName = Szunyi.IO.Files.GetShortFileName(files)

			End Sub
			Public Sub New(ID As Integer, Seqs As List(Of Sequence), Name As String)
				Me.Sequences = Seqs
				Me.ShortFileName = Name
				Me.UniqueID = ID
			End Sub
			Public Sub DoIt()
				Dim t As New Szunyi.IO.Import.Sequence(Files)
				t.DoIt()
				Me.Sequences = t.seqs

			End Sub
		End Class

		Public Class ExtFeatureList
			Public Property Features As New List(Of ExtFeature)
			Public Property FetauresByLocustag As New List(Of ExtFeature)

			Public dSeqIDs As New List(Of String)

			Dim MyComparerLocation As New Szunyi.Comparares.ExtFeatureLocationComparer
			Dim MyComparerLocusTag As New Szunyi.Comparares.ExtFeatureLocusTagComparer
			Public Property ShortFileName As String
			Public Property UniqueId As Integer
			Public ReadOnly Property Type = MyConstants.Features
			Public Property Seqs As New List(Of Bio.Sequence)
#Region "Settings"
			Public Sub SetInvestigateItems()
				Me.Seqs.Sort(New Comparares.SequenceIDComparer)
				Me.Features.Sort(MyComparerLocation)
				Me.FetauresByLocustag.AddRange(Me.Features)
				Me.FetauresByLocustag.Sort(MyComparerLocusTag)

				Dim SeqIDs = From x In Features Select x.SeqID Distinct


				If SeqIDs.Count > 0 Then dSeqIDs = SeqIDs.ToList

				SetIndexes()
			End Sub
			Private Function GetMaxLength(Features As List(Of ExtFeature)) As Long
				Dim MaxLength As Integer = 0
				For Each item In Features
					If item.Feature.Location.LocationEnd - item.Feature.Location.LocationStart > MaxLength Then	_
					MaxLength = item.Feature.Location.LocationEnd - item.Feature.Location.LocationStart
				Next
				Return MaxLength
			End Function
			Private Sub SetIndexes()
				If Features.Count < 2 Then Exit Sub
				Dim MaxLength = GetMaxLength(Features)
				Dim cSeqID As String = Features.Last.SeqID
				For i1 = Features.Count - 1 To 0 Step -1
					If Features(i1).SeqID = cSeqID Then
						Features(i1).FirstItemToInvestigate = i1
						For i2 = i1 - 1 To 0 Step -1
							If cSeqID = Features(i2).SeqID Then
								If Features(i2).Feature.Location.LocationEnd >= Features(i1).Feature.Location.LocationStart Then
									Features(i1).FirstItemToInvestigate = i2
								End If
								If Features(i2).Feature.Location.LocationStart + MaxLength < Features(i1).Feature.Location.LocationStart Then
									Exit For
								End If
							Else
								Exit For
							End If

						Next
					Else
						' Next sequence
						cSeqID = Features(i1).SeqID
						If i1 = 0 Then
							Features(i1).FirstItemToInvestigate = 0
							Features(i1).LastItemToInvestigate = 0
							Exit For
						Else
							i1 += 1
						End If
					End If
				Next

				For i1 = 0 To Features.Count - 1
					Features(i1).LastItemToInvestigate = i1
					If Features(i1).SeqID = cSeqID Then
						For i2 = i1 + 1 To Features.Count - 1
							If Features(i1).Feature.Location.LocationEnd < Features(i2).Feature.Location.LocationStart Or
							cSeqID <> Features(i2).SeqID Then
								Exit For
							Else
								Features(i1).LastItemToInvestigate = i2
							End If
						Next
					Else
						cSeqID = Features(i1).SeqID
						If i1 = Features.Count - 1 Then
							Features(i1).LastItemToInvestigate = i1
							Exit For
						Else
							i1 = i1 - 1
						End If
					End If

				Next
			End Sub
#End Region

			Public Function GetOverLappedItems(x As ExtFeature) As List(Of ExtFeature)
				Dim Index = Features.BinarySearch(x, MyComparerLocation)
				Dim tempy As New List(Of ExtFeature)
				If Index = -1 Then
					Return New List(Of ExtFeature)
				ElseIf Index < -1 Then
					Index = Math.Abs(Index) - 2
				End If
				Dim s = Features(Index).FirstItemToInvestigate - 1
				If s < 0 Then s = 0
				Dim e = Features(Index).FirstItemToInvestigate + 1
				If e = Features.Count Then e -= 1
				For i1 = s To e
					If Features(i1).Feature.Location.LocationStart >= x.Feature.Location.LocationStart _
					And Features(i1).Feature.Location.LocationStart <= x.Feature.Location.LocationEnd Then
						tempy.Add(Features(i1))
					ElseIf Features(i1).Feature.Location.LocationEnd <= x.Feature.Location.LocationEnd And
					Features(i1).Feature.Location.LocationEnd >= x.Feature.Location.LocationStart Then
						tempy.Add(Features(i1))
					ElseIf Features(i1).Feature.Location.LocationStart < x.Feature.Location.LocationStart And
					x.Feature.Location.LocationEnd > x.Feature.Location.LocationEnd Then
						tempy.Add(Features(i1))
					End If
				Next



				Return tempy

			End Function


		End Class


		Public Class LocationList
			Dim MyComparer As New Szunyi.Comparares.LocationComparer
			Public Property dSeqIDs As List(Of String)
			Public Property Files As New List(Of FileInfo)
			Public Property UniqueID As Integer
			Public Property Locations As New List(Of Szunyi.Location)
			Public Property ShortFileName As String
			Public Property Log As String

			Public Property Type As String = MyConstants.Locations
			Public Property SubType As String
			Public Property Header As String
			Public Sub New(files As List(Of FileInfo))
				Me.ShortFileName = Szunyi.IO.Files.GetShortFileName(files)
				Me.Files = files
				Me.SubType = MyConstants.Locations
			End Sub
			Public Sub New(Features As ExtFeatureList)
				For Each Feat In Features.Features
					Dim x As New Location(Feat)
					Me.Locations.Add(x)
				Next
				Me.SubType = MyConstants.Features
			End Sub

			Public Sub DoIt()
				' First is Header
				' First Column Is SeqID
				' Second Column is LocationStart
				' Third Column Is Location End

				For Each File In Files
					Using sr As New StreamReader(File.FullName)
						Header = sr.ReadLine
						Do
							Dim Line As String = sr.ReadLine
							Dim x As New Szunyi.Location(Line)
							Me.Locations.Add(x)
						Loop Until sr.EndOfStream = True
					End Using
				Next
				Me.Locations.Sort(MyComparer)
				SetIndexes()
			End Sub

			Public Sub SetInvestigateItems()
				Me.Locations.Sort(MyComparer)
				Dim SeqIDs = From x In Locations Select x.SeqID Distinct

				If SeqIDs.Count > 0 Then dSeqIDs = SeqIDs.ToList

				SetIndexes()
			End Sub
			Private Function GetMaxLength(Locis As List(Of Location)) As Long
				Dim MaxLength As Integer = 0
				For Each Loci In Locis
					If Loci.Endy - Loci.Start > MaxLength Then
						MaxLength = Loci.Endy - Loci.Start
					End If
				Next
				Return MaxLength
			End Function
			Private Sub SetIndexes()
				If Locations.Count < 2 Then Exit Sub
				Dim MaxLength = GetMaxLength(Locations)
				Dim cSeqID As String = Locations.Last.SeqID
				For i1 = Locations.Count - 1 To 0 Step -1
					If Locations(i1).SeqID = cSeqID Then
						Locations(i1).FirstItemToInvestigate = i1
						For i2 = i1 - 1 To 0 Step -1
							If cSeqID = Locations(i2).SeqID Then
								If Locations(i2).Endy >= Locations(i1).Start Then
									Locations(i1).FirstItemToInvestigate = i2
								End If
								If Locations(i2).Start + MaxLength < Locations(i1).Start Then
									Exit For
								End If
							Else
								Exit For
							End If

						Next
					Else
						' Next sequence
						cSeqID = Locations(i1).SeqID
						If i1 = 0 Then
							Locations(i1).FirstItemToInvestigate = 0
							Locations(i1).LastItemToInvestigate = 0
							Exit For
						Else
							i1 += 1
						End If
					End If
				Next

				For i1 = 0 To Locations.Count - 1
					Locations(i1).LastItemToInvestigate = i1
					If Locations(i1).SeqID = cSeqID Then
						For i2 = i1 + 1 To Locations.Count - 1
							If Locations(i1).Endy < Locations(i2).Start Or
								cSeqID <> Locations(i2).SeqID Then
								Exit For
							Else
								Locations(i1).LastItemToInvestigate = i2
							End If
						Next
					Else
						cSeqID = Locations(i1).SeqID
						If i1 = Locations.Count - 1 Then
							Locations(i1).LastItemToInvestigate = i1
							Exit For
						Else
							i1 = i1 - 1
						End If
					End If

				Next
			End Sub
		End Class


	End Class

	Public Class ExtFeature

		Private LociBuilder As New Bio.IO.GenBank.LocationBuilder
		Public Property Feature As FeatureItem
		Public Property LocusTag As String
		Public SeqID As String
		Public Property FirstItemToInvestigate As Integer
		Public Property LastItemToInvestigate As Integer
		Public Property Count As Integer = 0
		Public Sub New(feature As FeatureItem, iD As String)
			Me.Feature = feature
			Me.SeqID = iD
			If feature.Qualifiers.ContainsKey(StandardQualifierNames.LocusTag) Then
				Me.LocusTag = feature.Qualifiers(StandardQualifierNames.LocusTag).First.Replace(Chr(34), "")
			End If
		End Sub
		Public Sub New(Ltag As String)
			Me.LocusTag = Ltag.Replace(Chr(34), "")
		End Sub

		Public Function GetQuilifierValues(Qulifiers As List(Of String), FivePrime As Integer, ThreePrime As Integer,
								Optional Separator As String = " ") As String

			Dim str As New System.Text.StringBuilder
			For Each Qulifier In Qulifiers
				If Qulifier = MyConstants.Locations Then
					str.Append(LociBuilder.GetLocationString(Me.Feature.Location)).Append(Separator)
				Else
					If Me.Feature.Qualifiers.ContainsKey(Qulifier) = True Then
						str.Append(Szunyi.Text.GetText(Me.Feature.Qualifiers(Qulifier), Separator))
					Else
						str.Append(Separator)
					End If
				End If
			Next
			str.Length -= Separator.Length
			If FivePrime <> 0 Then str.Append("|5' Extension " & FivePrime)
			If ThreePrime <> 0 Then str.Append("|3' Extension " & ThreePrime)
			Return str.ToString
		End Function

		Public Function GetQuilifierValue(Qulifier As String, FivePrime As Integer, ThreePrime As Integer,
							Optional Separator As String = " ") As String

			Dim str As New System.Text.StringBuilder

			If Me.Feature.Qualifiers.ContainsKey(Qulifier) = True Then
				str.Append(Szunyi.Text.GetText(Me.Feature.Qualifiers(Qulifier), Separator))
			Else
				str.Append(Separator)
			End If

			str.Length -= Separator.Length
			If FivePrime <> 0 Then str.Append("|5' Extension " & FivePrime)
			If ThreePrime <> 0 Then str.Append("|3' Extension " & ThreePrime)
			Return str.ToString
		End Function

	End Class
	Public Class Location
		Public Property FirstItemToInvestigate As Integer
		Public Property Count As Integer
		Public Property LastItemToInvestigate As Integer
		Public Property SeqID As String
		Public Property Start As Integer
		Public Property Endy As Integer

		Public Property Extra As Object
		Public Sub New(Line As String)
			Dim s1() = Split(Line, vbTab)
			Me.SeqID = s1(0)
			Me.Start = s1(1)
			Me.Endy = s1(2)
			Me.Extra = s1
		End Sub
		Public Sub New(Feat As ExtFeature)
			Me.SeqID = Feat.SeqID
			Me.Start = Feat.Feature.Location.LocationStart
			Me.Endy = Feat.Feature.Location.LocationEnd
			Me.Extra = Feat
		End Sub
	End Class

End Namespace


