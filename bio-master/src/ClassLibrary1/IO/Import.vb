Imports System.IO
Imports System.Text
Imports Bio
Imports Bio.IO
Imports Bio.IO.GenBank
Imports Bio.Web.Blast
Imports ClassLibrary1.Szunyi.Comparares
Imports ClassLibrary1.Szunyi.Constants

Namespace Szunyi
    Namespace IO
        Public Class Import
            Public Class NotShared1File
                Public Property TheFIle As FileInfo

                Public Sub New(file As FileInfo)
                    Me.TheFIle = file
                End Sub
                Public Iterator Function ParseToArrayNotShared(Separator As String, FirstLineIndex As Integer) As IEnumerable(Of String())
                    If TheFIle.Exists = True Then
                        Using sr As New StreamReader(TheFIle.FullName)
                            For i1 = 1 To FirstLineIndex
                                sr.ReadLine()
                            Next
                            Do
                                Yield Split(sr.ReadLine, Separator)
                            Loop Until sr.EndOfStream = True
                        End Using
                    End If
                End Function
                Public Function Get_All(Separator As String, FirstLineIndex As Integer) As List(Of String())
                    Dim out As New List(Of String())
                    If TheFIle.Exists = True Then
                        Using sr As New StreamReader(TheFIle.FullName)
                            For i1 = 1 To FirstLineIndex
                                sr.ReadLine()
                            Next
                            Do
                                out.Add(Split(sr.ReadLine, Separator))
                            Loop Until sr.EndOfStream = True
                        End Using
                    End If
                    Return out
                End Function
            End Class
            Public Class Text
                Public Shared Function ReadToEnd(file As FileInfo) As String
                    Using sw As New StreamReader(file.FullName)
                        Return sw.ReadToEnd
                    End Using
                End Function

                Public Shared Function GetHeader(TheFile As FileInfo, NofHeaderLine As Integer,
                                         Optional ToRemove As List(Of String) = Nothing) As List(Of String)
                    Dim res As New List(Of String)
                    Dim s1() As String
                    If IsNothing(TheFile) = True Then Return Nothing
                    Using sr As New StreamReader(TheFile.FullName)
                        If IsNothing(ToRemove) = True Then
                            For i1 = 1 To NofHeaderLine
                                s1 = Split(sr.ReadLine, vbTab)
                                If res.Count = 0 Then
                                    res = s1.ToList
                                Else
                                    For i2 = 0 To s1.Count - 1
                                        res(i2) = res(i2) & " " & s1(i2)
                                    Next
                                End If
                            Next
                        Else
                            For i1 = 1 To NofHeaderLine
                                Dim Line As String = sr.ReadLine
                                For Each Item In ToRemove
                                    Line = Line.Replace(Item, "")
                                Next
                                s1 = Split(Line, vbTab)
                                If IsNothing(res) = True Then
                                    res = s1.ToList
                                Else
                                    For i2 = 0 To s1.Count - 1
                                        res(i2) = res(i2) & " " & s1(i2)
                                    Next
                                End If
                            Next
                        End If
                    End Using
                    Return res
                End Function

                Public Shared Function GetHeader(TheFile As FileInfo, NofHeaderLine As Integer,
                                          ToRemove As List(Of String),
                                          InterestingColumnsIDs As List(Of Integer)) As List(Of String)

                    Dim res As New List(Of String)
                    Dim resII As New Dictionary(Of Integer, String)
                    Dim s1() As String
                    If IsNothing(TheFile) = True Then Return Nothing
                    Using sr As New StreamReader(File.OpenRead(TheFile.FullName))
                        If NofHeaderLine = 0 Then
                            Do
                                Dim Line = sr.ReadLine
                                If Line.StartsWith("#") = False Then
                                    res = Split(Line, vbTab).ToList
                                    Exit Do
                                End If
                            Loop Until sr.EndOfStream = True
                        Else

                            If IsNothing(InterestingColumnsIDs) = True Then
                                For i1 = 1 To NofHeaderLine
                                    s1 = Split(sr.ReadLine, vbTab)
                                    If res.Count = 0 Then
                                        res = s1.ToList
                                    Else
                                        For i2 = 0 To s1.Count - 1
                                            res(i2) = res(i2) & " " & s1(i2)
                                        Next
                                    End If
                                Next
                            Else
                                For i1 = 1 To NofHeaderLine
                                    s1 = Split(sr.ReadLine, vbTab)
                                    For i2 = 0 To s1.Count - 1
                                        If InterestingColumnsIDs.Contains(i2) = True Then
                                            If resII.ContainsKey(i2) = False Then
                                                resII.Add(i2, s1(i2))
                                            Else
                                                resII(i2) = resII(i2) & " " & s1(i2)
                                            End If
                                        End If
                                    Next
                                Next
                                res = resII.Values.ToList
                            End If
                        End If

                        If IsNothing(ToRemove) = False Then res = Szunyi.Text.General.RemoveFromStrings(res, ToRemove)

                    End Using
                    Return res
                End Function

                Shared Function GetListForReplace(File As FileInfo, ColumnOfOriginalString As Integer, ColumnOfNewString As Integer) As List(Of String())
                    Dim res As New List(Of String())
                    If ColumnOfOriginalString = -1 Or ColumnOfNewString = -1 Then Return Nothing
                    Using sr As New StreamReader(File.FullName)
                        sr.ReadLine()
                        Do
                            Dim s1() = Split(sr.ReadLine, vbTab)
                            If s1.Length > 1 AndAlso s1.Length > ColumnOfNewString AndAlso s1.Length > ColumnOfOriginalString Then
                                Dim x(1) As String
                                x(0) = s1(ColumnOfOriginalString)
                                x(1) = s1(ColumnOfNewString)
                                res.Add(x)
                            End If

                        Loop Until sr.EndOfStream = True
                    End Using

                    Dim res2 = (From t In res Order By t(0) Ascending).ToList

                    Return res2
                End Function



                Shared Function GetListForReplace(Lines As List(Of String), Separator As String, ColumnOfOriginalString As Integer, ColumnOfNewString As Integer) As List(Of String())
                    Dim res As New List(Of String())
                    For Each Line In Lines
                        Dim s1() = Split(Line, Separator)
                        If s1.Length > 1 Then
                            Dim x(1) As String
                            x(0) = s1(ColumnOfOriginalString)
                            x(1) = s1(ColumnOfNewString)
                            res.Add(x)
                        End If


                    Next

                    Dim res2 = (From t In res Order By t(0) Ascending).ToList

                    Return res2
                End Function
                ''' <summary>
                ''' Read Defined number of lines, If NofLine =0 then return all of the lines on error return nothing
                ''' </summary>
                ''' <param name="file"></param>
                ''' <param name="NofLine"></param>
                ''' <returns></returns>
                Public Shared Function ReadLines(file As FileInfo, Optional NofLine As Integer = 0) As List(Of String)
                    Dim Lines As New List(Of String)
                    Try
                        Using sr As New StreamReader(file.FullName)
                            If NofLine <> 0 Then
                                For i1 = 1 To NofLine
                                    Do
                                        Lines.Add(sr.ReadLine)
                                        If Lines.Count = NofLine Then
                                            Exit For
                                        End If
                                    Loop Until sr.EndOfStream = True
                                Next
                            Else
                                Do
                                    Lines.Add(sr.ReadLine)

                                Loop Until sr.EndOfStream = True
                            End If

                        End Using
                    Catch ex As Exception
                        MsgBox(ex.ToString)
                        Return Nothing
                    End Try
                    Return Lines
                End Function

                Friend Shared Function LastLine(File As FileInfo) As String
                    Dim Line As String = ""
                    For Each Item In Szunyi.IO.Import.Text.Parse(File)
                        Line = Item
                    Next
                    Return Line
                End Function
                Public Shared Function LastLine_Contain(File As FileInfo, s As String) As String
                    Dim Line As String = ""
                    For Each Item In Szunyi.IO.Import.Text.Parse(File)
                        If Item.Contains(s) Then Line = Item
                    Next
                    Return Line
                End Function
