using VeteranEvidenceAssist.Core.Interfaces;
using VeteranEvidenceAssist.Core.Options;
using VeteranEvidenceAssist.App.ViewModels;

namespace VeteranEvidenceAssist.App.Pages;

public partial class DocumentsPage : ContentPage
{
    private readonly IDocumentImportService _documentImportService;
    private readonly IDocumentRepository _documentRepository;
    private readonly IFileHashService _fileHashService;
    private readonly LocalWorkspaceOptions _workspaceOptions;

    public DocumentsPage()
    {
        InitializeComponent();

        var services = IPlatformApplication.Current?.Services
            ?? throw new InvalidOperationException("Application services are unavailable.");

        _documentImportService = services.GetRequiredService<IDocumentImportService>();
        _documentRepository = services.GetRequiredService<IDocumentRepository>();
        _fileHashService = services.GetRequiredService<IFileHashService>();
        _workspaceOptions = services.GetRequiredService<LocalWorkspaceOptions>();

        LoadWorkspaceLabels();
        ImportResultsCollection.ItemsSource = Array.Empty<ImportResultItem>();
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
            var importedCount = 0;
            var duplicateCount = 0;
            var duplicateNames = new List<string>();
            var importResults = new List<ImportResultItem>();

            foreach (var file in selectedFiles)
            {
                ImportStatusLabel.Text = $"Importing {file.FileName} locally...";

                var sourceHash = await _fileHashService.ComputeSha256Async(file.FullPath);
                var existingDocument = await _documentRepository.FindBySha256HashAsync(sourceHash);
                var isDuplicate = existingDocument is not null && File.Exists(existingDocument.StoredPath);

                var importedDocument = await _documentImportService.ImportAsync(file.FullPath);

                if (isDuplicate)
                {
                    duplicateCount++;
                    duplicateNames.Add(file.FileName);
                    importResults.Add(ImportResultItem.Duplicate(file.FileName, importedDocument));
                }
                else
                {
                    importedCount++;
                    importResults.Add(ImportResultItem.Imported(file.FileName, importedDocument));
                }

                if (isDuplicate &&
                    !string.Equals(importedDocument.OriginalFileName, file.FileName, StringComparison.OrdinalIgnoreCase))
                {
                    ImportStatusLabel.Text = $"{file.FileName} is already imported as {importedDocument.OriginalFileName}. Reused the existing local record.";
                }

                processedCount++;
            }

            ImportStatusLabel.Text = BuildImportSummary(processedCount, importedCount, duplicateCount, duplicateNames);
            ImportResultsCollection.ItemsSource = importResults;
            await RefreshDocumentsAsync();
        }
        catch (Exception ex) when (ex is ArgumentException or FileNotFoundException or NotSupportedException or InvalidDataException)
        {
            ImportStatusLabel.Text = ex.Message;
            ImportResultsCollection.ItemsSource = new[]
            {
                ImportResultItem.Failed("Import stopped", ex.Message)
            };
        }
        finally
        {
            ChoosePdfsButton.IsEnabled = true;
        }
    }

    private async void OnCopyWorkspacePathClicked(object sender, EventArgs e)
    {
        await Clipboard.Default.SetTextAsync(_workspaceOptions.WorkspaceRootPath);
        ImportStatusLabel.Text = "Copied the local workspace path. No document contents were copied.";
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
            await Shell.Current.GoToAsync(AppShell.DocumentReviewAbsoluteRoute, new Dictionary<string, object>
            {
                [AppShell.DocumentIdQueryKey] = selectedDocument.Id.ToString()
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

    private void LoadWorkspaceLabels()
    {
        WorkspacePathLabel.Text = $"Workspace: {_workspaceOptions.WorkspaceRootPath}";
        DocumentsPathLabel.Text = $"Document copies: {_workspaceOptions.DocumentsDirectoryPath}";
        MetadataPathLabel.Text = $"Metadata index: {_workspaceOptions.DocumentMetadataPath}";
    }

    private static string BuildImportSummary(
        int processedCount,
        int importedCount,
        int duplicateCount,
        IReadOnlyCollection<string> duplicateNames)
    {
        if (duplicateCount == 0)
        {
            return $"Imported {importedCount} PDF file(s) into the local workspace. No files were uploaded.";
        }

        var duplicateSummary = duplicateNames.Count <= 3
            ? string.Join(", ", duplicateNames)
            : $"{string.Join(", ", duplicateNames.Take(3))}, and {duplicateNames.Count - 3} more";

        return $"Processed {processedCount} PDF file(s): {importedCount} new, {duplicateCount} already imported. " +
               $"Reused existing local records for {duplicateSummary}. No files were uploaded.";
    }

}
