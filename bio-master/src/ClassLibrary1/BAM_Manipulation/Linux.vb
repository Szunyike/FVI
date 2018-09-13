

Imports System.IO

Namespace Szunyi
    Namespace Linux
        Public Class SamTools
            Public Shared Function Extract_Regions(Files As IEnumerable(Of FileInfo), Region As String) As String
                Dim str As New System.Text.StringBuilder
                For Each File In Files
                    str.Append(Extract_Region(File, Region))
                Next
                Return str.ToString
            End Function
            Public Shared Function Extract_Regions(Files As IEnumerable(Of FileInfo), FIle As FileInfo) As String
                Dim str As New System.Text.StringBuilder
                Dim Lines = Szunyi.IO.Import.Text.ReadLines(FIle)
                For Each FIle In Files
                    For Each Line In Lines
                        str.Append(Extract_Region(FIle, Line)).AppendLine()
                    Next

                Next
                Return str.ToString
            End Function
            Public Shared Function Split_By_ReadGroups(Files As IEnumerable(Of FileInfo)) As String
                Dim str As New System.Text.StringBuilder
                For Each FIle In Files
                    Dim Header = Szunyi.BAM.Bam_Basic_IO.Headers.Get_Header(FIle)
                    For Each Item In Header.RecordFields
                        If Item.Typecode = "RG" Then
                            Dim F = Szunyi.IO.Linux.Get_FileName(FIle)
                            Dim nFile = Szunyi.IO.Files.Get_New_FileName.Append_Before_Extension(FIle, "_" & Item.Tags(1).Value)
                            Dim O = Szunyi.IO.Linux.Get_FileName(nFile)
                            str.Append("samtools view -h -r " & Item.Tags(0).Value & "  " & F & " > " & O).AppendLine()
                        End If
                    Next
                Next
                Return str.ToString
            End Function
            Public Shared Function Extract_Regions_BED(Files As IEnumerable(Of FileInfo), FIle As FileInfo) As String
                Dim str As New System.Text.StringBuilder
                Dim Lines = Szunyi.IO.Import.Text.ReadLines(FIle)
                For Each F In Files

                    str.Append(Extract_Region(F, FIle)).AppendLine()


                Next
                Return str.ToString
            End Function
            Public Shared Function Extract_Region(Bam_File As FileInfo, Bed_File As FileInfo) As String
                Dim str As New System.Text.StringBuilder
                Dim LinuxFIleName = Szunyi.IO.Linux.Get_FileName(Bam_File)
                Dim L_Bed_File = Szunyi.IO.Linux.Get_FileName(Bed_File)
                Dim nFIle = Szunyi.IO.Files.Get_New_FileName.Append_Before_Extension(Bam_File, Bed_File) 'As New FileInfo((Bam_File.FullName & Bed_File.Name & ".sam"))
                Dim LiNux_nFile = Szunyi.IO.Linux.Get_FileName(nFIle)
                str.Append("samtools view ")
                str.Append(" -L ").Append(L_Bed_File)
                str.Append(" -h ").Append(LinuxFIleName)

                str.Append(" > ")
                str.Append(LiNux_nFile)

                Return str.ToString
            End Function
            Public Shared Function Extract_Region(File As FileInfo, Region As String) As String
                Dim str As New System.Text.StringBuilder
                Dim LinuxFIleName = Szunyi.IO.Linux.Get_FileName(File)
                Dim nFIle As New FileInfo((File.FullName & Region.Replace(":", "-") & ".sam"))
                Dim LiNux_nFile = Szunyi.IO.Linux.Get_FileName(nFIle)
                str.Append("samtools view ")
                str.Append(" -h ").Append(LinuxFIleName)
                str.Append(" ").Append(Region)
                str.Append(" > ")
                str.Append(Szunyi.IO.Linux.Get_FileName(nFIle))

                Return str.ToString
            End Function

            Public Shared Function Sort_Index(Files As IEnumerable(Of FileInfo)) As String
                Dim str As New System.Text.StringBuilder
                For Each File In Files
                    str.Append(Sort_Index(File))
                Next
                Return str.ToString
            End Function
            Public Shared Function Sort_Index(File As FileInfo) As String
                Dim str As New System.Text.StringBuilder
                Dim LinuxFIleName = Szunyi.IO.Linux.Get_FileName(File)
                Dim SortedBamFileName = Szunyi.IO.Linux.Get_FileName(New FileInfo(File.FullName.Replace(".bam", ".sorted.bam")))
                str.Append("samtools sort " & LinuxFIleName & " > " & SortedBamFileName).Append(vbLf)
                str.Append("samtools index " & SortedBamFileName & "").Append(vbLf)


                Return str.ToString
            End Function
            Public Shared Function Sam_to_Bam_Sort_Index(Files As List(Of FileInfo)) As String
                Dim str As New System.Text.StringBuilder
                For Each File In Files
                    str.Append(Sam_to_Bam_Sort_Index(File))
                Next
                Return str.ToString
            End Function
            Public Shared Function Sam_to_Bam_Sort_Index(File As FileInfo) As String
                Dim str As New System.Text.StringBuilder
                Dim LinuxFIleName = Szunyi.IO.Linux.Get_FileName(File)
                Dim BamFIleName = Szunyi.IO.Linux.Get_FileName(New FileInfo(File.FullName.Replace(".sam", ".bam")))
                Dim SortedBamFileName = Szunyi.IO.Linux.Get_FileName(New FileInfo(File.FullName.Replace(".sam", ".sorted.bam")))
                str.Append("samtools view -bS " & LinuxFIleName & " | ")

                str.Append("samtools sort" & " > " & SortedBamFileName).Append(vbLf)
                str.Append("samtools index " & SortedBamFileName & "").Append(vbLf)

                Return str.ToString
            End Function

            Public Shared Function Bam_To_Sam(Files As IEnumerable(Of FileInfo)) As String
                Dim str As New System.Text.StringBuilder
                For Each file In Files
                    str.Append(Bam_To_sam(file))
                Next
                Return str.ToString
            End Function
            Public Shared Function Bam_To_sam(File As FileInfo) As String
                Dim LinuxFIleName = Szunyi.IO.Linux.Get_FileName(File)
                Dim BamFIleName = Szunyi.IO.Linux.Get_FileName(New FileInfo(File.FullName.Replace(".bam", ".sam")))
                Dim str As New System.Text.StringBuilder
                str.Append("samtools view -h ")
                str.Append(LinuxFIleName).Append(" > ")
                str.Append(BamFIleName).AppendLine()
                Return str.ToString
            End Function

            Public Shared Function Get_UnMapped_Reads(Files As List(Of FileInfo)) As String
                '  samtools view -b -f 4 216_5W_Ca1.bam > unmap_216_5W_Ca1.bam
                Dim str As New System.Text.StringBuilder
                For Each FIle In Files
                    Dim LinuxFIleName = Szunyi.IO.Linux.Get_FileName(FIle)
                    Dim nFIle = Szunyi.IO.Files.Get_New_FileName.Append_Before_Extension_wNew_Extension(FIle, "_UnMapped.bam")
                    Dim BamFIleName = Szunyi.IO.Linux.Get_FileName(nFIle)

                    str.Append("samtools view -b -f 4 ")
                    str.Append(LinuxFIleName).Append(" > ")
                    str.Append(BamFIleName).AppendLine()
                Next

                Return str.ToString
            End Function
        End Class
        Public Class Bowtie2
            Public Shared Function Get_Index(File As FileInfo, dbName As String) As String
                Dim str As New System.Text.StringBuilder
                str.Append("bowtie2-build ")
                str.Append(Szunyi.IO.Linux.Get_FileName(File))
                str.Append(" ").Append(dbName)
                str.AppendLine()
                Return str.ToString
            End Function
            Public Shared Function Get_Index(Files As List(Of FileInfo), dbName As String) As String
                Dim str As New System.Text.StringBuilder
                For Each File In Files
                    str.Append(Get_Index(File, dbName))
                Next
                Return str.ToString
            End Function

            Public Shared Function Get_Only_Aligned(DBs As List(Of String), files As List(Of FileInfo)) As String
                Dim str As New System.Text.StringBuilder
                For Each db In DBs
                    For Each file In files
                        str.Append("bowtie2 -x ").Append(db)
                        str.Append(" -U ").Append(Szunyi.IO.Linux.Get_FileName(file))
                        str.Append(" --no-unal -p ").Append(System.Environment.ProcessorCount - 1)
                        Dim nFile = Szunyi.IO.Files.Get_New_FileName.Append_Before_Extension_wNew_Extension(file, ".sam")
                        str.Append(" -S ").Append(Szunyi.IO.Linux.Get_FileName(nFile))

                        str.AppendLine()
                    Next
                Next
                Return str.ToString
            End Function
            Public Shared Function Paired_End(DBs As List(Of String), Files As List(Of FileInfo)) As String
                Dim str As New System.Text.StringBuilder
                Dim f = From x In Files Where x.Name.Contains("_1")
                Dim l = From x In Files Where x.Name.Contains("_2")
                Dim exp_Name = Split(Files.First.Name, "_").First
                If f.Count = 1 And l.Count = 1 Then
                    For Each db In DBs

                        str.Append("bowtie2 -x ").Append(db)
                        str.Append(" -1 ").Append(Szunyi.IO.Linux.Get_FileName(f.First))
                        str.Append(" -2 ").Append(Szunyi.IO.Linux.Get_FileName(l.First))
                        str.Append(" -q --no-unal -p ").Append(System.Environment.ProcessorCount - 1)
                        str.Append(" --rg-id ").Append(exp_Name)
                        Dim nFile As New FileInfo(Files.First.DirectoryName & "\" & exp_Name & ".sam")
                        str.Append(" -S ").Append(Szunyi.IO.Linux.Get_FileName(nFile))
                        str.AppendLine()

                    Next
                End If

                Return str.ToString
            End Function
            Public Shared Function Get_All(DBs As List(Of String), files As List(Of FileInfo)) As String
                Dim str As New System.Text.StringBuilder
                For Each db In DBs
                    For Each file In files
                        str.Append("bowtie2 -x ").Append(db)
                        str.Append(" -U ").Append(Szunyi.IO.Linux.Get_FileName(file))
                        Dim nFile = Szunyi.IO.Files.Get_New_FileName.Append_Before_Extension_wNew_Extension(file, ".sam")
                        str.Append(" -S ").Append(Szunyi.IO.Linux.Get_FileName(nFile))
                        str.Append("-p ").Append(System.Environment.ProcessorCount - 1)
                        str.AppendLine()
                    Next
                Next
                Return str.ToString
            End Function


        End Class
        Public Class Picard
            ''' <summary>
            ''' Create Picard Script wo Missing Read Group
            ''' </summary>
            ''' <param name="Files"></param>
            ''' <returns></returns>
            Public Shared Function Validate(Files As List(Of FileInfo)) As String
                Dim str As New System.Text.StringBuilder
                For Each File In Files
                    str.Append("java -jar picard.jar ValidateSamFile I=")
                    str.Append(Szunyi.IO.Files.Get_New_FileName.Windos_Console(File))
                    str.Append(" O=")
                    Dim nFile = Szunyi.IO.Files.Get_New_FileName.Append_Before_Extension_wNew_Extension(File, ".rep")
                    str.Append(Szunyi.IO.Files.Get_New_FileName.Windos_Console(nFile))
                    str.Append(" MO=10000000 IGNORE=RECORD_MISSING_READ_GROUP")
                    str.AppendLine()

                Next
                If str.Length > 0 Then Return str.ToString
                Return String.Empty
            End Function
            ''' <summary>
            ''' Create RG Infromation
            ''' </summary>
            ''' <param name="File"></param>
            ''' <returns></returns>
            Public Shared Function RG(File As FileInfo) As String
                Dim Lines = Szunyi.IO.Import.Text.Parse(File, "#")
                Dim str As New System.Text.StringBuilder
                For Each Line In Lines
                    Dim s = Split(Line, vbTab) ' FileName,RGID	RGLB	RGPL	RGPU	RGSM
                    Dim cFile As New FileInfo(s(0))
                    str.Append("java -jar picard.jar AddOrReplaceReadGroups I=")
                    str.Append(Szunyi.IO.Files.Get_New_FileName.Windos_Console(cFile))
                    str.Append(" O=")
                    Dim nFIle = Szunyi.IO.Files.Get_New_FileName.Append_Before_Extension(cFile, "_wRG")
                    str.Append(Szunyi.IO.Files.Get_New_FileName.Windos_Console(nFIle))
                    str.Append(" RGID=").Append(Chr(34)).Append(s(1)).Append(Chr(34))
                    str.Append(" RGLB=").Append(Chr(34)).Append(s(2)).Append(Chr(34))
                    str.Append(" RGPL=").Append(Chr(34)).Append(s(3)).Append(Chr(34))
                    str.Append(" RGPU=").Append(Chr(34)).Append(s(4)).Append(Chr(34))
                    str.Append(" RGSM=").Append(Chr(34)).Append(s(5)).Append(Chr(34))
                    str.AppendLine()
                Next
                If str.Length > 0 Then Return str.ToString
                Return String.Empty
            End Function

            Public Shared Function Get_Bad_Records(File As FileInfo) As List(Of Picard_Record)
                Dim out As New List(Of Picard_Record)
                For Each Line In Szunyi.IO.Import.Text.Parse(File)
                    If Line.StartsWith("ERROR") Then
                        out.Add(New Picard_Record(Line))
                    End If
                Next
                Dim res = From x In out Where x.Record_ID <> -1 Order By x.Record_ID Descending

                If res.Count > 0 Then
                    Dim out2 As New List(Of Picard_Record)
                    Dim gr = From x In res Group By x.Record_ID Into Group

                    For Each Item In gr
                        out2.Add(Item.Group.First)
                    Next

                    Return out2
                End If
                Return New List(Of Picard_Record)
            End Function
            Public Shared Function Get_Warning_Records(File As FileInfo) As List(Of Picard_Record)
                Dim out As New List(Of Picard_Record)
                For Each Line In Szunyi.IO.Import.Text.Parse(File)
                    If Line.StartsWith("WARNING") Then
                        out.Add(New Picard_Record(Line))
                    End If
                Next
                Dim res = From x In out Where x.Record_ID <> -1 Order By x.Record_ID Descending

                If res.Count > 0 Then
                    Dim out2 As New List(Of Picard_Record)
                    Dim gr = From x In res Group By x.Record_ID Into Group

                    For Each Item In gr
                        out2.Add(Item.Group.First)
                    Next

                    Return out2
                End If
                Return New List(Of Picard_Record)
            End Function
            Public Shared Sub Remove_Bad_Records(FIle As FileInfo, bRs As List(Of Picard_Record))
                Dim cBRS = (From x In bRs Order By x.Record_ID Ascending).ToList
                Dim Header = Szunyi.BAM.Bam_Basic_IO.Headers.Get_Header(FIle)
                Dim nFile = Szunyi.IO.Files.Get_New_FileName.Append_Before_Extension(FIle, "_Removed_Bad_Picard")
                Using sw As New Szunyi.BAM.Bam_Basic_IO.Export(nFile, Header)
                    Dim Index As Integer = 1
                    Dim Index_of_cBad_Record As Integer = 0
                    Dim RID_to_Remove = cBRS.First.Record_ID

                    For Each SAM In Szunyi.BAM.Bam_Basic_IO.Import.Parse(FIle)

                        If Index = RID_to_Remove Then
                            Index_of_cBad_Record += 1
                            If Index_of_cBad_Record < cBRS.Count Then
                                RID_to_Remove = cBRS(Index_of_cBad_Record).Record_ID
                            End If
                        Else
                            sw.Write(SAM)
                        End If
                        Index += 1
                    Next
                End Using
            End Sub
        End Class
        Public Class Picard_Record
            'Record 36541, Read name 92c22ec2-fa97-496e-98bd-8df25c659def, Read CIGAR M operator maps off end of reference

            Public Property Record_ID As Long = -1
            Public Property Read_ID As String
            Public Property Description As String
            Public Sub New(Line As String)
                '  Szunyi.IO.Files.Get_Files_Without_Extension()
                Dim s = Split(Line, ",")
                If s.Count = 3 Then
                    Me.Record_ID = s(0).Replace("ERROR: Record ", "")
                    Me.Read_ID = s(1).Replace(" Read name ", "")
                    Me.Description = s(2)
                ElseIf s.Count = 4 Then
                    Me.Record_ID = s(0).Replace("WARNING: Record ", "")
                    Me.Read_ID = s(1).Replace(" Read name ", "")
                    Me.Description = s(2) & s(3)
                ElseIf s.Count = 5 Then
                    Me.Record_ID = s(0).Replace("ERROR: Record ", "")
                    Me.Read_ID = s(1).Replace(" Read name ", "")
                    Me.Description = s(2)
                End If
            End Sub
        End Class
    End Namespace
End Namespace

