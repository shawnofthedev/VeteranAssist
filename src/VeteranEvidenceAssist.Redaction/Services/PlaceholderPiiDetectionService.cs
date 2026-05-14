using VeteranEvidenceAssist.Core.Interfaces;
using VeteranEvidenceAssist.Core.Models;

namespace VeteranEvidenceAssist.Redaction.Services;

public sealed class PlaceholderPiiDetectionService : IPiiDetectionService
{
    public Task<IReadOnlyList<PiiEntity>> DetectAsync(ExtractedTextBlock textBlock, CancellationToken cancellationToken = default)
    {
        // TODO Phase 1: Add tested local PII detectors for SSNs, VA file numbers, contact info, and identifiers.
        IReadOnlyList<PiiEntity> entities = [];
        return Task.FromResult(entities);
    }
}
