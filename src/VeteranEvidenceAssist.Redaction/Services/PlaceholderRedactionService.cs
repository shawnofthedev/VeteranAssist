using VeteranEvidenceAssist.Core.Interfaces;
using VeteranEvidenceAssist.Core.Models;

namespace VeteranEvidenceAssist.Redaction.Services;

public sealed class PlaceholderRedactionService : IRedactionService
{
    public Task<IReadOnlyList<RedactionDecision>> CreateReviewQueueAsync(VeteranDocument document, CancellationToken cancellationToken = default)
    {
        // TODO Phase 1: Build a manual review queue from detected PII before enabling export.
        IReadOnlyList<RedactionDecision> decisions = [];
        return Task.FromResult(decisions);
    }

    public Task<string> ExportRedactedPdfAsync(
        VeteranDocument document,
        IReadOnlyList<RedactionDecision> approvedDecisions,
        string outputPath,
        CancellationToken cancellationToken = default)
    {
        // TODO Phase 2: Implement true PDF redaction that removes underlying content and flattens the export.
        throw new NotImplementedException("Secure redacted PDF export is intentionally not implemented in Phase 0.");
    }
}
