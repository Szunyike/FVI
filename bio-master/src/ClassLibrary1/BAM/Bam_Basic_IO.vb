Imports System.IO
Imports System.Text
Imports Bio
Imports Bio.IO.SAM
Imports ClassLibrary1.Szunyi.BAM.Stat

Namespace Szunyi.BAM
    Namespace Bam_Basic_IO

        Public Class Headers
#Region "Get Headers"
            ''' <summary>
            ''' Return Distinct Read Group RG based on 
            ''' </summary>
            ''' <param name="files"></param>
            ''' <returns></returns>
            Public Shared Function Get_Read_Groups(files As List(Of FileInfo)) As List(Of SAMRecordField)
                Dim Headers = Get_Header(files)
                Dim out As New List(Of SAMRecordField)
                For Each Header As SAMAlignmentHeader In Headers
                    Dim RGs = From x In Header.RecordFields Where x.Typecode = "RG"

                    out.AddRange(RGs)

                    Dim kj As Int16 = 54
                Next
                Return out
            End Function
            Public Shared Function Get_Organism_Length(File As FileInfo, RName As String) As Long

                Dim S = Get_ReferenceSequences(File)
                Dim r = (From x In S Where x.Name = RName Select x.Length).Sum
                Return r

            End Function
            Public Shared Function Get_Organism_Length(File As FileInfo) As Long

                Dim S = Get_ReferenceSequences(File)
                Dim r = (From x In S Select x.Length).Sum
                Return r

            End Function
            Public Shared Function Get_Organism_Length(Files As List(Of FileInfo)) As Long

                Dim S = Get_ReferenceSequences(Files)
                Dim r = (From x In S Select x.Length).Sum
                Return r

            End Function
            ''' <summary>
            ''' 
            ''' </summary>
            ''' <param name="File"></param>
            ''' <returns></returns>
            Public Shared Function Get_Comments(File As FileInfo) As String
                If IsNothing(File) = True Then Return String.Empty
                Dim Header = Get_Header(File)
                Dim str As New System.Text.StringBuilder
                For Each CO In Header.Comments
                    str.Append("#").Append(CO).AppendLine()
                Next
                If str.Length > 0 Then
                    str.Length -= 2
                    Return str.ToString
                End If
                Return String.Empty
            End Function
            ''' <summary>
            ''' Return Headers From SAM OR Bam File
            ''' </summary>
            ''' <param name="Files"></param>
            ''' <returns></returns>
            Public Shared Function Get_Header(files As List(Of FileInfo)) As List(Of SAMAlignmentHeader)
                Dim out As New List(Of SAMAlignmentHeader)
                For Each File In files
                    out.Add(Get_Header(File))
                Next
                Return out
            End Function

            ''' <summary>
            ''' Return Header From SAM OR Bam File
            ''' </summary>
            ''' <param name="File"></param>
            ''' <returns></returns>
            Public Shared Function Get_Header(File As FileInfo) As SAMAlignmentHeader
                Try
                    Using sr As New FileStream(File.FullName, FileMode.Open)
                        If File.Extension = ".Bam" Or File.Extension = ".bam" Then
                            Dim sa As New Bio.IO.BAM.BAMParser()
                            Return sa.GetHeader(sr)
                        ElseIf File.Extension = ".Sam" Or File.Extension = ".sam" Then
                            Return Bio.IO.SAM.SAMParser.ParseSAMHeader(sr)
                        End If
                    End Using
                Catch ex As Exception

                End Try

                Return Nothing
            End Function
            ''' <summary>
            ''' Return Header From SAM OR Bam File
            ''' </summary>
            ''' <param name="File"></param>
            ''' <returns></returns>
            Public Shared Function Get_Header_s(h As SAMAlignmentHeader) As String

                Dim str As New System.Text.StringBuilder
                '  str.Append(File.Name).AppendLine()
                For Each I In h.RecordFields
                    str.Append("@").Append(I.Typecode)
                    For Each i1 In I.Tags
                        str.Append(vbTab).Append(i1.Tag).Append(":").Append(i1.Value)
                    Next
                    str.AppendLine()
                Next
                For Each I In h.ReferenceSequences
                    str.Append(I.Name).Append(vbTab).Append(I.Length).AppendLine()

                Next
                Return str.ToString
            End Function
            ''' <summary>
            ''' 
            ''' </summary>
            ''' <param name="reference_Sequences"></param>
            ''' <returns></returns>
            Public Shared Function Get_Header(reference_Sequences As List(Of ReferenceSequenceInfo)) As String
                Dim str As New System.Text.StringBuilder
                For Each refSeq In reference_Sequences
                    str.Append("@SQ").Append(vbTab).Append("SN:").Append(refSeq.Name).Append("LN:").Append(refSeq.Length).AppendLine()
                Next
                If str.Length > 0 Then str.Length -= 2
                Return str.ToString
            End Function

