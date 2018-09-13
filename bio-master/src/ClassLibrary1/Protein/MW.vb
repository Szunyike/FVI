Imports Bio


Namespace Szunyi
    Namespace Protein
        Public Class MW
            Public Property MWOfAAs_Monosiotopic As New Dictionary(Of Byte, Double)
            Public Property MWOfAAs_Avarage As New Dictionary(Of Byte, Double)

            Public Sub New()
                MWOfAAs_Monosiotopic.Add(AscW("A"), 71.03711)
                MWOfAAs_Monosiotopic.Add(AscW("R"), 156.10111)
                MWOfAAs_Monosiotopic.Add(AscW("N"), 114.04293)
                MWOfAAs_Monosiotopic.Add(AscW("D"), 115.02694)
                MWOfAAs_Monosiotopic.Add(AscW("C"), 103.00919)
                MWOfAAs_Monosiotopic.Add(AscW("E"), 129.04259)
                MWOfAAs_Monosiotopic.Add(AscW("Q"), 128.05858)
                MWOfAAs_Monosiotopic.Add(AscW("G"), 57.02146)
                MWOfAAs_Monosiotopic.Add(AscW("H"), 137.05891)
                MWOfAAs_Monosiotopic.Add(AscW("I"), 113.08406)
                MWOfAAs_Monosiotopic.Add(AscW("L"), 113.08406)
                MWOfAAs_Monosiotopic.Add(AscW("K"), 128.09496)
                MWOfAAs_Monosiotopic.Add(AscW("M"), 131.04049)
                MWOfAAs_Monosiotopic.Add(AscW("F"), 147.06841)
                MWOfAAs_Monosiotopic.Add(AscW("P"), 97.05276)
                MWOfAAs_Monosiotopic.Add(AscW("S"), 87.03203)
                MWOfAAs_Monosiotopic.Add(AscW("T"), 101.04768)
                MWOfAAs_Monosiotopic.Add(AscW("W"), 186.07931)
                MWOfAAs_Monosiotopic.Add(AscW("Y"), 163.06333)
                MWOfAAs_Monosiotopic.Add(AscW("V"), 99.06841)

                MWOfAAs_Avarage.Add(AscW("A"), 71.0788)
                MWOfAAs_Avarage.Add(AscW("R"), 156.1875)
                MWOfAAs_Avarage.Add(AscW("N"), 114.1038)
                MWOfAAs_Avarage.Add(AscW("D"), 115.0886)
                MWOfAAs_Avarage.Add(AscW("C"), 103.1388)
                MWOfAAs_Avarage.Add(AscW("E"), 129.1155)
                MWOfAAs_Avarage.Add(AscW("Q"), 128.1307)
                MWOfAAs_Avarage.Add(AscW("G"), 57.0519)
                MWOfAAs_Avarage.Add(AscW("H"), 137.1411)
                MWOfAAs_Avarage.Add(AscW("I"), 113.1594)
                MWOfAAs_Avarage.Add(AscW("L"), 113.1594)
                MWOfAAs_Avarage.Add(AscW("K"), 128.1741)
                MWOfAAs_Avarage.Add(AscW("M"), 131.1926)
                MWOfAAs_Avarage.Add(AscW("F"), 147.1766)
                MWOfAAs_Avarage.Add(AscW("P"), 97.1167)
                MWOfAAs_Avarage.Add(AscW("S"), 87.0782)
                MWOfAAs_Avarage.Add(AscW("T"), 101.1051)
                MWOfAAs_Avarage.Add(AscW("W"), 186.2132)
                MWOfAAs_Avarage.Add(AscW("Y"), 163.176)
                MWOfAAs_Avarage.Add(AscW("V"), 99.1326)

            End Sub
            Public Function GetMW(Seq As Bio.ISequence) As Double
                If Seq.Count = 0 Then Return 0
                Dim MW As Double = 0
                For Each AA In Seq
                    If MWOfAAs_Avarage.ContainsKey(AA) Then
                        MW += Me.MWOfAAs_Avarage(AA)
                    End If

                Next
                Dim MwI As Integer = MW + 18.01528
                Return MwI

            End Function
            Public Function GetMWs(Seqs As List(Of Bio.ISequence)) As List(Of Double)
                Dim out As New List(Of Double)
                For Each Seq In Seqs
                    out.Add(GetMW(Seq))
                Next
                Return out
            End Function
        End Class

        Public Class PI
            Private Cterm_pI_expasy As Dictionary(Of String, Double) = New Dictionary(Of String, Double)()
            Private Nterm_pI_expasy As Dictionary(Of String, Double) = New Dictionary(Of String, Double)()
            Private sideGroup_pI_expasy As Dictionary(Of String, Double) = New Dictionary(Of String, Double)()
            Private FoRmU As Double = 0.0R
            Private seq As String = Nothing ' sequenceAA
            Public Sub New()
                Me.Cterm_pI_expasy.Add("A", 3.55R)
                Me.Cterm_pI_expasy.Add("R", 3.55R)
                Me.Cterm_pI_expasy.Add("N", 3.55R)
                Me.Cterm_pI_expasy.Add("D", 4.55R)
                Me.Cterm_pI_expasy.Add("C", 3.55R)
                Me.Cterm_pI_expasy.Add("E", 4.75R)
                Me.Cterm_pI_expasy.Add("Q", 3.55R)
                Me.Cterm_pI_expasy.Add("G", 3.55R)
                Me.Cterm_pI_expasy.Add("H", 3.55R)
                Me.Cterm_pI_expasy.Add("I", 3.55R)
                Me.Cterm_pI_expasy.Add("L", 3.55R)
                Me.Cterm_pI_expasy.Add("K", 3.55R)
                Me.Cterm_pI_expasy.Add("M", 3.55R)
                Me.Cterm_pI_expasy.Add("F", 3.55R)
                Me.Cterm_pI_expasy.Add("P", 3.55R)
                Me.Cterm_pI_expasy.Add("S", 3.55R)
                Me.Cterm_pI_expasy.Add("T", 3.55R)
                Me.Cterm_pI_expasy.Add("W", 3.55R)
                Me.Cterm_pI_expasy.Add("Y", 3.55R)
                Me.Cterm_pI_expasy.Add("V", 3.55R)
                Me.Nterm_pI_expasy.Add("A", 7.59R)


                Me.Nterm_pI_expasy.Add("R", 7.5R)
                Me.Nterm_pI_expasy.Add("N", 7.5R)
                Me.Nterm_pI_expasy.Add("D", 7.5R)
                Me.Nterm_pI_expasy.Add("C", 7.5R)
                Me.Nterm_pI_expasy.Add("E", 7.7R)
                Me.Nterm_pI_expasy.Add("Q", 7.5R)
                Me.Nterm_pI_expasy.Add("G", 7.5R)
                Me.Nterm_pI_expasy.Add("H", 7.5R)
                Me.Nterm_pI_expasy.Add("I", 7.5R)
                Me.Nterm_pI_expasy.Add("L", 7.5R)
                Me.Nterm_pI_expasy.Add("K", 7.5R)
                Me.Nterm_pI_expasy.Add("M", 7.0R)
                Me.Nterm_pI_expasy.Add("F", 7.5R)
                Me.Nterm_pI_expasy.Add("P", 8.36R)
                Me.Nterm_pI_expasy.Add("S", 6.93R)
                Me.Nterm_pI_expasy.Add("T", 6.82R)
                Me.Nterm_pI_expasy.Add("W", 7.5R)
                Me.Nterm_pI_expasy.Add("Y", 7.5R)
                Me.Nterm_pI_expasy.Add("V", 7.44R)

                Me.sideGroup_pI_expasy.Add("R", -12.0R)
                Me.sideGroup_pI_expasy.Add("D", 4.05R)
                Me.sideGroup_pI_expasy.Add("C", 9.0R)
                Me.sideGroup_pI_expasy.Add("E", 4.45R)
                Me.sideGroup_pI_expasy.Add("H", -5.98R)
                Me.sideGroup_pI_expasy.Add("K", -10.0R)
                Me.sideGroup_pI_expasy.Add("Y", 10.0R)

            End Sub
            Public Function Get_PI(ByVal Seq As Bio.ISequence) As Double
                If Seq.Count = 0 Then Return Double.NaN
                Me.seq = Szunyi.Sequences.SequenceManipulation.SeqsToString.GetSeqAsString(Seq)
                Me.seq = Me.seq.Trim("*")
                ' sequenceAA

                Const epsilon As Double = 0.001
                Const iterationMax As Integer = 10000

                Dim counter As Integer = 0
                Dim pHs As Double = -2
                Dim pHe As Double = 16
                Dim pHm As Double


                Do While (counter < iterationMax) AndAlso (System.Math.Abs(pHs - pHe) >= epsilon)
                    pHm = (pHs + pHe) / 2

                    Dim pcs As Double = getpI(Nterm_pI_expasy, Cterm_pI_expasy, sideGroup_pI_expasy, pHs)

                    Dim pcm As Double = getpI(Nterm_pI_expasy, Cterm_pI_expasy, sideGroup_pI_expasy, pHm)
                    If pcs < 0 Then
                        Return pHs
                    End If
                    If ((pcs < 0) AndAlso (pcm > 0)) OrElse ((pcs > 0) AndAlso (pcm < 0)) Then
                        pHe = pHm
                    Else
                        pHs = pHm
                    End If
                    counter += 1
                Loop

                Dim pHround As Double = System.Math.Round(((pHs + pHe) / 2) * 100.0R)
                Return (pHround / 100.0R)

            End Function
            Public Function Get_PIs(seqs As List(Of Bio.ISequence)) As List(Of Double)
                Dim out As New List(Of Double)
                For Each S In seqs
                    out.Add(Get_PI(S))
                Next
                Return out
            End Function
            Private Function getpI(ByVal AApI_n As Dictionary(Of String, Double),
                                   ByVal AApI_c As Dictionary(Of String, Double),
                                   ByVal AApI_side As Dictionary(Of String, Double),
                                   ByVal PH As Double) As Double
                Dim sideAA As String
                Dim pHpK As Double
                Dim FoRmU As Double = 0.0R
                If Me.seq.Length = 0 Then Return Nothing
                Dim ntermAA As String = Me.seq.Chars(0).ToString()
                If ntermAA = "*" Then ntermAA =
                pHpK = PH - Convert.ToDouble(AApI_n(ntermAA).ToString())
                FoRmU += 1.0R / (1.0R + System.Math.Pow(10.0R, pHpK))
                If Me.seq.Contains("X") Then Return Double.NaN

                Dim cterm As String = Me.seq.Chars(Me.seq.Length() - 1).ToString()
                pHpK = Convert.ToDouble(AApI_c(cterm).ToString()) - PH
                FoRmU += -1.0R / (1.0R + System.Math.Pow(10.0R, pHpK))

                For t As Integer = 0 To Me.seq.Length() - 1
                    sideAA = Me.seq.Chars(t).ToString()
                    If AApI_side.ContainsKey(sideAA) Then
                        Dim value As Double = Convert.ToDouble(AApI_side(sideAA).ToString())
                        If value < 0.0R Then
                            pHpK = PH + value
                            FoRmU += 1.0R / (1.0R + System.Math.Pow(10.0R, pHpK))
                        Else
                            pHpK = value - PH
                            FoRmU += -1.0R / (1.0R + System.Math.Pow(10.0R, pHpK))
                        End If
                    End If
                Next t
                Return FoRmU
            End Function

        End Class

    End Namespace
End Namespace
