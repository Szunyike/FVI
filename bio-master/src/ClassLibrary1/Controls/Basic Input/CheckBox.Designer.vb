Namespace Controls
    <Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()>
    Partial Class CheckBox
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
            Me.CheckBox1 = New System.Windows.Forms.CheckBox()
            Me.GroupBox1.SuspendLayout()
            Me.SuspendLayout()
            '
            'GroupBox1
            '
            Me.GroupBox1.Controls.Add(Me.CheckBox1)
            Me.GroupBox1.Dock = System.Windows.Forms.DockStyle.Fill
            Me.GroupBox1.Location = New System.Drawing.Point(0, 0)
            Me.GroupBox1.Name = "GroupBox1"
            Me.GroupBox1.Size = New System.Drawing.Size(237, 46)
            Me.GroupBox1.TabIndex = 0
            Me.GroupBox1.TabStop = False
            '
            'CheckBox1
            '
            Me.CheckBox1.AutoSize = True
            Me.CheckBox1.Dock = System.Windows.Forms.DockStyle.Top
            Me.CheckBox1.Location = New System.Drawing.Point(3, 16)
            Me.CheckBox1.Name = "CheckBox1"
            Me.CheckBox1.Size = New System.Drawing.Size(231, 14)
            Me.CheckBox1.TabIndex = 0
            Me.CheckBox1.UseVisualStyleBackColor = True
            '
            'CheckBOx
            '
            Me.AutoScaleDimensions = New System.Drawing.SizeF(6.0!, 13.0!)
            Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
            Me.Controls.Add(Me.GroupBox1)
            Me.Name = "CheckBOx"
            Me.Size = New System.Drawing.Size(237, 46)
            Me.GroupBox1.ResumeLayout(False)
            Me.GroupBox1.PerformLayout()
            Me.ResumeLayout(False)

        End Sub

        Friend WithEvents GroupBox1 As System.Windows.Forms.GroupBox
        Friend WithEvents CheckBox1 As System.Windows.Forms.CheckBox
    End Class
End Namespace