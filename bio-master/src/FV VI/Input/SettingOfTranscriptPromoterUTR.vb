Imports System.ComponentModel
Imports Bio
Imports Bio.IO.GenBank

Namespace InputForms
    Public Class SettingOfTranscriptPromoterUTR
        Public WithEvents Setting As New ClassLibrary1.Szunyi.mRNA.Transcript.TranscriptManipulationSettings
        Public Sub New()

            ' This call is required by the designer.
            InitializeComponent()

            ' Add any initialization after the InitializeComponent() call.

        End Sub
        Private Sub SettingOfTranscriptPromoterUTR_Load(sender As Object, e As EventArgs) Handles MyBase.Load
            Try
                Me.ConstantPromoterLength.DataBindings.Add(New Binding("Text", Setting, "ConstantPromoterBeforeCDSLength"))
                Me.ConstantPromoterBeforeCDS.DataBindings.Add(New Binding("Checked", Setting, "ConstantPromoterBeforeCDS"))
                Me.PromoterNeeded.DataBindings.Add(New Binding("Checked", Setting, "PromoterNeeded"))
                Me.PromoterToCDSStart.DataBindings.Add(New Binding("Checked", Setting, "PromoterToCDSStart"))
                Me.ExtraPromoterLengthBeforeGene.DataBindings.Add(New Binding("Text", Setting, "ExtraPromoterLengthBeforeGene"))


                Me.ConstantUTRLength.DataBindings.Add(New Binding("Text", Setting, "ConstantUTRAfterCDSLength"))
                Me.ConstantUTR.DataBindings.Add(New Binding("Checked", Setting, "ConstantUTRAfterCDS"))
                Me.UTRNeeded.DataBindings.Add(New Binding("Checked", Setting, "UTRNeeded"))
                Me.UTRToEndOfGene.DataBindings.Add(New Binding("Checked", Setting, "UTRToEndOfGene"))
                Me.ExtraUTRLengthAfterGene.DataBindings.Add(New Binding("Text", Setting, "ExtraUTRLengthAfterGene"))

            Catch ex As Exception
                Dim alf As Int16 = 54
            End Try

        End Sub

        Private Sub Button1_Click(sender As Object, e As EventArgs) Handles Button1.Click 'OK
            Me.DialogResult = DialogResult.OK
        End Sub

        Private Sub Button2_Click(sender As Object, e As EventArgs) Handles Button2.Click ' Cancel
            Me.DialogResult = DialogResult.Cancel

        End Sub

#Region "Validating"
        Private Sub TextBox1_Validating(sender As Object, e As CancelEventArgs) Handles ConstantPromoterLength.Validating
            Try
                Dim i1 As Integer = ConstantPromoterLength.Text
            Catch ex As Exception
                MsgBox("Enter a valid integer")
                e.Cancel = True
            End Try
        End Sub
        Private Sub TextBox2_Validating(sender As Object, e As CancelEventArgs) Handles ExtraPromoterLengthBeforeGene.Validating
            Try
                Dim i1 As Integer = ExtraPromoterLengthBeforeGene.Text
            Catch ex As Exception
                MsgBox("Enter a valid integer")
                e.Cancel = True
            End Try
        End Sub

        Private Sub TextBox6_Validating(sender As Object, e As CancelEventArgs) Handles ConstantUTRLength.Validating
            Try
                Dim i1 As Integer = ConstantUTRLength.Text
            Catch ex As Exception
                MsgBox("Enter a valid integer")
                e.Cancel = True
            End Try
        End Sub




#End Region

    End Class
End Namespace
