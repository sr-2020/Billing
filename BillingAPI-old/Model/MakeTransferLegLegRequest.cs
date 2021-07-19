using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BillingAPI.Model
{
    public class MakeTransferLegLegRequest: ShopBasedRequest
    {
        public int ShopTo { get; set; }
        public decimal Amount { get; set; }
        public string Comment { get; set; }
    }
}
