Imports System.IO
Imports Bio

Namespace Szunyi
    Namespace Sequences
        Namespace Variants
            Public Class Variant_Constants

                Public Const Mapping = "Mapping"
                Public Const Position = "Position"
                Public Const Type = "Type"
                Public Const Length = "Length"
                Public Const Reference = "Reference"
                Public Const Allele = "Allele"
                Public Const Zygosity = "Zygosity"
                Public Const Count = "Count"
                Public Const Coverage = "Coverage"
                Public Const Frequency = "Frequency"
                Public Const SNV = "SNV"

                Public Const MNV = "MNV"
                Public Const Insertion = "Insertion"
                Public Const Deletion = "Deletion"
                Public Const Replacement = "Replacement"
            End Class

            Public Class Variant_Manipulation
                Public Shared Function Merge(x As List(Of Single_Variant)) As Single_Variant
                    For i1 = 1 To x.Count - 1
                        x.First.Count += x(i1).Count
                        x.First.Coverage += x(i1).Coverage
                    Next
                    x.First.Frequency = x.First.Count / x.First.Coverage * 100
                    Return x.First
                End Function
                Public Shared Function Get_All_Variants(x As List(Of Variants_In_Genome)) As List(Of Variants.Single_Variant)
                    Dim out As New List(Of Variants.Single_Variant)
                    For Each x1 In x
                        For Each x2 In x1.Variants_By_Chromosomes.Values
                            out.AddRange(x2.Variants)
                        Next
                    Next
                    Return out
                End Function
                Public Shared Function Get_All_Variants(x As Variants_In_Genome) As List(Of Variants.Single_Variant)
                    Dim out As New List(Of Variants.Single_Variant)
                    For Each x1 In x.Variants_By_Chromosomes
                        out.AddRange(x1.Value.Variants)
                    Next
                    Return out
                End Function
                Public Shared Iterator Function Get_Same_Variants(Variants As List(Of Variants.Single_Variant), Optional Count As Integer = -1) As IEnumerable(Of List(Of Variants.Single_Variant))
                    Dim x = From t In Variants Select t Group By t.SeqID, t.Allele, t.Reference, t.Position Into Group

                    If Count = -1 Then
                        For Each g In x
                            Yield g.Group.ToList
                        Next
                    Else
                        For Each g In x
                            If g.Group.Count = Count Then
                                Yield g.Group.ToList
                            End If
                        Next
                    End If

                End Function
                Public Shared Function Import(Files As List(Of FileInfo)) As Variants_In_Genome
                    Dim V_i_G As New Variants_In_Genome
                    For Each File In Files
                        V_i_G = Import(File, V_i_G)
                    Next
                    Return V_i_G
                End Function
                Public Shared Function Import(File As FileInfo, Optional V_i_G As Variants_In_Genome = Nothing) As Variants_In_Genome
                    If IsNothing(V_i_G) = True Then V_i_G = New Variants_In_Genome
                    Dim Header = Szunyi.IO.Import.Text.GetHeader(File, 1, Nothing, Nothing)

                    Dim SeqID_Index = Szunyi.Text.Lists.Get_Index(Header, Variant_Constants.Mapping)
                    Dim Position_Index = Szunyi.Text.Lists.Get_Index_Contains(Header, Variant_Constants.Position)
                    Dim Type_Index = Szunyi.Text.Lists.Get_Index(Header, Variant_Constants.Type)
                    Dim Length_Index = Szunyi.Text.Lists.Get_Index(Header, Variant_Constants.Length)
                    Dim Reference_Index = Szunyi.Text.Lists.Get_Index(Header, Variant_Constants.Reference)
                    Dim Allele_Index = Szunyi.Text.Lists.Get_Index(Header, Variant_Constants.Allele)
                    Dim Zygosity_Index = Szunyi.Text.Lists.Get_Index(Header, Variant_Constants.Zygosity)
                    Dim Count_Index = Szunyi.Text.Lists.Get_Index(Header, Variant_Constants.Count)
                    Dim Coverage_Index = Szunyi.Text.Lists.Get_Index(Header, Variant_Constants.Coverage)
                    Dim Frequency_Index = Szunyi.Text.Lists.Get_Index(Header, Variant_Constants.Frequency)
                    For Each Line In Szunyi.IO.Import.Text.ParseNotFirst(File, 1)
                        Dim s = Split(Line, vbTab)
                        Dim x As New Single_Variant(s(SeqID_Index), s(Position_Index), s(Type_Index), s(Length_Index), s(Reference_Index),
                                                s(Allele_Index), s(Zygosity_Index), s(Count_Index), s(Coverage_Index), s(Frequency_Index))
                        If V_i_G.Variants_By_Chromosomes.ContainsKey(x.SeqID) = False Then
                            V_i_G.Variants_By_Chromosomes.Add(x.SeqID, New Variants_In_Chromosome(x.SeqID))
                        End If
                        V_i_G.Variants_By_Chromosomes(x.SeqID).Variants.Add(x)
                    Next
                    Return V_i_G
                End Function
                Public Shared Sub Save(V_i_G As Variants_In_Genome, Optional File As FileInfo = Nothing)
                    If IsNothing(File) = True Then
                        File = Szunyi.IO.Files.Save.SelectSaveFile(Nothing)
                        If IsNothing(File) = True Then Exit Sub
                    End If
                    Dim str As New System.Text.StringBuilder
                    str.Append(GetHeader)
                    For Each Chro In V_i_G.Variants_By_Chromosomes.Values
                        For Each Item In Chro.Variants
                            str.Append(Item.SeqID).Append(vbTab)
                            str.Append(Item.Position).Append(vbTab)
                            Select Case Item.Type
                                Case Szunyi.Constants.Variants.Deletion
                                    str.Append(Variant_Constants.Deletion).Append(vbTab)
                                Case Szunyi.Constants.Variants.SNV
                                    str.Append(Variant_Constants.SNV).Append(vbTab)
                                Case Szunyi.Constants.Variants.MNV
                                    str.Append(Variant_Constants.MNV).Append(vbTab)
                                Case Szunyi.Constants.Variants.Replacement
                                    str.Append(Variant_Constants.Replacement).Append(vbTab)
                                Case Szunyi.Constants.Variants.Insertion
                                    str.Append(Variant_Constants.Insertion).Append(vbTab)
                            End Select
                            str.Append(Item.Length).Append(vbTab)
                            str.Append(Item.Reference).Append(vbTab)
                            str.Append(Item.Allele).Append(vbTab)
                            str.Append(Item.Zygosity).Append(vbTab)
                            str.Append(Item.Count).Append(vbTab)
                            str.Append(Item.Coverage).Append(vbTab)
                            str.Append(Item.Frequency).AppendLine()

                        Next
                    Next
                    str.Length -= 2
                    Szunyi.IO.Export.SaveText(str.ToString, File)
                End Sub
                Public Shared Sub Save(Variants As List(Of Single_Variant), Optional File As FileInfo = Nothing)
                    If IsNothing(File) = True Then
                        File = Szunyi.IO.Files.Save.SelectSaveFile(Szunyi.Constants.Files.Other.tdt)
                        If IsNothing(File) = True Then Exit Sub
                    End If
                    Dim str As New System.Text.StringBuilder
                    str.Append(GetHeader)
                    For Each Item In Variants
                        str.Append(Item.SeqID).Append(vbTab)
                        str.Append(Item.Position).Append(vbTab)
                        Select Case Item.Type
                            Case Szunyi.Constants.Variants.Deletion
                                str.Append(Variant_Constants.Deletion).Append(vbTab)
                            Case Szunyi.Constants.Variants.SNV
                                str.Append(Variant_Constants.SNV).Append(vbTab)
                            Case Szunyi.Constants.Variants.MNV
                                str.Append(Variant_Constants.MNV).Append(vbTab)
                            Case Szunyi.Constants.Variants.Replacement
                                str.Append(Variant_Constants.Replacement).Append(vbTab)
                            Case Szunyi.Constants.Variants.Insertion
                                str.Append(Variant_Constants.Insertion).Append(vbTab)
                        End Select
                        str.Append(Item.Length).Append(vbTab)
                        str.Append(Item.Reference).Append(vbTab)
                        str.Append(Item.Allele).Append(vbTab)
                        str.Append(Item.Zygosity).Append(vbTab)
                        str.Append(Item.Count).Append(vbTab)
                        str.Append(Item.Coverage).Append(vbTab)
                        str.Append(Item.Frequency).AppendLine()

                    Next

                    str.Length -= 2
                    Szunyi.IO.Export.SaveText(str.ToString, File)
                End Sub

                Private Shared Function GetHeader()
                    Dim str As New System.Text.StringBuilder
                    str.Append(Variant_Constants.Mapping).Append(vbTab)
                    str.Append(Variant_Constants.Position).Append(vbTab)
                    str.Append(Variant_Constants.Type).Append(vbTab)
                    str.Append(Variant_Constants.Length).Append(vbTab)
                    str.Append(Variant_Constants.Reference).Append(vbTab)
                    str.Append(Variant_Constants.Allele).Append(vbTab)
                    str.Append(Variant_Constants.Zygosity).Append(vbTab)
                    str.Append(Variant_Constants.Count).Append(vbTab)
                    str.Append(Variant_Constants.Coverage).Append(vbTab)
                    str.Append(Variant_Constants.Frequency).AppendLine()
                    Return str.ToString
                End Function
                Public Shared Function Get_Md_Sequence(sV_First As Single_Variant,
                                                   sV_Second As Single_Variant,
                                                   seq As Sequence, Extension As Integer) As Sequence
                    Dim SubSeq As Bio.Sequence = seq.GetSubSequenceByStartAndEnd(sV_First.Position - Extension - 1, sV_Second.Position + Extension - 1) '1 based to 0 based
                    Dim Ch = SubSeq.ConvertToString.ToCharArray.ToList
                    If sV_Second.Type = Constants.Variants.Insertion Then
                        Dim alf As Int16 = 54
                    End If
                    If sV_First.Type <> Constants.Variants.Insertion Then
                        For i1 = 1 To sV_First.Reference.Length
                            Ch.RemoveAt(Extension)
                        Next
                    End If
                    If sV_First.Type <> Constants.Variants.Deletion Then
                        For i1 = sV_First.Allele.Length - 1 To 0 Step -1
                            Ch.Insert(Extension, sV_First.Allele.Substring(i1, 1))
                        Next
                    End If

                    Dim Pos = Ch.Count - Extension
                    If sV_Second.Type <> Constants.Variants.Insertion Then
                        For i1 = 1 To sV_Second.Reference.Length
                            Ch.RemoveAt(Pos)
                        Next
                    End If
                    If sV_Second.Type <> Constants.Variants.Deletion Then
                        For i1 = sV_Second.Allele.Length - 1 To 0 Step -1
                            Ch.Insert(Pos, sV_Second.Allele.Substring(i1, 1))
                        Next
                    End If
                    Dim resSeq As String = Ch.ToArray
                    Dim nSeq As New Bio.Sequence(Alphabets.AmbiguousDNA, resSeq)
                    Return nSeq
                    Dim t = SubSeq.ConvertToString & vbCrLf & resSeq
                End Function

                Friend Shared Function Get_Original_And_Modified_Seqs(sV_First As Single_Variant,
                                                                  sV_Second As Single_Variant,
                                                                  Seq As Sequence,
                                                                  Extension As Integer) As List(Of Bio.Sequence)
                    Dim out As New List(Of Bio.Sequence)
                    out.Add(Seq.GetSubSequenceByStartAndEnd(sV_First.Position - Extension - 1, sV_Second.Position + Extension - 1))
                    out.Last.ID = Seq.ID & sV_First.Position & " - " & sV_Second.Position
                    out.Add(Get_Md_Sequence(sV_First, sV_Second, Seq, Extension))
                    out.Last.ID = Seq.ID & sV_First.Position & " - " & sV_Second.Position & " Var "
                    Return out
                End Function
            End Class
            Public Class Single_Variant

                Public Property SeqID As String
                Public Property Position As Long
                Public Property Type As Szunyi.Constants.Variants
                Public Property Length As Integer
                Public Property Reference As String
                Public Property Allele As String
                Public Property Zygosity As Szunyi.Constants.Variants
                Public Property Count As Integer
                Public Property Coverage As Double
                Public Property Frequency As Double



                Public Sub New(SeqID As String, Position As String, The_Type As String, Length As String, Reference As String, Allele As String,
                           Zygosity As String, Count As String, Coverage As String, Frequency As String)
                    Me.SeqID = SeqID
                    Me.Position = Position
                    Select Case The_Type
                        Case "SNV"
                            Me.Type = Constants.Variants.SNV
                        Case "MNV"
                            Me.Type = Constants.Variants.MNV
                        Case "Insertion"
                            Me.Type = Constants.Variants.Insertion
                        Case "Deletion"
                            Me.Type = Constants.Variants.Deletion
                        Case "Replacement"
                            Me.Type = Constants.Variants.Replacement
                        Case Else
                            Dim alf As Int16 = 54
                    End Select
                    Me.Length = Length
                    Me.Reference = Reference
                    Me.Allele = Allele
                    Select Case Zygosity
                        Case "Homozygous"
                            Me.Zygosity = Constants.Variants.Homozygous
                        Case "Heterozygous"
                            Me.Zygosity = Constants.Variants.Heterozygous
                        Case Else
                            Dim alf As Int16 = 43
                    End Select

                    Me.Count = Count
                    Me.Coverage = Coverage
                    Me.Frequency = Frequency
                End Sub

            End Class
            Public Class Variants_In_Genome
                Public Property Variants_By_Chromosomes As New SortedList(Of String, Variants_In_Chromosome)

            End Class
            Public Class Variants_In_Chromosome
                Public Property SeqID As String
                Public Property Variants As New List(Of Single_Variant)
                Public Sub New(SeqID As String)
                    Me.SeqID = SeqID
                End Sub
            End Class
        End Namespace
    End Namespace

End Namespace