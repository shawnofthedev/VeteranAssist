using VeteranEvidenceAssist.Core.Models;

namespace VeteranEvidenceAssist.Core.Interfaces;

public interface IPiiDetectionService
{
    Task<IReadOnlyList<PiiEntity>> DetectAsync(ExtractedTextBlock textBlock, CancellationToken cancellationToken = default);
}
