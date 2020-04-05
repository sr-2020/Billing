using Core.Model;
using System;
using System.Collections.Generic;
using System.Text;

namespace Billing.DTO
{
    public class ProductTypeDto
    {
        public ProductTypeDto(ProductType productType)
        {
            this.ProductTypeId = productType.Id;
            this.ProductTypeName = productType.Name;
        }
        public int ProductTypeId { get; set; }
        public string ProductTypeName { get; set; }
    }
}
