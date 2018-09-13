Imports ClassLibrary1.Szunyi.Outer_Programs
Namespace Controls
    Public Class cb
        Public Property I_P As Input_Description
        Dim Values As List(Of String)
        Dim Description As String
        Public Property SelectedValue As String

        Public Sub New(i_P As Input_Description)
            ' This call is required by the designer.
            InitializeComponent()

            ' Add any initialization after the InitializeComponent() call.

            Me.GroupBox1.Text = i_P.Flag
            Me.I_P = i_P
            Me.Label1.Text = i_P.Description
            Me.Values = i_P.Values

            Me.Description = i_P.Description
            ComboBox1.DataSource = Me.Values
            Me.Dock = System.Windows.Forms.DockStyle.Top
            ComboBox1.DataBindings.Add(New System.Windows.Forms.Binding("Text", i_P, "Selected_Value"))
            ComboBox1.SelectedIndex = i_P.Default_Value
        End Sub

        Private Sub ComboBox1_SelectedIndexChanged(sender As Object, e As EventArgs) Handles ComboBox1.SelectedIndexChanged
            '  Me.Label1.Text = Descriptions(ComboBox1.SelectedIndex)
            I_P.Selected_Value = ComboBox1.Items(ComboBox1.SelectedIndex)

        End Sub
    End Class
End Namespace