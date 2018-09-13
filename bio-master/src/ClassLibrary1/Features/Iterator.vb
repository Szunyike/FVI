Imports Bio.IO.GenBank

Namespace Szunyi
    Namespace Features
        Namespace FeatureManipulation
            Public Class Iterator
                Public Shared Function Get_All_Types(Features As List(Of FeatureItem)) As List(Of String)
                    Dim out As New List(Of String)
                    For Each sg In ByType(Features)
                        out.Add(sg.First.Key)
                    Next
                    Return out
                End Function
                Public Shared Iterator Function ByType(features As List(Of FeatureItem)) As IEnumerable(Of List(Of FeatureItem))
                    Dim x = From t In features Group By t.Key Into Group

                    For Each gr In x
                        Yield gr.Group.ToList

                    Next
                End Function
#Region "Location orientation is indifferent"
                ''' <summary>
                ''' Only Location start, orientation is indifferent
                ''' </summary>
                ''' <param name="features"></param>
                ''' <returns></returns>
                Public Shared Iterator Function ByLocationStart(features As List(Of FeatureItem)) As IEnumerable(Of List(Of FeatureItem))
                    Dim x = From t In features Group By t.Location.LocationStart Into Group

                    For Each gr In x
                        Yield gr.Group.ToList

                    Next
                End Function
                ''' <summary>
                ''' Only Location End, orientation is indifferent
                ''' </summary>
                ''' <param name="features"></param>
                ''' <returns></returns>
                Public Shared Iterator Function ByLocationEnd(features As List(Of FeatureItem)) As IEnumerable(Of List(Of FeatureItem))
                    Dim x = From t In features Group By t.Location.LocationEnd Into Group

                    For Each gr In x
                        Yield gr.Group.ToList

                    Next
                End Function
                ''' <summary>
                ''' Only Location start, end, orientation is indifferent
                ''' </summary>
                ''' <param name="features"></param>
                ''' <returns></returns>
                Public Shared Iterator Function ByLocation(features As List(Of FeatureItem)) As IEnumerable(Of List(Of FeatureItem))
                    Dim x = From t In features Group By t.Location.LocationStart, t.Location.LocationEnd Into Group

                    For Each gr In x
                        Yield gr.Group.ToList

                    Next
                End Function
                ''' <summary>
                ''' Only TSS, orientation is indifferent
                ''' </summary>
                ''' <param name="features"></param>
                ''' <returns></returns>
                Public Shared Iterator Function By_TSS(features As List(Of FeatureItem)) As IEnumerable(Of List(Of FeatureItem))
                    Dim x = From t In features Group By t.Location.TSS Into Group

                    For Each gr In x
                        Yield gr.Group.ToList

                    Next
                End Function
                ''' <summary>
                ''' Only PAS, orientation is indifferent
                ''' </summary>
                ''' <param name="features"></param>
                ''' <returns></returns>
                Public Shared Iterator Function By_PAS(features As List(Of FeatureItem)) As IEnumerable(Of List(Of FeatureItem))
                    Dim x = From t In features Group By t.Location.PAS Into Group

                    For Each gr In x
                        Yield gr.Group.ToList

                    Next
                End Function
#End Region
#Region "Orientation is important"
                ''' <summary>
                ''' Only Location start, orientation is important
                ''' </summary>
                ''' <param name="features"></param>
                ''' <returns></returns>
                Public Shared Iterator Function ByLocationStart_wOrientation(features As List(Of FeatureItem)) As IEnumerable(Of List(Of FeatureItem))
                    Dim x = From t In features Group By t.Location.LocationStart, t.Location.IsComplementer Into Group

                    For Each gr In x
                        Yield gr.Group.ToList

                    Next
                End Function
                ''' <summary>
                ''' Only Location End, orientation is important
                ''' </summary>
                ''' <param name="features"></param>
                ''' <returns></returns>
                Public Shared Iterator Function ByLocationEnd_wOrientation(features As List(Of FeatureItem)) As IEnumerable(Of List(Of FeatureItem))
                    Dim x = From t In features Group By t.Location.LocationEnd, t.Location.IsComplementer Into Group

                    For Each gr In x
                        Yield gr.Group.ToList

                    Next
                End Function
                ''' <summary>
                ''' Only Location start, end, orientation is important
                ''' </summary>
                ''' <param name="features"></param>
                ''' <returns></returns>
                Public Shared Iterator Function ByLocation_wOrientation(features As List(Of FeatureItem)) As IEnumerable(Of List(Of FeatureItem))
                    Dim x = From t In features Group By t.Location.LocationStart, t.Location.LocationEnd, t.Location.IsComplementer Into Group

                    For Each gr In x
                        Yield gr.Group.ToList

                    Next
                End Function
                ''' <summary>
                ''' Only TSS, orientation is important
                ''' </summary>
                ''' <param name="features"></param>
                ''' <returns></returns>
                Public Shared Iterator Function By_TSS_wOrientation(features As List(Of FeatureItem)) As IEnumerable(Of List(Of FeatureItem))
                    Dim x = From t In features Group By t.Location.TSS, t.Location.IsComplementer Into Group

                    For Each gr In x
                        Yield gr.Group.ToList

                    Next
                End Function
                ''' <summary>
                ''' Only TSS, orientation is important
                ''' </summary>
                ''' <param name="features"></param>
                ''' <returns></returns>
                Public Shared Iterator Function By_TSS_wOrientation_Ordered(features As List(Of FeatureItem)) As IEnumerable(Of List(Of FeatureItem))
                    Dim x = From t In features Group By t.Location.TSS, t.Location.IsComplementer Into Group

                    For Each gr In x
                        If gr.Group.First.Location.IsComplementer = True Then
                            Yield (From x1 In gr.Group Order By x1.Location.TSS Descending).ToList
                        Else
                            Yield (From x1 In gr.Group Order By x1.Location.TSS Ascending).ToList
                        End If


                    Next
                End Function
                ''' <summary>
                ''' Only PAS, orientation is important
                ''' </summary>
                ''' <param name="features"></param>
                ''' <returns></returns>
                Public Shared Iterator Function By_PAS_wOrientation(features As List(Of FeatureItem)) As IEnumerable(Of List(Of FeatureItem))
                    Dim x = From t In features Group By t.Location.PAS, t.Location.IsComplementer Into Group

                    For Each gr In x
                        Yield gr.Group.ToList

                    Next
                End Function

                Public Shared Iterator Function By_Label(Feats As List(Of FeatureItem)) As IEnumerable(Of List(Of FeatureItem))
                    Dim res = From x In Feats Group By x.Label Into Group

                    For Each gr In res
                        Yield gr.Group.ToList
                    Next
                End Function
#End Region
            End Class

            Public Class Sort
                Public Shared Function By_TSS_wOrientation(Feats As List(Of FeatureItem)) As List(Of FeatureItem)
                    If Feats.First.Location.IsComplementer = False Then
                        Return (From x In Feats Order By x.Location.TSS Ascending).ToList
                    Else
                        Return (From x In Feats Order By x.Location.TSS Descending).ToList
                    End If
                End Function
                Public Shared Function By_PAS_wOrientation(Feats As List(Of FeatureItem)) As List(Of FeatureItem)
                    If Feats.First.Location.IsComplementer = False Then
                        Return (From x In Feats Order By x.Location.PAS Ascending).ToList
                    Else
                        Return (From x In Feats Order By x.Location.PAS Descending).ToList
                    End If
                End Function
                Public Shared Function By_LocationStart_wOrientation(Feats As List(Of FeatureItem)) As List(Of FeatureItem)
                    If Feats.First.Location.IsComplementer = False Then
                        Return (From x In Feats Order By x.Location.LocationStart Ascending).ToList
                    Else
                        Return (From x In Feats Order By x.Location.LocationStart Descending).ToList
                    End If
                End Function
                Public Shared Function By_LocationEnd_wOrientation(Feats As List(Of FeatureItem)) As List(Of FeatureItem)
                    If Feats.First.Location.IsComplementer = False Then
                        Return (From x In Feats Order By x.Location.LocationEnd Ascending).ToList
                    Else
                        Return (From x In Feats Order By x.Location.LocationEnd Descending).ToList
                    End If
                End Function
            End Class
        End Namespace

    End Namespace
End Namespace

