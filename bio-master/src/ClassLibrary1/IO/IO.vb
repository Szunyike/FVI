Imports System.IO
Imports System.Net
Imports System.Drawing

Imports System.Text
Imports System.Windows.Forms
Imports Bio
Imports ClassLibrary1.Szunyi.Comparares
Imports ClassLibrary1.Szunyi.Features.FeatureManipulation
Imports Bio.Web.Blast

Namespace Szunyi
    Namespace IO
        Public Class Linux

            Public Shared Function Get_FileName(File As DirectoryInfo) As String
                Dim Drive = System.IO.Path.GetPathRoot(File.FullName)
                ' Set : and path separator
                Dim DriveToSmallCapital = File.FullName.Replace(Drive, Drive.ToLower).Replace(":", "").Replace("\", "/")
                ' escape space
                DriveToSmallCapital = DriveToSmallCapital.Replace(" ", "\ ")
                Dim tmpFile = "/mnt/" & DriveToSmallCapital
                Return tmpFile


            End Function
            Public Shared Function Get_FileName(File As FileInfo) As String
                Dim Drive = System.IO.Path.GetPathRoot(File.FullName)
                ' Set : and path separator
                Dim DriveToSmallCapital = File.FullName.Replace(Drive, Drive.ToLower).Replace(":", "").Replace("\", "/")
                ' escape space
                DriveToSmallCapital = DriveToSmallCapital.Replace(" ", "\ ")
                Dim tmpFile = "/mnt/" & DriveToSmallCapital
                Return tmpFile

            End Function
            Public Shared Function Get_FileName(DIr As DirectoryInfo, FIle As FileInfo, Extension As String) As String

                Dim tmpFile = "/mnt/" & DIr.FullName.Replace("\", "/").Replace("C:", "c").Replace("D:", "d").Replace("d:", "d").Replace("c:", "c")
                If tmpFile.EndsWith("/") = False Then tmpFile = tmpFile & "/"
                tmpFile = tmpFile & FIle.Name.Replace(FIle.Extension, Extension)

                Return tmpFile

            End Function
            Public Shared Function Get_FileName(DIr As DirectoryInfo, FIle1 As FileInfo, File2 As FileInfo, Extension As String) As String

                Dim tmpFile = "/mnt/" & DIr.FullName.Replace("\", "/").Replace("C:", "c").Replace("D:", "d").Replace("d:", "d").Replace("c:", "c")
                If tmpFile.EndsWith("/") = False Then tmpFile = tmpFile & "/"
                tmpFile = tmpFile & FIle1.Name.Replace(FIle1.Extension, "_") & File2.Name.Replace(File2.Extension, Extension)



                Return tmpFile

            End Function
        End Class

        Public Class Files
            Public Class Sort
                Public Class ByFileName
                    Implements IComparer(Of FileInfo)

                    Public Function Compare(x As FileInfo, y As FileInfo) As Integer Implements IComparer(Of FileInfo).Compare
                        Return x.Name.CompareTo(y.Name)
                    End Function
                End Class
            End Class
            Public Class Iter
                Public Shared Iterator Function ByFileName(Files As List(Of FileInfo)) As IEnumerable(Of IEnumerable(Of FileInfo))
                    Dim gr = From x In Files Group By x.Name Into Group

                    For Each g In gr
                        Yield g.Group
                    Next
                End Function
            End Class
            Public Class Filter
                ''' <summary>
                ''' Return list of files or empty list
                ''' </summary>
                ''' <param name="Extension"></param>
                ''' <param name="AllFiles"></param>
                ''' <returns></returns>
                Friend Shared Function GetFilesByExtension(Extension As String, AllFiles As List(Of FileInfo)) As List(Of FileInfo)
                    Dim t = From x In AllFiles Where x.Extension = Extension
                    If t.Count > 0 Then Return t.ToList
                    Return New List(Of FileInfo)
                End Function

                ''' <summary>
                ''' Return the List Of Fileinfo From List Of FileNames(String)
                ''' </summary>
                ''' <param name="FileNames"></param>
                ''' <returns></returns>
                Public Shared Function GetFilesFromFileNames(FileNames As List(Of String)) As List(Of FileInfo)
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
                Public Shared Function SelectFiles(Optional Filter As String = "",
                                               Optional Title As String = "Select Files",
                                               Optional StartDir As DirectoryInfo = Nothing) As List(Of FileInfo)

                    Dim ofd1 As New OpenFileDialog
                    ofd1.Title = Title
                    If IsNothing(StartDir) = False Then ofd1.InitialDirectory = StartDir.FullName
                    ofd1.Multiselect = True
                    If Filter <> "" Then ofd1.Filter = Filter
                    If ofd1.ShowDialog = DialogResult.OK Then
                        Dim Out As New List(Of FileInfo)
                        Dim Names = ofd1.FileNames
                        '  Names.Sort()
                        For Each FileName In ofd1.FileNames
                            Out.Add(New FileInfo(FileName))
                        Next
                        '   Out.Sort()
                        Return Out
                    End If
                    Return New List(Of FileInfo)
                End Function

                Public Shared Function SelectFiles(Files As List(Of FileInfo), s As String) As List(Of FileInfo)
                    Dim out As New List(Of FileInfo)
                    For Each File In Files
                        If File.Name.Contains(s) Then out.Add(File)
                    Next
                    Return out
                End Function
                Public Shared Function SelectFiles(Files As List(Of FileInfo), ls As List(Of String)) As List(Of FileInfo)
                    Dim out As New List(Of FileInfo)
                    For Each File In Files
                        For Each s In ls
                            If File.Name.Contains(s) Then
                                If out.Contains(File) = False Then out.Add(File)
                            End If
                        Next

                    Next
                    Return out
                End Function

                ''' <summary>
                ''' Return Fileinfo, else return nothing
                ''' </summary>
                ''' <param name="Title"></param>
                ''' <param name="Filter"></param>
                ''' <returns></returns>
                Public Shared Function SelectFile(Optional Filter As String = "",
                                              Optional Title As String = "Select Files",
                                              Optional StartDir As DirectoryInfo = Nothing) As FileInfo

                    Dim ofd1 As New OpenFileDialog
                    If IsNothing(StartDir) = False Then ofd1.InitialDirectory = StartDir.FullName
                    ofd1.Multiselect = False
                    ofd1.Title = Title
                    If Filter <> "" Then ofd1.Filter = Filter
                    If ofd1.ShowDialog = DialogResult.OK Then
                        Return New FileInfo(ofd1.FileName)
                    End If
                    Return Nothing
                End Function
                ''' <summary>
                ''' Retrun list of fileinfo where name contains, or nothing 
                ''' </summary>
                ''' <param name="allFiles"></param>
                ''' <param name="PartOfFileName"></param>
                ''' <returns></returns>
                Friend Shared Function FilterFilesByName(allFiles As List(Of FileInfo), PartOfFileName As String) As List(Of FileInfo)
                    Dim x = From t In allFiles Where t.Name.Contains(PartOfFileName)

                    If x.Count > 0 Then Return x.ToList

                    Return Nothing

                End Function

            End Class
            Public Class Group
                ''' <summary>
                ''' Using FIle.Name
                ''' </summary>
                ''' <param name="Files"></param>
                ''' <param name="separator"></param>
                ''' <returns></returns>
                Public Shared Iterator Function Iterate_By_First_Parts(Files As List(Of FileInfo), separator As String) As IEnumerable(Of List(Of FileInfo))
                    Dim gr = From x In Files Group By Split(x.Name, separator).First Into Group
                    For Each g In gr
                        Yield g.Group.ToList
                    Next
                End Function
                Public Shared Iterator Function Iterate_By_First_Parts(Files As Dictionary(Of FileInfo, List(Of String)), separator As String) As IEnumerable(Of List(Of KeyValuePair(Of FileInfo, List(Of String))))
                    Dim gr = From x In Files Group By Split(x.Key.Name, separator).First Into Group
                    For Each g In gr
                        Yield g.Group.ToList
                    Next
                End Function
                Public Shared Iterator Function Iterate_By_Last_Parts(Files As List(Of FileInfo), separator As String) As IEnumerable(Of List(Of FileInfo))
                    Dim gr = From x In Files Group By Split(x.Name, separator).Last Into Group
                    For Each g In gr
                        Yield g.Group.ToList
                    Next
                End Function

            End Class

            Public Class Move_Copy
                Public Shared Sub CopyTo(file As FileInfo, NewFile As FileInfo)
                    Try
                        file.CopyTo(NewFile.FullName)
                    Catch ex As Exception
                        MsgBox(ex.ToString)
                    End Try
                End Sub

                Public Shared Sub MergeFiles(Files As List(Of FileInfo), NewFile As FileInfo, Optional OnlyFirstHeader As Boolean = False)
                    Using sw As New StreamWriter(NewFile.FullName)
                        WriteAllFile(sw, Files(0))
                        For i1 = 1 To Files.Count - 1
                            If OnlyFirstHeader = True Then
                                WriteAllExceptHeader(sw, Files(i1))
                            Else
                                WriteAllFile(sw, Files(i1))
                            End If
                        Next
                    End Using

                End Sub
                Public Shared Sub MergeFiles(Basic_file As FileInfo, File_with_new_data As FileInfo, Optional OnlyFirstHeader As Boolean = False)
                    Using sw As New StreamWriter(Basic_file.FullName, True)
                        For Each line In Szunyi.IO.Import.Text.Parse(File_with_new_data)
                            '  sw.WriteLine()
                            sw.Write(line)
                        Next
                    End Using

                End Sub
                Public Shared Sub WriteAllFile(ByRef sw As StreamWriter, OldFile As FileInfo)
                    For Each Line In Szunyi.IO.Import.Text.Parse(OldFile)
                        If Line <> "" Then
                            sw.Write(Line)
                            sw.WriteLine()
                        End If
                    Next

                End Sub
                Public Shared Sub WriteAllExceptHeader(ByRef sw As StreamWriter, OldFile As FileInfo)
                    For Each Line In Szunyi.IO.Import.Text.ParseNotFirst(OldFile, 1)
                        sw.Write(Line)
                        sw.WriteLine()
                    Next

                End Sub

                Public Shared Function MoveOrRenameBlastFastaFiles(Files As List(Of FileInfo)) As List(Of FileInfo)
                    Dim Out As New List(Of FileInfo)
                    For Each File In Files
                        Out.Add(MoveOrRenameBlastFastaFile(File))
                    Next
                    Return Out
                End Function
                Public Shared Function MoveOrRenameBlastFastaFile(File As FileInfo) As FileInfo
                    Dim NewFileName = File.Name.Replace(" ", "_")

                    Dim NewFile As New FileInfo(My.Resources.BlastFastaFilesPath & NewFileName)
                    If NewFile.Exists = True Then
                        Return NewFile
                    End If
                    If File.DirectoryName & "\" = My.Resources.BlastFastaFilesPath Then
                        If NewFileName = File.Name Then
                            ' File is already in right place
                            ' Do not do anything
                            Return File
                        Else
                            ' Rename It
                            File.MoveTo(My.Resources.BlastFastaFilesPath & "\" & NewFileName)
                        End If
                    Else
                        ' Copy
                        Szunyi.IO.Files.Move_Copy.CopyTo(File, NewFile)
                    End If
                    Return NewFile
                End Function

            End Class

            Public Class Save
                Public Shared Function SelectSaveFile(Filter As String, Optional Title As String = "Save As") As FileInfo
                    Dim sfd1 As New SaveFileDialog
                    sfd1.Title = Title
                    sfd1.Filter = Filter
                    If sfd1.ShowDialog = DialogResult.OK Then
                        Return New FileInfo(sfd1.FileName)
                    Else
                        Return Nothing
                    End If
                End Function

            End Class

            Public Class Get_New_FileName
                ''' <summary>
                ''' Return FileName without Extension
                ''' </summary>
                ''' <param name="File"></param>
                ''' <returns></returns>
                Public Shared Function GetFileName_woExtension(File As FileInfo) As String
                    If File.Extension = "" Then Return File.Name
                    Return File.Name.Trim(File.Extension.ToCharArray)

                End Function
                ''' <summary>
                ''' replace extension with custom name
                ''' </summary>
                ''' <param name="fileInSearch"></param>
                ''' <param name="v"></param>
                ''' <returns></returns>
                Public Shared Function GetNewFile(fileInSearch As FileInfo, v As String) As FileInfo
                    Dim t = fileInSearch.DirectoryName & "\" & fileInSearch.Name.TrimEnd(fileInSearch.Extension.ToCharArray) & v
                    Dim Out As String = fileInSearch.FullName.Replace(fileInSearch.Extension, v)
                    Dim x As New FileInfo(t)
                    Return x

                End Function

                Public Shared Function Append_Before_Extension(BasicFile As FileInfo, Append As String) As FileInfo
                    Dim t = BasicFile.DirectoryName & "\" & BasicFile.Name.Trim(BasicFile.Extension.ToCharArray) & Append & BasicFile.Extension
                    Dim x As New FileInfo(t)
                    Return x

                End Function
                Public Shared Function Append_Before_Extension(BasicFile As FileInfo, Second_file As FileInfo) As FileInfo
                    Dim t = BasicFile.DirectoryName & "\" & BasicFile.Name.Trim(BasicFile.Extension.ToCharArray)
                    t = t & "_" & Second_file.Name.Trim(Second_file.Extension.ToCharArray) & BasicFile.Extension
                    Dim x As New FileInfo(t)
                    Return x

                End Function
                Public Shared Function Append_Before_Extension_wNew_Extension(BasicFile As FileInfo, SecondFile As FileInfo, ext As String) As FileInfo
                    Dim t = BasicFile.DirectoryName & "\" & BasicFile.Name.Trim(BasicFile.Extension.ToCharArray) &
                         "_" & SecondFile.Name.Trim(SecondFile.Extension.ToCharArray) & ext
                    Dim x As New FileInfo(t)
                    Return x

                End Function
                Public Shared Function Append_Before_Extension_wNew_Extension(BasicFile As FileInfo, Append As String) As FileInfo
                    Dim t = BasicFile.DirectoryName & "\" & BasicFile.Name.Trim(BasicFile.Extension.ToCharArray) & Append
                    Dim x As New FileInfo(t)
                    Return x

                End Function

                Public Shared Function Windos_Console(file As FileInfo) As String
                    Return Chr(34) & file.FullName & Chr(34)
                End Function


                Public Shared Function Replace_Extension(File As FileInfo, extension As String) As FileInfo
                    Dim Ext = File.Extension
                    Dim t = File.DirectoryName & "\" & File.Name.Trim(File.Extension.ToCharArray) & extension
                    Dim Out As String = File.FullName.Replace(File.Extension, extension).Replace("..", ".")
                    Dim x As New FileInfo(t)
                    Return x
                End Function
            End Class

            Public Class Extra
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
                                Dim s = Szunyi.Text.General.GetText(s1, ValuesColIndexes, vbTab)
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
                    If log.Length > 0 Then
                        ' MsgBox(log.ToString)
                    End If
                    Return out
                End Function

            End Class
        End Class

        Public Class Directory
            ''' <summary>
            ''' Retrun Nothing Or List of Directories
            ''' </summary>
            ''' <param name="Path"></param>
            ''' <returns></returns>
            Public Shared Function GetSubDirectories(Path As String) As List(Of DirectoryInfo)
                Dim Dir As New DirectoryInfo(Path)
                If Dir.Exists = False Then Return Nothing
                Dim Dirs = Dir.GetDirectories
                If Dirs.Count = 0 Then Return Nothing
                Return Dirs.ToList
            End Function
            ''' <summary>
            ''' Convert List of string) into list (of DirectoryInfo)
            ''' </summary>
            ''' <param name="ls"></param>
            ''' <returns></returns>
            Public Shared Function Get_Directories(ls As List(Of String)) As List(Of DirectoryInfo)
                Dim out As New List(Of DirectoryInfo)
                For Each Item In ls
                    out.Add(New DirectoryInfo(Item))
                Next
                Return out
            End Function
            ''' <summary>
            ''' Retrun SelectedPath or Nothing 
            ''' </summary>
            ''' <param name="Title"></param>
            ''' <returns></returns>
            Public Shared Function Get_Folder(Optional Title As String = "Select Folder") As DirectoryInfo
                Dim fbd1 As New FolderBrowserDialog
                fbd1.Description = Title
                If fbd1.ShowDialog = DialogResult.OK Then Return New DirectoryInfo(fbd1.SelectedPath)
                Return Nothing
            End Function

            Public Shared Function Get_Folder(Dir As DirectoryInfo, Title As String) As DirectoryInfo
                Dim fbd1 As New FolderBrowserDialog
                fbd1.SelectedPath = Dir.FullName
                fbd1.Description = Title
                If fbd1.ShowDialog = DialogResult.OK Then Return New DirectoryInfo(fbd1.SelectedPath)
                Return Nothing
            End Function

            ''' <summary>
            ''' Return List of FileInfo Or new List
            ''' </summary>
            ''' <param name="folder"></param>
            ''' <returns></returns>
            Public Shared Function GetAllFilesFromFolder_Recursive(Extension As String, Optional Folder As DirectoryInfo = Nothing) As IEnumerable(Of FileInfo)
                Dim dir As DirectoryInfo
                If IsNothing(Folder) = True Then
                    dir = Szunyi.IO.Directory.Get_Folder
                Else

                    dir = Folder
                End If


                If IsNothing(dir) = True OrElse dir.Exists = False Then Return New List(Of FileInfo)
                Try
                    Dim FIles = dir.GetFiles.ToList
                    For Each sDir In dir.EnumerateDirectories
                        FIles.AddRange(GetAllFilesFromFolder_Recursive(sDir))
                    Next
                    If Extension = "" Then
                        Return FIles
                    Else
                        Dim cFiles = From x In FIles Where x.Extension = Extension
                        Return cFiles
                    End If

                Catch ex As Exception
                    Return New List(Of FileInfo)
                End Try

            End Function
            ''' <summary>
            ''' Return List of FileInfo Or new List
            ''' </summary>
            ''' <param name="dir"></param>
            ''' <returns></returns>
            Public Shared Function GetFirstFilesFromFolder_Recursive(dir As DirectoryInfo) As List(Of FileInfo)
                If dir.Exists = False Then Return New List(Of FileInfo)
                Try

                    Dim FIles As New List(Of FileInfo)
                    If dir.GetFiles.Count > 0 Then
                        FIles.Add(dir.GetFiles.First)
                    End If
                    For Each sDir In dir.EnumerateDirectories
                        FIles.AddRange(GetFirstFilesFromFolder_Recursive(sDir))
                    Next
                    Return FIles.ToList
                Catch ex As Exception
                    Return New List(Of FileInfo)
                End Try

            End Function
            ''' <summary>
            ''' Return List of FileInfo Or new List
            ''' </summary>
            ''' <param name="dir"></param>
            ''' <returns></returns>
            Public Shared Function GetAllFilesFromFolder_Recursive(dir As DirectoryInfo) As List(Of FileInfo)
                If dir.Exists = False Then Return New List(Of FileInfo)
                Try
                    Dim FIles = dir.GetFiles.ToList
                    For Each sDir In dir.EnumerateDirectories
                        FIles.AddRange(GetAllFilesFromFolder_Recursive(sDir))
                    Next
                    Return FIles.ToList
                Catch ex As Exception
                    Return New List(Of FileInfo)
                End Try

            End Function
            ''' <summary>
            ''' Return List of FileInfo Or new List
            ''' </summary>
            ''' <param name="dir"></param>
            ''' <returns></returns>
            Public Shared Function GetAllFilesFromFolder(dir As DirectoryInfo) As List(Of FileInfo)
                If dir.Exists = False Then Return New List(Of FileInfo)
                Try
                    Dim FIles = dir.GetFiles.ToList

                    Return FIles.ToList
                Catch ex As Exception
                    Return New List(Of FileInfo)
                End Try

            End Function

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

                        TheWebClient.DownloadFile(uri, FileName)
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
                Public Shared Sub GoToWebPage(uri As String)
                    Try
                        Dim wb As New WebBrowser
                        wb.Navigate(uri)
                        Dim alf As Int16 = 54
                    Catch ex As Exception
                        Dim alf As Int16 = 54
                    End Try

                End Sub
            End Class
        End Class
    End Namespace
End Namespace

