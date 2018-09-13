Imports System.IO

Namespace Szunyi
    Namespace Math

        Public Class From_Files
            Public Shared Function Get_Sums(Files As List(Of FileInfo), ColumnIDs As List(Of Integer)) As Szunyi.Text.TableManipulation.Items_With_Properties
                Dim Header = Szunyi.IO.Import.Text.GetHeader(Files.First, 1, Nothing, Nothing)
                Dim I_w_Ps As New Szunyi.Text.TableManipulation.Items_With_Properties
                Dim Sums(Header.Count) As Double
                Dim I_w_P As New Szunyi.Text.TableManipulation.Item_With_Properties
                For Each FIle In Files
                    For Each s In Szunyi.IO.Import.Text.ParseToArray(FIle, vbTab, 1)
                        For Each ColINdex In ColumnIDs
                            Sums(ColINdex) += s(ColINdex)
                        Next
                    Next

                    For Each ColINdex In ColumnIDs
                        I_w_Ps.ItemHeaders.Add(FIle.Name & "-" & Header(ColINdex))
                        I_w_P.Properties.Add(Sums(ColINdex))
                    Next

                Next
                I_w_Ps.Items.Add(I_w_P)
                Return I_w_Ps
            End Function

            Public Shared Function Get_Means(Files As List(Of FileInfo), ColumnIDs As List(Of Integer)) As Szunyi.Text.TableManipulation.Items_With_Properties
                Dim Header = Szunyi.IO.Import.Text.GetHeader(Files.First, 1, Nothing, Nothing)
                Dim I_w_Ps As New Szunyi.Text.TableManipulation.Items_With_Properties
                Dim Sums(Header.Count) As Double
                Dim I_w_P As New Szunyi.Text.TableManipulation.Item_With_Properties
                For Each FIle In Files
                    Dim Count As Integer = 0
                    For Each s In Szunyi.IO.Import.Text.ParseToArray(FIle, vbTab, 1)
                        Count += 1
                        For Each ColINdex In ColumnIDs
                            Sums(ColINdex) += s(ColINdex)
                        Next
                    Next

                    For Each ColINdex In ColumnIDs
                        I_w_Ps.ItemHeaders.Add(FIle.Name & "-" & Header(ColINdex))
                        I_w_P.Properties.Add(Sums(ColINdex) / Count)
                    Next

                Next
                I_w_Ps.Items.Add(I_w_P)
                Return I_w_Ps
            End Function

            Public Shared Function Get_Frequencies(Files As List(Of FileInfo), ColumnIDs As List(Of Integer)) As Szunyi.Text.TableManipulation.Items_With_Properties
                Dim Max As Double = 0
                Dim Min As Double = 0
                Dim NOfRange As Integer = InputBox("Nof Range to SPlit?")
                Dim Header = Szunyi.IO.Import.Text.GetHeader(Files.First, 1, Nothing, Nothing)
                For Each FIle In Files
                    For Each s In Szunyi.IO.Import.Text.ParseToArray(FIle, vbTab, 1)

                        For Each ColINdex In ColumnIDs
                            If s.Length >= ColINdex Then
                                If Max < s(ColINdex) Then Max = s(ColINdex)
                                If Min > s(ColINdex) Then Min = s(ColINdex)
                            End If

                        Next
                    Next
                Next
                Dim BasicStep As Double = (Max - Min) / NOfRange
                Dim I_w_Ps As New Szunyi.Text.TableManipulation.Items_With_Properties
                For i1 = 1 To NOfRange
                    I_w_Ps.Items.Add(New Szunyi.Text.TableManipulation.Item_With_Properties(i1.ToString))
                    I_w_Ps.Items.Last.Properties.Add(Min + BasicStep * (i1 - 1))
                Next
                For Each FIle In Files
                    For Each ColINdex In ColumnIDs
                        Dim Counts(NOfRange) As Integer
                        For Each s In Szunyi.IO.Import.Text.ParseToArray(FIle, vbTab, 1)
                            If s.Length >= ColINdex Then
                                Dim d As Double = (s(ColINdex) - Min)
                                Dim r = d / BasicStep
                                Dim r1 As Integer = r
                                Counts(r1) += 1
                            End If

                        Next
                        I_w_Ps.Add_Values_WithOut_Keys(FIle.Name & " " & Header(ColINdex), Counts.ToList)

                    Next
                Next
                Return I_w_Ps
            End Function

            Public Shared Function Get_Sum(Is_w_Ps As Szunyi.Text.TableManipulation.Items_With_Properties,
                                           Optional Keys As List(Of String) = Nothing,
                                           Optional start As Integer = 0, Optional [End] As Integer = -1) _
                As Szunyi.Text.TableManipulation.Item_With_Properties
                If [End] - 1 Then [End] = Is_w_Ps.ItemHeaders.Count - 1
                Dim I_W_Ps As New Szunyi.Text.TableManipulation.Item_With_Properties

                If IsNothing(Keys) = True Then
                    Dim d(Is_w_Ps.ItemHeaders.Count)
                    For i1 = start To [End]
                        For Each Item In Is_w_Ps.Items
                            d(i1) += Item.Properties(i1)
                        Next
                    Next
                    I_W_Ps.Properties.AddRange(d)
                Else
                    Dim d(Is_w_Ps.ItemHeaders.Count) As Integer
                    For Each Key In Keys
                        Dim Index = Is_w_Ps.Get_Index(Key, Szunyi.Constants.TextMatch.Exact)
                        If Index > -1 Then
                            For i1 = start + 1 To [End]
                                d(i1) += Is_w_Ps.Items(Index).Properties(i1)
                            Next
                        End If
                    Next
                    Dim s As New List(Of String)
                    For Each d1 In d
                        s.Add(d1)
                    Next
                    I_W_Ps.Properties.AddRange(s)
                End If
                Return I_W_Ps
            End Function



        End Class
    End Namespace
End Namespace

