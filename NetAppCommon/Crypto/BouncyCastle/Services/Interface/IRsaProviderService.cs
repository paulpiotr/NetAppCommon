#region using

using System.Threading.Tasks;

#endregion

namespace NetAppCommon.Crypto.BouncyCastle.Services.Interface;

public interface IRsaProviderService
{
    public RsaProviderService Initialize(int dwKeySize = 2048);

    public Task<RsaProviderService> InitializeAsync(int dwKeySize = 2048);

    public string GetAsymmetricPrivateKeyAsString(string asymmetricPrivateKeyFilePath = null);

    public Task<string> GetAsymmetricPrivateKeyAsStringAsync(string asymmetricPrivateKeyFilePath = null);

    public string GetAsymmetricPublicKeyAsString(string asymmetricPublicKeyFilePath = null);

    public Task<string> GetAsymmetricPublicKeyAsStringAsync(string asymmetricPublicKeyFilePath = null);
}
