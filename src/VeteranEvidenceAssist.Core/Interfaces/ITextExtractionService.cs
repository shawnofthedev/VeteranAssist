using VeteranEvidenceAssist.Core.Models;

namespace VeteranEvidenceAssist.Core.Interfaces;

public interface ITextExtractionService
{
    Task<IReadOnlyList<ExtractedTextBlock>> ExtractTextAsync(VeteranDocument document, CancellationToken cancellationToken = default);
}
