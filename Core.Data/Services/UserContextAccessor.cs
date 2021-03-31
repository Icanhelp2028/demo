using Core.Common.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;

namespace Core.Data.Services
{
    /// <summary>登录信息</summary>
    public sealed class UserContextAccessor<TUserToken> : IUserContextAccessor where TUserToken : class, IUserToken
    {
        private readonly TUserToken _token;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly ILoginService _loginService;

        public UserContextAccessor(IOptions<TUserToken> options, IHttpContextAccessor httpContextAccessor, ILoginService loginService)
        {
            _token = options.Value;
            _httpContextAccessor = httpContextAccessor;
            _loginService = loginService;
        }

        public UserInfoContext UserContext
        {
            get
            {
                var token = _token.GetToken(_httpContextAccessor);
                if (token != null)
                {
                    var userInfo = _loginService.Get(token);
                    return userInfo == null ? UserInfoContext.Anonymous : new UserInfoContext(userInfo, token);
                }

                return UserInfoContext.Anonymous;
            }
        }
    }

    public interface IUserToken
    {
        string GetToken(IHttpContextAccessor httpContextAccessor);
    }

    public class QueryToken : IUserToken
    {
        /// <summary>
        /// 修改可通过：services.Configure<QueryToken>(p => p.TokenKey = "token");
        /// </summary>
        public string TokenKey { get; set; } = "token";
        public string GetToken(IHttpContextAccessor httpContextAccessor)
        {
            var token = httpContextAccessor.HttpContext.Request.Query[TokenKey];
            return token.Count > 0 ? token[0] : null;
        }
    }

    public class HeaderToken : IUserToken
    {
        /// <summary>
        /// 修改可通过：services.Configure<HeaderToken>(p => p.TokenKey = "token");
        /// </summary>
        public string TokenKey { get; set; } = "token";
        public string GetToken(IHttpContextAccessor httpContextAccessor)
        {
            var token = httpContextAccessor.HttpContext.Request.Headers[TokenKey];
            return token.Count > 0 ? token[0] : null;
        }
    }
}