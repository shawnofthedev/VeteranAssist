namespace VeteranEvidenceAssist.Core.Models;

public sealed class PromptTemplate
{
    public Guid Id { get; init; } = Guid.NewGuid();
    public string Name { get; set; } = string.Empty;
    public string Purpose { get; set; } = string.Empty;
    public string TemplateText { get; set; } = string.Empty;
    public bool AllowsExternalAiUse { get; set; }
}
