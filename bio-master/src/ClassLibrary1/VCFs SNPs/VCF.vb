Imports System.IO

Namespace Szunyi
    Namespace SNPs
        Public Class VCF_Manipulation
            Public Property Datas As New Dictionary(Of FileInfo, VCFs)
            Public Property Filtered_Datas As New Dictionary(Of FileInfo, VCFs)
            Public Property Files As List(Of FileInfo)
            Public Property Res As String
            Public Property IDAndRename As Dictionary(Of String, String)
            Public Sub New(Files As List(Of System.IO.FileInfo))
                Me.Files = Files
                For Each File In Files
                    Me.Datas.Add(File, New VCFs(File))
                Next
            End Sub
            Public Sub New(Files As List(Of System.IO.FileInfo), IDAndRename As Dictionary(Of String, String))
                Me.Files = Files
                Me.IDAndRename = IDAndRename
                For Each File In Files
                    Me.Datas.Add(File, New VCFs(File))
                Next
            End Sub

            Public Sub DoIt(MinRead As Integer, minFrequency As Double)
                Me.Filtered_Datas.Clear()
                For Each Item In Datas
                    Dim Vals = From x In Item.Value.VCFs Where x.Nof_Read >= MinRead And x.VariantFreguency >= minFrequency
                    If Vals.Count > 0 Then
                        Me.Filtered_Datas.Add(Item.Key, New VCFs(Vals.ToList))
                    End If
                Next

                Dim str As New System.Text.StringBuilder
                str.Append(vbTab)
                Dim dSNPs = Get_All_DIstinct_SNP()
                If IsNothing(Me.IDAndRename) = True Then
                    Dim Names = (From x In Me.Files Select x.Name).ToList
                    str.Append(Szunyi.Text.General.GetText(Names, vbTab)).AppendLine()
                Else
                    Dim Names = (From x In Me.IDAndRename Select x.Value).ToList
                    str.Append(Szunyi.Text.General.GetText(Names, vbTab)).AppendLine()
                End If
                Dim comp As New VCF_Comparer
                For Each SNP As VCF In dSNPs
                    str.Append(SNP.Pos & " " & SNP.ALT).Append(vbTab)
                    For Each item In Me.Filtered_Datas
                        Dim index = item.Value.VCFs.BinarySearch(SNP, comp)
                        If index > -1 Then
                            str.Append(item.Value.VCFs(index).VariantFreguency)
                            str.Append(vbTab)
                        Else
                            str.Append(vbTab)
                        End If

                    Next
                    str.AppendLine()
                Next
                Res = str.ToString
            End Sub
            Private Function Get_All_DIstinct_SNP() As List(Of VCF)


                Dim out As New List(Of VCF)
                For Each Item In Me.Filtered_Datas
                    out.AddRange(Item.Value.VCFs)
                Next
                Dim gr = From x In out Group By x.Pos, x.ALT Into Group
                Dim out2 As New List(Of VCF)
                Dim sGR = From x In gr Order By x.Pos Ascending
                For Each f In sGR
                    out2.Add(f.Group.First)
                Next
                Return out2
            End Function
        End Class
        Public Class VCFs
            Public Property VCFs As New List(Of VCF)
            Public Sub New(File As System.IO.FileInfo)
                For Each line In Szunyi.IO.Import.Text.Parse(File, "#")
                    Me.VCFs.Add(New VCF(line))
                Next
            End Sub
            Public Sub New(VCFs As List(Of VCF))
                Me.VCFs = VCFs
            End Sub
        End Class
        Public Class VCF
            Public Property SeqID As String
            Public Property Pos As Integer
            Public Property REF As String
            Public Property ALT As String
            Public Property QUAL As Double
            Public Property Filter As String
            Public Property VariantFreguency As Double
            Public Property Type As String
            Public Property Nof_Read As Integer
            Public Property Nof_Ref As Integer
            Public Property Nof_Alt
            Public Sub New(Line As String)
                Dim s = Split(Line, vbTab)
                Me.SeqID = s(0)
                Me.POs = s(1)
                Me.REF = s(3)
                Me.ALT = s(4)
                Me.QUAL = s(5)
                Me.Filter = s(6)
                Dim s1 = Split(s(7), ";")
                Dim IsVF = (From X In s1 Where X.Contains("VF"))
                Dim IsDP4 = (From X In s1 Where X.Contains("DP4"))
                Dim IsI16 = (From X In s1 Where X.Contains("I16"))
                If IsVF.Count > 0 Then
                    Me.VariantFreguency = (From X In s1 Where X.Contains("VF")).First.Split("=").Last
                ElseIf IsDP4.Count > 0 Then
                    Dim s2 = (From X In s1 Where X.Contains("DP4")).First.Split("=").Last.Split(",")
                ElseIf IsI16.Count > 0 Then
                    Dim s2 = (From X In s1 Where X.Contains("I16")).First.Split("=").Last.Split(",")
                    Me.Nof_Read = CInt(s2(0)) + CInt(s2(1)) + CInt(s2(2)) + CInt(s2(3))
                    Me.Nof_Ref = CInt(s2(0)) + CInt(s2(1))
                    Me.Nof_Alt = CInt(s2(2)) + CInt(s2(3))
                    Me.VariantFreguency = Me.Nof_Alt / Me.Nof_Read
                    Dim alf As Int16 = 43
                End If


            End Sub
        End Class
        Public Class VCF_Comparer
            Implements IComparer(Of VCF)

            Public Function Compare(x As VCF, y As VCF) As Integer Implements IComparer(Of VCF).Compare
                If x.SeqID <> y.SeqID Then Return x.SeqID.CompareTo(y.SeqID)
                If x.Pos <> y.Pos Then Return x.Pos.CompareTo(y.Pos)

                Return x.ALT.CompareTo(y.ALT)

            End Function
        End Class
    End Namespace
End Namespace

