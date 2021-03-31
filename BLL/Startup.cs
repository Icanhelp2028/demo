using BLL.Db;
using BLL.Injections;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace BLL
{
    public class Startup
    {
        public static void ConfigureServices(IConfiguration configuration, IServiceCollection services)
        {
            // 数据库注入//请求内共享
            services.AddDbContextPool<UserDb>(options => options.UseSqlServer(configuration.GetConnectionString(nameof(UserDb))));
            services.AddDbContextPool<ChatDb>(options => options.UseSqlServer(configuration.GetConnectionString(nameof(ChatDb))));

            // BLL注入
            var blls = typeof(UserBLL).Assembly.GetTypes();
            foreach (var bll in blls)
            {
                if (!bll.Name.EndsWith("BLL"))
                {
                    continue;
                }

                var ibll = bll.GetInterface("I" + bll.Name);
                if (ibll == null)
                {
                    // 通过实体类访问
                    services.AddScoped(bll);
                }
                else
                {
                    // 通过接口访问
                    services.AddScoped(ibll, bll);
                }
            }
        }
    }
}