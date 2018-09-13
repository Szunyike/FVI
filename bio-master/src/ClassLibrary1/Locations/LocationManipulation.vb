Imports System.IO
Imports Bio
Imports Bio.IO.GenBank
Imports Bio.IO.SAM
Imports ClassLibrary1.Szunyi.Alignment
Imports ClassLibrary1.Szunyi.Basic
Imports ClassLibrary1.Szunyi.Constants
Imports ClassLibrary1.Szunyi.Features
Imports ClassLibrary1.Szunyi.ListOf

Namespace Szunyi
    Namespace Location

        Public Class Import
            Public Property Location_Type As Szunyi.Constants.Location_Type
            Public Property File As FileInfo
            Public Property Ext_Feature_List As ExtFeatureList
            Public Property result As LocationList
            Public Property Title As String
            Public Property Type As String = Szunyi.Constants.BackGroundWork.Locations
            Public Sub New(File As FileInfo, Location_Type As Szunyi.Constants.Location_Type)
                Me.File = File
                Me.Location_Type = Location_Type
                Me.Title = Szunyi.IO.Files.Get_New_FileName.GetFileName_woExtension(File)
            End Sub
            Public Sub New(Ext_Feature_List As ExtFeatureList, Location_Type As Szunyi.Constants.Location_Type)
                Me.Ext_Feature_List = Ext_Feature_List
                Me.Location_Type = Location_Type
                Me.Title = Me.Ext_Feature_List.ShortFileName
            End Sub

            Private Function Get_Basic_Location(Line) As Basic_Location
                Select Case Location_Type
                    Case Szunyi.Constants.Location_Type.GFF3
                        Return Get_Basic_Location_From_GFF(Line)
                    Case Szunyi.Constants.Location_Type.BED
                        Return Get_Basic_Location_From_BED(Line)
                    Case Szunyi.Constants.Location_Type.Standard
                        Return Get_Basic_Location_From_Standard(Line)
                End Select
                Return Nothing
            End Function
            Private Function Get_Basic_Location_From_Feature(Feat As ExtFeature) As Basic_Location
                Dim x As New Basic_Location
                x.SeqID = Feat.Seq.ID
                x.Start = Feat.Feature.Location.LocationStart
                x.Endy = Feat.Feature.Location.LocationEnd
                x.Obj = Feat
                x.Extra = New List(Of String)
                x.Extra.Add(x.SeqID)
                x.Extra.Add(x.Start)
                x.Extra.Add(x.Endy)
                x.Extra.Add(Szunyi.Features.ExtFeatureManipulation.GetStrand(Feat))
                x.Extra.Add(Feat.Feature.Key)
                x.Extra.Add(Feat.LocusTag)
                x.Extra.Add(Szunyi.Features.ExtFeatureManipulation.GetTextFromExtFeature(Feat.Feature, StandardQualifierNames.Product))
                x.Ext_Key = x.Extra(5) & vbTab & x.Extra(4) & vbTab & x.Extra(6) & vbTab & x.Extra(3)
                Return x
            End Function
            Public Shared Function Get_Basic_Locations_From_GFF_File(FIle As FileInfo) As List(Of Basic_Location)
                If IsNothing(FIle) = True Then Return Nothing
                Dim res As New List(Of Basic_Location)
                For Each Line In Szunyi.IO.Import.Text.Parse(FIle, "#")
                    Dim t = Get_Basic_Location_From_GFF(Line)
                    If IsNothing(t) = False Then
                        res.Add(t)
                    Else
                        Dim alf As Int16 = 65
                    End If

                Next
                Return res
            End Function

            Private Shared Function Get_Basic_Location_From_GFF(Line As String) As Basic_Location

                If IsNothing(Line) = False Then

                    Dim s1() = Split(Line, vbTab)
                    If s1.Length > 6 Then
                        Dim x As New Basic_Location

                        x.SeqID = s1(0)
                        x.Start = s1(3) - 1
                        x.Endy = s1(4) - 1
                        If s1(6) = "+" Then
                            x.IsComplement = Szunyi.Constants.Strand_Type.Positive_strand
                        ElseIf s1(6) = "-" Then
                            x.IsComplement = Szunyi.Constants.Strand_Type.Negative_strand
                        Else
                            x.IsComplement = Szunyi.Constants.Strand_Type.Unknown_strand
                        End If
                        x.Location = Szunyi.Location.Common.GetLocation(x)
                        x.Extra = s1.ToList
                        Return x
                    End If
                End If

                Return Nothing
            End Function
            Private Function Get_Basic_Location_From_Standard(Line As String) As Basic_Location
                Dim x As New Basic_Location
                If IsNothing(Line) = False Then

                    Dim s1() = Split(Line, vbTab)
                    x.SeqID = s1(0)
                    x.Start = s1(1)
                    x.Endy = s1(2)
                    If s1(3) = "+" Then
                        x.IsComplement = Szunyi.Constants.Strand_Type.Positive_strand
                    ElseIf s1(3) = "-" Then
                        x.IsComplement = Szunyi.Constants.Strand_Type.Negative_strand
                    Else
                        x.IsComplement = Szunyi.Constants.Strand_Type.Unknown_strand
                    End If
                    x.Extra = s1.ToList
                End If
                Return x
            End Function
            Private Function Get_Basic_Location_From_BED(Line As String) As Basic_Location
                Dim s1() = Split(Line, vbTab)
                Dim x As New Basic_Location
                x.SeqID = s1(0)
                x.Start = s1(1)
                x.Endy = s1(2)
                If s1.Count > 3 Then
                    x.Name = s1(3)
                    If s1.Count > 5 Then
                        If s1(5) = "+" Then
                            x.IsComplement = Szunyi.Constants.Strand_Type.Positive_strand
                        ElseIf s1(5) = "-" Then
                            x.IsComplement = Szunyi.Constants.Strand_Type.Negative_strand
                        Else
                            x.IsComplement = Szunyi.Constants.Strand_Type.Unknown_strand
                        End If
                    End If
                End If
                x.Extra = s1.ToList
                Return x
            End Function
            Public Iterator Function Iterate_Basic_Location_From_BED(File As FileInfo) As IEnumerable(Of Basic_Location)
                Using sr As New StreamReader(File.FullName)
                    Do
                        Dim Line = sr.ReadLine

                        Dim s1() = Split(Line, vbTab)
                        Dim x As New Basic_Location
                        x.SeqID = s1(0)
                        x.Start = s1(1)
                        x.Endy = s1(2)
                        If s1.Count > 3 Then
                            '  x.Name = Split(s1(3), "/").First
                            If s1.Count > 5 Then
                                If s1(5) = "+" Then
                                    x.IsComplement = False
                                ElseIf s1(5) = "-" Then
                                    x.IsComplement = True
                                End If
                            End If
                            Yield x
                        End If
                    Loop Until sr.EndOfStream = True

                End Using
            End Function
            Public Shared Function Donor_Acceptor_Strand(fIle As FileInfo, Nof_First_Line As Integer) As List(Of ILocation)
                Dim out As New List(Of ILocation)
                For Each Line In Szunyi.IO.Import.Text.Parse(fIle, Nof_First_Line)
                    Dim s = Split(Line, vbTab)
                    Dim l = Szunyi.Location.Common.GetLocation(s(0), s(1), s(2))
                    l.Accession = s(0)
                    out.Add(l)
                Next
                Return out
            End Function
            Public Shared Function Name_Position_Strand(fIle As FileInfo, Nof_First_Line As Integer) As List(Of ILocation)
                Dim out As New List(Of ILocation)
                For Each Line In Szunyi.IO.Import.Text.Parse(fIle, Nof_First_Line)
                    Dim s = Split(Line, vbTab)
                    Dim l = Szunyi.Location.Common.GetLocation(s(1), s(1), s(2))
                    l.Accession = s(0)
                    out.Add(l)
                Next
                Return out
            End Function
        End Class

        Public Class Basic_Location_Manipulation
#Region "Create Basic Locations"
            Public Shared Function Get_Basic_LocationS(SAMs As List(Of Bio.IO.SAM.SAMAlignedSequence), Optional Seq As Bio.ISequence = Nothing, Optional Obj As Object = Nothing) As List(Of Basic_Location)
                Dim out As New List(Of Basic_Location)
                For Each SAM In SAMs
                    Dim l = Szunyi.Location.Common.GetLocation(New Szunyi.Alignment.Own_Al(SAM))
                    out.Add(Get_Basic_Location(l))
                    out.Last.Obj = SAM
                Next
                Return out
            End Function
            Public Shared Function Get_Basic_LocationS(TSs As List(Of Szunyi.mRNA.Transcript.TemplateSwitch), Optional Seq As Bio.ISequence = Nothing, Optional Obj As Object = Nothing) As List(Of Basic_Location)
                Dim out As New List(Of Basic_Location)
                For Each SAM In TSs
                    out.Add(Get_Basic_Location(SAM.Loci))
                    out.Last.Obj = SAM
                Next
                Return out
            End Function

#Region "From Locis"
            Public Shared Function Get_Basic_LocationS(Locis As List(Of List(Of ILocation)), Optional Seq As Bio.ISequence = Nothing, Optional Obj As Object = Nothing) As List(Of Basic_Location)
                Dim out As New List(Of Basic_Location)
                For Each Loci In Locis
                    out.Add(Get_Basic_Location(Loci.First, Seq, Obj))
                Next
                Return out
            End Function
            Public Shared Function Get_Basic_LocationS(Locis As List(Of ILocation), Optional Seq As Bio.ISequence = Nothing, Optional Obj As Object = Nothing) As List(Of Basic_Location)
                Dim out As New List(Of Basic_Location)
                For Each Loci In Locis
                    out.Add(Get_Basic_Location(Loci, Seq, Obj))
                Next
                Return out
            End Function
            Public Shared Function Get_Basic_Location(loci As ILocation, Optional Seq As Bio.ISequence = Nothing, Optional Obj As Object = Nothing) As Basic_Location
                Dim x As New Basic_Location()
                x.Start = loci.LocationStart
                x.Endy = loci.LocationEnd
                If loci.Operator = LocationOperator.Complement Then
                    x.IsComplement = True
                Else
                    x.IsComplement = False
                End If
                x.Location = loci
                x.Obj = Obj
                x.Seq = Seq
                Return x
            End Function
#End Region
#Region "From Features"
            Public Shared Function Get_Basic_LocationS(Locis As List(Of FeatureItem), Optional Seq As Bio.ISequence = Nothing) As List(Of Basic_Location)
                Dim out As New List(Of Basic_Location)
                For Each Loci In Locis
                    out.Add(Get_Basic_Locations_From_Feat(Loci, Seq))
                Next
                Return out
            End Function
            Public Shared Function Get_Basic_Locations_From_Feat(loci As FeatureItem, Optional Seq As Bio.ISequence = Nothing) As Basic_Location
                Dim x As New Basic_Location()
                x.Start = loci.Location.LocationStart
                x.Endy = loci.Location.LocationEnd
                If loci.Location.Operator = LocationOperator.Complement Then
                    x.IsComplement = True
                Else
                    x.IsComplement = False
                End If
                x.Obj = loci
                x.Seq = Seq
                x.Location = loci.Location
                Return x
            End Function
#End Region
#Region "From ExtFeature"
            Public Shared Function Get_Basic_Locations_From_ExtFeats(Locis As List(Of ExtFeature)) As List(Of Basic_Location)
                Dim out As New List(Of Basic_Location)
                For Each Loci In Locis
                    out.Add(Get_Basic_Locations_From_ExtFeat(Loci))
                Next
                Return out
            End Function
            Public Shared Function Get_Basic_Locations_From_ExtFeat(loci As ExtFeature) As Basic_Location
                Dim x As New Basic_Location()
                x.Start = loci.Feature.Location.LocationStart
                x.Endy = loci.Feature.Location.LocationEnd
                If loci.Feature.Location.Operator = LocationOperator.Complement Then
                    x.IsComplement = True
                Else
                    x.IsComplement = False
                End If
                x.Obj = loci
                x.Seq = loci.Seq
                x.Location = loci
                Return x
            End Function
#End Region


