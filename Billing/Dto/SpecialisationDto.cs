using System;
using System.Collections.Generic;
using System.Text;

namespace Billing.DTO
{
    public class SpecialisationDto
    {
        public int ShopId { get; set; }
        public string ShopName { get; set; }
        public int ProductTypeId { get; set; }
        public string ProductTypeName { get; set; }
    }
}
