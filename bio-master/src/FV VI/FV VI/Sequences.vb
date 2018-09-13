Imports System.IO
Imports Bio
Imports Bio.IO.GenBank
Imports FM_V.Szunyi.ListOf
Imports Format_Manipulator3.Szunyi.ListOf

Namespace Szunyi
    Namespace Sequences
        Public Class MaintainUniqueReads
            Public Property Files As New List(Of FileInfo)
            Public Property MinNofReads As Integer
            Public Property Res As New Dictionary(Of String, List(Of SeqwCount))

            Public ReadOnly Property Type As String = MyConstants.MaintainUniqeSequence
            Public Sub New(Files As List(Of FileInfo), MinNofReads As Integer)
                Me.Files = Files
                Me.MinNofReads = MinNofReads
            End Sub
            Public Sub New(Folder As String, MinNofReads As Integer)
                Me.MinNofReads = MinNofReads
                Me.Files = Szunyi.IO.Files.GetAllFilesFromFolder(Folder)

            End Sub
            Public Sub DoIt()
                For Each File In Files
                    Dim x As New FileStream(File.FullName, FileMode.Open)
                    Dim fa = Bio.IO.SequenceParsers.FindParserByFileName(File.FullName)
                    If IsNothing(fa) = False Then
                        Dim Rest As New Dictionary(Of String, List(Of SeqwCount))
                        For Each Seq In fa.Parse(x)
                            Dim t = New SeqwCount(Seq)
                            If Rest.ContainsKey(t.sSeq) = False Then Rest.Add(t.sSeq, New List(Of SeqwCount))
                            Dim Founded As Boolean = False
                            For Each Item In Rest(t.sSeq)
                                If Item.sSeq = t.sSeq Then
                                    Item.Count += 1
                                    Founded = True
                                    Exit For
                                End If
                            Next
                            If Founded = False Then Rest(t.sSeq).Add(t)
                        Next
                        Dim Out As New List(Of Bio.Sequence)
                        For Each Item In Rest
                            For Each SeqwCount In Item.Value
                                If SeqwCount.Count >= Me.MinNofReads Then Out.Add(SeqwCount.Seq)
                            Next
                        Next
                        Szunyi.IO.Export.SequncesToFasta(Out, New FileInfo(File.FullName & "Filtered.fa"))
                    End If

                Next
            End Sub
        End Class

        Public Class SeqsKmerswCounts
            Public Property Files As List(Of FileInfo)
            Public Property Length As Integer
            Public Property OnlyStarts As Boolean
            Public Property MaxNofInvestigatedSequences As Long
            Public Property OutPutType As Integer
            Public Property NofExportedItem As Integer

            Public Sub New(Files As List(Of FileInfo),
                           Length As Integer,
                           OnlyStarts As Boolean,
                           MaxNofInvestigatedSequences As Long,
                           OutPutType As Integer,
                           NofExportedItem As Integer)
                Me.Files = Files
                Me.Length = Length
                Me.OnlyStarts = OnlyStarts
                Me.MaxNofInvestigatedSequences = MaxNofInvestigatedSequences
                Me.OutPutType = OutPutType
                Me.NofExportedItem = NofExportedItem
            End Sub
            Public Sub DoIt()
                For Each File In Files
                    Dim res As New Dictionary(Of String, List(Of SeqwCount))
                    Dim cCount As Long = 0
                    Try
                        Dim x As New FileStream(File.FullName, FileMode.Open)
                        Dim fa = Bio.IO.SequenceParsers.FindParserByFileName(File.FullName)
                        For Each Seq In fa.Parse(x)
                            Dim t = New SeqwCount(Seq, Me.Length)
                            If res.ContainsKey(t.sSeq) = False Then res.Add(t.sSeq, New List(Of SeqwCount))
                            Dim Has As Boolean = False
                            For Each UsedItems In res(t.sSeq)
                                If UsedItems.sSeq = t.sSeq Then
                                    UsedItems.Count += 1
                                    Has = True
                                End If
                            Next
                            If Has = False Then res(t.sSeq).Add(t)
                            cCount += 1
                            If cCount = Me.MaxNofInvestigatedSequences Then
                                Export(res, File)
                                Exit For
                            End If
                        Next
                    Catch ex As Exception
                        Dim alf As Int16 = 54
                    End Try
                Next
            End Sub
            Private Sub Export(res As Dictionary(Of String, List(Of SeqwCount)), File As FileInfo)

                If Me.OutPutType = MyConstants.OutPutType.AsTabFile Then
                    Dim Count As Integer = 0
                    Dim str As New System.Text.StringBuilder
                    str.Append("Seq").Append(vbTab).Append("PPM").AppendLine()
                    Dim t As New List(Of SeqwCount)
                    For Each Item In res
                        For Each subitem In Item.Value
                            t.Add(subitem)
                        Next
                    Next
                    Dim t2 = (From x In t Order By x.Count Descending).ToList

                    For i1 = 0 To 5000
                        Dim SUbitem = t2(i1)
                        str.Append(SUbitem.sSeq).Append(vbTab).Append(SUbitem.Count / Me.MaxNofInvestigatedSequences * 1000000).AppendLine()
                    Next
                    Dim NewFIle = Szunyi.IO.Files.GetNewFile(File, ".tab")
                    Szunyi.IO.Export.SaveText(str.ToString, NewFIle)
                End If

            End Sub

        End Class

        Public Class SeqwCount
            Public Property Seq As Bio.ISequence
            Public Property Count As Integer = 1
            Public Property sSeq As String
            Public Sub New(Seq As Bio.Sequence, Optional Length As Integer = 0)
                Me.sSeq = Seq.ConvertToString(0, Seq.Count)
                Me.Seq = Seq

            End Sub
            Public Sub New(Seq As Bio.QualitativeSequence, Optional Length As Integer = 0)
                Dim str As New System.Text.StringBuilder
                For i1 = 0 To Seq.Count - 1
                    str.Append(Seq(i1))
                Next
                'Me.sSeq = str.ToString.ToUpper
            End Sub
            Public Sub New(Seq As Bio.ISequence, Optional Length As Integer = 0)
                Dim str As New System.Text.StringBuilder
                For i1 = 0 To Length - 1
                    str.Append(Chr(Seq(i1)))
                Next
                Me.sSeq = str.ToString.ToUpper
            End Sub
        End Class

        Public Class SequenceManinpulation
            Public SeqIDComparer As New Szunyi.Comparares.SequenceIDComparer
            Public ExtFeatureLocusTagComparer As New Szunyi.Comparares.ExtFeatureLocusTagComparer
            Public Sub RenameIDsByAscending(ByRef Seqs As List(Of Bio.Sequence))
                For i1 = 0 To Seqs.Count - 1
                    Seqs(i1).ID = i1
                Next
            End Sub
            Shared Function GetSeqAsString(Seq As Bio.Sequence, Optional Length As Integer = -1) As String
                If IsNothing(Seq) = True Then Return String.Empty
                If Length < 0 Then
                    If Seq.Count > 0 Then
                        Return Seq.ConvertToString(0, Seq.Count)
                    Else
                        Return String.Empty
                    End If
                Else
                    If Length < Seq.Count Then
                        Return Seq.ConvertToString(0, Length)
                    Else
                        Return Seq.ConvertToString(0, Seq.Count)
                    End If
                End If

            End Function
            Shared Function MergeSequences(PromoterSeq As Sequence, CDSSeq As Sequence, UTR3Seq As Sequence, BasicCDS As FeatureItem) As Bio.Sequence
                Dim s As String = GetSeqAsString(PromoterSeq) &
             GetSeqAsString(CDSSeq) &
                GetSeqAsString(UTR3Seq)
                Dim NSeq As New Sequence(Alphabets.AmbiguousDNA, s)
                Dim Md As New GenBankMetadata
                Md.Features = New Bio.IO.GenBank.SequenceFeatures
                Md.Locus = New GenBankLocusInfo
                Md.Locus.Name = BasicCDS.Qualifiers(StandardQualifierNames.LocusTag).First
                If BasicCDS.Qualifiers.ContainsKey(StandardQualifierNames.Product) = True Then
                    NSeq.ID = Md.Locus.Name & " " & BasicCDS.Qualifiers(StandardQualifierNames.Product).First.Replace(Chr(34), "")
                Else
                    NSeq.ID = Md.Locus.Name
                End If
                Dim PromoterSeqCount As Integer = 0

                If IsNothing(PromoterSeq) = False AndAlso PromoterSeq.Count > 0 Then
                    Dim Promoter As New Promoter("1.." & PromoterSeq.Count)
                    Md.Features.All.Add(Promoter)
                    PromoterSeqCount = PromoterSeq.Count
                End If
                If CDSSeq.Count > 0 Then
                    Dim CDS As New CodingSequence(PromoterSeqCount + 1 & ".." & PromoterSeqCount + CDSSeq.Count)
                    MergeQualifiers(BasicCDS, CDS)

                    Md.Features.All.Add(CDS)
                End If
                If IsNothing(UTR3Seq) = False AndAlso UTR3Seq.Count > 0 Then
                    Dim UTR As New ThreePrimeUtr(NSeq.Count - UTR3Seq.Count + 1 & ".." & NSeq.Count)
                    Md.Features.All.Add(UTR)

                End If
                NSeq.Metadata.Add(Bio.Util.Helper.GenBankMetadataKey, Md)

                Return NSeq
            End Function
            Private Shared Sub MergeQualifiers(ByRef BasicCDS As FeatureItem, ByRef CDS As CodingSequence)
                CDS.Qualifiers.Clear()
                For Each QulifierName In StandardQualifierNames.All
                    If BasicCDS.Qualifiers.ContainsKey(QulifierName) Then
                        Dim NList As List(Of String) = BasicCDS.Qualifiers(QulifierName).ToList
                        CDS.Qualifiers.Add(QulifierName, NList)
                    End If

                Next
            End Sub

            Public Function GetSequences(featureList As ListOf.ExtFeatureList, Qualifiers As List(Of String), FivePrimes As List(Of Integer), ThreePrimes As List(Of Integer)) As IEnumerable(Of Sequence)
                Dim Out As New List(Of Bio.Sequence)
                For Each ExtFeature In featureList.Features
                    Dim Seq = GetSeqByID(ExtFeature.SeqID, featureList.Seqs)
                    If IsNothing(FivePrimes) = False AndAlso IsNothing(ThreePrimes) = False Then
                        For Each FivePrime In FivePrimes
                            For Each ThreePrime In ThreePrimes
                                Out.Add(GetSequence(ExtFeature, Seq, FivePrime, ThreePrime))
                                Out.Last.ID = ExtFeature.GetQuilifierValues(Qualifiers, FivePrime, ThreePrime)
                            Next
                        Next
                    ElseIf IsNothing(FivePrimes) = False And IsNothing(ThreePrimes) = True
                        For Each FivePrime In FivePrimes
                            Out.Add(GetSequence(ExtFeature, Seq, FivePrime, Nothing))
                            Out.Last.ID = ExtFeature.GetQuilifierValues(Qualifiers, FivePrime, Nothing)
                        Next
                    ElseIf IsNothing(FivePrimes) = True And IsNothing(ThreePrimes) = False
                        For Each ThreePrime In ThreePrimes
                            Out.Add(GetSequence(ExtFeature, Seq, Nothing, ThreePrime))
                            Out.Last.ID = ExtFeature.GetQuilifierValues(Qualifiers, Nothing, ThreePrime)
                        Next
                    Else
                        Out.Add(GetSequence(ExtFeature, Seq, Nothing, Nothing))
                        Out.Last.ID = ExtFeature.GetQuilifierValues(Qualifiers, Nothing, Nothing)
                    End If

                Next
                Return Out
            End Function
            Public Function GetSequence(ExtFeature As ExtFeature, Seq As Bio.Sequence, FivePrime As Integer, ThreePrime As Integer) As Bio.Sequence
                Dim FivePrimeSeq As Bio.Sequence
                Dim ThreePrimeSeq As Bio.Sequence
                Dim BasicSeq As Bio.Sequence
                Try

                    If FivePrime <> 0 Then
                        If ExtFeature.Feature.Location.Operator = LocationOperator.Complement Then
                            Dim tmpPromoterSeq As Bio.Sequence = GetSequencesFromPosition(Seq, ExtFeature.Feature.Location.LocationEnd,
                                                                                        FivePrime)
                            FivePrimeSeq = tmpPromoterSeq.GetReverseComplementedSequence
                        Else
                            FivePrimeSeq = GetSequencesFromPosition(Seq, ExtFeature.Feature.Location.LocationStart -
                                                                            FivePrime - 1, FivePrime)
                        End If
                    End If

                    If ThreePrime <> 0 Then
                        If ExtFeature.Feature.Location.Operator = LocationOperator.Complement Then

                            Dim tmpUTRSeq = GetSequencesFromPosition(Seq, ExtFeature.Feature.Location.LocationStart - 1 - ThreePrime,
                                                                                   ThreePrime)
                            ThreePrimeSeq = tmpUTRSeq.GetReverseComplementedSequence

                        Else
                            ThreePrimeSeq = GetSequencesFromPosition(Seq, ExtFeature.Feature.Location.LocationStart -
                                                                                ThreePrime - 1, ThreePrime)
                        End If
                    End If
                Catch ex As Exception
                    Dim alf As Integer = 32
                End Try
                BasicSeq = ExtFeature.Feature.GetSubSequence(Seq)


                If ExtFeature.Feature.Location.Operator = LocationOperator.Complement Then
                    BasicSeq = BasicSeq.GetReverseComplementedSequence

                End If
                Dim t = MergeSequences(FivePrimeSeq, BasicSeq, ThreePrimeSeq, ExtFeature.Feature)
                Return t
            End Function
            Public Function GetSequenceByLocusTag(ExtFeatureList As List(Of Szunyi.ListOf.ExtFeatureList), LocusTag As String, FivePrime As Integer, ThreePrime As Integer) As Bio.Sequence
                Dim ExtFeature As ExtFeature = GetExtFeutureByLocusTag(ExtFeatureList, LocusTag)
                Dim Seq = GetSeqByID(ExtFeature.SeqID, ExtFeatureList)
                If IsNothing(ExtFeature) = True Then Return Nothing

                Dim FivePrimeSeq As Bio.Sequence
                Dim ThreePrimeSeq As Bio.Sequence
                Dim BasicSeq As Bio.Sequence
                Try

                    If FivePrime <> 0 Then
                        If ExtFeature.Feature.Location.Operator = LocationOperator.Complement Then
                            Dim tmpPromoterSeq As Bio.Sequence = GetSequencesFromPosition(Seq, ExtFeature.Feature.Location.LocationEnd,
                                                                                        FivePrime)
                            FivePrimeSeq = tmpPromoterSeq.GetReverseComplementedSequence
                        Else
                            FivePrimeSeq = GetSequencesFromPosition(Seq, ExtFeature.Feature.Location.LocationStart -
                                                                            FivePrime - 1, FivePrime)
                        End If
                    End If

                    If ThreePrime <> 0 Then
                        If ExtFeature.Feature.Location.Operator = LocationOperator.Complement Then

                            Dim tmpUTRSeq = GetSequencesFromPosition(Seq, ExtFeature.Feature.Location.LocationStart - 1 - ThreePrime,
                                                                                   ThreePrime)
                            ThreePrimeSeq = tmpUTRSeq.GetReverseComplementedSequence

                        Else
                            ThreePrimeSeq = GetSequencesFromPosition(Seq, ExtFeature.Feature.Location.LocationStart -
                                                                                ThreePrime - 1, ThreePrime)
                        End If
                    End If
                Catch ex As Exception
                    Dim alf As Integer = 32
                End Try
                BasicSeq = GetSequencesFromPosition(Seq, ExtFeature.Feature.Location.LocationStart, ExtFeature.Feature.Location.LocationEnd - ExtFeature.Feature.Location.LocationStart)


                If ExtFeature.Feature.Location.Operator = LocationOperator.Complement Then
                    BasicSeq = BasicSeq.GetReverseComplementedSequence

                End If
                Dim t = MergeSequences(FivePrimeSeq, BasicSeq, ThreePrimeSeq, ExtFeature.Feature)
                Return t
            End Function

            Private Function GetExtFeutureByLocusTag(extFeatureLists As List(Of ListOf.ExtFeatureList), locusTag As String) As ExtFeature
                locusTag = Chr(34) & locusTag & Chr(34)
                For Each extFeatureList In extFeatureLists
                    Dim t = extFeatureList.FetauresByLocustag.BinarySearch(New ExtFeature(locusTag), ExtFeatureLocusTagComparer)
                    If t > -1 Then
                        Return extFeatureList.FetauresByLocustag(t)
                    End If
                Next
                locusTag = locusTag.Replace(Chr(34), "")
                For Each extFeatureList In extFeatureLists
                    Dim t = extFeatureList.FetauresByLocustag.BinarySearch(New ExtFeature(locusTag), ExtFeatureLocusTagComparer)
                    If t > -1 Then
                        Return extFeatureList.FetauresByLocustag(t)
                    End If
                Next
                Return Nothing
            End Function

            Public Shared Function FromFeature(Seq As Bio.Sequence, Feat As FeatureItem)
                Dim TheNASeq As Bio.Sequence = Feat.GetSubSequence(Seq)

                If Feat.Location.Operator = LocationOperator.Complement Then
                    Return TheNASeq.GetReversedSequence
                Else
                    Return TheNASeq
                End If
            End Function
            Private Function GetSequencesFromPosition(seq As Sequence, Start As Integer, length As Integer) As Sequence
                Try
                    If length = 0 Then
                        Return New Bio.Sequence(Alphabets.AmbiguousDNA, "")
                    Else
                        If Start < 0 Then
                            length = length + Start
                            Start = 0
                        End If
                        If Start + length > seq.Count Then
                            length = seq.Count - Start
                        End If
                        Dim out = seq.GetSubSequence(Start, length)
                        Return out
                    End If
                Catch ex As Exception
                    Dim alf As Integer = 32
                End Try


            End Function

            Private Function GetSeqByID(SeqID As String, Seqs As List(Of Bio.Sequence)) As Bio.Sequence
                Dim Seq As New Bio.Sequence(Alphabets.AmbiguousDNA, "A")
                Seq.ID = SeqID
                Dim Index = Seqs.BinarySearch(Seq, SeqIDComparer)

                Return Seqs(Index)
            End Function
            Private Function GetSeqByID(SeqID As String, Seqs As List(Of Szunyi.ListOf.ExtFeatureList)) As Bio.Sequence
                Dim Seq As New Bio.Sequence(Alphabets.AmbiguousDNA, "A")
                Seq.ID = SeqID
                For Each Item In Seqs
                    Dim Index = Item.Seqs.BinarySearch(Seq, SeqIDComparer)
                    If Index > -1 Then Return Item.Seqs(Index)
                Next
                Return Nothing
            End Function

            Friend Shared Function MergeSequenceList(SeqLists As List(Of SequenceList)) As List(Of Sequence)
                Dim Out As New List(Of Sequence)
                For Each SeqList In SeqLists
                    Out.AddRange(SeqList.Sequences)
                Next
                Return Out
            End Function
        End Class
    End Namespace
End Namespace


