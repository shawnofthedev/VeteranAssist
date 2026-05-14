using VeteranEvidenceAssist.Core.Models;

namespace VeteranEvidenceAssist.Core.Interfaces;

public interface IOcrService
{
    Task<IReadOnlyList<ExtractedTextBlock>> ExtractPageTextAsync(DocumentPage page, CancellationToken cancellationToken = default);
}
