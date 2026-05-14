using VeteranEvidenceAssist.Core.Enums;
using VeteranEvidenceAssist.Core.ValueObjects;

namespace VeteranEvidenceAssist.Core.Models;

public sealed class PiiEntity
{
    public Guid Id { get; init; } = Guid.NewGuid();
    public Guid ExtractedTextBlockId { get; set; }
    public PiiEntityType EntityType { get; set; } = PiiEntityType.Unknown;
    public TextRange TextRange { get; set; } = new(0, 0);
    public string RedactedPreview { get; set; } = string.Empty;
    public double Confidence { get; set; }
    public string DetectionSource { get; set; } = "Local placeholder";
    public DateTimeOffset DetectedAt { get; init; } = DateTimeOffset.UtcNow;
}