#End Region

#Region "Get Adaptors From Comment Line"
            Public Shared Function Get_Adaptors(header As SAMAlignmentHeader) As List(Of Szunyi.Transcipts.Adaptor_Pair)
                Dim APs As New Szunyi.Transcipts.Adaptor_Pairs
                Dim Res As New List(Of Szunyi.Transcipts.Adaptor_Pair)
                For Each Item In header.Comments
                    Dim s = Split(Item, vbTab)
                    Dim s1 = Split(s.Last, ",")
                    If s1.Count = 5 Then
                        If s1.First.Length = 1 And (s1.Last = "True" Or s1.Last = "False") Then
                            Try
                                Dim Adaptore_Five_Prime As New Bio.Sequence(Alphabets.DNA, s1(2))
                                Dim Adaptore_Three_Prime As New Bio.Sequence(Alphabets.DNA, s1(3))
                                Dim r = From x In APs.A_Ps Where x.Name = s1(1) And x.PreFix = s1(0)
                                If r.Count = 1 Then
                                    Res.Add(r.First)
                                End If
                            Catch ex As Exception
                                Dim kj As Int16 = 54
                            End Try
                        End If
                    End If
                Next
                Return Res
            End Function
            Public Shared Function Get_Adaptors(headers As List(Of SAMAlignmentHeader)) As List(Of Szunyi.Transcipts.Adaptor_Pair)
                Dim res As New List(Of Szunyi.Transcipts.Adaptor_Pair)
                For Each Header In headers
                    res.AddRange(Get_Adaptors(Header))
                Next
                Return res.Distinct.ToList
            End Function
            Public Shared Function Get_Adaptors(Files As List(Of FileInfo)) As List(Of Szunyi.Transcipts.Adaptor_Pair)
                Dim res As New List(Of Szunyi.Transcipts.Adaptor_Pair)
                For Each File In Files
                    res.AddRange(Get_Adaptors(File))
                Next
                Return res.Distinct.ToList
            End Function
            Public Shared Function Get_Adaptors(File As FileInfo) As List(Of Szunyi.Transcipts.Adaptor_Pair)
                Dim Header = Szunyi.BAM.Bam_Basic_IO.Headers.Get_Header(File)
                Return Get_Adaptors(Header)
            End Function

            Friend Shared Function Merge(headers As List(Of SAMAlignmentHeader)) As SAMAlignmentHeader
                Dim Basic_Header = headers.First
                For i1 = 1 To headers.Count - 1
                    For Each RS In headers(i1).ReferenceSequences
                        Dim IsIt = From x In Basic_Header.ReferenceSequences Where x.Name = RS.Name

                        If IsIt.Count = 0 Then
                            Basic_Header.ReferenceSequences.Add(RS)
                        End If
                    Next
                    For Each RS In headers(i1).RecordFields
                        If RS.Typecode = "RG" Then
                            Dim IsIt = From x In Basic_Header.RecordFields Where x.ToString = RS.ToString
                            Basic_Header.RecordFields.Add(RS)
                            If IsIt.Count = 0 Then Basic_Header.RecordFields.Add(RS)
                        End If
                        If RS.Typecode = "SQ" Then
                            Dim IsIt = From x In Basic_Header.RecordFields Where x.ToString = RS.ToString
                            If IsIt.Count = 0 Then Basic_Header.RecordFields.Add(RS)
                        End If
                    Next

                    For Each RS In headers(i1).Comments

                        Dim IsIt = From x In Basic_Header.Comments Where x = RS

                        If IsIt.Count = 0 Then Basic_Header.Comments.Add(RS)
                    Next
                Next
                Return Basic_Header
            End Function
            Friend Shared Function Get_RGs(files As List(Of FileInfo)) As List(Of String)
                Dim Headers = Szunyi.BAM.Bam_Basic_IO.Headers.Get_Header(files)
                Dim RGS As New List(Of SAMRecordField)
                For Each h In Headers
                    Dim x = From t In h.RecordFields Where t.Typecode = "RG"
                    RGS.AddRange(x)
                Next
                Dim IDs As New List(Of String)
                For Each r In RGS
                    Dim ks = From x In r.Tags Where x.Tag = "ID"
                    For Each k In ks
                        IDs.Add(k.Value)
                    Next

                Next
                Return IDs.Distinct.ToList
            End Function
            Friend Shared Function Add_RGs(header As SAMAlignmentHeader, files As List(Of FileInfo)) As SAMAlignmentHeader
                For Each FIle In files
                    Dim x As New Bio.IO.SAM.SAMRecordField("RG")
                    x.Tags.Add(New SAMRecordFieldTag("ID", FIle.Name))
                    header.RecordFields.Add(x)
                Next
                Return header
            End Function
            Friend Shared Function Add_RGs(header As SAMAlignmentHeader, s() As String) As SAMAlignmentHeader
                For Each FIle In s
                    Dim x As New Bio.IO.SAM.SAMRecordField("RG")
                    x.Tags.Add(New SAMRecordFieldTag("ID", FIle))
                    header.RecordFields.Add(x)
                Next
                Return header
            End Function
