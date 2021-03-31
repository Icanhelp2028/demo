using BLL.Services;
using Core.Common.Generic;
using Core.Common.Services;
using Core.Data.Services;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.OpenApi.Models;
using System;
using System.IO;
using WebApp.Authentication;
using WebApp.Filters;

namespace WebApp
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
            services
                 // 添加控制器功能支持
                 .AddControllers(options =>
                {
                    // Model验证
                    options.Filters.Add<ModelValidateFilter>();

                    // 异常处理
                    options.Filters.Add<ExceptionFilter>();
                })
                .ConfigureApiBehaviorOptions(options =>
                {
                    // 取消Model验证错误处理
                    options.InvalidModelStateResponseFactory = context => null;
                })
                .AddJsonOptions(options =>
                {
                    // Json转换器
                    options.JsonSerializerOptions.Converters.Add(new DateTimeConverter());
                });

            // AddSingleton //////////////////////////////////////////////
            // 系统时间
            services.AddSingleton<ISysClockService>(p => new SysClockService(Configuration.GetConnectionString("UserDb")));

            // 序列生成
            services.AddSingleton<ISequenceService, SequenceService>();

            // 登录
            services.AddSingleton<ILoginService, LoginService>();

            // 加密
            services.AddSingleton<ISecretService, SecretService>();

            // 验证码
            services.AddSingleton<ICheckCodeService, CheckCodeService>();

            // Redis
            services.AddSingleton<IRedisContext>(p => new RedisContext(Configuration.GetSection("Redis")));

            // 登录信息
            services.AddSingleton<IUserContextAccessor, UserContextAccessor<HeaderToken>>();

            // IHttpContextAccessor
            services.AddHttpContextAccessor();

            // 日志
            services.AddSingleton<ILogger, LoggerService>();
            
            // 响应内容
            services.AddSingleton<IResponseService, ResponseService>();

            // services.AddSingleton<IAuthorizationHandler, AnonymousPolicyAuthorizationHandler>();

            // 身份认证
            services
                .AddAuthentication(options =>
                {
                    options.DefaultScheme = nameof(CustomAuthenticationHandler);
                    options.DefaultChallengeScheme = nameof(CustomAuthenticationHandler);
                    options.DefaultForbidScheme = nameof(CustomAuthenticationHandler);
                })
                .AddScheme<CustomAuthenticationSchemeOptions, CustomAuthenticationHandler>(nameof(CustomAuthenticationHandler), options => { });

            //// 鉴权
            //services.AddAuthorization(options =>
            //{
            //    // 只允许匿名访问
            //    options.AddPolicy(Policies.Anonymous, policy => policy.Requirements.Add(new AnonymousAuthorizationRequirement()));

            //    // 非游客
            //    options.AddPolicy(Policies.DenyGuest, policy => policy.RequireAssertion(p => p.User.Identity.IsAuthenticated && !p.User.IsInRole(nameof(UserRole.Guest))));
            //});

            // 域跨设置
            services.AddCors(options =>
                options.AddDefaultPolicy(
                    builder =>
                    {
                        builder
                            .SetIsOriginAllowed(origin => true)
                            .WithHeaders("token")
                            .WithMethods("GET", "POST");
                    })
            );

            BLL.Startup.ConfigureServices(Configuration, services);

            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Version = "v1", Title = "WebApi" });

                var dir = new DirectoryInfo(AppContext.BaseDirectory);
                foreach (FileInfo file in dir.EnumerateFiles("*.xml"))
                {
                    c.IncludeXmlComments(file.FullName);
                }
            });

        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseHttpsRedirection();

            app.UseRouting();

            app.UseCors();

            app.UseAuthentication();
            app.UseAuthorization();

            app
                .UseEndpoints(endpoints =>
                {
                    endpoints.MapControllers();
                });

            //app.Run(async context =>
            //{
            //    await context.Response.WriteAsync("404 Not Found.");
            //});

            // https://localhost:44320/swagger/index.html
            app.UseSwagger().UseSwaggerUI(c => { c.SwaggerEndpoint("/swagger/v1/swagger.json", "API V1"); });
        }
    }
}