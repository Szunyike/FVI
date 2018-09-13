Imports System.Drawing
Imports System.Windows.Forms
Imports Microsoft.Office.Interop

Namespace Szunyi


    Public Class Excel
        '      Dim app As Microsoft.Office.Interop.Excel.Application
        ''        Dim workbook As Microsoft.Office.Interop.Excel._Workbook
        Dim NofSheet As Integer = 1
        Dim IsActivated As Boolean = False
        Dim UsedNames As New List(Of String)
        Public Sub New()
            ' creating Excel Application
            Try


                '             app = New Microsoft.Office.Interop.Excel.Application()
                ' creating new WorkBook within Excel application
                ' workbook = app.Workbooks.Add(Type.Missing)
                ' creating new Excelsheet in workbook

                ''            app.Visible = True

                Dim Response = MsgBox("Is Excel Working", MsgBoxStyle.YesNo)
                If Response = MsgBoxResult.No Then Exit Sub

                IsActivated = True
            Catch ex As Exception
                MsgBox(ex.ToString)
            End Try
        End Sub
        Public Sub Copy(dgv1 As DataGridView, SheetName As String)
            ' Dim worksheet As Microsoft.Office.Interop.Excel._Worksheet = Nothing
            Dim i As Integer, j As Integer
            ' see the excel sheet behind the program


            Dim WorkSheetName As String = GetWorkSheetName(SheetName)
            If UsedNames.Contains(WorkSheetName) = True Then
                MsgBox("It Has Been Already Added!")
                Exit Sub
            Else

                Try
                    '        workbook.Sheets.Add(, , 1)
                    '       worksheet = workbook.Sheets(1)
                    '      worksheet.Name = WorkSheetName
                    '     UsedNames.Add(WorkSheetName)
                Catch ex As Exception

                End Try

            End If


            Try
                For i = 1 To dgv1.Columns.Count
                    '    worksheet.Cells(1, i) = dgv1.Columns(i - 1).HeaderText
                Next
                ' storing Each row and column value to excel sheet
                For i = 0 To dgv1.Rows.Count - 1
                    For j = 0 To dgv1.Columns.Count - 1
                        '       worksheet.Cells(i + 2, j + 1) = dgv1.Rows(i).Cells(j).Value.ToString().Replace(",", ".")
                        '     Dim style As Microsoft.Office.Interop.Excel.Style =
                        '      worksheet.Application.ActiveWorkbook.Styles.Add("NewStyle" & NofSheet & "_" & i & "_" & j)
                        ' style.Font.Bold = True
                        If IsWhite(dgv1.Rows(i).Cells(j).Style.BackColor) = False Then
                            '    style.Interior.Color = System.Drawing.ColorTranslator.ToOle(dgv1.Rows(i).Cells(j).Style.BackColor)
                            '   worksheet.Cells(i + 2, j + 1).Style = "NewStyle" & NofSheet & "_" & i & "_" & j
                        End If


                    Next
                Next
            Catch ex As Exception
                MsgBox(ex.ToString)
            End Try
            NofSheet += 1
        End Sub

        Private Function IsWhite(color As Color) As Boolean
            If color.A = 0 AndAlso color.B = 0 AndAlso color.G = 0 AndAlso color.R = 0 Then Return True
            Return False
        End Function

        Private Function GetWorkSheetName(SheetName As String) As String
            SheetName = SheetName.Replace("\", "").Replace("/", "").Replace("?", "").Replace("*", "").Replace("[", "").Replace("]", "").Replace(":", "").Replace("_", " ")
            If SheetName.Length > 31 Then
                SheetName = SheetName.Substring(0, 31)
            End If
            Return SheetName
        End Function

    End Class

End Namespace
