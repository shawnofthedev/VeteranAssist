using VeteranEvidenceAssist.Core.Interfaces;
using VeteranEvidenceAssist.Core.Models;
using VeteranEvidenceAssist.Core.Enums;

namespace VeteranEvidenceAssist.App.Pages;

public partial class DocumentsPage : ContentPage
{
    private readonly IDocumentImportService _documentImportService;
    private readonly IDocumentRepository _documentRepository;

    public DocumentsPage()
    {
        InitializeComponent();

        var services = IPlatformApplication.Current?.Services
            ?? throw new InvalidOperationException("Application services are unavailable.");

        _documentImportService = services.GetRequiredService<IDocumentImportService>();
        _documentRepository = services.GetRequiredService<IDocumentRepository>();
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();
        await RefreshDocumentsAsync();
    }

    private async void OnChoosePdfsClicked(object sender, EventArgs e)
    {
        try
        {
            ChoosePdfsButton.IsEnabled = false;
            ImportStatusLabel.Text = "Waiting for PDF selection...";

            var results = await FilePicker.Default.PickMultipleAsync(new PickOptions
            {
                PickerTitle = "Choose PDFs to import",
                FileTypes = FilePickerFileType.Pdf
            });

            var selectedFiles = results?.ToList() ?? [];
            if (selectedFiles.Count == 0)
            {
                ImportStatusLabel.Text = "Import canceled.";
                return;
            }

            var processedCount = 0;
            foreach (var file in selectedFiles)
            {
                ImportStatusLabel.Text = $"Importing {file.FileName} locally...";
                var importedDocument = await _documentImportService.ImportAsync(file.FullPath);
                if (!string.Equals(importedDocument.OriginalFileName, file.FileName, StringComparison.OrdinalIgnoreCase))
                {
                    ImportStatusLabel.Text = $"{file.FileName} is already imported as {importedDocument.OriginalFileName}. Reused the existing local record.";
                }

                processedCount++;
            }

            ImportStatusLabel.Text = $"Processed {processedCount} PDF file(s). Duplicates reuse existing local records. No files were uploaded.";
            await RefreshDocumentsAsync();
        }
        catch (Exception ex) when (ex is ArgumentException or FileNotFoundException or NotSupportedException or InvalidDataException)
        {
            ImportStatusLabel.Text = ex.Message;
        }
        finally
        {
            ChoosePdfsButton.IsEnabled = true;
        }
    }

    private async void OnRefreshClicked(object sender, EventArgs e)
    {
        await RefreshDocumentsAsync();
    }

    private async void OnDocumentSelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        if (e.CurrentSelection.FirstOrDefault() is not DocumentListItem selectedDocument)
        {
            return;
        }

        try
        {
            DocumentsCollection.SelectedItem = null;
            await Shell.Current.GoToAsync("//document-review", new Dictionary<string, object>
            {
                ["documentId"] = selectedDocument.Id.ToString()
            });
        }
        catch (Exception)
        {
            ImportStatusLabel.Text = "Could not open the document review page. The document remains safely imported locally.";
        }
    }

    private async Task RefreshDocumentsAsync()
    {
        var documents = await _documentRepository.ListDocumentsAsync();
        DocumentsCollection.ItemsSource = documents.Select(DocumentListItem.FromDocument).ToList();
    }

    private sealed record DocumentListItem(
        Guid Id,
        string OriginalFileName,
        int PageCount,
        string TextStatus,
        string ShortHash,
        string ImportedAtDisplay)
    {
        public static DocumentListItem FromDocument(VeteranDocument document)
        {
            var textBlockCount = document.Pages.Sum(page => page.TextBlocks.Count);
            var shortHash = document.Sha256Hash.Length > 12
                ? document.Sha256Hash[..12]
                : document.Sha256Hash;

            return new DocumentListItem(
                document.Id,
                document.OriginalFileName,
                document.Pages.Count,
                ToDisplayStatus(document, textBlockCount),
                shortHash,
                document.ImportedAt.ToLocalTime().ToString("g"));
        }

        private static string ToDisplayStatus(VeteranDocument document, int textBlockCount)
        {
            return document.ExtractionStatus switch
            {
                DocumentExtractionStatus.EmbeddedTextExtracted => "Embedded",
                DocumentExtractionStatus.OcrNeeded => "OCR needed",
                DocumentExtractionStatus.NoTextFound => "No text",
                DocumentExtractionStatus.ExtractionFailed => "Failed",
                _ => textBlockCount > 0 ? "Embedded" : "Unknown"
            };
        }
    }
}
