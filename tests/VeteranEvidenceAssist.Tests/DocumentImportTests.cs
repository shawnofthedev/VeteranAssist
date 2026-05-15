using System.Security.Cryptography;
using UglyToad.PdfPig.Content;
using UglyToad.PdfPig.Core;
using UglyToad.PdfPig.Fonts.Standard14Fonts;
using UglyToad.PdfPig.Writer;
using VeteranEvidenceAssist.Core.Models;
using VeteranEvidenceAssist.Core.Options;
using VeteranEvidenceAssist.Documents.Services;
using VeteranEvidenceAssist.Security.Services;
using VeteranEvidenceAssist.Storage.Repositories;

namespace VeteranEvidenceAssist.Tests;

public sealed class DocumentImportTests
{
    [Fact]
    public async Task Import_copies_pdf_into_workspace_and_hashes_copied_file()
    {
        using var workspace = TestWorkspace.Create();
        var sourcePdf = workspace.WriteMinimalPdf("source.pdf");
        var storage = new JsonLocalStorageService(workspace.Options);
        var importer = CreateImporter(storage, workspace.Options);
        var originalBytes = await File.ReadAllBytesAsync(sourcePdf);

        var document = await importer.ImportAsync(sourcePdf);

        Assert.NotEqual(sourcePdf, document.LocalFilePath);
        Assert.True(File.Exists(document.LocalFilePath));
        Assert.Equal(originalBytes, await File.ReadAllBytesAsync(sourcePdf));
        Assert.StartsWith(workspace.Options.ImportsDirectoryPath, document.LocalFilePath, StringComparison.OrdinalIgnoreCase);

        await using var stream = File.OpenRead(document.LocalFilePath);
        var expectedHash = Convert.ToHexString(await SHA256.HashDataAsync(stream));
        Assert.Equal(expectedHash, document.Sha256Hash);
        Assert.Contains(document.Pages, page => page.TextBlocks.Any(block => block.Text.Contains("Local import test", StringComparison.Ordinal)));
    }

    [Fact]
    public async Task Import_rejects_non_pdf_files()
    {
        using var workspace = TestWorkspace.Create();
        var textFile = Path.Combine(workspace.RootPath, "notes.txt");
        Directory.CreateDirectory(workspace.RootPath);
        await File.WriteAllTextAsync(textFile, "not a pdf");
        var importer = CreateImporter(new JsonLocalStorageService(workspace.Options), workspace.Options);

        await Assert.ThrowsAsync<NotSupportedException>(() => importer.ImportAsync(textFile));
    }

    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    public async Task Import_rejects_empty_paths(string filePath)
    {
        using var workspace = TestWorkspace.Create();
        var importer = CreateImporter(new JsonLocalStorageService(workspace.Options), workspace.Options);

        await Assert.ThrowsAsync<ArgumentException>(() => importer.ImportAsync(filePath));
    }

    [Fact]
    public async Task Import_rejects_missing_pdf_file()
    {
        using var workspace = TestWorkspace.Create();
        var missingPath = Path.Combine(workspace.RootPath, "missing.pdf");
        var importer = CreateImporter(new JsonLocalStorageService(workspace.Options), workspace.Options);

        await Assert.ThrowsAsync<FileNotFoundException>(() => importer.ImportAsync(missingPath));
    }

    [Fact]
    public async Task Import_creates_workspace_directories_when_missing()
    {
        using var workspace = TestWorkspace.Create();
        var sourcePdf = workspace.WriteMinimalPdf("workspace.pdf");
        var storage = new JsonLocalStorageService(workspace.Options);
        var importer = CreateImporter(storage, workspace.Options);

        await importer.ImportAsync(sourcePdf);

        Assert.True(Directory.Exists(workspace.Options.ImportsDirectoryPath));
        Assert.True(Directory.Exists(workspace.Options.MetadataDirectoryPath));
        Assert.True(File.Exists(workspace.Options.DocumentMetadataPath));
    }

