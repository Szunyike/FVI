Imports System.IO

Namespace Szunyi.DNA
    Public Class Chimera
        Public Iterator Function Get_ReadIDs(Files As List(Of FileInfo)) As IEnumerable(Of List(Of String))
            For Each File In Files

            Next
        End Function
        ''' <summary>
        ''' Get All Seq ID, Distinct and sort
        ''' </summary>
        ''' <param name="file"></param>
        ''' <returns></returns>
        Public Shared Function Get_ReadIDs(file As FileInfo) As List(Of String)
            Dim out As New List(Of String)
            For Each SAM In Szunyi.BAM.Bam_Basic_IO.Import.Parse(file)
                out.Add(SAM.QuerySequence.ID)
            Next
            out = out.Distinct.ToList
            out.Sort()
            Return out
        End Function

        Public Shared Function Get_ReadIDs_From_Tsv(files As List(Of FileInfo)) As Dictionary(Of FileInfo, List(Of String))
            Dim out As New Dictionary(Of FileInfo, List(Of String))
            For Each File In files
                Dim k = Szunyi.IO.Import.Text.Parse(File).ToList
                out.Add(File, k)
            Next
            Return out
        End Function
    End Class
End Namespace

