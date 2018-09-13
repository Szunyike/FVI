Imports System.IO
Imports System.Linq
Imports System.Numerics
Imports RDotNet
Imports ClassLibrary1.Szunyi.Location
Imports ClassLibrary1.Szunyi.Constants

Namespace Szunyi.Stat
    Public Enum Distributions
        Poisson = 1
        PolyaAeppli = 2
        Mixed_Poisson_PolyaAeppli = 3

    End Enum

    Interface IDistribution
        ReadOnly Property Name As String
        ReadOnly Property mean As Double
        ReadOnly Property File As FileInfo ' For merging in Later pass
        ReadOnly Property Hundrends As List(Of Integer)
        Property p As Double ' This Can be Set From OutSide for Fast Calculation
        ReadOnly Property width As Integer
        ReadOnly Property Count As Integer
        ReadOnly Property Index As Integer
        ReadOnly Property IsComplementer As Boolean
        ReadOnly Property Passed As Boolean
        ReadOnly Property Is_Local_Maximum As Boolean
        ReadOnly Property Distribution As List(Of Integer)
        ReadOnly Property Variance As Double
        ReadOnly Property SD As Double
        ReadOnly Property lambda As Double
        ReadOnly Property rho As Double

    End Interface

    Public Class PolyaAeppli
        Implements IDistribution
        Dim Name As String = "Polya-Aeppli"
        Dim mean As Double
        Dim File As FileInfo ' Later For Merging
        Dim Hundrends As List(Of Integer)
        Property p As Double
#Region "Properties"
        Public ReadOnly Property IDistribution_Name As String Implements IDistribution.Name
            Get
                Return Name
            End Get
        End Property

        Public ReadOnly Property IDistribution_mean As Double Implements IDistribution.mean
            Get
                Return mean
            End Get
        End Property

        Public ReadOnly Property IDistribution_File As FileInfo Implements IDistribution.File
            Get
                Return File
            End Get
        End Property

        Public ReadOnly Property IDistribution_Hundrends As List(Of Integer) Implements IDistribution.Hundrends
            Get
                Return Hundrends
            End Get
        End Property

        Public Property IDistribution_p As Double Implements IDistribution.p
            Get
                Return p
            End Get
            Set(value As Double)
                p = value
            End Set
        End Property

        Public ReadOnly Property IDistribution_width As Integer Implements IDistribution.width
            Get
                Return width
            End Get
        End Property

        Public ReadOnly Property IDistribution_Count As Integer Implements IDistribution.Count
            Get
                Return Count
            End Get
        End Property

        Public ReadOnly Property IDistribution_Index As Integer Implements IDistribution.Index
            Get
                Return Index
            End Get
        End Property

        Public ReadOnly Property IDistribution_IsComplementer As Boolean Implements IDistribution.IsComplementer
            Get
                Return IsComplementer
            End Get
        End Property

        Public ReadOnly Property IDistribution_Passed As Boolean Implements IDistribution.Passed
            Get
                Return Me.Passed
            End Get
        End Property

        Public ReadOnly Property IDistribution_Is_Local_Maximum As Boolean Implements IDistribution.Is_Local_Maximum
            Get
                Return Is_Local_Maximum
            End Get
        End Property

        Public ReadOnly Property IDistribution_Distribution As List(Of Integer) Implements IDistribution.Distribution
            Get
                Return Distribution
            End Get
        End Property

        Public ReadOnly Property IDistribution_Variance As Double Implements IDistribution.Variance
            Get
                Return Variance
            End Get
        End Property

        Public ReadOnly Property IDistribution_SD As Double Implements IDistribution.SD
            Get
                Return SD
            End Get
        End Property

        Public ReadOnly Property IDistribution_lambda As Double Implements IDistribution.lambda
            Get
                Return lambda
            End Get
        End Property

        Public ReadOnly Property IDistribution_rho As Double Implements IDistribution.rho
            Get
                Return rho
            End Get
        End Property
