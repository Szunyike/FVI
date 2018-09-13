Imports System.Windows.Forms.DataVisualization.Charting
Imports Szunyi_All

Public Class LongTranscripts
    Dim All_ORF_Names As New List(Of String)
    Dim Exps As New Dictionary(Of String, List(Of ORF))
    Dim Selected_Exp_Names As New List(Of String)
    Dim x As ClassLibrary1.ListManager
    Dim End_Distance As Integer
    Dim cFull = "Full"
    Dim cThree = "3 prime missing"
    Dim cFive = "5 prime missing"
    Dim cInner = "inner"
    Dim cMerged = "_Merged"
    Private Sub ImportToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles ImportToolStripMenuItem.Click
        Dim FIles = ClassLibrary1. Szunyi.IO.Files.Filter.SelectFiles("Select Files", ClassLibrary1.Szunyi.Constants.Files.All_TAB_Like)
        Me.All_ORF_Names.Clear()
        Me.Exps.Clear()
        Me.Selected_Exp_Names.Clear()
        If IsNothing(FIles) = True Then Exit Sub
        For Each FIle In FIles

            Dim ORFs As New List(Of ORF)
            For Each line In ClassLibrary1.Szunyi.IO.Import.Text.ParseToArray(FIle, vbTab, 0)
                If line(0) = String.Empty = False Then
                    ORFs.Add(New ORF(line))
                    If All_ORF_Names.Contains(ORFs.Last.Name) = False Then
                        All_ORF_Names.Add(ORFs.Last.Name)
                    End If
                Else
                    ORFs.Last.Reads.Add(New Read(line))
                End If
            Next
            Exps.Add(FIle.Name, ORFs)
        Next

        x.AddRange(All_ORF_Names)

        Dim tmp = From x In Exps Group By x.Key.Split("_").First Into Group

        For Each i In tmp
            If Exps.ContainsKey(i.First & cMerged) = False Then
                Exps.Add(i.First & cMerged, New List(Of ORF))
                For i1 = 0 To i.Group.Count - 1
                    If i1 = 0 Then
                        For Each Orf In i.Group(0).Value
                            Dim o As New ORF(Orf)
                            For Each read In Orf.Reads
                                o.Reads.Add(New Read(read))
                            Next
                            Exps(i.First & cMerged).Add(o)
                        Next

                    Else 'if not first just add
                        For i2 = 0 To i.Group(i1).Value.Count - 1
                            For Each read In i.Group(i1).Value(i2).Reads
                                Exps(i.First & cMerged)(i2).Reads.Add(New Read(read))
                            Next
                        Next
                    End If
                Next
            End If
        Next
    End Sub

    Private Sub ExperimentsToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles ExperimentsToolStripMenuItem.Click
        Dim NAmes = Exps.Keys.ToList
        Dim f1 As New CheckBoxForStringsFull(NAmes, -1, Nothing, Me.Selected_Exp_Names)
        If f1.ShowDialog = DialogResult.OK Then
            Me.Selected_Exp_Names = f1.SelectedStrings
        End If
    End Sub

    Private Sub LongTranscripts_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        x = New ClassLibrary1.ListManager
        x.Dock = DockStyle.Fill
        Me.SplitContainer1.Panel1.Controls.Add(x)

    End Sub

    Private Sub ViewToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles ViewToolStripMenuItem.Click

        Me.FlowLayoutPanel1.Controls.Clear()
        Dim SelExps = From x In Me.Exps Where Me.Selected_Exp_Names.Contains(x.Key)

        If SelExps.Count = 0 Then
            MsgBox("No Selected Expreiments")
            Exit Sub
        End If

        Dim Sel_ORFS = x.GetSelectedItems

        If Sel_ORFS.Count = 0 Then
            MsgBox("No Selected ORFs")
            Exit Sub
        End If

        Dim Labels As New List(Of String)
        Labels.Add(cFull)
        Labels.Add(cFive)
        Labels.Add(cThree)
        Labels.Add(cInner)
        Dim selEXPSII = SelExps.ToList
        selEXPSII.Sort(New SortBySpecial)
        For Each Orf In Sel_ORFS
            Dim ch As New Chart
            Dim Area As New System.Windows.Forms.DataVisualization.Charting.ChartArea(Orf.Name)
            Area.Position = New System.Windows.Forms.DataVisualization.Charting.ElementPosition(0, 0, 100, 100)
            Dim bORF = (From j In SelExps.First.Value Where j.Name = Orf.Name).First
            ch.Titles.Add(Orf.Name & ":" & bORF.Endy - bORF.Start)
            ch.ChartAreas.Add(Area)
            For Each Exp In selEXPSII
                Dim cORF = From k In Exp.Value Where k.Name = Orf.Name

                Dim Fu = From r In cORF.First.Reads Where r.Type = cFull

                Dim Fi = From r In cORF.First.Reads Where r.Type = cFive

                Dim T = From r In cORF.First.Reads Where r.Type = cThree

                Dim I = From r In cORF.First.Reads Where r.Type = cInner

                Dim Values As New List(Of Integer)

                Values.Add(Fu.Count)
                Values.Add(Fi.Count)
                Values.Add(T.Count)
                Values.Add(I.Count)
                Dim x1 As New Series
                x1.Name = Exp.Key
                x1.Points.DataBindXY(Labels, Values)
                ch.Series.Add(x1)
                x1.ChartType = SeriesChartType.Bar
            Next

            FlowLayoutPanel1.Controls.Add(ch)
        Next
    End Sub

    Private Sub EndDistanceToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles EndDistanceToolStripMenuItem.Click
        '    Dim inda As New ClassLibrary1.Controls.Get_Integer("Set End Distance", 20, 0, 5000)
        Me.End_Distance = ClassLibrary1.Szunyi.MyInputBox.GetInteger("Set End Distance")
        For Each Exp In Me.Exps
            For Each Orf In Exp.Value
                For Each Read In Orf.Reads
                    If Orf.Strand = "+" Then
                        If Orf.Start + Me.End_Distance > Read.Start Then
                            If Orf.Endy - Me.End_Distance < Read.Endy Then
                                Read.Type = cFull
                            Else
                                Read.Type = cThree
                            End If
                        Else
                            If Orf.Endy - Me.End_Distance < Read.Endy Then
                                Read.Type = cFive
                            Else
                                Read.Type = cInner
                            End If
                        End If
                    Else
                        If Orf.Start + Me.End_Distance > Read.Start Then
                            If Orf.Endy - Me.End_Distance < Read.Endy Then
                                Read.Type = cFull
                            Else
                                Read.Type = cFive
                            End If
                        Else
                            If Orf.Endy - Me.End_Distance < Read.Endy Then
                                Read.Type = cThree
                            Else
                                Read.Type = cInner
                            End If
                        End If
                    End If
                Next
            Next
        Next
    End Sub

    Private Sub ExportToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles ExportToolStripMenuItem.Click

        Dim SelExps = From x In Me.Exps Where Me.Selected_Exp_Names.Contains(x.Key)


        Dim selEXPSII = SelExps.ToList
        selEXPSII.Sort(New SortBySpecial)
        Dim str As New System.Text.StringBuilder
        str.Append("ORF Name")
        For Each Exp In selEXPSII
            str.Append(vbTab).Append(Exp.Key)
        Next
        str.AppendLine()
        For Each Orf In Me.All_ORF_Names
            Dim bORF = (From j In SelExps.First.Value Where j.Name = Orf).First
            str.Append(Orf)
            For Each Exp In selEXPSII
                Dim cORF = From k In Exp.Value Where k.Name = Orf
                str.Append(vbTab).Append(cORF.First.Reads.Count)
            Next
            str.AppendLine()

        Next
        Clipboard.SetText(str.ToString)
        ClassLibrary1.Szunyi.IO.Export.SaveText(str.ToString)
    End Sub
