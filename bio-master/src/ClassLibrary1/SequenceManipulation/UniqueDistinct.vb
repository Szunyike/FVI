
Imports Bio

Namespace Szunyi
    Namespace Sequences
        Namespace SequenceManipulation
            Public Class UniqueDistinct
#Region "Splice Variants"
                Public Shared Function GetSpliceVariants(Seqs As List(Of Bio.ISequence)) As List(Of Bio.ISequence)
                    Dim out As New List(Of Bio.ISequence)
                    For Each Item In UniqueDistinct.GetSeqGroupsByShortSeqID(Seqs)
                        If Item.Count > 1 Then out.AddRange(Item)
                    Next
                    Return out
                End Function

                ''' <summary>
                ''' Return Sequnces, Which Has not got Splice Variant
                ''' </summary>
                ''' <param name="Seqs"></param>
                ''' <returns></returns>
                Public Shared Function GetNoSpliceVariants(Seqs As List(Of Bio.ISequence)) As List(Of Bio.ISequence)
                    Dim Renamed = SequenceManipulation.ID.Rename(Seqs, Szunyi.Constants.StringRename.FirstAfterSplit, ".")

                    Return UniqueDistinct.GetUniqueSeqsByID(Renamed)
                End Function
                ''' <summary>
                ''' Return Sequneces, Which Has Splice Variants
                ''' </summary>
                ''' <param name="Seqs"></param>
                ''' <param name="Goods"></param>
                ''' <returns></returns>
                Public Shared Function GetHasGoodSpliceVariants(Seqs As List(Of ISequence), Goods As List(Of ISequence)) As List(Of Bio.ISequence)
                    Dim out As New List(Of Bio.ISequence)

                    Dim RenamedSecond = Szunyi.Sequences.SequenceManipulation.ID.Rename(Goods,
                                                                           Szunyi.Constants.StringRename.FirstAfterSplit, ".")

                    Dim GoodIDs = Szunyi.Sequences.SequenceManipulation.Common.GetSeqIDs(RenamedSecond)
                    GoodIDs.Sort()

                    For Each Seq In Seqs
                        Dim sSeqID = Seq.ID.Split(".").First
                        Dim Index = GoodIDs.BinarySearch(sSeqID)
                        If Index > -1 Then
                            out.Add(Seq)
                        End If
                    Next
                    Return out
                End Function


#End Region
                ''' <summary>
                ''' Return List Of  Bio.ISequence By Same SeqID
                ''' </summary>
                ''' <param name="seqs"></param>
                ''' <returns></returns>
                Public Shared Iterator Function GetSeqGroupsBySeqID(Seqs As List(Of Bio.ISequence)) As IEnumerable(Of List(Of Bio.ISequence))
                    Dim t = From x In Seqs Select x Order By Seqs.Count Descending Group By x.ID Into Group
                    For Each g In t
                        Yield g.Group.ToList
                    Next
                End Function
                ''' <summary>
                ''' Return List Of  Bio.ISequence By Same SeqID
                ''' </summary>
                ''' <param name="seqs"></param>
                ''' <returns></returns>
                Public Shared Iterator Function GetSeqGroupsBySeqID(Seqs As List(Of Bio.ISequence), NofLetter As Integer) As IEnumerable(Of List(Of Bio.ISequence))
                    Dim t = From x In Seqs Select x Order By Seqs.Count Descending Group By FirstLetters = x.ID.Substring(0, 2).ToUpper Into Group
                    For Each g In t
                        Yield g.Group.ToList
                    Next
                End Function
                ''' <summary>
                ''' Return List Of  Bio.ISequence By Same SeqID
                ''' </summary>
                ''' <param name="seqs"></param>
                ''' <returns></returns>
                Public Shared Iterator Function GetSeqGroupsByShortSeqID(Seqs As List(Of Bio.ISequence)) As IEnumerable(Of List(Of Bio.ISequence))
                    Dim t = From x In Seqs Select x Order By Seqs.Count Descending Group By x.ID.Split(".").First Into Group
                    For Each g In t
                        Yield g.Group.ToList
                    Next
                End Function

