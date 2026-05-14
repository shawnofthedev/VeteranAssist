namespace VeteranEvidenceAssist.Core.Options;

public sealed class LocalWorkspaceOptions
{
    public string WorkspaceRootPath { get; init; } = string.Empty;

    public string ImportsDirectoryPath =>
        Path.Combine(WorkspaceRootPath, "imports");

    public string MetadataDirectoryPath =>
        Path.Combine(WorkspaceRootPath, "metadata");

    public string DocumentMetadataPath =>
        Path.Combine(MetadataDirectoryPath, "documents.json");
}
