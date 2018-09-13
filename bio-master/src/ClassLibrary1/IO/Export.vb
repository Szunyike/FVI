Imports System.IO
Imports System.Text
Imports System.Windows.Forms
Imports Bio

Namespace Szunyi
    Namespace IO
        Public Class Export
            Public Class fastQ_Writter
                Implements IDisposable

                Public Property Stream As FileStream
                Public Property FQ_Writter As Bio.IO.FastQ.FastQFormatter
                Public Sub New(File As FileInfo)
                    FQ_Writter = New Bio.IO.FastQ.FastQFormatter
                    FQ_Writter.Open(File.FullName)
                End Sub
                Public Sub write(Seq As Bio.Sequence)
                    FQ_Writter.Format(Seq)
                End Sub
                Public Sub writeQ(Seq As Bio.QualitativeSequence)
                    FQ_Writter.Format(Seq)
                End Sub

#Region "IDisposable Support"
                Private disposedValue As Boolean ' To detect redundant calls

                ' IDisposable
                Protected Overridable Sub Dispose(disposing As Boolean)
                    If Not disposedValue Then
                        If disposing Then
                            ' TODO: dispose managed state (managed objects).
                        End If

                        FQ_Writter.Close
                    End If
                    disposedValue = True
                End Sub

                ' TODO: override Finalize() only if Dispose(disposing As Boolean) above has code to free unmanaged resources.
                'Protected Overrides Sub Finalize()
                '    ' Do not change this code.  Put cleanup code in Dispose(disposing As Boolean) above.
                '    Dispose(False)
                '    MyBase.Finalize()
                'End Sub

                ' This code added by Visual Basic to correctly implement the disposable pattern.
                Public Sub Dispose() Implements IDisposable.Dispose
                    ' Do not change this code.  Put cleanup code in Dispose(disposing As Boolean) above.
                    Dispose(True)
                    ' TODO: uncomment the following line if Finalize() is overridden above.
                    ' GC.SuppressFinalize(Me)
                End Sub
#End Region
            End Class
            Public Class fasta_Writter
                Implements IDisposable

                Public Property Stream As FileStream
                Public Property FQ_Writter As Bio.IO.FastA.FastAFormatter
                Public Sub New(File As FileInfo)
                    FQ_Writter = New Bio.IO.FastA.FastAFormatter
                    FQ_Writter.Open(File.FullName)
                End Sub
                Public Sub write(Seq As Bio.Sequence)
                    FQ_Writter.Format(Seq)
                End Sub
                Public Sub writeQ(Seq As Bio.QualitativeSequence)
                    FQ_Writter.Format(Seq)
                End Sub

#Region "IDisposable Support"
                Private disposedValue As Boolean ' To detect redundant calls

                ' IDisposable
                Protected Overridable Sub Dispose(disposing As Boolean)
                    If Not disposedValue Then
                        If disposing Then
                            ' TODO: dispose managed state (managed objects).
                        End If

                        FQ_Writter.Close
                    End If
                    disposedValue = True
                End Sub

                ' TODO: override Finalize() only if Dispose(disposing As Boolean) above has code to free unmanaged resources.
                'Protected Overrides Sub Finalize()
                '    ' Do not change this code.  Put cleanup code in Dispose(disposing As Boolean) above.
                '    Dispose(False)
                '    MyBase.Finalize()
                'End Sub

                ' This code added by Visual Basic to correctly implement the disposable pattern.
                Public Sub Dispose() Implements IDisposable.Dispose
                    ' Do not change this code.  Put cleanup code in Dispose(disposing As Boolean) above.
                    Dispose(True)
                    ' TODO: uncomment the following line if Finalize() is overridden above.
                    ' GC.SuppressFinalize(Me)
                End Sub
#End Region
            End Class
            Public Shared Sub Location(File As FileInfo)
                Using sw As New StreamWriter(File.FullName & ".loc")
                    Dim FIrst As Boolean = True
                    For Each BL In Szunyi.BAM.Bam_Basic_IO.Import.Parse_Into_Basic_Locations(File)
                        If FIrst = True Then
                            sw.Write(Szunyi.Location.Common.GetLocationString(BL.Location))
                            FIrst = False
                        Else
                            sw.WriteLine()
                            sw.Write(Szunyi.Location.Common.GetLocationString(BL.Location))
                        End If
                    Next
                End Using
            End Sub
            Public Shared Sub Location(Files As IEnumerable(Of FileInfo))
                For Each FIle In Files
                    Location(FIle)
                Next
            End Sub




#Region "Text"
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
                    Try
                        Using sg As New System.IO.StreamWriter(File.FullName)
                            sg.Write(Text)
                        End Using
                    Catch ex As Exception
                        MsgBox(ex.ToString)
                    End Try



                End Sub
#End Region

