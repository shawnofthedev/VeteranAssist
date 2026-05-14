namespace VeteranEvidenceAssist.Core.Enums;

public enum AuditActionType
{
    DocumentImported,
    TextExtracted,
    PiiDetected,
    RedactionReviewed,
    RedactedDocumentExported,
    EvidenceExtracted,
    PromptPacketGenerated,
    SettingsChanged
}
