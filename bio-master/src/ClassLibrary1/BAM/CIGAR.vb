Imports Bio
Imports Bio.IO.SAM


Namespace Szunyi.BAM
    Public Class CIGAR
        Public Enum Keys
            M_D = 1
            M_D_I = 2
            M_D_I_N = 3
            S = 4
            H = 5
            H_S = 6
        End Enum
        Public Shared Function Set_M_FIrst_Last_In_CIgars(CIgars As List(Of KeyValuePair(Of String, Integer))) As List(Of KeyValuePair(Of String, Integer))
            Dim toRemove As New List(Of Integer)
            For i1 = 0 To CIgars.Count - 1
                If CIgars(i1).Key = "M" Then Exit For
                toRemove.Add(i1)
            Next
            For i1 = CIgars.Count - 1 To 0 Step -1
                If CIgars(i1).Key = "M" Then Exit For
                toRemove.Add(i1)
            Next
            toRemove.Sort()
            toRemove.Reverse()
            For Each Item In toRemove
                CIgars.RemoveAt(Item)
            Next
            Return CIgars
        End Function

        Public Shared Function Get_CIGARS(SAM As SAMAlignedSequence, Type As Keys) As List(Of KeyValuePair(Of String, Integer))
            Dim Cigars = Get_CIGARS(SAM)
            Select Case Type
                Case Keys.M_D
                    Dim res = From x In Cigars Where x.Key = "M" Or x.Key = "D"

                    Return res.ToList
                Case Keys.M_D_I
                    Dim res = From x In Cigars Where x.Key = "M" Or x.Key = "D" Or x.Key = "I"

                    Return res.ToList
                Case Keys.M_D_I_N
                    Dim res = From x In Cigars Where x.Key = "M" Or x.Key = "D" Or x.Key = "I" Or x.Key = "N"

                    Return res.ToList
                Case Keys.S
                    Dim res = From x In Cigars Where x.Key = "S"

                    Return res.ToList
                Case Keys.H
                    Dim res = From x In Cigars Where x.Key = "H"

                    Return res.ToList
                Case Keys.H_S
                    Dim res = From x In Cigars Where x.Key = "H" Or x.Key = "S"

                    Return res.ToList
            End Select
        End Function
        ''' <summary>
        ''' Get KeyValuePair From CIGAR string
        ''' </summary>
        ''' <param name="SA"></param>
        ''' <returns></returns>
        Public Shared Function Get_CIGARS_woH_I(SA As Bio.IO.SAM.SAMAlignedSequence) As List(Of KeyValuePair(Of String, Integer))
            Dim CIGARS As New List(Of KeyValuePair(Of String, Integer))
            Dim cI As String = ""
            For i1 = 0 To SA.CIGAR.Count - 1
                Dim s As String = SA.CIGAR(i1)
                Dim i As Integer = 0
                If Integer.TryParse(s, 1) Then
                    cI = cI & s
                Else
                    If cI <> "" Then
                        Dim t As New KeyValuePair(Of String, Integer)(s, cI)
                        If t.Key <> "H" Then
                            If t.Key <> "I" Then CIGARS.Add(t)
                        End If

                        cI = ""
                    End If
                End If

            Next
            Return CIGARS
        End Function
        ''' <summary>
        ''' Get KeyValuePair From CIGAR string
        ''' </summary>
        ''' <param name="SA"></param>
        ''' <returns></returns>
        Public Shared Function Get_CIGARS_woH(SA As Bio.IO.SAM.SAMAlignedSequence) As List(Of KeyValuePair(Of String, Integer))
            Dim CIGARS As New List(Of KeyValuePair(Of String, Integer))
            Dim cI As String = ""
            For i1 = 0 To SA.CIGAR.Count - 1
                Dim s As String = SA.CIGAR(i1)
                Dim i As Integer = 0
                If Integer.TryParse(s, 1) Then
                    cI = cI & s
                Else
                    If cI <> "" Then
                        Dim t As New KeyValuePair(Of String, Integer)(s, cI)
                        If t.Key <> "H" Then CIGARS.Add(t)
                        cI = ""
                        End If
                    End If

            Next
            Return CIGARS
        End Function

        ''' <summary>
        ''' Get KeyValuePair From CIGAR string
        ''' </summary>
        ''' <param name="SA"></param>
        ''' <returns></returns>
        Public Shared Function Get_CIGARS(SA As Bio.IO.SAM.SAMAlignedSequence) As List(Of KeyValuePair(Of String, Integer))
            Dim CIGARS As New List(Of KeyValuePair(Of String, Integer))
            Dim cI As String = ""
            For i1 = 0 To SA.CIGAR.Count - 1
                Dim s As String = SA.CIGAR(i1)
                Dim i As Integer = 0
                If Integer.TryParse(s, 1) Then
                    cI = cI & s
                Else
                    If cI <> "" Then
                        Dim t As New KeyValuePair(Of String, Integer)(s, cI)
                        CIGARS.Add(t)
                        cI = ""
                    End If
                End If

            Next
            Return CIGARS
        End Function
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
        Public Shared Function Get_Smallest_Length(SAM As SAMAlignedSequence) As Integer
            Dim MdCigars = Get_CIGARS(SAM.CIGAR)
            Return (From x In MdCigars Where x.Key = "M" Order By x.Value Ascending Select x.Value).First
        End Function
        'Check First and Last Key for "H"
        Public Shared Function Has_Hard_Clip(SAM As SAMAlignedSequence) As Boolean
            Dim MdCigars = Get_CIGARS(SAM.CIGAR)
            If MdCigars.First.Key = "H" Or MdCigars.Last.Key = "H" Then
                Return True
            Else
                Return False
            End If
        End Function
        Public Shared Function Is_Correct_S(SAM As SAMAlignedSequence) As Boolean
            Dim MdCigars = Get_CIGARS(SAM.CIGAR)
            If MdCigars.First.Key = "S" Then
                If MdCigars(1).Key <> "M" Then
                    Return False
                End If
            End If
            If MdCigars.Last.Key = "S" Then
                If MdCigars(MdCigars.Count - 2).Key <> "M" Then
                    Return False
                End If
            End If
            Return True
        End Function
        Public Shared Function Get_CIGAR_String(mdCigars As List(Of KeyValuePair(Of String, Integer))) As String
            Dim str As New System.Text.StringBuilder
            For Each item In mdCigars
                str.Append(item.Value).Append(item.Key)
            Next
            Return str.ToString
        End Function
        Public Shared Function Get_CIGAR_String(mdCigars As MdCIgars) As String
            Dim str As New System.Text.StringBuilder
            For Each item In mdCigars.MdCIgars
                str.Append(item.Length).Append(item.Type)
            Next
            Return str.ToString
        End Function
#Region "Get Clipped Regions"
        ''' <summary>
        ''' Retrun five part hard clip position or -1
        ''' </summary>
        ''' <param name="sAM"></param>
        ''' <returns></returns>
        Public Shared Function Get_Five_Hard_Clip(sAM As SAMAlignedSequence) As Integer
            Dim CIgars = Get_CIGARS(sAM.CIGAR)
            If CIgars.First.Key = "H" Then Return CIgars.First.Value
            Return -1

        End Function
        ''' <summary>
        ''' Retrun five part soft clip position or -1
        ''' </summary>
        ''' <param name="sAM"></param>
        ''' <returns></returns>
        Public Shared Function Get_Five_Soft_Clip(sAM As SAMAlignedSequence) As Integer
            Dim CIgars = Get_CIGARS(sAM.CIGAR)
            If CIgars.First.Key = "S" Then Return CIgars.First.Value
            Return -1
        End Function
        ''' <summary>
        ''' Retrun three part hard clip position or -1
        ''' </summary>
        ''' <param name="sAM"></param>
        ''' <returns></returns>
        Public Shared Function Get_Three_Hard_Clip(sAM As SAMAlignedSequence) As Integer
            Dim CIgars = Get_CIGARS(sAM.CIGAR)
            If CIgars.Last.Key = "H" Then Return CIgars.Last.Value
            Return -1
        End Function
        ''' <summary>
        ''' Retrun three part soft clip position or -1
        ''' </summary>
        ''' <param name="sAM"></param>
        ''' <returns></returns>
        Public Shared Function Get_Three_Soft_Clip(sAM As SAMAlignedSequence) As Integer
            Dim CIgars = Get_CIGARS(sAM.CIGAR)
            If CIgars.Last.Key = "S" Then Return CIgars.Last.Value
            Return -1
        End Function
#End Region

#Region "Introns Insertions"
        ''' <summary>
        ''' Look for D,I,N
        ''' </summary>
        ''' <param name="SAM"></param>
        ''' <param name="Length"></param>
        ''' <returns></returns>
        Public Shared Function Has_Longer_Insertion(SAM As Bio.IO.SAM.SAMAlignedSequence, Optional Length As Integer = 1000) As Boolean
            Dim CIGARS = Get_CIGARS(SAM)
            Dim tmp = From x In CIGARS Where (x.Key = "D" Or x.Key = "I" Or x.Key = "N") And x.Value >= Length

            If tmp.Count = 0 Then Return False

            Return True
        End Function
        ''' <summary>
        ''' Look for D,I,N
        ''' </summary>
        ''' <param name="SAM"></param>
        ''' <param name="Length"></param>
        ''' <returns></returns>
        Public Shared Function Has_Smaller_Insertion(SAM As Bio.IO.SAM.SAMAlignedSequence, Optional Length As Integer = 1000) As Boolean
            Dim CIGARS = Get_CIGARS(SAM)
            Dim tmp = From x In CIGARS Where (x.Key = "D" Or x.Key = "I" Or x.Key = "N") And x.Value < Length

            If tmp.Count = 0 Then Return False

            Return True
        End Function
        ''' <summary>
        ''' Look for D,I,N
        ''' </summary>
        ''' <param name="CIGARs"></param>
        ''' <param name="Length"></param>
        ''' <returns></returns>
        Public Shared Function Has_Longer_Insertion(CIGARs As List(Of KeyValuePair(Of String, Integer)), Optional Length As Integer = 1000) As Boolean

            Dim tmp = From x In CIGARs Where (x.Key = "D" Or x.Key = "I" Or x.Key = "N") And x.Value >= Length

            If tmp.Count = 0 Then Return False

            Return True
        End Function
        ''' <summary>
        ''' Look for I,D,N
        ''' </summary>
        ''' <param name="SAM"></param>
        ''' <param name="min"></param>
        ''' <param name="max"></param>
        ''' <returns></returns>
        Public Shared Function Has_Insertion_Between(SAM As Bio.IO.SAM.SAMAlignedSequence, min As Integer, max As Integer) As Boolean
            Dim CIGARS = Get_CIGARS(SAM)
            Dim tmp = From x In CIGARS Where (x.Key = "D" Or x.Key = "I" Or x.Key = "N") And x.Value >= min And x.Value <= max

            If tmp.Count = 0 Then
                If min = 0 Then Return True
                Return False
            End If

            Return True
        End Function

        ''' <summary>
        ''' Look for I,D,N
        ''' </summary>
        ''' <param name="CIGARs"></param>
        ''' <param name="min"></param>
        ''' <param name="max"></param>
        ''' <returns></returns>
        Public Shared Function Has_Insertion_Between(CIGARs As List(Of KeyValuePair(Of String, Integer)), min As Integer, max As Integer) As Boolean

            Dim tmp = From x In CIGARs Where (x.Key = "D" Or x.Key = "I" Or x.Key = "N") And x.Value >= min And x.Value <= max

            If tmp.Count = 0 Then Return False

            Return True
        End Function
        Public Shared Function Get_Biggest_Intron_Length(SAM As SAMAlignedSequence) As Integer
            Dim CIgars = Get_CIGARS(SAM.CIGAR)
            Dim Deletions = From x In CIgars Where x.Key = "N" Or x.Key = "D" Order By x.Value Descending

            If Deletions.Count > 0 Then
                Return Deletions.First.Value
            Else
                Return -1
            End If
        End Function
        Public Shared Function Get_Biggest_Intron_Length(Cigars As List(Of KeyValuePair(Of String, Integer))) As Integer
            Dim Deletions = From x In Cigars Where x.Key = "N" Or x.Key = "D" Order By x.Value Descending

            If Deletions.Count > 0 Then
                Return Deletions.First.Value
            Else
                Return -1
            End If
        End Function
#End Region

#Region "Lengths"
        ''' <summary>
        ''' Look For M and I and return Sum
        ''' </summary>
        ''' <param name="CIGAR"></param>
        ''' <returns></returns>
        Public Shared Function Get_CIGAR_Length_wo_ClippedRegions(CIGAR As String) As Integer
            Dim x1 = Get_CIGARS(CIGAR)
            Dim count As Integer = 0
            For Each s In x1
                If s.Key = "M" Or s.Key = "I" Then
                    count += s.Value
                End If
            Next
            Return count
        End Function
        ''' <summary>
        ''' Look For M and I and return Sum
        ''' </summary>
        ''' <param name="CIGARs"></param>
        ''' <returns></returns>
        Public Shared Function Get_CIGAR_Length_wo_ClippedRegions(CIGARs As List(Of KeyValuePair(Of String, Integer))) As Integer

            Dim count As Integer = 0
            For Each s In CIGARs
                If s.Key = "M" Or s.Key = "I" Then
                    count += s.Value
                End If
            Next
            Return count
        End Function
        ''' <summary>
        ''' M +I Count
        ''' </summary>
        ''' <param name="Cigar"></param>
        ''' <returns></returns>
        Public Shared Function Get_Cigar_Match_Length(Cigar As String) As Integer
            Dim x1 = Get_CIGARS(Cigar)
            Dim count As Integer = 0
            For Each s In x1
                If s.Key = "M" Or s.Key = "I" Then
                    count += s.Value
                End If
            Next
            Return count
        End Function
        ''' <summary>
        ''' M + I + H +S
        ''' </summary>
        ''' <param name="Cigar"></param>
        ''' <returns></returns>
        Public Shared Function Get_CIGAR_Full_Length(Cigar As String) As Integer
            Dim x1 = Get_CIGARS(Cigar)
            Dim count As Integer = 0
            For Each s In x1
                If s.Key = "M" Or s.Key = "I" Or s.Key = "S" Then
                    count += s.Value
                End If
            Next
            Return count
        End Function


#End Region
        ''' <summary>
        ''' Return length of First if key = S else return 0
        ''' </summary>
        ''' <param name="SAM"></param>
        ''' <returns></returns>
        Public Shared Function Get_First_S_Length(SAM As SAMAlignedSequence) As Integer
            Dim c = Get_CIGARS(SAM.CIGAR)
            If c.First.Key = "S" Then Return c.First.Value
            Return 0
        End Function

        Public Shared Function Get_Last_S_Length(SAM As SAMAlignedSequence) As Integer
            Dim c = Get_CIGARS(SAM.CIGAR)
            If c.Last.Key = "S" Then Return c.Last.Value
            Return 0
        End Function

        Friend Shared Function Modify_S(sAM As SAMAlignedSequence, nofNA_to_Remove_5 As Integer, nofNA_to_Remove_3 As Integer) As String
            Dim c = Get_CIGARS(sAM.CIGAR)
            If c.First.Key = "S" Then
                Dim x As New KeyValuePair(Of String, Integer)("S", c.First.Value - nofNA_to_Remove_5)
                c.RemoveAt(0)
                c.Insert(0, x)
            End If

            If c.Last.Key = "S" Then
                Dim x As New KeyValuePair(Of String, Integer)("S", c.Last.Value - nofNA_to_Remove_3)
                c.RemoveAt(c.Count - 1)
                c.Add(x)
            End If
            Return Szunyi.BAM.CIGAR.Get_CIGAR_String(c)
        End Function

        Friend Shared Function Get_Length_woHS(SAM As SAMAlignedSequence) As Integer
            Dim cs = Get_CIGARS(SAM.CIGAR)
            Dim toRemove As Integer = 0
            For Each c In cs
                If c.Key = "S" Then
                    toRemove += c.Value
                End If
            Next
            Return SAM.QuerySequence.Count - toRemove
        End Function

        Friend Shared Function Get_Lengths_woHS(SAMs As List(Of SAMAlignedSequence)) As List(Of Integer)
            Dim out As New List(Of Integer)
            For Each SAM In SAMs
                out.Add(Get_Length_woHS(SAM))
            Next
            Return out
        End Function
