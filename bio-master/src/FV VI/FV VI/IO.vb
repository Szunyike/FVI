Imports System.ComponentModel
Imports System.IO
Imports System.Text
Imports Bio

Namespace Szunyi
	Namespace IO
		Public Class Files
			Friend Shared Function SelectFiles(Optional Title As String = "Select Files") As List(Of FileInfo)
				Dim Out As New List(Of FileInfo)
				Dim ofd1 As New OpenFileDialog
				ofd1.Multiselect = True
				If ofd1.ShowDialog = DialogResult.OK Then
					For Each FileName In ofd1.FileNames
						Out.Add(New FileInfo(FileName))
					Next
				End If
				Return Out
			End Function

			Friend Shared Function GetAllFilesFromFolder(folder As String) As List(Of FileInfo)
				Throw New NotImplementedException()
			End Function

			Friend Shared Function SelectFile(Optional Title As String = "Select Files") As FileInfo

				Dim ofd1 As New OpenFileDialog
				ofd1.Multiselect = False
				If ofd1.ShowDialog = DialogResult.OK Then
					Return New FileInfo(ofd1.FileName)
				End If
				Return Nothing
			End Function
			Friend Shared Function GetShortFileName(Files As List(Of FileInfo)) As String
				Select Case Files.Count
					Case 0
						Return String.Empty
					Case 1
						Return Files.First.Name.Replace(Files.First.Extension, "")
					Case Else
						Return InputBox("Enter Name of The List!")
				End Select
			End Function

			Friend Shared Function SelectSaveFile(Optional Title As String = "Save As") As FileInfo
				Dim sfd1 As New SaveFileDialog
				sfd1.Title = Title
				If sfd1.ShowDialog = DialogResult.OK Then
					Return New FileInfo(sfd1.FileName)
				Else
					Return Nothing
				End If
			End Function
			Friend Shared Function GetFolder(Optional Title As String = "Select Folder") As String
				Dim fbd1 As New FolderBrowserDialog
				fbd1.Description = Title
				If fbd1.ShowDialog = DialogResult.OK Then Return fbd1.SelectedPath
				Return ""
			End Function

			Shared Function GetIntrestingColumn(File As FileInfo, NofHeaderLine As Integer, Optional Title As String = "Select Column") As Integer
				Dim Headers = Szunyi.IO.Files.GetHeader(File, NofHeaderLine, Nothing, Nothing)

				Using x As New Select_Columns(Headers)
					x.Text = Title
					If x.ShowDialog = System.Windows.Forms.DialogResult.OK Then
						Return x.SelectedIndexes.First
					Else : Return -1
					End If
				End Using
			End Function
			Public Shared Function GetHeader(TheFile As FileInfo, NofHeaderLine As Integer,
										  ToRemove As List(Of String),
										  InterestingColumnsIDs As List(Of Integer)) As List(Of String)

				Dim res As New List(Of String)
				Dim resII As New Dictionary(Of Integer, String)
				Dim s1() As String
				If IsNothing(TheFile) = True Then Return Nothing
				Using sr As New StreamReader(TheFile.FullName)
					If IsNothing(InterestingColumnsIDs) = True Then
						For i1 = 1 To NofHeaderLine
							s1 = Split(sr.ReadLine, vbTab)
							If res.Count = 0 Then
								res = s1.ToList
							Else
								For i2 = 0 To s1.Count - 1
									res(i2) = res(i2) & " " & s1(i2)
								Next
							End If
						Next
					Else
						For i1 = 1 To NofHeaderLine
							s1 = Split(sr.ReadLine, vbTab)
							For i2 = 0 To s1.Count - 1
								If InterestingColumnsIDs.Contains(i2) = True Then
									If resII.ContainsKey(i2) = False Then
										resII.Add(i2, s1(i2))
									Else
										resII(i2) = resII(i2) & " " & s1(i2)
									End If
								End If
							Next
						Next
						res = resII.Values.ToList
					End If

					If IsNothing(ToRemove) = False Then res = Szunyi.Text.ToRemove(res, ToRemove)

				End Using
				Return res
			End Function
			''' <summary>
			''' Replece the Extension
			''' </summary>
			''' <param name="fileInSearch"></param>
			''' <param name="v"></param>
			''' <returns></returns>
			Friend Shared Function GetNewFileName(fileInSearch As FileInfo, v As String) As String
				Dim Out As String = fileInSearch.FullName.Replace(fileInSearch.Extension, "")
				Out = Out & v & fileInSearch.Extension
				Return Out

			End Function
			Friend Shared Function GetNewFile(fileInSearch As FileInfo, v As String) As FileInfo
				Dim Out As String = fileInSearch.FullName.Replace(fileInSearch.Extension, v)
				'	Out = Out & v & fileInSearch.Extension
				Dim x As New FileInfo(Out)
				Return x

			End Function

			Shared Function GetIntrestingColumns(File As FileInfo,
												 NofHeaderLine As Integer,
												 Optional Title As String = "Select Columns") As List(Of Integer)
				Dim Headers = Szunyi.IO.Files.GetHeader(File, NofHeaderLine, Nothing, Nothing)

				Using x As New Select_Columns(Headers)
					x.Text = Title
					If x.ShowDialog = System.Windows.Forms.DialogResult.OK Then
						Return x.SelectedIndexes.ToList
					Else : Return Nothing
					End If
				End Using
			End Function

			Public Shared Function GetValuesToDictionary(File As FileInfo,
													  IDColIndex As Integer,
													  ValuesColIndexes As List(Of Integer),
													  NofHeaderLines As Integer,
													   Optional Separator As String = vbTab) As Dictionary(Of String, List(Of String))
				Dim out As New Dictionary(Of String, List(Of String))
				Dim NofLine As Integer = 0
				Dim Line As String
				Dim s1()
				Dim log As New StringBuilder
				Try

					Using sr As New StreamReader(File.FullName)
						For i1 = 1 To NofHeaderLines
							sr.ReadLine()
							NofLine += 1
						Next
						Do
							Line = sr.ReadLine
							NofLine += 1
							s1 = Split(Line, Separator)
							Dim s = Szunyi.Text.GetText(s1, ValuesColIndexes, vbTab)
							Dim Key = s1(IDColIndex)
							If out.ContainsKey(Key) = False Then
								out.Add(Key, New List(Of String))
								out(Key).Add(s)
							Else
								out(Key).Add(s)
								log.Append(Key).AppendLine()
							End If


						Loop Until sr.EndOfStream = True
					End Using
				Catch ex As Exception
					MsgBox(ex.ToString)
				End Try
				Return out
			End Function

		End Class
		Public Class Import
			Public Class Text
				Public Shared Function ReadToEnd(file As FileInfo) As String
					Using sw As New StreamReader(file.FullName)
						Return sw.ReadToEnd
					End Using
				End Function

				Public Shared Function GetHeader(TheFile As FileInfo, NofHeaderLine As Integer,
										 Optional ToRemove As List(Of String) = Nothing) As List(Of String)
					Dim res As List(Of String)
					Dim s1() As String
					If IsNothing(TheFile) = True Then Return Nothing
					Using sr As New StreamReader(TheFile.FullName)



						If IsNothing(ToRemove) = True Then
							For i1 = 1 To NofHeaderLine
								s1 = Split(sr.ReadLine, vbTab)
								If IsNothing(res) = True Then
									res = s1.ToList
								Else
									For i2 = 0 To s1.Count - 1
										res(i2) = res(i2) & " " & s1(i2)
									Next
								End If
							Next
						Else
							For i1 = 1 To NofHeaderLine
								Dim Line As String = sr.ReadLine
								For Each Item In ToRemove
									Line = Line.Replace(Item, "")
								Next
								s1 = Split(Line, vbTab)
								If IsNothing(res) = True Then
									res = s1.ToList
								Else
									For i2 = 0 To s1.Count - 1
										res(i2) = res(i2) & " " & s1(i2)
									Next
								End If
							Next
						End If
					End Using
					Return res
				End Function

				Shared Function GetListForReplace(File As FileInfo, ColumnOfOriginalString As Integer, ColumnOfNewString As Integer) As List(Of String())
					Dim res As New List(Of String())
					Using sr As New StreamReader(File.FullName)
						sr.ReadLine()
						Do
							Dim s1() = Split(sr.ReadLine, vbTab)
							If s1.Length > 1 Then
								Dim x(1) As String
								x(0) = s1(ColumnOfOriginalString)
								x(1) = s1(ColumnOfNewString)
								res.Add(x)
							End If

						Loop Until sr.EndOfStream = True
					End Using

					Dim res2 = (From t In res Order By t(0) Ascending).ToList

					Return res2
				End Function

			End Class
			Public Class Sequence
                Public seqs As New List(Of bio.sequence)
                Public streams As New List(Of Stream)
				Public SeqFiles As New List(Of FileInfo)

				Public Sub New(File As FileInfo)
					Dim fa = Bio.IO.SequenceParsers.FindParserByFileName(File.FullName)
					If IsNothing(fa) = False Then SeqFiles.Add(File)

                End Sub
				Public Sub New(Files As List(Of FileInfo))
					For Each File In Files
                        Dim fa = Bio.IO.SequenceParsers.FindParserByFileName(File.FullName)
                        If IsNothing(fa) = False Then SeqFiles.Add(File)
					Next
				End Sub
				Public Sub DoIt()
                    For Each File In SeqFiles
                        Dim x As New FileStream(File.FullName, FileMode.Open)
                        Try

                            Dim fa = Bio.IO.SequenceParsers.FindParserByFileName(File.FullName)
                            For Each Seq In fa.Parse(x)
                                Me.seqs.Add(Seq)
                            Next
                        Catch ex As Exception
                            Dim alf As Int16 = 54
                        End Try
                        x.Close()
                    Next
                End Sub
				Public Function ImportIntoList(File As FileInfo) As List(Of Bio.Sequence)
					Dim Out As New List(Of Bio.Sequence)
					Dim x As New FileStream(File.FullName, FileMode.Open)
					Dim fa = Bio.IO.SequenceParsers.FindParserByFileName(File.FullName)
					For Each Seq In fa.Parse(x)
						Out.Add(Seq)
					Next
					x.Close()
					Return Out
				End Function
			End Class



		End Class
		Public Class Export
			Public Shared Sub SaveText(ByVal Text As String, Optional Title As String = "Save As")
				Dim sfd1 As New SaveFileDialog
				sfd1.Title = Title
				If sfd1.ShowDialog = DialogResult.OK Then
					Using sg As New System.IO.StreamWriter(sfd1.FileName)
						sg.Write(Text)
					End Using
				End If

			End Sub

			Public Shared Sub SaveText(ByVal Text As String, File As FileInfo)

				Using sg As New System.IO.StreamWriter(File.FullName)
					sg.Write(Text)
				End Using


			End Sub

			Friend Shared Sub SequncesToFasta(out As List(Of Sequence), Optional fileInfo As FileInfo = Nothing)
				If IsNothing(fileInfo) = True Then fileInfo = Szunyi.IO.Files.SelectSaveFile
				If IsNothing(fileInfo) = True Then Exit Sub
				Try
					Dim sw As New Bio.IO.FastA.FastAFormatter
					Dim TheStream As New FileStream(fileInfo.FullName, FileMode.Create)
					sw.Format(TheStream, out)
				Catch ex As Exception

				End Try



			End Sub

		End Class
	End Namespace
End Namespace

