Imports System.IO
Namespace Szunyi.BAM
    Public Class Orientation
        ''' <summary>
        ''' Seqs are Reads Come From Fasta or Fastq File, Files are BamFiles
        ''' Induvidual indicate Merge or working with Induvidual FIles
        ''' </summary>
        ''' <param name="Seqs"></param>
        ''' <param name="Files"></param>
        ''' <param name="induvidual"></param>
        Public Shared Sub dRNA_MinIon(Seqs As List(Of Bio.ISequence), Files As List(Of FileInfo), Induvidual As Boolean, cSetting As Szunyi.Transcipts.Score_Settings)
            Dim c As New Szunyi.Comparares.OneByOne.SequenceIDComparer
            Seqs.Sort(c)

            If Induvidual = True Then
                For Each File In Files
                    Dim Header = Bam_Basic_IO.Headers.Get_Header(File)
                    Dim Out As New List(Of Bio.IO.SAM.SAMAlignedSequence)
                    For Each SAM In Szunyi.BAM.Bam_Basic_IO.Import.Parse(File)
                        Dim Index = Seqs.BinarySearch(SAM.QuerySequence, c)
                        If Index > -1 Then
                            If SAM.QuerySequence.ConvertToString = Seqs(Index).ConvertToString Then
                                SAM.Flag = 0
                            Else
                                SAM.Flag = Bio.IO.SAM.SAMFlags.QueryOnReverseStrand
                            End If

                            Out.Add(SAM)
                        End If
                    Next
                    Dim nFile As New FileInfo(File.FullName & "_Good_Orientation.sam")
                    Szunyi.IO.Export.Export_Sam(Out, Header, nFile)
                Next
            Else
                Dim SaveFile = Szunyi.IO.Files.Save.SelectSaveFile(Constants.Files.SAM)
                If IsNothing(SaveFile) = False Then
                    Dim Headers = Bam_Basic_IO.Headers.Get_Header(Files)
                    Dim Out As New List(Of Bio.IO.SAM.SAMAlignedSequence)
                    For Each SAM In Szunyi.BAM.Bam_Basic_IO.Import.Parse(Files)
                        Dim Index = Seqs.BinarySearch(SAM.QuerySequence, c)
                        If Index > -1 Then
                            If SAM.QuerySequence.ConvertToString = Seqs(Index).ConvertToString Then
                                SAM.Flag = 0
                            Else
                                SAM.Flag = Bio.IO.SAM.SAMFlags.QueryOnReverseStrand
                            End If

                            Out.Add(SAM)
                        End If
                    Next

                    Szunyi.IO.Export.Export_Sam(Out, Headers.First, SaveFile)
                End If
            End If
        End Sub
    End Class

End Namespace

