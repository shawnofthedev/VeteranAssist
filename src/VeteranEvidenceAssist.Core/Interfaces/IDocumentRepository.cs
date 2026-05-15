using VeteranEvidenceAssist.Core.Models;

namespace VeteranEvidenceAssist.Core.Interfaces;

public interface IDocumentRepository
{
    Task SaveDocumentAsync(VeteranDocument document, CancellationToken cancellationToken = default);

    Task<VeteranDocument?> GetDocumentAsync(Guid documentId, CancellationToken cancellationToken = default);

    Task<VeteranDocument?> FindBySha256HashAsync(string sha256Hash, CancellationToken cancellationToken = default);

    Task<IReadOnlyList<VeteranDocument>> ListDocumentsAsync(CancellationToken cancellationToken = default);
}
