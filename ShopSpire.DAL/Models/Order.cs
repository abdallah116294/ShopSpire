﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ShopSpire.DAL.Models
{
    public class Order
    {
        #region Property
        public int OrderId { get; set; }
        [ForeignKey("User")]
        public string UserId { get; set; }
        public User User { get; set; }
        public DateTime Date { get; set; }
        #endregion
    }
}
