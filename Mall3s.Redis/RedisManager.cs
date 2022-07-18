using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using StackExchange.Redis;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mall3s.Core.Redis
{
    public class RedisManager : IRedisManager
    {
        private readonly IOptions<RedisOptions> _redisOptions;
        private ConnectionMultiplexer _conn;
        private static string _connStr;

        public RedisManager(
            IOptions<RedisOptions> redisOptions)
        {
            _redisOptions = redisOptions;
            _connStr = string.Format("{0}:{1},allowAdmin=true,password={2},defaultdatabase={3}",
              _redisOptions.Value.HostName,
              _redisOptions.Value.Port,
              _redisOptions.Value.Password,
              _redisOptions.Value.Defaultdatabase
            );
            RedisConnection();
        }

        private void RedisConnection()
        {
            try
            {
                Console.WriteLine($"Redis config: {_connStr}");
                _conn = ConnectionMultiplexer.Connect(_connStr);
                Console.WriteLine("Redis manager started!");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Redis connection error: {ex.Message}");
            }
        }

        private IDatabase GetDatabase()
        {
            try
            {
                return _conn.GetDatabase();
            }
            catch
            {
                _conn = ConnectionMultiplexer.Connect(_connStr);
                Console.WriteLine("Redis manager reconnection!");
                return _conn.GetDatabase();
            }
        }

        public bool Set<TEntity>(string key, TEntity entity, TimeSpan? cacheTime = null)
        {
            if (Exists(key))
            {
                Remove(key);
            }
            var result = GetDatabase().StringSet(key, JsonConvert.SerializeObject(entity));
            if (cacheTime != null)
            {
                return result && Expire(key, cacheTime.Value);
            }
            return result;
        }

        public bool Set<TEntity>(string key, TEntity[] entities, TimeSpan? cacheTime = null)
        {
            if (Exists(key))
            {
                Remove(key);
            }
            var redisValues = entities.Select(p => (RedisValue)(JsonConvert.SerializeObject(p))).ToArray();
            var result = GetDatabase().SetAdd(key, redisValues) == redisValues.Length;
            if (cacheTime != null)
            {
                return result && Expire(key, cacheTime.Value);
            }
            return result;
        }

        public bool Set<TEntity>(string key, List<TEntity> entities, TimeSpan? cacheTime = null)
        {
            if (Exists(key))
            {
                Remove(key);
            }
            return Set(key, entities.ToArray(), cacheTime);
        }

        public async Task<bool> SetAsync<TEntity>(string key, TEntity entity, TimeSpan? cacheTime = null)
        {
            if (await ExistsAsync(key))
            {
                await RemoveAsync(key);
            }
            var result = await GetDatabase().StringSetAsync(key, JsonConvert.SerializeObject(entity), cacheTime);
            return result;
        }

        public async Task<bool> SetAsync<TEntity>(string key, TEntity[] entities, TimeSpan? cacheTime = null)
        {
            if (await ExistsAsync(key))
            {
                await RemoveAsync(key);
            }
            var redisValues = entities.Select(p => (RedisValue)(JsonConvert.SerializeObject(p))).ToArray();
            var result = await GetDatabase().SetAddAsync(key, redisValues) == redisValues.Length;
            if (cacheTime != null)
            {
                return result && await ExpireAsync(key, cacheTime.Value);
            }
            return result;
        }

        public async Task<bool> SetAsync<TEntity>(string key, List<TEntity> entities, TimeSpan? cacheTime = null)
        {
            if (await ExistsAsync(key))
            {
                await RemoveAsync(key);
            }
            return await SetAsync(key, entities.ToArray(), cacheTime);
        }

        public async Task<bool> HashSetAsync(string key, string hashField, string value)
        {
            return await GetDatabase().HashSetAsync(key, hashField, value);
        }

        public long Count(string key)
        {
            return GetDatabase().ListLength(key);
        }

        public async Task<long> CountAsync(string key)
        {
            return await GetDatabase().ListLengthAsync(key);
        }

        public async Task<RedisValueWithExpiry> StringExpireAsync(string key)
        {
            return await GetDatabase().StringGetWithExpiryAsync(key);
        }

        public bool Exists(string key)
        {
            return GetDatabase().KeyExists(key);
        }

        public async Task<bool> ExistsAsync(string key)
        {
            return await GetDatabase().KeyExistsAsync(key);
        }

        public bool Expire(string key, TimeSpan cacheTime)
        {
            return GetDatabase().KeyExpire(key, DateTime.Now.AddSeconds(int.Parse(cacheTime.TotalSeconds.ToString())));
        }

        public async Task<bool> ExpireAsync(string key, TimeSpan cacheTime)
        {
            return await GetDatabase().KeyExpireAsync(key, DateTime.Now.AddSeconds(int.Parse(cacheTime.TotalSeconds.ToString())));
        }

        public bool Remove(string key)
        {
            return GetDatabase().KeyDelete(key);
        }

        public bool Remove(string[] keys)
        {
            var redisKeys = keys.Select(p => (RedisKey)(JsonConvert.SerializeObject(p))).ToArray();
            return GetDatabase().KeyDelete(redisKeys) == redisKeys.Length;
        }

        public bool Remove(List<string> keys)
        {
            return Remove(keys.ToArray());
        }

        public async Task<bool> RemoveAsync(string key)
        {
            return await GetDatabase().KeyDeleteAsync(key);
        }

        public async Task<bool> RemoveAsync(string[] keys)
        {
            var redisKeys = keys.Select(p => (RedisKey)(JsonConvert.SerializeObject(p))).ToArray();
            return await GetDatabase().KeyDeleteAsync(redisKeys) == redisKeys.Length;
        }

        public async Task<bool> RemoveAsync(List<string> keys)
        {
            return await RemoveAsync(keys.ToArray());
        }

        public async Task<bool> HashRemoveAsync(string key, string hashField)
        {
            return await GetDatabase().HashDeleteAsync(key,hashField);
        }

        public string BlockingDequeue(string key)
        {
            return GetDatabase().ListRightPop(key);
        }

        public async Task<string> BlockingDequeueAsync(string key)
        {
            return await GetDatabase().ListRightPopAsync(key);
        }

        public void Enqueue<TEntity>(string key, TEntity entity)
        {
            GetDatabase().ListLeftPush(key, JsonConvert.SerializeObject(entity));
        }

        public async Task EnqueueAsync<TEntity>(string key, TEntity entity)
        {
            await GetDatabase().ListLeftPushAsync(key, JsonConvert.SerializeObject(entity));
        }

        public long Increment(string key)
        {
            return GetDatabase().StringIncrement(key);
        }

        public async Task<long> IncrementAsync(string key)
        {
            return await GetDatabase().StringIncrementAsync(key);
        }

        public async Task<long> IncrementAsync(string key, string value)
        {
            return await GetDatabase().HashIncrementAsync(key,value);
        }

        public long Decrement(string key, string value)
        {
            return GetDatabase().HashDecrement(key, value);
        }

        public async Task<long> DecrementAsync(string key)
        {
            return await GetDatabase().StringDecrementAsync(key);
        }

        public async Task<long> DecrementAsync(string key, string value)
        {
            return await GetDatabase().HashDecrementAsync(key, value);
        }

        public async Task<Dictionary<string, string>> HashGetAllAsync(string key)
        {
            if (!(await ExistsAsync(key)))
            {
                return default;
            }
            var result = await GetDatabase().HashGetAllAsync(key);
            if (result.Length > 0)
            {
                return result.ToStringDictionary();
            }
            else
            {
                return default;
            }
        }

        public async Task<RedisValue> HashGetAsync(string key, string hashField)
        {
            if (!(await ExistsAsync(key)))
            {
                return default;
            }
            var result = await GetDatabase().HashGetAsync(key,hashField);
            return result;
        }

        public TEntity Get<TEntity>(string key)
        {
            if (!Exists(key))
            {
                return default;
            }
            var result = GetDatabase().StringGet(key);
            return JsonConvert.DeserializeObject<TEntity>(result);
        }

        public List<TEntity> GetList<TEntity>(string key)
        {
            if (!Exists(key))
            {
                return null;
            }
            var result = GetDatabase().SetMembers(key);
            return result.Select(p => JsonConvert.DeserializeObject<TEntity>(p)).ToList();
        }

        public TEntity[] GetArray<TEntity>(string key)
        {
            if (!Exists(key))
            {
                return null;
            }
            var result = GetDatabase().SetMembers(key);
            return result.Select(p => JsonConvert.DeserializeObject<TEntity>(p)).ToArray();
        }

        public async Task<TEntity> GetAsync<TEntity>(string key)
        {
            if (!await ExistsAsync(key))
            {
                return default;
            }
            var result = await GetDatabase().StringGetAsync(key);
            return JsonConvert.DeserializeObject<TEntity>(result);
        }

        public async Task<List<TEntity>> GetListAsync<TEntity>(string key)
        {
            if (!await ExistsAsync(key))
            {
                return null;
            }
            var result = await GetDatabase().SetMembersAsync(key);
            return result.Select(p => JsonConvert.DeserializeObject<TEntity>(p)).ToList();
        }

        public async Task<TEntity[]> GetArrayAsync<TEntity>(string key)
        {
            if (!await ExistsAsync(key))
            {
                return null;
            }
            var result = await GetDatabase().SetMembersAsync(key);
            return result.Select(p => JsonConvert.DeserializeObject<TEntity>(p)).ToArray();
        }

        public Redlock GetRedlock()
        {
            return new Redlock(_conn);
        }
    }
}