Imports System.ComponentModel
Imports System.IO
Imports System.Xml
Imports Bio.IO.GenBank
Imports ClassLibrary1
Imports ClassLibrary1.Szunyi
Imports ClassLibrary1.Szunyi.Constants

Public Class FileManager
    Public Property PropwValues As New Dictionary(Of String, List(Of String))
    Dim RefSeqs As List(Of Bio.ISequence)
    Dim All_Files As New List(Of FileInfo)
    Public Ref_Seqs_Files As List(Of FileInfo)
    Public Property AllValues As List(Of String)
    Public Property SelectedValues As List(Of String)
    Public Property Selected_SMRTs As New List(Of String)
    Public Property Descriptions As Szunyi.Text.TableManipulation.Items_With_Properties
    Public Property IDsAndRename As New Dictionary(Of String, String)
    Public ReadOnly Property VCF_Folder As New DirectoryInfo("D:\PacBio\vcf")
    Public ReadOnly Property RefSeq_Folder As New DirectoryInfo("D:\PacBio\RefSeqs")
    Public ReadOnly Property SAM_Folder As New DirectoryInfo("D:\PacBio\SAM")
    Public ReadOnly Property BAM_Folder As New DirectoryInfo("D:\PacBio\BAM")

#Region "Load"
    Private Sub FileManager_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        Load_Refresh()
    End Sub
    Private Sub RefreshToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles RefreshToolStripMenuItem.Click
        Load_Refresh()
    End Sub
    Public Sub Load_Refresh()
        Dim Dir As New DirectoryInfo("C:\Pacbio")
        Dim BasicFIle = New FileInfo(Dir.FullName & "\Sequencing data.tsv")
        If BasicFIle.Exists = False Then
            BasicFIle = ClassLibrary1.Szunyi.IO.Files.Filter.SelectFile
            If IsNothing(BasicFIle) = True Then Me.Close()
            Dir = BasicFIle.Directory
        End If
        Dim Alldir As New List(Of DirectoryInfo)

        All_Files = Szunyi.IO.Directory.GetAllFilesFromFolder_Recursive("", Dir)

        'Load Basic File
        Descriptions = New ClassLibrary1.Szunyi.Text.TableManipulation.Items_With_Properties(BasicFIle)
        Descriptions.DoIt(True)
        AllValues = Descriptions.Get_Distict_Headers_wValues

        'Load Fileinfos
        Dim DirOfRefSeqs As New DirectoryInfo(Dir.FullName & "\RefSeqs")
        Ref_Seqs_Files = DirOfRefSeqs.GetFiles.ToList
        Me.RefSeqs = Szunyi.IO.Import.Sequence.FromFiles(Ref_Seqs_Files)
    End Sub
#End Region

#Region "Extras"
    Private Sub GetMetaDatasToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles GetMetaDatasToolStripMenuItem.Click
        Dim Files = ClassLibrary1.Szunyi.IO.Files.Filter.SelectFiles("Select Files", ClassLibrary1.Szunyi.Constants.Files.Xml)
        Dim str As New System.Text.StringBuilder
        If IsNothing(Files) = True Then Exit Sub
        For Each File In Files
            If File.Name.Contains("metadata") Then
                str.Append(File.Name.Split(".").First)
                Dim xmldoc As New XmlDataDocument()
                Dim xmlnode As XmlNodeList
                Dim i As Integer

                Dim fs As New FileStream(File.FullName, FileMode.Open, FileAccess.Read)
                xmldoc.Load(fs)
                xmlnode = xmldoc.GetElementsByTagName("Name")
                For i = 0 To xmlnode.Count - 1
                    xmlnode(i).ChildNodes.Item(0).InnerText.Trim()
                    str.Append(vbTab).Append(xmlnode(i).ChildNodes.Item(0).InnerText.Trim())

                    ' str = xmlnode(i).ChildNodes.Item(0).InnerText.Trim() & "  " & xmlnode(i).ChildNodes.Item(1).InnerText.Trim() & "  " & xmlnode(i).ChildNodes.Item(2).InnerText.Trim()

                Next
                str.AppendLine()
            End If
        Next
    End Sub


    Private Function SortFiles(Files As List(Of FileInfo), IdsAndRename As Dictionary(Of String, String)) As List(Of FileInfo)
        Dim out As New List(Of FileInfo)
        For Each i In IdsAndRename
            Dim file = From x In Files Where x.Name.Contains(i.Key)

            If file.Count > 0 Then
                out.Add(file.First)
            End If
        Next
        Return out
    End Function
#End Region


