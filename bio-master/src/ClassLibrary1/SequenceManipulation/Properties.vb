Imports System.Text.RegularExpressions
Imports Bio
Imports ClassLibrary1.Szunyi.Protein

Namespace Szunyi
    Namespace Sequences
        Namespace SequenceManipulation
            Public Class Properties
                Public Shared Function Get_PolyA_Length(Type As Szunyi.Constants.ReadType_ByPolyA, Read As Bio.ISequence) As Integer
                    Dim Text = Szunyi.Sequences.SequenceManipulation.SeqsToString.GetSeqAsString(Read)
                    Select Case Type
                        Case Szunyi.Constants.ReadType_ByPolyA.bAT
                            Return -1
                        Case Szunyi.Constants.ReadType_ByPolyA.M
                            Return -1
                        Case Constants.ReadType_ByPolyA.A
                            Dim longestRun As New String(Text.[Select](Function(c, index) Text.Substring(index).TakeWhile(Function(e) e = c)).OrderByDescending(Function(e) e.Count()).First().ToArray())
                            Return longestRun.Count
                        Case Constants.ReadType_ByPolyA.T
                            Dim longestRun As New String(Text.[Select](Function(c, index) Text.Substring(index).TakeWhile(Function(e) e = c)).OrderByDescending(Function(e) e.Count()).First().ToArray())
                            Return longestRun.Count
                    End Select
                    Return -1
                End Function
                Public Shared Function Get_PolyA_Length(Type As Szunyi.Constants.ReadType_ByPolyA, Read As String) As Integer
                    Dim Text = Read
                    Select Case Type
                        Case Szunyi.Constants.ReadType_ByPolyA.bAT
                            Return -1
                        Case Szunyi.Constants.ReadType_ByPolyA.M
                            Return -1
                        Case Constants.ReadType_ByPolyA.A
                            Dim longestRun As New String(Text.[Select](Function(c, index) Text.Substring(index).TakeWhile(Function(e) e = c)).OrderByDescending(Function(e) e.Count()).First().ToArray())
                            Return longestRun.Count
                        Case Constants.ReadType_ByPolyA.T
                            Dim longestRun As New String(Text.[Select](Function(c, index) Text.Substring(index).TakeWhile(Function(e) e = c)).OrderByDescending(Function(e) e.Count()).First().ToArray())
                            Return longestRun.Count
                    End Select
                    Return -1
                End Function
                Public Shared Function Get_Not_First(Seq As Bio.ISequence, b As Byte) As Integer
                    For i1 = 0 To Seq.Count - 1
                        If Seq(i1) <> b Then Return i1
                    Next
                    Return Seq.Count - 1
                End Function

