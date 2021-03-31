//using Microsoft.AspNetCore.Authorization;
//using System.Threading.Tasks;

//namespace WebApp.Authorization
//{
//    public class AnonymousAuthorizationRequirement: IAuthorizationRequirement
//    {
//    }

//    /// <summary>
//    /// 鉴定是否为匿名
//    /// </summary>
//    public class AnonymousPolicyAuthorizationHandler : AuthorizationHandler<AnonymousAuthorizationRequirement>
//    {
//        protected override Task HandleRequirementAsync(AuthorizationHandlerContext context, AnonymousAuthorizationRequirement requirement)
//        {
//            if (context.User.Identity.IsAuthenticated)
//                context.Fail();
//            else
//                context.Succeed(requirement);

//            return Task.CompletedTask;
//        }
//    }
//}