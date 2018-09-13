Namespace Szunyi.mRNA.Transcript

    Public Class TemplateSwitch
        Public Property Loci As Bio.IO.GenBank.ILocation
        Public Property FiveOH As Bio.Sequence
        Public Property Length As Integer
        Public Property DonorSite As String
        Public Property AcceptorSite As String
        Public Property Repeat_First_Part As String
        Public Property Repeat_Second_Part As String
        Public Property Consensus As String
        Public Property Repeat_Length As Integer
        Public Property Count As Integer = 1
        Public Property Sw As New Bio.Algorithms.Alignment.SmithWatermanAligner
        Public Property FirstOffset As Int16
        Public Property LastOffset As Int16
        Public Property Surrounding_Seq_DonorSite As String
        Public Property Surrounding_Seq_AcceptorSite As String

        Public Sub New(Seq As Bio.ISequence, Intron As Bio.IO.GenBank.FeatureItem)
            Me.Loci = Intron.Location
            DoIt(Seq)
        End Sub
        Public Sub New(Seq As Bio.ISequence, Intron As Bio.IO.GenBank.Location)
            Me.Loci = Intron
            DoIt(Seq)
        End Sub
        Public Sub New(Seq As Bio.ISequence, Intron As Bio.IO.GenBank.ILocation)
            Me.Loci = Intron
            DoIt(Seq)
        End Sub
        Public Sub New(Seq As Bio.ISequence, Introns As List(Of Bio.IO.GenBank.ILocation))
            Me.Loci = Introns.First
            Me.Count = Introns.Count
            DoIt(Seq)
        End Sub
        Private Sub DoIt(Seq As Bio.ISequence)
            Sw.SimilarityMatrix = New Bio.SimilarityMatrices.DiagonalSimilarityMatrix(2, -10)
            Dim s11 = Seq.GetSubSequence(Loci.LocationStart - 7, 6 * 2)
            Dim s12 = Seq.GetSubSequence(Loci.LocationEnd - 1 - 5, 6 * 2)
            Me.Surrounding_Seq_DonorSite = s11.ConvertToString
            Me.Surrounding_Seq_AcceptorSite = s12.ConvertToString

            If Loci.IsComplementer = True Then
                Dim s = Seq.GetSubSequence(Loci.LocationStart - 1, 2)
                AcceptorSite = s.GetReverseComplementedSequence.ConvertToString
                Dim sl = Seq.GetSubSequence(Loci.LocationEnd - 2, 2)
                DonorSite = sl.GetReverseComplementedSequence.ConvertToString
            Else
                DonorSite = Seq.GetSubSequence(Loci.LocationStart - 1, 2).ConvertToString
                AcceptorSite = Seq.GetSubSequence(Loci.LocationEnd - 2, 2).ConvertToString

            End If


            For i1 = 5 To 2 Step -1
                Dim s1 = Seq.GetSubSequence(Loci.LocationStart - i1 - 1, i1 * 2)
                Dim s2 = Seq.GetSubSequence(Loci.LocationEnd - 1 - i1 + 1, i1 * 2)

                Dim res = Sw.Align(s1, s2)
                Dim Reps = Get_Overlaps(res.First.AlignedSequences, i1, i1)
                If IsNothing(Reps) = False Then

                    Me.Repeat_First_Part = Reps.Sequences.First.ConvertToString
                    Me.Repeat_Second_Part = Reps.Sequences.Last.ConvertToString
                    Dim b As Bio.Sequence = Reps.Metadata("Consensus")
                    Me.Consensus = b.ConvertToString
                    Me.Repeat_Length = b.Count
                    Dim StartOffSets As List(Of Long) = Reps.Metadata("StartOffsets")
                    Me.FirstOffset = StartOffSets.First
                    Me.LastOffset = StartOffSets.Last
                    If Repeat_Length >= 4 Then
                        Dim kj As Int16 = 43
                    End If
                    Exit For
                End If
            Next




        End Sub
        Private Function Get_Overlaps(AlignedSequences As List(Of Bio.Algorithms.Alignment.IAlignedSequence), LeftPos As Integer, RightPos As Integer) As Bio.Algorithms.Alignment.IAlignedSequence
            For Each AlSeq In AlignedSequences
                Dim StartOffSets As List(Of Long) = AlSeq.Metadata("StartOffsets")
                Dim EndOffSets As List(Of Long) = AlSeq.Metadata("EndOffsets")
                If StartOffSets.First <= LeftPos AndAlso StartOffSets.Last <= LeftPos AndAlso EndOffSets.First >= RightPos - 1 AndAlso EndOffSets.Last >= RightPos - 1 Then
                    Return AlSeq
                End If
            Next
            Return Nothing
        End Function

        Public Overrides Function ToString() As String
            Dim str As New System.Text.StringBuilder
            str.Append(Me.Count).Append(vbTab)
            str.Append(Szunyi.Location.Common.GetLocationStringTab(Me.Loci)).Append(vbTab)
            str.Append(Szunyi.Location.Common.Get_Length(Me.Loci)).Append(vbTab)
            str.Append(Me.DonorSite).Append(vbTab)
            str.Append(Me.AcceptorSite).Append(vbTab)
            str.Append(Me.Surrounding_Seq_DonorSite).Append(vbTab)
            str.Append(Me.Surrounding_Seq_AcceptorSite).Append(vbTab)
            str.Append(Me.Repeat_First_Part).Append(vbTab)
            str.Append(Me.Repeat_Second_Part).Append(vbTab)
            str.Append(Me.Consensus).Append(vbTab)
            str.Append(Me.Repeat_Length).Append(vbTab)
            str.Append(Me.FirstOffset).Append(vbTab)
            str.Append(Me.LastOffset).Append(vbTab)
            Return str.ToString
        End Function
    End Class

    Public Class Introns


            Public Shared Function Get_Overlaps(AlignedSequences As List(Of Bio.Algorithms.Alignment.IAlignedSequence), LeftPos As Integer, RightPos As Integer) As Bio.Algorithms.Alignment.IAlignedSequence
                For Each AlSeq In AlignedSequences
                    Dim StartOffSets As List(Of Long) = AlSeq.Metadata("StartOffsets")
                Dim EndOffSets As List(Of Long) = AlSeq.Metadata("EndOffsets")

                If StartOffSets.First = StartOffSets.Last Then
                    If StartOffSets.First < LeftPos And EndOffSets.First >= LeftPos Then
                        Return AlSeq
                    Else
                        Dim kj As Int16 = 54
                    End If

                End If

                'If StartOffSets.First <= LeftPos AndAlso StartOffSets.Last <= LeftPos AndAlso EndOffSets.First >= RightPos - 1 AndAlso EndOffSets.Last >= RightPos - 1 Then
                'Return AlSeq
                ' End If
            Next
                Return Nothing
            End Function
            Public Shared Function TemplateSwichHeader() As String

                Dim str As New System.Text.StringBuilder
                str.Append("strand").Append(vbTab)
                str.Append("start").Append(vbTab)
                str.Append("end").Append(vbTab)
                str.Append("count").Append(vbTab)
                str.Append("NOf diff exp").Append(vbTab)
                str.Append("intron length").Append(vbTab)
                str.Append("Donor Site").Append(vbTab)
                str.Append("Acceptor Site").Append(vbTab)
                str.Append("Repeat First Part").Append(vbTab)
                str.Append("Repeat Second Part").Append(vbTab)
                str.Append("Consensus").Append(vbTab)
                str.Append("Repeat Length")
                Return str.ToString




            End Function
            Public Shared Function TemplateSwitch(Seq As Bio.ISequence,
                                                  Intron As Bio.IO.GenBank.FeatureItem,
                                                  Sw As Bio.Algorithms.Alignment.SmithWatermanAligner) As String
                Dim str As New System.Text.StringBuilder
                '  str.Append(Szunyi.Location.Common.GetLocationStringTab(Intron)).Append(vbTab)
                '     str.Append(Intron.Qualifiers(Bio.IO.GenBank.StandardQualifierNames.IdentifiedBy).Count).Append(vbTab)
                '       str.Append(Intron.Label).Append(vbTab)
                str.Append(Intron.Location.LocationEnd - Intron.Location.LocationStart).Append(vbTab)
                If Intron.Location.IsComplementer = True Then
                    Dim s = Seq.GetSubSequence(Intron.Location.LocationStart - 1, 2)
                    s = s.GetReverseComplementedSequence
                    Dim sl = Seq.GetSubSequence(Intron.Location.LocationEnd - 2, 2)
                    sl = sl.GetReverseComplementedSequence
                    str.Append(sl.ConvertToString).Append(vbTab)
                    str.Append(s.ConvertToString).Append(vbTab)

                Else
                    Dim s = Seq.GetSubSequence(Intron.Location.LocationStart - 1, 2)
                    str.Append(s.ConvertToString).Append(vbTab)
                    Dim sl = Seq.GetSubSequence(Intron.Location.LocationEnd - 2, 2)
                    str.Append(sl.ConvertToString).Append(vbTab)
                End If
                Dim s11 = Seq.GetSubSequence(Intron.Location.LocationStart - 1 - 5, 5 * 2)
                Dim s12 = Seq.GetSubSequence(Intron.Location.LocationEnd - 5, 5 * 2)
                str.Append(s11.ConvertToString).Append(vbTab)
                str.Append(s12.ConvertToString).Append(vbTab)
                For i1 = 5 To 2 Step -1
                    Dim s1 = Seq.GetSubSequence(Intron.Location.LocationStart - 1 - i1, i1 * 2)
                    Dim s2 = Seq.GetSubSequence(Intron.Location.LocationEnd - i1, i1 * 2)

                    Dim res = Sw.Align(s1, s2)
                    Dim Reps = Get_Overlaps(res.First.AlignedSequences, i1, i1)
                    If IsNothing(Reps) = False Then
                        str.Append(Reps.Sequences.First.ConvertToString).Append(vbTab)
                        str.Append(Reps.Sequences.Last.ConvertToString).Append(vbTab)
                        Dim b As Bio.Sequence = Reps.Metadata("Consensus")
                        str.Append(b.ConvertToString).Append(vbTab)
                        str.Append(Reps.Sequences.First.Count)
                        Exit For
                    End If
                Next

                Return str.ToString
            End Function

        Public Shared Function TemplateSwitch(Seq As Bio.ISequence,
                                                  Location As Bio.IO.GenBank.ILocation,
                                                  Sw As Bio.Algorithms.Alignment.SmithWatermanAligner) As TemplateSwitch
            Dim str As New System.Text.StringBuilder
            str.Append(Szunyi.Location.Common.GetLocationStringTab(Location)).Append(vbTab)
            str.Append(Location.LocationEnd - Location.LocationStart).Append(vbTab)

            Dim x As New TemplateSwitch(Seq, Location)

            Return x
        End Function
        Public Shared Function TemplateSwitch(Seq As Bio.ISequence,
                                                  Locations As List(Of Bio.IO.GenBank.ILocation),
                                                  Sw As Bio.Algorithms.Alignment.SmithWatermanAligner) As String
            Dim Location = Locations.First
            Dim str As New System.Text.StringBuilder

            Dim x As New Szunyi.mRNA.Transcript.TemplateSwitch(Seq, Locations)

            str.Append(Szunyi.Location.Common.GetLocationStringTab(Location)).Append(vbTab)
            str.Append(Location.LocationEnd - Location.LocationStart).Append(vbTab)
            If Location.IsComplementer = True Then
                Dim s = Seq.GetSubSequence(Location.LocationStart - 1, 2)
                s = s.GetReverseComplementedSequence
                Dim sl = Seq.GetSubSequence(Location.LocationEnd - 2, 2)
                sl = sl.GetReverseComplementedSequence
                str.Append(sl.ConvertToString).Append(vbTab)
                str.Append(s.ConvertToString).Append(vbTab)
            Else
                Dim s = Seq.GetSubSequence(Location.LocationStart - 1, 2)
                str.Append(s.ConvertToString).Append(vbTab)
                Dim sl = Seq.GetSubSequence(Location.LocationEnd - 2, 2)
                str.Append(sl.ConvertToString).Append(vbTab)
            End If
            Dim s11 = Seq.GetSubSequence(Location.LocationStart - 1 - 5, 5 * 2)
            Dim s12 = Seq.GetSubSequence(Location.LocationEnd - 5, 5 * 2)
            str.Append(s11.ConvertToString).Append(vbTab)
            str.Append(s12.ConvertToString).Append(vbTab)
            For i1 = 5 To 2 Step -1
                Dim s1 = Seq.GetSubSequence(Location.LocationStart - 1 - i1, i1 * 2)
                Dim s2 = Seq.GetSubSequence(Location.LocationEnd - i1, i1 * 2)

                Dim res = Sw.Align(s1, s2)
                Dim Reps = Get_Overlaps(res.First.AlignedSequences, i1, i1)
                If IsNothing(Reps) = False Then
                    str.Append(Reps.Sequences.First.ConvertToString).Append(vbTab)
                    str.Append(Reps.Sequences.Last.ConvertToString).Append(vbTab)
                    Dim b As Bio.Sequence = Reps.Metadata("Consensus")
                    str.Append(b.ConvertToString).Append(vbTab)
                    str.Append(Reps.Sequences.First.Count)
                    Exit For
                End If
            Next

            Return str.ToString
        End Function

        Private Shared Function Get_Repetition_Rev(s1 As Bio.ISequence, s2 As Bio.ISequence) As String
                Dim str As New System.Text.StringBuilder
                For i1 = s1.Count - 1 To 0 Step -1
                    If s1(i1) = s2(i1) Then
                        str.Append(ChrW(s1(i1)))
                    Else
                        Exit For
                    End If
                Next
                str.Append(vbTab).Append(str.Length - 1)
                Return str.ToString
            End Function
            Private Shared Function Get_Repetition(s1 As Bio.ISequence, s2 As Bio.ISequence) As String
                Dim str As New System.Text.StringBuilder
                For i1 = 0 To s1.Count - 1
                    If s1(i1) = s2(i1) Then
                        str.Append(ChrW(s1(i1)))
                    Else
                        Exit For
                    End If
                Next
                str.Append(vbTab).Append(str.Length - 1)
                Return str.ToString
            End Function
        End Class
    End Namespace


