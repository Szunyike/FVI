Imports ClassLibrary1.Szunyi.Basic
Imports ClassLibrary1.Szunyi.Other_Database.BioSytem


Namespace Szunyi
    Public Class Helper
        Public Shared Function GetNameAndIDs(keggPathways As ThePathways) As List(Of Name_ID_Type_Title_Object)
            Dim x = From t In keggPathways.Pathways Select t Order By t.Description

            Dim out As New List(Of Name_ID_Type_Title_Object)

            For Each Pathway In x
                out.Add(New Name_ID_Type_Title_Object(Pathway.Description, Pathway.BsID, "Pathway", Pathway.Description, Pathway))
            Next
            Return out
        End Function
        Public Shared Function GetNameAndIDs(keggPathways As List(Of Pathway)) As List(Of Name_ID_Type_Title_Object)

            Dim out As New List(Of Name_ID_Type_Title_Object)
            For Each Pathway In keggPathways
                out.Add(New Name_ID_Type_Title_Object(Pathway.Description, Pathway.BsID, "Pathway", Pathway.Description, Pathway))
            Next
            Return out
        End Function
        Public Shared Function GetNameAndIDs(Groups As List(Of Szunyi.Other_Database.CrossRefs.CrossRefOneToMany)) As List(Of Name_ID_Type_Title_Object)
            Dim out As New List(Of Name_ID_Type_Title_Object)

            For Each Group In Groups
                out.Add(New Name_ID_Type_Title_Object(Group.One, 1, "Group", Group.One, Group))
            Next
            Return out
        End Function
        Public Shared Function GetNameAndIDs(Group As Szunyi.Other_Database.CrossRefs.CrossRefOneToMany) As List(Of Name_ID_Type_Title_Object)
            Dim out As New List(Of Name_ID_Type_Title_Object)


            out.Add(New Name_ID_Type_Title_Object(Group.One, 1, "Group", Group.One, Group))

            Return out
        End Function
    End Class
    Namespace Common
        Public Class StartAndEnd
            Public Property Start As Long
            Public Property Endy As Long
            Public Sub New()

            End Sub
        End Class
    End Namespace

End Namespace
