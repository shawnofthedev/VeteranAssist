using VeteranEvidenceAssist.App.Services;
using VeteranEvidenceAssist.Core.Options;

namespace VeteranEvidenceAssist.App.Pages;

public partial class SettingsPage : ContentPage
{
    private readonly LocalWorkspaceOptions _workspaceOptions;

    public SettingsPage()
    {
        InitializeComponent();

        var services = IPlatformApplication.Current?.Services
            ?? throw new InvalidOperationException("Application services are unavailable.");

        _workspaceOptions = services.GetRequiredService<LocalWorkspaceOptions>();

        LoadWorkspaceSettings();
        LoadAppearanceSettings();
    }

    private void LoadWorkspaceSettings()
    {
        WorkspacePathEntry.Text = _workspaceOptions.WorkspaceRootPath;
        DocumentsPathLabel.Text = $"Document copies: {_workspaceOptions.DocumentsDirectoryPath}";
        MetadataPathLabel.Text = $"Metadata index: {_workspaceOptions.DocumentMetadataPath}";
    }

    private void LoadAppearanceSettings()
    {
        AppearancePicker.ItemsSource = AppearanceSettingsService.DisplayNames;
        AppearancePicker.SelectedItem = AppearanceSettingsService.ToDisplayName(AppearanceSettingsService.GetSavedMode());
    }

    private void OnAppearanceSelectionChanged(object? sender, EventArgs e)
    {
        var selectedMode = AppearanceSettingsService.FromDisplayName(AppearancePicker.SelectedItem as string);
        AppearanceSettingsService.SaveAndApply(selectedMode);
    }
}