#Region "Parse Iterator"
                Public Shared Iterator Function ParseMoreLines(file As FileInfo, Nof_Line As Integer) As IEnumerable(Of List(Of String))

                    If file.Exists = True Then
                        Using sr As New StreamReader(file.FullName)

                            Do
                                Dim Out As New List(Of String)
                                For i1 = 1 To Nof_Line
                                    Out.Add(sr.ReadLine)
                                Next
                                If Out.Last = Nothing Then
                                    Dim alf As Int16 = 54
                                    For i1 = Nof_Line - 1 To 0 Step -1
                                        If Out(i1) = Nothing Then
                                            Out.RemoveAt(i1)
                                        End If
                                    Next
                                    Yield Out
                                    Exit Do

                                End If
                                Yield Out
                            Loop Until sr.EndOfStream = True

                        End Using
                    End If

                End Function
                Public Shared Iterator Function Read_Lines_by_Group(file As FileInfo, Nof_Line As Integer) As IEnumerable(Of List(Of String))
                    If file.Exists = True Then
                        Using sr As New StreamReader(file.FullName)

                            Do
                                Dim Out As New List(Of String)
                                For i1 = 1 To Nof_Line
                                    Out.Add(sr.ReadLine)
                                Next
                                If Out.Last = Nothing Then
                                    Dim alf As Int16 = 54
                                    Exit Do

                                End If
                                Yield Out
                            Loop Until sr.EndOfStream = True

                        End Using
                    End If

                End Function
                Public Shared Iterator Function Read_Lines_by_Group(file As FileInfo, Item_Separator As String) As IEnumerable(Of List(Of String))
                    If file.Exists = True Then
                        Using sr As New StreamReader(file.FullName)
                            Dim s As String
                            Dim Out As New List(Of String)
                            Do

                                s = sr.ReadLine
                                If s.StartsWith(Item_Separator) = True Then
                                    If Out.Count = 0 Then
                                        Out.Add(s)

                                    Else
                                        Yield Out
                                        Out.Clear()
                                        Out.Add(s)
                                    End If
                                Else
                                    Out.Add(s)
                                End If

                            Loop Until sr.EndOfStream = True
                            Yield Out
                        End Using
                    End If

                End Function
                ''' <summary>
                ''' Into Line All Lines
                ''' </summary>
                ''' <param name="File"></param>
                ''' <returns></returns>
                Public Shared Iterator Function Parse(File As FileInfo, Optional Not_Start_With As String = "") As IEnumerable(Of String)
                    If File.Exists = True Then
                        Using sr As New StreamReader(File.FullName)
                            If Not_Start_With <> "" Then
                                Do
                                    Dim Line = sr.ReadLine
                                    If IsNothing(Line) = False Then
                                        If Line.StartsWith(Not_Start_With) <> True Then
                                            Yield Line
                                            Exit Do
                                        End If
                                    End If
                                Loop Until sr.EndOfStream = True
                            End If
                            Do
                                Dim Line = sr.ReadLine
                                If IsNothing(Line) = False Then Yield Line
                            Loop Until sr.EndOfStream = True
                        End Using
                    End If
                End Function
                ''' <summary>
                ''' Into Line All Lines
                ''' </summary>
                ''' <param name="File"></param>
                '''  <param name="Not_Start_With"></param>
                ''' <returns></returns>
                Public Shared Iterator Function Parse_Group_Lines(File As FileInfo, Not_Start_With As String) As IEnumerable(Of List(Of String))
                    If File.Exists = True Then
                        Dim cLines As New List(Of String)
                        Using sr As New StreamReader(File.FullName)
                            Do
                                Dim Line = sr.ReadLine
                                If Line.StartsWith(Not_Start_With) = True Then
                                    If cLines.Count > 0 Then
                                        Yield Szunyi.Text.Lists.CloneStrings(cLines)
                                        cLines.Clear()

                                    End If

                                Else
                                    cLines.Add(Line)
                                End If
                            Loop Until sr.EndOfStream = True
                            Yield cLines


                        End Using
                    End If
                End Function
                ''' <summary>
                ''' Into Line All Lines
                ''' </summary>
                ''' <param name="File"></param>
                ''' <returns></returns>
                Public Shared Iterator Function Parse(File As FileInfo, FirstLineIndex As Integer) As IEnumerable(Of String)
                    If File.Exists = True Then
                        Using sr As New StreamReader(File.FullName)
                            For i1 = 1 To FirstLineIndex
                                sr.ReadLine()
                            Next
                            Do
                                Yield sr.ReadLine
                            Loop Until sr.EndOfStream = True

                        End Using
                    End If
                End Function
                ''' <summary>
                ''' Into Array All Lines
                ''' </summary>
                ''' <param name="file"></param>
                ''' <param name="Separator"></param>
                ''' <returns></returns>
                Public Shared Iterator Function ParseToArray(file As FileInfo, Separator As String) As IEnumerable(Of String())
                    If file.Exists = True Then
                        Using sr As New StreamReader(file.FullName, FileAccess.Read)
                            Do
                                Yield Split(sr.ReadLine, Separator)
                            Loop Until sr.EndOfStream = True
                        End Using
                    End If
                End Function

                ''' <summary>
                ''' Into Lines Skip First Lines
                ''' </summary>
                ''' <param name="File"></param>
                ''' <param name="FirstLineIndex"></param>
                ''' <returns></returns>
                Public Shared Iterator Function ParseNotFirst(File As FileInfo, FirstLineIndex As Integer) As IEnumerable(Of String)
                    Using sr As New StreamReader(File.FullName)
                        For i1 = 1 To FirstLineIndex
                            sr.ReadLine()
                        Next
                        Do
                            Yield sr.ReadLine
                        Loop Until sr.EndOfStream = True

                    End Using
                End Function

                ''' <summary>
                ''' Into Array Skip Firts Lines
                ''' </summary>
                ''' <param name="file"></param>
                ''' <param name="Separator"></param>
                ''' <param name="FirstLineIndex"></param>
                ''' <returns></returns>
                Public Shared Iterator Function ParseToArray(file As FileInfo, Separator As String, FirstLineIndex As Integer) As IEnumerable(Of String())
                    If IsNothing(file) = False Then

                        If file.Exists = True Then
                            Using sr As New StreamReader(file.FullName)
                                For i1 = 1 To FirstLineIndex
                                    sr.ReadLine()
                                Next
                                Do

                                    Yield Split(sr.ReadLine, Separator)
                                Loop Until sr.EndOfStream = True
                            End Using
                        End If
                    End If

                End Function

                Public Shared Iterator Function ParseByDelimiter(file As FileInfo, LineSeparator As String, Optional EndOfInteresting As String = Nothing) As IEnumerable(Of List(Of String))
                    If IsNothing(file) = False Then

                        If file.Exists = True Then
                            If IsNothing(EndOfInteresting) = True Then
                                Using sr As New StreamReader(file.FullName)
                                    Dim out As New List(Of String)
                                    Do
                                        Dim Line = sr.ReadLine
                                        If Line.StartsWith(LineSeparator) = True Then
                                            If out.Count > 0 Then
                                                Yield out
                                                out.Clear()
                                            End If
                                        Else

                                            out.Add(Line)
                                        End If
                                    Loop Until sr.EndOfStream = True
                                    If out.Count > 0 Then Yield out
                                End Using
                            Else
                                Using sr As New StreamReader(file.FullName)
                                    Dim out As New List(Of String)
                                    Do
                                        Dim Line = sr.ReadLine
                                        If Line.StartsWith(LineSeparator) = True Then
                                            If out.Count > 0 Then
                                                Yield out
                                                out.Clear()
                                            End If
                                        ElseIf Line.StartsWith(EndOfInteresting) Then
                                            Yield out
                                            Exit Function
                                        Else

                                            out.Add(Line)
                                        End If
                                    Loop Until sr.EndOfStream = True
                                    If out.Count > 0 Then Yield out
                                End Using
                            End If

                        End If
                    End If
                End Function

                Public Shared Function ParseFIrstLinesStartWiths(file As FileInfo, LineStart As String) As List(Of String)
                    Dim out As New List(Of String)
                    If IsNothing(file) = False Then

                        If file.Exists = True Then
                            Try
                                Using sr As New StreamReader(file.FullName)

                                    Do
                                        Dim Line = sr.ReadLine
                                        If IsNothing(Line) = False Then
                                            If Line.StartsWith(LineStart) = True Then
                                                out.Add(Line)
                                            Else
                                                Return out
                                            End If
                                        End If

                                    Loop Until sr.EndOfStream = True

                                End Using

                            Catch ex As Exception
                                Return New List(Of String)
                            End Try

                        End If
                    End If
                    Return out
                End Function
                Public Shared Iterator Function Ignore_Before_Contains_Group_By_Start(fIle As FileInfo, First_Interesting_Line As String, separator As String) As IEnumerable(Of List(Of String))

                    If IsNothing(fIle) = False Then

                        If fIle.Exists = True Then
                            Try
                                Using sr As New StreamReader(fIle.FullName)
                                    ' Find First Line
                                    Dim Line As String
                                    Do
                                        Line = sr.ReadLine
                                        If IsNothing(Line) = False Then
                                            If Line.Contains(First_Interesting_Line) = True Then
                                                Exit Do

                                            End If
                                        Else
                                            Yield New List(Of String)
                                        End If
                                    Loop
                                    Dim out As New List(Of String)
                                    out.Add(Line) ' The First Line
                                    Do
                                        Line = sr.ReadLine
                                        If Line.StartsWith(separator) = True Then
                                            out.Add(Line)
                                        ElseIf Line = "" Then
                                            Dim u As Int16 = 65
                                        Else

                                            Yield out
                                            out.Clear()
                                            out.Add(Line)
                                        End If

                                    Loop Until sr.EndOfStream = True

                                End Using

                            Catch ex As Exception

                            End Try

                        End If
                    End If

                End Function
                Public Shared Iterator Function Ignore_Before_Contains_Group_By_Not_Start(fIle As FileInfo, First_Interesting_Line As String, separator As String) As IEnumerable(Of List(Of String))

                    If IsNothing(fIle) = False Then

                        If fIle.Exists = True Then
                            Try
                                Using sr As New StreamReader(fIle.FullName)
                                    ' Find First Line
                                    Dim Line As String
                                    Do
                                        Line = sr.ReadLine
                                        If IsNothing(Line) = False Then
                                            If Line.Contains(First_Interesting_Line) = True Then
                                                Exit Do

                                            End If
                                        Else
                                            Yield New List(Of String)
                                        End If
                                    Loop
                                    Dim out As New List(Of String)
                                    out.Add(Line) ' The First Line
                                    Do
                                        Line = sr.ReadLine
                                        If Line.StartsWith(separator) = True Then
                                            out.Add(Line)
                                        ElseIf Line = "" Then
                                            Dim u As Int16 = 65
                                        Else

                                            Yield out
                                            out.Clear()
                                            out.Add(Line)
                                        End If

                                    Loop Until sr.EndOfStream = True

                                End Using

                            Catch ex As Exception

                            End Try

                        End If
                    End If

                End Function
                Public Iterator Function cIgnore_Before_Contains_Group_By_Not_Start(fIle As FileInfo, First_Interesting_Line As String, separator As String) As IEnumerable(Of List(Of String))

                    If IsNothing(fIle) = False Then

                        If fIle.Exists = True Then
                            Try
                                Using sr As New StreamReader(fIle.FullName)
                                    ' Find First Line
                                    Dim Line As String
                                    Do
                                        Line = sr.ReadLine
                                        If IsNothing(Line) = False Then
                                            If Line.Contains(First_Interesting_Line) = True Then
                                                Exit Do

                                            End If
                                        Else
                                            Yield New List(Of String)
                                        End If
                                    Loop
                                    Dim out As New List(Of String)
                                    out.Add(Line) ' The First Line
                                    Do
                                        Line = sr.ReadLine
                                        If Line.StartsWith(separator) = True Then
                                            out.Add(Line)
                                        ElseIf Line = "" Then

                                        Else

                                            Yield out
                                            out.Clear()
                                            out.Add(Line)
                                        End If

                                    Loop Until sr.EndOfStream = True

                                End Using

                            Catch ex As Exception

                            End Try

                        End If
                    End If

                End Function

#End Region


            End Class
            Public Class Sequence
                Public seqs As New List(Of Bio.ISequence)
                Public streams As New List(Of Stream)
                Public SeqFiles As New List(Of FileInfo)
                Public Sub New(Reader As StreamReader)
                    Me.streams.Add(Reader.BaseStream)
                End Sub
                Public Sub New(File As FileInfo)
                    Dim fa = Bio.IO.SequenceParsers.FindParserByFileName(File.FullName)
                    If IsNothing(fa) = False Then SeqFiles.Add(File)

                End Sub
                Public Sub New(Files As List(Of FileInfo))
                    For Each File In Files
                        Dim fa = Bio.IO.SequenceParsers.FindParserByFileName(File.FullName)
                        If IsNothing(fa) = False Then SeqFiles.Add(File)
                    Next
                End Sub
                Public Shared Iterator Function GetOnyByONe(File As FileInfo) As IEnumerable(Of ISequence)
                    Dim fa = Bio.IO.SequenceParsers.FindParserByFileName(File.FullName)
                    If IsNothing(fa) = False Then
                        Dim x As New FileStream(File.FullName, FileMode.Open)
                        For Each Seq In fa.Parse(x)
                            Yield Seq
                        Next
                    End If
                End Function
                Public Shared Iterator Function GetOnyByONe(Files As List(Of FileInfo)) As IEnumerable(Of ISequence)
                    If IsNothing(Files) = False Then
                        For Each File In Files


                            Dim fa = Bio.IO.SequenceParsers.FindParserByFileName(File.FullName)
                            If IsNothing(fa) = False Then
                                Using x As New FileStream(File.FullName, FileMode.Open)
                                    For Each Seq In fa.Parse(x)
                                        Yield Seq
                                    Next
                                End Using
                                fa.Close
                            End If
                        Next
                    End If
                End Function
                Public Sub DoIt()
                    For Each File In SeqFiles
                        Using x As New FileStream(File.FullName, FileMode.Open)

                            Dim fa = Bio.IO.SequenceParsers.FindParserByFileName(File.FullName)
                            For Each Seq In fa.Parse(x)
                                Me.seqs.Add(Seq)
                            Next
                        End Using


                    Next
                End Sub

                ''' <summary>
                ''' Return Sorted Listed of Bio.Sequence or Nothing
                ''' </summary>
                ''' <param name="File"></param>
                ''' <returns></returns>
                Public Shared Function FromUserSelection(Optional File As FileInfo = Nothing, Optional Filter As String = "") As List(Of Bio.ISequence)
                    Dim Out As New List(Of Bio.ISequence)
                    If IsNothing(File) = True Then
                        File = Szunyi.IO.Files.Filter.SelectFile(Filter, "Select Files")
                        If IsNothing(File) = True Then Return Nothing
                    End If
                    Dim x As New FileStream(File.FullName, FileMode.Open)
                    Dim fa = Bio.IO.SequenceParsers.FindParserByFileName(File.FullName)
                    For Each Seq In fa.Parse(x)
                        Out.Add(Seq)
                    Next
                    x.Close()
                    If Out.Count > 0 Then
                        Out.Sort(Comparares.AllComparares.BySeqID)
                        Return Out
                    Else
                        Return Nothing
                    End If

                End Function
                Public Shared Iterator Function From_Files_Iterator(Files As List(Of FileInfo)) As IEnumerable(Of Bio.ISequence)
                    For Each File In Files
                        Dim x As New FileStream(File.FullName, FileMode.Open)
                        Dim fa = Bio.IO.SequenceParsers.FindParserByFileName(File.FullName)
                        For Each Seq In fa.Parse(x)
                            Yield Seq
                        Next
                    Next
                End Function
                Public Shared Iterator Function From_File_Iterator(File As FileInfo) As IEnumerable(Of Bio.ISequence)
                    If IsNothing(File) = True Then
                        Yield Nothing
                    Else
                        Dim x As New FileStream(File.FullName, FileMode.Open)
                        Dim fa = Bio.IO.SequenceParsers.FindParserByFileName(File.FullName)
                        For Each Seq In fa.Parse(x)
                            Yield Seq
                        Next
                        fa.Close
                        x.Close()
                    End If

                End Function

                ''' <summary>
                ''' Return a List Of ISequences Sorted By SeqID or empty List Common parsers
                ''' </summary>
                ''' <param name="File"></param>
                ''' <returns></returns>
                Public Shared Function FromFile(File As FileInfo) As List(Of Bio.ISequence)
                    Dim Out As New List(Of Bio.ISequence)
                    If IsNothing(File) = True Then Return New List(Of Bio.ISequence)
                    If File.Exists = True Then
                        Try
                            Dim x As New FileStream(File.FullName, FileMode.Open)
                            Dim fa = Bio.IO.SequenceParsers.FindParserByFileName(File.FullName)
                            For Each Seq In fa.Parse(x)
                                Out.Add(Seq)
                            Next
                            x.Close()
                            Out.Sort(AllComparares.BySeqID)
                        Catch ex As Exception

                        Finally


                        End Try

                    End If

                    Return Out
                End Function

                Public Shared Function IDs(file As FileInfo) As List(Of String)
                    Dim out As New List(Of String)
                    For Each seq In From_File_Iterator(file)
                        out.Add(seq.ID)
                    Next
                    Return out
                End Function
            End Class
            Public Class BLAST
                Shared Function FromFileInfos(Files As List(Of FileInfo), log As StringBuilder) As List(Of BlastResult)
                    Dim x As New List(Of BlastResult)


                    For Each File In Files

                        If File.Exists = True Then
                            x.Add(Szunyi.IO.Import.BLAST.FromFileInfo(File, log))
                        End If
                    Next
                    Return x
                End Function
                Shared Function FromFileInfo(File As FileInfo, log As StringBuilder) As BlastResult

                    If File.Exists = True Then
                        Try
                            Using reader As TextReader = New StreamReader(File.FullName)
                                Dim BlastParser = New Extended.BlastXmlParser
                                Dim x = BlastParser.ParseXMLStream(reader, True, Nothing)
                                For Each item In x.Records
                                    item.Metadata = x.Metadata
                                Next
                                Return x
                            End Using
                        Catch ex As Exception
                            log.Append("Not avalible:" & File.Name)
                            Return Nothing
                        End Try

                    End If
                    Return Nothing
                End Function

                ''' <summary>
                ''' Return Empty List Or List Of BlastResults
                ''' </summary>
                ''' <param name="FileNames"></param>
                ''' <param name="log"></param>
                ''' <returns></returns>
                Public Shared Function FromFileNames(FileNames As List(Of String), log As StringBuilder) As List(Of BlastResult)
                    Dim x As New List(Of BlastResult)


                    For Each File In FileNames
                        Dim TheFile As New FileInfo(File)
                        If TheFile.Exists = True Then
                            x.Add(Szunyi.IO.Import.BLAST.FromFileInfo(TheFile, log))
                        End If
                    Next
                    Return x
                End Function
                ''' <summary>
                ''' Return BlastResult Or Nothing 
                ''' </summary>
                ''' <param name="FileName"></param>
                ''' <param name="log"></param>
                ''' <returns></returns>
                Public Shared Function FromFileName(FileName As String, log As StringBuilder) As BlastResult

                    Dim TheFile As New FileInfo(FileName)
                    If TheFile.Exists = True Then
                        Return (Szunyi.IO.Import.BLAST.FromFileInfo(TheFile, log))
                    Else
                        log.Append("File Not Found:").Append(FileName)
                    End If
                    Return Nothing
                End Function
            End Class
            Public Class GTH

                Public Property Files As New List(Of FileInfo)
                Public Property Seqs As List(Of Bio.ISequence)

                Public Sub New(files As List(Of FileInfo), Seqs As List(Of Bio.ISequence))
                    Me.Seqs = Seqs
                    Me.Files = files
                End Sub
                Public Sub DoIt()
                    Dim All_RefSeqs_Locations As New List(Of Bio.IO.GenBank.ILocation)
                    Dim All_EST_Exons As New List(Of Bio.IO.GenBank.ILocation)
                    Dim res As New Dictionary(Of String, List(Of ILocation))
                    res.Add("All", New List(Of ILocation))
                    res.Add("polyAT", New List(Of ILocation))
                    For Each File In Files
                        For Each Item In Szunyi.IO.Import.Text.ParseByDelimiter(File, "*", "--------------------------------------------------------------------------------")
                            If Item.First.StartsWith("$") = False Then
                                Dim woEmpty = Szunyi.Text.Lists.RemoveEmptyLines(Item)
                                Dim EST = Szunyi.Text.Lists.GetLineStartWith(woEmpty, "EST Sequence")
                                Dim gDNS_ID = Split(Szunyi.Text.Lists.GetLineStartWith(woEmpty, "Genomic Template"), "=").Last
                                Dim EST_ID = Get_EST_ID(EST)
                                Dim SeqTemplate = Szunyi.Text.Lists.GetLinesBetween(woEmpty, "EST Sequence", "Genomic Template")
                                Dim ALs = Szunyi.Text.Lists.GetLinesBetween(woEmpty, "Alignment (genomic DNA sequence = upper lines)", "*************")
                                Dim EstSeq = Szunyi.Sequences.SequenceManipulation.GetSequences.FromLinesOneSeq(SeqTemplate, Alphabets.AmbiguousDNA)
                                Dim Predictions = Szunyi.Text.Lists.GetLinesBetween(woEmpty, "Predicted", "MATCH")
                                Dim ExonLines = Szunyi.Text.Lists.GetLinesContains(Predictions, "Exon")
                                '   Dim ReadType As Szunyi.Constants.ReadType_ByPolyA = Szunyi.Sequences.SequenceManipulation.Properties.Get_Read_Type(EstSeq)
                                Dim gSeq = Szunyi.Sequences.SequenceManipulation.GetSequences.ByID(Seqs, gDNS_ID)
                                Dim gExons = Get_Exons(ExonLines)
                                '     If ReadType = ReadType.T Then
                                '      gExons = Szunyi.Features.FeatureManipulation.GetLocations.Change_Strand(gExons)

                            End If
                            ''        If ReadType = ReadType.A Or ReadType = ReadType.T Then
                            '    res("polyAT").Add(gExons)
                            '     End If
                            '       Dim mRNA = Get_Exons_For_mRNA(ExonLines)

                            '      If res.ContainsKey(ReadType.ToString) = False Then res.Add(ReadType.ToString, New List(Of ILocation))
                            '    res(ReadType.ToString).Add(gExons)
                            '    res("All").Add(gExons)
                            '     All_RefSeqs_Locations.Add(gExons)
                            '     All_EST_Exons.Add(mRNA)
                            Dim jh As Int16 = 54
                            '   End If
                        Next
                    Next
                    Analyse(res)

                End Sub



                Private Function GetCIGAR(estSeq As Bio.ISequence, mRNA As Bio.IO.GenBank.Location, gSeq As Bio.ISequence, gExons As Bio.IO.GenBank.Location) As Object
                    Dim mRNA_Exons_Seqs = Szunyi.Sequences.SequenceManipulation.GetSequences.From_Loci(estSeq, mRNA)
                    Dim gDNA_Exons_seqs = Szunyi.Sequences.SequenceManipulation.GetSequences.From_Loci(gSeq, gExons)
                    Dim sa = Szunyi.Sequences.SequenceManipulation.SeqsToString.GetSeqsAsString(mRNA_Exons_Seqs)
                    Dim sg = Szunyi.Sequences.SequenceManipulation.SeqsToString.GetSeqsAsString(gDNA_Exons_seqs)
                    '  Dim AL_Index = Szunyi.Text.Lists.Get_Index_Contains(woEmpty, "Alignment")
                    ' Dim Als = Szunyi.Text.Lists.GetSubList(woEmpty, AL_Index + 1)
                    Dim Out As New List(Of Bio.Algorithms.Alignment.AlignedSequence)
                    For i1 = 0 To sa.Count - 1
                        Dim t As New Bio.Algorithms.Alignment.SmithWatermanAligner()
                        Dim al = t.Align(mRNA_Exons_Seqs(i1), gDNA_Exons_seqs(i1))
                        Out.Add(al.First.AlignedSequences.First)
                        Dim c = Szunyi.Sequences.SequenceManipulation.SeqsToString.GetSeqAsString(al.First.PairwiseAlignedSequences.First.Consensus)
                        Dim f = Szunyi.Sequences.SequenceManipulation.SeqsToString.GetSeqAsString(al.First.PairwiseAlignedSequences.First.FirstSequence)
                        Dim s = Szunyi.Sequences.SequenceManipulation.SeqsToString.GetSeqAsString(al.First.PairwiseAlignedSequences.First.SecondSequence)

                        Windows.Forms.Clipboard.SetText(vbAbort & vbCrLf & f & vbCrLf & s)
                        Dim alf As Int16 = 54

                    Next
                End Function

                Private Sub Analyse(res As Dictionary(Of String, List(Of Bio.IO.GenBank.ILocation)))
                    Dim str2 As New System.Text.StringBuilder
                    For Each Item In res
                        If Item.Key = "polyAT" Then
                            Dim locis = Item.Value

                            Dim First_Exons = Szunyi.Features.FeatureManipulation.GetLocations.Get_First_Exons_Location(locis)
                            Dim All_Exons = Szunyi.Features.FeatureManipulation.GetLocations.Get_All_Exons_Location(locis)
                            Dim All_Introns = Szunyi.Features.FeatureManipulation.GetLocations.Get_All_Introns_Location(locis)
                            Dim Last_Exons = Szunyi.Features.FeatureManipulation.GetLocations.Get_Last_Exons_Location(locis)
                            Dim Merged_Introns = Szunyi.Location.Merging.MergeLocations(All_Introns, 10, ClassLibrary1.Szunyi.Constants.Sort_Locations_By.TSS_PAS, 2)
                            Dim Merged_First_Exons = Szunyi.Location.Merging.MergeLocations(First_Exons, 10, Sort_Locations_By.TSS_PAS, 2)
                            Dim Merged_All_Exons = Szunyi.Location.Merging.MergeLocations(All_Exons, 10, Sort_Locations_By.TSS_PAS, 2)
                            Dim Merged_Transcripts = Szunyi.Location.Merging.MergeLocations(locis, 10, Sort_Locations_By.TSS_PAS, 1)
                            Dim Merged_Last_Exons = Szunyi.Location.Merging.MergeLocations(Last_Exons, 10, 2)

                            Dim Intron_Report = Szunyi.Location.Report.GetReport(Merged_Introns)
                            Dim First_Exons_Report = Szunyi.Location.Report.GetReport(Merged_First_Exons)
                            Dim All_Exons_Report = Szunyi.Location.Report.GetReport(Merged_All_Exons)
                            Dim Last_Exons_Report = Szunyi.Location.Report.GetReport(Merged_Last_Exons)
                            Dim Transcript_Report = Szunyi.Location.Report.GetReport(Merged_Transcripts)

                            Dim TheSeq = Szunyi.Sequences.SequenceManipulation.Common.CloneSeq(Me.Seqs.First)
                            Dim md = Szunyi.Features.FeatureManipulation.GenBankMetaDataManipulation.GetGenBankMetaDataFromSeq(TheSeq)

                            Dim TSS = Get_TSS(First_Exons, 20, ClassLibrary1.Szunyi.Constants.Get_Position_From_LocationsBy.abundance, 2)
                            md.Features.All.AddRange(TSS)
                            Dim PolyA = Get_PolyA(Last_Exons, 10, Get_Position_From_LocationsBy.abundance, 2)
                            md.Features.All.AddRange(PolyA)
                            Dim Introns = Get_Introns(Merged_Introns, Get_Position_From_LocationsBy.abundance)
                            md.Features.All.AddRange(Introns)
                            Dim transcripts = Get_Transcripts(Merged_Transcripts, Get_Position_From_LocationsBy.abundance)
                            md.Features.All.AddRange(transcripts)
                            Szunyi.IO.Export.SaveSequencesToSingleGenBank(TheSeq)
                            Dim str As New System.Text.StringBuilder

                            '  Windows.Forms.Clipboard.SetText(str.ToString)
                        End If

                    Next

                    str2.AppendLine.AppendLine()
                    Windows.Forms.Clipboard.SetText(str2.ToString)
                End Sub

                Private Function Get_Transcripts(merged_Transcripts As IOrderedEnumerable(Of KeyValuePair(Of String, List(Of List(Of ILocation)))), abundance As Get_Position_From_LocationsBy) As Object
                    Dim out As New List(Of FeatureItem)
                    For Each s In merged_Transcripts
                        Dim alf As Int16 = 54
                        Dim feat As New FeatureItem(StandardFeatureKeys.MessengerRna, s.Key)
                        Dim ls As New List(Of String)
                        For Each sl In s.Value
                            ls.Add(sl.First.LocationStart & ".." & sl.First.LocationEnd & " e:" & sl.Count)
                        Next
                        feat.Qualifiers(StandardQualifierNames.Note) = ls
                        feat.Label = ls.First & " " & Get_Label("", s)
                        out.Add(feat)
                    Next
                    Return out
                End Function

                Private Function Get_Introns(merged_Introns As System.Linq.IOrderedEnumerable(Of System.Collections.Generic.KeyValuePair(Of String, System.Collections.Generic.List(Of System.Collections.Generic.List(Of Bio.IO.GenBank.ILocation)))), abundance As Get_Position_From_LocationsBy) As List(Of FeatureItem)
                    Dim out As New List(Of FeatureItem)
                    For Each s In merged_Introns
                        Dim alf As Int16 = 54
                        Dim feat As New FeatureItem(StandardFeatureKeys.Intron, s.Key)
                        Dim ls As New List(Of String)
                        For Each sl In s.Value
                            ls.Add(sl.First.LocationStart & ".." & sl.First.LocationEnd & " e:" & sl.Count)
                        Next
                        feat.Qualifiers(StandardQualifierNames.Note) = ls
                        feat.Label = Get_Label(ls.First.Split(" e:").First, s)

                        out.Add(feat)
                    Next
                    Return out
                End Function

                Private Function Get_PolyA(locis As List(Of ILocation), width As Integer, score As Get_Position_From_LocationsBy, MinNofItem As Integer) As List(Of FeatureItem)
                    Dim out As New List(Of FeatureItem)
                    Dim Last_Exons = Szunyi.Features.FeatureManipulation.GetLocations.Get_Last_Exons_Location(locis)
                    Dim fwLocis = Szunyi.Features.FeatureManipulation.GetLocations.Get_Only_Positive_Strands(Last_Exons)
                    Dim revLocis = Szunyi.Features.FeatureManipulation.GetLocations.Get_Only_Negative_Strands(Last_Exons)

                    For Each f In Szunyi.Location.Merging.MergeLocations(fwLocis, width, Sort_Locations_By.LE, MinNofItem)
                        Dim pos = Szunyi.Location.Get_Sites_Or_Positions.Get_Site(f, Sort_Locations_By.LE, score)
                        Dim t As New FeatureItem(StandardFeatureKeys.PolyASignal, pos & ".." & pos + 50)
                        t.Label = Get_Label(pos, f)
                        out.Add(t)
                    Next

                    For Each f In Szunyi.Location.Merging.MergeLocations(revLocis, width, Sort_Locations_By.TSS, MinNofItem)
                        Dim pos = Szunyi.Location.Get_Sites_Or_Positions.Get_Site(f, Sort_Locations_By.TSS, score)
                        Dim t As New FeatureItem(StandardFeatureKeys.PolyASignal, "complement(" & pos - 50 & ".." & pos & ")")
                        t.Label = Get_Label(pos, f)
                        out.Add(t)
                    Next

                    Return out
                End Function
                Private Function Get_Label(Pos As String, f As KeyValuePair(Of String, List(Of List(Of ILocation)))) As String
                    Dim c As Integer = 0
                    For Each j In f.Value
                        c += j.Count
                    Next
                    Return Pos & "_e:" & c
                End Function
                Private Function Get_Label(Pos As Integer, locis As List(Of ILocation)) As String

                    Return Pos & "_e:" & locis.Count
                End Function

                Private Function Get_TSS(locis As List(Of ILocation), width As Integer, score As Szunyi.Constants.Sort_Locations_By, MinNofItem As Integer) As List(Of FeatureItem)
                    Dim out As New List(Of FeatureItem)
                    Dim First_Exons = Szunyi.Features.FeatureManipulation.GetLocations.Get_First_Exons_Location(locis)
                    Dim fwLocis = Szunyi.Features.FeatureManipulation.GetLocations.Get_Only_Positive_Strands(First_Exons)
                    Dim revLocis = Szunyi.Features.FeatureManipulation.GetLocations.Get_Only_Negative_Strands(First_Exons)

                    For Each f In Szunyi.Location.Merging.MergeLocations(fwLocis, width, Sort_Locations_By.TSS, MinNofItem)
                        Dim pos = Szunyi.Location.Get_Sites_Or_Positions.Get_Site(f, Sort_Locations_By.TSS, score)
                        Dim t As New FeatureItem(StandardFeatureKeys.Promoter, pos & ".." & pos + 50)
                        t.Label = Get_Label(pos, f)
                        out.Add(t)
                    Next

                    For Each f In Szunyi.Location.Merging.MergeLocations(revLocis, width, Sort_Locations_By.LE, MinNofItem)
                        Dim pos = Szunyi.Location.Get_Sites_Or_Positions.Get_Site(f, Sort_Locations_By.LE, score)
                        Dim t As New FeatureItem(StandardFeatureKeys.Promoter, "complement(" & pos - 50 & ".." & pos & ")")
                        t.Label = Get_Label(pos, f)
                        out.Add(t)
                    Next

                    Return out

                End Function

                Private Function Get_AL_Seqs(Exons As List(Of String)) As List(Of Bio.Sequence)
                    Dim out As New List(Of Bio.Sequence)
                    Dim EST As New System.Text.StringBuilder
                    For i1 = 0 To Exons.Count - 2 Step 3
                        Dim ch = Exons(i1)
                        Dim g = Szunyi.Sequences.SequenceManipulation.GetSequences.FromLineOneSeq(Exons(i1), Alphabets.AmbiguousDNA)
                        Dim Est2 = Szunyi.Sequences.SequenceManipulation.GetSequences.FromLineOneSeq(Exons(i1 + 2), Alphabets.AmbiguousDNA)
                        Dim alf As Int16 = 32

                    Next
                End Function
                Private Function Get_Exons_For_mRNA(ExonLines As List(Of String)) As Bio.IO.GenBank.Location
                    Dim out As New List(Of Bio.IO.GenBank.Location)
                    Dim str As New System.Text.StringBuilder
                    Dim Loci As String
                    Dim isComplementer As Boolean = False
                    For Each Line In ExonLines
                        Dim s2 = Split(Line, ";")
                        Dim s = Split(s2(1), " ").ToList
                        Dim s1 = Szunyi.Text.Lists.RemoveEmptyLines(s)
                        Dim start As Integer = s1(1)
                        Dim endy As Integer = s1(2)
                        If endy < start Then
                            isComplementer = True
                        End If
                        str.Append(s1(1)).Append("..").Append(s1(2)).Append(",")
                    Next
                    If ExonLines.Count > 0 Then str.Length -= 1
                    If ExonLines.Count > 1 Then
                        Loci = "join(" & str.ToString & ")"
                    Else
                        Loci = str.ToString
                    End If
                    If isComplementer = True Then
                        Loci = "complement(" & Loci & ") "
                    End If
                    Dim x1 = Szunyi.Features.FeatureManipulation.GetLocations.LociBuilder.GetLocation(Loci)
                    Return x1
                End Function
                Private Function Get_Exons(ExonLines As List(Of String)) As Bio.IO.GenBank.Location
                    Dim out As New List(Of Bio.IO.GenBank.Location)
                    Dim str As New System.Text.StringBuilder
                    Dim Loci As String
                    If IsComplementer(ExonLines.First) = True Then
                        ExonLines.Reverse()
                        For Each Line In ExonLines

                            Dim s = Split(Line, " ").ToList
                            Dim s1 = Szunyi.Text.Lists.RemoveEmptyLines(s)
                            Dim start As Integer = s1(2)
                            Dim endy As Integer = s1(3)

                            str.Append(s1(3)).Append("..").Append(s1(2)).Append(",")
                        Next

                        If ExonLines.Count > 0 Then str.Length -= 1
                        If ExonLines.Count > 1 Then
                            Loci = "join(" & str.ToString & ")"
                        Else
                            Loci = str.ToString
                        End If
                        Loci = "complement(" & Loci & ") "
                    Else
                        For Each Line In ExonLines
                            Dim s = Split(Line, " ").ToList
                            Dim s1 = Szunyi.Text.Lists.RemoveEmptyLines(s)
                            Dim start As Integer = s1(2)
                            Dim endy As Integer = s1(3)

                            str.Append(s1(2)).Append("..").Append(s1(3)).Append(",")

                        Next
                        If ExonLines.Count > 0 Then str.Length -= 1
                        If ExonLines.Count > 1 Then
                            Loci = "join(" & str.ToString & ")"
                        Else
                            Loci = str.ToString
                        End If
                    End If




                    Dim x1 = Szunyi.Features.FeatureManipulation.GetLocations.LociBuilder.GetLocation(Loci)
                    Return x1
                End Function
                Private Function IsComplementer(line As String) As Boolean
                    Dim s = Split(line, " ").ToList
                    Dim s1 = Szunyi.Text.Lists.RemoveEmptyLines(s)
                    Dim start As Integer = s1(2)
                    Dim endy As Integer = s1(3)
                    If start > endy Then
                        Return True
                    Else
                        Return False
                    End If
                End Function
                Private Function Get_EST_ID(EST_Line As String) As String
                    Return Split(EST_Line, "description=").Last
                End Function
            End Class
            Public Class Headers
                Shared Function GetIntrestingColumns(File As FileInfo,
                                                 NofHeaderLine As Integer,
                                                 Optional Title As String = "Select Columns") As List(Of Integer)
                    Dim Headers = Szunyi.IO.Import.Text.GetHeader(File, NofHeaderLine, Nothing, Nothing)

                    Using x As New Select_Columns(Headers)
                        x.Text = Title
                        If x.ShowDialog = System.Windows.Forms.DialogResult.OK Then
                            Return x.SelectedIndexes.ToList
                        Else : Return Nothing
                        End If
                    End Using
                End Function
            End Class
            Public Class Locations
                Public Shared Iterator Function Parse_Into_Basic_Locations(Files As List(Of FileInfo)) As IEnumerable(Of Szunyi.Location.Basic_Location)
                    For Each File In Files

                    Next
                End Function
                Public Shared Iterator Function Parse_Into_Basic_Locations(File As FileInfo) As IEnumerable(Of Szunyi.Location.Basic_Location)
                    For Each Line In Szunyi.IO.Import.Text.Parse(File)
                        Dim l = Szunyi.Location.Common.LociBuilder.GetLocation(Line)
                        Yield Szunyi.Location.Basic_Location_Manipulation.Get_Basic_Location(l)
                    Next
                End Function
                Public Shared Function Strand_Position_From_File(File As FileInfo) As List(Of Szunyi.Location.Basic_Location)
                    Dim Igor_Values As New List(Of Bio.IO.GenBank.ILocation)
                    For Each Line In Szunyi.IO.Import.Text.Parse(File)
                        Dim s = Split(Line, vbTab)
                        Dim i As Integer = s(1)
                        Igor_Values.Add(Szunyi.Location.Common.GetLocation(i - 10, i + 10, s.First))
                    Next
                    Dim out = Szunyi.Location.Basic_Location_Manipulation.Get_Basic_LocationS(Igor_Values)
                    Return out
                End Function
                Public Shared Function Strand_Position_From_Files(Files As List(Of FileInfo)) As List(Of List(Of Szunyi.Location.Basic_Location))
                    Dim out As New List(Of List(Of Szunyi.Location.Basic_Location))
                    For Each File In Files
                        out.Add(Strand_Position_From_File(File))
                    Next
                    Return out
                End Function
            End Class

            Public Class Seq
                Implements IDisposable


                Dim fa As ISequenceParser
                Public Sub New(File As FileInfo)
                    fa = Bio.IO.SequenceParsers.FindParserByFileName(File.FullName)
                    If IsNothing(fa) = True Then Exit Sub

                    fa.Open(File.FullName)
                End Sub
                Public Function MoveNext() As Bio.ISequence
                    Return fa.ParseOne
                End Function

#Region "IDisposable Support"
                Private disposedValue As Boolean ' To detect redundant calls

                ' IDisposable
                Protected Overridable Sub Dispose(disposing As Boolean)
                    If Not disposedValue Then
                        If disposing Then
                            fa.Close
                        End If

                        ' TODO: free unmanaged resources (unmanaged objects) and override Finalize() below.
                        ' TODO: set large fields to null.
                    End If
                    disposedValue = True
                End Sub

                ' TODO: override Finalize() only if Dispose(disposing As Boolean) above has code to free unmanaged resources.
                'Protected Overrides Sub Finalize()
                '    ' Do not change this code.  Put cleanup code in Dispose(disposing As Boolean) above.
                '    Dispose(False)
                '    MyBase.Finalize()
                'End Sub

                ' This code added by Visual Basic to correctly implement the disposable pattern.
                Public Sub Dispose() Implements IDisposable.Dispose
                    ' Do not change this code.  Put cleanup code in Dispose(disposing As Boolean) above.
                    Dispose(True)
                    ' TODO: uncomment the following line if Finalize() is overridden above.
                    ' GC.SuppressFinalize(Me)
                End Sub
#End Region


            End Class
        End Class


    End Namespace

End Namespace

