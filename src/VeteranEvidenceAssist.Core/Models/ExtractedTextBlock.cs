using VeteranEvidenceAssist.Core.Enums;
using VeteranEvidenceAssist.Core.ValueObjects;

namespace VeteranEvidenceAssist.Core.Models;

public sealed class ExtractedTextBlock
{
    public Guid Id { get; init; } = Guid.NewGuid();
    public Guid DocumentPageId { get; set; }
    public string Text { get; set; } = string.Empty;
    public TextExtractionMethod ExtractionMethod { get; set; } = TextExtractionMethod.Unknown;
    public double Confidence { get; set; }
    public TextRange? SourceRange { get; set; }
    public string? BoundingBoxJson { get; set; }
    public DateTimeOffset ExtractedAt { get; init; } = DateTimeOffset.UtcNow;
}
