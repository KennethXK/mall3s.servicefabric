﻿using Mall3s.System.Entitys.Model.Permission.User;
using Mall3s.System.Entitys.Permission;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mall3s.Core.System.Manager
{
    /// <summary>
     /// 用户管理抽象
     /// </summary>
    public interface IUserCoreManager
    {
        /// <summary>
        /// 用户编号
        /// </summary>
        string UserId { get; }

        /// <summary>
        /// 租户ID
        /// </summary>
        string TenantId { get; }

        /// <summary>
        /// 用户账号
        /// </summary>
        string Account { get; }

        /// <summary>
        /// 用户昵称
        /// </summary>
        string RealName { get; }

        /// <summary>
        /// 当前用户 ToKen
        /// </summary>
        string ToKen { get; }

        /// <summary>
        /// 是否管理员
        /// </summary>
        bool IsAdministrator { get; }

        /// <summary>
        /// 用户信息
        /// </summary>
        UserEntity User { get; }

        /// <summary>
        /// 获取用户登录信息
        /// </summary>
        /// <returns></returns>
        Task<UserInfo> GetUserInfo();
    }
}
