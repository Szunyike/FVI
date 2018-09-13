Imports System.Globalization
Imports System.IO
Imports System.Text

Namespace Szunyi
    Namespace Text
        Namespace TableManipulation
            Public Class ReplaceStringsInFiles
                Dim FilesToReplace As List(Of FileInfo)
                Dim FileContaingReplacing As FileInfo
                Dim HeadersOfContainingReplacing As List(Of String)
                Dim ColumnOfOriginalString As Integer = -1
                Dim ColumnOfNewString As Integer = -1
                Dim IntrestingColumns As New List(Of Integer)
                Public ReadOnly Property Type = Szunyi.Constants.BackGroundWork.ReplaceStringsInFiles
                Public Sub New()
                    ' Get Tab File Which Contains Replacing motifs
                    ' Set To Search Column, and the Replace motif column
                    ' Get Tab file
                    ' Set column to Replace
                    ' Replace all the motifs
                    ' Save it
                    SetOriginalReplacing()
                    If Me.ColumnOfNewString = -1 Or Me.ColumnOfOriginalString = -1 Then Exit Sub
                    SetFilesToReplace()

                End Sub
                Private Sub SetOriginalReplacing()
                    FileContaingReplacing =  Szunyi.IO.Files.Filter.SelectFile("","Select File Which Contains Original Strings, and New Strings")
                    HeadersOfContainingReplacing = Szunyi.IO.Import.Text.GetHeader(FileContaingReplacing, 1, Nothing, Nothing)

                    Dim IntrestingColumns As New List(Of Integer)

                    Using x As New Select_Columns(HeadersOfContainingReplacing)
                        x.Text = "Select Original String Column"
                        If x.ShowDialog = System.Windows.Forms.DialogResult.OK Then
                            ColumnOfOriginalString = x.SelectedIndexes.First
                        Else : Exit Sub
                        End If
                    End Using

                    Using x As New Select_Columns(HeadersOfContainingReplacing)
                        x.Text = "Select New String Column"
                        If x.ShowDialog = System.Windows.Forms.DialogResult.OK Then
                            ColumnOfNewString = x.SelectedIndexes.First
                        Else : Exit Sub
                        End If
                    End Using
                End Sub
                Private Sub SetFilesToReplace()
                    Me.FilesToReplace = Szunyi.IO.Files.Filter.SelectFiles

                    Dim HeadersOfSearch = Szunyi.IO.Import.Text.GetHeader(Me.FilesToReplace.First, 0, Nothing, Nothing)
                    Using x As New Select_Columns(HeadersOfSearch)
                        x.Text = "Select New String Column"
                        If x.ShowDialog = System.Windows.Forms.DialogResult.OK Then
                            IntrestingColumns = x.SelectedIndexes
                        Else : Exit Sub
                        End If
                    End Using

                End Sub
                Public Sub DoIt()
                    Dim ListOfItems = Szunyi.IO.Import.Text.GetListForReplace(Me.FileContaingReplacing, ColumnOfOriginalString, ColumnOfNewString)
                    If IsNothing(ListOfItems) = True Then Exit Sub

                    Parallel.ForEach(FilesToReplace, Sub(File)
                                                         ReplaceStringsInColumns(File, ListOfItems, IntrestingColumns)
                                                     End Sub)

                End Sub
                Public Sub DOItSimple()
                    Dim ListOfItems = Szunyi.IO.Import.Text.GetListForReplace(Me.FileContaingReplacing, ColumnOfOriginalString, ColumnOfNewString)
                    If IsNothing(ListOfItems) = True Then Exit Sub

                    Parallel.ForEach(FilesToReplace, Sub(File)
                                                         Dim txt = Szunyi.IO.Import.Text.ReadToEnd(File)
                                                         For Each item In ListOfItems
                                                             If item(1) <> "" Then
                                                                 txt = txt.Replace(item(0), item(1))
                                                             End If

                                                         Next
                                                         Dim nFile As New FileInfo(File.FullName.Replace(File.Extension, "md" & File.Extension))
                                                         Szunyi.IO.Export.SaveText(txt, nFile)
                                                     End Sub)

                End Sub
                Public Sub ReplaceStringsInColumns(FileInSearch As FileInfo,
                 listOfItems As List(Of String()), intrestingColumns As List(Of Integer))

                    Dim res As New List(Of String)
                    Dim Comp As New ClassLibrary1.Szunyi.Comparares.OneByOne.StringComparer
                    Using sr As New StreamReader(FileInSearch.FullName)
                        Dim nFIle As New FileInfo(Szunyi.IO.Files.Get_New_FileName.Append_Before_Extension(FileInSearch, "Replaced").FullName)
                        Using sw As New StreamWriter(nFIle.FullName, True)
                            Do
                                Dim s1() = Split(sr.ReadLine, vbTab)
                                If s1.Length >= intrestingColumns.Max Then
                                    Dim x1(1) As String

                                    For Each Index In intrestingColumns
                                        x1(0) = s1(Index)
                                        Dim x = listOfItems.BinarySearch(x1, Comp)
                                        If x > -1 Then
                                            s1(Index) = listOfItems(x).Last
                                        End If
                                    Next
                                Else

                                End If

                                sw.Write(Szunyi.Text.General.GetText(s1, vbTab))
                                sw.WriteLine()
                            Loop Until sr.EndOfStream = True

                        End Using
                    End Using
                End Sub

            End Class




            ''' <summary>
            ''' If ColumnsToInclude is nothing, then all of the column will inclueded
            ''' </summary>

            Public Class Items_With_Properties
                Public Property Sorted As Boolean = False
                Public ReadOnly Property Type = Szunyi.Constants.BackGroundWork.Items_With_Properties
                Public Property Items As New List(Of Item_With_Properties)
                Public Property ItemHeaders As New List(Of String)
                Public Property Summary As String
                Public Property Title As String
                Public Property UniqueID As Integer
                Public Property IsUnique As Boolean
                Private Comp As New Comparares.OneByOne.Item_With_Properties_Comparer
                Dim tmpItem As New Item_With_Properties

                Private File As FileInfo
                Private IDCOl As Integer
                Private ColumnsToInclude As List(Of Integer)
                Private ForbiddenStrings As List(Of String)
                Private NofHeader As Integer = 1


#Region "New"


                Public Sub New(File As FileInfo,
                               IDcol As Integer,
                               ColumnsToInclude As List(Of Integer),
                               log As List(Of String),
                               ForbiddenStrings As List(Of String),
                               IsUnique As Boolean,
                               Optional NofHeader As Integer = 1)
                    Me.Title = File.Name
                    Me.File = File
                    Me.IDCOl = IDcol
                    Me.ColumnsToInclude = ColumnsToInclude
                    Me.ForbiddenStrings = ForbiddenStrings
                    Me.IsUnique = IsUnique
                    Me.NofHeader = NofHeader
                    DoIt(False)
                End Sub
                Public Sub New(File As FileInfo)
                    Me.Title = File.Name
                    Me.File = File
                    Me.IDCOl = 0
                    Me.ColumnsToInclude = Nothing
                    Me.ForbiddenStrings = Nothing
                    Me.IsUnique = False
                    Me.NofHeader = 1
                End Sub
                Public Sub DoIt(MustSort As Boolean)

                    If IsNothing(ColumnsToInclude) = True Then
                        Dim Header = Szunyi.IO.Import.Text.GetHeader(File, 1, Nothing, Nothing)
                        Me.ItemHeaders.AddRange(Header)
                        For Each s In Szunyi.IO.Import.Text.ParseToArray(File, vbTab, 1)

                            Dim x As New Item_With_Properties(s(IDCOl)) ' It is Only For ID
                            x.Properties.AddRange(s)

                            Me.Items.Add(x)

                        Next
                    Else
                        Dim AllHeader = Szunyi.IO.Import.Text.GetHeader(File, 1, Nothing, ColumnsToInclude)
                        Me.ItemHeaders.AddRange(AllHeader)
                        For Each s In Szunyi.IO.Import.Text.ParseToArray(File, vbTab, 1)
                            If s.Count > ColumnsToInclude.Max AndAlso s.Count > IDCOl Then
                                Dim x As New Item_With_Properties(s(IDCOl)) ' It is Only For ID

                                For Each index In ColumnsToInclude
                                    s(index) = Szunyi.Text.General.RemoveFromString(s(index), ForbiddenStrings)
                                    x.Properties.Add(s(index))
                                Next

                                Me.Items.Add(x)
                            End If

                        Next
                    End If
                    For i1 = Me.ItemHeaders.Count - 1 To 0 Step -1
                        If Me.ItemHeaders(i1) = "" Then
                            Me.DeleteColumn(i1)
                        End If
                    Next

                    Dim Nof As Integer = 0

                    If Me.IsUnique = True Then
                        '   Me.Items = Me.Items.Distinct(Comp).ToList
                    End If
                    If MustSort = True Then
                        Me.Items.Sort(Comp)
                        Me.Sorted = True
                    End If

                End Sub
                Public Function Get_Distict_Headers_wValues() As List(Of String)
                    Dim out As New List(Of String)
                    For i1 = 0 To Me.ItemHeaders.Count - 1
                        For i2 = 0 To Items.Count - 1
                            Dim s = Me.ItemHeaders(i1) & ":" & Me.Items(i2).Properties(i1)
                            out.Add(s)
                        Next
                    Next
                    Dim tmp = out.Distinct.ToList
                    Return tmp
                End Function


                Public Sub RenameIDsToShortLocusTAg()
                    For Each Item In Me.Items
                        Item.ID = Split(Item.ID, ".").First
                    Next
                End Sub
                Public Sub DeleteEmptyItems(StartColID As Integer, EndCOlID As Integer)
                    For i1 = Me.Items.Count - 1 To 0 Step -1
                        Dim Isempty As Boolean = True
                        For i2 = StartColID To EndCOlID
                            If Me.Items(i1).Properties(i2) <> "" Then
                                Isempty = False
                                Exit For
                            End If
                        Next
                        If Isempty = True Then
                            Me.Items.RemoveAt(i1)
                        End If
                    Next
                End Sub
                Public Sub New(Keys As List(Of String), IsUnique As Boolean)
                    Me.IsUnique = IsUnique
                    If Me.IsUnique = True Then
                        Dim dKey = Keys.Distinct.ToList
                        dKey.Sort()
                        For Each Key In dKey
                            Me.Items.Add(New Item_With_Properties(Key))
                        Next
                    Else
                        For Each Key In Keys
                            Me.Items.Add(New Item_With_Properties(Key))
                        Next
                    End If
                    Me.Items.Sort(Comp)

                End Sub

                Public Function FilterByIDs(filteredSeqsIDs As List(Of String)) As Items_With_Properties
                    Dim x = Me.Clone
                    Dim ToDelete As New List(Of Item_With_Properties)
                    filteredSeqsIDs.Sort()
                    For Each Item In x.Items
                        If filteredSeqsIDs.BinarySearch(Item.ID) < 0 Then
                            ToDelete.Add(Item)
                        End If
                    Next
                    For Each Item In ToDelete
                        x.Items.Remove(Item)
                    Next
                    Return x
                End Function

                Public Sub New()
                End Sub
                Public Sub New(Seqs As List(Of Bio.ISequence))
                    Me.ItemHeaders.Add("SeqID")
                    Me.ItemHeaders.Add("Sequence")
                    For Each Seq In Seqs
                        Me.Items.Add(New Item_With_Properties(Seq.ID))
                        Me.Items.Last.Properties.Add(Seq.ID)
                        Me.Items.Last.Properties.Add(Szunyi.Sequences.SequenceManipulation.SeqsToString.GetSeqAsString(Seq))
                    Next
                    Me.Items.Sort(Comp)
                End Sub

#End Region
                Public Function Get_Index(Key As String, Type As Szunyi.Constants.TextMatch) As Integer
                    Select Case Type
                        Case Szunyi.Constants.TextMatch.Exact
                            tmpItem.ID = Key
                            Return Me.Items.BinarySearch(tmpItem, Comp)
                        Case Szunyi.Constants.TextMatch.Contains
                            For i1 = 0 To Me.Items.Count - 1
                                If Me.Items(i1).ID.Contains(Key) = True Then
                                    Return i1
                                End If
                            Next
                    End Select
                    Return -1
                End Function
#Region "Add Values WithOut Keys"
                Public Sub Add_Values_WithOut_Keys(ItemHeader As String, Values() As Double)
                    ItemHeaders.Add(ItemHeader)
                    For i1 = 0 To Me.Items.Count - 1
                        Me.Items(i1).Properties.Add(Values(i1))
                    Next
                End Sub
                Public Sub Add_Values_WithOut_Keys(ItemHeader As String, Values As List(Of Double))
                    ItemHeaders.Add(ItemHeader)
                    For i1 = 0 To Me.Items.Count - 1
                        Me.Items(i1).Properties.Add(Values(i1))
                    Next
                End Sub

                Public Sub Add_Values_WithOut_Keys(ItemHeader As String, Values() As Long)
                    ItemHeaders.Add(ItemHeader)
                    For i1 = 0 To Me.Items.Count - 1
                        Me.Items(i1).Properties.Add(Values(i1))
                    Next
                End Sub
                Public Sub Add_Values_WithOut_Keys(ItemHeader As String, Values As List(Of Long))
                    ItemHeaders.Add(ItemHeader)
                    For i1 = 0 To Me.Items.Count - 1
                        Me.Items(i1).Properties.Add(Values(i1))
                    Next
                End Sub

                Public Sub Add_Values_WithOut_Keys(ItemHeader As String, Values() As Integer)
                    ItemHeaders.Add(ItemHeader)
                    For i1 = 0 To Me.Items.Count - 1
                        Me.Items(i1).Properties.Add(Values(i1))
                    Next
                End Sub
                Public Sub Add_Values_WithOut_Keys(ItemHeader As String, Values As List(Of Integer))
                    ItemHeaders.Add(ItemHeader)
                    For i1 = 0 To Me.Items.Count - 1
                        Me.Items(i1).Properties.Add(Values(i1))
                    Next
                End Sub

                Public Sub Add_Values_WithOut_Keys(ItemHeader As String, Values() As String)
                    ItemHeaders.Add(ItemHeader)
                    For i1 = 0 To Me.Items.Count - 1
                        Me.Items(i1).Properties.Add(Values(i1))
                    Next
                End Sub
                Public Sub Add_Values_WithOut_Keys(ItemHeader As String, Values As List(Of String))
                    ItemHeaders.Add(ItemHeader)
                    For i1 = 0 To Me.Items.Count - 1
                        Me.Items(i1).Properties.Add(Values(i1))
                    Next
                End Sub

                Public Sub Add_Values_WithOut_Keys(ItemHeaders As List(Of String), Values() As List(Of String))
                    Me.ItemHeaders.AddRange(ItemHeaders)
                    For Each l In Values
                        For i1 = 0 To Me.Items.Count - 1
                            Me.Items(i1).Properties.Add(l(i1))
                        Next
                    Next
                End Sub
#End Region

#Region "Insert Values WithOut Keys"
                Public Sub Insert_Values_WithOut_Keys(ItemHeader As String, Values() As Double, ColumnID As Integer)
                    ItemHeaders.Insert(ColumnID, ItemHeader)

                    For i1 = 0 To Me.Items.Count - 1
                        Me.Items(i1).Properties.Insert(ColumnID, Values(i1))
                    Next

                End Sub
                Public Sub Insert_Values_WithOut_Keys(ItemHeader As String, Values As List(Of Double), ColumnID As Integer)
                    ItemHeaders.Insert(ColumnID, ItemHeader)

                    For i1 = 0 To Me.Items.Count - 1
                        Me.Items(i1).Properties.Insert(ColumnID, Values(i1))
                    Next
                End Sub

                Public Sub Insert_Values_WithOut_Keys(ItemHeader As String, Values() As Long, ColumnID As Integer)
                    ItemHeaders.Insert(ColumnID, ItemHeader)

                    For i1 = 0 To Me.Items.Count - 1
                        Me.Items(i1).Properties.Insert(ColumnID, Values(i1))
                    Next
                End Sub
                Public Sub Insert_Values_WithOut_Keys(ItemHeader As String, Values As List(Of Long), ColumnID As Integer)
                    ItemHeaders.Insert(ColumnID, ItemHeader)

                    For i1 = 0 To Me.Items.Count - 1
                        Me.Items(i1).Properties.Insert(ColumnID, Values(i1))
                    Next
                End Sub

                Public Sub Insert_Values_WithOut_Keys(ItemHeader As String, Values() As Integer, ColumnID As Integer)
                    ItemHeaders.Insert(ColumnID, ItemHeader)

                    For i1 = 0 To Me.Items.Count - 1
                        Me.Items(i1).Properties.Insert(ColumnID, Values(i1))
                    Next
                End Sub
                Public Sub Insert_Values_WithOut_Keys(ItemHeader As String, Values As List(Of Integer), ColumnID As Integer)
                    ItemHeaders.Insert(ColumnID, ItemHeader)

                    For i1 = 0 To Me.Items.Count - 1
                        Me.Items(i1).Properties.Insert(ColumnID, Values(i1))
                    Next
                End Sub
                Public Sub Insert_Values_WithOut_Keys(ItemHeader As String, Values() As String, ColumnID As Integer)
                    ItemHeaders.Insert(ColumnID, ItemHeader)

                    For i1 = 0 To Me.Items.Count - 1
                        Me.Items(i1).Properties.Insert(ColumnID, Values(i1))
                    Next
                End Sub
                Public Sub Insert_Values_WithOut_Keys(ItemHeader As String, Values As List(Of String), ColumnID As Integer)
                    ItemHeaders.Insert(ColumnID, ItemHeader)

                    For i1 = 0 To Me.Items.Count - 1
                        Me.Items(i1).Properties.Insert(ColumnID, Values(i1))
                    Next
                End Sub
#End Region

#Region "Add Values By Key"
                Public Sub AddByKeys(ItemHeader As String, Keys As List(Of String), FoundValue As String, NotFoundValue As String)
                    ItemHeaders.Add(ItemHeader)
                    For Each Item In Items

                        If Keys.Contains(Item.ID) Then
                            Item.Properties.Add(FoundValue)
                        Else
                            Item.Properties.Add(NotFoundValue)
                        End If
                    Next
                End Sub
                Public Sub AddByKeys(ItemHeader As String, Keys As List(Of String), Values As List(Of String))
                    ItemHeaders.Add(ItemHeader)
                    Dim NofProperties = Me.ItemHeaders.Count
                    For i1 = 0 To Keys.Count - 1
                        Dim Key = (Keys(i1))
                        Dim Index = Get_Index(Key, Szunyi.Constants.TextMatch.Exact)

                        If Index > -1 Then
                            Me.Items(Index).Properties.Add(Values(i1))
                        Else
                            Dim alf As Int16 = 54
                        End If

                    Next
                    CheckItems()
                End Sub
                Public Sub Add_ValuesByKey(Key As String, Items As List(Of String))
                    Dim Index = Get_Index(Key, Szunyi.Constants.TextMatch.Exact)

                    If Index > -1 Then
                        Me.Items(Index).Properties.AddRange(Items)
                    End If
                End Sub

                Public Sub AddByKeys(ItemHeader As String, Keys As List(Of String), Values As List(Of Integer))
                    ItemHeaders.Add(ItemHeader)
                    Dim NofProperties = Me.ItemHeaders.Count
                    For i1 = 0 To Keys.Count - 1
                        Dim Key = (Keys(i1))
                        Dim Index = Get_Index(Key, Szunyi.Constants.TextMatch.Exact)

                        If Index > -1 Then
                            Me.Items(Index).Properties.Add(Values(i1))
                        Else
                            Dim alf As Int16 = 54
                        End If

                    Next
                    CheckItems()
                End Sub
                Public Sub AddByKeys(ItemHeader As String, Keys As List(Of String), Values As List(Of Long))
                    ItemHeaders.Add(ItemHeader)
                    Dim NofProperties = Me.ItemHeaders.Count
                    For i1 = 0 To Keys.Count - 1
                        Dim Key = (Keys(i1))
                        Dim Index = Get_Index(Key, Szunyi.Constants.TextMatch.Exact)

                        If Index > -1 Then
                            Me.Items(Index).Properties.Add(Values(i1))
                        Else
                            Dim alf As Int16 = 54
                        End If

                    Next
                    CheckItems()
                End Sub
                Public Sub AddByKeys(ItemHeader As String, Keys As List(Of String), Values As List(Of Double))
                    ItemHeaders.Add(ItemHeader)
                    Dim NofProperties = Me.ItemHeaders.Count
                    For i1 = 0 To Keys.Count - 1
                        Dim Key = (Keys(i1))
                        Dim Index = Get_Index(Key, Szunyi.Constants.TextMatch.Exact)

                        If Index > -1 Then
                            Me.Items(Index).Properties.Add(Values(i1))
                        Else
                            Dim alf As Int16 = 54
                        End If

                    Next
                    CheckItems()
                End Sub

#End Region


                Public Sub Add(File As FileInfo, IDCol As Integer, ColumnsToInclude As List(Of Integer), log As List(Of String))
                    Dim AllHeader = Szunyi.IO.Import.Text.GetHeader(File, 1, Nothing, ColumnsToInclude)
                    Me.ItemHeaders.AddRange(AllHeader)
                    Dim NofFound As Integer = 0
                    For Each s In Szunyi.IO.Import.Text.ParseToArray(File, vbTab, 1)
                        Dim Key As String = s(IDCol)
                        Dim Index = Get_Index(Key, Szunyi.Constants.TextMatch.Exact)


                        If Index > -1 Then

                            NofFound += 1
                            For i1 = Index To 0 Step -1
                                If Items(i1).ID = Key Then
                                    Dim TheItem = Me.Items(i1)
                                    Add_To_Item(TheItem, s, ColumnsToInclude)
                                Else
                                    Exit For
                                End If
                            Next
                            For i1 = Index + 1 To Me.Items.Count - 1
                                If Items(i1).ID = Key Then
                                    Dim TheItem = Me.Items(i1)
                                    Add_To_Item(TheItem, s, ColumnsToInclude)
                                Else
                                    Exit For
                                End If
                            Next
                        Else
                            ' Not Sorted
                            Dim SelItems = From x In Me.Items Where x.ID = s(IDCol)

                            For Each Item In SelItems
                                Add_To_Item(Item, s, ColumnsToInclude)
                            Next

                        End If
                    Next
                    CheckItems()

                End Sub
                Private Sub Add_To_Item(ByRef THeItem As Item_With_Properties, s As String(), ColumnsToInclude As List(Of Integer))
                    For Each Index In ColumnsToInclude
                        If Index < s.Count Then
                            THeItem.Properties.Add(s(Index))
                        Else
                            THeItem.Properties.Add("")
                        End If
                    Next
                End Sub



                Public Sub Add(ItemHeader As String, Keys As List(Of String),
                               Values As List(Of String),
                               Optional InsertionPosition As Integer = Nothing,
                               Optional StringsToRemove As List(Of String) = Nothing)
                    If IsNothing(InsertionPosition) = True Then
                        ItemHeaders.Add(ItemHeader)
                        For i1 = 0 To Keys.Count - 1
                            Dim Index = Get_Index(Keys(i1), Szunyi.Constants.TextMatch.Exact)

                            If Index > -1 Then
                                Values(i1) = Szunyi.Text.General.RemoveFromString(Values(i1), StringsToRemove)
                                Me.Items(Keys(i1)).Properties.Add(Values(i1))
                            End If
                        Next
                        CheckItems(Nothing) ' No Insertion add to the end
                    Else
                        ItemHeaders.Insert(InsertionPosition, ItemHeader)
                        For i1 = 0 To Keys.Count - 1
                            Dim Index = Get_Index(Keys(i1), Szunyi.Constants.TextMatch.Exact)

                            If Index > -1 Then
                                Values(i1) = Szunyi.Text.General.RemoveFromString(Values(i1), StringsToRemove)
                                Me.Items(Index).Properties.Insert(InsertionPosition, Values(i1))

                            End If
                        Next
                        Dim InsertionPositions As New List(Of Integer)
                        InsertionPositions.Add(InsertionPosition)
                        CheckItems(InsertionPositions)
                    End If

                End Sub
#Region "General Save Clone and Check "
                Public Sub CheckItems(Optional InsertionPositions As List(Of Integer) = Nothing)

                    Dim log As New List(Of String)
                    If IsNothing(InsertionPositions) = True Then
                        For Each Item In Me.Items
                            If Item.Properties.Count < Me.ItemHeaders.Count Then
                                log.Add(Item.ID)
                                For i1 = Item.Properties.Count To Me.ItemHeaders.Count - 1
                                    Item.Properties.Add("")
                                Next

                            End If
                        Next
                    Else
                        For Each Item In Me.Items
                            If Item.Properties.Count < Me.ItemHeaders.Count Then
                                log.Add(Item.ID)
                                For Each InsertionPosition In InsertionPositions
                                    Item.Properties.Insert(InsertionPosition, "")
                                Next

                            End If
                        Next
                    End If

                    If log.Count > 0 Then
                        Dim t = Szunyi.Text.General.GetText(log, ",")
                        Dim alf As Int16 = 54
                    End If
                End Sub

                ''' <summary>
                ''' Get all information as a tdt text
                ''' </summary>
                ''' <returns></returns>
                ''' 
                Public Overrides Function ToString() As String
                    If Items.Count = 0 Then Return String.Empty
                    Dim out As New StringBuilder
                    out.Append(Szunyi.Text.General.GetText(Me.ItemHeaders, vbTab)).AppendLine()
                    For Each Item As Item_With_Properties In Items
                        out.Append(Item.Get_Sub_Text(Me.ItemHeaders.Count - 1)).AppendLine()
                    Next
                    out.Length -= 2
                    Return out.ToString
                End Function
                Public Function Get_Text_With_ID() As String
                    Dim str As New StringBuilder
                    str.Append("ItemID")
                    str.Append(vbTab)
                    str.Append(Szunyi.Text.General.GetText(Me.ItemHeaders, vbTab))
                    str.AppendLine()

                    For Each Item As Item_With_Properties In Items
                        str.Append(Item.ID)
                        str.Append(vbTab)
                        str.Append(Item.ToString)
                        str.AppendLine()
                    Next
                    Return str.ToString

                End Function
                Public Function Get_Text_Without_ID() As String
                    Dim str As New StringBuilder
                    str.Append(Szunyi.Text.General.GetText(Me.ItemHeaders, vbTab))
                    str.AppendLine()

                    For Each Item As Item_With_Properties In Items
                        str.Append(Item.ToString)
                        str.AppendLine()
                    Next
                    Return str.ToString
                End Function
                Public Sub Save_WithOut_ID(Optional File As FileInfo = Nothing)
                    If IsNothing(File) = True Then
                        File = Szunyi.IO.Files.Save.SelectSaveFile("")
                        If IsNothing(File) = True Then Exit Sub
                    End If
                    Using sr As New StreamWriter(File.FullName)
                        sr.Write(Szunyi.Text.General.GetText(Me.ItemHeaders, vbTab))
                        sr.WriteLine()
                        For Each Item As Item_With_Properties In Items
                            sr.Write(Item.ToString)
                            sr.WriteLine()
                        Next
                    End Using
                End Sub
                Public Sub Save_With_ID(Optional File As FileInfo = Nothing)
                    If IsNothing(File) = True Then
                        File = Szunyi.IO.Files.Save.SelectSaveFile("")
                        If IsNothing(File) = True Then Exit Sub
                    End If
                    Using sr As New StreamWriter(File.FullName)
                        sr.Write("ItemID")
                        sr.Write(vbTab)
                        sr.Write(Szunyi.Text.General.GetText(Me.ItemHeaders, vbTab))
                        sr.WriteLine()
                        For Each Item As Item_With_Properties In Items
                            sr.Write(Item.ID)
                            sr.Write(vbTab)
                            sr.Write(Item.ToString)
                            sr.WriteLine()
                        Next
                    End Using
                End Sub
                Public Function Clone() As Items_With_Properties
                    Dim x As New Items_With_Properties
                    For Each Item In Me.ItemHeaders
                        x.ItemHeaders.Add(Item)
                    Next
                    For Each Prp In Me.Items
                        x.Items.Add(Prp.Clone)
                    Next
                    Return x
                End Function

                Public Function Get_Doubles(columnIDs As List(Of Integer)) As List(Of Double())
                    Dim res As New List(Of Double())

                    If IsNothing(columnIDs) = True Then
                        For i1 = 0 To Me.ItemHeaders.Count - 1
                            res.Add(Get_Doubles(i1))
                        Next
                    ElseIf columnIDs.Count = 0 Then
                        For i1 = 0 To Me.ItemHeaders.Count - 1
                            res.Add(Get_Doubles(i1))
                        Next
                    Else
                        For Each ColumnID In columnIDs
                            res.Add(Get_Doubles(ColumnID))
                        Next
                    End If



                    Return res
                End Function
                Public Function Get_Doubles(columnID As Integer, Optional Interesting_items As List(Of Item_With_Properties) = Nothing) As Double()
                    If IsNothing(Interesting_items) = True Then
                        Dim res(Me.Items.Count) As Double
                        Dim Total As Integer = 0
                        For i1 = 0 To Me.Items.Count - 1
                            If Me.Items(i1).Properties(columnID) = String.Empty Then
                                res(i1) = Double.NaN
                            Else
                                Dim d As Double
                                If Double.TryParse(Me.Items(i1).Properties(columnID), d) = True Then
                                    res(i1) = d
                                    Total += d
                                Else
                                    res(i1) = Double.NaN
                                End If
                            End If

                        Next
                        Return res
                    Else
                        Dim res As New List(Of Double)
                        Dim Total As Integer = 0
                        For Each Item In Interesting_items
                            If Item.Properties(columnID) = String.Empty Then
                                res.Add(Double.NaN)
                            Else
                                Dim d As Double
                                If Double.TryParse(Item.Properties(columnID), d) = True Then
                                    res.Add(d)
                                    Total += d
                                Else
                                    res.Add(Double.NaN)
                                End If
                            End If
                        Next

                        Return res.ToArray
                    End If

                End Function

                Public Sub Create_Pos_Neg_From_Coulmns(ItemHeader As String, Start_column As Integer, End_Column As Integer)
                    Me.ItemHeaders.Add(ItemHeader)
                    For Each Item In Me.Items
                        Dim IsAllPositive As Boolean = True
                        For i1 = Start_column To End_Column
                            If Item.Properties(i1) = "-" Then
                                IsAllPositive = False
                                Exit For
                            End If

                        Next
                        If IsAllPositive = True Then
                            Item.Properties.Add("+")
                        Else
                            Item.Properties.Add("-")
                        End If
                    Next

                End Sub

