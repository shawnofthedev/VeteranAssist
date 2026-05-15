using VeteranEvidenceAssist.Core.Enums;
using VeteranEvidenceAssist.Core.Models;

namespace VeteranEvidenceAssist.App.ViewModels;

public sealed class DocumentDetailViewModel
{
    public string FileName { get; init; } = string.Empty;
    public string ImportDate { get; init; } = string.Empty;
    public string ImportedAtUtc { get; init; } = string.Empty;
    public string Hash { get; init; } = string.Empty;
    public string ShortHash { get; init; } = string.Empty;
    public int PageCount { get; init; }
    public string ExtractionStatus { get; init; } = string.Empty;
    public bool IsOcrNeeded { get; init; }
    public string OcrRequiredDisplay => IsOcrNeeded ? "Yes" : "No";
    public string DocumentType { get; init; } = string.Empty;
    public string RedactionStatus { get; init; } = string.Empty;
    public string StoredPath { get; init; } = string.Empty;
    public IReadOnlyList<ExtractedTextPreviewItem> TextPreviewItems { get; init; } = [];

    public static DocumentDetailViewModel FromDocument(VeteranDocument document)
    {
        var textPreviewItems = document.Pages
            .OrderBy(page => page.PageNumber)
            .SelectMany(page => page.TextBlocks.Select(block => ExtractedTextPreviewItem.FromBlock(page, block)))
            .ToList();

        return new DocumentDetailViewModel
        {
            FileName = document.OriginalFileName,
            ImportDate = document.ImportedAt.ToLocalTime().ToString("g"),
            ImportedAtUtc = document.ImportedAtUtc.ToString("u"),
            Hash = document.Sha256Hash,
            ShortHash = CreateShortHash(document.Sha256Hash),
            PageCount = document.Pages.Count,
            ExtractionStatus = ToStatusLabel(document),
            IsOcrNeeded = document.RequiresOcr || document.ExtractionStatus == DocumentExtractionStatus.OcrNeeded,
            DocumentType = document.DocumentType.ToString(),
            RedactionStatus = document.RedactionStatus,
            StoredPath = document.StoredPath,
            TextPreviewItems = textPreviewItems
        };
    }

    private static string CreateShortHash(string hash)
    {
        return hash.Length > 12 ? hash[..12] : hash;
    }

    private static string ToStatusLabel(VeteranDocument document)
    {
        return document.ExtractionStatus switch
        {
            DocumentExtractionStatus.EmbeddedTextExtracted => "Embedded text found",
            DocumentExtractionStatus.OcrNeeded => "OCR needed",
            DocumentExtractionStatus.NoTextFound => "No text found",
            DocumentExtractionStatus.ExtractionFailed => "Extraction failed",
            _ => document.Pages.Any(page => page.TextBlocks.Count > 0) ? "Embedded text found" : "Unknown"
        };
    }
}

public sealed record ExtractedTextPreviewItem(string PageLabel, string Text)
{
    public static ExtractedTextPreviewItem FromBlock(DocumentPage page, ExtractedTextBlock block)
    {
        var method = block.ExtractionMethod == TextExtractionMethod.EmbeddedText
            ? "embedded text"
            : block.ExtractionMethod.ToString();

        return new ExtractedTextPreviewItem(
            $"Page {page.PageNumber} | {method} | confidence {block.Confidence:P0}",
            block.Text);
    }
}
