Imports System.ComponentModel
Imports System.Windows.Forms
Imports ClassLibrary1
Imports ClassLibrary1.Szunyi.Basic

<Serializable>
Public Class ListManager
    <NonSerialized()>
    Public NamesAndIDs As New BindingList(Of Name_ID_Type_Title_Object)

    Public cNamesAndIDs As New BindingList(Of Name_ID_Type_Title_Object)
    Public Property SelectedName As String

    Dim cLengthOfSearchString As Integer = 0
    Dim SlectedIndexes As New List(Of Integer)
    <NonSerialized()>
    Public Event NewSelection(x As Name_ID_Type_Title_Object)
    Public Event TextBoxEnter(x As String)
#Region "Add"
    ''' <summary>
    ''' Only String, It will Create NameAndIDAndTypes
    ''' </summary>
    ''' <param name="Items"></param>
    Public Sub AddRange(Items As IReadOnlyList(Of String))
        Me.cNamesAndIDs.Clear()
        For i1 = 0 To Items.Count - 1
            Me.NamesAndIDs.Add(New Name_ID_Type_Title_Object(Items(i1), i1, Items(i1), Items(i1), Items(i1)))
            Me.cNamesAndIDs.Add(New Name_ID_Type_Title_Object(Items(i1), i1, Items(i1), Items(i1), Items(i1)))
        Next
        ListBox1.DataSource = Me.cNamesAndIDs
        ListBox1.DisplayMember = "Name"
        ListBox1.Refresh()
    End Sub
    ''' <summary>
    ''' At First Clear
    ''' </summary>
    ''' <param name="Items"></param>
    Public Sub AddRange(Items As List(Of Name_ID_Type_Title_Object))
        If IsNothing(Me.cNamesAndIDs) = True Then Me.cNamesAndIDs = New BindingList(Of Name_ID_Type_Title_Object)
        Me.cNamesAndIDs.Clear()
        For Each Item In Items
            NamesAndIDs.Add(New Name_ID_Type_Title_Object(Item.Name, Item.ID, Item.Type, Item.Name, Item))
            cNamesAndIDs.Add(New Name_ID_Type_Title_Object(Item.Name, Item.ID, Item.Type, Item.Name, Item))
        Next
        ListBox1.DataSource = Me.cNamesAndIDs
        ListBox1.DisplayMember = "Name"
        ListBox1.Refresh()
    End Sub
    Public Sub Add(x As Name_ID_Type_Title_Object)
        Me.NamesAndIDs.Add(x)
        Me.cNamesAndIDs.Add(x)
        ListBox1.Refresh()
    End Sub
#End Region

#Region "New"
    Public Sub New(Items As List(Of String))

        ' This call is required by the designer.
        InitializeComponent()

        ' Add any initialization after the InitializeComponent() call.
        Me.cNamesAndIDs.Clear()
        For i1 = 0 To Items.Count - 1
            Me.NamesAndIDs.Add(New Name_ID_Type_Title_Object(Items(i1), i1, Items(i1), Items(i1), Items(i1)))
            Me.cNamesAndIDs.Add(New Name_ID_Type_Title_Object(Items(i1), i1, Items(i1), Items(i1), Items(i1)))
        Next
        ListBox1.DataSource = Me.cNamesAndIDs
        ListBox1.DisplayMember = "Name"
        ListBox1.Refresh()
    End Sub
    Public Sub New(Items As List(Of Name_ID_Type_Title_Object), Title As String)

        ' This call is required by the designer.
        InitializeComponent()

        ' Add any initialization after the InitializeComponent() call.
        For Each Item In Items
            Me.NamesAndIDs.Add(Item)
            Me.cNamesAndIDs.Add(Item)
        Next
        Me.GroupBox1.Text = Title
        ListBox1.DataSource = Me.cNamesAndIDs
        ListBox1.DisplayMember = "Name"

    End Sub
    Public Sub New(Title As String)

        ' This call is required by the designer.
        InitializeComponent()

        ' Add any initialization after the InitializeComponent() call.
        GroupBox1.Text = Title
        ListBox1.DataSource = Me.cNamesAndIDs
        ListBox1.DisplayMember = "Name"

    End Sub
    Public Sub New()

        ' This call is required by the designer.
        InitializeComponent()

        ' Add any initialization after the InitializeComponent() call.
        ListBox1.DataSource = Me.cNamesAndIDs
        ListBox1.DisplayMember = "Name"
    End Sub
#End Region

    Private Sub TextBox1_TextChanged(sender As Object, e As EventArgs) Handles TextBox1.TextChanged
        If TextBox1.Text.Length > 0 Then
            If cLengthOfSearchString < TextBox1.Text.Length Then Me.cNamesAndIDs = Me.NamesAndIDs
            Me.cNamesAndIDs = Filter()
            ListBox1.DataSource = cNamesAndIDs
        Else

            Me.cNamesAndIDs = NamesAndIDs
            ListBox1.DataSource = cNamesAndIDs
        End If
        Me.cLengthOfSearchString = TextBox1.Text.Length
    End Sub
    Private Function Filter() As BindingList(Of Name_ID_Type_Title_Object)
        Dim t = From x In Me.cNamesAndIDs Where x.Name.IndexOf(TextBox1.Text, StringComparison.OrdinalIgnoreCase) > -1 Select x

        If t.Count > 0 Then
            Dim h As New BindingList(Of Name_ID_Type_Title_Object)
            For Each Item In t
                h.Add(Item)
            Next
            Return h
        End If

        Return New BindingList(Of Name_ID_Type_Title_Object)

    End Function

    Private Sub ListBox1_DoubleClick(sender As Object, e As EventArgs) Handles ListBox1.DoubleClick

        If ListBox1.SelectedIndex > -1 Then
            Dim t As Name_ID_Type_Title_Object = ListBox1.SelectedItem
            Me.SelectedName = t.Name
            RaiseEvent NewSelection(ListBox1.SelectedItem)
        End If
    End Sub

    Public Function GetSelectedItems() As List(Of Name_ID_Type_Title_Object)
        Dim out As New List(Of Name_ID_Type_Title_Object)
        For Each Item In Me.ListBox1.SelectedItems
            out.Add(Item)
        Next
        Return out
    End Function

    Public Sub SetGroupBoxName(Title As String)
        Me.GroupBox1.Text = Title
    End Sub

    Private Sub TextBox1_KeyDown(sender As Object, e As KeyEventArgs) Handles TextBox1.KeyDown
        If e.KeyCode = Keys.Enter Then
            RaiseEvent TextBoxEnter(TextBox1.Text)
        End If
    End Sub

    Public Sub SetSelectedItem(name As String)
        For i1 = 0 To ListBox1.Items.Count - 1
            Dim t As Name_ID_Type_Title_Object = ListBox1.Items(i1)
            If t.Name = name Then
                ListBox1.SetSelected(i1, True)
                Exit Sub
            End If
        Next
    End Sub
End Class



