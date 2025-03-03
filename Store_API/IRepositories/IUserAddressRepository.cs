using Store_API.DTOs.Orders;
using Store_API.DTOs;

namespace Store_API.IRepositories
{
    public interface IUserAddressRepository
    {

        Task<Result<List<UserAddressDTO>>> GetUserAddresses(int userId);
        Task<Result<UserAddressDTO>> GetUserAddress(int userAddressId);
        Task<Result<dynamic>> UpsertUserAddresses(int userId, UserAddressDTO userAddressDTO);
        Task<Result<string>> Delete(int userAddressId);
    }
}
