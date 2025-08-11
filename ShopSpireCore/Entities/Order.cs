using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ShopSpireCore.Entities
{
    public class Order:BaseEntity
    {
        public string UserId { get; set; }
        public User User { get; set; }
        public DateTime OrderDate { get; set; } = DateTime.UtcNow;
        [Column(TypeName = "decimal(18,2)")]
        public decimal TotalAmount { get; set; }

        public OrderStatus Status { get; set; } = OrderStatus.Pending;
        public ICollection<ProductOrder> ProductOrders { get; set; } = new List<ProductOrder>();
    }
}
