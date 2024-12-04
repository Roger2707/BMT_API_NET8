using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Win32;
using Store_API.Data;
using Store_API.DTOs.Accounts;
using Store_API.Helpers;
using Store_API.Models;
using Store_API.Repositories;

namespace Store_API.Services
{
    public class AccountService : RepositoryService, IAccountRepository
    {
        private readonly StoreContext _db;
        private readonly IDapperService _dapperService;
        private readonly IImageRepository _imageService;
        private readonly UserManager<User> _userManager;
        private readonly EmailSenderService _emailSenderService;
        public AccountService(StoreContext db, IDapperService dapperService, IImageRepository imageService
            , UserManager<User> userManager, EmailSenderService emailSenderService) : base(dapperService, db)
        {
            _db = db;
            _dapperService = dapperService;
            _imageService = imageService;
            _userManager = userManager; 
            _emailSenderService = emailSenderService;
        }
        
        public async Task<List<AccountDTO>> GetAll()
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
                                , r.Name as RoleName
	                            , basket.Id as BasketId

                            FROM AspNetUserRoles user_role

                            LEFT JOIN AspNetUsers u ON u.Id = user_role.UserId
                            LEFT JOIN AspNetRoles r ON r.Id = user_role.RoleId
                            LEFT JOIN Baskets basket ON basket.UserId = user_role.UserId

                            WHERE r.Id != 1 ";

            
            List<dynamic> result = await _dapperService.QueryAsync(query, null);
            if(result.Count == 0) return null;
            List<AccountDTO> accounts = new List<AccountDTO>();

            foreach (var account in result)
            {
                AccountDTO accountDTO = new AccountDTO()
                {
                    UserName = account.UserName,
                    FullName = account.FullName,
                    Email = account.Email,
                    PhoneNumber = account.PhoneNumber,
                    Dob = account.Dob,
                    ImageUrl = account.ImageUrl,
                    RoleName = account.RoleName,
                    BasketId = CF.GetInt(account.BasketId),
                };

                accounts.Add(accountDTO);
            }

            return accounts;
        }

        public async Task Create(SignUpRequest request, string role)
        {
            // 1. Create password
            string password = request.Username[0].ToString().ToUpper() + request.Username.Substring(1).ToLower() + "@123";

            User user = new User
            {
                FullName = request.FullName,
                Email = request.Email,
                UserName = request.Username,
                PhoneNumber = request.PhoneNumber,
                PasswordHash = password,
            };

            try
            {
                // 2. Create User
                var result = await _userManager.CreateAsync(user, user.PasswordHash);

                if (result.Succeeded)
                {
                    await _userManager.AddToRoleAsync(user, role);

                    // 3. Send password to email
                    string content = $" Your Infomation below here : " + Environment.NewLine
                        + $" Username : {request.Username} " + Environment.NewLine
                        + $" Password : {password}. " + Environment.NewLine
                    + $" You can change your password after login the first time OR use this if you want ! ";

                    await _emailSenderService.SendEmailAsync(request.Email, "(IMPORTANT) Your Login Account Infos :", content);
                }
                else
                {
                    var errors = new List<string>();
                    foreach (var error in result.Errors)
                    {
                        errors.Add(error.Description);
                    }
                    throw new Exception(string.Join(", ", errors));
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public async Task SendLinkResetPwToMail(ForgetPasswordDTO model, User user)
        {
            var token = await _userManager.GeneratePasswordResetTokenAsync(user);
            token = token.Replace(" ", "+");
            token = CF.Base64ForUrlEncode(token);
            string linkResetPassword = $"http://localhost:3000/get-reset-password?email={model.Email}&token={token}";
            string htmlContent = $"This is your link to reset password :" + Environment.NewLine
                + $"Link :" + linkResetPassword;

            // Send Message to email
            await _emailSenderService.SendEmailAsync(model.Email, "Reset Password ROGER BMT APP (NET 8)", htmlContent);     
        }

        public Task Update()
        {
            throw new NotImplementedException();
        }
    }
}
