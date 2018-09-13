Imports Bio.IO.SAM

Namespace Szunyi.BAM
    Public Class GroupBy_Firsts
        ''' <summary>
        ''' Group SAMs by Query Name
        ''' </summary>
        ''' <param name="Sams"></param>
        ''' <returns></returns>
        Public Shared Function ReadID(Sams As List(Of SAMAlignedSequence)) As List(Of SAMAlignedSequence)
            Dim out = From x In Sams Group By x.QName Into Group

            Dim res As New List(Of SAMAlignedSequence)
            For Each g In out
                res.Add(g.Group.First)
            Next
            Return res
        End Function
        ''' <summary>
        ''' Group Sams By Reference Name
        ''' </summary>
        ''' <param name="Sams"></param>
        ''' <returns></returns>
        Public Shared Function RNameID(Sams As List(Of SAMAlignedSequence)) As List(Of SAMAlignedSequence)
            Dim out = From x In Sams Group By x.RName Into Group

            Dim res As New List(Of SAMAlignedSequence)
            For Each g In out
                res.Add(g.Group.First)
            Next
            Return res
        End Function
        ''' <summary>
        ''' Group SAMs by Pos
        ''' </summary>
        ''' <param name="Sams"></param>
        ''' <returns></returns>
        Public Shared Function ByPos_Cigar(Sams As List(Of SAMAlignedSequence)) As List(Of SAMAlignedSequence)
            Dim out = From x In Sams Group By x.Pos, x.RefEndPos, x.CIGAR Into Group

            Dim res As New List(Of SAMAlignedSequence)
            For Each g In out
                res.Add(g.Group.First)
            Next
            Return res
        End Function
        ''' <summary>
        ''' Group SAMs by Pos
        ''' </summary>
        ''' <param name="Sams"></param>
        ''' <returns></returns>
        Public Shared Function ByPos_Cigar_Flag(Sams As List(Of SAMAlignedSequence)) As List(Of SAMAlignedSequence)
            Dim out = From x In Sams Group By x.Pos, x.RefEndPos, x.CIGAR, x.Flag Into Group

            Dim res As New List(Of SAMAlignedSequence)
            For Each g In out
                res.Add(g.Group.First)
            Next
            Return res
        End Function
        ''' <summary>
        ''' Group SAMs by Pos
        ''' </summary>
        ''' <param name="Sams"></param>
        ''' <returns></returns>
        Public Shared Function ByPos(Sams As List(Of SAMAlignedSequence)) As List(Of SAMAlignedSequence)
            Dim out = From x In Sams Group By x.Pos, x.RefEndPos Into Group

            Dim res As New List(Of SAMAlignedSequence)
            For Each g In out
                res.Add(g.Group.First)
            Next
            Return res
        End Function

    End Class
    Public Class GroupBy
        ''' <summary>
        ''' Group SAMs by Query Name
        ''' </summary>
        ''' <param name="Sams"></param>
        ''' <returns></returns>
        Public Shared Iterator Function ReadID(Sams As List(Of SAMAlignedSequence)) As IEnumerable(Of List(Of SAMAlignedSequence))

            Dim wReadIDs = From x In Sams Select x Where IsNothing(x) = False
            Dim out = From x In wReadIDs Group By x.QName Into Group

            For Each g In out
                Yield g.Group.ToList
            Next
        End Function
        ''' <summary>
        ''' Group Sams By Reference Name
        ''' </summary>
        ''' <param name="Sams"></param>
        ''' <returns></returns>
        Public Shared Iterator Function RNameID(Sams As List(Of SAMAlignedSequence)) As IEnumerable(Of List(Of SAMAlignedSequence))
            Dim out = From x In Sams Group By x.RName Into Group

            For Each g In out
                Yield g.Group.ToList
            Next
        End Function
        ''' <summary>
        ''' Group SAMs by Pos
        ''' </summary>
        ''' <param name="Sams"></param>
        ''' <returns></returns>
        Public Shared Iterator Function ByPos_Cigar(Sams As List(Of SAMAlignedSequence)) As IEnumerable(Of List(Of SAMAlignedSequence))
            Dim out = From x In Sams Group By x.Pos, x.RefEndPos, x.CIGAR Into Group

            For Each g In out
                Yield g.Group.ToList
            Next
        End Function
        ''' <summary>
        ''' Group SAMs by Pos
        ''' </summary>
        ''' <param name="Sams"></param>
        ''' <returns></returns>
        Public Shared Iterator Function ByPos_Cigar_Flag(Sams As List(Of SAMAlignedSequence)) As IEnumerable(Of List(Of SAMAlignedSequence))
            Dim out = From x In Sams Group By x.Pos, x.RefEndPos, x.CIGAR, x.Flag Into Group

            For Each g In out
                Yield g.Group.ToList
            Next
        End Function
        ''' <summary>
        ''' Group SAMs by Pos
        ''' </summary>
        ''' <param name="Sams"></param>
        ''' <returns></returns>
        Public Shared Iterator Function ByPos(Sams As List(Of SAMAlignedSequence)) As IEnumerable(Of List(Of SAMAlignedSequence))
            Dim out = From x In Sams Group By x.Pos, x.RefEndPos Into Group

            For Each g In out
                Yield g.Group.ToList
            Next
        End Function
        Public Shared Function ByPos_ToList(Sams As List(Of SAMAlignedSequence)) As List(Of List(Of SAMAlignedSequence))
            Dim Res As New List(Of List(Of SAMAlignedSequence))
            Dim out = From x In Sams Group By x.Pos, x.RefEndPos Into Group

            For Each g In out
                Res.Add(g.Group.ToList)
            Next
            Return Res
        End Function
        ''' <summary>
        ''' Group SAMs by Pos
        ''' </summary>
        ''' <param name="Sams"></param>
        ''' <returns></returns>
        Public Shared Function ByPos_Cigar_ToList(Sams As List(Of SAMAlignedSequence)) As List(Of List(Of SAMAlignedSequence))
            Dim Res As New List(Of List(Of SAMAlignedSequence))
            Dim out = From x In Sams Group By x.Pos, x.RefEndPos, x.CIGAR Into Group

            For Each g In out
                Res.Add(g.Group.ToList)
            Next
            Return Res
        End Function
        ''' <summary>
        ''' Group SAMs by Pos
        ''' </summary>
        ''' <param name="Sams"></param>
        ''' <returns></returns>
        Public Shared Function ByPos_Cigar_Flag_ToList(Sams As List(Of SAMAlignedSequence)) As List(Of List(Of SAMAlignedSequence))
            Dim Res As New List(Of List(Of SAMAlignedSequence))
            Dim out = From x In Sams Group By x.Pos, x.RefEndPos, x.CIGAR, x.Flag Into Group

            For Each g In out
                Res.Add(g.Group.ToList)
            Next
            Return Res
        End Function
    End Class
End Namespace

