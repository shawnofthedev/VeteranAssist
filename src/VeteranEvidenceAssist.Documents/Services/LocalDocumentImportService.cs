using VeteranEvidenceAssist.Core.Enums;
using VeteranEvidenceAssist.Core.Interfaces;
using VeteranEvidenceAssist.Core.Models;
using VeteranEvidenceAssist.Core.Options;

namespace VeteranEvidenceAssist.Documents.Services;

public class LocalDocumentImportService : IDocumentImportService
{
    private const int MinimumEmbeddedTextCharacters = 10;
    private const int PreviewCharacterLimit = 4000;

    private readonly IDocumentRepository _documentRepository;
    private readonly ITextExtractionService _textExtractionService;
    private readonly IFileHashService _fileHashService;
    private readonly LocalWorkspaceOptions _workspaceOptions;

    public LocalDocumentImportService(
        IDocumentRepository documentRepository,
        ITextExtractionService textExtractionService,
        IFileHashService fileHashService,
        LocalWorkspaceOptions workspaceOptions)
    {
        _documentRepository = documentRepository;
        _textExtractionService = textExtractionService;
        _fileHashService = fileHashService;
        _workspaceOptions = workspaceOptions;
    }

    public async Task<VeteranDocument> ImportAsync(string filePath, CancellationToken cancellationToken = default)
    {
        ValidateImport(filePath);

        Directory.CreateDirectory(_workspaceOptions.DocumentsDirectoryPath);

        var sourceHash = await _fileHashService.ComputeSha256Async(filePath, cancellationToken);
        var existingDocument = await FindExistingImportAsync(sourceHash, cancellationToken);
        if (existingDocument is not null)
        {
            return existingDocument;
        }

        var documentId = Guid.NewGuid();
        var localFilePath = BuildLocalImportPath(documentId, filePath);
        Directory.CreateDirectory(Path.GetDirectoryName(localFilePath)!);

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
            DeleteEmptyDocumentDirectory(localFilePath);
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
        document.RequiresOcr = document.ExtractionStatus == DocumentExtractionStatus.OcrNeeded;
        document.ExtractedTextPreview = BuildPreview(document);

        await _documentRepository.SaveDocumentAsync(document, cancellationToken);

        return document;
    }

    private static DocumentExtractionStatus DetermineExtractionStatus(VeteranDocument document)
    {
        if (document.Pages.Count == 0)
        {
            return DocumentExtractionStatus.Unknown;
        }

        if (document.ExtractedTextCharacterCount >= MinimumEmbeddedTextCharacters)
        {
            return DocumentExtractionStatus.EmbeddedTextExtracted;
        }

        return DocumentExtractionStatus.OcrNeeded;
    }

    private async Task<VeteranDocument?> FindExistingImportAsync(string sha256Hash, CancellationToken cancellationToken)
    {
        var document = await _documentRepository.FindBySha256HashAsync(sha256Hash, cancellationToken);
        return document is not null && File.Exists(document.StoredPath)
            ? document
            : null;
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
        return Path.Combine(_workspaceOptions.DocumentsDirectoryPath, documentId.ToString("N"), "original.pdf");
    }

    private static string BuildPreview(VeteranDocument document)
    {
        var preview = string.Join(
            Environment.NewLine,
            document.Pages
                .OrderBy(static page => page.PageNumber)
                .SelectMany(static page => page.TextBlocks)
                .Select(static block => block.Text))
            .Trim();

        return preview.Length <= PreviewCharacterLimit
            ? preview
            : preview[..PreviewCharacterLimit];
    }

    private static void DeleteEmptyDocumentDirectory(string localFilePath)
    {
        var directory = Path.GetDirectoryName(localFilePath);
        if (string.IsNullOrWhiteSpace(directory) || !Directory.Exists(directory))
        {
            return;
        }

        if (!Directory.EnumerateFileSystemEntries(directory).Any())
        {
            Directory.Delete(directory);
        }
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
