using Core.Common.Extensions;
using Core.Common.Generic;
using Core.Common.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace WebApp.Controllers
{
    [ApiController]
    [Route("api")]
    public class HomeController
    {
        private readonly IResponseService _response;
        public HomeController(IResponseService response)
        {
            _response = response;
        }

        ///// <summary>权限测试，没有权限则返回json: { code:401 }，否则 { code:200 }，响应头都为200 OK</summary>
        //[Authorize(Roles = nameof(UserRole.User))]
        //[HttpPost("Test")]
        //public ResponseResult Test(ILogger logger)
        //{
        //    logger.LogInformation("This is a {test}", "TEST");
        //    return ResponseResult.Success;
        //}

        [HttpPost("Test")]
        public ResponseResult Test(
            [FromServices] ILogger logger)
        {
            logger.LogInformation("This is a {test}", "TEST");
            return ResponseResult.Success;
        }



        /// <summary>登录</summary>
        [HttpPost("Login")]
        public ResponseResult Login(
            [FromServices] ILoginService loginService,
            [FromServices] ICheckCodeService checkCodeService,
            [FromForm] string codeid,
            [FromForm] string code,
            [FromForm] string userName
        )
        {
            // 验证->验证码
            if (!checkCodeService.IsValidCode(codeid, code))
                return ResponseResult.Error;

            // 登录成功，保存登录信息
            var token = loginService.Login(new UserInfo(userName.GetHashCode(), userName, UserRole.User, 1));

            return _response.Success(new
            {
                token
            });
        }

        /// <summary>注册</summary>
        [HttpPost("Reg")]
        public ResponseResult Register(
            [FromServices] ICheckCodeService checkCodeService,
            [FromForm] string codeid,
            [FromForm] string code,
            [FromForm] string userName
        )
        {
            if (!checkCodeService.IsValidCode(codeid, code))
                return ResponseResult.Error;

            return ResponseResult.Success;
        }


        /// <summary>验证码</summary>
        [HttpPost("GetCheckCode")]
        public ResponseResult GetCheckCode(
            [FromServices] ICheckCodeService checkCodeService,
            [FromForm] int width = 0,
            [FromForm] int height = 30
        )
        {
            var capcha = new Captcha();
            var code = capcha.GenerateCode(4);
            var codeid = checkCodeService.SetCheckCode(code);
            var base64 = "data:image/png;base64," + capcha.GenerateImage(code, width, height);

            return _response.Success(new
            {
                codeid,
                image = base64
            });
        }
    }
}