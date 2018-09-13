Imports System.Drawing
Imports System.IO
Imports System.Windows.Forms
Imports System.Xml.Serialization

Public Class PhyloGeneticClorPicker
    Public Property BasicForColors As New List(Of BasicForColor)
    Public Property ListBox2Items As New List(Of BasicForColor)
    Public Property ColorGroups As New List(Of ColorGroup)
    Public Property ColorSchemas As New List(Of ColorSchema)

    Public Sub New(Vals As List(Of String()))

        ' This call is required by the designer.
        InitializeComponent()

        ' Add any initialization after the InitializeComponent() call.
        For i1 = 1 To Vals.Count - 1
            If IsNothing(Vals(i1)) = False Then
                For Each Item In Vals(i1)
                    BasicForColors.Add(New BasicForColor(Item, i1))
                Next
            End If
        Next
        ListBox1.DataSource = BasicForColors
        ListBox1.DisplayMember = "Name"

        ListBox2.DataSource = ListBox2Items
        ListBox2.DisplayMember = "Name"

    End Sub
    Private Sub RefreshLB2()
        ListBox2.DataSource = Nothing
        ListBox2.DataSource = ListBox2Items
        ListBox2.DisplayMember = "Name"
        ListBox2.Refresh()
    End Sub
    Private Sub RefreshLB3()
        ListBox3.DataSource = Nothing
        ListBox3.DataSource = ColorGroups
        ListBox3.DisplayMember = "Name"
        ListBox3.Refresh()
    End Sub
    Private Sub RefreshLB4()
        ListBox4.DataSource = Nothing
        ListBox4.DataSource = Me.ColorSchemas
        ListBox4.DisplayMember = "Name"
        ListBox4.Refresh()
    End Sub
    Private Sub LB1_DoubleClick(sender As Object, e As EventArgs) Handles ListBox1.DoubleClick
        Dim x = ListBox1.SelectedItem
        If ListBox2Items.Contains(x) = False Then ListBox2Items.Add(x)
        RefreshLB2()
    End Sub
    Private Sub LB2_DoubleClick(sender As Object, e As EventArgs) Handles ListBox2.DoubleClick
        Dim x = ListBox2.SelectedItem
        ListBox2Items.Remove(x)
        RefreshLB2()
    End Sub

    Private Sub CreateColor(sender As Object, e As EventArgs) Handles Button1.Click
        If Me.ListBox2Items.Count = 0 Then Exit Sub
        If ColorDialog1.ShowDialog = System.Windows.Forms.DialogResult.OK Then
            Dim x As ColorGroup = New ColorGroup(ColorDialog1.Color, ListBox2Items) ' 

            Me.ColorGroups.Add(x)
            RefreshLB3()
        End If

    End Sub

    Private Sub CreateSchmae(sender As Object, e As EventArgs) Handles Button2.Click

        Dim x As New ColorSchema(Me.ColorGroups)
        Me.ColorSchemas.Add(x)
        RefreshLB4()

    End Sub



    Private Sub Save(sender As Object, e As EventArgs) Handles Button3.Click
        Dim mySerializer As XmlSerializer = New XmlSerializer(GetType(ColorSchema))
        ' To write to a file, create a StreamWriter object.
        Dim Path = Szunyi.IO.Directory.Get_Folder
        For Each Schema In Me.ColorSchemas
            Dim myWriter As StreamWriter = New StreamWriter(Path.FullName & "\" & Schema.Name & ".xml")
            mySerializer.Serialize(myWriter, Schema)
            myWriter.Close()
        Next

    End Sub



    Private Sub ListBox3_DrawItem(sender As Object, e As DrawItemEventArgs) Handles ListBox3.DrawItem
        Dim Item As ColorGroup = ListBox3.Items(e.Index)
        e.DrawBackground()

        Dim myBrush As New SolidBrush(Item.Color)

        e.Graphics.DrawString(Item.Name, e.Font, myBrush, e.Bounds, StringFormat.GenericDefault)

        '
        e.DrawFocusRectangle()




    End Sub

    Private Sub DeleteLB3Item(sender As Object, e As EventArgs) Handles Button4.Click
        Dim Index = ListBox3.SelectedIndex
        Dim Item = ListBox3.SelectedItem
        If IsNothing(Item) = False Then
            Me.ColorGroups.Remove(Item)
            RefreshLB3()
        End If

    End Sub
End Class

<Serializable>
Public Class BasicForColor
    Public Property Name As String
    Public Property Index As Integer
    Public Sub New(Name As String, Index As Integer)
        Me.Name = Name
        Me.Index = Index
    End Sub
    Public Sub New()

    End Sub
End Class

<Serializable>
Public Class ColorGroup
    Public Property Name As String
    Public Property Color As ColorEx
    Public Property Items As New List(Of BasicForColor)
    Public Sub New(Color As Color, Items As List(Of BasicForColor))
        If Items.Count = 1 Then
            Me.Name = Items.First.Name
        Else
            Me.Name = InputBox("Enter The Name of The Schema")
        End If

        Me.Color = Color
        For Each Item In Items
            Me.Items.Add(Item)
        Next

    End Sub
    Public Sub New()

    End Sub
End Class

<Serializable>
Public Class ColorSchema
    Public Property Name As String
    Public Property ColorGroups As New List(Of ColorGroup)
    Public Sub New()

    End Sub
    Public Sub New(ColorGroups As List(Of ColorGroup))
        For Each s In ColorGroups
            Me.ColorGroups.Add(s)
        Next

        Do
            Me.Name = InputBox("Enter The Name of The Schema!")
            If Me.Name.Length > 2 Then Exit Do
            MsgBox("Name must be at Least Three Letter!")
        Loop
    End Sub

End Class

<Serializable>
Public Structure ColorEx
    Private m_color As Color

    Public Sub New(color As Color)
        m_color = color
    End Sub

    <XmlIgnore>
    Public Property Color() As Color
        Get
            Return m_color
        End Get
        Set
            m_color = Value
        End Set
    End Property

    <XmlAttribute>
    Public Property ColorHtml() As String
        Get
            Return ColorTranslator.ToOle(Me.Color)
        End Get
        Set
            Me.Color = ColorTranslator.FromOle(Value)
        End Set
    End Property

    Public Shared Widening Operator CType(colorEx As ColorEx) As Color
        Return colorEx.Color
    End Operator

    Public Shared Widening Operator CType(color As Color) As ColorEx
        Return New ColorEx(color)
    End Operator
End Structure