#End Region
            Public Shared Function ModifyPosition(Locis As List(Of Basic_Location), plusStrand As Integer, minusStrand As Integer) As List(Of Basic_Location)
                If IsNothing(Locis) = True Then Return Nothing
                Dim out As New List(Of Basic_Location)
                For Each loci In Locis
                    Dim oldLoci = loci.Location
                    Dim nLoci As Bio.IO.GenBank.ILocation
                    If oldLoci.IsComplementer = True Then
                        nLoci = Szunyi.Location.Common.GetLocation(oldLoci.LocationStart + minusStrand, oldLoci.LocationEnd + minusStrand, "-")
                        out.Add(Get_Basic_Location(nLoci))
                    Else
                        nLoci = Szunyi.Location.Common.GetLocation(oldLoci.LocationStart + plusStrand, oldLoci.LocationEnd + plusStrand, "+")
                        out.Add(Get_Basic_Location(nLoci))

                    End If
                    out.Last.Extra = loci.Extra
                Next
                Return out
            End Function
            Public Shared Function Get_Counts_Near(The_Mapping As Basic_Location,
                                                   sort As Sort_Locations_By,
                                                   Values As Dictionary(Of String, Integer()), width As Integer) As Integer
                Dim max = Values.First.Value.Count - 1
                Select Case sort
                    Case Sort_Locations_By.TSS
                        Dim out As Integer = 0
                        If The_Mapping.Location.IsComplementer = True Then
                            For i1 = The_Mapping.Location.TSS - width To The_Mapping.Location.TSS + width
                                If i1 > -1 And i1 < max Then
                                    out += Values("+")(i1)
                                End If
                            Next
                        Else
                            For i1 = The_Mapping.Location.TSS - width To The_Mapping.Location.TSS + width
                                If i1 > -1 And i1 < max Then
                                    out += Values("+")(i1)
                                End If
                            Next
                        End If
                        Return out

                    Case Sort_Locations_By.PAS
                        Dim out As Integer = 0
                        If The_Mapping.Location.IsComplementer = True Then
                            For i1 = The_Mapping.Location.PAS - width To The_Mapping.Location.PAS + width
                                If i1 > -1 And i1 < max Then
                                    out += Values("+")(i1)
                                End If
                            Next
                        Else
                            For i1 = The_Mapping.Location.PAS - width To The_Mapping.Location.PAS + width
                                If i1 > -1 And i1 < max Then
                                    out += Values("+")(i1)
                                End If
                            Next
                        End If
                        Return out


                    Case Sort_Locations_By.TSS_PAS
                        Dim out As Integer = 0
                        If The_Mapping.Location.IsComplementer = True Then
                            For i1 = The_Mapping.Location.LocationStart - width To The_Mapping.Location.LocationEnd + width
                                If i1 > -1 And i1 < max Then
                                    out += Values("+")(i1)
                                End If
                            Next
                        Else
                            For i1 = The_Mapping.Location.LocationStart - width To The_Mapping.Location.LocationEnd + width
                                If i1 > -1 And i1 < max Then
                                    out += Values("+")(i1)
                                End If
                            Next
                        End If
                        Return out
                    Case Sort_Locations_By.LE
                        Dim out As Integer = 0
                        If The_Mapping.Location.IsComplementer = True Then
                            For i1 = The_Mapping.Location.LocationEnd - width To The_Mapping.Location.LocationEnd + width
                                out += Values("-")(i1)

                            Next
                        Else
                            For i1 = The_Mapping.Location.LocationEnd - width To The_Mapping.Location.LocationEnd + width
                                out += Values("+")(i1)
                            Next
                        End If
                        Return out
                    Case Sort_Locations_By.TSS
                        Dim out As Integer = 0
                        If The_Mapping.Location.IsComplementer = True Then
                            For i1 = The_Mapping.Location.LocationStart - width To The_Mapping.Location.LocationStart + width
                                out += Values("-")(i1)

                            Next
                        Else
                            For i1 = The_Mapping.Location.LocationStart - width To The_Mapping.Location.LocationStart + width
                                out += Values("+")(i1)
                            Next
                        End If
                        Return out
                End Select

            End Function


            Public Shared Function Get_Start_Position(Loci As Basic_Location) As Integer
                If Loci.IsComplement = True Then
                    Return (Loci.Endy)
                Else
                    Return (Loci.Start)
                End If
            End Function

            Public Shared Function Get_Start_Positions(Locis As List(Of Basic_Location)) As List(Of Integer)
                Dim out As New List(Of Integer)
                For Each Loci In Locis
                    out.Add(Get_Start_Position(Loci))
                Next
                Return out
            End Function
            Public Shared Function Basic_Location_ToString(BL As Szunyi.Location.Basic_Location, Optional separator As String = vbTab) As String
                Dim str As New System.Text.StringBuilder
                str.Append(BL.SeqID).Append(separator)
                str.Append(BL.Start).Append(separator)
                str.Append(BL.Endy).Append(separator)
                If BL.IsComplement = False Then
                    str.Append("+").Append(separator)
                Else
                    str.Append("-").Append(separator)
                End If
                Return str.ToString
            End Function

            Public Shared Sub Extend_Basic_Location(ByRef Location_Lists As List(Of List(Of Basic_Location)), ByRef range As Integer, ByRef ExtendBy As Sort_Locations_By)
                For Each Locis In Location_Lists
                    Extend_Basic_Location(Locis, range, ExtendBy)
                Next
            End Sub
            Public Shared Sub Extend_Basic_Location(ByRef Location_List As List(Of Basic_Location), ByRef range As Integer, ByRef ExtendBy As Sort_Locations_By)
                For Each Loci In Location_List
                    Dim l = Szunyi.Location.Common.Get_Basic_Location(Loci, range, ExtendBy)
                Next
            End Sub
            Public Shared Function Get_Minimum(Mappings As List(Of Bio.IO.GenBank.ILocation), Sort As Szunyi.Constants.Sort_Locations_By) As Integer
                Select Case Sort
                    Case Sort_Locations_By.TSS
                        If Mappings.First.IsComplementer = True Then
                            Return (From x In Mappings Select x.TSS).Max
                        Else
                            Return (From x In Mappings Select x.TSS).Min
                        End If

                    Case Sort_Locations_By.PAS
                        If Mappings.First.IsComplementer = True Then
                            Return (From x In Mappings Select x.PAS).Min
                        Else
                            Return (From x In Mappings Select x.PAS).Max
                        End If

                    Case Sort_Locations_By.LE
                        Return (From x In Mappings Select x.LocationEnd).Min

                    Case Sort_Locations_By.TSS
                        Return (From x In Mappings Select x.LocationStart).Min

                End Select
                Return -1
            End Function
            Public Shared Function Get_Maximum(Mappings As List(Of Bio.IO.GenBank.ILocation), Sort As Szunyi.Constants.Sort_Locations_By) As Integer
                Select Case Sort
                    Case Sort_Locations_By.TSS
                        If Mappings.First.IsComplementer = True Then
                            Return (From x In Mappings Select x.TSS).Min
                        Else
                            Return (From x In Mappings Select x.TSS).Max
                        End If

                    Case Sort_Locations_By.PAS
                        If Mappings.First.IsComplementer = True Then
                            Return (From x In Mappings Select x.PAS).Max
                        Else
                            Return (From x In Mappings Select x.PAS).Min
                        End If

                    Case Sort_Locations_By.LE
                        Return (From x In Mappings Select x.LocationEnd).Max

                    Case Sort_Locations_By.TSS
                        Return (From x In Mappings Select x.LocationStart).Max

                End Select
                Return -1
            End Function

            Public Shared Function Get_Most_Abundants(Mappings As List(Of Bio.IO.GenBank.ILocation), Sort As Szunyi.Constants.Sort_Locations_By) As List(Of Bio.IO.GenBank.ILocation)
                Select Case Sort
                    Case Sort_Locations_By.TSS

                        Dim x = From t In Mappings Group By t.TSS, t.IsComplementer Into Group Order By Group.Count Descending

                        Return x.First.Group.ToList

                    Case Sort_Locations_By.PAS
                        Dim x = From t In Mappings Group By t.PAS, t.IsComplementer Into Group Order By Group.Count Descending

                        Return x.First.Group.ToList
                    Case Sort_Locations_By.TSS_PAS
                        Dim x = From t In Mappings Group By t.LocationStart, t.LocationEnd, t.IsComplementer Into Group Order By Group.Count Descending

                        Return x.First.Group.ToList
                    Case Sort_Locations_By.LE
                        Dim x = From t In Mappings Group By t.LocationEnd, t.IsComplementer Into Group Order By Group.Count Descending

                        Return x.First.Group.ToList
                    Case Sort_Locations_By.TSS
                        Dim x = From t In Mappings Group By t.LocationStart, t.IsComplementer Into Group Order By Group.Count Descending

                        Return x.First.Group.ToList
                End Select
                Return New List(Of ILocation)
            End Function

            Public Shared Function Get_Most_Abundants_tss_pas(Mappings As List(Of Bio.IO.GenBank.ILocation)) As Bio.IO.GenBank.ILocation
                Dim x = From t In Mappings Group By t.TSS, t.IsComplementer Into Group Order By Group.Count Descending

                Dim TSS_Pos = x.First.Group.First.TSS

                Dim x1 = From t In Mappings Group By t.PAS, t.IsComplementer Into Group Order By Group.Count Descending

                Dim PAS_Pos = x.First.Group.First.PAS

                Dim loci = Szunyi.Location.Common.GetLocation(TSS_Pos, PAS_Pos)

                Return loci
            End Function
            Public Shared Function Get_Most_Abundants(BLs As List(Of Basic_Location), Sort As Szunyi.Constants.Sort_Locations_By) As List(Of Basic_Location)

                Select Case Sort
                    Case Sort_Locations_By.TSS

                        Dim x = From t In BLs Group By t.Location.TSS, t.Location.IsComplementer Into Group Order By Group.Count Descending

                        Return x.First.Group.ToList

                    Case Sort_Locations_By.PAS
                        Dim x = From t In BLs Group By t.Location.PAS, t.Location.IsComplementer Into Group Order By Group.Count Descending

                        Return x.First.Group.ToList
                    Case Sort_Locations_By.TSS_PAS
                        Dim x = From t In BLs Group By t.Location.LocationStart, t.Location.LocationEnd, t.Location.IsComplementer Into Group Order By Group.Count Descending

                        Return x.First.Group.ToList
                    Case Sort_Locations_By.LE
                        Dim x = From t In BLs Group By t.Location.LocationEnd, t.Location.IsComplementer Into Group Order By Group.Count Descending

                        Return x.First.Group.ToList
                    Case Sort_Locations_By.TSS
                        Dim x = From t In BLs Group By t.Location.LocationStart, t.Location.IsComplementer Into Group Order By Group.Count Descending

                        Return x.First.Group.ToList
                End Select
                Return New List(Of Basic_Location)
            End Function
            Public Shared Function Get_Distribution(BLs As List(Of Basic_Location), sort As Sort_Locations_By, merging_Width As Integer) As Integer()

                Dim res(merging_Width * 2) As Integer
                Select Case sort
                    Case Sort_Locations_By.TSS

                        Dim x = From t In BLs Group By t.Location.TSS, t.Location.IsComplementer Into Group Order By Group.Count Descending

                        Dim Basic = x.First.Group.First.Location.TSS
                        For Each gr In x
                            res(gr.Group.First.Location.TSS - Basic + merging_Width) = gr.Group.Count

                        Next

                    Case Sort_Locations_By.PAS
                        Dim x = From t In BLs Group By t.Location.PAS, t.Location.IsComplementer Into Group Order By Group.Count Descending

                        Dim Basic = x.First.Group.First.Location.PAS
                        For Each gr In x
                            res(gr.Group.First.Location.PAS - Basic + merging_Width) = gr.Group.Count
                        Next

                    Case Sort_Locations_By.LE
                        Dim x = From t In BLs Group By t.Location.LocationEnd, t.Location.IsComplementer Into Group Order By Group.Count Descending

                        Dim Basic = x.First.Group.First.Location.LocationEnd
                        For Each gr In x
                            res(gr.Group.First.Location.LocationEnd - Basic + merging_Width) = gr.Group.Count

                        Next
                    Case Sort_Locations_By.TSS
                        Dim x = From t In BLs Group By t.Location.LocationStart, t.Location.IsComplementer Into Group Order By Group.Count Descending

                        Dim Basic = x.First.Group.First.Location.LocationStart
                        For Each gr In x
                            res(gr.Group.First.Location.LocationStart - Basic + merging_Width) = gr.Group.Count

                        Next
                End Select
                Return res
            End Function

        End Class

        Public Class Basic_Location
            Public Property FirstItemToInvestigate As Integer
            Public Property Count As Integer
            Public Property LastItemToInvestigate As Integer
            Public Property SeqID As String
            Public Property Start As Integer
            Public Property Endy As Integer
            Public Property IsComplement As Szunyi.Constants.Strand_Type
            Public Property Location As Bio.IO.GenBank.ILocation
            Public Property Name As String
            Public Property Extra As List(Of String)
            Public Property Ext_Key As String
            Public Property Obj As Object
            Public Property Seq As Bio.ISequence
            Public Sub New()


            End Sub

            Public Function Clone() As Basic_Location
                Dim x As New Basic_Location
                x.FirstItemToInvestigate = Me.FirstItemToInvestigate
                x.Count = Me.Count
                x.LastItemToInvestigate = Me.LastItemToInvestigate
                x.SeqID = Me.SeqID
                x.Start = Me.Start
                x.Endy = Me.Endy
                x.IsComplement = Me.IsComplement
                x.Location = Me.Location.Clone
                x.Name = Me.Name
                x.Extra = Me.Extra
                x.Ext_Key = Me.Ext_Key
                x.Obj = Me.Obj
                x.Seq = Me.Seq
                Return x
            End Function
            Public Overrides Function ToString() As String
                Return Szunyi.Location.Common.GetLocationStringTSS_PAS_Strand_Tab(Me.Location)
            End Function
        End Class

        Public Class LocationList
            Dim MyComparer As New Szunyi.Location.BioLocation_Comparers._ByStart
            Public Property Files As New List(Of FileInfo)
            Public Property UniqueID As Integer
            Public Property Locations As New List(Of Basic_Location)

            Public Property Title As String
            Public Property Log As String

            Public Property Type As String = Szunyi.Constants.BackGroundWork.Locations
            Public Property SubType As Szunyi.Constants.Location_Type
            Public Property Is_Index_Has_Done As Boolean = False
            Public Sub New(Locations As List(Of Basic_Location), Type As Szunyi.Constants.Location_Type, Title As String, IsSort As Szunyi.Constants.SOrting)
                Me.Locations = Locations

                Me.SubType = Type
                Me.Title = Title & " e:" & Locations.Count
                If IsSort = Constants.SOrting.Sorting Then
                    Me.Locations.Sort(MyComparer)
                    SetIndexes()
                End If
            End Sub
            Public Sub New(Locations As List(Of Bio.IO.GenBank.ILocation), Type As Szunyi.Constants.Location_Type, Title As String, IsSort As Szunyi.Constants.SOrting)
                Me.Locations = Szunyi.Location.Basic_Location_Manipulation.Get_Basic_LocationS(Locations)

                Me.SubType = Type
                Me.Title = Title & " e:" & Locations.Count
                If IsSort = Constants.SOrting.Sorting Then
                    Me.Locations.Sort(MyComparer)
                    SetIndexes()
                End If
            End Sub
            Public Sub Sort()
                Me.Locations.Sort(MyComparer)
                SetIndexes()
                Me.Is_Index_Has_Done = True
            End Sub

            ''' <summary>
            ''' For Clone Operation
            ''' </summary>
            Public Sub New()

            End Sub

            Private Function GetMaxLength(Locis As List(Of Szunyi.Location.Basic_Location)) As Long
                Dim MaxLength As Integer = 0
                For Each Loci In Locis
                    If Loci.Endy - Loci.Start > MaxLength Then
                        MaxLength = Loci.Endy - Loci.Start
                    End If
                Next
                Return MaxLength
            End Function
            Private Sub SetIndexes()
                If Locations.Count < 2 Then Exit Sub
                Dim MaxLength = GetMaxLength(Locations)
                Dim cSeqID As String = Locations.Last.SeqID
                For i1 = Locations.Count - 1 To 0 Step -1
                    If Locations(i1).SeqID = cSeqID Then
                        Locations(i1).FirstItemToInvestigate = i1
                        For i2 = i1 - 1 To 0 Step -1
                            If cSeqID = Locations(i2).SeqID Then
                                If Locations(i2).Endy >= Locations(i1).Start Then
                                    Locations(i1).FirstItemToInvestigate = i2
                                End If
                                If Locations(i2).Start + MaxLength < Locations(i1).Start Then
                                    Exit For
                                End If
                            Else
                                Exit For
                            End If

                        Next
                    Else
                        ' Next sequence
                        cSeqID = Locations(i1).SeqID
                        If i1 = 0 Then
                            Locations(i1).FirstItemToInvestigate = 0
                            Locations(i1).LastItemToInvestigate = 0
                            Exit For
                        Else
                            i1 += 1
                        End If
                    End If
                Next

                For i1 = 0 To Locations.Count - 1
                    Locations(i1).LastItemToInvestigate = i1
                    If Locations(i1).SeqID = cSeqID Then
                        For i2 = i1 + 1 To Locations.Count - 1
                            If Locations(i1).Endy < Locations(i2).Start Or
                                    cSeqID <> Locations(i2).SeqID Then
                                Exit For
                            Else
                                Locations(i1).LastItemToInvestigate = i2
                            End If
                        Next
                    Else
                        cSeqID = Locations(i1).SeqID
                        If i1 = Locations.Count - 1 Then
                            Locations(i1).LastItemToInvestigate = i1
                            Exit For
                        Else
                            i1 = i1 - 1
                        End If
                    End If

                Next
                Is_Index_Has_Done = True

            End Sub

            Public Function Clone() As LocationList
                Dim out As New LocationList
                out.Title = Me.Title
                out.Files = Me.Files
                out.SubType = Me.SubType

                For Each Loci In Me.Locations
                    out.Locations.Add(New Basic_Location())
                Next
                Return out
            End Function

            Public Sub Save(fOlder As String)
                Using sw As New StreamWriter(fOlder & "\" & Me.Title.Replace(":", "") & ".tdt")
                    '  sw.Write(Szunyi.Text.General.GetText(Me.Header, vbTab))
                    sw.WriteLine()
                    For Each Loci In Me.Locations
                        sw.Write(Szunyi.Text.General.GetText(Loci.Extra, vbTab))
                        sw.WriteLine()
                    Next
                End Using
            End Sub
        End Class

        Public Class LocationManipulation
            Public Shared Property Comp As New Szunyi.Location.BioLocation_Comparers.Contain_Full
            Public Shared Function Set_Start_End(Locis As List(Of ILocation), Basic As ILocation) As List(Of ILocation)
                Dim Out As New List(Of ILocation)
                For Each Loci In Locis

                    Dim Exons = Szunyi.Features.FeatureManipulation.GetLocations.Get_All_Exon_Location(Loci)
                    If Exons.Count > 1 Then
                        Dim mdExons As New List(Of ILocation)
                        For Each e In Exons
                            Dim lStart As Integer = -1
                            Dim lEnd As Integer = -1
                            If e.LocationStart = Loci.LocationStart Then
                                lStart = Basic.LocationStart
                            Else
                                lStart = e.LocationStart
                            End If
                            If e.LocationEnd = Loci.LocationEnd Then
                                lEnd = Basic.LocationEnd
                            Else
                                lEnd = e.LocationEnd
                            End If
                            Dim l = Szunyi.Location.Common.GetLocation(lStart, lEnd, e.Operator)
                            mdExons.Add(l)
                        Next
                        Dim tmp = Szunyi.Location.Common.GetLocation(mdExons, mdExons.First.Operator)
                        Dim kj = Szunyi.Location.Common.GetLocationString(tmp)
                        Out.Add(tmp)
                        Dim jkh As Int16 = 54
                    Else
                        Out.Add(Szunyi.Location.Common.Get_Location(Szunyi.Location.Common.GetLocationString(Basic)))
                    End If
                Next
                Return Out
            End Function
            Public Shared Function Merge_Locations(Feature_Locations As List(Of Szunyi.Location.LocationList)) As List(Of Szunyi.Location.Basic_Location)
                Dim Out As New List(Of Szunyi.Location.Basic_Location)
                For Each Location_List In Feature_Locations
                    Out.AddRange(Location_List.Locations)
                Next
                Return Out
            End Function
            Public Shared Function Get_Extended_Keys(All_locations As List(Of Szunyi.Location.Basic_Location)) As List(Of String)
                Dim out As New List(Of String)
                For Each Loci In All_locations
                    out.Add(Loci.Ext_Key)
                Next
                Return out
            End Function
            Public Shared Function Get_Extended_Key(loci As Szunyi.Location.Basic_Location) As String
                Return loci.Extra(5) & vbTab & loci.Extra(4) & vbTab & loci.Extra(6) & vbTab & loci.Extra(3)

            End Function
            Public Shared Function ConvertToExtFeatureLists(Seqs As List(Of ISequence), Locations As List(Of LocationList)) As Object
                Dim Out As New List(Of ExtFeatureList)
                Dim LociBuilder As New Bio.IO.GenBank.LocationBuilder
                Dim NewSeqs = Szunyi.Sequences.SequenceManipulation.Common.CloneSeqs(Seqs)
                For Each Location1 In Locations

                    For Each Loci In Location1.Locations
                        Dim Seq = Szunyi.Sequences.SequenceManipulation.GetSequences.ByID(NewSeqs, Loci.SeqID)
                        Try
                            Dim f As FeatureItem = Loci.Obj
                            Szunyi.Features.FeatureManipulation.GenBankMetaDataManipulation.AddFeature(Seq, f)
                        Catch ex As Exception

                        End Try

                    Next

                Next
                Szunyi.Features.FeatureManipulation.GenBankMetaDataManipulation.SortFeatures(NewSeqs)
                Szunyi.IO.Export.SaveSequencesToSingleGenBank(NewSeqs, Nothing, False)
                Return Nothing
            End Function

            Public Shared Function ConvertToExtFeatureLists(Feature_Locations As List(Of Szunyi.Location.LocationList))
                Dim out As New List(Of ExtFeature)
                For Each Feature_Location In Feature_Locations
                    out.AddRange(Get_ExtFeature_From_Location_list(Feature_Location))
                Next
                Return out
            End Function
            Public Shared Function Get_ExtFeature_From_Location_list(x As Szunyi.Location.LocationList) As List(Of ExtFeature)
                Dim Out As New List(Of ExtFeature)
                Dim t = GetType(Bio.IO.GenBank.FeatureItem)
                For Each Item In x.Locations
                    Try
                        Dim t1 = Item.Obj.GetType

                        If t1.Equals(t) Then
                            Dim f As Bio.IO.GenBank.FeatureItem = Item.Obj
                            Out.Add(New ExtFeature(f, Item.Seq))
                        End If

                    Catch ex As Exception

                    End Try

                Next
                Return Out
            End Function

            Public Shared Function Get_Items_Containing_Full(Locations As List(Of Basic_Location), Loci As Basic_Location) _
                As List(Of Basic_Location)
                Dim Out As New List(Of Basic_Location)
                Dim Index = Locations.BinarySearch(Loci, Comp)
                If Index > -1 Then

                    For i1 = Locations(Index).FirstItemToInvestigate To Locations(Index).LastItemToInvestigate
                        If Comp.Compare(Locations(i1), Loci) = 0 Then
                            Out.Add(Locations(i1))
                        Else
                            Dim alf As Int16 = 65
                        End If

                    Next
                    If Out.Count > 1 Then
                        Dim alf As Int16 = 54
                    End If
                End If

                Return Out
            End Function

            Public Shared Iterator Function Get_Groups_By_Extended_Key(feature_Location As LocationList) _
                As IEnumerable(Of List(Of Szunyi.Location.Basic_Location))

                Dim t = From x In feature_Location.Locations Group By x.Ext_Key Into Group

                For Each gr In t

                    Yield gr.Group.ToList
                Next
            End Function

            Public Shared Function Get_Extended_Locations(extension_Lengths As List(Of Integer), locations As List(Of LocationList)) As List(Of LocationList)
                Dim Out As New List(Of LocationList)
                For Each Loci In locations
                    For Each l In extension_Lengths
                        Dim nLoci = Loci.Clone
                        For Each Item In nLoci.Locations
                            Item.Start -= l
                            Item.Endy += l
                        Next
                        nLoci.Title = nLoci.Title & "ext By " & l
                        Out.Add(nLoci)
                    Next
                Next
                Return Out
            End Function

        End Class

        Public Class Merging
#Region "SubCLasses"
            Public Class For_Merge_Basic_Locations
                Public GroupSortedByCount As List(Of List(Of Basic_Location))
                Public GroupSortedByStart As List(Of List(Of Basic_Location))
                Public GroupSortedByEnd As List(Of List(Of Basic_Location))
                Public GroupSortedByPAS As List(Of List(Of Basic_Location))
                Public GroupSortedByTSS As List(Of List(Of Basic_Location))

                Public OneLociPerGroupByStart As List(Of Basic_Location)
                Public OneLociPerGroupPAS As List(Of Basic_Location)
                Public OneLociPerGroupTSS As List(Of Basic_Location)
                Public OneLociPerGroupEnd As List(Of Basic_Location)

                Public OneLoci_GroupSortedByStart As List(Of List(Of Basic_Location))
                Public OneLoci_GroupSortedByEnd As List(Of List(Of Basic_Location))
                Public OneLoci_GroupSortedByPAS As List(Of List(Of Basic_Location))
                Public OneLoci_GroupSortedByTSS As List(Of List(Of Basic_Location))

                Public Sub New(sorted As List(Of List(Of Basic_Location)), pos As Sort_Locations_By)
                    GroupSortedByCount = (From x In sorted Select x Order By x.Count Descending).ToList
                    Select Case pos
                        Case Sort_Locations_By.TSS_PAS
                            GroupSortedByStart = (From x In sorted Select x Order By x.First.Location.LocationStart)

                            OneLociPerGroupByStart = (From x In GroupSortedByStart Select x.First).ToList

                            GroupSortedByEnd = (From x In sorted Select x Order By x.First.Location.LocationEnd)
                            OneLociPerGroupEnd = (From x In GroupSortedByEnd Select x.First).ToList


                        Case Sort_Locations_By.LE
                            GroupSortedByEnd = sorted
                            OneLociPerGroupEnd = (From x In GroupSortedByEnd Select x.First).ToList

                        Case Sort_Locations_By.PAS

                            GroupSortedByPAS = sorted
                            GroupSortedByPAS.Sort(LociBinary.Gr_cBasic_Location_ByPAS)
                            OneLociPerGroupPAS = (From x In GroupSortedByPAS Select x.First).ToList
                            OneLociPerGroupPAS.Sort(LociBinary.cBasic_Location_ByPAS)

                        Case Sort_Locations_By.LS
                            GroupSortedByTSS = sorted
                            OneLociPerGroupByStart = (From x In GroupSortedByStart Select x.First).ToList

                        Case Sort_Locations_By.TSS
                            GroupSortedByTSS = sorted
                            GroupSortedByTSS.Sort(LociBinary.Gr_cBasic_Location_ByTSS)
                            OneLociPerGroupTSS = (From x In GroupSortedByTSS Select x.First).ToList
                            OneLociPerGroupTSS.Sort(LociBinary.cBasic_Location_ByTSS)
                            Dim t = From x In OneLociPerGroupTSS Where x.IsComplement = True

                            Dim jk As Int16 = 6
                    End Select
                End Sub
            End Class
#End Region
#Region "Merge"
            Public Shared Function MergeLocations(Feats As List(Of Bio.IO.GenBank.FeatureItem), Width As Integer, pos As Sort_Locations_By, Optional MinNof As Integer = 1) As List(Of List(Of Bio.IO.GenBank.FeatureItem))
                Dim Sorted As New List(Of List(Of Bio.IO.GenBank.FeatureItem))
                Dim out As New List(Of List(Of Bio.IO.GenBank.FeatureItem))
                Dim Locis = From x In Feats
                Select Case pos
                    Case Sort_Locations_By.TSS
                        Sorted = GroupBy_Start_End_Boths.Group_By_Locis_Start(Feats, MinNof).ToList
                    Case Sort_Locations_By.LE
                        Sorted = GroupBy_Start_End_Boths.Group_By_Locis_End(Feats, MinNof).ToList
                    Case Sort_Locations_By.TSS_PAS
                        Sorted = GroupBy_Start_End_Boths.Group_By_Locis_Start_End(Feats, MinNof).ToList

                End Select
                Dim SortedByStart = (From x In Sorted Order By x.First.Location.LocationStart).ToList
                Dim OneLociPerGroupStart = (From x In SortedByStart Select x.First).ToList
                Dim OneLociPerGroupSortedByStart = (From x In OneLociPerGroupStart Order By x.Location.LocationStart Ascending).ToList

                Dim SortedByEnd = (From x In Sorted Order By x.First.Location.LocationEnd).ToList
                Dim OneLociPerGroupEnd = (From x In SortedByEnd Select x.First).ToList
                Dim OneLociPerGroupSortedByEnd = (From x In OneLociPerGroupEnd Order By x.Location.LocationEnd Ascending).ToList
                Dim Used As New List(Of List(Of Bio.IO.GenBank.FeatureItem))
                Dim groups As New Dictionary(Of String, List(Of List(Of Bio.IO.GenBank.ILocation)))

                For Each Loci In Sorted
                    If Used.Contains(Loci) = False Then
                        Dim SameStarts As New List(Of List(Of Bio.IO.GenBank.FeatureItem))
                        Dim BinarySameStarts As New List(Of List(Of Bio.IO.GenBank.FeatureItem))
                        Dim BinarySameEnds As New List(Of List(Of Bio.IO.GenBank.FeatureItem))
                        Select Case pos
                            Case Sort_Locations_By.TSS
                                SameStarts = Get_Locis_Near._Start(Loci, Width, SortedByStart, OneLociPerGroupSortedByStart)
                            Case Sort_Locations_By.LE
                                BinarySameEnds = Get_Locis_Near._End(Loci, Width, SortedByEnd, OneLociPerGroupSortedByEnd)
                                SameStarts = Get_Locis_Near._End(Loci, Width, SortedByEnd, OneLociPerGroupSortedByEnd)
                            Case Sort_Locations_By.TSS_PAS
                                BinarySameStarts = Get_Locis_Near._Start(Loci, Width, SortedByStart, OneLociPerGroupSortedByStart)
                                SameStarts = Get_Locis_Near._End(Loci, Width, BinarySameStarts, OneLociPerGroupSortedByEnd)
                        End Select

                        Dim cLocis As New List(Of Bio.IO.GenBank.FeatureItem)
                        For Each SameStart In SameStarts
                            If Used.Contains(SameStart) = False Then
                                cLocis.AddRange(SameStart)
                                Used.Add(SameStart)

                            End If
                        Next
                        If cLocis.Count > 0 Then
                            out.Add(cLocis)
                        End If

                    Else
                        Dim alf As Integer = 43
                    End If

                Next

                Return out

            End Function
            Public Shared Function MergeLocations(Locis As List(Of Bio.IO.GenBank.ILocation), Width As Integer, pos As Sort_Locations_By, Optional MinNof As Integer = 1) _
                As List(Of List(Of Bio.IO.GenBank.ILocation))
                Dim Sorted = GroupBy(Locis, pos, MinNof, True)
                Dim out As New List(Of List(Of Bio.IO.GenBank.ILocation))

                Dim SortedByStart = (From x In Sorted Order By x.First.LocationStart).ToList
                Dim OneLociPerGroupStart = (From x In SortedByStart Select x.First).ToList
                Dim OneLociPerGroupSortedByStart = (From x In OneLociPerGroupStart Order By x.LocationStart Ascending).ToList

                Dim SortedByEnd = (From x In Sorted Order By x.First.LocationEnd).ToList
                Dim OneLociPerGroupEnd = (From x In SortedByEnd Select x.First).ToList
                Dim OneLociPerGroupSortedByEnd = (From x In OneLociPerGroupEnd Order By x.LocationEnd Ascending).ToList
                Dim Used As New List(Of List(Of Bio.IO.GenBank.ILocation))
                Dim groups As New Dictionary(Of String, List(Of List(Of Bio.IO.GenBank.ILocation)))

                For Each Loci In Sorted
                    If Used.Contains(Loci) = False Then
                        Dim SameStarts As New List(Of List(Of Bio.IO.GenBank.ILocation))
                        Dim BinarySameStarts As New List(Of List(Of Bio.IO.GenBank.ILocation))
                        Dim BinarySameEnds As New List(Of List(Of Bio.IO.GenBank.ILocation))
                        Select Case pos
                            Case Sort_Locations_By.TSS
                                SameStarts = Get_Locis_Near._Start(Loci, Width, SortedByStart, OneLociPerGroupSortedByStart)
                            Case Sort_Locations_By.LE
                                BinarySameEnds = Get_Locis_Near._End(Loci, Width, SortedByEnd, OneLociPerGroupSortedByEnd)
                                SameStarts = Get_Locis_Near._End(Loci, Width, SortedByEnd, OneLociPerGroupSortedByEnd)
                            Case Sort_Locations_By.TSS_PAS
                                BinarySameStarts = Get_Locis_Near._Start(Loci, Width, SortedByStart, OneLociPerGroupSortedByStart)
                                SameStarts = Get_Locis_Near._End(Loci, Width, BinarySameStarts)
                        End Select

                        Dim cLocis As New List(Of Bio.IO.GenBank.ILocation)
                        For Each SameStart In SameStarts
                            If Used.Contains(SameStart) = False Then
                                cLocis.AddRange(SameStart)
                                Used.Add(SameStart)

                            End If
                        Next
                        If cLocis.Count > 0 Then
                            out.Add(cLocis)
                        End If

                    Else
                        Dim alf As Integer = 43
                    End If

                Next

                Return out

            End Function
            Public Shared Function MergeLocations(Xth As Integer, Locis As List(Of Bio.IO.GenBank.ILocation), Width As Integer, pos As Sort_Locations_By, Optional MinNof As Integer = 1) _
                As List(Of List(Of Bio.IO.GenBank.ILocation))
                Dim Sorted As New List(Of List(Of Bio.IO.GenBank.ILocation))
                Dim out As New List(Of List(Of Bio.IO.GenBank.ILocation))
                Dim Xth_Introns = Szunyi.Features.FeatureManipulation.GetLocations.Get_xth_Exons_Location(Locis, Xth)
                Select Case pos
                    Case Sort_Locations_By.TSS
                        Sorted = GroupBy_Start_End_Boths.Group_By_Locis_Start_Xth(Locis, Xth, MinNof).ToList
                    Case Sort_Locations_By.LE
                        Sorted = GroupBy_Start_End_Boths.Group_By_Locis_End_xth(Locis, Xth, MinNof).ToList
                    Case Sort_Locations_By.TSS_PAS
                        Sorted = GroupBy_Start_End_Boths.Group_By_Locis_Start_End_xth(Locis, Xth, MinNof).ToList

                End Select
                Dim SortedByStart = (From x In Sorted Order By x.First.LocationStart).ToList
                Dim OneLociPerGroupStart = (From x In SortedByStart Select x.First).ToList
                Dim OneLociPerGroupSortedByStart = (From x In OneLociPerGroupStart Order By x.LocationStart Ascending).ToList

                Dim SortedByEnd = (From x In Sorted Order By x.First.LocationEnd).ToList
                Dim OneLociPerGroupEnd = (From x In SortedByEnd Select x.First).ToList
                Dim OneLociPerGroupSortedByEnd = (From x In OneLociPerGroupEnd Order By x.LocationEnd Ascending).ToList
                Dim Used As New List(Of List(Of Bio.IO.GenBank.ILocation))
                Dim groups As New Dictionary(Of String, List(Of List(Of Bio.IO.GenBank.ILocation)))

                For Each Loci In Sorted
                    If Used.Contains(Loci) = False Then
                        Dim SameStarts As New List(Of List(Of Bio.IO.GenBank.ILocation))
                        Dim BinarySameStarts As New List(Of List(Of Bio.IO.GenBank.ILocation))
                        Dim BinarySameEnds As New List(Of List(Of Bio.IO.GenBank.ILocation))
                        Select Case pos
                            Case Sort_Locations_By.TSS
                                SameStarts = Get_Locis_Near._Start_xth(Loci, Xth, Width, SortedByStart, OneLociPerGroupSortedByStart)
                            Case Sort_Locations_By.LE
                                SameStarts = Get_Locis_Near._End_xth(Loci, Xth, Width, SortedByEnd, OneLociPerGroupSortedByEnd)
                            Case Sort_Locations_By.TSS_PAS
                                BinarySameStarts = Get_Locis_Near._Start_xth(Loci, Xth, Width, SortedByStart, OneLociPerGroupSortedByStart)
                                SameStarts = Get_Locis_Near._End_xth(Loci, Xth, Width, BinarySameStarts, OneLociPerGroupSortedByEnd)
                        End Select

                        Dim cLocis As New List(Of Bio.IO.GenBank.ILocation)
                        For Each SameStart In SameStarts
                            If Used.Contains(SameStart) = False Then
                                cLocis.AddRange(SameStart)
                                Used.Add(SameStart)

                            End If
                        Next
                        If cLocis.Count > 0 Then
                            out.Add(cLocis)
                        End If

                    Else
                        Dim alf As Integer = 43
                    End If

                Next

                Return out

            End Function
            Public Shared Function MergeLocations(TSS_Or_PAS As List(Of FeatureItem), Feats As List(Of Szunyi.Location.Basic_Location),
                                                  Width As Integer, pos As Sort_Locations_By, Optional MinNof As Integer = 1) As List(Of List(Of Szunyi.Location.Basic_Location))
                Dim Sorted = GroupBy(Feats, pos, MinNof)

                Dim out As New List(Of List(Of Szunyi.Location.Basic_Location))

                Dim M As New Merging.For_Merge_Basic_Locations(Sorted, pos)

                Dim Used As New List(Of List(Of Szunyi.Location.Basic_Location))
                Dim groups As New Dictionary(Of String, List(Of List(Of Szunyi.Location.Basic_Location)))

                For Each Loci In M.GroupSortedByCount
                    If Used.Contains(Loci) = False Then
                        Dim SameStarts As New List(Of List(Of Szunyi.Location.Basic_Location))

                        Select Case pos
                            Case Sort_Locations_By.TSS
                                SameStarts = Get_Locis_Near._Start(Loci, Width, M.GroupSortedByStart, M.OneLociPerGroupByStart)
                            Case Sort_Locations_By.LE
                                SameStarts = Get_Locis_Near._End(Loci, Width, M.GroupSortedByEnd, M.OneLociPerGroupEnd)
                            Case Sort_Locations_By.TSS_PAS
                                Dim BinarySameStarts = Get_Locis_Near._Start(Loci, Width, M.GroupSortedByStart, M.OneLociPerGroupByStart)
                                SameStarts = Get_Locis_Near._End(Loci, Width, BinarySameStarts)
                            Case Sort_Locations_By.PAS
                                SameStarts = Get_Locis_Near._PAS(Loci, Width, M.GroupSortedByPAS, M.OneLociPerGroupPAS)

                            Case Sort_Locations_By.TSS
                                SameStarts = Get_Locis_Near._TSS(Loci, Width, M.GroupSortedByTSS, M.OneLociPerGroupTSS)

                        End Select

                        Dim cLocis As New List(Of Szunyi.Location.Basic_Location)

                        For Each SameStart In SameStarts
                            If Used.Contains(SameStart) = False Then
                                cLocis.AddRange(SameStart)
                                Used.Add(SameStart)
                            End If
                        Next
                        If cLocis.Count > 0 Then
                            out.Add(cLocis)
                        End If

                    End If

                Next

                Return out

            End Function

            Public Shared Function MergeLocations(Feats As List(Of Szunyi.Location.Basic_Location),
                                                  Width As Integer, pos As Sort_Locations_By, Optional MinNof As Integer = 1) As List(Of List(Of Szunyi.Location.Basic_Location))
                Dim Sorted = GroupBy(Feats, pos, MinNof) ' Exact

                Dim out As New List(Of List(Of Szunyi.Location.Basic_Location))

                Dim M As New Merging.For_Merge_Basic_Locations(Sorted, pos)

                Dim Used As New List(Of List(Of Szunyi.Location.Basic_Location))
                Dim groups As New Dictionary(Of String, List(Of List(Of Szunyi.Location.Basic_Location)))

                For Each Loci In M.GroupSortedByCount
                    If Used.Contains(Loci) = False Then
                        Dim SameStarts As New List(Of List(Of Szunyi.Location.Basic_Location))

                        Select Case pos
                            Case Sort_Locations_By.LS
                                SameStarts = Get_Locis_Near._Start(Loci, Width, M.GroupSortedByStart, M.OneLociPerGroupByStart)
                            Case Sort_Locations_By.LE
                                SameStarts = Get_Locis_Near._End(Loci, Width, M.GroupSortedByEnd, M.OneLociPerGroupEnd)
                            Case Sort_Locations_By.TSS_PAS
                                Dim BinarySameStarts = Get_Locis_Near._Start(Loci, Width, M.GroupSortedByStart, M.OneLociPerGroupByStart)
                                SameStarts = Get_Locis_Near._End(Loci, Width, BinarySameStarts)
                            Case Sort_Locations_By.PAS
                                SameStarts = Get_Locis_Near._PAS(Loci, Width, M.GroupSortedByPAS, M.OneLociPerGroupPAS)

                            Case Sort_Locations_By.TSS
                                SameStarts = Get_Locis_Near._TSS(Loci, Width, M.GroupSortedByTSS, M.OneLociPerGroupTSS)

                        End Select

                        Dim cLocis As New List(Of Szunyi.Location.Basic_Location)

                        For Each SameStart In SameStarts
                            If Used.Contains(SameStart) = False Then
                                cLocis.AddRange(SameStart)
                                Used.Add(SameStart)
                            End If
                        Next
                        If cLocis.Count > 0 Then
                            out.Add(cLocis)
                        End If

                    End If

                Next

                Return out

            End Function

#End Region
#Region "Group By"
            Public Shared Function GroupBy(Locis As List(Of FeatureItem), pos As Sort_Locations_By, MinNof As Integer, wOrientation As Boolean) As List(Of List(Of FeatureItem))
                Select Case pos
                    Case Sort_Locations_By.TSS
                    '    Return GroupBy_Start_End_Boths.Group_By_Locis_Start(Locis, MinNof, wOrientation).ToList
                    Case Sort_Locations_By.LE
                  '      Return GroupBy_Start_End_Boths.Group_By_Locis_End(Locis, MinNof, wOrientation).ToList
                    Case Sort_Locations_By.TSS_PAS
                '        Return GroupBy_Start_End_Boths.Group_By_Locis_Start_End(Locis, MinNof, wOrientation).ToList
                    Case Sort_Locations_By.TSS
               '         Return GroupBy_Start_End_Boths.Group_By_Locis_TSS(Locis, MinNof, wOrientation).ToList
                    Case Sort_Locations_By.PAS
                        '           Return GroupBy_Start_End_Boths.Group_By_Locis_PAS(Locis, MinNof, wOrientation).ToList
                End Select
                Return Nothing
            End Function
            Public Shared Function GroupBy(Locis As List(Of ILocation), pos As Sort_Locations_By, MinNof As Integer, wOrientation As Boolean) As List(Of List(Of ILocation))
                Select Case pos
                    Case Sort_Locations_By.TSS
                        Return GroupBy_Start_End_Boths.Group_By_Locis_Start(Locis, MinNof, wOrientation).ToList
                    Case Sort_Locations_By.LE
                        Return GroupBy_Start_End_Boths.Group_By_Locis_End(Locis, MinNof, wOrientation).ToList
                    Case Sort_Locations_By.TSS_PAS
                        Return GroupBy_Start_End_Boths.Group_By_Locis_Start_End(Locis, MinNof, wOrientation).ToList
                    Case Sort_Locations_By.TSS
                        Return GroupBy_Start_End_Boths.Group_By_Locis_TSS(Locis, MinNof, wOrientation).ToList
                    Case Sort_Locations_By.PAS
                        Return GroupBy_Start_End_Boths.Group_By_Locis_PAS(Locis, MinNof, wOrientation).ToList
                End Select
                Return Nothing
            End Function
            Public Shared Function GroupBy(Locis As List(Of Szunyi.Location.Basic_Location), pos As Sort_Locations_By, MinNof As Integer) As List(Of List(Of Szunyi.Location.Basic_Location))
                Select Case pos
                    Case Sort_Locations_By.TSS
                        Return GroupBy_Start_End_Boths.Group_By_Locis_Start(Locis, MinNof).ToList
                    Case Sort_Locations_By.LE
                        Return GroupBy_Start_End_Boths.Group_By_Locis_End(Locis, MinNof).ToList
                    Case Sort_Locations_By.TSS_PAS
                        Return GroupBy_Start_End_Boths.Group_By_Locis_Start_End(Locis, MinNof).ToList
                    Case Sort_Locations_By.TSS
                        Return GroupBy_Start_End_Boths.Group_By_Locis_TSS(Locis, MinNof).ToList
                    Case Sort_Locations_By.PAS
                        Return GroupBy_Start_End_Boths.Group_By_Locis_PAS(Locis, MinNof).ToList
                End Select
                Return Nothing
            End Function

#End Region

        End Class

        Public Class Get_Locis_Near
#Region "Basic_Location"
            Public Shared Function _Start_End(Loci As List(Of Szunyi.Location.Basic_Location), width As Integer, sorted As List(Of List(Of Szunyi.Location.Basic_Location))) As List(Of List(Of Szunyi.Location.Basic_Location))
                Dim NearStarts = From x In sorted Where x.First.Location.Operator = Loci.First.Location.Operator And x.First.Location.LocationStart > Loci.First.Location.LocationStart - width And x.First.Location.LocationStart < Loci.First.Location.LocationStart + width And x.First.Location.LocationEnd > Loci.First.Location.LocationEnd - width And x.First.Location.LocationEnd < Loci.First.Location.LocationEnd + width

                Dim Out As New List(Of List(Of Szunyi.Location.Basic_Location))

                For Each NearStart In NearStarts

                    Out.Add(NearStart)

                Next

                Return Out
            End Function
            Public Shared Function _Start(Loci As List(Of Szunyi.Location.Basic_Location), width As Integer, sorted As List(Of List(Of Szunyi.Location.Basic_Location)),
                                                        Locis As List(Of Szunyi.Location.Basic_Location)) As List(Of List(Of Szunyi.Location.Basic_Location))

                Dim mdLoci = Basic_Location_Manipulation.Get_Basic_Location(Szunyi.Features.FeatureManipulation.GetLocations.LociBuilder.GetLocation(Loci.First.Location.LocationStart - width & ".." & Loci.First.Location.LocationStart + width))
                Dim Index = Locis.BinarySearch(Loci.First, LociBinary.cBasic_Location_ByStart_wStrand)
                Dim out As New List(Of List(Of Szunyi.Location.Basic_Location))
                If Index > -1 Then
                    For i1 = Index To 0 Step -1
                        If Locis(i1).Location.LocationStart >= mdLoci.Location.LocationStart AndAlso
                            Locis(i1).Location.LocationStart <= mdLoci.Location.LocationEnd AndAlso Locis(i1).Location.Operator = Loci.First.Location.Operator Then
                            out.Add(sorted(i1))
                        ElseIf Locis(i1).Location.LocationStart >= mdLoci.Location.LocationStart AndAlso Locis(i1).Location.LocationStart <= mdLoci.Location.LocationEnd Then

                        Else

                            Exit For
                        End If
                    Next
                    For i1 = Index + 1 To Locis.Count - 1
                        If Locis(i1).Location.LocationStart >= mdLoci.Location.LocationStart AndAlso
                            Locis(i1).Location.LocationStart <= mdLoci.Location.LocationEnd AndAlso Locis(i1).Location.Operator = Loci.First.Location.Operator Then
                            out.Add(sorted(i1))
                        ElseIf Locis(i1).Location.LocationStart >= mdLoci.Location.LocationStart AndAlso Locis(i1).Location.LocationStart <= mdLoci.Location.LocationEnd Then
                        Else
                            Exit For
                        End If
                    Next
                Else
                    Dim alf As Int16 = 43
                End If

                Return out
            End Function
            Public Shared Function _TSS(Loci As List(Of Szunyi.Location.Basic_Location), width As Integer, sorted As List(Of List(Of Szunyi.Location.Basic_Location)),
                                                        Locis As List(Of Szunyi.Location.Basic_Location)) As List(Of List(Of Szunyi.Location.Basic_Location))

                Dim mdLoci = Basic_Location_Manipulation.Get_Basic_Location(Szunyi.Features.FeatureManipulation.GetLocations.LociBuilder.GetLocation(Loci.First.Location.TSS - width & ".." & Loci.First.Location.TSS + width))
                Dim d As New TSS_PAS_Comparers.Basic_Location_ByTSS
                Dim Index = Locis.BinarySearch(Loci.First, d)
                Dim out As New List(Of List(Of Szunyi.Location.Basic_Location))
                If Index < 0 Then
                    Dim kj As Int16 = 54
                End If
                If Index > -1 Then
                    For i1 = Index To 0 Step -1
                        If Locis(i1).Location.TSS >= mdLoci.Location.LocationStart AndAlso
                            Locis(i1).Location.TSS <= mdLoci.Location.LocationEnd AndAlso Locis(i1).Location.Operator = Loci.First.Location.Operator Then
                            out.Add(sorted(i1))
                        ElseIf Locis(i1).Location.TSS >= mdLoci.Location.LocationStart AndAlso Locis(i1).Location.TSS <= mdLoci.Location.LocationEnd Then

                        Else

                            Exit For
                        End If
                    Next
                    For i1 = Index + 1 To Locis.Count - 1
                        If Locis(i1).Location.TSS >= mdLoci.Location.LocationStart AndAlso
                            Locis(i1).Location.TSS <= mdLoci.Location.LocationEnd AndAlso Locis(i1).Location.Operator = Loci.First.Location.Operator Then
                            out.Add(sorted(i1))
                        ElseIf Locis(i1).Location.TSS >= mdLoci.Location.LocationStart AndAlso Locis(i1).Location.TSS <= mdLoci.Location.LocationEnd Then
                        Else
                            Exit For
                        End If
                    Next

                End If

                Return out
            End Function
            Public Shared Function _PAS(Loci As List(Of Szunyi.Location.Basic_Location), width As Integer, sorted As List(Of List(Of Szunyi.Location.Basic_Location)),
                                                        Locis As List(Of Szunyi.Location.Basic_Location)) As List(Of List(Of Szunyi.Location.Basic_Location))

                Dim mdLoci = Basic_Location_Manipulation.Get_Basic_Location(Szunyi.Features.FeatureManipulation.GetLocations.LociBuilder.GetLocation(Loci.First.Location.PAS - width & ".." & Loci.First.Location.PAS + width))
                Dim Index = Locis.BinarySearch(Loci.First, LociBinary.cBasic_Location_ByPAS)
                Dim out As New List(Of List(Of Szunyi.Location.Basic_Location))
                If Index > -1 Then
                    For i1 = Index To 0 Step -1
                        If Locis(i1).Location.PAS >= mdLoci.Location.LocationStart AndAlso
                            Locis(i1).Location.PAS <= mdLoci.Location.LocationEnd AndAlso Locis(i1).Location.IsComplementer = Loci.First.Location.IsComplementer Then
                            out.Add(sorted(i1))
                        Else
                            Exit For
                        End If
                    Next
                    For i1 = Index + 1 To Locis.Count - 1
                        If Locis(i1).Location.PAS >= mdLoci.Location.LocationStart AndAlso
                            Locis(i1).Location.PAS <= mdLoci.Location.LocationEnd AndAlso Locis(i1).Location.IsComplementer = Loci.First.Location.IsComplementer Then
                            out.Add(sorted(i1))
                        Else
                            Exit For
                        End If
                    Next
                Else
                    Dim kj As Int16 = 54
                End If

                Return out
            End Function
            Public Shared Function _Start_xth(Loci As List(Of Szunyi.Location.Basic_Location), xth As Integer, width As Integer, sorted As List(Of List(Of Szunyi.Location.Basic_Location)),
                                                        Locis As List(Of Szunyi.Location.Basic_Location)) As List(Of List(Of Szunyi.Location.Basic_Location))
                Dim xth_loci = Szunyi.Features.FeatureManipulation.GetLocations.Get_XTH_Exon_Location(Loci.First.Location, xth)
                Dim out As New List(Of List(Of Szunyi.Location.Basic_Location))
                For Each item In sorted
                    Dim xth_exon = Szunyi.Features.FeatureManipulation.GetLocations.Get_XTH_Exon_Location(item.First.Location, xth)
                    If xth_exon.LocationStart >= xth_loci.LocationStart - width AndAlso xth_exon.LocationStart <= xth_loci.LocationStart + width Then
                        out.Add(item)
                    End If

                Next
                Return out
            End Function
            Public Shared Function _End_xth(Loci As List(Of Szunyi.Location.Basic_Location), xth As Integer, width As Integer, sorted As List(Of List(Of Szunyi.Location.Basic_Location)), Locis As List(Of Szunyi.Location.Basic_Location)) As List(Of List(Of Szunyi.Location.Basic_Location))
                Dim xth_loci = Szunyi.Features.FeatureManipulation.GetLocations.Get_XTH_Exon_Location(Loci.First.Location, xth)
                Dim out As New List(Of List(Of Szunyi.Location.Basic_Location))
                For Each item In sorted
                    Dim xth_exon = Szunyi.Features.FeatureManipulation.GetLocations.Get_XTH_Exon_Location(item.First.Location, xth)
                    If xth_exon.LocationEnd >= xth_loci.LocationEnd - width AndAlso xth_exon.LocationEnd <= xth_loci.LocationEnd + width Then
                        out.Add(item)
                    End If

                Next
                Return out
            End Function
            Public Shared Function _End(Loci As List(Of Szunyi.Location.Basic_Location), width As Integer, sorted As List(Of List(Of Szunyi.Location.Basic_Location))) As List(Of List(Of Szunyi.Location.Basic_Location))
                Dim NearStarts = From x In sorted Where x.First.Location.Operator = Loci.First.Location.Operator And x.First.Location.LocationEnd > Loci.First.Location.LocationEnd - width And x.First.Location.LocationEnd < Loci.First.Location.LocationEnd + width And x.First.Location.LocationEnd > Loci.First.Location.LocationEnd - width And x.First.Location.LocationEnd < Loci.First.Location.LocationEnd + width

                Dim Out As New List(Of List(Of Szunyi.Location.Basic_Location))

                For Each NearStart In NearStarts

                    Out.Add(NearStart)

                Next

                Return Out
            End Function
            Public Shared Function _End(Loci As List(Of Szunyi.Location.Basic_Location), width As Integer, sorted As List(Of List(Of Szunyi.Location.Basic_Location)), Locis As List(Of Szunyi.Location.Basic_Location)) As List(Of List(Of Szunyi.Location.Basic_Location))
                Dim mdLoci = Basic_Location_Manipulation.Get_Basic_Location(Szunyi.Features.FeatureManipulation.GetLocations.LociBuilder.GetLocation(Loci.First.Location.LocationStart - width & ".." & Loci.First.Location.LocationStart + width))

                Dim Index = Locis.BinarySearch(mdLoci, LociBinary.cBasic_Location_ByEnd_wStrand)
                Dim out As New List(Of List(Of Szunyi.Location.Basic_Location))
                With mdLoci.Location
                    If Index > -1 Then
                        For i1 = Index To 0 Step -1
                            If Locis(i1).Location.LocationEnd >= .LocationStart AndAlso Locis(i1).Location.LocationEnd <= .LocationEnd AndAlso Locis(i1).Location.Operator = Loci.First.Location.Operator Then
                                out.Add(sorted(i1))
                            Else
                                Exit For
                            End If
                        Next
                        For i1 = Index + 1 To Locis.Count - 1
                            If Locis(i1).Location.LocationEnd >= .LocationStart AndAlso Locis(i1).Location.LocationEnd <= .LocationEnd AndAlso Locis(i1).Location.Operator = Loci.First.Location.Operator Then
                                out.Add(sorted(i1))
                            Else
                                Exit For
                            End If
                        Next
                    Else
                        Dim alf As Int16 = 43
                    End If
                End With


                Return out
            End Function

            Public Shared Function _Start_End(Loci As Szunyi.Location.Basic_Location, width As Integer, sorted As List(Of List(Of Szunyi.Location.Basic_Location))) As List(Of List(Of Szunyi.Location.Basic_Location))
                Dim NearStarts = From x In sorted Where x.First.Location.Operator = Loci.Location.Operator And x.First.Location.LocationStart > Loci.Location.LocationStart - width And x.First.Location.LocationStart < Loci.Location.LocationStart + width And x.First.Location.LocationEnd > Loci.Location.LocationEnd - width And x.First.Location.LocationEnd < Loci.Location.LocationEnd + width

                Dim Out As New List(Of List(Of Szunyi.Location.Basic_Location))

                For Each NearStart In NearStarts

                    Out.Add(NearStart)

                Next

                Return Out
            End Function
            Public Shared Function _Start(Loci As Szunyi.Location.Basic_Location, width As Integer, sorted As List(Of List(Of Szunyi.Location.Basic_Location)),
                                                        Locis As List(Of Szunyi.Location.Basic_Location)) As List(Of List(Of Szunyi.Location.Basic_Location))

                Dim mdLoci = Basic_Location_Manipulation.Get_Basic_Location(Szunyi.Features.FeatureManipulation.GetLocations.LociBuilder.GetLocation(Loci.Location.LocationStart - width & ".." & Loci.Location.LocationStart + width))
                Dim Index = Locis.BinarySearch(Loci, LociBinary.cBasic_Location_ByStart_wStrand)
                Dim out As New List(Of List(Of Szunyi.Location.Basic_Location))
                If Index > -1 Then
                    For i1 = Index To 0 Step -1
                        If Locis(i1).Location.LocationStart >= mdLoci.Location.LocationStart AndAlso
                            Locis(i1).Location.LocationStart <= mdLoci.Location.LocationEnd AndAlso Locis(i1).Location.Operator = Loci.Location.Operator Then
                            out.Add(sorted(i1))
                        ElseIf Locis(i1).Location.LocationStart >= mdLoci.Location.LocationStart AndAlso Locis(i1).Location.LocationStart <= mdLoci.Location.LocationEnd Then

                        Else

                            Exit For
                        End If
                    Next
                    For i1 = Index + 1 To Locis.Count - 1
                        If Locis(i1).Location.LocationStart >= mdLoci.Location.LocationStart AndAlso
                            Locis(i1).Location.LocationStart <= mdLoci.Location.LocationEnd AndAlso Locis(i1).Location.Operator = Loci.Location.Operator Then
                            out.Add(sorted(i1))
                        ElseIf Locis(i1).Location.LocationStart >= mdLoci.Location.LocationStart AndAlso Locis(i1).Location.LocationStart <= mdLoci.Location.LocationEnd Then
                        Else
                            Exit For
                        End If
                    Next
                Else
                    Dim alf As Int16 = 43
                End If

                Return out
            End Function
            Public Shared Function _TSS(Loci As Szunyi.Location.Basic_Location, width As Integer, sorted As List(Of List(Of Szunyi.Location.Basic_Location)),
                                                        Locis As List(Of Szunyi.Location.Basic_Location)) As List(Of List(Of Szunyi.Location.Basic_Location))

                Dim mdLoci = Basic_Location_Manipulation.Get_Basic_Location(Szunyi.Features.FeatureManipulation.GetLocations.LociBuilder.GetLocation(Loci.Location.TSS - width & ".." & Loci.Location.TSS + width))
                Dim Index = Locis.BinarySearch(Loci, LociBinary.cBasic_Location_ByTSS)
                Dim out As New List(Of List(Of Szunyi.Location.Basic_Location))

                If Index > -1 Then
                    For i1 = Index To 0 Step -1
                        If Locis(i1).Location.TSS >= mdLoci.Location.LocationStart AndAlso
                            Locis(i1).Location.TSS <= mdLoci.Location.LocationEnd AndAlso Locis(i1).Location.Operator = Loci.Location.Operator Then
                            out.Add(sorted(i1))
                        ElseIf Locis(i1).Location.TSS >= mdLoci.Location.LocationStart AndAlso Locis(i1).Location.TSS <= mdLoci.Location.LocationEnd Then

                        Else

                            Exit For
                        End If
                    Next
                    For i1 = Index + 1 To Locis.Count - 1
                        If Locis(i1).Location.TSS >= mdLoci.Location.LocationStart AndAlso
                            Locis(i1).Location.TSS <= mdLoci.Location.LocationEnd AndAlso Locis(i1).Location.Operator = Loci.Location.Operator Then
                            out.Add(sorted(i1))
                        ElseIf Locis(i1).Location.TSS >= mdLoci.Location.LocationStart AndAlso Locis(i1).Location.TSS <= mdLoci.Location.LocationEnd Then
                        Else
                            Exit For
                        End If
                    Next

                End If

                Return out
            End Function
            Public Shared Function _PAS(Loci As Szunyi.Location.Basic_Location, width As Integer, sorted As List(Of List(Of Szunyi.Location.Basic_Location)),
                                                        Locis As List(Of Szunyi.Location.Basic_Location)) As List(Of List(Of Szunyi.Location.Basic_Location))

                Dim mdLoci = Basic_Location_Manipulation.Get_Basic_Location(Szunyi.Features.FeatureManipulation.GetLocations.LociBuilder.GetLocation(Loci.Location.PAS - width & ".." & Loci.Location.PAS + width))
                Dim Index = Locis.BinarySearch(Loci, LociBinary.cBasic_Location_ByPAS)
                Dim out As New List(Of List(Of Szunyi.Location.Basic_Location))
                If Index > -1 Then
                    For i1 = Index To 0 Step -1
                        If Locis(i1).Location.PAS >= mdLoci.Location.LocationStart AndAlso
                            Locis(i1).Location.PAS <= mdLoci.Location.LocationEnd AndAlso Locis(i1).Location.IsComplementer = Loci.Location.IsComplementer Then
                            out.Add(sorted(i1))
                        Else
                            Exit For
                        End If
                    Next
                    For i1 = Index + 1 To Locis.Count - 1
                        If Locis(i1).Location.PAS >= mdLoci.Location.LocationStart AndAlso
                            Locis(i1).Location.PAS <= mdLoci.Location.LocationEnd AndAlso Locis(i1).Location.IsComplementer = Loci.Location.IsComplementer Then
                            out.Add(sorted(i1))
                        Else
                            Exit For
                        End If
                    Next
                Else
                    Dim kj As Int16 = 54
                End If

                Return out
            End Function
            Public Shared Function _Start_xth(Loci As Szunyi.Location.Basic_Location, xth As Integer, width As Integer, sorted As List(Of List(Of Szunyi.Location.Basic_Location)),
                                                        Locis As List(Of Szunyi.Location.Basic_Location)) As List(Of List(Of Szunyi.Location.Basic_Location))
                Dim xth_loci = Szunyi.Features.FeatureManipulation.GetLocations.Get_XTH_Exon_Location(Loci.Location, xth)
                Dim out As New List(Of List(Of Szunyi.Location.Basic_Location))
                For Each item In sorted
                    Dim xth_exon = Szunyi.Features.FeatureManipulation.GetLocations.Get_XTH_Exon_Location(item.First.Location, xth)
                    If xth_exon.LocationStart >= xth_loci.LocationStart - width AndAlso xth_exon.LocationStart <= xth_loci.LocationStart + width Then
                        out.Add(item)
                    End If

                Next
                Return out
            End Function
            Public Shared Function _End_xth(Loci As Szunyi.Location.Basic_Location, xth As Integer, width As Integer, sorted As List(Of List(Of Szunyi.Location.Basic_Location)), Locis As List(Of Szunyi.Location.Basic_Location)) As List(Of List(Of Szunyi.Location.Basic_Location))
                Dim xth_loci = Szunyi.Features.FeatureManipulation.GetLocations.Get_XTH_Exon_Location(Loci.Location, xth)
                Dim out As New List(Of List(Of Szunyi.Location.Basic_Location))
                For Each item In sorted
                    Dim xth_exon = Szunyi.Features.FeatureManipulation.GetLocations.Get_XTH_Exon_Location(item.First.Location, xth)
                    If xth_exon.LocationEnd >= xth_loci.LocationEnd - width AndAlso xth_exon.LocationEnd <= xth_loci.LocationEnd + width Then
                        out.Add(item)
                    End If

                Next
                Return out
            End Function
            Public Shared Function _End(Loci As Szunyi.Location.Basic_Location, width As Integer, sorted As List(Of List(Of Szunyi.Location.Basic_Location))) As List(Of List(Of Szunyi.Location.Basic_Location))
                Dim NearStarts = From x In sorted Where x.First.Location.Operator = Loci.Location.Operator And x.First.Location.LocationEnd > Loci.Location.LocationEnd - width And x.First.Location.LocationEnd < Loci.Location.LocationEnd + width And x.First.Location.LocationEnd > Loci.Location.LocationEnd - width And x.First.Location.LocationEnd < Loci.Location.LocationEnd + width

                Dim Out As New List(Of List(Of Szunyi.Location.Basic_Location))

                For Each NearStart In NearStarts

                    Out.Add(NearStart)

                Next

                Return Out
            End Function
            Public Shared Function _End(Loci As Szunyi.Location.Basic_Location, width As Integer, sorted As List(Of List(Of Szunyi.Location.Basic_Location)), Locis As List(Of Szunyi.Location.Basic_Location)) As List(Of List(Of Szunyi.Location.Basic_Location))
                Dim mdLoci = Basic_Location_Manipulation.Get_Basic_Location(Szunyi.Features.FeatureManipulation.GetLocations.LociBuilder.GetLocation(Loci.Location.LocationStart - width & ".." & Loci.Location.LocationStart + width))

                Dim Index = Locis.BinarySearch(mdLoci, LociBinary.cBasic_Location_ByEnd_wStrand)
                Dim out As New List(Of List(Of Szunyi.Location.Basic_Location))
                With mdLoci.Location
                    If Index > -1 Then
                        For i1 = Index To 0 Step -1
                            If Locis(i1).Location.LocationEnd >= .LocationStart AndAlso Locis(i1).Location.LocationEnd <= .LocationEnd AndAlso Locis(i1).Location.Operator = Loci.Location.Operator Then
                                out.Add(sorted(i1))
                            Else
                                Exit For
                            End If
                        Next
                        For i1 = Index + 1 To Locis.Count - 1
                            If Locis(i1).Location.LocationEnd >= .LocationStart AndAlso Locis(i1).Location.LocationEnd <= .LocationEnd AndAlso Locis(i1).Location.Operator = Loci.Location.Operator Then
                                out.Add(sorted(i1))
                            Else
                                Exit For
                            End If
                        Next
                    Else
                        Dim alf As Int16 = 43
                    End If
                End With


                Return out
            End Function

#End Region
#Region "Location"
            Public Shared Function _Start_End(Loci As List(Of Bio.IO.GenBank.ILocation), width As Integer, sorted As List(Of List(Of Bio.IO.GenBank.ILocation))) As List(Of List(Of Bio.IO.GenBank.ILocation))
                Dim NearStarts = From x In sorted Where x.First.Operator = Loci.First.Operator And x.First.LocationStart > Loci.First.LocationStart - width And x.First.LocationStart < Loci.First.LocationStart + width And x.First.LocationEnd > Loci.First.LocationEnd - width And x.First.LocationEnd < Loci.First.LocationEnd + width

                Dim Out As New List(Of List(Of Bio.IO.GenBank.ILocation))

                For Each NearStart In NearStarts

                    Out.Add(NearStart)

                Next

                Return Out
            End Function
            Public Shared Function _Start(Loci As List(Of Bio.IO.GenBank.ILocation), width As Integer, sorted As List(Of List(Of Bio.IO.GenBank.ILocation)),
                                                        Locis As List(Of Bio.IO.GenBank.ILocation)) As List(Of List(Of Bio.IO.GenBank.ILocation))

                Dim mdLoci = Szunyi.Features.FeatureManipulation.GetLocations.LociBuilder.GetLocation(Loci.First.LocationStart - width & ".." & Loci.First.LocationStart + width)
                Dim Index = Locis.BinarySearch(mdLoci, LociBinary.cBio_Location_ByStart)
                Dim out As New List(Of List(Of Bio.IO.GenBank.ILocation))
                If Index > -1 Then
                    For i1 = Index To 0 Step -1
                        If Locis(i1).LocationStart >= mdLoci.LocationStart AndAlso Locis(i1).LocationStart <= mdLoci.LocationEnd AndAlso Locis(i1).Operator = Loci.FirstOrDefault.Operator Then
                            out.Add(sorted(i1))
                        ElseIf Locis(i1).LocationStart >= mdLoci.LocationStart AndAlso Locis(i1).LocationStart <= mdLoci.LocationEnd Then

                        Else

                            Exit For
                        End If
                    Next
                    For i1 = Index + 1 To Locis.Count - 1
                        If Locis(i1).LocationStart >= mdLoci.LocationStart AndAlso Locis(i1).LocationStart <= mdLoci.LocationEnd AndAlso Locis(i1).Operator = Loci.FirstOrDefault.Operator Then
                            out.Add(sorted(i1))
                        ElseIf Locis(i1).LocationStart >= mdLoci.LocationStart AndAlso Locis(i1).LocationStart <= mdLoci.LocationEnd Then
                        Else
                            Exit For
                        End If
                    Next
                Else
                    Dim alf As Int16 = 43
                End If

                Return out
            End Function
            Public Shared Function _Start_xth(Loci As List(Of Bio.IO.GenBank.ILocation), xth As Integer, width As Integer, sorted As List(Of List(Of Bio.IO.GenBank.ILocation)),
                                                        Locis As List(Of Bio.IO.GenBank.ILocation)) As List(Of List(Of Bio.IO.GenBank.ILocation))
                Dim xth_loci = Szunyi.Features.FeatureManipulation.GetLocations.Get_XTH_Exon_Location(Loci.First, xth)
                Dim out As New List(Of List(Of Bio.IO.GenBank.ILocation))
                For Each item In sorted
                    Dim xth_exon = Szunyi.Features.FeatureManipulation.GetLocations.Get_XTH_Exon_Location(item.First, xth)
                    If xth_exon.LocationStart >= xth_loci.LocationStart - width AndAlso xth_exon.LocationStart <= xth_loci.LocationStart + width Then
                        out.Add(item)
                    End If

                Next
                Return out
            End Function
            Public Shared Function _End_xth(Loci As List(Of Bio.IO.GenBank.ILocation), xth As Integer, width As Integer, sorted As List(Of List(Of Bio.IO.GenBank.ILocation)), Locis As List(Of Bio.IO.GenBank.ILocation)) As List(Of List(Of Bio.IO.GenBank.ILocation))
                Dim xth_loci = Szunyi.Features.FeatureManipulation.GetLocations.Get_XTH_Exon_Location(Loci.First, xth)
                Dim out As New List(Of List(Of Bio.IO.GenBank.ILocation))
                For Each item In sorted
                    Dim xth_exon = Szunyi.Features.FeatureManipulation.GetLocations.Get_XTH_Exon_Location(item.First, xth)
                    If xth_exon.LocationEnd >= xth_loci.LocationEnd - width AndAlso xth_exon.LocationEnd <= xth_loci.LocationEnd + width Then
                        out.Add(item)
                    End If

                Next
                Return out
            End Function
            Public Shared Function _End(Loci As List(Of Bio.IO.GenBank.ILocation), width As Integer, sorted As List(Of List(Of Bio.IO.GenBank.ILocation))) As List(Of List(Of Bio.IO.GenBank.ILocation))
                Dim NearStarts = From x In sorted Where x.First.Operator = Loci.First.Operator And x.First.LocationEnd >= Loci.First.LocationEnd - width And x.First.LocationEnd <= Loci.First.LocationEnd + width

                Dim Out As New List(Of List(Of Bio.IO.GenBank.ILocation))

                For Each NearStart In NearStarts

                    Out.Add(NearStart)

                Next

                Return Out
            End Function
            Public Shared Function _End(Loci As List(Of Bio.IO.GenBank.ILocation), width As Integer, sorted As List(Of List(Of Bio.IO.GenBank.ILocation)), Locis As List(Of Bio.IO.GenBank.ILocation)) As List(Of List(Of Bio.IO.GenBank.ILocation))
                Dim mdLoci = Szunyi.Features.FeatureManipulation.GetLocations.LociBuilder.GetLocation(Loci.First.LocationEnd - width & ".." & Loci.First.LocationEnd + width)
                Dim Index = Locis.BinarySearch(mdLoci, LociBinary.cBio_Location_ByEnd)
                Dim out As New List(Of List(Of Bio.IO.GenBank.ILocation))
                If Index > -1 Then
                    For i1 = Index To 0 Step -1
                        If Locis(i1).LocationEnd >= mdLoci.LocationStart AndAlso Locis(i1).LocationEnd <= mdLoci.LocationEnd AndAlso Locis(i1).Operator = Loci.First.Operator Then
                            out.Add(sorted(i1))
                        Else
                            Exit For
                        End If
                    Next
                    For i1 = Index + 1 To Locis.Count - 1
                        If Locis(i1).LocationEnd >= mdLoci.LocationStart AndAlso Locis(i1).LocationEnd <= mdLoci.LocationEnd AndAlso Locis(i1).Operator = Loci.First.Operator Then
                            out.Add(sorted(i1))
                        Else
                            Exit For
                        End If
                    Next
                Else
                    Dim alf As Int16 = 43
                End If

                Return out
            End Function

#End Region
#Region "Features"
            Public Shared Function _Start_End(Loci As List(Of Bio.IO.GenBank.FeatureItem), width As Integer, sorted As List(Of List(Of Bio.IO.GenBank.FeatureItem))) As List(Of List(Of Bio.IO.GenBank.FeatureItem))
                Dim NearStarts = From x In sorted Where x.FirstOrDefault.Location.Operator = Loci.FirstOrDefault.Location.Operator And x.First.Location.LocationStart > Loci.First.Location.LocationStart - width And
                                                      x.First.Location.LocationStart < Loci.First.Location.LocationStart + width And x.First.Location.LocationEnd > Loci.First.Location.LocationEnd - width And x.First.Location.LocationEnd < Loci.First.Location.LocationEnd + width

                Dim Out As New List(Of List(Of Bio.IO.GenBank.FeatureItem))

                For Each NearStart In NearStarts

                    Out.Add(NearStart)

                Next

                Return Out
            End Function
            Public Shared Function _Start(Loci As List(Of Bio.IO.GenBank.FeatureItem), width As Integer, sorted As List(Of List(Of Bio.IO.GenBank.FeatureItem)),
                                                        Locis As List(Of Bio.IO.GenBank.FeatureItem)) As List(Of List(Of Bio.IO.GenBank.FeatureItem))

                Dim mdLoci = Szunyi.Features.FeatureManipulation.GetLocations.LociBuilder.GetLocation(Loci.First.Location.LocationStart - width & ".." & Loci.First.Location.LocationStart + width)
                Dim Index = Locis.BinarySearch(mdLoci, LociBinary.cFeature_Location_ByStart)
                Dim out As New List(Of List(Of Bio.IO.GenBank.FeatureItem))
                If Index > -1 Then
                    For i1 = Index To 0 Step -1
                        If Locis(i1).Location.LocationStart >= mdLoci.LocationStart AndAlso Locis(i1).Location.LocationStart <= mdLoci.LocationEnd AndAlso Locis(i1).Location.Operator = Loci.FirstOrDefault.Location.Operator Then
                            out.Add(sorted(i1))
                        ElseIf Locis(i1).Location.LocationStart >= mdLoci.LocationStart AndAlso Locis(i1).Location.LocationStart <= mdLoci.LocationEnd Then

                        Else

                            Exit For
                        End If
                    Next
                    For i1 = Index + 1 To Locis.Count - 1
                        If Locis(i1).Location.LocationStart >= mdLoci.LocationStart AndAlso Locis(i1).Location.LocationStart <= mdLoci.LocationEnd AndAlso Locis(i1).Location.Operator = Loci.FirstOrDefault.Location.Operator Then
                            out.Add(sorted(i1))
                        ElseIf Locis(i1).Location.LocationStart >= mdLoci.LocationStart AndAlso Locis(i1).Location.LocationStart <= mdLoci.LocationEnd Then
                        Else
                            Exit For
                        End If
                    Next
                Else
                    Dim alf As Int16 = 43
                End If

                Return out
            End Function
            Public Shared Function _Start_xth(Loci As List(Of Bio.IO.GenBank.FeatureItem), xth As Integer, width As Integer, sorted As List(Of List(Of Bio.IO.GenBank.FeatureItem)),
                                                        Locis As List(Of Bio.IO.GenBank.FeatureItem)) As List(Of List(Of Bio.IO.GenBank.FeatureItem))
                Dim xth_loci = Szunyi.Features.FeatureManipulation.GetLocations.Get_XTH_Exon_Location(Loci.First, xth)
                Dim out As New List(Of List(Of Bio.IO.GenBank.FeatureItem))
                For Each item In sorted
                    Dim xth_exon = Szunyi.Features.FeatureManipulation.GetLocations.Get_XTH_Exon_Location(item.First, xth)
                    If xth_exon.LocationStart >= xth_loci.LocationStart - width AndAlso xth_exon.LocationStart <= xth_loci.LocationStart + width Then
                        out.Add(item)
                    End If

                Next
                Return out
            End Function
            Public Shared Function _End_xth(Loci As List(Of Bio.IO.GenBank.FeatureItem), xth As Integer, width As Integer, sorted As List(Of List(Of Bio.IO.GenBank.FeatureItem)), Locis As List(Of Bio.IO.GenBank.FeatureItem)) As List(Of List(Of Bio.IO.GenBank.FeatureItem))
                Dim xth_loci = Szunyi.Features.FeatureManipulation.GetLocations.Get_XTH_Exon_Location(Loci.First, xth)
                Dim out As New List(Of List(Of Bio.IO.GenBank.FeatureItem))
                For Each item In sorted
                    Dim xth_exon = Szunyi.Features.FeatureManipulation.GetLocations.Get_XTH_Exon_Location(item.First, xth)
                    If xth_exon.LocationEnd >= xth_loci.LocationEnd - width AndAlso xth_exon.LocationEnd <= xth_loci.LocationEnd + width Then
                        out.Add(item)
                    End If

                Next
                Return out
            End Function


            Public Shared Function _End(Loci As Bio.IO.GenBank.FeatureItem,
                                                             width As Integer,
                                                             Locis As List(Of Bio.IO.GenBank.FeatureItem)) As List(Of Bio.IO.GenBank.FeatureItem)

                Dim mdLoci = Szunyi.Features.FeatureManipulation.GetLocations.LociBuilder.GetLocation(Loci.Location.LocationEnd - width & ".." & Loci.Location.LocationEnd + width)
                Dim mdFeat As New FeatureItem("a", mdLoci)
                Dim Index = Locis.BinarySearch(mdFeat, LociBinary.cFeature_Location_Contain_End)
                Dim out As New List(Of Bio.IO.GenBank.FeatureItem)
                If Index > -1 Then
                    For i1 = Index To 0 Step -1
                        If Locis(i1).Location.LocationEnd >= mdLoci.LocationStart AndAlso Locis(i1).Location.LocationEnd <= mdLoci.LocationEnd AndAlso Locis(i1).Location.Operator = Loci.Location.Operator Then
                            out.Add(Locis(i1))
                        ElseIf Locis(i1).Location.LocationEnd >= mdLoci.LocationStart AndAlso Locis(i1).Location.LocationEnd <= mdLoci.LocationEnd Then

                        Else

                            Exit For
                        End If
                    Next
                    For i1 = Index + 1 To Locis.Count - 1
                        If Locis(i1).Location.LocationEnd >= mdLoci.LocationStart AndAlso Locis(i1).Location.LocationEnd <= mdLoci.LocationEnd AndAlso Locis(i1).Location.Operator = Loci.Location.Operator Then
                            out.Add(Locis(i1))
                        ElseIf Locis(i1).Location.LocationEnd >= mdLoci.LocationStart AndAlso Locis(i1).Location.LocationEnd <= mdLoci.LocationEnd Then
                        Else
                            Exit For
                        End If
                    Next
                Else
                    Dim alf As Int16 = 43
                End If

                Return out
            End Function
            Public Shared Function _End(ToFind As List(Of Bio.IO.GenBank.FeatureItem),
                                                             width As Integer,
                                                             Locis As List(Of Bio.IO.GenBank.FeatureItem)) As List(Of Bio.IO.GenBank.FeatureItem)

                Dim out As New List(Of Bio.IO.GenBank.FeatureItem)
                For Each f In ToFind
                    out.AddRange(_End(f, width, Locis))
                Next

                Return out
            End Function

            Public Shared Function _End(Loci As List(Of Bio.IO.GenBank.FeatureItem), width As Integer, sorted As List(Of List(Of Bio.IO.GenBank.FeatureItem)), Locis As List(Of Bio.IO.GenBank.FeatureItem)) As List(Of List(Of Bio.IO.GenBank.FeatureItem))
                Dim mdLoci = Szunyi.Features.FeatureManipulation.GetLocations.LociBuilder.GetLocation(Loci.First.Location.LocationEnd - width & ".." & Loci.First.Location.LocationEnd + width)
                Dim Index = Locis.BinarySearch(mdLoci, LociBinary.cFeature_Location_ByEnd)
                Dim out As New List(Of List(Of Bio.IO.GenBank.FeatureItem))
                If Index > -1 Then
                    For i1 = Index To 0 Step -1
                        If Locis(i1).Location.LocationEnd >= mdLoci.LocationStart AndAlso Locis(i1).Location.LocationEnd <= mdLoci.LocationEnd AndAlso Locis(i1).Location.Operator = Loci.First.Location.Operator Then
                            out.Add(sorted(i1))
                        Else
                            Exit For
                        End If
                    Next
                    For i1 = Index + 1 To Locis.Count - 1
                        If Locis(i1).Location.LocationEnd >= mdLoci.LocationStart AndAlso Locis(i1).Location.LocationEnd <= mdLoci.LocationEnd AndAlso Locis(i1).Location.Operator = Loci.First.Location.Operator Then
                            out.Add(sorted(i1))
                        Else
                            Exit For
                        End If
                    Next
                Else
                    Dim alf As Int16 = 43
                End If

                Return out
            End Function

#End Region

        End Class


        Public Class GroupBy_Start_End_Boths
#Region "Locis"
            Public Shared Function Group_By_Locis_TSS(locis As List(Of ILocation), minNof As Integer, wOrientation As Boolean) As List(Of List(Of ILocation))
                Dim res As New List(Of List(Of ILocation))
                For Each LociGroup In Iterate.Iterate_By_Locis_TSS(locis, wOrientation)
                    If minNof <= LociGroup.Count Then
                        res.Add(LociGroup)
                        res.Last.AddRange(LociGroup)
                    End If
                Next
                Dim t = From x In res Select x Order By x.First.TSS
                Return t.ToList
            End Function
            Public Shared Function Group_By_Locis_PAS(locis As List(Of ILocation), minNof As Integer, wOrientation As Boolean) As List(Of List(Of ILocation))
                Dim res As New List(Of List(Of ILocation))
                For Each LociGroup In Iterate.Iterate_By_Locis_PAS(locis, wOrientation)
                    If minNof <= LociGroup.Count Then
                        res.Add(LociGroup)
                        res.Last.AddRange(LociGroup)
                    End If
                Next
                Dim t = From x In res Select x Order By x.First.TSS
                Return t.ToList
            End Function
            Public Shared Function Group_By_Locis_Start(Locis As List(Of Bio.IO.GenBank.ILocation), minNofItem As Integer, wOrientation As Boolean) As List(Of List(Of Bio.IO.GenBank.ILocation))
                Dim res As New List(Of List(Of Bio.IO.GenBank.ILocation))
                For Each LociGroup In Iterate.Iterate_By_Locis_Start(Locis, wOrientation)
                    If minNofItem <= LociGroup.Count Then
                        res.Add(New List(Of Bio.IO.GenBank.ILocation))
                        res.Last.AddRange(LociGroup)
                    End If
                Next
                Dim t = From x In res Select x Order By x.First.LocationStart
                Return t.ToList
            End Function
            Public Shared Function Group_By_Locis_Start_Xth(Locis As List(Of Bio.IO.GenBank.ILocation), Xth As Integer, Optional minNofItem As Int16 = 1) As List(Of List(Of Bio.IO.GenBank.ILocation))
                Dim res As New List(Of List(Of Bio.IO.GenBank.ILocation))
                For Each LociGroup In Iterate.Iterate_By_Locis_Start_Xth(Locis, Xth)
                    If minNofItem <= LociGroup.Count Then
                        res.Add(New List(Of Bio.IO.GenBank.ILocation))
                        res.Last.AddRange(LociGroup)
                    End If

                Next
                Dim t = From x In res Select x Order By x.First.LocationStart
                Return t.ToList
            End Function
            Public Shared Function Group_By_Locis_End_xth(Locis As List(Of Bio.IO.GenBank.ILocation), xth As Integer, Optional minNofItem As Int16 = 1) As List(Of List(Of Bio.IO.GenBank.ILocation))
                Dim res As New List(Of List(Of Bio.IO.GenBank.ILocation))
                For Each LociGroup In Iterate.Iterate_By_Locis_End_Xth(Locis, xth)
                    If minNofItem <= LociGroup.Count Then
                        res.Add(New List(Of Bio.IO.GenBank.ILocation))
                        res.Last.AddRange(LociGroup)
                    End If

                Next
                Dim t = From x In res Select x Order By x.First.LocationEnd
                Return t.ToList
            End Function
            Public Shared Function Group_By_Locis_End(Locis As List(Of Bio.IO.GenBank.ILocation), minNofItem As Integer, wOrientation As Boolean) As List(Of List(Of Bio.IO.GenBank.ILocation))
                Dim res As New List(Of List(Of Bio.IO.GenBank.ILocation))
                For Each LociGroup In Iterate.Iterate_By_Locis_End(Locis, wOrientation)
                    If minNofItem <= LociGroup.Count Then
                        res.Add(New List(Of Bio.IO.GenBank.ILocation))
                        res.Last.AddRange(LociGroup)
                    End If

                Next
                Dim t = From x In res Select x Order By x.First.LocationEnd
                Return t.ToList
            End Function
            Public Shared Function Group_By_Locis_Start_End(Locis As List(Of Bio.IO.GenBank.ILocation), minNofItem As Integer, wOrientation As Boolean) As List(Of List(Of Bio.IO.GenBank.ILocation))
                Dim res As New List(Of List(Of Bio.IO.GenBank.ILocation))
                For Each LociGroup In Iterate.Iterate_By_Locis_Start_End(Locis, wOrientation)
                    If minNofItem <= LociGroup.Count Then
                        res.Add(New List(Of Bio.IO.GenBank.ILocation))
                        res.Last.AddRange(LociGroup)
                    End If
                Next
                Dim t = OrderBy.OrderByCount(res)
                Return t
            End Function
            Public Shared Function Group_By_Locis_Start_End_xth(Locis As List(Of Bio.IO.GenBank.ILocation), xth As Integer, Optional minNofItem As Int16 = 1) As List(Of List(Of Bio.IO.GenBank.ILocation))
                Dim res As New List(Of List(Of Bio.IO.GenBank.ILocation))
                For Each LociGroup In Iterate.Iterate_By_Locis_Start_End_Xth(Locis, xth)
                    If minNofItem <= LociGroup.Count Then
                        res.Add(New List(Of Bio.IO.GenBank.ILocation))
                        res.Last.AddRange(LociGroup)
                    End If
                Next
                Dim t = OrderBy.OrderByCount(res)
                Return t
            End Function

#End Region

#Region "BAsic_Location"
            Public Shared Function Group_By_Locis_Start(Locis As List(Of Szunyi.Location.Basic_Location), Optional minNofItem As Int16 = 1) As List(Of List(Of Szunyi.Location.Basic_Location))
                Dim res As New List(Of List(Of Szunyi.Location.Basic_Location))
                For Each LociGroup In Iterate.Iterate_By_Locis_Start(Locis)
                    If minNofItem <= LociGroup.Count Then
                        res.Add(New List(Of Szunyi.Location.Basic_Location))
                        res.Last.AddRange(LociGroup)
                    End If
                Next
                Dim t = From x In res Select x Order By x.First.Location.LocationStart
                Return t.ToList
            End Function
            Public Shared Function Group_By_Locis_Start_Xth(Locis As List(Of Szunyi.Location.Basic_Location), Xth As Integer, Optional minNofItem As Int16 = 1) As List(Of List(Of Szunyi.Location.Basic_Location))
                Dim res As New List(Of List(Of Szunyi.Location.Basic_Location))
                For Each LociGroup In Iterate.Iterate_By_Locis_Start_Xth(Locis, Xth)
                    If minNofItem <= LociGroup.Count Then
                        res.Add(New List(Of Szunyi.Location.Basic_Location))
                        res.Last.AddRange(LociGroup)
                    End If

                Next
                Dim t = From x In res Select x Order By x.First.Location.LocationStart
                Return t.ToList
            End Function
            Public Shared Function Group_By_Locis_End_xth(Locis As List(Of Szunyi.Location.Basic_Location), xth As Integer, Optional minNofItem As Int16 = 1) As List(Of List(Of Szunyi.Location.Basic_Location))
                Dim res As New List(Of List(Of Szunyi.Location.Basic_Location))
                For Each LociGroup In Iterate.Iterate_By_Locis_End_Xth(Locis, xth)
                    If minNofItem <= LociGroup.Count Then
                        res.Add(New List(Of Szunyi.Location.Basic_Location))
                        res.Last.AddRange(LociGroup)
                    End If

                Next
                Dim t = From x In res Select x Order By x.First.Location.LocationEnd
                Return t.ToList
            End Function
            Public Shared Function Group_By_Locis_End(Locis As List(Of Szunyi.Location.Basic_Location), Optional minNofItem As Int16 = 1) As List(Of List(Of Szunyi.Location.Basic_Location))
                Dim res As New List(Of List(Of Szunyi.Location.Basic_Location))
                For Each LociGroup In Iterate.Iterate_By_Locis_End(Locis)
                    If minNofItem <= LociGroup.Count Then
                        res.Add(New List(Of Szunyi.Location.Basic_Location))
                        res.Last.AddRange(LociGroup)
                    End If

                Next
                Dim t = From x In res Select x Order By x.First.Location.LocationEnd
                Return t.ToList
            End Function
            Public Shared Function Group_By_Locis_Start_End(Locis As List(Of Szunyi.Location.Basic_Location), Optional minNofItem As Int16 = 1) As List(Of List(Of Szunyi.Location.Basic_Location))
                Dim res As New List(Of List(Of Szunyi.Location.Basic_Location))
                For Each LociGroup In Iterate.Iterate_By_Locis_Start_End(Locis)
                    If minNofItem <= LociGroup.Count Then
                        res.Add(New List(Of Szunyi.Location.Basic_Location))
                        res.Last.AddRange(LociGroup)
                    End If
                Next
                Dim t = OrderBy.OrderByCount(res)
                Return t
            End Function
            Public Shared Function Group_By_Locis_Start_End_xth(Locis As List(Of Szunyi.Location.Basic_Location), xth As Integer, Optional minNofItem As Int16 = 1) As List(Of List(Of Szunyi.Location.Basic_Location))
                Dim res As New List(Of List(Of Szunyi.Location.Basic_Location))
                For Each LociGroup In Iterate.Iterate_By_Locis_Start_End_Xth(Locis, xth)
                    If minNofItem <= LociGroup.Count Then
                        res.Add(New List(Of Szunyi.Location.Basic_Location))
                        res.Last.AddRange(LociGroup)
                    End If
                Next
                Dim t = OrderBy.OrderByCount(res)
                Return t
            End Function

            Public Shared Function Group_By_Locis_TSS(locis As List(Of Basic_Location), minNof As Integer) As List(Of List(Of Szunyi.Location.Basic_Location))
                Dim res As New List(Of List(Of Szunyi.Location.Basic_Location))
                For Each LociGroup In Iterate.Iterate_By_Locis_TSS(locis)
                    If minNof <= LociGroup.Count Then
                        res.Add(New List(Of Szunyi.Location.Basic_Location))
                        res.Last.AddRange(LociGroup)
                    End If
                Next
                Dim t = From x In res Select x Order By x.First.Location.TSS
                Return t.ToList
            End Function

            Friend Shared Function Group_By_Locis_PAS(locis As List(Of Basic_Location), minNof As Integer) As List(Of List(Of Szunyi.Location.Basic_Location))
                Dim res As New List(Of List(Of Szunyi.Location.Basic_Location))
                For Each LociGroup In Iterate.Iterate_By_Locis_PAS(locis)
                    If minNof <= LociGroup.Count Then
                        res.Add(New List(Of Szunyi.Location.Basic_Location))
                        res.Last.AddRange(LociGroup)
                    End If
                Next
                Dim t = From x In res Select x Order By x.First.Location.PAS
                Return t.ToList
            End Function
#End Region

#Region "Features"
            Public Shared Function Group_By_Locis_TSS(locis As List(Of FeatureItem), minNof As Integer) As List(Of List(Of FeatureItem))
                Dim res As New List(Of List(Of FeatureItem))
                For Each LociGroup In Iterate.Iterate_By_Locis_TSS(locis)
                    If minNof <= LociGroup.Count Then
                        res.Add(LociGroup)
                        res.Last.AddRange(LociGroup)
                    End If
                Next
                Dim t = From x In res Select x Order By x.First.Location.TSS
                Return t.ToList
            End Function
            Public Shared Function Group_By_Locis_PAS(locis As List(Of FeatureItem), minNof As Integer) As List(Of List(Of FeatureItem))
                Dim res As New List(Of List(Of FeatureItem))
                For Each LociGroup In Iterate.Iterate_By_Locis_PAS(locis)
                    If minNof <= LociGroup.Count Then
                        res.Add(LociGroup)
                        res.Last.AddRange(LociGroup)
                    End If
                Next
                Dim t = From x In res Select x Order By x.First.Location.PAS
                Return t.ToList
            End Function
            Public Shared Function Group_By_Locis_End(Feats As List(Of Bio.IO.GenBank.FeatureItem), Optional minNofItem As Int16 = 1) As List(Of List(Of Bio.IO.GenBank.FeatureItem))
                Dim res As New List(Of List(Of Bio.IO.GenBank.FeatureItem))
                For Each LociGroup In Iterate.Iterate_By_Locis_End(Feats)
                    If minNofItem <= LociGroup.Count Then
                        res.Add(New List(Of Bio.IO.GenBank.FeatureItem))
                        res.Last.AddRange(LociGroup)
                    End If

                Next
                Return res
            End Function
            Public Shared Function Group_By_Locis_Start_End(Feats As List(Of Bio.IO.GenBank.FeatureItem), Optional minNofItem As Int16 = 1) As List(Of List(Of Bio.IO.GenBank.FeatureItem))
                Dim res As New List(Of List(Of Bio.IO.GenBank.FeatureItem))
                For Each LociGroup In Iterate.Iterate_By_Locis_Start_End(Feats)
                    If minNofItem <= LociGroup.Count Then
                        res.Add(New List(Of Bio.IO.GenBank.FeatureItem))
                        res.Last.AddRange(LociGroup)
                    End If
                Next

                Return res
            End Function
            Public Shared Function Group_By_Locis_Start(Locis As List(Of Bio.IO.GenBank.FeatureItem), Optional minNofItem As Int16 = 1) As List(Of List(Of Bio.IO.GenBank.FeatureItem))
                Dim res As New List(Of List(Of Bio.IO.GenBank.FeatureItem))
                For Each LociGroup In Iterate.Iterate_By_Locis_Start(Locis)
                    If minNofItem <= LociGroup.Count Then
                        res.Add(New List(Of Bio.IO.GenBank.FeatureItem))
                        res.Last.AddRange(LociGroup)
                    End If

                Next
                Return res
            End Function



#End Region
        End Class

        Public Class OrderBy
            Public Shared Function OrderByStart(x As List(Of List(Of Bio.IO.GenBank.ILocation))) As List(Of List(Of Bio.IO.GenBank.ILocation))
                Dim l = From f In x Order By f.First.LocationStart Ascending

                Return l.ToList


            End Function
            Public Shared Function OrderByCount(x As List(Of List(Of Bio.IO.GenBank.ILocation))) As List(Of List(Of Bio.IO.GenBank.ILocation))
                Dim l = From f In x Order By f.Count Descending

                Return l.ToList

            End Function

#Region "Basic_Location"
            Public Shared Function OrderByStart(x As List(Of List(Of Szunyi.Location.Basic_Location))) As List(Of List(Of Szunyi.Location.Basic_Location))
                Dim l = From f In x Order By f.First.Location.LocationStart Ascending

                Return l.ToList


            End Function
            Public Shared Function OrderByCount(x As List(Of List(Of Szunyi.Location.Basic_Location))) As List(Of List(Of Szunyi.Location.Basic_Location))
                Dim l = From f In x Order By f.Count Descending

                Return l.ToList

            End Function
#End Region
        End Class

        Public Class Iterate
            Public Shared Iterator Function Iterate_By_Same_Locations(Locis As List(Of ILocation), width As Integer) As IEnumerable(Of List(Of ILocation))
                Dim nof = Locis.First.SubLocations.Count
                Dim index = 0
                Dim out As New List(Of List(Of ILocation))
                Dim gth As New List(Of List(Of List(Of ILocation)))
                Do
                    Dim Xth = Szunyi.Features.FeatureManipulation.GetLocations.Get_xth_Exons_Location(Locis, index)
                    Dim Xth_Merged = Szunyi.Location.Merging.MergeLocations(index, Locis, width, Sort_Locations_By.TSS_PAS, 2)
                    index += 1
                    gth.Add(Xth_Merged)

                Loop Until index = nof
                Dim StartList = gth.First
                For i1 = 1 To gth.Count - 1
                    StartList = Get_Common_Locations_ByRef(StartList, gth(i1))

                Next

                For Each item In StartList
                    Dim tmp As New List(Of String)
                    For Each j In item
                        tmp.Add(Szunyi.Location.Common.GetLocationString(j))
                    Next
                    Dim k = Szunyi.Text.General.GetText(tmp)
                    Yield item
                Next

            End Function

            Private Shared Function Get_Common_Locations_ByRef(jjj As List(Of List(Of ILocation)), list As List(Of List(Of ILocation))) As List(Of List(Of ILocation))
                Dim out As New List(Of List(Of ILocation))
                For i1 = 0 To jjj.Count - 1
                    For i2 = 0 To list.Count - 1
                        Dim sg As New List(Of ILocation)
                        Dim common = jjj(i1).Intersect(list(i2))
                        If common.Count > 0 Then
                            out.Add(common.ToList)
                        End If

                    Next
                Next
                Return out
            End Function

            Public Shared Iterator Function Iterate_By_Nof_Exons(Feats As List(Of FeatureItem)) As IEnumerable(Of List(Of Bio.IO.GenBank.FeatureItem))

                Dim gr = From x In Feats Group By Szunyi.Features.FeatureManipulation.GetLocations.Get_All_Exon_Location(x).Count Into Group

                For Each Item In gr
                    Yield Item.Group.ToList
                Next
            End Function
            Public Shared Iterator Function Iterate_By_Nof_Exons(Feats As List(Of ILocation)) As IEnumerable(Of List(Of Bio.IO.GenBank.ILocation))

                Dim gr = From x In Feats Group By Szunyi.Features.FeatureManipulation.GetLocations.Get_All_Exon_Location(x).Count Into Group

                For Each Item In gr
                    Yield Item.Group.ToList
                Next
            End Function
#Region "Locis"
            Public Shared Iterator Function Iterate_By_Locis_Start_Xth(locis As List(Of ILocation), xth As Integer) As IEnumerable(Of List(Of Bio.IO.GenBank.ILocation))

                Dim x = From t In locis Group By Szunyi.Features.FeatureManipulation.GetLocations.Get_XTH_Exon_Location(t, xth).LocationStart, t.IsComplementer Into Group

            End Function

            Public Shared Iterator Function Iterate_By_Locis_Start(Locis As List(Of Bio.IO.GenBank.ILocation), wOrientation As Boolean) As IEnumerable(Of List(Of Bio.IO.GenBank.ILocation))
                If wOrientation = True Then
                    Dim x = From t In Locis Group By t.LocationStart, t.IsComplementer Into Group

                    For Each j In x
                        Yield j.Group.ToList

                    Next
                Else
                    Dim x = From t In Locis Group By t.LocationStart Into Group

                    For Each j In x
                        Yield j.Group.ToList

                    Next
                End If


            End Function

            Public Shared Iterator Function Iterate_By_Locis_Start_End(Locis As List(Of Bio.IO.GenBank.ILocation), wOrientation As Boolean) As IEnumerable(Of List(Of Bio.IO.GenBank.ILocation))
                If wOrientation = True Then
                    Dim x = From t In Locis Group By t.LocationStart, t.LocationEnd, t.IsComplementer Into Group

                    For Each j In x
                        Yield j.Group.ToList
                    Next
                Else
                    Dim x = From t In Locis Group By t.LocationStart, t.LocationEnd Into Group

                    For Each j In x
                        Yield j.Group.ToList
                    Next
                End If
            End Function

            Public Shared Iterator Function Iterate_By_Locis_Start_End_Xth(Locis As List(Of Bio.IO.GenBank.ILocation), xth As Integer) As IEnumerable(Of List(Of Bio.IO.GenBank.ILocation))
                Dim x = From t In Locis Group By Szunyi.Features.FeatureManipulation.GetLocations.Get_XTH_Exon_Location(t, xth).LocationStart, Szunyi.Features.FeatureManipulation.GetLocations.Get_XTH_Exon_Location(t, xth).LocationEnd, t.IsComplementer Into Group

                For Each j In x
                    Yield j.Group.ToList
                Next
            End Function
            Public Shared Iterator Function Iterate_By_Locis_End_Xth(locis As List(Of ILocation), xth As Integer) As IEnumerable(Of List(Of Bio.IO.GenBank.ILocation))

                Dim x = From t In locis Group By Szunyi.Features.FeatureManipulation.GetLocations.Get_XTH_Exon_Location(t, xth).LocationEnd, t.IsComplementer Into Group

            End Function
            Public Shared Iterator Function Iterate_By_Locis_End(Locis As List(Of Bio.IO.GenBank.ILocation), wOrientation As Boolean) As IEnumerable(Of List(Of Bio.IO.GenBank.ILocation))
                If wOrientation = True Then
                    Dim x = From t In Locis Group By t.LocationEnd, t.IsComplementer Into Group

                    For Each j In x
                        Yield j.Group.ToList

                    Next
                Else
                    Dim x = From t In Locis Group By t.LocationEnd Into Group

                    For Each j In x
                        Yield j.Group.ToList

                    Next
                End If

            End Function
            Public Shared Iterator Function Iterate_By_Locis_TSS(locis As List(Of ILocation), wOrientation As Boolean) As IEnumerable(Of List(Of ILocation))
                If wOrientation = True Then
                    Dim x = From t In locis Group By t.TSS, t.IsComplementer Into Group

                    For Each j In x
                        Yield j.Group.ToList

                    Next
                Else
                    Dim x = From t In locis Group By t.TSS Into Group

                    For Each j In x
                        Yield j.Group.ToList

                    Next
                End If

            End Function

            Public Shared Iterator Function Iterate_By_Locis_PAS(locis As List(Of ILocation), wOrientation As Boolean) As IEnumerable(Of List(Of ILocation))
                If wOrientation = True Then
                    Dim x = From t In locis Group By t.PAS, t.IsComplementer Into Group

                    For Each j In x
                        Yield j.Group.ToList

                    Next
                Else
                    Dim x = From t In locis Group By t.PAS Into Group

                    For Each j In x
                        Yield j.Group.ToList

                    Next
                End If

            End Function

            Public Shared Iterator Function Iterate_By_LocationString(locis As List(Of ILocation)) As IEnumerable(Of List(Of ILocation))
                Dim x = From t In locis Group By l = Szunyi.Location.Common.GetLocationString(t) Into Group

                For Each j In x
                    Yield j.Group.ToList

                Next
            End Function
#End Region

#Region "Basic_Location"
            Public Shared Iterator Function Iterate_By_Locis_Start_Xth(locis As List(Of Szunyi.Location.Basic_Location), xth As Integer) As IEnumerable(Of List(Of Szunyi.Location.Basic_Location))

                Dim x = From t In locis Group By Szunyi.Features.FeatureManipulation.GetLocations.Get_XTH_Exon_Location(t, xth).LocationStart, t.Location.Operator Into Group

            End Function

            Public Shared Iterator Function Iterate_By_Locis_Start(Locis As List(Of Szunyi.Location.Basic_Location)) As IEnumerable(Of List(Of Szunyi.Location.Basic_Location))
                Dim x = From t In Locis Group By t.Location.LocationStart, t.Location.IsComplementer Into Group

                For Each j In x
                    Yield j.Group.ToList

                Next

            End Function

            Public Shared Iterator Function Iterate_By_Locis_Start_End(Locis As List(Of Szunyi.Location.Basic_Location)) As IEnumerable(Of List(Of Szunyi.Location.Basic_Location))
                Dim x = From t In Locis Group By t.Location.LocationStart, t.Location.LocationEnd, t.Location.IsComplementer Into Group

                For Each j In x
                    Yield j.Group.ToList
                Next
            End Function


            Public Shared Iterator Function Iterate_By_Locis_Start_End_Xth(Locis As List(Of Szunyi.Location.Basic_Location), xth As Integer) As IEnumerable(Of List(Of Szunyi.Location.Basic_Location))
                Dim x = From t In Locis Group By Szunyi.Features.FeatureManipulation.GetLocations.Get_XTH_Exon_Location(t.Location, xth).LocationStart,
                                            Szunyi.Features.FeatureManipulation.GetLocations.Get_XTH_Exon_Location(t, xth).LocationEnd, t.Location.Operator Into Group

                For Each j In x
                    Yield j.Group.ToList
                Next
            End Function
            Public Shared Iterator Function Iterate_By_Locis_End_Xth(locis As List(Of Szunyi.Location.Basic_Location), xth As Integer) As IEnumerable(Of List(Of Szunyi.Location.Basic_Location))

                Dim x = From t In locis Group By Szunyi.Features.FeatureManipulation.GetLocations.Get_XTH_Exon_Location(t, xth).LocationEnd, t.Location.Operator Into Group

            End Function
            Public Shared Iterator Function Iterate_By_Locis_End(Locis As List(Of Szunyi.Location.Basic_Location)) As IEnumerable(Of List(Of Szunyi.Location.Basic_Location))
                Dim x = From t In Locis Group By t.Location.LocationEnd, t.Location.IsComplementer Into Group

                For Each j In x
                    Yield j.Group.ToList

                Next

            End Function
            Public Shared Iterator Function Iterate_By_Locis_TSS(locis As List(Of Basic_Location)) As IEnumerable(Of List(Of Basic_Location))
                Dim x = From t In locis Group By t.Location.TSS, t.Location.IsComplementer Into Group

                For Each j In x
                    Yield j.Group.ToList

                Next
            End Function

            Public Shared Iterator Function Iterate_By_Locis_PAS(locis As List(Of Basic_Location)) As IEnumerable(Of List(Of Basic_Location))
                Dim x = From t In locis Group By t.Location.PAS, t.Location.IsComplementer Into Group

                For Each j In x
                    Yield j.Group.ToList

                Next
            End Function
#End Region

#Region "Feature_Location"
            Public Shared Iterator Function Iterate_By_Locis_Start_Xth(locis As List(Of FeatureItem), xth As Integer) As IEnumerable(Of List(Of FeatureItem))

                Dim x = From t In locis Group By Szunyi.Features.FeatureManipulation.GetLocations.Get_XTH_Exon_Location(t, xth).LocationStart, t.Location.Operator Into Group

            End Function

            Public Shared Iterator Function Iterate_By_Locis_Start(Locis As List(Of FeatureItem)) As IEnumerable(Of List(Of FeatureItem))
                Dim x = From t In Locis Group By t.Location.LocationStart, t.Location.Operator Into Group

                For Each j In x
                    Yield j.Group.ToList

                Next

            End Function

            Public Shared Iterator Function Iterate_By_Locis_Start_End(Locis As List(Of FeatureItem)) As IEnumerable(Of List(Of FeatureItem))
                Dim x = From t In Locis Group By t.Location.LocationStart, t.Location.LocationEnd, t.Location.Operator Into Group

                For Each j In x
                    Yield j.Group.ToList
                Next
            End Function


            Public Shared Iterator Function Iterate_By_Locis_Start_End_Xth(Locis As List(Of FeatureItem), xth As Integer) As IEnumerable(Of List(Of FeatureItem))
                Dim x = From t In Locis Group By Szunyi.Features.FeatureManipulation.GetLocations.Get_XTH_Exon_Location(t.Location, xth).LocationStart,
                                            Szunyi.Features.FeatureManipulation.GetLocations.Get_XTH_Exon_Location(t, xth).LocationEnd, t.Location.Operator Into Group

                For Each j In x
                    Yield j.Group.ToList
                Next
            End Function
            Public Shared Iterator Function Iterate_By_Locis_End_Xth(locis As List(Of FeatureItem), xth As Integer) As IEnumerable(Of List(Of FeatureItem))

                Dim x = From t In locis Group By Szunyi.Features.FeatureManipulation.GetLocations.Get_XTH_Exon_Location(t, xth).LocationEnd, t.Location.Operator Into Group

            End Function
            Public Shared Iterator Function Iterate_By_Locis_End(Locis As List(Of FeatureItem)) As IEnumerable(Of List(Of FeatureItem))
                Dim x = From t In Locis Group By t.Location.LocationEnd, t.Location.Operator Into Group

                For Each j In x
                    Yield j.Group.ToList

                Next

            End Function
            Public Shared Iterator Function Iterate_By_Locis_TSS(locis As List(Of FeatureItem)) As IEnumerable(Of List(Of FeatureItem))
                Dim x = From t In locis Group By t.Location.TSS, t.Location.Operator Into Group

                For Each j In x
                    Yield j.Group.ToList

                Next
            End Function

            Public Shared Iterator Function Iterate_By_Locis_PAS(locis As List(Of FeatureItem)) As IEnumerable(Of List(Of FeatureItem))
                Dim x = From t In locis Group By t.Location.PAS, t.Location.Operator Into Group

                For Each j In x
                    Yield j.Group.ToList

                Next
            End Function
#End Region


        End Class

        Public Class Report
            Public Shared Function GetReport(mergedLocations As List(Of List(Of Bio.IO.GenBank.ILocation))) As String
                Dim str As New System.Text.StringBuilder
                For Each Item In mergedLocations
                    str.Append(Szunyi.Features.FeatureManipulation.GetLocations.LociBuilder.GetLocationString(Item.First)).Append(vbTab)
                    str.Append(Item.Count).AppendLine()

                Next
                Return str.ToString
            End Function
            Public Shared Function GetReport(mergedLocations As System.Linq.IOrderedEnumerable(Of System.Collections.Generic.KeyValuePair(Of String, System.Collections.Generic.List(Of System.Collections.Generic.List(Of Bio.IO.GenBank.ILocation))))) As String
                Dim str As New System.Text.StringBuilder
                For Each Item In mergedLocations
                    str.Append(Item.Key).Append(vbTab)
                    str.Append(Get_All_Counts(Item.Value)).AppendLine()
                    For Each SubItem In Item.Value
                        str.Append(vbTab).Append(vbTab).Append(Szunyi.Features.FeatureManipulation.GetLocations.LociBuilder.GetLocationString(SubItem.First))
                        str.Append(vbTab).Append(SubItem.Count).AppendLine()
                    Next
                Next
                Return str.ToString
            End Function
            Private Shared Function Get_All_Counts(Locis As List(Of List(Of ILocation))) As Integer
                Dim out As Integer = 0
                For Each loci In Locis
                    out += loci.Count
                Next
                Return out
            End Function

        End Class
        Public Class Modify
            Public Shared Function Change_Location_Start(loci As Bio.IO.GenBank.ILocation, m As Integer) As Bio.IO.GenBank.ILocation
                Dim Others As New List(Of String)
                Dim l = Common.GetLocationString(loci)
                Dim IsDigit As Boolean = Char.IsNumber(l.First)
                Dim curr As String = ""
                For Each c As Char In l
                    If [Char].IsNumber(c) Then
                        If IsDigit = True Then
                            curr = curr & c
                        Else
                            IsDigit = True
                            Others.Add(curr)
                            curr = c
                        End If
                    Else
                        If IsDigit = False Then
                            curr = curr & c
                        Else
                            Others.Add(curr)

                            curr = c
                            IsDigit = False
                        End If
                    End If
                Next
                Others.Add(curr)
                Dim sb As New System.Text.StringBuilder
                Dim IsFirst As Boolean = True
                For Each Item In Others
                    Dim res As Integer
                    If Integer.TryParse(Item, res) = True AndAlso IsFIrst = True Then
                        sb.Append(res + m)
                        IsFirst = False
                    Else
                        sb.Append(Item)
                    End If
                Next
                Return Szunyi.Location.Common.Get_Location(sb.ToString)
            End Function
        End Class
        Public Class Common
            Public Shared LociBuilder As New Bio.IO.GenBank.LocationBuilder
#Region "String"
            Public Shared Function Get_Location_Strings(locis As List(Of List(Of Szunyi.Location.Basic_Location))) As List(Of String)
                Dim out As New List(Of String)
                For Each Loci In locis
                    out.Add("e:" & Loci.Count & " " & Szunyi.Features.FeatureManipulation.GetLocations.LociBuilder.GetLocationString(Loci.First))
                Next
                Return out
            End Function
            Public Shared Function Get_Location_Strings(locis As List(Of Szunyi.Location.Basic_Location)) As List(Of String)
                Dim out As New List(Of String)

                For Each Loci In locis
                    If IsNothing(Loci.Location.Accession) = True Then
                        out.Add(vbTab & Szunyi.Features.FeatureManipulation.GetLocations.LociBuilder.GetLocationString(Loci.Location))
                    Else
                        out.Add(Loci.Location.Accession & vbTab & Szunyi.Features.FeatureManipulation.GetLocations.LociBuilder.GetLocationString(Loci.Location))
                    End If

                Next
                Return out
            End Function
            Public Shared Function Get_Location_Strings(locis As List(Of List(Of ILocation))) As List(Of String)
                Dim out As New List(Of String)
                For Each Loci In locis
                    out.Add("e:" & Loci.Count & " " & Szunyi.Features.FeatureManipulation.GetLocations.LociBuilder.GetLocationString(Loci.First))
                Next
                Return out
            End Function
            Public Shared Function Get_Location_Strings(locis As List(Of ILocation)) As List(Of String)
                Dim out As New List(Of String)

                For Each Loci In locis
                    out.Add(Loci.Accession & vbTab & Szunyi.Features.FeatureManipulation.GetLocations.LociBuilder.GetLocationString(Loci))
                Next
                Return out
            End Function
            Public Shared Function Get_Location_Strings(locis As List(Of FeatureItem)) As List(Of String)
                Dim out As New List(Of String)

                For Each Freat In locis
                    Dim loci = Freat.Location
                    out.Add(loci.Accession & vbTab & Szunyi.Features.FeatureManipulation.GetLocations.LociBuilder.GetLocationString(loci))
                Next
                Return out
            End Function
            Public Shared Function GetLocationString(loci As Bio.IO.GenBank.ILocation) As String
                Return LociBuilder.GetLocationString(loci)
            End Function
            Public Shared Function GetLocationStringTab(Feat As Bio.IO.GenBank.FeatureItem) As String
                If Feat.Location.IsComplementer = True Then

                    Return "-" & vbTab & Feat.Location.LocationStart & vbTab & Feat.Location.LocationEnd
                Else
                    Return "+" & vbTab & Feat.Location.LocationStart & vbTab & Feat.Location.LocationEnd
                End If
            End Function
            Public Shared Function GetLocationStringTab(Feat As Szunyi.Location.Basic_Location, sort As Sort_Locations_By) As String
                Return GetLocationStringTab(Feat.Location, sort)
            End Function
            Public Shared Function GetLocationStringTab(Feat As Bio.IO.GenBank.ILocation, sort As Sort_Locations_By) As String
                Dim str As New System.Text.StringBuilder
                If Feat.IsComplementer = True Then
                    str.Append("-").Append(vbTab)
                Else
                    str.Append("+").Append(vbTab)
                End If

                Select Case sort
                    Case Sort_Locations_By.TSS_PAS
                        str.Append(Feat.LocationStart).Append(vbTab).Append(Feat.LocationEnd)
                    Case Sort_Locations_By.LE
                        str.Append(Feat.LocationEnd)
                    Case Sort_Locations_By.PAS
                        str.Append(Feat.PAS)
                    Case Sort_Locations_By.TSS
                        str.Append(Feat.LocationStart)
                    Case Sort_Locations_By.TSS
                        str.Append(Feat.TSS)

                End Select





                Return str.ToString
            End Function
            Public Shared Function GetLocationStringTab(Feat As Bio.IO.GenBank.Location, sort As Sort_Locations_By) As String
                Dim str As New System.Text.StringBuilder
                If Feat.IsComplementer = True Then
                    str.Append("-").Append(vbTab)
                Else
                    str.Append("+").Append(vbTab)
                End If

                Select Case sort
                    Case Sort_Locations_By.TSS_PAS
                        str.Append(Feat.LocationStart).Append(vbTab).Append(Feat.LocationEnd).Append(vbTab)
                    Case Sort_Locations_By.LE
                        str.Append(Feat.LocationEnd).Append(vbTab)
                    Case Sort_Locations_By.PAS
                        str.Append(Feat.PAS).Append(vbTab)
                    Case Sort_Locations_By.TSS
                        str.Append(Feat.LocationStart).Append(vbTab)
                    Case Sort_Locations_By.TSS
                        str.Append(Feat.TSS).Append(vbTab)

                End Select





                Return str.ToString
            End Function

            Public Shared Function GetLocationStringTab(Feat As Bio.IO.GenBank.FeatureItem, sort As Sort_Locations_By) As String
                Dim str As New System.Text.StringBuilder
                If Feat.Location.IsComplementer = True Then
                    str.Append("-").Append(vbTab)
                Else
                    str.Append("+").Append(vbTab)
                End If

                Select Case sort
                    Case Sort_Locations_By.TSS_PAS
                        str.Append(Feat.Location.LocationStart).Append(vbTab).Append(Feat.Location.LocationEnd).Append(vbTab)
                    Case Sort_Locations_By.LE
                        str.Append(Feat.Location.LocationEnd).Append(vbTab)
                    Case Sort_Locations_By.PAS
                        str.Append(Feat.Location.PAS).Append(vbTab)
                    Case Sort_Locations_By.TSS
                        str.Append(Feat.Location.LocationStart).Append(vbTab)
                    Case Sort_Locations_By.TSS
                        str.Append(Feat.Location.TSS).Append(vbTab)

                End Select





                Return str.ToString
            End Function
            Public Shared Function Get_LocationString_LSLE(Feat As Bio.IO.GenBank.ILocation) As String
                Return Feat.LocationStart & vbTab & Feat.LocationEnd

            End Function
            Public Shared Function GetLocationStringTab(Feat As Bio.IO.GenBank.ILocation) As String
                If Feat.IsComplementer = True Then

                    Return "-" & vbTab & Feat.LocationStart & vbTab & Feat.LocationEnd
                Else
                    Return "+" & vbTab & Feat.LocationStart & vbTab & Feat.LocationEnd
                End If
            End Function
            Public Shared Function GetLocationStringPASTab(Feat As Bio.IO.GenBank.ILocation) As String
                If Feat.IsComplementer = True Then

                    Return "-" & vbTab & Feat.PAS
                Else
                    Return "+" & vbTab & Feat.PAS
                End If
            End Function
            Public Shared Function GetLocationStringTSSTab(Feat As Bio.IO.GenBank.ILocation) As String
                If Feat.IsComplementer = True Then

                    Return "-" & vbTab & Feat.TSS & vbTab & Feat.LocationEnd
                Else
                    Return "+" & vbTab & Feat.TSS
                End If
            End Function
            Public Shared Function GetLocationStringTSS_PAS_Strand_Tab(Feat As Bio.IO.GenBank.ILocation) As String
                If Feat.IsComplementer = True Then

                    Return "-" & vbTab & Feat.TSS & vbTab & Feat.PAS
                Else
                    Return "+" & vbTab & Feat.TSS & vbTab & Feat.PAS
                End If
            End Function
            Public Shared Function GetLocationString(Feat As Bio.IO.GenBank.FeatureItem) As String
                Return LociBuilder.GetLocationString(Feat.Location)
            End Function
#End Region
#Region "Lengths"
            Public Shared Function Get_Length(Feats As List(Of ILocation)) As List(Of Integer)
                Dim out As New List(Of Integer)
                For Each f In Feats
                    out.Add(Get_Length(f))
                Next
                Return out
            End Function
            Public Shared Function Get_Length(Feats As List(Of FeatureItem)) As List(Of Integer)
                Dim out As New List(Of Integer)
                For Each f In Feats
                    out.Add(Get_Length(f))
                Next
                Return out
            End Function
            Public Shared Function Get_Length(Feat As FeatureItem) As Integer
                Dim Exons = Szunyi.Features.FeatureManipulation.GetLocations.Get_All_Exon_Location(Feat.Location)
                Dim res = (From x In Exons Select x.LocationEnd - x.LocationStart + 1).Sum
                Return res
            End Function
            Public Shared Function Get_Length(Feat As ILocation) As Integer
                Dim Exons = Szunyi.Features.FeatureManipulation.GetLocations.Get_All_Exon_Location(Feat)
                Dim res = (From x In Exons Select x.LocationEnd - x.LocationStart + 1).Sum
                Return res
            End Function
            Public Shared Function Get_Smaller(i1 As Integer, i2 As Integer) As Integer
                If i1 < i2 Then Return i1
                Return i2
            End Function
            Public Shared Function Get_Bigger(i1 As Integer, i2 As Integer) As Integer
                If i1 > i2 Then Return i1
                Return i2
            End Function
#End Region
#Region "Get_Locations "
            Public Shared Function Get_All_Intron_Location(Loci As ILocation) As List(Of ILocation)
                Dim exons = Get_All_Exon_Location(Loci)
                If exons.Count = 0 Then Return New List(Of ILocation)
                Dim out As New List(Of ILocation)
                For i1 = 0 To exons.Count - 2
                    If exons(i1).LocationEnd > exons(i1 + 1).LocationStart Then
                        Dim ald As Int16 = 54
                    End If
                    If exons(i1).Operator = LocationOperator.Complement Then
                        out.Add(LociBuilder.GetLocation("complement(" & exons(i1).LocationEnd + 1 & ".." & exons(i1 + 1).LocationStart - 1 & ")"))
                        out.Last.Accession = exons.First.Accession
                    Else
                        out.Add(LociBuilder.GetLocation(exons(i1).LocationEnd + 1 & ".." & exons(i1 + 1).LocationStart - 1))
                        out.Last.Accession = exons.First.Accession
                    End If

                Next

                Return out
            End Function
            Public Shared Function Get_All_Exons_Location(Locis As List(Of Bio.IO.GenBank.ILocation)) As List(Of Bio.IO.GenBank.ILocation)
                Dim out As New List(Of Bio.IO.GenBank.ILocation)
                For Each Loci In Locis
                    out.AddRange(Get_All_Exon_Location(Loci))
                Next
                Return out
            End Function
            Public Shared Function Get_All_Exons_Location(Locis As List(Of Bio.IO.GenBank.Location)) As List(Of Bio.IO.GenBank.ILocation)
                Dim out As New List(Of Bio.IO.GenBank.ILocation)
                For Each Loci In Locis
                    out.AddRange(Get_All_Exon_Location(Loci))
                Next
                Return out
            End Function
            Public Shared Function Get_Smallest_Exon_Location(Loci As Bio.IO.GenBank.ILocation) As Bio.IO.GenBank.ILocation
                Dim Exons = Get_All_Exon_Location(Loci)
                Dim es = From x In Exons Order By x.LocationEnd - x.LocationStart Ascending

                Return es.First
            End Function
            Public Shared Function Get_Biggest_Exon_Location(Loci As Bio.IO.GenBank.ILocation) As Bio.IO.GenBank.ILocation
                Dim Exons = Get_All_Exon_Location(Loci)
                Dim es = From x In Exons Order By x.LocationEnd - x.LocationStart Ascending

                Return es.Last
            End Function
            Public Shared Function Get_Biggest_Exon_Length(Loci As Bio.IO.GenBank.ILocation) As Integer
                Dim Exons = Get_All_Exon_Location(Loci)
                Dim es = From x In Exons Order By x.LocationEnd - x.LocationStart Ascending

                Return Get_Length(es.Last)
            End Function
            Public Shared Function Get_Biggest_Intron_Location(Loci As Bio.IO.GenBank.ILocation) As Bio.IO.GenBank.ILocation
                Dim Exons = Get_All_Intron_Location(Loci)
                If Exons.Count = 0 Then Return Nothing
                Dim es = From x In Exons Order By x.LocationEnd - x.LocationStart Ascending

                Return es.Last
            End Function
            Public Shared Function Get_Biggest_Intron_Length(Loci As Bio.IO.GenBank.ILocation) As Integer
                Dim Exons = Get_All_Intron_Location(Loci)
                If Exons.Count = 0 Then Return 0
                Dim es = From x In Exons Order By x.LocationEnd - x.LocationStart Ascending

                Return Get_Length(es.Last)
            End Function
            Public Shared Function Get_Smallest_Intron_Location(Loci As Bio.IO.GenBank.ILocation) As Bio.IO.GenBank.ILocation
                Dim Exons = Get_All_Intron_Location(Loci)
                If Exons.Count = 0 Then Return Nothing
                Dim es = From x In Exons Order By x.LocationEnd - x.LocationStart Ascending

                Return es.First
            End Function
            Public Shared Function Get_All_Exon_Location(Loci As Bio.IO.GenBank.ILocation) As List(Of Bio.IO.GenBank.ILocation)
                If IsNothing(Loci) = True Then Return Nothing
                Dim out As New List(Of ILocation)
                If Loci.SubLocations.Count = 0 Then ' No complement no join
                    out.Add(Loci)
                    out.Last.Accession = Loci.Accession
                    Return out
                End If
                If Loci.Operator <> LocationOperator.Complement Then ' no complement join
                    For Each subL In Loci.SubLocations
                        subL.Accession = Loci.Accession
                    Next
                    Return Loci.SubLocations
                End If
                If Loci.SubLocations.First.Operator = LocationOperator.Join Then ' complement join
                    For Each subloci As Bio.IO.GenBank.Location In Loci.SubLocations.First.SubLocations
                        Dim s = "complement(" & LociBuilder.GetLocationString(subloci) & ")"
                        out.Add(LociBuilder.GetLocation(s))
                        out.Last.Accession = Loci.Accession
                    Next
                    Return out
                Else ' complement no join
                    out.Add(Loci)
                    out.Last.Accession = Loci.Accession
                    Return out
                End If

            End Function
            Public Shared Function Get_All_Exon_Location(Loci As Bio.IO.GenBank.Location) As List(Of Bio.IO.GenBank.ILocation)
                If IsNothing(Loci) = True Then Return Nothing
                Dim out As New List(Of ILocation)
                If Loci.SubLocations.Count = 0 Then ' No complement no join
                    out.Add(Loci)
                    out.Last.Accession = Loci.Accession
                    Return out
                End If
                If Loci.Operator <> LocationOperator.Complement Then ' no complement join
                    For Each subL In Loci.SubLocations
                        subL.Accession = Loci.Accession
                    Next
                    Return Loci.SubLocations
                End If
                If Loci.SubLocations.First.Operator = LocationOperator.Join Then ' complement join
                    For Each subloci As Bio.IO.GenBank.Location In Loci.SubLocations.First.SubLocations
                        Dim s = "complement(" & LociBuilder.GetLocationString(subloci) & ")"
                        out.Add(LociBuilder.GetLocation(s))
                        out.Last.Accession = Loci.Accession
                    Next
                    Return out
                Else ' complement no join
                    out.Add(Loci)
                    out.Last.Accession = Loci.Accession
                    Return out
                End If

            End Function


            Public Shared Function Get_Full_Location(loci As ILocation) As ILocation
                If loci.Operator = LocationOperator.Join Then
                    Dim x = Szunyi.Location.Common.Get_Location(loci.LocationStart & ".." & loci.LocationEnd)
                    Return x
                ElseIf loci.Operator = LocationOperator.Complement Then
                    Dim x = Szunyi.Location.Common.Get_Location("complement(" & loci.LocationStart & ".." & loci.LocationEnd & ")")
                    Return x
                Else
                    Dim nLoci = loci.Clone

                    Return nLoci
                End If

            End Function
            Public Shared Function Get_Location(loci As String) As ILocation
                If loci = "" Then Return Nothing
                Return LociBuilder.GetLocation(loci)
            End Function
            Public Shared Function GetLocation(SAMs As List(Of SAMAlignedSequence)) As List(Of ILocation)
                Dim Out As New List(Of ILocation)
                Dim x = Szunyi.Alignment.Own_Al_Helper.Get_List(SAMs)
                Return GetLocation(x)
            End Function
            Public Shared Function GetLocation(x As List(Of Own_Al)) As List(Of ILocation)
                Dim out As New List(Of ILocation)
                For Each Item In x
                    out.Add(GetLocation(Item))
                Next
                Return out
            End Function
            Public Shared Function GetLocation(SAM As SAMAlignedSequence) As ILocation
                Dim x1 As New Own_Al(SAM)

                Return GetLocation(x1)
            End Function


            ''' <summary>
            ''' start, end, strand (+,-) Correct start and end
            ''' </summary>
            ''' <param name="st"></param>
            ''' <param name="endy"></param>
            ''' <param name="strand"></param>
            ''' <returns></returns>
            Public Shared Function GetLocation(st As String, endy As String, strand As String) As ILocation
                Dim s As Integer = st
                Dim e As Integer = endy
                If e < s Then
                    Dim tmp = e
                    e = s
                    s = tmp
                End If
                If strand = "+" Then
                    Return Get_Location(s & ".." & e)
                Else
                    Return Get_Location("complement(" & s & ".." & e & ")")
                End If

            End Function
            Public Shared Function GetLocation(st As Integer, endy As Integer, strand As String) As ILocation
                Dim s As Integer = st
                Dim e As Integer = endy
                If e < s Then
                    Dim tmp = e
                    e = s
                    s = tmp
                End If
                If strand = "+" Then
                    Return Get_Location(s & ".." & e)
                Else
                    Return Get_Location("complement(" & s & ".." & e & ")")
                End If

            End Function
            Public Shared Function GetLocation(st As Integer, endy As Integer, IsC As Boolean) As ILocation
                Dim s As Integer = st
                Dim e As Integer = endy
                If e < s Then
                    Dim tmp = e
                    e = s
                    s = tmp
                End If
                If IsC = True Then
                    If s < e Then

                    End If
                    Return Get_Location("complement(" & s & ".." & e & ")")
                Else
                    Return Get_Location(s & ".." & e)
                End If
            End Function
            Public Shared Function GetLocation(st As Integer, endy As Integer, Op As Bio.IO.GenBank.LocationOperator) As ILocation
                Dim s As Integer = st
                Dim e As Integer = endy
                If e < s Then
                    Dim tmp = e
                    e = s
                    s = tmp
                End If
                If Op = LocationOperator.Complement Then
                    Return Get_Location("complement(" & s & ".." & e & ")")

                Else
                    Return Get_Location(s & ".." & e)
                End If

            End Function
            Public Shared Function GetLocation(position As String, orientation As String) As ILocation
                Dim pos As Integer = position
                If orientation <> "+" Then
                    Return GetLocation(pos, pos - 1, True)
                Else
                    Return GetLocation(pos, pos + 1, False)
                End If

            End Function
            Public Shared Function GetLocation(pos As Integer, isCommplementer As Boolean) As ILocation
                If isCommplementer = True Then
                    Return GetLocation(pos, pos - 1, isCommplementer)
                Else
                    Return GetLocation(pos, pos + 1, isCommplementer)
                End If

            End Function
            Public Shared Function GetLocation(Tss_pos As Integer, PAS_POS As Integer) As ILocation
                If Tss_pos < PAS_POS Then
                    Return GetLocation(Tss_pos, PAS_POS, False)
                Else
                    Return GetLocation(Tss_pos, PAS_POS, True)
                End If
            End Function
            Public Shared Function GetLocation(locis As List(Of ILocation), Op As LocationOperator) As ILocation
                Dim Sorted = From x In locis Order By x.LocationStart
                Dim str As New System.Text.StringBuilder
                For Each s In Sorted
                    str.Append(s.LocationStart & ".." & s.LocationEnd & ",")
                Next
                str.Length -= 1
                If Op = LocationOperator.Complement Then
                    If locis.Count > 1 Then
                        Return Get_Location("complement(join(" & str.ToString & "))")
                    Else
                        Return Get_Location("complement(" & str.ToString & ")")
                    End If
                Else
                    If locis.Count > 1 Then
                        Return Get_Location("join(" & str.ToString & ")")
                    Else
                        Return Get_Location(str.ToString)
                    End If
                End If
            End Function
            Public Shared Function GetLocation(x As Szunyi.Location.Basic_Location) As Bio.IO.GenBank.ILocation
                Dim s As String = x.Start & ".." & x.Endy
                If x.IsComplement = Szunyi.Constants.Strand_Type.Negative_strand Then
                    s = "complement(" & s & ")"
                End If
                Dim l = LociBuilder.GetLocation(s)
                l.Accession = x.SeqID
                Return l
            End Function

            ''' <summary>
            ''' Return Range From Start or End Depends On Setting
            ''' </summary>
            ''' <param name="locis"></param>
            ''' <param name="score"></param>
            ''' <returns></returns>
            Public Shared Function Get_Location(locis As List(Of ILocation), score As Szunyi.Constants.Sort_Locations_By) As ILocation
                Dim min = Get_Sites_Or_Positions.Get_Site(locis, score, Get_Position_From_LocationsBy.minvalue)
                Dim max = Get_Sites_Or_Positions.Get_Site(locis, score, Get_Position_From_LocationsBy.maxvalue)

                If locis.First.Operator <> LocationOperator.Complement Then
                    Return LociBuilder.GetLocation(min & ".." & max)
                Else
                    Return LociBuilder.GetLocation("complement(" & min & ".." & max & ")")
                End If
            End Function
            Public Shared Function Get_Location(start As Integer, endy As Integer, strand As String) As ILocation
                If start > endy Then
                    If strand = "+" Then
                        Return LociBuilder.GetLocation(endy & ".." & start)
                    Else
                        Return LociBuilder.GetLocation("complement(" & endy & ".." & start & ")")
                    End If
                Else
                    If strand = "+" Then
                        Return LociBuilder.GetLocation(start & ".." & endy)
                    Else
                        Return LociBuilder.GetLocation("complement(" & start & ".." & endy & ")")
                    End If
                End If
            End Function
            Public Shared Function Get_Location(locis As List(Of FeatureItem), score As Szunyi.Constants.Sort_Locations_By) As ILocation
                Dim min = Get_Sites_Or_Positions.Get_Site(locis, score, Get_Position_From_LocationsBy.minvalue)
                Dim max = Get_Sites_Or_Positions.Get_Site(locis, score, Get_Position_From_LocationsBy.maxvalue)
                If min <> max Then
                    Dim alf As Int16 = 54
                End If
                If locis.First.Location.Operator <> LocationOperator.Complement Then
                    Return Szunyi.Features.FeatureManipulation.GetLocations.LociBuilder.GetLocation(min & ".." & max)
                Else
                    Return Szunyi.Features.FeatureManipulation.GetLocations.LociBuilder.GetLocation("complement(" & min & ".." & max & ")")
                End If
            End Function

