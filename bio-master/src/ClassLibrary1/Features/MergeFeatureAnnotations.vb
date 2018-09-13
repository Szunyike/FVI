Imports ClassLibrary1.Szunyi.Features.FeatureManipulation

Namespace Szunyi
    Namespace Features
        Public Class MergeFeatureAnnotation
            Private featuresFrom As List(Of String)
            Private featuresTo As List(Of String)
            Public Property Type As String = Szunyi.Constants.BackGroundWork.ModyfiedSequence
            Public Property Result As Szunyi.ListOf.SequenceList
            Public Property SeqLists As List(Of Szunyi.ListOf.SequenceList)

            Public Sub New(mergedSeqList As List(Of Szunyi.ListOf.SequenceList),
                       featuresTo As List(Of String),
                       featuresFrom As List(Of String))
                Me.SeqLists = mergedSeqList
                Me.featuresTo = featuresTo
                Me.featuresFrom = featuresFrom
            End Sub
            Public Sub DoIt()
                ' Get the two ExtFeatureList

                Dim cSearchSetting = New SettingForSearchInFeaturesAndQulifiers(featuresTo)

                Dim t As New Szunyi.ListOf.ExtFeatureList(cSearchSetting, Me.SeqLists)
                t.DoIt()

                Dim cSearchSetting2 = New SettingForSearchInFeaturesAndQulifiers(featuresFrom)
                Dim t2 As New ListOf.ExtFeatureList(cSearchSetting2, SeqLists)
                t2.DoIt()

                For Each Feat In t.FetauresByLocustag
                    MergeFeatures.FindAndMerge2Features(Feat, t2)
                Next

            End Sub
        End Class

    End Namespace
End Namespace
