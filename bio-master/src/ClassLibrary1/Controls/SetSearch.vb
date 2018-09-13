Public Class SetSearch
    Public ID As Integer
    Public Property QulifierName As String
    Public Property Type As String
    Public Property InterestingStrings As New List(Of String)
    Public Event CloseThis As EventHandler(Of Integer)
    Private Sub Button1_Click(sender As Object, e As EventArgs)
        RaiseEvent CloseThis(Me, ID)
    End Sub
    Public Sub New(txt As String, ID As Integer)

        ' This call is required by the designer.
        InitializeComponent()

        ' Add any initialization after the InitializeComponent() call.
        Me.Label1.Text = txt
        Me.QulifierName = txt
        Me.ID = ID
        Me.ComboBox1.SelectedIndex = 0
    End Sub

    Private Sub TextBox1_Validated(sender As Object, e As EventArgs)
        Dim s1 = Split(TextBox1.Text, vbCrLf)
        Me.InterestingStrings.Clear()
        For Each s In s1
            If s.Length > 1 Then Me.InterestingStrings.Add(s)
        Next
    End Sub

    Private Sub ComboBox1_SelectedIndexChanged(sender As Object, e As EventArgs) Handles ComboBox1.SelectedIndexChanged
        Me.Type = ComboBox1.SelectedItem
    End Sub

    Private Sub GroupBox1_Enter(sender As Object, e As EventArgs) Handles GroupBox1.Enter

    End Sub

    Private Sub TextBox1_Leave(sender As Object, e As EventArgs) Handles TextBox1.Leave
        Dim s1 = Split(TextBox1.Text, vbCrLf)
        Me.InterestingStrings.Clear()
        For Each s In s1
            If s.Length > 1 Then Me.InterestingStrings.Add(s)
        Next
    End Sub
End Class