#End Region

#Region "GetItems"
                Public Function Get_Header_Index(HeaderValue As String) As Integer
                    For i1 = 0 To Me.ItemHeaders.Count - 1
                        If HeaderValue = Me.ItemHeaders(i1) Then Return i1
                    Next
                    Return -1
                End Function
                Public Function Get_Items_By_Value(HeaderValue As String, Values As List(Of String)) As List(Of Item_With_Properties)
                    Dim Index = Get_Header_Index(HeaderValue)
                    If Index < 0 Then Return New List(Of Item_With_Properties)
                    Dim out As New List(Of Item_With_Properties)
                    For Each Value In Values
                        out.AddRange(Get_Items_By_Value(Index, Value))
                    Next
                    Return out
                End Function
                Public Function Get_Items_By_Value(HeaderValue As String, Value As String) As List(Of Item_With_Properties)
                    Dim Index = Get_Header_Index(HeaderValue)
                    If Index < 0 Then Return New List(Of Item_With_Properties)
                    Return Get_Items_By_Value(Index, Value)
                End Function
                Public Function Get_Items_By_Value(HeaderIndex As Integer, Value As String) As List(Of Item_With_Properties)
                    Dim out As New List(Of Item_With_Properties)
                    For Each Item In Me.Items
                        If Item.Properties(HeaderIndex) = Value Then
                            out.Add(Item)
                        End If
                    Next
                    Return out
                End Function

