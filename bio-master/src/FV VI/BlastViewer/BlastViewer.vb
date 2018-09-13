Imports System.IO
Imports Bio
Imports System.Text
Imports Bio.Web.Blast
Imports Bio.IO.GenBank
Imports System.Text.RegularExpressions
Imports System.ComponentModel
Imports ClassLibrary1.Szunyi.Blast
Imports ClassLibrary1.Szunyi.Blast.Helper.Paths
Imports ClassLibrary1
Imports ClassLibrary1.Szunyi.Features.FeatureManipulation
Imports ClassLibrary1.Szunyi.Sequences.SequenceManipulation

Public Class BlastViewer
    Dim LocBuilder As New LocationBuilder

    Dim TheBlastResult As New List(Of Bio.Web.Blast.BlastResult)
    ''' <summary>
    ''' This is always containig the original result
    ''' </summary>
    Dim OriginalBlastSearchResults As New List(Of Bio.Web.Blast.BlastResult)
    ''' <summary>
    ''' This is contains the filtered Blast Results which can be Reset
    ''' </summary>
    Dim OriginalBlastSearchRecords As New List(Of Bio.Web.Blast.BlastSearchRecord)
    Dim ClonedAndFilteredBlastSearchRecords As New List(Of Bio.Web.Blast.BlastSearchRecord)

    Dim GetInputBox As New Szunyi.MyInputBox


    Private MinimalLengthOfMatch As Integer = 0
    Private MinimalPercentOfQuery As Integer = 0
    Private CharOFTbAlignemnt As Integer
    Private evalue As Double = 0.0001
    Private minIdentityPercent As Double = 0.95
    Private SortOrders(25) As Integer

    Dim Pool As New List(Of String())
    Dim Selection As New Dictionary(Of String, List(Of Hit))

    Private Features As Dictionary(Of String, List(Of FeatureItem))
    Private Filters As Dictionary(Of String, String)
    Private Seqs As List(Of Bio.ISequence)
    Dim QuerySeqs As List(Of Bio.ISequence)
    Dim HitSeqs As List(Of Bio.ISequence)

    Dim OpenedFiles As New List(Of FileInfo)

    Private Selected_Query_IDs As New List(Of String)

#Region "BgWork"
    Private Sub CreateBgWork(Type As String, t As Object)
        Dim w = New BackgroundWorker
        w.WorkerReportsProgress = True
        w.WorkerSupportsCancellation = True
        AddHandler w.DoWork, AddressOf WorkerDoWork
        AddHandler w.ProgressChanged, AddressOf WorkerProgressChanged
        AddHandler w.RunWorkerCompleted, AddressOf WorkerCompleted

        w.RunWorkerAsync(t)

    End Sub
    Private Sub WorkerCompleted(sender As Object, e As RunWorkerCompletedEventArgs)
        Try
            Select Case e.Result.Type
                Case Szunyi.Constants.Blast.CreateDatabase
                    Dim alf As Int16 = 54

                Case Szunyi.Constants.Blast.RunBlast
                    Dim kj As Int16 = 54
            End Select
        Catch ex As Exception
            Dim alf As Int16 = 54
        End Try
    End Sub

    Private Sub WorkerProgressChanged(sender As Object, e As ProgressChangedEventArgs)
        'Throw New NotImplementedException()
    End Sub

    Private Sub WorkerDoWork(sender As Object, e As DoWorkEventArgs)

        e.Result = e.Argument
        Try
            e.Argument.DoIt
        Catch ex As Exception
            Dim alf As Int16 = 43

        End Try


    End Sub
#End Region

#Region "Create Databases"
    Private Sub NucleotideToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles NucleotideToolStripMenuItem.Click
        MoveFastaFile(True)
    End Sub

    Private Sub ProteinToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles ProteinToolStripMenuItem.Click
        MoveFastaFile(False)
    End Sub

    'Databases
    Private Sub NucleotideToolStripMenuItem1_Click(sender As Object, e As EventArgs) Handles NucleotideToolStripMenuItem1.Click
        Dim Files = Szunyi.IO.Files.Filter.SelectFiles("Select Database Files", Szunyi.Constants.Files.Fasta)
        For Each File In Files
            MoveFastaFile(True, File)
        Next
    End Sub
    'Databases
    Private Sub ProteinToolStripMenuItem1_Click(sender As Object, e As EventArgs) Handles ProteinToolStripMenuItem1.Click
        Dim Files = Szunyi.IO.Files.Filter.SelectFiles("Select Database Files", Szunyi.Constants.Files.Fasta)
        For Each File In Files
            MoveFastaFile(False, File)
        Next
    End Sub
    Private Sub MoveFastaFile(IsDNA As Boolean, Optional File As FileInfo = Nothing)
        If IsNothing(File) = True Then
            File = Szunyi.IO.Files.Filter.SelectFile(Szunyi.Constants.Files.Fasta, "Select Database file")
            If IsNothing(File) = True Then Exit Sub
        End If
        Dim NewFile As FileInfo = Szunyi.IO.Files.Move_Copy.MoveOrRenameBlastFastaFile(File)

        CreateDatabase(NewFile, IsDNA)
    End Sub


    Private Sub CreateDatabase(File As FileInfo, IsDna As Boolean)
        Dim t As New Szunyi.Blast.Console.CreateDatabase(File, IsDna)
        Me.CreateBgWork(Szunyi.Constants.Blast.CreateDatabase, t)


    End Sub


#End Region

#Region "Load"
    Private Sub Form1_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        CheckDirectories()
    End Sub

    Private Sub CheckDirectories()
        Dim ListOfDirs As New List(Of String)
        ListOfDirs.Add("C:\Blastviewer")
        ListOfDirs.Add("C:\Blastviewer\Blat")
        ListOfDirs.Add("C:\Blastviewer\BlatDBPath")
        ListOfDirs.Add("C:\Blastviewer\BlatResultPath")
        ListOfDirs.Add("C:\Blastviewer\Db")
        ListOfDirs.Add("C:\Blastviewer\FastaFiles")
        ListOfDirs.Add("C:\Blastviewer\Results")

        For Each s In ListOfDirs
            Dim Dir As New DirectoryInfo(s)
            If Dir.Exists = False Then Dir.Create()
        Next

    End Sub

#End Region

