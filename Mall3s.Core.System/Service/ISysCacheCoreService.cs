using Mall3s.System.Entitys.Model.Permission.User;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mall3s.Core.System.Service
{
    /// <summary>
    /// 缓存管理
    /// 版 本：V3.2
    /// 版 权：mall3s开发
    /// 作 者：Mall3s开发平台组
    /// 日 期：2021-06-01 
    /// </summary>
    public interface ISysCacheCoreService
    {
        /// <summary>
        /// 用户登录信息
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="userInfo"></param>
        /// <returns></returns>
        Task<bool> SetUserInfo(string userId, UserInfo userInfo, TimeSpan timeSpan);

        /// <summary>
        /// 用户是否存在
        /// </summary>
        /// <param name="userId">用户ID</param>
        /// <returns></returns>
        Task<bool> ExistsUserAsync(string userId);

        /// <summary>
        /// 缓存数据范围（机构Id集合）
        /// </summary>
        /// <param name="userId">用户ID</param>
        /// <param name="dataScopes">数据作用域</param>
        /// <returns></returns>
        Task SetDataScope(long userId, string dataScopes);

        /// <summary>
        /// 获取数据范围缓存（机构Id）
        /// </summary>
        /// <param name="userId">用户ID</param>
        /// <returns></returns>
        Task<string> GetDataScope(string userId);

        /// <summary>
        /// 获取验证码
        /// </summary>
        /// <param name="timestamp">时间戳</param>
        /// <returns></returns>
        Task<string> GetCode(string timestamp);

        /// <summary>
        /// 验证码
        /// </summary>
        /// <param name="timestamp">时间戳</param>
        /// <param name="code">验证码</param>
        /// <param name="timeSpan">过期时间</param>
        /// <returns></returns>
        Task<bool> SetCode(string timestamp, string code, TimeSpan timeSpan);

        /// <summary>
        /// 删除验证码
        /// </summary>
        /// <param name="timestamp">时间戳</param>
        /// <returns></returns>
        Task<bool> DelCode(string timestamp);

        /// <summary>
        /// 删除用户登录信息缓存
        /// </summary>
        /// <param name="userId">用户ID</param>
        /// <returns></returns>
        Task<bool> DelUserInfo(string userId);

        /// <summary>
        /// 删除岗位列表缓存
        /// </summary>
        /// <param name="userId">用户ID</param>
        /// <returns></returns>
        Task<bool> DelPosition(string userId);

        /// <summary>
        /// 删除岗位列表缓存
        /// </summary>
        /// <param name="userId">用户ID</param>
        /// <returns></returns>
        Task<bool> DelRole(string userId);

        /// <summary>
        /// 删除在线用户ID
        /// </summary>
        /// <param name="userId">用户ID</param>
        /// <returns></returns>
        bool DelOnlineUser(string userId);

        /// <summary>
        /// 获取所有缓存关键字
        /// </summary>
        /// <returns></returns>
        List<string> GetAllCacheKeys();

        /// <summary>
        /// 删除指定关键字缓存
        /// </summary>
        /// <param name="key">键</param>
        /// <returns></returns>
        bool Del(string key);

        /// <summary>
        /// 删除指定关键字缓存
        /// </summary>
        /// <param name="key">键</param>
        /// <returns></returns>
        Task<bool> DelAsync(string key);

        /// <summary>
        /// 删除指定关键字数组缓存
        /// </summary>
        /// <param name="key">键</param>
        /// <returns></returns>
        Task<bool> DelAsync(string[] key);

        /// <summary>
        /// 删除某特征关键字缓存
        /// </summary>
        /// <param name="key">键</param>
        /// <returns></returns>
        Task<bool> DelByPatternAsync(string key);

        /// <summary>
        /// 设置缓存
        /// </summary>
        /// <param name="key">键</param>
        /// <param name="value">值</param>
        /// <returns></returns>
        bool Set(string key, object value);

        /// <summary>
        /// 设置缓存
        /// </summary>
        /// <param name="key">键</param>
        /// <param name="value">值</param>
        /// <param name="timeSpan">过期时间</param>
        /// <returns></returns>
        bool Set(string key, object value, TimeSpan timeSpan);

        /// <summary>
        /// 设置缓存
        /// </summary>
        /// <param name="key">键</param>
        /// <param name="value">值</param>
        /// <returns></returns>
        Task<bool> SetAsync(string key, object value);

        /// <summary>
        /// 设置缓存
        /// </summary>
        /// <param name="key">键</param>
        /// <param name="value">值</param>
        /// <param name="timeSpan">过期时间</param>
        /// <returns></returns>
        Task<bool> SetAsync(string key, object value, TimeSpan timeSpan);

        /// <summary>
        /// 获取缓存
        /// </summary>
        /// <param name="key">键</param>
        /// <returns></returns>
        string Get(string key);

        /// <summary>
        /// 获取缓存
        /// </summary>
        /// <param name="key">键</param>
        /// <returns></returns>
        Task<string> GetAsync(string key);

        /// <summary>
        /// 获取缓存
        /// </summary>
        /// <typeparam name="T">对象</typeparam>
        /// <param name="key">键</param>
        /// <returns></returns>
        T Get<T>(string key);

        /// <summary>
        /// 获取缓存
        /// </summary>
        /// <typeparam name="T">对象</typeparam>
        /// <param name="key">键</param>
        /// <returns></returns>
        Task<T> GetAsync<T>(string key);

        /// <summary>
        /// 获取缓存过期时间
        /// </summary>
        /// <param name="key">键</param>
        /// <returns></returns>
        DateTime GetCacheOutTime(string key);

        /// <summary>
        /// 检查给定 key 是否存在
        /// </summary>
        /// <param name="key">键</param>
        /// <returns></returns>
        bool Exists(string key);

        /// <summary>
        /// 异步检查给定 key 是否存在
        /// </summary>
        /// <param name="key">键</param>
        /// <returns></returns>
        Task<bool> ExistsAsync(string key);

        /// <summary>
        /// 获取用户登录信息
        /// </summary>
        /// <param name="userId">用户ID</param>
        /// <returns></returns>
        Task<UserInfo> GetUserInfo(string userId);
    }
}
