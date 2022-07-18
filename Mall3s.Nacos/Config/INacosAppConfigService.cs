using Microsoft.Extensions.Configuration;
using Nacos.V2.Naming.Dtos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mall3s.Nacos.ServiceDiscovery
{
    /// <summary>
    /// 扩展的nacos配置中心读取
    /// </summary>
    public interface INacosAppConfigService
    {    /// <summary>
         /// 获取nacos微服务配置
         /// </summary>
         /// <param name="dataId"></param>
         /// <param name="groupName"></param>
         /// <returns></returns>
        Task<T> GetNacosMicroConfigAsync<T>(string dataId, string groupName, long timeoutMs = 5000L);

        /// <summary>
        /// 初始化数据
        /// </summary>
        /// <returns></returns>
          Task<ConfigurationBuilder> InitConfigurationBuilderData();
    }
}
