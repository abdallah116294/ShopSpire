using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using ShopSpire.Utilities.DTO;
using ShopSpire.Utilities.DTO.Cart;
using ShopSpireCore.Entities;
using ShopSpireCore.IRepositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authorization;
namespace ShopSpire.API.Controllers
{
    [Route("api/cart")]
    [ApiController]
    public class CartController : BaseController
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public CartController(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }
        [HttpGet("user/{userId}")]
        public async Task<IActionResult> GetCartItems(string userId)
        {
            try
            {
                var CartRepo = _unitOfWork.Repository<Cart>();
                var cartItems = await CartRepo.GetAllAsync(predicate:c=>c.UserId==userId,
                    query => query.Include(c => c.Product).ThenInclude(p => p.Category),
                query => query.Include(c => c.User)
                    );

                var cartDtos = cartItems.Select(c => new CartDTO
                {
                    Id = c.Id,
                    UserId = c.UserId,
                    ProductId = c.ProductId,
                    ProductName = c.Product.Name,
                    ProductPrice = c.Product.Price,
                    Quantity = c.Quantity,
                    TotalPrice = c.Product.Price * c.Quantity,
                    CategoryName = c.Product.Category?.Name
                }).ToList();

                return CreateResponse(new ResponseDto<IEnumerable<CartDTO>>
                {
                    IsSuccess = true,
                    Message = "Cart items retrieved successfully",
                    Data = cartDtos
                });
            }
            catch (Exception ex)
            {
                return CreateResponse(new ResponseDto<object>
                {
                    IsSuccess = false,
                    Message = $"An error occurred: {ex.Message}",
                    ErrorCode = ErrorCodes.Exception
                });
            }
        }
        [HttpPost("add")]
        public async Task<IActionResult> AddToCart([FromBody] AddToCartDTO dto, [FromQuery] string userId)
        {
            try
            {
                // Check if product exists
                var CartRepo = _unitOfWork.Repository<Cart>();
                var ProductRepo = _unitOfWork.Repository<Product>();

                var product = await ProductRepo.GetByIdAsync(dto.ProductId);
                if (product == null)
                {
                    return CreateResponse(new ResponseDto<object>
                    {
                        IsSuccess = false,
                        Message = "Product not found",
                        ErrorCode = ErrorCodes.NotFound
                    });
                }

                // Check if item already exists in cart
                var existingCartItem = await CartRepo.GetAllAsync(
                    predicate: c => c.UserId == userId && c.ProductId == dto.ProductId,
                       query => query.Include(c => c.Product).ThenInclude(p => p.Category),
                query => query.Include(c => c.User)

                );

                if (existingCartItem.Any())
                {
                    // Update quantity if item exists
                    var cartItem = existingCartItem.First();
                    await _unitOfWork.Repository<Cart>().UpdateAsync(cartItem.Id, entity =>
                    {
                        entity.Quantity += dto.Quantity;
                        entity.UpdatedAt = DateTime.UtcNow;
                    });
                }
                else
                {
                    // Add new item to cart
                    var newCartItem = new Cart
                    {
                        UserId = userId,
                        ProductId = dto.ProductId,
                        Quantity = dto.Quantity,
                        CreatedAt = DateTime.UtcNow,
                        UpdatedAt = DateTime.UtcNow
                    };

                    await _unitOfWork.Repository<Cart>().AddAsync(newCartItem);
                }

                await _unitOfWork.CompleteAsync();

                return CreateResponse(new ResponseDto<object>
                {
                    IsSuccess = true,
                    Message = "Item added to cart successfully"
                });
            }
            catch (Exception ex)
            {
                return CreateResponse(new ResponseDto<object>
                {
                    IsSuccess = false,
                    Message = $"An error occurred: {ex.Message}",
                    ErrorCode = ErrorCodes.Exception
                });
            }
        }

        [HttpDelete("{cartId}")]
        public async Task<IActionResult> RemoveFromCart(int cartId)
        {
            try
            {
                var cartItem = await _unitOfWork.Repository<Cart>().GetByIdAsync(cartId);
                if (cartItem == null)
                {
                    return CreateResponse(new ResponseDto<object>
                    {
                        IsSuccess = false,
                        Message = "Cart item not found",
                        ErrorCode = ErrorCodes.NotFound
                    });
                }

                await _unitOfWork.Repository<Cart>().DeleteAsync(cartId);
                await _unitOfWork.CompleteAsync();

                return CreateResponse(new ResponseDto<object>
                {
                    IsSuccess = true,
                    Message = "Item removed from cart successfully"
                });
            }
            catch (Exception ex)
            {
                return CreateResponse(new ResponseDto<object>
                {
                    IsSuccess = false,
                    Message = $"An error occurred: {ex.Message}",
                    ErrorCode = ErrorCodes.Exception
                });
            }
        }
        [HttpDelete("clear/{userId}")]
        public async Task<IActionResult> ClearCart(string userId)
        {
            try
            {
                var cartItems = await _unitOfWork.Repository<Cart>().GetAllAsync(
                    predicate: c => c.UserId == userId,
                        query => query.Include(c => c.Product).ThenInclude(p => p.Category),
query => query.Include(c => c.User)
                );

                foreach (var item in cartItems)
                {
                    await _unitOfWork.Repository<Cart>().DeleteAsync(item.Id);
                }

                await _unitOfWork.CompleteAsync();

                return CreateResponse(new ResponseDto<object>
                {
                    IsSuccess = true,
                    Message = "Cart cleared successfully"
                });
            }
            catch (Exception ex)
            {
                return CreateResponse(new ResponseDto<object>
                {
                    IsSuccess = false,
                    Message = $"An error occurred: {ex.Message}",
                    ErrorCode = ErrorCodes.Exception
                });
            }
        }
        [HttpGet("summary/{userId}")]
        public async Task<IActionResult> GetCartSummary(string userId)
        {
            try
            {
                var cartItems = await _unitOfWork.Repository<Cart>().GetAllAsync(
                    predicate: c => c.UserId == userId,
                    query => query.Include(c => c.Product)
                );

                var summary = new
                {
                    TotalItems = cartItems.Sum(c => c.Quantity),
                    TotalPrice = cartItems.Sum(c => c.Product.Price * c.Quantity),
                    ItemCount = cartItems.Count()
                };

                return CreateResponse(new ResponseDto<object>
                {
                    IsSuccess = true,
                    Message = "Cart summary retrieved successfully",
                    Data = summary
                });
            }
            catch (Exception ex)
            {
                return CreateResponse(new ResponseDto<object>
                {
                    IsSuccess = false,
                    Message = $"An error occurred: {ex.Message}",
                    ErrorCode = ErrorCodes.Exception
                });
            }
        }

    }
}
