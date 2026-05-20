using VeteranEvidenceAssist.Core.Enums;
using VeteranEvidenceAssist.Core.Models;
using VeteranEvidenceAssist.Documents.Services;

namespace VeteranEvidenceAssist.Tests;

public sealed class LocalOcrServiceTests
{
    [Fact]
    public async Task No_op_ocr_engine_returns_no_blocks_without_modifying_page()
    {
        var page = new DocumentPage
        {
            PageNumber = 1,
            VeteranDocumentId = Guid.NewGuid()
        };

        var service = new LocalOcrService(new NoOpLocalOcrEngine());

        var blocks = await service.ExtractPageTextAsync(page);

        Assert.Empty(blocks);
        Assert.Empty(page.TextBlocks);
    }

    [Fact]
    public async Task Ocr_service_normalizes_engine_blocks_to_local_ocr_with_page_provenance()
    {
        var page = new DocumentPage
        {
            PageNumber = 2,
            VeteranDocumentId = Guid.NewGuid()
        };
        var service = new LocalOcrService(new FakeOcrEngine([
            new ExtractedTextBlock
            {
                DocumentPageId = Guid.NewGuid(),
                Text = "  OCR text  ",
                ExtractionMethod = TextExtractionMethod.Unknown,
                Confidence = 1.5,
                BoundingBoxJson = """{"left":1,"top":2,"width":3,"height":4}"""
            },
            new ExtractedTextBlock
            {
                Text = "   ",
                Confidence = 0.5
            }
        ]));

        var blocks = await service.ExtractPageTextAsync(page);

        var block = Assert.Single(blocks);
        Assert.Equal(page.Id, block.DocumentPageId);
        Assert.Equal("OCR text", block.Text);
        Assert.Equal(TextExtractionMethod.LocalOcr, block.ExtractionMethod);
        Assert.Equal(1.0, block.Confidence);
        Assert.Equal("""{"left":1,"top":2,"width":3,"height":4}""", block.BoundingBoxJson);
    }

    [Fact]
    public async Task Ocr_service_honors_cancellation()
    {
        using var cancellation = new CancellationTokenSource();
        await cancellation.CancelAsync();
        var service = new LocalOcrService(new CancellationAwareOcrEngine());

        await Assert.ThrowsAsync<OperationCanceledException>(
            () => service.ExtractPageTextAsync(new DocumentPage { PageNumber = 1 }, cancellation.Token));
    }

    private sealed class FakeOcrEngine : ILocalOcrEngine
    {
        private readonly IReadOnlyList<ExtractedTextBlock> _blocks;

        public FakeOcrEngine(IReadOnlyList<ExtractedTextBlock> blocks)
        {
            _blocks = blocks;
        }

        public Task<IReadOnlyList<ExtractedTextBlock>> RecognizePageAsync(DocumentPage page, CancellationToken cancellationToken = default)
        {
            return Task.FromResult(_blocks);
        }
    }

    private sealed class CancellationAwareOcrEngine : ILocalOcrEngine
    {
        public Task<IReadOnlyList<ExtractedTextBlock>> RecognizePageAsync(DocumentPage page, CancellationToken cancellationToken = default)
        {
            cancellationToken.ThrowIfCancellationRequested();
            IReadOnlyList<ExtractedTextBlock> blocks = [];
            return Task.FromResult(blocks);
        }
    }
}
