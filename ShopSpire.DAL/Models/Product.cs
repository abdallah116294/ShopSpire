using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ShopSpire.DAL.Models
{
    public class Product
    {
        #region Property
        public int ProductId { get; set; }
        public string Name { get; set; }
        public decimal Price { get; set; }
        public string Description { get; set; }
        [ForeignKey("Category")]
        public int  CategoryID { get; set; }
        public Category Category { get; set; }
        [ForeignKey("Seller")]
        public string UserId { get; set; }
        public User Seller { get; set; }
        #endregion

    }
}
