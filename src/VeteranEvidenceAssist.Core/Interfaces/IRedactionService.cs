using VeteranEvidenceAssist.Core.Models;

namespace VeteranEvidenceAssist.Core.Interfaces;

public interface IRedactionService
{
    Task<IReadOnlyList<RedactionDecision>> CreateReviewQueueAsync(VeteranDocument document, CancellationToken cancellationToken = default);

    Task<string> ExportRedactedPdfAsync(
        VeteranDocument document,
        IReadOnlyList<RedactionDecision> approvedDecisions,
        string outputPath,
        CancellationToken cancellationToken = default);
}
