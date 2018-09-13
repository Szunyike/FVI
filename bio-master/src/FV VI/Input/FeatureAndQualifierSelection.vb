Imports Bio.IO.GenBank
Imports SzunyiControls

Public Class FeatureAndQualifierSelection
            Public Property SelectedFeatures As New List(Of String)
            Public Property SelectedQualifiers As New List(Of String)
            Public WithEvents SearchSettings As New List(Of SzunyiControls.SetSearch)

            Dim NofSearchSettings As Integer = 0

            Private Sub CloseSetSearh(Index As Integer)
                If Index > -1 Then
                    Dim alf As Int16 = 54
                End If

            End Sub
            Private Sub CloseSetSearhII(sender As Object, e As Integer)
        For Each Control In FlowLayoutPanel1.Controls
            Dim x As SetSearch = Control
            If x.ID = e Then
                FlowLayoutPanel1.Controls.Remove(Control)
                Exit For
            End If
        Next

    End Sub

            Private Sub FeatureAndQualifierSelection_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        '  Me.ListManager4.AddRange(StandardFeatureKeys.All)
        Me.ListManager5.AddRange(StandardQualifierNames.All)

    End Sub

            Private Sub Button2_Click(sender As Object, e As EventArgs) Handles Button2.Click
                Me.DialogResult = DialogResult.Cancel
                Me.Close()
            End Sub

            Private Sub Button1_Click(sender As Object, e As EventArgs) Handles Button1.Click
                Me.DialogResult = DialogResult.OK
                Me.Close()

            End Sub

    Public Sub ListManager5_NewSelection(x As String) Handles ListManager5.NewSelection
        Dim t As New SzunyiControls.SetSearch(x, NofSearchSettings)


        AddHandler t.CloseThis, AddressOf CloseSetSearhII

        Me.FlowLayoutPanel1.Controls.Add(t)
        NofSearchSettings += 1

    End Sub

    Private Sub Button3_Click(sender As Object, e As EventArgs) Handles Button3.Click
        Me.DialogResult = DialogResult.OK
        Dim x As New SettingForSearchInFeaturesAndQulifiers(Me)

        Me.Close()

    End Sub

    Private Sub Button4_Click(sender As Object, e As EventArgs) Handles Button4.Click
        Me.DialogResult = DialogResult.Cancel
        Me.Close()
    End Sub
End Class



Public Class SettingForSearchInFeaturesAndQulifiers
    Public Property SelectedFeatures As New List(Of String)

    Public Property SettingForSearchInQulifier As New List(Of SettingForSearchInQulifier)

    Public Sub New(f1 As FeatureAndQualifierSelection)
        Me.SelectedFeatures = f1.SelectedFeatures
        For Each Control In f1.FlowLayoutPanel1.Controls
            Dim t As SetSearch = Control
            SettingForSearchInQulifier.Add(New SettingForSearchInQulifier(t))
        Next

    End Sub
    Public Sub New(Type As String)
        Me.SelectedFeatures.Add(Type)
    End Sub
    Public Sub New(Types As List(Of String))
        Me.SelectedFeatures = Types
    End Sub
End Class
Public Enum SearchType
    Exact = 0
    Contains = 1
    NoValue = 2
    NotConsistOf = 3
    NotExactValue = 4
End Enum

Public Class SettingForSearchInQulifier
    Public Property QulifierName As String
    Public Property Type As SearchType
    Public Property InterestingStrings As New List(Of String)
    Public Sub New(t As SetSearch)
        Me.QulifierName = t.QulifierName
        Me.InterestingStrings = t.InterestingStrings
        Select Case t.Type
            Case "Contains"
                Me.Type = SearchType.Contains
            Case "Exact"
                Me.Type = SearchType.Exact
            Case "No Value"
                Me.Type = SearchType.NoValue
        End Select
    End Sub
End Class