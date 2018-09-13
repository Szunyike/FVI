Imports System.IO
Imports System.Text
Imports Bio
Imports Bio.IO.SAM
Imports ClassLibrary1.Szunyi.Location
Imports ClassLibrary1.Szunyi.Transcipts

Namespace Szunyi.BAM.Convert

    Public Class Table
#Region "TSS"
        Public Shared Function TSS(Feats As List(Of Bio.IO.GenBank.FeatureItem),
                                   Width As Integer,
                                   LocalWidth As Integer,
                                   P_Threshold As Double,
                                   Seqs As List(Of Bio.ISequence),
                                   File As FileInfo,
                                   BLs As List(Of Bio.IO.GenBank.ILocation),
                                   Enums As List(Of Integer)) As String

            Dim str As New System.Text.StringBuilder
            str.Append(Szunyi.BAM.Bam_Basic_IO.Headers.Get_Comments(File)).AppendLine()
            str.Append("# Local Width:").Append(LocalWidth).AppendLine()
            str.Append("# Width:").Append(Width).AppendLine()
            str.Append("# P-Threshold:").Append(P_Threshold).AppendLine()
            str.Append("# File:").Append(File.FullName).AppendLine()
            Dim x As New Szunyi.Location.Sites(Seqs.First, BLs, Szunyi.Constants.Sort_Locations_By.TSS, Width, LocalWidth, P_Threshold)
            str.Append("#Alpha:").Append(x.Get_alpha).AppendLine()

            Dim h = Split("Feature Key,Feature Label,Position,Strand,Count,Hundred,p-value,Is Local Maximum,Passed", ",")
            str.Append(Szunyi.Text.General.GetText(h, vbTab)).Append(vbTab)
            str.Append(x.Get_Distribution_Header)
            Dim PoIssons = x.Poisson(Feats, LocalWidth, Width, File)
            Dim Distribution = x.Get_Distribution(Feats)
            For i1 = 0 To PoIssons.Count - 1
                str.AppendLine()
                str.Append(PoIssons(i1)).Append(Distribution(i1))
            Next
            Return str.ToString
        End Function
        Public Shared Function TSS(Potential_TSS As List(Of Szunyi.Location.Basic_Location),
                                  Width As Integer,
                                   LocalWidth As Integer,
                                   P_Threshold As Double,
                                   Seq As Bio.ISequence,
                                   File As FileInfo,
                                   BLs As List(Of Bio.IO.GenBank.ILocation),
                                  Enums As List(Of Integer)) As List(Of Szunyi.Stat.Distribution_Result)


            Dim str As New System.Text.StringBuilder
            str.Append(Szunyi.BAM.Bam_Basic_IO.Headers.Get_Comments(File)).AppendLine()
            str.Append("# Local Width:").Append(LocalWidth).AppendLine()
            str.Append("# Width:").Append(Width).AppendLine()
            str.Append("# P-Threshold:").Append(P_Threshold).AppendLine()
            str.Append("#File:").Append(File.FullName).AppendLine()
            Dim Potential_TSS_Iloci = (From x1 In Potential_TSS Select x1.Location).ToList
            Dim x As New Szunyi.Location.Sites(Seq, BLs, Szunyi.Constants.Sort_Locations_By.TSS, Width, LocalWidth, P_Threshold)
            str.Append("#Alpha:").Append(x.Get_alpha).AppendLine()

            Dim h = Split("p-value calculation type,Position,Strand,Count,Hundred,p-value,Is Local Maximum,Passed", ",")
            str.Append(Szunyi.Text.General.GetText(h, vbTab)).Append(vbTab)
            str.Append(x.Get_Distribution_Header)
            Dim Distribution = x.Get_Distribution_Values(Potential_TSS)

            Dim Res As New List(Of Szunyi.Stat.Distribution_Result)
            For Each E In Enums
                Dim l As New Szunyi.Stat.Manager(x)
                Dim k = l.Calculate(Potential_TSS, LocalWidth, Width, File, E)
                For i1 = 0 To k.Count - 1
                    k(i1).Distribution = Distribution(i1)
                Next
                Res.AddRange(k)
            Next


            Return Res

        End Function

