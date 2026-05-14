namespace VeteranEvidenceAssist.Core.Models;

public sealed class EvidenceTimeline
{
    public Guid Id { get; init; } = Guid.NewGuid();
    public string Name { get; set; } = "Evidence Timeline";
    public DateTimeOffset CreatedAt { get; init; } = DateTimeOffset.UtcNow;
    public List<EvidenceItem> Items { get; init; } = [];
}
