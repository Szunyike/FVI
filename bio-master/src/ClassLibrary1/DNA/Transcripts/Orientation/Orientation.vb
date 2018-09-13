Imports System.IO
Imports Bio
Imports Bio.Algorithms.Alignment
Imports Bio.IO.SAM
Imports ClassLibrary1.Szunyi.mRNA

Namespace Szunyi.Transcipts
    Public Class Score_Settings
        Public Property Files As List(Of FileInfo)
        Public Property Regions As New List(Of Szunyi.Transcipts.Aligner_Setting)
        Public Property Adapter_Pairs As New List(Of Szunyi.Transcipts.Adaptor_Pair)

    End Class
    Public Class Orientation_Setting
        Public Property Files As List(Of FileInfo)
        Public Property Regions As New List(Of Szunyi.Transcipts.Aligner_Setting)
        Public Property Adapter_Pairs As New Dictionary(Of Szunyi.Transcipts.Adaptor_Pair, Adapter_Filtering)

    End Class
    <Serializable>
    Public Class Adapter_Filtering
        Public Sub New()

        End Sub

        Public Sub New(Five_Prime_Seq As String, Three_Prime_Seq As String, name As String)
            Me.Five_Prime_Seq = Five_Prime_Seq
            Me.Three_Prime_Seq = Three_Prime_Seq
            Me.AP_Name = name
        End Sub

        Public Property AP_Name As String
        Public Property Five_Prime_Seq As String
        Public Property Three_Prime_Seq As String
        Public Property Five_Prime_Adaptor_Score As Integer
        Public Property Three_Prime_Adaptor_Score As Integer

        Public Property Five_Prime_Query_Maximal_Position As Integer
        Public Property Three_Prime_Query_Maximal_Position As Integer
        Public Property Five_Prime_Query_Minimal_Position As Integer
        Public Property Three_Prime_Query_Minimal_Position As Integer

        Public Property Five_Prime_Adaptor_Maximal_Position As Integer
        Public Property Three_Prime_Adaptor_Maximal_Position As Integer
        Public Property Five_Prime_Adaptor_Minimal_Position As Integer
        Public Property Three_Prime_Adaptor_Minimal_Position As Integer
        Public Property Five_Prime_For_TSS As Boolean = False
        Public Property Three_Prime_For_TSS As Boolean = False
        Public Property Five_Prime_For_PAS As Boolean = False
        Public Property Three_Prime_For_PAS As Boolean = False

        Public Property Passed_Name As String
        Public Property Not_Passed_Name As String

        Public Property Diff As Integer
        Public Overrides Function ToString() As String
            Dim str As New System.Text.StringBuilder
            str.AppendLine()
            str.Append("#Five_Prime_Adaptor_Score:").Append(Five_Prime_Adaptor_Score).AppendLine()
            str.Append("#Three_Prime_Adaptor_Score:").Append(Three_Prime_Adaptor_Score).AppendLine()

            str.Append("#Five_Prime_Adaptor_Minimal_Position:").Append(Five_Prime_Adaptor_Minimal_Position).AppendLine()
            str.Append("#Five_Prime_Adaptor_Maximal_Position:").Append(Five_Prime_Adaptor_Maximal_Position).AppendLine()

            str.Append("#Three_Prime_Adaptor_Minimal_Position:").Append(Three_Prime_Adaptor_Minimal_Position).AppendLine()
            str.Append("#Three_Prime_Adaptor_Maximal_Position:").Append(Three_Prime_Adaptor_Maximal_Position).AppendLine()

            str.Append("#Five_Prime_Query_Minimal_Position:").Append(Five_Prime_Query_Minimal_Position).AppendLine()
            str.Append("#Five_Prime_Query_Maximal_Position:").Append(Five_Prime_Query_Maximal_Position).AppendLine()

            str.Append("#Three_Prime_Query_Minimal_Position:").Append(Three_Prime_Query_Minimal_Position).AppendLine()
            str.Append("#Three_Prime_Query_Maximal_Position:").Append(Three_Prime_Query_Maximal_Position).AppendLine()

            str.Append("#Five_Prime_For_TSS:").Append(Five_Prime_For_TSS).AppendLine()
            str.Append("#Three_Prime_For_TSS:").Append(Three_Prime_For_TSS).AppendLine()

            str.Append("#Five_Prime_For_PAS:").Append(Five_Prime_For_PAS).AppendLine()
            str.Append("#Three_Prime_For_PAS:").Append(Three_Prime_For_PAS).AppendLine()

            str.Append("#Passed_Name:").Append(Passed_Name).AppendLine()
            str.Append("#Not_Passed_Name:").Append(Not_Passed_Name).AppendLine()

            str.Append("#Diff Score:").Append(Diff)

            Return str.ToString

        End Function
    End Class

    Public Class Adaptor_Pair_Scores
        Private res As IEnumerable(Of SAMOptionalField)
        Public Property Five_Prime_Score As Integer
        Public Property Three_Prime_Score As Integer
        Public Property Five_Prime_Pos As Integer
        Public Property Three_Prime_Pos As Integer
        Public Property Diff As Integer

        Public Property Three_Prime_Pos_in_Adapter As Integer
        Public Property Five_Prime_Pos_in_Adapter As Integer

        Public Sub New()

        End Sub

        Public Sub New(res As IEnumerable(Of SAMOptionalField))
            Me.res = res
            If res.Count >= 4 Then
                For Each Item In res
                    Select Case Item.Tag.Last
                        Case "2"
                            Me.Five_Prime_Score = Item.Value
                        Case "4"
                            Me.Five_Prime_Pos = Item.Value
                        Case "3"
                            Me.Three_Prime_Score = Item.Value
                        Case "5"
                            Me.Three_Prime_Pos = Item.Value
                        Case "6"
                            Me.Five_Prime_Pos_in_Adapter = Item.Value
                        Case "7"
                            Me.Three_Prime_Pos_in_Adapter = Item.Value
                    End Select
                Next
                Me.Diff = System.Math.Abs(Me.Five_Prime_Score - Me.Three_Prime_Score)
            End If
        End Sub
    End Class
    Public Class Orientation
        Public Enum Type
            fw = 1
            rev = 2
            unknown = 3
            both = 4
            none = 5
        End Enum
        Private Shared Property Adaptors As New Adaptors

        ''' <summary>
        ''' U-polyT(Five Prime Adaptor, D-polyA(Three Prime Adaptor)
        ''' </summary>
        ''' <param name="U"></param>
        ''' <param name="D"></param>
        ''' <param name="cSetting"></param>
        ''' <param name="Five_Prime_Adaptor"></param>
        ''' <param name="Three_Prime_Adaptor"></param>
        ''' <returns></returns>
        Public Shared Function Get_AAR(U As Bio.ISequence, D As Bio.ISequence,
                                            cSetting As Szunyi.Transcipts.Aligner_Setting,
                                            Five_Prime_Adaptor As Bio.ISequence,
                                            Three_Prime_Adaptor As Bio.ISequence) As Adaptor_Aligner_Result


            Dim r_f As New List(Of IPairwiseSequenceAlignment)
            Dim r_t As New List(Of IPairwiseSequenceAlignment)

            If IsNothing(U) = True AndAlso IsNothing(D) = True Then
                ' Return Szunyi.mRNA.PolyA_Type.None
            ElseIf IsNothing(D) = True Then
                r_f = cSetting.Sm_Aligner.AlignSimple(U, Five_Prime_Adaptor)

            ElseIf IsNothing(U) = True Then
                r_t = cSetting.Sm_Aligner.AlignSimple(D, Three_Prime_Adaptor)

            Else
                r_f = cSetting.Sm_Aligner.AlignSimple(U, Five_Prime_Adaptor)
                r_t = cSetting.Sm_Aligner.AlignSimple(D, Three_Prime_Adaptor)

            End If
            Return New Adaptor_Aligner_Result(U, D, r_f, r_t)

        End Function

        Public Shared Function Merge(polyA_Orientation As PolyA_Type, cap_Orientation As PolyA_Type) As PolyA_Type
            If polyA_Orientation = cap_Orientation Then Return polyA_Orientation
            If polyA_Orientation = PolyA_Type.None And cap_Orientation <> PolyA_Type.None Then Return cap_Orientation
            If cap_Orientation = PolyA_Type.None And polyA_Orientation <> PolyA_Type.None Then Return polyA_Orientation
            If polyA_Orientation = PolyA_Type.Both Then
                If cap_Orientation = PolyA_Type.polyA Then
                    ' Return PolyA_Type.polyA_falsePolyT
                    Return PolyA_Type.polyA
                ElseIf cap_Orientation = PolyA_Type.polyT Then
                    ' Return PolyA_Type.polyT_falsePolyA
                    Return PolyA_Type.polyT
                Else
                    Return polyA_Orientation
                End If
            End If
            If cap_Orientation = PolyA_Type.Both Then
                If polyA_Orientation = PolyA_Type.polyA Then
                    ' Return PolyA_Type.polyA_falsePolyT
                    Return PolyA_Type.polyA
                ElseIf polyA_Orientation = PolyA_Type.polyT Then
                    Return PolyA_Type.polyT
                    '  Return PolyA_Type.polyT_falsePolyA
                Else
                    Return polyA_Orientation
                End If
            End If
            Return mRNA.PolyA_Type.Both
        End Function

        ''' <summary>
        ''' Enter Orientation For SAM.Flag
        ''' </summary>
        ''' <param name="sam"></param>
        ''' <param name="final_Orientation"></param>
        Public Shared Sub Set_SAM_Flag(ByRef sam As SAMAlignedSequence, final_Orientation As PolyA_Type)
            Select Case final_Orientation

                Case PolyA_Type.polyA
                    sam.Flag = 0
                Case PolyA_Type.polyA_falsePolyT
                    sam.Flag = 0
                Case PolyA_Type.polyT
                    sam.Flag = Bio.IO.SAM.SAMFlags.QueryOnReverseStrand
                Case PolyA_Type.polyT_falsePolyA
                    sam.Flag = Bio.IO.SAM.SAMFlags.QueryOnReverseStrand
            End Select
        End Sub

        ''' <summary>
        ''' Merge All Orientation
        ''' </summary>
        ''' <param name="SAM"></param>
        ''' <param name="ls"></param>
        ''' <returns></returns>
        Public Shared Function Get_Orientation(SAM As SAMAlignedSequence,
                                               ls As Dictionary(Of Szunyi.Transcipts.Adaptor_Pair, Szunyi.Transcipts.Adapter_Filtering), Optional Filter As Szunyi.mRNA.PolyA_Type = Nothing) As Szunyi.mRNA.PolyA_Type

            Dim Orientations As New List(Of Szunyi.mRNA.PolyA_Type)
            For Each l In ls
                Orientations.Add(Get_Orientation(SAM, l, Filter))
            Next

            If Orientations.Count > 1 Then
                    Dim M = Szunyi.Transcipts.Orientation.Merge(Orientations(0), Orientations(1))
                    For i1 = 2 To Orientations.Count - 1
                        M = Szunyi.Transcipts.Orientation.Merge(M, Orientations(i1))
                    Next
                    Return M
                Else
                    Return Orientations.First
                End If


        End Function
        Public Shared Function Get_Orientation(SAM As SAMAlignedSequence, first As KeyValuePair(Of Adaptor_Pair, Adapter_Filtering)) As Szunyi.mRNA.PolyA_Type
            Dim Filter = first.Value
            Dim AD = first.Key

            Dim cData = Szunyi.BAM.BAM_Optional_Filed_Manipulation.Get_Adapter_Value(SAM, AD)

            If cData.Five_Prime_Score >= Filter.Five_Prime_Adaptor_Score Then
                If cData.Three_Prime_Score >= Filter.Three_Prime_Adaptor_Score Then
                    Return Szunyi.mRNA.PolyA_Type.Both
                Else
                    If cData.Five_Prime_Pos_in_Adapter >= Filter.Five_Prime_Adaptor_Minimal_Position And cData.Five_Prime_Pos_in_Adapter <= Filter.Five_Prime_Adaptor_Maximal_Position Then
                        Return Szunyi.mRNA.PolyA.Reverse(Szunyi.mRNA.PolyA_Type.polyA, AD.isReverse)
                    Else
                        Return Szunyi.mRNA.PolyA_Type.None
                    End If
                    Return Szunyi.mRNA.PolyA.Reverse(Szunyi.mRNA.PolyA_Type.polyT, AD.isReverse)
                End If
            Else
                If cData.Three_Prime_Score >= Filter.Three_Prime_Adaptor_Score Then
                    If cData.Three_Prime_Pos_in_Adapter >= Filter.Three_Prime_Adaptor_Minimal_Position And cData.Three_Prime_Pos_in_Adapter <= Filter.Three_Prime_Adaptor_Maximal_Position Then
                        Return Szunyi.mRNA.PolyA.Reverse(Szunyi.mRNA.PolyA_Type.polyA, AD.isReverse)
                    Else
                        Return Szunyi.mRNA.PolyA_Type.None
                    End If

                Else
                        Return Szunyi.mRNA.PolyA_Type.None
                End If
            End If
        End Function
        Public Shared Function Get_Orientation(SAM As SAMAlignedSequence, first As KeyValuePair(Of Adaptor_Pair, Adapter_Filtering),
                                                Filter_For As Szunyi.mRNA.PolyA_Type) As Szunyi.mRNA.PolyA_Type
            Dim Filter = first.Value
            Dim AD = first.Key
            If Filter_For = 0 Then
                Return Get_Orientation(SAM, first)
            Else

                If first.Value.Five_Prime_For_PAS = True AndAlso Filter_For = PolyA_Type.For_PAS Then
                    Return Get_Orientation(SAM, first)
                ElseIf first.Value.Five_Prime_For_TSS = True AndAlso Filter_For = PolyA_Type.For_TSS Then
                    Return Get_Orientation(SAM, first)
                End If
            End If
            Return PolyA_Type.None
        End Function


        Friend Shared Function Get_Orientation(aLL_Orientation As IEnumerable(Of Type)) As Type
            Dim out = aLL_Orientation.First
            For Each o In aLL_Orientation
                If o = Type.both Then Return o
                If o = Type.fw Then
                    If out = Type.rev Then Return Type.unknown
                    out = o
                ElseIf o = Type.rev Then
                    If out = Type.fw Then Return Type.unknown
                    out = o
                Else ' Unknown orientation Do Nothing

                End If
            Next
            Return out
        End Function
    End Class
    Public Class Adaptor_Pairs
        Public Property A_Ps As New List(Of Adaptor_Pair)
        Public Sub New()
            Dim x As New Adaptors

            A_Ps.Add(New Adaptor_Pair("d", "cDNA MinIon Short", x.MinIon_Adaptor_5, x.MinIon_Adaptor_5_RevC, True))
            A_Ps.Add(New Adaptor_Pair("e", "cDNA MinIon wPolyA Tail", x.MinIon_Adaptor_5_wPCR, x.MinIon_Adaptor_3_RevC_wPolyA, True))
            A_Ps.Add(New Adaptor_Pair("a", "polyA-T", x.PolyT, x.PolyA, False))
            A_Ps.Add(New Adaptor_Pair("c", "CAP", x.Cap_Adaptor_5, x.Cap_Adaptor_3, True))
            A_Ps.Add(New Adaptor_Pair("p", "PacBio", x.Pacbio_5_Adaptor, x.PacBio_3_Adaptor, True))

        End Sub
    End Class
    <Serializable>
    Public Class Adaptor_Pair
        Public Sub New(Prefix As String, Name As String, minIon_Adaptor_5 As Sequence, minIon_Adaptor_3_RevC As Sequence, isReverse As Boolean)
            Me.PreFix = Prefix
            Me.Name = Name
            Me.Five_Prime_Adapter = minIon_Adaptor_5
            Me.Three_Prime_Adapter = minIon_Adaptor_3_RevC
            Me.isReverse = isReverse
        End Sub
        Public Property PreFix As String
        Public Property Name As String
        Public Property Five_Prime_Adapter As Bio.Sequence
        Public Property Three_Prime_Adapter As Bio.Sequence
        Public Property isReverse As Boolean
        Public Overrides Function ToString() As String
            Return PreFix & "," & Name & "," & Five_Prime_Adapter.ConvertToString & "," & Three_Prime_Adapter.ConvertToString & "," & isReverse
        End Function
    End Class
    Public Class Adaptor_Aligner_Result
        Public Property U As Bio.ISequence
        Public Property D As Bio.ISequence
        Public Property r_f As List(Of IPairwiseSequenceAlignment)
        Public Property r_t As List(Of IPairwiseSequenceAlignment)
        Public Sub New(u As ISequence, d As ISequence, r_f As List(Of IPairwiseSequenceAlignment), r_t As List(Of IPairwiseSequenceAlignment))
            Me.U = u
            Me.D = d
            Me.r_f = r_f
            Me.r_t = r_t
        End Sub

    End Class
    Public Class Adaptor_Aligner_Result_Manipulation
        Public Shared Function Get_Description(ADS As Dictionary(Of Szunyi.Transcipts.Adaptor_Pair, Szunyi.Transcipts.Adapter_Filtering)) As String
            Dim str As New System.Text.StringBuilder
            For Each AD In ADS
                str.Append("#")
                str.Append(AD.ToString)
                str.AppendLine()
            Next
            If str.Length > 0 Then
                str.Length -= 2

                Return str.ToString
            Else
                Return String.Empty
            End If
        End Function
        Public Shared Function Get_TSSs_By_Positions(Sams As List(Of Bio.IO.SAM.SAMAlignedSequence), ADs As Dictionary(Of Szunyi.Transcipts.Adaptor_Pair, Szunyi.Transcipts.Adapter_Filtering)) As List(Of Bio.IO.SAM.SAMAlignedSequence)
            Dim out As New List(Of SAMAlignedSequence)
            For Each Sam In Sams
                If Is_Good_By_Position_TSS(Sam, ADs) = True Then out.Add(Sam)
            Next
            Return out
        End Function
        Private Shared Function Is_Good_By_Position_TSS(Sam As SAMAlignedSequence, ADs As Dictionary(Of Szunyi.Transcipts.Adaptor_Pair, Adapter_Filtering)) As Boolean

            For Each AD In ADs
                Dim cData = Szunyi.BAM.BAM_Optional_Filed_Manipulation.Get_Adapter_Value(Sam, AD.Key)
                ' Three Prime Adaptor
                If AD.Value.Three_Prime_For_TSS = True Then
                    If AD.Value.Three_Prime_Adaptor_Score <= cData.Three_Prime_Score Then ' Score
                        If AD.Value.Three_Prime_Query_Minimal_Position <= cData.Three_Prime_Pos AndAlso AD.Value.Three_Prime_Query_Maximal_Position >= cData.Three_Prime_Pos Then ' Query
                            If AD.Value.Three_Prime_Adaptor_Minimal_Position <= cData.Three_Prime_Pos_in_Adapter AndAlso AD.Value.Three_Prime_Adaptor_Maximal_Position >= cData.Three_Prime_Pos_in_Adapter Then
                                Return True
                            End If
                        End If
                    End If

                End If

                If AD.Value.Five_Prime_For_TSS = True Then
                    If AD.Value.Five_Prime_Adaptor_Score <= cData.Five_Prime_Score Then ' Score
                        If AD.Value.Five_Prime_Query_Minimal_Position <= cData.Five_Prime_Pos AndAlso AD.Value.Five_Prime_Query_Maximal_Position >= cData.Five_Prime_Pos Then ' Query
                            If AD.Value.Five_Prime_Adaptor_Minimal_Position <= cData.Five_Prime_Pos_in_Adapter AndAlso AD.Value.Five_Prime_Adaptor_Maximal_Position >= cData.Five_Prime_Pos_in_Adapter Then
                                Return True
                            End If
                        End If
                    End If

                End If
            Next
            Return False
        End Function
        ''' <summary>
        ''' Return Score Passed the filter or Not, DIff Not implemented, position out of interest
        ''' </summary>
        ''' <param name="Sams"></param>
        ''' <param name="ADs"></param>
        ''' <returns></returns>
        Public Shared Function Get_PASs_By_Positions(Sams As List(Of Bio.IO.SAM.SAMAlignedSequence), ADs As Dictionary(Of Szunyi.Transcipts.Adaptor_Pair, Szunyi.Transcipts.Adapter_Filtering)) As List(Of Bio.IO.SAM.SAMAlignedSequence)
            Dim out As New List(Of SAMAlignedSequence)
            For Each Sam In Sams
                If Is_Good_By_Position_PAS(Sam, ADs) = True Then out.Add(Sam)
            Next
            Return out
        End Function
        Private Shared Function Is_Good_By_Position_PAS(Sam As SAMAlignedSequence, ADs As Dictionary(Of Szunyi.Transcipts.Adaptor_Pair, Adapter_Filtering)) As Boolean

            For Each AD In ADs
                Dim cData = Szunyi.BAM.BAM_Optional_Filed_Manipulation.Get_Adapter_Value(Sam, AD.Key)
                ' Three Prime Adaptor
                If AD.Value.Three_Prime_For_PAS = True Then
                    If AD.Value.Three_Prime_Adaptor_Score <= cData.Three_Prime_Score Then ' Score
                        If AD.Value.Three_Prime_Query_Minimal_Position <= cData.Three_Prime_Pos AndAlso AD.Value.Three_Prime_Query_Maximal_Position >= cData.Three_Prime_Pos Then ' Query
                            If AD.Value.Three_Prime_Adaptor_Minimal_Position <= cData.Three_Prime_Pos_in_Adapter AndAlso AD.Value.Three_Prime_Adaptor_Maximal_Position >= cData.Three_Prime_Pos_in_Adapter Then
                                Return True
                            End If
                        End If
                    End If

                End If

                If AD.Value.Five_Prime_For_PAS = True Then
                    If AD.Value.Five_Prime_Adaptor_Score <= cData.Five_Prime_Score Then ' Score
                        If AD.Value.Five_Prime_Query_Minimal_Position <= cData.Five_Prime_Pos AndAlso AD.Value.Five_Prime_Query_Maximal_Position >= cData.Five_Prime_Pos Then ' Query
                            If AD.Value.Five_Prime_Adaptor_Minimal_Position <= cData.Five_Prime_Pos_in_Adapter AndAlso AD.Value.Five_Prime_Adaptor_Maximal_Position >= cData.Five_Prime_Pos_in_Adapter Then
                                Return True
                            End If
                        End If
                    End If

                End If
            Next
            Return False
        End Function

        ''' <summary>
        ''' 
        ''' </summary>
        ''' <param name="AAR"></param>
        ''' <returns></returns>
        Public Shared Function Get_Scores(AAR As Adaptor_Aligner_Result) As String
            Return AAR.r_f.First.First.Score & vbTab & AAR.r_t.First.First.Score
        End Function
        Public Shared Function Get_AL(AAR As Adaptor_Aligner_Result) As String
            Return Get_Five_Seqs(AAR) & vbTab & Get_Three_Seqs(AAR)
        End Function
        Public Shared Function Get_Five_Prime_Pos_Adapter(aAR As Adaptor_Aligner_Result) As Integer
            Dim LastPos = (From x In aAR.r_f.First.AlignedSequences Select x.Metadata("EndOffSets")(1)).Max + 1
            Dim k As Integer = aAR.r_f.First.SecondSequence.Count

            Return LastPos - k
        End Function

        Public Shared Function Get_Three_Prime_Pos_Adapter(aAR As Adaptor_Aligner_Result) As String
            Dim FirstPos = (From x In aAR.r_t.First.AlignedSequences Select x.Metadata("StartOffSets")(1)).Min

            Return -FirstPos
        End Function
        ''' <summary>
        ''' - to 0 in S + in Match
        ''' </summary>
        ''' <param name="AAR"></param>
        ''' <returns></returns>
        Public Shared Function Get_Five_Prime_Pos(AAR As Adaptor_Aligner_Result) As Integer
            Dim LastPos = (From x In AAR.r_f.First.AlignedSequences Select x.Metadata("EndOffSets")(0)).Max + 1

            Dim SL = AAR.U.ID.Split(":").First.Length

            Return LastPos - SL

        End Function
        ''' <summary>
        ''' - to 0 in S + in Match
        ''' </summary>
        ''' <param name="AAR"></param>
        ''' <returns></returns>
        Public Shared Function Get_Three_Prime_Pos(AAR As Adaptor_Aligner_Result) As Integer

            Dim FirstPos = (From x In AAR.r_t.First.AlignedSequences Select x.Metadata("StartOffSets")(0)).Min

            Dim SL = AAR.D.ID.Split(":").Last.Length

            If FirstPos < SL Then
                Return SL - FirstPos
            Else
                Return -(FirstPos - SL)
            End If



        End Function
        Public Shared Function Get_Five_Prime_Score(AAR As Adaptor_Aligner_Result) As Double
            Return AAR.r_f.First.First.Score
        End Function
        Public Shared Function Get_Three_Prime_Score(AAR As Adaptor_Aligner_Result) As Double
            Return AAR.r_t.First.First.Score
        End Function
        Public Shared Function Get_Orientation(AAR As Adaptor_Aligner_Result, Score As Integer, Diff As Integer) As Szunyi.mRNA.PolyA_Type

            If IsNothing(AAR.U) = True AndAlso IsNothing(AAR.D) = True Then
                Return Szunyi.mRNA.PolyA_Type.None
            ElseIf IsNothing(AAR.D) = True Then

                If AAR.r_f.First.First.Score >= Score Then
                    Return Szunyi.mRNA.PolyA_Type.polyT
                Else
                    Return Szunyi.mRNA.PolyA_Type.None
                End If
            ElseIf IsNothing(AAR.U) = True Then

                If AAR.r_t.First.First.Score >= Score Then
                    Return Szunyi.mRNA.PolyA_Type.polyT
                Else
                    Return Szunyi.mRNA.PolyA_Type.None
                End If
            Else

                If AAR.r_f.First.First.Score >= Score AndAlso AAR.r_t.First.First.Score >= Score Then
                    Return Szunyi.mRNA.PolyA_Type.Both

                ElseIf AAR.r_f.First.First.Score >= Score Then
                    If System.Math.Abs(AAR.r_f.First.First.Score - AAR.r_t.First.First.Score) > Diff Then
                        Return Szunyi.mRNA.PolyA_Type.polyT
                    Else
                        Return Szunyi.mRNA.PolyA_Type.polyT_falsePolyA
                    End If

                ElseIf AAR.r_t.First.First.Score >= Score Then
                    If System.Math.Abs(AAR.r_t.First.First.Score - AAR.r_f.First.First.Score) > Diff Then
                        Return Szunyi.mRNA.PolyA_Type.polyA
                    Else
                        Return Szunyi.mRNA.PolyA_Type.polyA_falsePolyT
                    End If

                Else
                    Return Szunyi.mRNA.PolyA_Type.None
                End If
            End If

        End Function

        Public Shared Function Get_Three_Seqs(AAR As Adaptor_Aligner_Result) As String
            Dim str As New System.Text.StringBuilder
            str.Append(AAR.r_t.First.First.FirstSequence.ConvertToString).Append(vbTab)
            str.Append(AAR.r_t.First.First.SecondSequence.ConvertToString).Append(vbTab)
            str.Append(AAR.r_t.First.First.Consensus.ConvertToString)
            Return str.ToString
        End Function

        Public Shared Function Get_Five_Seqs(AAR As Adaptor_Aligner_Result) As String
            Dim str As New System.Text.StringBuilder
            str.Append(AAR.r_f.First.First.FirstSequence.ConvertToString).Append(vbTab)
            str.Append(AAR.r_f.First.First.SecondSequence.ConvertToString).Append(vbTab)
            str.Append(AAR.r_f.First.First.Consensus.ConvertToString)
            Return str.ToString
        End Function


    End Class
    Public Enum Sequencing_Type
        MinIon_CAP = 1
        MinIon_cDNA = 2
        PacBio = 3
        MinIon_dRNA
    End Enum
    ''' <summary>
    ''' Set Value and place to search For Adapters
    ''' </summary>
    Public Class Aligner_Setting
        Public Property Ouf_of_Match_Length As Integer = 30
        Public Property In_Match_Length As Integer = 10

        Public Property Sm_Aligner As New Bio.Algorithms.Alignment.SmithWatermanAligner
        Public Property sm As New Bio.SimilarityMatrices.DiagonalSimilarityMatrix(2, -3)
        Public Sub New(In_Match As Integer, Out_of_Match As Integer)
            Me.In_Match_Length = In_Match
            Me.Ouf_of_Match_Length = Out_of_Match
            Sm_Aligner.SimilarityMatrix = sm
            Sm_Aligner.GapOpenCost = -3
            Sm_Aligner.GapExtensionCost = -2

        End Sub
        Public Sub New(Type As Sequencing_Type, In_Match As Integer, Out_of_Match As Integer)
            Me.In_Match_Length = In_Match
            Me.Ouf_of_Match_Length = Out_of_Match
            Me.Sm_Aligner.SimilarityMatrix = sm
            Me.Sm_Aligner.GapOpenCost = -3
            Me.Sm_Aligner.GapExtensionCost = -2

        End Sub

        Public Sub Set_Header(ByRef header As SAMAlignmentHeader)
            header.Comments.Add("Diagonal value=" & Me.sm.DiagonalValue)
            header.Comments.Add("OFF Diagonal value=" & Me.sm.OffDiagonalValue)
            header.Comments.Add("GapOpenCost=" & Me.Sm_Aligner.GapOpenCost)
            header.Comments.Add("GapExtensionCost=" & Me.Sm_Aligner.GapExtensionCost)
            header.Comments.Add("In Match Length=" & Me.In_Match_Length)
            header.Comments.Add("Out Match Length=" & Me.Ouf_of_Match_Length)

        End Sub
    End Class
    Public Class Adaptors
        Public Property Pacbio_5_Adaptor As New Bio.Sequence(Bio.Alphabets.AmbiguousDNA, "AGAGTACATGGG")
        Public Property PacBio_3_Adaptor As New Bio.Sequence(Bio.Alphabets.AmbiguousDNA, "CCCATGTACTCT")

        Public Property PolyA As New Bio.Sequence(Bio.Alphabets.AmbiguousDNA, Szunyi.Text.General.Multiply("A", 35))
        Public Property PolyT As New Bio.Sequence(Bio.Alphabets.AmbiguousDNA, Szunyi.Text.General.Multiply("T", 35))

        Public Property MinIon_Adaptor_5 As New Bio.Sequence(Bio.Alphabets.AmbiguousDNA, "TGCCATTACGGCCGGG")
        Public Property MinIon_Adaptor_5_wPCR As New Bio.Sequence(Bio.Alphabets.AmbiguousDNA, "TTTCTGTTGGTGCTGATATTGCTGCCATTACGGCCGGG")
        Public Property MinIon_Adaptor_5_RevC As New Bio.Sequence(Bio.Alphabets.AmbiguousDNA, "CCCGGCCGTAATGGCA")
        Public Property MinIon_Adaptor_5_RevC_wPCR As New Bio.Sequence(Bio.Alphabets.AmbiguousDNA, "CCCGGCCGTAATGGCAGCAATATCAGCACCAACAGAAA")

        Public Property MinIon_Adaptor_3 As New Bio.Sequence(Bio.Alphabets.AmbiguousDNA, "ACTTGCCTGTCGCTCTATCTTC") ' Five prime before polyT PCR-VN
        Public Property MinIon_Adaptor_3_RevC As New Bio.Sequence(Bio.Alphabets.AmbiguousDNA, "GAAGATAGAGCGACAGGCAAGT") ' Three prime after polyA
        Public Property MinIon_Adaptor_3_wpolyT As New Bio.Sequence(Bio.Alphabets.AmbiguousDNA, "ACTTGCCTGTCGCTCTATCTTCTTTTTTTTTTTTTTTTTTTT") ' Five prime before polyT
        Public Property MinIon_Adaptor_3_RevC_wPolyA As New Bio.Sequence(Bio.Alphabets.AmbiguousDNA, "AAAAAAAAAAAAAAAAAAAAGAAGATAGAGCGACAGGCAAGT") ' Three prime after polyA

        Public Property Pacbio_Adaptors As New List(Of Bio.ISequence)
        Public Property MinIon_Adaptors As New List(Of Bio.ISequence)

        Public Property Cap_Adaptor_5 As New Bio.Sequence(Bio.Alphabets.AmbiguousDNA, "TGGATTGATATGTAATACGACTCACTATAG")
        Public Property Cap_PolyA_5 As New Bio.Sequence(Bio.Alphabets.AmbiguousDNA, "TCTCAGGCGTTTTTTTTTTTTTTTTTT")
        Public Property Cap_Adaptor_3 As New Bio.Sequence(Bio.Alphabets.AmbiguousDNA, "CTATAGTGAGTCGTATTACATATCAATCCA")
        Public Property Cap_PolyA_3 As New Bio.Sequence(Bio.Alphabets.AmbiguousDNA, "AAAAAAAAAAAAAAAAAACGCCTGAGA")

        Public Property Cap_Adaptors As New List(Of Bio.ISequence)


        Public Property PolyAT As New List(Of Bio.ISequence)
        Public Property All As New List(Of Bio.ISequence)
        Public Sub New()
            Pacbio_3_Adaptor.ID = "Pacbio 3' adapter"
            PacBio_5_Adaptor.ID = "Pacbio 5' adapter"

            PolyA.ID = "polyA"
            PolyT.ID = "polyT"
            MinIon_Adaptor_3.ID = "MinIon 3' adapter"
            MinIon_Adaptor_3_RevC.ID = "MinIon 3' RevC adapter"
            MinIon_Adaptor_5.ID = "MinIon 5' adapter"
            MinIon_Adaptor_5_RevC.ID = "MinIon 5' RevC adapter"
            MinIon_Adaptor_3_wpolyT.ID = "MinIon 3' wPolyT adapter"
            MinIon_Adaptor_3_RevC_wPolyA.ID = "MinIon 3' wPolyA RevC adapter"
            MinIon_Adaptor_5_wPCR.ID = "MinIon_Adaptor_5_wPCR"
            MinIon_Adaptor_5_RevC_wPCR.ID = "MinIon_Adaptor_5_RevC_wPCR"

            Pacbio_Adaptors.Add(Pacbio_3_Adaptor)
            Pacbio_Adaptors.Add(PacBio_5_Adaptor)
            Cap_Adaptor_5.ID = "Cap_Adaptor_5"
            Cap_Adaptor_3.ID = "Cap_Adaptor_3"
            Cap_PolyA_5.ID = "Cap_PolyA_5"
            Cap_PolyA_3.ID = "Cap_PolyA_3"

            MinIon_Adaptors.Add(MinIon_Adaptor_5)
            MinIon_Adaptors.Add(MinIon_Adaptor_5_RevC)
            MinIon_Adaptors.Add(MinIon_Adaptor_3)
            MinIon_Adaptors.Add(MinIon_Adaptor_3_RevC)
            MinIon_Adaptors.Add(MinIon_Adaptor_3_wpolyT)
            MinIon_Adaptors.Add(MinIon_Adaptor_3_RevC_wPolyA)
            MinIon_Adaptors.Add(MinIon_Adaptor_5_wPCR)
            MinIon_Adaptors.Add(MinIon_Adaptor_5_RevC_wPCR)

            PolyAT.Add(PolyA)
            PolyAT.Add(PolyT)

            Cap_Adaptors.Add(Cap_Adaptor_3)
            Cap_Adaptors.Add(Cap_Adaptor_5)
            Cap_Adaptors.Add(Cap_PolyA_5)
            Cap_Adaptors.Add(Cap_PolyA_3)

            All.AddRange(Pacbio_Adaptors)
            All.AddRange(MinIon_Adaptors)
            All.AddRange(PolyAT)
            All.AddRange(Cap_Adaptors)

        End Sub
    End Class
End Namespace

