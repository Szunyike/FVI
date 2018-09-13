Imports HDF5DotNet
Imports System.IO
Namespace Szunyi.HDF5
    Public Class Common
        Public Shared Function Get_Attributes_with_Values(FIle As FileInfo, Group_Name As String) As Dictionary(Of String, String)

            Dim FileID As H5FileOrGroupId = HDF5DotNet.H5F.open(FIle.FullName, H5F.OpenMode.ACC_RDONLY)

            Dim kk = H5G.open(FileID, "UniqueGlobalKey")
            Dim g1 As H5GroupId
            Try
                Dim k2 = H5G.open(FileID, "Raw/Reads")
                g1 = H5G.open(k2, Group_Name)
            Catch ex As Exception
                Return Nothing
            End Try




            Dim out As New Dictionary(Of String, String)
            For i1 = 0 To H5A.getNumberOfAttributes(g1) - 1
                Dim Attr_ID = H5A.openIndex(g1, i1)
                Dim dType = H5A.getType(Attr_ID)

                Dim jk1 = H5A.getInfo(Attr_ID)

                Dim tmp(jk1.dataSize - 1) As Byte
                Dim Buffer As New H5Array(Of Byte)(tmp)
                H5A.read(Of Byte)(Attr_ID, dType, Buffer)

                Dim enc As New System.Text.ASCIIEncoding
                Dim rres = enc.GetString(tmp).Replace(vbNullChar, "")
                Dim s As String = H5A.getName(Attr_ID)
                out.Add(s, rres)


            Next
            Return out
        End Function
        Public Shared Function Get_Attributes_with_Values(FIles As List(Of FileInfo), Group_Name As String) As Dictionary(Of String, List(Of String))
            Dim out As New Dictionary(Of String, List(Of String))
            Dim tmp As New List(Of Dictionary(Of String, String))
            For Each FIle In FIles
                tmp.Add(Get_Attributes_with_Values(FIle, Group_Name))
            Next
            ' Dim Keys As New List(Of String)
            Dim x = From h In tmp Select h.Keys

            For Each X1 In x
                For Each Key In X1
                    If out.ContainsKey(Key) = False Then out.Add(Key, New List(Of String))
                Next
            Next
            Dim NofItem As Integer = 1
            For Each t In tmp
                For Each Item In t
                    out(Item.Key).Add(Item.Value)
                Next
                For Each Item In out
                    If Item.Value.Count < NofItem Then
                        Item.Value.Add(String.Empty)
                    End If
                Next
                NofItem += 1
            Next
            Return out
        End Function

        ''' <summary>
        ''' 'Return Nothing or Array
        ''' </summary>
        ''' <typeparam name="T"></typeparam>
        ''' <param name="fileId"></param>
        ''' <param name="dataSetName"></param>
        ''' <returns></returns>
        Public Shared Function Read3DArray(Of T)(fileId As H5FileId, dataSetName As String) As T(,,)
            Try
                Dim dataset = H5D.open(fileId, dataSetName)
                Dim space = H5D.getSpace(dataset)
                Dim dims = H5S.getSimpleExtentDims(space)
                Dim dataType = H5D.[getType](dataset)
                ' this will also need a string hack...
                If GetType(T) = GetType(String) Then
                End If
                Dim dataArray As T(,,) = New T(dims(0) - 1, dims(1) - 1, dims(2) - 1) {}
                Dim wrapArray = New H5Array(Of T)(dataArray)
                H5D.read(dataset, dataType, wrapArray)
                Return dataArray
            Catch ex As Exception
                Return Nothing
            End Try

        End Function
        ''' <summary>
        ''' 'Return Nothing or Array
        ''' </summary>
        ''' <typeparam name="T"></typeparam>
        ''' <param name="fileId"></param>
        ''' <param name="dataSetName"></param>
        ''' <returns></returns>
        Public Shared Function Read2DArray(Of T)(fileId As H5FileId, dataSetName As String) As T(,)
            Try
                Dim dataset = H5D.open(fileId, dataSetName)
                Dim space = H5D.getSpace(dataset)
                Dim dims = H5S.getSimpleExtentDims(space)
                Dim dataType = H5D.[getType](dataset)
                ' this will also need a string hack...
                If GetType(T) = GetType(String) Then
                End If
                Dim dataArray As T(,) = New T(dims(0) - 1, dims(1) - 1) {}
                Dim wrapArray = New H5Array(Of T)(dataArray)
                H5D.read(dataset, dataType, wrapArray)
                Return dataArray
            Catch ex As Exception
                Return Nothing
            End Try

        End Function
        ''' <summary>
        ''' 'Return Nothing or Array
        ''' </summary>
        ''' <typeparam name="T"></typeparam>
        ''' <param name="fileId"></param>
        ''' <param name="dataSetName"></param>
        ''' <returns></returns>
        Public Shared Function Read1DArray(Of T)(fileId As H5FileId, dataSetName As String) As T()
            Try
                Dim dataset = H5D.open(fileId, dataSetName)
                Dim space = H5D.getSpace(dataset)
                Dim dims = H5S.getSimpleExtentDims(space)
                Dim dataType = H5D.[getType](dataset)
                If GetType(T) = GetType(String) Then
                    Dim stringLength As Integer = H5T.getSize(dataType)
                    Dim buffer As Byte() = New Byte(dims(0) * stringLength - 1) {}
                    H5D.read(dataset, dataType, New H5Array(Of Byte)(buffer))
                    Dim stuff As String = System.Text.ASCIIEncoding.ASCII.GetString(buffer)
                    '   Return stuff.SplitInParts(stringLength).[Select](Function(ss) DirectCast(DirectCast(ss, Object), T)).ToArray()
                End If
                Dim dataArray As T() = New T(dims(0) - 1) {}
                Dim wrapArray = New H5Array(Of T)(dataArray)
                H5D.read(dataset, dataType, wrapArray)
                Return dataArray
            Catch ex As Exception
                Return Nothing
            End Try

        End Function
    End Class
End Namespace

