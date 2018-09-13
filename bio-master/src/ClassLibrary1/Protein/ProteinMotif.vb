Imports System.Text.RegularExpressions
Imports Bio
Imports ClassLibrary1.Szunyi.Sequences.SequenceManipulation
Imports ClassLibrary1.Szunyi.Text.TableManipulation

Namespace Szunyi
    Namespace Protein
        Public Class ProteinMotif
            Public Property Name As String
            Public Property Pattern As String
            Public Sub New(Name As String, Pattern As String)
                Me.Name = Name
                Me.Pattern = Pattern
            End Sub
            Public Sub New(CompressedPattern As String)
                Dim s = Split(CompressedPattern, " ")
                Dim str As New System.Text.StringBuilder
                For Each s1 In s
                    Dim i1 As Integer
                    If Integer.TryParse(s1, i1) = True Then
                        str.Append("C").Append(Szunyi.Text.General.Multiply(".", i1)).Append("C")
                    Else ' X
                        str.Append("\w*")
                    End If
                Next
                Me.Name = CompressedPattern
                Me.Pattern = str.ToString.Replace("CC", "C")
            End Sub
        End Class

        Public Class NCRMotifs
            Public Property Motifs As New List(Of ProteinMotif)
            Public Sub New()
                Motifs.Add(New ProteinMotif("5 X 4"))
                Motifs.Add(New ProteinMotif("4 X 5"))
                Motifs.Add(New ProteinMotif("5 X 5"))
                Motifs.Add(New ProteinMotif("7 X 4"))
                Motifs.Add(New ProteinMotif("7 4 1"))
                Motifs.Add(New ProteinMotif("4 7 4 1"))
                Motifs.Add(New ProteinMotif("5 4 X 4 1"))
                Motifs.Add(New ProteinMotif("5 4 7 X 1"))
                Motifs.Add(New ProteinMotif("5 5 7 X 1"))
                Motifs.Add(New ProteinMotif("5 4 7 4 1"))
                Motifs.Add(New ProteinMotif("5 5 7 4 1"))
                '    Motifs.Add(New ProteinMotif("1"))
            End Sub


        End Class

        Public Class ProteinManipulation
            Public Shared Function Discover_Motifs(Seqs As List(Of Bio.ISequence),
                                                   Motifs As List(Of ProteinMotif)) As List(Of String)()

                Dim res(Motifs.Count - 1) As List(Of String)
                For i1 = 0 To Motifs.Count - 1
                    res(i1) = Discover_Motif(Seqs, Motifs(i1))
                Next
                Return res
            End Function
            Public Shared Function Discover_Motif(Seqs As List(Of Bio.ISequence),
                                                   Motif As ProteinMotif) As List(Of String)
                Dim res As New List(Of String)
                For Each Seq In Seqs
                    If HasSeqContainsMotif(Seq, Motif) Then
                        res.Add("+")
                    Else
                        res.Add("-")
                    End If

                Next
                Return res
            End Function
            Public Shared Function GetNames(Motifs As List(Of ProteinMotif)) As String
                Dim MotifNames = (From x In Motifs Select x.Name).ToList
                Return Szunyi.Text.General.GetText(MotifNames, vbTab)

            End Function

            ''' <summary>
            ''' Return Counts of Each Found Motif as array of integer
            ''' </summary>
            ''' <param name="Seqs"></param>
            ''' <param name="Motifs"></param>
            ''' <returns></returns>
            Public Shared Function Motif_Statistic(Seqs As List(Of Bio.ISequence), Motifs As List(Of ProteinMotif)) As String
                Dim Counts(Motifs.Count - 1) As Integer
                For Each Seq In Seqs
                    Dim s = Szunyi.Sequences.SequenceManipulation.SeqsToString.GetSeqAsString(Seq)
                    For i1 = 0 To Motifs.Count - 1
                        If HasSeqContainsMotif(s, Motifs(i1)) Then Counts(i1) += 1

                    Next
                Next

                Return Szunyi.Text.General.GetText(Counts, vbTab)
            End Function
            ''' <summary>
            ''' Return Counts of Each Found Motif as array of integer
            ''' </summary>
            ''' <param name="Seqs"></param>
            ''' <param name="Motifs"></param>
            ''' <returns></returns>
            Public Shared Function Detailed_Motif_Statistic(Seqs As List(Of Bio.ISequence), Motifs As List(Of ProteinMotif)) As String
                Dim Counts(Motifs.Count - 1) As List(Of Bio.ISequence)
                For i1 = 0 To Counts.Count - 1
                    Counts(i1) = New List(Of Bio.ISequence)
                Next
                For Each Seq In Seqs
                    Try
                        Dim s1 As Bio.Sequence = Seq
                        Dim s = s1.ConvertToString
                        For i1 = 0 To Motifs.Count - 1
                            If HasSeqContainsMotif(s, Motifs(i1)) Then
                                Counts(i1).Add(Seq)
                            End If

                        Next
                    Catch ex As Exception

                    End Try

                Next
                Dim str As New System.Text.StringBuilder
                str.Append(vbTab).Append(GetNames(Motifs)).AppendLine()
                For i1 = 0 To Motifs.Count - 1
                    str.Append(Motifs(i1).Name)
                    For i2 = 0 To Motifs.Count - 1
                        Dim t = Szunyi.Sequences.SequenceManipulation.UniqueDistinct.GetCommonSeqs(Counts(i1), Counts(i2))
                        str.Append(vbTab).Append(t.Count)
                    Next
                    str.AppendLine()
                Next
                Return str.ToString
            End Function

            ''' <summary>
            ''' Retrun List of  Bio.ISequence or Empty List
            ''' </summary>
            ''' <param name="Seqs"></param>
            ''' <param name="Motifs"></param>
            ''' <returns></returns>
            Public Shared Function GetSeqsContainsMotifs(Seqs As List(Of Bio.ISequence), Motifs As List(Of ProteinMotif)) As List(Of Bio.ISequence)
                Dim out As New List(Of Bio.ISequence)
                If IsNothing(Seqs) = True Then Return New List(Of Bio.ISequence)
                For Each Seq In Seqs
                    If IsNothing(Seq) = False Then
                        Dim sSeq = Szunyi.Sequences.SequenceManipulation.SeqsToString.GetSeqAsString(Seq)
                        For Each Motif In Motifs
                            Dim TheMatch As Match = Regex.Match(sSeq, Motif.Pattern, RegexOptions.IgnoreCase)
                            If TheMatch.Success = True Then
                                out.Add(Seq)
                                Exit For
                            End If
                        Next
                    End If

                Next
                Return out
            End Function
            ''' <summary>
            ''' Retrun List of  Bio.ISequence or Empty List
            ''' </summary>
            ''' <param name="Seqs"></param>
            ''' <param name="Motifs"></param>
            ''' <returns></returns>
            Public Shared Function GetSeqNoContainsMotifs(Seqs As List(Of Bio.ISequence), Motifs As List(Of ProteinMotif)) As List(Of Bio.ISequence)
                Dim out As New List(Of Bio.ISequence)
                If IsNothing(Seqs) = True Then Return New List(Of Bio.ISequence)
                For Each Seq In Seqs
                    If IsNothing(Seq) = False Then
                        Dim sSeq = Szunyi.Sequences.SequenceManipulation.SeqsToString.GetSeqAsString(Seq)
                        Dim Has As Boolean = False
                        For Each Motif In Motifs
                            Dim TheMatch As Match = Regex.Match(sSeq, Motif.Pattern, RegexOptions.IgnoreCase)
                            If TheMatch.Success = True Then
                                Has = True
                            End If
                        Next
                        If Has = False Then out.Add(Seq)
                    End If

                Next
                Return out
            End Function

            Public Shared Function HasSeqContainsMotifs(Seq As Bio.ISequence, Motifs As List(Of ProteinMotif)) As Boolean
                Dim sSeq =Szunyi.Sequences.SequenceManipulation.SeqsToString.GetSeqAsString(Seq)
                For Each Motif In Motifs
                    If HasSeqContainsMotif(Seq, Motif) = True Then
                        Return True
                    End If

                Next

                Return False
            End Function
            Public Shared Function HasSeqContainsMotif(Seq As Bio.ISequence, Motif As ProteinMotif) As Boolean
                Dim sSeq = Szunyi.Sequences.SequenceManipulation.SeqsToString.GetSeqAsString(Seq)

                Dim TheMatch As Match = Regex.Match(sSeq, Motif.Pattern, RegexOptions.IgnoreCase)
                If TheMatch.Success = True Then
                    Return True

                End If


                Return False
            End Function
            Public Shared Function HasSeqContainsMotif(Seq As String, Motif As ProteinMotif) As Boolean

                Dim TheMatch As Match = Regex.Match(Seq, Motif.Pattern, RegexOptions.IgnoreCase)
                If TheMatch.Success = True Then
                    Return True

                End If


                Return False
            End Function

            Public Shared Function GetNCRsValidation(SeqswDescriptions As Items_With_Properties,
                                                     Seqs As List(Of Bio.ISequence),
                                                     Prefix As String) As Items_With_Properties

                Dim ShortIDs = Szunyi.Sequences.SequenceManipulation.Properties.GetShortIDs(Seqs)
                Dim IDs = Szunyi.Sequences.SequenceManipulation.Properties.GetIDs(Seqs)

                Dim SeqsAsString = Szunyi.Sequences.SequenceManipulation.SeqsToString.GetSeqsAsString(Seqs)
                SeqswDescriptions.AddByKeys(Prefix & " Seq", IDs, SeqsAsString)

                Dim MWs = Szunyi.Sequences.SequenceManipulation.Properties.GetMWs(Seqs)
                SeqswDescriptions.AddByKeys(Prefix & " MWs", IDs, MWs)

                Dim IPs = Szunyi.Sequences.SequenceManipulation.Properties.GetIPs(Seqs)
                SeqswDescriptions.AddByKeys(Prefix & " IPs", IDs, IPs)

                Dim Lengths = Szunyi.Sequences.SequenceManipulation.Properties.GetLengths(Seqs)
                SeqswDescriptions.AddByKeys(Prefix & " Lengths", IDs, Lengths)

                Dim NofCys = Szunyi.Sequences.SequenceManipulation.AA.GetNofAA(Seqs, "C")
                SeqswDescriptions.AddByKeys(Prefix & " Nof Cys", IDs, NofCys)

                Dim TheNCRMotifs As New Szunyi.Protein.NCRMotifs()
                Dim WithNCRMotif = Szunyi.Protein.ProteinManipulation.GetSeqsContainsMotifs(Seqs, TheNCRMotifs.Motifs)
                Dim NoNCRMotif = Szunyi.Protein.ProteinManipulation.GetSeqNoContainsMotifs(Seqs, TheNCRMotifs.Motifs)

                SeqswDescriptions.AddByKeys(Prefix & " Has NCRMotif", Szunyi.Sequences.SequenceManipulation.Common.GetSeqIDs(WithNCRMotif), "+", "-")

                Dim t = Szunyi.Protein.ProteinManipulation.Discover_Motifs(Seqs, TheNCRMotifs.Motifs)

                Dim Motif_Names = From x In TheNCRMotifs.Motifs Select x.Name
                SeqswDescriptions.Add_Values_WithOut_Keys(Motif_Names.ToList, t)

                Return SeqswDescriptions
            End Function
        End Class
    End Namespace
End Namespace