#Region "Duplicated Seqs"
                ''' <summary>
                ''' It is Return All Of The Duplicated Sequence
                ''' </summary>
                ''' <param name="Seqs"></param>
                ''' <returns></returns>
                Public Shared Function GetDuplicatedSeqsBySeq(Seqs As List(Of Bio.ISequence)) As List(Of Bio.ISequence)
                    Dim out As New List(Of Bio.ISequence)
                    For Each g In GetDuplicatedSeqsBySeqIterator(Seqs)
                        out.AddRange(g)
                    Next

                    Return out
                End Function

                ''' <summary>
                ''' It is Return All Of The Duplicated Sequence
                ''' </summary>
                ''' <param name="Seqs"></param>
                ''' <returns></returns>
                Public Shared Iterator Function GetDuplicatedSeqsBySeqIterator(Seqs As List(Of Bio.ISequence)) As IEnumerable(Of List(Of Bio.ISequence))
                    Dim x = From t In Seqs Select t Group By t.ToArray Into Group

                    Dim out As New List(Of Bio.ISequence)
                    For Each g In x
                        If g.Group.Count > 1 Then
                            Yield g.Group.ToList
                        End If
                    Next

                End Function

                ''' <summary>
                ''' Return all of the Duplicated Seqs by ID
                ''' </summary>
                ''' <param name="Seqs"></param>
                ''' <returns></returns>
                Public Shared Function GetDuplicatedSeqsByID(Seqs As List(Of Bio.ISequence)) As List(Of Bio.ISequence)
                    Dim Out As New List(Of Bio.ISequence)
                    For Each SeqGroup In GetDuplicatedSeqsByIDIterator(Seqs)
                        Out.AddRange(SeqGroup)
                    Next
                    Return Out
                End Function

                ''' <summary>
                ''' Return all of the Duplicated Seqs by ID
                ''' </summary>
                ''' <param name="Seqs"></param>
                ''' <returns></returns>
                Public Shared Iterator Function GetDuplicatedSeqsByIDIterator(Seqs As List(Of Bio.ISequence)) As IEnumerable(Of List(Of Bio.ISequence))
                    Dim Out As New List(Of Bio.ISequence)
                    For Each SeqGroup In GetSeqGroupsBySeqID(Seqs)
                        If SeqGroup.Count > 1 Then
                            Yield SeqGroup
                        End If
                    Next
                End Function

                Public Shared Function GetDuplicatedSeqsBySeqAndID(Seqs As List(Of Bio.ISequence)) As List(Of Bio.ISequence)
                    Dim out As New List(Of Bio.ISequence)
                    For Each g1 In GetDuplicatedSeqsByIDIterator(Seqs)
                        For Each g2 In GetDuplicatedSeqsBySeqIterator(g1)
                            out.AddRange(g2)
                        Next
                    Next
                    Return out
                End Function

#End Region

#Region "Unique Seqs"
                ''' <summary>
                ''' Return all of the Unique and only Unique Seqs by ID
                ''' </summary>
                ''' <param name="Seqs"></param>
                ''' <returns></returns>
                Public Shared Function GetUniqueSeqsByID(Seqs As List(Of Bio.ISequence)) As List(Of Bio.ISequence)
                    Dim Out As New List(Of Bio.ISequence)
                    For Each SeqGroup In GetUniqueSeqsByIDIterator(Seqs)
                        Out.AddRange(SeqGroup)
                    Next
                    Return Out
                End Function
                ''' <summary>
                ''' Return all of the Unique Seqs by ID
                ''' </summary>
                ''' <param name="Seqs"></param>
                ''' <returns></returns>
                Public Shared Iterator Function GetUniqueSeqsByIDIterator(Seqs As List(Of Bio.ISequence)) As IEnumerable(Of List(Of Bio.ISequence))
                    Dim Out As New List(Of Bio.ISequence)
                    For Each SeqGroup In GetSeqGroupsBySeqID(Seqs)
                        If SeqGroup.Count = 1 Then
                            Yield SeqGroup
                        End If
                    Next

                End Function

                ''' <summary>
                ''' Return all of the Unique Seqs by Seqs
                ''' </summary>
                ''' <param name="Seqs"></param>
                ''' <returns></returns>
                Public Shared Function GetUniqueSeqsBySeq(Seqs As List(Of Bio.ISequence)) As List(Of Bio.ISequence)
                    Dim Out As New List(Of Bio.ISequence)
                    For Each SeqGroup In GetUniqueSeqsBySeqIterator(Seqs)
                        Out.AddRange(SeqGroup)
                    Next
                    Return Out
                End Function
                ''' <summary>
                ''' Return all of the Unique Seqs by Seqs
                ''' </summary>
                ''' <param name="Seqs"></param>
                ''' <returns></returns>
                Public Shared Iterator Function GetUniqueSeqsBySeqIterator(Seqs As List(Of Bio.ISequence)) As IEnumerable(Of List(Of Bio.ISequence))
                    Dim x = From t In Seqs Select t Group By t.ToArray Into Group

                    For Each g In x
                        If g.Group.Count = 1 Then
                            Yield g.Group.ToList
                        End If
                    Next

                End Function

                ''' <summary>
                ''' Return all of the Unique Seqs by Seqs
                ''' </summary>
                ''' <param name="Seqs"></param>
                ''' <returns></returns>
                Public Shared Function GetUniqueSeqsBySeqAndID(Seqs As List(Of Bio.ISequence)) As List(Of Bio.ISequence)
                    Dim Out As New List(Of Bio.ISequence)
                    For Each SeqGroup In GetUniqueSeqsBySeqIterator(Seqs)
                        Out.AddRange(SeqGroup)
                    Next
                    Return Out
                End Function
                ''' <summary>
                ''' Return all of the Unique Seqs by Seqs
                ''' </summary>
                ''' <param name="Seqs"></param>
                ''' <returns></returns>
                Public Shared Iterator Function GetUniqueSeqsBySeqAndIDIterator(Seqs As List(Of Bio.ISequence)) As IEnumerable(Of List(Of Bio.ISequence))
                    Dim x = From t In Seqs Select t Group By t.ToArray, t.ID Into Group

                    Dim out As New List(Of Bio.ISequence)
                    For Each g In x
                        If g.Group.Count = 1 Then
                            Yield g.Group
                        End If
                    Next

                End Function
