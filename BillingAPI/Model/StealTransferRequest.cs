using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BillingAPI.Model
{
    public class StealTransferRequest
    {
        public int From { get; set; }
        public int To { get; set; }
        public decimal Amount { get; set; }
        public string Comment { get; set; }
    }
}
