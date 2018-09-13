
Imports System.IO
Imports System.Windows.Forms
Imports System.Xml.Serialization

Namespace Szunyi
    Namespace Phylogenetic
        ''' <summary>
        ''' This class modifies the node and edge color depends on tab file wNodeName and color code like 255 255 0 or 255 123 34
        ''' </summary>
        ''' <remarks></remarks>
        Public Class NewXml
            Public Property Files As New List(Of FileInfo)
            Public Property FastaFiles As New List(Of FileInfo)
            Public Property NeXmLFiles As New List(Of FileInfo)
            Public Property ColorSchemaFIles As New List(Of FileInfo)
            Public Property Path As DirectoryInfo
            Public Property SeqIDs As New List(Of String)
            Public Property AllNodes As List(Of Node)
            Public Property EndNodes As List(Of Node)
            Public Property ColorSchemas As List(Of ColorSchema)
            Private Sub SetLabelName(Nodes As List(Of Node), File As FileInfo)
                Dim FastaFiles = From x In Me.FastaFiles Where x.Name.Contains(Szunyi.IO.Files.Get_New_FileName.GetFileName_woExtension(File))

                If FastaFiles.Count > 0 Then
                    Me.SeqIDs = Szunyi.Sequences.SequenceManipulation.Common.GetSeqIDs(FastaFiles.ToList)
                Else
                    Me.SeqIDs.Clear()
                    Exit Sub
                End If
                Dim log As New System.Text.StringBuilder
                For Each N In Nodes
                    Dim t = From x In SeqIDs Where x.StartsWith(N.Label)

                    If t.Count = 1 Then
                        N.Label = t.First
                    Else
                        log.Append(N.Label).Append(vbTab).Append(t.Count).AppendLine()
                    End If
                Next
            End Sub
            Private Function LoadColorSchemas() As List(Of ColorSchema)
                Dim out As New List(Of ColorSchema)
                Dim mySerializer As XmlSerializer = New XmlSerializer(GetType(ColorSchema))
                For Each File In Me.ColorSchemaFIles
                    Dim Stream As New StreamReader(File.FullName)
                    Dim x = mySerializer.Deserialize(Stream)
                    out.Add(x)
                Next
                Return out
            End Function
            Public Sub CreateColorSchemas()
                AllNodes = GetAllNodes(NeXmLFiles.First)
                If IsNothing(AllNodes) = True Then Exit Sub
                EndNodes = (From x In AllNodes Where x.Label <> "").ToList
                SetLabelName(EndNodes, NeXmLFiles.First)

            End Sub
            Private Sub CreateRTF(x As ColorSchema)
                Dim x1 As New RichTextBox
                For Each ColorGroup In x.ColorGroups
                    x1.Text = x1.Text & ColorGroup.Name & vbCrLf

                Next
                For Each ColorGroup In x.ColorGroups
                    x1.Find(ColorGroup.Name)

                    x1.SelectionColor = ColorGroup.Color.Color
                Next
                x1.SaveFile(Me.Path.FullName & "\" & x.Name & ".rtf")

            End Sub
            Public Sub DoIt()
                Me.Path = Szunyi.IO.Directory.Get_Folder
                Me.ColorSchemas = LoadColorSchemas()
                For Each ColorSchema In Me.ColorSchemas
                    CreateRTF(ColorSchema)
                    For Each File In NeXmLFiles
                        AllNodes = GetAllNodes(File)
                        EndNodes = (From x In AllNodes Where x.Label <> "").ToList
                        SetLabelName(EndNodes, File)
                        SetColors(EndNodes, ColorSchema)
                        Dim AllEdges = GetAllEdges(File)
                        Dim UsedNodes As New List(Of String)
                        Dim UsedEdges As New List(Of String)


                        Do
                            Dim Count As Integer = 0
                            For Each LabelledNode In AllNodes

                                If UsedNodes.Contains(LabelledNode.ID) = False Then ' Not used it yet
                                    Dim ParentEdge = GetParentEdge(LabelledNode, AllNodes, AllEdges)
                                    If IsNothing(ParentEdge) = False Then ' Not Root then
                                        ParentEdge.Color = LabelledNode.Color
                                        Dim ParentNode = GetNodeByID(AllNodes, ParentEdge.Source)
                                        Dim SisterEdges = GetSisterEdge(LabelledNode, AllNodes, AllEdges, ParentEdge, ParentNode)
                                        Dim SisterNodes As New List(Of Node)
                                        For Each SisterEdge In SisterEdges
                                            SisterNodes.Add(GetNodeByID(AllNodes, SisterEdge.Target))
                                        Next
                                        SisterNodes.Add(LabelledNode)
                                        If HasAllColor(SisterNodes) = True Then 'Return True If All Nodes has already a color
                                            If UsedNodes.Contains(LabelledNode.ID) = False Then UsedNodes.Add(LabelledNode.ID)

                                            If IsSameColor(SisterNodes) = True Then
                                                ParentNode.Color = SisterNodes.First.Color
                                            Else
                                                ParentNode.Color = "No Color"
                                            End If
                                        Else
                                            Dim alf As Int16 = 54
                                        End If

                                    End If

                                End If

                            Next
                        Loop Until UsedNodes.Count = AllNodes.Count - 1

                        CreateFile(AllNodes, AllEdges, File, ColorSchema)
                    Next
                Next

            End Sub
            Public Sub New()
                Me.Files = IO.Files.Filter.SelectFiles("Select  Files")
                If IsNothing(Me.Files) = True Then Exit Sub
                Dim t = From x In Files Where x.Extension = Szunyi.Constants.Files.PhylogeneticXml

                If t.Count > 0 Then
                    Me.NeXmLFiles = t.ToList
                End If

                Dim f = From x In Files Where Szunyi.Constants.Files.SequenceFileTypesToImport.Contains(x.Extension)
                If f.Count > 0 Then Me.FastaFiles = f.ToList

                Dim f1 = From x In Files Where x.Extension = ".xml"
                If f1.Count > 0 Then
                    Me.ColorSchemaFIles = f1.ToList
                End If
            End Sub

            ''' <summary>
            ''' Return All Nodes or Nothing if error happened
            ''' </summary>
            ''' <param name="File"></param>
            ''' <returns></returns>
            Private Function GetAllNodes(File As System.IO.FileInfo) As List(Of Node)
                Dim Lines = IO.Import.Text.ReadLines(File, Nothing)
                If IsNothing(Lines) = True Then Return Nothing
                Dim Nodes As New List(Of Node)
                For i1 = 0 To Lines.Count - 1
                    If Lines(i1).StartsWith("      <node") Then
                        Nodes.Add(New Node(Lines(i1), Lines(i1 + 1)))
                    End If
                Next
                Return Nodes
            End Function

            Private Function GetAllEdges(File As System.IO.FileInfo) As List(Of Edge)
                Dim Lines = IO.Import.Text.ReadLines(File, Nothing)
                Dim Edges As New List(Of Edge)
                For i1 = 0 To Lines.Count - 1
                    If Lines(i1).StartsWith("      <edge") Then
                        Edges.Add(New Edge(Lines(i1), Lines(i1 + 1)))
                    End If
                Next
                Return Edges
            End Function

            Private Function GetParentEdge(LabelledNode As Node, AllNodes As List(Of Node), AllEdges As List(Of Edge)) As Edge
                Dim res = From x In AllEdges Where x.Target = LabelledNode.ID

                Select Case res.Count
                    Case 0
                        Dim alf As Int16 = 54 ' root
                    Case 1
                        Return res.First
                    Case Else
                        Dim alf As Int16 = 43
                End Select
                Return Nothing
            End Function

            Private Function GetParentNode(LabelledNode As Node, AllNodes As List(Of Node), AllEdges As List(Of Edge), ParentEdge As Edge) As Object
                Dim r = From x In AllNodes Where x.ID = ParentEdge.Source

                If r.Count > 1 Then
                    Dim alf As Int16 = 54
                End If
                Return r.First

            End Function

            Private Function GetSisterEdge(LabelledNode As Node, AllNodes As List(Of Node), AllEdges As List(Of Edge),
                                           ParentEdge As Edge, ParentNode As Node) As List(Of Edge)
                Dim res = From x In AllEdges Where x.Source = ParentNode.ID

                Select Case res.Count

                    Case 1
                        Dim alf As Int16 = 54
                    Case 2

                    Case Else
                        Dim alf As Int16 = 54

                End Select
                Dim SimEdges As New List(Of Edge)
                For Each Item In res
                    If Item.Target <> LabelledNode.ID Then
                        SimEdges.Add(Item)
                    End If

                Next
                Return SimEdges
                Return Nothing

            End Function

            Private Function GetNodeByID(AllNodes As List(Of Node), ID As String) As Node
                Dim r = From x In AllNodes Where x.ID = ID

                Return r.First

            End Function

            Private Function GetEdgeByID(AllEdges As List(Of Edge), ID As String) As Edge
                Dim r = From x In AllEdges Where x.ID = ID

                Return r.First
            End Function

            Private Function IsSameColor(SisterNodes As List(Of Node)) As Boolean
                For i1 = 0 To SisterNodes.Count - 1
                    If SisterNodes(i1).Color <> SisterNodes(0).Color Then Return False
                Next
                Return True
            End Function

            Private Function HasAllColor(SisterNodes As List(Of Node)) As Boolean
                For Each Item In SisterNodes
                    If IsNothing(Item.Color) = True Then Return Nothing
                Next
                If IsNothing(SisterNodes.First.Color) = True Then
                    Return False
                End If

                Return True
            End Function

            Private Sub CreateFile(AllNodes As List(Of Node),
                                   AllEdges As List(Of Edge),
                                   File As System.IO.FileInfo,
                                   TheSchema As ColorSchema)
                Dim Lines = IO.Import.Text.ReadLines(File, Nothing)
                For i1 = 0 To Lines.Count - 1
                    If Lines(i1).StartsWith("      <node") Then
                        Dim x = New Node(Lines(i1), Lines(i1 + 1))
                        Dim hr = GetNodeByID(AllNodes, x.ID)
                        Dim s = Lines(i1 + 1)
                        If hr.Color <> "No Color" Then
                            Dim out2 As New System.Text.StringBuilder
                            out2.Append("        <meta content=" & Chr(34) & " fg=")
                            out2.Append(hr.Color)
                            Dim s1 = Split(Lines(i1 + 1), "        <meta content=")
                            If s1.Count > 1 Then
                                Dim wOutChr34 = s1(1).Trim(Chr(34))
                                out2.Append(wOutChr34)
                            Else
                                Dim wOutChr34 = s1(0).Trim(Chr(34))
                                out2.Append(wOutChr34)
                            End If


                            Lines(i1 + 1) = out2.ToString
                                Lines(i1 + 1) = Lines(i1 + 1).Replace(" " & Chr(34) & " ", " ")
                                Dim alf As Int16 = 54

                        End If

                    ElseIf Lines(i1).StartsWith("      <edge") Then
                        Dim x = New Edge(Lines(i1), Lines(i1 + 1))
                        Dim hr = GetEdgeByID(AllEdges, x.ID)

                        Dim s = Lines(i1 + 1)
                        If IsNothing(hr.Color) = False AndAlso hr.Color <> "No Color" Then
                            Dim out2 As New System.Text.StringBuilder
                            out2.Append("        <meta content=" & Chr(34) & " fg=")
                            out2.Append(hr.Color)
                            Dim s1 = Split(Lines(i1 + 1), "        <meta content=")
                            If s1.Count > 1 Then
                                Dim wOutChr34 = s1(1).Trim(Chr(34))
                                out2.Append(wOutChr34)
                            Else
                                Dim wOutChr34 = s1(0).Trim(Chr(34))
                                out2.Append(wOutChr34)
                            End If

                            Lines(i1 + 1) = out2.ToString
                            Lines(i1 + 1) = Lines(i1 + 1).Replace(" " & Chr(34) & " ", " ")
                            Dim alf As Int16 = 54
                        End If
                    End If
                Next
                Dim Txt = Szunyi.Text.General.GetText(Lines, vbCrLf)
                Dim NewFile As New FileInfo(Me.Path.FullName & "\" & TheSchema.Name & " " & File.Name)
                Szunyi.IO.Export.SaveText(Txt, NewFile)
            End Sub
            Private Sub SetColors(ByRef Nodes As List(Of Node), TheSchema As ColorSchema)
                For Each Node In Nodes
                    Node.Color = GetColor(Node, TheSchema)
                Next
            End Sub

            Private Function GetColor(node As Node, theSchema As ColorSchema) As String
                Dim s = Split(node.Label, "|")
                For Each ColorGroup In theSchema.ColorGroups
                    Dim Found As Boolean = True
                    For Each BasicColor In ColorGroup.Items
                        If s(BasicColor.Index) <> BasicColor.Name Then
                            Found = False
                        End If
                    Next
                    If Found = False Then
                        Return " 0 0 0 " ' This is Black
                    Else
                        With ColorGroup.Color.Color
                            Return " " & .R & " " & .G & " " & .B
                        End With

                    End If
                Next
                Return ""
            End Function

            Private Sub SetColors(Nodes As List(Of Node))
                Dim s1() = Split(Nodes.First.Label, "|")
                Dim Vals(s1.Count) As List(Of String)
                For Each Node In Nodes
                    Dim s() = Split(Node.Label, "|")
                    For i1 = 1 To s.Count - 1
                        If IsNothing(Vals(i1)) = True Then Vals(i1) = New List(Of String)
                        If Vals(i1).Contains(s(i1)) = False Then Vals(i1).Add(s(i1))
                    Next
                Next

                '  Dim x As New PhyloGeneticClorPicker(Vals)
                ' x.ShowDialog()
            End Sub

        End Class

        Public Class Node

            Public Property Label As String = ""
            Public Property Metadata As String
            Public Property ID As String
            Public Property Color As String

            Sub New(NodeString As String, MetaData As String)

                Me.Metadata = MetaData
                Dim s1() = Split(NodeString, " ")
                For Each s In s1
                    If s.StartsWith("id") Then
                        Dim s2() = Split(s, Chr(34))
                        Me.ID = s2(1)
                    ElseIf s.StartsWith("label") Then
                        Dim s2() = Split(s, Chr(34))
                        Me.Label = s2(1)
                        SetColor(Me.Label)
                    End If
                Next
            End Sub
            Private Sub SetColor(Label As String)
                If Label.StartsWith("M") Then
                    Me.Color = " 0 0 255 "
                ElseIf Label.StartsWith("N4") Then
                    Me.Color = " 0 255 0 "
                ElseIf Label.StartsWith("N5") Then
                    Me.Color = " 0 255 0 "
                ElseIf Label.StartsWith("N6") Then
                    Me.Color = " 255 0 0 "
                ElseIf Label.StartsWith("N7") Then
                    Me.Color = " 255 0 0 "
                ElseIf Label.StartsWith("D") Then
                    Me.Color = " 255 255 0 "
                End If
            End Sub
            Public Sub New()

            End Sub
        End Class
        Public Class Edge
            Public Property ID As String
            Public Property Target As String
            Public Property Source As String
            Public Property MetaData As String
            Public Property Color As String

            Public Sub New(EdgeString As String, MetaData As String)
                Dim s1 = Split(EdgeString, " ")
                Me.MetaData = MetaData
                For Each s In s1
                    If s.StartsWith("id") Then
                        Dim s2() = Split(s, Chr(34))
                        Me.ID = s2(1)
                    ElseIf s.StartsWith("source") Then
                        Dim s2() = Split(s, Chr(34))
                        Me.Source = s2(1)
                    ElseIf s.StartsWith("target") Then
                        Dim s2() = Split(s, Chr(34))
                        Me.Target = s2(1)
                    End If
                Next
            End Sub
        End Class
    End Namespace
End Namespace