Imports Microsoft.VisualBasic.CommandLine.Reflection
Imports Microsoft.VisualBasic.ConsoleDevice.STDIO
Imports Microsoft.VisualBasic.Scripting.MetaData
Imports Microsoft.VisualBasic.Linq
Imports Microsoft.VisualBasic.DocumentFormat.Csv
Imports RDotNET.Extensions.VisualBasic.RSystem
Imports RDotNET.Extensions.VisualBasic
Imports RDotNET.Extensions.Bioinformatics.VennDiagram.ModelAPI
Imports Microsoft.VisualBasic.DocumentFormat.Csv.DocumentStream

Public Class R
    Public Shared Function lvector(source1 As System.IO.FileInfo) As Dictionary(Of String, String())
        Dim source = New File(source1.FullName)
        Dim Width As Integer = source.First.Count
        Dim Vector = (From name As String
                      In source.First
                      Select k = name,
                          lst = New Microsoft.VisualBasic.List(Of String)).ToArray

        For row As Integer = 1 To source.RowNumbers - 1
            Dim Line As RowObject = source(row)
            For colums As Integer = 0 To Width - 1
                If Not String.IsNullOrEmpty(Line.Column(colums).Trim) Then
                    Call Vector(colums).lst.Add(CStr(row))
                End If
            Next
        Next
        Dim l = Vector.ToDictionary(Function(x) x.k, Function(x) x.lst.ToArray)
        Dim h1 = Generate(source)
        ' Return h1
        Dim laf As Int16 = 54
    End Function

    Private Shared Function GetRandomColor() As String
        Call VBMath.Randomize()
        Return RSystem.RColors(Rnd() * (RSystem.RColors.Length - 1))
    End Function

    Public Shared Sub Run(File As System.IO.FileInfo)
        Dim title As String = "title"
        Dim Out As New System.IO.FileInfo(File.FullName & ".test")
        Dim dataset As DocumentStream.File = New DocumentStream.File(File.FullName)
        Dim VennDiagram As VennDiagram = RModelAPI.Generate(source:=dataset)
        VennDiagram += From col As String In dataset.First Select {col, GetRandomColor()} '
        Dim R_HOME As String = "C:/R/R-3.2.5/bin/"


        VennDiagram.Title = "title"
        VennDiagram.saveTiff = File.FullName & ".scipt"

        Dim RScript As String = VennDiagram.RScript
        Dim EXPORT As String = FileIO.FileSystem.GetParentPath(File.FullName)
        EXPORT = $"{EXPORT}/{title.NormalizePathString}_venn.r"

        If Not R_HOME.DirectoryExists Then
            Call TryInit()
        Else
            Call TryInit(R_HOME)
        End If

        Call RScript.SaveTo(EXPORT, Encodings.ASCII.GetEncodings)
        Call VennDiagram.SaveAsXml(EXPORT.TrimFileExt & ".Xml")
        Call RSystem.Source(EXPORT)

        Printf("The venn diagram r script were saved at location:\n '%s'", EXPORT)
        Call Process.Start(Out.FullName)


    End Sub
    Public Shared Function __vector(source As File) As Dictionary(Of String, String())
        Dim Width As Integer = source.First.Count
        Dim Vector = (From name As String
                      In source.First
                      Select k = name,
                          lst = New Microsoft.VisualBasic.List(Of String)).ToArray

        For row As Integer = 1 To source.RowNumbers - 1
            Dim Line As RowObject = source(row)
            For colums As Integer = 0 To Width - 1
                If Not String.IsNullOrEmpty(Line.Column(colums).Trim) Then
                    Call Vector(colums).lst.Add(CStr(row))
                End If
            Next
        Next

        Return Vector.ToDictionary(Function(x) x.k, Function(x) x.lst.ToArray)
    End Function
    Public Shared Function Generate(source As DocumentStream.File) As VennDiagram
        Dim LQuery = From vec
                         In __vector(source:=source)
                     Select New Partition With {
                             .Vector = String.Join(", ", vec.Value),
                             .Name = vec.Key
                         } '
        Return New VennDiagram With {
                .partitions = LQuery.ToArray
            }
    End Function

End Class
