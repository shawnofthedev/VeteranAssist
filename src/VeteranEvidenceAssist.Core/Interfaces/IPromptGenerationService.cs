using VeteranEvidenceAssist.Core.Models;

namespace VeteranEvidenceAssist.Core.Interfaces;

public interface IPromptGenerationService
{
    Task<PromptPacket> GeneratePacketAsync(
        PromptTemplate template,
        IReadOnlyList<EvidenceItem> evidenceItems,
        CancellationToken cancellationToken = default);
}
