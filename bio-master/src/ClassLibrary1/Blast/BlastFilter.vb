Imports Bio.Web.Blast

Namespace Szunyi.Blast
    Public Class BlastFilter
        Public Shared Function GetFilteredBlastSearchRecordsNearHitEnd5(ClonedAndFilteredBlastSearchRecords As List(Of BlastSearchRecord), NearEnd As Integer) As List(Of BlastSearchRecord)
            Dim res As New List(Of BlastSearchRecord)


            Try
                For Each bRecord In ClonedAndFilteredBlastSearchRecords
                    res.Add(bRecord)
                    For i1 = res.Last.Hits.Count - 1 To 0 Step -1
                        For i2 = res.Last.Hits(i1).Hsps.Count - 1 To 0 Step -1
                            If res.Last.Hits(i1).Hsps(i2).HitEnd + NearEnd > res.Last.Hits(i1).Length Or
                                res.Last.Hits(i1).Hsps(i2).HitStart + NearEnd > res.Last.Hits(i1).Length Then
                            Else
                                res.Last.Hits(i1).Hsps.RemoveAt(i2)
                            End If

                        Next ' end hsps
                        If res.Last.Hits(i1).Hsps.Count = 0 Then
                            res.Last.Hits.RemoveAt(i1)
                        End If

                    Next
                    If res.Last.Hits.Count = 0 Then
                        res.Remove(res.Last)
                    End If
                Next

            Catch ex As Exception
                Dim alf As Integer = 43
            End Try

            MsgBox(res.Count)

            Return res
        End Function
        Public Shared Function GetFilteredBlastSearchRecordsNearEnd(ClonedAndFilteredBlastSearchRecords As List(Of BlastSearchRecord), NearEnd As Integer) As List(Of BlastSearchRecord)
            Dim res As New List(Of BlastSearchRecord)

            Try
                For Each bRecord In ClonedAndFilteredBlastSearchRecords
                    res.Add(bRecord)
                    For i1 = res.Last.Hits.Count - 1 To 0 Step -1
                        For i2 = res.Last.Hits(i1).Hsps.Count - 1 To 0 Step -1
                            If res.Last.Hits(i1).Hsps(i2).QueryEnd + NearEnd < res.Last.IterationQueryLength Then
                                res.Last.Hits(i1).Hsps.RemoveAt(i2)
                            Else
                                If res.Last.Hits(i1).Hsps(i2).HitFrame = 1 Then
                                    If res.Last.Hits(i1).Hsps(i2).HitEnd + 20 > res.Last.Hits(i1).Length Then
                                        res.Last.Hits(i1).Hsps.RemoveAt(i2)
                                    End If
                                ElseIf res.Last.Hits(i1).Hsps(i2).HitFrame = -1 Then

                                    If res.Last.Hits(i1).Hsps(i2).HitEnd < 20 Then
                                        res.Last.Hits(i1).Hsps.RemoveAt(i2)
                                    End If
                                End If

                            End If
                        Next ' end hsps
                        If res.Last.Hits(i1).Hsps.Count = 0 Then
                            res.Last.Hits.RemoveAt(i1)
                        End If

                    Next
                    If res.Last.Hits.Count = 0 Then
                        res.Remove(res.Last)
                    End If
                Next

            Catch ex As Exception
                Dim alf As Integer = 43
            End Try

            MsgBox(res.Count)

            Return res
        End Function

        ''' <summary>
        ''' Filter Out Hsps which e-value is lower than
        ''' </summary>
        ''' <param name="ClonedAndFilteredBlastSearchRecords"></param>
        ''' <param name="evalue"></param>
        ''' <returns></returns>
        Public Shared Function GetFilteredBlastSearchRecordsbyEvalue(ClonedAndFilteredBlastSearchRecords As List(Of BlastSearchRecord),
                                                              evalue As Double) As List(Of Bio.Web.Blast.BlastSearchRecord)
            Dim res As New List(Of BlastSearchRecord)
            Try
                For Each bRecord In ClonedAndFilteredBlastSearchRecords
                    res.Add(bRecord)
                    For i1 = res.Last.Hits.Count - 1 To 0 Step -1
                        For i2 = res.Last.Hits(i1).Hsps.Count - 1 To 0 Step -1
                            If res.Last.Hits(i1).Hsps(i2).EValue > evalue Then
                                res.Last.Hits(i1).Hsps.RemoveAt(i2)
                            End If
                        Next
                        If res.Last.Hits(i1).Hsps.Count = 0 Then res.Last.Hits.RemoveAt(i1)
                    Next
                    If res.Last.Hits.Count = 0 Then res.Remove(res.Last)
                Next
            Catch ex As Exception
                Dim alf As Integer = 43
            End Try
            Return res
        End Function

        ''' <summary>
        ''' Filter Out Hsps where Alignment Length is Lower
        ''' </summary>
        ''' <param name="ClonedAndFilteredBlastSearchRecords"></param>
        ''' <returns></returns>
        Public Shared Function SelectRecorsWhereAligmentLengthIsHigherThen(ClonedAndFilteredBlastSearchRecords _
                                                                     As List(Of BlastSearchRecord)) As List(Of Bio.Web.Blast.BlastSearchRecord)
            Dim MinimalLengthOfMatch = Szunyi.MyInputBox.GetInteger("Minimal Length of Alignment")
            Dim res As New List(Of BlastSearchRecord)
            Try
                For Each bRecord In ClonedAndFilteredBlastSearchRecords
                    res.Add(bRecord)
                    For i1 = res.Last.Hits.Count - 1 To 0 Step -1
                        For i2 = res.Last.Hits(i1).Hsps.Count - 1 To 0 Step -1
                            If res.Last.Hits(i1).Hsps(i2).AlignmentLength < MinimalLengthOfMatch Then
                                res.Last.Hits(i1).Hsps.RemoveAt(i2)
                            End If
                        Next
                        If res.Last.Hits(i1).Hsps.Count = 0 Then res.Last.Hits.RemoveAt(i1)
                    Next
                    If res.Last.Hits.Count = 0 Then res.Remove(res.Last)
                Next
            Catch ex As Exception
                Dim alf As Integer = 43
            End Try

            Return res

        End Function

        ''' <summary>
        ''' Filter Out Self Hsps QueryDef = HitDef
        ''' </summary>
        ''' <param name="ClonedAndFilteredBlastSearchRecords"></param>
        ''' <returns></returns>
        Public Shared Function NotShowSelfHits(ClonedAndFilteredBlastSearchRecords As List(Of BlastSearchRecord)) As List(Of BlastSearchRecord)
            Dim t As New List(Of BlastSearchRecord)
            For Each Item In ClonedAndFilteredBlastSearchRecords
                t.Add(Item)
                For i1 = t.Last.Hits.Count - 1 To 0 Step -1
                    If t.Last.IterationQueryDefinition = t.Last.Hits(i1).Id Then
                        t.Last.Hits.RemoveAt(i1)
                    End If
                Next
                If t.Last.Hits.Count = 0 Then t.RemoveAt(t.Count - 1)
            Next
            Return t
        End Function

        ''' <summary>
        ''' Check the First HSP For Identity Percent
        ''' </summary>
        ''' <param name="clonedAndFilteredBlastSearchRecords"></param>
        ''' <returns></returns>
        Public Shared Function GetRecordsWithMinimalPercentofAlignemnt(clonedAndFilteredBlastSearchRecords As List(Of BlastSearchRecord)) As List(Of BlastSearchRecord)
            Dim Percent = Szunyi.MyInputBox.GetDouble("Select minimal percent identity of alignment")
            Dim t As New List(Of BlastSearchRecord)

            For Each Item In clonedAndFilteredBlastSearchRecords
                t.Add(Item)
                For i1 = t.Last.Hits.Count - 1 To 0 Step -1
                    For i2 = t.Last.Hits(i1).Hsps.Count - 1 To 0 Step -1
                        Dim HSP = t.Last.Hits(i1).Hsps(i2)
                        If HSP.IdentitiesCount / HSP.AlignmentLength < Percent / 100 Then
                            t.Last.Hits(i1).Hsps.RemoveAt(i2)
                        End If
                    Next
                    If t.Last.Hits(i1).Hsps.Count = 0 Then t.Last.Hits.RemoveAt(i1)
                Next
                If t.Last.Hits.Count = 0 Then t.RemoveAt(t.Count - 1)
            Next
            Return t
        End Function


        Public Shared Function Get_Records_Same_Lengths(clonedAndFilteredBlastSearchRecords As List(Of BlastSearchRecord), diffPercent As Double) As List(Of BlastSearchRecord)
            Dim t As New List(Of BlastSearchRecord)

            For Each Item In clonedAndFilteredBlastSearchRecords
                t.Add(Item)
                For i1 = t.Last.Hits.Count - 1 To 0 Step -1
                    For i2 = t.Last.Hits(i1).Hsps.Count - 1 To 0 Step -1

                        Dim HSP = t.Last.Hits(i1).Hsps(i2)
                        If (Item.IterationQueryLength / t.Last.Hits(i1).Length) * 100 > 100 - diffPercent And (Item.IterationQueryLength / t.Last.Hits(i1).Length) * 100 < 100 + diffPercent Then

                        Else
                            t.Last.Hits(i1).Hsps.RemoveAt(i2)
                        End If

                    Next
                    If t.Last.Hits(i1).Hsps.Count = 0 Then t.Last.Hits.RemoveAt(i1)
                Next
                If t.Last.Hits.Count = 0 Then t.RemoveAt(t.Count - 1)
            Next
            Return t
        End Function
#Region "By Hit Count"
        Public Shared Function GetRecordswoHits(BlastSearchRecords As List(Of BlastSearchRecord)) As List(Of Bio.Web.Blast.BlastSearchRecord)
            Dim res = From x In BlastSearchRecords Where x.Hits.Count = 0

            If res.Count > 0 Then Return res.ToList
            Return New List(Of Bio.Web.Blast.BlastSearchRecord)

        End Function

        ''' <summary>
        ''' Retrun Records Which Have Hits
        ''' </summary>
        ''' <param name="BlastSearchRecords"></param>
        ''' <returns></returns>
        Public Shared Function GetRecordswithHits(BlastSearchRecords As List(Of BlastSearchRecord)) _
            As List(Of Bio.Web.Blast.BlastSearchRecord)
            Dim res = From x In BlastSearchRecords Where x.Hits.Count > 0

            If res.Count > 0 Then Return res.ToList
            Return New List(Of Bio.Web.Blast.BlastSearchRecord)

        End Function

