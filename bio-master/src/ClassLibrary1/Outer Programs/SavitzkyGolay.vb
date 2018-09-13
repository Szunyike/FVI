Imports System.IO

Namespace Szunyi
    Namespace Outer_Programs
        Public Class SavitzkyGolay
            Public Shared Function SavitzkyGolay(Window As Integer,
                                       Derivate As Integer,
                                       Values As List(Of Double)) As List(Of Double)
                If Values.Count < 2 * Window Then
                    Window = Values.Count / 2
                ElseIf Window = 0 Then
                    Window = 1
                End If
                Dim Val = Szunyi.Text.General.GetText(Values, vbCrLf) & ChrW(26)

                Dim sr As New MemoryStream

                Dim sw As New StreamWriter(sr)
                Dim consoleApp As New Process
                With consoleApp
                    .EnableRaisingEvents = False

                    .StartInfo.FileName = My.Resources.Other_Progrmas & "sav_gol.exe"
                    .StartInfo.RedirectStandardError = True
                    .StartInfo.RedirectStandardInput = True
                    .StartInfo.UseShellExecute = False
                    .StartInfo.CreateNoWindow = True
                    .StartInfo.RedirectStandardOutput = True
                    .StartInfo.WindowStyle = ProcessWindowStyle.Hidden
                    .StartInfo.Arguments = "-n" & Window & "," & Window & " -D" & Derivate

                End With

                Try
                    consoleApp.Start()
                    consoleApp.StandardInput.AutoFlush = True
                    For Each Item In Values
                        consoleApp.StandardInput.Write(Item & vbLf)
                    Next

                    consoleApp.StandardInput.Flush()
                    consoleApp.StandardInput.Close()
                    consoleApp.WaitForExit()

                    Dim err = consoleApp.StandardError.ReadToEnd
                    If err.Length > 0 Then
                        ' MsgBox(err.ToString)
                    End If
                    Dim out2 = consoleApp.StandardOutput.ReadToEnd
                    Dim s = Split(out2, vbCrLf)
                    Dim d = Szunyi.Text.Lists.Get_Doubles_Empty_strings_Ignored(s)

                    consoleApp.Close()
                    Return d
                Catch ex As Exception
                    MsgBox(ex.ToString)
                Finally
                    consoleApp.Close()
                End Try
                Return New List(Of Double)
            End Function

        End Class
    End Namespace
End Namespace

