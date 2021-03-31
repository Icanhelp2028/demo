using Core.Common.Extensions;
using Core.Common.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace WebApp.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/user")]
    public class UserController
    {
        private readonly IResponseService _response;
        private readonly UserInfo _userInfo;
        public UserController(IResponseService response, IUserContextAccessor userContextAccessor)
        {
            _response = response;
            _userInfo = userContextAccessor.UserContext.UserInfo;
        }

        /// <summary>会员信息</summary>
        [AllowAnonymous]
        [HttpPost("GetUserInfo")]
        public ResponseResult GetUserInfo()
        {
            if (_userInfo == null)
            {
                return ResponseResult.Error;
            }
            else
            {
                return _response.Success(_userInfo);
            }
        }

        /// <summary>签到</summary>
        [HttpPost("CheckIn")]
        public ResponseResult CheckIn()
        {
            return ResponseResult.Success;
        }
    }
}