#End Region
        Public Shared Function False_PAS(Feats As List(Of List(Of Bio.IO.GenBank.ILocation)),
                                   Width As Integer,
                                   LocalWidth As Integer,
                                   P_Threshold As Double,
                                   Seqs As List(Of Bio.ISequence),
                                   File As FileInfo,
                                   Sams As List(Of Bio.IO.SAM.SAMAlignedSequence),
                                   BLs As List(Of Bio.IO.GenBank.ILocation),
                                         Enums As List(Of Integer)) As String

            Dim str As New System.Text.StringBuilder
            str.Append(Szunyi.BAM.Bam_Basic_IO.Headers.Get_Comments(File)).AppendLine()
            str.Append("#:Local Width=").Append(LocalWidth).AppendLine()
            str.Append("#:Width=").Append(Width).AppendLine()
            str.Append("# P-Threshold:").Append(P_Threshold).AppendLine()
            str.Append("# File:").Append(File.FullName).AppendLine()
            Dim x As New Szunyi.Location.Sites(Seqs.First, BLs, Szunyi.Constants.Sort_Locations_By.PAS, Width, LocalWidth, P_Threshold)
            str.Append("#Alpha:").Append(x.Get_alpha).AppendLine()

            Dim h = Split("Position,Strand,Count,Hundred,p-value,Is Local Maximum,Passed", ",")
            str.Append(Szunyi.Text.General.GetText(h, vbTab)).Append(vbTab)
            str.Append(x.Get_Distribution_Header).Append(vbTab)
            Dim h1 = Split("Signal Sequnece,Distance From PAS,Distance from Optimal Position)", ",")
            str.Append(Szunyi.Text.General.GetText(h1, vbTab)).Append(vbTab)
            str.Append(x.Get_Nof_As_Heder)
            Dim For_PAS = Szunyi.Location.Basic_Location_Manipulation.Get_Basic_LocationS(Feats)
            Dim x1 = Szunyi.DNA.PA.Get_PolyA_Signals(Seqs.First, For_PAS, 50, -22)
            Dim PA = Szunyi.DNA.PA.Get_Poly_Signals_ToString(x1)
            Dim PoIssons = x.Poisson(For_PAS, 10, 50, File)
            Dim Distribution = x.Get_Distribution(For_PAS)
            Dim nof_A = x.Get_Nof_As(For_PAS, Seqs.First)
            For i1 = 0 To PoIssons.Count - 1
                str.AppendLine()
                str.Append(PoIssons(i1)).Append(Distribution(i1)).Append(vbTab).Append(PA(i1)).Append(vbTab).Append(nof_A(i1))
            Next
            Return str.ToString
        End Function
        Public Shared Function PAS(Potential_PAS As List(Of Szunyi.Location.Basic_Location),
                                  Width As Integer,
                                   LocalWidth As Integer,
                                   P_Threshold As Double,
                                   Seq As Bio.ISequence,
                                   File As FileInfo,
                                   BLs As List(Of Bio.IO.GenBank.ILocation),
                                   Enums As List(Of Integer)) As List(Of Szunyi.Stat.Distribution_Result)

            Dim str As New System.Text.StringBuilder
            str.Append(Szunyi.BAM.Bam_Basic_IO.Headers.Get_Comments(File)).AppendLine()
            str.Append("# Local Width:").Append(LocalWidth).AppendLine()
            str.Append("# Width:").Append(Width).AppendLine()
            str.Append("# P-Threshold:").Append(P_Threshold).AppendLine()
            str.Append("#File:").Append(File.FullName).AppendLine()
            Dim Potential_TSS_Iloci = (From x1 In Potential_PAS Select x1.Location).ToList
            Dim x As New Szunyi.Location.Sites(Seq, BLs, Szunyi.Constants.Sort_Locations_By.PAS, Width, LocalWidth, P_Threshold)
            str.Append("#Alpha:").Append(x.Get_alpha).AppendLine()

            Dim h = Split("Position,Strand,Count,Hundred,p-value,Is Local Maximum,Passed", ",")
            str.Append(Szunyi.Text.General.GetText(h, vbTab)).Append(vbTab)
            str.Append(x.Get_Distribution_Header)
            Dim Distribution = x.Get_Distribution_Values(Potential_PAS)

            Dim Res As New List(Of Szunyi.Stat.Distribution_Result)
            For Each E In Enums
                Dim l As New Szunyi.Stat.Manager(x)
                Dim k = l.Calculate(Potential_PAS, LocalWidth, Width, File, E)
                For i1 = 0 To k.Count - 1
                    k(i1).Distribution = Distribution(i1)
                Next
                Res.AddRange(k)
            Next


            Return Res

        End Function
        Public Shared Function PAS(Feats As List(Of Bio.IO.GenBank.FeatureItem),
                                   Width As Integer,
                                   LocalWidth As Integer,
                                   P_Threshold As Double,
                                   Seqs As List(Of Bio.ISequence),
                                   File As FileInfo,
                                   Sams As List(Of Bio.IO.SAM.SAMAlignedSequence),
                                   BLs As List(Of Bio.IO.GenBank.ILocation),
                                   Enums As List(Of Integer)) As String

            Dim str As New System.Text.StringBuilder
            str.Append(Szunyi.BAM.Bam_Basic_IO.Headers.Get_Comments(File)).AppendLine()
            str.Append("#:Local Width=").Append(LocalWidth).AppendLine()
            str.Append("#:Width=").Append(Width).AppendLine()
            str.Append("# P-Threshold:").Append(P_Threshold).AppendLine()

            Dim x As New Szunyi.Location.Sites(Seqs.First, BLs, Szunyi.Constants.Sort_Locations_By.PAS, Width, LocalWidth, P_Threshold)
            str.Append("#Alpha:").Append(x.Get_alpha).AppendLine()

            Dim h = Split("Feature Key,Feature Label,Position,Strand,Count,Hundred,p-value,Is Local Maximum,Passed", ",")
            str.Append(Szunyi.Text.General.GetText(h, vbTab)).Append(vbTab)
            str.Append(x.Get_Distribution_Header).Append(vbTab)
            Dim h1 = Split("Signal Sequnece,Distance From PAS,Distance from Optimal Position)", ",")
            str.Append(Szunyi.Text.General.GetText(h1, vbTab)).Append(vbTab)

            Dim x1 = Szunyi.DNA.PA.Get_PolyA_Signals(Seqs.First, Feats, 50, -22)
            Dim PA = Szunyi.DNA.PA.Get_Poly_Signals_ToString(x1)
            Dim PoIssons = x.Poisson(Feats, 10, 50, File)
            Dim Distribution = x.Get_Distribution(Feats)
            For i1 = 0 To PoIssons.Count - 1
                str.AppendLine()
                str.Append(PoIssons(i1)).Append(Distribution(i1)).Append(vbTab).Append(PA(i1))
            Next
            Return str.ToString
        End Function