#End Region


#Region "Filter Split Or Calculate Delete"
                Public Sub DeleteColumn(ColToDelete As Integer)
                    Me.ItemHeaders.RemoveAt(ColToDelete)
                    For Each Item In Me.Items
                        Item.Properties.RemoveAt(ColToDelete)
                    Next
                End Sub
                Public Function DeleteColumns(table_Manipulation_Settings As Table_Manipulation_Settings) As Items_With_Properties
                    Dim Columns_toDelete = From x In table_Manipulation_Settings.Items Where x.ToDelete = True Order By x.Column_ID Descending Select x.Column_ID

                    For Each Col In Columns_toDelete
                        Me.DeleteColumn(Col)
                    Next
                    Return Me
                End Function
                Public Function Filter(table_Manipulation_Settings As Table_Manipulation_Settings,
                                       First_Column As Integer, Last_Column As Integer) As Items_With_Properties
                    Dim Out As New Items_With_Properties()
                    Out.ItemHeaders.AddRange(Me.ItemHeaders)
                    Dim SelectedFIlters = From x In table_Manipulation_Settings.Items Where x.Filter_Display <> "" And x.Column_ID >= First_Column And x.Column_ID <= Last_Column

                    For Each Item In Me.Items
                        Dim Passed As Boolean = True
                        For Each Setting In SelectedFIlters
                            If Szunyi.Filter.Numerical_Filter.IsGood(Item.Properties(Setting.Column_ID),
                                                                     Setting.Filters) = False Then
                                Passed = False
                                Exit For
                            End If
                        Next
                        If Passed = True Then Out.Items.Add(Item.Clone)
                    Next
                    Return Out
                End Function



                Public Function SplitY(table_Manipulation_Settings As Table_Manipulation_Settings) As List(Of Items_With_Properties)

                    Dim SelectedCalculations = From x In table_Manipulation_Settings.Items Where x.Splitter <> ""

                    Dim res As New List(Of Items_With_Properties)
                    res.Add(Me.Clone)
                    For Each Spitter In SelectedCalculations
                        res = SplitIt(res, Spitter)
                    Next
                    Return res
                End Function

                Private Function SplitIt(x As List(Of Items_With_Properties), Splitter As Table_Manipulation_Setting) As List(Of Items_With_Properties)
                    Dim Out As New List(Of Items_With_Properties)
                    For Each ItemLIst In x
                        Dim t1 As New Items_With_Properties()
                        Dim t2 As New Items_With_Properties()
                        t1.ItemHeaders.AddRange(ItemLIst.ItemHeaders)
                        t1.Title = ItemLIst.Title & " Contains " & Splitter.Splitter
                        t2.ItemHeaders.AddRange(ItemLIst.ItemHeaders)
                        t2.Title = ItemLIst.Title & " Not Contains " & Splitter.Splitter
                        For Each Prop In ItemLIst.Items
                            If Prop.Properties(Splitter.Column_ID).Contains(Splitter.Splitter) Then
                                t1.Items.Add(Prop)
                            Else
                                t2.Items.Add(Prop)
                            End If

                        Next
                        Out.Add(t1)
                        Out.Add(t2)
                    Next
                    Return Out
                End Function

                Public Sub RenameID_ToShortLocusTag()
                    For Each Item In Me.Items
                        Item.ID = Split(Item.ID, ".").First
                    Next
                End Sub
                Public Sub ReNameID_Firts_Part(Separator As String)
                    For Each Item In Me.Items
                        Dim Index = Item.ID.LastIndexOf(Separator)
                        If Index > -1 Then
                            Item.ID = Item.ID.Substring(0, Index)
                        End If
                    Next
                    Me.Items.Sort(Szunyi.Comparares.AllComparares.By_Item_With_Properties_Comparer)

                End Sub

                Public Sub Merge(additional_Items As Items_With_Properties)

                    Me.ItemHeaders.AddRange(additional_Items.ItemHeaders)
                    Dim NofFound As Integer = 0
                    For Each Item In Me.Items
                        tmpItem.ID = Item.ID
                        Dim Index = additional_Items.Items.BinarySearch(tmpItem, Comp)
                        If Index > -1 Then
                            Item.Properties.AddRange(additional_Items.Items(Index).Properties)
                        End If
                    Next

                    CheckItems()
                End Sub

                Public Function Get_Values(ColID As Integer) As List(Of String)
                    Dim out As New List(Of String)
                    For Each Item In Me.Items
                        out.Add(Item.Properties(ColID))
                    Next
                    Return out
                End Function
                Public Function Get_Values(ColID As Integer, Keys As List(Of String)) As List(Of String)
                    Dim out As New List(Of String)
                    For Each Key In Keys
                        Dim Index = Get_Index(Key, Szunyi.Constants.TextMatch.Exact)
                        If Index > -1 Then
                            out.Add(Me.Items(Index).Properties(ColID))
                        Else

                            Dim ald As Int16 = 65
                        End If
                    Next

                    Return out
                End Function
                Public Function Get_Values(ColName As String, Keys As List(Of String)) As List(Of String)
                    Dim out As New List(Of String)
                    Dim ColID = Me.Get_Col_Id(ColName, 0)
                    For Each Key In Keys
                        Dim Index = Get_Index(Key, Szunyi.Constants.TextMatch.Exact)
                        If Index > -1 Then
                            out.Add(Me.Items(Index).Properties(ColID))
                        Else

                            Dim ald As Int16 = 65
                        End If
                    Next

                    Return out
                End Function
