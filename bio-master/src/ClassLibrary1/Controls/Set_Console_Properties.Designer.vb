Namespace Controls
    <Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()>
    Partial Class Set_Console_Properties
        Inherits System.Windows.Forms.Form

        'Form overrides dispose to clean up the component list.
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
            Me.Panel1 = New System.Windows.Forms.Panel()
            Me.SuspendLayout()
            '
            'Panel1
            '
            Me.Panel1.AutoScroll = True
            Me.Panel1.AutoSize = True
            Me.Panel1.Dock = System.Windows.Forms.DockStyle.Fill
            Me.Panel1.Location = New System.Drawing.Point(0, 0)
            Me.Panel1.Name = "Panel1"
            Me.Panel1.Size = New System.Drawing.Size(472, 569)
            Me.Panel1.TabIndex = 0
            '
            'Set_Console_Properties
            '
            Me.AutoScaleDimensions = New System.Drawing.SizeF(6.0!, 13.0!)
            Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
            Me.ClientSize = New System.Drawing.Size(472, 569)
            Me.Controls.Add(Me.Panel1)
            Me.Name = "Set_Console_Properties"
            Me.Text = "Set_Console_Properties"
            Me.ResumeLayout(False)
            Me.PerformLayout()

        End Sub

        Friend WithEvents Panel1 As System.Windows.Forms.Panel
    End Class
End Namespace

