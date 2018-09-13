Imports System.IO
Imports System.Windows.Forms
Imports System.Drawing
Imports ClassLibrary1

Public Class SelectFirstRowAndColumn_s_
    Dim Lines As List(Of String)
    Dim Separator As String = ""
    Public Property FirstLine As Integer
    Public Property SelectedColumns As New List(Of Integer)

    Dim Type As String
    Private Sub SelectFirstRowAndColumn_s__Load(sender As Object, e As EventArgs) Handles MyBase.Load


    End Sub
    Public Sub New(File As FileInfo, Type As String, Title As String)
        Me.Text = Title
        Lines = Szunyi.IO.Import.Text.ReadLines(File, 25)
        ' This call is required by the designer.
        InitializeComponent()
        Me.Type = Type
        Select Case Type
            Case Szunyi.Constants.DelimitedFileImport.SelectFirstRowAndColumn
                Me.Button1.Text = "First Row and Column is Selected"
            Case Szunyi.Constants.DelimitedFileImport.SelectFirstRowAndColumns
                Me.Button1.Text = "First Row and Columns are Selected"

            Case Szunyi.Constants.DelimitedFileImport.SelectHeaderRow

            Case Szunyi.Constants.DelimitedFileImport.SelectHeadersAndColumns

        End Select
        ' Add any initialization after the InitializeComponent() call.


    End Sub

    Private Sub RadioButton1_CheckedChanged(sender As Object, e As EventArgs) Handles RadioButton1.CheckedChanged
        ' Tab
        Me.Separator = vbTab
        SplitLines()
    End Sub

    Private Sub RadioButton2_CheckedChanged(sender As Object, e As EventArgs) Handles RadioButton2.CheckedChanged
        ' Semicolon
        Me.Separator = ";"
        SplitLines()
    End Sub

    Private Sub RadioButton3_CheckedChanged(sender As Object, e As EventArgs) Handles RadioButton3.CheckedChanged
        ' Colon
        Me.Separator = ":"
        SplitLines()
    End Sub

    Private Sub RadioButton4_CheckedChanged(sender As Object, e As EventArgs) Handles RadioButton4.CheckedChanged
        ' Space
        Me.Separator = " "
        SplitLines()
    End Sub

    Private Sub RadioButton5_CheckedChanged(sender As Object, e As EventArgs) Handles RadioButton5.CheckedChanged
        ' Other
        If TextBox1.Text <> "" Then
            Me.Separator = TextBox1.Text
            SplitLines()
        End If
    End Sub

    Private Sub TextBox1_TextChanged(sender As Object, e As EventArgs) Handles TextBox1.TextChanged
        If TextBox1.Text <> "" Then
            Me.Separator = TextBox1.Text
            SplitLines()
        End If
    End Sub
    Private Sub SplitLines()
        dgv1.Rows.Clear()
        dgv1.Columns.Clear()

        Dim x As New List(Of String())
        For Each Line In Lines
            x.Add(Split(Line, Separator))
        Next
        Dim MaxNofColumn = Szunyi.Text.General.GetMaxNofColumn(x)
        For i1 = 1 To MaxNofColumn
            Dim x1 As New DataGridViewCheckBoxColumn()
            dgv1.Columns.Add(x1)
            Dim x2 As New DataGridViewTextBoxColumn
            dgv1.Columns.Add(x2)
        Next

        For i1 = 0 To x.Count - 1
            Dim dgvNewRow As New DataGridViewRow
            For i2 = 0 To x(i1).Count - 1
                Dim t As New DataGridViewCheckBoxCell()

                dgvNewRow.Cells.Add(t)

                Dim t1 As New DataGridViewTextBoxCell
                t1.Value = x(i1)(i2).Replace(Chr(34), "")
                dgvNewRow.Cells.Add(t1)
                '        t.Enabled = True
            Next
            dgv1.Rows.Add(dgvNewRow)
        Next
        ' Me.dgv1.DataSource = x

    End Sub

    Private Sub dgv1_CellContentClick(sender As Object, e As DataGridViewCellEventArgs) Handles dgv1.CellContentClick

        Label1.Focus()
    End Sub



    Private Sub OK(sender As Object, e As EventArgs) Handles Button1.Click
        Me.SelectedColumns.Clear()
        For i1 = 0 To dgv1.RowCount - 1
            Me.FirstLine = i1
            For i2 = 0 To dgv1.ColumnCount - 1 Step 2
                If dgv1.Rows(i1).Cells(i2).Value = True Then
                    Me.SelectedColumns.Add(i2)
                End If
            Next
            If Me.SelectedColumns.Count > 0 Then
                Exit For
            End If
        Next
        Me.DialogResult = DialogResult.OK
        Me.Close()
    End Sub
End Class


Public Class MyDGVCheckBoxColumn
    Inherits DataGridViewCheckBoxColumn
    Private m_label As String

    Public Property Label() As String
        Get
            Return m_label
        End Get
        Set
            m_label = Value
        End Set
    End Property

    Public Overrides Property CellTemplate() As DataGridViewCell
        Get
            Return New MyDGVCheckBoxCell()
        End Get
        Set(value As DataGridViewCell)

        End Set
    End Property
End Class

Public Class MyDGVCheckBoxCell
    Inherits DataGridViewCheckBoxCell
    Private m_label As String

    Public Property Label() As String
        Get
            Return m_label
        End Get
        Set
            m_label = Value
        End Set
    End Property


    Protected Overrides Sub Paint(graphics As Graphics, clipBounds As Rectangle, cellBounds As Rectangle, rowIndex As Integer, elementState As DataGridViewElementStates, value As Object,
        formattedValue As Object, errorText As String, cellStyle As DataGridViewCellStyle, advancedBorderStyle As DataGridViewAdvancedBorderStyle, paintParts As DataGridViewPaintParts)

        ' the base Paint implementation paints the check box
        MyBase.Paint(graphics, clipBounds, cellBounds, rowIndex, elementState, value,
            formattedValue, errorText, cellStyle, advancedBorderStyle, paintParts)

        ' Get the check box bounds: they are the content bounds
        Dim contentBounds As Rectangle = Me.GetContentBounds(rowIndex)

        ' Compute the location where we want to paint the string.
        Dim stringLocation As New Point()

        ' Compute the Y.
        ' NOTE: the current logic does not take into account padding.
        stringLocation.Y = cellBounds.Y + 2


        ' Compute the X.
        ' Content bounds are computed relative to the cell bounds
        ' - not relative to the DataGridView control.
        stringLocation.X = cellBounds.X + contentBounds.Right + 2


        ' Paint the string.
        If Me.Label Is Nothing Then
            Dim col As MyDGVCheckBoxColumn = DirectCast(Me.OwningColumn, MyDGVCheckBoxColumn)
            Me.m_label = col.Label
        End If

        graphics.DrawString(Me.Label, Control.DefaultFont, System.Drawing.Brushes.Red, stringLocation)

    End Sub

End Class

'=======================================================
'Service provided by Telerik (www.telerik.com)
'Conversion powered by NRefactory.
'Twitter: @telerik
'Facebook: facebook.com/telerik
'=======================================================
