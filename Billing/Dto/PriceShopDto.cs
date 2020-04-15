using System;
using System.Collections.Generic;
using System.Text;

namespace Billing.DTO
{
    public class PriceShopDto 
    {
        public int IdOffer { get; set; }
        public long DateCreated { get; set; }
        public string ShopName { get; set; }
        public long DateTill { get; set; }
        public decimal FinalPrice { get; set; }
        public decimal ShopComission { get; set; }
        public string SkuName { get; set; }
        public int Count { get; set; }
        public string CorporationName { get; set; }
        public string CorporationLogo { get; set; }
        public string NomenklaturaName { get; set; }
        public string LifeStyle { get; set; }
        public string Description { get; set; }
        public string UrlPicture { get; set; }
        public string ProductTypeName { get; set; }
        public PriceShopDto(PriceDto dto)
        {
            this.IdOffer = dto.PriceId;
            this.DateCreated = dto.DateCreated;
            this.ShopName = dto.ShopName;
            this.ShopComission = dto.ShopComission;
            this.DateTill = dto.DateTill;
            this.FinalPrice = dto.FinalPrice;
            this.SkuName = dto.SkuName;
            this.Count = dto.Count;
            this.CorporationLogo = dto.CorporationLogo;
            this.CorporationName = dto.CorporationName;
            this.NomenklaturaName = dto.NomenklaturaName;
            this.LifeStyle = dto.LifeStyle;
            this.Description = dto.Description;
            this.UrlPicture = dto.UrlPicture;
            this.ProductTypeName = dto.ProductTypeName;
        }
    }
}
