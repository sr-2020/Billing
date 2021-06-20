using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BillingAPI.Model
{
    public class HackShopRequest
    {
        public int ShopId { get; set; }
        public int[] Models { get; set; }
    }
}
