using System.Collections;

namespace VeteranEvidenceAssist.App.Services;

public enum AppearanceMode
{
    SystemDefault,
    Light,
    Dark
}

public static class AppearanceSettingsService
{
    public const string SystemDefaultDisplayName = "System Default";
    public const string LightDisplayName = "Light";
    public const string DarkDisplayName = "Dark";

    private const string AppearanceModePreferenceKey = "settings.appearance.mode";

    public static IList DisplayNames { get; } = new[]
    {
        SystemDefaultDisplayName,
        LightDisplayName,
        DarkDisplayName
    };

    public static AppearanceMode GetSavedMode()
    {
        var savedValue = Preferences.Default.Get(AppearanceModePreferenceKey, AppearanceMode.SystemDefault.ToString());

        return Enum.TryParse(savedValue, ignoreCase: true, out AppearanceMode mode)
            ? mode
            : AppearanceMode.SystemDefault;
    }

    public static void SaveAndApply(AppearanceMode mode)
    {
        Preferences.Default.Set(AppearanceModePreferenceKey, mode.ToString());
        Apply(mode, Application.Current);
    }

    public static void ApplySavedTheme()
    {
        Apply(GetSavedMode(), Application.Current);
    }

    public static void ApplySavedTheme(Application application)
    {
        Apply(GetSavedMode(), application);
    }

    public static AppTheme ToAppTheme(AppearanceMode mode)
    {
        return mode switch
        {
            AppearanceMode.Light => AppTheme.Light,
            AppearanceMode.Dark => AppTheme.Dark,
            _ => AppTheme.Unspecified
        };
    }

    public static AppearanceMode FromDisplayName(string? displayName)
    {
        return displayName switch
        {
            LightDisplayName => AppearanceMode.Light,
            DarkDisplayName => AppearanceMode.Dark,
            _ => AppearanceMode.SystemDefault
        };
    }

    public static string ToDisplayName(AppearanceMode mode)
    {
        return mode switch
        {
            AppearanceMode.Light => LightDisplayName,
            AppearanceMode.Dark => DarkDisplayName,
            _ => SystemDefaultDisplayName
        };
    }

    private static void Apply(AppearanceMode mode, Application? application)
    {
        if (application is not null)
        {
            application.UserAppTheme = ToAppTheme(mode);
        }
    }
}
