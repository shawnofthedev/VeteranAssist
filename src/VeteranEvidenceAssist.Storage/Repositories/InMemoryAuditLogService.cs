using VeteranEvidenceAssist.Core.Interfaces;
using VeteranEvidenceAssist.Core.Models;

namespace VeteranEvidenceAssist.Storage.Repositories;

public sealed class InMemoryAuditLogService : IAuditLogService
{
    private readonly List<AuditLogEntry> _entries = [];
    private readonly Lock _lock = new();

    public Task RecordAsync(AuditLogEntry entry, CancellationToken cancellationToken = default)
    {
        // Store action metadata only. Never include raw document text or PII in audit messages.
        lock (_lock)
        {
            _entries.Add(entry);
        }

        return Task.CompletedTask;
    }

    public Task<IReadOnlyList<AuditLogEntry>> ListRecentAsync(int count, CancellationToken cancellationToken = default)
    {
        lock (_lock)
        {
            IReadOnlyList<AuditLogEntry> recentEntries = _entries
                .OrderByDescending(static entry => entry.CreatedAt)
                .Take(count)
                .ToList();

            return Task.FromResult(recentEntries);
        }
    }
}
