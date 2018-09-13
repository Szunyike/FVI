Imports Bio.IO.SAM

Namespace Szunyi.BAM
    Public Class Trim
        Public Shared Sub Remove_Five_SH(Sam As Bio.IO.SAM.SAMAlignedSequence)
            Dim MdCigars = Szunyi.BAM.CIGAR.Get_CIGARS(Sam.CIGAR)
            If MdCigars.First.Key = "S" Then
                Dim Length = MdCigars.First.Value

                MdCigars.RemoveAt(0)

                Try
                    Dim kj = Sam.QuerySequence.GetSubSequence(Length, Sam.QuerySequence.Count - Length)
                    Sam.QuerySequence = kj
                    Sam.CIGAR = Szunyi.BAM.CIGAR.Get_CIGAR_String(MdCigars)

                Catch
                    Dim hg As Int16 = 54
                End Try

            End If
        End Sub

        Public Shared Sub Remove_Three_SH(ByRef Sam As Bio.IO.SAM.SAMAlignedSequence)
            Dim MdCigars = Szunyi.BAM.CIGAR.Get_CIGARS(Sam.CIGAR)
            If MdCigars.Last.Key = "S" Then
                Dim Length = MdCigars.Last.Value
                MdCigars.RemoveAt(MdCigars.Count - 1)

                Try
                    Dim kjd = Sam.QuerySequence.GetSubSequence(0, Sam.QuerySequence.Count - Length)
                    Sam.CIGAR = Szunyi.BAM.CIGAR.Get_CIGAR_String(MdCigars)


                    Sam.QuerySequence = kjd
                Catch ex As Exception
                    Dim ald As Int16 = 54
                End Try

            End If
        End Sub

        Public Shared Function TrimH(CIGAR As String) As String
            Dim SH = Szunyi.BAM.CIGAR.Get_CIGARS(CIGAR)
            For i1 = SH.Count - 1 To 0 Step -1
                If SH(i1).Key = "H" Then
                    SH.RemoveAt(i1)
                End If
            Next
            Return Szunyi.BAM.CIGAR.Get_CIGAR_String(SH)
        End Function

        Public Shared Function TrimH(SAM As SAMAlignedSequence) As SAMAlignedSequence
            SAM.CIGAR = TrimH(SAM.CIGAR)
            Return SAM

        End Function
        Public Shared Function TrimS(SAM As SAMAlignedSequence, max_NofNA_toMaintain As Integer) As SAMAlignedSequence
            Dim First_S_Length = Szunyi.BAM.CIGAR.Get_First_S_Length(SAM)
            Dim nofNA_to_Remove_5 As Integer = 0
            If First_S_Length > max_NofNA_toMaintain Then
                nofNA_to_Remove_5 = First_S_Length - max_NofNA_toMaintain
            End If

            Dim Last_S_Length = Szunyi.BAM.CIGAR.Get_Last_S_Length(SAM)
            Dim nofNA_to_Remove_3 As Integer = 0
            If Last_S_Length > max_NofNA_toMaintain Then
                nofNA_to_Remove_3 = Last_S_Length - max_NofNA_toMaintain
            End If

            SAM.QuerySequence = SAM.QuerySequence.GetSubSequence(nofNA_to_Remove_5, SAM.QuerySequence.Count - nofNA_to_Remove_5 - nofNA_to_Remove_3)
            SAM.CIGAR = Szunyi.BAM.CIGAR.Modify_S(SAM, nofNA_to_Remove_5, nofNA_to_Remove_3)

            Dim MD = Szunyi.BAM.BAM_Optional_Filed_Manipulation.Get_MD(SAM)
            Return SAM
        End Function
        Public Shared Function TrimS_S(SAMs As List(Of SAMAlignedSequence), max_NofNA_toMaintain As Integer) As List(Of SAMAlignedSequence)
            Dim out As New List(Of SAMAlignedSequence)
            For Each SAM In SAMs
                out.Add(TrimS(SAM, max_NofNA_toMaintain))
            Next

            Return out
        End Function
    End Class
End Namespace

