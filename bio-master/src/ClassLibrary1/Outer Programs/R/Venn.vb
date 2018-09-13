

Namespace Szunyi
    Namespace Outer_Programs

        Public Class Venn
            Public Shared Function Get_Tiff(File As System.IO.FileInfo)
                '       R.R.Run(File)
                '       Dim x = R.R.lvector(File)
                ' Return x
            End Function
            Public Function Get_Tiff(x As Generic.List(Of Szunyi.Other_Database.CrossRefs.CrossRefOneToMany))
                    Dim Res As New Dictionary(Of String, String())
                    For Each Item In x
                        Res.Add(Item.One, Item.Many.ToArray)
                    Next
                End Function
            End Class

    End Namespace

End Namespace

