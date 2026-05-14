using System.Text;
using VeteranEvidenceAssist.Core.Interfaces;
using VeteranEvidenceAssist.Core.Models;

namespace VeteranEvidenceAssist.AI.Services;

public sealed class LocalPromptGenerationService : IPromptGenerationService
{
    public Task<PromptPacket> GeneratePacketAsync(
        PromptTemplate template,
        IReadOnlyList<EvidenceItem> evidenceItems,
        CancellationToken cancellationToken = default)
    {
        var evidencePreview = BuildEvidencePreview(evidenceItems);

        return Task.FromResult(new PromptPacket
        {
            PromptTemplateId = template.Id,
            PromptText = template.TemplateText.Replace("{{evidence}}", evidencePreview, StringComparison.OrdinalIgnoreCase),
            EvidenceTextPreview = evidencePreview,
            UserConfirmedReviewBeforeCopy = false
        });
    }

    private static string BuildEvidencePreview(IEnumerable<EvidenceItem> evidenceItems)
    {
        var builder = new StringBuilder();
        builder.AppendLine("User-reviewed evidence excerpts:");

        foreach (var item in evidenceItems.OrderBy(static item => item.EventDate))
        {
            builder.Append("- ");
            builder.Append(item.EventDate?.ToString("yyyy-MM-dd") ?? "Date unknown");
            builder.Append(" | ");
            builder.Append(item.Category);
            builder.Append(" | ");
            builder.Append(item.Summary);
            builder.Append(" | Source: ");
            builder.AppendLine(item.SourceReference?.LocationDescription ?? "Source not selected");
        }

        return builder.ToString();
    }
}
