Imports Bio.SimilarityMatrices
Imports ClassLibrary1
Imports Szunyi_All

Public Class Geneious_Variants
    Public Shared Function Get_Variants(File As IO.FileInfo) As List(Of Geneious_Variant)
        Dim Header = Szunyi.IO.Import.Text.GetHeader(File, 1)
        Dim iPolymorphism_Type = Szunyi.Text.Lists.Get_Index(Header, "Polymorphism Type")
        Dim iAmino_Acid_Change = Szunyi.Text.Lists.Get_Index(Header, "Amino Acid Change")
        Dim iChange = Szunyi.Text.Lists.Get_Index(Header, "Change")
        Dim iCDS_Codon_Number = Szunyi.Text.Lists.Get_Index(Header, "CDS Codon Number")
        Dim iCDS = Szunyi.Text.Lists.Get_Index(Header, "CDS")
        Dim iCodon_Change = Szunyi.Text.Lists.Get_Index(Header, "Codon Change")
        Dim iMinimum = Szunyi.Text.Lists.Get_Index(Header, "Minimum")
        Dim iMaximum = Szunyi.Text.Lists.Get_Index(Header, "Maximum")
        'Sequence_Name
        Dim iSequence_Name = Szunyi.Text.Lists.Get_Index(Header, "Sequence Name")
        Dim igene = Szunyi.Text.Lists.Get_Index(Header, "gene")
        Dim iLTag = Szunyi.Text.Lists.Get_Index(Header, "locus_tag")
        Dim out As New List(Of Geneious_Variant)
        For Each Line In Szunyi.IO.Import.Text.Parse(File, 1)
            If IsNothing(Line) = False Then
                Dim s = Split(Line, vbTab)
                Dim x As New Geneious_Variant(s(iPolymorphism_Type),
                                           s(iAmino_Acid_Change),
                                           s(iChange),
                                           s(iCDS_Codon_Number),
                                           s(iCDS),
                                            s(iCodon_Change),
                                              s(iMinimum),
                                              s(iMaximum),
                                              s(iSequence_Name),
                                              s(igene),
                                              s(iLTag))
                out.Add(x)
            End If

        Next
        Return out
    End Function
    Public Shared Iterator Function ByCDS(Vars As List(Of Geneious_Variant), CDSs As List(Of Bio.IO.GenBank.FeatureItem)) As IEnumerable(Of KeyValuePair(Of Bio.IO.GenBank.FeatureItem, List(Of Geneious_Variant)))
        Dim gr = From x In Vars Where x.CDS <> "" Group By x.CDS Into Group
        CDSs.Sort(New Szunyi.Comparares.OneByOne.FeatureItemComparer)
        Dim jh = From x2 In gr Order By x2.Group.First.Minimum

        Dim out As New List(Of KeyValuePair(Of Bio.IO.GenBank.FeatureItem, List(Of Geneious_Variant)))
        For Each g In jh
            Dim Vs = g.Group.ToList
            For Each Feat In CDSs
                Dim f = Feat
                Dim res = From x In Vs Where x.Minimum >= f.Location.LocationStart And x.Maximum <= f.Location.LocationEnd And x.LTag = f.Qualifiers(Bio.IO.GenBank.StandardQualifierNames.LocusTag).First.Replace(Chr(34), "")
                If res.Count > 0 Then
                    Dim s As New KeyValuePair(Of Bio.IO.GenBank.FeatureItem, List(Of Geneious_Variant))(f, res.ToList)

                    out.Add(s)
                End If
            Next
        Next
        Dim Out2 = From x In out Order By x.Key.Location.LocationStart Ascending

        For Each o In Out2
            Yield o
        Next
    End Function
    Public Shared Iterator Function By_Polymorphis_Type(v As List(Of Geneious_Variant)) As IEnumerable(Of List(Of Geneious_Variant))
        Dim gr = From x In v Group By x.Polymorphism_Type Into Group

        For Each g In gr
            Yield g.Group.ToList
        Next
    End Function

    Public Shared Function Get_InterGenic(polyMorf_Type As List(Of Geneious_Variant)) As List(Of Geneious_Variant)
        Dim out = From x In polyMorf_Type Where x.CDS = ""

        If out.Count = 0 Then Return New List(Of Geneious_Variant)
        Return out.ToList
    End Function

    Public Shared Function Get_InCDS(polyMorf_Type As List(Of Geneious_Variant)) As List(Of Geneious_Variant)
        Dim out = From x In polyMorf_Type Where x.CDS <> ""

        If out.Count = 0 Then Return New List(Of Geneious_Variant)
        Return out.ToList
    End Function

    Public Shared Function Get_Nof_Affected_NA(interGenic As List(Of Geneious_Variant)) As Integer
        Return (From x In interGenic Select x.Na_Change.Length).Sum

    End Function
    Public Shared Function Get_Nof_Affected_AA(Vars As List(Of Geneious_Variant)) As Integer
        Dim r = From x In Vars Select x.Codon_Change.Original_Codons

        Dim r2 As New List(Of Codon_Changes.Codon)
        For Each r1 In r
            r2.AddRange(r1)
        Next
        Return r2.Count
    End Function

    Public Shared Function Get_SNP_Type(cDS As List(Of Geneious_Variant), matrixes As List(Of SimilarityMatrix)) As List(Of Integer)
        Dim s As New List(Of Integer)
        For Each item In matrixes
            s.Add(0)
        Next

        s.Add(0) ' Insertion
        s.Add(0) ' Deletion
        Dim n As Integer = 0
        Dim n2 As Integer = 0
        For Each c In cDS
            If c.Polymorphism_Type.Contains("SNP") Then
                n += 1
                For Each s1 In c.Codon_Change.Original_Codons
                    For i1 = 0 To matrixes.Count - 1
                        s(i1) += matrixes(i1).Item(s1.Original_AA.First, s1.New_NA.First)
                    Next

                Next
            ElseIf c.Polymorphism_Type.Contains("Insertion") Then
                For Each s1 In c.Codon_Change.Original_Codons
                    If s1.Original_AA.Count > 0 AndAlso s1.New_AA.Count > 0 Then
                        For i1 = 0 To matrixes.Count - 1
                            s(i1) += matrixes(i1).Item(s1.Original_AA.First, s1.New_NA.First)
                        Next
                        n += 1
                    Else
                        s(s.Count - 2) += 1
                        n += 1
                    End If
                Next
            ElseIf c.Polymorphism_Type.Contains("Deletion") Then
                For Each s1 In c.Codon_Change.Original_Codons
                    If s1.Original_AA.Count > 0 AndAlso s1.New_AA.Count > 0 Then
                        For i1 = 0 To matrixes.Count - 1
                            s(i1) += matrixes(i1).Item(s1.Original_AA.First, s1.New_NA.First)
                        Next
                        n += 1
                    Else
                        s(s.Count - 1) += 1
                        n += 1
                    End If
                Next
            ElseIf c.Polymorphism_Type.Contains("Substitution") Then
                For Each s1 In c.Codon_Change.Original_Codons
                    For i1 = 0 To matrixes.Count - 1
                        s(i1) += matrixes(i1).Item(s1.Original_AA.First, s1.New_NA.First)
                    Next

                Next
            End If

        Next
        Return s
    End Function
    Public Shared Iterator Function By_Min_Max(Vars As List(Of Geneious_Variant)) As IEnumerable(Of Geneious_Variant)
        Dim gr = From x In Vars Group By x.Minimum, x.Maximum Into Group

        For Each g In gr
            Yield g.Group.First
        Next
    End Function
    Public Shared Iterator Function By_Min_Max_AA_Change(Vars As List(Of Geneious_Variant)) As IEnumerable(Of Geneious_Variant)
        Dim gr = From x In Vars Group By x.Minimum, x.Maximum, x.Amino_Acid_Change Into Group

        For Each g In gr
            Yield g.Group.First
        Next
    End Function
    Friend Shared Function Get_Unique_By_Interval(Vars As List(Of Geneious_Variant)) As List(Of Geneious_Variant)
        Return By_Min_Max(Vars).ToList
    End Function

    Friend Shared Function Get_Unique_By_Interval_AA_Change(Vars As List(Of Geneious_Variant)) As List(Of Geneious_Variant)
        Return By_Min_Max_AA_Change(Vars).ToList
    End Function
