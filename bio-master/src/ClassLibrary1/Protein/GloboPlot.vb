Imports System.Text
Imports Bio
Imports ClassLibrary1.Szunyi.Sequence_Analysis

Namespace Szunyi
    Namespace Protein
        Public Class GloboProt
            Public Property AAs_Properties As New Szunyi.Protein.AAs_Properties
            Public Property AAs_With_Values As AAs_With_Values
            Public Property Log As New System.Text.StringBuilder
            Public Property Parameters As GloboProt_Parameters = New GloboProt_Parameters
            Public Property Res As Sequences_Ranges

            Public Function Get_Results(res As Sequences_Ranges,
                                       Optional Hidrophobicity_Index_Name As String = Szunyi.Constants.Protein.Hidrophobicity_Indexes.Russel_Linding) As Sequences_Ranges
                Me.AAs_With_Values = Me.AAs_Properties.AA_Values(Hidrophobicity_Index_Name)
                For Each SRs In res.Ranges
                    If SRs.Analysis.ContainsKey(Szunyi.Constants.Sequence_Analysis.Disordered_by_Russell_Linding_definition) Then
                        SRs.Analysis.Remove(Szunyi.Constants.Sequence_Analysis.Disordered_by_Russell_Linding_definition)
                    End If
                    SRs.Analysis.Add(Szunyi.Constants.Sequence_Analysis.Disordered_by_Russell_Linding_definition, Get_Result(SRs, Log))
                Next
                Return res
            End Function
            Public Function Get_Sum(Seq As Bio.Sequence, RL As Dictionary(Of Byte, Double)) As List(Of Double)
                Dim Sums As New List(Of Double)
                Dim p = 1
                Dim Parameter As Double = 0
                Dim Sum As Double = 0
                For p = 0 To Seq.Count - 1
                    If RL.ContainsKey(Seq(p)) = True Then
                        If p = 0 Then
                            Sum = RL(Seq(p))
                        Else
                            Sum += RL(Seq(p)) ' * System.Math.Log10(p + 1)
                        End If
                    End If
                    Sums.Add(Sum)
                Next

                Return Sums
            End Function

            Private Function Get_Result(SRs As Sequence_Ranges,
                                        log As StringBuilder) As Szunyi.Sequence_Analysis.Sequence_Ranges_With_Values
                Dim sum_vector = Get_Sum(SRs.Seq, Me.AAs_With_Values.Values_By_AA)

                Dim smooth = Szunyi.Outer_Programs.SavitzkyGolay.SavitzkyGolay(Me.Parameters.smoothFrame, 0, sum_vector)
                Dim dydx_vector = Szunyi.Outer_Programs.SavitzkyGolay.SavitzkyGolay(Me.Parameters.smoothFrame, 1, sum_vector)

                Dim newHead As New List(Of Double)
                Dim newTail As New List(Of Double)

                ' test
                For i1 = 0 To Me.Parameters.smoothFrame - 1
                    Try
                        dydx_vector(i1) = ((sum_vector(i1 + 1) - sum_vector(i1)) / 2)
                    Catch ex As Exception
                        dydx_vector(i1) = ((sum_vector(i1) - sum_vector(i1 - 1)) / 2)
                    End Try

                Next
                dydx_vector(Me.Parameters.smoothFrame - 1) = dydx_vector(Me.Parameters.smoothFrame - 2)
                For i1 = dydx_vector.Count - 2 To dydx_vector.Count - Me.Parameters.smoothFrame Step -1
                    Try
                        dydx_vector(i1) = ((sum_vector(i1 + 1) - sum_vector(i1)) / 2)
                    Catch ex As Exception
                        dydx_vector(i1) = ((sum_vector(i1) - sum_vector(i1 - 1)) / 2)
                    End Try

                Next
                dydx_vector(dydx_vector.Count - 1) = dydx_vector(dydx_vector.Count - 2)
                If SRs.Seq.ID = "MTR_0107s0100.1" Then
                    Dim alf As Int16 = 54
                End If
                Dim sg = Szunyi.Sequence_Analysis.Slices.Get_Slices_Above_Threshold(dydx_vector,
                                                                    0,
                                                                    Me.Parameters.DOM_joinFrame,
                                                                    Me.Parameters.DOM_peakFrame)

                Dim sg2 = Szunyi.Sequence_Analysis.Slices.Get_Slices_Below_Threshold(dydx_vector,
                                                                    0,
                                                                    Me.Parameters.DIS_joinFrame,
                                                                    Me.Parameters.DIS_peakFrame)

                Dim x As New Szunyi.Sequence_Analysis.Sequence_Ranges_With_Values(sg, Me.Parameters, 0, dydx_vector.ToList)

                Return x
            End Function


        End Class
        Public Class Russell_Linding_Parameters
            Public Property Name As String = "Russell/Linding Parameters"
            Public Property Amino_Acid_w_Value As New Dictionary(Of Byte, Double)
            Public Sub New()
                Amino_Acid_w_Value.Add(AscW("N"c), 0.229885057471264)
                Amino_Acid_w_Value.Add(AscW("P"c), 0.552316012226663)
                Amino_Acid_w_Value.Add(AscW("Q"c), -0.187676577424997)
                Amino_Acid_w_Value.Add(AscW("A"c), -0.261538461538462)
                Amino_Acid_w_Value.Add(AscW("R"c), -0.176592654077609)

                Amino_Acid_w_Value.Add(AscW("S"c), 0.142883029808825)
                Amino_Acid_w_Value.Add(AscW("C"c), -0.0151515151515152)
                Amino_Acid_w_Value.Add(AscW("T"c), 0.00887797506611258)
                Amino_Acid_w_Value.Add(AscW("D"c), 0.227629796839729)
                Amino_Acid_w_Value.Add(AscW("E"c), -0.204684629516228)

                Amino_Acid_w_Value.Add(AscW("V"c), -0.386174834235195)
                Amino_Acid_w_Value.Add(AscW("F"c), -0.225572305974316)
                Amino_Acid_w_Value.Add(AscW("W"c), -0.243375458622095)
                Amino_Acid_w_Value.Add(AscW("G"c), 0.433225711769886)
                Amino_Acid_w_Value.Add(AscW("H"c), -0.00121743364986608)

                Amino_Acid_w_Value.Add(AscW("Y"c), -0.20750516775322)
                Amino_Acid_w_Value.Add(AscW("I"c), -0.422234699606962)
                Amino_Acid_w_Value.Add(AscW("K"c), -0.100092289621613)
                Amino_Acid_w_Value.Add(AscW("L"c), -0.337933495925287)
                Amino_Acid_w_Value.Add(AscW("M"c), -0.225903614457831)

                '         {'N':0.229885057471264,'P':0.552316012226663,'Q':-0.187676577424997,'A':-0.261538461538462,'R':-0.176592654077609, \
                'S':0.142883029808825,'C':-0.0151515151515152,'T':0.00887797506611258,'D':0.227629796839729,'E':-0.204684629516228, \
                'V':-0.386174834235195,'F':-0.225572305974316,'W':-0.243375458622095,'G':0.433225711769886,'H':-0.00121743364986608, \
                'Y':-0.20750516775322,'I':-0.422234699606962,'K':-0.100092289621613,'L':-0.337933495925287,'M':-0.225903614457831} 
            End Sub
        End Class
        Public Class GloboProt_Parameters
            Public Property smoothFrame As Integer = 10
            Public Property DOM_joinFrame As Integer = 3
            Public Property DOM_peakFrame As Integer = 5
            Public Property DIS_joinFrame As Integer = 3
            Public Property DIS_peakFrame As Integer = 5
        End Class
    End Namespace
End Namespace
