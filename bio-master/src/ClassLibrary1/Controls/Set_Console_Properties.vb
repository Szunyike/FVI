Imports System.Windows.Forms
Imports ClassLibrary1.Controls

Namespace Controls
    Public Class Set_Console_Properties
        Public Property Input_Descriptions As New List(Of Szunyi.Outer_Programs.Input_Description)
        Public Property obj As Object
        Public Sub New(obj As Object)
            Me.obj = obj
            ' This call is required by the designer.
            InitializeComponent()

            ' Add any initialization after the InitializeComponent() call.
            Dim t = obj.GetType
            If t.IsClass = True Then
                Dim Props = t.GetProperties
                Dim gr1 As New OK_Cancel

                '  Panel1.Controls.Add(gr1)
                AddHandler gr1.Button1.Click, AddressOf bOK_CLick
                AddHandler gr1.Button2.Click, AddressOf bCancel_Click
                For Each Prop In Props
                    Dim Pinfo = Prop.PropertyType.GetProperties()
                    Dim t1 = Prop.GetType
                    Dim I_P As Szunyi.Outer_Programs.Input_Description = Prop.GetValue(obj)
                    '       Dim pinfo2 = sI_Pg2.GetType.GetProperties
                    '   Dim x1 = CType(Prop, Szunyi.Outer_Programs.Input_Description)
                    Me.Input_Descriptions.Add(I_P)

                    Select Case I_P.Type
                            Case Szunyi.Outer_Programs.Input_Description_Type.Boolean
                            Dim gr As New CheckBox(I_P)

                            Me.Panel1.Controls.Add(gr)
                            Case Szunyi.Outer_Programs.Input_Description_Type.Double
                                Dim r As New Get_Double(I_P)
                                Me.Panel1.Controls.Add(r)
                            Case Szunyi.Outer_Programs.Input_Description_Type.Integer
                                Dim r As New Get_Integer(I_P)
                                Me.Panel1.Controls.Add(r)
                            Case Szunyi.Outer_Programs.Input_Description_Type.Selection
                                Dim x As New cb(I_P)
                                Me.Panel1.Controls.Add(x)
                        Case Szunyi.Outer_Programs.Input_Description_Type.String
                            Dim x As New GroupBox
                            Me.Panel1.Controls.Add(x)

                            Dim x1 As New Label
                            x1.DataBindings.Add(New System.Windows.Forms.Binding("Text", I_P, "Description"))
                            x1.Dock = DockStyle.Top
                            x.Controls.Add(x1)

                            Dim x2 As New TextBox
                            x2.DataBindings.Add(New System.Windows.Forms.Binding("Text", I_P, "Default_Value"))
                            x2.Dock = DockStyle.Top
                            x.Controls.Add(x2)
                                x.Dock = DockStyle.Top
                                Me.Panel1.Controls.Add(x)
                        End Select

                    Next
                Panel1.Controls.Add(gr1)



            End If

        End Sub

        Private Sub bCancel_Click(sender As Object, e As EventArgs)
            Me.DialogResult = DialogResult.Cancel
            Me.Close()
        End Sub

        Private Sub bOK_CLick(sender As Object, e As EventArgs)
            Me.DialogResult = DialogResult.OK
            Me.Close()

        End Sub
    End Class
End Namespace
