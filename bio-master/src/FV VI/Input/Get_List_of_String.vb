Public Class Get_List_of_String
    Public Property Strings As New List(Of String)
    Public MustSort As Boolean
    Public Sub New(Title As String, Optional MustSOrt As Boolean = True)

        ' This call is required by the designer.
        InitializeComponent()
        Me.Text = Title
        ' Add any initialization after the InitializeComponent() call.
        Me.TextBox1.MaxLength = 1024 * 1024 * 1024
        Me.MustSort = MustSOrt
    End Sub

    Private Sub Cancel(sender As Object, e As EventArgs) Handles Button2.Click
        Me.DialogResult = DialogResult.Cancel
        Me.Close()
    End Sub

    Private Sub OK(sender As Object, e As EventArgs) Handles Button1.Click
        Me.DialogResult = DialogResult.OK
        Dim s1() As String = Split(TextBox1.Text, vbCrLf)
        For Each s In s1
            If s <> "" Then Me.Strings.Add(s)
        Next
        If Me.MustSort = True Then
            Me.Strings.Sort()
        End If
        Me.Close()
    End Sub
End Class