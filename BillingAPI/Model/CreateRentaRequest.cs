using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BillingAPI.Model
{
    public class CreateRentaRequest : CharacterBasedRequest
    {
        public int PriceId { get; set; }
        public int Count { get; set; }
    }
}
