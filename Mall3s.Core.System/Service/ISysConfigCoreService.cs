using Mall3s.System.Entitys.Dto.System.SysConfig;
using Mall3s.System.Entitys.System;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mall3s.Core.System.Service
{
    public interface ISysConfigCoreService
    {
        /// <summary>
        /// 系统配置信息
        /// </summary>
        /// <param name="category">分类</param>
        /// <param name="key">键</param>
        /// <returns></returns>
        Task<SysConfigEntity> GetInfo(string category, string key);

        /// <summary>
        /// 获取系统配置
        /// </summary>
        /// <returns></returns>
        Task<SysConfigOutput> GetInfo();
    }
}
