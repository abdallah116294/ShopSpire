using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ShopSpire.Utilities.DTO;
using ShopSpireCore.Entities;
using ShopSpireCore.IRepositories;

namespace ShopSpire.API.Controllers
{
    [Route("api/product")]
    [ApiController]
    public class ProductController : BaseController
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        public ProductController(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }
        //Get All Products 
        [HttpGet("GetAllProducts")]
        public async Task<IActionResult> GetAllProducts() 
        {
            try
            {
                var _productRepository = _unitOfWork.Repository<Product>();
                var products = await _productRepository.GetAllAsync(
        predicate: null, // No filter
        query => query.Include(p => p.Category),
        query => query.Include(p => p.Seller)
    );
                if (products == null) 
                {
                    return CreateResponse(new ResponseDto<object> 
                    {
                        IsSuccess=false,
                        Message="No Product Found",
                        ErrorCode=ErrorCodes.NotFound,
                    }); 
                }
                var mapedProduct=_mapper.Map<IEnumerable <ProductDTO>>(products);
                return CreateResponse(new ResponseDto<IEnumerable <ProductDTO>> 
                {
                    IsSuccess=true,
                    Message="Get All Products Succesfful",
                    Data=mapedProduct
                });
            }
            catch(Exception ex)
            {
                return CreateResponse(new ResponseDto<object>
                {
                    IsSuccess=false,
                    Message= $"An Error Accured {ex}",
                    ErrorCode=ErrorCodes.Exception,
                });
            }
        }
        [HttpGet("GetSelectedProduct{id}")]
        public async Task<IActionResult>GetSelectedProduct(int id)
        {
            try
            {
                if (id == null)
                    return CreateResponse(new ResponseDto<object> { IsSuccess=false,Message="Id must not empty"});
                var product=await _unitOfWork.Repository<Product>().GetByIdAsync(id,
                     query => query.Include(p => p.Category),
                    query => query.Include(p => p.Seller)
                    );
                if (product == null)
                {
                    return CreateResponse(new ResponseDto<object> 
                    {
                        IsSuccess=false,
                        Message="No Product Found",
                        ErrorCode=ErrorCodes.NotFound,
                    });
                }
                var mappedProduct=_mapper.Map<ProductDTO>(product);
                return CreateResponse(new ResponseDto<ProductDTO> 
                {
                    IsSuccess=true,
                    Message="Get Product Succesful",
                    Data=mappedProduct,
                });
            }
            catch (Exception ex) 
            {
                return CreateResponse(new ResponseDto<object>
                {
                    IsSuccess = false,
                    Message = $"An Error Accured {ex}",
                    ErrorCode = ErrorCodes.Exception,
                });
            }
        }
        [HttpGet("GetProductsByCategoryId{categoryId}")]
        public async Task<IActionResult>GetProductsByCategoryId(int categoryId)
        {
            try
            {
                var category =await _unitOfWork.Repository<Category>().GetByIdAsync(categoryId);
                if (category == null) 
                {
                    return CreateResponse(new ResponseDto<object>
                    {
                        IsSuccess = false,
                        Message = $"No Category Found ",
                        ErrorCode = ErrorCodes.NotFound,
                    });
                }
                var products = await _unitOfWork.Repository<Product>().GetAllAsync(
                    predicate:p=>p.CategoryId==category.Id,
                    query=>query.Include(p=>p.Category),
                    query=>query.Include(p=>p.Seller)
                    );
                var mappedProducts = _mapper.Map<IEnumerable<ProductDTO>>(products);
                return CreateResponse(new ResponseDto<IEnumerable <ProductDTO>>
                {
                    IsSuccess = true,
                    Message = $"Get All Product By Category",
                    Data= mappedProducts,
                });
            }
            catch(Exception ex) 
            {
                return CreateResponse(new ResponseDto<object>
                {
                    IsSuccess = false,
                    Message = $"An Error Accured {ex}",
                    ErrorCode = ErrorCodes.Exception,
                });
            }
        }
        [Authorize]
        [HttpPost("CreateProduct")]
        public async Task<IActionResult> CreateProduct([FromForm]ProductCreateDTO dTO) 
        {
            try
            {
                var productRepo=_unitOfWork.Repository<Product>();
                var product = _mapper.Map<Product>(dTO);
                await productRepo.AddAsync(product);
                await _unitOfWork.CompleteAsync();
                var productDto = _mapper.Map<ProductDTO>(product);
                return CreateResponse(new ResponseDto<object>
                {
                    IsSuccess = true,
                    Message = "Create Product Succesful",
                    Data = productDto,
                });

            }
            catch(Exception ex)
            {
                return CreateResponse(new ResponseDto<object>
                {
                    IsSuccess = false,
                    Message = $"An Error Accured {ex}",
                    ErrorCode = ErrorCodes.Exception,
                });
            }
        }
        [Authorize]
        [HttpDelete("DeleteProduct{id}")]
        public async Task<IActionResult>DeleteProduct(int id) 
        {
            try
            {
                var productRepo = _unitOfWork.Repository<Product>();
                  await productRepo.DeleteAsync(id);
                await _unitOfWork.CompleteAsync();
                return CreateResponse(new ResponseDto<object> 
                {
                    IsSuccess=true,
                    Message="Delete Product Succeful"
                });
                
            }catch(Exception ex)
            {
                return CreateResponse(new ResponseDto<object>
                {
                    IsSuccess = false,
                    Message = $"An Error Accured {ex}",
                    ErrorCode = ErrorCodes.Exception,
                });

            }
        }
        [HttpPut("UpdateProduct{id}")]
        public async Task<IActionResult>UpdateProduct(int id, [FromForm]UpdateProductDTO dto)
        {
            try
            {
                var productRepo = _unitOfWork.Repository<Product>();
                await productRepo.UpdateAsync(id, entity => 
                {
                    if (!string.IsNullOrEmpty(dto.Name)) entity.Name = dto.Name;
                    if (!string.IsNullOrEmpty(dto.Dscription)) entity.Description = dto.Dscription;
                    if (dto.Price==0 && dto.Price > 0) entity.Price = dto.Price.Value;
                });
                await _unitOfWork.CompleteAsync();
                var productAfterUpdate  =await productRepo.GetByIdAsync(id,query=>query.Include(p=>p.Category),query=>query.Include(p=>p.Seller));  
                return CreateResponse(new ResponseDto<object>
                {
                    IsSuccess=true,
                    Message="Update Product Succesful",
                    Data= productAfterUpdate,
                });
            }catch(Exception ex)
            {
                return CreateResponse(new ResponseDto<object>
                {
                    IsSuccess = false,
                    Message = $"An Error Accured {ex}",
                    ErrorCode = ErrorCodes.Exception,
                });
            }
        }
      
    }
}
