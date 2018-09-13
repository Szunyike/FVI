Imports System.IO
Imports System.Text
Imports System.Threading
Imports Bio
Namespace Szunyi
    Namespace Blast
        Namespace Console
            Public Class CreateDatabase
                Private Files As New List(Of FileInfo)
                Private isDNA As Boolean
                Public Sub New(Files As List(Of FileInfo), isdna As Boolean)
                    Me.Files = Files
                    Me.isDNA = isdna
                End Sub
                Public Sub New(File As FileInfo, isdna As Boolean)
                    Me.Files.Add(File)
                    Me.isDNA = isdna
                End Sub
                Public Sub DoIt()
                    For Each File In Files
                        Dim consoleApp As New Process
                        With consoleApp
                            .EnableRaisingEvents = True
                            .StartInfo.FileName = My.Resources.BlastPath & "makeblastdb.exe"
                            .StartInfo.RedirectStandardError = True
                            .StartInfo.UseShellExecute = False
                            .StartInfo.CreateNoWindow = True
                            .StartInfo.WindowStyle = ProcessWindowStyle.Hidden
                        End With

                        Dim Arguments As New StringBuilder


                        Arguments.Append(" -in " & File.FullName)
                        Arguments.Append(" -out " & My.Resources.BlastDbPath & File.Name)
                        Arguments.Append(" -parse_seqids ")
                        If isDNA = True Then
                            Arguments.Append("-dbtype nucl ")
                        Else
                            Arguments.Append("-dbtype prot ")
                        End If
                        consoleApp.StartInfo.Arguments = Arguments.ToString

                        Dim out1 As System.IO.StreamReader

                        Try
                            consoleApp.Start()
                            out1 = consoleApp.StandardError
                            Dim alfr = out1.ReadToEnd
                            If alfr <> "" Then
                                ' MsgBox("Error Creating Database")
                                MsgBox(alfr)
                            Else

                            End If
                        Catch ex As Exception
                            MsgBox(ex.ToString)

                        End Try

                    Next
                End Sub


            End Class

            Public Class DoBlast
                Private dbFiles As List(Of FileInfo)
                Private queryFiles As List(Of FileInfo)
                Private SelectedProgram As String
                Private OutFmt As Integer
                Public Sub New(queryFiles As List(Of FileInfo), dbFiles As List(Of FileInfo), selectedProgram As String, fmt As Integer)
                    Me.queryFiles = queryFiles
                    Me.dbFiles = dbFiles
                    Me.SelectedProgram = selectedProgram
                    Me.OutFmt = fmt
                End Sub
                Public Sub DoIt()
                    Dim log As New System.Text.StringBuilder
                    Dim out1 As System.IO.StreamReader
                    Dim ResultFiles As New List(Of String)
                    For Each Query In queryFiles
                        For Each DbFile In dbFiles
                            Dim consoleApp As New Process
                            With consoleApp
                                .EnableRaisingEvents = True
                                '
                                .StartInfo.FileName = My.Resources.BlastPath & "\" & SelectedProgram
                                .StartInfo.RedirectStandardError = True
                                .StartInfo.UseShellExecute = False
                                Dim ResultFile As New FileInfo(My.Resources.BlastResultPath & "\" & DbFile.Name.Replace(DbFile.Extension, "") &
                                    "_" & Query.Name & ".xml")
                                Dim ResultFileII As New FileInfo(My.Resources.BlastResultPath & "\" &
                                DbFile.Name.Replace(DbFile.Extension, "") & " " & Query.Name & ".xml")

                                .StartInfo.Arguments = "-query " & Query.FullName &
                                   " -db " & DbFile.FullName.Replace(DbFile.Extension, "") &
                                   " -out " & ResultFile.FullName &
                                   " -outfmt " & OutFmt & " -max_target_seqs 5000  -evalue 1 -num_threads " &
                                   Environment.ProcessorCount
                                .StartInfo.CreateNoWindow = True
                                .StartInfo.WindowStyle = ProcessWindowStyle.Hidden



                                consoleApp.Start()
                                out1 = consoleApp.StandardError
                                Dim alfr = out1.ReadToEnd
                                If alfr <> "" Then
                                    ' MsgBox("Error Blast Database")
                                    MsgBox(alfr)
                                Else


                                    If ResultFileII.Exists = True Then ResultFileII.Delete()
                                    My.Computer.FileSystem.RenameFile(ResultFile.FullName, ResultFileII.Name)
                                    Beep()
                                End If

                            End With
                        Next
                    Next
                End Sub
            End Class

            Public Class Retrive
                Public Shared Property Ms As MemoryStream
                Public Shared Property TxtWriter As TextWriter

                Public Shared Property output As New StringBuilder()
                Public Shared Property [error] As New StringBuilder()
                Public Shared Property outputWaitHandle As New AutoResetEvent(False)
                Public Shared Property errorWaitHandle As New AutoResetEvent(False)
                Public Property Finished As Boolean = False

                Public Function GetSeqsFromBlastDatabase(DatabaseFile As FileInfo,
                                                                tmpFile As FileInfo,
                                                                log As StringBuilder) As Stream

                    output.Length = 0
                    [error].Length = 0

                    Dim Arguments As New StringBuilder

                    Arguments.Append("-db " & DatabaseFile.FullName)
                    Arguments.Append(" -entry_batch " & tmpFile.FullName)
                    Arguments.Append(" -outfmt %f ") ' As Fasta File


                    Using consoleApp As New Process

                        With consoleApp
                            .EnableRaisingEvents = True
                            .StartInfo.FileName = My.Resources.BlastPath & "\blastdbcmd.exe"
                            .StartInfo.RedirectStandardError = True
                            .StartInfo.RedirectStandardOutput = True
                            .StartInfo.UseShellExecute = False
                            .StartInfo.CreateNoWindow = True
                            .StartInfo.WindowStyle = ProcessWindowStyle.Hidden

                        End With

                        consoleApp.StartInfo.Arguments = Arguments.ToString
                        log.Append("blastdbcmd.exe " & Arguments.ToString)

                        AddHandler consoleApp.OutputDataReceived, AddressOf OutputUpdateReceived
                        AddHandler consoleApp.ErrorDataReceived, AddressOf ErrorDataReceived
                        Using outputWaitHandle As New AutoResetEvent(False)
                            Using errorWaitHandle As New AutoResetEvent(False)

                                consoleApp.Start()

                                consoleApp.BeginOutputReadLine()
                                consoleApp.BeginErrorReadLine()

                                ' Process completed. Check process.ExitCode here.
                                Do
                                    If consoleApp.WaitForExit(1000) AndAlso outputWaitHandle.WaitOne(1000) AndAlso errorWaitHandle.WaitOne(1000) Then
                                        ' Timed out.
                                        Dim alf As Int16 = 54
                                    Else
                                        If Me.Finished = True Then
                                            Dim ascii As Encoding = Encoding.ASCII
                                            Dim ms As MemoryStream
                                            SyncLock output
                                                Dim k = ascii.GetBytes(output.ToString)
                                                ms = New MemoryStream(k)
                                            End SyncLock
                                            Return ms
                                        End If
                                    End If

                                Loop

                            End Using
                        End Using
                    End Using


                    Return Ms
                End Function

                Private Sub ErrorDataReceived(sender As Object, e As DataReceivedEventArgs)
                    If e.Data Is Nothing Then
                        errorWaitHandle.[Set]()
                    Else
                        [error].AppendLine(e.Data)
                    End If
                End Sub

                Private Sub OutputUpdateReceived(sender As Object, e As DataReceivedEventArgs)
                    If e.Data Is Nothing Then
                        Me.Finished = True
                        outputWaitHandle.[Set]()
                    Else
                        output.AppendLine(e.Data)
                    End If
                End Sub


            End Class
        End Namespace
    End Namespace
End Namespace

