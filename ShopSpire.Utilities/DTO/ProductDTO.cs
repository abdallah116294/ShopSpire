using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ShopSpire.Utilities.DTO
{
    public class ProductDTO
    {
        public int Id { get; set; }              // From BaseEntity
        public string Name { get; set; }
        public decimal Price { get; set; }
        public string Description { get; set; }

        public int CategoryId { get; set; }
        public string CategoryName { get; set; } // Flattened from Category

        public string UserId { get; set; }
        public string SellerName { get; set; }   // Flattened from User/Selle
    }
}
