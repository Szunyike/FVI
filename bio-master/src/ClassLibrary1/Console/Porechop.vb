Imports System.IO
Namespace Szunyi.Console.Porechop
    Public Class ByReadID
        Implements IComparer(Of Result)

        Public Function Compare(x As Result, y As Result) As Integer Implements IComparer(Of Result).Compare
            Return x.ReadID.CompareTo(y.ReadID)
        End Function

    End Class
    Public Class Porechop
        Public Shared Sub Set_Orientations(x As Result, Five_Adaptors As List(Of String), Three_Adaptors As List(Of String))
            For Each Item In Five_Adaptors

                If IsNothing(x.Five_Prime_Adaptors(Item)) = True And IsNothing(x.Three_Prime_Adaptors(Item)) = True Then
                    x.Orientation_By_Adapters.Add(Item, Transcipts.Orientation.Type.none)
                Else
                    If IsNothing(x.Five_Prime_Adaptors(Item)) = True Then
                        x.Orientation_By_Adapters.Add(Item, Transcipts.Orientation.Type.rev)
                    ElseIf IsNothing(x.Three_Prime_Adaptors(Item)) = True Then
                        x.Orientation_By_Adapters.Add(Item, Transcipts.Orientation.Type.fw)
                    Else
                        x.Orientation_By_Adapters.Add(Item, Transcipts.Orientation.Type.both)
                    End If

                End If
            Next

            For Each Item In Three_Adaptors
                If IsNothing(x.Five_Prime_Adaptors(Item)) = True And IsNothing(x.Three_Prime_Adaptors(Item)) = True Then
                    x.Orientation_By_Adapters.Add(Item, Transcipts.Orientation.Type.none)
                Else

                    If IsNothing(x.Five_Prime_Adaptors(Item)) = True Then
                        x.Orientation_By_Adapters.Add(Item, Transcipts.Orientation.Type.fw)
                    ElseIf IsNothing(x.Three_Prime_Adaptors(Item)) = True Then
                        x.Orientation_By_Adapters.Add(Item, Transcipts.Orientation.Type.rev)
                    Else
                        x.Orientation_By_Adapters.Add(Item, Transcipts.Orientation.Type.both)
                    End If

                End If
            Next
            Dim ALL_Orientation = From x1 In x.Orientation_By_Adapters Select x1.Value
            x.Final_Orientation = Transcipts.Orientation.Get_Orientation(ALL_Orientation)
        End Sub
        Public Shared Function Get_it(Files As List(Of FileInfo), Dir As DirectoryInfo) As String
            Dim str As New System.Text.StringBuilder
            For Each FIle In Files
                str.Append("porechop -i ").Append(Szunyi.IO.Linux.Get_FileName(FIle))
                str.Append(" -b ").Append(Szunyi.IO.Linux.Get_FileName(Dir))
                str.Append(" -t ").Append(Environment.ProcessorCount - 1)
                str.Append(" -v 3")
                str.Append(" --no_split")
                str.Append(" --untrimmed")
                str.Append(" --verbosity 3")
                str.Append(" |tee ")
                Dim nFIle = Szunyi.IO.Files.Get_New_FileName.Replace_Extension(FIle, Szunyi.Constants.File_Extensions.PoreChop)
                str.Append(Szunyi.IO.Linux.Get_FileName(nFIle))
                str.AppendLine()
            Next
            Return str.ToString
        End Function

        Public Shared Function Get_Adapters(File As FileInfo) As List(Of String)
            Dim Found As Boolean = False
            Dim out As New List(Of String)
            For Each Line In Szunyi.IO.Import.Text.Parse(File)
                If Line.StartsWith("                                        start      read end") Then
                    Found = True
                End If
                If Found = True Then
                    If Line = String.Empty Then Exit For
                    out.Add(Split(Line.Substring(0, 44), ("32m")).Last.Trim(" "))

                End If
            Next
            Return out
        End Function

        Public Shared Function Get_Midle_Adapters(s As List(Of String)) As KeyValuePair(Of String, List(Of Porechop_Adaptor))
            Dim res As New KeyValuePair(Of String, List(Of Porechop_Adaptor))(s.First, New List(Of Porechop_Adaptor))
            For i1 = 1 To s.Count - 1
                Dim s1 = Split(s(i1), " (read coords: ")
                Dim x As New Porechop_Adaptor
                x.ID = s1.First.Trim
                Dim s2 = Split(s1(1), ",")
                Dim loc = s2(0).Replace("-", "..").Trim
                x.Location = Szunyi.Location.Common.Get_Location(loc)
                Dim iden = s2(1).Replace(" identity: ", "").Replace(" ", "").Replace("%)", "")
                x.full_Score = iden
                res.Value.Add(x)
            Next
            Return res
        End Function

        Public Shared Sub Set_TSS_PAS(x As Result, for_TSS As List(Of String), for_PAS As List(Of String))
            If x.Final_Orientation = Transcipts.Orientation.Type.fw Or x.Final_Orientation = Transcipts.Orientation.Type.rev Then
                For Each TSS In for_TSS
                    If x.Orientation_By_Adapters(TSS) = Transcipts.Orientation.Type.fw Or x.Orientation_By_Adapters(TSS) = Transcipts.Orientation.Type.rev Then
                        x.For_TSS = True
                    End If
                Next
                For Each PAS In for_PAS
                    If x.Orientation_By_Adapters(PAS) = Transcipts.Orientation.Type.fw Or x.Orientation_By_Adapters(PAS) = Transcipts.Orientation.Type.rev Then
                        x.For_PAS = True
                    End If
                Next
            End If
        End Sub
        ''' <summary>
        ''' Return Sorted by ReadID
        ''' </summary>
        ''' <param name="files"></param>
        ''' <returns></returns>
        Public Shared Function Get_Short_Porechop_Result(files As List(Of FileInfo), Max_Nof_Mismatcxh As Integer) As List(Of Short_Result)
            Dim Out As New List(Of Short_Result)
            For Each FIle In files
                For Each line In Szunyi.IO.Import.Text.Parse(FIle, "#")
                    Out.Add(New Short_Result(line, Max_Nof_Mismatcxh))
                Next
            Next
            Out.Sort(New Short_Result_ByReadID)
            Return Out
        End Function
    End Class
    Public Class Short_Result_ByReadID
        Implements IComparer(Of Short_Result)

        Public Function Compare(x As Short_Result, y As Short_Result) As Integer Implements IComparer(Of Short_Result).Compare
            Return x.ReadID.CompareTo(y.ReadID)
        End Function
    End Class
    Public Class Short_Result
        Public Property Orientation As Szunyi.Transcipts.Orientation.Type
        Public Property For_TSS As Boolean
        Public Property For_Pas As Boolean
        Public Property ReadID As String
        Public Property Five_Prime_Seq As String
        Public Property Three_Prime_Seq As String
        Public Property TSS_Location_Five As Bio.IO.GenBank.Location
        Public Property TSS_Location_Three As Bio.IO.GenBank.Location
        Public Property PAS_Location_Five As Bio.IO.GenBank.Location
        Public Property PAS_Location_Three As Bio.IO.GenBank.Location

        Public Property TSS_Location_Five_final_score As Integer = 0
        Public Property TSS_Location_Three_final_score As Integer = 0
        Public Property PAS_Location_Five_final_score As Integer = 0
        Public Property PAS_Location_Three_final_score As Integer = 0
        Public Sub New(Line As String, Max_Nof_Mismatcxh As Integer)
            Dim s = Split(Line, vbTab)
            If Line = String.Empty Then Exit Sub
            Me.ReadID = s.First
            Me.Five_Prime_Seq = s(1)
            Me.Three_Prime_Seq = s(2)

            ' 3 BC
            Me.Orientation = s(4)
            Me.For_TSS = s(5)
            Me.For_Pas = s(6)
            If s(11) <> String.Empty Then TSS_Location_Five_final_score = s(11)
            If s(12) <> String.Empty Then TSS_Location_Three_final_score = s(12)
            If s(13) <> String.Empty Then PAS_Location_Five_final_score = s(13)
            If s(14) <> String.Empty Then PAS_Location_Three_final_score = s(14)

            Me.TSS_Location_Five = Szunyi.Location.Common.Get_Location(s(7))
            If IsNothing(Me.TSS_Location_Five) = False Then
                Me.TSS_Location_Five = Szunyi.Location.Modify.Change_Location_Start(TSS_Location_Five, 1)
            End If
            Me.TSS_Location_Three = Szunyi.Location.Common.Get_Location(s(8))
            If IsNothing(Me.TSS_Location_Three) = False Then
                Me.TSS_Location_Three = Szunyi.Location.Modify.Change_Location_Start(TSS_Location_Three, 1)
            End If
            Me.PAS_Location_Five = Szunyi.Location.Common.Get_Location(s(9))
            Dim cNof_MisMatch As Integer = 0
            If IsNothing(Me.PAS_Location_Five) = False Then
                Me.PAS_Location_Five = Szunyi.Location.Modify.Change_Location_Start(PAS_Location_Five, 1)
                Dim i1 As Int16 = 0
                For i1 = Me.PAS_Location_Five.LocationEnd To Me.Five_Prime_Seq.Count - 1
                    Dim c = Me.Five_Prime_Seq(i1)
                    If c <> "T" Then
                        cNof_MisMatch += 1
                        If cNof_MisMatch > Max_Nof_Mismatcxh Then
                            Exit For
                        End If
                    End If
                        Dim jj As Int16 = 54
                Next
                If i1 <> Me.PAS_Location_Five.LocationEnd Then
                    Me.PAS_Location_Five = Szunyi.Location.Common.GetLocation(Me.PAS_Location_Five.LocationStart, i1)
                    Dim kk As Int16 = 54
                End If
                Dim jk As Int16 = 54
            End If
            Me.PAS_Location_Three = Szunyi.Location.Common.Get_Location(s(10))
            If IsNothing(Me.PAS_Location_Three) = False Then
                Me.PAS_Location_Three = Szunyi.Location.Modify.Change_Location_Start(PAS_Location_Three, 1)
            End If

        End Sub
        Public Sub New()

        End Sub
    End Class

    Public Class Result
        Public Property ReadID As String
        Public Property Five_Prime_Adaptors As New Dictionary(Of String, Porechop_Adaptor)
        Public Property Three_Prime_Adaptors As New Dictionary(Of String, Porechop_Adaptor)
        Public Property Middle_Prime_Adaptors As New Dictionary(Of String, Porechop_Adaptor)
        Public Property best_start_barcode As String
        Public Property best_end_barcode As String
        Public Property final_barcode As String
        Public Property Adaptors As String
        Public Property Start_Adaptors As String
        Public Property End_Adaptors As String
        Public Property Orientation_By_Adapters As New Dictionary(Of String, Szunyi.Transcipts.Orientation.Type)
        Public Property Final_Orientation As Szunyi.Transcipts.Orientation.Type
        Public Property For_TSS As Boolean = False
        Public Property For_PAS As Boolean = False

        Public Property Five_Prime_Seq As String
        Public Property Three_Prime_Seq As String

        Public Sub New(ReadID As String)
            Me.ReadID = ReadID
        End Sub
        Public Sub New(Lines As List(Of String), Interesting_Adapters As List(Of String), Minimum_Full_Score As Integer)
            Me.ReadID = Lines.First
            For Each Item In Interesting_Adapters
                Five_Prime_Adaptors.Add(Item, Nothing)
                Three_Prime_Adaptors.Add(Item, Nothing)
                Middle_Prime_Adaptors.Add(Item, Nothing)
            Next
            Dim FiveSeq As String = ""
            Dim ThreeSeq As String = ""
            For i1 = 0 To Lines.Count - 1

                If Lines(i1).Contains("start:") Then
                    Dim s = Split(Lines(i1), "start:")
                    FiveSeq = Bio.Alphabets.DNA.Remove_Non_Alphabetic_Chars(s.Last)
                    Me.Five_Prime_Seq = FiveSeq
                End If
                If Lines(i1).Contains("end:") Then
                    Dim s = Split(Lines(i1), "end:")
                    ThreeSeq = Bio.Alphabets.DNA.Remove_Non_Alphabetic_Chars(s.Last)
                    Me.Three_Prime_Seq = ThreeSeq
                End If
                If Lines(i1).Contains("start alignments") Then
                    For i2 = i1 + 1 To Lines.Count - 1
                        If Lines(i2).Substring(3, 1) <> " " Then
                            i1 = i2 - 1
                            Exit For
                        End If
                        Dim x As New Porechop_Adaptor(Lines(i2), Interesting_Adapters)
                        If x.full_Score > Minimum_Full_Score AndAlso x.ID <> "" Then Five_Prime_Adaptors(x.ID) = x
                    Next
                End If
                If Lines(i1).Contains("end alignments") Then
                    For i2 = i1 + 1 To Lines.Count - 1
                        If Lines(i2).Substring(3, 1) <> " " Then
                            i1 = i2 - 1
                            Exit For
                        End If
                        Dim x As New Porechop_Adaptor(Lines(i2), Interesting_Adapters)
                        If x.full_Score > Minimum_Full_Score AndAlso x.ID <> "" Then Three_Prime_Adaptors(x.ID) = x

                    Next
                End If
                If Lines(i1).Contains("best start barcode:") Then
                    best_start_barcode = Split(Lines(i1), " ")(10)
                ElseIf Lines(i1).Contains("best end barcode:") Then
                    best_end_barcode = Split(Lines(i1), " ")(12)
                ElseIf Lines(i1).Contains("final barcode call:") Then
                    final_barcode = Split(Lines(i1), " ")(10)
                    Exit For
                End If

            Next
            Dim str As New System.Text.StringBuilder
            For Each Item In Me.Five_Prime_Adaptors
                If IsNothing(Item.Value) = True Then
                    str.Append("-")
                Else
                    str.Append("+")
                End If
            Next
            Me.Start_Adaptors = str.ToString
            str.Length = 0
            For Each Item In Me.Three_Prime_Adaptors
                If IsNothing(Item.Value) = True Then
                    str.Append("-")
                Else
                    str.Append("+")
                End If
            Next
            Me.End_Adaptors = str.ToString
            Me.Adaptors = Me.Start_Adaptors & Me.End_Adaptors

        End Sub
        Public Function Get_Header() As String
            Dim str As New System.Text.StringBuilder
            str.Append("ReadID").Append(vbTab)
            str.Append("final barcode").Append(vbTab)
            str.Append("best start barcode").Append(vbTab)
            str.Append("best end barcode").Append(vbTab)
            For Each Item In Me.Five_Prime_Adaptors
                str.Append("Start:" & Item.Key).Append(vbTab)
            Next
            For Each Item In Me.Five_Prime_Adaptors
                str.Append("End:" & Item.Key).Append(vbTab)
            Next
            str.Append("Aggregate")
            Return str.ToString
        End Function
        Public Overrides Function ToString() As String
            Dim str As New System.Text.StringBuilder
            str.Append(Me.ReadID).Append(vbTab)
            str.Append(Me.final_barcode).Append(vbTab)
            str.Append(Me.best_start_barcode).Append(vbTab)
            str.Append(Me.best_end_barcode).Append(vbTab)
            Dim tmp As New System.Text.StringBuilder
            For Each Item In Me.Five_Prime_Adaptors
                If IsNothing(Item.Value) = True Then
                    str.Append("-")
                    tmp.Append(Szunyi.Text.General.Multiply("-" & vbTab, 4))
                Else
                    str.Append("+")
                    tmp.Append(Item.Value.Get_Text).Append(vbTab)
                End If
                str.Append(vbTab)
            Next
            For Each Item In Me.Three_Prime_Adaptors
                If IsNothing(Item.Value) = True Then
                    tmp.Append(Szunyi.Text.General.Multiply("-" & vbTab, 4))
                    str.Append("-")
                Else
                    str.Append("+")
                    tmp.Append(Item.Value.Get_Text).Append(vbTab)
                End If
                str.Append(vbTab)
            Next
            Return str.ToString & tmp.ToString
        End Function
    End Class

    Public Class Porechop_Adaptor

        Public Property ID As String
        Public Property full_Score As Integer
        Public Property partial_Score As Integer
        Public Property Location As Bio.IO.GenBank.Location
        Public Property forTSS As Boolean = False
        Public Property forPAS As Boolean = False
        Public Property Orientation As Szunyi.Transcipts.Orientation.Type = Transcipts.Orientation.Type.unknown

        Public Sub New()

        End Sub
        Public Sub New(Line As String, Optional Interesting_Adapters As List(Of String) = Nothing)
            Dim s = Split(Line.Trim, ",")
            If IsNothing(Interesting_Adapters) = False AndAlso Interesting_Adapters.Contains(s(0)) = False Then Exit Sub

            Me.ID = s(0)
            Me.full_Score = Split(s(1), "=").Last
            Me.partial_Score = Split(s(2), "=").Last
            Dim l = Split(s(3), ":")
            Me.Location = Szunyi.Location.Common.Get_Location(l.Last.Replace("-", ".."))

        End Sub
        Public Function Get_Text() As String
            Return Me.ID & vbTab & Me.full_Score & vbTab & Me.partial_Score & vbTab & Szunyi.Location.Common.GetLocationString(Me.Location)
        End Function

    End Class
    Public Class Statistic
        Public Shared Function ByBarCode(ls As List(Of Result)) As String
            Dim str As New System.Text.StringBuilder

        End Function
        Public Shared Function ByAdaptor(ls As List(Of Result)) As String

        End Function
        Public Shared Function ByBarCodeAndAdaptor_Orientation(ls As List(Of Result), Adaptors As List(Of String), Minimum_Final_Score As Integer)
            Dim nofAdapter = Adaptors.Count
            Dim Permutation = Szunyi.Math.Combinatory.GetBinaryPermutation(nofAdapter)
            Dim str As New System.Text.StringBuilder

            For Each BC In GroupBy.ByBarCode(ls)
                If BC.Count > 0 AndAlso BC.First.final_barcode <> "" Then
                    Dim TSS = From x In BC Where x.For_TSS = True
                    str.Append(Create_Adaptor_Groups(TSS, Adaptors, Permutation, BC.First.final_barcode & vbTab & "ForTSS" & vbTab, Minimum_Final_Score))

                    Dim PAS = From x In BC Where x.For_PAS = True
                    str.Append(Create_Adaptor_Groups(PAS, Adaptors, Permutation, BC.First.final_barcode & vbTab & "ForPAS" & vbTab, Minimum_Final_Score))

                    Dim none = From x In BC Where x.Final_Orientation = Transcipts.Orientation.Type.none
                    str.Append(Create_Adaptor_Groups(none, Adaptors, Permutation, BC.First.final_barcode & vbTab & "none" & vbTab, Minimum_Final_Score))

                    Dim unknown = From x In BC Where x.Final_Orientation = Transcipts.Orientation.Type.unknown
                    str.Append(Create_Adaptor_Groups(unknown, Adaptors, Permutation, BC.First.final_barcode & vbTab & "unknown" & vbTab, Minimum_Final_Score))

                    Dim both = From x In BC Where x.Final_Orientation = Transcipts.Orientation.Type.both
                    str.Append(Create_Adaptor_Groups(both, Adaptors, Permutation, BC.First.final_barcode & vbTab & "both" & vbTab, Minimum_Final_Score))

                    Dim fw = From x In BC Where x.Final_Orientation = Transcipts.Orientation.Type.fw
                    str.Append(Create_Adaptor_Groups(fw, Adaptors, Permutation, BC.First.final_barcode & vbTab & "fw" & vbTab, Minimum_Final_Score))

                    Dim rev = From x In BC Where x.Final_Orientation = Transcipts.Orientation.Type.rev
                    str.Append(Create_Adaptor_Groups(rev, Adaptors, Permutation, BC.First.final_barcode & vbTab & "rev" & vbTab, Minimum_Final_Score))

                    Dim Orientated = From x In BC Where x.Final_Orientation = Transcipts.Orientation.Type.fw Or x.Final_Orientation = Transcipts.Orientation.Type.rev
                    str.Append(Create_Adaptor_Groups(Orientated, Adaptors, Permutation, BC.First.final_barcode & vbTab & "fw-rev" & vbTab, Minimum_Final_Score))

                    str.Append(Create_Adaptor_Groups(BC, Adaptors, Permutation, BC.First.final_barcode & vbTab & "All" & vbTab, Minimum_Final_Score))
                End If
            Next
            Dim Header As New System.Text.StringBuilder
            Header.Append("barCode").Append(vbTab)
            Header.Append("SubSample").Append(vbTab)
            Header.Append("Found Adapters").Append(vbTab)
            Header.Append("Not Found Adapters").Append(vbTab)
            Header.Append("count of SubSample reads").Append(vbTab)
            Header.Append("count of barcoded reads").Append(vbTab)
            Header.Append("percent of SubSample/Sample").AppendLine()
            Return Header.ToString & str.ToString
        End Function

        Private Shared Function Create_Adaptor_Groups(ls As IEnumerable(Of Result), adaptors As List(Of String), permutations As List(Of Boolean()), Header As String, Minimum_Final_Score As Integer) As String
            Dim Founded As New List(Of List(Of Result))
            Dim notFounded As New List(Of List(Of Result))
            For Each Adaptor In adaptors
                Dim f = From x In ls Where x.Orientation_By_Adapters(Adaptor) <> Transcipts.Orientation.Type.none

                Dim nf = From x In ls Where x.Orientation_By_Adapters(Adaptor) = Transcipts.Orientation.Type.none

                If f.Count = 0 Then
                    Founded.Add(New List(Of Result))
                Else
                    Founded.Add(f.ToList)
                End If
                If nf.Count = 0 Then
                    notFounded.Add(New List(Of Result))
                Else

                    notFounded.Add(nf.ToList)
                End If

            Next
            Dim str As New System.Text.StringBuilder
            For Each perm In permutations
                Dim c As New List(Of Result)
                Dim started As Boolean = True
                Dim Founded_IN As String = ""
                For i1 = 0 To perm.Count - 1
                    If perm(i1) = True Then
                        Founded_IN = Founded_IN & adaptors(i1) & ","
                        If started = False Then
                            c = c.Intersect(Founded(i1)).ToList
                        Else
                            c = Founded(i1)
                            started = False
                        End If
                    End If
                Next
                str.Append(Header)
                str.Append(Founded_IN).Append(vbTab).Append(vbTab).Append(c.Count).Append(vbTab).Append(ls.Count).Append(vbTab).Append(c.Count * 100 / ls.Count).AppendLine()
            Next

            For Each perm In permutations
                Dim c As New List(Of Result)
                Dim started As Boolean = False
                Dim Founded_IN As String = ""
                Dim nFounded_IN As String = ""
                If perm(0) = True Then
                    c = Founded(0)
                Else
                    c = notFounded(0)
                End If
                For i1 = 0 To perm.Count - 1
                    If perm(i1) = True Then
                        Founded_IN = Founded_IN & adaptors(i1) & ","
                        c = c.Intersect(Founded(i1)).ToList
                    Else

                        nFounded_IN = nFounded_IN & adaptors(i1) & ","
                        c = c.Intersect(notFounded(i1)).ToList
                    End If
                Next
                str.Append(Header)
                str.Append(Founded_IN).Append(vbTab).Append(nFounded_IN).Append(vbTab).Append(c.Count).Append(vbTab).Append(ls.Count).Append(vbTab).Append(c.Count * 100 / ls.Count).AppendLine()
            Next
            Return str.ToString
        End Function

        Public Shared Function ByBarCodeAndAdaptor(ls As List(Of Result)) As String
            Dim str As New System.Text.StringBuilder
            For Each BC In GroupBy.ByBarCode(ls)
                For Each BA In GroupBy.ByAdaptors(BC)
                    str.Append(BA.First.final_barcode).Append(vbTab)
                    str.Append(BA.First.Adaptors).Append(vbTab)
                    str.Append(BA.Count).AppendLine()
                    Dim nFIle As New FileInfo("I:\tmp\" & BA.First.final_barcode & "_" & BA.First.Adaptors & "_porechop.log")
                    Using sw As New StreamWriter(nFIle.FullName)
                        sw.Write(BA.First.Get_Header)
                        sw.WriteLine()
                        For Each Item In BA
                            sw.Write(Item.ToString)
                            sw.WriteLine()
                        Next
                    End Using
                Next
            Next
            If str.Length > 0 Then
                str.Length -= 1
                Return str.ToString
            End If
        End Function
    End Class
    Public Class GroupBy
        Public Shared Iterator Function ByBarCode(ls As IEnumerable(Of Result)) As IEnumerable(Of IEnumerable(Of Result))
            Dim gr = From x In ls Group By x.final_barcode Into Group

            For Each g In gr
                Yield g.Group
            Next

        End Function
        Public Shared Iterator Function ByBarCodeAndAdaptor(ls As IEnumerable(Of Result)) As IEnumerable(Of IEnumerable(Of Result))
            Dim gr = From x In ls Group By x.final_barcode, x.Adaptors Into Group

            For Each g In gr
                Yield g.Group
            Next

        End Function
        Public Shared Iterator Function ByAdaptors(ls As IEnumerable(Of Result)) As IEnumerable(Of IEnumerable(Of Result))
            Dim gr = From x In ls Group By x.Adaptors Into Group

            For Each g In gr
                Yield g.Group
            Next

        End Function
    End Class
End Namespace

