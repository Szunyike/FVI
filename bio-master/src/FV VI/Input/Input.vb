Namespace input
    Public Class Input
        Shared Function GetIntrestingColumn(File As IO.FileInfo, NofHeaderLine As Integer, Optional Title As String = "Select Column") As Integer
            Dim Headers = ClassLibrary1.Szunyi.IO.Import.Text.GetHeader(File, NofHeaderLine, Nothing, Nothing)

            Using x As New Select_Columns(Headers)
                x.Text = Title
                If x.ShowDialog = System.Windows.Forms.DialogResult.OK Then
                    Return x.SelectedIndexes.First
                Else : Return -1
                End If
            End Using
        End Function
    End Class
End Namespace


