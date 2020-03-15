using Core.Model;
using System;
using System.Collections.Generic;
using System.Text;

namespace Billing.DTO
{
    public class RentaDto
    {
        public decimal FinalPrice { get; set; }
        public string ProductType { get; set; }
        public string Shop { get; set; }
    }
}
