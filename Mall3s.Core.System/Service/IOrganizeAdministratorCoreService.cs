using Mall3s.System.Entitys.Model.Permission.User;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mall3s.Core.System.Service
{
    /// <summary>
    /// 分级管理
    /// 版 本：V3.2.5
    /// 版 权：mall3s开发
    /// 作 者：Mall3s开发平台组
    /// 日 期：2021.09.27 
    /// </summary>
    public interface IOrganizeAdministratorCoreService
    {
        /// <summary>
        /// 获取用户数据范围
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        Task<List<UserDataScope>> GetUserDataScope(string userId);
    }
}
