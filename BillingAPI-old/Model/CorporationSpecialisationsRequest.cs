using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BillingAPI.Model
{
    public class CorporationSpecialisationsRequest : CorporationBasedRequest
    {
        public int Specialisation { get; set; }
        public decimal Ratio { get; set; }
    }
}
