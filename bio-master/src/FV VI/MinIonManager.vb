Imports System.ComponentModel
Imports System.IO
Imports ClassLibrary1

Public Class MinIonManager
    Public Property Descriptions As New BindingList(Of MinIon_Description)
    Public Property Selected As New BindingList(Of MinIon_Description)
    Public Property FastQ_Files As New List(Of FileInfo)
    Public Property Sam_Files As New List(Of FileInfo)
    Public Property Bam_Files As New List(Of FileInfo)
    Public Property Basic_Folder As New DirectoryInfo("c:/MinIon_All_Data")
    Public Property All_Values1 As List(Of String)
    Private Sub MinIonManager_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        Dim File As New FileInfo(Basic_Folder.FullName & "/Sequencing data - Minion.tsv")
        Dim Header = Szunyi.IO.Import.Text.GetHeader(File, 1)
        For Each Line In Szunyi.IO.Import.Text.Parse(File, 1)
            If IsNothing(Line) = False Then
                Me.Descriptions.Add(New MinIon_Description(Line))
                Me.Selected.Add(New MinIon_Description(Line))
            End If
        Next
        Read_Files()
        DataGridView1.DataSource = Me.Descriptions
        All_Values1 = Get_All_Values(Me.Descriptions)
        ' Put each of the columns into programmatic sort mode.
        For Each column As DataGridViewColumn In DataGridView1.Columns
            column.SortMode = DataGridViewColumnSortMode.Programmatic
        Next

    End Sub
    ''' <summary>
    ''' Return All Fastq File from Predefine Directory
    ''' </summary>
    ''' <returns></returns>
    Private Function Get_FastQ_ByRunIDs(Optional FastQDir As DirectoryInfo = Nothing) As List(Of FileInfo)
        If IsNothing(FastQDir) = True Then FastQDir = New DirectoryInfo(Basic_Folder.FullName & "/FastQByRunID")
        Dim r = FastQDir.GetFiles.ToList
        Return r
    End Function
    Private Sub Read_Files()
        Try
            Dim FastQDir As New DirectoryInfo(Basic_Folder.FullName & "/fastQ")
            Dim r = FastQDir.GetFiles.ToList
            Me.FastQ_Files = (From x In r Where x.Extension = ".fastq").ToList
        Catch ex As Exception

        End Try
        Try
            Dim FastQDir As New DirectoryInfo(Basic_Folder.FullName & "/Mappings_SAM")
            Dim r = FastQDir.GetFiles.ToList
            Me.Sam_Files = (From x In r Where x.Extension = ".sam").ToList

        Catch ex As Exception

        End Try
        Try
            Dim FastQDir As New DirectoryInfo(Basic_Folder.FullName & "/Mappings_BAM")
            Dim r = FastQDir.GetFiles.ToList
            Me.Sam_Files = (From x In r Where x.Extension = ".bam" Or x.Extension = ".bai").ToList

        Catch ex As Exception

        End Try

    End Sub
