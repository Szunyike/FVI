Imports System.IO
Imports System.Text
Imports System.Globalization
Imports System.Threading
Imports System.Drawing

Namespace Szunyi
    Namespace Other_Database
        Namespace SignalP
            ''' <summary>
            ''' Need SignalP Html File and a Sequence List
            ''' </summary>
            Public Class SignalPBuilder
                Public Property Files As New List(Of FileInfo)
                Public Property Seqs As New List(Of Bio.ISequence)
                Public Property FullSeqs As New List(Of Bio.ISequence)
                Public Property CleavageSites As New List(Of SignalPCleavageSite)
                Public Property MaturePeptides As New List(Of Bio.ISequence)
                Public Property SignalPeptides As New List(Of Bio.ISequence)
                Public Property Seq_Lists As List(Of Szunyi.ListOf.SequenceList)
                Public Sub New(Seqs As List(Of Bio.ISequence), Optional Files As List(Of FileInfo) = Nothing)
                    Thread.CurrentThread.CurrentCulture = New CultureInfo("pt-BR")

                    Me.Seqs = Seqs
                    Me.Seqs.Sort(Szunyi.Comparares.AllComparares.BySeqID)
                    If IsNothing(Files) = True Then
                        Me.Files =  Szunyi.IO.Files.Filter.SelectFiles("Select SignalP File", Szunyi.Constants.Files.Other.htm)
                        If IsNothing(Files) = True Then Exit Sub
                    End If
                    Me.Files = Files
                    DoIt()

                End Sub
                Public Sub New(SeqList As List(Of Szunyi.ListOf.SequenceList), Optional Files As List(Of FileInfo) = Nothing)
                    Thread.CurrentThread.CurrentCulture = New CultureInfo("pt-BR")
                    Me.Seq_Lists = SeqList
                    If IsNothing(Files) = True Then
                        Files =  Szunyi.IO.Files.Filter.SelectFiles("Select SignalP File", Szunyi.Constants.Files.Other.htm)
                        If IsNothing(Files) = True Then Exit Sub
                    End If
                    Me.Files = Files

                    For Each Seq_list In Me.Seq_Lists
                        Me.Seqs = Seq_list.Sequences
                        Me.MaturePeptides.Clear()
                        Me.SignalPeptides.Clear()
                        DoIt()
                        Me.Seqs.Sort(Szunyi.Comparares.AllComparares.BySeqID)
                        Me.MaturePeptides.Sort(Szunyi.Comparares.AllComparares.BySeqID)
                        Me.SignalPeptides.Sort(Szunyi.Comparares.AllComparares.BySeqID)
                        Me.Save(Seq_list.Files.First)
                    Next



                End Sub
                Private Function Get_Cleavage_Sites() As List(Of SignalPCleavageSite)
                    Dim CleavageSites As New List(Of SignalPCleavageSite)
                    For Each File In Files
                        Dim Name As String = ""
                        For Each Line In Szunyi.IO.Import.Text.Parse(File)
                            If Line.StartsWith("<B>&gt;") Then
                                Name = Line.Substring(7, Line.Length - 8)
                                If Name.Contains(" ") Then
                                    Name = Name.Replace("_", "|")
                                End If
                            End If
                            If Line.StartsWith("Name=") = True Then
                                Name = Line.Split(vbTab).First.Replace("Name=", "")
                                Dim ClevagePosition = GetClevagePosition(Line) ' -1 ha nincs
                                If ClevagePosition > 0 Then
                                    CleavageSites.Add(New SignalPCleavageSite(Name, ClevagePosition))

                                Else
                                    CleavageSites.Add(New SignalPCleavageSite(Name, -1, 0))
                                End If
                            End If
                        Next
                    Next
                    CleavageSites.Sort(Szunyi.Comparares.AllComparares.By_SignalP)
                    Return CleavageSites
                End Function
                Public Sub DoIt()

                    Dim Clevage_Sites = Get_Cleavage_Sites()
                    For Each Seq In Me.Seqs
                        Dim The_Cleavege_Site_Index = Clevage_Sites.BinarySearch(New SignalPCleavageSite(Seq.ID, -1), Szunyi.Comparares.AllComparares.By_SignalP)
                        If The_Cleavege_Site_Index > -1 Then
                            Dim Cleavage_Site = Clevage_Sites(The_Cleavege_Site_Index)
                            If Cleavage_Site.Position > 0 Then
                                Me.FullSeqs.Add(Seq)
                                Me.MaturePeptides.Add(Seq.GetSubSequence(Cleavage_Site.Position, Seq.Count - Cleavage_Site.Position))
                                Me.SignalPeptides.Add(Seq.GetSubSequence(0, Cleavage_Site.Position))
                                Me.MaturePeptides.Last.ID = Seq.ID
                                Me.SignalPeptides.Last.ID = Seq.ID
                            Else
                                Dim NewSeq As New Bio.Sequence(Bio.Alphabets.AmbiguousProtein, "")
                                NewSeq.ID = Seq.ID
                                '     Me.FullSeqs.Add(Seq)
                                '    Me.MaturePeptides.Add(NewSeq)
                                '   Me.SignalPeptides.Add(NewSeq)
                            End If
                        Else
                            Dim alf As Int16 = 54
                            ' No Cleavege Site is Founded
                        End If

                    Next

                End Sub
                Private Function GetClevagePosition(Line As String) As Integer
                    Dim s1() As String = Split(Line, "SP='YES' Cleavage site between pos. ")
                    If s1.Length = 1 Then Return -1
                    Dim s2() = Split(s1.Last, ":")
                    Dim s3() = Split(s2.First, " ")
                    Dim s5() = Split(Line, "D=")
                    Dim s6 = Split(s5.Last, " ")
                    Return s3.First

                End Function
                Public Sub Save(Optional File As FileInfo = Nothing)
                    If IsNothing(FIle) = True Then
                        File = Szunyi.IO.Files.Save.SelectSaveFile(Szunyi.Constants.Files.SignalP, "Save SignalP File as ")

                    End If
                    If Me.CleavageSites.Count = 0 Then Exit Sub
                    Dim str As New StringBuilder
                    str.Append(Me.CleavageSites.First.GetHeader).AppendLine()
                    For Each Item In Me.CleavageSites
                        str.Append(Item.ToString).AppendLine()
                    Next
                    str.Length -= 2
                    '   Szunyi.IO.Export.SaveText(str.ToString, FIle)

                    Dim MPTDT As New FileInfo(File.DirectoryName & "\" & Szunyi.IO.Files.Get_New_FileName.GetFileName_woExtension(File) & "_MaturePeptides.TDT")
                    Dim MPSeqs = Szunyi.Sequences.SequenceManipulation.SeqsToString.GetSeqsAsString(Me.MaturePeptides)
                    Szunyi.IO.Export.SaveText(Szunyi.Text.General.GetText(MPSeqs, vbCrLf), MPTDT)
                    Dim SPTDT As New FileInfo(File.DirectoryName & "\" & Szunyi.IO.Files.Get_New_FileName.GetFileName_woExtension(File) & "_SignalPeptides.TDT")
                    Dim SPSeqs = Szunyi.Sequences.SequenceManipulation.SeqsToString.GetSeqsAsString(Me.SignalPeptides)
                    Szunyi.IO.Export.SaveText(Szunyi.Text.General.GetText(SPSeqs, vbCrLf), MPTDT)


                    Dim MPFile As New FileInfo(File.DirectoryName & "\" & Szunyi.IO.Files.Get_New_FileName.GetFileName_woExtension(File) & "_MaturePeptides.fa")
                    Szunyi.IO.Export.SaveSequencesToSingleFasta(Me.MaturePeptides, MPFile)
                    Dim FPFile As New FileInfo(File.DirectoryName & "\" & Szunyi.IO.Files.Get_New_FileName.GetFileName_woExtension(File) & "_FullPeptides.fa")
                    Szunyi.IO.Export.SaveSequencesToSingleFasta(Me.FullSeqs, FPFile)
                    Dim SPFIle As New FileInfo(File.DirectoryName & "\" & Szunyi.IO.Files.Get_New_FileName.GetFileName_woExtension(File) & "_SignalPeptides.fa")
                    Szunyi.IO.Export.SaveSequencesToSingleFasta(Me.SignalPeptides, SPFIle)
                End Sub
                ''' <summary>
                ''' Must have signal SIte
                ''' </summary>
                ''' <returns></returns>
                Public Function GetGenesWithSignalPeptide() As SignalPBuilder
                    Dim x As New SignalPBuilder
                    For i1 = 0 To Me.FullSeqs.Count - 1
                        If SignalPeptides(i1).Count > 1 Then
                            x.FullSeqs.Add(Me.Seqs(i1))
                            x.MaturePeptides.Add(Me.MaturePeptides(i1))
                            x.SignalPeptides.Add(Me.SignalPeptides(i1))
                        End If
                    Next
                    x.FullSeqs.Sort(Szunyi.Comparares.AllComparares.BySeqID)
                    x.MaturePeptides.Sort(Szunyi.Comparares.AllComparares.BySeqID)
                    x.SignalPeptides.Sort(Szunyi.Comparares.AllComparares.BySeqID)
                    Return x
                End Function
                Public Sub New()

                End Sub
                Public Function Filter(IDs As List(Of String)) As SignalPBuilder
                    Dim x As New SignalPBuilder
                    For i1 = 0 To Me.FullSeqs.Count - 1
                        If IDs.Contains(Me.FullSeqs(i1).ID) = True Then
                            x.FullSeqs.Add(Me.FullSeqs(i1))
                            x.MaturePeptides.Add(Me.MaturePeptides(i1))
                            x.SignalPeptides.Add(Me.SignalPeptides(i1))
                        End If
                    Next
                    Return x
                End Function
            End Class

            Public Class SignalPCleavageSite
                Public Property SeqID As String
                Public Property Position As Integer
                Public Property D_Value As Double
                ''' <summary>
                ''' If No Cleavage then position = -1
                ''' </summary>
                ''' <param name="SeqID"></param>
                ''' <param name="Position"></param>
                Public Sub New(SeqID As String, Position As String, DValue As Double)
                    Me.SeqID = SeqID
                    Me.Position = Position
                    Me.D_Value = DValue
                End Sub
                Public Sub New(SeqID As String, Position As Integer)
                    Me.SeqID = SeqID
                    Me.Position = Position

                End Sub
                Public Sub New(Line As String)
                    Dim s1() = Split(Line, vbTab)
                    Me.SeqID = s1(0)
                    Me.Position = s1(1)
                    Me.D_Value = s1(2)
                End Sub
                Public Function GetHeader() As String
                    Return "SeqID" & vbTab & "Position" & vbTab & "DValue"
                End Function
                Public Overrides Function ToString() As String
                    Return Me.SeqID & vbTab & Me.Position & vbTab & Me.D_Value
                End Function
            End Class
        End Namespace
    End Namespace
End Namespace

