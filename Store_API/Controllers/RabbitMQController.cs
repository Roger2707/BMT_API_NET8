using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Store_API.RabbitMQConfig;

namespace Store_API.Controllers
{
    [Route("api/rabbitMQ")]
    [ApiController]
    public class RabbitMQController : ControllerBase
    {
        private readonly RabbitMQService _rabbitMQService;
        private readonly MessageQueue _messageQueue;

        public RabbitMQController(RabbitMQService rabbitMQService, MessageQueue messageQueue)
        {
            _rabbitMQService = rabbitMQService;
            _messageQueue = messageQueue;
        }

        [HttpPost("send")]
        public async Task<IActionResult> SendMessage([FromBody] object message)
        {
            await _rabbitMQService.SendMessageAsync(message);
            return Ok(new { Message = "Message sent successfully" });
        }

        [HttpGet("consume")]
        public IActionResult ConsumeMessages()
        {
            if (_messageQueue.TryDequeue(out var message))
            {
                return Ok(new { Message = message });
            }

            return NoContent(); 
        }
    }
}
