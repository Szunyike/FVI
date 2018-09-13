Imports System.IO
Imports Bio
Imports Bio.Algorithms.Alignment
Imports Bio.IO.SAM
Imports ClassLibrary1.Szunyi.Constants
Imports ClassLibrary1.Szunyi.BAM.SAM_Manipulation.QuerySequnce
Imports System.Text.RegularExpressions

Namespace Szunyi
    Namespace Alignment
        Public Class Own_Al
            Public Enum Type
                Hard_Ccip = 0
                Soft_Clip = 1
                Insertion = 2
                Deletion = 3
                MisMatch = 4
                Match = 5
                UnKnown = 6
                Intron = 7
            End Enum
            Public Property Sam As SAMAlignedSequence
            Public Property Parts As New List(Of Own_Al_Single_Part)
            Public Sub New(SAM As SAMAlignedSequence)
                Me.Sam = SAM

                Dim CIgars = Szunyi.BAM.CIGAR.Get_CIGARS(SAM, Szunyi.BAM.CIGAR.Keys.M_D)
                CIgars = Szunyi.BAM.CIGAR.Set_M_FIrst_Last_In_CIgars(CIgars)
                Dim CIgars_Full = Szunyi.BAM.CIGAR.Get_CIGARS(SAM, Szunyi.BAM.CIGAR.Keys.M_D_I_N)
                CIgars_Full = Szunyi.BAM.CIGAR.Set_M_FIrst_Last_In_CIgars(CIgars_Full)
                Dim Own_Cigars = Own_Al_Helper.Common.Cigars_to_Own(CIgars)
                Dim Own_Cigars_Full = Own_Al_Helper.Common.Cigars_to_Own(CIgars_Full)

                Dim Own_MDs = Get_Md(SAM)

                ' ^AAA is ambigous meaning ^A ant two MismActh or ^AAA
                Check_Correct_Deletions_MisMatches(Own_Cigars, Own_MDs)
                ' Insert Insertion, Introns to main MD (I,N)
                Insert_Insertions(Own_MDs, Own_Cigars_Full)
                ' Insert S
                Insert_S(Own_MDs, SAM)
                Dim Removed = Own_Al_Helper.Common.Remove_Zero_Matches(Own_MDs)


                Dim S = Szunyi.BAM.SAM_Manipulation.QuerySequnce.Get_wo_S(SAM)
                Set_Query_Positions(Removed, S.ConvertToString)

                ' M,MM,N+ length,I,D do Nothing
                Set_Reference_Positions(Own_MDs, SAM)

                Me.Parts = Removed


            End Sub
            Private Sub Set_Reference_Positions(Own_Mds As List(Of Own_Al_Single_Part), SAM As SAMAlignedSequence)
                Dim Increment As Integer = SAM.Pos
                For Each Item In Own_Mds
                    Select Case Item.Type
                        Case Own_Al.Type.Deletion
                            Item.Ref_Start = Increment
                            Item.Ref_End = Item.Ref_Start + Item.Length - 1
                            Increment += Item.Length
                        Case Own_Al.Type.Insertion

                        Case Own_Al.Type.Match
                            Item.Ref_Start = Increment
                            Item.Ref_End = Item.Ref_Start + Item.Length - 1
                            Increment += Item.Length

                        Case Own_Al.Type.MisMatch
                            Item.Ref_Start = Increment
                            Item.Ref_End = Item.Ref_Start + Item.Length - 1
                            Increment += Item.Length
                        Case Own_Al.Type.Intron
                            Item.Ref_Start = Increment
                            Item.Ref_End = Item.Ref_Start + Item.Length - 1
                            Increment += Item.Length
                    End Select
                Next
            End Sub
            Private Sub Insert_S(own_mds As List(Of Own_Al_Single_Part), SAM As SAMAlignedSequence)
                own_mds.RemoveAt(0)
                own_mds.Remove(own_mds.Last)
                Dim S = Szunyi.BAM.CIGAR.Get_First_S_Length(SAM)
                If S <> 0 Then
                    own_mds.Insert(0, (New Own_Al_Single_Part(SAM.QuerySequence.GetSubSequence(0, S))))
                End If
                Dim SL = Szunyi.BAM.CIGAR.Get_Last_S_Length(SAM)
                If SL <> 0 Then
                    own_mds.Add(New Own_Al_Single_Part(SAM.QuerySequence.GetSubSequence(SAM.QuerySequence.Count - SL, SL), own_mds.Last))
                End If
            End Sub
            Private Sub Insert_Insertions(Own_MDs As List(Of Own_Al_Single_Part), Own_Cigars_Full As List(Of Own_Al_Single_Part))
                Dim tmp = From x In Own_Cigars_Full Where x.Type = Type.Insertion Or x.Type = Type.Intron

                For Each I In tmp
                    Dim r = From x In Own_MDs Where x.Ref_Start <= I.Ref_Start And x.Ref_End >= I.Ref_Start
                    If r.Count = 1 Then
                        Dim the = r.First
                        Own_Al_Helper.Common.Split(Own_MDs, the, I)
                    Else

                        Dim Index = Own_MDs.IndexOf(r.Last)
                            Own_MDs.Insert(Index + 1, I)


                        End If

                Next
            End Sub
            Private Sub Set_Query_Positions(Own_MDs As List(Of Own_Al_Single_Part), QuerySeq As String)
                Dim Increase As Integer = 0
                For Each Item In Own_MDs
                    Try
                        If Item.Type = Type.Insertion Then

                            Item.Query_Seq = QuerySeq.Substring(Item.Query_Start - 1, Item.Length)
                            Increase += Item.Length
                            Dim kj As Int16 = 54
                        ElseIf Item.Type = Type.Deletion Then
                            Increase -= Item.Length
                        ElseIf Item.Type = Type.MisMatch Then
                            Item.Query_Seq = QuerySeq.Substring(Item.Ref_Start + Increase - 1, Item.Length)
                            Dim jk As Int16 = 54
                        ElseIf Item.Type = Type.Match Then
                            Item.Query_Seq = QuerySeq.Substring(Item.Ref_Start + Increase - 1, Item.Length)
                            Dim jk As Int16 = 54
                        Else
                            Dim hj As Int16 = 54
                        End If
                    Catch ex As Exception
                        Dim kj As Int16 = 54
                    End Try


                Next

            End Sub
            Private Sub Check_Correct_Deletions_MisMatches(Own_Cigars As List(Of Own_Al_Single_Part), Own_MDs As List(Of Own_Al_Single_Part))
                Dim Own_Md_Dels = From x In Own_MDs Where x.Type = Type.Deletion

                Dim Own_Cigars_Dels = From x In Own_Cigars Where x.Type = Type.Deletion

                For Each del In Own_Md_Dels.ToList
                    Dim res = From t In Own_Cigars_Dels Where t.Ref_Start = del.Ref_Start And t.Ref_End = del.Ref_End

                    If res.Count = 0 Then
                        Dim X = From t1 In Own_Cigars_Dels Where t1.Ref_Start = del.Ref_Start
                        Try
                            If X.Count = 1 Then
                                Dim Index = Own_MDs.IndexOf(del)
                                Dim Nof_Change_to_MisMatch = del.Ref_End - X.First.Ref_End
                                Dim ToInsert As New Own_Al_Single_Part()
                                ToInsert.Type = Type.MisMatch
                                ToInsert.Ref_Start = X.First.Ref_End + 1
                                ToInsert.Ref_End = del.Ref_End
                                ToInsert.Ref_Seq = del.Ref_Seq.Substring(del.Ref_Seq.Count - Nof_Change_to_MisMatch)
                                ToInsert.Length = Nof_Change_to_MisMatch
                                del.Ref_End -= Nof_Change_to_MisMatch
                                del.Ref_Seq = del.Ref_Seq.Substring(0, del.Ref_Seq.Count - Nof_Change_to_MisMatch)
                                del.Length -= 1
                                Own_MDs.Insert(Index + 1, ToInsert)
                                Dim kj As Int16 = 54
                            Else
                                Dim jk As Int16 = 54
                            End If
                        Catch ex As Exception

                        End Try

                    Else
                        Dim jk As Int16 = 65
                    End If
                Next
            End Sub
            Private Function Get_Md(SAm As SAMAlignedSequence) As List(Of Own_Al_Single_Part)
                Dim MD = (From x1 In SAm.OptionalFields Where x1.Tag = "MD").First.Value
                Dim Type As Own_Al.Type
                Type = Type.UnKnown
                Dim Numeric As String = ""
                Dim tmp As String = ""
                Dim Out As New List(Of Own_Al_Single_Part)
                Out.Add(New Own_Al_Single_Part)

                For i1 = 0 To MD.Count - 1
                    If IsNumeric(MD(i1)) = True Then
                        tmp = tmp & MD(i1)
                        Type = Own_Al.Type.Match
                    ElseIf MD(i1) = "^" Then
                        Out.Add(New Own_Al_Single_Part(tmp, SAm, Type, Out.Last))
                        tmp = ""
                        Type = Own_Al.Type.Deletion
                    Else
                        If Type = Own_Al.Type.Match Then
                            Out.Add(New Own_Al_Single_Part(tmp, SAm, Type, Out.Last))
                            tmp = ""
                        End If
                        If Type <> Own_Al.Type.Deletion Then Type = Own_Al.Type.MisMatch
                        Out.Add(New Own_Al_Single_Part(MD(i1), SAm, Type, Out.Last))
                    End If
                Next
                If tmp <> "" Then
                    Dim kj As Int16 = 54
                    If Type = Type.Match Then
                        Out.Add(New Own_Al_Single_Part(tmp, SAm, Type, Out.Last))
                    Else
                        Dim kj78 As Int16 = 65
                    End If
                End If
                Dim x = Own_Al_Helper.Common.Merge_Same_Type(Out)
                x.Add(New Own_Al_Single_Part)
                Return x
            End Function
        End Class
        Public Class Own_Al_Single_Part

            Public Property Type As Own_Al.Type
            Public Property Query_Start As Integer
            Public Property Query_End As Integer
            Public Property Ref_Start As Integer
            Public Property Ref_End As Integer
            Public Property Ref_Seq As String
            Public Property Query_Seq As String
            Public Property Length As Integer
            Public Sub New()

            End Sub
            Public Sub New(Type As Own_Al.Type, length As Integer, Last As Own_Al_Single_Part)
                If Type = Own_Al.Type.Insertion Then
                    Me.Ref_Start = Last.Ref_End
                    Me.Length = length
                    Me.Ref_End = Last.Ref_End
                    Me.Query_Start = Last.Query_End + 1
                    Me.Query_End = Me.Query_Start + length - 1
                ElseIf Type = Own_Al.Type.Deletion Then
                    Me.Ref_Start = Last.Ref_End + 1
                    Me.Length = length
                    Me.Ref_End = Me.Ref_Start + Me.Length - 1
                    Me.Query_Start = Last.Query_Start
                    Me.Query_End = Last.Query_End
                ElseIf Type = Own_Al.Type.Intron Then
                    Me.Ref_Start = Last.Ref_End
                    Me.Length = length
                    Me.Ref_End = Last.Ref_End
                    Me.Query_Start = Last.Query_End
                    Me.Query_End = Last.Query_End
                    Dim kj As Int16 = 65
                Else
                    Me.Query_Start = Last.Query_End + 1
                    Me.Query_End = Me.Query_Start + length - 1
                    Me.Ref_Start = Last.Ref_End + 1
                    Me.Length = length
                    Me.Ref_End = Me.Ref_Start + Me.Length - 1
                End If

                Me.Type = Type
            End Sub

            Public Sub New(Type As Own_Al.Type, Q_Start As Integer, Q_End As Integer, REf_Start As Integer, Ref_end As Integer, QSeq As Bio.Sequence, Ref_Seq As Bio.Sequence, Length As Integer)
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

            ''' <summary>
            ''' From MD String
            ''' </summary>
            ''' <param name="s"></param>
            ''' <param name="own_Al"></param>
            ''' <param name="sAm"></param>
            ''' <param name="type"></param>
            Public Sub New(s As String, sAm As SAMAlignedSequence, type As Own_Al.Type, Last As Own_Al_Single_Part)
                Me.Ref_Start = Last.Ref_End + 1

                Select Case type
                    Case Own_Al.Type.Match
                        Me.Length = s
                    Case Own_Al.Type.MisMatch
                        Me.Length = s.Length
                        Me.Ref_Seq = s
                    Case Own_Al.Type.Deletion
                        Me.Length = s.Length
                        Me.Ref_Seq = s
                End Select

                Me.Ref_End = Me.Ref_Start + Me.Length - 1
                Me.Type = type
            End Sub

            ''' <summary>
            ''' For First S
            ''' </summary>
            ''' <param name="QSeq"></param>
            Public Sub New(QSeq As Bio.ISequence)
                Me.Type = Own_Al.Type.Soft_Clip
                Me.Query_Start = 1
                Me.Query_End = QSeq.Count
                Me.Query_Seq = QSeq.ConvertToString
                Me.Length = QSeq.Count
            End Sub
            ''' <summary>
            ''' For Last S
            ''' </summary>
            ''' <param name="QSeq"></param>
            Public Sub New(QSeq As Bio.ISequence, Last As Own_Al_Single_Part)
                Me.Type = Own_Al.Type.Soft_Clip
                Me.Query_Start = Last.Query_End + 1
                Me.Query_End = Me.Query_Start + QSeq.Count
                Me.Query_Seq = QSeq.ConvertToString
                Me.Length = QSeq.Count
            End Sub
            Public Sub New(Type As Own_Al.Type, length As Integer)
                Me.Type = Type
                Me.Length = length
            End Sub
            Public Overrides Function ToString() As String
                Return Szunyi.Util_Helpers.Get_Enum_Name(Of Own_Al.Type)(Me.Type) & "," & Me.Length & "," & "," & Me.Ref_Start & "," & Me.Ref_End & "," & Me.Ref_Seq & "," & Me.Query_Start & "," & Me.Query_End
            End Function
            Public Function Clone()
                Dim x As New Own_Al_Single_Part
                x.Length = Me.Length
                x.Query_End = Me.Query_End
                x.Query_Start = Me.Query_Start
                x.Query_Seq = Me.Query_Seq
                x.Ref_End = Me.Ref_End
                x.Ref_Seq = Me.Ref_Seq
                x.Ref_Start = Me.Ref_Start
                x.Type = Me.Type
                Return x
            End Function
        End Class
        Public Class Positions
            Public Property Five_Prime As Integer = 0
            Public Property Three_Prime As Integer = 0
            Public Property ToProccess As Boolean = False
            Public Property ref As Bio.ISequence
            Public Property read As Bio.ISequence

            Public Property SAM As SAMAlignedSequence
            Public Sub New(F As Integer, T As Integer, ref As Bio.ISequence, read As Bio.ISequence, SAM As SAMAlignedSequence)
                Me.Three_Prime = T
                Me.Five_Prime = F
                If F <> 0 Or T <> 0 Then Me.ToProccess = True
                Me.ref = ref
                Me.read = read

                Me.SAM = SAM.Clone
            End Sub
            Public Overrides Function ToString() As String
                Dim M = From x In SAM.OptionalFields Where x.Tag = "MD"
                Return ref.ConvertToString & vbCrLf & read.ConvertToString & vbCrLf & SAM.Pos & vbCrLf & SAM.RefEndPos & vbCrLf & Five_Prime & vbCrLf & Three_Prime & vbCrLf & SAM.CIGAR & vbCrLf & M.First.Value
            End Function
        End Class
        Public Class Own_Al_Helper
            Public Class Modify
#Region "Remove exons"
                Public Shared Sub Remove_First_Exon(x1 As Own_Al, First_intron As Bio.IO.GenBank.ILocation, SAM As SAMAlignedSequence)
                    Dim The_intron = (From x In x1.Parts Where (x.Type = Own_Al.Type.Intron Or x.Type = Own_Al.Type.Deletion) And x.Ref_Start = First_intron.LocationStart)

                    If The_intron.Count = 1 Then
                        Dim Index = x1.Parts.IndexOf(The_intron.First)
                        SAM.Pos = x1.Parts(Index + 1).Ref_Start
                        Dim ToS As Integer = 0
                        For i1 = Index To 0 Step -1
                            Select Case x1.Parts(i1).Type
                                Case Own_Al.Type.Match
                                    ToS += x1.Parts(i1).Length
                                    x1.Parts.RemoveAt(i1)
                                Case Own_Al.Type.MisMatch
                                    ToS += x1.Parts(i1).Length
                                    x1.Parts.RemoveAt(i1)
                                Case Own_Al.Type.Insertion
                                    ToS += x1.Parts(i1).Length
                                    x1.Parts.RemoveAt(i1)
                                Case Own_Al.Type.Soft_Clip
                                    x1.Parts(i1).Length += ToS
                                Case Own_Al.Type.Intron
                                    x1.Parts.RemoveAt(i1)
                            End Select
                        Next
                        If x1.Parts.First.Type <> Own_Al.Type.Soft_Clip Then
                            x1.Parts.Insert(0, New Own_Al_Single_Part(Own_Al.Type.Soft_Clip, ToS))
                        End If
                        Dim jk As Int16 = 54
                    Else
                        Dim kj As Int16 = 54
                    End If
                End Sub
                Public Shared Sub Remove_Last_Exon(x1 As Own_Al, First_intron As Bio.IO.GenBank.ILocation, SAM As SAMAlignedSequence)
                    Dim The_intron = (From x In x1.Parts Where (x.Type = Own_Al.Type.Intron Or x.Type = Own_Al.Type.Deletion) And x.Ref_End = First_intron.LocationEnd)

                    If The_intron.Count = 1 Then
                        Dim Index = x1.Parts.IndexOf(The_intron.First)

                        Dim ToS As Integer = 0
                        For i1 = x1.Parts.Count - 1 To Index Step -1
                            Select Case x1.Parts(i1).Type
                                Case Own_Al.Type.Match
                                    ToS += x1.Parts(i1).Length
                                    x1.Parts.RemoveAt(i1)
                                Case Own_Al.Type.MisMatch
                                    ToS += x1.Parts(i1).Length
                                    x1.Parts.RemoveAt(i1)
                                Case Own_Al.Type.Insertion
                                    ToS += x1.Parts(i1).Length
                                    x1.Parts.RemoveAt(i1)
                                Case Own_Al.Type.Intron
                                    x1.Parts.RemoveAt(i1)

                            End Select
                        Next
                        If x1.Parts.Last.Type <> Own_Al.Type.Soft_Clip Then
                            x1.Parts.Add(New Own_Al_Single_Part(Own_Al.Type.Soft_Clip, ToS))
                        Else
                            x1.Parts.Last.Length += ToS
                        End If
                        Dim jk As Int16 = 54
                    Else
                        Dim kj As Int16 = 54
                    End If
                End Sub
#End Region
                Public Shared Sub Set_Cigar_Md(pos As Positions, x1 As Own_Al)
                    Dim Cigar = Szunyi.Alignment.Own_Al_Helper.Convert.Own_AL_To_CIGAR(x1.Parts)
                    Dim MD = Szunyi.Alignment.Own_Al_Helper.Convert.Own_AL_To_MD(x1.Parts)

                    pos.SAM.CIGAR = Cigar
                    Szunyi.BAM.BAM_Optional_Filed_Manipulation.Change_MD(pos.SAM, MD)
                End Sub
                Public Shared Sub Set_Cigar_Md(Sam As SAMAlignedSequence, x1 As Own_Al)
                    Dim Cigar = Szunyi.Alignment.Own_Al_Helper.Convert.Own_AL_To_CIGAR(x1.Parts)
                    Dim MD = Szunyi.Alignment.Own_Al_Helper.Convert.Own_AL_To_MD(x1.Parts)

                    Sam.CIGAR = Cigar
                    Szunyi.BAM.BAM_Optional_Filed_Manipulation.Change_MD(Sam, MD)
                    If Szunyi.BAM.CIgar_Md.Common.IsSame_Length(Sam) = False Then
                        Dim kj As Int16 = 54
                    End If
                End Sub
                Public Shared Sub Insert_Modify_First_S(pos As Positions, x1 As Own_Al)
                    If x1.Parts.First.Type = Alignment.Own_Al.Type.Soft_Clip Then
                        x1.Parts.First.Length -= pos.Five_Prime
                    Else
                        x1.Parts.Insert(0, New Szunyi.Alignment.Own_Al_Single_Part(Szunyi.Alignment.Own_Al.Type.Soft_Clip, -pos.Five_Prime))
                    End If
                    Dim toRemove As New List(Of Alignment.Own_Al_Single_Part)
                    For i1 = 1 To x1.Parts.Count - 1
                        If x1.Parts(i1).Type = Alignment.Own_Al.Type.Match Then Exit For
                        If x1.Parts(i1).Type = Alignment.Own_Al.Type.MisMatch Then toRemove.Add(x1.Parts(i1))
                    Next
                    For Each Item In toRemove
                        x1.Parts.Remove(Item)
                    Next
                End Sub
                Public Shared Sub Insert_Modify_Last_S(pos As Positions, x1 As Own_Al)
                    If x1.Parts.Last.Type = Alignment.Own_Al.Type.Soft_Clip Then
                        x1.Parts.Last.Length -= pos.Three_Prime
                    Else
                        x1.Parts.Add(New Szunyi.Alignment.Own_Al_Single_Part(Szunyi.Alignment.Own_Al.Type.Soft_Clip, -pos.Three_Prime))
                    End If
                    Dim toRemove As New List(Of Alignment.Own_Al_Single_Part)
                    For i1 = x1.Parts.Count - 2 To 0 Step -1
                        If x1.Parts(i1).Type = Alignment.Own_Al.Type.Match Then Exit For
                        If x1.Parts(i1).Type = Alignment.Own_Al.Type.MisMatch Then toRemove.Add(x1.Parts(i1))
                    Next
                    For Each Item In toRemove
                        x1.Parts.Remove(Item)
                    Next
                End Sub
                Public Shared Sub Modify_Ends(pos As Positions, x1 As Own_Al)
                    If pos.Five_Prime > 0 Then ' S decrease Match Increase

                        x1.Parts.First.Length -= pos.Five_Prime
                        x1.Parts(1).Length += pos.Five_Prime
                        Set_Cigar_Md(pos, x1)
                        pos.SAM.Pos -= pos.Five_Prime

                    ElseIf pos.Five_Prime < 0 Then ' S increase M decrease
                        Insert_Modify_First_S(pos, x1)
                        Set_Cigar_Md(pos, x1)
                        pos.SAM.Pos -= pos.Five_Prime
                    End If
                    If pos.Three_Prime > 0 Then ' S decrease Match Increase
                        x1.Parts.Last.Length -= pos.Three_Prime
                        x1.Parts(x1.Parts.Count - 2).Length += pos.Three_Prime
                        Set_Cigar_Md(pos, x1)
                    ElseIf pos.Three_Prime < 0 Then  ' S increase M decrease
                        Insert_Modify_Last_S(pos, x1)
                        Set_Cigar_Md(pos, x1)
                    End If
                    Dim Cigar = Own_Al_Helper.Convert.Own_AL_To_CIGAR(x1.Parts)
                    Dim l1 = Szunyi.BAM.CIgar_Md.Get_Properties.Cigar_Matched_Length_woInsertion(pos.SAM)
                    Dim l2 = Szunyi.BAM.CIgar_Md.Get_Properties.MD_Length(pos.SAM)
                    If l1 <> l2 Then

                        Dim kj As Int16 = 65
                    End If
                    If Cigar.Contains("-") Then
                        Dim kj As Int16 = 65
                    End If
                End Sub

#Region "Calculate Modify S"
                ''' <summary>
                ''' Calculate the modification lengths
                ''' </summary>
                ''' <param name="SAM"></param>
                ''' <returns></returns>
                Public Shared Function Get_Md_Positions(SAM As SAMAlignedSequence, Seq As Bio.Sequence) As Positions
                    Dim Md = Szunyi.BAM.SAM_Manipulation.Location.GetMdCIgars(SAM, Seq, True)

                    Dim read = Szunyi.BAM.SAM_Manipulation.Alignments.Get_Read_Alignmnet(Md) ' S not included
                    Dim ref = Szunyi.BAM.SAM_Manipulation.Alignments.Get_Ref_Alignmnet(Md) ' S included
                    Dim First_S = Szunyi.BAM.SAM_Manipulation.Common.Get_First_S(Md)
                    Dim Last_S = Szunyi.BAM.SAM_Manipulation.Common.Get_Last_S(Md)
                    Dim FirstPos As Integer = 0
                    Dim LastPos As Integer = 0

                    If read(First_S) <> ref(First_S) Then ' Mismatch in match, Shorten MD, Longer S
                        FirstPos = Get_First_Pos_Minus(read, ref, First_S) ' the difference is returned always -, s+abs(res), Md+res
                    Else
                        FirstPos = -Get_First_Pos_Plus(read, SAM, First_S, Seq) ' the difference is returned always - , -2 means S-2, M+2
                    End If
                    If read(read.Count - 1 - Last_S) <> ref(read.Count - 1 - Last_S) Then ' the difference is returned always +
                        LastPos = -Get_Last_Pos_Minus(read, ref, Last_S)
                    Else
                        LastPos = Get_Last_Pos_Plus(read, SAM, Last_S, Seq) ' the difference is returned always +
                    End If

                    Return New Positions(FirstPos, LastPos, ref, read, SAM)
                End Function
