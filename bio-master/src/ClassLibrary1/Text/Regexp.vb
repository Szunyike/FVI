

Imports System.Text.RegularExpressions
Imports Bio

Namespace Szunyi
    Namespace Text
        Public Class Regexp
            Public Shared Function GetDistinctStringsbyRegexp(ToSearch As String, Motif As String) As List(Of String)
                Dim TheMatches = Regex.Matches(ToSearch, Motif, RegexOptions.IgnoreCase)
                Dim TheMatch = Regex.Match(ToSearch, Motif, RegexOptions.IgnoreCase)
                Dim tmp As New List(Of String)
                For Each match As Match In TheMatches
                    tmp.Add(match.Value)

                Next
                Dim sg = tmp.Distinct
                tmp = sg.ToList

                Return tmp
            End Function
            ''' <summary>
            ''' Return List of Strings or Empty List Of Strings
            ''' </summary>
            ''' <param name="key"></param>
            ''' <param name="startSeparator"></param>
            ''' <param name="endSeparator"></param>
            ''' <returns></returns>
            Public Shared Function GetDistinctStrings(key As String, startSeparator As String, endSeparator As String) As List(Of String)
                Dim out As New List(Of String)
                Dim StartIndex As Integer = 0
                Do
                    StartIndex = key.IndexOf(startSeparator, StartIndex)
                    If StartIndex < 0 Then Exit Do
                    Dim EndIndex = key.IndexOf(endSeparator, StartIndex + 1)
                    Dim StartIndex2 = key.IndexOf(startSeparator, StartIndex + 1)
                    If StartIndex2 <> -1 AndAlso StartIndex2 < EndIndex Then
                        StartIndex = StartIndex2
                    Else
                        If EndIndex < 0 Then Exit Do
                        out.Add(key.Substring(StartIndex + 1, EndIndex - StartIndex - 1))
                        StartIndex = EndIndex + 1
                    End If

                Loop
                If out.Count = 0 Then Return New List(Of String)
                Return out.Distinct.ToList
            End Function
            ''' <summary>
            ''' Smaller And Bigger
            ''' </summary>
            ''' <param name="haystack"></param>
            ''' <param name="needle"></param>
            ''' <returns></returns>
            Public Shared Function SimpleBoyerMooreSearch(haystack As Byte(), needle As Byte()) As Integer
                Dim lookup As Integer() = New Integer(255) {}
                For i As Integer = 0 To lookup.Length - 1
                    lookup(i) = needle.Length
                Next

                For i As Integer = 0 To needle.Length - 1
                    lookup(needle(i)) = needle.Length - i - 1
                Next

                Dim index As Integer = needle.Length - 1
                Dim lastByte = needle.Last()
                While index < haystack.Length
                    Dim checkByte = haystack(index)
                    If haystack(index) = lastByte Then
                        Dim found As Boolean = True
                        For j As Integer = needle.Length - 2 To 0 Step -1
                            If haystack(index - needle.Length + j + 1) <> needle(j) Then
                                found = False
                                Exit For
                            End If
                        Next

                        If found Then
                            Return index - needle.Length + 1
                        Else
                            index += 1
                        End If
                    Else
                        index += lookup(checkByte)
                    End If
                End While
                Return -1
            End Function

            Public Shared Function HasMotifs(Seqs As List(Of Sequence), Motifs As List(Of String)) As List(Of Sequence)
                Dim out As New List(Of Bio.Sequence)
                For Each Seq In Seqs
                    Dim s = Seq.ConvertToString
                    For Each Motif In Motifs
                        If HasMotif(s, Motif) = True Then
                            out.Add(Seq)
                            Exit For
                        End If
                    Next
                Next
                Return out
            End Function

            Public Shared Function HasMotif(ToSearch As String, motif As String) As Boolean
                Dim TheMatches = Regex.Matches(ToSearch, motif, RegexOptions.IgnoreCase)
                If TheMatches.Count > 0 Then
                    Return True
                Else
                    Return False
                End If
            End Function
            Public Shared Function Find_Motifs(ToSearch As String, motif As String) As MatchCollection
                If ToSearch = String.Empty Then Return Nothing
                Dim TheMatches = Regex.Matches(ToSearch, motif, RegexOptions.IgnoreCase)
                If TheMatches.Count > 0 Then
                    Return TheMatches
                Else
                    Return Nothing
                End If
            End Function

            Public Shared Function Nof_Motifs(Seqs As List(Of Bio.ISequence), Motif As String) As List(Of String)
                Dim Out As New List(Of String)
                For Each Seq In Seqs
                    Out.Add(Nof_Motif(Seq, Motif))
                Next
                Return Out
            End Function

            Public Shared Function Nof_Motif(Seq As Bio.ISequence, Motif As String) As String
                Dim TheMatches = Regex.Matches(Szunyi.Sequences.SequenceManipulation.SeqsToString.GetSeqAsString(Seq), Motif, RegexOptions.IgnoreCase)
                If TheMatches.Count > 0 Then
                    Return TheMatches.Count
                Else
                    Return 0
                End If
            End Function
            Public Shared Function Get_Motifs(Seq As Bio.ISequence, Motif As String) As MatchCollection
                Dim TheMatches = Regex.Matches(Szunyi.Sequences.SequenceManipulation.SeqsToString.GetSeqAsString(Seq), Motif, RegexOptions.IgnoreCase)
                If TheMatches.Count > 0 Then
                    Return TheMatches
                Else
                    Return Nothing
                End If
            End Function
        End Class
    End Namespace
End Namespace

