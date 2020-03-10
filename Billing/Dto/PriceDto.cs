using System;
using System.Collections.Generic;
using System.Text;

namespace Billing.DTO
{
    public class PriceDto
    {
        public int PriceId { get; set; }
        public DateTime DateTill { get; set; }
        public decimal FinalPrice { get; set; }
        //public decimal CurrentScoring { get; set; }
        //public decimal Discount { get; set; }
    }
}
