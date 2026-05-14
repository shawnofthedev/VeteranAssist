using VeteranEvidenceAssist.Core.Interfaces;
using VeteranEvidenceAssist.Core.Models;

namespace VeteranEvidenceAssist.Documents.Services;

public sealed class PlaceholderTextExtractionService : ITextExtractionService, IOcrService
{
    public Task<IReadOnlyList<ExtractedTextBlock>> ExtractTextAsync(VeteranDocument document, CancellationToken cancellationToken = default)
    {
        // TODO Phase 1: Extract embedded text locally and route scanned pages to local OCR.
        IReadOnlyList<ExtractedTextBlock> blocks = [];
        return Task.FromResult(blocks);
    }

    public Task<IReadOnlyList<ExtractedTextBlock>> ExtractPageTextAsync(DocumentPage page, CancellationToken cancellationToken = default)
    {
        // TODO Phase 1: Use a local OCR engine. Do not upload page images silently.
        IReadOnlyList<ExtractedTextBlock> blocks = [];
        return Task.FromResult(blocks);
    }
}
