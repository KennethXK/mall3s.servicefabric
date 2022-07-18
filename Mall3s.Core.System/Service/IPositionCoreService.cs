using Mall3s.System.Entitys.Permission;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mall3s.Core.System.Service
{
    /// <summary>
    /// 业务契约：岗位管理
    /// </summary>
    public interface IPositionCoreService
    {
        /// <summary>
        /// 获取信息
        /// </summary>
        /// <param name="id">获取信息</param>
        /// <returns></returns>
        Task<PositionEntity> GetInfoById(string id);

        /// <summary>
        /// 获取岗位列表
        /// </summary>
        /// <returns></returns>
        Task<List<PositionEntity>> GetListAsync();

        /// <summary>
        /// 获取名称
        /// </summary>
        /// <param name="ids"></param>
        /// <returns></returns>
        string GetName(string ids);
    }
}
