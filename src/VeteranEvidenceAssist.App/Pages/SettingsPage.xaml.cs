using VeteranEvidenceAssist.App.Services;

namespace VeteranEvidenceAssist.App.Pages;

public partial class SettingsPage : ContentPage
{
    public SettingsPage()
    {
        InitializeComponent();
        LoadAppearanceSettings();
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
