Namespace Szunyi
    Namespace Protein

        Public Class Extinction_Coefficient
            Public Const Ext_Tyr As Integer = 1490
            Public Const Ext_Cys As Integer = 125
            Public Const Ext_Trp As Integer = 5500

            Dim C As Byte = AscW("C"c)
            Dim W As Byte = AscW("W"c)
            Dim Y As Byte = AscW("Y"c)

            Public Shared Property MW_Calculator As New Szunyi.Protein.MW

#Region "Absorbance"
            Public Function Get_Absorbances_Fully_Reduced_Cysteines(Seqs As List(Of Bio.ISequence)) As List(Of Double)
                Dim out As New List(Of Double)
                For Each Seq In Seqs
                    out.Add(Get_Absorbance_Fully_Reduced_Cysteines(Seq))
                Next
                Return out
            End Function

            Public Function Get_Absorbance_Fully_Reduced_Cysteines(Seq As Bio.ISequence) As Double

                Dim E = Get_Excitation_Fully_Reduced_Cysteine(Seq)
                Dim pMW = MW_Calculator.GetMW(Seq)
                Return E / pMW
            End Function

            Public Function Get_Absorbances_Oxidized_Cysteines(Seqs As List(Of Bio.ISequence)) As List(Of Double)
                Dim out As New List(Of Double)
                For Each Seq In Seqs
                    out.Add(Get_Absorbance_Oxidized_Cysteines(Seq))
                Next
                Return out
            End Function
            Public Function Get_Absorbance_Oxidized_Cysteines(Seq As Bio.ISequence) As Double

                Dim E = Get_Excitation_Oxidized_Cysteine(Seq)
                Dim pMW = MW_Calculator.GetMW(Seq)
                Return E / pMW
            End Function

#End Region

#Region "Excitation"
            Public Function Get_Excitation_Fully_Reduced_Cysteines(Seqs As List(Of Bio.ISequence)) As List(Of Double)
                Dim out As New List(Of Double)
                For Each Seq In Seqs
                    out.Add(Get_Excitation_Fully_Reduced_Cysteine(Seq))
                Next
                Return out
            End Function
            Public Function Get_Excitation_Fully_Reduced_Cysteine(Seq As Bio.ISequence) As Double

                Dim AA_Comp = Szunyi.Protein.Amino_Acid_Composition_Manipulation.Get_AA_Composition(Seq)

                Return Ext_Trp * AA_Comp.Counts(W) + Ext_Tyr * AA_Comp.Counts(Y)

            End Function

            Public Function Get_Excitation_Oxidized_Cysteines(Seqs As List(Of Bio.ISequence)) As List(Of Double)
                Dim out As New List(Of Double)
                For Each Seq In Seqs
                    out.Add(Get_Excitation_Oxidized_Cysteine(Seq))
                Next
                Return out
            End Function
            Public Function Get_Excitation_Oxidized_Cysteine(Seq As Bio.ISequence) As Double
                Dim AA_Comp = Szunyi.Protein.Amino_Acid_Composition_Manipulation.Get_AA_Composition(Seq)

                Return Ext_Cys * AA_Comp.Counts(C) / 2 + Ext_Trp * AA_Comp.Counts(W) + Ext_Tyr * AA_Comp.Counts(Y)
            End Function
#End Region

        End Class

    End Namespace
End Namespace

