Imports System.Drawing
Public Class ExtenededHsp
    Inherits Bio.Web.Blast.Hsp

    Public Fullseq As String
    Public Property hitLength As Integer
    Public Property HSP As Bio.Web.Blast.Hsp
    Public Property Query_Length As Integer
    Public Property Query_Definition As String
    Public Property Query_ID As String
    Public Property Hit_ID As String
    Public Property Hit_Definition As String
    Public Property TheHitFrame As Integer
    Public Sub New(Hsp As Bio.Web.Blast.Hsp, BRecord As Bio.Web.Blast.BlastSearchRecord, Hit As Bio.Web.Blast.Hit)
        Me.Hit_Definition = Hit.Def
        Me.Hit_ID = Hit.Id
        Me.AlignmentLength = Hsp.AlignmentLength
        Me.TheHitFrame = Hsp.HitFrame
        Me.HitStart = Hsp.HitStart
        Me.QueryStart = Hsp.QueryStart
        Me.QueryEnd = Hsp.QueryEnd
        Me.HitEnd = Hsp.HitEnd
        Me.IdentitiesCount = Hsp.IdentitiesCount
        Me.EValue = Hsp.EValue
        Me.Gaps = Hsp.Gaps
        Me.Score = Hsp.Score
        Me.Query_Length = BRecord.IterationQueryLength
        Me.Query_Definition = BRecord.IterationQueryDefinition
        Me.Query_ID = BRecord.IterationQueryId
        Me.hitLength = Hit.Length
        Me.Fullseq = Hsp.QuerySequence & vbCrLf & Hsp.Midline & vbCrLf & Hsp.HitSequence
        Me.HSP = Hsp
    End Sub


End Class
