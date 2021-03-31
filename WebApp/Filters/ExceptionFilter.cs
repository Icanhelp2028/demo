using Core.Common.Extensions;
using Core.Common.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using System.Threading.Tasks;

namespace WebApp.Filters
{
    /// <summary>异常处理</summary>
    public class ExceptionFilter : IAsyncExceptionFilter
    {
        private readonly IResponseService _response;
        public ExceptionFilter(IResponseService response)
        {
            _response = response;
        }

        public Task OnExceptionAsync(ExceptionContext context)
        {
            // 如果异常没有被处理则进行处理
            if (context.ExceptionHandled == false)
            {
                context.Result = new JsonResult(_response.Error(ResponseCode.Error, "Server Error", context.Exception));

                // 设置为true，表示异常已经被处理了
                context.ExceptionHandled = true;
            }

            return Task.CompletedTask;
        }
    }
}