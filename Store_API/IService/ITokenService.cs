using Store_API.Models;

namespace Store_API.IService
{
    public interface ITokenService
    {
        public Task<string> GenerateToken(User user);
    }
}
