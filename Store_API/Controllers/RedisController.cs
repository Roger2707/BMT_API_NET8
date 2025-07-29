using Microsoft.AspNetCore.Mvc;
using StackExchange.Redis;

namespace Store_API.Controllers
{
    [Route("api/redis")]
    [ApiController]
    public class RedisController : ControllerBase
    {
        private readonly IDatabase _redis;
        public RedisController(IConnectionMultiplexer redis)
        {
            _redis = redis.GetDatabase();
        }

        [HttpGet("set")]
        public async Task<IActionResult> SetKeyValue(string key, string value)
        {
            await _redis.StringSetAsync(key, value, TimeSpan.FromMinutes(15));
            return Ok(new { Message = $"Key '{key}' set with value '{value}'" });
        }

        [HttpGet("get")]
        public async Task<IActionResult> GetKeyValue(string key)
        {
            var value = await _redis.StringGetAsync(key);
            if (value.IsNullOrEmpty)
            {
                return NotFound(new { Message = $"Key '{key}' not found" });
            }
            return Ok(new { Key = key, Value = value.ToString() });
        }

        [HttpGet("delete")]
        public async Task<IActionResult> DelKeyValue(string key)
        {
            var value = await _redis.KeyDeleteAsync(key);
            return Ok();
        }

        [HttpGet("test-ping")]
        public async Task<IActionResult> TestPing()
        {
            var pong = await _redis.PingAsync();
            return Ok(new { Pong = pong });
        }
    }
}
