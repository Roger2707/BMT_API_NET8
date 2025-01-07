using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using StackExchange.Redis;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory.Database;

namespace Store_API.Controllers
{
    [Route("api/redis")]
    [ApiController]
    public class RedisController : ControllerBase
    {
        private readonly IConnectionMultiplexer _redis;
        public RedisController(IConnectionMultiplexer redis)
        {
            _redis = redis;
        }

        [HttpGet("set")]
        public async Task<IActionResult> SetKeyValue(string key, string value)
        {
            try
            {
                var db = _redis.GetDatabase();
                await db.StringSetAsync(key, value);
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
                var db = _redis.GetDatabase();
                var value = await db.StringGetAsync(key);
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

        [HttpGet("get-all-keys")]
        public IActionResult GetKeys()
        {
            try
            {
                var server = _redis.GetServer(_redis.GetEndPoints()[0]);
                var keys = server.Keys(pattern: "*");

                var result = new List<string>();
                foreach (var key in keys)
                {
                    result.Add($"{key.ToString()}, ");
                }
                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(new { Error = ex.Message });
            }
        }
    }
}
