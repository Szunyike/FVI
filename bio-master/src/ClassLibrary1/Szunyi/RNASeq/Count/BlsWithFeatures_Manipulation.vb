Imports Bio.IO.GenBank
Imports ClassLibrary1.Szunyi.mRNA.Transcript

Namespace Szunyi.RNASeq.Count
    Friend Class BlsWithFeatures_Manipulation
        Private c10 As Counter
        Private countAs As CountBy
        Public Property Results As List(Of BLsWithFeatures)
        Public Property Type As Szunyi.BAM.Settings .Count_As 
        Public Property Out As New Dictionary(Of FeatureItem, Integer)
        Public Property Counts As Counter
        Public Property mRNAwReads As Dictionary(Of FeatureItem, List(Of Szunyi.Location.Basic_Location))
        Public Sub New(c10 As Counter, Type As Szunyi.BAM.Settings.Count_As)
            Me.c10 = c10
            Me.countAs = countAs
            Me.Results = c10.Results
            Me.Type = Type
            Me.Counts = c10

            Select Case Type.Type.Selected_Value
                Case Szunyi.Util_Helpers.Get_Enum_Name(Of Szunyi.mRNA.Transcript.CountBy)(CountBy.All)
                    mRNAwReads = Get_Detailed_Count_By_Smallest()
                Case Szunyi.Util_Helpers.Get_Enum_Name(Of Szunyi.mRNA.Transcript.CountBy)(CountBy.Smallest)
                    mRNAwReads = Get_Detailed_Count_By_Smallest()
                Case Szunyi.Util_Helpers.Get_Enum_Name(Of Szunyi.mRNA.Transcript.CountBy)(CountBy.Unique)
                    mRNAwReads = Get_Detailed_Counts_By_Only_Uniqe()

            End Select


        End Sub

        Private Function Get_Detailed_Count_By_Smallest() As Dictionary(Of FeatureItem, List(Of Szunyi.Location.Basic_Location))
            Dim res As New Dictionary(Of FeatureItem, List(Of Szunyi.Location.Basic_Location))
            Dim unique = From x In Me.Results Where x.Features.Count = 1

            Dim Goods = From x In Me.Results Where x.Features.Count > 0

            Dim NotFoundReds = From x In Me.Results Where x.Features.Count = 0

            For Each Item In Goods
                If res.ContainsKey(Item.Features.Last) = False Then res.Add(Item.Features.Last, New List(Of Szunyi.Location.Basic_Location))
                res(Item.Features.Last).Add(Item.BL)
            Next
            Dim FoundFeatures = res.Keys.ToList
            Dim NotFOunds = Me.Counts.Feats.Except(FoundFeatures)
            For Each Item In NotFOunds
                res.Add(Item, New List(Of Szunyi.Location.Basic_Location))
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
            Dim NotFounds = Me.Counts.Feats.Except(FoundFeatures)
            For Each Item In NotFounds
                res.Add(Item, New List(Of Szunyi.Location.Basic_Location))
            Next

            For Each Read In Reads_with_More_Features
                Dim j = Read.Features
                '          Dim locis = Szunyi.Location.Common.GetLocationString(Read.Features)
                Dim kkkk As Int16 = 54
                '       Dim Feature = Get_Goods_from_More(Read)
                '        If IsNothing(Feature) = False Then
                '       If res.ContainsKey(Feature) = False Then res.Add(Feature, New List(Of Szunyi.Location.Basic_Location))
                '       res(Feature).Add(Read.BL)
                '       End If
            Next
            Return res
        End Function
        Private Function Get_Goods_from_More(Read As BLsWithFeatures) As FeatureItem
            Dim out = Get_with_Same_Introns(Read)
            If out.Count = 1 Then
                Return out.First
            Else
                Return Nothing
            End If


        End Function

        Private Function Get_with_Same_Introns(h As BLsWithFeatures) As List(Of FeatureItem)
            Dim f = h.BL.Location
            Dim IntronsOfReads = Szunyi.Features.FeatureManipulation.GetLocations.GetIntronLocations(f)
            Dim AllLocis As New List(Of String)
            Dim out As New List(Of FeatureItem)
            '   AllLocis.Add(Szunyi.Location.Common.GetLocationString(h.Read))
            For Each theRNA In h.Features
                AllLocis.Add(Szunyi.Location.Common.GetLocationString(theRNA))
                Dim IntronsOftheRNA = Szunyi.Features.FeatureManipulation.GetLocations.GetIntronLocations(theRNA)
                Dim Good As Boolean = True
                If IntronsOftheRNA.Count = IntronsOfReads.Count Then
                    For i1 = 0 To IntronsOftheRNA.Count - 1
                        Dim cExonOftheRNA = IntronsOftheRNA(i1)
                        Dim cExonOfRead = IntronsOfReads(i1)
                        If System.Math.Abs(cExonOftheRNA.LocationStart - cExonOfRead.LocationStart) > 10 Or
                                System.Math.Abs(cExonOftheRNA.LocationEnd - cExonOfRead.LocationEnd) > 10 Then
                            Good = False
                            Exit For
                        End If

                    Next
                    If Good = True Then
                        out.Add(theRNA)
                    End If
                End If
            Next
            Return out
        End Function


    End Class
End Namespace