#End Region
        Dim width As Integer
        Dim Count As Integer
        Dim Index As Integer
        Dim IsComplementer As Boolean
        Dim Passed As Boolean
        Dim Is_Local_Maximum As Boolean
        Dim Distribution As List(Of Integer)
        Dim Variance As Double
        Dim lambda As Double
        Dim rho As Double
        Dim SD As Double


        Public Sub New(site As Sites, width As Integer, Index As Integer, isComplementer As Boolean, Value As Integer, FIle As FileInfo, REngine As REngine, localWidht As Integer)
            Me.width = width
            Me.Count = Value
            Me.Index = Index
            Me.File = FIle
            Me.IsComplementer = isComplementer
            Hundrends = site.Get_Distribution(Index, isComplementer, site.sort)
            mean = Hundrends.Sum / Hundrends.Count

            Variance = Accord.Statistics.Measures.Variance(Hundrends.ToArray)
            SD = Accord.Statistics.Measures.StandardDeviation(Hundrends.ToArray, mean)
            lambda = 2 * (mean ^ 2) / (Variance + mean)
            rho = (Variance - mean) / (Variance + mean)

            REngine.Evaluate("library(polyaAeppli)")
            Try
                p = 1 - REngine.Evaluate(R).AsNumeric.First
            Catch ex As Exception
                p = -1
            End Try



        End Sub

        Private Function R() As String
            Dim str As New System.Text.StringBuilder
            str.Append("dPolyaAeppli(").Append(Me.Count).Append(",").Append(Me.lambda).Append(",").Append(Me.rho).Append(",log = FALSE)")
            Return str.ToString
        End Function
    End Class

    Public Class Poisson
        Implements IDistribution
        Dim Name As String = "Poisson"
        Dim mean As Double
        Dim File As FileInfo ' Later For Merging
        Dim Hundrends As List(Of Integer)
        Property p As Double
#Region "Properties"


        Public ReadOnly Property IDistribution_Name As String Implements IDistribution.Name
            Get
                Return Name
            End Get
        End Property

        Public ReadOnly Property IDistribution_mean As Double Implements IDistribution.mean
            Get
                Return mean
            End Get
        End Property

        Public ReadOnly Property IDistribution_File As FileInfo Implements IDistribution.File
            Get
                Return File
            End Get
        End Property

        Public ReadOnly Property IDistribution_Hundrends As List(Of Integer) Implements IDistribution.Hundrends
            Get
                Return Hundrends
            End Get
        End Property

        Public Property IDistribution_p As Double Implements IDistribution.p
            Get
                Return p
            End Get
            Set(value As Double)
                p = value
            End Set
        End Property

        Public ReadOnly Property IDistribution_width As Integer Implements IDistribution.width
            Get
                Return width
            End Get
        End Property

        Public ReadOnly Property IDistribution_Count As Integer Implements IDistribution.Count
            Get
                Return Count
            End Get
        End Property

        Public ReadOnly Property IDistribution_Index As Integer Implements IDistribution.Index
            Get
                Return Index
            End Get
        End Property

        Public ReadOnly Property IDistribution_IsComplementer As Boolean Implements IDistribution.IsComplementer
            Get
                Return IsComplementer
            End Get
        End Property

        Public ReadOnly Property IDistribution_Passed As Boolean Implements IDistribution.Passed
            Get
                Return Me.Passed
            End Get
        End Property

        Public ReadOnly Property IDistribution_Is_Local_Maximum As Boolean Implements IDistribution.Is_Local_Maximum
            Get
                Return Is_Local_Maximum
            End Get
        End Property

        Public ReadOnly Property IDistribution_Distribution As List(Of Integer) Implements IDistribution.Distribution
            Get
                Return Distribution
            End Get
        End Property

        Public ReadOnly Property IDistribution_Variance As Double Implements IDistribution.Variance
            Get
                Return Variance
            End Get
        End Property

        Public ReadOnly Property IDistribution_SD As Double Implements IDistribution.SD
            Get
                Return SD
            End Get
        End Property

        Public ReadOnly Property IDistribution_lambda As Double Implements IDistribution.lambda
            Get
                Return lambda
            End Get
        End Property

        Public ReadOnly Property IDistribution_rho As Double Implements IDistribution.rho
            Get
                Return rho
            End Get
        End Property
