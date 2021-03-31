using Core.Common.Services;
using Microsoft.Extensions.Logging;

namespace Core.Data.Services
{
    /// <summary>返回响应结果</summary>
    public sealed class ResponseService : IResponseService
    {
        private readonly ILogger _logger;

        /// <summary>返回响应结果</summary>
        public ResponseService(ILogger logger)
        {
            _logger = logger;
        }

        public ResponseResult Success(int userCode, string userMsg = null, object data = null)
        {
            return new ResponseResult
            {
                Code = userCode,
                Msg = userMsg,
                Data = data
            };
        }

        public ResponseResult Error(int userCode, string userMsg, string errorText, string errorTitle, object data = null)
        {
            _logger.LogError("Msg:{userMsg}\r\nTitle:{errorTitle}\r\nText:{errorText}", userMsg, errorTitle, errorText);

            return new ResponseResult
            {
                Code = userCode,
                Msg = userMsg,
                Data = data
            };
        }
    }
}