#Region "Introns"
        Public Shared Sub Intron(minIntronLength As Integer,
                                 maxIntronLength As Integer,
                                 MinExonBorderLength As Integer,
                                 file As FileInfo,
                                 Seqs As List(Of ISequence),
                                 I_Or_M As Boolean,
                                 wOrientation As Boolean)
            Dim All_Sites As New List(Of Szunyi.BAM.SAM_Manipulation.Location.MdCigar)


            Dim Real_Introns As New List(Of Bio.IO.GenBank.ILocation)
            For Each sam In Szunyi.BAM.Bam_Basic_IO.Import.Parse(file)

                Dim Loci = Szunyi.Location.Common.GetLocation(sam)

                Dim Intron = Szunyi.Location.Common.Get_All_Intron_Location(Loci)
                Dim Exons = Szunyi.Location.Common.Get_All_Exon_Location(Loci)
                For i1 = 0 To Intron.Count - 1
                    Dim cIntron = Intron(i1)
                    Dim s = Szunyi.Location.Common.GetLocationString(cIntron)
                    s = s.Replace(cIntron.LocationEnd, cIntron.LocationEnd + 1)
                    cIntron = Szunyi.Location.Common.Get_Location(s)

                    Dim Intron_length = Szunyi.Location.Common.Get_Length(cIntron)
                    If Intron_length >= minIntronLength AndAlso Intron_length <= maxIntronLength Then
                        Dim e1_length = Szunyi.Location.Common.Get_Length(Exons(i1))
                        If e1_length >= MinExonBorderLength Then
                            Dim e2_length = Szunyi.Location.Common.Get_Length(Exons(i1 + 1))
                            If e2_length >= MinExonBorderLength Then
                                Real_Introns.Add(cIntron)
                            End If
                        End If
                    End If

                Next


            Next
            If wOrientation = True Then
                Dim Introns_TDT = Get_Introns_tdt(Seqs.First, Real_Introns, minIntronLength, maxIntronLength, MinExonBorderLength, True)
                Dim NFIle As New FileInfo(file.FullName & "_Intron_Analysis_With_Orientation.tsv")
                Szunyi.IO.Export.SaveText(Introns_TDT, NFIle)
            Else
                Real_Introns = Szunyi.Location.Common.Set_Direction(Real_Introns, True)
                Dim Introns_TDT = Get_Introns_tdt(Seqs.First, Real_Introns, minIntronLength, maxIntronLength, MinExonBorderLength, False)
                Dim NFIle As New FileInfo(file.FullName & "_Intron_Analysis_With_Out_Orientation.tsv")
                Szunyi.IO.Export.SaveText(Introns_TDT, NFIle)
            End If




        End Sub

        Public Shared Sub Intron(minIntronLength As Integer,
                                 maxIntronLength As Integer,
                                 MinExonBorderLength As Integer,
                                 files As List(Of FileInfo),
                                 Seqs As List(Of ISequence),
                                 I_Or_M As Boolean,
                                 wOrientation As Boolean)

            For Each FIle In files

                Dim Real_Introns As New List(Of Bio.IO.GenBank.ILocation)
                For Each sam In Szunyi.BAM.Bam_Basic_IO.Import.Parse(FIle)

                    Dim Loci = Szunyi.Location.Common.GetLocation(sam)

                    Dim Intron = Szunyi.Location.Common.Get_All_Intron_Location(Loci)
                    Dim Exons = Szunyi.Location.Common.Get_All_Exon_Location(Loci)
                    For i1 = 0 To Intron.Count - 1
                        Dim cIntron = Intron(i1)

                        Dim Intron_length = Szunyi.Location.Common.Get_Length(cIntron)
                        If Intron_length >= minIntronLength AndAlso Intron_length <= maxIntronLength Then
                            Dim e1_length = Szunyi.Location.Common.Get_Length(Exons(i1))
                            If e1_length >= MinExonBorderLength Then
                                Dim e2_length = Szunyi.Location.Common.Get_Length(Exons(i1 + 1))
                                If e2_length >= MinExonBorderLength Then
                                    Real_Introns.Add(cIntron)
                                End If
                            End If
                        End If

                    Next


                Next
                If wOrientation = True Then
                    Dim Introns_TDT = Get_Introns_tdt(Seqs.First, Real_Introns, minIntronLength, maxIntronLength, MinExonBorderLength, True)
                    Dim NFIle As New FileInfo(FIle.FullName & "_Intron_Analysis_With_Orientation.tsv")
                    Szunyi.IO.Export.SaveText(Introns_TDT, NFIle)
                Else
                    Real_Introns = Szunyi.Location.Common.Set_Direction(Real_Introns, True)
                    Dim Introns_TDT = Get_Introns_tdt(Seqs.First, Real_Introns, minIntronLength, maxIntronLength, MinExonBorderLength, False)
                    Dim NFIle As New FileInfo(FIle.FullName & "_Intron_Analysis_With_Out_Orientation.tsv")
                    Szunyi.IO.Export.SaveText(Introns_TDT, NFIle)
                End If


            Next

        End Sub

        Public Shared Function Get_Introns_tdt(Seq As Bio.ISequence,
                                               Locis As List(Of Bio.IO.GenBank.ILocation),
                                               MinIntronLength As Integer,
                                               MaxIntronLength As Integer,
                                               MinExonBorderLength As Integer,
                                               wOrientation As Boolean) As String

            Dim SplicesSitesII As New System.Text.StringBuilder
            SplicesSitesII.Append(Szunyi.Text.General.GetText(Split("Nof Read:Strand:Location Start:Location End:Intron Length:Donor Site Sequence:Acceptor Site Sequence:Donor Site +-4 bp Sequence:Acceptor Site +-4 bp Sequence:Donor Site Alignment:Acceptor Site ALignment:Consensus Sequence Of Repeat:Length of Repeat:StarOffSet Donon:StaroffSet Acceptor", ":"), vbTab)).AppendLine()

            Dim Sw As New Bio.Algorithms.Alignment.SmithWatermanAligner
            Dim x As New Bio.SimilarityMatrices.DiagonalSimilarityMatrix(4, -4)

            Sw.SimilarityMatrix = x
            Sw.GapOpenCost = -4
            Sw.GapExtensionCost = -4


            SplicesSitesII.Append("#DiagonalSimilarityMatrix:").Append(x.DiagonalValue).Append(":").Append(x.OffDiagonalValue).AppendLine()
            SplicesSitesII.Append("#GapOpenCost:").Append(Sw.GapOpenCost).AppendLine()
            SplicesSitesII.Append("#GapExtensionCost:").Append(Sw.GapExtensionCost).AppendLine()
            SplicesSitesII.Append("#MinIntronLength:").Append(MinIntronLength).AppendLine()
            SplicesSitesII.Append("#MaxIntronLength:").Append(MaxIntronLength).AppendLine()
            SplicesSitesII.Append("#MinExonBorderLength:").Append(MinExonBorderLength).AppendLine()

            Dim CV = Szunyi.Location.Merging.GroupBy(Locis, Szunyi.Constants.Sort_Locations_By.TSS_PAS, 1, wOrientation)
            Dim TSs As New List(Of Szunyi.mRNA.Transcript.TemplateSwitch)

            For Each Loci In CV
                TSs.Add(New mRNA.Transcript.TemplateSwitch(Seq, Loci))
                TSs.Last.Count = Loci.Count

            Next
            For Each Item In TSs
                SplicesSitesII.Append(Item.ToString).AppendLine()
            Next
            Return SplicesSitesII.ToString
        End Function

