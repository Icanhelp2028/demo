using Core.Common.Extensions;
using Core.Common.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using WebApp.Extensions;

namespace WebApp.Filters
{
    public class ModelValidateFilter : IActionFilter
    {
        public void OnActionExecuted(ActionExecutedContext context)
        {
        }

        public void OnActionExecuting(ActionExecutingContext context)
        {
            // 验证model
            if (!context.ModelState.IsValid)
            {
                var responseService = (IResponseService)context.HttpContext.RequestServices.GetService(typeof(IResponseService));
                var errors = context.ModelState.GetErrors();
                var model = responseService.Success(ResponseCode.Error, errors);

                context.Result = new JsonResult(model);
            }
        }
    }
}