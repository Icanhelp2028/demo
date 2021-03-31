using BLL.Services;
using Core.Common.Services;
using Core.Data.Services;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using WebChat.Authentication;
using WebChat.Hubs;
using WebChat.Services;

namespace WebChat
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        public void ConfigureServices(IServiceCollection services)
        {
            var redis = Configuration.GetSection("Redis");

            services.AddHttpContextAccessor();
            /// Redis
            services.AddSingleton<IRedisContext>(p => new RedisContext(redis));
            /// 系统时间
            services.AddSingleton<ISysClockService>(p => new SysClockService(Configuration.GetConnectionString("UserDb")));
            /// 登录
            services.AddSingleton<ILoginService, LoginService>();
            /// 登录信息
            services.AddSingleton<IUserContextAccessor, UserContextAccessor<QueryToken>>();
            /// 日志
            services.AddSingleton<ILogger, LoggerService>();
            /// userId提供
            services.AddSingleton<IUserIdProvider, ChatUserIdProvider>();

            // 身份认证
            services
                .AddAuthentication(options =>
                {
                    options.DefaultScheme = nameof(CustomAuthenticationHandler);
                    options.DefaultChallengeScheme = nameof(CustomAuthenticationHandler);
                    options.DefaultForbidScheme = nameof(CustomAuthenticationHandler);
                })
                .AddScheme<CustomAuthenticationSchemeOptions, CustomAuthenticationHandler>(nameof(CustomAuthenticationHandler), options => { options.IsOutputJson = false; });

            // 域跨设置
            services.AddCors(options =>
                options.AddDefaultPolicy(
                    builder =>
                    {
                        builder
                            .SetIsOriginAllowed(origin => true)
                            .WithMethods("POST")
                            .AllowAnyHeader()
                            .AllowCredentials();
                    })
            );

            services
                .AddSignalR(p => p.EnableDetailedErrors = true)
                .AddRedis(redis.GetValue<string>("ConnectionString"));

            BLL.Startup.ConfigureServices(Configuration, services);
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env, IServiceProvider serviceProvider)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseHttpsRedirection();

            app.UseRouting();

            app.UseAuthentication();
            app.UseAuthorization();

            app.UseCors();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapHub<ChatHub>("/chat");
            });
        }
    }
}