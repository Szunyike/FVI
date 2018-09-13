Namespace Szunyi
    Namespace Basic
        Public Class Percents

            Public Property Min As Double
            Public Property Max As Double
            Public Property NofItem As Integer
            Public Ranges As New List(Of Double)
            Public Sub New(min As Double, max As Double, NofItem As Integer)
                Me.Min = min
                Me.Max = max
                Me.NofItem = NofItem
                Create_Ranges()
            End Sub
            Public Sub New(Ranges() As Double)
                Me.Min = Ranges.First
                Me.Max = Ranges.Last
                Me.Ranges = Ranges.ToList
            End Sub
            Public Sub New()
                Do
                    Dim s = InputBox("Enter Min, Max values and Nof Item separated by spcae or Ranges")
                    Try
                        Dim s1() = Split(s)
                        If s1.Count = 3 Then
                            Me.Min = s1(0)
                            Me.Max = s1(1)
                            Me.NofItem = s1(2)
                            Create_Ranges()
                        ElseIf s1.Count = 1 Then ' FOr Quantile
                            Me.NofItem = s1.First
                            For i1 = 0 To Me.NofItem
                                Me.Ranges.Add(i1)
                            Next
                        Else
                            For Each Item In s1
                                Me.Ranges.Add(Item)
                            Next
                        End If

                        Exit Do
                    Catch ex As Exception
                        Me.Ranges.Clear()
                        MsgBox("Bad Values")
                    End Try
                Loop

            End Sub
            Private Sub Create_Ranges()
                Dim Range As Double = (Me.Max - Me.Min) / Me.NofItem
                Me.Ranges.Add(Min)
                For i1 = 0 To NofItem - 1
                    Me.Ranges.Add(Ranges.Last + Range)
                Next
                Me.Ranges(Me.Ranges.Count - 1) = Max
            End Sub


            Public Function Get_Ranges() As String
                Dim str As New System.Text.StringBuilder
                For i1 = 0 To Me.Ranges.Count - 2
                    str.Append(Me.Ranges(i1))
                    str.Append("-")
                    str.Append(Me.Ranges(i1 + 1))
                    str.Append(vbTab)
                Next
                If str.Length > 0 Then str.Length -= 1
                Return str.ToString
            End Function
            Public Function Get_Labels(Percents As List(Of Double)) As List(Of String)

                Dim out As New List(Of String)
                For i = 0 To Me.Ranges.Count - 2
                    out.Add(Percents(i) & ":" & Me.Ranges(i) & "-" & Me.Ranges(i + 1))
                Next

                Return out
            End Function
            Public Function Get_Labels_From_True_Or_False(Percents As List(Of Double)) As List(Of String)

                Dim out As New List(Of String)
                out.Add(Percents.First & ":No Peaks")
                out.Add(Percents.Last & ":Has Peaks")

                Return out
            End Function

        End Class

        Public Class Percents_With_LocusTagID
            Public Property Name As String
            Public Property LocusTagIDs As New List(Of Integer)
            Public Sub New(Name As String)
                Me.Name = Name
            End Sub
        End Class
    End Namespace
End Namespace

