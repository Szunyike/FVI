Imports System.IO
Imports System.Text
Imports Bio
Imports ClassLibrary1.Szunyi.Other_Database.CrossRefs

Namespace Szunyi
    Namespace Text
        Public Class Dict
            Public Shared Function Get_Distinct_Keys(x As List(Of Dictionary(Of Integer, Integer))) As List(Of Integer)
                Dim out As New List(Of Integer)
                For Each Item In x
                    out.AddRange(Get_Distinct_Keys(Item))
                Next
                out = out.Distinct.ToList
                out.Sort()
                Return out
            End Function
            Public Shared Function Get_Distinct_Keys(x As Dictionary(Of Integer, Integer)) As List(Of Integer)
                Dim out As New List(Of Integer)
                For Each item In x
                    out.Add(item.Key)
                Next
                out = out.Distinct.ToList
                out.Sort()
                Return out
            End Function

            Friend Shared Function Get_as_Text(d As Dictionary(Of String, Integer), Optional separator As String = vbTab) As String
                Dim str As New System.Text.StringBuilder
                For Each i In d
                    str.Append(i.Key).Append(separator).Append(i.Value).AppendLine()
                Next
                Return str.ToString
            End Function
            Public Shared Function Merge(res As Dictionary(Of String, Dictionary(Of String, List(Of Bio.IO.SAM.SAMAlignedSequence)))) As String
                Dim Keys As New List(Of String)
                Dim str As New System.Text.StringBuilder
                str.Append("CDS").Append(vbTab).Append("Min").Append(vbTab).Append("Max").Append(vbTab).Append("Type").Append(vbTab).Append("IsComplementer").Append(vbTab)
                str.Append("Nof CDSs")
                For Each Item In res
                    str.Append(vbTab).Append(Item.Key)
                    For Each Key In Item.Value
                        If Keys.Contains(Key.Key) = False Then Keys.Add(Key.Key)
                    Next
                Next
                str.AppendLine()
                Keys.Sort()
                For Each Key In Keys
                    str.Append(Key).Append(vbTab)
                    Dim s = Split(Key, ",")
                    str.Append(s.Count)
                    For Each Item In res
                        str.Append(vbTab)
                        If Item.Value.ContainsKey(Key) Then
                            str.Append(Item.Value(Key).Count)
                        Else
                            str.Append(0)
                        End If
                    Next
                    str.AppendLine()
                Next
                Return str.ToString

            End Function

            Public Shared Function Merge(res As Dictionary(Of FileInfo, Dictionary(Of String, List(Of Bio.IO.SAM.SAMAlignedSequence)))) As String
                Dim Keys As New List(Of String)
                Dim str As New System.Text.StringBuilder
                str.Append("CDS").Append(vbTab).Append("Min").Append(vbTab).Append("Max").Append(vbTab).Append("Type").Append(vbTab).Append("IsComplementer").Append(vbTab)
                str.Append("Nof CDSs")
                For Each Item In res
                    str.Append(vbTab).Append(Item.Key.Name)
                    For Each Key In Item.Value
                        If Keys.Contains(Key.Key) = False Then Keys.Add(Key.Key)
                    Next
                Next
                str.AppendLine()
                Keys.Sort()
                For Each Key In Keys
                    str.Append(Key).Append(vbTab)
                    Dim s = Split(Key, ",")
                    str.Append(s.Count)
                    For Each Item In res
                        str.Append(vbTab)
                        If Item.Value.ContainsKey(Key) Then
                            str.Append(Item.Value(Key).Count)
                        Else
                            str.Append(0)
                        End If
                    Next
                    str.AppendLine()
                Next
                Return str.ToString
            End Function



            Public Shared Function Merge(res As Dictionary(Of FileInfo, Dictionary(Of String, Integer))) As Object
                Dim Keys As New List(Of String)
                Dim str As New System.Text.StringBuilder
                str.Append("CDS").Append(vbTab).Append("Min").Append(vbTab).Append("Max").Append(vbTab).Append("Type").Append(vbTab).Append("IsComplementer").Append(vbTab)
                str.Append("Nof CDSs")
                For Each Item In res
                    str.Append(vbTab).Append(Item.Key.Name)
                    For Each Key In Item.Value
                        If Keys.Contains(Key.Key) = False Then Keys.Add(Key.Key)
                    Next
                Next
                str.AppendLine()
                Keys.Sort()
                For Each Key In Keys
                    str.Append(Key).Append(vbTab)
                    Dim s = Split(Key, ",")
                    str.Append(s.Count)
                    For Each Item In res
                        str.Append(vbTab)
                        If Item.Value.ContainsKey(Key) Then
                            str.Append(Item.Value(Key))
                        Else
                            str.Append(0)
                        End If
                    Next
                    str.AppendLine()
                Next
                Return str.ToString
            End Function
        End Class
        Public Class General
            Public Shared Function getText(x1 As Dictionary(Of String, List(Of String))) As String
                Dim str As New System.Text.StringBuilder
                Dim out As New List(Of String)
                Dim Index As Integer = 0
                For Each i In x1
                    str.Append(i.Key).Append(vbTab)
                    If Index = 0 Then
                        For Each s In i.Value
                            out.Add(s)
                        Next
                        Index += 1
                    Else
                        For i1 = 0 To i.Value.Count - 1
                            out(i1) = out(i1) & vbTab & i.Value(i1)
                        Next
                    End If
                Next
                str.AppendLine()
                str.Append(Szunyi.Text.General.GetText(out))
                Return str.ToString
            End Function


            Public Shared Function GetColumnIndex(Header As List(Of String), ColumnName As String, s As Szunyi.Constants.TextMatch) As Integer
                Select Case s
                    Case Szunyi.Constants.TextMatch.Contains

                    Case Szunyi.Constants.TextMatch.Contains_SmallAndCapitalIsSame

                    Case Szunyi.Constants.TextMatch.StartWith

                    Case Szunyi.Constants.TextMatch.StartWith_SmallAndCapitalIsSame

                    Case Szunyi.Constants.TextMatch.Exact
                        For i = 0 To Header.Count - 1
                            If Header(i) = ColumnName Then
                                Return i
                            End If
                        Next
                    Case Szunyi.Constants.TextMatch.Exact_SmallAndCapitalIsSame
                        For i = 0 To Header.Count - 1
                            If Header(i).ToLower = ColumnName.ToLower Then
                                Return i
                            End If
                        Next
                End Select
                Return -1
            End Function

            Public Shared Function ToInteger(ls() As String) As Integer()
                Dim out As New List(Of Integer)
                For Each s In ls
                    out.Add(s)
                Next
                Return out.ToArray
            End Function

            Public Shared Function Get_Mt_LocusTags(LocusTags As List(Of String)) As List(Of String)
                Dim out As New List(Of String)
                For Each s In LocusTags
                    out.Add(Get_Mt_LocusTag(s))
                Next
                Return out
            End Function
            Public Shared Function Get_Short_Mt_LocusTag(LocusTag As String) As String
                Dim i = LocusTag.IndexOf(".")
                If i = -1 Then
                    Return LocusTag
                Else
                    Return LocusTag.Substring(0, i)
                End If

            End Function
            Public Shared Function Get_Mt_LocusTag(LocusTag As String) As String


                Dim i = LocusTag.IndexOf(".")
                If i = -1 Then
                    Return LocusTag
                Else
                    Dim i2 = LocusTag.IndexOf("_", i)
                    Dim i3 = LocusTag.IndexOf(" ", i)
                    Dim Min = GetSmallerButNotMinus1(i2, i3)
                    If Min = -1 Then
                        Return LocusTag.Substring(0, i)
                    Else
                        Return LocusTag.Substring(0, Min)
                    End If
                End If

            End Function
            Private Shared Function GetSmallerButNotMinus1(i1 As Integer, i2 As Integer)
                Dim i As New List(Of Integer)
                If i1 > -1 Then i.Add(i1)
                If i2 > -1 Then i.Add(i2)
                If i.Count = 0 Then Return -1
                Return i.Min
            End Function
            Public Shared Function Multiply(s As String, Nof As Integer) As String
                Dim str As New System.Text.StringBuilder
                For i1 = 1 To Nof
                    str.Append(s)
                Next
                Return str.ToString
            End Function

            Friend Shared Function Add_Before_And_End(names As List(Of String), ToAdd As String) As List(Of String)
                Dim out As New List(Of String)
                For Each Name In names
                    out.Add(ToAdd & Name & ToAdd)
                Next
                Return out
            End Function

            Friend Shared Function Insert(namesII As List(Of String), Insertion As String) As Object
                Dim str As New System.Text.StringBuilder
                For i1 = 0 To namesII.Count - 2
                    str.Append(namesII(i1)).Append(Insertion)
                Next
                str.Append(namesII.Last)
                Return str.ToString
            End Function

            Friend Shared Function GetDictinctList(Items As List(Of String)) As List(Of String)
                Dim t = Items.Distinct
                Items = t.ToList
                Items.Sort()
                Return Items
            End Function
            ''' <summary>
            ''' Return Index if cloumn header cell contains both of the string 
            ''' If no return -1
            ''' </summary>
            ''' <param name="FirstString"></param>
            ''' <param name="SecondString"></param>
            ''' <param name="Header"></param>
            ''' <returns></returns>

            Shared Function GetColumnNumbers(FirstString As String, SecondString As String, Header As List(Of String)) As Integer
                For i1 = 0 To Header.Count - 1
                    If Header(i1).ToUpper.Contains(FirstString.ToUpper) AndAlso Header(i1).ToUpper.Contains(SecondString.ToUpper) Then Return i1
                Next
                Return -1
            End Function
            ''' <summary>
            ''' If header cloumn cell text equal with Stringtosearch return index
            ''' if no return -1
            ''' </summary>
            ''' <param name="StringToSearch"></param>
            ''' <param name="Header"></param>
            ''' <returns></returns>
            Shared Function GetColumnNumbers(StringToSearch As String, Header As List(Of String)) As Integer
                For i1 = 0 To Header.Count - 1
                    If Header(i1).ToUpper = StringToSearch.ToUpper Then Return i1
                Next
                Return -1
            End Function
            Public Shared Function GetText(Bytes As List(Of Byte)) As String
                Return System.Text.Encoding.ASCII.GetString(Bytes.ToArray())
            End Function
            Public Shared Function GetText(x As Dictionary(Of String, Integer)) As String
                Dim str As New System.Text.StringBuilder
                For Each item In x
                    str.Append(item.Key).Append(vbTab).Append(item.Value).AppendLine()
                Next
                If str.Length > 0 Then str.Length -= 2
                Return str.ToString
            End Function
            Public Shared Function GetText(x As Dictionary(Of Integer, Integer), sep1 As String, sep2 As String) As String
                Dim str As New System.Text.StringBuilder
                For Each item In x
                    If str.Length = 0 Then
                        str.Append(item.Key).Append(sep1).Append(item.Value).Append(sep2)
                    Else
                        str.Append(sep1)
                        str.Append(item.Key).Append(sep1).Append(item.Value).Append(sep2)
                    End If
                Next
                If str.Length > 0 Then str.Length -= sep2.Length
                Return str.ToString
            End Function

            ''' <summary>
            ''' If header cloumn cell text equal with Stringtosearch return index
            ''' if no return -1
            ''' </summary>
            ''' <param name="StringsToSearch"></param>
            ''' <param name="Header"></param>
            ''' <returns></returns>
            Shared Function GetColumnNumbers(StringsToSearch As List(Of String), Header As List(Of String)) As List(Of Integer)
                Dim Out As New List(Of Integer)
                For i1 = 0 To StringsToSearch.Count - 1
                    StringsToSearch(i1) = StringsToSearch(i1).ToUpper
                Next
                StringsToSearch.Sort()
                For i1 = 0 To Header.Count - 1
                    Dim Index = StringsToSearch.BinarySearch(Header(i1).ToUpper)
                    If Index > -1 Then
                        Out.Add(i1)
                    End If
                Next
                Return Out
            End Function

            Friend Shared Function SplitIntoLong(txt As String, Separator As String) As List(Of Long)
                Dim s1() = Split(txt, Separator)
                Dim out As New List(Of Long)
                For Each s In s1
                    s = s.Trim
                    If s <> String.Empty Then
                        Try
                            out.Add(s)
                        Catch ex As Exception

                        End Try
                    End If
                Next
                Return out
            End Function

            Public Shared Function GetTextFromSplitted(txt As String, Separator As String, NofItem As Integer, Optional NewSeparator As String = " ") As String
                Dim s1() = Split(txt, Separator)
                Dim out As New StringBuilder
                If s1.Count < NofItem Then
                    For Each s In s1
                        out.Append(s).Append(NewSeparator)
                    Next
                    If out.Length > 0 Then out.Length -= Separator.Length
                Else
                    For i1 = 0 To NofItem - 1
                        out.Append(s1(i1)).Append(NewSeparator)
                    Next
                    If out.Length > 0 Then out.Length -= Separator.Length
                End If
                Return out.ToString
            End Function

            Public Shared Function GetMaxNofColumn(x As List(Of String())) As Integer
                Dim MaxNofColumn As Integer = 0
                For Each x1 In x
                    If x1.Count > MaxNofColumn Then MaxNofColumn = x1.Count
                Next
                Return MaxNofColumn
            End Function

            ''' <summary>
            ''' Remove All Of The Substrings From Strings
            ''' </summary>
            ''' <param name="OriginalStrings"></param>
            ''' <param name="StringsToRemove"></param>
            ''' <returns></returns>
            Friend Shared Function RemoveFromStrings(OriginalStrings As List(Of String), StringsToRemove As List(Of String)) As List(Of String)
                For Each OriginalString In OriginalStrings
                    OriginalString = RemoveFromString(OriginalString, StringsToRemove)
                Next
                Return OriginalStrings

            End Function
            ''' <summary>
            ''' Remove All Of The Substrings From Strings
            ''' </summary>
            ''' <param name="OriginalStrings"></param>
            ''' <param name="StringsToRemove"></param>
            ''' <returns></returns>
            Friend Shared Function RemoveFromStrings(OriginalStrings As List(Of String), StringsToRemove As String) As List(Of String)
                Dim out As New List(Of String)
                For Each OriginalString In OriginalStrings
                    out.Add(RemoveFromString(OriginalString, StringsToRemove))
                Next
                Return out

            End Function
            ''' <summary>
            ''' Remove All Of The Substrings From String
            ''' </summary>
            ''' <param name="OriginalString"></param>
            ''' <param name="StringsToRemove"></param>
            ''' <returns></returns>
            Public Shared Function RemoveFromString(OriginalString As String, StringsToRemove As List(Of String)) As String
                If IsNothing(StringsToRemove) = True Then Return OriginalString
                For Each StringToRemove In StringsToRemove
                    OriginalString = OriginalString.Replace(StringToRemove, "")
                Next

                Return OriginalString
            End Function
            ''' <summary>
            ''' Remove All Of The Substrings From String
            ''' </summary>
            ''' <param name="OriginalString"></param>
            ''' <param name="StringToRemove"></param>
            ''' <returns></returns>
            Public Shared Function RemoveFromString(OriginalString As String, StringToRemove As String) As String


                OriginalString = OriginalString.Replace(StringToRemove, "")

                Return OriginalString
            End Function
            Public Shared Function RemoveFromString(OriginalString As String, Chars() As Char) As String
                For Each c In Chars
                    OriginalString = OriginalString.Replace(c, "")
                Next

                Return OriginalString
            End Function

            Friend Shared Function Get_Nof_Occurence(s As String, c As Char) As Integer
                Return s.Count(Function(t) t = c)
            End Function
