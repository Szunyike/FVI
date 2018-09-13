Imports HDF5DotNet
Imports System.IO
Imports Bio
Imports Bio.IO.GenBank
Imports ClassLibrary1.Szunyi.Constants
Imports System.Text
Imports ClassLibrary1.Szunyi.Location

Namespace Szunyi
    Namespace PacBio
        Public Class Methylation
            Public Property Real_CCS_Files As List(Of FileInfo)
            Public Property Real_SubRead_Files As List(Of FileInfo)

            Public Sub New()
                Dim Files =  Szunyi.IO.Files.Filter.SelectFiles("H5 Files", Szunyi.Constants.Files.HDF5)
                If IsNothing(Files) = True Then Exit Sub
                Dim CCS_Files = From x In Files Where x.Name.Contains("ccs")

                If CCS_Files.Count = 0 Then Exit Sub

                Dim SubRead_Files = From x In Files Where x.Name.Contains("bax")

                If SubRead_Files.Count = 0 Then Exit Sub

                Real_SubRead_Files = SubRead_Files.ToList
                Real_CCS_Files = CCS_Files.ToList
                Import_Data()
            End Sub
            Private Sub Import_Data()

            End Sub
        End Class
        Public Class Pacbio_Transcript_Shared

            Public Shared Function Merge_Locations(Locis As List(Of ILocation), width As Integer, Key As String, Optional minCOunt As Integer = 1) As List(Of FeatureItem)
                Dim l = Szunyi.Location.Merging.MergeLocations(Locis, width,  Sort_Locations_By.TSS_PAS, minCOunt)
                Dim out As New List(Of FeatureItem)
                For Each same In l
                    Dim f As New FeatureItem(Key, same.First)
                    Dim IdentifiedBys = (From x In same Select x.Accession).ToList
                    f.Qualifiers(StandardQualifierNames.IdentifiedBy) = IdentifiedBys
                    out.Add(f)
                Next
                Return out
            End Function

            Public Shared Function Get_Sites(Mappings As List(Of Szunyi.Location.Basic_Location), Seq As Bio.Sequence, Sort As Sort_Locations_By) As Dictionary(Of String, Integer())
                Dim t(Seq.Count) As Integer
                Dim t1(Seq.Count) As Integer
                Dim res As New Dictionary(Of String, Integer())
                res.Add("+", t)
                res.Add("-", t1)
                Select Case Sort
                    Case Sort_Locations_By.TSS
                        For Each m In Mappings
                            If m.Location.IsComplementer = False Then
                                res("+")(m.Location.TSS) += 1
                            Else
                                res("-")(m.Location.TSS) += 1
                            End If
                        Next
                    Case  Sort_Locations_By.PAS
                        For Each m In Mappings
                            If m.Location.IsComplementer = False Then
                                res("+")(m.Location.PAS) += 1
                            Else
                                res("-")(m.Location.PAS) += 1
                            End If
                        Next
                    Case Else
                        Dim kj As Int16 = 54

                End Select
                Dim All_Sites = Szunyi.PacBio.Pacbio_Transcript_Shared.TSSs_PASs_ToString(res)


                Return res

                Return Get_Sites(Mappings, Seq, Sort)


            End Function
            Public Shared Function Get_Windows(x As Dictionary(Of String, Integer()), WindowSizes As List(Of Integer)) As Dictionary(Of String, Dictionary(Of String, List(Of Integer)))
                Dim out As New Dictionary(Of String, Dictionary(Of String, List(Of Integer)))
                For Each w In WindowSizes
                    out.Add(w, New Dictionary(Of String, List(Of Integer)))
                    Dim pozValues As New List(Of Integer)
                    Dim negValues As New List(Of Integer)
                    Dim poz As Integer = 0
                    Dim neg As Integer = 0
                    For i1 = 0 To w - 1
                        poz += x("+")(i1)
                        neg += x("-")(i1)
                    Next
                    pozValues.Add(poz)
                    negValues.Add(neg)
                    For i1 = w To x.First.Value.Count - 1
                        poz = poz - x("+")(i1 - w) + x("+")(i1)
                        neg = neg - x("-")(i1 - w) + x("-")(i1)
                        pozValues.Add(poz)
                        negValues.Add(neg)
                    Next
                    out(w).Add("+", pozValues)
                    out(w).Add("-", negValues)
                Next
                Return out
            End Function

            Public Shared Function Get_Smoothed(x As Dictionary(Of String, Integer()), width As Integer) As Dictionary(Of String, Double())
                Dim res As New Dictionary(Of String, Double())
                Dim t = x("+")
                Dim t1(t.Count) As Double
                For i1 = 0 To t.Count - 1
                    Dim v As Integer = 0
                    For i2 = i1 To i1 + width
                        If i2 < t.Count Then
                            v += t(i2)
                        End If
                    Next
                    t1(i1) = v / width
                Next
                res.Add("+", t1)
                t = x("-")
                Dim t2(t.Count) As Double
                For i1 = 0 To t.Count - 1
                    Dim v As Integer = 0
                    For i2 = i1 To i1 + width
                        If i2 < t.Count Then
                            v += t(i2)
                        End If
                    Next
                    t2(i1) = v / width
                Next
                res.Add("-", t2)
                Return res
            End Function
            Public Shared Function TSSs_PASs_ToString(tSSs As Dictionary(Of String, Integer())) As String
                Dim str As New System.Text.StringBuilder
                str.Append("pos").Append(vbTab)
                str.Append("+").Append(vbTab)
                str.Append("-").AppendLine()
                For i1 = 0 To tSSs.First.Value.Count - 1
                    str.Append(i1).Append(vbTab)
                    str.Append(tSSs("+")(i1)).Append(vbTab)
                    str.Append(tSSs("-")(i1)).AppendLine()
                Next
                Return str.ToString
            End Function
            Public Shared Function TSSs_PASs_ToString(tSSs As Dictionary(Of String, Double())) As String
                Dim str As New System.Text.StringBuilder
                str.Append("pos").Append(vbTab)
                str.Append("+").Append(vbTab)
                str.Append("-").AppendLine()
                For i1 = 0 To tSSs.First.Value.Count - 1
                    str.Append(i1).Append(vbTab)
                    str.Append(tSSs("+")(i1)).Append(vbTab)
                    str.Append(tSSs("-")(i1)).AppendLine()
                Next
                Return str.ToString
            End Function

            Public Shared Function Get_BedGraph(tSSs As Dictionary(Of String, Integer()), Seq As Bio.ISequence) As String
                Dim out As New System.Text.StringBuilder
                For index = 0 To tSSs("+").Count - 1
                    If tSSs("+")(index) <> 0 Then
                        out.Append(Seq.ID).Append(vbTab)
                        out.Append(index).Append(vbTab)
                        out.Append(index + 1).Append(vbTab)
                        out.Append(tSSs("+")(index)).AppendLine()
                    End If
                    If tSSs("-")(index) <> 0 Then
                        out.Append(Seq.ID).Append(vbTab)
                        out.Append(index - 1).Append(vbTab)
                        out.Append(index).Append(vbTab)
                        out.Append(tSSs("-")(index)).AppendLine()
                    End If
                Next
                If out.Length > 0 Then out.Length -= 2
                Return out.ToString
            End Function
        End Class

        Public Class GTH
            ' Public Property Mappings As New List(Of Mapping)
            ' Public Property UsedSeqs As New List(Of Bio.ISequence)
            ' Public Property File As FileInfo
            Public Shared Function Get_MAppings(File As FileInfo, Seqs As List(Of Bio.ISequence))
                Dim SAMs As New List(Of String)
                For Each Item In Szunyi.IO.Import.Text.ParseByDelimiter(File, "*", "--------------------------------------------------------------------------------")
                    If Item.First.StartsWith("$") = False Then
                        Dim woEmpty = Szunyi.Text.Lists.RemoveEmptyLines(Item)
                        Dim EST = Szunyi.Text.Lists.GetLineStartWith(woEmpty, "EST Sequence")
                        Dim gDNS_ID = Split(Szunyi.Text.Lists.GetLineStartWith(woEmpty, "Genomic Template"), "=").Last
                        Dim EST_ID = Get_EST_ID(EST)
                        Dim SeqTemplate = Szunyi.Text.Lists.GetLinesBetween(woEmpty, "EST Sequence", "Genomic Template")
                        Dim ALs = Szunyi.Text.Lists.GetLinesBetween(woEmpty, "Alignment (genomic DNA sequence = upper lines)", "*************")
                        Dim EstSeq = Szunyi.Sequences.SequenceManipulation.GetSequences.FromLinesOneSeq(SeqTemplate, Alphabets.AmbiguousDNA)
                        EstSeq.ID = EST_ID
                        Dim Predictions = Szunyi.Text.Lists.GetLinesBetween(woEmpty, "Predicted", "MATCH")
                        Dim ExonLines = Szunyi.Text.Lists.GetLinesContains(Predictions, "Exon")
                        Dim IntronLines = Szunyi.Text.Lists.GetLinesContains(Predictions, "Intron")
                        '       Dim ReadType As ReadType_ByPolyA = Szunyi.Sequences.SequenceManipulation.Properties.Get_Read_Type(EstSeq, Nothing, 15)
                        Dim gSeq = Szunyi.Sequences.SequenceManipulation.GetSequences.ByID(Seqs, gDNS_ID)

                        Dim gExons = Get_Exons(ExonLines)

                        Dim gIntrons = Get_Exons(IntronLines)
                        Dim gIntronLength = Szunyi.Features.FeatureManipulation.GetLocations.Get_All_Exon_Location(gIntrons)
                        Dim mRNAExon = Get_Exons_For_mRNA(ExonLines)
                        Dim TheCIgar As String = ""
                        If mRNAExon.LocationStart > 0 Then
                            TheCIgar = mRNAExon.LocationStart & "S"
                        End If
                        Dim CiGar = GetCIgar(ALs)
                        TheCIgar = TheCIgar & CiGar.First
                        Dim MergedCigars As New List(Of String)
                        Dim exonLength As New List(Of Integer)
                        For i1 = 0 To CiGar.Count - 1
                            MergedCigars.Add(CiGar(i1))
                            exonLength.Add(Szunyi.BAM.CIGAR.Get_CIGAR_Full_Length(MergedCigars.Last))
                        Next
                        For i1 = 1 To CiGar.Count - 1
                            TheCIgar = TheCIgar & gIntronLength(i1 - 1).LocationEnd - gIntronLength(i1 - 1).LocationStart + 1 & "N" & CiGar(i1)
                        Next
                        If mRNAExon.LocationEnd < EstSeq.Count - 1 Then
                            TheCIgar = TheCIgar & EstSeq.Count - mRNAExon.LocationEnd - 1 & "S"
                        End If

                        Dim l = Szunyi.BAM.CIGAR.Get_CIGAR_Full_Length(TheCIgar)
                        If EstSeq.Count <> Szunyi.BAM.CIGAR.Get_CIGAR_Full_Length(TheCIgar) Then
                            Dim kj As Int16 = 43
                        Else
                            ''    Mappings.Add(New Mapping(gSeq, EstSeq, gExons, mRNAExon, SAm))
                        End If



                    End If
                Next

            End Function
            Public Shared Function ToSam(File As FileInfo, Seqs As List(Of Bio.ISequence)) As String
                Dim Mappings = Get_MAppings(File, Seqs)
                Dim Sams As New List(Of String)
                For Each Item In Mappings
                    If Item.ReadType = ReadType_ByPolyA.A Or ReadType_ByPolyA.T Then
                        Dim SAM As New List(Of String)
                        SAM.Add(Item.mRNASeq.ID)
                        If Item.gExon.Operator = LocationOperator.Complement Then
                            SAM.Add(16)
                            Item.mRNASeq = Item.mRNASeq.GetReverseComplementedSequence
                        Else
                            SAM.Add(0)
                        End If
                        SAM.Add(Item.gSeq.ID)
                        SAM.Add(Item.gExon.LocationStart)
                        SAM.Add("255 ") 'mapq
                        SAM.Add(Item.Cigar)
                        SAM.Add("*")
                        SAM.Add("0")
                        SAM.Add(0)
                        SAM.Add(Item.mRNASeq.ConvertToString)
                        SAM.Add("*")

                        Sams.Add(Szunyi.Text.General.GetText(SAM, vbTab))
                    End If
                Next
                For Each Seq In Seqs
                    If IsNothing(Seq) = False Then
                        Dim s = "@SQ" & vbTab & "SN:" & Seq.ID & vbTab & "LN:" & Seq.Count
                        Sams.Insert(0, s)
                    End If

                Next

            End Function
            Private Shared Function Get_EST_ID(EST_Line As String) As String
                Return Split(EST_Line, "description=").Last
            End Function
            Private Shared Function GetCIgar(Lines As List(Of String)) As List(Of String)
                Dim ToRemove = "0123456789 ".ToCharArray
                Lines = Szunyi.Text.Lists.RemoveEmptyLines(Lines)
                Lines = Szunyi.Text.Lists.Get_Lines_Not_StartWith(Lines, "//")
                Dim Est = Szunyi.Text.Lists.Get_Every_Xth_Lines_as_String(Lines, 2, 3)
                Dim AL = Szunyi.Text.Lists.Get_Every_Xth_Lines_as_String(Lines, 1, 3)
                Dim genomic = Szunyi.Text.Lists.Get_Every_Xth_Lines_as_String(Lines, 0, 3)
                Dim Ests = Szunyi.Text.General.RemoveFromString(Est, ToRemove)
                Dim ALs = Szunyi.Text.General.RemoveFromString(AL, ToRemove)
                Dim genomics = Szunyi.Text.General.RemoveFromString(genomic, ToRemove)
                Dim Start As Integer = 1
                Dim out As New List(Of String)
                Do
                    Dim i1 = InStr(Start, Ests, ".")
                    If i1 = 0 Then
                        out.Add(Ests.Substring(Start - 1, Ests.Length - Start + 1))
                        out.Add(genomics.Substring(Start - 1, Ests.Length - Start + 1))
                        Exit Do
                    Else
                        out.Add(Ests.Substring(Start, i1 - Start))
                        out.Add(genomics.Substring(Start, i1 - Start))
                        For i2 = i1 To Ests.Length
                            If Ests.Substring(i2, 1) <> "." Then
                                Start = i2 + 1
                                Exit For
                            End If
                        Next
                    End If
                Loop
                Dim rs As New List(Of String)

                For i1 = 0 To out.Count - 2 Step 2
                    Dim Cigars As New List(Of String)
                    Dim Values As New List(Of Integer)
                    Dim e = out(i1)
                    Dim g = out(i1 + 1)
                    For i2 = 0 To e.Count - 1

                        If e(i2) = "-" Then
                            If Cigars.Count = 0 OrElse Cigars.Last <> "D" Then
                                Cigars.Add("D")
                                Values.Add(1)
                            Else
                                Values(Values.Count - 1) += 1
                            End If
                        ElseIf g(i2) = "-" Then
                            If Cigars.Count = 0 OrElse Cigars.Last <> "I" Then
                                Cigars.Add("I")
                                Values.Add(1)
                            Else
                                Values(Values.Count - 1) += 1
                            End If
                        Else
                            If Cigars.Count = 0 OrElse Cigars.Last <> "M" Then
                                Cigars.Add("M")
                                Values.Add(1)
                            Else
                                Values(Values.Count - 1) += 1
                            End If
                        End If
                    Next
                    Dim s As String = ""
                    For i2 = 0 To Values.Count - 1
                        s = s & (Values(i2)) & Cigars(i2)
                    Next
                    rs.Add(s)
                Next

                Return rs
            End Function
            Private Function Get_AL_Seqs(Exons As List(Of String)) As List(Of Bio.Sequence)
                Dim out As New List(Of Bio.Sequence)
                Dim EST As New System.Text.StringBuilder
                For i1 = 0 To Exons.Count - 2 Step 3
                    Dim ch = Exons(i1)
                    Dim g = Szunyi.Sequences.SequenceManipulation.GetSequences.FromLineOneSeq(Exons(i1), Alphabets.AmbiguousDNA)
                    Dim Est2 = Szunyi.Sequences.SequenceManipulation.GetSequences.FromLineOneSeq(Exons(i1 + 2), Alphabets.AmbiguousDNA)
                    Dim alf As Int16 = 32

                Next
            End Function
            ''' <summary>
            ''' 0 based index
            ''' </summary>
            ''' <param name="ExonLines"></param>
            ''' <returns></returns>
            Private Shared Function Get_Exons_For_mRNA(ExonLines As List(Of String)) As Bio.IO.GenBank.Location
                Dim out As New List(Of Bio.IO.GenBank.Location)
                Dim str As New System.Text.StringBuilder
                Dim Loci As String
                Dim isComplementer As Boolean = False
                For Each Line In ExonLines
                    Dim s2 = Split(Line, ";")
                    Dim s = Split(s2(1), " ").ToList
                    Dim s1 = Szunyi.Text.Lists.RemoveEmptyLines(s)
                    Dim start As Integer = s1(1) - 1
                    Dim endy As Integer = s1(2) - 1
                    If endy < start Then
                        isComplementer = True
                    End If
                    str.Append(start).Append("..").Append(endy).Append(",")
                Next
                If ExonLines.Count > 0 Then str.Length -= 1
                If ExonLines.Count > 1 Then
                    Loci = "join(" & str.ToString & ")"
                Else
                    Loci = str.ToString
                End If
                If isComplementer = True Then
                    Loci = "complement(" & Loci & ") "
                End If
                Dim x1 = Szunyi.Features.FeatureManipulation.GetLocations.LociBuilder.GetLocation(Loci)
                Return x1
            End Function
            Private Shared Function Get_Exons(ExonLines As List(Of String)) As Bio.IO.GenBank.Location
                Dim out As New List(Of Bio.IO.GenBank.Location)
                Dim str As New System.Text.StringBuilder
                Dim Loci As String
                If ExonLines.Count = 0 Then Return Nothing
                If IsComplementer(ExonLines.First) = True Then
                    ExonLines.Reverse()
                    For Each Line In ExonLines

                        Dim s = Split(Line, " ").ToList
                        Dim s1 = Szunyi.Text.Lists.RemoveEmptyLines(s)
                        Dim start As Integer = s1(2)
                        Dim endy As Integer = s1(3)

                        str.Append(s1(3)).Append("..").Append(s1(2)).Append(",")
                    Next

                    If ExonLines.Count > 0 Then str.Length -= 1
                    If ExonLines.Count > 1 Then
                        Loci = "join(" & str.ToString & ")"
                    Else
                        Loci = str.ToString
                    End If
                    Loci = "complement(" & Loci & ") "
                Else
                    For Each Line In ExonLines
                        Dim s = Split(Line, " ").ToList
                        Dim s1 = Szunyi.Text.Lists.RemoveEmptyLines(s)
                        Dim start As Integer = s1(2)
                        Dim endy As Integer = s1(3)

                        str.Append(s1(2)).Append("..").Append(s1(3)).Append(",")

                    Next
                    If ExonLines.Count > 0 Then str.Length -= 1
                    If ExonLines.Count > 1 Then
                        Loci = "join(" & str.ToString & ")"
                    Else
                        Loci = str.ToString
                    End If
                End If

                Dim x1 = Szunyi.Features.FeatureManipulation.GetLocations.LociBuilder.GetLocation(Loci)
                Return x1
            End Function
            Private Shared Function IsComplementer(line As String) As Boolean
                Dim s = Split(line, " ").ToList
                Dim s1 = Szunyi.Text.Lists.RemoveEmptyLines(s)
                Dim start As Integer = s1(2)
                Dim endy As Integer = s1(3)
                If start > endy Then
                    Return True
                Else
                    Return False
                End If
            End Function

        End Class
    End Namespace
End Namespace

