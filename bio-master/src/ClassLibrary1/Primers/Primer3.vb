Imports System.IO
Imports System.Text
Imports Bio
Imports Bio.IO.GenBank
Imports ClassLibrary1.Szunyi.Sequences

Namespace Szunyi
    Namespace Primers
        Public Class Constants
            Public Const SEQUENCE_ID = "SEQUENCE_ID="
            Public Const SEQUENCE_TEMPLATE = "SEQUENCE_TEMPLATE="
            Public Const PRIMER_PICK_LEFT = "PRIMER_PICK_LEFT="
            Public Const PRIMER_PICK_RIGHT = "PRIMER_PICK_RIGHT="
            Public Const PRIMER_OPT_SIZE = "PRIMER_OPT_SIZE="
            Public Const PRIMER_MIN_SIZE = "PRIMER_MIN_SIZE="
            Public Const PRIMER_MAX_SIZE = "PRIMER_MAX_SIZE="
            Public Const PRIMER_MAX_NS_ACCEPTED = "PRIMER_MAX_NS_ACCEPTED="
            Public Const PRIMER_PRODUCT_SIZE_RANGE = "PRIMER_PRODUCT_SIZE_RANGE="
            Public Const SEQUENCE_TARGET = "SEQUENCE_TARGET="
            Public Const PRIMER_PICK_ANYWAY = "PRIMER_PICK_ANYWAY="
            Public Const PRIMER_NUM_RETURN = "PRIMER_NUM_RETURN="
            Public Const SEQUENCE_FORCE_LEFT_START = "SEQUENCE_FORCE_LEFT_START="
            Public Const SEQUENCE_FORCE_LEFT_END = "SEQUENCE_FORCE_LEFT_END="
            Public Const SEQUENCE_FORCE_RIGHT_START = "SEQUENCE_FORCE_RIGHT_START="
            Public Const SEQUENCE_FORCE_RIGHT_END = "SEQUENCE_FORCE_RIGHT_END= "
            Public Const PRIMER_THERMODYNAMIC_PARAMETERS_PATH = "PRIMER_THERMODYNAMIC_PARAMETERS_PATH="
        End Class
        Public Class Basic_Primer_Design_Properties
            Public Property PRIMER_PICK_LEFT As Integer
            Public Property PRIMER_PICK_RIGHT As Integer
            Public Property PRIMER_OPT_SIZE As Integer
            Public Property PRIMER_MIN_SIZE As Integer
            Public Property PRIMER_MAX_SIZE As Integer
            Public Property PRIMER_MAX_NS_ACCEPTED As Integer
            Public Property PRIMER_PRODUCT_SIZE_RANGE As String
            Public Property PRIMER_PRODUCT_SIZE_MIN As Integer
            Public Property PRIMER_PRODUCT_SIZE_MAX As Integer
            Public Property SEQUENCE_TARGET As String
            Public Property SEQUENCE_FORCE_LEFT_START As Integer
            Public Property SEQUENCE_FORCE_LEFT_END As Integer
            Public Property SEQUENCE_FORCE_RIGHT_START As Integer
            Public Property SEQUENCE_FORCE_RIGHT_END As Integer
            Public Sub New(Type As Szunyi.Constants.Primer_Design)
                Select Case Type
                    Case Szunyi.Constants.Primer_Design.RT_PCR
                        PRIMER_PICK_LEFT = 1
                        PRIMER_PICK_RIGHT = 1
                        PRIMER_OPT_SIZE = 18
                        PRIMER_MIN_SIZE = 16
                        PRIMER_MAX_SIZE = 22
                        PRIMER_MAX_NS_ACCEPTED = 0
                        PRIMER_PRODUCT_SIZE_RANGE = "70-160"
                        PRIMER_PRODUCT_SIZE_MIN = 80
                        PRIMER_PRODUCT_SIZE_MAX = 150
                        SEQUENCE_TARGET = "1,50"
                    Case Szunyi.Constants.Primer_Design.Variant_From_Table
                        PRIMER_PICK_LEFT = 1
                        PRIMER_PICK_RIGHT = 1
                        PRIMER_OPT_SIZE = 18
                        PRIMER_MIN_SIZE = 16
                        PRIMER_MAX_SIZE = 22
                        PRIMER_MAX_NS_ACCEPTED = 0

                End Select
            End Sub

        End Class

        Public Class RTPCR
                Public Primer_Design_Paramaters As Basic_Primer_Design_Properties
                Public Path As String
            Public Sub New(Seqs As List(Of Bio.ISequence))
                '   Path = Szunyi.IO.Files.GetFolder("Select primer3 Folder")
                Me.Primer_Design_Paramaters = New Basic_Primer_Design_Properties(Szunyi.Constants.Primer_Design.RT_PCR)
                CreateInputFile(Seqs)

            End Sub
            Private Sub CreateInputFile(Seqs As List(Of Bio.ISequence))
                Dim str As New System.Text.StringBuilder
                For Each Seq In Seqs
                    str.Append(Constants.SEQUENCE_ID)
                    str.Append(Seq.ID).AppendLine()
                    str.Append(Constants.SEQUENCE_TEMPLATE)
                    str.Append(Szunyi.Sequences.SequenceManipulation.SeqsToString.GetSeqAsString(Seq)).AppendLine()
                    str.Append(Constants.PRIMER_MAX_NS_ACCEPTED).Append(Me.Primer_Design_Paramaters.PRIMER_MAX_NS_ACCEPTED).AppendLine()
                    str.Append(Constants.PRIMER_MAX_SIZE).Append(Me.Primer_Design_Paramaters.PRIMER_MAX_SIZE).AppendLine()
                    str.Append(Constants.PRIMER_MIN_SIZE).Append(Me.Primer_Design_Paramaters.PRIMER_MIN_SIZE).AppendLine()
                    str.Append(Constants.PRIMER_OPT_SIZE).Append(Me.Primer_Design_Paramaters.PRIMER_OPT_SIZE).AppendLine()
                    str.Append(Constants.PRIMER_PICK_LEFT).Append(Me.Primer_Design_Paramaters.PRIMER_PICK_LEFT).AppendLine()
                    str.Append(Constants.PRIMER_PICK_RIGHT).Append(Me.Primer_Design_Paramaters.PRIMER_PICK_RIGHT).AppendLine()
                    str.Append(Constants.PRIMER_PRODUCT_SIZE_RANGE).Append(Me.Primer_Design_Paramaters.PRIMER_PRODUCT_SIZE_RANGE).AppendLine()
                    '    str.Append(Constants.SEQUENCE_TARGET).Append(Me.SEQUENCE_TARGET).AppendLine()
                    '    str.Append(Constants.PRIMER_PICK_ANYWAY).Append("1").AppendLine()
                    str.Append(Constants.PRIMER_NUM_RETURN).Append("1").AppendLine()
                    str.Append("=").AppendLine() ' End Of The 

                Next
                Szunyi.IO.Export.SaveText(str.ToString)
            End Sub
            Public Sub DoIt()
                Dim consoleApp As New Process
                With consoleApp
                    .EnableRaisingEvents = True
                    .StartInfo.FileName = Path & "\primer3core.exe"
                    .StartInfo.RedirectStandardError = True
                    .StartInfo.UseShellExecute = False
                    .StartInfo.CreateNoWindow = True
                    .StartInfo.WindowStyle = ProcessWindowStyle.Hidden

                End With
                consoleApp.Start()
            End Sub
        End Class
        Public Class From_Variant_Table
            Public Property Seqs As List(Of Bio.ISequence)
            Public Property Files As List(Of FileInfo)
            Public Property Properties As From_Variant_Table_Properties = New From_Variant_Table_Properties
            Public Property Primer_Design_Properties = New Basic_Primer_Design_Properties(Szunyi.Constants.Primer_Design.Variant_From_Table)
            Dim tmpSeq As New Bio.Sequence(Alphabets.DNA, "A")
            Public Sub New(Seqs As List(Of Bio.ISequence), Files As List(Of FileInfo))
                Me.Seqs = Seqs
                Me.Files = Files

                For Each File In Files
                    Dim Primer3_Scripts As New List(Of String)
                    Dim V_i_G = Szunyi.Sequences.Variants.Variant_Manipulation.Import(File)
                    Dim OutSeqs As New List(Of Bio.ISequence)
                    For Each Chromosome In V_i_G.Variants_By_Chromosomes.Values
                        For i1 = Chromosome.Variants.Count - 1 To 1 Step -1
                            If Chromosome.Variants(i1).Position - Chromosome.Variants(i1 - 1).Position <= Me.Properties.Max_Product_Size AndAlso
                               Chromosome.Variants(i1).Position - Chromosome.Variants(i1 - 1).Position >= Me.Properties.Min_Product_Size Then
                                tmpSeq.ID = Chromosome.SeqID
                                Dim Index = Seqs.BinarySearch(tmpSeq, Szunyi.Comparares.AllComparares.BySeqID)
                                If Index > -1 Then
                                    OutSeqs.AddRange(Szunyi.Sequences.Variants.Variant_Manipulation.Get_Original_And_Modified_Seqs(Chromosome.Variants(i1 - 1), Chromosome.Variants(i1), Seqs(Index), Me.Properties.Max_Primer_Length))

                                    Primer3_Scripts.Add(Get_Primer3_Script(Chromosome.Variants(i1 - 1),
                                                  Chromosome.Variants(i1), Seqs(Index)))
                                End If
                            End If
                        Next
                    Next
                    Dim Primer3_Result_Strict = Szunyi.Outer_Programs.Primer3.Get_Results(Primer3_Scripts)
                    Dim Primer3File = Szunyi.IO.Files.Get_New_FileName.GetNewFile(File, " Strict.primer3")
                    Szunyi.IO.Export.SaveText(Primer3_Result_Strict, Primer3File)

                    Dim Primer3_Script_Flexible = Szunyi.Text.Lists.Replace(Primer3_Scripts, "PRIMER_PICK_ANYWAY=1" & vbCrLf, "")
                    Dim Primer3_Result_Flexible = Szunyi.Outer_Programs.Primer3.Get_Results(Primer3_Script_Flexible)
                    Dim Primer3FlexibleFile = Szunyi.IO.Files.Get_New_FileName.GetNewFile(File, " Flexible.primer3")
                    Szunyi.IO.Export.SaveText(Primer3_Result_Flexible, Primer3FlexibleFile)


                    Dim Seq_File = Szunyi.IO.Files.Get_New_FileName.GetNewFile(File, ".fa")
                    Szunyi.IO.Export.SaveSequencesToSingleFasta(OutSeqs, Seq_File)

                Next ' Files 

            End Sub

            Private Function Get_Primer3_Script(SV_First As Variants.Single_Variant, SV_Second As Variants.Single_Variant, Seq As Sequence) As String
                ' Get Ext Seq
                ' Get Positions
                Dim MdSeq As Bio.Sequence = Szunyi.Sequences.Variants.Variant_Manipulation.Get_Md_Sequence(SV_First, SV_Second, Seq, Me.Properties.Max_Primer_Length)
                Dim str As New System.Text.StringBuilder
                str.Append(Constants.SEQUENCE_ID)
                str.Append(Seq.ID & " " & SV_First.Position & "-" & SV_Second.Position).AppendLine()
                str.Append(Constants.SEQUENCE_TEMPLATE)
                str.Append(MdSeq.ConvertToString).AppendLine()
                str.Append(Constants.PRIMER_MAX_NS_ACCEPTED).Append(Me.Primer_Design_Properties.PRIMER_MAX_NS_ACCEPTED).AppendLine()
                str.Append(Constants.PRIMER_MAX_SIZE).Append(Me.Primer_Design_Properties.PRIMER_MAX_SIZE).AppendLine()
                str.Append(Constants.PRIMER_MIN_SIZE).Append(Me.Primer_Design_Properties.PRIMER_MIN_SIZE).AppendLine()
                str.Append(Constants.PRIMER_OPT_SIZE).Append(Me.Primer_Design_Properties.PRIMER_OPT_SIZE).AppendLine()
                str.Append(Constants.PRIMER_PICK_LEFT).Append(Me.Primer_Design_Properties.PRIMER_PICK_LEFT).AppendLine()
                str.Append(Constants.PRIMER_PICK_RIGHT).Append(Me.Primer_Design_Properties.PRIMER_PICK_RIGHT).AppendLine()
                str.Append(Constants.PRIMER_PICK_ANYWAY).Append("1").AppendLine()
                str.Append(Constants.SEQUENCE_FORCE_LEFT_END).Append(Me.Properties.Max_Primer_Length).AppendLine()
                str.Append(Constants.SEQUENCE_FORCE_RIGHT_END).Append(MdSeq.Count - Me.Properties.Max_Primer_Length).AppendLine()
                str.Append(Constants.PRIMER_THERMODYNAMIC_PARAMETERS_PATH).Append(My.Resources.Other_Progrmas).Append("primer3_config\").AppendLine()
                str.Append(Constants.PRIMER_PRODUCT_SIZE_RANGE).Append(MdSeq.Count - Me.Properties.Max_Primer_Length * 2)
                str.Append("-").Append(MdSeq.Count + Me.Properties.Max_Primer_Length).AppendLine()


                str.Append(Constants.PRIMER_NUM_RETURN).Append("1").AppendLine()
                str.Append("=").AppendLine() ' End Of The Seq


                Return str.ToString

            End Function
        End Class
        Public Class From_Variant_Table_Properties
            Public Property Max_Primer_Length As Integer = 25
            Public Property Min_Product_Size As Integer = 80
            Public Property Max_Product_Size As Integer = 1500
            Public Property Min_Count As Integer = 10
            Public Property Min_Frequency As Double = 0.9
            Public Property Include_Variants_In_Both_Primer As Boolean = True
        End Class
        Public Class Primer3Results
            Public Property Files As New List(Of FileInfo)
            Public Sub New(File As FileInfo)
                Files.Add(File)
            End Sub
            Public Sub New(Files As List(Of FileInfo))
                Me.Files = Files
            End Sub
            Public Function GetResult() As List(Of Primer3Result)
                Dim Res As New List(Of Primer3Result)
                For Each File In Files
                    Dim txt = Szunyi.IO.Import.Text.ReadToEnd(File)
                    Dim s1 = Split(txt, "=" & vbCrLf)
                    For Each s In s1
                        If s.IndexOf("PRIMER_LEFT_0_SEQUENCE") > -1 Then
                            Res.Add(New Primer3Result(s))
                        End If

                    Next
                Next

                Return Res
            End Function
            Public Shared Function GetResult(FIles As List(Of FileInfo)) As List(Of Primer3Result)
                Dim Res As New List(Of Primer3Result)
                For Each File In FIles
                    Dim txt = Szunyi.IO.Import.Text.ReadToEnd(File)
                    Dim s1 = Split(txt, "=" & vbCrLf)
                    For Each s In s1
                        If s.IndexOf("PRIMER_LEFT_0_SEQUENCE") > -1 Then
                            Res.Add(New Primer3Result(s))
                        End If
                    Next
                Next

                Return Res
            End Function
            Public Shared Function GetResult(File As FileInfo) As List(Of Primer3Result)
                Dim Res As New List(Of Primer3Result)
                Dim txt = Szunyi.IO.Import.Text.ReadToEnd(File)
                Dim s1 = Split(txt, "=" & vbCrLf)
                    For Each s In s1
                        If s.IndexOf("PRIMER_LEFT_0_SEQUENCE") > -1 Then
                            Res.Add(New Primer3Result(s))
                        End If
                    Next

                Return Res
            End Function

            Public Shared Function Convert_to_I_w_Ps(res As List(Of Primer3Result)) As Szunyi.Text.TableManipulation.Items_With_Properties

                Dim IDs = (From h In res Select h.SEQUENCE_ID).ToList
                Dim I_w_Ps As New Szunyi.Text.TableManipulation.Items_With_Properties(IDs, False)
                Dim Seq_Templates = (From h In res Select h.SEQUENCE_TEMPLATE).ToList
                I_w_Ps.Add_Values_WithOut_Keys("Sequence Template", Seq_Templates)

                Dim Left_Primer = (From h In res Select h.PRIMER_LEFT_0_SEQUENCE).ToList
                I_w_Ps.Add_Values_WithOut_Keys("Left Primer Sequence", Left_Primer)

                Dim Right_Sequence = (From h In res Select h.PRIMER_RIGHT_0_SEQUENCE).ToList
                I_w_Ps.Add_Values_WithOut_Keys("Right Primer Sequence", Right_Sequence)

                Dim TM_Left = (From h In res Select h.PRIMER_LEFT_0_TM).ToList
                I_w_Ps.Add_Values_WithOut_Keys("Left Primer TM", TM_Left)

                Dim TM_Right = (From h In res Select h.PRIMER_RIGHT_0_TM).ToList
                I_w_Ps.Add_Values_WithOut_Keys("Right Primer TM", TM_Right)

                Dim Product_Size = (From h In res Select h.PRIMER_PAIR_0_PRODUCT_SIZE).ToList
                I_w_Ps.Add_Values_WithOut_Keys("Product Size", Product_Size)

                Return I_w_Ps
            End Function

            Public Function GetResultInTDTShortForm() As String
                Dim res = GetResult()
                Dim str As New System.Text.StringBuilder
                str.Append(GetShortHeader)
                For Each p3Result In res
                    str.Append(GetShort(p3Result))
                Next
                str.Length -= 2
                Return str.ToString
            End Function

            Private Function GetShort(p3Result As Primer3Result) As String
                Dim str As New StringBuilder
                str.Append(p3Result.SEQUENCE_ID).Append(vbTab)
                str.Append(p3Result.SEQUENCE_TEMPLATE).Append(vbTab)
                str.Append(p3Result.PRIMER_RIGHT_0_SEQUENCE).Append(vbTab)
                str.Append(p3Result.PRIMER_LEFT_0_SEQUENCE).Append(vbTab)
                str.Append(p3Result.PRIMER_RIGHT_0_TM).Append(vbTab)
                str.Append(p3Result.PRIMER_LEFT_0_TM).Append(vbTab)
                str.Append(p3Result.PRIMER_PAIR_0_PRODUCT_SIZE).AppendLine()
                Return str.ToString

            End Function

            Private Function GetShortHeader()
                Dim str As New System.Text.StringBuilder
                str.Append("SeqID").Append(vbTab)
                str.Append("Seq").Append(vbTab)
                str.Append("Primer 5' Seq").Append(vbTab)
                str.Append("Primer 3' Seq").Append(vbTab)
                str.Append("Primer 5' TM").Append(vbTab)
                str.Append("Primer 3' TM").Append(vbTab)
                str.Append("Product Length").AppendLine()
                Return str.ToString
            End Function
        End Class
        Public Class Primer3Result
            Public Property PRIMER_PAIR_0_PENALTY As Double
            Public Property PRIMER_LEFT_0_PENALTY As Double
            Public Property PRIMER_RIGHT_0_PENALTY As Double
            Public Property PRIMER_LEFT_0_SEQUENCE As String
            Public Property PRIMER_RIGHT_0_SEQUENCE As String
            Public Property PRIMER_LEFT_0 As Double
            Public Property PRIMER_RIGHT_0 As Double
            Public Property PRIMER_LEFT_0_TM As Double
            Public Property PRIMER_RIGHT_0_TM As Double
            Public Property PRIMER_LEFT_0_GC_PERCENT As Double
            Public Property PRIMER_RIGHT_0_GC_PERCENT As Double
            Public Property PRIMER_LEFT_0_SELF_ANY_TH As Double
            Public Property PRIMER_RIGHT_0_SELF_ANY_TH As Double
            Public Property PRIMER_LEFT_0_SELF_END_TH As Double
            Public Property PRIMER_RIGHT_0_SELF_END_TH As Double
            Public Property PRIMER_LEFT_0_HAIRPIN_TH As Double
            Public Property PRIMER_RIGHT_0_HAIRPIN_TH As Double
            Public Property PRIMER_LEFT_0_END_STABILITY As Double
            Public Property PRIMER_RIGHT_0_END_STABILITY As Double
            Public Property PRIMER_PAIR_0_COMPL_ANY_TH As Double
            Public Property PRIMER_PAIR_0_COMPL_END_TH As Double
            Public Property PRIMER_PAIR_0_PRODUCT_SIZE As Double
            Public Property SEQUENCE_ID As String
            Public Property SEQUENCE_TEMPLATE As String
            Public Sub New(s As String)
                Dim s1() = Split(s, vbCrLf)
                If GetValue(s1, "PRIMER_LEFT_0_SEQUENCE") <> "" Then
                    PRIMER_PAIR_0_PENALTY = GetValue(s1, "PRIMER_PAIR_0_PENALTY")
                    PRIMER_LEFT_0_PENALTY = GetValue(s1, "PRIMER_LEFT_0_PENALTY")
                    PRIMER_RIGHT_0_PENALTY = GetValue(s1, "PRIMER_RIGHT_0_PENALTY")
                    PRIMER_LEFT_0_SEQUENCE = GetValue(s1, "PRIMER_LEFT_0_SEQUENCE")
                    PRIMER_RIGHT_0_SEQUENCE = GetValue(s1, "PRIMER_RIGHT_0_SEQUENCE")
                    PRIMER_LEFT_0 = GetValue(s1, "PRIMER_LEFT_0")
                    PRIMER_RIGHT_0 = GetValue(s1, "PRIMER_RIGHT_0")
                    PRIMER_LEFT_0_TM = GetValue(s1, "PRIMER_LEFT_0_TM")
                    PRIMER_RIGHT_0_TM = GetValue(s1, "PRIMER_RIGHT_0_TM")
                    PRIMER_LEFT_0_GC_PERCENT = GetValue(s1, "PRIMER_LEFT_0_GC_PERCENT")
                    PRIMER_RIGHT_0_GC_PERCENT = GetValue(s1, "PRIMER_RIGHT_0_GC_PERCENT")
                    PRIMER_LEFT_0_SELF_ANY_TH = GetValue(s1, "PRIMER_LEFT_0_SELF_ANY_TH")
                    PRIMER_RIGHT_0_SELF_ANY_TH = GetValue(s1, "PRIMER_RIGHT_0_SELF_ANY_TH")
                    PRIMER_LEFT_0_SELF_END_TH = GetValue(s1, "PRIMER_LEFT_0_SELF_END_TH")
                    PRIMER_RIGHT_0_SELF_END_TH = GetValue(s1, "PRIMER_RIGHT_0_SELF_END_TH")
                    PRIMER_LEFT_0_HAIRPIN_TH = GetValue(s1, "PRIMER_LEFT_0_HAIRPIN_TH")
                    PRIMER_RIGHT_0_HAIRPIN_TH = GetValue(s1, "PRIMER_RIGHT_0_HAIRPIN_TH")
                    PRIMER_LEFT_0_END_STABILITY = GetValue(s1, "PRIMER_LEFT_0_END_STABILITY")
                    PRIMER_RIGHT_0_END_STABILITY = GetValue(s1, "PRIMER_RIGHT_0_END_STABILITY")
                    PRIMER_PAIR_0_COMPL_ANY_TH = GetValue(s1, "PRIMER_PAIR_0_COMPL_ANY_TH")
                    PRIMER_PAIR_0_COMPL_END_TH = GetValue(s1, "PRIMER_PAIR_0_COMPL_END_TH")
                    PRIMER_PAIR_0_PRODUCT_SIZE = GetValue(s1, "PRIMER_PAIR_0_PRODUCT_SIZE")
                    SEQUENCE_ID = GetValue(s1, "SEQUENCE_ID")
                    SEQUENCE_TEMPLATE = GetValue(s1, "SEQUENCE_TEMPLATE")
                End If

            End Sub
            Private Function GetValue(s1 As String(), txtToSearch As String) As String
                Dim res = From s In s1 Where s.IndexOf(txtToSearch, StringComparison.InvariantCultureIgnoreCase) > -1

                If res.Count > 0 Then

                    Return Split(res.First, "=").Last

                Else

                    Return String.Empty
                End If
            End Function
        End Class
    End Namespace
End Namespace