#Region "Blast"

    Private Function GetDbFiles(IsDNA As Boolean) As List(Of FileInfo)
        Dim res As New List(Of FileInfo)
        Dim files = Szunyi.Blast.Helper.Paths.GetDBFiles

        If IsDNA = True Then
            Dim t = From x In files Where x.Extension = ".nhr"

            If t.Count > 0 Then Return t.ToList

            Return New List(Of FileInfo)
        Else
            Dim t = From x In files Where x.Extension = ".phr"

            If t.Count > 0 Then Return t.ToList

            Return New List(Of FileInfo)
        End If
        Return res
    End Function
    Private Sub BlatQnucDBnucToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles BlatQnucDBnucToolStripMenuItem.Click
        Dim Files = Szunyi.IO.FilesAndFolder.GetFiles.AllFilesFromFolder(My.Resources.BlatDBPath)

        If Files.Count = 0 Then Exit Sub

        Dim f1 As New CheckBoxForStringsFull(Files, -1)
        f1.Text = "Select DB Files"
        If f1.ShowDialog = DialogResult.Cancel Then Exit Sub

        Dim f2 As New CheckBoxForStringsFull(Files, -1)
        f2.Text = "Select Query Files"
        If f2.ShowDialog = DialogResult.Cancel Then Exit Sub

        Szunyi.Blat.ConsoleBlat.DoBlat(f2.SelectedFiles, f1.SelectedFiles)

    End Sub

    ''' <summary>
    ''' At first Select Query File based on program selection
    ''' After Select Database File based on program selection
    ''' </summary>
    ''' <param name="sender"></param>
    ''' <param name="e"></param>
    ''' <remarks></remarks>
    Private Sub BlastNToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles BlastNToolStripMenuItem.Click
        Dim Files = GetDbFiles(True)
        If Files.Count = 0 Then Exit Sub

        Dim f1 As New CheckBoxForStringsFull(Files, -1)
        If f1.ShowDialog = DialogResult.Cancel Then Exit Sub

        StartApps(f1.SelectedFiles, "blastn")


    End Sub

    Private Sub BlastPToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles BlastPToolStripMenuItem.Click
        Dim Files = GetDbFiles(False)
        If Files.Count = 0 Then Exit Sub

        Dim f1 As New CheckBoxForStringsFull(Files, -1)
        If f1.ShowDialog = DialogResult.Cancel Then Exit Sub
        StartApps(f1.SelectedFiles, "blastp")


    End Sub

    Private Sub BlastXToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles BlastXToolStripMenuItem.Click
        Dim Files = GetDbFiles(False)
        If Files.Count = 0 Then Exit Sub

        Dim f1 As New CheckBoxForStringsFull(Files, -1)
        If f1.ShowDialog = DialogResult.Cancel Then Exit Sub

        StartApps(f1.SelectedFiles, "blastx")


    End Sub

    Private Sub TBlastNToolStripMenuItem1_Click(sender As Object, e As EventArgs) Handles TBlastNToolStripMenuItem1.Click
        Dim Files = GetDbFiles(True)
        If Files.Count = 0 Then Exit Sub

        Dim f1 As New CheckBoxForStringsFull(Files, -1)
        If f1.ShowDialog = DialogResult.Cancel Then Exit Sub

        StartApps(f1.SelectedFiles, "tblastn")


    End Sub

    Private Sub TBlastXToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles TBlastXToolStripMenuItem.Click
        Dim Files = GetDbFiles(True)
        If Files.Count = 0 Then Exit Sub

        Dim f1 As New CheckBoxForStringsFull(Files, -1)
        If f1.ShowDialog = DialogResult.Cancel Then Exit Sub

        StartApps(f1.SelectedFiles, "tblastx")


    End Sub

    Private Sub StartApps(DbFiles As List(Of FileInfo), SelectedProgram As String)
        Dim QueryFiles = Szunyi.IO.Files.Filter.SelectFiles(Szunyi.Constants.Files.Fasta, "Select Query Files")

        If IsNothing(DbFiles) = True Or IsNothing(QueryFiles) = True Then Exit Sub
        If DbFiles.Count = 0 AndAlso QueryFiles.Count = 0 Then Exit Sub

        Dim tmp = Szunyi.IO.Files.Move_Copy.MoveOrRenameBlastFastaFiles(QueryFiles)

        Dim Blast_Output_Types = Szunyi.Util_Helpers.Get_All_Enum_Names(Of outfmt)(1)
        Dim f1 As New CheckBoxForStringsFull(Blast_Output_Types, 1)
        If f1.ShowDialog = DialogResult.OK Then
            Dim fmt = Szunyi.Util_Helpers.Get_Enum_Value(Of outfmt)(f1.SelectedStrings.First)
            Dim x As New Szunyi.Blast.Console.DoBlast(tmp, DbFiles, SelectedProgram, fmt)

            Me.CreateBgWork(Szunyi.Constants.Blast.RunBlast, x)
        End If


    End Sub

#End Region

#Region "View Results"

    Private Sub ResultsToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles ResultsToolStripMenuItem.Click

        Dim Files = Szunyi.Blast.Helper.Paths.GetResultFiles
        Dim f1 As New CheckBoxForStringsFull(Files, -1)
        If f1.ShowDialog = DialogResult.Cancel Then Exit Sub
        If f1.SelectedFiles.Count = 0 Then Exit Sub
        If Files.Count > 0 Then
            OpenResults(f1.SelectedFiles)
            Me.OpenedFiles = f1.SelectedFiles
        End If

        Me.QuerySeqs = Nothing
        Me.HitSeqs = Nothing
    End Sub
    Private Sub OpenResults(Files As List(Of FileInfo))

        Dim log As New System.Text.StringBuilder
        Me.OriginalBlastSearchResults = Szunyi.IO.Import.BLAST.FromFileInfos(Files, log)

        Me.OriginalBlastSearchRecords = Szunyi.Blast.BlastManipulation.GetRecords(OriginalBlastSearchResults)


        ClonedAndFilteredBlastSearchRecords = Szunyi.Blast.BlastManipulation.Clone(OriginalBlastSearchRecords)
        '   ClonedAndFilteredBlastSearchRecords = Filter.NotShowSelfHits(ClonedAndFilteredBlastSearchRecords)

        ListBox1.DataSource = ClonedAndFilteredBlastSearchRecords
        ListBox1.DisplayMember = "IterationQueryDefinition"

        Me.Text = Files.First.Name & " e:" & Me.ClonedAndFilteredBlastSearchRecords.Count & "/" & Me.OriginalBlastSearchRecords.Count

    End Sub

#End Region

#Region "Set Paths"
    Private Sub SetBlastPathToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles SetBlastPathToolStripMenuItem.Click

        '   SetBlastPath(True)

    End Sub

#End Region

#Region "Visualisation"

    Private Sub ListBox1_BindingContextChanged(sender As Object, e As EventArgs) Handles ListBox1.BindingContextChanged
        Dim alf As Int16 = 54
    End Sub

    Private Sub ListBox1_MouseDown(sender As Object, e As MouseEventArgs) Handles ListBox1.MouseDown
        If e.Button = MouseButtons.Right Then
            Dim r = MsgBox("Do you want to Delete", MsgBoxStyle.OkCancel)
            If r = MsgBoxResult.Ok Then
                Me.ClonedAndFilteredBlastSearchRecords.Remove(ListBox1.SelectedItem)

            End If
        End If
    End Sub


    Private Sub ListBox1_SelectedIndexChanged(sender As Object, e As EventArgs) Handles ListBox1.SelectedIndexChanged
        If ListBox1.SelectedIndex > -1 Then
            Dim cItem As Bio.Web.Blast.BlastSearchRecord = ListBox1.Items(ListBox1.SelectedIndex)
            Dim alf As Integer = 54
            lbhit.DataSource = cItem.Hits
            lbhit.DisplayMember = "Def"
            ShowAllHsps(cItem)
        End If
    End Sub
    Private Sub LB1_Double_Click(sender As Object, e As EventArgs) Handles ListBox1.MouseDoubleClick
        Dim Index = ListBox1.SelectedIndex
        If Index > -1 Then
            Dim s As Bio.Web.Blast.BlastSearchRecord = ListBox1.Items(Index)
            Selected_Querys.Items.Add(s)
            Selected_Querys.DisplayMember = "IterationQueryDefinition"
        End If

    End Sub
    Private Sub ShowAllHsps(cItem As BlastSearchRecord)
        Dim HSPs As New List(Of ExtenededHsp)
        For Each Hit In cItem.Hits
            For Each Hsp In Hit.Hsps
                HSPs.Add(New ExtenededHsp(Hsp, cItem, Hit))
            Next
        Next
        dgv1.DataSource = HSPs
        dgv1.Columns("HitFrame").Visible = True
        dgv1.Columns("HSP").Visible = False
        dgv1.Columns("QuerySequence").Visible = False
        dgv1.Columns("Midline").Visible = False
        dgv1.Columns("HitSequence").Visible = False
        dgv1.Columns("PatternFrom").Visible = False
        dgv1.Columns("PatternTo").Visible = False
    End Sub

    Private Sub ListBox2_SelectedIndexChanged(sender As Object, e As EventArgs) Handles lbhit.SelectedIndexChanged
        If lbhit.SelectedIndex > -1 Then
            Dim x As New List(Of ExtenededHsp)
            Dim cHit As Bio.Web.Blast.Hit = lbhit.Items(lbhit.SelectedIndex)
            If ListBox1.SelectedIndex < 0 Then Exit Sub
            Dim cBSR As Bio.Web.Blast.BlastSearchRecord = ListBox1.Items(ListBox1.SelectedIndex)
            For Each Hsp In cHit.Hsps
                x.Add(New ExtenededHsp(Hsp, cBSR, cHit))
            Next

            dgv1.DataSource = x

            dgv1.Columns("HSP").Visible = False
            dgv1.Columns("QuerySequence").Visible = False
            dgv1.Columns("Midline").Visible = False
            dgv1.Columns("HitSequence").Visible = False
            dgv1.Columns("PatternFrom").Visible = False
            dgv1.Columns("PatternTo").Visible = False
        End If
    End Sub
#Region "Dgv and PictureBox"
    Private Sub dgv1_ColumnHeaderMouseClick(sender As Object, e As DataGridViewCellMouseEventArgs) Handles dgv1.ColumnHeaderMouseClick

        Select Case SortOrders(e.ColumnIndex)
            Case SortOrder.None
                SortOrders(e.ColumnIndex) = SortOrder.Ascending
            Case SortOrder.Ascending
                SortOrders(e.ColumnIndex) = SortOrder.Descending
            Case SortOrder.Descending
                SortOrders(e.ColumnIndex) = SortOrder.Ascending
        End Select
        Try
            OrderHSPs(SortOrders(e.ColumnIndex), dgv1.Columns(e.ColumnIndex).Name)

        Catch ex As Exception
            MsgBox(ex.ToString)
        End Try


    End Sub
    Private Sub OrderHSPs(SO As Integer, ColumnName As String)
        Dim HSPS As List(Of ExtenededHsp) = dgv1.DataSource
        Dim res As New List(Of ExtenededHsp)
        Select Case ColumnName
            Case "Hit_Definition"
                If SO = SortOrder.Ascending Then
                    res = (From t In HSPS Order By t.Hit_Definition Ascending).ToList
                Else
                    res = (From t In HSPS Order By t.Hit_Definition Descending).ToList
                End If
            Case "Hit_ID"
                If SO = SortOrder.Ascending Then
                    res = (From t In HSPS Order By t.Hit_ID Ascending).ToList
                Else
                    res = (From t In HSPS Order By t.Hit_ID Descending).ToList
                End If
            Case "hitLength"
                If SO = SortOrder.Ascending Then
                    res = (From t In HSPS Order By t.hitLength Ascending).ToList
                Else
                    res = (From t In HSPS Order By t.hitLength Descending).ToList
                End If
            Case "Score"
                If SO = SortOrder.Ascending Then
                    res = (From t In HSPS Order By t.Score Ascending).ToList
                Else
                    res = (From t In HSPS Order By t.Score Descending).ToList
                End If
            Case "BitScore"
                If SO = SortOrder.Ascending Then
                    res = (From t In HSPS Order By t.BitScore Ascending).ToList
                Else
                    res = (From t In HSPS Order By t.BitScore Descending).ToList
                End If
            Case "HitStart"
                If SO = SortOrder.Ascending Then
                    res = (From t In HSPS Order By t.HitStart Ascending).ToList
                Else
                    res = (From t In HSPS Order By t.HitStart Descending).ToList
                End If
            Case "HitEnd"
                If SO = SortOrder.Ascending Then
                    res = (From t In HSPS Order By t.HitEnd Ascending).ToList
                Else
                    res = (From t In HSPS Order By t.HitEnd Descending).ToList
                End If
            Case "QueryStart"
                If SO = SortOrder.Ascending Then
                    res = (From t In HSPS Order By t.QueryStart Ascending).ToList
                Else
                    res = (From t In HSPS Order By t.QueryStart Descending).ToList
                End If
            Case "QueryEnd"
                If SO = SortOrder.Ascending Then
                    res = (From t In HSPS Order By t.QueryEnd Ascending).ToList
                Else
                    res = (From t In HSPS Order By t.QueryEnd Descending).ToList
                End If
            Case "Query_Length"
                If SO = SortOrder.Ascending Then
                    res = (From t In HSPS Order By t.Query_Length Ascending).ToList
                Else
                    res = (From t In HSPS Order By t.Query_Length Descending).ToList
                End If
            Case "AlignmentLength"
                If SO = SortOrder.Ascending Then
                    res = (From t In HSPS Order By t.AlignmentLength Ascending).ToList
                Else
                    res = (From t In HSPS Order By t.AlignmentLength Descending).ToList
                End If
            Case "IdentitiesCount"
                If SO = SortOrder.Ascending Then
                    res = (From t In HSPS Order By t.IdentitiesCount Ascending).ToList
                Else
                    res = (From t In HSPS Order By t.IdentitiesCount Descending).ToList
                End If
            Case "EValue"
                If SO = SortOrder.Ascending Then
                    res = (From t In HSPS Order By t.EValue Ascending).ToList
                Else
                    res = (From t In HSPS Order By t.EValue Descending).ToList
                End If
            Case "QueryFrame"
                If SO = SortOrder.Ascending Then
                    res = (From t In HSPS Order By t.QueryFrame Ascending).ToList
                Else
                    res = (From t In HSPS Order By t.QueryFrame Descending).ToList
                End If
            Case "HitFrame"
                If SO = SortOrder.Ascending Then
                    res = (From t In HSPS Order By t.HitFrame Ascending).ToList
                Else
                    res = (From t In HSPS Order By t.HitFrame Descending).ToList
                End If
            Case "Gaps"
                If SO = SortOrder.Ascending Then
                    res = (From t In HSPS Order By t.Gaps Ascending).ToList
                Else
                    res = (From t In HSPS Order By t.Gaps Descending).ToList
                End If
            Case "Density"
                If SO = SortOrder.Ascending Then
                    res = (From t In HSPS Order By t.Density Ascending).ToList
                Else
                    res = (From t In HSPS Order By t.Density Descending).ToList
                End If
        End Select
        If res.Count <> 0 Then dgv1.DataSource = res
    End Sub

    Private Sub dgv1_RowEnter(sender As Object, e As DataGridViewCellEventArgs) Handles dgv1.RowEnter
        Dim alf As Integer = 43
        Dim x As ExtenededHsp = dgv1.Rows(e.RowIndex).DataBoundItem
        Dim br As SolidBrush
        Select Case x.Score
            Case Is < 40
                br = New SolidBrush(Color.Black)
            Case Is < 50
                br = New SolidBrush(Color.Blue)
            Case Is < 80
                br = New SolidBrush(Color.Green)
            Case Is < 200
                br = New SolidBrush(Color.Pink)
            Case Else
                br = New SolidBrush(Color.Red)
        End Select
        Dim bmp As New Bitmap(pb1.Width, pb1.Height)
        Dim graphics As Graphics = Graphics.FromImage(bmp)

        Dim Range = x.Query_Length \ pb1.Width
        If Range = 0 Then
            Range = pb1.Width \ x.Query_Length
            Dim x1 As Integer = (x.QueryStart * Range)
            Dim x2 As Integer = (x.QueryEnd * Range)
            graphics.DrawLine(New Pen(br, 5), x1, 5, x2, 5)
        Else
            Dim x1 As Integer = (x.QueryStart / Range)
            Dim x2 As Integer = (x.QueryEnd / Range)
            graphics.DrawLine(New Pen(br, 5), x1, 5, x2, 5)
        End If


        Range = x.hitLength \ pb1.Width
        If Range = 0 Then
            If x.hitLength > 0 Then
                Range = pb1.Width \ x.hitLength
                Dim x1 As Integer = (x.HitStart * Range)
                Dim x2 As Integer = (x.HitEnd * Range)
                graphics.DrawLine(New Pen(br, 5), x1, 15, x2, 15)
            End If

        Else
            Dim x1 As Integer = (x.HitStart / Range)
            Dim x2 As Integer = (x.HitEnd / Range)
            graphics.DrawLine(New Pen(br, 5), x1, 15, x2, 15)
        End If


        Dim myFont As Font = tbAlignment.Font

        Dim Fs = graphics.MeasureString("a", myFont)
        DrawTextBox(x, Fs.Width)
        pb1.Image = bmp
        graphics.Dispose()

    End Sub

    Private Sub DrawTextBox(hsp As ExtenededHsp, FontWidth As Double)
        Dim NofCharPerLine As Integer = (Me.tbAlignment.Width) \ FontWidth

        Dim myFont As Font = tbAlignment.Font
        Dim graphics As Graphics = tbAlignment.CreateGraphics
        Dim s As New StringBuilder
        Do
            s.Append("a")
            Dim Fs = graphics.MeasureString(s.ToString, myFont)
            If Fs.Width > tbAlignment.Width - 10 Then
                NofCharPerLine = s.Length - 2
                Exit Do
            End If
        Loop

        Dim str As New StringBuilder
        If IsNothing(hsp.HSP.QuerySequence) = True Then Exit Sub
        For i1 = 0 To hsp.HSP.QuerySequence.Length Step NofCharPerLine
            If i1 + NofCharPerLine > hsp.HSP.QuerySequence.Length Then NofCharPerLine = hsp.HSP.QuerySequence.Length - i1
            str.Append(hsp.HSP.QuerySequence.Substring(i1, NofCharPerLine)).AppendLine()
            If IsNothing(hsp.HSP.Midline) = False Then str.Append(hsp.HSP.Midline.Substring(i1, NofCharPerLine)).AppendLine()
            str.Append(hsp.HSP.HitSequence.Substring(i1, NofCharPerLine)).AppendLine().AppendLine()
        Next
        Me.tbAlignment.Text = str.ToString
    End Sub

#End Region

    Private Sub tb1_TextChanged(sender As Object, e As EventArgs) Handles tb1.TextChanged
        If tb1.Text.Length > 2 Then
            ListBox1.DataSource = GetFilteredBlastSearchRecords(tb1.Text)
            ListBox1.DisplayMember = "IterationQueryDefinition"
        Else
            ListBox1.DataSource = ClonedAndFilteredBlastSearchRecords
            ListBox1.DisplayMember = "IterationQueryDefinition"
        End If
    End Sub



#End Region

#Region "Filter"
    ''' <summary>
    ''' Update Datasource And Set Propoerties
    ''' </summary>
    Private Sub RefreshListbox1()
        Me.ListBox1.DataSource = Me.ClonedAndFilteredBlastSearchRecords
        Me.ListBox1.DisplayMember = "Def"
        Me.Text = Me.OpenedFiles.First.Name & " " & Me.ClonedAndFilteredBlastSearchRecords.Count & "/" & Me.OriginalBlastSearchRecords.Count
    End Sub
    Private Sub Identity100QueryLengthHitLengthToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles Identity100QueryLengthHitLengthToolStripMenuItem.Click
        Me.ClonedAndFilteredBlastSearchRecords = BlastFilter.GetRecordswoPerfectHits_Identity_Query_Hit(Me.ClonedAndFilteredBlastSearchRecords)
        RefreshListbox1()
    End Sub

    Private Sub Identity100QueryLengthToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles Identity100QueryLengthToolStripMenuItem.Click
        Me.ClonedAndFilteredBlastSearchRecords = BlastFilter.GetRecordswoPerfectHits_Identity_Query(Me.ClonedAndFilteredBlastSearchRecords)
        RefreshListbox1()
    End Sub

    Private Sub Identity100HitLengthToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles Identity100HitLengthToolStripMenuItem.Click
        Me.ClonedAndFilteredBlastSearchRecords = BlastFilter.GetRecordswoPerfectHits_Identity_Hit(Me.ClonedAndFilteredBlastSearchRecords)
        RefreshListbox1()
    End Sub
    Private Sub SetIdentityToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles SetIdentityToolStripMenuItem.Click
        Try
            Me.minIdentityPercent = InputBox("Set minimal Identity Percent!")
            Me.minIdentityPercent = Me.minIdentityPercent / 100
            Me.ClonedAndFilteredBlastSearchRecords = GetFilteredBlastSearchRecordsbyIdentityPercent()
            Me.ListBox1.DataSource = Me.ClonedAndFilteredBlastSearchRecords
            Me.ListBox1.DisplayMember = "Def"
        Catch ex As Exception
            MsgBox("It Is Not a valid number")
        End Try

    End Sub
    Private Sub SetEvalueToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles SetEvalueToolStripMenuItem.Click
        Dim s1 As Double = Szunyi.MyInputBox.GetDouble("Set evalue")
        Try

            Me.ClonedAndFilteredBlastSearchRecords = BlastFilter.GetFilteredBlastSearchRecordsbyEvalue(Me.ClonedAndFilteredBlastSearchRecords, s1)
            RefreshListbox1()
        Catch ex As Exception
            MsgBox("It Is Not a valid number")
        End Try
    End Sub


    Private Function GetFilteredBlastSearchRecords(Filter As String) As List(Of Bio.Web.Blast.BlastSearchRecord)
        Dim res = From x In Me.ClonedAndFilteredBlastSearchRecords Where x.IterationQueryDefinition.ToUpper.Contains(Filter.ToUpper)

        If res.Count > 0 Then
            Return res.ToList
        Else
            Return New List(Of Bio.Web.Blast.BlastSearchRecord)

        End If
    End Function


    Private Function GetFilteredBlastSearchRecordsbyIdentityPercent() As List(Of Bio.Web.Blast.BlastSearchRecord)
        Dim res As New List(Of BlastSearchRecord)
        For Each Item In BlastFilter.GetRecordswithHits(ClonedAndFilteredBlastSearchRecords)

            res.Add(Item)
            For i1 = res.Last.Hits.Count - 1 To 0 Step -1
                If Item.IterationQueryDefinition = res.Last.Hits(i1).Def Then
                    res.Last.Hits.RemoveAt(i1)
                Else
                    For i2 = res.Last.Hits(i1).Hsps.Count - 1 To 0 Step -1
                        If res.Last.Hits(i1).Hsps(i2).IdentitiesCount * (1 / Me.minIdentityPercent) < Item.IterationQueryLength Then
                            res.Last.Hits(i1).Hsps.RemoveAt(i2)
                        Else
                            Dim alf As Int16 = 54

                        End If

                    Next
                    If res.Last.Hits(i1).Hsps.Count = 0 Then res.Last.Hits.RemoveAt(i1)
                End If


            Next
            If res.Last.Hits.Count = 0 Then res.Remove(res.Last)
        Next


        If res.Count > 0 Then Return res.ToList
        Return New List(Of Bio.Web.Blast.BlastSearchRecord)

    End Function

    Private Function GetFilteredBlastSearchRecordsbyEvalueReverse() As List(Of Bio.Web.Blast.BlastSearchRecord)
        Dim Fx = BlastFilter.GetRecordswithHits(ClonedAndFilteredBlastSearchRecords)
        Dim res = From x In Fx Where x.Hits.First.Hsps.First.IdentitiesCount / x.Hits.First.Hsps.First.AlignmentLength >= Me.minIdentityPercent

        If res.Count > 0 Then Return res.ToList
        Return New List(Of Bio.Web.Blast.BlastSearchRecord)

    End Function


    Private Sub SetMinimalLengthByQueryPercentToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles SetMinimalLengthByQueryPercentToolStripMenuItem.Click
        Try
            Me.MinimalPercentOfQuery = InputBox("Set minimal percent Of  Match!")
            Me.ClonedAndFilteredBlastSearchRecords = Szunyi.Blast.BlastFilter.GetRecordsWithMinimalPercentofAlignemnt(Me.ClonedAndFilteredBlastSearchRecords)
            RefreshListbox1()
        Catch ex As Exception
            MsgBox("It Is Not a valid number")
        End Try
    End Sub

