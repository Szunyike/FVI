Imports Bio.IO.GenBank
Imports ClassLibrary1

Public Class TRYIII
    Public Property SelectedFeatures As New List(Of String)
    Public Property SelectedQualifiers As New List(Of String)
    Public WithEvents SearchSettings As New List(Of SetSearch)
    Dim NofSearchSettings As Integer = 0
    Private Sub TRYIII_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        Me.StringSelectionCheckbox1 = New CheckBoxForStringsFull(StandardFeatureKeys.All.ToList, -1)
        Me.StringSelectionCheckbox2 = New CheckBoxForStringsFull(StandardQualifierNames.All.ToList, -1)
    End Sub
    Public Sub New(Items As List(Of String))

        ' This call is required by the designer.
        InitializeComponent()

        ' Add any initialization after the InitializeComponent() call.

    End Sub

    Private Sub CloseSetSearh(Index As Integer)
        If Index > -1 Then
            Dim alf As Int16 = 54
        End If

    End Sub
    Private Sub CloseSetSearhII(sender As Object, e As Integer)
        For Each Control In FLP.Controls
            Dim x As SetSearch = Control
            If x.ID = e Then
                FLP.Controls.Remove(Control)
                Exit For
            End If
        Next

    End Sub



    Private Sub Button2_Click(sender As Object, e As EventArgs) Handles Button2.Click
        Me.DialogResult = DialogResult.Cancel
        Me.Close()
    End Sub

    Private Sub Button1_Click(sender As Object, e As EventArgs) Handles Button1.Click
        Me.DialogResult = DialogResult.OK
        '   Me.SelectedFeatures = StringSelectionCheckbox1
        For Each Item In FLP.Controls
            Dim t As SetSearch = Item

        Next
        Me.Close()

    End Sub


End Class



