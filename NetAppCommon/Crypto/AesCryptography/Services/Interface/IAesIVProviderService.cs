#region using

using System.Threading.Tasks;

#endregion

namespace NetAppCommon.Crypto.AesCryptography.Services.Interface;

public interface IAesIVProviderService
{
    public string Encrypt(string text, string salt);

    public Task<string> EncryptAsync(string text, string salt);

    public string Decpypt(string text, string salt);

    public Task<string> DecpyptAsync(string text, string salt);
}
