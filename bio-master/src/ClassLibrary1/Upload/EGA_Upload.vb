Imports System.IO

Namespace Szunyi.Upload.EGA
    Public Class Analysys
        Public Shared Function Get_Submission_Add(Files As List(Of FileInfo))
            Dim str As New System.Text.StringBuilder
            str.Append("<?xml version=" & Chr(34) & "1.0" & Chr(34) & " encoding=" & Chr(34) & "ISO-8859-1" & Chr(34) & "?>").AppendLine()
            str.Append("<SUBMISSION_SET xsi:noNamespaceSchemaLocation=" & Chr(34) & "ftp: //ftp.sra.ebi.ac.uk/meta/xsd/sra_1_5/SRA.submission.xsd" & Chr(34) & " xmlns:xsi=" & Chr(34) & "http://www.w3.org/2001/XMLSchema-instance" & Chr(34) & ">").AppendLine()
            str.Append("<ACTIONS>")
            For Each FIle In Files
                str.Append("<ACTION>").AppendLine()
                str.Append("<ADD schema=" & Chr(34) & "analysis" & Chr(34) & "source=" & Chr(34) & "Analysis.xml" & Chr(34) & "/>").AppendLine()
                str.Append("</ACTION>").AppendLine()
            Next
            str.Append(" </ACTIONS>").AppendLine()
            str.Append("</SUBMISSION>").AppendLine()
            str.Append("</SUBMISSION_SET>")
            Return str.ToString
        End Function
        Public Shared Function Get_Analysis_XML(Files As List(Of FileInfo))
            Dim str As New System.Text.StringBuilder
            str.Append("<?xml version=" & Chr(34) & "1.0" & Chr(34) & "encoding = " & Chr(34) & "ISO-8859-1" & Chr(34) & "?>").AppendLine()
            str.Append("<ANALYSIS_SET>").AppendLine()


        End Function
    End Class
End Namespace

