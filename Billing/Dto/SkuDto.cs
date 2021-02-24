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
            this.CorporationName = sku.Corporation?.Name;
            this.Enabled = sku.Enabled;
            this.CorporationLogo = sku.Corporation?.CorporationLogoUrl;
            this.CorporationId = sku.CorporationId;
        }
        public SkuDto() : base() { }

        public int SkuId { get; set; }
        public string SkuName { get; set; }
        public int Count { get; set; }
        public int CorporationId { get; set; }
        public string CorporationName { get; set; }
        public bool Enabled { get; set; }
        public string CorporationLogo { get; set; }
    }
}
