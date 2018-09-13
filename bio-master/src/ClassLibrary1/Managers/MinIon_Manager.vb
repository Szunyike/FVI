Imports System.IO

Namespace Szunyi
    Namespace Manager
        Namespace MinIon
            Public Class MinIon_Manager
                Public Shared Function Create_Organisms() As Samples.Organims
                    Dim x As New Samples.Organims
                    Dim s = "9823,Sus scrofa,pig;9606,Homo sapiens,human;60711,Chlorocebus sabaeus,green monkey;7108,Spodoptera frugiperda,fall armyworm;307456,Autographa californica multiple nucleopolyhedrovirus,;10359,Human betaherpesvirus 5,Human cytomegalovirus;10335,Human alphaherpesvirus 3,Varicella-zoster virus;10298,Human alphaherpesvirus 1,Herpes simplex virus type 1;10345,Suid alphaherpesvirus 1,Pseudorabies virus;10245,Vaccinia virus,;11277,Vesicular stomatitis Indiana virus,"
                    For Each s1 In Split(s, ";")
                        x.Organims.Add(New Samples.Organism(s1))
                    Next
                    Dim k = Szunyi.IO.XML.Serialize(Of Samples.Organims)(x)
                    Szunyi.IO.Export.SaveText(k)
                End Function
            End Class

            Public Class FlowCell
                Public Property ID As String
                Public Property Type As String
                Public Property Version As String
                Public Property Script_Name As String
                Public Property Run_IDs As New List(Of String)
            End Class

        End Namespace
        Namespace Samples
            Public Class Sample
                ' tax_id	scientific_name	common_name	sample_title	sample_description
                Public Property Organism As Organism
                Public Property sample_title As String
                Public Property sample_description As String
                Public Property fields As New Dictionary(Of String, String)


            End Class
            <Serializable>
            Public Class Organims
                Public Property Organims As New List(Of Organism)
                Public Sub New()

                End Sub
            End Class
            <Serializable>
            Public Class Organism

                Public Property tax_id As Long
                Public Property scientific_name As String
                Public Property common_name As String
                Public Sub New(Line As String)
                    Dim s1 = Split(Line, ",")
                    Me.tax_id = s1(0)
                    Me.scientific_name = s1(1)
                    Me.common_name = s1(2)
                End Sub
                Public Sub New()

                End Sub
            End Class
            Public Class Field
                Public Property Name As String
                Public Property Is_Optional As Boolean
                Public Property Description As String
                Public Property Value As String
            End Class
            Public Class Fields
                Public Sub New()

                End Sub
            End Class
        End Namespace


        Public Class Libary

        End Class
    End Namespace

End Namespace

