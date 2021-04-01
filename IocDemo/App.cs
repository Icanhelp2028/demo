namespace IocDemo
{
    class App
    {
        public void ConfigureMyServices(IMyServiceCollection services)
        {
            services.AddScoped<IMain, Main>();
            services.AddScoped<IThreadA, ThreadA>();
            services.AddScoped<IThreadB, ThreadB>();
            services.AddSingleton<IThreadC, ThreadC>();
        }

        public void Starup(IMain main)
        {
            main.Run();
        }
    }
}