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
            }
            this.BasePrice = nomenklatura.BasePrice;
            this.NomenklaturaId = nomenklatura.Id;
            this.NomenklaturaName = nomenklatura.Name;
            this.LifeStyleId = nomenklatura.Lifestyle;
            this.Code = nomenklatura.Code;
            this.Description = nomenklatura.Description;
            this.PictureUrl = nomenklatura.PictureUrl;
            this.BaseCount = nomenklatura.BaseCount;
            this.Secret = nomenklatura.Secret;
        }
        public NomenklaturaDto() : base() { }
        public int NomenklaturaId { get; set; }
        public string NomenklaturaName { get; set; }
        public string Code { get; set; }
        public int BaseCount { get; set; }
        public int LifeStyleId { get; set; }
        public string LifeStyle { get; set; }
        public decimal BasePrice { get; set; }
        public string Description { get; set; }
        public string PictureUrl { get; set; }
        public string Secret { get; set; }
    }
}
