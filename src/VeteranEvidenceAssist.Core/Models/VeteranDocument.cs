using VeteranEvidenceAssist.Core.Enums;

namespace VeteranEvidenceAssist.Core.Models;

public sealed class VeteranDocument
{
    public Guid Id { get; init; } = Guid.NewGuid();
    public string OriginalFileName { get; set; } = string.Empty;
    public string LocalFilePath { get; set; } = string.Empty;
    public DocumentType DocumentType { get; set; } = DocumentType.Unknown;
    public string Sha256Hash { get; set; } = string.Empty;
    public DateTimeOffset ImportedAt { get; init; } = DateTimeOffset.UtcNow;
    public DocumentExtractionStatus ExtractionStatus { get; set; } = DocumentExtractionStatus.Unknown;
    public int ExtractedTextCharacterCount { get; set; }
    public bool ContainsSensitiveInformation { get; set; } = true;
    public List<DocumentPage> Pages { get; init; } = [];
}
