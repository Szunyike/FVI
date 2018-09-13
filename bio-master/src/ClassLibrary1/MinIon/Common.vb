Imports System.IO
Imports HDF5DotNet
Imports System
Imports ICSharpCode.SharpZipLib.Tar
Imports ICSharpCode.SharpZipLib.GZip

Namespace Szunyi.MinIon
    Public Class ToTar_FastQ
        Public Sub New()
            Dim File = Szunyi.IO.Files.Filter.SelectFile(Szunyi.Constants.Files.All_TAB_Like)
            If IsNothing(File) = True Then Exit Sub
            Dim Original_Files = Get_Original_Files(File)
            Dim BamFIles = Szunyi.IO.Files.Filter.SelectFiles(Constants.Files.FastQ)

            Dim nofMMoved As Integer = 0
            Dim nofFailed As Integer = 0
            If IsNothing(BamFIles) = True Then Exit Sub


            For Each BamFIle In BamFIles
                Dim log_Found As New System.Text.StringBuilder
                Dim log_Not_Found As New System.Text.StringBuilder
                log_Found.Append(BamFIle.FullName).AppendLine()
                log_Not_Found.Append(BamFIle.FullName).AppendLine()
                Dim tarOutFn As New FileInfo(BamFIle.FullName & ".tar.gz")
                Dim outStream As Stream = tarOutFn.Create

                Dim gzoStream = New GZipOutputStream(outStream)
                Dim tarOutputStream As TarOutputStream = New TarOutputStream(gzoStream)
                gzoStream.SetLevel(1)

                Dim x As New ReadID_FileName()
                Dim c As New cReadID
                Dim ReadIDs = Szunyi.IO.Import.Sequence.IDs(BamFIle)
                ReadIDs = Szunyi.Text.General.GetFirstParts(ReadIDs, " ")
                ReadIDs = ReadIDs.Distinct.ToList
                For Each ReadID In ReadIDs
                    x.ReadID = ReadID
                    Dim Index = Original_Files.BinarySearch(x, c)
                    If Index > -1 Then
                        Dim oriFile = Original_Files(Index).File
                        Using inputStream As Stream = oriFile.OpenRead
                            Dim tarName = oriFile.Name
                            Dim Entry As TarEntry = TarEntry.CreateTarEntry(tarName)
                            Entry.Size = inputStream.Length
                            tarOutputStream.PutNextEntry(Entry)
                            nofMMoved += 1
                            Dim localBuffer As Byte() = New Byte(32767) {}
                            Do
                                Dim numRead = inputStream.Read(localBuffer, 0, localBuffer.Length)
                                If numRead <= 0 Then
                                    Exit Do
                                End If
                                tarOutputStream.Write(localBuffer, 0, numRead)
                            Loop

                        End Using
                        log_Found.Append(ReadID).AppendLine()
                        tarOutputStream.CloseEntry()
                    Else
                        nofFailed += 1
                        log_Not_Found.Append(ReadID).AppendLine()
                    End If
                Next
                tarOutputStream.Close()
                If log_Found.Length > 0 Then
                    Szunyi.IO.Export.SaveText(nofMMoved & vbCrLf & log_Found.ToString, Szunyi.IO.Files.Get_New_FileName.Append_Before_Extension_wNew_Extension(BamFIle, "_Found_.tsv"))
                End If
                If log_Not_Found.Length > 0 Then
                    Szunyi.IO.Export.SaveText(nofFailed & vbCrLf & log_Not_Found.ToString, Szunyi.IO.Files.Get_New_FileName.Append_Before_Extension_wNew_Extension(BamFIle, "_Not_Found_.tsv"))
                End If
            Next ' Next File

        End Sub
        Private Function Get_Original_Files(file As FileInfo) As List(Of ReadID_FileName)
            Dim out As New List(Of ReadID_FileName)
            For Each Line In Szunyi.IO.Import.Text.Parse(file)
                If Line = String.Empty Then
                    Dim kj As Int16 = 54
                Else
                    If Line.EndsWith("failed") = False Then
                        Dim x = New ReadID_FileName(Line)
                        If IsNothing(x.ReadID) = False AndAlso IsNothing(x.File) = False Then
                            out.Add(New ReadID_FileName(Line))
                        End If

                    Else
                        Dim kj As Int16 = 54
                    End If

                End If

            Next
            out.Sort(New cReadID)
            Return out
        End Function
    End Class

    Public Class ToTar
        Public Sub New()
            Dim File = Szunyi.IO.Files.Filter.SelectFile(Szunyi.Constants.Files.All_TAB_Like)
            If IsNothing(File) = True Then Exit Sub
            Dim Original_Files = Get_Original_Files(File)
            Dim BamFIles = Szunyi.IO.Files.Filter.SelectFiles(Constants.Files.SAM_BAM)

            Dim nofMMoved As Integer = 0
            Dim nofFailed As Integer = 0
            If IsNothing(BamFIles) = True Then Exit Sub

            ' Check
            For Each BamFIle In BamFIles
                Dim log_Found As New System.Text.StringBuilder
                Dim log_Not_Found As New System.Text.StringBuilder
                log_Found.Append(BamFIle.FullName).AppendLine()
                log_Not_Found.Append(BamFIle.FullName).AppendLine()
                Dim ReadIDs = Szunyi.BAM.Bam_Basic_IO.Import.Get_Sorted_Distinct_Read_IDs(BamFIle)
                Dim x As New ReadID_FileName()
                Dim c1 As New cReadID
                Dim Found As Integer = 0
                Dim N_Found As Integer = 0
                For Each ReadID In ReadIDs
                    x.ReadID = ReadID
                    Dim Index = Original_Files.BinarySearch(x, c1)
                    If Index > -1 Then
                        Found += 1
                    Else
                        log_Not_Found.Append(x.ReadID).AppendLine()
                        N_Found += 1
                    End If
                Next
                Dim jk As Int16 = 87
            Next
            '
            Dim res As New Dictionary(Of String, Integer)
            For Each Item In Original_Files
                Dim s = Split(Item.File.FullName, "\fast5").First
                If res.ContainsKey(s) = False Then res.Add(s, 0)
                res(s) += 1
            Next
            Dim str As New System.Text.StringBuilder
            For Each item In res
                str.Append(item.Key).Append(vbTab).Append(item.Value).AppendLine()
            Next
            For Each BamFIle In BamFIles
                Dim log_Found As New System.Text.StringBuilder
                Dim log_Not_Found As New System.Text.StringBuilder
                log_Found.Append(BamFIle.FullName).AppendLine()
                log_Not_Found.Append(BamFIle.FullName).AppendLine()
                Dim tarOutFn As New FileInfo(BamFIle.FullName & ".tar.gz")
                Dim outStream As Stream = tarOutFn.Create

                Dim gzoStream = New GZipOutputStream(outStream)
                Dim tarOutputStream As TarOutputStream = New TarOutputStream(gzoStream)
                gzoStream.SetLevel(1)

                Dim x As New ReadID_FileName()
                Dim c As New cReadID
                Dim ReadIDs = Szunyi.BAM.Bam_Basic_IO.Import.Get_Sorted_Distinct_Read_IDs(BamFIle)
                '
                For Each ReadID In ReadIDs
                    x.ReadID = ReadID
                    Dim Index = Original_Files.BinarySearch(x, c)
                    If Index > -1 Then
                        Dim oriFile = Original_Files(Index).File
                        Using inputStream As Stream = oriFile.OpenRead
                            Dim tarName = oriFile.Name
                            Dim Entry As TarEntry = TarEntry.CreateTarEntry(tarName)
                            Entry.Size = inputStream.Length
                            tarOutputStream.PutNextEntry(Entry)
                            nofMMoved += 1
                            Dim localBuffer As Byte() = New Byte(32767) {}
                            Do
                                Dim numRead = inputStream.Read(localBuffer, 0, localBuffer.Length)
                                If numRead <= 0 Then
                                    Exit Do
                                End If
                                tarOutputStream.Write(localBuffer, 0, numRead)
                            Loop

                        End Using
                        log_Found.Append(ReadID).AppendLine()
                        tarOutputStream.CloseEntry()
                    Else
                        nofFailed += 1
                        log_Not_Found.Append(ReadID).AppendLine()
                    End If
                Next
                tarOutputStream.Close()
                If log_Found.Length > 0 Then
                    Szunyi.IO.Export.SaveText(nofMMoved & vbCrLf & log_Found.ToString, Szunyi.IO.Files.Get_New_FileName.Append_Before_Extension_wNew_Extension(BamFIle, "_Found_.tsv"))
                End If
                If log_Not_Found.Length > 0 Then
                    Szunyi.IO.Export.SaveText(nofFailed & vbCrLf & log_Not_Found.ToString, Szunyi.IO.Files.Get_New_FileName.Append_Before_Extension_wNew_Extension(BamFIle, "_Not_Found_.tsv"))
                End If
            Next ' Next File

        End Sub
        Private Function Get_Original_Files(file As FileInfo) As List(Of ReadID_FileName)
            Dim out As New List(Of ReadID_FileName)
            For Each Line In Szunyi.IO.Import.Text.Parse(file)
                If Line = String.Empty Then
                    Dim kj As Int16 = 54
                Else
                    If Line.EndsWith("failed") = False Then
                        Dim x = New ReadID_FileName(Line)
                        If IsNothing(x.ReadID) = False AndAlso IsNothing(x.File) = False Then
                            out.Add(New ReadID_FileName(Line))
                        End If

                    Else
                        Dim kj As Int16 = 54
                    End If

                End If

            Next
            out.Sort(New cReadID)
            Return out
        End Function
    End Class

    Public Class cReadID
        Implements IComparer(Of ReadID_FileName)

        Public Function Compare(x As ReadID_FileName, y As ReadID_FileName) As Integer Implements IComparer(Of ReadID_FileName).Compare
            Return x.ReadID.CompareTo(y.ReadID)
        End Function
    End Class

    Public Class cReadID_FileName
        Implements IComparer(Of ReadID_FileName)

        Public Function Compare(x As ReadID_FileName, y As ReadID_FileName) As Integer Implements IComparer(Of ReadID_FileName).Compare
            Return x.File.Name.CompareTo(y.File.Name)
        End Function
    End Class

    Public Class ReadID_FileName
        Public Property ReadID As String
        Public Property File As FileInfo
        Public Sub New()

        End Sub
        Public Sub New(Line As String)
            Dim s = Split(Line, vbTab)
            Me.ReadID = s(1).Substring(0, 36)
            If s.First.Length < 256 Then
                Me.File = New FileInfo(s.First)
            End If
        End Sub

    End Class

    Public Class HasBeenGetReadIDs
        Public Sub New(FIleList As FileInfo, ReadID_File As FileInfo)
            Dim ReadIDs = Get_Original_Files(ReadID_File)
            If ReadIDs.Count = 0 Then Exit Sub
            ReadIDs = (From x In ReadIDs Order By x.File.Name Ascending).ToList

            Dim Fast5 = Get_Fast5(FIleList)

            Dim log As New System.Text.StringBuilder
            Dim c As New cReadID_FileName
            Dim Found As Long = 0, notFOund As Long = 0
            Using sw As New StreamWriter("D:\missing_ReadIDs.tdt", True)
                For Each Item In Fast5
                    Dim x As New ReadID_FileName
                    x.File = Item
                    Dim Index = ReadIDs.BinarySearch(x, c)
                    If Index > -1 Then
                        Found += 1
                    Else

                        Dim t = HDF5.Get_Raw_Reads(x.File)
                        If IsNothing(t) = False Then
                            sw.Write(x.File.FullName)
                            sw.Write(vbTab)
                            sw.Write(t("read_id"))
                            sw.WriteLine()
                        Else
                            log.Append(x.File.FullName).AppendLine()
                            notFOund += 1
                        End If


                    End If
                Next
            End Using
            MsgBox(notFOund)
            Dim f = Szunyi.IO.Files.Save.SelectSaveFile(Szunyi.Constants.Files.All_TAB_Like)
            Szunyi.IO.Export.SaveText(log.ToString, f)

            Dim kj As Int16 = 54
        End Sub

        Private Function Get_Fast5(File As FileInfo) As List(Of FileInfo)
            Dim out As New List(Of FileInfo)
            For Each Line In Szunyi.IO.Import.Text.Parse(File)
                Dim f As New FileInfo(Line.Replace(Chr(34), "").Replace(",", "\"))
                out.Add(f)
            Next
            Dim res = (From x In out Order By x.Name Ascending).ToList
            Return res
        End Function

        Private Function Get_Original_Files(file As FileInfo) As List(Of ReadID_FileName)
            Dim out As New List(Of ReadID_FileName)
            For Each Line In Szunyi.IO.Import.Text.Parse(file)
                If Line = String.Empty Then
                    Dim kj As Int16 = 54
                Else
                    If Line.EndsWith("failed") = False Then
                        Dim x = New ReadID_FileName(Line)
                        If IsNothing(x.ReadID) = False AndAlso IsNothing(x.File) = False Then
                            out.Add(New ReadID_FileName(Line))
                        End If

                    Else
                        Dim kj As Int16 = 54
                    End If

                End If

            Next
            out.Sort(New cReadID)
            Return out
        End Function
    End Class

    Public Class File_wDIr
        Public Property ch As Integer
        Public Property read As Integer
        Public Property Flow_Cell As String
        Public Property FileName As String
        Public Property DirName As String
        Public Property IsMuxScan As Boolean = False
        Public Sub New(line As String)
            line = line.Replace(Chr(34), "")
            If line.Contains("mux_scan") Then
                Me.IsMuxScan = True
            End If
            Dim s = Split(line, ",")

            Me.FileName = s.Last
            Me.DirName = s.First

        End Sub

    End Class

    Public Class cFile_wDIr
        Implements IComparer(Of File_wDIr)

        Public Function Compare(x As File_wDIr, y As File_wDIr) As Integer Implements IComparer(Of File_wDIr).Compare
            Return x.FileName.CompareTo(y.FileName)
        End Function
    End Class

    Public Class Common
        Public Shared Function Get_First_Passed_fast5_Files(Dir As DirectoryInfo) As List(Of FileInfo)
            Dim out As New List(Of FileInfo)
            For Each d In Dir.GetDirectories
                For Each sDir In d.GetDirectories("fast5")

                    For Each passDir In sDir.GetDirectories("pass")
                        For Each FirstDIr In passDir.GetDirectories("0")
                            out.Add(FirstDIr.GetFiles.First)
                            Exit For
                        Next
                        Exit For
                    Next
                    Exit For
                Next
            Next
            Return out
        End Function

        Public Shared Function Get_RunID_From_Sam_File(FIle As FileInfo) As String

        End Function

        Public Shared Function Get_RunID_From_FastQ_File(file As FileInfo) As String
            Return Split(file.Name, "_")(2)
        End Function
        ''' <summary>
        ''' Search string after "runid=" " before " "
        ''' </summary>
        ''' <param name="iD"></param>
        ''' <returns></returns>
        Public Shared Function Get_RunID_From_SeqID(iD As String) As String
            Dim s = Split(iD, "runid=").Last
            Return Split(s, " ").First

        End Function
    End Class

    Public Class Bams_Fastq_RunID
        Public Sub New()
            Dim Bam_Files = Szunyi.IO.Files.Filter.SelectFiles(Szunyi.Constants.Files.SAM_BAM)
            If IsNothing(Bam_Files) = True Then Exit Sub
            Dim Fastq_Files = Szunyi.IO.Files.Filter.SelectFiles(Szunyi.Constants.Files.Fasta_FastQ)
            If IsNothing(Fastq_Files) = True Then Exit Sub
            Dim ls As New List(Of ReadID_RunID)

            For Each Seq In Szunyi.IO.Import.Sequence.From_Files_Iterator(Fastq_Files)
                ls.Add(New ReadID_RunID(Seq))
            Next

            Dim c As New cReadID_RunID_ByReadID
            ls.Sort(c)
            Dim log As New System.Text.StringBuilder
            For Each Bam_File In Bam_Files
                log.Append(Bam_File.FullName).AppendLine()
                Dim tmp As ReadID_RunID = ls.First
                Dim sg As New Dictionary(Of String, Integer)
                For Each SAM In Szunyi.BAM.Bam_Basic_IO.Import.Parse(Bam_File)
                    tmp.ReadID = SAM.QName
                    Dim Index = ls.BinarySearch(tmp, c)
                    If Index > -1 Then
                        If sg.ContainsKey(ls(Index).RunID) = False Then sg.Add(ls(Index).RunID, 0)
                        sg(ls(Index).RunID) += 1
                    End If
                Next
                Dim kj As Int16 = 54
            Next
        End Sub
    End Class

    Public Class ReadID_RunID
        Public Property ReadID As String
        Public Property RunID As String

        Public Sub New(Seq As Bio.ISequence)
            RunID = Common.Get_RunID_From_SeqID(Seq.ID)
            ReadID = Split(Seq.ID, " ").First
        End Sub

    End Class
    Public Class cReadID_RunID_ByReadID
        Implements IComparer(Of ReadID_RunID)

        Public Function Compare(x As ReadID_RunID, y As ReadID_RunID) As Integer Implements IComparer(Of ReadID_RunID).Compare
            Return x.ReadID.CompareTo(y.ReadID)
        End Function
    End Class

    Public Class HDF5
        Public Property File As FileInfo
        Public Shared Function Get_Contexts_And_Trackings(File As FileInfo) As Dictionary(Of String, String)
            Dim FileID As H5FileOrGroupId = HDF5DotNet.H5F.open(File.FullName, H5F.OpenMode.ACC_RDONLY)
            Dim kk = H5G.open(FileID, "/Raw/Reads")
            Dim l = H5G.getInfo(kk)
            Dim g1 = H5G.open(FileID, "UniqueGlobalKey/tracking_id")
            Dim out As New Dictionary(Of String, String)
            For i1 = 0 To H5A.getNumberOfAttributes(g1) - 1
                Dim Attr_ID = H5A.openIndex(g1, i1)
                Dim dType = H5A.getType(Attr_ID)

                Dim tmp(H5A.getInfo(Attr_ID).dataSize - 1) As Byte
                Dim Buffer As New H5Array(Of Byte)(tmp)
                H5A.read(Of Byte)(Attr_ID, dType, Buffer)

                Dim enc As New System.Text.ASCIIEncoding
                Dim Value = enc.GetString(tmp).Replace(vbNullChar, "")

                out.Add(H5A.getName(Attr_ID), Value)

            Next
            Dim g2 = H5G.open(FileID, "UniqueGlobalKey/context_tags")

            For i1 = 0 To H5A.getNumberOfAttributes(g2) - 1
                Dim Attr_ID = H5A.openIndex(g2, i1)
                Dim dType = H5A.getType(Attr_ID)

                Dim tmp(H5A.getInfo(Attr_ID).dataSize - 1) As Byte
                Dim Buffer As New H5Array(Of Byte)(tmp)
                H5A.read(Of Byte)(Attr_ID, dType, Buffer)

                Dim enc As New System.Text.ASCIIEncoding
                Dim Value = enc.GetString(tmp).Replace(vbNullChar, "")

                out.Add(H5A.getName(Attr_ID), Value)

            Next
            Return out

        End Function
        ''' <summary>
        ''' Get Info From Attributes
        ''' </summary>
        ''' <param name="File"></param>
        ''' <returns></returns>
        Public Shared Function Get_Context_Tags(File As FileInfo) As Dictionary(Of String, String)
            Dim FileID As H5FileOrGroupId = HDF5DotNet.H5F.open(File.FullName, H5F.OpenMode.ACC_RDONLY)
            Dim kk = H5G.open(FileID, "UniqueGlobalKey")
            Dim g1 = H5G.open(FileID, "UniqueGlobalKey/context_tags")
            Dim out As New Dictionary(Of String, String)
            For i1 = 0 To H5A.getNumberOfAttributes(g1) - 1
                Dim Attr_ID = H5A.openIndex(g1, i1)
                Dim dType = H5A.getType(Attr_ID)

                Dim tmp(H5A.getInfo(Attr_ID).dataSize - 1) As Byte
                Dim Buffer As New H5Array(Of Byte)(tmp)
                H5A.read(Of Byte)(Attr_ID, dType, Buffer)

                Dim enc As New System.Text.ASCIIEncoding
                Dim Value = enc.GetString(tmp).Replace(vbNullChar, "")

                out.Add(H5A.getName(Attr_ID), Value)

            Next
            Return out
        End Function
        ''' <summary>
        ''' Get Info From Attributes Tracking ID
        ''' </summary>
        ''' <param name="File"></param>
        ''' <returns></returns>
        Public Shared Function Get_TrackingID_Tags(File As FileInfo) As Dictionary(Of String, String)
            Dim FileID As H5FileOrGroupId = HDF5DotNet.H5F.open(File.FullName, H5F.OpenMode.ACC_RDONLY)
            Dim kk = H5G.open(FileID, "UniqueGlobalKey")
            Dim g1 = H5G.open(FileID, "UniqueGlobalKey/tracking_id")
            Dim out As New Dictionary(Of String, String)
            For i1 = 0 To H5A.getNumberOfAttributes(g1) - 1
                Dim Attr_ID = H5A.openIndex(g1, i1)
                Dim dType = H5A.getType(Attr_ID)

                Dim tmp(H5A.getInfo(Attr_ID).dataSize - 1) As Byte
                Dim Buffer As New H5Array(Of Byte)(tmp)
                H5A.read(Of Byte)(Attr_ID, dType, Buffer)

                Dim enc As New System.Text.ASCIIEncoding
                Dim Value = enc.GetString(tmp).Replace(vbNullChar, "")

                out.Add(H5A.getName(Attr_ID), Value)

            Next
            Return out
        End Function


        Public Shared Function Get_Attributes_with_Values(FIle As FileInfo, Group_Name As String) As Dictionary(Of String, String)

            Dim FileID As H5FileOrGroupId = HDF5DotNet.H5F.open(FIle.FullName, H5F.OpenMode.ACC_RDONLY)

            Dim kk = H5G.open(FileID, "UniqueGlobalKey")
            Dim g1 = H5G.open(FileID, "UniqueGlobalKey/context_tags")

            Dim out As New Dictionary(Of String, String)
            For i1 = 0 To H5A.getNumberOfAttributes(g1) - 1
                Dim Attr_ID = H5A.openIndex(g1, i1)
                Dim dType = H5A.getType(Attr_ID)

                Dim jk1 = H5A.getInfo(Attr_ID)

                Dim tmp(jk1.dataSize - 1) As Byte
                Dim Buffer As New H5Array(Of Byte)(tmp)
                H5A.read(Of Byte)(Attr_ID, dType, Buffer)

                Dim enc As New System.Text.ASCIIEncoding
                Dim rres = enc.GetString(tmp).Replace(vbNullChar, "")
                Dim s As String = H5A.getName(Attr_ID)
                out.Add(s, rres)


            Next
            Return out
        End Function

        Public Shared Function Get_Raw_Reads(File As FileInfo) As Dictionary(Of String, String)
            Try

                Dim f5 As H5FileOrGroupId = HDF5DotNet.H5F.open(File.FullName, H5F.OpenMode.ACC_RDONLY)

                Dim g1 = H5G.open(f5, "Raw/Reads")
                Dim ghhh = H5G.getObjectNameByIndex(g1, 0)
                Dim G2 = H5G.open(f5, "Raw/Reads/" & ghhh)
                Dim out As New Dictionary(Of String, String)
                For i1 = 0 To H5A.getNumberOfAttributes(G2) - 1
                    Dim Attr_ID = H5A.openIndex(G2, i1)
                    Dim dType = H5A.getType(Attr_ID)

                    Dim jk1 = H5A.getInfo(Attr_ID)

                    Dim tmp(jk1.dataSize - 1) As Byte
                    Dim Buffer As New H5Array(Of Byte)(tmp)
                    H5A.read(Of Byte)(Attr_ID, dType, Buffer)

                    Dim enc As New System.Text.ASCIIEncoding
                    Dim rres = enc.GetString(tmp).Replace(vbNullChar, "")
                    Dim s As String = H5A.getName(Attr_ID)
                    out.Add(s, rres)
                    H5A.close(Attr_ID)

                Next
                H5G.close(g1)
                H5G.close(G2)
                H5F.close(f5)


                Return out
            Catch ex As Exception
                Return Nothing
            End Try



        End Function
    End Class

    Public Class SequencingSummary
        Public Class cSS_Item_ByReadID
            Implements IComparer(Of SSItem)

            Public Function Compare(x As SSItem, y As SSItem) As Integer Implements IComparer(Of SSItem).Compare

                Return x.ReadID.CompareTo(y.ReadID)

            End Function

        End Class
        Public Enum Type
            barcode = 1
            RunId = 2
            BarCode_RunID = 3
            Bam = 4
            All = 5
            FastQ = 6
        End Enum
        Public Class SSItem
            Public FileName As String
            Public ReadID As String
            Public RunID As String
            Public BarCode As String
            Public Sub New(line As String)
                Dim s = Split(line, vbTab)
                Me.FileName = s(0)
                Me.ReadID = s(1)
                Me.RunID = s(2)

            End Sub
            Public Sub New(line As String, Index_of_Barcode As Integer)
                Dim s = Split(line, vbTab)
                Me.FileName = s(0)
                Me.ReadID = s(1)
                Me.RunID = s(2)
                Me.BarCode = s(Index_of_Barcode)
            End Sub
            Public Sub New()

            End Sub
            Public Overrides Function ToString() As String
                Return Me.FileName & vbTab & Me.ReadID & vbTab & Me.RunID & vbTab & Me.BarCode
            End Function
        End Class

        Public Class Tar
            Implements IDisposable

            Dim gzoStream As GZipOutputStream
            Dim tarOutputStream As TarOutputStream
            Public Property NofMoved As Integer = 0
            Dim Log_Writter As StreamWriter
            Public Sub New(File As FileInfo)
                Dim tarOutFn As New FileInfo(File.FullName & ".tar")
                Dim outStream As Stream = tarOutFn.Create

                Dim NofMoved As Integer = 0

                tarOutputStream = New TarOutputStream(outStream)

                '       gzoStream.SetLevel(1)
                Log_Writter = New StreamWriter(Szunyi.IO.Files.Get_New_FileName.GetNewFile(File, ".log").FullName)
                Log_Writter.AutoFlush = False
            End Sub
            Public Sub Write(inputStream As Byte(), ReadID As String)


                Dim Entry As TarEntry = TarEntry.CreateTarEntry(ReadID)
                Entry.Size = inputStream.Length
                tarOutputStream.PutNextEntry(Entry)

                tarOutputStream.Write(inputStream, 0, inputStream.Length)
                NofMoved += 1


                tarOutputStream.CloseEntry()
            End Sub
            Public Sub Write(File As FileInfo, ReadID As String)
                Using inputStream As Stream = File.OpenRead
                    Dim lb(inputStream.Length - 1) As Byte
                    Dim nr = inputStream.Read(lb, 0, inputStream.Length)


                    Dim Entry As TarEntry = TarEntry.CreateTarEntry(ReadID)
                    Entry.Size = inputStream.Length
                    tarOutputStream.PutNextEntry(Entry)
                    NofMoved += 1
                    Dim localBuffer As Byte() = New Byte(32767) {}
                    Do
                        Dim numRead = inputStream.Read(localBuffer, 0, localBuffer.Length)
                        If numRead <= 0 Then
                            Exit Do
                        End If
                        tarOutputStream.Write(localBuffer, 0, numRead)
                    Loop

                End Using

                tarOutputStream.CloseEntry()
            End Sub
            Public Shared Function Get_buffer(file As FileInfo) As Byte()
                Using inputStream As Stream = file.OpenRead
                    Dim lb(inputStream.Length - 1) As Byte
                    Dim nr = inputStream.Read(lb, 0, inputStream.Length)
                    Return lb
                End Using


            End Function
            Public Sub Write_Log(log As String)
                Log_Writter.WriteAsync(log & vbCrLf)

            End Sub
#Region "IDisposable Support"
            Private disposedValue As Boolean ' To detect redundant calls

            ' IDisposable
            Protected Overridable Sub Dispose(disposing As Boolean)
                If Not disposedValue Then
                    If disposing Then
                        ' TODO: dispose managed state (managed objects).
                        tarOutputStream.Close()
                        Log_Writter.Flush()
                        Log_Writter.Close()
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
        Public Property MinIonReads As New List(Of SSItem)
        Public Property log As New System.Text.StringBuilder
        Public Property Files_InFolder As New List(Of FileInfo)
        Public Property Bam_FIles As New List(Of FileInfo)
        Dim c As New Szunyi.IO.Files.Sort.ByFileName
        Dim c2 As New Szunyi.MinIon.SequencingSummary.cSS_Item_ByReadID
        Public Property Nof_Found As Integer = 0
        Public Property Nof_Not_Found As Integer = 0
        Public Sub New(Type As SequencingSummary.Type)
            If Type = Type.Bam Then
                Bam_FIles = Szunyi.IO.Files.Filter.SelectFiles(Constants.Files.SAM_BAM)
                If IsNothing(Bam_FIles) = True Then Exit Sub
            ElseIf Type = Type.FastQ Then
                Bam_FIles = Szunyi.IO.Files.Filter.SelectFiles(Constants.Files.FastQ)
                If IsNothing(Bam_FIles) = True Then Exit Sub
            End If

            Dim SS_File = Szunyi.IO.Files.Filter.SelectFile("", "Select Sequencing Summary File")
            If IsNothing(SS_File) = True Then Exit Sub

            Dim Files_InFOlder = Szunyi.IO.Files.Filter.SelectFile(Constants.Files.csv, "Select File List file")
            If IsNothing(SS_File) = True Then Exit Sub

            Set_SS_Items(SS_File)
            Set_FilesInFolder(Files_InFOlder)

            Select Case Type
                Case Type.barcode, Type.All, Type.BarCode_RunID, Type.RunId
                    Dim Grs As IEnumerable(Of IEnumerable(Of SSItem)) = Get_SS_Groups(Type)
                    For Each Gr As IEnumerable(Of SSItem) In Grs
                        Using x = Get_TAR(SS_File, Type, Gr.First)
                            x.Write_Log("SS Items Count" & vbTab & Me.MinIonReads.Count)
                            x.Write_Log("Files Count" & vbTab & Me.Files_InFolder.Count)
                            Parallel.ForEach(Gr, Sub(Item As SSItem)
                                                     Dim Index = Me.Files_InFolder.BinarySearch(New FileInfo(Item.FileName), c)
                                                     Dim TheFIle = (Me.Files_InFolder(Index))
                                                     If Index > -1 Then
                                                         Dim b = Tar.Get_buffer(TheFIle)
                                                         SyncLock x
                                                             x.Write(b, Item.ReadID)
                                                         End SyncLock

                                                     Else

                                                     End If


                                                 End Sub)

                        End Using
                    Next


                Case SequencingSummary.Type.Bam Or SequencingSummary.Type.FastQ

                    Dim Read As New SSItem
                    For Each File In Bam_FIles
                        Using x As New Tar(Szunyi.IO.Files.Get_New_FileName.Append_Before_Extension(SS_File, File.Name))
                            Dim uniqeIDs = Get_Unique_ID(File, Type).ToList
                            For Each SS_Item In Me.MinIonReads
                                Dim Index = uniqeIDs.BinarySearch(SS_Item.ReadID)
                                Dim Item = Me.MinIonReads(Index)
                                If Index > -1 Then
                                    x.Write(Me.Files_InFolder(Index), Item.ReadID)
                                    Nof_Found += 1
                                Else
                                    Nof_Not_Found += 1
                                    x.Write_Log(Item.ToString)
                                End If

                            Next
                        End Using
                    Next

            End Select

        End Sub

        ''' <summary>
        ''' Sorted
        ''' </summary>
        ''' <param name="file"></param>
        ''' <param name="type"></param>
        ''' <returns></returns>
        Private Function Get_Unique_ID(file As FileInfo, type As Type) As IEnumerable(Of String)
            If type = Type.Bam Then
                Return Szunyi.BAM.Bam_Basic_IO.Import.Get_Sorted_Distinct_Read_IDs(file)
            ElseIf type = type.FastQ Then
                Dim IDs = Szunyi.IO.Import.Sequence.IDs(file)
                IDs = Szunyi.Text.General.GetFirstParts(IDs, " ")
                Return IDs.Distinct
            Else
                Return New List(Of String)
            End If
        End Function

        Private Iterator Function Get_SS_Groups(type As Type) As IEnumerable(Of IEnumerable(Of SSItem))
            Select Case type
                Case Type.All
                    Yield Me.MinIonReads
                Case Type.barcode
                    For Each Item In Me.SS_By_Barcode
                        Yield Item
                    Next
                Case Type.BarCode_RunID
                    For Each Item In Me.SS_By_Barcode_RunID
                        Yield Item
                    Next
                Case Type.RunId
                    For Each Item In Me.SS_By_RunID
                        Yield Item
                    Next
            End Select
        End Function

        Private Function Get_TAR(SS_File As FileInfo, type As Type, SS_Item As SSItem) As Tar
            Select Case type
                Case Type.All
                    Return New Tar(SS_File)
                Case Type.barcode
                    Return New Tar(Szunyi.IO.Files.Get_New_FileName.Append_Before_Extension(SS_File, "_" & SS_Item.BarCode))
                Case Type.BarCode_RunID
                    Return New Tar(Szunyi.IO.Files.Get_New_FileName.Append_Before_Extension(SS_File, "_" & SS_Item.BarCode & "_" & SS_Item.RunID))
                Case Type.RunId
                    Return New Tar(Szunyi.IO.Files.Get_New_FileName.Append_Before_Extension(SS_File, "_" & SS_Item.RunID))
            End Select
        End Function

        Private Sub Set_SS_Items(SS_FIle As FileInfo)
            Dim Header = Szunyi.IO.Import.Text.GetHeader(SS_FIle, 1)
            If Header.Contains("barcode_arrangement") Then
                Dim Index_of_BarCode = Header.IndexOf("barcode_arrangement")
                For Each Line In Szunyi.IO.Import.Text.ParseNotFirst(SS_FIle, 1)
                    MinIonReads.Add(New SSItem(Line, Index_of_BarCode))
                Next
            Else
                For Each Line In Szunyi.IO.Import.Text.ParseNotFirst(SS_FIle, 1)
                    MinIonReads.Add(New SSItem(Line))
                Next
            End If

        End Sub

        Private Sub Set_FilesInFolder(File_in_Folder)
            For Each Line In Szunyi.IO.Import.Text.Parse(File_in_Folder, 1)
                Files_InFolder.Add(New FileInfo(Line.Trim(Chr(34))))
            Next
            Me.Files_InFolder.Sort(c)
        End Sub
#Region "Iterators"
        Private Iterator Function SS_By_FileName() As IEnumerable(Of IEnumerable(Of SSItem))
            Dim gr = From x In Me.MinIonReads Group By x.FileName Into Group

            For Each g In gr
                Yield g.Group
            Next
        End Function
        Private Iterator Function SS_By_ReadID() As IEnumerable(Of IEnumerable(Of SSItem))
            Dim gr = From x In Me.MinIonReads Group By x.ReadID Into Group

            For Each g In gr
                Yield g.Group
            Next
        End Function
        Private Iterator Function SS_By_RunID() As IEnumerable(Of IEnumerable(Of SSItem))
            Dim gr = From x In Me.MinIonReads Group By x.RunID Into Group

            For Each g In gr
                Yield g.Group
            Next
        End Function
        Private Iterator Function SS_By_Barcode_RunID() As IEnumerable(Of IEnumerable(Of SSItem))
            Dim gr = From x In Me.MinIonReads Group By x.BarCode, x.RunID Into Group

            For Each g In gr
                Yield g.Group
            Next
        End Function
        Private Iterator Function SS_By_Barcode() As IEnumerable(Of IEnumerable(Of SSItem))
            Dim gr = From x In Me.MinIonReads Group By x.BarCode Into Group

            For Each g In gr
                Yield g.Group
            Next
        End Function
#End Region

    End Class
End Namespace

