Imports System.IO
Imports Bio
Imports Bio.IO.GenBank
Imports ClassLibrary1.Szunyi.ListOf
Imports ClassLibrary1
Imports ClassLibrary1.Szunyi.Sequences.SequenceManipulation
Namespace Szunyi
    Public Class SettingForSearchInFeaturesAndQulifiers
        Public Property SelectedFeatures As New List(Of String)

        Public Property SettingForSearchInQulifier As New List(Of SettingForSearchInQulifier)


        Public Sub New()

        End Sub
        Public Sub New(Type As String)
            Me.SelectedFeatures.Add(Type)
        End Sub
        Public Sub New(Types As List(Of String))
            Me.SelectedFeatures = Types
        End Sub
    End Class
    Public Class SettingForSearchInQulifier
        Public Property QulifierName As String
        Public Property Type As SearchType
        Public Property InterestingStrings As New List(Of String)


        Public Sub New(qualName As String, Type As String, IntStrings As List(Of String))
            Me.QulifierName = qualName
            Select Case Type
                Case "Contains"
                    Me.Type = SearchType.Contains
                Case "Exact"
                    Me.Type = SearchType.Exact
                Case "No Value"
                    Me.Type = SearchType.NoValue
            End Select
            Me.InterestingStrings = IntStrings
        End Sub


    End Class
    Public Class SetSearchII
        Public Property QulifierName As String
        Public Property Type As String
        Public Property InterestingStrings As New List(Of String)
    End Class
    Public Class ListOf

        Public Class SequenceList
            Public Property Files As New List(Of FileInfo)
            Public Property UniqueID As Integer
            Public Property Sequences As New List(Of Bio.ISequence)
            Public Property ShortFileName As String
            Public Property ToolTipText As String
            Public Property Log As String

            Public Property Type As String = Constants.BackGroundWork.Sequences
            Public Sub New(file As FileInfo)
                Files.Add(file)
                Me.ShortFileName = Me.Files.First.Name.Replace(Me.Files.First.Extension, "")
            End Sub
            Public Sub New(files As List(Of FileInfo), Title As String)
                Me.Files.AddRange(files)
                Me.ShortFileName = Title

            End Sub
            Public Sub New(Seqs As List(Of Bio.ISequence), Name As String, ToolTipText As String)
                Me.Sequences = Seqs
                Me.ShortFileName = Name
                Me.ToolTipText = ToolTipText
            End Sub
            Public Sub DoIt()
                Dim t As New Szunyi.IO.Import.Sequence(Files)
                t.DoIt()
                Me.Sequences = t.seqs

            End Sub
        End Class

        Public Class ExtFeatureList
            ''' <summary>
            ''' Sort By Location (SeqID and After StartPositions After EndPosition After LocationString)
            ''' </summary>
            ''' <returns></returns>
            Public Property Features As New List(Of ExtFeature)
            Public Property FetauresByLocustag As New List(Of ExtFeature)
            Public Property SearchSetting As New SettingForSearchInFeaturesAndQulifiers
            Public Property ShortFileName As String
            Public Property UniqueId As Integer
            Public Property Type = Szunyi.Constants.BackGroundWork.Features
            Public Property Seqs As New List(Of Bio.ISequence)
            Public Sub New(SearchSetting As SettingForSearchInFeaturesAndQulifiers,
                           SequenceLists As List(Of Szunyi.ListOf.SequenceList), Optional Type As String = "")
                If Type <> "" Then Me.Type = Type
                Me.SearchSetting = SearchSetting
                Dim t = (From x In SequenceLists Select x.ShortFileName).ToList
                Me.ShortFileName = Szunyi.Text.General.GetText(t, " ")
                Me.Seqs = Merging.MergeSequenceList(SequenceLists)

            End Sub
            Public Sub New(SearchSetting As SettingForSearchInFeaturesAndQulifiers,
                           OriginalSeqs As List(Of Bio.ISequence), Optional Type As String = "")
                If Type <> "" Then Me.Type = Type
                Me.SearchSetting = SearchSetting
                Me.Seqs = OriginalSeqs

            End Sub
            Public Sub New()

            End Sub
            Public Sub New(SearchSetting As SettingForSearchInFeaturesAndQulifiers, SequenceList As Szunyi.ListOf.SequenceList)
                Me.ShortFileName = SequenceList.ShortFileName
                Me.SearchSetting = SearchSetting
                Me.Seqs = SequenceList.Sequences

            End Sub
            Public Sub New(Seqs As List(Of Bio.ISequence), FeatureType As String)

                Me.SearchSetting = New SettingForSearchInFeaturesAndQulifiers(FeatureType)
                Me.Seqs = Seqs
                DoIt()
            End Sub
            Public Sub DoIt()
                Dim Result As List(Of ExtFeature) = Szunyi.Features.ExtFeatureManipulation.GetExtFeatures(Me.SearchSetting, Me.Seqs)
                FetauresByLocustag = Result

                FetauresByLocustag.Sort(Comparares.AllComparares.ByExtFeatureLocusTag)
                Features = Szunyi.ListOf.CloneExtFeatureList(Result)
                Features.Sort(Comparares.AllComparares.ByExtFeatureLocation)
                If SearchSetting.SelectedFeatures.Count = 46 Then
                    Me.ShortFileName = Me.ShortFileName & " All Features"
                Else
                    ShortFileName = Szunyi.Text.General.GetText(SearchSetting.SelectedFeatures, " ") & Me.ShortFileName

                End If

                '  SetIndexes()
            End Sub
            Public Sub SetIt()
                Dim Result As List(Of ExtFeature) = Szunyi.Features.ExtFeatureManipulation.GetExtFeatures(Me.SearchSetting, Me.Seqs)

                FetauresByLocustag = Szunyi.ListOf.CloneExtFeatureList(Result)
                FetauresByLocustag.Sort(Comparares.AllComparares.ByExtFeatureLocusTag)
                Features.Sort(Comparares.AllComparares.ByExtFeatureLocation)


            End Sub
