using System.Collections.Concurrent;

namespace Store_API.RabbitMQConfig
{
    public class MessageQueue
    {
        private readonly ConcurrentQueue<string> _messages = new ConcurrentQueue<string>();

        public void Enqueue(string message)
        {
            _messages.Enqueue(message);
        }

        public bool TryDequeue(out string message)
        {
            return _messages.TryDequeue(out message);
        }

        public int Count => _messages.Count;
    }
}
