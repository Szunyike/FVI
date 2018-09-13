Imports System.IO
Imports System.Text

Namespace Szunyi

	Public Class Text
		Public Class ReplaceStringsInFiles
			Dim FilesToReplace As List(Of FileInfo)
			Dim FileContaingReplacing As FileInfo
			Dim HeadersOfContainingReplacing As List(Of String)
			Dim ColumnOfOriginalString As Integer, ColumnOfNewString As Integer
			Dim IntrestingColumns As New List(Of Integer)
			Public ReadOnly Property Type = MyConstants.ReplaceStringsInFiles
			Public Sub New()
				' Get Tab File Which Contains Replacing motifs
				' Set To Search Column, and the Replace motif column
				' Get Tab file
				' Set column to Replace
				' Replace all the motifs
				' Save it
				SetOriginalReplacing()
				SetFilesToReplace()

			End Sub
			Private Sub SetOriginalReplacing()
				FileContaingReplacing = Szunyi.IO.Files.SelectFile("Select File Which Contains Original Strings, and New Strings")
				HeadersOfContainingReplacing = Szunyi.IO.Files.GetHeader(FileContaingReplacing, 1, Nothing, Nothing)

				Dim IntrestingColumns As New List(Of Integer)

				Using x As New Select_Columns(HeadersOfContainingReplacing)
					x.Text = "Select Original String Column"
					If x.ShowDialog = System.Windows.Forms.DialogResult.OK Then
						ColumnOfOriginalString = x.SelectedIndexes.First
					Else : Exit Sub
					End If
				End Using

				Using x As New Select_Columns(HeadersOfContainingReplacing)
					x.Text = "Select New String Column"
					If x.ShowDialog = System.Windows.Forms.DialogResult.OK Then
						ColumnOfNewString = x.SelectedIndexes.First
					Else : Exit Sub
					End If
				End Using
			End Sub
			Private Sub SetFilesToReplace()
				Me.FilesToReplace = Szunyi.IO.Files.SelectFiles

				Dim HeadersOfSearch = Szunyi.IO.Files.GetHeader(Me.FilesToReplace.First, 1, Nothing, Nothing)
				Using x As New Select_Columns(HeadersOfSearch)
						x.Text = "Select New String Column"
						If x.ShowDialog = System.Windows.Forms.DialogResult.OK Then
							IntrestingColumns = x.SelectedIndexes
						Else : Exit Sub
						End If
					End Using

			End Sub
			Public Sub DoIt()
				Dim ListOfItems = Szunyi.IO.Import.Text.GetListForReplace(Me.FileContaingReplacing, ColumnOfOriginalString, ColumnOfNewString)
				For Each File In Me.FilesToReplace
					ReplaceStringsInColumns(File, ListOfItems, IntrestingColumns)
				Next
			End Sub
			Public Sub ReplaceStringsInColumns(FileInSearch As FileInfo,
				 listOfItems As List(Of String()), intrestingColumns As List(Of Integer))

				Dim res As New List(Of String)
				Dim Comp As New Szunyi.Comparares.StringComparer
				Using sr As New StreamReader(FileInSearch.FullName)
					Using sw As New StreamWriter(Szunyi.IO.Files.GetNewFileName(FileInSearch, "Replaced"))
						Do
							Dim s1() = Split(sr.ReadLine, vbTab)
							Dim x1(1) As String

							For Each Index In intrestingColumns
								x1(0) = s1(Index)
								Dim x = listOfItems.BinarySearch(x1, Comp)
								If x > -1 Then
									s1(Index) = listOfItems(x).Last
								End If
							Next
							sw.Write(Szunyi.Text.GetText(s1, vbTab))
							sw.WriteLine()
						Loop Until sr.EndOfStream = True

					End Using
				End Using
			End Sub

		End Class

		Friend Shared Function ToRemove(OriginalStrings As List(Of String), StringsToRemove As List(Of String)) As List(Of String)
			For Each OriginalString In OriginalStrings
				For Each StringToRemove In StringsToRemove
					OriginalString = OriginalString.Replace(StringToRemove, "")
				Next
			Next
			Return OriginalStrings
		End Function

		Shared Function GetText(x As String) As String
			Return x
		End Function
		Shared Function GetText(x() As String, Optional Separator As String = vbCrLf) As String
			Dim str As New StringBuilder
			For Each s In x
				str.Append(s).Append(Separator)
			Next
			If str.Length > 0 Then str.Length -= Separator.Length
			Return str.ToString
		End Function

		Shared Function GetText(list As List(Of Long), Optional Separator As String = vbTab) As String
			Dim out As New StringBuilder
			For Each Item In list
				out.Append(Item).Append(Separator)
			Next
			If out.Length > 0 Then out.Length -= Separator.Length
			Return out.ToString
		End Function

		Shared Function GetText(List As List(Of Double), Optional Separator As String = vbTab) As String
			Dim out As New StringBuilder
			For Each Item In List
				out.Append(Item).Append(Separator)
			Next
			If out.Length > 0 Then out.Length -= Separator.Length
			Return out.ToString
		End Function


		Shared Function GetText(x As List(Of String), Optional Separator As String = vbCrLf) As String
			Dim str As New StringBuilder
			For Each s In x
				str.Append(s).Append(Separator)
			Next
			If str.Length >= Separator.Length Then str.Length -= Separator.Length
			Return str.ToString
		End Function
		Shared Function GetText(x As List(Of Integer), Optional Separator As String = vbCrLf) As String
			Dim str As New StringBuilder
			For Each s In x
				str.Append(s).Append(Separator)
			Next
			If str.Length >= Separator.Length Then str.Length -= Separator.Length
			Return str.ToString
		End Function

		Friend Shared Function GetText(s1() As String, valuesColIndexes As List(Of Integer), Optional separator As String = vbTab) As String
			Dim out As New StringBuilder
			For Each Item In valuesColIndexes
				If Item < s1.Count Then
					out.Append(s1(Item)).Append(separator)
				Else
					out.Append(String.Empty).Append(separator)
				End If

			Next
			out.Length -= separator.Length
			Return out.ToString
		End Function
		Friend Shared Function GetStringFromInputbox(Title As String) As String
			Return InputBox(Title)
		End Function

		Friend Shared Sub ReplaceStringInFile(txtToReplace As String, txtNewString As String, file As FileInfo)
			Using sr As New StreamReader(file.FullName)
				Dim t = file.Name.Replace(file.Extension, "") & "Md" & file.Extension
				Using sw As New StreamWriter(file.DirectoryName & "\" & t)
					Do
						sw.Write(sr.ReadLine.Replace(txtToReplace, txtNewString) & vbCrLf)
					Loop Until sr.EndOfStream = True
				End Using
			End Using

		End Sub
		Friend Shared Sub ReplaceStringInFile(txtToReplace As String, txtNewString As String, files As List(Of FileInfo))
			For Each File In files
				ReplaceStringInFile(txtToReplace, txtNewString, File)
			Next
		End Sub

		Friend Shared Function GetIntegerFromInputbox(Optional txt As String = "Enter an Integer") As Integer
			Dim s = InputBox(txt)
			Try
				Dim t As Integer = s
				Return t
			Catch ex As Exception
				MsgBox(ex.ToString)
				Return Nothing
			End Try
		End Function
	End Class
End Namespace