#Region "Settings"

            Private Function GetMaxLength(Features As List(Of ExtFeature)) As Long
                Dim MaxLength As Integer = 0
                For Each item In Features
                    If item.Feature.Location.LocationEnd - item.Feature.Location.LocationStart > MaxLength Then _
                        MaxLength = item.Feature.Location.LocationEnd - item.Feature.Location.LocationStart
                Next
                Return MaxLength
            End Function
            Private Sub SetIndexes()
                If Features.Count < 2 Then Exit Sub
                Dim MaxLength = GetMaxLength(Features)
                Dim cSeqID As String = Features.Last.Seq.ID
                For i1 = Features.Count - 1 To 0 Step -1
                    If Features(i1).Seq.ID = cSeqID Then
                        Features(i1).FirstItemToInvestigate = i1
                        For i2 = i1 - 1 To 0 Step -1
                            If cSeqID = Features(i2).Seq.ID Then
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
                        cSeqID = Features(i1).Seq.ID
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
                    If Features(i1).Seq.ID = cSeqID Then
                        For i2 = i1 + 1 To Features.Count - 1
                            If Features(i1).Feature.Location.LocationEnd < Features(i2).Feature.Location.LocationStart Or
                                cSeqID <> Features(i2).Seq.ID Then
                                Exit For
                            Else
                                Features(i1).LastItemToInvestigate = i2
                            End If
                        Next
                    Else
                        cSeqID = Features(i1).Seq.ID
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
                Dim Index = Features.BinarySearch(x, Comparares.AllComparares.ByExtFeatureLocation)
                Dim tempy As New List(Of ExtFeature)
                If Index = -1 Then
                    Return New List(Of ExtFeature)
                ElseIf Index < -1 Then
                    Index = System.Math.Abs(Index) - 2
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

        Private Shared Function CloneExtFeatureList(result As List(Of ExtFeature)) As List(Of ExtFeature)
            Dim out As New List(Of ExtFeature)
            For Each Item In result
                out.Add(Item)
            Next
            Return out
        End Function


        Public Class ExtFeature

            Private LociBuilder As New Bio.IO.GenBank.LocationBuilder
            Public Property Feature As FeatureItem
            Public Property LocusTag As String
            Public Property FirstItemToInvestigate As Integer
            Public Property LastItemToInvestigate As Integer
            Public Property Count As Integer = 0
            ' Searching/Sorting is faster SeqID And Location
            Public Property LocationString As String
            Public Property Seq As Bio.Sequence
            Public Sub New(feature As FeatureItem, Seq As Bio.Sequence)
                Me.Feature = feature
                Me.Seq = Seq
                If feature.Qualifiers.ContainsKey(StandardQualifierNames.LocusTag) Then
                    Me.LocusTag = feature.Qualifiers(StandardQualifierNames.LocusTag).First.Replace(Chr(34), "")
                End If
                Me.LocationString = Me.Seq.ID & " " & LociBuilder.GetLocationString(feature.Location).Replace("<", "").Replace(">", "")
            End Sub
            Public Sub New(Ltag As String)
                Me.LocusTag = Ltag.Replace(Chr(34), "")
            End Sub

            Public Function GetQuilifierValues(Qulifiers As List(Of String), FivePrime As Integer, ThreePrime As Integer,
                                        Optional Separator As String = " ") As String

                Dim str As New System.Text.StringBuilder
                For Each Qulifier In Qulifiers
                    If Qulifier = Szunyi.Constants.BackGroundWork.Locations Then
                        str.Append(LociBuilder.GetLocationString(Me.Feature.Location)).Append(Separator)
                    Else
                        If Me.Feature.Qualifiers.ContainsKey(Qulifier) = True Then
                            str.Append(Szunyi.Text.General.GetText(Me.Feature.Qualifiers(Qulifier), Separator)).Append(Separator)
                        Else
                            str.Append(Separator)
                        End If
                    End If
                Next
                If str.Length >= Separator.Length Then str.Length -= Separator.Length
                If FivePrime <> 0 Then str.Append("|5' Extension " & FivePrime)
                If ThreePrime <> 0 Then str.Append("|3' Extension " & ThreePrime)
                Return str.ToString
            End Function

            Public Function GetQuilifierValue(Qulifier As String, FivePrime As Integer, ThreePrime As Integer,
                                    Optional Separator As String = " ") As String

                Dim str As New System.Text.StringBuilder

                If Me.Feature.Qualifiers.ContainsKey(Qulifier) = True Then
                    str.Append(Szunyi.Text.General.GetText(Me.Feature.Qualifiers(Qulifier), Separator))
                Else
                    str.Append(Separator)
                End If

                str.Length -= Separator.Length
                If FivePrime <> 0 Then str.Append("|5' Extension " & FivePrime)
                If ThreePrime <> 0 Then str.Append("|3' Extension " & ThreePrime)
                Return str.ToString
            End Function

        End Class


    End Class
