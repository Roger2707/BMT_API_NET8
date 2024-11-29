using Store_API.Models;

namespace Store_API.Repositories
{
    public interface ITokenRepository
    {
        public Task<string> GenerateToken(User user);
    }
}
