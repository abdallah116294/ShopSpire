using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ShopSpireCore.Entities
{
   public   class ProductOrder:BaseEntity
    {
        [ForeignKey("Order")]
        public int OrderId { get; set; }
        public Order Order { get; set; }
        [ForeignKey("Product")]
        public int ProductId { get; set; }
        public Product Product { get; set; }
        public int Quantity { get; set; }
        // Optional: Store price at time of order (important for historical data)
        [Column(TypeName = "decimal(18,2)")]
        public decimal UnitPrice { get; set; }

        // Optional: Calculate total for this line item
        [NotMapped]
        public decimal LineTotal => UnitPrice * Quantity;

        // Optional: Timestamps
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}
