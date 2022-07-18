using Mall3s.Common.Util;
using Mall3s.System.Entitys.Dto.Permission.Organize;
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
    public class OrganizeCoreService : IOrganizeCoreService
    {
        private readonly ISqlSugarRepository<OrganizeEntity> _organizeRepository;
        private readonly ISqlSugarRepository<PositionEntity> _positionRepository;
        private readonly ISqlSugarRepository<UserEntity> _userRepository;

        /// <summary>
        /// 初始化一个<see cref="OrganizeService"/>类型的新实例
        /// </summary>
        public OrganizeCoreService(ISqlSugarRepository<OrganizeEntity> organizeRepository, ISqlSugarRepository<PositionEntity> positionRepository, ISqlSugarRepository<UserEntity> userRepository)
        {
            _organizeRepository = organizeRepository;
            _positionRepository = positionRepository;
            _userRepository = userRepository;
        }

        #region PublicMethod

        /// <summary>
        /// 是否机构主管
        /// </summary>
        /// <param name="userId">用户ID</param>
        /// <returns></returns>
        [NonAction]
        public async Task<bool> GetIsManagerByUserId(string userId)
        {
            return await _organizeRepository.AnyAsync(o => o.EnabledMark.Equals(1) && o.DeleteMark == null && o.ManagerId == userId);
        }

        /// <summary>
        /// 获取机构列表(其他服务使用)
        /// </summary>
        /// <returns></returns>
        [NonAction]
        public async Task<List<OrganizeEntity>> GetListAsync()
        {
            return await _organizeRepository.Where(t => t.EnabledMark.Equals(1) && t.DeleteMark == null).OrderBy(o => o.SortCode).ToListAsync();
        }

        /// <summary>
        /// 获取公司列表(其他服务使用)
        /// </summary>
        /// <returns></returns>
        [NonAction]
        public async Task<List<OrganizeEntity>> GetCompanyListAsync()
        {
            return await _organizeRepository.Where(t => t.Category.Equals("company") && t.EnabledMark.Equals(1) && t.DeleteMark == null).OrderBy(o => o.SortCode).ToListAsync();
        }

        /// <summary>
        /// 下属机构
        /// </summary>
        /// <param name="organizeId">机构ID</param>
        /// <param name="isAdmin">是否管理员</param>
        /// <returns></returns>
        [NonAction]
        public async Task<string[]> GetSubsidiary(string organizeId, bool isAdmin)
        {
            var data = await _organizeRepository.Where(o => o.DeleteMark == null && o.EnabledMark.Equals(1)).OrderBy(o => o.SortCode).ToListAsync();
            if (!isAdmin)
            {
                data = data.TreeChildNode(organizeId, t => t.Id, t => t.ParentId);
            }
            return data.Select(m => m.Id).ToArray();
        }

        /// <summary>
        /// 下属机构
        /// </summary>
        /// <param name="organizeId">机构ID</param>
        /// <returns></returns>
        [NonAction]
        public async Task<List<string>> GetSubsidiary(string organizeId)
        {
            var data = await _organizeRepository.Where(o => o.DeleteMark == null && o.EnabledMark.Equals(1)).OrderBy(o => o.SortCode).ToListAsync();
            data = data.TreeChildNode(organizeId, t => t.Id, t => t.ParentId);
            return data.Select(m => m.Id).ToList();
        }

        /// <summary>
        /// 根据节点Id获取所有子节点Id集合，包含自己
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [NonAction]
        public async Task<List<string>> GetChildIdListWithSelfById(string id)
        {
            var childIdList = await _organizeRepository.Where(u => u.ParentId.Contains(id) && u.DeleteMark == null).Select(u => u.Id).ToListAsync();
            childIdList.Add(id);
            return childIdList;
        }

        /// <summary>
        /// 获取机构成员列表
        /// </summary>
        /// <param name="organizeId">机构ID</param>
        /// <returns></returns>
        [NonAction]
        public async Task<List<OrganizeMemberListOutput>> GetOrganizeMemberList(string organizeId)
        {
            var output = new List<OrganizeMemberListOutput>();
            if (organizeId.Equals("0"))
            {
                var data = await _organizeRepository.Where(o => o.ParentId.Equals("-1") && o.DeleteMark == null && o.EnabledMark.Equals(1)).OrderBy(o => o.SortCode).ToListAsync();
                data.ForEach(o =>
                {
                    output.Add(new OrganizeMemberListOutput
                    {
                        id = o.Id,
                        fullName = o.FullName,
                        enabledMark = o.EnabledMark,
                        type = o.Category,
                        icon = "icon-ym icon-ym-tree-organization3",
                        hasChildren = true,
                        isLeaf = false
                    });
                });
            }
            else
            {
                var userList = await _userRepository.Where(u => SqlFunc.ToString(u.OrganizeId).Equals(organizeId) && u.EnabledMark.Equals(1) && u.DeleteMark == null).OrderBy(o => o.SortCode).ToListAsync();
                userList.ForEach(u =>
                {
                    output.Add(new OrganizeMemberListOutput()
                    {
                        id = u.Id,
                        fullName = u.RealName + "/" + u.Account,
                        enabledMark = u.EnabledMark,
                        type = "user",
                        icon = "icon-ym icon-ym-tree-user2",
                        hasChildren = false,
                        isLeaf = true
                    });
                });
                var departmentList = await _organizeRepository.Where(o => o.ParentId.Equals(organizeId) && o.DeleteMark == null && o.EnabledMark.Equals(1)).OrderBy(o => o.SortCode).ToListAsync();
                departmentList.ForEach(o =>
                {
                    output.Add(new OrganizeMemberListOutput()
                    {
                        id = o.Id,
                        fullName = o.FullName,
                        enabledMark = o.EnabledMark,
                        type = o.Category,
                        icon = "icon-ym icon-ym-tree-department1",
                        hasChildren = true,
                        isLeaf = false
                    });
                });

            }
            return output;
        }

        /// <summary>
        /// 信息
        /// </summary>
        /// <param name="Id"></param>
        /// <returns></returns>
        public async Task<OrganizeEntity> GetInfoById(string Id)
        {
            return await _organizeRepository.SingleAsync(p => p.Id == Id);
        }

        #endregion
    }
}
