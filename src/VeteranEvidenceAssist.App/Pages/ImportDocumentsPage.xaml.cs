using VeteranEvidenceAssist.Core.Interfaces;

namespace VeteranEvidenceAssist.App.Pages;

public partial class ImportDocumentsPage : ContentPage
{
    private readonly IDocumentImportService _documentImportService;
    private readonly ILocalStorageService _localStorageService;

    public ImportDocumentsPage()
    {
        InitializeComponent();

        var services = IPlatformApplication.Current?.Services
            ?? throw new InvalidOperationException("Application services are unavailable.");

        _documentImportService = services.GetRequiredService<IDocumentImportService>();
        _localStorageService = services.GetRequiredService<ILocalStorageService>();
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();
        await RefreshDocumentsAsync();
    }

    private async void OnChoosePdfClicked(object sender, EventArgs e)
    {
        try
        {
            ChooseFilesButton.IsEnabled = false;
            StatusLabel.Text = "Waiting for PDF selection...";

            var result = await FilePicker.Default.PickAsync(new PickOptions
            {
                PickerTitle = "Choose a PDF to import",
                FileTypes = FilePickerFileType.Pdf
            });

            if (result is null)
            {
                StatusLabel.Text = "Import canceled.";
                return;
            }

            StatusLabel.Text = "Importing locally...";
            var document = await _documentImportService.ImportAsync(result.FullPath);
            StatusLabel.Text = $"Imported {document.OriginalFileName}. Extracted embedded text from {document.Pages.Count} page(s).";
            await RefreshDocumentsAsync();
        }
        catch (Exception ex) when (ex is ArgumentException or FileNotFoundException or NotSupportedException or InvalidDataException)
        {
            StatusLabel.Text = ex.Message;
        }
        finally
        {
            ChooseFilesButton.IsEnabled = true;
        }
    }

    private async Task RefreshDocumentsAsync()
    {
        DocumentsView.ItemsSource = await _localStorageService.ListDocumentsAsync();
    }
}
