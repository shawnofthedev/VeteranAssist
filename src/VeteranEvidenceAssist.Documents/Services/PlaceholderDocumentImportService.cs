using System.Security.Cryptography;
using VeteranEvidenceAssist.Core.Enums;
using VeteranEvidenceAssist.Core.Interfaces;
using VeteranEvidenceAssist.Core.Models;

namespace VeteranEvidenceAssist.Documents.Services;

public sealed class PlaceholderDocumentImportService : IDocumentImportService
{
    public async Task<VeteranDocument> ImportAsync(string filePath, CancellationToken cancellationToken = default)
    {
        await using var stream = File.OpenRead(filePath);
        var hash = await SHA256.HashDataAsync(stream, cancellationToken);

        return new VeteranDocument
        {
            OriginalFileName = Path.GetFileName(filePath),
            LocalFilePath = filePath,
            DocumentType = InferDocumentType(filePath),
            Sha256Hash = Convert.ToHexString(hash),
            ContainsSensitiveInformation = true
        };
    }

    private static DocumentType InferDocumentType(string filePath)
    {
        var fileName = Path.GetFileName(filePath);

        if (fileName.Contains("dd214", StringComparison.OrdinalIgnoreCase) ||
            fileName.Contains("dd-214", StringComparison.OrdinalIgnoreCase))
        {
            return DocumentType.DD214;
        }

        if (string.Equals(Path.GetExtension(filePath), ".pdf", StringComparison.OrdinalIgnoreCase))
        {
            return DocumentType.ScannedPdf;
        }

        return DocumentType.Unknown;
    }
}
