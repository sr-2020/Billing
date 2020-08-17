using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BillingAPI.Model
{
    public class MakeTransferLegSINRequest : ShopBasedRequest
    {
        public int Sin { get; set; }
        public decimal Amount { get; set; }
        public string Comment { get; set; }
    }
}
