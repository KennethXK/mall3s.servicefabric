using StackExchange.Redis;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Mall3s.Core.Redis
{
    public interface IRedisManager
    {
        /// <summary>
        /// Add a new entity
        /// </summary>
        /// <typeparam name="TEntity">Entity</typeparam>
        /// <param name="key">Key</param>
        /// <param name="entity">Entity</param>
        /// <param name="cacheTime">Cache time</param>
        /// <returns></returns>
        bool Set<TEntity>(string key, TEntity entity, TimeSpan? cacheTime = null);

        /// <summary>
        /// Add a new array
        /// </summary>
        /// <typeparam name="TEntity">Entity</typeparam>
        /// <param name="key">Key</param>
        /// <param name="entities">An array</param>
        /// <param name="cacheTime">Cache time</param>
        /// <returns></returns>
        bool Set<TEntity>(string key, TEntity[] entities, TimeSpan? cacheTime = null);

        /// <summary>
        /// Add a new list
        /// </summary>
        /// <typeparam name="TEntity">Entity</typeparam>
        /// <param name="key">Key</param>
        /// <param name="entities">An list</param>
        /// <param name="cacheTime">Cache time</param>
        /// <returns></returns>
        bool Set<TEntity>(string key, List<TEntity> entities, TimeSpan? cacheTime = null);

        /// <summary>
        /// Add a new entity
        /// </summary>
        /// <typeparam name="TEntity">Entity</typeparam>
        /// <param name="key">Key</param>
        /// <param name="entity">Entity</param>
        /// <param name="cacheTime">Cache time</param>
        /// <returns></returns>
        Task<bool> SetAsync<TEntity>(string key, TEntity entity, TimeSpan? cacheTime = null);

        /// <summary>
        /// Add a new array
        /// </summary>
        /// <typeparam name="TEntity">Entity</typeparam>
        /// <param name="key">Key</param>
        /// <param name="entities">An array</param>
        /// <param name="cacheTime">Cache time</param>
        /// <returns></returns>
        Task<bool> SetAsync<TEntity>(string key, TEntity[] entities, TimeSpan? cacheTime = null);

        /// <summary>
        /// Add a new list
        /// </summary>
        /// <typeparam name="TEntity">entity</typeparam>
        /// <param name="key">key</param>
        /// <param name="entities">a list</param>
        /// <param name="cacheTime">cache time</param>
        /// <returns></returns>
        Task<bool> SetAsync<TEntity>(string key, List<TEntity> entities, TimeSpan? cacheTime = null);

        /// <summary>
        /// Hash set
        /// </summary>
        /// <param name="key"></param>
        /// <param name="hashField"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        Task<bool> HashSetAsync(string key, string hashField, string value);

        /// <summary>
        /// Get list tatal
        /// </summary>
        /// <param name="key">Key</param>
        /// <returns></returns>
        long Count(string key);

        /// <summary>
        /// Get list tatal
        /// </summary>
        /// <param name="key">Key</param>
        /// <returns></returns>
        Task<long> CountAsync(string key);

        /// <summary>
        /// 过期剩余时间(s)
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        Task<RedisValueWithExpiry> StringExpireAsync(string key);

        /// <summary>
        /// 判断在缓存中是否存在该key的缓存数据
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        bool Exists(string key);

        /// <summary>
        /// 判断是否存在
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        Task<bool> ExistsAsync(string key);

        /// <summary>
        /// Set cache time
        /// </summary>
        /// <param name="key">Key</param>
        /// <param name="cacheTime">Cache time</param>
        /// <returns></returns>
        bool Expire(string key, TimeSpan cacheTime);

        /// <summary>
        /// Set cache time
        /// </summary>
        /// <param name="key">Key</param>
        /// <param name="cacheTime">Cache time</param>
        /// <returns></returns>
        /// <returns></returns>
        Task<bool> ExpireAsync(string key, TimeSpan cacheTime);

        /// <summary>
        /// Remove by key
        /// </summary>
        /// <param name="key">Key</param>
        /// <returns></returns>
        bool Remove(string key);

        /// <summary>
        /// Remove by key array
        /// </summary>
        /// <param name="keys">Key array</param>
        /// <returns></returns>
        bool Remove(string[] keys);

        /// <summary>
        /// Remove by key list
        /// </summary>
        /// <param name="keys">Key list</param>
        /// <returns></returns>
        bool Remove(List<string> keys);

        /// <summary>
        /// Remove by key
        /// </summary>
        /// <param name="key">Key</param>
        /// <returns></returns>
        Task<bool> RemoveAsync(string key);

        /// <summary>
        /// Remove by key array
        /// </summary>
        /// <param name="keys">Key array</param>
        /// <returns></returns>
        Task<bool> RemoveAsync(string[] keys);

        /// <summary>
        /// Remove by key list
        /// </summary>
        /// <param name="keys">Key list</param>
        /// <returns></returns>
        Task<bool> RemoveAsync(List<string> keys);

        /// <summary>
        /// Hash remove
        /// </summary>
        /// <param name="key"></param>
        /// <param name="hashField"></param>
        /// <returns></returns>
        Task<bool> HashRemoveAsync(string key,string hashField);

        /// <summary>
        /// Blocking Dequeue
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        string BlockingDequeue(string key);

        /// <summary>
        /// Blocking Dequeue
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        Task<string> BlockingDequeueAsync(string key);

        /// <summary>
        /// Enqueue entity
        /// </summary>
        /// <param name="key"></param>
        /// <param name="entity"></param>
        void Enqueue<TEntity>(string key, TEntity entity);

        /// <summary>
        /// Enqueue entity
        /// </summary>
        /// <param name="key"></param>
        /// <param name="entity"></param>
        Task EnqueueAsync<TEntity>(string key, TEntity entity);

        /// <summary>
        /// 实现递增
        /// </summary>
        /// <param name="key"></param>
        /// <remarks>
        /// //三种命令模式
        /// Sync,同步模式会直接阻塞调用者，但是显然不会阻塞其他线程。
        /// Async,异步模式直接走的是Task模型。
        /// Fire - and - Forget,就是发送命令，然后完全不关心最终什么时候完成命令操作。
        /// 即发即弃：通过配置 CommandFlags 来实现即发即弃功能，在该实例中该方法会立即返回，如果是string则返回null 如果是int则返回0.这个操作将会继续在后台运行，一个典型的用法页面计数器的实现：
        /// </remarks>
        /// <returns></returns>
        long Increment(string key);

        /// <summary>
        /// 实现递增
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        Task<long> IncrementAsync(string key);

        /// <summary>
        /// 实现递增
        /// </summary>
        /// <param name="key"></param>
        /// <param name="key"></param>
        /// <returns></returns>
        Task<long> IncrementAsync(string key,string value);

        /// <summary>
        /// 实现递减
        /// </summary>
        /// <param name="key"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        long Decrement(string key, string value);

        /// <summary>
        /// 实现递减
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        Task<long> DecrementAsync(string key);

        /// <summary>
        /// 实现递减
        /// </summary>
        /// <param name="key"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        Task<long> DecrementAsync(string key, string value);

        /// <summary>
        /// 获取hash所有元素
        /// </summary>
        /// <returns></returns>
        Task<Dictionary<string, string>> HashGetAllAsync(string key);

        Task<RedisValue> HashGetAsync(string key, string hashField);

        TEntity Get<TEntity>(string key);

        List<TEntity> GetList<TEntity>(string key);

        TEntity[] GetArray<TEntity>(string key);

        Task<TEntity> GetAsync<TEntity>(string key);

        Task<List<TEntity>> GetListAsync<TEntity>(string key);

        Task<TEntity[]> GetArrayAsync<TEntity>(string key);

        Redlock GetRedlock();
    }
}