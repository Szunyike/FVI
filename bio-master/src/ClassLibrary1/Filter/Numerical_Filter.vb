Imports ClassLibrary1.Szunyi.Constants.Operators
Namespace Szunyi
    Namespace Filter
        Public Class Numerical_Filter
            Public Shared Function IsGood(TestValue As Double, Filter As Numerical_Filter_Setting) As Boolean
                Try
                    Dim d As Double = TestValue
                    Select Case Filter.The_Operator
                        Case Constants.Operators.Equal
                            Return d = Filter.Value
                        Case Constants.Operators.Greater_Than
                            Return d > Filter.Value
                        Case Constants.Operators.Greater_Than_Or_Equal
                            Return d >= Filter.Value
                        Case Constants.Operators.Less_Than
                            Return d < Filter.Value
                        Case Constants.Operators.Less_than_Or_Equal
                            Return d <= Filter.Value
                        Case Constants.Operators.Not_Equal
                            Return d <> Filter.Value
                        Case Constants.Operators.Between
                            If d >= Filter.Value And d <= Filter.Value2 Then
                                Return True
                            Else
                                Return False
                            End If
                    End Select
                Catch ex As Exception
                    Return False
                End Try
                Return True
            End Function
            Public Shared Function IsGood(TestValue As String, Filters As List(Of Numerical_Filter_Setting)) As Boolean
                Dim d As Double
                If TestValue = String.Empty Then Return False
                Try
                    d = TestValue
                Catch ex As Exception
                    Return False
                End Try
                For Each Filt In Filters
                    If IsGood(d, Filt) = False Then Return False

                Next
                Return True
            End Function
            Public Shared Function Get_Strings(Test_Values As List(Of String), Filters As List(Of Numerical_Filter_Setting)) As List(Of String)
                Dim out As New List(Of String)

                For Each Test_Value In Test_Values
                    out.Add(Get_String_From_String(Test_Value, Filters))
                Next
                Return out
            End Function
            Public Shared Function Get_Strings(Test_Values As List(Of Double), Filters As List(Of Numerical_Filter_Setting)) As List(Of String)
                Dim out As New List(Of String)

                For Each Test_Value In Test_Values
                    out.Add(Get_String_From_Double(Test_Value, Filters))
                Next
                Return out
            End Function
            Public Shared Function Get_String_From_Double(Test_Value As Double, Filters As List(Of Numerical_Filter_Setting)) As String

                Dim out As String = String.Empty
                For Each Filt In Filters
                    If IsGood(Test_Value, Filt) = True Then
                        out = out & Filt.Result_String
                    End If
                Next

                Return out
            End Function

            Public Shared Function Get_String_From_String(Test_Value As String, Filters As List(Of Numerical_Filter_Setting)) As String
                Dim d As Double
                Dim out As String = String.Empty
                If Test_Value = String.Empty Then Return String.Empty
                Try
                    d = Test_Value
                Catch ex As Exception
                    Return String.Empty
                End Try
                For Each Filt In Filters
                    If IsGood(d, Filt) = True Then
                        out = out & Filt.Result_String
                    End If

                Next
                Return out
            End Function
        End Class
        Public Class Numerical_Filter_Setting
            Public The_Operator As ClassLibrary1.Szunyi.Constants.Operators
            Public Property Value As Double
            Public Property Value2 As Double
            Public Valid As Boolean = True
            Public Property Result_String As String = "test"
            Public Sub New(The_Operator As Szunyi.Constants.Operators, Min As Double, Max As Double, Optional String_Output As String = "")
                Me.Result_String = String_Output
                Me.The_Operator = The_Operator
                Value = Min
                Value2 = Max

            End Sub

            Public Sub New(s As String, Optional String_Output As String = "")
                Me.Result_String = String_Output
                If s.Contains(",") = True Then
                    Dim s1() = Split(s, ",")
                    If s1.Count = 2 Then '

                    End If
                End If
                If s.Contains("<=") Then
                    Me.The_Operator = Less_than_Or_Equal
                ElseIf s.Contains("=<") Then
                    Me.The_Operator = Less_than_Or_Equal
                ElseIf s.Contains(">=") Then
                    Me.The_Operator = Greater_Than_Or_Equal
                ElseIf s.Contains("=>") Then
                    Me.The_Operator = Greater_Than_Or_Equal
                ElseIf s.Contains(">") Then
                    Me.The_Operator = Greater_Than
                ElseIf s.Contains("<") Then
                    Me.The_Operator = Less_Than
                ElseIf s.Contains("=") Then
                    Me.The_Operator = Equal
                ElseIf s.Contains("<>") Then
                    Me.The_Operator = Not_Equal
                Else
                    Me.Valid = False
                End If

                Try
                    Me.Value = s.Replace(">", "").Replace("<", "").Replace("=", "").Replace(" ", "")
                    Me.Valid = True
                Catch ex As Exception
                    Me.Valid = False
                End Try

                If Me.Valid = False Then
                        MsgBox("Error in parsing" & vbCrLf & s)

                Else

                End If
                Me.Result_String = "tes"
            End Sub

        End Class
    End Namespace
End Namespace

