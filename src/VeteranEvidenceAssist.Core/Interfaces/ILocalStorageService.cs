using VeteranEvidenceAssist.Core.Models;

namespace VeteranEvidenceAssist.Core.Interfaces;

public interface ILocalStorageService
{
    Task SaveDocumentAsync(VeteranDocument document, CancellationToken cancellationToken = default);

    Task<VeteranDocument?> GetDocumentAsync(Guid documentId, CancellationToken cancellationToken = default);

    Task<IReadOnlyList<VeteranDocument>> ListDocumentsAsync(CancellationToken cancellationToken = default);

    Task SaveEvidenceTimelineAsync(EvidenceTimeline timeline, CancellationToken cancellationToken = default);
}
