Namespace Szunyi
    Namespace Alignment
        Public Class Aligners
            Public Property SM_Aligner_PolyA As Bio.Algorithms.Alignment.SmithWatermanAligner
            Public Property SM_Aligner_SMRT As Bio.Algorithms.Alignment.SmithWatermanAligner

            Public Sub New()
                SM_Aligner_PolyA = New Bio.Algorithms.Alignment.SmithWatermanAligner
                Dim SM As New Bio.SimilarityMatrices.DiagonalSimilarityMatrix(2, -2)
                SM_Aligner_PolyA.GapOpenCost = -2
                SM_Aligner_PolyA.GapExtensionCost = -2
                SM_Aligner_PolyA.SimilarityMatrix = SM

                SM_Aligner_SMRT = New Bio.Algorithms.Alignment.SmithWatermanAligner
                SM_Aligner_SMRT.GapOpenCost = -2
                SM_Aligner_SMRT.GapExtensionCost = -2
                SM_Aligner_SMRT.SimilarityMatrix = SM
            End Sub

        End Class
    End Namespace
End Namespace

