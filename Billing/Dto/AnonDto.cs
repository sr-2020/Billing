using System;
using System.Collections.Generic;
using System.Text;

namespace Billing.Dto
{
    public class AnonDto
    {
        public decimal Amount { get; set; }
        public string From { get; set; }
        public string To { get; set; }
        public DateTime OperationTime { get; set; }
    }
}
