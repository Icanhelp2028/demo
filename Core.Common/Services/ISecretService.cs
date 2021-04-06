namespace Core.Common.Services
{
    /// <summary>
    /// 加密类
    /// </summary>
    public interface ISecretService
    {
        string GenerateIV();
        string Encrypt(string plain, string iv);
        string Decrypt(string encrypt, string iv);
        string Hash(string plain);
    }
}