#Region "SMRT ID"
    Private Sub AnyToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles AnyToolStripMenuItem.Click
        Dim sg = Select_SMRTs()
        Set_Selected_SMRTIDS(sg, False)
    End Sub

    Private Sub AllToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles AllToolStripMenuItem.Click
        Dim sg = Select_SMRTs()
        Set_Selected_SMRTIDS(sg, True)
    End Sub

    ''' <summary>
    ''' Return files with extension Of Selected SMRT
    ''' </summary>
    ''' <param name="Extension"></param>
    ''' <returns></returns>
    Private Function Get_Files(Extension As String) As List(Of FileInfo)
        Dim e = From x In Me.All_Files Where x.Extension = Extension
        Dim out As New List(Of FileInfo)
        Dim Selected_SMRTIDs = get_Selected_SMRTIDs()
        For Each f In e
            For Each SMrt In Selected_SMRTIDs
                If f.Name.Contains(SMrt) Then
                    out.Add(f)
                End If
            Next
        Next
        Return out
    End Function
    Private Function Select_SMRTs() As List(Of String)
        Dim s As New CheckBoxForStringsFull(AllValues, -1)
        If s.ShowDialog = DialogResult.OK Then
            Return s.SelectedStrings
        End If
        Return New List(Of String)
    End Function
    Private Sub Set_Selected_SMRTIDS(ToSearch As List(Of String), All_Must_Contains As Boolean)
        Dim out As New List(Of Szunyi.Text.TableManipulation.Item_With_Properties)

        If All_Must_Contains = True Then
            Dim first As Boolean = True
            For Each gr In Get_Groups(ToSearch)
                Dim Index = Descriptions.Get_Index(gr.First, Szunyi.Constants.TextMatch.Contains)
                Dim Key As String = gr.First.Split(":").First
                Dim Values = Szunyi.Text.General.Get_Last_Parts(gr, ":")
                Dim jk = Me.Descriptions.Get_Items_By_Value(Key, Values)
                If first = True Then
                    out.AddRange(jk)
                    first = False
                Else
                    Dim sg = out.Intersect(jk).ToList
                    out = sg
                End If

            Next
            Dim d = out.Distinct.ToList
            GetSg(d)
            Me.Selected_SMRTs = (From x In d Select x.ID).ToList
        Else
            For Each gr In Get_Groups(ToSearch)
                Dim Index = Descriptions.Get_Index(gr.First, Szunyi.Constants.TextMatch.Contains)
                Dim Key As String = gr.First.Split(":").First
                Dim Values = Szunyi.Text.General.Get_Last_Parts(gr, ":")
                Dim jk = Me.Descriptions.Get_Items_By_Value(Key, Values)
                out.AddRange(jk)
            Next
            Dim d = out.Distinct.ToList
            GetSg(d)
            Me.Selected_SMRTs = (From x In d Select x.ID).ToList

        End If

    End Sub
    Private Iterator Function Get_Groups(ToSearch As List(Of String)) As IEnumerable(Of List(Of String))


        For Each Gr In From x In ToSearch Group By x.Split(":").First Into Group
            Yield Gr.Group.ToList

        Next
    End Function
#End Region

#Region "Console"
    Private Function Select_Bam_Files() As List(Of FileInfo)
        Dim f1 As New CheckBoxForStringsFull(Me.RefSeqs, -1)
        If f1.ShowDialog <> DialogResult.OK Then Return Nothing
        Dim BAMs = Get_Files(".bam")
        Dim IDs = (From x In f1.SelSeqs Select x.ID).ToList
        Dim GoodBams = Szunyi.IO.Files.Filter.SelectFiles(BAMs, IDs)
        Return GoodBams
        If GoodBams.Count <> DataGridView1.Rows.Count Or GoodBams.Count = 0 Then
            GoodBams = Szunyi.IO.Files.Filter.SelectFiles("Select Files", Szunyi.Constants.Files.BAM)
        End If
        Return GoodBams
    End Function

#End Region

    Private Sub GetSg(sg As List(Of Szunyi.Text.TableManipulation.Item_With_Properties))
        Dim out As New List(Of List(Of String))
        Dim dt As New DataTable
        dt.Columns.Add(New DataColumn("Selected", GetType(Boolean)))
        For Each Item In Me.Descriptions.ItemHeaders
            dt.Columns.Add(Item)
        Next
        For Each s In sg
            Dim k As New List(Of String)

            k.AddRange(s.Properties)
            If k.Count > Me.Descriptions.ItemHeaders.Count Then
                k.RemoveAt(k.Count - 1)
            End If
            Dim row As DataRow = dt.NewRow
            row.Item(0) = True
            For i1 = 0 To k.Count - 1
                row.Item(i1 + 1) = k(i1)
            Next

            dt.Rows.Add(row)
        Next
        Me.DataGridView1.DataSource = dt
    End Sub

#Region "GTH"
    Private Sub DoGTHToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles DoGTHToolStripMenuItem.Click
        Dim SAMs = Get_Files(".fa")
        Dim FIle = Szunyi.IO.Files.Filter.SelectFile("Reference Fasta", Szunyi.Constants.Files.Fasta)

        If IsNothing(FIle) = True Then Exit Sub

        Dim str As New System.Text.StringBuilder
        For Each Read_file In SAMs
            Dim tmpFile = "/mnt/" & FIle.FullName.Replace("\", "/").Replace("C:", "c")
            Dim tmpReDfiLE = "/mnt/" & Read_file.FullName.Replace("\", "/").Replace("C:", "c")
            Dim thedir = ("/mnt/" & Read_file.DirectoryName & "/").Replace("\", "/").Replace("C:", "c")
            Dim OutPutFileName = thedir & "GTH_" & FIle.Name.Replace(FIle.Extension, "") & "_" & Read_file.Name.Replace(Read_file.Extension, ".gth")
            str.Append("gth -genomic " & tmpFile & " -cdnaforward -cdna " & tmpReDfiLE & " -o " & OutPutFileName).AppendLine()
        Next
        Clipboard.SetText(str.ToString)
        Me.TextBox1.Text = str.ToString
    End Sub

    Private Sub ConvertGTHToBAMToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles ConvertGTHToBAMToolStripMenuItem.Click

        Dim Files = Szunyi.IO.Files.Filter.SelectFiles("Reference Fasta", Szunyi.Constants.Files.Fasta)
        Dim Seqs = Szunyi.IO.Import.Sequence.FromFiles(Files)
        If IsNothing(Seqs) = True Then Exit Sub
        For Each File In Files

            Dim res = Szunyi.PacBio.GTH.ToSam(File, Seqs)
        Next
    End Sub
#End Region


#Region "IO + other"

    Private Function Get_DIfferences(ResultFiles As List(Of FileInfo), SMRT_IDs As List(Of String)) As List(Of String)
        Dim k = Szunyi.PacBio.Common.Get_IDs(ResultFiles).Distinct.ToList
        Dim NotFounds = SMRT_IDs.Except(k).ToList
        Return NotFounds
    End Function

    Private Sub FeaturesForDifferentExpToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles FeaturesForDifferentExpToolStripMenuItem.Click
        Dim Files = Szunyi.IO.Files.Filter.SelectFiles("Select ref sequences", Szunyi.Constants.Files.GenBank)
        If IsNothing(Files) = True Then Exit Sub
        Dim Seqs = Szunyi.IO.Import.Sequence.FromFiles(Files)
        Dim s = Szunyi.PacBio.Common.Add_Nof_Source_to_Labels(Seqs, Me.Descriptions)
        Szunyi.IO.Export.SaveSequencesToMultipleGenBank(Seqs)
        Dim width As Integer = 15
        Dim t = Szunyi.PacBio.Common.PAS_and_PolyA_Regions(Seqs, width)

    End Sub


    Private Sub BamToolStripMenuItem1_Click(sender As Object, e As EventArgs)
        Dim BamFiles = Get_Files(".bam")
        If IsNothing(BamFiles) = True Then Exit Sub



        Dim cRefSeqs As New List(Of Bio.ISequence)
        For Each gr In Szunyi.Sequences.SequenceManipulation.UniqueDistinct.GetSameSequenceGroupsBySeqID(Me.RefSeqs)
            If gr.Count = 1 Then
                cRefSeqs.Add(gr.First)
            Else
                Dim sg = From x1 In gr Where x1.Metadata.Count > 0

                If sg.Count > 0 Then
                    cRefSeqs.Add(sg.First)
                Else
                    cRefSeqs.Add(gr.First)
                End If

            End If
        Next
        Dim lsii As New List(Of String)
        Dim f1 As New CheckBoxForStringsFull(cRefSeqs, -1)
        If f1.ShowDialog = DialogResult.OK Then
            Dim cSeqs = f1.SelSeqs
            If cSeqs.Count > 0 Then
                Dim cBamFiles = From x In BamFiles Where x.Name.Contains(cSeqs.First.ID)
                If cBamFiles.Count > 0 Then
                    Dim ls As New List(Of String)
                    ' aT FIRST One By One
                    For Each f In cBamFiles

                        Dim Mappings = Szunyi.BAM.Bam_Basic_IO.Import.Parse(f)
                        Dim dupli = From x1 In Mappings Group By x1.QuerySequence.ID, x1.Pos, x1.CIGAR Into Group

                        For Each d In dupli
                            If d.Group.Count > 1 Then
                                ls.Add(f.Name)
                                Exit For
                            End If

                        Next
                    Next

                    Dim MappingsII = Szunyi.BAM.Bam_Basic_IO.Import.Parse(cBamFiles.ToList)
                    Dim dupliII = From x1 In MappingsII Group By x1.QuerySequence.ID, x1.Pos Into Group

                    For Each d In dupliII
                        If d.Group.Count > 1 Then
                            lsii.Add(d.Group(0).QuerySequence.ID)
                        End If

                    Next
                End If
            End If
        End If
        Dim KJ As Int16 = 54
    End Sub

    Private Sub ReorderAndRenameSampleToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles ReorderAndRenameSampleToolStripMenuItem.Click
        Dim f1 As New Get_List_of_String("Enter Information", False)
        If f1.ShowDialog = DialogResult.OK Then

        End If
    End Sub

    Private Sub SelectFromFileToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles FromFileToolStripMenuItem.Click
        Dim File = Szunyi.IO.Files.Filter.SelectFile(Szunyi.Constants.Files.All_TAB_Like, "Select Files", New DirectoryInfo("C:\PacBio\Selections"))
        If IsNothing(File) = True Then Exit Sub
        Dim x = Szunyi.IO.Import.Text.ParseToArray(File, vbTab)
        Me.IDsAndRename.Clear()
        Dim SMRT_IDs As New List(Of String)
        For Each Line In x
            Dim SMRT = Szunyi.PacBio.Common.Get_ID(Line.Last)
            Me.IDsAndRename.Add(SMRT, Line.First)
            SMRT_IDs.Add(SMRT)
        Next
        Me.Selected_SMRTs = SMRT_IDs
        Dim Items = Me.Descriptions.Get_Items_By_Keys(Me.Selected_SMRTs)
        GetSg(Items)
    End Sub

#Region "Copy"
    Private Sub MergeBamFIlesToolStripMenuItem1_Click(sender As Object, e As EventArgs) Handles MergeBamFIlesToolStripMenuItem1.Click
        If IsNothing(Me.RefSeqs) = True Then Exit Sub
        Dim f1 As New CheckBoxForStringsFull(Me.RefSeqs, 1)
        If f1.ShowDialog <> DialogResult.OK Then Exit Sub
        Dim BAMs = Get_Files(".bam")
        Dim GoodBams = Szunyi.IO.Files.Filter.SelectFiles(BAMs, f1.SelSeqs.First.ID)

        Dim SaveFile = Szunyi.IO.Files.Save.SelectSaveFile(Szunyi.Constants.Files.BAM)
        If IsNothing(SaveFile) = True Then Exit Sub

        Dim x As New Szunyi.Outer_Programs.SamTools(GoodBams, SaveFile, Outer_Programs.SamTools_Subprogram.Merge_SOrt_Index)
        x.DoIt()
    End Sub
    Private Sub ReadsWithMoreCountsToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles ReadsWithMoreCountsToolStripMenuItem.Click
        Dim f1 As New CheckBoxForStringsFull(Me.RefSeqs, 1)
        If f1.ShowDialog <> DialogResult.OK Then Exit Sub
        Dim BAMs = Get_Files(".bam")
        Dim GoodBams = Szunyi.IO.Files.Filter.SelectFiles(BAMs, f1.SelSeqs.First.ID)
        Dim IDs = (From x In f1.SelSeqs Select x.ID).ToList
        Dim All_Sams As New List(Of Bio.IO.SAM.SAMAlignedSequence)
        For Each Sam In Szunyi.BAM.Bam_Basic_IO.Import.Parse(GoodBams, IDs)
            All_Sams.Add(Sam)
        Next
        Dim rep_Regions As New List(Of Bio.IO.GenBank.Location)
        rep_Regions.Add(Szunyi.Location.Common.Get_Location("1..11999"))
        rep_Regions.Add(Szunyi.Location.Common.Get_Location("20000..180887"))
        rep_Regions.Add(Szunyi.Location.Common.Get_Location("180888..192117"))
        rep_Regions.Add(Szunyi.Location.Common.Get_Location("192118..300000"))
        Dim str As New System.Text.StringBuilder
        For Each d In Szunyi.Alignment.BAM_SAM.Filter.Get_Duplicated_Reads(All_Sams)
            Dim d1 = Szunyi.Alignment.BAM_SAM.Filter.Get_Not_Self(d)

            Dim not_in = Szunyi.Alignment.BAM_SAM.Filter.Get_NotFully_Covered_Reads(d1, rep_Regions)

            If not_in.Count > 0 Then
                str.Append(not_in.First.QName).Append(vbTab)
                For Each n In not_in
                    str.Append(n.CIGAR).Append(vbTab)
                    str.Append([Enum].GetName(GetType(Bio.IO.SAM.SAMFlags), n.Flag)).Append(vbTab)
                    str.Append(n.Pos).Append(vbTab)
                    str.Append(n.RefEndPos).Append(vbTab)
                Next
                str.AppendLine()
            End If
        Next
        Clipboard.SetText(str.ToString)

    End Sub

    Private Sub BamToolStripMenuItem2_Click(sender As Object, e As EventArgs) Handles BamToolStripMenuItem2.Click

        Dim f1 As New CheckBoxForStringsFull(Me.RefSeqs, 1)
        If f1.ShowDialog <> DialogResult.OK Then Exit Sub
        Dim BAMs = Get_Files(".bam")
        Dim GoodBams = Szunyi.IO.Files.Filter.SelectFiles(BAMs, f1.SelSeqs.First.ID)

        Dim FOlder = Szunyi.IO.Directory.Get_Folder
        If IsNothing(FOlder) = True Then Exit Sub
        If Me.IDsAndRename.Count > 0 Then
            For Each File In GoodBams
                File.CopyTo(FOlder.FullName & "\" & File.Name, True)
            Next
            For Each ID In Me.IDsAndRename
                Dim sg = From x In GoodBams Where x.Name.Contains(ID.Key)

                If sg.Count = 1 Then
                    Dim thefile = sg.First
                    thefile.CopyTo(FOlder.FullName & "\" & ID.Value & ".bam", True)
                End If
            Next
        Else
            For Each File In GoodBams
                File.CopyTo(FOlder.FullName & "\" & File.Name, True)
            Next
        End If
    End Sub

    Private Sub FastqToolStripMenuItem1_Click(sender As Object, e As EventArgs) Handles FastqToolStripMenuItem1.Click
        Dim Files = Get_Files(".fastq")

        Dim FOlder = Szunyi.IO.Directory.Get_Folder
        If IsNothing(FOlder) = True Then Exit Sub
        Dim rename = MsgBox("Do Zou want to rename", MsgBoxStyle.YesNo)
        If rename = MsgBoxResult.Yes Then

            For Each ID In Me.IDsAndRename
                Dim sg = From x In Files Where x.Name.Contains(ID.Key)

                If sg.Count = 1 Then
                    Dim thefile = sg.First
                    thefile.CopyTo(FOlder.FullName & "\" & ID.Value & ".fastq", True)
                End If
            Next
        Else
            For Each File In Files
                File.CopyTo(FOlder.FullName & "\" & File.Name, True)
            Next
        End If
    End Sub

    Private Sub FastaToolStripMenuItem2_Click(sender As Object, e As EventArgs) Handles FastaToolStripMenuItem2.Click
        Dim Files = Get_Files(".fa")

        Dim FOlder = Szunyi.IO.Directory.Get_Folder
        If IsNothing(FOlder) = True Then Exit Sub
        If Me.IDsAndRename.Count > 0 Then
            Dim rename = MsgBox("Do Zou want to rename", MsgBoxStyle.YesNo)
            Dim index As Int16 = 0
            If rename = MsgBoxResult.Yes Then

                For Each ID In Me.IDsAndRename
                    index += 1
                    Dim sg = From x In Files Where x.Name.Contains(ID.Key)

                    If sg.Count = 1 Then
                        Dim thefile = sg.First
                        thefile.CopyTo(FOlder.FullName & "\" & ID.Value & "_" & index & ".fa", True)
                    End If
                Next

            Else
                For Each File In Files
                    File.CopyTo(FOlder.FullName & "\" & File.Name, True)
                Next

            End If


        Else
            For Each File In Files
                File.CopyTo(FOlder.FullName & "\" & File.Name, True)
            Next
        End If
    End Sub

#End Region


    Private Sub FreebayesToolStripMenuItem1_Click(sender As Object, e As EventArgs) Handles FreebayesToolStripMenuItem1.Click
        Dim FIles = Szunyi.IO.Files.Filter.SelectFiles("Reference Fasta", Szunyi.Constants.Files.Fasta)

        If IsNothing(FIles) = True Then Exit Sub

        Dim BamFiles = Get_Files(".bam")
        If IsNothing(BamFiles) = True Then Exit Sub

        If BamFiles.Count < Me.Selected_SMRTs.Count Then
            Dim alf = Get_DIfferences(BamFiles, Me.Selected_SMRTs)
            MsgBox(Szunyi.Text.General.GetText(alf))
        End If

        Dim str As New System.Text.StringBuilder
        For Each file In BamFiles
            str.Append("freebayes -f ")
            str.Append(Szunyi.IO.Linux.Get_FileName(FIles.First))
            str.Append(" ")
            Dim sf = Szunyi.IO.Linux.Get_FileName(file)
            str.Append(sf).Append(" > " & "/mnt/d/PacBio/vcf/" & file.Name.Replace(".bam", ".vcf"))
            str.AppendLine()
        Next

        Dim txt = str.ToString
    End Sub

    Private Function Has_Done(Ref_Name As String, Extension As String, Read_File As FileInfo) As Boolean
        Dim ID = Szunyi.PacBio.Common.Get_ID(Read_File.Name)
        Dim res = From x In Me.All_Files Where x.Extension = Extension And x.Name.Contains(Ref_Name) And x.Name.Contains(ID)

        If res.Count > 0 Then
            Return True
        Else
            Return False
        End If
    End Function

#End Region
#Region "Check"

    Private Sub BamToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles BamToolStripMenuItem3.Click

        Dim f1 As New CheckBoxForStringsFull(Me.RefSeqs, 1)
        If f1.ShowDialog <> DialogResult.OK Then Exit Sub
        Dim BAMs = Get_Files(".bam")
        Dim GoodBams = Szunyi.IO.Files.Filter.SelectFiles(BAMs, f1.SelSeqs.First.ID)
        If GoodBams.Count = Me.DataGridView1.RowCount Then
            MsgBox("error")
        Else
            Dim alf As Int16 = 65
        End If

    End Sub
    Private Sub FastaDuplication(sender As Object, e As EventArgs) Handles DuplicationToolStripMenuItem.Click
        Dim Fasta_files = Get_Files(".fa")
        Dim Seqs = Szunyi.IO.Import.Sequence.FromFiles(Fasta_files)
        Dim DUPL = Szunyi.Sequences.SequenceManipulation.UniqueDistinct.Get1CopyByID(Seqs)
        If DUPL.Count <> Seqs.Count Then
            Dim alf As Int16 = 54
        Else
            MsgBox("All Are Unique!")
        End If
    End Sub
    Private Sub FastQToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles FastQToolStripMenuItem2.Click
        Dim Fasta_files = Get_Files(".fastq")
        Dim rs = Get_DIfferences(Fasta_files, Me.Selected_SMRTs)

        Dim txt = Szunyi.Text.General.GetText(rs)
        If txt <> String.Empty Then
            MsgBox(txt)
            Clipboard.SetText(Szunyi.Text.General.GetText(rs))
        End If
    End Sub
    Private Sub FastaToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles FastaToolStripMenuItem.Click
        Dim Fasta_files = Get_Files(".fa")
        Dim rs = Get_DIfferences(Fasta_files, Me.Selected_SMRTs)

        Dim txt = Szunyi.Text.General.GetText(rs)
        If txt <> String.Empty Then
            MsgBox(txt)
            Clipboard.SetText(Szunyi.Text.General.GetText(rs))
        End If

    End Sub
#End Region

#Region "Console"
    Private Sub GMAP(sender As Object, e As EventArgs) Handles GmapToolStripMenuItem.Click

        Dim FIles = Szunyi.IO.Files.Filter.SelectFiles("Reference Fasta", Szunyi.Constants.Files.Fasta, RefSeq_Folder)

        If IsNothing(FIles) = True Then Exit Sub

        Dim Read_Files = Get_Files(".fa")
        Dim nof = Environment.ProcessorCount
        If IsNothing(Read_Files) = True Or Read_Files.Count = 0 Then
            Read_Files = Szunyi.IO.Files.Filter.SelectFiles("Selecet", Szunyi.Constants.Files.Fasta)
        End If
        Dim SMRT_IDs = Me.IDsAndRename.Keys.ToList
        Dim SMRT_IDs_From_Files = (From x In Read_Files Select x.Name.Split(".").First).ToList
        Dim NotFounds = SMRT_IDs.Except(SMRT_IDs_From_Files)
        If IsNothing(Read_Files) = True Then Exit Sub
        Dim OutDir = Szunyi.IO.Directory.Get_Folder

        Dim str As New System.Text.StringBuilder

        For Each File In FIles
            Dim Ref_Name = File.Name.Split(".").First
            For Each Read_file In Read_Files
                If Has_Done(Ref_Name, ".bam", Read_file) = True Then
                    Dim alf As Int16 = 54
                Else
                    Dim Linux_Read_File = Szunyi.IO.Linux.Get_FileName(Read_file)
                    Dim Ref_File = Szunyi.IO.Linux.Get_FileName(File)
                    Dim Linux_Out_Dir = Szunyi.IO.Linux.Get_FileName(OutDir) & "/"
                    Dim OutPutFileName = Linux_Out_Dir & "GMAP_" & File.Name.Replace(File.Extension, "") & "_" & Read_file.Name.Replace(Read_file.Extension, ".sam")
                    str.Append("gmap -g " & Ref_File & " -f samse -t " & Environment.ProcessorCount - 1 & " " & Linux_Read_File & " > " & OutPutFileName).AppendLine()


                End If

            Next
        Next
        If str.Length > 0 Then Clipboard.SetText(str.ToString)
        Me.TextBox1.Text = str.ToString
    End Sub
    Private Sub FromManagerToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles FromManagerToolStripMenuItem.Click
        Dim f1 As New CheckBoxForStringsFull(Me.RefSeqs, 1)
        If f1.ShowDialog <> DialogResult.OK Then Exit Sub
        Dim Ref_Seq_File = From x In Me.Ref_Seqs_Files Where x.Name.Contains(f1.SelSeqs.First.ID) And x.Extension = ".fa"
        Dim BamFiles = Get_Files(".bam")
        If IsNothing(BamFiles) = True Then Exit Sub

        If BamFiles.Count < Me.Selected_SMRTs.Count Then
            Dim alf = Get_DIfferences(BamFiles, Me.Selected_SMRTs)
            MsgBox(Szunyi.Text.General.GetText(alf))
        End If

        Dim Ref_Seq_Linux_Path As String = Szunyi.IO.Linux.Get_FileName(Ref_Seq_File.First)
        Dim str As New System.Text.StringBuilder
        For Each Bam_File In BamFiles
            If Bam_File.Name.Contains(f1.SelSeqs.First.ID) Then
                str.Append("samtools mpileup -v -u -f ")
                str.Append(Ref_Seq_Linux_Path)
                str.Append(" ")
                Dim Bam_File_Linux_Path = Szunyi.IO.Linux.Get_FileName(Bam_File)
                Dim VCF = Szunyi.IO.Linux.Get_FileName(VCF_Folder, Bam_File, ".vcf")


                str.Append(Bam_File_Linux_Path).Append(" > " & VCF).AppendLine()

            End If

        Next

        Dim txt = str.ToString
        Me.TextBox1.Text = txt
        Clipboard.SetText(txt)
    End Sub

    Private Sub FromFIlesToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles FromFIlesToolStripMenuItem.Click
        Dim f1 As New CheckBoxForStringsFull(Me.RefSeqs, 1)
        If f1.ShowDialog <> DialogResult.OK Then Exit Sub
        Dim Ref_Seq_File = From x In Me.Ref_Seqs_Files Where x.Name.Contains(f1.SelSeqs.First.ID) And x.Extension = ".fa"
        Dim BamFiles = Szunyi.IO.Files.Filter.SelectFiles("Select FIles", Szunyi.Constants.Files.BAM)
        If IsNothing(BamFiles) = True Then Exit Sub

        If BamFiles.Count < Me.Selected_SMRTs.Count Then
            Dim alf = Get_DIfferences(BamFiles, Me.Selected_SMRTs)
            MsgBox(Szunyi.Text.General.GetText(alf))
        End If

        Dim Ref_Seq_Linux_Path As String = Szunyi.IO.Linux.Get_FileName(Ref_Seq_File.First)
        Dim str As New System.Text.StringBuilder
        For Each Bam_File In BamFiles
            If Bam_File.Name.Contains(f1.SelSeqs.First.ID) Then
                str.Append("samtools mpileup -v -u -f ")
                str.Append(Ref_Seq_Linux_Path)
                str.Append(" ")
                Dim Bam_File_Linux_Path = Szunyi.IO.Linux.Get_FileName(Bam_File)
                Dim VCF = Szunyi.IO.Linux.Get_FileName(VCF_Folder, Bam_File, ".vcf")

                str.Append(Bam_File_Linux_Path).Append(" > " & VCF).AppendLine()

            End If

        Next

        Dim txt = str.ToString
        Me.TextBox1.Text = txt
        Clipboard.SetText(txt)
    End Sub

#End Region
#Region "Analysis"

    Private Sub VCFToolStripMenuItem1_Click(sender As Object, e As EventArgs) Handles VCFToolStripMenuItem1.Click
        Dim VCF_Files = Get_Files(".vcf")
        Dim Sorted_VCF_Files = SortFiles(VCF_Files, Me.IDsAndRename)
        Dim VCF_Analysator As New Szunyi.SNPs.VCF_Manipulation(Sorted_VCF_Files, Me.IDsAndRename)
        VCF_Analysator.DoIt(3, 0.01)
        Dim alf As Int16 = 54
    End Sub


    Private Sub RepeatLengthsToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles RepeatLengthsToolStripMenuItem1.Click
        Dim Width = Szunyi.MyInputBox.GetInteger("Select Border Sequence Length")
        Dim File = Szunyi.IO.Files.Filter.SelectFile
        If IsNothing(File) = True Then Exit Sub

        Dim f1 As New CheckBoxForStringsFull(Me.RefSeqs, 1)
        If f1.ShowDialog <> DialogResult.OK Then Exit Sub

        Dim str As New System.Text.StringBuilder
        For Each Line In Szunyi.IO.Import.Text.ParseToArray(File, vbTab, 1)
            Dim TheSeq = f1.SelSeqs.First
            If IsNothing(TheSeq) = False Then
                Dim f As New Szunyi.DNA.Repeat.Repeat_Description
                f.LeftBorder = TheSeq.GetSubSequence(Line(1) - Width - 1, Width)
                Dim LeftBorder = TheSeq.GetSubSequence(Line(1) - Width - 1, Width)
                f.Repeat = TheSeq.GetSubSequence(Line(1) - 1, Line(2) - Line(1) + 1)
                Dim RepeatRegion = TheSeq.GetSubSequence(Line(1) - 1, Line(2) - Line(1) + 1)
                f.RightBorder = TheSeq.GetSubSequence(Line(2), Width)
                Dim RightBorder = TheSeq.GetSubSequence(Line(2), Width)
                f.Repeat = New Bio.Sequence(Bio.Alphabets.AmbiguousDNA, Line(5))
                f.Start = Line(1)
                f.Endy = Line(2)
                str.Append(Szunyi.Text.General.GetText(Line, vbTab))
                str.Append(vbTab).Append(Szunyi.Sequences.SequenceManipulation.SeqsToString.GetSeqAsString(LeftBorder))
                str.Append(vbTab).Append(Szunyi.Sequences.SequenceManipulation.SeqsToString.GetSeqAsString(RepeatRegion))
                str.Append(vbTab).Append(Szunyi.Sequences.SequenceManipulation.SeqsToString.GetSeqAsString(RightBorder))
                Dim alf As Int16 = 43
            End If
        Next

        Dim BAMs = Get_Files(".bam")
        Dim GoodBams = Szunyi.IO.Files.Filter.SelectFiles(BAMs, f1.SelSeqs.First.ID)
        Dim SortedBams = SortFiles(GoodBams, Me.IDsAndRename)
        Dim repi As New Szunyi.DNA.Repeat.RepeatFinder(SortedBams, File, Me.IDsAndRename, Width, f1.SelSeqs.First)

        Dim rep_Finder As New Szunyi.DNA.Repeat.RepeatFinder(SortedBams, File, Me.IDsAndRename, Width, f1.SelSeqs.First)
        rep_Finder.DoIt()

        Me.TextBox1.Text = Szunyi.Text.General.GetText(rep_Finder.res, vbCrLf)
        Clipboard.SetText(Szunyi.Text.General.GetText(rep_Finder.res, vbCrLf))
    End Sub

    Private Sub SMRTIDsFromFoldersToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles SMRTIDsFromFoldersToolStripMenuItem.Click
        Dim FOlder = Szunyi.IO.Directory.Get_Folder
        If IsNothing(FOlder) = True Then Exit Sub
        Dim AllFiles = Szunyi.IO.Directory.GetAllFilesFromFolder_Recursive(FOlder.FullName)
        Dim H5_Files = From x In AllFiles Where x.Extension = ".h5"

        Dim sg = Szunyi.PacBio.Common.Get_IDs(H5_Files.ToList).Distinct.ToList

        Dim Fasta_files = Get_Files(".fa")
        Dim sg2 = Szunyi.PacBio.Common.Get_IDs(Fasta_files)
        Dim out As New List(Of FileInfo)
        Dim NotFound = sg.Except(sg2)
        Dim metadatafiles As New List(Of FileInfo)
        Dim fas As New List(Of FileInfo)
        For Each item In NotFound
            If item <> String.Empty Then

                Dim theFIle = From x In AllFiles Where x.Name.Contains(item) AndAlso x.FullName.Contains("bas.h5")

                Dim fa = From x In AllFiles Where x.Name.Contains(item) AndAlso x.FullName.Contains(".fa")

                Dim Metadata = From x In AllFiles Where x.Name.Contains(item) AndAlso x.FullName.Contains(".xml")
                fas.AddRange(fa)
                out.AddRange(theFIle)
                metadatafiles.AddRange(Metadata)
            End If

        Next
        Dim str As New System.Text.StringBuilder
        For Each item In out
            str.Append(item.FullName).AppendLine()
        Next

        For Each File In metadatafiles
            If File.FullName.Contains("metadata") Then
                str.Append(File.Name.Split(".").First)
                Dim xmldoc As New XmlDataDocument()
                Dim xmlnode As XmlNodeList
                Dim i As Integer

                Dim fs As New FileStream(File.FullName, FileMode.Open, FileAccess.Read)
                xmldoc.Load(fs)
                xmlnode = xmldoc.GetElementsByTagName("Name")
                For i = 0 To xmlnode.Count - 1
                    xmlnode(i).ChildNodes.Item(0).InnerText.Trim()
                    str.Append(vbTab).Append(xmlnode(i).ChildNodes.Item(0).InnerText.Trim())

                    ' str = xmlnode(i).ChildNodes.Item(0).InnerText.Trim() & "  " & xmlnode(i).ChildNodes.Item(1).InnerText.Trim() & "  " & xmlnode(i).ChildNodes.Item(2).InnerText.Trim()

                Next
                str.AppendLine()
            End If

        Next
        Clipboard.SetText(str.ToString)
    End Sub

    Private Sub BasicToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles BasicToolStripMenuItem.Click
        Dim GoodBams = Select_Bam_Files()
        If IsNothing(GoodBams) = True Then Exit Sub
        '  pysamstats --type coverage example.bam > example.coverage.txt
        Dim str As New System.Text.StringBuilder
        Dim Ref_Seq_Linux_Path As String = Szunyi.IO.Linux.Get_FileName(GoodBams.First)
        Dim f1 As New CheckBoxForStringsFull(Me.RefSeqs, 1)
        If f1.ShowDialog <> DialogResult.OK Then Exit Sub
        Dim Ref_Seq_File = From x In Me.Ref_Seqs_Files Where x.Name.Contains(f1.SelSeqs.First.ID) And x.Extension = ".fa"
        Dim Ref_Fasta = Szunyi.IO.Linux.Get_FileName(Ref_Seq_File.First)
        For Each Bam_File In GoodBams
            str.Append("pysamstats --type variation --fasta " & Ref_Fasta & " ")

            Dim Bam_File_Linux_Path = Szunyi.IO.Linux.Get_FileName(Bam_File)
            str.Append(Bam_File_Linux_Path).Append(" > ")
            Dim VCF = Szunyi.IO.Linux.Get_FileName(VCF_Folder, Bam_File, ".coverage.tsv")
            str.Append(VCF).AppendLine()
        Next
        Clipboard.SetText(str.ToString)
        Me.TextBox1.Text = str.ToString
    End Sub

    Private Sub VariationsToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles VariationsToolStripMenuItem.Click
        Dim f1 As New CheckBoxForStringsFull(Me.RefSeqs, 1)
        If f1.ShowDialog <> DialogResult.OK Then Exit Sub
        Dim Ref_Seq_File = From x In Me.Ref_Seqs_Files Where x.Name.Contains(f1.SelSeqs.First.ID) And x.Extension = ".fa"
        Dim Seqs = Szunyi.IO.Import.Sequence.FromFile(Ref_Seq_File.First)
        Dim Res As New List(Of Variations)
        Dim Files = Szunyi.IO.Files.Filter.SelectFiles("Select Files", Szunyi.Constants.Files.All_TAB_Like)
        If IsNothing(Files) = True Then Exit Sub
        For Each File In Files
            Res.Add(New Variations(Seqs.First, File))
        Next
        Dim stringSeparators() As String = {"_m"}

        Dim Sorted = (From x In Res Select x Order By x.File.Name.Split(stringSeparators, StringSplitOptions.None).Last.Split("_").First Ascending).ToList


        Dim out As New List(Of String)
        For Each Vari In Sorted.First.Positions
            out.Add(Vari.Pos & vbTab & Vari.Ref & vbTab & "A")
            out.Add(Vari.Pos & vbTab & Vari.Ref & vbTab & "C")
            out.Add(Vari.Pos & vbTab & Vari.Ref & vbTab & "G")
            out.Add(Vari.Pos & vbTab & Vari.Ref & vbTab & "T")
        Next
        For Each S In Sorted
            For i1 = 0 To S.Positions.Count - 1
                Dim index = i1 * 4
                out(index) = out(index) & vbTab & S.Positions(i1).A
                out(index + 1) = out(index + 1) & vbTab & S.Positions(i1).C
                out(index + 2) = out(index + 2) & vbTab & S.Positions(i1).G
                out(index + 3) = out(index + 3) & vbTab & S.Positions(i1).T
                Dim Total = S.Positions(i1).A + S.Positions(i1).C + S.Positions(i1).G + S.Positions(i1).T
                out(index) = out(index) & vbTab & S.Positions(i1).A / Total * 100
                out(index + 1) = out(index + 1) & vbTab & S.Positions(i1).C / Total * 100
                out(index + 2) = out(index + 2) & vbTab & S.Positions(i1).G / Total * 100
                out(index + 3) = out(index + 3) & vbTab & S.Positions(i1).T / Total * 100
            Next
        Next
        Dim Header = "Position" & vbTab & "Ref NA" & vbTab & "Variation"
        For Each File In Sorted
            Dim s1 = File.File.Name.Split(stringSeparators, StringSplitOptions.None).Last.Split("_").First
            Header = Header & vbTab & "Count " & s1 & vbTab & "Percents " & s1
        Next

        out.Insert(0, Header)
        Dim rest = Szunyi.Text.General.GetText(out)
        Szunyi.IO.Export.SaveText(rest)
        Dim alf As Int16 = 54
    End Sub

    Private Sub MappingsToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles MappingsToolStripMenuItem.Click
        Dim BAMs = Get_Files(".bam")

        Dim org = Szunyi.BAM.Bam_Basic_IO.Headers.Get_ReferenceSequences(BAMs)
        Dim Organims_Names = (From x In org Select x.Name).ToList

        Dim Res As New Dictionary(Of String, List(Of String))
        Dim f1 As New CheckBoxForStringsFull(Organims_Names, -1)
        If f1.ShowDialog <> DialogResult.OK Then Exit Sub

        For Each s In f1.SelectedStrings
            Res.Add(s, New List(Of String))
        Next
        Dim GoodBams = Szunyi.IO.Files.Filter.SelectFiles(BAMs, f1.SelectedStrings)

        Res.Add("*", New List(Of String))
        For Each TheBam In BAMs
            For Each sam In Szunyi.BAM.Bam_Basic_IO.Import.Parse(TheBam)
                If Res.ContainsKey(sam.RName) Then
                    Res(sam.RName).Add(sam.QName)
                End If
            Next
        Next
        Dim ResII As New Dictionary(Of String, List(Of String))
        For Each item In Res
            ResII.Add(item.Key, item.Value.Distinct.ToList)
        Next
        Dim alf As Int16 = 54
    End Sub

    Private Sub MappedReadsIntoFastQToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles MappedReadsIntoFastQToolStripMenuItem.Click
        Dim DIrs = Szunyi.IO.Directory.Get_Folder

        For Each SubDir In DIrs.GetDirectories
            For Each File In SubDir.GetFiles
                If File.Extension = ".bam" Then
                    Dim out As New List(Of Bio.ISequence)
                    Dim p = Szunyi.PacBio.Common.Get_ID(File)

                    Dim fqFIle = (From x In All_Files Where x.Name.Contains(p) And x.Extension = ".fastq").ToList

                    Dim Seqs = Szunyi.IO.Import.Sequence.FromFiles(fqFIle)

                    For Each Sam In Szunyi.BAM.Bam_Basic_IO.Import.Parse(File, "X14112")
                        Dim FQ = Szunyi.Sequences.SequenceManipulation.GetSequences.ByID(Seqs, Sam.QName)
                        out.Add(FQ)
                    Next
                    Dim outF As New FileInfo(File.DirectoryName & "\" & fqFIle.First.Name)
                    Szunyi.IO.Export.SaveSequencesToSingleFastQ(out, outF)
                End If

            Next

        Next
    End Sub

    Private Sub FreebayesToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles FreebayesToolStripMenuItem.Click
        Dim Selected_SMRTIDs = get_Selected_SMRTIDs()

        Dim x As New Ref_Files_Bam_Files(Me.RefSeq_Folder, Me.All_Files, Selected_SMRTIDs)
        If x.BamFiles.Count = 0 Then Exit Sub
        Dim str As New System.Text.StringBuilder
        For Each ref In x.RefFiles
            For Each TheBam In x.BamFiles
                str.Append("freebayes -f ").Append(Szunyi.IO.Linux.Get_FileName(ref)).Append(" ").Append(Szunyi.IO.Linux.Get_FileName(TheBam))
                str.Append(" >").Append(Szunyi.IO.Linux.Get_FileName(Me.VCF_Folder, ref, TheBam, ".vcf")).AppendLine()
            Next

        Next
        Clipboard.SetText(str.ToString)
        Me.TextBox1.Text = str.ToString
    End Sub
    Private Function get_Selected_SMRTIDs() As List(Of String)
        Dim SElSMRTs As New List(Of String)
        For Each row As DataGridViewRow In DataGridView1.Rows
            If row.Cells(0).Value = True Then
                SElSMRTs.Add(row.Cells(1).Value)
            End If
        Next
        Return SElSMRTs
    End Function

    Private Sub ReNameFilesToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles ReNameFilesToolStripMenuItem.Click
        If Me.IDsAndRename.Count = 0 Then Exit Sub
        Dim Files = Szunyi.IO.Files.Filter.SelectFiles
        If IsNothing(Files) = True Then Exit Sub
        For Each File In Files
            For Each Item In Me.IDsAndRename
                If File.Name.Contains(Item.Key) Then
                    File.MoveTo(File.FullName.Replace(File.Name, Item.Value) & File.Extension)
                End If
            Next
        Next
    End Sub

    Private Function Get_TSS(locis As List(Of ILocation), width As Integer, MinNofItem As Integer, MinNof_InGroup As Integer, score As Sort_Locations_By, Range As Integer()) As List(Of FeatureItem)
        Dim out As New List(Of FeatureItem)
        Dim First_Exons = Szunyi.Features.FeatureManipulation.GetLocations.Get_First_Exons_Location(locis)
        Dim fwLocis = Szunyi.Features.FeatureManipulation.GetLocations.Get_Only_Positive_Strands(First_Exons)
        Dim revLocis = Szunyi.Features.FeatureManipulation.GetLocations.Get_Only_Negative_Strands(First_Exons)
        Dim Ranges = Szunyi.Number.GetRanges(Range)
        For Each f In Szunyi.Location.Merging.MergeLocations(fwLocis, width,  Sort_Locations_By.TSS, MinNofItem)
            Dim pos = Szunyi.Location.Get_Sites_Or_Positions.Get_Site(f,  Sort_Locations_By.TSS, score)
            Dim loci = Szunyi.Location.Common.Get_Location(f,  Sort_Locations_By.TSS)
            Dim sg As String = Szunyi.Number.Which_Range(Ranges, f.Count)
            Dim t As New FeatureItem("TSS_own_" & sg, loci)
            t.Label = "e:" & f.Count
            t.Qualifiers(StandardQualifierNames.IdentifiedBy) = Szunyi.Location.Common.Get_Locis_Accesions(f)
            If IsNothing(t.Qualifiers(StandardQualifierNames.IdentifiedBy)) = True Then
                Dim kj As Int16 = 43
            End If
            t.Qualifiers(StandardQualifierNames.Note) = Szunyi.Location.Common.Get_Location_Strings(f)
            '   t.Qualifiers (StandardQualifierNames .PopulationVariant ) = Szunyi .Features 
            If Szunyi.Location.Common.Get_Count(f) >= MinNof_InGroup Then out.Add(t)
        Next
        Dim notFOunf As Int16 = 0
        Dim k = Szunyi.Location.Merging.MergeLocations(revLocis, width,  Sort_Locations_By.LE, MinNofItem)
        For Each f In Szunyi.Location.Merging.MergeLocations(revLocis, width,  Sort_Locations_By.LE, MinNofItem)
            Dim pos = Szunyi.Location.Get_Sites_Or_Positions.Get_Site(f,  Sort_Locations_By.LE, score)
            Dim loci = Szunyi.Location.Common.Get_Location(f,  Sort_Locations_By.LE)
            Dim sg As String = Szunyi.Number.Which_Range(Ranges, f.Count)
            Dim t As New FeatureItem("TSS_own_" & sg, loci)
            t.Label = "e:" & f.Count
            t.Qualifiers(StandardQualifierNames.IdentifiedBy) = Szunyi.Location.Common.Get_Locis_Accesions(f)
            If IsNothing(t.Qualifiers(StandardQualifierNames.IdentifiedBy).First) = True Then
                notFOunf += 1
            End If
            t.Qualifiers(StandardQualifierNames.Note) = Szunyi.Location.Common.Get_Location_Strings(f)

            If Szunyi.Location.Common.Get_Count(f) >= MinNof_InGroup Then out.Add(t)
        Next

        Return out

    End Function

    Private Function Get_PolyA(locis As List(Of ILocation), width As Integer, MinNofItem As Integer, MinNof_InGroup As Integer, score As Sort_Locations_By, range As Integer()) As List(Of FeatureItem)
        Dim out As New List(Of FeatureItem)
        Dim Ranges = Szunyi.Number.GetRanges(range)

        Dim Last_Exons = Szunyi.Features.FeatureManipulation.GetLocations.Get_Last_Exons_Location(locis)
        Dim fwLocis = Szunyi.Features.FeatureManipulation.GetLocations.Get_Only_Positive_Strands(Last_Exons)
        Dim revLocis = Szunyi.Features.FeatureManipulation.GetLocations.Get_Only_Negative_Strands(Last_Exons)

        For Each f In Szunyi.Location.Merging.MergeLocations(fwLocis, width,  Sort_Locations_By.LE, MinNofItem)
            Dim pos = Szunyi.Location.Get_Sites_Or_Positions.Get_Site(f,  Sort_Locations_By.LE, score)
            Dim loci = Szunyi.Location.Common.Get_Location(f,  Sort_Locations_By.LE)
            Dim sg As String = Szunyi.Number.Which_Range(Ranges, f.Count)
            Dim t As New FeatureItem("PAS_own_" & sg, loci)
            t.Label = "e:" & f.Count
            t.Qualifiers(StandardQualifierNames.IdentifiedBy) = Szunyi.Location.Common.Get_Locis_Accesions(f)
            t.Qualifiers(StandardQualifierNames.Note) = Szunyi.Location.Common.Get_Location_Strings(f)
            If Szunyi.Location.Common.Get_Count(f) >= MinNof_InGroup Then out.Add(t)
        Next

        For Each f In Szunyi.Location.Merging.MergeLocations(revLocis, width,  Sort_Locations_By.TSS, MinNofItem)
            Dim pos = Szunyi.Location.Get_Sites_Or_Positions.Get_Site(f,  Sort_Locations_By.TSS, score)
            Dim loci = Szunyi.Location.Common.Get_Location(f,  Sort_Locations_By.TSS)
            Dim sg As String = Szunyi.Number.Which_Range(Ranges, f.Count)
            Dim t As New FeatureItem("PAS_own_" & sg, loci)
            t.Qualifiers(StandardQualifierNames.IdentifiedBy) = Szunyi.Location.Common.Get_Locis_Accesions(f)
            t.Qualifiers(StandardQualifierNames.Note) = Szunyi.Location.Common.Get_Location_Strings(f)
            If Szunyi.Location.Common.Get_Count(f) >= MinNof_InGroup Then out.Add(t)
        Next

        Return out
    End Function



    ''' <summary>
    ''' Return One occurence at first GenBank, after fasta
    ''' </summary>
    ''' <param name="org"></param>
    ''' <returns></returns>
    Private Function Get_Ref_Seqs(org As List(Of String)) As List(Of Bio.ISequence)
        Dim out As New List(Of Bio.ISequence)
        For Each o In org
            Dim Segys = From x In Me.RefSeqs Where x.ID = o Order By x.Metadata.Count Descending

            If Segys.Count > 0 Then out.Add(Segys.First)


        Next
        Return out
    End Function
    Private Function Get_Ref_Seqs(BamFIles As List(Of FileInfo)) As List(Of Bio.ISequence)
        If IsNothing(BamFIles) = True Then Return Nothing
        Dim Org = Szunyi.BAM.Bam_Basic_IO.Headers.Select_Reference_SeqIDs(BamFIles)
        Return Me.Get_Ref_Seqs(Org)

    End Function


    Private Sub SMRTIDsCountsFromSamBAMToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles SMRTIDsCountsFromSamBAMToolStripMenuItem.Click
        Dim Files = Szunyi.IO.Files.Filter.SelectFiles(Szunyi.Constants.Files.SAM_BAM)
        Dim str As New System.Text.StringBuilder
        For Each FIle In Files
            Dim SMRT_IDs As New List(Of String)
            For Each Sam In Szunyi.BAM.Bam_Basic_IO.Import.Parse(FIle)
                Dim ID = Szunyi.PacBio.Common.Get_ID(Sam.QName)
                If SMRT_IDs.Contains(ID) = False Then
                    SMRT_IDs.Add(ID)
                End If
            Next
            str.Append(FIle.Name).Append(vbTab).Append(SMRT_IDs.Count).Append(vbTab)
            str.Append(Szunyi.Text.General.GetText(SMRT_IDs, ",")).AppendLine()
        Next
        Clipboard.SetText(str.ToString)
        Beep()

    End Sub

    Private Sub DataGridView1_CellContentClick(sender As Object, e As DataGridViewCellEventArgs) Handles DataGridView1.CellContentClick

    End Sub



