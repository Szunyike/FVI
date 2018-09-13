Imports ClassLibrary1

<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()>
Partial Class TRYIII
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
        Dim resources As System.ComponentModel.ComponentResourceManager = New System.ComponentModel.ComponentResourceManager(GetType(TRYIII))
        Me.StringSelectionCheckbox1 = New CheckBoxForStringsFull()
        Me.StringSelectionCheckbox2 = New CheckBoxForStringsFull()
        Me.Button1 = New System.Windows.Forms.Button()
        Me.Button2 = New System.Windows.Forms.Button()
        Me.FLP = New System.Windows.Forms.FlowLayoutPanel()
        Me.SuspendLayout()
        '
        'StringSelectionCheckbox1
        '
        Me.StringSelectionCheckbox1.Dock = System.Windows.Forms.DockStyle.Left
        Me.StringSelectionCheckbox1.Location = New System.Drawing.Point(0, 0)
        Me.StringSelectionCheckbox1.Name = "StringSelectionCheckbox1"
        Me.StringSelectionCheckbox1.SelectedFiles = CType(resources.GetObject("StringSelectionCheckbox1.SelectedFiles"), System.Collections.Generic.List(Of System.IO.FileInfo))
        Me.StringSelectionCheckbox1.SelectedObj = CType(resources.GetObject("StringSelectionCheckbox1.SelectedObj"), System.Collections.Generic.List(Of Object))
        Me.StringSelectionCheckbox1.SelectedStrings = CType(resources.GetObject("StringSelectionCheckbox1.SelectedStrings"), System.Collections.Generic.List(Of String))
        Me.StringSelectionCheckbox1.Size = New System.Drawing.Size(241, 261)
        Me.StringSelectionCheckbox1.TabIndex = 0
        '
        'StringSelectionCheckbox2
        '
        Me.StringSelectionCheckbox2.Dock = System.Windows.Forms.DockStyle.Left
        Me.StringSelectionCheckbox2.Location = New System.Drawing.Point(241, 0)
        Me.StringSelectionCheckbox2.Name = "StringSelectionCheckbox2"
        Me.StringSelectionCheckbox2.Size = New System.Drawing.Size(241, 261)
        Me.StringSelectionCheckbox2.TabIndex = 1
        '
        'Button1
        '
        Me.Button1.Dock = System.Windows.Forms.DockStyle.Bottom
        Me.Button1.Location = New System.Drawing.Point(482, 231)
        Me.Button1.Name = "Button1"
        Me.Button1.Size = New System.Drawing.Size(524, 30)
        Me.Button1.TabIndex = 2
        Me.Button1.Text = "Ok"
        Me.Button1.UseVisualStyleBackColor = True
        '
        'Button2
        '
        Me.Button2.Dock = System.Windows.Forms.DockStyle.Bottom
        Me.Button2.Location = New System.Drawing.Point(482, 201)
        Me.Button2.Name = "Button2"
        Me.Button2.Size = New System.Drawing.Size(524, 30)
        Me.Button2.TabIndex = 3
        Me.Button2.Text = "Cancel"
        Me.Button2.UseVisualStyleBackColor = True
        '
        'FLP
        '
        Me.FLP.Dock = System.Windows.Forms.DockStyle.Fill
        Me.FLP.Location = New System.Drawing.Point(482, 0)
        Me.FLP.Name = "FLP"
        Me.FLP.Size = New System.Drawing.Size(524, 201)
        Me.FLP.TabIndex = 4
        '
        'TRYIII
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(6.0!, 13.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.ClientSize = New System.Drawing.Size(1006, 261)
        Me.Controls.Add(Me.FLP)
        Me.Controls.Add(Me.Button2)
        Me.Controls.Add(Me.Button1)
        Me.Controls.Add(StringSelectionCheckbox2)
        Me.Controls.Add(StringSelectionCheckbox1)
        Me.Name = "TRYIII"
        Me.Text = "TRYIII"
        Me.ResumeLayout(False)

    End Sub

    Friend WithEvents StringSelectionCheckbox1 As CheckBoxForStringsFull
    Friend WithEvents StringSelectionCheckbox2 As CheckBoxForStringsFull
    Friend WithEvents Button1 As Button
    Friend WithEvents Button2 As Button
    Friend WithEvents FLP As FlowLayoutPanel
End Class