#Region "Get SubItem From Splitted txt"
            ''' <summary>
            ''' Return the first part of strings defined length
            ''' </summary>
            ''' <param name="dsOfAll"></param>
            ''' <param name="length"></param>
            ''' <returns></returns>
            Public Shared Function GetFirstParts(dsOfAll As List(Of String), length As Integer) As List(Of String)
                Dim out As New List(Of String)
                For Each s In dsOfAll
                    out.Add(GetFirstPart(s, length))
                Next
                Return out
            End Function

            Public Shared Function Is_Part_Contains(header As String, paramater As String, separator As String) As Boolean
                For Each s In Split(header, separator)
                    If s = paramater Then Return True
                Next
                Return False
            End Function

            ''' <summary>
            ''' return the first part of the string defined by length
            ''' </summary>
            ''' <param name="s"></param>
            ''' <param name="Length"></param>
            ''' <returns></returns>
            Public Shared Function GetFirstPart(s As String, Length As Integer) As String
                If s.Length >= Length Then
                    Return s.Substring(0, Length)
                Else
                    Return s
                End If
            End Function

            ''' <summary>
            ''' Return the first part of strings splitted by Separator
            ''' </summary>
            ''' <param name="Words"></param>
            ''' <param name="Separator"></param>
            ''' <returns></returns>
            Public Shared Function GetFirstParts(Words As List(Of String), Separator As String) As List(Of String)
                Dim out As New List(Of String)
                For Each s In Words
                    out.Add(GetFirstPart(s, Separator))
                Next
                Return out
            End Function
            ''' <summary>
            ''' return the first part of the string defined separator
            ''' </summary>
            ''' <param name="s"></param>
            ''' <param name="Separator"></param>
            ''' <returns></returns>
            Public Shared Function GetFirstPart(s As String, Separator As String) As String
                Dim s1() = Split(s, Separator)
                Return s1.First
            End Function

            Public Shared Function Get_Last_Parts(Words As List(Of String), Separator As String) As List(Of String)
                Dim out As New List(Of String)
                For Each Word In Words
                    out.Add(Get_Last_Part(Word, Separator))
                Next
                Return out
            End Function
            Public Shared Function Get_Last_Part(Word As String, Separator As String) As String
                Return Split(Word, Separator).Last
            End Function
            Public Shared Function Get_Parts(Words As List(Of String), Separator As String, Index As Integer) As List(Of String)
                Dim out As New List(Of String)
                For Each Word In Words
                    out.Add(Get_Part(Word, Separator, Index))
                Next
                Return out
            End Function
            Public Shared Function Get_Part(Word As String, Separator As String, Index As Integer) As String
                Dim s = Split(Word, Separator)
                Return s(Index)
            End Function

            Public Shared Function Get_Not_Last_Parts(Words As List(Of String), Separator As String) As List(Of String)
                Dim out As New List(Of String)
                For Each Word In Words
                    out.Add(Get_Not_Last_Part(Word, Separator))
                Next
                Return out
            End Function
            Public Shared Function Get_Not_Last_Part(Word As String, Separator As String) As String
                Dim Index = Word.LastIndexOf(Separator)
                If Index > -1 Then
                    Return Word.Substring(0, Index)
                Else
                    Return Word
                End If

            End Function
            Public Shared Function Get_Not_First_Parts(Words As List(Of String), Separator As String) As List(Of String)
                Dim out As New List(Of String)
                For Each Word In Words
                    out.Add(Get_Not_First_Part(Word, Separator))
                Next
                Return out
            End Function
            Public Shared Function Get_Not_First_Part(Word As String, Separator As String) As String
                Dim Index = Word.IndexOf(Separator)
                If Index > -1 Then
                    Return Word.Substring(Index + 1, Word.Length - Index - 1)
                Else
                    Return Word
                End If

            End Function

