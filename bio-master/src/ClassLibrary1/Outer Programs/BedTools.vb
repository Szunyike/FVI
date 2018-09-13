Imports System.IO

Namespace Szunyi
    Namespace Outer_Programs
        Public Class BedTools
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
            Public Sub New(File As FileInfo, Type As SamTools_Subprogram)
                Me.Files.Add(File)
                Me.Type = Type
            End Sub
            Public Sub DoIt()
                If IsNothing(Files) = True Then Exit Sub
                Dim str As New System.Text.StringBuilder
                For Each File In Files
                    str.Append("bedtools bamtobed -cigar -i " & Szunyi.IO.Linux.Get_FileName(File))

                Next
                Windows.Forms.Clipboard.SetText(str.ToString)
            End Sub

        End Class
    End Namespace
End Namespace

