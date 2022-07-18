using Mall3s.Core.System.Manager;
using Mall3s.WorkFlow.Entitys;
using Microsoft.AspNetCore.Mvc;
using SqlSugar;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mall3s.Core.WorkFlow.Service.Impl
{
    /// <summary>
    /// 流程委托
    /// 版 本：V3.2
    /// 版 权：mall3s开发
    /// 作 者：Mall3s开发平台组
    /// 日 期：2021-06-01 
    /// </summary>
    public class FlowDelegateCoreService : IFlowDelegateCoreService
    {
        private readonly ISqlSugarRepository<FlowDelegateEntity> _flowDelegateRepository;
        private readonly IUserCoreManager _userManager;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="flowDelegateRepository"></param>
        /// <param name="userManager"></param>
        public FlowDelegateCoreService(ISqlSugarRepository<FlowDelegateEntity> flowDelegateRepository, IUserCoreManager userManager)
        {
            _flowDelegateRepository = flowDelegateRepository;
            _userManager = userManager;
        }

        #region PublicMethod
        /// <summary>
        /// 新增
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        [NonAction]
        public async Task<int> Create(FlowDelegateEntity entity)
        {
            return await _flowDelegateRepository.Context.Insertable(entity).CallEntityMethod(m => m.Creator()).ExecuteCommandAsync();
        }

        /// <summary>
        /// 删除
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        [NonAction]
        public async Task<int> Delete(FlowDelegateEntity entity)
        {
            return await _flowDelegateRepository.Context.Updateable(entity).IgnoreColumns(ignoreAllNullColumns: true).CallEntityMethod(m => m.Delete()).ExecuteCommandAsync();
        }

        /// <summary>
        /// 获取所有委托给当前用户的委托人
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        [NonAction]
        public async Task<List<string>> GetDelegateUserId(string userId)
        {
            var list = await _flowDelegateRepository.Entities.Where(x => x.ToUserId == userId && x.EndTime > DateTime.Now && x.DeleteMark == null).OrderBy(o => o.CreatorTime, OrderByType.Desc).ToListAsync();
            return list.Select(x => x.CreatorUserId).ToList();
        }

        /// <summary>
        /// 详情
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [NonAction]
        public async Task<FlowDelegateEntity> GetInfo(string id)
        {
            return await _flowDelegateRepository.FirstOrDefaultAsync(x => x.Id == id && x.DeleteMark == null);
        }

        /// <summary>
        /// 当前用户所有委托
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        [NonAction]
        public async Task<List<FlowDelegateEntity>> GetList(string userId)
        {
            return await _flowDelegateRepository.Entities.Where(x => x.CreatorUserId == userId && x.DeleteMark == null).ToListAsync();
        }

        /// <summary>
        /// 修改
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        [NonAction]
        public async Task<int> Update(FlowDelegateEntity entity)
        {
            return await _flowDelegateRepository.Context.Updateable(entity).IgnoreColumns(ignoreAllNullColumns: true).CallEntityMethod(m => m.LastModify()).ExecuteCommandAsync();
        }
        #endregion
    }
}
