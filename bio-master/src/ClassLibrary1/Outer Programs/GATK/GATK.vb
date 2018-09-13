Imports System.IO

Namespace Szunyi
    Namespace Outer_Programs
        Namespace Settings
            Public Class HaploTypeCaller_Settings
                Public Property genotyping_mode As String = "DISCOVERY"
                Public Property stand_emit_conf As Integer = 10
                Public Property stand_call_conf As Integer = 30
                Public Sub New()

                End Sub
            End Class
        End Namespace
        Public Class GATK
            Public Shared Function Get_HaploTypeCaller(Ref As FileInfo,
                                                       BamFiles As List(Of FileInfo),
                                                       Optional GATK_Name As String = "$GATK",
                                                       Optional Settings As Settings.HaploTypeCaller_Settings = Nothing) As String
                If IsNothing(Settings) = True Then Settings = New Settings.HaploTypeCaller_Settings
                Dim str As New System.Text.StringBuilder
                For Each File In BamFiles
                    str.Append("java -jar " & GATK_Name)
                    str.Append(" -T HaplotypeCaller")
                    str.Append(" -R " & "/home/szunyi/Documents/ForWork/" & Ref.Name)
                    str.Append(" -I " & "/home/szunyi/Documents/ForWork/" & File.Name)
                    str.Append(" --genotyping_mode ").Append(Settings.genotyping_mode)
                    str.Append(" -stand_emit_conf ").Append(Settings.stand_emit_conf)
                    str.Append(" -stand_call_conf ").Append(Settings.stand_call_conf)
                    str.Append(" -o " & "/home/szunyi/Documents/ForWork/" & File.Name.Replace(File.Extension, ".vcf")).Append(vbCrLf)

                Next
                Return str.ToString
            End Function

        End Class
    End Namespace
End Namespace

