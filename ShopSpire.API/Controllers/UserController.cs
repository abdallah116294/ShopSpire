using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using ShopSpire.Utilities.DTO;
using ShopSpire.Utilities.Helpers;
using ShopSpireCore.Entities;
using ShopSpireCore.Services;

namespace ShopSpire.API.Controllers
{
    [Route("api/user")]
    [ApiController]
    public class UserController : BaseController
    {
        private readonly IUserService _userService;
        private readonly UserManager<User> userManager;
        private readonly TokenHelper _tokenHelper;

        public UserController(IUserService userService, TokenHelper tokenHelper, UserManager<User> userManager)
        {
            _userService = userService;
            _tokenHelper = tokenHelper;
            this.userManager = userManager;
        }

        [HttpPost("login")]
        public async Task<IActionResult> LoginAsync([FromBody] LoginDTO dto)
        {
            var response = await _userService.LoginAsync(dto);
            if (response.IsSuccess && response.Data != null)
            {
                return CreateResponse(new ResponseDto<object>
                {
                    IsSuccess = true,
                    Data = response,
                    Message = "Login successful."
                });
            }
            else
            {
                return CreateResponse(response);
            }
        }
        [HttpPost("register")]
        public async Task<IActionResult> RegisterAsync([FromBody] RegisterDTO dto)
        {
            var response = await _userService.RegisterAsync(dto);
            return CreateResponse(response);
        }
        [HttpPost("logout")]
        public async Task<IActionResult> LogoutAsync()
        {
            await _userService.LogoutAsync();
            return Ok(new ResponseDto<object> { IsSuccess = true, Message = "Logged out successfully." });
        }
        [HttpPost("forget-password")]
        public async Task<IActionResult> ForgetPassword([FromBody] ForgetPasswordDTO dto)
        {
            var response = await _userService.ForgetPassword(dto);
            return CreateResponse(response);
        }
        [HttpPost("reset-password")]
        public async Task<IActionResult> ResetPassword([FromBody] ResetPasswordDTO dto)
        {
            var response = await _userService.ResetPasswordAsync(dto.Email,dto.Token,dto.NewPassword);
            return CreateResponse(response);
        }
    }
}
