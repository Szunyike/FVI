Public Class VennTable
    Public Property VT As ClassLibrary1.Szunyi.Venn.Venn_Table
    Private Sub VennTable_Load(sender As Object, e As EventArgs) Handles MyBase.Load

    End Sub
    Public Sub New(VT As ClassLibrary1.Szunyi.Venn.Venn_Table)

        ' This call is required by the designer.
        InitializeComponent()

        ' Add any initialization after the InitializeComponent() call.
        Me.VT = VT

    End Sub

    Private Sub ByGeneToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles ByGeneToolStripMenuItem.Click
        Dim Dt As DataTable = Me.VT.Get_Gene_Dt(Me.VT.cExperimtens_With_GeneIDs, Me.VT.cGenes_By_Experimets)
        Me.dgv1.DataSource = Dt

    End Sub

    Private Sub ByExpToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles ByExpToolStripMenuItem.Click
        Dim Dt As DataTable = Me.VT.Get_Exp_Dt
        Me.dgv1.AllowUserToOrderColumns = False
        Me.dgv1.DataSource = Nothing
        Me.dgv1.DataSource = Dt

    End Sub

    Private Sub OneByONe(sender As Object, e As EventArgs) Handles ToolStripMenuItem2.Click
        Dim selCells = Me.dgv1.SelectedCells
        Dim alf As Int16 = 65
        Dim PathToSave = ClassLibrary1.Szunyi.IO.Directory.Get_Folder
        For Each Cell As DataGridViewTextBoxCell In selCells
            Dim t = Me.VT.GeneIDs(Cell.RowIndex, Cell.ColumnIndex - 1)
            Dim Shorts = t ' ClassLibrary1.Szunyi.Text.General.GetFirstParts(t, " ")
            Dim expName = dgv1.Columns(Cell.ColumnIndex).Name & " " & dgv1.Rows(Cell.RowIndex).Cells(0).Value & ".tab"
            Dim NewFIleNAme As System.IO.FileInfo
            If PathToSave.FullName.EndsWith("\") = True Then
                NewFIleNAme = New System.IO.FileInfo(PathToSave.FullName & expName)
            Else
                NewFIleNAme = New System.IO.FileInfo(PathToSave.FullName & "\" & expName)
            End If

            ClassLibrary1.Szunyi.IO.Export.SaveText(ClassLibrary1.Szunyi.Text.General.GetText(Shorts), NewFIleNAme)
        Next
    End Sub

    Private Sub Into_Common(sender As Object, e As EventArgs) Handles ToolStripMenuItem3.Click
        Dim All As New List(Of String)

        For Each Cell As DataGridViewTextBoxCell In dgv1.SelectedCells
            Dim t = Me.VT.GeneIDs(Cell.RowIndex, Cell.ColumnIndex - 1)
            Dim Shorts = t ' ClassLibrary1.Szunyi.Text.General.GetFirstParts(t, " ")
            All.AddRange(Shorts)
        Next
        Dim Last = All.Distinct.ToList
        ClassLibrary1.Szunyi.IO.Export.SaveText(ClassLibrary1.Szunyi.Text.General.GetText(Last))
    End Sub




    Private Sub ToolStripTextBox1_KeyDown(sender As Object, e As KeyEventArgs) Handles ToolStripTextBox1.KeyDown
        If e.KeyCode = Keys.Enter Then
            Me.VT.Filter(ToolStripTextBox1.Text)
            Me.dgv1.DataSource = Me.VT.Get_Gene_Dt(Me.VT.cExperimtens_With_GeneIDs, Me.VT.cGenes_By_Experimets)
        End If
    End Sub

    Private Sub ToolStripTextBox1_Click(sender As Object, e As EventArgs) Handles ToolStripTextBox1.Click
        Me.VT.cGenes_By_Experimets = Me.VT.Genes_By_Experimets
        Me.VT.cExperimtens_With_GeneIDs = Me.VT.Experimtens_With_GeneIDs
        Me.dgv1.DataSource = Me.VT.Get_Gene_Dt(Me.VT.cExperimtens_With_GeneIDs, Me.VT.cGenes_By_Experimets)
    End Sub

    Private Sub DistinctsToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles DistinctsToolStripMenuItem.Click
        If dgv1.SelectedCells.Count = 2 Then
            Dim PathToSave = ClassLibrary1.Szunyi.IO.Directory.Get_Folder

            Dim Cell1 = dgv1.SelectedCells(0)
            Dim Cell2 = dgv1.SelectedCells(1)
            Dim t1 = Me.VT.GeneIDs(Cell1.RowIndex, Cell1.ColumnIndex - 1)
            Dim t2 = Me.VT.GeneIDs(Cell2.RowIndex, Cell2.ColumnIndex - 1)
            Dim Shorts1 = t1 'ClassLibrary1.Szunyi.Text.General.GetFirstParts(t1, " ")
            Dim Shorts2 = t2 ' ClassLibrary1.Szunyi.Text.General.GetFirstParts(t2, " ")
            Dim d1 = Shorts1.Except(Shorts2)
            Dim d2 = Shorts2.Except(Shorts1)
            Dim FIleNAme1 = dgv1.Columns(Cell1.ColumnIndex).Name & "_" & dgv1.Rows(Cell1.RowIndex).Cells(0).Value & " - " &
            dgv1.Columns(Cell2.ColumnIndex).Name & "_" & dgv1.Rows(Cell2.RowIndex).Cells(0).Value
            Dim FIleNAme2 = dgv1.Columns(Cell2.ColumnIndex).Name & "_" & dgv1.Rows(Cell2.RowIndex).Cells(0).Value & " - " &
            dgv1.Columns(Cell1.ColumnIndex).Name & "_" & dgv1.Rows(Cell1.RowIndex).Cells(0).Value
            If IsNothing(PathToSave) = True Then Exit Sub
            If PathToSave.FullName.EndsWith("\") = True Then
                If d1.Count > 0 Then
                    Dim NewFIleNAme = New System.IO.FileInfo(PathToSave.FullName & FIleNAme1)
                    ClassLibrary1.Szunyi.IO.Export.SaveText(ClassLibrary1.Szunyi.Text.General.GetText(d1.ToList), NewFIleNAme)
                End If
                If d2.Count > 0 Then
                    Dim NewFIleNAme = New System.IO.FileInfo(PathToSave.FullName & FIleNAme2)
                    ClassLibrary1.Szunyi.IO.Export.SaveText(ClassLibrary1.Szunyi.Text.General.GetText(d2.ToList), NewFIleNAme)
                End If

            Else
                If d1.Count > 0 Then
                    Dim NewFIleNAme = New System.IO.FileInfo(PathToSave.FullName & "\" & FIleNAme1)
                    ClassLibrary1.Szunyi.IO.Export.SaveText(ClassLibrary1.Szunyi.Text.General.GetText(d1.ToList), NewFIleNAme)
                End If
                If d2.Count > 0 Then
                    Dim NewFIleNAme = New System.IO.FileInfo(PathToSave.FullName & "\" & FIleNAme2)
                    ClassLibrary1.Szunyi.IO.Export.SaveText(ClassLibrary1.Szunyi.Text.General.GetText(d2.ToList), NewFIleNAme)
                End If

            End If
            End If
    End Sub
    Private Sub ToolStripMenuItem4_Click(sender As Object, e As EventArgs) Handles ToolStripMenuItem4.Click
        Dim All As New List(Of String)
        If dgv1.SelectedCells.Count = 0 Then Exit Sub
        Dim FIrstCell As DataGridViewTextBoxCell = dgv1.SelectedCells(0)

        For Each Cell As DataGridViewTextBoxCell In dgv1.SelectedCells
            Dim t = Me.VT.GeneIDs(Cell.RowIndex, Cell.ColumnIndex - 1)
            Dim Shorts = t ' ClassLibrary1.Szunyi.Text.General.GetFirstParts(t, " ")
            If Cell Is FIrstCell Then
                All.AddRange(Shorts)
            Else
                Dim tmp = All.Intersect(Shorts).ToList
                All = tmp
            End If

        Next
        Dim Last = All.Distinct.ToList
        ClassLibrary1.Szunyi.IO.Export.SaveText(ClassLibrary1.Szunyi.Text.General.GetText(Last))
    End Sub
End Class