#End Region

#Region "Get Records wPerfect Hits Return All Hits"
        ''' <summary>
        ''' IdentityCount Of HSP = HitLength, Return All Hits
        ''' </summary>
        ''' <param name="clonedAndFilteredBlastSearchRecords"></param>
        ''' <returns></returns>
        Public Shared Function GetRecordswPerfectHits_Identity_Hit(clonedAndFilteredBlastSearchRecords As List(Of BlastSearchRecord)) As List(Of BlastSearchRecord)
            Dim res As New List(Of BlastSearchRecord)
            For Each bRecord In clonedAndFilteredBlastSearchRecords
                res.Add(bRecord)
                Dim HasPerfect As Boolean = False
                For i1 = res.Last.Hits.Count - 1 To 0 Step -1
                    For i2 = res.Last.Hits(i1).Hsps.Count - 1 To 0 Step -1
                        If res.Last.Hits(i1).Hsps(i2).IdentitiesCount = res.Last.Hits(i1).Length Then
                            HasPerfect = True
                        End If
                    Next
                Next
                If HasPerfect = False Then res.RemoveAt(res.Count - 1)
            Next
            Return res
        End Function

        ''' <summary>
        ''' IdentityCount Of HSP = QueryLength, Return All Hits
        ''' </summary>
        ''' <param name="clonedAndFilteredBlastSearchRecords"></param>
        ''' <returns></returns>
        Public Shared Function GetRecordswPerfectHits_Identity_Query(clonedAndFilteredBlastSearchRecords As List(Of BlastSearchRecord)) As List(Of BlastSearchRecord)
            Dim res As New List(Of BlastSearchRecord)
            For Each bRecord In clonedAndFilteredBlastSearchRecords
                res.Add(bRecord)
                Dim HasPerfect As Boolean = False
                For i1 = res.Last.Hits.Count - 1 To 0 Step -1
                    For i2 = res.Last.Hits(i1).Hsps.Count - 1 To 0 Step -1
                        If res.Last.Hits(i1).Hsps(i2).IdentitiesCount = bRecord.IterationQueryLength Then
                            HasPerfect = True
                        End If
                    Next
                Next
                If HasPerfect = False Then res.RemoveAt(res.Count - 1)
            Next
            Return res
        End Function

        ''' <summary>
        ''' IdentityCount of HSP = HitLength = QueryLength
        ''' </summary>
        ''' <param name="clonedAndFilteredBlastSearchRecords"></param>
        ''' <returns></returns>
        Public Shared Function GetRecordswPerfectHits_Identity_Query_Hit(clonedAndFilteredBlastSearchRecords As List(Of BlastSearchRecord)) As List(Of BlastSearchRecord)
            Dim res As New List(Of BlastSearchRecord)
            For Each bRecord In clonedAndFilteredBlastSearchRecords
                res.Add(bRecord)
                Dim HasPerfect As Boolean = False
                For i1 = res.Last.Hits.Count - 1 To 0 Step -1
                    For i2 = res.Last.Hits(i1).Hsps.Count - 1 To 0 Step -1
                        If res.Last.Hits(i1).Hsps(i2).IdentitiesCount = bRecord.IterationQueryLength = res.Last.Hits(i1).Length Then
                            HasPerfect = True
                        End If
                    Next
                Next
                If HasPerfect = False Then res.RemoveAt(res.Count - 1)
            Next
            Return res
        End Function
#End Region

#Region "Get Records WithOut Perfect Hits"
        ''' <summary>
        ''' Return Records WithOut Perfect Match IdentityCount = HitLength
        ''' </summary>
        ''' <param name="clonedAndFilteredBlastSearchRecords"></param>
        ''' <returns></returns>
        Public Shared Function GetRecordswoPerfectHits_Identity_Hit(clonedAndFilteredBlastSearchRecords As List(Of BlastSearchRecord)) As List(Of BlastSearchRecord)
            Dim res As New List(Of BlastSearchRecord)
            For Each bRecord In clonedAndFilteredBlastSearchRecords
                res.Add(bRecord)
                Dim HasPerfect As Boolean = False
                For i1 = res.Last.Hits.Count - 1 To 0 Step -1
                    For i2 = res.Last.Hits(i1).Hsps.Count - 1 To 0 Step -1
                        If res.Last.Hits(i1).Hsps(i2).IdentitiesCount = res.Last.Hits(i1).Length Then
                            HasPerfect = True
                        End If
                    Next
                Next
                If HasPerfect = True Then res.RemoveAt(res.Count - 1)
            Next
            Return res
        End Function

        ''' <summary>
        ''' Return Records WithOut Perfect Match IdentityCount=HitLength=QueryLength
        ''' </summary>
        ''' <param name="clonedAndFilteredBlastSearchRecords"></param>
        ''' <returns></returns>
        Public Shared Function GetRecordswoPerfectHits_Identity_Query_Hit(clonedAndFilteredBlastSearchRecords As List(Of BlastSearchRecord)) As List(Of BlastSearchRecord)
            Dim res As New List(Of BlastSearchRecord)
            For Each bRecord In clonedAndFilteredBlastSearchRecords
                res.Add(bRecord)
                Dim HasPerfect As Boolean = False
                For i1 = res.Last.Hits.Count - 1 To 0 Step -1
                    For i2 = res.Last.Hits(i1).Hsps.Count - 1 To 0 Step -1
                        If res.Last.Hits(i1).Hsps(i2).IdentitiesCount = res.Last.Hits(i1).Length And
                            res.Last.Hits(i1).Hsps(i2).IdentitiesCount = bRecord.IterationQueryLength Then
                            HasPerfect = True
                        End If
                    Next
                Next
                If HasPerfect = True Then res.RemoveAt(res.Count - 1)
            Next
            Return res
        End Function

        ''' <summary>
        ''' Reture Records WithOut Perfex Match IdentityCount = QueryLength
        ''' </summary>
        ''' <param name="clonedAndFilteredBlastSearchRecords"></param>
        ''' <returns></returns>
        Public Shared Function GetRecordswoPerfectHits_Identity_Query(clonedAndFilteredBlastSearchRecords As List(Of BlastSearchRecord)) As List(Of BlastSearchRecord)
            Dim res As New List(Of BlastSearchRecord)
            For Each bRecord In clonedAndFilteredBlastSearchRecords
                res.Add(bRecord)
                Dim HasPerfect As Boolean = False
                For i1 = res.Last.Hits.Count - 1 To 0 Step -1
                    For i2 = res.Last.Hits(i1).Hsps.Count - 1 To 0 Step -1
                        If res.Last.Hits(i1).Hsps(i2).IdentitiesCount = bRecord.IterationQueryLength Then
                            HasPerfect = True
                        End If
                    Next
                Next
                If HasPerfect = True Then res.RemoveAt(res.Count - 1)
            Next
            Return res
        End Function


