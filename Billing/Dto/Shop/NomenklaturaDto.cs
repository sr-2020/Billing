using Core.Model;
using System;
using System.Collections.Generic;
using System.Text;

namespace Billing.DTO
{
    public class NomenklaturaDto : SpecialisationDto
    {
        public NomenklaturaDto(Nomenklatura nomenklatura, bool main) 
            : base(nomenklatura?.Specialisation, false)
        {
            if (nomenklatura == null)
                return;
            if(main)
            {
                this.Id = nomenklatura.Id;
                this.Name = nomenklatura.Name;
            }
            this.BasePrice = nomenklatura.BasePrice;
            this.NomenklaturaId = nomenklatura.Id;
            this.NomenklaturaName = nomenklatura.Name;
            this.LifeStyleId = nomenklatura.Lifestyle;
            this.LifeStyle = BillingHelper.GetLifestyle(nomenklatura.Lifestyle).ToString();
            this.Code = nomenklatura.Code;
            this.Description = nomenklatura.Description;
            if (string.IsNullOrEmpty(nomenklatura.PictureUrl))
                this.PictureUrl = nomenklatura?.Specialisation?.ProductType?.PictureUrl ?? "";
            else
                this.PictureUrl = nomenklatura.PictureUrl;
            this.BaseCount = nomenklatura.BaseCount;
        }
        public int NomenklaturaId { get; set; }
        public string NomenklaturaName { get; set; }
        public string Code { get; set; }
        public int BaseCount { get; set; }
        public int LifeStyleId { get; set; }
        public string LifeStyle { get; set; }
        public decimal BasePrice { get; set; }
        public string Description { get; set; }
        public string PictureUrl { get; set; }
    }
}
