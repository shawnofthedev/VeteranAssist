namespace VeteranEvidenceAssist.Core.Models;

public sealed class PromptPacket
{
    public Guid Id { get; init; } = Guid.NewGuid();
    public Guid PromptTemplateId { get; set; }
    public string PromptText { get; set; } = string.Empty;
    public string EvidenceTextPreview { get; set; } = string.Empty;
    public bool UserConfirmedReviewBeforeCopy { get; set; }
    public DateTimeOffset CreatedAt { get; init; } = DateTimeOffset.UtcNow;
}