#Region "Get Positions"
                Private Shared Function Get_First_Pos_Plus(read As Bio.ISequence, SAM As SAMAlignedSequence, First_S As Integer, Seq As Bio.ISequence) As Integer

                    Dim First_Matched_Position_in_ref As Integer = SAM.Pos - 1
                    For First_Matched_Position_In_Read = First_S To 0 Step -1
                        If First_Matched_Position_in_ref >= 0 Then
                            If read(First_Matched_Position_In_Read) = Seq(First_Matched_Position_in_ref) Then
                                Dim j As Int16 = 54
                            Else
                                Return (First_Matched_Position_in_ref + 2 - SAM.Pos)
                            End If
                        Else
                            Return First_Matched_Position_in_ref
                        End If
                        First_Matched_Position_in_ref -= 1
                    Next
                    Return 0
                End Function
                Private Shared Function Get_Last_Pos_Plus(read As Bio.ISequence, SAM As SAMAlignedSequence, Last_S As Integer, Seq As Bio.ISequence) As Integer
                    If Last_S = 0 Then Return 0
                    Dim Last_Matched_Position_in_ref As Integer = SAM.RefEndPos
                    For Last_Matched_Position_In_Read = read.Count - Last_S To read.Count - 1
                        If Last_Matched_Position_In_Read = read.Count - 1 Or Last_Matched_Position_in_ref = Seq.Count Then
                            Return Last_Matched_Position_in_ref - SAM.RefEndPos
                        Else
                            If read(Last_Matched_Position_In_Read) = Seq(Last_Matched_Position_in_ref) Then
                                Dim j As Int16 = 54
                            Else
                                Return Last_Matched_Position_in_ref - SAM.RefEndPos
                            End If
                        End If

                        Last_Matched_Position_in_ref += 1
                        If Last_Matched_Position_in_ref = Seq.Count Then
                            Return Last_Matched_Position_in_ref - SAM.RefEndPos
                        End If
                    Next
                    Return Nothing
                End Function
                Private Shared Function Get_Last_Pos_Minus(read As Bio.ISequence, ref As Bio.ISequence, Last_S As Integer) As Integer
                    For Last_Matched_Position = read.Count - 1 - Last_S To 0 Step -1
                        If Last_Matched_Position >= 0 Then
                            If read(Last_Matched_Position) <> ref(Last_Matched_Position) Then
                                Dim j As Int16 = 54
                            Else
                                Return read.Count - 1 - Last_S - Last_Matched_Position
                            End If
                        Else
                            Return 0
                        End If
                    Next
                    Return Nothing
                End Function
                Private Shared Function Get_First_Pos_Minus(read As Bio.ISequence, ref As Bio.ISequence, First_S As Integer) As Integer
                    For First_Matched_Position = First_S To ref.Count - 1
                        If read.Count > First_Matched_Position And ref.Count > First_Matched_Position Then
                            If read(First_Matched_Position) <> ref(First_Matched_Position) Then
                                Dim j As Int16 = 54
                            Else
                                Return First_S - First_Matched_Position
                            End If
                        Else
                            Return read.Count - 1
                        End If
                    Next
                    Return Nothing
                End Function
