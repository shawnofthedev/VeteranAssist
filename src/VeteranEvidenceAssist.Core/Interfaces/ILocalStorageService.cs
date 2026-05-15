using VeteranEvidenceAssist.Core.Models;

namespace VeteranEvidenceAssist.Core.Interfaces;

public interface ILocalStorageService : IDocumentRepository
{
    Task SaveEvidenceTimelineAsync(EvidenceTimeline timeline, CancellationToken cancellationToken = default);
}
