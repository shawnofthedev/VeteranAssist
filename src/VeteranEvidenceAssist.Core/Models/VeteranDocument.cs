using VeteranEvidenceAssist.Core.Enums;

namespace VeteranEvidenceAssist.Core.Models;

public sealed class VeteranDocument
{
    public Guid Id { get; init; } = Guid.NewGuid();
    public Guid DocumentId => Id;
    public string OriginalFileName { get; set; } = string.Empty;
    public string LocalFilePath { get; set; } = string.Empty;
    public string StoredPath
    {
        get => LocalFilePath;
        set => LocalFilePath = value;
    }

    public DocumentType DocumentType { get; set; } = DocumentType.Unknown;
    public string Sha256Hash { get; set; } = string.Empty;
    public DateTimeOffset ImportedAt { get; init; } = DateTimeOffset.UtcNow;
    public DateTimeOffset ImportedAtUtc => ImportedAt.ToUniversalTime();
    public DocumentExtractionStatus ExtractionStatus { get; set; } = DocumentExtractionStatus.Unknown;
    public bool RequiresOcr { get; set; }
    public int ExtractedTextCharacterCount { get; set; }
    public string ExtractedTextPreview { get; set; } = string.Empty;
    public string RedactionStatus { get; set; } = "NotStarted";
    public bool ContainsSensitiveInformation { get; set; } = true;
    public List<DocumentPage> Pages { get; init; } = [];
}
