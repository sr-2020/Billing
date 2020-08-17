using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BillingAPI.Model
{
    public class WriteOfferRequest : ShopBasedRequest
    {
        public int RentaId { get; set; }
        public string Qr { get; set; }
    }
}
