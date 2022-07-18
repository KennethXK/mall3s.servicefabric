using Mall3s.System.Entitys.Model.Permission.User;
using Mall3s.System.Entitys.Permission;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mall3s.Core.System.Service
{
    /// <summary>
    /// 业务契约：用户信息
    /// </summary>
    public interface IUsersCoreService
    {
        /// <summary>
        /// 获取用户信息 根据用户ID
        /// </summary>
        /// <param name="userId">用户ID</param>
        /// <returns></returns>
        UserEntity GetInfoByUserId(string userId);


        /// <summary>
        /// 获取用户信息 根据用户ID
        /// </summary>
        /// <param name="userId">用户ID</param>
        /// <returns></returns>
        Task<UserInfo> GetUserInfo(string userId, string tenantId);

        /// <summary>
        /// 获取用户列表
        /// </summary>
        /// <returns></returns>
        Task<List<UserEntity>> GetList();
        /// <summary>
        /// 获取当前用户岗位信息
        /// </summary>
        /// <param name="PositionIds"></param>
        /// <returns></returns>

        Task<List<PositionInfo>> GetPosition(string PositionIds);
        /// <summary>
        /// 获取用户信息 根据用户ID
        /// </summary>
        /// <param name="userId">用户ID</param>
        /// <returns></returns>
        Task<UserEntity> GetInfoByUserIdAsync(string userId);

        /// <summary>
        /// 获取用户名称
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        Task<string> GetUserName(string userId);

    }
}
