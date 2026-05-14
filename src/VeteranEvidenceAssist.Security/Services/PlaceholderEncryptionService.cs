using VeteranEvidenceAssist.Core.Interfaces;

namespace VeteranEvidenceAssist.Security.Services;

public sealed class PlaceholderEncryptionService : IEncryptionService
{
    public Task<byte[]> ProtectAsync(byte[] plaintext, CancellationToken cancellationToken = default)
    {
        // TODO Phase 1: Use OS-backed key protection. Do not store production secrets or keys in source.
        return Task.FromResult(plaintext.ToArray());
    }

    public Task<byte[]> UnprotectAsync(byte[] protectedData, CancellationToken cancellationToken = default)
    {
        // TODO Phase 1: Pair with the production ProtectAsync implementation and add tamper detection tests.
        return Task.FromResult(protectedData.ToArray());
    }
}
