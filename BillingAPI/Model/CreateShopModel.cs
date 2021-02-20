using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BillingAPI.Model
{
    public class CreateShopModel
    {
        public int ShopId { get; set; }
        public decimal Amount { get; set; }
        public int LifeStyle { get; set; }
        public string Name { get; set; }
        public int Owner { get; set; }
    }
}