End Class
Public Class Geneious_Variant

    Public Sub New(iPolymorphism_Type As String,
                   iAmino_Acid_Change As String,
                   iChange As String,
                   iCDS_Codon_Number As String,
                   iCDS As String,
                   iCodon_Change As String,
                   iMinimum As String,
                   iMaximum As String,
                   iSequence_Name As String,
                   igene As String,
                   iLTag As String)
        Me.Polymorphism_Type = iPolymorphism_Type
        Me.Amino_Acid_Change = iAmino_Acid_Change
        Me.Change = iChange
        Me.Na_Change = New NA_Change(Me.Change)
        Me.Minimum = iMinimum
        Me.Maximum = iMaximum
        Me.Sequence_Name = iSequence_Name
        Me.gene = igene
        Me.LTag = iLTag
        If iCDS_Codon_Number = "" Then
            Me.CDS_Codon_Number = 0
        Else
            Me.CDS_Codon_Number = iCDS_Codon_Number
        End If

        Me.CDS = iCDS
        Me.Codon_Change = New Codon_Changes(iCodon_Change)


    End Sub
    Public Property Sequence_Name As String
    Public Property Polymorphism_Type As String
    Public Property Amino_Acid_Change As String
    Public Property Change As String
    Public Property CDS_Codon_Number As Integer
    Public Property CDS As String
    Public Property Minimum As Integer
    Public Property Maximum As Integer
    Public Property gene As String
    Public Property LTag As String
    Public Property sCodon_Change As String
    Public Property Codon_Change As Codon_Changes
    Public Property Nof_Affected_AA As Integer
    Public Property Nof_Affected_NA As Integer
    Public Property OriSeq As String
    Public Property NewSeq As String
    Public Property OriAA As String
    Public Property NewAA As String
    Public Property Na_Change As NA_Change
