using VeteranEvidenceAssist.Core.Enums;
using VeteranEvidenceAssist.Core.Models;
using VeteranEvidenceAssist.Core.ValueObjects;

namespace VeteranEvidenceAssist.Tests;

public sealed class DomainModelTests
{
    [Fact]
    public void New_document_defaults_to_sensitive_local_record()
    {
        var document = new VeteranDocument();

        Assert.NotEqual(Guid.Empty, document.Id);
        Assert.True(document.ContainsSensitiveInformation);
        Assert.Equal(DocumentType.Unknown, document.DocumentType);
    }

    [Fact]
    public void Evidence_item_preserves_source_reference()
    {
        var source = new SourceReference(Guid.NewGuid(), Guid.NewGuid(), 2, "Service treatment record page 2");

        var item = new EvidenceItem
        {
            Category = EvidenceCategory.Treatment,
            Summary = "User-reviewed factual treatment entry.",
            SourceReference = source,
            Confidence = 0.9
        };

        Assert.Equal(source, item.SourceReference);
        Assert.False(item.IsAiGeneratedInterpretation);
    }
}
