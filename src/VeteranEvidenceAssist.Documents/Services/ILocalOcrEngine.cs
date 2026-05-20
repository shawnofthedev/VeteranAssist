using VeteranEvidenceAssist.Core.Models;

namespace VeteranEvidenceAssist.Documents.Services;

public interface ILocalOcrEngine
{
    Task<IReadOnlyList<ExtractedTextBlock>> RecognizePageAsync(DocumentPage page, CancellationToken cancellationToken = default);
}
