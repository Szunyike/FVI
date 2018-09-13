
Imports System.IO
Imports ClassLibrary1.Szunyi.Filter
Namespace Szunyi
    Namespace Text
        Namespace TableManipulation
            Public Class Table_Manipulation_Settings
                Public Property Items As New List(Of Table_Manipulation_Setting)
                Public Property Last_Origianal_Column As Integer = -1
                Public Sub New()

                End Sub
                Public Sub New(File As FileInfo)
                    For Each s In Szunyi.IO.Import.Text.ParseToArray(File, vbTab)
                        Me.Items.Add(New Table_Manipulation_Setting(s))
                    Next
                End Sub
                Public Sub Save(File As FileInfo)
                    Dim str As New System.Text.StringBuilder
                    For Each Item In Me.Items
                        str.Append(Item.ToString).AppendLine()
                    Next
                    If Me.Items.Count > 0 Then
                        str.Length -= 2
                        Szunyi.IO.Export.SaveText(str.ToString, File)
                    End If

                End Sub
                Public Function clone() As Table_Manipulation_Settings
                    Dim out As New Table_Manipulation_Settings
                    out.Last_Origianal_Column = Me.Last_Origianal_Column
                    For Each Item In Me.Items
                        out.Items.Add(Item.clone)
                    Next
                    Return out
                End Function
            End Class

            Public Class Table_Manipulation_Setting
                Public Property Column_Name As String
                Public Property Column_ID As Integer
                Public Property Filter_Display As String
                Public Property Filters As New List(Of Numerical_Filter_Setting)
                Public Property ToDelete As Boolean
                Public Property Calculation As String
                Public Property Splitter As String
                Public Property The_Math_Calc As New Szunyi.Math.MathCalc
                Public Sub New(Name As String, ID As Integer)
                    Me.Column_Name = Name
                    Me.Column_ID = ID
                End Sub
                Public Sub New()

                End Sub
                Public Sub New(s() As String)
                    Me.Column_Name = s(0)
                    Me.Column_ID = s(1)
                    Me.ToDelete = s(2)
                    SetFilter(s(3), Me)
                    SetCalculation(s(4), Me.Column_Name, Me.The_Math_Calc)
                    Me.Splitter = s(5)
                End Sub
                Public Function SetFilter(Value As String, x As Table_Manipulation_Setting) As Boolean
                    If Value = String.Empty Then Return True
                    Dim s = Split(Value, ",")
                    Me.Filters.Clear()
                    Dim Out As Boolean = True
                    For Each S1 In s
                        x.Filters.Add(New Numerical_Filter_Setting(S1))
                        If x.Filters.Last.Valid = False Then Out = False
                    Next
                    x.Filter_Display = Value

                    Return Out

                End Function
                Public Function SetCalculation(expression As String, ColumnName As String, The_Math_Calc As Szunyi.Math.MathCalc) As Boolean
                    Me.Calculation = expression

                    Return True
                End Function
                Public Overrides Function ToString() As String
                    Return Me.Column_Name & vbTab & Me.Column_ID & vbTab & Me.ToDelete & vbTab & Me.Filter_Display & vbTab & Me.Calculation & vbTab & Splitter

                End Function

                Friend Function clone() As Table_Manipulation_Setting
                    Dim out As New Table_Manipulation_Setting
                    out.Column_Name = Me.Column_Name
                    out.Column_ID = Me.Column_ID
                    out.Filter_Display = Me.Filter_Display
                    SetFilter(Me.Filter_Display, out)
                    out.ToDelete = Me.ToDelete
                    out.Calculation = Me.Calculation
                    out.Splitter = Me.Splitter
                    SetCalculation(out.Calculation, out.Column_Name, out.The_Math_Calc)
                    Return out
                End Function
            End Class

            Public Class Values_From_Txt
                Public Shared Function Get_Columns_Values_As_Double(basic_Text As String,
                                                                    colID As Integer,
                                                                    FIrst_Line As Integer,
                                                                    Optional Main_Separator As String = vbCrLf,
                                                                    Optional Sub_Separator As String = vbTab) As List(Of Double)
                    Dim out As New List(Of Double)
                    Dim s() = Split(basic_Text, Main_Separator)
                    For i1 = FIrst_Line To s.Count - 1
                        Dim sII = Split(s(i1), Sub_Separator)
                        If sII.Count - 1 >= colID AndAlso sII(colID) <> String.Empty Then
                            out.Add(sII(colID))
                        End If

                    Next
                    Return out
                End Function
            End Class
        End Namespace
    End Namespace
End Namespace