#End Region


#Region "Filtering"
    Private Sub SameLengthsToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles SameLengthsToolStripMenuItem.Click
        Dim diffPercent As Double = Szunyi.MyInputBox.GetDouble("Max difference in percents")
        Me.ClonedAndFilteredBlastSearchRecords = BlastFilter.Get_Records_Same_Lengths(
          Me.ClonedAndFilteredBlastSearchRecords, diffPercent)
        RefreshListbox1()
    End Sub

    Private Sub SetMinimalLengthToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles SetMinimalLengthToolStripMenuItem.Click

        Me.ClonedAndFilteredBlastSearchRecords = BlastFilter.SelectRecorsWhereAligmentLengthIsHigherThen(
            Me.ClonedAndFilteredBlastSearchRecords)
        RefreshListbox1()

    End Sub
    Private Sub SetToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles SetToolStripMenuItem.Click
        Me.ClonedAndFilteredBlastSearchRecords = BlastFilter.GetRecordswoHits(Me.ClonedAndFilteredBlastSearchRecords)
        RefreshListbox1()
    End Sub
    Private Sub ShowHitsToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles ShowHitsToolStripMenuItem.Click
        Me.ClonedAndFilteredBlastSearchRecords = BlastFilter.GetRecordswithHits(Me.ClonedAndFilteredBlastSearchRecords)
        RefreshListbox1()
    End Sub

    Private Sub SetMinimalPercentOfAlignmentToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles SetMinimalPercentOfAlignmentToolStripMenuItem.Click
        Me.ClonedAndFilteredBlastSearchRecords = BlastFilter.GetRecordsWithMinimalPercentofAlignemnt(Me.ClonedAndFilteredBlastSearchRecords)
        RefreshListbox1()
    End Sub


#End Region

