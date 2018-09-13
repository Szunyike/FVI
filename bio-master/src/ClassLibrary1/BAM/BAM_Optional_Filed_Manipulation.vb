Imports System.Text.RegularExpressions
Imports Bio.IO.SAM
Imports ClassLibrary1.Szunyi.mRNA
Imports ClassLibrary1.Szunyi.Transcipts

Namespace Szunyi
    Namespace BAM
        Namespace CIgar_Md
            Public Class Get_Properties
                Public Shared Function MD_Length(sam As SAMAlignedSequence) As Integer
                    Dim MD_String = From x In sam.OptionalFields Where x.Tag = "MD"

                    If MD_String.Count = 1 Then
                        Dim k As New M_Ins_Del(MD_String.First.Value)
                        Return k.Matches + k.MisMatches + k.Deletions
                    End If
                    Return 0
                End Function
                Public Shared Function MD_Length(Md_String As String) As Integer


                    Dim k As New M_Ins_Del(Md_String)
                    Return k.Matches + k.MisMatches + k.Deletions

                    Return 0

                End Function
                ''' <summary>
                ''' Return sum of M I S and X
                ''' </summary>
                ''' <param name="SAM"></param>
                ''' <returns></returns>
                Public Shared Function Cigar_Read_Length(SAM As Bio.IO.SAM.SAMAlignedSequence) As Integer
                    Dim x1 = Common.Get_CIGARS(SAM.CIGAR)
                    Dim count As Integer = 0
                    For Each s In x1
                        If s.Key = "M" Or s.Key = "I" Or s.Key = "S" Or s.Key = "X" Then
                            count += s.Value
                        End If
                    Next
                    Return count
                End Function
                ''' <summary>
                ''' Return sum of M and X wo Insertions, Deletions
                ''' </summary>
                ''' <param name="SAM"></param>
                ''' <returns></returns>
                Public Shared Function Cigar_Matched_Length(SAM As Bio.IO.SAM.SAMAlignedSequence) As Integer
                    Dim x1 = Common.Get_CIGARS(SAM.CIGAR)
                    Dim count As Integer = 0
                    For Each s In x1
                        If s.Key = "M" Or s.Key = "X" Then
                            count += s.Value
                        End If
                    Next
                    Return count
                End Function
                ''' <summary>
                ''' Return sum of M,X,D. This is for compare MD Lengths
                ''' </summary>
                ''' <param name="SAM"></param>
                ''' <returns></returns>
                Public Shared Function Cigar_Matched_Length_woInsertion(SAM As Bio.IO.SAM.SAMAlignedSequence) As Integer
                    Dim x1 = Common.Get_CIGARS(SAM.CIGAR)
                    Dim count As Integer = 0
                    Dim Ms = (From x In x1 Where x.Key = "M" Select x.Value).Sum
                    Dim Dels = (From x In x1 Where x.Key = "D" Select x.Value).Sum
                    Dim Xs = (From x In x1 Where x.Key = "X" Select x.Value).Sum
                    Return Ms + Dels + Xs
                End Function
                ''' <summary>
                ''' Return sum of M,X,D. This is for compare MD Lengths
                ''' </summary>
                ''' <param name="SAM"></param>
                ''' <returns></returns>
                Public Shared Function Cigar_Matched_Length_woInsertion(CiGAR As String) As Integer
                    Dim x1 = Common.Get_CIGARS(CiGAR)
                    Dim count As Integer = 0
                    Dim Ms = (From x In x1 Where x.Key = "M" Select x.Value).Sum
                    Dim Dels = (From x In x1 Where x.Key = "D" Select x.Value).Sum
                    Dim Xs = (From x In x1 Where x.Key = "X" Select x.Value).Sum
                    Return Ms + Dels + Xs
                End Function
            End Class
            Public Class Common
                ''' <summary>
                ''' Get KeyValuePair From CIGAR string
                ''' </summary>
                ''' <param name="CIGAR"></param>
                ''' <returns></returns>
                Public Shared Function Get_CIGARS(CIGAR As String) As List(Of KeyValuePair(Of String, Integer))
                    Dim CIGARS As New List(Of KeyValuePair(Of String, Integer))
                    Dim cI As String = ""
                    For i1 = 0 To CIGAR.Count - 1
                        Dim s As String = CIGAR(i1)
                        Dim i As Integer = 0
                        If Integer.TryParse(s, 1) Then
                            cI = cI & s
                        Else
                            If cI = String.Empty Then cI = "0"
                            Dim t As New KeyValuePair(Of String, Integer)(s, cI)
                            CIGARS.Add(t)
                            cI = ""
                        End If
                    Next
                    Return CIGARS
                End Function

                Public Shared Function IsSame_Length(SAM As SAMAlignedSequence) As Boolean
                    If Get_Properties.Cigar_Matched_Length_woInsertion(SAM) = Get_Properties.MD_Length(SAM) Then
                        Return True
                    Else
                        Return False
                    End If
                End Function

            End Class
        End Namespace
        Public Class BAM_Optional_Filed_Manipulation
            Public Shared Sub Change_MD(SAM As SAMAlignedSequence, MD As String)
                Dim x1 = (From x In SAM.OptionalFields Where x.Tag = "MD").First
                x1.Value = MD
            End Sub

            ''' <summary>
            ''' am Optional Field AD = polyA or PolyT
            ''' </summary>
            ''' <param name="SAMs"></param>
            ''' <returns></returns>
            Public Shared Function Get_Orientation(SAMs As List(Of SAMAlignedSequence)) As List(Of SAMAlignedSequence)
                Dim Out As New List(Of SAMAlignedSequence)
                For Each Sam In SAMs
                    If Is_Orientation(Sam) = True Then Out.Add(Sam)
                Next
                Return Out
            End Function
            ''' <summary>
            ''' Bam Optional Field AD = polyA or PolyT
            ''' </summary>
            ''' <param name="SAM"></param>
            ''' <returns></returns>
            Public Shared Function Is_Orientation(SAM As SAMAlignedSequence) As Boolean
                Dim res = From x In SAM.OptionalFields Where x.Tag = "FO"

                If res.Count = 0 Then Return False

                If res.First.Value = Szunyi.mRNA.PolyA_Type.polyA Then Return True
                If res.First.Value = Szunyi.mRNA.PolyA_Type.polyT Then Return True

                Return False
            End Function

            ''' <summary>
            ''' return Bam Optional Field AD = polyA or PolyT
            ''' </summary>
            ''' <param name="SAM"></param>
            ''' <returns></returns>
            Public Shared Function Get_Adapter_Value(SAM As SAMAlignedSequence, AD As Szunyi.Transcipts.Adaptor_Pair) As Adaptor_Pair_Scores
                Dim res = From x In SAM.OptionalFields Where x.Tag.StartsWith(AD.PreFix)

                Return New Szunyi.Transcipts.Adaptor_Pair_Scores(res)


            End Function
            ''' <summary>
            ''' return Bam Optional Field PA Value
            ''' </summary>
            ''' <param name="SAM"></param>
            ''' <returns></returns>
            Public Shared Function Get_pA_Value(SAM As SAMAlignedSequence) As Szunyi.mRNA.PolyA_Type
                Dim res = From x In SAM.OptionalFields Where x.Tag = "PA"

                If res.Count = 0 Then Return Szunyi.mRNA.PolyA_Type.None

                Return res.First.Value

            End Function
            ''' <summary>
            ''' am Optional Field AD = polyA or PolyT
            ''' </summary>
            ''' <param name="SAMs"></param>
            ''' <returns></returns>
            Public Shared Function Get_Adapters(SAMs As IEnumerable(Of SAMAlignedSequence)) As List(Of SAMAlignedSequence)
                Dim Out As New List(Of SAMAlignedSequence)
                For Each Sam In SAMs
                    If Is_Adapter(Sam) = True Then Out.Add(Sam)
                Next
                Return Out
            End Function
            ''' <summary>
            ''' am Optional Field AD = polyA or PolyT
            ''' </summary>
            ''' <param name="SAMs"></param>
            ''' <returns></returns>
            Public Shared Function Get_Adapters(SAMs As IEnumerable(Of SAMAlignedSequence), field As String, score As Integer) As List(Of SAMAlignedSequence)
                Dim Out As New List(Of SAMAlignedSequence)
                For Each Sam In SAMs
                    If Is_Adapter(Sam, field, score) = True Then Out.Add(Sam)
                Next
                Return Out
            End Function
            ''' <summary>
            ''' Bam Optional Field AD = polyA or PolyT
            ''' </summary>
            ''' <param name="SAM"></param>
            ''' <returns></returns>
            Public Shared Function Is_Adapter(SAM As SAMAlignedSequence) As Boolean
                Dim res = From x In SAM.OptionalFields Where x.Tag = "b1"

                If res.Count = 0 Then Return False

                If res.First.Value = Szunyi.mRNA.PolyA_Type.polyA Then Return True
                If res.First.Value = Szunyi.mRNA.PolyA_Type.polyT Then Return True

                Return False
            End Function
            ''' <summary>
            ''' Bam Optional Field AD = polyA or PolyT
            ''' </summary>
            ''' <param name="SAM"></param>
            ''' <returns></returns>
            Public Shared Function Is_Adapter(SAM As SAMAlignedSequence, field As String, score As Integer) As Boolean
                Dim res = From x In SAM.OptionalFields Where x.Tag = field

                If res.Count = 0 Then Return False

                If res.First.Value >= score Then Return True

                Return False
            End Function
            ''' <summary>
            ''' Bam Optional Field CA = polyA or PolyT
            ''' </summary>
            ''' <param name="SAMs"></param>
            ''' <returns></returns>
            Public Shared Function Get_CAPs(SAMs As IEnumerable(Of SAMAlignedSequence)) As List(Of SAMAlignedSequence)
                Dim Out As New List(Of SAMAlignedSequence)
                For Each Sam In SAMs
                    If Is_CAP(Sam) = True Then Out.Add(Sam)
                Next
                Return Out
            End Function
            ''' <summary>
            ''' Bam Optional Field CA = polyA or PolyT
            ''' </summary>
            ''' <param name="SAM"></param>
            ''' <returns></returns>
            Public Shared Function Is_CAP(SAM As SAMAlignedSequence) As Boolean
                Dim res = From x In SAM.OptionalFields Where x.Tag = "CA"

                If res.Count = 0 Then Return False

                If res.First.Value = Szunyi.mRNA.PolyA_Type.polyA Then Return True
                If res.First.Value = Szunyi.mRNA.PolyA_Type.polyT Then Return True

                Return False
            End Function
            ''' <summary>
            ''' Bam Optional Field PA = polyA or PolyT
            ''' </summary>
            ''' <param name="SAMs"></param>
            ''' <returns></returns>
            Public Shared Function Get_PolyAs(SAMs As List(Of SAMAlignedSequence)) As List(Of SAMAlignedSequence)
                Dim Out As New List(Of SAMAlignedSequence)
                For Each Sam In SAMs
                    If Is_PolyA(Sam) = True Then Out.Add(Sam)
                Next
                Return Out
            End Function
            ''' <summary>
            ''' Bam Optional Field a1 = polyA or PolyT
            ''' </summary>
            ''' <param name="SAM"></param>
            ''' <returns></returns>
            Public Shared Function Is_PolyA(SAM As SAMAlignedSequence) As Boolean
                Dim res = From x In SAM.OptionalFields Where x.Tag = "a1"

                If res.Count = 0 Then Return False

                If res.First.Value = Szunyi.mRNA.PolyA_Type.polyA Then Return True
                If res.First.Value = Szunyi.mRNA.PolyA_Type.polyT Then Return True

                Return False
            End Function

            Public Shared Function Get_NMs(Sams As List(Of Bio.IO.SAM.SAMAlignedSequence)) As List(Of NM)
                Dim res As New List(Of NM)
                For Each Item In Szunyi.BAM.GroupBy.ReadID(Sams)
                    Dim n As New NM(Item)
                    res.Add(n)
                Next
                Return res
            End Function

            Public Class Iterate_Group
                Public Shared Iterator Function NM(NMs As List(Of NM)) As IEnumerable(Of List(Of NM))
                    Dim res = From x In NMs Group By x.Nof_Mapping, x.nof_diff_NM Into Group

                    For Each gr In res
                        Yield gr.Group.ToList
                    Next
                End Function

            End Class
            Public Class Get_Sams
                Public Shared Function Fron_NM(NMs As List(Of NM)) As List(Of Bio.IO.SAM.SAMAlignedSequence)
                    Dim out As New List(Of Bio.IO.SAM.SAMAlignedSequence)
                    For Each NM In NMs
                        out.AddRange(NM.Sams)
                    Next
                    Return out
                End Function
            End Class
            Public Class Statistic
                Public Shared Function NMs(Items As List(Of NM)) As String
                    Dim str As New System.Text.StringBuilder
                    str.Append(Items.Count).AppendLine()
                    str.Append("Nof_Mapping").Append(vbTab)
                    str.Append("Nof_diff_NM").Append(vbTab)
                    str.Append("Counts").Append(vbTab)
                    str.Append("Percent")
                    For Each gr In BAM_Optional_Filed_Manipulation.Iterate_Group.NM(Items)
                        str.AppendLine()
                        str.Append(gr.First.Nof_Mapping)
                        str.Append(vbTab)
                        str.Append(gr.First.nof_diff_NM)
                        str.Append(vbTab)
                        str.Append(gr.Count)
                        str.Append(vbTab)
                        str.Append(gr.Count / Items.Count * 100)
                    Next
                    Return str.ToString
                End Function
            End Class
            ''' <summary>
            ''' Add Tag,Type,Value to SAM
            ''' </summary>
            ''' <param name="SAM"></param>
            ''' <param name="Tag"></param>
            ''' <param name="Type"></param>
            ''' <param name="Value"></param>
            Public Shared Sub Add(SAM As SAMAlignedSequence, Tag As String, Type As String, Value As String)
                Dim x As New Bio.IO.SAM.SAMOptionalField
                x.Tag = Tag
                x.VType = Type
                x.Value = Value
                SAM.OptionalFields.Add(x)
            End Sub
            ''' <summary>
            ''' Orientation, Five_Prime Score, Three_Prime_Score,Five_Prime_Pos,Three_Prime_Pos,Adaptor Five Pos, Adaptor Three Pos
            ''' </summary>
            ''' <param name="sam"></param>
            ''' <param name="PreFix"></param>
            ''' <param name="aAR_pA"></param>
            Public Shared Sub Add_AAR(ByRef sam As SAMAlignedSequence, PreFix As String, aAR_pA As Adaptor_Aligner_Result)

                Szunyi.BAM.BAM_Optional_Filed_Manipulation.Add(sam, PreFix & "2", "i", Szunyi.Transcipts.Adaptor_Aligner_Result_Manipulation.Get_Five_Prime_Score(aAR_pA))
                Szunyi.BAM.BAM_Optional_Filed_Manipulation.Add(sam, PreFix & "3", "i", Szunyi.Transcipts.Adaptor_Aligner_Result_Manipulation.Get_Three_Prime_Score(aAR_pA))

                Szunyi.BAM.BAM_Optional_Filed_Manipulation.Add(sam, PreFix & "4", "i", Szunyi.Transcipts.Adaptor_Aligner_Result_Manipulation.Get_Five_Prime_Pos(aAR_pA))
                Szunyi.BAM.BAM_Optional_Filed_Manipulation.Add(sam, PreFix & "5", "i", Szunyi.Transcipts.Adaptor_Aligner_Result_Manipulation.Get_Three_Prime_Pos(aAR_pA))

                Szunyi.BAM.BAM_Optional_Filed_Manipulation.Add(sam, PreFix & "6", "i", Szunyi.Transcipts.Adaptor_Aligner_Result_Manipulation.Get_Five_Prime_Pos_Adapter(aAR_pA))
                Szunyi.BAM.BAM_Optional_Filed_Manipulation.Add(sam, PreFix & "7", "i", Szunyi.Transcipts.Adaptor_Aligner_Result_Manipulation.Get_Three_Prime_Pos_Adapter(aAR_pA))



            End Sub
            ''' <summary>
            ''' Return ID or Empty String
            ''' </summary>
            ''' <param name="sAM"></param>
            ''' <returns></returns>
            Public Shared Function Get_Read_Group_ID(sAM As SAMAlignedSequence) As String
                Dim x = From h In sAM.OptionalFields Where h.Tag = "RG"

                If x.Count = 1 Then
                    Return x.First.Value
                Else
                    Return String.Empty
                End If
            End Function

            Friend Shared Function Get_MD(sAM As SAMAlignedSequence) As Object
                Dim x = From h In sAM.OptionalFields Where h.Tag = "MD"

                If x.Count = 1 Then
                    Return x.First.Value
                Else
                    Return String.Empty
                End If
            End Function
        End Class


        Public Class M_Ins_Del
            Public Property Matches As Integer
            Public Property Insertions As Integer
            Public Property Deletions As Integer
            Public Property MisMatches As Integer
            Public Sub New(tmp As String)

                Dim t As Integer = 0
                Dim dels As New List(Of String)
                Dim MS As New List(Of String)
                Dim MMs As New List(Of String)
                For Each match As Match In System.Text.RegularExpressions.Regex.Matches(tmp, "[0-9]+")
                    Dim groups As GroupCollection = match.Groups

                    For Each gr As Group In groups
                        If gr.Success = True Then
                            Try
                                t += gr.Value
                                MS.Add(gr.Value)
                            Catch
                                Dim kj As Integer = 54
                            End Try
                        End If
                    Next
                Next
                Matches = t

                Dim D As Integer = 0
                Dim I As Integer = 0
                For Each match As Match In System.Text.RegularExpressions.Regex.Matches(tmp, "[\^ACGNT]+")
                    Dim groups As GroupCollection = match.Groups
                    For Each gr As Group In groups
                        If gr.Value.StartsWith("^") = False AndAlso gr.Value.Contains("^") Then
                            Dim s = Split(gr.Value, "^")
                            I += s.First.Count
                            MMs.Add(s.First)
                            D += s.Last.Count
                            dels.Add(s.Last)
                        ElseIf gr.Value.StartsWith("^") Then
                            D += gr.Value.Length - 1
                            dels.Add(gr.Value)
                        Else
                            I += gr.Value.Length
                            MMs.Add(gr.Value)
                        End If
                    Next
                Next
                MisMatches = I
                Deletions = D
            End Sub


        End Class

        Public Class NM
            Public Property Nof_Mapping As Integer
            Public Property Sams As List(Of Bio.IO.SAM.SAMAlignedSequence)
            Public Property nof_diff_NM As Integer
            Public Sub New(sams As List(Of Bio.IO.SAM.SAMAlignedSequence))
                Me.Sams = sams
                Me.Nof_Mapping = sams.Count
                Me.nof_diff_NM = Szunyi.Alignment.BAM_SAM.Sam_Manipulation.Get_Nof_Diff_NM(sams)
            End Sub
        End Class
        Public Class PolyA

        End Class


    End Namespace
End Namespace

