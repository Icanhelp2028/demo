namespace Core.Common.Services
{
    /// <summary>
    /// 验证码
    /// </summary>
    public interface ICheckCodeService
    {
        bool IsValidCode(string codeid, string code);

        string SetCheckCode(string code);
    }
}