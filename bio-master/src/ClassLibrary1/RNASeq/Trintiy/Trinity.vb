Imports System.IO

Namespace Szunyi
    Namespace RNASeq
        Public Class Trinity
            Public Sub New()

            End Sub
            Public Sub Get_And_ReName_Trinity_Fasta_Files()
                Dim Folder = Szunyi.IO.Directory.Get_Folder
                If IsNothing(Folder) = True Then Exit Sub
                Dim SubFolders = Folder.GetDirectories
                If IsNothing(SubFolders) = True Then Exit Sub
                Dim Rename As New Dictionary(Of String, String)
                Dim ReName_File = From x In Folder.GetFiles Where x.Name = "Trinity_ReName.txt"
                Dim ContigIDs As New System.Text.StringBuilder
                If ReName_File.Count = 1 Then
                    For Each Line In Szunyi.IO.Import.Text.ParseNotFirst(ReName_File.First, 1)
                        Dim s1() = Split(Line, vbTab)
                        Rename.Add(s1.Last, s1.First)
                    Next
                End If

                Dim Sel_Folders = From x In SubFolders Where x.Name.StartsWith("trinity")

                If Sel_Folders.Count = 0 Then Exit Sub

                For Each Sel_Folder In Sel_Folders
                    Dim Files = From x In Sel_Folder.GetFiles Where x.Name = "Trinity.fasta"

                    If Files.Count = 1 Then ' Founded
                        Dim Seqs = Szunyi.IO.Import.Sequence.FromFile(Files.First)
                        Dim OriginalIDs = Szunyi.Sequences.SequenceManipulation.Properties.GetIDs(Seqs)
                        Dim Key = Sel_Folder.Name.Replace("trinity_", "")

                        Dim ReNamed_Seqs = Szunyi.Sequences.SequenceManipulation.ID.Rename(Seqs, Szunyi.Constants.StringRename.AscendingWithPrefix, Rename(Key) & "_c")
                        Dim ReNamedIDs = Szunyi.Sequences.SequenceManipulation.Properties.GetIDs(ReNamed_Seqs)

                        ContigIDs.Append(Szunyi.Text.Lists.Merge(OriginalIDs, ReNamedIDs, vbTab, vbCrLf)).AppendLine()

                        Dim NewSeqFile As New FileInfo(Folder.FullName & "\" & Rename(Key) & ".fa")
                        Szunyi.IO.Export.SaveAsSimpleFasta(ReNamed_Seqs, NewSeqFile)


                    End If
                Next
                If ContigIDs.Length > 0 Then
                    ContigIDs.Length -= 2
                    Dim ID_File As New FileInfo(Folder.FullName & "\ContigIDs.tdt")
                    Szunyi.IO.Export.SaveText(ContigIDs.ToString, ID_File)
                End If
            End Sub
        End Class

    End Namespace
End Namespace
