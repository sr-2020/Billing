using System;
using System.Collections.Generic;
using System.Text;

namespace Billing.DTO
{
    public class ProductTypeDto
    {
        public int Id { get; set; }
        public string Code { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string LifeStyle { get; set; }
    }
}
