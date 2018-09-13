Imports System.IO
Imports Bio.IO.GenBank
Namespace Szunyi
    Namespace Venn
        Public Class R_Venn
            Public Shared Property Def_Dir As New DirectoryInfo("C:\R\")
            Public Shared Sub Test()
                Dim str As New System.Text.StringBuilder
                str.Append("library(VennDiagram);")
                str.AppendLine()
                str.Append("Venn.plot <- draw.quad.venn( area1 = 72, area2 = 86, area3 = 50, area4 = 52, n12 = 44, n13 = 27, n14 = 32,n23 = 38, n24 = 32, n34 = 20, n123 = 18,n124 = 17, n134 = 11,n234 = 13, n1234 = 6 ")
                str.Append(", category = c(")
                Dim Names = Split("A,B,C,D", ",")
                Dim NamesII = Szunyi.Text.General.Add_Before_And_End(Names.ToList, Chr(34))
                Dim Color = Split("orange,red,green,blue", ",")
                Dim ColorII = Szunyi.Text.General.Add_Before_And_End(Color.ToList, Chr(34))
                str.Append(Szunyi.Text.General.Insert(NamesII, ","))
                str.Append("),  fill = c(")
                str.Append(Szunyi.Text.General.Insert(ColorII, ","))
                str.Append(") ")
                str.Append("tiff(filename = " & Chr(34) & "C:/R/Quad_Venn_diagram13.tiff" & Chr(34) & ")").AppendLine()
                str.Append("grid.draw(Venn.plot); ").AppendLine()
                str.Append("dev.off()").AppendLine()
                Szunyi.IO.Export.SaveText(str.ToString, New FileInfo(Def_Dir.FullName & "test.r"))
                '      Dim Engine = RDotNet.REngine.CreateInstance("test2")
                ''      Engine.Evaluate(str.ToString)

            End Sub
        End Class
        Public Class Venn_Table
            Public Property Genes_By_Experimets As New List(Of GeneIDsAndExperiments)
            Public Property Experimtens_With_GeneIDs As New List(Of ExperimentsAndGeneIDs)

            Public Property cGenes_By_Experimets As New List(Of GeneIDsAndExperiments)
            Public Property cExperimtens_With_GeneIDs As New List(Of ExperimentsAndGeneIDs)

            Public Property Unique_GeneIDs As New List(Of String)
            Public GeneIDs(0, 0) As List(Of String)

            Public Sub New(Files As List(Of FileInfo))
                Dim AllGeneID As New List(Of String)


                For Each File In Files
                    Dim I_P_W As New Szunyi.Text.TableManipulation.Items_With_Properties(File)
                    I_P_W.DoIt(True)
                    Dim IDs = I_P_W.Get_Keys
                    AllGeneID.AddRange(IDs)
                    Dim x As New ExperimentsAndGeneIDs(File.Name, IDs)
                    Me.Experimtens_With_GeneIDs.Add(x)

                Next
                Dim tmp1 = From x In Me.Experimtens_With_GeneIDs Order By x.GeneIDs.Count
                Me.Experimtens_With_GeneIDs = tmp1.ToList

                Unique_GeneIDs = AllGeneID.Distinct.ToList
                Unique_GeneIDs.Sort()
                For Each uGeneID In Unique_GeneIDs
                    Genes_By_Experimets.Add(New GeneIDsAndExperiments(uGeneID))
                Next
                Dim com As New GenewExp_Comp
                For Each exp In Me.Experimtens_With_GeneIDs
                    For Each GeneID In exp.GeneIDs
                        Dim tmp As New GeneIDsAndExperiments(GeneID)
                        Dim Index = Me.Genes_By_Experimets.BinarySearch(tmp, com)
                        Me.Genes_By_Experimets(Index).Experiments.Add(exp.Experiment)
                    Next
                Next
                Dim res = From x In Me.Genes_By_Experimets Order By x.Experiments.Count Descending

                Me.Genes_By_Experimets = res.ToList

                Dim sg = Files.First.Directory.GetFiles("ShortLocusTagsAndProducts")
                Dim co As New Szunyi.Other_Database.CrossRefs.ComparerOfCrossrefOneToOneByFirst
                If sg.Count = 1 Then
                    Dim l = Szunyi.Other_Database.CrossRefs.CrossRefBuilders.GetOneToOneFromFile(sg.First)
                    l.Sort(co)
                    Dim tmpC As New Szunyi.Other_Database.CrossRefs.CrossRefOneToOne("a", "a")
                    For Each geneID In Me.Genes_By_Experimets
                        tmpC.First = geneID.GeneID
                        Dim Index = l.BinarySearch(tmpC, co)
                        If Index > -1 Then
                            geneID.GeneID = geneID.GeneID & " " & l(Index).Second
                        End If
                    Next
                    For Each exp In Me.Experimtens_With_GeneIDs
                        For i1 = 0 To exp.GeneIDs.Count - 1
                            tmpC.First = exp.GeneIDs(i1)
                            Dim Index = l.BinarySearch(tmpC, co)
                            If Index > -1 Then
                                exp.GeneIDs(i1) = exp.GeneIDs(i1) & " " & l(Index).Second
                            End If
                        Next

                    Next
                End If
                For Each Item In Me.Experimtens_With_GeneIDs
                    Me.cExperimtens_With_GeneIDs.Add(New ExperimentsAndGeneIDs(Item.Experiment, Item.GeneIDs))

                Next
                For Each Item In Me.Genes_By_Experimets
                    Me.cGenes_By_Experimets.Add(New GeneIDsAndExperiments(Item.GeneID, Item.Experiments))
                Next

            End Sub
            Public Sub New(x As Dictionary(Of String, String))

            End Sub
            Public Sub New(Seqs As List(Of Szunyi.ListOf.SequenceList))

            End Sub
            Public Function Get_Exp_Dt()
                Dim tmp1 = From x In Me.cExperimtens_With_GeneIDs Order By x.GeneIDs.Count
                Me.cExperimtens_With_GeneIDs = tmp1.ToList
                Dim dt As New DataTable
                dt.Columns.Add("Exp_Names")
                For Each Exp In Me.cExperimtens_With_GeneIDs
                    Dim col As New DataColumn(Exp.Experiment, System.Type.GetType("System.Int32"))

                    dt.Columns.Add(col)
                Next
                ReDim GeneIDs(Me.cExperimtens_With_GeneIDs.Count - 1, Me.cExperimtens_With_GeneIDs.Count)
                For i1 = 0 To Me.cExperimtens_With_GeneIDs.Count - 1
                    Dim row As DataRow = dt.NewRow
                    row.Item(0) = Me.cExperimtens_With_GeneIDs(i1).Experiment
                    For i2 = 0 To Me.cExperimtens_With_GeneIDs.Count - 1
                        Dim x As New GeneID(Me.cExperimtens_With_GeneIDs(i1).GeneIDs.Intersect(Me.cExperimtens_With_GeneIDs(i2).GeneIDs))
                        GeneIDs(i1, i2) = x.GeneIDs
                        row.Item(i2 + 1) = x.GeneIDs.Count
                    Next
                    dt.Rows.Add(row)
                Next
                Return dt
            End Function
            Public Function Get_Gene_Dt(ExpWGeneID As List(Of ExperimentsAndGeneIDs), GegeIDwExp As List(Of GeneIDsAndExperiments))
                Dim dt As New DataTable
                dt.Columns.Add("Gene")
                For Each Exp In ExpWGeneID
                    dt.Columns.Add(Exp.Experiment)
                Next

                For Each Item In GegeIDwExp
                    Dim row As DataRow = dt.NewRow
                    row.Item(0) = Item.GeneID
                    For Each exp In Item.Experiments
                        row.Item(exp) = "+"
                    Next
                    dt.Rows.Add(row)
                Next
                Return dt
            End Function

            Public Sub Filter(text As String)
                Dim tmp As New List(Of GeneIDsAndExperiments)
                For Each Item In Me.Genes_By_Experimets
                    If Item.GeneID.Contains(text) = True Then
                        Dim k As New GeneIDsAndExperiments(Item.GeneID)
                        For Each exp In Item.Experiments
                            k.Experiments.Add(exp)
                        Next
                        tmp.Add(k)
                    End If
                Next
                Me.cGenes_By_Experimets = tmp
                Dim x As New List(Of ExperimentsAndGeneIDs)
                For Each Item In Me.Experimtens_With_GeneIDs
                    x.Add(New ExperimentsAndGeneIDs(Item.Experiment))
                Next
                For Each gene In Me.cGenes_By_Experimets
                    For Each expe In gene.Experiments
                        Dim s = (From h In x Where h.Experiment = expe).First

                        s.GeneIDs.Add(gene.GeneID)
                    Next
                Next
                Me.cExperimtens_With_GeneIDs = x
            End Sub
        End Class
        Public Class ExperimentsAndGeneIDs
            Public Sub New(exp As String)
                Me.Experiment = exp
            End Sub
            Public Sub New(name As String, GeneIDs As List(Of String))
                Me.Experiment = name
                Me.GeneIDs = GeneIDs
            End Sub

            Public Property Experiment As String
            Public Property GeneIDs As New List(Of String)
        End Class
        Public Class GeneIDsAndExperiments
            Public Property GeneID As String
            Public Property Experiments As New List(Of String)
            Public Sub New(GeneID As String)
                Me.GeneID = GeneID
            End Sub
            Public Sub New(GeneID As String, Exps As List(Of String))
                Me.GeneID = GeneID
                For Each Item In Exps
                    Me.Experiments.Add(Item)
                Next
            End Sub
        End Class
        Public Class GeneID
            Private enumerable As IEnumerable(Of String)

            Public Sub New(enumerable As IEnumerable(Of String))
                Me.GeneIDs = enumerable.ToList
            End Sub

            Public Property GeneIDs As List(Of String)
            Public Overrides Function ToString() As String
                Return GeneIDs.Count
            End Function
        End Class

        Public Class GenewExp_Comp
            Implements IComparer(Of GeneIDsAndExperiments)

            Public Function Compare(x As GeneIDsAndExperiments, y As GeneIDsAndExperiments) As Integer Implements IComparer(Of GeneIDsAndExperiments).Compare
                Return x.GeneID.CompareTo(y.GeneID)
            End Function



        End Class
        Public Class Venn_Location
            ''' <summary>
            ''' Only FIrst, Only Second, Common
            ''' </summary>
            ''' <param name="first"></param>
            ''' <param name="second"></param>
            ''' <returns></returns>
            Public Shared Function FirstContains(first As List(Of ILocation), second As List(Of ILocation)) As List(Of List(Of ILocation))
                Dim OnlyFirst As New List(Of Bio.IO.GenBank.ILocation)
                Dim Common As New List(Of Bio.IO.GenBank.ILocation)

                Dim OnlySeconds As New List(Of Bio.IO.GenBank.ILocation)
                Dim used As New List(Of Bio.IO.GenBank.ILocation)
                For Each Item In first
                    Dim x1 = From x In second Where x.LocationStart >= Item.LocationStart And x.LocationEnd <= Item.LocationEnd And x.IsComplementer = Item.IsComplementer
                    If x1.Count = 0 Then
                        OnlyFirst.Add(Item)
                    Else
                        Common.Add(Item)
                        used.AddRange(x1.ToList)
                    End If
                Next
                OnlySeconds = second.Except(used).ToList
                Dim CommonII = used.Distinct.ToList
                Dim out As New List(Of List(Of ILocation))
                out.Add(OnlyFirst)
                out.Add(OnlySeconds)
                out.Add(CommonII)
                Return out
            End Function

            ''' <summary>
            ''' Only FIrst, Only Second, Common
            ''' </summary>
            ''' <param name="first"></param>
            ''' <param name="second"></param>
            ''' <returns></returns>
            Public Shared Function FirstContains(first As List(Of Szunyi.Location.Basic_Location), second As List(Of Szunyi.Location.Basic_Location)) As List(Of List(Of Szunyi.Location.Basic_Location))
                Dim OnlyFirst As New List(Of Szunyi.Location.Basic_Location)
                Dim Common As New List(Of Szunyi.Location.Basic_Location)

                Dim OnlySeconds As New List(Of Szunyi.Location.Basic_Location)
                Dim used As New List(Of Szunyi.Location.Basic_Location)
                For Each Item In first
                    Dim x1 = From x In second Where x.Location.LocationStart >= Item.Location.LocationStart And x.Location.LocationEnd <= Item.Location.LocationEnd And
                                                  x.Location.IsComplementer = Item.Location.IsComplementer
                    If x1.Count = 0 Then
                        OnlyFirst.Add(Item)
                    Else
                        Common.Add(Item)
                        used.AddRange(x1.ToList)
                    End If
                Next
                OnlySeconds = second.Except(used).ToList
                Dim CommonII = used.Distinct.ToList
                Dim out As New List(Of List(Of Szunyi.Location.Basic_Location))
                out.Add(OnlyFirst)
                out.Add(OnlySeconds)
                out.Add(CommonII)
                Return out
            End Function

        End Class
    End Namespace

End Namespace

