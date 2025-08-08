//using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Identity;
using ShopSpire.Utilities.DTO;
using ShopSpire.Utilities.Helpers;
using ShopSpireCore.Entities;
using ShopSpireCore.IRepositories;
using ShopSpireCore.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

namespace ShopSpire.Service
{
    public class UserService : IUserService
    {
        private readonly IUserRepository _userRepository;
        private readonly UserManager<User> _userManager;
        private readonly TokenHelper _tokenHelper;

        public UserService(IUserRepository userRepository, UserManager<User> userManager, TokenHelper tokenHelper )
        {
            _userRepository = userRepository;
            _userManager = userManager;
            _tokenHelper = tokenHelper;
        }

        public async Task<ResponseDto<object>> LoginAsync(LoginDTO dto)
        {
            try
            {
                var result = await _userRepository.LoginAsync(dto);
                if (result.Succeeded)
                {
                    var user = await _userManager.FindByEmailAsync(dto.Email);
                    if (user == null)
                    {
                        return new ResponseDto<object>
                        {
                            IsSuccess=false,
                            Message="User Not Found",
                            ErrorCode=ErrorCodes.BadRequest
                        };
                    }
                    var roles = await _userManager.GetRolesAsync(user);
                    var token = _tokenHelper.GenerateToken(new 
                        TokenDTO
                    { 
                        Email=dto.Email,
                        Id=user.Id,
                        Role=roles.FirstOrDefault()??"USER",
                    });
                    return new ResponseDto<object>
                    {
                        IsSuccess =true,
                        Data = new
                        {         
                            Email= dto.Email,
                            Token = token
                        },
                        Message="Login User Succes",

                    }; 
                }
                return new ResponseDto<object> 
                {
                   IsSuccess=false,
                   Message="Faild to Login",
                   ErrorCode= ErrorCodes.BadRequest,
                };
            }
            catch (Exception ex)
            {
                return new ResponseDto<object> 
                {
                    IsSuccess=false,
                    Message=$"An Error Happedn While Login user {ex}",
                    ErrorCode= ErrorCodes.Exception,
                };
            }
        }

        public async Task LogoutAsync()
        {
          await _userRepository.LogoutAsync();
        }

        public async Task<ResponseDto<object>> RegisterAsync(RegisterDTO dto)
        {
            try
            {
                var result = await _userRepository.RegisterAsync(dto);
                if (result.Succeeded)
                    return new ResponseDto<object>
                    {
                        IsSuccess=true,
                        Message="Resgiter Successful ",
                        Data=dto,
                    };
                return new ResponseDto<object>
                {
                    IsSuccess = false,
                    Message = "Error while Register User",
                    ErrorCode = ErrorCodes.BadRequest,
                };
            }catch(Exception ex)
            {
                return new ResponseDto<object>
                {
                    IsSuccess = false,
                    Message = $"An  Error Accured while Register User {ex}",
                    ErrorCode = ErrorCodes.Exception,
                };
            }
        }
    }
}
