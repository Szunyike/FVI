Imports ClassLibrary1.Szunyi.Constants
Namespace Szunyi.Location.Finding
    Public Interface IFind
        ' Property TSS_or_LS As List(Of Szunyi.Location.Basic_Location)
        ' Property PAS_or_LE As List(Of Szunyi.Location.Basic_Location)
        Property wStrand As Boolean
        Property wIntron As Boolean
        Function Get_BLs(loci As Basic_Location, width As Integer) As List(Of Basic_Location)
        Function Get_BLs(loci As Basic_Location, TSS_5 As Integer, TSS_3 As Integer, PAS_5 As Integer, PAS_3 As Integer) As List(Of Basic_Location)
        Function Get_BLs(locis As List(Of Basic_Location), width As Integer) As List(Of List(Of Basic_Location))
        Property Comps As BLs_Binary_Comparers

    End Interface
    Public Class TSS_PAS
        Implements Szunyi.Location.Finding.IFind
        Public Property By_TSS_LS As New List(Of Basic_Location)
        Public Property By_PAS_LE As New List(Of Basic_Location)
        Private _wStrand As Boolean
        Private _wIntron As Boolean
        Public Sub New(Locis As List(Of Basic_Location), wsStrand As Boolean, wIntron As Boolean)
            Me.comps = New BLs_Binary_Comparers
            Me.wStrand = wsStrand
            Me.wIntron = wIntron
            By_TSS_LS.AddRange(Locis)
            By_PAS_LE.AddRange(Locis)
            If Me.wStrand = True Then
                By_TSS_LS.Sort(comps.TSS_wStrand)
                By_PAS_LE.Sort(comps.PAS_wStrand)
            Else
                By_TSS_LS.Sort(comps.TSS_woStrand)
                By_PAS_LE.Sort(comps.PAS_woStrand)
            End If

        End Sub
        Public Property wStrand As Boolean Implements IFind.wStrand
            Get
                Return _wStrand
            End Get
            Set(value As Boolean)
                _wStrand = value
            End Set
        End Property

        Public Property wIntron As Boolean Implements IFind.wIntron
            Get
                Return _wIntron
            End Get
            Set(value As Boolean)
                _wIntron = value
            End Set
        End Property

        Public Property comps As BLs_Binary_Comparers Implements IFind.Comps
            Get
                Return New BLs_Binary_Comparers
            End Get
            Set(value As BLs_Binary_Comparers)

            End Set
        End Property

        Public Function Get_BLs(loci As Basic_Location, width As Integer) As List(Of Basic_Location) Implements IFind.Get_BLs
            Dim Locis_Near_TSS = Get_Locis_Near_TSS(loci, width, width)
            Dim Locis_Near_PAS = Get_Locis_Near_PAS(loci, width, width)
            Dim Common = Locis_Near_PAS.Intersect(Locis_Near_TSS)
            If Common.Count = 0 Then
                Return New List(Of Basic_Location)
            Else
                Return Common.ToList
            End If

        End Function
        Public Function Get_BLs(loci As Basic_Location, TSS_5 As Integer, TSS_3 As Integer, PAS_5 As Integer, PAS_3 As Integer) As List(Of Basic_Location) Implements IFind.Get_BLs
            Dim Locis_Near_TSS = Get_Locis_Near_TSS(loci, TSS_5, TSS_3)
            ' TEST
            Dim Res = From x In Locis_Near_TSS Where x.Location.TSS < loci.Location.TSS - TSS_5 Or x.Location.TSS > loci.Location.TSS + TSS_5
            If Res.Count > 0 Then
                Dim kj As Int16 = 54
            End If


            Dim Locis_Near_PAS = Get_Locis_Near_PAS(loci, PAS_5, PAS_3)
            Dim Resii = From x In Locis_Near_PAS Where x.Location.PAS < loci.Location.PAS - TSS_5 Or x.Location.PAS > loci.Location.PAS + TSS_5
            If Resii.Count > 0 Then
                Dim kj As Int16 = 54
            End If
            Dim Common = Locis_Near_PAS.Intersect(Locis_Near_TSS)
            If Common.Count = 0 Then
                Return New List(Of Basic_Location)
            Else
                Return Common.ToList
            End If

        End Function
        Private Function Get_Locis_Near_PAS(loci As Basic_Location, PAS_5 As Integer, PAS_3 As Integer) As List(Of Basic_Location)
            Dim F_Loci = Szunyi.Location.Common.Get_Basic_Location(loci.Location, -PAS_5, Sort_Locations_By.PAS)
            Dim First_Index As Integer
            Dim Index As Integer
            If Me.wStrand = True Then
                Index = Me.By_PAS_LE.BinarySearch(F_Loci, comps.PAS_wStrand)
            Else
                Index = Me.By_PAS_LE.BinarySearch(F_Loci, comps.PAS_woStrand)
            End If

            If Index < -1 Then
                First_Index = Not (Index) 'index of the first element that is larger than value or size of array
                If First_Index = Me.By_PAS_LE.Count Then
                    Return New List(Of Basic_Location) ' 
                Else

                End If
            Else
                First_Index = Index
                For i1 = Index To 0 Step -1
                    If Me.By_PAS_LE(i1).Location.PAS = loci.Location.PAS Then
                        First_Index = i1
                    Else
                        Exit For
                    End If
                Next
            End If
            Dim L_Loci = Szunyi.Location.Common.Get_Basic_Location(loci.Location, PAS_3, Sort_Locations_By.PAS)
            Dim Last_Index As Integer

            If Me.wStrand = True Then
                Index = Me.By_PAS_LE.BinarySearch(L_Loci, comps.PAS_wStrand)
            Else
                Index = Me.By_PAS_LE.BinarySearch(L_Loci, comps.PAS_woStrand)
            End If
            If Index <= -1 Then
                Last_Index = Not (Index)
                Last_Index -= 1 'index of the first element that is larger than value or size of array

            Else
                Last_Index = Index
                For i1 = Index + 1 To Me.By_TSS_LS.Count - 1
                    If Me.By_PAS_LE(i1).Location.PAS <= L_Loci.Location.PAS Then
                        Last_Index = i1
                    Else
                        Exit For
                    End If
                Next
            End If
            If Last_Index >= First_Index Then
                Return Me.By_PAS_LE.GetRange(First_Index, Last_Index - First_Index)
            End If
            Return New List(Of Basic_Location)
        End Function

        Private Function Get_Locis_Near_TSS(loci As Basic_Location, TSS_5 As Integer, TSS_3 As Integer) As List(Of Basic_Location)
            Dim F_Loci = Szunyi.Location.Common.Get_Basic_Location(loci.Location, -TSS_5, Sort_Locations_By.TSS)
            Dim First_Index As Integer
            Dim Index As Integer
            If Me.wStrand = True Then
                Index = Me.By_TSS_LS.BinarySearch(F_Loci, comps.TSS_wStrand)
            Else
                Index = Me.By_TSS_LS.BinarySearch(F_Loci, comps.TSS_woStrand)
            End If

            If Index < 0 Then
                First_Index = Not (Index) 'index of the first element that is larger than value or size of array
                If First_Index = Me.By_TSS_LS.Count Then
                    Return New List(Of Basic_Location) ' 
                End If
            Else
                First_Index = Index
                For i1 = Index To 0 Step -1
                    If Me.By_TSS_LS(i1).Location.TSS = loci.Location.TSS Then
                        First_Index = i1
                    Else
                        Exit For
                    End If
                Next
            End If
            Dim L_Loci = Szunyi.Location.Common.Get_Basic_Location(loci.Location, TSS_3, Sort_Locations_By.TSS)
            Dim Last_Index As Integer

            If Me.wStrand = True Then
                Index = Me.By_TSS_LS.BinarySearch(L_Loci, comps.TSS_wStrand)
            Else
                Index = Me.By_TSS_LS.BinarySearch(L_Loci, comps.TSS_woStrand)
            End If
            If Index <= -1 Then
                Last_Index = Not (Index)
                Last_Index -= 1 'index of the first element that is larger than value or size of array

            Else
                For i1 = Index + 1 To Me.By_TSS_LS.Count - 1
                    If Me.By_TSS_LS(i1).Location.TSS <= L_Loci.Location.TSS Then
                        Last_Index = i1
                    Else
                        Exit For
                    End If
                Next
            End If
            If Last_Index >= First_Index Then
                If First_Index > -1 And Last_Index > -1 Then
                    Return Me.By_TSS_LS.GetRange(First_Index, Last_Index - First_Index)
                Else
                    Dim kj As Int16 = -43
                End If
            End If
            Return New List(Of Basic_Location)
        End Function

        Public Function Get_BLs(locis As List(Of Basic_Location), width As Integer) As List(Of List(Of Basic_Location)) Implements IFind.Get_BLs
            Throw New NotImplementedException()
        End Function
    End Class
    Public Class BLs_Binary_Comparers
        Public Property TSS_wStrand As New _ByTSS_wStrand
        Public Property TSS_woStrand As New _ByTSS_woStrand
        Public Property PAS_wStrand As New _ByPAS_wStrand
        Public Property PAS_woStrand As New _ByPAS_woStrand
        Public Property LS_wStrand As New _ByStart_wStrand
        Public Property LS_woStrand As New _ByStart_woStrand
        Public Property LE_wStrand As New _ByEnd_wStrand
        Public Property LE_woStrand As New _ByEnd_woStrand
        ''' <summary.location.>
        ''' Sort/Find By.location. SeqID,OPerator and After By.location. StartPosition From Location
        ''' </summary.location.>
        Public Class _ByStart_wStrand
            Implements IComparer(Of Basic_Location)

            Public Function Compare(x As Basic_Location, y As Basic_Location) As Integer Implements IComparer(Of Basic_Location).Compare
                If x.SeqID <> y.SeqID Then Return x.SeqID.CompareTo(y.SeqID)
                If x.Location.Operator <> y.Location.Operator Then Return x.Location.Operator.CompareTo(y.Location.Operator)
                Return x.Location.LocationStart.CompareTo(y.Location.LocationStart)
            End Function


        End Class
        ''' <summary.location.>
        ''' Sort/Find By.location. Seq ID and After By.location. StartPosition From Location
        ''' </summary.location.>
        Public Class _ByEnd_wStrand
            Implements IComparer(Of Basic_Location)

            Public Function Compare(x As Basic_Location, y As Basic_Location) As Integer Implements IComparer(Of Basic_Location).Compare
                If x.SeqID <> y.SeqID Then Return x.SeqID.CompareTo(y.SeqID)
                If x.Location.Operator <> y.Location.Operator Then Return x.Location.Operator.CompareTo(y.Location.Operator)
                Return x.Location.LocationEnd.CompareTo(y.Location.LocationEnd)
            End Function

        End Class

        ''' <summary.location.>
        ''' Sort/Find By.location. Seq ID and After By.location.TSS From Location
        ''' </summary.location.>
        Public Class _ByTSS_wStrand
            Implements IComparer(Of Basic_Location)

            Public Function Compare(x As Basic_Location, y As Basic_Location) As Integer Implements IComparer(Of Basic_Location).Compare
                If x.SeqID <> y.SeqID Then Return x.SeqID.CompareTo(y.SeqID)
                If x.Location.IsComplementer <> y.Location.IsComplementer Then Return x.Location.IsComplementer.CompareTo(y.Location.IsComplementer)
                Return x.Location.TSS.CompareTo(y.Location.TSS)
            End Function

        End Class
        ''' <summary.location.>
        ''' Sort/Find By.location. Seq ID ,strandand After By.location.PAS From Location
        ''' </summary.location.>
        Public Class _ByPAS_wStrand
            Implements IComparer(Of Basic_Location)

            Public Function Compare(x As Basic_Location, y As Basic_Location) As Integer Implements IComparer(Of Basic_Location).Compare
                If x.SeqID <> y.SeqID Then Return x.SeqID.CompareTo(y.SeqID)
                If x.Location.Operator <> y.Location.Operator Then Return x.Location.Operator.CompareTo(y.Location.Operator)
                Return x.Location.PAS.CompareTo(y.Location.PAS)
            End Function

        End Class

        ''' <summary.location.>
        ''' Sort/Find By.location. SeqID, After By.location. StartPosition From Location
        ''' </summary.location.>
        Public Class _ByStart_woStrand
            Implements IComparer(Of Basic_Location)

            Public Function Compare(x As Basic_Location, y As Basic_Location) As Integer Implements IComparer(Of Basic_Location).Compare
                If x.SeqID <> y.SeqID Then Return x.SeqID.CompareTo(y.SeqID)
                Return x.Location.LocationStart.CompareTo(y.Location.LocationStart)
            End Function


        End Class
        ''' <summary.location.>
        ''' Sort/Find By.location. Seq ID and After By.location. StartPosition From Location
        ''' </summary.location.>
        Public Class _ByEnd_woStrand
            Implements IComparer(Of Basic_Location)

            Public Function Compare(x As Basic_Location, y As Basic_Location) As Integer Implements IComparer(Of Basic_Location).Compare
                If x.SeqID <> y.SeqID Then Return x.SeqID.CompareTo(y.SeqID)
                Return x.Location.LocationEnd.CompareTo(y.Location.LocationEnd)
            End Function

        End Class

        ''' <summary.location.>
        ''' Sort/Find By.location. Seq ID and After By.location.TSS From Location
        ''' </summary.location.>
        Public Class _ByTSS_woStrand
            Implements IComparer(Of Basic_Location)

            Public Function Compare(x As Basic_Location, y As Basic_Location) As Integer Implements IComparer(Of Basic_Location).Compare
                If x.SeqID <> y.SeqID Then Return x.SeqID.CompareTo(y.SeqID)
                Return x.Location.TSS.CompareTo(y.Location.TSS)
            End Function

        End Class
        ''' <summary.location.>
        ''' Sort/Find By.location. Seq ID ,strandand After By.location.PAS From Location
        ''' </summary.location.>
        Public Class _ByPAS_woStrand
            Implements IComparer(Of Basic_Location)

            Public Function Compare(x As Basic_Location, y As Basic_Location) As Integer Implements IComparer(Of Basic_Location).Compare
                If x.SeqID <> y.SeqID Then Return x.SeqID.CompareTo(y.SeqID)
                Return x.Location.PAS.CompareTo(y.Location.PAS)
            End Function

        End Class
    End Class
