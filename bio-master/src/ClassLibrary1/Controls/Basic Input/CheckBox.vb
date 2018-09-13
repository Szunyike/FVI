Namespace Controls
    Public Class CheckBox
        Private Sub GroupBox1_Enter(sender As Object, e As EventArgs) Handles GroupBox1.Enter

        End Sub
        Public Sub New(I_P As Szunyi.Outer_Programs.Input_Description)

            ' This call is required by the designer.
            InitializeComponent()
            Me.GroupBox1.Text = I_P.Flag
            ' Add any initialization after the InitializeComponent() call.
            CheckBox1.DataBindings.Add(New System.Windows.Forms.Binding("Text", I_P, "Description"))
            CheckBox1.DataBindings.Add(New System.Windows.Forms.Binding("Checked", I_P, "Default_Value"))
            Me.Dock = System.Windows.Forms.DockStyle.Top
        End Sub
    End Class
End Namespace

