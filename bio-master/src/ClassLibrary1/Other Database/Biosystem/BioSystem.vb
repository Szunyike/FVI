Imports System.IO

Imports Bio.IO.GenBank
Imports System.Text
Imports ClassLibrary1.Szunyi.Other_Database.CrossRefs

Namespace Szunyi
    Namespace Other_Database
        Namespace BioSytem
            Public Class Constants
                Public Const biosystems_biosystems_conserved As String = "biosystems_biosystems_conserved"
                Public Const biosystems_biosystems_linked As String = "biosystems_biosystems_linked"
                Public Const biosystems_biosystems_similar As String = "biosystems_biosystems_similar"
                Public Const biosystems_biosystems_specific As String = "biosystems_biosystems_specific"
                Public Const biosystems_biosystems_sub As String = "biosystems_biosystems_sub"
                Public Const biosystems_cdd_specific As String = "biosystems_cdd_specific"
                Public Const biosystems_gene As String = "biosystems_gene" ' Important
                Public Const biosystems_gene_all As String = "biosystems_gene_all"
                Public Const biosystems_pcsubstance As String = "biosystems_pcsubstance"
                Public Const biosystems_protein As String = "biosystems_protein"
                Public Const biosystems_protein_conserved As String = "biosystems_protein_conserved"
                Public Const biosystems_pubmed As String = "biosystems_pubmed"
                Public Const biosystems_taxonomy As String = "biosystems_taxonomy"

                Public Const bsid2infosed As String = "bsid2info.sed" ' Important
                Public Const BsIDsAndGeneIDs As String = "BsIDsAndGeneIDs"
                Public Const GeneIDsAndBsIDs As String = "GeneIDsAndBsIDs"
            End Class
            Public Class ThePathways
                Public Property Pathways As New List(Of Pathway)
                Public Property BsIDsAndGeneIds As List(Of CrossRefOneToMany)
                Public Property GeneIdsAndBsIDs As List(Of CrossRefOneToMany)
                Private PathwayBuilder As New BioSystemBuilder
                Public Sub New(FolderPath As String)


                    Me.BsIDsAndGeneIds = CrossRefBuilders.GetOneToManyFromFile(New FileInfo(FolderPath & Constants.BsIDsAndGeneIDs))
                    Me.GeneIdsAndBsIDs = CrossRefBuilders.GetOneToManyFromFile(New FileInfo(FolderPath & Constants.GeneIDsAndBsIDs))

                    Dim DiffGeneIDs = (From x In GeneIdsAndBsIDs Select x.One Distinct).ToArray
                    Me.Pathways.Add(New Pathway("All", -1))
                    Me.BsIDsAndGeneIds.Insert(0, New CrossRefOneToMany(-1, DiffGeneIDs))
                    For Each Line In Szunyi.IO.Import.Text.Parse(New FileInfo(FolderPath & Constants.bsid2infosed))
                        Me.Pathways.Add(New Pathway(Line))
                    Next
                    Me.Pathways.Sort(New PathWayBsIDComparer)
                End Sub
            End Class
            Public Class BioSystemBuilder
                Public Property Pathways As List(Of Pathway)
                Public Property BsIDsAndGeneIds As List(Of CrossRefOneToMany)
                Public Property GeneIdsAndBsIDs As List(Of CrossRefOneToMany)


                Private TheCrossRefBuilder As New CrossRefBuilders

                Public Sub CreateSubFromFullDatabase()

                    Dim OriginalDir = Szunyi.IO.Directory.Get_Folder("Select Directory Containing BiosystemFiles")
                    Dim DestinationPath = Szunyi.IO.Directory.Get_Folder("Select Destination Folder")
                    Dim OriginalFiles = OriginalDir.GetFiles
                    Dim TaxID = Szunyi.MyInputBox.GetInteger("Enter The Taxonomy ID")

                    Me.Pathways = GetOrganismSpecificPathways(New FileInfo(OriginalDir.FullName & "\" & BioSytem.Constants.bsid2infosed), TaxID)
                    SavePathways(DestinationPath.FullName)
                    Dim tmp = GetBsIdswithGeneIDs(New FileInfo(OriginalDir.FullName & "\" & BioSytem.Constants.biosystems_gene), TaxID)
                    Me.BsIDsAndGeneIds = TheCrossRefBuilder.GetOneToManyByFirst(tmp)
                    CrossRefBuilders.SaveOneToMany(DestinationPath.FullName, Constants.BsIDsAndGeneIDs, Me.BsIDsAndGeneIds)
                    Me.GeneIdsAndBsIDs = TheCrossRefBuilder.GetOneToManyBySecond(tmp)
                    CrossRefBuilders.SaveOneToMany(DestinationPath.FullName, Constants.GeneIDsAndBsIDs, Me.GeneIdsAndBsIDs)

                End Sub
                Private Sub SavePathways(FOlderPath As String)
                    Dim str As New StringBuilder
                    For Each Item In Me.Pathways
                        str.Append(Item.ToString).AppendLine()
                    Next
                    str.Length -= 2
                    Szunyi.IO.Export.SaveText(str.ToString, New FileInfo(FOlderPath &
                                                                              "\" & Constants.bsid2infosed))
                End Sub
                Private Function GetBsIdswithGeneIDs(File As FileInfo, taxID As Integer) As List(Of CrossRefOneToOne)
                    Dim tmp As New List(Of CrossRefOneToOne)
                    Dim BsIDs = (From x In Me.Pathways Select x.BsID).ToList
                    BsIDs.Sort()

                    For Each Line In Szunyi.IO.Import.Text.Parse(File)
                        Dim s1() As String = Split(Line, vbTab)
                        Dim Index = BsIDs.BinarySearch(s1(0))
                        If Index > -1 Then tmp.Add(New CrossRefOneToOne(s1(0), s1(1)))
                    Next
                    Return tmp

                End Function
                ''' <summary>
                ''' s1(6) Contains The TaxID
                ''' </summary>
                ''' <param name="File"></param>
                ''' <param name="TaxID"></param>
                ''' <returns></returns>
                Private Function GetOrganismSpecificPathways(File As FileInfo, TaxID As String) As List(Of Pathway)
                    Dim Out As New List(Of Pathway)
                    For Each Line In Szunyi.IO.Import.Text.Parse(File)
                        Dim s1() = Split(Line, vbTab)
                        If s1(6) = TaxID Then
                            Out.Add(New Pathway(s1))
                        End If

                    Next
                    Return Out
                End Function
            End Class

            Public Class Pathway
                Public Property TaxID As Long
                Public Property Source As String
                Public Property SourceID As String
                Public Property Description As String
                Public Property BsID As String
                Public Property DetailedDescription As String = ""
                Public Sub New(Line As String)
                    Dim s1() As String = Split(Line, vbTab)
                    Me.BsID = s1(0)
                    Me.Source = s1(1)
                    Me.SourceID = s1(2)
                    Me.Description = s1(3)
                    Me.TaxID = s1(4)
                    Me.DetailedDescription = s1(5)
                End Sub
                Public Sub New(Description As String, BsID As Integer)
                    Me.Description = Description
                    Me.BsID = BsID
                End Sub
                Public Sub New(s1() As String)
                    Me.BsID = s1(0)
                    Me.Source = s1(1)
                    Me.SourceID = s1(2)
                    Me.Description = s1(3)
                    Me.TaxID = s1(6)
                    If s1.Count > 7 Then Me.DetailedDescription = s1(7)
                End Sub
                Public Overrides Function ToString() As String
                    Return Me.BsID & vbTab & Me.Source & vbTab & Me.SourceID & vbTab & Me.Description & vbTab & Me.TaxID & vbTab & Me.DetailedDescription
                End Function
            End Class

            ' Based on BsID
            Public Class PathWayBsIDComparer
                Implements IComparer(Of Pathway)

                Public Function Compare(x As Pathway, y As Pathway) As Integer Implements IComparer(Of Pathway).Compare
                    Return x.BsID.CompareTo(y.BsID)
                End Function
            End Class
            Public Class PathWayNameComparer
                Implements IComparer(Of Pathway)

                Public Function Compare(x As Pathway, y As Pathway) As Integer Implements IComparer(Of Pathway).Compare
                    Return x.Description.CompareTo(y.Description)
                End Function
            End Class
        End Namespace
    End Namespace
End Namespace