End Class
Public Class NA_Change
    Public Property Original_NA As Bio.ISequence
    Public Property New_NA As Bio.ISequence
    Public Property Length As Integer
    Public Sub New(s As String)
        If s.StartsWith("+") Then
            Me.Original_NA = New Bio.Sequence(Bio.Alphabets.AmbiguousDNA, "")
            Me.New_NA = New Bio.Sequence(Bio.Alphabets.AmbiguousDNA, s.Substring(1))
            Me.Length = Me.New_NA.Count
        ElseIf s.StartsWith("-") Then
            Me.New_NA = New Bio.Sequence(Bio.Alphabets.AmbiguousDNA, "")
            Me.Original_NA = New Bio.Sequence(Bio.Alphabets.AmbiguousDNA, s.Substring(1))
            Me.Length = Me.Original_NA.Count
        ElseIf s.StartsWith("(") Then
            Dim s1 = Split(s, " -> ")
            Dim oriCount As Integer = Split(s1.First, ")").Last
            Dim newCount As Integer = Split(s1.Last, ")").Last
            Dim t = s1.First.Substring(1, s1.First.Length - oriCount.ToString.Count - 2)
            If oriCount > newCount Then
                t = Szunyi.Text.General.Multiply(t, oriCount - newCount)
                Me.Original_NA = New Bio.Sequence(Bio.Alphabets.AmbiguousDNA, t)
                Me.New_NA = New Bio.Sequence(Bio.Alphabets.AmbiguousDNA, "")
                Me.Length = Me.Original_NA.Count
            Else
                t = Szunyi.Text.General.Multiply(t, newCount - oriCount)
                Me.Original_NA = New Bio.Sequence(Bio.Alphabets.AmbiguousDNA, "")
                Me.New_NA = New Bio.Sequence(Bio.Alphabets.AmbiguousDNA, t)
                Me.Length = Me.New_NA.Count
            End If
        Else
            Dim s1 = Split(s, " -> ")
            Me.Original_NA = New Bio.Sequence(Bio.Alphabets.AmbiguousDNA, s1.First)
            Me.New_NA = New Bio.Sequence(Bio.Alphabets.AmbiguousDNA, s1.Last)
            Me.Length = Me.Original_NA.Count
        End If
    End Sub
