
Imports Bio.Web.Blast

Namespace Szunyi
    Namespace Blast
        Public Class BlastManipulation

            Public Sub New()

            End Sub
            Public Shared Function Get_All_HSPs_Near_End(Hits As List(Of Hit), Distance As Integer) As List(Of Hsp)
                Dim out As New List(Of Hsp)

                For Each Hit In Hits
                    Dim res = From x In Hit.Hsps Where x.HitStart < Distance Or x.HitEnd < Distance Or x.HitEnd + Distance > Hit.Length Or x.HitStart + Distance > Hit.Length

                    If res.Count > 0 Then
                        out.AddRange(res.ToList)
                    End If
                Next
                Return out
            End Function

            Public Shared Function Get_All_Hits(read As KeyValuePair(Of BlastSearchRecord, Dictionary(Of String, List(Of Hit)))) As List(Of Hit)
                Dim out As New List(Of Hit)
                For Each v In read.Value
                    out.AddRange(v.Value)
                Next
                Return out
            End Function
            Public Shared Function Get_All_HSPsII(read As KeyValuePair(Of BlastSearchRecord, Dictionary(Of String, List(Of Hit)))) As List(Of Hsp)
                Dim out As New List(Of Hsp)
                For Each v In read.Value
                    out.AddRange(Get_All_HSPs(v.Value))
                Next
                Return out
            End Function
            Public Shared Function Get_All_HSPs(Hits As List(Of Hit)) As List(Of Hsp)
                Dim out As New List(Of Hsp)
                For Each Hit In Hits
                    out.AddRange(Hit.Hsps)
                Next
                Return out
            End Function
            Public Shared Function Get_All_HSPs(bRecords As List(Of BlastSearchRecord)) As List(Of Hsp)
                Dim out As New List(Of Hsp)
                For Each bRecord In bRecords
                    out.AddRange(Get_All_HSPs(bRecord.Hits.ToList))
                Next

                Return out
            End Function
            Public Shared Function Clone(ls As List(Of BlastSearchRecord))
                Dim out As New List(Of BlastSearchRecord)
                For Each Item In ls
                    Dim r As BlastSearchRecord = CloneRecord(Item)
                    For Each Hit In CloneHits(Item.Hits)
                        r.Hits.Add(Hit)
                    Next
                    out.Add(r)
                Next
                Return out
            End Function

            Private Shared Function CloneHits(hits As IList(Of Hit)) As IList(Of Hit)
                Dim out As New List(Of Bio.Web.Blast.Hit)
                For Each Item In hits
                    Dim Hit As New Bio.Web.Blast.Hit
                    Hit.Accession = Item.Accession
                    Hit.Def = Item.Def
                    Hit.Id = Item.Id
                    Hit.Length = Item.Length
                    For Each hsp In cloneHsps(Item.Hsps)
                        Hit.Hsps.Add(hsp)
                    Next

                    out.Add(Hit)
                Next
                Return out
            End Function

            Private Shared Function cloneHsps(hsps As List(Of Hsp)) As List(Of Hsp)
                Dim out As New List(Of Hsp)
                For Each Item In hsps
                    Dim HSP As New Hsp
                    HSP.AlignmentLength = Item.AlignmentLength
                    HSP.BitScore = Item.BitScore
                    HSP.Density = Item.Density
                    HSP.EValue = Item.EValue
                    HSP.Gaps = Item.Gaps
                    HSP.HitEnd = Item.HitEnd
                    HSP.HitFrame = Item.HitFrame
                    HSP.HitSequence = Item.HitSequence
                    HSP.HitStart = Item.HitStart
                    HSP.IdentitiesCount = Item.IdentitiesCount
                    HSP.Midline = Item.Midline
                    HSP.PatternFrom = Item.PatternFrom
                    HSP.PatternTo = Item.PatternTo
                    HSP.PositivesCount = Item.PositivesCount
                    HSP.QueryEnd = Item.QueryEnd
                    HSP.QueryFrame = Item.QueryFrame
                    HSP.QuerySequence = Item.QuerySequence
                    HSP.QueryStart = Item.QueryStart
                    HSP.Score = Item.Score
                    out.Add(HSP)
                Next
                Return out
            End Function

            Private Shared Function CloneRecord(item As BlastSearchRecord) As BlastSearchRecord
                Dim r As New BlastSearchRecord
                r.IterationMessage = item.IterationMessage
                r.IterationNumber = item.IterationNumber
                r.IterationQueryDefinition = item.IterationQueryDefinition
                r.IterationQueryId = item.IterationQueryId
                r.IterationQueryLength = item.IterationQueryLength
                r.Metadata = New Bio.Web.Blast.BlastXmlMetadata
                r.Metadata.Database = item.Metadata.Database
                r.Metadata.ParameterEntrezQuery = item.Metadata.ParameterEntrezQuery
                r.Metadata.ParameterExpect = item.Metadata.ParameterExpect
                r.Metadata.ParameterFilter = item.Metadata.ParameterFilter

                r.Metadata.ParameterGapExtend = item.Metadata.ParameterGapExtend
                r.Metadata.ParameterGapOpen = item.Metadata.ParameterGapOpen
                r.Metadata.ParameterInclude = item.Metadata.ParameterInclude
                r.Metadata.ParameterMatchScore = item.Metadata.ParameterMatchScore
                r.Metadata.ParameterMatrix = item.Metadata.ParameterMatrix
                r.Metadata.ParameterMismatchScore = item.Metadata.ParameterMismatchScore
                r.Metadata.ParameterPattern = item.Metadata.ParameterPattern
                r.Metadata.Program = item.Metadata.Program
                r.Metadata.QueryDefinition = item.Metadata.QueryDefinition

                r.Metadata.QueryId = item.Metadata.QueryId
                r.Metadata.QueryLength = item.Metadata.QueryLength
                r.Metadata.QuerySequence = item.Metadata.QuerySequence
                r.Metadata.Reference = item.Metadata.Reference
                r.Metadata.Version = item.Metadata.Version
                r.Statistics = New Bio.Web.Blast.BlastStatistics
                r.Statistics.DatabaseLength = item.Statistics.DatabaseLength
                r.Statistics.EffectiveSearchSpace = item.Statistics.EffectiveSearchSpace
                r.Statistics.Entropy = item.Statistics.Entropy
                r.Statistics.HspLength = item.Statistics.HspLength
                r.Statistics.Kappa = item.Statistics.Kappa
                r.Statistics.Lambda = item.Statistics.Lambda
                r.Statistics.SequenceCount = item.Statistics.SequenceCount
                Return r
            End Function

            ''' <summary>
            ''' Return Perfect, or Which One is Longer
            ''' </summary>
            ''' <param name="Record"></param>
            ''' <returns></returns>
            Public Shared Function GetHSpDescription(Record As BlastSearchRecord) As String

                Dim TheHit = Record.Hits.First
                Dim TheHsp = Record.Hits.First.Hsps.First
                If TheHsp.IdentitiesCount = Record.IterationQueryLength Then
                    If TheHsp.IdentitiesCount = TheHit.Length Then
                        Return "Perfect"
                    Else
                        Return "Hit is Longer"
                    End If
                Else
                    If TheHsp.IdentitiesCount = TheHit.Length Then
                        Return "Query is Longer"
                    End If
                End If
                Return "MisMatches"
            End Function

            ''' <summary>
            ''' Return Unique QueryDefinitions as List Of String
            ''' </summary>
            ''' <param name="BlastRecords"></param>
            ''' <returns></returns>
            Public Shared Function GetUniqueQueryDefintions(BlastRecords As List(Of BlastSearchRecord)) As List(Of String)
                Dim out As New List(Of String)
                For Each BlastRecord In BlastRecords
                    out.Add(BlastRecord.IterationQueryDefinition)
                Next
                Return out.Distinct.ToList
            End Function

            ''' <summary>
            ''' Return All Hit Definitions
            ''' </summary>
            ''' <param name="blastRecords"></param>
            ''' <returns></returns>
            Public Shared Function GetAllHitDefinitions(blastRecords As List(Of BlastSearchRecord)) As List(Of String)
                Dim out As New List(Of String)
                For Each BlastRecord In blastRecords
                    For Each Hit In BlastRecord.Hits
                        out.Add(Hit.Def)
                    Next
                Next
                Return out
            End Function

            ''' <summary>
            ''' Return Unique Hit Definitions
            ''' </summary>
            ''' <param name="blastRecords"></param>
            ''' <returns></returns>
            Public Shared Function GetUniqueHitDefinitions(blastRecords As List(Of BlastSearchRecord)) As List(Of String)
                Dim out As New List(Of String)
                For Each BlastRecord In blastRecords
                    For Each Hit In BlastRecord.Hits
                        out.Add(Hit.Def)
                    Next
                Next
                Return out.Distinct.ToList
            End Function
            ''' <summary>
            ''' Return Unique Hit IDs
            ''' </summary>
            ''' <param name="blastRecords"></param>
            ''' <returns></returns>
            Public Shared Function GetUniqueHitIDs(blastRecords As List(Of BlastSearchRecord)) As List(Of String)
                Dim out As New List(Of String)
                For Each BlastRecord In blastRecords
                    For Each Hit In BlastRecord.Hits
                        out.Add(Hit.Id)
                    Next
                Next
                Return out.Distinct.ToList
            End Function

            ''' <summary>
            ''' Return All Query Definitons
            ''' </summary>
            ''' <param name="bRecords"></param>
            ''' <returns></returns>
            Public Shared Function GetAllQueryDefintions(bRecords As List(Of BlastSearchRecord)) As List(Of String)
                Dim out As New List(Of String)
                For Each bRecord In bRecords
                    out.Add(bRecord.IterationQueryDefinition)
                Next
                Return out
            End Function

            ''' <summary>
            ''' Return The BlastSearchRecords Order By QueryDefinition
            ''' </summary>
            ''' <param name="BlastSearchResults"></param>
            ''' <returns></returns>
            Public Shared Function GetRecords(BlastSearchResults As List(Of BlastResult)) As List(Of BlastSearchRecord)
                Dim x As New List(Of BlastSearchRecord)
                For Each BResult In BlastSearchResults
                    If IsNothing(BResult) = False Then
                        x.AddRange(BResult.Records)
                    End If

                Next
                Dim res = From t In x Order By t.IterationQueryDefinition Select t

                Return res.ToList
            End Function

            Public Shared Function ConvertHspS(hSPs As List(Of Hsp)) As List(Of Hsp)
                Dim out As New List(Of Hsp)
                For Each hsp In hSPs
                    If hsp.QueryFrame > -1 Then
                        out.Add(hsp)
                    Else
                        Dim x As New Hsp()
                        x.QueryEnd = hsp.QueryStart
                        x.QueryStart = hsp.QueryStart
                        out.Add(x)
                    End If
                Next
                Return out
            End Function
        End Class
    End Namespace
End Namespace

