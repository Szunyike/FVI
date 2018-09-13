Imports System.IO
Imports Bio.IO.GenBank
Imports ClassLibrary1.Szunyi
Imports ClassLibrary1.Szunyi.ListOf


Public Class FeatureAndLocationMerger
    Private SortedFeatureLists As List(Of ExtFeatureList)
    Private LocationLists As List(Of Szunyi.Location.LocationList)
    Private LocationListOfFeatures As New List(Of Szunyi.Location.LocationList)
    Public ReadOnly Property Type As String = Szunyi.Constants.BackGroundWork.MergeLocationsAndFeatures
    Private MergeAllFile As Boolean = False
    Private ToSave As FileInfo
    Private ToSaveFolder As DirectoryInfo
    Public Property SubType As String

    Public Property SubSubType As String

    Public Sub New(FeaturesLIsts As List(Of ExtFeatureList), LocationLists As List(Of Szunyi.Location.LocationList), SubType As String)
        Me.SortedFeatureLists = FeaturesLIsts
        Me.LocationLists = LocationLists
        For Each SortedFeature In Me.SortedFeatureLists
            Dim t As New Szunyi.Location.Import(SortedFeature, Szunyi.Constants.Location_Type.Features)
            ' t.DoIt()

            Me.LocationListOfFeatures.Add(t.result)
        Next

        Me.SubType = Type
        If Me.SortedFeatureLists.Count > 1 Or Me.LocationLists.Count > 1 Then
            Dim res = MsgBox("Merge All File to Single Output?", MsgBoxStyle.YesNo)
            Me.MergeAllFile = MsgBox("Merge All File to Single Output?", MsgBoxStyle.YesNo)
            If Me.MergeAllFile = True Then
                Me.ToSave = Szunyi.IO.Files.Save.SelectSaveFile(Szunyi.Constants.Files.Fasta)
            Else
                Me.ToSaveFolder = Szunyi.IO.Directory.Get_Folder
            End If
        End If
        DoIt()
    End Sub
    Public Sub DoIt()
        Dim Comp As New Szunyi.Location.BioLocation_Comparers.Contain_Full
        For Each i In Me.LocationListOfFeatures
            i.Locations.Sort(New Szunyi.Location.BioLocation_Comparers._ByStart)
        Next
        Dim Quals As New List(Of String)
        Quals.Add(StandardQualifierNames.LocusTag)
        Quals.Add(StandardQualifierNames.Product)

        For Each OriLoc In Me.LocationLists ' Files
            Dim NofFoundItem As Integer = 0

            For Each featlist In Me.SortedFeatureLists

            Next
            For Each Loci In OriLoc.Locations ' Location In Files
                For Each FeatList In Me.LocationListOfFeatures
                    Dim Index = FeatList.Locations.BinarySearch(Loci, Comp)
                    If Index > -1 Then

                        Dim Feat = FeatList.Locations(Index)
                        Try
                            Dim f As ExtFeature = Feat.Obj
                            Dim txt = Szunyi.Features.ExtFeatureManipulation.GetTextFromExtFeature(f.Feature, Quals, vbCrLf)
                            txt = txt.Replace(vbTab, "")
                            Dim s = Split(txt, vbCrLf)
                            Loci.Extra.Add(s.First)
                            Loci.Extra.Add(s(1))
                            NofFoundItem += 1
                        Catch ex As Exception

                        End Try

                    Else
                        Loci.Extra.Add("")
                        Loci.Extra.Add("")
                    End If

                Next

            Next
            Dim TheFile = Szunyi.IO.Files.Get_New_FileName.Replace_Extension(OriLoc.Files.First, "Ext")
            Dim str As New System.Text.StringBuilder

            For Each l In OriLoc.Locations
                str.Append(Szunyi.Text.General.GetText(l.Extra, vbTab)).AppendLine()
            Next
            Szunyi.IO.Export.SaveText(str.ToString, TheFile)
        Next

    End Sub

End Class