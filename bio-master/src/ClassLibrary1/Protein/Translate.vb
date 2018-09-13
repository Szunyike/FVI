Imports Bio.IO.GenBank

Namespace Szunyi
    Public Class Translate

        ''' <summary>
        ''' One based Frame
        ''' </summary>
        ''' <param name="Seq"></param>
        ''' <param name="Frame"></param>
        ''' <returns></returns>
        Public Shared Function TranaslateSeq(Seq As Bio.Sequence, Optional Frame As Integer = 0) As Bio.ISequence
            If Frame > 0 Then
                Frame -= 1
                Dim RNASeq = Bio.Algorithms.Translation.Transcription.Transcribe(Seq)
                Dim AASeq As Bio.ISequence = Bio.Algorithms.Translation.ProteinTranslation.Translate(RNASeq, Frame)
                AASeq.ID = Seq.ID
                Return AASeq

            Else
                Frame = System.Math.Abs(Frame + 1)
                Dim TheSeq = Seq.GetReverseComplementedSequence
                Dim RNASeq = Bio.Algorithms.Translation.Transcription.Transcribe(TheSeq)
                Dim AASeq As Bio.ISequence = Bio.Algorithms.Translation.ProteinTranslation.Translate(RNASeq, Frame)
                AASeq.ID = Seq.ID
                Return AASeq
            End If



        End Function
        Public Shared Function TranaslateSeqs(Seqs As List(Of Bio.ISequence), Optional RemoveTerminalStop As Boolean = False) As List(Of Bio.ISequence)
            Dim out As New List(Of Bio.ISequence)
            For Each Seq In Seqs
                out.Add(TranaslateSeq(Seq, 1))
            Next
            If RemoveTerminalStop = False Then Return out
            Dim Out2 As New List(Of Bio.ISequence)
            For Each S In out
                If S.Last = Bio.Alphabets.AmbiguousProtein.Ter Then
                    Dim NewSeq(S.Count - 2) As Byte
                    Dim Seq As Bio.Sequence = S
                    Seq.CopyTo(NewSeq, 0, Seq.Count - 1)
                    Out2.Add(New Bio.Sequence(Bio.Alphabets.AmbiguousProtein, NewSeq))

                    Out2.Last.ID = Seq.ID
                Else

                    Out2.Add(S)

                End If
            Next
            Return Out2

        End Function
        Public Shared Function TranaslateAsString(Seq As Bio.Sequence) As String

            Dim RNASeq = Bio.Algorithms.Translation.Transcription.Transcribe(Seq)
            Dim AASeq As Bio.Sequence = Bio.Algorithms.Translation.ProteinTranslation.Translate(RNASeq)
            Return AASeq.ConvertToString(0, AASeq.Count)

        End Function
        Public Shared Function TranaslateIntoString(Seq As Bio.Sequence, Frame As Integer) As String
            If Frame > 0 Then
                Dim RNASeq = Bio.Algorithms.Translation.Transcription.Transcribe(Seq)
                Dim AASeq As Bio.Sequence = Bio.Algorithms.Translation.ProteinTranslation.Translate(RNASeq, Frame - 1)
                Return AASeq.ConvertToString(0, AASeq.Count)
            Else
                Dim x As New Bio.Sequence(Bio.Alphabets.AmbiguousDNA, Seq.ToArray)
                Dim RevSeq = x.GetReverseComplementedSequence
                Dim RNASeq = Bio.Algorithms.Translation.Transcription.Transcribe(RevSeq)
                Dim AASeq As Bio.Sequence = Bio.Algorithms.Translation.ProteinTranslation.Translate(RNASeq, System.Math.Abs(Frame) - 1)

                Dim t = AASeq.ConvertToString(0, AASeq.Count)

                Return t
            End If


        End Function

        Public Shared Function GetBefore_M_or_Star(trSeq As String, startPos As Integer) As Integer
            For i1 = startPos To 0 Step -1
                Dim IndexOfStop = trSeq.LastIndexOf("*", startPos)
                If IndexOfStop = -1 Then IndexOfStop = 0
                Dim IndexOfFirstATG = trSeq.IndexOf("M", IndexOfStop)
                If IndexOfFirstATG > startPos Then
                    Return IndexOfStop
                ElseIf IndexOfFirstATG > 0 Then
                    Return IndexOfFirstATG
                End If

            Next
            Return 0
        End Function


        Public Shared Function GetAfter_Star(trSeq As String, startPos As Integer) As Integer
            For i1 = startPos To trSeq.Length - 1
                If trSeq.Substring(i1, 1) = "*" Then
                    If i1 + 1 > trSeq.Length Then
                        Return i1
                    Else
                        Return i1 + 1
                    End If
                End If
            Next
            Return trSeq.Length
        End Function
        Public Shared Function TranaslateFromFeature(Feat As FeatureItem, Seq As Bio.Sequence) As String
            Dim TheSeq As Bio.Sequence = Feat.GetSubSequence(Seq)

            If Feat.Location.Operator = LocationOperator.Complement Then TheSeq = TheSeq.GetReversedSequence

            Dim RNASeq = Bio.Algorithms.Translation.Transcription.Transcribe(TheSeq)
            Dim AASeq As Bio.Sequence = Bio.Algorithms.Translation.ProteinTranslation.Translate(RNASeq)
            Return AASeq.ConvertToString(0, AASeq.Count)

        End Function

        Public Shared Function TranslateIntoSequenceAllFrames(Seq As Bio.Sequence) As List(Of Bio.Sequence)
            Dim out As New List(Of Bio.Sequence)
            Dim RNASeq = Bio.Algorithms.Translation.Transcription.Transcribe(Seq)

            out.Add(TranslateIntoSequnence1Frame(Seq, RNASeq))
            out.Add(TranslateIntoSequnence2Frame(Seq, RNASeq))
            out.Add(TranslateIntoSequnence3Frame(Seq, RNASeq))

            Dim tmpSeq = Seq.GetReverseComplementedSequence
            RNASeq = Bio.Algorithms.Translation.Transcription.Transcribe(tmpSeq)
            out.Add(TranslateIntoSequnence1Frame(Seq, RNASeq))
            out.Add(TranslateIntoSequnence2Frame(Seq, RNASeq))
            out.Add(TranslateIntoSequnence3Frame(Seq, RNASeq))
            Return out
        End Function
        Public Shared Function TranslateIntoSequnence1Frame(Seq As Bio.Sequence, Optional RNASeq As Bio.Sequence = Nothing) As Bio.Sequence
            If IsNothing(RNASeq) = True Then RNASeq = Bio.Algorithms.Translation.Transcription.Transcribe(Seq)
            Dim AASeq As Bio.Sequence = Bio.Algorithms.Translation.ProteinTranslation.Translate(RNASeq, 0)
            AASeq.ID = Seq.ID
            Return AASeq
        End Function
        Public Shared Function TranslateIntoSequnence2Frame(Seq As Bio.Sequence, Optional RNASeq As Bio.Sequence = Nothing) As Bio.Sequence
            If IsNothing(RNASeq) = True Then RNASeq = Bio.Algorithms.Translation.Transcription.Transcribe(Seq)
            Dim AASeq As Bio.Sequence = Bio.Algorithms.Translation.ProteinTranslation.Translate(RNASeq, 1)
            AASeq.ID = Seq.ID
            Return AASeq
        End Function
        Public Shared Function TranslateIntoSequnence3Frame(Seq As Bio.Sequence, Optional RNASeq As Bio.Sequence = Nothing) As Bio.Sequence
            If IsNothing(RNASeq) = True Then RNASeq = Bio.Algorithms.Translation.Transcription.Transcribe(Seq)
            Dim AASeq As Bio.Sequence = Bio.Algorithms.Translation.ProteinTranslation.Translate(RNASeq, 2)
            AASeq.ID = Seq.ID
            Return AASeq
        End Function

        Public Shared Function TranslateIntoSequnenceMinus1Frame(Seq As Bio.Sequence, Optional RNASeq As Bio.Sequence = Nothing) As Bio.Sequence
            If IsNothing(RNASeq) = True Then
                Dim RevCompl_Seq = Seq.GetReverseComplementedSequence
                RNASeq = Bio.Algorithms.Translation.Transcription.Transcribe(RevCompl_Seq)
            End If
            Dim AASeq As Bio.Sequence = Bio.Algorithms.Translation.ProteinTranslation.Translate(RNASeq, 0)
            AASeq.ID = Seq.ID
            Return AASeq
        End Function
        Public Shared Function TranslateIntoSequnenceMinus2Frame(Seq As Bio.Sequence, Optional RNASeq As Bio.Sequence = Nothing) As Bio.Sequence
            If IsNothing(RNASeq) = True Then
                Dim RevCompl_Seq = Seq.GetReverseComplementedSequence
                RNASeq = Bio.Algorithms.Translation.Transcription.Transcribe(RevCompl_Seq)
            End If
            Dim AASeq As Bio.Sequence = Bio.Algorithms.Translation.ProteinTranslation.Translate(RNASeq, 1)
            AASeq.ID = Seq.ID
            Return AASeq
        End Function
        Public Shared Function TranslateIntoSequnenceMinus3Frame(Seq As Bio.Sequence, Optional RNASeq As Bio.Sequence = Nothing) As Bio.Sequence
            If IsNothing(RNASeq) = True Then
                Dim RevCompl_Seq = Seq.GetReverseComplementedSequence
                RNASeq = Bio.Algorithms.Translation.Transcription.Transcribe(RevCompl_Seq)
            End If
            Dim AASeq As Bio.Sequence = Bio.Algorithms.Translation.ProteinTranslation.Translate(RNASeq, 2)
            AASeq.ID = Seq.ID
            Return AASeq
        End Function
        Public Shared Function TranslateIntoSequence(Seq As Bio.Sequence, Frames As Integer()) As List(Of Bio.Sequence)
            Dim out As New List(Of Bio.Sequence)
            If Frames.Contains(1) Or Frames.Contains(2) Or Frames.Contains(3) Then
                Dim RNASeq = Bio.Algorithms.Translation.Transcription.Transcribe(Seq)
                If Frames.Contains(1) Then
                    Dim AASeq As Bio.Sequence = Bio.Algorithms.Translation.ProteinTranslation.Translate(RNASeq, 0)
                    AASeq.ID = Seq.ID
                    If AASeq.Count > 0 Then out.Add(AASeq)
                End If
                If Frames.Contains(2) Then
                    Dim AASeq As Bio.Sequence = Bio.Algorithms.Translation.ProteinTranslation.Translate(RNASeq, 1)
                    AASeq.ID = Seq.ID & "_AA_frame2"
                    If AASeq.Count > 0 Then out.Add(AASeq)
                End If
                If Frames.Contains(3) Then
                    Dim AASeq As Bio.Sequence = Bio.Algorithms.Translation.ProteinTranslation.Translate(RNASeq, 2)
                    AASeq.ID = Seq.ID & "_AA_frame3"
                    If AASeq.Count > 0 Then out.Add(AASeq)
                End If
            End If

            If Frames.Contains(-1) Or Frames.Contains(-2) Or Frames.Contains(-3) Then
                Dim RevComplSeq As Bio.Sequence = Seq.GetReverseComplementedSequence

                Dim RNASeq = Bio.Algorithms.Translation.Transcription.Transcribe(RevComplSeq)
                If Frames.Contains(1) Then
                    Dim AASeq As Bio.Sequence = Bio.Algorithms.Translation.ProteinTranslation.Translate(RNASeq, 0)
                    AASeq.ID = Seq.ID & "_AA_frame-1"
                    If AASeq.Count > 0 Then out.Add(AASeq)
                End If
                If Frames.Contains(2) Then
                    Dim AASeq As Bio.Sequence = Bio.Algorithms.Translation.ProteinTranslation.Translate(RNASeq, 1)
                    AASeq.ID = Seq.ID & "_AA_frame-2"
                    If AASeq.Count > 0 Then out.Add(AASeq)
                End If
                If Frames.Contains(3) Then
                    Dim AASeq As Bio.Sequence = Bio.Algorithms.Translation.ProteinTranslation.Translate(RNASeq, 2)
                    AASeq.ID = Seq.ID & "_AA_frame-3"
                    If AASeq.Count > 0 Then out.Add(AASeq)
                End If
            End If
            Return out
        End Function

    End Class
End Namespace

