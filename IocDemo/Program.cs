using System;
using System.Collections;

namespace IocDemo
{
    class Program
    {
        static void Main(string[] args)
        {
            App app = new App();

            // 自己实现注入
            MyServiceProvider services = new MyServiceProvider();
            app.ConfigureMyServices(services);

            // Starup方法参数注入
            var starup = typeof(App).GetMethod("Starup");
            if (starup == null)
                throw new Exception("Starup");

            var parameters = new ArrayList();

            foreach (var parameter in starup.GetParameters())
                parameters.Add(services.GetService(parameter.ParameterType));

            starup.Invoke(app, parameters.ToArray());
        }
    }
}