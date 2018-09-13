<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()>	_
Partial Class Select_Columns
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
		Me.ListBox1 = New System.Windows.Forms.ListBox()
		Me.Button2 = New System.Windows.Forms.Button()
		Me.Button1 = New System.Windows.Forms.Button()
		Me.SuspendLayout()
		'
		'ListBox1
		'
		Me.ListBox1.Dock = System.Windows.Forms.DockStyle.Fill
		Me.ListBox1.FormattingEnabled = True
		Me.ListBox1.Location = New System.Drawing.Point(75, 0)
		Me.ListBox1.Name = "ListBox1"
		Me.ListBox1.SelectionMode = System.Windows.Forms.SelectionMode.MultiExtended
		Me.ListBox1.Size = New System.Drawing.Size(311, 261)
		Me.ListBox1.TabIndex = 5
		'
		'Button2
		'
		Me.Button2.Dock = System.Windows.Forms.DockStyle.Right
		Me.Button2.Location = New System.Drawing.Point(386, 0)
		Me.Button2.Name = "Button2"
		Me.Button2.Size = New System.Drawing.Size(75, 261)
		Me.Button2.TabIndex = 4
		Me.Button2.Text = "Cancel"
		Me.Button2.UseVisualStyleBackColor = True
		'
		'Button1
		'
		Me.Button1.Dock = System.Windows.Forms.DockStyle.Left
		Me.Button1.Location = New System.Drawing.Point(0, 0)
		Me.Button1.Name = "Button1"
		Me.Button1.Size = New System.Drawing.Size(75, 261)
		Me.Button1.TabIndex = 3
		Me.Button1.Text = "OK"
		Me.Button1.UseVisualStyleBackColor = True
		'
		'Select_Columns
		'
		Me.AutoScaleDimensions = New System.Drawing.SizeF(6.0!, 13.0!)
		Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
		Me.ClientSize = New System.Drawing.Size(461, 261)
		Me.Controls.Add(Me.ListBox1)
		Me.Controls.Add(Me.Button2)
		Me.Controls.Add(Me.Button1)
		Me.Name = "Select_Columns"
		Me.Text = "Select_Columns"
		Me.ResumeLayout(False)

	End Sub

	Friend WithEvents ListBox1 As ListBox
	Friend WithEvents Button2 As Button
	Friend WithEvents Button1 As Button
End Class
