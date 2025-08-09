using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using ShopSpire.Utilities.DTO;
using ShopSpireCore.Entities;
using ShopSpireCore.IRepositories;

namespace ShopSpire.API.Controllers
{
    [Route("api/category")]
    [ApiController]
    public class CategoryController : BaseController
    {
        //Unit Of wok
        private readonly IUnitOfWork _unitOfWork;

        public CategoryController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }
        //Get All Categorires
        [HttpGet("GetAllCategories")]
        public async Task<IActionResult> GetAllCategories()
        {
            try
            {
                var categories = await _unitOfWork.Repository<Category>().GetAllAsync();
                if (categories == null)
                {
                    return CreateResponse(new ResponseDto<object>
                    {
                        IsSuccess = false,
                        Message = "Not Categories Found",
                        ErrorCode = ErrorCodes.NotFound
                    });
                }
                return CreateResponse(new ResponseDto<IEnumerable<Category>>
                {
                    IsSuccess = true,
                    Message = "Get Categories Succesful",
                    Data = categories
                });

            } catch (Exception ex)
            {
                return CreateResponse(new ResponseDto<object>
                {
                    IsSuccess = false,
                    Message = ex.Message,
                    ErrorCode = ErrorCodes.Exception
                });

            }
        }
        [HttpPost("CreateCategory")]
        public async Task<IActionResult> CreateCategory([FromForm] CreateCategoryDTO dto)
        {
            try
            {
                var catgoryRepository = _unitOfWork.Repository<Category>();
                var categorMaped = new Category
                {
                    Name = dto.Name,
                    Description = dto.Description
                };
                await catgoryRepository.AddAsync(categorMaped);
                await _unitOfWork.CompleteAsync();
                return CreateResponse(new ResponseDto<Category>
                {
                    IsSuccess = true,
                    Message = "Add Category Succesful",
                    Data = categorMaped,
                });
            } catch (Exception ex)
            {
                return CreateResponse(new ResponseDto<object>
                {
                    IsSuccess = false,
                    Message = $"Error Accured While Add Catgory {ex}",
                    ErrorCode = ErrorCodes.Exception
                });

            }
        }
        [HttpGet("GetCategoryById{id}")]
        public async Task<IActionResult> GetCategoryById(int id)
        {
            try
            {
                var catgoryRepository = _unitOfWork.Repository<Category>();
                var category = await catgoryRepository.GetByIdAsync(id);
                if (category == null)
                {
                    return CreateResponse(new ResponseDto<object>
                    {
                        IsSuccess = false,
                        Message = "no category found with this Id",
                        ErrorCode = ErrorCodes.NotFound,
                    });
                }
                return CreateResponse(new ResponseDto<Category>
                {
                    IsSuccess = true,
                    Message = "Get Category Successful",
                    Data = category,
                });

            }
            catch
            {
                return CreateResponse(new ResponseDto<object>
                {
                    IsSuccess = false,
                    Message = "Error happened While Getting Category with Id",
                    ErrorCode = ErrorCodes.Exception,
                });
            }

        }
        [HttpPut("UpdateCategory")]
        public async Task<IActionResult> UpdateCategory(int id, [FromForm] UpdateCategoryDTO dto)
        {
            try
            {
                var catgoryRepository = _unitOfWork.Repository<Category>();
               
                await catgoryRepository.UpdateAsync(id, 
                    entity => 
                    {
                        if (!string.IsNullOrEmpty(dto.Name)) entity.Name = dto.Name;
                        if (!string.IsNullOrEmpty(dto.Description)) entity.Description = dto.Description;
                    });
                await _unitOfWork.CompleteAsync();
                var category = await catgoryRepository.GetByIdAsync(id);
                return CreateResponse(new ResponseDto<Category>
                {
                    IsSuccess = true,
                    Message = "Update Categor Succesful",
                    Data = category
                });
            }
            catch (Exception ex)
            {
                return CreateResponse(new ResponseDto<object>
                {
                    IsSuccess = false,
                    Message =$"Error While Update Categor {ex.Message} ",
                    ErrorCode=ErrorCodes.Exception
                });
            }
        }
        [HttpDelete("DeleteCategory{id}")]
        public async  Task<IActionResult>DeleteCategory(int id)
        {
            try
            {
                var catgoryRepository = _unitOfWork.Repository<Category>();
                if(id==null)
                {
                    return CreateResponse(new ResponseDto<object> 
                    {
                        IsSuccess=false,
                        Message="Id is null",
                        ErrorCode=ErrorCodes.BadRequest,
                    });

                }
                await catgoryRepository.DeleteAsync(id);
                await _unitOfWork.CompleteAsync();
                return CreateResponse(new ResponseDto<object> 
                {
                  IsSuccess=true,
                  Message="Delete Category Succesful"
                });
            }catch(Exception ex)
            {
                return CreateResponse(new ResponseDto<object>
                {
                    IsSuccess = false,
                    Message = $"Error While Delete Category {ex}",
                    ErrorCode=ErrorCodes.Exception,
                });
            }
        } 
    }

}
