using Store_API.DTOs.Accounts;
using Store_API.Models.Users;

namespace Store_API.Repositories.IRepositories
{
    public interface IUserRepository : IRepository<User>
    {
        Task<List<UserDTO>> GetAll();
        Task<UserDTO> GetUser(string username);
        Task<UserDTO> GetUser(int userId);
        Task<List<Role>> GetRoles(int userId);
        public Task<bool> CheckRoleAsync(int userId, int roleId);
    }
}
