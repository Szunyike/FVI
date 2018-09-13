Namespace Szunyi
    Namespace Protein
        Public Class Amino_Acid_Composition_Manipulation
            Public Shared Function Get_Txt(AACs As List(Of Amino_Acid_Composition)) As String
                Dim str As New System.Text.StringBuilder
                str.Append("SeqID").Append(vbTab)
                str.Append(Get_Header).Append(vbTab)
                str.Append("SeqID").Append(vbTab)
                str.Append(Get_Header).AppendLine()

                For Each AAC In AACs
                    str.Append(Get_Count(AAC)).Append(vbTab).Append(Get_Percent(AAC)).AppendLine()
                Next
                str.Length -= 2
                Return str.ToString
            End Function
            Public Shared Function Get_AA_Composition(Seq As Bio.ISequence) As Amino_Acid_Composition
                Dim x As New Amino_Acid_Composition(Seq)
                Set_Counts_And_Percents(x)
                Return x
            End Function
            Public Shared Function Get_AA_Compositions(Seqs As List(Of Bio.ISequence)) As List(Of Amino_Acid_Composition)
                Dim Out As New List(Of Amino_Acid_Composition)
                For Each Seq In Seqs
                    Dim x As New Amino_Acid_Composition(Seq)
                    Set_Counts_And_Percents(x)
                    Out.Add(x)
                Next
                Out.Sort(Szunyi.Comparares.AllComparares.AA_Compositions)

                Return Out
            End Function

            Public Shared Function Get_Counts(Items As List(Of Amino_Acid_Composition)) As String
                Dim str As New System.Text.StringBuilder
                For Each Item In Items
                    str.Append(Get_Count(Item)).AppendLine()
                Next
                str.Length -= 2
                Return str.ToString
            End Function
            Public Shared Function Get_Count(Item As Amino_Acid_Composition) As String
                Dim str As New System.Text.StringBuilder
                str.Append(Item.ID).Append(vbTab)
                For Each Count In Item.Counts
                    str.Append(Count.Value).Append(vbTab)
                Next
                str.Length -= 1
                Return str.ToString
            End Function

            Public Shared Function Get_Percents(Items As List(Of Amino_Acid_Composition)) As String
                Dim str As New System.Text.StringBuilder
                For Each Item In Items
                    str.Append(Get_Percent(Item)).AppendLine()
                Next
                str.Length -= 2
                Return str.ToString
            End Function
            Public Shared Function Get_Percent(Item As Amino_Acid_Composition) As String
                Dim str As New System.Text.StringBuilder
                str.Append(Item.ID).Append(vbTab)
                For Each Percent In Item.Percents
                    str.Append(Percent.Value).Append(vbTab)
                Next
                str.Length -= 1
                Return str.ToString
            End Function

            Public Shared Function Get_Header() As String
                Dim str As New System.Text.StringBuilder
                For Each Item In Bio.Alphabets.Protein
                    If Item <> Bio.Alphabets.Protein.Ter AndAlso Bio.Alphabets.Protein.Gap Then
                        str.Append(ChrW(Item)).Append(vbTab)
                    End If
                Next
                str.Length -= 1
                Return str.ToString
            End Function
            Private Shared Sub Set_Counts_And_Percents(x As Amino_Acid_Composition)
                For Each Item In Bio.Alphabets.Protein
                    If Item <> Bio.Alphabets.Protein.Ter AndAlso Bio.Alphabets.Protein.Gap Then
                        Dim COunt As Integer = 0
                        For Each AA In x.Seq
                            If AA = Item Then
                                COunt += 1
                            End If
                        Next
                        x.Counts.Add(Item, COunt)
                        x.Percents.Add(Item, COunt / Szunyi.Sequences.SequenceManipulation.Counts.Get_Sequence_Length_Wo_Ter_And_Gap_Symbols(x.Seq))
                    End If

                Next
            End Sub
            Public Shared Sub Calculate_Percents(x As Amino_Acid_Composition)
                Dim Total As Double = 0
                For Each Item In x.Counts
                    Total += Item.Value
                Next
                x.Percents.Clear()
                For Each Item In x.Counts
                    x.Percents.Add(Item.Key, Item.Value / Total * 100)
                Next
            End Sub
            Public Shared Sub Set_Counts_And_Percents_Clear(x As Amino_Acid_Composition)
                For Each Item In Bio.Alphabets.Protein


                    x.Counts.Add(Item, 0)
                    x.Percents.Add(Item, 0)
                Next
            End Sub
            Public Function Clone(x As Amino_Acid_Composition) As Amino_Acid_Composition
                Dim t As New Amino_Acid_Composition(x.Seq)
                For Each Count In x.Counts
                    t.Counts.Add(Count.Key, Count.Value)
                Next
                For Each Percent In x.Percents
                    t.Percents.Add(Percent.Key, Percent.Value)
                Next
                Return t
            End Function
            Public Function Clones(x As List(Of Amino_Acid_Composition)) As List(Of Amino_Acid_Composition)
                Dim out As New List(Of Amino_Acid_Composition)
                For Each Item In x
                    out.Add(Clone(Item))
                Next
                Return out
            End Function

        End Class
        Public Class Amino_Acid_Composition
            Public ID As String
            Public Seq As Bio.ISequence
            Public Property Counts As New Dictionary(Of Byte, Double)
            Public Property Percents As New Dictionary(Of Byte, Double)
            Public Sub New(seq As Bio.ISequence)
                Me.Seq = seq
                Me.ID = Me.Seq.ID
            End Sub
            Public Sub New(Name As String)
                Me.ID = Name
            End Sub
        End Class
        Public Class Amino_Acid_Properties
            Public Property AA_Props As New Dictionary(Of Byte, Amino_Acid_Property)
            Public Sub New()
                Dim File As New System.IO.FileInfo(My.Resources.Basic_Files & "AA_Properties.txt")
                For Each s In Szunyi.IO.Import.Text.ParseToArray(File, vbTab, 1)
                    Dim x As New Amino_Acid_Property(s)
                    AA_Props.Add(AscW(x.One_Letter), x)
                Next
            End Sub
            Public Function Get_Mutation(A As String, B As String) As String
                Dim a1 As Byte = AscW(A)
                Dim b1 As Byte = AscW(B)
                Return Get_Mutation(a1, b1)
            End Function
            Public Function Get_Mutation(A As String) As String
                Dim a1 As Byte = AscW(A)

                Return Get_Mutation(a1)
            End Function
            Public Function Get_Mutation_Only_SideChanin(A As Byte) As String
                If AA_Props.ContainsKey(A) = False Then Return String.Empty
                Return (AA_Props(A).Based_On_Side_Chain)

            End Function

            Public Function Get_Mutation(A As Byte) As String
                Return (AA_Props(A).Side_Chain & " " & AA_Props(A).Side_Chain_Charge & " " & AA_Props(A).Side_Chain_Polarity)

            End Function
            Public Function Get_Mutation(A As Byte, B As Byte) As String
                Dim str As New System.Text.StringBuilder
                Try

                    str.Append(AA_Props(A).Side_Chain & " " & AA_Props(A).Side_Chain_Charge & " " & AA_Props(A).Side_Chain_Polarity)
                        str.Append(vbTab)
                        str.Append(AA_Props(B).Side_Chain & " " & AA_Props(B).Side_Chain_Charge & " " & AA_Props(B).Side_Chain_Polarity)


                Catch ex As Exception

                End Try

                Return str.ToString
            End Function
        End Class

        Public Class Amino_Acid_Property
            Private s As String
            Public Property Name As String
            Public Property Three_Letter As String
            Public Property One_Letter As String
            Public Property Based_On_Side_Chain As String
            Public Property Side_Chain As String
            Public Property Side_Chain_Polarity As String
            Public Property Side_Chain_Charge As String
            Public Property Hidropathy_Index As Double
            Public Property MW As Double

            Public Sub New(s As String())
                Me.Name = s(0)
                Me.Three_Letter = s(1)
                Me.One_Letter = s(2)
                Me.Based_On_Side_Chain = s(3)
                Me.Side_Chain = s(4)
                Me.Side_Chain_Polarity = s(5)
                Me.Side_Chain_Charge = s(6)
                ' Me.Hidropathy_Index = s(6)
                '    Me.MW = s(7)
            End Sub
        End Class
    End Namespace
End Namespace
