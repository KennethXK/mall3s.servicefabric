using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Serilog;
using Serilog.Events;
namespace Mall3s.Nacos
{
    /// <summary>
    ///Nacos拓展
    /// </summary>
    public static class NacosHostingExtensions
    {

        /// <summary>
        /// 添加Nacos拓展(这个扩展注意，在JNPF里面无效，请参考原生调用）
        /// </summary>
        /// <param name="hostBuilder"></param>
        /// <returns>IWebHostBuilder</returns>
        public static IWebHostBuilder UseNacos(this IWebHostBuilder hostBuilder)
        {
            hostBuilder.ConfigureAppConfiguration((context, configuration) =>
            {
               
                var c = configuration.Build();

                // read configuration from config files
                // it will use default json parser to parse the configuration store in nacos server.
                configuration.AddNacosV2Configuration(c.GetSection("NacosConfig"), logAction: x => x.AddSerilog(Log.Logger));
           
                // you also can specify ini or yaml parser as well.
                // builder.AddNacosV2Configuration(c.GetSection("NacosConfig"), Nacos.IniParser.IniConfigurationStringParser.Instance);
                // builder.AddNacosV2Configuration(c.GetSection("NacosConfig"), Nacos.YamlParser.YamlConfigurationStringParser.Instance);
            }).UseSerilog();


            return hostBuilder;
        }
    }

      
}
