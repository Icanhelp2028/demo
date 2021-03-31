namespace Core.Common.Services
{
    /// <summary>返回响应结果</summary>
    public interface IResponseService
    {
        /// <summary>成功</summary>
        ResponseResult Success(int userCode = ResponseCode.Success, string userMsg = null, object data = null);

        /// <summary>失败，并记录日志</summary>
        ResponseResult Error(int userCode, string userMsg, string errorText, string errorTitle, object data = null);
    }

    public class ResponseCode
    {
        /// <summary>成功</summary>
        public const int Success = 200;

        /// <summary>没有权限</summary>
        public const int Unauthorized = 401;

        /// <summary>失败</summary>
        public const int Error = 500;
    }

    public struct ResponseResult
    {
        /// <summary>成功</summary>
        public static readonly ResponseResult Success = new ResponseResult { Code = ResponseCode.Success, Msg = null, Data = null };

        /// <summary>失败</summary>
        public static readonly ResponseResult Error = new ResponseResult { Code = ResponseCode.Error, Msg = null, Data = null };

        /// <summary>没有权限</summary>
        public static readonly ResponseResult Unauthorized = new ResponseResult { Code = ResponseCode.Unauthorized, Msg = null, Data = null };

        public int Code { get; set; }
        public string Msg { get; set; }
        public object Data { get; set; }
    }
}