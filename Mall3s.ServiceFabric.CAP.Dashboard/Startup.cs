
using DotNetCore.CAP.Dashboard.NodeDiscovery;
using Mall3s.ServiceFabric.Cap;
using Mall3s.ServiceFabric.Core;
using Mall3s.ServiceFabric.MicroService;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Mall3s.ServiceFabric.CAP.Dashboard
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

            services.AddMall3sCap(Configuration, "CapSqlConnection");

            services.AddCap(x =>
            {
                //...

                // 注册 Dashboard
                x.UseDashboard();

                // 注册节点到 Consul
                x.UseDiscovery(d =>
                {
                    d.DiscoveryServerHostName = Configuration["CAPDashboard:ConsulHost"];// "121.4.154.38";
                    d.DiscoveryServerPort = Configuration["CAPDashboard:Port"].ToInt32();// 30085;
                    d.CurrentNodeHostName = Configuration["CAPDashboard:Server"];// "localhost";
                    d.CurrentNodePort = Configuration["CAPDashboard:Port"].ToInt32();// 5000;
                    d.NodeId = Configuration["CAPDashboard:NodeId"];// 
                    d.NodeName = Configuration["CAPDashboard:NodeName"];// 
                });
            });

            services.AddControllers();
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
                endpoints.MapControllers();
            });
        }
    }
}
