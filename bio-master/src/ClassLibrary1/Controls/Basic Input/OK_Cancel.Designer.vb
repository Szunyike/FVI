Namespace Controls
    <Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()>
    Partial Class OK_Cancel
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
            Me.Button1 = New System.Windows.Forms.Button()
            Me.Button2 = New System.Windows.Forms.Button()
            Me.GroupBox1.SuspendLayout()
            Me.SuspendLayout()
            '
            'GroupBox1
            '
            Me.GroupBox1.Controls.Add(Me.Button2)
            Me.GroupBox1.Controls.Add(Me.Button1)
            Me.GroupBox1.Dock = System.Windows.Forms.DockStyle.Fill
            Me.GroupBox1.Location = New System.Drawing.Point(0, 0)
            Me.GroupBox1.Name = "GroupBox1"
            Me.GroupBox1.Size = New System.Drawing.Size(200, 59)
            Me.GroupBox1.TabIndex = 0
            Me.GroupBox1.TabStop = False
            '
            'Button1
            '
            Me.Button1.Location = New System.Drawing.Point(6, 19)
            Me.Button1.Name = "Button1"
            Me.Button1.Size = New System.Drawing.Size(75, 23)
            Me.Button1.TabIndex = 0
            Me.Button1.Text = "Ok"
            Me.Button1.UseVisualStyleBackColor = True
            '
            'Button2
            '
            Me.Button2.Location = New System.Drawing.Point(101, 19)
            Me.Button2.Name = "Button2"
            Me.Button2.Size = New System.Drawing.Size(75, 23)
            Me.Button2.TabIndex = 2
            Me.Button2.Text = "Cancel"
            Me.Button2.UseVisualStyleBackColor = True
            '
            'OK_Cancel
            '
            Me.AutoScaleDimensions = New System.Drawing.SizeF(6.0!, 13.0!)
            Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
            Me.Controls.Add(Me.GroupBox1)
            Me.Name = "OK_Cancel"
            Me.Size = New System.Drawing.Size(200, 59)
            Me.GroupBox1.ResumeLayout(False)
            Me.ResumeLayout(False)

        End Sub

        Friend WithEvents GroupBox1 As System.Windows.Forms.GroupBox
        Friend WithEvents Button2 As System.Windows.Forms.Button
        Friend WithEvents Button1 As System.Windows.Forms.Button
    End Class
End Namespace