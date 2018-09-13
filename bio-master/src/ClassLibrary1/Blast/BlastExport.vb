Imports System.IO
Imports Bio.Web.Blast
Imports ClassLibrary1.Szunyi.Sequences.SequenceManipulation

Namespace Szunyi
    Namespace Blast

        Public Class Export
            Public Shared Sub Query_Definitions(bRecords As List(Of BlastSearchRecord))
                Dim QueryIDs = BlastManipulation.GetUniqueQueryDefintions(bRecords)
                Szunyi.IO.Export.SaveText(Szunyi.Text.General.GetText(QueryIDs, vbCrLf))
            End Sub
            Public Shared Sub Query_Sequences(bRecords As List(Of BlastSearchRecord), Files As List(Of FileInfo))
                Dim QueryIDs = BlastManipulation.GetUniqueQueryDefintions(bRecords)
                Dim QueryFiles = BlastBasicIO.GetQueryFiles(Files)
                Dim Seqs = Szunyi.IO.Import.Sequence.FromFiles(QueryFiles)
                Dim SelectedSeqs = GetSequences.ByIDs(Seqs, QueryIDs)
                Dim t1 = Szunyi.Sequences.SequenceManipulation.SeqsToString.GetSeqsAsFasta(SelectedSeqs)
                Dim out = Szunyi.Text.General.GetText(t1)
                If out.Length = 0 Then Exit Sub
                System.Windows.Forms.Clipboard.SetText(out)
                '      Szunyi.IO.Export.SaveSequencesToSingleFasta(SelectedSeqs)
            End Sub
            Public Shared Sub Hit_IDs(bRecords As List(Of BlastSearchRecord))
                Dim Hit_IDs = BlastManipulation.GetUniqueHitIDs(bRecords)
                Szunyi.IO.Export.SaveText(Szunyi.Text.General.GetText(Hit_IDs, vbCrLf))
            End Sub
            Public Shared Sub Hit_Sequences(bRecords As List(Of BlastSearchRecord), Opened_Files As List(Of FileInfo))
                Dim Hit_IDs = BlastManipulation.GetUniqueHitIDs(bRecords)
                Dim DatabaseFiles = BlastBasicIO.GetDatabaseFiles(Opened_Files)
                Dim log As New System.Text.StringBuilder
                Dim HitSeqs = BlastBasicIO.GetHitSeqsFromBlastDBs(DatabaseFiles, bRecords, log)

                Dim All_HspS = BlastManipulation.Get_All_HSPs(bRecords)
                Dim res = From x In All_HspS Select x.HitSequence

                Dim re = res.Distinct.ToList
                Dim str As New System.Text.StringBuilder
                For Each r In re
                    str.Append(r).AppendLine()
                Next

                Szunyi.IO.Export.SaveSequencesToSingleFasta(HitSeqs)
            End Sub
            ''' <summary>
            ''' TDT HitID, NofHit
            ''' Optionally UniqueHits Come From Original Records, so
            ''' It can show No Hits also
            ''' </summary>
            Public Shared Sub HitswCoverage(BlastRecords As List(Of BlastSearchRecord),
                                            Optional FullBlastSearchRecord As List(Of BlastSearchRecord) = Nothing)
                Dim AllHits = BlastManipulation.GetAllHitDefinitions(BlastRecords)
                Dim UniqueHits As List(Of String)
                If IsNothing(FullBlastSearchRecord) = True Then
                    UniqueHits = BlastManipulation.GetUniqueHitDefinitions(BlastRecords)
                Else
                    UniqueHits = BlastManipulation.GetUniqueHitDefinitions(FullBlastSearchRecord)
                End If
                Dim str As New System.Text.StringBuilder
                str.Append("HitDef").Append(vbTab).Append("Count").AppendLine()
                For Each Hit In UniqueHits
                    Dim Count = (From x In AllHits Where x = Hit).Count
                    str.Append(Hit).Append(vbTab).Append(Count).AppendLine()
                Next
                str.Length -= 2
                Szunyi.IO.Export.SaveText(str.ToString)
            End Sub

            ''' <summary>
            ''' Export only the Best hit in tdt format
            ''' </summary>
            ''' <param name="clonedAndFilteredBlastSearchRecords"></param>
            Public Shared Sub Best_Hits(clonedAndFilteredBlastSearchRecords As List(Of BlastSearchRecord))
                Dim str As New System.Text.StringBuilder
                str.Append("QueryID").Append(vbTab).Append("Hit Accession").Append(vbTab).Append("Hit Definition").Append(vbTab)
                str.Append("Query Length").Append(vbTab).Append("Hit Length").Append(vbTab)
                str.Append("(First Hsp Identity*2/(Query length + Hit Length)*100").Append(vbTab).Append("Match Type").AppendLine()
                For Each Record In clonedAndFilteredBlastSearchRecords
                    If Record.Hits.Count <> 0 Then
                        str.Append(Record.IterationQueryDefinition).Append(vbTab)
                        str.Append(Record.Hits.First.Id).Append(vbTab)
                        str.Append(Record.Hits.First.Def).Append(vbTab)
                        str.Append(Record.IterationQueryLength).Append(vbTab)
                        str.Append(Record.Hits.First.Length).Append(vbTab)
                        str.Append(((Record.Hits.First.Hsps.First.IdentitiesCount * 2) /
                            (Record.Hits.First.Length + Record.IterationQueryLength)) * 100).Append(vbTab)
                        str.Append(Szunyi.Blast.BlastManipulation.GetHSpDescription(Record)).AppendLine()

                    End If
                Next
                Szunyi.IO.Export.SaveText(str.ToString)
            End Sub
            Public Shared Sub Get_Mismatches(clonedAndFilteredBlastSearchRecords As List(Of BlastSearchRecord))
                Dim HitIDs = Blast.BlastManipulation.GetUniqueHitIDs(clonedAndFilteredBlastSearchRecords)
                Dim x As New Dictionary(Of String, Integer)
                Dim Identites As New Dictionary(Of String, Integer)
                For Each HitID In HitIDs
                    x.Add(HitID, 0)
                    Identites.Add(HitID, 0)
                Next
                Dim DNADiff As New Dictionary(Of String, Dictionary(Of String, List(Of String)))
                Dim AADiff As New Dictionary(Of String, Dictionary(Of String, List(Of String)))

                For Each Record In clonedAndFilteredBlastSearchRecords
                    DNADiff.Add(Record.IterationQueryDefinition, New Dictionary(Of String, List(Of String)))
                    For Each hit In Record.Hits
                        DNADiff(Record.IterationQueryDefinition).Add(hit.Id, New List(Of String))
                        For Each hsp In hit.Hsps

                            For i1 = 0 To hsp.QuerySequence.Count - 1
                                If hsp.QuerySequence(i1) <> hsp.HitSequence(i1) Then
                                    DNADiff(Record.IterationQueryDefinition)(hit.Id).Add(i1 + 1 & hsp.QuerySequence(i1) & " To " & hsp.HitSequence(i1))
                                End If
                            Next

                        Next
                    Next
                Next
                Dim str As New System.Text.StringBuilder
                For Each Gene In DNADiff
                    '   Dim res = From x1 In Gene Select x1 Order By x1.key
                    Dim alf As Int16 = 54
                    Dim res = From x1 In Gene.Value Order By x1.Key Ascending

                    If str.Length = 0 Then
                        str.Append(Gene.Key)
                        For Each Item In res
                            str.Append(vbTab).Append(Item.Key)
                        Next
                        str.AppendLine()
                    Else
                        str.Append(Gene.Key)

                        str.AppendLine()
                    End If

                    For Each Item In res
                        str.Append(vbTab).Append(Szunyi.Text.General.GetText(Item.Value, ":"))
                    Next
                    str.AppendLine()
                    Dim kja As Int16 = 54
                Next
                Dim st = Szunyi.Text.General.GetText(x)
                Dim st2 = Szunyi.Text.General.GetText(Identites)
                If st2 <> "" Then Windows.Forms.Clipboard.SetText(st2)
            End Sub

            ''' <summary>
            ''' Export only the Best hit in tdt format
            ''' </summary>
            ''' <param name="clonedAndFilteredBlastSearchRecords"></param>
            Public Shared Sub All_Hits(clonedAndFilteredBlastSearchRecords As List(Of BlastSearchRecord))
                Dim str As New System.Text.StringBuilder
                str.Append("QueryID").Append(vbTab).Append("Hit Accession").Append(vbTab).Append("Hit Definition").Append(vbTab)
                str.Append("Query Length").Append(vbTab).Append("Hit Length").Append(vbTab)
                str.Append("(First Hsp Identity*2/(Query length + Hit Length)*100").Append(vbTab).Append("Match Type").AppendLine()
                For Each Record In clonedAndFilteredBlastSearchRecords
                    For Each Hit In Record.Hits
                        str.Append(Record.IterationQueryDefinition).Append(vbTab)
                        str.Append(Hit.Id).Append(vbTab)
                        str.Append(Hit.Def).Append(vbTab)
                        str.Append(Record.IterationQueryLength).Append(vbTab)
                        str.Append(Hit.Length).Append(vbTab)
                        str.Append(((Hit.Hsps.First.IdentitiesCount * 2) /
                                    (Hit.Length + Record.IterationQueryLength)) * 100).Append(vbTab)
                        str.Append(Szunyi.Blast.BlastManipulation.GetHSpDescription(Record)).AppendLine()
                    Next
                Next
                Szunyi.IO.Export.SaveText(str.ToString)

            End Sub
            Public Shared Sub As_GFF(clonedAndFilteredBlastSearchRecords As List(Of BlastSearchRecord))
                Dim str As New System.Text.StringBuilder
                Dim str2 As New System.Text.StringBuilder
                str.Append(Get_GFF_Header)
                str2.Append(Get_GFF_Header)
                For Each Item In clonedAndFilteredBlastSearchRecords
                    For Each Hit In Item.Hits
                        Dim TheHSP = From x In Hit.Hsps Order By x.AlignmentLength Descending
                        str2.Append(Get_HSP(TheHSP.First, Hit, Item))

                        For Each hsp In Hit.Hsps
                            str.Append(Get_HSP(hsp, Hit, Item))
                        Next
                    Next
                Next
                Windows.Forms.Clipboard.SetText(str2.ToString)
                str.Length -= 2

                Szunyi.IO.Export.SaveText(str.ToString)
            End Sub
            ''' <summary>
            ''' Hit ID, Item ID, HitStart, HitEnd, Frame
            ''' </summary>
            ''' <param name="hsp"></param>
            ''' <param name="Hit"></param>
            ''' <param name="Item"></param>
            ''' <returns></returns>
            Public Shared Function Get_HSP(hsp As Bio.Web.Blast.Hsp,
                                           Hit As Bio.Web.Blast.Hit, Item As Bio.Web.Blast.BlastSearchRecord) As String
                Dim str As New System.Text.StringBuilder
                str.Append(Hit.Id).Append(vbTab)
                str.Append(Item.IterationQueryDefinition).Append(vbTab)
                str.Append(hsp.HitStart).Append(vbTab)
                str.Append(hsp.HitEnd).Append(vbTab)
                If hsp.HitFrame > 0 Then
                    str.Append("+").Append(vbTab)
                    str.Append(hsp.QueryStart).Append(vbTab)
                    str.Append(hsp.QueryEnd).AppendLine()
                Else
                    str.Append("-").Append(vbTab)
                    str.Append(hsp.QueryStart).Append(vbTab)
                    str.Append(hsp.QueryEnd).AppendLine()
                End If
                Return str.ToString
            End Function
            Public Shared Function Get_GFF_Header() As String
                Dim str As New System.Text.StringBuilder
                str.Append("Seq").Append(vbTab)
                str.Append("locus_tag").Append(vbTab)
                str.Append("Hit Start").Append(vbTab)
                str.Append("Hit End").Append(vbTab)
                str.Append("Strand").Append(vbTab)
                str.Append("Query Start").Append(vbTab)
                str.Append("Query End").Append(vbTab)
                str.AppendLine()
                Return str.ToString
            End Function
        End Class


    End Namespace
End Namespace

