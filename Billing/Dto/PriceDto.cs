using Core.Model;
using IoC;
using Settings;
using System;
using System.Collections.Generic;
using System.Text;

namespace Billing.DTO
{
    public class PriceDto : SkuDto
    {
        public PriceDto(Price price) : base(price.Sku)
        {
            this.PriceId = price.Id;
            this.DateCreated = new DateTimeOffset(price.DateCreated).ToUnixTimeSeconds();
            this.DateTill = new DateTimeOffset(price.DateCreated.AddMinutes(IocContainer.Get<ISettingsManager>().GetIntValue("price_minutes"))).ToUnixTimeSeconds();
            this.FinalPrice = price.FinalPrice;
            this.ShopComission = price.ShopComission;
            this.ShopName = price.Shop.Name;
        }
        public int PriceId { get; set; }
        public long DateCreated { get; set; }
        public string ShopName { get; set; }
        public long DateTill { get; set; }
        public decimal FinalPrice { get; set; }
        public decimal ShopComission { get; set; }

    }
}
