Imports System.IO

Public Class Select_Columns
    Public SelectedIndexes As New List(Of Integer)
    Private Sub Button2_Click(sender As Object, e As EventArgs) Handles Button2.Click
        Me.DialogResult = System.Windows.Forms.DialogResult.Cancel

    End Sub
    Public Sub New(Items As List(Of String), Optional Title As String = "Select Column")
        InitializeComponent()
        Me.Text = Title
        For i1 = 0 To Items.Count - 1
            If Items(i1) <> "" Then
                ListBox1.Items.Add(Items(i1))
            Else
                ListBox1.Items.Add("Col" & i1)
            End If
        Next

    End Sub
    Public Sub New(File As FileInfo)

        ' This call is required by the designer.
        InitializeComponent()

        ' Add any initialization after the InitializeComponent() call.

    End Sub
    Private Sub OK(sender As Object, e As EventArgs) Handles Button1.Click
        Me.SelectedIndexes = New List(Of Integer)
        For Each s In ListBox1.SelectedIndices
            Me.SelectedIndexes.Add(s)
        Next
        Me.DialogResult = System.Windows.Forms.DialogResult.OK

    End Sub

    Private Sub Select_Columns_Load(sender As Object, e As EventArgs) Handles MyBase.Load

    End Sub
End Class