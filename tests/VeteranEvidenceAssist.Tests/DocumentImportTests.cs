using System.Security.Cryptography;
using UglyToad.PdfPig.Content;
using UglyToad.PdfPig.Core;
using UglyToad.PdfPig.Fonts.Standard14Fonts;
using UglyToad.PdfPig.Writer;
using VeteranEvidenceAssist.Core.Models;
using VeteranEvidenceAssist.Core.Options;
using VeteranEvidenceAssist.Documents.Services;
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
        var importer = new PlaceholderDocumentImportService(storage, new PlaceholderTextExtractionService(), workspace.Options);

        var document = await importer.ImportAsync(sourcePdf);

        Assert.NotEqual(sourcePdf, document.LocalFilePath);
        Assert.True(File.Exists(document.LocalFilePath));
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
        var importer = new PlaceholderDocumentImportService(
            new JsonLocalStorageService(workspace.Options),
            new PlaceholderTextExtractionService(),
            workspace.Options);

        await Assert.ThrowsAsync<NotSupportedException>(() => importer.ImportAsync(textFile));
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

        public void Dispose()
        {
            if (Directory.Exists(RootPath))
            {
                Directory.Delete(RootPath, true);
            }
        }
    }
}