#End Region

#Region "Split Column"
                Public Sub Split_Columns_By_2Separator(ColID As Integer, mainSeparator As String, additional_Separator As String)
                    Dim Values = Me.Get_Values(ColID)
                    Dim Headers = Get_Sub_Strings(Values, mainSeparator, additional_Separator)
                    Dim Start_Index As Integer = Me.ItemHeaders.Count
                    Me.ItemHeaders.AddRange(Headers)
                    Me.CheckItems()
                    For Each Item In Me.Items
                        Dim m = Split(Item.Properties(ColID), mainSeparator)
                        For Each Sub_Item In m
                            Dim s = Split(Sub_Item.Trim(Chr(34)), Chr(34))
                            Dim ID = Get_Col_Id(s.First, Start_Index)
                            If ID > -1 Then
                                Item.Properties(ID) = s.Last
                            Else
                                Dim alf As Int16 = 54
                            End If
                        Next
                    Next
                End Sub
                Private Function Get_Col_Id(Header As String, Start_Index As Integer) As Integer
                    Header = Header.Trim(" ")
                    Header = Header.Trim(Chr(34))
                    For i1 = Start_Index To Me.ItemHeaders.Count - 1
                        If Me.ItemHeaders(i1) = Header Then Return i1
                    Next
                    Return -1
                End Function
                Private Function Get_Sub_Strings(values As List(Of String), mainSeparator As String, additional_Separator As String) As List(Of String)
                    Dim Headers As New List(Of String)
                    For Each Valu In values
                        Valu = Valu.Trim(Chr(34))
                        Dim s = Split(Valu, mainSeparator)
                        For Each s1 In s
                            Dim Seconds = Split(s1, additional_Separator)
                            If Seconds.Count > 1 Then
                                Seconds(0) = Seconds(0).Trim(" ")
                                If Seconds.First.StartsWith("/") Then
                                    If Headers.Contains(Seconds.First) = False Then Headers.Add(Seconds.First)
                                End If
                            End If
                        Next
                    Next
                    Return Headers
                End Function

                Public Function Get_Keys() As List(Of String)
                    Dim s As New List(Of String)
                    For Each Item In Me.Items
                        s.Add(Item.ID)
                    Next
                    Return s
                End Function
