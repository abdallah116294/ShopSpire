using ShopSpire.Utilities.DTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ShopSpireCore.Services
{
    public interface IRoleService
    {
        Task<ResponseDto<object>> AddRoleToUserAsync(string email, string roleName);
    }
}
