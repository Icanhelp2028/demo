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
                 // ��ӿ���������֧��
                 .AddControllers(options =>
                {
                    // Model��֤
                    options.Filters.Add<ModelValidateFilter>();

                    // �쳣����
                    options.Filters.Add<ExceptionFilter>();
                })
                .ConfigureApiBehaviorOptions(options =>
                {
                    // ȡ��Model��֤������
                    options.InvalidModelStateResponseFactory = context => null;
                })
                .AddJsonOptions(options =>
                {
                    // Jsonת����
                    options.JsonSerializerOptions.Converters.Add(new DateTimeConverter());
                });

            // AddSingleton //////////////////////////////////////////////
            // ϵͳʱ��
            services.AddSingleton<ISysClockService>(p => new SysClockService(Configuration.GetConnectionString("UserDb")));

            // ��������
            services.AddSingleton<ISequenceService, SequenceService>();

            // ��¼
            services.AddSingleton<ILoginService, LoginService>();

            // ����
            services.AddSingleton<ISecretService, SecretService>();

            // ��֤��
            services.AddSingleton<ICheckCodeService, CheckCodeService>();

            // Redis
            services.AddSingleton<IRedisContext>(p => new RedisContext(Configuration.GetSection("Redis")));

            // ��¼��Ϣ
            services.AddSingleton<IUserContextAccessor, UserContextAccessor<HeaderToken>>();

            // IHttpContextAccessor
            services.AddHttpContextAccessor();

            // ��־
            services.AddSingleton<ILogger, LoggerService>();
            
            // ��Ӧ����
            services.AddSingleton<IResponseService, ResponseService>();

            // services.AddSingleton<IAuthorizationHandler, AnonymousPolicyAuthorizationHandler>();

            // �����֤
            services
                .AddAuthentication(options =>
                {
                    options.DefaultScheme = nameof(CustomAuthenticationHandler);
                    options.DefaultChallengeScheme = nameof(CustomAuthenticationHandler);
                    options.DefaultForbidScheme = nameof(CustomAuthenticationHandler);
                })
                .AddScheme<CustomAuthenticationSchemeOptions, CustomAuthenticationHandler>(nameof(CustomAuthenticationHandler), options => { });

            //// ��Ȩ
            //services.AddAuthorization(options =>
            //{
            //    // ֻ������������
            //    options.AddPolicy(Policies.Anonymous, policy => policy.Requirements.Add(new AnonymousAuthorizationRequirement()));

            //    // ���ο�
            //    options.AddPolicy(Policies.DenyGuest, policy => policy.RequireAssertion(p => p.User.Identity.IsAuthenticated && !p.User.IsInRole(nameof(UserRole.Guest))));
            //});

            // �������
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