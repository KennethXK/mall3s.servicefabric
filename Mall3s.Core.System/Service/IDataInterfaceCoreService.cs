using Mall3s.System.Entitys.System;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mall3s.Core.System.Service
{

    /// <summary>
    /// 数据接口
    /// 版 本：V3.2
    /// 版 权：mall3s开发
    /// 作 者：Mall3s开发平台组
    /// 日 期：2021-06-01 
    /// </summary>
    public interface IDataInterfaceCoreService
    {
        /// <summary>
        /// 列表
        /// </summary>
        /// <returns></returns>
        Task<List<DataInterfaceEntity>> GetList();

        /// <summary>
        /// 信息
        /// </summary>
        /// <param name="id">主键id</param>
        /// <returns></returns>
        Task<DataInterfaceEntity> GetInfo(string id);

        /// <summary>
        /// 创建
        /// </summary>
        /// <param name="entity">实体</param>
        /// <returns></returns>
        Task<int> Create(DataInterfaceEntity entity);

        /// <summary>
        /// 删除
        /// </summary>
        /// <param name="entity">实体</param>
        /// <returns></returns>
        Task<int> Delete(DataInterfaceEntity entity);

        /// <summary>
        /// 修改
        /// </summary>
        /// <param name="entity">实体</param>
        /// <returns></returns>
        Task<int> Update(DataInterfaceEntity entity);

        /// <summary>
        /// sql接口查询
        /// </summary>
        /// <param name="id"></param>
        /// <param name="sql"></param>
        /// <returns></returns>
        Task<DataTable> GetData(DataInterfaceEntity entity);

        /// <summary>
        /// 接口查询
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        Task<DataTable> GetData(string id);
    }
}
