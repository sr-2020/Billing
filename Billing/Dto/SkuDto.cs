using System;
using System.Collections.Generic;
using System.Text;

namespace Billing.DTO
{
    public class SkuDto
    {
        public string SkuName { get; set; }
        public int Count { get; set; }
        public decimal BasePrice { get; set; }
        public string CorporationName { get; set; }
        public string NomenklaturaName { get; set; }
        public string TypeName { get; set; }
        public string Description { get; set; }
        public string Hidden { get; set; }
    }
}
