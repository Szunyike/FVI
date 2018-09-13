Imports System.ComponentModel
Imports System.IO
Imports System.Text
Imports System.Text.RegularExpressions
Imports Bio

Imports Bio.IO.GenBank
Imports ClassLibrary1.Szunyi.ListOf

Imports ClassLibrary1.Szunyi.Comparares
Imports ClassLibrary1.Szunyi
Imports ClassLibrary1
Imports ClassLibrary1.Szunyi.Sequences
Imports ClassLibrary1.Szunyi.Other_Database.CrossRefs
Imports System.Runtime.Serialization
Imports System.Runtime.Serialization.Formatters.Binary
Imports ClassLibrary1.Szunyi.Features.FeatureManipulation
Imports ClassLibrary1.Szunyi.Sequences.SequenceManipulation
Imports System.Globalization
Imports System.Threading
Imports System.Threading.Tasks

Imports ClassLibrary1.Szunyi.Constants
Imports ClassLibrary1.Szunyi.Blast
Imports ClassLibrary1.Szunyi.Features
Imports ClassLibrary1.Szunyi.Basic
Imports ClassLibrary1.Szunyi.Text.TableManipulation
Imports ClassLibrary1.Szunyi.BAM



Public Class Form1
    Public Property Helper As New ListOfs
    Public SelectedItems As New BindingList(Of Name_ID_Type_Title_Object)
    Dim QulifierSelector As New CheckBoxForStringsFull(Bio.IO.GenBank.StandardQualifierNames.All.ToList, -1)

    Dim LocationBuilder As New LocationBuilder


    Dim WithEvents PSequenceList As New ListManager(Constants.BackGroundWork.Sequences)
    Dim WithEvents PLocationsList As New ListManager(Szunyi.Constants.BackGroundWork.Locations)
    Dim WithEvents PFeaturesList As New ListManager(Szunyi.Constants.BackGroundWork.Features)
    Dim WithEvents PMappingList As New ListManager(Szunyi.Constants.BackGroundWork.Mapping)
    Dim WithEvents PCountsList As New ListManager(Szunyi.Constants.BackGroundWork.Counts)
    Dim WithEvents P_Item_With_Properties_List As New ListManager(Szunyi.Constants.BackGroundWork.Items_With_Properties)
    Dim WithEvents P_Sequences_With_Motifs As New ListManager(Szunyi.Constants.BackGroundWork.Sequences_With_Motifs)



    Private Sub Form1_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        Thread.CurrentThread.CurrentCulture = CultureInfo.CreateSpecificCulture("en-US")
        Thread.CurrentThread.CurrentUICulture = CultureInfo.CreateSpecificCulture("en-US")
        ListBox1.DataSource = SelectedItems
        ListBox1.DisplayMember = "Name"

        Me.FlowLayoutPanel1.Controls.Add(PSequenceList)
        Me.FlowLayoutPanel1.Controls.Add(PLocationsList)
        Me.FlowLayoutPanel1.Controls.Add(PFeaturesList)
        Me.FlowLayoutPanel1.Controls.Add(PMappingList)
        Me.FlowLayoutPanel1.Controls.Add(PCountsList)
        Me.FlowLayoutPanel1.Controls.Add(P_Item_With_Properties_List)
        Me.FlowLayoutPanel1.Controls.Add(P_Sequences_With_Motifs)
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

    Private Sub WorkerProgressChanged(sender As Object, e As ProgressChangedEventArgs)
        ' Throw New NotImplementedException()
    End Sub

    Private Sub AddToSequences_With_Motifs(e As Szunyi.Sequence_Analysis.Sequences_Ranges)
        If IsNothing(e) = True Then Exit Sub
        SyncLock ListOfs.ListOf_Sequences_With_Motifs
            ListOfs.ListOf_Sequences_With_Motifs.Add(e)
            ListOfs.ListOf_Sequences_With_Motifs.Last.UniqueID = ListOfs.Nof_Sequences_With_Motifs
            Me.P_Sequences_With_Motifs.Add(New Name_ID_Type_Title_Object(ListOfs.ListOf_Sequences_With_Motifs.Last.Title & " e:" &
                                          ListOfs.ListOf_Sequences_With_Motifs.Last.Seqs.Count,
                                          ListOfs.Nof_Sequences_With_Motifs,
                                          Constants.BackGroundWork.Sequences_With_Motifs,
                                          ListOfs.ListOf_Sequences_With_Motifs.Last.Title,
                                                     ListOfs.ListOfSequences.Last))

            ListOfs.Nof_Sequences_With_Motifs += 1
            SelectedItems.Add(P_Sequences_With_Motifs.NamesAndIDs.Last)
        End SyncLock
    End Sub
    Private Sub AddToSequenceList(e As SequenceList)
        If IsNothing(e) = True Then Exit Sub
        SyncLock ListOfs.ListOfSequences

            ListOfs.ListOfSequences.Add(e)
            ListOfs.ListOfSequences.Last.UniqueID = ListOfs.NofSequenceList
            Me.PSequenceList.Add(New Name_ID_Type_Title_Object(ListOfs.ListOfSequences.Last.ShortFileName & " e:" &
                                           ListOfs.ListOfSequences.Last.Sequences.Count,
                                           ListOfs.NofSequenceList,
                                           Constants.BackGroundWork.Sequences,
                                           ListOfs.ListOfSequences.Last.ShortFileName,
                                                      ListOfs.ListOfSequences.Last))

            ListOfs.NofSequenceList += 1
            SelectedItems.Add(PSequenceList.NamesAndIDs.Last)
        End SyncLock
    End Sub
    Private Sub AddToMappingList(e As Object)
        If IsNothing(e) = True Then Exit Sub
        SyncLock ListOfs.ListOfMappings

            ListOfs.ListOfMappings.Add(e)
            ListOfs.ListOfMappings.Last.UniqueID = ListOfs.NofMapping
            Me.PMappingList.Add(New Name_ID_Type_Title_Object(ListOfs.ListOfMappings.Last.ShortFileName,
                                                     ListOfs.NofMapping,
                                                    Constants.BackGroundWork.Mapping,
                                                     ListOfs.ListOfMappings.Last.ShortFileName,
                                                      ListOfs.ListOfMappings.Last))
            ListOfs.NofMapping += 1
            SelectedItems.Add(PMappingList.NamesAndIDs.Last)
        End SyncLock
    End Sub
    Private Sub AddToExtFeatureList(e As ListOf.ExtFeatureList)
        If IsNothing(e) = True Then Exit Sub
        SyncLock ListOfs.ListOfExtFeatures
            ListOfs.NofExtFeatures += 1
            Dim t As ExtFeatureList = e
            t.UniqueId = ListOfs.NofExtFeatures
            ListOfs.ListOfExtFeatures.Add(t)

            Me.PFeaturesList.Add(New Name_ID_Type_Title_Object(ListOfs.ListOfExtFeatures.Last.ShortFileName & " e:" _
                                                           & ListOfs.ListOfExtFeatures.Last.Features.Count,
                                                       ListOfs.NofExtFeatures,
                                                       Constants.BackGroundWork.Features,
                                                      ListOfs.ListOfExtFeatures.Last.ShortFileName,
                                                      ListOfs.ListOfExtFeatures.Last))
            SelectedItems.Add(PFeaturesList.NamesAndIDs.Last)
        End SyncLock
    End Sub
    Private Sub AddToLocations(e As Szunyi.Location.LocationList)
        SyncLock ListOfs.ListOfLocations
            ListOfs.NofLocations += 1
            Dim t As Szunyi.Location.LocationList = e
            t.UniqueID = ListOfs.NofLocations


            ListOfs.ListOfLocations.Add(t)

            Me.PLocationsList.Add(New Name_ID_Type_Title_Object(t.Title,
                                                        ListOfs.NofLocations,
                                                        Constants.BackGroundWork.Locations,
                                                      t.Title,
                                                        ListOfs.ListOfLocations.Last))

            SelectedItems.Add(PLocationsList.NamesAndIDs.Last)
        End SyncLock
    End Sub
    Private Sub AddTo_Item_With_Properties(x As Object)
        SyncLock ListOfs.ListOf_Item_With_Properties
            ListOfs.Nof_Item_With_Properties += 1
            Dim t As Szunyi.Text.TableManipulation.Items_With_Properties = x
            t.UniqueID = ListOfs.Nof_Item_With_Properties

            ListOfs.ListOf_Item_With_Properties.Add(t)

            Me.P_Item_With_Properties_List.Add(New Name_ID_Type_Title_Object(ListOfs.ListOf_Item_With_Properties.Last.Title,
                                                        ListOfs.Nof_Item_With_Properties,
                                                       Constants.BackGroundWork.Items_With_Properties,
                                                       ListOfs.ListOf_Item_With_Properties.Last.Title,
                                                                     ListOfs.ListOf_Item_With_Properties.Last))
        End SyncLock
    End Sub
    Private Sub WorkerCompleted(sender As Object, e As RunWorkerCompletedEventArgs)
        Try


            Select Case e.Result.Type
                Case Szunyi.Constants.BackGroundWork.Gff3Parser
                    AddToSequenceList(e.Result.Result)
                Case Szunyi.Constants.BackGroundWork.ModyfiedSequence
                    AddToSequenceList(e.Result.seqList)
                Case Szunyi.Constants.BackGroundWork.Sequences
                    AddToSequenceList(e.Result)
                Case Szunyi.Constants.BackGroundWork.Locations
                    AddToLocations(e.Result.result)

                Case Szunyi.Constants.BackGroundWork.Features
                    AddToExtFeatureList(e.Result)
                Case Szunyi.Constants.BackGroundWork.MaintainUniqeSequence
                    Me.TextBox1.Text = "Process Of Maintain Unique Sequences Has Been Finished" & vbCrLf & TextBox1.Text
                    Beep()
                Case Szunyi.Constants.BackGroundWork.ReplaceStringsInFiles
                    Me.TextBox1.Text = "Process Of Replace String in Files Has Been Finished" & vbCrLf & TextBox1.Text
                    Beep()
                Case Szunyi.Constants.BackGroundWork.DownLoad
                    Me.TextBox1.Text = e.Result.msg
                Case Szunyi.Constants.BackGroundWork.Mapping
                    AddToMappingList(e.Result)


                Case Szunyi.Constants.BackGroundWork.CDS_For_Exon_Intron
                    AddToExtFeatureList(e.Result)
                    Get_Exon_Intron_Promotor_UTR(e.Result)
                Case Szunyi.Constants.BackGroundWork.Items_With_Properties
                    AddTo_Item_With_Properties(e.Result)
            End Select
        Catch ex As Exception
            Dim alf As Int16 = 54
        End Try
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
    Private Function GetCustomFeaturesAndQulifiersSetting(Optional DiffFeatureKeys As List(Of String) = Nothing) As SettingForSearchInFeaturesAndQulifiers
        Dim f1 As New TRYIII(DiffFeatureKeys)
        If f1.ShowDialog = DialogResult.OK Then
            Dim x As New SettingForSearchInFeaturesAndQulifiers(DiffFeatureKeys)
            Dim SettingForSearchInQulifier As New List(Of SettingForSearchInQulifier)
            x.SelectedFeatures = f1.SelectedFeatures

            For Each Control In f1.FLP.Controls
                Dim t As SetSearch = Control
                Dim h As New SettingForSearchInQulifier(t.QulifierName, t.Type, t.InterestingStrings)

                SettingForSearchInQulifier.Add(h)
            Next
            x.SettingForSearchInQulifier = SettingForSearchInQulifier

            Return x
        End If
        Return Nothing
    End Function

    Private Function GetQulifiers(Optional DiffFeatures As List(Of String) = Nothing) As List(Of String)
        If IsNothing(DiffFeatures) = True Then DiffFeatures = Bio.IO.GenBank.StandardQualifierNames.All.ToList
        Dim t As New CheckBoxForStringsFull(Bio.IO.GenBank.StandardQualifierNames.All.ToList, -1)
        t.ShowDialog()
        If t.DialogResult = DialogResult.OK Then
            Return t.SelectedStrings
        End If
        Return New List(Of String)
    End Function



#End Region

#Region "ExportFeatureSequences"
    Private Sub WholeToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles WholeToolStripMenuItem.Click
        ExportFeatures(Szunyi.Constants.Extension.Whole)
    End Sub
    Private Sub GenomicProcessedToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles GenomicProcessedToolStripMenuItem.Click
        ExportFeatures(Szunyi.Constants.Extension.Whole_And_Processed)

    End Sub
    Private Sub WithPromoterToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles WithPromoterToolStripMenuItem.Click
        ExportFeatures(Szunyi.Constants.Extension.FivePrimeExtension)
    End Sub

    Private Sub WithUTRToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles WithUTRToolStripMenuItem.Click
        ExportFeatures(Szunyi.Constants.Extension.ThreePrimeExtension)
    End Sub

    Private Sub With53PrimeExtensionToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles With53PrimeExtensionToolStripMenuItem.Click
        ExportFeatures(Szunyi.Constants.Extension.FiveAndThreePrimeExtension)
    End Sub

    Private Sub ExportFeatures(t As String)
        ' Get SelectedFeatureLists
        Dim SelectedFeatureLists = GetSelectedFeatureList()
        If IsNothing(SelectedFeatureLists) = True Then Exit Sub
        Dim Seqs = Me.GetSeqsFromSelectedList(False)
        If IsNothing(Seqs) = True Then Exit Sub
        Dim DiffFeatures = Szunyi.Features.FeatureManipulation.Key.Get_All_Different_Keys(Seqs)
        Dim Qulifiers = GetQulifiers(DiffFeatures)
        '   Dim Seqs = Szunyi.Sequences.SequenceManinpulation.MergeSequenceList(Me.GetSelectedSequenceList)
        Dim Out As New List(Of Bio.ISequence)
        ' Szunyi.Sequences.SequenceManipulation.GetSequences.
        Dim OnlyFirst As Boolean = True
        Dim t1 = MsgBox("Only First?", MsgBoxStyle.YesNo)
        If t1 = MsgBoxResult.No Then
            OnlyFirst = False
        End If
        Dim With_Location As Boolean = True
        t1 = MsgBox("With Location?", MsgBoxStyle.YesNo)
        If t1 = MsgBoxResult.No Then
            With_Location = False
        End If
        Dim With_FileName As Boolean = True
        t1 = MsgBox("With FIleName?", MsgBoxStyle.YesNo)
        If t1 = MsgBoxResult.No Then
            With_FileName = False
        End If
        For Each FeatureList In SelectedFeatureLists
            Select Case t
                Case Szunyi.Constants.Extension.Whole
                    Out.AddRange(Szunyi.Features.ExtFeatureManipulation.GetSequences(FeatureList, Qulifiers, OnlyFirst, With_Location, With_FileName, Nothing, Nothing))
                Case Szunyi.Constants.Extension.Whole_And_Processed
                    For Each feat In FeatureList.Features
                        Dim x = Szunyi.Sequences.SequenceManipulation.Get_Sub_Sequence.Processed(feat.Seq, feat.Feature)
                        x.ID = Szunyi.Sequences.SequenceManipulation.ID.Get_ID(Qulifiers, OnlyFirst, With_Location, With_FileName, feat) & "-tr"

                        Dim x1 = Szunyi.Sequences.SequenceManipulation.Get_Sub_Sequence.Whole(feat.Seq, feat.Feature)
                        x1.ID = x.ID & "-genomic"
                        Out.Add(x)
                        If x.Count <> x1.Count Then
                            Out.Add(x1)
                        End If
                        Dim r As Int16 = 65
                    Next
            End Select
        Next
        Dim out2 As New List(Of Bio.ISequence)

        Szunyi.IO.Export.SaveSequencesToSingleFasta(Out)

    End Sub
#End Region

#Region "Import"
    Private Sub ImportSequences()
        Dim Files = Szunyi.IO.Files.Filter.SelectFiles(Szunyi.Constants.Files.SequenceFileTypesToImport)
        If IsNothing(Files) = True Then Exit Sub
        If Files.Count > 1 Then
            Dim x = MsgBox("Do you want to Import Separately", MsgBoxStyle.YesNo)
            If x = MsgBoxResult.Yes Then
                For Each File In Files
                    Dim t1 As New Szunyi.ListOf.SequenceList(File)
                    CreateBgWork(Szunyi.Constants.BackGroundWork.Sequences, t1)
                Next
                Exit Sub
            End If
        End If
        If Files.Count = 1 Then
            Dim Title = Szunyi.IO.Files.Get_New_FileName.GetFileName_woExtension(Files.First)
            Dim t As New Szunyi.ListOf.SequenceList(Files, Title)
            CreateBgWork(Szunyi.Constants.BackGroundWork.Sequences, t)
        Else
            Dim Title = InputBox("Enter the title of seqs")
            Dim t As New Szunyi.ListOf.SequenceList(Files, Title)
            CreateBgWork(Szunyi.Constants.BackGroundWork.Sequences, t)
        End If


    End Sub

    Private Sub ImportLocationsWithCounts()
        Dim Files As List(Of FileInfo) = Szunyi.IO.Files.Filter.SelectFiles("Select Files Contains Location. SeqId,Start,End other")
        If IsNothing(Files) = True Then Exit Sub
        For Each File In Files
            '  Dim t As New Szunyi.Location.LocationList(File, Szunyi.Constants.BackGroundWork.Locations_With_Counts)
            '   CreateBgWork(Szunyi.Constants.BackGroundWork.Locations_With_Counts, t)
        Next

    End Sub

#End Region

#Region "GetSelectedLists"
    ''' <summary>
    ''' If No Any FeatureList Return Nothing, else a list of Mappings
    ''' </summary>
    ''' <returns></returns>
    Public Function GetSelectedMappings(Optional ShowMsg As Boolean = True) As List(Of Szunyi.Other_Database.Affy.ParseAffy)
        Dim SelectedFeaturesIDs = From x1 In SelectedItems Where x1.Type = Szunyi.Constants.BackGroundWork.Mapping Select x1.ID

        If SelectedFeaturesIDs.Count = 0 Then
            If ShowMsg = True Then MsgBox("There is no any Location in the list")
            Return Nothing

        End If

        Return (From t1 In ListOfs.ListOfMappings Where SelectedFeaturesIDs.Contains(t1.UniqueID)).ToList


    End Function
    ''' <summary>
    ''' If No Any FeatureList Return Nothing, else a list of ExtFeatureList
    ''' </summary>
    ''' <returns></returns>
    Public Function GetSelectedFeatureList(Optional ShowMsg As Boolean = True) As List(Of ExtFeatureList)
        Dim SelectedFeaturesIDs = From x1 In SelectedItems Where x1.Type = Szunyi.Constants.BackGroundWork.Features Select x1.ID

        If SelectedFeaturesIDs.Count = 0 Then
            If ShowMsg = True Then MsgBox("There is no any Location in the list")
            Return Nothing
        End If

        Return (From t1 In ListOfs.ListOfExtFeatures Where SelectedFeaturesIDs.Contains(t1.UniqueId)).ToList


    End Function
    ''' <summary>
    ''' If No Any FeatureList Return Nothing, else a list of LocationList
    ''' </summary>
    ''' <returns></returns>
    Public Function GetSelectedLocationList(Optional ShowMsg As Boolean = True) As List(Of Szunyi.Location.LocationList)
        Dim SelectedFeaturesIDs = From x1 In SelectedItems Where x1.Type = Szunyi.Constants.BackGroundWork.Locations Select x1.ID

        If SelectedFeaturesIDs.Count = 0 Then
            If ShowMsg = True Then MsgBox("There is no any Location in the list")
            Return Nothing
        End If
        Return (From t1 In ListOfs.ListOfLocations Where SelectedFeaturesIDs.Contains(t1.UniqueID)).ToList
    End Function
    ''' <summary>
    ''' If No Any FeatureList Return Nothing, else a list of SequenceList
    ''' </summary>
    ''' <returns></returns>
    Public Function GetSelectedSequenceList(Optional ShowMsg As Boolean = True) As List(Of SequenceList)
        Dim SelectedFeaturesIDs = From x1 In SelectedItems Where x1.Type = Szunyi.Constants.BackGroundWork.Sequences Select x1.ID

        If SelectedFeaturesIDs.Count = 0 Then
            If ShowMsg = True Then MsgBox("There is no any Sequence in the list")

            Return Nothing
        End If
        Return (From t1 In ListOfs.ListOfSequences Where SelectedFeaturesIDs.Contains(t1.UniqueID)).ToList
    End Function
    Private Function GetSelected_Sequences_With_Motifs(Optional ShowMsg As Boolean = True) As List(Of Szunyi.Sequence_Analysis.Sequences_Ranges)
        Dim SelectedFeaturesIDs = From x1 In SelectedItems Where x1.Type = Szunyi.Constants.BackGroundWork.Sequences_With_Motifs Select x1.ID

        If SelectedFeaturesIDs.Count = 0 Then
            If ShowMsg = True Then MsgBox("There is no any Sequences With Motifs in the list")

            Return New List(Of Szunyi.Sequence_Analysis.Sequences_Ranges)
        End If
        Return (From t1 In ListOfs.ListOf_Sequences_With_Motifs Where SelectedFeaturesIDs.Contains(t1.UniqueID)).ToList
    End Function

    Public Function GetSelected_Items_With_Propeties(Optional ShowMsg As Boolean = True) As List(Of Szunyi.Text.TableManipulation.Items_With_Properties)
        Dim SelectedFeaturesIDs = From x1 In SelectedItems Where x1.Type = Szunyi.Constants.BackGroundWork.Items_With_Properties Select x1.ID

        If SelectedFeaturesIDs.Count = 0 Then
            If ShowMsg = True Then MsgBox("There is no any Location in the list")
            Return Nothing
        End If
        Return (From t1 In ListOfs.ListOf_Item_With_Properties Where SelectedFeaturesIDs.Contains(t1.UniqueID)).ToList
    End Function
#End Region

#Region "ListChanges"
    Private Sub ListBox1_SelectedIndexChanged(sender As Object, e As EventArgs) Handles ListBox1.DoubleClick
        Dim Index = ListBox1.SelectedIndex
        If Index > -1 Then
            Me.SelectedItems.RemoveAt(Index)
        End If
    End Sub
    Private Sub P_Sequences_With_Motifs_NewSelection(x1 As Name_ID_Type_Title_Object) Handles P_Sequences_With_Motifs.NewSelection
        Dim cSeq = From t In ListOfs.ListOf_Sequences_With_Motifs Where t.UniqueID = x1.ID

        Dim x As New Name_ID_Type_Title_Object(cSeq.First.Title,
                                      cSeq.First.UniqueID,
                                      Szunyi.Constants.BackGroundWork.Sequences_With_Motifs,
                                      cSeq.First.Title,
                                      cSeq.First)
        SelectedItems.Add(x)
    End Sub
    Private Sub PSequenceListChanged(X1 As Name_ID_Type_Title_Object) Handles PSequenceList.NewSelection
        Dim cSeq = From t In ListOfs.ListOfSequences Where t.UniqueID = X1.ID

        Dim x As New Name_ID_Type_Title_Object(cSeq.First.ShortFileName,
                                      cSeq.First.UniqueID,
                                      Szunyi.Constants.BackGroundWork.Sequences,
                                      cSeq.First.ShortFileName,
                                      cSeq.First)
        SelectedItems.Add(x)

    End Sub
    Private Sub P_Item_with_Properties_DC(X1 As Name_ID_Type_Title_Object) Handles P_Item_With_Properties_List.NewSelection
        Dim cSeq = From t In ListOfs.ListOf_Item_With_Properties Where t.UniqueID = X1.ID

        Dim x As New Name_ID_Type_Title_Object(cSeq.First.Title,
                                      cSeq.First.UniqueID,
                                      Szunyi.Constants.BackGroundWork.Items_With_Properties,
                                      cSeq.First.Title,
                                      cSeq.First)
        SelectedItems.Add(x)

    End Sub
    Private Sub PSMappingListChanged(X1 As Name_ID_Type_Title_Object) Handles PMappingList.NewSelection
        Dim cSeq = From t In ListOfs.ListOfMappings Where t.UniqueID = X1.ID

        Dim x As New Name_ID_Type_Title_Object(cSeq.First.ShortFileName,
                                      cSeq.First.UniqueID,
                                      Szunyi.Constants.BackGroundWork.Mapping,
                                      cSeq.First.ShortFileName,
                                      cSeq.First)
        SelectedItems.Add(x)

    End Sub
    Private Sub PLocationsChanged(X1 As Name_ID_Type_Title_Object) Handles PLocationsList.NewSelection
        Dim cSeq = From t In ListOfs.ListOfLocations Where t.UniqueID = X1.ID
        Dim x As New Name_ID_Type_Title_Object(cSeq.First.Title,
                                      cSeq.First.UniqueID,
                                      Szunyi.Constants.BackGroundWork.Locations,
                                      cSeq.First.Title, cSeq.First)
        SelectedItems.Add(x)

    End Sub

    Private Sub PFeaturesChanged(X1 As Name_ID_Type_Title_Object) Handles PFeaturesList.NewSelection
        Dim cSeq = From t In ListOfs.ListOfExtFeatures Where t.UniqueId = X1.ID
        Dim x As New Name_ID_Type_Title_Object(cSeq.First.ShortFileName,
                                      cSeq.First.UniqueId,
                                      Szunyi.Constants.BackGroundWork.Features,
                                      cSeq.First.ShortFileName,
                                      cSeq.First)
        SelectedItems.Add(x)

    End Sub

    Private Sub P_Item_With_Properties_ListCHanged(x1 As Name_ID_Type_Title_Object) Handles P_Item_With_Properties_List.NewSelection
        Dim cSeq = From t In ListOfs.ListOf_Item_With_Properties Where t.UniqueID = x1.ID
        Dim x As New Name_ID_Type_Title_Object(cSeq.First.Title,
                                      cSeq.First.UniqueID,
                                      Szunyi.Constants.BackGroundWork.Items_With_Properties,
                                      cSeq.First.Title,
                                      cSeq.First)
        SelectedItems.Add(x)
    End Sub

#End Region

#Region "MenuItems"
#Region "Import"
#Region "Import Locations"
    Private Sub Import_Standard_Location_Click(sender As Object, e As EventArgs) Handles Import_Standard_Location.Click
        Dim Files = Szunyi.IO.Files.Filter.SelectFiles("Select Locations Files", Szunyi.Constants.Files.All_TAB_Like)
        If IsNothing(Files) = True Then Exit Sub
        For Each File In Files
            Dim x As New Szunyi.Location.Import(File, Location_Type.Standard)
            CreateBgWork(Szunyi.Constants.BackGroundWork.Locations, x)
        Next
    End Sub

    Private Sub Import_Bed_Click(sender As Object, e As EventArgs) Handles Import_Bed.Click
        Dim Files = Szunyi.IO.Files.Filter.SelectFiles("Select BED FIles", Szunyi.Constants.Files.Other.BED)
        If IsNothing(Files) = True Then Exit Sub
        For Each File In Files
            Dim x As New Szunyi.Location.Import(File, Location_Type.BED)
            CreateBgWork(Szunyi.Constants.BackGroundWork.Locations, x)
        Next
    End Sub

    Private Sub Import_Gff3_Click(sender As Object, e As EventArgs) Handles Import_Gff3.Click
        Dim Files = Szunyi.IO.Files.Filter.SelectFiles("Select BED FIles", Szunyi.Constants.Files.gff3)
        If IsNothing(Files) = True Then Exit Sub
        For Each File In Files
            Dim x As New Szunyi.Location.Import(File, Location_Type.GFF3)
            CreateBgWork(Szunyi.Constants.BackGroundWork.Locations, x)
        Next
    End Sub

#End Region
    Private Sub Import_Blast_Click(sender As Object, e As EventArgs) Handles Import_Blast.Click

    End Sub
    Private Sub ImportSequncesToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles SequncesToolStripMenuItem.Click
        ImportSequences()
    End Sub
    Private Sub AffyProbesFromClassicalToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles ClassicalToolStripMenuItem.Click
        Dim File = Szunyi.IO.Files.Filter.SelectFile("Select Affy Probes File")
        If IsNothing(File) = True Then Exit Sub
        Dim x As New Szunyi.Other_Database.Affy.AffyProbes(File, 1, 0, 4)
        Dim SelectedSeqs = Me.GetSelectedSequenceList
        Dim Seqs = Merging.MergeSequenceList(SelectedSeqs)
        Dim t As New Szunyi.Other_Database.Affy.ParseAffy(Seqs, x)
        If SelectedSeqs.Count < 2 Then
            t.ShortFileName = "Affy Mapping:" & SelectedSeqs.First.ShortFileName

        Else
            Dim s = InputBox("Enter the Name of The AffyPobeList")
            t.ShortFileName = "Affy Mapping:" & s
        End If
        CreateBgWork(Szunyi.Constants.BackGroundWork.Mapping, t)
    End Sub
#End Region

#End Region

#Region "Locations"
    Private Sub ModifyGenePositionBasedOnLacationAndBamFileToolStripMenuItem_Click(sender As Object, e As EventArgs)
        Dim SelectedLocationsID = From x In SelectedItems Where x.Type = Szunyi.Constants.BackGroundWork.Locations Select x.ID

        If SelectedLocationsID.Count = 0 Then
            MsgBox("There is no any Location in the list")
            Exit Sub
        End If

        Dim SelectedLocations = From t1 In ListOfs.ListOfLocations Where SelectedLocationsID.Contains(t1.UniqueID)

        Dim SelectedSeqsID = From x1 In SelectedItems Where x1.Type = Szunyi.Constants.BackGroundWork.Sequences Select x1.ID

        If SelectedSeqsID.Count = 0 Then
            MsgBox("There is no any Seq in the list")
            Exit Sub
        End If

        Dim SelectedSeqs = From t1 In ListOfs.ListOfExtFeatures Where SelectedSeqsID.Contains(t1.UniqueId)

        Dim FIle = Szunyi.IO.Files.Filter.SelectFile("Select Bam File")



    End Sub

    Private Sub GetNeighbouroToolStripMenuItem_Click(sender As Object, e As EventArgs)
        ' Select Location From Listbox1
        Dim SelectedLocationsID = From x In SelectedItems Where x.Type = Szunyi.Constants.BackGroundWork.Locations Select x.ID

        If SelectedLocationsID.Count = 0 Then
            MsgBox("There is no any Location in the list")
            Exit Sub
        End If

        Dim SelectedLocations = From t In ListOfs.ListOfLocations Where SelectedLocationsID.Contains(t.UniqueID)

        Dim SelectedFeaturesIDs = From x1 In SelectedItems Where x1.Type = Szunyi.Constants.BackGroundWork.Features Select x1.ID

        If SelectedFeaturesIDs.Count = 0 Then
            MsgBox("There is no any Feature in the list")
            Exit Sub
        End If

        Dim SelectedFeatures = From t1 In ListOfs.ListOfExtFeatures Where SelectedFeaturesIDs.Contains(t1.UniqueId)



        Dim x2 As New FeatureAndLocationMerger(SelectedFeatures.ToList, SelectedLocations.ToList, "Szomszed")
        x2.DoIt()
    End Sub

#End Region

#Region "RNASeq"
    Private Sub FromFolderToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles FromFolderToolStripMenuItem.Click
        Dim folder = Szunyi.IO.Directory.Get_Folder
        Dim t = Szunyi.Text.General.GetIntegerFromInputbox

        Dim x As New Szunyi.Sequences.MaintainUniqueReads(folder.FullName, t)
        CreateBgWork(Szunyi.Constants.BackGroundWork.MaintainUniqeSequence, x)
    End Sub

    Private Sub FromFilesToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles FromFilesToolStripMenuItem.Click
        Dim Files = Szunyi.IO.Files.Filter.SelectFiles
        Dim t = Szunyi.Text.General.GetIntegerFromInputbox

        Dim x As New Szunyi.Sequences.MaintainUniqueReads(Files, t)
        CreateBgWork(Szunyi.Constants.BackGroundWork.MaintainUniqeSequence, x)
    End Sub
    Private Sub SimpleToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles SimpleToolStripMenuItem.Click
        Dim x As New Szunyi.Text.TableManipulation.ReplaceStringsInFiles
        ' If x.Ready = True Then CreateBgWork(Szunyi.Constants.BackGroundWork.ReplaceStringsInFiles, x)
        x.DOItSimple()
    End Sub
    Private Sub StringsInFilesToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles StringsInFilesToolStripMenuItem.Click
        Dim x As New Szunyi.Text.TableManipulation.ReplaceStringsInFiles
        ' If x.Ready = True Then CreateBgWork(Szunyi.Constants.BackGroundWork.ReplaceStringsInFiles, x)
        x.DoIt()

    End Sub
    Private Sub InAnotherColumnToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles InAnotherColumnToolStripMenuItem.Click
        Dim x As New Szunyi.Text.Table
        Dim OutputFolder = Szunyi.IO.Directory.Get_Folder
        Dim TheComp As New Szunyi.Other_Database.CrossRefs.ComparerOfCrossrefOneToOneByFirst
        Dim File = Szunyi.IO.Files.Filter.SelectFile("File Contains Original And new Strings")
        If IsNothing(File) = True Then Exit Sub
        Dim IndexOfOriginalString = input.Input.GetIntrestingColumn(File, 1, "Original Strings")
        Dim IndexOfNewString = input.Input.GetIntrestingColumn(File, 1, "New Strings")
        Dim CrossTable = Szunyi.Other_Database.CrossRefs.CrossRefBuilders.GetOneToOneFromFile(File, IndexOfOriginalString, IndexOfNewString, 1)

        Dim Files = Szunyi.IO.Files.Filter.SelectFiles("Select Files Where The Changing Happening")
        If IsNothing(Files) = True Then Exit Sub
        IndexOfOriginalString = input.Input.GetIntrestingColumn(Files.First, 1, "Original Strings")
        IndexOfNewString = input.Input.GetIntrestingColumn(Files.First, 1, "New Strings")
        x.ReplaceStringsInAnotherColumn(CrossTable, IndexOfOriginalString, IndexOfNewString, Files, OutputFolder.FullName)


    End Sub
#End Region

#Region "Text"
    Private Sub ReplaceFirstLineToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles ReplaceFirstLineToolStripMenuItem.Click
        Dim FIles = Szunyi.IO.Files.Filter.SelectFiles
        Dim NewLine = InputBox("New Line Text")
        For Each File In FIles
            '   Dim nFile = Szunyi.IO.Files.Get_New_FileName.GetNewFile("Replaced_First_Line" & File.Extension)
            '   Using sw As New StreamWriter(nFile.FullName)
            '    sw.Write(NewLine)
            '    For Each Line In Szunyi.IO.Import.Text.ParseNotFirst(File, 1)
            '     sw.Write(vbCrLf)
            '     sw.Write(Line)
            '  Next
            '      End Using

        Next
    End Sub

    Private Sub SplitByColumnsToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles SplitByColumnsToolStripMenuItem.Click
        Dim BasicFiles = Szunyi.IO.Files.Filter.SelectFiles("Select Basic Files")
        If IsNothing(BasicFiles) Then Exit Sub

        Dim Header_Of_Basic_Files = ClassLibrary1.Szunyi.IO.Import.Text.GetHeader(BasicFiles.First, Nothing, Nothing, Nothing)

        Dim ID_For_Basic_Files As New CheckBoxForStringsFull(Header_Of_Basic_Files, 1, "Select ID column")
        If ID_For_Basic_Files.ShowDialog <> DialogResult.OK Then Exit Sub
        For Each File In BasicFiles
            Dim x As New Dictionary(Of String, StringBuilder)
            For Each s In Szunyi.IO.Import.Text.ParseToArray(File, vbTab, 1)
                If s.Count > ID_For_Basic_Files.SelectedIndexes.First Then
                    Dim Key As String = s(ID_For_Basic_Files.SelectedIndexes.First)
                    If x.ContainsKey(Key) = False Then
                        x.Add(Key, New StringBuilder)
                        x(Key).Append(Szunyi.Text.General.GetText(Header_Of_Basic_Files, vbTab)).AppendLine()
                    End If
                    x(Key).Append(Szunyi.Text.General.GetText(s, vbTab)).AppendLine()
                End If

            Next
            Dim Folder = File.Directory
            For Each Item In x
                Dim NFIle As New FileInfo(Folder.FullName & "\" & Item.Key & " " & File.Name)
                Szunyi.IO.Export.SaveText(Item.Value.ToString, NFIle)
            Next
        Next

    End Sub
    Private Sub MaintainAllToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles MaintainAllToolStripMenuItem.Click
        ' Select First File
        ' Select Columns
        ' Select Other Files
        ' Select Other columns
        ' Do It as background
        Dim BasicFiles = Szunyi.IO.Files.Filter.SelectFiles("Select Basic Files")
        If IsNothing(BasicFiles) Then Exit Sub

        Dim Header_Of_Basic_Files = ClassLibrary1.Szunyi.IO.Import.Text.GetHeader(BasicFiles.First, 1, Nothing, Nothing)

        Dim ID_For_Basic_Files As New CheckBoxForStringsFull(Header_Of_Basic_Files, 1, "Select ID column")
        If ID_For_Basic_Files.ShowDialog <> DialogResult.OK Then Exit Sub

        Dim Items_For_Basic_Files As New CheckBoxForStringsFull(Header_Of_Basic_Files, -1, "Select Columns to include")
        If Items_For_Basic_Files.ShowDialog <> DialogResult.OK Then Exit Sub

        '    Items_For_Basic_Files.SelectedIndexes = Items_For_Basic_Files.SelectedIndexes.Except(ID_For_Basic_Files.SelectedIndexes).ToList


        Dim Additional_File = Szunyi.IO.Files.Filter.SelectFile("Select Additional File")
        If IsNothing(Additional_File) = True Then Exit Sub

        Dim Header_Of_Additional_File = ClassLibrary1.Szunyi.IO.Import.Text.GetHeader(Additional_File, 1, Nothing, Nothing)

        Dim ID_For_Additional_File As New CheckBoxForStringsFull(Header_Of_Additional_File, 1, "Select ID column")
        If ID_For_Additional_File.ShowDialog <> DialogResult.OK Then Exit Sub

        Dim Items_For_Additional_File As New CheckBoxForStringsFull(Header_Of_Additional_File, -1, "Select Columns to include")
        If Items_For_Additional_File.ShowDialog <> DialogResult.OK Then Exit Sub

        Items_For_Additional_File.SelectedIndexes = Items_For_Additional_File.SelectedIndexes.ToList
        Dim Log As New List(Of String)

        For Each File In BasicFiles
            Dim res As New Szunyi.Text.TableManipulation.Items_With_Properties(File,
                                                                         ID_For_Basic_Files.SelectedIndexes.First,
                                                                         Items_For_Basic_Files.SelectedIndexes, Log, Nothing, False)
            res.DoIt(False)
            '     res.ReNameID_Firts_Part("_")

            res.Add(Additional_File,
                    ID_For_Additional_File.SelectedIndexes.First,
                    Items_For_Additional_File.SelectedIndexes,
                    Log)
            Dim NewFIle As New FileInfo(File.FullName.Replace(File.Extension, " " & Additional_File.Name))
            Dim NewFIle2 = Szunyi.IO.Files.Get_New_FileName.GetNewFile(File, Additional_File.Name & File.Extension)

            ' Szunyi.IO.Export.SaveText(res.ToString, NewFIle
            res.Save_With_ID(NewFIle2)
            Dim alf As Int16 = 54
        Next





    End Sub

    Private Sub MaintainAllWithShortLocusTagToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles MaintainAllWithShortLocusTagToolStripMenuItem.Click
        ' Select First File
        ' Select Columns
        ' Select Other Files
        ' Select Other columns
        ' Do It as background
        Dim BasicFiles = Szunyi.IO.Files.Filter.SelectFiles()
        If IsNothing(BasicFiles) Then Exit Sub

        Dim Header_Of_Basic_Files = ClassLibrary1.Szunyi.IO.Import.Text.GetHeader(BasicFiles.First, 1, Nothing, Nothing)

        Dim ID_For_Basic_Files As New CheckBoxForStringsFull(Header_Of_Basic_Files, 1, "Select ID column")
        If ID_For_Basic_Files.ShowDialog <> DialogResult.OK Then Exit Sub

        Dim Items_For_Basic_Files As New CheckBoxForStringsFull(Header_Of_Basic_Files, -1, "Select Columns to include")
        If Items_For_Basic_Files.ShowDialog <> DialogResult.OK Then Exit Sub

        '    Items_For_Basic_Files.SelectedIndexes = Items_For_Basic_Files.SelectedIndexes.Except(ID_For_Basic_Files.SelectedIndexes).ToList


        Dim Additional_File = Szunyi.IO.Files.Filter.SelectFile("Select Additional File")
        If IsNothing(Additional_File) = True Then Exit Sub

        Dim Header_Of_Additional_File = ClassLibrary1.Szunyi.IO.Import.Text.GetHeader(Additional_File, 1, Nothing, Nothing)

        Dim ID_For_Additional_File As New CheckBoxForStringsFull(Header_Of_Additional_File, 1, "Select ID column")
        If ID_For_Additional_File.ShowDialog <> DialogResult.OK Then Exit Sub

        Dim Items_For_Additional_File As New CheckBoxForStringsFull(Header_Of_Additional_File, -1, "Select Columns to include")
        If Items_For_Additional_File.ShowDialog <> DialogResult.OK Then Exit Sub

        Items_For_Additional_File.SelectedIndexes = Items_For_Additional_File.SelectedIndexes.ToList
        Dim Log As New List(Of String)

        Dim Additional_Items As New Szunyi.Text.TableManipulation.Items_With_Properties(Additional_File,
                                                                         ID_For_Additional_File.SelectedIndexes.First,
                                                                         Items_For_Additional_File.SelectedIndexes, Log, Nothing, False)
        For Each File In BasicFiles
            Dim res As New Szunyi.Text.TableManipulation.Items_With_Properties(File,
                                                                         ID_For_Basic_Files.SelectedIndexes.First,
                                                                         Items_For_Basic_Files.SelectedIndexes, Log, Nothing, False)

            res.RenameIDsToShortLocusTAg()

            res.Merge(Additional_Items)

            Dim NewFIle As New FileInfo(File.FullName.Replace(File.Extension, " " & Additional_File.Name))
            Dim NewFIle2 As New FileInfo(NewFIle.FullName.Replace(NewFIle.Extension, ".tdt"))

            Szunyi.IO.Export.SaveText(res.ToString, NewFIle2)

        Next


    End Sub

    Private Sub InsertXAndVbtabInTheFirstLineToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles InsertXAndVbtabInTheFirstLineToolStripMenuItem.Click
        Dim s = Szunyi.Text.General.GetStringFromInputbox("Text to Insert into First Row")
        Dim Files = Szunyi.IO.Files.Filter.SelectFiles
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
        Dim file = Szunyi.IO.Files.Filter.SelectFile
        If IsNothing(file) = True Then Exit Sub

        Dim IDcol = input.Input.GetIntrestingColumn(file, 1, "Select Id Col")
        Dim OtherCols = Szunyi.IO.Import.Headers.GetIntrestingColumns(file, 1, "Select Other Columns")

        Dim file2 = Szunyi.IO.Files.Filter.SelectFiles
        If IsNothing(file2) = True Then Exit Sub
        Dim IDcol2 = input.Input.GetIntrestingColumn(file2.First, 1, "Select Id Col")
        Dim OtherCols2 = Szunyi.IO.Import.Headers.GetIntrestingColumns(file2.First, 1, "Select Other Columns")

        Dim log As New StringBuilder
        For Each fájl In file2
            Dim Header = Szunyi.IO.Import.Text.GetHeader(fájl, 1, Nothing, OtherCols2)

            Dim res = Szunyi.IO.Files.Extra.GetValuesToDictionary(fájl, IDcol2, OtherCols2, 1)
            Dim NewFIle = Szunyi.IO.Files.Save.SelectSaveFile("", "")
            Using sw As New StreamWriter(NewFIle.FullName, False)
                Dim NofLine As Integer = 0
                For Each Line In Szunyi.IO.Import.Text.Parse(file)
                    sw.Write(Line)
                    sw.Write(vbTab)
                    If NofLine = 0 Then
                        sw.Write(Szunyi.Text.General.GetText(Header, vbTab))
                    Else
                        Dim ID = Split(Line, vbTab)(IDcol)
                        If res.ContainsKey(ID) = False Then
                            log.Append(ID).AppendLine()
                        Else
                            sw.Write(Szunyi.Text.General.GetText(res(ID), vbTab))
                        End If
                    End If
                    NofLine += 1
                    sw.WriteLine()
                Next
            End Using

        Next

    End Sub

    Private Sub GetOveralappingFeaturesToolStripMenuItem_Click(sender As Object, e As EventArgs)
        ' Get FeatureList By SubTypes
        ' Get Locations 
        ' Find all Locations in in featurelist
        ' Set Count To Zero
        Dim Features = Me.GetSelectedFeatureList

        Dim Locations = Me.GetSelectedLocationList
        If IsNothing(Locations) = True Then Exit Sub
        Dim FeatureLocation = From x In Locations Where x.SubType = Szunyi.Constants.BackGroundWork.Features

        If FeatureLocation.Count = 0 Then
            MsgBox("No Any Feature!")
            Exit Sub
        End If

        Dim LocationLocation = From x1 In Locations Where x1.SubType = Szunyi.Constants.BackGroundWork.Locations

        If FeatureLocation.Count = 0 Then
            MsgBox("No Any Feature!")
            Exit Sub
        End If

        Dim x2 As New FeatureAndLocationMerger(Features, Locations, Szunyi.Constants.Merge.Count)




    End Sub



    Private Sub CountFormTDTToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles CountFormTDTToolStripMenuItem.Click
        Dim x As New Text.GetCount()

    End Sub

    Private Sub StingInFilesToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles StingInFilesToolStripMenuItem.Click
        Dim Original = InputBox("Original String")
        If Original = String.Empty Then Exit Sub
        Dim NewString = InputBox("New String")
        Dim FIles = Szunyi.IO.Files.Filter.SelectFiles
        If IsNothing(FIles) = True Then Exit Sub
        For Each FIle In FIles
            Dim txt = Szunyi.IO.Import.Text.ReadToEnd(FIle)
            txt = txt.Replace(Original, NewString)
            Szunyi.IO.Export.SaveText(txt, FIle)
        Next
    End Sub


    Private Sub SelectRowsContainingToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles SelectRowsContainingToolStripMenuItem.Click
        Dim Files = Szunyi.IO.Files.Filter.SelectFiles
        If IsNothing(Files) = True Then Exit Sub
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
            Dim OutFIle = Szunyi.IO.Files.Get_New_FileName.GetNewFile(File, txt)
            Szunyi.IO.Export.SaveText(str.ToString, OutFIle)
        Next
    End Sub

    Private Sub LocationsToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles LocationsToolStripMenuItem.Click

        Dim Seqs = Me.GetSeqsFromSelectedList(True)
        If IsNothing(Seqs) = True Then Exit Sub
        Dim DiffKeys = Szunyi.Features.FeatureManipulation.Key.Get_All_Different_Keys(Seqs)

        Dim f As New CheckBoxForStringsFull(DiffKeys, -1)
        If f.ShowDialog <> DialogResult.OK Then Exit Sub
        Dim Feats = Szunyi.Features.FeatureManipulation.GetFeaturesByType.GetFeturesByTypesFromSeqs(Seqs, f.SelectedStrings)
        Dim str As New System.Text.StringBuilder
        For Each Feat In Feats
            str.Append(Feat.Key).Append(vbTab)
            str.Append(Feat.Label).Append(vbTab)
            str.Append(Szunyi.Location.Common.GetLocationString(Feat)).Append(vbTab)
            str.Append(Szunyi.Location.Common.Get_Length(Feat)).AppendLine()
        Next
        If str.Length > 0 Then
            str.Length -= 2
            Clipboard.SetText(str.ToString)
        End If
    End Sub

    Private Sub ExportQualifersToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles QualifersToolStripMenuItem.Click
        Dim SelectedFeaturesLists = Me.GetSelectedFeatureList
        If IsNothing(SelectedFeaturesLists) = True Then Exit Sub
        Dim f As New CheckBoxForStringsFull(Bio.IO.GenBank.StandardQualifierNames.All.ToList, -1)
        If f.ShowDialog <> DialogResult.OK Then Exit Sub
        Dim Header = Szunyi.Text.General.GetText(f.SelectedStrings, vbTab) & vbCrLf
        For Each FeatureList In SelectedFeaturesLists
            Dim s = Header & Szunyi.Features.ExtFeatureManipulation.GetTextFromExtFeatureList(FeatureList, f.SelectedStrings, vbTab, vbCrLf)
            Szunyi.IO.Export.SaveText(s)
        Next

    End Sub
    Private Sub BorderSequencesToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles BorderSequencesToolStripMenuItem.Click
        Dim SelectedFeaturesLists = Me.GetSelectedFeatureList
        If IsNothing(SelectedFeaturesLists) = True Then Exit Sub
        Dim Feats = Szunyi.Features.ExtFeatureManipulation.MergeLists(SelectedFeaturesLists)
        Dim width As Integer = MyInputBox.GetInteger("Enter Width")
        Dim str As New System.Text.StringBuilder
        For Each Feat In Feats
            str.Append(Feat.Seq.GetSubSequence(Feat.Feature.Location.LocationStart - width, width)).Append(vbTab)
            str.Append(Feat.Seq.GetSubSequence(Feat.Feature.Location.LocationEnd, width)).AppendLine()
        Next
        Clipboard.SetText(str.ToString)
    End Sub
    Private Sub FirstPartToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles FirstPartToolStripMenuItem.Click
        ' Get Kmers
        ' Get Sequence Files
        Dim FIles = Szunyi.IO.Files.Filter.SelectFiles
        Dim x As New Szunyi.Sequences.SeqsKmerswCounts(FIles, 50, True, 1000000, Szunyi.Constants.OutPutType.AsTabFile, 10000)
        'Dim x As New BackgroundWorker
        x.DoIt()
    End Sub
    Private Sub StringsToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles StringsToolStripMenuItem.Click
        Dim FIles = Szunyi.IO.Files.Filter.SelectFiles
        If IsNothing(FIles) = True Then Exit Sub
        ' Select Interesting Columns
        ' Select Interesting Strings
        ' Select FOrbidden Strings
        Dim ColsToInvestigate = Szunyi.IO.Import.Headers.GetIntrestingColumns(FIles.First, 1)
        Dim x As New Get_List_of_String("Interesting Strings")
        If x.ShowDialog = DialogResult.OK Then
            Dim StringsForMaintains = x.Strings

            For Each File In FIles
                Dim CFile = Szunyi.IO.Files.Get_New_FileName.GetNewFile(File, "Contains" & File.Extension)
                Dim NCFile = Szunyi.IO.Files.Get_New_FileName.GetNewFile(File, "Not_Contains" & File.Extension)
                Dim Header = Szunyi.IO.Import.Text.GetHeader(File, 1)
                Dim TheHeader = Szunyi.Text.General.GetText(Header, vbTab)
                Using c As New StreamWriter(CFile.FullName)
                    Using N As New StreamWriter(NCFile.FullName)
                        c.Write(TheHeader)
                        N.Write(TheHeader)
                        For Each Line In Szunyi.IO.Import.Text.ParseNotFirst(File, 1)
                            Dim s1 = Split(Line, vbTab)
                            Dim Has As Boolean = False
                            For Each i In ColsToInvestigate

                                If StringsForMaintains.Contains(s1(i)) Then
                                    Has = True
                                    Exit For
                                End If

                            Next
                            If Has = True Then
                                c.WriteLine()
                                c.Write(Line)
                            Else
                                N.WriteLine()
                                N.Write(Line)
                            End If
                        Next

                    End Using
                End Using

            Next

        End If


    End Sub
    Private Sub StringToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles StringToolStripMenuItem.Click
        Dim Original = InputBox("Original String")
        Dim FIles = Szunyi.IO.Files.Filter.SelectFiles
        If IsNothing(FIles) = True Then Exit Sub
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
    Private Sub SelectSeqsByIDFIlesToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles SelectSeqsByIDFIlesToolStripMenuItem.Click
        Dim SeqLists = GetSelectedSequenceList()
        If IsNothing(SeqLists) = True Then Exit Sub
        Dim AllSeqs = Merging.MergeSequenceList(SeqLists)
        Dim tmpSeq As New Bio.Sequence(Alphabets.DNA, "")
        Dim Files = Szunyi.IO.Files.Filter.SelectFiles
        If IsNothing(Files) = True Then Exit Sub
        Dim RNSeqs = Szunyi.Sequences.SequenceManipulation.ID.Rename(AllSeqs, Szunyi.Constants.StringRename.FirstAfterSplit, ".")

        For Each File In Files
            Dim IDs = Szunyi.IO.Import.Text.ReadLines(File)
            Dim Out As New List(Of Bio.ISequence)
            For Each Id In IDs
                tmpSeq.ID = Id
                Dim i = RNSeqs.BinarySearch(tmpSeq, Comparares.AllComparares.BySeqIDAndLength)
                If i > -1 Then
                    Out.Add(RNSeqs(i))
                End If
            Next
            Dim NewFIle = Szunyi.IO.Files.Get_New_FileName.GetNewFile(File, ".fa")
            Szunyi.IO.Export.SaveSequencesToSingleFasta(Out, NewFIle)
        Next
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
        Dim AllSeqs = Merging.MergeSequenceList(SeqLists)
        Dim RN_Seqs = Szunyi.Sequences.SequenceManipulation.ID.Rename(AllSeqs, Szunyi.Constants.StringRename.FirstAfterSplit, ".")
        Dim tmpSeq As Bio.ISequence = New Bio.Sequence(Alphabets.DNA, "")
        Dim Out As New List(Of Bio.ISequence)
        Dim Log As New List(Of String)
        For Each Item In InterestingStrings

            tmpSeq.ID = Item
            Dim i = RN_Seqs.BinarySearch(tmpSeq, Comparares.AllComparares.BySeqIDAndLength)
            If i > -1 Then
                For i1 = i To 0 Step -1
                    If tmpSeq.ID = RN_Seqs(i1).ID Then
                        Out.Add(AllSeqs(i1))
                    Else
                        Exit For
                    End If
                Next
                For i1 = i + 1 To RN_Seqs.Count - 1
                    If tmpSeq.ID = RN_Seqs(i1).ID Then
                        Out.Add(AllSeqs(i1))
                    Else
                        Exit For
                    End If
                Next
            Else
                Log.Add(Item)
            End If
        Next
        Dim txt = Szunyi.Text.General.GetText(Log)
        Dim out2 As New List(Of Bio.ISequence)

        For Each Item In ForbiddenStrings
            Dim t1 = From h1 In Out Where h1.ID.IndexOf(Item) < 0

            If t1.Count <> Out.Count Then Out = t1.ToList
        Next
        Szunyi.IO.Export.Sequences(Out, False)

    End Sub
    Private Sub SelectSeqsByIDContainsToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles SelectSeqsByIDContainsToolStripMenuItem.Click
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
        Dim AllSeqs = Merging.MergeSequenceList(SeqLists)
        Dim out As New List(Of Bio.ISequence)
        For Each Item In InterestingStrings
            Dim cSeqs = From x1 In AllSeqs Where x1.ID.Contains(Item) = True
            out.AddRange(cSeqs)
        Next
        out = out.Distinct.ToList
        Szunyi.IO.Export.SaveSequencesToSingleFasta(out)
    End Sub

    Private Sub SelectSeqsByStartWithToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles SelectSeqsByStartWithToolStripMenuItem.Click
        Dim SeqList = Me.GetSelectedSequenceList(False)
        Dim Index As Integer = 0
        Dim St = InputBox("Enter Seqs")
        Dim out As New List(Of Bio.ISequence)
        For Each sl In SeqList
            Index += 1
            For Each Seq In sl.Sequences
                Dim s = Szunyi.Sequences.SequenceManipulation.SeqsToString.GetSeqAsString(Seq)
                If s.StartsWith(St) Then
                    out.Add(Seq)
                    Seq.ID = Index & "_" & Seq.ID
                End If
            Next
        Next
        Szunyi.IO.Export.SaveSequencesToSingleFasta(out)
    End Sub



    Private Sub WithCheckToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles WithCheckToolStripMenuItem.Click
        ExportSeqs(True)
    End Sub

    Private Sub WithOutCheckToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles WithOutCheckToolStripMenuItem.Click
        ExportSeqs(False)
    End Sub

    Private Sub ExportSeqs(WithCheck As Boolean)

        Dim SeqLists As List(Of SequenceList) = GetSelectedSequenceList(True)
        If IsNothing(SeqLists) = True Then Exit Sub
        For Each Sl In SeqLists
            For Each seq In Sl.Sequences
                If seq.Alphabet.Name <> "Dna" Then
                    Dim kj As Int16 = 43
                End If
            Next
        Next
        If IsNothing(SeqLists) = True Then Exit Sub
        If SeqLists.Count > 1 Then
            Dim x As MsgBoxResult = MsgBox("Do you want to save Separatley", MsgBoxStyle.YesNo)

            If x = MsgBoxResult.Yes Then
                Dim Folder = Szunyi.IO.Directory.Get_Folder("Select Folder To Save")
                For Each SeqList In SeqLists
                    Dim File As New FileInfo(Folder.FullName & "\" & SeqList.ShortFileName.Replace(":", "") & ".fa")
                    Szunyi.IO.Export.SaveSequencesToSingleFasta(SeqList.Sequences, File)
                Next
                Exit Sub
            End If

        End If

        Dim AllSeqs = Merging.MergeSequenceList(SeqLists)
        Szunyi.IO.Export.Sequences(AllSeqs, WithCheck)
    End Sub

    Private Sub BlastToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles BlastToolStripMenuItem.Click
        Dim x As New BlastViewer
        x.Show()

    End Sub




    Private Sub GetAgilentSureSelectForm(s As String, str2 As StringBuilder,
                                         i1 As Integer, Start As Integer, UsedSeq As List(Of String), SeqID As String, ByRef NewSeq As Bio.ISequence)
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

    Private Sub CopyFeatureAnnotationsIntoAnotherAnnotationsToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles CopyFeatureAnnotationsIntoAnotherAnnotationsToolStripMenuItem.Click
        Dim SeqLists = Me.GetSelectedSequenceList

        Dim x1 As New CheckBoxForStringsFull(Bio.IO.GenBank.StandardFeatureKeys.All.ToList, "Select Feature To")
        Dim FeaturesTo As New List(Of String)
        If x1.ShowDialog = DialogResult.OK Then
            FeaturesTo = x1.SelectedStrings
        End If

        Dim x2 As New CheckBoxForStringsFull(Bio.IO.GenBank.StandardFeatureKeys.All.ToList, "Select Feature To")
        Dim FeaturesFrom As New List(Of String)
        If x2.ShowDialog = DialogResult.OK Then
            FeaturesFrom = x2.SelectedStrings
        End If
        Dim t As New Szunyi.Features.FeatureManipulation.MergeFeatureAnnotation(SeqLists, FeaturesTo, FeaturesFrom)
        t.DoIt()
    End Sub
#Region "Rename"
#Region "SeqID"
    Private Sub PrefixAscendingToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles PrefixAscendingToolStripMenuItem.Click
        Dim SeqLists As List(Of SequenceList) = GetSelectedSequenceList()
        If IsNothing(SeqLists) = True Then Exit Sub
        Dim Prefix As String = ""
        If SeqLists.Count > 1 Then
            Dim x = MsgBox("Do you Want to Rename Separatley", MsgBoxStyle.YesNo)
            If x = MsgBoxResult.Yes Then
                For Each SeqList In SeqLists
                    Prefix = InputBox("Prefix Of " & SeqList.ShortFileName)
                    Szunyi.IO.Export.Sequences(ID.Rename(SeqList.Sequences, Szunyi.Constants.StringRename.AscendingWithPrefix, Prefix), False)
                Next
                Exit Sub
            End If
        End If
        Prefix = InputBox("Prefix")
        Dim Seqs = Me.GetSeqsFromSelectedList(True)
        Szunyi.IO.Export.Sequences(ID.Rename(Seqs, Szunyi.Constants.StringRename.AscendingWithPrefix, Prefix), False)
    End Sub

    Private Sub PostFix_Ascending(sender As Object, e As EventArgs) Handles ToolStripMenuItem12.Click
        Dim SeqLists As List(Of SequenceList) = GetSelectedSequenceList()
        If IsNothing(SeqLists) = True Then Exit Sub
        Dim Prefix As String = ""
        If SeqLists.Count > 1 Then
            Dim x = MsgBox("Do you Want to Rename Separatley", MsgBoxStyle.YesNo)
            If x = MsgBoxResult.Yes Then
                For Each SeqList In SeqLists
                    Prefix = InputBox("Prefix Of " & SeqList.ShortFileName)
                    Szunyi.IO.Export.Sequences(ID.Rename(SeqList.Sequences, Szunyi.Constants.StringRename.Ascending_With_PostFix, Prefix), False)
                Next
                Exit Sub
            End If
        End If
        Prefix = InputBox("Prefix")
        Dim Seqs = Me.GetSeqsFromSelectedList(True)
        Szunyi.IO.Export.Sequences(ID.Rename(Seqs, Szunyi.Constants.StringRename.Ascending_With_PostFix, Prefix), False)
    End Sub

    Private Sub FisrtPartOfToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles FisrtPartOfToolStripMenuItem.Click
        Dim Seqs = Me.GetSeqsFromSelectedList(True)
        If IsNothing(Seqs) = True Then Exit Sub
        Dim Separator = InputBox("Enter the Separator")
        If Separator.Length = 0 Then Exit Sub
        Szunyi.IO.Export.Sequences(ID.Rename(Seqs, StringRename.FirstAfterSplit, Separator), False)

    End Sub
    Private Sub FromFirstLocusTagToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles FromFirstLocusTagToolStripMenuItem.Click
        Dim Seqs = Me.GetSeqsFromSelectedList(True)
        If IsNothing(Seqs) = True Then Exit Sub
        Dim NewSeqs = Szunyi.Sequences.SequenceManipulation.ID.RenameByGeneNote(Seqs)
        Szunyi.IO.Export.SaveSequencesToSingleGenBank(NewSeqs)
    End Sub

    Private Sub LastPartOfAfterSplittingToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles LastPartOfAfterSplittingToolStripMenuItem.Click
        Dim Seqs = Me.GetSeqsFromSelectedList(True)
        If IsNothing(Seqs) = True Then Exit Sub
        Dim Separator = InputBox("Enter the Separator")
        If Separator.Length = 0 Then Exit Sub
        Szunyi.IO.Export.Sequences(ID.Rename(Seqs, StringRename.LastAfterSplit, Separator), False)

    End Sub
    Private Sub Not_Last_Part_After_Splitting(sender As Object, e As EventArgs) Handles ToolStripMenuItem5.Click
        Dim Seqs = Me.GetSeqsFromSelectedList(True)
        If IsNothing(Seqs) = True Then Exit Sub
        Dim Separator = InputBox("Enter the Separator")
        If Separator.Length = 0 Then Exit Sub
        Szunyi.IO.Export.Sequences(ID.Rename(Seqs, StringRename.Not_Last_Part, Separator), False)
    End Sub
    Private Sub Not_First_Part_After_Splitting(sender As Object, e As EventArgs) Handles ToolStripMenuItem7.Click
        Dim Seqs = Me.GetSeqsFromSelectedList(True)
        If IsNothing(Seqs) = True Then Exit Sub
        Dim Separator = InputBox("Enter the Separator")
        If Separator.Length = 0 Then Exit Sub
        Szunyi.IO.Export.Sequences(ID.Rename(Seqs, StringRename.Nor_First_Part, Separator), False)
    End Sub

#End Region
#Region "ReNameFeatures"
    Private Sub FirstPartAfterSplittinToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles FirstPartAfterSplittinToolStripMenuItem.Click
        RenameFeature(Szunyi.Constants.StringRename.FirstAfterSplit)
    End Sub

    Private Sub LastPartAfterSplittingToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles LastPartAfterSplittingToolStripMenuItem.Click
        RenameFeature(Szunyi.Constants.StringRename.LastAfterSplit)
    End Sub

    Public Sub RenameFeature(Subtype As String)
        Dim Seqs = Me.GetSelectedSequenceList
        Dim MergedSeqs = Merging.MergeSequenceList(Seqs)
        Dim Separator = InputBox("Select Separator")
        If Separator.Length = 0 Then Exit Sub
        Dim f1 As New TRYIII(New List(Of String))
        If f1.ShowDialog = DialogResult.OK Then
            Dim alf As Int16 = 54
            Dim x As New Szunyi.Features.FeatureManipulation.Rename(MergedSeqs,
                                                                    f1.SelectedFeatures,
                                                                    f1.SelectedQualifiers,
                                                                    Separator,
                                                                    Subtype,
                                                                    Seqs.Last.ShortFileName)



            CreateBgWork(Szunyi.Constants.BackGroundWork.ModyfiedSequence, x)

        End If
    End Sub


    Private Sub TDTToFastaToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles TDTToFastaToolStripMenuItem.Click
        Dim File = Szunyi.IO.Files.Filter.SelectFile
        Dim SeqIDCol = input.Input.GetIntrestingColumn(File, 1, "Select Column Contains SeqIDS")
        Dim SeqCol = input.Input.GetIntrestingColumn(File, 1, "Select Column Contains Sequences")
        Using sr As New StreamReader(File.FullName)
            sr.ReadLine()
            Dim out As New List(Of Bio.ISequence)
            Do
                Dim s1() = Split(sr.ReadLine, vbTab)
                Dim n As New Bio.Sequence(Alphabets.AmbiguousDNA, s1(SeqCol))
                n.ID = s1(SeqIDCol)
                out.Add(n)
            Loop Until sr.EndOfStream = True
            Szunyi.IO.Export.SaveSequencesToSingleFasta(out)
        End Using

    End Sub

    Private Sub SelectByAffyIDsToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles SelectByAffyIDsToolStripMenuItem.Click
        Dim Mapping = GetSelectedMappings()
        Dim AffyMappings = From x In Mapping Where x.SubType = Szunyi.Constants.BackGroundWork.AffyMapping

        If AffyMappings.Count = 0 Then Exit Sub
        Dim x1 = New Get_List_of_String("Add Interestring string!")
        If x1.ShowDialog = DialogResult.OK Then
            Dim AffyIDs = x1.Strings
            For Each o In AffyMappings
                Dim AffyMapping As Szunyi.Other_Database.Affy.ParseAffy = o
                Dim res = AffyMapping.GetAffyIdsAndGenes(AffyIDs)
                Dim t = AffyMapping.Export(Szunyi.Other_Database.Affy.ExportFormat.OnlyGeneIDs, Nothing, res, 5)
                IO.Export.SaveText(t, "Only Genes")

            Next

        End If

    End Sub

    Private Sub FromFullToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles FromFullToolStripMenuItem.Click
        Dim x As New Szunyi.Other_Database.BioSytem.BioSystemBuilder
        x.CreateSubFromFullDatabase()
    End Sub

    Private Sub CreateFromGenbankToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles CreateFromGenbankToolStripMenuItem.Click
        Dim Seqs = Merging.MergeSequenceList(Me.GetSelectedSequenceList)
        If IsNothing(Seqs) = True Then Exit Sub
        Dim x As New Szunyi.Other_Database.Annotation.AnnotationBuilderOpener
        x.CreateAnnotationForBiosystem(Seqs)
    End Sub


    ''' <summary>
    ''' Get Sequences From Merged Selected Lists Not Cloned
    ''' </summary>
    ''' <returns></returns>
    Private Function GetSeqsFromSelectedList(ShowMsg As Boolean) As List(Of Bio.ISequence)
        Dim SeqList = Me.GetSelectedSequenceList(ShowMsg)
        Dim Seqs = Merging.MergeSequenceList(SeqList)
        Return Seqs
    End Function


    Private Sub FromTabToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles FromTabToolStripMenuItem.Click
        Dim Seqs = Me.GetSeqsFromSelectedList(False)

        Dim X As New Get_List_of_String("First Is Old Seq ID, Seconf Is the New SeqID")
        If X.ShowDialog = DialogResult.OK Then
            Dim s1() = Split(X.TextBox1.Text, vbCrLf)
            Dim tmp As New List(Of Szunyi.Other_Database.CrossRefs.CrossRefOneToOne)
            For Each Item In s1
                Dim s2() = Split(Item, vbTab)
                tmp.Add(New Szunyi.Other_Database.CrossRefs.CrossRefOneToOne(s2.Last, s2.First))
            Next
            Dim Comp As New Szunyi.Other_Database.CrossRefs.ComparerOfCrossrefOneToOneByFirst
            tmp.Sort(Comp)
            For Each Seq In Seqs
                Dim h As New Szunyi.Other_Database.CrossRefs.CrossRefOneToOne(Seq.ID, "")
                Dim Index = tmp.BinarySearch(h, Comp)
                If Index > -1 Then
                    Seq.ID = tmp(Index).Second
                Else
                    Dim alf As Int16 = 43
                End If
            Next
        End If
        Szunyi.IO.Export.SaveSequencesToSingleGenBank(Seqs)
    End Sub

    Private Sub MergeExtFeaturesWithSeqToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles MergeExtFeaturesWithSeqToolStripMenuItem.Click
        Dim Seqs = GetSeqsFromSelectedList(False)
        Dim extFeatureLists = GetSelectedFeatureList()
        If IsNothing(extFeatureLists) = True Then Exit Sub
        For Each extFeatureList In extFeatureLists
            For Each Feature In extFeatureList.Features
                Dim seq = GetSequences.ByID(Seqs, Feature.Seq.ID)
                If IsNothing(seq) = False Then

                    Dim Md = GenBankMetaDataManipulation.GetGenBankMetaDataFromSeq(seq)
                    Md.Features.All.Add(Feature.Feature)
                Else
                    Seqs.Add(GetSequences.ByID(extFeatureList.Seqs, Feature.Seq.ID))

                End If

            Next
        Next
        Szunyi.IO.Export.SaveSequencesToSingleGenBank(Seqs, Nothing, True)
    End Sub

    Private Sub CountFromTDTToolStripMenuItem_Click(sender As Object, e As EventArgs)
        Dim x As New Text.GetCount

    End Sub

    Private Sub FromResultToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles FromResultToolStripMenuItem.Click
        Dim pATH = Szunyi.IO.Directory.Get_Folder
        Dim fiLES = pATH.GetFiles
        Dim X As New Szunyi.Other_Database.Affy.ParseAffy(fiLES.ToList)
        Me.AddToMappingList(X)
    End Sub




    Private Sub KeggToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles KeggToolStripMenuItem.Click
        Dim x As New Szunyi.Other_Database.Kegg.KeggDownloader()
        CreateBgWork("", x)
    End Sub

    Private Sub BriteToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles BriteToolStripMenuItem.Click
        Dim x As New Szunyi.Other_Database.Kegg.BriteDownloader()
        CreateBgWork("", x)
    End Sub

    Private Sub ProcessBriteFilesToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles ProcessBriteFilesToolStripMenuItem.Click
        Dim x As New Szunyi.Other_Database.Kegg.BriteProcess()
        CreateBgWork("", x)
    End Sub

    Private Sub ToTDTToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles ToTDTToolStripMenuItem.Click
        Dim Seqs = Me.GetSeqsFromSelectedList(True)
        If IsNothing(Seqs) = True Then Exit Sub
        Dim str As New StringBuilder
        For Each Seq In Seqs
            str.Append(Seq.ID).Append(vbTab)
            str.Append(Szunyi.Sequences.SequenceManipulation.SeqsToString.GetSeqAsString(Seq)).AppendLine()
        Next
        str.Length -= vbCrLf.Length
        Szunyi.IO.Export.SaveText(str.ToString)
    End Sub

    Private Sub OnlySeqIDsToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles OnlySeqIDsToolStripMenuItem.Click
        Dim SeqLists = Me.GetSelectedSequenceList
        If IsNothing(SeqLists) = True Then Exit Sub
        Dim Seqs = Merging.MergeSequenceList(SeqLists)
        Dim SeqIDs = From x In Seqs Select x.ID

        If SeqIDs.Count > 0 Then
            Dim out = Szunyi.Text.General.GetText(SeqIDs.ToList)
            Clipboard.SetText(out)
            Szunyi.IO.Export.SaveText(out)
        End If
    End Sub


    Private Sub OnlySeqsToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles OnlySeqsToolStripMenuItem.Click
        Dim SeqLists As List(Of SequenceList) = GetSelectedSequenceList()
        If IsNothing(SeqLists) = True Then Exit Sub
        If SeqLists.Count > 1 Then
            Dim x As MsgBoxResult = MsgBox("Do you want to save Separatley", MsgBoxStyle.YesNo)

            If x = MsgBoxResult.Yes Then

                For Each SeqList In SeqLists
                    Dim File As New FileInfo(System.Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) & "\" & SeqList.ShortFileName.Replace(":", "") & ".fa")
                    Dim sSeqs = Szunyi.Sequences.SequenceManipulation.SeqsToString.GetSeqsAsString(SeqList.Sequences)
                    Szunyi.IO.Export.SaveText(Szunyi.Text.General.GetText(sSeqs), File)
                Next
                Exit Sub
            End If
        End If

        Dim AllSeqs = Merging.MergeSequenceList(SeqLists)
        Dim FileII = Szunyi.IO.Files.Save.SelectSaveFile("")
        Dim sSeqsII = Szunyi.Sequences.SequenceManipulation.SeqsToString.GetSeqsAsString(AllSeqs)
        Szunyi.IO.Export.SaveText(Szunyi.Text.General.GetText(sSeqsII), FileII)
    End Sub

    Private Sub SetColorsToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles SetColorsToolStripMenuItem.Click
        Dim h As New Szunyi.Phylogenetic.NewXml
        h.CreateColorSchemas()

    End Sub

    Private Sub ColoringNexmlFilesToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles ColoringNexmlFilesToolStripMenuItem.Click
        Dim h As New Szunyi.Phylogenetic.NewXml
        h.DoIt()

    End Sub

    Private Sub NofIntronToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles NofIntronToolStripMenuItem.Click
        Dim extFeatureLists = GetSelectedFeatureList()
        Dim ExtFeatures = Szunyi.Features.ExtFeatureManipulation.MergeLists(extFeatureLists)
        Dim LTAgs = Szunyi.Features.ExtFeatureManipulation.GetLocusTags(ExtFeatures)
        Dim sLtags = Szunyi.Features.ExtFeatureManipulation.GetShortLocusTags(ExtFeatures)
        Dim NofExons = Szunyi.Features.ExtFeatureManipulation.GetNofExons(ExtFeatures)
        Dim CDSLength = Szunyi.Features.ExtFeatureManipulation.GetCDSLength(ExtFeatures)

        Dim CDSLengthinAA = Szunyi.Features.ExtFeatureManipulation.GetCDSLengthinAA(ExtFeatures)
        Dim SecondExonFrame = Szunyi.Features.ExtFeatureManipulation.Get2ndExonFrames(ExtFeatures)
        Dim str As New System.Text.StringBuilder
        For i1 = 0 To LTAgs.Count - 1
            str.Append(LTAgs(i1).Replace(Chr(34), "")).Append(vbTab)
            str.Append(sLtags(i1).Replace(Chr(34), "")).Append(vbTab)
            str.Append(NofExons(i1)).Append(vbTab)
            str.Append(CDSLength(i1)).Append(vbTab)
            str.Append(CDSLengthinAA(i1)).Append(vbTab)
            str.Append(SecondExonFrame(i1)).AppendLine()
        Next
        Szunyi.IO.Export.SaveText(str.ToString)
    End Sub



    Private Sub AnalysePrimersToolStripMenuItem_Click(sender As Object, e As EventArgs)
        Dim File = Szunyi.IO.Files.Filter.SelectFile
        Dim x As New Szunyi.Primers.Primer3Results(File)
        Dim txt = x.GetResultInTDTShortForm()
        Szunyi.IO.Export.SaveText(txt)
    End Sub

    Private Sub ByGeneToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles ByGeneToolStripMenuItem.Click
        Dim ExtFeatureLists = Me.GetSelectedFeatureList
        Dim LociBuilder As New Bio.IO.GenBank.LocationBuilder


        Dim Genes = Me.Get_Selected_Feature_Lists_By_Type(StandardFeatureKeys.Gene)
        Dim GenesII = Szunyi.Features.ExtFeatureManipulation.MergeLists(Genes)
        GenesII.Sort(AllComparares.ByExtFeatureLocusTag)
        If IsNothing(Genes) = True Then Exit Sub
        Dim CDSs = Me.Get_Selected_Feature_Lists_By_Type(StandardFeatureKeys.CodingSequence)
        If IsNothing(CDSs) = True Then Exit Sub
        Dim CDSsII = Szunyi.Features.ExtFeatureManipulation.MergeLists(CDSs)
        If CDSs.Count = 0 Then Exit Sub
        Dim Comp As New Szunyi.Comparares.OneByOne.ExtFeatureLocusTagComparer

        Dim Out As New SortedList(Of String, Bio.ISequence)
        Dim log As New StringBuilder
        For Each CDS In CDSsII
            Dim sLocusTag = Split(CDS.LocusTag, ".").First
            Dim Gene = Szunyi.Features.ExtFeatureManipulation.GetExtFeutureByLocusTag(GenesII, sLocusTag)
            If IsNothing(Gene) = False Then
                Szunyi.Sequences.SequenceManipulation.CheckAndRepair.CorrectStartAndEndGene(CDS.Feature, Gene.Feature, LociBuilder, log)

                Dim NewSeq = Szunyi.Features.ExtFeatureManipulation.GetSequence(Gene)
                NewSeq.ID = Gene.Feature.Qualifiers(StandardQualifierNames.LocusTag).First.Replace(Chr(34), "")
                If Out.ContainsKey(sLocusTag) = False Then
                    NewSeq.Metadata.Add(Bio.Util.Helper.GenBankMetadataKey, Szunyi.Features.FeatureManipulation.GenBankMetaDataManipulation.CreateNAGenBankMetaData(NewSeq))
                    Out.Add(sLocusTag, NewSeq)
                Else
                    NewSeq = Out(sLocusTag)
                End If

                Dim Md As GenBankMetadata = NewSeq.Metadata(Bio.Util.Helper.GenBankMetadataKey)
                'gene
                Dim FullLoci = LociBuilder.GetLocation(1 & ".." & NewSeq.Count)
                Dim newGene As New Gene(FullLoci)
                Szunyi.Features.FeatureManipulation.MergeFeatures.Merge2Features(CDS.Feature, newGene, True)
                Md.Features.Genes.Add(newGene)
                'mrna
                Dim MdLoc = Szunyi.Features.FeatureManipulation.GetLocations.GetNewLocation(CDS.Feature.Location, Gene.Feature.Location.LocationStart)
                If Gene.Feature.Location.Operator = LocationOperator.Complement = True Then
                    MdLoc = GetLocations.GetNewLocationsReverse(MdLoc.SubLocations.First, NewSeq.Count)
                End If
                Dim MdmRNA1st = LociBuilder.GetLocationString(MdLoc)
                Dim MdmRNA2nd() = Split(MdmRNA1st, "..")
                If MdmRNA2nd.Count > 2 Then
                    Dim k3 As Int16 = 54
                End If

                MdmRNA2nd(0) = MdmRNA2nd(0).Replace(MdLoc.StartData, "1")
                MdmRNA2nd(MdmRNA2nd.Count - 1) = MdmRNA2nd.Last.Replace(MdLoc.EndData, NewSeq.Count)
                Dim mRNALocation = LociBuilder.GetLocation(Szunyi.Text.General.GetText(MdmRNA2nd, ".."))
                Dim newmRNA As New MessengerRna(mRNALocation)
                Szunyi.Features.FeatureManipulation.MergeFeatures.Merge2Features(CDS.Feature, newmRNA, True)
                Md.Features.MessengerRNAs.Add(newmRNA)
                ' CDS
                Dim NewCds As New CodingSequence(MdLoc)
                If Szunyi.Features.FeatureManipulation.Common.IsSameTranslation(CDS, NewSeq, NewCds) = False Then
                    Dim gebasz As Boolean = True
                    Szunyi.Sequences.SequenceManipulation.CheckAndRepair.CorrectStartAndEndGene(CDS.Feature, Gene.Feature, LociBuilder, log)
                    log.Append(NewSeq.ID).AppendLine()
                End If


                Szunyi.Features.FeatureManipulation.MergeFeatures.Merge2Features(CDS.Feature, NewCds, True)
                Md.Features.All.Add(NewCds)
            Else
                log.Append(sLocusTag).AppendLine()
            End If

        Next ' Next CDS
        If log.Length > 0 Then
            Dim alf As Int16 = 54
        End If

        Dim k = From f In Out Select f.Value

        Dim j = k.ToList

        Szunyi.IO.Export.SaveSequencesToSingleGenBank(j)
    End Sub
    Private Function GetModifiedLocation(Locations As List(Of Bio.IO.GenBank.ILocation),
                                             Feature As Bio.IO.GenBank.FeatureItem) As Bio.IO.GenBank.Location
        Dim str As New System.Text.StringBuilder
        Dim LociBuilder As New Bio.IO.GenBank.LocationBuilder
        Select Case Locations.Count
            Case 1
                If Locations.First.Operator = Bio.IO.GenBank.LocationOperator.Join Then
                    Return GetModifiedLocation(Locations.First.SubLocations, Feature)
                Else
                    str.Append(1)
                    str.Append("..")
                    str.Append(Locations.First.LocationEnd - Locations.First.LocationStart + 1)
                End If
            Case Else
                str.Append("join(")
                If Locations.First.LocationStart > Locations.Last.LocationStart Then ' if complement
                    Dim LengthOfFeature = Feature.Location.LocationEnd - Feature.Location.LocationStart
                    For Each sLocation In Locations
                        Dim sStart = LengthOfFeature - (sLocation.LocationEnd - Feature.Location.LocationStart) + 1
                        str.Append(sStart)
                        str.Append("..")
                        str.Append(sStart + sLocation.LocationEnd - sLocation.LocationStart)
                        str.Append(",")
                    Next
                Else
                    For Each sLocation In Locations
                        str.Append(sLocation.LocationStart - Feature.Location.LocationStart + 1)
                        str.Append("..")
                        str.Append(sLocation.LocationEnd - Feature.Location.LocationStart + 1)
                        str.Append(",")
                    Next
                End If
                str.Length -= 1
                str.Append(")")
        End Select
        Return LociBuilder.GetLocation(str.ToString)

    End Function

    Private Sub CreateDIr(Path As String, SubDir As String)
        Dim x As New DirectoryInfo(Path & "\" & SubDir)
        If x.Exists = False Then x.Create()
    End Sub

    Private Sub FirstToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles FirstToolStripMenuItem.Click
        Dim Files = Szunyi.IO.Files.Filter.SelectFiles
        Szunyi.Text.Table.DeleteFirstColumns(Files)
    End Sub

    Private Sub LastToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles LastToolStripMenuItem.Click
        Dim Files = Szunyi.IO.Files.Filter.SelectFiles
        Szunyi.Text.Table.DeleteLastColumns(Files)

    End Sub

    Private Sub CustomToolStripMenuItem2_Click(sender As Object, e As EventArgs) Handles CustomToolStripMenuItem2.Click

    End Sub


    Private Sub PerfectMatchToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles PerfectMatchToolStripMenuItem.Click
        Dim IDFIle = Szunyi.IO.Files.Filter.SelectFile("File Contain SeqIDs")
        Dim IDs = Split(Szunyi.IO.Import.Text.ReadToEnd(IDFIle), vbCrLf).ToList
        IDs.Sort()
        Dim ReadFile = Szunyi.IO.Files.Filter.SelectFile("Select File Containing The Reads")
        Dim Out As New List(Of ISequence)
        For Each Seq In Szunyi.IO.Import.Sequence.GetOnyByONe(ReadFile)
            Dim Index = IDs.BinarySearch(Seq.ID)
            If Index > -1 Then
                Out.Add(Seq)
            End If
        Next
        Szunyi.IO.Export.SaveSequencesToSingleFasta(Out)
    End Sub

    Private Sub StartWithsToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles StartWithsToolStripMenuItem.Click
        Dim IDFIle = Szunyi.IO.Files.Filter.SelectFile("File Contain SeqIDs")
        Dim IDs = Split(Szunyi.IO.Import.Text.ReadToEnd(IDFIle), vbCrLf).ToList
        IDs.Sort()
        Dim ReadFile = Szunyi.IO.Files.Filter.SelectFile("Select File Containing The Reads")
        Dim Out As New List(Of ISequence)
        For Each Seq In Szunyi.IO.Import.Sequence.GetOnyByONe(ReadFile)
            For Each ID In IDs
                If Seq.ID.StartsWith(ID) Then Out.Add(Seq)
            Next
        Next
        Szunyi.IO.Export.SaveSequencesToSingleFasta(Out)
    End Sub

    Private Sub SplitSequencesToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles SplitSequencesToolStripMenuItem.Click
        Dim NofSeqPerFile = Szunyi.MyInputBox.GetInteger("Nof max Sequence!")
        If IsNothing(NofSeqPerFile) = True Then Exit Sub
        Dim Seqs = Me.GetSeqsFromSelectedList(True)
        Dim DefFile = Szunyi.IO.Files.Save.SelectSaveFile(Szunyi.Constants.Files.SequenceFileTypeToSave)

        Dim Index As Integer = 0
        For Each Group In Szunyi.Sequences.SequenceManipulation.Common.GetSeqGroupsByCount(Seqs, NofSeqPerFile)
            Dim NewFile = Szunyi.IO.Files.Get_New_FileName.Replace_Extension(DefFile, "_" & Index)
            Szunyi.IO.Export.SaveSequencesToSingleFasta(Group, NewFile)
            Index += 1
        Next
    End Sub

    Private Sub GeneWGenomicRegionToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles GeneWGenomicRegionToolStripMenuItem.Click
        Dim f As New InputForms.SettingOfTranscriptPromoterUTR
        If f.ShowDialog <> DialogResult.OK Then Exit Sub

        Dim Out As New List(Of Bio.ISequence)
        Dim Seqs = Me.GetSeqsFromSelectedList(False)

    End Sub


    Private Sub GetOldIDToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles GetOldIDToolStripMenuItem.Click
        Dim seqs = Me.GetSeqsFromSelectedList(True)
        Dim str As New StringBuilder
        For Each Seq In seqs
            Dim MD As Bio.IO.GenBank.GenBankMetadata = Seq.Metadata(Bio.Util.Helper.GenBankMetadataKey)

            For Each S In MD.Comments
                If S.Contains("The reference sequence is identical to ") Then
                    Dim s1() = Split(S, "The reference sequence is identical to ")
                    Dim s2() = Split(s1.Last, ".")
                    str.Append(Seq.ID).Append(vbTab).Append(s2.First).AppendLine()
                End If
            Next
        Next
        Szunyi.IO.Export.SaveText(str.ToString)
    End Sub




    Private Sub GffToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles GffToolStripMenuItem.Click
        Dim Seqs = Me.GetSeqsFromSelectedList(False)
        If IsNothing(Seqs) = True Then Exit Sub
        Dim SaveFile = Szunyi.IO.Files.Save.SelectSaveFile(Szunyi.Constants.Files.gff3, "")
        Dim g As New Szunyi.Sequences.Gff.GFFFormatter(Seqs, SaveFile)
        g.DoIt()
    End Sub

    Private Sub CvsToTdtToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles CvsToTdtToolStripMenuItem.Click
        Dim Files = Szunyi.IO.Files.Filter.SelectFiles("Select Files", ClassLibrary1.Szunyi.Constants.Files.Other.csv)
        Szunyi.IO.FilesAndFolder.Convert.CSV_to_TDT(Files)

    End Sub

    Private Sub MergeToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles MergeToolStripMenuItem.Click
        Dim Files = Szunyi.IO.Files.Filter.SelectFiles("", "Select Files")
        Dim SaveFIle = Szunyi.IO.Files.Save.SelectSaveFile("", "")
        Szunyi.IO.FilesAndFolder.Convert.Merge_Files(Files, SaveFIle)

    End Sub

    Private Sub LongerToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles LongerToolStripMenuItem.Click
        Dim Seqs = GetSeqsFromSelectedList(False)
        If IsNothing(Seqs) = True Then Exit Sub
        Dim t = GetSelectedSequenceList()
        If IsNothing(Seqs) = True Then Exit Sub
        Dim Length = Szunyi.MyInputBox.GetInteger("Select Minimum Length")
        Dim nSeqs = Szunyi.Sequences.SequenceManipulation.SelectBy.LongerThan(Seqs, Length)
        Dim s = InputBox("Enter the Title")
        Dim x As New ListOf.SequenceList(nSeqs, s, "LT " & Length & t.First.ShortFileName)
        AddToSequenceList(x)

    End Sub

    Private Sub ShorterToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles ShorterToolStripMenuItem.Click
        Dim Seqs = GetSeqsFromSelectedList(False)
        If IsNothing(Seqs) = True Then Exit Sub
        Dim t = GetSelectedSequenceList()
        If IsNothing(Seqs) = True Then Exit Sub
        Dim Length = Szunyi.MyInputBox.GetInteger("Select Maximum Length")
        Dim nSeqs = Szunyi.Sequences.SequenceManipulation.SelectBy.ShorterThan(Seqs, Length)
        Dim s = InputBox("Enter the Title")
        Dim x As New ListOf.SequenceList(nSeqs, s, "ST " & Length & t.First.ShortFileName)
        AddToSequenceList(x)
    End Sub

    Private Sub BetweenToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles BetweenToolStripMenuItem.Click
        Dim Seqs = GetSeqsFromSelectedList(False)
        If IsNothing(Seqs) = True Then Exit Sub
        Dim t = GetSelectedSequenceList()
        Dim Min = Szunyi.MyInputBox.GetInteger("Select Minimum Length")
        Dim Max = Szunyi.MyInputBox.GetInteger("Select Maximum Length")
        Dim nSeqs = Szunyi.Sequences.SequenceManipulation.SelectBy.LengthBetween(Seqs, Max, Min)
        Dim s = InputBox("Enter the Title")
        Dim x As New ListOf.SequenceList(nSeqs, s, "LB Min " & Min & "-Max" & Max & " " & t.First.ShortFileName)
        AddToSequenceList(x)
    End Sub

    Private Sub ByCountToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles ByCountToolStripMenuItem.Click
        Dim Seqs = Me.GetSeqsFromSelectedList(True)
        If IsNothing(Seqs) = True Then Exit Sub

        Dim t = GetSelectedSequenceList()
        Dim Min = Szunyi.MyInputBox.GetInteger("Select Minimum Count")
        Dim Max = Szunyi.MyInputBox.GetInteger("Select Maximum Count")
        If Max = 0 Then Exit Sub

        Dim Motif = InputBox("Enter the motif")
        If IsNothing(Motif) = True Then Exit Sub
        If Motif = String.Empty Then Exit Sub
        Dim nSeqs = AA.GetSeqsAACountsBetween(Seqs, Motif.First, Min, Max)
        Dim s = InputBox("Enter the Title")
        Dim x As New ListOf.SequenceList(nSeqs, s, "LB Min " & Min & "-Max" & Max & " " & t.First.ShortFileName)
        AddToSequenceList(x)
    End Sub





#End Region

#End Region

#End Region

#Region "SeqGroups"
#Region "GetUnique"
    ' Unique By ID
    Private Sub ByIDToolStripMenuItem2_Click(sender As Object, e As EventArgs) Handles ByIDToolStripMenuItem2.Click
        Dim x As New SeqGroup(Me.GetSelectedSequenceList, Szunyi.Constants.SeqGroups.UniqueByID)
        Me.AddToSequenceList(x.Result)

    End Sub

    Private Sub BySeqToolStripMenuItem2_Click(sender As Object, e As EventArgs) Handles BySeqToolStripMenuItem2.Click
        Dim x As New SeqGroup(Me.GetSelectedSequenceList, Szunyi.Constants.SeqGroups.UniqueBySeq)
        Me.AddToSequenceList(x.Result)

    End Sub

    Private Sub BySeqAndIDToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles BySeqAndIDToolStripMenuItem.Click
        Dim Seqs = Me.GetSelectedSequenceList
        If IsNothing(Seqs) = True Then Exit Sub
        Dim x As New SeqGroup(Me.GetSelectedSequenceList, Szunyi.Constants.SeqGroups.UniqueByIDAndSeq)
        Me.AddToSequenceList(x.Result)

    End Sub
#End Region
#Region "GetDuplicated"
    Private Sub ByIDToolStripMenuItem3_Click(sender As Object, e As EventArgs) Handles ByIDToolStripMenuItem3.Click
        Dim x As New SeqGroup(Me.GetSelectedSequenceList, Szunyi.Constants.SeqGroups.DuplicatedByID)
        Me.AddToSequenceList(x.Result)
    End Sub

    Private Sub BySeqToolStripMenuItem3_Click(sender As Object, e As EventArgs) Handles BySeqToolStripMenuItem3.Click
        Dim x As New SeqGroup(Me.GetSelectedSequenceList, Szunyi.Constants.SeqGroups.DuplicatedBySeq)
        Me.AddToSequenceList(x.Result)

    End Sub

    Private Sub BySeqAndIDToolStripMenuItem1_Click(sender As Object, e As EventArgs) Handles BySeqAndIDToolStripMenuItem1.Click
        Dim x As New SeqGroup(Me.GetSelectedSequenceList, Szunyi.Constants.SeqGroups.DuplicatedByIDAndSeq)
        Me.AddToSequenceList(x.Result)
    End Sub
#End Region
#Region "OneCopy"

    Private Sub ByIDToolStripMenuItem4_Click(sender As Object, e As EventArgs) Handles ByIDToolStripMenuItem4.Click
        Dim x As New SeqGroup(Me.GetSelectedSequenceList, Szunyi.Constants.SeqGroups.OneCopyByID)
        Me.AddToSequenceList(x.Result)
    End Sub

    Private Sub BySeqToolStripMenuItem4_Click(sender As Object, e As EventArgs) Handles BySeqToolStripMenuItem4.Click
        Dim x As New SeqGroup(Me.GetSelectedSequenceList, Szunyi.Constants.SeqGroups.OneCopyBySeq)
        Me.AddToSequenceList(x.Result)
    End Sub

    Private Sub BySeqAndIDToolStripMenuItem2_Click(sender As Object, e As EventArgs) Handles BySeqAndIDToolStripMenuItem2.Click
        Dim x As New SeqGroup(Me.GetSelectedSequenceList, Szunyi.Constants.SeqGroups.OneCopyByIDAndSeq)
        Me.AddToSequenceList(x.Result)
    End Sub
    Private Sub Split_Seqs_byAAPos_OneByOne_Click(sender As Object, e As EventArgs) Handles Split_Seqs_byAAPos_OneByOne.Click
        Dim Seqs = Me.GetSeqsFromSelectedList(True)
        If IsNothing(Seqs) = True Then Exit Sub
        Dim s = InputBox("Positions Separated by space")
        Dim Positions As New List(Of Integer)

        For Each Pos In Split(s, " ")
            Dim i As Integer
            If Integer.TryParse(Pos, i) = True Then
                Positions.Add(i)
            End If
        Next
        Dim Folder = Szunyi.IO.Directory.Get_Folder
        For Each t In Szunyi.Sequences.SequenceManipulation.Common.GetSeqGroupsByAA_Pos(Seqs, Positions)
            If Seqs.First.Alphabet Is Bio.Alphabets.DNA Or Seqs.First.Alphabet Is Bio.Alphabets.AmbiguousDNA Then

                Dim tr = Szunyi.Translate.TranaslateSeqs(t.Value, True)
                Dim nFIle As New FileInfo((Folder.FullName & "\" & t.Key & "_NA.fa").Replace("\\", "\"))
                Szunyi.IO.Export.SaveSequencesToSingleFasta(t.Value, nFIle)
                Dim nFile_AA As New FileInfo((Folder.FullName & "\" & t.Key & "_AA.fa").Replace("\\", "\"))
                Szunyi.IO.Export.SaveSequencesToSingleFasta(tr, nFile_AA)
            ElseIf Seqs.First.Alphabet Is Bio.Alphabets.Protein Or Seqs.First.Alphabet Is Bio.Alphabets.AmbiguousProtein Then
                Dim nFile_AA As New FileInfo((Folder.FullName & "\" & t.Key & "_AA.fa").Replace("\\", "\"))
                Szunyi.IO.Export.SaveSequencesToSingleFasta(Seqs, nFile_AA)
            End If

        Next
    End Sub

    Private Sub Split_Seqs_byAAPos_Combined_Click(sender As Object, e As EventArgs) Handles Split_Seqs_byAAPos_Combined.Click
        Dim Seqs = Me.GetSeqsFromSelectedList(True)
        If IsNothing(Seqs) = True Then Exit Sub

    End Sub
    Private Sub SplitBy_NofSequencesToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles NofSequencesToolStripMenuItem.Click
        Dim Seqs = Me.GetSeqsFromSelectedList(True)
        If IsNothing(Seqs) = True Then Exit Sub
        Dim NofSeq As Integer = Szunyi.MyInputBox.GetInteger("Nof Seq Per File")
        Dim Index As Integer = 0
        Dim s = InputBox("Base Name Of Splitted SeqGroup")
        For Each t In Szunyi.Sequences.SequenceManipulation.Common.GetSeqGroupsByCount(Seqs, NofSeq)
            Dim x As New Szunyi.ListOf.SequenceList(t, s & "_" & Index, s & "_" & Index)
            Index += 1
            Me.AddToSequenceList(x)
        Next
    End Sub
    Private Sub SplitByFIleLengthToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles ByFIleLengthToolStripMenuItem.Click
        Dim Seqs = Me.GetSeqsFromSelectedList(True)
        If IsNothing(Seqs) = True Then Exit Sub
        Dim Nof_Residues As Integer = Szunyi.MyInputBox.GetInteger("Length of File")
        Dim Index As Integer = 0
        Dim s = InputBox("Base Name Of Splitted SeqGroup")
        For Each t In Szunyi.Sequences.SequenceManipulation.Common.GetSeqGroupsBy_FileLength(Seqs, Nof_Residues)
            Dim x As New Szunyi.ListOf.SequenceList(t, s & "_" & Index, s & "_" & Index)
            Index += 1
            Me.AddToSequenceList(x)
        Next
    End Sub
    Private Sub SplitBy_NofResiduesToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles NofResiduesToolStripMenuItem.Click
        Dim Seqs = Me.GetSeqsFromSelectedList(True)
        If IsNothing(Seqs) = True Then Exit Sub
        Dim Nof_Residues As Integer = Szunyi.MyInputBox.GetInteger("Nof Residues Per File")
        Dim Index As Integer = 0
        Dim s = InputBox("Base Name Of Splitted SeqGroup")
        For Each t In Szunyi.Sequences.SequenceManipulation.Common.GetSeqGroupsByNof_Residues(Seqs, Nof_Residues)
            Dim x As New Szunyi.ListOf.SequenceList(t, s & "_" & Index, s & "_" & Index)
            Index += 1
            Me.AddToSequenceList(x)
        Next
    End Sub
    Private Sub SplitByNumberToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles SplitByNumberToolStripMenuItem.Click



    End Sub

    Private Sub SignalPAnalysisToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles SignalPAnalysisToolStripMenuItem.Click
        Dim Seqs = Me.GetSeqsFromSelectedList(False)
        Dim HtmlFiles = Szunyi.IO.Files.Filter.SelectFiles()
        Dim SignalP As New Szunyi.Other_Database.SignalP.SignalPBuilder(Seqs, HtmlFiles)
        SignalP.Save()
    End Sub

    Private Sub ExactSearchToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles ExactSearchToolStripMenuItem.Click
        Dim Seqs = Me.GetSeqsFromSelectedList(True)
        If IsNothing(Seqs) = True Then Exit Sub
        Dim Files = Szunyi.IO.Files.Filter.SelectFiles("Select Short Sequences as Fasta", Szunyi.Constants.Files.Fasta)
        Dim MotifSeqs = Szunyi.IO.Import.Sequence.FromFiles(Files)
        Dim str As New System.Text.StringBuilder
        str.Append("SeqID").Append(vbTab).Append("MotifID").AppendLine()
        Dim NofProcessedSeq As Integer = 0
        For Each Seq In Seqs
            For Each MotifSeq In MotifSeqs
                Dim t = Szunyi.Sequences.SequenceManipulation.UniqueDistinct.SimpleBoyerMooreSearch(MotifSeq, Seq)
                If t > -1 Then
                    str.Append(Seq.ID).Append(vbTab).Append(MotifSeq.ID).AppendLine()
                Else
                    Dim SSeq As Bio.ISequence = Seq.GetReverseComplementedSequence()
                    t = Szunyi.Sequences.SequenceManipulation.UniqueDistinct.SimpleBoyerMooreSearch(MotifSeq, SSeq)
                    If t > -1 Then
                        str.Append(Seq.ID).Append(vbTab).Append(MotifSeq.ID).AppendLine()
                    End If
                End If
            Next
            NofProcessedSeq += 1
        Next
        If str.Length > 15 Then str.Length -= 2
        Szunyi.IO.Export.SaveText(str.ToString)
    End Sub



#End Region

#End Region

#Region "Locations And Counts"

    Private Sub ExtendLocationsToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles ExtendLocationsToolStripMenuItem.Click

        Dim Locations = Me.GetSelectedLocationList(True)
        If IsNothing(Locations) = True Then Exit Sub
        Dim Extension_Lengths = Szunyi.MyInputBox.GetIntegers("Select Lengths Separated By Space")

        Dim Extended_Locations As List(Of Szunyi.Location.LocationList) = Szunyi.Location.LocationManipulation.Get_Extended_Locations(Extension_Lengths, Locations)
        For Each nLoci In Extended_Locations
            AddToLocations(nLoci)
        Next
    End Sub

#End Region

#Region "Item With Properties Table And Text"

    Private Sub ItemWithPropertiesToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles ItemWithPropertiesToolStripMenuItem.Click
        Dim sg = GetSelected_Items_With_Propeties(True)

        For Each Item In sg
            Dim Newfile = Szunyi.IO.Files.Save.SelectSaveFile("", Item.Title)
            Item.Save_WithOut_ID(Newfile)
        Next
    End Sub

    Private Sub TableManipulationToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles TableManipulationToolStripMenuItem.Click
        Dim x As New TableManipulation
        If x.ShowDialog() = DialogResult.OK Then
            Dim TheFiles() = x.Files.ToArray
            Dim log As New List(Of String)
            Parallel.ForEach(TheFiles, Sub(TheFile)
                                           Thread.CurrentThread.CurrentCulture = CultureInfo.CreateSpecificCulture("en-US")
                                           Thread.CurrentThread.CurrentUICulture = CultureInfo.CreateSpecificCulture("en-US")
                                           Dim t As New Szunyi.Text.TableManipulation.Items_With_Properties(TheFile, 0, Nothing, log, Nothing, False, 1)
                                           t.DoIt(True)
                                           Dim settings = x.Table_Manipulation_Settings.clone
                                           t = t.Filter(settings, 0, settings.Last_Origianal_Column)
                                           '      t = t.Calculate(settings)
                                           t = t.Filter(settings, settings.Last_Origianal_Column + 1, t.ItemHeaders.Count - 1)

                                           t = t.DeleteColumns(settings)

                                           t.Save_WithOut_ID(New FileInfo(x.SaveFolder & "\" & TheFile.Name))
                                       End Sub)

        End If
    End Sub

#End Region

#Region "Protein"
#Region "DisOrders"
    Private Sub Export_SequencesWithMotifsToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles SequencesWithMotifsToolStripMenuItem.Click
        Dim sg = GetSelected_Sequences_With_Motifs(True)

        For Each Item In sg
            Szunyi.Sequence_Analysis.Sequences_Ranges.Save(Item)
        Next
    End Sub
    Private Function Get_Seq_w_Motifs_List(Type As String) As List(Of Szunyi.Sequence_Analysis.Sequences_Ranges)
        Dim S_With_Motifs_List = Me.GetSelected_Sequences_With_Motifs(False)
        If S_With_Motifs_List.Count = 0 Then
            Dim SeqLists = Me.GetSelectedSequenceList()
            If IsNothing(SeqLists) = True Then Return S_With_Motifs_List
            For Each SeqList In SeqLists
                S_With_Motifs_List.Add(New Szunyi.Sequence_Analysis.Sequences_Ranges(SeqList, Type))

            Next
            Return S_With_Motifs_List
        End If
        For Each l In S_With_Motifs_List
            l.Title = "SA " & Type & l.Title
        Next
        Return S_With_Motifs_List
    End Function
    Private Sub Add_To_Seqs_with_Motifs_Is_Needed(S_With_Motifs_List As List(Of Szunyi.Sequence_Analysis.Sequences_Ranges))
        Dim tmp = Me.GetSelected_Sequences_With_Motifs(False)

        For Each S_W_MOtif In S_With_Motifs_List
            If IsNothing(tmp) = True Then
                Me.AddToSequences_With_Motifs(S_W_MOtif)
            Else
                If tmp.Contains(S_W_MOtif) = False Then
                    Me.AddToSequences_With_Motifs(S_W_MOtif)
                Else
                    Dim Item = From h In Me.SelectedItems Where h.Type = Szunyi.Constants.BackGroundWork.Sequences_With_Motifs And h.ID = S_W_MOtif.UniqueID

                    If Item.Count <> 0 Then Me.SelectedItems.Remove(Item.First)

                    Dim sg = From t In ListOfs.ListOf_Sequences_With_Motifs Where t.UniqueID = S_W_MOtif.UniqueID

                    If sg.Count <> 0 Then ListOfs.ListOf_Sequences_With_Motifs.Remove(sg.First)
                    Me.AddToSequences_With_Motifs(S_W_MOtif)



                End If
            End If
        Next
    End Sub

    Private Sub Find_All_DisOrders_Click(sender As Object, e As EventArgs) Handles Find_All_Disorder.Click
        Dim S_With_Motifs_List = Get_Seq_w_Motifs_List("All DisOrders")

        Dim a As New Szunyi.Protein.AGGRESCAN()
        For Each S_W_MOtif In S_With_Motifs_List
            S_W_MOtif = a.Get_Results(S_W_MOtif)
        Next
        Dim d As New Szunyi.Protein.DisEMBL()
        For Each S_W_MOtif In S_With_Motifs_List
            S_W_MOtif = d.Get_results(S_W_MOtif)
        Next
        Dim g As New Szunyi.Protein.GloboProt()
        For Each S_W_MOtif In S_With_Motifs_List
            S_W_MOtif = g.Get_Results(S_W_MOtif)
        Next
        Add_To_Seqs_with_Motifs_Is_Needed(S_With_Motifs_List)



    End Sub

    Private Sub Find_DisOrders_By_AggreScan_Click(sender As Object, e As EventArgs) Handles FInd_Disorders_By_AggrScan.Click
        Dim S_With_Motifs_List = Get_Seq_w_Motifs_List("AggreScan")
        Dim tmp = Me.GetSelected_Sequences_With_Motifs(False)

        Dim d As New Szunyi.Protein.AGGRESCAN()
        For Each S_W_MOtif In S_With_Motifs_List
            S_W_MOtif = d.Get_Results(S_W_MOtif)
        Next

        Add_To_Seqs_with_Motifs_Is_Needed(S_With_Motifs_List)

    End Sub

    Private Sub Find_DisOrders_By_DisEMBL_Click(sender As Object, e As EventArgs) Handles Find_Disorders_By_DIsEMBL.Click
        Dim S_With_Motifs_List = Get_Seq_w_Motifs_List("DisEMBL")
        Dim tmp = Me.GetSelected_Sequences_With_Motifs(False)

        Dim d As New Szunyi.Protein.DisEMBL()
        For Each S_W_MOtif In S_With_Motifs_List
            S_W_MOtif = d.Get_results(S_W_MOtif)
        Next
        Add_To_Seqs_with_Motifs_Is_Needed(S_With_Motifs_List)

    End Sub

    Private Sub Find_DisOrders_By_GloboProt_Click(sender As Object, e As EventArgs) Handles Find_DisOrders_By_GloboProt.Click
        Dim S_With_Motifs_List = Get_Seq_w_Motifs_List("GloboProt")
        Dim tmp = Me.GetSelected_Sequences_With_Motifs(False)

        Dim d As New Szunyi.Protein.GloboProt()
        For Each S_W_MOtif In S_With_Motifs_List
            S_W_MOtif = d.Get_Results(S_W_MOtif)
        Next
        Add_To_Seqs_with_Motifs_Is_Needed(S_With_Motifs_List)

    End Sub
#End Region

#Region "Protein Basic Paramaters"
    Private Function Get_Ext_I_w_Ps() As Szunyi.Text.TableManipulation.Ext_Items_With_Properties
        Dim res As New Szunyi.Text.TableManipulation.Ext_Items_With_Properties
        Dim Seqs = Me.GetSeqsFromSelectedList(True)
        If IsNothing(Seqs) = True Then Return Nothing
        Dim SeqLists = Me.GetSelectedSequenceList
        Dim Titles = From x In SeqLists Select x.ShortFileName

        res.Seqs = Seqs
        res.Title = Szunyi.Text.General.GetText(Titles.ToList, " ")
        res.I_w_Ps = New Szunyi.Text.TableManipulation.Items_With_Properties(Seqs)
        Return res
    End Function
    Private Sub Protein_Paramaters_All(sender As Object, e As EventArgs) Handles Protein_Parameters_All.Click
        Dim I_W_Ps = Get_Ext_I_w_Ps()

        Dim hi As New Szunyi.Protein.Hidrophobicity
        Dim Hidrophobicity_Values = hi.Get_HidroHidrophobicity_Indexes(I_W_Ps.Seqs)
        I_W_Ps.I_w_Ps.Add_Values_WithOut_Keys("Hidrophaticity Index", Hidrophobicity_Values)

        Dim ii As New Szunyi.Protein.Instability
        Dim Instability_Values = ii.Get_Instability_Indexes(I_W_Ps.Seqs)
        I_W_Ps.I_w_Ps.Add_Values_WithOut_Keys("Instability Index", Instability_Values)

        Dim MW As New Szunyi.Protein.MW
        Dim MWs = MW.GetMWs(I_W_Ps.Seqs)
        I_W_Ps.I_w_Ps.Add_Values_WithOut_Keys("MW", MWs)

        Dim PI As New Szunyi.Protein.PI
        Dim PIs = PI.Get_PIs(I_W_Ps.Seqs)
        I_W_Ps.I_w_Ps.Add_Values_WithOut_Keys("PI", PIs)

        Dim Aliphatic As New Szunyi.Protein.Aliphatic_Index
        Dim Aliphatic_Values = Aliphatic.Get_Aliphatic_Indexes(I_W_Ps.Seqs)
        I_W_Ps.I_w_Ps.Add_Values_WithOut_Keys("Aliphatic Index", Aliphatic_Values)

        Dim EC As New Szunyi.Protein.Extinction_Coefficient
        Dim ECs_Reduced = EC.Get_Excitation_Fully_Reduced_Cysteines(I_W_Ps.Seqs)
        I_W_Ps.I_w_Ps.Add_Values_WithOut_Keys("Ext. coefficient Fully Reduced", ECs_Reduced)

        Dim ECs_Oxidized = EC.Get_Excitation_Oxidized_Cysteines(I_W_Ps.Seqs)
        I_W_Ps.I_w_Ps.Add_Values_WithOut_Keys("Ext. coefficient Oxidized", ECs_Oxidized)

        Dim Abs_Reduced = EC.Get_Absorbances_Fully_Reduced_Cysteines(I_W_Ps.Seqs)
        I_W_Ps.I_w_Ps.Add_Values_WithOut_Keys("Absorbance Fully Reduced", Abs_Reduced)

        Dim Abs_Oxidized = EC.Get_Absorbances_Oxidized_Cysteines(I_W_Ps.Seqs)
        I_W_Ps.I_w_Ps.Add_Values_WithOut_Keys("Absorbance Oxidized", Abs_Oxidized)

        Dim PS As New Szunyi.Protein.Protein_Stability
        Dim Ps_Human = PS.Get_Half_Lifes(I_W_Ps.Seqs, "Mammalian")
        I_W_Ps.I_w_Ps.Add_Values_WithOut_Keys("Mammalian Half Life", Ps_Human)

        Dim Ps_Yeast = PS.Get_Half_Lifes(I_W_Ps.Seqs, "Yeast")
        I_W_Ps.I_w_Ps.Add_Values_WithOut_Keys("Yeast Half Life", Ps_Yeast)

        Dim Ps_Coli = PS.Get_Half_Lifes(I_W_Ps.Seqs, "E. Coli")
        I_W_Ps.I_w_Ps.Add_Values_WithOut_Keys("E. Coli Half Life", Ps_Coli)

        I_W_Ps.I_w_Ps.Title = "Prot Param All" & I_W_Ps.Title

        Dim AAs = Szunyi.Protein.Amino_Acid_Composition_Manipulation.Get_AA_Compositions(I_W_Ps.Seqs)
        For i = 0 To I_W_Ps.Seqs.Count - 1
            Dim Seq = AAs(i)
            If i = 0 Then
                For Each Item In Seq.Counts
                    I_W_Ps.I_w_Ps.ItemHeaders.Add("Nof: " & ChrW(Item.Key))
                Next
                For Each Item In Seq.Counts
                    I_W_Ps.I_w_Ps.ItemHeaders.Add("%:" & ChrW(Item.Key))
                Next
            End If
            For Each Item In Seq.Counts
                I_W_Ps.I_w_Ps.Items(i).Properties.Add(Item.Value)
            Next
            For Each Item In Seq.Percents
                I_W_Ps.I_w_Ps.Items(i).Properties.Add(Item.Value)
            Next
        Next

        Me.AddTo_Item_With_Properties(I_W_Ps.I_w_Ps)

    End Sub

    Private Sub Protein_Parameters_Absorbance_excitation_Click(sender As Object, e As EventArgs) Handles Protein_Parameters_Absorbance_excitation.Click
        Dim I_W_Ps = Get_Ext_I_w_Ps()

        Dim EC As New Szunyi.Protein.Extinction_Coefficient
        Dim ECs_Reduced = EC.Get_Excitation_Fully_Reduced_Cysteines(I_W_Ps.Seqs)
        I_W_Ps.I_w_Ps.Add_Values_WithOut_Keys("Ext. coefficient Fully Reduced", ECs_Reduced)

        Dim ECs_Oxidized = EC.Get_Excitation_Oxidized_Cysteines(I_W_Ps.Seqs)
        I_W_Ps.I_w_Ps.Add_Values_WithOut_Keys("Ext. coefficient Oxidized", ECs_Oxidized)

        Dim Abs_Reduced = EC.Get_Absorbances_Fully_Reduced_Cysteines(I_W_Ps.Seqs)
        I_W_Ps.I_w_Ps.Add_Values_WithOut_Keys("Absorbance Fully Reduced", Abs_Reduced)

        Dim Abs_Oxidized = EC.Get_Absorbances_Oxidized_Cysteines(I_W_Ps.Seqs)
        I_W_Ps.I_w_Ps.Add_Values_WithOut_Keys("Absorbance Oxidized", Abs_Oxidized)

        I_W_Ps.I_w_Ps.Title = "Absorbance" & I_W_Ps.Title
        Me.AddTo_Item_With_Properties(I_W_Ps.I_w_Ps)

    End Sub

    Private Sub Protein_Parameters_Instability_Click(sender As Object, e As EventArgs) Handles Protein_Parameters_Instability.Click
        Dim I_W_Ps = Get_Ext_I_w_Ps()

        Dim ii As New Szunyi.Protein.Instability
        Dim Instability_Values = ii.Get_Instability_Indexes(I_W_Ps.Seqs)
        I_W_Ps.I_w_Ps.Add_Values_WithOut_Keys("Instability Index", Instability_Values)

        I_W_Ps.I_w_Ps.Title = "Instability" & I_W_Ps.Title
        Me.AddTo_Item_With_Properties(I_W_Ps.I_w_Ps)

    End Sub

    Private Sub Protein_Parameters_MW_Click(sender As Object, e As EventArgs) Handles Protein_Parameters_MW.Click
        Dim I_W_Ps = Get_Ext_I_w_Ps()

        Dim MW As New Szunyi.Protein.MW
        Dim MWs = MW.GetMWs(I_W_Ps.Seqs)
        I_W_Ps.I_w_Ps.Add_Values_WithOut_Keys("MW", MWs)

        I_W_Ps.I_w_Ps.Title = "MW" & I_W_Ps.Title
        Me.AddTo_Item_With_Properties(I_W_Ps.I_w_Ps)
    End Sub

    Private Sub Protein_Parameters_PI_Click(sender As Object, e As EventArgs) Handles Protein_Parameters_PI.Click
        Dim I_W_Ps = Get_Ext_I_w_Ps()

        Dim PI As New Szunyi.Protein.PI
        Dim PIs = PI.Get_PIs(I_W_Ps.Seqs)
        I_W_Ps.I_w_Ps.Add_Values_WithOut_Keys("PI", PIs)

        I_W_Ps.I_w_Ps.Title = "PI" & I_W_Ps.Title
        Me.AddTo_Item_With_Properties(I_W_Ps.I_w_Ps)
    End Sub

    Private Sub Protein_Parameters_Half_Life_Click(sender As Object, e As EventArgs) Handles Protein_Parameters_Half_Life.Click
        Dim I_W_Ps = Get_Ext_I_w_Ps()

        Dim PS As New Szunyi.Protein.Protein_Stability
        Dim Ps_Human = PS.Get_Half_Lifes(I_W_Ps.Seqs, "Mammalian")
        I_W_Ps.I_w_Ps.Add_Values_WithOut_Keys("Mammalian Half Life", Ps_Human)

        Dim Ps_Yeast = PS.Get_Half_Lifes(I_W_Ps.Seqs, "Yeast")
        I_W_Ps.I_w_Ps.Add_Values_WithOut_Keys("Yeast Half Life", Ps_Yeast)

        Dim Ps_Coli = PS.Get_Half_Lifes(I_W_Ps.Seqs, "E. Coli")
        I_W_Ps.I_w_Ps.Add_Values_WithOut_Keys("E. Coli Half Life", Ps_Coli)

        I_W_Ps.I_w_Ps.Title = "Half Life" & I_W_Ps.Title
        Me.AddTo_Item_With_Properties(I_W_Ps.I_w_Ps)
    End Sub

    Private Sub Protein_Parameters_AA_Composition_Click(sender As Object, e As EventArgs) Handles Protein_Parameters_AA_Composition.Click
        Dim I_W_Ps = Get_Ext_I_w_Ps()
        Dim AAs = Szunyi.Protein.Amino_Acid_Composition_Manipulation.Get_AA_Compositions(I_W_Ps.Seqs)
        For i = 0 To I_W_Ps.Seqs.Count - 1
            Dim Seq = AAs(i)
            If i = 0 Then
                For Each Item In Seq.Counts
                    I_W_Ps.I_w_Ps.ItemHeaders.Add("Nof:" & ChrW(Item.Key))
                Next
                For Each Item In Seq.Counts
                    I_W_Ps.I_w_Ps.ItemHeaders.Add("%:" & ChrW(Item.Key))
                Next
            End If
            For Each Item In Seq.Counts
                I_W_Ps.I_w_Ps.Items(i).Properties.Add(Item.Value)
            Next
            For Each Item In Seq.Percents
                I_W_Ps.I_w_Ps.Items(i).Properties.Add(Item.Value)
            Next
        Next
        I_W_Ps.I_w_Ps.Title = "AA Composition" & I_W_Ps.Title
        Me.AddTo_Item_With_Properties(I_W_Ps.I_w_Ps)
    End Sub

    Private Sub Protein_Parameters_Hidrophobicity_Click(sender As Object, e As EventArgs) Handles Protein_Parameters_Hidrophobicity.Click
        Dim I_W_Ps = Get_Ext_I_w_Ps()
        Dim hi As New Szunyi.Protein.Hidrophobicity
        Dim Hidrophobicity_Values = hi.Get_HidroHidrophobicity_Indexes(I_W_Ps.Seqs)
        I_W_Ps.I_w_Ps.Add_Values_WithOut_Keys("Hidrophaticity Index", Hidrophobicity_Values)

        I_W_Ps.I_w_Ps.Title = "Hidrophaticity" & I_W_Ps.Title
        Me.AddTo_Item_With_Properties(I_W_Ps.I_w_Ps)
    End Sub

#End Region

#End Region

#Region "DNA"


#Region "Primer Design ANd Analysis"
    Private Sub Design_RT_PCR_Primers_Click(sender As Object, e As EventArgs) Handles Design_RT_PCR_Primers.Click
        Dim Seqs = Me.GetSeqsFromSelectedList(True)
        Dim x As New Szunyi.Primers.RTPCR(Seqs)

    End Sub

    Private Sub FromVariantTableToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles FromVariantTableToolStripMenuItem.Click
        Dim Seqs = Me.GetSeqsFromSelectedList(True)
        If IsNothing(Seqs) = True Then Exit Sub
        Dim Files = Szunyi.IO.Files.Filter.SelectFiles("Select Files With Variants")
        If IsNothing(Files) = True Then Exit Sub
        Dim x As New Szunyi.Primers.From_Variant_Table(Seqs, Files)


    End Sub
    Private Sub Primer3VariantTAbleToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles Primer3VariantTAbleToolStripMenuItem.Click
        Dim FIles = Szunyi.IO.Files.Filter.SelectFiles
        For Each File In FIles
            Dim Res = Szunyi.Primers.Primer3Results.GetResult(File)
            Dim I_w_Ps = Szunyi.Primers.Primer3Results.Convert_to_I_w_Ps(Res)

            Dim IDs = I_w_Ps.Get_Keys()
            Dim starts As New List(Of Long)
            Dim Ends As New List(Of Long)
            Dim Distances As New List(Of Long)
            For Each Id In IDs
                Dim s() = Split(Id, " ")
                Dim s1 = Split(s.Last, "-")
                starts.Add(s1.First)
                Ends.Add(s1.Last)
            Next
            For i1 = 0 To starts.Count - 2
                Distances.Add(starts(i1 + 1) - starts(i1))
            Next
            I_w_Ps.Add_Values_WithOut_Keys("Start", starts)
            I_w_Ps.Add_Values_WithOut_Keys("Ends", Ends)
            Distances.Add(0)
            I_w_Ps.Add_Values_WithOut_Keys("Distance", Distances)
            Dim NewFile = Szunyi.IO.Files.Get_New_FileName.GetNewFile(File, " Result.tdt")
            I_w_Ps.Save_With_ID(NewFile)
        Next

    End Sub

    Private Sub Primer3RTToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles Primer3RTToolStripMenuItem.Click

    End Sub
#End Region

#Region "Variants"
    Private Sub Variant_Filter_By_Frequency_Click(sender As Object, e As EventArgs) Handles Variant_Filter_By_Frquency.Click
        Dim d = Szunyi.MyInputBox.GetDouble("Enter the minimal frequency")
        Dim Files = Szunyi.IO.Files.Filter.SelectFiles
        If IsNothing(Files) = True Then Exit Sub
        For Each File In Files
            Dim x = Szunyi.Sequences.Variants.Variant_Manipulation.Import(File)
            Dim All = Szunyi.Sequences.Variants.Variant_Manipulation.Get_All_Variants(x)
            Dim Filtered = From h In All Where h.Frequency >= d


            If Filtered.Count > 0 Then
                Dim NewFile = Szunyi.IO.Files.Get_New_FileName.GetNewFile(File, " Freq more than " & d & File.Extension)
                Szunyi.Sequences.Variants.Variant_Manipulation.Save(Filtered.ToList, NewFile)

            End If

        Next
    End Sub

    Private Sub Maintain_Common_Variants_Click(sender As Object, e As EventArgs) Handles Maintain_Common_Variants.Click
        Dim Files = Szunyi.IO.Files.Filter.SelectFiles
        If IsNothing(Files) = True Then Exit Sub
        Dim Var_list As New List(Of Szunyi.Sequences.Variants.Variants_In_Genome)
        For Each File In Files
            Dim x = Szunyi.Sequences.Variants.Variant_Manipulation.Import(File)
            Var_list.Add(x)
        Next
        Dim All = Szunyi.Sequences.Variants.Variant_Manipulation.Get_All_Variants(Var_list)
        Dim Common_Unique As New List(Of Szunyi.Sequences.Variants.Single_Variant)

        For Each Item In Szunyi.Sequences.Variants.Variant_Manipulation.Get_Same_Variants(All, Files.Count)
            Common_Unique.Add(Szunyi.Sequences.Variants.Variant_Manipulation.Merge(Item))

        Next
        Szunyi.Sequences.Variants.Variant_Manipulation.Save(Common_Unique)


    End Sub
#End Region

#End Region
#Region "Feature Manipulation"
    Private Sub DeleteFeaturesToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles DeleteFeaturesToolStripMenuItem.Click
        Dim Seqs = Me.GetSeqsFromSelectedList(True)
        If IsNothing(Seqs) = True Then Exit Sub
        Dim cSeqs = Szunyi.Sequences.SequenceManipulation.Common.CloneSeqs(Seqs)
        Dim Feats = Me.GetSelectedFeatureList(True)
        If IsNothing(Feats) = True Then Exit Sub
        For Each Feat In Feats
            For Each extFeat In Feat.Features
                For Each Seq In cSeqs
                    If Seq.Metadata.ContainsKey(Bio.Util.Helper.GenBankMetadataKey) Then
                        Dim md As Bio.IO.GenBank.GenBankMetadata = Seq.Metadata(Bio.Util.Helper.GenBankMetadataKey)
                        Dim tmpFeat = From x In md.Features.All Where x.Key = extFeat.Feature.Key And x.Label = extFeat.Feature.Label

                        Szunyi.Features.FeatureManipulation.GenBankMetaDataManipulation.Remove_Feature(Seq, tmpFeat.First)
                        '    md.Features.All.Remove(extFeat.Feature)
                        Dim kj As Int16 = 54
                    End If
                Next
            Next
        Next
        Szunyi.IO.Export.SaveSequencesToSingleGenBank(cSeqs)
    End Sub
#End Region

#Region "GetFeatures"
#Region "By Annotation"
    Private Sub GetFeatures(bgWork_Title As String, Types As List(Of String))
        Dim SelectedSequences = From x In SelectedItems Where x.Type = Szunyi.Constants.BackGroundWork.Sequences Select x.ID

        If SelectedSequences.Count = 0 Then
            MsgBox("There Is no any Sequences In the list")
            Exit Sub
        End If
        Dim Seqs = From x In ListOfs.ListOfSequences Where SelectedSequences.Contains(x.UniqueID)
        Dim cSearchSetting As SettingForSearchInFeaturesAndQulifiers

        cSearchSetting = New SettingForSearchInFeaturesAndQulifiers(Types)

        Dim t As New Szunyi.ListOf.ExtFeatureList(cSearchSetting, Seqs.ToList, bgWork_Title)
        CreateBgWork(Szunyi.Constants.BackGroundWork.Features, t)


    End Sub

    Private Sub GetFeatures(bgWork_Title As String, Type As String)
        Dim SelectedSequences = From x In SelectedItems Where x.Type = Szunyi.Constants.BackGroundWork.Sequences Select x.ID

        If SelectedSequences.Count = 0 Then
            MsgBox("There Is no any Sequences In the list")
            Exit Sub
        End If
        Dim Seqs = From x In ListOfs.ListOfSequences Where SelectedSequences.Contains(x.UniqueID)

        Dim AllSeqs = Me.GetSeqsFromSelectedList(False)
        Dim DiffFeatures = Szunyi.Features.FeatureManipulation.Key.Get_All_Different_Keys(AllSeqs)

        Dim cSearchSetting As SettingForSearchInFeaturesAndQulifiers
        If Type = "" Then ' It Means Custom
            cSearchSetting = GetCustomFeaturesAndQulifiersSetting(DiffFeatures)
            If IsNothing(cSearchSetting) = True Then Exit Sub
        ElseIf Type = "All" Then
            cSearchSetting = New SettingForSearchInFeaturesAndQulifiers(DiffFeatures)
        Else

            cSearchSetting = New SettingForSearchInFeaturesAndQulifiers(Type)
        End If
        Dim t As New Szunyi.ListOf.ExtFeatureList(cSearchSetting, Seqs.ToList, bgWork_Title)
        CreateBgWork(Szunyi.Constants.BackGroundWork.Features, t)


    End Sub

    Private Sub Get_Exon_Intron_Promotor_UTR(FeatList As Szunyi.ListOf.ExtFeatureList)
        Dim Feats As New List(Of ExtFeatureList)
        If IsNothing(FeatList) = True Then
            Feats = Me.GetSelectedFeatureList()
            For i1 = Feats.Count - 1 To 0 Step -1
                If Feats(i1).SearchSetting.SelectedFeatures.Contains(StandardFeatureKeys.CodingSequence) = False Then
                    Feats.RemoveAt(i1)
                End If
            Next
        Else
            Feats.Add(FeatList)
        End If

        Dim Length As Integer = 1000
        Dim Exons As New ExtFeatureList
        Dim Promoters As New ExtFeatureList
        Dim UTRs As New ExtFeatureList
        Dim Introns As New ExtFeatureList

        For Each FeatList In Feats
            For Each Feat In FeatList.Features
                Dim ExonLocations = Szunyi.Features.FeatureManipulation.GetLocations.GetCDSExonsLocations(Feat.Feature)
                Dim IntronLocations = Szunyi.Features.FeatureManipulation.GetLocations.GetIntronLocationsFromExonLOcations(ExonLocations)
                Dim PromoterLocation = Szunyi.Features.FeatureManipulation.GetLocations.GetPromoterLocationFromCDS_With_Orintation(Feat, 1000)
                Dim UTRLocation = Szunyi.Features.FeatureManipulation.GetLocations.GetUTRLocationFromCDS_With_Orintation(Feat, 1000)

                Exons.Features.AddRange(ExtFeatureManipulation.CreateExtFeatures(ExonLocations, Feat, StandardFeatureKeys.Exon))
                Introns.Features.AddRange(ExtFeatureManipulation.CreateExtFeatures(IntronLocations, Feat, StandardFeatureKeys.Intron))
                Promoters.Features.Add(ExtFeatureManipulation.CreateExtFeature(PromoterLocation, Feat, StandardFeatureKeys.Promoter))
                UTRs.Features.Add(ExtFeatureManipulation.CreateExtFeature(UTRLocation, Feat, StandardFeatureKeys.ThreePrimeUtr))


            Next
        Next
        Exons.SetIt()
        Introns.SetIt()
        Promoters.SetIt()
        UTRs.SetIt()
        Exons.ShortFileName = "Exons e:" & Exons.Features.Count
        Introns.ShortFileName = "Introns e:" & Introns.Features.Count
        Promoters.ShortFileName = "Promoters e:" & Promoters.Features.Count
        UTRs.ShortFileName = "UTRs e:" & UTRs.Features.Count

        AddToExtFeatureList(Exons)
        AddToExtFeatureList(Introns)
        AddToExtFeatureList(Promoters)
        AddToExtFeatureList(UTRs)
    End Sub
    Private Sub AllToolStripMenuItem5_Click(sender As Object, e As EventArgs) Handles AllToolStripMenuItem5.Click
        GetFeatures(Szunyi.Constants.BackGroundWork.Features, "All")
    End Sub
    Private Sub GeneToolStripMenuItem1_Click(sender As Object, e As EventArgs) Handles GeneToolStripMenuItem1.Click
        GetFeatures(Szunyi.Constants.BackGroundWork.Features, StandardFeatureKeys.Gene)
    End Sub
    Private Sub AllToolStripMenuItem3_Click(sender As Object, e As EventArgs) Handles AllToolStripMenuItem3.Click
        GetFeatures(Szunyi.Constants.BackGroundWork.Features, StandardFeatureKeys.All.ToList)
    End Sub

    Private Sub CDSToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles ToolStripMenuItem8.Click
        GetFeatures(Szunyi.Constants.BackGroundWork.Features, StandardFeatureKeys.CodingSequence)
    End Sub
    Private Sub MRNAToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles MRNAToolStripMenuItem.Click
        GetFeatures(Szunyi.Constants.BackGroundWork.Features, StandardFeatureKeys.MessengerRna)
    End Sub

    Private Sub PromoterToolStripMenuItem2_Click(sender As Object, e As EventArgs) Handles PromoterToolStripMenuItem2.Click
        GetFeatures(Szunyi.Constants.BackGroundWork.Features, StandardFeatureKeys.Promoter)
    End Sub

    Private Sub UTRToolStripMenuItem1_Click(sender As Object, e As EventArgs) Handles UTRToolStripMenuItem1.Click
        GetFeatures(Szunyi.Constants.BackGroundWork.Features, StandardFeatureKeys.ThreePrimeUtr)
    End Sub

    Private Sub NcRNAToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles NcRNAToolStripMenuItem.Click
        GetFeatures(Szunyi.Constants.BackGroundWork.Features, StandardFeatureKeys.NonCodingRna)
    End Sub

    Private Sub RRNAToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles RRNAToolStripMenuItem.Click
        GetFeatures(Szunyi.Constants.BackGroundWork.Features, StandardFeatureKeys.RibosomalRna)
    End Sub

    Private Sub TRNAToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles TRNAToolStripMenuItem.Click
        GetFeatures(Szunyi.Constants.BackGroundWork.Features, StandardFeatureKeys.TransferRna)
    End Sub

    Private Sub CustomToolStripMenuItem3_Click(sender As Object, e As EventArgs) Handles CustomToolStripMenuItem3.Click
        GetFeatures(Szunyi.Constants.BackGroundWork.Features, "")
    End Sub

    Private Sub CommonToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles CommonToolStripMenuItem.Click
        GetFeatures(Szunyi.Constants.BackGroundWork.Features, StandardFeatureKeys.Gene)
        GetFeatures(Szunyi.Constants.BackGroundWork.Features, StandardFeatureKeys.MessengerRna)
        GetFeatures(Szunyi.Constants.BackGroundWork.Features, StandardFeatureKeys.NonCodingRna)
        GetFeatures(Szunyi.Constants.BackGroundWork.Features, StandardFeatureKeys.TransferRna)
        GetFeatures(Szunyi.Constants.BackGroundWork.Features, StandardFeatureKeys.RibosomalRna)
        GetFeatures(Szunyi.Constants.BackGroundWork.CDS_For_Exon_Intron, StandardFeatureKeys.CodingSequence)
    End Sub


#End Region
#Region "FromCDS"
    Private Sub Exon_From_CDS_(sender As Object, e As EventArgs) Handles ExonToolStripMenuItem.Click
        Dim Feature_Lists = Get_Selected_Feature_Lists_By_Type(StandardFeatureKeys.CodingSequence)
        If Feature_Lists.Count = 0 Then Exit Sub
        Exons_From_Feature_Lists(Feature_Lists, StandardFeatureKeys.CodingSequence)
    End Sub
    Private Sub Exons_From_Feature_Lists(Feature_Lists As List(Of ExtFeatureList), Type As String)

        For Each Feature_List In Feature_Lists
            Dim Exons As New ExtFeatureList

            For Each Feat In Feature_List.Features
                Dim ExonLocations = Szunyi.Features.FeatureManipulation.GetLocations.GetCDSExonsLocations(Feat.Feature)

                Exons.Features.AddRange(ExtFeatureManipulation.CreateExtFeatures(ExonLocations, Feat, StandardFeatureKeys.Exon))
            Next
            Exons.SetIt()
            Exons.ShortFileName = "Exons From " & Type & " e:" & Exons.Features.Count & " " & Feature_List.ShortFileName
            AddToExtFeatureList(Exons)
        Next


    End Sub
    Private Sub IntronsToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles IntronsToolStripMenuItem.Click
        Dim feats = Me.GetSelectedFeatureList
        If IsNothing(feats) = True Then Exit Sub
        Introns_From_Feature_Lists(feats, StandardFeatureKeys.CodingSequence)

    End Sub

    Private Sub Intron_From_CDS_(sender As Object, e As EventArgs) Handles IntronToolStripMenuItem.Click
        Dim Feature_Lists = Get_Selected_Feature_Lists_By_Type(StandardFeatureKeys.CodingSequence)
        Dim feats = Me.GetSelectedFeatureList
        If Feature_Lists.Count = 0 Then Exit Sub
        Introns_From_Feature_Lists(Feature_Lists, StandardFeatureKeys.CodingSequence)
    End Sub
    Private Sub FirstIntronToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles FirstIntronToolStripMenuItem.Click
        Dim Feature_Lists = Get_Selected_Feature_Lists_By_Type(StandardFeatureKeys.CodingSequence)
        If Feature_Lists.Count = 0 Then Exit Sub
        Introns_From_Feature_Lists(Feature_Lists, StandardFeatureKeys.CodingSequence, True)
    End Sub
    Private Sub Introns_From_Feature_Lists(Feature_Lists As List(Of ExtFeatureList), Type As String, Optional OnlyFirst As Boolean = False)

        For Each Feature_List In Feature_Lists
            Dim Introns As New ExtFeatureList
            For Each Feat In Feature_List.Features
                Dim ExonLocations = Szunyi.Features.FeatureManipulation.GetLocations.GetCDSExonsLocations(Feat.Feature)
                Dim IntronLocations = Szunyi.Features.FeatureManipulation.GetLocations.GetIntronLocationsFromExonLOcations(ExonLocations)
                Dim Index As Integer = 1
                For Each IntonLoc In IntronLocations
                    IntonLoc.Operator = Feat.Feature.Location.Operator
                Next
                If OnlyFirst = True Then
                    If IntronLocations.Count > 0 Then
                        If IntronLocations.First.Operator = LocationOperator.Complement Then
                            Introns.Features.Add(ExtFeatureManipulation.CreateExtFeature(IntronLocations.Last, Feat, StandardFeatureKeys.Intron))
                        Else
                            Introns.Features.Add(ExtFeatureManipulation.CreateExtFeature(IntronLocations.First, Feat, StandardFeatureKeys.Intron))
                        End If

                    End If

                Else
                    If IntronLocations.Count > 0 Then
                        Introns.Features.AddRange(ExtFeatureManipulation.CreateExtFeatures(IntronLocations, Feat, StandardFeatureKeys.Intron))
                    End If

                End If
            Next

            Introns.SetIt()
            Introns.ShortFileName = "Introns From " & Type & " e:" & Introns.Features.Count & " " & Feature_List.ShortFileName
            AddToExtFeatureList(Introns)
        Next


    End Sub

    Private Sub Genomic_From_CDS_(sender As Object, e As EventArgs) Handles GenomicRegionToolStripMenuItem.Click
        Dim Feature_Lists = Get_Selected_Feature_Lists_By_Type(StandardFeatureKeys.CodingSequence)
        If Feature_Lists.Count = 0 Then Exit Sub
        Genomic_From_Feature_Lists(Feature_Lists)
    End Sub
    Private Sub Promoter_From_CDS_(sender As Object, e As EventArgs) Handles PromoterToolStripMenuItem1.Click
        Dim Feature_Lists = Get_Selected_Feature_Lists_By_Type(StandardFeatureKeys.CodingSequence)
        If Feature_Lists.Count = 0 Then Exit Sub
        Promoters_From_Feature_Lists(Feature_Lists)
    End Sub

    Private Sub Up_AndDown_From_TSS_Feature_Lists(Feature_Lists As List(Of ExtFeatureList))
        Dim TSS_5primes = Szunyi.MyInputBox.GetIntegers("Enter The TSS 5' Lengths Sepatated By Space")
        Dim TSS_3Primes = Szunyi.MyInputBox.GetIntegers("Enter The TSS 3' Lengths Sepatated By Space")
        For Each Feature_List In Feature_Lists
            Dim Type = Szunyi.Text.General.GetText(Feature_List.SearchSetting.SelectedFeatures, " ")

            Up_AndDown_From_TSS(Feature_List, Type, TSS_5primes, TSS_3Primes)

        Next

    End Sub
    Private Sub Up_AndDown_From_TSS(Feature_List As ExtFeatureList, Type As String, TSS_5primes As List(Of Integer), TSS_3Primes As List(Of Integer))
        For Each TSS_5prime In TSS_5primes
            For Each TSS_3prime In TSS_3Primes
                Dim Promoters As New ExtFeatureList
                For Each Feat In Feature_List.Features
                    Dim PromoterLocation = GetLocations.Get_Up_Down_With_Orintation(Feat, TSS_5prime, TSS_3prime)

                    Promoters.Features.Add(ExtFeatureManipulation.CreateExtFeature(PromoterLocation, Feat, StandardFeatureKeys.Promoter))
                Next
                Promoters.SetIt()
                Promoters.ShortFileName = "From TSS Up " & Type & " " & TSS_5prime & " Down " & TSS_3prime & " bp e:" & Promoters.Features.Count & Feature_List.ShortFileName
                AddToExtFeatureList(Promoters)
            Next

        Next


    End Sub
    Private Sub Promoters_From_Feature_Lists(Feature_Lists As List(Of ExtFeatureList))
        Dim Promoter_Lengths = Szunyi.MyInputBox.GetIntegers("Enter The Promoter Lengths Sepatated By Space")

        For Each Feature_List In Feature_Lists
            Dim Type = Szunyi.Text.General.GetText(Feature_List.SearchSetting.SelectedFeatures, " ")
            Promoters_From_Feature_List(Feature_List, Type, Promoter_Lengths)
        Next
    End Sub
    Private Sub Genomic_From_Feature_Lists(Feature_Lists As List(Of ExtFeatureList))
        Dim Lengths = Szunyi.MyInputBox.GetIntegers("Enter The Genomic Lengths Sepatated By Space")



        For Each Feature_List In Feature_Lists
            Dim Type = Szunyi.Text.General.GetText(Feature_List.SearchSetting.SelectedFeatures, " ")
            Genomic_From_Feature_List(Feature_List, Type, Lengths)
        Next

    End Sub
    Private Sub Genomic_From_Feature_List(Feature_List As ExtFeatureList, Type As String, Lengths As List(Of Integer))


        For Each Length In Lengths
            Dim Promoters As New ExtFeatureList
            For Each Feat In Feature_List.Features
                Dim GenomicLocation = Szunyi.Features.FeatureManipulation.GetLocations.GetGenomicLocationFromCDS_With_Orintation(Feat, Length)


                Promoters.Features.Add(ExtFeatureManipulation.CreateExtFeature(GenomicLocation, Feat, StandardFeatureKeys.MiscFeature & Length))
            Next
            Promoters.SetIt()
            Promoters.ShortFileName = "Genomic Region From " & Type & " " & Length & "bp e:" & Promoters.Features.Count & Feature_List.ShortFileName
            AddToExtFeatureList(Promoters)
        Next


    End Sub
    Private Sub Promoters_From_Feature_List(Feature_List As ExtFeatureList, Type As String, Promoter_Lengths As List(Of Integer))
        For Each Promoter_Length In Promoter_Lengths
            Dim Promoters As New ExtFeatureList
            For Each Feat In Feature_List.Features
                Dim PromoterLocation = Szunyi.Features.FeatureManipulation.GetLocations.GetPromoterLocationFromCDS_With_Orintation(Feat, Promoter_Length)

                Promoters.Features.Add(ExtFeatureManipulation.CreateExtFeature(PromoterLocation, Feat, StandardFeatureKeys.Promoter & Promoter_Length))
            Next
            Promoters.SetIt()
            Promoters.ShortFileName = "Promoters From " & Type & " " & Promoter_Length & "bp e:" & Promoters.Features.Count & Feature_List.ShortFileName
            AddToExtFeatureList(Promoters)
        Next


    End Sub
    Private Sub UTR_From_CDS_(sender As Object, e As EventArgs) Handles UTRToolStripMenuItem.Click
        Dim Feature_Lists = Get_Selected_Feature_Lists_By_Type(StandardFeatureKeys.CodingSequence)
        If Feature_Lists.Count = 0 Then Exit Sub
        UTRs_From_Feature_Lists(Feature_Lists)
    End Sub
    Private Sub UTRs_From_Feature_Lists(Feature_Lists As List(Of ExtFeatureList))
        Dim UTR_Lengths = Szunyi.MyInputBox.GetIntegers("Enter The Promoter Lengths Sepatated By Space")

        For Each Feature_List In Feature_Lists
            Dim Type = Szunyi.Text.General.GetText(Feature_List.SearchSetting.SelectedFeatures, " ")
            UTRs_From_Feature_List(Feature_List, Type, UTR_Lengths)

        Next
    End Sub
    Private Sub UTRs_From_Feature_List(Feature_List As ExtFeatureList, Type As String, UTR_Lengths As List(Of Integer))

        For Each UTR_Length In UTR_Lengths
            Dim UTRs As New ExtFeatureList
            For Each Feat In Feature_List.Features
                Dim PromoterLocation = Szunyi.Features.FeatureManipulation.GetLocations.GetUTRLocationFromCDS_With_Orintation(Feat, UTR_Length)

                UTRs.Features.Add(ExtFeatureManipulation.CreateExtFeature(PromoterLocation, Feat, StandardFeatureKeys.ThreePrimeUtr & UTR_Length))
            Next
            UTRs.SetIt()
            UTRs.ShortFileName = "UTRs From " & Type & " " & UTR_Length & "bp e:" & UTRs.Features.Count & " " & Feature_List.ShortFileName
            AddToExtFeatureList(UTRs)
        Next


    End Sub

#End Region
#Region "From mRNA"
    Private Sub ExonToolStripMenuItem1_Click(sender As Object, e As EventArgs) Handles ExonToolStripMenuItem1.Click
        Dim Feature_Lists = Get_Selected_Feature_Lists_By_Type(StandardFeatureKeys.MessengerRna)
        If Feature_Lists.Count = 0 Then Exit Sub
        Exons_From_Feature_Lists(Feature_Lists, StandardFeatureKeys.MessengerRna)
    End Sub

    Private Sub IntronToolStripMenuItem1_Click(sender As Object, e As EventArgs) Handles IntronToolStripMenuItem1.Click
        Dim Feature_Lists = Get_Selected_Feature_Lists_By_Type(StandardFeatureKeys.MessengerRna)
        If Feature_Lists.Count = 0 Then Exit Sub
        Introns_From_Feature_Lists(Feature_Lists, StandardFeatureKeys.MessengerRna)
    End Sub

    Private Sub PromoterToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles PromoterToolStripMenuItem.Click
        Dim Feature_Lists = Get_Selected_Feature_Lists_By_Type(StandardFeatureKeys.MessengerRna)
        If Feature_Lists.Count = 0 Then Exit Sub
        Promoters_From_Feature_Lists(Feature_Lists)

    End Sub
    Private Sub UpAndDownFromTSSToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles UpAndDownFromTSSToolStripMenuItem.Click
        Dim Feature_Lists = Get_Selected_Feature_Lists_By_Type(StandardFeatureKeys.MessengerRna)
        If Feature_Lists.Count = 0 Then Exit Sub
        Up_AndDown_From_TSS_Feature_Lists(Feature_Lists)

    End Sub
    Private Sub UTRToolStripMenuItem2_Click(sender As Object, e As EventArgs) Handles UTRToolStripMenuItem2.Click
        Dim Feature_Lists = Get_Selected_Feature_Lists_By_Type(StandardFeatureKeys.MessengerRna)
        If Feature_Lists.Count = 0 Then Exit Sub
        UTRs_From_Feature_Lists(Feature_Lists)
    End Sub

    Private Sub PromoterFromAllToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles FromAllToolStripMenuItem.Click
        Dim Feature_Lists = GetSelectedFeatureList()
        If IsNothing(Feature_Lists) = True Then Exit Sub
        Promoters_From_Feature_Lists(Feature_Lists)
    End Sub

    Private Sub FromAllToolStripMenuItem1_Click(sender As Object, e As EventArgs) Handles FromAllToolStripMenuItem1.Click
        Dim Feature_Lists = GetSelectedFeatureList()
        If IsNothing(Feature_Lists) = True Then Exit Sub
        UTRs_From_Feature_Lists(Feature_Lists)
    End Sub



    Private Sub ItemsWithPropertiesToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles ItemsWithPropertiesToolStripMenuItem.Click
        Dim BasicFiles = Szunyi.IO.Files.Filter.SelectFiles("Select Basic Files")
        If IsNothing(BasicFiles) Then Exit Sub

        Dim Header_Of_Basic_Files = ClassLibrary1.Szunyi.IO.Import.Text.GetHeader(BasicFiles.First, 1, Nothing, Nothing)

        Dim ID_For_Basic_Files As New CheckBoxForStringsFull(Header_Of_Basic_Files, 1, "Select ID column")
        If ID_For_Basic_Files.ShowDialog <> DialogResult.OK Then Exit Sub

        Dim Items_For_Basic_Files As New CheckBoxForStringsFull(Header_Of_Basic_Files, -1, "Select Columns to include")
        If Items_For_Basic_Files.ShowDialog <> DialogResult.OK Then Exit Sub

        Dim Log As New List(Of String)
        '    Items_For_Basic_Files.SelectedIndexes = Items_For_Basic_Files.SelectedIndexes.Except(ID_For_Basic_Files.SelectedIndexes).ToList
        For Each File In BasicFiles
            Dim res As New Szunyi.Text.TableManipulation.Items_With_Properties(File,
                                                                         ID_For_Basic_Files.SelectedIndexes.First,
                                                                         Items_For_Basic_Files.SelectedIndexes, Log, Nothing, False)
            CreateBgWork(Szunyi.Constants.BackGroundWork.Items_With_Properties, res)
        Next
    End Sub


    Private Sub FromItemWithPropertiesToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles FromItemWithPropertiesToolStripMenuItem.Click
        Dim Items_With_Props = Me.GetSelected_Items_With_Propeties(True)
        If IsNothing(Items_With_Props) = True Then Exit Sub
        Dim Header_Of_Basic_Files = Items_With_Props.First.ItemHeaders

        Dim ID_For_SeqID As New CheckBoxForStringsFull(Header_Of_Basic_Files, 1, "Select Seq ID column")
        If ID_For_SeqID.ShowDialog <> DialogResult.OK Then Exit Sub

        Dim ID_For_Seq As New CheckBoxForStringsFull(Header_Of_Basic_Files, 1, "Select Seqence column")
        If ID_For_Seq.ShowDialog <> DialogResult.OK Then Exit Sub

        Dim Ids = Items_With_Props.First.Get_Values(ID_For_SeqID.SelectedIndexes.First)
        Dim Seqs = Items_With_Props.First.Get_Values(ID_For_Seq.SelectedIndexes.First)

        Dim str As New System.Text.StringBuilder
        For i1 = 0 To Ids.Count - 1
            str.Append(">").Append(Ids(i1)).AppendLine.Append(Seqs(i1)).AppendLine()
        Next
        Szunyi.IO.Export.SaveText(str.ToString)
    End Sub


    Private Sub CsvLineWithToTdtToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles CsvLineWithToTdtToolStripMenuItem.Click
        Dim BasicFiles = Szunyi.IO.Files.Filter.SelectFiles()
        If IsNothing(BasicFiles) = True Then Exit Sub
        Dim Header_Of_Basic_Files = ClassLibrary1.Szunyi.IO.Import.Text.GetHeader(BasicFiles.First, 1, Nothing, Nothing)

        Dim ID_For_Basic_Files As New CheckBoxForStringsFull(Header_Of_Basic_Files, 1, "Select ID column")
        If ID_For_Basic_Files.ShowDialog <> DialogResult.OK Then Exit Sub

        Dim Items_For_Basic_Files As New CheckBoxForStringsFull(Header_Of_Basic_Files, -1, "Select Columns to Include")
        If Items_For_Basic_Files.ShowDialog <> DialogResult.OK Then Exit Sub

        Dim Items_For_Splitting_Files As New CheckBoxForStringsFull(Header_Of_Basic_Files, 1, "Select Columns to Split")
        If Items_For_Splitting_Files.ShowDialog <> DialogResult.OK Then Exit Sub

        Dim MainSeparator = InputBox("Main Separator")
        Dim Addtional_Separator = InputBox("Additional Separator")

        Dim Log As New List(Of String)
        For Each File In BasicFiles

            Dim res As New Szunyi.Text.TableManipulation.Items_With_Properties(File,
                                                                                    ID_For_Basic_Files.SelectedIndexes.First,
                                                                                    Items_For_Basic_Files.SelectedIndexes, Log, Nothing, False)
            res.DoIt(True)
            res.Add(File, ID_For_Basic_Files.SelectedIndexes.First, Items_For_Splitting_Files.SelectedIndexes, Log)
            res.Split_Columns_By_2Separator(res.ItemHeaders.Count - 1, MainSeparator, Addtional_Separator)
            Dim NewFIle = Szunyi.IO.Files.Get_New_FileName.GetNewFile(File, "Md.tdt")
            res.Save_With_ID(NewFIle)
        Next
    End Sub

    Private Sub DistinctToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles DistinctToolStripMenuItem.Click
        Dim Seq_Lists = Me.GetSelectedSequenceList
        If IsNothing(Seq_Lists) = True Then Exit Sub
        Dim Common = Szunyi.Sequences.SequenceManipulation.UniqueDistinct.Get_Common_Seqs_ByIDs(Seq_Lists.First.Sequences, Seq_Lists.Last.Sequences)
        Dim x As New Szunyi.ListOf.SequenceList(Common,
                                                   Seq_Lists.First.ShortFileName & " And " & Seq_Lists.Last.ShortFileName,
                                                   Seq_Lists.First.ShortFileName & " And " & Seq_Lists.Last.ShortFileName)
        Me.AddToSequenceList(x)

        Dim A = Szunyi.Sequences.SequenceManipulation.UniqueDistinct.GetDistinctSeqsByIDs(Seq_Lists.First.Sequences, Seq_Lists.Last.Sequences)
        Dim x1 As New Szunyi.ListOf.SequenceList(A,
                                                   Seq_Lists.First.ShortFileName & " But Not in " & Seq_Lists.Last.ShortFileName,
                                                   Seq_Lists.First.ShortFileName & " But Not in " & Seq_Lists.Last.ShortFileName)
        Me.AddToSequenceList(x1)

        Dim B = Szunyi.Sequences.SequenceManipulation.UniqueDistinct.GetDistinctSeqsByIDs(Seq_Lists.Last.Sequences, Seq_Lists.First.Sequences)
        Dim x2 As New Szunyi.ListOf.SequenceList(B,
                                                   Seq_Lists.Last.ShortFileName & " But Not in " & Seq_Lists.First.ShortFileName,
                                                   Seq_Lists.Last.ShortFileName & " But Not in " & Seq_Lists.First.ShortFileName)
        Me.AddToSequenceList(x2)
    End Sub

    Private Sub ReNameFeatureKeyToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles ReNameFeatureKeyToolStripMenuItem.Click
        Dim Feature_Lists = Me.GetSelectedFeatureList()
        If IsNothing(Feature_Lists) Then Exit Sub
        Dim f1 As New CheckBoxForStringsFull(Bio.IO.GenBank.StandardFeatureKeys.All.ToList, 1, "Select New Feature Key")
        If f1.ShowDialog = DialogResult.OK Then

            For Each Feature_List In Feature_Lists
                For Each Feat In Feature_List.Features
                    Dim x As New FeatureItem(f1.SelectedStrings.First, Feat.Feature.Location)
                    Szunyi.Features.FeatureManipulation.MergeFeatures.Merge2Features(Feat.Feature, x, True)
                    Szunyi.Features.FeatureManipulation.GenBankMetaDataManipulation.AddFeature(Feat.Seq, x)
                    Szunyi.Features.FeatureManipulation.GenBankMetaDataManipulation.Remove_Feature(Feat.Seq, Feat.Feature)
                Next
            Next
        End If
        Dim Seqs = Szunyi.Features.ExtFeatureManipulation.Get_Original_Sequences(Feature_Lists)
        Szunyi.IO.Export.SaveSequencesToSingleGenBank(Seqs)
    End Sub

#End Region

    Private Function Get_Selected_Feature_Lists_By_Type(Type As String) As List(Of ExtFeatureList)
        Dim Feature_Lists = Me.GetSelectedFeatureList
        If IsNothing(Feature_Lists) = True Then Return New List(Of ExtFeatureList)
        Dim tmp = From x In Feature_Lists Where x.SearchSetting.SelectedFeatures.Contains(Type)

        If tmp.Count = 0 Then
            MsgBox("No " & Type & " In The Feature Lists")
            Return New List(Of ExtFeatureList)
        End If
        Return tmp.ToList
    End Function


#End Region



    Private Sub ContainsToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles ContainsToolStripMenuItem.Click
        Dim FIles = Szunyi.IO.Files.Filter.SelectFiles
        If IsNothing(FIles) = True Then Exit Sub
        Dim x As New Get_List_of_String("Add Interestring string!")

        Dim InterestingStrings As New List(Of String)
        Dim ForbiddenStrings As New List(Of String)
        If x.ShowDialog = DialogResult.OK Then
            InterestingStrings = x.Strings
        End If
        For Each File In FIles
            For Each s In InterestingStrings
                Dim s1() = Split(s, vbTab)

                If File.Name.Contains(s1.First) = True Then
                    Dim NewFIleNAme = File.FullName.Replace(s1.First, s1.Last)
                    File.MoveTo(NewFIleNAme)
                End If


            Next
        Next
    End Sub

    Private Sub ItemWIthPropertiesToolStripMenuItem2_Click(sender As Object, e As EventArgs) Handles ItemWIthPropertiesToolStripMenuItem2.Click
        Dim IWPS = Me.GetSelected_Items_With_Propeties(True)
        If IsNothing(IWPS) = True Then Exit Sub
        For Each IWP In IWPS
            Dim Header = IWP.ItemHeaders
            Dim Sg = Szunyi.Text.General.GetFirstParts(Header, "_")
            Dim SG2 As New List(Of Integer)
            For Each i In Sg
                Try
                    Dim i1 As Integer = i
                    SG2.Add(i1)
                Catch ex As Exception

                End Try
            Next
            SG2.Sort()
            Dim Out As New Dictionary(Of String, List(Of String))
            For Each Item In IWP.Items
                Out.Add(Item.ID, New List(Of String))
                Dim Index = IWP.Get_Index(Item.ID, Szunyi.Constants.TextMatch.Exact)
                For Each I In SG2
                    Try
                        Dim i1 As Integer = I
                        Dim COlName = From x In Header Where x.StartsWith(I & "_") = True
                        Dim COliD = Szunyi.Text.Lists.Get_Index(Header, COlName.First)
                        Out(Item.ID).Add(IWP.Items(Index).Properties(COliD))

                    Catch ex As Exception

                    End Try
                Next
            Next
            Dim str As New System.Text.StringBuilder
            For Each Item In Out
                str.Append(Item.Key).Append(vbTab)
                str.Append(Szunyi.Text.General.GetText(Item.Value, vbTab))
                str.AppendLine()
            Next
            Dim Alf As Int16 = 65
        Next
    End Sub

    Private Sub SelectDuplicatedReadToolStripMenuItem_Click(sender As Object, e As EventArgs)
        Dim Files = Szunyi.IO.Files.Filter.SelectFiles("Select Files", Szunyi.Constants.Files.Other.BAM)
        If IsNothing(Files) = True Then Exit Sub
        Dim Nof_Both As Integer = 0
        Dim str As New System.Text.StringBuilder
        For Each File In Files

            str.Append(File.Name).AppendLine()
            Using s As New FileStream(File.FullName, FileMode.Open)

                Dim bamReader As New Bio.IO.BAM.BAMParser()

                Dim Reads As New List(Of Bio.IO.SAM.SAMAlignedSequence)
                For Each SA As Bio.IO.SAM.SAMAlignedSequence In bamReader.Parse(s)
                    Reads.Add(SA)
                    SA.ToString()
                Next
                Dim Duplicated = From x In Reads Group By x.QName Into Group

                For Each Dupl In Duplicated
                    If Dupl.Group.Count > 1 Then
                        For i1 = 0 To Dupl.Group.Count - 1
                            Dim Sa = Dupl.Group(i1)

                            str.Append(Sa.QName).Append(vbTab)
                            str.Append(Sa.RName).Append(vbTab)
                            str.Append(Sa.Pos).Append(vbTab)
                            str.Append(Sa.RefEndPos).Append(vbTab)
                            '      str.Append(Sa.MPos).Append(vbTab)
                            str.Append(Sa.CIGAR).Append(vbTab)

                            str.Append(Sa.CIGAR).Append(vbTab)
                            str.AppendLine()
                        Next
                    End If
                Next
            End Using

        Next

    End Sub




    Private Sub LongTranscriptsToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles LongTranscriptsToolStripMenuItem.Click
        Dim f As New LongTranscripts
        f.Show()
    End Sub

    Private Sub ByReferenceSequencesToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles ByReferenceSequencesToolStripMenuItem.Click
        Dim Seqs = Me.GetSeqsFromSelectedList(True)
        If IsNothing(Seqs) = True Then Exit Sub
        Dim MatrixIDs = Get_MatrixS()
        If IsNothing(MatrixIDs) = True Then Exit Sub
        Dim SeqIDs = Szunyi.Sequences.SequenceManipulation.Common.GetSeqIDs(Seqs)

        Dim f1 As New CheckBoxForStringsFull(SeqIDs, -1)
        If f1.ShowDialog = DialogResult.OK Then
            Dim RefSeqs = Szunyi.Sequences.SequenceManipulation.GetSequences.ByIDs(Seqs, f1.SelectedStrings)
            ' Dim x As New Szunyi.DNA.Mutation.Count_Silent_Conservative_Semi_Radical(Seqs, RefSeqs, MatrixIDs)
        End If



    End Sub
    Private Function Get_MatrixS() As List(Of SimilarityMatrices.SimilarityMatrix)
        Dim Matrix As New Bio.SimilarityMatrices.SimilarityMatrix.StandardSimilarityMatrix
        Dim values() As Integer = CType([Enum].GetValues(GetType(Bio.SimilarityMatrices.SimilarityMatrix.StandardSimilarityMatrix)), Integer())
        Dim Names As New List(Of String)
        Dim EnumIDs As New List(Of Integer)
        For Each value In values
            Names.Add(CType(value, Bio.SimilarityMatrices.SimilarityMatrix.StandardSimilarityMatrix).ToString())
            EnumIDs.Add(value)
        Next

        Dim f1 As New CheckBoxForStringsFull(Names, -1)
        Dim MatrixIDs As New List(Of Integer)
        If f1.ShowDialog = DialogResult.OK Then

            For i1 = 0 To Names.Count - 1
                If f1.SelectedStrings.Contains(Names(i1)) Then
                    MatrixIDs.Add(EnumIDs(i1))
                End If
            Next
            ' Return MatrixIDs
        End If
        Dim Matrixes As New List(Of SimilarityMatrices.SimilarityMatrix)
        For Each MatrixID In MatrixIDs
            Dim M As New Bio.SimilarityMatrices.SimilarityMatrix(MatrixID)
            Matrixes.Add(M)
        Next
        Return Matrixes
    End Function
    Private Sub BetweenGropsToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles BetweenGropsToolStripMenuItem.Click
        Dim SeqLists = Me.GetSelectedSequenceList
        If IsNothing(SeqLists) = True Then Exit Sub
        Dim AllSeqs = Me.GetSeqsFromSelectedList(True)
        Dim dAllSeqs = Szunyi.Sequences.SequenceManipulation.UniqueDistinct.Get1CopyByID(AllSeqs)
        SeqLists.Add(New SequenceList(dAllSeqs, "AllSeqs", "AllSeqs"))
        Dim MatrixIDs = Get_MatrixS()
        If IsNothing(MatrixIDs) = True Then Exit Sub
        '    Dim x As New Szunyi.DNA.Mutation.Count_Silent_Conservative_Semi_Radical(SeqLists, MatrixIDs)




    End Sub

    Private Sub AAToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles AAToolStripMenuItem.Click
        Dim SeqLists = Me.GetSelectedSequenceList
        If IsNothing(SeqLists) = True Then Exit Sub
        Dim AllSeqs = Me.GetSeqsFromSelectedList(True)
        Dim dAllSeqs = Szunyi.Sequences.SequenceManipulation.UniqueDistinct.Get1CopyByID(AllSeqs)
        SeqLists.Clear()
        SeqLists.Add(New SequenceList(dAllSeqs, "AllSeqs", "AllSeqs"))
        Dim MatrixIDs = Get_MatrixS()
        If IsNothing(MatrixIDs) = True Then Exit Sub
        '     Dim x As New Szunyi.DNA.Mutation.Count_Silent_Conservative_Semi_Radical(SeqLists, MatrixIDs)
    End Sub

    Private Sub ColorPickerToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles ColorPickerToolStripMenuItem.Click
        Dim File = Szunyi.IO.Files.Filter.SelectFile("Select tab delimited File", Szunyi.Constants.Files.All_TAB_Like)
        If IsNothing(File) = True Then Exit Sub
        Dim Lines = Szunyi.IO.Import.Text.ReadLines(File, 0)
        Dim Vals As New List(Of String())
        For Each line In Lines
            Dim p = Split(line, vbTab)
            Vals.Add(p)
        Next
        Dim PhyloGeneticClorPicker As New PhyloGeneticClorPicker(Vals)

    End Sub

    Private Sub HDF5StartToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles HDF5StartToolStripMenuItem.Click
        Dim File = Szunyi.IO.Files.Filter.SelectFile("Select File", Szunyi.Constants.Files.HDF5)
        If IsNothing(File) = True Then Exit Sub
        ' Dim x As New Szunyi.PacBio.HDF5(File, True)

    End Sub

    Private Sub PulseIndexToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles PulseIndexToolStripMenuItem.Click
        Dim File = Szunyi.IO.Files.Filter.SelectFile("PulseIndex")
    End Sub

    Private Sub MethylationAnalysisToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles MethylationAnalysisToolStripMenuItem.Click
        Dim x As New Szunyi.PacBio.Methylation

    End Sub


    Private Sub FeaturesWithLocationsToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles FeaturesWithLocationsToolStripMenuItem.Click
        Dim Seqs = GetSeqsFromSelectedList(True)
        If IsNothing(Seqs) = True Then Exit Sub
        Dim str As New System.Text.StringBuilder
        str.Append("Feature Name").Append(vbTab)
        str.Append("Feature Key").Append(vbTab)
        str.Append("TSS").Append(vbTab)
        str.Append("PolyA").Append(vbTab)

        str.Append("Strand").Append(vbTab)
        str.Append("Location").Append(vbTab)
        str.Append("RefID").AppendLine()
        For Each Seq In Seqs


            Dim tmp As New List(Of String)
            tmp.AddRange(Szunyi.Constants.Features.CDSs_mRNAs_Genes)
            tmp.Add(StandardFeatureKeys.NonCodingRna)
            Dim Feats = Szunyi.Features.FeatureManipulation.GetFeaturesByType.GetFeturesByTypesFromSeq(Seq, tmp)
            For Each Feat In Feats
                If Feat.Qualifiers.ContainsKey(StandardFeatureKeys.Gene) Then
                    str.Append(Feat.Qualifiers(StandardFeatureKeys.Gene).First)
                Else
                    str.Append(Feat.Qualifiers(StandardQualifierNames.Note).First)
                End If
                str.Append(vbTab)
                str.Append(Feat.Key).Append(vbTab)
                If Feat.Location.Operator = LocationOperator.Complement Then
                    str.Append(Feat.Location.LocationEnd).Append(vbTab)
                    str.Append(Feat.Location.LocationStart).Append(vbTab)
                Else
                    str.Append(Feat.Location.LocationStart).Append(vbTab)
                    str.Append(Feat.Location.LocationEnd).Append(vbTab)
                End If

                If Feat.Location.Operator = LocationOperator.Complement Then
                    str.Append("-").Append(vbTab)
                Else
                    str.Append("+").Append(vbTab)
                End If
                str.Append(Szunyi.Features.FeatureManipulation.GetLocations.LociBuilder.GetLocationString(Feat.Location)).Append(vbTab)
                str.Append(Seq.ID)
                str.AppendLine()
            Next

        Next
        Dim res = str.ToString.Replace(Chr(34), "")
        Clipboard.SetText(res)
    End Sub

    Private Sub Gff3ToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles Gff3ToolStripMenuItem.Click
        Dim files = Szunyi.IO.Files.Filter.SelectFiles(Szunyi.Constants.Files.gff3)
        Dim p As New Bio.IO.Gff.GffParser()
        For Each File In files
            p.Open(File.FullName)
            Dim t = p.Parse
            Dim alf As Int16 = 54
        Next
    End Sub


    Private Sub ByFirstPartBeforeToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles ByFirstPartBeforeToolStripMenuItem.Click
        Dim Seqs = Me.GetSeqsFromSelectedList(True)
        Dim res As New Dictionary(Of String, Integer())
        For Each Seq In Seqs
            Dim ID = Szunyi.Text.General.GetFirstPart(Seq.ID, "/")
            If res.ContainsKey(ID) = False Then
                Dim His(10000) As Integer
                res.Add(ID, His)
            End If
            res(ID)(Seq.Count) += 1
        Next
        Dim out As New List(Of String)
        out.Add("Lengths")
        Dim str As New System.Text.StringBuilder
        For Each Item In res
            Dim total As Long = 0
            Dim count As Integer = 0
            out(0) = out(0) & vbTab & Item.Key
            For i1 = 0 To 10000

                If Item.Value(i1) <> 0 Then
                    count += Item.Value(i1)
                    total += Item.Value(i1) * i1
                End If
                If out.Count < 10002 Then
                    out.Add(i1 & vbTab & Item.Value(i1))
                Else
                    out(i1 + 1) = out(i1 + 1) & vbTab & Item.Value(i1)
                End If
            Next
            Dim avr = total / count
            str.Append(Item.Key).Append(vbTab)
            str.Append(total).Append(vbTab)
            str.Append(count).Append(vbTab)
            str.Append(avr).AppendLine()
        Next
        Clipboard.SetText(Szunyi.Text.General.GetText(out))
    End Sub


    Private Sub SplitBySMRTCellsFromFilesToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles SplitBySMRTCellsFromFilesToolStripMenuItem.Click
        Dim FIles = Szunyi.IO.Files.Filter.SelectFiles(Szunyi.Constants.Files.Fasta_FastQ)
        If IsNothing(FIles) = True Then Exit Sub
        Dim FOlder = FIles.First.Directory
        If IsNothing(FOlder) = True Then Exit Sub
        Dim Seqs As New List(Of Bio.ISequence)
        Dim FastqSeqs As New List(Of Bio.ISequence)
        For Each File In FIles

            Dim sName = File.Name.Replace(File.Extension, "")
            Seqs.AddRange(Szunyi.IO.Import.Sequence.FromFile(File))

        Next
        If IsNothing(Seqs) = False Then
            Dim gr = From x In Seqs Select x Group By x.ID.Split("/").First, x.GetType.Name Into Group
            For Each item In gr

                Dim theSeq = item.Group
                Dim uSeqs = Szunyi.Sequences.SequenceManipulation.UniqueDistinct.Get1CopyByID(theSeq.ToList)

                If Szunyi.Sequences.SequenceManipulation.Identify.IsBioSequence(uSeqs.First) = True Then
                    Dim nFile As New FileInfo(FOlder.FullName & "\" & item.First & ".1fa")
                    Szunyi.IO.Export.SaveSequencesToSingleFasta(item.Group.ToList, nFile)
                ElseIf Szunyi.Sequences.SequenceManipulation.Identify.IsQualitativeSequence(uSeqs.First) = True Then
                    Dim nFile As New FileInfo(FOlder.FullName & "\" & item.First & ".1fastq")
                    Szunyi.IO.Export.SaveSequencesToSingleFastQ(item.Group.ToList, nFile)
                Else
                    Dim alf As Int16 = 54
                End If

            Next

        End If
    End Sub

    Private Sub SPlitBySMRTCellsToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles SPlitBySMRTCellsToolStripMenuItem.Click
        Dim Seqs = Me.GetSeqsFromSelectedList(True)
        If IsNothing(Seqs) = True Then Exit Sub
        Dim gr = From x In Seqs Select x Group By x.ID.Split("/").First Into Group

        Dim FOlder = Szunyi.IO.Directory.Get_Folder
        If IsNothing(FOlder) = True Then Exit Sub

        For Each item In gr
            Dim nFile As New FileInfo(FOlder.FullName & "\" & item.First & ".fastq")
            Szunyi.IO.Export.SaveSequencesToSingleFastQ(item.Group.ToList, nFile)
        Next

    End Sub


    Private Sub ToSamToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles ToSamToolStripMenuItem.Click
        Dim Files = Szunyi.IO.Files.Filter.SelectFiles
        If IsNothing(Files) = True Then Exit Sub

        Dim Seqs = Me.GetSeqsFromSelectedList(True)
        If IsNothing(Seqs) = True Then Exit Sub
        For Each File In Files
            Dim ju = Szunyi.PacBio.GTH.ToSam(File, Seqs)
        Next


    End Sub







    Private Function tmop(rt_mps As List(Of FeatureItem))
        Dim t = From x In rt_mps Select x Group By x.Label.Split(Chr(34)).Count Into Group
        Return t


    End Function


    Private Iterator Function Get_Item_Witsh_Same_LocusTAgs(Feats As List(Of FeatureItem)) As IEnumerable
        Dim j = From x In Feats Select x Group By x.Qualifiers(StandardQualifierNames.LocusTag).First Into Group

        For Each g In j
            Yield g
        Next
    End Function


    Private Sub MotifsFromFimoToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles MotifsFromFimoToolStripMenuItem.Click
        Dim Seqs = Me.GetSeqsFromSelectedList(True)
        If IsNothing(Seqs) = True Then Exit Sub
        Dim file = Szunyi.IO.Files.Filter.SelectFile()
        If IsNothing(file) = True Then Exit Sub
        For Each Line In Szunyi.IO.Import.Text.ParseNotFirst(file, 1)
            Dim s = Split(Line, vbTab)
            Dim Loci As Bio.IO.GenBank.ILocation = Szunyi.Location.Common.Get_Location(s(2), s(3), s(4))
            Dim f As New FeatureItem(s(0), Loci)
            Dim notes As New List(Of String)
            notes.Add("p value=" & s(6))
            f.Qualifiers(StandardQualifierNames.Note) = notes
            For Each seq In Seqs
                Szunyi.Features.FeatureManipulation.GenBankMetaDataManipulation.AddFeature(seq, f)
            Next
        Next
        Szunyi.IO.Export.SaveSequencesToSingleGenBank(Seqs)
    End Sub


    Private Sub DeleteFeatueKeysToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles DeleteFeatueKeysToolStripMenuItem.Click
        Dim seqs = Me.GetSeqsFromSelectedList(True)
        If IsNothing(seqs) = True Then Exit Sub
        Dim cSeqs = Szunyi.Sequences.SequenceManipulation.Common.CloneSeqs(seqs)
        Dim QulaToDelete = Szunyi.Features.FeatureManipulation.Common.Get_Feature_Types(seqs)
        Dim QualNames = Szunyi.Features.FeatureManipulation.Qulifiers.Get_Distinct_Qulifiers(cSeqs)
        Dim f1 As New CheckBoxForStringsFull(QulaToDelete, -1)
        If f1.ShowDialog = DialogResult.OK Then
            Dim toDelete As New List(Of FeatureItem)
            For Each Seq In cSeqs
                For Each Feat In Szunyi.Features.FeatureManipulation.GetFeaturesByType.Get_All_Features(cSeqs)
                    If f1.SelectedStrings.Contains(Feat.Key) = True Then
                        toDelete.Add(Feat)
                    End If
                Next
            Next
            For Each I In toDelete
                Szunyi.Features.FeatureManipulation.GenBankMetaDataManipulation.Remove_Feature(cSeqs.First, I)

            Next
        End If

        Szunyi.IO.Export.SaveSequencesToSingleGenBank(cSeqs)

    End Sub
    Private Sub DeleteQuilifiersToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles DeleteQuilifiersToolStripMenuItem.Click
        Dim seqs = Me.GetSeqsFromSelectedList(True)
        If IsNothing(seqs) = True Then Exit Sub
        Dim cSeqs = Szunyi.Sequences.SequenceManipulation.Common.CloneSeqs(seqs)
        Dim QulaToDelete As New List(Of String)
        Dim QualNames = Szunyi.Features.FeatureManipulation.Qulifiers.Get_Distinct_Qulifiers(cSeqs)
        Dim f1 As New CheckBoxForStringsFull(QualNames, -1)
        If f1.ShowDialog = DialogResult.OK Then
            For Each Seq In cSeqs
                For Each Feat In Szunyi.Features.FeatureManipulation.GetFeaturesByType.GetFeturesByTypesFromSeq(Seq, StandardFeatureKeys.All.ToList)
                    For Each item In f1.SelectedStrings
                        If Feat.Qualifiers.ContainsKey(item) = True Then
                            Feat.Qualifiers.Remove(item)
                        End If
                    Next
                Next
            Next
        End If
        Szunyi.IO.Export.SaveSequencesToSingleGenBank(cSeqs)
    End Sub

    Private Sub PacbioManagerToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles PacbioManagerToolStripMenuItem.Click
        Dim x As New FileManager
        x.Show()

    End Sub

    Private Sub SearhForFilesToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles SearhForFilesToolStripMenuItem.Click
        Dim txt = Clipboard.GetText
        Dim s1() = Split(txt, vbCrLf)
        Dim Folder = Szunyi.IO.Directory.Get_Folder
        If IsNothing(Folder) = True Then Exit Sub
        Dim files = Szunyi.IO.Directory.GetAllFilesFromFolder_Recursive(Folder.FullName)

        Dim rest As New List(Of FileInfo)
        For Each s In s1
            If s.Length <> 0 Then
                Dim res = From x In files Where x.Name.Contains(s)
                rest.AddRange(res)
            End If
        Next
        Dim out = (From c In rest Select c.Name).ToList

        Dim fas = From k In rest Where k.Extension = ".fasta" Or k.Extension = ".fastq"
        Dim txt1 = Szunyi.Text.General.GetText(out)
        For Each file In fas

            file.CopyTo("D:\" & file.Name, True)
        Next
        Dim lk As Int16 = 54
    End Sub

    Private Sub FromMRNAsAndCDSsToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles FromMRNAsAndCDSsToolStripMenuItem.Click
        Dim Seqs = GetSeqsFromSelectedList(True)
        If IsNothing(Seqs) = True Then Exit Sub
        For Each Seq In Seqs
            Dim mRNAs = Szunyi.Features.FeatureManipulation.GetFeaturesByType.GetFeturesByTypeFromSeq(Seq, StandardFeatureKeys.MessengerRna)
            Dim CDSs = Szunyi.Features.FeatureManipulation.GetFeaturesByType.GetFeturesByTypeFromSeq(Seq, StandardFeatureKeys.CodingSequence)

            Dim out As New List(Of FeatureItem)
            For Each theRNA In mRNAs
                Dim selCDSs = From x In CDSs Where x.Location.LocationStart >= theRNA.Location.LocationStart AndAlso x.Location.LocationEnd <= theRNA.Location.LocationEnd AndAlso theRNA.Location.Operator = x.Location.Operator

                If selCDSs.Count > 0 Then
                    Dim min = (From x In selCDSs Select x.Location.LocationStart).Min
                    Dim max = (From x In selCDSs Select x.Location.LocationEnd).Max

                    If theRNA.Location.Operator <> LocationOperator.Complement Then
                        Dim Five_Loci = Szunyi.Location.Common.Get_Location(theRNA.Location.LocationStart & ".." & min)
                        Dim Five As New FeatureItem(StandardFeatureKeys.FivePrimeUtr, Five_Loci)

                        Dim Three_Loci = Szunyi.Location.Common.Get_Location(max & ".." & theRNA.Location.LocationEnd)
                        Dim Three As New FeatureItem(StandardFeatureKeys.ThreePrimeUtr, Three_Loci)

                        Szunyi.Features.FeatureManipulation.MergeFeatures.MergeQualifiers(theRNA, Three)
                        Szunyi.Features.FeatureManipulation.MergeFeatures.MergeQualifiers(theRNA, Five)
                        '  Szunyi.Features.FeatureManipulation.GenBankMetaDataManipulation.AddFeature(Seq, Five)
                        '  Szunyi.Features.FeatureManipulation.GenBankMetaDataManipulation.AddFeature(Seq, Three)
                        out.Add(Five)
                        out.Add(Three)
                    Else
                        Dim Five_Loci = Szunyi.Location.Common.Get_Location("complement(" & max & ".." & theRNA.Location.LocationEnd & ")")
                        Dim Five As New FeatureItem(StandardFeatureKeys.FivePrimeUtr, Five_Loci)

                        Dim Three_Loci = Szunyi.Location.Common.Get_Location("complement(" & theRNA.Location.LocationStart & ".." & min & ")")
                        Dim Three As New FeatureItem(StandardFeatureKeys.ThreePrimeUtr, Three_Loci)

                        Szunyi.Features.FeatureManipulation.MergeFeatures.MergeQualifiers(theRNA, Three)
                        Szunyi.Features.FeatureManipulation.MergeFeatures.MergeQualifiers(theRNA, Five)
                        '  Szunyi.Features.FeatureManipulation.GenBankMetaDataManipulation.AddFeature(Seq, Five)
                        '  Szunyi.Features.FeatureManipulation.GenBankMetaDataManipulation.AddFeature(Seq, Three)
                        out.Add(Five)
                        out.Add(Three)
                    End If
                End If

            Next
            Dim diff = From x In out Group By x.Location.LocationStart, x.Location.LocationEnd, x.Location.Operator Into Group

            For Each gr In diff
                Szunyi.Features.FeatureManipulation.GenBankMetaDataManipulation.AddFeature(Seq, gr.Group.First)
            Next
        Next
    End Sub
    Private Sub GeneiousLocationToolStripMenuItem1_Click(sender As Object, e As EventArgs) Handles GeneiousLocationToolStripMenuItem1.Click
        Dim Seqs = Me.GetSeqsFromSelectedList(True)
        If IsNothing(Seqs) = True Then Exit Sub
        For Each Seq In Seqs
            Dim Feats = Szunyi.Features.FeatureManipulation.GetFeaturesByType.Get_All_Features(Seq)
            For Each Feat In Feats
                Dim Loci = Feat.Location
                If Feat.Location.Operator = LocationOperator.Join AndAlso Feat.Location.SubLocations.Count > 0 AndAlso Feat.Location.SubLocations.First.Operator = LocationOperator.Complement Then
                    Dim kj As Int16 = 54
                    Dim s = Szunyi.Location.Common.GetLocationString(Feat)
                    Dim s1 = s.Replace("complement", "")
                    Dim s2 = s1.Replace("join", "complement(join")
                    Dim s3 = s2.Replace("((", "(")
                    Dim ret = Regex.Split(s3, "[^0-9]")
                    Dim N As New List(Of Integer)
                    For Each item In ret
                        If item <> String.Empty Then
                            N.Add(item)
                        End If
                    Next
                    N.Sort()

                    Dim str As New System.Text.StringBuilder
                    str.Append("complement(join(")
                    For i1 = 0 To N.Count - 1 Step 2
                        str.Append(N(i1)).Append("..").Append(N(i1 + 1)).Append(",")
                        Dim h As Int16 = 54
                    Next
                    str.Length -= 1
                    str.Append("))")
                    Dim loc = Szunyi.Location.Common.Get_Location(str.ToString)
                    Feat.Location = loc
                End If
            Next

        Next
        Szunyi.IO.Export.SaveSequencesToSingleGenBank(Seqs)
    End Sub

    Private Sub AddGeneQualifierToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles AddGeneQualifierToolStripMenuItem.Click
        Dim Seqs = Me.GetSeqsFromSelectedList(True)
        If IsNothing(Seqs) = True Then Exit Sub

    End Sub

    Private Sub TransferRepeatsToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles TransferRepeatsToolStripMenuItem.Click
        Dim Files = Szunyi.IO.Files.Filter.SelectFiles(Szunyi.Constants.Files.Fasta, "Select alignment fileS IN FASTA FORMAT")
        If IsNothing(Files) = True Then Exit Sub


        Dim Seqs = Me.GetSeqsFromSelectedList(True)
        If IsNothing(Seqs) = True Then
            Seqs = Szunyi.IO.Import.Sequence.FromUserSelection
        End If
        Dim RepFile = Szunyi.IO.Files.Filter.SelectFile("", "Select Repeat Files")
        Dim k = New Szunyi.DNA.Repeat.Repeats(RepFile)
        Dim Positions = Szunyi.DNA.Variants.Analysis.GetPos(Seqs.First, Seqs.Last)
        Dim str As New System.Text.StringBuilder
        Dim BaseSeq = (From x In Seqs Where x.ID = k.Repeats.First.SeqID).First
        Dim SecondSeq = (From x In Seqs Where x.ID <> k.Repeats.First.SeqID).First
        Dim out As New List(Of Szunyi.DNA.Repeat.Repeat_Description)
        For Each Rep In k.Repeats
            Dim x = Rep.Clone
            x.SeqID = SecondSeq.ID
            x.Start = Positions(BaseSeq.ID)(Rep.Start)
            x.Endy = Positions(BaseSeq.ID)(Rep.Endy)
            out.Add(x)
        Next
        Dim SaveFile = Szunyi.IO.Files.Save.SelectSaveFile("")
        Dim sw As New StreamWriter(SaveFile.FullName)
        For Each item In out
            sw.Write(item.ToString)
            If item IsNot out.Last Then sw.WriteLine()
        Next
    End Sub
    Private Sub GbkClustalwToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles GbkClustalwToolStripMenuItem.Click
        Dim Files = Szunyi.IO.Files.Filter.SelectFiles(Szunyi.Constants.Files.Fasta, "Select alignment fileS IN FASTA FORMAT")
        If IsNothing(Files) = True Then Exit Sub


        Dim Seqs = Me.GetSeqsFromSelectedList(True)
        If IsNothing(Seqs) = True Then
            Seqs = Szunyi.IO.Import.Sequence.FromUserSelection
        End If

        Dim Matrixes = Get_MatrixS()
        Seqs = (From x In Seqs Order By x.ID).ToList
        Dim RepeatFiles = Szunyi.IO.Files.Filter.SelectFiles("", "Select Repeat Files")

        Dim l As New Szunyi.DNA.Variants.Analysis(Files, Seqs, Matrixes, RepeatFiles)
        Dim alj = l.Get_Result_Merged
        Dim kj = Szunyi.DNA.Variants.SNP_MNP_INDEL_Manipulation.Export(l.Vars.First.Merged_Asc_AllSNPSs, Seqs, Matrixes)
        Dim str As New System.Text.StringBuilder
        For Each Item In Szunyi.DNA.Variants.SNP_MNP_INDEL_Manipulation.Get_SNP_By_CDS_First_Feat(l.Vars.First.Merged_Asc_AllSNPSs)
            Dim SNP_MNP_s = From x In Item Where x.SNP.Type = Szunyi.DNA.Variants.Variation_Type.SNP Or x.SNP.Type = Szunyi.DNA.Variants.Variation_Type.MNP Select x.SNP

            If SNP_MNP_s.Count > 0 Then
                str.Append(Seqs.First.ID).Append(vbTab)
                str.Append(Item.First.Feat1.Qualifiers(StandardQualifierNames.GeneSymbol).First).Append(vbTab)
                For i1 = 0 To Matrixes.Count - 1
                    str.Append(Matrixes(i1).Name.Replace(vbTab, "")).Append(vbTab)
                Next
                str.AppendLine()
                Dim tmpII = Szunyi.DNA.Variants.Analysis.Get_AA_Changes(SNP_MNP_s.ToList)
                str.Append(vbTab).Append(vbTab)
                For i1 = 0 To Matrixes.Count - 1
                    Dim index = i1
                    Dim m1 = (From x In tmpII Select x.MatrixValues(index)).Sum
                    str.Append(m1).Append(vbTab)
                Next
            Else
                str.Append(Seqs.Last.ID).Append(vbTab)
                str.Append(Item.First.Feat2.Qualifiers(StandardQualifierNames.GeneSymbol).First).Append(vbTab)
            End If

            Dim tmp = From x In Item Order By x.SNP.AA_Changes.First.First.CDS_AA_Position Ascending

            Dim ls As New List(Of String)

            For Each T In tmp
                Dim SNPs As New List(Of Szunyi.DNA.Variants.AA_Change)
                Dim AA_Changes As New List(Of Szunyi.DNA.Variants.AA_Change)
                For Each t1 In T.SNP.AA_Changes
                    If IsNothing(T.Feat2) = True OrElse t1.First.AASeq.ConvertToString <> t1.Second.AASeq.ConvertToString Then
                        AA_Changes.Add(t1)
                    End If
                Next
                Dim kj1 = Szunyi.DNA.Variants.SNP_ByCDS.Get_Text(AA_Changes, True, T.Feat2)

                If ls.Contains(kj1) = False Then
                    ls.Add(kj1)
                    str.Append(kj1)

                End If
            Next
            str.Length -= 1
            str.AppendLine()
        Next
        For Each Item In Szunyi.DNA.Variants.SNP_MNP_INDEL_Manipulation.Get_SNP_By_CDS_Second_Feat(l.Vars.First.Merged_Asc_AllSNPSs)
            Dim SNP_MNP_s = From x In Item Where x.SNP.Type = Szunyi.DNA.Variants.Variation_Type.SNP Or x.SNP.Type = Szunyi.DNA.Variants.Variation_Type.MNP Select x.SNP

            If SNP_MNP_s.Count > 0 Then
                str.Append(Seqs.Last.ID).Append(vbTab)
                str.Append(Item.First.Feat2.Qualifiers(StandardQualifierNames.GeneSymbol).First).Append(vbTab)
                For i1 = 0 To Matrixes.Count - 1
                    str.Append(Matrixes(i1).Name.Replace(vbTab, "")).Append(vbTab)
                Next
                str.AppendLine()
                Dim tmpII = Szunyi.DNA.Variants.Analysis.Get_AA_Changes(SNP_MNP_s.ToList)
                str.Append(vbTab).Append(vbTab)
                For i1 = 0 To Matrixes.Count - 1
                    Dim index = i1
                    Dim m1 = (From x In tmpII Select x.MatrixValues(index)).Sum
                    str.Append(m1).Append(vbTab)
                Next
            Else
                str.Append(Seqs.Last.ID).Append(vbTab)
                str.Append(Item.First.Feat2.Qualifiers(StandardQualifierNames.GeneSymbol).First).Append(vbTab)
            End If

            Dim ls As New List(Of String)
            Dim tmp = From x In Item Order By x.SNP.AA_Changes.First.Second.CDS_AA_Position Ascending
            For Each T In tmp
                Dim AA_Changes As New List(Of Szunyi.DNA.Variants.AA_Change)
                For Each t1 In T.SNP.AA_Changes
                    If IsNothing(T.Feat1) = True OrElse t1.First.AASeq.ConvertToString <> t1.Second.AASeq.ConvertToString Then
                        AA_Changes.Add(t1)
                    End If
                Next

                Dim kj1 = Szunyi.DNA.Variants.SNP_ByCDS.Get_Text(AA_Changes, False, T.Feat1)
                If ls.Contains(kj1) = False Then
                    ls.Add(kj1)
                    str.Append(kj1)

                End If
            Next
            str.Length -= 1
            str.AppendLine()
        Next

        Clipboard.SetText(alj & vbCrLf & str.ToString & vbCrLf & kj)



    End Sub

    Private Sub GetTotalLengthsToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles GetTotalLengthsToolStripMenuItem.Click
        Dim Files = Szunyi.IO.Files.Filter.SelectFiles(Constants.Files.Fasta_FastQ)
        If IsNothing(Files) = True Then Exit Sub
        Dim str As New System.Text.StringBuilder
        For Each File In Files
            str.Append(File.Name).Append(vbTab)
            Dim l As Long = 0
            For Each Seq In Szunyi.IO.Import.Sequence.GetOnyByONe(File)
                l += Seq.Count
            Next
            str.Append(l).AppendLine()
        Next
        Clipboard.SetText(str.ToString)
    End Sub

    Private Sub GetLengthsToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles GetLengthsToolStripMenuItem.Click
        Dim Seqs = Me.GetSeqsFromSelectedList(True)
        If IsNothing(Seqs) = True Then Exit Sub
        Dim out As New System.Text.StringBuilder
        For Each Seq In Seqs
            out.Append(Seq.ID).Append(vbTab).Append(Seq.Count).AppendLine()
        Next
        Clipboard.SetText(out.ToString)
    End Sub

    Private Sub AddInnerATGToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles AddInnerATGToolStripMenuItem.Click
        Dim Seqs = Me.GetSeqsFromSelectedList(True)
        If IsNothing(Seqs) = True Then Exit Sub
        For Each Seq In Seqs
            Dim nofiATG As Integer = 0
            Dim CDSs = Szunyi.Features.FeatureManipulation.GetFeaturesByType.GetFeturesByTypeFromSeq(Seq, StandardFeatureKeys.CodingSequence)
            For Each CDS In CDSs
                If CDS.Location.Operator <> LocationOperator.Complement Then
                    For i1 = CDS.Location.LocationStart + 2 To CDS.Location.LocationEnd Step 3 ' Do not Count FIrst ATG
                        If Seq(i1) = 65 AndAlso Seq(i1 + 1) = 84 AndAlso Seq(i1 + 2) = 71 Then
                            Dim f As New FeatureItem("iATG", i1 + 1 & ".." & i1 + 2)
                            Szunyi.Features.FeatureManipulation.MergeFeatures.MergeQualifiers(CDS, f)
                            Szunyi.Features.FeatureManipulation.GenBankMetaDataManipulation.AddFeature(Seq, f)
                            nofiATG += 1
                        End If

                    Next
                Else ' compl
                    For i1 = CDS.Location.LocationEnd - 4 To CDS.Location.LocationStart - 1 Step -3
                        If Seq(i1) = 84 AndAlso Seq(i1 - 1) = 65 AndAlso Seq(i1 - 2) = 67 Then
                            Dim f As New FeatureItem("iATG", "complement(" & i1 - 1 & ".." & i1 + 1 & ")")
                            Szunyi.Features.FeatureManipulation.MergeFeatures.MergeQualifiers(CDS, f)
                            Szunyi.Features.FeatureManipulation.GenBankMetaDataManipulation.AddFeature(Seq, f)
                            nofiATG += 1
                        End If

                    Next
                End If
            Next
            Szunyi.IO.Export.SaveSequencesToSingleGenBank(Seqs)
        Next
    End Sub
    Private Sub AnnotateStrainKeyStartendStrandNameToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles AnnotateStrainKeyStartendStrandNameToolStripMenuItem.Click
        Dim Seqs = Me.GetSeqsFromSelectedList(True)
        If IsNothing(Seqs) = True Then Exit Sub

        Dim file = Szunyi.IO.Files.Filter.SelectFile
        If IsNothing(file) = True Then Exit Sub
        Dim res = Szunyi.Features.FeatureManipulation.Annotate.Annotate_Strain_Key_Start_end_Strand_Name(Seqs, file)
        Szunyi.IO.Export.SaveSequencesToSingleGenBank(res)
    End Sub
    Private Sub DoraPOXToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles DoraPOXToolStripMenuItem.Click
        Dim Seqs = Me.GetSeqsFromSelectedList(True)
        If IsNothing(Seqs) = True Then Exit Sub

        Dim file = Szunyi.IO.Files.Filter.SelectFile
        If IsNothing(file) = True Then Exit Sub
        Dim res = Szunyi.Features.FeatureManipulation.Annotate.Annotate_Dora_POX(Seqs, file)
        Szunyi.IO.Export.SaveSequencesToSingleGenBank(res)
    End Sub

    Private Sub FilterForOnlyIntronicReadsToolStripMenuItem_Click(sender As Object, e As EventArgs)
        Dim Bams = Szunyi.IO.Files.Filter.SelectFiles("Select Bam Files", Szunyi.Constants.Files.BAM)
        If IsNothing(Bams) = True Then Exit Sub
        Dim SeqIDs = Szunyi.BAM.Bam_Basic_IO.Headers.Get_Reference_SeqIDS(Bams)
        Dim out As New List(Of Bio.IO.SAM.SAMAlignedSequence)
        Dim Headers = Bam_Basic_IO.Headers.Get_Header(Bams)
        Dim f1 As New CheckBoxForStringsFull(SeqIDs, -1)
        If f1.ShowDialog = DialogResult.OK Then
            For Each Sam In Szunyi.BAM.Bam_Basic_IO.Import.Parse(Bams, f1.SelectedStrings)
                If IsNothing(Sam) = False Then
                    If IsNothing(Szunyi.Alignment.BAM_SAM.Filter.Get_Intronic(Sam, 10, 10)) = False Then
                        out.Add(Sam)
                    End If
                End If

            Next
        End If

        Dim x As New Bio.IO.SAM.SAMFormatter()
        Dim s = Szunyi.IO.Files.Save.SelectSaveFile(Szunyi.Constants.Files.SAM)
        If IsNothing(s) = True Then Exit Sub
        Dim str As New FileStream(s.FullName, FileMode.CreateNew)
        x.Format(str, out, Headers.First)
    End Sub


    Private Function IsContains(ByRef firsts As IEnumerable(Of FeatureItem), ByRef mRNA As IEnumerable(Of FeatureItem),
                               ByRef Seq As Bio.ISequence) As String
        Dim str As New StringBuilder
        For Each first In firsts
            For Each item In mRNA
                If first.Location.LocationStart > item.Location.LocationStart Or first.Location.LocationEnd < item.Location.LocationEnd Then
                    If first.Location.SubLocations.Count <> 0 AndAlso item.Location.SubLocations.Count <> 0 Then
                        If first.Location.SubLocations.First.Operator = item.Location.SubLocations.First.Operator Then
                            str.Append(first.Key).Append(vbTab)
                            str.Append(first.Qualifiers(StandardQualifierNames.GeneSymbol).First).Append(vbTab)
                            str.Append(Szunyi.Location.Common.GetLocationString(first.Location)).Append(vbTab)
                            str.Append(item.Key).Append(vbTab)
                            str.Append(item.Qualifiers(StandardQualifierNames.GeneSymbol).First).Append(vbTab)
                            str.Append(Szunyi.Location.Common.GetLocationString(item.Location)).AppendLine()
                            Correct(first, item, Seq)
                        End If

                    Else
                        str.Append(first.Key).Append(vbTab)
                        str.Append(first.Qualifiers(StandardQualifierNames.GeneSymbol).First).Append(vbTab)
                        str.Append(Szunyi.Location.Common.GetLocationString(first.Location)).Append(vbTab)
                        str.Append(item.Key).Append(vbTab)
                        str.Append(item.Qualifiers(StandardQualifierNames.GeneSymbol).First).Append(vbTab)
                        str.Append(Szunyi.Location.Common.GetLocationString(item.Location)).AppendLine()
                        Correct(first, item, Seq)
                        Dim alf As Int16 = 54

                    End If

                End If
            Next
        Next

        Return str.ToString
    End Function

    Private Function Correct(ByVal BAD As FeatureItem, ByRef GOOD As FeatureItem, ByRef Seq As Bio.ISequence) As FeatureItem
        Dim s, e As Integer
        If BAD.Location.LocationStart < GOOD.Location.LocationStart Then
            s = BAD.Location.LocationStart
        Else
            s = GOOD.Location.LocationStart
        End If
        If BAD.Location.LocationEnd > GOOD.Location.LocationEnd Then
            e = BAD.Location.LocationEnd
        Else
            e = GOOD.Location.LocationEnd
        End If
        Dim x1 = Szunyi.Location.Common.GetLocationString(BAD.Location)
        Dim l As Bio.IO.GenBank.Location
        If x1.Contains("compl") = True Then
            l = Szunyi.Location.Common.Get_Location("complement(" & s & ".." & e & ")")
        Else
            l = Szunyi.Location.Common.Get_Location(s & ".." & e)
        End If
        Dim f As New FeatureItem(BAD.Key, l)
        Szunyi.Features.FeatureManipulation.MergeFeatures.Merge2Features(BAD, f, True)
        Dim md As GenBankMetadata = Seq.Metadata.First.Value
        For i1 = 0 To md.Features.All.Count - 1

            Dim x = md.Features.All(i1)

            If x.Key = BAD.Key Then
                If x.Qualifiers.ContainsKey(StandardQualifierNames.GeneSymbol) Then

                    If x.Qualifiers(StandardQualifierNames.GeneSymbol).First = BAD.Qualifiers(StandardQualifierNames.GeneSymbol).First And
x.Location.LocationStart = BAD.Location.LocationStart And x.Location.LocationEnd = BAD.Location.LocationEnd Then


                        md.Features.All.Add(f)
                        md.Features.All.Remove(x)
                        Return Nothing
                    End If
                End If
            End If

        Next


        Return f
    End Function

    Private Function Get_Errors(gene As IEnumerable(Of FeatureItem), Type As String, g As List(Of FeatureItem)) As String
        Dim str As New StringBuilder
        If gene.Count = 0 Then
            str.Append(g.First.Qualifiers(StandardQualifierNames.GeneSymbol).First).Append(vbTab)
            str.Append("No Feature").Append(vbTab)
            str.Append(Type).Append(vbTab).Append(g.First).AppendLine()
        ElseIf gene.Count > 1 Then
            For Each Item In gene
                str.Append(g.First.Qualifiers(StandardQualifierNames.GeneSymbol).First).Append(vbTab)
                str.Append("More").Append(vbTab)
                Dim l = Szunyi.Features.FeatureManipulation.GetLocations.LociBuilder.GetLocationString(Item.Location)
                str.Append(Type).Append(vbTab).Append(l).AppendLine()
            Next
        End If
        Return str.ToString
    End Function

    Private Iterator Function get_gS(Feats As List(Of FeatureItem)) As IEnumerable(Of List(Of FeatureItem))

        Dim gr = From x In Feats Group By x.Qualifiers(StandardQualifierNames.GeneSymbol).First, x.Location.Operator Into Group

        For Each g In gr
            Yield g.Group.ToList

        Next
    End Function

    Private Sub IntronToolStripMenuItem2_Click(sender As Object, e As EventArgs)
        Dim Seqs = Me.GetSeqsFromSelectedList(True)
        If IsNothing(Seqs) = True Then Exit Sub
        Dim cSeqs = Szunyi.Sequences.SequenceManipulation.Common.CloneSeqs(Seqs)

        For Each Seq In cSeqs
            Dim Features = Szunyi.Features.FeatureManipulation.GetFeaturesByType.GetFeturesByTypeFromSeq(Seq, StandardFeatureKeys.MessengerRna)
            Dim FeaturesToRemove As New List(Of FeatureItem)

            For Each Feat In Features
                Dim Exons = Szunyi.Features.FeatureManipulation.GetLocations.GetCDSExonsLocations(Feat)
                Dim Min = From x In Exons Where (x.LocationEnd - x.LocationStart) > 0

                If Min.Count <> Exons.Count Then

                    FeaturesToRemove.Add(Feat)
                    If Min.Count > 0 Then
                        Dim RealLoci = Szunyi.Location.Common.GetLocation(Min.ToList, Feat.Location.Operator)

                        Dim x As New FeatureItem(StandardFeatureKeys.MessengerRna, RealLoci)
                        Szunyi.Features.FeatureManipulation.MergeFeatures.Merge2Features(Feat, x, False)
                        Szunyi.Features.FeatureManipulation.GenBankMetaDataManipulation.AddFeature(Seq, x)
                    End If
                End If

            Next
            For Each h In FeaturesToRemove
                Szunyi.Features.FeatureManipulation.GenBankMetaDataManipulation.Remove_Feature(Seq, h)
                Dim kj As Int16 = 54
            Next
        Next
        Szunyi.IO.Export.SaveSequencesToSingleGenBank(cSeqs)
    End Sub

    Private Sub EnhancersToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles EnhancersToolStripMenuItem.Click
        Dim Seqs = Me.GetSeqsFromSelectedList(True)
        If IsNothing(Seqs) = True Then Exit Sub
        Dim str As New System.Text.StringBuilder
        Dim Header = Split("ID,mRNA Location,Element Location,Element Type,Relative Position,p-value", ",")
        str.Append(Szunyi.Text.General.GetText(Header, ",")).AppendLine()
        Dim t As New Szunyi.Features.Motifs.DNA_Motifs
        For Each Seq In Seqs
            Dim Enhancers = Szunyi.Features.FeatureManipulation.GetFeaturesByType.GetFeturesByTypeFromSeq(Seq, StandardFeatureKeys.Enhancer)
            Dim mRNAs = Szunyi.Features.FeatureManipulation.GetFeaturesByType.GetFeturesByTypeFromSeq(Seq, StandardFeatureKeys.MessengerRna)

            For Each loci In Enhancers
                Dim md = t.NamesWithPositions(loci.Label.Replace(Chr(34), ""))
                Dim cmRNAs As List(Of FeatureItem)

                If loci.Location.IsComplementer = True Then

                    cmRNAs = (From x In mRNAs Where x.Location.LocationEnd + 30 - md >= loci.Location.LocationStart And x.Location.LocationEnd - 30 + md < loci.Location.LocationStart And x.Location.IsComplementer = loci.Location.IsComplementer).ToList

                Else
                    cmRNAs = (From x In mRNAs Where x.Location.LocationStart - 30 + md <= loci.Location.LocationStart And x.Location.LocationStart + 30 + md > loci.Location.LocationStart And x.Location.IsComplementer = loci.Location.IsComplementer).ToList



                End If


                For Each cmRNA In cmRNAs
                    str.Append(cmRNA.Qualifiers(StandardQualifierNames.Note).First).Append(vbTab)
                    str.Append(Szunyi.Location.Common.GetLocationString(cmRNA)).Append(vbTab)
                    str.Append(Szunyi.Location.Common.GetLocationString(loci)).Append(vbTab)
                    str.Append(loci.Label).Append(vbTab)
                    If loci.Location.IsComplementer = True Then
                        str.Append(cmRNA.Location.LocationEnd - loci.Location.LocationEnd).Append(vbTab)
                    Else
                        str.Append(loci.Location.LocationStart - cmRNA.Location.LocationStart).Append(vbTab)
                    End If
                    str.Append(loci.Qualifiers(StandardQualifierNames.Note).First).Append(vbTab)
                    str.Append(loci.Qualifiers(StandardQualifierNames.Note).Last).Append(vbTab)
                    str.AppendLine()
                Next
            Next
        Next
        Clipboard.SetText(str.ToString)
    End Sub

    Private Sub FimoToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles FimoToolStripMenuItem.Click
        Dim Seqs = Me.GetSeqsFromSelectedList(True)
        If IsNothing(Seqs) = True Then Exit Sub
        Dim cSeqs = Szunyi.Sequences.SequenceManipulation.Common.CloneSeqs(Seqs)
        Dim File = Szunyi.IO.Files.Filter.SelectFile("Select File", Szunyi.Constants.Files.All_TAB_Like)
        If IsNothing(File) = True Then Exit Sub
        Dim mRNAs = Szunyi.Features.FeatureManipulation.GetFeaturesByType.GetFeturesByTypeFromSeq(Seqs.First, StandardFeatureKeys.MessengerRna)
        Dim nofGood As Integer = 0
        Dim NofLine As Integer = 0
        Dim t As New Szunyi.Features.Motifs.DNA_Motifs
        For Each Line In Szunyi.IO.Import.Text.ParseNotFirst(File, 1)
            NofLine += 1
            Dim s = Split(Line, vbTab)
            Dim loci = Szunyi.Location.Common.GetLocation(s(2), s(3), s(4))

            Dim feats As List(Of FeatureItem)
            Dim md = t.NamesWithPositions(s(0))
            If loci.IsComplementer = True Then
                feats = (From x In mRNAs Where x.Location.LocationEnd + 30 - md >= loci.LocationStart And x.Location.LocationEnd - 30 + md < loci.LocationStart And x.Location.IsComplementer = loci.IsComplementer).ToList

            Else
                feats = (From x In mRNAs Where x.Location.LocationStart - 30 + md <= loci.LocationStart And x.Location.LocationStart + 30 + md > loci.LocationStart And x.Location.IsComplementer = loci.IsComplementer).ToList

            End If


            For Each Feat In feats
                nofGood += 1
                If loci.IsComplementer = True Then
                    Dim kj As Int16 = 43
                End If
                Dim x As New FeatureItem(StandardFeatureKeys.Enhancer, loci)
                Dim ls As New List(Of String)
                ls.Add("p-value=" & s(6))
                ls.Add("Sequence=" & s(8))
                Dim Labels As New List(Of String)
                Labels.Add(s(0))
                x.Qualifiers(StandardQualifierNames.Label) = Labels
                x.Qualifiers(StandardQualifierNames.Note) = ls
                Szunyi.Features.FeatureManipulation.GenBankMetaDataManipulation.AddFeature(cSeqs.First, x)
                Exit For


            Next
        Next

        Szunyi.IO.Export.SaveSequencesToSingleGenBank(cSeqs)
    End Sub


    Private Sub RegulatoryToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles RegulatoryToolStripMenuItem.Click
        Dim seqs = Me.GetSeqsFromSelectedList(True)
        If IsNothing(seqs) = True Then Exit Sub
        Dim str As New System.Text.StringBuilder
        For Each seq In seqs
            Dim Feats = Szunyi.Features.FeatureManipulation.GetFeaturesByType.GetFeturesByTypeFromSeq(seq, "regulatory")
            Dim mrNAS = Szunyi.Features.FeatureManipulation.GetFeaturesByType.GetFeturesByTypeFromSeq(seq, StandardFeatureKeys.MessengerRna)
            mrNAS = (From x In mrNAS Where x.Qualifiers.ContainsKey(StandardQualifierNames.GeneSymbol)).ToList
            Dim genes = Szunyi.Features.FeatureManipulation.GetFeaturesByType.GetFeturesByTypeFromSeq(seq, StandardFeatureKeys.Gene)
            genes = (From x In genes Where x.Qualifiers.ContainsKey(StandardQualifierNames.GeneSymbol)).ToList

            For Each feat In Feats
                If feat.Qualifiers.ContainsKey(StandardQualifierNames.GeneSymbol) Then
                    str.Append(feat.Qualifiers(StandardQualifierNames.GeneSymbol).First).Append(vbTab)
                    Dim cmRNA = From x In mrNAS Where x.Qualifiers(StandardQualifierNames.GeneSymbol).First.Contains(feat.Qualifiers(StandardQualifierNames.GeneSymbol).First) And x.Location.IsComplementer = feat.Location.IsComplementer
                    If cmRNA.Count = 0 Then
                        cmRNA = From x In genes Where x.Qualifiers(StandardQualifierNames.GeneSymbol).First.Contains(feat.Qualifiers(StandardQualifierNames.GeneSymbol).First) And x.Location.IsComplementer = feat.Location.IsComplementer
                    End If
                    str.Append(Szunyi.Location.Common.GetLocationString(cmRNA.First.Location)).Append(vbTab)
                    str.Append(Szunyi.Location.Common.GetLocationString(feat.Location)).Append(vbTab)
                    Dim cSeq = Szunyi.Sequences.SequenceManipulation.SeqsToString.FromFeature(seq, feat)
                    Dim s = Szunyi.Sequences.SequenceManipulation.SeqsToString.GetSeqAsString(cSeq)
                    str.Append(cSeq).Append(vbTab)
                    If feat.Location.IsComplementer = True Then
                        str.Append(feat.Location.LocationEnd - cmRNA.First.Location.LocationStart)
                    Else
                        str.Append(cmRNA.First.Location.LocationEnd - feat.Location.LocationStart)
                    End If
                Else
                    str.Append(vbTab).Append(vbTab)
                    Dim cSeq = Szunyi.Sequences.SequenceManipulation.SeqsToString.FromFeature(seq, feat)
                    Dim s = Szunyi.Sequences.SequenceManipulation.SeqsToString.GetSeqAsString(cSeq)
                    str.Append(cSeq).Append(vbTab)
                End If

                str.Append(vbTab)
                str.Append(feat.Qualifiers("regulatory_class").First).AppendLine()

            Next
        Next
        Clipboard.SetText(str.ToString)
    End Sub


    Private Sub FimoToolStripMenuItem1_Click(sender As Object, e As EventArgs) Handles FimoToolStripMenuItem1.Click
        Dim Meme_File = Szunyi.IO.Files.Filter.SelectFile("Select Meme File!")
        Dim lMeme_File = Szunyi.IO.Linux.Get_FileName(Meme_File)
        Dim FastaFile = Szunyi.IO.Files.Filter.SelectFile("Select Fasta File!", Szunyi.Constants.Files.Fasta)

        Dim lFasta_File = Szunyi.IO.Linux.Get_FileName(FastaFile)
        Dim THresh = InputBox("Enter Threshold")
        Dim nFIle As New FileInfo(FastaFile.DirectoryName & "\" & THresh)
        Dim DIr = Szunyi.IO.Linux.Get_FileName(nFIle)
        Dim c = "fimo --oc " & DIr & " -thresh " & THresh & " " & lMeme_File & " " & lFasta_File
        Clipboard.SetText(c)
    End Sub


    Private Sub ByRowToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles ByRowToolStripMenuItem.Click
        Dim Dir = Szunyi.IO.Directory.Get_Folder
        If IsNothing(Dir) = True Then Exit Sub
        Dim All_Files = Szunyi.IO.Directory.GetAllFilesFromFolder_Recursive(Dir.FullName)
        If All_Files.Count = 0 Then Exit Sub
        Dim extensions = (From x In All_Files Select x.Extension).ToList.Distinct

        Dim f1 As New CheckBoxForStringsFull(extensions.ToList, -1)
        Dim str As New System.Text.StringBuilder
        If f1.ShowDialog = DialogResult.OK Then
            For Each subdir In Dir.GetDirectories
                str.Append(subdir.Name).Append(vbTab)
                Dim out As New List(Of FileInfo)
                For Each File In subdir.GetFiles
                    If f1.SelectedStrings.Contains(File.Extension) Then
                        out.Add(File)
                        str.Append(File.Name).Append(vbTab)
                    End If
                Next
                str.AppendLine()
            Next
            If str.Length > 0 Then Clipboard.SetText(str.ToString)
        End If
    End Sub

    Private Sub BamToBedToolStripMenuItem_Click_1(sender As Object, e As EventArgs) Handles BamToBedToolStripMenuItem.Click
        Dim Files = Szunyi.IO.Files.Filter.SelectFiles("", Szunyi.Constants.Files.BAM)
        Dim str As New System.Text.StringBuilder
        For Each File In Files
            Dim nFIle = Szunyi.IO.Files.Get_New_FileName.GetNewFile(File, ".bed")
            str.Append(" bedtools bamtobed -i " & Szunyi.IO.Linux.Get_FileName(File) & " > " & Szunyi.IO.Linux.Get_FileName(nFIle)).AppendLine()

        Next
        Clipboard.SetText(str.ToString)
    End Sub
    Private Sub Gff3ToolStripMenuItem1_Click(sender As Object, e As EventArgs) Handles Gff3ToolStripMenuItem1.Click
        Dim seqs = Me.GetSeqsFromSelectedList(True)
        If IsNothing(seqs) = True Then Exit Sub
        Dim FeatLists = Me.GetSelectedFeatureList(True)
        If IsNothing(FeatLists) = True Then Exit Sub
        Dim feats = From x In FeatLists.First.Features


        Dim featsII = (From x1 In feats Select x1.Feature).ToList
        featsII = (From x In featsII Order By x.Location.LocationStart).ToList
        Dim gff As New Szunyi.Sequences.Gff.GFFFormatter(Nothing, Nothing)
        Dim s = gff.Export_Wo_Parents(featsII, seqs.First)
        Clipboard.SetText(s)
        Dim alf As Int16 = 43

    End Sub

    Private Sub InnerORFsCheckToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles InnerORFsCheckToolStripMenuItem.Click
        Dim Seqs = Me.GetSeqsFromSelectedList(True)
        If IsNothing(Seqs) = True Then Exit Sub
        Dim DiffFeatures = Szunyi.Features.FeatureManipulation.Key.Get_All_Different_Keys(Seqs)
        Dim f1 As New CheckBoxForStringsFull(DiffFeatures, -1)
        If f1.ShowDialog <> DialogResult.OK Then Exit Sub
        Dim AAs As New List(Of Bio.ISequence)

        Dim Full_ORFs As New List(Of Szunyi.DNA.ORF)
        Dim FUll_AAs As New List(Of Bio.ISequence)
        Dim NAs As New List(Of Bio.ISequence)
        Dim str As New System.Text.StringBuilder
        For Each Seq In Seqs
            Dim Feats = Szunyi.Features.FeatureManipulation.GetFeaturesByType.GetFeturesByTypesFromSeq(Seq, f1.SelectedStrings)
            Dim CDSs = Szunyi.Features.FeatureManipulation.GetFeaturesByType.GetFeturesByTypeFromSeq(Seq, StandardFeatureKeys.CodingSequence)

            For Each Feat In Feats
                Dim Desc = Szunyi.Features.FeatureManipulation.Qulifiers.Get_Description(Feat, Szunyi.Constants.Features.Get_Description_Gene_Note, True, False, False)
                str.Append(Desc)

                Dim Feat_Seq = Feat.GetSubSequence(Seq)
                Dim ORFs = Szunyi.DNA.ORF_Finding.Get_All_ORFs(Seq, Feat, Szunyi.DNA.Frames.fr, True, False)
                Szunyi.DNA.ORF_Manipulation.Set_Inners(ORFs)
                Szunyi.DNA.ORF_Manipulation.Set_Names(ORFs, Desc)
                Dim Longest = Szunyi.DNA.ORF_Manipulation.Get_Longest(ORFs)
                Dim First = Szunyi.DNA.ORF_Manipulation.Get_First(ORFs)
                Full_ORFs.AddRange(Szunyi.DNA.ORF_Manipulation.Get_Fulls(ORFs))
                Dim Inners = Szunyi.DNA.ORF_Manipulation.Get_Inners(ORFs)
                Dim cCDSs = From x In CDSs Where x.Location.LocationStart >= Feat.Location.LocationStart And
                                               x.Location.LocationEnd <= Feat.Location.LocationEnd And
                                               x.Location.IsComplementer = Feat.Location.IsComplementer

                str.Append(Szunyi.DNA.ORF_Manipulation.Get_As_TSV(Longest)).Append(vbTab)
                str.Append(Szunyi.DNA.ORF_Manipulation.Get_As_TSV(First)).Append(vbTab)
                For Each CDS In cCDSs
                    str.Append(CDS.Qualifiers(StandardQualifierNames.GeneSymbol).First).Append(",")
                Next
                AAs.AddRange(Szunyi.DNA.ORF_Manipulation.Get_AAs(ORFs))
                NAs.AddRange(Szunyi.DNA.ORF_Manipulation.Get_NAs(ORFs))

                str.AppendLine()
            Next

        Next
        FUll_AAs = Szunyi.DNA.ORF_Manipulation.Get_AAs(Full_ORFs)
        Dim uAAs = Szunyi.Sequences.SequenceManipulation.UniqueDistinct.Get1CopyBySeqAndID(FUll_AAs)
        Dim d = Szunyi.Sequences.SequenceManipulation.UniqueDistinct.GetDuplicatedSeqsByID(FUll_AAs)
        Dim uS = Szunyi.Sequences.SequenceManipulation.UniqueDistinct.Get1CopyByID(uAAs)
        Dim tmp = Szunyi.Sequences.SequenceManipulation.Common.GetSeqsAsBioSeq(FUll_AAs)
        Dim uAAs2 = Szunyi.Sequences.SequenceManipulation.UniqueDistinct.Get1CopyBySeq(tmp)

        Dim k1 = Szunyi.Sequences.SequenceManipulation.UniqueDistinct.Get1CopyByID(AAs)
        Dim k2 = Szunyi.Sequences.SequenceManipulation.UniqueDistinct.GetDuplicatedSeqsBySeq(AAs)
        Clipboard.SetText(str.ToString)
        Szunyi.IO.Export.SaveSequencesToSingleFasta(FUll_AAs)

    End Sub

    Private Sub LongestToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles LongestToolStripMenuItem.Click
        Dim Seqs = Me.GetSeqsFromSelectedList(True)
        If IsNothing(Seqs) = True Then Exit Sub
        Dim str As New System.Text.StringBuilder
        Dim Out As New List(Of Bio.ISequence)
        Dim Longest As New List(Of Bio.ISequence)
        For Each Seq In Seqs
            Dim ORFs = Szunyi.DNA.ORF_Finding.Get_All_ORFs(Seq, Szunyi.DNA.Frames.fr, True, False)
            Dim l = Szunyi.DNA.ORF_Manipulation.Get_Longest(ORFs)
            Longest.Add(l.AASeq)
            Dim fORFs = Szunyi.DNA.ORF_Manipulation.Get_Fulls(ORFs)
            Dim AASeqs = Szunyi.DNA.ORF_Manipulation.Get_AAs(fORFs)
            Dim Goods = Szunyi.Sequences.SequenceManipulation.SelectBy.LongerThan(AASeqs, 20)
            Out.AddRange(Goods)
        Next

        Dim d = Szunyi.Sequences.SequenceManipulation.UniqueDistinct.Get1CopyBySeq(Out)
        Clipboard.SetText(Szunyi.Text.General.GetText(Szunyi.Sequences.SequenceManipulation.SeqsToString.GetSeqsAsFasta(Out)))

        Clipboard.SetText(Szunyi.Text.General.GetText(Szunyi.Sequences.SequenceManipulation.SeqsToString.GetSeqsAsFasta(Longest)))

        Dim kj As Int16 = 54

    End Sub

    Private Sub ByIDToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles ByIDToolStripMenuItem.Click
        Dim Fasta_files = Szunyi.IO.Files.Filter.SelectFiles("Select", Szunyi.Constants.Files.Fasta)
        Dim log As New List(Of String)
        For Each f In Fasta_files
            Dim Seqs = Szunyi.IO.Import.Sequence.FromFile(f)
            Dim DUPL = Szunyi.Sequences.SequenceManipulation.UniqueDistinct.Get1CopyByID(Seqs)
            If DUPL.Count <> Seqs.Count Then
                Szunyi.IO.Export.SaveAsSimpleFasta(DUPL, f)
                log.Add(f.Name)
            Else
                ' MsgBox("All Are Unique!")
            End If

        Next
        Clipboard.SetText(Szunyi.Text.General.GetText(log))

    End Sub



    Private Sub DeleteQulifiersExceptToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles DeleteQulifiersExceptToolStripMenuItem.Click
        Dim seqs = Me.GetSeqsFromSelectedList(True)
        If IsNothing(seqs) = True Then Exit Sub
        Dim cSeqs = Szunyi.Sequences.SequenceManipulation.Common.CloneSeqs(seqs)
        Dim QulaToDelete As New List(Of String)
        Dim QualNames = Szunyi.Features.FeatureManipulation.Qulifiers.Get_Distinct_Qulifiers(cSeqs)
        Dim AllfeatKey = Szunyi.Features.FeatureManipulation.GenBankMetaDataManipulation.Get_Features_Keys(seqs)

        Dim fa As New CheckBoxForStringsFull(AllfeatKey, -1, "Features not to delete")
        If fa.ShowDialog <> DialogResult.OK Then Exit Sub
        Dim f1 As New CheckBoxForStringsFull(QualNames, -1)
        If f1.ShowDialog = DialogResult.OK Then
            For Each Seq In cSeqs
                For Each Feat In Szunyi.Features.FeatureManipulation.GetFeaturesByType.GetFeturesByTypesFromSeq(Seq, StandardFeatureKeys.All.ToList)

                    If fa.SelectedStrings.Contains(Feat.Key) = False Then
                        For Each item In f1.SelectedStrings
                            If Feat.Qualifiers.ContainsKey(item) = True Then
                                Feat.Qualifiers.Remove(item)
                            End If
                        Next
                    End If

                Next

            Next
        End If
        Szunyi.IO.Export.SaveSequencesToSingleGenBank(cSeqs)
    End Sub

    Private Sub SelectedFeaturesToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles SelectedFeaturesToolStripMenuItem.Click
        Dim seqs = Me.GetSeqsFromSelectedList(True)
        If IsNothing(seqs) = True Then Exit Sub
        Dim cSeqs = Szunyi.Sequences.SequenceManipulation.Common.CloneSeqs(seqs)
        Dim QulaToDelete As New List(Of String)
        Dim QualNames = Szunyi.Features.FeatureManipulation.Qulifiers.Get_Distinct_Qulifiers(cSeqs)
        Dim AllfeatKey = Szunyi.Features.FeatureManipulation.GenBankMetaDataManipulation.Get_Features_Keys(seqs)

        Dim FeaturesQulifiersToRename As New CheckBoxForStringsFull(QualNames, -1, "Qulifier to Rename")
        If FeaturesQulifiersToRename.ShowDialog <> DialogResult.OK Then Exit Sub
        Dim NewLabel As New CheckBoxForStringsFull(QualNames, 1, "New Qulifier Key")
        If NewLabel.ShowDialog <> DialogResult.OK Then Exit Sub
        Dim SelectedFeatures As New CheckBoxForStringsFull(AllfeatKey, 1)
        If SelectedFeatures.ShowDialog <> DialogResult.OK Then Exit Sub

        For Each Seq In cSeqs
            For Each Feat In Szunyi.Features.FeatureManipulation.GetFeaturesByType.GetFeturesByTypesFromSeq(Seq, StandardFeatureKeys.All.ToList)

                If SelectedFeatures.SelectedStrings.Contains(Feat.Key) = True Then
                    For Each item In FeaturesQulifiersToRename.SelectedStrings
                        If Feat.Qualifiers.ContainsKey(item) = True Then
                            Feat.Qualifiers.Add(NewLabel.SelectedStrings.First, Feat.Qualifiers(item))

                            Feat.Qualifiers.Remove(item)
                        End If
                    Next
                End If

            Next

        Next

        Szunyi.IO.Export.SaveSequencesToSingleGenBank(cSeqs)
    End Sub



    Private Sub ForStrnad1ToNegativeStrandToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles ForStrnad1ToNegativeStrandToolStripMenuItem.Click
        Dim FIles = Szunyi.IO.Files.Filter.SelectFiles("S", Szunyi.Constants.Files.gff3)
        If IsNothing(FIles) = True Then Exit Sub
        For Each File In FIles
            Dim out As New List(Of String)
            For Each Line In Szunyi.IO.Import.Text.Parse(File, "#")
                Dim s = Split(Line, vbTab)
                If s(3) = s(4) Then
                    If s(6) = "+" Then
                        s(4) = CInt(s(4)) + 1
                    Else
                        s(3) = CInt(s(3)) - 1
                    End If
                End If

                out.Add(Szunyi.Text.General.GetText(s, vbTab))
            Next
            Dim t = Szunyi.Text.General.GetText(out)
            Szunyi.IO.Export.SaveText(t, New FileInfo(File.FullName & "md.gff3"))
        Next
    End Sub

    Private Sub ByOrganimsToolStripMenuItem_Click(sender As Object, e As EventArgs)
        Dim BamFiles = Szunyi.IO.Files.Filter.SelectFiles("S", Szunyi.Constants.Files.SAM_BAM)
        If IsNothing(BamFiles) = True Then Exit Sub
        Dim pA As Long = 0
        Dim nPA As Long = 0

        Dim org = Szunyi.BAM.Bam_Basic_IO.Headers.Get_ReferenceSequences(BamFiles)
        Dim Organims_Names = (From x In org Select x.Name).ToList
        Dim nfile = Szunyi.IO.Files.Get_New_FileName.GetNewFile(BamFiles.First, ".filtered.bam")
        Dim str As New System.Text.StringBuilder
        str.Append("samtools view ").Append(Szunyi.IO.Linux.Get_FileName(BamFiles.First)).Append(" " & Organims_Names.First & " > ").Append(Szunyi.IO.Linux.Get_FileName(nfile))
        Clipboard.SetText(str.ToString)
    End Sub

    Private Sub TestToolStripMenuItem_Click(sender As Object, e As EventArgs)
        Dim out As New List(Of String)

        For i1 = 1 To 5
            For i2 = 1 To 5
                Dim s As String = i1 & i2 ' & i3 & i4 & i5
                out.Add(s)
            Next
        Next
        Dim res As New Dictionary(Of String, Integer)
        For Each item In out
            Dim k = GetNofDIff(item)
            If res.ContainsKey(k) = False Then res.Add(k, 0)
            res(k) += 1
        Next
        Dim alf As Int16 = 54
    End Sub
    Private Function GetNofDIff(s As String) As Integer
        Dim d = s.ToCharArray
        Dim res As New Dictionary(Of Char, Int16)
        For Each item In d
            If res.ContainsKey(item) = False Then res.Add(item, 0)
            res(item) += 1
        Next
        Dim out As String = ""
        Dim resII = From x In res Order By x.Value Descending
        For Each item In resII
            out = out & item.Value
        Next
        Return out
    End Function


    Private Sub VennForIgorAndGFF3ToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles VennForIgorAndGFF3ToolStripMenuItem.Click
        Dim Igor_Files = Szunyi.IO.Files.Filter.SelectFiles("Select Igor output strand, pos")
        If IsNothing(Igor_Files) = True Then Exit Sub
        Dim FalseTSS = Szunyi.IO.Files.Filter.SelectFile("Select False Sites", Szunyi.Constants.Files.gff3)
        Dim False_Locations As New List(Of Szunyi.Location.Basic_Location)
        If IsNothing(FalseTSS) = False Then

            False_Locations = Szunyi.Sequences.Gff.GFF_Parser_Shared.Get_Basic_Locations(FalseTSS)
        End If
        Dim Gff3_Files = Szunyi.IO.Files.Filter.SelectFiles(Szunyi.Constants.Files.gff3)
        If IsNothing(Gff3_Files) = True Then Exit Sub
        Dim Igors = Szunyi.IO.Import.Locations.Strand_Position_From_Files(Igor_Files)
        Dim Range = Szunyi.MyInputBox.GetInteger("Enter the range for modifying")
        If IsNothing(Range) = True Then
            Szunyi.Location.Basic_Location_Manipulation.Extend_Basic_Location(Igors, Range, Szunyi.Constants.Sort_Locations_By.TSS_PAS)
        End If
        Dim Index As Integer = 0
        For Each Igor In Igors
            Dim gffIndex As Integer = 0
            For Each gff3 In Gff3_Files
                Dim gff = Szunyi.Sequences.Gff.GFF_Parser_Shared.Get_Basic_Locations(gff3)
                Dim res = Szunyi.Venn.Venn_Location.FirstContains(Igor, gff)

                Dim OnlyIgor = res.First
                Dim resII = Szunyi.Venn.Venn_Location.FirstContains(OnlyIgor, False_Locations)

                Dim jj = Szunyi.Text.General.GetText(Szunyi.Location.Common.Get_Location_Strings(resII(0)))
                Dim nFIle = Szunyi.IO.Files.Get_New_FileName.Append_Before_Extension_wNew_Extension(Igor_Files(Index), gff3, "_e" & resII(0).Count & "OnlyIgor.tsv") ' Only Igor pk False
                Szunyi.IO.Export.SaveText(jj, nFIle)

                jj = Szunyi.Text.General.GetText(Szunyi.Location.Common.Get_Location_Strings(resII(2)))
                nFIle = Szunyi.IO.Files.Get_New_FileName.Append_Before_Extension_wNew_Extension(Igor_Files(Index), gff3, "_e" & resII(2).Count & "OnlyIgor_With_False.tsv")
                Szunyi.IO.Export.SaveText(jj, nFIle)

                jj = Szunyi.Text.General.GetText(Szunyi.Location.Common.Get_Location_Strings(res(1)))
                nFIle = Szunyi.IO.Files.Get_New_FileName.Append_Before_Extension_wNew_Extension(Igor_Files(Index), gff3, "_e" & res(1).Count & "Only_gff.tsv")
                Szunyi.IO.Export.SaveText(jj, nFIle)
                jj = Szunyi.Text.General.GetText(Szunyi.Location.Common.Get_Location_Strings(res(2)))
                nFIle = Szunyi.IO.Files.Get_New_FileName.Append_Before_Extension_wNew_Extension(Igor_Files(Index), gff3, "_e" & res(2).Count & "Common.tsv")
                Szunyi.IO.Export.SaveText(jj, nFIle)
                gffIndex += 1
            Next
            Index += 1
        Next





    End Sub

    Private Sub MRNACDSGeneToolStripMenuItem_Click(sender As Object, e As EventArgs)
        Dim Seqs = Me.GetSeqsFromSelectedList(True)
        If IsNothing(Seqs) = True Then Exit Sub
        Dim str As New System.Text.StringBuilder
        For Each Seq In Seqs
            Dim feats = Szunyi.Features.FeatureManipulation.GetFeaturesByType.GetFeturesByTypesFromSeq(Seq, Szunyi.Constants.Features.CDSs_mRNAs_Genes)
            Dim RFeats = (From x In feats Where x.Qualifiers.ContainsKey(StandardQualifierNames.GeneSymbol) = False).ToList
            Dim BFeats = (From x In feats Where x.Qualifiers.ContainsKey(StandardQualifierNames.GeneSymbol) = True).ToList
            Dim BFeats2 = (From x In BFeats Where x.Qualifiers(StandardQualifierNames.GeneSymbol).Count > 0).ToList


            For Each g In get_gS(BFeats2)
                Dim gene = From x In g Where x.Key = StandardFeatureKeys.Gene

                Dim CDS = From x In g Where x.Key = StandardFeatureKeys.CodingSequence

                Dim mRNA = From x In g Where x.Key = StandardFeatureKeys.MessengerRna

                str.Append(Get_Errors(gene, StandardFeatureKeys.Gene, g))
                str.Append(Get_Errors(CDS, StandardFeatureKeys.CodingSequence, g))
                str.Append(Get_Errors(mRNA, StandardFeatureKeys.MessengerRna, g))

                str.Append(IsContains(mRNA, CDS, Seq))
                str.Append(IsContains(gene, CDS, Seq))
                str.Append(IsContains(gene, mRNA, Seq))
            Next
        Next

        Dim alf As Int16 = 54
        Clipboard.SetText(str.ToString)
        Szunyi.IO.Export.SaveSequencesToSingleGenBank(Seqs)
    End Sub


    Private Function Get_Overlapped_Items(Locis As ILocation, ExtFeatureLists As List(Of ExtFeatureList), seq As Bio.ISequence) As String
        Dim str As New System.Text.StringBuilder
        Dim hjg = Szunyi.Location.Common.GetLocationString(Locis)
        str.Append(hjg).Append(vbTab)
        Dim Feat As New FeatureItem("a", hjg)
        Dim TheSeq = Feat.GetSubSequence(seq)
        For Each Item In ExtFeatureLists
            Dim FoundFullItems As New List(Of String)
            Dim FoundFullASItems As New List(Of String)
            Dim FoundPartialItems As New List(Of String)
            Dim FoundPartialASItems As New List(Of String)
            Dim ContainsFull = From x In Item.Features Where x.Feature.Location.LocationStart >= Feat.Location.LocationStart And
                                                                   x.Feature.Location.LocationEnd <= Feat.Location.LocationEnd

            For Each cf In ContainsFull
                If cf.Feature.Location.IsComplementer = Locis.IsComplementer Then
                    Dim tmpSeq = cf.Feature.GetSubSequence(seq)
                    If Szunyi.Text.Regexp.SimpleBoyerMooreSearch(TheSeq.ToArray, tmpSeq.ToArray) > -1 Then FoundFullItems.Add(cf.Feature.Label.Replace(Chr(34), ""))
                Else
                    Dim tmpSeq As Bio.ISequence
                    If cf.Feature.Location.IsComplementer = True Then
                        tmpSeq = cf.Feature.GetSubSequence(seq).GetReversedSequence
                    Else
                        tmpSeq = cf.Feature.GetSubSequence(seq).GetComplementedSequence
                    End If
                    If Szunyi.Text.Regexp.SimpleBoyerMooreSearch(TheSeq.ToArray, tmpSeq.ToArray) > -1 Then
                        FoundFullASItems.Add(cf.Feature.Label.Replace(Chr(34), ""))
                    End If


                End If

            Next
            Dim ContainsPartial = From x In Item.Features Where (x.Feature.Location.LocationStart >= Feat.Location.LocationStart And
                                                                      x.Feature.Location.LocationStart <= Feat.Location.LocationEnd) Or
                                                                      (x.Feature.Location.LocationEnd >= Feat.Location.LocationStart And
                                                                      x.Feature.Location.LocationEnd <= Feat.Location.LocationEnd)

            For Each cf In ContainsPartial
                If cf.Feature.Location.IsComplementer = Locis.IsComplementer Then
                    FoundPartialItems.Add(cf.Feature.Label.Replace(Chr(34), ""))
                Else
                    FoundPartialASItems.Add(cf.Feature.Label.Replace(Chr(34), ""))
                End If
            Next
            str.Append(Szunyi.Text.General.GetText(FoundFullItems, ",")).Append(vbTab)
            Dim diFF = FoundPartialItems.Except(FoundFullItems).ToList
            str.Append(Szunyi.Text.General.GetText(diFF, ",")).Append(vbTab)

            str.Append(Szunyi.Text.General.GetText(FoundFullASItems, ",")).Append(vbTab)

            If FoundFullASItems.Count = 0 Then

            Else

                diFF = FoundPartialASItems.Except(FoundFullASItems).ToList
                str.Append(Szunyi.Text.General.GetText(diFF, ","))
            End If
            str.Append(vbTab)
        Next
        Return str.ToString
    End Function

    Private Sub OneByOneToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles OneByOneToolStripMenuItem.Click
        Dim files = Szunyi.IO.Files.Filter.SelectFiles

        Dim Index As Integer = 1
        For Each file In files
            Dim out As New List(Of Bio.ISequence)
            For Each t In Szunyi.IO.Import.Text.ParseMoreLines(file, 4)
                Dim s = Split(t(0), " ").First
                Dim x As New Bio.Sequence(Alphabets.AmbiguousDNA, t(1))

                x.ID = s.Replace("@", "")
                out.Add(x)
            Next
            Dim Nfile = Szunyi.IO.Files.Get_New_FileName.GetNewFile(file, ".fasta")
            Szunyi.IO.Export.SaveSequencesToSingleFasta(out, Nfile)
        Next

    End Sub

    Private Function ReadCount(Files As List(Of FileInfo)) As String
        Dim ReadIDs As New List(Of List(Of String))
        Dim str As New System.Text.StringBuilder
        For Each File In Files
            str.Append(File.Name).Append(vbTab)
            Dim tmp As New List(Of String)
            For Each Sam In Szunyi.BAM.Bam_Basic_IO.Import.Parse(File)
                tmp.Add(Sam.QName)
            Next
            ReadIDs.Add(tmp.Distinct.ToList)
        Next

        Dim x = Szunyi.Math.Combinatory.GetBinaryPermutation(Files.Count)
        str.Append("Count").AppendLine()
        For Each Item In x
            Dim Res As New List(Of String)
            For i1 = 0 To Files.Count - 1
                If Item(i1) = True Then
                    Res = ReadIDs(i1)
                    Exit For
                End If
            Next
            For i1 = 0 To Files.Count - 1
                str.Append(Item(i1)).Append(vbTab)
                If Item(i1) = True Then

                    Res = Res.Intersect(ReadIDs(i1)).ToList

                Else
                    Res = Res.Except(ReadIDs(i1)).ToList
                End If
            Next
            str.Append(Res.Count).AppendLine()
        Next
        Return str.ToString
    End Function
#Region "Console"
    Private Sub BarCodeMinIonToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles BarCodeToolStripMenuItem.Click
        Dim Reads = Szunyi.IO.Files.Filter.SelectFiles(Constants.Files.FastQ, "Select Fastq Files")
        If IsNothing(Reads) = True Then Exit Sub
        Dim BarCodes = Szunyi.IO.Files.Filter.SelectFiles(Constants.Files.Fasta, "Select BarCode Files")
        If IsNothing(BarCodes) = True Then Exit Sub
        Dim str As New System.Text.StringBuilder
        For Each Read In Reads
            For Each Barcode In BarCodes
                str.Append("jsa.np.barcode -seq").Append(Read.FullName).Append(" -bc ").Append(Barcode.FullName).AppendLine()
            Next
        Next
        Clipboard.SetText(str.ToString)
        Beep()
    End Sub
    Private Sub AllToolStripMenuItem9_Click(sender As Object, e As EventArgs) Handles AllToolStripMenuItem9.Click
        Dim str As New System.Text.StringBuilder
        str.Append("cd /mnt/c/Linux/toullig/target/dist/toullig-1.0-alpha/").AppendLine()
        Dim status = Split("pass|failed|unclassified", "|")
        Dim Type = Split("template|complement|consensus|transcript")

        str.Append("bash toullig.sh fast5tofastq -status pass,failed,unclassified -type template,complement,consensus,transcript ")
        Dim Dir = Szunyi.IO.Directory.Get_Folder
        str.Append(Szunyi.IO.Linux.Get_FileName(Dir)).Append(" ")
        Dir = Szunyi.IO.Directory.Get_Folder
        str.Append(Szunyi.IO.Linux.Get_FileName(Dir)).Append(" ")
        Clipboard.SetText(str.ToString)
        Beep()
    End Sub
    Private Sub SRA_ToolKIt_ToSamToolStripMenuItem1_Click(sender As Object, e As EventArgs) Handles ToSamToolStripMenuItem1.Click
        Dim s = "C:\BlastViewer\Other_Programs\sratoolkit.2.8.2-1-win64\bin\fastq-dump.exe "
        Dim Files = Szunyi.IO.Files.Filter.SelectFiles("sa")
        Dim str As New System.Text.StringBuilder
        If IsNothing(Files) = False Then
            For Each file In Files
                str.Append(s).Append(file.FullName).AppendLine()
            Next
        End If
        Clipboard.SetText(str.ToString)
    End Sub
    Private Sub AlbacoreToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles AlbacoreToolStripMenuItem.Click
        Dim s As String = "C:\Program Files\Oxford Nanopore\ont-Albacore\read_fast5_basecaller.exe --flowcell FLO-MIN106 --kit SQK-LSK108 –-input -r --barcode"
        Dim Dir = Szunyi.IO.Directory.Get_Folder("Input")
        Dim dir_o = Szunyi.IO.Directory.Get_Folder("Out")
        s = s & Dir.FullName & " –-save_path " & dir_o.FullName & " -–worker_threads 6"
        Clipboard.SetText(s)
    End Sub

    ''' <summary>
    ''' Coverage All Variations
    ''' </summary>
    ''' <param name="sender"></param>
    ''' <param name="e"></param>
    Private Sub AllToolStripMenuItem10_Click(sender As Object, e As EventArgs) Handles AllToolStripMenuItem10.Click
        Dim Files = Szunyi.IO.Files.Filter.SelectFiles(Szunyi.Constants.Files.BAM)
        If IsNothing(Files) = True Then Exit Sub
        Dim str As New System.Text.StringBuilder
        For Each File In Files
            Dim nFIle = Szunyi.IO.Files.Get_New_FileName.GetNewFile(File, "Plus_Strand.bed")
            str.Append(" bedtools genomecov -split -ibam " & Szunyi.IO.Linux.Get_FileName(File) & " -d -strand + > " & Szunyi.IO.Linux.Get_FileName(nFIle)).AppendLine()
            nFIle = Szunyi.IO.Files.Get_New_FileName.GetNewFile(File, "Neg_Strand.bed")
            str.Append(" bedtools genomecov -split -ibam " & Szunyi.IO.Linux.Get_FileName(File) & " -d -strand - > " & Szunyi.IO.Linux.Get_FileName(nFIle)).AppendLine()
            nFIle = Szunyi.IO.Files.Get_New_FileName.GetNewFile(File, "Both_Strand.bed")
            str.Append(" bedtools genomecov -split -ibam " & Szunyi.IO.Linux.Get_FileName(File) & " -d  > " & Szunyi.IO.Linux.Get_FileName(nFIle)).AppendLine()

        Next
        If str.Length > 0 Then Clipboard.SetText(str.ToString)
        Beep()
    End Sub

    ''' <summary>
    ''' Coverage +- strand in one 
    ''' </summary>
    ''' <param name="sender"></param>
    ''' <param name="e"></param>
    Private Sub ToolStripMenuItem18_Click(sender As Object, e As EventArgs) Handles ToolStripMenuItem18.Click
        Dim Files = Szunyi.IO.Files.Filter.SelectFiles(Szunyi.Constants.Files.BAM)
        If IsNothing(Files) = True Then Exit Sub
        Dim str As New System.Text.StringBuilder
        For Each File In Files
            Dim nFIle = Szunyi.IO.Files.Get_New_FileName.GetNewFile(File, "Both_Strand.bed")
            str.Append(" bedtools genomecov -split -ibam " & Szunyi.IO.Linux.Get_FileName(File) & " -d  > " & Szunyi.IO.Linux.Get_FileName(nFIle)).AppendLine()

        Next
        If str.Length > 0 Then Clipboard.SetText(str.ToString)
        Beep()
    End Sub
    ''' <summary>
    ''' Coverage for + and + strand
    ''' </summary>
    ''' <param name="sender"></param>
    ''' <param name="e"></param>
    Private Sub ToolStripMenuItem19_Click(sender As Object, e As EventArgs) Handles ToolStripMenuItem19.Click
        Dim Files = Szunyi.IO.Files.Filter.SelectFiles(Szunyi.Constants.Files.BAM)
        If IsNothing(Files) = True Then Exit Sub
        Dim str As New System.Text.StringBuilder
        For Each File In Files
            Dim nFIle = Szunyi.IO.Files.Get_New_FileName.GetNewFile(File, "Plus_Strand.bed")
            str.Append(" bedtools genomecov -split -ibam " & Szunyi.IO.Linux.Get_FileName(File) & " -d -strand + > " & Szunyi.IO.Linux.Get_FileName(nFIle)).AppendLine()
            nFIle = Szunyi.IO.Files.Get_New_FileName.GetNewFile(File, "Neg_Strand.bed")
            str.Append(" bedtools genomecov -split -ibam " & Szunyi.IO.Linux.Get_FileName(File) & " -d -strand - > " & Szunyi.IO.Linux.Get_FileName(nFIle)).AppendLine()

        Next
        If str.Length > 0 Then Clipboard.SetText(str.ToString)
        Beep()
    End Sub
    ''' <summary>
    ''' Coverage + strand
    ''' </summary>
    ''' <param name="sender"></param>
    ''' <param name="e"></param>
    Private Sub ToolStripMenuItem20_Click(sender As Object, e As EventArgs) Handles ToolStripMenuItem20.Click
        Dim Files = Szunyi.IO.Files.Filter.SelectFiles(Szunyi.Constants.Files.BAM)
        If IsNothing(Files) = True Then Exit Sub
        Dim str As New System.Text.StringBuilder
        For Each File In Files
            Dim nFIle = Szunyi.IO.Files.Get_New_FileName.GetNewFile(File, "Plus_Strand.bed")
            str.Append(" bedtools genomecov -split -ibam " & Szunyi.IO.Linux.Get_FileName(File) & " -d -strand + > " & Szunyi.IO.Linux.Get_FileName(nFIle)).AppendLine()

        Next
        If str.Length > 0 Then Clipboard.SetText(str.ToString)
        Beep()
    End Sub
    ''' <summary>
    ''' Coverage - strand
    ''' </summary>
    ''' <param name="sender"></param>
    ''' <param name="e"></param>
    Private Sub ToolStripMenuItem21_Click(sender As Object, e As EventArgs) Handles ToolStripMenuItem21.Click
        Dim Files = Szunyi.IO.Files.Filter.SelectFiles(Szunyi.Constants.Files.BAM)
        If IsNothing(Files) = True Then Exit Sub
        Dim str As New System.Text.StringBuilder
        For Each File In Files
            Dim nFIle = Szunyi.IO.Files.Get_New_FileName.GetNewFile(File, "Neg_Strand.bed")
            str.Append(" bedtools genomecov -split -ibam " & Szunyi.IO.Linux.Get_FileName(File) & " -d -strand - > " & Szunyi.IO.Linux.Get_FileName(nFIle)).AppendLine()

        Next
        If str.Length > 0 Then Clipboard.SetText(str.ToString)
        Beep()
    End Sub
    Private Sub GTHToolStripMenuItem1_Click(sender As Object, e As EventArgs) Handles GTHToolStripMenuItem1.Click
        Dim FIle = Szunyi.IO.Files.Filter.SelectFile("Reference Fasta", Szunyi.Constants.Files.Fasta)

        If IsNothing(FIle) = True Then Exit Sub

        Dim Read_Files = Szunyi.IO.Files.Filter.SelectFiles("reads", Szunyi.Constants.Files.Fasta)

        Dim str As New System.Text.StringBuilder
        For Each Read_file In Read_Files
            Dim tmpFile = Szunyi.IO.Linux.Get_FileName(Read_file)
            Dim tmpReDfiLE = "/mnt/" & Read_file.FullName.Replace("\", "/").Replace("C:", "c")
            Dim thedir = Szunyi.IO.Linux.Get_FileName(Read_file.Directory)
            Dim OutPutFileName = thedir & "GTH_" & FIle.Name.Replace(FIle.Extension, "") & "_" & Read_file.Name.Replace(Read_file.Extension, ".gth")
            str.Append("gth -genomic " & tmpFile & " -cdnaforward -cdna " & tmpReDfiLE & " -o " & OutPutFileName).AppendLine()
        Next
        Clipboard.SetText(str.ToString)
    End Sub
#Region "GMAP"
    Private Sub FromAllDatabesToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles FromAllDatabesToolStripMenuItem.Click
        Dim Read_Files = Szunyi.IO.Files.Filter.SelectFiles(Szunyi.Constants.Files.Fasta_FastQ, "reads")
        If IsNothing(Read_Files) = True Then Exit Sub
        Dim OutDir = Szunyi.IO.Directory.Get_Folder
        Dim dir As New DirectoryInfo("c:\Users\" & Environment.UserName & "\AppData\Local\lxss\rootfs\usr\local\share")
        Dim kj = dir.GetDirectories
        Dim kj1 = dir.GetFiles
        Dim SelDIrs = From x In kj Where x.Name.Contains("_") Select x.Name
        Dim str As New System.Text.StringBuilder
        For Each DB In SelDIrs
            For Each Read_file In Read_Files
                Dim Linux_Read_File = Szunyi.IO.Linux.Get_FileName(Read_file)
                Dim Ref_File = Szunyi.IO.Linux.Get_FileName(Read_file)
                Dim Linux_Out_Dir = Szunyi.IO.Linux.Get_FileName(OutDir) & "/"
                Dim OutPutFileName = Linux_Out_Dir & DB & "_" & "GMAP_" & Read_file.Name.Replace(Read_file.Extension, ".sam")
                str.Append("gmap -d " & DB & " --nofails -f samse -t " & Environment.ProcessorCount - 1 & " " & Linux_Read_File & " > " & OutPutFileName).AppendLine()
            Next
        Next

        Clipboard.SetText(str.ToString)
    End Sub

    Private Sub FromCustomDatabasToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles FromCustomDatabasToolStripMenuItem.Click
        Dim Read_Files = Szunyi.IO.Files.Filter.SelectFiles(Szunyi.Constants.Files.Fasta_FastQ, "reads")
        If IsNothing(Read_Files) = True Then Exit Sub
        Dim OutDir = Read_Files.First.Directory

        Dim dir As New DirectoryInfo("c:\Users\" & Environment.UserName & "\AppData\Local\lxss\rootfs\usr\local\share")
        If dir.Exists = False Then
            dir = New DirectoryInfo("c:\Users\" & Environment.UserName & "\AppData\Local\Packages")
            For Each sdir In dir.GetDirectories
                Dim ndir As New DirectoryInfo(sdir.FullName & "\LocalState\rootfs")
                If ndir.Exists = True Then
                    dir = New DirectoryInfo(ndir.FullName & "\usr\local\share")
                    Exit For
                End If
            Next
        End If
        Dim kj = dir.GetDirectories
        Dim kj1 = dir.GetFiles
        Dim SelDIrs = (From x In kj Where x.Name.Contains("_") Select x.Name).ToList

        Dim f1 As New CheckBoxForStringsFull(SelDIrs, -1, "Select Databases")
        Dim str As New System.Text.StringBuilder
        If f1.ShowDialog = DialogResult.OK Then

            For Each DB In f1.SelectedStrings
                For Each Read_file In Read_Files
                    Dim Linux_Read_File = Szunyi.IO.Linux.Get_FileName(Read_file)
                    Dim Ref_File = Szunyi.IO.Linux.Get_FileName(Read_file)
                    Dim Linux_Out_Dir = Szunyi.IO.Linux.Get_FileName(OutDir) & "/"
                    Dim OutPutFile As New FileInfo(OutDir.FullName & "/" & DB & "_" & "GMAP_" & Read_file.Name.Replace(Read_file.Extension, ".sam"))
                    Dim Linux_Output_File = Szunyi.IO.Linux.Get_FileName(OutPutFile)
                    str.Append("gmap -d " & DB & " --nofails -f samse -t " & Environment.ProcessorCount - 1 & " " & Linux_Read_File & " > " & Linux_Output_File).AppendLine()
                Next
            Next


            Clipboard.SetText(str.ToString)
        End If


    End Sub

    Private Sub OneByOneToolStripMenuItem1_Click(sender As Object, e As EventArgs) Handles OneByOneToolStripMenuItem1.Click
        Dim Files = Szunyi.IO.Files.Filter.SelectFiles(Szunyi.Constants.Files.GenBank)
        If IsNothing(Files) = True Then Exit Sub
        Dim str As New System.Text.StringBuilder
        For Each FIle In Files
            Dim Seqs = Szunyi.IO.Import.Sequence.FromFile(FIle)
            For Each Seq In Seqs
                Dim DB_Name = Szunyi.Features.FeatureManipulation.GenBankMetaDataManipulation.Get_Common_Name(Seq) & "_" & Szunyi.Features.FeatureManipulation.GenBankMetaDataManipulation.Get_Accesion(Seq)
                DB_Name = DB_Name.Replace(" ", "_")
                Dim nFIle = Szunyi.IO.Files.Get_New_FileName.GetNewFile(FIle, Seq.ID.Replace(" ", "_") & ".fa")
                Dim LinuxFileName = Szunyi.IO.Linux.Get_FileName(nFIle)
                Szunyi.IO.Export.SaveAsSimpleFasta(Seq, nFIle)
                str.Append("gmap_build -d " & DB_Name & " " & LinuxFileName).AppendLine()
            Next

        Next
        If str.Length > 0 Then Clipboard.SetText(str.ToString)
    End Sub

    Private Sub MergeToolStripMenuItem3_Click(sender As Object, e As EventArgs) Handles MergeToolStripMenuItem3.Click
        Dim DB_NAME As String = InputBox("Enter Database Name, must contain character: _")
        If DB_NAME.Contains("_") = False Then Exit Sub
        Dim dir As New DirectoryInfo("c:\Users\" & Environment.UserName & "\AppData\Local\lxss\rootfs\usr\local\share")
        Dim kj = dir.GetDirectories
        Dim kj1 = dir.GetFiles
        Dim SelDIrs = (From x In kj Where x.Name.Contains("_") Select x.Name).ToList
        If SelDIrs.Contains(DB_NAME) = True Then
            MsgBox("Already Contains permission denied")
            Exit Sub
        End If
        Dim str As New System.Text.StringBuilder
        Dim File = Szunyi.IO.Files.Filter.SelectFile(Constants.Files.Fasta)

        If IsNothing(File) = True Then Exit Sub
        Dim LinuxFileName = Szunyi.IO.Linux.Get_FileName(File)
        str.Append("gmap_build -d " & DB_NAME & " " & LinuxFileName).AppendLine()
        Clipboard.SetText(str.ToString)
    End Sub

#End Region

#Region "Bowtie2"

    Private Sub PairedEndToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles PairedEndToolStripMenuItem.Click
        Dim d As New DirectoryInfo("c:\Users\" & Environment.UserName & "\AppData\Local\lxss\root\")
        Dim cFiles = Szunyi.IO.Directory.GetAllFilesFromFolder(d)
        Dim Names = From x In cFiles Select x Where x.Extension = ".bt2"

        Dim dbNames = (From x In Names Select x.Name.Split(".").First).Distinct
        Dim f1 As New CheckBoxForStringsFull(dbNames.ToList, -1)
        If f1.ShowDialog = DialogResult.OK Then
            Dim str As New System.Text.StringBuilder
            Dim Files = Szunyi.IO.Files.Filter.SelectFiles(Szunyi.Constants.Files.Fasta_FastQ_gz)
            If IsNothing(Files) = True Then Exit Sub
            For Each Gr In Szunyi.IO.Files.Group.Iterate_By_First_Parts(Files, "_")
                str.Append(Szunyi.Linux.Bowtie2.Paired_End(f1.SelectedStrings, Gr)).AppendLine()
            Next
            Clipboard.SetText(str.ToString)
        End If
        Beep()
    End Sub
    Private Sub OnlyAlignedToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles OnlyAlignedToolStripMenuItem.Click
        Dim d As New DirectoryInfo("c:\Users\" & Environment.UserName & "\AppData\Local\lxss\root\")
        Dim cFiles = Szunyi.IO.Directory.GetAllFilesFromFolder(d)
        Dim Names = From x In cFiles Select x Where x.Extension = ".bt2"

        Dim dbNames = (From x In Names Select x.Name.Split(".").First).Distinct
        Dim f1 As New CheckBoxForStringsFull(dbNames.ToList, -1)
        If f1.ShowDialog = DialogResult.OK Then
            Dim Files = Szunyi.IO.Files.Filter.SelectFiles(Szunyi.Constants.Files.Fasta_FastQ_gz)
            If IsNothing(Files) = True Then Exit Sub
            Dim res = Szunyi.Linux.Bowtie2.Get_Only_Aligned(f1.SelectedStrings, Files)
            If res <> String.Empty Then Clipboard.SetText(res)
        End If
        Beep()
    End Sub

    Private Sub AllToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles AllToolStripMenuItem.Click
        Dim d As New DirectoryInfo("c:\Users\" & Environment.UserName & "\AppData\Local\lxss\root\")
        Dim cFiles = Szunyi.IO.Directory.GetAllFilesFromFolder(d)
        Dim Names = From x In cFiles Select x Where x.Extension = ".bt2"

        Dim dbNames = (From x In Names Select x.Name.Split(".").First).Distinct
        Dim f1 As New CheckBoxForStringsFull(dbNames.ToList, -1)
        If f1.ShowDialog = DialogResult.OK Then
            Dim Files = Szunyi.IO.Files.Filter.SelectFiles(Szunyi.Constants.Files.Fasta_FastQ)
            If IsNothing(Files) = True Then Exit Sub
            Dim res = Szunyi.Linux.Bowtie2.Get_All(f1.SelectedStrings, Files)
            If res <> String.Empty Then Clipboard.SetText(res)
        End If
        Beep()
    End Sub
    Private Sub CreateIndexToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles CreateIndexToolStripMenuItem.Click
        Dim File = Szunyi.IO.Files.Filter.SelectFile(Constants.Files.Fasta)
        If IsNothing(File) = True Then Exit Sub
        Dim s = InputBox("Enter the name of the database")
        If s = String.Empty Then Exit Sub
        Dim res = Szunyi.Linux.Bowtie2.Get_Index(File, s)
        If res <> String.Empty Then Clipboard.SetText(res)
        Beep()
    End Sub
#End Region
#Region "SamTools"
    ' Regions
    Private Sub RegionToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles RegionToolStripMenuItem.Click
        Dim Files = Szunyi.IO.Files.Filter.SelectFiles(Szunyi.Constants.Files.SAM_BAM, "Sam/Bam Files")
        If IsNothing(Files) = True Then Exit Sub
        Dim File = Szunyi.IO.Files.Filter.SelectFile(Szunyi.Constants.Files.Other.BED, "Foramt= eg: chr1:2-5674")
        If IsNothing(File) = True Then Exit Sub


        Dim str As New System.Text.StringBuilder
        str.Append(Szunyi.Linux.SamTools.Extract_Regions_BED(Files, File))
        If str.Length <> 0 Then Clipboard.SetText(str.ToString)
        Beep()

    End Sub
    ' Index
    Private Sub FromFilesToolStripMenuItem1_Click(sender As Object, e As EventArgs) Handles FromFilesToolStripMenuItem1.Click
        Dim SamFiles = Szunyi.IO.Files.Filter.SelectFiles(Szunyi.Constants.Files.BAM, "Bam Files")
        If IsNothing(SamFiles) = True Then Exit Sub
        If IsNothing(SamFiles) = True Then Exit Sub
        Dim str As New System.Text.StringBuilder
        For Each File In SamFiles
            Dim LinuxFIleName = Szunyi.IO.Linux.Get_FileName(File)

            str.Append("samtools index " & LinuxFIleName).AppendLine()
            'view -bS file.sam | samtools sort - file_sorted
        Next
        Clipboard.SetText(str.ToString)
    End Sub
    ' Index
    Private Sub FromSubFoldersToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles FromSubFoldersToolStripMenuItem.Click
        Dim SamFiles = Szunyi.IO.Directory.GetAllFilesFromFolder_Recursive(".bam")
        If IsNothing(SamFiles) = True Then Exit Sub
        Dim str As New System.Text.StringBuilder
        For Each File In SamFiles
            Dim LinuxFIleName = Szunyi.IO.Linux.Get_FileName(File)

            str.Append("samtools index " & LinuxFIleName).AppendLine()
            'view -bS file.sam | samtools sort - file_sorted
        Next
        Clipboard.SetText(str.ToString)
    End Sub
    ' BamtToSam
    Private Sub FromFIlesToolStripMenuItem3_Click(sender As Object, e As EventArgs) Handles FromFIlesToolStripMenuItem3.Click
        Dim SamFiles = Szunyi.IO.Files.Filter.SelectFiles(Szunyi.Constants.Files.BAM, "Bam Files")
        If IsNothing(SamFiles) = True Then Exit Sub
        Dim res = Szunyi.Linux.SamTools.Bam_To_Sam(SamFiles)

        Clipboard.SetText(res)
        Beep()
    End Sub

    ' BamToSam
    Private Sub FromSubFOldersToolStripMenuItem2_Click(sender As Object, e As EventArgs) Handles FromSubFOldersToolStripMenuItem2.Click
        Dim SamFiles = Szunyi.IO.Directory.GetAllFilesFromFolder_Recursive(".bam")
        If IsNothing(SamFiles) = True Then Exit Sub
        Dim res = Szunyi.Linux.SamTools.Bam_To_Sam(SamFiles)
        Beep()

    End Sub
    ' Sort Index
    Private Sub FromFilesToolStripMenuItem4_Click(sender As Object, e As EventArgs) Handles FromFilesToolStripMenuItem4.Click
        Dim SamFiles = Szunyi.IO.Files.Filter.SelectFiles(Szunyi.Constants.Files.BAM, "Bam Files")
        If IsNothing(SamFiles) = True Then Exit Sub
        Dim res = Szunyi.Linux.SamTools.Sort_Index(SamFiles)

        Clipboard.SetText(res)
        Beep()
    End Sub

    Private Sub FromFolderToolStripMenuItem1_Click(sender As Object, e As EventArgs) Handles FromFolderToolStripMenuItem1.Click
        Dim SamFiles = Szunyi.IO.Directory.GetAllFilesFromFolder_Recursive(".bam")
        If IsNothing(SamFiles) = True Then Exit Sub
        Dim res = Szunyi.Linux.SamTools.Sort_Index(SamFiles)

        Clipboard.SetText(res)
        Beep()
    End Sub
    Private Sub SplitByReadGroupToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles SplitByReadGroupToolStripMenuItem.Click
        Dim Files = Szunyi.IO.Files.Filter.SelectFiles(Szunyi.Constants.Files.SAM, "Sam Files")
        If IsNothing(Files) = True Then Exit Sub
        Dim str As New System.Text.StringBuilder
        str.Append(Szunyi.Linux.SamTools.Split_By_ReadGroups(Files))
        If str.Length <> 0 Then Clipboard.SetText(str.ToString)

        Beep()
    End Sub
    'Sam To Bam Sort Index Files
    Private Sub FromFilesToolStripMenuItem2_Click(sender As Object, e As EventArgs) Handles FromFilesToolStripMenuItem2.Click
        Dim Files = Szunyi.IO.Files.Filter.SelectFiles(Szunyi.Constants.Files.SAM, "Sam Files")
        If IsNothing(Files) = True Then Exit Sub
        Dim str As New System.Text.StringBuilder
        str.Append(Szunyi.Linux.SamTools.Sam_to_Bam_Sort_Index(Files))
        If str.Length <> 0 Then Clipboard.SetText(str.ToString)

        Beep()
    End Sub
    'Sam To Bam Sort Index Folders
    Private Sub FromSubFoldersToolStripMenuItem1_Click(sender As Object, e As EventArgs) Handles FromSubFoldersToolStripMenuItem1.Click
        Dim SamFiles = Szunyi.IO.Directory.GetAllFilesFromFolder_Recursive(".sam")
        If IsNothing(SamFiles) = True Then Exit Sub
        Dim str As New System.Text.StringBuilder

        str.Append(Szunyi.Linux.SamTools.Sam_to_Bam_Sort_Index(SamFiles.ToList))

        Clipboard.SetText(str.ToString)
    End Sub
    ' Get UnMapeed Reads
    Private Sub GetUnMappedReadsToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles GetUnMappedReadsToolStripMenuItem.Click
        Dim SamFiles = Szunyi.IO.Files.Filter.SelectFiles(Szunyi.Constants.Files.SAM_BAM, "SAM-BAM Files")
        If IsNothing(SamFiles) = True Then Exit Sub
        Dim res As String = Szunyi.Linux.SamTools.Get_UnMapped_Reads(SamFiles)
        If res <> String.Empty Then Clipboard.SetText(res)
        Beep()


    End Sub


#End Region
#End Region


    Private Sub MinIonManagerToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles MinIonManagerToolStripMenuItem.Click
        Dim f1 As New MinIonManager
        f1.Show()

    End Sub

    Private Sub GetFromBamsAndFastaqToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles GetFromBamsAndFastaqToolStripMenuItem.Click
        Dim Folder = Szunyi.IO.Directory.Get_Folder
        If IsNothing(Folder) = True Then Exit Sub
        Dim All_Files = Szunyi.IO.Directory.GetAllFilesFromFolder_Recursive(Folder.FullName)
        Dim Fasta_files = (From x In All_Files Where x.Extension = ".fastq").ToList
        Dim Seqs = Szunyi.IO.Import.Sequence.FromFiles(Fasta_files)
        Dim c As New Szunyi.Comparares.OneByOne.SequenceIDComparer
        Seqs.Sort(c)
        Dim Bam_files = (From x In All_Files Where x.Name.Contains("sorted") And x.Extension = ".bam")
        For Each Bam_file In Bam_files
            Dim FastQSeqs As New List(Of ISequence)
            For Each Sa In Szunyi.BAM.Bam_Basic_IO.Import.Parse(Bam_file)
                Dim i1 = Seqs.BinarySearch(Sa.QuerySequence, c)
                If i1 > -1 Then
                    FastQSeqs.Add(Seqs(i1))
                Else
                    Dim kj As Int16 = 54
                End If
            Next
            Szunyi.IO.Export.SaveAsQualitativeSeq(FastQSeqs, New FileInfo(Bam_file.FullName & ".fastq"))
        Next
    End Sub


    Private Sub CountFromFilesToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles CountFromFilesToolStripMenuItem.Click
        Dim Files = Szunyi.IO.Files.Filter.SelectFiles(Szunyi.Constants.Files.gff3)
        If IsNothing(Files) = True Then Exit Sub
        Dim str As New System.Text.StringBuilder
        For Each File In Files
            Dim Count As Long = 0
            For Each Seq In Szunyi.IO.Import.Sequence.GetOnyByONe(File)
                Count += 1
            Next
            str.Append(File.Name).Append(vbTab).Append(Count).AppendLine()
        Next
        str.Length -= 2
        Clipboard.SetText(str.ToString)
    End Sub
    Private Sub SeparaeFromTwoFilesToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles SeparaeFromTwolFilesToolStripMenuItem.Click
        Dim FIles = Szunyi.IO.Files.Filter.SelectFiles(Constants.Files.SAM_BAM)
        Dim SeqIDs As New Dictionary(Of String, FileInfo)
        Dim SAMs As New Dictionary(Of FileInfo, List(Of Bio.IO.SAM.SAMAlignedSequence))
        Dim All_SAMs As New List(Of Bio.IO.SAM.SAMAlignedSequence)
        Dim Writters As New Dictionary(Of FileInfo, Szunyi.BAM.Bam_Basic_IO.Export)

        For Each FIle In FIles
            Dim RefSeqs = Szunyi.BAM.Bam_Basic_IO.Headers.Get_Reference_SeqIDS(FIle)
            For Each RefSeq In RefSeqs
                SeqIDs.Add(RefSeq, FIle)
            Next

            SAMs.Add(FIle, Szunyi.BAM.Bam_Basic_IO.Import.ParseAll(FIle))
            Dim nFile = Szunyi.IO.Files.Get_New_FileName.Append_Before_Extension(FIle, "_really")
            Writters.Add(FIle, New Szunyi.BAM.Bam_Basic_IO.Export(nFile, Szunyi.BAM.Bam_Basic_IO.Headers.Get_Header(FIle)))
            All_SAMs.AddRange(SAMs(FIle))
        Next

        For Each ReadID In Szunyi.BAM.GroupBy.ReadID(All_SAMs)
            Dim Best = Szunyi.BAM.Filter_Split.Get_Best(ReadID)


            Writters(SeqIDs(Best.RName)).Write(Best)

        Next

        For Each w In Writters
            w.Value.Dispose()
        Next
    End Sub

    Private Sub TwoFilesIntomSameDistinctsToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles TwoFilesIntomSameDistinctsToolStripMenuItem.Click
        Dim FIles = Szunyi.IO.Files.Filter.SelectFiles(Constants.Files.SAM_BAM)
        If FIles.Count = 2 Then
            Dim F_IDs = Szunyi.BAM.Bam_Basic_IO.Import.Get_Sorted_Distinct_Read_IDs(FIles.First)
            Dim L_IDs = Szunyi.BAM.Bam_Basic_IO.Import.Get_Sorted_Distinct_Read_IDs(FIles.Last)
            Dim Only_F = F_IDs.Except(L_IDs).ToList
            Dim Only_L = L_IDs.Except(F_IDs).ToList
            Dim Common = F_IDs.Intersect(L_IDs).ToList
            Dim F_Only_F = Szunyi.IO.Files.Get_New_FileName.Append_Before_Extension(FIles.First, "_only")
            Dim F_Only_L = Szunyi.IO.Files.Get_New_FileName.Append_Before_Extension(FIles.Last, "_only")
            Dim F_Common = Szunyi.IO.Files.Get_New_FileName.Append_Before_Extension(FIles.First, FIles.Last)
            Using F As New Szunyi.BAM.Bam_Basic_IO.Export(F_Only_F)
                Using L As New Szunyi.BAM.Bam_Basic_IO.Export(F_Only_L)
                    Using C As New Szunyi.BAM.Bam_Basic_IO.Export(F_Common)
                        For Each SAM In Szunyi.BAM.Bam_Basic_IO.Import.Parse(FIles.First)
                            Dim Index = Only_F.BinarySearch(SAM.QName)
                            If Index > -1 Then F.Write(SAM)
                            Index = Common.BinarySearch(SAM.QName)
                            If Index > -1 Then C.Write(SAM)
                        Next
                        For Each SAM In Szunyi.BAM.Bam_Basic_IO.Import.Parse(FIles.Last)
                            Dim Index = Only_L.BinarySearch(SAM.QName)
                            If Index > -1 Then
                                L.Write(SAM)
                            End If

                        Next
                    End Using
                End Using
            End Using

        End If
    End Sub

    Private Sub ByExonLengthToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles ByExonLengthToolStripMenuItem.Click
        Dim FIles = Szunyi.IO.Files.Filter.SelectFiles(Constants.Files.SAM_BAM)
        Dim Max_Exon_Length = MyInputBox.GetInteger("Enter Max Exon Length")
        Dim Min_Intron_length = MyInputBox.GetInteger("Enter minimum Intron Length")
        For Each FIle In FIles
            Dim less = Szunyi.IO.Files.Get_New_FileName.Append_Before_Extension(FIle, "_Max_Exon_Less" & Max_Exon_Length)
            Dim more = Szunyi.IO.Files.Get_New_FileName.Append_Before_Extension(FIle, "_Max_Exon_More" & Max_Exon_Length)
            Dim Log = Szunyi.IO.Files.Get_New_FileName.Replace_Extension(FIle, Szunyi.Constants.File_Extensions.Log)
            Dim Header = Szunyi.BAM.Bam_Basic_IO.Headers.Get_Header(FIle)
            Using l As New Szunyi.BAM.Bam_Basic_IO.Export(less, Header)
                Using m As New Szunyi.BAM.Bam_Basic_IO.Export(more, Header)
                    Dim nof_less As Integer = 0
                    Dim nof_more As Integer = 0
                    For Each Sam In Szunyi.BAM.Bam_Basic_IO.Import.Parse(FIle)

                        Dim Loci = Szunyi.Location.Common.GetLocation(Sam)
                        Dim Exon = Szunyi.Location.Common.Get_Smallest_Exon_Location(Loci)
                        If Max_Exon_Length > Exon.LocationEnd - Exon.LocationStart Then
                            l.Write(Sam)
                            nof_less += 1
                        Else
                            nof_more += 1
                            m.Write(Sam)
                        End If
                    Next
                    Dim strlog As New System.Text.StringBuilder
                    strlog.Append("#").Append(FIle.FullName).AppendLine()
                    strlog.Append("#Max Exon Length:").Append(Max_Exon_Length).AppendLine()
                    strlog.Append("#Min Intron Length:").Append(Min_Intron_length).AppendLine()
                    strlog.Append("nof less:").Append(nof_less).AppendLine()
                    strlog.Append("nof more:").Append(nof_more).AppendLine()
                    Szunyi.IO.Export.SaveText(strlog.ToString, Log)
                End Using
            End Using
        Next

    End Sub
    Private Sub ByIntronToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles ByIntronToolStripMenuItem.Click
        Dim FIles = Szunyi.IO.Files.Filter.SelectFiles(Constants.Files.SAM_BAM)
        Dim Introns = MyInputBox.GetIntegers("Enter values separated by space")
        Introns.Sort()
        For Each FIle In FIles
            Dim out_w_Intron As New List(Of Bio.IO.SAM.SAMAlignedSequence)
            Dim out_wo_Intron As New List(Of Bio.IO.SAM.SAMAlignedSequence)

            Dim RefSeqs = Szunyi.BAM.Bam_Basic_IO.Headers.Get_ReferenceSequences(FIle)
            Dim Res As New Dictionary(Of String, Szunyi.BAM.Bam_Basic_IO.Export)
            Dim Outs As New Dictionary(Of String, List(Of Bio.IO.SAM.SAMAlignedSequence))
            For i1 = 0 To Introns.Count - 2
                Dim Item = "_I-" & Introns(i1) & "-" & Introns(i1 + 1)
                Dim Header = Bam_Basic_IO.Headers.Get_Header(FIle)
                If IsNothing(Header.Comments) = True Then
                    Header.Comments = New List(Of String)
                End If
                Header.Comments.Add(Item)

                Res.Add(Item, New Szunyi.BAM.Bam_Basic_IO.Export(Szunyi.IO.Files.Get_New_FileName.Append_Before_Extension(FIle, Item), Header))
                Outs.Add(Item, New List(Of Bio.IO.SAM.SAMAlignedSequence))
            Next
            For Each Sam In Szunyi.BAM.Bam_Basic_IO.Import.Parse(FIle)
                Dim Passed As New List(Of Boolean)

                For i1 = 0 To Introns.Count - 2
                    If Szunyi.BAM.CIGAR.Has_Longer_Insertion(Sam, Introns(i1 + 1)) = True Then
                        Passed.Add(False)
                    Else
                        If Szunyi.BAM.CIGAR.Has_Insertion_Between(Sam, Introns(i1), Introns(i1 + 1)) = True Then
                            Passed.Add(True)
                        Else
                            Passed.Add(False)
                        End If
                    End If

                Next
                For i1 = 0 To Passed.Count - 1
                    If Passed(i1) = True Then
                        Res("_I-" & Introns(i1) & "-" & Introns(i1 + 1)).Write(Sam)
                        Exit For
                    End If
                Next
            Next
            For Each Item In Res
                Item.Value.Dispose()
            Next
        Next

        Beep()


    End Sub
    Private Sub GetNonIntronicToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles GetNonIntronicToolStripMenuItem.Click
        Dim FIles = Szunyi.IO.Files.Filter.SelectFiles(Constants.Files.BAM)
        Dim MaxIntronLength = Szunyi.MyInputBox.GetInteger("Max Intron Length")
        Dim MinMatch = Szunyi.MyInputBox.GetInteger("Minimal Match")
        For Each FIle In FIles
            Dim out_w_Intron As New List(Of Bio.IO.SAM.SAMAlignedSequence)
            Dim out_wo_Intron As New List(Of Bio.IO.SAM.SAMAlignedSequence)
            Dim Header = Bam_Basic_IO.Headers.Get_Header(FIle)
            Dim RefSeqs = Szunyi.BAM.Bam_Basic_IO.Headers.Get_ReferenceSequences(FIle)

            For Each Sam In Szunyi.BAM.Bam_Basic_IO.Import.Parse(FIle)
                If Szunyi.BAM.CIGAR.Has_Longer_Insertion(Sam, MaxIntronLength) = False Then
                    '  Dim M = Szunyi.Alignment.BAM_SAM.Sam_Manipulation.Get_Matches(Sam)
                    '   If M >= MinMatch Then out_wo_Intron.Add(Sam)
                Else
                    '  out_wo_Intron.Add(Sam)
                End If
            Next
            Dim File_wo_Intron As New FileInfo(FIle.Directory.FullName & "\" & FIle.Name & "wo_Intron" & MaxIntronLength & "_minMatch" & MinMatch & ".sam")
            Using x As New Szunyi.BAM.Bam_Basic_IO.Export(File_wo_Intron, Header)
                x.Writes(out_wo_Intron)
            End Using

        Next
    End Sub

    Private Sub BamToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles BamToolStripMenuItem.Click

    End Sub

    Private Sub SplitByRefereneFromSubFoldersToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles SplitByRefereneFromSubFoldersToolStripMenuItem.Click
        Dim SamFiles = Szunyi.IO.Directory.GetAllFilesFromFolder_Recursive(".bam")
        If IsNothing(SamFiles) = True Then Exit Sub
        Dim str As New System.Text.StringBuilder
        For Each File In SamFiles
            Dim LinuxFIleName = Szunyi.IO.Linux.Get_FileName(File)
            '   Dim BamFIleName = Szunyi.IO.Linux.Get_FileName(New FileInfo(File.FullName.Replace(".bam", ".sam")))

            str.Append("bamtools split -in ")
            str.Append(LinuxFIleName).Append(" -reference ")
            str.AppendLine()

            'view -bS file.sam | samtools sort - file_sorted
        Next
        Clipboard.SetText(str.ToString)
    End Sub

    Private Sub ByReadIDsToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles ByReadIDsToolStripMenuItem.Click
        Dim File = Szunyi.IO.Files.Filter.SelectFile()
        If IsNothing(File) = True Then Exit Sub
        Dim ReadIDs As List(Of String) = Szunyi.IO.Import.Text.ReadLines(File, 0)
        ReadIDs.Sort()
        Dim files = Szunyi.IO.Files.Filter.SelectFiles(Constants.Files.BAM)
        If IsNothing(files) = False Then
            Dim out As New List(Of Bio.IO.SAM.SAMAlignedSequence)
            Dim Header = Bam_Basic_IO.Headers.Get_Header(files)
            For Each SAM In Szunyi.BAM.Bam_Basic_IO.Import.Parse(files)
                Dim Index = ReadIDs.BinarySearch(SAM.QName)
                If Index > -1 Then
                    out.Add(SAM)
                End If


            Next
            Szunyi.IO.Export.Export_Sam(out, Header.First)
        End If
    End Sub

    Private Sub UniqueMultiplyByCountToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles UniqueMultiplyByCountToolStripMenuItem.Click
        Dim files = Szunyi.IO.Files.Filter.SelectFiles(Constants.Files.BAM)
        If IsNothing(files) = True Then Exit Sub
        Dim Header = Bam_Basic_IO.Headers.Get_Header(files.First)
        Dim Sams = Szunyi.BAM.Bam_Basic_IO.Import.Parse(files).ToList

        Dim x As New Dictionary(Of Integer, List(Of Bio.IO.SAM.SAMAlignedSequence))

        Dim res = From x1 In Sams Group By x1.QName Into Group

        Dim Count As Integer = 0
        For Each item In res
            If x.ContainsKey(item.Group.Count) = False Then x.Add(item.Group.Count, New List(Of Bio.IO.SAM.SAMAlignedSequence))
            x(item.Group.Count).AddRange(item.Group.ToList)
            Count += 1
        Next
        For Each item In x
            Dim newFIle As New FileInfo(files.First.FullName & "_" & item.Key & ".sam")
            Szunyi.IO.Export.Export_Sam(item.Value, Header, newFIle)
        Next

    End Sub

    Private Sub ByOrganismToolStripMenuItem1_Click(sender As Object, e As EventArgs) Handles ByOrganismToolStripMenuItem1.Click
        Dim Files = Szunyi.IO.Files.Filter.SelectFiles("Select Files", Szunyi.Constants.Files.Other.BAM)
        If IsNothing(Files) = True Then Exit Sub
        Dim All As New Dictionary(Of String, List(Of Bio.ISequence))
        For Each File In Files

            Using s As New FileStream(File.FullName, FileMode.Open)
                Dim bamReader As New Bio.IO.BAM.BAMParser()

                For Each SA In bamReader.Parse(s)
                    If All.ContainsKey(SA.RName) = False Then All.Add(SA.RName, New List(Of Bio.ISequence))
                    All(SA.RName).Add(Szunyi.Sequences.SequenceManipulation.Common.GetSeqAsBioSeq(SA.QuerySequence))
                Next
            End Using

        Next
        Dim folder = Szunyi.IO.Directory.Get_Folder
        If IsNothing(folder) = True Then Exit Sub
        For Each Item In All
            Dim FIleName = Item.Key & ".fa"
            Dim illegalInFileName As New Regex("[\\/:*?""<>|]")
            Dim myString As String = illegalInFileName.Replace(FIleName, "_")

            Dim nFIle As New FileInfo(folder.FullName & "\" & myString)
            Szunyi.IO.Export.SaveSequencesToSingleFasta(Item.Value, nFIle)
        Next
    End Sub

    Private Sub CommonReadsToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles CommonReadsToolStripMenuItem.Click
        Dim Files = Szunyi.IO.Files.Filter.SelectFiles(Szunyi.Constants.Files.BAM)

        Dim ReadIDs As New List(Of List(Of String))
        Dim str As New System.Text.StringBuilder
        For Each File In Files
            str.Append(File.Name).Append(vbTab)
            Dim tmp As New List(Of String)
            For Each Sam In Szunyi.BAM.Bam_Basic_IO.Import.Parse(File)
                tmp.Add(Sam.QName)
            Next
            ReadIDs.Add(tmp.Distinct.ToList)
        Next

        Dim x = Szunyi.Math.Combinatory.GetBinaryPermutation(Files.Count)
        str.Append("Count").Append(vbTab).Append("All Reads").AppendLine()
        For Each Item In x
            Dim Res As New List(Of String)
            For i1 = 0 To Files.Count - 1
                If Item(i1) = True Then
                    Res = ReadIDs(i1)
                    Exit For
                End If
            Next
            For i1 = 0 To Files.Count - 1
                str.Append(Item(i1)).Append(vbTab)
                If Item(i1) = True Then

                    Res = Res.Intersect(ReadIDs(i1)).ToList

                Else
                    Res = Res.Except(ReadIDs(i1)).ToList
                End If
            Next
            str.Append(Res.Count).AppendLine()
        Next
        Clipboard.SetText(str.ToString)
    End Sub

    Private Sub AllToolStripMenuItem7_Click(sender As Object, e As EventArgs) Handles AllToolStripMenuItem7.Click
        Dim Seqs = Me.GetSeqsFromSelectedList(True)
        If IsNothing(Seqs) = True Then Exit Sub
        Dim str As New System.Text.StringBuilder
        Dim Lengths As New System.Text.StringBuilder
        Dim Values As New List(Of List(Of String))
        Dim Header As String = "Loci Accession" & vbTab & "Location" & vbTab & "CDS" & vbTab & "Mod 3" & vbTab & "Nof Stop Codon" & vbTab & "StartwATG" & vbTab & "EndwStopCodon" & vbTab & "Length" & vbCrLf
        For Each Seq In Seqs
            str.Append(Seq.ID).AppendLine()
            Lengths.Append(Seq.ID).AppendLine()
            Dim CDSs = Szunyi.Features.FeatureManipulation.GetFeaturesByType.GetFeturesByTypeFromSeq(Seq, StandardFeatureKeys.CodingSequence)
            Values.Add(Szunyi.Location.Common.Get_Location_Strings(CDSs))
            Values.Add(Szunyi.Features.Check.CDS.Mod3(Seq, True))
            Values.Add(Szunyi.Features.Check.CDS.Inner_Stop(Seq))
            Values.Add(Szunyi.Features.Check.CDS.StartwATG(Seq))
            Values.Add(Szunyi.Features.Check.CDS.EndwStop(Seq))
            Values.Add(Szunyi.Features.Check.CDS.Lengths(Seq))
        Next
        Dim res As New List(Of String)
        For i2 = 0 To Values.First.Count - 1
            Dim line As String = ""

            For i1 = 0 To Values.Count - 1

                line = line & Values(i1)(i2) & vbTab
            Next
            If line.Length > 0 Then line = line.Substring(0, line.Length - 1)
            res.Add(line)
        Next

        Clipboard.SetText(Header & Szunyi.Text.General.GetText(res))
    End Sub



    Private Sub MinionToSortedReadIDsToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles MinionToSortedReadIDsToolStripMenuItem.Click
        Dim ReadIDs As New List(Of String)
        Dim files = Szunyi.IO.Files.Filter.SelectFiles(Constants.Files.Fasta_FastQ)
        If IsNothing(files) = True Then Exit Sub
        For Each file In files
            For Each line In Szunyi.IO.Import.Text.ParseMoreLines(file, 4)
                Dim s = line.First.Split(" ").First.Substring(1)
                ReadIDs.Add(s)
            Next
        Next
        ReadIDs.Sort()
        Szunyi.IO.Export.SaveText(Szunyi.Text.General.GetText(ReadIDs))
    End Sub
    Private Sub CommonDistinctToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles CommonDistinctToolStripMenuItem.Click
        Dim files = Szunyi.IO.Files.Filter.SelectFiles
        Dim tmp As New List(Of List(Of String))
        For Each file In files
            Dim readids = Szunyi.IO.Import.Text.ReadLines(file)
            tmp.Add(readids)
        Next
        Dim c = tmp.First.Intersect(tmp.Last)
        Dim kj As Int16 = 54
    End Sub


    Private Sub GeneiousToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles GeneiousToolStripMenuItem.Click
        Dim Files = Szunyi.IO.Files.Filter.SelectFiles
        If IsNothing(Files) = True Then Exit Sub
        Dim Seqs = Me.GetSeqsFromSelectedList(True)
        If IsNothing(Seqs) = True Then Exit Sub

        Dim str As New System.Text.StringBuilder
        For Each file In Files
            Dim All_Variants = Geneious_Variants.Get_Variants(file)
            Dim cSeq = (From x In Seqs Where x.ID = All_Variants.First.Sequence_Name).First
            Dim CDSs = Szunyi.Features.FeatureManipulation.GetFeaturesByType.GetFeturesByTypeFromSeq(cSeq, StandardFeatureKeys.CodingSequence)

            Dim Unique_Varaiants_Interval = Geneious_Variants.Get_Unique_By_Interval(All_Variants)
            str.Append(file.Name).AppendLine()

            str.Append("Nof Event Whole").Append(vbTab).Append(Unique_Varaiants_Interval.Count).AppendLine()
            str.Append("Nof Affected NA Whole").Append(vbTab).Append(Geneious_Variants.Get_Nof_Affected_NA(Unique_Varaiants_Interval)).AppendLine()
            str.Append("Nof Affected AA Whole").Append(vbTab).Append(Geneious_Variants.Get_Nof_Affected_AA(All_Variants)).AppendLine()

            'InterGenic

            Dim InIntergenic = Geneious_Variants.Get_InterGenic(Unique_Varaiants_Interval)
            str.Append("Nof Event InterGenic").Append(vbTab).Append(InIntergenic.Count).AppendLine()
            str.Append("Nof Affected NA InterGenic").Append(vbTab).Append(Geneious_Variants.Get_Nof_Affected_NA(InIntergenic)).AppendLine()

            'CDS
            Dim inCDS_All = Geneious_Variants.Get_InCDS(All_Variants)
            Dim inCDS_Unique_InterVal = Geneious_Variants.Get_Unique_By_Interval(inCDS_All)
            str.Append("Nof Event CDS").Append(vbTab).Append(inCDS_Unique_InterVal.Count).AppendLine()
            str.Append("Nof Affected NA CDS").Append(vbTab).Append(Geneious_Variants.Get_Nof_Affected_NA(inCDS_Unique_InterVal)).AppendLine()
            str.Append("Nof Affected AA CDS").Append(vbTab).Append(Geneious_Variants.Get_Nof_Affected_AA(inCDS_All)).AppendLine()

            'Check
            Dim sg1 As New List(Of Geneious_Variant)
            For Each PolyMorf_Type In Geneious_Variants.By_Polymorphis_Type(All_Variants)
                Dim InCoding = Geneious_Variants.Get_InCDS(PolyMorf_Type)
                sg1.AddRange(Geneious_Variants.Get_Unique_By_Interval(InCoding))
            Next
            Dim diff = sg1.Except(inCDS_Unique_InterVal)

            Dim ByType = Split("Type,Nof Event InterGenic,Nof Affected NA InterGenic,Nof Event InCDS,Nof Affected NA InCDS,Nof Affected AA InCDS", ",")
            For Each PolyMorf_Type In Geneious_Variants.By_Polymorphis_Type(All_Variants)
                Dim pType = PolyMorf_Type.First.Polymorphism_Type

                ByType(0) = ByType(0) & vbTab & PolyMorf_Type.First.Polymorphism_Type
                Dim InterGenic = Geneious_Variants.Get_InterGenic(PolyMorf_Type)
                InterGenic = Geneious_Variants.Get_Unique_By_Interval(InterGenic)
                ByType(1) = ByType(1) & vbTab & InterGenic.Count ' Nof_Affected_Intergenic_NA
                ByType(2) = ByType(2) & vbTab & Geneious_Variants.Get_Nof_Affected_NA(InterGenic)

                Dim InCoding = Geneious_Variants.Get_InCDS(PolyMorf_Type)
                ByType(3) = ByType(3) & vbTab & Geneious_Variants.Get_Unique_By_Interval(InCoding).Count ' Event

                Dim Unique_InCoding_byLocation = Geneious_Variants.Get_Unique_By_Interval(InCoding)
                ByType(4) = ByType(4) & vbTab & Geneious_Variants.Get_Nof_Affected_NA(Unique_InCoding_byLocation) ' NA Count
                ByType(5) = ByType(5) & vbTab & Geneious_Variants.Get_Nof_Affected_AA(InCoding)


            Next
            Dim k = Szunyi.Text.General.GetText(ByType)
            str.Append(k)
            str.AppendLine.AppendLine()

            Dim ByTypeII = Split("Type,Nof Event InCDS,Nof Affected NA InCDS,Nof Affected AA InCDS", ",")
            For Each CDS In Geneious_Variants.ByCDS(All_Variants, CDSs)
                ByTypeII(0) = ByTypeII(0) & vbTab & CDS.Value.First.LTag & " " & CDS.Value.First.CDS & "(" & Szunyi.Location.Common.GetLocationString(CDS.Key.Location) & ")"

                ByTypeII(1) = ByTypeII(1) & vbTab & Geneious_Variants.Get_Unique_By_Interval(CDS.Value).Count ' Events
                ByTypeII(2) = ByTypeII(2) & vbTab & Geneious_Variants.Get_Nof_Affected_NA(CDS.Value)
                ByTypeII(3) = ByTypeII(3) & vbTab & Geneious_Variants.Get_Nof_Affected_AA(CDS.Value)

            Next
            Dim ByTypeIII = Split(",Conservative,Radical,Silent,Conservative By Side,Radical By Side,Insertion,Deletion", ",")

            Dim Matrixes As New List(Of SimilarityMatrices.SimilarityMatrix)

            Matrixes.Add(New SimilarityMatrices.SimilarityMatrix(7))
            Matrixes.Add(New SimilarityMatrices.SimilarityMatrix(8))

            Matrixes.Add(New SimilarityMatrices.SimilarityMatrix(9))
            Matrixes.Add(New SimilarityMatrices.SimilarityMatrix(10))
            Matrixes.Add(New SimilarityMatrices.SimilarityMatrix(11))

            For Each CDS In Geneious_Variants.ByCDS(All_Variants, CDSs)
                Dim sg = Geneious_Variants.Get_SNP_Type(CDS.Value, Matrixes)

                ByTypeIII(0) = ByTypeIII(0) & vbTab & CDS.Value.First.LTag & " " & CDS.Value.First.CDS & "(" & Szunyi.Location.Common.GetLocationString(CDS.Key.Location) & ")"
                For i1 = 0 To sg.Count - 1
                    ByTypeIII(i1 + 1) = ByTypeIII(i1 + 1) & vbTab & sg(i1)
                Next
            Next
            str.Append(Szunyi.Text.General.GetText(ByTypeII)).AppendLine()
            str.Append(Szunyi.Text.General.GetText(ByTypeIII)).AppendLine().AppendLine()
        Next
        Clipboard.SetText(str.ToString)
    End Sub

#Region "Features"
    Private Sub MoveFeatureQulifiersToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles MoveFeatureQulifiersToolStripMenuItem.Click
        Dim seqs = Me.GetSeqsFromSelectedList(True)
        If IsNothing(seqs) = True Then Exit Sub
        Dim cSeqs = Szunyi.Sequences.SequenceManipulation.Common.CloneSeqs(seqs)
        Dim QulaToDelete As New List(Of String)
        Dim QualNames = Szunyi.Features.FeatureManipulation.Qulifiers.Get_Distinct_Qulifiers(cSeqs)
        Dim f1 As New CheckBoxForStringsFull(QualNames, 1, "Qulifier To Move")
        If f1.ShowDialog = DialogResult.OK Then
            Dim f2 As New CheckBoxForStringsFull(QualNames, 1, "Qulifier is Replaced")
            If f2.ShowDialog = DialogResult.OK Then
                For Each Seq In cSeqs
                    For Each Feat In Szunyi.Features.FeatureManipulation.GetFeaturesByType.GetFeturesByTypesFromSeq(Seq, StandardFeatureKeys.All.ToList)
                        For Each item In f1.SelectedStrings
                            If Feat.Qualifiers.ContainsKey(item) = True Then
                                Feat.Qualifiers.Remove(item)
                            End If
                        Next
                    Next
                Next
            End If
        End If

        Szunyi.IO.Export.SaveSequencesToSingleGenBank(cSeqs)
    End Sub

    Private Sub InGeneByLocustagToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles InGeneByLocustagToolStripMenuItem.Click
        Dim seqs = Me.GetSeqsFromSelectedList(True)
        If IsNothing(seqs) = True Then Exit Sub
        Dim cSeqs = Szunyi.Sequences.SequenceManipulation.Common.CloneSeqs(seqs)
        Dim str As New System.Text.StringBuilder
        For Each Seq In cSeqs
            str.Append(Seq.ID).AppendLine()
            str.Append("gene CDS")
            Dim Genes = Szunyi.Features.FeatureManipulation.GetFeaturesByType.GetFeturesByTypeFromSeq(Seq, StandardFeatureKeys.Gene)
            Dim CDSs = Szunyi.Features.FeatureManipulation.GetFeaturesByType.GetFeturesByTypeFromSeq(Seq, StandardFeatureKeys.CodingSequence)
            For Each g In Genes
                Dim sel = From x In CDSs Where Szunyi.Location.Common.GetLocationString(x.Location) = Szunyi.Location.Common.GetLocationString(g.Location)

                If sel.Count = 0 Then
                    str.AppendLine()
                    str.Append(Szunyi.Location.Common.GetLocationString(g.Location))
                Else
                    For Each Item In sel
                        str.AppendLine()
                        str.Append(Szunyi.Location.Common.GetLocationString(g.Location)).Append(vbTab)
                        str.Append(Szunyi.Location.Common.GetLocationString(Item.Location))
                    Next
                End If
            Next

        Next
        str.AppendLine("CDS gene")
        For Each Seq In cSeqs
            str.Append(Seq.ID)
            Dim Genes = Szunyi.Features.FeatureManipulation.GetFeaturesByType.GetFeturesByTypeFromSeq(Seq, StandardFeatureKeys.Gene)
            Dim CDSs = Szunyi.Features.FeatureManipulation.GetFeaturesByType.GetFeturesByTypeFromSeq(Seq, StandardFeatureKeys.CodingSequence)
            For Each g In CDSs
                Dim sel = From x In Genes Where Szunyi.Location.Common.GetLocationString(x.Location) = Szunyi.Location.Common.GetLocationString(g.Location)

                If sel.Count = 0 Then
                    str.AppendLine()
                    str.Append(Szunyi.Location.Common.GetLocationString(g.Location))
                Else
                    For Each Item In sel
                        str.AppendLine()
                        str.Append(Szunyi.Location.Common.GetLocationString(g.Location)).Append(vbTab)
                        str.Append(Szunyi.Location.Common.GetLocationString(Item.Location))
                    Next
                End If
            Next

        Next
        Clipboard.SetText(str.ToString)
    End Sub




    Private Sub ByPositionToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles ByPositionToolStripMenuItem.Click
        Dim Files = Szunyi.IO.Files.Filter.SelectFiles(Szunyi.Constants.Files.Fasta, "Select alignment fileS IN FASTA FORMAT")
        If IsNothing(Files) = True Then Exit Sub
        Dim AL_Seqs = Szunyi.IO.Import.Sequence.FromFiles(Files)
        Dim Seqs = Me.GetSeqsFromSelectedList(True)
        If IsNothing(Seqs) = True Then
            Seqs = Szunyi.IO.Import.Sequence.FromUserSelection
        End If


        Seqs = (From x In Seqs Order By x.ID).ToList
        Dim Positions = Szunyi.DNA.Variants.Analysis.GetPos(AL_Seqs.First, AL_Seqs.Last)
        Dim f1 As New CheckBoxForStringsFull(Seqs, 1)
        Dim OriSeq, NewSeq As Bio.ISequence

        If f1.ShowDialog = DialogResult.OK Then
            OriSeq = f1.SelSeqs.First
            Dim f2 As New CheckBoxForStringsFull(Seqs, 1)
            If f2.ShowDialog = DialogResult.OK Then
                NewSeq = f2.SelSeqs.Last
                Dim Features = Szunyi.Features.FeatureManipulation.GetFeaturesByType.Get_All_Features(OriSeq)
                Dim FeatTypes = (From x In Features Select x.Key).Distinct.ToList
                Dim f3 As New CheckBoxForStringsFull(FeatTypes, -1)
                Dim transferd As Integer = 0
                If f3.ShowDialog = DialogResult.OK Then

                    Dim cFeats = Szunyi.Features.FeatureManipulation.GetFeaturesByType.GetFeturesByTypesFromSeq(OriSeq, f3.SelectedStrings)

                    For Each cFeat In cFeats
                        Try
                            Dim pos1 = Positions(OriSeq.ID)(cFeat.Location.LocationStart)
                            Dim pos2 = Positions(OriSeq.ID)(cFeat.Location.LocationEnd)
                            Dim loci = Szunyi.Location.Common.GetLocation(pos1, pos2, cFeat.Location.IsComplementer)
                            Dim f As New FeatureItem(cFeat.Key, loci)
                            Szunyi.Features.FeatureManipulation.MergeFeatures.Merge2Features(cFeat, f, True)
                            Szunyi.Features.FeatureManipulation.GenBankMetaDataManipulation.AddFeature(NewSeq, f)
                            transferd += 1
                        Catch ex As Exception
                            Dim kj As Int16 = 43
                        End Try

                    Next
                    Szunyi.IO.Export.SaveSequencesToSingleGenBank(NewSeq)

                End If
            End If
        End If



    End Sub




#End Region

#Region "Orientation"
#Region "Get_Scores"
    Private Function Get_Score_Setting() As Szunyi.Transcipts.Score_Settings
        Dim res As New Szunyi.Transcipts.Score_Settings
        Dim files = Szunyi.IO.Files.Filter.SelectFiles(Constants.Files.SAM_BAM)
        If IsNothing(files) = True Then Return Nothing
        res.Files = files
        Dim s = InputBox("Enter the regions in format in match length-out match length, separated by comma eg:10-30,0-25")
        Try
            Dim s1 = Split(s, ",")
            For Each s2 In s1
                Dim s3 = Split(s2, "-")
                If s3.Length = 2 Then
                    Dim x As New Szunyi.Transcipts.Aligner_Setting(s3.First, s3.Last)
                    res.Regions.Add(x)
                End If

            Next
        Catch ex As Exception
            MsgBox("Erron in regions")
            Return Nothing
        End Try
        Dim APs As New Szunyi.Transcipts.Adaptor_Pairs
        Dim Names = (From x In APs.A_Ps Select x.Name).ToList
        Dim f1 As New CheckBoxForStringsFull(Names, -1)
        If f1.ShowDialog <> DialogResult.OK Then Return Nothing
        For Each Item In f1.SelectedStrings
            Dim c = From x In APs.A_Ps Where x.Name = Item

            res.Adapter_Pairs.Add(c.First)
        Next
        Return res
    End Function
    ''' <summary>
    ''' Get Score from Adaptor Aligning
    ''' </summary>
    ''' <param name="sender"></param>
    ''' <param name="e"></param>
    Private Sub CAPToolStripMenuItem1_Click(sender As Object, e As EventArgs)
        Dim S = Get_Score_Setting()
        If IsNothing(S) = True Then Exit Sub
        For Each F In S.Files
            For Each R In S.Regions
                Dim R_postFix As String = ("_Region" & R.In_Match_Length & "_" & R.Ouf_of_Match_Length & "_")
                Dim str_Short As New System.Text.StringBuilder
                Dim str_Big As New System.Text.StringBuilder

                Dim Header = Bam_Basic_IO.Headers.Get_Header(F)
                For Each Item In S.Adapter_Pairs
                    Header.Comments.Add(Item.ToString)
                Next
                Dim File As New FileInfo((F.DirectoryName & "\" & "wAdaptor" & R_postFix & F.Name & ".sam"))
                Using x As New Szunyi.BAM.Bam_Basic_IO.Export(File, Header)
                    For Each sam In Szunyi.BAM.Bam_Basic_IO.Import.Parse(F)
                        If str_Short.Length = 0 Then
                            str_Short.Append(MappingII.Common_Mapping_V1_IO.Get_Short_Header_Score_Table(S.Adapter_Pairs)).AppendLine()
                            str_Big.Append(MappingII.Common_Mapping_V1_IO.Get_Big_Header_Score_Table(S.Adapter_Pairs)).AppendLine()
                        End If
                        str_Short.Append(sam.QName)
                        str_Big.Append(sam.QName)
                        Dim U = Szunyi.BAM.SAM_Manipulation.QuerySequnce.Get_Five_Primer_Region(sam, R)
                        Dim D = Szunyi.BAM.SAM_Manipulation.QuerySequnce.Get_Three_Primer_Region(sam, R)
                        For Each A In S.Adapter_Pairs
                            Dim AAR = Szunyi.Transcipts.Orientation.Get_AAR(U, D, R, A.Five_Prime_Adapter, A.Three_Prime_Adapter)
                            Szunyi.BAM.BAM_Optional_Filed_Manipulation.Add_AAR(sam, A.PreFix, AAR)
                            str_Short.Append(vbTab).Append(Szunyi.Transcipts.Adaptor_Aligner_Result_Manipulation.Get_Scores(AAR))
                            str_Big.Append(vbTab).Append(Szunyi.Transcipts.Adaptor_Aligner_Result_Manipulation.Get_Five_Prime_Score(AAR))
                            str_Big.Append(vbTab).Append(Szunyi.Transcipts.Adaptor_Aligner_Result_Manipulation.Get_Five_Seqs(AAR))
                            str_Big.Append(vbTab).Append(Szunyi.Transcipts.Adaptor_Aligner_Result_Manipulation.Get_Five_Prime_Pos(AAR))
                            str_Big.Append(vbTab).Append(Szunyi.Transcipts.Adaptor_Aligner_Result_Manipulation.Get_Three_Prime_Score(AAR))
                            str_Big.Append(vbTab).Append(Szunyi.Transcipts.Adaptor_Aligner_Result_Manipulation.Get_Three_Seqs(AAR))
                            str_Big.Append(vbTab).Append(Szunyi.Transcipts.Adaptor_Aligner_Result_Manipulation.Get_Three_Prime_Pos(AAR))

                        Next
                        x.Write(sam)

                        str_Short.AppendLine()
                        str_Big.AppendLine()
                    Next ' Last SAM
                End Using
                Szunyi.IO.Export.SaveText(str_Short.ToString, New FileInfo(F.DirectoryName & "\" & "Adaptor_Scores" & R_postFix & F.Name & ".tsv"))
                Szunyi.IO.Export.SaveText(str_Big.ToString, New FileInfo(F.DirectoryName & "\" & "Adaptor_Scores_Detailed" & R_postFix & F.Name & ".tsv"))

            Next
        Next

    End Sub

#End Region



#End Region

#Region "SAM-BAM"


    Private Sub MaintainBettersMapsOnlyToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles MaintainBettersMapsOnlyToolStripMenuItem.Click
        Dim FIles = Szunyi.IO.Files.Filter.SelectFiles(Szunyi.Constants.Files.SAM_BAM)
        If IsNothing(FIles) = True Then Exit Sub
        For Each File In FIles
            Dim Header = Szunyi.BAM.Bam_Basic_IO.Headers.Get_Header(File)
            Dim Sams = Szunyi.BAM.Bam_Basic_IO.Import.Parse(File).ToList
            Dim out As New List(Of Bio.IO.SAM.SAMAlignedSequence)
            For Each ByReads In Szunyi.BAM.GroupBy.ReadID(Sams)
                If ByReads.Count = 1 Then
                    out.Add(ByReads.First)
                Else
                    out.AddRange(Szunyi.BAM.Filter_Split.Get_Bests_From_Groupby_ReadID(ByReads))
                End If
            Next
            Dim nFIle = Szunyi.IO.Files.Get_New_FileName.Append_Before_Extension(File, "e-" & out.Count & "-only_Bests")
            Szunyi.IO.Export.Export_Sam(out, Header, nFIle)
        Next



    End Sub

    Private Sub IntoBamToolStripMenuItem1_Click(sender As Object, e As EventArgs) Handles IntoBamToolStripMenuItem1.Click
        Dim Files = Szunyi.IO.Files.Filter.SelectFiles(Szunyi.Constants.Files.SAM_BAM)
        If IsNothing(Files) = True Then Exit Sub
        Dim All As New Dictionary(Of FileInfo, Dictionary(Of String, List(Of Bio.IO.SAM.SAMAlignedSequence)))
        Dim Species = Szunyi.BAM.Bam_Basic_IO.Headers.Get_Reference_SeqIDS(Files)

        For Each File In Files

            Dim res As New Dictionary(Of String, List(Of Bio.IO.SAM.SAMAlignedSequence)) 'Organism And COunt
            For Each sp In Species
                res.Add(sp, New List(Of Bio.IO.SAM.SAMAlignedSequence))
            Next

            For Each SA In Szunyi.BAM.Bam_Basic_IO.Import.Parse(File)

                If res.ContainsKey(SA.RName) = False Then res.Add(SA.RName, New List(Of Bio.IO.SAM.SAMAlignedSequence))
                res(SA.RName).Add(SA)

            Next
            For Each Item In res
                Dim Header = Bam_Basic_IO.Headers.Get_Header(File)
                Dim k As New Bio.IO.SAM.SAMAlignmentHeader
                k.RecordFields = Header.RecordFields
                For Each r In Header.ReferenceSequences
                    If Item.Key = r.Name Then k.ReferenceSequences.Add(r)
                Next

                For i1 = k.RecordFields.Count - 1 To 0 Step -1
                    If k.RecordFields(i1).Typecode = "SQ" Then
                        If k.RecordFields(i1).Tags.First.Value <> Item.Key Then
                            k.RecordFields.RemoveAt(i1)
                        End If
                    End If
                Next

                Szunyi.IO.Export.Export_Sam(Item.Value, k, New FileInfo(File.FullName & "_" & Item.Key & ".sam"))
            Next


        Next
    End Sub

    Private Sub IntoBamToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles IntoBamToolStripMenuItem.Click
        Dim Org_FIle = Szunyi.IO.Files.Filter.SelectFile(Constants.Files.All_TAB_Like, "Select Org File Name tab IDs ")
        If IsNothing(Org_FIle) = True Then Exit Sub
        Dim Chrs_With_Species As New Dictionary(Of String, String)
        Dim Species As New List(Of String)
        For Each Line In Szunyi.IO.Import.Text.ReadLines(Org_FIle, 0)
            Dim s = Split(Line, vbTab)

            Species.Add(s.First)
            If Chrs_With_Species.ContainsKey(s(1)) = False Then Chrs_With_Species.Add(s(1), s.First)
        Next

        Species = Species.Distinct.ToList
        Dim Files = Szunyi.IO.Files.Filter.SelectFiles(Szunyi.Constants.Files.SAM_BAM)
        If IsNothing(Files) = True Then Exit Sub
        Dim All As New Dictionary(Of FileInfo, Dictionary(Of String, List(Of Bio.IO.SAM.SAMAlignedSequence)))
        For Each File In Files
            Dim Header = Bam_Basic_IO.Headers.Get_Header(File)
            Dim res As New Dictionary(Of String, List(Of Bio.IO.SAM.SAMAlignedSequence)) 'Organism And COunt
            For Each sp In Species
                res.Add(sp, New List(Of Bio.IO.SAM.SAMAlignedSequence))
            Next


            For Each SA In Szunyi.BAM.Bam_Basic_IO.Import.Parse(File)
                If Chrs_With_Species.ContainsKey(SA.RName) Then
                    Dim Species_Name = Chrs_With_Species(SA.RName)
                    If res.ContainsKey(Species_Name) = False Then res.Add(Species_Name, New List(Of Bio.IO.SAM.SAMAlignedSequence))
                    res(Species_Name).Add(SA)
                End If
            Next
            For Each Item In res
                Szunyi.IO.Export.Export_Sam(Item.Value, Header, New FileInfo(File.FullName & "_" & Item.Key & ".sam"))
            Next


        Next


    End Sub

    Private Sub MergeSAMBySMRTIDsToolStripMenuItem_Click(sender As Object, e As EventArgs)
        Dim Files = Szunyi.IO.Files.Filter.SelectFiles(Szunyi.Constants.Files.SAM)

        Dim Ids = Szunyi.PacBio.Common.Get_IDs(Files).Distinct
        Dim str As New System.Text.StringBuilder
        For Each Id In Ids
            Dim sFIles = (From x In Files Where x.Name.Contains(Id)).ToList
            Dim NFIle As New FileInfo((sFIles.First.Directory.FullName & "/" & Id & ".sam"))
            Szunyi.BAM.Bam_Basic_IO.Export.Merge_Sams(sFIles, NFIle)
        Next

    End Sub

    Private Sub IntoFastaToolStripMenuItem2_Click(sender As Object, e As EventArgs) Handles IntoFastaToolStripMenuItem2.Click
        Dim Files = Szunyi.IO.Files.Filter.SelectFiles(Szunyi.Constants.Files.SAM_BAM)
        If IsNothing(Files) = True Then Exit Sub
        For Each File In Files
            Dim out As New List(Of Bio.ISequence)
            Dim fq As New Bio.IO.FastA.FastAFormatter()
            fq.Open(File.FullName & "_Unique.fasta")
            fq.AutoFlush = True
            Dim fq1 As New Bio.IO.FastA.FastAFormatter()
            fq1.Open(File.FullName & "_All.fasta")
            fq1.AutoFlush = True
            Dim SAMs = Szunyi.BAM.Bam_Basic_IO.Import.ParseAll(File)
            For Each Unique In Szunyi.BAM.GroupBy.ReadID(SAMs)
                fq.Format(Unique.First.QuerySequence)
            Next
            For Each sam In SAMs
                fq1.Format(sam.QuerySequence)
            Next
            fq1.Close
            fq.Close
        Next
    End Sub
    Private Sub IntoFastQToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles IntoFastQToolStripMenuItem.Click
        Dim Files = Szunyi.IO.Files.Filter.SelectFiles(Szunyi.Constants.Files.SAM_BAM)
        If IsNothing(Files) = True Then Exit Sub
        For Each File In Files
            Dim out As New List(Of Bio.ISequence)
            Dim fq As New Bio.IO.FastQ.FastQFormatter()
            fq.Open(File.FullName & ".fastq")
            fq.AutoFlush = True
            Dim SAMs = Szunyi.BAM.Bam_Basic_IO.Import.ParseAll(File)
            For Each Unique In Szunyi.BAM.GroupBy.ReadID(SAMs)
                fq.Format(Unique.First.QuerySequence)
            Next
            fq.Close
        Next
    End Sub

    Private Sub CMVLikeByOrgToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles CMVLikeByOrgToolStripMenuItem.Click
        Dim Files = Szunyi.IO.Files.Filter.SelectFiles(Szunyi.Constants.Files.SAM_BAM)
        Dim RefSeqs = Szunyi.BAM.Bam_Basic_IO.Headers.Get_ReferenceSequences(Files)
        Dim str As New System.Text.StringBuilder
        '   Dim Header = Split("File Name,Nof Read,Nof Mapping,Median Read Length,Avr. Read Length,Avr. Read Length SD,Median Aligned Read Length,Avr. Aligned Read Length,Avr. Aligned Read Length SD,Deleteion %,Deleteion % SD,Insertion %,Insertion % SD,Match%,Match% SD,MisMatch%,MisMatch% SD,Coverage", ",")
        '    str.Append(Szunyi.Text.General.GetText(Header, vbTab))
        str.Append(Szunyi.BAM.Stat.Stat.Get_Header_By_ORGs(RefSeqs)).AppendLine()

        For Each GoodBam In Files
            str.AppendLine()
            Dim Sams = Szunyi.BAM.Bam_Basic_IO.Import.Parse(GoodBam)

            Dim GoodSams = (From x In Sams Where x.Flag <> Bio.IO.SAM.SAMFlags.UnmappedQuery).ToList
            If GoodSams.Count <> 0 Then

                str.Append(Szunyi.BAM.Stat.Stat.All_By_ORG(GoodSams, GoodBam, RefSeqs))
            Else

                str.Append(GoodBam.Name)
            End If

        Next
        If str.Length > 0 Then
            Clipboard.SetText(str.ToString)
        End If
        Beep()

    End Sub

    Private Sub Stat_CMVLikeToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles CMVLikeToolStripMenuItem.Click
        Dim Files = Szunyi.IO.Files.Filter.SelectFiles(Szunyi.Constants.Files.SAM_BAM)
        Dim str As New System.Text.StringBuilder
        Dim Header = Split("File Name,Nof Read,Nof Mapping,Median Read Length,Avr. Read Length,Avr. Read Length SD,Median Aligned Read Length,Avr. Aligned Read Length,Avr. Aligned Read Length SD,Deleteion %,Deleteion % SD,Insertion %,Insertion % SD,Match%,Match% SD,MisMatch%,MisMatch% SD,Coverage", ",")

        str.Append(Szunyi.Text.General.GetText(Header, vbTab))
        For Each GoodBam In Files
            str.AppendLine()
            Dim Sams = Szunyi.BAM.Bam_Basic_IO.Import.Parse(GoodBam)

            Dim GoodSams = (From x In Sams Where x.Flag <> Bio.IO.SAM.SAMFlags.UnmappedQuery).ToList
            If GoodSams.Count <> 0 Then
                str.Append(Szunyi.BAM.Stat.Stat.All(GoodSams, GoodBam))
            Else
                str.Append(GoodBam.Name)
            End If

        Next
        Clipboard.SetText(str.ToString)
        Beep()
    End Sub


    Private Sub SPlitByToolStripMenuItem1_Click(sender As Object, e As EventArgs)
        Dim BFIle = Szunyi.IO.Files.Filter.SelectFile(Szunyi.Constants.Files.All_TAB_Like)
        If IsNothing(BFIle) = True Then Exit Sub
        Dim ByName As New Dictionary(Of String, List(Of Bio.IO.SAM.SAMAlignedSequence))
        Dim BySMRTID As New Dictionary(Of String, String)
        For Each l In Szunyi.IO.Import.Text.ParseToArray(BFIle, vbTab)
            If ByName.ContainsKey(l.First) = False Then ByName.Add(l.First, New List(Of Bio.IO.SAM.SAMAlignedSequence))
            BySMRTID.Add(l.Last, l.First)
        Next
        Dim Files = Szunyi.IO.Files.Filter.SelectFiles(Szunyi.Constants.Files.SAM_BAM)
        Dim str As New System.Text.StringBuilder

        For Each File In Files
            Dim Header = Bam_Basic_IO.Headers.Get_Header(File)
            For Each Sam In Szunyi.BAM.Bam_Basic_IO.Import.Parse(File)
                Dim ID = Szunyi.PacBio.Common.Get_ID(Sam.QuerySequence.ID)
                Dim Key = BySMRTID(ID)
                ByName(Key).Add(Sam)
            Next
            For Each Name1 In ByName
                Dim s = Split(File.FullName, "_GMAP")
                Dim nFile As New FileInfo(s.First & Name1.Key & ".sam")
                Szunyi.IO.Export.Export_Sam(Name1.Value, Header, nFile)
            Next
        Next

    End Sub


    Private Sub CutDownOverHangsToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles CutDownOverHangsToolStripMenuItem.Click
        Dim Files = Szunyi.IO.Files.Filter.SelectFiles(Constants.Files.SAM_BAM)
        If IsNothing(Files) = True Then Exit Sub
        Dim i = MyInputBox.GetInteger("Enter max nof NA maintain in Soft Clipped Regions")
        For Each FIle In Files
            Dim Header = Bam_Basic_IO.Headers.Get_Header(FIle)
            Dim out As New List(Of Bio.IO.SAM.SAMAlignedSequence)
            For Each SAM In Szunyi.BAM.Bam_Basic_IO.Import.Parse(FIle)
                SAM = Szunyi.BAM.Trim.TrimH(SAM)
                SAM = Szunyi.BAM.Trim.TrimS(SAM, i)
                out.Add(SAM)
            Next
            Dim nFIle As New FileInfo(FIle.FullName & "_MaxinS_" & i & ".sam")
            Szunyi.IO.Export.Export_Sam(out, Header, nFIle)
        Next
    End Sub

#Region "Split"

#End Region
#End Region


    Private Sub GetReadIDsToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles GetReadIDsToolStripMenuItem.Click
        Dim Files = Szunyi.IO.Files.Filter.SelectFiles(Constants.Files.SAM_BAM)
        If IsNothing(Files) = True Then Exit Sub
        For Each File In Files
            Dim s = Szunyi.DNA.Chimera.Get_ReadIDs(File)
            Szunyi.IO.Export.SaveText(Szunyi.Text.General.GetText(s), New FileInfo(File.FullName & "_ReadIDs.tsv"))
        Next
    End Sub

    Private Sub GetFastafastqFromReadIDsToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles GetFastafastqFromReadIDsToolStripMenuItem.Click
        Dim File = Szunyi.IO.Files.Filter.SelectFile(Constants.Files.All_TAB_Like)
        If IsNothing(File) = True Then Exit Sub
        Dim ReadIDs = Szunyi.IO.Import.Text.ReadLines(File)
        ReadIDs.Sort()
        Dim Files = Szunyi.IO.Files.Filter.SelectFiles(Constants.Files.Fasta_FastQ)
        If IsNothing(Files) = True Then Exit Sub

        For Each F In Files
            Dim fa As New List(Of Bio.ISequence)
            Dim fq As New List(Of Bio.ISequence)
            Szunyi.IO.Export.SaveAsQualitativeSeq(fq, New FileInfo(F.FullName & ".fastq.tmp"))
            For Each Seq In Szunyi.IO.Import.Sequence.GetOnyByONe(F)
                Dim ID = Split(Seq.ID, " ").First
                Dim Index = ReadIDs.BinarySearch(ID)
                If Index > -1 Then
                    If Seq.GetType.ToString = "Bio.QualitativeSequence" Then
                        Seq.ID = ID
                        fq.Add(Seq)
                    Else
                        fa.Add(Seq)
                    End If
                End If
            Next
            If fq.Count > 0 Then Szunyi.IO.Export.SaveAsQualitativeSeq(fq, New FileInfo(F.FullName & ".fastq"))
            If fa.Count > 0 Then Szunyi.IO.Export.SaveAsSimpleFasta(fa, New FileInfo(F.FullName & ".fastq"))

        Next

    End Sub

#Region "Table"

#Region "Intron"
    Private Sub WithOrientationToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles WithOrientationToolStripMenuItem.Click
        Dim Seqs = Me.GetSeqsFromSelectedList(True)
        If IsNothing(Seqs) = True Then Exit Sub
        Dim Files = Szunyi.IO.Files.Filter.SelectFiles(Szunyi.Constants.Files.SAM_BAM)
        If IsNothing(Files) = True Then Exit Sub
        Dim x As New Szunyi.BAM.Settings.Intron

        Dim MinIntronLength = MyInputBox.GetInteger("Minimum length of Intron")
        Dim MaxIntronLength = MyInputBox.GetInteger("Maximum Intron Length")
        Dim MinExonBorderLength = MyInputBox.GetInteger("Minimum Exon Border Length")

        Szunyi.BAM.Convert.Table.Intron(MinIntronLength, MaxIntronLength, MinExonBorderLength, Files, Seqs, True, True)


    End Sub

    Private Sub WoOrientationToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles WoOrientationToolStripMenuItem.Click
        Dim Seqs = Me.GetSeqsFromSelectedList(True)
        If IsNothing(Seqs) = True Then Exit Sub
        Dim Files = Szunyi.IO.Files.Filter.SelectFiles(Szunyi.Constants.Files.SAM_BAM)
        If IsNothing(Files) = True Then Exit Sub
        Dim MinIntronLength = MyInputBox.GetInteger("Minimum length of Intron")
        Dim MaxIntronLength = MyInputBox.GetInteger("Maximum Intron Length")
        Dim MinExonBorderLength = MyInputBox.GetInteger("Minimum Exon Border Length")
        For Each File In Files
            Szunyi.BAM.Convert.Table.Intron(MinIntronLength, MaxIntronLength, MinExonBorderLength, File, Seqs, True, False)
        Next

    End Sub

    Private Sub BothToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles BothToolStripMenuItem.Click
        Me.Get_Bam_Files()
        Dim Seqs = Me.GetSeqsFromSelectedList(True)
        If IsNothing(Seqs) = True Then Exit Sub
        Dim Files = Szunyi.IO.Files.Filter.SelectFiles(Szunyi.Constants.Files.SAM_BAM)
        If IsNothing(Files) = True Then Exit Sub
        Dim MinIntronLength = MyInputBox.GetInteger("Minimum length of Intron")
        Dim MaxIntronLength = MyInputBox.GetInteger("Maximum Intron Length")
        Dim MinExonBorderLength = MyInputBox.GetInteger("Minimum Exon Border Length")
        Szunyi.BAM.Convert.Table.Intron(MinIntronLength, MaxIntronLength, MinExonBorderLength, Files, Seqs, True, False)
        Szunyi.BAM.Convert.Table.Intron(MinIntronLength, MaxIntronLength, MinExonBorderLength, Files, Seqs, True, True)

    End Sub

#End Region
    Private Sub FalsePolyAToTableToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles FlasePolyAToTableToolStripMenuItem.Click
        Dim files = Szunyi.IO.Files.Filter.SelectFiles(Constants.Files.BAM)
        Dim Seqs = Me.GetSeqsFromSelectedList(True)
        If IsNothing(Seqs) = True Then Exit Sub

        If IsNothing(files) = False Then
            For Each File In files
                Dim str As New System.Text.StringBuilder
                str.Append("pA or pT").Append(vbTab)
                str.Append("Position").Append(vbTab)
                str.Append("Nof Read").Append(vbTab)
                str.Append("Nof A From Beginning").Append(vbTab)
                str.Append("Nof Consecutive A").Append(vbTab)
                str.Append("Percents of A").Append(vbTab)
                str.Append("Nof T From Beginning").Append(vbTab)
                str.Append("Nof Consecutive T").Append(vbTab)
                str.Append("Percents of T").AppendLine()
                Dim Sams = Szunyi.BAM.Bam_Basic_IO.Import.ParseAll(File)

                For Each LS In From x In Sams Group By x.Pos Into Group
                    str.Append("pT").Append(vbTab)
                    str.Append(LS.Pos).Append(vbTab)
                    str.Append(LS.Group.Count).Append(vbTab)
                    str.Append(Szunyi.mRNA.PolyA.False_polyA(Seqs.First, LS.Group.First, 10, mRNA.False_PolyAT_Discovering.From_Beginning, 0, Bio.Alphabets.DNA.A)).Append(vbTab)
                    str.Append(Szunyi.mRNA.PolyA.False_polyA(Seqs.First, LS.Group.First, 10, mRNA.False_PolyAT_Discovering.Nof_Consecutive_AT, 0, Bio.Alphabets.DNA.A)).Append(vbTab)
                    str.Append(Szunyi.mRNA.PolyA.False_polyA(Seqs.First, LS.Group.First, 10, mRNA.False_PolyAT_Discovering.Percent_of_AT, 0, Bio.Alphabets.DNA.A)).Append(vbTab)
                    str.Append(Szunyi.mRNA.PolyA.False_polyT(Seqs.First, LS.Group.First, 10, mRNA.False_PolyAT_Discovering.From_Beginning, 0, Bio.Alphabets.DNA.T)).Append(vbTab)
                    str.Append(Szunyi.mRNA.PolyA.False_polyT(Seqs.First, LS.Group.First, 10, mRNA.False_PolyAT_Discovering.Nof_Consecutive_AT, 0, Bio.Alphabets.DNA.T)).Append(vbTab)
                    str.Append(Szunyi.mRNA.PolyA.False_polyT(Seqs.First, LS.Group.First, 10, mRNA.False_PolyAT_Discovering.Percent_of_AT, 0, Bio.Alphabets.DNA.T)).AppendLine()
                Next

                For Each LS In From x In Sams Group By x.RefEndPos Into Group
                    str.Append("pA").Append(vbTab)
                    str.Append(LS.RefEndPos).Append(vbTab)
                    str.Append(LS.Group.Count).Append(vbTab)
                    str.Append(Szunyi.mRNA.PolyA.False_polyA(Seqs.First, LS.Group.First, 10, mRNA.False_PolyAT_Discovering.From_Beginning, 0, Bio.Alphabets.DNA.A)).Append(vbTab)
                    str.Append(Szunyi.mRNA.PolyA.False_polyA(Seqs.First, LS.Group.First, 10, mRNA.False_PolyAT_Discovering.Nof_Consecutive_AT, 0, Bio.Alphabets.DNA.A)).Append(vbTab)
                    str.Append(Szunyi.mRNA.PolyA.False_polyA(Seqs.First, LS.Group.First, 10, mRNA.False_PolyAT_Discovering.Percent_of_AT, 0, Bio.Alphabets.DNA.A)).Append(vbTab)
                    str.Append(Szunyi.mRNA.PolyA.False_polyT(Seqs.First, LS.Group.First, 10, mRNA.False_PolyAT_Discovering.From_Beginning, 0, Bio.Alphabets.DNA.T)).Append(vbTab)
                    str.Append(Szunyi.mRNA.PolyA.False_polyT(Seqs.First, LS.Group.First, 10, mRNA.False_PolyAT_Discovering.Nof_Consecutive_AT, 0, Bio.Alphabets.DNA.T)).Append(vbTab)
                    str.Append(Szunyi.mRNA.PolyA.False_polyT(Seqs.First, LS.Group.First, 10, mRNA.False_PolyAT_Discovering.Percent_of_AT, 0, Bio.Alphabets.DNA.T)).AppendLine()
                Next
                Dim nfile As New FileInfo(File.FullName & "_FalsePolyAT_Table.tsv")
                Szunyi.IO.Export.SaveText(str.ToString, nfile)

            Next

        End If
    End Sub

    Private Sub ScoreTableFromAdaptersToolStripMenuItem1_Click(sender As Object, e As EventArgs) Handles ScoreTableFromAdaptersToolStripMenuItem1.Click
        Dim files = Szunyi.IO.Files.Filter.SelectFiles(Constants.Files.SAM_BAM)
        If IsNothing(files) = True Then Exit Sub
        For Each File In files
            Szunyi.BAM.Convert.Table.Adaptor_Distribution(File)
        Next


    End Sub
    Private Sub WOrientatonToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles WOrientatonToolStripMenuItem.Click

    End Sub

    Private Sub WoOrientationToolStripMenuItem1_Click_1(sender As Object, e As EventArgs) Handles WoOrientationToolStripMenuItem1.Click
        Dim Seqs = Me.GetSeqsFromSelectedList(True)
        If IsNothing(Seqs) = True Then Exit Sub

        Dim AllFeatureTpye = Szunyi.Features.FeatureManipulation.Common.Get_Feature_Types(Seqs)

        Dim f1 As New CheckBoxForStringsFull(AllFeatureTpye, -1, "Select Features")
        If f1.ShowDialog <> DialogResult.OK Then Exit Sub

        Dim Feats = Szunyi.Features.FeatureManipulation.GetFeaturesByType.GetFeturesByTypesFromSeqs(Seqs, f1.SelectedStrings)

        Dim Bam_files = Get_Bam_Files()

        Dim Width As Integer = MyInputBox.GetInteger("Enter Width")

        For Each File In Bam_files

            Dim str As New System.Text.StringBuilder

            str.Append("#").Append(File.Name).AppendLine()
            str.Append("#Width").Append(Width).AppendLine()


            Dim SAMs = Szunyi.BAM.Bam_Basic_IO.Import.ParseAll(File)
            Dim f_SAMs = Szunyi.BAM.Filter_Split.Get_Where_No_Intorns(SAMs, 40)
            Dim s = Split("Feat Strand,Feat Start,Feat End,Name Of Feat,cof PAS,cof transcripts,Read Strand,Read Start,Read End,Read Accession", ",")
            Dim Own_ALs = Szunyi.Alignment.Own_Al_Helper.Get_List(f_SAMs)
            Dim Locis = Szunyi.Location.Common.GetLocation(Own_ALs)
            str.Append(Szunyi.Text.General.GetText(s, vbTab)).AppendLine()
            For Each Feat In Feats
                If Feat.Location.IsComplementer = True Then
                    Dim cLocis = From x In Locis Where x.LocationStart - Width <= Feat.Location.PAS And x.LocationStart + Width >= Feat.Location.PAS
                    Dim Perfect_Locis = (From x In cLocis Where x.LocationEnd - Width <= Feat.Location.TSS And x.LocationEnd + Width >= Feat.Location.TSS).ToList
                    Dim Longest = (From x In cLocis Order By x.LocationEnd).ToList
                    str.Append(Szunyi.Location.Common.GetLocationStringTab(Feat)).Append(vbTab).Append(Feat.Qualifiers("Name").First).Append(vbTab)
                    str.Append(cLocis.Count).Append(vbTab)
                    str.Append(Perfect_Locis.Count).Append(vbTab)
                    If Longest.Count = 0 Then
                        str.AppendLine()
                    Else
                        str.Append(Szunyi.Location.Common.GetLocationStringTab(Longest.Last)).Append(vbTab)
                        str.Append(Longest.Last.Accession)
                        str.AppendLine()
                    End If


                Else
                    Dim cLocis = From x In Locis Where x.LocationEnd - Width <= Feat.Location.PAS And x.LocationEnd + Width >= Feat.Location.PAS


                    Dim Perfect_Locis = (From x In cLocis Where x.LocationStart - Width <= Feat.Location.TSS And x.LocationEnd + Width >= Feat.Location.TSS).ToList
                    Dim Longest = (From x In cLocis Order By x.LocationStart).ToList
                    str.Append(Szunyi.Location.Common.GetLocationStringTab(Feat)).Append(vbTab).Append(Feat.Qualifiers("Name").First).Append(vbTab)
                    str.Append(cLocis.Count).Append(vbTab)
                    str.Append(Perfect_Locis.Count).Append(vbTab)
                    If Longest.Count = 0 Then
                        str.AppendLine()
                    Else
                        str.Append(Szunyi.Location.Common.GetLocationStringTab(Longest.Last)).Append(vbTab)
                        str.Append(Longest.Last.Accession)
                        str.AppendLine()
                    End If


                    Dim alf As Int16 = 43
                End If
            Next

            Clipboard.SetText(str.ToString)
            Beep()

        Next ' Bam File



    End Sub
    Private Sub ToScoreByFilesToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles ToScoreByFilesToolStripMenuItem.Click
        Dim Files = Get_Bam_Files()
        If IsNothing(Files) = True Then Exit Sub
        Dim AD_Files = Szunyi.IO.Files.Filter.SelectFiles(Constants.Files.Xml)
        If IsNothing(AD_Files) = True Then Exit Sub
        For Each FIle In Files
            Dim Headers = Bam_Basic_IO.Headers.Get_Header(FIle)
            Dim Adaptors = Szunyi.BAM.Bam_Basic_IO.Headers.Get_Adaptors(Headers)
            For Each AD_File In AD_Files
                Dim res = ClassLibrary1.Szunyi.IO.XML.Deserialize(Of List(Of Szunyi.Transcipts.Adapter_Filtering))(ClassLibrary1.Szunyi.IO.Import.Text.ReadToEnd(AD_File))
                Dim ADs As New Dictionary(Of Szunyi.Transcipts.Adaptor_Pair, Szunyi.Transcipts.Adapter_Filtering)
                For Each r In res
                    Dim x = From h In Adaptors Where r.AP_Name = h.Name
                    ADs.Add(x.First, r)
                Next

                Dim Header As New System.Text.StringBuilder
                Header.Append("# ").Append(FIle.FullName).AppendLine()
                Header.Append(Szunyi.BAM.Bam_Basic_IO.Headers.Get_Comments(FIle))
                Header.Append(Szunyi.Transcipts.Adaptor_Aligner_Result_Manipulation.Get_Description(ADs))

                Headers.Comments.Add(Szunyi.Transcipts.Adaptor_Aligner_Result_Manipulation.Get_Description(ADs).Replace(vbCrLf, ","))
                Dim Sams = Szunyi.BAM.Bam_Basic_IO.Import.Parse(FIle)
                Dim Orgs = Szunyi.BAM.Bam_Basic_IO.Headers.Get_ReferenceSequences(FIle)


                For Each Org In Orgs
                    Dim cSams = From x In Sams Where x.RName = Org.Name

                    If cSams.Count > 0 Then
                        Dim out = Szunyi.mRNA.PolyA.Get_Dictionary_wListOfSams
                        For Each SAM In cSams
                            Dim M_Orientation = Szunyi.Transcipts.Orientation.Get_Orientation(SAM, ADs)

                            Szunyi.Transcipts.Orientation.Set_SAM_Flag(SAM, M_Orientation)
                            out(Szunyi.mRNA.PolyA_Type.All).Add(SAM)
                            out(M_Orientation).Add(SAM)
                        Next
                        out(Szunyi.mRNA.PolyA_Type.polyAT).AddRange(out(Szunyi.mRNA.PolyA_Type.polyA))
                        out(Szunyi.mRNA.PolyA_Type.polyAT).AddRange(out(Szunyi.mRNA.PolyA_Type.polyT))

                        Dim For_PAS = Szunyi.Transcipts.Adaptor_Aligner_Result_Manipulation.Get_PASs_By_Positions(out(Szunyi.mRNA.PolyA_Type.polyAT), ADs)
                        Dim For_TSS = Szunyi.Transcipts.Adaptor_Aligner_Result_Manipulation.Get_TSSs_By_Positions(out(Szunyi.mRNA.PolyA_Type.polyAT), ADs)


                        Dim Locis_TSS = Szunyi.Location.Common.GetLocation(For_TSS)
                        Dim TSS_Sites As New Szunyi.Location.Sites(Org.Length, Locis_TSS, Sort_Locations_By.TSS)
                        Dim TSSs_Table = Header.ToString & vbCrLf & TSS_Sites.Poisson(10, 50, 0.05)

                        Dim Locis_PAS = Szunyi.Location.Common.GetLocation(out(Szunyi.mRNA.PolyA_Type.polyAT))
                        Dim PAS_Sites As New Szunyi.Location.Sites(Org.Length, Locis_TSS, Sort_Locations_By.PAS)
                        Dim PAS_Table = Header.ToString & vbCrLf & PAS_Sites.Poisson(10, 50, 0.05)


                        Dim Dir As New DirectoryInfo(FIle.DirectoryName & "\" & Org.Name & "_" & AD_File.Name)
                        Dir.Create()
                        Szunyi.IO.Export.SaveText(TSSs_Table, New FileInfo(Dir.FullName & "\" & FIle.Name & "_TSSs_Adapters_Founded.tsv"))
                        Szunyi.IO.Export.SaveText(PAS_Table, New FileInfo(Dir.FullName & "\" & FIle.Name & "_PASs_PolyAT_Founded.tsv"))
                        Szunyi.mRNA.PolyA.Write_Orientation(out, Dir, FIle, Headers)
                    End If

                Next
            Next
        Next
    End Sub


#End Region


#Region "File Selections"
    ''' <summary>
    ''' return Sam Or Bam Files or Nothing
    ''' </summary>
    ''' <returns></returns>
    Private Shared Function Get_Bam_Files(Optional Title As String = "") As List(Of FileInfo)
        Dim FIles = Szunyi.IO.Files.Filter.SelectFiles(Constants.Files.SAM_BAM, Title)
        Return FIles

    End Function
    Private Shared Function Get_Fast_a_q_Files() As List(Of FileInfo)
        Dim FIles = Szunyi.IO.Files.Filter.SelectFiles(Constants.Files.Fasta_FastQ)
        Return FIles

    End Function
    Private Shared Function Get_TSV_FIle() As FileInfo
        Dim FIles = Szunyi.IO.Files.Filter.SelectFile(Constants.Files.All_TAB_Like)
        Return FIles

    End Function
#End Region

    Private Sub MinionBarCodeToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles MinionBarCodeToolStripMenuItem.Click
        Dim File = Szunyi.IO.Files.Filter.SelectFile(Constants.Files.All_TAB_Like)
        If IsNothing(File) = True Then Exit Sub
        ' Name Date RunID FlowCellID
        Dim RunIDs_FlowCellIDs As New Dictionary(Of String, String)
        Dim FlowCells As New Dictionary(Of String, List(Of String))
        For Each Line In Szunyi.IO.Import.Text.ParseNotFirst(File, 1)
            Dim s = Split(Line, vbTab)
            RunIDs_FlowCellIDs.Add(s(2), s(3))
            If FlowCells.ContainsKey(s(3)) = False Then
                FlowCells.Add(s(3), New List(Of String))
            End If
        Next
        Dim FastQ_Files = Szunyi.IO.Files.Filter.SelectFiles(Constants.Files.FastQ)
        If IsNothing(FastQ_Files) = True Then Exit Sub
        Dim Log As New List(Of String)
        Dim out As New Dictionary(Of String, List(Of String))
        For Each Seq In Szunyi.IO.Import.Sequence.From_Files_Iterator(FastQ_Files)
            Dim PureSeqID = Split(Seq.ID, " ").First
            Dim RunID = Szunyi.MinIon.Common.Get_RunID_From_SeqID(Seq.ID)
            If RunIDs_FlowCellIDs.ContainsKey(RunID) = False Then
                If Log.Contains(RunID) = False Then Log.Add(RunID)
            Else
                Dim FC = RunIDs_FlowCellIDs(RunID)
                FlowCells(FC).Add(PureSeqID)
            End If

        Next
        For Each FC In FlowCells
            FC.Value.Sort()
            Szunyi.IO.Export.SaveText(Szunyi.Text.General.GetText(FC.Value), New FileInfo(File.DirectoryName & "\" & FC.Key & "Sorted_ReadIDs"))
        Next

    End Sub

    Private Sub ForTarToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles ForTarToolStripMenuItem.Click
        ' Bam FIles, ReadID File
        ' Create SubDirectory For Bam Files
        ' Copy Files into Dir
        Dim x As New Szunyi.MinIon.ToTar


    End Sub

    Private Sub GetReadIDFromFilenamesToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles GetReadIDFromFilenamesToolStripMenuItem.Click
        Dim FIle = Szunyi.IO.Files.Filter.SelectFile()
        If IsNothing(FIle) = True Then Exit Sub
        Dim notFOund As Integer = 0
        Dim log As New System.Text.StringBuilder
        Using sw As New StreamWriter(FIle.FullName & "_wReadID.tdt", True)
            For Each Line In Szunyi.IO.Import.Text.Parse(FIle)
                If Line.EndsWith(".fast5") Then
                    Dim cFile As New FileInfo(Line)

                    Dim t = Szunyi.MinIon.HDF5.Get_Raw_Reads(cFile)
                    If IsNothing(t) = False Then
                        sw.Write(cFile.FullName)
                        sw.Write(vbTab)
                        sw.Write(t("read_id"))
                        sw.WriteLine()
                    Else
                        Log.Append(cFile.FullName).AppendLine()
                        notFOund += 1
                    End If


                End If
            Next
        End Using
        MsgBox("Not FOund:" & notFOund)
        Szunyi.IO.Export.SaveText(log.ToString, New FileInfo(FIle.FullName & "_wReadID.log"))
    End Sub
    Private Sub StFileFromRecursiceDirsToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles StFileFromRecursiceDirsToolStripMenuItem.Click
        Dim x As New Szunyi.MinIon.ToTar_FastQ
    End Sub

    Private Sub MergeSelectedSamFIlesToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles MergeSelectedSamFIlesToolStripMenuItem.Click
        Dim FIles = Szunyi.IO.Files.Filter.SelectFiles(Szunyi.Constants.Files.SAM)
        If IsNothing(FIles) = True Then Exit Sub
        Dim Name As String = InputBox("Enter the name of the FIle")
        If Name = "" Then Exit Sub
        Szunyi.BAM.Bam_Basic_IO.Export.Merge_Sams(FIles, New FileInfo(FIles.First.DirectoryName & "\" & Name & ".sam"))

    End Sub
    Private Sub WithCustomRGToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles WithCustomRGToolStripMenuItem.Click
        Dim FIles = Szunyi.IO.Files.Filter.SelectFiles(Szunyi.Constants.Files.SAM)
        If IsNothing(FIles) = True Then Exit Sub
        Dim Name As String = InputBox("Enter the name of the FIle")
        If Name = "" Then Exit Sub
        Szunyi.BAM.Bam_Basic_IO.Export.Merge_Sams_RG(FIles, New FileInfo(FIles.First.DirectoryName & "\" & Name & ".sam"))

    End Sub
    Private Sub BySMRTIDFromFIleNameToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles BySMRTIDFromFIleNameToolStripMenuItem.Click
        Dim Files = Szunyi.IO.Files.Filter.SelectFiles(Szunyi.Constants.Files.SAM)

        Dim Ids = Szunyi.PacBio.Common.Get_IDs(Files).Distinct
        Dim str As New System.Text.StringBuilder
        For Each Id In Ids
            Dim sFIles = (From x In Files Where x.Name.Contains(Id)).ToList
            Dim NFIle As New FileInfo((sFIles.First.Directory.FullName & "/" & Id & ".sam"))
            Szunyi.BAM.Bam_Basic_IO.Export.Merge_Sams(sFIles, NFIle)
        Next
    End Sub

    Private Sub ByLastPartToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles ByLastPartToolStripMenuItem.Click
        Dim s = InputBox("Enter the separator string")
        If s = "" Then Exit Sub
        Dim Files = Szunyi.IO.Files.Filter.SelectFiles(Szunyi.Constants.Files.SAM)
        If IsNothing(Files) = True Then Exit Sub
        Dim gr = From x In Files Group By x.Name.Split(s).Last Into Group

        For Each g In gr
            Szunyi.BAM.Bam_Basic_IO.Export.Merge_Sams(g.Group, New FileInfo(g.Group.First.DirectoryName & "\" & s & ".sam"))
        Next
    End Sub

    Private Sub SameFileNamesToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles SameFileNamesToolStripMenuItem.Click
        Dim F_Dir = Szunyi.IO.Directory.Get_Folder("Select First Dir Containing Sam Files")
        If IsNothing(F_Dir) Then Exit Sub

        Dim S_Dir = Szunyi.IO.Directory.Get_Folder("Select Second Dir Containing Sam Files")
        If IsNothing(S_Dir) Then Exit Sub

        Dim O_Dir = Szunyi.IO.Directory.Get_Folder("Select Output Dir Containing Sam Files")
        If IsNothing(O_Dir) Then Exit Sub

        Dim All_Files = Szunyi.IO.Directory.GetAllFilesFromFolder(F_Dir)
        All_Files.AddRange(Szunyi.IO.Directory.GetAllFilesFromFolder(S_Dir))
        Dim Sam_Files = From x In All_Files Where x.Extension = ".sam"

        If Sam_Files.Count > 0 Then
            Dim gr = From x In Sam_Files Group By x.Name Into Group

            For Each g In gr
                Dim nFile As New FileInfo(O_Dir.FullName & "\" & g.Name)
                Szunyi.BAM.Bam_Basic_IO.Export.Merge_Sams(g.Group.ToList, nFile)
            Next

        End If
    End Sub
    Private Sub Picard_RGToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles RGToolStripMenuItem.Click

        Dim FIle = Get_TSV_FIle()
        If IsNothing(FIle) = True Then Exit Sub
        Dim res = Szunyi.Linux.Picard.RG(FIle)
        If res <> String.Empty Then Clipboard.SetText(res)
        Beep()
    End Sub
    Private Sub RemoveWarningsToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles RemoveWarningsToolStripMenuItem.Click
        Dim FIle = Szunyi.IO.Files.Filter.SelectFile(Szunyi.Constants.Files.rep)

        If IsNothing(FIle) = True Then Exit Sub

        Dim BRs = Szunyi.Linux.Picard.Get_Warning_Records(FIle)
        If BRs.Count = 0 Then Exit Sub
        Dim Sam_File = Get_Bam_Files()
        Szunyi.Linux.Picard.Remove_Bad_Records(Sam_File.First, BRs)
    End Sub
    Private Sub RemoveBADToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles RemoveBADToolStripMenuItem.Click
        Dim Sam_Files = Get_Bam_Files()
        For Each Sam_File In Sam_Files
            Dim x = Szunyi.IO.Files.Get_New_FileName.Replace_Extension(Sam_File, ".rep")
            If x.Exists = True Then

                Dim BRs = Szunyi.Linux.Picard.Get_Bad_Records(x)

                Szunyi.Linux.Picard.Remove_Bad_Records(Sam_File, BRs)
            End If
        Next


    End Sub
    Private Sub Picard_ValidateToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles ValidateToolStripMenuItem.Click
        Dim Files = Get_Bam_Files()
        If IsNothing(Files) = True Then Exit Sub
        Dim res = Szunyi.Linux.Picard.Validate(Files)
        If res <> String.Empty Then Clipboard.SetText(res)
        Beep()

    End Sub


    Private Sub FromFilesCheckToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles FromFilesCheckToolStripMenuItem.Click
        Dim FIles = Get_Fast_a_q_Files()
        Dim str As New System.Text.StringBuilder
        str.Append("FileName").Append(vbTab).Append("Sequence Count").Append(vbTab).Append("Unique Count").Append(vbTab).Append("Duplicated COunt")
        For Each File In FIles
            Dim Seqs = Szunyi.IO.Import.Sequence.FromFile(File)
            Dim USeq = Szunyi.Sequences.SequenceManipulation.UniqueDistinct.GetUniqueSeqsByID(Seqs)
            Dim dSeq = Szunyi.Sequences.SequenceManipulation.UniqueDistinct.GetDuplicatedSeqsByID(Seqs)
            Dim OneSeqByID = Szunyi.Sequences.SequenceManipulation.UniqueDistinct.Get1CopyByID(Seqs)

            If dSeq.Count > 0 Then
                Dim kj As Int16 = 43
            End If
            str.AppendLine.Append(File.Name).Append(vbTab).Append(Seqs.Count).Append(vbTab).Append(USeq.Count).Append(dSeq.Count)
            Dim nFIle As New FileInfo(File.FullName & "_Unique_e" & USeq.Count & File.Extension)
            Szunyi.IO.Export.SaveSequencesToSingleFasta(OneSeqByID, nFIle)
        Next
        Clipboard.SetText(str.ToString)

        Beep()

    End Sub

    Private Sub ReNameHeadersToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles ReNameHeadersToolStripMenuItem.Click
        Dim Original_Header_Files = Get_Bam_Files("Select Original Header")
        If IsNothing(Original_Header_Files) = True Then Exit Sub
        Dim Header = Szunyi.BAM.Bam_Basic_IO.Headers.Get_Header(Original_Header_Files.First)
        Dim Files = Get_Bam_Files("Select Files to Modify Header")
        If IsNothing(Files) = True Then Exit Sub

        For Each FIle In Files
            Dim nFIle As New FileInfo(FIle.FullName & "_reNamedHeader.sam")
            Using x As New Szunyi.BAM.Bam_Basic_IO.Export(nFIle, Header)

                For Each Sam In Szunyi.BAM.Bam_Basic_IO.Import.Parse(FIle)
                    x.Write(Sam)
                Next
            End Using


        Next
    End Sub

    Private Sub GetScoresToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles GetScoresToolStripMenuItem.Click
        Dim S = Get_Score_Setting()
        If IsNothing(S) = True Then Exit Sub
        For Each F In S.Files
            For Each R In S.Regions
                Dim R_postFix As String = ("_Region" & R.In_Match_Length & "_" & R.Ouf_of_Match_Length & "_")
                Dim str_Short As New System.Text.StringBuilder
                Dim str_Big As New System.Text.StringBuilder

                Dim Header = Bam_Basic_IO.Headers.Get_Header(F)
                Dim File As New FileInfo(F.DirectoryName & "\" & "wAdaptor" & R_postFix & F.Name & ".sam")
                For Each Item In S.Adapter_Pairs
                    Header.Comments.Add(Item.ToString)
                Next

                Using x As New Bam_Basic_IO.Export(File, Header)
                    For Each sam In Szunyi.BAM.Bam_Basic_IO.Import.Parse(F)
                        If str_Short.Length = 0 Then
                            str_Short.Append(MappingII.Common_Mapping_V1_IO.Get_Short_Header_Score_Table(S.Adapter_Pairs)).AppendLine()
                            str_Big.Append(MappingII.Common_Mapping_V1_IO.Get_Big_Header_Score_Table(S.Adapter_Pairs)).AppendLine()


                        End If
                        str_Short.Append(sam.QName)
                        str_Big.Append(sam.QName).Append(vbTab)
                        str_Big.Append(sam.Pos).Append(vbTab)
                        str_Big.Append(sam.RefEndPos).Append(vbTab)

                        Dim U = Szunyi.BAM.SAM_Manipulation.QuerySequnce.Get_Five_Primer_Region(sam, R)
                        Dim D = Szunyi.BAM.SAM_Manipulation.QuerySequnce.Get_Three_Primer_Region(sam, R)
                        str_Big.Append(U.ConvertToString).Append(":").Append(D.ConvertToString)
                        For Each A In S.Adapter_Pairs
                            Dim AAR = Szunyi.Transcipts.Orientation.Get_AAR(U, D, R, A.Five_Prime_Adapter, A.Three_Prime_Adapter)
                            Szunyi.BAM.BAM_Optional_Filed_Manipulation.Add_AAR(sam, A.PreFix, AAR)
                            str_Short.Append(vbTab).Append(Szunyi.Transcipts.Adaptor_Aligner_Result_Manipulation.Get_Scores(AAR))
                            str_Big.Append(vbTab).Append(Szunyi.Transcipts.Adaptor_Aligner_Result_Manipulation.Get_Five_Prime_Score(AAR))
                            str_Big.Append(vbTab).Append(Szunyi.Transcipts.Adaptor_Aligner_Result_Manipulation.Get_Five_Seqs(AAR))
                            str_Big.Append(vbTab).Append(Szunyi.Transcipts.Adaptor_Aligner_Result_Manipulation.Get_Five_Prime_Pos(AAR))
                            str_Big.Append(vbTab).Append(Szunyi.Transcipts.Adaptor_Aligner_Result_Manipulation.Get_Five_Prime_Pos_Adapter(AAR))

                            str_Big.Append(vbTab).Append(Szunyi.Transcipts.Adaptor_Aligner_Result_Manipulation.Get_Three_Prime_Score(AAR))
                            str_Big.Append(vbTab).Append(Szunyi.Transcipts.Adaptor_Aligner_Result_Manipulation.Get_Three_Seqs(AAR))
                            str_Big.Append(vbTab).Append(Szunyi.Transcipts.Adaptor_Aligner_Result_Manipulation.Get_Three_Prime_Pos(AAR))
                            str_Big.Append(vbTab).Append(Szunyi.Transcipts.Adaptor_Aligner_Result_Manipulation.Get_Three_Prime_Pos_Adapter(AAR))

                        Next
                        x.Write(sam)

                        str_Short.AppendLine()
                        str_Big.AppendLine()
                    Next ' Last SAM
                End Using
                Szunyi.IO.Export.SaveText(str_Short.ToString, New FileInfo(F.DirectoryName & "\" & "Adaptor_Scores" & R_postFix & F.Name & ".tsv"))
                Szunyi.IO.Export.SaveText(str_Big.ToString, New FileInfo(F.DirectoryName & "\" & "Adaptor_Scores_Detailed" & R_postFix & F.Name & ".tsv"))

            Next
        Next
    End Sub

    ''' <summary>
    ''' From Adaptor modified Bam File
    ''' </summary>
    ''' <param name="sender"></param>
    ''' <param name="e"></param>
    Private Sub SetOrientationBasedOnBAMScoresToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles SetOrientationBasedOnBAMScoresToolStripMenuItem.Click
        Dim Files = Get_Bam_Files()
        If IsNothing(Files) = True Then Exit Sub
        Dim AD_Files = Szunyi.IO.Files.Filter.SelectFiles(Constants.Files.Xml)
        If IsNothing(AD_Files) = True Then Exit Sub
        For Each FIle In Files
            Dim Headers = Bam_Basic_IO.Headers.Get_Header(FIle)
            Dim Adaptors = Szunyi.BAM.Bam_Basic_IO.Headers.Get_Adaptors(Headers)
            For Each AD_File In AD_Files
                Dim res = ClassLibrary1.Szunyi.IO.XML.Deserialize(Of List(Of Szunyi.Transcipts.Adapter_Filtering))(ClassLibrary1.Szunyi.IO.Import.Text.ReadToEnd(AD_File))
                Dim ADs As New Dictionary(Of Szunyi.Transcipts.Adaptor_Pair, Szunyi.Transcipts.Adapter_Filtering)
                For Each r In res
                    Dim x = From h In Adaptors Where r.AP_Name = h.Name
                    ADs.Add(x.First, r)
                Next

                Dim Header As New System.Text.StringBuilder
                Header.Append("# ").Append(FIle.FullName).AppendLine()
                Header.Append(Szunyi.BAM.Bam_Basic_IO.Headers.Get_Comments(FIle))
                Header.Append(Szunyi.Transcipts.Adaptor_Aligner_Result_Manipulation.Get_Description(ADs))

                Headers.Comments.Add(Szunyi.Transcipts.Adaptor_Aligner_Result_Manipulation.Get_Description(ADs).Replace(vbCrLf, ","))
                Dim Sams = Szunyi.BAM.Bam_Basic_IO.Import.Parse(FIle)
                Dim Orgs = Szunyi.BAM.Bam_Basic_IO.Headers.Get_ReferenceSequences(FIle)


                For Each Org In Orgs
                    Dim cSams = From x In Sams Where x.RName = Org.Name

                    If cSams.Count > 0 Then
                        Dim out = Szunyi.mRNA.PolyA.Get_Dictionary_wListOfSams
                        For Each SAM In cSams
                            Dim M_Orientation = Szunyi.Transcipts.Orientation.Get_Orientation(SAM, ADs)
                            Szunyi.Transcipts.Orientation.Set_SAM_Flag(SAM, M_Orientation)

                            If M_Orientation = Szunyi.mRNA.PolyA_Type.polyA Or M_Orientation = Szunyi.mRNA.PolyA_Type.polyT Then
                                Dim For_PAS = Szunyi.Transcipts.Orientation.Get_Orientation(SAM, ADs, Szunyi.mRNA.PolyA_Type.For_PAS)
                                If For_PAS <> mRNA.PolyA_Type.None Then
                                    out(mRNA.PolyA_Type.For_PAS).Add(SAM)
                                End If
                                Dim For_TSS = Szunyi.Transcipts.Orientation.Get_Orientation(SAM, ADs, Szunyi.mRNA.PolyA_Type.For_TSS)
                                If For_TSS <> mRNA.PolyA_Type.None Then
                                    out(mRNA.PolyA_Type.For_TSS).Add(SAM)
                                End If
                            End If

                            '  out(Szunyi.mRNA.PolyA_Type.All).Add(SAM)
                            out(M_Orientation).Add(SAM)
                        Next
                        out(Szunyi.mRNA.PolyA_Type.polyAT).AddRange(out(Szunyi.mRNA.PolyA_Type.polyA))
                        out(Szunyi.mRNA.PolyA_Type.polyAT).AddRange(out(Szunyi.mRNA.PolyA_Type.polyT))



                        Dim Dir As New DirectoryInfo(FIle.DirectoryName & "\" & Org.Name & "_" & AD_File.Name)
                        Dir.Create()

                        Szunyi.mRNA.PolyA.Write_Orientation(out, Dir, FIle, Headers)
                    End If

                Next
            Next
        Next
    End Sub



    Private Sub PacBioPGToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles PacBioPGToolStripMenuItem.Click
        Dim FIles = Get_Bam_Files()
        For Each File In FIles
            Dim Header = Szunyi.BAM.Bam_Basic_IO.Headers.Get_Header(File)
            Dim PG = (From x In Header.RecordFields Where x.Typecode = "PG").First
            Dim X1 = From x In PG.Tags Where x.Tag = "CL"
            If X1.Count > 0 Then
                PG.Tags.Remove(X1.First)
            End If

            Dim SAMs = Szunyi.BAM.Bam_Basic_IO.Import.ParseAll(File)
            Szunyi.IO.Export.Export_Sam(SAMs, Header, New FileInfo(File.FullName & "MdHeader.sam"))
        Next
    End Sub

    Private Sub SelectMoreMappingsToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles SelectMoreMappingsToolStripMenuItem.Click
        Dim Files = Get_Bam_Files()
        If IsNothing(Files) = True Then Exit Sub

        For Each File In Files
            Dim Seqs As New List(Of Bio.ISequence)
            Dim Header = Szunyi.BAM.Bam_Basic_IO.Headers.Get_Header(File)
            Dim Sams = Szunyi.BAM.Bam_Basic_IO.Import.ParseAll(File)
            Dim Out As New List(Of Bio.IO.SAM.SAMAlignedSequence)
            For Each Item In Szunyi.BAM.GroupBy.ReadID(Sams)
                If Item.Count > 1 Then
                    If Szunyi.BAM.SAM_Manipulation.QuerySequnce.Get_Five_S(Item.First).Count > 200 AndAlso Szunyi.BAM.SAM_Manipulation.QuerySequnce.Get_Three_S(Item.Last).Count > 200 Then
                        Out.AddRange(Item)
                        Seqs.Add(Item.First.QuerySequence)
                    ElseIf Szunyi.BAM.SAM_Manipulation.QuerySequnce.Get_Five_S(Item.Last).Count > 200 AndAlso Szunyi.BAM.SAM_Manipulation.QuerySequnce.Get_Three_S(Item.First).Count > 200 Then
                    End If

                End If
            Next
            Szunyi.IO.Export.SaveSequencesToSingleFasta(Seqs, New FileInfo(File.FullName & "_e" & Seqs.Count & ".fa"))
        Next

    End Sub



    Private Sub ReadLengthDistributionsToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles ReadLengthDistributionsToolStripMenuItem.Click
        Dim Files = Szunyi.IO.Files.Filter.SelectFiles(Szunyi.Constants.Files.SAM_BAM)
        Dim bins = MyInputBox.GetIntegers("Read Length bins separated by space")
        ' Szunyi.BAM.Convert.Table.Aligned_Length_Distribution(Files, bins)
        Szunyi.BAM.Convert.Table.All_Length_Distributions(Files, bins)
    End Sub

    Private Sub SitesOldToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles SitesOldToolStripMenuItem.Click
        Dim Seqs = Me.GetSeqsFromSelectedList(True)
        If IsNothing(Seqs) = True Then Exit Sub
        Dim cSeqs = Szunyi.Sequences.SequenceManipulation.Common.CloneSeqs(Seqs)
        Dim polyA_Motifs = Split("AAAAAG,AAGAAA,AATAAA,AATACA,AATAGA,AATATA,ACTAAA,AGTAAA,ATTAAA,CATAAA,GATAAA,TATAAA", ",")
        Dim PolyA_Motifs_rev = Split("CTTTTT,TTTCTT,TTTATT,TGTATT,TCTATT,TATATT,TTTAGT,TTTACT,TTTAAT,TTTATG,TTTATC,TTTATA", ",")
        '    polyA_Motifs = Split("AATGAA,AATCAA,AATTAA,AACAAA,AAAAAA,AATAAT,AATAAC,AATAAG", ",")
        '       PolyA_Motifs_rev = Split("TTCATT,TTGCTT,TTAATT,TTTGTT,TTTTTT,ATTATT,GTTATT,CTTATT", ",")
        Dim Count As Integer = 0
        For Each Motif In polyA_Motifs
            For Each Seq In cSeqs
                Dim MC = Szunyi.Text.Regexp.Get_Motifs(Seq, Motif)
                For I1 = 0 To MC.Count - 1
                    Dim Item = MC.Item(I1)
                    Dim l As New FeatureItem(StandardFeatureKeys.PolyASite, Szunyi.Location.Common.Get_Location(Item.Index + 1 & ".." & Item.Index + Motif.Length))
                    Szunyi.Features.FeatureManipulation.GenBankMetaDataManipulation.AddFeature(Seq, l)
                    Count += 1
                Next
            Next
        Next
        For Each Motif In PolyA_Motifs_rev
            For Each Seq In cSeqs
                Dim MC = Szunyi.Text.Regexp.Get_Motifs(Seq, Motif)
                For I1 = 0 To MC.Count - 1
                    Dim Item = MC.Item(I1)
                    Dim l As New FeatureItem(StandardFeatureKeys.PolyASite, Szunyi.Location.Common.Get_Location("complement(" & Item.Index + 1 & ".." & Item.Index + Motif.Length & ")"))
                    Szunyi.Features.FeatureManipulation.GenBankMetaDataManipulation.AddFeature(Seq, l)
                    Count += 1
                Next
            Next
        Next
        Szunyi.IO.Export.SaveSequencesToSingleGenBank(cSeqs)

    End Sub

    Private Sub SignalsOldToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles SignalsOldToolStripMenuItem.Click

        Dim Seqs = Me.GetSeqsFromSelectedList(True)
        If IsNothing(Seqs) = True Then Exit Sub
        Dim cSeqs = Szunyi.Sequences.SequenceManipulation.Common.CloneSeqs(Seqs)

        Dim AllFeatureTpye = Szunyi.Features.FeatureManipulation.Common.Get_Feature_Types(cSeqs)

        Dim f1 As New CheckBoxForStringsFull(AllFeatureTpye, -1)
        If f1.ShowDialog <> DialogResult.OK Then Exit Sub

        Dim mRNAs = Szunyi.Features.FeatureManipulation.GetFeaturesByType.GetFeturesByTypesFromSeq(Seqs.First, f1.SelectedStrings)
        Dim PolyAs = Szunyi.Features.FeatureManipulation.Annotate.Get_PolyA_Signals(Seqs.First, 50, mRNAs)

        Szunyi.Features.FeatureManipulation.GenBankMetaDataManipulation.AddFeatures(cSeqs.First, PolyAs)

        Szunyi.IO.Export.SaveSequencesToSingleGenBank(cSeqs.First)
    End Sub

    Private Sub FromGenomicPositionsToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles FromGenomicPositionsToolStripMenuItem.Click
        Dim Seqs = Me.GetSeqsFromSelectedList(True)
        If IsNothing(Seqs) = True Then Exit Sub
        For Each Seq In Seqs
            Dim str As New System.Text.StringBuilder
            Dim Feats As New List(Of FeatureItem)
            For i1 As Integer = 0 To Seq.Count - 2
                For i2 As Integer = i1 To Seq.Count - 1
                    If Seq(i2) = Bio.Alphabets.DNA.A Then

                    Else
                        If i2 <> i1 Then
                            Dim f As New FeatureItem(StandardFeatureKeys.PolyASignal, Szunyi.Location.Common.GetLocation(i2, i1, False))
                            f.Label = i2 - i1
                            i1 = i2
                            Feats.Add(f)
                            Exit For
                        End If
                    End If
                Next
            Next
            For i1 As Integer = 0 To Seq.Count - 2
                For i2 As Integer = i1 To Seq.Count - 1
                    If Seq(i2) = Bio.Alphabets.DNA.T Then

                    Else
                        If i2 <> i1 Then
                            Dim f As New FeatureItem(StandardFeatureKeys.PolyASignal, Szunyi.Location.Common.GetLocation(i2, i1, True))
                            f.Label = i2 - i1
                            i1 = i2
                            Feats.Add(f)
                            Exit For
                        End If
                    End If
                Next
            Next
            Dim gr = From x In Feats Group By x.Label Into Group

            For Each G In gr
                For Each Item In G.Group
                    str.Append(G.Label).Append(vbTab)
                    str.Append(Szunyi.Location.Common.GetLocationStringTSS_PAS_Strand_Tab(Item.Location)).Append(vbTab)
                    Dim x = Szunyi.DNA.PA.Get_PolyA_Signal(Seq, Item, 50, -22)
                    Dim Best = x.Get_Best
                    str.Append(Best.ToString).AppendLine()
                Next
            Next
            Clipboard.SetText(str.ToString)
            Dim jk As Int16 = 43
        Next

    End Sub
    Private Sub FinalNowToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles FinalNowToolStripMenuItem.Click
        Dim Seqs = Me.GetSeqsFromSelectedList(True)
        If IsNothing(Seqs) = True Then Exit Sub

        Dim AllFeatureTpye = Szunyi.Features.FeatureManipulation.Common.Get_Feature_Types(Seqs)

        Dim f1 As New CheckBoxForStringsFull(AllFeatureTpye, -1)
        If f1.ShowDialog <> DialogResult.OK Then Exit Sub

        Dim str As New System.Text.StringBuilder
        Dim Header = Split("ID,strand,Seq,mRNA Location,PolyA Signal Sequence,Absolute Position,Relative Position,Nof A From Beginning,Nof consecutive A,Percent of A,Nof T From Beginning,Nof consecutive T,Percent of T", ",")
        str.Append(Szunyi.Text.General.GetText(Header, vbTab)).AppendLine()
        For Each Seq In Seqs
            Dim Feats = Szunyi.Features.FeatureManipulation.GetFeaturesByType.GetFeturesByTypesFromSeq(Seqs.First, f1.SelectedStrings)
            Dim PAs As New List(Of Szunyi.DNA.PolyA_Signal_Sites)
            For Each Feat In Feats
                Dim x = Szunyi.DNA.PA.Get_PolyA_Signal(Seq, Feat, 50, -22)
                Dim Best = x.Get_Best
                PAs.Add(x)
                str.Append(Feat.Qualifiers(StandardQualifierNames.Label).First).Append(vbTab)
                If Feat.Location.IsComplementer = True Then
                    str.Append("-").Append(vbTab)
                    Dim pSeq = Seq.GetSubSequence(Feat.Location.LocationStart - 1, 10)
                    str.Append(pSeq.ConvertToString).Append(vbTab)
                Else
                    str.Append("+").Append(vbTab)
                    Dim pSeq = Seq.GetSubSequence(Feat.Location.LocationStart - 10, 10)
                    str.Append(pSeq.ConvertToString).Append(vbTab)
                End If

                str.Append(Szunyi.Location.Common.GetLocationString(Feat)).Append(vbTab)
                str.Append(Best.ToString).Append(vbTab)
                str.Append(Szunyi.mRNA.PolyA.False_polyA(Seqs.First, Feat.Location, 10, mRNA.False_PolyAT_Discovering.From_Beginning, 0, Bio.Alphabets.DNA.A)).Append(vbTab)
                str.Append(Szunyi.mRNA.PolyA.False_polyA(Seqs.First, Feat.Location, 10, mRNA.False_PolyAT_Discovering.Nof_Consecutive_AT, 0, Bio.Alphabets.DNA.A)).Append(vbTab)
                str.Append(Szunyi.mRNA.PolyA.False_polyA(Seqs.First, Feat.Location, 10, mRNA.False_PolyAT_Discovering.Percent_of_AT, 0, Bio.Alphabets.DNA.A)).Append(vbTab)
                str.Append(Szunyi.mRNA.PolyA.False_polyT(Seqs.First, Feat.Location, 10, mRNA.False_PolyAT_Discovering.From_Beginning, 0, Bio.Alphabets.DNA.T)).Append(vbTab)
                str.Append(Szunyi.mRNA.PolyA.False_polyT(Seqs.First, Feat.Location, 10, mRNA.False_PolyAT_Discovering.Nof_Consecutive_AT, 0, Bio.Alphabets.DNA.T)).Append(vbTab)
                str.Append(Szunyi.mRNA.PolyA.False_polyT(Seqs.First, Feat.Location, 10, mRNA.False_PolyAT_Discovering.Percent_of_AT, 0, Bio.Alphabets.DNA.T)).AppendLine()
            Next
            Clipboard.SetText(str.ToString)

        Next

    End Sub

    Private Sub DragonAgeToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles DragonAgeToolStripMenuItem.Click
        Dim Seqs = Me.GetSeqsFromSelectedList(True)
        If IsNothing(Seqs) = True Then Exit Sub
        Dim cSeqs = Szunyi.Sequences.SequenceManipulation.Common.CloneSeqs(Seqs)
        Dim File = Szunyi.IO.Files.Filter.SelectFile("Select File", Szunyi.Constants.Files.All_TAB_Like)
        If IsNothing(File) = True Then Exit Sub

        Dim AllFeatureTpye = Szunyi.Features.FeatureManipulation.Common.Get_Feature_Types(cSeqs)

        Dim f1 As New CheckBoxForStringsFull(AllFeatureTpye, -1)
        If f1.ShowDialog <> DialogResult.OK Then Exit Sub

        Dim mRNAs = Szunyi.Features.FeatureManipulation.GetFeaturesByType.GetFeturesByTypesFromSeq(Seqs.First, f1.SelectedStrings)
        Dim nofGood As Integer = 0
        For Each Line In Szunyi.IO.Import.Text.ParseNotFirst(File, 1)
            Dim s = Split(Line, vbTab)
            Dim pos As Integer = s(1)
            If s.First.StartsWith(">i-") Then
                pos = Seqs.First.Count - pos
                Dim Feats = From x In mRNAs Where x.Location.LocationStart <= pos And x.Location.LocationEnd > pos And x.Location.IsComplementer = True

                For Each Feat In Feats
                    If pos > Feat.Location.LocationStart And pos < Feat.Location.LocationStart + 50 Then
                        nofGood += 1
                        Dim x As New FeatureItem(StandardFeatureKeys.PolyASignal, "complement(" & pos + 1 & ".." & pos - 4 & ")")
                        Dim ls As New List(Of String)
                        ls.Add(s(2))
                        x.Qualifiers(StandardQualifierNames.Note) = ls
                        Szunyi.Features.FeatureManipulation.GenBankMetaDataManipulation.AddFeature(cSeqs.First, x)
                        Exit For
                    End If

                Next
            Else
                Dim Feats = From x In mRNAs Where x.Location.LocationStart <= s(1) And x.Location.LocationEnd > s(1) And x.Location.IsComplementer = False

                For Each Feat In Feats
                    If pos < Feat.Location.LocationEnd And pos > Feat.Location.LocationEnd - 50 Then
                        nofGood += 1
                        Dim x As New FeatureItem(StandardFeatureKeys.PolyASignal, pos & ".." & pos + 5)
                        Dim ls As New List(Of String)
                        ls.Add(s(2))
                        x.Qualifiers(StandardQualifierNames.Note) = ls
                        Szunyi.Features.FeatureManipulation.GenBankMetaDataManipulation.AddFeature(cSeqs.First, x)
                        Exit For
                    End If

                Next
            End If
        Next
        Szunyi.IO.Export.SaveSequencesToSingleGenBank(cSeqs)
    End Sub
#Region "Create Transcripts"


    Private Function get_max_diff(items As List(Of Integer)) As Integer
        Dim max As Integer = 0
        For i1 = 0 To items.Count - 2
            For i2 = i1 + 1 To items.Count - 1
                If System.Math.Abs(items(i1) - items(i1 + 1)) > max Then max = System.Math.Abs(items(i1) - items(i1 + 1))
            Next
        Next

        Return max
    End Function

    Private Sub GetHeaderFromANotherFIleToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles GetHeaderFromANotherFIleToolStripMenuItem.Click
        Dim Files = Get_Bam_Files("Select Ref Header")
        If IsNothing(Files) = True Then Exit Sub
        Dim H = Szunyi.BAM.Bam_Basic_IO.Headers.Get_Header(Files.First)
        Files = Get_Bam_Files("Select Files to Change Headers")
        If IsNothing(Files) = True Then Exit Sub
        For Each file In Files
            Dim nFile As New FileInfo(file.FullName & "_Md_header.sam")
            Dim header = Szunyi.BAM.Bam_Basic_IO.Headers.Get_Header(file)
            Using x As New Szunyi.BAM.Bam_Basic_IO.Export(nFile, header)
                For Each sam In Szunyi.BAM.Bam_Basic_IO.Import.Parse(file)
                    x.Write(sam)
                Next
            End Using

        Next
    End Sub

    Private Sub ChangeQNAmeToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles ChangeQNAmeToolStripMenuItem.Click
        Dim Files = Get_Bam_Files("Select Files to Change Headers")
        If IsNothing(Files) = True Then Exit Sub
        For Each file In Files
            Dim h = Szunyi.BAM.Bam_Basic_IO.Headers.Get_Header(file)
            Dim n As New FileInfo(file.FullName & "_RQ.sam")
            Using x As New Szunyi.BAM.Bam_Basic_IO.Export(n, h)
                For Each sam In Szunyi.BAM.Bam_Basic_IO.Import.Parse(file)
                    sam.RName = h.ReferenceSequences.First.Name
                    x.Write(sam)
                Next
            End Using

        Next
    End Sub

    Private Sub GetOrgnismsToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles GetOrgnismsToolStripMenuItem.Click
        Dim FIles = Szunyi.IO.Files.Filter.SelectFiles(Szunyi.Constants.Files.SAM_BAM, "Bam Files")
        If IsNothing(FIles) = True Then Exit Sub
        Dim org = Szunyi.BAM.Bam_Basic_IO.Headers.Get_ReferenceSequences(FIles)
        Dim Organims_Names = (From x In org Select x.Name).ToList
        If Organims_Names.Count <> 0 Then Clipboard.SetText(Szunyi.Text.General.GetText(Organims_Names))

    End Sub


#Region "Remove"
    Private Sub BadCigarToolStripMenuItem_Click_1(sender As Object, e As EventArgs) Handles BadCigarToolStripMenuItem.Click
        Dim Files = Get_Bam_Files()
        If IsNothing(Files) = True Then Exit Sub

        Szunyi.BAM.Filter_Split.Remove_Bad_Cigars(Files)
    End Sub


    Private Sub HCLongIntronicToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles HCLongIntronicToolStripMenuItem.Click
        Dim Files = Get_Bam_Files()
        If IsNothing(Files) = True Then Exit Sub
        Dim MaxIntronLength = MyInputBox.GetInteger("Enter Max Intron Length")
        Dim SoftClip = MyInputBox.GetInteger("Enter Soft Clip region Length")
        Szunyi.BAM.Filter_Split.Remove_HC_Duplicated_Max_Intron(Files, MaxIntronLength, SoftClip)

    End Sub

    Private Sub HCToolStripMenuItem1_Click(sender As Object, e As EventArgs) Handles HCToolStripMenuItem1.Click
        Dim Files = Get_Bam_Files()
        If IsNothing(Files) = True Then Exit Sub
        Dim s As String = Szunyi.BAM.Filter_Split.Remove_HC(Files, False)

    End Sub

    Private Sub DuplicatedSAMsByPositionToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles DuplicatedSAMsByPositionToolStripMenuItem.Click
        Dim Files = Get_Bam_Files()
        If IsNothing(Files) = True Then Exit Sub
        Dim s As String = Szunyi.BAM.Filter_Split.Remove_Duplicated_by_Position(Files, False)
    End Sub

    Private Sub SelectFilesContainNamesToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles SelectFilesContainNamesToolStripMenuItem.Click
        Dim f = Clipboard.GetText
        Dim f1 = Split(f, vbCrLf)
        Dim FIles = Szunyi.IO.Files.Filter.SelectFiles()
        Dim eXtensions = (From x In FIles Select x.Extension).Distinct.ToList
        Dim f2 As New CheckBoxForStringsFull(eXtensions, 1)
        If f2.ShowDialog = DialogResult.OK Then
            Dim str As New System.Text.StringBuilder
            For Each s In f1

                If s <> "" Then
                    Dim r = From x In FIles Where x.Extension = f2.SelectedStrings.First And x.Name.Contains(s)

                    If r.Count = 1 Then
                        str.Append(r.First.Name).Append(vbTab).Append(s).AppendLine()
                    ElseIf r.Count = 0 Then
                        str.Append(vbTab).Append(s).AppendLine()
                    Else
                        For Each r1 In r
                            str.Append(r1.Name).Append(vbTab).Append(s).Append(vbTab)
                        Next
                        str.AppendLine()
                    End If
                End If

            Next

            If str.Length > 0 Then Clipboard.SetText(str.ToString)
            Beep()

        End If

    End Sub
    Private Sub MD5ToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles MD5ToolStripMenuItem.Click
        Dim f = Clipboard.GetText
        Dim f1 = Split(f, vbCrLf)
        Dim FIles = Szunyi.IO.Files.Filter.SelectFiles()
        Dim str As New System.Text.StringBuilder
        For Each f2 In f1
            Dim c_Files = From x In FIles Where x.Name.Contains(".md5")


            For Each f3 In c_Files
                str.Append(f3.Name).AppendLine()

                str.Append(Szunyi.IO.Import.Text.ReadToEnd(f3).Replace(vbCrLf, "")).Append(vbTab)

            Next


        Next


        Dim kj As Int16 = 54
    End Sub

    Private Sub ByRGToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles ByRGToolStripMenuItem.Click
        Dim Seqs = Me.GetSeqsFromSelectedList(True)
        If IsNothing(Seqs) = True Then Exit Sub
        If Seqs.Count <> 1 Then Exit Sub
        Dim CDSs = Szunyi.Features.FeatureManipulation.GetFeaturesByType.GetFeturesByTypeFromSeq(Seqs.First, StandardFeatureKeys.CodingSequence)
        Dim Files = Get_Bam_Files()
        If IsNothing(Files) = True Then Exit Sub
        Dim Res As New Dictionary(Of String, Dictionary(Of String, List(Of Bio.IO.SAM.SAMAlignedSequence)))
        Dim Max_Intron = MyInputBox.GetInteger("Maximal Intron Size")
        For Each File In Files
            Dim r As New Dictionary(Of String, Dictionary(Of String, List(Of Bio.IO.SAM.SAMAlignedSequence)))
            Dim SAMs As New List(Of Bio.IO.SAM.SAMAlignedSequence)
            Dim Header = Szunyi.BAM.Bam_Basic_IO.Headers.Get_Header(File)
            For Each Feat In CDSs
                Dim s = Feat.Qualifiers(StandardQualifierNames.LocusTag).First & vbTab & Feat.Location.LocationStart
            Next

            Using NC As New Szunyi.BAM.Bam_Basic_IO.Export(Szunyi.IO.Files.Get_New_FileName.Append_Before_Extension(File, "_non_coding"), Header)
                Using Cistronic As New Szunyi.BAM.Bam_Basic_IO.Export(Szunyi.IO.Files.Get_New_FileName.Append_Before_Extension(File, "_Cistronic"), Header)
                    Using Mono_Cistronic As New Szunyi.BAM.Bam_Basic_IO.Export(Szunyi.IO.Files.Get_New_FileName.Append_Before_Extension(File, "_Mono_Cistronic"), Header)
                        For Each SAM In Szunyi.BAM.Bam_Basic_IO.Import.Parse(File)

                            If Szunyi.BAM.CIGAR.Get_Biggest_Intron_Length(SAM) < Max_Intron Then


                                Dim l = Szunyi.Location.Common.GetLocation(SAM)
                                Dim c_CDSs = From x In CDSs Where x.Location.LocationStart > l.LocationStart And
                                                       x.Location.LocationEnd <= l.LocationEnd

                                Dim Names = From x In c_CDSs Select x.Qualifiers(StandardQualifierNames.LocusTag).First

                                Select Case Names.Count
                                    Case 0
                                        NC.Write(SAM)
                                    Case 1
                                        Mono_Cistronic.Write(SAM)
                                    Case Else
                                        Dim Min = (From x In c_CDSs Select x.Location.LocationStart).Min
                                        Dim Max = (From x In c_CDSs Select x.Location.LocationEnd).Max

                                        Dim ori = (From x In c_CDSs Select x.Location.IsComplementer).Distinct.Count


                                        Dim c_Names = Names.ToList
                                        c_Names.Sort()
                                        Dim Tag = Szunyi.Text.General.GetText(c_Names.ToList, ",")
                                        Tag = Tag & vbTab & Min & vbTab & Max & vbTab
                                        If ori = 1 Then
                                            Tag = Tag & "Cistronic"
                                        Else
                                            Tag = Tag & "Complex"
                                        End If
                                        Tag = Tag & vbTab & l.IsComplementer
                                        Dim RD_ID = Szunyi.BAM.BAM_Optional_Filed_Manipulation.Get_Read_Group_ID(SAM)

                                        If r.ContainsKey(RD_ID) = False Then r.Add(RD_ID, New Dictionary(Of String, List(Of Bio.IO.SAM.SAMAlignedSequence)))

                                        If r(RD_ID).ContainsKey(Tag) = False Then
                                            r(RD_ID).Add(Tag, New List(Of Bio.IO.SAM.SAMAlignedSequence))
                                        End If

                                        r(RD_ID)(Tag).Add(SAM)
                                        Cistronic.Write(SAM)
                                End Select
                            End If
                        Next ' SAM
                    End Using
                End Using
            End Using
            Dim tmp As New Dictionary(Of String, List(Of Bio.IO.SAM.SAMAlignedSequence))
            For Each Item In r

                For Each sItem In Item.Value
                    If tmp.ContainsKey(sItem.Key) = False Then tmp.Add(sItem.Key, New List(Of Bio.IO.SAM.SAMAlignedSequence))
                    tmp(sItem.Key).AddRange(sItem.Value)
                Next
            Next
            For Each Item In tmp
                Dim Locis = Szunyi.Location.Common.GetLocation(Item.Value)
                Dim L = Szunyi.Location.Common.Create_Biggest(Locis)
                If Item.Key.Contains("Complex") Then

                    If Item.Value.Count = 1 Then
                        Dim f As New FeatureItem("C_Unique", L)
                        Szunyi.Features.FeatureManipulation.GenBankMetaDataManipulation.AddFeature(Seqs.First, f)
                    Else
                        Dim f As New FeatureItem("C_More", L)
                        Szunyi.Features.FeatureManipulation.GenBankMetaDataManipulation.AddFeature(Seqs.First, f)
                    End If
                Else
                    If Item.Value.Count = 1 Then
                        Dim f As New FeatureItem("P_Uniqe", L)
                        Szunyi.Features.FeatureManipulation.GenBankMetaDataManipulation.AddFeature(Seqs.First, f)
                    Else
                        Dim f As New FeatureItem("P_More", L)
                        Szunyi.Features.FeatureManipulation.GenBankMetaDataManipulation.AddFeature(Seqs.First, f)
                    End If
                End If

            Next
            Szunyi.IO.Export.SaveSequencesToSingleGenBank(Seqs.First)
            Dim txt = "#Max Intron Size" & Max_Intron & vbCrLf & Szunyi.Text.Dict.Merge(r)
            Clipboard.SetText(txt)
        Next ' FIle
    End Sub
    ''' <summary>
    ''' Parse All Reads, Find CDS Completly Covered By The Read Get LocusTags
    ''' Merge and Sort Into Dictionary
    ''' Merge Dictionaries and Save 
    ''' </summary>
    ''' <param name="sender"></param>
    ''' <param name="e"></param>
    Private Sub To1FileToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles To1FileToolStripMenuItem.Click
        Dim Seqs = Me.GetSeqsFromSelectedList(True)
        If IsNothing(Seqs) = True Then Exit Sub
        If Seqs.Count <> 1 Then Exit Sub
        Dim CDSs = Szunyi.Features.FeatureManipulation.GetFeaturesByType.GetFeturesByTypeFromSeq(Seqs.First, StandardFeatureKeys.CodingSequence)
        Dim Files = Get_Bam_Files()
        If IsNothing(Files) = True Then Exit Sub
        Dim Res As New Dictionary(Of FileInfo, Dictionary(Of String, List(Of Bio.IO.SAM.SAMAlignedSequence)))
        Dim Max_Intron = MyInputBox.GetInteger("Maximal Intron Size")
        For Each File In Files
            Dim SAMs As New List(Of Bio.IO.SAM.SAMAlignedSequence)
            Dim Header = Szunyi.BAM.Bam_Basic_IO.Headers.Get_Header(File)
            For Each Feat In CDSs
                Dim s = Feat.Qualifiers(StandardQualifierNames.LocusTag).First & vbTab & Feat.Location.LocationStart
            Next
            Res.Add(File, New Dictionary(Of String, List(Of Bio.IO.SAM.SAMAlignedSequence)))
            Using NC As New Szunyi.BAM.Bam_Basic_IO.Export(Szunyi.IO.Files.Get_New_FileName.Append_Before_Extension(File, "_non_coding"), Header)
                Using Cistronic As New Szunyi.BAM.Bam_Basic_IO.Export(Szunyi.IO.Files.Get_New_FileName.Append_Before_Extension(File, "_Cistronic"), Header)
                    Using Mono_Cistronic As New Szunyi.BAM.Bam_Basic_IO.Export(Szunyi.IO.Files.Get_New_FileName.Append_Before_Extension(File, "_Mono_Cistronic"), Header)
                        For Each SAM In Szunyi.BAM.Bam_Basic_IO.Import.Parse(File)

                            If Szunyi.BAM.CIGAR.Get_Biggest_Intron_Length(SAM) < Max_Intron Then


                                Dim l = Szunyi.Location.Common.GetLocation(SAM)
                                Dim c_CDSs = From x In CDSs Where x.Location.LocationStart > l.LocationStart And
                                                       x.Location.LocationEnd <= l.LocationEnd

                                Dim Names = From x In c_CDSs Select x.Qualifiers(StandardQualifierNames.LocusTag).First

                                Select Case Names.Count
                                    Case 0
                                        NC.Write(SAM)
                                    Case 1
                                        Mono_Cistronic.Write(SAM)
                                    Case Else
                                        Dim Min = (From x In c_CDSs Select x.Location.LocationStart).Min
                                        Dim Max = (From x In c_CDSs Select x.Location.LocationEnd).Max

                                        Dim ori = (From x In c_CDSs Select x.Location.IsComplementer).Distinct.Count


                                        Dim c_Names = Names.ToList
                                        c_Names.Sort()
                                        Dim Tag = Szunyi.Text.General.GetText(c_Names.ToList, ",")
                                        Tag = Tag & vbTab & Min & vbTab & Max & vbTab
                                        If ori = 1 Then
                                            Tag = Tag & "Cistronic"
                                        Else
                                            Tag = Tag & "Complex"
                                        End If
                                        Tag = Tag & vbTab & l.IsComplementer
                                        Dim RD_ID = Szunyi.BAM.BAM_Optional_Filed_Manipulation.Get_Read_Group_ID(SAM)


                                        If Res(File).ContainsKey(Tag) = False Then
                                            Res(File).Add(Tag, New List(Of Bio.IO.SAM.SAMAlignedSequence))
                                        End If

                                        Res(File)(Tag).Add(SAM)
                                        Cistronic.Write(SAM)
                                End Select
                            End If
                        Next ' SAM
                    End Using
                End Using
            End Using
            Dim tmp As New Dictionary(Of String, List(Of Bio.IO.SAM.SAMAlignedSequence))
            For Each Item In Res

                For Each sItem In Item.Value
                    If tmp.ContainsKey(sItem.Key) = False Then tmp.Add(sItem.Key, New List(Of Bio.IO.SAM.SAMAlignedSequence))
                    tmp(sItem.Key).AddRange(sItem.Value)
                Next
            Next
            For Each Item In tmp
                Dim Locis = Szunyi.Location.Common.GetLocation(Item.Value)
                Dim L = Szunyi.Location.Common.Create_Biggest(Locis)
                If Item.Key.Contains("Complex") Then

                    If Item.Value.Count = 1 Then
                        Dim f As New FeatureItem("C_Unique", L)
                        Szunyi.Features.FeatureManipulation.GenBankMetaDataManipulation.AddFeature(Seqs.First, f)
                    Else
                        Dim f As New FeatureItem("C_More", L)
                        Szunyi.Features.FeatureManipulation.GenBankMetaDataManipulation.AddFeature(Seqs.First, f)
                    End If
                Else
                    If Item.Value.Count = 1 Then
                        Dim f As New FeatureItem("P_Uniqe", L)
                        Szunyi.Features.FeatureManipulation.GenBankMetaDataManipulation.AddFeature(Seqs.First, f)
                    Else
                        Dim f As New FeatureItem("P_More", L)
                        Szunyi.Features.FeatureManipulation.GenBankMetaDataManipulation.AddFeature(Seqs.First, f)
                    End If
                End If

            Next
            '      Szunyi.IO.Export.SaveSequencesToSingleGenBank(Seqs.First)
            Dim txt = "#Max Intron Size" & Max_Intron & vbCrLf & Szunyi.Text.Dict.Merge(Res)
            Clipboard.SetText(txt)
        Next ' FIle

    End Sub

    Private Sub VCFToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles VCFToolStripMenuItem.Click
        Dim Files = Szunyi.IO.Files.Filter.SelectFiles
        If IsNothing(Files) = True Then Exit Sub
        For Each FIle In Files
            Dim str As New System.Text.StringBuilder
            Dim Index As Integer = 0
            For Each line In Szunyi.IO.Import.Text.Parse(FIle)
                Index += 1
                Select Case Index
                    Case 2
                        ' Do nothing
                    Case 5
                        Dim line1 = line.Replace("+", "")
                        If line1.Count + 1 <> line.Count Then
                            Dim kj As Int16 = 43
                        End If
                        str.Append(line1).AppendLine()
                    Case 8
                        Dim line1 = line.Replace("+", "")
                        If line1.Count + 1 <> line.Count Then
                            Dim kj As Int16 = 43
                        End If
                        str.Append(line1).AppendLine()
                    Case 7
                        Dim line1 = line.Replace("+", "")
                        If line1.Count + 1 <> line.Count Then
                            Dim kj As Int16 = 43
                        End If
                        str.Append(line1).AppendLine()
                    Case Else
                        str.Append(line).AppendLine()
                End Select

            Next
            str.Length -= 2
            Dim nFIle As New FileInfo(FIle.FullName & ".md.vcf")
            Szunyi.IO.Export.SaveText(str.ToString, nFIle)
        Next
    End Sub

    Private Sub CopyFilesEGAToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles CopyFilesEGAToolStripMenuItem.Click
        Dim Files = Szunyi.IO.Files.Filter.SelectFiles
        If IsNothing(Files) = True Then Exit Sub
        Dim dest = Szunyi.IO.Directory.Get_Folder
        If IsNothing(dest) = True Then Exit Sub
        Dim s = InputBox("Enter string to find separated by ,")

        Dim s1 = Split(s, ",")
        Dim ID = Clipboard.GetText
        Dim IDs = Split(ID, vbCrLf)
        For Each s In s1
            Dim C = From x In Files Where x.Name.Contains(s) = True

            For Each c1 In C
                For Each ID In IDs
                    If ID <> "" Then
                        If c1.Name.Contains(ID) = True Then
                            c1.MoveTo(dest.FullName & "\" & c1.Name)
                        End If
                    End If
                Next

            Next
        Next
        Beep()

    End Sub
    Private Sub TranscriptFromTSSAndPASToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles TranscriptFromTSSAndPASToolStripMenuItem.Click
        Dim Seqs = Me.GetSeqsFromSelectedList(True)
        If IsNothing(Seqs) = True Then Exit Sub
        Dim TSS = Select_Custom_Features(Seqs)
        If IsNothing(TSS) = True Then Exit Sub
        Dim PAS = Select_Custom_Features(Seqs)
        If IsNothing(PAS) = True Then Exit Sub
        For Each T In TSS
            For Each P In PAS
                If T.Location.IsComplementer = P.Location.IsComplementer Then
                    If System.Math.Abs(T.Location.LocationStart - P.Location.LocationStart) < 10000 Then
                        Dim l = Szunyi.Location.Common.GetLocation(T.Location.TSS, P.Location.PAS)
                        Dim f As New FeatureItem("tr", l)
                        Szunyi.Features.FeatureManipulation.GenBankMetaDataManipulation.AddFeature(Seqs.First, f)
                    End If
                End If
            Next
        Next
        Szunyi.IO.Export.SaveSequencesToSingleGenBank(Seqs.First)
        Dim kj As Int16 = 54
    End Sub


    Private Function Select_Custom_Features(Optional seqs As List(Of ISequence) = Nothing, Optional Sel_Features As List(Of String) = Nothing, Optional Title As String = "") As List(Of FeatureItem)
        If IsNothing(seqs) = True Then
            Dim FIles = Szunyi.IO.Files.Filter.SelectFiles(Constants.Files.GenBank)
            If IsNothing(FIles) = True Then Return New List(Of FeatureItem)
            seqs = Szunyi.IO.Import.Sequence.FromFiles(FIles)
        End If
        Dim AllFeatureTpye = Szunyi.Features.FeatureManipulation.Common.Get_Feature_Types(seqs)

        Dim f1 As New CheckBoxForStringsFull(AllFeatureTpye, -1, "Select Features " & Title, Sel_Features)
        If f1.ShowDialog <> DialogResult.OK Then Return Nothing

        Dim Feats = Szunyi.Features.FeatureManipulation.GetFeaturesByType.GetFeturesByTypesFromSeqs(seqs, f1.SelectedStrings)

        Return Feats
    End Function
    Private Function Select_Custom_Qulifiers(Optional Seqs As List(Of ISequence) = Nothing) As List(Of String)
        Dim dQuals As New List(Of String)
        If IsNothing(Seqs) = True Then
            Dim Feats = Szunyi.Features.FeatureManipulation.GetFeaturesByType.Get_All_Features(Seqs)
            Dim Quals = From x In Feats Select x.Qualifiers.Keys


            For Each Q In Quals
                For Each Key In Q
                    If dQuals.Contains(Key) = False Then dQuals.Add(Key)
                Next
            Next
        Else
            dQuals = Bio.IO.GenBank.StandardQualifierNames.All.ToList
        End If

        Dim x1 As New CheckBoxForStringsFull(dQuals, -1)
        If x1.ShowDialog = DialogResult.OK Then
            Return x1.SelectedStrings
        Else
            Return dQuals
        End If
    End Function
    Private Sub PASFindPoissonDistributionPolyAFromFeaturesToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles PASFindPoissonDistributionPolyAFromFeaturesToolStripMenuItem.Click

    End Sub

    Private Sub TSSFindPoissonDistributionFromFeaturesToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles TSSFindPoissonDistributionFromFeaturesToolStripMenuItem.Click
        Dim Seqs = Me.GetSeqsFromSelectedList(True)
        If IsNothing(Seqs) = True Then Exit Sub
        Dim Feats = Select_Custom_Features(Seqs)
        If IsNothing(Feats) = True Then Exit Sub
        Dim Bam_Files = Get_Bam_Files()
        If IsNothing(Bam_Files) = True Then Exit Sub
        Dim LocalWidth As Integer = MyInputBox.GetInteger("Enter Local Width +-")
        Dim Width As Integer = MyInputBox.GetInteger("Enter Width +-")
        Dim P_Threshold = 0.05
        Dim Dist_Names = Szunyi.Util_Helpers.Get_All_Enum_Names(Of Szunyi.Stat.Distributions)(Szunyi.Stat.Distributions.Poisson)
        Dim f1 As New CheckBoxForStringsFull(Dist_Names, -1)
        If f1.ShowDialog = DialogResult.OK Then
            Dim Enums = Szunyi.Util_Helpers.Get_Enum_Value(Of Szunyi.Stat.Distributions)(f1.SelectedStrings)
            For Each FIle In Bam_Files
                Dim SAMs = Szunyi.BAM.Bam_Basic_IO.Import.ParseAll(FIle)
                Dim BLs = Szunyi.Location.Common.GetLocation(SAMs)
                Dim Res = Szunyi.BAM.Convert.Table.TSS(Feats, Width, LocalWidth, P_Threshold, Seqs, FIle, BLs, Enums)
                Dim nFIle As New FileInfo(FIle.FullName & "_TSS.tsv")
                Szunyi.IO.Export.SaveText(Res, nFIle)


            Next
        End If
    End Sub
    'TSS Full Analysis
    Private Sub TSSFindPoissonDIstributionToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles TSSFindPoissonDIstributionToolStripMenuItem.Click
        Dim Bam_Files = Get_Bam_Files()
        If IsNothing(Bam_Files) = True Then Exit Sub
        Dim Seq_File = Szunyi.IO.Files.Filter.SelectFile(Constants.Files.GenBank)
        If IsNothing(Seq_File) = True Then Exit Sub
        Dim Seqs = Szunyi.IO.Import.Sequence.FromFile(Seq_File)
        Dim LocalWidth As Integer = MyInputBox.GetInteger("Enter Local Width +-")
        Dim Width As Integer = MyInputBox.GetInteger("Enter Width +-")
        Dim P_Threshold = 0.05
        Dim res As New Dictionary(Of FileInfo, List(Of Szunyi.Stat.Distribution_Result))
        Dim Dist_Names = Szunyi.Util_Helpers.Get_All_Enum_Names(Of Szunyi.Stat.Distributions)(Szunyi.Stat.Distributions.Poisson)
        Dim f1 As New CheckBoxForStringsFull(Dist_Names, -1)
        If f1.ShowDialog = DialogResult.OK Then
            Dim Enums = Szunyi.Util_Helpers.Get_Enum_Value(Of Szunyi.Stat.Distributions)(f1.SelectedStrings)
            For Each FIle In Bam_Files

                Dim x1 As New Szunyi.Stat.Manager(LocalWidth, Width, Sort_Locations_By.TSS, Seqs.First, "TSS", FIle, Enums, P_Threshold)

                res.Add(FIle, x1.Calculate)
                Dim t = x1.Get_Header & x1.Get_Text
                Dim nFIle = Szunyi.IO.Files.Get_New_FileName.Append_Before_Extension_wNew_Extension(FIle, "_TSS.tsv")
                Szunyi.IO.Export.SaveText(t, nFIle)

                Dim t1 = x1.Site.Get_All_Count
                Dim nFIle2 = Szunyi.IO.Files.Get_New_FileName.Append_Before_Extension_wNew_Extension(FIle, "_TSS_Counts.tsv")
                Szunyi.IO.Export.SaveText(t1, nFIle2)
            Next

        End If
        Dim Distinct = Szunyi.Transcipts.Analysis.Merge_Poisson(res, 0)
        Szunyi.Features.FeatureManipulation.Annotate.Poisson(Seqs.First, Distinct, "TSS")
        Dim GBK_FIle = Szunyi.IO.Files.Get_New_FileName.Append_Before_Extension_wNew_Extension(Seq_File, "_TSS.gbk")
        Szunyi.IO.Export.SaveSequencesToSingleGenBank(Seqs.First, GBK_FIle, False)
        Dim j As Int16 = 54
    End Sub
    Private Sub ByPreDefinedTSSTESLocFIleToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles ByPreDefinedTSSTESLocFIleToolStripMenuItem.Click

        Dim x1 As New FolderSelectDialog
        If x1.ShowDialog <> DialogResult.OK Then Exit Sub
        Dim OutDIr As New DirectoryInfo(x1.FolderNames.First)

        Dim Seq_File = Szunyi.IO.Files.Filter.SelectFile(Constants.Files.GenBank)
        If IsNothing(Seq_File) = True Then Exit Sub

        '  Seqs = Szunyi.Features.FeatureManipulation.Common.Correct_Location(Seqs)
        Dim TSS_plus_minus = MyInputBox.GetInteger("Set TSS +-", 10)
        Dim TES_plus_minus = MyInputBox.GetInteger("Set TES +-", 10)
        Dim Intron_plus_minus = MyInputBox.GetInteger("Set Intron +-", 2)
        Dim nofAA = MyInputBox.GetInteger("Set minimal nof AA in .5 variant ", 20)

        Dim Seqs = Szunyi.IO.Import.Sequence.FromFile(Seq_File)
        Dim Sel_Qulifier = Select_Quilifiers(Seqs)
        Dim TSS_L As New List(Of String)
        TSS_L.Add("TSS")
        Dim TES_L As New List(Of String)
        TES_L.Add("TES")
        Dim Intron_L As New List(Of String)
        Intron_L.Add("Intron")
        Dim TSSs = Select_Custom_Features(Seqs, TSS_L, "for TSS")
        If TSSs.Count = 0 Then Exit Sub

        Dim BL_TSSs = Szunyi.Location.Basic_Location_Manipulation.Get_Basic_LocationS(TSSs)
        Dim TESs = Select_Custom_Features(Seqs, TES_L, "for TES")
        If TESs.Count = 0 Then Exit Sub
        Dim BL_TESs = Szunyi.Location.Basic_Location_Manipulation.Get_Basic_LocationS(TESs)

        Dim Introns = Select_Custom_Features(Seqs, Intron_L, "for Intron")
        If Introns.Count = 0 Then Exit Sub
        Dim BL_Introns = Szunyi.Location.Basic_Location_Manipulation.Get_Basic_LocationS(Introns)

        Dim TSS_FInder = New Szunyi.Location.Basic_Location_Finder(BL_TSSs, False)
        Dim TES_FInder = New Szunyi.Location.Basic_Location_Finder(BL_TESs, False)
        Dim Intron_FInder = New Szunyi.Location.Basic_Location_Finder(BL_Introns, False)


        Dim Bam_FIles = Szunyi.IO.Files.Filter.SelectFiles(Szunyi.Constants.Files.Loc_SAM_BAM)
        If IsNothing(Bam_FIles) = True Then Exit Sub
        Dim GBKs As New List(Of Bio.ISequence)
        For Each Bam_File In Bam_FIles
            Dim cSeqs = Szunyi.Sequences.SequenceManipulation.Common.CloneSeqs(Seqs)
            Szunyi.Features.FeatureManipulation.Common.Correct_Location(cSeqs)
            Dim BL_Sams2 As IEnumerable(Of Szunyi.Location.Basic_Location)

            If Bam_File.Extension = ".loc" Then
                BL_Sams2 = Szunyi.IO.Import.Locations.Parse_Into_Basic_Locations(Bam_File)
            Else
                BL_Sams2 = Szunyi.BAM.Bam_Basic_IO.Import.Parse_Into_Basic_Locations(Bam_File)
            End If

            Dim x2 As New Szunyi.Transcipts.Analysis()
            Dim TRs2 = Szunyi.Transcipts.Find_Transcripts.Get_Final_Transcripts(TSS_FInder,
                                        TES_FInder,
                                        Intron_FInder,
                                        BL_Introns,
                                        BL_Sams2,
                                        TSS_plus_minus,
                                        TES_plus_minus,
                                       Intron_plus_minus)




            Dim Transcripts = Szunyi.Transcipts.Find_Transcripts.Create_Transcripts(TSS_FInder, TES_FInder, BL_Introns, TRs2)
            Szunyi.Features.FeatureManipulation.GenBankMetaDataManipulation.AddFeatures(cSeqs.First, Transcripts)

            Dim CDSs2 = Szunyi.Features.FeatureManipulation.GetFeaturesByType.GetFeturesByTypeFromSeq(cSeqs.First, StandardFeatureKeys.CodingSequence)
            Dim Real_Named_TRs2 = Szunyi.Transcipts.Set_Transcipt_Names.Set_Real_Names(Transcripts, CDSs2, cSeqs.First, Sel_Qulifier.First, nofAA)
            Szunyi.Transcipts.Set_Transcipt_Names.Set_L_AT_Variants(Real_Named_TRs2)
            Szunyi.Transcipts.Set_Transcipt_Names.Set_Intron_Variants(Real_Named_TRs2)
            Dim nTRs2 = Szunyi.Features.FeatureManipulation.Key.ReName_Keys(Real_Named_TRs2, "nTr")
            Szunyi.Features.FeatureManipulation.GenBankMetaDataManipulation.AddFeatures(cSeqs.First, nTRs2)
            Szunyi.Features.FeatureManipulation.GenBankMetaDataManipulation.AddFeatures(cSeqs.First, Real_Named_TRs2)
            Szunyi.IO.Export.SaveSequencesToSingleGenBank(cSeqs.First, New FileInfo(Bam_File.FullName & "TSS-" & TSS_plus_minus & "_TES-" & TES_plus_minus & "_intron-" & Intron_plus_minus & ".gb"))
            cSeqs.First.ID = Bam_File.Name
            GBKs.Add(cSeqs.First)
        Next


        Dim TRSIII As List(Of FeatureItem) = Szunyi.Transcipts.Analysis.Set_Notes(GBKs)
        Dim cSeqs1 = Szunyi.Sequences.SequenceManipulation.Common.CloneSeqs(Seqs)
        '     Szunyi.Features.FeatureManipulation.GenBankMetaDataManipulation.AddFeatures(cSeqs1.First, TRSIII)
        Dim CDSs = Szunyi.Features.FeatureManipulation.GetFeaturesByType.GetFeturesByTypeFromSeq(cSeqs1.First, StandardFeatureKeys.CodingSequence)

        Dim Real_Named_TRs = Szunyi.Transcipts.Set_Transcipt_Names.Set_Real_Names(TRSIII, CDSs, cSeqs1.First, Sel_Qulifier.First, nofAA)
        Szunyi.Transcipts.Set_Transcipt_Names.Set_L_AT_Variants(Real_Named_TRs)
        Szunyi.Transcipts.Set_Transcipt_Names.Set_Intron_Variants(Real_Named_TRs)
        Dim nTRs = Szunyi.Features.FeatureManipulation.Key.ReName_Keys(Real_Named_TRs, "nTr")
        Szunyi.Features.FeatureManipulation.GenBankMetaDataManipulation.AddFeatures(cSeqs1.First, nTRs)
        Szunyi.Features.FeatureManipulation.GenBankMetaDataManipulation.AddFeatures(cSeqs1.First, Real_Named_TRs)
        Szunyi.IO.Export.SaveSequencesToSingleGenBank(cSeqs1.First, New FileInfo(Bam_FIles.First.DirectoryName & "\All_TSS-" & TSS_plus_minus & "_TES-" & TES_plus_minus & "_intron-" & Intron_plus_minus & ".gb"))

    End Sub

    Private Sub TSSPASFromDirectoriesToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles TSSPASFromDirectoriesToolStripMenuItem.Click
        Dim x As New FolderSelectDialog

        If x.ShowDialog = DialogResult.OK Then
            Dim Seq_File = Szunyi.IO.Files.Filter.SelectFile(Constants.Files.GenBank)
            If IsNothing(Seq_File) = True Then Exit Sub
            Dim Seqs = Szunyi.IO.Import.Sequence.FromFile(Seq_File)
            Dim cSeqs = Szunyi.Sequences.SequenceManipulation.Common.CloneSeqs(Seqs)

            Dim Intron As New Szunyi.BAM.Settings.Intron
            Dim c As New Controls.Set_Console_Properties(Intron)
            If c.ShowDialog <> DialogResult.OK Then Exit Sub

            Dim TSS_PAS_TR As New Szunyi.BAM.Settings.TSS_PAS_TR
            Dim c2 As New Controls.Set_Console_Properties(TSS_PAS_TR)
            If c2.ShowDialog <> DialogResult.OK Then Exit Sub

            Dim CountBy As New Szunyi.BAM.Settings.Count_As
            Dim c3 As New Controls.Set_Console_Properties(CountBy)
            If c3.ShowDialog <> DialogResult.OK Then Exit Sub

            Dim res As New Dictionary(Of FileInfo, List(Of Szunyi.Stat.Distribution_Result))
            Dim Dirs = Szunyi.IO.Directory.Get_Directories(x.FolderNames.ToList)

            Dim All_TSSs As New List(Of Szunyi.Location.Basic_Location)
            Dim All_PASs As New List(Of Szunyi.Location.Basic_Location)
            Dim All_PAs As New List(Of Szunyi.Location.Basic_Location)
            cSeqs = Szunyi.Sequences.SequenceManipulation.Common.CloneSeqs(Seqs)
            For Each D In Dirs
                Dim k As New Szunyi.Transcipts.Analysis(D, cSeqs.First, Intron, TSS_PAS_TR, CountBy)
                k.DoIt()
                All_PAs.AddRange(k.PAs)
                All_TSSs.AddRange(k.TSSs)
                All_PASs.AddRange(k.PASs)
            Next
            Dim ALL As New Szunyi.Transcipts.Analysis(Dirs.First.Parent, cSeqs.First, Intron, TSS_PAS_TR, CountBy)
            ALL.TSSs = All_TSSs
            ALL.PAs = All_PAs
            ALL.PASs = All_PASs
            ALL.DoIt()
            Dim jk As Int16 = 54
        End If ' Directories


    End Sub
    ' PAS Analysis
    Private Sub FindPoissonDistributionPolyAToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles FindPoissonDistributionPolyAToolStripMenuItem.Click
        Dim Bam_Files = Get_Bam_Files()
        If IsNothing(Bam_Files) = True Then Exit Sub
        Dim Seq_File = Szunyi.IO.Files.Filter.SelectFile(Constants.Files.GenBank)
        If IsNothing(Seq_File) = True Then Exit Sub
        Dim Seqs = Szunyi.IO.Import.Sequence.FromFile(Seq_File)
        Dim LocalWidth As Integer = MyInputBox.GetInteger("Enter Local Width +-")
        Dim Width As Integer = MyInputBox.GetInteger("Enter Width +-")
        Dim P_Threshold = 0.05
        Dim res As New Dictionary(Of FileInfo, List(Of Szunyi.Stat.Distribution_Result))

        Dim Dist_Names = Szunyi.Util_Helpers.Get_All_Enum_Names(Of Szunyi.Stat.Distributions)(Szunyi.Stat.Distributions.Poisson)
        Dim f1 As New CheckBoxForStringsFull(Dist_Names, -1)
        If f1.ShowDialog = DialogResult.OK Then
            Dim Enums = Szunyi.Util_Helpers.Get_Enum_Value(Of Szunyi.Stat.Distributions)(f1.SelectedStrings)
            For Each FIle In Bam_Files
                Dim x1 As New Szunyi.Stat.Manager(LocalWidth, Width, Sort_Locations_By.PAS, Seqs.First, "PAS", FIle, Enums, P_Threshold)

                res.Add(FIle, x1.Calculate)
                Dim t = x1.Get_Header & x1.Get_Text
                x1.Add_Features(Seqs.First)
                Dim nFIle = Szunyi.IO.Files.Get_New_FileName.Append_Before_Extension_wNew_Extension(FIle, "_PAS.tsv")
                Szunyi.IO.Export.SaveText(t, nFIle)

                Dim t1 = x1.Site.Get_All_Count
                Dim nFIle2 = Szunyi.IO.Files.Get_New_FileName.Append_Before_Extension_wNew_Extension(FIle, "_PAS_Counts.tsv")
                Szunyi.IO.Export.SaveText(t1, nFIle2)

            Next
            Dim Distinct = Szunyi.Transcipts.Analysis.Merge_Poisson(res, 0)
            Szunyi.Features.FeatureManipulation.Annotate.Poisson(Seqs.First, Distinct, "PAS")
            Dim GBK_FIle = Szunyi.IO.Files.Get_New_FileName.Append_Before_Extension_wNew_Extension(Seq_File, "_PAS.gbk")
            Szunyi.IO.Export.SaveSequencesToSingleGenBank(Seqs.First, GBK_FIle, False)
        End If
    End Sub
#Region "False PAS"
    Private Sub FromBAMAndGenomicToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles FromBAMAndGenomicToolStripMenuItem.Click
        Dim Seqs = Me.GetSeqsFromSelectedList(True)
        If IsNothing(Seqs) = True Then Exit Sub
        Dim Bam_Files = Get_Bam_Files()
        If IsNothing(Bam_Files) = True Then Exit Sub
        Dim Local_Width As Integer = MyInputBox.GetInteger("Enter Local Width +-")
        Dim Width As Integer = MyInputBox.GetInteger("Enter Width +-")
        Dim P_Threshold = MyInputBox.GetDouble("Enter P-Value")
        Dim Dist_Names = Szunyi.Util_Helpers.Get_All_Enum_Names(Of Szunyi.Stat.Distributions)(Szunyi.Stat.Distributions.Poisson)
        Dim f1 As New CheckBoxForStringsFull(Dist_Names, -1)
        If f1.ShowDialog = DialogResult.OK Then
            Dim Enums = Szunyi.Util_Helpers.Get_Enum_Value(Of Szunyi.Stat.Distributions)(f1.SelectedStrings)
            For Each FIle In Bam_Files
                Dim SAMs = Szunyi.BAM.Bam_Basic_IO.Import.ParseAll(FIle)
                Dim BLs = Szunyi.Location.Common.GetLocation(SAMs)
                Dim Merged = Szunyi.Location.Merging.GroupBy(BLs, Sort_Locations_By.PAS, 1, True)

                Dim Res2 = Szunyi.BAM.Convert.Table.False_PAS(Merged, Width, Local_Width, P_Threshold, Seqs, FIle, SAMs, BLs, Enums)
                Dim nFIle2 As New FileInfo(FIle.FullName & "_PAS.tsv")
                Szunyi.IO.Export.SaveText(Res2, nFIle2)
            Next
        End If
    End Sub



    Private Sub FromFeaturesToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles FromFeaturesToolStripMenuItem.Click
        Dim Seqs = Me.GetSeqsFromSelectedList(True)
        If IsNothing(Seqs) = True Then Exit Sub
        Dim Feats = Select_Custom_Features(Seqs)
        If IsNothing(Feats) = True Then Exit Sub
        Dim Bam_Files = Get_Bam_Files()
        If IsNothing(Bam_Files) = True Then Exit Sub
        Dim LocalWidth As Integer = MyInputBox.GetInteger("Enter Local Width +-")
        Dim Width As Integer = MyInputBox.GetInteger("Enter Width +-")
        Dim P_Threshold = 0.05
        Dim Dist_Names = Szunyi.Util_Helpers.Get_All_Enum_Names(Of Szunyi.Stat.Distributions)(Szunyi.Stat.Distributions.Poisson)
        Dim f1 As New CheckBoxForStringsFull(Dist_Names, -1)
        If f1.ShowDialog = DialogResult.OK Then
            Dim Enums = Szunyi.Util_Helpers.Get_Enum_Value(Of Szunyi.Stat.Distributions)(f1.SelectedStrings)
            For Each FIle In Bam_Files
                Dim SAMs = Szunyi.BAM.Bam_Basic_IO.Import.ParseAll(FIle)
                Dim BLs = Szunyi.Location.Common.GetLocation(SAMs)
                Dim Res = Szunyi.BAM.Convert.Table.TSS(Feats, Width, LocalWidth, P_Threshold, Seqs, FIle, BLs, Enums)
                Dim nFIle As New FileInfo(FIle.FullName & "_TSS.tsv")
                Szunyi.IO.Export.SaveText(Res, nFIle)

                Dim Res2 = Szunyi.BAM.Convert.Table.PAS(Feats, Width, LocalWidth, P_Threshold, Seqs, FIle, SAMs, BLs, Enums)
                Dim nFIle2 As New FileInfo(FIle.FullName & "_PAS.tsv")
                Szunyi.IO.Export.SaveText(Res2, nFIle2)
                Dim kj As Int16 = 54
            Next
        End If
    End Sub

    Private Sub ByXMLToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles ByXMLToolStripMenuItem.Click
        Dim Bam_Files = Get_Bam_Files()
        If IsNothing(Bam_Files) = True Then Exit Sub
        Dim AD_Files = Szunyi.IO.Files.Filter.SelectFiles(Constants.Files.Xml)
        If IsNothing(AD_Files) = True Then Exit Sub
        For Each FIle In Bam_Files
            Dim HeaderPassed = Bam_Basic_IO.Headers.Get_Header(FIle)
            Dim HeaderNotPassed = Bam_Basic_IO.Headers.Get_Header(FIle)

            Dim Adaptors = Szunyi.BAM.Bam_Basic_IO.Headers.Get_Adaptors(HeaderPassed)
            For Each AD_File In AD_Files
                Dim res = ClassLibrary1.Szunyi.IO.XML.Deserialize(Of List(Of Szunyi.Transcipts.Adapter_Filtering))(ClassLibrary1.Szunyi.IO.Import.Text.ReadToEnd(AD_File))
                Dim ADs As New Dictionary(Of Szunyi.Transcipts.Adaptor_Pair, Szunyi.Transcipts.Adapter_Filtering)
                For Each r In res
                    Dim x = From h In Adaptors Where r.AP_Name = h.Name
                    ADs.Add(x.First, r)
                Next
                For Each AD In AD_Files
                    HeaderPassed.Comments.Add("#Passed By " & AD.FullName)
                    HeaderNotPassed.Comments.Add("#Not Passed By " & AD.FullName)
                Next

                Dim Passed As FileInfo = Szunyi.IO.Files.Get_New_FileName.Append_Before_Extension(FIle, "_Passed_" & AD_File.Name)
                Dim NotPassed As FileInfo = Szunyi.IO.Files.Get_New_FileName.Append_Before_Extension(FIle, "_Not_Passed_" & AD_File.Name)

                Using SAM_w_Passed As New Szunyi.BAM.Bam_Basic_IO.Export(Passed, HeaderPassed)
                    Using SAM_w_NotPassed As New Szunyi.BAM.Bam_Basic_IO.Export(NotPassed, HeaderNotPassed)

                        For Each SAM In Szunyi.BAM.Bam_Basic_IO.Import.Parse(FIle)
                            If Szunyi.BAM.Filter_Split.IsGood_By_Position(SAM, ADs) Then
                                SAM_w_Passed.Write(SAM)
                            Else
                                SAM_w_NotPassed.Write(SAM)
                            End If
                        Next
                    End Using
                End Using

            Next
        Next

    End Sub

    Private Sub PorechopToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles PorechopToolStripMenuItem.Click
        Dim Files = Get_Fast_a_q_Files()
        If IsNothing(Files) = True Then Exit Sub
        Dim DIr = Szunyi.IO.Directory.Get_Folder()
        Dim r = Szunyi.Console.Porechop.Porechop.Get_it(Files, DIr)
        If r <> "" Then Clipboard.SetText(r)
        Beep()
    End Sub


    Private Sub COLinesToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles COLinesToolStripMenuItem.Click
        Dim Files = Get_Bam_Files()
        If IsNothing(Files) = True Then Exit Sub
        Szunyi.BAM.Filter_Split.Remove_Comment_Lines(Files)
    End Sub

    Private Sub RemovePicardToolStripMenuItem_Click_1(sender As Object, e As EventArgs) Handles RemovePicardToolStripMenuItem.Click
        Dim Files = Get_Bam_Files()
        If IsNothing(Files) = True Then Exit Sub
        Dim DIr = Files.First.Directory
        Dim RepFiles = From x In DIr.GetFiles Where x.Extension = ".rep"
        If RepFiles.Count = 0 Then Exit Sub

        For Each File In Files
            Dim cRep = From x In RepFiles Where x.Name.Contains(File.Name.Replace(File.Extension, ""))
            If cRep.Count <> 0 Then
                Dim BadRecords = Szunyi.Linux.Picard.Get_Bad_Records(cRep.First)
                If BadRecords.Count > 0 Then
                    Dim LastBadRecordID = BadRecords.Last.Record_ID
                    Dim Header = Szunyi.BAM.Bam_Basic_IO.Headers.Get_Header(File)

                    Dim nFIle = Szunyi.IO.Files.Get_New_FileName.Append_Before_Extension(File, "_Picard")
                    Using x1 As New Szunyi.BAM.Bam_Basic_IO.Export(nFIle, Header)
                        Dim Index As Integer = 0
                        For Each Sam In Szunyi.BAM.Bam_Basic_IO.Import.Parse(File)
                            Index += 1
                            If Index = LastBadRecordID Then
                                BadRecords.RemoveAt(BadRecords.Count - 1)
                                If BadRecords.Count = 0 Then
                                    LastBadRecordID = -2
                                Else
                                    LastBadRecordID = BadRecords.Last.Record_ID
                                End If

                            Else
                                x1.Write(Sam)
                            End If
                        Next
                    End Using
                End If
            End If
        Next
    End Sub
    Private Sub FullToolStripMenuItem1_Click(sender As Object, e As EventArgs) Handles FullToolStripMenuItem1.Click
        Dim Files = Get_Bam_Files()
        If IsNothing(Files) = True Then Exit Sub
        Dim x As New Szunyi.BAM.Filter_Split.RemoveSetting
        Dim c As New Controls.Set_Console_Properties(x)
        If c.ShowDialog = DialogResult.OK Then
            Szunyi.BAM.Filter_Split.Remove(Files, c.Input_Descriptions) ' It is also remove duplications
        End If
    End Sub
    Private Sub ModifySmallExonToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles ModifySmallExonToolStripMenuItem.Click
        Dim Files = Get_Bam_Files()
        If IsNothing(Files) = True Then Exit Sub
        Dim seqs = Me.GetSeqsFromSelectedList(True)
        If IsNothing(seqs) = True Then Exit Sub
        Dim x As New Szunyi.BAM.Settings.Last_Exons
        Dim t1 As New ClassLibrary1.Controls.Set_Console_Properties(x)
        If t1.ShowDialog <> DialogResult.OK Then Exit Sub

        For Each FIle In Files
            Dim ForCheck = Szunyi.IO.Files.Get_New_FileName.Append_Before_Extension(FIle, "_mdS_common")
            Dim nFile = Szunyi.IO.Files.Get_New_FileName.Append_Before_Extension(FIle, "_mdS")
            Dim logFile = Szunyi.IO.Files.Get_New_FileName.Replace_Extension(nFile, Szunyi.Constants.File_Extensions.Log)
            Dim logFile_Detaild = Szunyi.IO.Files.Get_New_FileName.Append_Before_Extension(logFile, "_Detailed")
            Using Check_Common As New Szunyi.BAM.Bam_Basic_IO.Export(ForCheck, Szunyi.BAM.Bam_Basic_IO.Headers.Get_Header(FIle))
                Using New_Sam_File As New Szunyi.BAM.Bam_Basic_IO.Export(nFile, Szunyi.BAM.Bam_Basic_IO.Headers.Get_Header(FIle))
                    Dim str_log As New System.Text.StringBuilder
                    Dim Stats As New List(Of Szunyi.BAM.SAM_Manipulation.Stat)
                    For Each sam In Szunyi.BAM.Bam_Basic_IO.Import.Parse(FIle)
                        Dim ToProcess As Boolean = False
                        Dim IsProccesed As Boolean = False
                        Dim oriSam = sam.Clone
                        Dim Stat As New SAM_Manipulation.Stat(sam)
                        Do
                            Dim x1 As New Szunyi.Alignment.Own_Al(sam)
                            Dim Loci = Szunyi.Location.Common.GetLocation(x1)
                            Dim Introns = Szunyi.Location.Common.Get_All_Intron_Location(Loci)

                            ToProcess = False
                            If Introns.Count <> 0 Then
                                Dim Exons = Szunyi.Location.Common.Get_All_Exon_Location(Loci)
                                If Szunyi.Location.Common.Get_Length(Exons.First) <= x.Exon_Length.Default_Value AndAlso Szunyi.Location.Common.Get_Length(Introns.First) >= x.Intron_Length.Default_Value Then
                                    Dim Exon_Seq = Szunyi.Sequences.SequenceManipulation.GetSequences.From_Loci(seqs.First, Exons.First)
                                    If Szunyi.DNA.Common.Get_AT_Percent(Exon_Seq.First, Loci.IsComplementer) >= x.AT_Percent.Default_Value Then
                                        ToProcess = True
                                        Szunyi.Alignment.Own_Al_Helper.Modify.Remove_First_Exon(x1, Introns.First, sam)
                                        Stat.Removed_5_Prime_Exon += 1
                                        Szunyi.Alignment.Own_Al_Helper.Modify.Set_Cigar_Md(sam, x1)
                                        IsProccesed = True
                                    End If
                                End If
                                If Szunyi.Location.Common.Get_Length(Exons.Last) <= x.Exon_Length.Default_Value AndAlso Szunyi.Location.Common.Get_Length(Introns.Last) >= x.Intron_Length.Default_Value Then
                                    Dim Exon_Seq = Szunyi.Sequences.SequenceManipulation.GetSequences.From_Loci(seqs.First, Exons.Last)
                                    If Szunyi.DNA.Common.Get_AT_Percent(Exon_Seq.First, Loci.IsComplementer) >= x.AT_Percent.Default_Value Then
                                        ToProcess = True
                                        Stat.Removed_3_Prime_Exon += 1
                                        Szunyi.Alignment.Own_Al_Helper.Modify.Remove_Last_Exon(x1, Introns.Last, sam)
                                        Szunyi.Alignment.Own_Al_Helper.Modify.Set_Cigar_Md(sam, x1)
                                        IsProccesed = True
                                    End If
                                End If
                            Else ' No Intron Try Modifiy End
                                Dim Pos = Szunyi.Alignment.Own_Al_Helper.Modify.Get_Md_Positions(sam, seqs.First)
                                If Pos.ToProccess = True Then
                                    Stat.Five += Pos.Five_Prime
                                    Stat.Three += Pos.Three_Prime
                                    Szunyi.Alignment.Own_Al_Helper.Modify.Modify_Ends(Pos, x1)
                                    Szunyi.Alignment.Own_Al_Helper.Modify.Set_Cigar_Md(sam, x1)
                                    IsProccesed = True
                                End If
                                ToProcess = False
                            End If
                            If ToProcess = False Then
                                Stats.Add(Stat)
                                Exit Do
                            End If
                        Loop
                        New_Sam_File.Write(sam)
                        If IsProccesed = True Then
                            Check_Common.Write(oriSam)
                            Check_Common.Write(sam)
                        End If
                    Next
                    Szunyi.IO.Export.SaveText(BAM.SAM_Manipulation.Stat.Get_Aggregate_Result(Stats, x), logFile)
                    Szunyi.IO.Export.SaveText(BAM.SAM_Manipulation.Stat.Get_Detailed_Result(Stats, x), logFile_Detaild)
                End Using
            End Using

        Next

    End Sub
    Public Sub ModifySsToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles ModifySsToolStripMenuItem.Click
        Dim Files = Get_Bam_Files()
        If IsNothing(Files) = True Then Exit Sub
        Dim seqs = Me.GetSeqsFromSelectedList(True)
        If IsNothing(seqs) = True Then Exit Sub
        For Each FIle In Files
            Dim ForCheck = Szunyi.IO.Files.Get_New_FileName.Append_Before_Extension(FIle, "_mdS_common")
            Dim nFile = Szunyi.IO.Files.Get_New_FileName.Append_Before_Extension(FIle, "_mdS")
            Dim logFile = Szunyi.IO.Files.Get_New_FileName.Replace_Extension(nFile, Szunyi.Constants.File_Extensions.Log)
            Dim logFile_Detaild = Szunyi.IO.Files.Get_New_FileName.Append_Before_Extension(logFile, "_Detailed")
            Using Check_Common As New Szunyi.BAM.Bam_Basic_IO.Export(ForCheck, Szunyi.BAM.Bam_Basic_IO.Headers.Get_Header(FIle))
                Using w2 As New Szunyi.BAM.Bam_Basic_IO.Export(nFile, Szunyi.BAM.Bam_Basic_IO.Headers.Get_Header(FIle))
                    Dim str_log As New System.Text.StringBuilder
                    Dim Stats As New List(Of Szunyi.BAM.SAM_Manipulation.Stat)
                    For Each sam In Szunyi.BAM.Bam_Basic_IO.Import.Parse(FIle)
                        Dim x1 As New Szunyi.Alignment.Own_Al(sam)
                        Dim Loci = Szunyi.Location.Common.GetLocation(x1)
                        Dim Pos = Szunyi.Alignment.Own_Al_Helper.Modify.Get_Md_Positions(sam, seqs.First)
                        Stats.Add(New Szunyi.BAM.SAM_Manipulation.Stat(Pos))
                        If Pos.ToProccess = True Then
                            Szunyi.Alignment.Own_Al_Helper.Modify.Modify_Ends(Pos, x1)
                            Check_Common.Write(sam)
                            Check_Common.Write(Pos.SAM)
                        Else
                            w2.Write(Pos.SAM)
                        End If
                    Next
                    Szunyi.IO.Export.SaveText(BAM.SAM_Manipulation.Stat.Get_Aggregate_Result(Stats, Nothing), logFile)
                    Szunyi.IO.Export.SaveText(BAM.SAM_Manipulation.Stat.Get_Detailed_Result(Stats, Nothing), logFile_Detaild)
                End Using
            End Using

        Next

    End Sub

    Private Sub GetMdStrinsToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles GetMdStrinsToolStripMenuItem.Click
        Dim Files = Get_Bam_Files()
        If IsNothing(Files) = True Then Exit Sub
        Dim str As New System.Text.StringBuilder
        For Each File In Files
            For Each sam In Szunyi.BAM.Bam_Basic_IO.Import.Parse(File)
                Dim MD_String = (From x In sam.OptionalFields Where x.Tag = "MD").First.Value
                str.Append(MD_String).AppendLine()
            Next
        Next
        Clipboard.SetText(str.ToString)
    End Sub

    Private Sub FastAFastQToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles FastAFastQToolStripMenuItem.Click
        Dim Files = Get_Bam_Files()
        If IsNothing(Files) = True Then Exit Sub
        Dim str As New System.Text.StringBuilder
        For Each File In Files
            Dim fq = Szunyi.IO.Files.Get_New_FileName.Append_Before_Extension(File, "_fastq")
            Using x1 As New Szunyi.BAM.Bam_Basic_IO.Export(fq, Szunyi.BAM.Bam_Basic_IO.Headers.Get_Header(File))
                For Each sam In Szunyi.BAM.Bam_Basic_IO.Import.Parse(File)
                    If sam.QuerySequence.GetType.ToString.Contains("Qua") Then
                        x1.Write(sam)
                    End If
                Next
            End Using
        Next
    End Sub

    Private Sub OrgToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles OrgToolStripMenuItem.Click
        Szunyi.Manager.MinIon.MinIon_Manager.Create_Organisms()

    End Sub

    Private Sub CircosToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles CircosToolStripMenuItem.Click
        Dim Files = Szunyi.IO.Files.Filter.SelectFiles(Szunyi.Constants.Files.All_TAB_Like)
        If IsNothing(Files) = True Then Exit Sub
        For Each File In Files
            Dim t = Szunyi.IO.Import.Text.ReadLines(File)
            Dim s = InputBox("Enter merging values separated by space")
            If s = String.Empty Then Exit Sub
            Dim s1 = Split(s, " ")
            For Each s In s1
                Dim sumFIle = Szunyi.IO.Files.Get_New_FileName.Append_Before_Extension(File, "_m_" & s & "sum")
                Dim sumlogFIle = Szunyi.IO.Files.Get_New_FileName.Append_Before_Extension(File, "_m_" & s & "sum_log")
                Dim str_sum As New System.Text.StringBuilder
                Dim str_sum_log As New System.Text.StringBuilder
                If IsNumeric(s) = True Then
                    Dim Index As Integer = 0
                    For i1 = 0 To t.Count - CInt(s) Step CInt(s)
                        Index += 1
                        Dim Sum As Integer = 0
                        Dim v As New List(Of String)
                        For i2 = i1 To i1 + CInt(s)
                            v = Split(t(i2), vbTab).ToList
                            Sum += v(3)
                        Next
                        v(1) = Index * CInt(s) - CInt(s) + 1
                        v(2) = Index * CInt(s)
                        v(3) = Sum
                        str_sum.Append(Szunyi.Text.General.GetText(v, " ")).Append(vbLf)
                        If Sum = 0 Then
                            v(3) = 0
                        Else
                            v(3) = System.Math.Log10(Sum)
                        End If

                        str_sum_log.Append(Szunyi.Text.General.GetText(v, " ")).Append(vbLf)
                    Next
                    str_sum.Length -= 1
                    str_sum_log.Length -= 1
                    Szunyi.IO.Export.SaveText(str_sum.ToString, sumFIle)
                    Szunyi.IO.Export.SaveText(str_sum_log.ToString, sumlogFIle)
                End If
            Next
        Next
    End Sub

    Private Sub GroupByStartEndToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles GroupByStartEndToolStripMenuItem.Click
        Dim Seqs = Me.GetSeqsFromSelectedList(True)
        If IsNothing(Seqs) = True Then Exit Sub
        Dim DiffFeatures = Szunyi.Features.FeatureManipulation.Key.Get_All_Different_Keys(Seqs)
        Dim f1 As New CheckBoxForStringsFull(DiffFeatures, -1)
        If f1.ShowDialog <> DialogResult.OK Then Exit Sub

        Dim str As New System.Text.StringBuilder
        For Each Seq In Seqs
            Dim Feats = Szunyi.Features.FeatureManipulation.GetFeaturesByType.GetFeturesByTypesFromSeq(Seq, f1.SelectedStrings)
            For Each Same_TSS In Szunyi.Features.FeatureManipulation.Iterator.By_TSS_wOrientation(Feats)
                str.Append(Seq.ID).Append(vbTab)
                str.Append("TSS").Append(vbTab)
                str.Append(Same_TSS.First.Location.TSS).Append(vbTab)
                str.Append(Szunyi.Location.Common.Get_Strand(Same_TSS.First.Location.IsComplementer)).Append(vbTab)
                str.Append(Same_TSS.Count).AppendLine()
            Next
            For Each Same_TSS In Szunyi.Features.FeatureManipulation.Iterator.By_PAS_wOrientation(Feats)
                str.Append(Seq.ID).Append(vbTab)
                str.Append("PAS").Append(vbTab)
                str.Append(Same_TSS.First.Location.PAS).Append(vbTab)
                str.Append(Szunyi.Location.Common.Get_Strand(Same_TSS.First.Location.IsComplementer)).Append(vbTab)
                str.Append(Same_TSS.Count).AppendLine()
            Next
        Next
        If str.Length > 0 Then
            str.Length -= 2
            Clipboard.SetText(str.ToString)
        End If
    End Sub

    Private Sub UTRsORFsToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles UTRsORFsToolStripMenuItem.Click
        Dim Seqs = Me.GetSeqsFromSelectedList(True)
        If IsNothing(Seqs) = True Then Exit Sub
        Dim DiffKeys = Szunyi.Features.FeatureManipulation.Key.Get_All_Different_Keys(Seqs)

        Dim f As New CheckBoxForStringsFull(DiffKeys, -1)
        If f.ShowDialog <> DialogResult.OK Then Exit Sub

        For Each Seq In Seqs
            Dim cSeq = Szunyi.Sequences.SequenceManipulation.Common.CloneSeq(Seq)

            Dim Feats = Szunyi.Features.FeatureManipulation.GetFeaturesByType.GetFeturesByTypesFromSeqs(Seqs, f.SelectedStrings)

            Dim CDSs = Szunyi.Features.FeatureManipulation.GetFeaturesByType.GetFeturesByTypeFromSeq(Seq, StandardFeatureKeys.CodingSequence)

            Dim newORFs As New List(Of FeatureItem)
            Dim newEL_ORFs As New List(Of FeatureItem)

            Dim All_ORFs As New List(Of Szunyi.DNA.ORF)
            For Each cmRNA In Feats
                Dim cCDS = Szunyi.Features.FeatureManipulation.Common.Get_Feature_Near_5_Prime_End(cmRNA, CDSs)
                If IsNothing(cCDS) = False Then
                    Dim fUTRwCDS = Szunyi.Features.FeatureManipulation.Common.Get_Five_UTRwCDS(cmRNA, cCDS)
                    Dim fUTR = Szunyi.Features.FeatureManipulation.Common.Get_Five_UTR(cmRNA, cCDS)
                    Dim ORFs = Szunyi.DNA.ORF_Finding.Get_All_ORFs_ByOrientation(Seq, fUTRwCDS)
                    Dim uORFs = Szunyi.DNA.ORF_Finding.Get_uORFs(ORFs, fUTR)
                    Dim Longest_uORFs = Szunyi.DNA.ORF_Finding.Get_Longest(uORFs)

                    For Each ORF In Longest_uORFs
                        Dim l As Bio.IO.GenBank.Location
                        If cCDS.Location.IsComplementer = True Then
                            l = Szunyi.Location.Common.GetLocation(fUTR.Location.LocationEnd - ORF.Pos, fUTR.Location.LocationEnd - ORF.Endy + 1, True)
                        Else
                            l = Szunyi.Location.Common.GetLocation(fUTR.Location.LocationStart + ORF.Pos, fUTR.Location.LocationStart + ORF.Endy - 1, False)
                        End If
                        Dim feat As New FeatureItem("ouORF", l)
                        feat.Label = Szunyi.Features.FeatureManipulation.Common.Get_LocusTag(cCDS)
                        Szunyi.Features.FeatureManipulation.Qulifiers.Add(feat, StandardQualifierNames.IdentifiedBy, cmRNA.Label)
                        cCDS.Label = feat.Label
                        ORF.Parent_CDS = cCDS
                        ORF.loci = l
                        ORF.Parent_mRNSs.Add(cmRNA)
                        All_ORFs.Add(ORF)
                        If l.PAS = cCDS.Location.PAS Then
                            newEL_ORFs.Add(feat)
                        Else
                            newORFs.Add(feat)
                        End If

                    Next

                Else
                    ' No CDS
                End If
            Next 'mRNA
            Dim Merged_ORFs = Szunyi.DNA.ORF_Manipulation.Merge(All_ORFs)
            Dim ByCDS = Szunyi.DNA.ORF_Manipulation.Stat_By_CDS(Merged_ORFs)
            Dim ByORF = Szunyi.DNA.ORF_Manipulation.Stat_By_ORF(Merged_ORFs)
            Dim BymRNA = Szunyi.DNA.ORF_Manipulation.Stat_By_mRNA(Merged_ORFs, Feats)
            Dim unique_new_ORFs = Szunyi.Features.FeatureManipulation.UniqueDistict.ByLocation(newORFs)
            Dim ByLabels = Szunyi.Features.FeatureManipulation.Iterator.By_Label(unique_new_ORFs)
            For Each ByLabel In ByLabels
                Dim Ordered = Szunyi.Features.FeatureManipulation.Sort.By_TSS_wOrientation(ByLabel)
                For i1 = 1 To Ordered.Count
                    Ordered(i1 - 1).Label = i1 & " " & Ordered(i1 - 1).Label
                    Szunyi.Features.FeatureManipulation.GenBankMetaDataManipulation.AddFeature(cSeq, Ordered(i1 - 1))
                Next
            Next
            For Each EL In Szunyi.Features.FeatureManipulation.UniqueDistict.ByLocation(newEL_ORFs)
                EL.Label = "EL " & EL.Label
                Szunyi.Features.FeatureManipulation.GenBankMetaDataManipulation.AddFeature(cSeq, EL)
            Next

            Szunyi.IO.Export.SaveSequencesToSingleGenBank(cSeq)
        Next ' Seq

    End Sub
    Private Sub UTRsToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles UTRsToolStripMenuItem.Click
        Dim Seqs = Me.GetSeqsFromSelectedList(True)
        If IsNothing(Seqs) = True Then Exit Sub
        Dim DiffKeys = Szunyi.Features.FeatureManipulation.Key.Get_All_Different_Keys(Seqs)

        Dim f As New CheckBoxForStringsFull(DiffKeys, -1)
        If f.ShowDialog <> DialogResult.OK Then Exit Sub

        For Each Seq In Seqs

            Dim Feats = Szunyi.Features.FeatureManipulation.GetFeaturesByType.GetFeturesByTypesFromSeqs(Seqs, f.SelectedStrings)

            Dim CDSs = Szunyi.Features.FeatureManipulation.GetFeaturesByType.GetFeturesByTypeFromSeq(Seq, StandardFeatureKeys.CodingSequence)

            For Each The_mRNA In Feats
                Dim cCDSs = From x In CDSs Where The_mRNA.Location.LocationStart <= x.Location.LocationStart And
                                               The_mRNA.Location.LocationEnd >= x.Location.LocationEnd And
                                           The_mRNA.Location.IsComplementer = x.Location.IsComplementer Order By x.Location.LocationEnd - x.Location.LocationStart Descending

                For Each CDS In cCDSs

                    Dim s = The_mRNA.GetSubSequence(Seq).ConvertToString
                    Dim c = CDS.GetSubSequence(Seq).ConvertToString
                    If s.Contains(c) = True Then
                        If The_mRNA.Location.IsComplementer = True Then
                            Dim fUTR_Location = Szunyi.Location.Common.GetLocation(The_mRNA.Location.LocationEnd, CDS.Location.LocationEnd + 1, True)
                            Dim tUTR_Location = Szunyi.Location.Common.GetLocation(CDS.Location.LocationStart - 1, The_mRNA.Location.LocationStart, True)
                            Dim fF As New FeatureItem(StandardFeatureKeys.FivePrimeUtr, fUTR_Location)
                            Dim tmp As String = ""
                            Dim CDS_Name As String = Szunyi.Features.FeatureManipulation.Common.GetName(CDS).First
                            s = The_mRNA.Label
                            fF.Label = s & "_" & CDS_Name
                            Szunyi.Features.FeatureManipulation.GenBankMetaDataManipulation.AddFeature(Seq, fF)

                            Dim tF As New FeatureItem(StandardFeatureKeys.ThreePrimeUtr, tUTR_Location)
                            tF.Label = s & "_" & CDS_Name
                            Szunyi.Features.FeatureManipulation.GenBankMetaDataManipulation.AddFeature(Seq, tF)
                            Dim ORFs = Szunyi.DNA.ORF_Finding.Get_All_ORFs(Seq, fF, Szunyi.DNA.Frames.frm, True, True)

                            Dim byLength = From x In ORFs Order By x.AASeq.Count Descending
                            For Each Item In byLength
                                Dim kj44 As Int16 = 54
                            Next
                            Dim jh As Int16 = 54
                        Else
                            Dim fUTR_Location = Szunyi.Location.Common.GetLocation(The_mRNA.Location.LocationStart, CDS.Location.LocationStart - 1)
                            Dim tUTR_Location = Szunyi.Location.Common.GetLocation(CDS.Location.LocationEnd + 1, The_mRNA.Location.LocationEnd)
                            Dim fF As New FeatureItem(StandardFeatureKeys.FivePrimeUtr, fUTR_Location)
                            Dim tmp As String = ""
                            Dim CDS_Name As String = Szunyi.Features.FeatureManipulation.Common.GetName(CDS).First

                            s = The_mRNA.Label

                            fF.Label = s & "_" & CDS_Name
                            Szunyi.Features.FeatureManipulation.GenBankMetaDataManipulation.AddFeature(Seq, fF)

                            Dim tF As New FeatureItem(StandardFeatureKeys.ThreePrimeUtr, tUTR_Location)
                            tF.Label = s & "_" & CDS_Name
                            Szunyi.Features.FeatureManipulation.GenBankMetaDataManipulation.AddFeature(Seq, tF)
                            Dim ORFs = Szunyi.DNA.ORF_Finding.Get_All_ORFs(Seq, fF, Szunyi.DNA.Frames.fr, True, True)
                            Dim byLength = From x In ORFs Order By x.AASeq.Count Descending
                            For Each Item In byLength
                                Dim kj11 As Int16 = 54
                            Next
                            Dim jh As Int16 = 54
                        End If
                        '  Exit For
                    End If

                Next
            Next


        Next
        Szunyi.IO.Export.SaveSequencesToSingleGenBank(Seqs)
    End Sub

    Private Sub LocusTagsToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles LocusTagsToolStripMenuItem.Click
        Dim seqs = Me.GetSeqsFromSelectedList(True)
        If IsNothing(seqs) = True Then Exit Sub
        Dim cSeqs = Szunyi.Sequences.SequenceManipulation.Common.CloneSeqs(seqs)
        Dim str As New System.Text.StringBuilder
        Dim Nof = Szunyi.MyInputBox.GetInteger("Nof numbers")
        Dim Prefix = InputBox("Enter prefix")
        For Each Seq In cSeqs
            Dim Genes = Szunyi.Features.FeatureManipulation.GetFeaturesByType.GetFeturesByTypeFromSeq(Seq, StandardFeatureKeys.Gene)
            Dim CDSs = Szunyi.Features.FeatureManipulation.GetFeaturesByType.GetFeturesByTypeFromSeq(Seq, StandardFeatureKeys.CodingSequence)
            Dim Index As Integer = 0
            For Each g In Genes
                Dim sel = From x In CDSs Where Szunyi.Location.Common.GetLocationString(x.Location) = Szunyi.Location.Common.GetLocationString(g.Location)

                Index += 1
                Dim cL = Index * 10
                Dim Values As New List(Of String)
                Values.Add(Prefix & cL.ToString("D" & Nof))
                g.Qualifiers(StandardQualifierNames.LocusTag) = Values
                sel.First.Qualifiers(StandardQualifierNames.LocusTag) = Values
            Next
        Next
        Szunyi.IO.Export.SaveSequencesToSingleGenBank(cSeqs)
    End Sub

    Private Sub FoundNotFOundFastqAndBamToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles FoundNotFOundFastqAndBamToolStripMenuItem.Click
        ' Get all Read IF From Sam Files
        ' Sort
        ' Parse all fastq
        ' if found to 1 file
        ' if not to another files
        Dim Files = Get_Bam_Files()
        If IsNothing(Files) = True Then Exit Sub
        Dim FastQs = Get_Fast_a_q_Files()
        If IsNothing(FastQs) = True Then Exit Sub
        Dim ReadIDs = Szunyi.BAM.Bam_Basic_IO.Import.Get_Sorted_Distinct_Read_IDs(Files)
        For Each file In FastQs
            Using wFOund As New Szunyi.IO.Export.fastQ_Writter(Szunyi.IO.Files.Get_New_FileName.Append_Before_Extension_wNew_Extension(file, "_Found.fq"))
                Using wNot_FOund As New Szunyi.IO.Export.fastQ_Writter(Szunyi.IO.Files.Get_New_FileName.Append_Before_Extension_wNew_Extension(file, "_Not_Found.fq"))

                    For Each seq In Szunyi.IO.Import.Sequence.From_File_Iterator(file)
                        Dim Index = ReadIDs.BinarySearch(Split(seq.ID, " ").First)
                        If Index < 0 Then ' Not Found
                            wNot_FOund.writeQ(seq)
                        Else ' Found
                            wFOund.writeQ(seq)
                        End If
                    Next
                End Using
            End Using
        Next
    End Sub


#End Region

#End Region

#End Region
    ''' <summary>
    ''' Return List of FeatureItem or new list of featureitem
    ''' </summary>
    ''' <param name="Seq"></param>
    ''' <returns></returns>
    Public Function Select_Features() As List(Of FeatureItem)
        Dim Seqs = Me.GetSeqsFromSelectedList(True)
        If IsNothing(Seqs) = True Then Return New List(Of FeatureItem)
        Dim DiffKeys = Szunyi.Features.FeatureManipulation.Key.Get_All_Different_Keys(Seqs)

        Dim f As New CheckBoxForStringsFull(DiffKeys, -1)
        If f.ShowDialog <> DialogResult.OK Then Return New List(Of FeatureItem)
        Return Szunyi.Features.FeatureManipulation.GetFeaturesByType.GetFeturesByTypesFromSeqs(Seqs, f.SelectedStrings)
    End Function

    ''' <summary>
    ''' Return List of FeatureItem or new list of featureitem
    ''' </summary>
    ''' <param name="Seq"></param>
    ''' <returns></returns>
    Public Function Select_Features(Seq As Bio.ISequence) As List(Of FeatureItem)

        Dim DiffKeys = Szunyi.Features.FeatureManipulation.Key.Get_All_Different_Keys(Seq)

        Dim f As New CheckBoxForStringsFull(DiffKeys, -1)
        If f.ShowDialog <> DialogResult.OK Then Return New List(Of FeatureItem)
        Return Szunyi.Features.FeatureManipulation.GetFeaturesByType.GetFeturesByTypesFromSeq(Seq, f.SelectedStrings)
    End Function
    ''' <summary>
    ''' retrun list of string or empty list
    ''' </summary>
    ''' <returns></returns>
    Public Function Select_Quilifiers() As List(Of String)
        Dim Seqs = Me.GetSeqsFromSelectedList(True)
        If IsNothing(Seqs) = True Then Return New List(Of String)
        Dim QualNames = Szunyi.Features.FeatureManipulation.Qulifiers.Get_Distinct_Qulifiers(Seqs)
        Dim f1 As New CheckBoxForStringsFull(QualNames, 1, "Qulifier To Search")
        If f1.ShowDialog <> DialogResult.OK Then Return New List(Of String)
        Return f1.SelectedStrings
    End Function
    Public Function Select_Quilifiers(Seqs As List(Of Bio.ISequence)) As List(Of String)

        Dim QualNames = Szunyi.Features.FeatureManipulation.Qulifiers.Get_Distinct_Qulifiers(Seqs)
        Dim f1 As New CheckBoxForStringsFull(QualNames, 1, "Qulifier To Search")
        If f1.ShowDialog <> DialogResult.OK Then Return New List(Of String)
        Return f1.SelectedStrings
    End Function
    Private Sub DuplicateToolStripMenuItem1_Click(sender As Object, e As EventArgs) Handles DuplicateToolStripMenuItem1.Click
        Dim Seqs = Me.GetSeqsFromSelectedList(True)
        If IsNothing(Seqs) = True Then Exit Sub

        Dim Feats = Select_Features()
        If Feats.Count = 0 Then Exit Sub
        Dim Quals = Select_Quilifiers()
        If Quals.Count = 0 Then Exit Sub
        Dim Files = Szunyi.IO.Files.Filter.SelectFiles(Szunyi.Constants.Files.All_TAB_Like)
        For Each File In Files
            For Each Line In Szunyi.IO.Import.Text.ParseNotFirst(File, 1)
                Dim s = Split(Line, vbTab)
                For Each q In Quals
                    For Each f In Feats
                        If f.Qualifiers.ContainsKey(q) = True AndAlso f.Qualifiers(q).First.Contains(s.First) Then
                            Dim d As New FeatureItem(s.Last, f.Location)
                            Szunyi.Features.FeatureManipulation.MergeFeatures.MergeQualifiers(f, d)
                            Szunyi.Features.FeatureManipulation.GenBankMetaDataManipulation.AddFeature(Seqs.First, d)

                        End If
                    Next
                Next
            Next
        Next
        Szunyi.IO.Export.SaveSequencesToSingleGenBank(Seqs)
    End Sub

    Private Sub ByFeaturesToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles ByFeaturesToolStripMenuItem.Click
        Dim OriSeqFile = Szunyi.IO.Files.Filter.SelectFile(Constants.Files.GenBank, "From Transfer")
        If IsNothing(OriSeqFile) = True Then Exit Sub
        Dim ToSeqFIle = Szunyi.IO.Files.Filter.SelectFile(Constants.Files.GenBank, "To Transfer")
        If IsNothing(ToSeqFIle) = True Then Exit Sub
        Dim OriSeq = Szunyi.IO.Import.Sequence.FromFile(OriSeqFile).First
        Dim ToSeq = Szunyi.IO.Import.Sequence.FromFile(ToSeqFIle).First
        Dim Feats = Select_Features(OriSeq)
        For Each Feat In Feats
            Szunyi.Features.FeatureManipulation.GenBankMetaDataManipulation.AddFeature(ToSeq, Feat)
        Next
        Szunyi.IO.Export.SaveSequencesToSingleGenBank(ToSeq)
    End Sub


    Private Sub BEDWithColorsToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles BEDWithColorsToolStripMenuItem.Click
        Dim Seqs = GetSeqsFromSelectedList(True)
        If IsNothing(Seqs) = True Then Exit Sub
        Dim Feats = Select_Features()
        If Feats.Count = 0 Then Exit Sub
        Dim FIle = Szunyi.IO.Files.Save.SelectSaveFile(Constants.Files.BED)
        Dim str As New System.Text.StringBuilder
        Dim dFeateKeys = (From x In Feats Select x.Key).Distinct
        Dim SelQuals = Select_Custom_Qulifiers()
        Dim out As New List(Of ISequenceRange)
        For Each d In dFeateKeys
            Dim c As New Windows.Forms.ColorDialog
            c.Tag = d
            If c.ShowDialog = DialogResult.OK Then
                Dim cFs = (From x In Feats Where x.Key = d).ToList
                out.AddRange(Szunyi.Features.FeatureManipulation.Convert.ToBed(cFs, Seqs.First, c.Color, SelQuals))
            End If
        Next
        Dim b As New Bio.IO.Bed.BedFormatter
        b.Format(out, FIle.FullName)


    End Sub

    Private Sub CIGARMDToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles CIGARMDToolStripMenuItem.Click
        '   Dim Seqs = GetSeqsFromSelectedList(True)
        '   If IsNothing(Seqs) = True Then Exit Sub
        Dim BamFiles = Get_Bam_Files()
        If IsNothing(BamFiles) = True Then Exit Sub
        Dim out As New List(Of Bio.IO.SAM.SAMAlignedSequence)
        Dim out2 As New List(Of Bio.IO.SAM.SAMAlignedSequence)
        Dim All As New List(Of Bio.IO.SAM.SAMAlignedSequence)
        For Each SAM In Szunyi.BAM.Bam_Basic_IO.Import.Parse(BamFiles)
            Dim x1 As New Szunyi.Alignment.Own_Al(SAM)


        Next
        Dim OriBad = Szunyi.IO.Files.Get_New_FileName.Append_Before_Extension(BamFiles.First, "Original_BAD")
        Dim x As New Szunyi.BAM.Bam_Basic_IO.Export(OriBad, Szunyi.BAM.Bam_Basic_IO.Headers.Get_Header(BamFiles.First))
        x.Writes(out)
        Dim kj2 As Int16 = 54
    End Sub

    Private Sub UniqueToolStripMenuItem1_Click(sender As Object, e As EventArgs) Handles UniqueToolStripMenuItem1.Click
        Dim Seqs = GetSeqsFromSelectedList(True)
        If IsNothing(Seqs) = True Then Exit Sub
        Dim FIle = Szunyi.IO.Files.Filter.SelectFile(Constants.Files.All_TAB_Like)
        If IsNothing(FIle) = True Then Exit Sub
        Dim Header = Szunyi.IO.Import.Text.GetHeader(FIle, 1)
        Dim f1 As New CheckBoxForStringsFull(Header, 1, "Start")
        If f1.ShowDialog <> DialogResult.OK Then Exit Sub
        Dim Start = Header.IndexOf(f1.SelectedStrings.First)
        f1.Clear_Selections()
        f1.Text = "Orientation"
        If f1.ShowDialog <> DialogResult.OK Then Exit Sub
        Dim Orientation = Header.IndexOf(f1.SelectedStrings.First)

        f1.Clear_Selections()
        f1.Text = "Feature Key"
        If f1.ShowDialog <> DialogResult.OK Then Exit Sub
        Dim Key = Header.IndexOf(f1.SelectedStrings.First)

        f1.Clear_Selections()
        f1.Text = "Label"
        If f1.ShowDialog <> DialogResult.OK Then Exit Sub
        Dim Label = Header.IndexOf(f1.SelectedStrings.First)

        f1.Clear_Selections()
        f1.Text = "End"
        Dim Endy As Integer = -1
        If f1.ShowDialog = DialogResult.OK Then
            Endy = Header.IndexOf(f1.SelectedStrings.First)
        End If
        For Each line In Szunyi.IO.Import.Text.Parse(FIle, 1)
            Dim s = Split(line, vbTab)
            Dim l As Bio.IO.GenBank.ILocation
            If Endy = -1 Then
                l = Szunyi.Location.Common.GetLocation(s(Start), s(Orientation))
            Else
                l = Szunyi.Location.Common.GetLocation(s(Start), s(Endy), s(Orientation))
            End If
            Dim f As New FeatureItem(s(Key), l)
            f.Label = s(Label)
            Szunyi.Features.FeatureManipulation.GenBankMetaDataManipulation.AddFeature(Seqs.First, f)
        Next
        Szunyi.IO.Export.SaveSequencesToSingleGenBank(Seqs.First)
    End Sub

    Private Sub SMToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles SMToolStripMenuItem.Click
        Dim Seqs = GetSeqsFromSelectedList(True)
        If IsNothing(Seqs) = True Then Exit Sub
        Dim Sel_Fets = Select_Features(Seqs.First)
        If Sel_Fets.Count = 0 Then Exit Sub

        Dim Local_Width = MyInputBox.GetInteger("Local Width")
        Dim Match = MyInputBox.GetInteger("Match Score")
        Dim MisMatch = MyInputBox.GetInteger("MisMatch Score")
        Dim GapOpen = MyInputBox.GetInteger("Gap Open Cost")

        Dim Str As New System.Text.StringBuilder
        Str.Append("#Local Width").Append(Local_Width).AppendLine()
        Str.Append("#Match Score").Append(Match).AppendLine()
        Str.Append("#MisMatch Score").Append(MisMatch).AppendLine()
        Str.Append("#Gap Open Cost").Append(GapOpen).AppendLine()

        Dim pA = New Bio.Sequence(Alphabets.DNA, Szunyi.Text.General.Multiply("A", Local_Width * 2 + 1))
        Dim pT = New Bio.Sequence(Alphabets.DNA, Szunyi.Text.General.Multiply("T", Local_Width * 2 + 1))
        Dim SM As New Bio.SimilarityMatrices.DiagonalSimilarityMatrix(Match, MisMatch)
        Dim Sm_Aligner As New Bio.Algorithms.Alignment.SmithWatermanAligner()
        Sm_Aligner.SimilarityMatrix = SM
        Sm_Aligner.GapOpenCost = -System.Math.Abs(GapOpen)
        Str.Append("Label").Append(vbTab).Append("pos").Append(vbTab).Append("Score").Append(vbTab).Append("pA/pT pos. start").Append(vbTab).Append("pA/pT pos end")
        Str.Append(vbTab).Append("Seq").Append(vbTab).Append("Consensus").AppendLine()
        For Each F In Sel_Fets
            Str.Append(F.Label).Append(vbTab).Append(Szunyi.Location.Common.GetLocationString(F)).Append(vbTab)
            Dim s = Seqs.First.GetSubSequence(F.Location.LocationEnd - Local_Width, Local_Width * 2)
            If F.Location.IsComplementer = False Then
                Dim r_f = Sm_Aligner.AlignSimple(s, pA)
                If r_f.First.AlignedSequences.Count = 0 Then
                    Str.Append("0").Append(vbTab).AppendLine()
                Else
                    Str.Append(r_f.First.First.Score).Append(vbTab)
                    Dim OffStes1 = (From x In r_f.First.PairwiseAlignedSequences Select x.Metadata("StartOffsets")(0)).Distinct.ToList
                    Dim OffStes3 = (From x In r_f.First.PairwiseAlignedSequences Select x.Metadata("EndOffsets")(0)).Distinct.ToList
                    Dim Best_Value_start As Integer = 2 * Local_Width
                    Dim Best_position_start As Integer = 0
                    Dim Best_Value_end As Integer = 2 * Local_Width
                    Dim Best_position_end As Integer = 0
                    For Each Item In OffStes1
                        If System.Math.Abs(Local_Width - Item) < Best_Value_start Then
                            Best_position_start = Item
                            Best_Value_start = System.Math.Abs(Local_Width - Item)
                        End If
                    Next
                    For Each Item In OffStes3
                        If System.Math.Abs(Local_Width - Item) < Best_Value_end Then
                            Best_position_end = Item
                            Best_Value_end = System.Math.Abs(Local_Width - Item)
                        End If
                    Next
                    If Best_Value_start <= Best_Value_end Then
                        Str.Append(Best_position_start + 1).Append(vbTab).Append(Best_position_start + r_f.First.First.Consensus.Count).Append(vbTab)
                    Else
                        Str.Append(Best_position_end - r_f.First.First.Consensus.Count + 2).Append(vbTab).Append(Best_position_end + 1).Append(vbTab)
                    End If
                    Str.Append(s.ConvertToString).Append(vbTab).Append(r_f.First.First.Consensus.ConvertToString).AppendLine()
                End If
            Else
                Dim r_f = Sm_Aligner.AlignSimple(s, pT)

                If r_f.First.AlignedSequences.Count = 0 Then
                    Str.Append("0").Append(vbTab).AppendLine()
                Else
                    Str.Append(r_f.First.First.Score).Append(vbTab)
                    Dim OffStes1 = (From x In r_f.First.PairwiseAlignedSequences Select x.Metadata("StartOffsets")(0)).Distinct.ToList
                    Dim OffStes3 = (From x In r_f.First.PairwiseAlignedSequences Select x.Metadata("EndOffsets")(0)).Distinct.ToList
                    Dim Best_Value_start As Integer = 2 * Local_Width
                    Dim Best_position_start As Integer = 0
                    Dim Best_Value_end As Integer = 2 * Local_Width
                    Dim Best_position_end As Integer = 0
                    For Each Item In OffStes1
                        If System.Math.Abs(Local_Width - Item) < Best_Value_start Then
                            Best_position_start = Item
                            Best_Value_start = System.Math.Abs(Local_Width - Item)
                        End If
                    Next
                    For Each Item In OffStes3
                        If System.Math.Abs(Local_Width - Item) < Best_Value_end Then
                            Best_position_end = Item
                            Best_Value_end = System.Math.Abs(Local_Width - Item)
                        End If
                    Next
                    If Best_Value_start <= Best_Value_end Then
                        Str.Append(Best_position_start + 1).Append(vbTab).Append(Best_position_start + r_f.First.First.Consensus.Count).Append(vbTab)
                    Else
                        Str.Append(Best_position_end - r_f.First.First.Consensus.Count + 2).Append(vbTab).Append(Best_position_end + 1).Append(vbTab)
                    End If
                    Str.Append(s.ConvertToString).Append(vbTab).Append(r_f.First.First.Consensus.ConvertToString).AppendLine()
                End If
            End If
        Next
    End Sub

    Private Sub ByKeyAndPositionToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles ByKeyAndPositionToolStripMenuItem.Click
        Dim Seqs = Me.GetSeqsFromSelectedList(True)
        If IsNothing(Seqs) = True Then Exit Sub
        Dim Feats = Select_Features(Seqs.First)
        Dim same = From x In Feats Group By x.Location.LocationEnd, x.Location.LocationStart, x.Key Into Group

        For Each s In same
            If s.Group.Count > 1 Then
                For i1 = 1 To s.Group.Count - 1
                    Szunyi.Features.FeatureManipulation.GenBankMetaDataManipulation.Remove_Feature(Seqs.First, s.Group(i1))
                Next
            End If
        Next
        Szunyi.IO.Export.SaveSequencesToSingleGenBank(Seqs.First)
    End Sub

    Private Sub FToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles FToolStripMenuItem.Click
        Dim x1 As New Szunyi.BAM.Stat.Stat_Settings
        Dim t1 As New ClassLibrary1.Controls.Set_Console_Properties(x1)
        If t1.ShowDialog <> DialogResult.OK Then Exit Sub
        Dim FIles = Get_Bam_Files()
        If IsNothing(FIles) = True Then Exit Sub
        Dim x As New Szunyi.BAM.Stat.StatII(x1, FIles)

        Dim kj As Int16 = 54
    End Sub



    Private Sub PacBioSubReadsToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles PacBioSubReadsToolStripMenuItem.Click
        Dim BamFIles = Get_Bam_Files()
        If IsNothing(BamFIles) = True Then Exit Sub
        Dim ReadIDs As New Dictionary(Of FileInfo, List(Of String))

        For Each File In BamFIles
            Dim x = Szunyi.BAM.Bam_Basic_IO.Import.Get_Sorted_Read_IDs(File)
            Dim x1 = Szunyi.Text.General.Get_Not_Last_Parts(x, "/")
            ReadIDs.Add(File, x1)
        Next
        Dim Pacbio_BAM_Files = Get_Bam_Files("PacBio BAMs")
        For Each FIle In Pacbio_BAM_Files
            Dim Writters As New Dictionary(Of FileInfo, Szunyi.BAM.Bam_Basic_IO.Export)
            For Each R In ReadIDs
                Dim k = Szunyi.IO.Files.Get_New_FileName.Append_Before_Extension(FIle, R.Key)
                Dim w As New Szunyi.BAM.Bam_Basic_IO.Export(k, Szunyi.BAM.Bam_Basic_IO.Headers.Get_Header(FIle))
                Writters.Add(R.Key, w)
            Next
            For Each SAM In Szunyi.BAM.Bam_Basic_IO.Import.Parse(FIle)
                Dim tRID = Szunyi.Text.General.Get_Not_Last_Part(SAM.QName, "/")
                For Each Item In ReadIDs
                    Dim Index = Item.Value.BinarySearch(tRID)
                    If Index > -1 Then
                        Writters(Item.Key).Write(SAM)
                    Else
                        Dim kj As Int16 = 54
                    End If
                Next
            Next
        Next
    End Sub

    Private Sub COmpareToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles COmpareToolStripMenuItem.Click
        Dim Seqs = Me.GetSeqsFromSelectedList(True)

        If IsNothing(Seqs) = True Then Exit Sub
        Dim cSeqs = Szunyi.Sequences.SequenceManipulation.Common.CloneSeqs(Seqs)
        Szunyi.Features.FeatureManipulation.Common.Set_Features_Location_accession(cSeqs)

        Dim Feats = Select_Custom_Features(cSeqs)
        If IsNothing(Feats) = True OrElse Feats.Count = 0 Then Exit Sub
        Dim Common As New List(Of FeatureItem)
        Dim Differents As New List(Of FeatureItem)
        Dim Res As New Dictionary(Of FeatureItem, Dictionary(Of String, Boolean))
        Dim str As New System.Text.StringBuilder
        Dim SeqIDs = Szunyi.Sequences.SequenceManipulation.Common.GetSeqIDs(cSeqs)
        str.Append(Split("Key,TSS,PAS,Strand", vbTab)).Append(vbTab)
        str.Append(Szunyi.Text.General.GetText(SeqIDs, vbTab)).AppendLine()

        Dim gr = From x In Feats Group By x.Location.LocationStart, x.Location.LocationEnd, x.Location.IsComplementer, x.Key Into Group

        For Each g In gr
            Dim NofFound As Int16 = 0
            str.Append(g.Key).Append(vbTab).Append(Szunyi.Location.Common.GetLocationStringTSS_PAS_Strand_Tab(g.Group.First.Location))
            str.Append(vbTab)
            For Each SeqID In SeqIDs
                Dim k = From x In g.Group Where x.Location.Accession = SeqID

                If k.Count <> 0 Then
                    str.Append("+").Append(vbTab)
                    NofFound += 1
                Else
                    str.Append("-").Append(vbTab)
                End If
            Next
            str.Append(NofFound).AppendLine()
        Next

        Clipboard.SetText(str.ToString)
    End Sub

    Private Sub SubFolderFileAndFullfilenameToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles SubFolderFileAndFullfilenameToolStripMenuItem.Click
        Dim FOlder = New FolderSelectDialog
        If FOlder.ShowDialog = DialogResult.OK Then
            Dim SaveFile = Szunyi.IO.Files.Save.SelectSaveFile(Constants.Files.All_TAB_Like)
            Dim str As New System.Text.StringBuilder
            If IsNothing(SaveFile) = True Then
                For Each F In FOlder.FolderNames
                    Dim Files = Szunyi.IO.Directory.GetAllFilesFromFolder_Recursive(F)
                    For Each File In Files
                        str.Append(File.Name).Append(vbTab).Append(File.FullName).AppendLine()
                    Next
                Next
                If str.Length > 0 Then
                    str.Length -= 2
                    Szunyi.IO.Export.SaveText(str.ToString, SaveFile)
                End If

            End If

        End If
    End Sub

    Private Sub ReadIdsAndFastqToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles ReadIdsAndFastqToolStripMenuItem.Click
        Dim File = Szunyi.IO.Files.Filter.SelectFile(Constants.Files.All_TAB_Like)
        If IsNothing(File) = True Then Exit Sub
        Dim IDs = Szunyi.IO.Import.Text.ReadLines(File)
        IDs.Sort()
        Dim SeqFiles = Szunyi.IO.Files.Filter.SelectFiles(Constants.Files.SequenceFileTypesToImport)
        If IsNothing(SeqFiles) = True Then Exit Sub
        Dim log As New System.Text.StringBuilder
        For Each File In SeqFiles

            Dim SeqIDs As List(Of String) = Szunyi.IO.Import.Sequence.IDs(File)
            Dim fSeqIDs = Szunyi.Text.General.GetFirstParts(SeqIDs, " ")
            fSeqIDs.Sort()
            For Each R_ID In IDs
                Dim Index = fSeqIDs.BinarySearch(R_ID)
                If Index > -1 Then
                    log.Append(R_ID).Append(vbTab).Append(File.FullName).AppendLine()
                End If
            Next
        Next

        Dim kj As Int16 = 32
    End Sub



    Private Sub VcfValidatorToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles VcfValidatorToolStripMenuItem.Click
        Dim files = Szunyi.IO.Files.Filter.SelectFiles(Constants.Files.vcf)
        If IsNothing(files) = True Then Exit Sub
        Dim str As New System.Text.StringBuilder
        For Each FIle In files
            str.Append("/mnt/c/Linux/vcf/vcf_validator -i ").Append(Szunyi.IO.Linux.Get_FileName(FIle)).AppendLine()
        Next
        If str.Length > 0 Then Clipboard.SetText(str.ToString)
    End Sub

    Private Sub AnalysisToolStripMenuItem_Click(sender As Object, e As EventArgs)
        Dim FIles = Szunyi.IO.Files.Filter.SelectFiles(Constants.Files.gpg)
        Dim File = Szunyi.IO.Files.Filter.SelectFile(Constants.Files.All_TAB_Like, "Select Sample List")
        If IsNothing(File) = True Then Exit Sub
        If IsNothing(FIles) = True Then Exit Sub
        Dim AllFiles = FIles.First.Directory.GetFiles
        Dim str As New System.Text.StringBuilder
        str.Append(Chr(34) & "Sample alias" & Chr(34) & ", ")
        str.Append(Chr(34) & "First Fastq File" & Chr(34) & ", ")
        str.Append(Chr(34) & "First Checksum" & Chr(34) & ", ")
        str.Append(Chr(34) & "First Unencrypted checksum" & Chr(34) & ", ")
        str.Append(Chr(34) & "Second Fastq File" & Chr(34) & ", ")
        str.Append(Chr(34) & "Second Checksum" & Chr(34) & ", ")
        str.Append(Chr(34) & "Second Unencrypted checksum" & Chr(34)).AppendLine()
        Dim log As New System.Text.StringBuilder
        Dim Samples As New List(Of String)
        Try


            For Each Item In Szunyi.IO.Import.Text.Parse(File)
                If Item = "YA626" Then
                    Dim kjjj As Integer = 54
                End If
                Dim TheFIle = From x In FIles Where x.Name.Contains(Item)
                Samples.Add(Item)
                If TheFIle.Count = 2 Or TheFIle.Count = 4 Then
                    str.Append(Chr(34))
                    str.Append(Item)
                    str.Append(Chr(34))
                    str.Append(",")
                    Dim First_Fastq = From x In AllFiles Where x.Extension = ".gz" And x.Name.Contains("_1_") And x.Name.Contains(Item)
                    If First_Fastq.Count > 0 Then
                        str.Append(Chr(34))
                        str.Append("EBI_INBOX.ega-box-830_").Append(First_Fastq.First.Name).Append(Chr(34)).Append(",")
                        '      str.Append(Chr(34)).Append(Chr(34)).Append(",")
                        '      str.Append(Chr(34)).Append(Chr(34)).Append(",")
                        Dim First_GPG = From x In AllFiles Where x.Name = First_Fastq.First.Name & ".gpg.md5"
                        str.Append(Chr(34))
                        If First_GPG.Count > 0 Then
                            str.Append(Szunyi.IO.Import.Text.ReadToEnd(First_GPG.First)).Append(Chr(34)).Append(",")
                        Else
                            str.Append(Chr(34))
                            str.Append(",")
                        End If

                        Dim First_GPG_u = From x In AllFiles Where x.Name = First_Fastq.First.Name & ".md5"
                        str.Append(Chr(34))
                        If First_GPG_u.Count > 0 Then

                            str.Append(Szunyi.IO.Import.Text.ReadToEnd(First_GPG_u.First)).Append(Chr(34)).Append(",")
                        Else
                            str.Append(Chr(34))
                            str.Append(",")
                        End If
                    End If

                    Dim Second_Fastq = From x In AllFiles Where x.Extension = ".gz" And x.Name.Contains("_2_") And x.Name.Contains(Item)
                    If Second_Fastq.Count > 0 Then
                        str.Append(Chr(34))
                        str.Append("EBI_INBOX.ega-box-830_")
                        str.Append(Second_Fastq.First.Name).Append(Chr(34)).Append(",")
                        '       str.Append(Chr(34)).Append(Chr(34)).Append(",")
                        '        str.Append(Chr(34)).Append(Chr(34))
                        Dim Second_GPG = From x In AllFiles Where x.Name = Second_Fastq.First.Name & ".gpg.md5"
                        str.Append(Chr(34))
                        If Second_GPG.Count > 0 Then
                            str.Append(Szunyi.IO.Import.Text.ReadToEnd(Second_GPG.First)).Append(Chr(34)).Append(",")
                        Else
                            str.Append(Chr(34))
                            str.Append(",")
                        End If
                        Dim Second_GPG_u = From x In AllFiles Where x.Name = Second_Fastq.First.Name & ".md5"
                        If Second_GPG_u.Count > 0 Then
                            str.Append(Chr(34))
                            str.Append(Szunyi.IO.Import.Text.ReadToEnd(Second_GPG_u.First))
                            str.Append(Chr(34))
                        Else
                            str.Append(Chr(34))

                        End If
                        str.AppendLine()


                    ElseIf TheFIle.Count = 0 Then
                        log.Append(Item).Append(vbTab).Append("Not Found").AppendLine()
                    Else
                        log.Append(Item).Append(vbTab).Append("More Files").AppendLine()
                    End If
                End If

            Next
        Catch ex As Exception
            Dim jjjj As Integer = 43
        End Try
        Dim SmaplesII = Szunyi.Text.General.GetText(Samples)
        If str.Length > 0 Then
            str.Length -= 2
            Szunyi.IO.Export.SaveText(str.ToString)
        End If
    End Sub



    Private Sub NotFoundToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles NotFoundToolStripMenuItem.Click
        Dim Fastq_Files = Szunyi.IO.Files.Filter.SelectFiles(Constants.Files.Fasta_FastQ)
        If IsNothing(Fastq_Files) Then Exit Sub
        Dim Bam_Files = Get_Bam_Files()
        If IsNothing(Bam_Files) Then Exit Sub
        Dim SeqIDs = Szunyi.BAM.Bam_Basic_IO.Import.Get_Sorted_Distinct_Read_IDs(Bam_Files)
        For Each File In Fastq_Files
            Dim sw As New Bio.IO.FastQ.FastQFormatter()
            sw.AutoFlush = True
            sw.Open(Szunyi.IO.Files.Get_New_FileName.Append_Before_Extension(File, "_NotFoundYet").FullName)
            Dim LastSeq As Bio.ISequence
            Try

                For Each Seq In Szunyi.IO.Import.Sequence.From_File_Iterator(File)
                    Dim s = Split(Seq.ID, " ").First
                    LastSeq = Seq
                    Dim Index = SeqIDs.BinarySearch(s)
                    If Index < 0 Then
                        sw.Format(Seq)
                    End If
                Next
            Catch ex As Exception
                MsgBox(LastSeq.ID)
            End Try

            sw.Close
        Next
    End Sub

    Private Sub LengthToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles LengthToolStripMenuItem.Click
        Dim Bam_Files = Get_Bam_Files()
        If IsNothing(Bam_Files) Then Exit Sub
        Dim str As New System.Text.StringBuilder
        For Each File In Bam_Files
            Dim sg As New Dictionary(Of Integer, Integer)
            For Each Seq In Szunyi.BAM.Bam_Basic_IO.Import.Parse(File)
                If sg.ContainsKey(Seq.QuerySequence.Count) = False Then sg.Add(Seq.QuerySequence.Count, 0)
                sg(Seq.QuerySequence.Count) += 1
            Next
            Dim kj As Int16 = 54
        Next

    End Sub

    Private Sub CHeckEGAFilesToolStripMenuItem_Click(sender As Object, e As EventArgs)
        Dim FOlder = New FolderSelectDialog
        If FOlder.ShowDialog = DialogResult.OK Then
            Dim FOlderII = New FolderSelectDialog
            If FOlderII.ShowDialog = DialogResult.OK Then
                Dim str As New System.Text.StringBuilder
                Dim FIles = Szunyi.IO.Directory.GetAllFilesFromFolder(New DirectoryInfo(FOlder.FolderNames.First))
                Dim FIlesII = Szunyi.IO.Directory.GetAllFilesFromFolder(New DirectoryInfo(FOlderII.FolderNames.First))

                Dim AllFiles As New List(Of FileInfo)
                AllFiles.AddRange(FIles)
                AllFiles.AddRange(FIlesII)

                Dim SampleNames = (From x In AllFiles Select x.Name).ToList


                Dim gr = From x In AllFiles Group By d = Szunyi.Text.General.Get_Not_First_Part(x.Name, ".") Into Group

                For Each g In gr
                    Dim gr2 = From x In g.Group Group By x.Name Into Group

                    For Each Item In gr2
                        str.Append(g.d).Append(vbTab).Append(Item.Name).Append(vbTab).Append(Item.Group.Count).AppendLine()
                    Next


                Next
                If str.Length > 0 Then Clipboard.SetText(str.ToString)
            End If

        End If
    End Sub

    Private Sub EGAIIToolStripMenuItem_Click(sender As Object, e As EventArgs)
        Dim FOlder = New FolderSelectDialog
        If FOlder.ShowDialog = DialogResult.OK Then
            Dim FOlderII = New FolderSelectDialog
            If FOlderII.ShowDialog = DialogResult.OK Then
                Dim str As New System.Text.StringBuilder

                Dim AllFiles As New List(Of FileInfo)
                AllFiles.AddRange(Szunyi.IO.Directory.GetAllFilesFromFolder(New DirectoryInfo(FOlder.FolderNames.First)))
                AllFiles.AddRange(Szunyi.IO.Directory.GetAllFilesFromFolder(New DirectoryInfo(FOlderII.FolderNames.First)))

                Dim SampleNames = (From x In AllFiles Select x.Name).ToList
                SampleNames = Szunyi.Text.General.GetFirstParts(SampleNames, "_")
                Dim fq = (From x In AllFiles Where x.Name.Contains("fastq.gz")).ToList

                For Each S In SampleNames
                    Dim k = From x In fq Where x.Name.Contains(S)

                    str.Append(S).Append(vbTab).Append(k.Count).AppendLine()
                Next



                If str.Length > 0 Then Clipboard.SetText(str.ToString)
            End If

        End If
    End Sub

    Private Sub FromDiffDirectoriesToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles FromDiffDirectoriesToolStripMenuItem.Click
        Dim f = New FolderSelectDialog
        If f.ShowDialog = DialogResult.OK Then
            For Each d In f.FolderNames
                Dim cDIr As New DirectoryInfo(d)
                Dim FIles = cDIr.GetFiles

                Dim SAM_BAM_Files = From x In FIles Where x.Extension = ".bam" Or x.Extension = ".sam"

                If SAM_BAM_Files.Count > 0 Then
                    Dim Result_File As New FileInfo(cDIr.Parent.FullName & "\" & cDIr.Name & ".sam")

                    Szunyi.BAM.Bam_Basic_IO.Export.Merge_Sams(SAM_BAM_Files.ToList, Result_File)
                End If
            Next
        End If
    End Sub

    Private Sub FastqstatsToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles FastqstatsToolStripMenuItem.Click
        Dim Files = Szunyi.IO.Files.Filter.SelectFiles(Szunyi.Constants.Files.FastQ)
        Dim str As New System.Text.StringBuilder
        For Each File In Files
            str.Append("fastq-stats ")
            str.Append(Szunyi.IO.Linux.Get_FileName(File))
            Dim nFIle = Szunyi.IO.Files.Get_New_FileName.Append_Before_Extension_wNew_Extension(File, ".tdt")
            'str.Append(" -x ").Append(Szunyi.IO.Linux.Get_FileName(nFIle))
            str.Append(" > ").Append(Szunyi.IO.Linux.Get_FileName(nFIle))
            str.AppendLine()
        Next
        If str.Length > 0 Then
            Clipboard.SetText(str.ToString)
        End If
    End Sub

    Private Sub FastqstatsMergeToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles FastqstatsMergeToolStripMenuItem.Click
        Dim Files = Szunyi.IO.Files.Filter.SelectFiles(Szunyi.Constants.Files.All_TAB_Like)
        Dim str As New System.Text.StringBuilder
        Dim s = Split("number of reads in the fastq file,read length mean,read length stdev,phred scale used ,Number of reads used to generate duplicate read statistics,Number of bases to assess for duplicity ,Number of reads that are duplicates ,Number sequences that are duplicated ,base Quality min,base Quality max,base Quality mean,base Quality stdev,%A,%C,%G,%T,%N,total number of bases", ",")
        Dim header As String = ""
        For Each FIle In Files
            header = header & vbTab & FIle.Name
            Dim Lines = Szunyi.IO.Import.Text.ParseToArray(FIle, vbTab)
            s(0) = s(0) & vbTab & Lines(0)(1)
            s(1) = s(1) & vbTab & Lines(2)(1)
            s(2) = s(2) & vbTab & Lines(3)(1)

            For i1 = 5 To 8
                s(i1 - 2) = s(i1 - 2) & vbTab & Lines(i1)(1)
            Next
            s(7) = s(7) & vbTab & Lines(10)(1)
            For i1 = 24 To 33
                s(i1 - 16) = s(i1 - 16) & vbTab & Lines(i1)(1)
            Next

        Next
        Dim res = header & vbCrLf & Szunyi.Text.General.GetText(s, vbCrLf)
        Clipboard.SetText(res)
        Dim kj As Int16 = 54
    End Sub

    Private Sub TestToolStripMenuItem_Click_1(sender As Object, e As EventArgs) Handles TestToolStripMenuItem.Click
        Dim x As New List(Of ILocation)
        x.Add(Szunyi.Location.Common.GetLocation(1, 10, False))
        x.Add(Szunyi.Location.Common.GetLocation(20, 30, False))
        x.Add(Szunyi.Location.Common.GetLocation(40, 100, False))
        x.Add(Szunyi.Location.Common.GetLocation(50, 100, False))
        x.Add(Szunyi.Location.Common.GetLocation(60, 120, False))
        x.Add(Szunyi.Location.Common.GetLocation(110, 150, False))
        Dim t = Szunyi.Location.OverLapping_Locations.Get_Non_OverLappingGroups(x)


        Dim x1 As New Szunyi.Common.StartAndEnd
        x1.Start = 1234

        Dim res = Szunyi.Util_Helpers.Get_Property_Value(x1, "Start")
        Dim res4 = Szunyi.Util_Helpers.Get_Property_Value(x1, "Endy")
        Dim res2 = Szunyi.Util_Helpers.Get_Property_Value(x1, 0)
        Dim res3 = Szunyi.Util_Helpers.Get_Property_Value(x1, 1)
        Dim kj2 As Int16 = 54
    End Sub

    Private Sub C7BpToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles C7BpToolStripMenuItem.Click
        Dim seqs = Me.GetSeqsFromSelectedList(True)
        If IsNothing(seqs) = True Then Exit Sub
        Dim out As New List(Of Bio.ISequence)
        For Each Seq In seqs

            For i1 = 7 To Seq.Count - 8
                If Seq(i1) = Alphabets.DNA.C Then
                    out.Add(Seq.GetSubSequence(i1 - 7, 15))
                    out.Last.ID = i1

                End If
            Next
        Next
        Dim res As New Dictionary(Of Integer, Dictionary(Of Byte, Integer))
        Dim Header As String = ""
        For i1 = 0 To 14
            Header = Header & vbTab & i1
            res.Add(i1, New Dictionary(Of Byte, Integer))
            res(i1).Add(Bio.Alphabets.DNA.A, 0)
            res(i1).Add(Bio.Alphabets.DNA.C, 0)
            res(i1).Add(Bio.Alphabets.DNA.G, 0)
            res(i1).Add(Bio.Alphabets.DNA.T, 0)
        Next
        For Each Seq In out
            For i1 = 0 To 14
                res(i1)(Seq(i1)) += 1
            Next
        Next
        Header = Header & vbCrLf & ChrW(Bio.Alphabets.DNA.A)
        For i1 = 0 To 14
            Header = Header & vbTab & res(i1)(Bio.Alphabets.DNA.A)
        Next

        Header = Header & vbCrLf & ChrW(Bio.Alphabets.DNA.C)
        For i1 = 0 To 14
            Header = Header & vbTab & res(i1)(Bio.Alphabets.DNA.C)
        Next

        Header = Header & vbCrLf & ChrW(Bio.Alphabets.DNA.G)
        For i1 = 0 To 14
            Header = Header & vbTab & res(i1)(Bio.Alphabets.DNA.G)
        Next

        Header = Header & vbCrLf & ChrW(Bio.Alphabets.DNA.T)
        For i1 = 0 To 14
            Header = Header & vbTab & res(i1)(Bio.Alphabets.DNA.T)
        Next

        Szunyi.IO.Export.SaveSequencesToSingleFasta(out)
    End Sub

    Private Sub MinIonReadIDsToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles MinIonReadIDsToolStripMenuItem.Click
        Dim Bam_Files = Get_Bam_Files(True)
        If IsNothing(Bam_Files) = True Then Exit Sub
        Dim File = Szunyi.IO.Files.Filter.SelectFile(Files.All_TAB_Like)
        If IsNothing(File) = True Then Exit Sub
        Dim Lines = Szunyi.IO.Import.Text.Parse(File)
        Dim ReadIDs = From x In Lines Where x.Length = 36
        Dim DReadIDs = ReadIDs.Distinct.ToList
        DReadIDs.Sort()
        Dim Filtered As Integer = 0
        For Each File In Bam_Files
            Dim nFIle = Szunyi.IO.Files.Get_New_FileName.Append_Before_Extension_wNew_Extension(File, "_Deleted.sam")
            Using Export As New Szunyi.BAM.Bam_Basic_IO.Export(nFIle, Szunyi.BAM.Bam_Basic_IO.Headers.Get_Header(File))
                For Each Sam In Szunyi.BAM.Bam_Basic_IO.Import.Parse(File)
                    Dim Index = DReadIDs.BinarySearch(Sam.QName)
                    If Index > -1 Then
                        ' FOund Not Write It, Do nothing
                        Filtered += 1
                    Else
                        Export.Write(Sam)
                    End If
                Next
            End Using

        Next
        Dim kj As Integer = 43
    End Sub

    Private Sub PreciseToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles PreciseToolStripMenuItem.Click
        Dim Seqs = Me.GetSeqsFromSelectedList(True)
        If IsNothing(Seqs) Then Exit Sub
        Dim Files = Get_Bam_Files()
        If IsNothing(Files) = True Then Exit Sub
        Dim Header = (From x In Files Select x.Name).ToList
        Header.Insert(0, "Transcript")
        Header.Insert(1, "Location")
        Header.Insert(2, "Key")
        Header.Insert(3, "PAS Seq 10 bp")
        Header.Insert(4, "nof A/T")
        Header.Insert(5, "Signal Sequence")
        Header.Insert(6, "Signal Position")
        Header.Insert(7, "Signal DIstance From Optimal")
        Dim str As New System.Text.StringBuilder
        str.Append(Szunyi.Text.General.GetText(Header, vbTab)).AppendLine()
        Dim Feats = Select_Features()
        Dim BL_Feats = Szunyi.Location.Basic_Location_Manipulation.Get_Basic_LocationS(Feats)
        BL_Feats = (From x In BL_Feats Order By x.Location.LocationStart).ToList
        Dim Finder As New Szunyi.Location.Basic_Location_Finder(BL_Feats, True)
        Dim Index As Integer = 0
        Dim Ls As New List(Of String)
        For Each File In Files
            Dim Res As New Dictionary(Of FeatureItem, Integer)
            For Each Item In BL_Feats
                Res.Add(Item.Obj, 0)
            Next
            For Each SAM In Szunyi.BAM.Bam_Basic_IO.Import.Parse(File)
                Dim l = Szunyi.Location.Common.GetLocation(SAM)
                Dim BL = Szunyi.Location.Basic_Location_Manipulation.Get_Basic_Location(l)
                Dim TSSs = Finder.Find_Items_byLoci(BL, 20, Sort_Locations_By.TSS)
                Dim PAss = Finder.Find_Items_byLoci(BL, 20, Sort_Locations_By.PAS)
                Dim COmmmon = TSSs.Intersect(PAss)
                If COmmmon.Count > 0 Then
                    For Each c In COmmmon
                        Res(c.Obj) += 1
                    Next
                End If
            Next ' FIle
            If Index = 0 Then
                Index += 1
                For Each Item In Res
                    Dim F As FeatureItem = Item.Key
                    Dim str1 As New System.Text.StringBuilder
                    str1.Append(F.Label).Append(vbTab)
                    str1.Append(Szunyi.Location.Common.GetLocationString(F.Location)).Append(vbTab)
                    str1.Append(F.Key).Append(vbTab)
                    Dim Last10 = Szunyi.Sequences.SequenceManipulation.Get_Sub_Sequence.Before(Seqs.First, F.Location, Sort_Locations_By.PAS, 10)
                    str1.Append(Last10.ConvertToString).Append(vbTab)

                    If F.Location.IsComplementer = True Then
                        Dim jk = Szunyi.mRNA.PolyA.False_polyT(Seqs.First, F.Location, 10, mRNA.False_PolyAT_Discovering.From_Beginning, 0, Bio.Alphabets.DNA.A)
                        str1.Append(jk).Append(vbTab)
                    Else
                        Dim kj = Szunyi.mRNA.PolyA.False_polyA(Seqs.First, F.Location, 10, mRNA.False_PolyAT_Discovering.From_Beginning, 0, Bio.Alphabets.DNA.A)
                        str1.Append(kj).Append(vbTab)
                    End If
                    Dim x = Szunyi.DNA.PA.Get_PolyA_Signal(Seqs.First, F, 50, -22)

                    str1.Append(x.Get_Best.ToString)
                    Ls.Add(str1.ToString & vbTab & Item.Value)
                Next
            Else
                Dim i1 As Integer = 0
                For Each Item In Res

                    Ls(i1) = Ls(i1) & vbTab & Item.Value
                    i1 += 1
                Next
            End If

        Next
        Dim k = str.ToString & vbCrLf & Szunyi.Text.General.GetText(Ls, vbCrLf)
        Clipboard.SetText(k)
    End Sub

    Private Sub DefaultToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles DefaultToolStripMenuItem.Click
        Dim Seqs = Me.GetSeqsFromSelectedList(True)
        If IsNothing(Seqs) = True Then Exit Sub
        Dim cSeqs = Szunyi.Sequences.SequenceManipulation.Common.CloneSeqs(Seqs)
        Dim Feats = Select_Custom_Features(cSeqs)
        If IsNothing(Feats) = True OrElse Feats.Count = 0 Then Exit Sub
        Dim res = Szunyi.mRNA.Transcript.Transcript.ReName_Default(cSeqs.First, Feats)
    End Sub

    Private Sub MergedBarcodedDastqToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles MergedBarcodedDastqToolStripMenuItem.Click
        Dim Folder As New FolderSelectDialog
        If Folder.ShowDialog = DialogResult.OK Then

            For Each f In Folder.FolderNames
                Dim str As New System.Text.StringBuilder
                Dim d As New DirectoryInfo(f)
                For Each sd In d.GetDirectories
                    Dim Files = sd.GetFiles.ToList
                    Dim nFIle As New FileInfo(f & "\" & sd.Name & ".fastq")
                    Szunyi.IO.Files.Move_Copy.MergeFiles(Files, nFIle, False)
                    For Each fi In Files
                        Str.Append(fi.FullName & vbTab & nFIle.FullName).AppendLine()
                    Next
                Next
                Dim loGFIle As New FileInfo(d.FullName & "\Merging.log")
                Szunyi.IO.Export.SaveText(str.ToString, loGFIle)
                Dim kj As Int16 = 54
            Next
        End If
    End Sub

    Private Sub MergedBarcodedFromFoldersToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles MergedBarcodedFromFoldersToolStripMenuItem.Click
        Dim Folder As New FolderSelectDialog
        If Folder.ShowDialog = DialogResult.OK Then
            Dim AllFiles As New List(Of FileInfo)
            For Each f In Folder.FolderNames
                Dim d As New DirectoryInfo(f)
                AllFiles.AddRange(d.GetFiles)
            Next
            Dim OutPut As New FolderSelectDialog
            If OutPut.ShowDialog = DialogResult.OK Then
                Dim gr = From x In AllFiles Group By x.Name Into Group

                For Each g In gr
                    Dim nFile As New FileInfo(OutPut.FolderNames.First & "\" & g.Name)
                    Szunyi.IO.Files.Move_Copy.MergeFiles(g.Group.ToList, nFile, False)

                Next
            End If

        End If
    End Sub

    Private Sub FullNotFullToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles FullNotFullToolStripMenuItem.Click
        Dim Seqs = Me.GetSeqsFromSelectedList(True)
        If IsNothing(Seqs) Then Exit Sub
        Dim Files = Get_Bam_Files()
        If IsNothing(Files) = True Then Exit Sub

        Dim Feats = Select_Features()
        Dim BL_Feats = Szunyi.Location.Basic_Location_Manipulation.Get_Basic_LocationS(Feats)
        BL_Feats = (From x In BL_Feats Order By x.Location.LocationStart).ToList
        Dim Finder As New Szunyi.Location.Basic_Location_Finder(BL_Feats, True)
        Dim Finder_woOrientation As New Szunyi.Location.Basic_Location_Finder(BL_Feats, False)
        Dim Index As Integer = 0
        Dim str As New System.Text.StringBuilder
        str.Append("FileName").Append(vbTab)
        str.Append("nof Complete Reads wOrientation").Append(vbTab)
        str.Append("Nof Partial Reads wOrientation").Append(vbTab)
        str.Append("nof Complete Reads woOrientation").Append(vbTab)
        str.Append("Nof Partial Reads woOrientation").AppendLine()
        Dim Ls As New List(Of String)
        '     Dim MAx = MyInputBox.GetInteger("Max nof reads to Investigate")
        For Each File In Files

            Dim Complete_FIle_wOrientation = Szunyi.IO.Files.Get_New_FileName.Append_Before_Extension_wNew_Extension(File, "_Full_wOrientation.sam")
            Dim Partial_FIle_wOrientation = Szunyi.IO.Files.Get_New_FileName.Append_Before_Extension_wNew_Extension(File, "_Partial_wOrientation.sam")
            Dim Complete_FIle_woOrientation = Szunyi.IO.Files.Get_New_FileName.Append_Before_Extension_wNew_Extension(File, "_Full_woOrientation.sam")
            Dim Partial_FIle_woOrientation = Szunyi.IO.Files.Get_New_FileName.Append_Before_Extension_wNew_Extension(File, "_Partial_woOrientation.sam")
            Dim Header = Szunyi.BAM.Bam_Basic_IO.Headers.Get_Header(File)
            Dim nof_Complete_wOrientation As Integer = 0
            Dim nof_Partial_wOrientation As Integer = 0
            Dim nof_Complete_woOrientation As Integer = 0
            Dim nof_Partial_woOrientation As Integer = 0
            Using Complete As New Szunyi.BAM.Bam_Basic_IO.Export(Complete_FIle_wOrientation, Header)
                Using Partial_B As New Szunyi.BAM.Bam_Basic_IO.Export(Partial_FIle_wOrientation, Header)
                    Using Complete_woOrientation As New Szunyi.BAM.Bam_Basic_IO.Export(Complete_FIle_woOrientation, Header)
                        Using Partial_Be_woOrientation As New Szunyi.BAM.Bam_Basic_IO.Export(Partial_FIle_woOrientation, Header)
                            Dim nof As Integer = 0
                            For Each SAM In Szunyi.BAM.Bam_Basic_IO.Import.Parse(File)
                                If SAM.Flag <> Bio.IO.SAM.SAMFlags.UnmappedQuery Then
                                    nof += 1
                                    '   If nof > MAx Then Exit For
                                    Dim l = Szunyi.Location.Common.GetLocation(SAM)
                                    Dim BL = Szunyi.Location.Basic_Location_Manipulation.Get_Basic_Location(l)
                                    Dim TSSs = Finder.Find_Items_byLoci(BL, 20, Sort_Locations_By.TSS)
                                    Dim PAss = Finder.Find_Items_byLoci(BL, 20, Sort_Locations_By.PAS)
                                    Dim COmmmon = TSSs.Intersect(PAss)
                                    If COmmmon.Count > 0 Then
                                        Complete.Write(SAM)
                                        nof_Complete_wOrientation += 1
                                    Else
                                        Partial_B.Write(SAM)
                                        nof_Partial_wOrientation += 1
                                    End If

                                    TSSs = Finder_woOrientation.Find_Items_byLoci(BL, 20, Sort_Locations_By.LS)
                                    PAss = Finder_woOrientation.Find_Items_byLoci(BL, 20, Sort_Locations_By.LE)
                                    COmmmon = TSSs.Intersect(PAss)
                                    If COmmmon.Count > 0 Then
                                        Complete_woOrientation.Write(SAM)
                                        nof_Complete_woOrientation += 1
                                    Else
                                        Partial_Be_woOrientation.Write(SAM)
                                        nof_Partial_woOrientation += 1
                                    End If
                                End If
                            Next ' FIle
                        End Using
                    End Using
                End Using
            End Using


            str.Append(File.Name).Append(vbTab)
            str.Append(nof_Complete_wOrientation).Append(vbTab)
            str.Append(nof_Partial_wOrientation).Append(vbTab)
            str.Append(nof_Complete_woOrientation).Append(vbTab)
            str.Append(nof_Partial_woOrientation).AppendLine()
        Next
        If Files.Count > 0 And str.Length > 45 Then
            str.Length -= 2
            Szunyi.IO.Export.SaveText(str.ToString, New FileInfo(Files.First.DirectoryName & "\Complete-Partial.tsv"))
        End If
    End Sub
    Private Sub ToLocationTabWIntronsToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles ToLocationTabWIntronsToolStripMenuItem.Click
        Dim Bam_Files = Get_Bam_Files("Select Bam FIles")
        If IsNothing(Bam_Files) = True Then Exit Sub
        For Each FIle In Bam_Files
            Using sw As New StreamWriter(FIle.FullName & ".loc")
                Dim FIrst As Boolean = True
                For Each BL In Szunyi.BAM.Bam_Basic_IO.Import.Parse_Into_Basic_Locations(FIle)
                    If FIrst = True Then
                        sw.Write(BL.Location.Accession & vbTab)
                        sw.Write(Szunyi.Location.Common.GetLocationString(BL.Location) & vbTab)
                        sw.Write(Szunyi.Location.Common.Get_LocationString_wIntron(BL.Location))
                        FIrst = False
                    Else
                        sw.WriteLine()
                        sw.Write(BL.Location.Accession & vbTab)
                        sw.Write(Szunyi.Location.Common.GetLocationString(BL.Location) & vbTab)
                        sw.Write(Szunyi.Location.Common.Get_LocationString_wIntron(BL.Location))
                    End If
                Next
            End Using
        Next
    End Sub
    Private Sub ToLocationFromSubDirectoriesToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles ToLocationFromSubDirectoriesToolStripMenuItem.Click
        Dim x1 As New FolderSelectDialog
        Dim ALL_Files As New List(Of FileInfo)
        If x1.ShowDialog = DialogResult.OK Then
            For Each d In x1.FolderNames
                ALL_Files.AddRange(Szunyi.IO.Directory.GetAllFilesFromFolder_Recursive(New DirectoryInfo(d)))
            Next
            Dim Sel_Files = From x In ALL_Files Where x.Extension = ".sam" Or x.Extension = ".bam"

            Szunyi.IO.Export.Location(Sel_Files)
        End If
    End Sub
    Private Sub ToLocationToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles ToLocationToolStripMenuItem.Click
        Dim Bam_Files = Get_Bam_Files("Select Bam FIles")
        If IsNothing(Bam_Files) = True Then Exit Sub
        Szunyi.IO.Export.Location(Bam_Files)
    End Sub


    'Porechop stat


    Private Sub BYNofToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles BYNofToolStripMenuItem.Click
        Dim nof = MyInputBox.GetInteger("Select Nof", 500000)
        Dim Files = Szunyi.IO.Files.Filter.SelectFiles(Szunyi.Constants.Files.SequenceFileTypesToImport)
        For Each FIle In Files
            Dim part As Integer = 0
            Dim Out As New List(Of Bio.ISequence)
            For Each Seq In Szunyi.IO.Import.Sequence.GetOnyByONe(FIle)
                Out.Add(Seq)
                If Out.Count = nof Then
                    Dim nFIle = Szunyi.IO.Files.Get_New_FileName.Append_Before_Extension(FIle, "_" & part)

                    Szunyi.IO.Export.SaveSequencesToSingleFastQ(Out, nFIle)
                    part += 1
                    Out.Clear()
                End If
            Next
            If Out.Count > 0 Then
                Dim nFIle = Szunyi.IO.Files.Get_New_FileName.Append_Before_Extension(FIle, "_" & part)

                Szunyi.IO.Export.SaveSequencesToSingleFastQ(Out, nFIle)
            End If
        Next

    End Sub

    Private Sub KmersToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles KmersToolStripMenuItem.Click
        Dim nof = MyInputBox.GetInteger("Select length of KMER", 500000)
        Dim Files = Szunyi.IO.Files.Filter.SelectFiles(Szunyi.Constants.Files.SequenceFileTypesToImport)
        Dim str As New System.Text.StringBuilder
        For Each file In Files
            Dim res As New Dictionary(Of String, List(Of String))
            For Each Seq In Szunyi.IO.Import.Sequence.GetOnyByONe(Files)
                For i1 = 0 To Seq.Count - 1 - nof
                    Dim s = Seq.GetSubSequence(i1, nof).ConvertToString
                    If res.ContainsKey(s) = False Then res.Add(s, New List(Of String))
                    res(s).Add(s)
                Next
            Next

            For Each Item In res

                Dim t = From x In Item.Value Group By x Into Group
                For Each t1 In t
                    str.Append(t1.x).Append(vbTab).Append(file.Name).Append(vbTab).Append(t1.Group.Count).AppendLine()
                Next

            Next
        Next
        Szunyi.IO.Export.SaveText(str.ToString)
    End Sub

    Private Sub SetOrientationInBamFIlesToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles SetOrientationInBamFIlesToolStripMenuItem.Click
        Dim Files = Szunyi.IO.Files.Filter.SelectFiles(Szunyi.Constants.Files.PoreCHop)
        If IsNothing(Files) = True OrElse Files.Count = 0 Then Exit Sub
        Dim Max_Nof_Mismatch = MyInputBox.GetInteger("Enter max nof MisMatch in polyT")
        Dim PoreChop_Result = Szunyi.Console.Porechop.Porechop.Get_Short_Porechop_Result(Files, Max_Nof_Mismatch)
        Files = Szunyi.IO.Files.Filter.SelectFiles(Szunyi.Constants.Files.SAM_BAM)
        If IsNothing(Files) = True Then Exit Sub
        Dim x As New Szunyi.Console.Porechop.Short_Result
        Dim comp As New Szunyi.Console.Porechop.Short_Result_ByReadID
        Dim str_SAM_to_BAm_Sort_Index As New System.Text.StringBuilder
        Dim For_PAS_Range = MyInputBox.GetInteger("Set PAS range", 5)
        Dim For_TSS_Range = MyInputBox.GetInteger("Set TSS range", 3)


        For Each FIle In Files
            Dim DIr As New DirectoryInfo(FIle.DirectoryName & "\" & FIle.Name.Replace(FIle.Extension, ""))
            DIr.Create()
            Dim Header = Szunyi.BAM.Bam_Basic_IO.Headers.Get_Header(FIle)
            Dim f_pAT As New FileInfo(DIr.FullName & "\polyAT_" & FIle.Name.Replace(FIle.Extension, ".sam"))
            Dim f_All As New FileInfo(DIr.FullName & "\All_" & FIle.Name.Replace(FIle.Extension, ".sam"))
            Dim f_None As New FileInfo(DIr.FullName & "\None_" & FIle.Name.Replace(FIle.Extension, ".sam"))
            Dim f_Both As New FileInfo(DIr.FullName & "\Both_" & FIle.Name.Replace(FIle.Extension, ".sam"))
            Dim f_For_TSS As New FileInfo(DIr.FullName & "\For_TSS_" & FIle.Name.Replace(FIle.Extension, ".sam"))
            Dim f_For_PAS As New FileInfo(DIr.FullName & "\For_PAS_" & FIle.Name.Replace(FIle.Extension, ".sam"))
            Dim f_For_TSS_In_Range As New FileInfo(DIr.FullName & "\For_TSS_In_Range_" & FIle.Name.Replace(FIle.Extension, ".sam"))
            Dim f_For_PAS_In_Range As New FileInfo(DIr.FullName & "\For_PAS_In_Range_" & FIle.Name.Replace(FIle.Extension, ".sam"))
            Dim f_For_PAS_TSS_Common As New FileInfo(DIr.FullName & "\TSS_PAS_in_Range_" & FIle.Name.Replace(FIle.Extension, ".sam"))

            Dim f_For_PAS_Not_In_Range As New FileInfo(DIr.FullName & "\For_PAS_Not_In_Range_" & FIle.Name.Replace(FIle.Extension, ".sam"))
            Dim f_For_TSS_Not_In_Range As New FileInfo(DIr.FullName & "\For_TSS_Not_In_Range_" & FIle.Name.Replace(FIle.Extension, ".sam"))
            str_SAM_to_BAm_Sort_Index.Append(Szunyi.Linux.SamTools.Sam_to_Bam_Sort_Index(f_pAT))
            str_SAM_to_BAm_Sort_Index.Append(Szunyi.Linux.SamTools.Sam_to_Bam_Sort_Index(f_All))
            str_SAM_to_BAm_Sort_Index.Append(Szunyi.Linux.SamTools.Sam_to_Bam_Sort_Index(f_None))
            str_SAM_to_BAm_Sort_Index.Append(Szunyi.Linux.SamTools.Sam_to_Bam_Sort_Index(f_Both))
            str_SAM_to_BAm_Sort_Index.Append(Szunyi.Linux.SamTools.Sam_to_Bam_Sort_Index(f_For_TSS))
            str_SAM_to_BAm_Sort_Index.Append(Szunyi.Linux.SamTools.Sam_to_Bam_Sort_Index(f_For_PAS))
            str_SAM_to_BAm_Sort_Index.Append(Szunyi.Linux.SamTools.Sam_to_Bam_Sort_Index(f_For_PAS_TSS_Common))
            str_SAM_to_BAm_Sort_Index.Append(Szunyi.Linux.SamTools.Sam_to_Bam_Sort_Index(f_For_PAS_In_Range))
            str_SAM_to_BAm_Sort_Index.Append(Szunyi.Linux.SamTools.Sam_to_Bam_Sort_Index(f_For_TSS_In_Range))
            str_SAM_to_BAm_Sort_Index.Append(Szunyi.Linux.SamTools.Sam_to_Bam_Sort_Index(f_For_PAS_Not_In_Range))
            str_SAM_to_BAm_Sort_Index.Append(Szunyi.Linux.SamTools.Sam_to_Bam_Sort_Index(f_For_TSS_Not_In_Range))
            Using polyAT As New Szunyi.BAM.Bam_Basic_IO.Export(f_pAT, Header)
                Using All As New Szunyi.BAM.Bam_Basic_IO.Export(f_All, Header)
                    Using None As New Szunyi.BAM.Bam_Basic_IO.Export(f_None, Header)
                        Using Both As New Szunyi.BAM.Bam_Basic_IO.Export(f_Both, Header)
                            Using For_TSS As New Szunyi.BAM.Bam_Basic_IO.Export(f_For_TSS, Header)
                                Using For_PAS As New Szunyi.BAM.Bam_Basic_IO.Export(f_For_PAS, Header)
                                    Using For_PAS_TSS_In_Range As New Szunyi.BAM.Bam_Basic_IO.Export(f_For_PAS_TSS_Common, Header)
                                        Using For_TSS_In_Range As New Szunyi.BAM.Bam_Basic_IO.Export(f_For_TSS_In_Range, Header)
                                            Using For_PAS_In_Range As New Szunyi.BAM.Bam_Basic_IO.Export(f_For_PAS_In_Range, Header)
                                                Using For_PAS_Not_In_Range As New Szunyi.BAM.Bam_Basic_IO.Export(f_For_PAS_Not_In_Range, Header)
                                                    Using For_TSS_Not_In_Range As New Szunyi.BAM.Bam_Basic_IO.Export(f_For_TSS_Not_In_Range, Header)
                                                        For Each sam In Szunyi.BAM.Bam_Basic_IO.Import.Parse(FIle)
                                                            x.ReadID = sam.QName
                                                            Dim Index = PoreChop_Result.BinarySearch(x, comp)
                                                            If Index > -1 Then
                                                                Dim cPoreChop = PoreChop_Result(Index)
                                                                Dim GMAP_Seq = sam.QuerySequence.ConvertToString
                                                                Dim GMAP_RC_Seq = sam.QuerySequence.GetReverseComplementedSequence.ConvertToString
                                                                Dim Is_TSS_InRange As Boolean = False
                                                                Dim Is_PAS_InRange As Boolean = False
                                                                Dim FiveS = Szunyi.BAM.SAM_Manipulation.Common.Get_First_S(sam, Nothing)
                                                                Dim ThreeS = Szunyi.BAM.SAM_Manipulation.Common.Get_Last_S(sam, Nothing)
                                                                Select Case cPoreChop.Orientation
                                                                    Case Szunyi.Transcipts.Orientation.Type.both
                                                                        Both.Write(sam)
                                                                    Case Transcipts.Orientation.Type.fw
                                                                        If cPoreChop.Five_Prime_Seq <> "" AndAlso GMAP_Seq.StartsWith(cPoreChop.Five_Prime_Seq) Then
                                                                            sam.Flag = 0
                                                                            If IsNothing(cPoreChop.TSS_Location_Five) = False Then
                                                                                Dim distFromEnd = cPoreChop.TSS_Location_Five.LocationEnd ' Modified in impoort
                                                                                If FiveS - For_TSS_Range <= distFromEnd And FiveS + For_TSS_Range >= distFromEnd Then
                                                                                    Is_TSS_InRange = True
                                                                                End If
                                                                            End If
                                                                            If IsNothing(cPoreChop.PAS_Location_Three) = False Then
                                                                                Dim distFromEnd = 150 - cPoreChop.PAS_Location_Three.LocationStart ' Not modified in import LS should increase 1
                                                                                If ThreeS - For_PAS_Range <= distFromEnd And ThreeS + For_PAS_Range >= distFromEnd Then
                                                                                    Is_PAS_InRange = True
                                                                                End If
                                                                            End If
                                                                        ElseIf cPoreChop.Five_Prime_Seq <> "" AndAlso GMAP_RC_Seq.StartsWith(cPoreChop.Five_Prime_Seq) Then
                                                                            If IsNothing(cPoreChop.TSS_Location_Five) = False Then
                                                                                Dim distFromEnd = 150 - cPoreChop.TSS_Location_Five.LocationStart - 1
                                                                                If ThreeS - For_TSS_Range <= cPoreChop.TSS_Location_Five.LocationEnd And ThreeS + For_TSS_Range >= cPoreChop.TSS_Location_Five.LocationEnd Then
                                                                                    Is_TSS_InRange = True
                                                                                End If
                                                                            End If
                                                                            If IsNothing(cPoreChop.PAS_Location_Three) = False Then
                                                                                Dim distFromEnd = 150 - cPoreChop.PAS_Location_Three.LocationStart
                                                                                If FiveS - For_PAS_Range <= 150 - distFromEnd And FiveS + For_PAS_Range >= distFromEnd Then
                                                                                    Is_PAS_InRange = True
                                                                                End If
                                                                            End If
                                                                            sam.Flag = 16 'valid
                                                                        End If
                                                                        polyAT.Write(sam)
                                                                    Case Transcipts.Orientation.Type.rev
                                                                        If cPoreChop.Five_Prime_Seq <> "" AndAlso GMAP_Seq.StartsWith(cPoreChop.Five_Prime_Seq) Then
                                                                            If IsNothing(cPoreChop.TSS_Location_Three) = False Then
                                                                                Dim distFromEnd = 150 - cPoreChop.TSS_Location_Three.LocationStart
                                                                                If ThreeS - For_TSS_Range <= distFromEnd And ThreeS + For_TSS_Range >= distFromEnd Then
                                                                                    Is_TSS_InRange = True
                                                                                End If
                                                                            End If
                                                                            If IsNothing(cPoreChop.PAS_Location_Five) = False Then
                                                                                Dim distFromEnd = cPoreChop.PAS_Location_Five.LocationEnd
                                                                                If FiveS - For_PAS_Range <= distFromEnd And FiveS + For_PAS_Range >= distFromEnd Then
                                                                                    Is_PAS_InRange = True
                                                                                End If
                                                                            End If
                                                                            sam.Flag = 16
                                                                        ElseIf cPoreChop.Five_Prime_Seq <> "" AndAlso GMAP_RC_Seq.StartsWith(cPoreChop.Five_Prime_Seq) Then
                                                                            If IsNothing(cPoreChop.TSS_Location_Three) = False Then
                                                                                Dim distFromEnd = 150 - cPoreChop.TSS_Location_Three.LocationStart
                                                                                If ThreeS - For_TSS_Range <= distFromEnd And ThreeS + For_TSS_Range >= distFromEnd Then
                                                                                    Is_TSS_InRange = True
                                                                                End If
                                                                            End If
                                                                            If IsNothing(cPoreChop.PAS_Location_Five) = False Then
                                                                                Dim distFromEnd = cPoreChop.PAS_Location_Five.LocationEnd
                                                                                If FiveS - For_TSS_Range <= distFromEnd And FiveS + For_TSS_Range >= distFromEnd Then
                                                                                    Is_PAS_InRange = True
                                                                                End If
                                                                            End If
                                                                            sam.Flag = 0
                                                                        End If


                                                                        polyAT.Write(sam)
                                                                    Case Transcipts.Orientation.Type.unknown
                                                                        None.Write(sam)
                                                                End Select
                                                                If cPoreChop.For_Pas = True Then For_PAS.Write(sam)

                                                                If cPoreChop.For_TSS = True Then For_TSS.Write(sam)

                                                                If Is_PAS_InRange = True AndAlso Is_TSS_InRange = True Then For_PAS_TSS_In_Range.Write(sam)

                                                                If Is_TSS_InRange = True Then For_TSS_In_Range.Write(sam)

                                                                If Is_PAS_InRange = True Then For_PAS_In_Range.Write(sam)

                                                                If Is_TSS_InRange = False AndAlso cPoreChop.For_TSS = True Then For_TSS_Not_In_Range.Write(sam)

                                                                If Is_PAS_InRange = False AndAlso cPoreChop.For_Pas = True Then For_PAS_Not_In_Range.Write(sam)
                                                            Else
                                                                Dim kj As Int16 = 65
                                                            End If
                                                            All.Write(sam)
                                                        Next
                                                    End Using
                                                End Using
                                            End Using
                                        End Using
                                    End Using
                                End Using
                            End Using
                        End Using
                    End Using
                End Using
            End Using
        Next
        Szunyi.IO.Export.SaveText(str_SAM_to_BAm_Sort_Index.ToString, New FileInfo(Files.First.DirectoryName & "\samtools.sh"))
    End Sub

    Private Sub RenameReadIDsToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles RenameReadIDsToolStripMenuItem.Click
        Dim FIles = Get_Bam_Files()
        If IsNothing(FIles) = True Then Exit Sub
        For Each File In FIles
            Dim Header = Szunyi.BAM.Bam_Basic_IO.Headers.Get_Header(File)
            Dim nFIle = Szunyi.IO.Files.Get_New_FileName.Append_Before_Extension(File, "_wnewIDs")
            Using sw As New Szunyi.BAM.Bam_Basic_IO.Export(nFIle, Header)
                Dim Index As Integer = 0
                For Each SAM In Szunyi.BAM.Bam_Basic_IO.Import.Parse(File)
                    Index += 1
                    SAM.QName = SAM.QName & "_" & Index
                    sw.Write(SAM)
                Next
            End Using


        Next
    End Sub

    Private Sub FastQCToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles FastQCToolStripMenuItem.Click
        Dim Files = Szunyi.IO.Files.Filter.SelectFiles(Constants.Files.SAM_BAM_Fastq)
        If IsNothing(Files) = True Then Exit Sub
        Dim str As New System.Text.StringBuilder
        For Each FIle In Files
            str.Append("./fastqc ").Append(Szunyi.IO.Linux.Get_FileName(FIle)).Append(vbLf)
        Next
        Clipboard.SetText(str.ToString)
    End Sub



    Private Sub StatisticToolStripMenuItem1_Click(sender As Object, e As EventArgs) Handles StatisticToolStripMenuItem1.Click
        Dim FIles = Szunyi.IO.Files.Filter.SelectFiles(Constants.Files.PoreCHop)
        If FIles.Count = 0 Then Exit Sub
        Dim Adapters = Szunyi.Console.Porechop.Porechop.Get_Adapters(FIles.First)
        Dim f1 As New CheckBoxForStringsFull(Adapters, -1, "Select Interesting 5' Adapters")
        If f1.ShowDialog <> DialogResult.OK Then Exit Sub

        Dim Five_Prime_Adapters = f1.SelectedStrings
        Dim Selected_Adapters As New List(Of String)
        Dim f2 As New CheckBoxForStringsFull(Adapters, -1, "Select Interesting 3' Adapters")
        If f2.ShowDialog <> DialogResult.OK Then Exit Sub
        Dim Three_Prime_Adapters = f2.SelectedStrings

        Dim For_TSS As New List(Of String)
        If Five_Prime_Adapters.Count < 2 Then
            For_TSS = Five_Prime_Adapters
        Else
            Dim f3 As New CheckBoxForStringsFull(Five_Prime_Adapters, -1, "Select Interesting For TSS")
            If f3.ShowDialog <> DialogResult.OK Then Exit Sub
            For_TSS = f3.SelectedStrings
        End If

        Dim For_PAS As New List(Of String)
        If Three_Prime_Adapters.Count < 2 Then
            For_PAS = Three_Prime_Adapters
        Else
            Dim f4 As New CheckBoxForStringsFull(Three_Prime_Adapters, -1, "Select Interesting For PAS")
            If f4.ShowDialog <> DialogResult.OK Then Exit Sub
            For_PAS = f4.SelectedStrings
        End If


        Dim Minimum_Full_Score = MyInputBox.GetInteger("Set Minimum Final Score", 70)
        Adapters.Clear()
        Adapters.AddRange(Five_Prime_Adapters)
        Adapters.AddRange(Three_Prime_Adapters)
        Dim IsMiddleAdpapter As Boolean = False
        For Each FIle In FIles
            Dim SortBy As New Szunyi.Console.Porechop.ByReadID
            Dim ls As New List(Of Szunyi.Console.Porechop.Result)
            IsMiddleAdpapter = False
            For Each s In Szunyi.IO.Import.Text.Ignore_Before_Contains_Group_By_Not_Start(FIle, "Trimming adapters from read ends", " ")
                If s.Count <> 0 Then
                    If IsMiddleAdpapter = False Then
                        Dim x As New Szunyi.Console.Porechop.Result(s, Adapters, Minimum_Full_Score)
                        Szunyi.Console.Porechop.Porechop.Set_Orientations(x, Five_Prime_Adapters, Three_Prime_Adapters)
                        Szunyi.Console.Porechop.Porechop.Set_TSS_PAS(x, For_TSS, For_PAS)
                        ls.Add(x)
                        For Each line In s
                            If line.Contains("Discarding reads containing middle adapters") Then
                                Dim jk As Int16 = 54
                                ls.Sort(SortBy)
                                IsMiddleAdpapter = True
                            End If
                        Next
                    Else
                        Exit For
                        Dim t = Szunyi.Console.Porechop.Porechop.Get_Midle_Adapters(s)
                        Dim Key = Split(t.Key, "_").First
                        Dim p1 As New Szunyi.Console.Porechop.Result(Key)
                        Dim Index = ls.BinarySearch(p1, SortBy)
                        If Index < 0 Then
                            Dim jjjj As Int16 = 54
                        Else
                            For Each Item In t.Value
                                If (ls(Index)).Middle_Prime_Adaptors.ContainsKey(Key) Then
                                    ls(Index).Middle_Prime_Adaptors(Key) = Item
                                End If
                            Next

                            Dim kkkkk As Int16 = 54
                        End If
                        Dim jju As Boolean = True
                    End If


                End If

            Next

            '      Dim stat = ls.First.Get_Header & vbCrLf & Szunyi.Console.Porechop.Statistic.ByBarCodeAndAdaptor(ls)
            Dim stat2 = Szunyi.Console.Porechop.Statistic.ByBarCodeAndAdaptor_Orientation(ls, Adapters, Minimum_Full_Score)
            Dim nfile = Szunyi.IO.Files.Get_New_FileName.Append_Before_Extension_wNew_Extension(FIle, "_" & Minimum_Full_Score & "_PoreChop_Adaptor_Stat.tsv")
            Szunyi.IO.Export.SaveText(stat2, nfile)
            Dim nFile2 = Szunyi.IO.Files.Get_New_FileName.Append_Before_Extension_wNew_Extension(FIle, "_" & Minimum_Full_Score & "_PoreChop_Adaptor_Orientations.porechop")
            Using t As New StreamWriter(nFile2.FullName)
                Dim header As String = "#ReadID" & vbTab & "Five_Prime_Seq" & vbTab & "Three_Prime_Seq" & vbTab & "BarCode" & vbTab & "Final Orientation" & vbTab & "For TSS" & vbTab & "ForPAS" & vbTab & "TSS_Location_In_Five_Prime" & vbTab & "TSS_Location_In_three_Prime" _
 & vbTab & "PAS_Location_In_Five_Prime" & vbTab & "PAS_Location_In_three_Prime"
                For Each Item In Adapters
                    header = header & vbTab & "Orientation By Adapter:" & Item
                Next
                header = header & vbCrLf & "# Five Prime Adapters:" & Szunyi.Text.General.GetText(Five_Prime_Adapters, ",")
                header = header & vbCrLf & "# Three Prime Adapters:" & Szunyi.Text.General.GetText(Three_Prime_Adapters, ",")
                header = header & vbCrLf & "# TSS Adapters:" & Szunyi.Text.General.GetText(For_TSS, ",")
                header = header & vbCrLf & "# PAS Adapters:" & Szunyi.Text.General.GetText(For_PAS, ",")
                Dim jj = Szunyi.Util_Helpers.Get_All_Enum_Names_Values(Of Szunyi.Transcipts.Orientation.Type)(Szunyi.Transcipts.Orientation.Type.both)
                header = header & vbCrLf & "#Orientation Enums:" & Szunyi.Text.General.GetText(jj, ";")
                t.Write(header)
                For Each Item In ls
                    t.WriteLine()

                    t.Write(Split(Item.ReadID, " ").First)
                    t.Write(vbTab)
                    t.Write(Item.Five_Prime_Seq)
                    t.Write(vbTab)
                    t.Write(Item.Three_Prime_Seq)
                    t.Write(vbTab)
                    t.Write(Item.final_barcode)
                    t.Write(vbTab)
                    t.Write(Item.Final_Orientation)
                    t.Write(vbTab)
                    t.Write(Item.For_TSS)
                    t.Write(vbTab)
                    t.Write(Item.For_PAS)
                    t.Write(vbTab)
                    If For_TSS.Count > 0 AndAlso IsNothing(Item.Five_Prime_Adaptors(For_TSS.First)) = False Then t.Write(Szunyi.Location.Common.GetLocationString(Item.Five_Prime_Adaptors(For_TSS.First).Location))
                    t.Write(vbTab)
                    If For_TSS.Count > 0 AndAlso IsNothing(Item.Three_Prime_Adaptors(For_TSS.First)) = False Then t.Write(Szunyi.Location.Common.GetLocationString(Item.Three_Prime_Adaptors(For_TSS.First).Location))
                    t.Write(vbTab)
                    If For_PAS.Count > 0 AndAlso IsNothing(Item.Five_Prime_Adaptors(For_PAS.First)) = False Then t.Write(Szunyi.Location.Common.GetLocationString(Item.Five_Prime_Adaptors(For_PAS.First).Location))
                    t.Write(vbTab)
                    If For_PAS.Count > 0 AndAlso IsNothing(Item.Three_Prime_Adaptors(For_PAS.First)) = False Then t.Write(Szunyi.Location.Common.GetLocationString(Item.Three_Prime_Adaptors(For_PAS.First).Location))
                    t.Write(vbTab)
                    If For_TSS.Count > 0 AndAlso IsNothing(Item.Five_Prime_Adaptors(For_TSS.First)) = False Then t.Write(Item.Five_Prime_Adaptors(For_TSS.First).full_Score)
                    t.Write(vbTab)
                    If For_TSS.Count > 0 AndAlso IsNothing(Item.Three_Prime_Adaptors(For_TSS.First)) = False Then t.Write(Item.Three_Prime_Adaptors(For_TSS.First).full_Score)
                    t.Write(vbTab)
                    If For_PAS.Count > 0 AndAlso IsNothing(Item.Five_Prime_Adaptors(For_PAS.First)) = False Then t.Write(Item.Five_Prime_Adaptors(For_PAS.First).full_Score)
                    t.Write(vbTab)
                    If For_PAS.Count > 0 AndAlso IsNothing(Item.Three_Prime_Adaptors(For_PAS.First)) = False Then t.Write(Item.Three_Prime_Adaptors(For_PAS.First).full_Score)

                    For Each A In Item.Orientation_By_Adapters
                        t.Write(vbTab)
                        t.Write(A.Value)
                    Next

                Next
            End Using
        Next

        Dim jh As Int16 = 43
    End Sub

    Private Sub PolyAReadLengthDistributionToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles PolyAReadLengthDistributionToolStripMenuItem.Click
        Dim Bed_Files = Szunyi.IO.Files.Filter.SelectFiles(Constants.Files.BED)
        If IsNothing(Bed_Files) = True Then Exit Sub
        Dim Bam_Files = Get_Bam_Files()
        If IsNothing(Bam_Files) = True OrElse Bam_Files.Count = 0 Then Exit Sub
        Dim PoreChop_FIle = Szunyi.IO.Files.Filter.SelectFiles(Constants.Files.PoreCHop)
        If IsNothing(PoreChop_FIle) = True Then Exit Sub
        Dim PC_Results = Szunyi.Console.Porechop.Porechop.Get_Short_Porechop_Result(PoreChop_FIle, 0)
        Dim cPC As New Szunyi.Console.Porechop.Short_Result_ByReadID
        PC_Results.Sort(cPC)
        Dim All_SAMs = Szunyi.BAM.Bam_Basic_IO.Import.ParseAll_Into_Basic_Locations(Bam_Files)

        Dim PAS_Finder As New Szunyi.Location.Basic_Location_Finder(All_SAMs, True)
        Dim Range = MyInputBox.GetInteger("Enter Range", 10)

        For Each Bed_File In Bed_Files
            Dim x As New Bio.IO.Bed.BedParser
            Dim BEDs = x.ParseRange(Bed_File.FullName)
            Dim Res(150) As Integer
            For Each BED In BEDs
                Dim loci = Szunyi.Location.Common.Get_Location(BED.Start, BED.End, BED.Metadata("Strand"))
                Dim BL_loci = Szunyi.Location.Basic_Location_Manipulation.Get_Basic_Location(loci)
                Dim Found_SAMs = PAS_Finder.Find_Items_byLoci(BL_loci, Range, Sort_Locations_By.PAS)
                BED.Metadata.Add("BlockSizes", "")
                Dim BlockSizes As New List(Of Integer)
                For Each fSAM In Found_SAMs
                    Dim x1 As New Szunyi.Console.Porechop.Short_Result()
                    x1.ReadID = fSAM.Location.Accession
                    Dim Index = PC_Results.BinarySearch(x1, cPC)
                    If Index > -1 Then
                        Dim Length As Integer = 0
                        If IsNothing(PC_Results(Index).PAS_Location_Five) = False Then
                            Length = PC_Results(Index).PAS_Location_Five_final_score
                        ElseIf IsNothing(PC_Results(Index).PAS_Location_Three) = False Then
                            Length = PC_Results(Index).PAS_Location_Three_final_score
                        End If
                        If Length = 0 Then
                            Dim kj As Int16 = 54
                        End If
                        BlockSizes.Add(Length)
                        If Length > Res.Count - 1 Then ReDim Preserve Res(Length)
                        Res(Length) += 1

                    Else
                        Dim jk As Int16 = 54
                    End If
                Next
                BlockSizes.Sort()
                BED.Metadata("BlockSizes") = Szunyi.Text.General.GetText(BlockSizes, ",")
            Next
            Dim str As New System.Text.StringBuilder

            Dim txt = Szunyi.Text.General.GetText(Res)
            Szunyi.IO.Export.SaveText(txt, Szunyi.IO.Files.Get_New_FileName.Append_Before_Extension_wNew_Extension(Bed_File, "_" & Bam_Files.First.Name & "_PolyA_Length_Distribution.tsv"))
            Dim NfiLE = Szunyi.IO.Files.Get_New_FileName.Append_Before_Extension_wNew_Extension(Bed_File, "_" & Bam_Files.First.Name & "_PolyA_Length_Distribution.bed")
            Dim w As New Bio.IO.Bed.BedFormatter
            w.Format(BEDs, NfiLE.FullName)

        Next
    End Sub

    Private Sub ByBarcodeToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles ByBarcodeToolStripMenuItem.Click
        Dim x As New Szunyi.MinIon.SequencingSummary(MinIon.SequencingSummary.Type.barcode)

    End Sub

    Private Sub ByRunIDToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles ByRunIDToolStripMenuItem.Click

        Dim x As New Szunyi.MinIon.SequencingSummary(MinIon.SequencingSummary.Type.RunId)
    End Sub

    Private Sub ByBarcodeAndRunIDToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles ByBarcodeAndRunIDToolStripMenuItem.Click
        Dim x As New Szunyi.MinIon.SequencingSummary(MinIon.SequencingSummary.Type.BarCode_RunID)
    End Sub

    Private Sub AllToolStripMenuItem4_Click(sender As Object, e As EventArgs) Handles AllToolStripMenuItem4.Click
        Dim x As New Szunyi.MinIon.SequencingSummary(MinIon.SequencingSummary.Type.All)
    End Sub

    Private Sub ByBamFilesToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles ByBamFilesToolStripMenuItem.Click
        Dim x As New Szunyi.MinIon.SequencingSummary(MinIon.SequencingSummary.Type.Bam)
    End Sub


    Private Sub RCToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles RCToolStripMenuItem.Click
        Dim Files = Szunyi.IO.Files.Filter.SelectFiles(Constants.Files.Fasta_FastQ)
        If IsNothing(Files) = False Then
            For Each FIle In Files
                Dim nFIle = Szunyi.IO.Files.Get_New_FileName.Append_Before_Extension(FIle, "_RC")
                Using exp As New Szunyi.IO.Export.fasta_Writter(nFIle)
                    For Each Seq In Szunyi.IO.Import.Sequence.GetOnyByONe(FIle)
                        Dim nSeq = Seq.GetReverseComplementedSequence

                        exp.write(nSeq)
                    Next
                End Using


            Next
        End If
    End Sub

    Private Sub GetUniquenonUniqueFromPOwershellCsvToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles GetUniquenonUniqueFromPOwershellCsvToolStripMenuItem.Click
        Dim File = Szunyi.IO.Files.Filter.SelectFile(Constants.Files.All_TAB_Like)
        If IsNothing(File) = True Then Exit Sub
        Dim Files As New List(Of FileInfo)
        For Each Line In Szunyi.IO.Import.Text.Parse(File, 1)
            Files.Add(New FileInfo(Line.Trim(Chr(34))))
        Next
        Dim Log As New System.Text.StringBuilder
        For Each GroupByName In Szunyi.IO.Files.Iter.ByFileName(Files)
            If GroupByName.Count = 2 Then
                Dim f = Szunyi.IO.Import.Text.ReadToEnd(GroupByName.First)
                Dim l = Szunyi.IO.Import.Text.ReadToEnd(GroupByName.Last)
                If f <> l Then
                    Log.Append(GroupByName.First.Name)
                    For Each Item In GroupByName
                        Log.Append(vbTab & Item.FullName)
                    Next
                    Log.AppendLine()
                End If
            ElseIf GroupByName.Count > 2 Then
                Log.Append(GroupByName.First.Name)
                For Each Item In GroupByName
                    Log.Append(vbTab & Item.FullName)
                Next
                Log.AppendLine()

            End If

        Next
        Szunyi.IO.Export.SaveText(Log.ToString)
        Dim kj As Int16 = 54
    End Sub

    Private Sub SequencingSummaryCheckToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles SequencingSummaryCheckToolStripMenuItem.Click

    End Sub
End Class

Public Class ListOfs
    Public Shared Property x As New Dictionary(Of String, ListManager)


    Public Shared Property ListOfSequences As New List(Of SequenceList)
    Public Shared Property ListOfLocations As New List(Of Szunyi.Location.LocationList)
    Public Shared Property ListOfExtFeatures As New List(Of ExtFeatureList)
    Public Shared Property ListOfMappings As New List(Of Szunyi.Other_Database.Affy.ParseAffy)
    Public Shared Property ListOf_Item_With_Properties As New List(Of Szunyi.Text.TableManipulation.Items_With_Properties)
    Public Shared Property ListOf_Sequences_With_Motifs As New List(Of Szunyi.Sequence_Analysis.Sequences_Ranges)


    Public Shared Property NofSequenceList As Integer
    Public Shared Property NofLocations As Integer
    Public Shared Property NofExtFeatures As Integer
    Public Shared Property NofMapping As Integer
    Public Shared Property NofCounts As Integer
    Public Shared Property Nof_Item_With_Properties As Integer
    Public Shared Property Nof_Sequences_With_Motifs As Integer
End Class

