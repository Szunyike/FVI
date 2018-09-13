Imports System.IO
Imports ClassLibrary1
Imports ClassLibrary1.Szunyi.Filter

Public Class TableManipulation
    Public Property Table_Manipulation_Settings As New Szunyi.Text.TableManipulation.Table_Manipulation_Settings
    Private MathCellForEdit As DataGridViewCell
    Public Property Files As New List(Of FileInfo)
    Public Property SaveFolder As String

    Private Sub Cancel(sender As Object, e As EventArgs) Handles Button2.Click
        Me.DialogResult = DialogResult.Cancel
    End Sub

    Private Sub Add_Column(sender As Object, e As EventArgs) Handles Button4.Click
        Dim s = InputBox("Enter the name of the new Column")
        If s <> String.Empty Then
            Me.Table_Manipulation_Settings.Items.Add(New Szunyi.Text.TableManipulation.Table_Manipulation_Setting(s, Me.Table_Manipulation_Settings.Items.Count))
        End If
        dgv1.DataSource = Nothing
        dgv1.DataSource = Me.Table_Manipulation_Settings.Items
        dgv1.Refresh()
    End Sub

    Private Function Check() As Boolean
        Dim EveryThingOK As Boolean = True
        For i1 = 0 To Me.dgv1.RowCount - 1
            If IsNothing(dgv1.Rows(i1).Cells(2).Value) = True Then dgv1.Rows(i1).Cells(2).Value = ""
            If IsNothing(dgv1.Rows(i1).Cells(4).Value) = True Then dgv1.Rows(i1).Cells(4).Value = ""
            Me.Table_Manipulation_Settings.Items(i1).ToDelete = dgv1.Rows(i1).Cells(3).Value
            If Me.Table_Manipulation_Settings.Items(i1).SetFilter(dgv1.Rows(i1).Cells(2).Value, Me.Table_Manipulation_Settings.Items(i1)) = False Then
                EveryThingOK = False
            End If
            If Me.Table_Manipulation_Settings.Items(i1).SetCalculation(dgv1.Rows(i1).Cells(4).Value,
                                                                       dgv1.Rows(i1).Cells(0).Value,
                                                                        Me.Table_Manipulation_Settings.Items(i1).The_Math_Calc) = False Then
                EveryThingOK = False
            End If
        Next
        Return EveryThingOK
    End Function
    Private Sub OK(sender As Object, e As EventArgs) Handles Button1.Click
        If Check() = True Then
            Me.DialogResult = DialogResult.OK
            Me.Close()
        End If
    End Sub
    Private Sub Save(sender As Object, e As EventArgs) Handles Button3.Click

        If Check() = True Then
            Me.DialogResult = DialogResult.OK
            Dim FIle = Szunyi.IO.Files.Save.SelectSaveFile(Szunyi.Constants.Files.Other.tdt)
            If IsNothing(FIle) = False Then
                Dim t = FIle.Directory
                Dim SubDir As New DirectoryInfo(t.FullName & "\" & FIle.Name)

                Me.SaveFolder = SubDir.FullName
                If SubDir.Exists = False Then SubDir.Create()

                Dim NewFIle As New FileInfo(Me.SaveFolder & "\" & FIle.Name)
                Me.Table_Manipulation_Settings.Save(NewFIle)

            End If
            Me.Close()
        End If
    End Sub
    Public Sub New()

        ' This call is required by the designer.
        InitializeComponent()

        ' Add any initialization after the InitializeComponent() call.
        Me.Files = ClassLibrary1. Szunyi.IO.Files.Filter.SelectFiles("Select Files")
        Dim Header = Szunyi.IO.Import.Text.GetHeader(Files.First, 1, Nothing, Nothing)
        Me.Table_Manipulation_Settings.Last_Origianal_Column = Header.Count - 1
        For i1 = 0 To Header.Count - 1
            Me.Table_Manipulation_Settings.Items.Add(New Szunyi.Text.TableManipulation.Table_Manipulation_Setting(Header(i1), i1))
        Next
        dgv1.DataSource = Me.Table_Manipulation_Settings.Items

    End Sub
    Private Sub UpDateDataSource()
        Me.dgv1.DataSource = Nothing
        dgv1.DataSource = Me.Table_Manipulation_Settings.Items
    End Sub

    Private Sub dgv1_MouseClick(sender As Object, e As MouseEventArgs) Handles dgv1.MouseClick
        Dim CurrentCell = dgv1.HitTest(e.X, e.Y)
        If CurrentCell.ColumnIndex = 4 Then ' Math
            MathCellForEdit = dgv1.Rows(CurrentCell.RowIndex).Cells(CurrentCell.ColumnIndex)
        ElseIf CurrentCell.ColumnIndex = 0 AndAlso IsNothing(MathCellForEdit) = False Then
            MathCellForEdit.Value = MathCellForEdit.Value & "col" & dgv1.Rows(CurrentCell.RowIndex).Cells(1).Value ' Index
            dgv1.CurrentCell = MathCellForEdit
            dgv1.BeginEdit(False)
        Else
            MathCellForEdit = Nothing
        End If
    End Sub

    Private Sub Button5_Click(sender As Object, e As EventArgs) Handles Button5.Click
        Dim File =  Szunyi.IO.Files.Filter.SelectFile()
        If IsNothing(File) = False Then
            Dim LastColumnID As Integer = Me.Table_Manipulation_Settings.Items.Count - 1
            Me.Table_Manipulation_Settings = New Szunyi.Text.TableManipulation.Table_Manipulation_Settings(File)
            Me.Table_Manipulation_Settings.Last_Origianal_Column = LastColumnID
            UpDateDataSource()
        End If
    End Sub

    Private Sub TableManipulation_Load(sender As Object, e As EventArgs) Handles MyBase.Load

    End Sub
End Class


