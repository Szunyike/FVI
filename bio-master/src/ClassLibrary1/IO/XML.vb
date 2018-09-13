Imports System.Xml.Serialization

Namespace Szunyi.IO
    Public Class XML
        Public Shared Function Serialize(Of T)(ByVal dataToSerialize As T) As String
            Try
                Dim stringwriter = New System.IO.StringWriter()
                Dim serializer = New XmlSerializer(GetType(T))
                serializer.Serialize(stringwriter, dataToSerialize)
                Return stringwriter.ToString()
            Catch ex As Exception
                Throw
            End Try
        End Function
        Public Shared Function Deserialize(Of T)(ByVal xmlText As String) As T
            Try
                Dim stringReader = New System.IO.StringReader(xmlText)
                Dim serializer = New XmlSerializer(GetType(T))
                Return CType(serializer.Deserialize(stringReader), T)
            Catch ex As Exception
                Throw
            End Try
        End Function

    End Class
End Namespace

