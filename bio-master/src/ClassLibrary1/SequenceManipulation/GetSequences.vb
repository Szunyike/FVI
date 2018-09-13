Imports Bio
Imports Bio.IO.GenBank

Namespace Szunyi
    Namespace Sequences
        Namespace SequenceManipulation
            Public Class Identify
                Public Shared Function IsBioSequence(Seq As Bio.ISequence) As Boolean
                    Dim t = Seq.GetType
                    If t.FullName = "Bio.Sequence" Then Return True

                    Return False

                End Function
                Public Shared Function IsQualitativeSequence(Seq As Bio.ISequence) As Boolean
                    Dim t = Seq.GetType
                    If t.FullName = "Bio.QualitativeSequence" Then Return True

                    Return False

                End Function
            End Class
            Public Class DNA
                Public Shared Function Get_Max_Consequitve(Seq As Bio.Sequence) As KeyValuePair(Of Byte, Integer)
                    Dim currByte = Seq(0)
                    Dim count = 1
                    Dim maxCount As Integer = 0
                    Dim maxByte As Byte
                    For i1 = 1 To Seq.Count - 1
                        If Seq(i1) = currByte Then
                            count += 1
                        Else
                            If maxCount >= count Then
                                currByte = Seq(i1)
                                count = 1
                            Else
                                maxByte = currByte
                                currByte = Seq(i1)
                                maxCount = count
                                count = 1
                            End If
                        End If
                    Next
                    Dim x As New KeyValuePair(Of Byte, Integer)(maxByte, maxCount)
                    Return x
                End Function
                Public Shared Function FindPolyAs(Seqs As List(Of Bio.ISequence), Optional WIdth As Integer = 10, Optional MaxNotA As Integer = 2) As List(Of Integer)
                    Dim out As New List(Of Integer)
                    For Each seq In Seqs
                        out.Add(FindPolyA(seq, WIdth, MaxNotA))
                    Next
                    Return out
                End Function

                Public Shared Function FindPolyA(Seq As Bio.ISequence, Optional WIdth As Integer = 10, Optional MaxNotA As Integer = 2) As Integer
                    Dim HasA As Boolean = False
                    Dim Apos As Integer = -1
                    For i1 = 0 To Seq.Count - 1 - WIdth
                        Dim NofNotA As Integer = 0

                        For i2 = i1 To i1 + WIdth
                            If Seq(i2) <> 65 Then
                                NofNotA += 1
                                If NofNotA > MaxNotA Then Exit For
                            Else

                            End If
                        Next

                        If NofNotA < MaxNotA Then
                            HasA = True
                            For i2 = i1 To i1 + WIdth
                                If Seq(i2) = 65 Then
                                    Apos = i2
                                    Exit For
                                End If
                            Next

                        Else

                        End If

                    Next

                    If HasA = True Then
                        Return Apos
                    Else
                        Return -1
                    End If
                End Function

                Public Shared Function FindPolyT(Seq As Sequence, Optional WIdth As Integer = 10, Optional MaxNotA As Integer = 2) As Integer
                    Dim HasA As Boolean = False
                    Dim Apos As Integer = -1
                    For i1 = 0 To Seq.Count - 1 - WIdth
                        Dim NofNotA As Integer = 0

                        For i2 = i1 To i1 + WIdth
                            If Seq(i2) <> 81 Then
                                NofNotA += 1
                                If NofNotA > MaxNotA Then Exit For
                            Else

                            End If
                        Next

                        If NofNotA < MaxNotA Then
                            HasA = True
                            Apos = i1
                            Exit For
                        Else

                        End If

                    Next

                    If HasA = True Then
                        Return Apos
                    Else
                        Return -1
                    End If
                End Function

            End Class
            Public Class GetSequences
                Public Shared Property SeqIDComparer As New Comparares.OneByOne.SequenceIDComparer
                Public Shared Property LociBuilder As New Bio.IO.GenBank.LocationBuilder

                Public Shared Function ByID(Seqs As List(Of Bio.ISequence), SeqID As String) As Bio.ISequence
                    Dim Seq As New Bio.Sequence(Alphabets.AmbiguousDNA, "A")
                    Seq.ID = SeqID
                    Dim Index = Seqs.BinarySearch(Seq, SeqIDComparer)
                    If Index > -1 Then
                        Return Seqs(Index)
                    Else
                        Return Nothing
                    End If

                End Function
                Public Shared Function ByIDs(Seqs As List(Of Bio.ISequence), SeqIDs As List(Of String)) As List(Of Bio.ISequence)
                    Dim Seq As New Bio.Sequence(Alphabets.AmbiguousDNA, "A")
                    Seqs.Sort(Szunyi.Comparares.AllComparares.BySeqID)
                    Dim Out As New List(Of Bio.ISequence)
                    For Each Index In SeqIDs
                        Out.Add(ByID(Seqs, Index))
                    Next
                    Return Out
                End Function
                Public Shared Function ByID(Seqs As List(Of Szunyi.ListOf.ExtFeatureList), SeqID As String) As Bio.Sequence
                    Dim Seq As New Bio.Sequence(Alphabets.AmbiguousDNA, "A")
                    Seq.ID = SeqID
                    For Each Item In Seqs
                        Dim Index = Item.Seqs.BinarySearch(Seq, SeqIDComparer)
                        If Index > -1 Then
                            Return Item.Seqs(Index)

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
                Public Shared Function From_Loci(Seq As Bio.Sequence, Loci As Bio.IO.GenBank.Location) As List(Of Bio.ISequence)
                    Dim out As New List(Of Bio.ISequence)
                    If Loci.Operator = LocationOperator.Complement Then
                        If Loci.SubLocations.First.Operator = LocationOperator.Join Then
                            For Each subloci In Loci.SubLocations.First.SubLocations
                                Dim nSeq = Szunyi.Sequences.SequenceManipulation.GetSequences.SeqFromStartAndEnd(Seq, subloci.LocationStart - 1, subloci.LocationEnd)
                                Dim rSeq = nSeq.GetReverseComplementedSequence

                                out.Add(rSeq)
                            Next
                        Else
                            Dim nSeq = Szunyi.Sequences.SequenceManipulation.GetSequences.SeqFromStartAndEnd(Seq, Loci.SubLocations.First.LocationStart - 1, Loci.SubLocations.First.LocationEnd)
                            Dim rSeq = nSeq.GetReverseComplementedSequence

                            out.Add(rSeq)

                        End If
                    Else
                        If Loci.Operator = LocationOperator.Join = False Then
                            out.Add(Szunyi.Sequences.SequenceManipulation.GetSequences.SeqFromStartAndEnd(Seq, Loci.LocationStart - 1, Loci.LocationEnd))
                        Else
                            For Each Loct In Loci.SubLocations
                                Dim nSeq = Szunyi.Sequences.SequenceManipulation.GetSequences.SeqFromStartAndEnd(Seq, Loct.LocationStart - 1, Loct.LocationEnd)
                                out.Add(nSeq)

                            Next
                        End If


                    End If
                    Return out
                End Function
