Imports System.ComponentModel
Imports System.IO
Imports System.Text
Imports System.Text.RegularExpressions
Imports Bio
Imports Bio.Core
Imports Bio.IO.GenBank
Imports FV_VI.Szunyi
Imports FV_VI.Szunyi.ListOf
Imports FV_VI.Szunyi.Sequences

Public Class Form1
    Public Property Helper As New ListOfs
    Public SelectedItems As New BindingList(Of NameAndIDAndType)
    Dim QulifierSelector As New SelectQualifiers(Bio.IO.GenBank.StandardQualifierNames.All.ToList)

    Dim LocationBuilder As New LocationBuilder

    Dim SeqIDComparer As New Szunyi.Comparares.SequenceIDComparer

    Dim WithEvents PSequenceList As New ListManager
    Dim WithEvents PLocationsList As New ListManager
    Dim WithEvents PFeaturesList As New ListManager
    Dim WithEvents PMappingList As New ListManager
    Private Sub Form1_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        PSequenceList.GroupBox1.Text = MyConstants.BackGroundWork.Sequences
        PLocationsList.GroupBox1.Text = MyConstants.BackGroundWork.Locations
        PFeaturesList.GroupBox1.Text = MyConstants.BackGroundWork.Features
        PMappingList.GroupBox1.Text = MyConstants.BackGroundWork.Mapping

        ListBox1.DataSource = SelectedItems
        ListBox1.DisplayMember = MyConstants.Name
        Me.FlowLayoutPanel1.Controls.Add(PSequenceList)
        Me.FlowLayoutPanel1.Controls.Add(PLocationsList)
        Me.FlowLayoutPanel1.Controls.Add(PFeaturesList)
        Me.FlowLayoutPanel1.Controls.Add(PMappingList)

    End Sub


#Region "BackGroundWorker"
    Private Sub CreateBgWork(Type As String, t As Object)
        Dim w = New BackgroundWorker
        w.WorkerReportsProgress = True
        w.WorkerSupportsCancellation = True
        AddHandler w.DoWork, AddressOf WorkerDoWork
        AddHandler w.ProgressChanged, AddressOf WorkerProgressChanged
        AddHandler w.RunWorkerCompleted, AddressOf WorkerCompleted

        w.RunWorkerAsync(t)

    End Sub
    Private Sub AddToSequenceList(e As SequenceList)
        SyncLock ListOfs.ListOfSequences
            ListOfs.ListOfSequences.Add(e)
            ListOfs.ListOfSequences.Last.UniqueID = ListOfs.NofSequenceList
            Me.PSequenceList.Add(New NameAndID(ListOfs.ListOfSequences.Last.ShortFileName & " e:" &
                                               ListOfs.ListOfSequences.Last.Sequences.Count, ListOfs.NofSequenceList))
            ListOfs.NofSequenceList += 1
        End SyncLock
    End Sub
    Private Sub AddToMappingList(e As Object)
        SyncLock ListOfs.ListOfMappings
            ListOfs.ListOfMappings.Add(e)
            ListOfs.ListOfMappings.Last.UniqueID = ListOfs.NofMapping
            Me.PSequenceList.Add(New NameAndID(ListOfs.ListOfMappings.Last.ShortFileName, ListOfs.NofMapping))
            ListOfs.NofMapping += 1
        End SyncLock
    End Sub
    Private Sub WorkerCompleted(sender As Object, e As RunWorkerCompletedEventArgs)
        Try


            Select Case e.Result.Type
                Case MyConstants.BackGroundWork.Gff3Parser
                    AddToSequenceList(e.Result.Result)
                Case MyConstants.BackGroundWork.ModyfiedSequence
                    AddToSequenceList(e.Result.seqList)
                Case MyConstants.BackGroundWork.Sequences
                    AddToSequenceList(e.Result)
                Case MyConstants.BackGroundWork.Locations
                    SyncLock ListOfs.ListOfLocations
                        ListOfs.ListOfLocations.Add(e.Result)
                        ListOfs.NofLocations += 1
                        Me.PLocationsList.Add(New NameAndID(ListOfs.ListOfLocations.Last.ShortFileName, ListOfs.NofLocations))
                    End SyncLock
                Case MyConstants.BackGroundWork.Features
                    SyncLock ListOfs.ListOfExtFeatures
                        ListOfs.NofExtFeatures += 1
                        Dim t As ExtFeatureList = e.Result
                        t.UniqueId = ListOfs.NofExtFeatures
                        ListOfs.ListOfExtFeatures.Add(t)

                        Me.PFeaturesList.Add(New NameAndID(ListOfs.ListOfExtFeatures.Last.ShortFileName & " e:" _
                                                           & ListOfs.ListOfExtFeatures.Last.Features.Count, ListOfs.NofExtFeatures))
                    End SyncLock
                Case MyConstants.MaintainUniqeSequence
                    Me.TextBox1.Text = "Process Of Maintain Unique Sequences Has Been Finished" & vbCrLf & TextBox1.Text
                    Beep()
                Case MyConstants.ReplaceStringsInFiles
                    Me.TextBox1.Text = "Process Of Replace String in Files Has Been Finished" & vbCrLf & TextBox1.Text
                    Beep()
                Case MyConstants.BackGroundWork.DownLoad
                    Me.TextBox1.Text = e.Result.msg
                Case MyConstants.BackGroundWork.Mapping
                    AddToMappingList(e)
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

#Region "DataFromNewForms"
    Private Function GetCustomFeaturesAndQulifiersSetting() As SettingForSearchInFeaturesAndQulifiers
        Dim f1 As New SelectFeaturesAndQulifiers
        If f1.ShowDialog = DialogResult.OK Then
            Return New SettingForSearchInFeaturesAndQulifiers(f1)
        End If
        Return Nothing
    End Function

    Private Function GetQulifiers() As List(Of String)
        Dim t As New SelectQualifiers(Bio.IO.GenBank.StandardQualifierNames.All.ToList)
        t.ShowDialog()
        If t.DialogResult = DialogResult.OK Then
            Return t.SelectedStrings
        End If
        Return New List(Of String)
    End Function

    Private Function GetIntegerList(Optional Title As String = "Enter Integers separated by space") As List(Of Integer)
        Dim x = InputBox(Title)
        Dim s1() = Split(x, " ")
        Dim out As New List(Of Integer)
        Dim log As New StringBuilder
        For Each s In s1
            Dim res As Integer
            If Int32.TryParse(s, res) = True Then
                out.Add(res)
            Else
                log.Append(s).AppendLine()
            End If
        Next
        If log.Length > 0 Then
            MsgBox("Not integers:" & vbCrLf & log.ToString)
        End If
        Return out
    End Function

#End Region

#Region "ExportFeatureSequences"
    Private Sub WholeToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles WholeToolStripMenuItem.Click
        ExportFeatures(MyConstants.Extension.Whole)
    End Sub

    Private Sub WithPromoterToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles WithPromoterToolStripMenuItem.Click
        ExportFeatures(MyConstants.Extension.FivePrimeExtension)
    End Sub

    Private Sub WithUTRToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles WithUTRToolStripMenuItem.Click
        ExportFeatures(MyConstants.Extension.ThreePrimeExtension)
    End Sub

    Private Sub With53PrimeExtensionToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles With53PrimeExtensionToolStripMenuItem.Click
        ExportFeatures(MyConstants.Extension.FiveAndThreePrimeExtension)
    End Sub

    Private Sub ExportFeatures(t As String)
        ' Get SelectedFeatureLists
        Dim SelectedFeatureLists = GetSelectedFeatureList()
        If IsNothing(SelectedFeatureLists) = True Then Exit Sub
        Dim Qulifiers = GetQulifiers()
        Dim Seqs = Szunyi.Sequences.SequenceManinpulation.MergeSequenceList(Me.GetSelectedSequenceList)
        Dim Out As New List(Of Bio.Sequence)
        Dim x As New Szunyi.Sequences.SequenceManinpulation
        For Each FeatureList In SelectedFeatureLists
            Select Case t
                Case MyConstants.Extension.Whole
                    Out.AddRange(x.GetSequences(FeatureList, Qulifiers, Nothing, Nothing))

                Case MyConstants.Extension.FivePrimeExtension
                    Dim Extension = GetIntegerList("Select 5 prime lengths sepatated by space")
                    Out.AddRange(x.GetSequences(FeatureList, Qulifiers, Extension, Nothing))
                Case MyConstants.Extension.ThreePrimeExtension
                    Dim Extension = GetIntegerList("Select 3 prime lengths sepatated by space")
                    Out.AddRange(x.GetSequences(FeatureList, Qulifiers, Nothing, Extension))
                Case MyConstants.Extension.FiveAndThreePrimeExtension
                    Dim FivePrimes = GetIntegerList("Select  5 prime lengths sepatated by space")
                    Dim ThreePrimes = GetIntegerList("Select  3 prime lengths sepatated by space")
                    Out.AddRange(x.GetSequences(FeatureList, Qulifiers, FivePrimes, ThreePrimes))
            End Select
        Next
        Szunyi.IO.Export.SaveSequencesToSingleFasta(Out)

    End Sub
