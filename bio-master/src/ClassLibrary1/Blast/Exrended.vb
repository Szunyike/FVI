Imports System.Text
Imports System.IO
Imports Bio
Imports System.Globalization
Imports Bio.IO
Imports Bio.Util
Imports System.Windows.Forms
Imports System.Xml
Imports Bio.Web
Imports Bio.IO.GenBank

Imports Bio.Web.Blast
Imports ClassLibrary1.Extended

Public Class Helpery
    Public Function ForSuffixTree() As Bio.Sequence
        Dim seqs = OpenReferenceGenomeNetBio("Select Contig Files!")

        Dim str As New System.Text.StringBuilder
        Dim Ends As New List(Of Integer)
        Dim SegNames As New List(Of String)
        Ends.Add(0)
        Dim totalCount As Long
        For Each Seq In seqs.Values
            SegNames.Add(Seq.ID)
            totalCount += Seq.Count
            Ends.Add(totalCount)
            str.Append(Seq.ConvertToString(0, Seq.Count))
            SegNames.Add(Seq.ID & "rev")
            totalCount += Seq.Count
            Ends.Add(totalCount)
            Dim RevSeq As Bio.Sequence = Seq.GetReverseComplementedSequence
            str.Append(RevSeq.ConvertToString(0, RevSeq.Count))
        Next
        Dim result(totalCount) As Byte

        Dim NewSeq As New Sequence(Alphabets.AmbiguousDNA, str.ToString)

        Dim features As GenBank.SequenceFeatures = New GenBank.SequenceFeatures
        Dim FeaturesList As List(Of GenBank.FeatureItem) = features.All

        For i1 = 0 To Ends.Count - 2
            Dim loc As String = Ends(i1) + 1 & ".." & Ends(i1 + 1)
            Dim fea As New GenBank.FeatureItem(GenBank.StandardFeatureKeys.MiscStructure, loc)
            fea.Label = SegNames(i1)
            FeaturesList.Add(fea)
        Next
        Dim GBMetadata As New GenBank.GenBankMetadata
        GBMetadata.Features = features
        GBMetadata.Locus = New GenBank.GenBankLocusInfo
        GBMetadata.Accession = New GenBank.GenBankAccession
        NewSeq.Metadata.Add(Bio.Util.Helper.GenBankMetadataKey, GBMetadata)
        Return NewSeq
    End Function
    Public Function ForSuffixTreeS() As List(Of Bio.Sequence)
        Dim seqs = OpenReferenceGenomeNetBio("Select Contig Files!")
        Dim OutSeqs As New List(Of Bio.Sequence)

        Dim str As New System.Text.StringBuilder
        Dim Ends As New List(Of Integer)
        Dim SegNames As New List(Of String)
        Ends.Add(0)
        Dim totalCount As Long
        For Each Seq In seqs.Values
            SegNames.Add(Seq.ID)
            totalCount += Seq.Count
            Ends.Add(totalCount)
            str.Append(Seq.ConvertToString(0, Seq.Count))
            SegNames.Add(Seq.ID & "rev")
            totalCount += Seq.Count
            Ends.Add(totalCount)
            Dim RevSeq As Bio.Sequence = Seq.GetReverseComplementedSequence
            str.Append(RevSeq.ConvertToString(0, RevSeq.Count))
            If totalCount > 10000000 Then
                OutSeqs.Add(GetGenbank(str.ToString, Ends, SegNames))
                Ends.Clear()
                Ends.Add(0)
                SegNames.Clear()
                totalCount = 0
                str.Clear()
            End If
        Next
        OutSeqs.Add(GetGenbank(str.ToString, Ends, SegNames))
        Return OutSeqs
    End Function
    Private Function GetGenbank(Sequence As String, ends As List(Of Integer), SegNames As List(Of String)) As Bio.Sequence
        Dim NewSeq As New Sequence(Alphabets.AmbiguousDNA, Sequence)

        Dim features As GenBank.SequenceFeatures = New GenBank.SequenceFeatures
        Dim FeaturesList As List(Of GenBank.FeatureItem) = features.All

        For i1 = 0 To ends.Count - 2
            Dim loc As String = ends(i1) + 1 & ".." & ends(i1 + 1)
            Dim fea As New GenBank.FeatureItem(GenBank.StandardFeatureKeys.MiscStructure, loc)
            fea.Label = SegNames(i1)
            FeaturesList.Add(fea)
        Next
        Dim GBMetadata As New GenBank.GenBankMetadata
        GBMetadata.Features = features
        GBMetadata.Locus = New GenBank.GenBankLocusInfo
        GBMetadata.Accession = New GenBank.GenBankAccession
        NewSeq.Metadata.Add(Bio.Util.Helper.GenBankMetadataKey, GBMetadata)
        Return NewSeq
    End Function

#Region "IO"
#Region "Export"
    Public Sub SaveSequence(seq As ISequence)
        Dim sfd1 As New SaveFileDialog
        sfd1.Title = "Save Sequence As:"
        If sfd1.ShowDialog = DialogResult.OK Then
            Dim x As New FileStream(sfd1.FileName, FileMode.CreateNew)
            Dim t As New Bio.IO.FastA.FastAFormatter
            t.Format(x, seq)

            x.Close()
        End If
    End Sub
