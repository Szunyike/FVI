Imports Bio
Imports Bio.IO.GenBank
Imports ClassLibrary1.Szunyi.Comparares.OneByOne

Imports ClassLibrary1.Szunyi.Other_Database.Affy
Imports ClassLibrary1.Szunyi.ListOf
Imports ClassLibrary1.Szunyi.Other_Database.SignalP
Imports ClassLibrary1.Szunyi.Protein
Imports ClassLibrary1.Szunyi.Basic.Count
Imports Bio.IO.SAM

Namespace Szunyi
    Namespace Comparares
        Public Class AllComparares
            Public Shared Property By_Gene As New ByGeneComparer
            Public Shared Property BySeqID As New SequenceIDComparer
            Public Shared Property BySeqIDAndLength As New SequenceIDAndLengthComparer
            Public Shared Property ByExtFeatureLocation As New ExtFeatureLocationComparer
            Public Shared Property ByExtFeatureLocusTag As New ExtFeatureLocusTagComparer
            '   Public Shared Property ByLocationStart As New Szunyi.Location.Basic_Location_Comparers._ByStart

            Public Shared Property ByFeauterItemLocation As New FeatureItemComparer
            Public Shared Property ByLocusTag As New LocusTagComparerOfFeatureItem
            Public Shared Property By_Item_With_Properties_Comparer As New Item_With_Properties_Comparer
            Public Shared Property By_SignalP As New SignalP_Cleavege_Site_Comparer
            Public Shared Property AA_Compositions As New AA_Compositions_Comparer

            Public Class Affy
                Public Shared Property AffyBySeq As New AffyProbeComparerBySeq
                Public Shared Property AffyByID As New AffyProbeComparerByID
                Public Shared Property AffyProbesBySeq As New AffyGroupComparerBySeq
                Public Shared Property AffyProbesByID As New AffyGroupComparerByAffyID
                Public Shared Property ByCount As New AffyByCount

            End Class

            Public Class Basic_Location
                '    Public Shared Property ByLocationStart As New LocationComparer_ByStart
                '   Public Shared Property LocationComparer_ByEnd As New LocationComparer_ByEnd
            End Class
        End Class

        Namespace OneByOne

            Public Class Sam_QName
                Implements IEqualityComparer(Of SAMAlignedSequence)

                Implements IComparer(Of Bio.IO.SAM.SAMAlignedSequence)

                Public Function Compare(x As SAMAlignedSequence, y As SAMAlignedSequence) As Integer Implements IComparer(Of SAMAlignedSequence).Compare
                    Return x.QName.CompareTo(y.QName)
                End Function

                Public Function Equals(x As SAMAlignedSequence, y As SAMAlignedSequence) As Boolean Implements IEqualityComparer(Of SAMAlignedSequence).Equals
                    If x.QName = y.QName Then Return True

                    Return False
                End Function

                Public Function GetHashCode(obj As SAMAlignedSequence) As Integer Implements IEqualityComparer(Of SAMAlignedSequence).GetHashCode
                    Return obj.RName.GetHashCode
                End Function
            End Class
            Public Class AA_Compositions_Comparer
                Implements IComparer(Of Szunyi.Protein.Amino_Acid_Composition)

                Public Function Compare(x As Amino_Acid_Composition, y As Amino_Acid_Composition) As Integer Implements IComparer(Of Amino_Acid_Composition).Compare
                    Return x.Seq.ID.CompareTo(y.Seq.ID)
                End Function
            End Class
            Public Class StringComparer
                Implements IComparer(Of String())

                Public Function Compare(ByVal x() As String, ByVal y() As String) As Integer Implements IComparer(Of String()).Compare

                    Return x.First.CompareTo(y.First)

                End Function

            End Class

            Public Class Item_With_Properties_Comparer
                Implements IComparer(Of ClassLibrary1.Szunyi.Text.TableManipulation.Item_With_Properties)


                Public Function Compare(x As Szunyi.Text.TableManipulation.Item_With_Properties, y As Szunyi.Text.TableManipulation.Item_With_Properties) As Integer Implements IComparer(Of Szunyi.Text.TableManipulation.Item_With_Properties).Compare
                    Return x.ID.CompareTo(y.ID)
                End Function
            End Class

            Public Class SignalP_Cleavege_Site_Comparer
                Implements IComparer(Of ClassLibrary1.Szunyi.Other_Database.SignalP.SignalPCleavageSite)

                Public Function Compare(x As SignalPCleavageSite, y As SignalPCleavageSite) As Integer Implements IComparer(Of SignalPCleavageSite).Compare
                    Return x.SeqID.CompareTo(y.SeqID)
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
            ''' <summary>
            ''' Sort/Find By SeqID After Length Descending in List Of Bio.Sequence
            ''' </summary>
            Public Class BySeqID_ByLength_ByContent
                Implements IComparer(Of Bio.ISequence)

                Public Function Compare(x As Bio.ISequence, y As Bio.ISequence) As Integer Implements IComparer(Of Bio.ISequence).Compare
                    If x.ID <> y.ID Then
                        Return x.ID.CompareTo(y.ID)
                    ElseIf y.Count <> x.Count > 0 Then
                        Return y.Count.CompareTo(x.Count)
                    Else
                        For i1 = 0 To x.Count - 1
                            If x(i1) <> y(i1) Then
                                Return x(i1).CompareTo(y(i1))
                            End If
                        Next
                    End If
                    Return x.ID.CompareTo(y.ID)
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
                    Dict.Add(StandardFeatureKeys.TransferRna, 1)
                End Sub
                Public Function Compare(x As FeatureItem, y As FeatureItem) As Integer Implements IComparer(Of FeatureItem).Compare
                    If x.Location.LocationStart <> y.Location.LocationStart Then
                        Return x.Location.LocationStart.CompareTo(y.Location.LocationStart)
                    Else
                        If x.Location.LocationEnd <> y.Location.LocationEnd Then
                            Return y.Location.LocationEnd.CompareTo(x.Location.LocationEnd)
                        Else
                            If Dict.ContainsKey(x.Key) AndAlso Dict.ContainsKey(y.Key) Then
                                Return Dict(x.Key).CompareTo(Dict(y.Key))
                            Else
                                Return x.Key.CompareTo(y.Key)
                            End If
                        End If

                    End If
                End Function
            End Class

            ''' <summary>
            ''' Sort/Find by SeqID, After Startposition, After EndPosition, After Location String
            ''' </summary>
            Public Class ExtFeatureLocationComparer
                Implements IComparer(Of ExtFeature)

                Public Function Compare(x As ExtFeature, y As ExtFeature) As Integer Implements IComparer(Of ExtFeature).Compare
                    Return x.LocationString.CompareTo(y.LocationString)
                End Function

            End Class

            ''' <summary>
            ''' Sort/Find by LocusTag of ExtFeature
            ''' </summary>
            Public Class ExtFeatureLocusTagComparer
                Implements IComparer(Of ExtFeature)

                Public Function Compare(x As ExtFeature, y As ExtFeature) As Integer Implements IComparer(Of ExtFeature).Compare
                    If IsNothing(x.LocusTag) = False AndAlso IsNothing(y.LocusTag) = False Then
                        Return x.LocusTag.CompareTo(y.LocusTag)
                    Else
                        Return x.Seq.ID.CompareTo(y.Seq.ID)
                    End If

                End Function

            End Class

            Public Class BioLocationComparer
                Implements IComparer(Of Bio.IO.GenBank.Location)

                Public Function Compare(x As Bio.IO.GenBank.Location, y As Bio.IO.GenBank.Location) As Integer Implements IComparer(Of Bio.IO.GenBank.Location).Compare
                    Return x.LocationStart.CompareTo(y.LocationStart)
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
            ''' <summary>
            ''' Sort/Find By Count Descending
            ''' </summary>
            Public Class AffyByCount
                Implements IComparer(Of Szunyi.Other_Database.Affy.AffyIDwCount)

                Public Function Compare(x As AffyIDwCount, y As AffyIDwCount) As Integer Implements IComparer(Of AffyIDwCount).Compare
                    Return y.Count.CompareTo(x.Count)
                End Function
            End Class
