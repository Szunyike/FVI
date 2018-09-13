Imports System.IO
Imports Bio.IO.GenBank
Imports ClassLibrary1.Szunyi.BAM.Convert
Imports ClassLibrary1.Szunyi.Constants
Imports ClassLibrary1.Szunyi.Location
Imports ClassLibrary1.Szunyi.mRNA.Transcript

Namespace Szunyi.Transcipts
    Public Enum Tr_Analysis_Types
        TSS_Distribution = 1
        PAS_Distribution = 2
        TSS_Poisson = 3
        PAS_Poisson = 4
        PolyA_Signal = 5
        A_T_Before_PAS = 6
        Cistronic_Complex = 7

    End Enum
    Public Class Analysis
        Public Property Dir As DirectoryInfo
        Public Property OriSeq As Bio.ISequence
        Public Property Seq As Bio.ISequence
        Public Property Intron As Szunyi.BAM.Settings.Intron
        Public Property TSS_PAS_TR As Szunyi.BAM.Settings.TSS_PAS_TR
        Public Property CountBy As Szunyi.BAM.Settings.Count_As


        Public Property TSSs As New List(Of Szunyi.Location.Basic_Location)
        Public Property PASs As New List(Of Szunyi.Location.Basic_Location)
        Public Property PAs As New List(Of Szunyi.Location.Basic_Location)

        Public Property Site_Manager_PAS As Szunyi.Stat.Manager
        Public Property Site_Manager_TSS As Szunyi.Stat.Manager
        Public Property Qual As String
        Public Property Nof_AA As Integer

        Dim TSS_Files As IEnumerable(Of FileInfo)
        Dim PAS_Files As IEnumerable(Of FileInfo)
        Dim PA_Files As IEnumerable(Of FileInfo)
        Public Sub New()

        End Sub
        Public Sub New(Dir As DirectoryInfo,
                       Seq As Bio.ISequence,
                       Intron As Szunyi.BAM.Settings.Intron,
                       TSS_PAS_TR As Szunyi.BAM.Settings.TSS_PAS_TR,
                       CountBy As Szunyi.BAM.Settings.Count_As)
            Me.Dir = Dir
            Me.Seq = Szunyi.Sequences.SequenceManipulation.Common.CloneSeq(Seq)
            Me.OriSeq = Seq
            Me.Intron = Intron
            Me.TSS_PAS_TR = TSS_PAS_TR
            Me.CountBy = CountBy

        End Sub
        Public Sub Import()
            TSS_Files = From x1 In Dir.GetFiles Where x1.Name.StartsWith("For_TSS") And (x1.Extension = ".loc")
            If TSS_Files.Count > 0 Then
                TSSs = Szunyi.IO.Import.Locations.Parse_Into_Basic_Locations(TSS_Files.First).ToList
            Else
                TSS_Files = From x1 In Dir.GetFiles Where x1.Name.StartsWith("For_TSS") And (x1.Extension = ".sam" Or x1.Extension = ".bam")
                TSSs = Szunyi.BAM.Bam_Basic_IO.Import.ParseAll_Into_Basic_Locations(TSS_Files.First)
            End If

            PAS_Files = From x1 In Dir.GetFiles Where x1.Name.StartsWith("For_PAS") And (x1.Extension = ".loc")
            If PAS_Files.Count > 0 Then
                PASs = Szunyi.IO.Import.Locations.Parse_Into_Basic_Locations(PAS_Files.First).ToList
            Else
                PAS_Files = From x1 In Dir.GetFiles Where x1.Name.StartsWith("For_PAS") And (x1.Extension = ".sam" Or x1.Extension = ".bam")
                PASs = Szunyi.BAM.Bam_Basic_IO.Import.ParseAll_Into_Basic_Locations(PAS_Files.First)
            End If

            PA_Files = From x1 In Dir.GetFiles Where x1.Name.StartsWith("polyAT") And (x1.Extension = ".loc")
            If PA_Files.Count > 0 Then
                PAs = Szunyi.IO.Import.Locations.Parse_Into_Basic_Locations(PA_Files.First).ToList
            Else
                PA_Files = From x1 In Dir.GetFiles Where x1.Name.StartsWith("polyAT") And (x1.Extension = ".sam" Or x1.Extension = ".bam")
                PAs = Szunyi.BAM.Bam_Basic_IO.Import.ParseAll_Into_Basic_Locations(PA_Files.First)
            End If

        End Sub
        Private Function Filter_PAS(PASs As List(Of FeatureItem))
            Dim out As New List(Of FeatureItem)
            Dim Max_AT = Me.TSS_PAS_TR.Max_nof_AT.Default_Value
            For Each Feat In PASs
                Dim Failed As Boolean = True
                If Feat.Location.IsComplementer = True Then
                    For i1 = Feat.Location.TSS To Feat.Location.TSS + Max_AT - 1
                        If Me.Seq(i1) <> Bio.Alphabets.DNA.T Then
                            Failed = False
                            Exit For
                        End If
                    Next
                    If Failed = False Then
                        out.Add(Feat)
                    Else
                        Szunyi.Features.FeatureManipulation.GenBankMetaDataManipulation.Remove_Feature(Seq, Feat)
                    End If
                Else

                    For i1 = Feat.Location.PAS - 1 To Feat.Location.PAS - Max_AT - 1 Step -1
                        If Me.Seq(i1) <> Bio.Alphabets.DNA.A Then
                            Failed = False
                            Exit For
                        End If
                    Next
                End If
                If Failed = False Then
                    out.Add(Feat)
                Else
                    Szunyi.Features.FeatureManipulation.GenBankMetaDataManipulation.Remove_Feature(Seq, Feat)
                End If
            Next
            Return out

        End Function
        Public Sub DoIt()
            If Me.TSSs.Count = 0 Then Import()
            Dim Basic_FileName = Me.Dir.FullName & "\" & Dir.Name
            Dim The_Enum = Szunyi.Util_Helpers.Get_Enum_Value(Of Szunyi.Stat.Distributions)(TSS_PAS_TR.Distribution.Selected_Value)
            Site_Manager_PAS = New Szunyi.Stat.Manager(TSS_PAS_TR.LocalWidth.Default_Value,
                                                           TSS_PAS_TR.Width.Default_Value,
                                                           Sort_Locations_By.PAS,
                                                           Seq, "PAS",
                                                           PASs,
                                                           The_Enum,
                                                           TSS_PAS_TR.P_Threshold.Default_Value)
            Site_Manager_PAS.Calculate()
            Site_Manager_PAS.Save(New FileInfo(Basic_FileName & "ALL_PAS"), Seq) ' Save It 2 tsv file and a genbank
            Dim newPASs = Site_Manager_PAS.Add_Features(Seq)

            Dim BL_NewPAS = Szunyi.Location.Basic_Location_Manipulation.Get_Basic_LocationS(newPASs)
            Dim PAS_Finder As New Szunyi.Location.Basic_Location_Finder(BL_NewPAS, True)
            Site_Manager_TSS = New Szunyi.Stat.Manager(TSS_PAS_TR.LocalWidth.Default_Value,
                                                           TSS_PAS_TR.Width.Default_Value,
                                                           Sort_Locations_By.TSS,
                                                           Seq, "TSS",
                                                           TSSs,
                                                           The_Enum,
                                                           TSS_PAS_TR.P_Threshold.Default_Value)
            Site_Manager_TSS.Calculate()
            Site_Manager_TSS.Save(New FileInfo(Basic_FileName & "ALL_TSS"), Seq)
            Dim newTSSs = Site_Manager_TSS.Add_Features(Seq)

            Dim BL_NewTSS = Szunyi.Location.Basic_Location_Manipulation.Get_Basic_LocationS(newTSSs)
            Dim TSS_Finder As New Szunyi.Location.Basic_Location_Finder(BL_NewTSS, True)
            Dim Filtered_PAS = Filter_PAS(newPASs)

            Dim TS = Get_Introns()
            Dim Introns = Szunyi.Location.Basic_Location_Manipulation.Get_Basic_LocationS(TS)
            Szunyi.Features.FeatureManipulation.GenBankMetaDataManipulation.AddFeatures(Seq, TS)

            Dim Intron_Finder As New Szunyi.Location.Basic_Location_Finder(Introns, True)
            Dim noFFound As Integer = 0
            Dim Final_Transcripts = Szunyi.Transcipts.Find_Transcripts.Get_Final_Transcripts(TSS_Finder, PAS_Finder, Intron_Finder, Introns, Me.PAs, TSS_PAS_TR.LocalWidth.Default_Value, TSS_PAS_TR.LocalWidth.Default_Value, 0)

            Dim str As New System.Text.StringBuilder
            For Each t In Final_Transcripts
                str.Append(t.Key).AppendLine()
            Next
            Dim TRSIII = Szunyi.Transcipts.Find_Transcripts.Create_Transcripts(TSS_Finder, PAS_Finder, Introns, Final_Transcripts)

            Dim CDSs = Szunyi.Features.FeatureManipulation.GetFeaturesByType.GetFeturesByTypeFromSeq(Seq, StandardFeatureKeys.CodingSequence)
            Dim Real_Named_TRs = Szunyi.Transcipts.Set_Transcipt_Names.Set_Real_Names(TRSIII, CDSs, Seq, Me.TSS_PAS_TR.Qulifier.Selected_Value, Me.Nof_AA)
            Szunyi.Features.FeatureManipulation.GenBankMetaDataManipulation.Remove_Feature(Seq, Real_Named_TRs)

            Szunyi.Transcipts.Set_Transcipt_Names.Set_L_AT_Variants(Real_Named_TRs)
            Szunyi.Transcipts.Set_Transcipt_Names.Set_Intron_Variants(Real_Named_TRs)
            Szunyi.Features.FeatureManipulation.GenBankMetaDataManipulation.AddFeatures(Seq, Real_Named_TRs)
            Dim nTR = Szunyi.Features.FeatureManipulation.Common.Clones(Real_Named_TRs)
            nTR = Szunyi.Features.FeatureManipulation.Key.ReName_Keys(nTR, "nTr")
            Szunyi.Features.FeatureManipulation.GenBankMetaDataManipulation.AddFeatures(Seq, nTR)
            Szunyi.IO.Export.SaveSequencesToSingleGenBank(Seq, New FileInfo(Basic_FileName & Seq.ID & "TSS-PAS-Tr-Counts-LSATs.gb"))

        End Sub
        Public Function Get_Introns() As List(Of Szunyi.mRNA.Transcript.TemplateSwitch)
            Dim Real_Introns As New List(Of Bio.IO.GenBank.ILocation)
            For Each loci In Me.PAs

                Dim Intron = Szunyi.Location.Common.Get_All_Intron_Location(loci.Location)
                Dim Exons = Szunyi.Location.Common.Get_All_Exon_Location(loci.Location)
                For i1 = 0 To Intron.Count - 1
                    Dim cIntron = Intron(i1)

                    Dim Intron_length = Szunyi.Location.Common.Get_Length(cIntron)
                    If Intron_length >= Me.Intron.Min_Intron_Length.Default_Value AndAlso Intron_length <= Me.Intron.Max_Intron_Length.Default_Value Then
                        Dim e1_length = Szunyi.Location.Common.Get_Length(Exons(i1))
                        If e1_length >= Me.Intron.Exon_Length.Default_Value Then
                            Dim e2_length = Szunyi.Location.Common.Get_Length(Exons(i1 + 1))
                            If e2_length >= Me.Intron.Exon_Length.Default_Value Then
                                Real_Introns.Add(cIntron)
                            End If
                        End If
                    End If

                Next
            Next
            Dim res As New List(Of Szunyi.mRNA.Transcript.TemplateSwitch)
            Dim Merged = Szunyi.Location.Merging.MergeLocations(Real_Introns, 0, Sort_Locations_By.TSS_PAS, 1)
            Dim Sw As New Bio.Algorithms.Alignment.SmithWatermanAligner
            Dim str As New System.Text.StringBuilder
            For Each I In Merged
                Dim t As New Szunyi.mRNA.Transcript.TemplateSwitch(Me.Seq, I)
                res.Add(t)

            Next
            Dim tmp As IEnumerable(Of Szunyi.mRNA.Transcript.TemplateSwitch)
            If Me.Intron.GTAG.Default_Value = 1 Then
                tmp = From x In res Where x.DonorSite = "GT" And x.AcceptorSite = "AG" And (x.Repeat_Length <= Me.Intron.Max_Repetition_Length.Default_Value Or x.FirstOffset <> x.LastOffset)
            Else
                tmp = From x In res Where x.Repeat_Length <= Me.Intron.Max_Repetition_Length.Default_Value Or x.FirstOffset <> x.LastOffset
            End If
            If tmp.Count > 0 Then
                Return tmp.ToList
            Else
                Return New List(Of mRNA.Transcript.TemplateSwitch)
            End If

        End Function


        Private Function Set_Introns(TRs As List(Of FeatureItem), TS As List(Of Szunyi.mRNA.Transcript.TemplateSwitch)) As List(Of FeatureItem)
            Dim wIntrons As New List(Of FeatureItem)
            For Each TR In TRs
                Dim PotIntrons = From x In TS Where x.Loci.LocationStart > TR.Location.LocationStart And x.Loci.LocationEnd < TR.Location.LocationEnd And x.Loci.IsComplementer = TR.Location.IsComplementer
                Dim Potential_Introns_Locations = From x In PotIntrons Select x.Loci
                If Potential_Introns_Locations.Count > 0 Then
                    For Each NonOVerlappingIntronGroup In Szunyi.Location.OverLapping_Locations.Get_Non_OverLappingGroups(Potential_Introns_Locations)
                        Dim s As String = "join(" & TR.Location.LocationStart & ".." & NonOVerlappingIntronGroup.First.LocationStart - 1
                        For i1 = 1 To NonOVerlappingIntronGroup.Count - 2
                            s = s & "," & NonOVerlappingIntronGroup(i1).LocationEnd + 1 & ".." & NonOVerlappingIntronGroup(i1 + 1).LocationStart - 1

                        Next
                        s = s & "," & NonOVerlappingIntronGroup.Last.LocationEnd + 1 & ".." & TR.Location.LocationEnd
                        If TR.Location.IsComplementer = True Then s = "complement" & s & ")"
                        Dim tr1 As New FeatureItem(TR.Key, Szunyi.Location.Common.Get_Location(s))
                        wIntrons.Add(tr1)
                    Next

                End If

            Next
            Return wIntrons
        End Function
        Private Function Count_Transcripts(TRs As List(Of FeatureItem)) As List(Of FeatureItem)
            Dim c10 As New Szunyi.RNASeq.Count.Counter(Seq, Me.CountBy, Me.PAs, TRs)
            c10.DoIt(True)
            ' Reads With Features One To Many Order by mRNA size Decreasing
            Dim out As New List(Of FeatureItem)
            For Each Item In c10.mRNAwReads
                out.Add(Item.Key)
            Next
            TRs = out
            Dim ToRemove As New List(Of FeatureItem)
            For Each Item In c10.mRNAwReads
                If Item.Value.Count > 0 Then
                    Szunyi.Features.FeatureManipulation.Qulifiers.Add(Item.Key, StandardQualifierNames.Note, Item.Value.Count)
                    Dim h As Int16 = 54
                Else
                    ToRemove.Add(Item.Key)
                End If

            Next
            For Each Item In ToRemove
                Szunyi.Features.FeatureManipulation.GenBankMetaDataManipulation.Remove_Feature(Me.Seq, Item)
                TRs.Remove(Item)
            Next
            Szunyi.IO.Export.SaveSequencesToSingleGenBank(Seq, New FileInfo(Dir.FullName & "\" & Seq.ID & "TSS-PAS-Trs-Counts.gb"))
            Return out
            Dim kj As Int16 = 65
        End Function

        Public Shared Function Do_Poisson(Folder As System.IO.DirectoryInfo,
                                             Local_width As Integer,
                                             WindowSize As Integer,
                                             Mappings As List(Of Szunyi.Location.Basic_Location),
                                             Sort As Sort_Locations_By,
                                             seq As Bio.ISequence,
                                             Type As String,
                                             File As System.IO.FileInfo,
                                             Enums As List(Of Integer)) As List(Of Szunyi.Stat.Distribution_Result)

            Dim Sites = Szunyi.PacBio.Pacbio_Transcript_Shared.Get_Sites(Mappings, seq, Sort)
            Dim Nfile = Szunyi.IO.Files.Get_New_FileName.Replace_Extension(File, ".tsv")

            Dim All_Sites = Szunyi.PacBio.Pacbio_Transcript_Shared.TSSs_PASs_ToString(Sites)
            Szunyi.IO.Export.SaveText(All_Sites, Nfile)
            Dim PE As New Szunyi.PoissonEvaluator
            Dim Merged = Szunyi.Location.Merging.MergeLocations(Mappings, Local_width, Sort, 1) ' As List(Of List(Of Szunyi.Location.Basic_Location))
            Dim For_Test As New List(Of Basic_Location)
            For Each M In Merged
                If M.Count > 1 Then
                    For_Test.Add(Szunyi.Location.Basic_Location_Manipulation.Get_Most_Abundants(M, Sort).First)
                End If
            Next
            Dim Locis = From x In Mappings Select x.Location
            If Sort = Sort_Locations_By.TSS Then
                Return Szunyi.BAM.Convert.Table.TSS(For_Test, WindowSize, Local_width, 0.05, seq, File, Locis.ToList, Enums)
            ElseIf Sort = Sort_Locations_By.PAS Then
                Return Szunyi.BAM.Convert.Table.PAS(For_Test, WindowSize, Local_width, 0.05, seq, File, Locis.ToList, Enums)
            Else
                Return Nothing
            End If


        End Function
        Public Shared Function Merge_Poisson(res As Dictionary(Of FileInfo, List(Of Szunyi.Stat.Distribution_Result)), width As Integer) As List(Of List(Of Szunyi.Stat.Distribution_Result))
            Dim Result As New List(Of List(Of Szunyi.Stat.Distribution_Result))
            Dim All As New List(Of Szunyi.Stat.Distribution_Result)
            For Each F In res
                For Each V In F.Value
                    All.Add(V)
                Next
            Next

            ' Work Only For Local Max and Passed TSS OR PAS
            Dim Valids = From x In All Where x.Is_Local_Maximum = True And x.Passed = True

            Dim Sorted = From x In Valids Order By x.Count Ascending, x.Count Descending

            ' Do Merging
            Dim Used As New List(Of Szunyi.Stat.Distribution_Result)
            For Each p In Sorted
                Dim similar = From x In Sorted Where x.Index - width <= p.Index And x.Index + width >= p.Index And x.IsComplementer = p.IsComplementer

                Dim currents As New List(Of Szunyi.Stat.Distribution_Result)
                For Each s In similar
                    If Used.Contains(s) = False Then
                        Used.Add(s)
                        currents.Add(s)
                    End If
                Next
                If currents.Count > 0 Then
                    Dim tmp As New List(Of Szunyi.Stat.Distribution_Result)
                    tmp.AddRange(currents)
                    Result.Add(tmp)
                End If
            Next

            Return Result

        End Function

        Public Shared Function Merge_Poisson(res As Dictionary(Of FileInfo, Poisson_Result), width As Integer) As List(Of List(Of Poisson))
            Dim Result As New List(Of List(Of Poisson))
            Dim All As New List(Of Poisson)
            For Each F In res
                For Each V In F.Value.Poissons
                    All.Add(V)
                Next
            Next

            ' Work Only For Local Max and Passed TSS OR PAS
            Dim Valids = From x In All Where x.Is_Local_Maximum = True And x.Passed = True

            Dim Sorted = From x In Valids Order By x.p Ascending, x.Count Descending

            ' Do Merging
            Dim Used As New List(Of Poisson)
            For Each p In Sorted
                Dim similar = From x In Sorted Where x.Index - width <= p.Index And x.Index + width >= p.Index And x.IsComplementer = p.IsComplementer

                Dim currents As New List(Of Poisson)
                For Each s In similar
                    If Used.Contains(s) = False Then
                        Used.Add(s)
                        currents.Add(s)
                    End If
                Next
                If currents.Count > 0 Then
                    Dim tmp As New List(Of Poisson)
                    tmp.AddRange(currents)
                    Result.Add(tmp)
                End If
            Next

            Return Result

        End Function

        Public Shared Function Set_Notes(GBKs As List(Of Bio.ISequence)) As List(Of FeatureItem)
            Dim ext As New List(Of Szunyi.ListOf.ExtFeature)
            For Each GBK In GBKs
                Dim cnTRs = Szunyi.Features.FeatureManipulation.GetFeaturesByType.GetFeturesByTypeFromSeq(GBK, "nTr")
                ext.AddRange(Szunyi.Features.ExtFeatureManipulation.GetExtFeaturesFromFeature(cnTRs, GBK))
            Next
            Dim TRs As New List(Of FeatureItem)
            For Each g In Szunyi.Features.ExtFeatureManipulation.Parse.ByLocationString(ext)
                Dim ls As New List(Of String)
                Dim Count As Integer = 0
                For Each Feat In g
                    Count += Feat.Feature.Qualifiers(StandardQualifierNames.Note).First
                Next
                ls.Add(Count)
                g.First.Feature.Qualifiers(StandardQualifierNames.Note) = ls
                Dim st As String = ""
                For Each Seq As Bio.ISequence In GBKs
                    Dim t = From a1 In g Where Seq.ID = a1.Seq.ID

                    If t.Count = 0 Then
                        st = st & "-"
                    Else
                        st = st & "+"
                    End If
                Next
                Dim ls2 As New List(Of String)
                ls2.Add(st)
                g.First.Feature.Qualifiers(StandardQualifierNames.CloneLibrary) = ls2
                TRs.Add(g.First.Feature)
            Next
            Return TRs
        End Function
    End Class

    Public Class TSS_Distribution_Setting
        Public Property Name As String
        Public Property Width As Integer
        Public Overrides Function ToString() As String
            Return "#TSS Distribution Setting:Name=" & Name & " Width=" & Width
        End Function
    End Class
    Public Class PAS_Distribution_Setting
        Public Property Name As String
        Public Property Width As Integer
        Public Overrides Function ToString() As String
            Return "#PAS Distribution Setting:Name=" & Name & " Width=" & Width
        End Function
    End Class
    Public Class TSS_Poisson
        Public Property Name As String
        Public Property Local_Width As Integer
        Public Property Width As Integer
        Public Property p_value As Double
        Public Overrides Function ToString() As String
            Return "#TSS Poisson Setting:Name=" & Name & " Width=" & Width & " Local Width=" & Local_Width & " p-value=" & p_value
        End Function
    End Class
    Public Class PAS_Poisson
        Public Property Name As String
        Public Property Local_Width As Integer
        Public Property Width As Integer
        Public Property p_value As Double
        Public Overrides Function ToString() As String
            Return "#PAS Poisson Setting:Name=" & Name & " Width=" & Width & " Local Width=" & Local_Width & " p-value=" & p_value
        End Function
    End Class
    Public Class PolyA_Signal
        Public Property Name As String
        Public Property Best_Position As Integer
        Public Property Width As Integer
        Public Overrides Function ToString() As String
            Return "#PolyA Signal Setting:Name=" & Name & " Width=" & Width & " Best Position=" & Best_Position
        End Function
    End Class
    Public Class A_T_Before_PAS
        Public Property Name As String
        Public Property Width As Integer
        Public Overrides Function ToString() As String
            Return "#PolyA Signal Setting:Name=" & Name & " Width=" & Width
        End Function
    End Class
    Public Class Cistronic_Complex

    End Class
    Public Class Find_Transcripts


        Public Shared Function Get_Final_Transcripts(TSS_Finder As Szunyi.Location.Basic_Location_Finder,
                                               PAS_Finder As Szunyi.Location.Basic_Location_Finder,
                                               Intron_Finder As Szunyi.Location.Basic_Location_Finder,
                                               Introns As List(Of Basic_Location),
                                               SAMs_BLs As IEnumerable(Of Basic_Location),
                                              TSS_Width As Integer,
                                              TES_Width As Integer,
                                              Intron_Width As Integer) As SortedList(Of String, Integer)



            Dim noFFound As Integer = 0
            Dim Final_Transcripts As New SortedList(Of String, Integer)
            Dim nof_Found As Integer = 0
            Dim nof_Not_Found As Integer = 0
            For Each Item In SAMs_BLs
                ' Dim cTSS = TSS_Finder.Find_Index_byLoci(Item, TSS_Width, Sort_Locations_By.TSS)
                ' Dim cPAS = PAS_Finder.Find_Index_byLoci(Item, TES_Width, Sort_Locations_By.PAS)
                Dim exon = Szunyi.Location.Common.Get_Biggest_Exon_Length(Item.Location)
                Dim Intron = Szunyi.Location.Common.Get_Biggest_Intron_Length(Item.Location)
                If exon > 20000 Then
                    Dim jk As Int16 = 65
                End If
                Dim cTSS = TSS_Finder.Find_Index_byLoci(Item, TSS_Width, Sort_Locations_By.LS, True)
                Dim cPAS = PAS_Finder.Find_Index_byLoci(Item, TES_Width, Sort_Locations_By.LE, True)
                Get_nTRs(cTSS, cPAS, TSS_Finder, PAS_Finder, Intron_Finder, Intron_Width, Introns, Item, TSS_Width, TES_Width, Final_Transcripts, "LSLET-")

                cTSS = TSS_Finder.Find_Index_byLoci(Item, TSS_Width, Sort_Locations_By.LS, False)
                cPAS = PAS_Finder.Find_Index_byLoci(Item, TES_Width, Sort_Locations_By.LE, False)
                Get_nTRs(cTSS, cPAS, TSS_Finder, PAS_Finder, Intron_Finder, Intron_Width, Introns, Item, TSS_Width, TES_Width, Final_Transcripts, "LSLEF-")

                cTSS = TSS_Finder.Find_Index_byLoci(Item, TSS_Width, Sort_Locations_By.LE, True)
                cPAS = PAS_Finder.Find_Index_byLoci(Item, TES_Width, Sort_Locations_By.LS, True)
                Get_nTRs(cTSS, cPAS, TSS_Finder, PAS_Finder, Intron_Finder, Intron_Width, Introns, Item, TSS_Width, TES_Width, Final_Transcripts, "LELST-")

                cTSS = TSS_Finder.Find_Index_byLoci(Item, TSS_Width, Sort_Locations_By.LE, False)
                cPAS = PAS_Finder.Find_Index_byLoci(Item, TES_Width, Sort_Locations_By.LS, False)
                Get_nTRs(cTSS, cPAS, TSS_Finder, PAS_Finder, Intron_Finder, Intron_Width, Introns, Item, TSS_Width, TES_Width, Final_Transcripts, "LELST-")

            Next
            Return Final_Transcripts
        End Function

        Private Shared Sub Get_nTRs(cTSS As List(Of Integer), cPAS As List(Of Integer),
                                  tSS_Finder As Basic_Location_Finder, pAS_Finder As Basic_Location_Finder, intron_Finder As Basic_Location_Finder,
                                  intron_Width As Integer, Introns As List(Of Basic_Location), Item As Basic_Location,
                                  TSS_Width As Integer, TES_Width As Integer,
                                  Final_Transcripts As SortedList(Of String, Integer), Key As String)

            Dim cIntrons = Szunyi.Location.Common.Get_All_Intron_Location(Item.Location)
            Dim BL_Introns = Szunyi.Location.Basic_Location_Manipulation.Get_Basic_LocationS(cIntrons)
            If cIntrons.Count > 0 Then
                Dim IntronIDs = Get_IntronIDs(intron_Finder, intron_Width, cIntrons, Introns, BL_Introns)
                If IntronIDs.Count <> 0 Then
                    If cPAS.Count = 0 Or cTSS.Count = 0 Then

                    ElseIf cPAS.Count = 1 And cTSS.Count = 1 Then
                        Key = Key & cTSS.First & "-" & cPAS.First & "-" & Szunyi.Text.General.GetText(IntronIDs, "-")
                        If Final_Transcripts.ContainsKey(Key) = False Then Final_Transcripts.Add(Key, 0)
                        Final_Transcripts(Key) += 1
                    Else

                    End If
                End If

            Else
                If cPAS.Count = 0 Or cTSS.Count = 0 Then
                    Dim jj As Int16 = 54
                ElseIf cPAS.Count = 1 And cTSS.Count = 1 Then
                    Key = Key & cTSS.First & "-" & cPAS.First
                    If Final_Transcripts.ContainsKey(Key) = False Then Final_Transcripts.Add(Key, 0)
                    Final_Transcripts(Key) += 1
                Else

                End If
            End If

        End Sub
        ''' <summary>
        ''' List of introns index if all intron is founded or empty list
        ''' </summary>
        ''' <param name="intron_Finder"></param>
        ''' <param name="intron_Width"></param>
        ''' <param name="cIntrons"></param>
        ''' <param name="Introns"></param>
        ''' <param name="BL_Introns"></param>
        ''' <returns></returns>
        Private Shared Function Get_IntronIDs(intron_Finder As Basic_Location_Finder, intron_Width As Integer,
                                       cIntrons As List(Of ILocation), Introns As List(Of Basic_Location), BL_Introns As List(Of Basic_Location)) As List(Of Integer)
            Dim IntronIDs As New List(Of Integer)
            For Each cIntron In BL_Introns
                Dim cI1 = intron_Finder.Find_Items_byLoci(cIntron, intron_Width, Sort_Locations_By.LS)
                Dim cI2 = intron_Finder.Find_Items_byLoci(cIntron, intron_Width, Sort_Locations_By.LE)
                If IsNothing(cI1) = False AndAlso IsNothing(cI2) = False Then
                    Dim Common = cI1.Intersect(cI2)
                    If Common.Count = 1 Then
                        Dim Index = Introns.IndexOf(Common.First)
                        IntronIDs.Add(Introns.IndexOf(Common.First))
                    Else
                        IntronIDs.Clear()
                        Exit For
                    End If
                Else
                    IntronIDs.Clear()
                    Exit For
                End If
            Next
            IntronIDs = IntronIDs.Distinct.ToList
            IntronIDs.Sort()
            Return IntronIDs
        End Function

        Public Shared Function Create_Transcripts(TSS_Finder As Basic_Location_Finder, PAS_Finder As Basic_Location_Finder, Introns As List(Of Basic_Location), Final_Transcripts As SortedList(Of String, Integer)) As List(Of FeatureItem)
            Dim out As New List(Of FeatureItem)
            For Each Item In Final_Transcripts
                Dim s1 = Split(Item.Key, "-")

                Dim TSS = Get_TSS_Start(s1.First, s1(1), TSS_Finder)
                Dim TSS_Orientation = Get_TSS_Start_Orientation(s1.First, s1(1), TSS_Finder)
                Dim PAS = Get_PAS_Start(s1.First, s1(2), PAS_Finder)
                Dim PAS_Orientation = Get_PAS_Start_Orientation(s1.First, s1(2), PAS_Finder)
                Dim isComplementer As Boolean
                If s1.First.EndsWith("T") Then
                    isComplementer = True
                Else
                    isComplementer = False
                End If
                If (isComplementer = False And TSS < PAS And TSS_Orientation = False And PAS_Orientation = False) Or (isComplementer = True And PAS < TSS And TSS_Orientation = True And PAS_Orientation = True) Then
                    If s1.Count = 3 Then ' No Intron
                        Dim l = Szunyi.Location.Common.GetLocation(TSS, PAS, isComplementer)
                        out.Add(New FeatureItem("tr", l))
                        Szunyi.Features.FeatureManipulation.Qulifiers.Add(out.Last, StandardQualifierNames.IdentifiedBy, Item.Key)
                        Szunyi.Features.FeatureManipulation.Qulifiers.Add(out.Last, StandardQualifierNames.Note, Item.Value)
                        out.Last.Label = Item.Value ' Count

                    Else
                        Dim cIntrons As New List(Of Basic_Location)
                        For i1 = 3 To s1.Count - 1
                            cIntrons.Add(Introns(s1(i1)))
                        Next
                        cIntrons = (From x In cIntrons Order By x.Location.LocationStart).ToList
                        Dim s As String = ""
                        If isComplementer = True Then
                            s = "join(" & PAS & ".." & cIntrons.First.Location.LocationStart - 1
                            For i1 = 0 To cIntrons.Count - 2
                                s = s & "," & cIntrons(i1).Location.LocationEnd + 1 & ".." & cIntrons(i1 + 1).Location.LocationStart - 1

                            Next
                            s = s & "," & cIntrons.Last.Location.LocationEnd + 1 & ".." & TSS
                            s = "complement(" & s & ")"
                        Else
                            s = "join(" & TSS & ".." & cIntrons.First.Location.LocationStart - 1
                            For i1 = 0 To cIntrons.Count - 2
                                s = s & "," & cIntrons(i1).Location.LocationEnd + 1 & ".." & cIntrons(i1 + 1).Location.LocationStart - 1

                            Next
                            s = s & "," & cIntrons.Last.Location.LocationEnd + 1 & ".." & PAS
                        End If

                        Dim tr1 As New FeatureItem("tr", Szunyi.Location.Common.Get_Location(s))
                        out.Add(tr1)
                        Szunyi.Features.FeatureManipulation.Qulifiers.Add(out.Last, StandardQualifierNames.IdentifiedBy, Item.Key)
                        Szunyi.Features.FeatureManipulation.Qulifiers.Add(out.Last, StandardQualifierNames.Note, Item.Value)
                        out.Last.Label = Item.Value ' |Count
                    End If
                Else
                    Dim jj As Int16 = 54
                End If
            Next
            Return out
        End Function
        Private Shared Function Get_PAS_Start(LocType As String, Index As String, PAS_Finder As Basic_Location_Finder) As Integer
            Dim s = LocType.Substring(2, 2)
            If s = "LS" Then
                Return PAS_Finder.By_Location_Start(Index).Location.LocationStart
            ElseIf s = "LE" Then
                Return PAS_Finder.By_Location_End(Index).Location.LocationEnd
            End If
        End Function
        Private Shared Function Get_PAS_Start_Orientation(LocType As String, Index As String, PAS_Finder As Basic_Location_Finder) As Boolean
            Dim s = LocType.Substring(2, 2)
            If s = "LS" Then
                Return PAS_Finder.By_Location_Start(Index).Location.IsComplementer
            ElseIf s = "LE" Then
                Return PAS_Finder.By_Location_End(Index).Location.IsComplementer
            End If
        End Function
        Private Shared Function Get_TSS_Start(LocType As String, Index As String, TSS_Finder As Basic_Location_Finder) As Integer
            Dim s = LocType.Substring(0, 2)
            If s = "LS" Then
                Return TSS_Finder.By_Location_Start(Index).Location.LocationStart
            ElseIf s = "LE" Then
                Return TSS_Finder.By_Location_End(Index).Location.LocationEnd
            End If
        End Function
        Private Shared Function Get_TSS_Start_Orientation(LocType As String, Index As String, TSS_Finder As Basic_Location_Finder) As Boolean
            Dim s = LocType.Substring(0, 2)
            If s = "LS" Then
                Return TSS_Finder.By_Location_Start(Index).Location.IsComplementer
            ElseIf s = "LE" Then
                Return TSS_Finder.By_Location_End(Index).Location.IsComplementer
            End If
        End Function
    End Class
    Public Class Set_Transcipt_Names
        Public Shared Function Set_Real_Names(Feats As List(Of FeatureItem), CDSs As List(Of FeatureItem), Seq As Bio.ISequence, Qual As String, Nof_AA As Integer) As List(Of FeatureItem)

            Dim trs = Szunyi.Features.FeatureManipulation.Common.Clones(Feats)

            For Each Tr In trs

                Dim cCDSs_woOrientation = Szunyi.Location.OverLapping_Locations.Get_Inner_Feature_Items_woOrientation(Tr, CDSs)
                Dim cCDSs_wOrientation = Szunyi.Location.OverLapping_Locations.Get_Inner_Feature_Items_wOrientation(Tr, CDSs)
                If cCDSs_wOrientation.Count = 0 Then ' No Fully Overlapped CDS
                    Dim truncated_S = Szunyi.Location.OverLapping_Locations.Get_Longest_OverLapping_Item_wOrientation(Tr, CDSs)
                    Dim truncated_AS = Szunyi.Location.OverLapping_Locations.Get_Longest_OverLapping_Item_woOrientation(Tr, CDSs)
                    If IsNothing(truncated_S) = True Then '? AS
                        If IsNothing(truncated_AS) = True Then
                            Tr.Key = "TR-NC"
                            Tr.Label = Szunyi.Location.Common.GetLocationString(Tr.Location)
                            Szunyi.Features.FeatureManipulation.Qulifiers.Add(Tr, StandardQualifierNames.Allele, Szunyi.Location.Common.GetLocationString(Tr.Location))
                        Else
                            Tr.Key = "TR-AS"
                            Tr.Label = (truncated_AS.Qualifiers(Qual).First).Replace(Chr(34), "").ToUpper & "-AS"
                            Szunyi.Features.FeatureManipulation.Qulifiers.Add(Tr, StandardQualifierNames.Allele, Szunyi.Location.Common.GetLocationString(truncated_AS.Location))
                        End If
                    Else ' This is truncated Sense
                        Set_Truncated(truncated_S, truncated_AS, Tr, Seq, Qual, Nof_AA)
                    End If
                ElseIf cCDSs_woOrientation.Count = 1 Then ' Only 1 
                    If cCDSs_woOrientation.Count = cCDSs_wOrientation.Count Then ' Ha Sense
                        Tr.Key = "TR-MONO"
                        Tr.Label = (cCDSs_woOrientation.First.Qualifiers(Qual).First).Replace(Chr(34), "").ToUpper
                        Szunyi.Features.FeatureManipulation.Qulifiers.Add(Tr, StandardQualifierNames.Allele, Szunyi.Location.Common.GetLocationString(cCDSs_woOrientation.First.Location))
                    Else
                        Tr.Key = "TR-AS"
                        Tr.Label = (cCDSs_woOrientation.First.Qualifiers(Qual).First).Replace(Chr(34), "").ToUpper
                        Szunyi.Features.FeatureManipulation.Qulifiers.Add(Tr, StandardQualifierNames.Allele, Szunyi.Location.Common.GetLocationString(cCDSs_woOrientation.First.Location))
                    End If

                Else

                    If cCDSs_wOrientation.Count = cCDSs_woOrientation.Count Then ' Poly
                        Tr.Key = "POLY-" & cCDSs_woOrientation.Count
                        Dim tmp = From x In cCDSs_woOrientation Order By x.Location.LocationStart Ascending

                        Dim Names = (From x In tmp Select x.Qualifiers(Qual).First).ToList
                        If Tr.Location.IsComplementer = True Then
                            Names.Reverse()
                        End If
                        Szunyi.Features.FeatureManipulation.Qulifiers.Add(Tr, StandardQualifierNames.Allele, Szunyi.Location.Common.GetLocationString(cCDSs_wOrientation.First.Location))
                        Tr.Label = Szunyi.Text.General.GetText(Names, "-").Replace(Chr(34), "").ToUpper

                    Else ' Complex
                        Tr.Key = "COMPLEX" & cCDSs_woOrientation.Count
                        Dim tmp = From x In cCDSs_woOrientation Order By x.Location.LocationStart Ascending

                        Dim Names = (From x In tmp Select x.Qualifiers(Qual).First).ToList
                        If Tr.Location.IsComplementer = True Then Names.Reverse()
                        Szunyi.Features.FeatureManipulation.Qulifiers.Add(Tr, StandardQualifierNames.Allele, Szunyi.Location.Common.GetLocationString(cCDSs_wOrientation.First.Location))
                        Tr.Label = Szunyi.Text.General.GetText(Names, "-").Replace(Chr(34), "").ToUpper


                    End If
                End If
                ' trs.Add(Tr)
            Next

            Return trs
        End Function
        Private Shared Sub Set_Truncated(truncated_S As FeatureItem, truncated_AS As FeatureItem, Tr As FeatureItem, Seq As Bio.ISequence, Qual As String, Nof_AA As Integer)
            If Tr.Location.IsComplementer = False Then
                If Tr.Location.TSS > truncated_S.Location.TSS And Tr.Location.LocationEnd >= truncated_S.Location.LocationEnd Then
                    Set_Point5(Tr, truncated_S, Seq, Qual, Nof_AA)
                Else
                    Tr.Key = "TR-NC"
                    Tr.Label = (truncated_S.Qualifiers(Qual).First).Replace(Chr(34), "").ToUpper & "-NC"
                    Szunyi.Features.FeatureManipulation.Qulifiers.Add(Tr, StandardQualifierNames.Allele, Szunyi.Location.Common.GetLocationString(truncated_S.Location))
                End If

            Else
                If Tr.Location.TSS < truncated_S.Location.TSS And Tr.Location.PAS <= truncated_S.Location.PAS Then
                    Set_Point5(Tr, truncated_S, Seq, Qual, Nof_AA)
                Else
                    Tr.Key = "TR-NC"
                    Tr.Label = (truncated_S.Qualifiers(Qual).First).Replace(Chr(34), "").ToUpper & "-NC"
                    Szunyi.Features.FeatureManipulation.Qulifiers.Add(Tr, StandardQualifierNames.Allele, Szunyi.Location.Common.GetLocationString(truncated_S.Location))
                End If

            End If
        End Sub
        Private Shared Sub Set_Point5(tr As FeatureItem, truncated_S As FeatureItem, seq As Bio.ISequence, Qual As String, Nof_AA As Integer)
            Dim tr1 As String = ""
            Dim Orfs As New List(Of Szunyi.DNA.ORF)
            If tr.Location.IsComplementer = False Then
                Dim tSeq = seq.GetSubSequence(tr.Location.LocationStart, tr.Location.LocationEnd - tr.Location.LocationStart)
                Orfs = Szunyi.DNA.ORF_Finding.Get_All_ORFs(tSeq, DNA.Frames.fr, True, True)
                Dim TheCDS = truncated_S.GetSubSequence(seq)
                tr1 = Szunyi.DNA.Translate.Translate(TheCDS).ConvertToString
            Else
                Dim tSeq = seq.GetSubSequence(tr.Location.LocationStart, tr.Location.LocationEnd - tr.Location.LocationStart).GetReverseComplementedSequence
                Orfs = Szunyi.DNA.ORF_Finding.Get_All_ORFs(tSeq, DNA.Frames.fr, True, True)
                Dim TheCDS = truncated_S.GetSubSequence(seq).GetReversedSequence
                tr1 = Szunyi.DNA.Translate.Translate(TheCDS).ConvertToString
            End If

            Dim IsPoint5 As Boolean = False
            For Each Item In Orfs
                If Item.AASeq.Count >= Nof_AA Then
                    If tr1.Contains(Item.AASeq.ConvertToString) Then
                        tr.Key = "TR-.5"
                        tr.Label = (truncated_S.Qualifiers(Qual)).First.Replace(Chr(34), "").ToUpper & ".5"
                        Szunyi.Features.FeatureManipulation.Qulifiers.Add(tr, StandardQualifierNames.Allele, Szunyi.Location.Common.GetLocationString(truncated_S.Location))
                        IsPoint5 = True
                        Exit For
                    End If
                End If

            Next
            If IsPoint5 = False Then
                tr.Key = "TR-NC"
                tr.Label = (truncated_S.Qualifiers(Qual).First).Replace(Chr(34), "").ToUpper & "-NC"
                Szunyi.Features.FeatureManipulation.Qulifiers.Add(tr, StandardQualifierNames.Allele, Szunyi.Location.Common.GetLocationString(truncated_S.Location))
            End If
        End Sub
        Public Shared Sub Set_L_AT_Variants(TRs As List(Of FeatureItem))

            For Each Gr In Common.ByKeyByLabelByAllele(TRs)
                If Gr.Count > 1 Then
                    Dim Abundant = Common.MostAbundant(Gr)
                    Dim L_List = (From x In Gr Select x.Location.TSS).ToList
                    L_List = L_List.Distinct.ToList
                    L_List.Sort()
                    Dim AT_LIst = (From x In Gr Select x.Location.PAS).ToList
                    AT_LIst = AT_LIst.Distinct.ToList
                    AT_LIst.Sort()

                    Dim L = L_List.IndexOf(Abundant.Location.TSS)
                    Dim AT = AT_LIst.IndexOf(Abundant.Location.PAS)
                    If Abundant.Location.IsComplementer = False Then
                        For Each Item In Gr
                            Dim C_L = L_List.IndexOf(Item.Location.TSS)
                            If C_L < L Then
                                Item.Label = "L" & L - C_L & " " & Item.Label
                            ElseIf C_L > L Then
                                Item.Label = "S" & C_L - L & " " & Item.Label
                            Else
                                Dim kj As Int16 = 65
                            End If
                        Next
                        For Each Item In Gr
                            Dim c_AT = AT_LIst.IndexOf(Item.Location.PAS)
                            If c_AT < AT Then ' AT-S
                                Item.Label = Item.Label & " AT-S" & AT - c_AT
                            ElseIf c_AT > AT Then 'AT-L
                                Item.Label = Item.Label & " AT-L" & c_AT - AT
                            Else
                                Dim kj As Int16 = 65
                            End If
                        Next


                    Else
                        For Each Item In Gr
                            Dim C_L = L_List.IndexOf(Item.Location.TSS)
                            If C_L < L Then
                                Item.Label = "S" & L - C_L & "  " & Item.Label
                            ElseIf C_L > L Then
                                Item.Label = "L" & C_L - L & "  " & Item.Label
                            Else
                                Dim kj As Int16 = 65
                            End If

                        Next
                        For Each Item In Gr
                            Dim c_AT = AT_LIst.IndexOf(Item.Location.PAS)
                            If c_AT < AT Then ' AT-S
                                Item.Label = Item.Label & " AT-L" & AT - c_AT
                            ElseIf c_AT > AT Then 'AT-L
                                Item.Label = Item.Label & " AT-S" & c_AT - AT
                            Else
                                Dim kj As Int16 = 54
                            End If
                        Next
                    End If
                End If


            Next

        End Sub
        Public Shared Sub Set_Intron_Variants(TRs As List(Of FeatureItem))
            For Each Gr In Common.ByKeyByLabel(TRs) ' By Key By Label
                If Gr.Count > 1 Then
                    Dim Index As Integer = 1
                    For Each Item In Gr
                        If Szunyi.Location.Common.Get_All_Exon_Location(Item.Location).Count > 1 Then ' Intronic
                            Item.Label = Item.Label & " Sp-" & Index
                            Index += 1
                        End If

                    Next
                End If
            Next
        End Sub
        Public Class Set_Splice_Variants

        End Class
        Public Class Common
            ''' <summary>
            ''' return the most abundant transcripts (information is stored in note qulifier)
            ''' </summary>
            ''' <param name="TRs"></param>
            ''' <returns></returns>
            Public Shared Function MostAbundant(TRs As List(Of FeatureItem)) As FeatureItem
                Dim max = 0
                Dim c As FeatureItem
                For Each Item In TRs
                    If Item.Qualifiers.ContainsKey(StandardQualifierNames.Note) = True Then
                        Dim j As Integer = Item.Qualifiers(StandardQualifierNames.Note).First
                        If j > max Then
                            max = j
                            c = Item
                        End If
                    Else
                        Dim jk As Int16 = 65
                    End If

                Next
                Return c

            End Function
            ''' <summary>
            ''' Label is the name of transcripts
            ''' </summary>
            ''' <param name="TRs"></param>
            ''' <returns></returns>
            Public Shared Iterator Function ByKeyByLabel(TRs As List(Of FeatureItem)) As IEnumerable(Of List(Of FeatureItem))

                Dim res = From x In TRs Group By x.Key, x.Label Into Group

                For Each r In res
                    Yield r.Group.ToList
                Next

            End Function

            Public Shared Iterator Function ByKeyByLabelByAllele(TRs As List(Of FeatureItem)) As IEnumerable(Of List(Of FeatureItem))

                Dim res = From x In TRs Group By x.Key, x.Label, x.Qualifiers(StandardQualifierNames.Allele).First Into Group

                For Each r In res
                    Yield r.Group.ToList
                Next
            End Function
        End Class
    End Class
End Namespace