#End Region

#Region "Import"
    Private Sub ImportSequences()
        Dim Files = Szunyi.IO.Files.SelectFiles("Select Sequence Files", MyConstants.Files.SequenceFilesToImport.SequenceFileTypesToImport)
        Dim t As New Szunyi.ListOf.SequenceList(Files)
        CreateBgWork(MyConstants.BackGroundWork.Sequences, t)

    End Sub
    Private Sub ImportLocations()
        Dim Files As List(Of FileInfo) = Szunyi.IO.Files.SelectFiles("Select Files Contains Location. SeqId,Start,End other")
        Dim t As New Szunyi.ListOf.LocationList(Files)
        CreateBgWork(MyConstants.BackGroundWork.Locations, t)
    End Sub


#End Region

#Region "GetFeatures"
    Private Sub GeneToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles GeneToolStripMenuItem.Click
        GetFeatures(StandardFeatureKeys.Gene)
    End Sub

    Private Sub PromoterToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles PromoterToolStripMenuItem.Click
        GetFeatures(StandardFeatureKeys.Promoter)
    End Sub

    Private Sub CDSToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles CDSToolStripMenuItem.Click
        GetFeatures(StandardFeatureKeys.CodingSequence)
    End Sub

    Private Sub CustomToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles CustomToolStripMenuItem.Click
        GetFeatures()
    End Sub
    Private Sub GeneWithUTRToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles GeneWithUTRToolStripMenuItem.Click
        GetFeatures(StandardFeatureKeys.ThreePrimeUtr)
    End Sub
    Private Sub GetFeatures(Optional Type As String = "")
        Dim SelectedSequences = From x In SelectedItems Where x.Type = MyConstants.BackGroundWork.Sequences Select x.ID

        If SelectedSequences.Count = 0 Then
            MsgBox("There is no any Sequences in the list")
            Exit Sub
        End If
        Dim Seqs = From x In ListOfs.ListOfSequences Where SelectedSequences.Contains(x.UniqueID)
        Dim cSearchSetting As SettingForSearchInFeaturesAndQulifiers
        If Type = "" Then ' It Means Custom
            cSearchSetting = GetCustomFeaturesAndQulifiersSetting()
            If IsNothing(cSearchSetting) = True Then Exit Sub
        Else
            cSearchSetting = New SettingForSearchInFeaturesAndQulifiers(Type)
        End If
        Dim t As New Szunyi.ListOf.ExtFeatureList(cSearchSetting, Seqs.ToList)
        CreateBgWork(MyConstants.BackGroundWork.Features, t)


    End Sub
#End Region

#Region "GetSelectedLists"
    ''' <summary>
    ''' If No Any FeatureList Return Nothing, else a list of ExtFeatureList
    ''' </summary>
    ''' <returns></returns>
    Private Function GetSelectedFeatureList(Optional StartWiths As String = "") As List(Of ExtFeatureList)
        Dim SelectedFeaturesIDs As IEnumerable(Of Integer)
        If StartWiths = "" Then
            SelectedFeaturesIDs = From x1 In SelectedItems Where x1.Type = MyConstants.BackGroundWork.Features Select x1.ID
        Else
            SelectedFeaturesIDs = From x1 In SelectedItems Where x1.Type = MyConstants.BackGroundWork.Features And x1.Name.StartsWith(StartWiths) Select x1.ID

        End If

        If SelectedFeaturesIDs.Count = 0 Then
            MsgBox("There is no any Location in the list")
            Return Nothing
        End If

        Return (From t1 In ListOfs.ListOfExtFeatures Where SelectedFeaturesIDs.Contains(t1.UniqueId)).ToList


    End Function
    ''' <summary>
    ''' If No Any FeatureList Return Nothing, else a list of LocationList
    ''' </summary>
    ''' <returns></returns>
    Private Function GetSelectedLocationList() As List(Of LocationList)
        Dim SelectedFeaturesIDs = From x1 In SelectedItems Where x1.Type = MyConstants.BackGroundWork.Locations Select x1.ID

        If SelectedFeaturesIDs.Count = 0 Then
            MsgBox("There is no any Location in the list")
            Return Nothing
        End If
        Return (From t1 In ListOfs.ListOfLocations Where SelectedFeaturesIDs.Contains(t1.UniqueID)).ToList
    End Function
    ''' <summary>
    ''' If No Any FeatureList Return Nothing, else a list of SequenceList
    ''' </summary>
    ''' <returns></returns>
    Private Function GetSelectedSequenceList() As List(Of SequenceList)
        Dim SelectedFeaturesIDs = From x1 In SelectedItems Where x1.Type = MyConstants.BackGroundWork.Sequences Select x1.ID

        If SelectedFeaturesIDs.Count = 0 Then
            MsgBox("There is no any SequncesList!")
            Return Nothing
        End If
        Return (From t1 In ListOfs.ListOfSequences Where SelectedFeaturesIDs.Contains(t1.UniqueID)).ToList
    End Function
#End Region

#Region "ListChanges"
    Private Sub ListBox1_SelectedIndexChanged(sender As Object, e As EventArgs) Handles ListBox1.DoubleClick
        Dim Index = ListBox1.SelectedIndex
        If Index > -1 Then
            Me.SelectedItems.RemoveAt(Index)
        End If
    End Sub

    Private Sub PSequenceListChanged(ID As Integer) Handles PSequenceList.NewSelection
        Dim cSeq = From t In ListOfs.ListOfSequences Where t.UniqueID = ID

        Dim x As New NameAndIDAndType(cSeq.First.ShortFileName, cSeq.First.UniqueID, MyConstants.BackGroundWork.Sequences)
        SelectedItems.Add(x)

    End Sub
    Private Sub PLocationsChanged(ID As Integer) Handles PLocationsList.NewSelection
        Dim cSeq = From t In ListOfs.ListOfLocations Where t.UniqueID = ID
        Dim x As New NameAndIDAndType(cSeq.First.ShortFileName, cSeq.First.UniqueID, MyConstants.BackGroundWork.Locations)
        SelectedItems.Add(x)

    End Sub

    Private Sub PFeaturesChanged(ID As Integer) Handles PFeaturesList.NewSelection
        Dim cSeq = From t In ListOfs.ListOfExtFeatures Where t.UniqueId = ID
        Dim x As New NameAndIDAndType(cSeq.First.ShortFileName, cSeq.First.UniqueId, MyConstants.BackGroundWork.Features)
        SelectedItems.Add(x)

    End Sub

#End Region
#Region "MenuItems"
    Private Sub ImportSequncesToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles SequncesToolStripMenuItem.Click
        ImportSequences()
    End Sub
    Private Sub BlastFileToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles BlastFileToolStripMenuItem.Click

    End Sub
    Private Sub LocationToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles LocationToolStripMenuItem.Click
        ImportLocations()
    End Sub
    Private Sub AffyProbesFromFastaToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles FromFastaToolStripMenuItem.Click


    End Sub

    Private Sub AffyProbesFromClassicalToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles ClassicalToolStripMenuItem.Click
        Dim File = Szunyi.IO.Files.SelectFile("Select Affy Probes File")
        Dim x As New Szunyi.Other_Database.Affy.AffyProbes(File, 1, 0, 4)
        Dim SelectedSeqs = Me.GetSelectedSequenceList
        Dim Seqs = Szunyi.Sequences.SequenceManinpulation.MergeSequenceList(SelectedSeqs)
        Dim t As New Szunyi.Other_Database.Affy.ParseAffy(Seqs, x)
        CreateBgWork(MyConstants.BackGroundWork.Mapping, t)
    End Sub

    Private Sub AffyProbesFromCustomToolStripMenuItem1_Click(sender As Object, e As EventArgs) Handles CustomToolStripMenuItem1.Click

    End Sub