#Region "Pool"
    Private Sub GetPoolNAmesToolStripMenuItem_Click(sender As Object, e As EventArgs)
        Dim FIles = Szunyi.IO.Files.Filter.SelectFiles

        Using sr As New StreamReader(FIles.First.FullName)
            Do
                Dim line = Split(sr.ReadLine, vbTab)
                Me.Pool.Add(line)
            Loop Until sr.EndOfStream = True
        End Using
        Dim out As New System.Text.StringBuilder
        For Each Result In Me.ClonedAndFilteredBlastSearchRecords

            If Result.Hits.Count = 2 Then
                out.Append(Result.IterationQueryDefinition).AppendLine()
                out.Append(GetNameFromPoolTable(Result.Hits.First.Def, Result.Hits.Last.Def)).AppendLine.AppendLine()
            ElseIf Result.Hits.Count = 1 Then
                out.Append(Result.IterationQueryDefinition).AppendLine()
                out.Append(Result.Hits.First.Def).AppendLine.AppendLine()

            ElseIf Result.Hits.Count > 2 Then 'Complex
                Dim Rows = GetRowsOrColumns(Result.Hits, True)
                Dim Cols = GetRowsOrColumns(Result.Hits, False)
                out.Append(Result.IterationQueryDefinition).AppendLine()
                For Each Row In Rows
                    For Each COl In Cols
                        out.Append(GetNameFromPoolTable(Row.Def, COl.Def)).AppendLine()
                    Next
                Next
                out.AppendLine.AppendLine()
            End If
        Next
    End Sub
    Private Function GetRowsOrColumns(Hits As List(Of Hit), ISrow As Boolean)
        Dim out As New List(Of Hit)
        For Each Hit In Hits
            If ISrow = True Then
                If Hit.Def.Contains(".R") Then
                    out.Add(Hit)
                End If
            Else
                If Hit.Def.Contains(".C") Then
                    out.Add(Hit)
                End If
            End If

        Next
        Return out
    End Function
    Private Function GetShortContigName(x As String) As String
        Dim s = x.Replace("_(Single)_trimmed_contig_", "c")
        Return Split(s, " ").First
    End Function
    Private Function GetPoolName(x As String) As String


        Return Split(x, "_").First
    End Function
    Private Function GetNameFromPoolTable(p1 As String, p2 As String) As String
        Dim t = GetPoolName(p1)
        Dim r As Integer
        Dim c As Integer
        If t.Last = "R" Then
            r = GetFirstNumberFromString(t)
        ElseIf t.Last = "C" Then
            c = GetFirstNumberFromString(t)
        Else
            Dim alf As Int16 = 43
        End If
        t = GetPoolName(p2)
        If t.Last = "R" Then
            r = GetFirstNumberFromString(t)
        ElseIf t.Last = "C" Then
            c = GetFirstNumberFromString(t)
        Else
            Dim alf As Int16 = 43
        End If
        Return Me.Pool(r)(c)
    End Function
    Private Function GetFirstNumberFromString(x As String) As Integer
        Dim ch = x.ToCharArray
        For i1 = 0 To ch.Count - 1
            If IsNumeric(ch(i1)) = True Then
                Dim out As String = ch(i1)
                For i2 = i1 + 1 To ch.Count - 1
                    If IsNumeric(ch(i2)) Then
                        out = out & ch(i2)
                    Else
                        Dim i As Integer = out
                        Return i
                    End If
                Next
            End If
        Next
        Return Nothing
    End Function

#End Region



    Private Function GetLOcationString(t As List(Of ReturnPointForBlast)) As String
        Dim str As New System.Text.StringBuilder
        If t.First.IsReverse = True Then str.Append("complement(")
        If t.Count > 1 Then str.Append("join(")
        Dim tf = From x In t Order By x.MdHitStart Ascending


        For Each item In tf
            If item.MdHitStart < item.MdHitEnd Then
                str.Append(item.MdHitStart).Append("..").Append(item.MdHitEnd).Append(",")
            Else
                str.Append(item.MdHitEnd).Append("..").Append(item.MdHitStart).Append(",")
            End If

        Next
        str.Length -= 1
        If t.Count > 1 Then str.Append(")")
        If t.First.IsReverse = True Then str.Append(")")
        Return str.ToString

    End Function


    Private Sub ShowUniqueQueriesToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles ShowUniqueQueriesToolStripMenuItem.Click
        Dim Used As New HashSet(Of String)
        Dim NewBlastSearchRecords As New Dictionary(Of String, BlastSearchRecord)
        For Each Record In Me.ClonedAndFilteredBlastSearchRecords
            If Used.Contains(Record.IterationQueryDefinition) = True Then
                NewBlastSearchRecords.Remove(Record.IterationQueryDefinition)

            Else
                Used.Add(Record.IterationQueryDefinition)
                NewBlastSearchRecords.Add(Record.IterationQueryDefinition, Record)
            End If
        Next
        Me.ClonedAndFilteredBlastSearchRecords = NewBlastSearchRecords.Values.ToList
        ListBox1.DataSource = Me.ClonedAndFilteredBlastSearchRecords

        ListBox1.DisplayMember = "IterationQueryDefinition"
    End Sub




#Region "Extra GENOMIC iNDEL ETC"
#Region "Find From Genomic"

    Private Sub FindFullLengthGenomicClonesToolStripMenuItem_Click(sender As Object, e As EventArgs)
        Dim Unknown As New System.Text.StringBuilder
        Dim Problematic As New System.Text.StringBuilder
        Dim Features As New Dictionary(Of String, List(Of FeatureItem))
        If IsNothing(Features) = True Then
            Seqs = Szunyi.IO.Import.Sequence.FromUserSelection()
            '    Features = Szunyi.Features.FeatureManipulation.GetFeaturesByType.GetFeturesByTypeFromSeqs(Seqs, StandardFeatureKeys.CodingSequence)

        End If

        Dim OriginalQueryFile = GetOriginalQueryFile(Me.Text)
        Dim OriginalSeqs As New List(Of Bio.ISequence)
        If OriginalQueryFile.Exists Then OriginalSeqs = Szunyi.IO.Import.Sequence.FromFile(OriginalQueryFile)
        Dim NOfs As New Dictionary(Of Integer, List(Of BlastSearchRecord))
        Dim res As New List(Of BlastSearchRecord)
        Dim NofAdded As Integer = 0
        Dim log As New StringBuilder
        Dim NofInserted As Integer = 0
        Dim tmpSeq As New Bio.Sequence(Alphabets.AmbiguousDNA, "A")
        For Each Item In Me.ClonedAndFilteredBlastSearchRecords
            Dim Groups = GetGroups(Item)
            Dim SubGroups = GetSubGroups(Groups, Item, 2500)
            If NOfs.ContainsKey(SubGroups.Count) = False Then NOfs.Add(SubGroups.Count, New List(Of BlastSearchRecord))
            NOfs(SubGroups.Count).Add(Item)
            Dim HasFound As Boolean = False
            For Each PotentialGene In SubGroups

                tmpSeq.ID = Split(SubGroups.First.Key, vbTab).First
                Dim Index = Seqs.BinarySearch(tmpSeq, Szunyi.Comparares.AllComparares.BySeqID)
                If Index > -1 Then
                    Dim GB As Bio.IO.GenBank.GenBankMetadata = Seqs(Index).Metadata(Bio.Util.Helper.GenBankMetadataKey)

                    Dim Locis = GetLocation(SubGroups.First.Value, Item, New Bio.Web.Blast.Hit) 'Gene, mRNA,CDS

                    Dim NextLocusTag As Integer = 1

                    If Features.ContainsKey(Item.IterationQueryDefinition) = False Then
                        ' New Gene
                        Dim TheGene = GetGene(Locis.First, Item.IterationQueryDefinition, 1, "Newly Inserted")
                        GB.Features.All.Add(TheGene)
                        Features.Add(Item.IterationQueryDefinition, New List(Of FeatureItem))
                        Features(Item.IterationQueryDefinition).Add(GB.Features.All.Last)
                    Else
                        NextLocusTag = Features(Item.IterationQueryDefinition).Count \ 2 + 1 ' The Next LocusTags
                    End If

                    Dim mRNAs = GetmRNA(Locis(1), Item.IterationQueryDefinition, NextLocusTag, "")

                    Dim TheCDS = GetCDS(Locis.Last, Item.IterationQueryDefinition, NextLocusTag, "")

                    If TestLocactionAndTranslation(TheCDS, Seqs(Index),
                                                   OriginalSeqs(Item.IterationQueryDefinition),
                                                   Locis.Last, log) = True Then


                        Dim Insertionindex = GB.Features.All.IndexOf(Features(Item.IterationQueryDefinition).Last)

                        GB.Features.All.Insert(Insertionindex + 1, mRNAs)
                        GB.Features.All.Insert(Insertionindex + 2, TheCDS)

                        HasFound = True
                        NofInserted += 1

                    Else
                        log.Append("Bad Translation" & vbTab & Item.IterationQueryDefinition & vbCrLf)
                    End If
                Else
                    log.Append("Not fOUND" & tmpSeq.ID & vbCrLf)
                End If

            Next ' Next Potetial Gene
            If HasFound = False Then log.Append(Item.IterationQueryDefinition).AppendLine()
        Next ' Next BlastSearchRecord



        Szunyi.IO.Export.SaveSequencesToSingleGenBank(Seqs.ToList)


        ListBox1.DataSource = res
        ListBox1.DisplayMember = "IterationQueryDefinition"
    End Sub
    Private Function TestLocactionAndTranslation(TheCDS As CodingSequence,
                                                 GenomeicDNA As Bio.Sequence,
                                                 OriginalCDSSeq As Bio.Sequence,
                                                 Loci As Bio.IO.GenBank.Location,
                                                 ByRef log As StringBuilder) As Boolean
        Dim f = LocBuilder.GetLocationString(Loci)
        Dim LengthOFLog As Integer = log.Length
        If f.Contains("0..0") Then
            log.Append(OriginalCDSSeq.ID).Append(vbTab)
            log.Append("0..0").Append(vbTab)
            log.Append(f).AppendLine()
        End If

        Try
            Dim Translation = Szunyi.Translate.TranaslateFromFeature(TheCDS, GenomeicDNA)
            Dim OriTranslation = Szunyi.Translate.TranaslateAsString(OriginalCDSSeq)
            If Translation <> OriTranslation Then
                log.Append(OriginalCDSSeq.ID).Append(vbTab)
                log.Append("Mis translation").Append(vbTab)
                log.Append(f).AppendLine()
            End If
        Catch ex As Exception
            log.Append(OriginalCDSSeq.ID).Append(vbTab)
            log.Append("Error In software").Append(vbTab)
            log.Append(f).AppendLine()
        End Try
        If log.Length = LengthOFLog Then Return True ' No error
        Return False
    End Function
    Private Function GetGroups(Item As BlastSearchRecord) As Dictionary(Of String, List(Of Hit))
        Dim Groups As New Dictionary(Of String, List(Of Hit))
        For Each Hit In Item.Hits
            If Groups.ContainsKey(Hit.Def) = False Then Groups.Add(Hit.Def, New List(Of Hit))
            Groups(Hit.Def).Add(Hit)
        Next
        Return Groups
    End Function
    Private Function GetSubGroups(Groups As Dictionary(Of String, List(Of Hit)),
                                Item As BlastSearchRecord, MaxDiffereBetweenExons As Integer) As Dictionary(Of String, List(Of Hsp))


        Dim SubGroups As New Dictionary(Of String, List(Of Hsp))

        For Each Group In Groups
            Dim Hsps = Group.Value.First.Hsps

            Dim OrderedHsps = (From x In Hsps Order By x.HitStart).ToList

            If OrderedHsps.Count = 1 Then
                SubGroups.Add(Group.Key & vbTab & SubGroups.Count + 1, New List(Of Hsp))
                SubGroups.Last.Value.Add(OrderedHsps.First)
            Else
                Dim Finished As Boolean = False
                For i1 = 0 To OrderedHsps.Count - 2
                    If Finished = True Then
                        Exit For
                    End If
                    For i2 = i1 + 1 To OrderedHsps.Count - 1
                        If OrderedHsps(i1).HitStart + MaxDiffereBetweenExons > OrderedHsps(i2).HitStart Then
                            If i1 = 0 And i2 = 1 Then
                                SubGroups.Add(Group.Key & vbTab & SubGroups.Count + 1, New List(Of Hsp))
                                SubGroups.Last.Value.Add(OrderedHsps(i1))
                                SubGroups.Last.Value.Add(OrderedHsps(i2))
                            Else
                                SubGroups.Last.Value.Add(OrderedHsps(i2))
                            End If
                        Else ' ha nagy a difi
                            If i1 = 0 And SubGroups.Count = 0 Then
                                SubGroups.Add(Group.Key & vbTab & SubGroups.Count + 1, New List(Of Hsp))
                                SubGroups.Last.Value.Add(OrderedHsps(i1))
                            End If
                            SubGroups.Add(Group.Key & vbTab & SubGroups.Count + 1, New List(Of Hsp))
                            SubGroups.Last.Value.Add(OrderedHsps(i2))


                            i1 = i2
                        End If
                        If i2 = OrderedHsps.Count - 1 Then
                            Finished = True
                        End If
                    Next
                Next
            End If
        Next
        Dim res As New Dictionary(Of String, List(Of Hsp))
        For Each SubGroup In SubGroups
            Dim IdentityCount As Integer = 0
            For Each Hsp In SubGroup.Value
                IdentityCount += Hsp.IdentitiesCount
            Next
            If IdentityCount >= Item.IterationQueryLength Then
                res.Add(SubGroup.Key, SubGroup.Value)
            Else
                Dim alf As Int16 = 54
            End If

        Next
        Return res
    End Function

    Private Function GetLocation(SelectedHSps As List(Of Hsp),
                                 BRecord As BlastSearchRecord, Hit As Bio.Web.Blast.Hit) As List(Of Bio.IO.GenBank.Location)

        Dim Locis As New List(Of Bio.IO.GenBank.Location)
        Dim r As New List(Of ExtenededHsp)
        For Each item In SelectedHSps
            r.Add(New ExtenededHsp(item, BRecord, Hit))
        Next
        Dim OrderedHsps As List(Of ExtenededHsp) = (From x In r Order By x.QueryStart Ascending).ToList


        Dim t As New List(Of ReturnPointForBlast)
        t.Add(New ReturnPointForBlast(OrderedHsps.First, BRecord.IterationQueryDefinition))
        For i1 = 1 To OrderedHsps.Count - 1
            Dim x1 As New ReturnPointForBlast(t.Last, OrderedHsps(i1))
            t.Add(x1)
        Next

        Dim LocBuilder As New Bio.IO.GenBank.LocationBuilder

        ' Gene
        Dim GeneLocation As New List(Of ReturnPointForBlast)
        GeneLocation.Add(New ReturnPointForBlast(OrderedHsps.First, BRecord.IterationQueryDefinition))
        GeneLocation.First.MdHitEnd = t.Last.MdHitEnd
        Locis.Add(LocBuilder.GetLocation(GetLOcationString(GeneLocation)))
        Dim s As String = GetLOcationString(t)
        Dim i = s.LastIndexOf("(")
        If i <> -1 Then
            Dim s1 = s.Insert(i + 1, "<")
            i = s1.LastIndexOf(".")
            s1 = s1.Insert(i + 1, ">")
            Locis.Add(LocBuilder.GetLocation(s1))
        Else
            Dim alf As Int16 = 54
        End If

        'CDS
        Dim Loci = LocBuilder.GetLocation(s)
        Locis.Add(Loci)
        Return Locis
    End Function
    Private Function GetmRNA(Loci As Bio.IO.GenBank.Location,
                             HitName As String,
                             Note As String,
                             Type As String) As Bio.IO.GenBank.MessengerRna
        Dim mRNA As New Bio.IO.GenBank.MessengerRna(Loci)
        Dim LocusTags As New List(Of String)
        LocusTags.Add(HitName)
        mRNA.LocusTag.Add(HitName)
        mRNA.Product.Add("Nodule Cysteine-Rich (NCR) secreted peptide")
        Dim Notes As New List(Of String)
        Notes.Add(Type & ";")
        Notes.Add(HitName & "." & Note)
        mRNA.Note.AddRange(Notes)
        Return mRNA

    End Function
    Private Function GetGene(Loci As Bio.IO.GenBank.Location,
                          HitName As String,
                          Note As String,
                          Type As String) As Bio.IO.GenBank.Gene
        Dim TheGene As New Bio.IO.GenBank.Gene(Loci)
        Dim LocusTags As New List(Of String)
        LocusTags.Add(HitName)
        TheGene.LocusTag.Add(HitName)
        TheGene.Product.Add("Nodule Cysteine-Rich (NCR) secreted peptide")
        Dim Notes As New List(Of String)
        Notes.Add("New by Szunyi" & ";")
        Notes.Add(HitName & "." & Note)
        TheGene.Note.AddRange(Notes)
        Return TheGene

    End Function
    Private Function GetCDS(Loci As Bio.IO.GenBank.Location,
                            HitName As String,
                            Note As String,
                            Type As String) As Bio.IO.GenBank.CodingSequence
        Dim mRNA As New Bio.IO.GenBank.CodingSequence(Loci)
        Dim LocusTags As New List(Of String)
        LocusTags.Add(HitName)
        mRNA.LocusTag.Add(HitName)
        mRNA.Product.Add("Nodule Cysteine-Rich (NCR) secreted peptide")
        Dim Notes As New List(Of String)
        Notes.Add(Type & ";")

        Notes.Add(HitName & "." & Note)
        mRNA.Note.AddRange(Notes)


        mRNA.CodonStart.Add("1")

        Return mRNA

    End Function

    Private Function getFilters() As Dictionary(Of String, String)
        Filters = New Dictionary(Of String, String)
        Dim x = Szunyi.IO.Files.Filter.SelectFiles
        For Each File In x
            Using sr As New StreamReader(File.FullName)
                Do
                    Dim Line As String = sr.ReadLine
                    Dim s1() = Split(Line, vbTab)
                    If Filters.ContainsKey(s1(0)) = False Then Filters.Add(s1(0), Line)
                Loop Until sr.EndOfStream = True
            End Using
        Next
        Return Filters
    End Function

