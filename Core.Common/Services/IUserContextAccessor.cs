using System.Text.Json.Serialization;

namespace Core.Common.Services
{
    /// <summary>登录信息</summary>
    public interface IUserContextAccessor
    {
        UserInfoContext UserContext { get; }
    }

    public class UserInfoContext
    {
        public static readonly UserInfoContext Anonymous = new UserInfoContext();
        private UserInfoContext() { }
        public UserInfoContext(UserInfo userInfo, string token)
        {
            UserInfo = userInfo;
            Token = token;
        }

        public UserInfo UserInfo { get; }
        public string Token { get; }
    }

    /// <summary>角色</summary>
    public enum UserRole
    {
        /// <summary>
        /// 游客
        /// </summary>
        Guest = 1,

        /// <summary>
        /// 会员
        /// </summary>
        User = 2,

        /// <summary>
        /// 代理
        /// </summary>
        Agent = 3
    }

    public class UserInfo
    {
        public UserInfo(int userId, string userName, UserRole role, int ownId)
        {
            UserId = userId;
            UserName = userName;
            Role = role;
            OwnId = ownId;
        }

        [JsonPropertyName("id")]
        public int UserId { get; }

        [JsonPropertyName("name")]
        public string UserName { get; }

        [JsonPropertyName("ownid")]
        public int OwnId { get; }

        [JsonPropertyName("role")]
        public UserRole Role { get; }
    }
}