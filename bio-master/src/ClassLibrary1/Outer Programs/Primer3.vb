Imports System.IO

Namespace Szunyi
    Namespace Outer_Programs
        Public Class Primer3
            Public Shared Property Res As New System.Text.StringBuilder
            Public Shared Function Get_Result(Input As String) As String
                Dim consoleApp As New Process
                With consoleApp
                    .EnableRaisingEvents = True
                    .StartInfo.FileName = My.Resources.Other_Progrmas & "primer3_core.exe"

                    .StartInfo.RedirectStandardError = True
                    .StartInfo.RedirectStandardInput = True
                    .StartInfo.RedirectStandardOutput = True

                    .StartInfo.UseShellExecute = False
                    .StartInfo.CreateNoWindow = True
                    .StartInfo.WindowStyle = ProcessWindowStyle.Hidden

                End With
                Try
                    consoleApp.Start()
                    consoleApp.StandardInput.Write(Input)

                    consoleApp.StandardInput.Flush()
                    consoleApp.StandardInput.Close()
                    consoleApp.WaitForExit()

                    Dim err = consoleApp.StandardError.ReadToEnd
                    If err.Length > 0 Then
                        ' MsgBox(err.ToString)
                    End If
                    Dim out2 = consoleApp.StandardOutput.ReadToEnd
                    Return out2

                    consoleApp.Close()

                Catch ex As Exception
                    MsgBox(ex.ToString)
                Finally
                    consoleApp.Close()
                End Try
            End Function

            Friend Shared Function Get_Results(primer3_Scripts As List(Of String)) As String
                Dim consoleApp As New Process
                Dim Out As New List(Of String)
                With consoleApp
                    .EnableRaisingEvents = True
                    .StartInfo.FileName = My.Resources.Other_Progrmas & "primer3_core.exe"
                    .StartInfo.RedirectStandardError = True
                    .StartInfo.RedirectStandardInput = True
                    .StartInfo.RedirectStandardOutput = True
                    .StartInfo.UseShellExecute = False
                    .StartInfo.CreateNoWindow = True
                    .StartInfo.WindowStyle = ProcessWindowStyle.Hidden
                End With
                Try
                    AddHandler consoleApp.OutputDataReceived,
                    AddressOf SortOutputHandler

                    consoleApp.Start()
                    consoleApp.BeginOutputReadLine()
                    For Each s In primer3_Scripts
                        consoleApp.StandardInput.Write(s)
                    Next
                    consoleApp.StandardInput.Close()
                    consoleApp.WaitForExit()
                    Return Res.ToString

                    consoleApp.Close()

                Catch ex As Exception
                    MsgBox(ex.ToString)
                Finally
                    consoleApp.Close()
                End Try
                Return Nothing
            End Function

            Private Shared Sub SortOutputHandler(sender As Object, e As DataReceivedEventArgs)
                If Not String.IsNullOrEmpty(e.Data) Then
                    Res.Append(e.Data).AppendLine()
                End If

            End Sub
        End Class
    End Namespace
End Namespace

