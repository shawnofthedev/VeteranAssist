namespace VeteranEvidenceAssist.Core.Models;

public sealed class DocumentPage
{
    public Guid Id { get; init; } = Guid.NewGuid();
    public Guid VeteranDocumentId { get; set; }
    public int PageNumber { get; set; }
    public int? WidthPixels { get; set; }
    public int? HeightPixels { get; set; }
    public List<ExtractedTextBlock> TextBlocks { get; init; } = [];
}