#Region "Positive Negative"
                ''' <summary>
                ''' Return + if pass the criteria else -
                ''' </summary>
                ''' <param name="Seqs"></param>
                ''' <param name="Min_Length"></param>
                ''' <param name="Max_Length"></param>
                ''' <returns></returns>
                Public Shared Function Get_Pos_Neg_ByLengths(Seqs As List(Of Bio.ISequence),
                                                         Min_Length As Integer,
                                                         Max_Length As Integer) As List(Of String)
                    Dim out As New List(Of String)
                    For Each Seq In Seqs
                        out.Add(Get_Pos_Neg_ByLength(Seq, Min_Length, Max_Length))
                    Next
                    Return out
                End Function
                Public Shared Function Get_Pos_Neg_ByLength(Seq As Bio.ISequence,
                                                         Min_Length As Integer,
                                                         Max_Length As Integer) As String
                    If Seq.Count >= Min_Length AndAlso Seq.Count <= Max_Length Then
                        Return "+"
                                                                     Else
                        Return "-"
                    End If
                End Function

                Public Shared Function Get_Pos_Neg_ByAACounts(Seqs As List(Of Bio.ISequence),
                                                              c As Char,
                                                              Min_Count As Integer,
                                                              Max_Count As Integer) As List(Of String)

                    Dim B As Byte = AscW(c)
                    Dim Out As New List(Of String)
                    For Each Seq In Seqs
                        Out.Add(Get_Pos_Neg_ByAACount(Seq, B, Min_Count, Max_Count))
                    Next
                    Return Out
                End Function
                Public Shared Function Get_Pos_Neg_ByAACount(Seq As Bio.ISequence,
                                                              B As Byte, Min_Count As Integer, Max_Count As Integer) As String
                    Dim t = (From x In Seq.ToArray Where x = B).Count
                    If t >= Min_Count AndAlso t <= Max_Count Then
                        Return "+"
                    Else
                        Return "-"
                    End If
                End Function


#End Region
                ''' <summary>
                ''' Return the Lengths Of Seq or Empty List of String
                ''' </summary>
                ''' <param name="Seqs"></param>
                ''' <returns></returns>
                Public Shared Function GetLengths(Seqs As List(Of Bio.ISequence)) As List(Of Long)

                    If IsNothing(Seqs) = True Then Return New List(Of Long)
                    Dim out As New List(Of Long)
                    For Each Seq In Seqs
                        out.Add(GetLength(Seq))
                    Next
                    Return out
                End Function
                Public Shared Function GetLength(Seq As Bio.ISequence) As Long

                    Return Seq.Count
                End Function
                ''' <summary>
                ''' Return IDs or Empty List Of String
                ''' </summary>
                ''' <param name="Seqs"></param>
                ''' <returns></returns>
                Public Shared Function GetIDs(Seqs As List(Of Bio.ISequence)) As List(Of String)

                    If IsNothing(Seqs) = True Then Return New List(Of String)
                    Dim res = From x In Seqs Select x.ID
                    If IsNothing(res) = True Then Return New List(Of String)
                    Return res.ToList
                End Function
                ''' <summary>
                ''' Return ShortIDs or Empty List of String
                ''' </summary>
                ''' <param name="Seqs"></param>
                ''' <returns></returns>
                Public Shared Function GetShortIDs(Seqs As List(Of Bio.ISequence)) As List(Of String)
                    If IsNothing(Seqs) = True Then Return New List(Of String)
                    Dim res = From x In Seqs Select Split(x.ID, ".").First
                    If IsNothing(res) = True Then Return New List(Of String)
                    Return res.ToList

                End Function

                Public Shared Function GetMWs(seqs As List(Of Bio.ISequence)) As List(Of Double)
                    Dim x As New Szunyi.Protein.MW
                    Dim MWs As New List(Of Double)
                    For Each Seq In seqs
                        MWs.Add(x.GetMW(Seq))
                    Next
                    Return MWs
                End Function

                Public Shared Function GetIPs(seqs As List(Of Bio.ISequence)) As List(Of Double)
                    Dim x As New Szunyi.Protein.PI
                    Dim IPs As New List(Of Double)
                    For Each Seq In seqs
                        IPs.Add(x.Get_PI(Seq))
                    Next
                    Return IPs
                End Function

                Public Shared Function GetSeqs(Seqs As List(Of Bio.ISequence)) As List(Of String)
                    Dim Out As New List(Of String)

                    For Each Seq In Seqs
                        Out.Add(Szunyi.Sequences.SequenceManipulation.SeqsToString.GetSeqAsString(Seq))
                    Next
                    Return Out
                End Function

                Public Shared Function Get_Pos_Neg_By_Motifs(Seqs As List(Of Bio.ISequence), motifs As List(Of ProteinMotif)) As List(Of String)
                    Dim out As New List(Of String)
                    For Each Seq In Seqs
                        Dim sSeq = Szunyi.Sequences.SequenceManipulation.SeqsToString.GetSeqAsString(Seq)
                        Dim Has_Motif As Boolean = False
                        For Each Motif In motifs
                            Dim TheMatch As Match = Regex.Match(sSeq, Motif.Pattern, RegexOptions.IgnoreCase)
                            If TheMatch.Success = True Then
                                Has_Motif = True
                                Exit For
                            End If
                        Next
                        If Has_Motif = True Then
                            out.Add("+")
                        Else
                            out.Add("-")
                        End If
                    Next
                    Return out
                End Function
                Public Shared Function Get_Pos_Neg_By_Motif(Seqs As List(Of Sequence), motif As ProteinMotif) As List(Of String)
                    Dim out As New List(Of String)
                    For Each Seq In Seqs

                        Dim sSeq = Seq.ConvertToString(0, Seq.Count)
                        Dim TheMatch As Match = Regex.Match(sSeq, motif.Pattern, RegexOptions.IgnoreCase)
                        If TheMatch.Success = True Then
                            out.Add("+")
                        Else
                            out.Add("-")
                        End If
                    Next

                    Return out


                End Function
            End Class
        End Namespace
    End Namespace
End Namespace