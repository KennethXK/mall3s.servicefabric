
using Mall3s.Dependency;
using Mall3s.Nacos.ServiceDiscovery;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Nacos.V2;
using Nacos.V2.DependencyInjection;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mall3s.Nacos.Config
{
  
    public class NacosAppConfigService : INacosAppConfigService, ITransient
    {
        private INacosConfigService _svc ;
        private IConfiguration _configuration;
        private IServiceProvider _serviceProvider;
        public NacosAppConfigService(IConfiguration configuration)
        {
            
            _configuration = configuration;
            _serviceProvider = InitServiceProvider();
            _svc = _serviceProvider.GetService<INacosConfigService>();
        }
        #region 初始化
        private IServiceProvider InitServiceProvider()
        {
            IServiceCollection services = new ServiceCollection();
           
            services.AddNacosV2Config(_configuration,null, "NacosConfig");
            services.AddLogging(builder => { builder.AddConsole(); });

            IServiceProvider serviceProvider = services.BuildServiceProvider();

            return serviceProvider;
        }
        #endregion
        #region 初始化数据
        /// <summary>
        /// 初始化数据
        /// </summary>
        /// <returns></returns>
        public async Task<ConfigurationBuilder> InitConfigurationBuilderData()
        {
            var builder = new ConfigurationBuilder();

            var configDatas = _configuration.GetSection("NacosConfig:Listeners").Get<List<ListenersModel>>();
          
           
                foreach (var configData in configDatas)
                {
                    var nacosData = await GetNacosMicroConfigAsync<string>(configData.DataId, configData.Group);
                    byte[] array = Encoding.ASCII.GetBytes(nacosData);
                        MemoryStream stream = new MemoryStream(array);

                        builder.AddJsonStream(stream);
                    

                }
          
            return builder;


        } 
        #endregion
        public class ListenersModel
        {
            public bool Optional { get; set; }
            public string DataId { get; set; }
            public string Group { get; set; }
        }
        /// <summary>
        /// 获取微服务配置中心地址
        /// </summary>
        /// <param name="dataId"></param>
        /// <param name="groupName"></param>
        /// <param name="timeoutMs"></param>
        /// <returns></returns>
        public async Task<T> GetNacosMicroConfigAsync<T>(string dataId, string groupName,long timeoutMs=5000L)
        {
          var data= await  _svc.GetConfig(dataId, groupName, timeoutMs);
             if(data is string)
            {
                return (T)Convert.ChangeType(data, typeof(T));
            }
            return JsonConvert.DeserializeObject<T>(data);
        }
       
        
    }
}
