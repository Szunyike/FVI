Imports System.IO
Namespace Szunyi
    Namespace PacBio
        Public Class Pacbio_Transcript_Discovery_Settings
            Public Property Settings As New List(Of Pacbio_Transcript_Discovery_Setting)
            Public Sub New(files As List(Of FileInfo))
                For Each File In files
                    Settings.Add(New Pacbio_Transcript_Discovery_Setting(File))
                Next
            End Sub
        End Class
        Public Class Pacbio_Transcript_Discovery_Setting
            Public Property Name As String
            Public Property minNOf_polyA As Integer
            Public Property Intron_Min_count As Integer
            Public Property Intron_Width As Integer
            Public Property Intron_Min_Group_Count As Integer
            Public Property Intron_Groups As Integer()
            Public Property PolyA_Min_count As Integer
            Public Property PolyA_Width As Integer
            Public Property PolyA_Min_Group_Count As Integer
            Public Property PolyA_Groups As Integer()
            Public Property TSS_Min_count As Integer
            Public Property TSS_Width As Integer
            Public Property TSS_Min_Group_Count As Integer
            Public Property TSS_Groups As Integer()
            Public Property Transcript_Min_count As Integer
            Public Property Transcript_Width As Integer
            Public Property Transcript_Min_Group_Count As Integer
            Public Property Transcript_Groups As Integer()
            Public Property Min_Intron_length As Integer = -1
            Public Property Max_Intron_length As Integer = -1
            Public Property Min_Exon_Length As Integer = -1 '	40
            Public Property Max_Trimmed_End As Integer = -1 '80
            Public Property Max_Nof_Insertion As Integer = -1  '60
            Public Property Max_Nof_Mismatch As Integer = -1  '60
            Public Property Max_percent_Of_MisMatch_Insertion_Deletion_ToRead As Integer = -1  '' 5
            Public Property Merged_Reads_Groups As Integer()
            Public Property Write_TR As Boolean
            Public Property Write_Reads As Boolean
            Public Property Write_Intorns As Boolean
            Public Property Write_PAS As Boolean
            Public Property Write_TSS As Boolean
            Public Property Write_Merged_Reads As Boolean
            Public Property Write_IP As Boolean
            Public Property Write_Mature_Peptide As Boolean

            Public Sub New(FIle As FileInfo)
                Dim lines = Szunyi.IO.Import.Text.ReadLines(FIle)
                Me.Name = lines.First
                Me.minNOf_polyA = lines(1).Split(vbTab).Last

                Dim s = Split(lines(3), vbTab)
                Me.Intron_Min_count = s(1)
                Me.Intron_Width = s(2)
                Me.Intron_Min_Group_Count = s(3)
                Me.Intron_Groups = Szunyi.Text.General.ToInteger(Split(s(4), ","))

                s = Split(lines(4), vbTab)
                Me.PolyA_Min_count = s(1)
                Me.PolyA_Width = s(2)
                Me.PolyA_Min_Group_Count = s(3)
                Me.PolyA_Groups = Szunyi.Text.General.ToInteger(Split(s(4), ","))

                s = Split(lines(5), vbTab)
                Me.TSS_Min_count = s(1)
                Me.TSS_Width = s(2)
                Me.TSS_Min_Group_Count = s(3)
                Me.TSS_Groups = Szunyi.Text.General.ToInteger(Split(s(4), ","))

                s = Split(lines(6), vbTab)
                Me.Transcript_Min_count = s(1)
                Me.Transcript_Width = s(2)
                Me.Transcript_Min_Group_Count = s(3)
                Me.Transcript_Groups = Szunyi.Text.General.ToInteger(Split(s(4), ","))

                s = Split(lines(7), vbTab)
                If s.Count = 3 Then
                    Me.Min_Intron_length = s(1)
                    Me.Max_Intron_length = s.Last

                    s = Split(lines(8), vbTab)
                    Me.Min_Exon_Length = s.Last

                    s = Split(lines(9), vbTab)
                    Me.Max_Trimmed_End = s.Last

                    s = Split(lines(10), vbTab)
                    Me.Max_Nof_Insertion = s.Last

                    s = Split(lines(11), vbTab)
                    Me.Max_Nof_Mismatch = s.Last

                    s = Split(lines(12), vbTab)
                    Me.Max_percent_Of_MisMatch_Insertion_Deletion_ToRead = s.Last
                End If
                s = Split(lines(13), vbTab)
                Merged_Reads_Groups = Szunyi.Text.General.ToInteger(Split(s(1), ","))
                Write_TR = Split(lines(14), vbTab).Last
                Write_Reads = Split(lines(15), vbTab).Last
                Write_Intorns = Split(lines(16), vbTab).Last
                Write_PAS = Split(lines(17), vbTab).Last
                Write_TSS = Split(lines(18), vbTab).Last
                Write_Merged_Reads = Split(lines(19), vbTab).Last
                Write_IP = Split(lines(20), vbTab).Last
                Write_Mature_Peptide = Split(lines(21), vbTab).Last
            End Sub
        End Class

    End Namespace
End Namespace