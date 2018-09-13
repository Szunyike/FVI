Imports System.IO
Imports System.Text.RegularExpressions
Imports Bio.IO.SAM
Imports ClassLibrary1.Szunyi.Alignment

Namespace Szunyi.BAM.Stat
    Public Class Stat
        Public Shared Function Get_Header_By_ORGs(RefSeqs As List(Of Bio.IO.SAM.ReferenceSequenceInfo)) As String
            Dim Header = ("Nof Read,Nof Mapping,Median Read Length,Avr. Read Length,Avr. Read Length SD,Median Alignment Length,Avr. Alignment Length,Avr. Aligment Length SD,Deleteion %,Deleteion % SD,Insertion %,Insertion % SD,Match%,Match% SD,MisMatch%,MisMatch% SD,Coverage")
            Dim str As New System.Text.StringBuilder

            Dim Headers As New List(Of String)
            Headers.Add("File Name")
            For Each RefSeq In RefSeqs
                Dim s = Split(Header.Replace(",", " of " & RefSeq.Name & ","), ",")
                s(s.Count - 1) = s.Last & " of " & RefSeq.Name
                Headers.AddRange(s)
            Next
            Return Szunyi.Text.General.GetText(Headers, vbTab)

        End Function
        Public Shared Function All_By_ORG(Sams As List(Of Bio.IO.SAM.SAMAlignedSequence),
                                          File As FileInfo,
                                          RefSeqs As List(Of Bio.IO.SAM.ReferenceSequenceInfo)) As String
            Dim str As New System.Text.StringBuilder
            Dim out As New List(Of String)
            out.Add(File.Name)
            Dim Header = ("Nof Read,Nof Mapping,Median Read Length,Avr. Read Length,Avr. Read Length SD,Median Alignment Length,Avr. Alignment Length,Avr. Aligment Length SD,Deleteion %,Deleteion % SD,Insertion %,Insertion % SD,Match%,Match% SD,MisMatch%,MisMatch% SD,Coverage")

            For Each RefSeq In RefSeqs
                Dim ByORG = From x In Sams Where x.RName = RefSeq.Name
                If ByORG.Count > 0 Then

                    out.Add(NofRead(ByORG))
                    out.Add(NofMappings(ByORG))
                    out.Add(Median_Read_Lengths(ByORG))
                    out.Add(Avr_Read_Lengths(ByORG))
                    out.Add(Avr_Read_Lengths_SD(ByORG))
                    out.Add(Get_Median_Alignment_Length(ByORG))
                    out.Add(Get_Avr_Alignment_Length(ByORG))
                    out.Add(Get_Avr_Alignment_Length_SD(ByORG))
                    '    out.AddRange(Get_Match_MisMatch_Insertion_Deletions_SD(ByORG))
                    out.Add(Coverage(ByORG, File))
                Else
                    out.Add("")
                    out.Add("")
                    out.Add("")
                    out.Add("")
                    out.Add("")
                    out.Add("")
                    out.Add("")
                    out.Add("")
                    out.Add("")
                    out.Add("")
                    out.Add("")
                    out.Add("")
                    out.Add("")
                    out.Add("")
                    out.Add("")
                    out.Add("")
                    out.Add("")
                End If

            Next

            Return ClassLibrary1.Szunyi.Text.General.GetText(out, vbTab)
        End Function
        Public Shared Function All(Sams As List(Of Bio.IO.SAM.SAMAlignedSequence), File As FileInfo) As String
            Dim Header = Split("File Name, Nof Read, Nof Mapping, Avr. Read Length,Avr. Read Length SD, Avr. Aligned Read Length,
                            Avr. Aligned Read Length SD,Deleteion %, Deleteion % SD, Insertion %,Insertion % SD, Match%,Match% SD, MisMatch%, MisMatch% SD,Coverage", ",")
            'Dim Header = Split("File Name,Nof Read,Nof Mapping,Median Read Length,Avr. Read Length,Avr. Read Length SD,Median Aligned Read Length,Avr. Aligned Read Length,Avr. Aligned Read Length SD,Deleteion %,Deleteion % SD,Insertion %,Insertion % SD,Match%,Match% SD,MisMatch%,MisMatch% SD,Coverage", ",")

            Dim str As New System.Text.StringBuilder
            Dim out As New List(Of String)
            out.Add(File.Name)
            If Sams.Count <> 0 Then
                out.Add(NofRead(Sams))
                out.Add(NofMappings(Sams))
                out.Add(Median_Read_Lengths(Sams))
                out.Add(Avr_Read_Lengths(Sams))
                out.Add(Avr_Read_Lengths_SD(Sams))
                out.Add(Get_Median_Alignment_Length(Sams))
                out.Add(Get_Avr_Alignment_Length(Sams))
                out.Add(Get_Avr_Alignment_Length_SD(Sams))

                '    out.AddRange(Get_Match_MisMatch_Insertion_Deletions_SD(Sams))
                out.Add(Coverage(Sams, File))
            End If
            Return ClassLibrary1.Szunyi.Text.General.GetText(out, vbTab)
        End Function

        Public Shared Function CalculateStandardDeviation(data As List(Of Double)) As Double
            If data.Count = 0 Then Return 0
            Dim mean As Double = data.Average()
            Dim squares As New List(Of Double)
            Dim squareAvg As Double

            For Each value As Double In data
                squares.Add(System.Math.Pow(value - mean, 2))
            Next

            squareAvg = squares.Average()

            Return System.Math.Sqrt(squareAvg)
        End Function
        ''' <summary>
        ''' Nof distict QName
        ''' </summary>
        ''' <param name="Sams"></param>
        ''' <returns></returns>
        Public Shared Function NofRead(Sams As IEnumerable(Of SAMAlignedSequence)) As Integer
            Return (From x In Sams Select x.QName).Distinct.Count
        End Function
        ''' <summary>
        ''' Nof SAMs in FIles
        ''' </summary>
        ''' <param name="Sams"></param>
        ''' <returns></returns>
        Public Shared Function NofMappings(Sams As IEnumerable(Of SAMAlignedSequence)) As Integer
            Return Sams.Count
        End Function
        ''' <summary>
        ''' SD of Best Alignments
        ''' </summary>
        ''' <param name="Sams"></param>
        ''' <returns></returns>
        Public Shared Function Avr_Read_Lengths_SD(Sams As IEnumerable(Of SAMAlignedSequence)) As String
            Dim RLs As New List(Of Double)
            For Each Item In Get_Bests(Sams)
                RLs.Add(Item.QuerySequence.Count)
            Next
            Return CalculateStandardDeviation(RLs)
        End Function
        ''' <summary>
        ''' Avarage of Read Lengths
        ''' </summary>
        ''' <param name="Sams"></param>
        ''' <returns></returns>
        Public Shared Function Avr_Read_Lengths(Sams As IEnumerable(Of SAMAlignedSequence)) As Integer
            Dim RLs As New List(Of Integer)
            For Each Item In Get_Bests(Sams)
                RLs.Add(Item.QuerySequence.Count)
            Next
            Return RLs.Sum / RLs.Count
        End Function
        ''' <summary>
        ''' Median Of Best Alignments Reads
        ''' </summary>
        ''' <param name="sams"></param>
        ''' <returns></returns>
        Public Shared Function Median_Read_Lengths(sams As IEnumerable(Of SAMAlignedSequence)) As Double
            Dim RLs As New List(Of Integer)
            For Each Item In Get_Bests(sams)
                RLs.Add(Item.QuerySequence.Count)
            Next
            Return Szunyi.Number.Get_Median(RLs)
        End Function

        Public Shared Function Get_Avr_Alignment_Length_SD(Sams As IEnumerable(Of SAMAlignedSequence)) As String
            Dim Best_ALs As New List(Of Double)

            For Each Item In Get_Bests(Sams)
                Best_ALs.Add(Get_Aligned_Length(Item))
            Next
            Return CalculateStandardDeviation(Best_ALs)
        End Function
        Public Shared Function Get_Avr_Alignment_Length(Sams As IEnumerable(Of SAMAlignedSequence)) As Integer
            Dim Best_ALs As New List(Of Integer)

            For Each Item In Get_Bests(Sams)
                Best_ALs.Add(Get_Aligned_Length(Item))
            Next
            Return Best_ALs.Sum / Best_ALs.Count
        End Function
        Public Shared Function Get_Median_Alignment_Length(Sams As IEnumerable(Of SAMAlignedSequence)) As Integer
            Dim Best_ALs As New List(Of Integer)

            For Each Item In Get_Bests(Sams)
                Best_ALs.Add(Get_Aligned_Length(Item))
            Next
            Return Szunyi.Number.Get_Median(Best_ALs)
        End Function
        Public Shared Function Coverage(Sams As IEnumerable(Of SAMAlignedSequence), BamFile As FileInfo, Optional RName As String = "") As Double
            Dim Length As Long
            If RName <> "" Then
                Length = Get_Length(BamFile, Sams.First.RName)
            Else
                Length = Get_Length(BamFile)
            End If


            Dim Best_ALs As New List(Of Integer)
            For Each Item In Get_Bests(Sams)
                Best_ALs.Add(Get_Aligned_Length(Item))
            Next
            Return Best_ALs.Sum / Length
        End Function
#Region "SubRoutines"
        Private Shared Function Get_Length(File As FileInfo, RName As String) As Long
            Using sr As New FileStream(File.FullName, FileMode.Open)
                If File.Extension = ".bam" Then
                    Dim sa As New Bio.IO.BAM.BAMParser()
                    Dim t = sa.GetHeader(sr)

                    For Each item In t.ReferenceSequences
                        If item.Name = RName Then
                            Return item.Length
                        End If
                    Next

                ElseIf File.Extension = ".sam" Then
                    Dim sa As New Bio.IO.SAM.SAMParser()

                    Dim hd = Bio.IO.SAM.SAMParser.ParseSAMHeader(sr)

                    For Each item In hd.ReferenceSequences
                        If item.Name = RName Then
                            Return item.Length
                        End If
                    Next

                End If

            End Using
            Return 0
        End Function

        Private Shared Function Get_Length(File As FileInfo) As Long
            Using sr As New FileStream(File.FullName, FileMode.Open)
                If File.Extension = ".bam" Then
                    Dim sa As New Bio.IO.BAM.BAMParser()
                    Dim t = sa.GetHeader(sr)
                    Dim l As Long
                    For Each item In t.ReferenceSequences
                        l += item.Length
                    Next
                    Return l
                ElseIf File.Extension = ".sam" Then
                    Dim sa As New Bio.IO.SAM.SAMParser()

                    Dim hd = Bio.IO.SAM.SAMParser.ParseSAMHeader(sr)
                    Dim l As Long
                    For Each item In hd.ReferenceSequences
                        l += item.Length
                    Next
                    Return l
                End If

            End Using
            Return 0
        End Function
        Private Shared Iterator Function Get_Bests(sams As IEnumerable(Of SAMAlignedSequence)) As IEnumerable(Of SAMAlignedSequence)
            Dim gr = From x In sams Group By x.QName Into Group

            For Each g In gr
                Dim Bests As New Dictionary(Of Integer, Bio.IO.SAM.SAMAlignedSequence)
                For Each Item In g.Group
                    Dim l = Get_Aligned_Length(Item)
                    If Bests.ContainsKey(l) = False Then Bests.Add(l, Item)
                Next
                Dim r = From x In Bests Order By x.Key Descending

                Yield r.First.Value
            Next

        End Function
        Public Shared Function Get_Aligned_Length(SAM As Bio.IO.SAM.SAMAlignedSequence) As Integer
            If IsNothing(SAM.QuerySequence) = True Then Return 0
            Dim l = SAM.QuerySequence.Count
            For Each Item In Get_CIGARS(SAM.CIGAR)

                If Item.Key = "S" Then ' Or Item.Key = "D" Then
                    l -= Item.Value
                End If
            Next
            Return l
        End Function
        Private Shared Function Get_CIGARS(CIGAR As String) As List(Of KeyValuePair(Of String, Integer))
            Dim CIGARS As New List(Of KeyValuePair(Of String, Integer))
            Dim cI As String = ""
            For i1 = 0 To CIGAR.Count - 1
                Dim s As String = CIGAR(i1)
                Dim i As Integer = 0
                If Integer.TryParse(s, 1) Then
                    cI = cI & s
                Else
                    If cI = String.Empty Then cI = "0"
                    Dim t As New KeyValuePair(Of String, Integer)(s, cI)
                    CIGARS.Add(t)
                    cI = ""
                End If
            Next
            Return CIGARS
        End Function

#End Region


    End Class
    Public Class Stat_Settings
        Public Property By_Files As Szunyi.Outer_Programs.Input_Description
        Public Property By_Org As Szunyi.Outer_Programs.Input_Description
        Public Property By_Read_Group As Szunyi.Outer_Programs.Input_Description
        Public Property By_Alignments As Szunyi.Outer_Programs.Input_Description
        Public Property Combine As Szunyi.Outer_Programs.Input_Description
        Public Sub New()
            By_Files = New Outer_Programs.Input_Description("By Files",
                                                               Outer_Programs.Input_Description_Type.Boolean,
                                                               "Create Stat for every Files",
                                                               1, 100, 20, 20, 1, "True", "")
            By_Org = New Outer_Programs.Input_Description("By Organism",
                                                                 Outer_Programs.Input_Description_Type.Boolean,
                                                                 "Create Stat for every Organism from different files",
                                                                 1, 200000, 1, 200000, 0, "True", "")
            By_Read_Group = New Outer_Programs.Input_Description("By Read Group",
                                                                 Outer_Programs.Input_Description_Type.Boolean,
                                                                 "Create Stat for every Read Group",
                                                                 1, 200000, 1, 200000, 0, "True", "")
            By_Alignments = New Outer_Programs.Input_Description("Use All, only Best Alignments, or create both version",
                                                              Outer_Programs.Input_Description_Type.Selection,
                                                              "Use All, only Best Alignments, or create both version",
                                                              1, 100, 1, 100, 0, "Best|All|Both", "")
            Combine = New Outer_Programs.Input_Description("Do You want to combine the different settings",
                                                                 Outer_Programs.Input_Description_Type.Boolean,
                                                                 "Do You want to combine the different settings",
                                                                 1, 200000, 1, 200000, 0, "True", "")


        End Sub
    End Class
    Public Class Stat_Comparer
        Implements IComparer(Of Simple_Stat)

        Public Function Compare(x As Simple_Stat, y As Simple_Stat) As Integer Implements IComparer(Of Simple_Stat).Compare
            Return x.ID.CompareTo(y.ID)
        End Function
    End Class
    Public Class StatII
        Dim Setting As Stat_Settings
        Dim Bam_Files As List(Of FileInfo)
        Public Property By_Files As New Dictionary(Of FileInfo, List(Of Simple_Stat))
        Public Property By_Org As New Dictionary(Of String, List(Of Simple_Stat))
        Public Property By_Read_Group As New Dictionary(Of String, List(Of Simple_Stat))
        Public Property By_Al_Type As New Dictionary(Of String, List(Of Simple_Stat))
        Public Sub New(x As Stat_Settings, Files As List(Of FileInfo))
            If Files.Count = 0 Then Exit Sub
            Setting = x
            Bam_Files = Files
            Create_Dictionaries()
            Do_It()
            Dim str As New System.Text.StringBuilder
            str.Append(Get_Header).AppendLine()
            If Setting.By_Files.Default_Value = 1 Then
                Dim All_Reads As New List(Of Simple_Stat)
                Dim Genome_Length As Long
                For Each Item In By_Files
                    Genome_Length = Szunyi.BAM.Bam_Basic_IO.Headers.Get_Organism_Length(Item.Key)
                    For Each AL In By_Al_Type
                        If AL.Value.Count <> 0 Then

                            Dim res = Combine(Item.Value, AL.Value)
                            All_Reads.AddRange(res)
                            str.Append(Get_Text(Item.Key.FullName, res, AL.Key, Genome_Length))
                            str.AppendLine()
                        End If

                    Next
                    str.AppendLine()
                Next
                For Each AL In By_Al_Type
                    If AL.Value.Count <> 0 Then
                        Dim Cumulative = Combine(AL.Value, All_Reads)
                        str.Append(Get_Text("All_File", Cumulative, AL.Key, Genome_Length))
                        str.AppendLine()
                    End If
                Next
                Genome_Length = Szunyi.BAM.Bam_Basic_IO.Headers.Get_Organism_Length(Files.First)
                Dim RGs = Szunyi.BAM.Bam_Basic_IO.Headers.Get_Read_Groups(Files)
                For Each Item In Me.By_Read_Group
                    If Item.Value.Count <> 0 Then
                        For Each AL In By_Al_Type
                            If AL.Value.Count <> 0 Then

                                Dim res = Combine(Item.Value, AL.Value)
                                All_Reads.AddRange(res)
                                For Each sg In RGs
                                    Dim tmp = From x1 In sg.Tags Where x1.Tag = "ID" And x1.Value = Item.Key
                                    If tmp.Count > 0 Then
                                        Dim kj = (From x4 In sg.Tags Where x4.Tag = "LB" Select x4.Value).First
                                        str.Append(Get_Text(kj, res, AL.Key, Genome_Length))
                                        str.AppendLine()
                                    End If

                                Next


                            End If

                        Next
                        str.AppendLine()
                    End If
                Next
            End If

            Dim x2 = Szunyi.IO.Files.Save.SelectSaveFile(Constants.Files.All_TAB_Like)
            Szunyi.IO.Export.SaveText(str.ToString, x2)
            Dim hj As Int16 = 65
        End Sub
        Private Function Get_Text(Name As String, res As List(Of Simple_Stat), Key As String, Genome_Length As Long) As String
            Dim str As New System.Text.StringBuilder
            str.Append(Name).Append(vbTab).Append(Key).Append(vbTab)
            str.Append(res.Count).Append(vbTab)
            ' Introns
            str.Append(Szunyi.Alignment.Own_Al_Helper.Properties.Get_Reads_Sum_wIntrons(res)).Append(vbTab)
            Dim I = Szunyi.Alignment.Own_Al_Helper.Properties.Get_Avarage_and_SD_Intron(res) ' Length
            str.Append(I.Avarage).Append(vbTab)
            str.Append(I.SD).Append(vbTab)
            ' Exons
            str.Append(Szunyi.Alignment.Own_Al_Helper.Properties.Get_Exons_Per_Mappings(res)).Append(vbTab)
            Dim E = Szunyi.Alignment.Own_Al_Helper.Properties.Get_Avarage_and_SD_Exon(res)
            str.Append(E.Avarage).Append(vbTab)
            str.Append(E.SD).Append(vbTab)
            'Reads Median, avr, SD
            Dim Read_Lengths = (From x1 In res Select x1.Read_Length).ToList
            str.Append(Szunyi.Number.Get_Median(Read_Lengths)).Append(vbTab)

            Dim Aligned_Read_Lengths = (From x1 In res Select x1.Aligned_Read_Length).ToList
            str.Append(Szunyi.Number.Get_Median(Aligned_Read_Lengths)).Append(vbTab)
            'Read Lengths
            Dim RL = Szunyi.Alignment.Own_Al_Helper.Properties.Get_Avarage_and_SD_Read(res)
            str.Append(RL.Avarage).Append(vbTab)
            str.Append(RL.SD).Append(vbTab)
            'Aligned Read Lengths
            Dim ARL = Szunyi.Alignment.Own_Al_Helper.Properties.Get_Avarage_and_SD_Aligned_Read(res)
            str.Append(ARL.Avarage).Append(vbTab)

            str.Append(ARL.SD).Append(vbTab)
            Dim Dels = Szunyi.Alignment.Own_Al_Helper.Properties.Get_Percents_and_Sd_Deletions(res)
            str.Append(Dels.Avarage).Append(vbTab)
            str.Append(Dels.SD).Append(vbTab)
            '     str.Append(Dels.SEM).Append(vbTab)

            Dim Ins = Szunyi.Alignment.Own_Al_Helper.Properties.Get_Percents_and_Sd_Insertion(res)
            str.Append(Ins.Avarage).Append(vbTab)
            str.Append(Ins.SD).Append(vbTab)
            '     str.Append(Ins.SEM).Append(vbTab)

            Dim Match = Szunyi.Alignment.Own_Al_Helper.Properties.Get_Percents_and_Sd_Match(res)
            str.Append(Match.Avarage).Append(vbTab)
            str.Append(Match.sd).Append(vbTab)
            '      str.Append(Match.SEM).Append(vbTab)


            Dim MisMatch = Szunyi.Alignment.Own_Al_Helper.Properties.Get_Percents_and_Sd_MisMatch(res)
            str.Append(MisMatch.Avarage).Append(vbTab)
            str.Append(MisMatch.SD).Append(vbTab)
            '        str.Append(MisMatch.SEM).Append(vbTab)

            Dim S = Szunyi.Alignment.Own_Al_Helper.Properties.Get_Percents_and_Sd_S(res)
            str.Append(S.Avarage).Append(vbTab)
            str.Append(S.SD).Append(vbTab)

            str.Append(ARL.SUM / Genome_Length)

            Return str.ToString
        End Function
        Private Function Coverage()

        End Function
        Private Function Combine(l1 As List(Of Simple_Stat), l2 As List(Of Simple_Stat)) As List(Of Simple_Stat)
            Dim c As New Stat_Comparer
            Dim Out As New List(Of Simple_Stat)
            l2.Sort(c)
            For Each Item In l1
                Dim Index = l2.BinarySearch(Item, c)
                If Index >= 0 Then
                    Out.Add(Item)
                Else
                    Dim tt As Int16 = 43
                End If
            Next
            Return Out
        End Function

        Private Function Get_Header()
            Dim str As New System.Text.StringBuilder
            str.Append("FileName").Append(vbTab)
            str.Append("Mapping Type").Append(vbTab)
            str.Append("Nof Mappings").Append(vbTab)

            str.Append("Nof Mappings Has Introns").Append(vbTab)
            str.Append("Avr. Intron Length").Append(vbTab)
            str.Append("Avr. Intron Length SD").Append(vbTab)

            str.Append("Nof Exons Per Mapping").Append(vbTab)
            str.Append("Avr. Exon Length").Append(vbTab)
            str.Append("Avr. Exon Length SD").Append(vbTab)

            str.Append("Median Read Length").Append(vbTab)
            str.Append("Median Aligned Read Length").Append(vbTab)

            str.Append("Avr. Read Length").Append(vbTab)
            str.Append("Avr. Read Length SD").Append(vbTab)

            str.Append("Avr. Aligned Read Length").Append(vbTab)
            str.Append("Avr. Aligned Read Length SD").Append(vbTab)

            str.Append("Avr. Deletion %").Append(vbTab)
            str.Append("Avr. Deletion % SD").Append(vbTab)

            str.Append("Avr. Insertion %").Append(vbTab)
            str.Append("Avr. Insertion % SD").Append(vbTab)

            str.Append("Avr. Match %").Append(vbTab)
            str.Append("Avr. Match % SD").Append(vbTab)

            str.Append("Avr. MisMatch %").Append(vbTab)
            str.Append("Avr. MisMatch % SD").Append(vbTab)

            str.Append("Avr. Soft Clip Lengths").Append(vbTab)
            str.Append("Avr. Soft Clip Lengths SD").Append(vbTab)
            str.Append("Coverage").Append(vbTab)
            Return str.ToString



            ' Nof Read,Nof Mapping,Median Read Length,Avr. Read Length,Avr. Read Length SD,Median Aligned Read Length,Avr. Aligned Read Length,Avr. Aligned Read Length SD,Deleteion %,Deleteion % SD,Insertion %,Insertion % SD,Match%,Match% SD,MisMatch%,MisMatch% SD,Coverage

        End Function
        Private Sub Do_It()
            Dim Index As Long = 0
            For Each File In Me.Bam_Files
                Dim current As New List(Of Simple_Stat)
                Dim Header = Szunyi.BAM.Bam_Basic_IO.Headers.Get_Header(File)
                For Each SAM In Szunyi.BAM.Bam_Basic_IO.Import.Parse(File)

                    If SAM.Flag <> SAMFlags.UnmappedQuery Then
                        Index += 1
                        If IsNothing(SAM.QuerySequence) = False Then
                            Dim x1 As New Simple_Stat(SAM, Index)
                            If Me.Setting.By_Files.Default_Value = 1 Then ' Ha Selected
                                Me.By_Files(File).Add(x1)
                            End If
                            If Me.Setting.By_Org.Default_Value = 1 Then ' Ha Selected
                                Me.By_Org(SAM.RName).Add(x1)
                            End If
                            If Me.Setting.By_Read_Group.Default_Value = 1 Then ' Ha Selected
                                Dim GroupID = Szunyi.BAM.BAM_Optional_Filed_Manipulation.Get_Read_Group_ID(SAM)
                                If GroupID <> String.Empty Then
                                    If Me.By_Read_Group.ContainsKey(GroupID) = False Then Me.By_Read_Group.Add(GroupID, New List(Of Simple_Stat))
                                    Me.By_Read_Group(GroupID).Add(x1)
                                End If
                            End If
                            By_Al_Type("All").Add(x1)
                            current.Add(x1)
                        End If
                    End If
                Next
                If Me.Setting.By_Alignments.Selected_Value = "Best" Or Me.Setting.By_Alignments.Selected_Value = "Both" Then
                    For Each i In ByGroupID(current)
                        Me.By_Al_Type("Best").Add(i.First)
                    Next

                End If
            Next

        End Sub
        Private Iterator Function ByGroupID(current As List(Of Simple_Stat)) As IEnumerable(Of List(Of Simple_Stat))
            Dim r = From x In current Group By x.Read_ID Into Group

            For Each gr In r
                Dim Best = From k In gr.Group Order By k.Match.Sum Descending
                Yield Best.ToList
            Next
        End Function

        Private Sub Create_Dictionaries()
            For Each File In Bam_Files
                By_Files.Add(File, New List(Of Simple_Stat))
            Next
            Dim Ref_Seq_IDs = Szunyi.BAM.Bam_Basic_IO.Headers.Get_Reference_SeqIDS(Bam_Files)
            For Each ref_Seq_ID In Ref_Seq_IDs
                By_Org.Add(ref_Seq_ID, New List(Of Simple_Stat))
            Next
            Dim RGs = Szunyi.BAM.Bam_Basic_IO.Headers.Get_RGs(Me.Bam_Files)
            For Each rg In RGs
                By_Read_Group.Add(rg, New List(Of Simple_Stat))
            Next

            By_Al_Type.Add("All", New List(Of Simple_Stat))
            By_Al_Type.Add("Best", New List(Of Simple_Stat))
            By_Al_Type.Add("Both", New List(Of Simple_Stat))

        End Sub
    End Class
    Public Class Simple_Stat
        Public Property ID As Long
        Public Property Read_ID As String
        Public Property Read_Length As Long '
        Public Property Aligned_Read_Length As Long ' I + M + MM
        Public Property Nof_Exon As Long
        Public Property Exon_Lengths As New List(Of Long)
        Public Property Nof_Intron As Long
        Public Property Intron_lengths As New List(Of Long)
        Public Property Alignment_Length As Long ' I+D+M+MM
        Public Property Aligned_Ref_Length As Long ' D + M +MM
        Public Property Soft_Clip As Long 'S
        Public Property Insertions As Nof_Sum_Avarage
        Public Property Deletions As Nof_Sum_Avarage
        Public Property MisMatch As Nof_Sum_Avarage
        Public Property Match As Nof_Sum_Avarage

        Public Property Sam As SAMAlignedSequence

        Public Sub New(sam As SAMAlignedSequence, Index As Long)
            Me.Sam = sam
            Me.ID = Index
            Me.Read_ID = sam.QName
            Me.Read_Length = sam.QuerySequence.Count
            Dim x1 As New Szunyi.Alignment.Own_Al(sam)
            Me.Aligned_Read_Length = Szunyi.Alignment.Own_Al_Helper.Properties.Get_Aligned_Read_Length(x1) ' I M MM
            Me.Aligned_Ref_Length = Szunyi.Alignment.Own_Al_Helper.Properties.Get_Aligned_Ref_Length(x1) ' D M MM
            Me.Alignment_Length = Szunyi.Alignment.Own_Al_Helper.Properties.Get_Alignment_Length(x1) ' D, I, M,MM
            Me.Soft_Clip = Szunyi.Alignment.Own_Al_Helper.Properties.Get_S(x1)
            Dim l = Szunyi.Location.Common.GetLocation(x1)

            Dim exons = Szunyi.Location.Common.Get_All_Exon_Location(l)
            Dim Lengths = Szunyi.Location.Common.Get_Length(exons)
            Dim Long_Lengths = Szunyi.Math.Sum_Avarage.Convert_to_Long_List(Lengths)
            Me.Exon_Lengths.AddRange(Long_Lengths)

            Me.Nof_Exon = exons.Count

            Dim Introns = Szunyi.Location.Common.Get_All_Intron_Location(l)


            Lengths = Szunyi.Location.Common.Get_Length(Introns)
            Long_Lengths = Szunyi.Math.Sum_Avarage.Convert_to_Long_List(Lengths)
            Me.Intron_lengths.AddRange(Long_Lengths)

            Me.Nof_Intron = Intron_lengths.Count

            Me.Insertions = Szunyi.Alignment.Own_Al_Helper.Properties.Get_Summary(x1, Szunyi.Alignment.Own_Al.Type.Insertion, Me.Alignment_Length)
            Me.Deletions = Szunyi.Alignment.Own_Al_Helper.Properties.Get_Summary(x1, Szunyi.Alignment.Own_Al.Type.Deletion, Me.Alignment_Length)
            Me.Match = Szunyi.Alignment.Own_Al_Helper.Properties.Get_Summary(x1, Szunyi.Alignment.Own_Al.Type.Match, Me.Alignment_Length)
            Me.MisMatch = Szunyi.Alignment.Own_Al_Helper.Properties.Get_Summary(x1, Szunyi.Alignment.Own_Al.Type.MisMatch, Me.Alignment_Length)
            If Me.Insertions.Sum + Me.MisMatch.Sum + Me.Match.Sum + Me.Deletions.Sum <> Me.Alignment_Length Then
                Dim kj As Int16 = 65
            End If
            Me.Sam = Nothing

        End Sub
    End Class

    Public Class Nof_Sum_Avarage

        Private aligned_Length As Integer

        Public Sub New(type As Own_Al.Type, Items As IEnumerable(Of Own_Al_Single_Part), aligned_Length As Integer)
            Me.Type = type

            Me.aligned_Length = aligned_Length
            Me.Nof = Items.Count
            Me.Sum = (From x In Items Select x.Length).Sum
            Me.Sum_per_Nof = Me.Sum / Me.Nof
            Me.Norm_Nof = (100 / aligned_Length) * Me.Nof
            Me.Norm_Sum = (100 / aligned_Length) * Me.Sum
        End Sub

        Public Property Nof As Integer
        Public Property Sum As Integer
        Public Property Sum_per_Nof As Double
        Public Property Norm_Nof As Double
        Public Property Norm_Sum As Double
        Public Property Type As Szunyi.Alignment.Own_Al.Type

    End Class
End Namespace

