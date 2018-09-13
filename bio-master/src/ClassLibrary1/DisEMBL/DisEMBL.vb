Imports System.IO
Imports System.Text
Imports Bio
Imports ClassLibrary1.Szunyi.Outer_Programs
Imports ClassLibrary1.Szunyi.Sequence_Analysis

Namespace Szunyi
    Namespace Protein
        Public Class DisEMBL
            Public Property res As Sequences_Ranges
            Public Property log As New StringBuilder


            Public Property Parameters As New Dictionary(Of String, DisEMBL_Parameters)
            Public ReadOnly Property tmpFileName As String = "tmp.txt"
            Public ReadOnly Property tmpFileNameII As String = "tmpII.txt"
            Public Sub New()
                Dim Coil_Paramter = New DisEMBL_Parameters(8, 8, 4, 1.2, 0.516, 0.43)
                Dim Remark_Parameter = New DisEMBL_Parameters(8, 8, 4, 1.2, 0.6, 0.5)
                Dim Hot_Loops_Parameter As New DisEMBL_Parameters(8, 8, 4, 1.4, 0.1204, 0.086)

                Me.Parameters.Add("Coil", Coil_Paramter)
                Me.Parameters.Add("Remark", Remark_Parameter)
                Me.Parameters.Add("Hot_Loops", Hot_Loops_Parameter)
            End Sub
            Public Function Get_results(res As Szunyi.Sequence_Analysis.Sequences_Ranges) As Szunyi.Sequence_Analysis.Sequences_Ranges
                Me.res = res
                For Each Item In Me.res.Ranges
                    Get_Result(Item, log)
                Next
                Return Me.res
            End Function
            Public Function Get_Result(ByRef Res As Sequence_Ranges,
                                       ByRef log As StringBuilder
                                       ) As Szunyi.Sequence_Analysis.Sequence_Ranges

                Dim Basic_Text = Szunyi.Outer_Programs.DisEMBL.Get_Basic_Values(Res.Seq)

                Dim Coils_Raw = Text.TableManipulation.Values_From_Txt.Get_Columns_Values_As_Double(Basic_Text, 0, 0)

                Dim HOTLOOPS_raw = Text.TableManipulation.Values_From_Txt.Get_Columns_Values_As_Double(Basic_Text, 1, 0)

                Dim REM465_raw = Text.TableManipulation.Values_From_Txt.Get_Columns_Values_As_Double(Basic_Text, 2, 0)

                Dim Rem465_Parameter = Me.Parameters("Remark")
                Dim Coil_Parameter = Me.Parameters("Coil")
                Dim Hot_Loop_Paramater = Me.Parameters("Hot_Loops")
                Dim REM465_smooth = SavitzkyGolay.SavitzkyGolay(Rem465_Parameter.smooth_frame, 0, REM465_raw)
                Dim COILS_smooth = SavitzkyGolay.SavitzkyGolay(Coil_Parameter.smooth_frame, 0, Coils_Raw)
                Dim HOTLOOPS_smooth = SavitzkyGolay.SavitzkyGolay(Hot_Loop_Paramater.smooth_frame, 0, HOTLOOPS_raw)

                Dim COILS_Slices = Szunyi.Sequence_Analysis.Slices.Get_Slices(COILS_smooth,
                                              Coil_Parameter)

                Dim COILS_Result As New Sequence_Ranges_With_Values(COILS_Slices,
                      Coil_Parameter, Coil_Parameter.Threshold, COILS_smooth)


                Res.Analysis.Add(Szunyi.Constants.Sequence_Analysis.Disordered_by_Loops_coils_definition, COILS_Result)

                Dim REM465_Slices = Szunyi.Sequence_Analysis.Slices.Get_Slices(REM465_smooth, Rem465_Parameter)

                Dim REM465_Result As New Sequence_Ranges_With_Values(REM465_Slices,
                                  Rem465_Parameter, Rem465_Parameter.Threshold, REM465_smooth)

                Res.Analysis.Add(Szunyi.Constants.Sequence_Analysis.Disordered_by_Remark_465_definition, REM465_Result)


                Dim Hot_Loops_Slices = Szunyi.Sequence_Analysis.Slices.Get_Slices(HOTLOOPS_smooth, Hot_Loop_Paramater)

                Dim Hot_Loops_Result As New Sequence_Ranges_With_Values(Hot_Loops_Slices,
                                   Hot_Loop_Paramater, Hot_Loop_Paramater.Threshold, HOTLOOPS_smooth)

                Res.Analysis.Add(Szunyi.Constants.Sequence_Analysis.Disordered_by_Hot_loops_definition, Hot_Loops_Result)

                Return Res
            End Function





        End Class

        Public Class DisEMBL_Parameters
            Public Property smooth_frame As Integer = 8
            Public Property peak_frame As Integer = 8
            Public Property join_frame As Integer = 4
            Public Property fold As Double
            Public Property Threshold As Double
            Public Property Expect_Value As Double
            Public Sub New(smooth_frame As Integer,
                            peak_frame As Integer,
                            join_frame As Integer,
                           fold As Double,
                           Threshold As Double,
                           Expect_Value As Double)
                Me.smooth_frame = smooth_frame
                Me.peak_frame = peak_frame
                Me.join_frame = join_frame
                Me.fold = fold
                Me.Threshold = Threshold
                Me.Expect_Value = Expect_Value


            End Sub
        End Class
    End Namespace
End Namespace