#End Region

#Region "Group"

                ''' <summary>
                ''' Return SeqGroups By Seq ID
                ''' </summary>
                ''' <param name="Seqs"></param>
                ''' <returns></returns>
                Public Shared Iterator Function GetSameSequenceGroupsBySeqID(Seqs As List(Of Bio.ISequence)) As IEnumerable(Of List(Of Bio.ISequence))
                    Dim x = From t In Seqs Select t Group By t.ID Into Group

                    Dim out As New List(Of Bio.ISequence)
                    For Each g In x

                        Yield g.Group.ToList

                    Next
                End Function

                ''' <summary>
                ''' Return SeqGroups By Seq
                ''' </summary>
                ''' <param name="Seqs"></param>
                ''' <returns></returns>
                Public Shared Iterator Function GetSameSequenceGroupsBySeq(Seqs As List(Of Bio.ISequence)) As IEnumerable(Of List(Of Bio.ISequence))
                    Dim x = From t In Seqs Select t Group By t.ConvertToString Into Group

                    For Each g In x
                        Yield g.Group.ToList
                    Next
                End Function
                ''' <summary>
                ''' Return SeqGroups By Seq
                ''' </summary>
                ''' <param name="Seqs"></param>
                ''' <returns></returns>
                Public Shared Iterator Function GetSameSequenceGroupsBySeq(Seqs As List(Of Bio.Sequence)) As IEnumerable(Of List(Of Bio.Sequence))
                    Dim x = From t In Seqs Select t Group By t.ConvertToString Into Group

                    For Each g In x
                        Yield g.Group.ToList
                    Next
                End Function

                ''' <summary>
                ''' Return SeqGroups By Seqs and ID
                ''' </summary>
                ''' <param name="Seqs"></param>
                ''' <returns></returns>
                Public Shared Iterator Function GetSameSequenceGroupsBySeqAndID(Seqs As List(Of Bio.ISequence)) As IEnumerable(Of List(Of Bio.ISequence))
                    Dim x = From t In Seqs Select t Group By t.ConvertToString, t.ID Into Group

                    For Each g In x
                        Yield g.Group.ToList
                    Next
                End Function

                ''' <summary>
                ''' Return the Common Sequences based ByReference or empty list
                ''' </summary>
                ''' <param name="a"></param>
                ''' <param name="b"></param>
                ''' <returns></returns>
                Public Shared Function GetCommonSeqs(a As List(Of Bio.ISequence), b As List(Of Bio.ISequence)) As List(Of Bio.ISequence)
                    Dim res = a.Intersect(b)

                    If res.Count > 0 Then Return res.ToList
                    Return New List(Of Bio.ISequence)

                End Function

#End Region

#Region "1Copy"
                Public Shared Function Get1CopyBySeqAndID(Seqs As List(Of Bio.ISequence)) As List(Of Bio.ISequence)
                    Dim Out As New List(Of Bio.ISequence)
                    For Each Item In GetSameSequenceGroupsBySeqAndID(Seqs)
                        Out.Add(Item.First)
                    Next
                    Return Out
                End Function
                Public Shared Function Get1CopyBySeq(Seqs As List(Of Bio.ISequence)) As List(Of Bio.ISequence)
                    Dim Out As New List(Of Bio.ISequence)
                    For Each Item In GetSameSequenceGroupsBySeq(Seqs)
                        If Item.Count > 1 Then
                            Dim alf As Int16 = 54
                        End If
                        Out.Add(Item.First)
                    Next
                    Return Out
                End Function
                Public Shared Function Get1CopyBySeq(Seqs As List(Of Bio.Sequence)) As List(Of Bio.ISequence)
                    Dim Out As New List(Of Bio.ISequence)
                    For Each Item In GetSameSequenceGroupsBySeq(Seqs)
                        If Item.Count > 1 Then
                            Dim alf As Int16 = 54
                        End If
                        Out.Add(Item.First)
                    Next
                    Return Out
                End Function
                Public Shared Function Get1CopyByID(Seqs As List(Of Bio.ISequence)) As List(Of Bio.ISequence)
                    Dim Out As New List(Of Bio.ISequence)
                    For Each Item In GetSameSequenceGroupsBySeqID(Seqs)
                        Out.Add(Item.First)
                    Next
                    Return Out
                End Function
