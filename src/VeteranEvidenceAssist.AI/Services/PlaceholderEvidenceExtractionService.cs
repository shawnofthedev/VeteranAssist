using VeteranEvidenceAssist.Core.Interfaces;
using VeteranEvidenceAssist.Core.Models;

namespace VeteranEvidenceAssist.AI.Services;

public sealed class PlaceholderEvidenceExtractionService : IEvidenceExtractionService
{
    public Task<IReadOnlyList<EvidenceItem>> ExtractEvidenceAsync(VeteranDocument document, CancellationToken cancellationToken = default)
    {
        // TODO Phase 2: Extract factual evidence only; keep interpretation separate from source evidence.
        IReadOnlyList<EvidenceItem> items = [];
        return Task.FromResult(items);
    }
}
