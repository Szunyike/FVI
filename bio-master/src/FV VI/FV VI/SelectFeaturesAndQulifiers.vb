Public Class SelectFeaturesAndQulifiers
	Public Property OK As Boolean = False
	Public Property SelectedFeatures As New List(Of String)
	Public Property SelectedQualifiers As New List(Of String)

	Private Sub Select_Qualifiers_And_Names_Load(sender As Object, e As EventArgs) Handles MyBase.Load
		ListBox1.SelectionMode = SelectionMode.MultiExtended
        ListBox2.SelectionMode = SelectionMode.MultiExtended
        ListBox1.Items.AddRange(Bio.IO.GenBank.StandardFeatureKeys.All.ToArray)
        ListBox2.Items.AddRange(Bio.IO.GenBank.StandardQualifierNames.All.ToArray)

    End Sub

	Private Sub bOK(sender As Object, e As EventArgs) Handles Button1.Click
		Me.OK = True
		For Each s In ListBox1.SelectedItems
			SelectedFeatures.Add(s)
		Next
		For Each s In ListBox2.SelectedItems
			SelectedQualifiers.Add(s)
		Next
        Me.DialogResult = DialogResult.OK
        Me.Close()
	End Sub

	Private Sub Button2_Click_1(sender As Object, e As EventArgs) Handles Button2.Click
        Me.DialogResult = DialogResult.Cancel
        Me.Close()
	End Sub
End Class
Public Class SettingForSearchInFeaturesAndQulifiers
	Public Property SelectedFeatures As New List(Of String)
	Public Property SelectedQualifiers As New List(Of String)

	Public Property SearchStringInQulifiers As New List(Of String)

	Public Property IsPerfectMatch As Boolean

	Public Sub New(f1 As SelectFeaturesAndQulifiers)
		Me.SelectedFeatures = f1.SelectedFeatures
		Me.SelectedQualifiers = f1.SelectedQualifiers
		Me.IsPerfectMatch = f1.Perfect.CheckState
		Dim S1() = Split(f1.TextBox1.Text, vbCrLf)
		Dim tmp = From x In S1 Where x <> ""

		If tmp.Count > 0 Then SearchStringInQulifiers = tmp.ToList


	End Sub
	Public Sub New(Type As String)
		Me.SelectedFeatures.Add(Type)
	End Sub
End Class