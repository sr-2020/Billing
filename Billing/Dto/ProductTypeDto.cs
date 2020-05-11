using Core.Model;
using FileHelper;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;

namespace Billing.DTO
{
    public class ProductTypeDto
    {
        public ProductTypeDto(ProductType productType)
        {
            this.ProductTypeId = productType.Id;
            this.ProductTypeName = productType.Name;
            this.DiscountType = productType.DiscountType;
        }
        public ProductTypeDto() { }
        public int ProductTypeId { get; set; }
        [Column(0, false)]
        public string ProductTypeName { get; set; }
        [DisplayName("тип скидки(1 или 2)")]
        [Column(1, false)]
        public int DiscountType { get; set; }
    }
}
