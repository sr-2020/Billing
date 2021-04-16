using Core.Model;
using Core.Primitives;
using System;
using System.Collections.Generic;
using System.Text;

namespace Billing.Dto
{
    public class ProductTypeDto
    {
        public ProductTypeDto(ProductType pt, bool main)
        {
            if (pt == null)
            {
                return;
            }
            if (main)
            {
                Id = pt.Id;
                Name = pt.Name;
            }
            DiscountType = pt.DiscountType;
            ProductTypeName = pt.Name;
            ProductTypeId = pt.Id;
            InstantConsume = pt.InstantConsume;
            Alias = pt.Alias;
        }
        public int Id { get; set; }
        public int ProductTypeId { get; set; }
        public string ProductTypeName { get; set; }
        public string Name { get; set; }
        public string Alias { get; set; }
        public int DiscountType { get; set; }
        public bool InstantConsume { get; set; }
    }
}