#End Region

        Public Shared Sub Aligned_Length_Distribution(files As List(Of FileInfo), max As Integer)
            For Each F In files
                Dim Res(max) As Integer
                Dim All = Szunyi.BAM.Bam_Basic_IO.Import.ParseAll(F)
                Dim Best = Szunyi.BAM.Filter_Split.Get_Bests_From_Groupby_ReadID(All)
                Using sw As New StreamWriter(F.FullName & "_AL_Distribution_ReadID.tsv")
                    For Each Sam In Best
                        Dim AL_Length = Szunyi.BAM.Stat.Stat.Get_Aligned_Length(Sam)
                        If AL_Length < max Then
                            Res(AL_Length) += 1
                        End If

                        sw.Write(Sam.QName)
                        sw.Write(vbTab)
                        sw.Write(AL_Length)
                        sw.WriteLine()
                    Next
                End Using
                Dim t = F.Name & vbCrLf & Szunyi.Text.General.GetText(Res)
                Szunyi.IO.Export.SaveText(t, New FileInfo(F.FullName & "_AL_Distribution.tsv"))
            Next
            For Each F In files
                Dim Res(max) As Integer
                Using sw As New StreamWriter(F.FullName & "_AL_Distribution_ReadID.tsv")
                    Dim All = Szunyi.BAM.Bam_Basic_IO.Import.ParseAll(F)
                    Dim Best = Szunyi.BAM.Filter_Split.Get_Bests_(All)
                    For Each Sam In Best
                        Dim AL_Length = Szunyi.BAM.Stat.Stat.Get_Aligned_Length(Sam)
                        sw.Write(Sam.QName)
                        sw.Write(vbTab)
                        sw.Write(AL_Length)
                        sw.WriteLine()
                    Next
                End Using
                Dim t = F.Name & vbCrLf & Szunyi.Text.General.GetText(Res)

            Next
        End Sub

        Public Shared Sub Adaptor_Distribution(file As FileInfo)
            ' Get Adaptors 
            ' Create Tables
            Dim Headers = Bam_Basic_IO.Headers.Get_Header(file)
            Dim Adaptors = Szunyi.BAM.Bam_Basic_IO.Headers.Get_Adaptors(Headers)
            Dim x As New Adaptor_dist(Adaptors)
            Dim kj As Int16 = 43
            For Each SAM In Szunyi.BAM.Bam_Basic_IO.Import.Parse(file)
                For Each AD In Adaptors
                    Dim cData = Szunyi.BAM.BAM_Optional_Filed_Manipulation.Get_Adapter_Value(SAM, AD)
                    Dim j = x.Sg(AD)(AD.Five_Prime_Adapter.ID & "In_Adaptor")
                    If j.ContainsKey(cData.Five_Prime_Score) = False Then j.Add(cData.Five_Prime_Score, New Dictionary(Of Integer, Integer))
                    If j(cData.Five_Prime_Score).ContainsKey(cData.Five_Prime_Pos_in_Adapter) = False Then j(cData.Five_Prime_Score).Add(cData.Five_Prime_Pos_in_Adapter, 0)
                    j(cData.Five_Prime_Score)(cData.Five_Prime_Pos_in_Adapter) += 1


                    j = x.Sg(AD)(AD.Three_Prime_Adapter.ID & "In_Adaptor")
                    If j.ContainsKey(cData.Three_Prime_Score) = False Then j.Add(cData.Three_Prime_Score, New Dictionary(Of Integer, Integer))
                    If j(cData.Three_Prime_Score).ContainsKey(cData.Three_Prime_Pos_in_Adapter) = False Then j(cData.Three_Prime_Score).Add(cData.Three_Prime_Pos_in_Adapter, 0)
                    j(cData.Three_Prime_Score)(cData.Three_Prime_Pos_in_Adapter) += 1

                    j = x.Sg(AD)(AD.Five_Prime_Adapter.ID & "In_Match")
                    If j.ContainsKey(cData.Five_Prime_Score) = False Then j.Add(cData.Five_Prime_Score, New Dictionary(Of Integer, Integer))
                    If j(cData.Five_Prime_Score).ContainsKey(cData.Five_Prime_Pos) = False Then j(cData.Five_Prime_Score).Add(cData.Five_Prime_Pos, 0)
                    j(cData.Five_Prime_Score)(cData.Five_Prime_Pos) += 1

                    j = x.Sg(AD)(AD.Three_Prime_Adapter.ID & "In_Match")
                    If j.ContainsKey(cData.Three_Prime_Score) = False Then j.Add(cData.Three_Prime_Score, New Dictionary(Of Integer, Integer))
                    If j(cData.Three_Prime_Score).ContainsKey(cData.Three_Prime_Pos) = False Then j(cData.Three_Prime_Score).Add(cData.Three_Prime_Pos, 0)
                    j(cData.Three_Prime_Score)(cData.Three_Prime_Pos) += 1

                Next

            Next
            Dim str As New System.Text.StringBuilder
            For Each Item In x.Sg

                For Each sItem In Item.Value
                    str.Append(sItem.Key).AppendLine()
                    str.Append("Score")
                    For i1 = -10 To 30
                        str.Append(vbTab).Append(i1)
                    Next

                    Dim s = From x1 In sItem.Value Order By x1.Key Ascending

                    For Each Sc In s
                        str.AppendLine()
                        str.Append(Sc.Key)
                        Dim Pos = From x1 In Sc.Value Order By x1.Key Ascending


                        For i1 = -10 To 30
                            If Sc.Value.ContainsKey(i1) = True Then
                                str.Append(vbTab).Append(Sc.Value(i1))
                            Else
                                str.Append(vbTab)
                            End If
                        Next
                    Next
                    str.AppendLine()
                Next



            Next
            Dim nFile As New FileInfo(file.FullName & "_Adapter_Distribution.tsv")
            Szunyi.IO.Export.SaveText(str.ToString, nFile)
            Dim res = str.ToString
        End Sub

        Public Shared Sub All_Length_Distributions(files As List(Of FileInfo), bins As List(Of Integer))

            Dim All_Max_Read_Length_CL As Integer = 1000
            Dim All_Max_Read_Length_AL As Integer = 1000

            Dim Result_AL As New Dictionary(Of FileInfo, Integer())
            Dim Result_CL As New Dictionary(Of FileInfo, Integer())
            Dim header As New System.Text.StringBuilder
            header.Append("bin")
            For Each F In files
                header.Append(vbTab).Append(F.Name)
                'header.Append(vbTab).Append(F.Name & "_log10_Counts")
                Dim Max_Read_Length As Integer = 1000
                Dim Res_AL(Max_Read_Length) As Integer
                Dim Res_CL(Max_Read_Length) As Integer
                Dim Bests = Szunyi.BAM.Bam_Basic_IO.Import.ParseAll_IntoStat_Bests(F)

                Using sw As New StreamWriter(F.FullName & "_AL_Distribution_ReadID.tsv")
                    Using sw1 As New StreamWriter(F.FullName & "_CL_Distribution_ReadID.tsv")
                        For Each Sam In Bests
                            Dim AL_Length = Sam.Aligned_Read_Length
                            If Sam.Read_Length >= Res_CL.Count Then
                                ReDim Preserve Res_CL(Sam.Read_Length)
                                All_Max_Read_Length_CL = Sam.Read_Length
                            End If
                            If Sam.Aligned_Read_Length >= Res_AL.Count Then
                                ReDim Preserve Res_AL(Sam.Aligned_Read_Length)
                                All_Max_Read_Length_AL = Sam.Aligned_Read_Length
                            End If
                            Res_AL(AL_Length) += 1
                            Res_CL(Sam.Aligned_Read_Length) += 1

                            sw.Write(Sam.Read_ID)
                            sw.Write(vbTab)
                            sw.Write(AL_Length)
                            sw.WriteLine()

                            sw1.Write(Sam.Read_ID)
                            sw1.Write(vbTab)
                            sw1.Write(Sam.Read_Length)
                            sw1.WriteLine()
                        Next
                    End Using
                End Using
                Result_AL.Add(F, Res_AL)
                Result_CL.Add(F, Res_CL)
            Next

            For Each Bin In bins
                Dim maxCount_AL = (From x In Result_AL Select x.Value.Count).Max
                Dim nof_Bin_AL = maxCount_AL \ Bin

                Dim AL(nof_Bin_AL) As String
                Dim AL_log10(nof_Bin_AL) As String
                For i1 = 0 To AL.Count - 1
                    AL(i1) = (i1 + 1) * Bin + 1 - Bin & "-" & Bin * (i1 + 1)
                    AL_log10(i1) = (i1 + 1) * Bin + 1 - Bin & "-" & Bin * (i1 + 1)
                Next
                For Each Item In Result_AL
                    Dim tmp(nof_Bin_AL) As Integer
                    For i1 = 0 To Item.Value.Count - 1
                        tmp(i1 \ Bin) += Item.Value(i1)
                    Next
                    For i1 = 0 To AL.Count - 1
                        AL(i1) = AL(i1) & vbTab & tmp(i1)
                        Dim d = System.Math.Log10(tmp(i1))
                        If Double.IsInfinity(d) = True Or Double.IsNaN(d) = True Then
                            AL_log10(i1) = AL_log10(i1) & vbTab & 0
                        Else
                            AL_log10(i1) = AL_log10(i1) & vbTab & d
                        End If
                    Next
                Next
                Dim nFile As New FileInfo(files.First.DirectoryName & "\Distribution_AL_bin_Counts" & Bin & ".tsv")
                Szunyi.IO.Export.SaveText(header.ToString & vbCrLf & Szunyi.Text.General.GetText(AL), nFile)

                Dim nFile2 As New FileInfo(files.First.DirectoryName & "\Distribution_AL_bin_log10_Counts" & Bin & ".tsv")
                Szunyi.IO.Export.SaveText(header.ToString & vbCrLf & Szunyi.Text.General.GetText(AL_log10), nFile2)
            Next

            For Each Bin In bins
                Dim maxCount_CL = (From x In Result_CL Select x.Value.Count).Max
                Dim nof_Bin_CL = maxCount_CL \ Bin

                Dim CL(nof_Bin_CL) As String
                Dim CL_log10(nof_Bin_CL) As String

                For i1 = 0 To CL.Count - 1
                    CL(i1) = (i1 + 1) * Bin + 1 - Bin & "-" & Bin * (i1 + 1)
                    CL_log10(i1) = (i1 + 1) * Bin + 1 - Bin & "-" & Bin * (i1 + 1)
                Next
                For Each Item In Result_CL
                    Dim tmp(nof_Bin_CL) As Integer
                    For i1 = 0 To Item.Value.Count - 1
                        tmp(i1 \ Bin) += Item.Value(i1)
                    Next
                    For i1 = 0 To tmp.Count - 1
                        CL(i1) = CL(i1) & vbTab & tmp(i1)
                        Dim d = System.Math.Log10(tmp(i1))
                        If Double.IsInfinity(d) = True Or Double.IsNaN(d) = True Then
                            CL_log10(i1) = CL_log10(i1) & vbTab & 0
                        Else
                            CL_log10(i1) = CL_log10(i1) & vbTab & d
                        End If
                    Next
                Next
                Dim nFile As New FileInfo(files.First.DirectoryName & "\Distribution_CL_bin_Counts" & Bin & ".tsv")
                Szunyi.IO.Export.SaveText(header.ToString & vbCrLf & Szunyi.Text.General.GetText(CL), nFile)

                Dim nFile2 As New FileInfo(files.First.DirectoryName & "\Distribution_CL_bin_log10_Counts" & Bin & ".tsv")
                Szunyi.IO.Export.SaveText(header.ToString & vbCrLf & Szunyi.Text.General.GetText(CL_log10), nFile2)

            Next
        End Sub
    End Class

    Public Class Poisson_Result
        Public Property Poissons As List(Of Poisson)
        ' Public Property distribution As List(Of List(Of Integer))
        Public Property Header As String

        Public Sub New(poIssons As List(Of Poisson), distribution As List(Of List(Of Integer)), str As StringBuilder, file As FileInfo)
            For i1 = 0 To poIssons.Count - 1
                poIssons(i1).Distribution = distribution(i1)
            Next
            Me.Poissons = poIssons
            '      Me.distribution = distribution
            Me.Header = str.ToString
        End Sub
        Public Overrides Function ToString() As String
            Dim str As New System.Text.StringBuilder
            str.Append(Header)
            For i1 = 0 To Poissons.Count - 1
                str.AppendLine()
                str.Append(Poissons(i1).ToString)

            Next
            Return str.ToString
        End Function
    End Class

    Public Class Adaptor_dist
        Private adaptors As List(Of Adaptor_Pair)
        Public Sg As New Dictionary(Of Adaptor_Pair, Dictionary(Of String, Dictionary(Of Integer, Dictionary(Of Integer, Integer))))
        Public Sub New(adaptors As List(Of Adaptor_Pair))
            Me.adaptors = adaptors
            For Each a In adaptors

                Sg.Add(a, New Dictionary(Of String, Dictionary(Of Integer, Dictionary(Of Integer, Integer))))
                Dim txt = a.Five_Prime_Adapter.ID & "In_Adaptor"
                Sg(a).Add(a.Five_Prime_Adapter.ID & "In_Adaptor", New Dictionary(Of Integer, Dictionary(Of Integer, Integer)))
                Sg(a).Add(a.Five_Prime_Adapter.ID & "In_Match", New Dictionary(Of Integer, Dictionary(Of Integer, Integer)))
                Sg(a).Add(a.Three_Prime_Adapter.ID & "In_Adaptor", New Dictionary(Of Integer, Dictionary(Of Integer, Integer)))
                Sg(a).Add(a.Three_Prime_Adapter.ID & "In_Match", New Dictionary(Of Integer, Dictionary(Of Integer, Integer)))
            Next
        End Sub

        Public Property x As Szunyi.mRNA.PolyA

    End Class

End Namespace

