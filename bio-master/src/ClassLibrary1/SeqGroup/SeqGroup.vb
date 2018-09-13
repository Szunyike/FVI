Imports ClassLibrary1.Szunyi
Imports ClassLibrary1.Szunyi.ListOf
Imports ClassLibrary1.Szunyi.Sequences.SequenceManipulation

Namespace Szunyi
    Namespace Sequences
        Public Class SeqGroup

            Public Property Type As String
            Public Property OriginalSeqs As New List(Of Bio.ISequence)
            Public Property ResultSeqs As New List(Of Bio.ISequence)
            Public Property Title As String
            Public Property TooltipText As String
            Public Property Result As ListOf.SequenceList
            Public Sub New(SeqLists As List(Of ListOf.SequenceList), Type As String)

                Me.Type = Type
                OriginalSeqs = Merging.MergeSequenceList(SeqLists)
                Dim SeqListNames = Szunyi.Sequences.SequenceManipulation.Common.GetSeqListsName(SeqLists)
                SeqListNames.Insert(0, Type)
                Me.TooltipText = Szunyi.Text.General.GetText(SeqListNames)

                Select Case Type
                    Case Constants.SeqGroups.UniqueByID
                        Me.ResultSeqs = UniqueDistinct.GetUniqueSeqsByID(OriginalSeqs)
                    Case Constants.SeqGroups.UniqueBySeq
                        Me.ResultSeqs = UniqueDistinct.GetUniqueSeqsBySeq(OriginalSeqs)
                    Case Constants.SeqGroups.UniqueByIDAndSeq
                        Me.ResultSeqs = UniqueDistinct.GetUniqueSeqsBySeqAndID(OriginalSeqs)
                    Case Constants.SeqGroups.DuplicatedByID
                        Me.ResultSeqs = UniqueDistinct.GetDuplicatedSeqsByID(OriginalSeqs)
                    Case Constants.SeqGroups.DuplicatedBySeq
                        Me.ResultSeqs = UniqueDistinct.GetDuplicatedSeqsBySeq(OriginalSeqs)
                    Case Constants.SeqGroups.DuplicatedByIDAndSeq
                        Me.ResultSeqs = UniqueDistinct.GetDuplicatedSeqsBySeqAndID(OriginalSeqs)
                    Case Constants.SeqGroups.OneCopyByID
                        Me.ResultSeqs = UniqueDistinct.Get1CopyByID(OriginalSeqs)
                    Case Constants.SeqGroups.OneCopyBySeq
                        Me.ResultSeqs = UniqueDistinct.Get1CopyBySeq(OriginalSeqs)
                    Case Constants.SeqGroups.OneCopyByIDAndSeq
                        Me.ResultSeqs = UniqueDistinct.Get1CopyBySeqAndID(OriginalSeqs)
                End Select
                If Me.ResultSeqs.Count > 0 Then
                    Dim s = InputBox("Title Of Sequence List e:" & Me.ResultSeqs.Count)
                    Me.Result = New SequenceList(Me.ResultSeqs, s, Me.TooltipText)
                End If


            End Sub
        End Class
    End Namespace
End Namespace