#End Region
#Region "Accession"
            Public Shared Function Get_Locis_Accesions(locis As List(Of List(Of ILocation))) As List(Of String)
                Dim out As New List(Of String)
                For Each Loci In locis
                    For Each Locz In Loci
                        out.Add(Locz.Accession)
                    Next
                Next
                Return out
            End Function
            Public Shared Function Get_Locis_Accesions(locis As List(Of ILocation)) As List(Of String)
                Dim out As New List(Of String)

                For Each Loci In locis

                    out.Add(Loci.Accession)

                Next
                Return out
            End Function

#End Region
#Region "Counts"
            Public Shared Function Get_Count(Locis As List(Of List(Of ILocation))) As Integer
                Dim c As Integer = 0
                For Each Loci In Locis
                    c += Loci.Count
                Next
                Return c
            End Function
            Public Shared Function Get_Count(locis As List(Of ILocation)) As Integer
                Return locis.Count
            End Function
#End Region
#Region "Start End"

            ''' <summary>
            ''' Always + strand
            ''' </summary>
            ''' <param name="locis"></param>
            ''' <returns></returns>
            Public Shared Function Get_Start_Location(locis As List(Of ILocation)) As List(Of ILocation)
                Dim out As New List(Of ILocation)
                For Each loci In locis
                    out.Add(LociBuilder.GetLocation(loci.LocationStart & ".." & loci.LocationStart))
                Next
                Return out
            End Function
            ''' <summary>
            ''' Always + strand
            ''' </summary>
            ''' <param name="locis"></param>
            ''' <returns></returns>
            Public Shared Function Get_End_Location(locis As List(Of ILocation)) As List(Of ILocation)
                Dim out As New List(Of ILocation)
                For Each loci In locis
                    out.Add(LociBuilder.GetLocation(loci.LocationEnd & ".." & loci.LocationEnd))
                Next
                Return out
            End Function


            Public Shared Function Get_No_Complement_Locations(locis As List(Of ILocation)) As List(Of ILocation)
                Dim out As New List(Of ILocation)
                For Each Loci In locis
                    out.Add(Get_No_Complement_Locations(Loci))
                Next
                Return out
            End Function
            Public Shared Function Get_No_Complement_Locations(loci As ILocation) As ILocation
                If IsNothing(loci.Accession) = True OrElse loci.Accession = "" Then
                    Dim alf As Int16 = 54
                End If
                If loci.Operator = LocationOperator.Complement Then
                    loci.SubLocations.First.Accession = loci.Accession
                    Return loci.SubLocations.First
                Else
                    Return loci
                End If

            End Function
            Public Shared Function Get_Correct_Location(locis As List(Of ILocation)) As List(Of ILocation)
                Dim out As New List(Of ILocation)
                For Each Loci In locis
                    out.Add(Get_Correct_Location(Loci))
                Next
                Return out
            End Function
            Public Shared Function Get_Correct_Location(loci As ILocation) As ILocation
                If loci.Operator = LocationOperator.Complement Then
                    If loci.SubLocations.First.Operator = LocationOperator.Join Then
                        Dim res = From x In loci.SubLocations.First.SubLocations Order By x.LocationStart Ascending

                        loci.SubLocations.First.SubLocations = res.ToList
                    End If
                ElseIf loci.Operator = LocationOperator.Join Then
                    Dim res = From x In loci.SubLocations Order By x.LocationStart Ascending

                    If loci.SubLocations.First.Operator = LocationOperator.Complement Then
                        ' loci.SubLocations.Clear()
                        Dim x = Szunyi.Features.FeatureManipulation.GetLocations.LociBuilder.GetLocation(loci.LocationStart & ".." & loci.LocationEnd)
                        x.Operator = LocationOperator.Complement
                        Dim x1 = Szunyi.Features.FeatureManipulation.GetLocations.LociBuilder.GetLocation(loci.LocationStart & ".." & loci.LocationEnd)
                        x1.Operator = LocationOperator.Join
                        Dim cOper = loci.Clone
                        Dim ls As New List(Of ILocation)
                        For Each sl In res
                            x1.SubLocations.Add(Szunyi.Features.FeatureManipulation.GetLocations.LociBuilder.GetLocation(sl.LocationStart & ".." & sl.LocationEnd))

                        Next

                        x.SubLocations.Add(x1)
                        Return x
                    Else
                        loci.SubLocations = res.ToList
                    End If
                End If
                Return loci

            End Function
            Public Shared Function Create_Biggest(Locis As List(Of ILocation)) As ILocation
                Dim s = (From x In Locis Select x.LocationStart).Min
                Dim e = (From x In Locis Select x.LocationEnd).Max
                Dim l As Bio.IO.GenBank.Location = Szunyi.Features.FeatureManipulation.GetLocations.LociBuilder.GetLocation(s & ".." & e)
                If Locis.First.Operator = LocationOperator.Complement Then
                    l = Szunyi.Features.FeatureManipulation.GetLocations.Change_Strand(l)
                End If

                If l.LocationEnd - l.LocationStart > 10000 Then
                    Dim alf As Int16 = 43
                End If
                Return l
            End Function

            Public Shared Function IsOverLapped(gExon1 As ILocation, gExon2 As ILocation) As Boolean
                If gExon1.LocationEnd >= gExon2.LocationStart And gExon1.LocationEnd <= gExon2.LocationEnd Then
                    Return True
                End If
                If gExon1.LocationStart >= gExon2.LocationStart And gExon1.LocationStart <= gExon2.LocationEnd Then
                    Return True
                End If
                Return False
            End Function

            Public Shared Function Get_Basic_Location(start As Integer, endy As Integer, Strand As String) As Basic_Location
                Dim l = Szunyi.Location.Common.GetLocation(start, endy, Strand)
                Dim x = Szunyi.Location.Basic_Location_Manipulation.Get_Basic_Location(l)
                Return x

            End Function
            Public Shared Function Get_Basic_Location(start As Integer, endy As Integer, Strand As Bio.IO.GenBank.LocationOperator) As Basic_Location
                If Strand = LocationOperator.Complement Then
                    Dim l = Szunyi.Location.Common.GetLocation(start, endy, "-")
                    Dim x = Szunyi.Location.Basic_Location_Manipulation.Get_Basic_Location(l)
                    Return x
                Else
                    Dim l = Szunyi.Location.Common.GetLocation(start, endy, "+")
                    Dim x = Szunyi.Location.Basic_Location_Manipulation.Get_Basic_Location(l)
                    Return x
                End If


            End Function
            Public Shared Function Get_Basic_Location(loci As ILocation, range As Integer, ExtendBy As Constants.Sort_Locations_By) As Basic_Location
                Select Case ExtendBy
                    Case Sort_Locations_By.TSS
                        Dim l = Szunyi.Location.Common.GetLocation(loci.TSS + range, loci.PAS, loci.IsComplementer)
                        Dim x = Szunyi.Location.Basic_Location_Manipulation.Get_Basic_Location(l)
                        Return x
                    Case Sort_Locations_By.LE
                        Dim l = Szunyi.Location.Common.GetLocation(loci.LocationStart, loci.LocationEnd + range, loci.IsComplementer)
                        Dim x = Szunyi.Location.Basic_Location_Manipulation.Get_Basic_Location(l)
                        Return x
                    Case Sort_Locations_By.TSS_PAS
                        Dim l = Szunyi.Location.Common.GetLocation(loci.LocationStart + range, loci.LocationEnd + range, loci.IsComplementer)
                        Dim x = Szunyi.Location.Basic_Location_Manipulation.Get_Basic_Location(l)
                        Return x

                    Case Sort_Locations_By.PAS

                        Dim l = Szunyi.Location.Common.GetLocation(loci.TSS, loci.PAS + range, loci.IsComplementer)
                        Dim x = Szunyi.Location.Basic_Location_Manipulation.Get_Basic_Location(l)
                        Return x
                    Case Sort_Locations_By.LS

                        Dim l = Szunyi.Location.Common.GetLocation(loci.LocationStart + range, loci.LocationEnd, loci.IsComplementer)
                        Dim x = Szunyi.Location.Basic_Location_Manipulation.Get_Basic_Location(l)
                        Return x
                End Select


            End Function

            Friend Shared Function Set_Direction(Locis As List(Of ILocation), ForPositive As Boolean) As List(Of ILocation)
                Dim out As New List(Of ILocation)
                If ForPositive = True Then
                    For Each loci In Locis
                        loci.IsComplementer = False
                        out.Add(loci)
                    Next
                Else
                    For Each loci In Locis
                        loci.IsComplementer = True
                        out.Add(loci)
                    Next
                End If
                Return out
            End Function


            Friend Shared Function Get_Value(location As ILocation, sort As Sort_Locations_By) As Integer
                Select Case sort
                    Case Sort_Locations_By.LE
                        Return location.LocationEnd
                    Case Sort_Locations_By.LS
                        Return location.LocationStart
                    Case Sort_Locations_By.PAS
                        Return location.PAS
                    Case Sort_Locations_By.TSS
                        Return location.TSS
                    Case Else
                        Return -1
                End Select

            End Function
            ''' <summary>
            ''' Return + or -
            ''' </summary>
            ''' <param name="location"></param>
            ''' <returns></returns>
            Public Shared Function Get_Strand(location As ILocation) As String
                If location.IsComplementer = True Then
                    Return "-"
                Else
                    Return "+"
                End If
            End Function
            ''' <summary>
            ''' Return + or -
            ''' </summary>
            ''' <param name="isComplementer"></param>
            ''' <returns></returns>
            Public Shared Function Get_Strand(isComplementer As Boolean) As String
                If isComplementer = True Then
                    Return "-"
                Else
                    Return "+"
                End If
            End Function

            Public Shared Function GetLocation(Own_AL As Szunyi.Alignment.Own_Al) As ILocation
                Dim Introns = From x In Own_AL.Parts Where x.Type = Own_Al.Type.Intron
                Dim RefStart As Integer = 0
                If Own_AL.Parts.First.Type = Own_Al.Type.Soft_Clip Then
                    RefStart = Own_AL.Parts(1).Ref_Start
                Else
                    RefStart = Own_AL.Parts.First.Ref_Start
                End If
                Dim RefEnd As Integer = 0
                If Own_AL.Parts.Last.Type = Own_Al.Type.Soft_Clip Then
                    RefEnd = Own_AL.Parts(Own_AL.Parts.Count - 2).Ref_End
                Else
                    RefEnd = Own_AL.Parts.Last.Ref_End
                End If

                Dim IsReverse As Boolean = False
                If Own_AL.Sam.Flag = SAMFlags.QueryOnReverseStrand Then
                    IsReverse = True
                End If

                If Introns.Count = 0 Then
                    Dim l = GetLocation(RefStart, RefEnd, IsReverse)
                    l.Accession = Own_AL.Sam.QName
                    Return l
                Else
                    Dim l = GetLocation(RefStart, RefEnd, IsReverse)
                    l.Accession = Own_AL.Sam.QName
                    Dim Exons As New List(Of ILocation)
                    Exons.Add(GetLocation(RefStart, Introns(0).Ref_Start - 1))
                    For i1 = 0 To Introns.Count - 2
                        Exons.Add(GetLocation(Introns(i1).Ref_End + 1, Introns(i1 + 1).Ref_Start - 1))
                    Next
                    Exons.Add(GetLocation(Introns.Last.Ref_End + 1, RefEnd))
                    Exons = (From x In Exons Where x.LocationEnd - x.LocationStart > 0).ToList
                    If IsReverse = True Then
                        l.SubLocations.First.Operator = LocationOperator.Join
                        l.SubLocations.First.IsComplementer = True
                        l.SubLocations.First.SubLocations.AddRange(Exons)
                    Else
                        l.Operator = LocationOperator.Join
                        l.SubLocations.AddRange(Exons)
                    End If

                    Return l
                End If

            End Function

            Public Shared Function Get_LocationString_wIntron(location As ILocation) As String
                Dim str As New System.Text.StringBuilder
                str.Append(GetLocationStringTab(location)).Append(vbTab)
                Dim Introns = Szunyi.Location.Common.Get_All_Intron_Location(location)
                For Each I In Introns
                    str.Append(vbTab).Append(Szunyi.Location.Common.Get_LocationString_LSLE(I))
                Next
                Return str.ToString
            End Function
