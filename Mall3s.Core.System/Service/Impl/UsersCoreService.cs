using Mall3s.Common.Extension;
using Mall3s.Common.Util;
using Mall3s.Dependency;
using Mall3s.System.Entitys.Model.Permission.User;
using Mall3s.System.Entitys.Permission;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SqlSugar;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UAParser;

namespace Mall3s.Core.System.Service.Impl
{
    public class UsersCoreService : IUsersCoreService
    {
        private readonly ISqlSugarRepository<UserEntity> _userRepository;  // 用户表仓储
        private readonly ISqlSugarRepository<PositionEntity> _positionRepository;
        private readonly ISqlSugarRepository<RoleEntity> _roleRepository;
        private readonly IOrganizeCoreService _organizeService; // 机构表仓储
        private readonly ISysConfigCoreService _sysConfigService; //系统配置仓储
        private readonly ISysCacheCoreService _sysCacheService;

        private readonly HttpContext _httpContext;

        /// <summary>
        /// 初始化一个<see cref="UsersService"/>类型的新实例
        /// </summary>
        public UsersCoreService(ISqlSugarRepository<UserEntity> userRepository, ISqlSugarRepository<PositionEntity> positionRepository, ISqlSugarRepository<RoleEntity> roleRepository, IOrganizeCoreService organizeService, ISysConfigCoreService sysConfigService, ISysCacheCoreService sysCacheService)
        {
            _userRepository = userRepository;
            _positionRepository = positionRepository;
            _roleRepository = roleRepository;
            _organizeService = organizeService;
            _sysCacheService = sysCacheService;
            _sysConfigService = sysConfigService;
            _httpContext = App.HttpContext;
        }

        #region PublicMethod

        /// <summary>
        /// 获取用户信息 根据用户ID
        /// </summary>
        /// <param name="userId">用户ID</param>
        /// <returns></returns>
        [NonAction]
        public UserEntity GetInfoByUserId(string userId)
        {
            return _userRepository.FirstOrDefault(u => u.Id == userId && u.DeleteMark == null);
        }

        /// <summary>
        /// 获取用户信息 根据用户ID
        /// </summary>
        /// <param name="userId">用户ID</param>
        /// <returns></returns>
        [NonAction]
        public async Task<UserEntity> GetInfoByUserIdAsync(string userId)
        {
            return await _userRepository.FirstOrDefaultAsync(u => u.Id == userId && u.DeleteMark == null);
        }

        /// <summary>
        /// 获取用户列表
        /// </summary>
        /// <returns></returns>
        [NonAction]
        public async Task<List<UserEntity>> GetList()
        {
            return await _userRepository.Entities.Where(u => u.DeleteMark == null).ToListAsync();
        }

        /// <summary>
        /// 获取用户信息 根据用户ID
        /// </summary>
        /// <param name="userId">用户ID</param>
        /// <param name="tenantId">租户ID</param>
        /// <returns></returns>
        [NonAction]
        public async Task<UserInfo> GetUserInfo(string userId, string tenantId)
        {
            var clent = Parser.GetDefault().Parse(_httpContext.Request.Headers["User-Agent"]);
            var ipAddress = _httpContext.GetRemoteIpAddressToIPv4();
            var ipAddressName = await NetUtil.GetLocation(ipAddress);
            var defaultPortalId = string.Empty;
            var userDataScope = new List<UserDataScope>();
            await Scoped.Create(async (_, scope) =>
            {
                var services = scope.ServiceProvider;

                var _portalService = App.GetService<IPortalCoreService>(services);
                var _organizeAdministratorService = App.GetService<IOrganizeAdministratorCoreService>(services);
                userDataScope = await _organizeAdministratorService.GetUserDataScope(userId);
                defaultPortalId = await _portalService.GetDefault();
            });
            var sysConfigInfo = await _sysConfigService.GetInfo("SysConfig", "tokentimeout");
            var data = await _userRepository.Context.Queryable<UserEntity, OrganizeEntity>((a, b) => new JoinQueryInfos(JoinType.Left, b.Id == SqlFunc.ToString(a.OrganizeId))).Where(a => a.Id == userId)
                .Select((a, b) => new UserInfo
                {
                    userId = a.Id,
                    headIcon = SqlFunc.MergeString("/api/File/Image/userAvatar/", a.HeadIcon),
                    userAccount = a.Account,
                    userName = a.RealName,
                    gender = SqlFunc.ToInt32(a.Gender),
                    organizeId = a.OrganizeId,
                    organizeName = b.FullName,
                    managerId = a.ManagerId,
                    isAdministrator = SqlFunc.IIF(a.IsAdministrator == 1, true, false),
                    portalId = SqlFunc.IIF(a.PortalId == null, defaultPortalId, a.PortalId),
                    positionId = a.PositionId,
                    roleId = a.RoleId,
                    prevLoginTime = a.PrevLogTime,
                    prevLoginIPAddress = a.PrevLogIP
                }).FirstAsync();
            data.loginTime = DateTime.Now;
            data.loginIPAddress = ipAddress;
            data.loginIPAddressName = ipAddressName;
            data.prevLoginIPAddressName = await NetUtil.GetLocation(data.prevLoginIPAddress);
            data.loginPlatForm = clent.String;
            data.subsidiary = await _organizeService.GetSubsidiary(data.organizeId, data.isAdministrator);
            data.subordinates = await this.GetSubordinates(userId);
            data.positionIds = data.positionId == null ? null : await GetPosition(data.positionId);
            data.roleIds = data.roleId == null ? null : data.roleId.Split(',').ToArray();
            data.dataScope = userDataScope;
            //根据系统配置过期时间自动过期
            await _sysCacheService.SetUserInfo(tenantId + "_" + userId, data, TimeSpan.FromMinutes(sysConfigInfo.Value.ToDouble()));

            return data;
        }

