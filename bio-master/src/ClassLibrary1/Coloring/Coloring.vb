Imports System.Drawing

Namespace Szunyi
    Namespace Coloring
        Public Enum ColoringType
            FC = 1
            pVal = 2
            CrudeValues = 3
            Percent = 4
        End Enum
        Public Class Coloring
            Dim FC_Colors As New List(Of MinMaxColor)
            Dim pVal_Colors As New List(Of MinMaxColor)
            Dim CrudeValues_Colors As New List(Of MinMaxColor)
            Dim Percent_Colors As New List(Of MinMaxColor)
            Public CellStyles As New List(Of MinMaxCellStyle)
            Public All_Colors As New List(Of MinMaxColor)

            Public Function GetColor(Value As String, Type As ColoringType) As Color
                Dim d As Double = Value
                Select Case Type
                    Case ColoringType.FC
                        Dim x = From t In FC_Colors Where d >= t.Min AndAlso d <= t.Max Select t.Color
                        Return x.First

                    Case ColoringType.pVal
                        Dim x = From t In pVal_Colors Where d >= t.Min AndAlso d <= t.Max Select t.Color
                        Return x.First

                    Case ColoringType.CrudeValues
                        Dim x = From t In CrudeValues_Colors Where d >= t.Min AndAlso d <= t.Max Select t.Color
                        Return x.First
                    Case ColoringType.Percent
                        If Double.IsNaN(d) = True Then Return Color.Wheat
                        Dim x = From t In Percent_Colors Where d >= t.Min AndAlso d <= t.Max Select t.Color

                        Return x.First
                End Select
            End Function
            Public Function GetAllCellStyle() As List(Of System.Windows.Forms.DataGridViewCellStyle)
                Dim t = From x In Me.CellStyles Select x.CellStyle

                Return t.ToList
            End Function
            Public Function GetCellStyleIndex(Value As String, Type As ColoringType) As Integer
                Dim d As Double = Value
                Select Case Type
                    Case ColoringType.FC
                        Dim x = From t In FC_Colors Where d >= t.Min AndAlso d <= t.Max Select t.Index
                        Return x.First

                    Case ColoringType.pVal
                        Dim x = From t In pVal_Colors Where d >= t.Min AndAlso d <= t.Max Select t.Index
                        Return x.First

                    Case ColoringType.CrudeValues
                        Dim x = From t In CrudeValues_Colors Where d >= t.Min AndAlso d <= t.Max Select t.Index
                        Return x.First
                    Case ColoringType.Percent
                        If Double.IsNaN(d) = True Then Return 0
                        Dim x = From t In Percent_Colors Where d >= t.Min AndAlso d <= t.Max Select t.Index

                        Return x.First
                End Select
                Return 0
            End Function

            Public Sub New()
                AddTo(FC_Colors, Double.NegativeInfinity, 0.5, Color.Blue)
                AddTo(FC_Colors, 0.5, 0.66, Color.CornflowerBlue)
                AddTo(FC_Colors, 0.66, 1.5, Color.Green)
                AddTo(FC_Colors, 1.5, 2, Color.Orange)
                AddTo(FC_Colors, 2, Double.PositiveInfinity, Color.Red)

                AddTo(pVal_Colors, 0, 0.05, Color.DarkGreen)
                AddTo(pVal_Colors, 0.05, 0.15, Color.GreenYellow)
                AddTo(pVal_Colors, 0.15, 1, Color.Red)

                AddTo(CrudeValues_Colors, 0, 10, Color.Red)
                AddTo(CrudeValues_Colors, 10, 100, Color.PaleVioletRed)
                AddTo(CrudeValues_Colors, 100, 500, Color.Yellow)
                AddTo(CrudeValues_Colors, 500, 2000, Color.YellowGreen)
                AddTo(CrudeValues_Colors, 2000, Double.PositiveInfinity, Color.Green)

                AddTo(Percent_Colors, 0, 5, Color.Red)
                AddTo(Percent_Colors, 5, 10, Color.PaleVioletRed)
                AddTo(Percent_Colors, 10, 30, Color.Yellow)
                AddTo(Percent_Colors, 30, 50, Color.YellowGreen)
                AddTo(Percent_Colors, 50, 100, Color.Green)

            End Sub
            Private Sub AddTo(ByRef x As List(Of MinMaxColor), Min As Double, max As Double, Color As Color)
                Dim i1 As New MinMaxColor(Min, max, Color, All_Colors.Count)
                x.Add(i1)
                All_Colors.Add(i1)
                CellStyles.Add(New MinMaxCellStyle(i1))
            End Sub
        End Class
        Public Class MinMaxColor
            Public Property Min As Double
            Public Property Max As Double
            Public Property Color As Color
            Public Property Index As Integer
            Public Sub New(Min As Double, max As Double, Color As Color, Index As Integer)
                Me.Min = Min
                Me.Max = max
                Me.Color = Color
                Me.Index = Index
            End Sub
        End Class
        Public Class MinMaxCellStyle
            Public Property Index As Integer
            Public Property Min As Double
            Public Property Max As Double
            Public Property CellStyle As New System.Windows.Forms.DataGridViewCellStyle
            Public Sub New(Min As Double, max As Double, Color As Color, Index As Integer)
                Me.Min = Min
                Me.Max = max
                Me.CellStyle.BackColor = Color
                Me.Index = Index
            End Sub
            Public Sub New(MinMax As MinMaxColor)
                Me.Min = MinMax.Min
                Me.Max = MinMax.Max
                Me.CellStyle.BackColor = MinMax.Color
                Me.Index = MinMax.Index
            End Sub
        End Class

    End Namespace
End Namespace
