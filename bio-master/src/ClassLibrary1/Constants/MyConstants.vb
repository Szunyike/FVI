Imports Bio.IO.GenBank
Namespace Szunyi
    Namespace Constants
        Public Enum SequencingType
            Illumina = 1
            PacBio = 2
            PacBio_SMRT = 3
            MinIOn = 4
            IonTorrent = 5
            Solid = 6
            Basic = 7
        End Enum
        Public Class Chart
            Public Enum Pie_Chart_Type
                Percent = 1
                True_Or_False = 2
                Avarage = 3
                Quantile = 4
            End Enum
        End Class
        Public Enum TextMatch
            Exact = 1
            Exact_SmallAndCapitalIsSame = 2
            Contains = 3
            Contains_SmallAndCapitalIsSame = 4
            StartWith = 5
            StartWith_SmallAndCapitalIsSame = 6
        End Enum
        Public Enum SOrting
            NoSorting = 1
            Sorting = 2
        End Enum
        Public Enum Primer_Design
            RT_PCR = 1
            Variant_From_Table = 2
        End Enum
        Public Enum Variants
            SNV = 1
            MNV = 2
            Insertion = 3
            Deletion = 4
            Replacement = 5
            Heterozygous = 6
            Homozygous = 7

        End Enum

        Public Enum Operators
            Less_Than = 0
            Less_than_Or_Equal = 1
            Greater_Than = 2
            Greater_Than_Or_Equal = 3
            Equal = 4
            Not_Equal = 5
            Between = 6
        End Enum
        Public Class File_Extensions
            Public Const Log = ".log"
            Public Const Xml = ".xml"
            Public Const PoreCHop = ".porechop"
        End Class
        Public Class Files
            Public Const csv = "csv (*.csv)|*.csv"
            Public Const PoreCHop = "PoreChop|*.porechop"
            Public Const SAM_BAM_Fastq As String = "Sam/Bam/fastq (*.sam,*bam.*.fastq)|*.sam;*.bam.*.fastq|Sam (*.sam)|*.sam|Bam (*.bam)|*.bam|fastq (*.fastq)|*.fastq"
            Public Const Loc_SAM_BAM As String = "GBK LocationFiles or SAM/BAM|*.loc;*.sam;*.bam"
            Public Const Location As String = "GBK LocationFiles|*.loc"
            Public Const rep As String = "Picard Report Files|*.rep"
            Public Const gpg As String = "EGA Files|*.gpg"
            Public Const vcf As String = "Vcf Files|*.vcf"
            Public Const BED As String = "Bed Files|*.bed"
            Public Const SelectFolder As String = "Select Folder"
            Public Const Log = "log Files|*.log"
            Public Const Xml = "xml Files|*.xml"
            Public Const All_TAB_Like = "All Tab-Like Files|*.tdt;*.tab;*.txt;*.tsv;*.csv"
            Public Const Fast5 As String = "Fast5 (*.fast5)|*.fast5"
            Public Const Fasta As String = "Fasta (*.fa,*.fas,*.fasta.*.fna)|*.fa;*.fas;*.fasta;*.fna"
            Public Const GenBank As String = "GenBank (*.gbk,*.gb)|*.gb;*.gbk"
            Public Const gff3 As String = "gff3 (*.gff)|*.gff3"
            Public Const SAM As String = "Sam (*.sam)|*.sam"
            Public Const BAM As String = "Bam (*.bam)|*.bam"
            Public Const SAM_BAM As String = "Sam/Bam (*.sam,*bam)|*.sam;*.bam|Sam (*.sam)|*.sam|Bam (*.bam)|*.bam"
            Public Const HDF5 As String = "HDF5 (*.h5)|*.h5"
            Public Const Gzip As String = "Gzip Files(*.gz) |*.gz"
            Public Const Fasta_FastQ As String = "Fasta and Fastq (*.fa,*.fas,*.fasta,*.fast1,*.fq)|*.fa;*.fas;*.fasta;*.fq;*.fastq"
            Public Const Fasta_FastQ_gz As String = "Fasta, Fastq and gz (*.fa,*.fas,*.fasta,*.fast1,*.fq,*.gz)|*.fa;*.fas;*.fasta;*.fq;*.fastq,*.gz|Fasta and Fastq (*.fa,*.fas,*.fasta,*.fast1,*.fq)|*.fa;*.fas;*.fasta;*.fq;*.fastq|Gzip Files(*.gz) |*.gz"
            Public Const FastQ As String = "fastq (*.fq,*.fastq)|*.fq;*.fastq"
            Public Const Mapping As String = "Mappings(*.mapping)|*.mappings"
            Public Const SequenceFileTypesToImport As String = "All Sequence File|*.fa;*.fas;*.fasta;*gb;*.gbk;*.fq;*.fastQ|" &
                    "Fasta (*.fa,*.fas,*.fasta) Multiple|*.fa;*.fas;*.fasta|" &
            "Fasta (*.fa,*.fas) Multiple|*.fa;*.fas|" &
            "FastQ (*.fq,*.fastq) Multiple|*.fq;*.fastq|" &
            "GenBank Single (*.gbk,*.gb)|*.gb;*.gbk|" &
            "GenBank Multiple (*.gbk,*.gb)|*.gb;*.gbk"
            Public Const SequenceFileTypeToSave As String = "Fasta (*.fa,*.fas,*.fasta) Multiple|*.fa;*.fas;*.fasta|" &
            "Fasta (*.fa,*.fas) Multiple|*.fa;*.fas|" &
             "FastQ (*.fastq) Multiple|*.fastq|" &
            "GenBank Single (*.gbk,*.gb)|*.gb;*.gbk|" &
            "GenBank Multiple (*.gbk,*.gb)|*.gb;*.gbk"
            Public Const Settings = "Settings Files|*.set"
            Public Const PhylogeneticXml = "Nexml Files|*.nexml"
            Public Const SignalP = ".SignalP"

            Public Class Other
                Public Const XmlForFilter = "Xml Files|*.xml"
                Public Const htm = "htm Files|*.htm"
                Public Const csv = "csv Files|*.csv"
                Public Const tdt = "tab delimited text Files|*.tdt"
                Public Const Blat = "Blat Result File|*.pslx"
                Public Const BED = "BED Files|*.bed"

                Public Const SAM As String = "Sam (*.sam)|*.sam"
                Public Const BAM As String = "Bam (*.bam)|*.bam"
                Public Const VCF As String = "VCF (*.vcf)|*.vcf"
            End Class
        End Class
        Public Class Protein
            Public Class Hidrophobicity_Indexes
                Public Const Aboderin As String = "Aboderin"
                Public Const AbrahamLeo As String = "AbrahamLeo"
                Public Const AggreScan As String = "AggreScan"
                Public Const Argos As String = "Argos"
                Public Const BlackMould As String = "BlackMould"
                Public Const BullBreese As String = "BullBreese"
                Public Const Casari As String = "Casari"
                Public Const Chothia As String = "Chothia"
                Public Const Cid As String = "Cid"
                Public Const Cowan3_4 As String = "Cowan3_4"
                Public Const Cowan7_5 As String = "Cowan7_5"
                Public Const Eisenberg As String = "Eisenberg"
                Public Const Engelman As String = "Engelman"
                Public Const Fasman As String = "Fasman"
                Public Const Fauchere As String = "Fauchere"
                Public Const Goldsack As String = "Goldsack"
                Public Const Guy As String = "Guy"
                Public Const HoppWoods As String = "HoppWoods"
                Public Const Janin As String = "Janin"
                Public Const Jones As String = "Jones"
                Public Const Juretic As String = "Juretic"
                Public Const Kidera As String = "Kidera"
                Public Const Kuhn As String = "Kuhn"
                Public Const KyteDoolittle As String = "KyteDoolittle"
                Public Const Levitt As String = "Levitt"
                Public Const Manavalan As String = "Manavalan"
                Public Const Miyazawa As String = "Miyazawa"
                Public Const Parker As String = "Parker"
                Public Const Ponnuswamy As String = "Ponnuswamy"
                Public Const Prabhakaran As String = "Prabhakaran"
                Public Const Rao As String = "Rao"
                Public Const Rose As String = "Rose"
                Public Const Roseman As String = "Roseman"
                Public Const Sweet As String = "Sweet"
                Public Const Tanford As String = "Tanford"
                Public Const Welling As String = "Welling"
                Public Const Wilson As String = "Wilson"
                Public Const Wolfenden As String = "Wolfenden"
                Public Const Zimmerman As String = "Zimmerman"

                Public Const Russel_Linding As String = "Russel/Linding"
                Public Const Hopp_Woods As String = "Hopp-Woods"
                Public Const Kyte_Doolittle As String = "Kyte-Doolittle"
                Public Const Remark465 As String = "Remark465"
                Public Const Deleage_Roux As String = "Deleage/Roux"
                Public Const Bfactor_2STD As String = "Bfactor (2STD)"


            End Class
        End Class
        Public Class Sequence_Analysis
            Public Property All As New List(Of String)
            Public Const Disordered_by_Loops_coils_definition = "Disordered by Loops/coils definition"
            Public Const Disordered_by_Hot_loops_definition = "Disordered by Hot-loops definition"
            Public Const Disordered_by_Remark_465_definition = "Disordered by Remark-465 definition"
            Public Const Disordered_by_Russell_Linding_definition = "Disordered by Russell/Linding definition"
            Public Const Disordered_by_AggreScan = "Disordered by AggreScan"
            Public Sub New()
                All.Add(Disordered_by_AggreScan)
                All.Add(Disordered_by_Russell_Linding_definition)
                All.Add(Disordered_by_Remark_465_definition)
                All.Add(Disordered_by_Hot_loops_definition)
                All.Add(Disordered_by_Loops_coils_definition)
            End Sub
        End Class
        Public Enum Orientation
            A = 1
            T = 2
            Missing = 3
            Confused = 4
        End Enum
        Public Enum ReadType_ByPolyA
            A = 1
            T = 2
            M = 3
            bAT = 4
            polyAT = 5

        End Enum
        Public Enum Read_Type_BySMRT_Secret_Sequence
            Five = 0
            Three = 1
            Both = 3
            None = 4
        End Enum
        Public Enum Get_Position_From_LocationsBy
            minvalue = 1
            maxvalue = 2
            abundance = 3
            weight = 4
        End Enum
        Public Enum Sort_Locations_By
            TSS = 16
            PAS = 17
            LS = 18
            LE = 19
            TSS_PAS = 20
            LS_LE = 21

        End Enum
        Public Enum StringRename
            AscendingWithPrefix = 1
            FirstAfterSplit = 2
            LastAfterSplit = 3
            Not_Last_Part = 4
            Nor_First_Part = 5
            Ascending_With_PostFix = 6
            Insert_Before = 7
        End Enum
        Public Enum LocusTag
            Full = 1
            Shorty = 2
            Pure = 3
        End Enum
        Public Class Blast
            Public Const CreateDatabase As String = "CreateDatabase"
            Public Const RunBlast As String = "RunBlast"
        End Class
        Public Class Features
            Public Shared Property ncRNA_Qulaifiers As String() = Split("antisense_RNA;autocatalytically_spliced_intron;ribozyme;hammerhead_ribozyme;lncRNA;RNase_P_RNA;RNase_MRP_RNA;telomerase_RNA;guide_RNA;rasiRNA;scRNA;siRNA;miRNA;piRNA;snoRNA;snRNA;SRP_RNA;vault_RNA;Y_RNA;other", ";")
            Public Shared Property CDSs_mRNAs_Genes As List(Of String) = Split(StandardFeatureKeys.CodingSequence & ":" &
                StandardFeatureKeys.Gene & ":" & StandardFeatureKeys.MessengerRna, ":").ToList
            Public Shared Property Get_Description_Gene_Note As String() = Split(StandardQualifierNames.GeneSymbol & vbTab & StandardQualifierNames.Note, vbTab)
        End Class
        Public Enum Strand_Type
            Same_Strand = 1
            Different_Strand = 2
            Both_Strand = 3
            Positive_strand = 4
            Negative_strand = 5
            Unknown_strand = 6
        End Enum
        Public Class Strand
            Public Shared Property Same_Strand As String = "Same_Strand"
            Public Shared Property DIff_Strand As String = "Different_Strand"
            Public Shared Property Both_Strand As String = "Both_Strand"

        End Class
        Public Class Other_Database
            Public Class KEGG
                Public Const MainHttp As String = "http://www.genome.jp/kegg-bin/show_organism?menu_type=pathway_maps&org="
                Public Const PictureHttpForSplit As String = "/kegg-bin/show_pathway?"
                Public Const PictureHttpToDownload As String = "http://www.genome.jp/kegg/pathway/"
                Public Const KeggPictureFileExtension As String = ".png"
                Public Const DownloadedPictureFileExtension As String = ".bmp"
                Public Const KeggXmlHttp As String = "http://www.genome.jp/kegg-bin/download?entry="
                Public Const KeggXmlFileExtension As String = "&format=kgml"
                Public Const DownloadedXmlFileExtension As String = ".xml"
            End Class
        End Class
        Public Class Extension
            Public Const Whole As String = "Whole"
            Public Const FivePrimeExtension As String = "FivePrimeExtension"
            Public Const ThreePrimeExtension As String = "ThreePrimeExtension"
            Public Const FiveAndThreePrimeExtension As String = "FiveAndThreePrimeExtension"
            Public Const Whole_And_Processed As String = "Whole_And_Processed"
        End Class
        Public Enum Location_Find_Type
            Count_Both = 1
            Count_Start_Independent_Strain = 2
            Count_End_Independent_Strain = 3
            Contain_Full = 4
        End Enum
        Public Enum Location_Type
            BED = 1
            GFF3 = 2
            Features = 3
            Standard = 4
        End Enum
        Public Class Merge
            Public Const Neighbor As String = "Neighbor"
            Public Const Count As String = "Count"

        End Class
        Public Class LocationsAndCounts
            Public Const SeqID = "SeqID"
            Public Const Start = "Start"
            Public Const [End] = "End"
            Public Const Strand = "Strand"
            Public Const Type = "Type"
            Public Const LocusTag = "LocusTag"
            Public Const Product = "Product"

            Public Const Interesting = "Interesting"
            Public Const Non_Interesting = "Not_Interesting"
            Public Const Total = "Total"
            Public Const Percents = "Percents"
        End Class
        Public Class OutPutType
            Public Const AsTabFile As Integer = 0

        End Class
        Public Class SeqGroups
            Public Const UniqueByID As String = "Unique By ID"
            Public Const UniqueBySeq As String = "Unique By Seq"
            Public Const UniqueByIDAndSeq As String = "Unique By ID & Seq"

            Public Const DuplicatedByID As String = "Duplicated By ID"
            Public Const DuplicatedBySeq As String = "Duplicated By Seq"
            Public Const DuplicatedByIDAndSeq As String = "Duplicated By ID & Seq"

            Public Const OneCopyByID As String = "1 Copy By ID"
            Public Const OneCopyBySeq As String = "1 Copy By Seq"
            Public Const OneCopyByIDAndSeq As String = "1 Copy By ID & Seq"
        End Class
        Public Class TranscriptType
            Public Const CDS As Integer = 1
            Public Const CDSwPromoter As Integer = 2
            Public Const CDSw3PrimeUTR As Integer = 3
            Public Const CDSwPromoterAnd3PrimeUTR As Integer = 4
            Public Const Gene As Integer = 5
            Public Const CDSToEndOfGene As Integer = 6
            Public Const CDSToEndOfGeneANd3Prime As Integer = 7

        End Class
        Public Class DelimitedFileImport
            Public Const SelectFirstRowAndColumn = "SelectFirstRowAndColumn"
            Public Const SelectFirstRowAndColumns = "SelectFirstRowAndColumns"
            Public Const SelectHeaderRow As String = "Select Header Row"
            Public Const SelectHeadersAndColumns = "SelectHeaderAndColumns"

        End Class
        Public Class BackGroundWork
            Public Const Sequences_With_Motifs As String = "Sequences With Motifs"
            Public Const Features As String = "Get_Features"
            Public Const Locations As String = "Get_Locations"
            Public Const Sequences As String = "Import_Sequences"
            Public Const ModyfiedSequence As String = "Modified_Sequences"
            Public Const Gff3Parser As String = "Gff3Parser_Sequences"
            Public Const DownLoad As String = "DownLoad"
            Public Const Mapping As String = "Mapping"
            Public Const AffyMapping As String = "AffyMapping"
            Public Const ReplaceStringsInFiles = "ReplaceStringsInFiles"
            Public Const MaintainUniqeSequence As String = "MaintanOnlyUniqueSequences"
            Public Const Name As String = "ShortFileName"
            Public Const TranscriptDiscovery As String = "TranscriptDiscovery"
            Public Const MergeLocationsAndFeatures As String = "MergeLocationsAndFeatures"
            Public Const Counts As String = "Counts"
            Public Const CDS_For_Exon_Intron As String = "CDS_For_Exon_Intron"
            Public Const Items_With_Properties As String = "Items_With_Properties"
            Public Const SamTools = "SamTools"
        End Class
    End Namespace
End Namespace