#End Region

                Public Function Get_Items_By_Keys(Keys As List(Of String)) As List(Of Item_With_Properties)
                    Dim out As New List(Of Item_With_Properties)
                    For Each Key In Keys
                        Dim Index = Get_Index(Key, Szunyi.Constants.TextMatch.Exact)
                        Dim Index2 = Get_Index(Key, Szunyi.Constants.TextMatch.Contains)
                        If Index > -1 Then
                            out.Add(Me.Items(Index))
                        Else
                            Dim alf As Int16 = 54
                        End If
                    Next
                    Return out
                End Function
            End Class
            Public Class Ext_Items_With_Properties
                Public Property Title As String
                Public Property Seqs As New List(Of Bio.ISequence)
                Public Property I_w_Ps As Items_With_Properties

            End Class


            Public Class Item_With_Properties
                Public Property ID As String
                Public Property Properties As New List(Of String)
                Public Sub New()

                End Sub
                Public Sub New(ID As String)
                    Me.ID = ID

                End Sub
                Public Overrides Function ToString() As String
                    Szunyi.Text.General.GetText(Me.Properties, vbTab)
                    Dim out As New System.Text.StringBuilder

                    For Each Prop In Me.Properties
                        out.Append(Prop).Append(vbTab)
                    Next
                    If out.Length > 0 Then out.Length -= 1
                    Return out.ToString
                End Function
                Public Function Get_Sub_Text(Max_ColID As Integer, Optional MinCOlID As Integer = 0)
                    Dim out As New System.Text.StringBuilder


                    For i1 = MinCOlID To Max_ColID
                            out.Append(Me.Properties(i1)).Append(vbTab)
                        Next

                    If out.Length > 0 Then out.Length -= 1
                    Return out.ToString

                End Function
                Public Function Clone() As Item_With_Properties
                    Dim x As New Item_With_Properties
                    x.ID = Me.ID
                    For Each Item In Properties
                        x.Properties.Add(Item)
                    Next
                    Return x
                End Function
            End Class
        End Namespace
    End Namespace
End Namespace


