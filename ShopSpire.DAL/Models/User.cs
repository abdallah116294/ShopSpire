using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ShopSpire.DAL.Models
{
    public class User: IdentityUser
    {
        #region Property
        public string FirstName { get; set; }
        public string LastName { get; set; }
        #endregion
    }
}
