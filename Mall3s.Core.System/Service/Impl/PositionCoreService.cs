using Mall3s.Common.Extension;
using Mall3s.Core.System.Manager;
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
    /// 业务实现：岗位管理
    /// 版 本：V3.2.0
    /// 版 权：mall3s开发
    /// 作 者：Mall3s开发平台组
    /// 日 期：2021.06.07 
    /// </summary>
    public class PositionCoreService : IPositionCoreService
    {
        private readonly ISqlSugarRepository<PositionEntity> _positionRepository;
        private readonly ISysCacheCoreService _sysCacheService;
        private readonly IOrganizeCoreService _organizeService;
        private readonly IUserCoreManager _userManager;

        /// <summary>
        /// 初始化一个<see cref="PositionService"/>类型的新实例
        /// </summary>
        public PositionCoreService(ISqlSugarRepository<PositionEntity> positionRepository, IOrganizeCoreService organizeService, ISysCacheCoreService sysCacheService, IUserCoreManager userManager)
        {
            _organizeService = organizeService;
            _positionRepository = positionRepository;
            _sysCacheService = sysCacheService;
            _userManager = userManager;
        }

        #region PublicMethod

        /// <summary>
        /// 获取信息
        /// </summary>
        /// <param name="id">获取信息</param>
        /// <returns></returns>
        [NonAction]
        public async Task<PositionEntity> GetInfoById(string id)
        {
            return await _positionRepository.SingleAsync(p => p.Id == id);
        }

        /// <summary>
        /// 获取岗位列表
        /// </summary>
        /// <returns></returns>
        [NonAction]
        public async Task<List<PositionEntity>> GetListAsync()
        {
            return await _positionRepository.Entities.Where(u => u.DeleteMark == null).ToListAsync();
        }

        /// <summary>
        /// 名称
        /// </summary>
        /// <param name="ids"></param>
        /// <returns></returns>
        [NonAction]
        public string GetName(string ids)
        {
            if (ids.IsNullOrEmpty())
            {
                return "";
            }
            var idList = ids.Split(",").ToList();
            var nameList = new List<string>();
            var roleList = _positionRepository.Entities.Where(x => x.DeleteMark == null && x.EnabledMark == 1).ToList();
            foreach (var item in idList)
            {
                var info = roleList.Find(x => x.Id == item);
                if (info != null && info.FullName.IsNotEmptyOrNull())
                {
                    nameList.Add(info.FullName);
                }
            }
            var name = string.Join(",", nameList);
            return name;
        }
        #endregion
    }
}
