Imports Bio.IO.GenBank
Namespace Szunyi
	Public Class Comparares
		Public Class StringComparer
			Implements IComparer(Of String())

			Public Function Compare(ByVal x() As String, ByVal y() As String) As Integer Implements IComparer(Of String()).Compare

				Return x.First.CompareTo(y.First)

			End Function

		End Class
		Public Class ExtFeatureLocationComparer
			Implements IComparer(Of ExtFeature)

			Public Function Compare(x As ExtFeature, y As ExtFeature) As Integer Implements IComparer(Of ExtFeature).Compare
				If x.SeqID = y.SeqID Then
					Return x.Feature.Location.LocationStart.CompareTo(y.Feature.Location.LocationStart)
				Else
					Return x.SeqID.CompareTo(y.SeqID)
				End If
			End Function

		End Class
		Public Class ExtFeatureLocusTagComparer
			Implements IComparer(Of ExtFeature)

			Public Function Compare(x As ExtFeature, y As ExtFeature) As Integer Implements IComparer(Of ExtFeature).Compare
				Return x.LocusTag.CompareTo(y.LocusTag)

			End Function

		End Class

		Public Class LocationComparer
			Implements IComparer(Of Location)

			Public Function Compare(x As Location, y As Location) As Integer Implements IComparer(Of Location).Compare
				If x.SeqID = y.SeqID Then
					Return x.Start.CompareTo(y.Start)
				Else
					Return x.SeqID.CompareTo(y.SeqID)
				End If
			End Function

		End Class

		Public Class SequenceIDComparer
			Implements IComparer(Of Bio.ISequence)

			Public Function Compare(x As Bio.ISequence, y As Bio.ISequence) As Integer Implements IComparer(Of Bio.ISequence).Compare
				Return x.ID.CompareTo(y.ID)
			End Function
		End Class
	End Class
End Namespace



