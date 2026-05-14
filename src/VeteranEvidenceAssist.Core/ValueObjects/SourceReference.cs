namespace VeteranEvidenceAssist.Core.ValueObjects;

public sealed record SourceReference(
    Guid DocumentId,
    Guid? PageId,
    int? PageNumber,
    string LocationDescription);
