using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BillingAPI.Model
{
    public class StealRentaRequest
    {
        public int RentaId { get; set; }
        public int? To { get; set; }
    }
}
