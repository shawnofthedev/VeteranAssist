namespace VeteranEvidenceAssist.Core.Options;

public sealed class LocalWorkspaceOptions
{
    public string WorkspaceRootPath { get; init; } = string.Empty;

    public string DocumentsDirectoryPath =>
        Path.Combine(WorkspaceRootPath, "Documents");

    public string ImportsDirectoryPath =>
        DocumentsDirectoryPath;

    public string MetadataDirectoryPath =>
        Path.Combine(WorkspaceRootPath, "metadata");

    public string DocumentMetadataPath =>
        Path.Combine(MetadataDirectoryPath, "documents.json");
}
