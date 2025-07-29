using Store_API.Models.Users;

namespace Store_API.Services.IService
{
    public interface ITokenService
    {
        public Task<string> GenerateToken(User user);
        public string GeneratePasswordResetToken(User user);
    }
}
