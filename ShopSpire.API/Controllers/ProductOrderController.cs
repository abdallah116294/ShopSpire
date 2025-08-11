using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ShopSpire.Utilities.DTO.Order;
using ShopSpireCore.Entities;
using ShopSpireCore.IRepositories;
using ShopSpireCore.Services;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
namespace ShopSpire.API.Controllers
{
    [Route("api/ProductOrder")]
    [ApiController]
    public class ProductOrderController : BaseController
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly ILogger<ProductOrderController> _logger;

        public ProductOrderController(
            IUnitOfWork unitOfWork,
            IMapper mapper,
            ILogger<ProductOrderController> logger)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _logger = logger;
        }

        // 1. GetAllProductOrders
        [HttpGet]
        public async Task<ActionResult<IEnumerable<ProductOrderDTO>>> GetAllProductOrders()
        {
            try
            {
                var productOrders = await _unitOfWork.Repository<ProductOrder>()
                    .GetAllAsync(
                        predicate: null,
                        po => po.Product,
                        po => po.Order
                    );

                var productOrderDtos = _mapper.Map<IEnumerable<ProductOrderDTO>>(productOrders);

                return Ok(productOrderDtos);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while getting all product orders");
                return StatusCode(500, "An error occurred while processing your request");
            }
        }

        // 2. GetSelectedProductOrder
        [HttpGet("{id}")]
        public async Task<ActionResult<ProductOrderDTO>> GetSelectedProductOrder(int id)
        {
            try
            {
                var productOrder = await _unitOfWork.Repository<ProductOrder>()
                    .GetByIdAsync(id, po => po.Product, po => po.Order);

                if (productOrder == null)
                {
                    return NotFound($"Product order with ID {id} not found");
                }

                var productOrderDto = _mapper.Map<ProductOrderDTO>(productOrder);
                return Ok(productOrderDto);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while getting product order with ID {Id}", id);
                return StatusCode(500, "An error occurred while processing your request");
            }
        }

        // 3. GetProductOrdersByOrder
        [HttpGet("order/{orderId}")]
        public async Task<ActionResult<IEnumerable<ProductOrderDTO>>> GetProductOrdersByOrder(int orderId)
        {
            try
            {
                var productOrders = await _unitOfWork.Repository<ProductOrder>()
                    .GetAllAsync(
                        predicate: po => po.OrderId == orderId,
                        po => po.Product,
                        po => po.Order
                    );

                if (!productOrders.Any())
                {
                    return NotFound($"No product orders found for order ID {orderId}");
                }

                var productOrderDtos = _mapper.Map<IEnumerable<ProductOrderDTO>>(productOrders);
                return Ok(productOrderDtos);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while getting product orders for order ID {OrderId}", orderId);
                return StatusCode(500, "An error occurred while processing your request");
            }
        }

        // 4. GetProductOrdersBySeller
        [HttpGet("seller/{sellerId}")]
        public async Task<ActionResult<IEnumerable<ProductOrderDTO>>> GetProductOrdersBySeller(string sellerId)
        {
            try
            {
                // Assuming Product has a SellerId property
                var productOrders = await _unitOfWork.Repository<ProductOrder>()
                    .GetAllAsync(
                        predicate: po => po.Product.Seller.Id == sellerId,
                        po => po.Product,
                        po => po.Order
                    );

                if (!productOrders.Any())
                {
                    return NotFound($"No product orders found for seller ID {sellerId}");
                }

                var productOrderDtos = _mapper.Map<IEnumerable<ProductOrderDTO>>(productOrders);
                return Ok(productOrderDtos);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while getting product orders for seller ID {SellerId}", sellerId);
                return StatusCode(500, "An error occurred while processing your request");
            }
        }

        // 5. NewProductOrder
        [HttpPost]
        public async Task<ActionResult<ProductOrderDTO>> NewProductOrder([FromBody] CreateProductOrderDTO createDto)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                // Validate that Order exists
                var order = await _unitOfWork.Repository<Order>().GetByIdAsync(createDto.OrderId);
                if (order == null)
                {
                    return BadRequest($"Order with ID {createDto.OrderId} does not exist");
                }

                // Validate that Product exists
                var product = await _unitOfWork.Repository<Product>().GetByIdAsync(createDto.ProductId);
                if (product == null)
                {
                    return BadRequest($"Product with ID {createDto.ProductId} does not exist");
                }

                // Check if ProductOrder already exists for this Order-Product combination
                var existingProductOrders = await _unitOfWork.Repository<ProductOrder>().GetAllAsync(
                                        predicate: po => po.OrderId == createDto.OrderId && po.ProductId == createDto.ProductId,
                    query => query.Include(p => p.Product),
                    query => query.Include(p => p.Order)
                    );

                if (existingProductOrders.Any())
                {
                    return BadRequest("Product order already exists for this order-product combination");
                }

                var productOrder = new ProductOrder
                {
                    OrderId = createDto.OrderId,
                    ProductId = createDto.ProductId,
                    Quantity = createDto.Quantity,
                    UnitPrice = createDto.UnitPrice,
                    CreatedAt = DateTime.UtcNow
                };

                await _unitOfWork.Repository<ProductOrder>().AddAsync(productOrder);
                await _unitOfWork.CompleteAsync();

                // Fetch the created entity with includes
                var createdProductOrder = await _unitOfWork.Repository<ProductOrder>()
                    .GetByIdAsync(productOrder.Id, po => po.Product, po => po.Order);

                var productOrderDto = _mapper.Map<ProductOrderDTO>(createdProductOrder);

                return CreatedAtAction(
                    nameof(GetSelectedProductOrder),
                    new { id = productOrder.Id },
                    productOrderDto
                );
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while creating new product order");
                return StatusCode(500, "An error occurred while processing your request");
            }
        }

        // 6. UpdateProductOrder
        [HttpPut("{id}")]
        public async Task<ActionResult<ProductOrderDTO>> UpdateProductOrder(int id, [FromBody] UpdateProductOrderDTO updateDto)
        {
            try
            {
                if (id != updateDto.Id)
                {
                    return BadRequest("ID mismatch between route and body");
                }

                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                var productOrder = await _unitOfWork.Repository<ProductOrder>().GetByIdAsync(id);
                if (productOrder == null)
                {
                    return NotFound($"Product order with ID {id} not found");
                }

                // Update using the repository's UpdateAsync method with action
                await _unitOfWork.Repository<ProductOrder>().UpdateAsync(id, po =>
                {
                    po.Quantity = updateDto.Quantity;
                    po.UnitPrice = updateDto.UnitPrice;
                });

                await _unitOfWork.CompleteAsync();

                // Fetch updated entity with includes
                var updatedProductOrder = await _unitOfWork.Repository<ProductOrder>()
                    .GetByIdAsync(id, po => po.Product, po => po.Order);

                var productOrderDto = _mapper.Map<ProductOrderDTO>(updatedProductOrder);
                return Ok(productOrderDto);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while updating product order with ID {Id}", id);
                return StatusCode(500, "An error occurred while processing your request");
            }
        }

        // 7. DeleteProductOrder
        [HttpDelete("{id}")]
        public async Task<ActionResult> DeleteProductOrder(int id)
        {
            try
            {
                var productOrder = await _unitOfWork.Repository<ProductOrder>().GetByIdAsync(id);
                if (productOrder == null)
                {
                    return NotFound($"Product order with ID {id} not found");
                }

                await _unitOfWork.Repository<ProductOrder>().DeleteAsync(id);
                await _unitOfWork.CompleteAsync();

                return NoContent();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while deleting product order with ID {Id}", id);
                return StatusCode(500, "An error occurred while processing your request");
            }
        }

        // Bonus: Get ProductOrder statistics
        [HttpGet("statistics")]
        public async Task<ActionResult> GetProductOrderStatistics()
        {
            try
            {
                var allProductOrders = await _unitOfWork.Repository<ProductOrder>().GetAllAsync();

                var totalOrders = allProductOrders.Count;
                var totalQuantity = allProductOrders.Sum(po => po.Quantity);
                var totalRevenue = allProductOrders.Sum(po => po.UnitPrice * po.Quantity);

                return Ok(new
                {
                    TotalProductOrders = totalOrders,
                    TotalQuantityOrdered = totalQuantity,
                    TotalRevenue = totalRevenue
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while getting product order statistics");
                return StatusCode(500, "An error occurred while processing your request");
            }
        }
    }
}
