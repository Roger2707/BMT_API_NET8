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
            try
            {
                await _redis.StringSetAsync(key, value, TimeSpan.FromMinutes(15));
                return Ok(new { Message = $"Key '{key}' set with value '{value}'" });
            }
            catch (Exception ex)
            {
                return BadRequest(new { Error = ex.Message });
            }
        }

        [HttpGet("get")]
        public async Task<IActionResult> GetKeyValue(string key)
        {
            try
            {
                var value = await _redis.StringGetAsync(key);
                if (value.IsNullOrEmpty)
                {
                    return NotFound(new { Message = $"Key '{key}' not found" });
                }
                return Ok(new { Key = key, Value = value.ToString() });
            }
            catch (Exception ex)
            {
                return BadRequest(new { Error = ex.Message });
            }
        }

        [HttpGet("delete")]
        public async Task<IActionResult> DelKeyValue(string key)
        {
            try
            {
                var value = await _redis.KeyDeleteAsync(key);
                return Ok();
            }
            catch (Exception ex)
            {
                return BadRequest(new { Error = ex.Message });
            }
        }

        [HttpGet("test-ping")]
        public async Task<IActionResult> TestPing()
        {
            try
            {
                var pong = await _redis.PingAsync();
                return Ok(new { Pong = pong });
            }
            catch (Exception ex)
            {
                return BadRequest(new { Error = ex.ToString() });
            }
        }
    }
}
