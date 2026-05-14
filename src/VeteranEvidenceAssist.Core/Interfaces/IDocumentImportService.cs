using VeteranEvidenceAssist.Core.Models;

namespace VeteranEvidenceAssist.Core.Interfaces;

public interface IDocumentImportService
{
    Task<VeteranDocument> ImportAsync(string filePath, CancellationToken cancellationToken = default);
}
