using Microsoft.AspNetCore.Identity;
using ShopSpire.Utilities.DTO;
using ShopSpireCore.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ShopSpireCore.IRepositories
{
   public interface  IUserRepository
    {
        Task LogoutAsync();
        Task<bool> LoginAsync(LoginDTO dto );
        Task<IdentityResult> RegisterAsync(RegisterDTO dto);
        Task<User> FindByEmailAsync (string email);
        Task<bool> SetOtpAync(User user, string otpCode);
        Task<string> GetOtpAsyn(User user);
        Task<bool> RemoveOtpAsync(User user);
        Task<IdentityResult> ResetPasswordAsync(User user, string resetToken, string newPassword);
        Task<string> GeneratePasswordResetTokenAsync(User user);
        Task<List<User>> GetAllSeller();
        Task<User> GetUserById(string id);

    }
}
