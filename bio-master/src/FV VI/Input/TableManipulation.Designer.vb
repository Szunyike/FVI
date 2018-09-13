<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Class TableManipulation
    Inherits System.Windows.Forms.Form

    'Form overrides dispose to clean up the component list.
    <System.Diagnostics.DebuggerNonUserCode()> _
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
    <System.Diagnostics.DebuggerStepThrough()> _
    Private Sub InitializeComponent()
        Me.Button1 = New System.Windows.Forms.Button()
        Me.Button2 = New System.Windows.Forms.Button()
        Me.Button3 = New System.Windows.Forms.Button()
        Me.Button4 = New System.Windows.Forms.Button()
        Me.dgv1 = New System.Windows.Forms.DataGridView()
        Me.Button5 = New System.Windows.Forms.Button()
        CType(Me.dgv1, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.SuspendLayout()
        '
        'Button1
        '
        Me.Button1.Dock = System.Windows.Forms.DockStyle.Bottom
        Me.Button1.Location = New System.Drawing.Point(0, 376)
        Me.Button1.Name = "Button1"
        Me.Button1.Size = New System.Drawing.Size(373, 23)
        Me.Button1.TabIndex = 0
        Me.Button1.Text = "OK"
        Me.Button1.UseVisualStyleBackColor = True
        '
        'Button2
        '
        Me.Button2.Dock = System.Windows.Forms.DockStyle.Bottom
        Me.Button2.Location = New System.Drawing.Point(0, 353)
        Me.Button2.Name = "Button2"
        Me.Button2.Size = New System.Drawing.Size(373, 23)
        Me.Button2.TabIndex = 1
        Me.Button2.Text = "Cancel"
        Me.Button2.UseVisualStyleBackColor = True
        '
        'Button3
        '
        Me.Button3.Dock = System.Windows.Forms.DockStyle.Bottom
        Me.Button3.Location = New System.Drawing.Point(0, 330)
        Me.Button3.Name = "Button3"
        Me.Button3.Size = New System.Drawing.Size(373, 23)
        Me.Button3.TabIndex = 2
        Me.Button3.Text = "Save"
        Me.Button3.UseVisualStyleBackColor = True
        '
        'Button4
        '
        Me.Button4.Dock = System.Windows.Forms.DockStyle.Bottom
        Me.Button4.Location = New System.Drawing.Point(0, 307)
        Me.Button4.Name = "Button4"
        Me.Button4.Size = New System.Drawing.Size(373, 23)
        Me.Button4.TabIndex = 3
        Me.Button4.Text = "Add Column"
        Me.Button4.UseVisualStyleBackColor = True
        '
        'dgv1
        '
        Me.dgv1.AllowUserToDeleteRows = False
        Me.dgv1.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize
        Me.dgv1.Dock = System.Windows.Forms.DockStyle.Fill
        Me.dgv1.Location = New System.Drawing.Point(0, 0)
        Me.dgv1.Name = "dgv1"
        Me.dgv1.Size = New System.Drawing.Size(373, 307)
        Me.dgv1.TabIndex = 4
        '
        'Button5
        '
        Me.Button5.Dock = System.Windows.Forms.DockStyle.Bottom
        Me.Button5.Location = New System.Drawing.Point(0, 284)
        Me.Button5.Name = "Button5"
        Me.Button5.Size = New System.Drawing.Size(373, 23)
        Me.Button5.TabIndex = 5
        Me.Button5.Text = "Load Saves Settings"
        Me.Button5.UseVisualStyleBackColor = True
        '
        'TableManipulation
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(6.0!, 13.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.ClientSize = New System.Drawing.Size(373, 399)
        Me.Controls.Add(Me.Button5)
        Me.Controls.Add(Me.dgv1)
        Me.Controls.Add(Me.Button4)
        Me.Controls.Add(Me.Button3)
        Me.Controls.Add(Me.Button2)
        Me.Controls.Add(Me.Button1)
        Me.Name = "TableManipulation"
        Me.Text = "TableManipulation"
        CType(Me.dgv1, System.ComponentModel.ISupportInitialize).EndInit()
        Me.ResumeLayout(False)

    End Sub

    Friend WithEvents Button1 As Button
    Friend WithEvents Button2 As Button
    Friend WithEvents Button3 As Button
    Friend WithEvents Button4 As Button
    Friend WithEvents dgv1 As DataGridView
    Friend WithEvents Button5 As Button
End Class
