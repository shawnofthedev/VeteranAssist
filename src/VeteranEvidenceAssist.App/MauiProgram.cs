using Microsoft.Extensions.Logging;
using Microsoft.Extensions.DependencyInjection;
using VeteranEvidenceAssist.AI.Services;
using VeteranEvidenceAssist.Core.Interfaces;
using VeteranEvidenceAssist.Core.Options;
using VeteranEvidenceAssist.Documents.Services;
using VeteranEvidenceAssist.Redaction.Services;
using VeteranEvidenceAssist.Security.Services;
using VeteranEvidenceAssist.Storage.Data;
using VeteranEvidenceAssist.Storage.Repositories;

namespace VeteranEvidenceAssist.App;

public static class MauiProgram
{
    public static MauiApp CreateMauiApp()
    {
        var builder = MauiApp.CreateBuilder();
        builder
            .UseMauiApp<App>()
            .ConfigureFonts(fonts =>
            {
                fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
                fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
            });

#if DEBUG
        builder.Logging.AddDebug();
#endif

        var workspacePath = FileSystem.AppDataDirectory;
        var databasePath = Path.Combine(workspacePath, "veteran-evidence-assist.db");

        builder.Services.AddSingleton(new LocalWorkspaceOptions { WorkspaceRootPath = workspacePath });
        builder.Services.AddSingleton(new LocalStorageOptions { DatabasePath = databasePath });
        builder.Services.AddSingleton<IDocumentImportService, LocalDocumentImportService>();
        builder.Services.AddSingleton<PlaceholderTextExtractionService>();
        builder.Services.AddSingleton<ITextExtractionService>(provider => provider.GetRequiredService<PlaceholderTextExtractionService>());
        builder.Services.AddSingleton<IOcrService>(provider => provider.GetRequiredService<PlaceholderTextExtractionService>());
        builder.Services.AddSingleton<IPiiDetectionService, PlaceholderPiiDetectionService>();
        builder.Services.AddSingleton<IRedactionService, PlaceholderRedactionService>();
        builder.Services.AddSingleton<IEvidenceExtractionService, PlaceholderEvidenceExtractionService>();
        builder.Services.AddSingleton<IPromptGenerationService, LocalPromptGenerationService>();
        builder.Services.AddSingleton<ILocalStorageService, JsonLocalStorageService>();
        builder.Services.AddSingleton<IDocumentRepository>(provider => provider.GetRequiredService<ILocalStorageService>());
        builder.Services.AddSingleton<IAuditLogService, InMemoryAuditLogService>();
        builder.Services.AddSingleton<IEncryptionService, PlaceholderEncryptionService>();
        builder.Services.AddSingleton<IFileHashService, Sha256FileHashService>();

        return builder.Build();
    }
}
