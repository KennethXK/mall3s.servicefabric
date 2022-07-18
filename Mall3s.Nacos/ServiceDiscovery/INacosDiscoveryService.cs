
using Nacos.V2.Naming.Dtos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mall3s.Nacos.ServiceDiscovery
{
    /// <summary>
    /// 扩展的服务发现类
    /// </summary>
    
    public interface INacosDiscoveryService
    {    /// <summary>
         /// 获取nacos微服务地址
         /// </summary>
         /// <param name="serviceName"></param>
         /// <param name="groupName"></param>
         /// <returns></returns>
        Task<string> GetNacosMicroBaseUrlAsync(string serviceName, string groupName);
        /// <summary>
        /// 获取服务发现Instance
        /// </summary>
        /// <param name="serviceName"></param>
        /// <param name="groupName"></param>
        /// <returns></returns>
       Task<Instance> GetInstanceAsync(string serviceName, string groupName);


    }
}
