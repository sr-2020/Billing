using System;
using System.Collections.Generic;
using System.Text;

namespace Billing.DTO
{
    public class CorporationDto
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public decimal Balance { get; set; }
        public string CorporationUrl { get; set; }
    }
}
