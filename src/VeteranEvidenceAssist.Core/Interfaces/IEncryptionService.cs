namespace VeteranEvidenceAssist.Core.Interfaces;

public interface IEncryptionService
{
    Task<byte[]> ProtectAsync(byte[] plaintext, CancellationToken cancellationToken = default);

    Task<byte[]> UnprotectAsync(byte[] protectedData, CancellationToken cancellationToken = default);
}
