Namespace Szunyi
    Namespace Text

        Public Class Lists
            Public Shared Function Get_Lines_Not_StartWith(Lines As List(Of String), start As String) As List(Of String)
                Dim out As New List(Of String)
                For Each line In Lines
                    If line.StartsWith(start) = False Then out.Add(line)
                Next
                Return out
            End Function
            Public Shared Function Get_Every_Xth_Lines(Lines As List(Of String), start As Integer, xth As Integer) As List(Of String)
                Dim out As New List(Of String)
                For i1 = start To Lines.Count - 1 Step xth
                    out.Add(Lines(i1))
                Next
                Return out
            End Function
            Public Shared Function Get_Every_Xth_Lines_as_String(Lines As List(Of String), start As Integer, xth As Integer) As String
                Dim OUT = Get_Every_Xth_Lines(Lines, start, xth)
                Return Szunyi.Text.General.GetText(OUT, "")
            End Function

            Public Shared Function Insert_Text_in_Every_LineStarts(Lines As List(Of String), ToInsert As String) As List(Of String)
                Dim out As New List(Of String)
                For Each Line In Lines
                    out.Add(ToInsert & Line)
                Next
                Return out
            End Function
            Public Shared Iterator Function ItearateByStart(Lines As List(Of String), separator As String) As IEnumerable(Of List(Of String))
                Dim out As New List(Of String)
                For Each Line In Lines
                    If Line.StartsWith(separator) = False Then
                        out.Add(Line)
                    Else
                        Yield (out)
                        out.Clear()
                    End If
                Next
                If out.Count > 0 Then Yield out
            End Function
            Public Shared Function GetLinesContains(Lines As List(Of String), Contain As String) As List(Of String)
                Dim out As New List(Of String)
                For i1 = 0 To Lines.Count - 1
                    If Lines(i1).Contains(Contain) = True Then
                        out.Add(Lines(i1))
                    End If
                Next
                Return out
            End Function
            Public Shared Function GetLineStartWith(Lines As List(Of String), start As String) As String
                For i1 = 0 To Lines.Count - 1
                    If Lines(i1).StartsWith(start) = True Then
                        Return Lines(i1)
                    End If
                Next
                Return String.Empty
            End Function
            Public Shared Function GetLinesBetween(Lines As List(Of String), start As String, Endy As String) As List(Of String)
                Dim out As New List(Of String)
                For i1 = 0 To Lines.Count - 1
                    If Lines(i1).StartsWith(start) = True Then
                        Dim i2 As Integer = i1 + 1
                        Do Until i2 = Lines.Count
                            If Lines(i2).StartsWith(Endy) Then Exit Do
                            out.Add(Lines(i2))
                            i2 += 1
                            If Lines.Count = i2 Then
                                Exit Do
                            End If
                        Loop
                        Return out
                    End If
                Next
                Return out
            End Function



            Public Shared Function RemoveEmptyLines(Lines As List(Of String)) As List(Of String)
                Dim out As New List(Of String)
                For Each Line In Lines
                    If Line <> String.Empty Then
                        out.Add(Line)
                    End If
                Next
                Return out
            End Function
            Public Shared Function Get_String_List(NUmbers As List(Of Integer)) As List(Of String)
                Dim out As New List(Of String)
                For Each nu In NUmbers
                    out.Add(nu)
                Next
                Return out
            End Function
            Public Shared Function Get_String_List(NUmbers As List(Of Double)) As List(Of String)
                Dim out As New List(Of String)
                For Each nu In NUmbers
                    out.Add(nu)
                Next
                Return out
            End Function
            Public Shared Function GetListFormText(t As String) As List(Of String)
                Dim out As New List(Of String)
                out.Add(t)
                Return out
            End Function

            Public Shared Function CloneStrings(x As List(Of String)) As List(Of String)
                Dim out As New List(Of String)
                For Each Item In x
                    out.Add(Item)
                Next
                Return out
            End Function

            Public Shared Function Ignore_Empty_Or_StartWiths(Lines As List(Of String), ToIgnore As String) As List(Of String)
                Dim Out As New List(Of String)
                For Each Line In Lines
                    If Line.StartsWith(ToIgnore) = False And Line <> String.Empty Then
                        Out.Add(Line)
                    End If
                Next
                Return Out
            End Function
            Public Shared Function Ignore_Empty_Or_StartWiths(Lines As List(Of String), ToIgnores As List(Of String)) As List(Of String)
                Dim tmp As New List(Of String)
                tmp = Lines
                For Each ToIgnore In ToIgnores
                    tmp = Ignore_Empty_Or_StartWiths(tmp, ToIgnore)
                Next
                Return tmp
            End Function
            ''' <summary>
            ''' Return the indox or -1
            ''' </summary>
            ''' <param name="header"></param>
            ''' <param name="For_Search"></param>
            ''' <returns></returns>
            Public Shared Function Get_Index_Contains(header As List(Of String), For_Search As String) As Integer
                For i1 = 0 To header.Count - 1
                    If header(i1).Contains(For_Search) = True Then
                        Return i1
                    End If
                Next
                Return -1

            End Function
            ''' <summary>
            ''' Return the indox or -1
            ''' </summary>
            ''' <param name="header"></param>
            ''' <param name="For_Search"></param>
            ''' <returns></returns>
            Public Shared Function Get_Index(header As List(Of String), For_Search As String) As Integer
                For i1 = 0 To header.Count - 1
                    If header(i1) = For_Search = True Then
                        Return i1
                    End If
                Next
                Return -1

            End Function

            ''' <summary>
            ''' Return a new list startr with given position
            ''' </summary>
            ''' <param name="Items"></param>
            ''' <param name="FirstItemIndex"></param>
            ''' <returns></returns>
            Public Shared Function GetSubList(Items As List(Of String), FirstItemIndex As Integer) As List(Of String)
                Dim out As New List(Of String)
                For i1 = FirstItemIndex To Items.Count - 1
                    out.Add(Items(i1))
                Next
                Return out
            End Function

            Public Shared Function Get_Integers(s As String, Separator As String) As Integer()
                s = s.Trim(" ")
                Dim s1() = Split(s, Separator)
                Dim out As New List(Of Integer)
                For i1 = 0 To s1.Count - 1
                    If s1(i1) <> "" Then
                        out.Add(s1(i1).Trim)
                    End If
                Next
                Return out.ToArray
            End Function
            Public Shared Function Get_Doubles(s As String, Separator As String) As Double()
                s = s.Trim(" ")
                Dim s1() = Split(s, Separator)
                Dim out(s1.Count - 1) As Double
                For i1 = 0 To s1.Count - 1
                    out(i1) = s1(i1)
                Next
                Return out
            End Function

            ''' <summary>
            ''' Return True if contains in one of the string: else return False
            ''' Case sensitivitymust be set
            ''' </summary>
            ''' <param name="txt"></param>
            ''' <param name="Strings"></param>
            ''' <param name="CaseSensitive"></param>
            ''' <returns></returns>
            Public Shared Function ContainStrings(txt As String, Strings As List(Of String), CaseSensitive As Boolean) As Boolean
                If CaseSensitive = True Then
                    For Each s In Strings
                        If txt.Contains(s) = True Then Return True
                    Next
                Else
                    Dim Index As Integer = 0
                    For Each s In Strings
                        If txt.IndexOf(s) > -1 Then Return True
                    Next
                End If
                Return False
            End Function

            Friend Shared Function Merge_Lists(First_Items As List(Of String), Second_Items As List(Of String), Sepatator As String) As List(Of String)
                Dim Out As New List(Of String)
                For i1 = 0 To First_Items.Count - 1
                    Out.Add(First_Items(i1) & Sepatator & Second_Items(i1))
                Next
                Return Out
            End Function

            Public Shared Function Split_Items(Items As List(Of String), Separator As String) As List(Of String)
                Dim out As New List(Of String)
                For Each Item In Items

                    out.AddRange(Split(Item, Separator))
                Next
                Return out
            End Function
            Public Shared Function Split_Items_From_First(Items As List(Of String), Separator As String) As List(Of String)
                Dim out As New List(Of String)
                For Each Item In Items
                    Dim s = Split(Item, Separator)
                    out.Add(s.First)
                Next
                Return out.Distinct.ToList
            End Function
            Public Shared Function Split_Items_From_Last(Items As List(Of String), Separator As String) As List(Of String)
                Dim out As New List(Of String)
                For Each Item In Items
                    Dim s = Split(Item, Separator)
                    out.Add(s.Last)
                Next
                Return out.Distinct.ToList
            End Function
            Public Shared Function Split_Items_Not_Last(Items As List(Of String), Separator As String) As List(Of String)
                Dim out As New List(Of String)
                For Each Item In Items

                    out.Add(Split_Item_Not_Last(Item, Separator))
                Next
                Return out.Distinct.ToList
            End Function
            Public Shared Function Split_Item_Not_Last(item As String, Separator As String) As String
                Dim s = Split(item, Separator)
                Dim tmp As String = ""
                For i1 = 0 To s.Count - 2
                    tmp = tmp & Separator & s(i1)
                Next
                Return tmp.Trim(Separator)
            End Function
            Public Shared Function Get_Doubles_Empty_strings_Ignored(s() As String) As List(Of Double)
                Dim out As New List(Of Double)
                For Each Item In s
                    If Item <> String.Empty Then
                        out.Add(Item)
                    End If

                Next
                Return out
            End Function

            Public Shared Function Replace(ls As List(Of String), OriS As String, newS As String) As Object
                Dim out As New List(Of String)
                For Each Item In ls
                    out.Add(Item.Replace(OriS, newS))
                Next
                Return out
            End Function
            Public Shared Function Merge(l1() As Integer, l2() As Integer, separator As String, separator2 As String) As String
                If IsNothing(l1) = True Or IsNothing(l2) = True Then
                    Return String.Empty
                End If
                If l1.Count <> l2.Count Then
                    Return String.Empty
                Else
                    Dim str As New System.Text.StringBuilder
                    For i1 = 0 To l1.Count - 1
                        str.Append(l1(i1))
                        str.Append(separator)
                        str.Append(l2(i1))
                        str.Append(separator2)
                    Next
                    If str.Length > 0 Then
                        str.Length -= separator2.Length
                    End If
                    Return str.ToString
                End If
            End Function

            Public Shared Function Merge(l1 As List(Of String), l2 As List(Of String), separator As String, separator2 As String) As String
                If IsNothing(l1) = True Or IsNothing(l2) = True Then
                    Return String.Empty
                End If
                If l1.Count <> l2.Count Then
                    Return String.Empty
                Else
                    Dim str As New System.Text.StringBuilder
                    For i1 = 0 To l1.Count - 1
                        str.Append(l1(i1))
                        str.Append(separator)
                        str.Append(l2(i1))
                        str.Append(separator2)
                    Next
                    If str.Length > 0 Then
                        str.Length -= separator2.Length
                    End If
                    Return str.ToString
                End If
            End Function
        End Class
    End Namespace
End Namespace


