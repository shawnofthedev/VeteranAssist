using VeteranEvidenceAssist.Core.Enums;
using VeteranEvidenceAssist.Core.ValueObjects;

namespace VeteranEvidenceAssist.Core.Models;

public sealed class EvidenceItem
{
    public Guid Id { get; init; } = Guid.NewGuid();
    public EvidenceCategory Category { get; set; } = EvidenceCategory.Unknown;
    public DateOnly? EventDate { get; set; }
    public string Summary { get; set; } = string.Empty;
    public string SourceExcerpt { get; set; } = string.Empty;
    public SourceReference? SourceReference { get; set; }
    public double Confidence { get; set; }
    public bool IsAiGeneratedInterpretation { get; set; }
    public DateTimeOffset ExtractedAt { get; init; } = DateTimeOffset.UtcNow;
}
