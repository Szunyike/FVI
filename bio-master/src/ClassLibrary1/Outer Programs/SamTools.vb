Imports System.IO

Namespace Szunyi
    Namespace Outer_Programs
        Public Enum SamTools_Subprogram
            sort = 0
            index = 1
            Convert_Bam_To_Sam = 2
            sort_And_index = 3
            Remove_Not_Paired = 4
            Get_UnMapped_Reads = 5
            Get_Mapped_Reads = 6
            Convert_Sam_to_Bam = 7
            Convert_sam_to_Bam_Sort_Index = 8
            Merge = 9
            Merge_SOrt_Index = 10
            Merge_Only_Mapped_Sort_Index = 11
        End Enum
        Public Class SamTools
            Public Property Files_To_Delete As New List(Of FileInfo)
            Public Property Files As New List(Of FileInfo)
            Public Property OutPutFiles As New List(Of FileInfo)
            Public Property Type As SamTools_Subprogram
            Public Property SaveFile As FileInfo
            Public Sub New(Files As List(Of FileInfo), OutPutFiles As List(Of FileInfo), Type As SamTools_Subprogram)
                Me.Files = Files
                Me.SaveFile = SaveFile
                Me.Type = Type
                Me.OutPutFiles = OutPutFiles
            End Sub
            Public Sub New(Files As List(Of FileInfo), SaveFile As FileInfo, Type As SamTools_Subprogram)
                Me.Files = Files
                Me.SaveFile = SaveFile
                Me.Type = Type
                Me.OutPutFiles.Add(SaveFile)
            End Sub
            Public Sub New(Files As List(Of FileInfo), Type As SamTools_Subprogram)
                Me.Files = Files
                Me.Type = Type
            End Sub
            Public Sub New(Files As IEnumerable(Of FileInfo), Type As SamTools_Subprogram)
                For Each File In Files
                    Me.Files.Add(File)
                Next
                Me.Type = Type
            End Sub
            Public Sub New(File As FileInfo, Type As SamTools_Subprogram)
                Me.Files.Add(File)
                Me.Type = Type
            End Sub
            Public Sub DoIt()
                If IsNothing(Files) = True Then Exit Sub
                Select Case Type
                    Case SamTools_Subprogram.Convert_Bam_To_Sam
                        Do_Convert_Bam_To_Sam()
                    Case SamTools_Subprogram.index
                        DoIndex()
                    Case SamTools_Subprogram.sort
                        DoSort()
                    Case SamTools_Subprogram.sort_And_index
                        DoSortAndIndex()
                    Case SamTools_Subprogram.Remove_Not_Paired
                        DoRemoveNotPaired()
                    Case SamTools_Subprogram.Get_Mapped_Reads
                        Get_Mapped_Reads()
                    Case SamTools_Subprogram.Get_UnMapped_Reads
                        Get_UnMapped_Reads()
                    Case SamTools_Subprogram.Convert_Sam_to_Bam
                        Do_Convert_Sam_To_Bam()
                    Case SamTools_Subprogram.Convert_sam_to_Bam_Sort_Index
                        Do_Convert_Sam_To_Bam_SOrt_and_Index()
                    Case SamTools_Subprogram.Merge
                        Do_Merge()
                    Case SamTools_Subprogram.Merge_SOrt_Index
                        Do_Merge()
                        Get_Mapped_Reads()
                        DoSort()
                        DoIndex()
                    Case SamTools_Subprogram.Merge_Only_Mapped_Sort_Index
                        Do_Convert_Sam_To_Bam_only_Mapped()
                        '  Get_Mapped_Reads()
                        Do_Merge()
                        DoSort()
                        DoIndex()
                        Szunyi.IO.FilesAndFolder.Delete.Files(Me.Files_To_Delete)

                End Select
            End Sub
            Private Sub Do_Merge()
                Dim str As New System.Text.StringBuilder
                Dim log As New System.Text.StringBuilder
                str.Append(Me.SaveFile.FullName)
                For Each File In Files
                    str.Append(" ").Append(File.FullName)
                Next
                Dim cmd = " merge " & str.ToString
                Do_Console(cmd, log, True)
                If log.Length > 0 Then
                    MsgBox(log.ToString)

                Else
                    Files_To_Delete.AddRange(Me.Files)

                End If
                Me.Files.Clear()
                Me.Files.AddRange(Me.OutPutFiles)

            End Sub
            Private Sub Get_UnMapped_Reads()
                Dim log As New System.Text.StringBuilder
                For Each File In Files
                    Dim NewFileName = Chr(34) & File.FullName.Replace(File.Extension, "_Only_UnMapped_Reads.bam") & Chr(34)
                    Dim cmd = " view -f 4 " & Chr(34) & File.FullName & Chr(34) & " -b -o " & NewFileName
                    Do_Console(cmd, log, True)
                Next
                If log.Length > 0 Then
                    MsgBox(log.ToString)
                End If
            End Sub
            Private Sub Get_Mapped_Reads()
                Dim NewFiles As New List(Of FileInfo)
                Dim log As New System.Text.StringBuilder
                For Each File In Files
                    Dim NewFileName = Chr(34) & File.FullName.Replace(File.Extension, "_Only_Mapped_Reads.bam") & Chr(34)
                    Dim NewFIle As New FileInfo(File.FullName.Replace(File.Extension, "_Only_Mapped_Reads.bam"))

                    Dim cmd = " view " & Chr(34) & File.FullName & Chr(34) & " -b -F 4 -o " & NewFileName
                    NewFiles.Add(NewFIle)
                    Do_Console(cmd, Log, True)
                Next
                Files_To_Delete.AddRange(Me.Files)
                Me.Files = NewFiles
            End Sub
            Private Sub DoRemoveNotPaired()
                Dim log As New System.Text.StringBuilder
                For Each File In Files
                    Dim NewFileName = Chr(34) & File.FullName.Replace(File.Extension, "_Only_Paired.bam") & Chr(34)
                    Dim cmd = " view -F 0x02 " & Chr(34) & File.FullName & Chr(34) & " -u -b -o " & NewFileName
                    Do_Console(cmd, log, True)
                Next
            End Sub
            Private Sub Do_Convert_Bam_To_Sam()
                Dim log As New System.Text.StringBuilder
                For Each File In Files
                    Dim NewFileName = Chr(34) & File.FullName.Replace(File.Extension, ".sam") & Chr(34)
                    Dim cmd = " view " & Chr(34) & File.FullName & Chr(34) & " -h -o " & NewFileName
                    Do_Console(cmd, log, True)
                Next
                If log.Length > 0 Then
                    MsgBox(log.ToString)
                End If
                Files_To_Delete.AddRange(Me.Files)
            End Sub
            Private Sub Do_Convert_Sam_To_Bam()
                Dim NewFiles As New List(Of FileInfo)
                Dim log As New System.Text.StringBuilder
                For Each File In Files
                    Dim NewFileName = Chr(34) & File.FullName.Replace(File.Extension, ".bam") & Chr(34)
                    Dim NewFile As New FileInfo(File.FullName.Replace(File.Extension, ".bam"))
                    Dim cmd = " view " & Chr(34) & File.FullName & Chr(34) & " -b -S -o " & NewFileName
                    Do_Console(cmd, log, True)
                    NewFiles.Add(NewFile)
                Next
                If log.Length > 0 Then
                    MsgBox(log.ToString)
                End If
                Files_To_Delete.AddRange(Me.Files)
                Me.Files = NewFiles
            End Sub
            Private Sub Do_Convert_Sam_To_Bam_only_Mapped()
                Dim NewFiles As New List(Of FileInfo)
                Dim log As New System.Text.StringBuilder
                For Each File In Files
                    Dim NewFileName = Chr(34) & File.FullName.Replace(File.Extension, "Only_Mapped.bam") & Chr(34)
                    Dim NewFile As New FileInfo(File.FullName.Replace(File.Extension, "Only_Mapped.bam"))
                    Dim cmd = " view " & Chr(34) & File.FullName & Chr(34) & " -b -S -F 4 " & " -o " & NewFileName
                    Do_Console(cmd, log, True)
                    NewFiles.Add(NewFile)
                Next
                If log.Length > 0 Then
                    MsgBox(log.ToString)
                End If
                '   Files_To_Delete.AddRange(Me.Files)
                Me.Files = NewFiles
            End Sub
            Private Sub Do_Convert_Sam_To_Bam_SOrt_and_Index()
                Do_Convert_Sam_To_Bam()
                DoSort()
                DoIndex()
            End Sub
            Private Sub DoIndex()
                Dim log As New System.Text.StringBuilder
                For Each File In Files
                    Dim cmd = " index " & Chr(34) & File.FullName & Chr(34)
                    Do_Console(cmd, log, True)
                Next
                If log.Length > 0 Then
                    MsgBox(log.ToString)
                End If
            End Sub
            Private Sub DoSort()
                Dim newfiles As New List(Of FileInfo)
                Dim log As New System.Text.StringBuilder
                For Each File In Files
                    Dim NewFileName = File.FullName.Replace(File.Extension, ".sorted" & File.Extension)
                    Dim NewFile As New FileInfo(NewFileName)
                    NewFileName = File.FullName.Replace(File.Extension, ".sorted")
                    Dim cmd = " sort " & Chr(34) & File.FullName & Chr(34) & " " & Chr(34) & NewFileName & Chr(34)
                    Do_Console(cmd, log, True)
                    newfiles.Add(NewFile)
                Next
                If log.Length > 0 Then
                    MsgBox(log.ToString)
                End If
                Files_To_Delete.AddRange(Me.Files)
                Me.Files = newfiles
            End Sub
            Private Sub DoSortAndIndex()
                DoSort()
                DoIndex()
            End Sub
            Private Sub Do_Console(cmd As String, log As System.Text.StringBuilder, WaitFOrExit As Boolean)
                Dim consoleApp As New Process
                With consoleApp
                    .EnableRaisingEvents = False

                    .StartInfo.FileName = My.Resources.Other_Progrmas & "samtools.exe"
                    .StartInfo.RedirectStandardError = True
                    .StartInfo.RedirectStandardInput = False
                    .StartInfo.UseShellExecute = False
                    .StartInfo.CreateNoWindow = True
                    .StartInfo.RedirectStandardOutput = False
                    .StartInfo.WindowStyle = ProcessWindowStyle.Hidden
                    .StartInfo.Arguments = cmd

                End With

                Try
                    consoleApp.Start()

                    If WaitFOrExit = True Then
                        consoleApp.WaitForExit()

                    End If


                    Dim err = consoleApp.StandardError.ReadToEnd
                    If err.Length > 0 Then
                        log.Append(err.ToString)
                        '   MsgBox(err.ToString)
                    End If


                    '  consoleApp.Close()

                Catch ex As Exception
                    log.Append(ex.Message)
                    MsgBox(ex.ToString)
                Finally
                    consoleApp.Close()
                End Try
            End Sub
        End Class
        Public Class GMAP
            Public Shared Function Get_s(Read_File As FileInfo, db_Name As String, OutDir As DirectoryInfo) As String
                Dim Linux_Read_File = Szunyi.IO.Linux.Get_FileName(Read_File)
                Dim Ref_File = Szunyi.IO.Linux.Get_FileName(Read_File)
                Dim Linux_Out_Dir = Szunyi.IO.Linux.Get_FileName(OutDir) & "/"
                Dim OutPutFile As New FileInfo(OutDir.FullName & "/" & db_Name & "_" & "GMAP_" & Read_File.Name.Replace(Read_File.Extension, ".sam"))
                Dim Linux_Output_File = Szunyi.IO.Linux.Get_FileName(OutPutFile)
                Return ("gmap -d " & db_Name & " --nofails -f samse -t " & Environment.ProcessorCount - 1 & " " & Linux_Read_File & " > " & Linux_Output_File)
            End Function

        End Class
    End Namespace
End Namespace



