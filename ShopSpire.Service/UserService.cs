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
        private readonly IEmailService _emailService;

        public UserService(IUserRepository userRepository, UserManager<User> userManager, TokenHelper tokenHelper, IEmailService emailService)
        {
            _userRepository = userRepository;
            _userManager = userManager;
            _tokenHelper = tokenHelper;
            _emailService = emailService;
        }

        public async Task<ResponseDto<object>> ForgetPassword(ForgetPasswordDTO dto)
        {
            try
            {
                var user= await _userRepository.FindByEmailAsync(dto.Email);
                if (user == null) 
                {
                    return new ResponseDto<object>
                    {
                        IsSuccess = false,
                        Message = "User not Found",
                        ErrorCode=ErrorCodes.NotFound,
                        
                    };
                }
                var otpCode=new Random().Next(100000, 999999).ToString();
                var result = await _userRepository.SetOtpAync(user,otpCode);
                if (!result)
                {
                    return new ResponseDto<object>
                    {
                        IsSuccess = false,
                        Message = "Erro while Generate Otp"
                    };

                }
                await _emailService.SendEmailAsync(user.Email,"Reset Password Code", $"Your OTP code is: {otpCode}");
                return new ResponseDto<object>
                {
                    IsSuccess=true,
                    Message="Otp Send to your Email"
                };
            }
            catch(Exception ex)
            {
                return new ResponseDto<object> 
                {
                    IsSuccess=false,
                    Message="An Error Accured While Foreget Password"
                };
            }
        }

        public async Task<ResponseDto<object>> GetAllSeller()
        {
            try
            {
                var users = await _userRepository.GetAllSeller();
                if(users == null)
                {
                    return new ResponseDto<object>
                    {
                        IsSuccess = false,
                        Message = "No Seller Found",
                        ErrorCode=ErrorCodes.NotFound,
                    };
                }
                return new ResponseDto<object>
                {
                    IsSuccess = true,
                    Message = "Get All Sellers",
                    Data = users
                };
            }
            catch(Exception ex)
            {
                return new ResponseDto<object>
                {
                    IsSuccess = false,
                    Message = "An Error Accured While Foreget Password"
                };
            }
        }

        public async Task<User> GetUserByID(string id)
        {
            return await _userRepository.GetUserById(id);
        }

        public async Task<ResponseDto<object>> LoginAsync(LoginDTO dto)
        {
            try
            {
                var result = await _userRepository.LoginAsync(dto);
                if (result==true)
                {
                    var user = await _userManager.FindByEmailAsync(dto.Email);
                    var roles = await _userManager.GetRolesAsync(user);
                    var token = _tokenHelper.GenerateToken(new
                        TokenDTO
                    {
                        Email = dto.Email,
                        Id = user.Id,
                        Role = roles.FirstOrDefault(),
                    });
                    return new ResponseDto<object>
                    {
                        IsSuccess = true,
                        Data = new
                        {
                            Email = dto.Email,
                            Token = token
                        },
                        Message = "Login User Succes",

                    };
                }
                return new ResponseDto<object>
                {
                    IsSuccess = false,
                    Message = "Faild to Login",
                    ErrorCode = ErrorCodes.BadRequest,
                };
            }
            catch (Exception ex)
            {
                return new ResponseDto<object>
                {
                    IsSuccess = false,
                    Message = $"An Error Happedn While Login user {ex}",
                    ErrorCode = ErrorCodes.Exception,
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
                        IsSuccess = true,
                        Message = "Resgiter Successful ",
                        Data = dto,
                    };
                return new ResponseDto<object>
                {
                    IsSuccess = false,
                    Message = "Error while Register User",
                    ErrorCode = ErrorCodes.BadRequest,
                };
            }
            catch (Exception ex)
            {
                return new ResponseDto<object>
                {
                    IsSuccess = false,
                    Message = $"An  Error Accured while Register User {ex}",
                    ErrorCode = ErrorCodes.Exception,
                };
            }
        }

        public async Task<ResponseDto<object>> ResetPasswordAsync(string email, string otp, string newPassword)
        {
            try
            {
                var user = await _userRepository.FindByEmailAsync(email);
                if (user == null)
                {
                    return new ResponseDto<object>
                    {
                        IsSuccess = false,
                        Message = "No User Found",
                        ErrorCode = ErrorCodes.NotFound,
                    };
                }
                var storedOtp = await _userRepository.GetOtpAsyn(user);
                if (storedOtp == null || storedOtp != otp) 
                {
                    return new ResponseDto<object>
                    {
                        IsSuccess = false,
                        Message = "Invalid or expired OTP"
                    };
                }
                var resetToken = await _userRepository.GeneratePasswordResetTokenAsync(user);
                var resetResult = await _userRepository.ResetPasswordAsync(user, resetToken, newPassword);
                if(!resetResult.Succeeded)
                    return new ResponseDto<object>()
                    {
                        IsSuccess=false,
                        Message= "Error while resetting your password"
                    };
                await _userRepository.RemoveOtpAsync(user);
                return new ResponseDto<object>
                {
                    IsSuccess = true,
                    Message = "Password has been reset successfully."
                };
            }catch(Exception ex)
            {
                return new ResponseDto<object>
                {
                    IsSuccess = false,
                    Message = "Error Whilre Reset your Password",
                    ErrorCode = ErrorCodes.Exception,
                };
            }
        }
    }
}
