Namespace Szunyi
    Public Class MyInputBox
        Friend Shared Function GetDouble(Title As String) As Double
            Dim s = InputBox(Title)
            Try
                Dim d As Double = s
                Return d
            Catch ex As Exception
                If s.Contains(".") Then
                    s = s.Replace(".", ",")
                    Try
                        Dim d As Double = s
                        Return d
                    Catch ex1 As Exception
                        Return Nothing
                    End Try
                ElseIf s.Contains(",") Then
                    s = s.Replace(",", ".")
                    Try
                        Dim d As Double = s
                        Return d
                    Catch ex1 As Exception
                        Dim alf As Int16 = 43
                        Return Nothing
                    End Try
                End If
                Return Nothing
            End Try
        End Function

        Friend Shared Function GetInteger(Title As String) As Integer
            Dim s = InputBox(Title)
            Try
                Dim i As Integer = s
                Return i
            Catch ex As Exception

            End Try
            Return Nothing
        End Function
    End Class
End Namespace

