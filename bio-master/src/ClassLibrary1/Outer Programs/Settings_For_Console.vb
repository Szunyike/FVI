Namespace Szunyi
    Namespace Outer_Programs
        Public Class Settings_For_Console
            Public Shared Function Get_Console_Command(x As List(Of Input_Description)) As String
                Dim str As New System.Text.StringBuilder
                For Each pron In x
                    If pron.Selected_Value <> String.Empty Then
                        Select Case pron.Type
                            Case Input_Description_Type.Boolean
                                If pron.Selected_Value = 1 Then
                                    str.Append("-").Append(pron.Flag).Append(" ")
                                Else
                                    str.Append("-").Append(pron.Flag).Append(" ")
                                End If
                            Case Input_Description_Type.Selection
                                str.Append("-").Append(pron.Flag).Append(" ").Append(pron.Selected_Value).Append(" ")
                            Case Input_Description_Type.String

                            Case Else
                                If pron.Selected_Value <> Double.NaN.ToString Then
                                    str.Append("-").Append(pron.Flag).Append(" ").Append(pron.Selected_Value).Append(" ")
                                End If
                        End Select

                    End If

                Next
                Return str.ToString
            End Function
        End Class
        Public Class Input_Description
            Public Property Flag As String ' minsites, minw
            Public Property Type As Input_Description_Type ' Integer, Double, string, Selection 'Ey Enum$
            Public Property Description As String
            Public Property MinValue As Double
            Public Property MaxValue As Double
            Public Property Default_Min_Value As Double
            Public Property Default_Max_Value As Double
            Public Property Values As List(Of String)
            Public Property Selected_Value As String
            Public Property Default_Value As Double
            Public Property File_Flag As String
            Public Sub New(Flag As String,
                           Type As Input_Description_Type,
                           Description As String,
                           MinValue As Double,
                           MaxValue As Double,
                           Default_Min_Value As Double,
                           Default_Max_Value As Double,
                           Default_Value As Double,
                           Selections As String,
                           File_Flag As String)
                Me.Flag = Flag
                Me.Type = Type
                Me.Description = Description
                Me.MinValue = MinValue
                Me.MaxValue = MaxValue
                Me.Default_Min_Value = Default_Min_Value
                Me.Default_Max_Value = Default_Max_Value
                Me.Default_Value = Default_Value
                Me.Values = Split(Selections, "|").ToList
                If Me.Type = Input_Description_Type.Selection Then
                    Me.Selected_Value = Me.Values.First
                End If
                Me.File_Flag = File_Flag
            End Sub

            Public Sub New(Flag As String,
                           Type As Input_Description_Type,
                           Description As String,
                           MinValue As Double,
                           MaxValue As Double,
                           Default_Min_Value As Double,
                           Default_Max_Value As Double,
                           Default_Value As Double,
                           Selections As List(Of String),
                           File_Flag As String)
                Me.Flag = Flag
                Me.Type = Type
                Me.Description = Description
                Me.MinValue = MinValue
                Me.MaxValue = MaxValue

                Me.Default_Min_Value = Default_Min_Value
                Me.Default_Max_Value = Default_Max_Value
                Me.Default_Value = Default_Value
                Me.Values = Selections
                If Me.Type = Input_Description_Type.Selection Then
                    Me.Selected_Value = Me.Values(Me.Default_Value)
                End If
                Me.File_Flag = File_Flag
            End Sub

        End Class

        Public Enum Input_Description_Type
            [Integer] = 1
            [Double] = 2
            [String] = 3
            Selection = 4
            [Boolean] = 5

        End Enum
    End Namespace
End Namespace


