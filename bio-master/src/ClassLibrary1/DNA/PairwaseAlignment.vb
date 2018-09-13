Imports Bio.IO.GenBank

Namespace Szunyi
    Namespace DNA
        Public Class Frame
            Public Shared Function GetFirtNucleotideOfCodon(pos As Integer, feat As Bio.IO.GenBank.FeatureItem) As ILocation
                If feat.Location.IsComplementer = False Then

                    Dim frame = GetFrame(pos, feat)
                    Select Case frame
                        Case 1
                            Return Szunyi.Location.Common.GetLocation(pos - 1, pos, "+")
                        Case 2
                            Return Szunyi.Location.Common.GetLocation(pos - 2, pos - 1, "+") ' pos - 1
                        Case 3
                            Return Szunyi.Location.Common.GetLocation(pos - 3, pos - 2, "+") ' pos - 2
                    End Select
                Else

                    Dim frame = GetFrame(pos, feat) ' Loci end
                    Select Case frame
                        Case 1
                            Return Szunyi.Location.Common.GetLocation(pos - 2, pos - 1, "-")' pos
                        Case 2
                            Return Szunyi.Location.Common.GetLocation(pos - 1, pos, "-") 'pos + 1 ' <
                        Case 3
                            Return Szunyi.Location.Common.GetLocation(pos, pos + 1, "-") ' pos + 2 ' <
                    End Select
                End If
            End Function
            Public Shared Function Get_Codons(SNP_Location As ILocation, feat As FeatureItem, Seq As Bio.ISequence) As List(Of Szunyi.DNA.Variants.Codon)
                Dim codons As New List(Of Szunyi.DNA.Variants.Codon)
                If feat.Location.IsComplementer = False Then
                    For i1 = SNP_Location.LocationStart To SNP_Location.LocationEnd - 1
                        Dim l = Szunyi.Location.Common.GetLocation(i1, i1, "+")
                        codons.Add(Get_Codon(l, feat, Seq))
                    Next
                Else
                    For i1 = SNP_Location.LocationEnd To SNP_Location.LocationStart + 1 Step -1
                        Dim l = Szunyi.Location.Common.GetLocation(i1, i1, "-")
                        codons.Add(Get_Codon(l, feat, Seq))
                    Next
                End If
                Dim RealCodons = From x In codons Where IsNothing(x) = False
                Dim res = From x In RealCodons Group By x.Loci.LocationStart Into Group
                Dim out As New List(Of Szunyi.DNA.Variants.Codon)
                For Each r In res
                    out.Add(r.Group.First)
                Next
                Return out
            End Function
            Public Shared Function Get_Codon(SNP_Location As ILocation, feat As FeatureItem, Seq As Bio.ISequence) As Szunyi.DNA.Variants.Codon
                Dim l As ILocation
                Dim FirstNucle As ILocation

                Dim frame As Integer
                If feat.Location.IsComplementer = False Then
                    FirstNucle = GetFirtNucleotideOfCodon(SNP_Location.LocationStart, feat)
                    l = Szunyi.Location.Common.GetLocation(FirstNucle.LocationStart, FirstNucle.LocationStart + 3, "+")
                    frame = GetFrame(SNP_Location.LocationStart, feat)
                    Dim alfy As Int16 = 54
                Else

                    FirstNucle = GetFirtNucleotideOfCodon(SNP_Location.LocationEnd, feat)
                    If IsNothing(FirstNucle) = False Then
                        l = Szunyi.Location.Common.GetLocation(FirstNucle.LocationEnd - 3, FirstNucle.LocationEnd, "-")
                        frame = GetFrame(SNP_Location.LocationEnd, feat)
                        Dim alfy As Int16 = 54
                    End If
                End If
                If IsNothing(l) = False Then
                    Dim tmpSeq = Szunyi.Sequences.SequenceManipulation.Get_Sub_Sequence.Whole(Seq, l)
                    Dim AASeq = Szunyi.DNA.Translate.Translate(tmpSeq)
                    Dim AA = Szunyi.DNA.Translate.To_String(tmpSeq)
                    Dim t As New DNA.Variants.Codon(tmpSeq, AASeq, l, frame, feat)
                    Dim alf As Int16 = 45
                    Return t
                Else
                    Return Nothing

                End If

            End Function
            ''' <summary>
            ''' Return the frame 1,2,3,-1,-2,-3
            ''' </summary>
            ''' <param name="pos"></param>
            ''' <param name="Feat"></param>
            ''' <returns></returns>
            Public Shared Function GetFrame(pos As Integer, Feat As Bio.IO.GenBank.FeatureItem) As Integer
                If Feat.Location.IsComplementer = False Then
                    Dim d = pos - Feat.Location.LocationStart
                    Dim f = d Mod 3
                    Select Case f
                        Case 0
                            Return 1
                        Case 1
                            Return 2
                        Case 2
                            Return 3
                    End Select
                Else
                    Dim d = Feat.Location.LocationEnd + 1 - pos
                    Dim f = d Mod 3
                    Select Case f
                        Case 0
                            Return 1 'ggod
                        Case 1
                            Return 2
                        Case 2
                            Return 3
                    End Select
                End If
                Return 0
            End Function
        End Class

    End Namespace
End Namespace

