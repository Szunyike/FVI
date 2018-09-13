Imports System.IO

Namespace Szunyi
    Namespace Alignment
        Namespace BAM_SAM
            Public Enum Into
                Fasta = 1
                Bam = 2
                Sam = 3
                FastQ = 4
            End Enum
            Public Enum OrganismsFrom
                Additioanl_File = 1
                From_Bam_Files = 2
            End Enum

            Public Class Bam_Split
                Public Property Into As BAM_SAM.Into
                Public Property OrganismFrom As BAM_SAM.OrganismsFrom
                Public Property Bam_Files As List(Of FileInfo)
                Public Property Organism_Names As New List(Of String)
                Public Sub New(Into As BAM_SAM.Into, OrganismFrom As BAM_SAM.OrganismsFrom, Bam_Files As List(Of FileInfo))
                    Me.Into = Into
                    Me.OrganismFrom = OrganismFrom
                    Me.Bam_Files = Bam_Files

                End Sub
            End Class
        End Namespace
    End Namespace
End Namespace

