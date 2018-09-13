Imports System.IO
Imports Bio
Imports Bio.IO.GenBank
Imports Bio.SimilarityMatrices
Imports ClassLibrary1.Szunyi.DNA.Repeat

Namespace Szunyi
    Namespace DNA
        Public Class Common
            Public Shared Function Get_AT_Percent(Seq As Bio.ISequence) As Double
                Return (((From x In Seq Where x = Bio.Alphabets.DNA.A).Count + (From x In Seq Where x = Bio.Alphabets.DNA.T).Count) / Seq.Count) * 100


            End Function
            Public Shared Function Get_AT_Percent(Seq As Bio.ISequence, isComplenter As Boolean) As Double
                If isComplenter = False Then
                    Return ((From x In Seq Where x = Bio.Alphabets.DNA.A).Count / Seq.Count) * 100

                Else
                    Return (((From x In Seq Where x = Bio.Alphabets.DNA.T).Count) / Seq.Count) * 100

                End If


            End Function
            Public Shared Function Get_Percents(Seq As Bio.Sequence) As String
                Dim nofA = (From x In Seq Where x = Bio.Alphabets.DNA.A).Count

                Dim nofC = (From x In Seq Where x = Bio.Alphabets.DNA.C).Count

                Dim nofG = (From x In Seq Where x = Bio.Alphabets.DNA.G).Count

                Dim nofT = (From x In Seq Where x = Bio.Alphabets.DNA.T).Count

                Dim str As New System.Text.StringBuilder
                Return (nofC + nofG) / (nofA + nofC + nofG + nofT) * 100

            End Function
            Public Shared Function Get_Max_Consequitve(Seq As Bio.Sequence, b As Byte) As Integer
                Dim currByte = b
                Dim count = 0
                Dim maxCount As Integer = 0
                For i1 = 0 To Seq.Count - 1
                    If Seq(i1) = currByte Then
                        count += 1
                    Else
                        If maxCount >= count Then
                            count = 0
                        Else
                            maxCount = count
                            count = 0
                        End If
                    End If
                Next
                Return maxCount
            End Function
            Public Shared Function Get_NofA(Seq As Bio.ISequence)
                Return (From x In Seq Where x = Bio.Alphabets.DNA.A).Count
            End Function
            Public Shared Function Get_NofT(Seq As Bio.ISequence)
                Return (From x In Seq Where x = Bio.Alphabets.DNA.T).Count
            End Function
            Public Shared Function Get_Percents_All_Header() As String
                Dim str As New System.Text.StringBuilder
                str.Append("NofA").Append(vbTab)
                str.Append("NofC").Append(vbTab)
                str.Append("NofG").Append(vbTab)
                str.Append("NofT").Append(vbTab)
                str.Append("Percent A").Append(vbTab)
                str.Append("Percent C").Append(vbTab)
                str.Append("Percent G").Append(vbTab)
                str.Append("Percent T").Append(vbTab)
                Return str.ToString
            End Function
            Public Shared Function Get_Percents_All(Seq As Bio.Sequence) As String
                Dim nofA = (From x In Seq Where x = Bio.Alphabets.DNA.A).Count

                Dim nofC = (From x In Seq Where x = Bio.Alphabets.DNA.C).Count

                Dim nofG = (From x In Seq Where x = Bio.Alphabets.DNA.G).Count

                Dim nofT = (From x In Seq Where x = Bio.Alphabets.DNA.T).Count

                Dim str As New System.Text.StringBuilder
                str.Append(nofA).Append(vbTab)
                str.Append(nofC).Append(vbTab)
                str.Append(nofG).Append(vbTab)
                str.Append(nofT).Append(vbTab)

                str.Append(nofA / Seq.Count * 100).Append(vbTab)
                str.Append(nofC / Seq.Count * 100).Append(vbTab)
                str.Append(nofG / Seq.Count * 100).Append(vbTab)
                str.Append(nofT / Seq.Count * 100).Append(vbTab)

                Return str.ToString
            End Function
            Public Shared Function Get_Percents_GC(Seq As Bio.Sequence) As Double
                Dim nofC = (From x In Seq Where x = Bio.Alphabets.DNA.C).Count

                Dim nofG = (From x In Seq Where x = Bio.Alphabets.DNA.G).Count

                Return (nofC + nofG) * Seq.Count * 100

            End Function
        End Class
        Namespace Variants
            Public Class Analysis
                Private files As List(Of FileInfo)
                Private seqs As List(Of ISequence)
                Private matrixes As List(Of SimilarityMatrix)
                Public Property Vars As New List(Of Szunyi.DNA.Variants.Alignment)
                Public Property Reps As New Dictionary(Of String, Szunyi.DNA.Repeat.Repeats)

                Public Sub New(files As List(Of FileInfo), seqs As List(Of ISequence), matrixes As List(Of SimilarityMatrix), RepeatFile As List(Of FileInfo))
                    Me.files = files
                    Me.seqs = seqs
                    Me.matrixes = matrixes

                    For Each File In files
                        Dim Var As New Szunyi.DNA.Variants.Alignment(File)
                        Vars.Add(Var)
                    Next
                    For Each RepFile In RepeatFile
                        Dim k = New Szunyi.DNA.Repeat.Repeats(RepFile)
                        Reps.Add(k.Repeats.First.SeqID, k)
                    Next


                End Sub
                Public Shared Function GetPos(BaseSeq As Bio.ISequence, SecondSeq As Bio.ISequence) As Dictionary(Of String, Dictionary(Of Integer, Integer))

                    Dim sg As New Dictionary(Of String, Dictionary(Of Integer, Integer))
                    Dim pos1 As Integer = 0
                    Dim pos2 As Integer = 0
                    sg.Add(BaseSeq.ID, New Dictionary(Of Integer, Integer))
                    sg.Add(SecondSeq.ID, New Dictionary(Of Integer, Integer))
                    For i1 = 0 To BaseSeq.Count - 1
                        If BaseSeq(i1) <> Alphabets.AmbiguousDNA.Gap Then
                            pos1 += 1
                        Else
                            Dim jk As Int16 = 32
                        End If
                        If SecondSeq(i1) <> Alphabets.AmbiguousDNA.Gap Then
                            pos2 += 1
                        Else
                            Dim jk As Int16 = 43
                        End If
                        If sg(BaseSeq.ID).ContainsKey(pos1) = False Then sg(BaseSeq.ID).Add(pos1, pos2)
                        If sg(SecondSeq.ID).ContainsKey(pos2) = False Then sg(SecondSeq.ID).Add(pos2, pos1)

                    Next

                    Return sg
                End Function
                Public Function Get_Result_Merged() As String
                    Set_Codons()
                    Set_Repeats()
                    Dim Res As New System.Text.StringBuilder

                    For Each Var In Vars
                        Dim gr = SNP_MNP_INDEL_Manipulation.Group(Var.Merged_Asc_AllSNPSs)
                        Res.Append(Var.Seqs.First.ID & vbTab & Var.Seqs.Last.ID).AppendLine()
                        For Each Item In gr(0)
                            For Each Item1 In gr(1)
                                For Each item2 In gr(2)
                                    Dim txt = Item.Key & " " & Item1.Key & " " & item2.Key
                                    Dim CommonItems = Item.Value.Intersect(Item1.Value.Intersect(item2.Value))
                                    Res.Append(txt).Append(vbTab)
                                    Res.Append(CommonItems.Count).Append(vbTab)
                                    Res.Append((From x In CommonItems Select x.First_Bytes.Count).Sum).AppendLine()

                                Next
                            Next
                        Next

                    Next
                    Res.AppendLine()
                    Windows.Forms.Clipboard.SetText(Res.ToString)
                    Dim str As New System.Text.StringBuilder
                    Dim out As New List(Of String)
                    Dim Header As New List(Of String)
                    Dim str1 As New System.Text.StringBuilder
                    For Each Var In Vars

                        Dim All_Ins_Del = (From x In Var.Merged_Asc_AllSNPSs Where x.Type = Variation_Type.INS Or x.Type = Variation_Type.Del).ToList
                        Dim All_SNP = (From x In Var.Merged_Asc_AllSNPSs Where x.Type = Variation_Type.SNP).ToList
                        Dim All_MNP = (From x In Var.Merged_Asc_AllSNPSs Where x.Type = Variation_Type.MNP).ToList
                        Dim All_Del = (From x In Var.Merged_Asc_AllSNPSs Where x.Type = Variation_Type.Del).ToList
                        Dim All_Ins = (From x In Var.Merged_Asc_AllSNPSs Where x.Type = Variation_Type.INS).ToList
                        Dim InRepeat = (From x In Var.Merged_Asc_AllSNPSs Where x.IsInRepeatRegion = True).ToList
                        Dim kj As New System.Text.StringBuilder
                        For Each item In All_MNP
                            kj.Append(item.ToString).AppendLine()
                        Next
                        Windows.Forms.Clipboard.SetText(kj.ToString)
                        Dim InRepeatContainsRepeat = (From x In Var.Merged_Asc_AllSNPSs Where x.ContainRepeatUnit = True).ToList
                        Dim InCDS_COntains_Repeat = (From x In InRepeatContainsRepeat Where x.AA_Changes.Count > 0).ToList
                        Dim InIntergenicRegion_Contain_Repeat = (From x In InRepeatContainsRepeat Where x.AA_Changes.Count = 0).ToList
                        Dim Codings = (From x In Var.Merged_Asc_AllSNPSs Where x.AA_Changes.Count > 0).ToList
                        Dim InterGenic_All_Ins_Del = (From x In All_Ins_Del Where x.AA_Changes.Count = 0)
                        Dim InterGenic_SNP = (From x In All_SNP Where x.AA_Changes.Count = 0)
                        Dim InterGenic_MNP = (From x In All_MNP Where x.AA_Changes.Count = 0)

                        Dim Coding_All_Ins_Del = (From x In All_Ins_Del Where x.AA_Changes.Count > 0)
                        Dim Coding_SNP = (From x In All_SNP Where x.AA_Changes.Count > 0)
                        Dim Coding_MNP = (From x In All_MNP Where x.AA_Changes.Count > 0)
                        str1.Append(Var.Seqs.First.ID & vbTab & Var.Seqs.Last.ID).AppendLine()
                        str1.Append("Non-Coding InDel Counts").Append(vbTab)
                        str1.Append(InterGenic_All_Ins_Del.Count).AppendLine()

                        str1.Append("Non-Coding InDel Nof Nucleotide").Append(vbTab)
                        str1.Append((From x In InterGenic_All_Ins_Del Select x.First_Bytes.Count).Sum).AppendLine()

                        str1.Append("Non-Coding MNV Counts").Append(vbTab)
                        str1.Append(InterGenic_MNP.Count).AppendLine()

                        str1.Append("Non-Coding SNV Counts").Append(vbTab)
                        str1.Append(InterGenic_SNP.Count).AppendLine()
                        str1.Append("Non-Coding MNP Nof Nucleotide").Append(vbTab)
                        str1.Append((From x In InterGenic_MNP Select x.First_Bytes.Count).Sum).AppendLine()

                        str1.Append("Coding InDel Counts").Append(vbTab)
                        str1.Append(Coding_All_Ins_Del.Count).AppendLine()
                        str1.Append("Coding InDel Nof Nucleotide").Append(vbTab)
                        str1.Append((From x In Coding_All_Ins_Del Select x.First_Bytes.Count).Sum).AppendLine()

                        str1.Append("Coding MNP Counts").Append(vbTab)
                        str1.Append(Coding_MNP.Count).AppendLine()
                        str1.Append("Coding MNP Nof Nucleotide").Append(vbTab)
                        str1.Append((From x In Coding_MNP Select x.First_Bytes.Count).Sum).AppendLine()


                        str1.Append("Coding SNP Counts").Append(vbTab)
                        str1.Append(Coding_SNP.Count).AppendLine()

                        Dim tmp = Get_AA_Changes(Coding_SNP.ToList)
                        str1.Append("Nof Diff CDS All").Append(vbTab)
                        str1.Append(GetNofDiffCDS(Var.Asc_AllSNPSs)).AppendLine()

                        str1.Append("Nof Diff CDS SNP").Append(vbTab)
                        str1.Append(GetNofDiffCDS(Coding_SNP.ToList)).AppendLine()

                        str1.Append("Nof Diff CDS MNP").Append(vbTab)
                        str1.Append(GetNofDiffCDS(Coding_MNP.ToList)).AppendLine()

                        str1.Append("Nof Diff CDS InDel").Append(vbTab)
                        str1.Append(GetNofDiffCDS(Coding_All_Ins_Del.ToList)).AppendLine()

                        For i1 = 0 To matrixes.Count - 1
                            str1.Append(tmp.First.Matrixes(i1).Name.Replace(vbTab, "")).Append(vbTab)
                        Next
                        str1.AppendLine()
                        For i1 = 0 To matrixes.Count - 1
                            Dim m1 = (From x In tmp Select x.MatrixValues(i1)).Sum
                            str1.Append(m1).Append(vbTab)
                        Next
                        str1.AppendLine()
                        Windows.Forms.Clipboard.SetText(str1.ToString)
                    Next
                    If Res.ToString.Length > 0 AndAlso str1.Length > 0 Then
                        Return Res.ToString & vbCrLf & str1.ToString
                    Else
                        Return String.Empty
                    End If
                End Function
                Public Shared Function Get_AA_Changes(ls As List(Of SNP_MNP_INDEL)) As List(Of AA_Change)
                    Dim AA_CHanges1 = (From x In ls Select x.AA_Changes).ToList

                    Dim tmp As New List(Of AA_Change)
                    For Each Item In AA_CHanges1
                        tmp.AddRange(Item)
                    Next
                    Return tmp
                End Function
                Private Function GetNofDiffCDS(ls As List(Of SNP_MNP_INDEL)) As Integer
                    Dim kj = Get_AA_Changes(ls)
                    Dim Feats As New List(Of FeatureItem)
                    For Each item In kj
                        If IsNothing(item.First) = False Then
                            Feats.Add(item.First.Feat)
                        End If
                    Next
                    If Feats.Count = 0 Then
                        Return 0
                    Else
                        Return Feats.Distinct.Count
                    End If
                End Function
                Private Sub Set_Codons()
                    For Each var In Vars
                        Dim cSeqs = Szunyi.Sequences.SequenceManipulation.SelectBy.GetSeqContainsIDs(seqs, var.Seqs)
                        Szunyi.DNA.Variants.SNP_MNP_INDEL_Manipulation.Set_Codons(var.Merged_Asc_AllSNPSs, cSeqs, matrixes)
                        Szunyi.DNA.Variants.SNP_MNP_INDEL_Manipulation.Set_Codons(var.Asc_AllSNPSs, cSeqs, matrixes)
                        Szunyi.DNA.Variants.SNP_MNP_INDEL_Manipulation.Set_Codons(var.Merged_Desc_AllSNPSs, cSeqs, matrixes)
                        Szunyi.DNA.Variants.SNP_MNP_INDEL_Manipulation.Set_Codons(var.Desc_AllSNPSs, cSeqs, matrixes)
                    Next
                End Sub
                Private Sub Set_Repeats()
                    For Each var In Vars
                        Dim cSeqs = Szunyi.Sequences.SequenceManipulation.SelectBy.GetSeqContainsIDs(seqs, var.Seqs)
                        Dim RepeatKey As String = ""
                        If Reps.ContainsKey(cSeqs.First.ID) = True Then
                            RepeatKey = cSeqs.First.ID
                        ElseIf Reps.ContainsKey(cSeqs.last.ID) = True Then
                            RepeatKey = cSeqs.Last.ID
                        End If
                        Szunyi.DNA.Variants.SNP_MNP_INDEL_Manipulation.Set_Repeats(var.Merged_Asc_AllSNPSs, cSeqs, Reps(RepeatKey))
                        Szunyi.DNA.Variants.SNP_MNP_INDEL_Manipulation.Set_Repeats(var.Asc_AllSNPSs, cSeqs, Reps(RepeatKey))
                        Szunyi.DNA.Variants.SNP_MNP_INDEL_Manipulation.Set_Repeats(var.Merged_Desc_AllSNPSs, cSeqs, Reps(RepeatKey))
                        Szunyi.DNA.Variants.SNP_MNP_INDEL_Manipulation.Set_Repeats(var.Desc_AllSNPSs, cSeqs, Reps(RepeatKey))
                    Next
                End Sub
            End Class
            Public Class Alignment
                Public Property Asc_AllSNPSs As New List(Of Szunyi.DNA.Variants.SNP_MNP_INDEL)
                Public Property Desc_AllSNPSs As New List(Of Szunyi.DNA.Variants.SNP_MNP_INDEL)
                Public Property Merged_Asc_AllSNPSs As New List(Of Szunyi.DNA.Variants.SNP_MNP_INDEL)
                Public Property Merged_Desc_AllSNPSs As New List(Of Szunyi.DNA.Variants.SNP_MNP_INDEL)
                Public Property Seqs As List(Of Bio.ISequence)
                Public Property Sum As String
                Private Function Get_AL(ListOfSeqs As List(Of Bio.ISequence)) As List(Of SNP_MNP_INDEL)
                    Dim FirstPosition As Integer = 0
                    Dim SecondPosition As Integer = 0
                    Dim al1 = ListOfSeqs.First.ToArray
                    Dim al2 = ListOfSeqs.Last.ToArray
                    Dim SNPS As New List(Of SNP_MNP_INDEL)
                    For Common_Position As Integer = 0 To ListOfSeqs.First.Count - 1
                        Dim FirstByte = al1(Common_Position)
                        Dim SecondByte = al2(Common_Position)
                        If FirstByte <> SecondByte Then
                            Dim x As Szunyi.DNA.Variants.SNP_MNP_INDEL
                            If FirstByte = 45 Then
                                Dim First_Location = Szunyi.Location.Common.GetLocation(FirstPosition + 1, FirstPosition + 1, "+")
                                Dim Second_Location = Szunyi.Location.Common.GetLocation(SecondPosition + 1, SecondPosition + 2, "+")
                                Dim Common_Location = Szunyi.Location.Common.GetLocation(Common_Position + 1, Common_Position + 2, "+")
                                x = New Szunyi.DNA.Variants.SNP_MNP_INDEL(First_Location, Second_Location, Common_Location, FirstByte, SecondByte, Szunyi.DNA.Variants.Variation_Type.INS)

                                SecondPosition += 1
                            ElseIf SecondByte = 45 Then
                                Dim First_Location = Szunyi.Location.Common.GetLocation(FirstPosition + 1, FirstPosition + 2, "+")
                                Dim Second_Location = Szunyi.Location.Common.GetLocation(SecondPosition + 1, SecondPosition + 1, "+")
                                Dim Common_Location = Szunyi.Location.Common.GetLocation(Common_Position + 1, Common_Position + 2, "+")
                                x = New Szunyi.DNA.Variants.SNP_MNP_INDEL(First_Location, Second_Location, Common_Location, FirstByte, SecondByte, Szunyi.DNA.Variants.Variation_Type.Del)
                                FirstPosition += 1
                            Else
                                Dim First_Location = Szunyi.Location.Common.GetLocation(FirstPosition + 1, FirstPosition + 2, "+")
                                Dim Second_Location = Szunyi.Location.Common.GetLocation(SecondPosition + 1, SecondPosition + 2, "+")
                                Dim Common_Location = Szunyi.Location.Common.GetLocation(Common_Position + 1, Common_Position + 2, "+")
                                x = New Szunyi.DNA.Variants.SNP_MNP_INDEL(First_Location, Second_Location, Common_Location, FirstByte, SecondByte, Szunyi.DNA.Variants.Variation_Type.SNP)
                                FirstPosition += 1
                                SecondPosition += 1
                            End If
                            SNPS.Add(x)
                        Else
                            FirstPosition += 1
                            SecondPosition += 1
                        End If
                    Next
                    Return SNPS
                End Function

                Public Sub New(File As System.IO.FileInfo)
                    Dim AL = Szunyi.IO.Import.Sequence.FromFile(File)
                    AL = (From x In AL Order By x.ID Ascending).ToList
                    Me.Seqs = AL
                    Me.Asc_AllSNPSs = Get_AL(AL)

                    Dim AL2 = (From x In AL Order By x.ID Descending).ToList
                    Me.Desc_AllSNPSs = Get_AL(AL2)


                    Merged_Asc_AllSNPSs = Szunyi.DNA.Variants.SNP_MNP_INDEL_Manipulation.MergeSNPs(Asc_AllSNPSs)
                    Merged_Asc_AllSNPSs = (From x In Merged_Asc_AllSNPSs Order By x.First_Location.LocationStart).ToList

                    Merged_Desc_AllSNPSs = Szunyi.DNA.Variants.SNP_MNP_INDEL_Manipulation.MergeSNPs(Desc_AllSNPSs)
                    Merged_Desc_AllSNPSs = (From x In Merged_Desc_AllSNPSs Order By x.First_Location.LocationStart).ToList



                End Sub

                Public Function Get_Sum_Asc() As String
                    Dim sum As String
                    sum = Me.Seqs.First.ID & vbTab & Me.Seqs.Last.ID & vbCrLf
                    sum = sum & Szunyi.DNA.Variants.SNP_MNP_INDEL_Manipulation.Get_Aggregate_Of_SNPs(Asc_AllSNPSs)
                    Return sum
                End Function

                Public Function Get_Sum_Desc() As String
                    Dim sum As String
                    sum = Me.Seqs.Last.ID & vbTab & Me.Seqs.First.ID & vbCrLf
                    sum = sum & Szunyi.DNA.Variants.SNP_MNP_INDEL_Manipulation.Get_Aggregate_Of_SNPs(Desc_AllSNPSs)
                    Return sum
                End Function

                Public Function Get_Opposite_Nucleotide(pos As Integer, SeqID As Integer) As Integer

                End Function
            End Class

            Public Class Codon
                Public Property Frame As Integer
                Public Property Loci As ILocation
                Public Property Seq As Bio.ISequence
                Public Property AASeq As Bio.ISequence
                Public Property CDS_NA_Position As Integer
                Public Property CDS_AA_Position As Integer
                Public Property Feat As FeatureItem
                Public Sub New(NASeq As Bio.ISequence, AASeq As Bio.ISequence, loci As ILocation, frame As Integer, feat As FeatureItem)
                    Me.Seq = NASeq
                    Me.AASeq = AASeq
                    Me.Loci = loci
                    Me.Frame = frame
                    Me.Feat = feat
                    If feat.Location.IsComplementer = False Then
                        Me.CDS_NA_Position = loci.LocationStart - feat.Location.LocationStart
                    Else
                        Me.CDS_NA_Position = feat.Location.LocationEnd - loci.LocationEnd
                    End If
                    Me.CDS_AA_Position = Me.CDS_NA_Position \ 3
                End Sub
                Public Overrides Function ToString() As String
                    Dim s = Frame & vbTab & Seq.ConvertToString & vbTab & AASeq.ConvertToString & vbTab _
                        & Szunyi.Location.Common.GetLocationString(Me.Loci) & vbTab & Me.CDS_NA_Position & vbTab & CDS_AA_Position & vbTab

                    If IsNothing(Me.Feat) = True Then
                        Return s
                    Else
                        Return s & Feat.Qualifiers(StandardQualifierNames.GeneSymbol).First.Replace(Chr(34), "")
                    End If
                End Function
                Public Function empty() As String
                    Return Szunyi.Text.General.Multiply(vbTab, 7)
                End Function
            End Class

            Public Class SNP_MNP_INDEL_Manipulation
                Public Shared Function Group(ls As List(Of SNP_MNP_INDEL)) As List(Of Dictionary(Of String, List(Of SNP_MNP_INDEL)))
                    ' SNP, MNP, SNP-MNP,Insertion,Deletion,Indel
                    ' In CDS or not
                    ' In repetition or Not
                    ' Is contain repeat or not
                    Dim res As New List(Of Dictionary(Of String, List(Of SNP_MNP_INDEL)))

                    Dim MutationByType As New Dictionary(Of String, List(Of SNP_MNP_INDEL))
                    MutationByType.Add("SNP", Get_SNP_ByType(ls, Variation_Type.SNP))
                    MutationByType.Add("MNP", Get_SNP_ByType(ls, Variation_Type.MNP))
                    Dim Insertions = Get_SNP_ByType(ls, Variation_Type.INS)
                    Dim Deletions = Get_SNP_ByType(ls, Variation_Type.Del)
                    Dim InDels As New List(Of SNP_MNP_INDEL)
                    InDels.AddRange(Insertions)
                    InDels.AddRange(Deletions)
                    MutationByType.Add("InDel", InDels)

                    res.Add(MutationByType)

                    Dim ByReps As New Dictionary(Of String, List(Of SNP_MNP_INDEL))
                    Dim InReps = (From x In ls Where x.IsInRepeatRegion = True).ToList
                    Dim COntainRepeetions = (From x In ls Where x.ContainRepeatUnit = True).ToList
                    Dim NotInRepetition = (From x In ls Where x.IsInRepeatRegion = False).ToList
                    ByReps.Add("In Repeat Region", InReps)
                    ByReps.Add("Contain Repeat Motif", COntainRepeetions)
                    ByReps.Add("Not In Repeat Region", NotInRepetition)

                    res.Add(ByReps)

                    Dim IntergenicOrProtein As New Dictionary(Of String, List(Of SNP_MNP_INDEL))
                    Dim InterGenic = (From x In ls Where x.AA_Changes.Count = 0).ToList
                    Dim CDSs = (From x In ls Where x.AA_Changes.Count > 0).ToList
                    IntergenicOrProtein.Add("Intergenic", InterGenic)
                    IntergenicOrProtein.Add("InCDS", CDSs)
                    res.Add(IntergenicOrProtein)
                    Return res

                End Function
                Public Shared Function Get_SNP_With_AA_Changes(ls As List(Of SNP_MNP_INDEL)) As List(Of SNP_MNP_INDEL)

                    Dim tmp = (From x In ls Where x.AA_Changes.Count > 0)
                    If tmp.Count > 0 Then
                        Return tmp.ToList
                    End If
                    Return New List(Of SNP_MNP_INDEL)
                End Function
                Public Shared Function Get_SNP_WithOut_AA_Changes(ls As List(Of SNP_MNP_INDEL)) As List(Of SNP_MNP_INDEL)
                    Dim tmp = (From x In ls Where x.AA_Changes.Count = 0)
                    If tmp.Count > 0 Then
                        Return tmp.ToList
                    End If
                    Return New List(Of SNP_MNP_INDEL)
                End Function
                Public Shared Function Get_SNP_ByType(ls As List(Of SNP_MNP_INDEL), Type As Variation_Type) As List(Of SNP_MNP_INDEL)
                    Dim tmp = (From x In ls Where x.Type = Type)
                    If tmp.Count > 0 Then
                        Return tmp.ToList
                    End If
                    Return New List(Of SNP_MNP_INDEL)
                End Function
                ''' <summary>
                ''' Return total nof different SNPs/Insertions/Deletions
                ''' </summary>
                ''' <param name="AllSNPSs"></param>
                ''' <returns></returns>
                Public Shared Function Get_Aggregate_Of_SNPs(AllSNPSs As List(Of SNP_MNP_INDEL)) As String
                    Dim d As New Dictionary(Of String, Integer)
                    For Each SNP In AllSNPSs
                        Dim f = SNP_MNP_INDEL_Manipulation.Get_Mutation_As_String(SNP)
                        If d.ContainsKey(f) = False Then d.Add(f, 0)
                        d(f) += 1
                    Next
                    Dim s = Szunyi.Text.Dict.Get_as_Text(d)
                    Return s
                End Function
                Public Shared Function Export(MergedSNPS As List(Of SNP_MNP_INDEL), Seqs As List(Of Bio.ISequence), Matrixes As List(Of Bio.SimilarityMatrices.SimilarityMatrix)) As String
                    Dim Header As New System.Text.StringBuilder
                    Header.Append("Type").Append(vbTab)
                    Header.Append(Seqs.First.ID & " Location").Append(vbTab)
                    Header.Append(Seqs.Last.ID & " Location").Append(vbTab)
                    Header.Append(Seqs.First.ID & " Seq").Append(vbTab)
                    Header.Append(Seqs.Last.ID & " Seq").Append(vbTab)

                    Header.Append(Seqs.First.ID & " Frame").Append(vbTab)
                    Header.Append(Seqs.First.ID & " Codon Seq").Append(vbTab)
                    Header.Append(Seqs.First.ID & " AA Seq").Append(vbTab)
                    Header.Append(Seqs.First.ID & " Codon Location").Append(vbTab)
                    Header.Append(Seqs.First.ID & " CDS NA Position").Append(vbTab)
                    Header.Append(Seqs.First.ID & " CDS AA Position").Append(vbTab)
                    Header.Append(Seqs.First.ID & " CDS Name").Append(vbTab)

                    Header.Append(Seqs.Last.ID & " Frame").Append(vbTab)
                    Header.Append(Seqs.Last.ID & " Codon Seq").Append(vbTab)
                    Header.Append(Seqs.Last.ID & " AA Seq").Append(vbTab)
                    Header.Append(Seqs.Last.ID & " Codon Location").Append(vbTab)
                    Header.Append(Seqs.Last.ID & " CDS NA Position").Append(vbTab)
                    Header.Append(Seqs.Last.ID & " CDS AA Position").Append(vbTab)
                    Header.Append(Seqs.Last.ID & " CDS Name").Append(vbTab)
                    For Each Matrix1 In Matrixes
                        Header.Append(Matrix1.Name).Append(vbTab)
                    Next
                    Header.AppendLine()
                    Dim h As String = Header.ToString
                    Dim cSeqs As New List(Of Bio.ISequence)

                    For Each SNP In MergedSNPS

                        Dim s = SNP_MNP_INDEL_Manipulation.GetText(SNP, Seqs, Matrixes)

                        h = h & s


                    Next
                    Return h
                End Function


                Public Shared Function MergeSNPs(AllSNPSs As List(Of SNP_MNP_INDEL)) As List(Of SNP_MNP_INDEL)
                    Dim out As New List(Of SNP_MNP_INDEL)

                    Dim INS = From x In AllSNPSs Where x.Type = Variation_Type.INS

                    Dim Dels = From x In AllSNPSs Where x.Type = Variation_Type.Del

                    Dim SNPs = From x In AllSNPSs Where x.Type = Variation_Type.SNP

                    out.AddRange(MergeInvSNPs(INS.ToList))
                    out.AddRange(MergeInvSNPs(Dels.ToList))
                    out.AddRange(MergeInvSNPs(SNPs.ToList))
                    Return out
                End Function
                Public Shared Function MergeInvSNPs(SNPs As List(Of SNP_MNP_INDEL)) As List(Of SNP_MNP_INDEL)
                    Dim out As New List(Of SNP_MNP_INDEL)
                    For i1 = 0 To SNPs.Count - 2
                        Dim FirstBytes As New List(Of Byte)
                        Dim SecondBytes As New List(Of Byte)
                        FirstBytes.Add(SNPs(i1).First_Byte)
                        SecondBytes.Add(SNPs(i1).Second_Byte)
                        For i2 = i1 + 1 To SNPs.Count - 1

                            If SNPs(i2).Common_Location.LocationEnd - SNPs(i1).Common_Location.LocationEnd <> i2 - i1 Then
                                ' Big diff
                                ' Check before
                                If SNPs(i2 - 1).Common_Location.LocationEnd - SNPs(i1).Common_Location.LocationEnd = i2 - i1 - 1 And i2 - i1 - 1 > 0 Then
                                    ' Merge
                                    out.Add(New SNP_MNP_INDEL(SNPs(i1), SNPs(i2 - 1), FirstBytes, SecondBytes))
                                    '    out.Add(Merge_2SNPs(SNPs(i1), SNPs(i2 - 1)))
                                Else
                                    '    out.Add(Merge_2SNPs(SNPs(i1), SNPs(i1)))
                                    out.Add(New SNP_MNP_INDEL(SNPs(i1), SNPs(i1), FirstBytes, SecondBytes))
                                End If
                                i1 = i2 - 1
                                Exit For
                            Else
                                FirstBytes.Add(SNPs(i2).First_Byte)
                                SecondBytes.Add(SNPs(i2).Second_Byte)
                            End If
                        Next
                    Next
                    Return out

                End Function
                Public Shared Sub Set_Repeats(ls As List(Of SNP_MNP_INDEL), cSeqs As List(Of ISequence), reps As Repeats)
                    Dim isFirst As Boolean = False

                    If reps.Repeats.First.SeqID = cSeqs.First.ID Then isFirst = True
                    If isFirst = False Then If reps.Repeats.First.SeqID <> cSeqs.Last.ID Then Exit Sub

                    For Each x In ls

                        If isFirst = True Then
                            Dim kj As Int16 = 54
                            Dim sg = From h In reps.Repeats Where x.First_Location.LocationStart >= h.Start And x.First_Location.LocationEnd <= h.Endy

                            If sg.Count > 0 Then
                                x.IsInRepeatRegion = True
                                Dim seqy As New Bio.Sequence(Alphabets.AmbiguousDNA, x.First_Bytes.ToArray)
                                Dim seqy2 As New Bio.Sequence(Alphabets.AmbiguousDNA, x.Second_Bytes.ToArray)

                                If seqy.ConvertToString.Contains(sg.First.Repeat.ConvertToString) Then
                                    x.ContainRepeatUnit = True
                                End If
                                If seqy2.ConvertToString.Contains(sg.First.Repeat.ConvertToString) Then
                                    x.ContainRepeatUnit = True
                                End If
                            End If

                        Else
                            Dim sg = From h In reps.Repeats Where x.Second_Location.LocationStart >= h.Start And x.Second_Location.LocationEnd <= h.Endy

                            If sg.Count > 0 Then
                                x.IsInRepeatRegion = True
                                Dim seqy As New Bio.Sequence(Alphabets.AmbiguousDNA, x.First_Bytes.ToArray)
                                Dim seqy2 As New Bio.Sequence(Alphabets.AmbiguousDNA, x.Second_Bytes.ToArray)

                                If seqy.ConvertToString.Contains(sg.First.Repeat.ConvertToString) Then
                                    x.ContainRepeatUnit = True
                                End If
                                If seqy2.ConvertToString.Contains(sg.First.Repeat.ConvertToString) Then
                                    x.ContainRepeatUnit = True
                                End If
                            End If
                        End If
                    Next
                End Sub
                Public Shared Sub Set_Codons(ls As List(Of SNP_MNP_INDEL), Seqs As List(Of Bio.ISequence), Matrixes As List(Of Bio.SimilarityMatrices.SimilarityMatrix))
                    For Each x In ls
                        Dim s_Last = GetCDSNAME(Seqs.Last, x, False)
                        Dim s_First = GetCDSNAME(Seqs.First, x, True)
                        If s_First.Count <> 0 Or s_Last.Count <> 0 Then
                            Select Case x.Type
                                Case Variation_Type.Del
                                    For Each Item In s_First
                                        x.AA_Changes.Add(New AA_Change(Item, Nothing, Matrixes))
                                    Next
                                Case Variation_Type.INS
                                    For Each Item In s_Last
                                        x.AA_Changes.Add(New AA_Change(Nothing, Item, Matrixes))
                                    Next
                                Case Else
                                    If s_Last.Count = s_First.Count Then
                                        For i1 = 0 To s_First.Count - 1
                                            x.AA_Changes.Add(New AA_Change(s_First(i1), s_Last(i1), Matrixes))
                                        Next

                                    Else
                                        Dim ald As Int16 = 43

                                    End If

                            End Select

                        End If
                    Next


                End Sub
                Public Shared Function GetText(x As SNP_MNP_INDEL, Seqs As List(Of Bio.ISequence), Matrixes As List(Of Bio.SimilarityMatrices.SimilarityMatrix)) As String
                    Dim s_Last = GetCDSNAME(Seqs.Last, x, False)
                    Dim s_First = GetCDSNAME(Seqs.First, x, True)
                    Dim str As New System.Text.StringBuilder
                    If s_First.Count = 0 AndAlso s_Last.Count = 0 Then
                        Return x.ToString & vbCrLf
                    End If
                    Select Case x.Type
                        Case Variation_Type.Del
                            For Each Item In s_First
                                str.Append(x.ToString)
                                str.Append(Item.empty)
                                str.Append(Item.ToString).AppendLine()
                            Next
                        Case Variation_Type.INS
                            For Each Item In s_Last
                                str.Append(x.ToString)
                                str.Append(Item.empty)

                                str.Append(Item.ToString).AppendLine()
                            Next
                        Case Else
                            If s_Last.Count = s_First.Count Then
                                For i1 = 0 To s_Last.Count - 1
                                    str.Append(x.ToString)
                                    str.Append(s_First(i1).ToString).Append(vbTab)
                                    str.Append(s_Last(i1).ToString).Append(vbTab)

                                    For Each Matrix1 In Matrixes
                                        Dim alf As Int16 = 54
                                        str.Append(Matrix1.Item(s_First(i1).AASeq.First, s_Last(i1).AASeq.First)).Append(vbTab)
                                    Next
                                    str.AppendLine()
                                Next
                            Else
                                Dim ald As Int16 = 43

                            End If

                    End Select


                    Return str.ToString
                End Function
                Public Shared Function GetCDSNAME(Seq As Bio.Sequence, SNP As SNP_MNP_INDEL, Isfirst As Boolean) As List(Of Codon)
                    Dim CDSs = Szunyi.Features.FeatureManipulation.GetFeaturesByType.GetFeturesByTypeFromSeq(Seq, Bio.IO.GenBank.StandardFeatureKeys.CodingSequence)
                    If IsNothing (CDSs) = True Then Return New List(Of Codon) 
                    Dim pos As Integer = 0
                    If Isfirst = True Then
                        pos = SNP.First_Location.LocationStart
                    Else
                        pos = SNP.Second_Location.LocationStart
                    End If
                    Dim Founded = From x In CDSs Where pos >= x.Location.LocationStart And pos <= x.Location.LocationEnd

                    Dim codons As New List(Of Szunyi.DNA.Variants.Codon)
                    For Each f In Founded
                        If Isfirst = True Then
                            If f.Location.Operator = Bio.IO.GenBank.LocationOperator.Complement Then
                                codons = Szunyi.DNA.Frame.Get_Codons(SNP.First_Location, f, Seq)
                            Else
                                codons = Szunyi.DNA.Frame.Get_Codons(SNP.First_Location, f, Seq)
                            End If
                        Else
                            If f.Location.Operator = Bio.IO.GenBank.LocationOperator.Complement Then
                                codons = Szunyi.DNA.Frame.Get_Codons(SNP.Second_Location, f, Seq)
                            Else
                                codons = Szunyi.DNA.Frame.Get_Codons(SNP.Second_Location, f, Seq)
                            End If

                        End If
                    Next
                    Return codons
                End Function
                Public Shared Function Get_Mutation_As_String(x As SNP_MNP_INDEL) As String
                    Return ChrW(x.First_Byte) & vbTab & ChrW(x.Second_Byte)
                End Function
                Public Shared Function GetChar(b As Byte) As String
                    If b <> 0 Then
                        Return ChrW(b)
                    Else
                        Return "0"
                    End If
                End Function

                Public Shared Iterator Function Get_SNP_By_CDS_Both(ls As List(Of SNP_MNP_INDEL)) As IEnumerable(Of List(Of SNP_ByCDS))
                    Dim out As New List(Of SNP_ByCDS)
                    For Each item In ls
                        For Each AA_change In item.AA_Changes
                            If IsNothing(AA_change.First) = False AndAlso IsNothing(AA_change.Second) = False Then
                                out.Add(New SNP_ByCDS(AA_change.First.Feat, AA_change.Second.Feat, item))
                            ElseIf IsNothing(AA_change.First) = False Then
                                out.Add(New SNP_ByCDS(AA_change.First.Feat, Nothing, item))
                            ElseIf IsNothing(AA_change.Second) = False Then
                                out.Add(New SNP_ByCDS(Nothing, AA_change.Second.Feat, item))
                            End If
                        Next
                    Next
                    Dim res = From x In out Group By x.Feat1, x.Feat2 Into Group

                    For Each item In res
                        Yield item.Group.ToList
                    Next
                End Function

                Public Shared Iterator Function Get_SNP_By_CDS_First_Feat(ls As List(Of SNP_MNP_INDEL)) As IEnumerable(Of List(Of SNP_ByCDS))
                    Dim out As New List(Of SNP_ByCDS)
                    For Each item In ls
                        For Each AA_change In item.AA_Changes
                            If IsNothing(AA_change.First) = False AndAlso IsNothing(AA_change.Second) = False Then
                                out.Add(New SNP_ByCDS(AA_change.First.Feat, AA_change.Second.Feat, item))
                            ElseIf IsNothing(AA_change.First) = False Then
                                out.Add(New SNP_ByCDS(AA_change.First.Feat, Nothing, item))
                            ElseIf IsNothing(AA_change.Second) = False Then
                                '   out.Add(New SNP_ByCDS(Nothing, AA_change.Second.Feat, item))
                            End If
                        Next
                    Next
                    Dim res = From x In out Group By x.Feat1 Into Group

                    For Each item In res
                        Yield item.Group.ToList
                    Next
                End Function
                Public Shared Iterator Function Get_SNP_By_CDS_Second_Feat(ls As List(Of SNP_MNP_INDEL)) As IEnumerable(Of List(Of SNP_ByCDS))
                    Dim out As New List(Of SNP_ByCDS)
                    For Each item In ls
                        For Each AA_change In item.AA_Changes
                            If IsNothing(AA_change.First) = False AndAlso IsNothing(AA_change.Second) = False Then
                                out.Add(New SNP_ByCDS(AA_change.First.Feat, AA_change.Second.Feat, item))
                            ElseIf IsNothing(AA_change.First) = False Then
                                '   out.Add(New SNP_ByCDS(AA_change.First.Feat, Nothing, item))
                            ElseIf IsNothing(AA_change.Second) = False Then
                                out.Add(New SNP_ByCDS(Nothing, AA_change.Second.Feat, item))
                            End If
                        Next
                    Next
                    Dim res = From x In out Group By x.Feat2 Into Group

                    For Each item In res
                        Yield item.Group.ToList
                    Next
                End Function
                Public Shared Function Get_As_Protein(g As List(Of SNP_ByCDS), WithSilent As Boolean) As String
                    Dim str As New System.Text.StringBuilder
                    Dim Nof_AA_Change As Integer = 0
                    Dim feat As FeatureItem
                    For Each Item In g
                        For Each AA_Change In Item.SNP.AA_Changes
                            If IsNothing(AA_Change.First) = False AndAlso
                            IsNothing(AA_Change.Second) = False AndAlso
                            AA_Change.First.AASeq.ConvertToString = AA_Change.Second.AASeq.ConvertToString And WithSilent = False Then
                                Dim kj As Int16 = 43

                            Else
                                If IsNothing(AA_Change.First) = False Then
                                    feat = AA_Change.First.Feat
                                    Nof_AA_Change += AA_Change.First.AASeq.Count
                                    str.Append(AA_Change.First.AASeq.ConvertToString)
                                    str.Append(AA_Change.First.CDS_AA_Position)

                                Else
                                    Nof_AA_Change += AA_Change.Second.AASeq.Count
                                    str.Append("-")
                                    str.Append(AA_Change.Second.CDS_AA_Position)

                                End If


                                If IsNothing(AA_Change.Second) = True Then
                                    str.Append("-")
                                Else
                                    str.Append(AA_Change.Second.AASeq.ConvertToString).Append(" ")
                                End If
                            End If

                        Next
                    Next
                    If IsNothing(feat) = True Then
                        Return String.Empty
                    Else
                        Return feat.Qualifiers(StandardQualifierNames.GeneSymbol).First & vbTab & "Nof AA Chage " & Nof_AA_Change & " " & str.ToString

                    End If
                End Function


            End Class

            Public Class SNP_ByCDS
                Private item As SNP_MNP_INDEL

                Public Sub New(feat1 As FeatureItem, feat2 As FeatureItem, item As SNP_MNP_INDEL)
                    Me.Feat1 = feat1
                    Me.Feat2 = feat2
                    Me.SNP = item
                End Sub

                Public Property Feat1 As FeatureItem
                Public Property Feat2 As FeatureItem
                Public Property SNP As SNP_MNP_INDEL
                Public Shared Function Get_Text(x As SNP_ByCDS, IsFirst As Boolean) As String
                    Dim out As String
                    Dim Pos As String
                    If IsFirst = True Then
                        If x.SNP.AA_Changes.Count = 1 Then
                            Pos = x.SNP.AA_Changes.First.First.CDS_AA_Position
                        Else
                            Pos = x.SNP.AA_Changes.First.First.CDS_AA_Position & "-" & x.SNP.AA_Changes.Last.First.CDS_AA_Position
                        End If
                    Else
                        If x.SNP.AA_Changes.Count = 1 Then
                            Pos = x.SNP.AA_Changes.First.Second.CDS_AA_Position
                        Else
                            Pos = x.SNP.AA_Changes.First.Second.CDS_AA_Position & "-" & x.SNP.AA_Changes.Last.Second.CDS_AA_Position
                        End If
                    End If
                    If IsFirst = True Then
                        For Each b In x.SNP.AA_Changes
                            out = out & b.First.AASeq.ConvertToString
                        Next


                        If IsNothing(x.Feat2) = True Then
                            out = "Ins(" & Pos & ")" & out & ";"
                        Else
                            Dim F As String = ""
                            Dim S As String = ""
                            For Each b In x.SNP.AA_Changes
                                F = F & b.First.AASeq.ConvertToString
                                S = S & b.Second.AASeq.ConvertToString
                            Next
                            out = F & Pos & S & ";"
                        End If
                    End If
                    Return out
                End Function
                Public Shared Function Get_Text(x As List(Of AA_Change), IsFirst As Boolean, feat2 As FeatureItem) As String
                    If x.Count = 0 Then Return String.Empty
                    Dim out As String
                    Dim Pos As String
                    If IsFirst = True Then
                        If x.Count = 1 Then
                            Pos = x.First.First.CDS_AA_Position
                        Else
                            Pos = x.First.First.CDS_AA_Position & "-" & x.Last.First.CDS_AA_Position
                        End If
                    Else
                        If x.Count = 1 Then
                            Pos = x.First.Second.CDS_AA_Position
                        Else
                            Pos = x.First.Second.CDS_AA_Position & "-" & x.Last.Second.CDS_AA_Position
                        End If
                    End If
                    If IsFirst = True Then
                        For Each b In x
                            out = out & b.First.AASeq.ConvertToString
                        Next


                        If IsNothing(feat2) = True Then
                            out = "Ins(" & Pos & ")" & out & ";"
                        Else
                            Dim F As String = ""
                            Dim S As String = ""
                            For Each b In x
                                F = F & b.First.AASeq.ConvertToString
                                S = S & b.Second.AASeq.ConvertToString
                            Next
                            out = F & Pos & S & ";"


                        End If
                    Else ' IsFirst = false
                        For Each b In x
                            out = out & b.Second.AASeq.ConvertToString
                        Next


                        If IsNothing(feat2) = True Then
                            out = "Ins(" & Pos & ")" & out & ";"
                        Else
                            Dim F As String = ""
                            Dim S As String = ""
                            For Each b In x
                                F = F & b.Second.AASeq.ConvertToString
                                S = S & b.First.AASeq.ConvertToString
                            Next
                            out = F & Pos & S & ";"


                        End If
                    End If
                    Return out
                End Function

                Public Shared Function Get_Counts(aA_Changes As List(Of AA_Change), v As Boolean, feat2 As FeatureItem, matrixes As List(Of SimilarityMatrix)) As Object
                    Throw New NotImplementedException()
                End Function
            End Class

            Public Class SNP_MNP_INDEL
                Public Overrides Function ToString() As String
                    Dim str As New System.Text.StringBuilder
                    str.Append([Enum].GetName(GetType(Variation_Type), Me.Type)).Append(vbTab)
                    str.Append(Szunyi.Location.Common.GetLocationString(Me.First_Location)).Append(vbTab)
                    str.Append(Szunyi.Location.Common.GetLocationString(Me.Second_Location)).Append(vbTab)
                    str.Append(Szunyi.Text.General.GetText(Me.First_Bytes)).Append(vbTab)
                    str.Append(Szunyi.Text.General.GetText(Me.Second_Bytes)).Append(vbTab)
                    Return str.ToString
                End Function

                Public Sub New(first_Location As ILocation, second_Location As ILocation, common_Location As ILocation, firstByte As Byte, secondByte As Byte, Variant_Type As Variation_Type)
                    Me.First_Location = first_Location
                    Me.Second_Location = second_Location
                    Me.Common_Location = common_Location
                    Me.First_Byte = firstByte
                    Me.Second_Byte = secondByte
                    Me.Type = Variant_Type
                End Sub

                Public Property First_Location As Bio.IO.GenBank.ILocation
                Public Property Second_Location As Bio.IO.GenBank.ILocation
                Public Property Common_Location As Bio.IO.GenBank.ILocation
                Public Property First_Byte As Byte
                Public Property Second_Byte As Byte
                Public Property Type As Variation_Type
                Public Property First_Bytes As New List(Of Byte)
                Public Property Second_Bytes As New List(Of Byte)
                Public Property AA_Changes As New List(Of AA_Change)
                Public Property IsInRepeatRegion As Boolean = False
                Public Property ContainRepeatUnit As Boolean = False
                Public Sub New(First_Snp As SNP_MNP_INDEL, Second_SNP As SNP_MNP_INDEL, FirstBytes As List(Of Byte), SecondBytes As List(Of Byte))
                    If First_Snp Is Second_SNP Then
                        Me.First_Location = First_Snp.First_Location
                        Me.Second_Location = First_Snp.Second_Location
                        Me.Common_Location = First_Snp.Common_Location
                        Me.First_Byte = First_Snp.First_Byte
                        Me.Second_Byte = First_Snp.Second_Byte
                        Me.First_Bytes.Add(Me.First_Byte)
                        Me.Second_Bytes.Add(Me.Second_Byte)
                        Me.Type = First_Snp.Type

                    Else
                        ' Merging
                        Me.First_Bytes = FirstBytes
                        Me.Second_Bytes = SecondBytes
                        If First_Snp.Type = Variation_Type.SNP Then
                            Me.Type = Variation_Type.MNP
                        Else
                            Me.Type = First_Snp.Type
                        End If
                        Me.First_Location = Szunyi.Location.Common.GetLocation(First_Snp.First_Location.LocationStart, Second_SNP.First_Location.LocationEnd, "+")
                        Me.Second_Location = Szunyi.Location.Common.GetLocation(First_Snp.Second_Location.LocationStart, Second_SNP.Second_Location.LocationEnd, "+")
                        Me.Common_Location = Szunyi.Location.Common.GetLocation(First_Snp.Common_Location.LocationStart, Second_SNP.Common_Location.LocationEnd, "+")

                    End If
                End Sub


            End Class

            Public Class AA_Change
                Public Property First As Codon
                Public Property Second As Codon
                Public Property Matrixes As List(Of Bio.SimilarityMatrices.SimilarityMatrix)
                Public Property MatrixValues As New List(Of Integer)
                Public Sub New(f As Codon, s As Codon, Matrixes As List(Of Bio.SimilarityMatrices.SimilarityMatrix))
                    Me.First = f
                    Me.Second = s
                    Me.Matrixes = Matrixes
                    If IsNothing(f) = False AndAlso IsNothing(s) = False Then
                        For Each Matrixq In Matrixes
                            MatrixValues.Add(Matrixq.Item(f.AASeq.First, s.AASeq.First))
                        Next
                    End If
                End Sub
            End Class

            Public Enum Variation_Type
                SNP = 1
                MNP = 2
                INS = 3
                Del = 4

            End Enum
        End Namespace

    End Namespace
End Namespace

