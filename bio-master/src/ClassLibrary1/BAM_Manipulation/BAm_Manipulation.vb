Imports System.IO
Imports System.Text
Imports System.Text.RegularExpressions
Imports Bio
Imports Bio.IO.GenBank
Imports Bio.IO.SAM
Imports ClassLibrary1.Szunyi.Constants
Imports ClassLibrary1.Szunyi.PacBio
Imports ClassLibrary1.Szunyi.BAM.CIGAR
Imports ClassLibrary1.Szunyi.BAM

Namespace Szunyi
    Namespace Alignment
        Namespace BAM_SAM

            Public Class Sam_Manipulation
                Public Shared Function Get_Read_Position(Sam As Bio.IO.SAM.SAMAlignedSequence) As String

                    Dim st = Get_Five_Soft_Clip(Sam)
                    Dim endy = -Get_Three_Soft_Clip(Sam) + Sam.QuerySequence.Count
                    Return st & vbTab & endy
                End Function

                Public Shared Function Get_First_Match(Exon As List(Of KeyValuePair(Of String, Integer))) As KeyValuePair(Of String, Integer)
                    Dim AllM = From x In Exon Where x.Key = "M"
                    If AllM.Count > 0 Then
                        Return AllM.First
                    Else
                        Return Nothing
                    End If

                End Function
                Public Shared Function Get_Last_Match(Exon As List(Of MdCigar)) As MdCigar
                    Dim AllM = From x In Exon Where x.Type = "M"

                    If AllM.Count > 0 Then
                        Return AllM.Last
                    Else
                        Return Nothing
                    End If
                End Function
                Public Shared Function Get_Last_Match(Exon As List(Of KeyValuePair(Of String, Integer))) As KeyValuePair(Of String, Integer)
                    Dim AllM = From x In Exon Where x.Key = "M"

                    If AllM.Count > 0 Then
                        Return AllM.Last
                    Else
                        Return Nothing
                    End If
                End Function



                Public Class Aggregate_Cytosine_Methylation
                    Public Property Files As List(Of FileInfo)
                    Public Property Seqs As List(Of Bio.ISequence)
                    Dim X As New Dictionary(Of String, SeqForMethylation)
                    Public Property NofUnknow As Long
                    Public Sub New(Files As List(Of FileInfo), Seqs As List(Of Bio.ISequence))
                        Me.Files = Files
                        Me.Seqs = Seqs
                        '  Me.Seqs.Sort(Szunyi.Comparares.AllComparares.BySeqID)
                    End Sub
                    Private Function GetX() As Dictionary(Of String, SeqForMethylation)
                        X.Clear()
                        For Each Seq In Seqs
                            X.Add(Seq.ID, New SeqForMethylation(Seq))
                        Next
                        Return X
                    End Function

                    Public Function GetNewX() As Dictionary(Of String, SeqForMethylation)
                        Dim X1 As New Dictionary(Of String, SeqForMethylation)
                        For Each Seq In Seqs
                            X1.Add(Seq.ID, New SeqForMethylation(Seq))
                        Next
                        Return X1
                    End Function
                    Private Function Get_NA_Bases(MoreLines As List(Of String)) As List(Of NaBase)
                        Dim NA_Bases As New List(Of NaBase)
                        For Each Line In MoreLines
                            If Line.StartsWith("@") = False Then
                                Dim s1() = Split(Line, vbTab)

                                Dim Flags = Bio.IO.SAM.SAMAlignedSequenceHeader.GetFlag(s1(1))
                                If Flags.HasFlag(Flags.QueryOnReverseStrand) And Flags.HasFlag(Flags.FirstReadInPair) Then
                                    ' ga
                                    NA_Bases.AddRange(Get_NaBase_For_Methylation_Negative_Strand(s1))
                                ElseIf Flags.HasFlag(Flags.MateOnReverseStrand) And Flags.HasFlag(Flags.SecondReadInPair) Then
                                    ' ga
                                    NA_Bases.AddRange(Get_NaBase_For_Methylation_Negative_Strand(s1))
                                ElseIf Flags.HasFlag(Flags.MateOnReverseStrand) And Flags.HasFlag(Flags.FirstReadInPair) Then
                                    'ct
                                    NA_Bases.AddRange(Get_NaBase_For_Methylation_Positive_Strand(s1))
                                ElseIf Flags.HasFlag(Flags.QueryOnReverseStrand) And Flags.HasFlag(Flags.SecondReadInPair) Then
                                    'ct
                                    NA_Bases.AddRange(Get_NaBase_For_Methylation_Positive_Strand(s1))
                                Else
                                    Dim alf As Int16 = 65
                                End If


                            End If
                        Next
                        Return NA_Bases
                    End Function
                    Private Function Merge_NA_Bases(Na_Bases As List(Of NaBase)) As Dictionary(Of String, SeqForMethylation)
                        Dim y = GetNewX()
                        For Each NA In Na_Bases
                            Dim seq = y(NA.SeqID)
                            If seq.Loci.ContainsKey(NA.Position) = False Then
                                seq.Loci.Add(NA.Position, NA)
                            Else
                                seq.Loci(NA.Position).NofC += NA.NofC
                                seq.Loci(NA.Position).NofG += NA.NofG
                                seq.Loci(NA.Position).NofCT += NA.NofCT
                                seq.Loci(NA.Position).NofGA += NA.NofGA
                            End If
                        Next
                        Return y
                    End Function
                    Public Sub DoIt()

                        For Each File In Me.Files
                            Dim MoreLines = Szunyi.IO.Import.Text.ParseMoreLines(File, 1000000)
                            Dim Merged_Subs As New List(Of Dictionary(Of String, SeqForMethylation))

                            X = GetX()

                            Parallel.ForEach(MoreLines, Sub(MoreLine)
                                                            Dim NA_Bases = Get_NA_Bases(MoreLine)
                                                            Dim Merged_Na_Bases = Merge_NA_Bases(NA_Bases)
                                                            Merged_Subs.Add(Merged_Na_Bases)
                                                        End Sub)

                            For Each Seq In Me.Seqs
                                Dim ByChr = From x In Merged_Subs Where x(Seq.ID).Loci.Count > 0 Select x(Seq.ID)


                                If ByChr.Count > 0 Then
                                    Dim ByStart = From x In ByChr Order By x.Loci.First.Value.Position Ascending

                                    For Each Item In ByStart

                                        For Each d In Item.Loci
                                            If X(d.Value.SeqID).Loci.ContainsKey(d.Value.Position) = False Then
                                                X(d.Value.SeqID).Loci.Add(d.Value.Position, d.Value)
                                            Else
                                                X(d.Value.SeqID).Loci(d.Value.Position).NofC += d.Value.NofC
                                                X(d.Value.SeqID).Loci(d.Value.Position).NofCT += d.Value.NofCT
                                                X(d.Value.SeqID).Loci(d.Value.Position).NofG += d.Value.NofG
                                                X(d.Value.SeqID).Loci(d.Value.Position).NofGA += d.Value.NofGA
                                            End If
                                        Next
                                    Next

                                End If
                            Next

                            SaveIndependent(File)
                        Next

                    End Sub
                    Private Function Get_NaBase_For_Methylation_Positive_Strand(s1() As String) As List(Of NaBase)
                        Dim out As New List(Of NaBase)
                        For i1 = 0 To s1(9).Length - 1
                            Dim Pos As Long = i1 + CInt(s1(3)) - 1
                            If X(s1(2)).Seq(Pos) = 67 Then ' "C" Then
                                Dim t As New NaBase()
                                t.SeqID = s1(2)
                                t.Position = i1 + CInt(s1(3))
                                Select Case s1(9).Substring(i1, 1)
                                    Case "C"
                                        t.NofC += 1
                                    Case "T"
                                        t.NofCT += 1
                                End Select
                                out.Add(t)
                            End If
                        Next
                        Return out
                    End Function
                    Private Function Get_NaBase_For_Methylation_Unknown_Strand(s1() As String) As List(Of NaBase)
                        Dim out As New List(Of NaBase)
                        For i1 = 0 To s1(9).Length - 1
                            Dim Pos As Long = i1 + CInt(s1(3)) - 1
                            If X(s1(2)).Seq(Pos) = 67 Then ' "C" Then
                                Dim t As New NaBase()
                                t.SeqID = s1(2)
                                t.Position = i1 + CInt(s1(3))
                                Select Case s1(9).Substring(i1, 1)
                                    Case "C"
                                        t.NofC += 1
                                    Case "G"
                                        t.NofG += 1
                                End Select
                                out.Add(t)
                            End If
                        Next
                        Return out
                    End Function


                    Private Function Get_NaBase_For_Methylation_Negative_Strand(s1() As String) As List(Of NaBase)
                        Dim out As New List(Of NaBase)
                        For i1 = 0 To s1(9).Length - 1
                            Dim Pos As Long = i1 + CInt(s1(3)) - 1
                            If X(s1(2)).Seq(Pos) = 71 Then ' G
                                Dim t As New NaBase()
                                t.SeqID = s1(2)
                                t.Position = i1 + CInt(s1(3))
                                Select Case s1(9).Substring(i1, 1)
                                    Case "G"
                                        t.NofG += 1
                                    Case "A"
                                        t.NofGA += 1
                                End Select
                                out.Add(t)
                            End If
                        Next
                        Return out
                    End Function
                    Private Function get_Strand_From_CT_or_GA(s1() As String) As String
                        Dim t As New NaBase()
                        For i1 = 0 To s1(9).Length - 1
                            Dim Pos As Long = i1 + CInt(s1(3)) - 1

                            If X(s1(2)).Seq(Pos) = 67 Then ' "C" Then
                                '     If X(s1(2)).Loci.ContainsKey(Pos) = False Then X(s1(2)).Loci.Add(Pos, New NaBase)
                                Select Case s1(9).Substring(i1, 1)
                                    Case "C"
                                        t.NofC += 1
                                    Case "T"
                                        t.NofCT += 1
                                End Select
                            ElseIf X(s1(2)).Seq(Pos) = 71 Then ' "G" Then
                                '    If X(s1(2)).Loci.ContainsKey(Pos) = False Then X(s1(2)).Loci.Add(Pos, New NaBase)
                                Select Case s1(9).Substring(i1, 1)
                                    Case "G"
                                        t.NofG += 1
                                    Case "A"
                                        t.NofGA += 1
                                End Select
                            End If
                        Next
                        If t.NofGA > t.NofCT Then
                            Return "-"
                        ElseIf t.NofCT > t.NofGA Then
                            Return "+"
                        Else
                            Return ""
                        End If
                    End Function
                    Private Function GetIndependent(SeqID As String, l As KeyValuePair(Of Long, NaBase)) As String
                        Dim str As New System.Text.StringBuilder
                        str.Append(SeqID).Append(vbTab)
                        str.Append(l.Key - 1).Append(vbTab).Append(l.Key - 1).Append(vbTab).Append(vbTab)
                        str.Append(l.Value.NofCT + l.Value.NofGA).Append(vbTab)
                        str.Append(l.Value.NofG + l.Value.NofC).Append(vbTab)
                        str.Append(l.Value.NofGA + l.Value.NofCT + l.Value.NofG + l.Value.NofC)
                        Return str.ToString
                    End Function
                    Private Sub SaveIndependent(File As FileInfo)
                        Dim NewFIle = Szunyi.IO.Files.Get_New_FileName.Append_Before_Extension_wNew_Extension(File, "WoStrandSeparation.tab")
                        Using sw As New StreamWriter(NewFIle.FullName)
                            sw.Write(GetHeader)
                            For Each Seq In X
                                For Each l In Seq.Value.Loci
                                    sw.WriteLine()
                                    sw.Write(GetIndependent(Seq.Key, l))
                                Next
                            Next
                        End Using
                        NewFIle = Szunyi.IO.Files.Get_New_FileName.Append_Before_Extension_wNew_Extension(File, "With_Strans_Separations.tab")
                        Using sw As New StreamWriter(NewFIle.FullName)
                            sw.Write(GetHeader)
                            For Each Seq In X
                                For Each l In Seq.Value.Loci
                                    sw.WriteLine()
                                    sw.Write(GetStranddependent(Seq.Key, l))
                                Next
                            Next
                        End Using


                    End Sub

                    Private Function GetStranddependent(SeqID As String, l As KeyValuePair(Of Long, NaBase)) As String
                        Dim str As New System.Text.StringBuilder
                        If l.Value.NofC + l.Value.NofCT > l.Value.NofG + l.Value.NofGA Then
                            str.Append(SeqID).Append(vbTab)
                            str.Append(l.Key - 1).Append(vbTab).Append(l.Key - 1).Append(vbTab)
                            str.Append("+").Append(vbTab)
                            str.Append(l.Value.NofC).Append(vbTab)
                            str.Append(l.Value.NofCT).Append(vbTab)
                            str.Append(l.Value.NofCT + l.Value.NofC).AppendLine()
                        Else
                            str.Append(SeqID).Append(vbTab)
                            str.Append(l.Key - 1).Append(vbTab).Append(l.Key - 1).Append(vbTab)
                            str.Append("-").Append(vbTab)
                            str.Append(l.Value.NofG).Append(vbTab)
                            str.Append(l.Value.NofGA).Append(vbTab)
                            str.Append(l.Value.NofG + l.Value.NofGA).AppendLine()

                        End If
                        If str.Length > 0 Then str.Length -= 2


                        Return str.ToString
                    End Function

                    Private Function GetHeader()
                        Dim str As New System.Text.StringBuilder
                        str.Append("SeqID").Append(vbTab)
                        str.Append("Start").Append(vbTab)
                        str.Append("End").Append(vbTab)
                        str.Append("Strand").Append(vbTab)
                        str.Append("Interesting").Append(vbTab)
                        str.Append("NotInteresting").Append(vbTab)
                        str.Append("Total")
                        Return str.ToString
                    End Function
                    Private Sub Save(Original_File As FileInfo)
                        Dim str As New System.Text.StringBuilder
                        str.Append("SeqID").Append(vbTab)
                        str.Append("Location").Append(vbTab)
                        str.Append("C").Append(vbTab)
                        str.Append("CT").Append(vbTab)
                        str.Append("G").Append(vbTab)
                        str.Append("GA").AppendLine()

                        For Each Seq In X
                            For Each l In Seq.Value.Loci
                                str.Append(Seq.Key).Append(vbTab)
                                str.Append(l.Key).Append(vbTab)
                                str.Append(l.Value.NofC).Append(vbTab)
                                str.Append(l.Value.NofCT).Append(vbTab)
                                str.Append(l.Value.NofG).Append(vbTab)
                                str.Append(l.Value.NofGA).AppendLine()
                            Next
                        Next
                        Dim NewFIle = Szunyi.IO.Files.Get_New_FileName.Append_Before_Extension_wNew_Extension(Original_File, ".tab")
                        Szunyi.IO.Export.SaveText(str.ToString, NewFIle)
                    End Sub
                End Class

                Public Class NaBase
                    Public Position As Integer
                    '  Public ID As String
                    Public SeqID As String
                    Public NofC As Integer
                    Public NofG As Integer
                    Public NofCT As Integer
                    Public NofGA As Integer
                End Class

                Public Class SeqForMethylation

                    Public ID As String
                    Public Seq As Bio.Sequence
                    Public Loci As New SortedList(Of Long, NaBase)


                    Sub New(Seq As Bio.Sequence)
                        ' TODO: Complete member initialization 
                        Me.Seq = Seq
                        Me.ID = Seq.ID

                    End Sub


                End Class


                Public Shared Function Get_Nof_Diff_NM(Sams As List(Of SAMAlignedSequence)) As Object
                    Dim res As New List(Of Integer)
                    For Each SAM In Sams
                        Dim kj = SAM.OptionalFields

                        Dim NM = Get_Optional_Field(SAM, "NM")
                        res.Add(NM.Value)

                        Dim h As Integer = 43
                    Next
                    Return res.Distinct.Count
                End Function
                Public Shared Function Get_Optional_Field(sam As SAMAlignedSequence, OptionalField As String) As Bio.IO.SAM.SAMOptionalField

                    Dim x = From t In sam.OptionalFields Where t.Tag = OptionalField

                    If x.Count > 0 Then
                        Return x.First

                    Else
                        Return Nothing
                    End If

                End Function
            End Class

            Public Class Filter

                Public Shared Iterator Function Get_Duplicated_Reads(all_Sams As List(Of SAMAlignedSequence)) As IEnumerable(Of List(Of SAMAlignedSequence))
                    Dim out = From x In all_Sams Group By x.QName Into Group

                    For Each g In out
                        If g.Group.Count > 1 Then
                            Yield g.Group.ToList
                        End If
                    Next
                End Function

                Public Shared Function Get_Intronic(Sam As SAMAlignedSequence, min_Intron_Length As Integer, Min_Match As Integer) As SAMAlignedSequence
                    Dim MdCIg = Get_CIGARS(Sam)
                    Dim First_Match = Sam_Manipulation.Get_First_Match(MdCIg)
                    Dim Last_Match = Sam_Manipulation.Get_Last_Match(MdCIg)
                    Dim Biggest_Intron_Length = Szunyi.BAM.CIGAR.Get_Biggest_Intron_Length(MdCIg)
                    If Biggest_Intron_Length >= min_Intron_Length AndAlso First_Match.Value >= Min_Match AndAlso Last_Match.Value >= Min_Match Then
                        Return Sam
                    Else
                        Return Nothing
                    End If
                End Function

                Public Shared Function Get_Not_Self(d As List(Of SAMAlignedSequence)) As List(Of SAMAlignedSequence)
                    Dim bads As New List(Of SAMAlignedSequence)
                    For i1 = 0 To d.Count - 2
                        For i2 = i1 + 1 To d.Count - 1
                            If d(i2).Pos >= d(i1).Pos And d(i2).Pos <= d(i1).RefEndPos Or d(i1).Pos >= d(i2).Pos And d(i1).Pos <= d(i2).RefEndPos Then
                                bads.Add(d(i2))
                            End If
                        Next
                    Next
                    If bads.Count > 0 Then
                        Return New List(Of SAMAlignedSequence)
                    Else
                        Return d
                    End If
                End Function

                Public Shared Function Get_NotFully_Covered_Reads(d As List(Of SAMAlignedSequence), rep_Regions As List(Of Bio.IO.GenBank.Location)) As List(Of SAMAlignedSequence)
                    Dim Covered As New List(Of SAMAlignedSequence)
                    For Each d1 In d
                        For Each rep_region In rep_Regions
                            If d1.Pos >= rep_region.LocationStart AndAlso d1.RefEndPos <= rep_region.LocationEnd Then
                                Covered.Add(d1)
                            End If
                        Next
                    Next
                    Dim res = d.Except(Covered)
                    If res.Count > 0 Then
                        Return d
                    Else
                        Return New List(Of SAMAlignedSequence)
                    End If
                End Function



            End Class
        End Namespace
    End Namespace
End Namespace


