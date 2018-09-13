Namespace Szunyi.BAM.Settings
    Public Class Last_Exons
        Public Property Exon_Length As Szunyi.Outer_Programs.Input_Description
        Public Property Intron_Length As Szunyi.Outer_Programs.Input_Description
        Public Property AT_Percent As Szunyi.Outer_Programs.Input_Description

        Public Sub New()
            Exon_Length = New Outer_Programs.Input_Description("Exon_Length to dismiss",
                                                               Outer_Programs.Input_Description_Type.Integer,
                                                               "Maximal Exon lengts is investigated",
                                                               1, 100, 20, 20, 15, "True", "Max_Exon")
            Intron_Length = New Outer_Programs.Input_Description("Minimal Intron Length next to exon",
                                                                 Outer_Programs.Input_Description_Type.Integer,
                                                                 "Minimal Intron Length next to exon",
                                                                 1, 200000, 1, 200000, 20, "True", "Min_Intron")
            AT_Percent = New Outer_Programs.Input_Description("Minimal A or T percent",
                                                              Outer_Programs.Input_Description_Type.Double,
                                                              "Minimal A or T percent",
                                                              1, 100, 1, 100, 70, "True", "AT")

        End Sub
    End Class
    Public Class Intron
        Public Property Exon_Length As Szunyi.Outer_Programs.Input_Description
        Public Property Min_Intron_Length As Szunyi.Outer_Programs.Input_Description
        Public Property Max_Intron_Length As Szunyi.Outer_Programs.Input_Description
        Public Property GTAG As Szunyi.Outer_Programs.Input_Description
        Public Property Max_Repetition_Length As Szunyi.Outer_Programs.Input_Description
        Public Sub New()
            Exon_Length = New Outer_Programs.Input_Description("Exon_Length to dismiss",
                                                               Outer_Programs.Input_Description_Type.Integer,
                                                               "Maximal Exon lengts is investigated",
                                                               1, 100, 20, 20, 15, "True", "Max_Exon")
            Min_Intron_Length = New Outer_Programs.Input_Description("Minimal Intron Length next to exon",
                                                                 Outer_Programs.Input_Description_Type.Integer,
                                                                 "Minimal Intron Length next to exon",
                                                                 1, 200000, 1, 200000, 20, "True", "Min_Intron")
            Max_Intron_Length = New Outer_Programs.Input_Description("Minimal Intron Length next to exon",
                                                                 Outer_Programs.Input_Description_Type.Integer,
                                                                 "Maximal Intron Length next to exon",
                                                                 1, 200000, 1, 200000, 10000, "True", "Min_Intron")

            GTAG = New Outer_Programs.Input_Description("Must use GT-AG rule<",
                                                                 Outer_Programs.Input_Description_Type.Boolean,
                                                                 "Maximal Intron Length next to exon",
                                                                 1, 200000, 1, 200000, 1, "True", "Min_Intron")

            Max_Repetition_Length = New Outer_Programs.Input_Description("Maximal Nof NA repetition near intron site",
                                                                 Outer_Programs.Input_Description_Type.Integer,
                                                                 "Maximal Intron Length next to exon",
                                                                 0, 10, 0, 10, 2, "True", "Min_Intron")
        End Sub



    End Class
    Public Class TSS_PAS_TR

        Public Property LocalWidth As Szunyi.Outer_Programs.Input_Description
        Public Property Width As Szunyi.Outer_Programs.Input_Description
        Public Property nofAA As Szunyi.Outer_Programs.Input_Description
        Public Property P_Threshold As Szunyi.Outer_Programs.Input_Description
        Public Property Distribution As Szunyi.Outer_Programs.Input_Description
        Public Property Qulifier As Szunyi.Outer_Programs.Input_Description
        Public Property Max_nof_AT As Szunyi.Outer_Programs.Input_Description
        Public Sub New()
            nofAA = New Outer_Programs.Input_Description("Minimal nof AA in .5 Variants",
                                                                Outer_Programs.Input_Description_Type.Integer,
                                                                "Minimal nof AA in .5 Variants",
                                                                1, 200000, 1, 200000, 20, "True", "Min_Intron")

            LocalWidth = New Outer_Programs.Input_Description("Set Local Width",
                                                                Outer_Programs.Input_Description_Type.Integer,
                                                                "Set Local Width",
                                                                1, 2000, 1, 2000, 10, "True", "Min_Intron")


            Width = New Outer_Programs.Input_Description("Set Width",
                                                               Outer_Programs.Input_Description_Type.Integer,
                                                               "Set Width",
                                                               1, 20000, 1, 20000, 50, "True", "Min_Intron")

            Max_nof_AT = New Outer_Programs.Input_Description("Set maximun nof AT at PAS",
                                                               Outer_Programs.Input_Description_Type.Integer,
                                                               "Set Width",
                                                               0, 10, 0, 10, 2, "True", "Min_Intron")

            P_Threshold = New Outer_Programs.Input_Description("Set p-value",
                                                               Outer_Programs.Input_Description_Type.Double,
                                                               "Set p-value",
                                                               0, 1, 0, 1, 0.05, "True", "Min_Intron")

            Dim Names = Szunyi.Util_Helpers.Get_All_Enum_Names(Of Szunyi.Stat.Distributions)(Szunyi.Stat.Distributions.Poisson)
            Distribution = New Outer_Programs.Input_Description("Set Distribution",
                                                               Outer_Programs.Input_Description_Type.Selection,
                                                               "Set Distribution",
                                                               0, 1, 0, 1, 0, Names, "Min_Intron")
            Dim Qulifiers = Bio.IO.GenBank.StandardQualifierNames.All.ToList

            Qulifier = New Outer_Programs.Input_Description("Set Qulifier For Naming",
                                                               Outer_Programs.Input_Description_Type.Selection,
                                                               "Set Qulifier For Naming",
                                                               0, 1, 0, 1, 30, Qulifiers, "Min_Intron")
        End Sub
    End Class

    Public Class Count_As
        Public Property w_woIntron As Szunyi.Outer_Programs.Input_Description
        Public Property w_woOrientation As Szunyi.Outer_Programs.Input_Description
        Public Property Type As Szunyi.Outer_Programs.Input_Description
        Public Property TSS_5 As Szunyi.Outer_Programs.Input_Description
        Public Property TSS_3 As Szunyi.Outer_Programs.Input_Description
        Public Property PAS_5 As Szunyi.Outer_Programs.Input_Description
        Public Property PAS_3 As Szunyi.Outer_Programs.Input_Description
        Public Sub New()
            w_woIntron = New Outer_Programs.Input_Description("With Intron?",
                                                                Outer_Programs.Input_Description_Type.Boolean,
                                                                "With Intron?",
                                                                1, 200000, 1, 200000, 1, "True", "Min_Intron")

            w_woOrientation = New Outer_Programs.Input_Description("With Orientation?",
                                                                Outer_Programs.Input_Description_Type.Boolean,
                                                                "With Orientation?",
                                                                1, 200000, 1, 200000, 1, "True", "Min_Intron")

            Dim Names = Szunyi.Util_Helpers.Get_All_Enum_Names(Of Szunyi.mRNA.Transcript.CountBy)(Szunyi.mRNA.Transcript.CountBy.Smallest)

            Type = New Outer_Programs.Input_Description("Count As",
                                                                Outer_Programs.Input_Description_Type.Selection,
                                                                "Count As",
                                                                1, 200000, 1, 200000, 1, Names, "Min_Intron")

            TSS_5 = New Outer_Programs.Input_Description("Enter alloweed 5' overhang in TSS bp",
                                                               Outer_Programs.Input_Description_Type.Integer,
                                                               "Enter alloweed 5' overhang in TSS bp",
                                                               1, 1000, 1, 1000, 10, "True", "Min_Intron")
            TSS_3 = New Outer_Programs.Input_Description("Enter alloweed 3' overhang in TSS bp, -1 indicate do not take account",
                                                               Outer_Programs.Input_Description_Type.Integer,
                                                               "Enter alloweed 3' overhang in TSS bp, -1 indicate do not take account",
                                                               1, 1000, 1, 1000, 10, "True", "Min_Intron")

            PAS_5 = New Outer_Programs.Input_Description("Enter alloweed 5' overhang in PAS bp",
                                                               Outer_Programs.Input_Description_Type.Integer,
                                                               "Enter alloweed 5' overhang in PAS bp",
                                                               1, 1000, 1, 1000, 10, "True", "Min_Intron")

            PAS_3 = New Outer_Programs.Input_Description("Enter alloweed 3' overhang in PAS bp, -1 indicate do not take account",
                                                               Outer_Programs.Input_Description_Type.Integer,
                                                               "Enter alloweed 3' overhang in PAS bp, -1 indicate do not take account",
                                                               1, 1000, 1, 1000, 10, "True", "Min_Intron")


        End Sub

    End Class
End Namespace

