<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Class PhyloGeneticClorPicker
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
        Me.GroupBox1 = New System.Windows.Forms.GroupBox()
        Me.ListBox1 = New System.Windows.Forms.ListBox()
        Me.GroupBox2 = New System.Windows.Forms.GroupBox()
        Me.Button1 = New System.Windows.Forms.Button()
        Me.ListBox2 = New System.Windows.Forms.ListBox()
        Me.GroupBox3 = New System.Windows.Forms.GroupBox()
        Me.Button4 = New System.Windows.Forms.Button()
        Me.Button2 = New System.Windows.Forms.Button()
        Me.ListBox3 = New System.Windows.Forms.ListBox()
        Me.GroupBox4 = New System.Windows.Forms.GroupBox()
        Me.Button3 = New System.Windows.Forms.Button()
        Me.ListBox4 = New System.Windows.Forms.ListBox()
        Me.ColorDialog1 = New System.Windows.Forms.ColorDialog()
        Me.GroupBox1.SuspendLayout()
        Me.GroupBox2.SuspendLayout()
        Me.GroupBox3.SuspendLayout()
        Me.GroupBox4.SuspendLayout()
        Me.SuspendLayout()
        '
        'GroupBox1
        '
        Me.GroupBox1.Controls.Add(Me.ListBox1)
        Me.GroupBox1.Dock = System.Windows.Forms.DockStyle.Left
        Me.GroupBox1.Location = New System.Drawing.Point(0, 0)
        Me.GroupBox1.Name = "GroupBox1"
        Me.GroupBox1.Size = New System.Drawing.Size(211, 262)
        Me.GroupBox1.TabIndex = 0
        Me.GroupBox1.TabStop = False
        Me.GroupBox1.Text = "Basic"
        '
        'ListBox1
        '
        Me.ListBox1.Dock = System.Windows.Forms.DockStyle.Fill
        Me.ListBox1.FormattingEnabled = True
        Me.ListBox1.Location = New System.Drawing.Point(3, 16)
        Me.ListBox1.Name = "ListBox1"
        Me.ListBox1.Size = New System.Drawing.Size(205, 243)
        Me.ListBox1.TabIndex = 0
        '
        'GroupBox2
        '
        Me.GroupBox2.Controls.Add(Me.Button1)
        Me.GroupBox2.Controls.Add(Me.ListBox2)
        Me.GroupBox2.Dock = System.Windows.Forms.DockStyle.Left
        Me.GroupBox2.Location = New System.Drawing.Point(211, 0)
        Me.GroupBox2.Name = "GroupBox2"
        Me.GroupBox2.Size = New System.Drawing.Size(211, 262)
        Me.GroupBox2.TabIndex = 1
        Me.GroupBox2.TabStop = False
        Me.GroupBox2.Text = "1St"
        '
        'Button1
        '
        Me.Button1.Dock = System.Windows.Forms.DockStyle.Top
        Me.Button1.Location = New System.Drawing.Point(3, 16)
        Me.Button1.Name = "Button1"
        Me.Button1.Size = New System.Drawing.Size(205, 24)
        Me.Button1.TabIndex = 1
        Me.Button1.Text = "Create Color"
        Me.Button1.UseVisualStyleBackColor = True
        '
        'ListBox2
        '
        Me.ListBox2.Dock = System.Windows.Forms.DockStyle.Bottom
        Me.ListBox2.FormattingEnabled = True
        Me.ListBox2.Location = New System.Drawing.Point(3, 47)
        Me.ListBox2.Name = "ListBox2"
        Me.ListBox2.Size = New System.Drawing.Size(205, 212)
        Me.ListBox2.TabIndex = 0
        '
        'GroupBox3
        '
        Me.GroupBox3.Controls.Add(Me.Button4)
        Me.GroupBox3.Controls.Add(Me.Button2)
        Me.GroupBox3.Controls.Add(Me.ListBox3)
        Me.GroupBox3.Dock = System.Windows.Forms.DockStyle.Left
        Me.GroupBox3.Location = New System.Drawing.Point(422, 0)
        Me.GroupBox3.Name = "GroupBox3"
        Me.GroupBox3.Size = New System.Drawing.Size(211, 262)
        Me.GroupBox3.TabIndex = 2
        Me.GroupBox3.TabStop = False
        Me.GroupBox3.Text = "2nd"
        '
        'Button4
        '
        Me.Button4.Dock = System.Windows.Forms.DockStyle.Fill
        Me.Button4.Location = New System.Drawing.Point(102, 16)
        Me.Button4.Name = "Button4"
        Me.Button4.Size = New System.Drawing.Size(106, 31)
        Me.Button4.TabIndex = 2
        Me.Button4.Text = "Delete Item"
        Me.Button4.UseVisualStyleBackColor = True
        '
        'Button2
        '
        Me.Button2.Dock = System.Windows.Forms.DockStyle.Left
        Me.Button2.Location = New System.Drawing.Point(3, 16)
        Me.Button2.Name = "Button2"
        Me.Button2.Size = New System.Drawing.Size(99, 31)
        Me.Button2.TabIndex = 1
        Me.Button2.Text = "Create Schema"
        Me.Button2.UseVisualStyleBackColor = True
        '
        'ListBox3
        '
        Me.ListBox3.Dock = System.Windows.Forms.DockStyle.Bottom
        Me.ListBox3.DrawMode = System.Windows.Forms.DrawMode.OwnerDrawFixed
        Me.ListBox3.FormattingEnabled = True
        Me.ListBox3.Location = New System.Drawing.Point(3, 47)
        Me.ListBox3.Name = "ListBox3"
        Me.ListBox3.Size = New System.Drawing.Size(205, 212)
        Me.ListBox3.TabIndex = 0
        '
        'GroupBox4
        '
        Me.GroupBox4.Controls.Add(Me.Button3)
        Me.GroupBox4.Controls.Add(Me.ListBox4)
        Me.GroupBox4.Dock = System.Windows.Forms.DockStyle.Left
        Me.GroupBox4.Location = New System.Drawing.Point(633, 0)
        Me.GroupBox4.Name = "GroupBox4"
        Me.GroupBox4.Size = New System.Drawing.Size(211, 262)
        Me.GroupBox4.TabIndex = 3
        Me.GroupBox4.TabStop = False
        Me.GroupBox4.Text = "b"
        '
        'Button3
        '
        Me.Button3.Dock = System.Windows.Forms.DockStyle.Top
        Me.Button3.Location = New System.Drawing.Point(3, 16)
        Me.Button3.Name = "Button3"
        Me.Button3.Size = New System.Drawing.Size(205, 24)
        Me.Button3.TabIndex = 1
        Me.Button3.Text = "Save"
        Me.Button3.UseVisualStyleBackColor = True
        '
        'ListBox4
        '
        Me.ListBox4.Dock = System.Windows.Forms.DockStyle.Bottom
        Me.ListBox4.FormattingEnabled = True
        Me.ListBox4.Location = New System.Drawing.Point(3, 47)
        Me.ListBox4.Name = "ListBox4"
        Me.ListBox4.Size = New System.Drawing.Size(205, 212)
        Me.ListBox4.TabIndex = 0
        '
        'PhyloGeneticClorPicker
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(6.0!, 13.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.ClientSize = New System.Drawing.Size(846, 262)
        Me.Controls.Add(Me.GroupBox4)
        Me.Controls.Add(Me.GroupBox3)
        Me.Controls.Add(Me.GroupBox2)
        Me.Controls.Add(Me.GroupBox1)
        Me.Name = "PhyloGeneticClorPicker"
        Me.Text = "h"
        Me.GroupBox1.ResumeLayout(False)
        Me.GroupBox2.ResumeLayout(False)
        Me.GroupBox3.ResumeLayout(False)
        Me.GroupBox4.ResumeLayout(False)
        Me.ResumeLayout(False)

    End Sub

    Friend WithEvents GroupBox1 As System.Windows.Forms.GroupBox
    Friend WithEvents ListBox1 As System.Windows.Forms.ListBox
    Friend WithEvents GroupBox2 As System.Windows.Forms.GroupBox
    Friend WithEvents Button1 As System.Windows.Forms.Button
    Friend WithEvents ListBox2 As System.Windows.Forms.ListBox
    Friend WithEvents GroupBox3 As System.Windows.Forms.GroupBox
    Friend WithEvents Button2 As System.Windows.Forms.Button
    Friend WithEvents ListBox3 As System.Windows.Forms.ListBox
    Friend WithEvents GroupBox4 As System.Windows.Forms.GroupBox
    Friend WithEvents Button3 As System.Windows.Forms.Button
    Friend WithEvents ListBox4 As System.Windows.Forms.ListBox
    Private vals As List(Of String)()



    Friend WithEvents ColorDialog1 As System.Windows.Forms.ColorDialog
    Friend WithEvents Button4 As System.Windows.Forms.Button
End Class
