using Core.Model;
using System;
using System.Collections.Generic;
using System.Text;

namespace Billing.Dto.Shop
{
    public class CorporationDto : OrganisationBaseDto
    {
        public CorporationDto(CorporationWallet corporation) : base(corporation)
        {
            CorporationUrl = corporation.CorporationLogoUrl;
            CurrentKPI = corporation.CurrentKPI;
            LastKPI = corporation.LastKPI;
        }
        public string CorporationUrl { get; set; }
        public decimal CurrentKPI { get; set; }
        public decimal LastKPI { get; set; }
    }
}
