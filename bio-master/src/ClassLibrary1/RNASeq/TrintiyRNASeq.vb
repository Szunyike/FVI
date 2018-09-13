Namespace Szunyi
    Namespace RNASeq
        Namespace Trintiy
            Public Class SS_lib_type
                Public ReadOnly Property FR = "Forward Reverse"
                Public ReadOnly Property RF = "Reverse Forward"
                Public ReadOnly Property F = "Forward"
                Public ReadOnly Property R = "Reverse"

                Public Property All_SS_Lib_Types As list(Of String)
                Public Sub New()
                    All_SS_Lib_Types = New list(Of String)
                    All_SS_Lib_Types.add(FR)
                    All_SS_Lib_Types.add(RF)
                    All_SS_Lib_Types.add(F)
                    All_SS_Lib_Types.add(R)
                End Sub
            End Class

            Public Class TrintiyRNASeq
                Public Property SS_Lib_Types As New SS_lib_type
                Dim f1 As New Get_List_of_String("Select Libary Type")




            End Class

        End Namespace
    End Namespace
End Namespace
