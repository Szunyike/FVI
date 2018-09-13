Imports System.IO
Imports System.Text
Imports Bio
Imports Bio.IO.SAM

Namespace Szunyi.BAM.Bam_Basic_IO
    Public Class Export
        Implements IDisposable

        Dim tw As StreamWriter
        Dim SAM_Writter As Bio.IO.SAM.SAMFormatter
        Public Shared Sub Merge_Sams_RG(Files As List(Of FileInfo), nFIle As FileInfo)
            Dim Headers = Bam_Basic_IO.Headers.Get_Header(Files)
            Dim Header = Bam_Basic_IO.Headers.Merge(Headers)
            Header = Bam_Basic_IO.Headers.Add_RGs(Header, Files)
            Using x As New Export(nFIle, Header)
                For Each File In Files
                    For Each SAM In Bam_Basic_IO.Import.Parse(File)
                        BAM.BAM_Optional_Filed_Manipulation.Add(SAM, "RG", "Z", File.Name)
                        x.Write(SAM)
                    Next
                Next
            End Using
        End Sub
        Public Shared Sub Merge_Sams(Files As List(Of FileInfo), nFIle As FileInfo)
            Dim Headers = Bam_Basic_IO.Headers.Get_Header(Files)
            Dim Header = Bam_Basic_IO.Headers.Merge(Headers)
            '  Header = Bam_Basic_IO.Headers.Add_RGs(Header, Files)
            Using x As New Export(nFIle, Header)
                For Each File In Files
                    For Each SAM In Bam_Basic_IO.Import.Parse(File)
                        x.Write(SAM)
                    Next
                Next
            End Using
        End Sub

        Public Sub New(File As FileInfo, Header As SAMAlignmentHeader)
            tw = New StreamWriter(File.FullName, False)
            SAM_Writter = New Bio.IO.SAM.SAMFormatter()
            Bio.IO.SAM.SAMFormatter.WriteHeader(tw, Header)
            tw.Flush()
        End Sub
        Public Sub New(File As FileInfo)
            Dim Header = Headers.Get_Header(File)
            tw = New StreamWriter(File.FullName, True)
            SAM_Writter = New Bio.IO.SAM.SAMFormatter()
            Bio.IO.SAM.SAMFormatter.WriteHeader(tw, Header)
            tw.Flush()
        End Sub
        ''' <summary>
        ''' Write the SAM to the file end
        ''' </summary>
        ''' <param name="SAM"></param>
        Public Sub Write(SAM As SAMAlignedSequence)
            Bio.IO.SAM.SAMFormatter.WriteSAMAlignedSequence(tw, SAM)
            tw.Flush()
        End Sub
        ''' <summary>
        ''' Write the SAMs to the file end
        ''' </summary>
        ''' <param name="SAMs"></param>
        Public Sub Writes(SAMs As IEnumerable(Of SAMAlignedSequence))
            For Each SAM In SAMs
                Bio.IO.SAM.SAMFormatter.WriteSAMAlignedSequence(tw, SAM)
            Next
            tw.Flush()
        End Sub

#Region "IDisposable Support"
        Private disposedValue As Boolean ' To detect redundant calls

        ' IDisposable
        Protected Overridable Sub Dispose(disposing As Boolean)
            If Not disposedValue Then
                If disposing Then
                    ' TODO: dispose managed state (managed objects).
                End If
                tw.Flush()
                tw.Close()
                ' TODO: free unmanaged resources (unmanaged objects) and override Finalize() below.
                ' TODO: set large fields to null.
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
End Namespace