#End Region

#Region "Get Text"
            Shared Function GetText(x As String) As String
                Return x
            End Function
            Shared Function GetText(x() As String, Optional Separator As String = vbCrLf) As String
                Dim str As New StringBuilder
                For Each s In x
                    str.Append(s).Append(Separator)
                Next
                If str.Length > 0 Then str.Length -= Separator.Length
                Return str.ToString
            End Function

            Shared Function GetText(list As List(Of Long), Optional Separator As String = vbCrLf) As String
                Dim out As New StringBuilder
                For Each Item In list
                    out.Append(Item).Append(Separator)
                Next
                If out.Length > 0 Then out.Length -= Separator.Length
                Return out.ToString
            End Function

            Shared Function GetText(List As List(Of Double), Optional Separator As String = vbCrLf) As String
                Dim out As New StringBuilder
                For Each Item In List
                    out.Append(Item).Append(Separator)
                Next
                If out.Length > 0 Then out.Length -= Separator.Length
                Return out.ToString
            End Function
            Public Shared Function GetText(x As List(Of String), Optional Separator As String = vbCrLf) As String
                Dim str As New StringBuilder
                If IsNothing(x) = True Then Return String.Empty
                For Each s In x
                    str.Append(s).Append(Separator)
                Next
                If str.Length >= Separator.Length Then str.Length -= Separator.Length
                Return str.ToString
            End Function
            Public Shared Function GetText(x As List(Of Integer), Optional Separator As String = vbCrLf) As String
                Dim str As New StringBuilder
                For Each s In x
                    str.Append(s).Append(Separator)
                Next
                If str.Length >= Separator.Length Then str.Length -= Separator.Length
                Return str.ToString
            End Function
            Public Shared Function GetText(x As Integer(), Optional Separator As String = vbCrLf) As String
                Dim str As New StringBuilder
                For Each s In x
                    str.Append(s).Append(Separator)
                Next
                If str.Length >= Separator.Length Then str.Length -= Separator.Length
                Return str.ToString
            End Function
            Public Shared Function GetText(s1() As String, valuesColIndexes As List(Of Integer), Optional separator As String = vbTab) As String
                Dim out As New StringBuilder
                For Each Item In valuesColIndexes
                    If Item < s1.Count Then
                        out.Append(s1(Item)).Append(separator)
                    Else
                        out.Append(String.Empty).Append(separator)
                    End If

                Next
                out.Length -= separator.Length
                Return out.ToString
            End Function

