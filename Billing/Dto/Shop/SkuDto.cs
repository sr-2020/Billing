using Core.Model;
using System;
using System.Collections.Generic;
using System.Text;

namespace Billing.DTO
{
    public class SkuDto : NomenklaturaDto
    {
        public SkuDto(Sku sku, bool main) : base(sku?.Nomenklatura, false)
        {
            if (sku == null)
                return;
            if(main)
            {
                this.Id = sku.Id;
                Name = sku.Name;
            }
            this.SkuId = sku.Id;
            this.SkuName = sku.Name;
            this.Count = sku.Count;
            this.Enabled = sku.Enabled;
            this.CorporationId = sku.CorporationId;
            this.SkuBaseCount = sku.SkuBaseCount;
            this.SkuBasePrice = sku.SkuBasePrice;
            this.Price = sku.Price;
        }
        public int SkuId { get; set; }
        public string SkuName { get; set; }
        public int Count { get; set; }
        public int CorporationId { get; set; }
        public bool Enabled { get; set; }
        public int? SkuBasePrice { get; set; }
        public int? SkuBaseCount { get; set; }
        public decimal Price { get; set; }
    }
}
