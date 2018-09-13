Imports FV_VI.Szunyi.ListOf
Imports Bio
Imports Bio.IO.GenBank

Namespace Szunyi.Sequences
    Public Class GetSequences

        ''' <summary>
        ''' Return the List Of Sequences, Find By BinarySearch in Sorted List
        ''' </summary>
        ''' <param name="seqs"></param>
        ''' <param name="SeqIDs"></param>
        ''' <returns></returns>
        Friend Shared Function ByIDs(Seqs As List(Of Sequence), SeqIDs As List(Of String)) As List(Of Bio.Sequence)
            Dim tmp As New Bio.Sequence(Alphabets.DNA, "")
            Dim out As New List(Of Bio.Sequence)
            For Each SeqID In SeqIDs
                tmp.ID = SeqID
                Dim i = Seqs.BinarySearch(tmp, AllComparares.BySeqID)
                If i > -1 Then
                    out.Add(Seqs(i))
                End If
            Next
            Return out
        End Function
        ''' <summary>
        ''' Return The Seq Or Nothing 
        ''' </summary>
        ''' <param name="Seqs"></param>
        ''' <param name="SeqID"></param>
        ''' <returns></returns>
        Friend Shared Function ByID(Seqs As List(Of Sequence), SeqID As String) As Bio.Sequence
            Dim tmp As New Bio.Sequence(Alphabets.DNA, "")

            tmp.ID = SeqID
            Dim i = Seqs.BinarySearch(tmp, AllComparares.BySeqID)
            If i > -1 Then Return Seqs(i)

            Return Nothing
        End Function
    End Class
    Public Class RemoveDuplicatedSeqBySequence
        Public Property SeqLists As New List(Of SequenceList)
        Public Property SeqList As SequenceList
        Public Property AllSeqs As List(Of Bio.Sequence)
        Public Property UniqueSeqs As List(Of Bio.Sequence)
        Public Property Type As String = MyConstants.BackGroundWork.ModyfiedSequence
        Public Property UniqueID As Integer
        Public Sub New(SeqLists As List(Of SequenceList), ID As Integer)
            Me.SeqLists = SeqLists
            Me.UniqueID = ID
        End Sub
        Public Sub DoIt()
            'Create All Seqs
            'Do Dictionary of Seqs By SeqString using only Uniuqe
            'Fill UniqueSeqs
            Me.AllSeqs = Sequences.SequenceManinpulation.MergeSequenceList(Me.SeqLists)
            Dim SeqsKmerswCounts = New Szunyi.Sequences.SeqsKmerswCounts(Me.AllSeqs, 0, False, 0, 0, 0)
            SeqsKmerswCounts.DoIt()
            Me.UniqueSeqs = SeqsKmerswCounts.GetSequneces
            Me.SeqList = (New SequenceList(Me.UniqueSeqs, "RD:" & Me.SeqLists.Last.ShortFileName))
            Me.SeqList.UniqueID = Me.UniqueID
        End Sub
    End Class
    Public Class RenameSequenceIDs
        Public Property SeqLists As New List(Of SequenceList)
        Public Property SeqList As SequenceList
        Public Property AllSeqs As List(Of Bio.Sequence)
        Public Property UniqueSeqs As List(Of Bio.Sequence)
        Public Property Type As String = MyConstants.BackGroundWork.ModyfiedSequence
        Public Property UniqueID As Integer
        Public Property PreFix As String
        Public Property ShortFileName As String
        Public Property SubType As String = ""
        Public Sub New(Seqs As List(Of Bio.Sequence), SubType As String, Optional Prefix As String = "")
            Me.AllSeqs = Seqs
            Me.PreFix = Prefix
            Me.SubType = SubType
        End Sub
        Public Sub New(SeqLists As List(Of SequenceList), SubType As String, Optional Prefix As String = "")
            Me.SeqLists = SeqLists
            Me.AllSeqs = Sequences.SequenceManinpulation.MergeSequenceList(Me.SeqLists)
            Me.PreFix = Prefix
            Me.SubType = SubType
            Me.ShortFileName = "Renamed: " & SeqLists.Last.ShortFileName
        End Sub
        Public Sub DoIt()
            Select Case Me.SubType
                Case MyConstants.StringRename.AscendingWithPrefix
                    For i1 = 0 To Me.AllSeqs.Count - 1
                        Me.AllSeqs(i1).ID = PreFix & i1
                    Next
                Case MyConstants.StringRename.FirstAfterSplit
                    For i1 = 0 To Me.AllSeqs.Count - 1
                        Me.AllSeqs(i1).ID = Split(Me.AllSeqs(i1).ID, PreFix).First
                    Next
                Case MyConstants.StringRename.LastAfterSplit
                    For i1 = 0 To Me.AllSeqs.Count - 1
                        Me.AllSeqs(i1).ID = Split(Me.AllSeqs(i1).ID, PreFix).Last
                    Next
            End Select
            Me.AllSeqs.Sort(Comparares.AllComparares.BySeqID)
            Me.SeqList = New SequenceList(Me.AllSeqs, Me.ShortFileName)

        End Sub
    End Class
    Public Class MergeSequenceAnnotations
        Public Class MainFeatures
            Public Property CDSs As ExtFeatureList
            Public Property Genes As ExtFeatureList
            Public Property mRNAs As ExtFeatureList
        End Class
        Public Property SeqLists As List(Of Szunyi.ListOf.SequenceList)
        Public Property Good As Boolean = True
        Public Property First As New MainFeatures
        Public Property Second As New MainFeatures
        Dim SeqManinpulation As New Szunyi.Sequences.SequenceManinpulation
        Public Sub New(SeqLists As List(Of Szunyi.ListOf.SequenceList))
            Me.SeqLists = SeqLists
            If Check() = False Then
                Me.Good = False
            Else
                SetExtFeatureLists()
            End If

        End Sub
        Private Function Check()
            If SeqLists.Count <> 2 Then
                MsgBox("There must be two sequence List")
                Return False
            End If

            Return True
        End Function
        Public Sub DoIt()
            Dim FeatManipulation As New Szunyi.GenBank.FeatureManipulation
            Dim NewCOunt As Integer = 0
            For Each Feat In Me.Second.CDSs.Features
                Dim Md As Bio.IO.GenBank.GenBankMetadata =
                        SeqManinpulation.GetGenBankMetadataBySeqID(Feat.SeqID, Me.SeqLists.First)

                Dim x = Me.First.CDSs.Features.BinarySearch(Feat, New Comparares.ExtFeatureLocationComparer)
                If x >= 0 Then
                    ' It is Already Contains Dont Do anything
                    Me.First.CDSs.Features(x).Feature = FeatManipulation.Merge2Features(Feat.Feature, Me.Second.CDSs.Features(x).Feature)
                Else
                    Dim alf As Int16 = 54
                    NewCOunt += 1
                    Dim tmpFeat As New ExtFeature(Split(Feat.LocusTag, ".").First)
                    If IsNothing(Md.Features) = True Then Md.Features = New SequenceFeatures

                    Md.Features.All.Add(Feat.Feature)

                    Dim x1 = Me.Second.Genes.FetauresByLocustag.BinarySearch(tmpFeat, New Comparares.ExtFeatureLocusTagComparer)
                    If x1 >= 0 Then
                        Dim newGene = Me.Second.Genes.FetauresByLocustag(x1)
                        Md.Features.All.Add(newGene.Feature)
                    End If

                    Dim x2 = Me.Second.mRNAs.FetauresByLocustag.BinarySearch(Feat, New Comparares.ExtFeatureLocusTagComparer)
                    If x2 >= 0 Then
                        Dim newMrna = Me.Second.mRNAs.FetauresByLocustag(x2)
                        Md.Features.All.Add(newMrna.Feature)
                    End If

                End If

            Next
            Szunyi.IO.Export.SaveSequencesToSingleGenBank(Me.SeqLists.First.Sequences)
            Szunyi.IO.Export.SaveSequencesToSingleGenBank(Me.SeqLists.Last.Sequences)
        End Sub
        ''' <summary>
        ''' Set mRNA,CDS and Gene List for First and Last SequencLists
        ''' </summary>
        Private Sub SetExtFeatureLists()

            Me.First.CDSs = GetExtFeatureList(StandardFeatureKeys.CodingSequence, SeqLists.First)
            Me.First.mRNAs = GetExtFeatureList(StandardFeatureKeys.MessengerRna, SeqLists.First)
            Me.First.Genes = GetExtFeatureList(StandardFeatureKeys.Gene, SeqLists.First)

            Me.Second.CDSs = GetExtFeatureList(StandardFeatureKeys.CodingSequence, SeqLists.Last)
            Me.Second.mRNAs = GetExtFeatureList(StandardFeatureKeys.MessengerRna, SeqLists.Last)
            Me.Second.Genes = GetExtFeatureList(StandardFeatureKeys.Gene, SeqLists.Last)
        End Sub
        ''' <summary>
        ''' SubRoutine for getting correct Feature By Type
        ''' </summary>
        ''' <param name="FeatureType"></param>
        ''' <param name="SeqList"></param>
        ''' <returns></returns>
        Private Function GetExtFeatureList(FeatureType As String, SeqList As Szunyi.ListOf.SequenceList) As ExtFeatureList
            Dim cSearchSetting = New SettingForSearchInFeaturesAndQulifiers(FeatureType)
            Dim x1 As New Szunyi.ListOf.ExtFeatureList(cSearchSetting, SeqList)
            x1.DoIt()
            Return x1
        End Function
    End Class
    Public Class KmerManipulation
        ''' <summary>
        ''' Return All of the Kmer in Sequnce as List Of String or Empty List
        ''' </summary>
        ''' <param name="Seq"></param>
        ''' <param name="KmerLength"></param>
        ''' <returns></returns>
        Public Shared Function GetAllKmer(Seq As Bio.Sequence, KmerLength As Integer) As List(Of String)
            Dim sSeq = Seq.ConvertToString(0, Seq.Count).ToUpper
            Dim out As New List(Of String)
            If Seq.Count >= KmerLength Then
                For i1 = 0 To Seq.Count - KmerLength
                    out.Add(sSeq.Substring(i1, KmerLength))
                Next
            End If
            Return out
        End Function
    End Class
End Namespace

