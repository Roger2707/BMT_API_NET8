using Microsoft.AspNetCore.Identity;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Store_API.Data;
using Store_API.DTOs;
using Store_API.DTOs.Orders;
using Store_API.IRepositories;
using Store_API.Models;
using Store_API.Models.OrderAggregate;

namespace Store_API.Repositories
{
    public class UserAddressRepository : IUserAddressRepository
    {
        private readonly IDapperService _dapperService;
        public readonly StoreContext _db;
        public UserAddressRepository(StoreContext db, IDapperService dapperService)
        {
            _db = db;
            _dapperService = dapperService;
        }

        #region Retrieve

        public async Task<Result<List<UserAddressDTO>>> GetUserAddresses(int userId)
        {
            string query = @" SELECT Id, UserId, City, District, Ward, PostalCode, StreetAddress, Country FROM UserAddresses WHERE UserId = @UserId ";
            var p = new { UserId = userId };
            List<UserAddressDTO> userAddresses = new List<UserAddressDTO>();
            var result = await _dapperService.QueryAsync(query, p);

            if (result.Count != 0 && result != null)
            {
                for (int i = 0; i < result.Count; i++)
                {
                    userAddresses.Add(new UserAddressDTO
                    {
                        Id = result[i].Id,
                        City = result[i].City,
                        District = result[i].District,
                        Ward = result[i].Ward,
                        PostalCode = result[i].PostalCode,
                        StreetAddress = result[i].StreetAddress,
                        Country = result[i].Country,
                        UserId = userId
                    });
                }
            }

            return Result<List<UserAddressDTO>>.Success(userAddresses);
        }

        public async Task<Result<UserAddressDTO>> GetUserAddress(int userAddressId)
        {
            string query = @" SELECT Id, UserId, City, District, Ward, PostalCode, StreetAddress, Country FROM UserAddresses WHERE Id = @Id ";
            var p = new { Id = userAddressId };  
            var userAddress = new UserAddressDTO();
            var result = await _dapperService.QueryAsync(query, p);

            if (result != null)
            {
                userAddress.City = result[0].City;
                userAddress.District = result[0].District;
                userAddress.Ward = result[0].Ward;
                userAddress.PostalCode = result[0].PostalCode;
                userAddress.StreetAddress = result[0].StreetAddress;
                userAddress.Country = result[0].Country;
                userAddress.UserId = result[0].UserId;
            }

            return Result<UserAddressDTO>.Success(userAddress);
        }

        #endregion


        #region CRUD

        public async Task<Result<dynamic>> UpsertUserAddresses(int userId, UserAddressDTO userAddressDTO)
        {
            int id = userAddressDTO.Id;
            try
            {
                if (id != 0)
                {
                    string query = @" UPDATE UserAddresses 
                                        SET City = @City, District = @District, Ward = @Ward
                                        , StreetAddress = @StreetAddress, PostalCode = @PostalCode, Country = @Country
                                        WHERE Id = @Id
                                    ";
                    var parameters = new[]
                    {
                        new SqlParameter("@City", userAddressDTO.City ?? (object)DBNull.Value),
                        new SqlParameter("@District", userAddressDTO.District ?? (object)DBNull.Value),
                        new SqlParameter("@Ward", userAddressDTO.Ward ?? (object)DBNull.Value),
                        new SqlParameter("@StreetAddress", userAddressDTO.StreetAddress ?? (object)DBNull.Value),
                        new SqlParameter("@PostalCode", userAddressDTO.PostalCode ?? (object)DBNull.Value),
                        new SqlParameter("@Country", userAddressDTO.Country ?? (object)DBNull.Value),
                        new SqlParameter("@Id", userId)
                    };
                    await _db.Database.ExecuteSqlRawAsync(query, parameters);
                    return Result<dynamic>.Success(id);
                }
                else
                {
                    var userAddress = new UserAddress
                    {
                        City = userAddressDTO.City,
                        District = userAddressDTO.District,
                        Ward = userAddressDTO.Ward,
                        StreetAddress = userAddressDTO.StreetAddress,
                        PostalCode = userAddressDTO.PostalCode,
                        Country = userAddressDTO.Country,
                        UserId = userId,
                    };
                    await _db.UserAddresses.AddAsync(userAddress);
                    await _db.SaveChangesAsync();
                    return Result<dynamic>.Success(userAddress.Id);
                }
            }
            catch (SqlException ex)
            {
                _dapperService.Rollback();
                return Result<dynamic>.Failure(ex.Message);
            }
        }

        public async Task<Result<string>> Delete(int userAddressId)
        {
            string query = @" DELETE FROM UserAddresses WHERE Id = @Id ";
            var p = new { Id = userAddressId };
            try
            {
                _dapperService.BeginTrans();
                await _dapperService.Execute(query, p);
                _dapperService.Commit();
            }
            catch (SqlException ex)
            {
                _dapperService.Rollback();
                return Result<string>.Failure(ex.Message);
            }
            return Result<string>.Success("Deleted UserAddress Successfully !");
        }

        #endregion
    }
}
