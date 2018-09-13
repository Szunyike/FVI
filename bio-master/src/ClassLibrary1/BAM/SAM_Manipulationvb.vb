Imports System.IO
Imports Bio
Imports Bio.IO.SAM
Imports ClassLibrary1.Szunyi.Alignment
Imports ClassLibrary1.Szunyi.BAM.Settings

Namespace Szunyi
    Namespace BAM
        Public Class SAM_Manipulation
            Public Class QuerySequnce

                ''' <summary>
                ''' ID = Soft clipped Seq + ":" + Match Seq
                ''' </summary>
                ''' <param name="Sam"></param>
                ''' <param name="Adaptor_Setting"></param>
                ''' <returns></returns>
                Public Shared Function Get_Five_Primer_Region(Sam As Bio.IO.SAM.SAMAlignedSequence, Adaptor_Setting As Szunyi.Transcipts.Aligner_Setting) As Bio.ISequence
                    Try
                        Dim Five_Prime_Region_In_Soft_Clipped As Bio.ISequence
                        Five_Prime_Region_In_Soft_Clipped = New Bio.Sequence(Alphabets.AmbiguousDNA, "")
                        Dim Five_Prime_Region_In_Matches As Bio.ISequence
                        Five_Prime_Region_In_Matches = New Bio.Sequence(Alphabets.AmbiguousDNA, "")

                        Dim QuerySequence = Szunyi.Sequences.SequenceManipulation.Common.GetSeqAsBioSeq(Sam.QuerySequence)

                        Dim Cigars = CIGAR.Get_CIGARS(Sam)
                        Dim Five_Prime_Region As Bio.ISequence
                        If Cigars.First.Key = "S" Then
                            If Cigars.First.Value >= Adaptor_Setting.Ouf_of_Match_Length Then
                                Five_Prime_Region_In_Soft_Clipped = QuerySequence.GetSubSequence(Cigars.First.Value - Adaptor_Setting.Ouf_of_Match_Length, Adaptor_Setting.Ouf_of_Match_Length)
                                If Cigars.First.Value + Adaptor_Setting.In_Match_Length > QuerySequence.Count Then
                                    Five_Prime_Region = New Bio.Sequence(Alphabets.AmbiguousDNA, Five_Prime_Region_In_Soft_Clipped.ConvertToString)
                                    Five_Prime_Region.ID = -Adaptor_Setting.Ouf_of_Match_Length
                                Else
                                    Five_Prime_Region_In_Matches = QuerySequence.GetSubSequence(Cigars.First.Value, Adaptor_Setting.In_Match_Length)
                                    Five_Prime_Region = New Bio.Sequence(Alphabets.AmbiguousDNA, Five_Prime_Region_In_Soft_Clipped.ConvertToString & Five_Prime_Region_In_Matches.ConvertToString)
                                    Five_Prime_Region.ID = -Adaptor_Setting.Ouf_of_Match_Length
                                End If

                            Else
                                Five_Prime_Region_In_Soft_Clipped = QuerySequence.GetSubSequence(0, Cigars.First.Value)
                                Five_Prime_Region_In_Matches = QuerySequence.GetSubSequence(Cigars.First.Value, Adaptor_Setting.In_Match_Length)
                                Five_Prime_Region = New Bio.Sequence(Alphabets.AmbiguousDNA, Five_Prime_Region_In_Soft_Clipped.ConvertToString & Five_Prime_Region_In_Matches.ConvertToString)
                                Five_Prime_Region.ID = -Adaptor_Setting.Ouf_of_Match_Length

                            End If
                        Else
                            Five_Prime_Region = QuerySequence.GetSubSequence(0, Adaptor_Setting.In_Match_Length)
                            Five_Prime_Region = New Bio.Sequence(Alphabets.AmbiguousDNA, Five_Prime_Region.ConvertToString)

                        End If
                        Five_Prime_Region.ID = Five_Prime_Region_In_Soft_Clipped.ConvertToString & ":" & Five_Prime_Region_In_Matches.ConvertToString
                        Return Five_Prime_Region
                    Catch ex As Exception
                        Return New Bio.Sequence(Alphabets.AmbiguousDNA, "")
                    End Try

                End Function
                ''' <summary>
                ''' ID = Soft clipped Seq + ":" + Match Seq
                ''' </summary>
                ''' <param name="Sam"></param>
                ''' <param name="Adaptor_Setting"></param>
                ''' <returns></returns>
                Public Shared Function Get_Three_Primer_Region(Sam As Bio.IO.SAM.SAMAlignedSequence, Adaptor_Setting As Szunyi.Transcipts.Aligner_Setting) As Bio.ISequence
                    Dim Cigars = CIGAR.Get_CIGARS(Sam)
                    If Cigars.Last.Key = "H" Then Cigars.RemoveAt(Cigars.Count - 1)
                    Dim Three_Prime_Region_In_Matches As Bio.ISequence
                    Three_Prime_Region_In_Matches = New Bio.Sequence(Alphabets.AmbiguousDNA, "")
                    Dim Three_Prime_Region_In_Soft_Clipped As Bio.ISequence
                    Three_Prime_Region_In_Soft_Clipped = New Bio.Sequence(Alphabets.AmbiguousDNA, "")
                    Dim QuerySequence = Szunyi.Sequences.SequenceManipulation.Common.GetSeqAsBioSeq(Sam.QuerySequence)
                    Dim Three_Prime_Region As Bio.Sequence
                    Try
                        If Cigars.Last.Key = "S" Then
                            If Cigars.Last.Value >= Adaptor_Setting.Ouf_of_Match_Length Then
                                Three_Prime_Region_In_Matches = QuerySequence.GetSubSequence(Sam.QuerySequence.Count - Cigars.Last.Value - Adaptor_Setting.In_Match_Length, Adaptor_Setting.In_Match_Length)
                                Three_Prime_Region_In_Soft_Clipped = QuerySequence.GetSubSequence(Sam.QuerySequence.Count - Cigars.Last.Value, Adaptor_Setting.Ouf_of_Match_Length)

                                Three_Prime_Region = New Bio.Sequence(Alphabets.AmbiguousDNA, Three_Prime_Region_In_Matches.ConvertToString & Three_Prime_Region_In_Soft_Clipped.ConvertToString)
                                Three_Prime_Region.ID = -Adaptor_Setting.Ouf_of_Match_Length
                            Else
                                Three_Prime_Region_In_Matches = QuerySequence.GetSubSequence(Sam.QuerySequence.Count - Cigars.Last.Value - Adaptor_Setting.In_Match_Length, Adaptor_Setting.In_Match_Length)
                                Three_Prime_Region_In_Soft_Clipped = QuerySequence.GetSubSequence(Sam.QuerySequence.Count - Cigars.Last.Value, Cigars.Last.Value)
                                Three_Prime_Region = New Bio.Sequence(Alphabets.AmbiguousDNA, Three_Prime_Region_In_Matches.ConvertToString & Three_Prime_Region_In_Soft_Clipped.ConvertToString)
                                Three_Prime_Region.ID = -Cigars.Last.Value
                            End If
                        Else
                            Three_Prime_Region_In_Matches = QuerySequence.GetSubSequence(Sam.QuerySequence.Count - Adaptor_Setting.In_Match_Length, Adaptor_Setting.In_Match_Length)
                            Three_Prime_Region = Three_Prime_Region_In_Matches
                            Three_Prime_Region = New Bio.Sequence(Alphabets.AmbiguousDNA, Three_Prime_Region.ConvertToString)

                            Three_Prime_Region.ID = 1
                        End If
                    Catch ex As Exception
                        Dim kj As Int16 = 43
                    End Try
                    Three_Prime_Region.ID = Three_Prime_Region_In_Soft_Clipped.ConvertToString & ":" & Three_Prime_Region_In_Matches.ConvertToString
                    Return Three_Prime_Region
                End Function
                Public Shared Function Get_Five_S(Sam As SAMAlignedSequence) As Bio.ISequence
                    Dim Cigars = CIGAR.Get_CIGARS(Sam)
                    If Cigars.First.Key = "S" Then
                        Dim str As New System.Text.StringBuilder
                        For i1 = 0 To Cigars.First.Value - 1
                            str.Append(ChrW(Sam.QuerySequence(i1)))
                        Next

                        Dim Seq As New Bio.Sequence(Alphabets.AmbiguousDNA, str.ToString)
                        Return Seq
                    End If
                    Return New Bio.Sequence(Alphabets.AmbiguousDNA, "")
                End Function
                Public Shared Function Get_Three_S(Sam As SAMAlignedSequence) As Bio.ISequence
                    Dim Cigars = CIGAR.Get_CIGARS(Sam)
                    If Cigars.Last.Key = "S" Then
                        Dim str As New System.Text.StringBuilder
                        For i1 = Sam.QuerySequence.Count - Cigars.Last.Value - 1 To Sam.QuerySequence.Count - 1
                            str.Append(ChrW(Sam.QuerySequence(i1)))
                        Next

                        Dim Seq As New Bio.Sequence(Alphabets.AmbiguousDNA, str.ToString)
                        Return Seq
                    End If
                    Return New Bio.Sequence(Alphabets.AmbiguousDNA, "")
                End Function
                Public Shared Function Get_Full(SAM As SAMAlignedSequence) As Bio.ISequence
                    Dim str As New System.Text.StringBuilder
                    For i1 = 0 To SAM.QuerySequence.Count - 1
                        str.Append(ChrW(SAM.QuerySequence(i1)))
                    Next
                    Dim Seq As New Bio.Sequence(Alphabets.AmbiguousDNA, str.ToString)
                    Return Seq
                End Function
                Public Shared Function Get_wo_S(SAM As SAMAlignedSequence) As Bio.ISequence
                    Dim S = Szunyi.BAM.CIGAR.Get_First_S_Length(SAM)
                    Dim SL = Szunyi.BAM.CIGAR.Get_Last_S_Length(SAM)

                    Dim str As New System.Text.StringBuilder
                    For i1 = S To SAM.QuerySequence.Count - 1
                        str.Append(ChrW(SAM.QuerySequence(i1)))
                    Next
                    Dim Seq As New Bio.Sequence(Alphabets.AmbiguousDNA, str.ToString)
                    Return Seq
                End Function
                Public Shared Function Get_Max_S(Sam As SAMAlignedSequence) As Integer
                    Dim Cigars = CIGAR.Get_CIGARS(Sam)
                    Dim max As Integer = 0
                    If Cigars.Last.Key = "S" Then
                        max = Cigars.Last.Value
                    End If
                    If Cigars.First.Key = "S" Then
                        Dim x = Cigars.First.Value
                        If x > max Then max = x
                    End If
                    Return max


                End Function
            End Class
            Public Class Location
                Public Shared Function HasHardClip(SAMs As List(Of Bio.IO.SAM.SAMAlignedSequence)) As Boolean
                    For Each Sam In SAMs
                        If HasHardClip(Sam) = True Then Return True
                    Next
                    Return False
                End Function
                Public Shared Function HasHardClip(SAM As Bio.IO.SAM.SAMAlignedSequence) As Boolean
                    Dim C = Get_CIGARS(SAM.CIGAR)
                    If C.First.Key = "H" Or C.Last.Key = "H" Then
                        Return True
                    End If

                    Return False
                End Function
                Public Shared Function Get_First_Match(Exon As List(Of MdCigar)) As MdCigar
                    Dim AllM = From x In Exon Where x.Type = "M"
                    If AllM.Count > 0 Then
                        Return AllM.First
                    Else
                        Return Nothing
                    End If

                End Function
                Public Shared Function Get_Last_Match(Exon As List(Of MdCigar)) As MdCigar
                    Dim AllM = From x In Exon Where x.Type = "M"

                    If AllM.Count > 0 Then
                        Return AllM.Last
                    Else
                        Return Nothing
                    End If
                End Function


                Public Class MdCIgars
                    Public Property MdCIgars As List(Of MdCigar)
                    Public Property Is_Fw As Boolean
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
                End Class
                Public Shared Function GetMdCIgars(SAM As Bio.IO.SAM.SAMAlignedSequence,
                                                   RefSeq As Bio.Sequence,
                                                   isFull As Boolean) As MdCIgars
                    SAM.QuerySequence = Szunyi.Sequences.SequenceManipulation.Common.GetSeqAsBioSeq(SAM.QuerySequence)
                    Dim out As New List(Of MdCigar)
                    Dim Q_Start As Integer = 1
                    Dim Ref_Start As Integer = SAM.Pos
                    Dim C = Get_CIGARS(SAM.CIGAR)
                    If isFull = True Then
                        For Each Cigar In C
                            Try
                                Dim mdCIgar As New MdCigar()

                                Select Case Cigar.Key
                                    Case "N"
                                        mdCIgar = New MdCigar(Cigar.Key, Q_Start, Q_Start, Ref_Start, Ref_Start + Cigar.Value - 1, SAM.QuerySequence, RefSeq, Cigar.Value, SAM)
                                        Ref_Start += Cigar.Value
                                    Case "I"
                                        mdCIgar = New MdCigar(Cigar.Key, Q_Start - 1, Q_Start + Cigar.Value, Ref_Start, Ref_Start, SAM.QuerySequence, RefSeq, Cigar.Value, SAM)
                                        Q_Start += Cigar.Value
                                    Case "M"
                                        mdCIgar = New MdCigar(Cigar.Key, Q_Start - 1, Q_Start + Cigar.Value, Ref_Start - 1, Ref_Start + Cigar.Value - 1, SAM.QuerySequence, RefSeq, Cigar.Value, SAM)
                                        Q_Start += Cigar.Value
                                        Ref_Start += Cigar.Value
                                    Case "H"
                                        Dim alf As Integer = 54
                                    Case "D"
                                        mdCIgar = New MdCigar(Cigar.Key, Q_Start, Q_Start, Ref_Start - 1, Ref_Start + Cigar.Value - 1, SAM.QuerySequence, RefSeq, Cigar.Value, SAM)
                                        Ref_Start += Cigar.Value
                                    Case "S"
                                        mdCIgar = New MdCigar(Cigar.Key, Q_Start - 1, Q_Start + Cigar.Value, Ref_Start, Ref_Start, SAM.QuerySequence, RefSeq, Cigar.Value, SAM)
                                        Q_Start += Cigar.Value

                                    Case Else
                                        Dim alf As Int16 = 54
                                End Select
                                out.Add(mdCIgar)
                            Catch ex As Exception
                                Dim kj As Int16 = 65
                            End Try

                        Next
                    Else
                        For Each Cigar In C
                            Try
                                Dim mdCIgar As New MdCigar()

                                Select Case Cigar.Key
                                    Case "N"
                                        mdCIgar = New MdCigar(Cigar.Key, Q_Start, Q_Start, Ref_Start, Ref_Start + Cigar.Value - 1, Cigar.Value, SAM)
                                        Ref_Start += Cigar.Value
                                    Case "I"
                                        mdCIgar = New MdCigar(Cigar.Key, Q_Start - 1, Q_Start + Cigar.Value, Ref_Start, Ref_Start, Cigar.Value, SAM)
                                        Q_Start += Cigar.Value
                                    Case "M"
                                        mdCIgar = New MdCigar(Cigar.Key, Q_Start - 1, Q_Start + Cigar.Value, Ref_Start - 1, Ref_Start + Cigar.Value - 1, Cigar.Value, SAM)
                                        Q_Start += Cigar.Value
                                        Ref_Start += Cigar.Value
                                    Case "H"
                                        Dim alf As Integer = 54
                                    Case "D"
                                        mdCIgar = New MdCigar(Cigar.Key, Q_Start, Q_Start, Ref_Start - 1, Ref_Start + Cigar.Value - 1, Cigar.Value, SAM)
                                        Ref_Start += Cigar.Value
                                    Case "S"
                                        mdCIgar = New MdCigar(Cigar.Key, Q_Start - 1, Q_Start + Cigar.Value, Ref_Start, Ref_Start, Cigar.Value, SAM)
                                        Q_Start += Cigar.Value

                                    Case Else
                                        Dim alf As Int16 = 54
                                End Select
                                out.Add(mdCIgar)
                            Catch ex As Exception
                                Dim kj As Int16 = 65
                            End Try
                        Next
                    End If

                    Dim m As New MdCIgars
                    m.MdCIgars = out
                    If SAM.Flag = SAMFlags.QueryOnReverseStrand Then
                        m.Is_Fw = False
                    Else
                        m.Is_Fw = True
                    End If
                    Return m
                End Function
                Public Shared Function Get_CIGARS(CIGAR As String) As List(Of KeyValuePair(Of String, Integer))
                    Dim CIGARS As New List(Of KeyValuePair(Of String, Integer))
                    Dim cI As String = ""
                    For i1 = 0 To CIGAR.Count - 1
                        Dim s As String = CIGAR(i1)
                        Dim i As Integer = 0
                        If Integer.TryParse(s, 1) Then
                            cI = cI & s
                        Else
                            If cI = String.Empty Then
                                cI = "0"
                            End If
                            Dim t As New KeyValuePair(Of String, Integer)(s, cI)
                            CIGARS.Add(t)
                            cI = ""
                        End If
                    Next
                    Return CIGARS
                End Function


                Public Class MdCigar
                    Public Property Type As String
                    Public Property Query_Start As Integer
                    Public Property Query_End As Integer
                    Public Property Ref_Start As Integer
                    Public Property Ref_End As Integer
                    Public Property Ref_Seq As Bio.ISequence
                    Public Property Query_Seq As Bio.ISequence
                    Public Property Length As Integer
                    Public Property Is_Fw As Boolean
                    Public Property SAM As Bio.IO.SAM.SAMAlignedSequence
                    Public Sub New()

                    End Sub
                    Public Sub New(Type As String,
                                   Q_Start As Integer,
                                   Q_End As Integer,
                                   REf_Start As Integer,
                                   Ref_end As Integer,
                                   Length As Integer,
                                   SAM As Bio.IO.SAM.SAMAlignedSequence)
                        Me.Type = Type
                        Me.Query_End = Q_End
                        Me.Query_Start = Q_Start
                        Me.Ref_Start = REf_Start
                        Me.Ref_End = Ref_end

                        If SAM.Flag = SAMFlags.QueryOnReverseStrand Then
                            Me.Is_Fw = False
                        Else
                            Me.Is_Fw = True
                        End If
                        Me.SAM = SAM
                        Me.Length = Length
                    End Sub
                    Public Sub New(Type As String,
                                   Q_Start As Integer,
                                   Q_End As Integer,
                                   REf_Start As Integer,
                                   Ref_end As Integer,
                                   QSeq As Bio.ISequence,
                                   Ref_Seq As Bio.ISequence,
                                   Length As Integer,
                                   SAM As Bio.IO.SAM.SAMAlignedSequence)
                        Me.Type = Type
                        Me.Query_End = Q_End
                        Me.Query_Start = Q_Start
                        Me.Ref_Start = REf_Start
                        Me.Ref_End = Ref_end
                        If Query_End <> Q_Start Then Me.Query_Seq = QSeq.GetSubSequence(Me.Query_Start, Length)
                        If IsNothing(Ref_Seq) = False Then
                            If REf_Start <> Ref_end Then Me.Ref_Seq = Ref_Seq.GetSubSequence(Me.Ref_Start, Length)
                        End If
                        If SAM.Flag = SAMFlags.QueryOnReverseStrand Then
                            Me.Is_Fw = False
                        Else
                            Me.Is_Fw = True
                        End If
                        Me.SAM = SAM
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


            End Class

            Public Class Common
