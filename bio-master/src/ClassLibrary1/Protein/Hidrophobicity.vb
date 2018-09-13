Namespace Szunyi
    Namespace Protein
        Public Class Hidrophobicity
            Public Property AAs_With_Values As New Szunyi.Protein.AAs_Properties
            Public Function Get_HidroHidrophobicity_Indexes(Seqs As List(Of Bio.ISequence),
                                                            Optional Hidrophobicity_Index_Name As String = "KyteDoolittle") As List(Of Double)
                Dim Out As New List(Of Double)
                For Each seq In Seqs
                    Out.Add(Get_HidroHidrophobicity_Index(seq, Hidrophobicity_Index_Name))
                Next
                Return Out
            End Function
            Public Function Get_HidroHidrophobicity_Index(Seq As Bio.ISequence, Hidrophobicity_Index_Name As String) As Double
                Dim Out As Double
                Dim AAs_With_Values = Me.AAs_With_Values.AA_Values(Hidrophobicity_Index_Name)
                For Each s In Seq
                    If AAs_With_Values.Values_By_AA.ContainsKey(s) = True Then
                        Out += AAs_With_Values.Values_By_AA(s)
                    End If

                Next

                Return Out / Szunyi.Sequences.SequenceManipulation.Counts.Get_Sequence_Length_Wo_Ter_And_Gap_Symbols(Seq)
            End Function
        End Class

    End Namespace
End Namespace

