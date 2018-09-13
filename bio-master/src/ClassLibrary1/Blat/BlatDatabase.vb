Imports System.IO
Imports System.Text

Namespace Szunyi
    Namespace Blat
        Public Class BlatDatabase
            Private Files As New List(Of FileInfo)

            Public Sub New(File As FileInfo)
                Me.Files.Add(File)

            End Sub
            Public Sub DoIt()
                For Each File In Files
                    Dim consoleApp As New Process
                    With consoleApp
                        .EnableRaisingEvents = True
                        .StartInfo.FileName = My.Resources.BlatPath & "faToTwoBit.exe "

                        .StartInfo.RedirectStandardError = True
                        .StartInfo.UseShellExecute = False
                        .StartInfo.CreateNoWindow = True
                        .StartInfo.WindowStyle = ProcessWindowStyle.Hidden
                    End With

                    Dim Arguments As New StringBuilder
                    Arguments.Append(File.FullName).Append(" ")
                    Arguments.Append(My.Resources.BlatDBPath & File.Name.Replace(File.Extension, ".2bit"))
                    consoleApp.StartInfo.Arguments = Arguments.ToString

                    Dim out1 As System.IO.StreamReader

                    Try
                        consoleApp.Start()
                        out1 = consoleApp.StandardError
                        Dim alfr = out1.ReadToEnd
                        If alfr <> "" Then
                            ' MsgBox("Error Creating Database")
                            MsgBox(alfr)
                        Else

                        End If
                    Catch ex As Exception
                        MsgBox(ex.ToString)

                    End Try

                Next
            End Sub

        End Class

    End Namespace
End Namespace
