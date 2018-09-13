
Imports Bio.Web.Blast

Class SearchForDoubles

    Private _bResults As List(Of BlastSearchRecord)
    Private _MinNofCommon As Integer
    Private _Pool As List(Of String())
    Sub New(BResults As List(Of BlastSearchRecord), MinNofHits As Integer, MaxNofHits As Integer, MinNofCommon As Integer, Pool As List(Of String()))

        ' TODO: Complete member initialization 
        _MinNofCommon = MinNofCommon
        _bResults = BResults
        _Pool = Pool
        _bResults = Helper.RemoveHSpswLowPercent(_bResults, 0.8)
        _bResults = Helper.GetBlastRecordwHits(MinNofHits, MaxNofHits, BResults)

        Dim jj As Dictionary(Of String, Dictionary(Of String, List(Of String))) = GetResult()
        Dim res As Dictionary(Of String, Dictionary(Of String, List(Of String))) = GetFiltered(jj)
        Dim FinalResult As String = GetFinalResult(res)

    End Sub
    Private Function GetNameFromPoolTable(p1 As String, p2 As String) As String
        Dim t = GetPoolName(p1)
        Dim r As Integer
        Dim c As Integer
        If t.Last = "R" Then
            r = Helper.GetFirstNumberFromString(t)
        ElseIf t.Last = "C" Then
            c = Helper.GetFirstNumberFromString(t)
        Else
            Return String.Empty
        End If
        t = GetPoolName(p2)
        If t.Last = "R" Then
            r = Helper.GetFirstNumberFromString(t)
        ElseIf t.Last = "C" Then
            c = Helper.GetFirstNumberFromString(t)
        Else
            Return String.Empty
        End If
        Return Me._Pool(r)(c)
    End Function
    Private Function GetPoolName(x As String) As String
        Return Split(x, "_").First
    End Function

    Private Function GetResult() As Dictionary(Of String, Dictionary(Of String, List(Of String)))
        Dim jj As New Dictionary(Of String, Dictionary(Of String, List(Of String)))
        For Each Item In _bResults
            Dim HitPoolNames As New List(Of String)
            Dim Key As String = GetPoolName(Item.IterationQueryDefinition)
            If jj.ContainsKey(Key) = False Then jj.Add(Key, New Dictionary(Of String, List(Of String)))
            For Each HitName In Item.Hits
                HitPoolNames.Add(GetPoolName(HitName.Def))
            Next
            HitPoolNames.Sort()
            Dim jh = Helper.GetTextFromListOfStringUniqeOrdered(HitPoolNames, vbCrLf)
            If jj(Key).ContainsKey(jh) = False Then jj(Key).Add(jh, New List(Of String))
            jj(Key)(jh).Add(Item.IterationQueryDefinition)
        Next
        Return jj
    End Function

    Private Function GetFiltered(jj As Dictionary(Of String, Dictionary(Of String, List(Of String)))) As Dictionary(Of String, Dictionary(Of String, List(Of String)))
        Dim res As New Dictionary(Of String, Dictionary(Of String, List(Of String)))
        Dim str As New System.Text.StringBuilder
        For Each PoolName In jj
            For Each Commons In PoolName.Value
                If Commons.Value.Count >= _MinNofCommon Then
                    If res.ContainsKey(PoolName.Key) = False Then res.Add(PoolName.Key, New Dictionary(Of String, List(Of String)))
                    If res(PoolName.Key).ContainsKey(Commons.Key) = False Then res(PoolName.Key).Add(Commons.Key, New List(Of String))
                    res(PoolName.Key)(Commons.Key).AddRange(Commons.Value)
                    str.Append(PoolName.Key).AppendLine()
                    str.Append(Commons.Key).AppendLine()
                    str.Append(Helper.GetStringFromListOfString(Commons.Value)).AppendLine.AppendLine()
                End If
            Next
        Next

        Return res
    End Function

    Private Function GetFinalResult(res As Dictionary(Of String, Dictionary(Of String, List(Of String)))) As String
        Dim out As New System.Text.StringBuilder
        For Each item In res
            For Each commons In item.Value
                out.Append(item.Key.Replace(vbCrLf, "")).Append(vbTab)
                For Each Values In commons.Value
                    out.Append(Values).AppendLine()
                Next
                Dim s = commons.Key.Split(vbCrLf)
                For Each s1 In s
                    If s1 <> vbCrLf Then
                        Dim t = GetNameFromPoolTable(item.Key, s1)
                        If t.Length > 0 Then
                            out.Append(s1).AppendLine()
                            out.Append(t).AppendLine()
                        End If
                    End If
                Next
            Next
        Next
        Return out.ToString
    End Function

End Class