        /// <summary>
        /// 获取用户信息 根据用户账户
        /// </summary>
        /// <param name="account">用户账户</param>
        /// <returns></returns>
        [NonAction]
        public async Task<UserEntity> GetInfoByAccount(string account)
        {
            return await _userRepository.FirstOrDefaultAsync(u => u.Account == account || u.RealName == account || u.MobilePhone == account && u.DeleteMark == null);
        }

        /// <summary>
        /// 获取用户信息 根据登录信息
        /// </summary>
        /// <param name="account">用户账户</param>
        /// <param name="password">用户密码</param>
        /// <param name="isMoble">是否是手机号登录</param>
        /// <returns></returns>
        [NonAction]
        public async Task<UserEntity> GetInfoByLogin(string account, string password, bool isMoble)
        {

            if (isMoble)
                return await _userRepository.FirstOrDefaultAsync(u => u.MobilePhone == account && u.Password == password && u.DeleteMark == null);
            else
                return await _userRepository.FirstOrDefaultAsync(u => (u.Account == account || u.RealName == account) && u.Password == password && u.DeleteMark == null);
        }

        /// <summary>
        /// 根据用户姓名获取用户ID
        /// </summary>
        /// <param name="realName">用户姓名</param>
        /// <returns></returns>
        [NonAction]
        public async Task<string> GetUserIdByRealName(string realName)
        {
            return (await _userRepository.FirstOrDefaultAsync(u => u.RealName == realName && u.DeleteMark == null)).Id;
        }

        /// <summary>
        /// 获取下属
        /// </summary>
        /// <param name="managerId">主管Id</param>
        /// <returns></returns>
        [NonAction]
        public async Task<string[]> GetSubordinates(string managerId)
        {
            List<string> data = new List<string>();
            var userIds = await _userRepository.Where(m => m.ManagerId == managerId && m.DeleteMark == null).OrderBy(o => o.SortCode).Select(m => m.Id).ToListAsync();
            data.AddRange(userIds);
            data.AddRange(await GetInfiniteSubordinats(userIds.ToArray()));
            return data.ToArray();
        }


        private async Task<List<string>> GetInfiniteSubordinats(string[] parentIds)
        {
            List<string> data = new List<string>();
            if (parentIds.ToList().Count > 0)
            {
                var userIds = await _userRepository.Context.Queryable<UserEntity>().In(it => it.ManagerId, parentIds).Where(it => it.DeleteMark == null).OrderBy(it => it.SortCode).Select(it => it.Id).ToListAsync();
                data.AddRange(userIds);
                //data.AddRange(await GetInfiniteSubordinats(userIds.ToArray()));
            }
            return data;

        }

        /// <summary>
        /// 获取下属
        /// </summary>
        /// <param name="managerId">主管Id</param>
        /// <returns></returns>
        [NonAction]
        public async Task<List<string>> GetSubordinatesAsync(string managerId)
        {
            return await _userRepository.Where(m => m.ManagerId == managerId && m.DeleteMark == null).OrderBy(o => o.SortCode).Select(s => s.Id).ToListAsync();
        }

        /// <summary>
        /// 下属机构
        /// </summary>
        /// <param name="organizeId">机构ID</param>
        /// <returns></returns>
        [NonAction]
        public async Task<List<string>> GetSubOrganizeIds(string organizeId)
        {
            var data = await _organizeService.GetListAsync();
            data = data.TreeChildNode(organizeId, t => t.Id, t => t.ParentId);
            return data.Select(m => m.Id).ToList();
        }

        /// <summary>
        /// 获取下属
        /// </summary>
        /// <param name="userId">用户ID</param>
        /// <returns></returns>
        [NonAction]
        public async Task<List<string>> GetSubordinateId(string userId)
        {
            var data = await _userRepository.Where(u => u.ManagerId == userId && u.DeleteMark == null).OrderBy(o => o.SortCode).ToListAsync();
            return data.Select(m => m.Id).ToList();
        }

        /// <summary>
        /// 是否存在机构用户
        /// </summary>
        /// <param name="organizeId">机构ID</param>
        /// <returns></returns>
        [NonAction]
        public async Task<bool> ExistOrganizeUser(string organizeId)
        {
            return await _userRepository.AnyAsync(u => u.OrganizeId.Equals(organizeId) && u.DeleteMark == null);
        }

        /// <summary>
        /// 获取用户名
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        [NonAction]
        public async Task<string> GetUserName(string userId)
        {
            var entity = await _userRepository.FirstOrDefaultAsync(x => x.Id == userId && x.DeleteMark == null);
            if (entity.IsNullOrEmpty())
                return "";
            return entity.RealName + "/" + entity.Account;
        }

        /// <summary>
        /// 获取当前用户岗位信息
        /// </summary>
        /// <param name="PositionIds"></param>
        /// <returns></returns>
        [NonAction]
        public async Task<List<PositionInfo>> GetPosition(string PositionIds)
        {
            var ids = PositionIds.Split(",");
            return await _positionRepository.Entities.In(it => it.Id, ids).Select(it => new { id = it.Id, name = it.FullName }).MergeTable().Select<PositionInfo>().ToListAsync();
        }
        #endregion
    }

}
