Imports System.Text

Namespace Services.ScriptBuilder

    Public Class ScriptBuilder

        Public ReadOnly Property Script As StringBuilder

        Sub New(sb As StringBuilder)
            Script = sb
        End Sub

        Sub New(capacity As Integer)
            Script = New StringBuilder(capacity)
        End Sub

        Sub New()
        End Sub

        Public Overrides Function ToString() As String
            Return Script.ToString
        End Function

        ''' <summary>
        ''' Append
        ''' </summary>
        ''' <param name="sb"></param>
        ''' <param name="s"></param>
        ''' <returns></returns>
        Public Shared Operator &(sb As ScriptBuilder, s As String) As ScriptBuilder
            Call sb.Script.Append(s)
            Return sb
        End Operator

        ''' <summary>
        ''' AppendLine
        ''' </summary>
        ''' <param name="sb"></param>
        ''' <param name="s"></param>
        ''' <returns></returns>
        Public Shared Operator +(sb As ScriptBuilder, s As String) As ScriptBuilder
            Call sb.Script.AppendLine(s)
            Return sb
        End Operator

        ''' <summary>
        ''' AppendLine
        ''' </summary>
        ''' <param name="sb"></param>
        ''' <param name="s"></param>
        ''' <returns></returns>
        Public Shared Operator +(sb As ScriptBuilder, s As IRToken) As ScriptBuilder
            Call sb.Script.AppendLine(s.RScript)
            Return sb
        End Operator
    End Class
End Namespace