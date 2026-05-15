using VeteranEvidenceAssist.Core.Interfaces;
using VeteranEvidenceAssist.Core.Models;
using VeteranEvidenceAssist.App.ViewModels;

namespace VeteranEvidenceAssist.App.Pages;

public partial class DocumentReviewPage : ContentPage, IQueryAttributable
{
    private readonly IDocumentRepository _documentRepository;
    private Guid? _documentId;

    public DocumentReviewPage()
    {
        InitializeComponent();

        var services = IPlatformApplication.Current?.Services
            ?? throw new InvalidOperationException("Application services are unavailable.");

        _documentRepository = services.GetRequiredService<IDocumentRepository>();
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

        var detail = DocumentDetailViewModel.FromDocument(document);

        DocumentTitleLabel.Text = detail.FileName;
        DocumentMetaLabel.Text = $"Imported {detail.ImportDate} | SHA-256 {detail.Hash}";
        TextStatusLabel.Text = detail.ExtractionStatus;
        ExtractionMethodLabel.Text = $"Extraction: {detail.ExtractionStatus} | OCR required: {detail.OcrRequiredDisplay}";
        ImportedAtUtcLabel.Text = $"Imported UTC: {detail.ImportedAtUtc}";
        Sha256Label.Text = $"SHA-256: {detail.Hash}";
        ExtractionStatusLabel.Text = $"Extraction status: {detail.ExtractionStatus}";
        OcrRequiredLabel.Text = $"OCR required: {detail.OcrRequiredDisplay}";
        DocumentTypeLabel.Text = $"Document type: {detail.DocumentType}";
        RedactionStatusLabel.Text = $"Redaction status: {detail.RedactionStatus}";
        OcrNeededWarning.IsVisible = detail.IsOcrNeeded;
        PageSlider.Maximum = Math.Max(1, detail.PageCount);
        PageSlider.Value = 1;
        ExtractedTextCollection.ItemsSource = detail.TextPreviewItems;
    }

    private async Task<VeteranDocument?> GetSelectedDocumentAsync()
    {
        if (_documentId is Guid documentId)
        {
            return await _documentRepository.GetDocumentAsync(documentId);
        }

        var documents = await _documentRepository.ListDocumentsAsync();
        return documents.FirstOrDefault();
    }

    private void ShowEmptyState()
    {
        DocumentTitleLabel.Text = "No document selected";
        DocumentMetaLabel.Text = "Import a PDF from the Documents page to review extracted text.";
        TextStatusLabel.Text = "No document";
        ExtractionMethodLabel.Text = "Extraction: none";
        ImportedAtUtcLabel.Text = "Imported UTC: -";
        Sha256Label.Text = "SHA-256: -";
        ExtractionStatusLabel.Text = "Extraction status: -";
        OcrRequiredLabel.Text = "OCR required: -";
        DocumentTypeLabel.Text = "Document type: -";
        RedactionStatusLabel.Text = "Redaction status: -";
        OcrNeededWarning.IsVisible = false;
        ExtractedTextCollection.ItemsSource = Array.Empty<ExtractedTextPreviewItem>();
        PageSlider.Maximum = 1;
        PageSlider.Value = 1;
    }
}
