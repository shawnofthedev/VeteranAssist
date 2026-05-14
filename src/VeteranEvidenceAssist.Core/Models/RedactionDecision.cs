using VeteranEvidenceAssist.Core.Enums;

namespace VeteranEvidenceAssist.Core.Models;

public sealed class RedactionDecision
{
    public Guid Id { get; init; } = Guid.NewGuid();
    public Guid PiiEntityId { get; set; }
    public RedactionDecisionStatus Status { get; set; } = RedactionDecisionStatus.PendingReview;
    public string ReplacementText { get; set; } = "[REDACTED]";
    public string? ReviewerNote { get; set; }
    public DateTimeOffset? ReviewedAt { get; set; }
}
