using VeteranEvidenceAssist.Core.Enums;
using VeteranEvidenceAssist.Core.Interfaces;
using VeteranEvidenceAssist.Core.Models;

namespace VeteranEvidenceAssist.App.Pages;

public partial class DocumentReviewPage : ContentPage, IQueryAttributable
{
    private readonly ILocalStorageService _localStorageService;
    private Guid? _documentId;

    public DocumentReviewPage()
    {
        InitializeComponent();

        var services = IPlatformApplication.Current?.Services
            ?? throw new InvalidOperationException("Application services are unavailable.");

        _localStorageService = services.GetRequiredService<ILocalStorageService>();
    }

    public void ApplyQueryAttributes(IDictionary<string, object> query)
    {
        if (query.TryGetValue("documentId", out var value) &&
            Guid.TryParse(value?.ToString(), out var documentId))
        {
            _documentId = documentId;
        }
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();
        await LoadDocumentAsync();
    }

    private async Task LoadDocumentAsync()
    {
        var document = await GetSelectedDocumentAsync();
        if (document is null)
        {
            ShowEmptyState();
            return;
        }

        var textBlocks = document.Pages
            .OrderBy(page => page.PageNumber)
            .SelectMany(page => page.TextBlocks.Select(block => ExtractedTextListItem.FromBlock(page, block)))
            .ToList();

        DocumentTitleLabel.Text = document.OriginalFileName;
        DocumentMetaLabel.Text = $"Imported {document.ImportedAt.ToLocalTime():g} | {document.Pages.Count} page(s) | SHA-256 {ShortHash(document.Sha256Hash)}";
        TextStatusLabel.Text = textBlocks.Count > 0 ? "Embedded text found" : "No embedded text";
        ExtractionMethodLabel.Text = textBlocks.Count > 0 ? "Extraction: embedded PDF text" : "Extraction: none";
        PageSlider.Maximum = Math.Max(1, document.Pages.Count);
        PageSlider.Value = 1;
        ExtractedTextCollection.ItemsSource = textBlocks;
    }

    private async Task<VeteranDocument?> GetSelectedDocumentAsync()
    {
        if (_documentId is Guid documentId)
        {
            return await _localStorageService.GetDocumentAsync(documentId);
        }

        var documents = await _localStorageService.ListDocumentsAsync();
        return documents.FirstOrDefault();
    }

    private void ShowEmptyState()
    {
        DocumentTitleLabel.Text = "No document selected";
        DocumentMetaLabel.Text = "Import a PDF from the Documents page to review extracted text.";
        TextStatusLabel.Text = "No document";
        ExtractionMethodLabel.Text = "Extraction: none";
        ExtractedTextCollection.ItemsSource = Array.Empty<ExtractedTextListItem>();
        PageSlider.Maximum = 1;
        PageSlider.Value = 1;
    }

    private static string ShortHash(string hash)
    {
        return hash.Length > 12 ? hash[..12] : hash;
    }

    private sealed record ExtractedTextListItem(string PageLabel, string Text)
    {
        public static ExtractedTextListItem FromBlock(DocumentPage page, ExtractedTextBlock block)
        {
            var method = block.ExtractionMethod == TextExtractionMethod.EmbeddedText
                ? "embedded text"
                : block.ExtractionMethod.ToString();

            return new ExtractedTextListItem(
                $"Page {page.PageNumber} | {method} | confidence {block.Confidence:P0}",
                block.Text);
        }
    }
}
