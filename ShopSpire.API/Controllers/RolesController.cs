using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using ShopSpire.Utilities.DTO;
using ShopSpireCore.Services;

namespace ShopSpire.API.Controllers
{
    [Route("api/role")]
    [ApiController]
    public class RolesController : BaseController
    {
        private readonly IRoleService _roleService;

        public RolesController(IRoleService roleService)
        {
            _roleService = roleService;
        }
        [HttpPost]
        public async Task<IActionResult> AddRoleTouser([FromForm] AddRoleToUserDTO dto)
        {
            var result = await _roleService.AddRoleToUserAsync(dto.Email, dto.RoleName);
            return CreateResponse(result);
        }
    }
}
