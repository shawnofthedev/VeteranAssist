using System.Text.Json;
using VeteranEvidenceAssist.Core.Interfaces;
using VeteranEvidenceAssist.Core.Models;
using VeteranEvidenceAssist.Core.Options;

namespace VeteranEvidenceAssist.Storage.Repositories;

public sealed class JsonLocalStorageService : ILocalStorageService
{
    private static readonly JsonSerializerOptions SerializerOptions = new(JsonSerializerDefaults.Web)
    {
        WriteIndented = true
    };

    private readonly SemaphoreSlim _gate = new(1, 1);
    private readonly LocalWorkspaceOptions _workspaceOptions;

    public JsonLocalStorageService(LocalWorkspaceOptions workspaceOptions)
    {
        _workspaceOptions = workspaceOptions;
    }

    public async Task SaveDocumentAsync(VeteranDocument document, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(document);

        await _gate.WaitAsync(cancellationToken);
        try
        {
            var documents = await ReadDocumentsAsync(cancellationToken);
            var existingIndex = documents.FindIndex(existing => existing.Id == document.Id);

            if (existingIndex >= 0)
            {
                documents[existingIndex] = document;
            }
            else
            {
                documents.Add(document);
            }

            await WriteDocumentsAsync(documents, cancellationToken);
        }
        finally
        {
            _gate.Release();
        }
    }

    public async Task<VeteranDocument?> GetDocumentAsync(Guid documentId, CancellationToken cancellationToken = default)
    {
        await _gate.WaitAsync(cancellationToken);
        try
        {
            var documents = await ReadDocumentsAsync(cancellationToken);
            return documents.FirstOrDefault(document => document.Id == documentId);
        }
        finally
        {
            _gate.Release();
        }
    }

    public async Task<IReadOnlyList<VeteranDocument>> ListDocumentsAsync(CancellationToken cancellationToken = default)
    {
        await _gate.WaitAsync(cancellationToken);
        try
        {
            var documents = await ReadDocumentsAsync(cancellationToken);
            return documents
                .OrderByDescending(document => document.ImportedAt)
                .ToList();
        }
        finally
        {
            _gate.Release();
        }
    }

    public Task SaveEvidenceTimelineAsync(EvidenceTimeline timeline, CancellationToken cancellationToken = default)
    {
        // Phase 1 persists document metadata only. Evidence timelines arrive in a later workflow.
        return Task.CompletedTask;
    }

    private async Task<List<VeteranDocument>> ReadDocumentsAsync(CancellationToken cancellationToken)
    {
        if (!File.Exists(_workspaceOptions.DocumentMetadataPath))
        {
            return [];
        }

        await using var stream = File.OpenRead(_workspaceOptions.DocumentMetadataPath);
        return await JsonSerializer.DeserializeAsync<List<VeteranDocument>>(stream, SerializerOptions, cancellationToken)
            ?? [];
    }

    private async Task WriteDocumentsAsync(List<VeteranDocument> documents, CancellationToken cancellationToken)
    {
        Directory.CreateDirectory(_workspaceOptions.MetadataDirectoryPath);

        var tempPath = $"{_workspaceOptions.DocumentMetadataPath}.tmp";
        await using (var stream = File.Create(tempPath))
        {
            await JsonSerializer.SerializeAsync(stream, documents, SerializerOptions, cancellationToken);
        }

        File.Move(tempPath, _workspaceOptions.DocumentMetadataPath, true);
    }
}
