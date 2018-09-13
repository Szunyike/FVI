Imports System.IO

Namespace Szunyi
    Namespace Blat
        Public Class BlatFilter
            Public Shared Function GetPerfect(BlatResults As List(Of BlatResult)) As List(Of BlatResult)

                Dim res = From x In BlatResults Where x.match = x.Q_Size

                If res.Count > 0 Then
                    Return res.ToList
                Else
                    Return New List(Of BlatResult)
                End If
            End Function
        End Class
        Public Class BlatImport
            Public Shared Function Import(Files As List(Of FileInfo)) As List(Of BlatResult)
                Dim Out As New List(Of BlatResult)
                For Each File In Files
                    Out.AddRange(Import(File))
                Next
                Return Out

            End Function
            Public Shared Function Import(File As FileInfo) As List(Of BlatResult)
                Dim out As New List(Of BlatResult)
                For Each s In Szunyi.IO.Import.Text.ParseToArray(File, vbTab, 5)
                    out.Add(New BlatResult(s))
                Next
                Return out
            End Function
        End Class
        Public Class BlatResult
            Public Property match As Integer
            Public Property mis_match As Integer
            Public Property rep_match As Integer
            Public Property Ns As Integer
            Public Property Q_Gap_Count As Integer
            Public Property Q_Gap_Bases As Integer
            Public Property T_Gap_Count As Integer
            Public Property T_Gap_bases As Integer
            Public Property Strand As String
            Public Property Q_Name As String
            Public Property Q_Size As Integer
            Public Property Q_Start As Integer
            Public Property Q_End As Integer
            Public Property T_Name As String
            Public Property T_Size As Integer
            Public Property T_Start As Integer
            Public Property T_End As Integer

            Public Property Block_Count As Integer
            Public Property Block_Sizes As New List(Of Long)
            Public Property Q_Starts As New List(Of Long)
            Public Property T_Starts As New List(Of Long)
            Public Property Q_Seq As New List(Of Bio.Sequence)
            Public Property T_Seq As New List(Of Bio.Sequence)
            Public Sub New(s() As String)
                match = s(0)
                mis_match = s(1)
                rep_match = s(2)
                Ns = s(3)
                Q_Gap_Count = s(4)
                Q_Gap_Bases = s(5)
                T_Gap_Count = s(6)
                T_Gap_bases = s(7)
                Strand = s(8)
                Q_Name = s(9)
                Q_Size = s(10)
                Q_Start = s(11)
                Q_End = s(12)
                T_Name = s(13)
                T_Size = s(14)
                T_Start = s(15)
                T_End = s(16)
                Block_Count = s(17)


                Block_Sizes.AddRange(Szunyi.Text.General.SplitIntoLong(s(18), ","))
                Q_Starts.AddRange(Szunyi.Text.General.SplitIntoLong(s(19), ","))
                T_Starts.AddRange(Szunyi.Text.General.SplitIntoLong(s(20), ","))
                Dim qSeqs = Split(s(21), ",")
                Q_Seq = Szunyi.Sequences.SequenceManipulation.GetSequences.FromStrings(qSeqs)
                Dim TSeqs = Split(s(22), ",")
                T_Seq = Szunyi.Sequences.SequenceManipulation.GetSequences.FromStrings(TSeqs)
            End Sub
        End Class
    End Namespace

End Namespace

