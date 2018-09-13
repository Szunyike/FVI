Imports System.ComponentModel
Imports System.IO
Imports ClassLibrary1

Public Class CheckBoxForStringsFull
    Dim Items As New BindingList(Of ForCheckBox)
    Dim cItems As New BindingList(Of ForCheckBox)
    Dim cLengthOfSearchString As Integer = 0
    Public Property SelectedStrings As New List(Of String)
    Public Property SelectedIndexes As New List(Of Integer)
    Public Property SelectedFiles As New List(Of FileInfo)
    Public Property SelectedObj As New List(Of Object)
    Public Property Max_Nof_SelectedItems As Integer
    Public Property SelSeqs As New List(Of Bio.ISequence)
    Private Sub SelectQualifiers_Load(sender As Object, e As EventArgs) Handles MyBase.Load


    End Sub
#Region "New"
    Public Sub New()

        ' This call is required by the designer.
        InitializeComponent()

        ' Add any initialization after the InitializeComponent() call.

    End Sub
    ''' <summary>
    ''' Default is Qulifier + Location
    ''' </summary>
    Public Sub New(Strings As List(Of String),
                   Max_Nof_SelectedItems As Integer,
                   Optional Title As String = "", Optional Selected_Items As List(Of String) = Nothing)

        ' This call is required by the designer.
        InitializeComponent()
        Me.Max_Nof_SelectedItems = Max_Nof_SelectedItems
        Me.Text = Title
        For Each Item In Strings
            Items.Add(New ForCheckBox(Item, Item))
            cItems.Add(New ForCheckBox(Item, Item))
        Next
        Set_Selection(Selected_Items)
        Init()


        ' Add any initialization after the InitializeComponent() call.

    End Sub
    Public Sub New(Strings As SortedList(Of String, Short), Max_Nof_SelectedItems As Integer, Optional Title As String = "")

        ' This call is required by the designer.
        InitializeComponent()
        Me.Max_Nof_SelectedItems = Max_Nof_SelectedItems
        Me.Text = Title
        For Each Item In Strings.Keys
            Items.Add(New ForCheckBox(Item, Item))
            cItems.Add(New ForCheckBox(Item, Item))
        Next
        Init()


        ' Add any initialization after the InitializeComponent() call.

    End Sub
    Public Sub New(Strings As List(Of Bio.ISequence),
                   Max_Nof_SelectedItems As Integer,
                   Optional Title As String = "", Optional Selected_Items As List(Of String) = Nothing)

        ' This call is required by the designer.
        InitializeComponent()
        Me.Max_Nof_SelectedItems = Max_Nof_SelectedItems
        Me.Text = Title
        For Each Item In Strings
            Items.Add(New ForCheckBox(Item.ID, Item))
            cItems.Add(New ForCheckBox(Item.ID, Item))
        Next
        Set_Selection(Selected_Items)
        Init()


        ' Add any initialization after the InitializeComponent() call.

    End Sub

    ''' <summary>
    ''' Select From Files
    ''' </summary>
    ''' <param name="Files"></param>
    Public Sub New(Files As List(Of FileInfo),
                   Max_Nof_SelectedItems As Integer,
                   Optional Title As String = "",
                   Optional Selected_Files As List(Of FileInfo) = Nothing)

        ' This call is required by the designer.
        InitializeComponent()
        Me.Max_Nof_SelectedItems = Max_Nof_SelectedItems
        Me.Text = Title
        ' Add any initialization after the InitializeComponent() call.
        For Each File In Files
            Items.Add(New ForCheckBox(File.Name, File))
            cItems.Add(New ForCheckBox(File.Name, File))
        Next
        Init()
        If IsNothing(SelectedFiles) = False Then
            Dim Selected_File_Names = (From x In SelectedFiles Select x.Name).ToList
            Set_Selection(Selected_File_Names)
        End If

    End Sub
#End Region
    Private Sub Init()
        CheckedListBox1.DataSource = cItems
        CheckedListBox1.DisplayMember = "Name"

    End Sub
    Private Sub Set_Selection(Selected_Items As List(Of String))
        If IsNothing(Selected_Items) = True Then Exit Sub
        Dim alf As Int16 = 54
        For Each cItem In cItems
            For Each s In Selected_Items
                Dim i1 = s.IndexOf(cItem.Name, comparisonType:=StringComparison.InvariantCultureIgnoreCase)
                If i1 <> -1 Then
                    cItem.Checked = True
                End If
            Next

        Next
        SetIt()
    End Sub
    Private Sub OK(sender As Object, e As EventArgs) Handles Button1.Click
        Me.DialogResult = DialogResult.OK

        Dim s As String = ""
        Dim f As New FileInfo("C:\TEMP.TMP")
        Dim t = From x In Me.Items Where x.Checked = True Select x
        Dim t1 = From x1 In Me.CheckedListBox1.Items Where x1.Checked = True Select x1

        If t.Count > Max_Nof_SelectedItems AndAlso Max_Nof_SelectedItems <> -1 Then
            MsgBox("Too many Selected Items")
            Exit Sub
        End If
        If t1.Count > t.Count Then
            Select Case t1.First.obj.GetType

                Case s.GetType
                    For Each Item In t1
                        Me.SelectedStrings.Add(Item.Name)
                    Next
                Case f.GetType
                    For Each Item In t1
                        Me.SelectedFiles.Add(Item.obj)
                    Next
                Case Else 'Seq
                    For Each Item In t1
                        Me.SelSeqs.Add(Item.obj)
                    Next
            End Select
            For i1 = 0 To Me.Items.Count - 1
                If Me.Items(i1).Checked = True Then
                    Me.SelectedIndexes.Add(i1)
                End If
            Next
            For i1 = 0 To Me.CheckedListBox1.Items.Count - 1
                If Me.CheckedListBox1.Items(i1).Checked = True Then
                    Me.SelectedIndexes.Add(i1)
                End If
            Next
        Else
            If t.Count > 0 Then
                Select Case t.First.obj.GetType

                    Case s.GetType
                        For Each Item In t
                            Me.SelectedStrings.Add(Item.Name)
                        Next
                    Case f.GetType
                        For Each Item In t
                            Me.SelectedFiles.Add(Item.obj)
                        Next
                    Case Else 'Seq
                        For Each Item In t
                            Me.SelSeqs.Add(Item.obj)
                        Next
                End Select
                For i1 = 0 To Me.Items.Count - 1
                    If Me.Items(i1).Checked = True Then
                        Me.SelectedIndexes.Add(i1)
                    End If
                Next
                For i1 = 0 To Me.CheckedListBox1.Items.Count - 1
                    If Me.CheckedListBox1.Items(i1).Checked = True Then
                        Me.SelectedIndexes.Add(i1)
                    End If
                Next
            End If
        End If

            Me.Close()
    End Sub

    Private Sub Cancel(sender As Object, e As EventArgs) Handles Button2.Click
        Me.DialogResult = DialogResult.Cancel
        Me.Close()
    End Sub
    Private Sub SetIt()
        CheckedListBox1.DataSource = cItems
        CheckedListBox1.DisplayMember = "Name"
        CheckedListBox1.ValueMember = "Checked"
        For i1 = 0 To cItems.Count - 1
            CheckedListBox1.SetItemChecked(i1, cItems(i1).Checked)
        Next
    End Sub

    Private Sub TextBox1_TextChanged(sender As Object, e As EventArgs) Handles TextBox1.TextChanged
        If TextBox1.Text.Length > 0 Then
            If cLengthOfSearchString < TextBox1.Text.Length Then Me.cItems = Me.Items
            Me.cItems = Filter()
            SetIt()
        Else

            Me.cItems = Items
            SetIt()
        End If
        Me.cLengthOfSearchString = TextBox1.Text.Length
    End Sub
    Private Function Filter() As BindingList(Of ForCheckBox)
        Dim s1() = Split(TextBox1.Text, " ")
        For Each s In s1
            Me.cItems = Filter(s)
        Next

        Return Me.cItems

    End Function
    Private Function Filter(s As String) As BindingList(Of ForCheckBox)

        Dim t = From x In Me.cItems Where x.Name.IndexOf(s, StringComparison.InvariantCultureIgnoreCase) > -1 Select x

        If t.Count > 0 Then
            Dim h As New BindingList(Of ForCheckBox)
            For Each Item In t
                h.Add(Item)
            Next
            Return h
        Else
            Return New BindingList(Of ForCheckBox)
        End If


    End Function

    Private Sub CheckedListBox1_ItemCheck(sender As Object, e As ItemCheckEventArgs) Handles CheckedListBox1.ItemCheck

        Dim x As ForCheckBox = CheckedListBox1.Items(e.Index)
        For Each Item In cItems
            '       If Item.Name = x.Name Then Item.Checked = e.NewValue
        Next
        For Each Item In Items
            '      If Item.Name = x.Name Then Item.Checked = e.NewValue
        Next
        x.Checked = e.NewValue

    End Sub

    Private Sub CheckBox1_CheckedChanged(sender As Object, e As EventArgs) Handles CheckBox1.CheckedChanged
        If CheckBox1.Checked = True Then
            For Each Item In Me.cItems
                Item.Checked = True
            Next
            SetIt()
        Else ' Do Unselect Current Visible Items
            For Each Item In Me.cItems
                Item.Checked = False
            Next
            SetIt()
        End If

    End Sub

    Public Sub Clear_Selections()
        For Each cItem In cItems

            cItem.Checked = False

        Next
        Me.SelectedStrings.Clear()
        Me.SelectedFiles.Clear()
        Me.SelectedIndexes.Clear()
        Me.SelectedObj.Clear()
        Me.SelSeqs.Clear()
        SetIt()
    End Sub
End Class

Public Class ForCheckBox
    Public Property Checked As Boolean = False
    Public Property Name As String
    Public Property obj As Object
    Public Sub New(Name As String)
        Me.Name = Name
    End Sub
    Public Sub New(Name As String, obj As Object)
        Me.Name = Name
        Me.obj = obj
    End Sub
End Class

