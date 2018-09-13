
Imports Bio
Imports ClassLibrary1.Szunyi.Features.FeatureManipulation

Namespace Szunyi
    Namespace Features
        Public Class ReName
            Public Class Rename
                Private SubType As String
                Private mergedSeqs As List(Of Bio.ISequence)
                Private selectedFeatures As List(Of String)
                Private selectedQualifiers As List(Of String)
                Private separator As String
                Private Name As String
                Public Property Type As String = Szunyi.Constants.BackGroundWork.ModyfiedSequence
                Public Property SeqList As Szunyi.ListOf.SequenceList

                Public Sub New(mergedSeqs As List(Of ISequence),
                           selectedFeatures As List(Of String),
                           selectedQualifiers As List(Of String),
                           separator As String,
                           SubType As String,
                           Name As String)
                    Me.mergedSeqs = mergedSeqs
                    Me.selectedFeatures = selectedFeatures
                    Me.selectedQualifiers = selectedQualifiers
                    Me.separator = separator
                    Me.SubType = SubType
                    Dim s = InputBox("Enter the Title")
                    Me.SeqList = New Szunyi.ListOf.SequenceList(mergedSeqs, s, "RN Features: " & Name)

                End Sub
                Public Sub DoIt()
                    Dim Features = GetFeaturesByType.GetFeturesByTypesFromSeqs(mergedSeqs, selectedFeatures)
                    Select Case Me.SubType
                        Case Szunyi.Constants.StringRename.FirstAfterSplit
                            For Each Feat In Features
                                For Each Qual In selectedQualifiers
                                    If Feat.Qualifiers.ContainsKey(Qual) AndAlso Feat.Qualifiers(Qual).Count > 0 Then
                                        Dim x As New List(Of String)
                                        x.Add(Split(Feat.Qualifiers(Qual).First, Me.separator).First)
                                        Feat.Qualifiers(Qual) = x
                                    End If
                                Next
                            Next
                        Case Szunyi.Constants.StringRename.LastAfterSplit
                            For Each Feat In Features
                                For Each Qual In selectedQualifiers
                                    If Feat.Qualifiers.ContainsKey(Qual) AndAlso Feat.Qualifiers(Qual).Count > 0 Then
                                        Dim x As New List(Of String)
                                        x.Add(Split(Feat.Qualifiers(Qual).First, Me.separator).Last)
                                        Feat.Qualifiers(Qual) = x
                                    End If
                                Next
                            Next
                    End Select

                End Sub
            End Class
        End Class
    End Namespace
End Namespace