#End Region
    Private Sub IndelManipulationToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles IndelManipulationToolStripMenuItem.Click
        '
        Dim str As New System.Text.StringBuilder
        Dim NewSequences As New List(Of Bio.ISequence)
        For Each Item In Me.ClonedAndFilteredBlastSearchRecords
            Dim TheHsp = Item.Hits.First.Hsps.First
            Dim t As New StringBuilder
            For i1 = 0 To TheHsp.Midline.Count - 1
                If TheHsp.QuerySequence(i1) = "-" Then
                    Dim alf As Int16 = 54
                ElseIf TheHsp.HitSequence(i1) = "-" Then
                    Dim N = TheHsp.QuerySequence(i1)
                    Dim c = GetConsecutive(i1, TheHsp.QuerySequence, N)
                    If c < 3 Then
                        t.Append(TheHsp.QuerySequence(i1))
                    End If

                Else
                    t.Append(TheHsp.HitSequence(i1))
                End If
            Next
            Dim MdQuerySeq As New Bio.Sequence(Alphabets.AmbiguousDNA, t.ToString)
            MdQuerySeq.ID = "Md" & Item.IterationQueryDefinition
            NewSequences.Add(MdQuerySeq)
            Dim MdQuerySeqAASeqs As New List(Of Bio.ISequence)
            Dim Frames = New Integer() {1, 2, 3, -1, -2, -3}
            MdQuerySeqAASeqs.AddRange(Szunyi.Translate.TranslateIntoSequence(MdQuerySeq, Frames))
            Dim MdQuerySeqMaxOrfLength As Integer = AA.GetLongestORFLengthFromSeqs(MdQuerySeqAASeqs, False, False)

            Dim QuerySeq As New Bio.Sequence(Alphabets.AmbiguousDNA, TheHsp.QuerySequence)
            Dim QuerySeqAASeqs As New List(Of Bio.ISequence)
            QuerySeqAASeqs.AddRange(Szunyi.Translate.TranslateIntoSequence(QuerySeq, Frames))
            Dim QuerySeqMaxOrfLength As Integer = AA.GetLongestORFLengthFromSeqs(QuerySeqAASeqs, False, False)

            Dim HitSeq As New Bio.Sequence(Alphabets.AmbiguousDNA, TheHsp.HitSequence)
            Dim HitSeqAASeqs As New List(Of Bio.ISequence)
            HitSeqAASeqs.AddRange(Szunyi.Translate.TranslateIntoSequence(HitSeq, Frames))
            Dim HitSeqMaxOrfLength As Integer = AA.GetLongestORFLengthFromSeqs(HitSeqAASeqs, False, False)
            str.Append(Item.IterationQueryDefinition).Append(vbTab)
            str.Append(HitSeqMaxOrfLength).Append(vbTab)
            str.Append(MdQuerySeqMaxOrfLength).Append(vbTab)
            str.Append(QuerySeqMaxOrfLength).AppendLine()

        Next
        Szunyi.IO.Export.SaveSequencesToSingleFasta(NewSequences)

    End Sub

    Private Function GetConsecutive(Index As Integer, Seq As String, N As String) As Integer
        Dim COunt As Int16 = 1
        For i2 = Index - 1 To Index - 4 Step -1
            If i2 > 0 Then
                If Seq(i2) = N Then
                    COunt += 1
                Else
                    Exit For
                End If
            End If
        Next
        For i2 = Index + 1 To Index + 4
            If i2 < Seq.Count Then
                If Seq(i2) = N Then
                    COunt += 1
                Else
                    Exit For
                End If
            End If
        Next
        Return COunt
    End Function


    Private Sub NearEndToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles NearEndToolStripMenuItem.Click
        Dim i1 As Integer = InputBox("Max bp at the End")
        Me.ClonedAndFilteredBlastSearchRecords = BlastFilter.GetFilteredBlastSearchRecordsNearEnd(ClonedAndFilteredBlastSearchRecords, i1)

        RefreshListbox1()
    End Sub
    Private Sub Hit5PrimeToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles Hit5PrimeToolStripMenuItem.Click
        Dim i1 As Integer = InputBox("Max bp at the End")
        Me.ClonedAndFilteredBlastSearchRecords = BlastFilter.GetFilteredBlastSearchRecordsNearHitEnd5(ClonedAndFilteredBlastSearchRecords, i1)

        RefreshListbox1()
    End Sub
#End Region

#Region "Export"
#Region "Export Filtered"
    Private Sub EF_Query_Seqs(sender As Object, e As EventArgs) Handles ToolStripMenuItem7.Click
        Export.Query_Sequences(Me.ClonedAndFilteredBlastSearchRecords, Me.OpenedFiles)
    End Sub

    Private Sub EF_Query_IDs(sender As Object, e As EventArgs) Handles ToolStripMenuItem8.Click
        Export.Query_Definitions(Me.ClonedAndFilteredBlastSearchRecords)
    End Sub

    Private Sub EF_Hit_Seqs(sender As Object, e As EventArgs) Handles ToolStripMenuItem9.Click
        Export.Hit_Sequences(Me.ClonedAndFilteredBlastSearchRecords, Me.OpenedFiles)
    End Sub

    Private Sub EF_Hit_IDs(sender As Object, e As EventArgs) Handles ToolStripMenuItem10.Click
        Export.Hit_IDs(Me.ClonedAndFilteredBlastSearchRecords)
    End Sub

    Private Sub EF_Best_Hits(sender As Object, e As EventArgs) Handles ToolStripMenuItem11.Click
        Export.Best_Hits(Me.ClonedAndFilteredBlastSearchRecords)
    End Sub

    Private Sub EF_All_Hits(sender As Object, e As EventArgs) Handles ToolStripMenuItem12.Click
        Export.All_Hits(Me.ClonedAndFilteredBlastSearchRecords)
    End Sub

    Private Sub EF_Blast_XML(sender As Object, e As EventArgs) Handles ToolStripMenuItem13.Click

    End Sub
