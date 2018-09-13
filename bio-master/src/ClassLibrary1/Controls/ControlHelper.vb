Imports System.Windows.Forms

Namespace Szunyi
    Namespace TheControls
        Public Class ControlHelper
            Public Shared Function GetDGVTextBoxColumn(Name As String, Type As System.Type) As DataGridViewTextBoxColumn
                Dim Col As New DataGridViewTextBoxColumn
                Col.Name = Name
                Col.HeaderText = Name
                Col.ValueType = Type
                Return Col
            End Function
        End Class
    End Namespace
End Namespace

