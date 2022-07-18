using Mall3s.Common.Filter;
using Mall3s.System.Entitys.Dto.System.Province;
using Mall3s.System.Entitys.System;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mall3s.Core.System.Service
{
    /// <summary>
    /// 行政区划
    /// 版 本：V3.2
    /// 版 权：mall3s开发
    /// 作 者：Mall3s开发平台组
    /// 日 期：2021-06-01 
    /// </summary>
    public interface IProvinceCoreService
    {

        /// <summary>
        /// 获取行政区划列表
        /// </summary>
        /// <returns></returns>
        Task<List<ProvinceEntity>> GetList();

        /// <summary>
        /// 列表
        /// </summary>
        /// <param name="parentId"></param>
        /// <returns></returns>
        Task<List<ProvinceEntity>> GetList(string id);
    }
}
