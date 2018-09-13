Namespace Szunyi
    Namespace Basic
        <Serializable>
        Public Class NameAndID
            Public Property Name As String
            Public Property Title As String
            Public ID As Integer
            Public Sub New(Name As String, ID As Integer, Title As String)
                Me.Name = Name
                Me.ID = ID
                Me.Title = Title
            End Sub
        End Class

        <Serializable>
        Public Class Name_ID_Type_Title_Object
            Public Property Name As String
            Public Property ID As Integer
            Public Property Title As String
            Public Property Type As String
            Public Property obj As Object
            Public Sub New(Name As String, ID As Integer, type As String, Title As String, Optional obj As Object = Nothing)
                Me.Name = Name
                Me.ID = ID
                Me.Type = type
                Me.Title = Title
                Me.obj = obj
            End Sub
        End Class

    End Namespace
End Namespace

