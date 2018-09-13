Imports Bio
Imports Bio.IO.GenBank

Namespace Szunyi
    Namespace DNA
        Public Class PA
            Public Shared PolyA_Signal_Motifs_All As String() = Split("AATGAA,AATCAA,AATTAA,AACAAA,AAAAAA,AATAAT,AATAAC,AATAAG,AAAAAG,AAGAAA,AATAAA,AATACA,AATAGA,AATATA,ACTAAA,AGTAAA,ATTAAA,CATAAA,GATAAA,TATAAA", ",")
            'Public Shared polyA_Motifs_Main As String() = Split("AAAAAG,AAGAAA,AATAAA,AATACA,AATAGA,AATATA,ACTAAA,AGTAAA,ATTAAA,CATAAA,GATAAA,TATAAA", ",")
            'Public Shared PolyA_Motifs_Main_rev As String() = Split("CTTTTT,TTTCTT,TTTATT,TGTATT,TCTATT,TATATT,TTTAGT,TTTACT,TTTAAT,TTTATG,TTTATC,TTTATA", ",")
            ' Public Shared polyA_Motifs As String() = Split("AATGAA,AATCAA,AATTAA,AACAAA,AAAAAA,AATAAT,AATAAC,AATAAG", ",")
            '    Public Shared PolyA_Motifs_rev As String() = Split("TTCATT,TTGCTT,TTAATT,TTTGTT,TTTTTT,ATTATT,GTTATT,CTTATT", ",")
            Public Shared Function Get_PolyA_Signals(Seq As Bio.Sequence,
                                                     Locis As List(Of Szunyi.Location.Basic_Location),
                                                     length As Integer,
                                                     Best_Position As Integer) As List(Of PolyA_Signal_Sites)
                Dim out As New List(Of PolyA_Signal_Sites)
                For Each Loci In Locis
                    Dim The_Seq = Szunyi.Sequences.SequenceManipulation.Get_Sub_Sequence.Before(Seq, Loci, Szunyi.Constants.Sort_Locations_By.PAS, length)
                    Dim Motifs As List(Of PolyA_Signal_Site) = Get_Motifs(The_Seq, PolyA_Signal_Motifs_All, Best_Position)
                    Dim x As New PolyA_Signal_Sites(Loci, The_Seq, Motifs)
                    out.Add(x)
                Next
                Return out
            End Function
            Public Shared Function Get_PolyA_Signals(Seq As Bio.Sequence,
                                                     Feats As List(Of Bio.IO.GenBank.FeatureItem),
                                                     length As Integer,
                                                     Best_Position As Integer) As List(Of PolyA_Signal_Sites)
                Dim out As New List(Of PolyA_Signal_Sites)
                For Each Loci In Feats
                    Dim The_Seq = Szunyi.Sequences.SequenceManipulation.Get_Sub_Sequence.Before(Seq, Loci.Location, Szunyi.Constants.Sort_Locations_By.PAS, length)
                    Dim Motifs As List(Of PolyA_Signal_Site) = Get_Motifs(The_Seq, PolyA_Signal_Motifs_All, Best_Position)
                    Dim x As New PolyA_Signal_Sites(Loci.Location, The_Seq, Motifs)
                    out.Add(x)
                Next
                Return out
            End Function
            Public Shared Function Get_PolyA_Signal(Seq As Bio.ISequence,
                                                     Loci As Szunyi.Location.Basic_Location,
                                                     length As Integer,
                                                     Best_Position As Integer) As PolyA_Signal_Sites
                Dim out As New List(Of PolyA_Signal_Sites)
                Dim The_Seq = Szunyi.Sequences.SequenceManipulation.Get_Sub_Sequence.Before(Seq, Loci, Szunyi.Constants.Sort_Locations_By.PAS, length)
                Dim Motifs As List(Of PolyA_Signal_Site) = Get_Motifs(The_Seq, PolyA_Signal_Motifs_All, Best_Position)
                Dim x As New PolyA_Signal_Sites(Loci, The_Seq, Motifs)
                Return x


            End Function
            Public Shared Function Get_PolyA_Signal(Seq As Bio.ISequence,
                                                     Loci As ILocation,
                                                     length As Integer,
                                                     Best_Position As Integer) As PolyA_Signal_Sites
                Dim out As New List(Of PolyA_Signal_Sites)
                Dim The_Seq = Szunyi.Sequences.SequenceManipulation.Get_Sub_Sequence.Before(Seq, Loci, Szunyi.Constants.Sort_Locations_By.PAS, length)
                Dim Motifs As List(Of PolyA_Signal_Site) = Get_Motifs(The_Seq, PolyA_Signal_Motifs_All, Best_Position)
                Dim x As New PolyA_Signal_Sites(Loci, The_Seq, Motifs)
                Return x


            End Function
            Public Shared Function Get_PolyA_Signal(Seq As Bio.ISequence,
                                                     Feat As FeatureItem,
                                                     length As Integer,
                                                     Best_Position As Integer) As PolyA_Signal_Sites
                Dim out As New List(Of PolyA_Signal_Sites)
                Dim The_Seq = Szunyi.Sequences.SequenceManipulation.Get_Sub_Sequence.Before(Seq, Feat.Location, Szunyi.Constants.Sort_Locations_By.PAS, length)
                Dim Motifs As List(Of PolyA_Signal_Site) = Get_Motifs(The_Seq, PolyA_Signal_Motifs_All, Best_Position)
                Dim x As New PolyA_Signal_Sites(Feat.Location, The_Seq, Motifs)
                Return x


            End Function
            Private Shared Function Get_Motifs(seq As Sequence, Motifs() As String, best_Position As Integer) As List(Of PolyA_Signal_Site)
                Dim out As New List(Of PolyA_Signal_Site)
                For Each Motif In Motifs
                    Dim MC = Szunyi.Text.Regexp.Get_Motifs(seq, Motif)
                    If IsNothing(MC) = False Then
                        For I2 = 0 To MC.Count - 1
                            Dim Item = MC.Item(I2)
                            out.Add(New PolyA_Signal_Site(Item.Value, Item.Index, best_Position))
                        Next
                    End If
                Next
                If out.Count > 1 Then
                    Dim jk As Int16 = 545
                End If
                Return out
            End Function
            Public Shared Function Get_Poly_Signals_ToString(x As List(Of PolyA_Signal_Sites)) As List(Of String)
                Dim out As New List(Of String)
                For Each Item In x
                    out.Add(Item.Get_Best.ToString)
                Next
                Return out
            End Function


        End Class
        Public Class PolyA_Signal_Sites
            Public PolyA_Signal_Sites As New Dictionary(Of String, List(Of Integer))
            Public Property Basic_Location As Szunyi.Location.Basic_Location
            Public Property Signal_Sites As List(Of PolyA_Signal_Site)
            Public Property InvestigatedSequence As Bio.ISequence
            Public Function Get_Best() As PolyA_Signal_Site
                ' First AATAAA
                ' Second From All Pos
                Dim x = From a In Signal_Sites Where a.Sequence = "AATAAA"

                Select Case x.Count
                    Case = 0
                        Dim x1 = From a In Signal_Sites Order By a.Distance Ascending

                        If x1.Count > 0 Then Return x1.First

                        Return New PolyA_Signal_Site
                    Case 1
                        Return x.First
                    Case Else
                        Dim x1 = From a In x Order By a.Distance Ascending

                        Return x1.First
                End Select
            End Function
            Public Function Get_All()
                Dim x = From a In Me.Signal_Sites Order By a.Distance Ascending

                If x.Count > 0 Then
                    Return x.ToList
                Else
                    Return New List(Of PolyA_Signal_Site)
                End If
            End Function
            Public Sub New(loci As Szunyi.Location.Basic_Location, The_Seq As Bio.ISequence, PolyAs As List(Of PolyA_Signal_Site))
                Me.Basic_Location = loci
                Me.InvestigatedSequence = The_Seq
                Me.Signal_Sites = PolyAs
            End Sub
            Public Sub New(loci As ILocation, The_Seq As Bio.ISequence, PolyAs As List(Of PolyA_Signal_Site))
                Me.Basic_Location = Szunyi.Location.Basic_Location_Manipulation.Get_Basic_Location(loci)
                Me.InvestigatedSequence = The_Seq
                Me.Signal_Sites = PolyAs
            End Sub
        End Class
        Public Class PolyA_Signal_Site
            Public Property Sequence As String
            Public Property Position As Integer
            Public Property Distance As Integer
            Public Sub New()

            End Sub
            Public Sub New(Seq As String, Pos As Integer, Best_Position As Integer)
                Me.Sequence = Seq
                Me.Position = Pos
                Me.Distance = System.Math.Abs(Pos + Best_Position)
            End Sub
            Public Overrides Function ToString() As String
                Return Me.Sequence & vbTab & Me.Position & vbTab & Me.Distance
            End Function
        End Class
    End Namespace
End Namespace

