Namespace Controls
    <Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()>
    Partial Class Get_Integer
        Inherits System.Windows.Forms.UserControl

        'UserControl overrides dispose to clean up the component list.
        <System.Diagnostics.DebuggerNonUserCode()>
        Protected Overrides Sub Dispose(ByVal disposing As Boolean)
            Try
                If disposing AndAlso components IsNot Nothing Then
                    components.Dispose()
                End If
            Finally
                MyBase.Dispose(disposing)
            End Try
        End Sub

        'Required by the Windows Form Designer
        Private components As System.ComponentModel.IContainer

        'NOTE: The following procedure is required by the Windows Form Designer
        'It can be modified using the Windows Form Designer.  
        'Do not modify it using the code editor.
        <System.Diagnostics.DebuggerStepThrough()>
        Private Sub InitializeComponent()
            Me.GroupBox1 = New System.Windows.Forms.GroupBox()
            Me.TextBox1 = New System.Windows.Forms.TextBox()
            Me.Label1 = New System.Windows.Forms.Label()
            Me.GroupBox1.SuspendLayout()
            Me.SuspendLayout()
            '
            'GroupBox1
            '
            Me.GroupBox1.Controls.Add(Me.TextBox1)
            Me.GroupBox1.Controls.Add(Me.Label1)
            Me.GroupBox1.Dock = System.Windows.Forms.DockStyle.Fill
            Me.GroupBox1.Location = New System.Drawing.Point(0, 0)
            Me.GroupBox1.Name = "GroupBox1"
            Me.GroupBox1.Size = New System.Drawing.Size(297, 70)
            Me.GroupBox1.TabIndex = 1
            Me.GroupBox1.TabStop = False
            '
            'TextBox1
            '
            Me.TextBox1.Dock = System.Windows.Forms.DockStyle.Top
            Me.TextBox1.Location = New System.Drawing.Point(3, 29)
            Me.TextBox1.Name = "TextBox1"
            Me.TextBox1.Size = New System.Drawing.Size(291, 20)
            Me.TextBox1.TabIndex = 1
            '
            'Label1
            '
            Me.Label1.AutoSize = True
            Me.Label1.Dock = System.Windows.Forms.DockStyle.Top
            Me.Label1.Location = New System.Drawing.Point(3, 16)
            Me.Label1.Name = "Label1"
            Me.Label1.Size = New System.Drawing.Size(39, 13)
            Me.Label1.TabIndex = 0
            Me.Label1.Text = "Label1"
            '
            'Get_Integer
            '
            Me.AutoScaleDimensions = New System.Drawing.SizeF(6.0!, 13.0!)
            Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
            Me.Controls.Add(Me.GroupBox1)
            Me.Name = "Get_Integer"
            Me.Size = New System.Drawing.Size(297, 70)
            Me.GroupBox1.ResumeLayout(False)
            Me.GroupBox1.PerformLayout()
            Me.ResumeLayout(False)

        End Sub

        Friend WithEvents GroupBox1 As System.Windows.Forms.GroupBox
        Friend WithEvents TextBox1 As System.Windows.Forms.TextBox
        Friend WithEvents Label1 As System.Windows.Forms.Label

        Private Sub GroupBox1_Enter(sender As Object, e As EventArgs) Handles GroupBox1.Enter

        End Sub
    End Class
End Namespace
