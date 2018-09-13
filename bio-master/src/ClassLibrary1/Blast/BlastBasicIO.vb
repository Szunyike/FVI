Imports System.IO
Imports Bio.Web.Blast
Imports ClassLibrary1.Szunyi.Sequences.SequenceManipulation
Imports ClassLibrary1.Szunyi.Features.FeatureManipulation

Namespace Szunyi
    Namespace Blast
        Public Class BlastBasicIO
            ''' <summary>
            ''' Return Sorted List Of Blast Query Sequences By IDs
            ''' </summary>
            ''' <param name="FileNames"></param>
            ''' <param name="BlastRecords"></param>
            ''' <returns></returns>
            Public Shared Function GetQuerySeqs(FileNames As List(Of String),
                                                BlastRecords As List(Of BlastSearchRecord),
                                                log As System.Text.StringBuilder) _
                As List(Of Bio.ISequence)

                Dim QueryIDs = BlastManipulation.GetUniqueQueryDefintions(BlastRecords)
                Dim QueryFiles = GetQueryFiles(FileNames)
                Return GetSelectedSeqsFromFiles(QueryFiles, QueryIDs, log)
            End Function

            ''' <summary>
            ''' Return Sorted List Of Blast Hit Sequences By IDs
            ''' </summary>
            ''' <param name="Files"></param>
            ''' <param name="BlastRecords"></param>
            ''' <returns></returns>
            Public Shared Function GetHitSeqsFromBlastDBs(Files As List(Of System.IO.FileInfo),
                                              BlastRecords As List(Of BlastSearchRecord),
                                              log As System.Text.StringBuilder) As List(Of Bio.ISequence)
                Dim Out As New List(Of Bio.ISequence)
                For Each File In Files
                    Out.AddRange(GetHitSeqsDFromBlastDB(File, BlastRecords, log))
                Next
                Return Out.Distinct.ToList

            End Function
            ''' <summary>
            ''' Return Sorted List Of Blast Hit Full Sequences By IDs
            ''' </summary>
            ''' <param name="File"></param>
            ''' <param name="BlastRecords"></param>
            ''' <returns></returns>
            Public Shared Function GetHitSeqsDFromBlastDB(File As FileInfo,
                                              BlastRecords As List(Of BlastSearchRecord),
                                              log As System.Text.StringBuilder) As List(Of Bio.ISequence)
                Dim HitIDs = BlastManipulation.GetUniqueHitIDs(BlastRecords)
                Dim DbFile = BlastBasicIO.GetDatabaseFile(File)
                Return GetSelectedSeqsFromBlastDbFile(DbFile, HitIDs, log)

            End Function
            ''' <summary>
            ''' Return Sorted List Of Blast Hit Sequences By IDs
            ''' </summary>
            ''' <param name="Seqs"></param>
            ''' <param name="BlastRecords"></param>
            ''' <returns></returns>
            Public Shared Function GetSeqs(Seqs As List(Of Bio.ISequence), BlastRecords As List(Of BlastSearchRecord)) As List(Of Bio.Sequence)

                Dim Out As New List(Of Bio.Sequence)
                For Each BlastSearchRecord In BlastRecords
                    For Each Hit In BlastSearchRecord.Hits
                        For Each Hsp In Hit.Hsps
                            Dim TheSeq = (From x In Seqs Where x.ID = Hit.Def).First

                            Out.Add(TheSeq)
                        Next
                    Next
                Next

                Dim lout = Out.Distinct.ToList
                Return lout
            End Function


            ''' <summary>
            ''' Return ORFs Amino Acid Sequence List 
            ''' </summary>
            ''' <param name="Seqs"></param>
            ''' <param name="BlastRecords"></param>
            ''' <param name="log"></param>
            ''' <param name="BlastProgramName"></param>
            ''' <returns></returns>
            Public Shared Function GetORFs(Seqs As List(Of Bio.ISequence),
                                           BlastRecords As List(Of BlastSearchRecord),
                                           log As System.Text.StringBuilder,
                                           BlastProgramName As String) As List(Of Bio.ISequence)
                Dim tmpSeqs = Szunyi.Sequences.SequenceManipulation.ID.Rename(Seqs, Szunyi.Constants.StringRename.FirstAfterSplit, " ")
                tmpSeqs.Sort(Szunyi.Comparares.AllComparares.BySeqID)
                Dim Used As New HashSet(Of String)
                Dim Out As New List(Of Bio.ISequence)
                Dim tmpSeq As New Bio.Sequence(Bio.Alphabets.AmbiguousDNA, "A")
                For Each BlastSearchRecord In BlastRecords
                    For Each Hit In BlastSearchRecord.Hits
                        Dim TheSeq As Bio.Sequence = Nothing
                        For Each Hsp In Hit.Hsps
                            Dim Key = Hit.Id & ":" & Hsp.HitFrame
                            If Used.Contains(Key) = False Then
                                Used.Add(Key)
                                If IsNothing(TheSeq) = True Then
                                    tmpSeq.ID = Hit.Id
                                    Dim Index = tmpSeqs.BinarySearch(tmpSeq, ClassLibrary1.Szunyi.Comparares.AllComparares.BySeqID)
                                    If Index > -1 Then
                                        Dim t = GetORF(tmpSeqs(Index), Hsp, BlastProgramName)
                                        If IsNothing(t) = False Then
                                            If t.Count = 0 Then
                                                log.Append("Not Found:" & Hit.Id & vbCrLf)
                                            End If
                                            Out.AddRange(t)
                                        End If

                                    Else
                                        log.Append("Not Found:" & Hit.Id & vbCrLf)
                                    End If
                                End If

                            End If

                        Next
                    Next
                Next
                Return Out
            End Function


            Public Shared Function GetORFs(Seqs As List(Of Bio.ISequence),
                                           BlastResult As BlastResult,
                                           log As System.Text.StringBuilder) As List(Of Bio.ISequence)
                Dim tmpSeqs = Szunyi.Sequences.SequenceManipulation.ID.Rename(Seqs, Szunyi.Constants.StringRename.FirstAfterSplit, " ")
                tmpSeqs.Sort(Szunyi.Comparares.AllComparares.BySeqID)
                Dim Used As New HashSet(Of String)
                Dim Out As New List(Of Bio.ISequence)
                Dim tmpSeq As New Bio.Sequence(Bio.Alphabets.AmbiguousDNA, "A")
                For Each BlastSearchRecord In BlastResult.Records
                    For Each Hit In BlastSearchRecord.Hits
                        Dim TheSeq As Bio.Sequence = Nothing
                        For Each Hsp In Hit.Hsps
                            Dim Key = Hit.Id & ":" & Hsp.HitFrame
                            If Used.Contains(Key) = False Then
                                Used.Add(Key)
                                If IsNothing(TheSeq) = True Then
                                    tmpSeq.ID = Hit.Id
                                    Dim Index = tmpSeqs.BinarySearch(tmpSeq, ClassLibrary1.Szunyi.Comparares.AllComparares.BySeqID)
                                    If Index > -1 Then
                                        Dim t = GetORF(Seqs(Index), Hsp, BlastResult.Metadata.Program)
                                        If t.Count = 0 Then
                                            Dim alf As Int16 = 54
                                        End If
                                        Out.AddRange(t)
                                    Else
                                        log.Append("Not Found:" & Hit.Id & vbCrLf)
                                    End If
                                End If

                            End If

                        Next
                    Next
                Next
                If log.Length > 0 Then
                    '  MsgBox(log.ToString)
                End If
                Return Out
            End Function

            Public Shared Function GetORF(Seq As Bio.Sequence, hsp As Bio.Web.Blast.Hsp, BlastProgram As String) As List(Of Bio.ISequence)
                Dim out As New List(Of Bio.ISequence)
                Select Case BlastProgram
                    Case "tblastn"
                        Dim AASeq = Szunyi.Translate.TranaslateSeq(Seq, hsp.HitFrame)

                        Dim LongestHitORF = AA.GetORFS(New Bio.Sequence(Bio.Alphabets.AmbiguousProtein, hsp.HitSequence)).First
                        Dim LongestHitOrf_WithOut_Insertion_Symbols = Szunyi.Sequences.SequenceManipulation.Common.RemoveInsertionSymbols(LongestHitORF)

                        Dim AASeqs As List(Of Bio.ISequence) = Szunyi.Sequences.SequenceManipulation.AA.GetORFS(AASeq)
                        Dim t = Szunyi.Sequences.SequenceManipulation.SelectBy.GetSeqContains(AASeqs, LongestHitOrf_WithOut_Insertion_Symbols)
                        If t.Count > 0 Then Return t
                        AASeqs = Szunyi.Sequences.SequenceManipulation.AA.GetORFsFromBeginning(AASeq)
                        t = Szunyi.Sequences.SequenceManipulation.SelectBy.GetSeqContains(AASeqs, LongestHitOrf_WithOut_Insertion_Symbols)
                        Return t
                    Case "blastn"

                    Case "blastp"

                        out.Add(Seq)

                End Select
                Return out

            End Function

            ''' <summary>
            ''' Return Sorted List Of Blast Hit Sequences By IDs using Blastcmd
            ''' </summary>
            ''' <param name="FileName"></param>
            ''' <param name="BlastRecords"></param>
            ''' <returns></returns>
            Public Shared Function GetHitSeqs(FileName As String, BlastRecords As List(Of BlastSearchRecord)) As List(Of Bio.ISequence)
                Dim HitIDs = BlastManipulation.GetUniqueHitDefinitions(BlastRecords)
                Dim DatabaseFile = BlastBasicIO.GetDatabaseFile(FileName)
                Return GetSelectedSeqsFromFile(DatabaseFile, HitIDs)

            End Function

            ''' <summary>
            ''' Return Sorted List Of Sequences By IDs
            ''' </summary>
            ''' <param name="FileNames"></param>
            ''' <param name="IDs"></param>
            ''' <returns></returns>
            Private Shared Function GetSelectedSeqsFromFiles(FileNames As List(Of String),
                                                             IDs As List(Of String),
                                                             log As System.Text.StringBuilder) As List(Of Bio.ISequence)
                Dim Files = Szunyi.IO.Files.Filter.GetFilesFromFileNames(FileNames)
                Dim Seqs As List(Of Bio.ISequence) = Szunyi.IO.Import.Sequence.FromFiles(Files)
                Dim SelectedSeqs = GetSequences.ByIDs(Seqs, IDs)
                SelectedSeqs.Sort(Comparares.AllComparares.BySeqID)
                Return SelectedSeqs

            End Function
            Private Shared Function GetSelectedSeqsFromFile(File As System.IO.FileInfo, IDs As List(Of String)) As List(Of Bio.ISequence)

                Dim Seqs As List(Of Bio.ISequence) = Szunyi.IO.Import.Sequence.FromFile(File)
                Dim SelectedSeqs = GetSequences.ByIDs(Seqs, IDs)
                SelectedSeqs.Sort(Comparares.AllComparares.BySeqID)
                Return SelectedSeqs

            End Function
            ''' <summary>
            ''' Return Sorted List Of Sequences By IDs
            ''' </summary>
            ''' <param name="Files"></param>
            ''' <param name="IDs"></param>
            ''' <returns></returns>
            Private Shared Function GetSelectedSeqsFromFiles(Files As List(Of System.IO.FileInfo),
                                                             IDs As List(Of String),
                                                             log As System.Text.StringBuilder) As List(Of Bio.ISequence)

                Dim Seqs = Szunyi.IO.Import.Sequence.FromFiles(Files)
                Dim SelectedSeqs = GetSequences.ByIDs(Seqs, IDs)
                SelectedSeqs.Sort(Comparares.AllComparares.BySeqID)
                Return SelectedSeqs

            End Function

            Public Shared Function GetSelectedSeqsFromBlastDbFile(DataBaseFile As FileInfo,
                                                                  HitIDs As List(Of String),
                                                                  log As System.Text.StringBuilder) As List(Of Bio.ISequence)
                ' 1 Write IDs To a File
                ' 1/1 Create Fileinof and Delete If it is existing

                Dim tmpFile As New FileInfo(My.Resources.BlastDbPath & "2.tmp")
                If tmpFile.Exists = True Then tmpFile.Delete()
                Szunyi.IO.Export.SaveText(Szunyi.Text.General.GetText(HitIDs) & vbCrLf, tmpFile)
                Dim h As New Szunyi.Blast.Console.Retrive

                Dim Reader = h.GetSeqsFromBlastDatabase(DataBaseFile, tmpFile, log)
                Dim out As New List(Of Bio.ISequence)
                Dim fa As New Bio.IO.FastA.FastAParser()
                Reader.Position = 0
                Try
                    For Each Seq In fa.Parse(Reader)
                        Seq.ID = Seq.ID.Replace("lcl|", "").Trim
                        out.Add(Seq)
                    Next
                Catch ex As Exception
                    MsgBox(log.ToString)
                End Try
                If HitIDs.Count <> out.Count Then
                    Dim IDs = Szunyi.Sequences.SequenceManipulation.Properties.GetIDs(out)

                    Dim dif = HitIDs.Except(IDs)
                    log.Append("Missing Sequences:").AppendLine()
                    log.Append(Szunyi.Text.General.GetText(dif.ToList, " ")).AppendLine()

                End If
                out.Sort(Szunyi.Comparares.AllComparares.BySeqID)
                Return out
            End Function

            ''' <summary>
            ''' Split By Space(It is used to Separate Database And Query FileName)
            ''' And return the Last Filenames(QueryFileNames)
            ''' </summary>
            ''' <param name="FileNames"></param>
            ''' <returns></returns>

            Public Shared Function GetQueryFiles(FileNames As List(Of String)) As List(Of System.IO.FileInfo)
                Dim out As New List(Of System.IO.FileInfo)
                For Each FileName In FileNames
                    out.Add(GetQueryFile(FileName))
                Next
                Return out
            End Function

            ''' <summary>
            ''' Split By Space(It is used to Separate Database And Query FileName)
            ''' And return the Last Filenames(QueryFileNames)
            ''' </summary>
            ''' <param name="Files"></param>
            ''' <returns></returns>
            Public Shared Function GetQueryFiles(FIles As List(Of FileInfo)) As List(Of System.IO.FileInfo)
                Dim out As New List(Of System.IO.FileInfo)
                For Each FIle In FIles
                    out.Add(GetQueryFile(FIle.Name))
                Next
                Return out
            End Function

            ''' <summary>
            ''' Split By Space(It is used to Separate Database And Query FileName)
            ''' And return the Last Filenames(QueryFileNames)
            ''' </summary>
            ''' <param name="FileName"></param>
            ''' <returns></returns>
            Public Shared Function GetQueryFile(FileName As String) As System.IO.FileInfo

                Return New System.IO.FileInfo(My.Resources.BlastFastaFilesPath & Split(FileName, " ").Last.Replace(".xml", ""))


            End Function


            ''' <summary>
            ''' Split By Space(It is used to Separate Database And Query FileName)
            ''' And return the First Filenames(Database FileNames)
            ''' </summary>
            ''' <param name="Files"></param>
            ''' <returns></returns>
            Public Shared Function GetDatabaseFiles(Files As List(Of FileInfo)) As List(Of System.IO.FileInfo)
                Dim out As New List(Of System.IO.FileInfo)
                For Each File In Files
                    out.Add(GetDatabaseFile(File))
                Next
                Return out
            End Function

            ''' <summary>
            ''' Return Nothing Or BlastDatabaseFile
            ''' </summary>
            ''' <param name="File"></param>
            ''' <returns></returns>
            Public Shared Function GetDatabaseFile(File As FileInfo) As FileInfo
                If IsNothing(File) = True Then Return Nothing
                Return GetDatabaseFile(File.Name)
            End Function

            Public Shared Function GetDatabaseFile(FileName As String) As FileInfo
                Dim s = Split(FileName, " ")
                Return New FileInfo(My.Resources.BlastDbPath & s.First)
            End Function
            ''' <summary>
            ''' Return The Last Part after splitted Space
            ''' </summary>
            ''' <param name="FileName"></param>
            ''' <returns></returns>

            Public Shared Function GetHitFileFastaFile(FileName As String) As System.IO.FileInfo
                Dim tmp = Split(FileName, " ").First
                tmp = tmp.Split("\").Last
                Return New System.IO.FileInfo(My.Resources.BlastFastaFilesPath & "\" & tmp)
            End Function

            ''' <summary>
            ''' Return The Last Part after splitted Space
            ''' </summary>
            ''' <param name="File"></param>
            ''' <returns></returns>
            Public Shared Function GetHitFileFastaFile(File As FileInfo) As System.IO.FileInfo
                Dim tmp = Split(File.Name, " ").First
                tmp = tmp.Split("\").Last
                Return New System.IO.FileInfo(My.Resources.BlastFastaFilesPath & "\" & tmp)
            End Function
        End Class

    End Namespace
End Namespace

