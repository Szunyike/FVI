Public Class Get_List_of_String
    Public Property Strings As New List(Of String)
    Public Property IsPerfectMatch As Boolean
    Private Sub Button1_Click(sender As Object, e As EventArgs) Handles Button1.Click
        Me.DialogResult = DialogResult.OK
        Dim s1() = Split(TextBox1.Text, vbCrLf)
        If s1.Length > 0 Then
            Dim h = From s In s1 Where s.Length > 1

            If h.Count > 0 Then Me.Strings = h.ToList
        End If
        Me.IsPerfectMatch = Me.CheckBox1.Checked
        Me.Close()
    End Sub

    Private Sub Button2_Click(sender As Object, e As EventArgs) Handles Button2.Click
        Me.DialogResult = DialogResult.Cancel
        Me.Close()

    End Sub

    Private Sub Get_List_of_String_Load(sender As Object, e As EventArgs) Handles MyBase.Load

    End Sub
    Public Sub New(Title As String)

        ' This call is required by the designer.
        InitializeComponent()
        Me.Text = Title
        ' Add any initialization after the InitializeComponent() call.

    End Sub
End Class