End Class
Public Class ORF
    Public Property Name As String
    Public Property Start As Integer
    Public Property Endy As Integer
    Public Property Strand As String
    Public Property Reads As New List(Of Read)
    Public Sub New(s1 As String())
        If s1.Length > 7 Then
            Me.Start = s1(3)
            Me.Endy = s1(4)
            Me.Strand = s1(6)
            Me.Name = s1(8)
        End If

    End Sub
    Public Sub New(orf As ORF)
        Me.Start = orf.Start
        Me.Endy = orf.Endy
        Me.Strand = orf.Strand
        Me.Name = orf.Name
    End Sub
End Class

Public Class Read
    Public Property Name As String
    Public Property Start As Integer
    Public Property Endy As Integer
    Public Property Strand As String
    Public Property Type As String
    Public Sub New(s1 As String())
        Me.Start = s1(2)
        Me.Endy = s1(3)
        Me.Strand = s1(4)
        Me.Name = s1(1)
    End Sub
    Public Sub New(read As Read)
        Me.Start = read.Start
        Me.Endy = read.Endy
        Me.Strand = read.Strand
        Me.Name = read.Name
    End Sub
End Class
Public Class SortBySpecial
    Implements IComparer(Of KeyValuePair(Of String, List(Of ORF)))

    Public Function Compare(x As KeyValuePair(Of String, List(Of ORF)), y As KeyValuePair(Of String, List(Of ORF))) As Integer Implements IComparer(Of KeyValuePair(Of String, List(Of ORF))).Compare
        Dim x1 = getNumeric(Split(x.Key, "_").First)
        Dim y1 = getNumeric(Split(y.Key, "_").First)
        Return x1.CompareTo(y1)
    End Function

    Public Function getNumeric(value As String) As Integer
        Dim output As New System.Text.StringBuilder
        For i = 0 To value.Length - 1
            If IsNumeric(value(i)) Then
                output.Append(value(i))
            End If
        Next
        Return output.ToString()
    End Function
End Class