#End Region
#Region "Copy Folder"
    Public Sub FOlderToFolder()
        Dim fbd1 As New FolderBrowserDialog
        If fbd1.ShowDialog = DialogResult.OK Then
            Dim ParentDir As String = fbd1.SelectedPath
            If fbd1.ShowDialog = DialogResult.OK Then
                Dim DestinationDir As String = fbd1.SelectedPath
                If Directory.Exists(DestinationDir) = False Then
                    Directory.CreateDirectory(DestinationDir)
                End If
                For Each Folder In Directory.GetDirectories(ParentDir)
                    iteratedir(Folder, DestinationDir)
                Next
                CopyFiles(ParentDir, DestinationDir)
            End If
        End If
    End Sub
    Public Sub iteratedir(ParentDir As String, Destdir As String)
        For Each Folder In Directory.GetDirectories(ParentDir)
            iteratedir(Folder, Destdir)
        Next
        CopyFiles(ParentDir, Destdir)
    End Sub
    Public Sub CopyFiles(pDir As String, destdir As String)
        For Each FileName In Directory.GetFiles(pDir)
            Dim i As Int16 = FileName.LastIndexOf("\")
            Dim s As String = FileName.Substring(i, FileName.Length - i)
            s = destdir & s
            On Error Resume Next
            If File.Exists(s) = False Then
                File.Copy(FileName, s)
            End If

        Next
    End Sub

#End Region
    Public Sub SaveText(ByVal Text As String, Title As String)
        Dim sfd1 As New SaveFileDialog
        sfd1.Title = Title
        If sfd1.ShowDialog = DialogResult.OK Then
            Using sg As New System.IO.StreamWriter(sfd1.FileName)
                sg.Write(Text)
            End Using
        End If

    End Sub
    Public Function RemoveIllegalCharachter(FileName As String) As String
        FileName = FileName.Replace("\", "")
        FileName = FileName.Replace("/", "")
        FileName = FileName.Replace(":", "")

        Return FileName
    End Function
    Public Sub SaveText(ByVal Text As String, FileInfo As FileInfo)

        Using sg As New System.IO.StreamWriter(FileInfo.FullName)
            sg.Write(Text)
        End Using


    End Sub
    Public Sub SaveText(ByVal ls As List(Of String), Title As String)
        If ls.Count = 0 Then Exit Sub
        Dim sfd1 As New SaveFileDialog
        sfd1.Title = Title
        Dim str As New System.Text.StringBuilder
        For Each Item In ls
            str.Append(Item).AppendLine()
        Next
        str.Length = str.Length - 2
        If sfd1.ShowDialog = DialogResult.OK Then
            Using sg As New System.IO.StreamWriter(sfd1.FileName)
                sg.Write(str.ToString)
            End Using
        End If

    End Sub
    Public Sub SaveDataTable(ByRef dt As DataTable)
        Dim ofd1 As New SaveFileDialog
        If ofd1.ShowDialog = DialogResult.OK Then
            Dim str As New System.Text.StringBuilder
            For Each Col As DataColumn In dt.Columns
                str.Append(Col.ColumnName).Append(vbTab)
            Next

            str.AppendLine()
            For Each row As DataRow In dt.Rows
                For i1 = 0 To dt.Columns.Count - 1
                    str.Append(row.Item(i1).ToString.Replace(vbTab, "")).Append(vbTab)
                Next


                str.AppendLine()
            Next
            Using sw As New System.IO.StreamWriter(ofd1.FileName)
                sw.Write(str.ToString)
            End Using
        End If
    End Sub
    Public Function OpenReferenceGenomeNetBio(Title As String) As Dictionary(Of String, Bio.Sequence)
        Dim ofd1 As New OpenFileDialog
        ofd1.Title = "Select Reference Genome File(s)"
        ofd1.Multiselect = True
        Dim log As New StringBuilder
        Dim RefSeqs As New Dictionary(Of String, Global.Bio.Sequence)
        If ofd1.ShowDialog = DialogResult.OK Then
            For Each FileName As String In ofd1.FileNames
                Dim parser = Global.Bio.IO.SequenceParsers.FindParserByFileName(FileName)
                If IsNothing(parser) = False Then

                    Try
                        Dim x As New FileStream(FileName, FileMode.Open)
                        For Each Seq As Bio.Sequence In parser.Parse(x)
                            If RefSeqs.ContainsKey(Seq.ID) = True Then
                                log.Append("Duplicate Key:").Append(Seq.ID).AppendLine()
                            End If
                            RefSeqs.Add(Seq.ID, Seq)
                        Next
                        x.Close()
                    Catch ex As Exception
                        log.Append(FileName).AppendLine()
                    End Try


                End If
            Next
        End If
        If log.Length > 0 Then
            MsgBox(log.ToString)
        End If
        Return RefSeqs
    End Function
    Public Function OpenReferenceGenomeNetBio(File As System.IO.FileInfo) As Dictionary(Of String, Global.Bio.Sequence)

        Dim RefSeqs As New Dictionary(Of String, Global.Bio.Sequence)
        Dim log As New StringBuilder
        Dim parser = Global.Bio.IO.SequenceParsers.FindParserByFileName(File.FullName)
        If IsNothing(parser) = False Then
            Dim str As New FileStream(File.FullName, FileMode.Open)
            Dim x = parser.Parse(str)
            If x.Count = 1 Then
                RefSeqs.Add(File.Name, x.First)
            Else
                For Each Seq In x
                    If RefSeqs.ContainsKey(Seq.ID) = True Then
                        log.Append("Duplicate Key:").Append(Seq.ID).AppendLine()
                    End If
                    RefSeqs.Add(Seq.ID, Seq)
                Next
            End If
            str.Close()
        End If

        If log.Length > 0 Then
            MsgBox(log.ToString)
        End If
        Return RefSeqs
    End Function
    Public Function OpenBlastToDictionaryOfBlastResult(File As FileInfo,
                      Optional ByRef Result As Dictionary(Of String, BlastSearchRecord) = Nothing) As Dictionary(Of String, BlastSearchRecord)
        Dim NewResults As New Dictionary(Of String, BlastSearchRecord)

        Dim ofd1 As New OpenFileDialog
        ofd1.Filter = "*.xml|*.xml"
        ofd1.Multiselect = True
        ofd1.Title = "Select BLAST File"
        Dim x As New List(Of BlastResult)
        If IsNothing(Result) = True Then Result = New Dictionary(Of String, BlastSearchRecord)
        Using reader As TextReader = New StreamReader(File.FullName)
            Dim BlastParser = New Extended.BlastXmlParser
            Dim x1 = BlastParser.ParseXMLStream(reader, True)

            For Each Record In x1.Records
                Result.Add(Record.IterationQueryDefinition, Record)
            Next
        End Using


        Return Result
    End Function

    Public Function OpenBlastToDictionaryOfBlastResult(Files As List(Of FileInfo), WithAlignment As Boolean,
                                                       Optional Filter As List(Of String) = Nothing,
        Optional ByRef Result As Dictionary(Of String, List(Of BlastSearchRecord)) = Nothing) As Dictionary(Of String, List(Of BlastSearchRecord))
        Dim NewResults As New Dictionary(Of String, BlastSearchRecord)


        Dim x As New List(Of BlastResult)
        If IsNothing(Result) = True Then Result = New Dictionary(Of String, List(Of BlastSearchRecord))
        Dim NofDuplicated As Long = 0
        For Each File In Files
            Using reader As TextReader = New StreamReader(File.FullName)
                Dim BlastParser = New Extended.BlastXmlParser
                Dim x1 = BlastParser.ParseXMLStream(reader, WithAlignment, Filter)

                For Each Record In x1.Records
                    If Record.Hits.Count > 0 Then
                        Dim QName As String = GetLocusTag(Record.IterationQueryDefinition)
                        If Result.ContainsKey(QName) = True Then
                            Result(QName).Add(Record)
                        Else
                            Result.Add(QName, New List(Of BlastSearchRecord))
                            Result(QName).Add(Record)
                        End If
                    End If

                Next
            End Using
        Next

        Return Result
    End Function
    Private Function GetLocusTag(s1 As String) As String
        Dim s2() As String = s1.Split("|")
        If s2.Length <> 1 Then
            For i1 = 0 To s2.Length - 2
                If s2(i1) = "locus" Then
                    Return s2(i1 + 1)
                End If
            Next
        End If
        Return s1
    End Function
    Public Function OpenBlastToListOfBlastResult(Optional FileName As String = "") As List(Of BlastSearchRecord)
        Dim NewResults As New List(Of BlastSearchRecord)
        Dim ofd1 As New OpenFileDialog
        ofd1.Multiselect = True
        ofd1.Filter = "*.xml|*.xml"
        ofd1.Multiselect = True
        ofd1.Title = "Select BLAST File"
        Dim x As New List(Of BlastResult)
        If ofd1.ShowDialog = DialogResult.OK Then
            For Each FileName In ofd1.FileNames
                If FileName <> "" Then
                    Using reader As TextReader = New StreamReader(FileName)
                        Dim BlastParser = New Extended.BlastXmlParser
                        For Each Record In BlastParser.ParseXMLStream(reader, True).Records
                            NewResults.Add(Record)
                        Next
                    End Using
                End If
            Next

        End If

        Return NewResults
    End Function

    Public Function OpenBlastToExtendedHit(Optional BlastWoAlignment As Boolean = False) As List(Of ExtendedHit)
        Dim ofd1 As New OpenFileDialog
        ofd1.Multiselect = True
        ofd1.Title = "Select BLAST File"
        ofd1.Filter = "*.xml|*.xml"
        Dim ExtendedHitList As New List(Of ExtendedHit)
        If ofd1.ShowDialog = DialogResult.OK Then
            For Each FileName In ofd1.FileNames
                Using reader As TextReader = New StreamReader(FileName)
                    Dim BlastParser As New Extended.BlastXmlParser
                    For Each BlastRecord In BlastParser.ParseXMLStream(reader, True).Records
                        If BlastWoAlignment = True Then
                            For Each hit In BlastRecord.Hits
                                Dim x As New ExtendedHit
                                x.Hit = hit
                                x.QueryDef = BlastRecord.IterationQueryDefinition
                                ExtendedHitList.Add(x)
                            Next
                        End If
                    Next
                End Using
            Next
        End If
        Return ExtendedHitList
    End Function

    Public Sub SaveSequences(out As Dictionary(Of String, Global.Bio.ISequence))
        Dim sfd1 As New SaveFileDialog
        sfd1.Title = "Save Sequences As"
        If sfd1.ShowDialog = DialogResult.OK Then
            Dim outList As New List(Of Global.Bio.ISequence)
            For Each sg In out.Values
                outList.Add(sg)
            Next
            Dim x As New FileStream(sfd1.FileName, FileMode.CreateNew)
            Dim t As New Global.Bio.IO.FastA.FastAFormatter()
            t.Format(x, outList)

            x.Close()

        End If
    End Sub

    Public Sub SaveSequences(out As List(Of Global.Bio.ISequence), Optional FileName As String = "")
        Dim sfd1 As New SaveFileDialog
        sfd1.Title = "Save Sequences As"
        If FileName <> "" Then
            sfd1.FileName = FileName
            sfd1.DefaultExt = ".fas"
        End If
        If sfd1.ShowDialog = DialogResult.OK Then
            Dim x As New FileStream(sfd1.FileName, FileMode.CreateNew)
            Dim t As New Global.Bio.IO.FastA.FastAFormatter()
            t.Format(x, out)

            x.Close()
        End If
    End Sub

    Public Function OpenTabDelimitedFile() As DataTable
        Dim ofd1 As New OpenFileDialog
        Dim dt As New DataTable
        If ofd1.ShowDialog = DialogResult.OK Then
            Dim Separator = DetermineSeparator(ofd1.FileName)
            Using sr As New StreamReader(ofd1.FileName)
                dt.Columns.AddRange(CreateColumnsFronLine(sr.ReadLine, Separator).ToArray)
                Do
                    Dim s1() As String = sr.ReadLine.Split(Separator)
                    Dim Row As DataRow = dt.NewRow
                    Row.ItemArray = s1
                    dt.Rows.Add(Row)
                Loop Until sr.EndOfStream = True
            End Using
        End If
        Return dt
    End Function
    Public Function CreateColumnsFronLine(line As String, separator As String) As List(Of DataColumn)
        Dim s1() As String = line.Split(separator)
        Dim ListOfColumns As New List(Of DataColumn)

        Dim ColumNames As New List(Of String)
        Dim IsDifferent As Boolean = True
        For Each ColumnName In s1
            If ColumNames.Contains(ColumnName) = False Then
                ColumNames.Add(ColumnName)
            Else
                IsDifferent = False
            End If
        Next
        If IsDifferent = True Then
            For Each ColumnName In s1
                Dim NewColumn As New DataColumn(ColumnName)
                ListOfColumns.Add(NewColumn)
            Next
        Else
            For i1 = 0 To s1.Length - 1
                Dim NewColumn As New DataColumn(i1)
                ListOfColumns.Add(NewColumn)
            Next
        End If

        Return ListOfColumns
    End Function
    Public Function DetermineSeparator(FileName As String) As String
        Dim s1 As New List(Of String)
        s1.Add(vbTab)
        s1.Add(",")
        s1.Add(";")
        For Each Separator In s1
            If CheckTxtFile(FileName, Separator) = True Then
                Return Separator
            End If
        Next
        Return String.Empty
    End Function
    Public Function CheckTxtFile(FileName As String, Separator As String)
        Using sr As New StreamReader(FileName)
            Dim Count As Integer
            Dim s1() As String = Split(sr.ReadLine, Separator)
            Count = s1.Length
            If Count = 1 Then Return False
            Do
                If Count <> sr.ReadLine.Split(Separator).Length Then
                    Return False
                End If
            Loop Until sr.EndOfStream = True
            Return True
        End Using
        Return False
    End Function

    Public Sub SortingFile()
        Dim ofd1 As New OpenFileDialog
        ofd1.Title = "Select File"
        If ofd1.ShowDialog <> DialogResult.OK Then Exit Sub
        Dim Lines As New List(Of String)
        Using sr As New StreamReader(ofd1.FileName)
            Do
                Lines.Add(sr.ReadLine)
            Loop Until sr.EndOfStream = True
        End Using
        Lines.Sort()
        Using sw As New StreamWriter(ofd1.FileName & "2")
            For Each line In Lines
                sw.Write(line)
                sw.Write(vbCrLf)
            Next
        End Using

    End Sub
#End Region

    Public Function ListToString(x As List(Of String)) As String
        Dim str As New StringBuilder
        For Each s In x.Distinct
            str.Append(s).AppendLine()
        Next
        Return str.ToString
    End Function
    Public Function GetPureFileName(Filename As String) As String
        Dim i1 As Integer = Filename.LastIndexOf("\")
        Dim i2 As Integer = Filename.LastIndexOf(".")
        Try
            Return Filename.Substring(i1, i2 - i1)
        Catch ex As Exception
            Return String.Empty
        End Try


    End Function
    Public Sub FastaToSeqIDPosition(OriginalFileName As String, NewFileName As String, Optional Increment As Integer = 1)
        Dim Portion As Integer = 10000000
        Dim Positions As New List(Of Long)
        Dim Round As Integer = 0
        Dim LastWrited As Long = 0

        Using InStream As New FileStream(OriginalFileName, FileMode.Open, FileAccess.Read)
            Using br As New BinaryReader(InStream)
                Using OutStream As New FileStream(NewFileName, FileMode.Create, FileAccess.ReadWrite)
                    Using bw As New BinaryWriter(OutStream)
                        Dim max = InStream.Length
                        Do
                            Dim bytes() As Byte
                            If max = InStream.Position Then Exit Do
                            If max - InStream.Position < Portion Then
                                bytes = br.ReadBytes(max - InStream.Position)
                            Else
                                bytes = br.ReadBytes(Portion)
                            End If
                            For i = 0 To bytes.Count - 1
                                If bytes(i) = 62 Then
                                    Positions.Add(i + Portion * Round)
                                End If
                            Next
                            For i = 0 To Positions.Count - 1 Step Increment
                                bw.Write(Positions(i))
                                LastWrited = i
                            Next
                            If LastWrited <> Positions.Count - 1 Then
                                Dim sg = New List(Of Long)
                                For i = LastWrited + 1 To Positions.Count - 1
                                    sg.Add(Positions(i))
                                Next
                                Positions.Clear()
                                Positions = sg
                            Else
                                Positions.Clear()
                            End If
                        Loop

                    End Using
                End Using
            End Using
        End Using


    End Sub
    Public Function GetDirs(Optional maxDepth As Integer = 2) As List(Of System.IO.DirectoryInfo)
        Dim fbd1 As New FolderBrowserDialog
        Dim Dirs As New List(Of System.IO.DirectoryInfo)
        If fbd1.ShowDialog = DialogResult.OK Then
            Dim BasicDir As New DirectoryInfo(fbd1.SelectedPath)
            ProcessDirectory(BasicDir, Dirs, 0, maxDepth)
        End If
        Return Dirs
    End Function
    Public Function GetFiles(Optional Depth As Integer = 2) As List(Of System.IO.FileInfo)
        Dim fbd1 As New FolderBrowserDialog
        Dim Dirs As New List(Of System.IO.DirectoryInfo)

        If fbd1.ShowDialog = DialogResult.OK Then
            Dim BasicDir As New DirectoryInfo(fbd1.SelectedPath)
            ProcessDirectory(BasicDir, Dirs, 0, Depth)
        End If
        Return GetAllFilesFromDirectories(Dirs)
    End Function
    Private Sub ProcessDirectory(ByVal targetDirectory As DirectoryInfo, Dirs As List(Of System.IO.DirectoryInfo),
                                CurrentDepth As Integer, MaxDepth As Integer)
        CurrentDepth += 1
        If CurrentDepth > MaxDepth Then Exit Sub
        For Each SubDirectory In targetDirectory.GetDirectories
            Dirs.Add(SubDirectory)
            ProcessDirectory(SubDirectory, Dirs, CurrentDepth, MaxDepth)
        Next

    End Sub
    Private Function GetAllFilesFromDirectories(Dirs As List(Of System.IO.DirectoryInfo)) As List(Of System.IO.FileInfo)
        Dim Files As New List(Of System.IO.FileInfo)
        For Each SubDir In Dirs
            Files.AddRange(SubDir.GetFiles)
        Next
        Return Files
    End Function

End Class


Namespace Extended
    Public Enum Constants
        LocusTag = 0
        GeneID = 1
        GeneSymbol = 2
        Definition = 3
        GI = 4

    End Enum
    Public Class BlastXmlParser

#Region "Methods"

        Private Shared Sub DoBlastOutput(ByVal element As String, ByVal value As String, ByVal metadata As BlastXmlMetadata)
            Select Case element
                Case "BlastOutput_program"
                    metadata.Program = value
                    Exit Select
                Case "BlastOutput_version"
                    metadata.Version = value
                    Exit Select
                Case "BlastOutput_reference"
                    metadata.Reference = value
                    Exit Select
                Case "BlastOutput_db"
                    metadata.Database = value
                    Exit Select
                Case "BlastOutput_query-ID"
                    metadata.QueryId = value
                    Exit Select
                Case "BlastOutput_query-def"
                    metadata.QueryDefinition = value
                    Exit Select
                Case "BlastOutput_query-len"
                    metadata.QueryLength = Integer.Parse(value, CultureInfo.InvariantCulture)
                    Exit Select
                Case "BlastOutput_query-seq"
                    metadata.QuerySequence = value
                    Exit Select
                Case Else
                    Dim message As String = [String].Format(CultureInfo.CurrentCulture, "DoBlast", element)
                    Throw New FormatException(message)
            End Select
        End Sub

        Private Shared Sub DoParameters(ByVal element As String, ByVal value As String, ByVal metadata As BlastXmlMetadata)
            Select Case element
                Case "Parameters_matrix"
                    metadata.ParameterMatrix = value
                    Exit Select
                Case "Parameters_expect"
                    metadata.ParameterExpect = Double.Parse(value, CultureInfo.InvariantCulture)
                    Exit Select
                Case "Parameters_include"
                    metadata.ParameterInclude = Double.Parse(value, CultureInfo.InvariantCulture)
                    Exit Select
                Case "Parameters_sc-match"
                    metadata.ParameterMatchScore = Integer.Parse(value, CultureInfo.InvariantCulture)
                    Exit Select
                Case "Parameters_sc-mismatch"
                    metadata.ParameterMismatchScore = Integer.Parse(value, CultureInfo.InvariantCulture)
                    Exit Select
                Case "Parameters_gap-open"
                    metadata.ParameterGapOpen = Integer.Parse(value, CultureInfo.InvariantCulture)
                    Exit Select
                Case "Parameters_gap-extend"
                    metadata.ParameterGapExtend = Integer.Parse(value, CultureInfo.InvariantCulture)
                    Exit Select
                Case "Parameters_filter"
                    metadata.ParameterFilter = value
                    Exit Select
                Case "Parameters_pattern"
                    metadata.ParameterPattern = value
                    Exit Select
                Case "Parameters_entrez-query"
                    metadata.ParameterEntrezQuery = value
                    Exit Select
                Case Else
                    Dim message As String = [String].Format(CultureInfo.CurrentCulture, "DoParameters", element)
                    Throw New FormatException(message)
            End Select
        End Sub

        Private Shared Sub DoIteration(ByVal element As String, ByVal value As String, ByVal curRecord As BlastSearchRecord)
            Select Case element
                Case "Iteration_iter-num"
                    curRecord.IterationNumber = Integer.Parse(value, CultureInfo.InvariantCulture)
                    Exit Select
                Case "Iteration_query-ID"
                    curRecord.IterationQueryId = value
                    Exit Select
                Case "Iteration_query-def"
                    curRecord.IterationQueryDefinition = value
                    Exit Select
                Case "Iteration_query-len"
                    curRecord.IterationQueryLength = Integer.Parse(value, CultureInfo.InvariantCulture)
                    Exit Select
                Case "Iteration_hits"
                    ' ignore
                    Exit Select
                Case "Iteration_stat"
                    ' ignore
                    Exit Select
                Case "Iteration_message"
                    curRecord.IterationMessage = value
                    Exit Select
                Case Else
                    Dim message As String = [String].Format(CultureInfo.CurrentCulture, "DoIteration", element)
                    Throw New FormatException(message)
            End Select
        End Sub

        Private Shared Sub DoHit(ByVal element As String, ByVal value As String, ByVal curHit As Hit)
            Select Case element
                Case "Hit_num"
                    ' ignore
                    Exit Select
                Case "Hit_id"
                    curHit.Id = value
                    Exit Select
                Case "Hit_def"
                    curHit.Def = value
                    Exit Select
                Case "Hit_accession"
                    curHit.Accession = value
                    Exit Select
                Case "Hit_len"
                    curHit.Length = Long.Parse(value, CultureInfo.InvariantCulture)
                    Exit Select
                Case "Hit_hsps"
                    ' ignore
                    Exit Select
                Case Else
                    Dim message As String = [String].Format(CultureInfo.CurrentCulture, "DoHit", element)
                    Throw New FormatException(message)
            End Select
        End Sub

        Private Shared Sub DoHsp(ByVal element As String, ByVal value As String, ByVal hsp As Hsp)
            Select Case element
                Case "Hsp_num"
                    ' ignore
                    Exit Select
                Case "Hsp_bit-score"
                    hsp.BitScore = Double.Parse(value, CultureInfo.InvariantCulture)
                    Exit Select
                Case "Hsp_score"
                    hsp.Score = Double.Parse(value, CultureInfo.InvariantCulture)
                    Exit Select
                Case "Hsp_evalue"
                    hsp.EValue = Double.Parse(value, CultureInfo.InvariantCulture)
                    Exit Select
                Case "Hsp_query-from"
                    hsp.QueryStart = Long.Parse(value, CultureInfo.InvariantCulture)
                    Exit Select
                Case "Hsp_query-to"
                    hsp.QueryEnd = Long.Parse(value, CultureInfo.InvariantCulture)
                    Exit Select
                Case "Hsp_hit-from"
                    hsp.HitStart = Long.Parse(value, CultureInfo.InvariantCulture)
                    Exit Select
                Case "Hsp_hit-to"
                    hsp.HitEnd = Long.Parse(value, CultureInfo.InvariantCulture)
                    Exit Select
                Case "Hsp_query-frame"
                    hsp.QueryFrame = Long.Parse(value, CultureInfo.InvariantCulture)
                    Exit Select
                Case "Hsp_hit-frame"
                    hsp.HitFrame = Long.Parse(value, CultureInfo.InvariantCulture)
                    Exit Select
                Case "Hsp_identity"
                    hsp.IdentitiesCount = Long.Parse(value, CultureInfo.InvariantCulture)
                    Exit Select
                Case "Hsp_positive"
                    hsp.PositivesCount = Long.Parse(value, CultureInfo.InvariantCulture)
                    Exit Select
                Case "Hsp_align-len"
                    hsp.AlignmentLength = Long.Parse(value, CultureInfo.InvariantCulture)
                    Exit Select
                Case "Hsp_density"
                    hsp.Density = Integer.Parse(value, CultureInfo.InvariantCulture)
                    Exit Select
                Case "Hsp_qseq"
                    hsp.QuerySequence = value
                    Exit Select
                Case "Hsp_hseq"
                    hsp.HitSequence = value
                    Exit Select
                Case "Hsp_midline"
                    hsp.Midline = value
                    Exit Select
                Case "Hsp_pattern-from"
                    hsp.PatternFrom = Integer.Parse(value, CultureInfo.InvariantCulture)
                    Exit Select
                Case "Hsp_pattern-to"
                    hsp.PatternTo = Integer.Parse(value, CultureInfo.InvariantCulture)
                    Exit Select
                Case "Hsp_gaps"
                    hsp.Gaps = Integer.Parse(value, CultureInfo.InvariantCulture)
                    Exit Select
                Case Else
                    Dim message As String = [String].Format(CultureInfo.CurrentCulture, "DoHsp", element)
                    Throw New FormatException(message)
            End Select
        End Sub

        Private Shared Sub DoStatistics(ByVal element As String, ByVal value As String, ByVal curStats As BlastStatistics)
            Select Case element
                Case "Statistics_db-num"
                    ' ignore
                    Exit Select
                Case "Statistics_db-len"
                    curStats.DatabaseLength = Long.Parse(value, CultureInfo.InvariantCulture)
                    Exit Select
                Case "Statistics_hsp-len"
                    curStats.HspLength = Long.Parse(value, CultureInfo.InvariantCulture)
                    Exit Select
                Case "Statistics_eff-space"
                    curStats.EffectiveSearchSpace = Double.Parse(value, CultureInfo.InvariantCulture)
                    Exit Select
                Case "Statistics_kappa"
                    curStats.Kappa = Double.Parse(value, CultureInfo.InvariantCulture)
                    Exit Select
                Case "Statistics_lambda"
                    curStats.Lambda = Double.Parse(value, CultureInfo.InvariantCulture)
                    Exit Select
                Case "Statistics_entropy"
                    curStats.Entropy = Double.Parse(value, CultureInfo.InvariantCulture)
                    Exit Select
                Case Else
                    Dim message As String = [String].Format(CultureInfo.CurrentCulture, "DoStatistics", element)
                    Throw New FormatException(message)
            End Select
        End Sub

        ''' <summary>
        ''' This method expects a single XML document and returns one BlastResult.
        ''' </summary>
        ''' <param name="doc">A Stringbuilder containing the XML document.</param>
        ''' <returns>The BlastResult.</returns>
        Private Shared Function ParseXML(ByVal doc As StringBuilder) As BlastResult
            Dim result As New BlastResult()
            Try
                Dim settings As New XmlReaderSettings()
                settings.DtdProcessing = DtdProcessing.Ignore
                ' don't error when encountering a DTD spec
                ' Setting the XmlResolver to null causes the DTDs specified in the XML
                ' header to be ignored. 
                settings.XmlResolver = Nothing

                Dim sr As StringReader = Nothing
                Try
                    sr = New StringReader(doc.ToString())
                    Using r As XmlReader = XmlReader.Create(sr, settings)
                        Dim curElement As String = String.Empty
                        Dim curRecord As New BlastSearchRecord()
                        Dim curHit As Hit = Nothing
                        Dim curHsp As Hsp = Nothing
                        Dim curStatistics As BlastStatistics = Nothing
                        Dim curMetadata As BlastXmlMetadata = Nothing
                        While r.Read()
                            Select Case r.NodeType
                                Case XmlNodeType.Element
                                    curElement = r.Name
                                    ' ApplicationLog.WriteLine("element: " + curElement);
                                    If curElement = "Hit" Then
                                        curHit = New Hit()
                                    ElseIf curElement = "Hsp" Then
                                        curHsp = New Hsp()
                                    ElseIf curElement = "Statistics" Then
                                        curStatistics = New BlastStatistics()
                                    ElseIf curElement = "BlastOutput" Then
                                        curMetadata = New BlastXmlMetadata()
                                    End If
                                    Exit Select
                                Case XmlNodeType.Text
                                    ' ApplicationLog.WriteLine("text: " + r.Value);
                                    If curElement.StartsWith("BlastOutput_", StringComparison.OrdinalIgnoreCase) Then
                                        DoBlastOutput(curElement, r.Value, curMetadata)
                                    ElseIf curElement.StartsWith("Parameters_", StringComparison.OrdinalIgnoreCase) Then
                                        DoParameters(curElement, r.Value, curMetadata)
                                    ElseIf curElement.StartsWith("Iteration_", StringComparison.OrdinalIgnoreCase) Then
                                        DoIteration(curElement, r.Value, curRecord)
                                    ElseIf curElement.StartsWith("Statistics_", StringComparison.OrdinalIgnoreCase) Then
                                        DoStatistics(curElement, r.Value, curStatistics)
                                    ElseIf curElement.StartsWith("Hit_", StringComparison.OrdinalIgnoreCase) Then
                                        DoHit(curElement, r.Value, curHit)
                                    ElseIf curElement.StartsWith("Hsp_", StringComparison.OrdinalIgnoreCase) Then
                                        DoHsp(curElement, r.Value, curHsp)
                                    Else
                                        ' ApplicationLog.WriteLine("BlastXMLParser Unhandled: curElement '{0}'", curElement)
                                    End If
                                    Exit Select
                                Case XmlNodeType.XmlDeclaration
                                    ' ApplicationLog.WriteLine("declaration: {0}, {1}", r.Name, r.Value);
                                    Exit Select
                                Case XmlNodeType.ProcessingInstruction
                                    ' ApplicationLog.WriteLine("instruction: {0}, {1}", r.Name, r.Value);
                                    Exit Select
                                Case XmlNodeType.Comment
                                    ' ApplicationLog.WriteLine("comment: " + r.Value);
                                    Exit Select
                                Case XmlNodeType.EndElement
                                    ' ApplicationLog.WriteLine("endelement: " + r.Name);
                                    If r.Name = "Iteration" Then
                                        result.Records.Add(curRecord)
                                        curRecord = New BlastSearchRecord()
                                    ElseIf r.Name = "Statistics" Then
                                        curRecord.Statistics = curStatistics
                                    ElseIf r.Name = "Hit" Then
                                        curRecord.Hits.Add(curHit)
                                    ElseIf r.Name = "Hsp" Then
                                        curHit.Hsps.Add(curHsp)
                                    ElseIf r.Name = "BlastOutput" Then
                                        result.Metadata = curMetadata
                                    End If
                                    Exit Select
                            End Select
                        End While
                    End Using
                Finally
                    If sr IsNot Nothing Then
                        sr.Dispose()
                    End If
                End Try
            Catch e As Exception
                ' ApplicationLog.Exception(e)
                Throw
            End Try
            Return result
        End Function

        ''' <summary>
        ''' This method expects a single XML document and returns one BlastResult.
        ''' </summary>
        ''' <param name="reader ">A TextReader containing the XML document.</param>
        ''' <returns>The BlastResult.</returns>
        Public Function ParseXMLStream(ByVal reader As TextReader, WithAlignment As Boolean,
                                       Optional Filter As List(Of String) = Nothing) As BlastResult
            Dim result As New BlastResult()
            Dim infile As New StringBuilder
            Try
                Dim sr As StringReader = Nothing

                Try

                    Dim line As String = reader.ReadLine
                    infile.Append(line)
                    Do
                        line = reader.ReadLine
                        infile.Append(line)
                    Loop Until line.Contains("</BlastOutput_param>") = True
                    infile.Append("</BlastOutput>")

                    ParseXmlStream(result, infile.ToString, WithAlignment, Filter)
                    infile.Length = 0
                    Do
                        line = reader.ReadLine

                        If line.Contains("<Iteration>") Then
                            infile.Append(line)
                            Do
                                line = reader.ReadLine
                                infile.Append(line)
                                If line.Contains("</Iteration>") = True Then
                                    ParseXmlStream(result, infile.ToString, WithAlignment, Filter)
                                    infile.Length = 0
                                    Exit Do
                                End If
                            Loop
                        ElseIf line.Contains("</BlastOutput>") = True Then
                            Exit Do
                        End If
                    Loop

                Finally
                    If sr IsNot Nothing Then
                        sr.Dispose()
                    End If
                End Try
            Catch e As Exception
                '  ApplicationLog.Exception(e)
                Dim alf As Int16 = 32
            End Try
            Return result
        End Function
        Private Sub ParseXmlStream(ByRef result As BlastResult, ByVal InFile As String, WithAlignment As Boolean, Optional Filter As List(Of String) = Nothing)
            Dim sr = New StringReader(InFile.ToString())
            Dim settings As New XmlReaderSettings()
            settings.DtdProcessing = DtdProcessing.Ignore
            ' don't error when encountering a DTD spec
            ' Setting the XmlResolver to null causes the DTDs specified in the XML
            ' header to be ignored. 
            settings.XmlResolver = Nothing

            Using r As XmlReader = XmlReader.Create(sr, settings)
                Dim curElement As String = String.Empty
                Dim curRecord As New BlastSearchRecord()
                Dim curHit As Hit = Nothing
                Dim curHsp As Hsp = Nothing
                Dim curStatistics As BlastStatistics = Nothing
                Dim curMetadata As BlastXmlMetadata = Nothing
                While r.Read()
                    Select Case r.NodeType
                        Case XmlNodeType.Element
                            curElement = r.Name
                            ' ApplicationLog.WriteLine("element: " + curElement);
                            If curElement = "Hit" Then
                                curHit = New Hit()
                            ElseIf curElement = "Hsp" Then
                                curHsp = New Hsp()
                            ElseIf curElement = "Statistics" Then
                                curStatistics = New BlastStatistics()
                            ElseIf curElement = "BlastOutput" Then
                                curMetadata = New BlastXmlMetadata()
                            End If
                            Exit Select
                        Case XmlNodeType.Text
                            ' ApplicationLog.WriteLine("text: " + r.Value);
                            If curElement.StartsWith("BlastOutput_", StringComparison.OrdinalIgnoreCase) Then
                                DoBlastOutput(curElement, r.Value, curMetadata)
                            ElseIf curElement.StartsWith("Parameters_", StringComparison.OrdinalIgnoreCase) Then
                                DoParameters(curElement, r.Value, curMetadata)
                            ElseIf curElement.StartsWith("Iteration_", StringComparison.OrdinalIgnoreCase) Then
                                DoIteration(curElement, r.Value, curRecord)
                            ElseIf curElement.StartsWith("Statistics_", StringComparison.OrdinalIgnoreCase) Then
                                DoStatistics(curElement, r.Value, curStatistics)
                            ElseIf curElement.StartsWith("Hit_", StringComparison.OrdinalIgnoreCase) Then
                                DoHit(curElement, r.Value, curHit)
                            ElseIf curElement.StartsWith("Hsp_", StringComparison.OrdinalIgnoreCase) Then
                                DoHsp(curElement, r.Value, curHsp)
                                If WithAlignment = False Then
                                    curHsp.HitSequence = ""
                                    curHsp.QuerySequence = ""
                                    curHsp.Midline = ""
                                End If
                            Else
                                '  ApplicationLog.WriteLine("BlastXMLParser Unhandled: curElement '{0}'", curElement)
                            End If
                            Exit Select
                        Case XmlNodeType.XmlDeclaration
                            ' ApplicationLog.WriteLine("declaration: {0}, {1}", r.Name, r.Value);
                            Exit Select
                        Case XmlNodeType.ProcessingInstruction
                            ' ApplicationLog.WriteLine("instruction: {0}, {1}", r.Name, r.Value);
                            Exit Select
                        Case XmlNodeType.Comment
                            ' ApplicationLog.WriteLine("comment: " + r.Value);
                            Exit Select
                        Case XmlNodeType.EndElement
                            ' ApplicationLog.WriteLine("endelement: " + r.Name);
                            If r.Name = "Iteration" Then
                                If IsNothing(Filter) = True Then
                                    result.Records.Add(curRecord)

                                ElseIf Filter.Contains(curRecord.IterationQueryDefinition) = True Then
                                    result.Records.Add(curRecord)
                                End If

                                curRecord = New BlastSearchRecord()
                            ElseIf r.Name = "Statistics" Then
                                curRecord.Statistics = curStatistics
                            ElseIf r.Name = "Hit" Then
                                curRecord.Hits.Add(curHit)
                            ElseIf r.Name = "Hsp" Then
                                curHit.Hsps.Add(curHsp)
                            ElseIf r.Name = "BlastOutput" Then
                                result.Metadata = curMetadata
                            End If
                            Exit Select
                    End Select
                End While
            End Using

        End Sub

        ''' <summary>
        ''' Read XML BLAST data from the reader, and build one or more
        ''' BlastRecordGroup objects (each containing one or more
        ''' BlastSearchRecord results).
        ''' </summary>
        ''' <param name="reader">The text source</param>
        ''' <returns>A list of BLAST iteration objects</returns>
        Public Function Parse(ByVal reader As TextReader) As List(Of BlastResult)
            Dim records As New List(Of BlastResult)()
            Dim sb As New StringBuilder()
            Dim lineNumber As Long = 0
            Dim line As String = ReadNextLine(reader, False)
            lineNumber += 1
            While Not String.IsNullOrEmpty(line)
                If line.StartsWith("RPS-BLAST", StringComparison.OrdinalIgnoreCase) Then
                    line = ReadNextLine(reader, False)
                    lineNumber += 1
                    Continue While
                End If
                If line.StartsWith("<?xml version", StringComparison.OrdinalIgnoreCase) AndAlso lineNumber > 1 Then
                    records.Add(ParseXML(sb))
                    sb = New StringBuilder()
                End If
                sb.AppendLine(line)
                line = ReadNextLine(reader, False)
                lineNumber += 1
            End While

            If sb.Length > 0 Then
                records.Add(ParseXML(sb))
                ' records.Add(ParseXMLStream(reader))

            End If
            If records.Count = 0 Then
                Dim message As String = "NoBlastRecord" 'My.Resources.BlastNoRecords
                '  Trace.Report(message)
                Throw New FormatException(message)
            End If
            Return records
        End Function

        ''' <summary>
        ''' Read XML BLAST data from the specified file, and build one or more
        ''' BlastRecordGroup objects (each containing one or more
        ''' BlastSearchRecord results).
        ''' </summary>
        ''' <param name="fileName">The name of the file</param>
        ''' <returns>A list of BLAST iteration objects</returns>
        Public Function Parse(ByVal fileName As String) As List(Of BlastResult)
            Dim records As New List(Of BlastResult)()
            Using reader As TextReader = New StreamReader(fileName)
                records = DirectCast(Parse(reader), List(Of BlastResult))
            End Using
            Return records
        End Function

        ''' <summary>
        ''' Reads next line considering
        ''' </summary>
        ''' <returns></returns>
        Private Shared Function ReadNextLine(ByVal reader As TextReader, ByVal skipBlankLines As Boolean) As String
            Dim line As String
            If reader.Peek() = -1 Then
                Return Nothing
            End If

            line = reader.ReadLine()
            While skipBlankLines AndAlso String.IsNullOrWhiteSpace(line) AndAlso reader.Peek() <> -1
                line = reader.ReadLine()
            End While

            Return line
        End Function

#End Region
    End Class

    <Serializable()>
    Public Class ExtendedHit
        Implements IComparable(Of ExtendedHit)

        Private m_IdentityPercent As Double
        Private m_IdentityCount As Integer
        Public Property Comparsion As String

        Public Sub New()
            m_IdentityCount = 20
            m_IdentityPercent = 0.3
            m_Filtered = True
            m_HSP = New HSPValues
        End Sub
        Public Sub New(Hit As Bio.Web.Blast.Hit)
            m_IdentityCount = 20
            m_IdentityPercent = 0.3
            m_Filtered = True
            m_ExtendedHit = Hit
            m_HSP = New HSPValues
            DoHSP()
        End Sub
        Public Sub New(ByVal IdentityCount As Integer, ByVal IdentityPercent As Double)
            m_IdentityCount = IdentityCount
            m_IdentityPercent = IdentityPercent
            m_Filtered = True
            m_HSP = New HSPValues
        End Sub

        Private m_ExtendedHit As Blast.Hit
        Public Property Hit() As Hit
            Get
                Return m_ExtendedHit
            End Get
            Set(ByVal value As Hit)
                m_ExtendedHit = value
                DoHSP()
            End Set
        End Property

        Private m_QueryDef As String
        Public Property QueryDef() As String
            Get
                Return m_QueryDef
            End Get
            Set(ByVal value As String)
                m_QueryDef = value
            End Set
        End Property

        Public FullQueryDef As String

        Private m_CDS As ExtendedCDS
        Public Property CDS() As ExtendedCDS
            Get
                Return m_CDS
            End Get
            Set(ByVal value As ExtendedCDS)
                m_CDS = value
            End Set
        End Property

        Private m_HSP As HSPValues
        Public ReadOnly Property HSP() As HSPValues
            Get
                Return m_HSP
            End Get
        End Property

        Private m_Filtered As Boolean
        Public ReadOnly Property Filtered As Boolean
            Get
                Return m_Filtered
            End Get
        End Property


        Private Function Filter(IdentityCount As Integer, IdentityPercent As Double) As Boolean
            Dim Response As Boolean = True
            For Each HSP As Hsp In m_ExtendedHit.Hsps
                If HSP.IdentitiesCount >= IdentityCount Then
                    If HSP.IdentitiesCount / HSP.AlignmentLength >= IdentityPercent Then
                        Response = False
                        Exit For
                    End If
                End If
            Next
            Return Response
        End Function


        Public TotalHitLength As Integer

        Public Sub DoHSP()

            For Each HSP As Hsp In m_ExtendedHit.Hsps
                If HSP.HitFrame < 0 Then
                    Dim x As Integer = m_HSP.HitStart
                    m_HSP.HitStart = m_HSP.HitEnd
                    m_HSP.HitEnd = x
                End If
                If m_HSP.HitStart > HSP.HitStart Or m_HSP.HitStart = -1 Then
                    m_HSP.HitStart = HSP.HitStart
                End If
                If m_HSP.QueryStart > HSP.QueryStart Or m_HSP.QueryStart = -1 Then
                    m_HSP.QueryStart = HSP.QueryStart
                End If
                If m_HSP.HitEnd < HSP.HitEnd Or m_HSP.HitEnd = -1 Then
                    m_HSP.HitEnd = HSP.HitEnd
                End If
                If m_HSP.QueryEnd < HSP.QueryEnd Or m_HSP.QueryEnd = -1 Then
                    m_HSP.QueryEnd = HSP.QueryEnd
                End If
                If HSP.IdentitiesCount >= m_IdentityCount Then
                    If HSP.IdentitiesCount / HSP.AlignmentLength >= m_IdentityPercent Then
                        m_Filtered = False
                    End If
                End If
                m_HSP.Identites += HSP.IdentitiesCount

                TotalHitLength += Math.Abs(m_HSP.HitEnd - m_HSP.HitStart)
                If TotalHitLength < 0 Then
                    Dim alf As Int16 = 43
                End If
            Next
        End Sub

        Public Shared HitStartComparsion As Comparison(Of ExtendedHit) = Function(p1 As ExtendedHit, p2 As ExtendedHit) p1.HSP.HitStart.CompareTo(p2.HSP.HitStart)
        Public Shared QueryStartComparsion As Comparison(Of ExtendedHit) = Function(p1 As ExtendedHit, p2 As ExtendedHit) p1.HSP.QueryStart.CompareTo(p2.HSP.QueryStart)
        Public Shared CompQueryDef As Comparison(Of ExtendedHit) = Function(p1 As ExtendedHit, p2 As ExtendedHit) p1.QueryDef.CompareTo(p2.QueryDef)
        Public Shared ReOrderedPositionComparsion As Comparison(Of ExtendedHit) = Function(p1 As ExtendedHit, p2 As ExtendedHit) p1.CDS.ReOrderedPositon.CompareTo(p2.CDS.ReOrderedPositon)
        Public Shared RefGenomeIndexReOrderedPositionComaprsion As Comparison(Of ExtendedHit) = AddressOf RefGenomeIndexReOrderedPositionSort
        Public Shared FullQueryDefComparsion As Comparison(Of ExtendedHit) = Function(p1 As ExtendedHit, p2 As ExtendedHit) p1.FullQueryDef.CompareTo(p2.FullQueryDef)
        Public Shared HitDefHitStarComaprsion As Comparison(Of ExtendedHit) = AddressOf HitDefHitStart


        Private Shared Function RefGenomeIndexReOrderedPositionSort(p1 As ExtendedHit, p2 As ExtendedHit) As Integer
            If p1.CDS.RefGenomeIndex = p2.CDS.RefGenomeIndex Then
                Return p1.CDS.ReOrderedPositon.CompareTo(p2.CDS.ReOrderedPositon)
            Else
                Return p1.CDS.RefGenomeIndex.CompareTo(p2.CDS.RefGenomeIndex)
            End If
        End Function

        Public Function CompareTo(ByVal other As ExtendedHit) As Integer Implements System.IComparable(Of ExtendedHit).CompareTo
            If Comparsion = FullQueryDef Then
                Return FullQueryDef.CompareTo(other.FullQueryDef)
            End If
            Return -1
        End Function

        Private Shared Function HitDefHitStart(p1 As ExtendedHit, p2 As ExtendedHit) As Integer
            If p1.Hit.Def = p2.Hit.Def Then
                Return p1.HSP.HitStart.CompareTo(p2.HSP.HitStart)
            Else
                Return p1.Hit.Def.CompareTo(p2.Hit.Def)
            End If
        End Function

    End Class

    <Serializable()>
    Public Class HSPValues
        Public Sub New()
            m_HitStart = -1
            m_QueryStart = -1
            m_HitEnd = -1
            m_QueryEnd = -1
        End Sub
        Private m_QueryStart As New Integer
        Public Property QueryStart() As Integer
            Get
                Return m_QueryStart
            End Get
            Set(ByVal value As Integer)
                m_QueryStart = value
            End Set
        End Property

        Private m_QueryEnd As New Integer
        Public Property QueryEnd() As Integer
            Get
                Return m_QueryEnd
            End Get
            Set(ByVal value As Integer)
                m_QueryEnd = value
            End Set
        End Property

        Private m_HitStart As New Integer
        Public Property HitStart() As Integer
            Get
                Return m_HitStart
            End Get
            Set(ByVal value As Integer)
                m_HitStart = value
            End Set
        End Property

        Private m_HitEnd As New Integer
        Public Property HitEnd() As Integer
            Get
                Return m_HitEnd
            End Get
            Set(ByVal value As Integer)
                m_HitEnd = value
            End Set
        End Property

        Private m_Identities As New Integer
        Public Property Identites() As Integer
            Get
                Return m_Identities
            End Get
            Set(ByVal value As Integer)
                m_Identities = value
            End Set
        End Property
    End Class

    <Serializable()>
    Public Class ExtendedCDS

        Implements IComparable(Of ExtendedCDS)

        Public Sub New()
            m_Hits = New List(Of ExtendedHit)()
        End Sub

#Region "Properties"
        Public Source As String
        Public CurrComparsion As String
        Public Difference As Double
        Public GeneID As Integer
        Private m_Position As Integer

        Public Definition As String = ""
        Public Property Position() As Integer
            Get
                Return m_Position
            End Get
            Set(ByVal value As Integer)
                m_Position = value
            End Set
        End Property


        Private m_ReOrderedPosition As Integer
        Public Property ReOrderedPositon() As Integer
            Get
                Return m_ReOrderedPosition
            End Get
            Set(ByVal value As Integer)
                m_ReOrderedPosition = value
            End Set
        End Property


        Private m_RefGenomIndex As Integer
        Public Property RefGenomeIndex() As Integer
            Get
                Return m_RefGenomIndex
            End Get
            Set(ByVal value As Integer)
                m_RefGenomIndex = value
            End Set
        End Property

        Private m_RefGenomeName As String
        Public Property RefGenomeName() As String
            Get
                Return m_RefGenomeName
            End Get
            Set(ByVal value As String)
                m_RefGenomeName = value
            End Set
        End Property

        Private m_CDS As CodingSequence = Nothing
        ' Private m_CDS As Bio.IO.GenBank.CodingSequence
        Public Property CDS() As CodingSequence
            Get
                Return m_CDS
            End Get
            Set(ByVal value As CodingSequence)
                m_CDS = value
                GetGIFromCDS(m_CDS)
            End Set
        End Property

        ''' <summary>
        ''' The list of BlastSearchRecords in the document.
        ''' </summary>
        Public Property Hits() As List(Of ExtendedHit)
            Get
                Return m_Hits
            End Get
            Set(ByVal value As List(Of ExtendedHit))
                m_Hits = value
                If m_Hits.Count > 0 Then
                    m_HasHit = True
                End If
            End Set
        End Property
        Private m_Hits As List(Of ExtendedHit)

        Public Property GI() As Integer
            Get
                Return m_GI
            End Get
            Set(ByVal value As Integer)
                m_GI = value
            End Set
        End Property
        Private m_GI As Integer

        Private m_Islands As List(Of Island)
        Public Property Islands() As List(Of Island)
            Get
                Return m_Islands
            End Get
            Set(ByVal value As List(Of Island))
                m_Islands = value
            End Set
        End Property

        Private m_HasHit As Boolean
        Public Property HasHit() As Boolean
            Get
                Return m_HasHit
            End Get
            Set(ByVal value As Boolean)
                m_HasHit = value
            End Set
        End Property

#End Region
#Region "Sort"
        '    Public Shadows IslandScoreComaprsion As Comparison(Of ExtendedCDS ) = Function (p1 As ExtendedCDS ,p2 As ExtendedCDS ) p1.Islands .
        Public Shared GeneLocusTagComparison As Comparison(Of ExtendedCDS) = _
            Function(p1 As ExtendedCDS, p2 As ExtendedCDS)
                Dim s1 As New List(Of String)
                s1.Add("")
                If p1.CDS.LocusTag.Count = 0 Then
                    Return -1
                End If
                If p2.CDS.LocusTag.Count = 0 Then
                    Return -1
                End If
                Return p1.CDS.LocusTag(0).CompareTo(p2.CDS.LocusTag(0))

            End Function
        Public Shared GeneSymbolComparsion As Comparison(Of ExtendedCDS) = _
            Function(p1 As ExtendedCDS, p2 As ExtendedCDS)

                Return p1.CDS.GeneSymbol.CompareTo(p2.CDS.GeneSymbol)

            End Function
        '                     p1.CDS.LocusTag(0).CompareTo(p2.CDS.LocusTag(0))
        Public Shared GeneIDComparison As Comparison(Of ExtendedCDS) = Function(p1 As ExtendedCDS, p2 As ExtendedCDS) p1.GeneID.CompareTo(p2.GeneID)
        Public Shared GIComparison As Comparison(Of ExtendedCDS) = Function(p1 As ExtendedCDS, p2 As ExtendedCDS) p1.GI.CompareTo(p2.GI)
        Public Shared PositionComparison As Comparison(Of ExtendedCDS) = Function(p1 As ExtendedCDS, p2 As ExtendedCDS) p1.Position.CompareTo(p2.Position)
        Public Shared DifferenceComparison As Comparison(Of ExtendedCDS) = Function(p1 As ExtendedCDS, p2 As ExtendedCDS) p1.Difference.CompareTo(p2.Difference)
        Public Shared DefinitionComparison As Comparison(Of ExtendedCDS) = Function(p1 As ExtendedCDS, p2 As ExtendedCDS) p1.Definition.CompareTo(p2.Definition)


        Public Function CompareTo(ByVal other As ExtendedCDS) As Integer Implements System.IComparable(Of ExtendedCDS).CompareTo
            If other.CurrComparsion = Constants.LocusTag Then
                Dim s1 As New List(Of String)
                s1.Add("")
                If m_CDS.LocusTag.Count = 0 Then
                    Return -1
                End If
                If other.CDS.LocusTag.Count = 0 Then
                    Return -1
                End If
                Return m_CDS.LocusTag(0).CompareTo(other.CDS.LocusTag(0))
            ElseIf other.CurrComparsion = Constants.GeneID Then
                Return GeneID.CompareTo(other.GeneID)
            ElseIf other.CurrComparsion = Constants.GeneSymbol Then
                Return m_CDS.GeneSymbol.CompareTo(other.CDS.GeneSymbol)
            ElseIf other.CurrComparsion = Constants.Definition Then
                Return Definition.CompareTo(other.Definition)
            ElseIf other.CurrComparsion = Constants.GI Then
                Return GI.CompareTo(other.GI)
            End If
            Return -1
        End Function

#End Region
#Region "Methods"
        Private Sub GetGIFromCDS(ByVal CDS As CodingSequence)
            For Each g In CDS.DatabaseCrossReference
                If g.StartsWith(Chr(34) & "GI:", StringComparison.OrdinalIgnoreCase) Then
                    g = g.Replace(Chr(34), "")

                    m_GI = g.Remove(0, 3)
                    Exit Sub
                End If
            Next
            m_GI = 0
        End Sub
#End Region
    End Class

    <Serializable()>
    Public Class Island
        Implements IComparable(Of Island)


        Private m_IslandItems As New List(Of IslandItem)
        Public Property IslandItems() As List(Of IslandItem)
            Get
                Return m_IslandItems
            End Get
            Set(ByVal value As List(Of IslandItem))
                m_IslandItems = value
            End Set
        End Property

        Public Sub New()
            m_IslandStart = -1
            m_IslandEnd = -1
        End Sub
        Public Sub New(ByVal IslandStart As Integer, ByVal IslandEnd As Integer)
            m_IslandStart = IslandStart
            m_IslandEnd = IslandEnd
            SetLength()
        End Sub
        Private m_ContigName As String
        Public Property ContigName() As String
            Get
                Return m_ContigName
            End Get
            Set(ByVal value As String)
                m_ContigName = value
            End Set
        End Property

        ''' <summary>
        ''' Delete All Island and set Integer parameters to -1 
        ''' </summary>
        Public Sub Clear()
            m_IslandItems.Clear()
            m_IslandEnd = -1
            m_IslandStart = -1
            m_IslandLength = -1
        End Sub
        Public Function Clone()
            Return DirectCast(MemberwiseClone(), Island)
        End Function

        Private Sub SetLength()
            If m_IslandStart > -1 And m_IslandEnd > -1 Then
                If m_IslandEnd >= m_IslandStart Then
                    m_IslandLength = m_IslandEnd - m_IslandStart + 1
                Else
                    MsgBox("End is smaller then Start:")
                End If
            End If
        End Sub
        Private m_IslandStart As Integer
        Public Property IslandStart() As Integer
            Get
                Return m_IslandStart
            End Get
            Set(ByVal value As Integer)
                m_IslandStart = value
                SetLength()
            End Set
        End Property

        Private m_RefGenomIndex As Integer
        Public Property RefGenomeIndex() As Integer
            Get
                Return m_RefGenomIndex
            End Get
            Set(ByVal value As Integer)
                m_RefGenomIndex = value
            End Set
        End Property

        Private m_IslandStartOriginal As Integer
        Public Property IslandStartOriginal() As Integer
            Get
                Return m_IslandStartOriginal
            End Get
            Set(ByVal value As Integer)
                m_IslandStartOriginal = value
            End Set
        End Property

        Private m_IslandEnd As Integer
        Public Property IslandEnd() As Integer
            Get
                Return m_IslandEnd
            End Get
            Set(ByVal value As Integer)
                m_IslandEnd = value
                SetLength()
            End Set
        End Property

        Private m_IslandLength As Integer
        Public Property IslandLength() As Integer
            Get
                Return m_IslandLength
            End Get
            Set(ByVal value As Integer)
                m_IslandLength = value
            End Set
        End Property

        Public Function CompareTo(ByVal other As Island) As Integer Implements System.IComparable(Of Island).CompareTo
            Return m_IslandLength.CompareTo(other.IslandLength)
        End Function
    End Class

    <Serializable()>
    Public Class IslandItem
        Public Sub New()

        End Sub

        Public Function Clone()
            Return DirectCast(MemberwiseClone(), IslandItem)
        End Function
        Private m_ContigStartBp As Integer
        Public Property ContigStartBP() As Integer
            Get
                Return m_ContigStartBp
            End Get
            Set(ByVal value As Integer)
                m_ContigStartBp = value
            End Set
        End Property

        Private m_ContigEndBp As Integer
        Public Property ContigEndBp() As Integer
            Get
                Return m_ContigEndBp
            End Get
            Set(ByVal value As Integer)
                m_ContigEndBp = value
            End Set
        End Property

        Private m_GI As Integer
        Public Property GI() As Integer
            Get
                Return m_GI
            End Get
            Set(ByVal value As Integer)
                m_GI = value
            End Set
        End Property

        Public Property RefGenomeIndex As Integer
    End Class


End Namespace

