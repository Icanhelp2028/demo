namespace Core.Common.Services
{
    /// <summary>
    /// 加密类
    /// </summary>
    public interface ISecretService
    {
        string Encrypt(string plain);
        string Decrypt(string encrypt);
        string Hash(string plain);
    }
}