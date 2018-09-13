Imports System.IO
Imports System.Text

Namespace Szunyi
    Namespace Blat
        Public Class ConsoleBlat
            Public Shared Sub DoBlat(QueryFiles As List(Of FileInfo), DbFIles As List(Of FileInfo))
                'Temporaly Move Files To Blat Path
                Dim log As New List(Of String)

                Dim NewQueryFIles = Szunyi.IO.FilesAndFolder.MoveFiles.Move(QueryFiles, New DirectoryInfo(My.Resources.BlatPath), log)
                Dim NewDatabaseFiles = Szunyi.IO.FilesAndFolder.MoveFiles.Move(DbFIles, New DirectoryInfo(My.Resources.BlatPath), log)

                For Each QueryFile In QueryFiles
                    For Each DBFile In DbFIles
                        Dim consoleApp As New Process
                        With consoleApp
                            .StartInfo.WorkingDirectory = My.Resources.BlatPath
                            .EnableRaisingEvents = True
                            .StartInfo.FileName = My.Resources.BlatPath & "blat.exe "
                            .StartInfo.RedirectStandardError = True
                            .StartInfo.RedirectStandardOutput = True
                            .StartInfo.UseShellExecute = False
                            .StartInfo.CreateNoWindow = True
                            .StartInfo.WindowStyle = ProcessWindowStyle.Hidden
                        End With
                        Dim Arguments As New StringBuilder

                        Arguments.Append(DBFile.Name).Append(" ")
                        Arguments.Append(QueryFile.Name).Append(" ")
                        Dim ResultFile As New FileInfo(My.Resources.BlatPath &
                                                       QueryFile.Name.Replace(QueryFile.Extension, "") &
                                                      DBFile.Name.Replace(DBFile.Extension, ".pslx"))
                        Dim ResultFileII As New FileInfo(My.Resources.BlatResultPath &
                                                       QueryFile.Name.Replace(QueryFile.Extension, "") &
                                                       " " & DBFile.Name.Replace(DBFile.Extension, ".pslx"))
                        Arguments.Append(ResultFile.Name)
                        Arguments.Append(" -stepSize=5 ")
                        Arguments.Append("-tileSize=11 ")
                        Arguments.Append("-out=pslx")
                        consoleApp.StartInfo.Arguments = Arguments.ToString

                        Dim out1 As System.IO.StreamReader
                        Dim out2 As System.IO.StreamReader
                        Try
                            consoleApp.Start()
                            out1 = consoleApp.StandardError
                            out2 = consoleApp.StandardOutput
                            Dim alfr = out1.ReadToEnd
                            Dim alfr2 = out2.ReadToEnd
                            If alfr <> "" Then
                                ' MsgBox("Error Creating Database")
                                MsgBox(alfr)
                            End If
                        Catch ex As Exception
                            MsgBox(ex.ToString)

                        End Try
                        Szunyi.IO.FilesAndFolder.MoveFiles.Move(ResultFile, ResultFileII, log)

                    Next

                    Szunyi.IO.FilesAndFolder.MoveFiles.Move(NewQueryFIles, QueryFiles, log)



                Next
            End Sub
        End Class

    End Namespace
End Namespace
