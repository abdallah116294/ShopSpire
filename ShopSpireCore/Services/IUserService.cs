using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using ShopSpire.Utilities.DTO;
using ShopSpireCore.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ShopSpireCore.Services
{
    public  interface  IUserService
    {
        Task LogoutAsync();
        Task<ResponseDto<object>> LoginAsync(LoginDTO dto);
        Task<ResponseDto<object>> RegisterAsync(RegisterDTO dto);
         Task<ResponseDto<object>> ForgetPassword(ForgetPasswordDTO dto);
        Task<ResponseDto<object>> ResetPasswordAsync(string email, string otp, string newPassword);
        Task<ResponseDto<object>> GetAllSeller();
        Task<User> GetUserByID(string id);
    }
}
