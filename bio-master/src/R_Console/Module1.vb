Imports System.IO

Module Module1

    Sub Main()

        Dim Def_Dir As New DirectoryInfo("C:\R\")
        Dim str As New System.Text.StringBuilder
        str.Append("library(VennDiagram); venn.plot <- draw.quad.venn( area1 = 72, area2 = 86, area3 = 50, area4 = 52, n12 = 44, n13 = 27, n14 = 32,n23 = 38, n24 = 32, n34 = 20, n123 = 18,n124 = 17, n134 = 11,n234 = 13, n1234 = 6 ")
        str.Append(", category = c(")
        Dim Names = Split("A,B,C,D", ",")
        Dim NamesII = Add_Before_And_End(Names.ToList, Chr(34))
        Dim Color = Split("orange,red,green,blue", ",")
        Dim ColorII = Add_Before_And_End(Names.ToList, Chr(34))
        str.Append(Insert(NamesII, ","))
        str.Append("),  fill = c(")
        str.Append(Insert(ColorII, ","))
        str.Append("); ")
        str.Append("tiff(filename = " & Chr(34) & "C:/R/Quad_Venn_diagram13.tiff" & Chr(34) & ";")
        str.Append("grid.draw(Venn.plot); ")
        str.Append("dev.off();")
        '  Szunyi.IO.Export.SaveText(str.ToString, New FileInfo(Def_Dir.FullName & "test.r"))
        Dim Engine = RDotNet.REngine.CreateInstance("test ")
        Engine.Evaluate(str.ToString)

    End Sub
    Friend Function Add_Before_And_End(names As List(Of String), ToAdd As String) As List(Of String)
        Dim out As New List(Of String)
        For Each Name In names
            out.Add(ToAdd & Name & ToAdd)
        Next
        Return out
    End Function

    Friend Function Insert(namesII As List(Of String), Insertion As String) As Object
        Dim str As New System.Text.StringBuilder
        For i1 = 0 To namesII.Count - 2
            str.Append(namesII(i1)).Append(Insertion)
        Next
        str.Append(namesII.Last)
        Return str.ToString
    End Function


End Module
