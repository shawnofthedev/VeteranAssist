using VeteranEvidenceAssist.Core.Enums;
using VeteranEvidenceAssist.Core.Interfaces;
using VeteranEvidenceAssist.Core.Models;
using VeteranEvidenceAssist.Core.Options;

namespace VeteranEvidenceAssist.Documents.Services;

public sealed class PlaceholderDocumentImportService : IDocumentImportService
{
    private readonly ILocalStorageService _localStorageService;
    private readonly ITextExtractionService _textExtractionService;
    private readonly IFileHashService _fileHashService;
    private readonly LocalWorkspaceOptions _workspaceOptions;

    public PlaceholderDocumentImportService(
        ILocalStorageService localStorageService,
        ITextExtractionService textExtractionService,
        IFileHashService fileHashService,
        LocalWorkspaceOptions workspaceOptions)
    {
        _localStorageService = localStorageService;
        _textExtractionService = textExtractionService;
        _fileHashService = fileHashService;
        _workspaceOptions = workspaceOptions;
    }

    public async Task<VeteranDocument> ImportAsync(string filePath, CancellationToken cancellationToken = default)
    {
        ValidateImport(filePath);

        Directory.CreateDirectory(_workspaceOptions.ImportsDirectoryPath);

        var documentId = Guid.NewGuid();
        var localFilePath = BuildLocalImportPath(documentId, filePath);

        await using (var source = File.OpenRead(filePath))
        await using (var destination = File.Create(localFilePath))
        {
            await source.CopyToAsync(destination, cancellationToken);
        }

        var hash = await _fileHashService.ComputeSha256Async(localFilePath, cancellationToken);

        var document = new VeteranDocument
        {
            Id = documentId,
            OriginalFileName = Path.GetFileName(filePath),
            LocalFilePath = localFilePath,
            DocumentType = InferDocumentType(filePath),
            Sha256Hash = hash,
            ContainsSensitiveInformation = true
        };

        IReadOnlyList<ExtractedTextBlock> textBlocks;
        try
        {
            textBlocks = await _textExtractionService.ExtractTextAsync(document, cancellationToken);
        }
        catch
        {
            File.Delete(localFilePath);
            throw;
        }

        foreach (var block in textBlocks)
        {
            var page = document.Pages.FirstOrDefault(page => page.Id == block.DocumentPageId);
            page?.TextBlocks.Add(block);
        }

        document.ExtractedTextCharacterCount = document.Pages
            .SelectMany(page => page.TextBlocks)
            .Sum(block => block.Text.Length);
        document.ExtractionStatus = DetermineExtractionStatus(document);

        await _localStorageService.SaveDocumentAsync(document, cancellationToken);

        return document;
    }

    private static DocumentExtractionStatus DetermineExtractionStatus(VeteranDocument document)
    {
        if (document.Pages.Count == 0)
        {
            return DocumentExtractionStatus.Unknown;
        }

        if (document.ExtractedTextCharacterCount >= 10)
        {
            return DocumentExtractionStatus.EmbeddedTextExtracted;
        }

        return DocumentExtractionStatus.OcrNeeded;
    }

    private static void ValidateImport(string filePath)
    {
        if (string.IsNullOrWhiteSpace(filePath))
        {
            throw new ArgumentException("A PDF file path is required.", nameof(filePath));
        }

        if (!File.Exists(filePath))
        {
            throw new FileNotFoundException("The selected import file was not found.", filePath);
        }

        if (!string.Equals(Path.GetExtension(filePath), ".pdf", StringComparison.OrdinalIgnoreCase))
        {
            throw new NotSupportedException("Phase 1 imports support PDF files only.");
        }
    }

    private string BuildLocalImportPath(Guid documentId, string originalFilePath)
    {
        var extension = Path.GetExtension(originalFilePath).ToLowerInvariant();
        return Path.Combine(_workspaceOptions.ImportsDirectoryPath, $"{documentId:N}{extension}");
    }

    private static DocumentType InferDocumentType(string filePath)
    {
        var fileName = Path.GetFileName(filePath);

        if (fileName.Contains("dd214", StringComparison.OrdinalIgnoreCase) ||
            fileName.Contains("dd-214", StringComparison.OrdinalIgnoreCase))
        {
            return DocumentType.DD214;
        }

        if (string.Equals(Path.GetExtension(filePath), ".pdf", StringComparison.OrdinalIgnoreCase))
        {
            return DocumentType.Other;
        }

        return DocumentType.Unknown;
    }
}