End Namespace


Public Enum SearchType
    Exact = 0
    Contains = 1
    NoValue = 2
    NotConsistOf = 3
    NotExactValue = 4
End Enum

Namespace Szunyi
    Namespace mRNA
        Namespace Transcript
            Public Class Transcript
                Public Property CDS As ExtFeature
                Public Property Gene As ExtFeature
                Public Property LocusTag As String
                Public Property ShortLocusTag As String
                Public Property SeqID As String
                Public Property GeneLocationStartOld As Long
                Public Property GeneLocationEndOld As Long
                Public Property GeneLocationStartNew As Long
                Public Property GeneLocationEndNew As Long
                Public Property TheError As String

                Public Property CDSLocationStart As Long
                Public Property CDSLocationEnd As Long


                Public Property PromoterSeq As String = ""
                Public Property CDSSeq As String = ""
                Public Property ThreePrimerUTRSeq As String = ""
                Public Property ExtraThreePrimeUTRSeq As String = ""
                Public Property ExtraPromoterSeq As String = ""
                Public Property Seq As String = ""
                Public Property Product As String = ""
                Public Property PromoterSeqLength As Integer
                Public Property CDSSeqLength As Integer
                Public Property ThreePrimerUTRSeqLength As Integer
                Public Property ExtraThreePrimeUTRSeqLength As Integer
                Public Property ExtraPromoterSeqLength As Integer
                Public Property SeqLength As Integer
                Public Function GetSeqAsFasta() As Bio.Sequence
                    Dim x As New Bio.Sequence(Bio.Alphabets.AmbiguousDNA, Me.Seq)
                    x.ID = Me.LocusTag & "|" & Me.Product & "|" & Me.ExtraPromoterSeqLength & "|" &
                    Me.PromoterSeqLength & "|" & Me.CDSSeqLength & "|" & Me.ThreePrimerUTRSeqLength &
                    "|" & Me.ExtraThreePrimeUTRSeqLength
                    Return x

                End Function
                Public Function GetSeqAsGenBank() As Bio.Sequence
                    Dim x As New Bio.Sequence(Bio.Alphabets.AmbiguousDNA, Me.Seq)
                    Dim Md = Szunyi.Features.FeatureManipulation.GenBankMetaDataManipulation.CreateNAGenBankMetaData(x)
                    If Me.PromoterSeqLength > 0 Or Me.ExtraPromoterSeqLength > 0 Then
                        Dim p As New Promoter("1.." & Me.PromoterSeqLength + Me.ExtraPromoterSeqLength)
                        Md.Features.All.Add(p)
                    End If
                    x.ID = Me.CDS.LocusTag
                    Dim TotalPromoterLength = Me.PromoterSeqLength + Me.ExtraPromoterSeqLength + 1
                    If TotalPromoterLength = 0 Then TotalPromoterLength += 1
                    Dim CDS As New CodingSequence(TotalPromoterLength & ".." &
                                              TotalPromoterLength + Me.CDSSeqLength - 1)
                    Dim TotalPromoterAndCDSLength = TotalPromoterLength + Me.CDSSeqLength

                    Szunyi.Features.FeatureManipulation.MergeFeatures.Merge2Features(Me.CDS.Feature, CDS, True)
                    Md.Features.All.Add(CDS)
                    If Me.ExtraThreePrimeUTRSeqLength > 0 Or Me.ThreePrimerUTRSeqLength > 0 Then
                        Dim utr As New Bio.IO.GenBank.ThreePrimeUtr(TotalPromoterAndCDSLength &
                        ".." & x.Count)
                        Md.Features.All.Add(utr)
                    End If
                    x.Metadata.Add(Bio.Util.Helper.GenBankMetadataKey, Md)
                    Return x
                End Function
                Public Shared Function ReName_Default(Seq As Bio.ISequence, TRs As List(Of FeatureItem)) As Bio.ISequence
                    Szunyi.Features.FeatureManipulation.GenBankMetaDataManipulation.Remove_Feature(Seq, TRs)
                    Dim CDSs = Szunyi.Features.FeatureManipulation.GetFeaturesByType.GetFeturesByTypeFromSeq(Seq, StandardFeatureKeys.CodingSequence)
                    For Each Tr In TRs
                        Szunyi.Features.FeatureManipulation.Qulifiers.Add(Tr, StandardQualifierNames.Note, Tr.Label)
                        Dim cCDSs_woOrientation = Szunyi.Location.OverLapping_Locations.Get_Inner_Feature_Items_woOrientation(Tr, CDSs)
                        Dim cCDSs_wOrientation = Szunyi.Location.OverLapping_Locations.Get_Inner_Feature_Items_wOrientation(Tr, CDSs)
                        If cCDSs_wOrientation.Count = 0 Then ' No Fully Overlapped CDS
                            Dim truncated_S = Szunyi.Location.OverLapping_Locations.Get_Longest_OverLapping_Item_wOrientation(Tr, CDSs)
                            Dim truncated_AS = Szunyi.Location.OverLapping_Locations.Get_Longest_OverLapping_Item_woOrientation(Tr, CDSs)

                        End If
                    Next
                End Function
            End Class
            Public Class TranscriptManipulationSettings
                Public Property PromoterNeeded As Boolean = False
                Public Property PromoterToCDSStart As Boolean = False
                Public Property ConstantPromoterBeforeCDS As Boolean = False
                Public Property ConstantPromoterBeforeCDSLength As Integer = 0
                Public Property ExtraPromoterLengthBeforeGene As Integer = 0

                Public Property UTRNeeded As Boolean = False
                Public Property UTRToEndOfGene As Boolean = False
                Public Property ConstantUTRAfterCDS As Boolean = False
                Public Property ConstantUTRAfterCDSLength As Integer = 0
                Public Property ExtraUTRLengthAfterGene As Integer = 0

                Public Sub New()

                End Sub
            End Class
            Public Class TranscriptManipulation
                '  Dim SequenceManipulator As New Szunyi.Sequences.SequenceManinpulation
                Dim Settings As TranscriptManipulationSettings
                Dim LociBuilder As New Bio.IO.GenBank.LocationBuilder
                Public Sub New(Settings As TranscriptManipulationSettings)
                    Me.Settings = Settings

                End Sub

                Public Function GetTranscript(Gene As ExtFeature, CDS As ExtFeature, chromosome As Sequence) As Transcript
                    Dim newTranscript As New Transcript
                    SetCDS(newTranscript, CDS)
                    SetGene(newTranscript, Gene)
                    SetNewLocation(newTranscript, CDS, Gene, chromosome)
                    SetLengths(newTranscript)

                    Return newTranscript
                End Function
                Public Sub SetLengths(ByRef newTranscript As Transcript)
                    newTranscript.ExtraPromoterSeqLength = newTranscript.ExtraPromoterSeq.Length
                    newTranscript.PromoterSeqLength = newTranscript.PromoterSeq.Length
                    newTranscript.CDSSeqLength = newTranscript.CDSSeq.Length
                    newTranscript.ThreePrimerUTRSeqLength = newTranscript.ThreePrimerUTRSeq.Length
                    newTranscript.ExtraThreePrimeUTRSeqLength = newTranscript.ExtraThreePrimeUTRSeq.Length
                End Sub

                Public Sub SetNewLocation(newTranscript As Transcript, CDS As ExtFeature, Gene As ExtFeature, Chromosome As Sequence)
                    If Gene.Feature.Location.Operator = LocationOperator.Complement Then
                        SetNewComplementLocation(newTranscript, CDS, Gene, Chromosome)
                    Else
                        SetNewNotComplementLocation(newTranscript, CDS, Gene, Chromosome)
                    End If
                    With newTranscript
                        .Seq = .ExtraPromoterSeq & .PromoterSeq & .CDSSeq & .ThreePrimerUTRSeq & .ExtraThreePrimeUTRSeq
                    End With
                End Sub

                Public Sub SetNewNotComplementLocation(newTranscript As Transcript, CDS As ExtFeature, gene As ExtFeature, chromosome As Sequence)

                    '  newTranscript.CDSSeq = Szunyi.Sequences.SequenceManipulation.SeqsToString.FromFeature(chromosome, CDS.Feature).ConvertToString

                    If Me.Settings.PromoterNeeded = True Then
                        If Me.Settings.ConstantPromoterBeforeCDS = True Then
                            newTranscript.PromoterSeq = SeqsToString.SeqFromStartAndEndAsString(chromosome,
                                                                            CDS.Feature.Location.LocationStart,
                                                                            Me.Settings.ConstantPromoterBeforeCDSLength)

                        ElseIf Me.Settings.PromoterToCDSStart = True Then
                            newTranscript.PromoterSeq =
                            SeqsToString.SeqFromStartAndEndAsString(chromosome,
                                                                            gene.Feature.Location.LocationStart,
                                                                            CDS.Feature.Location.LocationStart - 1)
                            If Me.Settings.ExtraPromoterLengthBeforeGene > 0 Then
                                newTranscript.ExtraPromoterSeq =
                                SeqsToString.SeqFromStartAndEndAsString(chromosome,
                                                                             gene.Feature.Location.LocationStart -
                                                                            Me.Settings.ExtraPromoterLengthBeforeGene,
                                                                             gene.Feature.Location.LocationStart)
                            End If
                        End If
                    End If
                    If Me.Settings.UTRNeeded = True Then
                        If Me.Settings.ConstantUTRAfterCDS = True Then

                            newTranscript.ThreePrimerUTRSeq =
                                SeqsToString.SeqFromStartAndLengthAsString(chromosome,
                                                                            CDS.Feature.Location.LocationEnd,
                                                                            Me.Settings.ConstantUTRAfterCDSLength)
                        ElseIf Me.Settings.UTRToEndOfGene = True Then
                            newTranscript.ThreePrimerUTRSeq =
                                SeqsToString.SeqFromStartAndEndAsString(chromosome,
                                                                            CDS.Feature.Location.LocationEnd,
                                                                             gene.Feature.Location.LocationEnd)
                            If Me.Settings.ExtraUTRLengthAfterGene > 0 Then
                                newTranscript.ExtraThreePrimeUTRSeq =
                                    SeqsToString.SeqFromStartAndLengthAsString(chromosome,
                                                                             gene.Feature.Location.LocationEnd,
                                                                            Me.Settings.ExtraUTRLengthAfterGene)
                            End If
                        End If
                    End If




                End Sub

                Private Sub SetNewComplementLocation(newTranscript As Transcript, cDS As ExtFeature, gene As ExtFeature, chromosome As Sequence)


                    '     newTranscript.CDSSeq = Szunyi.Sequences.SequenceManipulation.SeqsToString.FromFeature(chromosome, cDS.Feature).ConvertToString

                    If Me.Settings.PromoterNeeded = True Then
                        If Me.Settings.ConstantPromoterBeforeCDS = True Then ' Not Done
                            newTranscript.PromoterSeq =
                            SeqsToString.SeqFromStartAndLengthAsStringReverseComplemented(chromosome,
                                                                            cDS.Feature.Location.LocationEnd,
                                                                            Me.Settings.ConstantPromoterBeforeCDSLength)

                        ElseIf Me.Settings.PromoterToCDSStart = True Then ' Done
                            newTranscript.PromoterSeq =
                            SeqsToString.SeqFromStartAndEndAsStringReverseComplemented(chromosome,
                                                                            cDS.Feature.Location.LocationEnd,
                                                                            gene.Feature.Location.LocationEnd)
                            If Me.Settings.ExtraPromoterLengthBeforeGene > 0 Then 'Done
                                newTranscript.ExtraPromoterSeq =
                                SeqsToString.SeqFromStartAndLengthAsStringReverseComplemented(chromosome,
                                                                             gene.Feature.Location.LocationEnd,
                                                                            Me.Settings.ExtraPromoterLengthBeforeGene)
                            End If
                        End If
                    End If
                    If Me.Settings.UTRNeeded = True Then
                        If Me.Settings.ConstantUTRAfterCDS = True Then ' Not Done

                            newTranscript.ThreePrimerUTRSeq =
                            SeqsToString.SeqFromStartAndEndAsString(chromosome,
                                                                        cDS.Feature.Location.LocationEnd,
                                                                        Me.Settings.ConstantUTRAfterCDSLength)
                        ElseIf Me.Settings.UTRToEndOfGene = True Then ' Done
                            newTranscript.ThreePrimerUTRSeq =
                               SeqsToString.SeqFromStartAndEndAsStringReverseComplemented(chromosome,
                                                                               gene.Feature.Location.LocationStart,
                                                                              cDS.Feature.Location.LocationStart - 1)
                            If Me.Settings.ExtraUTRLengthAfterGene > 0 Then ' Done
                                newTranscript.ExtraThreePrimeUTRSeq =
                                    SeqsToString.SeqFromStartAndLengthAsStringReverseComplemented(chromosome,
                                                                                  gene.Feature.Location.LocationStart -
                                                                                 Me.Settings.ExtraUTRLengthAfterGene,
                                                                                   Me.Settings.ExtraUTRLengthAfterGene)
                            End If
                        End If
                    End If

                End Sub



                Public Shared Sub SetGene(newTranscript As Transcript, Gene As ExtFeature)
                    newTranscript.ShortLocusTag = Gene.LocusTag
                    newTranscript.GeneLocationStartOld = Gene.Feature.Location.LocationStart
                    newTranscript.GeneLocationEndOld = Gene.Feature.Location.LocationEnd
                    newTranscript.Gene = Gene
                End Sub

                Public Shared Sub SetCDS(ByRef newTranscript As Transcript, CDS As ExtFeature)
                    newTranscript.LocusTag = CDS.LocusTag
                    newTranscript.CDSLocationStart = CDS.Feature.Location.LocationStart
                    newTranscript.CDSLocationEnd = CDS.Feature.Location.LocationEnd
                    newTranscript.CDS = CDS
                End Sub

                Public Function ConvertTranscripts(transciptList As List(Of Transcript)) As List(Of Sequence)
                    Dim Out As New List(Of Sequence)

                    For Each Tr In transciptList

                        Out.Add(Tr.GetSeqAsGenBank)

                    Next
                    Return Out
                End Function
            End Class
            Public Enum CountBy
                Smallest = 1
                Unique = 2
                All = 3


            End Enum

        End Namespace

    End Namespace
End Namespace