#End Region
            Public Shared Function modify(Loci As Bio.IO.GenBank.Location, m As Integer) As Bio.IO.GenBank.Location

                Dim Others As New List(Of String)
                Dim l = GetLocationString(Loci)
                Dim IsDigit As Boolean = Char.IsNumber(l.First)
                Dim curr As String = ""
                For Each c As Char In l
                    If [Char].IsNumber(c) Then
                        If IsDigit = True Then
                            curr = curr & c
                        Else
                            IsDigit = True
                            Others.Add(curr)
                            curr = c
                        End If
                    Else
                        If IsDigit = False Then
                            curr = curr & c
                        Else
                            Others.Add(curr)
                            curr = c
                            IsDigit = False
                        End If
                    End If
                Next
                Others.Add(curr)
                Dim sb As New System.Text.StringBuilder
                For Each Item In Others
                    Dim res As Integer
                    If Integer.TryParse(Item, res) = True Then
                        sb.Append(res + m)
                    Else
                        sb.Append(Item)
                    End If
                Next
                Return Szunyi.Location.Common.Get_Location(sb.ToString)
            End Function
        End Class

        Public Class OverLapping_Locations
            Public Class wLength
                Public Shared Function Get_Maximal_Overlapping_Features(Basic As FeatureItem, Features As List(Of FeatureItem), wOrientation As Boolean) As List(Of FeatureItem)
                    Dim All As New List(Of Dictionary(Of Integer, List(Of FeatureItem)))
                    All.Add(Get_5_Prime_Overhangs(Basic, Features, wOrientation))
                    ALl.Add(Get_3_Prime_Overhangs(Basic, Features, wOrientation))
                    ALl.Add(Get_Inner_Feature_Items(Basic, Features, wOrientation))

                    Return Merge(All)
                End Function
                Private Shared Function Merge(All As List(Of Dictionary(Of Integer, List(Of FeatureItem)))) As List(Of FeatureItem)
                    Dim out As New Dictionary(Of Integer, List(Of FeatureItem))
                    For Each ls In All
                        For Each Item In ls
                            If out.ContainsKey(Item.Key) = False Then out.Add(Item.Key, New List(Of FeatureItem))
                            out(Item.Key).AddRange(Item.Value)
                        Next
                    Next
                    Dim Keys = out.Keys
                    Dim max = Keys.Max
                    Return out(max)
                End Function


                Public Shared Function Get_Inner_Feature_Items(Basic As FeatureItem, Features As List(Of FeatureItem), wOrientation As Boolean) As Dictionary(Of Integer, List(Of FeatureItem))
                    Dim out = From x In Features Where x.Location.LocationStart >= Basic.Location.LocationStart And
                                                 x.Location.LocationEnd <= Basic.Location.LocationEnd

                    Dim res As New Dictionary(Of Integer, List(Of FeatureItem))
                    For Each Item In out
                        Dim OL_Length = Item.Location.LocationEnd - Item.Location.LocationStart
                        If wOrientation = True Then
                            If Item.Location.IsComplementer = Basic.Location.IsComplementer Then
                                If res.ContainsKey(OL_Length) = False Then res.Add(OL_Length, New List(Of FeatureItem))
                                res(OL_Length).Add(Item)
                            End If
                        Else
                            If res.ContainsKey(OL_Length) = False Then res.Add(OL_Length, New List(Of FeatureItem))
                            res(OL_Length).Add(Item)
                        End If
                    Next
                    Return res

                End Function
                Public Shared Function Get_5_Prime_Overhangs(basic As FeatureItem, Features As List(Of FeatureItem), wOrientation As Boolean) As Dictionary(Of Integer, List(Of FeatureItem))
                    Dim out = From x In Features Where x.Location.LocationStart <= basic.Location.LocationStart And
                                                 x.Location.LocationEnd <= basic.Location.LocationEnd And
                                                 x.Location.LocationEnd > basic.Location.LocationStart

                    Dim res As New Dictionary(Of Integer, List(Of FeatureItem))
                    For Each Item In out
                        Dim OL_Length = Item.Location.LocationEnd - basic.Location.LocationStart
                        If wOrientation = True Then
                            If Item.Location.IsComplementer = basic.Location.IsComplementer Then
                                If res.ContainsKey(OL_Length) = False Then res.Add(OL_Length, New List(Of FeatureItem))
                                res(OL_Length).Add(Item)
                            End If
                        Else
                            If res.ContainsKey(OL_Length) = False Then res.Add(OL_Length, New List(Of FeatureItem))
                            res(OL_Length).Add(Item)
                        End If
                    Next
                    Return res

                End Function
                Public Shared Function Get_3_Prime_Overhangs(basic As FeatureItem, Features As List(Of FeatureItem), wOrientation As Boolean) As Dictionary(Of Integer, List(Of FeatureItem))
                    Dim out = From x In Features Where x.Location.LocationStart > basic.Location.LocationStart And
                                                 x.Location.LocationEnd > basic.Location.LocationEnd And
                                                 x.Location.LocationStart < basic.Location.LocationEnd

                    Dim res As New Dictionary(Of Integer, List(Of FeatureItem))
                    For Each Item In out
                        Dim OL_Length = basic.Location.LocationEnd - Item.Location.LocationStart
                        If wOrientation = True Then
                            If Item.Location.IsComplementer = basic.Location.IsComplementer Then
                                If res.ContainsKey(OL_Length) = False Then res.Add(OL_Length, New List(Of FeatureItem))
                                res(OL_Length).Add(Item)
                            End If
                        Else
                            If res.ContainsKey(OL_Length) = False Then res.Add(OL_Length, New List(Of FeatureItem))
                            res(OL_Length).Add(Item)
                        End If
                    Next
                    Return res

                End Function

            End Class
            ''' <summary>
            ''' 'Basic is longer then 
            ''' </summary>
            ''' <param name="Basic"></param>
            ''' <param name="Features"></param>
            ''' <returns></returns>
            Public Shared Function Get_Inner_Feature_Items_wOrientation(Basic As FeatureItem, Features As List(Of FeatureItem)) As List(Of FeatureItem)
                Dim out = From x In Features Where x.Location.LocationStart >= Basic.Location.LocationStart And
                                                 x.Location.LocationEnd <= Basic.Location.LocationEnd And
                                                    Basic.Location.IsComplementer = x.Location.IsComplementer

                If out.Count > 0 Then Return out.ToList
                Return New List(Of FeatureItem)

            End Function
            ''' <summary>
            ''' 'Basic is longer then 
            ''' </summary>
            ''' <param name="Basic"></param>
            ''' <param name="Features"></param>
            ''' <returns></returns>
            Public Shared Function Get_Inner_Feature_Items_woOrientation(Basic As FeatureItem, Features As List(Of FeatureItem)) As List(Of FeatureItem)
                Dim out = From x In Features Where x.Location.LocationStart >= Basic.Location.LocationStart And
                                                 x.Location.LocationEnd <= Basic.Location.LocationEnd

                If out.Count > 0 Then Return out.ToList
                Return New List(Of FeatureItem)

            End Function
            Public Shared Function Get_5_Prime_Overhangs_wOrientation(basic As FeatureItem, Features As List(Of FeatureItem)) As List(Of FeatureItem)
                Dim out = From x In Features Where x.Location.LocationStart < basic.Location.LocationStart And
                                                 x.Location.LocationEnd < basic.Location.LocationEnd And
                                                 x.Location.LocationEnd >= basic.Location.LocationStart And
                                                 basic.Location.IsComplementer = x.Location.IsComplementer
                          Order By x.Location.LocationEnd - basic.Location.LocationStart Descending

                If out.Count > 0 Then Return out.ToList
                Return New List(Of FeatureItem)

            End Function
            Public Shared Function Get_5_Prime_Overhangs_woOrientation(basic As FeatureItem, Features As List(Of FeatureItem)) As List(Of FeatureItem)
                Dim out = From x In Features Where x.Location.LocationStart < basic.Location.LocationStart And
                                                 x.Location.LocationEnd < basic.Location.LocationEnd And
                                                 x.Location.LocationEnd > basic.Location.LocationStart
                          Order By x.Location.LocationEnd - basic.Location.LocationStart Descending

                If out.Count > 0 Then Return out.ToList
                Return New List(Of FeatureItem)

            End Function
            Public Shared Function Get_3_Prime_Overhangs_wOrientation(basic As FeatureItem, Features As List(Of FeatureItem)) As List(Of FeatureItem)
                Dim out = From x In Features Where x.Location.LocationStart > basic.Location.LocationStart And
                                                 x.Location.LocationEnd > basic.Location.LocationEnd And
                                                 x.Location.LocationStart < basic.Location.LocationEnd And
                                                 basic.Location.IsComplementer = x.Location.IsComplementer
                          Order By basic.Location.LocationEnd - x.Location.LocationStart Descending

                If out.Count > 0 Then Return out.ToList
                Return New List(Of FeatureItem)

            End Function
            Public Shared Function Get_3_Prime_Overhangs_woOrientation(basic As FeatureItem, Features As List(Of FeatureItem)) As List(Of FeatureItem)
                Dim out = From x In Features Where x.Location.LocationStart > basic.Location.LocationStart And
                                                 x.Location.LocationEnd > basic.Location.LocationEnd And
                                                 x.Location.LocationStart < basic.Location.LocationEnd
                          Order By basic.Location.LocationEnd - x.Location.LocationStart Descending

                If out.Count > 0 Then Return out.ToList
                Return New List(Of FeatureItem)

            End Function
            Public Shared Function Get_5_and_3_Prime_Overhangs(basic As FeatureItem, Features As List(Of FeatureItem)) As List(Of FeatureItem)
                Dim out = From x In Features Where x.Location.LocationStart < basic.Location.LocationStart And x.Location.LocationEnd > basic.Location.LocationEnd And basic.Location.Operator = x.Location.Operator

                If out.Count > 0 Then Return out.ToList
                Return New List(Of FeatureItem)
            End Function
            Public Shared Function Get_Biggers_Overhangs_wOrientation(basic As FeatureItem, Features As List(Of FeatureItem)) As List(Of FeatureItem)
                Dim out = From x In Features Where x.Location.LocationStart < basic.Location.LocationStart And
                                                 x.Location.LocationEnd > basic.Location.LocationEnd And
                                                 basic.Location.IsComplementer = x.Location.IsComplementer

                If out.Count > 0 Then Return out.ToList
                Return New List(Of FeatureItem)
            End Function
            Public Shared Function Get_Biggers_Overhangs_woOrientation(basic As FeatureItem, Features As List(Of FeatureItem)) As List(Of FeatureItem)
                Dim out = From x In Features Where x.Location.LocationStart < basic.Location.LocationStart And
                                                 x.Location.LocationEnd > basic.Location.LocationEnd

                If out.Count > 0 Then Return out.ToList
                Return New List(Of FeatureItem)
            End Function
            Public Shared Function Get_All_Overlaps_Features(basic As FeatureItem, Features As List(Of FeatureItem)) As List(Of FeatureItem)
                Dim out As New List(Of FeatureItem)
                out.AddRange(Get_Inner_Feature_Items_wOrientation(basic, Features))
                out.AddRange(Get_5_Prime_Overhangs_wOrientation(basic, Features))
                out.AddRange(Get_3_Prime_Overhangs_wOrientation(basic, Features))
                out.AddRange(Get_Biggers_Overhangs_wOrientation(basic, Features))

                Dim sg = out.Distinct.ToList

                Return sg
            End Function

            Public Shared Function Get_Longest_OverLapping_Item_wOrientation(tr As FeatureItem, Features As List(Of FeatureItem)) As FeatureItem
                Dim F = Get_5_Prime_Overhangs_wOrientation(tr, Features)
                Dim T = Get_3_Prime_Overhangs_wOrientation(tr, Features)
                Dim I = Get_Inner_Feature_Items_wOrientation(tr, Features)
                Dim B = Get_Biggers_Overhangs_wOrientation(tr, Features)

                If F.Count = 0 And T.Count = 0 Then
                    If I.Count Or B.Count > 0 Then
                        Return B.First
                    Else
                        Dim res = From x In Features Where x.Location.TSS <= tr.Location.TSS And x.Location.PAS >= tr.Location.PAS And tr.Location.IsComplementer = x.Location.IsComplementer

                        If res.Count > 0 Then
                            Return res.First
                        Else
                            Return Nothing
                        End If
                    End If
                End If
                If F.Count > 0 Then
                    If T.Count = 0 Then
                        Return F.First
                    Else
                        If F.First.Location.LocationEnd - tr.Location.LocationStart >= tr.Location.LocationEnd - T.First.Location.LocationStart Then
                            Return F.First
                        Else
                            Return T.First
                        End If
                    End If
                End If
                Return T.First
            End Function
            Public Shared Function Get_Longest_OverLapping_Item_woOrientation(tr As FeatureItem, Features As List(Of FeatureItem)) As FeatureItem
                Dim F = Get_5_Prime_Overhangs_woOrientation(tr, Features)
                Dim T = Get_3_Prime_Overhangs_woOrientation(tr, Features)
                Dim I = Get_Inner_Feature_Items_woOrientation(tr, Features)
                Dim B = Get_Biggers_Overhangs_woOrientation(tr, Features)
                Dim c As FeatureItem
                If F.Count = 0 And T.Count = 0 Then
                    If I.Count > 0 Then
                        Return I.First
                    End If
                    If B.Count > 0 Then
                        Return B.First
                    End If
                    Dim res = From x In Features Where x.Location.LocationStart <= tr.Location.LocationStart And x.Location.LocationEnd >= tr.Location.LocationEnd
                    If res.Count > 0 Then
                        Return res.First
                    Else
                        Return Nothing
                    End If

                End If
                If F.Count > 0 Then
                    If T.Count = 0 Then
                        Return F.First
                    Else
                        If F.First.Location.LocationEnd - tr.Location.LocationStart >= tr.Location.LocationEnd - T.First.Location.LocationStart Then
                            Return F.First
                        Else
                            Return T.First
                        End If
                    End If
                End If
                Return T.First
            End Function

            ''' <summary>
            ''' Retrun empty List , or location groups are not overlappings
            ''' </summary>
            ''' <param name="potential_Introns_Locations"></param>
            ''' <returns></returns>
            Public Shared Iterator Function Get_Non_OverLappingGroups(potential_Introns_Locations As IEnumerable(Of ILocation)) As IEnumerable(Of List(Of ILocation))

                Select Case potential_Introns_Locations.Count
                    Case 0
                        Yield New List(Of ILocation)
                    Case 1
                        Yield potential_Introns_Locations.ToList
                    Case Else
                        potential_Introns_Locations = From x In potential_Introns_Locations Order By x.LocationStart Ascending
                        Dim t = potential_Introns_Locations.ToList
                        Dim OLs = Get_Overlapping_Locis(t)
                        Dim res As New List(Of Windows.Forms.TreeNode)
                        Dim cIndex As Integer = 0
                        For cIndex = 0 To t.Count - 1
                            Dim x As New Windows.Forms.TreeNode(cIndex)
                            res.Add(GetGraph(t, OLs, x, cIndex))
                        Next
                        Dim out As New List(Of String)
                        For Each Node In res
                            Dim sg As New List(Of String)
                            sg.Add(Node.Text)
                            Dim Ls As New List(Of String)
                            out.AddRange(Get_Node_FullText(Node, sg, Ls))
                        Next
                        For Each item In out
                            Dim s = Split(item, vbTab)
                            Dim kk As New List(Of ILocation)
                            For Each s1 In s
                                kk.Add(t(s1))
                            Next
                            Yield kk
                        Next
                        Dim kj As Int16 = 54
                End Select

            End Function
            Private Shared Function Get_Node_FullText(nodes As Windows.Forms.TreeNode, t As List(Of String), ls As List(Of String)) As List(Of String)

                For Each node As Windows.Forms.TreeNode In nodes.Nodes
                    If node.Text = "" Then
                        ls.Add(Szunyi.Text.General.GetText(t, vbTab))

                    Else
                        t.Add(node.Text)
                        Get_Node_FullText(node, t, ls)
                        t.RemoveAt(t.Count - 1)
                    End If

                Next
                Return ls
            End Function
            Private Shared Function GetGraph(Locis As List(Of ILocation), OLs As Dictionary(Of ILocation, List(Of Integer)), res As Windows.Forms.TreeNode, cIndex As Integer) As Windows.Forms.TreeNode
                res.Nodes.Add(New Windows.Forms.TreeNode("")) ' Close
                For i1 = cIndex + 1 To Locis.Count - 1
                    If OLs(Locis(cIndex)).Contains(i1) = True Then
                        ' Do Nothing It is ovelapping
                    Else
                        res.Nodes.Add(New Windows.Forms.TreeNode(i1))
                        GetGraph(Locis, OLs, res.Nodes.Item(res.Nodes.Count - 1), i1)
                    End If

                Next
                Return res
            End Function
            Private Shared Function Get_Overlapping_Locis(Locis As List(Of ILocation)) As Dictionary(Of ILocation, List(Of Integer))
                Dim out As New Dictionary(Of ILocation, List(Of Integer))
                For Each Loci In Locis
                    out.Add(Loci, New List(Of Integer))
                    Dim t = From x In Locis Where (x.LocationStart >= Loci.LocationStart And x.LocationStart <= Loci.LocationEnd) Or (x.LocationEnd >= Loci.LocationStart And x.LocationEnd <= Loci.LocationStart)

                    For Each Item In t
                        out(Loci).Add(Locis.IndexOf(Item))
                    Next

                Next
                Return out
            End Function
            Private Shared Function Get_Overlapping_Locis(Locis As List(Of Bio.IO.GenBank.Location)) As Dictionary(Of ILocation, List(Of Integer))
                Dim out As New Dictionary(Of ILocation, List(Of Integer))
                For Each Loci In Locis
                    out.Add(Loci, New List(Of Integer))
                    Dim t = From x In Locis Where (x.LocationStart >= Loci.LocationStart And x.LocationStart <= Loci.LocationEnd) Or (x.LocationEnd >= Loci.LocationStart And x.LocationEnd <= Loci.LocationStart)

                    For Each Item In t
                        out(Loci).Add(Locis.IndexOf(Item))
                    Next
                    out(Loci).AddRange(t.ToList)
                Next
                Return out
            End Function
        End Class

        Public Class Get_Sites_Or_Positions
            Public Shared Function Get_Sites(Groups As Dictionary(Of String, List(Of List(Of ILocation))), start As Sort_Locations_By, abundance As Get_Position_From_LocationsBy) As List(Of Integer)
                Dim out As New List(Of Integer)
                For Each Group In Groups
                    Dim Sites As New List(Of Integer)
                    For Each Locis In Group.Value
                        Sites.AddRange(Get_All_Position(Locis, start))
                    Next
                    out.Add(Get_Exact_Site(Sites, abundance))
                Next
                Return out
            End Function
            Public Shared Function Get_Site(Group As KeyValuePair(Of String, System.Collections.Generic.List(Of System.Collections.Generic.List(Of Bio.IO.GenBank.ILocation))),
                                        start As Sort_Locations_By, abundance As Szunyi.Constants.Get_Position_From_LocationsBy) As Integer

                Dim Sites As New List(Of Integer)
                For Each Locis In Group.Value
                    Sites.AddRange(Get_All_Position(Locis, start))
                Next
                Return Get_Exact_Site(Sites, abundance)
            End Function
            Public Shared Function Get_Site(Group As List(Of Bio.IO.GenBank.FeatureItem),
                                        start As Sort_Locations_By, abundance As Szunyi.Constants.Get_Position_From_LocationsBy) As Integer

                Dim Sites As New List(Of Integer)
                For Each Locis In Group
                    Sites.AddRange(Get_All_Position(Locis, start))
                Next
                Return Get_Exact_Site(Sites, abundance)
            End Function
            Public Shared Function Get_Site(locis As List(Of Bio.IO.GenBank.ILocation),
                                        start As Sort_Locations_By, abundance As Szunyi.Constants.Get_Position_From_LocationsBy) As Integer

                Dim Sites As New List(Of Integer)
                Sites.AddRange(Get_All_Position(locis, start))
                Return Get_Exact_Site(Sites, abundance)
            End Function
            Private Shared Function Get_Exact_Site(Sites As List(Of Integer), abundance As Get_Position_From_LocationsBy) As Integer
                If Sites.Count = 0 Then Return -1
                Select Case abundance
                    Case Get_Position_From_LocationsBy.abundance
                        Dim maxRepeatedItem = Sites.GroupBy(Function(x) x).OrderByDescending(Function(x) x.Count()).First().Key
                        Return maxRepeatedItem

                    Case Get_Position_From_LocationsBy.maxvalue
                        Return Sites.Max
                    Case Get_Position_From_LocationsBy.minvalue
                        Return Sites.Min
                    Case Get_Position_From_LocationsBy.weight
                        Dim x As Double = 0
                        For Each Site In Sites
                            x += Site
                        Next
                        Dim i As Integer = x / Sites.Count
                        Return i
                End Select
                Return Nothing
            End Function

            Private Shared Function Get_All_Position(locis As List(Of ILocation), start As Sort_Locations_By) As List(Of Integer)
                Dim out As New List(Of Integer)
                Select Case start
                    Case Sort_Locations_By.TSS
                        For Each loci In locis
                            out.Add(loci.LocationStart)
                        Next
                    Case Sort_Locations_By.LE
                        For Each loci In locis
                            out.Add(loci.LocationEnd)
                        Next
                    Case Sort_Locations_By.TSS_PAS
                        For Each loci In locis
                            out.Add(loci.LocationStart)
                            out.Add(loci.LocationEnd)
                        Next
                End Select
                Return out
            End Function

            Private Shared Function Get_All_Position(loci As FeatureItem, start As Sort_Locations_By) As List(Of Integer)
                Dim out As New List(Of Integer)
                Select Case start
                    Case Sort_Locations_By.TSS

                        out.Add(loci.Location.LocationStart)

                    Case Sort_Locations_By.LE

                        out.Add(loci.Location.LocationEnd)

                    Case Sort_Locations_By.TSS_PAS

                        out.Add(loci.Location.LocationStart)
                        out.Add(loci.Location.LocationEnd)

                End Select
                Return out
            End Function

        End Class

        Public Class LociBinary
            Public Shared Property Comp As New Szunyi.Location.BioLocation_Comparers.Contain_Full

            Public Shared Property cBio_Location_ByStart As New BioLocation_Comparers._ByStart
            Public Shared Property cBio_Location_ByEnd As New BioLocation_Comparers._ByEnd
            Public Shared Property cBio_Location_Contain_Full As New BioLocation_Comparers.Contain_Full
            Public Shared Property cBio_Location_Contain_End As New BioLocation_Comparers.Contain_End
            Public Shared Property cBio_Location_Contain_Start As New BioLocation_Comparers.Contain_Start


            Public Shared Property cBasic_Location_ByStart_wStrand As New Basic_Location_Comparers._ByStart_wStrand
            Public Shared Property cBasic_Location_ByEnd_wStrand As New Basic_Location_Comparers._ByEnd_wStrand
            Public Shared Property cBasic_Location_ByTSS_wStrand As New Basic_Location_Comparers._ByTSS_wStrand
            Public Shared Property cBasic_Location_ByPAS_wStrand As New Basic_Location_Comparers._ByPAS_wStrand

            Public Shared Property cBasic_Location_ByStart_woStrand As New Basic_Location_Comparers._ByStart_woStrand
            Public Shared Property cBasic_Location_ByEnd_woStrand As New Basic_Location_Comparers._ByEnd_woStrand
            Public Shared Property cBasic_Location_ByTSS_woStrand As New Basic_Location_Comparers._ByTSS_woStrand
            Public Shared Property cBasic_Location_ByPAS_woStrand As New Basic_Location_Comparers._ByPAS_woStrand

            Public Shared Property cBasic_Location_Contain_Full As New Basic_Location_Comparers.Contain_Full
            Public Shared Property cBasic_Location_Contain_End As New Basic_Location_Comparers.Contain_End
            Public Shared Property cBasic_Location_Contain_Start As New Basic_Location_Comparers.Contain_Start


            Public Shared Property cFeature_Location_ByStart As New Features_Location_Comparers._ByStart
            Public Shared Property cFeature_Location_ByEnd As New Features_Location_Comparers._ByEnd
            Public Shared Property cFeature_Location_Contain_Full As New Features_Location_Comparers.Contain_Full
            Public Shared Property cFeature_Location_Contain_End As New Features_Location_Comparers.Contain_End
            Public Shared Property cFeature_Location_Contain_Start As New Features_Location_Comparers.Contain_Start

            Public Shared Property cFeature_Location_ByPAS As New TSS_PAS_Comparers.Feature_Location_ByPAS
            Public Shared Property cFeature_Location_ByTSS As New TSS_PAS_Comparers.Feature_Location_ByTSS
            Public Shared Property cBasic_Location_ByPAS As New TSS_PAS_Comparers.Basic_Location_ByPAS
            Public Shared Property cBasic_Location_ByTSS As New TSS_PAS_Comparers.Basic_Location_ByTSS
            Public Shared Property cBio__Location_ByPAS As New TSS_PAS_Comparers.Bio_Location_ByPAS
            Public Shared Property cBio__Location_ByTSS As New TSS_PAS_Comparers.Bio_Location_ByTSS

            Public Shared Property Gr_cBasic_Location_ByTSS As New TSS_PAS_Comparers.Gr_Basic_Location_ByTSS
            Public Shared Property Gr_cBasic_Location_ByPAS As New TSS_PAS_Comparers.Gr_Basic_Location_ByPAS
        End Class
        Public Class BioLocation_Comparers
#Region "Bio.Location"
            Public Class BioLocation
                Implements IEqualityComparer(Of Bio.IO.GenBank.ILocation)
                Implements IComparer(Of Bio.IO.GenBank.ILocation)

                Public Function Compare(x As ILocation, y As ILocation) As Integer Implements IComparer(Of ILocation).Compare
                    If x.Operator <> y.Operator Then
                        Return x.Operator.CompareTo(y.Operator)
                    ElseIf x.LocationStart <> y.LocationStart Then
                        Return x.LocationStart.CompareTo(y.LocationStart)
                    Else

                        If x.SubLocations.Count <> y.SubLocations.Count Then

                        End If
                        If x.LocationStart <> y.LocationStart Or y.LocationEnd <> x.LocationEnd Then
                            Return False
                        Else
                            If x.SubLocations.Count <> y.SubLocations.Count Then
                                Return False
                            ElseIf x.SubLocations.Count = 0 Then
                                Return True
                            Else
                                For i1 = 0 To x.SubLocations.Count - 1
                                    If Equals(x.SubLocations(i1), y.SubLocations(i1)) = False Then
                                        Return False
                                    End If
                                    Return True
                                Next
                            End If
                        End If
                    End If
                    Return True
                End Function

                Public Overloads Function Equals(x As ILocation, y As ILocation) As Boolean Implements IEqualityComparer(Of ILocation).Equals
                    If x.Operator <> y.Operator Then
                        Return False
                    Else
                        If x.LocationStart <> y.LocationStart Or y.LocationEnd <> x.LocationEnd Then
                            Return False
                        Else
                            If x.SubLocations.Count <> y.SubLocations.Count Then
                                Return False
                            ElseIf x.SubLocations.Count = 0 Then
                                Return True
                            Else
                                For i1 = 0 To x.SubLocations.Count - 1
                                    If Equals(x.SubLocations(i1), y.SubLocations(i1)) = False Then
                                        Return False
                                    End If
                                    Return True
                                Next
                            End If
                        End If
                    End If
                    Return True
                End Function

                Public Overloads Function GetHashCode(obj As ILocation) As Integer Implements IEqualityComparer(Of ILocation).GetHashCode

                    Dim hCode As Integer = obj.LocationStart Xor obj.LocationEnd
                    Return hCode.GetHashCode()
                End Function


            End Class

            ''' <summary>
            ''' Sort/Find By Seq ID,Operator and After By StartPosition From Location
            ''' FOr Compare First is the bigger, that can contains y which is the second 
            ''' </summary>
            Public Class Contain_Full
                Implements IComparer(Of Bio.IO.GenBank.ILocation)

                Public Function Compare(x As ILocation, y As ILocation) As Integer Implements IComparer(Of ILocation).Compare
                    If x.Operator <> y.Operator Then Return x.Operator.CompareTo(y.Operator)
                    If x.LocationStart <= y.LocationStart And x.LocationEnd >= y.LocationEnd Then
                        Return 0
                    End If
                    Return x.LocationStart.CompareTo(y.LocationStart)
                End Function

            End Class
            ''' <summary>
            ''' Sort/Find By Seq ID,Operator and After By StartPosition From Location
            ''' FOr Compare First is the bigger, that can contains y which is the second 
            ''' </summary>
            Public Class Contain_Start
                Implements IComparer(Of Bio.IO.GenBank.ILocation)

                Public Function Compare(x As ILocation, y As ILocation) As Integer Implements IComparer(Of ILocation).Compare
                    If x.Operator <> y.Operator Then Return x.Operator.CompareTo(y.Operator)
                    If x.LocationStart < y.LocationStart And x.LocationEnd > y.LocationStart Then
                        Return 0
                    End If
                    Return x.LocationStart.CompareTo(y.LocationStart)

                End Function

            End Class
            ''' <summary>
            ''' Sort/Find By Seq ID,Operator and After By StartPosition From Location
            ''' FOr Compare First is the bigger, that can contains y which is the second 
            ''' </summary>
            Public Class Contain_End
                Implements IComparer(Of Bio.IO.GenBank.ILocation)

                Public Function Compare(x As ILocation, y As ILocation) As Integer Implements IComparer(Of ILocation).Compare
                    If x.Operator <> y.Operator Then Return x.Operator.CompareTo(y.Operator)
                    If x.LocationStart > y.LocationEnd And x.LocationEnd > y.LocationEnd Then
                        Return 0
                    End If
                    Return x.LocationStart.CompareTo(y.LocationStart)

                End Function

            End Class
            ''' <summary>
            ''' Sort/Find By SeqID,OPerator and After By StartPosition From Location
            ''' </summary>
            Public Class _ByStart
                Implements IComparer(Of Bio.IO.GenBank.ILocation)

                Public Function Compare(x As ILocation, y As ILocation) As Integer Implements IComparer(Of ILocation).Compare
                    If x.Operator <> y.Operator Then Return x.Operator.CompareTo(y.Operator)
                    Return x.LocationStart.CompareTo(y.LocationStart)
                End Function


            End Class
            ''' <summary>
            ''' Sort/Find By Seq ID and After By StartPosition From Location
            ''' </summary>
            Public Class _ByEnd
                Implements IComparer(Of Bio.IO.GenBank.ILocation)

                Public Function Compare(x As ILocation, y As ILocation) As Integer Implements IComparer(Of ILocation).Compare
                    If x.Operator <> y.Operator Then Return x.Operator.CompareTo(y.Operator)
                    Return x.LocationEnd.CompareTo(y.LocationEnd)
                End Function

            End Class
#End Region


        End Class

        Public Class Features_Location_Comparers
#Region "Bio.Location"
            ''' <summary.location.>
            ''' Sort/Find By.location. Seq ID,Operator and After By.location. StartPosition From Location
            ''' FOr Compare First is the bigger, that can contains y.location. which is the second 
            ''' </summary.location.>
            Public Class Contain_Full
                Implements IComparer(Of Bio.IO.GenBank.FeatureItem)

                Public Function Compare(x As FeatureItem, y As FeatureItem) As Integer Implements IComparer(Of FeatureItem).Compare
                    If x.Location.Operator <> y.Location.Operator Then Return x.Location.Operator.CompareTo(y.Location.Operator)
                    If x.Location.LocationStart <= y.Location.LocationStart And x.Location.LocationEnd >= y.Location.LocationEnd Then
                        Return 0
                    End If
                    Return x.Location.LocationStart.CompareTo(y.Location.LocationStart)
                End Function

            End Class
            ''' <summary.location.>
            ''' Sort/Find By.location. Seq ID,Operator and After By.location. StartPosition From Location
            ''' FOr Compare First is the bigger, that can contains y.location. which is the second 
            ''' </summary.location.>
            Public Class Contain_Start
                Implements IComparer(Of Bio.IO.GenBank.FeatureItem)

                Public Function Compare(x As FeatureItem, y As FeatureItem) As Integer Implements IComparer(Of FeatureItem).Compare
                    If x.Location.Operator <> y.Location.Operator Then Return x.Location.Operator.CompareTo(y.Location.Operator)
                    If x.Location.LocationStart < y.Location.LocationStart And x.Location.LocationEnd > y.Location.LocationStart Then
                        Return 0
                    End If
                    Return x.Location.LocationStart.CompareTo(y.Location.LocationStart)

                End Function

            End Class
            ''' <summary.location.>
            ''' Sort/Find By.location. Seq ID,Operator and After By.location. StartPosition From Location
            ''' FOr Compare First is the bigger, that can contains y.location. which is the second 
            ''' </summary.location.>
            Public Class Contain_End
                Implements IComparer(Of Bio.IO.GenBank.FeatureItem)

                Public Function Compare(x As FeatureItem, y As FeatureItem) As Integer Implements IComparer(Of FeatureItem).Compare
                    If x.Location.Operator <> y.Location.Operator Then Return x.Location.Operator.CompareTo(y.Location.Operator)
                    If x.Location.LocationStart > y.Location.LocationEnd And x.Location.LocationEnd > y.Location.LocationEnd Then
                        Return 0
                    End If
                    Return x.Location.LocationStart.CompareTo(y.Location.LocationStart)

                End Function

            End Class
            ''' <summary.location.>
            ''' Sort/Find By.location. SeqID,OPerator and After By.location. StartPosition From Location
            ''' </summary.location.>
            Public Class _ByStart
                Implements IComparer(Of Bio.IO.GenBank.FeatureItem)

                Public Function Compare(x As FeatureItem, y As FeatureItem) As Integer Implements IComparer(Of FeatureItem).Compare
                    If x.Location.Operator <> y.Location.Operator Then Return x.Location.Operator.CompareTo(y.Location.Operator)
                    Return x.Location.LocationStart.CompareTo(y.Location.LocationStart)
                End Function


            End Class
            ''' <summary.location.>
            ''' Sort/Find By.location. Seq ID and After By.location. StartPosition From Location
            ''' </summary.location.>
            Public Class _ByEnd
                Implements IComparer(Of Bio.IO.GenBank.FeatureItem)

                Public Function Compare(x As FeatureItem, y As FeatureItem) As Integer Implements IComparer(Of FeatureItem).Compare
                    If x.Location.Operator <> y.Location.Operator Then Return x.Location.Operator.CompareTo(y.Location.Operator)
                    Return x.Location.LocationEnd.CompareTo(y.Location.LocationEnd)
                End Function

            End Class
#End Region


        End Class

        Public Class Basic_Location_Comparers
#Region "Bio.Location"
            ''' <summary.location.>
            ''' Sort/Find By.location. Seq ID,Operator and After By.location. StartPosition From Location
            ''' FOr Compare First is the bigger, that can contains y.location. which is the second 
            ''' </summary.location.>
            Public Class Contain_Full
                Implements IComparer(Of Szunyi.Location.Basic_Location)

                Public Function Compare(x As Basic_Location, y As Basic_Location) As Integer Implements IComparer(Of Szunyi.Location.Basic_Location).Compare
                    If x.Location.Operator <> y.Location.Operator Then Return x.Location.Operator.CompareTo(y.Location.Operator)
                    If x.Location.LocationStart <= y.Location.LocationStart And x.Location.LocationEnd >= y.Location.LocationEnd Then
                        Return 0
                    End If
                    Return x.Location.LocationStart.CompareTo(y.Location.LocationStart)
                End Function

            End Class
            Public Class Accession
                Implements IComparer(Of Szunyi.Location.Basic_Location)

                Public Function Compare(x As Basic_Location, y As Basic_Location) As Integer Implements IComparer(Of Szunyi.Location.Basic_Location).Compare
                    Return x.Location.Accession.CompareTo(y.Location.Accession)
                End Function

            End Class

            ''' <summary.location.>
            ''' Sort/Find By.location. Seq ID,Operator and After By.location. StartPosition From Location
            ''' FOr Compare First is the bigger, that can contains y.location. which is the second 
            ''' </summary.location.>
            Public Class Contain_Start
                Implements IComparer(Of Basic_Location)

                Public Function Compare(x As Basic_Location, y As Basic_Location) As Integer Implements IComparer(Of Basic_Location).Compare
                    If x.Location.Operator <> y.Location.Operator Then Return x.Location.Operator.CompareTo(y.Location.Operator)
                    If x.Location.LocationStart < y.Location.LocationStart And x.Location.LocationEnd > y.Location.LocationStart Then
                        Return 0
                    End If
                    Return x.Location.LocationStart.CompareTo(y.Location.LocationStart)

                End Function

            End Class
            ''' <summary.location.>
            ''' Sort/Find By.location. Seq ID,Operator and After By.location. StartPosition From Location
            ''' FOr Compare First is the bigger, that can contains y.location. which is the second 
            ''' </summary.location.>
            Public Class Contain_End
                Implements IComparer(Of Basic_Location)

                Public Function Compare(x As Basic_Location, y As Basic_Location) As Integer Implements IComparer(Of Basic_Location).Compare
                    If x.Location.IsComplementer <> y.Location.IsComplementer Then Return x.Location.IsComplementer.CompareTo(y.Location.IsComplementer)
                    If x.Location.LocationStart > y.Location.LocationEnd And x.Location.LocationEnd > y.Location.LocationEnd Then
                        Return 0
                    End If
                    Return x.Location.LocationStart.CompareTo(y.Location.LocationStart)

                End Function

            End Class
            ''' <summary.location.>
            ''' Sort/Find By.location. SeqID,OPerator and After By.location. StartPosition From Location
            ''' </summary.location.>
            Public Class _ByStart_wStrand
                Implements IComparer(Of Basic_Location)

                Public Function Compare(x As Basic_Location, y As Basic_Location) As Integer Implements IComparer(Of Basic_Location).Compare
                    If x.Location.IsComplementer <> y.Location.IsComplementer Then Return x.Location.IsComplementer.CompareTo(y.Location.IsComplementer)
                    Return x.Location.LocationStart.CompareTo(y.Location.LocationStart)
                End Function


            End Class
            ''' <summary.location.>
            ''' Sort/Find By.location. Seq ID and After By.location. StartPosition From Location
            ''' </summary.location.>
            Public Class _ByEnd_wStrand
                Implements IComparer(Of Basic_Location)

                Public Function Compare(x As Basic_Location, y As Basic_Location) As Integer Implements IComparer(Of Basic_Location).Compare
                    If x.Location.IsComplementer <> y.Location.IsComplementer Then Return x.Location.IsComplementer.CompareTo(y.Location.IsComplementer)
                    Return x.Location.LocationEnd.CompareTo(y.Location.LocationEnd)
                End Function

            End Class

            ''' <summary.location.>
            ''' Sort/Find By.location. Seq ID and After By.location.TSS From Location
            ''' </summary.location.>
            Public Class _ByTSS_wStrand
                Implements IComparer(Of Basic_Location)

                Public Function Compare(x As Basic_Location, y As Basic_Location) As Integer Implements IComparer(Of Basic_Location).Compare
                    If x.Location.IsComplementer <> y.Location.IsComplementer Then Return x.Location.IsComplementer.CompareTo(y.Location.IsComplementer)
                    Return x.Location.TSS.CompareTo(y.Location.TSS)
                End Function

            End Class
            ''' <summary.location.>
            ''' Sort/Find By.location. Seq ID ,strandand After By.location.PAS From Location
            ''' </summary.location.>
            Public Class _ByPAS_wStrand
                Implements IComparer(Of Basic_Location)

                Public Function Compare(x As Basic_Location, y As Basic_Location) As Integer Implements IComparer(Of Basic_Location).Compare
                    If x.Location.IsComplementer <> y.Location.IsComplementer Then Return x.Location.IsComplementer.CompareTo(y.Location.IsComplementer)
                    Return x.Location.PAS.CompareTo(y.Location.PAS)
                End Function

            End Class

            ''' <summary.location.>
            ''' Sort/Find By.location. SeqID,OPerator and After By.location. StartPosition From Location
            ''' </summary.location.>
            Public Class _ByStart_woStrand
                Implements IComparer(Of Basic_Location)

                Public Function Compare(x As Basic_Location, y As Basic_Location) As Integer Implements IComparer(Of Basic_Location).Compare
                    Return x.Location.LocationStart.CompareTo(y.Location.LocationStart)
                End Function


            End Class
            ''' <summary.location.>
            ''' Sort/Find By.location. Seq ID and After By.location. StartPosition From Location
            ''' </summary.location.>
            Public Class _ByEnd_woStrand
                Implements IComparer(Of Basic_Location)

                Public Function Compare(x As Basic_Location, y As Basic_Location) As Integer Implements IComparer(Of Basic_Location).Compare
                    Return x.Location.LocationEnd.CompareTo(y.Location.LocationEnd)
                End Function

            End Class

            ''' <summary.location.>
            ''' Sort/Find By.location. Seq ID and After By.location.TSS From Location
            ''' </summary.location.>
            Public Class _ByTSS_woStrand
                Implements IComparer(Of Basic_Location)

                Public Function Compare(x As Basic_Location, y As Basic_Location) As Integer Implements IComparer(Of Basic_Location).Compare
                    Return x.Location.TSS.CompareTo(y.Location.TSS)
                End Function

            End Class
            ''' <summary.location.>
            ''' Sort/Find By.location. Seq ID ,strandand After By.location.PAS From Location
            ''' </summary.location.>
            Public Class _ByPAS_woStrand
                Implements IComparer(Of Basic_Location)

                Public Function Compare(x As Basic_Location, y As Basic_Location) As Integer Implements IComparer(Of Basic_Location).Compare
                    Return x.Location.PAS.CompareTo(y.Location.PAS)
                End Function

            End Class
#End Region


        End Class

        Public Class TSS_PAS_Comparers
#Region "Bio.Location"

            Public Class Basic_Location_ByTSS
                Implements IComparer(Of Basic_Location)

                Public Function Compare(x As Basic_Location, y As Basic_Location) As Integer Implements IComparer(Of Basic_Location).Compare
                    If x.Location.IsComplementer <> y.Location.IsComplementer Then Return x.Location.IsComplementer.CompareTo(y.Location.IsComplementer)
                    Return x.Location.TSS.CompareTo(y.Location.TSS)
                End Function


            End Class
            Public Class Gr_Basic_Location_ByTSS
                Implements IComparer(Of List(Of Basic_Location))

                Public Function Compare(x As List(Of Basic_Location), y As List(Of Basic_Location)) As Integer Implements IComparer(Of List(Of Basic_Location)).Compare

                    If x.First.Location.IsComplementer <> y.First.Location.IsComplementer Then Return x.First.Location.IsComplementer.CompareTo(y.First.Location.IsComplementer)
                    Return x.First.Location.TSS.CompareTo(y.First.Location.TSS)
                End Function


            End Class
            Public Class Gr_Basic_Location_ByPAS
                Implements IComparer(Of List(Of Basic_Location))

                Public Function Compare(x As List(Of Basic_Location), y As List(Of Basic_Location)) As Integer Implements IComparer(Of List(Of Basic_Location)).Compare

                    If x.First.Location.IsComplementer <> y.First.Location.IsComplementer Then Return x.First.Location.IsComplementer.CompareTo(y.First.Location.IsComplementer)
                    Return x.First.Location.PAS.CompareTo(y.First.Location.PAS)
                End Function


            End Class
            Public Class Basic_Location_ByPAS
                Implements IComparer(Of Basic_Location)

                Public Function Compare(x As Basic_Location, y As Basic_Location) As Integer Implements IComparer(Of Basic_Location).Compare
                    If x.Location.IsComplementer <> y.Location.IsComplementer Then Return x.Location.IsComplementer.CompareTo(y.Location.IsComplementer)
                    Return x.Location.PAS.CompareTo(y.Location.PAS)
                End Function

            End Class

            Public Class Bio_Location_ByTSS
                Implements IComparer(Of ILocation)

                Public Function Compare(x As ILocation, y As ILocation) As Integer Implements IComparer(Of ILocation).Compare
                    If x.IsComplementer <> y.IsComplementer Then Return x.IsComplementer.CompareTo(y.IsComplementer)

                    Return x.TSS.CompareTo(y.TSS)
                End Function


            End Class

            Public Class Bio_Location_ByPAS
                Implements IComparer(Of ILocation)

                Public Function Compare(x As ILocation, y As ILocation) As Integer Implements IComparer(Of ILocation).Compare
                    If x.IsComplementer <> y.IsComplementer Then Return x.IsComplementer.CompareTo(y.IsComplementer)

                    Return x.PAS.CompareTo(y.PAS)
                End Function

            End Class

            Public Class Feature_Location_ByTSS
                Implements IComparer(Of FeatureItem)

                Public Function Compare(x As FeatureItem, y As FeatureItem) As Integer Implements IComparer(Of FeatureItem).Compare
                    If x.Location.IsComplementer <> y.Location.IsComplementer Then Return x.Location.IsComplementer.CompareTo(y.Location.IsComplementer)

                    Return x.Location.TSS.CompareTo(y.Location.TSS)
                End Function


            End Class

            Public Class Feature_Location_ByPAS
                Implements IComparer(Of FeatureItem)

                Public Function Compare(x As FeatureItem, y As FeatureItem) As Integer Implements IComparer(Of FeatureItem).Compare
                    If x.Location.IsComplementer <> y.Location.IsComplementer Then Return x.Location.IsComplementer.CompareTo(y.Location.IsComplementer)

                    Return x.Location.PAS.CompareTo(y.Location.PAS)
                End Function

            End Class
#End Region


        End Class
    End Namespace
End Namespace
