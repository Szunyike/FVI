Imports Bio
Imports Bio.IO.GenBank
Imports FV_VI.Szunyi.Other_Database.Affy

Namespace Szunyi
    Public Module Comparares
        Public Class AllComparares
            Public Shared Property BySeqID As New SequenceIDComparer
            Public Shared Property BySeqIDAndLength As New SequenceIDAndLengthComparer
            Public Shared Property ByExtFeatureLocation As New ExtFeatureLocationComparer
            Public Shared Property ByExtFeatureLocusTag As New ExtFeatureLocationComparer
            Public Shared Property ByLocationStart As New LocationComparer
            Public Shared Property ByFeauterItemLocation As New FeatureItemComparer
            Public Class Affy
                Public Shared Property AffyBySeq As New AffyProbeComparerBySeq
                Public Shared Property AffyByID As New AffyProbeComparerByID
                Public Shared Property AffyProbesBySeq As New AffyGroupComparerBySeq
                Public Shared Property AffyProbesByID As New AffyGroupComparerByAffyID
            End Class

        End Class

        Public Class StringComparer
            Implements IComparer(Of String())

            Public Function Compare(ByVal x() As String, ByVal y() As String) As Integer Implements IComparer(Of String()).Compare

                Return x.First.CompareTo(y.First)

            End Function

        End Class

#Region "SeqID"
        ''' <summary>
        ''' Sort/Find By SeqID in List Of Bio.Sequence
        ''' </summary>
        Public Class SequenceIDComparer
            Implements IComparer(Of Bio.ISequence)

            Public Function Compare(x As Bio.ISequence, y As Bio.ISequence) As Integer Implements IComparer(Of Bio.ISequence).Compare

                Return x.ID.CompareTo(y.ID)


            End Function
        End Class

        ''' <summary>
        ''' Sort/Find By SeqID After Length Descending in List Of Bio.Sequence
        ''' </summary>
        Public Class SequenceIDAndLengthComparer
            Implements IComparer(Of Bio.ISequence)

            Public Function Compare(x As Bio.ISequence, y As Bio.ISequence) As Integer Implements IComparer(Of Bio.ISequence).Compare
                If x.ID <> y.ID Then
                    Return x.ID.CompareTo(y.ID)
                ElseIf y.Count > 0 And x.Count > 0 Then
                    Return y.Count.CompareTo(x.Count)
                Else
                    Return x.ID.CompareTo(y.ID)
                End If

            End Function
        End Class
#End Region

#Region "Feature And Locations"
        ''' <summary>
        ''' Compare By Start, After End Reverse After gene, mrna,CDS
        ''' </summary>
        Public Class FeatureItemComparer
            Implements IComparer(Of FeatureItem)
            Dim Dict As New Dictionary(Of String, Integer)
            Public Sub New()
                Dict.Add(StandardFeatureKeys.Gene, 0)
                Dict.Add(StandardFeatureKeys.MessengerRna, 1)
                Dict.Add(StandardFeatureKeys.CodingSequence, 2)
            End Sub
            Public Function Compare(x As FeatureItem, y As FeatureItem) As Integer Implements IComparer(Of FeatureItem).Compare
                If x.Location.LocationStart <> y.Location.LocationStart Then
                    Return x.Location.LocationStart.CompareTo(y.Location.LocationStart)
                Else
                    If x.Location.LocationEnd <> y.Location.LocationEnd Then
                        Return y.Location.LocationEnd.CompareTo(x.Location.LocationEnd)
                    Else
                        Return Dict(x.Key).CompareTo(Dict(y.Key))
                    End If

                End If
            End Function
        End Class

        ''' <summary>
        ''' Sort/Find by SeqID, After Startposition, After EndPosition, After Location String
        ''' </summary>
        Public Class ExtFeatureLocationComparer
            Implements IComparer(Of ExtFeature)
            Dim LociBuilder As New Bio.IO.GenBank.LocationBuilder
            Public Function Compare(x As ExtFeature, y As ExtFeature) As Integer Implements IComparer(Of ExtFeature).Compare
                If x.SeqID = y.SeqID Then
                    If x.Feature.Location.LocationStart <> y.Feature.Location.LocationStart Then
                        Return x.Feature.Location.LocationStart.CompareTo(y.Feature.Location.LocationStart)
                    Else
                        If x.Feature.Location.LocationEnd <> y.Feature.Location.LocationEnd Then
                            Return x.Feature.Location.LocationEnd.CompareTo(y.Feature.Location.LocationEnd)
                        Else
                            Return LociBuilder.GetLocationString(x.Feature.Location).CompareTo(LociBuilder.GetLocationString(y.Feature.Location))
                        End If
                    End If
                Else
                    Return x.SeqID.CompareTo(y.SeqID)
                End If
            End Function

        End Class

        ''' <summary>
        ''' Sort/Find by LocusTag of ExtFeature
        ''' </summary>
        Public Class ExtFeatureLocusTagComparer
            Implements IComparer(Of ExtFeature)

            Public Function Compare(x As ExtFeature, y As ExtFeature) As Integer Implements IComparer(Of ExtFeature).Compare
                Return x.LocusTag.CompareTo(y.LocusTag)

            End Function

        End Class

        ''' <summary>
        ''' Sort/Find By Seq ID and After By StartPosition From Location
        ''' </summary>
        Public Class LocationComparer
            Implements IComparer(Of Location)

            Public Function Compare(x As Location, y As Location) As Integer Implements IComparer(Of Location).Compare
                If x.SeqID = y.SeqID Then
                    Return x.Start.CompareTo(y.Start)
                Else
                    Return x.SeqID.CompareTo(y.SeqID)
                End If
            End Function

        End Class
#End Region

#Region "Affy"
        ''' <summary>
        ''' Sort/Find By Seq in AffyProbe
        ''' </summary>
        Public Class AffyProbeComparerBySeq
            Implements IComparer(Of Szunyi.Other_Database.Affy.AffyProbe)

            Public Function Compare(x As AffyProbe, y As AffyProbe) As Integer Implements IComparer(Of AffyProbe).Compare
                Return x.Seq.CompareTo(y.Seq)
            End Function
        End Class

        ''' <summary>
        ''' Sort/Find By AffyID in AffyProbe
        ''' </summary>
        Public Class AffyProbeComparerByID
            Implements IComparer(Of Szunyi.Other_Database.Affy.AffyProbe)

            Public Function Compare(x As AffyProbe, y As AffyProbe) As Integer Implements IComparer(Of AffyProbe).Compare
                Return x.ID.CompareTo(y.ID)
            End Function
        End Class
        Public Class AffyGroupComparerBySeq
            Implements IComparer(Of Szunyi.Other_Database.Affy.AffyProbeBySeq)

            Public Function Compare(x As AffyProbeBySeq, y As AffyProbeBySeq) As Integer Implements IComparer(Of AffyProbeBySeq).Compare
                Return x.Seq.CompareTo(y.Seq)
            End Function
        End Class

        Public Class AffyGroupComparerByAffyID
            Implements IComparer(Of Szunyi.Other_Database.Affy.AffyParsingResultByAffyID)

            Public Function Compare(x As AffyParsingResultByAffyID, y As AffyParsingResultByAffyID) As Integer Implements IComparer(Of AffyParsingResultByAffyID).Compare
                Return x.AffyID.CompareTo(y.AffyID)
            End Function
        End Class
#End Region
    End Module
End Namespace



