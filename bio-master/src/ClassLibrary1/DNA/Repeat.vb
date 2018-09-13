Imports System.IO

Namespace Szunyi
    Namespace DNA
        Namespace Repeat
            Public Class RepeatFinder
                Public Property OriRepeats As Repeats
                Public Bam_Files As List(Of FileInfo)
                Public Common_Names As New List(Of FileInfo)
                Public res As List(Of String)
                Public IDAndRename As Dictionary(Of String, String)
                Public Sub New(BamFiles As List(Of FileInfo), File As FileInfo, IDAndRename As Dictionary(Of String, String))
                    OriRepeats = New Repeats(File)
                    Me.Bam_Files = BamFiles
                    Me.IDAndRename = IDAndRename

                End Sub
                Public Sub New(BamFiles As List(Of FileInfo), File As FileInfo, IDAndRename As Dictionary(Of String, String), width As Integer, Seq As Bio.Sequence)
                    OriRepeats = New Repeats(File, width, Seq)
                    Me.Bam_Files = BamFiles
                    Me.IDAndRename = IDAndRename

                End Sub

                Public Sub DoIt()
                    If IsNothing(Me.IDAndRename) = False Then

                        Dim grs = From x In Me.IDAndRename Group By x.Value Into Group

                        For Each gr In grs
                            Dim gr_BamFIles As New List(Of FileInfo)
                            For Each Item In gr.Group
                                Dim cBamFIles = From f In Bam_Files Where f.Name.Contains(Item.Key)

                                If cBamFIles.Count > 0 Then gr_BamFIles.AddRange(cBamFIles.ToList)
                            Next
                            For Each orirepeat In OriRepeats.Repeats
                                orirepeat.Results.Add(gr.Value, New List(Of Integer))
                            Next
                            For Each orirepeat In OriRepeats.Repeats
                                For Each TheBam In gr_BamFIles
                                    For Each Sam In Szunyi.BAM.Bam_Basic_IO.Import.Parse(TheBam, orirepeat.SeqID, orirepeat.Start, orirepeat.Endy)
                                        Dim l = Get_Length(Sam, orirepeat)
                                        If l > -1 Then
                                            orirepeat.Results(gr.Value).Add(l)
                                        End If
                                    Next
                                Next
                            Next
                        Next
                    Else
                        For Each TheBam In Me.Bam_Files

                            Parallel.ForEach(OriRepeats.Repeats, Sub(OriRepeat As Repeat_Description)

                                                                     OriRepeat.Results.Add(TheBam.Name, New List(Of Integer))
                                                                     For Each Sam In Szunyi.BAM.Bam_Basic_IO.Import.Parse(TheBam, OriRepeat.SeqID, OriRepeat.Start, OriRepeat.Endy)
                                                                         Dim l = Get_Length(Sam, OriRepeat)
                                                                         If l > -1 Then
                                                                             OriRepeat.Results(TheBam.Name).Add(l)
                                                                         End If
                                                                     Next
                                                                 End Sub)

                        Next



                    End If
                    Dim t = Analyse()
                    Dim h = Split(t, vbCrLf)
                    Me.res = h.ToList
                End Sub
                Private Function Analyse() As String
                    Dim str As New System.Text.StringBuilder
                    For Each Rep In OriRepeats.Repeats
                        Dim t = IsThereAnyDiff(Rep)
                        str.Append(t)

                    Next
                    Return str.ToString
                End Function
                Private Function Get_Nof_Repetitions(res As List(Of Integer), Repeat_Length As Integer) As Dictionary(Of Integer, Integer)
                    Dim x As New Dictionary(Of Integer, Integer)
                    For Each Item In res
                        Dim tmp = Item / Repeat_Length
                        Dim i As Integer = tmp
                        If x.ContainsKey(i) = False Then x.Add(i, 0)
                        x(tmp) += 1
                    Next
                    Return x
                End Function
                Private Function IsThereAnyDiff(rep As Repeat_Description) As String
                    Dim str As New System.Text.StringBuilder
                    Dim AllReps As New Dictionary(Of String, Dictionary(Of Integer, Integer))
                    Dim sg As New List(Of Integer)
                    For Each rest In rep.Results
                        AllReps.Add(rest.Key, New Dictionary(Of Integer, Integer))
                        Dim reps = Get_Nof_Repetitions(rest.Value, rep.Repeat.Count)
                        AllReps(rest.Key) = reps
                        sg.AddRange(Szunyi.Text.Dict.Get_Distinct_Keys(reps))

                    Next

                    Dim dKeys = sg.Distinct.ToList
                    dKeys.Sort()
                    If dKeys.Count > 1 Then
                        str.Append(rep.Start & vbTab & rep.Endy & vbTab & rep.SeqID & vbTab & rep.Nof_Repetition & vbTab & Szunyi.Sequences.SequenceManipulation.SeqsToString.GetSeqAsString(rep.Repeat)).AppendLine()
                        str.Append("Nof Repetition").Append(vbTab).Append(Szunyi.Text.General.GetText(dKeys, vbTab)).AppendLine()
                        For Each repi In AllReps
                            str.Append(repi.Key).Append(vbTab)
                            For Each Key In dKeys
                                If repi.Value.ContainsKey(Key) Then
                                    str.Append(repi.Value(Key)).Append(vbTab)
                                Else
                                    str.Append("0").Append(vbTab)
                                End If
                            Next
                            str.AppendLine()
                        Next
                    Else
                        Dim alf As Int16 = 54
                    End If


                    Return str.ToString
                End Function
                Private Function Get_Length(SAM As Bio.IO.SAM.SAMAlignedSequence, Repeat As Repeat_Description) As Integer
                    Dim str As New System.Text.StringBuilder
                    Dim length As Integer = -1
                    Dim Fw_LB = Szunyi.Sequences.SequenceManipulation.UniqueDistinct.SimpleBoyerMooreSearch(Repeat.LeftBorder, SAM.QuerySequence)

                    Dim Rev_LB = Szunyi.Sequences.SequenceManipulation.UniqueDistinct.SimpleBoyerMooreSearch_Reverse_Complement(Repeat.LeftBorder, SAM.QuerySequence)

                    Dim Fw_RB = Szunyi.Sequences.SequenceManipulation.UniqueDistinct.SimpleBoyerMooreSearch(Repeat.RightBorder, SAM.QuerySequence)

                    Dim Rev_RB = Szunyi.Sequences.SequenceManipulation.UniqueDistinct.SimpleBoyerMooreSearch_Reverse_Complement(Repeat.RightBorder, SAM.QuerySequence)

                    Dim QSeq = Szunyi.Sequences.SequenceManipulation.SeqsToString.GetSeqAsString(SAM.QuerySequence)
                    Dim SubSeq As Bio.ISequence
                    Dim Fw_Repeat As Integer
                    Dim Rev_Repeat As Integer
                    If Fw_LB <> -1 AndAlso Fw_RB <> -1 AndAlso Fw_LB < Fw_RB Then
                        SubSeq = SAM.QuerySequence.GetSubSequence(Fw_LB, Fw_RB - Fw_LB)
                        Fw_Repeat = Szunyi.Sequences.SequenceManipulation.UniqueDistinct.SimpleBoyerMooreSearch(Repeat.Repeat, SubSeq)
                        If Fw_Repeat <> -1 Then
                            length = Fw_RB - Fw_LB - Repeat.LeftBorder.Count
                        Else
                            Dim alk As Int16 = 54
                        End If
                    ElseIf Fw_LB <> -1 AndAlso Fw_RB <> -1 AndAlso Fw_LB > Fw_RB Then
                        SubSeq = SAM.QuerySequence.GetSubSequence(Fw_RB, Fw_LB - Fw_RB)
                        Fw_Repeat = Szunyi.Sequences.SequenceManipulation.UniqueDistinct.SimpleBoyerMooreSearch(Repeat.Repeat, SubSeq)
                        If Fw_Repeat <> -1 Then
                            length = Fw_LB - Fw_RB - Repeat.LeftBorder.Count
                        Else
                            Dim alk As Int16 = 54
                        End If
                    ElseIf Rev_LB <> -1 AndAlso Rev_RB <> -1 AndAlso Rev_LB < Rev_RB Then
                        SubSeq = SAM.QuerySequence.GetSubSequence(Rev_LB, Rev_RB - Rev_LB)
                        Rev_Repeat = Szunyi.Sequences.SequenceManipulation.UniqueDistinct.SimpleBoyerMooreSearch_Reverse_Complement(Repeat.Repeat, SubSeq)
                        Fw_Repeat = Szunyi.Sequences.SequenceManipulation.UniqueDistinct.SimpleBoyerMooreSearch(Repeat.Repeat, SubSeq)
                        If Rev_Repeat <> -1 Then
                            length = Rev_RB - Rev_LB - Repeat.LeftBorder.Count
                        Else
                            Dim alk As Int16 = 54
                        End If

                    ElseIf Rev_LB <> -1 AndAlso Rev_RB <> -1 AndAlso Rev_LB > Rev_RB Then
                        SubSeq = SAM.QuerySequence.GetSubSequence(Rev_RB, Rev_LB - Rev_RB)
                        Rev_Repeat = Szunyi.Sequences.SequenceManipulation.UniqueDistinct.SimpleBoyerMooreSearch_Reverse_Complement(Repeat.Repeat, SubSeq)
                        Fw_Repeat = Szunyi.Sequences.SequenceManipulation.UniqueDistinct.SimpleBoyerMooreSearch(Repeat.Repeat, SubSeq)
                        If Rev_Repeat <> -1 Then
                            length = Rev_LB - Rev_RB - Repeat.LeftBorder.Count
                        Else
                            Dim alk As Int16 = 54
                        End If
                    End If




                    If length > 1000 Then
                        Dim alf As Integer = 54
                    End If
                    Return length

                End Function

            End Class
            Public Class Repeats
                Public Repeats As New List(Of Repeat_Description)
                Public Sub New()

                End Sub
                Public Sub New(File As FileInfo)
                    Dim Lines = Szunyi.IO.Import.Text.ParseNotFirst(File, 0)

                    For Each Line In Lines
                        Me.Repeats.Add(New Repeat_Description(Line))
                    Next
                End Sub
                Public Function Get_Values() As String
                    Dim str As New System.Text.StringBuilder
                    For Each rep In Repeats
                        str.Append(Szunyi.Text.General.GetText(rep.all, vbTab)).Append(vbTab).Append(rep.Description).AppendLine()
                    Next
                    Dim alf As Int16 = 65
                    Return str.ToString
                End Function
                Public Sub New(file As FileInfo, width As Integer, Seq As Bio.ISequence)
                    Dim Lines = Szunyi.IO.Import.Text.ParseNotFirst(file, 0)

                    For Each Line In Lines
                        Me.Repeats.Add(New Repeat_Description(Line, width, Seq))
                    Next
                End Sub
            End Class
            Public Class Repeat_Description
                Public Property Description As String
                Public LeftBorder As Bio.ISequence
                Public RightBorder As Bio.ISequence

                Public Repeat As Bio.ISequence
                Public SeqID As String
                Public Start As Integer
                Public Endy As Integer
                Public all As List(Of String)
                Public Nof_Repetition_Precise As Double
                Public Nof_Repetition As Integer
                Public CorrectLeftBorder As Bio.ISequence
                Public CorrectRightBorder As Bio.ISequence
                Public Property IsGood As Boolean
                Public Property Results As New Dictionary(Of String, List(Of Integer))

                Public Function Clone() As Repeat_Description
                    Dim x As New Repeat_Description
                    x.Description = Me.Description
                    x.LeftBorder = Me.LeftBorder
                    x.RightBorder = Me.RightBorder
                    x.Repeat = Me.Repeat
                    x.SeqID = Me.SeqID
                    x.Start = Me.Start
                    x.Endy = Me.Endy
                    x.all = Me.all
                    x.Nof_Repetition_Precise = Me.Nof_Repetition_Precise
                    x.Nof_Repetition = Me.Nof_Repetition
                    x.CorrectLeftBorder = Me.CorrectLeftBorder
                    x.CorrectRightBorder = Me.CorrectRightBorder
                    x.IsGood = Me.IsGood
                    x.Results = Me.Results
                    Return x
                End Function
                Public Sub New(line As String)
                    Dim s = Split(line, vbTab)
                    Me.all = s.ToList
                    Me.SeqID = s(0)
                    Me.Start = s(1)
                    Me.Endy = s(2)
                    Me.Nof_Repetition = s(4).Split(".").First
                    Me.Repeat = New Bio.Sequence(Bio.Alphabets.AmbiguousDNA, s(5))
                    Me.LeftBorder = New Bio.Sequence(Bio.Alphabets.AmbiguousDNA, s(6))
                    Me.RightBorder = New Bio.Sequence(Bio.Alphabets.AmbiguousDNA, s(8))
                    Me.Nof_Repetition_Precise = s(4)
                    Me.Description = s.Last
                End Sub
                Public Sub New(line As String, width As Integer, Seq As Bio.ISequence)
                    Dim s = Split(line, vbTab)
                    Me.all = s.ToList
                    Me.SeqID = s(0)
                    Me.Repeat = New Bio.Sequence(Bio.Alphabets.AmbiguousDNA, s(5))
                    Me.LeftBorder = New Bio.Sequence(Bio.Alphabets.AmbiguousDNA, s(6))
                    Me.RightBorder = New Bio.Sequence(Bio.Alphabets.AmbiguousDNA, s(8))
                    Me.Start = s(1)
                    Me.Endy = s(2)
                    Me.Nof_Repetition = s(4).Split(".").First
                    Me.Nof_Repetition_Precise = s(4)
                    Me.LeftBorder = Seq.GetSubSequence(Me.Start - width - 1, width)
                    Me.RightBorder = Seq.GetSubSequence(Me.Endy, width)
                    Me.Description = s.Last
                End Sub
                Public Overrides Function ToString() As String
                    Dim str As New System.Text.StringBuilder
                    str.Append(Me.SeqID).Append(vbTab)

                    str.Append(Me.Start).Append(vbTab)
                    str.Append(Me.Endy).Append(vbTab)
                    str.Append(Me.Repeat.Count).Append(vbTab)
                    str.Append(Me.Nof_Repetition_Precise).Append(vbTab)

                    str.Append(Me.Repeat.ConvertToString).Append(vbTab)
                    str.Append(Me.LeftBorder).Append(vbTab)
                    str.Append(vbTab)
                    str.Append(Me.RightBorder).Append(vbTab)
                    Return str.ToString



                End Function
                Public Sub New()

                End Sub
            End Class

        End Namespace
    End Namespace
End Namespace