#Region "Sequences"
                Public Shared Sub SaveSequencesToSingleFasta(out As List(Of Bio.Sequence), Optional fileInfo As FileInfo = Nothing)
                    If IsNothing(fileInfo) = True Then fileInfo = Szunyi.IO.Files.Save.SelectSaveFile(Szunyi.Constants.Files.Fasta, "Save As e:" & out.Count)
                    If IsNothing(fileInfo) = True Then Exit Sub
                    If out.Count < 1 Then Exit Sub
                    If out.First.GetType.ToString.Contains("Qual") Then
                        SaveAsQualitativeSeq(out, fileInfo)
                    Else
                        SaveAsSimpleFasta(out, fileInfo)
                    End If

                End Sub
                Public Shared Sub SaveSequencesToSingleFasta(out As List(Of Bio.ISequence), Optional fileInfo As FileInfo = Nothing)
                    If IsNothing(fileInfo) = True Then fileInfo = Szunyi.IO.Files.Save.SelectSaveFile(Szunyi.Constants.Files.Fasta, "Save As e:" & out.Count)
                    If IsNothing(fileInfo) = True Then Exit Sub

                    Dim sw As New Bio.IO.FastA.FastAFormatter
                    Dim TheStream As New FileStream(fileInfo.FullName, FileMode.Create)
                    Try
                        sw.Format(TheStream, out)
                    Catch ex As Exception
                        MsgBox(ex.ToString)
                    End Try
                    TheStream.Close()


                End Sub
                Public Shared Sub SaveAsQualitativeSeq(out As List(Of Bio.ISequence), File As FileInfo)
                    Dim sw As New Bio.IO.FastQ.FastQFormatter
                    Dim TheStream As New FileStream(File.FullName, FileMode.Create)
                    Try
                        sw.Format(TheStream, out)
                    Catch ex As Exception
                        MsgBox(ex.ToString)
                    End Try
                    TheStream.Close()

                End Sub
                Public Shared Sub SaveAsQualitativeSeq(out As List(Of Bio.Sequence), File As FileInfo)
                    Dim sw As New Bio.IO.FastQ.FastQFormatter
                    Dim TheStream As New FileStream(File.FullName, FileMode.Create)
                    Try
                        sw.Format(TheStream, out)
                    Catch ex As Exception
                        MsgBox(ex.ToString)
                    End Try
                    TheStream.Close()

                End Sub
                Public Shared Sub SaveAsSimpleFasta(out As List(Of Bio.Sequence), File As FileInfo)
                    Dim sw As New Bio.IO.FastA.FastAFormatter
                    Dim TheStream As New FileStream(File.FullName, FileMode.Create)
                    Try
                        sw.Format(TheStream, out)
                    Catch ex As Exception
                        MsgBox(ex.ToString)
                    End Try
                    TheStream.Close()

                End Sub
                Public Shared Sub SaveAsSimpleFasta(out As Bio.ISequence, File As FileInfo)
                    Dim sw As New Bio.IO.FastA.FastAFormatter
                    Dim TheStream As New FileStream(File.FullName, FileMode.Create)
                    Try
                        sw.Format(TheStream, out)
                    Catch ex As Exception
                        MsgBox(ex.ToString)
                    End Try
                    TheStream.Close()

                End Sub
                Public Shared Sub SaveAsSimpleFasta(out As List(Of Bio.ISequence), File As FileInfo)
                    Dim sw As New Bio.IO.FastA.FastAFormatter
                    Dim TheStream As New FileStream(File.FullName, FileMode.Create)
                    Try
                        sw.Format(TheStream, out)
                    Catch ex As Exception
                        MsgBox(ex.ToString)
                    End Try
                    TheStream.Close()

                End Sub

                Public Shared Sub SaveSequencesToSingleFastQ(out As List(Of Bio.ISequence), Optional fileInfo As FileInfo = Nothing)
                    If IsNothing(fileInfo) = True Then fileInfo = Szunyi.IO.Files.Save.SelectSaveFile(Szunyi.Constants.Files.Fasta, "Save As e:" & out.Count)
                    If IsNothing(fileInfo) = True Then Exit Sub

                    Dim sw As New Bio.IO.FastQ.FastQFormatter
                    Dim TheStream As New FileStream(fileInfo.FullName, FileMode.Create)
                    Try
                        sw.Format(TheStream, out)
                    Catch ex As Exception
                        MsgBox(ex.ToString)
                    End Try
                    TheStream.Close()


                End Sub
                Public Shared Sub SaveSequencesToSingleGenBank(out As List(Of ISequence),
                                                               Optional fileInfo As FileInfo = Nothing, Optional WithCheck As Boolean = False)
                    If IsNothing(fileInfo) = True Then fileInfo = Szunyi.IO.Files.Save.SelectSaveFile(Szunyi.Constants.Files.GenBank)
                    If IsNothing(fileInfo) = True Then Exit Sub
                    Dim log As New StringBuilder
                    If WithCheck = True Then Szunyi.Sequences.SequenceManipulation.Reapair.ReapirSeqs(out)

                    Dim sw As New Bio.IO.GenBank.GenBankFormatter

                    Dim TheStream As New FileStream(fileInfo.FullName, FileMode.Create)
                    Try
                        sw.Format(TheStream, out)
                    Catch ex As Exception
                        Dim alf As Int16 = 54
                    End Try
                    TheStream.Close()

                End Sub
                Public Shared Sub SaveSequencesToMultipleGenBank(out As List(Of ISequence),
                                                             Optional Dir As DirectoryInfo = Nothing, Optional WithCheck As Boolean = False, Optional WithDate As Boolean = True)
                    If IsNothing(Dir) = True Then Dir = Szunyi.IO.Directory.Get_Folder
                    If IsNothing(Dir) = True Then Exit Sub
                    Dim log As New StringBuilder
                    If WithCheck = True Then Szunyi.Sequences.SequenceManipulation.Reapair.ReapirSeqs(out)

                    Dim sw As New Bio.IO.GenBank.GenBankFormatter
                    For Each Seq In out
                        If WithDate = True Then
                            Dim date1 = Now
                            Dim y = date1.Year & "_" & date1.Month & "_" & date1.Day
                            Dim TheStream As New FileStream(Dir.FullName & "\" & Seq.ID & "_" & y & ".gbk", FileMode.Create)
                            sw.Format(TheStream, Seq)

                            TheStream.Close()
                        Else
                            Dim TheStream As New FileStream(Dir.FullName & "\" & Seq.ID & ".gbk", FileMode.Create)
                            Try
                                sw.Format(TheStream, Seq)
                            Catch ex As Exception
                                Dim alf As Int16 = 54
                            End Try
                            TheStream.Close()
                        End If

                    Next


                End Sub
                Public Shared Sub SaveSequencesToSingleGenBank(out As ISequence,
                                                               Optional fileInfo As FileInfo = Nothing, Optional WithCheck As Boolean = False)
                    If IsNothing(fileInfo) = True Then fileInfo = Szunyi.IO.Files.Save.SelectSaveFile(Szunyi.Constants.Files.GenBank)
                    If IsNothing(fileInfo) = True Then Exit Sub
                    Dim log As New StringBuilder
                    If WithCheck = True Then Szunyi.Sequences.SequenceManipulation.Reapair.ReapirSeqs(out)

                    Dim sw As New Bio.IO.GenBank.GenBankFormatter

                    Dim TheStream As New FileStream(fileInfo.FullName, FileMode.Create)
                    Try
                        sw.Format(TheStream, out)
                    Catch ex As Exception
                        Dim alf As Int16 = 54
                    End Try
                    TheStream.Close()

                End Sub
                Public Shared Sub Sequences(allSeqs As List(Of ISequence), WithCheck As Boolean)
                    Dim ofd1 As New OpenFileDialog
                    ofd1.ValidateNames = False
                    ofd1.CheckFileExists = False
                    ofd1.CheckPathExists = True
                    ofd1.Multiselect = False
                    ofd1.Filter = Szunyi.Constants.Files.SequenceFileTypeToSave
                    ofd1.FilterIndex = 0
                    ofd1.FileName = Szunyi.Constants.Files.SelectFolder
                    If allSeqs.Count = 0 Then Exit Sub
                    If allSeqs.First.Metadata.ContainsKey(Bio.Util.Helper.GenBankMetadataKey) = True Then
                        ofd1.FilterIndex = 3
                    End If
                    If ofd1.ShowDialog = DialogResult.OK Then
                        Select Case ofd1.FilterIndex
                            Case 1
                                Dim File As New FileInfo(ofd1.FileName)
                                SaveSequencesToSingleFasta(allSeqs, File)
                            Case 2
                                If ofd1.FileName = Szunyi.Constants.Files.SelectFolder Then

                                End If
                            Case 3 ' FastQ
                                Dim File As New FileInfo(ofd1.FileName)
                                SaveAsQualitativeSeq(allSeqs, File)
                            Case 4
                                Dim File As New FileInfo(ofd1.FileName)
                                SaveSequencesToSingleGenBank(allSeqs, File, WithCheck)
                        End Select
                    End If
                End Sub

                Public Shared Sub SaveAsSimpleFasta(folder As DirectoryInfo, five_Clipped As Dictionary(Of Integer, List(Of Sequence)), v As String)
                    For Each Item In five_Clipped
                        Dim NewFIle As New FileInfo(folder.FullName & "\" & v & Item.Key & "e" & Item.Value.Count & ".fa")
                        Dim Seqs = Item.Value.ToList
                        If Seqs.Count > 1 Then
                            Szunyi.IO.Export.SaveSequencesToSingleFasta(Seqs, NewFIle)
                        End If
                    Next
                End Sub

#End Region

                Public Shared Sub Export_Sam(Sams As List(Of Bio.IO.SAM.SAMAlignedSequence), Header As Bio.IO.SAM.SAMAlignmentHeader, Optional File As FileInfo = Nothing)
                    Dim x As New Bio.IO.SAM.SAMFormatter()
                    If IsNothing(File) = True Then
                        Dim s = Szunyi.IO.Files.Save.SelectSaveFile(Szunyi.Constants.Files.SAM)
                        If IsNothing(s) = True Then Exit Sub
                        File = s
                    End If

                    Dim str As New FileStream(File.FullName, FileMode.Create)
                    x.Format(str, Sams, Header)
                    str.Flush()
                    str.Close()
                    x.Close
                End Sub
            End Class
    End Namespace

End Namespace