End Class
Public Class Codon_Changes
    Public Class Codon
        Public Property Original_NA As Bio.ISequence
        Public Property Original_AA As Bio.ISequence
        Public Property New_NA As Bio.ISequence
        Public Property New_AA As Bio.ISequence
        Public Sub New(Ori As String, N As String)
            Me.Original_NA = New Bio.Sequence(Bio.Alphabets.AmbiguousDNA, Ori)
            Me.Original_AA = Szunyi.Translate.TranslateIntoSequnence1Frame(Me.Original_NA)
            Me.New_NA = New Bio.Sequence(Bio.Alphabets.AmbiguousDNA, N)
            Me.New_AA = Szunyi.Translate.TranslateIntoSequnence1Frame(Me.New_NA)
        End Sub
    End Class
    Public Property Original_Codons As New List(Of Codon)

    Public Sub New(s As String)
        If s = "" Then Exit Sub
        Dim s1() = Split(s, " -> ")
        Dim Oris = Split(s1.First, ",")
        Dim News = Split(s1.Last, ",")

        If Oris.Count = News.Count Then ' SNP,MNP
            For i1 = 0 To Oris.Count - 1
                Me.Original_Codons.Add(New Codon(Oris(i1), News(i1)))
            Next
        ElseIf Oris.Count > News.Count Then ' Deletion
            If News.Count > 1 Then
                Dim kj As Int16 = 43
            End If
            If News.First = "" Then
                For i1 = 0 To Oris.Count - 1
                    Me.Original_Codons.Add(New Codon(Oris(i1), ""))
                Next
            Else '  Rearranged Codon
                If News.First = Oris.First Then ' First Codon

                    For i1 = 1 To Oris.Count - 1
                        Me.Original_Codons.Add(New Codon(Oris(i1), ""))
                    Next
                ElseIf News.First = Oris.Last Then ' Last Codon
                    For i1 = 0 To Oris.Count - 2
                        Me.Original_Codons.Add(New Codon(Oris(i1), ""))
                    Next

                Else ' There is MisMatch in Codons
                    For i2 = 0 To 2
                        If News.First.Substring(i2, 1) = Oris.First.Substring(i2, 0) And
                            News.First.Substring(i2, 1) = Oris.Last.Substring(i2, 0) Then
                            ' Check Next
                        ElseIf News.First.Substring(i2, 1) = Oris.First.Substring(i2, 0) Then
                            Me.Original_Codons.Add(New Codon(Oris(0), News(0)))
                            For i1 = 1 To Oris.Count - 1
                                Me.Original_Codons.Add(New Codon(Oris(i1), ""))
                                Exit For
                            Next
                        ElseIf News.First.Substring(i2, 1) = Oris.Last.Substring(i2, 0) Then
                            For i1 = 0 To Oris.Count - 2
                                Me.Original_Codons.Add(New Codon(Oris(i1), ""))
                            Next
                            Me.Original_Codons.Add(New Codon(Oris.Last, News.First))
                            Exit For
                        Else
                            Me.Original_Codons.Add(New Codon(Oris(0), News(0)))
                            For i1 = 1 To Oris.Count - 1
                                Me.Original_Codons.Add(New Codon(Oris(i1), ""))

                            Next
                            Exit For
                        End If
                    Next
                End If

            End If

        Else ' Insertion
            If Oris.First = "" Then
                For i1 = 0 To News.Count - 1
                    Me.Original_Codons.Add(New Codon("", News(i1)))
                Next
            Else ' Rearranged Codon
                If Oris.Count > 1 Then
                    Dim kj As Int16 = 43
                End If
                If News.First = Oris.First Then ' First Codon
                    For i1 = 1 To News.Count - 1
                        Me.Original_Codons.Add(New Codon("", News(i1)))
                    Next

                ElseIf News.Last = Oris.First Then ' Last Codon
                    For i1 = 0 To News.Count - 2
                        Me.Original_Codons.Add(New Codon("", News(i1)))
                    Next

                    Dim kj As Int16 = 54
                Else ' There is MisMatch in Codons
                    For i2 = 0 To 2
                        If News.First.Substring(i2, 1) = Oris.First.Substring(i2, 0) And
                            News.Last.Substring(i2, 1) = Oris.First.Substring(i2, 0) Then
                            Dim j As Int16 = 54
                            ' Check Next
                        ElseIf News.First.Substring(i2, 1) = Oris.First.Substring(i2, 0) Then
                            Me.Original_Codons.Add(New Codon(Oris(0), News(0)))
                            For i1 = 1 To News.Count - 1
                                Me.Original_Codons.Add(New Codon("", News(i1)))

                            Next
                            Exit For
                        ElseIf News.Last.Substring(i2, 1) = Oris.First.Substring(i2, 0) Then
                            For i1 = 0 To News.Count - 2
                                Me.Original_Codons.Add(New Codon("", News(i1)))
                            Next
                            Me.Original_Codons.Add(New Codon(Oris.First, News.Last))
                            Exit For
                        Else
                            Me.Original_Codons.Add(New Codon(Oris(0), News(0)))
                            For i1 = 1 To News.Count - 1
                                Me.Original_Codons.Add(New Codon("", News(i1)))
                            Next
                            Exit For
                        End If
                    Next
                End If
            End If
        End If
    End Sub
End Class
