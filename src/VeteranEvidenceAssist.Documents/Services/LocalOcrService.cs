using VeteranEvidenceAssist.Core.Enums;
using VeteranEvidenceAssist.Core.Interfaces;
using VeteranEvidenceAssist.Core.Models;

namespace VeteranEvidenceAssist.Documents.Services;

public sealed class LocalOcrService : IOcrService
{
    private readonly ILocalOcrEngine _ocrEngine;

    public LocalOcrService(ILocalOcrEngine ocrEngine)
    {
        _ocrEngine = ocrEngine;
    }

    public async Task<IReadOnlyList<ExtractedTextBlock>> ExtractPageTextAsync(DocumentPage page, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(page);

        var recognizedBlocks = await _ocrEngine.RecognizePageAsync(page, cancellationToken);

        return recognizedBlocks
            .Where(block => !string.IsNullOrWhiteSpace(block.Text))
            .Select(block => new ExtractedTextBlock
            {
                DocumentPageId = page.Id,
                Text = block.Text.Trim(),
                ExtractionMethod = TextExtractionMethod.LocalOcr,
                Confidence = Math.Clamp(block.Confidence, 0, 1),
                SourceRange = block.SourceRange,
                BoundingBoxJson = block.BoundingBoxJson
            })
            .ToList();
    }
}
