using DotXxlJob.Core;
using Mall3s.ServiceFabric.Core;
using Mall3s.ServiceFabric.MicroService;
using Mall3s.ServiceFabric.TestApi.JobHandler;
using Mall3s.XxlJob.Extensions;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WebApiClient.Extensions.Nacos;

namespace Mall3s.ServiceFabric.TestApi
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {

            //添加微服务
            services.AddMicroService(Configuration);

            //注册IHttp服务请求
            services.AddNacosDiscoveryTypedClient<ITestHttpApi>("test.Mall3s.HelloWorld", "DEFAULT");

            services.AddRazorPages();
            services.AddTranslate(Configuration);
            services.AddQiNiu(Configuration);
            /*services.AddXxlJobExecutor(Configuration);
            services.AddSingleton<IJobHandler, TestHandler>();
            services.AddSingleton<IJobHandler, Test1Handler>();
            services.AddAutoRegistry();*/

            //services.AddXxlJobByParams(Configuration,typeof(TestHandler),typeof(Test1Handler), typeof(TestHandler));

            /*services.AddXxlJobByColl(Configuration, new List<Type>
            {
                typeof(Test1Handler),
                typeof(TestHandler)
            });*/
            //自动模式xxl后台任务
            services.AddHealthChecks();
            services.AddXxlJobAuto(Configuration);
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Error");
            }
            //注入mall3s微服务
            app.UseMicroService(Configuration);

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapHealthChecks("\"/nacos/\"");
                endpoints.MapRazorPages();
            });
            app.UseXxlJobExecutor();
        }
    }
}
