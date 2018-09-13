Imports System.Drawing

Namespace Szunyi
    Public Class Number
        Public Shared Function GetRanges(vals() As Integer) As List(Of Point)
            If IsNothing(vals) = True Then Return New List(Of Point)
            Dim valsII As List(Of Integer) = vals.ToList
            valsII.Insert(0, 0)
            Dim out As New List(Of Point)
            For i1 = 0 To valsII.Count - 2
                out.Add(New Point(valsII(i1) + 1, valsII(i1 + 1)))
            Next
            Return out
        End Function

        Public Shared Function Which_Range(Ranges As List(Of Point), count As Integer) As String
            If Ranges.Count = 0 Then Return String.Empty
            Dim pt = From x In Ranges Where x.X <= count And x.Y >= count
            If pt.Count <> 0 Then
                Return pt.First.X & "-" & pt.First.Y
            Else
                Return "Mt_" & Ranges.Last.Y
            End If
        End Function
        Public Shared Function Get_Median(numbers As List(Of Double)) As Double
            Dim numberCount As Integer = numbers.Count
            Dim halfIndex As Integer = numbers.Count \ 2
            Dim sortedNumbers = numbers.OrderBy(Function(n) n)
            Dim median As Double
            If (numberCount Mod 2 = 0) Then
                median = (sortedNumbers.ElementAt(halfIndex) + sortedNumbers.ElementAt(halfIndex - 1)) / 2
            Else
                median = sortedNumbers.ElementAt(halfIndex)
            End If
            Return median
        End Function
        Public Shared Function Get_Median(numbers As List(Of Long)) As Double
            If numbers.Count = 0 Then Return -1
            Dim numberCount As Integer = numbers.Count
            Dim halfIndex As Integer = numbers.Count \ 2
            Dim sortedNumbers = numbers.OrderBy(Function(n) n)
            Dim median As Double
            If (numberCount Mod 2 = 0) Then
                median = (sortedNumbers.ElementAt(halfIndex) + sortedNumbers.ElementAt(halfIndex - 1)) / 2
            Else
                median = sortedNumbers.ElementAt(halfIndex)
            End If
            Return median
        End Function
        Public Shared Function Get_Median(numbers As List(Of Integer)) As Double
            Dim numberCount As Integer = numbers.Count
            Dim halfIndex As Integer = numbers.Count \ 2
            Dim sortedNumbers = numbers.OrderBy(Function(n) n)
            Dim median As Double
            If (numberCount Mod 2 = 0) Then
                median = (sortedNumbers.ElementAt(halfIndex) + sortedNumbers.ElementAt(halfIndex - 1)) / 2
            Else
                median = sortedNumbers.ElementAt(halfIndex)
            End If
            Return median
        End Function
    End Class
End Namespace

