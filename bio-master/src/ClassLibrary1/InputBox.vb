Namespace Szunyi
    Public Class MyInputBox
        Public Shared Function GetDouble(Title As String) As Double
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

        ''' <summary>
        ''' Return Integer Or Nothing 
        ''' </summary>
        ''' <param name="Title"></param>
        ''' <returns></returns>
        Public Shared Function GetInteger(Title As String, Optional txt As String = "") As Integer
            Try
                Dim i As Integer = InputBox(Title,, txt)
                Return i
            Catch ex As Exception
                Return Nothing
            End Try
        End Function

        ''' <summary>
        ''' Return Integer Or Nothing 
        ''' </summary>
        ''' <param name="Title"></param>
        ''' <returns></returns>
        Public Shared Function GetIntegers(Title As String) As List(Of Integer)
            Dim s1() = Split(InputBox(Title), " ")
            Dim out As New List(Of Integer)
            For Each s In s1
                Try
                    out.Add(s)

                Catch ex As Exception

                End Try

            Next
            Return out
        End Function
    End Class
End Namespace