#Region "CIGAR"
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
                Public Shared Function Cigars_to_String(mdCigars As List(Of KeyValuePair(Of String, Integer))) As String
                    Dim str As New System.Text.StringBuilder
                    For Each item In mdCigars
                        str.Append(item.Value).Append(item.Key)
                    Next
                    Return str.ToString
                End Function
                Public Shared Function Cigars_to_String(mdCigar As SAM_Manipulation.Location.MdCIgars) As String
                    Dim str As New System.Text.StringBuilder
                    For Each item In mdCigar.MdCIgars
                        str.Append(item.Length).Append(item.Type)
                    Next
                    Return str.ToString
                End Function
                Public Shared Function Get_First_S(sam As SAMAlignedSequence, refseq As Bio.ISequence) As Integer
                    Dim md = Location.GetMdCIgars(sam, refseq, True)
                    If md.MdCIgars.First.Type = "S" Then
                        Return md.MdCIgars.First.Length
                    Else
                        Return 0
                    End If
                End Function

                Public Shared Function Get_Last_S(sam As SAMAlignedSequence, refseq As Bio.ISequence) As Integer
                    Dim md = Location.GetMdCIgars(sam, refseq, True)
                    If md.MdCIgars.Last.Type = "S" Then
                        Return md.MdCIgars.Last.Length
                    Else
                        Return 0
                    End If
                End Function
                Public Shared Function Get_First_S(md As Location.MdCIgars) As Integer
                    If md.MdCIgars.First.Type = "S" Then
                        Return md.MdCIgars.First.Length
                    Else
                        Return 0
                    End If
                End Function

                Public Shared Function Get_Last_S(md As Location.MdCIgars) As Integer

                    If md.MdCIgars.Last.Type = "S" Then
                        Return md.MdCIgars.Last.Length
                    Else
                        Return 0
                    End If
                End Function

                Public Shared Sub Modifiy_First_S(Sam As SAMAlignedSequence, nofBase As Integer, Md As Location.MdCIgars)
                    If Md.MdCIgars.First.Type = "S" Then
                        Md.MdCIgars.First.Length += nofBase
                        Md.MdCIgars(1).Length -= nofBase
                        '      Dim c = Cigars_to_String(Md.)
                    Else
                        '     Md.MdCIgars.Insert(0, New Location.MdCigar("S", 0, nofBase, 0, 0, Sam))
                    End If
                End Sub