#End Region
#Region "Export From All"
    Private Sub EA_Query_Seqs_Click(sender As Object, e As EventArgs) Handles EA_Query_Seqs.Click
        Szunyi.Blast.Export.Query_Sequences(Me.OriginalBlastSearchRecords, Me.OpenedFiles)
    End Sub

    Private Sub EA_Best_Hits_Click(sender As Object, e As EventArgs) Handles EA_Best_Hits.Click
        Szunyi.Blast.Export.Best_Hits(Me.OriginalBlastSearchRecords)
    End Sub

    Private Sub EA_Blast_XML_Click(sender As Object, e As EventArgs) Handles EA_Blast_XML.Click

    End Sub

    Private Sub EA_Hit_IDs_Click(sender As Object, e As EventArgs) Handles EA_Hit_IDs.Click
        Szunyi.Blast.Export.Hit_IDs(Me.OriginalBlastSearchRecords)
    End Sub

    Private Sub EA_Hit_Seqs_Click(sender As Object, e As EventArgs) Handles EA_Hit_Seqs.Click
        Export.Hit_Sequences(Me.OriginalBlastSearchRecords, Me.OpenedFiles)
    End Sub

    Private Sub EA_Query_IDS_Click(sender As Object, e As EventArgs) Handles EA_Query_IDS.Click
        Export.Query_Definitions(Me.OriginalBlastSearchRecords)
    End Sub



#End Region
#Region "Export Selected"
    Private Function Get_Items_From_Selected_Queries() As List(Of BlastSearchRecord)
        Dim Items As New List(Of BlastSearchRecord)
        For Each Item In Selected_Querys.Items
            Items.Add(Item)
        Next
        Return Items
    End Function
    Private Sub SelectedQueryIDsToolStripMenuItem_Click(sender As Object, e As EventArgs)
        Dim Items = Get_Items_From_Selected_Queries()
        Szunyi.Blast.Export.Query_Definitions(Items)
    End Sub


    Private Sub QuerySeqsToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles ES_Query_Seqs.Click
        Dim Items = Get_Items_From_Selected_Queries()
        Szunyi.Blast.Export.Query_Sequences(Items, Me.OpenedFiles)
        ' OK
    End Sub

    Private Sub QueryIDsToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles ES_Query_IDs.Click


    End Sub

    Private Sub HitSeqsToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles ES_Hit_Seqs.Click
        Dim Items = Get_Items_From_Selected_Queries()
        Szunyi.Blast.Export.Hit_Sequences(Items, Me.OpenedFiles)
        ' ok
    End Sub

    Private Sub HitIDsToolStripMenuItem1_Click(sender As Object, e As EventArgs) Handles ES_Hit_IDs.Click
        Dim Items = Get_Items_From_Selected_Queries()
        Szunyi.Blast.Export.Hit_IDs(Items)
        ' ok
    End Sub

    Private Sub BestHitsInTDTToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles ES_Best_Hits.Click
        Dim Items = Get_Items_From_Selected_Queries()
        Szunyi.Blast.Export.Best_Hits(Items)
    End Sub

    Private Sub BlastXMLToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles ES_Blast_XML.Click

    End Sub
