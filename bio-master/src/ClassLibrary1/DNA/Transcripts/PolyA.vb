Imports System.IO
Imports System.Text.RegularExpressions
Imports Bio.IO.SAM

Namespace Szunyi.mRNA
    Public Enum PolyA_Type
        polyA = 1
        polyT = 2
        None = 3
        Both = 4
        polyA_falsePolyT = 5
        polyT_falsePolyA = 6
        All = 7
        polyAT = 8
        For_TSS = 9
        For_PAS = 10
    End Enum
    Public Enum False_PolyAT_Discovering
        From_Beginning = 1
        Nof_Consecutive_AT = 2
        SM_aligner = 3
        Percent_of_AT = 4
    End Enum
    Public Class PolyA
#Region "Reverse"
        ''' <summary>
        ''' Return Reverse Orientation
        ''' </summary>
        ''' <param name="Orientation"></param>
        ''' <returns></returns>
        Public Shared Function Reverse(Orientation As PolyA_Type) As PolyA_Type
            Select Case Orientation
                Case PolyA_Type.polyA
                    Return PolyA_Type.polyT
                Case PolyA_Type.polyA_falsePolyT
                    Return PolyA_Type.polyT_falsePolyA
                Case PolyA_Type.polyT
                    Return PolyA_Type.polyA
                Case PolyA_Type.polyT_falsePolyA
                    Return PolyA_Type.polyA_falsePolyT
                Case Else
                    Return Orientation
            End Select
        End Function

        ''' <summary>
        ''' Return Reverse Orientation or Orientation if IsReverse = false
        ''' </summary>
        ''' <param name="Orientation"></param>
        ''' <returns></returns>
        Public Shared Function Reverse(Orientation As PolyA_Type, IsReverse As Boolean) As PolyA_Type
            If IsReverse = False Then Return Orientation
            Select Case Orientation
                Case PolyA_Type.polyA
                    Return PolyA_Type.polyT
                Case PolyA_Type.polyA_falsePolyT
                    Return PolyA_Type.polyT_falsePolyA
                Case PolyA_Type.polyT
                    Return PolyA_Type.polyA
                Case PolyA_Type.polyT_falsePolyA
                    Return PolyA_Type.polyA_falsePolyT
                Case Else
                    Return Orientation
            End Select
        End Function

        ''' <summary>
        ''' Return Reverse Orientation or Orientation if IsReverse = false
        ''' </summary>
        ''' <param name="Orientation"></param>
        ''' <returns></returns>
        Public Shared Function Reverse(Orientation As PolyA_Type, IsReverse As Szunyi.Transcipts.Adaptor_Pair) As PolyA_Type
            If IsReverse.isReverse = False Then Return Orientation
            Select Case Orientation
                Case PolyA_Type.polyA
                    Return PolyA_Type.polyT
                Case PolyA_Type.polyA_falsePolyT
                    Return PolyA_Type.polyT_falsePolyA
                Case PolyA_Type.polyT
                    Return PolyA_Type.polyA
                Case PolyA_Type.polyT_falsePolyA
                    Return PolyA_Type.polyA_falsePolyT
                Case Else
                    Return Orientation
            End Select
        End Function
#End Region


        Public Shared Function Get_Dictionary_wListOfSams() As Dictionary(Of PolyA_Type, List(Of Bio.IO.SAM.SAMAlignedSequence))
            Dim out As New Dictionary(Of PolyA_Type, List(Of Bio.IO.SAM.SAMAlignedSequence))
            out.Add(PolyA_Type.Both, New List(Of Bio.IO.SAM.SAMAlignedSequence))
            out.Add(PolyA_Type.None, New List(Of Bio.IO.SAM.SAMAlignedSequence))
            out.Add(PolyA_Type.polyA, New List(Of Bio.IO.SAM.SAMAlignedSequence))
            out.Add(PolyA_Type.polyA_falsePolyT, New List(Of Bio.IO.SAM.SAMAlignedSequence))
            out.Add(PolyA_Type.polyT, New List(Of Bio.IO.SAM.SAMAlignedSequence))
            out.Add(PolyA_Type.polyT_falsePolyA, New List(Of Bio.IO.SAM.SAMAlignedSequence))
            out.Add(PolyA_Type.All, New List(Of Bio.IO.SAM.SAMAlignedSequence))
            out.Add(PolyA_Type.polyAT, New List(Of Bio.IO.SAM.SAMAlignedSequence))
            out.Add(PolyA_Type.For_PAS, New List(Of Bio.IO.SAM.SAMAlignedSequence))
            out.Add(PolyA_Type.For_TSS, New List(Of Bio.IO.SAM.SAMAlignedSequence))
            Return out
        End Function
#Region "Select Case"
        Private Shared Function Get_Result_T(pSeq As Bio.ISequence, Type As False_PolyAT_Discovering, NA As Byte, length As Integer)
            Select Case Type
                Case False_PolyAT_Discovering.From_Beginning
                    Dim Count As Integer = 0
                    For i1 = 0 To pSeq.Count - 1
                        If pSeq(i1) = NA Then
                            Count += 1
                        Else
                            Return Count
                        End If
                    Next
                    Return Count
                Case False_PolyAT_Discovering.Nof_Consecutive_AT
                    Try
                        Dim Result As String = If(String.IsNullOrEmpty(pSeq.ConvertToString), String.Empty, Regex.Matches(pSeq.ConvertToString, "(" & ChrW(NA) & ")\1*", RegexOptions.None).Cast(Of Match)().OrderByDescending(Function(x) x.Length).First().Value)
                        Return Result.Count

                    Catch ex As Exception
                        Return 0
                    End Try

                Case False_PolyAT_Discovering.SM_aligner
                    Return False
                Case False_PolyAT_Discovering.Percent_of_AT
                    Dim Count As Integer = 0
                    For Each c In pSeq
                        If c = NA Then Count += 1
                    Next
                    Return Count / length * 100
                Case Else
                    Return Nothing
            End Select
        End Function
        Private Shared Function Get_Result_A(pSeq As Bio.ISequence, Type As False_PolyAT_Discovering, NA As Byte, length As Integer)
            Select Case Type
                Case False_PolyAT_Discovering.From_Beginning
                    Dim Count As Integer = 0
                    For i1 = pSeq.Count - 1 To 0 Step -1
                        If pSeq(i1) = NA Then
                            Count += 1
                        Else
                            Return Count
                        End If
                    Next
                    Return Count
                Case False_PolyAT_Discovering.Nof_Consecutive_AT
                    Try
                        Dim Result As String = If(String.IsNullOrEmpty(pSeq.ConvertToString), String.Empty, Regex.Matches(pSeq.ConvertToString, "(" & ChrW(NA) & ")\1*", RegexOptions.None).Cast(Of Match)().OrderByDescending(Function(x) x.Length).First().Value)
                        Return Result.Count

                    Catch ex As Exception
                        Return 0
                    End Try

                Case False_PolyAT_Discovering.SM_aligner
                    Return False
                Case False_PolyAT_Discovering.Percent_of_AT
                    Dim Count As Integer = 0
                    For Each c In pSeq
                        If c = NA Then Count += 1
                    Next
                    Return Count / length * 100

            End Select
        End Function

        Private Shared Function Get_Result_Is_False_A(pseq As Bio.ISequence, NA As Byte, THreshold As Double, length As Integer, type As False_PolyAT_Discovering)
            Select Case type
                Case False_PolyAT_Discovering.From_Beginning
                    Dim Count As Integer = 0
                    For i1 = pseq.Count - 1 To 0 Step -1
                        If pseq(i1) = NA Then
                            Count += 1
                        Else
                            Exit For
                        End If
                    Next

                    If Count >= THreshold Then Return True Else Return False

                Case False_PolyAT_Discovering.Nof_Consecutive_AT
                    Try
                        Dim Result As String = If(String.IsNullOrEmpty(pseq.ConvertToString), String.Empty, Regex.Matches(pseq.ConvertToString, "(" & ChrW(NA) & ")\1*", RegexOptions.None).Cast(Of Match)().OrderByDescending(Function(x) x.Length).First().Value)
                        If Result.Count >= THreshold Then
                            Return True
                        Else
                            Return False
                        End If
                    Catch ex As Exception
                        Return False
                    End Try


                Case False_PolyAT_Discovering.SM_aligner
                    Return False
                Case False_PolyAT_Discovering.Percent_of_AT
                    Dim Count As Integer = 0
                    For Each c In pseq
                        If c = NA Then Count += 1
                    Next
                    If Count / length * 100 > THreshold Then
                        Return True
                    Else
                        Return False
                    End If
            End Select
        End Function

        Private Shared Function Get_Result_Is_False_T(pseq As Bio.ISequence, NA As Byte, THreshold As Double, length As Integer, type As False_PolyAT_Discovering) As Boolean
            Select Case type
                Case False_PolyAT_Discovering.From_Beginning
                    Dim Count As Integer = 0
                    For i1 = 0 To pseq.Count - 1
                        If pseq(i1) = NA Then
                            Count += 1
                        Else
                            Exit For
                        End If
                    Next

                    If Count >= THreshold Then Return True Else Return False

                Case False_PolyAT_Discovering.Nof_Consecutive_AT
                    Try
                        Dim Result As String = If(String.IsNullOrEmpty(pseq.ConvertToString), String.Empty, Regex.Matches(pseq.ConvertToString, "(" & ChrW(NA) & ")\1*", RegexOptions.None).Cast(Of Match)().OrderByDescending(Function(x) x.Length).First().Value)
                        If Result.Count >= THreshold Then
                            Return True
                        Else
                            Return False
                        End If
                    Catch ex As Exception
                        Return False
                    End Try


                Case False_PolyAT_Discovering.SM_aligner
                    Return False
                Case False_PolyAT_Discovering.Percent_of_AT
                    Dim Count As Integer = 0
                    For Each c In pseq
                        If c = NA Then Count += 1
                    Next
                    If Count / length * 100 > THreshold Then
                        Return True
                    Else
                        Return False
                    End If
            End Select
        End Function
#End Region

#Region "ILocation"
        Public Shared Function False_polyA(Seq As Bio.ISequence,
                                           SAM As Bio.IO.GenBank.ILocation,
                                           length As Integer,
                                           Type As False_PolyAT_Discovering,
                                           Threshold As Double,
                                           NA As Byte) As Double
            Dim pSeq = Seq.GetSubSequence(SAM.LocationEnd - length, length)
            Dim x = Get_Result_A(pSeq, Type, NA, length)
            Return x
        End Function
        Public Shared Function False_polyT(Seq As Bio.ISequence,
                                           SAM As Bio.IO.GenBank.ILocation,
                                           length As Integer,
                                           Type As False_PolyAT_Discovering,
                                           Threshold As Double,
                                            NA As Byte) As Double

            Dim pSeq = Seq.GetSubSequence(SAM.LocationStart - 1, length)
            Return Get_Result_T(pSeq, Type, NA, length)

        End Function
        Public Shared Function Is_False_polyA(Seq As Bio.ISequence,
                                              SAM As Bio.IO.GenBank.ILocation,
                                              length As Integer,
                                              Type As False_PolyAT_Discovering,
                                              Threshold As Double,
                                              NA As Byte) As Boolean

            Dim pSeq = Seq.GetSubSequence(SAM.LocationStart - length, length)
            Return Get_Result_Is_False_A(pSeq, NA, Threshold, length, Type)

        End Function
        Public Shared Function Is_False_polyT(Seq As Bio.ISequence,
                                              SAM As Bio.IO.GenBank.ILocation,
                                              length As Integer,
                                              Type As False_PolyAT_Discovering,
                                              Threshold As Double,
                                              NA As Byte) As Boolean
            Dim pSeq = Seq.GetSubSequence(SAM.LocationStart - 1, length)
            Return Get_Result_Is_False_T(pSeq, NA, Threshold, length, Type)

        End Function

#End Region
#Region "SAM"
        Public Shared Function False_polyA(Seq As Bio.ISequence,
                                           SAM As Bio.IO.SAM.SAMAlignedSequence,
                                           length As Integer,
                                           Type As False_PolyAT_Discovering,
                                           Threshold As Double,
                                           NA As Byte) As Double
            Dim pSeq = Seq.GetSubSequence(SAM.RefEndPos - length, length)
            Return Get_Result_A(pSeq, Type, NA, length)

        End Function
        Public Shared Function False_polyT(Seq As Bio.ISequence,
                                           SAM As Bio.IO.SAM.SAMAlignedSequence,
                                           length As Integer,
                                           Type As False_PolyAT_Discovering,
                                           Threshold As Double,
                                            NA As Byte) As Double

            Dim pSeq = Seq.GetSubSequence(SAM.Pos - 1, length)
            Return Get_Result_T(pSeq, Type, NA, length)
        End Function
        Public Shared Function Is_False_polyA(Seq As Bio.ISequence,
                                              SAM As Bio.IO.SAM.SAMAlignedSequence,
                                              length As Integer,
                                              Type As False_PolyAT_Discovering,
                                              Threshold As Double,
                                              NA As Byte) As Boolean

            Dim pSeq = Seq.GetSubSequence(SAM.RefEndPos - length, length)
            Return Get_Result_Is_False_A(pSeq, NA, Threshold, length, Type)
        End Function
        Public Shared Function Is_False_polyT(Seq As Bio.ISequence,
                                              SAM As Bio.IO.SAM.SAMAlignedSequence,
                                              length As Integer,
                                              Type As False_PolyAT_Discovering,
                                              Threshold As Double,
                                              NA As Byte) As Boolean
            Dim pSeq = Seq.GetSubSequence(SAM.RefEndPos - length, length)
            Return Get_Result_Is_False_T(pSeq, NA, Threshold, length, Type)

        End Function
        Public Shared Sub Write_Orientation(out As Dictionary(Of PolyA_Type, List(Of SAMAlignedSequence)), file As FileInfo, Optional Header As Bio.IO.SAM.SAMAlignmentHeader = Nothing)
            If IsNothing(Header) = True Then Header = Szunyi.BAM.Bam_Basic_IO.Headers.Get_Header(file)

            Dim x1 As New DirectoryInfo(file.DirectoryName & "\" & file.Name.Replace(".bam", "").Replace(".sam", "").Replace(".sorted", ""))
            If x1.Exists = False Then x1.Create()

            For Each Item In out
                Dim kj = CType(Item.Key, Szunyi.mRNA.PolyA_Type).ToString()
                Dim SFIle As New FileInfo(x1.FullName & "/" & kj & "_" & file.Name & ".sam")
                Szunyi.IO.Export.Export_Sam(Item.Value, Header, SFIle)
            Next
        End Sub
        Public Shared Sub Write_Orientation(out As Dictionary(Of PolyA_Type, List(Of SAMAlignedSequence)), Dir As DirectoryInfo, file As FileInfo, Optional Header As Bio.IO.SAM.SAMAlignmentHeader = Nothing)
            If IsNothing(Header) = True Then Header = Szunyi.BAM.Bam_Basic_IO.Headers.Get_Header(file)

            For Each Item In out
                If Item.Value.Count > 0 Then
                    Dim kj = CType(Item.Key, Szunyi.mRNA.PolyA_Type).ToString() & "_e" & Item.Value.Count & "_"
                    Dim SFIle As New FileInfo(Dir.FullName & "/" & kj & "_" & file.Name)
                    Szunyi.IO.Export.Export_Sam(Item.Value, Header, SFIle)
                End If
            Next
        End Sub
        Public Shared Sub Write_Orientation(out As Dictionary(Of PolyA_Type, List(Of SAMAlignedSequence)), file As FileInfo, cSetting As Szunyi.Transcipts.Aligner_Setting)
            Dim Header = Szunyi.BAM.Bam_Basic_IO.Headers.Get_Header(file)
            cSetting.Set_Header(Header)

            out(Szunyi.mRNA.PolyA_Type.polyAT).AddRange(out(Szunyi.mRNA.PolyA_Type.polyA))
            out(Szunyi.mRNA.PolyA_Type.polyAT).AddRange(out(Szunyi.mRNA.PolyA_Type.polyT))
            Dim x1 As New DirectoryInfo(file.DirectoryName & "\" & file.Name.Replace(".bam", "").Replace(".sam", "").Replace(".sorted", ""))
            If x1.Exists = False Then x1.Create()

            For Each Item In out
                Dim kj = CType(Item.Key, Szunyi.mRNA.PolyA_Type).ToString()
                Dim SFIle As New FileInfo(x1.FullName & "/" & kj & "_" & file.Name & ".sam")
                Szunyi.IO.Export.Export_Sam(Item.Value, Header, SFIle)
            Next
        End Sub

#End Region
    End Class

End Namespace
