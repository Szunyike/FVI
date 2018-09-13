Imports System.IO
Imports System.Text
Imports System.Windows.Forms
Imports Bio

Namespace Szunyi
    Namespace Blast
        Namespace Helper
            Public Class StringWithCounts
                Public Property Name As String
                Public Property Count As Integer
            End Class
            Public Class StringwCountsComparer
                Implements IComparer(Of StringWithCounts)

                Public Function Compare(x As StringWithCounts, y As StringWithCounts) As Integer Implements IComparer(Of StringWithCounts).Compare
                    Return x.Name.CompareTo(y.Name)
                End Function
            End Class
            Public Class Paths

                ''' <summary>
                ''' Create App Directories in locac\AppData folder
                ''' </summary>
                ''' <remarks></remarks>

                Public Shared Function GetDBFiles() As List(Of FileInfo)
                    Dim Dir As New DirectoryInfo(My.Resources.BlastDbPath)
                    Dim Files = Dir.GetFiles
                    Return Files.ToList
                End Function
                Public Shared Function GetResultFiles() As List(Of FileInfo)
                    Dim Dir As New DirectoryInfo(My.Resources.BlastResultPath)
                    Dim Files = Dir.GetFiles
                    Return Files.ToList
                End Function


                Public Shared Function GetOriginalQueryFile(s As String) As FileInfo
                    Dim i = s.LastIndexOf("_") + 1
                    Dim s1 = s.Substring(i, s.Length - i)
                    s1 = My.Resources.BlastFastaFilesPath & s1
                    s1 = s1.Replace(".xml", ".fa")
                    Return New FileInfo(s1)
                End Function

                Public Shared Function GetFastaPath() As String
                    Return My.Resources.BlastFastaFilesPath
                End Function
            End Class
        End Namespace


    End Namespace
End Namespace

