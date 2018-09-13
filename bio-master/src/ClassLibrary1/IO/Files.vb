Imports System.IO
Namespace Szunyi
    Namespace IO
        Namespace FilesAndFolder
            Public Class Filter
                ''' <summary>
                ''' Retrun New List Of List of Fileinfo where extensions are match
                ''' </summary>
                ''' <param name="Files"></param>
                ''' <param name="Extension"></param>
                ''' <returns></returns>
                Public Shared Function FilterFilesByExtension(Files As List(Of FileInfo), Extension As String) As List(Of FileInfo)
                    Dim res = From x In Files Where x.Extension = Extension

                    If res.Count = 0 Then Return New List(Of FileInfo)
                    Return res.ToList

                End Function
                ''' <summary>
                ''' Retrun New List Of Fileinfo or List of Fileinfo where file.name contains query string
                ''' </summary>
                ''' <param name="Files"></param>
                ''' <param name="PartialName"></param>
                ''' <returns></returns>
                Public Shared Function FilterFilesByNameContains(Files As List(Of FileInfo), PartialName As String) As List(Of FileInfo)
                    Dim res = From x In Files Where x.Name.IndexOf(PartialName) > -1

                    If res.Count = 0 Then Return New List(Of FileInfo)
                    Return res.ToList
                End Function
            End Class

            Public Class GetFiles
                ''' <summary>
                ''' Retrurn Empty List Or List of FIleinfoFrom FOlder
                ''' </summary>
                ''' <param name="FolderPath"></param>
                ''' <returns></returns>
                Public Shared Function AllFilesFromFolder(FolderPath As String) As List(Of FileInfo)
                    Dim Dir As New DirectoryInfo(FolderPath)
                    Return AllFilesFromFolder(Dir)
                End Function
                ''' <summary>
                '''  Retrurn Empty List Or List of FIleinfoFrom FOlder
                ''' </summary>
                ''' <param name="Dir"></param>
                ''' <returns></returns>
                Public Shared Function AllFilesFromFolder(Dir As DirectoryInfo) As List(Of FileInfo)
                    Dim Files = Dir.GetFiles
                    If Files.Count > 0 Then Return Files.ToList
                    Return New List(Of FileInfo)

                End Function
            End Class

            Public Class MoveFiles
                Public Shared Sub Move(OriginalFiles As List(Of FileInfo), NewFIles As List(Of FileInfo), log As List(Of String))
                    For i1 = 0 To OriginalFiles.Count - 1
                        Move(OriginalFiles(i1), NewFIles(i1), log)
                    Next
                End Sub
                Public Shared Sub Move(OriginalFile As FileInfo, NewFIle As FileInfo, log As List(Of String))
                    Try
                        If NewFIle.Exists = False Then
                            OriginalFile.MoveTo(NewFIle.FullName)

                        End If

                    Catch ex As Exception
                        log.Add("Unable Move From " & OriginalFile.FullName & " to " & NewFIle.FullName)
                    End Try

                End Sub
                Public Shared Function Move(Files As List(Of FileInfo), Dir As DirectoryInfo, log As List(Of String))
                    Dim Out As New List(Of FileInfo)
                    For Each OriginalFile In Files
                        Dim NewFile = New FileInfo(Dir.FullName & OriginalFile.Name)

                        Try

                            Out.Add(NewFile)
                            If NewFile.Exists = False Then
                                OriginalFile.MoveTo(NewFile.FullName)
                            End If

                        Catch ex As Exception
                            log.Add("Unable Move From " & OriginalFile.FullName & " to " & NewFIle.FullName)
                        End Try

                    Next

                    Return Out
                End Function
            End Class

            Public Class Convert
                Public Shared Sub Merge_Files(Files As List(Of FileInfo), SaveFile As FileInfo)
                    Try
                        Using sw As New StreamWriter(SaveFile.FullName)
                            For Each File In Files
                                For Each Line In Szunyi.IO.Import.Text.Parse(File)
                                    sw.Write(Line)
                                    sw.WriteLine()
                                Next
                            Next
                        End Using
                    Catch ex As Exception
                        MsgBox(ex.ToString)
                    End Try
                End Sub
                Public Shared Sub CSV_to_TDT(Files As List(Of FileInfo))
                    For Each File In Files
                        Dim NewFile = Szunyi.IO.Files.Get_New_FileName.GetNewFile(File, ".tdt")
                        Using sw As New StreamWriter(NewFile.FullName)
                            For Each Line In Szunyi.IO.Import.Text.Parse(File)
                                Dim s = Line.Replace(",", vbTab)
                                sw.Write(s)
                                sw.WriteLine()
                            Next
                        End Using
                    Next
                End Sub
                Public Shared Sub TDT_To_CSV(Files As List(Of FileInfo))
                    For Each File In Files
                        Dim NewFile = Szunyi.IO.Files.Get_New_FileName.GetNewFile(File, ".tdt")
                        Using sw As New StreamWriter(NewFile.FullName)
                            For Each Line In Szunyi.IO.Import.Text.Parse(File)
                                Dim s = Line.Replace(vbTab, ";")
                                sw.Write(s)
                                sw.WriteLine()
                            Next
                        End Using
                    Next
                End Sub
            End Class

            Public Class Delete
                Public Shared Sub Files(Files_To_Delete As List(Of FileInfo))
                    Dim log As New List(Of String)
                    Try
                        For Each File In Files_To_Delete
                            File.Delete()
                        Next
                    Catch ex As Exception
                        MsgBox(ex.ToString)
                    End Try
                End Sub

            End Class

        End Namespace
    End Namespace

End Namespace
