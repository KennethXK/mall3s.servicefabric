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
    /// 业务实现：部门管理
    /// </summary>
    public class DepartmentCoreService : IDepartmentCoreService
    {
        private readonly ISqlSugarRepository<OrganizeEntity> _departmentRepository;
        private readonly ISqlSugarRepository<PositionEntity> _positionRepository;
        private readonly IOrganizeCoreService _organizeService;
        private readonly ISqlSugarRepository<UserEntity> _userRepository;
        private readonly SqlSugarScope db;// 核心对象：拥有完整的SqlSugar全部功能
        private readonly ISysConfigCoreService _sysConfigService;
        /// <summary>
        /// 初始化一个<see cref="DepartmentService"/>类型的新实例
        /// </summary>
        /// <param name="departmentRepository"></param>
        /// <param name="positionRepository"></param>
        /// <param name="userRepository"></param>
        /// <param name="organizeService"></param>
        /// <param name="sysConfigService"></param>
        /// <param name="synThirdInfoService"></param>
        public DepartmentCoreService(ISqlSugarRepository<OrganizeEntity> departmentRepository,
            ISqlSugarRepository<PositionEntity> positionRepository,
            ISqlSugarRepository<UserEntity> userRepository,
            IOrganizeCoreService organizeService,
            ISysConfigCoreService sysConfigService)
        {
            _departmentRepository = departmentRepository;
            _positionRepository = positionRepository;
            _userRepository = userRepository;
            _organizeService = organizeService;
            db = departmentRepository.Context;
            _sysConfigService = sysConfigService;
        }

        #region PublicMethod

        /// <summary>
        /// 获取部门列表(其他服务使用)
        /// </summary>
        /// <returns></returns>
        [NonAction]
        public async Task<List<OrganizeEntity>> GetListAsync()
        {
            return await _departmentRepository.Where(t => t.Category.Equals("department") && t.EnabledMark.Equals(1) && t.DeleteMark == null).OrderBy(o => o.SortCode).ToListAsync();
        }

        /// <summary>
        /// 部门名称
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [NonAction]
        public string GetDepName(string id)
        {
            var entity = _departmentRepository.FirstOrDefault(x => x.Id == id && x.Category == "department" && x.EnabledMark.Equals(1) && x.DeleteMark == null);
            var name = entity == null ? "" : entity.FullName;
            return name;
        }

        /// <summary>
        /// 公司名称
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [NonAction]
        public string GetComName(string id)
        {
            var name = "";
            var entity = _departmentRepository.FirstOrDefault(x => x.Id == id && x.EnabledMark.Equals(1) && x.DeleteMark == null);
            if (entity == null)
            {
                return name;
            }
            else
            {
                if (entity.Category == "company")
                {
                    return entity.FullName;
                }
                else
                {
                    var pEntity = _departmentRepository.FirstOrDefault(x => x.Id == entity.ParentId && x.EnabledMark.Equals(1) && x.DeleteMark == null);
                    return GetComName(pEntity.Id);
                }
            }
        }

        /// <summary>
        /// 公司id
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [NonAction]
        public string GetCompanyId(string id)
        {
            var entity = _departmentRepository.FirstOrDefault(x => x.Id == id && x.EnabledMark.Equals(1) && x.DeleteMark == null);
            if (entity == null)
            {
                return "";
            }
            else
            {
                if (entity.Category == "company")
                {
                    return entity.Id;
                }
                else
                {
                    var pEntity = _departmentRepository.FirstOrDefault(x => x.Id == entity.ParentId && x.EnabledMark.Equals(1) && x.DeleteMark == null);
                    return GetCompanyId(pEntity.Id);
                }
            }
        }

        #endregion
    }
}
