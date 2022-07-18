using Mall3s.Common.Extension;
using Mall3s.Core.System.Manager;
using Mall3s.Core.System.Service;
using Mall3s.System.Entitys.Permission;
using Mall3s.System.Entitys.System;
using Mall3s.VisualDev.Entitys;
using Microsoft.AspNetCore.Mvc;
using SqlSugar;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mall3s.Core.VisualDev.Service.Impl
{
    public class PortalCoreService : IPortalCoreService
    {
        private readonly ISqlSugarRepository<PortalEntity> _portalRepository;
        private readonly ISqlSugarRepository<AuthorizeEntity> _authorizeRepository; //权限操作表仓储
        private readonly ISqlSugarRepository<RoleEntity> _roleRepository;
        private readonly IUserCoreManager _userManager;
        private readonly IFileCoreService _fileService;

        /// <summary>
        /// 初始化一个<see cref="PortalService"/>类型的新实例
        /// </summary>
        public PortalCoreService(ISqlSugarRepository<PortalEntity> portalRepository,
            ISqlSugarRepository<RoleEntity> roleRepository,
            IUserCoreManager userManager, ISqlSugarRepository<AuthorizeEntity> authorizeRepository, 
            IFileCoreService fileService)
        {
            _portalRepository = portalRepository;
            _userManager = userManager;
            _authorizeRepository = authorizeRepository;
            _roleRepository = roleRepository;
            _fileService = fileService;
        }

        #region PublicMethod

        /// <summary>
        /// 获取默认
        /// </summary>
        /// <returns></returns>
        [NonAction]
        public async Task<string> GetDefault()
        {
            var user = _userManager.User;
            if (!user.IsAdministrator.ToBool())
            {
                if (!string.IsNullOrEmpty(user.RoleId))
                {
                    var roleIds = user.RoleId.Split(',');
                    var roleId = await _roleRepository.Entities.In(r => r.Id, roleIds).Where(r => r.EnabledMark.Equals(1) && r.DeleteMark == null).Select(r => r.Id).ToListAsync();
                    var items = await _authorizeRepository.Entities.In(a => a.ObjectId, roleId)
                        .Where(a => a.ItemType == "portal")
                        .GroupBy(it => new { it.ItemId })
                        .Select(it => new { it.ItemId })
                        .ToListAsync();
                    if (items.Count == 0) return null;
                    var portalList = await _portalRepository.Entities.In(p => p.Id, items.Select(it => it.ItemId).ToArray()).Where(p => p.EnabledMark.Equals(1) && p.DeleteMark == null).OrderBy(o => o.SortCode).Select(s => s.Id).ToListAsync();
                    return portalList.FirstOrDefault();
                }
                return null;
            }
            else
            {
                var portalList = await _portalRepository.Entities.Where(p => p.EnabledMark.Equals(1) && p.DeleteMark == null).OrderBy(o => o.SortCode).Select(s => s.Id).ToListAsync();
                return portalList.FirstOrDefault();
            }
        }

        #endregion
    }
}
