<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()>	_
Partial Class SelectFeaturesAndQulifiers
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
		Me.SplitContainer1 = New System.Windows.Forms.SplitContainer()
		Me.SplitContainer2 = New System.Windows.Forms.SplitContainer()
		Me.ListBox2 = New System.Windows.Forms.ListBox()
		Me.Button2 = New System.Windows.Forms.Button()
		Me.Panel1 = New System.Windows.Forms.Panel()
		Me.Perfect = New System.Windows.Forms.CheckBox()
		Me.CheckBox1 = New System.Windows.Forms.CheckBox()
		Me.TextBox1 = New System.Windows.Forms.TextBox()
		Me.Button1 = New System.Windows.Forms.Button()
		CType(Me.SplitContainer1, System.ComponentModel.ISupportInitialize).BeginInit()
		Me.SplitContainer1.Panel1.SuspendLayout()
		Me.SplitContainer1.Panel2.SuspendLayout()
		Me.SplitContainer1.SuspendLayout()
		CType(Me.SplitContainer2, System.ComponentModel.ISupportInitialize).BeginInit()
		Me.SplitContainer2.Panel1.SuspendLayout()
		Me.SplitContainer2.Panel2.SuspendLayout()
		Me.SplitContainer2.SuspendLayout()
		Me.Panel1.SuspendLayout()
		Me.SuspendLayout()
		'
		'ListBox1
		'
		Me.ListBox1.Dock = System.Windows.Forms.DockStyle.Fill
		Me.ListBox1.FormattingEnabled = True
		Me.ListBox1.Location = New System.Drawing.Point(0, 0)
		Me.ListBox1.Name = "ListBox1"
		Me.ListBox1.Size = New System.Drawing.Size(264, 202)
		Me.ListBox1.TabIndex = 0
		'
		'SplitContainer1
		'
		Me.SplitContainer1.Dock = System.Windows.Forms.DockStyle.Fill
		Me.SplitContainer1.Location = New System.Drawing.Point(0, 0)
		Me.SplitContainer1.Name = "SplitContainer1"
		Me.SplitContainer1.Orientation = System.Windows.Forms.Orientation.Horizontal
		'
		'SplitContainer1.Panel1
		'
		Me.SplitContainer1.Panel1.Controls.Add(Me.SplitContainer2)
		'
		'SplitContainer1.Panel2
		'
		Me.SplitContainer1.Panel2.Controls.Add(Me.Button2)
		Me.SplitContainer1.Panel2.Controls.Add(Me.Panel1)
		Me.SplitContainer1.Panel2.Controls.Add(Me.TextBox1)
		Me.SplitContainer1.Panel2.Controls.Add(Me.Button1)
		Me.SplitContainer1.Size = New System.Drawing.Size(554, 261)
		Me.SplitContainer1.SplitterDistance = 202
		Me.SplitContainer1.TabIndex = 1
		'
		'SplitContainer2
		'
		Me.SplitContainer2.Dock = System.Windows.Forms.DockStyle.Fill
		Me.SplitContainer2.Location = New System.Drawing.Point(0, 0)
		Me.SplitContainer2.Name = "SplitContainer2"
		'
		'SplitContainer2.Panel1
		'
		Me.SplitContainer2.Panel1.Controls.Add(Me.ListBox1)
		'
		'SplitContainer2.Panel2
		'
		Me.SplitContainer2.Panel2.Controls.Add(Me.ListBox2)
		Me.SplitContainer2.Size = New System.Drawing.Size(554, 202)
		Me.SplitContainer2.SplitterDistance = 264
		Me.SplitContainer2.TabIndex = 0
		'
		'ListBox2
		'
		Me.ListBox2.Dock = System.Windows.Forms.DockStyle.Fill
		Me.ListBox2.FormattingEnabled = True
		Me.ListBox2.Location = New System.Drawing.Point(0, 0)
		Me.ListBox2.Name = "ListBox2"
		Me.ListBox2.Size = New System.Drawing.Size(286, 202)
		Me.ListBox2.TabIndex = 1
		'
		'Button2
		'
		Me.Button2.Dock = System.Windows.Forms.DockStyle.Fill
		Me.Button2.Location = New System.Drawing.Point(553, 0)
		Me.Button2.Name = "Button2"
		Me.Button2.Size = New System.Drawing.Size(1, 55)
		Me.Button2.TabIndex = 5
		Me.Button2.Text = "Cancel"
		Me.Button2.UseVisualStyleBackColor = True
		'
		'Panel1
		'
		Me.Panel1.Controls.Add(Me.Perfect)
		Me.Panel1.Controls.Add(Me.CheckBox1)
		Me.Panel1.Dock = System.Windows.Forms.DockStyle.Left
		Me.Panel1.Location = New System.Drawing.Point(440, 0)
		Me.Panel1.Name = "Panel1"
		Me.Panel1.Size = New System.Drawing.Size(113, 55)
		Me.Panel1.TabIndex = 4
		'
		'Perfect
		'
		Me.Perfect.AutoSize = True
		Me.Perfect.Location = New System.Drawing.Point(3, 34)
		Me.Perfect.Name = "Perfect"
		Me.Perfect.Size = New System.Drawing.Size(99, 17)
		Me.Perfect.TabIndex = 5
		Me.Perfect.Text = "Perfect Match?"
		Me.Perfect.UseVisualStyleBackColor = True
		'
		'CheckBox1
		'
		Me.CheckBox1.AutoSize = True
		Me.CheckBox1.Location = New System.Drawing.Point(3, 3)
		Me.CheckBox1.Name = "CheckBox1"
		Me.CheckBox1.Size = New System.Drawing.Size(103, 17)
		Me.CheckBox1.TabIndex = 4
		Me.CheckBox1.Text = "Search in dbxref"
		Me.CheckBox1.UseVisualStyleBackColor = True
		'
		'TextBox1
		'
		Me.TextBox1.Dock = System.Windows.Forms.DockStyle.Left
		Me.TextBox1.Location = New System.Drawing.Point(97, 0)
		Me.TextBox1.Multiline = True
		Me.TextBox1.Name = "TextBox1"
		Me.TextBox1.Size = New System.Drawing.Size(343, 55)
		Me.TextBox1.TabIndex = 2
		'
		'Button1
		'
		Me.Button1.Dock = System.Windows.Forms.DockStyle.Left
		Me.Button1.Location = New System.Drawing.Point(0, 0)
		Me.Button1.Name = "Button1"
		Me.Button1.Size = New System.Drawing.Size(97, 55)
		Me.Button1.TabIndex = 0
		Me.Button1.Text = "OK"
		Me.Button1.UseVisualStyleBackColor = True
		'
		'SelectQulifiers
		'
		Me.AutoScaleDimensions = New System.Drawing.SizeF(6.0!, 13.0!)
		Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
		Me.ClientSize = New System.Drawing.Size(554, 261)
		Me.Controls.Add(Me.SplitContainer1)
		Me.Name = "SelectQulifiers"
		Me.Text = "SelectQulifiers"
		Me.SplitContainer1.Panel1.ResumeLayout(False)
		Me.SplitContainer1.Panel2.ResumeLayout(False)
		Me.SplitContainer1.Panel2.PerformLayout()
		CType(Me.SplitContainer1, System.ComponentModel.ISupportInitialize).EndInit()
		Me.SplitContainer1.ResumeLayout(False)
		Me.SplitContainer2.Panel1.ResumeLayout(False)
		Me.SplitContainer2.Panel2.ResumeLayout(False)
		CType(Me.SplitContainer2, System.ComponentModel.ISupportInitialize).EndInit()
		Me.SplitContainer2.ResumeLayout(False)
		Me.Panel1.ResumeLayout(False)
		Me.Panel1.PerformLayout()
		Me.ResumeLayout(False)

	End Sub

	Friend WithEvents ListBox1 As ListBox
	Friend WithEvents SplitContainer1 As SplitContainer
	Friend WithEvents SplitContainer2 As SplitContainer
	Friend WithEvents ListBox2 As ListBox
	Friend WithEvents Button2 As Button
	Friend WithEvents Panel1 As Panel
	Friend WithEvents Perfect As CheckBox
	Friend WithEvents CheckBox1 As CheckBox
	Friend WithEvents TextBox1 As TextBox
	Friend WithEvents Button1 As Button
End Class
