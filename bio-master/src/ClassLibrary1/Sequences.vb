Imports System.IO
Imports System.Text
Imports Bio
Imports Bio.IO.GenBank
Imports ClassLibrary1.Szunyi.ListOf
Imports ClassLibrary1.Szunyi
Imports ClassLibrary1.Szunyi.Comparares
Imports ClassLibrary1.Szunyi.Sequences.SequenceManipulation.CheckAndRepair


Imports ClassLibrary1.Szunyi.Sequences.SequenceManipulation
Imports ClassLibrary1.Szunyi.Features.FeatureManipulation
Imports ClassLibrary1.Szunyi.Blat
Imports ClassLibrary1.Szunyi.Constants
Imports ClassLibrary1.Szunyi.Location

Namespace Szunyi
    Namespace Sequences
        Namespace SequenceManipulation


            Public Class ID
                ''' <summary>
                ''' Return a new cloned list with renamed IDS
                ''' </summary>
                ''' <param name="Seqs"></param>
                ''' <param name="Type"></param>
                ''' <param name="PrefixOrLimiter"></param>
                ''' <returns></returns>
                Public Shared Function Rename(Seqs As List(Of Bio.ISequence),
                                              Type As Szunyi.Constants.StringRename,
                                              Optional PrefixOrLimiter As String = "") As List(Of Bio.ISequence)
                    Dim Out = Common.CloneSeqs(Seqs)
                    Select Case Type
                        Case Szunyi.Constants.StringRename.AscendingWithPrefix

                            For i1 = 0 To Out.Count - 1
                                Out(i1).ID = PrefixOrLimiter & i1
                            Next
                        Case Szunyi.Constants.StringRename.FirstAfterSplit
                            For i1 = 0 To Out.Count - 1
                                Out(i1).ID = Split(Out(i1).ID, PrefixOrLimiter).First
                            Next
                        Case Szunyi.Constants.StringRename.LastAfterSplit
                            For i1 = 0 To Out.Count - 1
                                Out(i1).ID = Split(Out(i1).ID, PrefixOrLimiter).Last
                            Next
                        Case Szunyi.Constants.StringRename.Not_Last_Part
                            For i1 = 0 To Out.Count - 1
                                Out(i1).ID = Szunyi.Text.General.Get_Not_Last_Part(Out(i1).ID, PrefixOrLimiter)
                            Next
                        Case Szunyi.Constants.StringRename.Nor_First_Part
                            For i1 = 0 To Out.Count - 1
                                Out(i1).ID = Szunyi.Text.General.Get_Not_First_Part(Out(i1).ID, PrefixOrLimiter)
                            Next
                        Case Szunyi.Constants.StringRename.Ascending_With_PostFix
                            For i1 = 0 To Out.Count - 1
                                Out(i1).ID = Out(i1).ID & PrefixOrLimiter & i1
                            Next
                        Case Szunyi.Constants.StringRename.Insert_Before
                            For i1 = 0 To Out.Count - 1
                                Out(i1).ID = PrefixOrLimiter & Out(i1).ID
                            Next
                        Case Else
                            Dim alf As Int16 = 43
                    End Select
                    Return Out
                End Function

                Public Shared Function RenameByGeneNote(Seqs As List(Of Bio.ISequence))
                    Dim NewSeqs = Szunyi.Sequences.SequenceManipulation.Common.CloneSeqs(Seqs)
                    For Each Seq In NewSeqs
                        Dim Feats = Szunyi.Features.FeatureManipulation.GetFeaturesByType.GetFeturesByTypeFromSeq(Seq, StandardFeatureKeys.Gene)
                        Seq.ID = Feats.First.Qualifiers(StandardQualifierNames.Note).First
                    Next
                    Return NewSeqs
                End Function

                Public Shared Function GetMaxNumberFromSeqs(Seqs As List(Of Sequence), Prefix As String, NofDigit As Integer) As Integer
                    Dim Motif As String = Prefix & "[0-9]{" & NofDigit & "}"
                    Dim MaxID As Integer = 0
                    For Each Seq In Seqs
                        For Each ID In Szunyi.Text.Regexp.GetDistinctStringsbyRegexp(Seq.ID, Motif)
                            Dim i As Integer = ID.Replace(Prefix, "")
                            If i > MaxID Then
                                MaxID = i
                            End If
                        Next

                    Next
                    Dim h = GetAllNCRNUmbersFromSeqs(Seqs, Prefix, NofDigit)
                    Return MaxID

                End Function

                Public Shared Function Get_NCBI_RefseqID(rName As String) As String
                    Dim r As New RegularExpressions.Regex("[A-Za-z]{2}[0-9]{6}")
                    Dim rs = r.Match(rName)
                    Return rs.Value
                End Function

                Public Shared Function GetAllNCRNUmbersFromSeqs(Seqs As List(Of Sequence), Prefix As String, NofDigit As Integer) As List(Of String)
                    Dim Motif As String = Prefix & "[0-9]{" & NofDigit & "}"
                    Dim Out As New List(Of String)
                    For Each Seq In Seqs
                        For Each ID In Szunyi.Text.Regexp.GetDistinctStringsbyRegexp(Seq.ID, Motif)
                            Out.Add(ID)
                        Next
                    Next

                    Return Out

                End Function

                Public Shared Sub InsertNumbersIntoSeqIDs(Seqs As List(Of Sequence),
                                                          Prefix As String, NofDigit As Integer,
                                                          Delimiter As String, WhereToInsert As Integer, IdNumber As Integer)
                    Dim Motif As String = Prefix & "[0-9]{" & NofDigit & "}"
                    For Each Seq In Seqs
                        If Szunyi.Text.Regexp.HasMotif(Seq.ID, Motif) = False Then
                            Dim s = Split(Seq.ID, Delimiter).ToList
                            s.Insert(WhereToInsert, Prefix & IdNumber.ToString("D" & NofDigit))
                            Seq.ID = Szunyi.Text.General.GetText(s, " ")
                            IdNumber += 1
                        End If
                    Next
                End Sub
                Public Shared Function Add_Number_At_The_End_Of_The_IDs(Seqs As List(Of Bio.ISequence)) As List(Of Bio.ISequence)

                    For i1 = 0 To Seqs.Count - 1
                        Seqs(i1).ID = Seqs(i1).ID & "_" & i1
                    Next

                    Return Seqs

                End Function

                Public Shared Function Remove_Number_At_The_End_Of_The_IDs(Seqs As List(Of Bio.ISequence)) As List(Of Bio.ISequence)
                    For Each Seq In Seqs
                        Seq = Remove_Number_At_The_End_Of_The_ID(Seq)
                    Next
                    Return Seqs
                End Function
                Public Shared Function Remove_Number_At_The_End_Of_The_ID(Seq As Sequence) As Sequence
                    Dim Index = Seq.ID.LastIndexOf("_")
                    If Index > -1 Then Seq.ID = Seq.ID.Substring(0, Index)
                    Return Seq
                End Function

                Public Shared Function Get_ID(qulifiers As List(Of String), onlyFirst As Boolean, with_Location As Boolean, with_FileName As Boolean, feat As ExtFeature) As String

                    Dim ID As String = ""
                    If with_FileName = True Then ID = feat.Seq.ID & vbTab

                    If with_Location = True Then ID = ID & feat.LocationString & vbTab

                    ID = ID & Szunyi.Features.FeatureManipulation.Qulifiers.Get_Values_From_Feature(feat.Feature, qulifiers, onlyFirst)

                    ID = ID.Replace(Chr(34), "").Replace(" ", "_").Replace("__", "_")

                    Return ID

                End Function
            End Class

            Public Class Merging
                ''' <summary>
                ''' Merge The Sequences Int o Single List
                ''' </summary>
                ''' <param name="a"></param>
                ''' <param name="b"></param>
                ''' <returns></returns>
                Public Shared Function MergeSequences(a As List(Of Bio.ISequence), b As List(Of Bio.ISequence)) As List(Of Bio.ISequence)
                    a.AddRange(b)
                    Return a.ToList
                End Function

                Shared Function MergeSequences(PromoterSeq As Sequence, CDSSeq As Sequence, UTR3Seq As Sequence, BasicCDS As FeatureItem) As Bio.ISequence
                    Dim s As String = SeqsToString.GetSeqAsString(PromoterSeq) &
                    SeqsToString.GetSeqAsString(CDSSeq) &
                    SeqsToString.GetSeqAsString(UTR3Seq)
                    Dim NSeq As New Sequence(Alphabets.AmbiguousDNA, s)
                    Dim Md As New GenBankMetadata
                    Md.Features = New Bio.IO.GenBank.SequenceFeatures
                    Md.Locus = New GenBankLocusInfo

                    If BasicCDS.Qualifiers.ContainsKey(StandardQualifierNames.LocusTag) Then
                        Md.Locus.Name = BasicCDS.Qualifiers(StandardQualifierNames.LocusTag).First
                    End If

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
                        Szunyi.Features.FeatureManipulation.MergeFeatures.MergeQualifiers(BasicCDS, CDS)

                        Md.Features.All.Add(CDS)
                    End If
                    If IsNothing(UTR3Seq) = False AndAlso UTR3Seq.Count > 0 Then
                        Dim UTR As New ThreePrimeUtr(NSeq.Count - UTR3Seq.Count + 1 & ".." & NSeq.Count)
                        Md.Features.All.Add(UTR)

                    End If
                    NSeq.Metadata.Add(Bio.Util.Helper.GenBankMetadataKey, Md)

                    Return NSeq
                End Function

                ''' <summary>
                ''' Return the List of Sorted Sequences 
                ''' </summary>
                ''' <param name="SeqLists"></param>
                ''' <returns></returns>
                Public Shared Function MergeSequenceList(SeqLists As List(Of SequenceList)) As List(Of Bio.ISequence)
                    Dim Out As New List(Of Bio.ISequence)
                    If IsNothing(SeqLists) = True Then Return Nothing
                    For Each SeqList In SeqLists
                        Out.AddRange(SeqList.Sequences)
                    Next
                    Out.Sort(Comparares.AllComparares.BySeqIDAndLength)
                    Return Out
                End Function

                Public Shared Function Seqs_With_Blat(Seqs As List(Of ISequence), BlatResult As List(Of BlatResult)) As List(Of Bio.ISequence)
                    Dim cloned_Seqs = Szunyi.Sequences.SequenceManipulation.Common.CloneSeqs(Seqs)
                    Dim Out As New List(Of Bio.ISequence)
                    Dim NotFound As New List(Of Bio.ISequence)
                    For Each Item In BlatResult
                        Dim TheSeq As Bio.ISequence = Szunyi.Sequences.SequenceManipulation.GetSequences.ByID(cloned_Seqs, Item.T_Name)
                        Dim Md = Szunyi.Features.FeatureManipulation.GenBankMetaDataManipulation.GetOrCreateGenBankMetaDataFromSeq(TheSeq)

                        If IsNothing(TheSeq) = False Then
                            Dim Notes As New List(Of String)
                            Notes.Add(Item.Q_Name)
                            For Each Query_Seq In Item.Q_Seq
                                Dim x As FeatureItem
                                Dim Pos = Szunyi.Sequences.SequenceManipulation.UniqueDistinct.SimpleBoyerMooreSearch(Query_Seq, TheSeq)
                                If Pos > -1 Then
                                    x = New Bio.IO.GenBank.NonCodingRna(Pos & ".." & Pos + Query_Seq.Count)
                                    x.Qualifiers(StandardQualifierNames.LocusTag) = Notes
                                    x.Qualifiers(StandardQualifierNames.Product) = Notes
                                    Md.Features.All.Add(x)
                                    Out.Add(TheSeq)
                                Else
                                    Pos = Szunyi.Sequences.SequenceManipulation.UniqueDistinct.SimpleBoyerMooreSearch_Reverse_Complement(Query_Seq, TheSeq)
                                    If Pos > -1 Then
                                        x = New Bio.IO.GenBank.NonCodingRna("complement(" & Pos & ".." & Pos + Query_Seq.Count & ")")
                                        x.Qualifiers(StandardQualifierNames.LocusTag) = Notes
                                        x.Qualifiers(StandardQualifierNames.Product) = Notes
                                        Md.Features.All.Add(x)
                                        Out.Add(TheSeq)
                                    Else
                                        NotFound.Add(TheSeq)
                                    End If

                                End If
                            Next
                        End If
                    Next
                    Dim out2 = Szunyi.Sequences.SequenceManipulation.UniqueDistinct.Get1CopyByID(Out)
                    Dim NotFoundII = Szunyi.Sequences.SequenceManipulation.UniqueDistinct.Get1CopyByID(NotFound)
                    Return out2

                End Function
            End Class

            Public Class Sort
                ''' <summary>
                ''' Return Sequence By Count Descending
                ''' </summary>
                ''' <param name="group"></param>
                ''' <returns></returns>
                Public Shared Function GetSequenceByLengthDescending(group As IEnumerable(Of Bio.ISequence)) As List(Of Bio.ISequence)
                    Return (From x In group.ToList Order By x.Count Descending).ToList

                End Function
                ''' <summary>
                ''' Return Sequence By Count Ascending
                ''' </summary>
                ''' <param name="group"></param>
                ''' <returns></returns>
                Public Shared Function GetSequenceByLengthAscending(group As IEnumerable(Of Bio.ISequence)) As List(Of Bio.ISequence)
                    Return (From x In group.ToList Order By x.Count Ascending).ToList
                End Function

            End Class

            Public Class SeqsToString
                ''' <summary>
                ''' Return The UpperCase String Represantion Of Sequences
                ''' If length is empthy the Full, else the partion of The First Part
                ''' </summary>
                ''' <param name="Seqs"></param>
                ''' <param name="Length"></param>
                ''' <returns></returns>			
                Public Shared Function GetSeqsAsString(seqs As List(Of ISequence), Optional length As Integer = -1) As List(Of String)
                    Dim out As New List(Of String)
                    For Each Seq In seqs
                        out.Add(GetSeqAsString(Seq, length))
                    Next
                    Return out
                End Function
                ''' <summary>
                ''' Return The UpperCase String Represantion Of Sequences
                ''' If length is empthy the Full, else the partion of The First Part
                ''' </summary>
                ''' <param name="Seqs"></param>
                ''' <param name="Length"></param>
                ''' <returns></returns>			
                Public Shared Function GetSeqsAsFasta(seqs As List(Of Sequence), Optional length As Integer = -1) As List(Of String)
                    Dim out As New List(Of String)
                    For Each Seq In seqs
                        out.Add(GetSeqAsFasta(Seq, length))
                    Next
                    Return out
                End Function
                ''' <summary>
                ''' Return The UpperCase String Represantion Of Sequences
                ''' If length is empthy the Full, else the partion of The First Part
                ''' </summary>
                ''' <param name="Seqs"></param>
                ''' <param name="Length"></param>
                ''' <returns></returns>			
                Public Shared Function GetSeqsAsFasta(seqs As List(Of ISequence), Optional length As Integer = -1) As List(Of String)
                    Dim out As New List(Of String)
                    For Each Seq In seqs
                        out.Add(GetSeqAsFasta(Seq, length))
                    Next
                    Return out
                End Function
                ''' <summary>
                ''' Return The UpperCase String Represantion Of Sequences
                ''' If length is empthy the Full, else the partion of The First Part
                ''' </summary>
                ''' <param name="Seq"></param>
                ''' <param name="Length"></param>
                ''' <returns></returns>			
                Public Shared Function GetSeqAsFasta(seq As Bio.ISequence, Optional length As Integer = -1) As String
                    Dim out As New List(Of String)
                    Dim s = ">" & seq.ID & vbCrLf & GetSeqAsString(seq, length) & vbCrLf
                    Return s
                End Function
                ''' <summary>
                ''' Return The UpperCase String Represantion Of Sequence
                ''' If length is empthy the Full, else the partion of The First Part
                ''' </summary>
                ''' <param name="Seq"></param>
                ''' <param name="Length"></param>
                ''' <returns></returns>
                Shared Function GetSeqAsString(Seq As Bio.ISequence, Optional Length As Integer = -1) As String
                    If IsNothing(Seq) = True Then Return String.Empty
                    If Length < 0 Then
                        If Seq.Count > 0 Then
                            Dim theSeq As Bio.Sequence = Seq
                            Return theSeq.ConvertToString
                        Else
                            Return String.Empty
                        End If
                    Else
                        If Length < Seq.Count Then
                            Return Szunyi.Sequences.SequenceManipulation.SeqsToString.GetSeqAsString(Seq, Length)
                        Else
                            Return Szunyi.Sequences.SequenceManipulation.SeqsToString.GetSeqAsString(Seq)
                        End If
                    End If

                End Function

                ''' <summary>
                ''' If on - strand then add the reverse complemented sequence 
                ''' </summary>
                ''' <param name="Seq"></param>
                ''' <param name="Feat"></param>
                ''' <returns></returns>
                Public Shared Function FromFeature(Seq As Sequence, Feat As FeatureItem) As Bio.ISequence
                    Dim NewSeq As Bio.ISequence = GetSequences.FromFeature(Seq, Feat)
                    Return NewSeq
                End Function
                ''' <summary>
                ''' If on - strand then add the reverse complemented sequence 
                ''' </summary>
                ''' <param name="Seq"></param>
                ''' <param name="Features"></param>
                ''' <returns></returns>
                Public Shared Function FromFeatures(Seq As Sequence, Features As List(Of FeatureItem)) As List(Of Bio.ISequence)
                    Dim out As New List(Of Bio.ISequence)
                    For Each Feature In Features
                        out.Add(FromFeature(Seq, Feature))
                    Next
                    Return out
                End Function
                Public Shared Function SeqFromStartAndLengthAsString(seq As Sequence, Start As Integer, Length As Integer) As String
                    Dim TheSeq = GetSequences.ByStartAndLength(seq, Start, Length)
                    Return Szunyi.Sequences.SequenceManipulation.SeqsToString.GetSeqAsString(TheSeq)
                End Function

                Public Shared Function SeqFromStartAndLengthAsStringReverseComplemented(seq As Sequence, start As Integer, length As Integer) As String
                    Dim TheSeq As Bio.ISequence = GetSequences.SeqFromStartAndLengthReverseComplemented(seq, start, length)
                    Return Szunyi.Sequences.SequenceManipulation.SeqsToString.GetSeqAsString(seq)
                End Function


                Public Shared Function SeqFromStartAndEndAsString(seq As Sequence, Start As Integer, Endy As Integer) As String
                    Dim TheSeq = GetSequences.ByStartAndLength(seq, Start, Endy - Start)
                    Return Szunyi.Sequences.SequenceManipulation.SeqsToString.GetSeqAsString(TheSeq)
                End Function

                Public Shared Function SeqFromStartAndEndAsStringReverseComplemented(seq As Sequence, Start As Integer, Endy As Integer) As String
                    Dim TheSeq As Bio.ISequence = GetSequences.SeqFromStartAndEndReverseComplemented(seq, Start, Endy)
                    Return Szunyi.Sequences.SequenceManipulation.SeqsToString.GetSeqAsString(seq)
                End Function

            End Class

            Public Class CDS
                Public Shared Property LociBuilder As New Bio.IO.GenBank.LocationBuilder
                Public Shared Function GetCDSFromNAAndAA(NASeq As Bio.ISequence, AASeq As Bio.ISequence) As Bio.ISequence
                    Dim AllTranslatedNaSeq = Szunyi.Translate.TranslateIntoSequenceAllFrames(NASeq)
                    Dim AAval = AASeq.ToArray
                    For i1 = 0 To 5
                        Dim StartIndex = Szunyi.Text.Regexp.SimpleBoyerMooreSearch(AllTranslatedNaSeq(i1).ToArray, AAval)
                        If StartIndex > -1 Then
                            If i1 < 3 Then ' Fw
                                Return NASeq.GetSubSequence(StartIndex * 3 + i1, AAval.Count * 3)
                                Exit For
                            Else ' rev COmnplement
                                Dim RevComplSeq = NASeq.GetReverseComplementedSequence
                                Return RevComplSeq.GetSubSequence(StartIndex * 3 + i1 - 3, AAval.Count * 3)
                                Exit For
                            End If
                        End If
                    Next
                    Return Nothing
                End Function
                Public Shared Function GetCDSFromNAAndAA(NASeqs As List(Of Bio.ISequence), AASeqs As List(Of Bio.ISequence))
                    NASeqs.Sort(Szunyi.Comparares.AllComparares.BySeqID)
                    Dim out As New List(Of Bio.ISequence)
                    Dim tmpSeq As New Bio.Sequence(Alphabets.AmbiguousDNA, "A")
                    For Each AASeq In AASeqs
                        tmpSeq.ID = AASeq.ID
                        Dim Index = NASeqs.BinarySearch(tmpSeq, Szunyi.Comparares.AllComparares.BySeqID)
                        If Index > -1 Then
                            Dim NewSeq = GetCDSFromNAAndAA(NASeqs(Index), AASeq)
                            If IsNothing(NewSeq) = False Then out.Add(NewSeq)
                        End If
                    Next
                    Return out
                End Function
                Public Shared Function GetCDSFromNAAndAA(NaSeq As Bio.ISequence, AASeq As Bio.ISequence, Md As GenBankMetadata) As Bio.ISequence
                    NaSeq.Metadata.Clear()
                    NaSeq.Metadata.Add(Bio.Util.Helper.GenBankMetadataKey, Md)
                    Dim AllTranslatedNaSeq = Szunyi.Translate.TranslateIntoSequenceAllFrames(NaSeq)
                    Dim AAval = AASeq.ToArray
                    For i1 = 0 To 5
                        Dim StartIndex = Szunyi.Text.Regexp.SimpleBoyerMooreSearch(AllTranslatedNaSeq(i1).ToArray, AAval)
                        Dim Loci As Bio.IO.GenBank.Location

                        If StartIndex > -1 Then
                            If i1 < 3 Then ' Fw
                                Dim Start = StartIndex * 3 + i1 + 1
                                Dim Endy = StartIndex * 3 + i1 + AAval.Count * 3
                                If Endy > NaSeq.Count Then Endy = NaSeq.Count

                                Loci = LociBuilder.GetLocation(Start & ".." & Endy)
                            Else ' rev COmnplement
                                Dim Start = StartIndex * 3 + i1 - 2
                                Dim Endy = Start + AAval.Count * 3 - 1
                                If Endy >= NaSeq.Count Then Endy = NaSeq.Count - 1

                                Loci = LociBuilder.GetLocation(Start & ".." & Endy)
                                NaSeq = NaSeq.GetReverseComplementedSequence

                            End If
                            Dim FullLoci = LociBuilder.GetLocation(1 & ".." & NaSeq.Count)
                            Dim t As New List(Of String)
                            t.Add(NaSeq.ID)
                            Szunyi.Features.FeatureManipulation.GenBankMetaDataManipulation.AddFeature(NaSeq,
                                                                                                       FullLoci,
                                                                                                       StandardFeatureKeys.Gene,
                                                                                                       t)

                            Szunyi.Features.FeatureManipulation.GenBankMetaDataManipulation.AddFeature(NaSeq,
                                                                                                       FullLoci,
                                                                                                       StandardFeatureKeys.MessengerRna,
                                                                                                       t)
                            Szunyi.Features.FeatureManipulation.GenBankMetaDataManipulation.AddFeature(NaSeq,
                                                                                                       Loci,
                                                                                                       StandardFeatureKeys.CodingSequence,
                                                                                                       t)


                            Return Szunyi.Sequences.SequenceManipulation.Common.CloneSeq(NaSeq)
                        End If
                    Next
                    Return Nothing
                End Function

            End Class

            Public Class Common
                Public Shared Function merge(First As Bio.ISequence, Last As Bio.Sequence) As Bio.ISequence
                    Dim s As String
                    Dim alp As Object
                    If IsNothing(First) = False Then
                        s = First.ConvertToString
                        alp = First.Alphabet
                    End If
                    If IsNothing(Last) = False Then
                        s = s & Last.ConvertToString
                        alp = Last.Alphabet
                    End If
                    Return New Bio.Sequence(alp, s)
                End Function
                Public Shared Function GetSeqAsBioSeq(OriSeq As Bio.ISequence) As Bio.Sequence
                    If OriSeq.GetType().Name = "QualitativeSequence" Then
                        Dim t1 As Bio.QualitativeSequence = OriSeq
                        Dim t2 As New Bio.Sequence(Alphabets.AmbiguousDNA, t1.GetSequence)
                        t2.ID = t1.ID

                        Return t2
                    Else
                        Return OriSeq
                    End If
                End Function
                Public Shared Function GetSeqsAsBioSeq(Seqs As List(Of Bio.ISequence)) As List(Of Bio.Sequence)
                    Dim out As New List(Of Bio.Sequence)
                    For Each seq In Seqs
                        out.Add(GetSeqAsBioSeq(seq))
                    Next
                    Return out
                End Function
                ''' <summary>
                ''' Return the Cloned Sequence or Nothing
                ''' </summary>
                ''' <param name="Seq"></param>
                ''' <returns></returns>
                Public Shared Function CloneSeq(Seq As Bio.ISequence) As Bio.ISequence
                    If IsNothing(Seq) = True Then Return Nothing
                    If Seq.GetType.Name = "QualitativeSequence" Then
                        Dim s As QualitativeSequence = Seq

                        Dim x As New Bio.QualitativeSequence(Seq.Alphabet, s.FormatType, Seq.ToArray, s.GetEncodedQualityScores)
                        x.ID = Seq.ID
                        Return x
                    Else
                        Dim x As New Bio.Sequence(Seq.Alphabet, Seq.ToArray)
                        x.ID = Seq.ID
                        If Seq.Metadata.ContainsKey(Bio.Util.Helper.GenBankMetadataKey) Then
                            Dim Md As Bio.IO.GenBank.GenBankMetadata = Seq.Metadata(Bio.Util.Helper.GenBankMetadataKey)
                            x.Metadata.Add(Bio.Util.Helper.GenBankMetadataKey, Md.Clone)
                        End If
                        Return x
                    End If


                End Function

                ''' <summary>
                ''' Return the Cloned Sequences, Nothings are ignored
                ''' </summary>
                ''' <param name="Seqs"></param>
                ''' <returns></returns>
                Public Shared Function CloneSeqs(Seqs As List(Of Bio.ISequence)) As List(Of Bio.ISequence)
                    Dim Out As New List(Of Bio.ISequence)
                    For Each Seq In Seqs
                        If IsNothing(Seq) = False Then
                            Out.Add(CloneSeq(Seq))
                        Else
                            Dim alf As Int16 = 65
                        End If

                    Next
                    Return Out
                End Function

                ''' <summary>
                ''' Return The Merged Name OF SequnceLists
                ''' </summary>
                ''' <param name="SeqLists"></param>
                ''' <returns></returns>
                Public Shared Function GetSeqListsName(SeqLists As List(Of SequenceList)) As List(Of String)
                    If IsNothing(SeqLists) = True Then Return New List(Of String)
                    Dim Out As New List(Of String)
                    Dim str As New StringBuilder
                    For Each SeqList In SeqLists
                        Out.Add(SeqList.ShortFileName)
                    Next
                    Return Out
                End Function

                ''' <summary>
                ''' Sorted List of String or Empty List
                ''' </summary>
                ''' <param name="Files"></param>
                ''' <returns></returns>
                Public Shared Function GetSeqIDs(Files As List(Of FileInfo)) As List(Of String)
                    Dim Seqs = Szunyi.IO.Import.Sequence.FromFiles(Files)
                    If Seqs.Count = 0 Then Return New List(Of String)
                    Dim SeqIDs = From x In Seqs Select x.ID
                    Return SeqIDs.ToList

                End Function
                ''' <summary>
                ''' Sorted List of String or Empty List
                ''' </summary>
                ''' <param name="Seqs"></param>
                ''' <returns></returns>
                Public Shared Function GetSeqIDs(Seqs As List(Of Bio.ISequence)) As List(Of String)

                    If Seqs.Count = 0 Then Return New List(Of String)
                    Dim SeqIDs = From x In Seqs Select x.ID
                    Return SeqIDs.ToList

                End Function
                ''' <summary>
                ''' Retrun SeqGroups by Default Nof Sequences
                ''' </summary>
                ''' <param name="seqs"></param>
                ''' <param name="nofSeqPerFile"></param>
                ''' <returns></returns>
                Public Shared Iterator Function GetSeqGroupsByCount(seqs As List(Of Bio.ISequence), nofSeqPerFile As Integer) As IEnumerable(Of List(Of Bio.ISequence))
                    Dim out As New List(Of Bio.ISequence)
                    For Each Seq In seqs
                        out.Add(Seq)
                        If out.Count Mod nofSeqPerFile = 0 Then
                            Yield Szunyi.Sequences.SequenceManipulation.Common.CloneSeqs(out)
                            out.Clear()
                        End If
                    Next
                    Yield Szunyi.Sequences.SequenceManipulation.Common.CloneSeqs(out)
                End Function
                Public Shared Iterator Function GetSeqGroupsByNof_Residues(seqs As List(Of Bio.ISequence), nof_Residues As Integer) As IEnumerable(Of List(Of Bio.ISequence))
                    Dim out As New List(Of Bio.ISequence)
                    Dim curr_Nof_Residues As Integer = 0
                    For Each Seq In seqs
                        If curr_Nof_Residues + Seq.Count > nof_Residues Then
                            Yield Szunyi.Sequences.SequenceManipulation.Common.CloneSeqs(out)
                            out.Clear()
                            curr_Nof_Residues = 0
                        Else
                            out.Add(Seq)
                            curr_Nof_Residues += Seq.Count
                        End If

                    Next
                    If curr_Nof_Residues > nof_Residues Then
                        Dim out2 As New List(Of Bio.ISequence)
                        out2.Add(Szunyi.Sequences.SequenceManipulation.Common.CloneSeqs(out).Last)
                        out.RemoveAt(out.Count - 1)
                        Yield out
                        Yield out2
                    Else
                        Yield Szunyi.Sequences.SequenceManipulation.Common.CloneSeqs(out)
                    End If

                End Function
                Public Shared Iterator Function GetSeqGroupsBy_FileLength(seqs As List(Of Bio.ISequence), nof_Residues As Integer) As IEnumerable(Of List(Of Bio.ISequence))
                    Dim out As New List(Of Bio.ISequence)
                    Dim curr_Nof_Residues As Integer = 0
                    For Each Seq In seqs
                        If curr_Nof_Residues + Seq.Count + Seq.ID.Count + 3 > nof_Residues Then
                            Yield Szunyi.Sequences.SequenceManipulation.Common.CloneSeqs(out)
                            out.Clear()
                            curr_Nof_Residues = 0
                        Else
                            out.Add(Seq)
                            curr_Nof_Residues += Seq.Count + Seq.ID.Count + 3
                        End If

                    Next
                    If curr_Nof_Residues > nof_Residues Then
                        Dim out2 As New List(Of Bio.ISequence)
                        out2.Add(Szunyi.Sequences.SequenceManipulation.Common.CloneSeqs(out).Last)
                        out.RemoveAt(out.Count - 1)
                        Yield out
                        Yield out2
                    Else
                        Yield Szunyi.Sequences.SequenceManipulation.Common.CloneSeqs(out)
                    End If
                End Function
                Friend Shared Function RemoveInsertionSymbols(Seq As Bio.ISequence) As Bio.ISequence
                    Dim b As Byte = AscW("-")
                    Dim s = From j As Byte In Seq.ToArray Where j <> b

                    Dim res As New Bio.Sequence(Seq.Alphabet, s.ToArray)
                    res.ID = Seq.ID
                    Return res

                End Function

                Public Shared Iterator Function GetSeqGroupsByAA_Pos(seqs As List(Of ISequence),
                                                                     positions As List(Of Integer)) As IEnumerable(Of KeyValuePair(Of String, List(Of Bio.ISequence)))
                    Dim tr As List(Of Bio.ISequence)
                    If seqs.First.Alphabet Is Bio.Alphabets.DNA Or seqs.First.Alphabet Is Bio.Alphabets.AmbiguousDNA Then
                        tr = Szunyi.Translate.TranaslateSeqs(seqs, True)

                    ElseIf seqs.First.Alphabet Is Bio.Alphabets.Protein Or seqs.First.Alphabet Is Bio.Alphabets.AmbiguousProtein Then
                        tr = seqs
                    Else
                        Yield Nothing

                    End If
                    Dim out As New Dictionary(Of String, List(Of Bio.ISequence))
                    For Each POs In positions
                        For Index = 0 To seqs.Count - 1
                            Dim b = POs & ChrW(tr(Index)(POs - 1))
                            If out.ContainsKey(b) = False Then out.Add(b, New List(Of Bio.ISequence))
                            out(b).Add(seqs(Index))
                        Next
                    Next
                    For Each Item In out
                        Yield (Item)
                    Next
                End Function
            End Class

            Public Class SelectBy
                Public Shared Function GetLongestByID(Seqs As List(Of Bio.ISequence)) As List(Of Bio.ISequence)
                    Dim out As New List(Of Bio.ISequence)

                    For Each g In UniqueDistinct.GetSeqGroupsBySeqID(Seqs)
                        If g.Count = 1 Then
                            out.Add(g.First)
                        Else
                            Dim OrderAASeqs = Sort.GetSequenceByLengthAscending(g)
                            out.Add(OrderAASeqs.Last) ' The Longer Shoul Always Ad To The List
                            For i1 = 0 To OrderAASeqs.Count - 2
                                Dim Found As Boolean = False
                                For i2 = i1 + 1 To OrderAASeqs.Count - 1
                                    Dim StartIndex = UniqueDistinct.SimpleBoyerMooreSearch(OrderAASeqs(i1), OrderAASeqs(i2))
                                    If StartIndex > -1 Then
                                        Found = True
                                    Else
                                        Dim alf As Int16 = 54
                                    End If
                                Next
                                If Found = False Then
                                    out.Add(OrderAASeqs(i1))
                                End If
                            Next
                        End If

                    Next
                    Return out

                End Function
                ''' <summary>
                ''' Retrun Empty List or All Of The ProteinSeqs Founded
                ''' Duplicated seqs may return multiple copies
                ''' </summary>
                ''' <param name="Seqs"></param>
                ''' <param name="Seq"></param>
                ''' <returns></returns>
                Public Shared Function GetSeqContains(Seqs As List(Of Bio.ISequence), Seq As Bio.ISequence) As List(Of Bio.ISequence)
                    Dim out As New List(Of Bio.ISequence)


                    For Each SubSeq In Seqs
                        Dim StartIndex = Szunyi.Text.Regexp.SimpleBoyerMooreSearch(SubSeq.ToArray, Seq.ToArray)

                        If StartIndex > -1 Then
                            out.Add(SubSeq)
                        End If
                    Next
                    Return out
                End Function
                ''' <summary>
                ''' Return Empty List of Seqs than shorter
                ''' </summary>
                ''' <param name="Seqs"></param>
                ''' <param name="MaxLenght"></param>
                ''' <returns></returns>
                Public Shared Function ShorterThan(Seqs As List(Of Bio.ISequence), MaxLenght As Integer) As List(Of Bio.ISequence)
                    If IsNothing(Seqs) = True Then Return New List(Of Bio.ISequence)
                    Dim x = From t In Seqs Where t.Count < MaxLenght

                    If x.Count > 0 Then
                        Return x.ToList
                    Else
                        Return New List(Of Bio.ISequence)
                    End If
                End Function
                ''' <summary>
                ''' Retrun Empty List or Seqs Longer Than
                ''' </summary>
                ''' <param name="Seqs"></param>
                ''' <param name="MinLength"></param>
                ''' <returns></returns>
                Public Shared Function LongerThan(Seqs As List(Of ISequence), MinLength As Integer) As List(Of Bio.ISequence)
                    If IsNothing(Seqs) = True Then Return New List(Of Bio.ISequence)
                    Dim x = From t In Seqs Where t.Count > MinLength

                    If x.Count > 0 Then
                        Return x.ToList
                    Else
                        Return New List(Of Bio.ISequence)
                    End If
                End Function
                ''' <summary>
                ''' Retrun Empty List or Seqs which length are Between in and Max
                ''' </summary>
                ''' <param name="Seqs"></param>
                ''' <param name="MaxLength"></param>
                ''' <param name="MinLength"></param>
                ''' <returns></returns>
                Public Shared Function LengthBetween(Seqs As List(Of Bio.ISequence), MaxLength As Integer, MinLength As Integer) As List(Of Bio.ISequence)
                    If IsNothing(Seqs) = True Then Return New List(Of Bio.ISequence)
                    Dim t = LongerThan(Seqs, MinLength)
                    Return ShorterThan(t, MaxLength)
                End Function

                Public Shared Function GetSeqContainsIDs(GBks As List(Of ISequence), Fastas As List(Of ISequence)) As List(Of Bio.ISequence)
                    Dim out As New List(Of Bio.ISequence)
                    For Each seq In Fastas
                        Dim x = From h In GBks Where h.ID.Contains(seq.ID.Split(" ").First)

                        If x.Count > 0 Then out.AddRange(x)
                    Next
                    out.Distinct
                    Return out
                End Function
            End Class

            Public Class AA
                Public Shared Function GetORFFromHitAsAA(BlastSearchRecords As List(Of Bio.Web.Blast.BlastSearchRecord), Seqs As List(Of Bio.ISequence)) As List(Of Bio.ISequence)
                    Dim tmpSeq As New Bio.Sequence(Alphabets.AmbiguousDNA, "A")
                    Dim Out As New List(Of Bio.ISequence)
                    For Each Record In BlastSearchRecords
                        For Each Hit In Record.Hits
                            tmpSeq.ID = Hit.Id
                            Dim Index = Seqs.BinarySearch(tmpSeq, Szunyi.Comparares.AllComparares.BySeqID)
                            If Index > -1 Then
                                For Each Hsp In Hit.Hsps
                                    Dim FullTranslated = Szunyi.Translate.TranaslateSeq(Seqs(Index), Hsp.HitFrame)
                                    Dim LongestAA = GetLongestORFFromAA(FullTranslated, True, True)

                                    Dim StartWithMet = GetFromFirstMethionin(LongestAA)
                                    StartWithMet.ID = Seqs(Index).ID
                                    Out.Add(StartWithMet)

                                Next
                            End If

                        Next
                    Next
                    If Out.Count = 0 Then Return Out

                    Dim DistincSeq = Szunyi.Sequences.SequenceManipulation.UniqueDistinct.Get1CopyBySeqAndID(Out)
                    Return DistincSeq

                End Function

                Public Shared Function GetLongestORFLengthFromSeqs(Seqs As List(Of Bio.ISequence),
                                                           MustStartWithATG As Boolean,
                                                           WithStopCodon As Boolean) As Integer
                    Dim Lengths As New List(Of Integer)
                    For Each Seq In Seqs
                        Lengths.Add(GetLongestORFLengthFromSeq(Seq, MustStartWithATG, WithStopCodon))
                    Next
                    Return Lengths.Max
                End Function

                Public Shared Function GetLongestORFLengthFromSeq(Seq As Bio.ISequence,
                                                           MustStartWithATG As Boolean,
                                                           WithStopCodon As Boolean) As Integer
                    Dim ORFs = GetLongestORFFromAA(Seq, MustStartWithATG, WithStopCodon)
                    Return ORFs.Count

                End Function
                ''' <summary>
                ''' Return Seq or Nothing 
                ''' </summary>
                ''' <param name="Seq"></param>
                ''' <param name="MustStarWithAtg"></param>
                ''' <param name="WithStopCodon"></param>
                ''' <returns></returns>
                Public Shared Function GetLongestORFFromAA(SeqI As Bio.ISequence,
                                                           MustStarWithAtg As Boolean,
                                                           WithStopCodon As Boolean) As Bio.ISequence
                    Dim seq As Bio.Sequence = SeqI
                    Dim tmp As New List(Of Szunyi.Common.StartAndEnd)
                    Dim FirstMet = GetFirstMethinonin(seq, MustStarWithAtg)
                    If IsNothing(FirstMet) = True Then Return Nothing
                    tmp.Add(GetFirstMethinonin(seq, MustStarWithAtg))

                    Do
                        tmp.Last.Endy = System.Array.IndexOf(Of Byte)(seq.ToArray, Bio.Alphabets.AmbiguousProtein.Ter, tmp.Last.Start + 1)
                        If tmp.Last.Endy < 0 Then ' No Stop Codon
                            tmp.Last.Endy = seq.Count - 1
                            Exit Do
                        Else

                            Dim n As New Szunyi.Common.StartAndEnd
                            n.Start = System.Array.IndexOf(Of Byte)(seq.ToArray, Bio.Alphabets.AmbiguousProtein.M, tmp.Last.Endy + 1)
                            If n.Start < 0 Then Exit Do
                            tmp.Add(n)
                        End If
                    Loop

                    Dim res = From k In tmp Order By k.Endy - k.Start Descending ' Order By Length Descending

                    Dim Length = res.First.Endy - res.First.Start ' Longest AA

                    Dim out As New Bio.Sequence(Bio.Alphabets.AmbiguousProtein, "")
                    If WithStopCodon = True Then
                        Dim bNewSeq(Length) As Byte
                        seq.CopyTo(bNewSeq, res.First.Start, Length + 1)

                        out = New Bio.Sequence(Alphabets.AmbiguousProtein, bNewSeq)
                        Dim alf2 As Int16 = 5
                    Else
                        Dim alf As Int16 = 56
                    End If

                    out.ID = seq.ID

                    If WithStopCodon = False Then
                        Dim bNewSeq(Length - 1) As Byte
                        seq.CopyTo(bNewSeq, res.First.Start, Length)
                        out = New Bio.Sequence(Alphabets.AmbiguousProtein, bNewSeq)
                    ElseIf res.First.Endy = seq.Count - 1 And WithStopCodon = True Then
                        Dim bNewSeq(Length) As Byte
                        seq.CopyTo(bNewSeq, res.First.Start, Length + 1)
                        out = New Bio.Sequence(Alphabets.AmbiguousProtein, bNewSeq)
                    Else
                        Dim bNewSeq(Length) As Byte
                        seq.CopyTo(bNewSeq, res.First.Start, Length + 1)
                        out = New Bio.Sequence(Alphabets.AmbiguousProtein, bNewSeq)
                    End If
                    Return out

                End Function
                ''' <summary>
                ''' If ATG must have and There is no M then return Nothing 
                ''' </summary>
                ''' <param name="Seq"></param>
                ''' <param name="MustStartWithATG"></param>
                ''' <returns></returns>
                Private Shared Function GetFirstMethinonin(Seq As Bio.ISequence, MustStartWithATG As Boolean) As Szunyi.Common.StartAndEnd
                    Dim x As New Szunyi.Common.StartAndEnd
                    If MustStartWithATG = False Then
                        x.Start = 0
                    Else
                        x.Start = System.Array.IndexOf(Of Byte)(Seq.ToArray, Bio.Alphabets.AmbiguousProtein.M)
                        If x.Start < 0 Then Return Nothing
                    End If
                    Return x
                End Function

                Public Shared Function GetLongestORFsFromAA(Seqs As List(Of Bio.ISequence),
                                                           MustStarWithAtg As Boolean,
                                                           WithStopCodon As Boolean) As List(Of Bio.ISequence)
                    Dim out As New List(Of Bio.ISequence)
                    For Each Seq In Seqs
                        out.Add(GetLongestORFFromAA(Seq, MustStarWithAtg, WithStopCodon))
                    Next
                    Return out

                End Function

                Public Shared Function GetFromFirstMethionin(Seq As Bio.ISequence) As Bio.ISequence

                    Dim Index As Integer = System.Array.IndexOf(Of Byte)(Seq.ToArray, Bio.Alphabets.AmbiguousProtein.M)
                    If Index < 0 Then
                        Return Seq
                    Else
                        Dim NewSeq = Seq.GetSubSequence(Index, Seq.Count - Index)
                        NewSeq.ID = Seq.ID

                        Return NewSeq
                    End If

                End Function
                Public Shared Function GetFromFirstMethionin(Seqs As List(Of Bio.ISequence)) As List(Of Bio.ISequence)
                    Dim out As New List(Of Bio.ISequence)
                    For Each Seq In Seqs
                        out.Add(GetFromFirstMethionin(Seq))
                    Next
                    Return out
                End Function
                ''' <summary>
                ''' Return AASeqs  Where Start with Methionine And Ends With Stop Codon
                ''' </summary>
                ''' <param name="Seqs"></param>
                ''' <returns></returns>
                Public Shared Function GetPerfectAASeqs(Seqs As List(Of Bio.ISequence)) As List(Of Bio.ISequence)
                    Dim out As New List(Of Bio.ISequence)
                    For Each Seq In Seqs
                        Dim t = Szunyi.Sequences.SequenceManipulation.SeqsToString.GetSeqAsString(Seq)
                        If t.StartsWith("M") Then
                            If t.IndexOf("*") = t.Count - 1 Then
                                out.Add(Seq)
                            End If
                        End If
                    Next
                    Return out
                End Function

                ''' <summary>
                ''' GetSeqs Containin defined AminoAcid between min and max
                ''' </summary>
                ''' <param name="Seqs"></param>
                ''' <param name="C"></param>
                ''' <param name="MinNof"></param>
                ''' <param name="MaxNof"></param>
                ''' <returns></returns>
                Public Shared Function GetSeqsAACountsBetween(Seqs As List(Of Bio.ISequence), C As Char, MinNof As Integer, MaxNof As Integer) As List(Of Bio.ISequence)
                    If IsNothing(Seqs) = True Then Return New List(Of Bio.ISequence)
                    Dim out As New List(Of Bio.ISequence)
                    Dim B As Byte = AscW(C)
                    For Each Seq In Seqs
                        If IsNothing(Seq) = False Then
                            Dim t = (From x In Seq.ToArray Where x = B).Count
                            If t >= MinNof AndAlso t <= MaxNof Then
                                out.Add(Seq)
                            End If
                        End If

                    Next
                    Return out
                End Function
                Public Shared Function GetNofAA(Seqs As List(Of Bio.ISequence), C As Char) As List(Of Integer)
                    If IsNothing(Seqs) = True Then Return New List(Of Integer)
                    Dim out As New List(Of Integer)
                    Dim B As Byte = AscW(C)
                    For Each Seq In Seqs
                        If IsNothing(Seq) = False Then
                            Dim t = (From x In Seq.ToArray Where x = B).Count
                            out.Add(t)
                        End If

                    Next
                    Return out
                End Function
                Public Shared Function GetORFsFromBeginning(SeqI As Bio.ISequence) As List(Of Bio.ISequence)
                    Dim seq As Bio.Sequence = SeqI
                    Dim tmp As New List(Of Szunyi.Common.StartAndEnd)
                    Dim x As New Szunyi.Common.StartAndEnd
                    x.Start = 0
                    tmp.Add(x)
                    Do
                        tmp.Last.Endy = System.Array.IndexOf(Of Byte)(seq.ToArray, Bio.Alphabets.AmbiguousProtein.Ter, tmp.Last.Start + 1)
                        If tmp.Last.Endy < 0 Then
                            tmp.Last.Endy = seq.Count - 1
                            Exit Do
                        Else

                            Dim n As New Szunyi.Common.StartAndEnd
                            n.Start = System.Array.IndexOf(Of Byte)(seq.ToArray, Bio.Alphabets.AmbiguousProtein.M, tmp.Last.Endy + 1)
                            If n.Start < 0 Then Exit Do
                            tmp.Add(n)
                        End If
                    Loop

                    Dim res As New List(Of Bio.ISequence)
                    For Each Item In tmp
                        Dim out As Bio.ISequence
                        Dim Length = Item.Endy - Item.Start

                        Dim bNewSeq(Length) As Byte
                        seq.CopyTo(bNewSeq, Item.Start, Length + 1)
                        out = New Bio.Sequence(Alphabets.AmbiguousProtein, bNewSeq)

                        out.ID = seq.ID
                        res.Add(out)
                    Next

                    res.Sort(Szunyi.Comparares.AllComparares.BySeqIDAndLength)

                    Return res
                End Function

                Public Shared Function GetORFS(Seq As Bio.ISequence) As List(Of Bio.ISequence)
                    Dim tmp As New List(Of Szunyi.Common.StartAndEnd)
                    Dim x As New Szunyi.Common.StartAndEnd

                    x.Start = System.Array.IndexOf(Of Byte)(Seq.ToArray, Bio.Alphabets.AmbiguousProtein.M)
                    If x.Start = -1 Then x.Start = 0
                    tmp.Add(x)
                    Do
                        tmp.Last.Endy = System.Array.IndexOf(Of Byte)(Seq.ToArray, Bio.Alphabets.AmbiguousProtein.Ter, tmp.Last.Start + 1)
                        If tmp.Last.Endy < 0 Then
                            tmp.Last.Endy = Seq.Count - 1
                            Exit Do
                        Else

                            Dim n As New Szunyi.Common.StartAndEnd
                            n.Start = System.Array.IndexOf(Of Byte)(Seq.ToArray, Bio.Alphabets.AmbiguousProtein.M, tmp.Last.Endy + 1)
                            If n.Start < 0 Then Exit Do
                            tmp.Add(n)
                        End If
                    Loop

                    Dim res As New List(Of Bio.ISequence)
                    For Each Item In tmp
                        Dim out As Bio.ISequence
                        Dim Length = Item.Endy - Item.Start

                        Dim bNewSeq(Length) As Byte
                        Dim s As Bio.Sequence = Seq
                        s.CopyTo(bNewSeq, Item.Start, Length + 1)
                        out = New Bio.Sequence(Alphabets.AmbiguousProtein, bNewSeq)

                        out.ID = Seq.ID
                        res.Add(out)
                    Next

                    res.Sort(Szunyi.Comparares.AllComparares.BySeqIDAndLength)

                    Return res
                End Function

                Friend Shared Function GetAADIstancesFromSeqs(Seqs As List(Of Sequence), B As Byte) As String
                    Dim res As New SortedList(Of Bio.ISequence, List(Of Integer))
                    Dim str As New System.Text.StringBuilder
                    If IsNothing(Seqs) = True Then Return Nothing
                    For Each Seq In Seqs
                        Dim Index As Integer = 0
                        Dim Positions As New List(Of Integer)
                        Do
                            If Seq.Item(Index) = B Then Positions.Add(Index)
                            Index += 1
                        Loop Until Index = Seq.Count - 1
                        str.Append(Seq.ID).Append(vbTab).Append(Positions.Count).Append(vbTab)
                        Dim Diffs As New List(Of Integer)
                        For i1 = 0 To Positions.Count - 2
                            Diffs.Add(Positions(i1 + 1) - Positions(i1))
                            str.Append(Positions(i1 + 1) - Positions(i1)).Append(vbTab)
                        Next
                        '   res.Add(Seq, Diffs)
                        str.AppendLine()

                    Next
                    If Seqs.Count > 0 Then str.Length -= 2
                    Return str.ToString
                End Function
            End Class

            Public Class tmp


                ''' <summary>
                ''' Return Empty String Or SeqID + Start + End (if all of the string founded in one chromosome)
                ''' </summary>
                ''' <param name="subSequences"></param>
                ''' <param name="Seqs"></param>
                ''' <returns></returns>
                Public Shared Function GetPerfectLocationsByStrings(subSequences As List(Of String),
                                                            sSeqs As List(Of String), Seqs As List(Of Bio.ISequence)) As String
                    Dim str As New StringBuilder
                    For i1 = 0 To sSeqs.Count - 1
                        Dim str1 As New StringBuilder
                        For Each SubSequence In subSequences

                            Dim Index = sSeqs(i1).IndexOf(SubSequence, StringComparison.InvariantCultureIgnoreCase)
                            If Index < 0 Then
                                str1.Length = 0
                                Exit For
                            Else
                                str1.Append(Seqs(i1).ID).Append(vbTab).Append(Index).Append(vbTab).Append(Index + SubSequence.Length).AppendLine()
                            End If
                        Next
                        If str1.Length <> 0 Then
                            str1.Length -= vbCrLf.Length
                            Return str1.ToString
                        End If
                    Next
                    Return String.Empty
                End Function

                Public Shared Function GetReverseComplements(subSequencesAsString As List(Of String)) As List(Of String)
                    Dim out As New List(Of String)
                    For Each s In subSequencesAsString
                        Dim x As New Bio.Sequence(Alphabets.AmbiguousDNA, s.ToUpper)
                        Dim t As Bio.Sequence = x.GetReverseComplementedSequence
                        out.Add(t.ConvertToString(0, t.Count))
                    Next
                    Return out
                End Function


                Public Shared Function CreateGenBankFrom_mRNA_And_CDS(seqs As List(Of Sequence)) As List(Of Bio.ISequence)
                    Dim out As New List(Of Bio.ISequence)
                    If IsNothing(seqs) = True Then Return Nothing
                    For i1 = 0 To seqs.Count - 1
                        Dim Md As GenBankMetadata = GenBankMetaDataManipulation.CreateNAGenBankMetaData(seqs(i1))
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
                            Dim oriseq = SeqsToString.GetSeqAsString(seqs(i1))
                            If seqs(i1).ID = seqs(i2).ID Then

                                Dim CDSSeq = SeqsToString.GetSeqAsString(seqs(i2))
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
                    '      Szunyi.IO.Export.SaveSequencesToSingleGenBank(out)
                    Return out
                End Function


            End Class

            Public Class Get_Sub_Sequence
                Public Shared Function Processed(Seq As Bio.ISequence, feat As FeatureItem) As Bio.ISequence
                    feat.Location = Szunyi.Location.Common.Get_Correct_Location(feat.Location)
                    Dim NOFiNTRON = Szunyi.Features.FeatureManipulation.GetLocations.GetIntronLocations(feat)
                    If NOFiNTRON.Count > 0 Then
                        Dim alf As Int16 = 54
                    End If
                    Dim t = feat.GetSubSequence(Seq)

                    If feat.Location.IsComplementer = True Then
                        Return t.GetReversedSequence
                    Else
                        Return t
                    End If
                End Function
                Public Shared Function Whole(Seq As Bio.ISequence, feat As FeatureItem) As Bio.ISequence
                    Dim tmpSeq = Seq.GetSubSequence(feat.Location.LocationStart - 1, feat.Location.LocationEnd - feat.Location.LocationStart + 1)

                    If feat.Location.IsComplementer = True Then
                        Return tmpSeq.GetReverseComplementedSequence
                    Else
                        Return tmpSeq
                    End If
                End Function
                Public Shared Function Whole(Seq As Bio.ISequence, l As ILocation) As Bio.ISequence
                    Dim tmpSeq = Seq.GetSubSequence(l.LocationStart, l.LocationEnd - l.LocationStart)

                    If l.IsComplementer = True Then
                        Return tmpSeq.GetReverseComplementedSequence
                    Else
                        Return tmpSeq
                    End If
                End Function

                Public Shared Function Before(Seq As ISequence, loci As Basic_Location, Sort As Sort_Locations_By, length As Integer) As Bio.ISequence
                    Dim Basic As Integer = -1
                    Select Case Sort
                        Case  Sort_Locations_By.TSS
                            Basic = loci.Location.LocationStart
                        Case  Sort_Locations_By.LE
                            Basic = loci.Location.LocationEnd
                        Case  Sort_Locations_By.PAS
                            Basic = loci.Location.PAS
                        Case Sort_Locations_By.TSS
                            Basic = loci.Location.TSS
                    End Select
                    If loci.IsComplement = False Then
                        Basic -= length
                    End If
                    Dim x = Seq.GetSubSequence(Basic, length)
                    If loci.IsComplement = True Then
                        Return x.GetReverseComplementedSequence
                    Else
                        Return x
                    End If
                End Function
                Public Shared Function Before(Seq As ISequence, loci As ILocation, Sort As Sort_Locations_By, length As Integer) As Bio.ISequence
                    Dim Basic As Integer = -1
                    Select Case Sort
                        Case  Sort_Locations_By.TSS
                            Basic = loci.LocationStart
                        Case  Sort_Locations_By.LE
                            Basic = loci.LocationEnd
                        Case  Sort_Locations_By.PAS
                            Basic = loci.PAS
                            If loci.IsComplementer = True Then Basic = Basic - 1


                        Case Sort_Locations_By.TSS
                            Basic = loci.TSS
                    End Select
                    If loci.IsComplementer = False Then
                        Basic -= length
                    Else

                    End If
                    Dim x = Seq.GetSubSequence(Basic, length)
                    If loci.IsComplementer = True Then
                        Return x.GetReverseComplementedSequence
                    Else
                        Return x
                    End If
                End Function
            End Class
        End Namespace



        Public Class KmerManipulation
            ''' <summary>
            ''' Return All of the Kmer in Sequnce as List Of String or Empty List
            ''' </summary>
            ''' <param name="Seq"></param>
            ''' <param name="KmerLength"></param>
            ''' <returns></returns>
            Public Shared Function GetAllKmer(Seq As Bio.ISequence, KmerLength As Integer) As List(Of String)
                Dim sSeq = Szunyi.Sequences.SequenceManipulation.SeqsToString.GetSeqAsString(Seq)
                Dim out As New List(Of String)
                If Seq.Count >= KmerLength Then
                    For i1 = 0 To Seq.Count - KmerLength
                        out.Add(sSeq.Substring(i1, KmerLength))
                    Next
                End If
                Return out
            End Function
        End Class

        Public Class MaintainUniqueReads
            Public Property Files As New List(Of FileInfo)
            Public Property MinNofReads As Integer
            Public Property Res As New Dictionary(Of String, List(Of SeqwCount))

            Public ReadOnly Property Type As String = Szunyi.Constants.BackGroundWork.MaintainUniqeSequence
            Public Sub New(Files As List(Of FileInfo), MinNofReads As Integer)
                Me.Files = Files
                Me.MinNofReads = MinNofReads
            End Sub
            Public Sub New(File As FileInfo, MinNofReads As Integer)
                Me.Files.Add(File)
                Me.MinNofReads = MinNofReads
            End Sub
            Public Sub New(Folder As String, MinNofReads As Integer)
                Me.MinNofReads = MinNofReads
                Me.Files = Szunyi.IO.Directory.GetAllFilesFromFolder(New DirectoryInfo(Folder))

            End Sub
            Public Function DoIT(Seqs As List(Of Bio.ISequence))
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
                Dim Out As New List(Of Bio.ISequence)
                Dim Index As Integer = 0
                For Each Item In Rest
                    For Each SeqwCount In Item.Value
                        If SeqwCount.Count >= Me.MinNofReads Then
                            Out.Add(SeqwCount.Seq)
                            Index += 1
                            Out.Last.ID = Index & "_" & SeqwCount.Count
                        End If
                    Next
                Next
                Return Out
            End Function
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
                        Dim Out As New List(Of Bio.ISequence)
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
            Public Property BasicSeqs As List(Of Bio.ISequence)
            Public Property ModifiedLengthSeqs As List(Of Bio.ISequence)
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
            Public Sub New(Seqs As List(Of Bio.ISequence),
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

            Private Function GetModifiedLengthSeqs(BasicSeqs As List(Of Bio.ISequence), length As Integer, onlyStarts As Boolean) As List(Of Sequence)
                Throw New NotImplementedException()
            End Function
            Public Function GetSequneces() As List(Of Bio.ISequence)
                Dim out As New List(Of Bio.ISequence)
                For Each Item In Result
                    For Each Seq In Item.Value
                        out.Add(Seq.Seq)
                    Next
                Next
                Return out
            End Function
            Private Sub Export(res As Dictionary(Of String, List(Of SeqwCount)), File As FileInfo)

                If Me.OutPutType = Szunyi.Constants.OutPutType.AsTabFile Then
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
                    Dim NewFIle = Szunyi.IO.Files.Get_New_FileName.GetNewFile(File, ".tab")
                    Szunyi.IO.Export.SaveText(str.ToString, NewFIle)
                End If

            End Sub

        End Class

        Public Class SeqwCount
            Public Property Seq As Bio.ISequence
            Public Property Count As Integer = 1
            Public Property sSeq As String
            Public Sub New(Seq As Bio.ISequence, Optional Length As Integer = 0, Optional OnlyStart As Boolean = False)
                If Length > Seq.Count Then
                    ' It is false no valid result
                Else
                    Try
                        Dim s As Bio.Sequence = Seq
                        Me.sSeq = s.ConvertToString(0, Seq.Count)
                    Catch ex As Exception

                    End Try

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

        End Class
    End Namespace
End Namespace