    [Fact]
    public async Task Corrupt_pdf_does_not_persist_partial_metadata()
    {
        using var workspace = TestWorkspace.Create();
        Directory.CreateDirectory(workspace.RootPath);
        var corruptPdf = Path.Combine(workspace.RootPath, "corrupt.pdf");
        await File.WriteAllTextAsync(corruptPdf, "not really a pdf");
        var storage = new JsonLocalStorageService(workspace.Options);
        var importer = CreateImporter(storage, workspace.Options);

        await Assert.ThrowsAnyAsync<Exception>(() => importer.ImportAsync(corruptPdf));

        var documents = await storage.ListDocumentsAsync();
        Assert.Empty(documents);
    }

    [Fact]
    public async Task File_hash_service_generates_sha256_for_file()
    {
        using var workspace = TestWorkspace.Create();
        Directory.CreateDirectory(workspace.RootPath);
        var filePath = Path.Combine(workspace.RootPath, "hash-me.pdf");
        await File.WriteAllTextAsync(filePath, "hash input");

        var hash = await new Sha256FileHashService().ComputeSha256Async(filePath);

        await using var stream = File.OpenRead(filePath);
        var expectedHash = Convert.ToHexString(await SHA256.HashDataAsync(stream));
        Assert.Equal(expectedHash, hash);
    }

    [Fact]
    public async Task Metadata_persists_across_storage_instances()
    {
        using var workspace = TestWorkspace.Create();
        var document = new VeteranDocument
        {
            OriginalFileName = "record.pdf",
            LocalFilePath = Path.Combine(workspace.Options.ImportsDirectoryPath, "record.pdf"),
            Sha256Hash = "ABC123"
        };

        var firstStorage = new JsonLocalStorageService(workspace.Options);
        await firstStorage.SaveDocumentAsync(document);

        var secondStorage = new JsonLocalStorageService(workspace.Options);
        var savedDocument = await secondStorage.GetDocumentAsync(document.Id);

        Assert.NotNull(savedDocument);
        Assert.Equal(document.OriginalFileName, savedDocument.OriginalFileName);
        Assert.Equal(document.Sha256Hash, savedDocument.Sha256Hash);
    }

    [Fact]
    public async Task Metadata_round_trips_pages_and_text_blocks()
    {
        using var workspace = TestWorkspace.Create();
        var sourcePdf = workspace.WriteMinimalPdf("round-trip.pdf");
        var storage = new JsonLocalStorageService(workspace.Options);
        var importer = CreateImporter(storage, workspace.Options);

        var importedDocument = await importer.ImportAsync(sourcePdf);
        var reloadedStorage = new JsonLocalStorageService(workspace.Options);
        var savedDocument = await reloadedStorage.GetDocumentAsync(importedDocument.Id);

        Assert.NotNull(savedDocument);
        var savedBlock = Assert.Single(savedDocument.Pages.SelectMany(page => page.TextBlocks));
        Assert.Contains("Local import test", savedBlock.Text, StringComparison.Ordinal);
        Assert.Contains(savedDocument.Pages, page => page.PageNumber == 1);
    }

    [Fact]
    public async Task List_documents_orders_newest_imports_first()
    {
        using var workspace = TestWorkspace.Create();
        var storage = new JsonLocalStorageService(workspace.Options);
        var olderDocument = new VeteranDocument
        {
            OriginalFileName = "older.pdf",
            LocalFilePath = Path.Combine(workspace.Options.ImportsDirectoryPath, "older.pdf"),
            Sha256Hash = "OLDER",
            ImportedAt = DateTimeOffset.UtcNow.AddDays(-1)
        };
        var newerDocument = new VeteranDocument
        {
            OriginalFileName = "newer.pdf",
            LocalFilePath = Path.Combine(workspace.Options.ImportsDirectoryPath, "newer.pdf"),
            Sha256Hash = "NEWER",
            ImportedAt = DateTimeOffset.UtcNow
        };

        await storage.SaveDocumentAsync(olderDocument);
        await storage.SaveDocumentAsync(newerDocument);

        var documents = await storage.ListDocumentsAsync();

        Assert.Equal(newerDocument.Id, documents[0].Id);
        Assert.Equal(olderDocument.Id, documents[1].Id);
    }