#End Region

            Public Shared Function GetTextList(txt As String) As List(Of String)
                Dim out As New List(Of String)
                out.Add(txt)
                Return out
            End Function

            Public Shared Function GetStringFromInputbox(Title As String) As String
                Return InputBox(Title)
            End Function

            Public Shared Sub ReplaceStringInFile(txtToReplace As String, txtNewString As String, file As FileInfo)
                Using sr As New StreamReader(file.FullName)
                    Dim t = file.Name.Replace(file.Extension, "") & "Md" & file.Extension
                    Using sw As New StreamWriter(file.DirectoryName & "\" & t)
                        Do
                            sw.Write(sr.ReadLine.Replace(txtToReplace, txtNewString) & vbCrLf)
                        Loop Until sr.EndOfStream = True
                    End Using
                End Using

            End Sub
            Public Shared Sub ReplaceStringInFile(txtToReplace As String, txtNewString As String, files As List(Of FileInfo))
                For Each File In files
                    ReplaceStringInFile(txtToReplace, txtNewString, File)
                Next
            End Sub

            ''' <summary>
            ''' Return an Integer or Nothing
            ''' </summary>
            ''' <param name="txt"></param>
            ''' <returns></returns>
            Public Shared Function GetIntegerFromInputbox(Optional txt As String = "Enter an Integer") As Integer
                Dim s = InputBox(txt)
                Try
                    Dim t As Integer = s
                    Return t
                Catch ex As Exception
                    MsgBox(ex.ToString)
                    Return Nothing
                End Try
            End Function

            Public Shared Function GetNofAA(v As String, seq As Sequence) As String
                Dim tmp = seq.ConvertToString(0, seq.Count)
                Dim Count As Integer = 0
                For Each c In tmp
                    If c = v Then Count += 1
                Next
                Return Count
            End Function

            Public Shared Function Merge_Line_By_LIne(sg As List(Of String)) As String
                Dim x As New List(Of List(Of String))
                For Each s In sg
                    If IsNothing(s) = False Then
                        Dim l = Split(s, vbCrLf)
                        x.Add(l.ToList)
                    End If
                Next
                Dim max = (From t In x Select t.Count).Max

                For Each s In x
                    Do
                        If s.Count < max Then
                            s.Add(Szunyi.Text.General.Multiply(vbTab, 4))
                        Else
                            Exit Do
                        End If
                    Loop

                Next


                Dim Out As New List(Of String)
                For Each item In x
                    item.Sort()
                Next
                Dim Start As Boolean = True
                For Each s In x
                    For i1 = 0 To s.Count - 1
                        If Start = True Then
                            Out.Add(s(i1))
                        Else

                            Out(i1) = Out(i1) & vbTab & s(i1)
                        End If
                    Next
                    Start = False
                Next
                Return Szunyi.Text.General.GetText(Out)
            End Function
        End Class

        Public Class Table
            Shared Function GetDictionaryFromTable(File As FileInfo, IDColumn As Integer, ValueColumn As Integer,
                                               Optional Separator As String = vbTab, Optional ToRemove As List(Of String) = Nothing) As Dictionary(Of String, String)
                Dim Res As New Dictionary(Of String, String)
                Using sr As New StreamReader(File.FullName)
                    sr.ReadLine() ' Skip Header
                    Do
                        Dim s1() = Split(sr.ReadLine, Separator)
                        Res.Add(s1(IDColumn).Replace(Chr(34), ""), s1(ValueColumn).Replace(Chr(34), ""))
                    Loop Until sr.EndOfStream = True
                End Using
                Return Res
            End Function

            Shared Function GetTextFrom2LevelDictionary(TwoLevelDictionary As Dictionary(Of String, Dictionary(Of String, Long)), Optional Separator As String = vbTab) As String
                Dim out As New System.Text.StringBuilder
                Dim SampleNames = TwoLevelDictionary.First.Value.Keys.ToList
                SampleNames.Sort()


                out.Append("FeatureID").Append(vbTab).Append(Szunyi.Text.General.GetText(SampleNames, vbTab))
                For Each Item In TwoLevelDictionary
                    If Item.Value.Count = SampleNames.Count Then
                        out.AppendLine.Append(Item.Key)
                        For Each SampleName In SampleNames

                            out.Append(vbTab).Append(Item.Value(SampleName))
                        Next
                    End If

                Next

                Return out.ToString
            End Function

            Shared Function GetTextFrom2ndLevelDictionary(SampleDescription As Dictionary(Of String, Dictionary(Of String, FileInfo))) As String
                Dim str As New System.Text.StringBuilder
                For Each Item In SampleDescription
                    For Each SuBitem In Item.Value
                        str.Append(SuBitem.Key.Replace(" ", "_")).Append(vbTab)
                    Next
                Next
                str.Length -= 1
                Return str.ToString
            End Function

            Public Sub ReplaceStringsInAnotherColumn(crossTable As List(Of CrossRefOneToOne), indexOfOriginalString As Integer, indexOfNewString As Integer, files As List(Of FileInfo), outputFolder As String)
                Dim TheComp As New Szunyi.Other_Database.CrossRefs.ComparerOfCrossrefOneToOneByFirst
                For Each File In files
                    Dim str As New StringBuilder
                    For Each Line In Szunyi.IO.Import.Text.ParseToArray(File, vbTab)
                        Dim Index = crossTable.BinarySearch(New Szunyi.Other_Database.CrossRefs.CrossRefOneToOne(Line(indexOfOriginalString), ""), TheComp)
                        If Index > -1 Then
                            Line(indexOfNewString) = crossTable(Index).Second.Replace(Chr(34), "")
                        End If

                        str.Append(Szunyi.Text.General.GetText(Line, vbTab)).AppendLine()
                    Next
                    If str.Length > 0 Then str.Length -= 2
                    Szunyi.IO.Export.SaveText(str.ToString, New FileInfo(outputFolder & "\" & File.Name))
                Next
            End Sub

            Public Shared Sub DeleteFirstColumns(files As List(Of FileInfo))
                For Each File In files
                    Dim str As New System.Text.StringBuilder
                    For Each Line In Szunyi.IO.Import.Text.ParseToArray(File, vbTab)

                        For i1 = 1 To Line.Count - 1
                            str.Append(Line(i1)).Append(vbTab)
                        Next
                        str.Length -= 1
                        str.AppendLine()
                    Next
                    str.Length -= 2
                    Szunyi.IO.Export.SaveText(str.ToString, File)
                Next
            End Sub

            Public Shared Sub DeleteLastColumns(files As List(Of FileInfo))
                For Each File In files
                    Dim str As New System.Text.StringBuilder
                    For Each Line In Szunyi.IO.Import.Text.ParseToArray(File, vbTab)

                        For i1 = 0 To Line.Count - 2
                            str.Append(Line(i1)).Append(vbTab)
                        Next
                        str.Length -= 1
                        str.AppendLine()
                    Next
                    str.Length -= 1

                    Szunyi.IO.Export.SaveText(str.ToString, File)
                Next
            End Sub
        End Class
    End Namespace
End Namespace


