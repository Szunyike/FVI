Imports System.IO

Namespace Szunyi
    Namespace Outer_Programs
        Public Class DisEMBL
            Public Shared Function Get_Basic_Values(Seq As Bio.Sequence) As String
                Dim s = Seq.ConvertToString.ToUpper & vbCrLf & ChrW(26)
                Dim Finished As Boolean = False
                Do
                    s = Seq.ConvertToString.ToUpper & vbLf ' ChrW(26)
                    If Finished = True Then Exit Do
                    Dim sr As New MemoryStream
                    Dim sw As New StreamWriter(sr)
                    Dim consoleApp As New Process
                    With consoleApp
                        .EnableRaisingEvents = True

                        .StartInfo.FileName = My.Resources.Other_Progrmas & "disEMBL.exe"
                        .StartInfo.RedirectStandardError = False
                        .StartInfo.RedirectStandardInput = True
                        .StartInfo.UseShellExecute = False
                        .StartInfo.CreateNoWindow = True
                        .StartInfo.RedirectStandardOutput = True
                        .StartInfo.WindowStyle = ProcessWindowStyle.Hidden

                    End With

                    Try
                        consoleApp.Start()
                        consoleApp.StandardInput.AutoFlush = True
                        consoleApp.StandardInput.Write(s)
                        consoleApp.StandardInput.Flush()
                        consoleApp.StandardInput.Close()

                        consoleApp.WaitForExit(3000)
                        If consoleApp.HasExited = True Then
                            Dim out2 = consoleApp.StandardOutput.ReadToEnd
                            consoleApp.Close()
                            Return out2
                        Else
                            consoleApp.StandardInput.Write(vbLf)
                            consoleApp.WaitForExit(3000)
                            consoleApp.Close() ' Beakad's
                        End If


                    Catch ex As Exception
                        MsgBox(ex.ToString)

                    End Try


                Loop

                Return String.Empty
            End Function
        End Class
    End Namespace
End Namespace