#End Region

            End Class
            Public Class Alignments
                Public Shared Function Get_Ref_Alignmnet(Sam As SAMAlignedSequence, Ref_Seq As Bio.Sequence) As Bio.Sequence
                    Dim MdCigars = SAM_Manipulation.Location.GetMdCIgars(Sam, Ref_Seq, True)
                    Dim str As New System.Text.StringBuilder
                    For Each CIgar In MdCigars.MdCIgars
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
                Public Shared Function Get_Ref_Alignmnet(MdCigars As Location.MdCIgars) As Bio.Sequence

                    Dim str As New System.Text.StringBuilder
                    For Each CIgar In MdCigars.MdCIgars
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
                Public Shared Function Get_Read_Alignmnet(Sam As SAMAlignedSequence, Ref_Seq As Bio.Sequence) As Bio.Sequence
                    Dim MdCigars = SAM_Manipulation.Location.GetMdCIgars(Sam, Ref_Seq, True)
                    'fw
                    Dim str As New System.Text.StringBuilder
                    For Each CIgar In MdCigars.MdCIgars
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
                Public Shared Function Get_Read_Alignmnet(MdCigars As Location.MdCIgars) As Bio.Sequence
                    'fw
                    Dim str As New System.Text.StringBuilder
                    For Each CIgar In MdCigars.MdCIgars
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

            End Class
            Public Class Modify_Ends_Remove_Exons
                Public Shared Sub Modify_Last_Exons(files As List(Of FileInfo), seqs As List(Of ISequence), x As Last_Exons)
                    Dim d As New Dictionary(Of String, Bio.ISequence)
                    For Each seq In seqs
                        d.Add(seq.ID, seq)
                    Next

                    For Each File In files
                        Dim Header = Szunyi.BAM.Bam_Basic_IO.Headers.Get_Header(File)
                        Dim nFIle2 = Szunyi.IO.Files.Get_New_FileName.Append_Before_Extension(File, "_md_-Small_Exon")

                        Dim nFIle = Szunyi.IO.Files.Get_New_FileName.Append_Before_Extension(File, "_md_SmallExon_For_Test")
                        Using w As New Szunyi.BAM.Bam_Basic_IO.Export(nFIle2, Header)
                            Dim s = Split("M,L,R,O", ",")
                            Szunyi.BAM.Bam_Basic_IO.Headers.Add_RGs(Header, s)
                            Using w_test As New Szunyi.BAM.Bam_Basic_IO.Export(nFIle, Header)
                                For Each SAM In Szunyi.BAM.Bam_Basic_IO.Import.Parse(File)
                                    If d.ContainsKey(SAM.RName) Then
                                        Dim OriginalSAM = SAM.Clone
                                        Dim x1 As New Szunyi.Alignment.Own_Al(SAM)
                                        Dim oLoci = Szunyi.Location.Common.GetLocation(x1)
                                        Dim oExons = Szunyi.Location.Common.Get_All_Exon_Location(oLoci)
                                        Do

                                            Dim cSAM = SAM.Clone
                                            Dim Processed As Boolean = False
                                            Dim Loci = Szunyi.Location.Common.GetLocation(x1)
                                            Dim Exons = Szunyi.Location.Common.Get_All_Exon_Location(Loci)
                                            If Szunyi.Location.Common.Get_Length(Exons.First) <= x.Exon_Length.Default_Value Then
                                                Dim Exon_Seq = Szunyi.Sequences.SequenceManipulation.GetSequences.From_Loci(d(SAM.RName), Exons.First)
                                                If Szunyi.DNA.Common.Get_AT_Percent(Exon_Seq.First, Loci.IsComplementer) >= x.AT_Percent.Default_Value Then
                                                    w_test.Write(SAM)
                                                    '       Dim nof_NA_ToRemove = Remove_First_Exon_From_Cigar(SAM, Exons.First, d(SAM.RName))

                                                    SAM.QName = SAM.QName & "L"
                                                    w_test.Write(SAM)
                                                    Processed = True
                                                End If
                                            ElseIf Szunyi.Location.Common.Get_Length(Exons.Last) <= x.Exon_Length.Default_Value Then
                                                Dim Exon_Seq = Szunyi.Sequences.SequenceManipulation.GetSequences.From_Loci(d(SAM.RName), Exons.Last)
                                                If Szunyi.DNA.Common.Get_AT_Percent(Exon_Seq.First) >= x.AT_Percent.Default_Value Then
                                                    w_test.Write(SAM)
                                                    '        Dim nof_NA_ToRemove = Remove_Last_Exon_From_Cigar(SAM, Exons.Last, d(SAM.RName))

                                                    SAM.QName = SAM.QName & "R"
                                                    w_test.Write(SAM)
                                                    Processed = True
                                                End If
                                            End If
                                            If Processed = False Then Exit Do
                                        Loop

                                    End If
                                Next
                            End Using
                        End Using
                    Next ' FIle
                End Sub
            End Class
            Public Class Stat
                Public Property ReadID As String
                Public Property Five As Integer
                Public Property Three As Integer
                Public Property Removed_5_Prime_Exon As Integer = 0
                Public Property Removed_3_Prime_Exon As Integer = 0
                Public Sub New(pos As Szunyi.Alignment.Positions)
                    Me.ReadID = pos.SAM.QName
                    Me.Five = pos.Five_Prime
                    Me.Three = pos.Three_Prime
                End Sub
                Public Sub New(SAM As Bio.IO.SAM.SAMAlignedSequence)
                    Me.ReadID = SAM.QName
                End Sub
                Public Shared Function Get_Detailed_Result(Stats As List(Of Stat), x As Settings.Last_Exons) As String
                    If IsNothing(x) = True Then x = New Settings.Last_Exons
                    Dim str As New System.Text.StringBuilder
                    str.Append("#Minimum A/T percent").Append(vbTab)
                    str.Append(x.AT_Percent).AppendLine()
                    str.Append("#Minimum Exon Length").Append(vbTab)
                    str.Append(x.Exon_Length).AppendLine()
                    str.Append("#Minimum Intron Length").Append(vbTab)
                    str.Append(x.Intron_Length).AppendLine()
                    str.Append("Read ID").Append(vbTab)
                    str.Append("Nof NA Modified at 5 prime").Append(vbTab)
                    str.Append("Nof NA Modified at 3 prime").Append(vbTab)
                    str.Append("Nof exons Removed at 5 prime").Append(vbTab)
                    str.Append("Nof exons Removed at 3 prime").AppendLine()
                    For Each Item In Stats
                        str.Append(Item.ReadID).Append(vbTab)
                        str.Append(Item.Five).Append(vbTab)
                        str.Append(Item.Three).Append(vbTab)
                        str.Append(Item.Removed_5_Prime_Exon).Append(vbTab)
                        str.Append(Item.Removed_3_Prime_Exon).AppendLine()
                    Next
                    Return str.ToString
                End Function
                Public Shared Function Get_Aggregate_Result(Stats As List(Of Stat), x1 As Settings.Last_Exons) As String
                    If IsNothing(x1) = True Then x1 = New Settings.Last_Exons
                    Dim F As New Dictionary(Of Integer, Integer)
                    Dim T As New Dictionary(Of Integer, Integer)
                    Dim str As New System.Text.StringBuilder
                    str.Append("#Minimum A/T percent").Append(vbTab)
                    str.Append(x1.AT_Percent.Default_Value).AppendLine()
                    str.Append("#Minimum Exon Length").Append(vbTab)
                    str.Append(x1.Exon_Length.Default_Value).AppendLine()
                    str.Append("#Minimum Intron Length").Append(vbTab)
                    str.Append(x1.Intron_Length.Default_Value).AppendLine()
                    For Each s In Stats
                        If F.ContainsKey(s.Five) = False Then F.Add(s.Five, 0)
                        F(s.Five) += 1
                        If T.ContainsKey(s.Three) = False Then T.Add(s.Three, 0)
                        T(s.Three) += 1
                    Next
                    Dim ByF = From x In T Order By x.Key Ascending
                    str.Append("Five Prime").AppendLine()
                    Dim Keys = (From x In ByF Select x.Key).ToList
                    Dim Values = (From x In ByF Select x.Value).ToList
                    str.Append(Szunyi.Text.General.GetText(Keys, vbTab)).AppendLine()
                    str.Append(Szunyi.Text.General.GetText(Values, vbTab)).AppendLine()
                    Dim Nof_S_F = (From x In ByF Where x.Key < 0 Select x.Value).Sum
                    str.Append("Nof Read Shortened at 5' prime").Append(vbTab).Append(Nof_S_F).AppendLine()
                    Dim Nof_0_F = (From x In ByF Where x.Key = 0 Select x.Value).Sum
                    str.Append("Nof Read not changed at 5' prime").Append(vbTab).Append(Nof_0_F).AppendLine()
                    Dim Nof_L_F = (From x In ByF Where x.Key > 0 Select x.Value).Sum
                    str.Append("Nof Read Longered at 5' prime").Append(vbTab).Append(Nof_L_F).AppendLine()

                    str.Append("Three Prime").AppendLine()
                    ByF = From x In F Order By x.Key Ascending
                    str.Append("Five Prime").AppendLine()
                    Keys = (From x In ByF Select x.Key).ToList
                    Values = (From x In ByF Select x.Value).ToList
                    str.Append(Szunyi.Text.General.GetText(Keys, vbTab)).AppendLine()
                    str.Append(Szunyi.Text.General.GetText(Values, vbTab)).AppendLine()
                    Nof_S_F = (From x In ByF Where x.Key < 0 Select x.Value).Sum
                    str.Append("Nof Read Shortened at 3' prime").Append(vbTab).Append(Nof_S_F).AppendLine()
                    Nof_0_F = (From x In ByF Where x.Key = 0 Select x.Value).Sum
                    str.Append("Nof Read not changed at 3' prime").Append(vbTab).Append(Nof_0_F).AppendLine()
                    Nof_L_F = (From x In ByF Where x.Key > 0 Select x.Value).Sum
                    str.Append("Nof Read Longered at 3' prime").Append(vbTab).Append(Nof_L_F).AppendLine()

                    Dim F_I As New Dictionary(Of Integer, Integer)
                    Dim T_I As New Dictionary(Of Integer, Integer)
                    For Each s In Stats
                        If F_I.ContainsKey(s.Removed_5_Prime_Exon) = False Then F_I.Add(s.Removed_5_Prime_Exon, 0)
                        F_I(s.Removed_5_Prime_Exon) += 1
                        If T_I.ContainsKey(s.Removed_3_Prime_Exon) = False Then T_I.Add(s.Removed_3_Prime_Exon, 0)
                        T_I(s.Removed_3_Prime_Exon) += 1
                    Next
                    str.Append("Five Prime Removed Exons").AppendLine()
                    Dim By_F_I = From x In F_I Order By x.Key Ascending
                    For Each Item In By_F_I
                        str.Append(Item.Key).Append(vbTab).Append(Item.Value).AppendLine()
                    Next
                    str.AppendLine()
                    str.Append("Three Prime Removed Exons").AppendLine()
                    Dim By_T_I = From x In T_I Order By x.Key Ascending
                    For Each Item In By_T_I
                        str.Append(Item.Key).Append(vbTab).Append(Item.Value).AppendLine()
                    Next
                    Return str.ToString
                End Function
            End Class

        End Class

    End Namespace
End Namespace

