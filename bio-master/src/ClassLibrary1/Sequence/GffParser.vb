Imports System.Drawing
Imports System.IO
Imports Bio.IO.GenBank
Imports ClassLibrary1
Imports ClassLibrary1.Szunyi.ListOf
Imports System.Text

Namespace Szunyi
    Namespace Sequences
        Namespace Gff
            Public Class GFF_Parser_Shared
                Public Shared Function Get_Annotation(File As FileInfo) As List(Of Annotation)
                    Dim res As New List(Of Annotation)
                    Using sr As New StreamReader(File.FullName)
                        Do
                            Dim Line As String = sr.ReadLine
                            If Line.StartsWith("#") = False Then ' Else This is a comment and forget it!
                                Dim x As New Annotation(Line)
                                Res.Add(x)
                            End If
                        Loop Until sr.EndOfStream = True
                    End Using
                    Return res
                End Function
                Public Shared Function Get_Locations(Annotations As List(Of Annotation)) As List(Of Bio.IO.GenBank.ILocation)
                    Dim out As New List(Of Bio.IO.GenBank.ILocation)
                    For Each item In Annotations
                        If item.IsComplementer = True Then
                            out.Add(Szunyi.Location.Common.GetLocation(item.Start, item.Endy, "-"))
                        Else
                            out.Add(Szunyi.Location.Common.GetLocation(item.Start, item.Endy, "+"))
                        End If
                        out.Last.Accession = item.ID
                    Next
                    Return out
                End Function
                Public Shared Function Get_Basic_Location(Annotations As List(Of Annotation)) As List(Of Szunyi.Location.Basic_Location)
                    Dim out As New List(Of Szunyi.Location.Basic_Location)
                    For Each item In Annotations
                        If item.IsComplementer = True Then
                            out.Add(Szunyi.Location.Common.Get_Basic_Location(item.Start, item.Endy, "-"))
                        Else
                            out.Add(Szunyi.Location.Common.Get_Basic_Location(item.Start, item.Endy, "+"))
                        End If
                        out.Last.Location.Accession = item.ID
                        out.Last.Obj = item
                    Next
                    Return out
                End Function
                Public Shared Function Get_Basic_Locations(File As FileInfo) As List(Of Szunyi.Location.Basic_Location)
                    Dim f1 = Szunyi.Sequences.Gff.GFF_Parser_Shared.Get_Annotation(File)
                    Dim f2 = Szunyi.Sequences.Gff.GFF_Parser_Shared.Get_Basic_Location(f1)
                    Return f2
                End Function
            End Class
            Public Class GffParser

                Dim LociBuilder As New LocationBuilder
                Dim tmpSeq As New Bio.Sequence(Bio.Alphabets.AmbiguousDNA, "")
                Dim SeqList As List(Of Bio.ISequence)
                Dim Gff3Files As List(Of FileInfo)
                Dim SeqIDComprer As New Szunyi.Comparares.OneByOne.SequenceIDComparer
                Dim GenBankMetaDatas As New List(Of GenBankMetadata)
                Public Property Name As String
                Public Property Result As SequenceList
                Public Property Type As String = Szunyi.Constants.BackGroundWork.Gff3Parser
                Public Shared Property IllegalCharacters As List(Of String)

                Public Sub New(SeqList As List(Of Bio.ISequence), Gff3Files As List(Of FileInfo), Name As String)
                    SeqList.Sort(SeqIDComprer) ' Just to be sure
                    Me.Name = Name
                    For Each Seq In SeqList
                        If Seq.Metadata.ContainsKey(Bio.Util.Helper.GenBankMetadataKey) = False Then
                            Dim x As New GenBankMetadata
                            x.Locus = New Bio.IO.GenBank.GenBankLocusInfo
                            x.Locus.Date = Now
                            x.Locus.MoleculeType = MoleculeType.DNA
                            x.Locus.Name = Seq.ID
                            x.Locus.SequenceLength = Seq.Count
                            x.Locus.StrandTopology = SequenceStrandTopology.Linear
                            x.Accession = New GenBankAccession()
                            x.Accession.Primary = Seq.ID
                            x.Source = New Bio.IO.GenBank.SequenceSource
                            x.Source.CommonName = "Unknown"
                            x.Source.Organism = New Bio.IO.GenBank.OrganismInfo
                            x.Source.Organism.Species = "Unknown"
                            x.Source.Organism.Genus = "Unknown"
                            x.Features = New Bio.IO.GenBank.SequenceFeatures
                            Seq.Metadata.Add(Bio.Util.Helper.GenBankMetadataKey, x)
                        End If
                    Next
                    Me.GenBankMetaDatas = Szunyi.Features.FeatureManipulation.GenBankMetaDataManipulation.GetGenBankMetaDatasFromSeqs(SeqList)
                    Me.SeqList = SeqList
                    Me.Gff3Files = Gff3Files

                End Sub
                Public Sub DoIt()


                    BuildGenBankAnnotation(ImportGff3Annotation())
                    Dim s = InputBox("Enter the Title")
                    Me.Result = New Szunyi.ListOf.SequenceList(Me.SeqList, s, "Gff3 Parsed: " & Name)

                End Sub

                Private Function ImportGff3Annotation() As List(Of Annotation)
                    Dim res As New List(Of Annotation)
                    For Each File In Me.Gff3Files
                        Using sr As New StreamReader(File.FullName)
                            Do
                                Dim Line As String = sr.ReadLine
                                If Line.StartsWith("#") = False Then ' Else This is a comment and forget it!
                                    Dim x As New Annotation(Line)
                                    res.Add(x)
                                End If
                            Loop Until sr.EndOfStream = True
                        End Using
                    Next
                    Return res
                End Function
                Private Sub BuildGenBankAnnotation(Annotations As List(Of Annotation))
                    ' Build Hierarchy For CDS
                    Dim str As New System.Text.StringBuilder
                    Dim lb As New LocationBuilder
                    For i1 = 0 To Annotations.Count - 1
                        With Annotations(i1)
                            Try
                                Dim loc = GetLocation(Annotations(i1))
                                Select Case .Type
                                    Case "gene"
                                        Dim x As New Gene(loc)
                                        AddFeature(x, Annotations(i1), GenBankMetaDatas)
                                    Case "CDS"
                                        str.Length = 0
                                        Dim pts As New List(Of Point)
                                        For i2 = i1 To Annotations.Count - 1
                                            If Annotations(i2).Parent <> Annotations(i1).Parent Then

                                                Dim h = From x1 In pts Select x1 Order By x1.X

                                                For Each v In h
                                                    str.Append(v.X & ".." & v.Y & ",")
                                                Next
                                                str.Length -= 1
                                                loc = GetLocation(str.ToString, Annotations(i1))
                                                Dim x As New CodingSequence(loc)
                                                AddFeature(x, Annotations(i1), GenBankMetaDatas)
                                                i1 = i2 - 1
                                                Exit For
                                            Else
                                                pts.Add(New Point(Annotations(i2).Start, Annotations(i2).Endy))

                                            End If
                                        Next
                                    Case "exon"
                                        Dim alf As Integer = 43
                                    Case "mRNA"
                                        Dim x As New MessengerRna(loc)
                                        AddFeature(x, Annotations(i1), GenBankMetaDatas)
                                    Case "transposable_element"
                                        Dim x As New Mobile_Element(loc)
                                        AddFeature(x, Annotations(i1), GenBankMetaDatas)
                                    Case "tRNA"
                                        Dim x As New TransferRna(loc)
                                        AddFeature(x, Annotations(i1), GenBankMetaDatas)
                                    Case "rRNA"
                                        Dim x As New RibosomalRna(loc)
                                        AddFeature(x, Annotations(i1), GenBankMetaDatas)
                                    Case Else
                                        If Szunyi.Constants.Features.ncRNA_Qulaifiers.Contains(.Type) Then
                                            Dim x As New NonCodingRna(loc)
                                            Dim h As New List(Of String)
                                            h.Add(.Type)
                                            x.Qualifiers.Add(StandardQualifierNames.NonCodingRnaClass, h)
                                            AddFeature(x, Annotations(i1), GenBankMetaDatas)
                                        Else
                                            Dim x As New FeatureItem(.Type, loc)
                                            AddFeature(x, Annotations(i1), GenBankMetaDatas)
                                        End If



                                End Select

                            Catch ex As Exception
                                Dim alf As Int16 = 43
                            End Try

                        End With

                    Next

                End Sub
                Private Sub AddFeature(x As Object, ann As Annotation, ByRef GenBankMetadatas As List(Of GenBankMetadata))
                    Try
                        x.Qualifiers(Bio.IO.GenBank.StandardQualifierNames.LocusTag) = Szunyi.Text.General.GetTextList(ann.ID)
                        If ann.Product <> "" Then _
                            x.Qualifiers(Bio.IO.GenBank.StandardQualifierNames.Product) = Szunyi.Text.General.GetTextList(ann.Product)
                        If ann.Label <> "" Then _
                            x.Qualifiers(Bio.IO.GenBank.StandardQualifierNames.Label) = Szunyi.Text.General.GetTextList(ann.Label)

                        Me.tmpSeq.ID = ann.SeqID
                        Dim cSeq = Me.SeqList.BinarySearch(Me.tmpSeq, SeqIDComprer)
                        Me.SeqList(cSeq).Metadata(Bio.Util.Helper.GenBankMetadataKey).Features.All.Add(x)
                        Dim alf As Int16 = 54
                    Catch ex As Exception
                        Dim alf As Int16 = 56
                    End Try

                End Sub
                Private Function GetLocation(p1 As String, ann As Annotation) As Bio.IO.GenBank.Location
                    Try

                        If Split(p1, "..").Count > 2 Then p1 = "join(" & p1 & ")"
                        If ann.IsComplementer = True Then p1 = "complement(" & p1 & ")"
                        Return LociBuilder.GetLocation(p1)
                    Catch ex As Exception
                        Return Nothing
                    End Try

                End Function
                Private Function GetLocation(Item As Annotation) As Bio.IO.GenBank.Location
                    Try
                        Dim loc As New LocationBuilder
                        Dim p1 As String = Item.Start & ".." & Item.Endy
                        If Item.IsComplementer = True Then p1 = "complement(" & p1 & ")"
                        Return loc.GetLocation(p1)
                    Catch ex As Exception
                        Return Nothing
                    End Try

                End Function

                Private Function GetGenbanbkLocation(p1 As String, IsComplementer As Boolean) As Bio.IO.GenBank.Location
                    Try
                        If Split(p1, "..").Count > 2 Then
                            p1 = "join(" & p1 & ")"
                        End If
                        If IsComplementer = True Then
                            p1 = "complement(" & p1 & ")"
                        End If
                        Dim loc = New Bio.IO.GenBank.Location
                        Dim bd As New Bio.IO.GenBank.LocationBuilder
                        Return bd.GetLocation(p1)
                    Catch ex As Exception
                        Dim alf As Integer = 43
                    End Try
                    Return Nothing

                End Function


            End Class
            Public Class Annotation
                Public SeqID As String
                Public Type As String
                Public Start As Integer
                Public Endy As Integer
                Public IsComplementer As Boolean = False
                Public Frame As Integer
                Public Parent As String
                Public ID As String
                Public Product As String
                Public Label As String
                Dim s1() As String
                Public Sub New(Line As String)
                    s1 = Split(Line, vbTab)
                    Me.SeqID = s1(0)
                    Me.Type = s1(2)
                    Me.Start = s1(3)
                    Me.Endy = s1(4)
                    If s1(6) = "-" Then IsComplementer = True
                    If IsNumeric(s1(7)) = True Then Me.Frame = s1(7)

                    Dim s2 = Split(s1(8), ";")
                    Me.ID = s2(0).Split("=").Last
                    For i1 = 0 To s2.Count - 1
                        If s2(i1).StartsWith("Parent") Then
                            Me.Parent = s2(i1).Split("=").Last
                        ElseIf s2(i1).StartsWith("Product") Then
                            Me.Product = s2(i1).Split("=").Last
                        ElseIf s2(i1).StartsWith("description") Then
                            Me.Product = s2(i1).Split("=").Last
                        ElseIf s2(i1).StartsWith("Name") Then
                            Me.Label = s2(i1).Split("=").Last
                        ElseIf s2(i1).StartsWith("conf_class") Then
                            Dim t1 As Integer = 43
                        Else

                            Dim alft As Integer = 43
                        End If

                    Next
                    Dim alf As Int16 = 43
                End Sub
                Public Overrides Function ToString() As String
                    Return Szunyi.Text.General.GetText(s1, vbTab)


                End Function

            End Class
            Public Class GFFFormatter
                Public Seqs As List(Of Bio.ISequence)
                Public File As FileInfo
                Public Shared IllegalCharacters() As Char = (";=," & Chr(34)).ToArray
                Dim LociBuilder As New Bio.IO.GenBank.LocationBuilder

                Public Sub New(Seqs As List(Of Bio.ISequence), File As FileInfo)
                    Me.Seqs = Seqs
                    Me.File = File
                End Sub
                Public Function Export_Wo_Parents(Feats As List(Of FeatureItem), Seq As Bio.ISequence)
                    Dim str As New System.Text.StringBuilder
                    str.Append("##gff-version 3").AppendLine()
                    For Each theRNA In Feats
                        Add_Key_Start_End_Complement(Seq, theRNA.Location, StandardFeatureKeys.MessengerRna, str)
                        str.Append(".").Append(vbTab) ' phase
                        Dim Locus_Tag As String
                        If theRNA.Qualifiers.ContainsKey(StandardQualifierNames.GeneSymbol) = True AndAlso theRNA.Qualifiers(StandardQualifierNames.GeneSymbol).Count > 0 Then
                            Locus_Tag = theRNA.Qualifiers(StandardQualifierNames.GeneSymbol).First
                        Else

                            Locus_Tag = theRNA.Label
                        End If


                        str.Append("ID=").Append(Locus_Tag).AppendLine()
                        Dim Exons = Szunyi.Features.FeatureManipulation.GetLocations.GetCDSExonsLocations(theRNA)
                        Dim FirstExonLoc = LociBuilder.GetLocation(theRNA.Location.LocationStart & ".." & Exons.First.LocationEnd)
                        FirstExonLoc.Operator = theRNA.Location.Operator
                        Dim LastExonLoc = LociBuilder.GetLocation(Exons.Last.LocationStart & ".." & theRNA.Location.LocationEnd)
                        LastExonLoc.Operator = theRNA.Location.Operator

                        Exons(0) = FirstExonLoc
                        Exons(Exons.Count - 1) = LastExonLoc

                        If theRNA.Location.Operator = LocationOperator.Complement Then
                            For i1 = Exons.Count - 1 To 0 Step -1
                                Add_Key_Start_End_Complement(Seq, Exons(i1), StandardFeatureKeys.Exon, str)
                                str.Append(".").Append(vbTab) ' phase
                                Add_ID_ParentID(str, StandardFeatureKeys.MessengerRna & "_" & StandardFeatureKeys.Exon & "_" & i1 + 1,
                                            Locus_Tag)
                            Next
                        Else
                            For i1 = 0 To Exons.Count - 1
                                Add_Key_Start_End_Complement(Seq, Exons(i1), StandardFeatureKeys.Exon, str)
                                str.Append(".").Append(vbTab) ' phase
                                Add_ID_ParentID(str, StandardFeatureKeys.MessengerRna & "_" & StandardFeatureKeys.Exon & "_" & i1 + 1,
                                            Locus_Tag)
                            Next
                        End If

                        Dim Introns = Szunyi.Features.FeatureManipulation.GetLocations.GetIntronLocationsFromExonLOcations(Exons)
                        For Each Intron In Introns
                            Intron.Operator = theRNA.Location.Operator
                        Next
                        If theRNA.Location.Operator = LocationOperator.Complement Then
                            For i1 = Introns.Count - 1 To 0 Step -1
                                Add_Key_Start_End_Complement(Seq, Introns(i1), StandardFeatureKeys.Intron, str)
                                str.Append(".").Append(vbTab) ' phase
                                Add_ID_ParentID(str, StandardFeatureKeys.MessengerRna & "_" & StandardFeatureKeys.Intron & "_" & i1 + 1,
                                            Locus_Tag)
                            Next
                        Else
                            For i1 = 0 To Introns.Count - 1
                                Add_Key_Start_End_Complement(Seq, Introns(i1), StandardFeatureKeys.Intron, str)
                                str.Append(".").Append(vbTab) ' phase
                                Add_ID_ParentID(str, StandardFeatureKeys.MessengerRna & "_" & StandardFeatureKeys.Intron & "_" & i1 + 1,
                                            Locus_Tag)
                            Next
                        End If


                    Next
                    Return str.ToString
                End Function
                Public Sub DoIt()
                    Dim str As New System.Text.StringBuilder
                    str.Append("##gff-version 3").AppendLine()
                    Using ws As New StreamWriter(Me.File.FullName)
                        For Each Seq In Seqs
                            Szunyi.Features.FeatureManipulation.Common.Correct_LocusTags_From_Seq(Seq)

                            Dim Md = Szunyi.Features.FeatureManipulation.GenBankMetaDataManipulation.GetGenBankMetaDataFromSeq(Seq)
                            If IsNothing(Md) = False Then
                                SetGenes(Seq, Md, str, ws) ' Genes, mrna,CDS
                                For Each FeatKey In Bio.IO.GenBank.StandardFeatureKeys.All
                                    If FeatKey <> StandardFeatureKeys.CodingSequence Or FeatKey <> StandardFeatureKeys.Gene Or
                                            FeatKey <> StandardFeatureKeys.Intron Or FeatKey <> StandardFeatureKeys.Exon Or
                                            FeatKey <> StandardFeatureKeys.MessengerRna Then
                                        If FeatKey = StandardFeatureKeys.NonCodingRna Then
                                            Set_NC_RNAs(Seq, Md, str, StandardFeatureKeys.NonCodingRna, ws)
                                        Else
                                            Set_No_Parent_Simple(Seq, Md, str, FeatKey, ws)

                                        End If
                                    End If
                                Next

                            End If

                        Next
                    End Using

                End Sub
                Private Sub Set_NC_RNAs(ByRef seq As Bio.ISequence, ByRef Md As Bio.IO.GenBank.GenBankMetadata, ByRef str As StringBuilder, Feature_Key As String, ws As StreamWriter)
                    Dim Features = Szunyi.Features.FeatureManipulation.GetFeaturesByType.GetFeatureByTypeFromMetadata(Feature_Key, Md)
                    If IsNothing(Features) = False Then
                        For Each Feat In Features
                            Dim Locus_Tag = Szunyi.Features.FeatureManipulation.Common.Get_LocusTag(Feat)
                            Dim Key = Feat.Qualifiers(StandardQualifierNames.NonCodingRnaClass).First.Replace(Chr(34), "")
                            Add_Key_Start_End_Complement(seq, Feat.Location, Key, str)
                            str.Append(".").Append(vbTab) ' phase
                            Add_ID_ParentID(str, Locus_Tag)  ' No Parent For Gene
                            ws.Write(str.ToString)
                            str.Length = 0
                        Next
                    End If

                End Sub
                Private Sub Set_No_Parent_Simple(ByRef seq As Bio.ISequence, ByRef Md As Bio.IO.GenBank.GenBankMetadata, ByRef str As StringBuilder, Feature_Key As String, ws As StreamWriter)
                    Dim Features = Szunyi.Features.FeatureManipulation.GetFeaturesByType.GetFeatureByTypeFromMetadata(Feature_Key, Md)
                    If IsNothing(Features) = False Then
                        For Each Feat In Features
                            Dim Locus_Tag = Szunyi.Features.FeatureManipulation.Common.Get_LocusTag(Feat)
                            Add_Key_Start_End_Complement(seq, Feat.Location, Feature_Key, str)
                            str.Append(".").Append(vbTab) ' phase
                            Add_ID_ParentID(str, Locus_Tag)  ' No Parent For Gene
                            ws.Write(str.ToString)
                            str.Length = 0
                        Next
                    End If


                End Sub
                Private Sub SetGenes(ByRef seq As Bio.ISequence, ByRef Md As Bio.IO.GenBank.GenBankMetadata, ByRef str As StringBuilder, Ws As StreamWriter)
                    Dim Genes = Szunyi.Features.FeatureManipulation.GetFeaturesByType.GetFeatureByTypeFromMetadata(StandardFeatureKeys.Gene, Md)
                    Dim lmRNAs As New List(Of FeatureItem)
                    Dim lCDSss As New List(Of FeatureItem)
                    If IsNothing(Genes) = False Then
                        Dim TheGenes = Genes.ToList
                        TheGenes.Sort(Szunyi.Comparares.AllComparares.By_Gene)

                        Dim mRNAs = Szunyi.Features.FeatureManipulation.GetFeaturesByType.GetFeatureByTypeFromMetadata(StandardFeatureKeys.MessengerRna, Md)
                        If IsNothing(mRNAs) = False Then
                            lmRNAs = mRNAs.ToList
                            lmRNAs.Sort(Szunyi.Comparares.AllComparares.By_Gene)
                        End If

                        Dim CDSs = Szunyi.Features.FeatureManipulation.GetFeaturesByType.GetFeatureByTypeFromMetadata(StandardFeatureKeys.CodingSequence, Md)
                        If IsNothing(CDSs) = False Then
                            lCDSss = CDSs.ToList
                            lCDSss.Sort(Szunyi.Comparares.AllComparares.By_Gene)
                        End If

                        For Each Gene In Genes
                            '  Dim Locus_Tag = Szunyi.Features.FeatureManipulation.Common.Get_LocusTag(Gene)
                            Dim Locus_Tag = Gene.Qualifiers(StandardFeatureKeys.Gene).First
                            Dim SG = From x In lmRNAs Where x.Qualifiers.ContainsKey(StandardFeatureKeys.Gene)
                            Dim gmRNAsII = From x In SG Where x.Qualifiers(StandardFeatureKeys.Gene).First = Locus_Tag

                            Dim gmrnas As New List(Of FeatureItem)

                            If gmRNAsII.Count > 0 Then
                                gmrnas = gmRNAsII.ToList
                            End If
                            Dim gCDSs = From x In lCDSss Where x.Qualifiers(StandardFeatureKeys.Gene).First = Locus_Tag

                            '   Dim gmRNAs = Szunyi.Features.FeatureManipulation.UniqueDistict.Get_Features_Has_Full_LocusTag_From_Short_LocusTag(lmRNAs, Locus_Tag)
                            '  Dim gCDSs = Szunyi.Features.FeatureManipulation.UniqueDistict.Get_Features_Has_Full_LocusTag_From_Short_LocusTag(lCDSss, Locus_Tag)
                            If gmrnas.Count <> gCDSs.Count Then
                                Dim alf As Int16 = 65
                            Else
                                Add_Key_Start_End_Complement(seq, Gene.Location, StandardFeatureKeys.Gene, str)
                                str.Append(".").Append(vbTab) ' phase
                                Add_ID_ParentID(str, Locus_Tag)  ' No Parent For Gene

                                For i1 = 0 To gmrnas.Count - 1
                                    gmrnas(i1) = Szunyi.Features.FeatureManipulation.GetLocations.CorrectStandAndEnd(gmrnas(i1), Gene)
                                    Add_mRNA(str, gmrnas(i1), seq)
                                    Add_CDS(str, gCDSs(i1), seq)
                                Next
                                Ws.Write(str.ToString)
                                str.Length = 0
                            End If
                        Next
                    End If
                End Sub
                Private Sub Add_CDS(str As StringBuilder, mRNA As FeatureItem, Seq As Bio.ISequence)
                    Dim Exons = Szunyi.Features.FeatureManipulation.GetLocations.GetCDSExonsLocations(mRNA)
                    Dim Locus_Tag = Szunyi.Features.FeatureManipulation.Common.Get_LocusTag(mRNA)
                    Dim cExonLength As Integer = 0
                    Dim SortedExons As List(Of ILocation)
                    If Exons.First.Operator = LocationOperator.Complement Then
                        SortedExons = (From x In Exons Order By x.LocationStart Descending).ToList
                    Else
                        SortedExons = (From x In Exons Order By x.LocationStart Ascending).ToList
                    End If
                    For i1 = 0 To Exons.Count - 1
                        Add_Key_Start_End_Complement(Seq, SortedExons(i1), StandardFeatureKeys.CodingSequence, str)
                        Dim m = cExonLength Mod 3
                        If m = 1 Then
                            m = 2
                        ElseIf m = 2 Then
                            m = 1
                        End If
                        str.Append(m).Append(vbTab)
                        cExonLength += SortedExons(i1).LocationEnd - SortedExons(i1).LocationStart + 1
                        Add_ID_ParentID(str, StandardFeatureKeys.CodingSequence & "_" & StandardFeatureKeys.Exon & "_" & i1 + 1,
                                        Locus_Tag)
                    Next

                End Sub

                Private Sub Add_mRNA(str As StringBuilder, mRNA As FeatureItem, Seq As Bio.ISequence)

                    Add_Key_Start_End_Complement(Seq, mRNA.Location, StandardFeatureKeys.MessengerRna, str)
                    str.Append(".").Append(vbTab) ' phase
                    Add_ID_Parent_Note(mRNA, str)
                    Dim Exons = Szunyi.Features.FeatureManipulation.GetLocations.GetCDSExonsLocations(mRNA)
                    Dim FirstExonLoc = LociBuilder.GetLocation(mRNA.Location.LocationStart & ".." & Exons.First.LocationEnd)
                    FirstExonLoc.Operator = mRNA.Location.Operator
                    Dim LastExonLoc = LociBuilder.GetLocation(Exons.Last.LocationStart & ".." & mRNA.Location.LocationEnd)
                    LastExonLoc.Operator = mRNA.Location.Operator

                    Exons(0) = FirstExonLoc
                    Exons(Exons.Count - 1) = LastExonLoc
                    Dim Locus_Tag = Szunyi.Features.FeatureManipulation.Common.Get_LocusTag(mRNA)
                    If mRNA.Location.Operator = LocationOperator.Complement Then
                        For i1 = Exons.Count - 1 To 0 Step -1
                            Add_Key_Start_End_Complement(Seq, Exons(i1), StandardFeatureKeys.Exon, str)
                            str.Append(".").Append(vbTab) ' phase
                            Add_ID_ParentID(str, StandardFeatureKeys.MessengerRna & "_" & StandardFeatureKeys.Exon & "_" & i1 + 1,
                                            Locus_Tag)
                        Next
                    Else
                        For i1 = 0 To Exons.Count - 1
                            Add_Key_Start_End_Complement(Seq, Exons(i1), StandardFeatureKeys.Exon, str)
                            str.Append(".").Append(vbTab) ' phase
                            Add_ID_ParentID(str, StandardFeatureKeys.MessengerRna & "_" & StandardFeatureKeys.Exon & "_" & i1 + 1,
                                            Locus_Tag)
                        Next
                    End If

                    Dim Introns = Szunyi.Features.FeatureManipulation.GetLocations.GetIntronLocationsFromExonLOcations(Exons)
                    For Each Intron In Introns
                        Intron.Operator = mRNA.Location.Operator
                    Next
                    If mRNA.Location.Operator = LocationOperator.Complement Then
                        For i1 = Introns.Count - 1 To 0 Step -1
                            Add_Key_Start_End_Complement(Seq, Introns(i1), StandardFeatureKeys.Intron, str)
                            str.Append(".").Append(vbTab) ' phase
                            Add_ID_ParentID(str, StandardFeatureKeys.MessengerRna & "_" & StandardFeatureKeys.Intron & "_" & i1 + 1,
                                            Locus_Tag)
                        Next
                    Else
                        For i1 = 0 To Introns.Count - 1
                            Add_Key_Start_End_Complement(Seq, Introns(i1), StandardFeatureKeys.Intron, str)
                            str.Append(".").Append(vbTab) ' phase
                            Add_ID_ParentID(str, StandardFeatureKeys.MessengerRna & "_" & StandardFeatureKeys.Intron & "_" & i1 + 1,
                                            Locus_Tag)
                        Next
                    End If


                End Sub
                Private Sub Add_ID_ParentID(str As StringBuilder, ID As String, Optional ParentID As String = "")
                    str.Append("ID=").Append(ID)
                    If ParentID <> "" Then
                        str.Append(";")
                        str.Append("Parent=").Append(ParentID)
                    End If
                    str.AppendLine()
                End Sub
                Private Sub Add_ID_Parent_Note(Feat As FeatureItem, str As StringBuilder)
                    str.Append("ID=").Append(Szunyi.Features.FeatureManipulation.Common.Get_LocusTag(Feat))
                    str.Append(";")
                    str.Append("Parent=")
                    str.Append(Szunyi.Features.FeatureManipulation.Common.Get_ShortLocusTag(Feat))
                    Dim pr = Szunyi.Features.FeatureManipulation.Common.Get_ByQualifier_First(StandardQualifierNames.Product, Feat)
                    If pr.Length <> 0 Then str.Append(";Note=").Append(pr)
                    Dim DbXrefs = Szunyi.Features.FeatureManipulation.Common.Get_ByQualifier_All(StandardQualifierNames.DatabaseCrossReference, Feat)
                    If DbXrefs.Count > 0 Then
                        str.Append(";Dbxref=")
                        For Each DbXref In DbXrefs
                            DbXref = Szunyi.Text.General.RemoveFromString(DbXref, Szunyi.Sequences.Gff.GFFFormatter.IllegalCharacters)
                            str.Append(DbXref).Append(",")
                        Next
                        str.Length -= 1
                    End If
                    str.AppendLine()
                End Sub

                Private Sub Add_Key_Start_End_Complement(Seq As Bio.ISequence, Location As ILocation, Type As String, ByRef str As StringBuilder)
                    str.Append(Seq.ID).Append(vbTab)
                    str.Append(vbTab) ' Source
                    str.Append(Type).Append(vbTab) ' Feature Key
                    Add_Location(Location, str)
                End Sub
                Private Sub Add_Location(location As ILocation, str As StringBuilder)
                    If location.LocationStart > location.LocationEnd Then
                        str.Append(location.LocationEnd).Append(vbTab).Append(location.LocationStart).Append(vbTab)
                    Else
                        str.Append(location.LocationStart).Append(vbTab).Append(location.LocationEnd).Append(vbTab)

                    End If
                    str.Append(vbTab) ' Score
                    If location.Operator = LocationOperator.Complement Then
                        str.Append("-")
                    Else
                        str.Append("+")
                    End If
                    str.Append(vbTab)
                End Sub

            End Class
        End Namespace
    End Namespace
End Namespace
