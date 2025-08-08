using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using ShopSpire.Utilities.DTO;

namespace ShopSpire.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BaseController : ControllerBase
    {
        protected IActionResult CreateResponse<T>(ResponseDto<T> response) where T : class 
        {
            if (response.IsSuccess)
                return Ok(response);
            return response.ErrorCode switch
            {
                ErrorCodes.ValidationError => BadRequest(response),
                ErrorCodes.BadRequest => BadRequest(response),
                ErrorCodes.Unauthorized => Unauthorized(response),
                ErrorCodes.Forbidden => StatusCode(StatusCodes.Status403Forbidden, response),
                ErrorCodes.NotFound => NotFound(response),
                ErrorCodes.Exception => StatusCode(StatusCodes.Status500InternalServerError, response),
                ErrorCodes.DuplicateError => Conflict(response),
                _ => BadRequest(response)
            };
        }
        protected string GetUserId()
        {
            return User?.Claims.FirstOrDefault(x => x.Type == "_e")?.Value ?? string.Empty;
        }

    }
}
