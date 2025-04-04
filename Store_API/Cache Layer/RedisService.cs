using StackExchange.Redis;
using System.Text.Json;

namespace Store_API.Cache_Layer
{
    public class RedisService : IRedisService
    {
        private readonly IConnectionMultiplexer _redis;

        public RedisService(IConnectionMultiplexer redis)
        {
            _redis = redis;
        }

        public async Task<T> GetAsync<T>(string key)
        {
            var db = _redis.GetDatabase();
            var value = await db.StringGetAsync(key);
            return value.HasValue ? JsonSerializer.Deserialize<T>(value) : default;
        }

        public async Task SetAsync<T>(string key, T value, TimeSpan expiration)
        {
            var db = _redis.GetDatabase();
            var json = JsonSerializer.Serialize(value);
            await db.StringSetAsync(key, json, expiration);
        }

        public async Task<IEnumerable<string>> GetKeysAsync(string pattern)
        {
            var server = _redis.GetServer(_redis.GetEndPoints().First());
            var keys = server.Keys(pattern: pattern);
            return await Task.FromResult(keys.Select(k => k.ToString()));
        }

        public async Task<TimeSpan?> GetKeyTimeToLiveAsync(string key)
        {
            var db = _redis.GetDatabase();
            return await db.KeyTimeToLiveAsync(key);
        }
    }
}