#End Region
    Private Sub ExportTBlastNToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles ExportTBlastNToolStripMenuItem.Click
        Dim str As New System.Text.StringBuilder
        For Each Record In ClonedAndFilteredBlastSearchRecords
            If Record.Hits.Count <> 0 Then
                str.Append(Record.IterationQueryDefinition).Append(vbTab)
                str.Append(Record.Hits.First.Def).Append(vbTab)
                str.Append(Record.IterationQueryLength).Append(vbTab)
                '  Dim common = Helper.GetNumberOfSameLetterFromHSP(Record.Hits.First.Hsps.First)
                ' str.Append(common).Append(vbTab)
                '  str.Append((common / Record.IterationQueryLength) * 100).Append(vbTab)
                '  str.Append(GetHSpDescription(Record)).AppendLine()
            Else
                str.Append(Record.IterationQueryId)
                '    Helper.Text.Append(str, vbTab, 2)
                str.AppendLine()
            End If
        Next
        Szunyi.IO.Export.SaveText(str.ToString)
    End Sub

    Private Sub ExportHitSequencesInFasta(sender As Object, e As EventArgs)
        Dim log As New System.Text.StringBuilder

        If IsNothing(Me.HitSeqs) = True Then
            Dim DatabaseFiles = BlastBasicIO.GetDatabaseFiles(Me.OpenedFiles)

            Me.HitSeqs = BlastBasicIO.GetHitSeqsFromBlastDBs(DatabaseFiles, Me.ClonedAndFilteredBlastSearchRecords, log)
        End If
        Dim UniqueHitSeqs = Szunyi.Sequences.SequenceManipulation.UniqueDistinct.Get1CopyBySeqAndID(HitSeqs)
        Dim ORFs As List(Of Bio.ISequence) = BlastBasicIO.GetORFs(Me.HitSeqs, Me.ClonedAndFilteredBlastSearchRecords, log, Me.OriginalBlastSearchResults.First.Metadata.Program)

        Dim OrfsII = Szunyi.Sequences.SequenceManipulation.UniqueDistinct.Get1CopyBySeqAndID(ORFs)
        Dim The_DIr = Szunyi.IO.Directory.Get_Folder
        Dim FOlder = The_DIr.FullName
        For Each Group In Szunyi.Sequences.SequenceManipulation.UniqueDistinct.GetSeqGroupsBySeqID(OrfsII, 2)
            Dim GBKs As New List(Of Bio.ISequence) ' = CreateGenBankFrom_AAAndNA(Group, UniqueHitSeqs)

            Dim GBKFIle As New FileInfo(FOlder & "\" & Group.First.ID.Substring(0, 2) & ".gbk")
            Szunyi.IO.Export.SaveSequencesToSingleGenBank(GBKs, GBKFIle)

            Dim AAFIle As New FileInfo(FOlder & "\" & Group.First.ID.Substring(0, 2) & "_AA.fa")
            Dim IDs = Szunyi.Sequences.SequenceManipulation.Properties.GetIDs(Group)

            Szunyi.IO.Export.SaveSequencesToSingleFasta(Group, AAFIle)

            Dim NAFIle As New FileInfo(FOlder & "\" & Group.First.ID.Substring(0, 2) & "_NA.fa")
            Dim NASeqs = Szunyi.Sequences.SequenceManipulation.GetSequences.ByIDs(UniqueHitSeqs, IDs)
            Szunyi.IO.Export.SaveSequencesToSingleFasta(NASeqs, NAFIle)
        Next


    End Sub

#End Region

#Region "Discard All Not Perfect Hits"
    Private Sub Identity100QToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles Identity100QToolStripMenuItem.Click
        Me.ClonedAndFilteredBlastSearchRecords =
            BlastFilter.RemoveNotPerfectMatchesByHitLengthAndQueryLength(Me.ClonedAndFilteredBlastSearchRecords)
        RefreshListbox1()
    End Sub

    Private Sub Identity100QueryLengthToolStripMenuItem1_Click(sender As Object, e As EventArgs) Handles Identity100QueryLengthToolStripMenuItem1.Click
        Me.ClonedAndFilteredBlastSearchRecords =
           BlastFilter.RemoveNotPerfectMatchesByQueryLength(Me.ClonedAndFilteredBlastSearchRecords)
        RefreshListbox1()
    End Sub

    Private Sub Identity100HitLengthToolStripMenuItem1_Click(sender As Object, e As EventArgs) Handles Identity100HitLengthToolStripMenuItem1.Click
        Me.ClonedAndFilteredBlastSearchRecords =
           BlastFilter.RemoveNotPerfectMatchesByHitLength(Me.ClonedAndFilteredBlastSearchRecords)
        RefreshListbox1()
    End Sub
#End Region

#Region "Show All Hits Where There Is Perfect Match"
    Private Sub Identity100QueryLengthHitLengthToolStripMenuItem1_Click(sender As Object, e As EventArgs) Handles Identity100QueryLengthHitLengthToolStripMenuItem1.Click
        Me.ClonedAndFilteredBlastSearchRecords = BlastFilter.GetRecordswPerfectHits_Identity_Query_Hit(Me.ClonedAndFilteredBlastSearchRecords)
        RefreshListbox1()
    End Sub

    Private Sub Identity100QueryLengthToolStripMenuItem2_Click(sender As Object, e As EventArgs) Handles Identity100QueryLengthToolStripMenuItem2.Click
        Me.ClonedAndFilteredBlastSearchRecords = BlastFilter.GetRecordswPerfectHits_Identity_Query(Me.ClonedAndFilteredBlastSearchRecords)

        RefreshListbox1()
    End Sub

    Private Sub Identity100HitLengthToolStripMenuItem2_Click(sender As Object, e As EventArgs) Handles Identity100HitLengthToolStripMenuItem2.Click
        Me.ClonedAndFilteredBlastSearchRecords = BlastFilter.GetRecordswPerfectHits_Identity_Hit(Me.ClonedAndFilteredBlastSearchRecords)

        RefreshListbox1()
    End Sub

    Private Sub AllHitsCoverageToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles AllHitsCoverageToolStripMenuItem.Click
        Szunyi.Blast.Export.HitswCoverage(Me.ClonedAndFilteredBlastSearchRecords, Me.OriginalBlastSearchRecords)

    End Sub

    Private Sub FindGenomicToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles FindGenomicToolStripMenuItem.Click
        Dim x As New Szunyi.Blast.Genomic(Me.ClonedAndFilteredBlastSearchRecords, Me.OpenedFiles)
        x.DoIt()
    End Sub

    Private Sub ToolStripMenuItem1_Click(sender As Object, e As EventArgs) Handles ToolStripMenuItem1.Click
        ClonedAndFilteredBlastSearchRecords = BlastFilter.NotShowSelfHits(ClonedAndFilteredBlastSearchRecords)

        RefreshListbox1()
    End Sub



    Private Sub HitIDsToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles HitIDsToolStripMenuItem.Click
        Dim HitIDs = Szunyi.Blast.BlastManipulation.GetUniqueHitIDs(Me.ClonedAndFilteredBlastSearchRecords)
        Dim txt = Szunyi.Text.General.GetText(HitIDs)
        Szunyi.IO.Export.SaveText(txt)
    End Sub



    Private Sub Reset(sender As Object, e As EventArgs) Handles ToolStripTextBox1.Click
        Me.ClonedAndFilteredBlastSearchRecords = BlastManipulation.Clone(Me.OriginalBlastSearchRecords)
        RefreshListbox1()
    End Sub

    Private Sub tbHit_KeyDown(sender As Object, e As KeyEventArgs) Handles tbhit.KeyDown
        If e.KeyCode = Keys.Enter Then
            If tbhit.Text.Length >= 2 Then
                ClonedAndFilteredBlastSearchRecords = Szunyi.Blast.BlastFilter.GetFilteredBlastSearchRecordsByHit(tbhit.Text, Me.OriginalBlastSearchRecords)
                RefreshListbox1()

            Else
                Me.ClonedAndFilteredBlastSearchRecords = Szunyi.Blast.BlastManipulation.Clone(OriginalBlastSearchRecords)
                RefreshListbox1()
            End If
        End If
    End Sub

    Private Sub CreateDatabaseToolStripMenuItem1_Click(sender As Object, e As EventArgs) Handles CreateDatabaseToolStripMenuItem1.Click
        Dim File = Szunyi.IO.Files.Filter.SelectFile("Select Files",
                                              Szunyi.Constants.Files.Fasta,
                                              New DirectoryInfo(My.Resources.BlastFastaFilesPath))
        Dim x As New Szunyi.Blat.BlatDatabase(File)
        x.DoIt()
    End Sub

    Private Sub AsGffToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles AsGffToolStripMenuItem.Click
        Szunyi.Blast.Export.As_GFF(Me.ClonedAndFilteredBlastSearchRecords)


    End Sub



#End Region

#Region "Filter By Query IDs"
    Private Sub Discard_Equal_By_Query_IDs(sender As Object, e As EventArgs) Handles EqualToolStripMenuItem.Click
        Dim Query_IDs = Get_Strings()
        If IsNothing(Query_IDs) = True Then Exit Sub
        Me.ClonedAndFilteredBlastSearchRecords =
            BlastFilter.Discard_Equal_Query_Definitions(Me.ClonedAndFilteredBlastSearchRecords, Query_IDs)
        RefreshListbox1()
    End Sub

    Private Sub Discard_Contain_By_Query_IDs(sender As Object, e As EventArgs) Handles ContainToolStripMenuItem.Click
        Dim Query_IDs = Get_Strings()
        If IsNothing(Query_IDs) = True Then Exit Sub
        Me.ClonedAndFilteredBlastSearchRecords =
           BlastFilter.Discard_Contain_Query_Definitions(Me.ClonedAndFilteredBlastSearchRecords, Query_IDs)
        RefreshListbox1()
    End Sub

    Private Sub Discard_Start_With_By_Query_IDs(sender As Object, e As EventArgs) Handles StartWithToolStripMenuItem.Click
        Dim Query_IDs = Get_Strings()
        If IsNothing(Query_IDs) = True Then Exit Sub
        Me.ClonedAndFilteredBlastSearchRecords =
           BlastFilter.Discard_Start_With_Query_Definitions(Me.ClonedAndFilteredBlastSearchRecords, Query_IDs)
        RefreshListbox1()
    End Sub

    Private Sub Maintain_Equal_By_Query_IDs(sender As Object, e As EventArgs) Handles EqualToolStripMenuItem1.Click
        Dim Query_IDs = Get_Strings()
        If IsNothing(Query_IDs) = True Then Exit Sub
        Me.ClonedAndFilteredBlastSearchRecords =
           BlastFilter.Maintain_Equal_With_Query_Definitions(Me.ClonedAndFilteredBlastSearchRecords, Query_IDs)
        RefreshListbox1()
    End Sub

    Private Sub Maintain_Contain_By_Query_IDs(sender As Object, e As EventArgs) Handles ContainToolStripMenuItem1.Click
        Dim Query_IDs = Get_Strings()
        If IsNothing(Query_IDs) = True Then Exit Sub
        Me.ClonedAndFilteredBlastSearchRecords =
           BlastFilter.Maintain_Contain_With_Query_Definitions(Me.ClonedAndFilteredBlastSearchRecords, Query_IDs)
        RefreshListbox1()
    End Sub

    Private Sub Maintain_Start_With__By_Query_IDs(sender As Object, e As EventArgs) Handles StartWithToolStripMenuItem1.Click
        Dim Query_IDs = Get_Strings()
        If IsNothing(Query_IDs) = True Then Exit Sub
        Me.ClonedAndFilteredBlastSearchRecords =
           BlastFilter.Maintain_Start_With_Query_Definitions(Me.ClonedAndFilteredBlastSearchRecords, Query_IDs)
        RefreshListbox1()
    End Sub
    Private Function Get_Strings(Optional Title As String = "Enter Stings Separated By Carrige Return") As List(Of String)
        Dim InterestingStrings As New List(Of String)
        Dim x As New Get_List_of_String(Title)

        If x.ShowDialog = DialogResult.OK Then
            Return x.Strings
        End If
        Return Nothing
    End Function





    Private Sub Selected_Querys_SelectedIndexChanged(sender As Object, e As EventArgs) Handles Selected_Querys.MouseDoubleClick
        Dim Index = Selected_Querys.SelectedIndex
        If Index > -1 Then
            Selected_Querys.Items.RemoveAt(Index)
        End If

    End Sub

    Private Sub OptionsToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles OptionsToolStripMenuItem.Click

    End Sub

    Private Sub MultifastaByOrganismsToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles MultifastaByOrganismsToolStripMenuItem.Click
        ' Get Hit IDs
        Dim log As New System.Text.StringBuilder
        Dim QueryIDs = BlastManipulation.GetUniqueQueryDefintions(Me.ClonedAndFilteredBlastSearchRecords)
        Dim QueryFiles = BlastBasicIO.GetQueryFiles(Me.OpenedFiles)
        Dim Seqs = Szunyi.IO.Import.Sequence.FromFiles(QueryFiles)
        Dim QUery_Seqs = GetSequences.ByIDs(Seqs, QueryIDs)

        Dim uHit_IDs = Szunyi.Blast.BlastManipulation.GetUniqueHitIDs(Me.ClonedAndFilteredBlastSearchRecords)
        Dim Org_wGood_Genes As New Dictionary(Of String, Integer)

        For Each h In uHit_IDs
            Org_wGood_Genes.Add(h, 0)
        Next
        Dim Hit_Seqs = BlastBasicIO.GetHitSeqsFromBlastDBs(Me.OpenedFiles, Me.ClonedAndFilteredBlastSearchRecords, log)
        Dim Org = Szunyi.Text.Lists.Split_Items_Not_Last(uHit_IDs, "_")
        Dim nofGood As Integer = 1
        Dim res As New Dictionary(Of String, String)
        For Each o In Org
            res.Add(o, String.Empty)

        Next
        Dim QName = QUery_Seqs.First.ID.Split("_").First
        Dim COunt As Int16 = 0
        For Each brecord In Me.ClonedAndFilteredBlastSearchRecords
            Dim HitIds = (From x In brecord.Hits Select x.Id).ToList
            For Each hitid In HitIds
                Org_wGood_Genes(hitid) += 1
            Next
            Dim orgII = Szunyi.Text.Lists.Split_Items_Not_Last(HitIds, "_").Distinct.ToList

            If brecord.IterationQueryDefinition.Contains(Org.First) Then
                If orgII.Count = Org.Count Then
                    Dim alfu As Int16 = 65

                    COunt += 1

                    For Each HitID In HitIds
                        Dim OrgName = Szunyi.Text.Lists.Split_Item_Not_Last(HitID, "_")
                        res(OrgName) = res(OrgName) ' & Szunyi.Sequences.SequenceManipulation.GetSequences.ByID(Hit_Seqs, HitID).ConvertToString
                    Next
                    '  res(QName) = res(QName) & Szunyi.Sequences.SequenceManipulation.GetSequences.ByID(QUery_Seqs, brecord.IterationQueryDefinition).ConvertToString
                End If
            End If
        Next


        Dim str As New System.Text.StringBuilder
        Dim str2 As New System.Text.StringBuilder
        For Each item In res
            str2.Append(item.Key).Append(vbTab).Append(item.Value.Count).AppendLine()
            str.Append(">").Append(item.Key).AppendLine.Append(item.Value).AppendLine()
        Next
        Dim alf As Int16 = 43
    End Sub

    Private Sub MismatchesToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles MismatchesToolStripMenuItem.Click
        Szunyi.Blast.Export.Get_Mismatches(Me.ClonedAndFilteredBlastSearchRecords)
    End Sub

    Private Sub BestFormOutfmt7ToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles BestFormOutfmt7ToolStripMenuItem.Click
        Dim out As New Dictionary(Of String, List(Of Double))
        Dim file = Szunyi.IO.Files.Filter.SelectFile()
        If IsNothing(file) = True Then Exit Sub
        For Each Lines In Szunyi.IO.Import.Text.Parse_Group_Lines(file, "#")
            Dim Used As New List(Of String)
            For Each Line In Lines
                Dim s = Split(Line, vbTab)
                If Used.Contains(s(1)) = False Then
                    If out.ContainsKey(s(1)) = False Then out.Add(s(1), New List(Of Double))
                    out(s(1)).Add(s(2))
                    Used.Add(s(1))
                End If

            Next

        Next
        Dim str As New System.Text.StringBuilder
        str.Append("SeqID").Append(vbTab)
        str.Append("SIM Identity").Append(vbTab)
        str.Append("Nof Matches").Append(vbTab)
        str.Append("Avarage Identity").AppendLine()

        For Each o In out
            str.Append(o.Key).Append(vbTab)
            Dim d = o.Value.Sum
            str.Append(d).Append(vbTab)
            str.Append(o.Value.Count).Append(vbTab)
            str.Append(d / o.Value.Count).AppendLine()
            Clipboard.SetText(str.ToString)
        Next
        Clipboard.SetText(str.ToString)
    End Sub

    Private Sub COuntPerfectHitsToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles COuntPerfectHitsToolStripMenuItem.Click
        Dim out As New Dictionary(Of String, Integer)

        For Each R In Me.OriginalBlastSearchRecords
            For Each Hit In R.Hits

                For Each Hsp In Hit.Hsps
                    If Hsp.HitFrame < 0 Then
                        If Hsp.QueryStart < 20 And Hsp.HitEnd < 20 And Hsp.HitStart + 20 > Hit.Length And Hsp.QueryEnd + 20 > R.IterationQueryLength Then
                            Dim key As String = R.IterationQueryDefinition & vbTab & R.IterationQueryLength
                            If out.ContainsKey(key) = False Then out.Add(key, 0)
                            out(key) += 1
                        End If
                    Else
                        If Hsp.QueryStart < 20 And Hsp.HitStart < 20 And Hsp.HitEnd + 20 > Hit.Length And Hsp.QueryEnd + 20 > R.IterationQueryLength Then
                            Dim key As String = R.IterationQueryDefinition & vbTab & R.IterationQueryLength
                            If out.ContainsKey(key) = False Then out.Add(key, 0)
                            out(key) += 1
                        End If
                    End If

                Next
            Next
        Next
        Dim str As New System.Text.StringBuilder
        For Each o In out
            str.Append(o.Key).Append(vbTab).Append(o.Value).AppendLine()
        Next
        If str.Length <> 0 Then Clipboard.SetText(str.ToString)
        Dim alf As Int16 = 54
    End Sub

    Private Sub CountIncompleteToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles CountIncompleteToolStripMenuItem.Click
        Dim tmp As New Dictionary(Of BlastSearchRecord, Dictionary(Of String, List(Of Hit)))
        For Each R In Me.OriginalBlastSearchRecords
            tmp.Add(R, New Dictionary(Of String, List(Of Hit)))
            tmp(R).Add("PstI", New List(Of Hit))
            tmp(R).Add("StyI", New List(Of Hit))
            tmp(R).Add("HaeI", New List(Of Hit))
            For Each Hit In R.Hits
                Dim key = Hit.Id.Substring(0, 4)
                tmp(R)(key).Add(Hit)
            Next
        Next
        Dim log As New List(Of String)
        Dim logII As New List(Of String)
        Dim res As New Dictionary(Of String, List(Of Integer))
        Dim Nof_Ligated As Int16 = 0
        Dim All As New List(Of String)
        Dim Perfect As New List(Of String)
        Dim NotPerfect As New List(Of String)
        Dim Ligated As New List(Of String)
        Dim nofNotStrict As Integer = 0
        For Each Read In tmp
            Dim t = Has_Perfect(Read)
            Dim t1 = Is_Ligated(Read)
            If Read.Key.IterationQueryDefinition.Contains("146452") Then
                Dim kj As Int16 = 54
            End If
            If t <> String.Empty Or t1 = False Then
                If t <> String.Empty Then
                    Dim h = Read.Value(t).First
                    If res.ContainsKey(h.Id) = False Then res.Add(h.Id, New List(Of Integer))
                    Dim length = System.Math.Abs(h.Hsps.First.HitEnd - h.Hsps.First.HitStart) + 1
                    res(h.Id).Add(length)
                    If h.Length < Read.Key.IterationQueryLength - 30 Then
                        log.Add(Read.Key.IterationQueryDefinition)
                    End If
                Else
                    Dim allHits = BlastManipulation.Get_All_Hits(Read)
                    Dim AllHitsII = From x In allHits Where x.Hsps.Count = 1
                    Dim SameDuplication = From x In allHits Where x.Hsps.Count > 1

                    If SameDuplication.Count = 0 Then

                        Dim GoodHSPs = BlastManipulation.Get_All_HSPs_Near_End(AllHitsII.ToList, 20)
                        If GoodHSPs.Count = 1 Then
                            Dim TheHit = From x In allHits Where x.Hsps.Contains(GoodHSPs.First) Select x

                            Dim h = TheHit.First
                            If res.ContainsKey(h.Id) = False Then res.Add(h.Id, New List(Of Integer))
                            Dim length = System.Math.Abs(h.Hsps.First.HitEnd - h.Hsps.First.HitStart) + 1
                            res(h.Id).Add(length)

                            If h.Length < Read.Key.IterationQueryLength - 30 Then
                                log.Add(Read.Key.IterationQueryDefinition)
                            End If

                        ElseIf GoodHSPs.Count > 1 Then

                            If allHits.Count = 1 Then
                                Dim h = allHits.First
                                If res.ContainsKey(h.Id) = False Then res.Add(h.Id, New List(Of Integer))
                                Dim length = System.Math.Abs(h.Hsps.First.HitEnd - h.Hsps.First.HitStart) + 1
                                res(h.Id).Add(length)

                                If h.Length < Read.Key.IterationQueryLength - 30 Then
                                    log.Add(Read.Key.IterationQueryDefinition)
                                End If

                            Else
                                Dim gr = GetGroupsj(allHits)
                                Dim added As Integer = 0


                                For Each g In gr
                                    Dim gr1() As Hit = g.group

                                    If gr1.Count = 1 Then
                                        Dim h = gr1.First
                                        If res.ContainsKey(h.Id) = False Then res.Add(h.Id, New List(Of Integer))
                                        '    res(h.Id).Add(Read.Key.IterationQueryLength)
                                        If h.Length < Read.Key.IterationQueryLength - 30 Then
                                            log.Add(Read.Key.IterationQueryDefinition)
                                        End If
                                        added += 1
                                    End If
                                Next

                                For Each g In gr
                                    Dim gr1() As Hit = g.group

                                    If gr1.Count = 1 Then
                                        Dim h = gr1.First
                                        If res.ContainsKey(h.Id) = False Then res.Add(h.Id, New List(Of Integer))
                                        Dim length = System.Math.Abs(h.Hsps.First.HitEnd - h.Hsps.First.HitStart) + 1

                                        res(h.Id).Add(length)
                                        If h.Length < Read.Key.IterationQueryLength - 30 Then
                                            log.Add(Read.Key.IterationQueryDefinition)
                                        End If
                                        added += 1
                                    End If
                                Next
                                If added > 1 Then
                                    logII.Add(Read.Key.IterationQueryDefinition)
                                    nofNotStrict += 1
                                End If
                            End If
                        Else
                            ' Npo Good HSPS
                        End If
                    End If
                End If



            Else
                Ligated.Add(Read.Key.IterationQueryDefinition)
                Nof_Ligated += 1
            End If

        Next
        Dim strII As New StringBuilder
        Dim ah As New List(Of Hit)

        For Each item In res
            strII.Append(item.Key)
            item.Value.Sort()
            For Each i In item.Value
                strII.Append(vbTab).Append(i)
            Next
            strII.AppendLine()
        Next
        Dim laf = Szunyi.Text.General.GetText(All)
        Dim nPerf = Szunyi.Text.General.GetText(NotPerfect)
        Dim Lig = Szunyi.Text.General.GetText(Ligated)
        Dim Perf = Szunyi.Text.General.GetText(Perfect)
        Dim logs = Szunyi.Text.General.GetText(log)
        Dim logIIs = Szunyi.Text.General.GetText(logII)
        Clipboard.SetText(strII.ToString)
        Dim alf As Int16 = 54
    End Sub

    Private Function GetGroupsj(allHits As List(Of Hit)) As Object
        Dim gr = From x In allHits Group By RE = x.Accession.Substring(0, 4) Into Group

        Return gr.ToList
    End Function

    Private Function Is_Ligated(read As KeyValuePair(Of BlastSearchRecord, Dictionary(Of String, List(Of Hit)))) As Boolean
        Dim HSPs As New List(Of Hsp)
        For Each Item In read.Value
            HSPs.AddRange(BlastManipulation.Get_All_HSPs(Item.Value))
        Next
        If read.Value("HaeI").Count > 0 And (read.Value("PstI").Count > 0 Or read.Value("StyI").Count > 0) Then
            Return True
        End If
        Dim mdHSPs = BlastManipulation.ConvertHspS(HSPs)
        Dim FoundedPos(read.Key.IterationQueryLength - 1) As Boolean

        For Each Hsp In mdHSPs
            For i1 = Hsp.QueryStart - 1 To Hsp.QueryEnd - 1
                FoundedPos(i1) = True
            Next
        Next
        Dim NotMappedRegionLength = (From x In FoundedPos Where x = False).Count
        If NotMappedRegionLength > 100 Then
            Return True
        End If
        Dim SortedHSPs = From x In HSPs Order By x.QueryStart Ascending
        Dim sg As Int16 = 20
        For i1 = 0 To SortedHSPs.Count - 2
            For i2 = i1 + 1 To HSPs.Count - 1
                If SortedHSPs(i1).QueryStart < sg And
                    SortedHSPs(i1).QueryEnd + 2 * sg > SortedHSPs(i2).QueryStart And
                   System.Math.Abs(SortedHSPs(i2).QueryStart - SortedHSPs(i1).QueryEnd) < 2 * sg And
                    SortedHSPs(i2).QueryEnd + sg > read.Key.IterationQueryLength Then
                    Return True
                End If
            Next
        Next
        Return False
    End Function

    Private Function Has_Perfect(read As KeyValuePair(Of BlastSearchRecord, Dictionary(Of String, List(Of Hit)))) As String
        For Each s In read.Value
            If s.Value.Count = 1 Then
                If IsPerfectHSP(s.Value.First.Hsps.First, s.Value.First, s.Value.First.Length) Then
                    Return s.Key
                End If
            End If
        Next
        Return String.Empty
    End Function
    Private Function IsPerfectHSP(HSP As Hsp, Hit As Hit, Length As Integer) As Boolean
        If HSP.HitFrame < 0 Then
            If HSP.QueryStart < 20 And HSP.HitEnd < 20 And HSP.HitStart + 20 > Hit.Length And HSP.QueryEnd + 20 > Length Then
                Return True
            End If
        Else
            If HSP.QueryStart < 20 And HSP.HitStart < 20 And HSP.HitEnd + 20 > Hit.Length And HSP.QueryEnd + 20 > Length Then
                Return True
            End If
        End If
        Return False
    End Function

    Private Sub RemoveOverlappedHSPsToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles RemoveOverlappedHSPsToolStripMenuItem.Click
        Me.ClonedAndFilteredBlastSearchRecords = BlastFilter.Remove_OverLapped_HSPs(Me.ClonedAndFilteredBlastSearchRecords, 20)
        Me.ClonedAndFilteredBlastSearchRecords = BlastFilter.Remove_Single_HSPs(Me.ClonedAndFilteredBlastSearchRecords)
        Me.ClonedAndFilteredBlastSearchRecords = BlastFilter.Maintain_Opposite_Hit_Frames(Me.ClonedAndFilteredBlastSearchRecords)

        RefreshListbox1()
    End Sub

    Private Sub ES_All_Hits_Click(sender As Object, e As EventArgs) Handles ES_All_Hits.Click
        Dim Items = Get_Items_From_Selected_Queries()
        Szunyi.Blast.Export.All_Hits(Items)
    End Sub









#End Region


End Class

Public Class HspByLocation
    Public HspsByLocation As New Dictionary(Of String, List(Of LocationwHit)) 'chromosome

    Public Sub New(x As List(Of BlastResult))
        Dim NoHit As Integer = 0
        For Each t In x
            For Each Record In t.Records
                If Record.Hits.Count = 0 Then NoHit += 1
                For Each Hit In Record.Hits
                    For Each Hsp In Hit.Hsps
                        If HspsByLocation.ContainsKey(Hit.Def) = False Then HspsByLocation.Add(Hit.Def, New List(Of LocationwHit))
                        HspsByLocation(Hit.Def).Add(New LocationwHit(Hsp, Hit, Record))
                    Next
                Next
            Next
        Next

    End Sub

End Class

Public Class LocationwHit
    Public Location As Point
    Public TheHit As Hit
    Public Hsp As Hsp
    Public Record As BlastSearchRecord

    Sub New(Hsp As Hsp, Hit As Hit, Record As BlastSearchRecord)
        ' TODO: Complete member initialization 
        If Hsp.HitStart > Hsp.HitEnd Then
            Me.Location = New Point(Hsp.HitEnd, Hsp.HitStart)
        Else
            Me.Location = New Point(Hsp.HitStart, Hsp.HitEnd)
        End If

        Me.Hsp = Hsp
        Me.TheHit = Hit
        Me.Record = Record
    End Sub

End Class

Public Enum outfmt
    query_anchored_wIdentities = 1
    query_anchored_wNoIdentities = 2
    flat_query_anchore_wIdentities = 3
    flat_query_anchore_wNoIdentities = 4
    XML = 5
    tabular = 6
    tabular_With_Comment_Lines = 7
    Text_ASN_1 = 8
    Binary_ASN_1 = 9
    Comma_separated_values = 10
    BLAST_archive_format = 11


End Enum