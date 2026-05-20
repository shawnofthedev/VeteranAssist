using System.Text.Json;
using UglyToad.PdfPig;
using UglyToad.PdfPig.DocumentLayoutAnalysis.TextExtractor;
using VeteranEvidenceAssist.Core.Enums;
using VeteranEvidenceAssist.Core.Interfaces;
using VeteranEvidenceAssist.Core.Models;

namespace VeteranEvidenceAssist.Documents.Services;

public sealed class PdfPigTextExtractionService : ITextExtractionService
{
    public Task<IReadOnlyList<ExtractedTextBlock>> ExtractTextAsync(VeteranDocument document, CancellationToken cancellationToken = default)
    {
        if (!string.Equals(Path.GetExtension(document.LocalFilePath), ".pdf", StringComparison.OrdinalIgnoreCase))
        {
            throw new NotSupportedException("Phase 1 text extraction supports PDF files only.");
        }

        var blocks = new List<ExtractedTextBlock>();

        using var pdf = PdfDocument.Open(document.LocalFilePath);
        foreach (var pdfPage in pdf.GetPages())
        {
            cancellationToken.ThrowIfCancellationRequested();

            var page = new DocumentPage
            {
                VeteranDocumentId = document.Id,
                PageNumber = pdfPage.Number,
                WidthPixels = Convert.ToInt32(Math.Round(pdfPage.Width)),
                HeightPixels = Convert.ToInt32(Math.Round(pdfPage.Height))
            };

            document.Pages.Add(page);

            var text = ContentOrderTextExtractor.GetText(pdfPage).Trim();
            if (string.IsNullOrWhiteSpace(text))
            {
                continue;
            }

            blocks.Add(new ExtractedTextBlock
            {
                DocumentPageId = page.Id,
                Text = text,
                ExtractionMethod = TextExtractionMethod.EmbeddedText,
                Confidence = 1.0,
                BoundingBoxJson = JsonSerializer.Serialize(new
                {
                    left = 0,
                    bottom = 0,
                    width = pdfPage.Width,
                    height = pdfPage.Height
                })
            });
        }

        return Task.FromResult<IReadOnlyList<ExtractedTextBlock>>(blocks);
    }

}
