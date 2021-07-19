using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BillingAPI.Model
{
    public class AddScoringCategoryRequest
    {
        public string CategoryName { get; set; }
        public bool Relative { get; set; }
        public decimal Weight { get; set; }
    }
}
