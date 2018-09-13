Imports System.IO
Imports Bio.IO.GenBank
Imports ClassLibrary1.Szunyi.Constants
Imports ClassLibrary1.Szunyi.Features
Imports ClassLibrary1.Szunyi.Features.FeatureManipulation

Namespace Szunyi
    Namespace PacBio
        Public Class Common
            Public Shared Function Get_ID(File As FileInfo) As String
                Dim t = Szunyi.Text.Regexp.Find_Motifs(File.Name, "[m]{1}\d{6}[_]{1}\d{6}[_]{1}\d{5}[_]{1}[c]{1}\d{33}_s1_[pX]{1}[0]{1}")
                If IsNothing(t) = True Then Return String.Empty
                If t.Count > 0 Then
                    Return t.Item(0).Value

                Else
                    Return String.Empty
                End If
            End Function
            Public Shared Function Get_ID(File As String) As String
                Dim t = Szunyi.Text.Regexp.Find_Motifs(File, "[m]{1}\d{6}[_]{1}\d{6}[_]{1}\d{5}[_]{1}[c]{1}\d{33}_s1_[pX]{1}[0]{1}")
                If IsNothing(t) = True Then Return String.Empty
                If t.Count > 0 Then
                    Return t.Item(0).Value

                Else
                    Return String.Empty
                End If
            End Function
            Public Shared Function Get_IDs(Files As List(Of FileInfo)) As List(Of String)
                Dim out As New List(Of String)
                If IsNothing(Files) = True Then Return out
                For Each File In Files
                    Dim ID = Get_ID(File)
                    If ID <> "" Then
                        out.Add(ID)
                    End If
                Next
                Return out

            End Function
            Public Shared Function Add_Nof_Source_to_Labels(ByRef Seqs As List(Of Bio.ISequence), IPW As Szunyi.Text.TableManipulation.Items_With_Properties) As String
                Dim str As New System.Text.StringBuilder
                Dim IndexOfLibary = IPW.Get_Header_Index("library")
                For Each Seq In Seqs
                    str.Append(Seq.ID)
                    Dim md = Szunyi.Features.FeatureManipulation.GenBankMetaDataManipulation.GetGenBankMetaDataFromSeq(Seq)
                    Dim sg = From x In md.Features.All Where x.Qualifiers.ContainsKey(StandardQualifierNames.IdentifiedBy) = True

                    For Each Intron In sg

                        If IsNothing(IPW) = False Then
                            Dim CommonNames As New List(Of String)
                            For Each Note In Intron.Qualifiers(StandardQualifierNames.IdentifiedBy)
                                If IsNothing(Note) = False Then
                                    Dim sName = Note.Split("/").First
                                    Dim Index = IPW.Get_Index(sName.Replace(" ", ""), TextMatch.Contains)
                                    If Index > -1 Then
                                        Dim cName = IPW.Items(Index).Properties(IndexOfLibary)
                                        CommonNames.Add(cName)
                                    End If
                                    Dim alf As Int16 = 54
                                End If
                            Next
                            Dim d = CommonNames.Distinct.ToList
                            Intron.Label = Intron.Label & " de:" & d.Count

                        End If

                    Next

                Next
                Return str.ToString
            End Function
            Public Shared Function Add_Nof_Source_to_Labels(ByRef Feats As List(Of FeatureItem), IPW As Szunyi.Text.TableManipulation.Items_With_Properties) As List(Of FeatureItem)
                Dim str As New System.Text.StringBuilder
                Dim IndexOfLibary = IPW.Get_Header_Index("library")
                For Each Feat In Feats

                    If IsNothing(IPW) = False Then
                        Dim CommonNames As New List(Of String)
                        For Each Note In Feat.Qualifiers(StandardQualifierNames.IdentifiedBy)
                            If IsNothing(Note) = False Then
                                Dim sName = Note.Split("/").First
                                Dim Index = IPW.Get_Index(sName.Replace(" ", ""), TextMatch.Contains)
                                If Index > -1 Then
                                    Dim cName = IPW.Items(Index).Properties(IndexOfLibary)
                                    CommonNames.Add(cName)
                                End If
                                Dim alf As Int16 = 54
                            End If
                        Next
                        Dim d = CommonNames.Distinct.ToList
                        Feat.Label = Feat.Label & " de:" & d.Count

                    End If

                Next
                Return Feats

            End Function
            Public Shared Function PAS_and_PolyA_Regions(seqs As List(Of Bio.ISequence), width As Integer) As String
                Dim str As New System.Text.StringBuilder
                str.Append("SeqID").Append(vbTab)
                str.Append("Location").Append(vbTab)
                str.Append("Nof A/T in " & width).Append(vbTab)
                For Each Seq In seqs
                    Dim PAS = Szunyi.Features.FeatureManipulation.GetFeaturesByType.GetFeturesByTypeFromSeq(Seq, "PAS_own")
                    For Each PA In PAS
                        If PA.Location.Operator = LocationOperator.Complement Then
                            Dim tmpSeq = Szunyi.Sequences.SequenceManipulation.GetSequences.SeqFromStartAndLength(Seq, PA.Location.LocationStart - width, width)
                            Dim charColl As System.Text.RegularExpressions.MatchCollection = System.Text.RegularExpressions.Regex.Matches(tmpSeq.ConvertToString, "T")
                            str.Append(Seq.ID).Append(vbTab)
                            str.Append(Szunyi.Features.FeatureManipulation.GetLocations.LociBuilder.GetLocationString(PA.Location)).Append(vbTab)
                            str.Append(charColl.Count).Append(vbTab)

                        Else

                            str.Append(Seq.ID).Append(vbTab)
                            Dim tmpSeqII = Szunyi.Sequences.SequenceManipulation.GetSequences.SeqFromStartAndLength(Seq, PA.Location.LocationStart, width)
                            Dim charCollII As System.Text.RegularExpressions.MatchCollection = System.Text.RegularExpressions.Regex.Matches(tmpSeqII.ConvertToString, "A")
                            str.Append(Szunyi.Features.FeatureManipulation.GetLocations.LociBuilder.GetLocationString(PA.Location)).Append(vbTab)
                            str.Append(charCollII.Count)
                        End If
                        str.AppendLine()
                    Next
                Next
                Return str.ToString
            End Function
            Public Shared Function PacBio_Transcripts(ByRef Seq As Bio.ISequence) As List(Of FeatureItem)
                Dim CDSs = Szunyi.Features.FeatureManipulation.GetFeaturesByType.GetFeturesByTypeFromSeq(Seq, StandardFeatureKeys.CodingSequence)
                Dim CDSsWoJoin = Szunyi.Features.FeatureManipulation.GetLocations.Get_Full_Locations(CDSs) 'No join, maintain orientation
                Dim Operons = GetFeaturesByType.GetFeturesByTypeFromSeqStartWith(Seq, "tr_own")
                Dim out As New List(Of FeatureItem)

                If IsNothing(Operons) = False And IsNothing(CDSs) = False Then
                    Dim gMapPos As Integer = 0
                    For Each TrII In Operons
                        If TrII.Location.Operator = LocationOperator.Join Then
                            Dim a As Int16 = 54
                        ElseIf TrII.Location.Operator = LocationOperator.Complement Then
                            Dim b As Int16 = 54
                        End If
                        Dim tmpLoc = GetLocations.Get_Full_Location(TrII)
                        Dim tr As New FeatureItem("a", tmpLoc.Location)
                        If tr.Location.Operator = LocationOperator.Join Then tr.Location.Operator = tr.Location.SubLocations.First.Operator

                        Dim x As New OL_Manipulation(tr, CDSsWoJoin, StandardQualifierNames.GeneSymbol)
                        x.DoIt()


                        TrII.Label = TrII.Label & ";" & x.SymbolicName
                        If TrII.Qualifiers.ContainsKey(StandardQualifierNames.GenomicMapPosition) = False Then
                            TrII.Qualifiers.Add(StandardQualifierNames.GenomicMapPosition, New List(Of String))
                        End If
                        If x.SymbolicName = "" Then
                            TrII.Qualifiers(StandardQualifierNames.GenomicMapPosition).Add(gMapPos)
                            gMapPos += 1
                        Else
                            TrII.Qualifiers(StandardQualifierNames.GenomicMapPosition).Add(x.LocationName)
                        End If


                    Next

                    For Each s1 In GetSame(Operons)
                        Dim RealName = s1.First.Label.Split(";").Last
                        Dim s2 = s1.First.Label.Split(";")
                        Dim jh = Szunyi.Text.Lists.GetSubList(s2.ToList, 1)
                        RealName = Szunyi.Text.General.GetText(jh, ",")
                        Dim IdentifedbBy As New List(Of String)
                        Dim Notes As New List(Of String)
                        Dim c As Integer = 0
                        For Each item In s1
                            Notes.AddRange(item.Qualifiers(StandardQualifierNames.Note))
                            IdentifedbBy.AddRange(item.Qualifiers(StandardQualifierNames.IdentifiedBy))
                        Next

                        Dim Locis = (From x1 In s1 Select x1.Location).ToList
                        Dim ThelOCI = Szunyi.Location.Common.Create_Biggest(Locis)
                        Dim x As New Bio.IO.GenBank.MaturePeptide(ThelOCI)
                        x.Label = "e:" & IdentifedbBy.Count & " " & RealName
                        x.Qualifiers(StandardQualifierNames.Note) = Notes
                        x.Qualifiers(StandardQualifierNames.IdentifiedBy) = IdentifedbBy

                        out.Add(x)
                        '  Szunyi.Features.FeatureManipulation.GenBankMetaDataManipulation.AddFeature(Seq, x)


                    Next
                    For Each OP In Operons
                        OP.Qualifiers(StandardQualifierNames.GenomicMapPosition).Clear()
                    Next
                End If
                Return out

            End Function

            Public Shared Function PacBio_Transcripts(ByRef Seq As Bio.ISequence, FeatType As String) As String

                Dim mRNAs = GetFeaturesByType.GetFeturesByTypeFromSeq(Seq, StandardFeatureKeys.MessengerRna)
                Dim Operons = GetFeaturesByType.GetFeturesByTypeFromSeq(Seq, FeatType)
                Dim CDSs = Szunyi.Features.FeatureManipulation.GetFeaturesByType.GetFeturesByTypeFromSeq(Seq, StandardFeatureKeys.CodingSequence)
                If IsNothing(Operons) = False And IsNothing(CDSs) = False Then
                    Dim gMapPos As Integer = 0
                    For Each Tr In Operons
                        Dim OLs = Szunyi.Location.OverLapping_Locations.Get_All_Overlaps_Features(Tr, CDSs)
                        Dim F = Szunyi.Location.OverLapping_Locations.Get_5_Prime_Overhangs_wOrientation(Tr, CDSs)
                        Dim I = Szunyi.Location.OverLapping_Locations.Get_Inner_Feature_Items_wOrientation(Tr, CDSs)
                        Dim T = Szunyi.Location.OverLapping_Locations.Get_3_Prime_Overhangs_wOrientation(Tr, CDSs)
                        Dim Fs_Gene = Szunyi.Features.FeatureManipulation.Qulifiers.Get_Values_From_Features(F, StandardQualifierNames.GeneSymbol)
                        Fs_Gene = Szunyi.Text.Lists.Insert_Text_in_Every_LineStarts(Fs_Gene, "P5 ")
                        Dim Fs = Szunyi.Features.FeatureManipulation.GetLocations.Get_Location_Strings(F)
                        Fs = Szunyi.Text.Lists.Insert_Text_in_Every_LineStarts(Fs, "P5 ")

                        Dim Fs_Gene_LocusTag = Szunyi.Features.FeatureManipulation.Qulifiers.Get_Values_From_Features(F, StandardQualifierNames.LocusTag)
                        Fs_Gene_LocusTag = Szunyi.Text.Lists.Insert_Text_in_Every_LineStarts(Fs_Gene_LocusTag, "P5 ")

                        Dim Ins = Szunyi.Features.FeatureManipulation.GetLocations.Get_Location_Strings(I)
                        Dim Ins_Gene = Szunyi.Features.FeatureManipulation.Qulifiers.Get_Values_From_Features(I, StandardQualifierNames.GeneSymbol)

                        Dim Ins_Gene_LocusTag = Szunyi.Features.FeatureManipulation.Qulifiers.Get_Values_From_Features(I, StandardQualifierNames.LocusTag)

                        Dim Ts = Szunyi.Features.FeatureManipulation.GetLocations.Get_Location_Strings(T)
                        Dim Ts_Gene = Szunyi.Features.FeatureManipulation.Qulifiers.Get_Values_From_Features(T, StandardQualifierNames.GeneSymbol)
                        Dim Ts_Gene_LocusTag = Szunyi.Features.FeatureManipulation.Qulifiers.Get_Values_From_Features(T, StandardQualifierNames.LocusTag)

                        Ts = Szunyi.Text.Lists.Insert_Text_in_Every_LineStarts(Ts, "P3 ")
                        Ts_Gene = Szunyi.Text.Lists.Insert_Text_in_Every_LineStarts(Ts_Gene, "P3 ")
                        Dim Ts_LocusTag = Szunyi.Text.Lists.Insert_Text_in_Every_LineStarts(Ts_Gene_LocusTag, "P3 ")

                        Dim Names As New List(Of String)
                        Names.AddRange(Fs)
                        Names.AddRange(Ins)
                        Names.AddRange(Ts)
                        Names = Names.Distinct.ToList
                        Names.Sort()
                        Dim RealNames As New List(Of String)
                        If Ts_Gene_LocusTag.Count > 0 Or Ins_Gene_LocusTag.Count > 0 Or Fs_Gene_LocusTag.Count > 0 Then
                            If Fs_Gene_LocusTag.Count > 0 Or Ts_Gene_LocusTag.Count > 0 Then
                                Dim ald As Int16 = 54
                            End If
                            RealNames.AddRange(Fs_Gene_LocusTag)
                            RealNames.AddRange(Ins_Gene_LocusTag)
                            RealNames.AddRange(Ts_LocusTag)
                        Else

                            RealNames.AddRange(Fs_Gene)
                            RealNames.AddRange(Ins_Gene)
                            RealNames.AddRange(Ts_Gene)
                        End If

                        RealNames = RealNames.Distinct.ToList
                        RealNames.Sort()

                        Dim TheName = Szunyi.Text.General.GetText(RealNames, ";").Replace(Chr(34), "")
                        Tr.Label = Tr.Label & "/" & TheName
                        If Tr.Qualifiers.ContainsKey(StandardQualifierNames.GenomicMapPosition) = False Then
                            Tr.Qualifiers.Add(StandardQualifierNames.GenomicMapPosition, New List(Of String))
                        End If
                        If RealNames.Count <> 0 Then
                            Tr.Qualifiers(StandardQualifierNames.GenomicMapPosition).Add(Szunyi.Text.General.GetText(Names, ";").Replace(Chr(34), ""))
                        Else
                            Tr.Qualifiers(StandardQualifierNames.GenomicMapPosition).Add(gMapPos)
                            gMapPos += 1
                        End If


                    Next
                    Dim STR As New System.Text.StringBuilder
                    For Each s1 In GetSame(Operons)
                        Dim RealName = s1.First.Label.Split("/").Last

                        Dim IdentifedbBy As New List(Of String)
                        Dim Notes As New List(Of String)
                        Dim c As Integer = 0
                        For Each item In s1
                            Notes.AddRange(item.Qualifiers(StandardQualifierNames.Note))
                            IdentifedbBy.AddRange(item.Qualifiers(StandardQualifierNames.IdentifiedBy))

                        Next

                        Dim Locis = (From x1 In s1 Select x1.Location).ToList
                        Dim ThelOCI = Szunyi.Location.Common.Create_Biggest(Locis)
                        Dim x As New Bio.IO.GenBank.MaturePeptide(ThelOCI)
                        x.Label = "/e:" & c & " " & RealName
                        x.Qualifiers(StandardQualifierNames.Note) = Notes
                        x.Qualifiers(StandardQualifierNames.IdentifiedBy) = IdentifedbBy
                        Szunyi.Features.FeatureManipulation.GenBankMetaDataManipulation.AddFeature(Seq, x)
                        Dim CA = RealName.Split(";")
                        If CA.Count > 1 Then
                            STR.Append(RealName).Append(vbTab)
                            STR.Append(Szunyi.Features.FeatureManipulation.GetLocations.LociBuilder.GetLocationString(ThelOCI)).Append(vbTab)
                            STR.Append("/e:" & IdentifedbBy.Count).AppendLine()
                        End If

                    Next
                    For Each OP In Operons
                        OP.Qualifiers(StandardQualifierNames.GenomicMapPosition).Clear()
                    Next
                    Return STR.ToString
                End If
                Return String.Empty

            End Function

            Private Shared Iterator Function GetSame(mRNAs As List(Of FeatureItem)) As IEnumerable(Of List(Of FeatureItem))
                If IsNothing(mRNAs) = True Then
                    Exit Function
                End If
                If mRNAs.Count = 0 Then
                    Exit Function
                End If
                Dim s = From j In mRNAs Group By j.Qualifiers(StandardQualifierNames.GenomicMapPosition).First Into Group

                For Each item In s
                    Dim res = item.Group.ToList
                    Yield res
                Next
            End Function

            Private Shared Function GetX(SeqID As String, Type As String, trs As List(Of FeatureItem), gC As List(Of FeatureItem), OptimalWidth As Integer) As String
                Dim str As New System.Text.StringBuilder
                For Each tr In trs
                    str.Append(SeqID).Append(vbTab)
                    If tr.Location.Operator = LocationOperator.Complement Then
                        Dim Near = Szunyi.Location.Get_Locis_Near._End(tr, OptimalWidth + 100, gC)
                        Dim Best = From x In Near Where x.Location.Operator = tr.Location.Operator And x.Location.LocationEnd > tr.Location.LocationEnd Order By System.Math.Abs(x.Location.LocationEnd - OptimalWidth - tr.Location.LocationEnd) Ascending

                        If Best.Count > 0 Then
                            str.Append(Type).Append(vbTab).Append(Szunyi.Location.Common.GetLocationString(tr.Location)).Append(vbTab).Append(Szunyi.Location.Common.GetLocationString(Best.First.Location))
                            str.Append(vbTab).Append(System.Math.Abs(Best.First.Location.LocationEnd - tr.Location.LocationEnd))
                            str.AppendLine()
                        Else
                            str.Append(Type).Append(vbTab).Append(Szunyi.Location.Common.GetLocationString(tr.Location)).AppendLine()
                        End If

                    Else
                        '    Dim Near = Szunyi.Location.Get_Locis_Near._Start(tr, OptimalWidth + 100, gC)
                        '   Dim Best = From x In Near Where x.Location.Operator = Bio.IO.GenBank.LocationOperator.None And x.Location.LocationStart < tr.Location.LocationStart Order By System.Math.Abs(-x.Location.LocationStart + OptimalWidth - tr.Location.LocationStart) Ascending

                        'If Best.Count > 0 Then
                        'str.Append(Type).Append(vbTab).Append(Szunyi.Features.Locis.LociBuilder.GetLocationString(tr.Location)).Append(vbTab).Append(Szunyi.Features.Locis.LociBuilder.GetLocationString(Best.First.Location))
                        'str.Append(vbTab).Append(System.Math.Abs(Best.First.Location.LocationStart - tr.Location.LocationStart))
                        'str.AppendLine()
                        ' Else
                        'str.Append(Type).Append(vbTab).Append(Szunyi.Features.Locis.LociBuilder.GetLocationString(tr.Location)).AppendLine()
                        ' End If

                    End If

                Next
                Return str.ToString
            End Function

        End Class

        Public Class OL_Manipulation
            Private cDSs As List(Of FeatureItem)
            Private tr As FeatureItem
            Private revtr As FeatureItem
            Private Qualifier As String
            Public OLs As New List(Of OL)
            Public Property LocationName As String
            Public Property SymbolicName As String
            Public Sub New(tr As FeatureItem, cDSs As List(Of FeatureItem), Qulifier As String)
                Me.tr = tr
                Dim x As New FeatureItem("tr_own", Szunyi.Features.FeatureManipulation.GetLocations.Change_Strand(tr.Location))
                revtr = x
                Me.cDSs = cDSs
                Me.Qualifier = Qualifier
            End Sub
            Private Function Get_All_LocationStrings(OLs As List(Of OL)) As List(Of String)
                Dim test = (From x In OLs Select x.Location_Strings)
                Dim out As New List(Of String)
                For Each Item In test
                    For Each LocS In Item
                        out.Add(LocS.Split(" ").Last)
                    Next
                Next
                Return out
            End Function
            Private Function Get_All_LocationStringsFull(OLs As List(Of OL)) As List(Of String)
                Dim test = (From x In OLs Select x.Location_Strings)
                Dim out As New List(Of String)
                For Each Item In test
                    For Each LocS In Item
                        out.Add(LocS)
                    Next
                Next
                Return out
            End Function
            Public Function GetAllSortedLocations(OLs As List(Of OL)) As List(Of ILocation)
                Dim AllLocis As New List(Of ILocation)
                Dim Locis = From x In OLs Select x.Locations
                Dim SortedLocations As List(Of ILocation)
                For Each Loci In Locis
                    AllLocis.AddRange(Loci)
                Next

                If Me.tr.Location.Operator = LocationOperator.Complement Then
                    SortedLocations = (From x In AllLocis Order By x.LocationEnd Descending).ToList
                Else
                    SortedLocations = (From x In AllLocis Order By x.LocationStart Ascending).ToList
                End If
                Return SortedLocations
            End Function
            Public Sub DoIt()
                If tr.Location.Operator = LocationOperator.Complement Then
                    Dim alfh As Int16 = 65
                End If
                Dim OLs = Get_All_OLs(tr)
                Dim revOLS = Get_All_OLs(revtr)
                Dim SortedLocisII As List(Of ILocation)
                If OLs.Count > 0 Then
                    Dim Full = Get_All_LocationStringsFull(OLs)
                    Dim AllLocationString = Get_All_LocationStrings(OLs)
                    Dim dAllLocationString = AllLocationString.Distinct.ToList
                    If Full.Count <> dAllLocationString.Count Then
                        Dim alk As Int16 = 54
                    End If
                    SortedLocisII = GetAllSortedLocations(OLs)
                    For Each Loci In SortedLocisII
                        For Each ol In OLs
                            For i1 = 0 To ol.Location_Strings.Count - 1
                                If Loci Is ol.Locations(i1) Then
                                    Me.SymbolicName = (Me.SymbolicName & " " & ol.Symbols(i1)).TrimStart(" ").TrimEnd(" ")
                                    Me.LocationName = (Me.LocationName & " " & ol.Location_Strings(i1)).TrimStart(" ").TrimEnd(" ")
                                End If
                            Next
                        Next
                    Next
                Else
                    If revOLS.Count > 0 Then
                        Dim Full = Get_All_LocationStringsFull(revOLS)
                        Dim AllLocationString = Get_All_LocationStrings(revOLS)
                        Dim dAllLocationString = AllLocationString.Distinct.ToList
                        If Full.Count <> dAllLocationString.Count Then
                            Dim alk As Int16 = 54
                        End If
                        SortedLocisII = GetAllSortedLocations(revOLS)
                        For Each Loci In SortedLocisII
                            For Each ol In revOLS
                                For i1 = 0 To ol.Location_Strings.Count - 1
                                    If Loci Is ol.Locations(i1) Then
                                        Me.SymbolicName = (Me.SymbolicName & " AS" & " " & ol.Symbols(i1)).TrimStart(" ").TrimEnd(" ")
                                        Me.LocationName = (Me.LocationName & " AS" & " " & ol.Location_Strings(i1)).TrimStart(" ").TrimEnd(" ")
                                    End If
                                Next
                            Next
                        Next
                    Else
                        Me.LocationName = ""
                        Me.SymbolicName = ""
                    End If

                End If

                Dim alf As Int16 = 54
            End Sub
            Private Function Get_All_OLs(tr As FeatureItem) As List(Of OL)
                Dim OLs As New List(Of OL)
                Dim Five = Szunyi.Location.OverLapping_Locations.Get_5_Prime_Overhangs_wOrientation(tr, cDSs)
                If Five.Count > 0 Then
                    Dim OL_Five = Get_OLs(Five, "P5 ")
                    OLs.Add(OL_Five)
                End If

                Dim Full = Szunyi.Location.OverLapping_Locations.Get_Inner_Feature_Items_wOrientation(tr, cDSs)
                If Full.Count > 0 Then
                    Dim OL_Full = Get_OLs(Full, "F ")
                    OLs.Add(OL_Full)
                End If

                Dim Three = Szunyi.Location.OverLapping_Locations.Get_3_Prime_Overhangs_wOrientation(tr, cDSs)
                If Three.Count > 0 Then
                    Dim OL_Three = Get_OLs(Three, "P3 ")
                    OLs.Add(OL_Three)
                End If

                Dim Inner = Szunyi.Location.OverLapping_Locations.Get_Biggers_Overhangs_wOrientation(tr, cDSs)
                If Inner.Count > 0 Then
                    Dim OL_Inner = Get_OLs(Inner, "I ")

                    OLs.Add(OL_Inner)
                End If
                Dim sg = From x In OLs Where x.Symbols.Contains("I ") = True

                Dim sgII = From x In OLs Where x.Symbols.Contains("P ") = True

                If sg.Count > 0 AndAlso sgII.Count > 0 Then
                    Dim ald As Int16 = 54
                End If
                Return OLs
            End Function
            Private Function Get_OLs(f As List(Of FeatureItem), Ext As String) As OL
                Dim GP As New List(Of String)
                GP.Add(StandardQualifierNames.GeneSymbol)
                GP.Add(StandardQualifierNames.Product)
                GP.Add(StandardQualifierNames.LocusTag)
                Dim Symbols = Szunyi.Features.FeatureManipulation.Qulifiers.Get_Values_From_Features(f, GP)
                Symbols = Szunyi.Text.Lists.Insert_Text_in_Every_LineStarts(Symbols, Ext)
                Dim Location_Strings = Szunyi.Features.FeatureManipulation.GetLocations.Get_Location_Strings(f)
                Location_Strings = Szunyi.Text.Lists.Insert_Text_in_Every_LineStarts(Location_Strings, Ext)
                If Symbols.Count <> Location_Strings.Count Then
                    Dim alf As Int16 = 54
                End If
                If Location_Strings.Count > Symbols.Count Then
                    Symbols = Szunyi.Features.FeatureManipulation.Qulifiers.Get_Values_From_Features(f, StandardQualifierNames.LocusTag)
                    Symbols = Szunyi.Text.Lists.Insert_Text_in_Every_LineStarts(Symbols, Ext)
                End If
                Dim x As New OL(Symbols, Location_Strings)
                Return x
            End Function
        End Class

        Public Class OL
            Public Symbols As New List(Of String)
            Public Location_Strings As New List(Of String)
            Public Locations As New List(Of ILocation)

            Public Sub New(Symbols As List(Of String), Location_Strings As List(Of String))
                Dim used As New List(Of String)
                For i1 = 0 To Location_Strings.Count - 1
                    If used.Contains(Location_Strings(i1)) = False Then
                        used.Add(Location_Strings(i1))
                        Me.Location_Strings.Add(Location_Strings(i1))
                        If Symbols.Count > i1 Then
                            Me.Symbols.Add(Symbols(i1).Replace(Chr(34), " "))
                        End If
                        Me.Locations.Add(Szunyi.Location.Common.Get_Location(Location_Strings(i1).Split(" ").Last))
                    End If
                Next
            End Sub
        End Class
    End Namespace
End Namespace

