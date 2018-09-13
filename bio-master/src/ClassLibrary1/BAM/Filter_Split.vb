Imports System.IO
Imports Bio.IO.SAM
Imports ClassLibrary1.Szunyi.Outer_Programs

Namespace Szunyi.BAM

    Public Class Filter_Split
        Public Shared Function IsGood_By_Position(SAm As SAMAlignedSequence, ADs As Dictionary(Of Szunyi.Transcipts.Adaptor_Pair, Szunyi.Transcipts.Adapter_Filtering)) As Boolean

            For Each AD In ADs
                Dim cData = Szunyi.BAM.BAM_Optional_Filed_Manipulation.Get_Adapter_Value(SAm, AD.Key)
                ' Three Prime Adaptor

                If AD.Value.Three_Prime_Adaptor_Score <= cData.Three_Prime_Score Then ' Score
                    If AD.Value.Three_Prime_Query_Minimal_Position <= cData.Three_Prime_Pos AndAlso AD.Value.Three_Prime_Query_Maximal_Position >= cData.Three_Prime_Pos Then ' Query
                        If AD.Value.Three_Prime_Adaptor_Minimal_Position <= cData.Three_Prime_Pos_in_Adapter AndAlso AD.Value.Three_Prime_Adaptor_Maximal_Position >= cData.Three_Prime_Pos_in_Adapter Then
                            Return True
                        End If
                    End If
                End If




                If AD.Value.Five_Prime_Adaptor_Score <= cData.Five_Prime_Score Then ' Score
                    If AD.Value.Five_Prime_Query_Minimal_Position <= cData.Five_Prime_Pos AndAlso AD.Value.Five_Prime_Query_Maximal_Position >= cData.Five_Prime_Pos Then ' Query
                        If AD.Value.Five_Prime_Adaptor_Minimal_Position <= cData.Five_Prime_Pos_in_Adapter AndAlso AD.Value.Five_Prime_Adaptor_Maximal_Position >= cData.Five_Prime_Pos_in_Adapter Then
                            Return True
                        End If
                    End If
                End If


            Next
            Return False
        End Function

        Private Shared Function Get_Score_By_CIGAR(SAM As SAMAlignedSequence) As Double
            Dim x = CIGAR.Get_CIGARS(SAM)
            Dim score = CIGAR.Get_Cigar_Match_Length(SAM.CIGAR) / CIGAR.Get_CIGAR_Full_Length(SAM.CIGAR)

            Return score
        End Function
        Public Shared Function Get_Score_By_MD(SAM As SAMAlignedSequence) As Double
            Dim MD = Szunyi.BAM.BAM_Optional_Filed_Manipulation.Get_MD(SAM)
            Dim r As New M_Ins_Del(MD)
            Return r.Matches

        End Function
        ''' <summary>
        ''' 
        ''' </summary>
        ''' <param name="SAMs"></param>
        ''' <returns></returns>
        Public Shared Function Get_Best(SAMs As List(Of SAMAlignedSequence)) As SAMAlignedSequence
            Dim Best As SAMAlignedSequence
            Best = SAMs.First
            Dim score = Get_Score_By_MD(Best)

            For i1 = 1 To SAMs.Count - 1
                Dim sc = Get_Score_By_MD(SAMs(i1))
                If sc > score Then
                    score = sc
                    Best = SAMs(i1)
                End If
            Next
            Return Best
        End Function
        Public Shared Function Get_Bests_(byReads As List(Of SAMAlignedSequence)) As List(Of SAMAlignedSequence)
            Dim Best As SAMAlignedSequence
            Best = byReads.First
            Dim score = Get_Score_By_MD(Best)
            Dim out As New List(Of SAMAlignedSequence)
            out.Add(Best)
            For i1 = 1 To byReads.Count - 1
                Dim sc = Get_Score_By_MD(byReads(i1))
                If sc > score Then
                    score = sc
                    out.Clear()
                    out.Add(byReads(i1))
                ElseIf sc = score Then
                    out.Add(byReads(i1))
                End If
            Next
            Return out
        End Function

        Public Shared Function Get_Bests_From_Groupby_ReadID(SAMS As List(Of SAMAlignedSequence)) As List(Of SAMAlignedSequence)
            Dim out As New List(Of SAMAlignedSequence)
            For Each gr In Szunyi.BAM.GroupBy.ReadID(SAMS)
                out.Add(Get_Best(gr))
            Next
            Return out
        End Function

        Public Shared Iterator Function ByOrg(SAMS As List(Of SAMAlignedSequence)) As IEnumerable(Of List(Of SAMAlignedSequence))
            For Each gr In Szunyi.BAM.GroupBy.RNameID(SAMS)
                Yield gr
            Next
        End Function
        Public Shared Function Get_File_By_ORG(sam As SAMAlignedSequence, BySeqIDs As Dictionary(Of FileInfo, List(Of String))) As KeyValuePair(Of FileInfo, List(Of String))
            For Each BySeq In BySeqIDs
                BySeq.Value.Sort()
                Dim Index = BySeq.Value.BinarySearch(sam.RName)
                If Index > -1 Then Return BySeq
            Next
            Return Nothing
        End Function

        Public Shared Function Strand_By_Flag(SAMs As List(Of SAMAlignedSequence)) As Dictionary(Of String, List(Of SAMAlignedSequence))
            Dim Out As New Dictionary(Of String, List(Of SAMAlignedSequence))
            Out.Add("+", New List(Of SAMAlignedSequence))
            Out.Add("-", New List(Of SAMAlignedSequence))
            For Each Sam In SAMs
                If Sam.Flag = SAMFlags.QueryOnReverseStrand = True Then
                    Out("-").Add(Sam)
                Else
                    Out("+").Add(Sam)
                End If
            Next
            Return Out
        End Function

        Public Shared Function Strand_By_FO(SAMs As List(Of SAMAlignedSequence)) As Dictionary(Of String, List(Of SAMAlignedSequence))
            Dim Out As New Dictionary(Of String, List(Of SAMAlignedSequence))
            Out.Add("+", New List(Of SAMAlignedSequence))
            Out.Add("-", New List(Of SAMAlignedSequence))

            Dim t = Szunyi.BAM.BAM_Optional_Filed_Manipulation.Get_Orientation(SAMs)
            For Each Sam In t
                If Sam.Flag = SAMFlags.QueryOnReverseStrand = True Then
                    Out("-").Add(Sam)
                Else
                    Out("+").Add(Sam)
                End If
            Next
            Return Out
        End Function
        Public Shared Function Strand_By_PA(SAMs As List(Of SAMAlignedSequence)) As Dictionary(Of String, List(Of SAMAlignedSequence))
            Dim Out As New Dictionary(Of String, List(Of SAMAlignedSequence))
            Out.Add("+", New List(Of SAMAlignedSequence))
            Out.Add("-", New List(Of SAMAlignedSequence))

            Dim t = Szunyi.BAM.BAM_Optional_Filed_Manipulation.Get_PolyAs(SAMs)
            For Each Sam In t
                If Sam.Flag = SAMFlags.QueryOnReverseStrand = True Then
                    Out("-").Add(Sam)
                Else
                    Out("+").Add(Sam)
                End If
            Next
            Return Out
        End Function
        Public Shared Sub Remove_HC_Duplicated_Max_Intron(Files As List(Of FileInfo), maxIntronLength As Integer, SoftClip As Integer)
            For Each FIle In Files
                Dim Header = Szunyi.BAM.Bam_Basic_IO.Headers.Get_Header(FIle)
                Dim nFile = Szunyi.IO.Files.Get_New_FileName.Append_Before_Extension(FIle, "_No_HC_Unique_mIntron" & maxIntronLength & "Sc_" & SoftClip & ".sam")
                Dim out As New List(Of SAMAlignedSequence)
                For Each SAM In Szunyi.BAM.Bam_Basic_IO.Import.Parse(FIle)
                    If Szunyi.BAM.SAM_Manipulation.Location.HasHardClip(SAM) = False Then
                        If Szunyi.BAM.CIGAR.Has_Longer_Insertion(SAM, maxIntronLength) = False Then
                            out.Add(SAM)
                        End If
                    End If
                Next
                Dim Un = Szunyi.BAM.GroupBy_Firsts.ByPos_Cigar_Flag(out)
                Dim Un_S = Szunyi.BAM.Trim.TrimS_S(Un, SoftClip)
                Szunyi.IO.Export.Export_Sam(Un_S, Header, nFile)
            Next

        End Sub
        Public Shared Function Remove_HC(File As FileInfo, IsTest As Boolean) As String
            If IsTest = True Then
                Dim HC As Integer = 0

                For Each Sam In Szunyi.BAM.Bam_Basic_IO.Import.Parse(File)
                    If Szunyi.BAM.SAM_Manipulation.Location.HasHardClip(Sam) = True Then
                        HC += 1
                    End If

                Next
                Return File.Name & vbTab & HC
            Else
                Dim HC As Integer = 0

                Dim Header = Szunyi.BAM.Bam_Basic_IO.Headers.Get_Header(File)
                Dim nFile = Szunyi.IO.Files.Get_New_FileName.Append_Before_Extension(File, "_No_HC")
                Using x As New Szunyi.BAM.Bam_Basic_IO.Export(nFile, Header)
                    For Each Sam In Szunyi.BAM.Bam_Basic_IO.Import.Parse(File)
                        If Szunyi.BAM.SAM_Manipulation.Location.HasHardClip(Sam) = False Then
                            x.Write(Sam)
                        Else
                            HC += 1
                        End If

                    Next
                End Using
                Return File.Name & vbTab & HC
            End If


        End Function
        Public Shared Function Remove_HC(Files As List(Of FileInfo), IsTest As Boolean) As String
            Dim str As New System.Text.StringBuilder

            For Each FIle In Files
                str.Append(Remove_HC(FIle, IsTest)).AppendLine()
            Next
            Return str.ToString
        End Function
        Public Shared Function Remove_Duplicated_by_Position(file As FileInfo, IsTest As Boolean) As String
            Dim str As New System.Text.StringBuilder
            Dim str_Header As New System.Text.StringBuilder
            Dim Header = Szunyi.BAM.Bam_Basic_IO.Headers.Get_Header(file)
            str_Header.Append(file.Name).AppendLine()
            str_Header.Append(Szunyi.BAM.Bam_Basic_IO.Headers.Get_Header_s(Header))
            Dim Sams = Szunyi.BAM.Bam_Basic_IO.Import.Parse(file).ToList
            Dim Duplicated As New List(Of Bio.IO.SAM.SAMAlignedSequence)
            Dim Unique As New List(Of Bio.IO.SAM.SAMAlignedSequence)

            For Each ByReads In Szunyi.BAM.GroupBy.ReadID(Sams)

                If ByReads.Count = 1 Then
                    Unique.Add(ByReads.First)
                Else
                    For Each ByPos In Szunyi.BAM.GroupBy.ByPos_Cigar_Flag(ByReads)
                        If ByPos.Count > 1 Then
                            Duplicated.Add(ByPos.First)
                            Unique.Add(ByPos.First)
                        Else
                            Unique.Add(ByPos.First)
                        End If
                    Next
                End If

            Next
            str.Append(file.Name).Append(vbTab).Append(Sams.Count).Append(vbTab).Append(Unique.Count).Append(vbTab).Append(Duplicated.Count).AppendLine()
            If IsTest = False Then
                Dim nFIle = Szunyi.IO.Files.Get_New_FileName.Append_Before_Extension(file, "_Unique")
                Szunyi.IO.Export.Export_Sam(Unique, Header, nFIle)
            End If
            Return str.ToString
        End Function
        Public Shared Function Remove_Duplicated_by_Position(Files As List(Of FileInfo), IsTest As Boolean) As String
            Dim str As New System.Text.StringBuilder

            For Each File In Files
                str.Append(Remove_Duplicated_by_Position(File, IsTest)).AppendLine()
            Next
            Return str.ToString
        End Function

        Public Shared Function Remove_HC_Duplicated_by_Position(files As List(Of FileInfo), v As Boolean) As String
            Dim str As New System.Text.StringBuilder
            str.Append(Remove_Duplicated_by_Position(files, v))
            str.AppendLine()
            str.Append(Remove_HC(files, v))
            str.AppendLine()
            Return str.ToString
        End Function

        Public Shared Function Get_Where_No_Intorns(sAMs As List(Of SAMAlignedSequence), length As Integer) As List(Of SAMAlignedSequence)
            Dim out As New List(Of SAMAlignedSequence)
            For Each Sam In sAMs
                Dim CIGARS = Szunyi.BAM.SAM_Manipulation.Common.Get_CIGARS(Sam)
                Dim tmp = From x In CIGARS Where (x.Key = "D" Or x.Key = "I" Or x.Key = "N") And x.Value >= length
                If tmp.Count = 0 Then
                    out.Add(Sam)
                Else
                    Dim jk As Int16 = 32
                End If
            Next
            Return out
        End Function

        Public Shared Sub Remove_Bad_Cigars(files As List(Of FileInfo))
            For Each File In files
                Dim nFIle = Szunyi.IO.Files.Get_New_FileName.Append_Before_Extension(File, "_GoddCigars")
                Dim x As New Szunyi.BAM.Bam_Basic_IO.Export(nFIle, Szunyi.BAM.Bam_Basic_IO.Headers.Get_Header(File))
                For Each Sam In Szunyi.BAM.Bam_Basic_IO.Import.Parse(File)
                    Dim CIGARS = Szunyi.BAM.SAM_Manipulation.Common.Get_CIGARS(Sam)
                    Dim CL = Szunyi.BAM.CIGAR.Get_CIGAR_Full_Length(Sam.CIGAR)
                    If CL <> Sam.QuerySequence.Count Then
                        Dim jk As Int16 = 43
                    Else
                        x.Write(Sam)
                    End If
                Next
            Next

        End Sub

        Public Class RemoveSetting
            Public Property Hard_Clip As Szunyi.Outer_Programs.Input_Description
            Public Property Intron_Filter As Szunyi.Outer_Programs.Input_Description
            Public Property Bad_Cigar As Szunyi.Outer_Programs.Input_Description
            Public Property Soft_Clip As Szunyi.Outer_Programs.Input_Description
            Public Property Insert_After_S As Szunyi.Outer_Programs.Input_Description
            Public Sub New()
                Hard_Clip = New Outer_Programs.Input_Description("Hard Clip", Outer_Programs.Input_Description_Type.Boolean, "Are You want to Remove All Hard Clips Mappings?", Nothing, Nothing, Nothing, Nothing, 1, "True", "HC")
                Bad_Cigar = New Outer_Programs.Input_Description("Bad CIGAR", Outer_Programs.Input_Description_Type.Boolean, "Are You want to Remove All Mappings with CIGAR inconsistence?", Nothing, Nothing, Nothing, Nothing, 1, "True", "BC")
                Intron_Filter = New Outer_Programs.Input_Description("Intron Filter", Outer_Programs.Input_Description_Type.Integer, "Are You want keep only Mappings with this intron size?", -1, 100000000, 0, 1000, 1000, "True", "mIS")
                Soft_Clip = New Outer_Programs.Input_Description("Soft Clip", Outer_Programs.Input_Description_Type.Integer, "Are You want trim Soft Clip?", -1, 100000000, 0, 1000, 50, "True", "SCt")
                Insert_After_S = New Outer_Programs.Input_Description("Not M after-before S", Outer_Programs.Input_Description_Type.Boolean, "Are You want to Remove Not M after-before S?", Nothing, Nothing, Nothing, Nothing, 1, "True", "SM")

            End Sub
        End Class

        Public Shared Sub Remove(Files As List(Of FileInfo), input_Descriptions As List(Of Input_Description))
            Dim HC = (From x In input_Descriptions Where x.Flag = "Hard Clip" Select x).First
            Dim BC = (From x In input_Descriptions Where x.Flag = "Bad CIGAR" Select x).First
            Dim I_F = (From x In input_Descriptions Where x.Flag = "Intron Filter" Select x).First
            Dim Sc_F = (From x In input_Descriptions Where x.Flag = "Soft Clip" Select x).First
            Dim M_S = (From x In input_Descriptions Where x.Flag = "Not M after-before S" Select x).First
            Dim c = HC.File_Flag & HC.Default_Value & "_" & BC.File_Flag & BC.Default_Value & "_" & I_F.File_Flag & I_F.Default_Value & "_" & Sc_F.File_Flag & Sc_F.Default_Value
            For Each File In Files
                Dim Header = Szunyi.BAM.Bam_Basic_IO.Headers.Get_Header(File)
                Header.Comments.Add("Hard clip:" & HC.Default_Value)
                Header.Comments.Add("CIGAR and sequence length are consistent:" & BC.Default_Value)
                Header.Comments.Add("Maximal Intron Size:" & I_F.Default_Value)
                Header.Comments.Add("Soft Clip Trimmed to:" & Sc_F.Default_Value)
                Dim nFIle = Szunyi.IO.Files.Get_New_FileName.Append_Before_Extension(File, c)
                Dim Out As New List(Of SAMAlignedSequence)
                Using x1 As New Szunyi.BAM.Bam_Basic_IO.Export(nFIle, Header)
                    For Each SAM In Szunyi.BAM.Bam_Basic_IO.Import.Parse(File)
                        If HC.Default_Value = False OrElse Szunyi.BAM.CIGAR.Has_Hard_Clip(SAM) = False Then
                            If M_S.Default_Value = False OrElse Szunyi.BAM.CIGAR.Is_Correct_S(SAM) = True Then
                                If BC.Default_Value = False OrElse Szunyi.BAM.CIGAR.Get_CIGAR_Full_Length(SAM.CIGAR) = SAM.QuerySequence.Count Then
                                    If I_F.Default_Value = -1 OrElse I_F.Default_Value > Szunyi.BAM.CIGAR.Get_Biggest_Intron_Length(SAM) Then
                                        If Sc_F.Default_Value <> -1 Then Szunyi.BAM.Trim.TrimS(SAM, Sc_F.Default_Value) ' Soft Clip
                                        If SAM.QuerySequence.GetType.ToString.Contains("Qual") Then
                                            Dim q As Bio.QualitativeSequence = SAM.QuerySequence
                                            If q.Valid = True Then
                                                Out.Add(SAM)
                                                '  x1.Write(SAM)
                                            Else
                                                Dim kj As Int16 = 54
                                            End If

                                        Else
                                            ' x1.Write(SAM)
                                            Out.Add(SAM)
                                        End If

                                    End If
                                End If
                            End If
                        End If
                    Next
                    Dim Un = Szunyi.BAM.GroupBy_Firsts.ByPos_Cigar_Flag(Out)
                    For Each u In Un
                        x1.Write(u)
                    Next
                End Using

            Next
        End Sub

        Public Shared Sub Remove_Comment_Lines(files As List(Of FileInfo))
            For Each File In files
                Dim Header = Szunyi.BAM.Bam_Basic_IO.Headers.Get_Header(File)
                Header.Comments.Clear()
                Dim nFIle = Szunyi.IO.Files.Get_New_FileName.Append_Before_Extension(File, "woCO")
                Using x1 As New Szunyi.BAM.Bam_Basic_IO.Export(nFIle, Header)
                    For Each SAM In Szunyi.BAM.Bam_Basic_IO.Import.Parse(File)
                        x1.Write(SAM)
                    Next

                End Using
            Next
        End Sub
    End Class
End Namespace