#End Region
                ''' <summary>
                ''' Based On SeqID
                ''' Retrurn A Contains, but Not A
                ''' </summary>
                ''' <param name="a"></param>
                ''' <param name="b"></param>
                ''' <returns></returns>
                Public Shared Function GetDistinctSeqsByIDs(a As List(Of Bio.ISequence), b As List(Of Bio.ISequence)) As List(Of ISequence)

                    Dim Keys = (From Key In b Select Key.ID).ToList

                    Dim t = From x In a Where Keys.Contains(x.ID) = False

                    If t.Count = 0 Then
                        Return New List(Of Bio.ISequence)
                    Else Return t.ToList
                    End If


                End Function
                ''' <summary>
                ''' Based On SeqID
                ''' Retrurn A Contains, but Not A
                ''' </summary>
                ''' <param name="a"></param>
                ''' <param name="b"></param>
                ''' <returns></returns>
                Public Shared Function Get_Common_Seqs_ByIDs(a As List(Of Bio.ISequence), b As List(Of Bio.ISequence)) As List(Of Bio.ISequence)

                    Dim Keys_A = (From Key In a Select Key.ID).ToList
                    Dim Keys_B = (From Key In a Select Key.ID).ToList
                    Dim Keys_Common = Keys_A.Intersect(Keys_B)

                    Dim t = From x In a Where Keys_Common.Contains(x.ID) = True

                    If t.Count = 0 Then
                        Return New List(Of Bio.ISequence)
                    Else Return t.ToList
                    End If


                End Function


                ''' <summary>
                ''' Return the StartIndex Of SubSequence or -1 
                ''' </summary>
                ''' <param name="SmallerSeq"></param>
                ''' <param name="BiggerSeq"></param>
                ''' <returns></returns>
                Public Shared Function SimpleBoyerMooreSearch(SmallerSeq As Bio.ISequence, BiggerSeq As Bio.ISequence) As Integer
                    Dim needle = SmallerSeq.ToArray
                    Dim haystack = BiggerSeq.ToArray
                    Dim lookup As Integer() = New Integer(255) {}
                    For i As Integer = 0 To lookup.Length - 1
                        lookup(i) = needle.Length
                    Next

                    For i As Integer = 0 To needle.Length - 1
                        lookup(needle(i)) = needle.Length - i - 1
                    Next

                    Dim index As Integer = needle.Length - 1
                    Dim lastByte = needle.Last()
                    While index < haystack.Length
                        Dim checkByte = haystack(index)
                        If haystack(index) = lastByte Then
                            Dim found As Boolean = True
                            For j As Integer = needle.Length - 2 To 0 Step -1
                                If haystack(index - needle.Length + j + 1) <> needle(j) Then

                                    found = False
                                    Exit For
                                End If
                            Next

                            If found Then
                                Return index - needle.Length + 1
                            Else
                                index += 1
                            End If
                        Else
                            index += lookup(checkByte)
                        End If
                    End While
                    Return -1
                End Function

                ''' <summary>
                ''' Return the StartIndex Of SubSequence or -1 
                ''' </summary>
                ''' <param name="SmallerSeq"></param>
                ''' <param name="BiggerSeq"></param>
                ''' <returns></returns>
                Public Shared Function SimpleBoyerMooreSearch_Reverse_Complement(SmallerSeq As Sequence, BiggerSeq As Sequence) As Integer
                    Dim TheSeq = SmallerSeq.GetReverseComplementedSequence
                    Return SimpleBoyerMooreSearch(TheSeq, BiggerSeq)

                End Function

                Public Shared Function Get_Indexes_Of_First_Occurence(all_Seqs As List(Of Bio.ISequence), tmp As List(Of Bio.ISequence)) As List(Of Integer)
                    Dim out As New List(Of Integer)
                    For i1 = 0 To tmp.Count - 1
                        If all_Seqs.Contains(tmp(i1)) Then
                            out.Add(all_Seqs.IndexOf(tmp(i1)))
                        End If
                    Next
                    Return out
                End Function
            End Class

        End Namespace
    End Namespace
End Namespace

