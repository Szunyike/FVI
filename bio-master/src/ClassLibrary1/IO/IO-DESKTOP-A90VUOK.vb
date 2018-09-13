Imports System.ComponentModel
Imports System.IO
Imports System.Net
Imports System.Text
Imports Bio

Namespace Szunyi
    Namespace IO
        Public Class Files
            ''' <summary>
            ''' Return DirectoryInfo From FolderBrowserDialog
            ''' </summary>
            ''' <param name="Path"></param>
            ''' <returns></returns>
            Public Shared Function getDirectory(Optional Path As String = "") As DirectoryInfo
                Dim fbd1 As New FolderBrowserDialog
                fbd1.Description = "Select Input Folder"
                If Path <> "" Then fbd1.SelectedPath = Path
                If fbd1.ShowDialog = DialogResult.OK Then Return New DirectoryInfo(fbd1.SelectedPath)
                Return Nothing
            End Function
            ''' <summary>
            ''' Return the List Of Fileinfo From List Of FileNames(String)
            ''' </summary>
            ''' <param name="FileNames"></param>
            ''' <returns></returns>
            Friend Shared Function GetFilesFromFileNames(FileNames As List(Of String)) As Object
                Dim Out As New List(Of FileInfo)
                For Each FileName In FileNames
                    Out.Add(New FileInfo(FileName))
                Next
                Return Out
            End Function

            ''' <summary>
            ''' Return a list of FileInfos Or Nothing
            ''' </summary>
            ''' <param name="Title"></param>
            ''' <param name="Filter"></param>
            ''' <returns></returns>
            Friend Shared Function SelectFiles(Optional Title As String = "Select Files", Optional Filter As String = "") As List(Of FileInfo)

                Dim ofd1 As New OpenFileDialog
                ofd1.Multiselect = True
                If Filter <> "" Then ofd1.Filter = Filter
                If ofd1.ShowDialog = DialogResult.OK Then
                    Dim Out As New List(Of FileInfo)
                    For Each FileName In ofd1.FileNames
                        Out.Add(New FileInfo(FileName))
                    Next
                    Return Out
                End If
                Return Nothing
            End Function
            ''' <summary>
            ''' Return Empty List Or List Of Files
            ''' </summary>
            ''' <param name="folder"></param>
            ''' <returns></returns>
            Friend Shared Function GetAllFilesFromFolder(folder As String) As List(Of FileInfo)
                Dim Dir As New DirectoryInfo(folder)
                Dim l = Dir.GetFiles
                If l.Count = 0 Then Return New List(Of FileInfo)
                Return l.ToList
            End Function
            ''' <summary>
            ''' Return Fileinfo, else return nothing
            ''' </summary>
            ''' <param name="Title"></param>
            ''' <param name="Filter"></param>
            ''' <returns></returns>
            Friend Shared Function SelectFile(Optional Title As String = "Select Files", Optional Filter As String = "") As FileInfo

                Dim ofd1 As New OpenFileDialog
                ofd1.Multiselect = False
                ofd1.Title = Title
                If Filter <> "" Then ofd1.Filter = Filter
                If ofd1.ShowDialog = DialogResult.OK Then
                    Return New FileInfo(ofd1.FileName)
                End If
                Return Nothing
            End Function
            Friend Shared Function GetShortFileName(Files As List(Of FileInfo)) As String
                Select Case Files.Count
                    Case 0
                        Return String.Empty
                    Case 1
                        Return Files.First.Name.Replace(Files.First.Extension, "")
                    Case Else
                        Return InputBox("Enter Name of The List!")
                End Select
            End Function

            Friend Shared Function SelectSaveFile(Filter As String, Optional Title As String = "Save As") As FileInfo
                Dim sfd1 As New SaveFileDialog
                sfd1.Title = Title
                sfd1.Filter = Filter
                If sfd1.ShowDialog = DialogResult.OK Then
                    Return New FileInfo(sfd1.FileName)
                Else
                    Return Nothing
                End If
            End Function
            Friend Shared Function GetFolder(Optional Title As String = "Select Folder") As String
                Dim fbd1 As New FolderBrowserDialog
                fbd1.Description = Title
                If fbd1.ShowDialog = DialogResult.OK Then Return fbd1.SelectedPath
                Return ""
            End Function

            Shared Function GetIntrestingColumn(File As FileInfo, NofHeaderLine As Integer, Optional Title As String = "Select Column") As Integer
                Dim Headers = Szunyi.IO.Files.GetHeader(File, NofHeaderLine, Nothing, Nothing)

                Using x As New Select_Columns(Headers)
                    x.Text = Title
                    If x.ShowDialog = System.Windows.Forms.DialogResult.OK Then
                        Return x.SelectedIndexes.First
                    Else : Return -1
                    End If
                End Using
            End Function
            Public Shared Function GetHeader(TheFile As FileInfo, NofHeaderLine As Integer,
                                          ToRemove As List(Of String),
                                          InterestingColumnsIDs As List(Of Integer)) As List(Of String)

                Dim res As New List(Of String)
                Dim resII As New Dictionary(Of Integer, String)
                Dim s1() As String
                If IsNothing(TheFile) = True Then Return Nothing
                Using sr As New StreamReader(TheFile.FullName)
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

                    If IsNothing(ToRemove) = False Then res = Szunyi.Text.ToRemove(res, ToRemove)

                End Using
                Return res
            End Function
            ''' <summary>
            ''' Replece the Extension
            ''' </summary>
            ''' <param name="fileInSearch"></param>
            ''' <param name="v"></param>
            ''' <returns></returns>
            Friend Shared Function GetNewFileName(fileInSearch As FileInfo, v As String) As String
                Dim Out As String = fileInSearch.FullName.Replace(fileInSearch.Extension, "")
                Out = Out & v & fileInSearch.Extension
                Return Out

            End Function
            Friend Shared Function GetNewFile(fileInSearch As FileInfo, v As String) As FileInfo
                Dim Out As String = fileInSearch.FullName.Replace(fileInSearch.Extension, v)
                '	Out = Out & v & fileInSearch.Extension
                Dim x As New FileInfo(Out)
                Return x

            End Function

            Shared Function GetIntrestingColumns(File As FileInfo,
                                                 NofHeaderLine As Integer,
                                                 Optional Title As String = "Select Columns") As List(Of Integer)
                Dim Headers = Szunyi.IO.Files.GetHeader(File, NofHeaderLine, Nothing, Nothing)

                Using x As New Select_Columns(Headers)
                    x.Text = Title
                    If x.ShowDialog = System.Windows.Forms.DialogResult.OK Then
                        Return x.SelectedIndexes.ToList
                    Else : Return Nothing
                    End If
                End Using
            End Function

            Public Shared Function GetValuesToDictionary(File As FileInfo,
                                                      IDColIndex As Integer,
                                                      ValuesColIndexes As List(Of Integer),
                                                      NofHeaderLines As Integer,
                                                       Optional Separator As String = vbTab) As Dictionary(Of String, List(Of String))
                Dim out As New Dictionary(Of String, List(Of String))
                Dim NofLine As Integer = 0
                Dim Line As String
                Dim s1()
                Dim log As New StringBuilder
                Try

                    Using sr As New StreamReader(File.FullName)
                        For i1 = 1 To NofHeaderLines
                            sr.ReadLine()
                            NofLine += 1
                        Next
                        Do
                            Line = sr.ReadLine
                            NofLine += 1
                            s1 = Split(Line, Separator)
                            Dim s = Szunyi.Text.GetText(s1, ValuesColIndexes, vbTab)
                            Dim Key = s1(IDColIndex)
                            If out.ContainsKey(Key) = False Then
                                out.Add(Key, New List(Of String))
                                out(Key).Add(s)
                            Else
                                out(Key).Add(s)
                                log.Append(Key).AppendLine()
                            End If


                        Loop Until sr.EndOfStream = True
                    End Using
                Catch ex As Exception
                    MsgBox(ex.ToString)
                End Try
                Return out
            End Function

        End Class
        Public Class Import
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

                Shared Function GetListForReplace(File As FileInfo, ColumnOfOriginalString As Integer, ColumnOfNewString As Integer) As List(Of String())
                    Dim res As New List(Of String())
                    Using sr As New StreamReader(File.FullName)
                        sr.ReadLine()
                        Do
                            Dim s1() = Split(sr.ReadLine, vbTab)
                            If s1.Length > 1 Then
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

                Friend Shared Function ReadLines(file As FileInfo, NofLine As Integer) As List(Of String)
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

                Friend Shared Iterator Function Parse(fileName As String) As IEnumerable(Of String)

                    Using sr As New StreamReader(fileName)
                        Do
                            Yield (sr.ReadLine)
                        Loop Until sr.EndOfStream = True
                    End Using
                End Function
            End Class
            Public Class Sequence
                Public seqs As New List(Of Bio.Sequence)
                Public streams As New List(Of Stream)
                Public SeqFiles As New List(Of FileInfo)

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
                Public Sub DoIt()
                    For Each File In SeqFiles
                        Dim x As New FileStream(File.FullName, FileMode.Open)
                        Try

                            Dim fa = Bio.IO.SequenceParsers.FindParserByFileName(File.FullName)
                            For Each Seq In fa.Parse(x)
                                Me.seqs.Add(Seq)
                            Next
                        Catch ex As Exception
                            Dim alf As Int16 = 54
                        End Try
                        x.Close()
                    Next
                End Sub

                ''' <summary>
                ''' Return Sorted Listed of Bio.Sequence or Nothing
                ''' </summary>
                ''' <param name="File"></param>
                ''' <returns></returns>
                Public Shared Function ImportIntoList(Optional File As FileInfo = Nothing, Optional Filter As String = "") As List(Of Bio.Sequence)
                    Dim Out As New List(Of Bio.Sequence)
                    If IsNothing(File) = True Then
                        File = Szunyi.IO.Files.SelectFile("Select Files", Filter)
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
                ''' <summary>
                ''' Return a List Of Sequences Sorted By SeqID
                ''' </summary>
                ''' <param name="Files"></param>
                ''' <returns></returns>
                Public Shared Function FromFiles(Files As List(Of FileInfo)) As List(Of Bio.Sequence)
                    Dim Out As New List(Of Bio.Sequence)
                    For Each File In Files
                        Out.AddRange(Szunyi.IO.Import.Sequence.FromFile(File))
                    Next
                    Return Out
                End Function

                Public Shared Function FromFile(File As FileInfo) As List(Of Bio.Sequence)
                    Dim Out As New List(Of Bio.Sequence)
                    If File.Exists = True Then
                        Dim x As New FileStream(File.FullName, FileMode.Open)
                        Dim fa = Bio.IO.SequenceParsers.FindParserByFileName(File.FullName)
                        For Each Seq In fa.Parse(x)
                            Out.Add(Seq)
                        Next
                        x.Close()
                        Out.Sort(AllComparares.BySeqID)
                    End If

                    Return Out
                End Function
            End Class



        End Class
        Public Class Export
#Region "Text"
            Public Shared Sub SaveText(ByVal Text As String, Optional Title As String = "Save As")
                Dim sfd1 As New SaveFileDialog
                sfd1.Title = Title
                If sfd1.ShowDialog = DialogResult.OK Then
                    Using sg As New System.IO.StreamWriter(sfd1.FileName)
                        sg.Write(Text)
                    End Using
                End If

            End Sub

            Public Shared Sub SaveText(ByVal Text As String, File As FileInfo)

                Using sg As New System.IO.StreamWriter(File.FullName)
                    sg.Write(Text)
                End Using


            End Sub
#End Region

#Region "Sequences"
            Friend Shared Sub SaveSequencesToSingleFasta(out As List(Of Sequence), Optional fileInfo As FileInfo = Nothing)
                If IsNothing(fileInfo) = True Then fileInfo = Szunyi.IO.Files.SelectSaveFile(MyConstants.Files.SequenceFilesToSave.FastaSingle)
                If IsNothing(fileInfo) = True Then Exit Sub

                Dim sw As New Bio.IO.FastA.FastAFormatter
                Dim TheStream As New FileStream(fileInfo.FullName, FileMode.Create)
                Try
                    sw.Format(TheStream, out)
                Catch ex As Exception
                    MsgBox(ex.ToString)
                End Try
                TheStream.Close()


                sw.Close
            End Sub
            Friend Shared Sub SaveSequencesToSingleGenBank(out As List(Of Sequence),
                                                           Optional fileInfo As FileInfo = Nothing, Optional WithCheck As Boolean = False)
                If IsNothing(fileInfo) = True Then fileInfo = Szunyi.IO.Files.SelectSaveFile(MyConstants.Files.SequenceFilesToSave.GenBankSingle)
                If IsNothing(fileInfo) = True Then Exit Sub
                If WithCheck = True Then
                    Dim x As New Szunyi.GenBank.FeatureManipulation
                    x.CorrectFeaturesStartAndEnd(out)

                End If

                Dim sw As New Bio.IO.GenBank.GenBankFormatter

                Dim TheStream As New FileStream(fileInfo.FullName, FileMode.Create)
                Try
                    sw.Format(TheStream, out)
                Catch ex As Exception
                    Dim alf As Int16 = 54
                End Try
                TheStream.Close()
                sw.Close
            End Sub
            Friend Shared Sub Sequences(allSeqs As List(Of Sequence), WithCheck As Boolean)
                Dim ofd1 As New OpenFileDialog
                ofd1.ValidateNames = False
                ofd1.CheckFileExists = False
                ofd1.CheckPathExists = True
                ofd1.Multiselect = False
                ofd1.Filter = MyConstants.Files.SequenceFilesToSave.SequenceFileTypeToSave
                ofd1.FilterIndex = 0
                ofd1.FileName = MyConstants.Files.SequenceFilesToSave.SelectFolder
                If ofd1.ShowDialog = DialogResult.OK Then
                    Select Case ofd1.FilterIndex
                        Case 1
                            Dim File As New FileInfo(ofd1.FileName)
                            SaveSequencesToSingleFasta(allSeqs, File)
                        Case 2
                            If ofd1.FileName = MyConstants.Files.SequenceFilesToSave.SelectFolder Then

                            End If
                        Case 3
                            Dim File As New FileInfo(ofd1.FileName)
                            SaveSequencesToSingleGenBank(allSeqs, File, WithCheck)
                        Case 4
                            If ofd1.FileName = MyConstants.Files.SequenceFilesToSave.SelectFolder Then

                            End If
                    End Select
                End If
            End Sub

#End Region


        End Class
        Public Class Net
            Public Class DownLoader
                Public Shared TheWebClient As New WebClient
                ''' <summary>
                ''' Return The Image or Nothing 
                ''' </summary>
                ''' <param name="Uri"></param>
                ''' <returns></returns>
                Public Shared Function GetImage(Uri As String) As Image
                    Try

                        Dim image_stream As New MemoryStream(TheWebClient.DownloadData(Uri))
                        Return Image.FromStream(image_stream)
                    Catch ex As Exception
                        MessageBox.Show("Error downloading picture " &
                                Uri & vbCrLf & ex.Message,
                                "Download Error",
                                MessageBoxButtons.OK,
                                MessageBoxIcon.Error)
                    End Try
                    Return Nothing
                End Function

                ''' <summary>
                ''' If Succesful Return Nothing else Return The Error Message
                ''' </summary>
                ''' <param name="uri"></param>
                ''' <param name="FileName"></param>
                ''' <returns></returns>
                Public Shared Function DownLoadAndSave(uri As String, FileName As String, FMode As FileMode, FAcces As FileAccess) As String
                    Try

                        Using image_stream As New MemoryStream(TheWebClient.DownloadData(uri))
                            Using fs As New FileStream(FileName, FMode, FAcces)

                                Dim b = image_stream.ToArray
                                fs.Write(b, 0, image_stream.Length)

                            End Using
                        End Using

                    Catch ex As Exception
                        Return ex.ToString
                    End Try
                    Return Nothing
                End Function

                ''' <summary>
                ''' Return The WebPage as string Or Nothing if Errors Happens
                ''' </summary>
                ''' <param name="uri"></param>
                ''' <returns></returns>
                Public Shared Function DownLoadText(uri As String) As String
                    Try
                        Return TheWebClient.DownloadString(uri)
                    Catch ex As Exception
                        Return Nothing
                    End Try

                End Function


            End Class
        End Class
    End Namespace
End Namespace

