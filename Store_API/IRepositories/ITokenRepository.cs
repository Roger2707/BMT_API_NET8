using Store_API.Models;
using System.IdentityModel.Tokens.Jwt;

namespace Store_API.Repositories
{
    public interface ITokenRepository
    {
        public Task<string> GenerateToken(User user);
        public Task<JwtSecurityToken> ValidateToken(string token);
    }
}
