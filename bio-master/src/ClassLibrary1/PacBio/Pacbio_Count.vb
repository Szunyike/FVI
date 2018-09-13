Imports System.IO
Imports Bio.IO.GenBank
Namespace Szunyi
    Namespace RNASeq.Count
        Public Class BLsWithFeatures_Sort_Accession
            Implements IComparer(Of BLsWithFeatures)

            Public Function Compare(x As BLsWithFeatures, y As BLsWithFeatures) As Integer Implements IComparer(Of BLsWithFeatures).Compare
                If x.BL.Location.Accession = y.BL.Location.Accession Then
                    Return x.BL.Location.LocationStart.CompareTo(y.BL.Location.LocationStart)
                Else
                    Return x.BL.Location.Accession.CompareTo(y.BL.Location.Accession)
                End If

            End Function
        End Class
        Public Class Counter
            Public Property Seq As Bio.ISequence

            Public Property Results As New List(Of BLsWithFeatures)
            Public Property Reads As New List(Of FeatureItem)
            Public Property Bam_Files As New List(Of FileInfo)
            Public Property Seqs As New List(Of Bio.ISequence)
            Public Property Feats As List(Of FeatureItem)
            Public Property Settings As Szunyi.BAM.Settings.Count_As
            Public Property BLs As List(Of Szunyi.Location.Basic_Location)
            Private ByAccession As New BLsWithFeatures_Sort_Accession

            Public Property mRNAwReads As New Dictionary(Of FeatureItem, List(Of Szunyi.Location.Basic_Location))

            Public Sub New(Seqs As List(Of Bio.ISequence), x As Szunyi.BAM.Settings.Count_As, BLs As List(Of Szunyi.Location.Basic_Location), Features As List(Of FeatureItem))
                Me.Seqs = Seqs
                Me.BLs = BLs
                For Each BL In Me.BLs
                    Results.Add(New BLsWithFeatures(BL, New List(Of FeatureItem)))
                Next
                Results.Sort(ByAccession)
                Me.Settings = x
                Feats = Features
            End Sub
            Public Sub New(Seq As Bio.ISequence, x As Szunyi.BAM.Settings.Count_As, BLs As List(Of Szunyi.Location.Basic_Location), Features As List(Of FeatureItem))
                Me.Seqs.Add(Seq)
                Me.BLs = BLs
                For Each BL In Me.BLs
                    Results.Add(New BLsWithFeatures(BL, New List(Of FeatureItem)))
                Next
                Results.Sort(ByAccession)
                Me.Settings = x
                Feats = Features
            End Sub
            Public Function DoIt(SeeOrientation As Boolean) As List(Of BLsWithFeatures)

                Dim log As New System.Text.StringBuilder()


                Dim x As New Szunyi.Location.Finding.TSS_PAS(Me.BLs, True, False)
                Dim NotFound As Integer = 0
                For Each Feat In Me.Feats
                    Dim F_BL = Szunyi.Location.Basic_Location_Manipulation.Get_Basic_Locations_From_Feat(Feat)
                    Dim LocisII = x.Get_BLs(F_BL, Settings.TSS_5.Default_Value, Settings.TSS_3.Default_Value, Settings.PAS_5.Default_Value, Settings.PAS_3.Default_Value)
                    For Each Item In LocisII
                        Dim l As New BLsWithFeatures(Item, New List(Of FeatureItem))
                        Dim Index = Me.Results.BinarySearch(l, ByAccession)
                        Me.Results(Index).Features.Add(Feat)
                    Next
                Next

                For Each item In Me.Results
                    item.Features = (From x4 In item.Features Order By x4.Location.LocationEnd - x4.Location.LocationStart Descending).ToList
                Next

                Select Case Me.Settings.Type.Selected_Value
                    Case Szunyi.Util_Helpers.Get_Enum_Name(Of Szunyi.mRNA.Transcript.CountBy)(Szunyi.mRNA.Transcript.CountBy.All)
                        mRNAwReads = Get_Detailed_Count_By_All()
                    Case Szunyi.Util_Helpers.Get_Enum_Name(Of Szunyi.mRNA.Transcript.CountBy)(Szunyi.mRNA.Transcript.CountBy.Smallest)
                        mRNAwReads = Get_Detailed_Count_By_Smallest()
                    Case Szunyi.Util_Helpers.Get_Enum_Name(Of Szunyi.mRNA.Transcript.CountBy)(Szunyi.mRNA.Transcript.CountBy.Unique)
                        mRNAwReads = Get_Detailed_Counts_By_Only_Uniqe()

                End Select

                Return Me.Results
            End Function
            Private Function Get_Detailed_Count_By_Smallest() As Dictionary(Of FeatureItem, List(Of Szunyi.Location.Basic_Location))
                Dim res As New Dictionary(Of FeatureItem, List(Of Szunyi.Location.Basic_Location))

                Dim Goods = From x In Me.Results Where x.Features.Count > 0

                For Each Item In Goods
                    If res.ContainsKey(Item.Features.Last) = False Then res.Add(Item.Features.Last, New List(Of Szunyi.Location.Basic_Location))
                    res(Item.Features.Last).Add(Item.BL)
                Next

                Return res

            End Function
            Private Function Get_Detailed_Count_By_All() As Dictionary(Of FeatureItem, List(Of Szunyi.Location.Basic_Location))
                Dim res As New Dictionary(Of FeatureItem, List(Of Szunyi.Location.Basic_Location))

                Dim Goods = From x In Me.Results Where x.Features.Count > 0


                For Each Item In Goods
                    For Each sItem In Item.Features
                        If res.ContainsKey(sItem) = False Then res.Add(sItem, New List(Of Szunyi.Location.Basic_Location))
                        res(sItem).Add(Item.BL)
                    Next
                Next
                Return res

            End Function
            Private Function Get_Detailed_Counts_By_Only_Uniqe() As Dictionary(Of FeatureItem, List(Of Szunyi.Location.Basic_Location))
                Dim res As New Dictionary(Of FeatureItem, List(Of Szunyi.Location.Basic_Location))

                Dim Reads_with_One_Feature = From x In Me.Results Where x.Features.Count = 1

                Dim Reads_with_More_Features = From x In Me.Results Where x.Features.Count > 1

                Dim Reads_without_Any_Features = From x In Me.Results Where x.Features.Count = 0

                For Each Item In Reads_with_One_Feature
                    If res.ContainsKey(Item.Features.First) = False Then res.Add(Item.Features.First, New List(Of Szunyi.Location.Basic_Location))
                    res(Item.Features.First).Add(Item.BL)
                Next
                Dim FoundFeatures = res.Keys.ToList


                Return res
            End Function
        End Class
        Public Class BLsWithFeatures
            Private found_mRNAs As IEnumerable(Of FeatureItem)

            Public Sub New(BL As Szunyi.Location.Basic_Location, found_mRNAs As IEnumerable(Of FeatureItem))
                Me.BL = BL
                Dim res = From x In found_mRNAs Order By x.Location.LocationEnd - x.Location.LocationStart Descending
                Me.Features.AddRange(res)
            End Sub

            Public Property BL As Szunyi.Location.Basic_Location
            Public Property Features As New List(Of FeatureItem)

        End Class
    End Namespace
    Namespace PacBio
        Public Class Normalisation
            Public Shared Property a As Double = 0.001288287
            Public Shared Property b As Double = 0.008581223
            Public Shared Property c As Double = 4.052790325
            Public Shared Property d As Double = 0.003154719
            Public Shared Property e As Double = 2.195216193
            Public Shared Function Norm_it(Lengths As List(Of Integer)) As Double
                Dim d As Double = 0
                For Each l In Lengths
                    d += Norm_It(l)
                Next
                Return d
            End Function
            Public Shared Function Norm_It(Length As Double) As Double

                Dim t = System.Math.Pow(b * Length, c) * a

                Dim t2 = System.Math.Pow(e, -d * Length)

                Return 1 / (t * t2)
            End Function
        End Class
    End Namespace
End Namespace

