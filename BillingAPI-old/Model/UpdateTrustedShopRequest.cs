using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BillingAPI.Model
{
    public class UpdateTrustedShopRequest: ShopBasedRequest
    {
        public List<int> TrustedModels { get; set; }
    }
}