#End Region
        Dim width As Integer
        Dim Count As Integer
        Dim Index As Integer
        Dim IsComplementer As Boolean
        Public Passed As Boolean
        Public Is_Local_Maximum As Boolean
        Dim Distribution As List(Of Integer)
        Dim Variance As Double
        Dim lambda As Double
        Dim rho As Double
        Dim SD As Double


        Public Sub New(site As Sites, width As Integer, Index As Integer, isComplementer As Boolean, Value As Integer, FIle As FileInfo, REngine As REngine)
            Me.width = width
            Me.Count = Value
            Me.Index = Index
            Me.File = FIle
            Me.IsComplementer = isComplementer
            Hundrends = site.Get_Distribution(Index, isComplementer, site.sort)
            mean = Hundrends.Sum / Hundrends.Count
            If mean <> 0 Then
                Dim poi As New Accord.Statistics.Distributions.Univariate.PoissonDistribution(mean)
                p = 1 - poi.DistributionFunction(Value, True)
            Else
                p = -1
            End If

            Variance = Accord.Statistics.Measures.Variance(Hundrends.ToArray)
            SD = Accord.Statistics.Measures.StandardDeviation(Hundrends.ToArray, mean)


        End Sub

        Private Function R() As String
            Dim str As New System.Text.StringBuilder
            str.Append("dPolyaAeppli(").Append(Me.Count).Append(",").Append(Me.lambda).Append(",").Append(Me.rho).Append(",log = FALSE)")
            Return str.ToString
        End Function
    End Class

    Public Class Mixed_Poisson_PolyaAppeli

        Implements IDistribution
        Dim Name As String = "Mixed_Poisson_PolyaAppeli"
        Dim mean As Double
        Dim File As FileInfo ' Later For Merging
        Dim Hundrends As List(Of Integer)
        Property p As Double
#Region "Properties"
        Public ReadOnly Property IDistribution_Name As String Implements IDistribution.Name
            Get
                Return Name
            End Get
        End Property

        Public ReadOnly Property IDistribution_mean As Double Implements IDistribution.mean
            Get
                Return mean
            End Get
        End Property

        Public ReadOnly Property IDistribution_File As FileInfo Implements IDistribution.File
            Get
                Return File
            End Get
        End Property

        Public ReadOnly Property IDistribution_Hundrends As List(Of Integer) Implements IDistribution.Hundrends
            Get
                Return Hundrends
            End Get
        End Property

        Public Property IDistribution_p As Double Implements IDistribution.p
            Get
                Return p
            End Get
            Set(value As Double)
                p = value
            End Set
        End Property

        Public ReadOnly Property IDistribution_width As Integer Implements IDistribution.width
            Get
                Return width
            End Get
        End Property

        Public ReadOnly Property IDistribution_Count As Integer Implements IDistribution.Count
            Get
                Return Count
            End Get
        End Property

        Public ReadOnly Property IDistribution_Index As Integer Implements IDistribution.Index
            Get
                Return Index
            End Get
        End Property

        Public ReadOnly Property IDistribution_IsComplementer As Boolean Implements IDistribution.IsComplementer
            Get
                Return IsComplementer
            End Get
        End Property

        Public ReadOnly Property IDistribution_Passed As Boolean Implements IDistribution.Passed
            Get
                Return Me.Passed
            End Get
        End Property

        Public ReadOnly Property IDistribution_Is_Local_Maximum As Boolean Implements IDistribution.Is_Local_Maximum
            Get
                Return Is_Local_Maximum
            End Get
        End Property

        Public ReadOnly Property IDistribution_Distribution As List(Of Integer) Implements IDistribution.Distribution
            Get
                Return Distribution
            End Get
        End Property

        Public ReadOnly Property IDistribution_Variance As Double Implements IDistribution.Variance
            Get
                Return Variance
            End Get
        End Property

        Public ReadOnly Property IDistribution_SD As Double Implements IDistribution.SD
            Get
                Return SD
            End Get
        End Property

        Public ReadOnly Property IDistribution_lambda As Double Implements IDistribution.lambda
            Get
                Return lambda
            End Get
        End Property

        Public ReadOnly Property IDistribution_rho As Double Implements IDistribution.rho
            Get
                Return rho
            End Get
        End Property