    [Fact]
    public async Task Pdf_text_extraction_reads_text_based_pdf()
    {
        using var workspace = TestWorkspace.Create();
        var sourcePdf = workspace.WriteMinimalPdf("text-based.pdf");
        var storage = new JsonLocalStorageService(workspace.Options);
        var importer = CreateImporter(storage, workspace.Options);

        var document = await importer.ImportAsync(sourcePdf);

        var extractedText = string.Join(Environment.NewLine, document.Pages.SelectMany(page => page.TextBlocks).Select(block => block.Text));
        Assert.Contains("Local import test", extractedText, StringComparison.Ordinal);
    }

    [Fact]
    public async Task Pdf_without_embedded_text_creates_page_metadata_without_ocr()
    {
        using var workspace = TestWorkspace.Create();
        var sourcePdf = workspace.WriteBlankPdf("blank.pdf");
        var storage = new JsonLocalStorageService(workspace.Options);
        var importer = CreateImporter(storage, workspace.Options);

        var document = await importer.ImportAsync(sourcePdf);

        var page = Assert.Single(document.Pages);
        Assert.Equal(1, page.PageNumber);
        Assert.Empty(page.TextBlocks);
    }

    [Fact]
    public async Task Duplicate_pdf_imports_create_separate_local_records_with_same_hash()
    {
        using var workspace = TestWorkspace.Create();
        var sourcePdf = workspace.WriteMinimalPdf("duplicate.pdf");
        var storage = new JsonLocalStorageService(workspace.Options);
        var importer = CreateImporter(storage, workspace.Options);

        var firstImport = await importer.ImportAsync(sourcePdf);
        var secondImport = await importer.ImportAsync(sourcePdf);

        var documents = await storage.ListDocumentsAsync();
        Assert.Equal(2, documents.Count);
        Assert.NotEqual(firstImport.Id, secondImport.Id);
        Assert.NotEqual(firstImport.LocalFilePath, secondImport.LocalFilePath);
        Assert.Equal(firstImport.Sha256Hash, secondImport.Sha256Hash);
    }

    private static PlaceholderDocumentImportService CreateImporter(JsonLocalStorageService storage, LocalWorkspaceOptions options)
    {
        return new PlaceholderDocumentImportService(
            storage,
            new PlaceholderTextExtractionService(),
            new Sha256FileHashService(),
            options);
    }

    private sealed class TestWorkspace : IDisposable
    {
        private TestWorkspace(string rootPath)
        {
            RootPath = rootPath;
            Options = new LocalWorkspaceOptions { WorkspaceRootPath = rootPath };
        }

        public string RootPath { get; }

        public LocalWorkspaceOptions Options { get; }

        public static TestWorkspace Create()
        {
            return new TestWorkspace(Path.Combine(Path.GetTempPath(), "vea-tests", Guid.NewGuid().ToString("N")));
        }

        public string WriteMinimalPdf(string fileName)
        {
            Directory.CreateDirectory(RootPath);
            var path = Path.Combine(RootPath, fileName);

            var builder = new PdfDocumentBuilder();
            var page = builder.AddPage(PageSize.A4);
            var font = builder.AddStandard14Font(Standard14Font.Helvetica);
            page.AddText("Local import test", 12, new PdfPoint(25, 700), font);
            File.WriteAllBytes(path, builder.Build());

            return path;
        }

        public string WriteBlankPdf(string fileName)
        {
            Directory.CreateDirectory(RootPath);
            var path = Path.Combine(RootPath, fileName);

            var builder = new PdfDocumentBuilder();
            builder.AddPage(PageSize.A4);
            File.WriteAllBytes(path, builder.Build());

            return path;
        }

        public void Dispose()
        {
            if (Directory.Exists(RootPath))
            {
                Directory.Delete(RootPath, true);
            }
        }
    }
}