#Region "MD_CIGARS"
        Public Shared Function GetMdCIgars(SA As Bio.IO.SAM.SAMAlignedSequence, RefSeq As Bio.Sequence) As MdCIgars
            Dim out As New List(Of MdCigar)
            Dim Q_Start As Integer = 1
            Dim Ref_Start As Integer = SA.Pos
            SA.QuerySequence = Szunyi.Sequences.SequenceManipulation.Common.GetSeqAsBioSeq(SA.QuerySequence)
            For Each Cigar In Get_CIGARS(SA.CIGAR)
                Try
                    Dim mdCIgar As New MdCigar()
                    Select Case Cigar.Key
                        Case "N"
                            mdCIgar = New MdCigar(Cigar.Key, Q_Start, Q_Start, Ref_Start, Ref_Start + Cigar.Value - 1, SA.QuerySequence, RefSeq, Cigar.Value)
                            Ref_Start += Cigar.Value
                        Case "I"
                            mdCIgar = New MdCigar(Cigar.Key, Q_Start - 1, Q_Start + Cigar.Value, Ref_Start, Ref_Start, SA.QuerySequence, RefSeq, Cigar.Value)
                            Q_Start += Cigar.Value
                        Case "M"
                            mdCIgar = New MdCigar(Cigar.Key, Q_Start - 1, Q_Start + Cigar.Value, Ref_Start - 1, Ref_Start + Cigar.Value - 1, SA.QuerySequence, RefSeq, Cigar.Value)
                            Q_Start += Cigar.Value
                            Ref_Start += Cigar.Value
                        Case "H"
                            Dim alf As Integer = 54
                        Case "D"
                            mdCIgar = New MdCigar(Cigar.Key, Q_Start, Q_Start, Ref_Start - 1, Ref_Start + Cigar.Value - 1, SA.QuerySequence, RefSeq, Cigar.Value)
                            Ref_Start += Cigar.Value
                        Case "S"
                            mdCIgar = New MdCigar(Cigar.Key, Q_Start - 1, Q_Start + Cigar.Value, Ref_Start, Ref_Start, SA.QuerySequence, RefSeq, Cigar.Value)
                            Q_Start += Cigar.Value

                        Case Else
                            Dim alf As Int16 = 54
                    End Select
                    out.Add(mdCIgar)
                Catch ex As Exception
                    Dim kj As Int16 = 65
                End Try

            Next
            Dim m As New MdCIgars
            m.MdCIgars = out
            Return m
        End Function