#End Region
#Region "Locations"
    Private Sub ModifyGenePositionBasedOnLacationAndBamFileToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles ModifyGenePositionBasedOnLacationAndBamFileToolStripMenuItem.Click
        Dim SelectedLocationsID = From x In SelectedItems Where x.Type = MyConstants.BackGroundWork.Locations Select x.ID

        If SelectedLocationsID.Count = 0 Then
            MsgBox("There is no any Location in the list")
            Exit Sub
        End If

        Dim SelectedLocations = From t1 In ListOfs.ListOfLocations Where SelectedLocationsID.Contains(t1.UniqueID)

        Dim SelectedSeqsID = From x1 In SelectedItems Where x1.Type = MyConstants.BackGroundWork.Sequences Select x1.ID

        If SelectedSeqsID.Count = 0 Then
            MsgBox("There is no any Seq in the list")
            Exit Sub
        End If

        Dim SelectedSeqs = From t1 In ListOfs.ListOfExtFeatures Where SelectedSeqsID.Contains(t1.UniqueId)

        Dim FIle = Szunyi.IO.Files.SelectFile("Select Bam File")


        Dim t As New Szunyi.TranscriptDiscovery(SelectedSeqs.ToList, SelectedLocations.ToList, FIle)
        CreateBgWork(MyConstants.TranscriptDiscovery, t)

    End Sub

    Private Sub GetNeighbouroToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles GetNeighbouroToolStripMenuItem.Click
        ' Select Location From Listbox1
        Dim SelectedLocationsID = From x In SelectedItems Where x.Type = MyConstants.BackGroundWork.Locations Select x.ID

        If SelectedLocationsID.Count = 0 Then
            MsgBox("There is no any Location in the list")
            Exit Sub
        End If

        Dim SelectedLocations = From t In ListOfs.ListOfLocations Where SelectedLocationsID.Contains(t.UniqueID)

        Dim SelectedFeaturesIDs = From x1 In SelectedItems Where x1.Type = MyConstants.BackGroundWork.Features Select x1.ID

        If SelectedFeaturesIDs.Count = 0 Then
            MsgBox("There is no any Feature in the list")
            Exit Sub
        End If

        Dim SelectedFeatures = From t1 In ListOfs.ListOfExtFeatures Where SelectedFeaturesIDs.Contains(t1.UniqueId)



        Dim x2 As New FeatureAndLocationMerger(SelectedFeatures.ToList, SelectedLocations.ToList, "Szomszed")
    End Sub

#End Region

#Region "RNASeq"
    Private Sub FromFolderToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles FromFolderToolStripMenuItem.Click
        Dim folder = Szunyi.IO.Files.GetFolder
        Dim t = Szunyi.Text.GetIntegerFromInputbox

        Dim x As New Szunyi.Sequences.MaintainUniqueReads(folder, t)
        CreateBgWork(MyConstants.MaintainUniqeSequence, x)
    End Sub

    Private Sub FromFilesToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles FromFilesToolStripMenuItem.Click
        Dim Files = Szunyi.IO.Files.SelectFiles
        Dim t = Szunyi.Text.GetIntegerFromInputbox

        Dim x As New Szunyi.Sequences.MaintainUniqueReads(Files, t)
        CreateBgWork(MyConstants.MaintainUniqeSequence, x)
    End Sub

    Private Sub StringsInFilesToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles StringsInFilesToolStripMenuItem.Click
        Dim x As New Szunyi.Text.ReplaceStringsInFiles
        CreateBgWork(MyConstants.ReplaceStringsInFiles, x)
    End Sub

#End Region

