using VeteranEvidenceAssist.Core.Enums;
using VeteranEvidenceAssist.Core.Models;

namespace VeteranEvidenceAssist.App.ViewModels;

public sealed record DocumentListItem(
    Guid Id,
    string OriginalFileName,
    int PageCount,
    string TextStatus,
    string ShortHash,
    string ImportedAtDisplay)
{
    private const int ShortHashLength = 12;

    public static DocumentListItem FromDocument(VeteranDocument document)
    {
        var textBlockCount = document.Pages.Sum(page => page.TextBlocks.Count);

        return new DocumentListItem(
            document.Id,
            document.OriginalFileName,
            document.Pages.Count,
            ToDisplayStatus(document, textBlockCount),
            ToShortHash(document.Sha256Hash),
            document.ImportedAt.ToLocalTime().ToString("g"));
    }

    private static string ToDisplayStatus(VeteranDocument document, int textBlockCount)
    {
        return document.ExtractionStatus switch
        {
            DocumentExtractionStatus.EmbeddedTextExtracted => "Embedded",
            DocumentExtractionStatus.OcrNeeded => "OCR needed",
            DocumentExtractionStatus.NoTextFound => "No text",
            DocumentExtractionStatus.ExtractionFailed => "Failed",
            _ => textBlockCount > 0 ? "Embedded" : "Unknown"
        };
    }

    private static string ToShortHash(string sha256Hash)
    {
        return sha256Hash.Length > ShortHashLength
            ? sha256Hash[..ShortHashLength]
            : sha256Hash;
    }
}