#End Region

#Region "Alignments"
        Public Shared Function Get_Ref_Alignmnet(Sam As SAMAlignedSequence, Ref_Seq As Bio.Sequence) As Bio.Sequence
            Return Get_Ref_Alignment(GetMdCIgars(Sam, Ref_Seq), Ref_Seq)

        End Function

        Public Shared Function Get_Ref_Alignment(x As MdCIgars, Ref_Seq As Bio.Sequence) As Bio.Sequence
            Dim str As New System.Text.StringBuilder
            For Each CIgar In x.MdCIgars
                Select Case CIgar.Type
                    Case "S"
                        str.Append(Szunyi.Text.General.Multiply("-", CIgar.Length))
                    Case "M"
                        str.Append(CIgar.Ref_Seq.ConvertToString)
                    Case "I"
                        str.Append(Szunyi.Text.General.Multiply("-", CIgar.Length))
                    Case "D"
                        str.Append(CIgar.Ref_Seq.ConvertToString)

                    Case "N"
                        '  str.Append(Szunyi.Text.General.Multiply("?", CIgar.Length))
                End Select
            Next

            Return New Bio.Sequence(Alphabets.AmbiguousDNA, str.ToString)


        End Function
        Public Shared Function Get_Read_Alignment(x As MdCIgars, Ref_Seq As Bio.Sequence, Read_Seq As Bio.Sequence) As Bio.Sequence
            'fw
            Dim str As New System.Text.StringBuilder
            For Each CIgar In x.MdCIgars
                Select Case CIgar.Type
                    Case "S"
                        str.Append(CIgar.Query_Seq.ConvertToString)
                    Case "M"
                        str.Append(CIgar.Query_Seq.ConvertToString)
                    Case "I"
                        str.Append(CIgar.Query_Seq.ConvertToString)
                    Case "D" ' Deletion
                        str.Append(Szunyi.Text.General.Multiply("-", CIgar.Length))
                    Case "N" ' Intron
                        '  str.Append(Szunyi.Text.General.Multiply("?", CIgar.Length))
                End Select
            Next
            Return New Bio.Sequence(Alphabets.AmbiguousDNA, str.ToString)


        End Function

        Public Shared Function Get_Read_Alignment(Sam As SAMAlignedSequence, Ref_Seq As Bio.Sequence, Read_Seq As Bio.Sequence) As Bio.Sequence

            Return Get_Read_Alignment(GetMdCIgars(Sam, Ref_Seq), Ref_Seq, Read_Seq)


        End Function