#End Region

#Region "Remove Not Perfect Hits"
        ''' <summary>
        ''' Return Only Perfect Matches IdentityCount = HitLength
        ''' </summary>
        ''' <param name="BlastSearchRecords"></param>
        ''' <returns></returns>
        Public Shared Function RemoveNotPerfectMatchesByHitLength(BlastSearchRecords As List(Of BlastSearchRecord)) _
            As List(Of BlastSearchRecord)
            Dim res As New List(Of BlastSearchRecord)
            For Each bRecord In BlastSearchRecords
                res.Add(bRecord)
                Dim HasPerfect As Boolean = False
                For i1 = res.Last.Hits.Count - 1 To 0 Step -1
                    For i2 = res.Last.Hits(i1).Hsps.Count - 1 To 0 Step -1
                        If res.Last.Hits(i1).Hsps(i2).IdentitiesCount <> res.Last.Hits(i1).Length Then
                            res.Last.Hits(i1).Hsps.RemoveAt(i2)
                        End If
                    Next
                    If res.Last.Hits(i1).Hsps.Count = 0 Then
                        res.Last.Hits.RemoveAt(i1)
                    End If
                Next
                If res.Last.Hits.Count = 0 Then
                    res.RemoveAt(res.Count - 1)
                End If
            Next
            Return res
        End Function

        ''' <summary>
        ''' Return Only Perfect Matches IdentityCount = QueryLength
        ''' </summary>
        ''' <param name="BlastSearchRecords"></param>
        ''' <returns></returns>
        Public Shared Function RemoveNotPerfectMatchesByQueryLength(BlastSearchRecords As List(Of BlastSearchRecord)) _
            As List(Of BlastSearchRecord)
            Dim res As New List(Of BlastSearchRecord)
            For Each bRecord In BlastSearchRecords
                res.Add(bRecord)
                Dim HasPerfect As Boolean = False
                For i1 = res.Last.Hits.Count - 1 To 0 Step -1
                    For i2 = res.Last.Hits(i1).Hsps.Count - 1 To 0 Step -1
                        If res.Last.Hits(i1).Hsps(i2).IdentitiesCount <> bRecord.IterationQueryLength Then
                            res.Last.Hits(i1).Hsps.RemoveAt(i2)
                        End If
                    Next
                    If res.Last.Hits(i1).Hsps.Count = 0 Then
                        res.Last.Hits.RemoveAt(i1)
                    End If
                Next
                If res.Last.Hits.Count = 0 Then
                    res.RemoveAt(res.Count - 1)
                End If
            Next
            Return res
        End Function

        ''' <summary>
        ''' Return Only Perfect Matches IdentityCount = QueryLength = HitLength
        ''' </summary>
        ''' <param name="BlastSearchRecords"></param>
        ''' <returns></returns>
        Public Shared Function RemoveNotPerfectMatchesByHitLengthAndQueryLength(BlastSearchRecords As List(Of BlastSearchRecord)) _
            As List(Of BlastSearchRecord)
            Dim res As New List(Of BlastSearchRecord)
            For Each bRecord In BlastSearchRecords
                res.Add(bRecord)
                Dim HasPerfect As Boolean = False
                For i1 = res.Last.Hits.Count - 1 To 0 Step -1
                    For i2 = res.Last.Hits(i1).Hsps.Count - 1 To 0 Step -1
                        If res.Last.Hits(i1).Hsps(i2).IdentitiesCount <> res.Last.Hits(i1).Length Or
                            res.Last.Hits(i1).Hsps(i2).IdentitiesCount <> bRecord.IterationQueryLength Then
                            res.Last.Hits(i1).Hsps.RemoveAt(i2)
                        End If
                    Next
                    If res.Last.Hits(i1).Hsps.Count = 0 Then
                        res.Last.Hits.RemoveAt(i1)
                    End If
                Next
                If res.Last.Hits.Count = 0 Then
                    res.RemoveAt(res.Count - 1)
                End If
            Next
            Return res
        End Function

        Public Shared Function GetFilteredBlastSearchRecordsByHit(QueryString As String,
                                                                  BlastSearchRecords As List(Of BlastSearchRecord)) _
            As List(Of BlastSearchRecord)

            Dim res As New List(Of BlastSearchRecord)
            For Each bRecord In BlastSearchRecords
                res.Add(bRecord)
                Dim HasPerfect As Boolean = False
                For i1 = res.Last.Hits.Count - 1 To 0 Step -1
                    If res.Last.Hits(i1).Def.Contains(QueryString) = False AndAlso
                            res.Last.Hits(i1).Id.Contains(QueryString) = False Then
                        res.Last.Hits.RemoveAt(i1)
                    End If

                Next
                If res.Last.Hits.Count = 0 Then
                    res.RemoveAt(res.Count - 1)
                End If
            Next
            Return res
        End Function


#End Region

#Region "By Query Definiton"
        Public Shared Function Discard_Equal_Query_Definitions(BlastSearchRecords As List(Of BlastSearchRecord), query_IDs As List(Of String)) As List(Of BlastSearchRecord)
            Dim res As New List(Of BlastSearchRecord)
            For Each bRecord In BlastSearchRecords
                Dim Index = query_IDs.BinarySearch(bRecord.IterationQueryDefinition)
                If Index < 0 Then
                    res.Add(bRecord)
                End If
            Next
            Return res
        End Function

        Public Shared Function Discard_Contain_Query_Definitions(BlastSearchRecords As List(Of BlastSearchRecord), query_IDs As List(Of String)) As List(Of BlastSearchRecord)
            Dim res As New List(Of BlastSearchRecord)
            For Each bRecord In BlastSearchRecords
                If Is_Query_Definition_Contain_One_of_The_Id(bRecord, query_IDs) = False Then
                    res.Add(bRecord)
                End If
            Next
            Return res
        End Function

        Public Shared Function Discard_Start_With_Query_Definitions(BlastSearchRecords As List(Of BlastSearchRecord), query_IDs As List(Of String)) As List(Of BlastSearchRecord)
            Dim res As New List(Of BlastSearchRecord)
            For Each bRecord In BlastSearchRecords
                If Is_Query_Definition_Start_With_One_of_The_Id(bRecord, query_IDs) = False Then
                    res.Add(bRecord)
                End If
            Next
            Return res
        End Function


        Public Shared Function Maintain_Equal_With_Query_Definitions(BlastSearchRecords As List(Of BlastSearchRecord), query_IDs As List(Of String)) As List(Of BlastSearchRecord)
            Dim res As New List(Of BlastSearchRecord)
            For Each bRecord In BlastSearchRecords
                Dim Index = query_IDs.BinarySearch(bRecord.IterationQueryDefinition)
                If Index > -1 Then
                    res.Add(bRecord)
                End If
            Next
            Return res
        End Function

        Public Shared Function Maintain_Contain_With_Query_Definitions(BlastSearchRecords As List(Of BlastSearchRecord), query_IDs As List(Of String)) As List(Of BlastSearchRecord)
            Dim res As New List(Of BlastSearchRecord)
            For Each bRecord In BlastSearchRecords
                If Is_Query_Definition_Contain_One_of_The_Id(bRecord, query_IDs) = True Then
                    res.Add(bRecord)
                End If
            Next
            Return res
        End Function

        Public Shared Function Maintain_Start_With_Query_Definitions(BlastSearchRecords As List(Of BlastSearchRecord), query_IDs As List(Of String)) As List(Of BlastSearchRecord)
            Dim res As New List(Of BlastSearchRecord)
            For Each bRecord In BlastSearchRecords
                If Is_Query_Definition_Contain_One_of_The_Id(bRecord, query_IDs) = True Then
                    res.Add(bRecord)
                End If
            Next
            Return res
        End Function

        Public Shared Function Is_Query_Definition_Contain_One_of_The_Id(x As BlastSearchRecord, query_IDs As List(Of String)) As Boolean
            For Each Query_id In query_IDs
                If x.IterationQueryDefinition.IndexOf(Query_id, comparisonType:=StringComparison.InvariantCultureIgnoreCase) > -1 Then Return True
            Next
            Return False
        End Function
        Public Shared Function Is_Query_Definition_Start_With_One_of_The_Id(x As BlastSearchRecord, query_IDs As List(Of String)) As Boolean
            For Each Query_id In query_IDs
                If x.IterationQueryDefinition.IndexOf(Query_id, comparisonType:=StringComparison.InvariantCultureIgnoreCase) = 0 Then Return True
            Next
            Return False
        End Function

        Public Shared Function Remove_OverLapped_HSPs(clonedAndFilteredBlastSearchRecords As List(Of BlastSearchRecord), maxOverLap As Integer) As List(Of BlastSearchRecord)
            Dim t As New List(Of BlastSearchRecord)

            For Each R In clonedAndFilteredBlastSearchRecords
                For Each Hit In R.Hits
                    Dim HSPs = Hit.Hsps

                    Dim HSPsToRemove = Remove_OverLapped_HSPs(HSPs, maxOverLap)
                    For i1 = Hit.Hsps.Count - 1 To 0 Step -1
                        If HSPsToRemove.Contains(Hit.Hsps(i1)) = False Then
                            Hit.Hsps.RemoveAt(i1)
                        End If
                    Next


                Next
                For i1 = R.Hits.Count - 1 To 0 Step -1
                    If R.Hits(i1).Hsps.Count = 0 Then
                        R.Hits.RemoveAt(i1)
                    End If
                Next
                If R.Hits.Count > 0 Then
                    t.Add(R)
                End If
            Next

            Return t
        End Function
        Public Shared Function Maintain_Opposite_Hit_Frames(clonedAndFilteredBlastSearchRecords As List(Of BlastSearchRecord)) As List(Of BlastSearchRecord)
            Dim ToRemoveBS As New List(Of BlastSearchRecord)
            For Each R In clonedAndFilteredBlastSearchRecords
                Dim toRemove As New List(Of Hit)
                For Each Hit In R.Hits
                    Dim HASFw As Boolean = False
                    Dim HasRev As Boolean = False

                    For Each hsp In Hit.Hsps
                        If hsp.HitFrame < 0 Then
                            HasRev = True
                        Else
                            HASFw = True
                        End If
                    Next
                    If HASFw = False Or HasRev = False Then
                        toRemove.Add(Hit)
                    End If
                Next
                For Each Item In toRemove
                    R.Hits.Remove(Item)
                Next
                If R.Hits.Count = 0 Then ToRemoveBS.Add(R)
            Next
            For Each Item In ToRemoveBS
                clonedAndFilteredBlastSearchRecords.Remove(Item)
            Next
            Return clonedAndFilteredBlastSearchRecords
        End Function
        Public Shared Function Remove_Single_HSPs(clonedAndFilteredBlastSearchRecords As List(Of BlastSearchRecord)) As List(Of BlastSearchRecord)
            Dim t As New List(Of BlastSearchRecord)
            Dim ToRemoveBS As New List(Of BlastSearchRecord)
            For Each R In clonedAndFilteredBlastSearchRecords
                Dim ToRemove As New List(Of Hit)
                For Each Hit In R.Hits
                    If Hit.Hsps.Count = 1 Then ToRemove.Add(Hit)
                Next
                For Each t1 In ToRemove
                    R.Hits.Remove(t1)
                Next
                If R.Hits.Count = 0 Then ToRemoveBS.Add(R)
            Next
            For Each Item In ToRemoveBS
                clonedAndFilteredBlastSearchRecords.Remove(Item)
            Next
            Return clonedAndFilteredBlastSearchRecords
        End Function
        Private Shared Function Remove_OverLapped_HSPs(HSPs As IList(Of Hsp), maxOverLap As Integer) As IList(Of Hsp)
            Dim out As New List(Of Hsp)
            If HSPs.Count = 0 Then Return HSPs
            Dim cHSPs = (From x In HSPs Order By x.QueryStart Ascending).ToList
            Dim toRemove As New List(Of Integer)
            For i1 = 0 To HSPs.Count - 1
                For i2 = 0 To HSPs.Count - 1
                    If i1 < i2 Then
                        If cHSPs(i1).QueryEnd > cHSPs(i2).QueryStart AndAlso (cHSPs(i1).QueryEnd - cHSPs(i2).QueryStart) >= maxOverLap Then
                            toRemove.Add(i1)
                            toRemove.Add(i2)
                        End If
                    End If
                Next
            Next
            If toRemove.Count = 0 Then Return HSPs

            toRemove = toRemove.Distinct.ToList
            toRemove.Sort()
            toRemove.Reverse()
            For Each Item In toRemove
                cHSPs.RemoveAt(Item)
            Next
            If cHSPs.Count > 1 Then
                Dim kj As Int16 = 54
            End If
            Return cHSPs
        End Function
#End Region
    End Class
End Namespace