#End Region


        Dim width As Integer
        Dim Count As Integer
        Dim Index As Integer
        Dim IsComplementer As Boolean
        Public Passed As Boolean
        Public Is_Local_Maximum As Boolean
        Dim Distribution As List(Of Integer)
        Dim Variance As Double
        Dim lambda As Double
        Dim rho As Double
        Dim SD As Double


        Public Sub New(site As Sites, width As Integer, Index As Integer, isComplementer As Boolean, Value As Integer, FIle As FileInfo, REngine As REngine)
            Me.width = width
            Me.Count = Value
            Me.Index = Index
            Me.File = FIle
            Me.IsComplementer = isComplementer
            Hundrends = site.Get_Distribution(Index, isComplementer, site.sort)
            mean = Hundrends.Sum / Hundrends.Count
            Variance = Accord.Statistics.Measures.Variance(Hundrends.ToArray)
            SD = Accord.Statistics.Measures.StandardDeviation(Hundrends.ToArray, mean)
            If mean >= Variance Then ' Poisson
                If mean <> 0 Then
                    Dim poi As New Accord.Statistics.Distributions.Univariate.PoissonDistribution(mean)
                    p = 1 - poi.DistributionFunction(Value, True)
                Else
                    p = -1
                End If
            Else
                lambda = 2 * (mean ^ 2) / (Variance + mean)
                rho = (Variance - mean) / (Variance + mean)

                REngine.Evaluate("library(polyaAeppli)")
                p = 1 - REngine.Evaluate(R).AsNumeric.First

            End If

        End Sub

        Private Function R() As String
            Dim str As New System.Text.StringBuilder
            str.Append("dPolyaAeppli(").Append(Me.Count).Append(",").Append(Me.lambda).Append(",").Append(Me.rho).Append(",log = FALSE)")
            Return str.ToString
        End Function
    End Class

    Public Class Manager
        Public Property Site As Szunyi.Location.Sites
        Public Property Local_width As Integer
        Public Property WindowSize As Integer
        Public Property seq As Bio.ISequence
        Public Property Type As String
        Public Property File As System.IO.FileInfo
        Public Property Enums As New List(Of Integer)
        Public Property P_Threshold As Double
        Public Property Sort As Sort_Locations_By
        Public Property alpha As Double
        Public Property Interesting_Locations As New List(Of Basic_Location)

        Public Property Result As List(Of Distribution_Result)
        Public Sub New(Site As Szunyi.Location.Sites)
            Me.Site = Site
        End Sub
        Public Function Get_Header() As String
            Dim str As New System.Text.StringBuilder
            str.Append(Szunyi.BAM.Bam_Basic_IO.Headers.Get_Comments(File)).AppendLine()
            str.Append("# Local Width:").Append(Local_width).AppendLine()
            str.Append("# Width:").Append(WindowSize).AppendLine()
            str.Append("# P-Threshold:").Append(P_Threshold).AppendLine()
            If IsNothing(File) = False Then str.Append("#File:").Append(File.FullName).AppendLine()
            str.Append("#Type:").Append(Me.Type).AppendLine()
            Dim h = Split("p-value calculation type,mean,sd,variance,lambda,rho,Position,Strand,Count,Hundred,p-value,Is Local Maximum,Passed", ",")
            str.Append(Szunyi.Text.General.GetText(h, vbTab)).Append(vbTab)
            str.Append(Get_Distribution_Header(Me.Local_width))
            Return str.ToString
        End Function

        Public Function Add_Features(Seq As Bio.ISequence) As List(Of Bio.IO.GenBank.FeatureItem)
            Dim Key = Me.Type
            Dim Count As Integer = 0
            Dim Feats As New List(Of Bio.IO.GenBank.FeatureItem)
            If Me.Type = "PAS" Then
                For Each Item In Me.Result
                    '  Dim Key = Item.Passed.ToString.Substring(0) & Item.Is_Local_Maximum.ToString.Substring(0)
                    If Item.Is_Local_Maximum = True Then
                        Dim Loci As Bio.IO.GenBank.ILocation
                        If Item.IsComplementer = True Then
                            Loci = Szunyi.Location.Common.GetLocation(Item.Index + 1, Item.IsComplementer)

                        Else
                            Loci = Szunyi.Location.Common.GetLocation(Item.Index - 1, Item.IsComplementer)
                        End If


                        Dim Feat As New Bio.IO.GenBank.FeatureItem(Key, loci)
                        Feat.Label = Item.Count
                        Feats.Add(Feat)

                        Szunyi.Features.FeatureManipulation.GenBankMetaDataManipulation.AddFeature(Seq, Feat)
                        Count += 1
                    End If
                Next
            Else ' TSS
                For Each Item In Me.Result

                    If Item.Is_Local_Maximum = True AndAlso Item.Passed = True Then
                        Dim loci = Szunyi.Location.Common.GetLocation(Item.Index, Item.IsComplementer)

                        Dim Feat As New Bio.IO.GenBank.FeatureItem(Key, loci)
                        Feat.Label = Item.Count
                        Feats.Add(Feat)

                        Szunyi.Features.FeatureManipulation.GenBankMetaDataManipulation.AddFeature(Seq, Feat)
                        Count += 1
                    End If
                Next
            End If
            Return Feats


        End Function
        Public Sub New(Local_width As Integer,
                                             WindowSize As Integer,
                                             Sort As Sort_Locations_By,
                                             seq As Bio.ISequence,
                                             Type As String,
                                             BLs As List(Of Basic_Location),
                                             Enums As List(Of Integer),
                                             P_Threshold As Double)
            Me.Local_width = Local_width
            Me.Sort = Sort
            Me.seq = seq
            Me.Type = Type
            Me.Sort = Sort

            Me.Enums = Enums
            Me.P_Threshold = P_Threshold
            Me.WindowSize = WindowSize

            Dim Merged = Szunyi.Location.Merging.MergeLocations(BLs, Local_width, Sort, 0) ' As List(Of List(Of Szunyi.Location.Basic_Location))
            Dim For_Test As New List(Of Basic_Location)
            For Each M In Merged
                If M.Count > 1 Then
                    Interesting_Locations.Add(Szunyi.Location.Basic_Location_Manipulation.Get_Most_Abundants(M, Sort).First)
                End If
            Next
            Me.Site = New Szunyi.Location.Sites(seq, BLs, Sort) ' .PacBio.Pacbio_Transcript_Shared.Get_Sites(For_Test, seq, Sort)
            Me.Site.Local_Width = Me.Local_width
            Me.Site.Width = Me.WindowSize
            Me.Site.p_Threshold = Me.P_Threshold
        End Sub
        Public Sub New(Local_width As Integer,
                                             WindowSize As Integer,
                                             Sort As Sort_Locations_By,
                                             seq As Bio.ISequence,
                                             Type As String,
                                             BLs As List(Of Basic_Location),
                                             The_Enum As Integer,
                                             P_Threshold As Double)
            Me.Local_width = Local_width
            Me.Sort = Sort
            Me.seq = seq
            Me.Type = Type
            Me.Sort = Sort

            Me.Enums.Add(The_Enum)
            Me.P_Threshold = P_Threshold
            Me.WindowSize = WindowSize

            Dim Merged = Szunyi.Location.Merging.MergeLocations(BLs, Local_width, Sort, 0) ' As List(Of List(Of Szunyi.Location.Basic_Location))

            Dim For_Test As New List(Of Basic_Location)
            For Each M In Merged
                If M.Count > 1 Then

                    Interesting_Locations.Add(M.First)



                End If
            Next

            Me.Site = New Szunyi.Location.Sites(seq, BLs, Sort) ' .PacBio.Pacbio_Transcript_Shared.Get_Sites(For_Test, seq, Sort)
            Me.Site.Local_Width = Me.Local_width
            Me.Site.Width = Me.WindowSize
            Me.Site.p_Threshold = Me.P_Threshold
        End Sub
        Private Sub Set_Interesting_Locationsq()

        End Sub
        Public Sub New(Local_width As Integer,
                                             WindowSize As Integer,
                                             Sort As Sort_Locations_By,
                                             seq As Bio.ISequence,
                                             Type As String,
                                             File As System.IO.FileInfo,
                                             Enums As List(Of Integer),
                                             P_Threshold As Double)
            Me.Local_width = Local_width
            Me.sort = Sort
            Me.seq = seq
            Me.Type = Type
            Me.Sort = Sort
            Me.File = File
            Me.Enums = Enums
            Me.P_Threshold = P_Threshold
            Me.WindowSize = WindowSize
            Dim SAMs = Szunyi.BAM.Bam_Basic_IO.Import.ParseAll(File)
            Dim Locis = Szunyi.Location.Common.GetLocation(SAMs)
            Dim BL_LOcis = Szunyi.Location.Basic_Location_Manipulation.Get_Basic_LocationS(Locis)
            Dim Merged = Szunyi.Location.Merging.MergeLocations(BL_LOcis, Local_width, Sort, 1) ' As List(Of List(Of Szunyi.Location.Basic_Location))
            Dim For_Test As New List(Of Basic_Location)
            For Each M In Merged
                If M.Count > 1 Then
                    Interesting_Locations.Add(Szunyi.Location.Basic_Location_Manipulation.Get_Most_Abundants(M, Sort).First)
                End If
            Next
            Me.Site = New Szunyi.Location.Sites(seq, BL_LOcis, Sort) ' .PacBio.Pacbio_Transcript_Shared.Get_Sites(For_Test, seq, Sort)
            Me.Site.Local_Width = Me.Local_width
            Me.Site.Width = Me.WindowSize
            Me.Site.p_Threshold = Me.P_Threshold
        End Sub
        Public Sub New(Local_width As Integer,
                                             WindowSize As Integer,
                                             Sort As Sort_Locations_By,
                                             seq As Bio.ISequence,
                                             Type As String,
                                             Sams As List(Of Bio.IO.SAM.SAMAlignedSequence),
                                             Enums As List(Of Integer),
                                             P_Threshold As Double)
            Me.Local_width = Local_width
            Me.Sort = Sort
            Me.seq = seq
            Me.Type = Type
            Me.Sort = Sort
            Me.File = File
            Me.Enums = Enums
            Me.P_Threshold = P_Threshold
            Me.WindowSize = WindowSize

            Dim Locis = Szunyi.Location.Common.GetLocation(SAMs)
            Dim BL_LOcis = Szunyi.Location.Basic_Location_Manipulation.Get_Basic_LocationS(Locis)
            Dim Merged = Szunyi.Location.Merging.MergeLocations(BL_LOcis, Local_width, Sort, 0) ' As List(Of List(Of Szunyi.Location.Basic_Location))
            Dim For_Test As New List(Of Basic_Location)
            For Each M In Merged
                If M.Count > 1 Then
                    Interesting_Locations.Add(Szunyi.Location.Basic_Location_Manipulation.Get_Most_Abundants(M, Sort).First)
                End If
            Next
            Me.Site = New Szunyi.Location.Sites(seq, BL_LOcis, Sort) ' .PacBio.Pacbio_Transcript_Shared.Get_Sites(For_Test, seq, Sort)
            Me.Site.Local_Width = Me.Local_width
            Me.Site.Width = Me.WindowSize
            Me.Site.p_Threshold = Me.P_Threshold
        End Sub
        Private Function Get_Count(Feat As Basic_Location, Index As Integer) As Integer
            If Feat.Location.IsComplementer = True Then
                Return Me.Site.Neg(Index)
            Else
                Return Me.Site.Pos(Index)
            End If
        End Function
        Public Function Calculate() As List(Of Distribution_Result)
            Dim LocalMaxPos = Site.Get_Local_Maximums(False)
            Dim LocalMaxNeg = Site.Get_Local_Maximums(True)

            Dim Alpha = P_Threshold / (Me.WindowSize * 2 + 1) / (LocalMaxPos.Count + LocalMaxNeg.Count)
            Dim out As New List(Of Distribution_Result)
            '        Dim REngine = RDotNet.REngine.GetInstance
            '      REngine.Initialize()
            Dim jj = From x In Me.Interesting_Locations Where x.Location.TSS = 4181
            For Each E In Me.Enums
                For Each feat In Me.Interesting_Locations
                    Dim Index = Szunyi.Location.Common.Get_Value(feat.Location, Me.Site.sort)
                    Dim Count = Get_Count(feat, Index)
                    If Count > 0 Then
                        Dim x1 As New Szunyi.Stat.Distribution_Result(Me.Site, Me.WindowSize, Me.Local_width, Index, feat.IsComplement, Count, File, Nothing, E)

                        x1.Is_Local_Maximum = Me.Site.Is_Local_Maximum(Index, Me.WindowSize, feat.Location.IsComplementer)

                        If x1.p_value < Alpha Then
                            x1.Passed = True
                        Else
                            x1.Passed = False
                        End If
                        out.Add(x1)
                        x1.Distribution = Me.Site.Get_Distribution(feat, Me.Sort)

                    End If
                Next
            Next
            Me.Result = out
            Return out
        End Function
        Public Function Get_Text()
            'p-value calculation type,sd,variance,lambda,rho,Position,Strand,Count,Hundred,p-value,Is Local Maximum,Passed", ",")
            Dim str As New System.Text.StringBuilder
            For Each DR In Me.Result
                str.AppendLine()
                str.Append(DR.Name).Append(vbTab)
                str.Append(DR.mean).Append(vbTab)
                str.Append(DR.SD).Append(vbTab)
                str.Append(DR.Variance).Append(vbTab)
                str.Append(DR.lambda).Append(vbTab)
                str.Append(DR.rho).Append(vbTab)
                str.Append(DR.Index).Append(vbTab)
                str.Append(Szunyi.Location.Common.Get_Strand(DR.IsComplementer)).Append(vbTab)
                str.Append(DR.Count).Append(vbTab)
                str.Append(DR.Hundreds.Sum).Append(vbTab)
                str.Append(DR.p_value).Append(vbTab)
                str.Append(DR.Is_Local_Maximum).Append(vbTab)
                str.Append(DR.Passed).Append(vbTab)
                str.Append(Szunyi.Text.General.GetText(DR.Distribution, vbTab))
            Next
            Return str.ToString

        End Function
        Public Function Calculate(Feats As List(Of Basic_Location), LocalWidth As Integer, Width As Integer, File As FileInfo, E As Integer, Optional P_Threshold As Double = 0.05) As List(Of Distribution_Result)
            Dim LocalMaxPos = Site.Get_Local_Maximums(False)
            Dim LocalMaxNeg = Site.Get_Local_Maximums(True)

            Dim Alpha = P_Threshold / (Width * 2 + 1) / (LocalMaxPos.Count + LocalMaxNeg.Count)
            Dim out As New List(Of Distribution_Result)
            Dim REngine = RDotNet.REngine.GetInstance
            REngine.Initialize()

            For Each feat In Feats
                Dim Index = Szunyi.Location.Common.Get_Value(feat.Location, Me.Site.sort)
                Dim Count = Get_Count(feat, Index)
                Dim x1 As New Szunyi.Stat.Distribution_Result(Me.Site, Width, LocalWidth, Index, feat.IsComplement, Count, File, REngine, E)

                x1.Is_Local_Maximum = Me.Site.Is_Local_Maximum(Index, LocalWidth, feat.Location.IsComplementer)

                If x1.p_value < Alpha Then
                    x1.Passed = True
                Else
                    x1.Passed = False
                End If
                out.Add(x1)
            Next


            Return out
        End Function
        Public Shared Function Get_Distribution_Header(Local_Width As Integer) As String
            Dim str As New System.Text.StringBuilder
            For i1 = -Local_Width To Local_Width
                str.Append(i1).Append(vbTab)
            Next
            If str.Length > 0 Then str.Length -= 1
            Return str.ToString
        End Function
        Public Shared Function Get_Text(DRs As List(Of Distribution_Result)) As String
            Dim str As New System.Text.StringBuilder
            Dim h = Split("p-value calculation type,lambda,Variance,SD,Position,Strand,Count,Hundred,p-value,Is Local Maximum,Passed", ",")
            str.Append(Szunyi.Text.General.GetText(h, vbTab)).Append(vbTab)
            str.Append(Get_Distribution_Header(DRs.First.LocalWidth))
            For Each DR In DRs
                str.AppendLine()
                str.Append(DR.Name).Append(vbTab)
                str.Append(DR.mean).Append(vbTab)
                str.Append(DR.lambda).Append(vbTab)
                str.Append(DR.Variance).Append(vbTab)
                str.Append(DR.SD).Append(vbTab)
                str.Append(DR.rho).Append(vbTab)
                str.Append(DR.Index).Append(vbTab)
                str.Append(Szunyi.Location.Common.Get_Strand(DR.IsComplementer)).Append(vbTab)
                str.Append(DR.Count).Append(vbTab)
                str.Append(DR.Hundreds.Sum).Append(vbTab)
                str.Append(DR.p_value).Append(vbTab)
                str.Append(DR.Is_Local_Maximum).Append(vbTab)
                str.Append(DR.Passed).Append(vbTab)
                str.Append(Szunyi.Text.General.GetText(DR.Distribution, vbTab))
            Next
            Return str.ToString
        End Function

        Public Sub Save(File As FileInfo, Seq As Bio.ISequence)
            Dim t = Me.Get_Header & Me.Get_Text

            Dim nFIle = Szunyi.IO.Files.Get_New_FileName.Append_Before_Extension_wNew_Extension(File, ".tsv")

            Szunyi.IO.Export.SaveText(t, nFIle)

            Dim t1 = Me.Site.Get_All_Count
            Dim nFIle2 = Szunyi.IO.Files.Get_New_FileName.Append_Before_Extension_wNew_Extension(File, "_Counts.tsv")

            Szunyi.IO.Export.SaveText(t1, nFIle2)


        End Sub
    End Class
    Public Class Distribution_Result
        Public Property File As FileInfo
        Public Property Width As Integer
        Public Property LocalWidth As Integer
        Public Property Count As Integer
        Public Property Index As Integer
        Public Property IsComplementer As Boolean
        Public Property Passed As Boolean
        Public Property Is_Local_Maximum As Boolean
        Public Property Distribution As List(Of Integer)
        Public Property Variance As Double
        Public Property lambda As Double
        Public Property rho As Double
        Public Property SD As Double
        Public Property p_value As Double
        Public Property Name As String
        Public Property Hundreds As List(Of Integer)
        Public Property mean As Double


        Public Sub New(site As Sites, width As Integer, LocalWidth As Integer, Index As Integer, isComplementer As Boolean, Value As Integer, FIle As FileInfo, REngine As REngine, E As Szunyi.Stat.Distributions)
            Me.Width = width
            Me.LocalWidth = LocalWidth
            Me.Count = Value
            Me.Index = Index
            Me.File = FIle
            Me.IsComplementer = isComplementer
            Hundreds = site.Get_Distributions(Index, isComplementer, site.sort, Me.Width)

            mean = Hundreds.Sum / Hundreds.Count
            '        Hundreds = site.Get_Distributions_woIndex(Index, isComplementer, site.sort, Me.Width)

            Variance = Accord.Statistics.Measures.Variance(Hundreds.ToArray)
            SD = Accord.Statistics.Measures.StandardDeviation(Hundreds.ToArray, mean)
            Me.Name = Szunyi.Util_Helpers.Get_Enum_Name(Of Szunyi.Stat.Distributions)(E)

            Select Case E
                Case Szunyi.Stat.Distributions.Poisson
                    Dim poi As New Accord.Statistics.Distributions.Univariate.PoissonDistribution(mean)
                    p_value = 1 - poi.DistributionFunction(Value, True)
                Case Szunyi.Stat.Distributions.PolyaAeppli
                    lambda = 2 * (mean ^ 2) / (Variance + mean)
                    rho = (Variance - mean) / (Variance + mean)

                    REngine.Evaluate("library(polyaAeppli)")
                    If rho > 0 Then
                        p_value = 1 - REngine.Evaluate(R).AsNumeric.First
                        ' p_value = REngine.Evaluate(R).AsNumeric.First
                    Else
                        p_value = 1
                    End If
                Case Szunyi.Stat.Distributions.Mixed_Poisson_PolyaAeppli
                    If mean >= Variance Then ' Poisson

                        Dim poi As New Accord.Statistics.Distributions.Univariate.PoissonDistribution(mean)
                        p_value = 1 - poi.DistributionFunction(Value, True)

                    Else
                        lambda = 2 * (mean ^ 2) / (Variance + mean)
                        rho = (Variance - mean) / (Variance + mean)

                        REngine.Evaluate("library(polyaAeppli)")
                        p_value = 1 - REngine.Evaluate(R).AsNumeric.First
                        '        p_value = REngine.Evaluate(R).AsNumeric.First
                    End If
                Case Szunyi.Stat.Distributions.PolyaAeppli
                    lambda = 2 * (mean ^ 2) / (Variance + mean)
                    rho = (Variance - mean) / (Variance + mean)

                    REngine.Evaluate("library(polyaAeppli)")
                    If rho > 0 Then
                        p_value = 1 - REngine.Evaluate(R).AsNumeric.First
                        ' p_value = REngine.Evaluate(R).AsNumeric.First
                    Else
                        p_value = 1
                    End If
                Case Else
                    Dim kj As Int16 = 43
            End Select


        End Sub
        Private Function R() As String
            Dim str As New System.Text.StringBuilder
            str.Append("pPolyaAeppli(").Append(Me.Count).Append(",").Append(Me.lambda).Append(",").Append(Me.rho).Append(",log = FALSE)")
            Return str.ToString
        End Function
    End Class
End Namespace