#End Region
            Public Class ByGeneComparer
                Implements IComparer(Of FeatureItem)

                Public Function Compare(x As FeatureItem, y As FeatureItem) As Integer Implements IComparer(Of FeatureItem).Compare
                    If IsNothing(x) = True Then Return -1
                    If IsNothing(y) = True Then Return 1
                    If x.Qualifiers.ContainsKey(StandardQualifierNames.GeneSymbol) = False Then
                        Return -1
                    End If
                    If y.Qualifiers.ContainsKey(StandardQualifierNames.GeneSymbol) = False Then
                        Return +1
                    End If
                    Dim xlTag = x.Qualifiers(StandardQualifierNames.GeneSymbol).First
                    Dim ylTag = y.Qualifiers(StandardQualifierNames.GeneSymbol).First
                    Return xlTag.CompareTo(ylTag)
                End Function
            End Class
            Public Class LocusTagComparerOfFeatureItem
                Implements IComparer(Of FeatureItem)

                Public Function Compare(x As FeatureItem, y As FeatureItem) As Integer Implements IComparer(Of FeatureItem).Compare
                    If IsNothing(x) = True Then Return -1
                    If IsNothing(y) = True Then Return 1
                    If x.Qualifiers.ContainsKey(StandardQualifierNames.LocusTag) = False Then
                        Return -1
                    End If
                    If y.Qualifiers.ContainsKey(StandardQualifierNames.LocusTag) = False Then
                        Return +1
                    End If
                    Dim xlTag = x.Qualifiers(StandardQualifierNames.LocusTag).First
                    Dim ylTag = y.Qualifiers(StandardQualifierNames.LocusTag).First
                    Return xlTag.CompareTo(ylTag)
                End Function
            End Class



        End Namespace

    End Namespace
End Namespace



