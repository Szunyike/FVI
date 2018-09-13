<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Class VennTable
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
        Me.dgv1 = New System.Windows.Forms.DataGridView()
        Me.MenuStrip1 = New System.Windows.Forms.MenuStrip()
        Me.ViewToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem()
        Me.ByGeneToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem()
        Me.ByExpToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem()
        Me.SaveToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem()
        Me.ToolStripMenuItem2 = New System.Windows.Forms.ToolStripMenuItem()
        Me.ToolStripMenuItem3 = New System.Windows.Forms.ToolStripMenuItem()
        Me.ToolStripMenuItem4 = New System.Windows.Forms.ToolStripMenuItem()
        Me.ToolStripTextBox1 = New System.Windows.Forms.ToolStripTextBox()
        Me.ResetToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem()
        Me.DistinctsToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem()
        CType(Me.dgv1, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.MenuStrip1.SuspendLayout()
        Me.SuspendLayout()
        '
        'dgv1
        '
        Me.dgv1.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize
        Me.dgv1.Dock = System.Windows.Forms.DockStyle.Fill
        Me.dgv1.Location = New System.Drawing.Point(0, 27)
        Me.dgv1.Name = "dgv1"
        Me.dgv1.Size = New System.Drawing.Size(725, 382)
        Me.dgv1.TabIndex = 0
        '
        'MenuStrip1
        '
        Me.MenuStrip1.Items.AddRange(New System.Windows.Forms.ToolStripItem() {Me.ViewToolStripMenuItem, Me.SaveToolStripMenuItem, Me.ToolStripTextBox1, Me.ResetToolStripMenuItem})
        Me.MenuStrip1.Location = New System.Drawing.Point(0, 0)
        Me.MenuStrip1.Name = "MenuStrip1"
        Me.MenuStrip1.Size = New System.Drawing.Size(725, 27)
        Me.MenuStrip1.TabIndex = 1
        Me.MenuStrip1.Text = "MenuStrip1"
        '
        'ViewToolStripMenuItem
        '
        Me.ViewToolStripMenuItem.DropDownItems.AddRange(New System.Windows.Forms.ToolStripItem() {Me.ByGeneToolStripMenuItem, Me.ByExpToolStripMenuItem})
        Me.ViewToolStripMenuItem.Name = "ViewToolStripMenuItem"
        Me.ViewToolStripMenuItem.Size = New System.Drawing.Size(44, 23)
        Me.ViewToolStripMenuItem.Text = "View"
        '
        'ByGeneToolStripMenuItem
        '
        Me.ByGeneToolStripMenuItem.Name = "ByGeneToolStripMenuItem"
        Me.ByGeneToolStripMenuItem.Size = New System.Drawing.Size(117, 22)
        Me.ByGeneToolStripMenuItem.Text = "By Gene"
        '
        'ByExpToolStripMenuItem
        '
        Me.ByExpToolStripMenuItem.Name = "ByExpToolStripMenuItem"
        Me.ByExpToolStripMenuItem.Size = New System.Drawing.Size(117, 22)
        Me.ByExpToolStripMenuItem.Text = "By Exp"
        '
        'SaveToolStripMenuItem
        '
        Me.SaveToolStripMenuItem.DropDownItems.AddRange(New System.Windows.Forms.ToolStripItem() {Me.ToolStripMenuItem2, Me.ToolStripMenuItem3, Me.ToolStripMenuItem4, Me.DistinctsToolStripMenuItem})
        Me.SaveToolStripMenuItem.Name = "SaveToolStripMenuItem"
        Me.SaveToolStripMenuItem.Size = New System.Drawing.Size(43, 23)
        Me.SaveToolStripMenuItem.Text = "Save"
        '
        'ToolStripMenuItem2
        '
        Me.ToolStripMenuItem2.Name = "ToolStripMenuItem2"
        Me.ToolStripMenuItem2.Size = New System.Drawing.Size(202, 22)
        Me.ToolStripMenuItem2.Text = "One By One"
        '
        'ToolStripMenuItem3
        '
        Me.ToolStripMenuItem3.Name = "ToolStripMenuItem3"
        Me.ToolStripMenuItem3.Size = New System.Drawing.Size(202, 22)
        Me.ToolStripMenuItem3.Text = "Into One All"
        '
        'ToolStripMenuItem4
        '
        Me.ToolStripMenuItem4.Name = "ToolStripMenuItem4"
        Me.ToolStripMenuItem4.Size = New System.Drawing.Size(202, 22)
        Me.ToolStripMenuItem4.Text = "Into One Only Common"
        '
        'ToolStripTextBox1
        '
        Me.ToolStripTextBox1.Name = "ToolStripTextBox1"
        Me.ToolStripTextBox1.Size = New System.Drawing.Size(100, 23)
        '
        'ResetToolStripMenuItem
        '
        Me.ResetToolStripMenuItem.Name = "ResetToolStripMenuItem"
        Me.ResetToolStripMenuItem.Size = New System.Drawing.Size(47, 23)
        Me.ResetToolStripMenuItem.Text = "Reset"
        '
        'DistinctsToolStripMenuItem
        '
        Me.DistinctsToolStripMenuItem.Name = "DistinctsToolStripMenuItem"
        Me.DistinctsToolStripMenuItem.Size = New System.Drawing.Size(202, 22)
        Me.DistinctsToolStripMenuItem.Text = "Distincts"
        '
        'VennTable
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(6.0!, 13.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.ClientSize = New System.Drawing.Size(725, 409)
        Me.Controls.Add(Me.dgv1)
        Me.Controls.Add(Me.MenuStrip1)
        Me.MainMenuStrip = Me.MenuStrip1
        Me.Name = "VennTable"
        Me.Text = "VennTable"
        CType(Me.dgv1, System.ComponentModel.ISupportInitialize).EndInit()
        Me.MenuStrip1.ResumeLayout(False)
        Me.MenuStrip1.PerformLayout()
        Me.ResumeLayout(False)
        Me.PerformLayout()

    End Sub

    Friend WithEvents dgv1 As DataGridView
    Friend WithEvents MenuStrip1 As MenuStrip
    Friend WithEvents ViewToolStripMenuItem As ToolStripMenuItem
    Friend WithEvents ByGeneToolStripMenuItem As ToolStripMenuItem
    Friend WithEvents ByExpToolStripMenuItem As ToolStripMenuItem
    Friend WithEvents SaveToolStripMenuItem As ToolStripMenuItem
    Friend WithEvents ToolStripMenuItem2 As ToolStripMenuItem
    Friend WithEvents ToolStripMenuItem3 As ToolStripMenuItem
    Friend WithEvents ToolStripMenuItem4 As ToolStripMenuItem
    Friend WithEvents ToolStripTextBox1 As ToolStripTextBox
    Friend WithEvents ResetToolStripMenuItem As ToolStripMenuItem
    Friend WithEvents DistinctsToolStripMenuItem As ToolStripMenuItem
End Class
