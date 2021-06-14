using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BillingAPI.Model
{
    public class UpdateFactorCategoryRequest
    {
        public int FactorId { get; set; }
        public int NewCategoryId { get; set; }
    }
}
