using Core.Model;
using System;
using System.Collections.Generic;
using System.Text;

namespace Billing.DTO
{
    public class NomenklaturaDto : ProductTypeDto
    {
        public NomenklaturaDto(Nomenklatura nomenklatura) : base(nomenklatura.ProductType)
        {
            this.BasePrice = nomenklatura.BasePrice;
            this.NomenklaturaId = nomenklatura.Id;
            this.NomenklaturaName = nomenklatura.Name;
            this.LifeStyle = BillingHelper.GetLifeStyle(nomenklatura.Lifestyle).ToString();
            this.Code = nomenklatura.Code;
            this.Description = nomenklatura.Description;
            this.UrlPicture = nomenklatura.PictureUrl;
        }
        public int NomenklaturaId { get; set; }
        public string NomenklaturaName { get; set; }
        public string Code { get; set; }
        public string LifeStyle { get; set; }
        public decimal BasePrice { get; set; }
        public string Description { get; set; }
        public string UrlPicture { get; set; }
    }
}
