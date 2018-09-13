Namespace InputForms

    <Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()>
    Partial Class SettingOfTranscriptPromoterUTR
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
            Me.GroupBox1 = New System.Windows.Forms.GroupBox()
            Me.Label1 = New System.Windows.Forms.Label()
            Me.ExtraPromoterLengthBeforeGene = New System.Windows.Forms.TextBox()
            Me.PromoterToCDSStart = New System.Windows.Forms.CheckBox()
            Me.ConstantPromoterLength = New System.Windows.Forms.TextBox()
            Me.ConstantPromoterBeforeCDS = New System.Windows.Forms.CheckBox()
            Me.PromoterNeeded = New System.Windows.Forms.CheckBox()
            Me.GroupBox3 = New System.Windows.Forms.GroupBox()
            Me.Label2 = New System.Windows.Forms.Label()
            Me.ExtraUTRLengthAfterGene = New System.Windows.Forms.TextBox()
            Me.UTRToEndOfGene = New System.Windows.Forms.CheckBox()
            Me.ConstantUTRLength = New System.Windows.Forms.TextBox()
            Me.ConstantUTR = New System.Windows.Forms.CheckBox()
            Me.UTRNeeded = New System.Windows.Forms.CheckBox()
            Me.Button1 = New System.Windows.Forms.Button()
            Me.Button2 = New System.Windows.Forms.Button()
            Me.GroupBox1.SuspendLayout()
            Me.GroupBox3.SuspendLayout()
            Me.SuspendLayout()
            '
            'GroupBox1
            '
            Me.GroupBox1.Controls.Add(Me.Label1)
            Me.GroupBox1.Controls.Add(Me.ExtraPromoterLengthBeforeGene)
            Me.GroupBox1.Controls.Add(Me.PromoterToCDSStart)
            Me.GroupBox1.Controls.Add(Me.ConstantPromoterLength)
            Me.GroupBox1.Controls.Add(Me.ConstantPromoterBeforeCDS)
            Me.GroupBox1.Controls.Add(Me.PromoterNeeded)
            Me.GroupBox1.Location = New System.Drawing.Point(0, 12)
            Me.GroupBox1.Name = "GroupBox1"
            Me.GroupBox1.Size = New System.Drawing.Size(318, 126)
            Me.GroupBox1.TabIndex = 0
            Me.GroupBox1.TabStop = False
            Me.GroupBox1.Text = "Promoter"
            '
            'Label1
            '
            Me.Label1.AutoSize = True
            Me.Label1.Location = New System.Drawing.Point(220, 78)
            Me.Label1.Name = "Label1"
            Me.Label1.Size = New System.Drawing.Size(47, 13)
            Me.Label1.TabIndex = 13
            Me.Label1.Text = "Extra Bp"
            '
            'ExtraPromoterLengthBeforeGene
            '
            Me.ExtraPromoterLengthBeforeGene.Location = New System.Drawing.Point(114, 71)
            Me.ExtraPromoterLengthBeforeGene.Name = "ExtraPromoterLengthBeforeGene"
            Me.ExtraPromoterLengthBeforeGene.Size = New System.Drawing.Size(100, 20)
            Me.ExtraPromoterLengthBeforeGene.TabIndex = 5
            Me.ExtraPromoterLengthBeforeGene.Text = "0"
            '
            'PromoterToCDSStart
            '
            Me.PromoterToCDSStart.AutoSize = True
            Me.PromoterToCDSStart.Checked = True
            Me.PromoterToCDSStart.CheckState = System.Windows.Forms.CheckState.Checked
            Me.PromoterToCDSStart.Location = New System.Drawing.Point(13, 66)
            Me.PromoterToCDSStart.Name = "PromoterToCDSStart"
            Me.PromoterToCDSStart.Size = New System.Drawing.Size(95, 17)
            Me.PromoterToCDSStart.TabIndex = 3
            Me.PromoterToCDSStart.Text = "Gene to CDS?"
            Me.PromoterToCDSStart.UseVisualStyleBackColor = True
            '
            'ConstantPromoterLength
            '
            Me.ConstantPromoterLength.Location = New System.Drawing.Point(114, 45)
            Me.ConstantPromoterLength.Name = "ConstantPromoterLength"
            Me.ConstantPromoterLength.Size = New System.Drawing.Size(100, 20)
            Me.ConstantPromoterLength.TabIndex = 2
            Me.ConstantPromoterLength.Text = "0"
            '
            'ConstantPromoterBeforeCDS
            '
            Me.ConstantPromoterBeforeCDS.AutoSize = True
            Me.ConstantPromoterBeforeCDS.Location = New System.Drawing.Point(13, 43)
            Me.ConstantPromoterBeforeCDS.Name = "ConstantPromoterBeforeCDS"
            Me.ConstantPromoterBeforeCDS.Size = New System.Drawing.Size(74, 17)
            Me.ConstantPromoterBeforeCDS.TabIndex = 1
            Me.ConstantPromoterBeforeCDS.Text = "Constant?"
            Me.ConstantPromoterBeforeCDS.UseVisualStyleBackColor = True
            '
            'PromoterNeeded
            '
            Me.PromoterNeeded.AutoSize = True
            Me.PromoterNeeded.Location = New System.Drawing.Point(13, 20)
            Me.PromoterNeeded.Name = "PromoterNeeded"
            Me.PromoterNeeded.Size = New System.Drawing.Size(70, 17)
            Me.PromoterNeeded.TabIndex = 0
            Me.PromoterNeeded.Text = "Needed?"
            Me.PromoterNeeded.UseVisualStyleBackColor = True
            '
            'GroupBox3
            '
            Me.GroupBox3.Controls.Add(Me.Label2)
            Me.GroupBox3.Controls.Add(Me.ExtraUTRLengthAfterGene)
            Me.GroupBox3.Controls.Add(Me.UTRToEndOfGene)
            Me.GroupBox3.Controls.Add(Me.ConstantUTRLength)
            Me.GroupBox3.Controls.Add(Me.ConstantUTR)
            Me.GroupBox3.Controls.Add(Me.UTRNeeded)
            Me.GroupBox3.Location = New System.Drawing.Point(0, 144)
            Me.GroupBox3.Name = "GroupBox3"
            Me.GroupBox3.Size = New System.Drawing.Size(318, 126)
            Me.GroupBox3.TabIndex = 7
            Me.GroupBox3.TabStop = False
            Me.GroupBox3.Text = "3' UTR"
            '
            'Label2
            '
            Me.Label2.AutoSize = True
            Me.Label2.Location = New System.Drawing.Point(220, 67)
            Me.Label2.Name = "Label2"
            Me.Label2.Size = New System.Drawing.Size(47, 13)
            Me.Label2.TabIndex = 14
            Me.Label2.Text = "Extra Bp"
            '
            'ExtraUTRLengthAfterGene
            '
            Me.ExtraUTRLengthAfterGene.Location = New System.Drawing.Point(114, 64)
            Me.ExtraUTRLengthAfterGene.Name = "ExtraUTRLengthAfterGene"
            Me.ExtraUTRLengthAfterGene.Size = New System.Drawing.Size(100, 20)
            Me.ExtraUTRLengthAfterGene.TabIndex = 5
            Me.ExtraUTRLengthAfterGene.Text = "0"
            '
            'UTRToEndOfGene
            '
            Me.UTRToEndOfGene.AutoSize = True
            Me.UTRToEndOfGene.Checked = True
            Me.UTRToEndOfGene.CheckState = System.Windows.Forms.CheckState.Checked
            Me.UTRToEndOfGene.Location = New System.Drawing.Point(13, 66)
            Me.UTRToEndOfGene.Name = "UTRToEndOfGene"
            Me.UTRToEndOfGene.Size = New System.Drawing.Size(95, 17)
            Me.UTRToEndOfGene.TabIndex = 3
            Me.UTRToEndOfGene.Text = "CDS to Gene?"
            Me.UTRToEndOfGene.UseVisualStyleBackColor = True
            '
            'ConstantUTRLength
            '
            Me.ConstantUTRLength.Location = New System.Drawing.Point(114, 43)
            Me.ConstantUTRLength.Name = "ConstantUTRLength"
            Me.ConstantUTRLength.Size = New System.Drawing.Size(100, 20)
            Me.ConstantUTRLength.TabIndex = 2
            Me.ConstantUTRLength.Text = "0"
            '
            'ConstantUTR
            '
            Me.ConstantUTR.AutoSize = True
            Me.ConstantUTR.Location = New System.Drawing.Point(13, 43)
            Me.ConstantUTR.Name = "ConstantUTR"
            Me.ConstantUTR.Size = New System.Drawing.Size(74, 17)
            Me.ConstantUTR.TabIndex = 1
            Me.ConstantUTR.Text = "Constant?"
            Me.ConstantUTR.UseVisualStyleBackColor = True
            '
            'UTRNeeded
            '
            Me.UTRNeeded.AutoSize = True
            Me.UTRNeeded.Location = New System.Drawing.Point(13, 20)
            Me.UTRNeeded.Name = "UTRNeeded"
            Me.UTRNeeded.Size = New System.Drawing.Size(70, 17)
            Me.UTRNeeded.TabIndex = 0
            Me.UTRNeeded.Text = "Needed?"
            Me.UTRNeeded.UseVisualStyleBackColor = True
            '
            'Button1
            '
            Me.Button1.Location = New System.Drawing.Point(0, 277)
            Me.Button1.Name = "Button1"
            Me.Button1.Size = New System.Drawing.Size(123, 40)
            Me.Button1.TabIndex = 13
            Me.Button1.Text = "OK"
            Me.Button1.UseVisualStyleBackColor = True
            '
            'Button2
            '
            Me.Button2.Location = New System.Drawing.Point(129, 276)
            Me.Button2.Name = "Button2"
            Me.Button2.Size = New System.Drawing.Size(123, 40)
            Me.Button2.TabIndex = 14
            Me.Button2.Text = "Cancel"
            Me.Button2.UseVisualStyleBackColor = True
            '
            'SettingOfTranscriptPromoterUTR
            '
            Me.AutoScaleDimensions = New System.Drawing.SizeF(6.0!, 13.0!)
            Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
            Me.ClientSize = New System.Drawing.Size(487, 358)
            Me.Controls.Add(Me.Button2)
            Me.Controls.Add(Me.Button1)
            Me.Controls.Add(Me.GroupBox3)
            Me.Controls.Add(Me.GroupBox1)
            Me.Name = "SettingOfTranscriptPromoterUTR"
            Me.Text = "SettingOfTranscriptPromoterUTR"
            Me.GroupBox1.ResumeLayout(False)
            Me.GroupBox1.PerformLayout()
            Me.GroupBox3.ResumeLayout(False)
            Me.GroupBox3.PerformLayout()
            Me.ResumeLayout(False)

        End Sub

        Friend WithEvents GroupBox1 As GroupBox
        Friend WithEvents Label1 As Label
        Friend WithEvents ExtraPromoterLengthBeforeGene As TextBox
        Friend WithEvents PromoterToCDSStart As CheckBox
        Friend WithEvents ConstantPromoterLength As TextBox
        Friend WithEvents ConstantPromoterBeforeCDS As CheckBox
        Friend WithEvents PromoterNeeded As CheckBox
        Friend WithEvents GroupBox3 As GroupBox
        Friend WithEvents Label2 As Label
        Friend WithEvents ExtraUTRLengthAfterGene As TextBox
        Friend WithEvents UTRToEndOfGene As CheckBox
        Friend WithEvents ConstantUTRLength As TextBox
        Friend WithEvents ConstantUTR As CheckBox
        Friend WithEvents UTRNeeded As CheckBox
        Friend WithEvents Button1 As Button
        Friend WithEvents Button2 As Button
    End Class
End Namespace