#End Region

#Region "Reference Seqs"
            ''' <summary>
            ''' return all unique Reference Sequence info
            ''' </summary>
            ''' <param name="Files"></param>
            ''' <returns></returns>
            Public Shared Function Get_ReferenceSequences(Files As List(Of FileInfo)) As List(Of Bio.IO.SAM.ReferenceSequenceInfo)
                Dim Headers = Bam_Basic_IO.Headers.Get_Header(Files)

                Dim RefSeqs As New List(Of Bio.IO.SAM.ReferenceSequenceInfo)
                For Each Header In Headers
                    RefSeqs.AddRange(Header.ReferenceSequences)
                Next
                Dim uRefSeqs = From c In RefSeqs Select New With {Key c.Name, c.Length} Distinct.ToList
                Dim out As New List(Of Bio.IO.SAM.ReferenceSequenceInfo)
                For Each RefSeq In uRefSeqs
                    out.Add(New Bio.IO.SAM.ReferenceSequenceInfo(RefSeq.Name, RefSeq.Length))
                Next
                Return out
            End Function

            ''' <summary>
            ''' return all unique Reference Sequence info
            ''' </summary>
            ''' <param name="File"></param>
            ''' <returns></returns>
            Public Shared Function Get_ReferenceSequences(File As FileInfo) As List(Of Bio.IO.SAM.ReferenceSequenceInfo)
                Dim Headers = Bam_Basic_IO.Headers.Get_Header(File)

                Dim RefSeqs As New List(Of Bio.IO.SAM.ReferenceSequenceInfo)

                RefSeqs.AddRange(Headers.ReferenceSequences)

                Dim uRefSeqs = From c In RefSeqs Select New With {Key c.Name, c.Length} Distinct.ToList
                Dim out As New List(Of Bio.IO.SAM.ReferenceSequenceInfo)
                For Each RefSeq In uRefSeqs
                    out.Add(New Bio.IO.SAM.ReferenceSequenceInfo(RefSeq.Name, RefSeq.Length))
                Next
                Return out
            End Function

            ''' <summary>
            ''' Return DIstinct SeqIDs
            ''' </summary>
            ''' <param name="Bams"></param>
            ''' <returns></returns>
            Public Shared Function Select_Reference_SeqIDs(Bams As List(Of FileInfo)) As List(Of String)
                Dim IDs = Get_Reference_SeqIDS(Bams)
                Return IDs.Distinct.ToList
            End Function

            ''' <summary>
            ''' Return Distinct list of SeqIDs or new list
            ''' </summary>
            ''' <param name="Bams"></param>
            ''' <returns></returns>
            Public Shared Function Get_Reference_SeqIDS(Bams As List(Of FileInfo)) As List(Of String)
                Dim IDs = Get_ReferenceSequences(Bams)
                If IDs.Count > 0 Then
                    Dim res = (From x In IDs Select x.Name).ToList
                    Return res.Distinct.ToList
                Else
                    Return New List(Of String)
                End If
            End Function


            ''' <summary>
            ''' Return Distinct list of SeqIDs or new list Ordered
            ''' </summary>
            ''' <param name="Bam"></param>
            ''' <returns></returns>
            Public Shared Function Get_Reference_SeqIDS(ByRef BAm As FileInfo) As List(Of String)
                Dim IDs = Get_ReferenceSequences(BAm)
                If IDs.Count > 0 Then
                    Dim res = (From x In IDs Select x.Name).ToList
                    res = res.Distinct.ToList
                    res.Sort()
                    Return res.ToList
                Else
                    Return New List(Of String)
                End If
            End Function
#End Region


        End Class

        Public Class Import

#Region "Read IDs"
            Public Shared Function Get_Sorted_Distinct_Read_IDs(files As List(Of FileInfo)) As List(Of String)
                Dim Read_IDs = Get_Read_IDs(files)
                Read_IDs.Distinct
                Read_IDs.Sort()
                Return Read_IDs

            End Function
            Public Shared Function Get_Sorted_Distinct_Read_IDs(file As FileInfo) As List(Of String)
                Dim Read_IDs = Get_Read_IDs(file)
                Read_IDs = Read_IDs.Distinct.ToList
                Read_IDs.Sort()
                Return Read_IDs

            End Function
            Public Shared Function Get_Read_IDs(file As FileInfo) As List(Of String)
                Dim out As New List(Of String)
                For Each sam In Szunyi.BAM.Bam_Basic_IO.Import.Parse(file)
                    out.Add(sam.QName)
                Next
                Return out
            End Function
            Public Shared Function Get_Read_IDs(files As List(Of FileInfo)) As List(Of String)
                Dim out As New List(Of String)
                For Each sam In Szunyi.BAM.Bam_Basic_IO.Import.Parse(files)
                    out.Add(sam.QName)
                Next
                Return out
            End Function
            Public Shared Function Get_Sorted_Read_IDs(file As FileInfo) As List(Of String)
                Dim Read_IDs = Get_Read_IDs(file)
                Read_IDs.Sort()
                Return Read_IDs
            End Function
#End Region
#Region "Parse"
            Public Shared Function ParseAll_Into_Locations(Files As List(Of FileInfo)) As List(Of Bio.IO.GenBank.ILocation)
                Dim out As New List(Of Bio.IO.GenBank.ILocation)
                For Each FIle In Files
                    out.AddRange(ParseAll_Into_Locations(FIle))
                Next
                Return out
            End Function
            Public Shared Function ParseAll_Into_Locations(File As FileInfo) As List(Of Bio.IO.GenBank.ILocation)
                Dim out As New List(Of Bio.IO.GenBank.ILocation)
                For Each SAM In Parse(File)
                    out.Add(Szunyi.Location.Common.GetLocation(SAM))
                Next
                Return out
            End Function
            Public Shared Function ParseAll_Into_Basic_Locations(Files As List(Of FileInfo)) As List(Of Szunyi.Location.Basic_Location)
                Dim out As New List(Of Szunyi.Location.Basic_Location)
                For Each FIle In Files
                    out.AddRange(ParseAll_Into_Basic_Locations(FIle))
                Next
                Return out
            End Function
            Public Shared Function ParseAll_Into_Basic_Locations(File As FileInfo) As List(Of Szunyi.Location.Basic_Location)
                Dim out As New List(Of Szunyi.Location.Basic_Location)
                Dim SAMs = Parse(File)
                Parallel.ForEach(SAMs, Sub(SAM)
                                           Dim x = (Szunyi.Location.Common.GetLocation(SAM))
                                           SyncLock out
                                               out.Add(Szunyi.Location.Basic_Location_Manipulation.Get_Basic_Location(x))
                                           End SyncLock
                                       End Sub)

                Return out
            End Function

            Public Shared Function ParseAll(Files As List(Of FileInfo)) As List(Of Bio.IO.SAM.SAMAlignedSequence)
                Dim allsam As New List(Of Bio.IO.SAM.SAMAlignedSequence)
                For Each File In Files
                    allsam.AddRange(ParseAll(File))
                Next
                Return allsam
            End Function
            Public Shared Function ParseAll(fIle As FileInfo) As List(Of Bio.IO.SAM.SAMAlignedSequence)
                Dim allsam As New List(Of Bio.IO.SAM.SAMAlignedSequence)
                For Each Sam In Parse(fIle)

                    allsam.Add(Sam)

                Next
                Return allsam
            End Function
            Public Shared Iterator Function Parse(File As FileInfo) As IEnumerable(Of Bio.IO.SAM.SAMAlignedSequence)
                If File.Extension = ".bam" Then
                    Using sr As New FileStream(File.FullName, FileMode.Open)

                        Dim sa As New Bio.IO.BAM.BAMParser()
                        For Each SAM As Bio.IO.SAM.SAMAlignedSequence In sa.Parse(sr)
                            Yield (SAM)
                        Next
                    End Using
                ElseIf File.Extension = ".sam" Then
                    For Each Line In Szunyi.IO.Import.Text.Parse(File, "@")
                        If Line <> "" Then
                            Yield Bio.IO.SAM.SAMParser.ParseSequence(Line)
                        End If

                    Next
                End If


            End Function

            Public Shared Iterator Function Parse(File As FileInfo, SeqID As String, Start As Integer, Endy As Integer) As IEnumerable(Of Bio.IO.SAM.SAMAlignedSequence)
                Dim sa As New Bio.IO.BAM.BAMParser()
                Dim sg = Bio.IO.BAM.BAMParserExtensions.ParseRange(sa, File.FullName, SeqID, Start, Endy)
                If IsNothing(sg) = False Then

                    For Each Sam In sg.AlignedSequences
                        Yield (Sam)
                    Next
                End If

            End Function

            Public Shared Iterator Function Parse(Files As List(Of FileInfo), SeqID As String) As IEnumerable(Of Bio.IO.SAM.SAMAlignedSequence)
                Dim sa As New Bio.IO.BAM.BAMParser()
                For Each file In Files
                    Dim st = Bio.IO.BAM.BAMParserExtensions.ParseRange(sa, file.FullName, SeqID)

                    For Each Sam In st.AlignedSequences
                        Yield (Sam)
                    Next

                Next
            End Function

            Public Shared Iterator Function Parse(File As FileInfo, SeqID As String) As IEnumerable(Of Bio.IO.SAM.SAMAlignedSequence)
                Dim sa As New Bio.IO.BAM.BAMParser()
                Dim sg = Bio.IO.BAM.BAMParserExtensions.ParseRange(sa, File.FullName, SeqID)
                If IsNothing(sg) = False Then

                    For Each Sam In sg.AlignedSequences
                        Yield (Sam)
                    Next
                End If

            End Function

            Public Shared Iterator Function Parse(Files As List(Of FileInfo), SeqIDs As List(Of String)) As IEnumerable(Of Bio.IO.SAM.SAMAlignedSequence)
                Dim sa As New Bio.IO.BAM.BAMParser()
                For Each File In Files
                    For Each SeqID In SeqIDs

                        For Each Sam In Bio.IO.BAM.BAMParserExtensions.ParseRange(sa, File.FullName, SeqID).AlignedSequences
                            Yield (Sam)
                        Next

                    Next
                Next


            End Function

            Public Shared Iterator Function Parse(Files As List(Of FileInfo)) As IEnumerable(Of Bio.IO.SAM.SAMAlignedSequence)
                For Each File In Files
                    For Each sam In Parse(File)
                        Yield sam
                    Next
                Next
            End Function

            Public Shared Function ParseAll_IntoStat(FIle As FileInfo) As List(Of Simple_Stat)
                Dim current As New List(Of Simple_Stat)
                Dim Header = Szunyi.BAM.Bam_Basic_IO.Headers.Get_Header(FIle)
                Dim Index As Integer = 0
                For Each SAM In Szunyi.BAM.Bam_Basic_IO.Import.Parse(FIle)

                    If SAM.Flag <> SAMFlags.UnmappedQuery Then
                        Index += 1
                        If IsNothing(SAM.QuerySequence) = False Then
                            current.Add(New Simple_Stat(SAM, Index))
                        End If
                    End If
                Next
                Return current

            End Function
            Public Shared Iterator Function ParseAll_IntoStat_Bests(FIle As FileInfo) As IEnumerable(Of Simple_Stat)
                Dim current As New List(Of Simple_Stat)
                Dim Header = Szunyi.BAM.Bam_Basic_IO.Headers.Get_Header(FIle)
                Dim Index As Integer = 0
                For Each SAM In Szunyi.BAM.Bam_Basic_IO.Import.Parse(FIle)

                    If SAM.Flag <> SAMFlags.UnmappedQuery Then
                        Index += 1
                        If IsNothing(SAM.QuerySequence) = False Then
                            current.Add(New Simple_Stat(SAM, Index))
                        End If
                    End If
                Next

                Dim r = From x In current Group By x.Read_ID Into Group

                For Each gr In r
                    Dim Best = From k In gr.Group Order By k.Match.Sum Descending
                    Yield Best.First
                Next

            End Function

            Public Shared Iterator Function Parse_Into_Basic_Locations(File As FileInfo) As IEnumerable(Of Szunyi.Location.Basic_Location)
                For Each SAM In Parse(File)
                    If SAM.Flag <> SAMFlags.UnmappedQuery Then
                        If SAM.QName = "m161125_121431_42197_c101121862550000001823256805221734_s1_p0/87604/ccs" Then
                            Dim jj As Int16 = 65
                        End If
                        Dim l = Szunyi.Location.Common.GetLocation(SAM)

                        Yield Szunyi.Location.Basic_Location_Manipulation.Get_Basic_Location(l)
                    End If
                Next

            End Function
            Public Shared Iterator Function Parse_Into_Basic_Locations(Files As List(Of FileInfo)) As IEnumerable(Of Szunyi.Location.Basic_Location)
                For Each SAM In Parse(Files)
                    If SAM.Flag <> SAMFlags.UnmappedQuery Then
                        Dim l = Szunyi.Location.Common.GetLocation(SAM)
                        Yield Szunyi.Location.Basic_Location_Manipulation.Get_Basic_Location(l)
                    End If
                Next

            End Function
#End Region
        End Class


    End Namespace


End Namespace
