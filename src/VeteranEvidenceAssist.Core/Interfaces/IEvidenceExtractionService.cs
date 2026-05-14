using VeteranEvidenceAssist.Core.Models;

namespace VeteranEvidenceAssist.Core.Interfaces;

public interface IEvidenceExtractionService
{
    Task<IReadOnlyList<EvidenceItem>> ExtractEvidenceAsync(VeteranDocument document, CancellationToken cancellationToken = default);
}
