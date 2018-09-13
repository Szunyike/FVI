Namespace Szunyi
    Namespace Protein
        Public Class Aliphatic_Index
            Public Const Ala = 1
            Public Const Val = 2.9
            Public Const Leu_Ile = 3.9

            Dim A As Byte = AscW("A"c)
            Dim V As Byte = AscW("V"c)
            Dim I As Byte = AscW("I"c)
            Dim L As Byte = AscW("L"c)

            Public Function Get_Aliphatic_Indexes(Seqs As List(Of Bio.ISequence)) As List(Of Double)
                Dim out As New List(Of Double)
                For Each Seq In Seqs
                    out.Add(Get_Aliphatic_Index(Seq))
                Next
                Return out
            End Function
            Public Function Get_Aliphatic_Index(seq As Bio.ISequence) As Double
                Dim AA_Comp = Szunyi.Protein.Amino_Acid_Composition_Manipulation.Get_AA_Composition(seq)
                Dim out = AA_Comp.Percents(A) * 100
                out += AA_Comp.Percents(V) * 100 * Val
                out += AA_Comp.Percents(I) * 100 * Leu_Ile
                out += AA_Comp.Percents(L) * 100 * Leu_Ile
                Return out

            End Function
        End Class
    End Namespace
End Namespace