#Region "Sg"
    Private Sub IntoDictionaryToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles IntoDictionaryToolStripMenuItem.Click
        ' Get Folder
        ' Get Filenames Group by before _GMAP and after last after _
        ' Create Dictionaries
        ' Copies FIles
        Dim Folder = ClassLibrary1.Szunyi.IO.Directory.Get_Folder
        Into_Dictionaries(Folder)

    End Sub
    Private Function Into_Dictionaries(Folder As DirectoryInfo) As List(Of DirectoryInfo)
        ' Get Folder
        ' Get Filenames Group by before _GMAP and after last after _
        ' Create Dictionaries
        ' Copies FIles
        ' Return all Created Directory for subproccessing
        Dim FIles = Folder.GetFiles
        Dim sg = From x In FIles Select x Group By Split(x.Name, "_GMAP").First.Split("_").Last Into Group
        Dim SubFolers As New List(Of DirectoryInfo)

        For Each File In sg
            Dim Path As String = Folder.FullName & "\" & File.Last
            IO.Directory.CreateDirectory(Path)
            SubFolers.Add(New DirectoryInfo(Path))
            For Each cFile In File.Group
                cFile.MoveTo(Path & "\" & cFile.Name)
            Next
        Next
        Return SubFolers
    End Function
    Private Sub SamsIntoDIctionaryToBamSOrtIndexToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles SamsIntoDIctionaryToBamSOrtIndexToolStripMenuItem.Click
        ' Get Folder
        ' Get Filenames Group by before _GMAP and after last after _
        ' Create Dictionaries
        ' Copies FIles
        ' Return all Created Directory for subproccessing
        Dim Folder = ClassLibrary1.Szunyi.IO.Directory.Get_Folder
        If IsNothing(Folder) = True Then Exit Sub
        Dim str As New System.Text.StringBuilder
        For Each sFolder In Into_Dictionaries(Folder)
            Dim FIles = From x In sFolder.GetFiles Where x.Extension = ".sam"
            If FIles.Count > 0 Then
                str.Append(Szunyi.Linux.SamTools.Sam_to_Bam_Sort_Index(FIles.ToList)).AppendLine()
            End If
        Next
        Clipboard.SetText(str.ToString)
    End Sub

    Private Sub SamToBamFromSubDirectoriesToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles SamToBamFromSubDirectoriesToolStripMenuItem.Click
        Dim FOlder = Szunyi.IO.Directory.Get_Folder
        Dim Files = Szunyi.IO.Directory.GetAllFilesFromFolder_Recursive(FOlder.FullName)
        Dim Sam_Files = From x In Files Where x.Extension = ".sam"

        If Sam_Files.Count > 0 Then
            Clipboard.SetText(Szunyi.Linux.SamTools.Sam_to_Bam_Sort_Index(Sam_Files.ToList))
        End If


    End Sub

    Private Sub OpenToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles OpenToolStripMenuItem.Click
        Dim Folder = Szunyi.IO.Directory.Get_Folder
        If IsNothing(Folder) = True Then Exit Sub
        Dim File As New FileInfo(Folder.FullName & "/Sequencing data - Minion.tsv")
        Dim Header = Szunyi.IO.Import.Text.GetHeader(File, 1)
        For Each Line In Szunyi.IO.Import.Text.Parse(File, 1)
            Me.Descriptions.Add(New MinIon_Description(Line))
        Next

    End Sub

    Private Sub GetFlowCellIDsAndRunIDsToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles GetFlowCellIDsAndRunIDsToolStripMenuItem.Click
        Dim Folder = Szunyi.IO.Directory.Get_Folder ' reads
        If IsNothing(Folder) = True Then Exit Sub

        Dim str As New System.Text.StringBuilder
        Dim Dict As New Dictionary(Of String, String)
        For Each d In Folder.GetDirectories
            str.Append(d.Name).Append(vbTab)
            str.Append(d.CreationTime).Append(vbTab)
            Dim RunID As String = ""
            Try
                Dim fq As New DirectoryInfo(d.FullName & "\fastq\pass\")
                If fq.Exists = False Then
                    str.Append("No Pass dir").Append(vbTab)
                Else
                    Dim fq1 = fq.GetFiles.First
                    RunID = Split(fq1.Name, "_")(2)
                    str.Append(RunID).Append(vbTab)
                End If
            Catch ex As Exception
                str.Append(vbTab)
            End Try
            Dim FlowCellID As String = ""
            Try
                Dim fq As New DirectoryInfo(d.FullName & "\fast5\pass\")
                If fq.Exists = False Then
                    Dim kj As Int16 = 54
                    fq = New DirectoryInfo(d.FullName & "\fast5\skip\")
                    If fq.Exists = True Then
                        Dim jh As Int16 = 54
                    Else
                        Dim jkh As Int16 = 54
                    End If
                End If
                Dim dfq = fq.GetDirectories.First
                Dim fq1 = dfq.GetFiles.First
                FlowCellID = Split(fq1.Name, "_")(3)
                str.Append(RunID).Append(vbTab)
                str.Append(FlowCellID).Append(vbTab)
            Catch ex As Exception
                str.Append(vbTab)
            End Try
            Try
                Dim fq As New DirectoryInfo(d.FullName & "\fast5\skip\")
                Dim dfq = fq.GetDirectories.First
                Dim fq1 = dfq.GetFiles.First
                Dim s2 = Szunyi.IO.Import.Text.ReadToEnd(fq1)
                Dim regexp As String = "[a-z0-9]{40}"
                Dim m = System.Text.RegularExpressions.Regex.Match(s2, regexp)
                For Each r In m.Groups
                    str.Append(r.value)
                Next
            Catch ex As Exception
                str.Append(vbTab)
            End Try
            Dict.Add(d.Name, RunID & vbTab & FlowCellID)
            str.AppendLine()
        Next
        If str.Length > 0 Then         Clipboard.SetText(str.ToString)
        Dim s = Clipboard.GetText
        Dim s1 = Split(s, vbCrLf)
        Dim str2 As New System.Text.StringBuilder
        For Each item In s1
            str2.Append(item).Append(vbTab)
            If Dict.ContainsKey(item) Then str2.Append(Dict(item))
            str2.AppendLine.AppendLine()
        Next

        Clipboard.SetText(str2.ToString)
    End Sub

#End Region


#Region "Selection"
    Private Function Select_SMRTs() As List(Of String)

        Dim s As New CheckBoxForStringsFull(All_Values1, -1)
        If s.ShowDialog = DialogResult.OK Then
            Return s.SelectedStrings
        End If
        Return New List(Of String)
    End Function
    Private Function Get_All_Values(x As BindingList(Of MinIon_Description)) As List(Of String)
        Dim out As New List(Of String)
        For Each obj In x
            For Each p As System.Reflection.PropertyInfo In obj.GetType().GetProperties()
                If p.CanRead Then
                    Try
                        out.Add(p.Name & " : " & p.GetValue(obj, Nothing))
                    Catch ex As Exception

                    End Try

                End If
            Next
        Next
        Dim tmp = out.Distinct.ToList
        Return tmp
    End Function
    Private Sub Set_Selected_SMRTIDS(ToSearch As List(Of String), All_Must_Contains As Boolean)
        Me.Selected.Clear()
        If All_Must_Contains = True Then
            Dim Valids As New List(Of MinIon_Description)
            For Each gr In ToSearch
                Dim Prop = gr.Split(" : ").First
                Dim Val = gr.Split(" : ").Last
                Dim tmp As New List(Of MinIon_Description)
                For Each obj In Me.Descriptions
                    For Each p As System.Reflection.PropertyInfo In obj.GetType().GetProperties()
                        If p.CanRead Then
                            If p.Name = Prop AndAlso p.GetValue(obj, Nothing) = Val Then
                                tmp.Add(obj)
                                Exit For
                            End If
                        End If
                    Next
                Next
                If gr = ToSearch.First Then
                    Valids.AddRange(tmp)
                Else
                    Valids = Valids.Intersect(tmp).ToList
                End If
            Next
            For Each i In Valids
                Selected.Add(i)
            Next
        Else
            For Each gr In ToSearch
                Dim Prop = gr.Split(" : ").First
                Dim Val = gr.Split(" : ").Last
                For Each obj In Me.Descriptions
                    For Each p As System.Reflection.PropertyInfo In obj.GetType().GetProperties()
                        If p.CanRead Then
                            If p.Name = Prop AndAlso p.GetValue(obj, Nothing) = Val Then
                                If Selected.Contains(obj) = False Then Selected.Add(obj)
                                Exit For
                            End If
                        End If
                    Next
                Next

            Next
        End If
        Me.DataGridView1.DataSource = Selected
    End Sub

    Private Sub AnyToolStripMeGet_All_ValuesnuItem_Click(sender As Object, e As EventArgs) Handles AnyToolStripMenuItem.Click

        Dim s = Select_SMRTs()
        Set_Selected_SMRTIDS(s, False)
    End Sub

    Private Sub AllToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles AllToolStripMenuItem.Click
        Dim s = Select_SMRTs()
        Set_Selected_SMRTIDS(s, True)
    End Sub

    Private Sub FromFileToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles FromFileToolStripMenuItem.Click
        Dim All_Values = Get_All_Values(Me.Descriptions)
        Set_Selected_SMRTIDS(All_Values, True)
    End Sub
#End Region


    Private Sub FastQByRunIDToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles FastQByRunIDToolStripMenuItem.Click
        Try
            Dim RunIDs = (From x In Me.Selected Where x.Selected = True And x.Run_ID <> "" Select x.Run_ID).Distinct.ToList
            Dim Selected_Fastq_Files =  Szunyi.IO.Files.Filter.SelectFiles(Me.FastQ_Files, RunIDs)
            For Each RunID In RunIDs
                Dim cFastQ_Files = From x In Selected_Fastq_Files Where x.Name.Contains(RunID)
                Dim nFile As New FileInfo("C:/MinIon All Data/FastQByRunID/" & RunID & ".fastq")
                Using sw As New StreamWriter(nFile.FullName)
                    For Each File In cFastQ_Files
                        For Each line In Szunyi.IO.Import.Text.Parse(File, 0)
                            line = line.Replace("U", "T")
                            sw.Write(line)
                            sw.WriteLine()
                        Next
                    Next
                End Using
            Next
        Catch ex As Exception
            MsgBox(ex.ToString)
        End Try

    End Sub

    Private Sub FastQBySelectionToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles FastQBySelectionToolStripMenuItem.Click

    End Sub

    Private Sub ResetToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles ResetToolStripMenuItem.Click
        Me.DataGridView1.DataSource = Me.Descriptions
    End Sub

    Private Sub ByRunIDsAndOrgsFromSamsToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles ByRunIDsAndOrgsFromSamsToolStripMenuItem.Click
        ' Get currently Selected RunIDs
        ' Get Sam Files
        ' Group By Organism
        ' Create Induvidual Stats
        ' Create Merge Stat
        ' Save
        Dim Header = Split("Organism,RunId,File Name, Nof Read, Nof Mapping, Avr. Read Length,Avr. Read Length SD, Avr. Aligned Read Length,Avr. Aligned Read Length SD,Deleteion %, Deleteion % SD, Insertion %,Insertion % SD, Match%,Match% SD, MisMatch%, MisMatch% SD,Coverage", ",")
        Dim str As New System.Text.StringBuilder
        str.Append(Szunyi.Text.General.GetText(Header, vbTab))
        Dim RunIDs = Get_Selected_RunIDs()
        Dim Sam_Files =  Szunyi.IO.Files.Filter.SelectFiles(Me.Sam_Files, RunIDs)
        For Each g In Szunyi.IO.Files.Group.Iterate_By_First_Parts(Sam_Files, "_GMAP_")
            Dim All_Sams As New List(Of Bio.IO.SAM.SAMAlignedSequence)
            For Each FIle In g
                str.AppendLine()
                Dim Sams = Szunyi.BAM.Bam_Basic_IO.Import.Parse(FIle)
                All_Sams.AddRange(Sams)
                Dim GoodSams = (From x In Sams Where x.Flag <> Bio.IO.SAM.SAMFlags.UnmappedQuery).ToList
                str.Append(Split(FIle.Name, "_GMAP_").First).Append(vbTab)
                str.Append(Split(FIle.Name, "_GMAP_").Last).Append(vbTab)
                str.Append(Szunyi.BAM.Stat.Stat.All(GoodSams, FIle))
            Next
            str.AppendLine()
            str.Append(Split(g.First.Name, "_GMAP_").First).Append(vbTab).Append("_Aggregate").Append(vbTab)
            str.Append(Szunyi.BAM.Stat.Stat.All(All_Sams, g.First))
        Next
        Clipboard.SetText(str.ToString)
        Beep()
    End Sub
    Private Function Get_Selected_RunIDs() As List(Of String)
        Try
            Dim RunIDs = (From x In Me.Selected Where x.Run_ID <> "" Select x.Run_ID).Distinct.ToList
            Return RunIDs
        Catch ex As Exception
            Return New List(Of String)
        End Try

    End Function
#Region "Check"
    Private Sub SamFilesToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles SamFilesToolStripMenuItem.Click
        Dim RunIDs = Get_Selected_RunIDs()
        Dim Sam_Files =  Szunyi.IO.Files.Filter.SelectFiles(Me.Sam_Files, RunIDs)
        Dim str As New System.Text.StringBuilder
        Dim log As New System.Text.StringBuilder
        For Each item In Me.Descriptions
            For Each VirusID In item.VirusIDs
                If VirusID <> "" Then
                    Dim res = From x In Sam_Files Where x.Name.Contains(item.Run_ID) AndAlso x.Name.Contains(VirusID)

                    If res.Count = 0 Then
                        log.Append(item.Run_ID).Append(vbTab).Append(VirusID).AppendLine()
                    End If
                End If
            Next
        Next
        Dim dict As New Dictionary(Of String, List(Of String)) ' RunID, Org Name
        Dim Orgs As New List(Of String)
        For Each g In Szunyi.IO.Files.Group.Iterate_By_First_Parts(Sam_Files, "_GMAP_")
            For Each File In g
                Dim RunID = File.Name.Split("_GMAP_").Last.Replace(File.Extension, "")
                Dim Org = File.Name.Split("_GMAP_").First
                Orgs.Add(Org)
                If dict.ContainsKey(RunID) = False Then dict.Add(RunID, New List(Of String))
                dict(RunID).Add(Org)
            Next
        Next
        str.Append(vbTab).Append(Szunyi.Text.General.GetText(RunIDs, vbTab))

        For Each org In Orgs
            str.AppendLine.Append(org)
            For Each RunID In RunIDs
                str.Append(vbTab)
                If dict.ContainsKey(RunID) = False Then
                    str.Append("-")
                Else
                    If dict(RunID).Contains(org) = True Then
                        str.Append("+")
                    Else
                        str.Append("-")
                    End If
                End If

            Next
        Next
        Clipboard.SetText(str.ToString)
        Beep()
    End Sub
    Private Sub FromTableToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles FromTableToolStripMenuItem.Click
        Dim RunIDs = Get_Selected_RunIDs()
        Read_Files()
        Dim Sam_Files = Me.Sam_Files
        Dim FastQ_Files =  Szunyi.IO.Files.Filter.SelectFiles(Me.FastQ_Files, RunIDs)
        Dim str As New System.Text.StringBuilder
        For Each Item In Me.Selected
            Dim res = From x In Sam_Files Where x.Name.Contains(Item.Run_ID)

            Dim VirusIDs = Item.VirusIDs
            For Each V In Item.GMAP_Names
                Dim r2 = From x In res Where x.Name.Contains(V)

                If r2.Count = 0 Then
                    str.Append("gmap -d ").Append(V).Append(" --nofails -f samse -t 7 /mnt/c/MinIon\ All\ Data/FastQByRunID/").Append(Item.Run_ID).Append(".fastq")
                    str.Append(" > /mnt/c/MinIon\ All\ Data/Mappings_SAM/").Append(V).Append("_GMAP_").Append(Item.Run_ID).Append(".sam").AppendLine()
                Else
                    Dim kj As Int16 = 43
                End If
            Next

        Next
        Clipboard.SetText(str.ToString)
    End Sub
    Private Sub FastQToolStripMenuItem2_Click(sender As Object, e As EventArgs) Handles FastQToolStripMenuItem2.Click
        Dim str As New System.Text.StringBuilder
        str.Length = 0
        Dim RunIDs = Get_Selected_RunIDs()
        Dim Files = Get_FastQ_ByRunIDs()

        For Each RunID In RunIDs
            Dim r = From x In Files Where x.Name.Contains(RunID)

            If r.Count = 0 Then
                str.Append(RunID).AppendLine()
            End If

        Next
        Clipboard.SetText(str.ToString)
    End Sub
    Private Sub SAMHasBeenPutToBamToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles SAMHasBeenPutToBamToolStripMenuItem.Click
        Dim Sam_Files = Get_Sam_Files_From_Mappings_Sam_Folder()
        Dim Bam_Files = Get_Bam_Files_From_Mappings_Bam_Folder()
        Dim str As New System.Text.StringBuilder
        Dim out As New List(Of FileInfo)
        For Each S In Sam_Files
            Dim res = From x In Bam_Files Where x.Name.Contains(Split(S.Name, ".").First)

            If res.Count = 0 Then
                out.Add(S)
            Else
                Dim kj As Int16 = 54

            End If
        Next
        Dim k = Get_BB(out)
    End Sub
    Private Function Get_BB(samfiles As List(Of FileInfo)) As String
        Dim str As New System.Text.StringBuilder
        str.Length = 0
        For Each File In samfiles
            Dim LinuxFIleName = Szunyi.IO.Linux.Get_FileName(File)
            Dim BamFIleName = Szunyi.IO.Linux.Get_FileName(New FileInfo(File.FullName.Replace(".sam", ".bam")))
            Dim SortedBamFileName = Szunyi.IO.Linux.Get_FileName(New FileInfo(File.FullName.Replace(".sam", ".sorted.bam")))
            str.Append("samtools view -bS " & LinuxFIleName & " | ")

            str.Append("samtools sort" & " > " & SortedBamFileName).AppendLine()
            str.Append("samtools index " & SortedBamFileName & "").AppendLine()

        Next
        Clipboard.SetText(str.ToString)
        Beep()
        Return str.ToString
    End Function

    Private Sub BamToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles BamToolStripMenuItem.Click
        Dim RunIDs = Get_Selected_RunIDs()
        Dim cBamFiles = Szunyi.IO.Files.Filter.SelectFiles(Me.Sam_Files, RunIDs)
        Dim BasicDir = Szunyi.IO.Directory.Get_Folder()
        Dim gr = From x In cBamFiles Group By Split(x.Name, "_GMAP_").First Into Group

        For Each Org In gr
            Dim d1 As New DirectoryInfo(BasicDir.FullName & "\" & Org.First)
            d1.Create()
            For Each File In Org.Group
                File.CopyTo(d1.FullName & "\" & File.Name)
            Next
        Next

    End Sub
#End Region

    Private Sub FastQByRunIDFromFIlesToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles FastQByRunIDFromFIlesToolStripMenuItem.Click
        Dim files =  Szunyi.IO.Files.Filter.SelectFiles(Szunyi.Constants.Files.Fasta_FastQ)
        If IsNothing(files) = True Then Exit Sub
        Dim out As New Dictionary(Of String, List(Of FileInfo))
        For Each File In files
            Dim RunID = Szunyi.MinIon.Common.Get_RunID_From_FastQ_File(File)
            If out.ContainsKey(RunID) = False Then out.Add(RunID, New List(Of FileInfo))
            out(RunID).Add(File)

        Next
        For Each o In out
            Dim nFile As New FileInfo(Me.Basic_Folder.FullName & "/FastQByRunID/" & o.Key & ".fastq")
            Szunyi.IO.Files.Move_Copy.MergeFiles(o.Value, nFile)

        Next
    End Sub

    Private Sub FastqBySeqIdIntoRunIdToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles FastqBySeqIdIntoRunIdToolStripMenuItem.Click
        Dim files =  Szunyi.IO.Files.Filter.SelectFiles(Szunyi.Constants.Files.FastQ)
        If IsNothing(files) = True Then Exit Sub
        Dim out As New Dictionary(Of String, List(Of FileInfo))
        Dim Writters As New Dictionary(Of String, FileStream)
        Dim x As New Bio.IO.FastQ.FastQFormatter
        For Each File In files
            For Each seq As Bio.QualitativeSequence In Szunyi.IO.Import.Sequence.GetOnyByONe(File)
                Dim RunID = Szunyi.MinIon.Common.Get_RunID_From_SeqID(seq.ID)
                If Writters.ContainsKey(RunID) = False Then
                    Writters.Add(RunID, New FileStream(File.DirectoryName & "\" & RunID & ".fastq", FileMode.Create, FileAccess.ReadWrite))
                End If
                x.Format(Writters(RunID), seq)
            Next
        Next
        For Each w In Writters
            w.Value.Flush()
            w.Value.Close()
        Next
    End Sub

    Private Function Get_Sam_Files_From_Mappings_Sam_Folder() As List(Of FileInfo)
        Dim FastQDir As New DirectoryInfo(Basic_Folder.FullName & "/Mappings_SAM")
        Dim r = FastQDir.GetFiles.ToList
        Return (From x In r Where x.Extension = ".sam").ToList
    End Function
    Private Function Get_Bam_Files_From_Mappings_Sam_Folder() As List(Of FileInfo)
        Dim FastQDir As New DirectoryInfo(Basic_Folder.FullName & "/Mappings_BAM")
        Dim r = FastQDir.GetFiles.ToList
        Return (From x In r Where x.Extension = ".sam").ToList
    End Function
    Private Function Get_Bam_Files_From_Mappings_Bam_Folder() As List(Of FileInfo)
        Dim FastQDir As New DirectoryInfo(Basic_Folder.FullName & "/Mappings_BAM")
        Dim r = FastQDir.GetFiles.ToList
        Return (From x In r Where x.Extension = ".bam").ToList
    End Function

    Private Sub SkippedFast5ToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles SkippedFast5ToolStripMenuItem.Click
        Dim f = Szunyi.IO.Directory.Get_Folder
        Dim f2 As New DirectoryInfo(f.FullName & "\skipped")
        f2.Create()
        For Each f1 In f.GetDirectories
            Dim f5 = f1.GetDirectories("fast5")
            If f5.Count > 0 Then
                Dim skipped = f5.First.GetDirectories("skip")
                If skipped.Count > 0 Then
                    For Each sk In skipped.First.GetDirectories

                        For Each FIle In sk.GetFiles
                            FIle.CopyTo(f2.FullName & "\" & FIle.Name)
                        Next
                    Next
                End If
            End If
        Next
    End Sub

    Private Sub FromOriginalMinIonFolderToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles FromOriginalMinIonFolderToolStripMenuItem.Click
        Dim RunIDs = Get_Selected_RunIDs()

        Dim d As New DirectoryInfo("C:\MinION_Data\reads\")
        For Each cDir In d.GetDirectories
            Dim sDIr As New DirectoryInfo(cDir.FullName & "\fastq\pass")
            If sDIr.Exists = True Then

                Dim FIles = sDIr.GetFiles.ToList
                If RunIDs.Contains(Szunyi.MinIon.Common.Get_RunID_From_FastQ_File(FIles.First)) = True Then
                    Dim nFIle As New FileInfo("C:\MinIon All Data\FastQByRunID\" & Szunyi.MinIon.Common.Get_RunID_From_FastQ_File(FIles.First) & ".fastq")
                    Szunyi.IO.Files.Move_Copy.MergeFiles(FIles, nFIle)
                End If
            End If
        Next
    End Sub

    Private Sub MergeSkippedToPassedToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles MergeSkippedToPassedToolStripMenuItem.Click
        ' 1 Get Files
        ' 2, Check is there a same file in C:\MinIon All Data\FastQByRunID\
        ' 3 Merge or Move
        Dim FIles =  Szunyi.IO.Files.Filter.SelectFiles(Szunyi.Constants.Files.FastQ)
        Dim ByRunID = Get_FastQ_ByRunIDs()
        For Each FIle In FIles
            Dim r = From x In ByRunID Where x.Name = FIle.Name

            If r.Count = 0 Then
                FIle.MoveTo(ByRunID.First.DirectoryName & "\" & FIle.Name)

            Else
                Szunyi.IO.Files.Move_Copy.MergeFiles(r.First, FIle)
            End If
        Next
    End Sub

    Private Sub CheckFastqInTableToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles CheckFastqInTableToolStripMenuItem.Click
        Dim FQ = Get_FastQ_ByRunIDs()
        Dim str As New System.Text.StringBuilder
        For Each f In FQ
            Dim d = Split(f.Name, ".fastq").First
            Dim x = From h In Me.Descriptions Where h.Run_ID = d

            If x.Count = 0 Then
                str.Append(d).AppendLine()
            End If

        Next
        Clipboard.SetText(str.ToString)
    End Sub

    Private Sub GMAPToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles GMAPToolStripMenuItem.Click
        Dim Sam_Files = Get_Sam_Files_From_Mappings_Sam_Folder()
        Dim FQ_Files = Get_FastQ_ByRunIDs()
        Dim str As New System.Text.StringBuilder
        Dim log As New System.Text.StringBuilder
        For Each item In Me.Selected
            For Each GMAP_Name In item.GMAP_Names
                Dim file = From x In Sam_Files Where x.Name.Contains(GMAP_Name) And x.Name.Contains(item.Run_ID)

                If file.Count = 0 Then ' then do GMAP
                    Try
                        Dim ReadFIle = (From X In FQ_Files Where X.Name.Contains(item.Run_ID)).First
                        str.Append(Szunyi.Outer_Programs.GMAP.Get_s(ReadFIle, GMAP_Name, New DirectoryInfo(Me.Basic_Folder.FullName & "\Mappings_SAM"))).AppendLine()
                    Catch ex As Exception
                        log.Append(item.Run_ID).AppendLine()
                    End Try


                End If
            Next
        Next
        If str.Length <> 0 Then Clipboard.SetText(str.ToString)
    End Sub
    Private Sub FastQIntoFlowCellIDsToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles FastQIntoFlowCellIDsToolStripMenuItem.Click
        Dim FQ_Files = Get_FastQ_ByRunIDs()
        Dim All_Values = Get_All_Values(Me.Descriptions)
        Dim Dir = Szunyi.IO.Directory.Get_Folder
        Dim RealFQs As New List(Of FileInfo)
        Dim x = Get_Selected_RunIDs()
        For Each fw In FQ_Files
            For Each x1 In x
                If fw.Name.Contains(x1) = True Then
                    RealFQs.Add(fw)
                End If
            Next
        Next
        For Each F In RealFQs
            F.CopyTo(Dir.FullName & "\" & F.Name)
        Next

    End Sub


    Private Sub GetAllInfoToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles GetAllInfoToolStripMenuItem.Click
        Dim Files =  Szunyi.IO.Files.Filter.SelectFiles(Szunyi.Constants.Files.Fast5)
        If IsNothing(Files) = True Then Exit Sub
        Dim DIR = Szunyi.IO.Directory.Get_Folder
        Dim FIrtsFast5FIles = Szunyi.MinIon.Common.Get_First_Passed_fast5_Files(DIR)
        Dim x = Szunyi.HDF5.Common.Get_Attributes_with_Values(FIrtsFast5FIles, "UniqueGlobalKey/context_tags")
        Dim x1 = Szunyi.HDF5.Common.Get_Attributes_with_Values(FIrtsFast5FIles, "UniqueGlobalKey/tracking_id")
        Dim ttt = Szunyi.Text.General.GetText(x1)
        Dim kj As Int16 = 54
    End Sub

    Private Sub BarCodesToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles BarCodesToolStripMenuItem.Click
        Dim Dir = Szunyi.IO.Directory.Get_Folder
        If IsNothing(Dir) = True Then Exit Sub
        For Each SubDir In Dir.GetDirectories
            Dim SaveFile As New FileInfo(Dir.FullName & "\" & SubDir.Name & ".fastq")
            Dim FIles = SubDir.GetFiles.ToList
            Szunyi.IO.FilesAndFolder.Convert.Merge_Files(FIles, SaveFIle)
        Next
    End Sub

    Private Sub IntoFolderToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles IntoFolderToolStripMenuItem.Click

        Dim FQ_Files = Get_FastQ_ByRunIDs()
        Dim Dir = Szunyi.IO.Directory.Get_Folder
        Dim RealFQs As New List(Of FileInfo)
        Dim x = Get_Selected_RunIDs()
        For Each fw In FQ_Files
            For Each x1 In x
                If fw.Name.Contains(x1) = True Then
                    RealFQs.Add(fw)
                End If
            Next
        Next
        For Each F In RealFQs
            F.CopyTo(Dir.FullName & "\" & F.Name)
        Next
    End Sub

    Private Sub MergeIntoFIleToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles MergeIntoFIleToolStripMenuItem.Click
        Dim FQ_Files = Get_FastQ_ByRunIDs()
        Dim RealFQs As New List(Of FileInfo)
        Dim x = Get_Selected_RunIDs()
        Dim notFound As New List(Of String)
        For Each x1 In x
            Dim f As Boolean = False
            For Each fw In FQ_Files
                If fw.Name.Contains(x1) = True Then
                    RealFQs.Add(fw)
                    f = True
                    Exit For
                End If
            Next
            If f = False Then
                notFound.Add(x1)
            End If
        Next
        Dim SaveFile = Szunyi.IO.Files.Save.SelectSaveFile(Szunyi.Constants.Files.FastQ)
        '  Dim d = RealFQs.Distinct(x)
        If RealFQs.Count = Me.DataGridView1.Rows.Count Then
            Szunyi.IO.Files.Move_Copy.MergeFiles(RealFQs, SaveFile)
        Else
            MsgBox(Szunyi.Text.General.GetText(notFound))
            Szunyi.IO.Files.Move_Copy.MergeFiles(RealFQs, SaveFile)
        End If

    End Sub

    Private Sub DataGridView1_ColumnHeaderMouseClick(sender As Object, e As DataGridViewCellMouseEventArgs) Handles DataGridView1.ColumnHeaderMouseClick
        ' Check which column is selected, otherwise set NewColumn to Nothing.
        Dim newColumn As DataGridViewColumn =
        DataGridView1.Columns(e.ColumnIndex)


        Dim oldColumn As DataGridViewColumn = DataGridView1.SortedColumn
        Dim direction As ListSortDirection

        ' If oldColumn is null, then the DataGridView is not currently sorted.
        If oldColumn IsNot Nothing Then

            ' Sort the same column again, reversing the SortOrder.
            If oldColumn Is newColumn AndAlso DataGridView1.SortOrder =
                SortOrder.Ascending Then
                direction = ListSortDirection.Descending
            Else

                ' Sort a new column and remove the old SortGlyph.
                direction = ListSortDirection.Ascending
                oldColumn.HeaderCell.SortGlyphDirection = SortOrder.None
            End If
        Else
            direction = ListSortDirection.Ascending
        End If
    End Sub
End Class
Public Class MinIon_Description
    Public Property Selected As Boolean = True
    Public Property Starting_Date As String
    Public Property Description As String
    Public Property Run_ID As String
    Public Property Flowcell_ID As String
    Public Property Sample_type As String
    Public Property Virus As String
    Public Property VirusIDs As New List(Of String)
    Public Property GMAP_Names As New List(Of String)
    Public Property Post_infection_time As String
    Public Property Host_Cell_Line As String
    Public Property Host As String
    Public Property RNS_Selection As String
    Public Property Amplification_Type As String
    Public Property Amplification_Count As Integer
    Public Property Size_Selection As String
    Public Property BarCode As String
    Public Sub New(Line As String)
        Dim s = Split(Line, vbTab)
        Starting_Date = s(0)
        Description = s(1)
        Run_ID = s(2)
        Flowcell_ID = s(3)
        Sample_type = s(4)
        Virus = s(5)
        VirusIDs = s(6).Split(",").ToList
        GMAP_Names = s(7).Split(",").ToList

        Post_infection_time = s(8)
        Host_Cell_Line = s(9)
        Host = s(10)
        RNS_Selection = s(11)
        Amplification_Type = s(12)
        Amplification_Count = s(13)
        Size_Selection = s(14)
        BarCode = s(15)
    End Sub
End Class