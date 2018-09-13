Imports System.IO

Namespace Szunyi
    Namespace Protein
        Public Class DisEMBL1
            Public Property na As Integer = 21
            Public Property mw As Integer = 41
            Public Property mh As Integer = 30
            Public Property Russel As New DisEMBL.Russel
            Public Property Missing As New DisEMBL.Missing
            Public Property bFactor As New DisEMBL.bFactor

            Public res As New List(Of String)
            Private Function feed_forward(ByVal s() As Integer, ByVal w() As Single, ByVal nw As Integer, ByVal nh As Integer) As Single

                Dim h(mh - 1) As Single
                Dim o(1) As Single
                Dim x As Single
                Dim i As Integer
                Dim j As Integer
                For Each si In s
                    si += (mw - nw) / 2
                Next
                ' s += (mw - nw) / 2

                ' Feed input values to hidden layer making use of sparse encoding 
                For i = 0 To nh - 1
                    x = w((na * nw + 1) * (i + 1) - 1)
                    For j = 0 To nw - 1
                        x += w((na * nw + 1) * i + na * j + s(j))
                    Next j
                    If x <= -16 Then
                        h(i) = 0
                    ElseIf x >= 16 Then
                        h(i) = 1
                    Else
                        h(i) = Russel.Sigmoid(CInt(Fix(8 * x + 128)))
                    End If
                Next i

                ' Feed hidden layer values to output layer 
                For i = 0 To 1
                    x = w((na * nw + 1) * nh + (nh + 1) * (i + 1) - 1)
                    For j = 0 To nh - 1
                        x += w((na * nw + 1) * nh + (nh + 1) * i + j) * h(j)
                    Next j
                    If x <= -16 Then
                        o(i) = 0
                    ElseIf x >= 16 Then
                        o(i) = 1
                    Else
                        o(i) = Russel.Sigmoid(CInt(Fix(8 * x + 128)))
                    End If
                Next i

                ' Combine the scores from the two output neurons 
                Return ((o(0) + 1 - o(1)) / 2)

            End Function
            Private Sub predict(ByVal s() As Integer)

                Dim sm As Single
                Dim sb As Single
                Dim sr As Single

                'C++ TO VB CONVERTER WARNING: C++ to VB Converter cannot determine whether both operands of this division are integer types - if they are then you should use the VB integer division operator:
                If s((mw - 1) / 2) = na - 1 Then
                    Return
                End If

                sr = 0
                sr += feed_forward(s, Russel.r19_1, 19, 30)
                sr += feed_forward(s, Russel.r19_2, 19, 30)
                sr += feed_forward(s, Russel.r19_3, 19, 30)
                sr += feed_forward(s, Russel.r19_4, 19, 30)
                sr += feed_forward(s, Russel.r19_5, 19, 30)
                sr /= 5
                sr = 0.07387214 + 0.8020778 * sr

                sb = 0
                sb += feed_forward(s, bFactor.b41_1, 41, 5)
                sb += feed_forward(s, bFactor.b41_2, 41, 5)
                sb += feed_forward(s, bFactor.b41_3, 41, 5)
                sb += feed_forward(s, bFactor.b41_4, 41, 5)
                sb += feed_forward(s, bFactor.b41_5, 41, 5)
                sb /= 5
                sb = 0.08016882 + 0.6282424 * sb

                sm = 0
                sm += feed_forward(s, Missing.m9_1, 9, 30)
                sm += feed_forward(s, Missing.m9_2, 9, 30)
                sm += feed_forward(s, Missing.m9_3, 9, 30)
                sm += feed_forward(s, Missing.m9_4, 9, 30)
                sm += feed_forward(s, Missing.m9_5, 9, 30)
                sm += feed_forward(s, Missing.m21_1, 21, 30)
                sm += feed_forward(s, Missing.m21_2, 21, 30)
                sm += feed_forward(s, Missing.m21_3, 21, 30)
                sm += feed_forward(s, Missing.m21_4, 21, 30)
                sm += feed_forward(s, Missing.m21_5, 21, 30)
                sm /= 10
                res.Add("{0:f}" & ControlChars.Tab & "{1:f}" & ControlChars.Tab & "{2:f}")
                res.Add(sr & vbTab & sr & sb & vbTab & sm)

                Console.Write("{0:f}" & ControlChars.Tab & "{1:f}" & ControlChars.Tab & "{2:f}" & ControlChars.Lf, sr, sr * sb, sm)

            End Sub

            Function Main(ByVal ARGC As Integer, ByVal ARGV() As String, Full_text As String) As Integer

                Dim alphabet As String = "FIVWMLCHYAGNRTPDEQSK"

                Dim p As String
                Dim c As Integer
                Dim i As Integer
                Dim j As Integer
                Dim s(mw - 1) As Integer

                For i = 0 To mw - 1
                    s(i) = na - 1
                Next i

                For Each The_Char In Full_text
                    c = AscW(The_Char)
                    p = StringFunctions.StrChr(alphabet, The_Char)
                    If p IsNot Nothing Then
                        For i = 1 To mw - 1
                            s(mw - i) = s(mw - i - 1)
                        Next i
                        s(0) = p
                        predict(s)
                    ElseIf The_Char = ControlChars.Lf Then
                        i = 0
                        'C++ TO VB CONVERTER WARNING: C++ to VB Converter cannot determine whether both operands of this division are integer types - if they are then you should use the VB integer division operator:
                        Do While i < (mw - 1) / 2
                            For j = 1 To mw - 1
                                s(mw - j) = s(mw - j - 1)
                            Next j
                            s(0) = na - 1
                            predict(s)
                            i += 1
                        Loop
                    End If

                Next
                Dim txt = Szunyi.Text.General.GetText(res)
                Return (0)

            End Function


        End Class
        Friend Module StringFunctions
            '------------------------------------------------------------------------------------
            '	This method allows replacing a single character in a string, to help convert
            '	C++ code where a single character in a character array is replaced.
            '------------------------------------------------------------------------------------
            Friend Function ChangeCharacter(ByVal sourceString As String, ByVal charIndex As Integer, ByVal changeChar As Char) As String
                Return If(charIndex > 0, sourceString.Substring(0, charIndex), "") _
        & changeChar.ToString() & If(charIndex < sourceString.Length - 1, sourceString.Substring(charIndex + 1), "")
            End Function

            '------------------------------------------------------------------------------------
            '	This method simulates the classic C string function 'isxdigit' (and 'iswxdigit').
            '------------------------------------------------------------------------------------
            Friend Function IsXDigit(ByVal character As Char) As Boolean
                If Char.IsDigit(character) Then
                    Return True
                ElseIf "ABCDEFabcdef".IndexOf(character) > -1 Then
                    Return True
                Else
                    Return False
                End If
            End Function

            '------------------------------------------------------------------------------------
            '	This method simulates the classic C string function 'strchr' (and 'wcschr').
            '------------------------------------------------------------------------------------
            Friend Function StrChr(ByVal stringToSearch As String, ByVal charToFind As Char) As String
                Dim index As Integer = stringToSearch.IndexOf(charToFind)
                If index > -1 Then
                    Return index
                Else
                    Return Nothing
                End If
            End Function

            '------------------------------------------------------------------------------------
            '	This method simulates the classic C string function 'strrchr' (and 'wcsrchr').
            '------------------------------------------------------------------------------------
            Friend Function StrRChr(ByVal stringToSearch As String, ByVal charToFind As Char) As String
                Dim index As Integer = stringToSearch.LastIndexOf(charToFind)
                If index > -1 Then
                    Return stringToSearch.Substring(index)
                Else
                    Return Nothing
                End If
            End Function

            '------------------------------------------------------------------------------------
            '	This method simulates the classic C string function 'strstr' (and 'wcsstr').
            '------------------------------------------------------------------------------------
            Friend Function StrStr(ByVal stringToSearch As String, ByVal stringToFind As String) As String
                Dim index As Integer = stringToSearch.IndexOf(stringToFind)
                If index > -1 Then
                    Return stringToSearch.Substring(index)
                Else
                    Return Nothing
                End If
            End Function

            '------------------------------------------------------------------------------------
            '	This method simulates the classic C string function 'strtok' (and 'wcstok').
            '	Note that the .NET string 'Split' method cannot be used to simulate 'strtok' since
            '	it doesn't allow changing the delimiters between each token retrieval.
            '------------------------------------------------------------------------------------
            Private activeString As String
            Private activePosition As Integer
            Friend Function StrTok(ByVal stringToTokenize As String, ByVal delimiters As String) As String
                If Not stringToTokenize Is Nothing Then
                    activeString = stringToTokenize
                    activePosition = -1
                End If

                'the stringToTokenize was never set:
                If activeString Is Nothing Then
                    Return Nothing
                End If

                'all tokens have already been extracted:
                If activePosition = activeString.Length Then
                    Return Nothing
                End If

                'bypass delimiters:
                activePosition += 1
                Do While activePosition < activeString.Length AndAlso delimiters.IndexOf(activeString(activePosition)) > -1
                    activePosition += 1
                Loop

                'only delimiters were left, so return null:
                If activePosition = activeString.Length Then
                    Return Nothing
                End If

                'get starting position of string to return:
                Dim startingPosition As Integer = activePosition

                'read until next delimiter:
                Do
                    activePosition += 1
                Loop While activePosition < activeString.Length AndAlso delimiters.IndexOf(activeString.Chars(activePosition)) = -1

                Return activeString.Substring(startingPosition, activePosition - startingPosition)
            End Function
        End Module
    End Namespace
End Namespace