#Region "Text"
    Private Sub MaintainAllToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles MaintainAllToolStripMenuItem.Click
        ' Select First File
        ' Select Columns
        ' Select Other Files
        ' Select Other columns
        ' Do It as background
        Dim file = Szunyi.IO.Files.SelectFile("Select a File which has unique IDs")
        If IsNothing(file) = True Then Exit Sub


        Dim IDcol = Szunyi.IO.Files.GetIntrestingColumn(file, 1, "Select Id Col")
        Dim OtherCols = Szunyi.IO.Files.GetIntrestingColumns(file, 1, "Select Other Columns")
        Dim AllHeader = Szunyi.IO.Files.GetHeader(file, 1, Nothing, OtherCols)

        Dim file2 = Szunyi.IO.Files.SelectFiles
        If IsNothing(file2) = True Then Exit Sub
        Dim IDcol2 = Szunyi.IO.Files.GetIntrestingColumn(file2.First, 1, "Select Id Col")
        Dim OtherCols2 = Szunyi.IO.Files.GetIntrestingColumns(file2.First, 1, "Select Other Columns")

        Dim log As New StringBuilder

        Dim res = Szunyi.IO.Files.GetValuesToDictionary(file, IDcol, OtherCols, 1)

        For Each file In file2
            'Dim res2 = Szunyi.IO.Files.GetValuesToDictionary(file, IDcol2, OtherCols2, 1)
            Dim out As New StringBuilder
            Using sr As New StreamReader(file.FullName)
                out.Append(sr.ReadLine).Append(vbTab).Append(Szunyi.Text.GetText(AllHeader, vbTab)).AppendLine()
                Do
                    Dim Line As String = sr.ReadLine
                    Dim s1() = Split(Line, vbTab)
                    If res.ContainsKey(s1(IDcol2)) Then
                        out.Append(Line).Append(vbTab).Append(res(s1(IDcol2)).First)
                    Else
                        out.Append(Line)
                    End If
                    out.AppendLine()
                Loop Until sr.EndOfStream = True
            End Using

            Szunyi.IO.Export.SaveText(out.ToString)
        Next




    End Sub

    Private Sub InsertXAndVbtabInTheFirstLineToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles InsertXAndVbtabInTheFirstLineToolStripMenuItem.Click
        Dim s = Szunyi.Text.GetStringFromInputbox("Text to Insert into First Row")
        Dim Files = Szunyi.IO.Files.SelectFiles
        For Each File In Files
            Dim txt = Szunyi.IO.Import.Text.ReadToEnd(File)
            txt = s & vbTab & txt
            Szunyi.IO.Export.SaveText(txt, New FileInfo(File.FullName & ".Md.tab"))

        Next
    End Sub

    Private Sub MaintallAllDuplicatedKeysToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles MaintallAllDuplicatedKeysToolStripMenuItem.Click
        ' Select First File
        ' Select Columns
        ' Select Other Files
        ' Select Other columns
        ' Do It as background
        Dim file = Szunyi.IO.Files.SelectFile
        If IsNothing(file) = True Then Exit Sub


        Dim IDcol = Szunyi.IO.Files.GetIntrestingColumn(file, 1, "Select Id Col")
        Dim OtherCols = Szunyi.IO.Files.GetIntrestingColumns(file, 1, "Select Other Columns")

        Dim file2 = Szunyi.IO.Files.SelectFiles
        Dim IDcol2 = Szunyi.IO.Files.GetIntrestingColumn(file2.First, 1, "Select Id Col")
        Dim OtherCols2 = Szunyi.IO.Files.GetIntrestingColumns(file2.First, 1, "Select Other Columns")

        Dim log As New StringBuilder

        Dim res = Szunyi.IO.Files.GetValuesToDictionary(file, IDcol, OtherCols, 1)

    End Sub

    Private Sub GetOveralappingFeaturesToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles GetOveralappingFeaturesToolStripMenuItem.Click
        ' Get FeatureList By SubTypes
        ' Get Locations 
        ' Find all Locations in in featurelist
        ' Set Count To Zero
        Dim Features = Me.GetSelectedFeatureList

        Dim Locations = Me.GetSelectedLocationList

        Dim FeatureLocation = From x In Locations Where x.SubType = MyConstants.BackGroundWork.Features

        If FeatureLocation.Count = 0 Then
            MsgBox("No Any Feature!")
            Exit Sub
        End If

        Dim LocationLocation = From x1 In Locations Where x1.SubType = MyConstants.BackGroundWork.Locations

        If FeatureLocation.Count = 0 Then
            MsgBox("No Any Feature!")
            Exit Sub
        End If

        Dim x2 As New FeatureAndLocationMerger(Features, Locations, MyConstants.Merge.Count)




    End Sub

    Private Sub BothPositionToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles BothPositionToolStripMenuItem.Click

    End Sub

    Private Sub StartPositionToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles StartPositionToolStripMenuItem.Click

    End Sub

    Private Sub EndPositionToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles EndPositionToolStripMenuItem.Click

    End Sub




    Private Sub AddPromotersAndUTRsToTheFeatureListToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles AddPromotersAndUTRsToTheFeatureListToolStripMenuItem.Click
        Dim SelectedFeaturesLists = Me.GetSelectedFeatureList
        Dim ExtPromoters As List(Of Integer) = Me.GetIntegerList("Enter Integers separated by space for pronoters")
        Dim ExtUTRs As List(Of Integer) = Me.GetIntegerList("Enter Integers separated by space for UTRs")
        ' GetGenes
        Dim AllFeature As New List(Of ExtFeatureList)
        For Each FeatureList In SelectedFeaturesLists
            For Each Feature In FeatureList.Features
                If Feature.Feature.Key = StandardFeatureKeys.CodingSequence Then

                End If
            Next
        Next
    End Sub

    Private Sub ToolStripMenuItem3_Click(sender As Object, e As EventArgs) Handles ToolStripMenuItem3.Click
        Dim File = Szunyi.IO.Files.SelectFile("Select File Contains FileName and LocusTag")
        Dim Header = Szunyi.IO.Files.GetHeader(File, 1, Nothing, Nothing)
        Dim t As New Select_Columns(Header, "Select Columns Contain LocusTags")

        Dim t1 As New Select_Columns(Header, "Select Columns Contain FileNames")

        Dim t2 As New Select_Columns(Header, "Select Columns Contain IDs")

        Dim SelectedLocusTagIndexes As New List(Of Integer)
        Dim SelectedFileNameIndex As New List(Of Integer)
        Dim SelectedIDIndexes As New List(Of Integer)
        If t.ShowDialog = DialogResult.OK Then
            SelectedLocusTagIndexes = t.SelectedIndexes
        End If
        If t1.ShowDialog = DialogResult.OK Then
            SelectedFileNameIndex = t1.SelectedIndexes
        End If
        If t2.ShowDialog = DialogResult.OK Then
            SelectedIDIndexes = t2.SelectedIndexes
        End If
        If SelectedLocusTagIndexes.Count = 0 Or SelectedFileNameIndex.Count = 0 Then Exit Sub


        Dim Fetures = Me.GetSelectedFeatureList

        Dim Folder = Szunyi.IO.Files.GetFolder

        Dim txt = Szunyi.IO.Import.Text.ReadToEnd(File)
        Dim Rows = Split(txt, vbCrLf)
        Dim x As New Szunyi.Sequences.SequenceManinpulation
        Dim CodonCodeScript As New StringBuilder

        Dim ImportSeq As New Szunyi.IO.Import.Sequence(New FileInfo("C:\1.txt"))
        Dim SeqMani As New Szunyi.Sequences.SequenceManinpulation
        For i1 = 1 To Rows.Count - 1
            Dim s1() = Split(Rows(i1), vbTab)
            Dim str As New StringBuilder
            Dim Seqs As New List(Of Bio.Sequence)
            Dim f As New FileInfo(Folder & "\" & s1(15))
            Dim Out As New FileInfo(Folder & "\" & f.Name & "wGenes.fa")
            Seqs.AddRange(ImportSeq.ImportIntoList(f))
            SeqMani.RenameIDsByAscending(Seqs)
            Dim tmp As String = ""
            For Each Item In SelectedIDIndexes
                s1(Item) = s1(Item).Replace("(NCR)", "")
                Dim i = InStr(s1(Item), "NCR")
                If i > 0 Then
                    tmp = tmp & Mid(s1(Item), i, 6)
                End If

            Next
            For Each LocusTagIndex In SelectedLocusTagIndexes
                Dim s = s1(LocusTagIndex)
                '    Seqs.Add(x.GetSequenceByLocusTag(Fetures, s, 100, 100))
                Seqs.Last.ID = (Seqs.Last.ID & tmp).Replace(Chr(34), "")

            Next

            Szunyi.IO.Export.SaveSequencesToSingleFasta(Seqs, Out)
            '	CodonCodeScript.Length = 0
            CodonCodeScript.Append("newProject").AppendLine()
            CodonCodeScript.Append("importSample ").Append(Chr(34)).Append(Out.FullName).Append(Chr(34)).AppendLine()
            CodonCodeScript.Append("selectAllUnassembledSamples").AppendLine()
            CodonCodeScript.Append("assemble").AppendLine()
            CodonCodeScript.Append("saveProjectAs ").Append(Chr(34)).Append(Out.FullName & ".ccap").Append(Chr(34)).AppendLine()
            CodonCodeScript.Append("exportConsensus exportAll=true writeQuals=false gapped=true format=Fasta file= ")
            CodonCodeScript.Append(Chr(34)).Append(Out.FullName.Replace(Out.Name, "") & Seqs.Last.ID & ".fa").Append(Chr(34)).AppendLine()




        Next

    End Sub

    Private Sub MaintainCommonToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles MaintainCommonToolStripMenuItem.Click

    End Sub

    Private Sub StingInFilesToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles StingInFilesToolStripMenuItem.Click
        Dim Original = InputBox("Original String")
        Dim NewString = InputBox("New String")
        Dim FIles = Szunyi.IO.Files.SelectFiles
        For Each FIle In FIles
            Dim txt = Szunyi.IO.Import.Text.ReadToEnd(FIle)
            txt = txt.Replace(Original, NewString)
            Szunyi.IO.Export.SaveText(txt, FIle)
        Next
    End Sub

    Private Sub SelectRowsContainingToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles SelectRowsContainingToolStripMenuItem.Click
        Dim Files = Szunyi.IO.Files.SelectFiles
        Dim txt = InputBox("Enter String")
        For Each File In Files
            Dim str As New StringBuilder
            Using sr As New StreamReader(File.FullName)
                str.Append(sr.ReadLine).AppendLine() ' Header
                Do
                    Dim Line = sr.ReadLine
                    If Line.Contains(txt) Then str.Append(Line).AppendLine()
                Loop Until sr.EndOfStream = True
            End Using
            Dim OutFIle = Szunyi.IO.Files.GetNewFile(File, txt)
            Szunyi.IO.Export.SaveText(str.ToString, OutFIle)
        Next
    End Sub

    Private Sub LocationsToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles LocationsToolStripMenuItem.Click

    End Sub

    Private Sub ExportQualifersToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles QualifersToolStripMenuItem.Click
        Dim SelectedFeaturesLists = Me.GetSelectedFeatureList
        If IsNothing(SelectedFeaturesLists) = True Then Exit Sub
        Dim f As New SelectQualifiers(Bio.IO.GenBank.StandardQualifierNames.All.ToList)
        If f.ShowDialog <> DialogResult.OK Then Exit Sub
        Dim t As New Szunyi.GenBank.ExtFeatureManipulation()
        Dim Header = Szunyi.Text.GetText(f.SelectedStrings, vbTab) & vbCrLf
        For Each FeatureList In SelectedFeaturesLists
            Dim s = Header & t.GetTextFromExtFeatureList(FeatureList, f.SelectedStrings, vbTab, vbCrLf)
            Szunyi.IO.Export.SaveText(s)
        Next

    End Sub

    Private Sub FirstPartToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles FirstPartToolStripMenuItem.Click
        ' Get Kmers
        ' Get Sequence Files
        Dim FIles = Szunyi.IO.Files.SelectFiles
        Dim x As New Szunyi.Sequences.SeqsKmerswCounts(FIles, 50, True, 1000000, MyConstants.OutPutType.AsTabFile, 10000)
        'Dim x As New BackgroundWorker
        x.DoIt()
    End Sub

    Private Sub StringToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles StringToolStripMenuItem.Click
        Dim Original = InputBox("Original String")
        Dim FIles = Szunyi.IO.Files.SelectFiles
        For Each FIle In FIles
            Dim out As New StringBuilder
            Using sr As New StreamReader(FIle.FullName)
                Do
                    Dim Line = sr.ReadLine
                    If Line.IndexOf(Original) > 0 Then out.Append(Line).AppendLine()
                Loop Until sr.EndOfStream = True
            End Using
            If out.Length > 0 Then
                out.Length -= 2
                Szunyi.IO.Export.SaveText(out.ToString)
            End If
        Next
    End Sub

    Private Sub GetTranscriptsToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles GetTranscriptsToolStripMenuItem.Click
        Dim Out As New List(Of Bio.Sequence)
        Dim Genes = Me.GetSelectedFeatureList(StandardFeatureKeys.Gene & " ")
        If IsNothing(Genes) = True Then Exit Sub
        Dim CDss = Me.GetSelectedFeatureList(StandardFeatureKeys.CodingSequence & " ")
        If CDss.Count = 0 Then Exit Sub
        Dim Comp As New Szunyi.Comparares.ExtFeatureLocusTagComparer
        Dim x As New Transcription.TranscriptManipulationSettings

        Dim tmpSeq As New Sequence(Bio.Alphabets.AmbiguousDNA, "")
        Dim TransciptManipulator As New Szunyi.Transcription.TranscriptManipulation(x)
        Dim TransciptList As New List(Of Szunyi.Transcription.Transcript)
        For Each CDSList In CDss
            For Each CDS In CDSList.FetauresByLocustag
                For Each GeneList In Genes
                    Dim sLocusTag = Split(CDS.LocusTag, ".").First
                    Dim GeneID = GeneList.FetauresByLocustag.BinarySearch(New ExtFeature(sLocusTag), Comp)
                    tmpSeq.ID = CDS.SeqID
                    Dim SeqsID = CDSList.Seqs.BinarySearch(tmpSeq, SeqIDComparer)
                    Dim Chromosome As Bio.Sequence = CDSList.Seqs(SeqsID)
                    If GeneID > -1 Then
                        Dim Gene = GeneList.FetauresByLocustag(GeneID)
                        TransciptList.Add(TransciptManipulator.GetTranscript(Gene, CDS, Chromosome))

                    End If ' If GeneID Founded
                Next ' Next GeneList
            Next ' Next CDSList
        Next
        Out = TransciptManipulator.ConvertTranscripts(TransciptList)

        Dim Name As String = "Transcripts Of " & Genes.First.ShortFileName & " e:" & Out.Count
        Dim t1 As New SequenceList(Out, Name)

        Me.PSequenceList.Add(New NameAndID(Name, ListOfs.NofSequenceList))
        Me.AddToSequenceList(t1)
        Szunyi.IO.Export.SaveSequencesToSingleFasta(Out)
    End Sub

    Private Sub SelectSeqsByIDToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles SelectSeqsByIDToolStripMenuItem.Click
        Dim SeqLists = GetSelectedSequenceList()
        If IsNothing(SeqLists) = True Then Exit Sub
        Dim x As New Get_List_of_String("Add Interestring string!")

        Dim InterestingStrings As New List(Of String)
        Dim ForbiddenStrings As New List(Of String)
        If x.ShowDialog = DialogResult.OK Then
            InterestingStrings = x.Strings
        End If
        Dim y As New Get_List_of_String("Select String is Forbidden!")
        If y.ShowDialog = DialogResult.OK Then
            ForbiddenStrings = y.Strings
        End If
        Dim AllSeqs As List(Of Bio.Sequence) = Szunyi.Sequences.SequenceManinpulation.MergeSequenceList(SeqLists)
        Dim tmpSeq As New Bio.Sequence(Alphabets.DNA, "")
        Dim Out As New List(Of Sequence)
        Dim Log As New List(Of String)
        For Each Item In InterestingStrings

            tmpSeq.ID = Item
            Dim i = AllSeqs.BinarySearch(tmpSeq, Comparares.AllComparares.BySeqID)
            If i > -1 Then
                Out.Add(AllSeqs(i))
            Else
                Log.Add(Item)
            End If
        Next
        Dim txt = Szunyi.Text.GetText(Log)
        Dim out2 As New List(Of Bio.Sequence)

        For Each Item In ForbiddenStrings
            Dim t1 = From h1 In Out Where h1.ID.IndexOf(Item) < 0

            If t1.Count <> Out.Count Then Out = t1.ToList
        Next
        Szunyi.IO.Export.SaveSequencesToSingleFasta(Out)

    End Sub

    Private Sub RemoveDuplicatedBySeqToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles RemoveDuplicatedBySeqToolStripMenuItem.Click
        Dim SeqLists As List(Of SequenceList) = GetSelectedSequenceList()
        If IsNothing(SeqLists) = True Then Exit Sub
        Dim t As New RemoveDuplicatedSeqBySequence(SeqLists, ListOfs.ListOfSequences.Count)
        CreateBgWork(MyConstants.BackGroundWork.Sequences, t)
    End Sub

    Private Sub WithCheckToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles WithCheckToolStripMenuItem.Click
        ExportSeqs(True)
    End Sub

    Private Sub WithOutCheckToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles WithOutCheckToolStripMenuItem.Click
        ExportSeqs(False)
    End Sub

    Private Sub ExportSeqs(WithCheck As Boolean)
        Dim SeqLists As List(Of SequenceList) = GetSelectedSequenceList()
        Dim AllSeqs = Szunyi.Sequences.SequenceManinpulation.MergeSequenceList(SeqLists)
        Szunyi.IO.Export.Sequences(AllSeqs, WithCheck)
    End Sub

    Private Sub BlastToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles BlastToolStripMenuItem.Click
        Dim x As New BlastViewer
        x.Show()

    End Sub

    Private Sub ReplaceSeqsInSeqsToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles ReplaceSeqsInSeqsToolStripMenuItem.Click
        Dim Seqs = Me.GetSelectedSequenceList
        If IsNothing(Seqs) = True Then Exit Sub

    End Sub



    Private Sub ToolStripMenuItem4_Click(sender As Object, e As EventArgs) Handles ToolStripMenuItem4.Click
        Dim x As New Szunyi.Transcription.TranscriptManipulationSettings
    End Sub

    Private Sub TranslateToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles TranslateToolStripMenuItem.Click
        Dim SeqLists = GetSelectedSequenceList()
        If IsNothing(SeqLists) = True Then Exit Sub
        Dim Out As New List(Of Bio.Sequence)
        Dim x As New Szunyi.Sequences.Translate
        For Each Seqlist In SeqLists
            For Each Seq In Seqlist.Sequences
                Out.Add(Szunyi.Sequences.Translate.Tranaslate(Seq))
            Next
        Next
        Szunyi.IO.Export.SaveSequencesToSingleFasta(Out)
    End Sub

    Private Sub SignalP1ToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles SignalP1ToolStripMenuItem.Click
        Dim ofd1 As New OpenFileDialog
        ofd1.Title = "Select SignalP File"
        Dim out As New System.Text.StringBuilder
        If ofd1.ShowDialog = System.Windows.Forms.DialogResult.OK Then

            Using sr As New StreamReader(ofd1.FileName)
                Do
                    Dim Line As String = sr.ReadLine
                    If Line.StartsWith("Name=") = True Then
                        Dim s4() = Split(Line, vbTab)
                        Dim Name As String = s4.First.Replace("Name=", "")
                        out.Append(Name).Append(vbTab)
                        If Line.Contains("SP='YES'") = True Then
                            Dim s1() As String = Split(Line, "SP='YES' Cleavage site between pos. ")
                            Dim s2() = Split(s1.Last, ":")
                            Dim s3() = Split(s2.First, " ")
                            out.Append(s3.First)
                        Else
                            out.Append("Not Found")
                        End If
                        out.AppendLine()

                    End If

                Loop Until sr.EndOfStream = True
            End Using
            Szunyi.IO.Export.SaveText(out.ToString)
        End If
    End Sub

    Private Sub SignalP2ToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles SignalP2ToolStripMenuItem.Click
        Dim SeqLists = GetSelectedSequenceList()
        If IsNothing(SeqLists) = True Then Exit Sub
        Dim File = Szunyi.IO.Files.SelectFile
        Dim txt = Szunyi.IO.Import.Text.ReadToEnd(File)
        Dim s1() = Split(txt, vbCrLf)
        Dim Out As New List(Of String)
        Dim str As New System.Text.StringBuilder
        For Each Seqlist In SeqLists

            For i1 = 0 To Seqlist.Sequences.Count - 1
                Dim seq = Seqlist.Sequences(i1)
                Dim AASeq = seq.ConvertToString(0, seq.Count).ToUpper
                If Seqlist.ShortFileName.Contains("NA") Then
                    str.Append(vbTab).Append(seq.ID).Append(vbTab).Append(seq.ConvertToString(0, seq.Count)).AppendLine()
                Else


                    str.Append(s1(i1)).Append(vbTab)
                    str.Append(seq.ID).Append(vbTab)

                    str.Append(AASeq).Append(vbTab)
                    Dim match1 As Match = Regex.Match(AASeq, "C.....C\w*C....C", RegexOptions.IgnoreCase)
                    Dim match2 As Match = Regex.Match(AASeq, "C....C\w*C.....C", RegexOptions.IgnoreCase)
                    If match1.Success = True And match2.Success = True Then
                        str.Append("Both")
                    ElseIf match1.Success = True
                        str.Append("C5C C4C")
                    ElseIf match2.Success = True Then
                        str.Append("C4C C5C")
                    Else
                        str.Append("No Motifs")

                    End If
                    str.Append(vbTab)
                    Dim s2() = Split(s1(i1), vbTab)
                    If s2(1) <> "Not Found" Then
                        Dim x As Bio.Sequence = seq.GetSubSequence(0, s2(1))
                        str.Append(x.ConvertToString(0, x.Count)).Append(vbTab)
                        Dim x1 As Bio.Sequence = seq.GetSubSequence(s2(1), seq.Count - s2(1))
                        str.Append(x1.ConvertToString(0, x1.Count)).Append(vbTab)
                        str.Append(Szunyi.Text.GetNofAA("C", x1)).AppendLine()
                    Else
                        str.Append(vbTab).Append(vbTab).Append(Szunyi.Text.GetNofAA("C", seq)).AppendLine()
                    End If
                End If
            Next

        Next
    End Sub

    Private Sub Get120BpToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles Get120BpToolStripMenuItem.Click
        Dim SeqLists = GetSelectedSequenceList()
        If IsNothing(SeqLists) = True Then Exit Sub
        Dim str As New System.Text.StringBuilder
        Dim str2 As New System.Text.StringBuilder
        str2.Append("TargetID	ProbeID	Sequence	Replication").AppendLine() 'LocusTag, Ascending, Sequence, 1
        Dim UsedSeqs As New List(Of String)
        Dim Out As New List(Of Bio.Sequence)
        Dim i2 As Integer = 0
        For Each Seqlist In SeqLists

            For Each Seq In Seqlist.Sequences
                Dim MdSeqID = Seq.ID.Replace(vbTab, " ")

                Dim s = Seq.ConvertToString(0, Seq.Count)
                Dim s1() = Split(Seq.ID, "|")
                Dim pLength As Integer = s1(2)
                pLength += s1(3)
                Dim cdsLength As Integer = s1(2)
                cdsLength += s1(3)
                cdsLength += s1(4)
                str.Append(Seq.ID).Append(vbTab) ' 
                Dim StartPositions As New List(Of Integer)
                StartPositions.Add(pLength - 60)
                StartPositions.Add(cdsLength - 120)
                StartPositions.Add(cdsLength)
                StartPositions.Add(pLength)
                For i1 = 0 To StartPositions.Count - 1
                    ' str2.Append(Seq.ID).Append(vbTab)
                    Dim newseq As New Bio.Sequence(Alphabets.AmbiguousDNA, "")
                    GetAgilentSureSelectForm(s, str2, i1, StartPositions(i1), UsedSeqs, Seq.ID, newseq)
                    If newseq.Count <> 0 Then
                        newseq.ID = Split(Seq.ID, "|").First & "_" & i1
                        Out.Add(newseq)
                    End If
                Next

                'str.Append(GetSubString(s, pLength - 120))
                str.Append(GetSubString(s, pLength - 60))
                str.Append(GetSubString(s, pLength))
                str.Append(GetSubString(s, cdsLength - 120))
                str.Append(GetSubString(s, cdsLength - 60))
                str.Append(GetSubString(s, cdsLength))
                str.Append(GetSubString(s, cdsLength + 120))
                str.AppendLine()

            Next
            Szunyi.IO.Export.SaveSequencesToSingleFasta(Out)
        Next
    End Sub



    Private Sub GetAgilentSureSelectForm(s As String, str2 As StringBuilder,
                                         i1 As Integer, Start As Integer, UsedSeq As List(Of String), SeqID As String, ByRef NewSeq As Bio.Sequence)
        Dim t = GetSubString(s, Start).ToUpper
        If UsedSeq.Contains(t) = False Then
            UsedSeq.Add(t)
            If t.Contains("N") = False AndAlso t.Count = 120 Then
                Dim x As New Bio.Sequence(Alphabets.AmbiguousDNA, t)
                NewSeq = x
                str2.Append(Split(SeqID, "|").First & "_" & i1).Append(vbTab)
                str2.Append(i1).Append(vbTab)
                str2.Append(t).Append(vbTab).Append("1").AppendLine()
            End If
        End If

    End Sub

    Private Function GetSubString(s As String, start As Integer) As String
        Try
            Return s.Substring(start, 120).ToUpper
        Catch ex As Exception
            Return ""
        End Try
    End Function

    Private Sub SelectRowsTestToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles SelectRowsTestToolStripMenuItem.Click
        Dim File As FileInfo = Szunyi.IO.Files.SelectFile
        Dim x As New SelectFirstRowAndColumn_s_(File, MyConstants.DelimitedFileImport.SelectFirstRowAndColumns)
        x.ShowDialog()
    End Sub

    Private Sub Gff3AndFastaToGenBankToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles Gff3AndFastaToGenBankToolStripMenuItem.Click
        Dim SeqLists = Me.GetSelectedSequenceList
        If IsNothing(SeqLists) = True Then Exit Sub
        Dim MergedSeqlist = Szunyi.Sequences.SequenceManinpulation.MergeSequenceList(SeqLists)
        Dim SeqListsNames = From x In SeqLists Select x.ShortFileName


        Dim MergedSeqListName As String = Szunyi.Text.GetText(SeqListsNames.ToList, " ")
        Dim Gff3Files = Szunyi.IO.Files.SelectFiles("Select gff file", MyConstants.Files.SequenceFilesToImport.gff3)
        If IsNothing(Gff3Files) = True Then Exit Sub
        Dim t As New Szunyi.Sequences.Gff.GffParser(MergedSeqlist, Gff3Files, MergedSeqListName)
        CreateBgWork(MyConstants.BackGroundWork.Gff3Parser, t)

    End Sub

    Private Sub InsertFeatureIntoSeqFromClipboardToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles InsertFeatureIntoSeqFromClipboardToolStripMenuItem.Click
        Dim SelSeqLists = Me.GetSelectedSequenceList
        Dim Seq = Szunyi.Sequences.SequenceManinpulation.MergeSequenceList(SelSeqLists)
        Dim s As New TextImport
        If s.ShowDialog = DialogResult.OK Then
            Dim s1() As String = Split(s.Text, vbCrLf)
            Dim Lines As New List(Of String)
            For i1 = 0 To s1.Count - 1
                If s1(i1) <> "" Then
                    Lines.Add(s1(i1))
                Else
                    iNSERTIT(Lines, Seq)
                    Lines.Clear()
                End If
            Next
        End If
        Szunyi.IO.Export.Sequences(Seq, True)
    End Sub
    Private Sub iNSERTIT(lINES As List(Of String), SEQS As List(Of Sequence))
        Dim pts As New List(Of Point)
        Dim compl As Boolean
        Dim integers As New List(Of Long)
        Dim s1() As String

        For Each line In lINES
            s1 = Split(line, vbTab)
            If s1(1) = "-" Then
                compl = True
            Else
                compl = False
            End If
            Dim i2 As Integer = s1(2)
            Dim i3 As Integer = s1(3)
            If i2 < i3 Then
                pts.Add(New Point(i2, i3))
            Else
                pts.Add(New Point(i3, i2))
            End If

        Next
        Dim h = From x4 In pts Select x4 Order By x4.X
        Dim str As New StringBuilder
        For Each v In h
            str.Append(v.X & ".." & v.Y & ",")
            integers.Add(v.X)
            integers.Add(v.Y)
        Next
        str.Length -= 1
        integers.Sort()

        Dim loc = GetLocation(str.ToString, compl)
        Dim x As New CodingSequence(loc)
        Dim products As New List(Of String)
        products.Add("Nodule Cysteine-Rich (NCR) secreted peptide")
        x.Qualifiers(StandardQualifierNames.Product) = products
        x.LocusTag.Add(s1(4))
        Dim tmpseq As New Bio.Sequence(Alphabets.AmbiguousDNA, "")
        tmpseq.ID = s1(0)
        Dim cSeq = SEQS.BinarySearch(tmpseq, New Comparares.SequenceIDComparer)
        '  Dim x1 As New Gene(integers.First & ".." & integers.Last)
        Dim x1 As New Gene("7320651..7323571")
        x1.LocusTag.Add(s1(4))
        ' Dim x2 As New MessengerRna(integers.First & ".." & integers.Last)
        Dim x2 As New MessengerRna("join(7320651..7321189,7323093..7323571)")
        x2.LocusTag.Add(s1(4))


        SEQS(cSeq).Metadata(Bio.Util.Helper.GenBankMetadataKey).Features.All.Add(x1)
        SEQS(cSeq).Metadata(Bio.Util.Helper.GenBankMetadataKey).Features.All.Add(x2)
        SEQS(cSeq).Metadata(Bio.Util.Helper.GenBankMetadataKey).Features.All.Add(x)
    End Sub
    Private Function GetLocation(p1 As String, comp As Boolean) As Bio.IO.GenBank.Location
        Try
            Dim locibuilder As New LocationBuilder
            If Split(p1, "..").Count > 2 Then p1 = "join(" & p1 & ")"
            If comp = True Then p1 = "complement(" & p1 & ")"
            Return locibuilder.GetLocation(p1)
        Catch ex As Exception
            Return Nothing
        End Try

    End Function

    Private Sub OligoFastaToAgilentToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles OligoFastaToAgilentToolStripMenuItem.Click
        Dim SelSequenceList = Me.GetSelectedSequenceList
        Dim Seqs = Szunyi.Sequences.SequenceManinpulation.MergeSequenceList(SelSequenceList)
        Dim str As New System.Text.StringBuilder
        Dim i1 As Integer = 0
        For Each Seq In Seqs

            str.Append(Seq.ID).Append(vbTab)
            str.Append(Seq.ConvertToString(0, Seq.Count)).Append(vbTab)
            str.AppendLine()
        Next
        Szunyi.IO.Export.SaveText(str.ToString)
    End Sub

    Private Sub Merge2GenBankAnnotationsToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles Merge2GenBankAnnotationsToolStripMenuItem.Click
        Dim seqLists = Me.GetSelectedSequenceList
        Dim x As New Szunyi.Sequences.MergeSequenceAnnotations(seqLists)
        x.DoIt()
    End Sub

    Private Sub CopyFeatureAnnotationsIntoAnotherAnnotationsToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles CopyFeatureAnnotationsIntoAnotherAnnotationsToolStripMenuItem.Click
        Dim SeqLists = Me.GetSelectedSequenceList

        Dim x1 As New SelectQualifiers(Bio.IO.GenBank.StandardFeatureKeys.All.ToList, "Select Feature To")
        Dim FeaturesTo As New List(Of String)
        If x1.ShowDialog = DialogResult.OK Then
            FeaturesTo = x1.SelectedStrings
        End If

        Dim x2 As New SelectQualifiers(Bio.IO.GenBank.StandardFeatureKeys.All.ToList, "Select Feature To")
        Dim FeaturesFrom As New List(Of String)
        If x2.ShowDialog = DialogResult.OK Then
            FeaturesFrom = x2.SelectedStrings
        End If
        Dim t As New Szunyi.GenBank.FeatureManipulation.MergeFeatureAnnotation(SeqLists, FeaturesTo, FeaturesFrom)
        t.DoIt()
    End Sub
#Region "Rename"
#Region "SeqID"
    Private Sub PrefixAscendingToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles PrefixAscendingToolStripMenuItem.Click
        Dim SeqLists As List(Of SequenceList) = GetSelectedSequenceList()
        If IsNothing(SeqLists) = True Then Exit Sub
        Dim s = InputBox("Prefix")
        Dim t As New RenameSequenceIDs(SeqLists, ListOfs.ListOfSequences.Count, s)
        CreateBgWork(MyConstants.BackGroundWork.Sequences, t)
    End Sub

    Private Sub FisrtPartOfToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles FisrtPartOfToolStripMenuItem.Click
        Dim SeqLists As List(Of SequenceList) = GetSelectedSequenceList()
        If IsNothing(SeqLists) = True Then Exit Sub
        Dim Separator = InputBox("Enter the Separator")
        If Separator.Length = 0 Then Exit Sub
        Dim t As New Szunyi.Sequences.RenameSequenceIDs(SeqLists, MyConstants.StringRename.FirstAfterSplit, Separator)
        CreateBgWork(MyConstants.BackGroundWork.ModyfiedSequence, t)
    End Sub

    Private Sub LastPartOfAfterSplittingToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles LastPartOfAfterSplittingToolStripMenuItem.Click
        Dim SeqLists As List(Of SequenceList) = GetSelectedSequenceList()
        If IsNothing(SeqLists) = True Then Exit Sub
        Dim Separator = InputBox("Enter the Separator")
        If Separator.Length = 0 Then Exit Sub
        Dim t As New Szunyi.Sequences.RenameSequenceIDs(SeqLists, MyConstants.StringRename.LastAfterSplit, Separator)
        CreateBgWork(MyConstants.BackGroundWork.ModyfiedSequence, t)
    End Sub



    Private Sub BToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles BToolStripMenuItem.Click

    End Sub

    Private Sub RenameFeaturesToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles RenameFeaturesToolStripMenuItem.Click

    End Sub
#End Region
#Region "ReNameFeatures"
    Private Sub FirstPartAfterSplittinToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles FirstPartAfterSplittinToolStripMenuItem.Click
        RenameFeature(MyConstants.StringRename.FirstAfterSplit)
    End Sub

    Private Sub LastPartAfterSplittingToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles LastPartAfterSplittingToolStripMenuItem.Click
        RenameFeature(MyConstants.StringRename.FirstAfterSplit)
    End Sub

    Private Sub RenameFeature(Subtype As String)
        Dim Seqs = Me.GetSelectedSequenceList
        Dim MergedSeqs = Szunyi.Sequences.SequenceManinpulation.MergeSequenceList(Seqs)
        Dim Separator = InputBox("Select Separator")
        If Separator.Length = 0 Then Exit Sub
        Dim f1 As New SelectFeaturesAndQulifiers
        If f1.ShowDialog = DialogResult.OK Then
            Dim alf As Int16 = 54
            Dim x As New Szunyi.GenBank.FeatureManipulation.Rename(MergedSeqs,
                                                                    f1.SelectedFeatures,
                                                                    f1.SelectedQualifiers,
                                                                    Separator,
                                                                    MyConstants.StringRename.LastAfterSplit,
                                                                    Seqs.Last.ShortFileName)



            CreateBgWork(MyConstants.BackGroundWork.ModyfiedSequence, x)

        End If
    End Sub

    Private Sub ExportSeqIDsToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles ExportSeqIDsToolStripMenuItem.Click
        Dim SeqLists = Me.GetSelectedSequenceList
        Dim Seqs = Szunyi.Sequences.SequenceManinpulation.MergeSequenceList(SeqLists)
        Dim SeqIDs = From x In Seqs Select x.ID

        If SeqIDs.Count > 0 Then
            Dim out = Szunyi.Text.GetText(SeqIDs.ToList)
            Szunyi.IO.Export.SaveText(out)
        End If
    End Sub

    Private Sub TDTToFastaToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles TDTToFastaToolStripMenuItem.Click
        Dim File = Szunyi.IO.Files.SelectFile
        Using sr As New StreamReader(File.FullName)
            sr.ReadLine()
            Dim out As New List(Of Bio.Sequence)
            Do
                Dim s1() = Split(sr.ReadLine, vbTab)
                Dim n As New Bio.Sequence(Alphabets.AmbiguousDNA, s1(1))
                n.ID = s1(0)
                out.Add(n)
            Loop Until sr.EndOfStream = True
            Szunyi.IO.Export.SaveSequencesToSingleFasta(out)
        End Using

    End Sub

    Private Sub CreateGenBankFromMRNAsAndCDSsToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles CreateGenBankFromMRNAsAndCDSsToolStripMenuItem.Click
        Dim SelectedSeqLists = Me.GetSelectedSequenceList
        Dim Seqs = Szunyi.Sequences.SequenceManinpulation.MergeSequenceList(SelectedSeqLists)
        Dim X As List(Of Bio.Sequence) = Szunyi.Sequences.SequenceManinpulation.CreateGenBankFrom_mRNA_And_CDS(Seqs)

    End Sub

    Private Sub DownLoadKeggToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles DownLoadKeggToolStripMenuItem.Click

        Dim x As New Szunyi.Other_Database.Kegg.KeggDownloader()
        CreateBgWork("", x)

    End Sub

    Private Sub FromFullDatabaseToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles FromFullDatabaseToolStripMenuItem.Click
        Dim x As New Szunyi.Other_Database.BioSytem.BioSystemBuilder
        x.CreateSubFromFullDatabase()
    End Sub






#End Region

#End Region

#End Region
End Class

Public Class ListOfs
    Public Shared Property ListOfSequences As New List(Of SequenceList)
    Public Shared Property ListOfLocations As New List(Of LocationList)
    Public Shared Property ListOfExtFeatures As New List(Of ExtFeatureList)
    Public Shared Property ListOfMappings As New List(Of Mapping)

    Public Shared Property NofSequenceList As Integer
    Public Shared Property NofLocations As Integer
    Public Shared Property NofExtFeatures As Integer
    Public Shared Property NofMapping As Integer
End Class
Public Class Mapping
    Public Property ShortFileName As String
    Public Property UniqueID As Integer
    Public Property Type As String = MyConstants.BackGroundWork.Mapping
    Public Property SubType As String = MyConstants.BackGroundWork.AffyMapping
    Public Property O As Object
    Public Sub New(x As Object)
        O = x
    End Sub
End Class
Public Class FeatureAndLocationMerger
    Private SortedFeatureLists As List(Of ExtFeatureList)
    Private LocationLists As List(Of LocationList)
    Private LocationListOfFeatures As New List(Of LocationList)
    Public ReadOnly Property Type As String = MyConstants.MergeLocationsAndFeatures
    Private MergeAllFile As Boolean = False
    Private ToSave As FileInfo
    Private ToSaveFolder As String
    Public Property SubType As String
    Public LocationComparer As New Szunyi.Comparares.LocationComparer
    Public Property SubSubType As String

    Public Sub New(FeaturesLIsts As List(Of ExtFeatureList), LocationLists As List(Of LocationList), SubType As String)
        Me.SortedFeatureLists = FeaturesLIsts
        Me.LocationLists = LocationLists
        For Each SortedFeature In Me.SortedFeatureLists
            Me.LocationListOfFeatures.Add(New LocationList(SortedFeature))
        Next

        Me.SubType = Type
        If Me.SortedFeatureLists.Count > 1 Or Me.LocationLists.Count > 1 Then
            Dim res = MsgBox("Merge All File to Single Output?", MsgBoxStyle.YesNo)
            Me.MergeAllFile = MsgBox("Merge All File to Single Output?", MsgBoxStyle.YesNo)
            If Me.MergeAllFile = True Then
                Me.ToSave = Szunyi.IO.Files.SelectSaveFile(MyConstants.Files.SequenceFilesToSave.FastaSingle)
            Else
                Me.ToSaveFolder = Szunyi.IO.Files.GetFolder
            End If
        End If
        'DoIt()
    End Sub
    Public Sub DoIt()
        Select Case SubType
            Case MyConstants.Merge.Neighbor
                Neighbor()
            Case MyConstants.Merge.Count
                Counts()
        End Select
    End Sub
#Region "Neighbor"
    Private Sub Neighbor()
        Dim Quali As New SelectQualifiers(Bio.IO.GenBank.StandardQualifierNames.All.ToList)
        Dim h = Quali.ShowDialog
        Dim out As New StringBuilder
        If Me.MergeAllFile = True Then
            For Each Locis In Me.LocationLists
                For Each Loci In Locis.Locations
                    For Each Feats In Me.LocationListOfFeatures
                        Dim t = getSZomszed(Loci, Feats)

                    Next
                Next
            Next
        Else
            For Each Locis In Me.LocationLists
                For Each Loci In Locis.Locations
                    For Each Feats In Me.LocationListOfFeatures

                        Dim t = getSZomszed(Loci, Feats)

                        '	out.Append(p.GetText(t, Quali.SelectedQualifiers, vbTab))

                        Dim h1() As String = Loci.Extra
                        Dim s As List(Of String) = h1.ToList

                        out.Append(Szunyi.Text.GetText(s, vbTab))
                        out.AppendLine()
                    Next
                Next
            Next
        End If
        Szunyi.IO.Export.SaveText(out.ToString)

    End Sub

    Private Function getSZomszed(loci As Szunyi.Location, Feats As LocationList) As Szunyi.ExtFeature()
        Dim Index = Feats.Locations.BinarySearch(loci, New Szunyi.Comparares.LocationComparer())
        Dim s(1) As Szunyi.ExtFeature

        s(0) = Feats.Locations(Math.Abs(Index) - 2).Extra
        s(1) = Feats.Locations(Math.Abs(Index) - 1).Extra
        Return s

    End Function
#End Region
#Region "Count"
    Private Sub Counts()
        If IsNothing(Me.ToSaveFolder) = False Then
            For Each FeatList In Me.LocationListOfFeatures
                SetCountZero(FeatList)
                For Each LocationList In Me.LocationLists
                    For Each Loci In LocationList.Locations
                        Dim t = GetAllItems(FeatList, Loci)
                    Next
                Next
            Next
        End If
    End Sub
    Private Sub SetCountZero(ByRef FeatList As LocationList)
        For Each Feat In FeatList.Locations
            Feat.Count = 0
        Next
    End Sub
    Public Function GetAllItems(List As LocationList, loci As Szunyi.Location) As List(Of Szunyi.Location)


        Dim tempy As New List(Of Szunyi.Location)
        Dim Index = List.Locations.BinarySearch(loci, Me.LocationComparer)

        If Index > -1 Then
            For i1 = List.Locations(Index).FirstItemToInvestigate To List.Locations(Index).LastItemToInvestigate
                If List.Locations(i1).Start <= loci.Start AndAlso List.Locations(i1).Endy >= loci.Endy Then
                    tempy.Add(List.Locations(i1))
                End If
            Next
            Return tempy
        ElseIf Index = -1 Then
            Return New List(Of Szunyi.Location)
        Else
            Index = Math.Abs(Index)
            For i1 = List.Locations(Index - 2).FirstItemToInvestigate To List.Locations(Index - 2).LastItemToInvestigate
                If List.Locations(i1).Start <= loci.Start AndAlso List.Locations(i1).Endy >= loci.Endy Then
                    tempy.Add(List.Locations(i1))
                End If
            Next
            Return tempy
        End If
    End Function
#End Region
End Class



