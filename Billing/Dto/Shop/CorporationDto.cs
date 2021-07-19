using Billing.DTO;
using Core.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Billing.Dto.Shop
{
    public class CorporationDetailedDto: CorporationDto
    {
        public CorporationDetailedDto(CorporationWallet corporation, List<Specialisation> specialisations, List<CorporationSpecialisation> corpspecs) : base(corporation)
        {
            Specialisations = specialisations.Select(s => new SpecialisationDto(s, true, (corpspecs.FirstOrDefault(c=>c.SpecialisationId == s.Id)?.Ratio) ?? 0)).ToList();
        }
        public List<SpecialisationDto> Specialisations { get; set; }
    }


    public class CorporationDto : OrganisationBaseDto
    {
        public CorporationDto(CorporationWallet corporation) : base(0, corporation)
        {
            CorporationUrl = corporation.CorporationLogoUrl;
            CurrentKPI = corporation.CurrentKPI;
            LastKPI = corporation.LastKPI;
            CurrentSkuSold = corporation.SkuSold;
            LastSkuSold = corporation.LastSkuSold;
        }
        public string CorporationUrl { get; set; }
        public decimal CurrentKPI { get; set; }
        public decimal LastKPI { get; set; }
        public decimal CurrentSkuSold { get; set; }
        public decimal LastSkuSold { get; set; }
    }
}
