using Microsoft.AspNetCore.Identity;
using ShopSpire.Utilities.DTO;
using ShopSpireCore.Entities;
using ShopSpireCore.IRepositories;
using ShopSpireCore.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ShopSpire.Repository.Repositories
{
    public class UserRepository : IUserRepository
    {
        private readonly UserManager<User> _userManager;
        private readonly SignInManager<User> _signInManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly IEmailService _emailService;
        private readonly IUnitOfWork _unitOfWork;

        public UserRepository(UserManager<User> userManager, SignInManager<User> signInManager, RoleManager<IdentityRole> roleManager, IEmailService emailService, IUnitOfWork unitOfWork)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _roleManager = roleManager;
            _emailService = emailService;
            _unitOfWork = unitOfWork;
        }

        public async Task<User> FindByEmailAsync(string email)
        {
          return await _userManager.FindByEmailAsync(email);
        }

        public async Task<string> GeneratePasswordResetTokenAsync(User user)
        {
            return await _userManager.GeneratePasswordResetTokenAsync(user);
        }

        public async Task<List<User>> GetAllSeller()
        {
            var users=await _unitOfWork.Repository<User>().GetAllAsync();
            return users;
        }

        public async Task<string> GetOtpAsyn(User user)
        {
            return await _userManager.GetAuthenticationTokenAsync(user, "ShopSpire", "ResetPassword");
        }

        public async Task<bool> LoginAsync(LoginDTO dto)
        {
            var user = await _userManager.FindByEmailAsync(dto.Email);
            if (user == null)
            {
                return false;
            }
            var result = await _userManager.CheckPasswordAsync(user, dto.Password);
            return result;
        }

        public async Task LogoutAsync()
        {
            await _signInManager.SignOutAsync();
        }

        public async Task<IdentityResult> RegisterAsync(RegisterDTO dto)
        {
            var user = new User
            {
                UserName=string.Join(dto.FirstName, dto.LastName),
                Email = dto.Email,
                PhoneNumber = dto.PhoneNumber,
                FirstName = dto.FirstName,
                LastName = dto.LastName
            };
            var result =await _userManager.CreateAsync(user,dto.Password);
           
            if (result.Succeeded)
            {
                await _userManager.AddToRoleAsync(user, "USER");
            }
            return result;
        }

        public async Task<bool> RemoveOtpAsync(User user)
        {
            var result = await _userManager.RemoveAuthenticationTokenAsync(user, "ShopSpire", "ResetPassword");
            return result.Succeeded;
        }

        public async Task<IdentityResult> ResetPasswordAsync(User user, string resetToken, string newPassword)
        {
            return await _userManager.ResetPasswordAsync(user,resetToken,newPassword);

        }

        public async Task<bool> SetOtpAync(User user, string otpCode)
        {
            var result = await _userManager.SetAuthenticationTokenAsync(user,"ShopSpire","ResetPassword",otpCode);
            return result.Succeeded;
        }
    }
}
