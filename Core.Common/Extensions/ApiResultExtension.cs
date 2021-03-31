using Core.Common.Services;
using System;

namespace Core.Common.Extensions
{
    public static class ApiResultExtension
    {
        public static ResponseResult Success(this IResponseService core, object data) => core.Success(ResponseCode.Success, null, data);

        public static ResponseResult Success(this IResponseService core, int userCode, object data = null) => core.Success(userCode, null, data);

        public static ResponseResult Success(this IResponseService core, string userMsg, object data = null) => core.Success(ResponseCode.Success, userMsg, data);

        public static ResponseResult Error(this IResponseService core, int userCode, string userMsg, string errorText, object data = null) => core.Error(userCode, userMsg, errorText, userMsg, data);

        public static ResponseResult Error(this IResponseService core, int userCode, string userMsg, Exception exception, object data = null) => core.Error(userCode, userMsg, exception.StackTrace, exception.Message, data);
    }
}