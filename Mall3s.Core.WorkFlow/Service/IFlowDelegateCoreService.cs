using Mall3s.WorkFlow.Entitys;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mall3s.Core.WorkFlow.Service
{
    public interface IFlowDelegateCoreService
    {
        #region PublicMethod
        /// <summary>
        /// 新增
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        
        Task<int> Create(FlowDelegateEntity entity);

        /// <summary>
        /// 删除
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        
        Task<int> Delete(FlowDelegateEntity entity);

        /// <summary>
        /// 获取所有委托给当前用户的委托人
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        
        Task<List<string>> GetDelegateUserId(string userId);

        /// <summary>
        /// 详情
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        
        Task<FlowDelegateEntity> GetInfo(string id);

        /// <summary>
        /// 当前用户所有委托
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        
        Task<List<FlowDelegateEntity>> GetList(string userId);

        /// <summary>
        /// 修改
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        
        Task<int> Update(FlowDelegateEntity entity);
        #endregion
    }
}
