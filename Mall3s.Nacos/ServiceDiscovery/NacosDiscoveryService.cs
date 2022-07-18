using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Mall3s.Dependency;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Nacos.V2;
using Nacos.V2.DependencyInjection;
using Nacos.V2.Naming.Dtos;

namespace Mall3s.Nacos.ServiceDiscovery
{
    /// <summary>
    /// nacos服务发现地址
    /// </summary>
    public class NacosDiscoveryService: INacosDiscoveryService, ITransient
    {
        private IServiceProvider _serviceProvider;

        private IConfiguration _configuration;
        private readonly INacosNamingService _svc;
        public NacosDiscoveryService(IConfiguration configuration)
        {
            _configuration = configuration;
              _serviceProvider = InitServiceProvider();
            _svc = _serviceProvider.GetService<INacosNamingService>();
        }
        #region 初始化
        private IServiceProvider InitServiceProvider()
        {
            IServiceCollection services = new ServiceCollection();

            services.AddNacosV2Naming(_configuration, null, "NacosConfig");
            services.AddLogging(builder => { builder.AddConsole(); });

            IServiceProvider serviceProvider = services.BuildServiceProvider();

            return serviceProvider;
        }
        #endregion
        /// <summary>
        /// 获取nacos微服务地址
        /// </summary>
        /// <param name="serviceName"></param>
        /// <param name="groupName"></param>
        /// <returns></returns>
        public async Task<string> GetNacosMicroBaseUrlAsync(string serviceName,string groupName)
        {
          var instance=await GetInstanceAsync( serviceName,  groupName);
            var host = $"{instance.Ip}:{instance.Port}";
            var baseUrl = instance.Metadata.TryGetValue("secure", out _)
             ? $"https://{host}"
             : $"http://{host}";
            return baseUrl;
        }
        /// <summary>
        /// 获取服务发现Instance
        /// </summary>
        /// <param name="serviceName"></param>
        /// <param name="groupName"></param>
        /// <returns></returns>
        public  async Task<Instance> GetInstanceAsync(string serviceName, string groupName)
        {
           return await _svc.SelectOneHealthyInstance(serviceName, groupName);
        }


        //服务下线

        //public async Task DeregisterInstance()
        //{
        //    var serviceName = _configuration.GetSection("NacosConfig:ServiceName")?.Value;
        //    var groupName = _configuration.GetSection("NacosConfig:GroupName")?.Value;
        //    var ip = _configuration.GetSection("NacosConfig:Ip")?.Value;
        //    var port = _configuration.GetSection("NacosConfig:Port")?.Value;
        //    if (string.IsNullOrEmpty(ip)||string.IsNullOrEmpty(port))
        //    {
        //        //本地获取当前服务器ip地址和端口
        //    }

           
        //    Console.WriteLine($"======================注销实例成功");
        //}

    }
}
