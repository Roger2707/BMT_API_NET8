using Store_API.DTOs.Accounts;
using Store_API.Models;
using Store_API.Services;

namespace Store_API.Repositories
{
    public interface IAccountRepository : IRepository
    {
        public Task<List<AccountDTO>> GetAll();
        public Task Create(SignUpRequest request, string role);
        public Task SendLinkResetPwToMail(ForgetPasswordDTO model, User user);
    }
}
