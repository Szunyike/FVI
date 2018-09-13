Imports Bio.IO.GenBank

Namespace Szunyi.Features
    Public Class Feat_To_String
        Public Shared Function Only_One_From_Qulifier(Quals() As String, Feat As Bio.IO.GenBank.FeatureItem) As String
            For Each Q In Quals
                If Feat.Qualifiers.ContainsKey(Q) AndAlso Feat.Qualifiers(Q).Count > 0 Then
                    If Feat.Qualifiers(Q).First <> String.Empty Then
                        Return Feat.Qualifiers(Q).First
                    End If
                End If
            Next
            Return String.Empty
        End Function
        Public Shared Function Only_One_From_Qulifier(Quals As List(Of String), Feat As Bio.IO.GenBank.FeatureItem) As String
            For Each Q In Quals
                If Feat.Qualifiers.ContainsKey(Q) AndAlso Feat.Qualifiers(Q).Count > 0 Then
                    If Feat.Qualifiers(Q).First <> String.Empty Then
                        Return Feat.Qualifiers(Q).First
                    End If
                End If
            Next
            Return String.Empty
        End Function

        Public Shared Function By_Qualifiers(selected_Qulifiers As List(Of String), Feat As FeatureItem) As String
            Dim str As New System.Text.StringBuilder
            For Each Q In selected_Qulifiers
                If Feat.Qualifiers.ContainsKey(Q) = True Then
                    str.Append(Feat.Qualifiers(Q).First).Append(vbTab)
                Else
                    str.Append(vbTab)
                End If
            Next
            If str.Length > 0 Then str.Length -= 1
            Return str.ToString
        End Function
    End Class
End Namespace
Namespace Szunyi.Features.FeatureManipulation
    Public Class Convert
        Public Shared Function ToBed(Feats As List(Of FeatureItem), seq As Bio.ISequence) As List(Of Bio.SequenceRange)
            Dim out As New List(Of Bio.SequenceRange)
            For Each f In Feats
                out.Add(ToBed(f, seq))

            Next
        End Function
        Public Shared Function ToBed(Feat As FeatureItem, seq As Bio.ISequence) As Bio.SequenceRange
            Dim x As New Bio.SequenceRange(seq.ID, Feat.Location.LocationStart, Feat.Location.LocationEnd)
            x.Metadata.Add("Name", Feat.Label)
            x.Metadata.Add("Strand", Szunyi.Location.Common.Get_Strand(Feat.Location))
            Return x
        End Function
        Public Shared Function ToBed(Feat As FeatureItem, seq As Bio.ISequence, c As Drawing.Color, SelQuals As List(Of String)) As Bio.SequenceRange
            Dim x As New Bio.SequenceRange(seq.ID, Feat.Location.LocationStart, Feat.Location.LocationEnd)
            If IsNothing(SelQuals) = True OrElse SelQuals.Count = 0 Then
                x.Metadata.Add("Name", Szunyi.Features.FeatureManipulation.Common.Get_ByQualifier_First(SelQuals.First, Feat))
            Else
                x.Metadata.Add("Name", Feat.Label)
            End If

            x.Metadata.Add("Strand", Szunyi.Location.Common.Get_Strand(Feat.Location))

            x.Metadata.Add("ItemRGB", c.R & "," & c.G & "," & c.B)
            Return x
        End Function
        Public Shared Function ToBed(Feats As List(Of FeatureItem), seq As Bio.ISequence, c As Drawing.Color, SelQuals As List(Of String)) As List(Of Bio.SequenceRange)
            Dim out As New List(Of Bio.SequenceRange)
            For Each f In Feats
                out.Add(ToBed(f, seq, c, SelQuals))
            Next
            Return out
        End Function
    End Class
End Namespace

