using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ShopSpire.DAL.Models
{
    public class Category
    {
        #region Property
        public int CategoryId { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        #endregion
    }
}
