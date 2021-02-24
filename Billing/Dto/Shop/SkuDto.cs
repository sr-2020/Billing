using Core.Model;
using System;
using System.Collections.Generic;
using System.Text;

namespace Billing.DTO
{
    public class SkuDto : NomenklaturaDto
    {
        public SkuDto(Sku sku) : base(sku?.Nomenklatura)
        {
            if (sku == null)
                return;
            this.SkuId = sku.Id;
            this.SkuName = sku.Name;
            this.Count = sku.Count;
            this.Enabled = sku.Enabled;
            this.CorporationId = sku.CorporationId;
        }
        public SkuDto() : base() { }

        public int SkuId { get; set; }
        public string SkuName { get; set; }
        public int Count { get; set; }
        public int CorporationId { get; set; }
        public bool Enabled { get; set; }
    }
}
