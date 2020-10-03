using System;
using System.Collections.Generic;
using System.Text;

namespace Billing.Dto
{
    public class InsuranceDto
    {
        public int SkuId { get; set; }
        public string SkuName { get; set; }
        public string LifeStyle { get; set; }
        public string ShopName { get; set; }
        public DateTime BuyTime { get; set; }
        public string PersonName { get; set; }
    }
}
