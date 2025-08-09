using ShopSpire.Utilities.DTO;
using ShopSpireCore.IRepositories;
using ShopSpireCore.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ShopSpire.Service
{
    public class RoleService : IRoleService
    {
        private readonly IRoleRepository _roleRespository;

        public RoleService(IRoleRepository roleRespository)
        {
            _roleRespository = roleRespository;
        }

        public  async Task<ResponseDto<object>> AddRoleToUserAsync(string email, string roleName)
        {
            try
            {
                var result = await _roleRespository.AddRoleToUserAsync(email, roleName);
                if (result)
                {
                    return new ResponseDto<object>
                    {
                        IsSuccess = true,
                        Data = null,
                        Message = "Role added to user successfully."
                    };
                }
                return new ResponseDto<object>
                {
                    IsSuccess = false,
                    Message = "Failed to add role to user.",
                    Data = null,
                    ErrorCode = ErrorCodes.BadRequest
                };
            }
            catch (Exception)
            {
                return new ResponseDto<object>
                {
                    IsSuccess = false,
                    Message = "An error occurred while adding role to user.",
                    Data = null,
                    ErrorCode = ErrorCodes.Exception
                };
            }
        }
    }
}
