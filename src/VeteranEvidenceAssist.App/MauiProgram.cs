using Microsoft.Extensions.Logging;
using Microsoft.Extensions.DependencyInjection;
using VeteranEvidenceAssist.AI.Services;
using VeteranEvidenceAssist.Core.Interfaces;
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

        var databasePath = Path.Combine(FileSystem.AppDataDirectory, "veteran-evidence-assist.db");

        builder.Services.AddSingleton(new LocalStorageOptions { DatabasePath = databasePath });
        builder.Services.AddSingleton<IDocumentImportService, PlaceholderDocumentImportService>();
        builder.Services.AddSingleton<PlaceholderTextExtractionService>();
        builder.Services.AddSingleton<ITextExtractionService>(provider => provider.GetRequiredService<PlaceholderTextExtractionService>());
        builder.Services.AddSingleton<IOcrService>(provider => provider.GetRequiredService<PlaceholderTextExtractionService>());
        builder.Services.AddSingleton<IPiiDetectionService, PlaceholderPiiDetectionService>();
        builder.Services.AddSingleton<IRedactionService, PlaceholderRedactionService>();
        builder.Services.AddSingleton<IEvidenceExtractionService, PlaceholderEvidenceExtractionService>();
        builder.Services.AddSingleton<IPromptGenerationService, LocalPromptGenerationService>();
        builder.Services.AddSingleton<ILocalStorageService, InMemoryLocalStorageService>();
        builder.Services.AddSingleton<IAuditLogService, InMemoryAuditLogService>();
        builder.Services.AddSingleton<IEncryptionService, PlaceholderEncryptionService>();

        return builder.Build();
    }
}
