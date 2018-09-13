<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Class FileManager
    Inherits System.Windows.Forms.Form

    'Form overrides dispose to clean up the component list.
    <System.Diagnostics.DebuggerNonUserCode()> _
    Protected Overrides Sub Dispose(ByVal disposing As Boolean)
        Try
            If disposing AndAlso components IsNot Nothing Then
                components.Dispose()
            End If
        Finally
            MyBase.Dispose(disposing)
        End Try
    End Sub

    'Required by the Windows Form Designer
    Private components As System.ComponentModel.IContainer

    'NOTE: The following procedure is required by the Windows Form Designer
    'It can be modified using the Windows Form Designer.  
    'Do not modify it using the code editor.
    <System.Diagnostics.DebuggerStepThrough()> _
    Private Sub InitializeComponent()
        Me.SplitContainer1 = New System.Windows.Forms.SplitContainer()
        Me.DataGridView1 = New System.Windows.Forms.DataGridView()
        Me.TextBox1 = New System.Windows.Forms.TextBox()
        Me.MenuStrip1 = New System.Windows.Forms.MenuStrip()
        Me.SelectToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem()
        Me.SMRTIDsToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem()
        Me.AnyToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem()
        Me.AllToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem()
        Me.FromFileToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem()
        Me.SelectTypeToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem()
        Me.RefreshToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem()
        Me.ReorderAndRenameSampleToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem()
        Me.ConsoleToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem()
        Me.GTHToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem()
        Me.DoGTHToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem()
        Me.ConvertGTHToBAMToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem()
        Me.TranscriptDiscoveryToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem()
        Me.GmapToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem()
        Me.FeaturesForDifferentExpToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem()
        Me.AllAnalysisToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem()
        Me.VCFToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem()
        Me.FreebayesToolStripMenuItem1 = New System.Windows.Forms.ToolStripMenuItem()
        Me.SamtoolsToolStripMenuItem1 = New System.Windows.Forms.ToolStripMenuItem()
        Me.FromManagerToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem()
        Me.FromFIlesToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem()
        Me.PySamBamStatisticToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem()
        Me.BasicToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem()
        Me.FreebayesToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem()
        Me.ExtraToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem()
        Me.GetMetaDatasToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem()
        Me.VariationsToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem()
        Me.StatisticToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem()
        Me.AllAndMappedSequnecesToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem()
        Me.MappingsToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem()
        Me.GetMappedReadCountsByFilesToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem()
        Me.CopyToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem()
        Me.FastaToolStripMenuItem2 = New System.Windows.Forms.ToolStripMenuItem()
        Me.FastqToolStripMenuItem1 = New System.Windows.Forms.ToolStripMenuItem()
        Me.BamToolStripMenuItem2 = New System.Windows.Forms.ToolStripMenuItem()
        Me.MergeBamFIlesToolStripMenuItem1 = New System.Windows.Forms.ToolStripMenuItem()
        Me.MappedReadsIntoFastQToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem()
        Me.ReadsWithMoreCountsToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem()
        Me.ReNameFilesToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem()
        Me.AnalysisToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem()
        Me.VCFToolStripMenuItem1 = New System.Windows.Forms.ToolStripMenuItem()
        Me.ReadLengthsToolStripMenuItem1 = New System.Windows.Forms.ToolStripMenuItem()
        Me.RepeatLengthsToolStripMenuItem1 = New System.Windows.Forms.ToolStripMenuItem()
        Me.CheckToolStripMenuItem1 = New System.Windows.Forms.ToolStripMenuItem()
        Me.FastaToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem()
        Me.FastQToolStripMenuItem2 = New System.Windows.Forms.ToolStripMenuItem()
        Me.BamToolStripMenuItem3 = New System.Windows.Forms.ToolStripMenuItem()
        Me.DuplicationToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem()
        Me.SMRTIDsFromFoldersToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem()
        Me.SMRTIDsCountsFromSamBAMToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem()
        CType(Me.SplitContainer1, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.SplitContainer1.Panel1.SuspendLayout()
        Me.SplitContainer1.Panel2.SuspendLayout()
        Me.SplitContainer1.SuspendLayout()
        CType(Me.DataGridView1, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.MenuStrip1.SuspendLayout()
        Me.SuspendLayout()
        '
        'SplitContainer1
        '
        Me.SplitContainer1.Dock = System.Windows.Forms.DockStyle.Fill
        Me.SplitContainer1.Location = New System.Drawing.Point(0, 24)
        Me.SplitContainer1.Name = "SplitContainer1"
        '
        'SplitContainer1.Panel1
        '
        Me.SplitContainer1.Panel1.Controls.Add(Me.DataGridView1)
        '
        'SplitContainer1.Panel2
        '
        Me.SplitContainer1.Panel2.Controls.Add(Me.TextBox1)
        Me.SplitContainer1.Size = New System.Drawing.Size(649, 286)
        Me.SplitContainer1.SplitterDistance = 431
        Me.SplitContainer1.TabIndex = 0
        '
        'DataGridView1
        '
        Me.DataGridView1.AllowUserToAddRows = False
        Me.DataGridView1.AllowUserToDeleteRows = False
        Me.DataGridView1.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize
        Me.DataGridView1.Dock = System.Windows.Forms.DockStyle.Fill
        Me.DataGridView1.Location = New System.Drawing.Point(0, 0)
        Me.DataGridView1.Name = "DataGridView1"
        Me.DataGridView1.Size = New System.Drawing.Size(431, 286)
        Me.DataGridView1.TabIndex = 0
        '
        'TextBox1
        '
        Me.TextBox1.Dock = System.Windows.Forms.DockStyle.Fill
        Me.TextBox1.Location = New System.Drawing.Point(0, 0)
        Me.TextBox1.MaxLength = 327670
        Me.TextBox1.Multiline = True
        Me.TextBox1.Name = "TextBox1"
        Me.TextBox1.ScrollBars = System.Windows.Forms.ScrollBars.Both
        Me.TextBox1.Size = New System.Drawing.Size(214, 286)
        Me.TextBox1.TabIndex = 0
        '
        'MenuStrip1
        '
        Me.MenuStrip1.Items.AddRange(New System.Windows.Forms.ToolStripItem() {Me.SelectToolStripMenuItem, Me.ConsoleToolStripMenuItem, Me.ExtraToolStripMenuItem, Me.StatisticToolStripMenuItem, Me.CopyToolStripMenuItem, Me.AnalysisToolStripMenuItem, Me.CheckToolStripMenuItem1})
        Me.MenuStrip1.Location = New System.Drawing.Point(0, 0)
        Me.MenuStrip1.Name = "MenuStrip1"
        Me.MenuStrip1.Size = New System.Drawing.Size(649, 24)
        Me.MenuStrip1.TabIndex = 1
        Me.MenuStrip1.Text = "MenuStrip1"
        '
        'SelectToolStripMenuItem
        '
        Me.SelectToolStripMenuItem.DropDownItems.AddRange(New System.Windows.Forms.ToolStripItem() {Me.SMRTIDsToolStripMenuItem, Me.SelectTypeToolStripMenuItem, Me.RefreshToolStripMenuItem, Me.ReorderAndRenameSampleToolStripMenuItem})
        Me.SelectToolStripMenuItem.Name = "SelectToolStripMenuItem"
        Me.SelectToolStripMenuItem.Size = New System.Drawing.Size(50, 20)
        Me.SelectToolStripMenuItem.Text = "Select"
        '
        'SMRTIDsToolStripMenuItem
        '
        Me.SMRTIDsToolStripMenuItem.DropDownItems.AddRange(New System.Windows.Forms.ToolStripItem() {Me.AnyToolStripMenuItem, Me.AllToolStripMenuItem, Me.FromFileToolStripMenuItem})
        Me.SMRTIDsToolStripMenuItem.Name = "SMRTIDsToolStripMenuItem"
        Me.SMRTIDsToolStripMenuItem.Size = New System.Drawing.Size(226, 22)
        Me.SMRTIDsToolStripMenuItem.Text = "SMRT IDs"
        '
        'AnyToolStripMenuItem
        '
        Me.AnyToolStripMenuItem.Name = "AnyToolStripMenuItem"
        Me.AnyToolStripMenuItem.Size = New System.Drawing.Size(123, 22)
        Me.AnyToolStripMenuItem.Text = "Any"
        '
        'AllToolStripMenuItem
        '
        Me.AllToolStripMenuItem.Name = "AllToolStripMenuItem"
        Me.AllToolStripMenuItem.Size = New System.Drawing.Size(123, 22)
        Me.AllToolStripMenuItem.Text = "All"
        '
        'FromFileToolStripMenuItem
        '
        Me.FromFileToolStripMenuItem.Name = "FromFileToolStripMenuItem"
        Me.FromFileToolStripMenuItem.Size = New System.Drawing.Size(123, 22)
        Me.FromFileToolStripMenuItem.Text = "From File"
        '
        'SelectTypeToolStripMenuItem
        '
        Me.SelectTypeToolStripMenuItem.Name = "SelectTypeToolStripMenuItem"
        Me.SelectTypeToolStripMenuItem.Size = New System.Drawing.Size(226, 22)
        Me.SelectTypeToolStripMenuItem.Text = "Select Type"
        '
        'RefreshToolStripMenuItem
        '
        Me.RefreshToolStripMenuItem.Name = "RefreshToolStripMenuItem"
        Me.RefreshToolStripMenuItem.Size = New System.Drawing.Size(226, 22)
        Me.RefreshToolStripMenuItem.Text = "Refresh"
        '
        'ReorderAndRenameSampleToolStripMenuItem
        '
        Me.ReorderAndRenameSampleToolStripMenuItem.Name = "ReorderAndRenameSampleToolStripMenuItem"
        Me.ReorderAndRenameSampleToolStripMenuItem.Size = New System.Drawing.Size(226, 22)
        Me.ReorderAndRenameSampleToolStripMenuItem.Text = "Reorder and Rename Sample"
        '
        'ConsoleToolStripMenuItem
        '
        Me.ConsoleToolStripMenuItem.DropDownItems.AddRange(New System.Windows.Forms.ToolStripItem() {Me.GTHToolStripMenuItem, Me.TranscriptDiscoveryToolStripMenuItem, Me.GmapToolStripMenuItem, Me.FeaturesForDifferentExpToolStripMenuItem, Me.AllAnalysisToolStripMenuItem, Me.VCFToolStripMenuItem, Me.PySamBamStatisticToolStripMenuItem, Me.FreebayesToolStripMenuItem})
        Me.ConsoleToolStripMenuItem.Name = "ConsoleToolStripMenuItem"
        Me.ConsoleToolStripMenuItem.Size = New System.Drawing.Size(62, 20)
        Me.ConsoleToolStripMenuItem.Text = "Console"
        '
        'GTHToolStripMenuItem
        '
        Me.GTHToolStripMenuItem.DropDownItems.AddRange(New System.Windows.Forms.ToolStripItem() {Me.DoGTHToolStripMenuItem, Me.ConvertGTHToBAMToolStripMenuItem})
        Me.GTHToolStripMenuItem.Name = "GTHToolStripMenuItem"
        Me.GTHToolStripMenuItem.Size = New System.Drawing.Size(208, 22)
        Me.GTHToolStripMenuItem.Text = "GTH"
        '
        'DoGTHToolStripMenuItem
        '
        Me.DoGTHToolStripMenuItem.Name = "DoGTHToolStripMenuItem"
        Me.DoGTHToolStripMenuItem.Size = New System.Drawing.Size(186, 22)
        Me.DoGTHToolStripMenuItem.Text = "Do GTH"
        '
        'ConvertGTHToBAMToolStripMenuItem
        '
        Me.ConvertGTHToBAMToolStripMenuItem.Name = "ConvertGTHToBAMToolStripMenuItem"
        Me.ConvertGTHToBAMToolStripMenuItem.Size = New System.Drawing.Size(186, 22)
        Me.ConvertGTHToBAMToolStripMenuItem.Text = "Convert GTH to BAM"
        '
        'TranscriptDiscoveryToolStripMenuItem
        '
        Me.TranscriptDiscoveryToolStripMenuItem.Name = "TranscriptDiscoveryToolStripMenuItem"
        Me.TranscriptDiscoveryToolStripMenuItem.Size = New System.Drawing.Size(208, 22)
        Me.TranscriptDiscoveryToolStripMenuItem.Text = "Transcript Discovery"
        '
        'GmapToolStripMenuItem
        '
        Me.GmapToolStripMenuItem.Name = "GmapToolStripMenuItem"
        Me.GmapToolStripMenuItem.Size = New System.Drawing.Size(208, 22)
        Me.GmapToolStripMenuItem.Text = "Gmap"
        '
        'FeaturesForDifferentExpToolStripMenuItem
        '
        Me.FeaturesForDifferentExpToolStripMenuItem.Name = "FeaturesForDifferentExpToolStripMenuItem"
        Me.FeaturesForDifferentExpToolStripMenuItem.Size = New System.Drawing.Size(208, 22)
        Me.FeaturesForDifferentExpToolStripMenuItem.Text = "Features For Different Exp"
        '
        'AllAnalysisToolStripMenuItem
        '
        Me.AllAnalysisToolStripMenuItem.Name = "AllAnalysisToolStripMenuItem"
        Me.AllAnalysisToolStripMenuItem.Size = New System.Drawing.Size(208, 22)
        Me.AllAnalysisToolStripMenuItem.Text = "All Analysis"
        '
        'VCFToolStripMenuItem
        '
        Me.VCFToolStripMenuItem.DropDownItems.AddRange(New System.Windows.Forms.ToolStripItem() {Me.FreebayesToolStripMenuItem1, Me.SamtoolsToolStripMenuItem1})
        Me.VCFToolStripMenuItem.Name = "VCFToolStripMenuItem"
        Me.VCFToolStripMenuItem.Size = New System.Drawing.Size(208, 22)
        Me.VCFToolStripMenuItem.Text = "VCF"
        '
        'FreebayesToolStripMenuItem1
        '
        Me.FreebayesToolStripMenuItem1.Name = "FreebayesToolStripMenuItem1"
        Me.FreebayesToolStripMenuItem1.Size = New System.Drawing.Size(126, 22)
        Me.FreebayesToolStripMenuItem1.Text = "Freebayes"
        '
        'SamtoolsToolStripMenuItem1
        '
        Me.SamtoolsToolStripMenuItem1.DropDownItems.AddRange(New System.Windows.Forms.ToolStripItem() {Me.FromManagerToolStripMenuItem, Me.FromFIlesToolStripMenuItem})
        Me.SamtoolsToolStripMenuItem1.Name = "SamtoolsToolStripMenuItem1"
        Me.SamtoolsToolStripMenuItem1.Size = New System.Drawing.Size(126, 22)
        Me.SamtoolsToolStripMenuItem1.Text = "Samtools"
        '
        'FromManagerToolStripMenuItem
        '
        Me.FromManagerToolStripMenuItem.Name = "FromManagerToolStripMenuItem"
        Me.FromManagerToolStripMenuItem.Size = New System.Drawing.Size(152, 22)
        Me.FromManagerToolStripMenuItem.Text = "From Manager"
        '
        'FromFIlesToolStripMenuItem
        '
        Me.FromFIlesToolStripMenuItem.Name = "FromFIlesToolStripMenuItem"
        Me.FromFIlesToolStripMenuItem.Size = New System.Drawing.Size(152, 22)
        Me.FromFIlesToolStripMenuItem.Text = "From FIles"
        '
        'PySamBamStatisticToolStripMenuItem
        '
        Me.PySamBamStatisticToolStripMenuItem.DropDownItems.AddRange(New System.Windows.Forms.ToolStripItem() {Me.BasicToolStripMenuItem})
        Me.PySamBamStatisticToolStripMenuItem.Name = "PySamBamStatisticToolStripMenuItem"
        Me.PySamBamStatisticToolStripMenuItem.Size = New System.Drawing.Size(208, 22)
        Me.PySamBamStatisticToolStripMenuItem.Text = "PySam Bam Statistic"
        '
        'BasicToolStripMenuItem
        '
        Me.BasicToolStripMenuItem.Name = "BasicToolStripMenuItem"
        Me.BasicToolStripMenuItem.Size = New System.Drawing.Size(101, 22)
        Me.BasicToolStripMenuItem.Text = "Basic"
        '
        'FreebayesToolStripMenuItem
        '
        Me.FreebayesToolStripMenuItem.Name = "FreebayesToolStripMenuItem"
        Me.FreebayesToolStripMenuItem.Size = New System.Drawing.Size(208, 22)
        Me.FreebayesToolStripMenuItem.Text = "Freebayes"
        '
        'ExtraToolStripMenuItem
        '
        Me.ExtraToolStripMenuItem.DropDownItems.AddRange(New System.Windows.Forms.ToolStripItem() {Me.GetMetaDatasToolStripMenuItem, Me.VariationsToolStripMenuItem})
        Me.ExtraToolStripMenuItem.Name = "ExtraToolStripMenuItem"
        Me.ExtraToolStripMenuItem.Size = New System.Drawing.Size(44, 20)
        Me.ExtraToolStripMenuItem.Text = "Extra"
        '
        'GetMetaDatasToolStripMenuItem
        '
        Me.GetMetaDatasToolStripMenuItem.Name = "GetMetaDatasToolStripMenuItem"
        Me.GetMetaDatasToolStripMenuItem.Size = New System.Drawing.Size(151, 22)
        Me.GetMetaDatasToolStripMenuItem.Text = "Get MetaDatas"
        '
        'VariationsToolStripMenuItem
        '
        Me.VariationsToolStripMenuItem.Name = "VariationsToolStripMenuItem"
        Me.VariationsToolStripMenuItem.Size = New System.Drawing.Size(151, 22)
        Me.VariationsToolStripMenuItem.Text = "Variations"
        '
        'StatisticToolStripMenuItem
        '
        Me.StatisticToolStripMenuItem.DropDownItems.AddRange(New System.Windows.Forms.ToolStripItem() {Me.AllAndMappedSequnecesToolStripMenuItem, Me.MappingsToolStripMenuItem, Me.GetMappedReadCountsByFilesToolStripMenuItem})
        Me.StatisticToolStripMenuItem.Name = "StatisticToolStripMenuItem"
        Me.StatisticToolStripMenuItem.Size = New System.Drawing.Size(60, 20)
        Me.StatisticToolStripMenuItem.Text = "Statistic"
        '
        'AllAndMappedSequnecesToolStripMenuItem
        '
        Me.AllAndMappedSequnecesToolStripMenuItem.Name = "AllAndMappedSequnecesToolStripMenuItem"
        Me.AllAndMappedSequnecesToolStripMenuItem.Size = New System.Drawing.Size(251, 22)
        Me.AllAndMappedSequnecesToolStripMenuItem.Text = "All and Mapped Sequneces"
        '
        'MappingsToolStripMenuItem
        '
        Me.MappingsToolStripMenuItem.Name = "MappingsToolStripMenuItem"
        Me.MappingsToolStripMenuItem.Size = New System.Drawing.Size(251, 22)
        Me.MappingsToolStripMenuItem.Text = "Mappings"
        '
        'GetMappedReadCountsByFilesToolStripMenuItem
        '
        Me.GetMappedReadCountsByFilesToolStripMenuItem.Name = "GetMappedReadCountsByFilesToolStripMenuItem"
        Me.GetMappedReadCountsByFilesToolStripMenuItem.Size = New System.Drawing.Size(251, 22)
        Me.GetMappedReadCountsByFilesToolStripMenuItem.Text = "Get Mapped Read Counts By Files"
        '
        'CopyToolStripMenuItem
        '
        Me.CopyToolStripMenuItem.DropDownItems.AddRange(New System.Windows.Forms.ToolStripItem() {Me.FastaToolStripMenuItem2, Me.FastqToolStripMenuItem1, Me.BamToolStripMenuItem2, Me.MergeBamFIlesToolStripMenuItem1, Me.MappedReadsIntoFastQToolStripMenuItem, Me.ReadsWithMoreCountsToolStripMenuItem, Me.ReNameFilesToolStripMenuItem})
        Me.CopyToolStripMenuItem.Name = "CopyToolStripMenuItem"
        Me.CopyToolStripMenuItem.Size = New System.Drawing.Size(47, 20)
        Me.CopyToolStripMenuItem.Text = "Copy"
        '
        'FastaToolStripMenuItem2
        '
        Me.FastaToolStripMenuItem2.Name = "FastaToolStripMenuItem2"
        Me.FastaToolStripMenuItem2.Size = New System.Drawing.Size(209, 22)
        Me.FastaToolStripMenuItem2.Text = "Fasta"
        '
        'FastqToolStripMenuItem1
        '
        Me.FastqToolStripMenuItem1.Name = "FastqToolStripMenuItem1"
        Me.FastqToolStripMenuItem1.Size = New System.Drawing.Size(209, 22)
        Me.FastqToolStripMenuItem1.Text = "Fastq"
        '
        'BamToolStripMenuItem2
        '
        Me.BamToolStripMenuItem2.Name = "BamToolStripMenuItem2"
        Me.BamToolStripMenuItem2.Size = New System.Drawing.Size(209, 22)
        Me.BamToolStripMenuItem2.Text = "Bam"
        '
        'MergeBamFIlesToolStripMenuItem1
        '
        Me.MergeBamFIlesToolStripMenuItem1.Name = "MergeBamFIlesToolStripMenuItem1"
        Me.MergeBamFIlesToolStripMenuItem1.Size = New System.Drawing.Size(209, 22)
        Me.MergeBamFIlesToolStripMenuItem1.Text = "Merge Bam FIles"
        '
        'MappedReadsIntoFastQToolStripMenuItem
        '
        Me.MappedReadsIntoFastQToolStripMenuItem.Name = "MappedReadsIntoFastQToolStripMenuItem"
        Me.MappedReadsIntoFastQToolStripMenuItem.Size = New System.Drawing.Size(209, 22)
        Me.MappedReadsIntoFastQToolStripMenuItem.Text = "Mapped Reads into FastQ"
        '
        'ReadsWithMoreCountsToolStripMenuItem
        '
        Me.ReadsWithMoreCountsToolStripMenuItem.Name = "ReadsWithMoreCountsToolStripMenuItem"
        Me.ReadsWithMoreCountsToolStripMenuItem.Size = New System.Drawing.Size(209, 22)
        Me.ReadsWithMoreCountsToolStripMenuItem.Text = "Reads With More Counts"
        '
        'ReNameFilesToolStripMenuItem
        '
        Me.ReNameFilesToolStripMenuItem.Name = "ReNameFilesToolStripMenuItem"
        Me.ReNameFilesToolStripMenuItem.Size = New System.Drawing.Size(209, 22)
        Me.ReNameFilesToolStripMenuItem.Text = "ReName Files"
        '
        'AnalysisToolStripMenuItem
        '
        Me.AnalysisToolStripMenuItem.DropDownItems.AddRange(New System.Windows.Forms.ToolStripItem() {Me.VCFToolStripMenuItem1, Me.ReadLengthsToolStripMenuItem1, Me.RepeatLengthsToolStripMenuItem1})
        Me.AnalysisToolStripMenuItem.Name = "AnalysisToolStripMenuItem"
        Me.AnalysisToolStripMenuItem.Size = New System.Drawing.Size(62, 20)
        Me.AnalysisToolStripMenuItem.Text = "Analysis"
        '
        'VCFToolStripMenuItem1
        '
        Me.VCFToolStripMenuItem1.Name = "VCFToolStripMenuItem1"
        Me.VCFToolStripMenuItem1.Size = New System.Drawing.Size(155, 22)
        Me.VCFToolStripMenuItem1.Text = "VCF"
        '
        'ReadLengthsToolStripMenuItem1
        '
        Me.ReadLengthsToolStripMenuItem1.Name = "ReadLengthsToolStripMenuItem1"
        Me.ReadLengthsToolStripMenuItem1.Size = New System.Drawing.Size(155, 22)
        Me.ReadLengthsToolStripMenuItem1.Text = "Read Lengths"
        '
        'RepeatLengthsToolStripMenuItem1
        '
        Me.RepeatLengthsToolStripMenuItem1.Name = "RepeatLengthsToolStripMenuItem1"
        Me.RepeatLengthsToolStripMenuItem1.Size = New System.Drawing.Size(155, 22)
        Me.RepeatLengthsToolStripMenuItem1.Text = "Repeat Lengths"
        '
        'CheckToolStripMenuItem1
        '
        Me.CheckToolStripMenuItem1.DropDownItems.AddRange(New System.Windows.Forms.ToolStripItem() {Me.FastaToolStripMenuItem, Me.FastQToolStripMenuItem2, Me.BamToolStripMenuItem3, Me.DuplicationToolStripMenuItem, Me.SMRTIDsFromFoldersToolStripMenuItem, Me.SMRTIDsCountsFromSamBAMToolStripMenuItem})
        Me.CheckToolStripMenuItem1.Name = "CheckToolStripMenuItem1"
        Me.CheckToolStripMenuItem1.Size = New System.Drawing.Size(52, 20)
        Me.CheckToolStripMenuItem1.Text = "Check"
        '
        'FastaToolStripMenuItem
        '
        Me.FastaToolStripMenuItem.Name = "FastaToolStripMenuItem"
        Me.FastaToolStripMenuItem.Size = New System.Drawing.Size(257, 22)
        Me.FastaToolStripMenuItem.Text = "Fasta"
        '
        'FastQToolStripMenuItem2
        '
        Me.FastQToolStripMenuItem2.Name = "FastQToolStripMenuItem2"
        Me.FastQToolStripMenuItem2.Size = New System.Drawing.Size(257, 22)
        Me.FastQToolStripMenuItem2.Text = "FastQ"
        '
        'BamToolStripMenuItem3
        '
        Me.BamToolStripMenuItem3.Name = "BamToolStripMenuItem3"
        Me.BamToolStripMenuItem3.Size = New System.Drawing.Size(257, 22)
        Me.BamToolStripMenuItem3.Text = "Bam"
        '
        'DuplicationToolStripMenuItem
        '
        Me.DuplicationToolStripMenuItem.Name = "DuplicationToolStripMenuItem"
        Me.DuplicationToolStripMenuItem.Size = New System.Drawing.Size(257, 22)
        Me.DuplicationToolStripMenuItem.Text = "Duplication"
        '
        'SMRTIDsFromFoldersToolStripMenuItem
        '
        Me.SMRTIDsFromFoldersToolStripMenuItem.Name = "SMRTIDsFromFoldersToolStripMenuItem"
        Me.SMRTIDsFromFoldersToolStripMenuItem.Size = New System.Drawing.Size(257, 22)
        Me.SMRTIDsFromFoldersToolStripMenuItem.Text = "SMRT IDs From Folders"
        '
        'SMRTIDsCountsFromSamBAMToolStripMenuItem
        '
        Me.SMRTIDsCountsFromSamBAMToolStripMenuItem.Name = "SMRTIDsCountsFromSamBAMToolStripMenuItem"
        Me.SMRTIDsCountsFromSamBAMToolStripMenuItem.Size = New System.Drawing.Size(257, 22)
        Me.SMRTIDsCountsFromSamBAMToolStripMenuItem.Text = "SMRT_IDs Counts  From Sam_BAM"
        '
        'FileManager
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(6.0!, 13.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.ClientSize = New System.Drawing.Size(649, 310)
        Me.Controls.Add(Me.SplitContainer1)
        Me.Controls.Add(Me.MenuStrip1)
        Me.MainMenuStrip = Me.MenuStrip1
        Me.Name = "FileManager"
        Me.Text = "FileManager"
        Me.SplitContainer1.Panel1.ResumeLayout(False)
        Me.SplitContainer1.Panel2.ResumeLayout(False)
        Me.SplitContainer1.Panel2.PerformLayout()
        CType(Me.SplitContainer1, System.ComponentModel.ISupportInitialize).EndInit()
        Me.SplitContainer1.ResumeLayout(False)
        CType(Me.DataGridView1, System.ComponentModel.ISupportInitialize).EndInit()
        Me.MenuStrip1.ResumeLayout(False)
        Me.MenuStrip1.PerformLayout()
        Me.ResumeLayout(False)
        Me.PerformLayout()

    End Sub

    Friend WithEvents SplitContainer1 As SplitContainer
    Friend WithEvents MenuStrip1 As MenuStrip
    Friend WithEvents SelectToolStripMenuItem As ToolStripMenuItem
    Friend WithEvents SMRTIDsToolStripMenuItem As ToolStripMenuItem
    Friend WithEvents AnyToolStripMenuItem As ToolStripMenuItem
    Friend WithEvents AllToolStripMenuItem As ToolStripMenuItem
    Friend WithEvents SelectTypeToolStripMenuItem As ToolStripMenuItem
    Friend WithEvents ConsoleToolStripMenuItem As ToolStripMenuItem
    Friend WithEvents GTHToolStripMenuItem As ToolStripMenuItem
    Friend WithEvents TranscriptDiscoveryToolStripMenuItem As ToolStripMenuItem
    Friend WithEvents ExtraToolStripMenuItem As ToolStripMenuItem
    Friend WithEvents GetMetaDatasToolStripMenuItem As ToolStripMenuItem
    Friend WithEvents DataGridView1 As DataGridView
    Friend WithEvents TextBox1 As TextBox
    Friend WithEvents DoGTHToolStripMenuItem As ToolStripMenuItem
    Friend WithEvents ConvertGTHToBAMToolStripMenuItem As ToolStripMenuItem
    Friend WithEvents GmapToolStripMenuItem As ToolStripMenuItem
    Friend WithEvents StatisticToolStripMenuItem As ToolStripMenuItem
    Friend WithEvents FeaturesForDifferentExpToolStripMenuItem As ToolStripMenuItem
    Friend WithEvents AllAnalysisToolStripMenuItem As ToolStripMenuItem
    Friend WithEvents RefreshToolStripMenuItem As ToolStripMenuItem
    Friend WithEvents ReorderAndRenameSampleToolStripMenuItem As ToolStripMenuItem
    Friend WithEvents FromFileToolStripMenuItem As ToolStripMenuItem
    Friend WithEvents CopyToolStripMenuItem As ToolStripMenuItem
    Friend WithEvents FastaToolStripMenuItem2 As ToolStripMenuItem
    Friend WithEvents FastqToolStripMenuItem1 As ToolStripMenuItem
    Friend WithEvents BamToolStripMenuItem2 As ToolStripMenuItem
    Friend WithEvents VCFToolStripMenuItem As ToolStripMenuItem
    Friend WithEvents FreebayesToolStripMenuItem1 As ToolStripMenuItem
    Friend WithEvents SamtoolsToolStripMenuItem1 As ToolStripMenuItem
    Friend WithEvents AnalysisToolStripMenuItem As ToolStripMenuItem
    Friend WithEvents VCFToolStripMenuItem1 As ToolStripMenuItem
    Friend WithEvents ReadLengthsToolStripMenuItem1 As ToolStripMenuItem
    Friend WithEvents CheckToolStripMenuItem1 As ToolStripMenuItem
    Friend WithEvents FastaToolStripMenuItem As ToolStripMenuItem
    Friend WithEvents FastQToolStripMenuItem2 As ToolStripMenuItem
    Friend WithEvents BamToolStripMenuItem3 As ToolStripMenuItem
    Friend WithEvents DuplicationToolStripMenuItem As ToolStripMenuItem
    Friend WithEvents AllAndMappedSequnecesToolStripMenuItem As ToolStripMenuItem
    Friend WithEvents RepeatLengthsToolStripMenuItem1 As ToolStripMenuItem
    Friend WithEvents FromManagerToolStripMenuItem As ToolStripMenuItem
    Friend WithEvents FromFIlesToolStripMenuItem As ToolStripMenuItem
    Friend WithEvents SMRTIDsFromFoldersToolStripMenuItem As ToolStripMenuItem
    Friend WithEvents PySamBamStatisticToolStripMenuItem As ToolStripMenuItem
    Friend WithEvents BasicToolStripMenuItem As ToolStripMenuItem
    Friend WithEvents VariationsToolStripMenuItem As ToolStripMenuItem
    Friend WithEvents MappingsToolStripMenuItem As ToolStripMenuItem
    Friend WithEvents GetMappedReadCountsByFilesToolStripMenuItem As ToolStripMenuItem
    Friend WithEvents MergeBamFIlesToolStripMenuItem1 As ToolStripMenuItem
    Friend WithEvents MappedReadsIntoFastQToolStripMenuItem As ToolStripMenuItem
    Friend WithEvents ReadsWithMoreCountsToolStripMenuItem As ToolStripMenuItem
    Friend WithEvents FreebayesToolStripMenuItem As ToolStripMenuItem
    Friend WithEvents ReNameFilesToolStripMenuItem As ToolStripMenuItem
    Friend WithEvents SMRTIDsCountsFromSamBAMToolStripMenuItem As ToolStripMenuItem
End Class
