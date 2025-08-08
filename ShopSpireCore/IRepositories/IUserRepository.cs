using Microsoft.AspNetCore.Identity;
using ShopSpire.Utilities.DTO;
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
        Task<SignInResult> LoginAsync(LoginDTO dto );
        Task<IdentityResult> RegisterAsync(RegisterDTO dto);
    }
}
