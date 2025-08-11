using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ShopSpire.Utilities.DTO.Order
{
    public class OrderDTO
    {
        public int OrderId { get; set; }
        public string UserId { get; set; }
        public DateTime OrderDate { get; set; }
        public decimal TotalAmount { get; set; }
        public string Status { get; set; }
        public string? ShippingAddress { get; set; }
        public List<ProductOrderDTO> Items { get; set; } = new();
    }
}
