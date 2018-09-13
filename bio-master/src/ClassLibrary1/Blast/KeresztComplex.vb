Imports System.IO
Imports Bio.Web.Blast


Public Class KeresztComplex
    Dim Path As IO.DirectoryInfo
    Dim XmlFiles As List(Of FileInfo)
    Dim PoolFile As List(Of FileInfo)
    Dim BResults As List(Of BlastSearchRecord)
    Dim ContigswPools As New Dictionary(Of String, List(Of String))
    Dim COntigwHits As New Dictionary(Of String, List(Of Hit))
    Dim DictOfBlastSearchRecord As New Dictionary(Of String, BlastSearchRecord)
    Dim Pool As List(Of String())
    Dim Seqs As Dictionary(Of String, Bio.Sequence)
    Public Sub New()

        Path = Szunyi.IO.Files.getDirectory("d:\Work\Tnt-Mere\")

        If IsNothing(Path) = True Then Exit Sub
        Dim AllFIles = Path.GetFiles
        XmlFiles = Szunyi.IO.Files.GetFilesByExtension(".xml", AllFIles.ToList)
        PoolFile = Helper.GetFilesByExtension(".txt", AllFIles.ToList)
        BResults = Helper.GetBlastSearchRecords(XmlFiles)
        BResults = Helper.GetBlastRecordwHits(1, BResults)
        DictOfBlastSearchRecord = Helper.GetDictionaryOfBlastSearchRecord(BResults)
        Pool = GetPoolTable()
        Dim FastaFiles = Helper.GetFilesByExtension(".fa", AllFIles.ToList)
        Seqs = Helper.GetSeqs(FastaFiles.ToList)
        ' Simple()
        Analyse()
        SearchForDOuble()
    End Sub
    Private Sub Analyse()
        Dim alf = Helper.GetBlastRecordwHits(2, BResults)
        Dim alf2 = Helper.GetBlastRecordwHits(2, 4, alf)
        ContigswPools.Clear()
        For Each Result In alf2
            Dim sContigName = GetShortContigName(Result.IterationQueryDefinition)
            Dim x = GetPoolName(Result.IterationQueryDefinition)
            If ContigswPools.ContainsKey(x) = False Then ContigswPools.Add(x, New List(Of String))
            For Each Hit In Result.Hits
                ContigswPools(x).Add(GetPoolName(Hit.Def))
            Next
        Next
        Dim k As New Dictionary(Of String, List(Of String))
        For Each Result In alf2
            Dim s As String = Helper.GetSortedHitsDef(Result.Hits)
            If k.ContainsKey(s) = False Then k.Add(s, New List(Of String))
            k(s).Add(Result.IterationQueryDefinition)
        Next
        Dim gh = (From j In k Where j.Value.Count > 1 Order By j.Value.Count Descending).ToList
        Dim jj As New Dictionary(Of String, Dictionary(Of String, List(Of String)))
        For Each Item In alf2
            Dim t As New List(Of String)
            Dim Key As String = GetPoolName(Item.IterationQueryDefinition)
            If jj.ContainsKey(Key) = False Then jj.Add(Key, New Dictionary(Of String, List(Of String)))
            For Each HitName In Item.Hits
                t.Add(GetPoolName(HitName.Def))
            Next
            t.Sort()
            Dim jh = Helper.GetTextFromListOfStringUniqeOrdered(t, vbCrLf)
            If jj(Key).ContainsKey(jh) = False Then jj(Key).Add(jh, New List(Of String))
            jj(Key)(jh).Add(Item.IterationQueryDefinition)
        Next
        Dim lll As New System.Text.StringBuilder
        For Each item In jj
            For Each sItem In item.Value
                Dim k2 = Split(sItem.Key.Remove(sItem.Key.Length - 2), vbCrLf)
                If k2.Length > 2 Then

                End If

            Next


        Next
        Dim None As New System.Text.StringBuilder
        Dim Simple As New System.Text.StringBuilder
        Dim SimpleWmoreHitFromSame As New System.Text.StringBuilder
        Dim Complex As New System.Text.StringBuilder
        Dim nNone, nSimple, NSimpleWmoreHits, nComplex As Integer
        For Each Result In BResults
            Dim sContigName = GetShortContigName(Result.IterationQueryDefinition)
            '   ContigswPools.Add(sContigName, New List(Of String))
            If Result.Hits.Count = 0 Then
                nNone += 1
                None.Append(sContigName).AppendLine()
            ElseIf Result.Hits.Count = 1 Then
                nSimple += 1
                Simple.Append(sContigName).Append(vbTab).Append(GetShortContigName(Result.Hits.First.Def)).AppendLine()
            Else
                Dim HitPool As New List(Of String)
                Dim IsComplex As Boolean = False
                For Each Hit In Result.Hits
                    Dim s = GetPoolName(Hit.Def)
                    If HitPool.Contains(s) = False Then
                        HitPool.Add(s)
                    Else
                        HitPool.Add(s)
                        IsComplex = True
                    End If
                Next
                If IsComplex = True Then
                    Complex.Append(sContigName)
                    For Each Item In HitPool
                        Complex.Append(vbTab).Append(Item)
                    Next
                    Complex.AppendLine()
                    nComplex += 1
                Else
                    SimpleWmoreHitFromSame.Append(sContigName)
                    For Each Item In HitPool
                        SimpleWmoreHitFromSame.Append(vbTab).Append(Item)
                    Next
                    SimpleWmoreHitFromSame.AppendLine()
                    NSimpleWmoreHits += 1
                End If
            End If
        Next
    End Sub
#Region "Search For Double Lines"
    Private Sub SearchForDOuble()
        Dim x As New SearchForDoubles(Me.BResults, 2, 4, 3, Me.Pool)

    End Sub
#End Region


    Private Sub Simple()
        Dim Out As New Dictionary(Of String, List(Of Bio.ISequence))
        For Each Result In BResults
            If Result.Hits.Count = 1 Then
                Dim NameOfTheLine = GetNameFromPoolTable(Result.IterationQueryDefinition, Result.Hits.First.Def)
                If Out.ContainsKey(NameOfTheLine) = False Then Out.Add(NameOfTheLine, New List(Of Bio.ISequence))
                Out(NameOfTheLine).Add(Me.Seqs(Result.IterationQueryDefinition))

                Out(NameOfTheLine).Add(Me.Seqs(Result.Hits.First.Def))
            End If
        Next
        Helper.SaveSequencesIntoDictionary(Out)

    End Sub
    
    Private Function GetShortContigName(x As String) As String
        Dim s = x.Replace("_(single)_trimmed_contig_", "c")
        Return Split(s, " ").First
    End Function
    Private Function GetPoolName(x As String) As String


        Return Split(x, "_").First
    End Function
    Private Function GetPoolTable() As List(Of String())
        Dim File = Me.PoolFile.First
        Dim Table As New List(Of String())
        Using sr As New StreamReader(File.FullName)

            Do
                Dim line = Split(sr.ReadLine, vbTab)
                Table.Add(line)
            Loop Until sr.EndOfStream = True
        End Using
        Return Table
    End Function

    Private Function GetNameFromPoolTable(p1 As String, p2 As String) As String
        Dim t = GetPoolName(p1)
        Dim r As Integer
        Dim c As Integer
        If t.Last = "R" Then
            r = Helper.GetFirstNumberFromString(t)
        ElseIf t.Last = "C" Then
            c = Helper.GetFirstNumberFromString(t)
        Else
            Dim alf As Int16 = 43
        End If
        t = GetPoolName(p2)
        If t.Last = "R" Then
            r = Helper.GetFirstNumberFromString(t)
        ElseIf t.Last = "C" Then
            c = Helper.GetFirstNumberFromString(t)
        Else
            Dim alf As Int16 = 43
        End If
        Return Me.Pool(r)(c)
    End Function
  
End Class
