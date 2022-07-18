using Mall3s.ServiceFabric.Core;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;

namespace Mall3s.ServiceFabric.Cap
{
    public static class ServiceCollectionExtensions
    {
        
        /// <summary>
        /// 添加Cap服务
        /// </summary>
        /// <typeparam name="TDbContext"></typeparam>
        /// <param name="services"></param>
        /// <returns></returns>
        public static IServiceCollection AddMall3sCap<TDbContext>(this IServiceCollection services, IConfiguration configuration)
            where TDbContext : DbContext
        {
            services.AddCap(x =>
            {
                x.UseEntityFramework<TDbContext>();
                x.UseDashboard();
                x.UseRabbitMQ(configure =>
                {
                    configure.HostName = configuration["RabbitMQ:Host"];//ConfigManagerConf.GetValue("RabbitMQ:Host");
                    configure.UserName = configuration["RabbitMQ:UserName"]; //ConfigManagerConf.GetValue("RabbitMQ:UserName");
                    configure.Password = configuration["RabbitMQ:Password"]; //ConfigManagerConf.GetValue("RabbitMQ:Password");
                    configure.Port = configuration["RabbitMQ:Port"].ToInt32();
                });
                x.FailedRetryCount = 5;
            });
            return services;
        }

        /// <summary>
        /// GetServiceId
        /// </summary>
        /// <param name="services"></param>
        /// <param name="connectionStrings"></param>
        /// <returns></returns>
        public static IServiceCollection AddMall3sCap(this IServiceCollection services, IConfiguration configuration, string connKey)
        {
            var conn = configuration.GetSection(connKey);
            var connectionStrings = string.Format(conn.GetValue<string>("DefaultConnection"), conn.GetValue<string>("DBName"));
            services.AddCap(x =>
            {
              //;string.IsNullOrEmpty(configuration["SerivceDiscovery:Version"]) ? "v1" : configuration["SerivceDiscovery:Version"];
                x.UseMySql(connectionStrings);
                
                x.UseDashboard();
                x.UseRabbitMQ(configure =>
                {
                    configure.HostName = configuration["RabbitMQ:Host"];//ConfigManagerConf.GetValue("RabbitMQ:Host");
                    configure.UserName = configuration["RabbitMQ:UserName"]; //ConfigManagerConf.GetValue("RabbitMQ:UserName");
                    configure.Password = configuration["RabbitMQ:Password"]; //ConfigManagerConf.GetValue("RabbitMQ:Password");
                    configure.Port =configuration["RabbitMQ:Port"].ToInt32();
                });
                x.FailedRetryCount = 5;
            });
            return services;
        }
    }
}