#End Region
            End Class

            Public Class Common
                Public Shared Function Remove_Zero_Matches(Own_MDs As List(Of Own_Al_Single_Part)) As List(Of Own_Al_Single_Part)
                    Dim out As New List(Of Own_Al_Single_Part)
                    For Each Item In Own_MDs
                        If Item.Ref_End >= Item.Ref_Start Then
                            out.Add(Item)
                        Else
                            Dim kj As Int16 = 54
                        End If
                    Next
                    Return out
                End Function

                Public Shared Sub Split(Own_MDs As List(Of Own_Al_Single_Part), To_Split As Own_Al_Single_Part, To_Insert As Own_Al_Single_Part)
                    Dim Index = Own_MDs.IndexOf(To_Split)

                    Dim NofFirstPart = To_Insert.Ref_End - To_Split.Ref_Start + 1
                    Dim NofLastPart = To_Split.Length - NofFirstPart
                    If To_Split.Type = Own_Al.Type.Match Or To_Split.Type = Own_Al.Type.MisMatch Then
                        Dim First As New Own_Al_Single_Part
                        First.Ref_Start = To_Split.Ref_Start
                        First.Ref_End = To_Split.Ref_Start + NofFirstPart - 1
                        First.Length = NofFirstPart
                        If To_Split.Type = Own_Al.Type.MisMatch Then
                            First.Ref_Seq = To_Split.Ref_Seq.Substring(0, NofFirstPart)
                        End If
                        First.Type = To_Split.Type
                        Dim Last As New Own_Al_Single_Part
                        Last.Ref_Start = First.Ref_End + 1
                        Last.Ref_End = To_Split.Ref_End
                        Last.Length = NofLastPart
                        If To_Split.Type = Own_Al.Type.MisMatch Then
                            Last.Ref_Seq = To_Split.Ref_Seq.Substring(First.Length)
                        End If
                        Last.Type = To_Split.Type

                        Dim kj As Int16 = 54
                        Own_MDs.RemoveAt(Index)
                        Own_MDs.Insert(Index, Last)
                        Own_MDs.Insert(Index, To_Insert)
                        Own_MDs.Insert(Index, First)
                    ElseIf To_Split.Type = Own_Al.Type.Deletion Then
                        Own_MDs.Insert(Index + 1, To_Insert)
                    Else

                        Dim jjj As Int16 = 554
                    End If

                End Sub

                Public Shared Function Cigars_to_Own(x As List(Of KeyValuePair(Of String, Integer))) As List(Of Own_Al_Single_Part)
                    Dim out As New List(Of Own_Al_Single_Part)
                    out.Add(New Own_Al_Single_Part)
                    For Each s In x
                        Dim Type = Get_type(s.Key)
                        out.Add(New Own_Al_Single_Part(Type, s.Value, out.Last))
                    Next

                    Return out
                End Function

                Public Shared Function Get_type(Key As String) As Own_Al.Type
                    Select Case Key
                        Case "H"
                            Return Own_Al.Type.Hard_Ccip
                        Case "S"
                            Return Own_Al.Type.Soft_Clip
                        Case "I"
                            Return Own_Al.Type.Insertion
                        Case "D"
                            Return Own_Al.Type.Deletion
                        Case "M"
                            Return Own_Al.Type.Match
                        Case "N"
                            Return Own_Al.Type.Intron
                    End Select
                End Function

                Public Shared Function Clone_Own_ALs(Own_ALs As List(Of Own_Al_Single_Part)) As List(Of Own_Al_Single_Part)
                    Dim out As New List(Of Own_Al_Single_Part)
                    For Each Item In Own_ALs
                        out.Add(Item.Clone)
                    Next
                    Return out
                End Function