#Region "GetSequenceByPosition"

                Public Shared Function SeqFromStartAndLengthAsString(seq As Sequence, Start As Integer, Length As Integer) As String
                    Dim TheSeq = ByStartAndLength(seq, Start, Length)
                    Return Szunyi.Sequences.SequenceManipulation.SeqsToString.GetSeqAsString(TheSeq)
                End Function
                Public Shared Function SeqFromStartAndLengthReverseComplemented(seq As Sequence, start As Integer, length As Integer) As Bio.Sequence
                    Dim TheSeq = ByStartAndLength(seq, start, length)
                    Return TheSeq.GetReverseComplementedSequence
                End Function
                Public Shared Function SeqFromStartAndLengthAsStringReverseComplemented(seq As Sequence, start As Integer, length As Integer) As String
                    Dim TheSeq As Bio.Sequence = SeqFromStartAndLengthReverseComplemented(seq, start, length)
                    Return TheSeq.ConvertToString(0, TheSeq.Count)
                End Function
                Public Shared Function ByStartAndLength(Seqs As List(Of ISequence), Start As Integer, Length As Integer) As List(Of ISequence)
                    Dim out As New List(Of Bio.ISequence)
                    For Each Seq In Seqs
                        out.Add(SeqFromStartAndLength(Seq, Start, Length))
                    Next
                    Return out
                End Function
                Public Shared Function ByStartAndLength(seq As ISequence, Start As Integer, Length As Integer) As ISequence
                    Return SeqFromStartAndLength(seq, Start, Length)
                End Function
                Public Shared Function SeqFromStartAndEndAsString(seq As Sequence, Start As Integer, Endy As Integer) As String
                    Dim TheSeq = SeqFromStartAndEnd(seq, Start, Endy)
                    Return TheSeq.ConvertToString(0, TheSeq.Count)
                End Function
                Public Shared Function FromLineOneSeq(Line As String, alp As Bio.DnaAlphabet) As Bio.Sequence
                    Dim str As New System.Text.StringBuilder


                    For Each ch As Char In Line
                        Dim b As Byte = AscW(ch)
                        If alp.nucleotideValueMap.ContainsKey(b) Then
                            str.Append(ch)
                        Else
                            Dim kjr As Int16 = 54
                        End If
                    Next

                    Return New Bio.Sequence(Bio.Alphabets.AmbiguousDNA, str.ToString)
                End Function
                Public Shared Function FromLinesOneSeq(Lines As List(Of String), alp As Bio.DnaAlphabet) As Bio.Sequence
                    Dim str As New System.Text.StringBuilder

                    For Each Line In Lines
                        For Each ch As Char In Line
                            Dim b As Byte = AscW(ch)
                            If alp.nucleotideValueMap.ContainsKey(b) Then
                                str.Append(ch)
                            Else
                                Dim kjr As Int16 = 54
                            End If
                        Next
                    Next
                    Return New Bio.Sequence(Bio.Alphabets.AmbiguousDNA, str.ToString)
                End Function
                Public Shared Function FromStrings(Seqs() As String) As List(Of Sequence)
                    Dim out As New List(Of Bio.Sequence)
                    For Each Item In Seqs
                        out.Add(New Bio.Sequence(Alphabets.AmbiguousDNA, Item))
                    Next
                    Return out
                End Function

                Public Shared Function SeqFromStartAndEnd(seq As Sequence, Start As Integer, Endy As Integer) As Sequence
                    Return SeqFromStartAndLength(seq, Start, Endy - Start)
                End Function

                Public Shared Function SeqFromStartAndEndReverseComplemented(seq As Sequence, Start As Integer, Endy As Integer) As Bio.Sequence
                    Dim TheSeq = SeqFromStartAndLength(seq, Start, Endy - Start)
                    Return TheSeq.GetReverseComplementedSequence
                End Function
                Public Shared Function SeqFromStartAndEndAsStringReverseComplemented(seq As Sequence, Start As Integer, Endy As Integer) As String
                    Dim TheSeq As Bio.Sequence = SeqFromStartAndEndReverseComplemented(seq, Start, Endy)
                    Return TheSeq.ConvertToString(0, TheSeq.Count)
                End Function
                Public Shared Function SeqFromStartAndLength(seq As Sequence, Start As Integer, Length As Integer) As Bio.Sequence
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

                Public Shared Function Create_GenBankFrom_Genes_And_CDSs(NA_Seqs As List(Of Bio.ISequence), CDS_Seqs As List(Of Bio.ISequence)) As List(Of Bio.ISequence)
                    NA_Seqs.Sort(Szunyi.Comparares.AllComparares.BySeqID)

                    Dim Out As New List(Of Bio.ISequence)
                    For Each CDS_Seq In CDS_Seqs
                        Dim Index = NA_Seqs.BinarySearch(CDS_Seq, Szunyi.Comparares.AllComparares.BySeqID)
                        If Index > -1 Then
                            Dim Na_Seq = NA_Seqs(Index)
                            Dim Md = Szunyi.Features.FeatureManipulation.GenBankMetaDataManipulation.CreateNAGenBankMetaData(Na_Seq)
                            Dim nSeq = Szunyi.Sequences.SequenceManipulation.CDS.GetCDSFromNAAndAA(Na_Seq, CDS_Seq, Md)
                            nSeq.ID = CDS_Seq.ID
                            If IsNothing(nSeq) = False Then
                                Out.Add(nSeq)
                            End If
                        Else
                            Dim tmpSeq = Szunyi.Sequences.SequenceManipulation.Common.CloneSeq(CDS_Seq)
                            tmpSeq = Szunyi.Sequences.SequenceManipulation.ID.Remove_Number_At_The_End_Of_The_ID(tmpSeq)
                            Index = NA_Seqs.BinarySearch(tmpSeq, Szunyi.Comparares.AllComparares.BySeqID)
                            If Index > -1 Then
                                Dim Na_Seq1 = NA_Seqs(Index)
                                Na_Seq1.ID = CDS_Seq.ID
                                Dim Md1 = Szunyi.Features.FeatureManipulation.GenBankMetaDataManipulation.CreateNAGenBankMetaData(Na_Seq1)
                                Dim nSeq1 = Szunyi.Sequences.SequenceManipulation.CDS.GetCDSFromNAAndAA(Na_Seq1, tmpSeq, Md1)
                                nSeq1.ID = CDS_Seq.ID
                                If IsNothing(nSeq1) = False Then
                                    Out.Add(nSeq1)
                                End If

                            End If
                        End If

                    Next
                    Return Out
                End Function


#End Region

            End Class

            Public Class Counts
                Public Shared Function Get_Sequence_Length_Wo_Ter_And_Gap_Symbols(Seq As Bio.ISequence) As Integer
                    Dim alp = Bio.Alphabets.AmbiguousProtein
                    Dim Index As Integer = 0
                    For Each s In Seq
                        If alp.CheckIsGap(s) = False AndAlso alp.Ter <> s Then
                            Index += 1
                        Else
                            Dim alf As Integer = 54
                        End If
                    Next
                    Return Index
                End Function
            End Class
        End Namespace
    End Namespace
End Namespace