End Namespace
Namespace Szunyi.Location

    Public Class Basic_Location_Finder
        Public Property By_TSS As New List(Of Basic_Location)
        Public Property By_PAS As New List(Of Basic_Location)
        Public Property By_Location_Start As New List(Of Basic_Location)
        Public Property By_Location_End As New List(Of Basic_Location)

        Dim comps As New LociBinary
        Public Property wStrand As Boolean = True

        Public Sub New(Locis As List(Of Basic_Location), wStrand As Boolean)
            Me.wStrand = wStrand
            Me.By_Location_Start.AddRange(Locis)
            Me.By_Location_Start.Sort(Get_Comp(Sort_Locations_By.LS))
            Me.By_Location_End.AddRange(Locis)
            Me.By_Location_End.Sort(Get_Comp(Sort_Locations_By.LE))
            Me.By_TSS.AddRange(Locis)
            Me.By_TSS.Sort(Get_Comp(Sort_Locations_By.TSS))
            Me.By_PAS.AddRange(Locis)
            Me.By_PAS.Sort(Get_Comp(Sort_Locations_By.PAS))
        End Sub
        Private Function Get_Comp(Type As Sort_Locations_By)
            Select Case Type
                Case Sort_Locations_By.LE
                    If Me.wStrand = True Then
                        Return LociBinary.cBasic_Location_ByEnd_wStrand
                    Else
                        Return LociBinary.cBasic_Location_ByEnd_woStrand
                    End If
                Case Sort_Locations_By.LS
                    If Me.wStrand = True Then
                        Return LociBinary.cBasic_Location_ByStart_wStrand
                    Else
                        Return LociBinary.cBasic_Location_ByStart_woStrand
                    End If
                Case Sort_Locations_By.PAS
                    If Me.wStrand = True Then
                        Return LociBinary.cBasic_Location_ByPAS_wStrand
                    Else
                        Return LociBinary.cBasic_Location_ByPAS_woStrand
                    End If
                Case Sort_Locations_By.TSS
                    If Me.wStrand = True Then
                        Return LociBinary.cBasic_Location_ByTSS_wStrand
                    Else
                        Return LociBinary.cBasic_Location_ByTSS_woStrand
                    End If
            End Select

        End Function
        Private Function Get_Item(Index As Integer, type As Sort_Locations_By) As Basic_Location
            Select Case type
                Case Sort_Locations_By.TSS
                    Return Me.By_TSS(Index)
                Case Sort_Locations_By.PAS
                    Return Me.By_PAS(Index)
                Case Sort_Locations_By.LS
                    Return Me.By_Location_Start(Index)
                Case Sort_Locations_By.LE
                    Return Me.By_Location_End(Index)
            End Select
        End Function
        Private Function Get_Index(loci As Basic_Location, type As Sort_Locations_By, comp As Object) As Integer
            Select Case type
                Case Sort_Locations_By.TSS
                    Return Me.By_TSS.BinarySearch(loci, comp)
                Case Sort_Locations_By.PAS
                    Return Me.By_PAS.BinarySearch(loci, comp)
                Case Sort_Locations_By.LS
                    Return Me.By_Location_Start.BinarySearch(loci, comp)
                Case Sort_Locations_By.LE
                    Return Me.By_Location_End.BinarySearch(loci, comp)
                Case Else
                    Dim kj As Int16 = 54
            End Select

        End Function
        Private Function Get_First_Index(Index As Integer, Type As Sort_Locations_By, loci As Basic_Location) As Integer
            Dim First_Index As Integer = Index
            For i1 = Index To 0 Step -1
                Select Case Type
                    Case Sort_Locations_By.TSS
                        If Me.By_TSS(i1).Location.TSS = loci.Location.TSS Then
                            First_Index = i1
                        Else Exit For
                        End If
                    Case Sort_Locations_By.PAS
                        If Me.By_PAS(i1).Location.PAS = loci.Location.PAS Then
                            First_Index = i1
                        Else Exit For
                        End If
                    Case Sort_Locations_By.LS
                        If Me.By_Location_Start(i1).Location.LocationStart = loci.Location.LocationStart Then
                            First_Index = i1
                        Else Exit For
                        End If
                    Case Sort_Locations_By.LE
                        If Me.By_Location_End(i1).Location.LocationEnd = loci.Location.LocationEnd Then
                            First_Index = i1
                        Else Exit For
                        End If
                    Case Else
                        Dim kj As Int16 = 54
                End Select
            Next
            Return First_Index
        End Function
        Private Function Get_Last_Index(Index As Integer, Type As Sort_Locations_By, loci As Basic_Location) As Integer
            Dim First_Index As Integer = Index
            For i1 = Index To Me.By_TSS.Count - 1
                Select Case Type
                    Case Sort_Locations_By.TSS
                        If Me.By_TSS(i1).Location.TSS = loci.Location.TSS Then
                            First_Index = i1
                        Else Exit For
                        End If
                    Case Sort_Locations_By.PAS
                        If Me.By_PAS(i1).Location.PAS = loci.Location.PAS Then
                            First_Index = i1
                        Else Exit For
                        End If
                    Case Sort_Locations_By.LS
                        If Me.By_Location_Start(i1).Location.LocationStart = loci.Location.LocationStart Then
                            First_Index = i1
                        Else Exit For
                        End If
                    Case Sort_Locations_By.LE
                        If Me.By_Location_End(i1).Location.LocationEnd = loci.Location.LocationEnd Then
                            First_Index = i1
                        Else Exit For
                        End If
                    Case Else
                        Dim kj As Int16 = 54
                End Select
            Next
            Return First_Index
        End Function
        ''' <summary>
        ''' width 0 means exact match, width is always positive
        ''' </summary>
        ''' <param name="loci"></param>
        ''' <param name="Type"></param>
        ''' <returns></returns>
        ''' 
        Private Function Get_First_Index_equal_or_larger(loci As Basic_Location, type As Sort_Locations_By) As Integer

            Dim comp = Get_Comp(type)
            Dim Index = Get_Index(loci, type, comp)
            If Index < 0 Then
                Return Not (Index) 'index of the first element that is larger than value or size of array
            Else
                Return Get_First_Index(Index, type, loci)
            End If



        End Function
        Private Function Get_Last_Index_equeal_or_smaller(loci As Basic_Location, Type As Sort_Locations_By) As Integer


            Dim comp = Get_Comp(Type)
            Dim Index = Get_Index(loci, Type, comp)
            If Index < 0 Then
                Return (Not (Index)) - 1
            Else
                Return Get_Last_Index(Index, Type, loci)
            End If

        End Function
        Public Function Find_Items_byLoci(loci As Basic_Location, Width As Integer, Type As Sort_Locations_By) As List(Of Basic_Location)
            Dim Current As New List(Of Basic_Location)
            Dim F_Loci = Szunyi.Location.Common.Get_Basic_Location(loci.Location.TSS - Width, loci.Location.PAS - Width, loci.Location.Operator)
            Dim L_Loci = Szunyi.Location.Common.Get_Basic_Location(loci.Location.TSS + Width, loci.Location.PAS + Width, loci.Location.Operator)

            Dim F_Index = Get_First_Index_equal_or_larger(F_Loci, Type)
            Dim L_Index = Get_Last_Index_equeal_or_smaller(L_Loci, Type)
            Dim l = Get_List(Type)
            For i1 = F_Index To L_Index
                Current.Add(l(i1))
            Next
            Return Current
        End Function
        Public Function Find_Index_byLoci(loci As Basic_Location, Width As Integer, Type As Sort_Locations_By, Optional IsComplementer As Object = Nothing) As List(Of Integer)
            Dim Current As New List(Of Integer)
            Dim F_Loci = Szunyi.Location.Common.Get_Basic_Location(loci.Location, -Width, Type)
            Dim L_Loci = Szunyi.Location.Common.Get_Basic_Location(loci.Location, Width, Type)

            Dim F_Index = Get_First_Index_equal_or_larger(F_Loci, Type)
            Dim L_Index = Get_Last_Index_equeal_or_smaller(L_Loci, Type)
            Dim l = Get_List(Type)
            If IsNothing(IsComplementer) = True Then
                For i1 = F_Index To L_Index
                    Current.Add(i1)
                Next
            Else
                For i1 = F_Index To L_Index
                    Dim Item = Get_Item(i1, Type)
                    If Item.Location.IsComplementer = IsComplementer Then
                        Current.Add(i1)
                    End If
                Next

            End If

            Return Current
        End Function
        Private Function Get_List(Type As Sort_Locations_By) As List(Of Basic_Location)
            Select Case Type
                Case Sort_Locations_By.LS
                    Return Me.By_Location_Start
                Case Sort_Locations_By.LE
                    Return Me.By_Location_End
                Case Sort_Locations_By.TSS
                    Return Me.By_TSS
                Case Sort_Locations_By.PAS
                    Return Me.By_PAS
            End Select
            Return Nothing
        End Function

    End Class


End Namespace
