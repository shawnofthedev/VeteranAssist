using VeteranEvidenceAssist.Core.Models;

namespace VeteranEvidenceAssist.Documents.Services;

public sealed class NoOpLocalOcrEngine : ILocalOcrEngine
{
    public Task<IReadOnlyList<ExtractedTextBlock>> RecognizePageAsync(DocumentPage page, CancellationToken cancellationToken = default)
    {
        IReadOnlyList<ExtractedTextBlock> blocks = [];
        return Task.FromResult(blocks);
    }
}
