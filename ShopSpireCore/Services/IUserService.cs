using Microsoft.AspNetCore.Identity;
using ShopSpire.Utilities.DTO;
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
    }
}
