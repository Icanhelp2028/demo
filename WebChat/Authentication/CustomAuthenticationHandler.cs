using Core.Common.Services;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System.Security.Claims;
using System.Text.Encodings.Web;
using System.Text.Json;
using System.Threading.Tasks;

namespace WebChat.Authentication
{
    /// <summary>
    /// 角色身份验证配置
    /// </summary>
    public class CustomAuthenticationSchemeOptions : AuthenticationSchemeOptions
    {
        public bool IsOutputJson { get; set; } = true;
    }

    /// <summary>
    /// 角色身份认证
    /// </summary>
    public class CustomAuthenticationHandler : AuthenticationHandler<CustomAuthenticationSchemeOptions>
    {
        private readonly IUserContextAccessor _userContextAccessor;

        public CustomAuthenticationHandler(
            IOptionsMonitor<CustomAuthenticationSchemeOptions> options,
            ILoggerFactory logger,
            UrlEncoder encoder,
            ISystemClock clock,
            IUserContextAccessor userContextAccessor) : base(options, logger, encoder, clock)
        {
            this._userContextAccessor = userContextAccessor;
        }

        /// <summary>
        /// DefaultScheme,授权
        /// </summary>
        protected override Task<AuthenticateResult> HandleAuthenticateAsync()
        {
            var userInfo = _userContextAccessor.UserContext.UserInfo;
            if (userInfo == null)
                return Task.FromResult(AuthenticateResult.NoResult());

            // HttpContext.User.Name
            var claimName = new Claim(ClaimTypes.Name, userInfo.UserName);

            // Authorize(Role="{0}")
            var claimRole = new Claim(ClaimTypes.Role, userInfo.Role.ToString());

            var claimIdentity = new ClaimsIdentity(new[] { claimRole, claimName }, nameof(CustomAuthenticationHandler));
            var claimsPrincipal = new ClaimsPrincipal(claimIdentity);
            var authenticationTicket = new AuthenticationTicket(claimsPrincipal, nameof(CustomAuthenticationHandler));
            var result = AuthenticateResult.Success(authenticationTicket);

            return Task.FromResult(result);
        }

        /// <summary>
        /// DefaultChallengeScheme,禁止访问401 Unauthorized
        /// 认证错误，表示这个请求没有被服务器认证或者客户端传送的证书错误，可以修改后在进行重试
        /// </summary>
        protected override async Task HandleChallengeAsync(AuthenticationProperties properties)
        {
            await HandleAsync(StatusCodes.Status401Unauthorized);
        }

        /// <summary>
        /// DefaultForbidScheme,禁止访问403 Forbidden
        /// 用户认证后，但权限不足，无法对该资源进行操作
        /// </summary>
        protected override async Task HandleForbiddenAsync(AuthenticationProperties properties)
        {
            await HandleAsync(StatusCodes.Status403Forbidden);
        }

        private Task HandleAsync(int statusCodes)
        {
            if (Options.IsOutputJson)
            {
                Response.ContentType = "application/json";
                Response.StatusCode = StatusCodes.Status200OK;

                var json = JsonSerializer.Serialize(ResponseResult.Unauthorized);
                return Response.WriteAsync(json);
            }
            else
            {
                Response.StatusCode = statusCodes;
                return Response.WriteAsync(ResponseResult.Unauthorized.ToString());
            }
        }
    }
}