Imports System.IO

Namespace Szunyi
    Namespace Alignment
        Public Class Manipulation
            Public Shared Function ToString(AL As Bio.Algorithms.Alignment.PairwiseAlignedSequence) As String
                Dim str As New System.Text.StringBuilder
                str.Append(AL.Consensus.ConvertToString).Append(";")
                str.Append(AL.FirstSequence.ConvertToString).Append(";")
                str.Append(AL.SecondSequence.ConvertToString).Append(";")
                str.Append(AL.FirstOffset).Append(";")
                str.Append(AL.Score).Append(";")
                str.Append(AL.SecondOffset).Append(";")
                For Each M In AL.Metadata
                    str.Append(M.Key).Append(";")
                    Select Case M.Key
                        Case = "Insertions"
                            For Each s In M.Value
                                str.Append(s).Append(",")
                            Next
                            str.Length -= 1
                        Case "StartOffSets"
                            For Each s In M.Value
                                str.Append(s).Append(",")
                            Next
                            str.Length -= 1
                        Case = "EndOffSets"
                            For Each s In M.Value
                                str.Append(s).Append(",")
                            Next

                            str.Length -= 1
                        Case Else
                            str.Append(M.Value).Append(";")
                    End Select
                    str.Length -= 1

                Next

                Return str.ToString

            End Function

        End Class
        Public Class Meme
            Private main_Folder As DirectoryInfo
            Private seq_Lists As List(Of ListOf.SequenceList)
            Dim x As New Dictionary(Of String, List(Of String))

            Public Sub New(seq_Lists As List(Of ListOf.SequenceList), main_Folder As DirectoryInfo)
                Me.seq_Lists = seq_Lists
                Me.main_Folder = main_Folder
                Dim SubFolders = Me.main_Folder.GetDirectories
                For Each SubFolder In SubFolders
                    Dim FIles = SubFolder.GetFiles("meme.txt")
                    Dim Lines = Szunyi.IO.Import.Text.ReadLines(FIles.First)
                    For i1 = 0 To Lines.Count - 1
                        If Lines(i1).Contains("sites sorted by position p-value") Then
                            Dim s() = Split(Lines(i1), " ")
                            Dim IDs As New List(Of String)
                            For i2 = i1 + 4 To Lines.Count - 1
                                If Lines(i2).StartsWith("-") = True Then
                                    Exit For
                                Else
                                    IDs.Add(Split(Lines(i2), " ").First)
                                End If
                            Next
                            x.Add(SubFolder.Name & " Motif " & s(1), IDs)
                        End If
                    Next

                Next
                Dim str As New System.Text.StringBuilder
                str.Append(vbTab)
                For Each Seq_list In Me.seq_Lists
                    str.Append(Seq_list.ShortFileName).Append(vbTab)
                Next
                str.Length -= 1
                str.AppendLine()
                For Each Seq_list In Me.seq_Lists
                    str.Append(Seq_list.Sequences.Count).Append(vbTab)
                Next
                str.Length -= 1
                str.AppendLine()
                For Each Item In x
                    str.Append(Item.Key)
                    For Each Seq_list In Me.seq_Lists
                        Dim IDs = Szunyi.Sequences.SequenceManipulation.Properties.GetShortIDs(Seq_list.Sequences)
                        Dim Common = IDs.Intersect(Item.Value)
                        str.Append(vbTab).Append(Common.Count)
                    Next
                    str.AppendLine()
                Next
                Dim Motif = Get_Motif_Probability_Matixes()

            End Sub
            Public Function Get_Motif_Probability_Matixes()
                Dim SubFolders = Me.main_Folder.GetDirectories
                Dim str As New System.Text.StringBuilder
                str.Append("MEME version 4").AppendLine.AppendLine()
                str.Append("ALPHABET= ACGT").AppendLine.AppendLine()
                str.Append("strands: +").AppendLine()

                For Each SubFolder In SubFolders
                    Dim FIles = SubFolder.GetFiles("meme.txt")
                    Dim Lines = Szunyi.IO.Import.Text.ReadLines(FIles.First)
                    For i1 = 0 To Lines.Count - 1
                        If Lines(i1).Contains("Background letter frequencies (fr") Then
                            str.Append(Lines(i1)).AppendLine()
                            str.Append(Lines(i1 + 1)).AppendLine().AppendLine()

                        End If

                        If Lines(i1).Contains("letter-probability matrix") Then
                            Dim s() = Split(Lines(i1 - 2), " ")
                            str.AppendLine("MOTIF ").Append(SubFolder.Name & "_" & s(1)).AppendLine()
                            For i2 = i1 To Lines.Count - 1
                                If Lines(i2).Contains("--") Then
                                    Exit For
                                Else
                                    str.Append(Lines(i2)).AppendLine()
                                End If

                            Next
                            str.AppendLine()
                        End If
                    Next
                Next
                Return str.ToString
            End Function
        End Class

        Public Class Fimo
            Private main_Folder As DirectoryInfo
            Private seq_Lists As List(Of ListOf.SequenceList)
            Dim x As New Dictionary(Of String, List(Of String))
            Dim nof_iteration As Integer = 2

            Public Sub New(seq_Lists As List(Of ListOf.SequenceList), main_Folder As DirectoryInfo)
                Me.seq_Lists = seq_Lists
                Me.main_Folder = main_Folder

                Me.seq_Lists = seq_Lists
                Me.main_Folder = main_Folder
                Dim SubFolders = Me.main_Folder.GetDirectories
                For Each SubFolder In SubFolders
                    Dim FIles = SubFolder.GetFiles("fimo.txt")
                    Dim Lines = Szunyi.IO.Import.Text.ReadLines(FIles.First)
                    For i1 = 1 To Lines.Count - 1
                        Dim s() = Split(Lines(i1), vbTab)
                        Dim Name = SubFolder.Name & " " & s.First
                        If x.ContainsKey(Name) = False Then x.Add(Name, New List(Of String))
                        x(Name).Add(s(1))
                    Next
                Next
                Dim str As New System.Text.StringBuilder
                str.Append(vbTab)
                For Each Seq_list In Me.seq_Lists
                    str.Append(Seq_list.ShortFileName).Append(vbTab)
                Next
                str.Length -= 1
                str.AppendLine().Append(vbTab)
                For Each Seq_list In Me.seq_Lists
                    str.Append(Seq_list.Sequences.Count).Append(vbTab)
                Next
                str.Length -= 1
                str.AppendLine()
                For Each Item In x
                    str.Append(Item.Key)
                    For Each Seq_list In Me.seq_Lists
                        Dim IDs = Szunyi.Sequences.SequenceManipulation.Properties.GetShortIDs(Seq_list.Sequences)
                        Dim Common = IDs.Intersect(Item.Value)
                        str.Append(vbTab).Append(Common.Count)
                    Next
                    str.AppendLine()
                Next
                Dim Extended(Get_Extended)
            End Sub
            Public Function Get_Extended() As String
                Dim All_Motifs = (From h In x Select h).ToList
                Dim All_Motifs2 = From h In x Select h.Value
                Dim For_Test As New Dictionary(Of String, List(Of String))
                For i1 = 0 To All_Motifs.Count - 2
                    For i2 = i1 + 1 To All_Motifs.Count - 1
                        Dim Common = All_Motifs(i1).Value.Intersect(All_Motifs(i2).Value)
                        For_Test.Add(All_Motifs(i1).Key & " " & All_Motifs(i2).Key, Common.ToList)
                    Next
                Next
                For i1 = 0 To All_Motifs.Count - 3
                    For i2 = i1 + 1 To All_Motifs.Count - 2
                        For i3 = i2 + 1 To All_Motifs.Count - 1
                            Dim Common = All_Motifs(i1).Value.Intersect(All_Motifs(i2).Value).Intersect(All_Motifs(i3).Value)
                            For_Test.Add(All_Motifs(i1).Key & " " & All_Motifs(i2).Key & All_Motifs(i3).Key, Common.ToList)

                        Next

                    Next
                Next
                Dim str As New System.Text.StringBuilder
                str.Append(vbTab)
                For Each Seq_list In Me.seq_Lists
                    str.Append(Seq_list.ShortFileName).Append(vbTab)
                Next
                str.Length -= 1
                str.AppendLine().Append(vbTab)
                For Each Seq_list In Me.seq_Lists
                    str.Append(Seq_list.Sequences.Count).Append(vbTab)
                Next
                str.Length -= 1
                str.AppendLine()
                For Each Item In For_Test
                    str.Append(Item.Key)
                    For Each Seq_list In Me.seq_Lists
                        Dim IDs = Szunyi.Sequences.SequenceManipulation.Properties.GetShortIDs(Seq_list.Sequences)
                        Dim Common = IDs.Intersect(Item.Value)
                        str.Append(vbTab).Append(Common.Count)
                    Next
                    str.AppendLine()
                Next
                Return str.ToString
            End Function

        End Class
    End Namespace
End Namespace
