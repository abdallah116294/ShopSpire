using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using ShopSpire.Utilities.DTO;
using ShopSpire.Utilities.DTO.Order;
using ShopSpireCore.Entities;
using ShopSpireCore.IRepositories;
using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore;

namespace ShopSpire.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class OrderController : BaseController
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public OrderController(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        // Create order from cart
        [HttpPost("create-from-cart")]
        public async Task<IActionResult> CreateOrderFromCart([FromBody] CreateOrderDTO dto, [FromQuery] string userId)
        {
            try
            {
                // Get user's cart items
                var cartItems = await _unitOfWork.Repository<Cart>().GetAllAsync(
                    predicate: c => c.UserId == userId,
                    query => query.Include(c => c.Product)
                );

                if (!cartItems.Any())
                {
                    return CreateResponse(new ResponseDto<object>
                    {
                        IsSuccess = false,
                        Message = "Cart is empty",
                        ErrorCode = ErrorCodes.BadRequest
                    });
                }

                // Create new order
                var order = new Order
                {
                    UserId = userId,
                    OrderDate = DateTime.UtcNow,     
                    Status = OrderStatus.Pending
                };

                await _unitOfWork.Repository<Order>().AddAsync(order);
                await _unitOfWork.CompleteAsync(); // Save to get OrderId

                decimal totalAmount = 0;

                // Create ProductOrder entries
                foreach (var cartItem in cartItems)
                {
                    var productOrder = new ProductOrder
                    {
                        OrderId = order.Id,
                        ProductId = cartItem.ProductId,
                        Quantity = cartItem.Quantity,
                        UnitPrice = cartItem.Product.Price // Store current price
                    };

                    await _unitOfWork.Repository<ProductOrder>().AddAsync(productOrder);
                    totalAmount += productOrder.UnitPrice * productOrder.Quantity;
                }

                // Update order total
                await _unitOfWork.Repository<Order>().UpdateAsync(order.Id, entity =>
                {
                    entity.TotalAmount = totalAmount;
                });

                // Clear cart
                foreach (var cartItem in cartItems)
                {
                    await _unitOfWork.Repository<Cart>().DeleteAsync(cartItem.Id);
                }

                await _unitOfWork.CompleteAsync();

                return CreateResponse(new ResponseDto<object>
                {
                    IsSuccess = true,
                    Message = "Order created successfully",
                    Data = new { OrderId = order.Id, TotalAmount = totalAmount }
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

        // Get order with details
        [HttpGet("{orderId}")]
        public async Task<IActionResult> GetOrder(int orderId)
        {
            try
            {
                var order = await _unitOfWork.Repository<Order>().GetByIdAsync(orderId,
                    query => query.Include(o => o.ProductOrders).ThenInclude(p=>p.Product),
                    query => query.Include(o => o.User)
                );

                if (order == null)
                {
                    return CreateResponse(new ResponseDto<object>
                    {
                        IsSuccess = false,
                        Message = "Order not found",
                        ErrorCode = ErrorCodes.NotFound
                    });
                }

                var orderDto = new OrderDTO
                {
                    OrderId = order.Id,
                    UserId = order.UserId,
                    OrderDate = order.OrderDate,
                    TotalAmount = order.TotalAmount,
                    Status = order.Status.ToString(),
                    Items = order.ProductOrders.Select(po => new ProductOrderDTO
                    {
                        Id = po.Id,
                        OrderId = po.OrderId,
                        ProductId = po.ProductId,
                        ProductName = po.Product.Name,
                        UnitPrice = po.UnitPrice,
                        Quantity = po.Quantity,
                        LineTotal = po.UnitPrice * po.Quantity
                    }).ToList()
                };

                return CreateResponse(new ResponseDto<OrderDTO>
                {
                    IsSuccess = true,
                    Message = "Order retrieved successfully",
                    Data = orderDto
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

        // Get user's orders
        [HttpGet("user/{userId}")]
        public async Task<IActionResult> GetUserOrders(string userId)
        {
            try
            {
                var orders = await _unitOfWork.Repository<Order>().GetAllAsync(
                    predicate: o => o.UserId == userId,
                    query => query.Include(o => o.ProductOrders).ThenInclude(po => po.Product)
                );

                var orderDtos = orders.Select(order => new OrderDTO
                {
                    OrderId = order.Id,
                    UserId = order.UserId,
                    OrderDate = order.OrderDate,
                    TotalAmount = order.TotalAmount,
                    Status = order.Status.ToString(),
                    Items = order.ProductOrders.Select(po => new ProductOrderDTO
                    {
                        Id = po.Id,
                        OrderId = po.OrderId,
                        ProductId = po.ProductId,
                        ProductName = po.Product.Name,
                        UnitPrice = po.UnitPrice,
                        Quantity = po.Quantity,
                        LineTotal = po.UnitPrice * po.Quantity
                    }).ToList()
                }).OrderByDescending(o => o.OrderDate).ToList();

                return CreateResponse(new ResponseDto<IEnumerable<OrderDTO>>
                {
                    IsSuccess = true,
                    Message = "Orders retrieved successfully",
                    Data = orderDtos
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

        // Update order status
        [HttpPut("{orderId}/status")]
        public async Task<IActionResult> UpdateOrderStatus(int orderId, [FromBody] UpdateOrderStatusDTO dto)
        {
            try
            {
                var order = await _unitOfWork.Repository<Order>().GetByIdAsync(orderId);
                if (order == null)
                {
                    return CreateResponse(new ResponseDto<object>
                    {
                        IsSuccess = false,
                        Message = "Order not found",
                        ErrorCode = ErrorCodes.NotFound
                    });
                }

                await _unitOfWork.Repository<Order>().UpdateAsync(orderId, entity =>
                {
                    entity.Status = dto.Status;
                });

                await _unitOfWork.CompleteAsync();

                return CreateResponse(new ResponseDto<object>
                {
                    IsSuccess = true,
                    Message = "Order status updated successfully"
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

    public class UpdateOrderStatusDTO
    {
        [Required]
        public OrderStatus Status { get; set; }
    }
}
