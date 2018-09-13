Imports ClassLibrary1.Szunyi.Transcipts

Public Class Adaptor_Scores

    Public Property res As New List(Of Adapter_Filtering)

    Public Sub New(adaptors As List(Of Adaptor_Pair))

        For Each AD In adaptors
            res.Add(New Adapter_Filtering(AD.Five_Prime_Adapter.ConvertToString, AD.Three_Prime_Adapter.ConvertToString, AD.Name))
        Next
    End Sub

    Private Sub Adaptor_Scores_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        InitializeComponent()

        Load1()
    End Sub
    Private Sub Load1()
        Me.L_5Seq.DataBindings.Add("Text", res, "Five_Prime_Seq")
        Me.L_3Seq.DataBindings.Add("Text", res, "Three_Prime_Seq")
        Me.SC_5.DataBindings.Add("Text", res, "Five_Prime_Adaptor_Score")
        Me.Sc_3.DataBindings.Add("Text", res, "Three_Prime_Adaptor_Score")
        Me.Pos_5_max.DataBindings.Add("Text", res, "Five_Prime_Query_Maximal_Position")
        Me.Pos_3_max.DataBindings.Add("Text", res, "Three_Prime_Query_Maximal_Position")
        Me.Pos_5_min.DataBindings.Add("Text", res, "Five_Prime_Query_Minimal_Position")
        Me.Pos_3_min.DataBindings.Add("Text", res, "Three_Prime_Query_Minimal_Position")

        Me.Pos_5_Max_Adapter.DataBindings.Add("Text", res, "Five_Prime_Adaptor_Maximal_Position")
        Me.Pos_3_Max_Adapter.DataBindings.Add("Text", res, "Three_Prime_Adaptor_Maximal_Position")
        Me.Pos_5_Min_Adapter.DataBindings.Add("Text", res, "Five_Prime_Adaptor_Minimal_Position")
        Me.Pos_3_Min_Adapter.DataBindings.Add("Text", res, "Three_Prime_Adaptor_Minimal_Position")


        Me.DIff.DataBindings.Add("Text", res, "Diff")

        Me.Ch_F_TSS.DataBindings.Add(New Binding("Checked", res, "Five_Prime_For_TSS", False, DataSourceUpdateMode.OnPropertyChanged, False))
        Me.Ch_F_PAS.DataBindings.Add(New Binding("Checked", res, "Five_Prime_For_PAS", False, DataSourceUpdateMode.OnPropertyChanged, False))
        Me.CH_3_TSS.DataBindings.Add(New Binding("Checked", res, "Three_Prime_For_TSS", False, DataSourceUpdateMode.OnPropertyChanged, False))
        Me.CH_3_PAS.DataBindings.Add(New Binding("Checked", res, "Three_Prime_For_PAS", False, DataSourceUpdateMode.OnPropertyChanged, False))

    End Sub
    Private Sub ListBox1_SelectedIndexChanged(sender As Object, e As EventArgs) Handles ListBox1.SelectedIndexChanged

    End Sub

    Private Sub TextBox2_KeyPress(sender As Object, e As KeyPressEventArgs) Handles SC_5.KeyPress
        Dim allowedChars As String = "0123456789"

        If allowedChars.IndexOf(e.KeyChar) = -1 Then
            ' Invalid Character
            e.Handled = True
        End If

    End Sub

    Private Sub TextBox3_KeyPress(sender As Object, e As KeyPressEventArgs) Handles Sc_3.KeyPress
        Dim allowedChars As String = "0123456789"

        If allowedChars.IndexOf(e.KeyChar) = -1 Then
            ' Invalid Character
            e.Handled = True
        End If

    End Sub

    Private Sub TextBox4_KeyPress(sender As Object, e As KeyPressEventArgs) Handles DIff.KeyPress
        Dim allowedChars As String = "0123456789"

        If allowedChars.IndexOf(e.KeyChar) = -1 Then
            ' Invalid Character
            e.Handled = True
        End If

    End Sub

    Private Sub TextBox5_KeyPress(sender As Object, e As KeyPressEventArgs) Handles Pos_3_min.KeyPress
        Dim allowedChars As String = "-0123456789"

        If allowedChars.IndexOf(e.KeyChar) = -1 Then
            ' Invalid Character
            e.Handled = True
        End If
    End Sub

    Private Sub TextBox6_KeyPress(sender As Object, e As KeyPressEventArgs) Handles Pos_5_min.KeyPress
        Dim allowedChars As String = "-0123456789"

        If allowedChars.IndexOf(e.KeyChar) = -1 Then
            ' Invalid Character
            e.Handled = True
        End If
    End Sub
    Private Sub TextB_KeyPress(sender As Object, e As KeyPressEventArgs) Handles Pos_5_max.KeyPress
        Dim allowedChars As String = "-0123456789"

        If allowedChars.IndexOf(e.KeyChar) = -1 Then
            ' Invalid Character
            e.Handled = True
        End If
    End Sub
    Private Sub Save(sender As Object, e As EventArgs) Handles Button4.Click
        Dim l1 = ClassLibrary1.Szunyi.IO.XML.Serialize(Of List(Of Adapter_Filtering))(res)
        If l1.Count > 0 Then
            Dim SaveFile = ClassLibrary1.Szunyi.IO.Files.Save.SelectSaveFile(ClassLibrary1.Szunyi.Constants.Files.Xml)
            ClassLibrary1.Szunyi.IO.Export.SaveText(l1, SaveFile)
        End If
    End Sub

    Private Sub Load_From_File(sender As Object, e As EventArgs) Handles Button5.Click
        Dim FIle = ClassLibrary1. Szunyi.IO.Files.Filter.SelectFile(ClassLibrary1.Szunyi.Constants.Files.Xml)
        If IsNothing(FIle) = False Then
            res = ClassLibrary1.Szunyi.IO.XML.Deserialize(Of List(Of Adapter_Filtering))(ClassLibrary1.Szunyi.IO.Import.Text.ReadToEnd(FIle))
            Me.ListBox1.DataSource = Nothing
            Me.ListBox1.DataSource = res
            Me.ListBox1.DisplayMember = "AP_Name"
            For Each c In Me.Controls
                If c.databindings.count > 0 Then
                    c.dattabinding.removeat(0)
                End If
            Next
            Load1()


        End If
    End Sub

    Private Sub Button2_Click(sender As Object, e As EventArgs) Handles Button2.Click
        Me.DialogResult = DialogResult.OK
        Me.Close()

    End Sub

    Private Sub Button3_Click(sender As Object, e As EventArgs) Handles Button3.Click
        Me.DialogResult = DialogResult.Cancel
        Me.Close()
    End Sub



    Private Sub Pos_3_max_KeyPress(sender As Object, e As KeyPressEventArgs) Handles Pos_3_max.KeyPress
        Dim allowedChars As String = "-0123456789"

        If allowedChars.IndexOf(e.KeyChar) = -1 Then
            ' Invalid Character
            e.Handled = True
        End If
    End Sub
End Class