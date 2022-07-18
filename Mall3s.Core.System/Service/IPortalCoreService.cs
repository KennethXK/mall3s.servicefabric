using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mall3s.Core.System.Service
{
    /// <summary>
    /// 业务契约：门户设计
    /// </summary>
    public interface IPortalCoreService
    {
        /// <summary>
        /// 获取默认门户
        /// </summary>
        /// <returns></returns>
        Task<string> GetDefault();
    }
}
