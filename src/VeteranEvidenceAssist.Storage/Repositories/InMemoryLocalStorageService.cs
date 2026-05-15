using System.Collections.Concurrent;
using VeteranEvidenceAssist.Core.Interfaces;
using VeteranEvidenceAssist.Core.Models;

namespace VeteranEvidenceAssist.Storage.Repositories;

public sealed class InMemoryLocalStorageService : ILocalStorageService
{
    private readonly ConcurrentDictionary<Guid, VeteranDocument> _documents = new();
    private readonly ConcurrentDictionary<Guid, EvidenceTimeline> _timelines = new();

    public Task SaveDocumentAsync(VeteranDocument document, CancellationToken cancellationToken = default)
    {
        // TODO Phase 1: Replace with SQLite persistence and encrypted-at-rest storage for sensitive fields.
        _documents[document.Id] = document;
        return Task.CompletedTask;
    }

    public Task<VeteranDocument?> GetDocumentAsync(Guid documentId, CancellationToken cancellationToken = default)
    {
        _documents.TryGetValue(documentId, out var document);
        return Task.FromResult(document);
    }

    public Task<IReadOnlyList<VeteranDocument>> ListDocumentsAsync(CancellationToken cancellationToken = default)
    {
        IReadOnlyList<VeteranDocument> documents = _documents.Values
            .OrderByDescending(static document => document.ImportedAt)
            .ToList();

        return Task.FromResult(documents);
    }

    public Task<VeteranDocument?> FindBySha256HashAsync(string sha256Hash, CancellationToken cancellationToken = default)
    {
        var document = _documents.Values.FirstOrDefault(document =>
            string.Equals(document.Sha256Hash, sha256Hash, StringComparison.OrdinalIgnoreCase));

        return Task.FromResult(document);
    }

    public Task SaveEvidenceTimelineAsync(EvidenceTimeline timeline, CancellationToken cancellationToken = default)
    {
        _timelines[timeline.Id] = timeline;
        return Task.CompletedTask;
    }
}