#Region "Merge"
                Private Shared Sub Merge(ls As List(Of Own_Al_Single_Part), i1 As Integer, Optional Type As Own_Al.Type = Own_Al.Type.UnKnown)
                    ls(i1 - 1).Ref_End = ls(i1).Ref_End
                    ls(i1 - 1).Length += ls(i1).Length


                    If Type = Own_Al.Type.UnKnown Then
                        ls(i1 - 1).Type = Own_Al.Type.Match
                    Else
                        ls(i1 - 1).Query_Seq = ls(i1 - 1).Query_Seq & ls(i1).Query_Seq
                        ls(i1 - 1).Ref_Seq = ls(i1 - 1).Ref_Seq & ls(i1).Ref_Seq
                    End If
                    ls.RemoveAt(i1)
                End Sub

                Public Shared Function Merge_Same_Type(ls As List(Of Own_Al_Single_Part)) As List(Of Own_Al_Single_Part)
                    For i1 = ls.Count - 1 To 1 Step -1
                        If ls(i1).Type = ls(i1 - 1).Type Then
                            Merge(ls, i1, ls(i1).Type)
                        End If
                    Next
                    Return ls
                End Function

                Public Shared Function Merge_Match_MisMatch(Own_ALs As List(Of Own_Al_Single_Part)) As List(Of Own_Al_Single_Part)
                    Dim Clone = Clone_Own_ALs(Own_ALs)
                    For i1 = Clone.Count - 1 To 1 Step -1
                        If (Clone(i1).Type = Own_Al.Type.Match Or Clone(i1).Type = Own_Al.Type.MisMatch) And (Clone(i1 - 1).Type = Own_Al.Type.Match Or Clone(i1 - 1).Type = Own_Al.Type.MisMatch) Then
                            Merge(Clone, i1)
                        End If
                    Next
                    Return Clone
                End Function

