using VeteranEvidenceAssist.Core.Models;

namespace VeteranEvidenceAssist.App.ViewModels;

public sealed record ImportResultItem(string Status, string FileName, string Detail)
{
    private const int ShortHashLength = 12;

    public static ImportResultItem Imported(string selectedFileName, VeteranDocument document)
    {
        return new ImportResultItem(
            "New",
            selectedFileName,
            $"Copied into local workspace as {document.OriginalFileName}. Hash {ToShortHash(document.Sha256Hash)}.");
    }

    public static ImportResultItem Duplicate(string selectedFileName, VeteranDocument document)
    {
        return new ImportResultItem(
            "Already imported",
            selectedFileName,
            $"Reused existing local record {document.OriginalFileName}. Hash {ToShortHash(document.Sha256Hash)}.");
    }

    public static ImportResultItem Failed(string selectedFileName, string detail)
    {
        return new ImportResultItem("Needs attention", selectedFileName, detail);
    }

    private static string ToShortHash(string sha256Hash)
    {
        return sha256Hash.Length > ShortHashLength
            ? sha256Hash[..ShortHashLength]
            : sha256Hash;
    }
}
