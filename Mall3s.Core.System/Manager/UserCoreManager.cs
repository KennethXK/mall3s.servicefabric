using Mall3s.Common.Const;
using Mall3s.Common.Enum;
using Mall3s.Core.System.Service;
using Mall3s.System.Entitys.Model.Permission.User;
using Mall3s.System.Entitys.Permission;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mall3s.Core.System.Manager
{
    /// <summary>
    /// 用户管理
    /// </summary>
    public class UserCoreManager : IUserCoreManager
    {
        private readonly IUsersCoreService _userService; // 用户服务

        private readonly ISysCacheCoreService _sysCacheService;

        private readonly HttpContext _httpContext;



        /// <summary>
        /// 
        /// </summary>
        /// <param name="userService"></param>
        /// <param name="sysCacheService"></param>
        /// <param name="httpContextAccessor"></param>
        public UserCoreManager(IUsersCoreService userService, ISysCacheCoreService sysCacheService)
        {
            _userService = userService;
            _sysCacheService = sysCacheService;
            _httpContext = App.HttpContext;
        }

        /// <summary>
        /// 用户信息
        /// </summary>
        public UserEntity User
        {
            get => _userService.GetInfoByUserId(UserId);
        }

        /// <summary>
        /// 用户ID
        /// </summary>
        public string UserId
        {
            get => _httpContext.User.FindFirst(ClaimConst.CLAINM_USERID)?.Value;
        }

        /// <summary>
        /// 用户账号
        /// </summary>
        public string Account
        {
            get => _httpContext.User.FindFirst(ClaimConst.CLAINM_ACCOUNT)?.Value;
        }

        /// <summary>
        /// 用户昵称
        /// </summary>
        public string RealName
        {
            get => _httpContext.User.FindFirst(ClaimConst.CLAINM_REALNAME)?.Value;
        }

        /// <summary>
        /// 当前用户 token
        /// </summary>
        public string ToKen
        {
            get => _httpContext.Request.Headers["Authorization"];
        }

        public string TenantId
        {
            get => _httpContext.User.FindFirst(ClaimConst.TENANT_ID)?.Value;
        }

        /// <summary>
        /// 是否是管理员
        /// </summary>
        public bool IsAdministrator
        {
            get => _httpContext.User.FindFirst(ClaimConst.CLAINM_ADMINISTRATOR)?.Value == ((int)AccountType.Administrator).ToString();
        }


        /// <summary>
        /// 获取用户登录信息
        /// </summary>
        /// <returns></returns>
        public async Task<UserInfo> GetUserInfo()
        {
            var data = new UserInfo();
            var userCache = await _sysCacheService.GetUserInfo(TenantId + "_" + UserId);
            if (userCache == null)
            {
                data = await _userService.GetUserInfo(UserId, TenantId);
            }
            else
            {
                data = userCache;
            }
            return data;
        }
    }
}
