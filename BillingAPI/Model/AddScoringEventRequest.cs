using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BillingAPI.Model
{
    public class AddScoringEventRequest
    {
        public int FactorId { get; set; }
        public int Lifestyle { get; set; }
        public decimal Value { get; set; }
    }
}
