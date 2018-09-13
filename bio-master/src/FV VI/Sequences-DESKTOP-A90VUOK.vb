Imports System.IO
Imports Bio
Imports Bio.IO.GenBank
Imports FV_VI.Szunyi.ListOf


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
                        Szunyi.IO.Export.SaveSequencesToSingleFasta(Out, New FileInfo(File.FullName & "Filtered.fa"))
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
            Public Property BasicSeqs As List(Of Bio.Sequence)
            Public Property ModifiedLengthSeqs As List(Of Bio.Sequence)
            Public Property Result As New Dictionary(Of String, List(Of SeqwCount))
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
            Public Sub New(Seqs As List(Of Bio.Sequence),
                           Length As Integer,
                           OnlyStarts As Boolean,
                           MaxNofInvestigatedSequences As Long,
                           OutPutType As Integer,
                           NofExportedItem As Integer)
                Me.BasicSeqs = Seqs
                Me.Length = Length
                Me.OnlyStarts = OnlyStarts
                Me.MaxNofInvestigatedSequences = MaxNofInvestigatedSequences
                Me.OutPutType = OutPutType
                Me.NofExportedItem = NofExportedItem



            End Sub
            Public Sub DoIt()
                '  Me.ModifiedLengthSeqs = GetModifiedLengthSeqs(Me.BasicSeqs, Me.Length, Me.OnlyStarts)

                For Each Seq In Me.BasicSeqs
                    Dim t = New SeqwCount(Seq, Me.Length, Me.OnlyStarts)
                    If Me.Result.ContainsKey(t.sSeq) = False Then Me.Result.Add(t.sSeq, New List(Of SeqwCount))
                    Dim Has As Boolean = False
                    For Each UsedItems In Me.Result(t.sSeq)
                        If UsedItems.sSeq = t.sSeq Then
                            UsedItems.Count += 1
                            Has = True
                        End If
                    Next
                    If Has = False Then Me.Result(t.sSeq).Add(t)
                Next

            End Sub

            Private Function GetModifiedLengthSeqs(BasicSeqs As List(Of Bio.Sequence), length As Integer, onlyStarts As Boolean) As List(Of Sequence)
                Throw New NotImplementedException()
            End Function
            Public Function GetSequneces() As List(Of Bio.Sequence)
                Dim out As New List(Of Bio.Sequence)
                For Each Item In Result
                    For Each Seq In Item.Value
                        out.Add(Seq.Seq)
                    Next
                Next
                Return out
            End Function
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
            Public Sub New(Seq As Bio.Sequence, Optional Length As Integer = 0, Optional OnlyStart As Boolean = False)
                If Length > Seq.Count Then
                    ' It is false no valid result
                Else
                    Me.sSeq = Seq.ConvertToString(0, Seq.Count)
                    Me.Seq = Seq
                End If
            End Sub
            Public Sub New(Seq As Bio.QualitativeSequence, Optional Length As Integer = 0, Optional OnlyStart As Boolean = False)
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
            Public LociBuilder As New Bio.IO.GenBank.LocationBuilder
            Public Sub RenameIDsByAscending(ByRef Seqs As List(Of Bio.Sequence))
                For i1 = 0 To Seqs.Count - 1
                    Seqs(i1).ID = i1
                Next
            End Sub
            Shared Function GetUniqueSeqs(Seqs As List(Of Bio.Sequence)) As List(Of Bio.Sequence)
                Dim Rest As New Dictionary(Of String, List(Of SeqwCount))
                For Each Seq In Seqs
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
                Dim out As New List(Of Bio.Sequence)
                For Each Item In Rest
                    For Each Seq In Item.Value
                        out.Add(Seq.Seq)
                    Next
                Next
                Return out
            End Function
            ''' <summary>
            ''' Return The UpperCase String Represantion Of Sequence
            ''' If length is empthy the Full, else the partion of The First Part
            ''' </summary>
            ''' <param name="Seq"></param>
            ''' <param name="Length"></param>
            ''' <returns></returns>
            Shared Function GetSeqAsString(Seq As Bio.Sequence, Optional Length As Integer = -1) As String
                If IsNothing(Seq) = True Then Return String.Empty
                If Length < 0 Then
                    If Seq.Count > 0 Then
                        Return Seq.ConvertToString(0, Seq.Count).ToUpper
                    Else
                        Return String.Empty
                    End If
                Else
                    If Length < Seq.Count Then
                        Return Seq.ConvertToString(0, Length).ToUpper
                    Else
                        Return Seq.ConvertToString(0, Seq.Count).ToUpper
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
                        Out.Last.ID = ExtFeature.GetQuilifierValues(Qualifiers, Nothing, Nothing).Replace(Chr(34), "").Replace(vbCrLf, " ")
                    End If

                Next
                Return Out
            End Function
            Public Function GetSequence(ExtFeature As ExtFeature, Seq As Bio.Sequence, FivePrime As Integer, ThreePrime As Integer) As Bio.Sequence
                Dim FivePrimeSeq As New Bio.Sequence(Alphabets.AmbiguousDNA, "")
                Dim ThreePrimeSeq As New Bio.Sequence(Alphabets.AmbiguousDNA, "")
                Dim BasicSeq As Bio.Sequence
                If IsNothing(Seq) = True Then Return Nothing
                Try
                    If FivePrime <> 0 Then
                        If ExtFeature.Feature.Location.Operator = LocationOperator.Complement Then
                            Dim tmpPromoterSeq As Bio.Sequence = SeqFromStartAndLength(Seq, ExtFeature.Feature.Location.LocationEnd,
                                                                                        FivePrime)
                            FivePrimeSeq = tmpPromoterSeq.GetReverseComplementedSequence
                        Else
                            FivePrimeSeq = SeqFromStartAndLength(Seq, ExtFeature.Feature.Location.LocationStart -
                                                                            FivePrime - 1, FivePrime)
                        End If
                    End If

                    If ThreePrime <> 0 Then
                        If ExtFeature.Feature.Location.Operator = LocationOperator.Complement Then

                            Dim tmpUTRSeq = SeqFromStartAndLength(Seq, ExtFeature.Feature.Location.LocationStart - 1 - ThreePrime,
                                                                                   ThreePrime)
                            ThreePrimeSeq = tmpUTRSeq.GetReverseComplementedSequence

                        Else
                            ThreePrimeSeq = SeqFromStartAndLength(Seq, ExtFeature.Feature.Location.LocationStart -
                                                                                ThreePrime - 1, ThreePrime)
                        End If
                    End If
                Catch ex As Exception
                    Dim alf As Integer = 32
                End Try
                BasicSeq = ExtFeature.Feature.GetSubSequence(Seq)


                If ExtFeature.Feature.Location.Operator = LocationOperator.Complement Then
                    BasicSeq = BasicSeq.GetReversedSequence
                    Dim h = BasicSeq.GetComplementedSequence
                End If
                Dim t = MergeSequences(FivePrimeSeq, BasicSeq, ThreePrimeSeq, ExtFeature.Feature)
                Return t
            End Function
            Public Function GetSequenceByLocusTag(ExtFeatureList As Szunyi.ListOf.ExtFeatureList, LocusTag As String, FivePrime As Integer, ThreePrime As Integer) As Bio.Sequence
                Dim ExtFeature As ExtFeature = GetExtFeutureByLocusTag(ExtFeatureList, LocusTag)
                Dim Seq = GetSeqByID(ExtFeature.SeqID, ExtFeatureList.Seqs)
                If IsNothing(ExtFeature) = True Then Return Nothing

                Dim FivePrimeSeq As New Bio.Sequence(Alphabets.AmbiguousDNA, "")
                Dim ThreePrimeSeq As New Bio.Sequence(Alphabets.AmbiguousDNA, "")
                Dim BasicSeq As Bio.Sequence
                Try

                    If FivePrime <> 0 Then
                        If ExtFeature.Feature.Location.Operator = LocationOperator.Complement Then
                            Dim tmpPromoterSeq As Bio.Sequence = SeqFromStartAndLength(Seq, ExtFeature.Feature.Location.LocationEnd,
                                                                                        FivePrime)
                            FivePrimeSeq = tmpPromoterSeq.GetReverseComplementedSequence
                        Else
                            FivePrimeSeq = SeqFromStartAndLength(Seq, ExtFeature.Feature.Location.LocationStart -
                                                                            FivePrime - 1, FivePrime)
                        End If
                    End If

                    If ThreePrime <> 0 Then
                        If ExtFeature.Feature.Location.Operator = LocationOperator.Complement Then

                            Dim tmpUTRSeq = SeqFromStartAndLength(Seq, ExtFeature.Feature.Location.LocationStart - 1 - ThreePrime,
                                                                                   ThreePrime)
                            ThreePrimeSeq = tmpUTRSeq.GetReverseComplementedSequence

                        Else
                            ThreePrimeSeq = SeqFromStartAndLength(Seq, ExtFeature.Feature.Location.LocationStart -
                                                                                ThreePrime - 1, ThreePrime)
                        End If
                    End If
                Catch ex As Exception
                    Dim alf As Integer = 32
                End Try
                BasicSeq = SeqFromStartAndEnd(Seq, ExtFeature.Feature.Location.LocationStart,
                                                     ExtFeature.Feature.Location.LocationEnd)


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
            Public Shared Function GetExtFeutureByLocusTag(extFeatureLists As ListOf.ExtFeatureList, locusTag As String) As ExtFeature

                Dim t = ExtFeatureList.FetauresByLocustag.BinarySearch(New ExtFeature(locusTag), Comparares.AllComparares.ByExtFeatureLocusTag)
                If t > -1 Then
                    Return ExtFeatureList.FetauresByLocustag(t)
                End If

                Return Nothing
            End Function
            Public Function FromFeature(Seq As Bio.Sequence, Feat As FeatureItem)
                Dim TheNASeq As Bio.Sequence = Feat.GetSubSequence(Seq)

                If Feat.Location.Operator = LocationOperator.Complement Then
                    Return TheNASeq.GetReversedSequence
                Else
                    Return TheNASeq
                End If
            End Function
            ''' <summary>
            ''' If on - strand then add the reverse complemented sequence 
            ''' </summary>
            ''' <param name="Seq"></param>
            ''' <param name="Feat"></param>
            ''' <returns></returns>
            Public Function FromFeatureAsString(Seq As Sequence, Feat As FeatureItem) As String
                Dim NewSeq As Bio.Sequence = FromFeature(Seq, Feat)
                Return NewSeq.ConvertToString(0, NewSeq.Count)
            End Function
#Region "GetSequenceByPosition"
            ''' <summary>
            ''' Main routine with checking, return Bio.Seq (or empty Bio.Seq)
            ''' 
            ''' </summary>
            ''' <param name="seq"></param>
            ''' <param name="Start"></param>
            ''' <param name="Length"></param>
            ''' <returns></returns>
            Public Function SeqFromStartAndLength(seq As Sequence, Start As Integer, Length As Integer) As Bio.Sequence
                If Length <= 0 Then
                    Return New Bio.Sequence(Alphabets.AmbiguousDNA, "")
                Else
                    If Start < 0 Then
                        Length = Length + Start
                        Start = 0
                    End If
                    If Start + Length > seq.Count Then
                        Length = seq.Count - Start
                    End If
                    If Start = seq.Count Then Return New Bio.Sequence(Alphabets.AmbiguousDNA, "")
                    Dim out As Bio.Sequence = seq.GetSubSequence(Start, Length)
                    Return out
                End If

            End Function
            Public Function SeqFromStartAndLengthAsString(seq As Sequence, Start As Integer, Length As Integer) As String
                Dim TheSeq = SeqFromStartAndLength(seq, Start, Length)
                Return TheSeq.ConvertToString(0, TheSeq.Count)
            End Function
            Public Function SeqFromStartAndLengthReverseComplemented(seq As Sequence, start As Integer, length As Integer) As Bio.Sequence
                Dim TheSeq = Me.SeqFromStartAndLength(seq, start, length)
                Return TheSeq.GetReverseComplementedSequence
            End Function
            Public Function SeqFromStartAndLengthAsStringReverseComplemented(seq As Sequence, start As Integer, length As Integer) As String
                Dim TheSeq As Bio.Sequence = SeqFromStartAndLengthReverseComplemented(seq, start, length)
                Return TheSeq.ConvertToString(0, TheSeq.Count)
            End Function

            Public Function SeqFromStartAndEnd(seq As Sequence, Start As Integer, Endy As Integer) As Sequence
                Return Me.SeqFromStartAndLength(seq, Start, Endy - Start)
            End Function
            Public Function SeqFromStartAndEndAsString(seq As Sequence, Start As Integer, Endy As Integer) As String
                Dim TheSeq = Me.SeqFromStartAndEnd(seq, Start, Endy)
                Return TheSeq.ConvertToString(0, TheSeq.Count)
            End Function
            Public Function SeqFromStartAndEndReverseComplemented(seq As Sequence, Start As Integer, Endy As Integer) As Bio.Sequence
                Dim TheSeq = Me.SeqFromStartAndLength(seq, Start, Endy - Start)
                Return TheSeq.GetReverseComplementedSequence
            End Function
            Public Function SeqFromStartAndEndAsStringReverseComplemented(seq As Sequence, Start As Integer, Endy As Integer) As String
                Dim TheSeq As Bio.Sequence = SeqFromStartAndEndReverseComplemented(seq, Start, Endy)
                Return TheSeq.ConvertToString(0, TheSeq.Count)
            End Function

#End Region
#Region "GetSeq"
#Region "ByID"
            ''' <summary>
            ''' Return the Sequence Or Nothing
            ''' </summary>
            ''' <param name="SeqID"></param>
            ''' <param name="Seqs"></param>
            ''' <returns></returns>
            Public Shared Function GetSeqByID(SeqID As String, Seqs As List(Of Bio.Sequence)) As Bio.Sequence
                Dim Seq As New Bio.Sequence(Alphabets.AmbiguousDNA, "A")
                Seq.ID = SeqID
                Dim Index = Seqs.BinarySearch(Seq, Szunyi.Comparares.AllComparares.BySeqID)
                If Index >= -1 Then Return Seqs(Index)
                Return Nothing
            End Function


#End Region
#End Region
#Region "GetGenBankMetadata"
            ''' <summary>
            ''' Return GenBankMetadata Or New GenBankMetadata
            ''' </summary>
            ''' <param name="seqID"></param>
            ''' <param name="SeqList"></param>
            ''' <returns></returns>
            Friend Function GetGenBankMetadataBySeqID(seqID As String, SeqList As SequenceList) As GenBankMetadata
                Dim Seq = GetSeqByID(seqID, SeqList.Sequences)
                If Seq.Metadata.ContainsKey(Bio.Util.Helper.GenBankMetadataKey) = False Then
                    Return New Bio.IO.GenBank.GenBankMetadata
                Else
                    Return Seq.Metadata(Bio.Util.Helper.GenBankMetadataKey)
                End If
            End Function
#End Region
            ''' <summary>
            ''' Return the List of Sorted Sequences
            ''' </summary>
            ''' <param name="SeqLists"></param>
            ''' <returns></returns>
            Friend Shared Function MergeSequenceList(SeqLists As List(Of SequenceList)) As List(Of Bio.Sequence)
                Dim Out As New List(Of Bio.Sequence)
                If IsNothing(SeqLists) = True Then Return Nothing
                For Each SeqList In SeqLists
                    Out.AddRange(SeqList.Sequences)
                Next
                Out.Sort(Comparares.AllComparares.BySeqIDAndLength)
                Return Out
            End Function

            Friend Shared Function CreateGenBankFrom_mRNA_And_CDS(seqs As List(Of Sequence)) As List(Of Sequence)
                Dim out As New List(Of Bio.Sequence)
                If IsNothing(seqs) = True Then Return Nothing
                For i1 = 0 To seqs.Count - 1
                    Dim Md As GenBankMetadata = CreateGenBankMetaData(seqs(i1))
                    If seqs(i1).Metadata.ContainsKey(Bio.Util.Helper.GenBankMetadataKey) = False Then
                        seqs(i1).Metadata.Add(Bio.Util.Helper.GenBankMetadataKey, Md)
                    Else
                        seqs(i1).Metadata(Bio.Util.Helper.GenBankMetadataKey) = Md
                    End If
                    Md = seqs(i1).Metadata(Bio.Util.Helper.GenBankMetadataKey)

                    For i2 = i1 + 1 To seqs.Count - 1
                        Dim LocationString As String = "1.." & seqs(i1).Count
                        Dim LocBuilder As New LocationBuilder
                        Dim Loci = LocBuilder.GetLocation(LocationString)
                        Dim gene As New Bio.IO.GenBank.Gene(Loci)
                        gene.LocusTag.Add(seqs(i1).ID)
                        If IsNothing(Md.Features) = True Then Md.Features = New Bio.IO.GenBank.SequenceFeatures
                        Md.Features.All.Add(gene)
                        Dim oriseq = Szunyi.Sequences.SequenceManinpulation.GetSeqAsString(seqs(i1))
                        If seqs(i1).ID = seqs(i2).ID Then

                            Dim CDSSeq = Szunyi.Sequences.SequenceManinpulation.GetSeqAsString(seqs(i2))
                        Dim FirstPosition = InStr(oriseq, CDSSeq) + 1
                                LocationString = FirstPosition & ".." & FirstPosition + CDSSeq.Length - 2
                                Dim CDS As New Bio.IO.GenBank.CodingSequence(LocationString)
                                Dim mRNA As New Bio.IO.GenBank.MessengerRna(LocationString)
                                CDS.LocusTag.Add(seqs(i1).ID)
                                mRNA.LocusTag.Add(seqs(i1).ID)
                                Md.Features.All.Add(mRNA)
                                Md.Features.All.Add(CDS)
                            Else
                                If Md.Features.All.Count > 1 Then out.Add(seqs(i1))
                            i1 = i2 - 1
                            Exit For
                        End If
                    Next
                Next
                Szunyi.IO.Export.SaveSequencesToSingleGenBank(out)
            End Function

            Private Shared Function CreateGenBankMetaData(Seq As Sequence) As GenBankMetadata
                Dim x As New Bio.IO.GenBank.GenBankMetadata
                x.Locus = New Bio.IO.GenBank.GenBankLocusInfo
                x.Locus.Date = Now
                x.Locus.MoleculeType = MoleculeType.DNA
                x.Locus.Name = Seq.ID
                x.Locus.SequenceLength = Seq.Count
                x.Locus.StrandTopology = SequenceStrandTopology.Linear
                x.Accession = New GenBankAccession()
                x.Accession.Primary = Seq.ID
                x.Source = New Bio.IO.GenBank.SequenceSource
                x.Source.CommonName = "Medicago truncatula (barrel medic)"
                x.Source.Organism = New Bio.IO.GenBank.OrganismInfo
                x.Source.Organism.Species = "Medicago truncatula"
                x.Source.Organism.Genus = "Eukaryota; Viridiplantae; Streptophyta; Embryophyta; Tracheophyta;Spermatophyta; Magnoliophyta; eudicotyledons; Gunneridae;" &
            "Pentapetalae; rosids; fabids; Fabales; Fabaceae; Papilionoideae;Trifolieae; Medicago."
                Return x
            End Function
        End Class


    End Namespace
End Namespace


