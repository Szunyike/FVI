Imports System.IO
Imports Bio.IO.GenBank
Imports ClassLibrary1.Szunyi.Constants
Imports ClassLibrary1.Szunyi.Location

Namespace Szunyi.BAM
    ''' <summary>
    ''' Compare By ReadID and After By LocationStart
    ''' </summary>
    Public Class IlocationCOmparter
        Implements IComparer(Of ILocation)

        Public Function Compare(x As ILocation, y As ILocation) As Integer Implements IComparer(Of ILocation).Compare
            If x.Accession <> y.Accession Then
                Return x.Accession.CompareTo(y.Accession)
            Else
                Return x.LocationStart.CompareTo(y.LocationStart)
            End If
        End Function
    End Class

    Public Class Create_Transcripts
        Public Property TSSs_byMergingWidth_LL_BL As Dictionary(Of Integer, List(Of List(Of Szunyi.Location.Basic_Location)))
        Public Property PASs_byMergingWidth_LL_BL As Dictionary(Of Integer, List(Of List(Of Szunyi.Location.Basic_Location)))

        Public Property TSSs_byMergingWidth_LL_Mappings As New Dictionary(Of Integer, List(Of List(Of ILocation)))
        Public Property PASs_byMergingWidth_LL_Mappings As New Dictionary(Of Integer, List(Of List(Of ILocation)))

        Public Property Mappings_with_TSS_GroupIDs As New Dictionary(Of Integer, Dictionary(Of ILocation, Integer))
        Public Property Mappings_with_PAS_GroupIDs As New Dictionary(Of Integer, Dictionary(Of ILocation, Integer))

        Public Property Seq As Bio.ISequence
        Public Property Merging_Sizes As List(Of Integer)
        Public Property Window_Sizes As List(Of Integer)

        Dim Reads_w_Tss As Dictionary(Of ILocation, List(Of List(Of Szunyi.Location.Basic_Location)))
        Dim Reads_w_PAS As New Dictionary(Of ILocation, List(Of Szunyi.Location.Basic_Location))

        Dim TSS_Mappings As New Dictionary(Of Integer, List(Of KeyValuePair(Of ILocation, List(Of List(Of Szunyi.Location.Basic_Location)))))
        Dim PAS_Mappings As New Dictionary(Of Integer, List(Of KeyValuePair(Of ILocation, List(Of List(Of Szunyi.Location.Basic_Location)))))

        '   Dim Reads_w_TssII As New List(Of KeyValuePair(Of Mapping, List(Of List(Of Szunyi.Location.Basic_Location))))
        '  Dim Reads_w_PASII As New List(Of KeyValuePair(Of Mapping, List(Of List(Of Szunyi.Location.Basic_Location))))

        Dim PAS_Sites As Sites
        Dim TSS_Sites As Sites

        Dim FOlder As DirectoryInfo
        Dim comp As New IlocationCOmparter

        Dim Interesting_TSS_Locis As List(Of Szunyi.Location.Basic_Location)
        Dim Interesting_PAS_Locis As List(Of Szunyi.Location.Basic_Location)

        Dim Interesting_TSS_Locis_wMappings As List(Of List(Of Szunyi.Location.Basic_Location))
        Dim Interesting_PAS_Locis_wMappings As List(Of List(Of Szunyi.Location.Basic_Location))
        Public Property Features As List(Of FeatureItem)

        Public Sub New(TSS As Dictionary(Of Integer, List(Of List(Of Szunyi.Location.Basic_Location))),
                                                  PAS As Dictionary(Of Integer, List(Of List(Of Szunyi.Location.Basic_Location))),
                                                  Seq As Bio.ISequence,
                                                    WindowSizes As List(Of Integer),
                                                    FOlder As DirectoryInfo,
                       TSS_Locis As List(Of Basic_Location),
                       PAS_Locis As List(Of Basic_Location),
                        Optional I_TSS_Locis As List(Of Szunyi.Location.Basic_Location) = Nothing,
                                         Optional I_PAS_Locis As List(Of Szunyi.Location.Basic_Location) = Nothing,
                                             Optional Features As List(Of FeatureItem) = Nothing)

            Me.TSSs_byMergingWidth_LL_BL = TSS
            Me.PASs_byMergingWidth_LL_BL = PAS
            Me.Seq = Seq

            Me.Window_Sizes = WindowSizes
            Me.FOlder = FOlder
            Me.Interesting_PAS_Locis = I_PAS_Locis
            Me.Interesting_TSS_Locis = I_TSS_Locis
            Me.Features = Features

            Me.PAS_Sites = New Sites(Me.Seq, PAS_Locis,  Sort_Locations_By.PAS)
            Me.TSS_Sites = New Sites(Me.Seq, TSS_Locis, Sort_Locations_By.TSS)
            Convert()
            Init()

        End Sub

        Private Function Get_All_Locations(values As Dictionary(Of Integer, List(Of List(Of Basic_Location))).ValueCollection) As List(Of Basic_Location)
            Dim out As New List(Of Basic_Location)
            For Each Item In values
                For Each s In Item
                    out.AddRange(s)
                Next
            Next
            Return out
        End Function

        Private Sub Convert()
            For Each T In Me.TSSs_byMergingWidth_LL_BL
                Me.TSSs_byMergingWidth_LL_Mappings.Add(T.Key, New List(Of List(Of ILocation)))
                For Each Item In T.Value
                    Dim tmp As New List(Of ILocation)
                    For Each M In Item
                        tmp.Add(M.Location)
                    Next
                    tmp.Sort(comp)
                    Me.TSSs_byMergingWidth_LL_Mappings(T.Key).Add(tmp)
                Next
            Next

            For Each T In Me.PASs_byMergingWidth_LL_BL
                Me.PASs_byMergingWidth_LL_Mappings.Add(T.Key, New List(Of List(Of ILocation)))
                For Each Item In T.Value
                    Dim tmp As New List(Of ILocation)
                    For Each M In Item
                        tmp.Add(M.Location)
                    Next
                    tmp.Sort(comp)
                    Me.PASs_byMergingWidth_LL_Mappings(T.Key).Add(tmp)
                Next
            Next
        End Sub

        Private Sub Init()
            For Each Merging_Sized In Me.TSSs_byMergingWidth_LL_Mappings
                Me.Mappings_with_TSS_GroupIDs.Add(Merging_Sized.Key, New Dictionary(Of ILocation, Integer))
                For i1 = 0 To Merging_Sized.Value.Count - 1
                    For Each M In Merging_Sized.Value(i1)
                        If Me.Mappings_with_TSS_GroupIDs(Merging_Sized.Key).ContainsKey(M) = False Then
                            Me.Mappings_with_TSS_GroupIDs(Merging_Sized.Key).Add(M, i1)
                        End If
                    Next
                Next
            Next

            For Each Merging_Sized In Me.PASs_byMergingWidth_LL_Mappings
                Me.Mappings_with_PAS_GroupIDs.Add(Merging_Sized.Key, New Dictionary(Of ILocation, Integer))
                For i1 = 0 To Merging_Sized.Value.Count - 1
                    For Each M In Merging_Sized.Value(i1)
                        If Me.Mappings_with_PAS_GroupIDs(Merging_Sized.Key).ContainsKey(M) = False Then
                            Me.Mappings_with_PAS_GroupIDs(Merging_Sized.Key).Add(M, i1)
                        End If
                    Next
                Next
            Next



        End Sub

        Public Function DoIt(ExtName As String)
            Dim res = Analyse() ' Dictionary(Of Integer, Dictionary(Of Integer, List(Of Ilocation))) ' TSS Index, Pas Index
            Dim Out As New Dictionary(Of FileInfo, String)
            Dim Index As Integer = 0
            For Each M_TSS In Me.TSSs_byMergingWidth_LL_Mappings
                For Each M_PAS In Me.PASs_byMergingWidth_LL_Mappings
                    Dim Transcript_ID As Integer = 0
                    Dim ToWrite = res(Index)
                    Dim x As New FileInfo(Me.FOlder.FullName & "\TSS_" & M_TSS.Key & "_PAS" & M_PAS.Key & "_" & ExtName & ".tsv")
                    Dim str As New System.Text.StringBuilder
                    '         str.Append("TSS_ID").Append(vbTab).Append("PAS_ID").Append(vbTab)
                    str.Append(Get_Header(M_TSS.Key, M_PAS.Key))
                    Dim TSS_ID_Group_ID As Dictionary(Of Integer, Integer) = Get_Group_IDs(ToWrite)
                    Dim Groups = Get_Group_Locations(ToWrite, TSS_ID_Group_ID) ' min max
                    Dim TSS_ID As Integer = 0
                    For Each T In ToWrite
                        For Each P In T.Value
                            Transcript_ID += 1
                            Dim cGroup = Groups(TSS_ID_Group_ID(T.Key))
                            str.Append(Get_Text(cGroup, TSS_ID_Group_ID(T.Key), Transcript_ID, P.Value, P, T, M_TSS.Key, M_PAS.Key, 50, ToWrite)).AppendLine()
                        Next
                        TSS_ID += 1
                    Next
                    Index += 1
                    Out.Add(x, str.ToString)
                    Szunyi.IO.Export.SaveText(str.ToString, x)
                Next
            Next


        End Function

        Private Function Get_Group_Locations(toWrite As Dictionary(Of Integer, Dictionary(Of Integer, List(Of ILocation))),
                                             tSS_ID_Group_ID As Dictionary(Of Integer, Integer)) As Dictionary(Of Integer, ILocation)
            Dim out As New Dictionary(Of Integer, List(Of ILocation))
            For Each Item In tSS_ID_Group_ID
                If out.ContainsKey(Item.Value) = False Then out.Add(Item.Value, New List(Of ILocation))
                For Each PAS In toWrite(Item.Key).Values
                    out(Item.Value).AddRange(PAS)
                Next
            Next
            Dim res As New Dictionary(Of Integer, ILocation)
            For Each item In out
                Dim min = Basic_Location_Manipulation.Get_Minimum(item.Value,  Sort_Locations_By.TSS)
                Dim max = Basic_Location_Manipulation.Get_Minimum(item.Value,  Sort_Locations_By.LE)
                Dim x = Szunyi.Location.Common.GetLocation(min, max)
                res.Add(item.Key, x)

            Next
            Return res
        End Function

        Private Function Get_Pas_Index_w_TSS_Index(res As Dictionary(Of Integer, Dictionary(Of Integer, List(Of ILocation)))) As Dictionary(Of Integer, List(Of Integer))
            Dim Pas_Index_w_TSS_Index As New Dictionary(Of Integer, List(Of Integer))
            For Each TSS In res
                For Each PAS In TSS.Value
                    If Pas_Index_w_TSS_Index.ContainsKey(PAS.Key) = False Then
                        Pas_Index_w_TSS_Index.Add(PAS.Key, New List(Of Integer))
                    End If
                    Pas_Index_w_TSS_Index(PAS.Key).Add(TSS.Key)
                Next
            Next
            Return Pas_Index_w_TSS_Index
        End Function

        Private Function Get_TSS_Index_w_PAS_Index(res As Dictionary(Of Integer, Dictionary(Of Integer, List(Of ILocation)))) As Dictionary(Of Integer, List(Of Integer))
            Dim Pas_Index_w_TSS_Index As New Dictionary(Of Integer, List(Of Integer))
            For Each TSS In res
                Pas_Index_w_TSS_Index.Add(TSS.Key, New List(Of Integer))
                For Each PAS In TSS.Value

                    Pas_Index_w_TSS_Index(TSS.Key).Add(PAS.Key)
                Next
            Next
            Return Pas_Index_w_TSS_Index
        End Function

        Private Function Get_Group_IDs(res As Dictionary(Of Integer, Dictionary(Of Integer, List(Of ILocation)))) As Dictionary(Of Integer, Integer)
            Dim TSS_ID_Group_ID As New Dictionary(Of Integer, Integer)
            Dim Used_TSS_IDs As New List(Of Integer)
            Dim Used_PAS_IDs As New List(Of Integer)
            Dim Pas_Index_w_TSS_Index As Dictionary(Of Integer, List(Of Integer)) = Get_Pas_Index_w_TSS_Index(res)
            Dim TSS_Index_w_PAS_Index = Get_TSS_Index_w_PAS_Index(res)
            Dim Group_ID As Integer = 0
            For Each TSS In res
                If TSS.Value.Count = 0 Then Used_TSS_IDs.Add(TSS.Key)
                If Used_TSS_IDs.Contains(TSS.Key) = False Then
                    Used_TSS_IDs.Add(TSS.Key)
                    Dim cTSS As New List(Of Integer)
                    cTSS.Add(TSS.Key)
                    Group_ID += 1
                    TSS_ID_Group_ID.Add(TSS.Key, Group_ID)
                    Do
                        Dim nCTSS As New List(Of Integer)
                        For Each TS In cTSS
                            Dim t = res(TS)
                            Dim Pas_Indexes = TSS_Index_w_PAS_Index(TS)

                            For Each Pas_Index In Pas_Indexes
                                If Used_PAS_IDs.Contains(Pas_Index) = False Then
                                    Used_PAS_IDs.Add(Pas_Index)
                                    For Each TSS_Key In Pas_Index_w_TSS_Index(Pas_Index)
                                        If Used_TSS_IDs.Contains(TSS_Key) = True Or cTSS.Contains(TSS_Key) = True Then

                                        Else
                                            ' Not Used TSS, Not In List

                                            nCTSS.Add(TSS_Key)
                                            If TSS_ID_Group_ID.ContainsKey(TSS_Key) = False Then
                                                TSS_ID_Group_ID.Add(TSS_Key, Group_ID)
                                                Used_TSS_IDs.Add(TSS_Key)
                                            End If
                                        End If

                                    Next
                                End If
                            Next
                        Next
                        cTSS = nCTSS

                    Loop Until cTSS.Count = 0
                    Dim kjh As Integer = 43
                End If

            Next
            Dim ch = (From x In TSS_ID_Group_ID Select x.Value).ToList.Distinct.Count
            Return TSS_ID_Group_ID
        End Function

        Private Function Get_Header(TSS_Width As Integer, Pas_Width As Integer) As String
            Dim str As New System.Text.StringBuilder
            str.Append("Group ID").Append(vbTab)
            str.Append("Group Min Position").Append(vbTab)
            str.Append("Group Max Position").Append(vbTab)
            str.Append("Group Length").Append(vbTab)
            str.Append("Transcript ID").Append(vbTab)
            If IsNothing(Me.Features) = False Then
                str.Append("CDSs Full Same Strand").Append(vbTab)
                str.Append("Count of CDSs Full Same Strand").Append(vbTab)
                str.Append("CDS Partial Same Strand").Append(vbTab)
                str.Append("Count of CDSs Partial Same Strand").Append(vbTab)
                str.Append("CDSs Full Diff. Strand").Append(vbTab)
                str.Append("Count of CDSs Full Diff. Strand").Append(vbTab)
                str.Append("CDSs Partial Diff. Strand").Append(vbTab)
                str.Append("Count of CDSs Partial Diff. Strand").Append(vbTab)
            End If

            str.Append("TSS ID").Append(vbTab)
            str.Append("PAS ID").Append(vbTab)

            str.Append("Strand").Append(vbTab)
            str.Append("Transcript length").Append(vbTab)
            str.Append("TSS Most Abundant Position").Append(vbTab)
            str.Append("PAS Most Abundant Position").Append(vbTab)
            str.Append("TSS Min Position").Append(vbTab)
            str.Append("TSS Max Position").Append(vbTab)
            str.Append("PAS Min Position").Append(vbTab)
            str.Append("PAS Max Position").Append(vbTab)

            str.Append("Nof of diff TSS").Append(vbTab)
            str.Append("Nof of diff PAS").Append(vbTab)
            str.Append("Nof of Reads In Transcripts").Append(vbTab)

            str.Append("Count of TSS").Append(vbTab)
            str.Append("Count of TSS in Merged Width").Append(vbTab)
            str.Append("Count of TSS in WindowSize").Append(vbTab)
            '    str.Append("P value of TSS in Merged With").Append(vbTab)
            '     str.Append("P value of TSS in WindowSize").Append(vbTab)
            str.Append("Count of PAS").Append(vbTab)
            str.Append("Count of PAS in Merged Width").Append(vbTab)
            str.Append("Count of PAS in WindowSize").Append(vbTab)
            '    str.Append("P value of PAS in Merged With").Append(vbTab)
            '     str.Append("P value of PAS in WindowSize").Append(vbTab)

            '     str.Append("Percent of False PolyA Precise").Append(vbTab)
            'str.Append("Percent of False PolyA in Merged Regions").Append(vbTab)
            str.Append("Nof A From Beginning").Append(vbTab)
            str.Append("Nof Consecutive A").Append(vbTab)
            str.Append("Percents of A").Append(vbTab)
            str.Append("Nof T From Beginning").Append(vbTab)
            str.Append("Nof Consecutive T").Append(vbTab)
            str.Append("Percents of T").Append(vbTab)

            str.Append("Best polyA Signal Sequence").Append(vbTab)
            str.Append("Best polyA Signal Sequence Position").Append(vbTab)
            str.Append("Best polyA Signal Sequence Distance").Append(vbTab)


            For i1 = -TSS_Width To TSS_Width
                str.Append("TSS" & i1).Append(vbTab)
            Next
            For i1 = -Pas_Width To Pas_Width
                str.Append("PAS" & i1).Append(vbTab)
            Next
            ' Ide kell bepakolni ay intronokat!
            str.AppendLine()

            Return str.ToString
        End Function

        Public Shared Function Convert(x As Object) As ILocation
            Dim m As ILocation = x
            Return m
        End Function

        Private Function Analyse() As List(Of Dictionary(Of Integer, Dictionary(Of Integer, List(Of ILocation)))) ' TSS Index, Pas Index
            Dim out As New List(Of Dictionary(Of Integer, Dictionary(Of Integer, List(Of ILocation))))
            For Each M_TSS In Me.TSSs_byMergingWidth_LL_Mappings
                For Each M_PAS In Me.PASs_byMergingWidth_LL_Mappings
                    Dim resII As New Dictionary(Of Integer, Dictionary(Of Integer, List(Of ILocation))) ' TSS Index, Pas Index
                    Dim l = SubAnalyse(M_TSS, M_PAS)
                    out.Add(l)
                Next
            Next
            Return out
        End Function
        ''' <summary>
        ''' 
        ''' </summary>
        ''' <param name="m_TSS"></param>
        ''' <param name="m_PAS"></param>
        ''' <returns></returns>
        Private Function SubAnalyse(m_TSS As KeyValuePair(Of Integer, List(Of List(Of ILocation))), m_PAS As KeyValuePair(Of Integer, List(Of List(Of ILocation)))) As Dictionary(Of Integer, Dictionary(Of Integer, List(Of ILocation)))
            Dim res As New Dictionary(Of Integer, Dictionary(Of Integer, List(Of ILocation))) ' TSS Index, Pas Index
            Dim cTSS = Me.TSSs_byMergingWidth_LL_Mappings(m_TSS.Key)
            Dim cPAS = Me.PASs_byMergingWidth_LL_Mappings(m_PAS.Key)

            Dim cTSS_wIDS As New Location_w_IDs(cTSS)
            Dim cPAS_wIDS As New Location_w_IDs(cPAS)

            Dim comp As New IlocationCOmparter
            For i1 = 0 To cTSS.Count - 1
                res.Add(i1, New Dictionary(Of Integer, List(Of ILocation)))
                Dim PasIndexes As New List(Of Integer)

                For Each Loci In cTSS(i1)
                    Dim PasIndex = cPAS_wIDS.Get_ID(Loci) ' BinarySearch
                    If PasIndex <> -1 Then
                        If res(i1).ContainsKey(PasIndex) = False Then
                            res(i1).Add(PasIndex, New List(Of ILocation))
                        End If
                        res(i1)(PasIndex).Add(Loci)
                    End If
                Next
            Next

            Return res
        End Function

        Public Class Location_w_ID_comp
            Implements IComparer(Of Location_w_ID)

            Public Function Compare(x As Location_w_ID, y As Location_w_ID) As Integer Implements IComparer(Of Location_w_ID).Compare
                If x.Loci.Accession <> y.Loci.Accession Then
                    Return x.Loci.Accession.CompareTo(y.Loci.Accession)
                Else
                    Return x.Loci.LocationStart.CompareTo(y.Loci.LocationStart)
                End If
            End Function
        End Class

        Public Class Location_w_IDs
            Public Property L As New List(Of Location_w_ID)
            Dim c As New Location_w_ID_comp
            Public Sub New(Locis As List(Of List(Of Bio.IO.GenBank.ILocation)))
                For i1 = 0 To Locis.Count - 1
                    For Each loci In Locis(i1)
                        L.Add(New Location_w_ID(loci, i1))
                    Next
                Next

                L.Sort(c)
            End Sub
            Public Function Get_ID(Loci As Bio.IO.GenBank.ILocation) As Integer
                Dim x As New Location_w_ID(Loci, 0)
                Dim Index = Me.L.BinarySearch(x, c)
                If Index > -1 Then
                    Return Me.L(Index).ID
                Else
                    Return -1
                End If

            End Function
        End Class

        Public Class Location_w_ID
            Public Property Loci As Bio.IO.GenBank.ILocation
            Public Property ID As Integer
            Public Sub New(Loci As Bio.IO.GenBank.ILocation, ID As Integer)
                Me.Loci = Loci
                Me.ID = ID
            End Sub
        End Class

        Private Function Get_Text(Groups As ILocation, Group_ID As Integer, Transcript_ID As Integer, DiffIntron As List(Of ILocation),
                                      pAS_Index As KeyValuePair(Of Integer, List(Of ILocation)),
                                      tSS_Index As KeyValuePair(Of Integer, Dictionary(Of Integer, List(Of ILocation))),
                                      TSS_merging_Width As Integer,
                                      PAS_Merging_Width As Integer,
                                      windowSize As Integer,
                                      res As Dictionary(Of Integer, Dictionary(Of Integer, List(Of ILocation)))) As String

            Dim str As New System.Text.StringBuilder
            str.Append(Group_ID).Append(vbTab)
            str.Append(Groups.LocationStart).Append(vbTab)
            str.Append(Groups.LocationEnd).Append(vbTab)
            str.Append(Groups.LocationEnd - Groups.LocationStart).Append(vbTab)

            str.Append(Transcript_ID).Append(vbTab)
            str.Append(Get_Overlapped_Items(DiffIntron))

            str.Append(tSS_Index.Key).Append(vbTab)
            str.Append(pAS_Index.Key).Append(vbTab)
            If pAS_Index.Value.First.IsComplementer = True Then
                str.Append("-").Append(vbTab)
            Else
                str.Append("+").Append(vbTab)
            End If
            Dim MostAbundant_TSS = Szunyi.Location.Basic_Location_Manipulation.Get_Most_Abundants(DiffIntron, Sort_Locations_By.TSS)
            Dim MostAbundant_PAS = Szunyi.Location.Basic_Location_Manipulation.Get_Most_Abundants(DiffIntron,  Sort_Locations_By.PAS)

            str.Append(System.Math.Abs(MostAbundant_TSS.First.TSS - MostAbundant_PAS.First.PAS)).Append(vbTab)
            str.Append(MostAbundant_TSS.First.TSS).Append(vbTab)
            str.Append(MostAbundant_PAS.First.PAS).Append(vbTab)
            str.Append(Basic_Location_Manipulation.Get_Minimum(DiffIntron, Sort_Locations_By.TSS)).Append(vbTab)
            str.Append(Basic_Location_Manipulation.Get_Maximum(DiffIntron, Sort_Locations_By.TSS)).Append(vbTab)
            str.Append(Basic_Location_Manipulation.Get_Minimum(DiffIntron,  Sort_Locations_By.PAS)).Append(vbTab)
            str.Append(Basic_Location_Manipulation.Get_Maximum(DiffIntron,  Sort_Locations_By.PAS)).Append(vbTab)



            Dim kj = (From x In res Where x.Value.ContainsKey(pAS_Index.Key)).ToList
            str.Append(kj.Count).Append(vbTab) ' Nof Diff TSS
            str.Append(res(tSS_Index.Key).Count).Append(vbTab) ' Nod Diff PAS
            str.Append(DiffIntron.Count).Append(vbTab) ' Count of Reads
            Dim One_Pos_TSS = TSS_Sites.Get_Distribution(MostAbundant_TSS.First, Sort_Locations_By.TSS)
            Dim Merged_TSS = TSS_Sites.Get_Distribution(MostAbundant_TSS.First, Sort_Locations_By.TSS)
            Dim WindowSize_TSS = TSS_Sites.Get_Distribution(MostAbundant_TSS.First, Sort_Locations_By.TSS)

            str.Append(One_Pos_TSS.Sum).Append(vbTab)
            str.Append(Merged_TSS.Sum).Append(vbTab)
            str.Append(WindowSize_TSS.Sum).Append(vbTab)

            If Merged_TSS.Sum / Merged_TSS.Count > 0 Then
                Dim Poi_Avarage_TSS_Merged As New Accord.Statistics.Distributions.Univariate.PoissonDistribution(Merged_TSS.Sum / Merged_TSS.Count)
                Dim p_Poi_Avarage_TSS_Merged = 1 - Poi_Avarage_TSS_Merged.ProbabilityMassFunction(One_Pos_TSS.Sum)
                '   str.Append(p_Poi_Avarage_TSS_Merged).Append(vbTab)
            Else
                '  str.Append(vbTab)
            End If
            If WindowSize_TSS.Sum / WindowSize_TSS.Count > 0 Then
                Dim Poi_Avarage_TSS_WindowSize As New Accord.Statistics.Distributions.Univariate.PoissonDistribution(WindowSize_TSS.Sum / WindowSize_TSS.Count)
                Dim p_Poi_Avarage_TSS_WindowSize = 1 - Poi_Avarage_TSS_WindowSize.ProbabilityMassFunction(One_Pos_TSS.Sum)
                '   str.Append(p_Poi_Avarage_TSS_WindowSize).Append(vbTab)
            Else
                '   str.Append(vbTab)
            End If
            Dim One_Pos_PAS = PAS_Sites.Get_Distribution(MostAbundant_PAS.First,  Sort_Locations_By.PAS)
            Dim Merged_PAS = PAS_Sites.Get_Distribution(MostAbundant_PAS.First,  Sort_Locations_By.PAS)
            Dim WindowSize_PAS = PAS_Sites.Get_Distribution(MostAbundant_PAS.First,  Sort_Locations_By.PAS)

            str.Append(One_Pos_PAS.Sum).Append(vbTab)
            str.Append(Merged_PAS.Sum).Append(vbTab)
            str.Append(WindowSize_PAS.Sum).Append(vbTab)

            If Merged_PAS.Sum / Merged_PAS.Count > 0 Then
                Dim Poi_Avarage_PAS_Merged As New Accord.Statistics.Distributions.Univariate.PoissonDistribution(Merged_PAS.Sum / Merged_PAS.Count)
                Dim p_Poi_Avarage_PAS_Merged = 1 - Poi_Avarage_PAS_Merged.ProbabilityMassFunction(One_Pos_PAS.Sum)
                '   str.Append(p_Poi_Avarage_PAS_Merged).Append(vbTab)
            Else
                '  str.Append(vbTab)
            End If
            If WindowSize_PAS.Sum / WindowSize_PAS.Count > 0 Then
                Dim Poi_Avarage_PAS_WindowSize As New Accord.Statistics.Distributions.Univariate.PoissonDistribution(WindowSize_PAS.Sum / WindowSize_PAS.Count)
                Dim p_Poi_Avarage_PAS_WindowSize = 1 - Poi_Avarage_PAS_WindowSize.ProbabilityMassFunction(One_Pos_PAS.Sum)
                '   str.Append(p_Poi_Avarage_PAS_WindowSize).Append(vbTab)
            Else
                '   str.Append(vbTab)
            End If
            str.Append(Szunyi.mRNA.PolyA.False_polyA(Me.Seq, MostAbundant_PAS.First, 10, mRNA.False_PolyAT_Discovering.From_Beginning, 0, Bio.Alphabets.DNA.A)).Append(vbTab)
            str.Append(Szunyi.mRNA.PolyA.False_polyA(Me.Seq, MostAbundant_PAS.First, 10, mRNA.False_PolyAT_Discovering.Nof_Consecutive_AT, 0, Bio.Alphabets.DNA.A)).Append(vbTab)
            str.Append(Szunyi.mRNA.PolyA.False_polyA(Me.Seq, MostAbundant_PAS.First, 10, mRNA.False_PolyAT_Discovering.Percent_of_AT, 0, Bio.Alphabets.DNA.A)).Append(vbTab)
            str.Append(Szunyi.mRNA.PolyA.False_polyT(Me.Seq, MostAbundant_PAS.First, 10, mRNA.False_PolyAT_Discovering.From_Beginning, 0, Bio.Alphabets.DNA.T)).Append(vbTab)
            str.Append(Szunyi.mRNA.PolyA.False_polyT(Me.Seq, MostAbundant_PAS.First, 10, mRNA.False_PolyAT_Discovering.Nof_Consecutive_AT, 0, Bio.Alphabets.DNA.T)).Append(vbTab)
            str.Append(Szunyi.mRNA.PolyA.False_polyT(Me.Seq, MostAbundant_PAS.First, 10, mRNA.False_PolyAT_Discovering.Percent_of_AT, 0, Bio.Alphabets.DNA.T)).Append(vbTab)
            '' Missing AT


            Dim PA_Sites = Szunyi.DNA.PA.Get_PolyA_Signal(Me.Seq, MostAbundant_PAS.First, 50, -22)
            str.Append(PA_Sites.Get_Best).Append(vbTab)


            str.Append(Szunyi.Text.General.GetText(Merged_TSS, vbTab)).Append(vbTab)
            str.Append(Szunyi.Text.General.GetText(Merged_PAS, vbTab))
            Return str.ToString
        End Function

        Private Function Get_Overlapped_Items(Locis As List(Of ILocation)) As String
            Dim str As New System.Text.StringBuilder
            Dim hjg = Szunyi.Location.Common.GetLocationString(Locis.First)

            Dim Feat As New FeatureItem("a", hjg)
            Dim TheSeq = Feat.GetSubSequence(Me.Seq)
            If IsNothing(Me.Features) = False Then
                Dim item = Locis.First
                Dim FoundFullItems As New List(Of String)
                Dim FoundFullASItems As New List(Of String)
                Dim FoundPartialItems As New List(Of String)
                Dim FoundPartialASItems As New List(Of String)
                Dim ContainsFull = From x In Me.Features Where x.Location.LocationStart >= Feat.Location.LocationStart And
                                                                   x.Location.LocationEnd <= Feat.Location.LocationEnd

                For Each cf In ContainsFull
                    If cf.Location.IsComplementer = Locis.First.IsComplementer Then
                        FoundFullItems.Add(Szunyi.Features.FeatureManipulation.Qulifiers.Get_Common_Values(cf, ","))

                    Else
                        FoundFullASItems.Add(Szunyi.Features.FeatureManipulation.Qulifiers.Get_Common_Values(cf, ","))
                    End If

                Next
                Dim ContainsPartial = From x In Me.Features Where (x.Location.LocationStart >= Feat.Location.LocationStart And
                                                                      x.Location.LocationStart <= Feat.Location.LocationEnd) Or
                                                                      (x.Location.LocationEnd >= Feat.Location.LocationStart And
                                                                      x.Location.LocationEnd <= Feat.Location.LocationEnd)

                For Each cf In ContainsPartial
                    If cf.Location.IsComplementer = Locis.First.IsComplementer Then
                        FoundPartialItems.Add(Szunyi.Features.FeatureManipulation.Qulifiers.Get_Common_Values(cf, ","))
                    Else
                        FoundPartialASItems.Add(Szunyi.Features.FeatureManipulation.Qulifiers.Get_Common_Values(cf, ","))
                    End If
                Next

                str.Append(Szunyi.Text.General.GetText(FoundFullItems, ",")).Append(vbTab).Append(FoundFullItems.Count).Append(vbTab)
                Dim diFF = FoundPartialItems.Except(FoundFullItems).ToList
                str.Append(Szunyi.Text.General.GetText(diFF, ",")).Append(vbTab).Append(diFF.Count).Append(vbTab)

                str.Append(Szunyi.Text.General.GetText(FoundFullASItems, ",")).Append(vbTab).Append(FoundFullASItems.Count).Append(vbTab)



                diFF = FoundPartialASItems.Except(FoundFullASItems).ToList
                str.Append(Szunyi.Text.General.GetText(diFF, ",")).Append(diFF.Count).Append(vbTab)

                str.Append(vbTab)

            End If
            If str.Length = 0 Then Return String.Empty
            Return str.ToString
        End Function

    End Class
End Namespace

