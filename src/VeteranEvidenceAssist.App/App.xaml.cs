using VeteranEvidenceAssist.App.Services;

namespace VeteranEvidenceAssist.App;

public partial class App : Application
{
    public App()
    {
        InitializeComponent();
        AppearanceSettingsService.ApplySavedTheme(this);
    }

    protected override Window CreateWindow(IActivationState? activationState)
    {
        return new Window(new AppShell());
    }
}