#End Region
End Class
Public Class Ref_Files_Bam_Files
    Public Property RefFiles As New List(Of FileInfo)
    Public Property BamFiles As New List(Of FileInfo)
    Public Property All_Files As New List(Of FileInfo)
    Public Sub New(RefSeq_Folder As DirectoryInfo, All_Files As List(Of FileInfo), SMRTIDs As List(Of String))
        Me.All_Files = All_Files
        Me.RefFiles = Szunyi.IO.Files.Filter.SelectFiles("Reference Fasta", Szunyi.Constants.Files.Fasta, RefSeq_Folder)
        If IsNothing(RefFiles) = True Then Exit Sub
        Dim Seqs = Szunyi.IO.Import.Sequence.FromFiles(RefFiles)
        Dim SeqIDs = (From x In Seqs Select x.ID).ToList
        Dim BAMs = Get_Files(".bam", SMRTIDs)
        BamFiles = Szunyi.IO.Files.Filter.SelectFiles(BAMs, SeqIDs)
        Dim alf As Int16 = 43
    End Sub

    ''' <summary>
    ''' Return files with extension Of Selected SMRT
    ''' </summary>
    ''' <param name="Extension"></param>
    ''' <returns></returns>
    Private Function Get_Files(Extension As String, SMRTIDs As List(Of String)) As List(Of FileInfo)
        Dim e = From x In Me.All_Files Where x.Extension = Extension
        Dim out As New List(Of FileInfo)
        Dim SElSMRTs As New List(Of String)

        For Each f In e
            For Each SMrt In SMRTIDs
                If f.Name.Contains(SMrt) Then
                    out.Add(f)
                End If
            Next
        Next
        Return out
    End Function

End Class
Public Class Variations
    Public Property Positions As New List(Of Variation)
    Public Property Seq As Bio.ISequence
    Public Property File As FileInfo
    Public Sub New(Seq As Bio.ISequence, File As FileInfo)
        Me.Seq = Seq
        Me.File = File
        For i1 = 0 To Seq.Count - 1
            Positions.Add(New Variation(i1, Seq.Item(i1)))
        Next
        For Each Line In Szunyi.IO.Import.Text.ParseNotFirst(File, 1)
            Dim s = Split(Line, vbTab)
            Me.Positions(s(1) - 1).A = s(13)
            Me.Positions(s(1) - 1).C = s(15)
            Me.Positions(s(1) - 1).G = s(17)
            Me.Positions(s(1) - 1).T = s(19)
        Next
    End Sub
End Class
Public Class Variation
    Public Property Pos As Integer
    Public Property Ref As Char
    Public Property A As Integer
    Public Property C As Integer
    Public Property T As Integer
    Public Property G As Integer
    Public Sub New(x As Integer, Ref As Byte)
        Me.Pos = x
        Me.Ref = Chr(Ref)
        Dim alf As Int16 = 43
    End Sub
End Class
