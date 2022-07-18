using Mall3s.Common.Enum;
using Mall3s.Common.Extension;
using Mall3s.FriendlyException;
using Mall3s.System.Entitys.Permission;
using Microsoft.AspNetCore.Mvc;
using SqlSugar;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mall3s.Core.System.Service.Impl
{

    /// <summary>
    /// 业务实现：用户关系
    /// </summary>
    public class UserRelationCoreService : IUserRelationCoreService
    {
        private readonly ISqlSugarRepository<UserRelationEntity> _userRelationRepository;

        /// <summary>
        /// 初始化一个<see cref="UserRelationService"/>类型的新实例
        /// </summary>
        /// <param name="userRelationRepository"></param>
        /// <param name="userRepository"></param>
        public UserRelationCoreService(ISqlSugarRepository<UserRelationEntity> userRelationRepository)
        {
            _userRelationRepository = userRelationRepository;
        }



        #region PublicMethod

        /// <summary>
        /// 创建用户岗位关系
        /// </summary>
        /// <param name="userId">用户ID</param>
        /// <param name="ids">岗位ID</param>
        /// <returns></returns>
        [NonAction]
        public List<UserRelationEntity> CreateByPosition(string userId, string ids)
        {
            List<UserRelationEntity> userRelationList = new List<UserRelationEntity>();
            if (!ids.IsNullOrEmpty())
            {
                var position = new List<string>(ids.Split(','));
                position.ForEach(item =>
                {
                    var entity = new UserRelationEntity();
                    entity.ObjectType = "Position";
                    entity.ObjectId = item;
                    entity.SortCode = position.IndexOf(item);
                    entity.UserId = userId;
                    userRelationList.Add(entity);
                });
            }
            return userRelationList;
        }

        /// <summary>
        /// 创建用户角色关系
        /// </summary>
        /// <param name="userId">用户ID</param>
        /// <param name="ids">角色ID</param>
        /// <returns></returns>
        [NonAction]
        public List<UserRelationEntity> CreateByRole(string userId, string ids)
        {
            List<UserRelationEntity> userRelationList = new List<UserRelationEntity>();
            if (!ids.IsNullOrEmpty())
            {
                var position = new List<string>(ids.Split(','));
                position.ForEach(item =>
                {
                    var entity = new UserRelationEntity();
                    entity.ObjectType = "Role";
                    entity.ObjectId = item;
                    entity.SortCode = position.IndexOf(item);
                    entity.UserId = userId;
                    userRelationList.Add(entity);
                });
            }
            return userRelationList;
        }

        /// <summary>
        /// 创建用户关系
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        [NonAction]
        public async Task Create(List<UserRelationEntity> input)
        {
            try
            {
                //开启事务
                _userRelationRepository.Ado.BeginTran();

                await _userRelationRepository.Context.Insertable(input).CallEntityMethod(m => m.Creator()).ExecuteCommandAsync();

                _userRelationRepository.Ado.CommitTran();
            }
            catch (Exception)
            {
                _userRelationRepository.Ado.RollbackTran();
                throw;
            }
        }

        /// <summary>
        /// 删除
        /// </summary>
        /// <param name="id">用户ID</param>
        /// <returns></returns>
        [NonAction]
        public async Task Delete(string id)
        {
            try
            {
                //开启事务
                _userRelationRepository.Ado.BeginTran();

                await _userRelationRepository.DeleteAsync(u => u.UserId == id);

                _userRelationRepository.Ado.CommitTran();
            }
            catch (Exception)
            {
                _userRelationRepository.Ado.RollbackTran();
                throw Mall3sException.Oh(ErrorCode.D5003);
            }
        }

        /// <summary>
        /// 根据用户主键获取列表
        /// </summary>
        /// <param name="userId">用户主键</param>
        /// <returns></returns>
        [NonAction]
        public async Task<dynamic> GetListByUserId(string userId)
        {
            return await _userRelationRepository.Where(m => m.UserId == userId).OrderBy(o => o.CreatorTime).ToListAsync();
        }

        /// <summary>
        /// 获取岗位
        /// </summary>
        /// <param name="userId">用户主键</param>
        /// <returns></returns>
        [NonAction]
        public async Task<List<string>> GetPositionId(string userId)
        {
            var data = await _userRelationRepository.Where(m => m.UserId == userId && m.ObjectType == "Position").OrderBy(o => o.CreatorTime).ToListAsync();
            return data.Select(m => m.ObjectId).ToList();
        }

        /// <summary>
        /// 获取用户
        /// </summary>
        /// <param name="type"></param>
        /// <param name="objId"></param>
        /// <returns></returns>
        [NonAction]
        public List<string> GetUserId(string type, string objId)
        {
            var data = _userRelationRepository.Entities.Where(x => x.ObjectId == objId && x.ObjectType == type).Select(x => x.UserId).Distinct().ToList();
            return data;
        }
        #endregion
    }
}
