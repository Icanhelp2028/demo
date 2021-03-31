namespace Core.Common.Services
{
    /// <summary>登录信息存储</summary>
    public interface ILoginService
    {
        UserInfo Get(string token);
        void Logout(string token);
        void AddExpire(string token);
        string Login(UserInfo userInfo);
        void Update(string token, UserInfo userInfo);
    }
}