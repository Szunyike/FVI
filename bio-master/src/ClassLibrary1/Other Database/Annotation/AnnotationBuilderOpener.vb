Imports System.IO
Imports Bio.IO.GenBank
Imports ClassLibrary1.Szunyi.Features.FeatureManipulation
Imports ClassLibrary1.Szunyi.Other_Database.CrossRefs


Namespace Szunyi
    Namespace Other_Database
        Namespace Annotation
            Public Class TheAnnotation
                Public Property LocusTagsAndGeneIDs As New List(Of CrossRefOneToOne)
                Public Property GeneIDsAndLocusTags As New List(Of CrossRefOneToOne)
                Public Property ShortLocusTagAndGeneIDs As New List(Of CrossRefOneToOne)
                Public Property GeneIDsAndShortLocusTags As New List(Of CrossRefOneToOne)

                Public Property LocusTagsAndProducts As New List(Of CrossRefOneToOne)
                Public Property ShortLocusTagsAndProducts As New List(Of CrossRefOneToOne)

                Public Property GIAndShortLocusTag As New List(Of CrossRefOneToOne)
                Public Property GIAndLocusTag As New List(Of CrossRefOneToOne)
                Public Property LocusTagsAndGI As New List(Of CrossRefOneToOne)
                Public Property ShortLocusTagAndGI As New List(Of CrossRefOneToOne)

                Public Property ProductsAndLocusTags As New List(Of CrossRefOneToMany)
                Public Property ProductsAndShortLocusTags As New List(Of CrossRefOneToMany)
                Public Sub New(FolderPath As String)
                    Me.LocusTagsAndGeneIDs = CrossRefBuilders.GetOneToOneFromFile(New FileInfo(FolderPath & Constants.LocusTagsAndGeneIDS))

                    Me.GeneIDsAndLocusTags = CrossRefBuilders.GetOneToOneFromFile(New FileInfo(FolderPath & Constants.GeneIDsAndLocsTags))

                    Me.ShortLocusTagAndGeneIDs = CrossRefBuilders.GetOneToOneFromFile(New FileInfo(FolderPath & Constants.ShortLocusTagAndGeneIDs))
                    Me.GeneIDsAndShortLocusTags = CrossRefBuilders.GetOneToOneFromFile(New FileInfo(FolderPath & Constants.GeneIDsAndShortLocusTags))
                    Me.LocusTagsAndProducts = CrossRefBuilders.GetOneToOneFromFile(New FileInfo(FolderPath & Constants.LocusTagsAndProducts))
                    Me.ShortLocusTagsAndProducts = CrossRefBuilders.GetOneToOneFromFile(New FileInfo(FolderPath & Constants.ShortLocusTagsAndProducts))

                    Me.GIAndShortLocusTag = CrossRefBuilders.GetOneToOneFromFile(New FileInfo(FolderPath & Constants.GIAndShortLocusTag))
                    Me.GIAndLocusTag = CrossRefBuilders.GetOneToOneFromFile(New FileInfo(FolderPath & Constants.GIAndLocusTag))
                    Me.LocusTagsAndGI = CrossRefBuilders.GetOneToOneFromFile(New FileInfo(FolderPath & Constants.LocusTagsAndGI))
                    Me.ShortLocusTagAndGI = CrossRefBuilders.GetOneToOneFromFile(New FileInfo(FolderPath & Constants.ShortLocusTagAndGI))

                    Me.ProductsAndLocusTags = CrossRefBuilders.GetOneToManyFromFile(New FileInfo(FolderPath & Constants.ProductsAndLocusTags))
                    Me.ProductsAndShortLocusTags = CrossRefBuilders.GetOneToManyFromFile(New FileInfo(FolderPath & Constants.ProductsAndShortLocusTags))
                End Sub
            End Class
            Public Class Constants
                Public Const BsIDsAndGeneIDs As String = "BsIDsAndGeneIDs"
                Public Const GeneIDsAndBsIDs As String = "GeneIDsAndBsIDs"

                Public Const LocusTagsAndGeneIDS As String = "LocusTagsAndGeneIDS"
                Public Const GeneIDsAndLocsTags As String = "GeneIDSAndLocusTags"
                Public Const ShortLocusTagAndGeneIDs As String = "ShortLocusTagAndGeneIDs"
                Public Const GeneIDsAndShortLocusTags As String = "GeneIDsAndShortLocusTags"
                Public Const LocusTagsAndProducts As String = "LocusTagsAndProducts"
                Public Const ShortLocusTagsAndProducts As String = "ShortLocusTagsAndProducts"

                Public Const GIAndShortLocusTag As String = "GIAndShortLocusTag"
                Public Const GIAndLocusTag As String = "GIAndLocusTag"
                Public Const LocusTagsAndGI As String = "LocusTagsAndGI"
                Public Const ShortLocusTagAndGI As String = "ShortLocusTagAndGI"

                Public Const ProductsAndLocusTags As String = "ProductsAndLocusTags"
                Public Const ProductsAndShortLocusTags As String = "ProductsAndShortLocusTags"

            End Class
            Public Class AnnotationBuilderOpener

                Public Property LocusTagsAndGeneIDs As New List(Of CrossRefOneToOne)
                Public Property GeneIDsAndLocusTags As New List(Of CrossRefOneToOne)
                Public Property ShortLocusTagAndGeneIDs As New List(Of CrossRefOneToOne)
                Public Property GeneIDsAndShortLocusTags As New List(Of CrossRefOneToOne)
                Public Property LocusTagsAndProducts As New List(Of CrossRefOneToOne)
                Public Property ShortLocusTagsAndProducts As New List(Of CrossRefOneToOne)

                Public Property GIAndShortLocusTag As New List(Of CrossRefOneToOne)
                Public Property GIAndLocusTag As New List(Of CrossRefOneToOne)
                Public Property LocusTagsAndGI As New List(Of CrossRefOneToOne)
                Public Property ShortLocusTagAndGI As New List(Of CrossRefOneToOne)

                Public Property ProductsAndLocusTags As New List(Of CrossRefOneToMany)
                Public Property ProductsAndShortLocusTags As New List(Of CrossRefOneToMany)


                Private TheCrossRefBuilder As New CrossRefBuilders

                Public Sub CreateAnnotationForBiosystem(Seqs As List(Of Bio.ISequence))
                    ' For CDS Short
                    Dim CDSs = GetFeaturesByType.GetFeturesByTypeFromSeqs(Seqs, StandardFeatureKeys.CodingSequence)
                    Dim tmpLocusTagsAndGeneIDs As New List(Of CrossRefOneToOne)
                    Dim tmpLocusTagAndGI As New List(Of CrossRefOneToOne)

                    Dim s1() = Split(StandardQualifierNames.Product & " " & StandardQualifierNames.Note & " " & StandardQualifierNames.GeneSymbol)
                    For Each CDS In CDSs
                        Dim x = MergeFeatures.GetCorrespondingQulifierAndDbXref(CDS,
                                       StandardQualifierNames.LocusTag, StandardQualifierNames.DatabaseCrossReference, "GeneID:")
                        If IsNothing(x) = False Then tmpLocusTagsAndGeneIDs.Add(x)

                        Dim x2 = MergeFeatures.GetCorrespondingQulifierAndDbXref(CDS,
                                       StandardQualifierNames.LocusTag, StandardQualifierNames.DatabaseCrossReference, "GI:")
                        If IsNothing(x2) = False Then tmpLocusTagAndGI.Add(x2)

                        Dim x1 = MergeFeatures.GetCorrespondingQulifierAndDbXref(CDS, StandardQualifierNames.LocusTag, StandardQualifierNames.Product)
                        If IsNothing(x1) = False Then Me.LocusTagsAndProducts.Add(x1)

                    Next
                    'One To One
                    Me.LocusTagsAndGeneIDs = TheCrossRefBuilder.GetSortedOneToOne(True, tmpLocusTagsAndGeneIDs)
                    Me.GeneIDsAndLocusTags = TheCrossRefBuilder.GetSortedOneToOne(False, tmpLocusTagsAndGeneIDs)
                    Me.ShortLocusTagAndGeneIDs = TheCrossRefBuilder.GetwShortLocusTag(Me.LocusTagsAndGeneIDs, True)
                    Me.GeneIDsAndShortLocusTags = TheCrossRefBuilder.GetwShortLocusTag(Me.LocusTagsAndGeneIDs, False)

                    Me.LocusTagsAndGI = TheCrossRefBuilder.GetSortedOneToOne(True, tmpLocusTagAndGI)
                    Me.GIAndLocusTag = TheCrossRefBuilder.GetSortedOneToOne(False, tmpLocusTagAndGI)
                    Me.ShortLocusTagAndGI = TheCrossRefBuilder.GetwShortLocusTag(Me.LocusTagsAndGI, True)
                    Me.GIAndShortLocusTag = TheCrossRefBuilder.GetwShortLocusTag(Me.LocusTagsAndGI, False)


                    Me.LocusTagsAndProducts = TheCrossRefBuilder.GetwShortLocusTag(Me.LocusTagsAndProducts, True)
                    Me.ShortLocusTagsAndProducts = TheCrossRefBuilder.GetwShortLocusTag(Me.LocusTagsAndProducts, True)
                    ' One To Many
                    Me.ProductsAndLocusTags = TheCrossRefBuilder.GetOneToManyBySecond(Me.LocusTagsAndProducts)
                    Me.ProductsAndShortLocusTags = TheCrossRefBuilder.GetOneToManyBySecond(Me.ShortLocusTagsAndProducts)

                    Dim Folder = Szunyi.IO.Directory.Get_Folder
                    SaveAnnotation(Folder.FullName)
                End Sub
                Public Sub SaveAnnotation(FolderPath As String)
                    'One To One
                    If IsNothing(FolderPath) = True Then Exit Sub
                    CrossRefBuilders.SaveOneToOne(FolderPath & "\" & Constants.LocusTagsAndGeneIDS, Me.LocusTagsAndGeneIDs)
                    CrossRefBuilders.SaveOneToOne(FolderPath & "\" & Constants.GeneIDsAndLocsTags, Me.GeneIDsAndLocusTags)
                    CrossRefBuilders.SaveOneToOne(FolderPath & "\" & Constants.ShortLocusTagAndGeneIDs, Me.ShortLocusTagAndGeneIDs)
                    CrossRefBuilders.SaveOneToOne(FolderPath & "\" & Constants.GeneIDsAndShortLocusTags, Me.GeneIDsAndShortLocusTags)
                    CrossRefBuilders.SaveOneToOne(FolderPath & "\" & Constants.LocusTagsAndProducts, Me.LocusTagsAndProducts)
                    CrossRefBuilders.SaveOneToOne(FolderPath & "\" & Constants.ShortLocusTagsAndProducts, Me.ShortLocusTagsAndProducts)

                    CrossRefBuilders.SaveOneToOne(FolderPath & "\" & Constants.GIAndLocusTag, Me.GIAndLocusTag)
                    CrossRefBuilders.SaveOneToOne(FolderPath & "\" & Constants.GIAndShortLocusTag, Me.GIAndShortLocusTag)
                    CrossRefBuilders.SaveOneToOne(FolderPath & "\" & Constants.LocusTagsAndGI, Me.LocusTagsAndGI)
                    CrossRefBuilders.SaveOneToOne(FolderPath & "\" & Constants.ShortLocusTagAndGI, Me.ShortLocusTagAndGI)


                    'One To Many
                    TheCrossRefBuilder.SaveOneToMany(FolderPath & "\" & Constants.ProductsAndLocusTags, Me.ProductsAndLocusTags)
                    TheCrossRefBuilder.SaveOneToMany(FolderPath & "\" & Constants.ProductsAndShortLocusTags, Me.ProductsAndShortLocusTags)


                End Sub
                Public Function CreateAnnotationFromFiles(FolderPath As String) As TheAnnotation
                    Dim x As New TheAnnotation(FolderPath)
                    x.LocusTagsAndGeneIDs = CrossRefBuilders.GetOneToOneFromFile(New FileInfo(FolderPath & "\" & Constants.LocusTagsAndGeneIDS))
                    x.GeneIDsAndLocusTags = CrossRefBuilders.GetOneToOneFromFile(New FileInfo(FolderPath & "\" & Constants.GeneIDsAndLocsTags))
                    x.ShortLocusTagAndGeneIDs = CrossRefBuilders.GetOneToOneFromFile(New FileInfo(FolderPath & "\" & Constants.ShortLocusTagAndGeneIDs))
                    x.GeneIDsAndShortLocusTags = CrossRefBuilders.GetOneToOneFromFile(New FileInfo(FolderPath & "\" & Constants.GeneIDsAndShortLocusTags))
                    x.LocusTagsAndProducts = CrossRefBuilders.GetOneToOneFromFile(New FileInfo(FolderPath & "\" & Constants.LocusTagsAndProducts))
                    x.ShortLocusTagsAndProducts = CrossRefBuilders.GetOneToOneFromFile(New FileInfo(FolderPath & "\" & Constants.ShortLocusTagsAndProducts))

                    x.ProductsAndLocusTags = CrossRefBuilders.GetOneToManyFromFile(New FileInfo(FolderPath & "\" & Constants.ProductsAndLocusTags))
                    x.ProductsAndShortLocusTags = CrossRefBuilders.GetOneToManyFromFile(New FileInfo(FolderPath & "\" & Constants.ProductsAndShortLocusTags))
                    Return x
                End Function
            End Class
        End Namespace
    End Namespace
End Namespace


