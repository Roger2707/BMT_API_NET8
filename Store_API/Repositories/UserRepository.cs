using Store_API.Data;
using Store_API.DTOs.Accounts;
using Store_API.DTOs.User;
using Store_API.Infrastructures;
using Store_API.Models.Users;
using Store_API.Repositories.IRepositories;

namespace Store_API.Repositories
{
    public class UserRepository : Repository<User>, IUserRepository
    {
        public UserRepository(StoreContext db, IDapperService dapperService) : base(db, dapperService)
        {
        }

        #region Retrieve

        public async Task<List<UserDTO>> GetAll()
        {
            string query = @" 
                            SELECT 
                                u.Id
                                , u.UserName
                                , u.FullName
                                , u.Email
                                , u.PhoneNumber
                                , u.Dob
                                , u.ImageUrl
                                , u.PublicId
                                , u.Provider
                                , r.Name as RoleName
                            FROM Users u
                            INNER JOIN userRoles user_role ON u.Id = user_role.UserId
                            INNER JOIN Roles r ON r.Id = user_role.RoleId 
                            ";

            var users = await _dapperService.QueryAsync<UserDTO>(query, null);
            return users;
        }

        public async Task<UserDTO> GetUser(string username)
        {
            string query = @" 
                                SELECT 
                                    u.Id
                                    , FullName
                                    , UserName
                                    , Email
                                    , Dob
                                    , PhoneNumber
                                    , ImageUrl
                                    , u.PublicId
                                    , u.Provider
                                    , ISNULL(b.Id, NULL) as BasketId
                                    , ua.Id as AddressId
                                    , ua.City
                                    , ua.District
                                    , ua.Ward
                                    , ua.StreetAddress
                                    , ua.PostalCode
                                    , ua.Country
                                FROM Users u
                                LEFT JOIN Baskets b ON b.UserId = u.Id
                                LEFT JOIN UserAddresses ua ON ua.UserId = u.Id
                                WHERE UserName = @UserName
                                ";

            var user = await _dapperService.QueryAsync<UserDapperRow>(query, new { UserName = username });
            if (user == null) return null;

            var userDTO = user
                .GroupBy(g => new { g.Id, g.Username, g.FullName, g.Email, g.ImageUrl, g.Dob, g.PublicId, g.PhoneNumber, g.BasketId, g.Provider })
                .Select(u => new UserDTO
                {
                    Id = u.Key.Id,
                    UserName = u.Key.Username,
                    FullName = u.Key.FullName,
                    Email = u.Key.Email,
                    ImageUrl = u.Key.ImageUrl,
                    Dob = u.Key.Dob,
                    PublicId = u.Key.PublicId,
                    PhoneNumber = u.Key.PhoneNumber,
                    BasketId = u.Key.BasketId,
                    Provider = u.Key.Provider,
                    Token = "",
                    UserAddresses = u.Select(a => new UserAddressDTO
                    {
                        Id = a.AddressId,
                        City = a.City,
                        District = a.District,
                        Ward = a.Ward,
                        StreetAddress = a.StreetAddress,
                        PostalCode = a.PostalCode,
                        Country = a.Country
                    }).ToList()
                })
                .FirstOrDefault();

            return userDTO;
        }

        public async Task<UserDTO> GetUser(int userId)
        {
            string query = @$" 
                                SELECT 
                                    u.Id
                                    , FullName
                                    , UserName
                                    , Email
                                    , Dob
                                    , PhoneNumber
                                    , ImageUrl
                                    , u.PublicId
                                    , u.Provider
                                    , ISNULL(b.Id, NULL) as BasketId
                                    , ua.Id as AddressId
                                    , ua.City
                                    , ua.District
                                    , ua.Ward
                                    , ua.StreetAddress
                                    , ua.PostalCode
                                    , ua.Country
                                FROM Users u
                                LEFT JOIN Baskets b ON b.UserId = u.Id
                                LEFT JOIN UserAddresses ua ON ua.UserId = u.Id
                                WHERE u.Id = @UserId
                                ";

            var user = await _dapperService.QueryAsync<UserDapperRow>(query, new { UserId = userId });
            if (user == null) return null;

            var userDTO = user
                .GroupBy(g => new { g.Id, g.Username, g.FullName, g.Email, g.ImageUrl, g.Dob, g.PublicId, g.PhoneNumber, g.BasketId, g.Provider })
                .Select(u => new UserDTO
                {
                    Id = u.Key.Id,
                    UserName = u.Key.Username,
                    FullName = u.Key.FullName,
                    Email = u.Key.Email,
                    ImageUrl = u.Key.ImageUrl,
                    Dob = u.Key.Dob,
                    PublicId = u.Key.PublicId,
                    PhoneNumber = u.Key.PhoneNumber,
                    Provider = u.Key.Provider,
                    BasketId = u.Key.BasketId,
                    Token = "",
                    UserAddresses = u.Select(a => new UserAddressDTO
                    {
                        Id = a.AddressId,
                        City = a.City,
                        District = a.District,
                        Ward = a.Ward,
                        StreetAddress = a.StreetAddress,
                        PostalCode = a.PostalCode,
                        Country = a.Country
                    }).ToList()
                })
                .FirstOrDefault();

            return userDTO;
        }

        public async Task<List<Role>> GetRoles(int userId)
        {
            string query = @"
                            SELECT r.*
                            FROM Roles r 
                            WHERE r.Id IN (SELECT RoleId FROM UserRoles WHERE UserId = @UserId)
                            ";
            var roles = await _dapperService.QueryAsync<Role>(query, new { UserId = userId });
            if (roles == null || roles.Count == 0) return null;
            return roles;
        }

        #endregion

        #region CRUDs / Check

        public async Task<bool> CheckRoleAsync(int userId, int roleId)
        {
            string query = @" SELECT COUNT(1) FROM UserRoles WHERE UserId = @UserId AND RoleId = @RoleId";
            var isExisted = await _dapperService.QueryFirstOrDefaultAsync<int>(query, new { UserId = userId, RoleId = roleId });
            return isExisted > 0;
        }

        #endregion
    }
}
