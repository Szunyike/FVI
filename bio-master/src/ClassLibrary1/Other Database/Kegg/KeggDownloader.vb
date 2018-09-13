Imports System.Drawing
Imports System.IO
Imports System.Net
Imports System.Text.RegularExpressions
Imports System.Windows.Forms
Imports ClassLibrary1.Szunyi.Other_Database.CrossRefs

Namespace Szunyi
    Namespace Other_Database
        Namespace Kegg
            Public Enum BriteType
                C
                D
                E
            End Enum
            Public Class BriteProcess
                Public Property Files As List(Of FileInfo)
                Public Property OutPutPath As DirectoryInfo
                Public Sub New()
                    Me.Files =  Szunyi.IO.Files.Filter.SelectFiles
                    Me.OutPutPath = Szunyi.IO.Directory.Get_Folder
                End Sub
                Private Shared Function GetIndexInAlphabet(value As Char) As Integer
                    ' Uses the uppercase character unicode code point. 'A' = U+0042 = 65, 'Z' = U+005A = 90
                    Dim upper As Char = Char.ToUpper(value)
                    If upper < "A"c OrElse upper > "Z"c Then
                        Return -1
                    End If

                    Return Microsoft.VisualBasic.AscW(upper) - Microsoft.VisualBasic.AscW("A"c)
                End Function

                Public Sub DoIt()
                    Dim Bs As New List(Of CrossRefs.CrossRefOneToMany)
                    For Each File In Files
                        Dim t = Szunyi.IO.Import.Text.ReadToEnd(File)
                        Dim s1() = Split(t, vbLf)
                        Dim LocusTagIndex = GetBriteType(s1)
                        Dim cHeaders(26) As String
                        Dim BriteswLocusTags(26) As BritewLocusTags
                        Dim cLocusTags As New List(Of String)
                        For Each s In s1
                            If s.Length > 2 Then
                                Dim c As Char = s.Substring(0, 1).ToUpper
                                Dim Index = GetIndexInAlphabet(c)
                                If Index > -1 Then
                                    If Index = LocusTagIndex Then
                                        'Dim LocusTag = Regex.Match(s, "\s[a-zA-Z0-9]{3,5}_[a-zA-Z0-9]{3,9}\s") ' medicago
                                        '  Dim LocusTag = Regex.Match(s, "\b[a-zA-Z0-9]{7,10}\b") '_[a-zA-Z0-9]{3,9}\s") ' At

                                        Dim LocusTag = Regex.Match(s, "\bCHLREDRAFT_[0-9]{1,6}\b") '_[a-zA-Z0-9]{3,9}\s") ' chlamzdomonas

                                        If LocusTag.Success = True Then
                                            cLocusTags.Add(LocusTag.Value.Trim)
                                        Else
                                            Dim alf As Int16 = 54
                                        End If


                                    Else
                                        If cHeaders(Index) <> "" Then
                                            For i1 = 0 To Index
                                                If IsNothing(BriteswLocusTags(i1)) = True Then BriteswLocusTags(i1) = New BritewLocusTags
                                                BriteswLocusTags(i1).LocusTags.AddRange(cLocusTags.ToArray.Clone)
                                            Next
                                            Dim h() As String = BriteswLocusTags(Index).LocusTags.ToArray.Clone
                                            Bs.Add(New CrossRefOneToMany(cHeaders(Index), h))
                                            BriteswLocusTags(Index).LocusTags.Clear()
                                            cLocusTags.Clear()

                                        End If

                                        If s.Contains("<b>") Then
                                            cHeaders(Index) = Szunyi.Text.Regexp.GetDistinctStrings(s, ">", "<").First
                                        Else
                                            cHeaders(Index) = s.Substring(1, s.Length - 1).TrimStart

                                        End If
                                    End If
                                End If
                            End If
                        Next ' Lines
                    Next ' Files

                    For i1 = Bs.Count - 1 To 0 Step -1
                        If Bs(i1).Many.Count = 0 Then
                            Bs.RemoveAt(i1)
                        End If
                    Next
                    Szunyi.Other_Database.CrossRefs.CrossRefBuilders.SaveOneToMany(New FileInfo(Me.OutPutPath.FullName & "\Brite.tab"), Bs)

                End Sub


                Private Function GetBriteType(Lines As String()) As Integer
                    Dim maxChar As Char = "A"
                    For Each Line In Lines
                        If Line.Length <> 0 Then
                            Dim c As Char = Line.Substring(0, 1).ToUpper
                            If Char.IsLetter(c) = True Then
                                If maxChar < c Then maxChar = c
                            End If
                        End If

                    Next
                    Return GetIndexInAlphabet(maxChar)
                End Function

            End Class
            Public Class BritewLocusTags
                Public Property Name As String
                Public Property LocusTags As New List(Of String)
            End Class
            Public Class BriteDownloader
                Dim WithEvents wb As New WebBrowser
                Private KeggID As String
                Dim PathwayIDs As New List(Of String)
                Dim FolderOfDownloadedFile As DirectoryInfo
                Dim Log As New System.Text.StringBuilder
                Public Sub New()
                    Me.FolderOfDownloadedFile = Szunyi.IO.Directory.Get_Folder
                    If IsNothing(Me.FolderOfDownloadedFile) = True Then Exit Sub
                    KeggID = InputBox("Enterthe KeggID of the Organims")
                    If FolderOfDownloadedFile.Exists = False Then FolderOfDownloadedFile.Create()


                End Sub
                Public Sub DoIt()
                    If IsNothing(Me.FolderOfDownloadedFile) = True Then Exit Sub

                    Dim txt = Szunyi.IO.Net.DownLoader.DownLoadText("http://www.kegg.jp/kegg-bin/show_organism?menu_type=gene_catalogs&org=" & KeggID)
                    PathwayIDs = GetPathwaysIDs(txt)

                    For Each Item In PathwayIDs
                        Dim Http = "http://www.kegg.jp/kegg-bin/download_htext?htext=" & Item & "&format=htext"
                        Dim Res = Szunyi.IO.Net.DownLoader.DownLoadText(Http)
                        Szunyi.IO.Export.SaveText(Res,
                                                  New FileInfo(Me.FolderOfDownloadedFile.FullName & "\" & Item))
                        If IsNothing(Res) = False Then Log.Append(Res).AppendLine.AppendLine()
                    Next
                    MsgBox("Ready DownLoading")
                End Sub

                Private Function GetPathwaysIDs(txt As String) As List(Of String)
                    Dim s1() = Split(txt, "/kegg-bin/get_htext?")
                    Dim out As New List(Of String)
                    For i1 = 2 To s1.Count - 1
                        Dim s2 = Split(s1(i1), Chr(34))
                        out.Add(s2.First)
                    Next
                    Return out
                End Function
            End Class
            Public Class KeggDownloader
                Dim WithEvents wb As New WebBrowser
                Private KeggID As String
                Dim PathwayIDs As New List(Of String)
                Dim FolderOfDownloadedFile As DirectoryInfo
                Dim Log As New System.Text.StringBuilder
                Public Property Type As String = Szunyi.Constants.BackGroundWork.DownLoad
                Public Property msg As String = "Download Kegg Files Are Ready"
                Public Sub New()
                    Me.FolderOfDownloadedFile = Szunyi.IO.Directory.Get_Folder
                    KeggID = InputBox("Enterthe KeggID of the Organims")
                    If FolderOfDownloadedFile.Exists = False Then FolderOfDownloadedFile.Create()


                End Sub
                Public Sub DoIt()

                    Dim txt = Szunyi.IO.Net.DownLoader.DownLoadText(Szunyi.Constants.Other_Database.KEGG.MainHttp & KeggID)
                    PathwayIDs = GetPathwaysIDs(txt)

                    For Each Item In PathwayIDs
                        Dim TheFile As New FileInfo(FolderOfDownloadedFile.FullName & "\" & Item &
                            Szunyi.Constants.Other_Database.KEGG.DownloadedPictureFileExtension)
                        If TheFile.Exists = False Then
                            Dim ThePicture = GetPicture(KeggID, Item)
                            If IsNothing(ThePicture) = False Then
                                ThePicture.Save(TheFile.FullName, Imaging.ImageFormat.Bmp)

                                If IsNothing(ThePicture) = True Then Log.Append("Error in Downloading:").Append(Item).AppendLine()

                                Dim Res = Szunyi.IO.Net.DownLoader.DownLoadAndSave(
                            Szunyi.Constants.Other_Database.KEGG.KeggXmlHttp & Item &
                            Szunyi.Constants.Other_Database.KEGG.KeggXmlFileExtension,
                            Me.FolderOfDownloadedFile.FullName & "\" & Item &
                            Szunyi.Constants.Other_Database.KEGG.DownloadedXmlFileExtension, FileMode.Create, FileAccess.Write)

                                If IsNothing(Res) = False Then Log.Append(Res).AppendLine.AppendLine()
                            Else ' error in downloading
                                Log.Append("Picture:" & Item).AppendLine.AppendLine()
                            End If
                        Else ' Exist or Not

                        End If

                    Next
                    If Log.Length > 0 Then MsgBox(Log.ToString)
                    MsgBox("Ready DownLoading")
                End Sub

                Private Function GetPicture(OrganismID As String, KeggID As String) As Image
                    Dim url As String = Szunyi.Constants.Other_Database.KEGG.PictureHttpToDownload &
                        OrganismID & "/" & KeggID & Szunyi.Constants.Other_Database.KEGG.KeggPictureFileExtension

                    Return Szunyi.IO.Net.DownLoader.GetImage(url)


                    Return Nothing
                End Function

                ''' <summary>
                ''' Return The Crude Pathways IDs
                ''' </summary>
                ''' <param name="documentText"></param>
                ''' <returns></returns>
                Private Function GetPathwaysIDs(documentText As String) As List(Of String)
                    Dim s1() = Split(documentText, ("/kegg-bin/show_pathway?"))
                    Dim IDs As New List(Of String)
                    For i1 = 1 To s1.Length - 1
                        IDs.Add(Split(s1(i1), Chr(34)).First)
                    Next
                    Return IDs
                End Function
            End Class

            Public Class KeggManipulation
                Public Shared Function LoadXml(TheFile As FileInfo) As List(Of KeggItem)
                    Dim sg As New List(Of KeggItem)
                    Using sr As New System.IO.StreamReader(TheFile.FullName)

                        Do
                            Dim line As String = sr.ReadLine
                            If line.StartsWith("    <entry id=") = True Then
                                Dim Finished As Boolean = False
                                Dim res As String = line
                                Do
                                    line = sr.ReadLine
                                    res = res & line
                                    If line.StartsWith("    </entry>") = True Then
                                        Finished = True
                                        sg.Add(New KeggItem(res))
                                    End If
                                Loop Until Finished = True
                            End If
                        Loop Until sr.EndOfStream = True


                    End Using
                    Dim out As New List(Of KeggItem)
                    For Each Item In sg
                        If Item.Type = "gene" Then out.Add(Item)
                    Next
                    Return out

                End Function

            End Class

            Public Class KeggItem
                Public Names As List(Of String)
                Public x As Integer
                Public y As Integer
                Public width As Integer
                Public height As Integer
                Public Property Color As Color
                Public BaseText As String
                Public Rec As Rectangle
                Public Rec2 As Rectangle
                Public Type As String
                Public Sub New(txt As String)
                    Me.BaseText = txt
                    Me.Names = GetName()
                    Me.x = GetX()
                    Me.y = GetY()
                    Me.width = GetWidth()
                    Me.height = GetHeight()
                    Me.Rec = New Rectangle(Me.x - Me.width / 2, Me.y - Me.height / 2, Me.width, Me.height)
                    Me.Rec2 = New Rectangle(Me.x, Me.y, Me.width, Me.height)
                End Sub

                Private Function GetName() As List(Of String)
                    Dim s1() As String = Split(Me.BaseText, "name")
                    Dim s2() = Split(s1(1), Chr(34))
                    Dim s3() = s2(1).Split(" ")
                    Me.Type = s2(3)
                    Dim out As New List(Of String)
                    For Each Item In s3
                        Dim s4 = Split(Item, ":")
                        If s4.Length > 1 Then out.Add(s4(1))
                    Next
                    Return out
                End Function
                Private Function GetX() As Integer
                    Dim s1 = Split(Me.BaseText, " x=")
                    If s1.Length = 1 Then Return 0
                    Dim s2 = Split(s1(1), " ")
                    Dim s3 = s2(0).Replace(Chr(34), "")
                    Return s3
                End Function
                Private Function GetY() As Integer
                    Dim s1 = Split(Me.BaseText, " y=")
                    If s1.Length = 1 Then Return 0
                    Dim s2 = Split(s1(1), " ")
                    Dim s3 = s2(0).Replace(Chr(34), "")
                    Return s3
                End Function
                Private Function GetWidth() As Integer
                    Dim s1 = Split(Me.BaseText, " width=")
                    If s1.Length = 1 Then Return 0
                    Dim s2 = Split(s1(1), " ")
                    Dim s3 = s2(0).Replace(Chr(34), "")
                    Return s3
                End Function
                Private Function GetHeight() As Integer
                    Dim s1 = Split(Me.BaseText, " height=")
                    If s1.Length = 1 Then Return 0
                    Dim s2 = Split(s1(1), "/")
                    Dim s3 = s2(0).Replace(Chr(34), "")
                    Return s3
                End Function
            End Class
        End Namespace
    End Namespace
End Namespace

