Imports System.IO
Imports System.Text
Imports System.Text.RegularExpressions
Imports Bio
Imports Bio.IO.GenBank
Imports ClassLibrary1
Imports ClassLibrary1.Szunyi
Imports ClassLibrary1.Szunyi.Basic
Imports ClassLibrary1.Szunyi.Basic.Count
Imports ClassLibrary1.Szunyi.Comparares
Imports ClassLibrary1.Szunyi.Comparares.OneByOne
Imports ClassLibrary1.Szunyi.Features.FeatureManipulation
Imports ClassLibrary1.Szunyi.ListOf
Imports ClassLibrary1.Szunyi.Location
Imports ClassLibrary1.Szunyi.Text.TableManipulation

Namespace Szunyi
    Namespace Features
        Namespace Check
            Public Class CDS
                Public Shared Function EndwStop(seq As Bio.ISequence, Optional WithCDSName As Boolean = False) As List(Of String)
                    Dim CDSs = Szunyi.Features.FeatureManipulation.GetFeaturesByType.GetFeturesByTypeFromSeq(seq, StandardFeatureKeys.CodingSequence)
                    Dim out As New List(Of String)
                    For Each CDS In CDSs
                        Dim tr = Szunyi.DNA.Translate.TranaslateToString(CDS, seq)
                        If WithCDSName = True Then
                            If tr.EndsWith("*") Then
                                out.Add(Szunyi.Features.FeatureManipulation.Qulifiers.Get_Common_Values(CDS, vbTab) & "True")
                            Else
                                out.Add(Szunyi.Features.FeatureManipulation.Qulifiers.Get_Common_Values(CDS, vbTab) & "False")

                            End If
                        Else
                            If tr.EndsWith("*") Then
                                out.Add("True")
                            Else
                                out.Add("False")

                            End If
                        End If

                    Next
                    Return out
                End Function

                Public Shared Function StartwATG(Seq As Bio.ISequence, Optional WithCDSName As Boolean = False) As List(Of String)
                    Dim CDSs = Szunyi.Features.FeatureManipulation.GetFeaturesByType.GetFeturesByTypeFromSeq(Seq, StandardFeatureKeys.CodingSequence)
                    Dim out As New List(Of String)
                    For Each CDS In CDSs
                        Dim tr = Szunyi.DNA.Translate.TranaslateToString(CDS, Seq)
                        If WithCDSName = True Then
                            If tr.StartsWith("M") Then
                                out.Add(Szunyi.Features.FeatureManipulation.Qulifiers.Get_Common_Values(CDS, vbTab) & "True")
                            Else
                                out.Add(Szunyi.Features.FeatureManipulation.Qulifiers.Get_Common_Values(CDS, vbTab) & "False")

                            End If
                        Else
                            If tr.StartsWith("M") Then
                                out.Add("True")
                            Else
                                out.Add("False")

                            End If
                        End If

                    Next
                    Return out
                End Function
                Public Shared Function Lengths(Seq As Bio.ISequence, Optional WithCDSName As Boolean = False) As List(Of String)
                    Dim CDSs = Szunyi.Features.FeatureManipulation.GetFeaturesByType.GetFeturesByTypeFromSeq(Seq, StandardFeatureKeys.CodingSequence)
                    Dim out As New List(Of String)

                    For Each CDS In CDSs
                        Dim Length = CDS.Location.LocationEnd - CDS.Location.LocationStart
                        If WithCDSName = True Then
                            out.Add(Szunyi.Features.FeatureManipulation.Qulifiers.Get_Common_Values(CDS, vbTab) & vbTab & Length)
                        Else
                            out.Add(Length)
                        End If

                    Next
                    Return out
                End Function
                Public Shared Function Mod3(Seq As Bio.ISequence, Optional WithCDSName As Boolean = False) As List(Of String)
                    Dim CDSs = Szunyi.Features.FeatureManipulation.GetFeaturesByType.GetFeturesByTypeFromSeq(Seq, StandardFeatureKeys.CodingSequence)
                    Dim out As New List(Of String)
                    For Each CDS In CDSs
                        Dim Length = CDS.Location.LocationEnd - CDS.Location.LocationStart
                        If WithCDSName = True Then
                            out.Add(Szunyi.Features.FeatureManipulation.Qulifiers.Get_Common_Values(CDS, vbTab) & vbTab & Length Mod 3)
                        Else
                            out.Add(Length Mod 3)
                        End If

                    Next
                    Return out
                End Function
                Public Shared Function Inner_Stop(Seq As Bio.ISequence, Optional WithCDSName As Boolean = False) As List(Of String)
                    Dim CDSs = Szunyi.Features.FeatureManipulation.GetFeaturesByType.GetFeturesByTypeFromSeq(Seq, StandardFeatureKeys.CodingSequence)
                    Dim out As New List(Of String)

                    For Each CDS In CDSs
                        Dim tr = Szunyi.DNA.Translate.TranaslateToString(CDS, Seq)
                        If tr.Last = "*" Then
                            tr = tr.Substring(0, tr.Length - 1)
                        End If
                        Dim s = Split(tr, "*")
                        If WithCDSName = True Then
                            out.Add(Szunyi.Features.FeatureManipulation.Qulifiers.Get_Common_Values(CDS, vbTab) & vbTab & s.Length)

                        Else
                            out.Add(s.Length)

                        End If

                    Next
                    Return out
                End Function
            End Class
        End Namespace
        Namespace Motifs
            Public Class DNA_Motifs
                Public NamesWithPositions As New Dictionary(Of String, Integer)
                Public Sub New()
                    NamesWithPositions.Add("GC-box", -110)
                    NamesWithPositions.Add("TATA-Box", -31)
                    NamesWithPositions.Add("XCPE1", -8)
                    NamesWithPositions.Add("MTE", 8)
                    NamesWithPositions.Add("CCAAT-box", -70)
                    NamesWithPositions.Add("BREux", -37)

                    NamesWithPositions.Add("INR", -2)

                    NamesWithPositions.Add("DPE", -28)
                    NamesWithPositions.Add("BREd", -25)
                    NamesWithPositions.Add("BREu", -35)
                    NamesWithPositions.Add("MED-1", -1)
                    NamesWithPositions.Add("DCE_S_I", -1)
                    NamesWithPositions.Add("DCE_S_II", -1)
                    NamesWithPositions.Add("DCE_S_III", -1)

                End Sub

            End Class
        End Namespace
        Namespace FeatureManipulation
            Public Class Key
                Public Shared Function Get_All_Different_Keys(Seqs As List(Of Bio.ISequence)) As List(Of String)
                    Dim out As New List(Of String)
                    For Each Seq In Seqs
                        out.AddRange(Get_All_Different_Keys(Seq))
                    Next
                    If out.Count > 0 Then
                        Return out.Distinct.ToList
                    Else
                        Return out
                    End If
                End Function
                Public Shared Function Get_All_Different_Keys(Seq As Bio.ISequence) As List(Of String)
                    Dim md = Szunyi.Features.FeatureManipulation.GenBankMetaDataManipulation.GetGenBankMetaDataFromSeq(Seq)
                    If IsNothing(md) = True Then Return New List(Of String)
                    Dim res = From x In md.Features.All Select x.Key

                    If res.Count > 0 Then
                        Return res.Distinct.ToList
                    Else
                        Return New List(Of String)
                    End If
                End Function

                Public Shared Function ReName_Keys(TRs As List(Of FeatureItem), Key As String) As List(Of FeatureItem)
                    Dim nTRs = Szunyi.Features.FeatureManipulation.Common.Clones(TRs)
                    For Each Item In nTRs
                        Item.Key = Key
                    Next
                    Return nTRs
                End Function
            End Class
            Public Class Annotate
                Public Shared Function Find_PolyA_Sites(Seq As Bio.ISequence, length As Integer) As Bio.ISequence
                    Dim SSeq = Szunyi.Sequences.SequenceManipulation.SeqsToString.GetSeqAsString(Seq)
                    Dim polyA_Motifs = Split("AAAAAG,AAGAAA,AATAAA,AATACA,AATAGA,AATATA,ACTAAA,AGTAAA,ATTAAA,CATAAA,GATAAA,TATAAA", ",")
                    '    polyA_Motifs = Split("AATGAA,AATCAA,AATTAA,AACAAA,AAAAAA,AATAAT,AATAAC,AATAAG", ",")
                    '       PolyA_Motifs_rev = Split("TTCATT,TTGCTT,TTAATT,TTTGTT,TTTTTT,ATTATT,GTTATT,CTTATT", ",")
                End Function
                Public Shared Function Get_PolyA_Signals(Seq As Bio.ISequence, length As Integer, Optional mRNAs As List(Of FeatureItem) = Nothing) As List(Of FeatureItem)
                    Dim polyA_Motifs = Split("AAAAAG,AAGAAA,AATAAA,AATACA,AATAGA,AATATA,ACTAAA,AGTAAA,ATTAAA,CATAAA,GATAAA,TATAAA,AATGAA,AATCAA,AATTAA,AACAAA,AAAAAA,AATAAT,AATAAC,AATAAG", ",")
                    If IsNothing(mRNAs) = True Then
                        mRNAs = Szunyi.Features.FeatureManipulation.GetFeaturesByType.GetFeturesByTypeFromSeq(Seq, StandardFeatureKeys.MessengerRna)
                    End If

                    Dim out As New List(Of FeatureItem)
                    Dim UsedLocations As New List(Of String)
                    For Each theRNA In mRNAs
                        If theRNA.Qualifiers.ContainsKey(StandardQualifierNames.Note) AndAlso theRNA.Qualifiers(StandardQualifierNames.Note).First.Contains("rl2?") Then
                            Dim kj As Int16 = 54
                        End If
                        Dim theRNASeq = theRNA.GetSubSequence(Seq)
                        Dim LAstPart As Bio.ISequence
                        Dim ssEq As String
                        If theRNA.Location.IsComplementer = True Then
                            theRNASeq = theRNASeq.GetComplementedSequence
                            LAstPart = theRNASeq.GetSubSequence(0, length)
                            ssEq = Szunyi.Sequences.SequenceManipulation.SeqsToString.GetSeqAsString(LAstPart.GetReverseComplementedSequence)
                        Else
                            LAstPart = theRNASeq.GetSubSequence(theRNASeq.Count - length, length)
                            ssEq = Szunyi.Sequences.SequenceManipulation.SeqsToString.GetSeqAsString(LAstPart)
                        End If


                        For Each sMotif In polyA_Motifs
                            Dim startpos As Integer = 1
                            Do
                                Dim i = InStr(startpos, ssEq, sMotif)
                                If i > 0 Then
                                    startpos = i + 1
                                    Dim f As FeatureItem
                                    If theRNA.Location.IsComplementer = True Then
                                        Dim st As Integer = theRNA.Location.LocationStart + length - i + 1
                                        f = New FeatureItem(StandardFeatureKeys.PolyASignal, "complement(" & st - sMotif.Count & ".." & st - 1 & ")")
                                        Dim l = Szunyi.Location.Common.GetLocationString(f)
                                        If UsedLocations.Contains(l) = False Then
                                            UsedLocations.Add(l)
                                            out.Add(f)
                                        End If
                                        Dim tmp = f.GetSubSequence(Seq)
                                        ' out.Add(f)
                                    Else
                                        Dim st As Integer = theRNA.Location.LocationEnd - length + i
                                        f = New FeatureItem(StandardFeatureKeys.PolyASignal, st & ".." & st + sMotif.Count - 1)
                                        Dim tmp = f.GetSubSequence(Seq)
                                        Dim l = Szunyi.Location.Common.GetLocationString(f)
                                        If UsedLocations.Contains(l) = False Then
                                            UsedLocations.Add(l)
                                            out.Add(f)
                                        End If
                                    End If
                                Else
                                    Exit Do
                                End If
                            Loop

                        Next
                        Dim alf As Int16 = 54
                    Next
                    Return out
                End Function
                Public Shared Function Pacbio_Diff_Exp_Count(Seq As Bio.ISequence, IPW As Items_With_Properties) As Bio.ISequence
                    Dim md = Szunyi.Features.FeatureManipulation.GenBankMetaDataManipulation.GetGenBankMetaDataFromSeq(Seq)
                    Dim Feats = md.Features.All
                    Pacbio_Diff_Exp_Count_ForFeatures(Feats, IPW)
                    Return Seq
                End Function
                Public Shared Sub Pacbio_Diff_Exp_Count_ForFeatures(Feats As List(Of FeatureItem), IPW As Items_With_Properties)
                    If IsNothing(IPW) = True Then Exit Sub
                    For Each Feat In Feats

                        Dim CommonNames As New List(Of String)
                        If Feat.Qualifiers.ContainsKey(StandardQualifierNames.IdentifiedBy) = True Then
                            For i1 = 0 To Feat.Qualifiers(StandardQualifierNames.IdentifiedBy).Count - 1
                                Dim s = Feat.Qualifiers(StandardQualifierNames.IdentifiedBy)(i1)
                                If IsNothing(s) = False Then
                                    Dim sName = s.Split("/").First
                                    Dim Index = IPW.Get_Index(sName.Replace(" ", ""), Constants.TextMatch.Contains)
                                    If Index > -1 Then
                                        Dim cName = IPW.Items(Index).Properties(1)
                                        CommonNames.Add(cName)
                                    End If
                                End If

                            Next


                            Dim d = CommonNames.Distinct.ToList
                            Feat.Label = Feat.Label & " de e:" & d.Count
                        End If


                    Next
                End Sub
                Public Shared Function Annotate_Dora_POX(Seqs As List(Of Bio.ISequence), file As FileInfo) As List(Of Bio.ISequence)
                    Dim cSeqs = Szunyi.Sequences.SequenceManipulation.Common.CloneSeqs(Seqs)
                    Dim Header = Szunyi.IO.Import.Text.GetHeader(file, 1)
                    For Each s In Szunyi.IO.Import.Text.ParseToArray(file, vbTab, 1)
                        If s.Count >= 8 Then
                            For i1 = 2 To 7
                                If s(i1) <> "" Then
                                    Dim f As New FeatureItem(Header(i1), Szunyi.Location.Common.Get_Location(s(8)))
                                    f.Label = s(i1)
                                    Szunyi.Features.FeatureManipulation.GenBankMetaDataManipulation.AddFeature(cSeqs.First, f)
                                End If
                            Next
                        End If
                    Next
                    Return cSeqs
                End Function
                Public Shared Function Annotate_Strain_Key_Start_end_Strand_Name(Seqs As List(Of Bio.ISequence), file As FileInfo) As List(Of Bio.ISequence)
                    Dim cSeqs = Szunyi.Sequences.SequenceManipulation.Common.CloneSeqs(Seqs)

                    For Each s In Szunyi.IO.Import.Text.ParseToArray(file, vbTab, 1)
                        Dim cSEQ = Szunyi.Sequences.SequenceManipulation.GetSequences.ByID(cSeqs, s(0))
                        If IsNothing(cSEQ) = True Then cSEQ = cSeqs.First
                        Dim loci As Bio.IO.GenBank.ILocation
                        If s(4) = "+" Then
                            loci = Szunyi.Location.Common.Get_Location(s(2) & ".." & s(3))
                        Else
                            loci = Szunyi.Location.Common.Get_Location("complement(" & s(3) & ".." & s(2) & ")")
                        End If

                        Dim feat As New Bio.IO.GenBank.FeatureItem(s(1), loci)
                        Dim ls As New List(Of String)
                        ls.Add(s(5))
                        feat.Qualifiers.Add(StandardQualifierNames.GeneSymbol, ls)
                        Szunyi.Features.FeatureManipulation.GenBankMetaDataManipulation.AddFeature(cSEQ, feat)
                    Next

                    Return cSeqs
                End Function

                ' name, key, tss,pas, loci
                Private Shared Function GetLoci(s() As String) As Bio.IO.GenBank.ILocation
                    Dim exS As String = ""
                    Dim exE As String = ""
                    If s(4).Contains("?") Then
                        exS = "GetXmlNamespace"
                        s(4) = s(4).Replace("?", "")
                    End If
                    If s(6).Contains("?") Then
                        exE = "GetXmlNamespace"
                        s(6) = s(6).Replace("?", "")
                    End If

                    Dim st As Integer = s(4).Split("/").First
                    Dim en As Integer = s(6).Split("/").First
                    Dim loci As Bio.IO.GenBank.ILocation
                    If st > en Then
                        If exE <> String.Empty Then exE = "<"
                        If exS <> String.Empty Then exS = ">"
                        loci = Szunyi.Location.Common.Get_Location("complement(" & exE & en & ".." & st & exS & ")")
                    Else
                        If exE <> String.Empty Then exE = ">"
                        If exS <> String.Empty Then exS = "<"
                        loci = Szunyi.Location.Common.Get_Location(exS & st & ".." & en & exE)
                    End If
                    Return loci
                End Function
                Public Shared Function Annotate_Ala_Dora_Final(Seqs As List(Of Bio.ISequence), Files As List(Of FileInfo)) As List(Of Bio.ISequence)
                    Dim cSeqs = Szunyi.Sequences.SequenceManipulation.Common.CloneSeqs(Seqs)
                    For Each Seq In cSeqs
                        For Each File In Files
                            Dim UsedLocis As New List(Of String)
                            Dim Lines = Szunyi.IO.Import.Text.Parse(File, 1)
                            For Each Line In Lines
                                Dim s = Split(Line, vbTab)
                                If s(4) <> String.Empty And s(6) <> String.Empty Then
                                    Dim loci = GetLoci(s)
                                    Dim ls = Szunyi.Location.Common.GetLocationString(loci)
                                    If UsedLocis.Contains(ls) = False Then
                                        UsedLocis.Add(ls)
                                        If s(2) = String.Empty Then s(2) = "mRNA"
                                        Dim f1 As New FeatureItem(s(2), loci)
                                        f1.Label = s(1)
                                        Szunyi.Features.FeatureManipulation.GenBankMetaDataManipulation.AddFeature(Seq, f1)
                                    Else
                                        Dim alf As Int16 = 54
                                    End If
                                End If

                            Next


                        Next
                    Next
                    Return cSeqs
                End Function

                Public Shared Function Annotate_Ala_Dora(Seqs As List(Of Bio.ISequence), Files As List(Of FileInfo)) As List(Of Bio.ISequence)
                    Dim cSeqs = Szunyi.Sequences.SequenceManipulation.Common.CloneSeqs(Seqs)
                    For Each Seq In cSeqs
                        For Each File In Files
                            Dim Lines = Szunyi.IO.Import.Text.Parse(File, 1)
                            For Each Line In Lines
                                Dim s = Split(Line, vbTab)
                                If s.First <> String.Empty Then
                                    If s(1).Contains("sp") = True Then
                                        Dim l = Get_Location(s(2), s(3))
                                        Dim Locis As New List(Of Bio.IO.GenBank.ILocation)
                                        For i1 = 4 To s.Count - 1
                                            If s(i1) <> String.Empty Then Locis.Add(Get_fw_loci(s(i1)))
                                        Next
                                        Dim res = (From x In Locis Order By x.LocationStart Ascending).ToList

                                        Dim l2 = Szunyi.Location.Common.Get_Location(l.LocationStart & ".." & l.LocationEnd)
                                        l2.Operator = LocationOperator.Join
                                        l2.SubLocations.AddRange(res)
                                        If l.SubLocations.Count = 0 Then
                                            l.Operator = LocationOperator.Join
                                            l.SubLocations.AddRange(res)
                                        Else
                                            l.SubLocations.First.Operator = LocationOperator.Join
                                            l.SubLocations.First.SubLocations.AddRange(res)
                                        End If
                                        Dim f As New FeatureItem("n_" & s.First, l)
                                        Dim lk = Szunyi.Location.Common.GetLocationString(l)
                                        f.Label = s(1)
                                        Dim f1 As New FeatureItem("new_tr", l)
                                        f1.Label = s(1)
                                        Szunyi.Features.FeatureManipulation.GenBankMetaDataManipulation.AddFeature(Seq, f)
                                        Szunyi.Features.FeatureManipulation.GenBankMetaDataManipulation.AddFeature(Seq, f1)
                                    Else
                                        Dim f As New FeatureItem("n_" & s.First, Get_Location(s(2), s(3)))
                                        f.Label = s(1)
                                        Dim f1 As New FeatureItem("new_tr", Get_Location(s(2), s(3)))
                                        f1.Label = s(1)
                                        Szunyi.Features.FeatureManipulation.GenBankMetaDataManipulation.AddFeature(Seq, f)
                                        Szunyi.Features.FeatureManipulation.GenBankMetaDataManipulation.AddFeature(Seq, f1)
                                    End If
                                End If
                            Next
                        Next
                    Next
                    Return cSeqs
                End Function
                Private Shared Function Get_fw_loci(s As String) As Bio.IO.GenBank.ILocation
                    If s = String.Empty Then Return Nothing
                    Dim s1 = Split(s, "-")
                    Dim st As Integer = s1.First
                    Dim e As Integer = s1.Last
                    If st > e Then
                        Dim l = Szunyi.Location.Common.Get_Location(e & ".." & st)
                        Return l
                    Else
                        Dim l = Szunyi.Location.Common.Get_Location(st & ".." & e)
                        Return l
                    End If
                End Function
                Private Shared Function Get_Location(TSS As String, PAS As String) As Bio.IO.GenBank.ILocation
                    Dim s As Integer = TSS
                    Dim e As Integer = PAS
                    Dim loci As Bio.IO.GenBank.ILocation
                    If s < e Then

                        loci = Szunyi.Location.Common.Get_Location(s & ".." & e)
                    Else
                        loci = Szunyi.Location.Common.Get_Location("complement(" & e & ".." & s & ")")
                    End If
                    Return loci

                End Function
                Private Class TIS_PAS_TSS_Annotation
                    Public Property Original_CDSs As List(Of FeatureItem)
                    Public Property CDS_Name As String
                    Public Property Start As Integer
                    Public Property Endy As Integer
                    Public Property Features As New List(Of FeatureItem)
                    Public Property log As New System.Text.StringBuilder
                    Public Property Seq As Bio.ISequence
                    Public Sub New(Lines As List(Of String), CDSs As List(Of FeatureItem), Seq As Bio.ISequence)
                        Me.Seq = Seq
                        Me.Original_CDSs = CDSs
                        Dim s() = Split(Lines.First, vbTab)
                        Me.CDS_Name = s(3)
                        Me.Start = s(0)
                        Me.Endy = s(1)
                        Dim newCDSs As New List(Of FeatureItem)
                        For Each Line In Lines
                            If Line.Split(vbTab)(5) = "TIS" Then
                                Dim nCDSs = Get_TIS(Line)
                                Me.Features.AddRange(nCDSs)
                                newCDSs.AddRange(nCDSs)
                            End If
                        Next
                        CDSs.AddRange(newCDSs)
                        For Each Line In Lines
                            If Line.Split(vbTab)(5) <> "TIS" Then
                                Dim feats = Get_5Prime_3Primes(Line, CDSs)
                                If IsNothing(feats) = False Then
                                    Me.Features.AddRange(feats)
                                End If

                            End If

                        Next
                        Me.Features = Szunyi.Features.FeatureManipulation.MergeFeatures.MergeByTypeAnsPositions(Me.Features)

                    End Sub

                    Private Function Get_TIS(line As String) As List(Of FeatureItem)
                        Dim out As New List(Of FeatureItem)
                        Dim s = Split(line, vbTab)
                        Dim TheCDS = From x In Original_CDSs Where x.Qualifiers(StandardQualifierNames.LocusTag).First.Replace(Chr(34), "") = CDS_Name

                        Select Case TheCDS.Count
                            Case 0
                                Dim nulla As Int16 = 43
                            Case 1
                                Dim All_Pos = Szunyi.Text.Lists.Get_Integers(s(7), ";").Distinct.ToList
                                Dim Abundants = Szunyi.Text.Lists.Get_Integers(s(8), ";").Distinct.ToList
                                If Abundants.Count = 0 Then Abundants = All_Pos
                                For Each pos In All_Pos
                                    Dim Loci = Szunyi.DNA.ORF_Finding.Get_Location_From_Start_Position(Me.Seq, pos, TheCDS.First.Location.Operator, DNA.StartCodons.ATG)
                                    If IsNothing(Loci) = True Then

                                        Dim Found As Boolean = False
                                    Else
                                        Dim Found As Boolean = True
                                        Dim x As New FeatureItem(StandardFeatureKeys.CodingSequence, Loci)
                                        Szunyi.Features.FeatureManipulation.MergeFeatures.Merge2Features(TheCDS.First, x, True)
                                        out.Add(x)
                                    End If
                                Next


                            Case Else
                                Dim sok As Int16 = 43
                                Dim All_Pos = Szunyi.Text.Lists.Get_Integers(s(7), ";").Distinct.ToList
                                Dim Abundants = Szunyi.Text.Lists.Get_Integers(s(8), ";").Distinct.ToList
                                If Abundants.Count = 0 Then Abundants = All_Pos
                                For Each pos In All_Pos
                                    Dim Loci = Szunyi.DNA.ORF_Finding.Get_Location_From_Start_Position(Me.Seq, pos, TheCDS.First.Location.Operator, DNA.StartCodons.ATG)
                                    If IsNothing(Loci) = True Then

                                        Dim Found As Boolean = False
                                    Else
                                        Dim Found As Boolean = True
                                        Dim x As New FeatureItem(StandardFeatureKeys.CodingSequence, Loci)
                                        Szunyi.Features.FeatureManipulation.MergeFeatures.Merge2Features(TheCDS.First, x, True)
                                        out.Add(x)
                                    End If
                                Next
                        End Select
                        Return out
                    End Function

                    Public Function Get_5Prime_3Primes(Line As String, CDSs As List(Of FeatureItem)) As List(Of FeatureItem)
                        Dim out As New List(Of FeatureItem)
                        Dim s = Line.Split(vbTab)
                        Dim PMID = s(4)
                        Dim ExpType = s(6)
                        Dim All_Pos = Szunyi.Text.Lists.Get_Integers(s(7), ";").Distinct.ToList
                        Dim Abundants = Szunyi.Text.Lists.Get_Integers(s(8), ";").Distinct.ToList
                        If Abundants.Count = 0 Then Abundants = All_Pos
                        All_Pos = All_Pos.Except(Abundants).ToList
                        Dim cCDS = From x In CDSs Where x.Location.LocationStart = Me.Start And x.Location.LocationEnd = Me.Endy
                        Dim TheCDS As FeatureItem
                        If cCDS.Count <> 1 Then
                            log.Append(Me.CDS_Name)
                            Return Nothing
                        Else
                            TheCDS = cCDS.First
                        End If
                        Select Case s(5)
                            Case "TSS" '5 prime UTR
                                out.AddRange(Get_5PrimeUTRs(All_Pos.ToList, ExpType, TheCDS))
                                out.AddRange(Get_5PrimeUTRs(Abundants.ToList, "MA " & ExpType, TheCDS))
                                out.AddRange(Get_TSSs(All_Pos.ToList, ExpType, TheCDS))
                                out.AddRange(Get_TSSs(Abundants.ToList, "MA " & ExpType, TheCDS))
                            Case "PAS" ' 3 prime UTR
                                out.AddRange(Get_3PrimeUTRs(All_Pos.ToList, ExpType, TheCDS))
                                out.AddRange(Get_3PrimeUTRs(Abundants.ToList, "MA " & ExpType, TheCDS))
                                out.AddRange(Get_PAS(All_Pos.ToList, ExpType, TheCDS))
                                out.AddRange(Get_PAS(Abundants.ToList, "MA " & ExpType, TheCDS))

                            Case String.Empty

                            Case Else

                        End Select
                        Return out

                    End Function
                    Private Function Get_PAS(Positions As List(Of Integer), AdditionalLabel As String, theCDS As FeatureItem) As List(Of FeatureItem)
                        Dim out As New List(Of FeatureItem)
                        If theCDS.Location.Operator = LocationOperator.Complement Then
                            For Each Pos In Positions
                                If Pos > Me.Endy Then
                                    Dim x As New Bio.IO.GenBank.PolyASignal("complement(" & Pos & ".." & Pos & ")")
                                    x.Label = "PAS " & Me.CDS_Name
                                    Dim notes As New List(Of String)
                                    notes.Add(AdditionalLabel)
                                    x.Qualifiers.Add(StandardQualifierNames.Note, notes)
                                    out.Add(x)
                                Else
                                    log.Append(Me.CDS_Name & vbTab & Szunyi.Features.FeatureManipulation.GetLocations.LociBuilder.GetLocationString(theCDS.Location) & vbTab & AdditionalLabel & vbTab & Pos & vbTab & "3 UTR").AppendLine()

                                End If
                            Next
                        Else
                            For Each Pos In Positions
                                If Pos < Me.Start Then
                                    Dim x As New Bio.IO.GenBank.PolyASignal(Pos & ".." & Pos)
                                    x.Label = "PAS " & Me.CDS_Name
                                    Dim notes As New List(Of String)
                                    notes.Add(AdditionalLabel)
                                    x.Qualifiers.Add(StandardQualifierNames.Note, notes)
                                    out.Add(x)
                                Else
                                    log.Append(Me.CDS_Name & vbTab & Szunyi.Features.FeatureManipulation.GetLocations.LociBuilder.GetLocationString(theCDS.Location) & vbTab & AdditionalLabel & vbTab & Pos & vbTab & "3 UTR").AppendLine()

                                End If
                            Next
                        End If
                        Return out
                    End Function
                    Private Function Get_3PrimeUTRs(Positions As List(Of Integer), AdditionalLabel As String, theCDS As FeatureItem) As List(Of FeatureItem)
                        Dim out As New List(Of FeatureItem)
                        If theCDS.Location.Operator = LocationOperator.Complement Then
                            For Each Pos In Positions
                                If Pos > Me.Endy Then
                                    Dim x As New Bio.IO.GenBank.ThreePrimeUtr("complement(" & Me.Endy & ".." & Pos & ")")
                                    x.Label = StandardFeatureKeys.ThreePrimeUtr & " " & Me.CDS_Name
                                    Dim notes As New List(Of String)
                                    notes.Add(AdditionalLabel)
                                    x.Qualifiers.Add(StandardQualifierNames.Note, notes)
                                    out.Add(x)
                                Else
                                    log.Append(Me.CDS_Name & vbTab & Szunyi.Features.FeatureManipulation.GetLocations.LociBuilder.GetLocationString(theCDS.Location) & vbTab & AdditionalLabel & vbTab & Pos & vbTab & "3 UTR").AppendLine()

                                End If
                            Next
                        Else
                            For Each Pos In Positions
                                If Pos < Me.Start Then
                                    Dim x As New Bio.IO.GenBank.ThreePrimeUtr(Pos & ".." & Me.Start)
                                    x.Label = StandardFeatureKeys.ThreePrimeUtr & " " & Me.CDS_Name
                                    Dim notes As New List(Of String)
                                    notes.Add(AdditionalLabel)
                                    x.Qualifiers.Add(StandardQualifierNames.Note, notes)
                                    out.Add(x)
                                Else
                                    log.Append(Me.CDS_Name & vbTab & Szunyi.Features.FeatureManipulation.GetLocations.LociBuilder.GetLocationString(theCDS.Location) & vbTab & AdditionalLabel & vbTab & Pos & vbTab & "3 UTR").AppendLine()

                                End If
                            Next
                        End If
                        Return out
                    End Function

                    Private Function Get_5PrimeUTRs(Positions As List(Of Integer), AdditionalLabel As String, theCDS As FeatureItem) As List(Of FeatureItem)
                        Dim out As New List(Of FeatureItem)
                        If theCDS.Location.Operator = LocationOperator.Complement Then
                            For Each Pos In Positions
                                If Pos >= Me.Endy Then
                                    Dim x As New Bio.IO.GenBank.FivePrimeUtr("complement(" & Me.Endy & ".." & Pos & ")")
                                    x.Label = StandardFeatureKeys.FivePrimeUtr & " " & Me.CDS_Name
                                    Dim notes As New List(Of String)
                                    notes.Add(AdditionalLabel)
                                    x.Qualifiers.Add(StandardQualifierNames.Note, notes)
                                    out.Add(x)
                                Else
                                    log.Append(Me.CDS_Name & vbTab & Szunyi.Features.FeatureManipulation.GetLocations.LociBuilder.GetLocationString(theCDS.Location) & vbTab & AdditionalLabel & vbTab & Pos & vbTab & "5 UTR").AppendLine()
                                End If
                            Next
                        Else
                            For Each Pos In Positions
                                If Pos < Me.Start Then
                                    Dim x As New Bio.IO.GenBank.FivePrimeUtr(Pos & ".." & Me.Start)
                                    x.Label = StandardFeatureKeys.FivePrimeUtr & " " & Me.CDS_Name
                                    Dim notes As New List(Of String)
                                    notes.Add(AdditionalLabel)
                                    x.Qualifiers.Add(StandardQualifierNames.Note, notes)
                                    out.Add(x)
                                Else
                                    log.Append(Me.CDS_Name & vbTab & Szunyi.Features.FeatureManipulation.GetLocations.LociBuilder.GetLocationString(theCDS.Location) & vbTab & AdditionalLabel & vbTab & Pos & vbTab & "5 UTR").AppendLine()

                                End If
                            Next
                        End If
                        Return out
                    End Function
                    Private Function Get_TSSs(Positions As List(Of Integer), AdditionalLabel As String, theCDS As FeatureItem) As List(Of FeatureItem)
                        Dim out As New List(Of FeatureItem)
                        If theCDS.Location.Operator = LocationOperator.Complement Then
                            For Each Pos In Positions
                                If Pos >= Me.Endy Then
                                    Dim x As New Bio.IO.GenBank.FeatureItem("TSS", "complement(" & Pos & ".." & Pos & ")")

                                    x.Label = "TSS " & Me.CDS_Name
                                    Dim notes As New List(Of String)
                                    notes.Add(AdditionalLabel)
                                    x.Qualifiers.Add(StandardQualifierNames.Note, notes)
                                    out.Add(x)
                                Else
                                    log.Append(Me.CDS_Name & vbTab & Szunyi.Features.FeatureManipulation.GetLocations.LociBuilder.GetLocationString(theCDS.Location) & vbTab & AdditionalLabel & vbTab & Pos & vbTab & "5 UTR").AppendLine()
                                End If
                            Next
                        Else
                            For Each Pos In Positions
                                If Pos < Me.Start Then
                                    Dim x As New Bio.IO.GenBank.FeatureItem("TSS", Pos & ".." & Pos)


                                    x.Label = "TSS " & Me.CDS_Name
                                    Dim notes As New List(Of String)
                                    notes.Add(AdditionalLabel)
                                    x.Qualifiers.Add(StandardQualifierNames.Note, notes)
                                    out.Add(x)
                                Else
                                    log.Append(Me.CDS_Name & vbTab & Szunyi.Features.FeatureManipulation.GetLocations.LociBuilder.GetLocationString(theCDS.Location) & vbTab & AdditionalLabel & vbTab & Pos & vbTab & "5 UTR").AppendLine()

                                End If
                            Next
                        End If
                        Return out
                    End Function
                End Class
                Public Shared Sub Poisson(ByRef Seq As ISequence, Poisson_Group As List(Of List(Of Szunyi.Stat.Distribution_Result)), FeatKey As String)
                    For Each Item In Poisson_Group

                        Dim l = Szunyi.Location.Common.GetLocation(Item.First.Index, Item.First.IsComplementer)
                        Dim Feat As New FeatureItem(FeatKey, l)
                        Szunyi.Features.FeatureManipulation.Qulifiers.Add(Feat, StandardQualifierNames.Label, Item.Count)
                        Dim cFIle_Names = (From x In Item Select x.File.Name).ToList
                        Szunyi.Features.FeatureManipulation.Qulifiers.Add(Feat, StandardQualifierNames.IdentifiedBy, cFIle_Names)
                        Szunyi.Features.FeatureManipulation.GenBankMetaDataManipulation.AddFeature(Seq, Feat)

                    Next
                End Sub
                Public Shared Sub Poisson(ByRef Seq As ISequence, Poisson_Group As List(Of List(Of Poisson)), FeatKey As String)
                    For Each Item In Poisson_Group

                        Dim l = Szunyi.Location.Common.GetLocation(Item.First.Index, Item.First.IsComplementer)
                        Dim Feat As New FeatureItem(FeatKey, l)
                        Szunyi.Features.FeatureManipulation.Qulifiers.Add(Feat, StandardQualifierNames.Label, Item.Count)
                        Dim cFIle_Names = (From x In Item Select x.File.Name).ToList
                        Szunyi.Features.FeatureManipulation.Qulifiers.Add(Feat, StandardQualifierNames.IdentifiedBy, cFIle_Names)
                        Szunyi.Features.FeatureManipulation.GenBankMetaDataManipulation.AddFeature(Seq, Feat)

                    Next
                End Sub
            End Class

            Public Class GetFeaturesByType
                Public Shared Function Get_All_Features(seq As Bio.ISequence) As List(Of FeatureItem)
                    Dim out As New List(Of FeatureItem)

                    Dim md = Szunyi.Features.FeatureManipulation.GenBankMetaDataManipulation.GetGenBankMetaDataFromSeq(seq)
                        If IsNothing(md) = True Then Return out
                        out.AddRange((From x In md.Features.All).ToList)


                    Return out

                End Function
                Public Shared Function Get_All_Features(seqs As List(Of Bio.ISequence)) As List(Of FeatureItem)
                    Dim out As New List(Of FeatureItem)
                    For Each Seq In seqs

                        Dim md = Szunyi.Features.FeatureManipulation.GenBankMetaDataManipulation.GetGenBankMetaDataFromSeq(Seq)
                        If IsNothing(md) = True Then Return out
                        out.AddRange((From x In md.Features.All).ToList)
                    Next

                    Return out

                End Function
                Public Shared Function GetFeturesByTypeFromSeqStartWith(seq As Bio.ISequence, key As String) As List(Of FeatureItem)
                    Dim out As New List(Of FeatureItem)
                    Dim md = Szunyi.Features.FeatureManipulation.GenBankMetaDataManipulation.GetGenBankMetaDataFromSeq(seq)
                    If IsNothing(md) = True Then Return out
                    out = (From x In md.Features.All Where x.Key.StartsWith(key) = True).ToList

                    Return out
                End Function
                Public Shared Function GetFeturesByTypeFromSeqContains(seq As Bio.ISequence, key As String) As List(Of FeatureItem)
                    Dim out As New List(Of FeatureItem)
                    Dim md = Szunyi.Features.FeatureManipulation.GenBankMetaDataManipulation.GetGenBankMetaDataFromSeq(seq)
                    If IsNothing(md) = True Then Return out
                    out = (From x In md.Features.All Where x.Key.Contains(key) = True).ToList

                    Return out
                End Function
                Public Shared Function GetFeturesByTypesFromSeqs(Seqs As List(Of Bio.ISequence),
                                               SelectedFeatureTypes As List(Of String)) As List(Of FeatureItem)

                    Dim MDs = GenBankMetaDataManipulation.GetGenBankMetaDatasFromSeqs(Seqs)
                    Return GetFeturesByTypesFromMetaDatas(MDs, SelectedFeatureTypes)

                End Function
                Public Shared Function GetFeturesByTypesFromSeq(Seq As Bio.ISequence,
                                               SelectedFeatureTypes As List(Of String)) As List(Of FeatureItem)

                    Dim MD = GenBankMetaDataManipulation.GetGenBankMetaDataFromSeq(Seq)
                    Return GetFeturesByTypesFromMetadata(MD, SelectedFeatureTypes)

                End Function
                ''' <summary>
                ''' Return not Sorted List
                ''' </summary>
                ''' <param name="Seqs"></param>
                ''' <param name="SelectedFeatureType"></param>
                ''' <returns></returns>
                Public Shared Function GetFeturesByTypeFromSeqs(Seqs As List(Of Bio.ISequence),
                                               SelectedFeatureType As String) As List(Of FeatureItem)

                    Dim MDs = GenBankMetaDataManipulation.GetGenBankMetaDatasFromSeqs(Seqs)
                    Dim out As New List(Of FeatureItem)
                    For Each Md In MDs
                        Dim t = GetFeatureByTypeFromMetadata(SelectedFeatureType, Md)
                        If IsNothing(t) = False Then out.AddRange(t)
                    Next
                    Return out

                End Function
                Public Shared Function GetFeturesSeqsByTypeFromSeqs(Seqs As List(Of Bio.ISequence),
                                               SelectedFeatureType As String,
                                               ByRef log As StringBuilder) As List(Of Bio.ISequence)

                    Dim out As New List(Of Bio.ISequence)
                    For Each Seq In Seqs
                        out.AddRange(GetFeturesSeqByTypeFromSeqs(Seq, SelectedFeatureType, log))
                    Next
                    If log.Length > 0 Then
                        Dim alf As Int16 = 54
                    End If
                    Return out

                End Function
                Public Shared Function GetFeturesSeqByTypeFromSeqs(Seq As Bio.ISequence,
                                               SelectedFeatureType As String, ByRef log As StringBuilder) As List(Of Bio.ISequence)


                    Dim out As New List(Of Bio.ISequence)

                    Dim MD = GenBankMetaDataManipulation.GetGenBankMetaDataFromSeq(Seq)
                    Dim Features = GetFeatureByTypeFromMetadata(SelectedFeatureType, MD)
                    If IsNothing(Features) = False Then
                        For Each Feat In Features
                            Try
                                out.Add(Feat.GetSubSequence(Seq))
                                out.Last.ID = Feat.Qualifiers(StandardQualifierNames.LocusTag).First
                            Catch ex As Exception
                                log.Append(Seq.ID).AppendLine()
                            End Try

                        Next
                    End If

                    Return out

                End Function

                ''' <summary>
                ''' Return Nothing Or Sorted list (By LocusTAg)
                ''' </summary>
                ''' <param name="Seq"></param>
                ''' <param name="SelectedFeatureType"></param>
                ''' <returns></returns>
                Public Shared Function GetFeturesByTypeFromSeq(Seq As Bio.ISequence,
                                               SelectedFeatureType As String) As List(Of FeatureItem)

                    Dim Md = GenBankMetaDataManipulation.GetGenBankMetaDataFromSeq(Seq)
                    Dim SelectedFeatures = GetFeatureByTypeFromMetadata(SelectedFeatureType, Md)


                    If IsNothing(SelectedFeatures) = False Then
                        Dim SelFeats = SelectedFeatures.ToList
                        '   SelFeats.Sort(AllComparares.ByLocusTag)
                        Return SelFeats
                    End If
                    Return Nothing
                End Function
                Public Shared Function GetFeturesByTypesFromMetadata(MD As Bio.IO.GenBank.GenBankMetadata,
                                               SelectedFeatureTypes As List(Of String)) As List(Of FeatureItem)

                    Dim out As New List(Of FeatureItem)
                    For Each SelectedFeature In SelectedFeatureTypes
                        Dim t = GetFeatureByTypeFromMetadata(SelectedFeature, MD)
                        If IsNothing(t) = False Then out.AddRange(t)
                    Next
                    Return out

                End Function
                ''' <summary>
                ''' Return list or empty list
                ''' </summary>
                ''' <param name="MDs"></param>
                ''' <param name="SelectedFeatureTypes"></param>
                ''' <returns></returns>
                Public Shared Function GetFeturesByTypesFromMetaDatas(MDs As List(Of Bio.IO.GenBank.GenBankMetadata),
                                                SelectedFeatureTypes As List(Of String)) As List(Of FeatureItem)

                    Dim Out As New List(Of FeatureItem)
                    For Each Md In MDs

                        Dim t = GetFeturesByTypesFromMetadata(Md, SelectedFeatureTypes)
                        If IsNothing(t) = False Then Out.AddRange(t)
                    Next

                    Return Out
                End Function
                Public Shared Function GetFeturesByTypesFromMetaDatas(MDs As List(Of Bio.IO.GenBank.GenBankMetadata),
                                               SelectedFeatureType As String) As List(Of FeatureItem)

                    Dim Out As New List(Of FeatureItem)
                    For Each Md In MDs

                        Dim t = GetFeatureByTypeFromMetadata(SelectedFeatureType, Md)
                        If IsNothing(t) = False Then Out.AddRange(t)
                    Next

                    Return Out
                End Function



                ''' <summary>
                ''' Return Ienumarable of FeatureItem or Nothing
                ''' </summary>
                ''' <param name="Type"></param>
                ''' <param name="md"></param>
                ''' <returns></returns>
                Public Shared Function GetFeatureByTypeFromMetadata(Type As String, md As Bio.IO.GenBank.GenBankMetadata) As IEnumerable(Of FeatureItem)
                    If IsNothing(md) = True Then Return Nothing
                    If IsNothing(md.Features) = True Then Return Nothing

                    Select Case Type

                        Case = StandardFeatureKeys.Attenuator
                            If md.Features.Attenuators.Count > 0 Then Return md.Features.Attenuators

                        Case = StandardFeatureKeys.CaaTSignal
                            If md.Features.CAATSignals.Count > 0 Then Return md.Features.CAATSignals

                        Case = StandardFeatureKeys.CodingSequence
                            If md.Features.CodingSequences.Count > 0 Then Return md.Features.CodingSequences

                        Case = StandardFeatureKeys.DisplacementLoop
                            If md.Features.DisplacementLoops.Count > 0 Then Return md.Features.DisplacementLoops

                        Case = StandardFeatureKeys.Enhancer
                            If md.Features.Enhancers.Count > 0 Then Return md.Features.Enhancers

                        Case = StandardFeatureKeys.Exon
                            If md.Features.Exons.Count > 0 Then Return md.Features.Exons

                        Case = StandardFeatureKeys.FivePrimeUtr
                            If md.Features.FivePrimeUTRs.Count > 0 Then Return md.Features.FivePrimeUTRs

                        Case = StandardFeatureKeys.GcSingal
                            If md.Features.GCSignals.Count > 0 Then Return md.Features.GCSignals

                        Case = StandardFeatureKeys.Gene
                            If md.Features.Genes.Count > 0 Then Return md.Features.Genes

                        Case = StandardFeatureKeys.InterveningDna
                            If md.Features.InterveningDNAs.Count > 0 Then Return md.Features.InterveningDNAs

                        Case = StandardFeatureKeys.Intron
                            If md.Features.Introns.Count > 0 Then Return md.Features.Introns

                        Case = StandardFeatureKeys.LongTerminalRepeat
                            If md.Features.LongTerminalRepeats.Count > 0 Then Return md.Features.LongTerminalRepeats

                        Case = StandardFeatureKeys.MaturePeptide
                            If md.Features.MaturePeptides.Count > 0 Then Return md.Features.MaturePeptides

                        Case = StandardFeatureKeys.MessengerRna
                            If md.Features.MessengerRNAs.Count > 0 Then Return md.Features.MessengerRNAs

                        Case = StandardFeatureKeys.Minus10Signal
                            If md.Features.Minus10Signals.Count > 0 Then Return md.Features.Minus10Signals

                        Case = StandardFeatureKeys.Minus35Signal
                            If md.Features.Minus35Signals.Count > 0 Then Return md.Features.Minus35Signals

                        Case = StandardFeatureKeys.MiscBinding
                            If md.Features.MiscBindings.Count > 0 Then Return md.Features.MiscBindings

                        Case = StandardFeatureKeys.MiscDifference
                            If md.Features.MiscDifferences.Count > 0 Then Return md.Features.MiscDifferences

                        Case = StandardFeatureKeys.MiscFeature
                            If md.Features.MiscFeatures.Count > 0 Then Return md.Features.MiscFeatures

                        Case = StandardFeatureKeys.MiscRecombination
                            If md.Features.MiscRecombinations.Count > 0 Then Return md.Features.MiscRecombinations

                        Case = StandardFeatureKeys.MiscRna
                            If md.Features.MiscRNAs.Count > 0 Then Return md.Features.MiscRNAs

                        Case = StandardFeatureKeys.MiscSignal
                            If md.Features.MiscSignals.Count > 0 Then Return md.Features.MiscSignals

                        Case = StandardFeatureKeys.MiscStructure
                            If md.Features.MiscStructures.Count > 0 Then Return md.Features.MiscStructures

                        Case = StandardFeatureKeys.ModifiedBase
                            If md.Features.ModifiedBases.Count > 0 Then Return md.Features.ModifiedBases

                        Case = StandardFeatureKeys.NonCodingRna
                            If md.Features.NonCodingRNAs.Count > 0 Then Return md.Features.NonCodingRNAs

                        Case = StandardFeatureKeys.OperonRegion
                            If md.Features.OperonRegions.Count > 0 Then Return md.Features.OperonRegions

                        Case = StandardFeatureKeys.PolyASignal
                            If md.Features.PolyASignals.Count > 0 Then Return md.Features.PolyASignals

                        Case = StandardFeatureKeys.PolyASite
                            If md.Features.PolyASites.Count > 0 Then Return md.Features.PolyASites

                        Case = StandardFeatureKeys.PrecursorRna
                            If md.Features.PrecursorRNAs.Count > 0 Then Return md.Features.PrecursorRNAs

                        Case = StandardFeatureKeys.Promoter
                            If md.Features.Promoters.Count > 0 Then Return md.Features.Promoters

                        Case = StandardFeatureKeys.ProteinBindingSite
                            If md.Features.ProteinBindingSites.Count > 0 Then Return md.Features.ProteinBindingSites

                        Case = StandardFeatureKeys.RepeatRegion
                            If md.Features.RepeatRegions.Count > 0 Then Return md.Features.RepeatRegions

                        Case = StandardFeatureKeys.ReplicationOrigin
                            If md.Features.ReplicationOrigins.Count > 0 Then Return md.Features.ReplicationOrigins

                        Case = StandardFeatureKeys.RibosomalRna
                            If md.Features.RibosomalRNAs.Count > 0 Then Return md.Features.RibosomalRNAs

                        Case = StandardFeatureKeys.RibosomeBindingSite
                            If md.Features.RibosomeBindingSites.Count > 0 Then Return md.Features.RibosomeBindingSites

                        Case = StandardFeatureKeys.SignalPeptide
                            If md.Features.SignalPeptides.Count > 0 Then Return md.Features.SignalPeptides

                        Case = StandardFeatureKeys.StemLoop
                            If md.Features.StemLoops.Count > 0 Then Return md.Features.StemLoops

                        Case = StandardFeatureKeys.TataSignal
                            If md.Features.TATASignals.Count > 0 Then Return md.Features.TATASignals

                        Case = StandardFeatureKeys.Terminator
                            If md.Features.Terminators.Count > 0 Then Return md.Features.Terminators

                        Case = StandardFeatureKeys.ThreePrimeUtr
                            If md.Features.ThreePrimeUTRs.Count > 0 Then Return md.Features.ThreePrimeUTRs

                        Case = StandardFeatureKeys.TransferMessengerRna
                            If md.Features.TransferMessengerRNAs.Count > 0 Then Return md.Features.TransferMessengerRNAs

                        Case = StandardFeatureKeys.TransferRna
                            If md.Features.TransferRNAs.Count > 0 Then Return md.Features.TransferRNAs

                        Case = StandardFeatureKeys.TransitPeptide
                            If md.Features.TransitPeptides.Count > 0 Then Return md.Features.TransitPeptides

                        Case = StandardFeatureKeys.UnsureSequenceRegion
                            If md.Features.UnsureSequenceRegions.Count > 0 Then Return md.Features.UnsureSequenceRegions

                        Case = StandardFeatureKeys.Variation
                            If md.Features.Variations.Count > 0 Then Return md.Features.Variations
                        Case Else
                            Dim res = From x In md.Features.All Where x.Key = Type

                            If res.Count > 0 Then Return res.ToList

                            Return New List(Of FeatureItem)
                    End Select
                    Return Nothing
                End Function

            End Class

            Public Class GenBankMetaDataManipulation

                Public Shared Sub SortFeatures(Seqs As List(Of Bio.ISequence))
                    For Each Seq In Seqs
                        SortFeatures(Seq)
                    Next
                End Sub
                Public Shared Sub SortFeatures(Seq As Bio.ISequence)
                    Dim Md = FeatureManipulation.GenBankMetaDataManipulation.GetGenBankMetaDataFromSeq(Seq)
                    If IsNothing(Md) = False Then
                        If IsNothing(Md.Features) = False Then
                            Md.Features.All.Sort(New Szunyi.Comparares.OneByOne.FeatureItemComparer)
                        End If
                    End If
                End Sub
                ''' <summary>
                ''' Create Default GenBankMetaData as unknown Organism and FeatureList
                ''' </summary>
                ''' <param name="Seq"></param>
                ''' <returns></returns>
                Public Shared Function CreateNAGenBankMetaData(Seq As Sequence) As GenBankMetadata
                    Dim x As New Bio.IO.GenBank.GenBankMetadata
                    x.Locus = New Bio.IO.GenBank.GenBankLocusInfo(Seq)
                    x.Locus.Date = Now
                    x.Locus.MoleculeType = MoleculeType.DNA
                    x.Locus.Name = Seq.ID
                    x.Locus.SequenceLength = Seq.Count
                    x.Locus.StrandTopology = SequenceStrandTopology.Linear
                    x.Accession = New GenBankAccession()
                    x.Accession.Primary = Seq.ID
                    x.Source = New Bio.IO.GenBank.SequenceSource
                    x.Source.CommonName = "Unknown."
                    x.Source.Organism = New Bio.IO.GenBank.OrganismInfo
                    x.Source.Organism.Species = "Unknown"
                    x.Source.Organism.Genus = "Unknown."
                    x.Features = New SequenceFeatures
                    Return x
                End Function

                ''' <summary>
                ''' Return the List of GenBankMetadatas or Empty List
                ''' </summary>
                ''' <param name="Seqs"></param>
                ''' <returns></returns>
                Public Shared Function GetGenBankMetaDatasFromSeqs(Seqs As List(Of Bio.ISequence)) As List(Of Bio.IO.GenBank.GenBankMetadata)
                    Dim Out As New List(Of Bio.IO.GenBank.GenBankMetadata)
                    If IsNothing(Seqs) = True Then Return Out
                    For Each Seq In Seqs
                        If Seq.Metadata.ContainsKey(Bio.Util.Helper.GenBankMetadataKey) Then
                            Out.Add(Seq.Metadata(Bio.Util.Helper.GenBankMetadataKey))
                        End If
                    Next
                    Return Out
                End Function
                ''' <summary>
                ''' Return The GenBankMetaData or Nothing
                ''' </summary>
                ''' <param name="Seq"></param>
                ''' <returns></returns>
                Public Shared Function GetGenBankMetaDataFromSeq(Seq As Bio.ISequence) As Bio.IO.GenBank.GenBankMetadata
                    If IsNothing(Seq) = True Then Return Nothing
                    If Seq.Metadata.ContainsKey(Bio.Util.Helper.GenBankMetadataKey) Then
                        Return (Seq.Metadata(Bio.Util.Helper.GenBankMetadataKey))
                    End If
                    Return Nothing
                End Function
                Public Shared Function GetOrCreateGenBankMetaDataFromSeq(Seq As Bio.ISequence) As Bio.IO.GenBank.GenBankMetadata
                    If IsNothing(Seq) = True Then Return Nothing
                    If Seq.Metadata.ContainsKey(Bio.Util.Helper.GenBankMetadataKey) Then
                        Return (Seq.Metadata(Bio.Util.Helper.GenBankMetadataKey))
                    Else
                        Dim md As New Bio.IO.GenBank.GenBankMetadata
                        md.Accession = New GenBankAccession()
                        md.Source = New Bio.IO.GenBank.SequenceSource()
                        md.Source.Organism = New OrganismInfo
                        md.Locus = New Bio.IO.GenBank.GenBankLocusInfo
                        md.Features = New SequenceFeatures
                        Seq.Metadata.Add(Bio.Util.Helper.GenBankMetadataKey, md)
                        Return Seq.Metadata(Bio.Util.Helper.GenBankMetadataKey)
                    End If

                End Function

                Public Shared Sub AddFeature(ByRef Seq As Sequence, Loci As ILocation, FeatureKey As String, Optional LocusTags As List(Of String) = Nothing)
                    Dim x As New FeatureItem(FeatureKey, Loci)
                    If IsNothing(LocusTags) = False Then x.Qualifiers(StandardQualifierNames.LocusTag) = LocusTags
                    Dim Md = Szunyi.Features.FeatureManipulation.GenBankMetaDataManipulation.GetOrCreateGenBankMetaDataFromSeq(Seq)
                    Md.Features.All.Add(x)

                End Sub
                Public Shared Sub AddFeature(ByRef Seq As Sequence, Feat As FeatureItem)
                    Dim Md = Szunyi.Features.FeatureManipulation.GenBankMetaDataManipulation.GetOrCreateGenBankMetaDataFromSeq(Seq)
                    If IsNothing(Md) = False Then
                        AddFeature(Md, Feat, Feat.Key)
                    End If
                End Sub
                Public Shared Sub AddFeatures(ByRef Seq As Sequence, Feats As List(Of FeatureItem))
                    Dim Md = Szunyi.Features.FeatureManipulation.GenBankMetaDataManipulation.GetOrCreateGenBankMetaDataFromSeq(Seq)
                    For Each Feat In Feats
                        AddFeature(Md, Feat, Feat.Key)
                    Next

                End Sub
                Public Shared Sub AddFeatures(ByRef Seq As Sequence, Feats As List(Of Szunyi.mRNA.Transcript.TemplateSwitch))
                    Dim Md = Szunyi.Features.FeatureManipulation.GenBankMetaDataManipulation.GetOrCreateGenBankMetaDataFromSeq(Seq)
                    For Each Feat In Feats
                        Dim f As New FeatureItem(StandardFeatureKeys.Intron, Feat.Loci)
                        f.Label = Feat.Count
                        AddFeature(Md, f, StandardFeatureKeys.Intron)
                    Next

                End Sub
                Public Shared Sub AddFeature(Md As GenBankMetadata, x As FeatureItem, FeatureKey As String)
                    Select Case FeatureKey
                        Case StandardFeatureKeys.CodingSequence
                            Dim l As New CodingSequence(x.Location)
                            Szunyi.Features.FeatureManipulation.MergeFeatures.Merge2Features(x, l, True)
                            Md.Features.All.Add(l)
                        Case StandardFeatureKeys.MessengerRna
                            Dim l As New MessengerRna(x.Location)
                            Szunyi.Features.FeatureManipulation.MergeFeatures.Merge2Features(x, l, True)
                            Md.Features.All.Add(l)
                            Dim alf As Int16 = 65
                        Case StandardFeatureKeys.Gene
                            Dim l As New Gene(x.Location)
                            Szunyi.Features.FeatureManipulation.MergeFeatures.Merge2Features(x, l, True)
                            Md.Features.All.Add(l)
                        Case StandardFeatureKeys.NonCodingRna
                            Dim l As New NonCodingRna(x.Location)
                            Szunyi.Features.FeatureManipulation.MergeFeatures.Merge2Features(x, l, True)
                            Md.Features.All.Add(l)
                        Case StandardFeatureKeys.MiscFeature
                            Dim l As New MiscFeature(x.Location)
                            Szunyi.Features.FeatureManipulation.MergeFeatures.Merge2Features(x, l, True)
                            Md.Features.All.Add(l)
                        Case StandardFeatureKeys.MiscRecombination
                            Dim l As New MiscRecombination(x.Location)
                            Szunyi.Features.FeatureManipulation.MergeFeatures.Merge2Features(x, l, True)
                            Md.Features.All.Add(l)
                        Case StandardFeatureKeys.MiscSignal
                            Dim l As New MiscSignal(x.Location)
                            Szunyi.Features.FeatureManipulation.MergeFeatures.Merge2Features(x, l, True)
                            Md.Features.All.Add(l)
                        Case StandardFeatureKeys.Intron
                            Dim l As New Intron(x.Location)
                            Szunyi.Features.FeatureManipulation.MergeFeatures.Merge2Features(x, l, True)
                            Md.Features.All.Add(l)
                        Case StandardFeatureKeys.Promoter
                            Dim l As New Promoter(x.Location)
                            Szunyi.Features.FeatureManipulation.MergeFeatures.Merge2Features(x, l, True)
                            Md.Features.All.Add(l)
                        Case StandardFeatureKeys.Exon
                            Dim l As New Exon(x.Location)
                            Szunyi.Features.FeatureManipulation.MergeFeatures.Merge2Features(x, l, True)
                            Md.Features.All.Add(l)
                        Case StandardFeatureKeys.Terminator
                            Dim l As New Terminator(x.Location)
                            Szunyi.Features.FeatureManipulation.MergeFeatures.Merge2Features(x, l, True)
                            Md.Features.All.Add(l)
                        Case StandardFeatureKeys.Mobile_element
                            Dim l As New Mobile_Element(x.Location)
                            Szunyi.Features.FeatureManipulation.MergeFeatures.Merge2Features(x, l, True)
                            Md.Features.All.Add(l)
                        Case StandardFeatureKeys.PolyASignal
                            Dim l As New PolyASignal(x.Location)
                            Szunyi.Features.FeatureManipulation.MergeFeatures.Merge2Features(x, l, True)
                            Md.Features.All.Add(l)
                        Case StandardFeatureKeys.MaturePeptide
                            Dim l As New MaturePeptide(x.Location)
                            Szunyi.Features.FeatureManipulation.MergeFeatures.Merge2Features(x, l, True)
                            Md.Features.All.Add(l)
                        Case StandardFeatureKeys.OperonRegion
                            Dim l As New OperonRegion(x.Location)
                            Szunyi.Features.FeatureManipulation.MergeFeatures.Merge2Features(x, l, True)
                            Md.Features.All.Add(l)
                        Case StandardFeatureKeys.FivePrimeUtr
                            Dim l As New FivePrimeUtr(x.Location)
                            Szunyi.Features.FeatureManipulation.MergeFeatures.Merge2Features(x, l, True)
                            Md.Features.All.Add(l)
                        Case StandardFeatureKeys.ThreePrimeUtr
                            Dim l As New ThreePrimeUtr(x.Location)
                            Szunyi.Features.FeatureManipulation.MergeFeatures.Merge2Features(x, l, True)
                            Md.Features.All.Add(l)
                        Case Else
                            Md.Features.All.Add(x)
                            Dim alf As Int16 = 65
                    End Select


                End Sub

                Public Shared Sub Remove_Feature(seq As Sequence, x As FeatureItem)
                    Dim Md = Szunyi.Features.FeatureManipulation.GenBankMetaDataManipulation.GetOrCreateGenBankMetaDataFromSeq(seq)
                    Md.Features.All.Remove(x)
                End Sub
                Public Shared Sub Remove_Feature(seq As Sequence, Feats As List(Of FeatureItem))
                    If IsNothing(Feats) = True Then Exit Sub
                    Dim Md = Szunyi.Features.FeatureManipulation.GenBankMetaDataManipulation.GetOrCreateGenBankMetaDataFromSeq(seq)
                    For Each x In Feats
                        Md.Features.All.Remove(x)
                    Next
                End Sub

                Public Shared Function GetHeaders(seqs As List(Of ISequence)) As List(Of String)
                    Dim out As New List(Of String)
                    Dim Header As New System.Text.StringBuilder
                    Header.Append("Name").Append(vbTab)
                    Header.Append("Length").Append(vbTab)
                    Header.Append("GC%").Append(vbTab)
                    Header.Append("Seq Type").Append(vbTab)
                    Header.Append("Molecula Type").Append(vbTab)
                    Header.Append("Seq strand").Append(vbTab)
                    Header.Append("Common Name").Append(vbTab)
                    Header.Append("Strain").Append(vbTab)
                    For Each Seq In seqs
                        Dim md = GetGenBankMetaDataFromSeq(Seq)
                        If IsNothing(md) = False Then
                            Dim str As New System.Text.StringBuilder
                            str.Append(md.Locus.Name).Append(vbTab)
                            str.Append(md.Locus.SequenceLength).Append(vbTab)
                            str.Append(Szunyi.DNA.Common.Get_Percents_GC(Seq)).Append(vbTab)

                            str.Append(Bio.IO.GenBank.GenBankLocusTokenParser.LocusConstants.AlphabetTypes(md.Locus.MoleculeType)).Append(vbTab)
                            str.Append(Bio.IO.GenBank.GenBankLocusTokenParser.LocusConstants.SequenceStrandTypes(md.Locus.Strand)).Append(vbTab)
                            str.Append(md.Locus.Strand).Append(vbTab)
                            '  str.Append(md.Locus.StrandTopology).Append(vbTab)
                            '  str.Append(md.Locus.Date.ToString).Append(vbTab)
                            ' str.Append(md.Accession.Primary).Append(vbTab)
                            ' str.Append(md.Accession.Secondary).Append(vbTab)
                            ' str.Append(md.Definition).Append(vbTab)
                            Dim s = md.Source
                            str.Append(s.CommonName).Append(vbTab)

                            Dim feats = Szunyi.Features.FeatureManipulation.GetFeaturesByType.GetFeatureByTypeFromMetadata("source", md)
                            If feats.Count <> 0 Then
                                Dim Q = Szunyi.Features.FeatureManipulation.Qulifiers.Get_All_Values(feats.First)
                                str.Append(Q)
                            End If
                            out.Add(str.ToString)
                        End If
                    Next
                    Return out
                End Function

                Public Shared Function Get_Features_Keys(seqs As List(Of ISequence)) As List(Of String)
                    Dim out As New List(Of String)
                    Dim Feat = Szunyi.Features.FeatureManipulation.GetFeaturesByType.Get_All_Features(seqs)

                    For Each f In Feat
                        If out.Contains(f.Key) = False Then out.Add(f.Key)
                    Next
                    Return out

                End Function
                Public Shared Function Get_Accesion(Seq As Bio.Sequence) As String
                    Dim md = GetGenBankMetaDataFromSeq(Seq)
                    If IsNothing(md) = True Then Return String.Empty
                    Return md.Accession.Primary
                End Function
                Public Shared Function Get_Common_Name(seq As ISequence) As String
                    Dim Source = Get_Source(seq)
                    If IsNothing(Source) = True Then Return String.Empty
                    If IsNothing(Source.Organism) = True Then Return String.Empty
                    Return Source.Organism.Species
                End Function
                Public Shared Function Get_Source(Seq As Bio.ISequence) As Bio.IO.GenBank.SequenceSource
                    Dim md As Bio.IO.GenBank.GenBankMetadata = Seq.Metadata(Bio.Util.Helper.GenBankMetadataKey)
                    If IsNothing(md) = True Then Return Nothing
                    Return md.Source
                End Function
            End Class

            Public Class MergeFeatures
                Public Shared Function MergeByTypeAnsPositions(Features As List(Of FeatureItem)) As List(Of FeatureItem)
                    Dim out As New List(Of FeatureItem)
                    For Each FeatureType In FeatureManipulation.Iterator.ByType(Features)
                        For Each SamePosFeat In FeatureManipulation.Iterator.ByLocation(FeatureType)
                            If SamePosFeat.Count > 1 Then
                                Dim nFeat = Szunyi.Features.FeatureManipulation.MergeFeatures.MergeQualifiers(SamePosFeat)
                                out.Add(nFeat)
                            Else
                                out.Add(SamePosFeat.First)
                            End If
                        Next
                    Next
                    Return out
                End Function

                ''' <summary>
                ''' Merge the Annotaion if different, but not The LocusTag
                ''' </summary>
                ''' <param name="FeatureForm"></param>
                ''' <param name="FeatureTo"></param>
                ''' <param name="WithLocusTag"></param>
                ''' <returns></returns>
                Public Shared Function Merge2Features(FeatureForm As FeatureItem,
                                                      FeatureTo As FeatureItem,
                                                      WithLocusTag As Boolean, Optional WithLocations As Boolean = False) As FeatureItem
                    Dim Qulifiers = StandardQualifierNames.All
                    For Each Qual In StandardQualifierNames.All
                        If WithLocusTag = False And Qual = StandardQualifierNames.LocusTag Then

                        Else

                            If FeatureTo.Qualifiers.ContainsKey(Qual) = False AndAlso FeatureForm.Qualifiers.ContainsKey(Qual) = True Then
                                FeatureTo.Qualifiers(Qual) = FeatureForm.Qualifiers(Qual)
                            ElseIf FeatureTo.Qualifiers.ContainsKey(Qual) AndAlso FeatureForm.Qualifiers.ContainsKey(Qual) = True Then
                                Dim sFeat1 = Szunyi.Text.General.GetText(FeatureTo.Qualifiers(Qual), " ").Replace(Chr(34), "")
                                Dim sFeat2 = Szunyi.Text.General.GetText(FeatureForm.Qualifiers(Qual), " ").Replace(Chr(34), "")
                                If sFeat1 <> sFeat2 Then
                                    Dim x As New List(Of String)
                                    x.Add(sFeat1)
                                    x.Add(sFeat2)
                                    FeatureTo.Qualifiers(Qual) = x
                                End If
                            End If
                        End If

                    Next
                    If WithLocations = True Then

                    End If
                    Return FeatureTo
                End Function

                ''' <summary>
                ''' Find the Merge the Annotaion if different, but not The LocusTag
                ''' </summary>
                ''' <param name="FeatureTo"></param>
                ''' <param name="FeatList"></param>
                ''' <returns></returns>
                Public Shared Function FindAndMerge2Features(FeatureTo As ExtFeature,
                                                  FeatList As Szunyi.ListOf.ExtFeatureList) As FeatureItem
                    Dim LocusTags = Common.GetLocusTags(FeatureTo)
                    Dim FeaturesFrom As List(Of ExtFeature) = ExtFeatureManipulation.GetExtFeatures(LocusTags, FeatList)


                    Dim Qulifiers = StandardQualifierNames.All
                    For Each CorrespondingExtFeature In FeaturesFrom
                        For Each Qual In StandardQualifierNames.All
                            If Qual <> StandardQualifierNames.LocusTag Then
                                If FeatureTo.Feature.Qualifiers.ContainsKey(Qual) = False AndAlso
                                CorrespondingExtFeature.Feature.Qualifiers.ContainsKey(Qual) = True Then
                                    FeatureTo.Feature.Qualifiers(Qual) = CorrespondingExtFeature.Feature.Qualifiers(Qual)
                                ElseIf FeatureTo.Feature.Qualifiers.ContainsKey(Qual) AndAlso
                                CorrespondingExtFeature.Feature.Qualifiers.ContainsKey(Qual) = True Then
                                    Dim sFeat1 = FeatureTo.Feature.Qualifiers(Qual).ToList
                                    Dim SFeat2 As New List(Of String)
                                    For Each Item In sFeat1
                                        SFeat2.AddRange(Split(Item, vbCrLf))
                                    Next
                                    For Each Line In CorrespondingExtFeature.Feature.Qualifiers(Qual)
                                        If SFeat2.Contains(Line) = False Then
                                            SFeat2.Add(Line)
                                        Else
                                            Dim alf As Int16 = 43
                                        End If
                                    Next
                                    FeatureTo.Feature.Qualifiers(Qual) = SFeat2
                                End If
                            End If

                        Next
                    Next

                    Return FeatureTo.Feature
                End Function

                ''' <summary>
                ''' Return the Connencted Qulifiers or Nothing
                ''' if dbcrossref Not "" then search in DatabaseCrossFiles
                ''' </summary>
                ''' <param name="Feat"></param>
                ''' <param name="FirstQulifier"></param>
                ''' <param name="SecondQulifier"></param>
                ''' <param name="dbCrossRef"></param>
                ''' <returns></returns>
                Public Shared Function GetCorrespondingQulifierAndDbXref(Feat As FeatureItem, FirstQulifier As String, SecondQulifier As String, Optional dbCrossRef As String = "") As Szunyi.Other_Database.CrossRefs.CrossRefOneToOne
                    If Feat.Qualifiers.ContainsKey(FirstQulifier) = False Then Return Nothing
                    If Feat.Qualifiers.ContainsKey(SecondQulifier) = False Then Return Nothing
                    Dim First = Szunyi.Text.General.GetText(Feat.Qualifiers(FirstQulifier), " ").Replace(Chr(34), "")
                    If dbCrossRef <> "" Then
                        Dim tmp = From x In Feat.Qualifiers(SecondQulifier) Where x.Contains(dbCrossRef)

                        If tmp.Count = 0 Then Return Nothing
                        Dim Second = Szunyi.Text.General.GetText(tmp.ToList).Replace(dbCrossRef, "").Replace(Chr(34), "")
                        Return New Szunyi.Other_Database.CrossRefs.CrossRefOneToOne(First, Second)
                    Else
                        Dim Second = Szunyi.Text.General.GetText(Feat.Qualifiers(SecondQulifier), " ")
                        Return New Szunyi.Other_Database.CrossRefs.CrossRefOneToOne(First, Second)
                    End If

                End Function

                ''' <summary>
                ''' Return the Connencted Qulifiers or Nothing
                ''' if dbcrossref Not "" then search in DatabaseCrossFiles
                ''' </summary>
                ''' <param name="Feat"></param>
                ''' <param name="FirstQulifier"></param>
                ''' <param name="SecondQulifiers"></param>
                ''' <returns></returns>
                Public Shared Function GetCorrespondingQulifierAndDbXref(Feat As FeatureItem, FirstQulifier As String, SecondQulifiers() As String) As Szunyi.Other_Database.CrossRefs.CrossRefOneToOne
                    If Feat.Qualifiers.ContainsKey(FirstQulifier) = False Then Return Nothing
                    Dim Out As New List(Of String)
                    For Each SecondQulifier In SecondQulifiers
                        If Feat.Qualifiers.ContainsKey(SecondQulifier) = True Then
                            Dim s = Szunyi.Text.General.GetText(Feat.Qualifiers(SecondQulifier), " ")
                            If Out.Contains(s) = False Then Out.Add(s)
                        End If
                    Next
                    If Out.Count = 0 Then Return Nothing
                    Dim First = Szunyi.Text.General.GetText(Feat.Qualifiers(FirstQulifier), " ").Replace(Chr(34), "")


                    Dim Second = Szunyi.Text.General.GetText(Out, " ").Replace(Chr(34), "")
                    Return New Szunyi.Other_Database.CrossRefs.CrossRefOneToOne(First, Second)


                End Function

                Public Shared Sub MergeQualifiers(ByRef From As FeatureItem, ByRef Toa As FeatureItem)
                    Toa.Qualifiers.Clear()
                    For Each sg In From.Qualifiers
                        Toa.Qualifiers.Add(sg.Key, sg.Value)
                    Next

                End Sub
                Public Shared Function MergeQualifiers(Feats As List(Of FeatureItem)) As FeatureItem
                    Dim out As New FeatureItem(Feats.First.Key, Feats.First.Location)
                    For Each QulifierName In StandardQualifierNames.All
                        Dim t As New List(Of String)
                        For Each feat In Feats
                            If feat.Qualifiers.ContainsKey(QulifierName) Then
                                t.AddRange(feat.Qualifiers(QulifierName))
                            End If
                        Next
                        t = t.Distinct.ToList
                        If t.Count > 1 Then
                            Dim alf As Int16 = 43
                            out.Qualifiers.Add(QulifierName, t)
                        ElseIf t.Count = 1 Then
                            out.Qualifiers.Add(QulifierName, t)
                        End If
                    Next
                    Return out
                End Function
                Public Shared Function MergeQualifiers(Feats() As FeatureItem) As FeatureItem
                    Dim out As New FeatureItem(Feats.First.Key, Feats.First.Location)
                    For Each QulifierName In StandardQualifierNames.All
                        Dim t As New List(Of String)
                        For Each feat In Feats
                            If feat.Qualifiers.ContainsKey(QulifierName) Then
                                t.AddRange(feat.Qualifiers(QulifierName))
                            End If
                        Next
                        t = t.Distinct.ToList
                        If t.Count > 1 Then
                            Dim alf As Int16 = 43
                            out.Qualifiers.Add(QulifierName, t)
                        ElseIf t.Count = 1 Then
                            out.Qualifiers.Add(QulifierName, t)
                        End If
                    Next
                    Return out
                End Function
                Public Shared Function MergeQualifiers(BasicFeat As FeatureItem, Feats As List(Of FeatureItem)) As FeatureItem
                    For Each QulifierName In StandardQualifierNames.All
                        Dim t As New List(Of String)
                        For Each feat In Feats
                            If feat.Qualifiers.ContainsKey(QulifierName) Then
                                t.AddRange(feat.Qualifiers(QulifierName))
                            End If
                        Next
                        t = t.Distinct.ToList
                        If t.Count > 1 Then
                            Dim alf As Int16 = 43
                            BasicFeat.Qualifiers.Add(QulifierName, t)
                        ElseIf t.Count = 1 Then
                            BasicFeat.Qualifiers.Add(QulifierName, t)
                        End If

                    Next
                    Return BasicFeat
                End Function
                Friend Shared Function AddFeaturesFromOneSeqToAnotherByMatchingFeatureSequence(ByRef Features_From As Sequence, ByRef result As Sequence) As Bio.ISequence

                    Dim All_Features = Szunyi.Features.FeatureManipulation.GetFeaturesByType.GetFeturesByTypesFromSeq(Features_From, StandardFeatureKeys.All.ToList)
                    Dim All_Features_Seqs = Szunyi.Sequences.SequenceManipulation.SeqsToString.FromFeatures(Features_From, All_Features)
                    Dim tmp = Szunyi.Sequences.SequenceManipulation.UniqueDistinct.Get1CopyBySeq(All_Features_Seqs)
                    Dim Indexes As List(Of Integer) = Szunyi.Sequences.SequenceManipulation.UniqueDistinct.Get_Indexes_Of_First_Occurence(All_Features_Seqs, tmp)

                    Dim Md As Bio.IO.GenBank.GenBankMetadata = Szunyi.Features.FeatureManipulation.GenBankMetaDataManipulation.GetOrCreateGenBankMetaDataFromSeq(result)
                    Dim nSeqDupl As New Bio.Sequence(Alphabets.AmbiguousDNA, result.ConvertToString & result.ConvertToString)

                    For Each Index In Indexes
                        If All_Features(Index).Location.LocationEnd - All_Features(Index).Location.LocationStart > 12 Then
                            Dim Pos = Szunyi.Text.Regexp.SimpleBoyerMooreSearch(nSeqDupl.ToArray, All_Features_Seqs(Index).ToArray) + 1
                            If Pos > 0 Then

                                Dim Endy As Integer = Pos + All_Features_Seqs(Index).Count - 1

                                If Endy > result.Count Then
                                    Endy = Endy - result.Count
                                End If
                                If Endy < 1 Then
                                    Endy = result.Count + Endy
                                End If
                                If Pos < 1 Then
                                    Pos = result.Count + Pos
                                End If
                                Dim x As New FeatureItem(All_Features(Index).Key, Pos & ".." & Endy)
                                Szunyi.Features.FeatureManipulation.MergeFeatures.Merge2Features(All_Features(Index), x, True)
                                Szunyi.Features.FeatureManipulation.GenBankMetaDataManipulation.AddFeature(Md, x, x.Key)
                                Dim t As Int16 = 76
                            End If
                            Dim tmpSeq = All_Features_Seqs(Index).GetReverseComplementedSequence


                            Dim Pos2 = Szunyi.Text.Regexp.SimpleBoyerMooreSearch(nSeqDupl.ToArray, tmpSeq.ToArray) + 1
                            If Pos2 > 0 Then

                                Dim Endy As Integer = Pos2 + All_Features_Seqs(Index).Count - 1
                                If Pos2 < 1 Then
                                    Pos2 = result.Count + Pos2
                                End If
                                If Endy < 1 Then
                                    Endy = result.Count + Endy
                                End If
                                If Endy > result.Count Then
                                    Endy = Endy - result.Count
                                End If

                                Dim x As New FeatureItem(All_Features(Index).Key, "complement(" & Pos2 & ".." & Endy & ")")
                                Szunyi.Features.FeatureManipulation.MergeFeatures.Merge2Features(All_Features(Index), x, True)
                                Szunyi.Features.FeatureManipulation.GenBankMetaDataManipulation.AddFeature(Md, x, x.Key)
                                Dim t As Int16 = 76
                            End If
                        End If

                    Next
                    Return result
                End Function

                Friend Shared Function AddFeaturesFromOneSeqToAnotherByMatchingSequences(result As Sequence, att_Seqs As List(Of Sequence), All_Features As List(Of FeatureItem)) As Sequence

                    Dim All_Features_Seqs = att_Seqs

                    Dim Md As Bio.IO.GenBank.GenBankMetadata = Szunyi.Features.FeatureManipulation.GenBankMetaDataManipulation.GetOrCreateGenBankMetaDataFromSeq(result)
                    Dim nSeqDupl As New Bio.Sequence(Alphabets.AmbiguousDNA, result.ConvertToString & result.ConvertToString)

                    For i1 = 0 To All_Features.Count - 1
                        If All_Features(i1).Location.LocationEnd - All_Features(i1).Location.LocationStart > 12 Then
                            Dim Pos = Szunyi.Text.Regexp.SimpleBoyerMooreSearch(nSeqDupl.ToArray, All_Features_Seqs(i1).ToArray) + 1
                            If Pos > 0 Then

                                Dim Endy As Integer = Pos + All_Features_Seqs(i1).Count - 1

                                If Endy > result.Count Then
                                    Endy = Endy - result.Count
                                End If
                                If Endy < 1 Then
                                    Endy = result.Count + Endy
                                End If
                                If Pos < 1 Then
                                    Pos = result.Count + Pos
                                End If
                                Dim x As New FeatureItem(All_Features(i1).Key, Pos & ".." & Endy)
                                Szunyi.Features.FeatureManipulation.MergeFeatures.Merge2Features(All_Features(i1), x, True)
                                Szunyi.Features.FeatureManipulation.GenBankMetaDataManipulation.AddFeature(Md, x, x.Key)
                                Dim t As Int16 = 76
                            End If
                            Dim tmpSeq = All_Features_Seqs(i1).GetReverseComplementedSequence


                            Dim Pos2 = Szunyi.Text.Regexp.SimpleBoyerMooreSearch(nSeqDupl.ToArray, tmpSeq.ToArray) + 1
                            If Pos2 > 0 Then

                                Dim Endy As Integer = Pos2 + All_Features_Seqs(i1).Count - 1
                                If Pos2 < 1 Then
                                    Pos2 = result.Count + Pos2
                                End If
                                If Endy < 1 Then
                                    Endy = result.Count + Endy
                                End If
                                If Endy > result.Count Then
                                    Endy = Endy - result.Count
                                End If

                                Dim x As New FeatureItem(All_Features(i1).Key, "complement(" & Pos2 & ".." & Endy & ")")
                                Szunyi.Features.FeatureManipulation.MergeFeatures.Merge2Features(All_Features(i1), x, True)
                                Szunyi.Features.FeatureManipulation.GenBankMetaDataManipulation.AddFeature(Md, x, x.Key)
                                Dim t As Int16 = 76
                            End If
                        End If

                    Next
                    Return result
                End Function
            End Class

            Public Class GetFeatureByQualifier

                Public Shared Function GetFeaturesByQulifiersPerfect(t As List(Of FeatureItem),
                                                                             searchSetting As SettingForSearchInQulifier)
                    Dim out As New List(Of FeatureItem)

                    For Each s In searchSetting.InterestingStrings
                        Dim g = From x In t Where x.Qualifiers.ContainsKey(searchSetting.QulifierName) AndAlso
                                                    String.Compare(x.Qualifiers(searchSetting.QulifierName).First, s, StringComparison.InvariantCultureIgnoreCase)
                        If g.Count > 0 Then out.AddRange(g.ToList)
                    Next


                    Return out
                End Function
                Public Shared Function GetFeaturesByQulifiersContains(t As List(Of FeatureItem),
                                                                      searchSetting As SettingForSearchInQulifier)
                    Dim out As New List(Of FeatureItem)

                    For Each s In searchSetting.InterestingStrings
                        Dim g = From x In t Where x.Qualifiers.ContainsKey(searchSetting.QulifierName) AndAlso
                                                    x.Qualifiers(searchSetting.QulifierName).First.IndexOf(s, StringComparison.InvariantCultureIgnoreCase) > -1
                        If g.Count > 0 Then out.AddRange(g.ToList)
                    Next


                    Return out
                End Function
                Public Shared Function GetFeaturesByNoValues(t As List(Of FeatureItem),
                                                                      searchSetting As SettingForSearchInQulifier)
                    Dim out As New List(Of FeatureItem)


                    Dim g = From x In t Where x.Qualifiers.ContainsKey(searchSetting.QulifierName) = False OrElse
                                                    x.Qualifiers(searchSetting.QulifierName).Count = 0
                    If g.Count > 0 Then out.AddRange(g.ToList)

                    Return out
                End Function

                Public Shared Function GetShortLocusTag(FeatItem As FeatureItem) As String
                    Return Split(GetLocusTag(FeatItem), ".").First
                End Function
                Public Shared Function GetLocusTag(FeatItem As FeatureItem) As String
                    If FeatItem.Qualifiers.ContainsKey(StandardQualifierNames.LocusTag) Then
                        Return FeatItem.Qualifiers(StandardQualifierNames.LocusTag).First
                    Else
                        Return String.Empty
                    End If

                End Function
            End Class

            Public Class Common
                Public Shared Function Clones(Feats As List(Of FeatureItem)) As List(Of FeatureItem)
                    Dim out As New List(Of FeatureItem)
                    For Each Item In Feats
                        out.Add(Item.Clone)
                    Next
                    Return out
                End Function
                Public Shared Function GetName(Feat As FeatureItem) As List(Of String)
                    Dim out As New List(Of String)

                    If Feat.Qualifiers.ContainsKey(StandardQualifierNames.LocusTag) AndAlso Feat.Qualifiers(StandardQualifierNames.LocusTag).Count > 0 Then
                        out.Add(Feat.Qualifiers(StandardQualifierNames.LocusTag).First)
                    ElseIf Feat.Qualifiers.ContainsKey(StandardQualifierNames.GeneSymbol) AndAlso Feat.Qualifiers(StandardQualifierNames.GeneSymbol).Count > 0 Then
                        out.Add(Feat.Qualifiers(StandardQualifierNames.GeneSymbol).First)
                    ElseIf Feat.Qualifiers.ContainsKey(StandardQualifierNames.Note) AndAlso Feat.Qualifiers(StandardQualifierNames.Note).Count > 0 Then
                        out.Add(Feat.Qualifiers(StandardQualifierNames.Note).First)
                    ElseIf Feat.Label <> "" Then
                        out.Add(Feat.Label)
                    End If
                    If out.Count = 0 Then out.Add("UnKnown")
                    Return out
                End Function
                '    Public Shared LocusTagComparer As New 
                Public Shared Function GetExtFeatures(locusTags As List(Of String), featList As ListOf.ExtFeatureList) As List(Of ExtFeature)
                    Dim x As New List(Of String)
                    Dim Out As New List(Of ExtFeature)
                    Dim tmpExtFeature As New Szunyi.ListOf.ExtFeature("")
                    Dim LocusTagComparer = Szunyi.Comparares.AllComparares.ByExtFeatureLocusTag
                    For Each LocusTag In locusTags

                        tmpExtFeature.LocusTag = LocusTag
                        Dim i1 = featList.FetauresByLocustag.BinarySearch(tmpExtFeature, LocusTagComparer)
                        If i1 >= 0 Then
                            Out.Add(featList.FetauresByLocustag(i1))
                            For i2 = i1 - 1 To 0 Step -1
                                If featList.FetauresByLocustag(i2).LocusTag = LocusTag Then
                                    Out.Add(featList.FetauresByLocustag(i2))
                                Else
                                    Exit For
                                End If
                            Next
                            For i2 = i1 + 1 To featList.FetauresByLocustag.Count - 1
                                If featList.FetauresByLocustag(i2).LocusTag = LocusTag Then
                                    Out.Add(featList.FetauresByLocustag(i2))
                                Else
                                    Exit For
                                End If
                            Next
                        End If
                    Next
                    Return Out
                End Function

                Public Shared Function GetLocusTags(ExtFeat As ExtFeature) As List(Of String)
                    Dim LocusTags As New List(Of String) ' First is full, Second is Short LocusTag
                    LocusTags.Add(ExtFeat.LocusTag)
                    Dim tmp = Split(ExtFeat.LocusTag, ".").First
                    If LocusTags.First <> tmp Then LocusTags.Add(tmp)
                    Return LocusTags
                End Function

                Public Shared Function Get_ShortLocusTags(Feats As List(Of FeatureItem)) As List(Of String)
                    Dim out As New List(Of String)
                    For Each Feat In Feats
                        out.Add(Get_ShortLocusTag(Feat))
                    Next
                    Return out
                End Function

                Public Shared Function Get_ShortLocusTag(Feat As FeatureItem) As String
                    Dim out As New List(Of String)
                    Dim Ltag = Get_LocusTag(Feat)
                    Dim s = Split(Ltag, ".").First
                    Return s.Trim(Chr(34))

                End Function

                Public Shared Function GetNofIntron(feature As FeatureItem) As Integer
                    If feature.Location.Operator = LocationOperator.Complement Then
                        If feature.Location.SubLocations.First.Operator = LocationOperator.Join Then
                            Return feature.Location.SubLocations.First.SubLocations.Count - 1
                        Else
                            Return 0
                        End If
                    Else ' No Complement
                        If feature.Location.Operator = LocationOperator.Join Then
                            Return feature.Location.SubLocations.First.SubLocations.Count - 1
                        Else
                            Return 0
                        End If
                    End If
                End Function

                Public Shared Function GetNofExon(feature As FeatureItem) As Integer
                    If feature.Location.Operator = LocationOperator.Complement Then
                        If feature.Location.SubLocations.First.Operator = LocationOperator.Join Then
                            Return feature.Location.SubLocations.First.SubLocations.Count
                        Else
                            Return 1
                        End If
                    Else ' No Complement
                        If feature.Location.Operator = LocationOperator.Join Then
                            Return feature.Location.SubLocations.Count
                        Else
                            Return 1
                        End If
                    End If
                End Function

                Public Shared Function IsSameTranslation(CDS As ExtFeature, NewSeq As Bio.ISequence, NewCds As FeatureItem) As Boolean
                    Dim locibuilder As New Bio.IO.GenBank.LocationBuilder
                    Dim s, s2 As String
                    Try
                        s = Szunyi.Translate.TranaslateFromFeature(CDS.Feature, CDS.Seq)
                        s2 = Szunyi.Translate.TranaslateFromFeature(NewCds, NewSeq)
                        If s = s2 Then Return True
                    Catch ex As Exception

                        Dim gebasy As Boolean = True
                    End Try

                    Return False
                End Function

                Public Shared Sub Correct_LocusTags_From_Seqs(Seqs As List(Of Bio.ISequence))
                    For Each Seq In Seqs
                        Correct_LocusTags_From_Seq(Seq)
                    Next
                End Sub

                Public Shared Sub Correct_LocusTags_From_Seq(Seq As Bio.ISequence)
                    Dim Md = FeatureManipulation.GenBankMetaDataManipulation.GetGenBankMetaDataFromSeq(Seq)
                    If IsNothing(Md) = False Then
                        Correct_LocusTags_From_MetaData(Md)
                    End If
                End Sub
                Public Shared Sub Correct_LocusTags_From_MetaData(Md As Bio.IO.GenBank.GenBankMetadata)
                    If IsNothing(Md) = False Then
                        For Each Feat As FeatureItem In Md.Features.All
                            Correct_LocusTag(Feat)
                        Next
                    End If
                End Sub
                Public Shared Sub Correct_LocusTag(Feat As FeatureItem)
                    If Feat.Qualifiers.ContainsKey(StandardQualifierNames.LocusTag) = True Then
                        Dim LocusTag = Get_LocusTag(Feat)
                        Feat.Qualifiers(StandardQualifierNames.LocusTag).Clear()
                        Feat.Qualifiers(StandardQualifierNames.LocusTag).Add(LocusTag)
                    End If
                End Sub
                Public Shared Function Convert_FullSeqs_To_Features(Seqs As List(Of Sequence), Feature_Key As String) As List(Of FeatureItem)
                    Dim Out As New List(Of FeatureItem)
                    For Each Seq In Seqs
                        Out.Add(Convert_FullSeq_To_Feature(Seq, Feature_Key))
                    Next
                    Return Out
                End Function
                Public Shared Function Convert_FullSeq_To_Feature(Seq As Sequence, Feature_Key As String) As FeatureItem

                    Dim t As New FeatureItem(Feature_Key, "1.." & Seq.Count)
                    t.Qualifiers.Add(StandardQualifierNames.Label, Szunyi.Text.Lists.GetListFormText(Seq.ID))

                    Return t
                End Function

                ''' <summary>
                ''' Return LocusTags as list Of String 
                ''' If no LocusTag founded Empty String is returned
                ''' </summary>
                ''' <param name="Feats"></param>
                ''' <returns></returns>
                Public Shared Function Get_LocusTags(Feats As List(Of FeatureItem)) As List(Of String)
                    Dim Out As New List(Of String)
                    For Each Feat In Feats
                        Out.Add(Get_LocusTag(Feat))
                    Next
                    Return Out
                End Function
                ''' <summary>
                ''' Return LocusTag or Empty String
                ''' </summary>
                ''' <param name="feat"></param>
                ''' <returns></returns>
                Public Shared Function Get_LocusTag(feat As FeatureItem) As String
                    If feat.Qualifiers.ContainsKey(StandardQualifierNames.LocusTag) = True AndAlso
                        feat.Qualifiers(StandardQualifierNames.LocusTag).Count > 0 Then
                        Return feat.Qualifiers(StandardQualifierNames.LocusTag).First.Replace(Chr(34), "")
                    Else
                        Return String.Empty
                    End If

                End Function

                Public Shared Function Get_ByQualifier_First(Qualifier_Name As String, Feat As FeatureItem) As String
                    If Feat.Qualifiers.ContainsKey(Qualifier_Name) = True Then
                        Return Feat.Qualifiers(Qualifier_Name).First
                    Else
                        Return String.Empty
                    End If
                End Function

                Public Shared Function Get_ByQualifier_All(Qualifier_Name As String, feat As FeatureItem) As List(Of String)
                    If feat.Qualifiers.ContainsKey(Qualifier_Name) = True Then
                        Return feat.Qualifiers(Qualifier_Name).ToList
                    Else
                        Return New List(Of String)
                    End If
                End Function

                Friend Shared Function Get_Feature_ByLocusTag(cDss As IEnumerable(Of FeatureItem), locus_Tag As String) As Object
                    Throw New NotImplementedException()
                End Function

                Public Shared Function Get_Feature_Types(cSeqs As List(Of ISequence)) As List(Of String)
                    Dim out As New List(Of String)
                    For Each Seq In cSeqs
                        out.AddRange(Get_Feature_Types(Seq))
                    Next
                    Return out.Distinct.ToList
                End Function
                Public Shared Function Get_Feature_Types(Seq As ISequence) As List(Of String)
                    Dim Md = Szunyi.Features.FeatureManipulation.GenBankMetaDataManipulation.GetGenBankMetaDataFromSeq(Seq)
                    If IsNothing(Md) = True Then Return New List(Of String)
                    Dim out = (From x In Md.Features.All Select x.Key)
                    If out.Count = 0 Then Return New List(Of String)
                    Return out.Distinct.ToList

                End Function

                Public Shared Function Get_Feature_Near_5_Prime_End(cmRNA As FeatureItem, cDSs As List(Of FeatureItem)) As FeatureItem
                    If cmRNA.Location.IsComplementer = False Then
                        Dim res = From x In cDSs Where x.Location.LocationStart >= cmRNA.Location.LocationStart And x.Location.LocationEnd <= cmRNA.Location.LocationEnd And x.Location.IsComplementer = False Order By x.Location.LocationStart Ascending

                        If res.Count > 0 Then Return res.First

                        Return Nothing
                    Else
                        Dim res = From x In cDSs Where x.Location.LocationStart >= cmRNA.Location.LocationStart And x.Location.LocationEnd <= cmRNA.Location.LocationEnd And x.Location.IsComplementer = True Order By x.Location.LocationEnd Descending

                        If res.Count > 0 Then Return res.First

                        Return Nothing
                    End If
                End Function

                Public Shared Function Get_Five_UTRwCDS(cmRNA As FeatureItem, cCDS As FeatureItem) As FeatureItem
                    Dim l As Bio.IO.GenBank.ILocation
                    If cmRNA.Location.IsComplementer = False Then
                        l = Szunyi.Location.Common.GetLocation(cmRNA.Location.LocationStart, cCDS.Location.LocationEnd, False)

                    Else
                        l = Szunyi.Location.Common.GetLocation(cmRNA.Location.LocationEnd, cCDS.Location.LocationStart, True)
                    End If
                    Return New FeatureItem("tmp", l)
                End Function

                Public Shared Function Get_Five_UTR(cmRNA As FeatureItem, cCDS As FeatureItem) As FeatureItem
                    Dim l As Bio.IO.GenBank.ILocation
                    If cmRNA.Location.IsComplementer = False Then
                        l = Szunyi.Location.Common.GetLocation(cmRNA.Location.LocationStart, cCDS.Location.LocationStart, False)

                    Else
                        l = Szunyi.Location.Common.GetLocation(cmRNA.Location.LocationEnd, cCDS.Location.LocationEnd, True)
                    End If
                    Return New FeatureItem("tmp", l)
                End Function

                Public Shared Sub Set_Features_Location_accession(Seqs As List(Of ISequence))
                    For Each seq In Seqs
                        For Each Feat In Szunyi.Features.FeatureManipulation.GetFeaturesByType.Get_All_Features(seq)
                            Feat.Location.Accession = seq.ID
                        Next

                    Next
                End Sub

                Public Shared Function Correct_Location(seqs As List(Of ISequence)) As List(Of ISequence)
                    For Each Seq In seqs
                        Dim Feats = Szunyi.Features.FeatureManipulation.GetFeaturesByType.Get_All_Features(Seq)
                        For Each Feat In Feats
                            Dim Loci = Feat.Location
                            If Feat.Location.Operator = LocationOperator.Join AndAlso Feat.Location.SubLocations.Count > 0 AndAlso Feat.Location.SubLocations.First.Operator = LocationOperator.Complement Then
                                Dim kj As Int16 = 54
                                Dim s = Szunyi.Location.Common.GetLocationString(Feat)
                                Dim s1 = s.Replace("complement", "")
                                Dim s2 = s1.Replace("join", "complement(join")
                                Dim s3 = s2.Replace("((", "(")
                                Dim ret = Regex.Split(s3, "[^0-9]")
                                Dim N As New List(Of Integer)
                                For Each item In ret
                                    If item <> String.Empty Then
                                        N.Add(item)
                                    End If
                                Next
                                N.Sort()

                                Dim str As New System.Text.StringBuilder
                                str.Append("complement(join(")
                                For i1 = 0 To N.Count - 1 Step 2
                                    str.Append(N(i1)).Append("..").Append(N(i1 + 1)).Append(",")
                                    Dim h As Int16 = 54
                                Next
                                str.Length -= 1
                                str.Append("))")
                                Dim loc = Szunyi.Location.Common.Get_Location(str.ToString)
                                Feat.Location = loc
                            End If
                        Next
                    Next
                    Return seqs
                End Function
            End Class

            Public Class GetLocations
                Public Shared Property LociBuilder As New Bio.IO.GenBank.LocationBuilder
                Public Shared Function Get_Full_Locations(Feats As List(Of FeatureItem)) As List(Of FeatureItem)
                    Dim out As New List(Of FeatureItem)
                    If IsNothing(Feats) = False Then
                        For Each Feat In Feats
                            out.Add(Get_Full_Location(Feat))
                        Next
                    End If

                    Return out
                End Function
                Public Shared Function Get_Full_Location(Feat As FeatureItem) As FeatureItem

                    Dim loci = Szunyi.Location.Common.Get_Full_Location(Feat.Location)

                    Dim x As New FeatureItem(Feat.Key, loci)
                    Szunyi.Features.FeatureManipulation.MergeFeatures.Merge2Features(Feat, x, True)
                    Return x
                End Function
                Public Shared Function Get_Location_Strings(Feats As List(Of FeatureItem)) As List(Of String)
                    Dim out As New List(Of String)
                    If IsNothing(Feats) OrElse Feats.Count = 0 Then Return out
                    For Each Feat In Feats
                        out.Add(LociBuilder.GetLocationString(Feat.Location))
                    Next
                    Return out
                End Function
                Public Shared Function Get_Location_String(Feat As FeatureItem) As String
                    Return (LociBuilder.GetLocationString(Feat.Location))

                End Function
                Public Shared Function FromPositions(Positions As List(Of Integer), Optional IsReverese As Boolean = False) As Bio.IO.GenBank.Location
                    Dim s As String = ""
                    For i1 = 0 To Positions.Count - 2 Step 2
                        s = s & Positions(i1) & ".." & Positions(i1 + 1) & ","
                    Next
                    s = s.TrimEnd(",")
                    If Positions.Count > 2 Then
                        s = "join(" & s & ")"
                    End If
                    If IsReverese = True Then
                        s = "complement(" & s & ")"
                    End If
                    Dim loci = LociBuilder.GetLocation(s)
                    Return loci
                End Function
                Public Shared Function GetBiggest(Features As List(Of FeatureItem), FeatureType As String) As Object
                    Dim min = (From x In Features Select x.Location.LocationStart).Min
                    Dim max = (From x In Features Select x.Location.LocationEnd).Max
                    Dim x1 As Bio.IO.GenBank.Location = GetNewLocation(Features.First.Location, min, max)
                    Select Case FeatureType
                        Case StandardFeatureKeys.CodingSequence
                            Dim res As New CodingSequence(x1)
                            Szunyi.Features.FeatureManipulation.MergeFeatures.Merge2Features(Features.First, res, True)
                            Return res
                        Case StandardFeatureKeys.MessengerRna
                            Dim res As New MessengerRna(x1)
                            Szunyi.Features.FeatureManipulation.MergeFeatures.Merge2Features(Features.First, res, True)
                            Return res
                        Case StandardFeatureKeys.Gene
                            Dim res As New Gene(x1)
                            Szunyi.Features.FeatureManipulation.MergeFeatures.Merge2Features(Features.First, res, True)
                            Return res
                    End Select
                    Return Nothing

                End Function
                Public Shared Function IsSame(Features As List(Of FeatureItem)) As Boolean

                    Dim refLocation = LociBuilder.GetLocationString(Features.First.Location)
                    For Each Feat In Features
                        If LociBuilder.GetLocationString(Feat.Location) <> refLocation Then Return False
                    Next
                    Return True
                End Function
                Public Shared Function GetGeneLocationFromCDS(CDSLocation As Bio.IO.GenBank.Location) As Bio.IO.GenBank.Location
                    Dim s As String = ""
                    If CDSLocation.Operator = LocationOperator.Complement Then
                        s = "complement(" & CDSLocation.LocationStart & ".." & CDSLocation.LocationEnd & ")"
                    Else
                        s = CDSLocation.LocationStart & ".." & CDSLocation.LocationEnd
                    End If
                    Return LociBuilder.GetLocation(s)
                End Function
                Public Shared Function GetSubLocations(location As Bio.IO.GenBank.Location, Sublocations As List(Of Bio.IO.GenBank.Location)) _
                    As List(Of Bio.IO.GenBank.Location)

                    If location.SubLocations.Count = 0 Then
                        Sublocations.Add(location)

                    Else
                        For Each Item In location.SubLocations
                            Sublocations = GetSubLocations(Item, Sublocations)
                        Next
                    End If
                    Return Sublocations
                End Function
                ''' <summary>
                ''' 
                ''' </summary>
                ''' <param name="feature"></param>
                ''' <returns></returns>
                Public Shared Function GetSubSequences(feature As ExtFeature) As List(Of ISequence)
                    Dim SubLocations = GetLocations.GetSubLocations(feature.Feature.Location, New List(Of Bio.IO.GenBank.Location))
                    Dim out As New List(Of Bio.ISequence)
                    For Each SubLocation In SubLocations
                        out.Add(feature.Seq.GetSubSequence(SubLocation.LocationStart - 1, SubLocation.LocationEnd - SubLocation.LocationStart - 1))

                    Next
                    Return out
                End Function

                Public Shared Function GetSubSequencesAsString(feature As ExtFeature) As List(Of String)
                    Dim Seqs = GetSubSequences(feature)
                    Return Szunyi.Sequences.SequenceManipulation.SeqsToString.GetSeqsAsString(Seqs)
                End Function

                Public Shared Function GetNewLocationsReverse(Location As Bio.IO.GenBank.Location, Length As Integer) As Bio.IO.GenBank.Location
                    Dim x As New Bio.IO.GenBank.LocationBuilder
                    Dim tmp = Length - Location.LocationEnd + 1 & ".." &
                         Length - Location.LocationStart + 1

                    Dim newLoc = x.GetLocation(tmp)
                    If Location.Operator <> LocationOperator.Complement Then newLoc.Operator = Location.Operator
                    For i1 = Location.SubLocations.Count - 1 To 0 Step -1
                        Dim Loci = Location.SubLocations(i1)
                        newLoc.SubLocations.Add(GetNewLocationsReverse(Loci, Length))
                    Next
                    Return newLoc
                End Function
                Public Shared Function GetNewLocation(Location As Bio.IO.GenBank.Location, locationStart As Integer) As Bio.IO.GenBank.Location
                    Dim x As New Bio.IO.GenBank.LocationBuilder
                    Dim tmp = Location.LocationStart - locationStart + 1 & ".." &
                        Location.LocationEnd - locationStart + 1

                    Dim newLoc = x.GetLocation(tmp)
                    newLoc.Operator = Location.Operator
                    For Each Loci As Bio.IO.GenBank.Location In Location.SubLocations
                        newLoc.SubLocations.Add(GetNewLocation(Loci, locationStart))
                    Next
                    Return newLoc
                End Function
                Public Shared Function GetNewLocation(Location As Bio.IO.GenBank.ILocation) As Bio.IO.GenBank.ILocation
                    Dim x As New Bio.IO.GenBank.LocationBuilder
                    If IsNothing(Location) = True Then Return Nothing
                    Dim tmp = x.GetLocationString(Location)
                    Dim newLoc = x.GetLocation(tmp)
                    newLoc.Operator = Location.Operator

                    Return newLoc
                End Function
                Public Shared Function GetNewLocation(Location As Bio.IO.GenBank.Location, locationStart As Integer, LocationEnd As Integer) As Bio.IO.GenBank.Location
                    Dim x As New Bio.IO.GenBank.LocationBuilder
                    If Location.LocationStart < locationStart Then locationStart = Location.LocationStart
                    If LocationEnd < Location.LocationEnd Then LocationEnd = Location.LocationEnd
                    Dim tmp As String = ""
                    If Location.Operator = LocationOperator.Complement Then
                        tmp = "complement(" & locationStart & ".." & LocationEnd & ")"
                    Else
                        tmp = locationStart & ".." & LocationEnd
                    End If

                    Return LociBuilder.GetLocation(tmp)
                End Function
                Public Shared Function GetIntrons_From_Features(Feats As List(Of FeatureItem)) As List(Of FeatureItem)
                    Dim out As New List(Of FeatureItem)
                    For Each Feat In Feats

                        Dim exons = Get_All_Exon_Location(Feat)
                        Dim Introns = Get_All_Intron_Location(exons)
                        Dim Loci As Bio.IO.GenBank.ILocation = Szunyi.Location.Merging.MergeLocations(Introns, 1, Constants.Sort_Locations_By.TSS_PAS, 1)
                        If IsNothing(Loci) = False Then
                            Dim i As New FeatureItem(StandardFeatureKeys.Intron, Loci)
                            out.Add(MergeFeatures.Merge2Features(Feat, i, True))
                        End If
                    Next
                    Return out
                End Function
                Public Shared Function GetIntrons_From_Features(Locis As List(Of ILocation)) As List(Of ILocation)
                    Dim out As New List(Of ILocation)
                    For Each Feat In Locis

                        Dim exons = Get_All_Exon_Location(Feat)
                        Dim Introns = Get_All_Intron_Location(exons)
                        Dim Loci As Bio.IO.GenBank.ILocation = Szunyi.Location.Merging.MergeLocations(Introns, 1, Constants.Sort_Locations_By.TSS_PAS, 1)
                        If IsNothing(Loci) = False Then
                            Dim i As New FeatureItem(StandardFeatureKeys.Intron, Loci)
                            Loci.Accession = Feat.Accession
                            If IsNothing(Loci.Accession) = True OrElse Loci.Accession = "" Then
                                Dim alf As Int16 = 54
                            End If
                            out.Add(Loci)
                        End If
                    Next
                    Return out
                End Function
                Public Shared Function Get_All_Introns_Location(locis As List(Of Bio.IO.GenBank.ILocation)) As List(Of ILocation)
                    Dim out As New List(Of Bio.IO.GenBank.ILocation)
                    For Each Loci In locis
                        Dim Exons = Get_All_Exon_Location(Loci)
                        out.AddRange(Get_All_Intron_Location(Exons))
                    Next
                    Return out
                End Function
                Public Shared Function Get_All_Introns_Location(loci As Bio.IO.GenBank.ILocation) As List(Of ILocation)
                    Dim Exons = Get_All_Exon_Location(loci)
                    Return Get_All_Intron_Location(Exons)

                End Function
                Public Shared Function Get_All_Intron_Location(Exons As List(Of ILocation)) As List(Of ILocation)
                    If Exons.Count = 0 Then Return New List(Of ILocation)
                    Dim out As New List(Of ILocation)
                    For i1 = 0 To Exons.Count - 2
                        If Exons(i1).LocationEnd > Exons(i1 + 1).LocationStart Then
                            Dim ald As Int16 = 54
                        End If
                        If Exons(i1).Operator = LocationOperator.Complement Then
                            out.Add(LociBuilder.GetLocation("complement(" & Exons(i1).LocationEnd + 1 & ".." & Exons(i1 + 1).LocationStart - 1 & ")"))
                            out.Last.Accession = Exons.First.Accession
                        Else
                            out.Add(LociBuilder.GetLocation(Exons(i1).LocationEnd + 1 & ".." & Exons(i1 + 1).LocationStart - 1))
                            out.Last.Accession = Exons.First.Accession
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
                Public Shared Function Get_All_Exon_Location(Feat As Bio.IO.GenBank.FeatureItem) As List(Of Bio.IO.GenBank.ILocation)
                    If IsNothing(Feat) = True Then Return Nothing
                    Dim loci = Feat.Location
                    Dim out As New List(Of ILocation)
                    If loci.SubLocations.Count = 0 Then ' No complement no join
                        out.Add(loci)
                        out.Last.Accession = loci.Accession
                        Return out
                    End If
                    If loci.Operator <> LocationOperator.Complement Then ' no complement join
                        For Each subL In loci.SubLocations
                            subL.Accession = loci.Accession
                        Next
                        Return loci.SubLocations
                    End If
                    If loci.SubLocations.First.Operator = LocationOperator.Join Then ' complement join
                        For Each subloci As Bio.IO.GenBank.Location In loci.SubLocations.First.SubLocations
                            Dim s = "complement(" & LociBuilder.GetLocationString(subloci) & ")"
                            out.Add(LociBuilder.GetLocation(s))
                            out.Last.Accession = loci.Accession
                        Next
                        Return out
                    Else ' complement no join
                        out.Add(loci)
                        out.Last.Accession = loci.Accession
                        Return out
                    End If

                End Function
                Public Shared Function GetCDSExonsLocations(Feat As FeatureItem) As List(Of Bio.IO.GenBank.ILocation)
                    Dim out As New List(Of ILocation)
                    If Feat.Location.SubLocations.Count = 0 Then ' No complement no join
                        out.Add(Feat.Location)
                        Return out
                    End If
                    If Feat.Location.Operator <> LocationOperator.Complement Then ' no complement join
                        Return Feat.Location.SubLocations
                    End If
                    If Feat.Location.SubLocations.First.Operator = LocationOperator.Join Then ' complement join
                        For Each loci As Bio.IO.GenBank.Location In Feat.Location.SubLocations.First.SubLocations
                            Dim s = "complement(" & LociBuilder.GetLocationString(loci) & ")"
                            out.Add(LociBuilder.GetLocation(s))
                        Next
                        Return out
                    Else ' complement no join
                        out.Add(Feat.Location)
                        Return out
                    End If


                End Function
                Public Shared Function Get_Last_Exons_Location(locis As List(Of ILocation)) As List(Of ILocation)
                    Dim out As New List(Of Bio.IO.GenBank.ILocation)
                    For Each Loci In locis
                        out.Add(Get_Last_Exon_Location(Loci))
                    Next
                    Return out
                End Function
                Public Shared Function Get_xth_Exons_Location(locis As List(Of ILocation), xth As Integer) As List(Of ILocation)
                    Dim out As New List(Of Bio.IO.GenBank.ILocation)
                    For Each Loci In locis
                        out.Add(Get_XTH_Exon_Location(Loci, xth))
                    Next
                    Return out
                End Function
                Public Shared Function Get_XTH_Exon_Location(loci As ILocation, xth As Integer) As ILocation
                    If loci.Operator = LocationOperator.Complement Then
                        Return loci.SubLocations.First.SubLocations(xth)
                    ElseIf loci.Operator = LocationOperator.Join Then
                        Return loci.SubLocations(xth)

                    Else 'no complement join
                        Return loci.SubLocations(xth)
                    End If
                End Function
                Public Shared Function Get_Last_Exon_Location(loci As ILocation) As ILocation
                    Dim out As New List(Of Bio.IO.GenBank.ILocation)
                    If loci.SubLocations.Count = 0 Then ' No complement no join
                        Return loci
                    ElseIf loci.Operator = LocationOperator.Complement Then
                        If loci.SubLocations.First.Operator <> LocationOperator.Join Then
                            Dim t = loci.Clone
                            t.Operator = LocationOperator.Complement
                            Return t
                        Else
                            Dim t = loci.SubLocations.First.SubLocations.First.Clone
                            t.Operator = LocationOperator.Complement
                            Return t
                        End If

                    Else 'no complement join
                        Return loci.SubLocations.Last
                    End If
                    Return out
                End Function

                ''' <summary>
                ''' Depends on location operators
                ''' </summary>
                ''' <param name="Locis"></param>
                ''' <returns></returns>
                Public Shared Function Get_First_Exons_Location(Locis As List(Of Bio.IO.GenBank.ILocation)) As List(Of Bio.IO.GenBank.ILocation)
                    Dim out As New List(Of Bio.IO.GenBank.ILocation)
                    For Each Loci In Locis
                        out.Add(Get_First_Exon_Location(Loci))
                    Next
                    Return out
                End Function
                ''' <summary>
                ''' Depends on location operator
                ''' </summary>
                ''' <param name="loci"></param>
                ''' <returns></returns>
                Public Shared Function Get_First_Exon_Location(loci As Bio.IO.GenBank.Location) As Bio.IO.GenBank.Location
                    If loci.SubLocations.Count = 0 Then ' No complement no join
                        Return loci
                    ElseIf loci.Operator = LocationOperator.Complement Then
                        If loci.SubLocations.First.Operator <> LocationOperator.Join Then
                            Dim t = loci.Clone
                            t.Operator = LocationOperator.Complement
                            Return t
                        Else
                            Dim t = loci.SubLocations.First.SubLocations.Last.Clone
                            t.Operator = LocationOperator.Complement
                            t.Accession = loci.Accession
                            Return t
                        End If

                    Else 'no complement join
                        Dim t = loci.SubLocations.First.Clone
                        t.Accession = loci.Accession
                        Return t
                    End If
                End Function
                Public Shared Function GetCDSExonsLocations(Loci As Bio.IO.GenBank.Location) As List(Of Bio.IO.GenBank.ILocation)
                    Dim out As New List(Of ILocation)
                    If Loci.SubLocations.Count = 0 Then ' No complement no join
                        out.Add(Loci)
                        Return out
                    End If
                    If Loci.Operator <> LocationOperator.Complement Then ' no complement join
                        Return Loci.SubLocations
                    End If
                    If Loci.SubLocations.First.Operator = LocationOperator.Join Then ' complement join
                        For Each subloci As Bio.IO.GenBank.Location In Loci.SubLocations.First.SubLocations
                            Dim s = "complement(" & LociBuilder.GetLocationString(subloci) & ")"
                            out.Add(LociBuilder.GetLocation(s))
                        Next
                        Return out
                    Else ' complement no join
                        out.Add(Loci)
                        Return out
                    End If


                End Function
                Public Shared Function GetIntronLocations(Feat As FeatureItem) As List(Of ILocation)
                    Dim Exons = GetCDSExonsLocations(Feat)
                    Return GetIntronLocationsFromExonLOcations(Exons)
                End Function

                Public Shared Function GetIntronLocationsFromExonLOcations(Ls As List(Of Bio.IO.GenBank.ILocation)) As List(Of Bio.IO.GenBank.ILocation)
                    If Ls.Count = 0 Then Return New List(Of ILocation)
                    Dim out As New List(Of ILocation)
                    For i1 = 0 To Ls.Count - 2
                        If Ls(i1).LocationEnd > Ls(i1 + 1).LocationStart Then
                            out.Add(LociBuilder.GetLocation(Ls(i1 + 1).LocationEnd + 1 & ".." & Ls(i1).LocationStart - 1))
                        Else
                            out.Add(LociBuilder.GetLocation(Ls(i1).LocationEnd + 1 & ".." & Ls(i1 + 1).LocationStart - 1))
                        End If
                        If Ls.First.IsComplementer = True Then
                            out(out.Count - 1) = Change_Strand(out.Last)
                        End If
                    Next
                    Return out
                End Function
                ''' <summary>
                ''' Return Start And End Without Checking and WithOut Orintation
                ''' </summary>
                ''' <param name="CDS"></param>
                ''' <param name="length"></param>
                ''' <returns></returns>
                Public Shared Function Get_Up_Down_With_Orintation(CDS As ExtFeature, TSS_FivePrimeLength As Integer, TSS3PrimeLength As Integer) As Bio.IO.GenBank.Location
                    If CDS.Feature.Location.Operator = LocationOperator.Complement Then
                        Dim sData As Integer = CDS.Feature.Location.LocationEnd + 1 - TSS3PrimeLength
                        Dim eData As Integer = CDS.Feature.Location.LocationEnd + 1 + TSS_FivePrimeLength
                        If eData > CDS.Seq.Count Then
                            eData = CDS.Seq.Count
                        End If
                        If sData < 0 Then
                            sData = 1
                        End If
                        Return LociBuilder.GetLocation("complement(" & sData & ".." & eData & ")")

                    Else
                        Dim sData As Integer = CDS.Feature.Location.LocationStart - TSS_FivePrimeLength - 1
                        Dim eData As Integer = CDS.Feature.Location.LocationStart - 1 + TSS3PrimeLength
                        If eData > CDS.Seq.Count Then
                            eData = CDS.Seq.Count
                        End If
                        If sData < 0 Then
                            sData = 1
                        End If
                        Return LociBuilder.GetLocation(sData & ".." & eData)

                    End If
                End Function
                ''' <summary>
                ''' Return Start And End Without Checking and WithOut Orintation
                ''' </summary>
                ''' <param name="CDS"></param>
                ''' <param name="length"></param>
                ''' <returns></returns>
                Public Shared Function GetPromoterLocationFromCDS_With_Orintation(CDS As ExtFeature, Length As Integer) As Bio.IO.GenBank.Location
                    If CDS.Feature.Location.Operator = LocationOperator.Complement Then
                        Dim sData As Integer = CDS.Feature.Location.LocationEnd + 1
                        Dim eData As Integer = CDS.Feature.Location.LocationEnd + 1 + Length
                        If eData > CDS.Seq.Count Then
                            eData = CDS.Seq.Count
                        End If
                        If sData < 0 Then
                            sData = 1
                        End If
                        Return LociBuilder.GetLocation("complement(" & sData & ".." & eData & ")")

                    Else
                        Dim sData As Integer = CDS.Feature.Location.LocationStart - Length - 1
                        Dim eData As Integer = CDS.Feature.Location.LocationStart - 1
                        If eData > CDS.Seq.Count Then
                            eData = CDS.Seq.Count
                        End If
                        If sData < 0 Then
                            sData = 1
                        End If
                        Return LociBuilder.GetLocation(sData & ".." & eData)

                    End If
                End Function
                ''' <summary>
                ''' Return Start And End Without Checking and WithOut Orintation
                ''' </summary>
                ''' <param name="CDS"></param>
                ''' <param name="length"></param>
                ''' <returns></returns>
                Public Shared Function GetGenomicLocationFromCDS_With_Orintation(CDS As ExtFeature, Length As Integer) As Bio.IO.GenBank.Location
                    If CDS.Feature.Location.Operator = LocationOperator.Complement Then
                        Dim sData As Integer = CDS.Feature.Location.LocationStart + 1 - Length
                        Dim eData As Integer = CDS.Feature.Location.LocationEnd + 1 + Length
                        Return LociBuilder.GetLocation("complement(" & sData & ".." & eData & ")")

                    Else
                        Dim sData As Integer = CDS.Feature.Location.LocationStart - Length - 1
                        Dim eData As Integer = CDS.Feature.Location.LocationEnd + Length - 1
                        Return LociBuilder.GetLocation(sData & ".." & eData)

                    End If
                End Function

                ''' <summary>
                ''' Return Start And End Without Checking and WithOut Orintation
                ''' </summary>
                ''' <param name="CDS"></param>
                ''' <param name="length"></param>
                ''' <returns></returns>
                Public Shared Function GetUTRLocationFromCDS_With_Orintation(CDS As ExtFeature, length As Integer) As Bio.IO.GenBank.Location
                    If CDS.Feature.Location.Operator <> LocationOperator.Complement Then
                        Dim sData As Integer = CDS.Feature.Location.LocationEnd + 1
                        Dim eData As Integer = CDS.Feature.Location.LocationEnd + 1 + length
                        Return LociBuilder.GetLocation("complement(" & sData & ".." & eData & ")")

                    Else
                        Dim sData As Integer = CDS.Feature.Location.LocationStart - length - 1
                        Dim eData As Integer = CDS.Feature.Location.LocationStart - 1
                        Return LociBuilder.GetLocation(sData & ".." & eData)

                    End If
                End Function

                Public Shared Function CorrectStandAndEnd(mRNA As FeatureItem, gene As FeatureItem) As FeatureItem
                    Dim Exons = Szunyi.Features.FeatureManipulation.GetLocations.GetCDSExonsLocations(mRNA)
                    Dim FirstExonLoc = LociBuilder.GetLocation(gene.Location.LocationStart & ".." & Exons.First.LocationEnd)
                    FirstExonLoc.Operator = mRNA.Location.Operator
                    Exons(0) = FirstExonLoc
                    Dim LastExonLoc = LociBuilder.GetLocation(Exons.Last.LocationStart & ".." & gene.Location.LocationEnd)
                    LastExonLoc.Operator = mRNA.Location.Operator
                    Exons(Exons.Count - 1) = LastExonLoc
                    Dim str As New System.Text.StringBuilder
                    If mRNA.Location.Operator = LocationOperator.Complement Then
                        str.Append("complement(")
                        If Exons.Count > 1 Then
                            str.Append("join(")
                            For Each Loci In Exons
                                str.Append(Loci.LocationStart).Append("..").Append(Loci.LocationEnd).Append(",")
                            Next
                            str.Length -= 1
                            str.Append(")")
                        Else
                            str.Append(Exons.First.LocationStart).Append("..").Append(Exons.First.LocationEnd)
                        End If
                        str.Append(")")
                    Else
                        If Exons.Count > 1 Then
                            str.Append("join(")
                            For Each Loci In Exons
                                str.Append(Loci.LocationStart).Append("..").Append(Loci.LocationEnd).Append(",")
                            Next
                            str.Length -= 1
                            str.Append(")")
                        Else
                            str.Append(Exons.First.LocationStart).Append("..").Append(Exons.First.LocationEnd)
                        End If
                    End If
                    Try
                        mRNA.Location = LociBuilder.GetLocation(str.ToString)
                    Catch ex As Exception
                        Dim alf As Int16 = 65
                    End Try

                    Return mRNA
                End Function

                Public Shared Function Change_Strand(gExons As Bio.IO.GenBank.Location) As Bio.IO.GenBank.Location
                    Dim s = LociBuilder.GetLocationString(gExons)
                    If s.Contains("complement") Then
                        s = s.Replace("complement", "")
                        s = Mid(s, 2, s.Length - 2)
                        Dim jk = LociBuilder.GetLocation(s)
                        jk.Accession = gExons.Accession
                        Return jk
                    Else
                        If gExons.SubLocations.Count > 0 AndAlso gExons.Operator = LocationOperator.None Then gExons.Operator = LocationOperator.Join
                        s = LociBuilder.GetLocationString(gExons)
                        s = "complement(" & s & ")"
                    End If
                    Dim nLOC = LociBuilder.GetLocation(s)
                    nLOC.Accession = gExons.Accession
                    Return nLOC
                End Function

                Public Shared Function Get_Only_Positive_Strands(Locis As List(Of ILocation)) As List(Of ILocation)
                    Dim out As New List(Of ILocation)
                    For Each Loci In Locis
                        If Loci.Operator <> LocationOperator.Complement Then
                            out.Add(Loci)
                        End If
                    Next
                    Return out
                End Function

                Public Shared Function Get_Only_Negative_Strands(Locis As List(Of ILocation)) As List(Of ILocation)
                    Dim out As New List(Of ILocation)
                    For Each Loci In Locis
                        If Loci.Operator = LocationOperator.Complement Then
                            out.Add(Loci)
                        End If
                    Next
                    Return out
                End Function
            End Class


            Public Class MergeFeatureAnnotation
                Private featuresFrom As List(Of String)
                Private featuresTo As List(Of String)
                Public Property Type As String = Szunyi.Constants.BackGroundWork.ModyfiedSequence
                Public Property Result As Szunyi.ListOf.SequenceList
                Public Property SeqLists As List(Of Szunyi.ListOf.SequenceList)

                Public Sub New(mergedSeqList As List(Of Szunyi.ListOf.SequenceList),
                               featuresTo As List(Of String),
                               featuresFrom As List(Of String))
                    Me.SeqLists = mergedSeqList
                    Me.featuresTo = featuresTo
                    Me.featuresFrom = featuresFrom
                End Sub
                Public Sub DoIt()
                    ' Get the two ExtFeatureList

                    Dim cSearchSetting = New SettingForSearchInFeaturesAndQulifiers(featuresTo)

                    Dim t As New Szunyi.ListOf.ExtFeatureList(cSearchSetting, Me.SeqLists)
                    t.DoIt()

                    Dim cSearchSetting2 = New SettingForSearchInFeaturesAndQulifiers(featuresFrom)
                    Dim t2 As New Szunyi.ListOf.ExtFeatureList(cSearchSetting2, Me.SeqLists)
                    t2.DoIt()


                    For Each Feat In t.FetauresByLocustag
                        Szunyi.Features.FeatureManipulation.MergeFeatures.FindAndMerge2Features(Feat, t2)
                    Next

                End Sub
            End Class

            Public Class Rename
                Private SubType As String
                Private mergedSeqs As List(Of Bio.ISequence)
                Private selectedFeatures As List(Of String)
                Private selectedQualifiers As List(Of String)
                Private separator As String
                Private Name As String
                Public Property Type As String = Szunyi.Constants.BackGroundWork.ModyfiedSequence
                Public Property SeqList As Szunyi.ListOf.SequenceList

                Public Sub New(mergedSeqs As List(Of Bio.ISequence),
                               selectedFeatures As List(Of String),
                               selectedQualifiers As List(Of String),
                               separator As String,
                               SubType As String,
                               Name As String)
                    Me.mergedSeqs = mergedSeqs
                    Me.selectedFeatures = selectedFeatures
                    Me.selectedQualifiers = selectedQualifiers
                    Me.separator = separator
                    Me.SubType = SubType
                    Dim s = InputBox("Enter the Title")
                    Me.SeqList = New Szunyi.ListOf.SequenceList(mergedSeqs, s, "RN Features: " & Name)

                End Sub
                Public Sub DoIt()
                    Dim Features = GetFeaturesByType.GetFeturesByTypesFromSeqs(mergedSeqs, selectedFeatures)
                    Select Case Me.SubType
                        Case Szunyi.Constants.StringRename.FirstAfterSplit
                            For Each Feat In Features
                                For Each Qual In selectedQualifiers
                                    If Feat.Qualifiers.ContainsKey(Qual) AndAlso Feat.Qualifiers(Qual).Count > 0 Then
                                        Dim x As New List(Of String)
                                        x.Add(Split(Feat.Qualifiers(Qual).First, Me.separator).First)
                                        Feat.Qualifiers(Qual) = x
                                    End If
                                Next
                            Next
                        Case Szunyi.Constants.StringRename.LastAfterSplit
                            For Each Feat In Features
                                For Each Qual In selectedQualifiers
                                    If Feat.Qualifiers.ContainsKey(Qual) AndAlso Feat.Qualifiers(Qual).Count > 0 Then
                                        Dim x As New List(Of String)
                                        x.Add(Split(Feat.Qualifiers(Qual).First, Me.separator).Last)
                                        Feat.Qualifiers(Qual) = x
                                    End If
                                Next
                            Next
                    End Select

                End Sub

            End Class

            Public Class UniqueDistict
                Public Shared Property LociBuilder As New LocationBuilder

                Public Shared Function Get_Same_Features_By_Locations(First As List(Of FeatureItem), Second As List(Of FeatureItem)) As List(Of FeatureItem)
                    Dim out As New List(Of FeatureItem)
                    For Each f In First
                        Dim Sames = From x In Second Where f.Location.LocationStart = x.Location.LocationStart And
                                                         f.Location.LocationEnd = x.Location.LocationEnd And
                                                         f.Location.IsComplementer = x.Location.IsComplementer

                        If Sames.Count > 0 Then
                            out.Add(f)
                        End If
                    Next
                    Return out
                End Function
                Public Shared Function Get_Uniqe_Features_By_Locations(First As List(Of FeatureItem), Second As List(Of FeatureItem)) As List(Of FeatureItem)
                    Dim out As New List(Of FeatureItem)
                    For Each f In First
                        Dim Sames = From x In Second Where f.Location.LocationStart = x.Location.LocationStart And
                                                         f.Location.LocationEnd = x.Location.LocationEnd And
                                                         f.Location.IsComplementer = x.Location.IsComplementer

                        If Sames.Count = 0 Then
                            out.Add(f)
                        End If
                    Next
                    Return out
                End Function
                Public Shared Function Get_Features_Has_Full_LocusTag_From_Short_LocusTag(Features As List(Of FeatureItem), LocusTag As String) As List(Of FeatureItem)
                    Dim Out As New List(Of FeatureItem)
                    If IsNothing(Features) = True Then Return Out
                    Dim tmp As New FeatureItem("A", "1..1")
                    tmp.Qualifiers(StandardQualifierNames.LocusTag) = New List(Of String)
                    tmp.Qualifiers(StandardQualifierNames.LocusTag).Add("tmp")
                    Dim Index As Integer = 1

                    Do
                        tmp.Qualifiers(StandardQualifierNames.LocusTag)(0) = LocusTag & "." & Index
                        Index += 1
                        Dim IndexOfFeat = Features.BinarySearch(tmp, Szunyi.Comparares.AllComparares.ByLocusTag)
                        If IndexOfFeat > -1 Then
                            Out.Add(Features(IndexOfFeat))
                        Else
                            Return Out
                        End If
                    Loop
                End Function

                Public Shared Iterator Function GetDuplicatedFeaturesByLocusTagAndLocation(Features As List(Of FeatureItem)) As IEnumerable(Of List(Of FeatureItem))
                    Dim LociBuilder As New LocationBuilder
                    If IsNothing(Features) = False Then
                        Dim t = From x In Features Select x Group By Loc = LociBuilder.GetLocationString(x.Location) & x.Qualifiers(StandardQualifierNames.LocusTag).First Into Group
                        For Each g In t
                            If g.Group.Count > 1 Then
                                Yield g.Group.ToList
                            End If

                        Next
                    End If


                End Function
                Public Shared Iterator Function GetDuplicatedFeaturesByLocusTag(Features As List(Of FeatureItem)) As IEnumerable(Of List(Of FeatureItem))
                    If IsNothing(Features) = False Then
                        Dim t = From x In Features Select x Group By x.Qualifiers(StandardQualifierNames.LocusTag).First Into Group
                        For Each g In t
                            If g.Group.Count > 1 Then
                                Yield g.Group.ToList
                            End If

                        Next
                    End If

                End Function

                Public Shared Function ByLocation(Feats As List(Of FeatureItem)) As List(Of FeatureItem)

                    Dim out As New List(Of FeatureItem)
                    Dim res = From x In Feats Group By x.Location.LocationStart, x.Location.LocationEnd, x.Location.IsComplementer Into Group

                    For Each r In res

                        out.Add(Szunyi.Features.FeatureManipulation.MergeFeatures.MergeQualifiers(r.Group.ToList))
                    Next
                    Return out
                End Function
            End Class

            Public Class Qulifiers
                Public Shared Sub Add()

                End Sub
                Public Shared Function Get_Distinct_Qulifiers(seqs As List(Of Bio.ISequence)) As List(Of String)
                    Dim Feats = GetFeaturesByType.Get_All_Features(seqs)
                    Dim out As New List(Of String)
                    For Each f In Feats
                        For Each q In f.Qualifiers
                            If out.Contains(q.Key) = False Then out.Add(q.Key)
                        Next
                    Next
                    Return out
                End Function
                Public Shared Function Get_All_Values(Feat As FeatureItem) As String
                    Dim satr As New System.Text.StringBuilder
                    For Each Q In StandardQualifierNames.All
                        If Feat.Qualifiers.ContainsKey(Q) = True AndAlso Feat.Qualifiers(Q).Count > 0 Then
                            satr.Append(Feat.Qualifiers(Q).First)
                        End If
                        satr.Append(vbTab)
                    Next
                    satr.Length -= 1
                    Return satr.ToString
                End Function
                Public Shared Function Get_Values_From_Features(Feats As List(Of FeatureItem), Type As String) As List(Of String)
                    Dim out As New List(Of String)
                    For Each Feat In Feats
                        If Feat.Qualifiers.ContainsKey(Type) Then
                            out.AddRange(Feat.Qualifiers(Type))
                        Else
                            Dim alf As Int16 = 54
                        End If
                    Next
                    Return out
                End Function
                Public Shared Function Get_Values_From_Features(Feats As List(Of FeatureItem), Types As List(Of String)) As List(Of String)
                    Dim out As New List(Of String)
                    For Each Feat In Feats
                        out.Add(Get_Values_From_Feature(Feat, Types))
                    Next
                    Return out
                End Function
                Public Shared Function Get_Values_From_Feature(Feat As FeatureItem, Type As String, Optional OnlyFirst As Boolean = False) As String
                    Dim out As New List(Of String)
                    If Feat.Qualifiers.ContainsKey(Type) Then
                        If OnlyFirst = True Then
                            out.Add(Feat.Qualifiers(Type).First)
                        Else
                            out.AddRange(Feat.Qualifiers(Type))
                        End If

                    End If
                    Return Szunyi.Text.General.GetText(out, " ")
                End Function

                Public Shared Function Get_Values_From_Feature(Feat As FeatureItem, Types As List(Of String), Optional OnlyFirst As Boolean = False) As String
                    Dim out As New List(Of String)
                    For Each Type In Types
                        If Feat.Qualifiers.ContainsKey(Type) Then
                            If OnlyFirst = True Then
                                out.Add(Feat.Qualifiers(Type).First)
                            Else
                                out.AddRange(Feat.Qualifiers(Type))
                            End If
                        End If
                    Next

                    Return Szunyi.Text.General.GetText(out, " ")
                End Function

                Public Shared Sub Rename_Qulifier_Value(Feat As FeatureItem, Key As String, original_String As String, new_String As String)
                    If Feat.Qualifiers.ContainsKey(Key) = True Then
                        Dim ls = Feat.Qualifiers(Key).ToList
                        Dim nLS As New List(Of String)
                        For Each s In ls
                            s = s.Replace(original_String, new_String)
                            nLS.Add(s)
                        Next
                        Feat.Qualifiers(Key) = nLS
                    End If
                End Sub

                Public Shared Function Get_Description(feat As FeatureItem, get_Description_Gene_Note() As String, OnlyFirst As Boolean, wKey As Boolean, wLabel As Boolean) As String
                    Dim str As New System.Text.StringBuilder
                    If wKey = True Then str.Append(feat.Key).Append(vbTab)
                    For Each item In get_Description_Gene_Note
                        If feat.Qualifiers.ContainsKey(item) AndAlso feat.Qualifiers(item).Count > 0 Then
                            str.Append(feat.Qualifiers(item).First).Append(vbTab)
                            If OnlyFirst = True Then
                                str.Length -= 1
                                Dim s1 As String = str.ToString.Replace(Chr(34), "")
                                Return s1
                                Return str.ToString
                            End If
                        End If
                    Next
                    If wLabel = True Then str.Append(feat.Label)

                    Dim s As String = str.ToString.Replace(Chr(34), "")
                    Return s
                End Function

                Friend Shared Function Get_Common_Values(feature As FeatureItem, Separator As String) As String
                    Dim str As New System.Text.StringBuilder
                    If feature.Qualifiers.ContainsKey(StandardQualifierNames.GeneSymbol) Then
                        str.Append(feature.Qualifiers(StandardQualifierNames.GeneSymbol).First).Append(",")
                    End If
                    If feature.Qualifiers.ContainsKey(StandardQualifierNames.Note) Then
                        'str.Append(feature.Qualifiers(StandardQualifierNames.Note).First).Append(",")
                    End If
                    If feature.Qualifiers.ContainsKey(StandardQualifierNames.LocusTag) Then
                        str.Append(feature.Qualifiers(StandardQualifierNames.LocusTag).First).Append(",")
                    End If
                    Dim s As String = str.ToString.Replace(Chr(34), "")
                    s = s.Replace(",,", ",").Replace(vbTab, " ").Replace(vbCrLf, " ")

                    Return s
                End Function

                Public Shared Sub Add(f As FeatureItem, Qual_Name As String, Qual_Value As String)
                    Dim ls As New List(Of String)
                    ls.Add(Qual_Value)
                    If f.Qualifiers.ContainsKey(Qual_Name) = False Then
                        f.Qualifiers.Add(Qual_Name, ls)
                    Else
                        f.Qualifiers(Qual_Name).Add(Qual_Value)
                    End If
                End Sub
                Public Shared Sub Add(f As FeatureItem, Qual_Name As String, Qual_Values As List(Of String))

                    If f.Qualifiers.ContainsKey(Qual_Name) = False Then
                        f.Qualifiers.Add(Qual_Name, Qual_Values)
                    Else
                        f.Qualifiers(Qual_Name) = Qual_Values
                    End If
                End Sub
            End Class
        End Namespace



    End Namespace
End Namespace