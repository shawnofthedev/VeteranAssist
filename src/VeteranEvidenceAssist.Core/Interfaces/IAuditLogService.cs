using VeteranEvidenceAssist.Core.Models;

namespace VeteranEvidenceAssist.Core.Interfaces;

public interface IAuditLogService
{
    Task RecordAsync(AuditLogEntry entry, CancellationToken cancellationToken = default);

    Task<IReadOnlyList<AuditLogEntry>> ListRecentAsync(int count, CancellationToken cancellationToken = default);
}