#End Region
    End Class

    Public Class MdCIgars
        Public Property MdCIgars As List(Of MdCigar)
        Public Overrides Function ToString() As String
            Dim str As New System.Text.StringBuilder
            For Each m In MdCIgars
                str.Append(m.Length).Append(",")
                str.Append(m.Query_End).Append(",")
                If IsNothing(m.Query_Seq) = True Then
                    str.Append(",")
                    str.Append(",")
                Else
                    str.Append(m.Query_Seq.ID).Append(",")
                    str.Append(m.Query_Seq.ConvertToString).Append(",")
                End If
                str.Append(m.Query_Start).Append(",")
                str.Append(m.Ref_End).Append(",")
                If IsNothing(m.Ref_Seq) = True Then
                    str.Append(",")
                    str.Append(",")
                Else
                    str.Append(m.Ref_Seq.ID).Append(",")
                    str.Append(m.Ref_Seq.ConvertToString).Append(",")
                End If

                str.Append(m.Ref_Start).Append(",")
                str.Append(m.Type).Append(":")
            Next
            '   Dim t = Get_From_Line(str.ToString)
            Return str.ToString
        End Function
        Public Shared Function Get_From_Line(Line As String) As MdCIgars
            Dim m As New MdCIgars
            m.MdCIgars = New List(Of MdCigar)
            Dim l = Split(Line, ":")
            For Each MdCigar In l
                If MdCigar <> "" Then
                    Dim s = Split(MdCigar, ",")
                    Dim c As New MdCigar
                    c.Length = s(0)
                    c.Query_End = s(1)
                    c.Query_Seq = New Bio.Sequence(Alphabets.AmbiguousDNA, s(3))
                    c.Query_Seq.ID = s(2)
                    c.Query_Start = s(4)
                    c.Ref_End = s(5)
                    c.Ref_Seq = New Bio.Sequence(Alphabets.AmbiguousDNA, s(7))
                    c.Ref_Seq.ID = s(6)
                    c.Ref_Start = s(8)
                    c.Type = s(9)
                    m.MdCIgars.Add(c)
                End If
            Next
            Return m
        End Function

        ''' <summary>
        ''' Create from MD String
        ''' </summary>
        ''' <param name="SAM"></param>
        ''' <param name="Seq"></param>
        Public Sub New(SAM As SAMAlignedSequence, Seq As Bio.ISequence)
            Dim MD = (From x1 In SAM.OptionalFields Where x1.Tag = "MD").First.Value
            Dim Type As String = "UnKnown"
            Dim tmp As String = ""
            For i1 = 0 To MD.Count - 1
                If IsNumeric(MD(i1)) = True Then

                    tmp = tmp & MD(i1)
                    Type = "Match"
                ElseIf MD(i1) = "^" Then

                    Dim x As New MdCigar(tmp, Me, SAM, Type)
                    Me.MdCIgars.Add(x)
                    tmp = ""
                    Type = "Insertion"
                Else

                    If Type = "Match" Then

                        Dim x1 As New MdCigar(tmp, Me, SAM, Type)
                            tmp = ""
                            Me.MdCIgars.Add(x1)

                        End If


                        If Type <> "Insertion" Then Type = "MisMatch"
                    Dim x As New MdCigar(MD(i1), Me, SAM, Type)
                    Me.MdCIgars.Add(x)
                End If

                Dim kj As Int16 = 54
            Next
            If tmp.Length > 0 Then
                Dim x1 As New MdCigar(tmp, Me, SAM, Type)
                tmp = ""
                Me.MdCIgars.Add(x1)
            End If
            Dim sg As New List(Of MdCigar)
            sg.Add(Me.MdCIgars.First)
            For i1 = 1 To Me.MdCIgars.Count - 1
                If sg.Last.Type = MdCIgars(i1).Type Then
                    sg(sg.Count - 1).Length += MdCIgars(i1).Length
                    sg(sg.Count - 1).Query_Seq = New Bio.Sequence(Alphabets.DNA, sg(sg.Count - 1).Query_Seq.ConvertToString & Me.MdCIgars(i1).Query_Seq.ConvertToString)

                Else
                    sg.Add(MdCIgars(i1))
                End If
            Next
            Me.MdCIgars = sg
            Dim jhy = Me.toMD(sg)
            Dim jjj As Int16 = 54
        End Sub
        Public Sub New()

        End Sub
        Public Function toMD(mdcigars As List(Of MdCigar)) As String
            Dim str As New System.Text.StringBuilder
            For Each m In mdcigars
                Select Case m.Type
                    Case "MM"
                        str.Append(m.Query_Seq.ConvertToString)
                    Case "I"
                        str.Append("^")
                        str.Append(m.Query_Seq.ConvertToString)
                    Case "M"
                        str.Append(m.Length)
                End Select
            Next
            '   Dim t = Get_From_Line(str.ToString)
            Return str.ToString
        End Function
        Public Function toMD()
            Dim str As New System.Text.StringBuilder
            For Each m In MdCIgars
                Select Case m.Type
                    Case "MM"
                        str.Append(m.Query_Seq.ConvertToString)
                    Case "I"
                        str.Append("^")
                        str.Append(m.Query_Seq.ConvertToString)
                    Case "M"
                        str.Append(m.Length)
                End Select
            Next
            '   Dim t = Get_From_Line(str.ToString)
            Return str.ToString
        End Function
    End Class

    Public Class MdCigar
        Public Property Type As String
        Public Property Query_Start As Integer
        Public Property Query_End As Integer
        Public Property Ref_Start As Integer
        Public Property Ref_End As Integer
        Public Property Ref_Seq As Bio.Sequence
        Public Property Query_Seq As Bio.Sequence
        Public Property Length As Integer
        Public Sub New(s As String, mDs As MdCIgars, sam As SAMAlignedSequence, type As String)
            If IsNothing(mDs.MdCIgars) = True Then
                mDs.MdCIgars = New List(Of MdCigar)

            End If
            Select Case type
                Case "Match"
                    Me.Type = "M"
                    Me.Length = s
                Case "Insertion"
                    Me.Type = "I"
                    Me.Length = s.Length
                    Me.Query_Seq = New Bio.Sequence(Alphabets.DNA, s)
                Case "MisMatch"
                    Me.Type = "MM"
                    Me.Length = s.Length
                    Me.Query_Seq = New Bio.Sequence(Alphabets.DNA, s)
                    Me.Length = s.Length
            End Select
        End Sub
        Public Sub New()

        End Sub
        Public Sub New(Type As String, Q_Start As Integer, Q_End As Integer, REf_Start As Integer, Ref_end As Integer, QSeq As Bio.Sequence, Ref_Seq As Bio.Sequence, Length As Integer)
            Me.Type = Type
            Me.Query_End = Q_End
            Me.Query_Start = Q_Start
            Me.Ref_Start = REf_Start
            Me.Ref_End = Ref_end
            If Query_End <> Q_Start Then Me.Query_Seq = QSeq.GetSubSequence(Me.Query_Start, Length)
            If IsNothing(Ref_Seq) = False Then
                If REf_Start <> Ref_end Then Me.Ref_Seq = Ref_Seq.GetSubSequence(Me.Ref_Start, Length)
            End If

            Me.Length = Length
        End Sub
        Public Overrides Function ToString() As String
            Dim str As New System.Text.StringBuilder
            If IsNothing(Me.Ref_Seq) = True Then
                str.Append(vbTab)
            Else
                str.Append(Me.Ref_Seq.ID).Append(vbTab)
            End If
            If IsNothing(Me.Query_Seq) = True Then
                str.Append(vbTab)
            Else
                str.Append(Me.Query_Seq.ID).Append(vbTab)
            End If
            str.Append(Me.Type & vbTab & Me.Query_Start & vbTab & Me.Query_End & vbTab & Me.Ref_Start & vbTab & Me.Ref_End & vbTab & Me.Length & vbTab)
            If IsNothing(Me.Query_Seq) = False Then
                str.Append(Me.Query_Seq.ConvertToString)
            End If
            str.Append(vbTab)
            If IsNothing(Me.Ref_Seq) = False Then
                str.Append(Me.Ref_Seq.ConvertToString)
            End If
            Return str.ToString
        End Function
    End Class


End Namespace

