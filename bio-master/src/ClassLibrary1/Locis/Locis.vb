Imports Bio.IO.GenBank

Namespace Szunyi
    Namespace All_Locis
        Class Comp
            Class Bio_Location
                Public Class Location_ByStart
                    Implements IComparer(Of Bio.IO.GenBank.ILocation)
                    Public Function Compare(x As ILocation, y As ILocation) As Integer Implements IComparer(Of ILocation).Compare
                        Return x.LocationStart.CompareTo(y.LocationStart)
                    End Function
                End Class
                Public Class Location_ByStart_Operator
                    Implements IComparer(Of Bio.IO.GenBank.ILocation)
                    Public Function Compare(x As ILocation, y As ILocation) As Integer Implements IComparer(Of ILocation).Compare
                        If x.Operator <> y.Operator Then
                            Return x.Operator.CompareTo(y.Operator)
                        Else
                            Return x.LocationStart.CompareTo(y.LocationStart)
                        End If
                    End Function
                End Class
                Public Class Location_ByEnd
                    Implements IComparer(Of Bio.IO.GenBank.ILocation)
                    Public Function Compare(x As ILocation, y As ILocation) As Integer Implements IComparer(Of ILocation).Compare
                        Return x.LocationEnd.CompareTo(y.LocationEnd)
                    End Function
                End Class
                Public Class Location_ByEnd_Operator
                    Implements IComparer(Of Bio.IO.GenBank.ILocation)
                    Public Function Compare(x As ILocation, y As ILocation) As Integer Implements IComparer(Of ILocation).Compare
                        If x.Operator <> y.Operator Then
                            Return x.Operator.CompareTo(y.Operator)
                        Else
                            Return x.LocationEnd.CompareTo(y.LocationEnd)
                        End If
                    End Function
                End Class

                Public Class Location_ByStart_Contains
                    Implements IComparer(Of Bio.IO.GenBank.ILocation)

                    Public Function Compare(x As ILocation, y As ILocation) As Integer Implements IComparer(Of ILocation).Compare
                        If x.LocationStart >= y.LocationStart And x.LocationStart <= y.LocationEnd Then
                            Return 0
                        End If
                        If x.LocationStart <> y.LocationStart Then
                            Return x.LocationStart.CompareTo(y.LocationStart)
                        Else
                            Return x.LocationEnd.CompareTo(y.LocationEnd)
                        End If

                    End Function
                End Class
                Public Class Location_ByStart_Contains_Operator
                    Implements IComparer(Of Bio.IO.GenBank.ILocation)

                    Public Function Compare(x As ILocation, y As ILocation) As Integer Implements IComparer(Of ILocation).Compare
                        If x.Operator <> y.Operator Then Return x.Operator.CompareTo(y.Operator)
                        If x.LocationStart >= y.LocationStart And x.LocationStart <= y.LocationEnd Then
                            Return 0
                        End If
                        If x.LocationStart <> y.LocationStart Then
                            Return x.LocationStart.CompareTo(y.LocationStart)
                        Else
                            Return x.LocationEnd.CompareTo(y.LocationEnd)
                        End If

                    End Function
                End Class

                Public Class Location_ByEnd_Contains
                    Implements IComparer(Of Bio.IO.GenBank.ILocation)

                    Public Function Compare(x As ILocation, y As ILocation) As Integer Implements IComparer(Of ILocation).Compare
                        If x.LocationEnd >= y.LocationEnd And x.LocationEnd <= y.LocationEnd Then
                            Return 0
                        End If
                        If x.LocationEnd <> y.LocationEnd Then
                            Return x.LocationEnd.CompareTo(y.LocationEnd)
                        Else
                            Return x.LocationEnd.CompareTo(y.LocationEnd)
                        End If

                    End Function
                End Class
                Public Class Location_ByEnd_Contains_Operator
                    Implements IComparer(Of Bio.IO.GenBank.ILocation)

                    Public Function Compare(x As ILocation, y As ILocation) As Integer Implements IComparer(Of ILocation).Compare
                        If x.Operator <> y.Operator Then Return x.Operator.CompareTo(y.Operator)
                        If x.LocationEnd >= y.LocationEnd And x.LocationEnd <= y.LocationEnd Then
                            Return 0
                        End If
                        If x.LocationEnd <> y.LocationEnd Then
                            Return x.LocationEnd.CompareTo(y.LocationEnd)
                        Else
                            Return x.LocationEnd.CompareTo(y.LocationEnd)
                        End If

                    End Function
                End Class

                Public Class Location_Full_Contains
                    Implements IComparer(Of Bio.IO.GenBank.ILocation)

                    Public Function Compare(x As ILocation, y As ILocation) As Integer Implements IComparer(Of ILocation).Compare
                        If x.LocationStart <= y.LocationStart And x.LocationEnd >= y.LocationEnd Then
                            Return 0
                        End If
                        Return x.LocationStart.CompareTo(y.LocationStart)
                    End Function
                End Class
                Public Class Location_Full_Contains_Operator
                    Implements IComparer(Of Bio.IO.GenBank.ILocation)

                    Public Function Compare(x As ILocation, y As ILocation) As Integer Implements IComparer(Of ILocation).Compare
                        If x.Operator <> y.Operator Then
                            Return x.Operator.CompareTo(y.Operator)
                        ElseIf x.LocationStart <= y.LocationStart And x.LocationEnd >= y.LocationEnd Then
                            Return 0
                        Else
                            Return x.LocationStart.CompareTo(y.LocationStart)
                        End If

                    End Function

                End Class

            End Class
            Class Bio_Features
                Public Class Features_ByStart
                    Implements IComparer(Of Bio.IO.GenBank.FeatureItem)
                    Public Function Compare(x As FeatureItem, y As FeatureItem) As Integer Implements IComparer(Of FeatureItem).Compare
                        Return x.Location.LocationStart.CompareTo(y.Location.LocationStart)
                    End Function
                End Class
                Public Class Features_ByStart_Operator
                    Implements IComparer(Of Bio.IO.GenBank.FeatureItem)
                    Public Function Compare(x As FeatureItem, y As FeatureItem) As Integer Implements IComparer(Of FeatureItem).Compare
                        If x.Location.Operator <> y.Location.Operator Then
                            Return x.Location.Operator.CompareTo(y.Location.Operator)
                        Else
                            Return x.Location.LocationStart.CompareTo(y.Location.LocationStart)
                        End If

                    End Function
                End Class
                Public Class Features_ByEnd
                    Implements IComparer(Of Bio.IO.GenBank.FeatureItem)
                    Public Function Compare(x As FeatureItem, y As FeatureItem) As Integer Implements IComparer(Of FeatureItem).Compare
                        Return x.Location.LocationEnd.CompareTo(y.Location.LocationEnd)
                    End Function
                End Class
                Public Class Features_ByEnd_Operator
                    Implements IComparer(Of Bio.IO.GenBank.FeatureItem)
                    Public Function Compare(x As FeatureItem, y As FeatureItem) As Integer Implements IComparer(Of FeatureItem).Compare
                        If x.Location.Operator = y.Location.Operator Then
                            Return x.Location.LocationEnd.CompareTo(y.Location.LocationEnd)
                        Else
                            Return x.Location.Operator.CompareTo(y.Location.Operator)
                        End If

                    End Function
                End Class
                Public Class Features_ByStart_Contains
                    Implements IComparer(Of Bio.IO.GenBank.FeatureItem)

                    Public Function Compare(x As FeatureItem, y As FeatureItem) As Integer Implements IComparer(Of FeatureItem).Compare
                        If x.Location.LocationStart >= y.Location.LocationStart And x.Location.LocationStart <= y.Location.LocationEnd Then
                            Return 0
                        End If
                        If x.Location.LocationStart <> y.Location.LocationStart Then
                            Return x.Location.LocationStart.CompareTo(y.Location.LocationStart)
                        Else
                            Return x.Location.LocationEnd.CompareTo(y.Location.LocationEnd)
                        End If
                    End Function


                End Class
                Public Class Features_ByStart_Contains_Operator
                    Implements IComparer(Of Bio.IO.GenBank.FeatureItem)

                    Public Function Compare(x As FeatureItem, y As FeatureItem) As Integer Implements IComparer(Of FeatureItem).Compare
                        If x.Location.Operator <> y.Location.Operator Then Return x.Location.Operator.CompareTo(y.Location.Operator)

                        If x.Location.LocationStart >= y.Location.LocationStart And x.Location.LocationStart <= y.Location.LocationEnd Then
                            Return 0
                        End If
                        If x.Location.LocationStart <> y.Location.LocationStart Then
                            Return x.Location.LocationStart.CompareTo(y.Location.LocationStart)
                        Else
                            Return x.Location.LocationEnd.CompareTo(y.Location.LocationEnd)
                        End If
                    End Function


                End Class
                Public Class Features_ByEnd_Contains
                    Implements IComparer(Of Bio.IO.GenBank.FeatureItem)

                    Public Function Compare(x As FeatureItem, y As FeatureItem) As Integer Implements IComparer(Of FeatureItem).Compare
                        If x.Location.LocationEnd >= y.Location.LocationEnd And x.Location.LocationEnd <= y.Location.LocationEnd Then
                            Return 0
                        End If
                        If x.Location.LocationEnd <> y.Location.LocationEnd Then
                            Return x.Location.LocationEnd.CompareTo(y.Location.LocationEnd)
                        Else
                            Return x.Location.LocationEnd.CompareTo(y.Location.LocationEnd)
                        End If
                    End Function


                End Class
                Public Class Features_ByEnd_Contains_Operator
                    Implements IComparer(Of Bio.IO.GenBank.FeatureItem)

                    Public Function Compare(x As FeatureItem, y As FeatureItem) As Integer Implements IComparer(Of FeatureItem).Compare
                        If x.Location.Operator <> y.Location.Operator Then Return x.Location.Operator.CompareTo(y.Location.Operator)

                        If x.Location.LocationEnd >= y.Location.LocationEnd And x.Location.LocationEnd <= y.Location.LocationEnd Then
                            Return 0
                        End If
                        If x.Location.LocationEnd <> y.Location.LocationEnd Then
                            Return x.Location.LocationEnd.CompareTo(y.Location.LocationEnd)
                        Else
                            Return x.Location.LocationEnd.CompareTo(y.Location.LocationEnd)
                        End If
                    End Function


                End Class
                Public Class Features_Full_Contains
                    Implements IComparer(Of Bio.IO.GenBank.FeatureItem)

                    Public Function Compare(x As FeatureItem, y As FeatureItem) As Integer Implements IComparer(Of FeatureItem).Compare
                        If x.Location.LocationStart <= y.Location.LocationStart And x.Location.LocationEnd >= y.Location.LocationEnd Then
                            Return 0
                        End If
                        Return x.Location.LocationStart.CompareTo(y.Location.LocationStart)
                    End Function
                End Class
                Public Class Features_Full_Contains_Operator
                    Implements IComparer(Of Bio.IO.GenBank.FeatureItem)

                    Public Function Compare(x As FeatureItem, y As FeatureItem) As Integer Implements IComparer(Of FeatureItem).Compare
                        If x.Location.Operator <> y.Location.Operator Then
                            Return x.Location.Operator.CompareTo(y.Location.Operator)
                        ElseIf x.Location.LocationStart <= y.Location.LocationStart And x.Location.LocationEnd >= y.Location.LocationEnd Then
                            Return 0
                        Else
                            Return x.Location.LocationStart.CompareTo(y.Location.LocationStart)
                        End If
                    End Function
                End Class
            End Class
            Class Ready_Comparers
                Public Shared Property Bio_Location_ByStart As New Comp.Bio_Location.Location_ByStart
                Public Shared Property Bio_Location_ByStart_Operator As New Comp.Bio_Location.Location_ByStart_Operator
                Public Shared Property Bio_Location_ByEnd As New Comp.Bio_Location.Location_ByEnd
                Public Shared Property Bio_Location_ByEnd_Operator As New Comp.Bio_Location.Location_ByEnd_Operator
                Public Shared Property Bio_Location_ByStart_Contains As New Comp.Bio_Location.Location_ByEnd_Contains
                Public Shared Property Bio_Location_ByStart_Contains_Operator As New Comp.Bio_Location.Location_ByStart_Contains_Operator
                Public Shared Property Bio_Location_ByEnd_Contains As New Comp.Bio_Location.Location_ByEnd_Contains
                Public Shared Property Bio_Location_ByEnd_Contains_Operator As New Comp.Bio_Location.Location_ByEnd_Contains_Operator
                Public Shared Property Bio_Location_Full_Contains As New Comp.Bio_Location.Location_Full_Contains
                Public Shared Property Bio_Location_Full_Contains_Operator As New Comp.Bio_Location.Location_Full_Contains_Operator

                Public Shared Property Bio_Features_ByStart As New Comp.Bio_Features.Features_ByStart
                Public Shared Property Bio_Features_ByStart_Operator As New Comp.Bio_Features.Features_ByStart_Operator
                Public Shared Property Bio_Features_ByEnd As New Comp.Bio_Features.Features_ByEnd
                Public Shared Property Bio_Features_ByEnd_Operator As New Comp.Bio_Features.Features_ByEnd_Operator
                Public Shared Property Bio_Features_ByStart_Contains As New Comp.Bio_Features.Features_ByEnd_Contains
                Public Shared Property Bio_Features_ByStart_Contains_Operator As New Comp.Bio_Features.Features_ByStart_Contains_Operator
                Public Shared Property Bio_Features_ByEnd_Contains As New Comp.Bio_Features.Features_ByEnd_Contains
                Public Shared Property Bio_Features_ByEnd_Contains_Operator As New Comp.Bio_Features.Features_ByEnd_Contains_Operator
                Public Shared Property Bio_Features_Full_Contains As New Comp.Bio_Features.Features_Full_Contains
                Public Shared Property Bio_Features_Full_Contains_Operator As New Comp.Bio_Features.Features_Full_Contains_Operator

            End Class
        End Class
        Class FindFeatures
            Private Shared Function General(Feat As FeatureItem, Feats As List(Of FeatureItem), The_Comparer As Object) As List(Of FeatureItem)
                Dim Index = Feats.BinarySearch(Feat, The_Comparer)
                Dim out As New List(Of Bio.IO.GenBank.FeatureItem)
                If Index > -1 Then
                    For i1 = Index To 0 Step -1
                        If The_Comparer.Compare(Feat, Feats(Index)) = 0 Then
                            out.Add(Feats(i1))
                        Else
                            Exit For
                        End If
                    Next
                    For i1 = Index + 1 To Feats.Count - 1
                        If The_Comparer.Compare(Feat, Feats(Index)) = 0 Then
                            out.Add(Feats(i1))
                        Else Exit For
                        End If
                    Next
                End If

                Return out
            End Function
            Public Shared Function ByStart_Contains(Feat As FeatureItem, Feats As List(Of FeatureItem)) As List(Of FeatureItem)
                Dim The_Comparer = Comp.Ready_Comparers.Bio_Features_ByStart
                Return General(Feat, Feats, The_Comparer)


            End Function
            Public Shared Function ByEnd_Contains(Feat As FeatureItem, Feats As List(Of FeatureItem)) As List(Of FeatureItem)

            End Function
            Public Shared Function ByFull_Contains(Feat As FeatureItem, Feats As List(Of FeatureItem)) As List(Of FeatureItem)

            End Function
            Public Shared Function ByStart_Or_End_Contains(Feat As FeatureItem, Feats As List(Of FeatureItem)) As List(Of FeatureItem)

            End Function
        End Class
        Class Find_Locis

        End Class

    End Namespace

End Namespace

