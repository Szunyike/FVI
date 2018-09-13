Imports System.IO
Imports System.Text
Imports Bio
Imports ClassLibrary1
Imports System.Windows.Forms

Namespace Text
        Public Class ReplaceStringsInFiles
            Dim FilesToReplace As List(Of FileInfo)
            Dim FileContaingReplacing As FileInfo
            Dim HeadersOfContainingReplacing As List(Of String)
            Dim ColumnOfOriginalString As Integer, ColumnOfNewString As Integer
            Dim IntrestingColumns As New List(Of Integer)
            Dim FirstRowOfChangingFile As Integer
            Public ReadOnly Property Type = Szunyi.Constants.BackGroundWork.ReplaceStringsInFiles
            Public Property Ready As Boolean = False
            Public Sub New()
                ' Get Tab File Which Contains Replacing motifs
                ' Set To Search Column, and the Replace motif column
                ' Get Tab file
                ' Set column to Replace
                ' Replace all the motifs
                ' Save it
                If SetOriginalReplacing() = False Then Exit Sub
                SetFilesToReplace()

            End Sub
            Private Function SetOriginalReplacing() As Boolean
                FileContaingReplacing =  Szunyi.IO.Files.Filter.SelectFile("Select File Which Contains Original Strings, and New Strings")
            HeadersOfContainingReplacing = Szunyi.IO.Import.Text.GetHeader(FileContaingReplacing, 1, Nothing, Nothing)

            Dim IntrestingColumns As New List(Of Integer)

            Using x As New Select_Columns(HeadersOfContainingReplacing)
                x.Text = "Select Original String Column"
                If x.ShowDialog = System.Windows.Forms.DialogResult.OK Then
                    ColumnOfOriginalString = x.SelectedIndexes.First
                Else : Return False
                End If
            End Using

            Using x As New Select_Columns(HeadersOfContainingReplacing)
                x.Text = "Select New String Column"
                If x.ShowDialog = System.Windows.Forms.DialogResult.OK Then
                    ColumnOfNewString = x.SelectedIndexes.First
                Else : Return False
                End If
            End Using
            Return True
        End Function
        Private Sub SetFilesToReplace()
            Me.FilesToReplace = Szunyi.IO.Files.Filter.SelectFiles
            Dim x1 As New SelectFirstRowAndColumn_s_(Me.FilesToReplace.First,
                                                         Szunyi.Constants.DelimitedFileImport.SelectFirstRowAndColumns,
                                                         "sf")
            If x1.ShowDialog = DialogResult.OK Then
                Me.IntrestingColumns = x1.SelectedColumns
                Me.FirstRowOfChangingFile = x1.FirstLine
                Me.Ready = True
            End If


        End Sub
        Public Sub DoIt()
            Dim ListOfItems = Szunyi.IO.Import.Text.GetListForReplace(Me.FileContaingReplacing, ColumnOfOriginalString, ColumnOfNewString)
            For Each File In Me.FilesToReplace
                ReplaceStringsInColumns(File, ListOfItems, IntrestingColumns)
            Next
        End Sub
        Public Sub ReplaceStringsInColumns(FileInSearch As FileInfo,
                 listOfItems As List(Of String()), intrestingColumns As List(Of Integer))

            Dim res As New List(Of String)
            Dim Comp As New Szunyi.Comparares.OneByOne.StringComparer
            Using sr As New StreamReader(FileInSearch.FullName)
                Using sw As New StreamWriter(Szunyi.IO.Files.Get_New_FileName.Append_Before_Extension(FileInSearch, "Replaced").FullName)
                    For i1 = 1 To Me.FirstRowOfChangingFile
                        sw.Write(sr.ReadLine())
                        sw.WriteLine()
                    Next
                    Do
                        Dim s1() = Split(sr.ReadLine, vbTab)
                        Dim x1(1) As String

                        For Each Index In intrestingColumns
                            x1(0) = s1(Index)
                            Dim x = listOfItems.BinarySearch(x1, Comp)
                            If x > -1 Then
                                s1(Index) = listOfItems(x).Last
                            End If
                        Next
                        sw.Write(Szunyi.Text.General.GetText(s1, vbTab))
                        sw.WriteLine()
                    Loop Until sr.EndOfStream = True

                End Using
            End Using
        End Sub

    End Class


    Public Class GetCount
        Public Files As List(Of FileInfo)
        Dim ForbiddenStrings As New List(Of String)
        Dim InterestingStrings As New List(Of String)
        Dim InterestingColIndex As Integer
        Dim StartSeparator As String
        Dim EndSeparator As String
        Public Sub New()
            Me.Files = Szunyi.IO.Files.Filter.SelectFiles()
            If IsNothing(Files) = True Then Exit Sub
            Dim Header = Szunyi.IO.Import.Text.GetHeader(Me.Files.First, 1, Nothing, Nothing)

            InterestingColIndex = input.Input.GetIntrestingColumn(Me.Files.First, 1)
            Dim ls As New Get_List_of_String("Select Forbidden Strings")

            If ls.ShowDialog = DialogResult.OK Then
                ForbiddenStrings = Szunyi.Text.Lists.CloneStrings(ls.Strings)
            Else
                Exit Sub
            End If
            ls.Text = "Select Interesting Strings"
            If ls.ShowDialog = DialogResult.OK Then
                InterestingStrings = Szunyi.Text.Lists.CloneStrings(ls.Strings)
            Else
                Exit Sub
            End If
            Dim s = InputBox("Enter The Substring Separators divided by space")
            Dim s1() = Split(s, " ")
            If s1.Count <> 2 Then Exit Sub
            Me.StartSeparator = s1.First
            Me.EndSeparator = s1.Last
            DoIt()

        End Sub
        Public Sub DoIt()

            For Each File In Files
                Dim X As New SortedList(Of String, Integer)
                For Each Item In Szunyi.IO.Import.Text.Parse(File)
                    Dim s1() = Split(Item, vbTab)
                    Dim cItem = s1(InterestingColIndex)
                    If Szunyi.Text.Lists.ContainStrings(cItem, ForbiddenStrings, False) = False Then
                        If InterestingStrings.Count = 0 OrElse Szunyi.Text.Lists.ContainStrings(cItem, InterestingStrings, False) = True Then
                            Dim Index = X.IndexOfKey(cItem)
                            If Index < 0 Then
                                X.Add(cItem, 1)
                            Else
                                X(cItem) = X(cItem) + 1
                            End If
                        End If
                    End If
                Next
                Save(X, File)
                Dim t As New GetCountWithSubGroups(X, File, Me.StartSeparator, Me.EndSeparator)
            Next
        End Sub
        Private Sub Save(x As SortedList(Of String, Integer), File As FileInfo)
            Dim NewFIle = File.FullName & "WithCount" & ".tab"
            Dim str As New StringBuilder
            For Each Item In x
                str.Append(Item.Key).Append(vbTab).Append(Item.Value).AppendLine()
            Next
            If str.Length > 0 Then str.Length -= 2
            Szunyi.IO.Export.SaveText(str.ToString, New FileInfo(NewFIle))

        End Sub
    End Class
    Public Class GetCountWithSubGroups
        Public Property Files As List(Of FileInfo)
        Public StringColIndex As Integer
        Public CountColIndex As Integer
        Public Motif As String
        Private x As SortedList(Of String, Integer)
        Private file As FileInfo
        Private startSeparator As String
        Private endSeparator As String

        Public Sub New()
            Me.Files = Szunyi.IO.Files.Filter.SelectFiles()
            If IsNothing(Files) = True Then Exit Sub
            Dim Header = Szunyi.IO.Import.Text.GetHeader(Me.Files.First, 1, Nothing, Nothing)

            StringColIndex = input.Input.GetIntrestingColumn(Me.Files.First, 1, "Select String Column")
            CountColIndex = input.Input.GetIntrestingColumn(Me.Files.First, 1, "Select Count Column")

            Motif = InputBox("Enter regexp formula")
                DoIt()
            End Sub

            Public Sub New(x As SortedList(Of String, Integer), file As FileInfo, startSeparator As String, endSeparator As String)
                Me.x = x
                Me.file = file
                Me.startSeparator = startSeparator
                Me.endSeparator = endSeparator
                Dim StrainNames As New SortedList(Of String, Integer)
                Dim SpeciesNames As New SortedList(Of String, Integer)
                Dim GenusNames As New SortedList(Of String, Integer)
                For Each Item In x
                    Dim DistinctFullNames = Szunyi.Text.Regexp.GetDistinctStrings(Item.Key, startSeparator, endSeparator)
                    For Each StrainName In DistinctFullNames
                        Dim c As Char = StrainName.First
                        If Char.IsLetter(c) = True Then

                            If StrainNames.ContainsKey(StrainName) = False Then
                                StrainNames.Add(StrainName, Item.Value)
                            Else
                                StrainNames(StrainName) = StrainNames(StrainName) + Item.Value
                            End If

                            Dim SpeciesName = Szunyi.Text.General.GetTextFromSplitted(StrainName, "_", 2)
                            If SpeciesNames.ContainsKey(SpeciesName) = False Then
                                SpeciesNames.Add(SpeciesName, Item.Value)
                            Else
                                SpeciesNames(SpeciesName) = SpeciesNames(SpeciesName) + Item.Value
                            End If
                            Dim GenusName = Szunyi.Text.General.GetTextFromSplitted(StrainName, "_", 1)
                            If GenusNames.ContainsKey(GenusName) = False Then
                                GenusNames.Add(GenusName, Item.Value)
                            Else
                                GenusNames(GenusName) = GenusNames(GenusName) + Item.Value
                            End If
                        End If

                    Next
                Next
            Save(StrainNames, Szunyi.IO.Files.Get_New_FileName.GetNewFile(file, "_StrainswCounts.tab"))
            Save(SpeciesNames, Szunyi.IO.Files.Get_New_FileName.GetNewFile(file, "_SpecieswCounts.tab"))
            Save(GenusNames, Szunyi.IO.Files.Get_New_FileName.GetNewFile(file, "_GenusswCounts.tab"))
        End Sub

            Public Sub DoIt()
            For Each file In Files
                Dim x As New SortedList(Of String, Integer)
                For Each s In Szunyi.IO.Import.Text.Parse(file)
                    Dim s1() = Split(s, vbTab)
                    Dim tmp = Szunyi.Text.Regexp.GetDistinctStringsbyRegexp(s1(StringColIndex), Motif)
                    For Each stmp In tmp
                        If x.ContainsKey(stmp) = False Then
                            x.Add(stmp, s1(CountColIndex))
                        Else
                            x(stmp) = x(stmp) + s1(CountColIndex)
                        End If
                    Next
                Next
                '  Save(x, New FileInfo(file.FullName)

            Next
        End Sub
            Private Sub Save(x As SortedList(Of String, Integer), File As FileInfo)

                Dim str As New StringBuilder
                For Each Item In x
                    str.Append(Item.Key).Append(vbTab).Append(Item.Value).AppendLine()
                Next
                If str.Length > 0 Then str.Length -= 2
                Szunyi.IO.Export.SaveText(str.ToString, File)
            End Sub
        End Class
    End Namespace
