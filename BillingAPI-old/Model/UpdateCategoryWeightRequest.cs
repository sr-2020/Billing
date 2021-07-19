using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BillingAPI.Model
{
    public class UpdateCategoryWeightRequest
    {
        public int CategoryId { get; set; }
        public decimal Weight { get; set; }
    }
}
