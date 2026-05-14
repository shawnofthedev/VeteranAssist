using VeteranEvidenceAssist.Core.Enums;

namespace VeteranEvidenceAssist.Core.Models;

public sealed class AuditLogEntry
{
    public Guid Id { get; init; } = Guid.NewGuid();
    public AuditActionType ActionType { get; set; }
    public string Message { get; set; } = string.Empty;
    public Guid? RelatedEntityId { get; set; }
    public DateTimeOffset CreatedAt { get; init; } = DateTimeOffset.UtcNow;
}
