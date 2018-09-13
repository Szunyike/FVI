Namespace Szunyi
    Namespace Protein
        Public Class Protein_Stability
            Public Property N_Term_Rules As New N_term_Rules
            Public Function Get_Half_Lifes(Seqs As List(Of Bio.ISequence), Organism As String) As List(Of Integer)
                Dim Out As New List(Of Integer)
                For Each Seq In Seqs
                    Out.Add(Get_Half_Life(Seq, Organism))
                Next
                Return Out
            End Function
            Public Function Get_Half_Life(Seq As Bio.ISequence, Organim As String) As Integer
                If Me.N_Term_Rules.N_term_Rules.ContainsKey(Organim) = True Then
                    Return Me.N_Term_Rules.N_term_Rules(Organim).Times_By_Nterm_AA(Seq(0))
                Else
                    Return 0
                End If
            End Function
        End Class
        Public Class N_term_Rules
            Public Property N_term_Rules As New Dictionary(Of String, N_term_rule)
            Public Sub New()
                Add_Mammalian()
                Add_Yeast()
                Add_E_Coli()

            End Sub

            Private Sub Add_E_Coli()
                Dim x As New N_term_rule
                x.ID = "E. Coli"
                Dim d As New Dictionary(Of Byte, Integer)
                d.Add(Convert.ToByte("R"c), 2)
                d.Add(Convert.ToByte("N"c), 600)
                d.Add(Convert.ToByte("D"c), 600)
                d.Add(Convert.ToByte("C"c), 600)
                d.Add(Convert.ToByte("E"c), 600)
                d.Add(Convert.ToByte("Q"c), 600)
                d.Add(Convert.ToByte("G"c), 600)
                d.Add(Convert.ToByte("H"c), 600)
                d.Add(Convert.ToByte("I"c), 600)
                d.Add(Convert.ToByte("L"c), 2)
                d.Add(Convert.ToByte("K"c), 2)
                d.Add(Convert.ToByte("M"c), 600)
                d.Add(Convert.ToByte("F"c), 2)
                d.Add(Convert.ToByte("P"c), 0)
                d.Add(Convert.ToByte("S"c), 600)
                d.Add(Convert.ToByte("T"c), 600)
                d.Add(Convert.ToByte("W"c), 2)
                d.Add(Convert.ToByte("Y"c), 2)
                d.Add(Convert.ToByte("V"c), 600)
                d.Add(Convert.ToByte("A"c), 600)
                x.Times_By_Nterm_AA = d
                Me.N_term_Rules.Add(x.ID, x)
            End Sub

            Private Sub Add_Yeast()
                Dim x As New N_term_rule
                x.ID = "Yeast"
                Dim d As New Dictionary(Of Byte, Integer)
                d.Add(Convert.ToByte("R"c), 2)
                d.Add(Convert.ToByte("N"c), 3)
                d.Add(Convert.ToByte("D"c), 3)
                d.Add(Convert.ToByte("C"c), 1200)
                d.Add(Convert.ToByte("E"c), 10)
                d.Add(Convert.ToByte("Q"c), 30)
                d.Add(Convert.ToByte("G"c), 1200)
                d.Add(Convert.ToByte("H"c), 10)
                d.Add(Convert.ToByte("I"c), 30)
                d.Add(Convert.ToByte("L"c), 3)
                d.Add(Convert.ToByte("K"c), 3)
                d.Add(Convert.ToByte("M"c), 1200)
                d.Add(Convert.ToByte("F"c), 3)
                d.Add(Convert.ToByte("P"c), 1200)
                d.Add(Convert.ToByte("S"c), 1200)
                d.Add(Convert.ToByte("T"c), 1200)
                d.Add(Convert.ToByte("W"c), 3)
                d.Add(Convert.ToByte("Y"c), 10)
                d.Add(Convert.ToByte("V"c), 1200)
                d.Add(Convert.ToByte("A"c), 1200)
                x.Times_By_Nterm_AA = d
                Me.N_term_Rules.Add(x.ID, x)
            End Sub

            Private Sub Add_Mammalian()
                Dim x As New N_term_rule
                x.ID = "Mammalian"
                Dim d As New Dictionary(Of Byte, Integer)
                d.Add(Convert.ToByte("R"c), 60)
                d.Add(Convert.ToByte("N"c), 84)
                d.Add(Convert.ToByte("D"c), 66)
                d.Add(Convert.ToByte("C"c), 72)
                d.Add(Convert.ToByte("E"c), 48)
                d.Add(Convert.ToByte("Q"c), 60)
                d.Add(Convert.ToByte("G"c), 1800)
                d.Add(Convert.ToByte("H"c), 210)
                d.Add(Convert.ToByte("I"c), 1200)
                d.Add(Convert.ToByte("L"c), 330)
                d.Add(Convert.ToByte("K"c), 78)
                d.Add(Convert.ToByte("M"c), 1800)
                d.Add(Convert.ToByte("F"c), 66)
                d.Add(Convert.ToByte("P"c), 1200)
                d.Add(Convert.ToByte("S"c), 114)
                d.Add(Convert.ToByte("T"c), 432)
                d.Add(Convert.ToByte("W"c), 168)
                d.Add(Convert.ToByte("Y"c), 168)
                d.Add(Convert.ToByte("V"c), 6000)
                d.Add(Convert.ToByte("A"c), 264)
                x.Times_By_Nterm_AA = d
                Me.N_term_Rules.Add(x.ID, x)
            End Sub
        End Class
        Public Class N_term_rule
            Public Property ID As String
            Public Property Times_By_Nterm_AA As New Dictionary(Of Byte, Integer)

        End Class
    End Namespace
End Namespace