#End Region
            End Class
            Public Class Properties
                Public Shared Function Get_Percents_and_Sd_Deletions(ls As List(Of Szunyi.BAM.Stat.Simple_Stat))
                    Dim p = (From x In ls Select x.Deletions.Norm_Sum).ToList
                    Dim x1 As New Avd_SD(p)
                    Return x1
                End Function
                Public Shared Function Get_Percents_and_Sd_Insertion(ls As List(Of Szunyi.BAM.Stat.Simple_Stat))
                    Dim p = (From x In ls Select x.Insertions.Norm_Sum).ToList
                    Dim x1 As New Avd_SD(p)
                    Return x1
                End Function
                Public Shared Function Get_Percents_and_Sd_Match(ls As List(Of Szunyi.BAM.Stat.Simple_Stat))
                    Dim p = (From x In ls Select x.Match.Norm_Sum).ToList
                    Dim x1 As New Avd_SD(p)
                    Return x1
                End Function
                Public Shared Function Get_Percents_and_Sd_MisMatch(ls As List(Of Szunyi.BAM.Stat.Simple_Stat))
                    Dim p = (From x In ls Select x.MisMatch.Norm_Sum).ToList
                    Dim x1 As New Avd_SD(p)
                    Return x1
                End Function
                Public Shared Function Get_Percents_and_Sd_S(ls As List(Of Szunyi.BAM.Stat.Simple_Stat))
                    Dim p = (From x In ls Select x.Soft_Clip).ToList
                    Dim x1 As New Avd_SD(p)
                    Return x1
                End Function
                ''' <summary>
                ''' SUm of Exons/ Count of mappings
                ''' </summary>
                ''' <param name="ls"></param>
                ''' <returns></returns>
                Public Shared Function Get_Exons_Per_Mappings(ls As List(Of Szunyi.BAM.Stat.Simple_Stat)) As Double
                    Return (From x In ls Select x.Nof_Exon).Sum / ls.Count
                End Function
                ' Count of Match and MisMatches and Insertions
                Friend Shared Function Get_Aligned_Read_Length(x1 As Own_Al) As Integer
                    Return (From x In x1.Parts Where (x.Type = Own_Al.Type.Match Or x.Type = Own_Al.Type.MisMatch Or x.Type = Own_Al.Type.Insertion) Select x.Length).Sum
                End Function
                ' Count of Match and MisMatches and Deletions
                Friend Shared Function Get_Aligned_Ref_Length(x1 As Own_Al) As Integer
                    Return (From x In x1.Parts Where (x.Type = Own_Al.Type.Match Or x.Type = Own_Al.Type.MisMatch Or x.Type = Own_Al.Type.Deletion) Select x.Length).Sum
                End Function
                ' Count of Match and MisMatches Deletions Insertions
                Friend Shared Function Get_Alignment_Length(x1 As Own_Al) As Integer
                    Return (From x In x1.Parts Where (x.Type = Own_Al.Type.Match Or x.Type = Own_Al.Type.MisMatch Or x.Type = Own_Al.Type.Deletion Or x.Type = Own_Al.Type.Insertion) Select x.Length).Sum
                End Function
                Friend Shared Function Get_Summary(x1 As Own_Al, type As Own_Al.Type, aligned_Length As Integer) As Szunyi.BAM.Stat.Nof_Sum_Avarage
                    Dim x = From t In x1.Parts Where t.Type = type
                    Dim res As New Szunyi.BAM.Stat.Nof_Sum_Avarage(type, x, aligned_Length)
                    Return res
                End Function
                ''' <summary>
                ''' Return Sum of Soft clips
                ''' </summary>
                ''' <param name="x1"></param>
                ''' <returns></returns>
                Public Shared Function Get_S(x1 As Own_Al)
                    Dim res As Integer = 0
                    If x1.Parts.First.Type = Own_Al.Type.Soft_Clip Then res += x1.Parts.First.Length
                    If x1.Parts.Last.Type = Own_Al.Type.Soft_Clip Then res += x1.Parts.Last.Length
                    Return res

                End Function
                Public Shared Function Get_Reads_Sum_wIntrons(ls As List(Of Szunyi.BAM.Stat.Simple_Stat))
                    Dim res = From x In ls Where x.Nof_Intron > 0

                    Return res.Count
                End Function

                Public Shared Function Get_Avarage_and_SD_Exon(ls As List(Of Szunyi.BAM.Stat.Simple_Stat)) As Avd_SD
                    Dim Lengths As New List(Of Long)
                    For Each Item In ls
                        Lengths.AddRange(Item.Exon_Lengths)
                    Next
                    Return New Avd_SD(Lengths)

                End Function
                Public Shared Function Get_Avarage_and_SD_Read(ls As List(Of Szunyi.BAM.Stat.Simple_Stat)) As Avd_SD
                    Dim lengths = (From x In ls Select x.Read_Length).ToList
                    Return New Avd_SD(lengths)
                End Function
                Public Shared Function Get_Avarage_and_SD_Aligned_Read(ls As List(Of Szunyi.BAM.Stat.Simple_Stat)) As Avd_SD
                    Dim lengths = (From x In ls Select x.Alignment_Length).ToList
                    Return New Avd_SD(lengths)
                End Function
                Public Shared Function Get_Avarage_and_SD_Intron(ls As List(Of Szunyi.BAM.Stat.Simple_Stat)) As Avd_SD
                    Dim Lengths As New List(Of Long)
                    For Each Item In ls
                        Lengths.AddRange(Item.Intron_lengths)
                    Next
                    Return New Avd_SD(Lengths)

                End Function
            End Class
            Public Class Avd_SD
                Public Property SUM As Long
                Public Property Avarage As Double
                Public Property SD As Double
                Public Property SEM As Double
                Public Sub New(Lengths As List(Of Long))
                    Me.SUM = Lengths.Sum
                    Me.Avarage = Lengths.Sum / Lengths.Count
                    Me.SD = CalculateStandardDeviation(Lengths)
                    Me.SEM = (Me.SD / System.Math.Sqrt(Lengths.Count))
                End Sub
                Public Sub New(Lengths As List(Of Double))
                    Me.SUM = Lengths.Sum
                    Me.Avarage = Lengths.Sum / Lengths.Count
                    Me.SD = CalculateStandardDeviation(Lengths)
                    Me.SEM = (Me.SD / System.Math.Sqrt(Lengths.Count))
                End Sub
                Private Shared Function CalculateStandardDeviation(data As List(Of Double)) As Double
                    If data.Count = 0 Then Return 0
                    Dim mean As Double = data.Average()
                    Dim squares As New List(Of Double)
                    Dim squareAvg As Double

                    For Each value As Double In data
                        squares.Add(System.Math.Pow(value - mean, 2))
                    Next

                    squareAvg = squares.Average()

                    Return System.Math.Sqrt(squareAvg)
                End Function
                Private Shared Function CalculateStandardDeviation(data As List(Of Long)) As Double
                    If data.Count = 0 Then Return 0
                    Dim mean As Double = data.Average()
                    Dim squares As New List(Of Double)
                    Dim squareAvg As Double

                    For Each value As Double In data
                        squares.Add(System.Math.Pow(value - mean, 2))
                    Next

                    squareAvg = squares.Average()

                    Return System.Math.Sqrt(squareAvg)
                End Function
                Private Shared Function CalculateStandardDeviation(data As List(Of Integer)) As Double
                    If data.Count = 0 Then Return 0
                    Dim mean As Double = data.Average()
                    Dim squares As New List(Of Double)
                    Dim squareAvg As Double

                    For Each value As Double In data
                        squares.Add(System.Math.Pow(value - mean, 2))
                    Next

                    squareAvg = squares.Average()

                    Return System.Math.Sqrt(squareAvg)
                End Function
            End Class

            Public Class Percent_SD
                Public Property Percent As Double
                Public Property SD As Double
                Public Sub New(Stats As List(Of Szunyi.BAM.Stat.Simple_Stat), type As Szunyi.Alignment.Own_Al.Type, Sum_of_aligned_Read_Lengths As Integer)
                    Dim Lengths As Integer
                    Select Case type
                        Case Own_Al.Type.Insertion
                     '       Lengths = From x In Stats Select x.Insertions
                        Case Own_Al.Type.Deletion

                        Case Own_Al.Type.Match

                        Case Own_Al.Type.MisMatch

                    End Select
                    '       Me.Percent = Lengths.Sum / Lengths.Count
                    '         Me.SD = CalculateStandardDeviation(Lengths)
                End Sub
                Private Shared Function CalculateStandardDeviation(data As List(Of Double)) As Double
                    If data.Count = 0 Then Return 0
                    Dim mean As Double = data.Average()
                    Dim squares As New List(Of Double)
                    Dim squareAvg As Double

                    For Each value As Double In data
                        squares.Add(System.Math.Pow(value - mean, 2))
                    Next

                    squareAvg = squares.Average()

                    Return System.Math.Sqrt(squareAvg)
                End Function
                Private Shared Function CalculateStandardDeviation(data As List(Of Integer)) As Double
                    If data.Count = 0 Then Return 0
                    Dim mean As Double = data.Average()
                    Dim squares As New List(Of Double)
                    Dim squareAvg As Double

                    For Each value As Double In data
                        squares.Add(System.Math.Pow(value - mean, 2))
                    Next

                    squareAvg = squares.Average()

                    Return System.Math.Sqrt(squareAvg)
                End Function
            End Class
            ''' <summary>
            ''' Convert SAMs to Own_ALs
            ''' </summary>
            ''' <param name="Sams"></param>
            ''' <returns></returns>
            Public Shared Function Get_List(Sams As List(Of SAMAlignedSequence)) As List(Of Own_Al)
                Dim out As New List(Of Own_Al)
                For Each SAM In Sams
                    out.Add(New Own_Al(SAM))
                Next
                Return out
            End Function



#End Region

            Public Class Convert
#Region "ToString"
                Public Shared Function Own_AL_To_CIGAR(Own_ALs As List(Of Own_Al_Single_Part)) As String
                    Dim str As New System.Text.StringBuilder
                    Dim Clone = Common.Clone_Own_ALs(Own_ALs)
                    Dim Merged = Common.Merge_Match_MisMatch(Clone)
                    For Each AL In Merged
                        str.Append(AL.Length)
                        Select Case AL.Type
                            Case Own_Al.Type.Deletion
                                str.Append("D")
                            Case Own_Al.Type.Hard_Ccip
                                str.Append("H")
                            Case Own_Al.Type.Insertion
                                str.Append("I")
                            Case Own_Al.Type.Match
                                str.Append("M")
                            Case Own_Al.Type.MisMatch
                                str.Append("M")
                            Case Own_Al.Type.MisMatch
                                str.Append("")
                            Case Own_Al.Type.Soft_Clip
                                str.Append("S")
                            Case Own_Al.Type.Intron
                                str.Append("N")
                        End Select

                    Next
                    Return str.ToString
                End Function
                Public Shared Function Own_AL_To_MD_GMAP(Own_ALs As List(Of Own_Al_Single_Part)) As String
                    Dim str As New System.Text.StringBuilder
                    Dim Clone = Common.Clone_Own_ALs(Own_ALs)
                    Clone = (From x In Clone Where x.Type <> Own_Al.Type.Insertion).ToList
                    Dim Merged = Common.Merge_Same_Type(Clone)
                    For i1 = 0 To Merged.Count - 1
                        With Merged(i1)
                            Select Case .Type
                                Case Own_Al.Type.Deletion
                                    str.Append("^").Append(.Ref_Seq)
                                Case Own_Al.Type.Match
                                    str.Append(.Length)
                                Case Own_Al.Type.MisMatch

                                    str.Append(.Ref_Seq)


                            End Select
                        End With
                    Next

                    Return str.ToString
                End Function
                Public Shared Function Own_AL_To_MD(Own_ALs As List(Of Own_Al_Single_Part)) As String
                    Dim str As New System.Text.StringBuilder
                    Dim Clone = Common.Clone_Own_ALs(Own_ALs)
                    Clone = (From x In Clone Where x.Type = Own_Al.Type.Match Or x.Type = Own_Al.Type.MisMatch Or x.Type = Own_Al.Type.Deletion).ToList
                    Dim Merged = Common.Merge_Same_Type(Clone)
                    For i1 = 0 To Merged.Count - 1
                        With Merged(i1)
                            Select Case .Type
                                Case Own_Al.Type.Deletion
                                    str.Append("^").Append(.Ref_Seq)
                                Case Own_Al.Type.Match
                                    str.Append(.Length)
                                Case Own_Al.Type.MisMatch
                                    If i1 = 0 Then
                                        For i2 = 0 To .Ref_Seq.Count - 1
                                            str.Append("0")
                                            str.Append(.Ref_Seq.Substring(i2, 1))
                                        Next
                                    ElseIf Merged(i1 - 1).Type = Own_Al.Type.Deletion Then
                                        For i2 = 0 To .Ref_Seq.Count - 1
                                            str.Append("0")
                                            str.Append(.Ref_Seq.Substring(i2, 1))
                                        Next
                                    Else
                                        If .Ref_Seq.Count = 1 Then
                                            str.Append(.Ref_Seq)
                                        Else
                                            str.Append(.Ref_Seq.Substring(0, 1))
                                            For i2 = 1 To .Ref_Seq.Count - 1
                                                str.Append("0")
                                                str.Append(.Ref_Seq.Substring(i2, 1))
                                            Next
                                        End If
                                    End If

                            End Select
                        End With
                    Next

                    Return str.ToString
                End Function
                Public Shared Function Own_AL_To_cs(Own_ALs As List(Of Own_Al_Single_Part)) As String
                    Dim str As New System.Text.StringBuilder
                    For i1 = 0 To Own_ALs.Count - 1
                        If Own_ALs(i1).Type = Own_Al.Type.Match Then
                            str.Append(":").Append(Own_ALs(i1).Length)
                            Dim t As New List(Of Own_Al.Type)
                            For i2 = i1 + 1 To Own_ALs.Count - 1
                                If Own_ALs(i2).Type = Own_Al.Type.Match Then
                                    i1 = i2 - 1
                                    Exit For
                                Else
                                    If t.Contains(Own_ALs(i2).Type) = True Then
                                        Dim jj As Int16 = 54
                                    Else
                                        t.Add(Own_ALs(i2).Type)
                                        Select Case Own_ALs(i2).Type
                                            Case Own_Al.Type.Insertion
                                                str.Append("+").Append(Own_ALs(i2).Query_Seq.ToLower)
                                            Case Own_Al.Type.MisMatch
                                                For i3 = 0 To Own_ALs(i2).Ref_Seq.Count - 1
                                                    str.Append("*")
                                                    str.Append(Own_ALs(i2).Ref_Seq.Substring(i3, 1).ToLower)
                                                    str.Append(Own_ALs(i2).Query_Seq.Substring(i3, 1).ToLower)
                                                Next

                                            Case Own_Al.Type.Deletion
                                                str.Append("-").Append(Own_ALs(i2).Ref_Seq.ToLower)
                                        End Select
                                    End If
                                End If
                            Next
                        End If
                    Next
                    Return str.ToString
                End Function
#End Region
            End Class


        End Class
    End Namespace
    Namespace MappingII
        Public Class Manipulation

            Public Shared Iterator Function GroupBy_QNAMe(res As List(Of Common_Mapping_V1)) As IEnumerable(Of Common_Mapping_V1())

                Dim sg = From x In res Group By x.Sam.QName Into Group Order By Group.Count Descending

                For Each item In sg
                    Yield item.Group
                Next


            End Function
        End Class
        Interface Common_Mapping
            Sub From_Sam(SAm As Bio.IO.SAM.SAMAlignedSequence, version As Integer)
            Function To_Sam() As Bio.IO.SAM.SAMAlignedSequence
            Property Type As Constants.SequencingType
            ReadOnly Property Version As Integer
            Property Sam As SAMAlignedSequence
            Property Values As List(Of Bio.Algorithms.Alignment.PairwiseAlignedSequence)
            Property Alignments As List(Of List(Of Bio.Algorithms.Alignment.PairwiseAlignedSequence))
        End Interface
        Interface Common_Mapping_IO
            Function Parse(File As System.IO.FileInfo,
                           Type As Szunyi.Constants.SequencingType,
                           SM_aligner As Bio.Algorithms.Alignment.SmithWatermanAligner,
                                                      Adaptors As List(Of Bio.ISequence)) As IEnumerable(Of Common_Mapping_V1)

        End Interface
        Public Enum Type
            MinIon_CAP = 1
            MinIon_cDNA = 2
            PacBio = 3
        End Enum

        Public Class Adaptors
            Public Property Pacbio_3_Adaptor As New Bio.Sequence(Bio.Alphabets.AmbiguousDNA, "AGAGTACATGGG")
            Public Property PacBio_5_Adaptor As New Bio.Sequence(Bio.Alphabets.AmbiguousDNA, "CCCATGTACTCT")

            Public Property PolyA As New Bio.Sequence(Bio.Alphabets.AmbiguousDNA, Szunyi.Text.General.Multiply("A", 60))
            Public Property PolyT As New Bio.Sequence(Bio.Alphabets.AmbiguousDNA, Szunyi.Text.General.Multiply("T", 60))

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
        Public Class Common_Mapping_V1_IO


#Region "OutPut"
            Public Shared Function Get_Big_Score_Table(res As List(Of Common_Mapping_V1)) As String
                If IsNothing(res) = True OrElse res.Count = 0 Then Return String.Empty
                Dim out As New List(Of String)
                out.Add(Get_Big_Header_Score_Table(res.First))
                For Each Item In res
                    Dim tmp As New List(Of String)
                    tmp.Add(Item.Sam.QName)
                    tmp.Add(Item.Sam.QuerySequence.ConvertToString)
                    tmp.Add(Item.Sam.Pos)
                    tmp.Add(Item.Sam.RefEndPos)
                    tmp.Add(Item.Sam.Pos - Szunyi.BAM.CIGAR.Get_Five_Soft_Clip(Item.Sam))
                    tmp.Add(Item.Sam.RefEndPos + Szunyi.BAM.CIGAR.Get_Three_Soft_Clip(Item.Sam))
                    For Each A In Item.Alignments
                        tmp.Add(A.First.Score)
                        tmp.Add(A.First.FirstSequence.ConvertToString)
                        tmp.Add(A.First.SecondSequence.ConvertToString)
                        tmp.Add(A.First.Consensus.ConvertToString)
                    Next
                    out.Add(Szunyi.Text.General.GetText(tmp, vbTab))
                Next
                Return Szunyi.Text.General.GetText(out, vbCrLf)
            End Function
            Public Shared Function Get_Big_Score(Item As Common_Mapping_V1) As String
                Dim tmp As New List(Of String)
                tmp.Add(Item.Sam.QName)
                tmp.Add(Item.Sam.QuerySequence.ConvertToString)
                tmp.Add(Item.Sam.Pos)
                tmp.Add(Item.Sam.RefEndPos)
                tmp.Add(Item.Sam.Pos - Szunyi.BAM.CIGAR.Get_Five_Soft_Clip(Item.Sam))
                tmp.Add(Item.Sam.RefEndPos + Szunyi.BAM.CIGAR.Get_Three_Soft_Clip(Item.Sam))
                tmp.Add(Item.Five_Prime_Region.ConvertToString)
                tmp.Add(Item.Three_Prime_Region.ConvertToString)
                For Each A In Item.Alignments
                    tmp.Add(A.First.FirstSequence.ID)
                    tmp.Add(A.First.SecondSequence.ID)
                    tmp.Add(A.First.Score)
                    tmp.Add(A.First.FirstSequence.ConvertToString)
                    tmp.Add(A.First.SecondSequence.ConvertToString)
                    tmp.Add(A.First.Consensus.ConvertToString)
                Next
                Return Szunyi.Text.General.GetText(tmp, vbTab)


            End Function
            Public Shared Function Get_Big_Header_Score_Table(r As List(Of Szunyi.Transcipts.Adaptor_Pair)) As String
                Dim str As New System.Text.StringBuilder
                str.Append("ReadID").Append(vbTab)
                str.Append("Pos").Append(vbTab)
                str.Append("RefEndPos").Append(vbTab)
                str.Append("Seq")
                For Each AP In r
                    str.Append(vbTab)
                    str.Append(AP.Five_Prime_Adapter.ID & " Score").Append(vbTab)
                    str.Append(AP.Five_Prime_Adapter.ID & " Adaptor Seq").Append(vbTab)
                    str.Append(AP.Five_Prime_Adapter.ID & " Read Seq").Append(vbTab)
                    str.Append(AP.Five_Prime_Adapter.ID & " Consensus").Append(vbTab)
                    str.Append(AP.Five_Prime_Adapter.ID & " Pos").Append(vbTab)
                    str.Append(AP.Five_Prime_Adapter.ID & " Adapter Pos").Append(vbTab)
                    str.Append(AP.Three_Prime_Adapter.ID & " Score").Append(vbTab)
                    str.Append(AP.Three_Prime_Adapter.ID & " Adaptor Seq").Append(vbTab)
                    str.Append(AP.Three_Prime_Adapter.ID & " Read Seq").Append(vbTab)
                    str.Append(AP.Three_Prime_Adapter.ID & " Consensus").Append(vbTab)
                    str.Append(AP.Five_Prime_Adapter.ID & " Pos").Append(vbTab)
                    str.Append(AP.Three_Prime_Adapter.ID & " Adapter Pos")
                Next
                Return str.ToString
            End Function
            Public Shared Function Get_Big_Header_Score_Table(r As Common_Mapping_V1) As String
                Dim str As New System.Text.StringBuilder
                Dim s = Split("ReadID,Seq,Pos,Pos End,Pos ws,Pos End ws,Five Primer Region Seq,Three Prime Region Seq", ",")
                str.Append(Szunyi.Text.General.GetText(s, vbTab))
                For i1 = 1 To r.Alignments.Count / 2
                    str.Append(vbTab)
                    str.Append(r.Alignments(i1).First.SecondSequence.ID & " " & "Five Score")
                    str.Append(vbTab)
                    str.Append(r.Alignments(i1).First.SecondSequence.ID & " " & "Five First Seq")
                    str.Append(vbTab)
                    str.Append(r.Alignments(i1).First.SecondSequence.ID & " " & "Five Second Seq")
                    str.Append(vbTab)
                    str.Append(r.Alignments(i1).First.SecondSequence.ID & " " & "Five Consensus")
                Next
                For i1 = 1 To r.Alignments.Count / 2
                    str.Append(vbTab)
                    str.Append(r.Alignments(i1).First.SecondSequence.ID & " " & "Three Score")
                    str.Append(vbTab)
                    str.Append(r.Alignments(i1).First.SecondSequence.ID & " " & "Three First Seq")
                    str.Append(vbTab)
                    str.Append(r.Alignments(i1).First.SecondSequence.ID & " " & "Three Second Seq")
                    str.Append(vbTab)
                    str.Append(r.Alignments(i1).First.SecondSequence.ID & " " & "Three Consensus")
                Next

                Return str.ToString
            End Function
            Public Shared Function Get_Short_Score(l As Common_Mapping_V1) As String
                Dim tmp As New List(Of String)
                tmp.Add(l.Sam.QName)
                For Each A In l.Alignments
                    tmp.Add(A.First.Score)
                Next
                Return Szunyi.Text.General.GetText(tmp, vbTab)

            End Function

            Public Shared Function Get_Short_Scorre_Table(res As List(Of Common_Mapping_V1)) As String
                If IsNothing(res) = True OrElse res.Count = 0 Then Return String.Empty
                Dim out As New List(Of String)
                out.Add(Get_Short_Header_Score_Table(res.First))
                For Each Item In res
                    Dim tmp As New List(Of String)
                    tmp.Add(Item.Sam.QName)
                    For Each A In Item.Alignments
                        tmp.Add(A.First.Score)
                    Next
                    out.Add(Szunyi.Text.General.GetText(tmp, vbTab))
                Next
                Return Szunyi.Text.General.GetText(out, vbCrLf)
            End Function
            Public Shared Function Get_Short_Header_Score_Table(r As Common_Mapping_V1) As String
                Dim str As New System.Text.StringBuilder
                str.Append("ReadID")
                For i1 = 1 To r.Alignments.Count / 2
                    str.Append(vbTab)
                    str.Append(r.Alignments(i1).First.SecondSequence.ID & " " & "Five")
                Next
                For i1 = 1 To r.Alignments.Count / 2
                    str.Append(vbTab)
                    str.Append(r.Alignments(i1).First.SecondSequence.ID & " " & "Three")
                Next

                Return str.ToString
            End Function
            Public Shared Function Get_Short_Header_Score_Table(r As List(Of Szunyi.Transcipts.Adaptor_Pair)) As String
                Dim str As New System.Text.StringBuilder
                str.Append("ReadID")
                For Each AP In r
                    str.Append(vbTab)
                    str.Append(AP.Five_Prime_Adapter.ID)
                    str.Append(vbTab)
                    str.Append(AP.Three_Prime_Adapter.ID)
                Next

                Return str.ToString
            End Function
#End Region

        End Class
        Public Class Common_Mapping_V1
            Implements Common_Mapping
            Private _Alignments As New List(Of List(Of PairwiseAlignedSequence))
            Private _Sam As Bio.IO.SAM.SAMAlignedSequence
            Private _Type As SequencingType
            Private _Values As New List(Of Bio.Algorithms.Alignment.PairwiseAlignedSequence)
            Public Five_Prime_Region As Sequence
            Public Three_Prime_Region As Sequence
            Public ReadOnly Property Common_Mapping_Version As Integer Implements Common_Mapping.Version
                Get
                    Return 1
                End Get
            End Property

            Public Property Type As SequencingType Implements Common_Mapping.Type
                Get
                    Return _Type
                End Get
                Set(value As SequencingType)
                    _Type = Type
                End Set
            End Property

            Public Property Sam As SAMAlignedSequence Implements Common_Mapping.Sam
                Get
                    Return _Sam
                End Get
                Set(value As SAMAlignedSequence)
                    _Sam = value
                End Set
            End Property


            Public Property Values As List(Of PairwiseAlignedSequence) Implements Common_Mapping.Values
                Get
                    Return _Values
                End Get
                Set(value As List(Of PairwiseAlignedSequence))
                    _Values = value
                End Set
            End Property

            Public Property Alignments As List(Of List(Of PairwiseAlignedSequence)) Implements Common_Mapping.Alignments
                Get
                    Return _Alignments
                End Get
                Set(value As List(Of List(Of PairwiseAlignedSequence)))
                    _Alignments = value
                End Set
            End Property

            Public Sub From_Sam(SAm As SAMAlignedSequence, version As Integer) Implements Common_Mapping.From_Sam
                Throw New NotImplementedException()
            End Sub

            Public Function To_Sam() As SAMAlignedSequence Implements Common_Mapping.To_Sam
                Throw New NotImplementedException()
            End Function
        End Class
    End Namespace
End Namespace

