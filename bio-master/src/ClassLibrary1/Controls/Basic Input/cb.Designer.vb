Namespace Controls
    <Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()>
    Partial Class cb
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
            Me.Label1 = New System.Windows.Forms.Label()
            Me.ComboBox1 = New System.Windows.Forms.ComboBox()
            Me.GroupBox1.SuspendLayout()
            Me.SuspendLayout()
            '
            'GroupBox1
            '
            Me.GroupBox1.Controls.Add(Me.ComboBox1)
            Me.GroupBox1.Controls.Add(Me.Label1)
            Me.GroupBox1.Dock = System.Windows.Forms.DockStyle.Fill
            Me.GroupBox1.Location = New System.Drawing.Point(0, 0)
            Me.GroupBox1.Name = "GroupBox1"
            Me.GroupBox1.Size = New System.Drawing.Size(256, 62)
            Me.GroupBox1.TabIndex = 0
            Me.GroupBox1.TabStop = False
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
            'ComboBox1
            '
            Me.ComboBox1.Dock = System.Windows.Forms.DockStyle.Top
            Me.ComboBox1.FormattingEnabled = True
            Me.ComboBox1.Location = New System.Drawing.Point(3, 29)
            Me.ComboBox1.Name = "ComboBox1"
            Me.ComboBox1.Size = New System.Drawing.Size(250, 21)
            Me.ComboBox1.TabIndex = 1
            '
            'cb
            '
            Me.AutoScaleDimensions = New System.Drawing.SizeF(6.0!, 13.0!)
            Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
            Me.Controls.Add(Me.GroupBox1)
            Me.Name = "cb"
            Me.Size = New System.Drawing.Size(256, 62)
            Me.GroupBox1.ResumeLayout(False)
            Me.GroupBox1.PerformLayout()
            Me.ResumeLayout(False)

        End Sub

        Friend WithEvents GroupBox1 As System.Windows.Forms.GroupBox
        Friend WithEvents ComboBox1 As System.Windows.Forms.ComboBox
        Friend WithEvents Label1 As System.Windows.Forms.Label
    End Class
End Namespace
