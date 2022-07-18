using Mall3s.WorkFlow.Entitys;
using Mall3s.WorkFlow.Entitys.Dto.FlowEngine;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mall3s.Core.WorkFlow.Service
{
    /// <summary>
    /// 流程引擎
    /// 版 本：V3.2
    /// 版 权：mall3s开发
    /// 作 者：Mall3s开发平台组
    /// 日 期：2021-06-01 
    /// </summary>
    public interface IFlowEngineCoreService
    {
        /// <summary>
        /// 列表
        /// </summary>
        /// <returns></returns>
        Task<List<FlowEngineEntity>> GetList();

        /// <summary>
        /// 信息
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        Task<FlowEngineEntity> GetInfo(string id);

        /// <summary>
        /// 信息
        /// </summary>
        /// <param name="enCode">流程编码</param>
        /// <returns></returns>
        Task<FlowEngineEntity> GetInfoByEnCode(string enCode);

        /// <summary>
        /// 删除
        /// </summary>
        /// <param name="entity">实体对象</param>
        Task<int> Delete(FlowEngineEntity entity);

        /// <summary>
        /// 创建
        /// </summary>
        /// <param name="entity">实体对象</param>
        /// <param name="visibleList">可见范围</param>
        Task<FlowEngineEntity> Create(FlowEngineEntity entity, List<FlowEngineVisibleEntity> visibleList);

        /// <summary>
        /// 更新
        /// </summary>
        /// <param name="id">主键值</param>
        /// <param name="entity">实体对象</param>
        /// <param name="visibleList">可见范围</param>
        Task<int> Update(string id, FlowEngineEntity entity, List<FlowEngineVisibleEntity> visibleList);

        /// <summary>
        /// 更新
        /// </summary>
        /// <param name="id">主键值</param>
        /// <param name="entity">实体对象</param>
        Task<int> Update(string id, FlowEngineEntity entity);

        /// <summary>
        /// 可见流程列表
        /// </summary>
        /// <param name="userId">用户主键</param>
        /// <returns></returns>
        Task<List<FlowEngineVisibleEntity>> GetVisibleFlowList(string userId);

    }
}
