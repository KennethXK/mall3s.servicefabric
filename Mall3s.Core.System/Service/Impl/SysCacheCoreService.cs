using Mall3s.Common.Cache;
using Mall3s.Common.Const;
using Mall3s.Dependency;
using Mall3s.Message.Entitys.Model.IM;
using Mall3s.System.Entitys.Dto.Permission.Position;
using Mall3s.System.Entitys.Dto.Permission.Role;
using Mall3s.System.Entitys.Model.Permission.User;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mall3s.Core.System.Service.Impl
{
    public class SysCacheCoreService : ISysCacheCoreService
    {
        private readonly ICache _cache;
        private readonly CacheOptions _cacheOptions;

        /// <summary>
        /// 初始化一个<see cref="SysCacheService"/>类型的新实例
        /// </summary>
        /// <param name="cacheOptions"></param>
        /// <param name="resolveNamed"></param>
        public SysCacheCoreService(IOptions<CacheOptions> cacheOptions, Func<string, ISingleton, object> resolveNamed)
        {
            _cacheOptions = cacheOptions.Value;
            _cache = resolveNamed(_cacheOptions.CacheType.ToString(), default) as ICache;
        }

        #region PulicMethod

        /// <summary>
        /// 用户是否存在
        /// </summary>
        /// <param name="userId">用户ID</param>
        /// <returns></returns>
        [NonAction]
        public async Task<bool> ExistsUserAsync(string userId)
        {
            var cacheKey = CommonConst.CACHE_KEY_USER + $"{userId}";
            return await ExistsAsync(cacheKey);
        }

        /// <summary>
        /// 获取数据范围缓存（机构Id）
        /// </summary>
        /// <param name="userId">用户ID</param>
        /// <returns></returns>
        [NonAction]
        public async Task<string> GetDataScope(string userId)
        {
            var cacheKey = CommonConst.CACHE_KEY_DATASCOPE + $"{userId}";
            return await GetAsync<string>(cacheKey);
        }

        /// <summary>
        /// 缓存数据范围（机构Id集合）
        /// </summary>
        /// <param name="userId">用户ID</param>
        /// <param name="dataScopes">数据作用域</param>
        /// <returns></returns>
        [NonAction]
        public async Task SetDataScope(long userId, string dataScopes)
        {
            var cacheKey = CommonConst.CACHE_KEY_DATASCOPE + $"{userId}";
            await SetAsync(cacheKey, dataScopes);
        }

        /// <summary>
        /// 获取验证码
        /// </summary>
        /// <param name="timestamp">时间戳</param>
        /// <returns></returns>
        [NonAction]
        public async Task<string> GetCode(string timestamp)
        {
            var cacheKey = CommonConst.CACHE_KEY_CODE + $"{timestamp}";
            return await GetAsync<string>(cacheKey);
        }

        /// <summary>
        /// 验证码
        /// </summary>
        /// <param name="timestamp">时间戳</param>
        /// <param name="code">验证码</param>
        /// <param name="timeSpan">过期时间</param>
        /// <returns></returns>
        [NonAction]
        public async Task<bool> SetCode(string timestamp, string code, TimeSpan timeSpan)
        {
            var cacheKey = CommonConst.CACHE_KEY_CODE + $"{timestamp}";
            return await SetAsync(cacheKey, code, timeSpan);
        }

        /// <summary>
        /// 删除验证码
        /// </summary>
        /// <param name="timestamp">时间戳</param>
        /// <returns></returns>
        [NonAction]
        public Task<bool> DelCode(string timestamp)
        {
            var cacheKey = CommonConst.CACHE_KEY_CODE + $"{timestamp}";
            DelAsync(cacheKey);
            return Task.FromResult(true);
        }

        /// <summary>
        /// 获取许可
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        [NonAction]
        public async Task<List<dynamic>> GetPermission(string userId)
        {
            var cacheKey = CommonConst.CACHE_KEY_PERMISSION + $"{userId}";
            return await GetAsync<List<dynamic>>(cacheKey);
        }

        /// <summary>
        /// 用户登录信息
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="userInfo"></param>
        /// <returns></returns>
        [NonAction]
        public async Task<bool> SetPermission(string userId, UserInfo userInfo)
        {
            var cacheKey = CommonConst.CACHE_KEY_PERMISSION + $"{userId}";
            return await SetAsync(cacheKey, userInfo);
        }

        /// <summary>
        /// 获取用户登录信息
        /// </summary>
        /// <param name="userId">用户ID</param>
        /// <returns></returns>
        [NonAction]
        public async Task<UserInfo> GetUserInfo(string userId)
        {
            var cacheKey = CommonConst.CACHE_KEY_USER + $"{userId}";
            return await GetAsync<UserInfo>(cacheKey);
        }

        /// <summary>
        /// 用户登录信息
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="userInfo"></param>
        /// <param name="timeSpan"></param>
        /// <returns></returns>
        [NonAction]
        public async Task<bool> SetUserInfo(string userId, UserInfo userInfo, TimeSpan timeSpan)
        {
            var cacheKey = CommonConst.CACHE_KEY_USER + $"{userId}";
            return await SetAsync(cacheKey, userInfo);
        }

        /// <summary>
        /// 删除用户登录信息缓存
        /// </summary>
        /// <param name="userId">用户ID</param>
        /// <returns></returns>
        public Task<bool> DelUserInfo(string userId)
        {
            var cacheKey = CommonConst.CACHE_KEY_USER + $"{userId}";
            DelAsync(cacheKey);
            return Task.FromResult(true);
        }

        /// <summary>
        /// 获取岗位列表信息
        /// </summary>
        /// <param name="userId">用户ID</param>
        /// <returns></returns>
        [NonAction]
        public List<PositionCacheListOutput> GetPositionList(string userId)
        {
            var cacheKey = CommonConst.CACHE_KEY_POSITION + $"{userId}";
            return Get<List<PositionCacheListOutput>>(cacheKey);
        }

        /// <summary>
        /// 保存岗位列表信息
        /// </summary>
        /// <param name="userId">用户ID</param>
        /// <param name="positionList">岗位列表</param>
        /// <param name="timeSpan">过期时间</param>
        /// <returns></returns>
        [NonAction]
        public bool SetPositionList(string userId, List<PositionCacheListOutput> positionList, TimeSpan timeSpan)
        {
            var cacheKey = CommonConst.CACHE_KEY_POSITION + $"{userId}";
            return Set(cacheKey, positionList, timeSpan);
        }

        /// <summary>
        /// 删除岗位列表缓存
        /// </summary>
        /// <param name="userId">用户ID</param>
        /// <returns></returns>
        public Task<bool> DelPosition(string userId)
        {
            var cacheKey = CommonConst.CACHE_KEY_POSITION + $"{userId}";
            DelAsync(cacheKey);
            return Task.FromResult(true);
        }

        /// <summary>
        /// 获取角色列表信息
        /// </summary>
        /// <param name="userId">用户ID</param>
        /// <returns></returns>
        [NonAction]
        public List<RoleCacheListOutput> GetRoleList(string userId)
        {
            var cacheKey = CommonConst.CACHE_KEY_ROLE + $"{userId}";
            return Get<List<RoleCacheListOutput>>(cacheKey);
        }

        /// <summary>
        /// 保存角色列表信息
        /// </summary>
        /// <param name="userId">用户ID</param>
        /// <param name="roleList">角色列表</param>
        /// <param name="timeSpan">过期时间</param>
        /// <returns></returns>
        [NonAction]
        public bool SetRoleList(string userId, List<RoleCacheListOutput> roleList, TimeSpan timeSpan)
        {
            var cacheKey = CommonConst.CACHE_KEY_ROLE + $"{userId}";
            return Set(cacheKey, roleList, timeSpan);
        }

        /// <summary>
        /// 删除角色列表缓存
        /// </summary>
        /// <param name="userId">用户ID</param>
        /// <returns></returns>
        public Task<bool> DelRole(string userId)
        {
            var cacheKey = CommonConst.CACHE_KEY_ROLE + $"{userId}";
            DelAsync(cacheKey);
            return Task.FromResult(true);
        }

        /// <summary>
        /// 获取在线用户列表
        /// </summary>
        /// <param name="tenantId">租户ID</param>
        /// <returns></returns>
        [NonAction]
        public List<UserOnlineModel> GetOnlineUserList(string tenantId)
        {
            var cacheKey = CommonConst.CACHE_KEY_ONLINE_USER + $"{tenantId}";
            return Get<List<UserOnlineModel>>(cacheKey);
        }

        /// <summary>
        /// 保存在线用户列表
        /// </summary>
        /// <param name="tenantId">租户ID</param>
        /// <param name="onlineList">在线用户列表</param>
        /// <returns></returns>
        [NonAction]
        public bool SetOnlineUserList(string tenantId, List<UserOnlineModel> onlineList)
        {
            var cacheKey = CommonConst.CACHE_KEY_ONLINE_USER + $"{tenantId}";
            return Set(cacheKey, onlineList);
        }

        /// <summary>
        /// 删除在线用户ID
        /// </summary>
        /// <param name="Id">用户ID</param>
        /// <returns></returns>
        [NonAction]
        public bool DelOnlineUser(string Id)
        {
            var tenantId = Id.Split('_').ToList().First();
            var userId = Id.Split('_').ToList().Last();
            var cacheKey = CommonConst.CACHE_KEY_ONLINE_USER + $"{tenantId}";
            var list = Get<List<UserOnlineModel>>(cacheKey);
            var online = list.Find(it => it.userId == userId);
            list.RemoveAll((x) => x.connectionId == online.connectionId);
            return Set(cacheKey, list);
        }

        /// <summary>
        /// 获取所有缓存关键字
        /// </summary>
        /// <returns></returns>
        [NonAction]
        public List<string> GetAllCacheKeys()
        {
            var cacheItems = _cache.GetAllKeys();
            if (cacheItems == null) return new List<string>();
            return cacheItems.Where(u => !u.ToString().StartsWith("mini-profiler")).Select(u => u).ToList();
        }

        /// <summary>
        /// 删除指定关键字缓存
        /// </summary>
        /// <param name="key">键</param>
        /// <returns></returns>
        [NonAction]
        public bool Del(string key)
        {
            _cache.Del(key);
            return true;
        }

        /// <summary>
        /// 删除指定关键字缓存
        /// </summary>
        /// <param name="key">键</param>
        /// <returns></returns>
        [NonAction]
        public Task<bool> DelAsync(string key)
        {
            _cache.DelAsync(key);
            return Task.FromResult(true);
        }

        /// <summary>
        /// 删除指定关键字数组缓存
        /// </summary>
        /// <param name="key">键</param>
        /// <returns></returns>
        [NonAction]
        public Task<bool> DelAsync(string[] key)
        {
            _cache.DelAsync(key);
            return Task.FromResult(true);
        }

        /// <summary>
        /// 删除某特征关键字缓存
        /// </summary>
        /// <param name="key">键</param>
        /// <returns></returns>
        [NonAction]
        public Task<bool> DelByPatternAsync(string key)
        {
            _cache.DelByPatternAsync(key);
            return Task.FromResult(true);
        }

        /// <summary>
        /// 设置缓存
        /// </summary>
        /// <param name="key">键</param>
        /// <param name="value">值</param>
        /// <returns></returns>
        [NonAction]
        public bool Set(string key, object value)
        {
            return _cache.Set(key, value);
        }

        /// <summary>
        /// 设置缓存
        /// </summary>
        /// <param name="key">键</param>
        /// <param name="value">值</param>
        /// <param name="timeSpan">过期时间</param>
        /// <returns></returns>
        [NonAction]
        public bool Set(string key, object value, TimeSpan timeSpan)
        {
            return _cache.Set(key, value, timeSpan);
        }

        /// <summary>
        /// 设置缓存
        /// </summary>
        /// <param name="key">键</param>
        /// <param name="value">值</param>
        /// <returns></returns>
        [NonAction]
        public async Task<bool> SetAsync(string key, object value)
        {
            return await _cache.SetAsync(key, value);
        }

        /// <summary>
        /// 设置缓存
        /// </summary>
        /// <param name="key">键</param>
        /// <param name="value">值</param>
        /// <param name="timeSpan">过期时间</param>
        /// <returns></returns>
        [NonAction]
        public async Task<bool> SetAsync(string key, object value, TimeSpan timeSpan)
        {
            return await _cache.SetAsync(key, value, timeSpan);
        }

        /// <summary>
        /// 获取缓存
        /// </summary>
        /// <param name="key">键</param>
        /// <returns></returns>
        [NonAction]
        public string Get(string key)
        {
            return _cache.Get(key);
        }

        /// <summary>
        /// 获取缓存
        /// </summary>
        /// <param name="key">键</param>
        /// <returns></returns>
        [NonAction]
        public async Task<string> GetAsync(string key)
        {
            return await _cache.GetAsync(key);
        }

        /// <summary>
        /// 获取缓存
        /// </summary>
        /// <typeparam name="T">对象</typeparam>
        /// <param name="key">键</param>
        /// <returns></returns>
        [NonAction]
        public T Get<T>(string key)
        {
            return _cache.Get<T>(key);
        }

        /// <summary>
        /// 获取缓存
        /// </summary>
        /// <typeparam name="T">对象</typeparam>
        /// <param name="key">键</param>
        /// <returns></returns>
        [NonAction]
        public Task<T> GetAsync<T>(string key)
        {
            return _cache.GetAsync<T>(key);
        }

        /// <summary>
        /// 获取缓存过期时间
        /// </summary>
        /// <param name="key">键</param>
        /// <returns></returns>
        [NonAction]
        public DateTime GetCacheOutTime(string key)
        {
            return _cache.GetCacheOutTime(key);
        }

        /// <summary>
        /// 检查给定 key 是否存在
        /// </summary>
        /// <param name="key">键</param>
        /// <returns></returns>
        [NonAction]
        public bool Exists(string key)
        {
            return _cache.Exists(key);
        }

        /// <summary>
        /// 检查给定 key 是否存在
        /// </summary>
        /// <param name="key">键</param>
        /// <returns></returns>
        [NonAction]
        public Task<bool> ExistsAsync(string key)
        {
            return _cache.ExistsAsync(key);
        }

        #endregion
    }
}
