using Core.Model;
using System;
using System.Collections.Generic;
using System.Text;

namespace Billing.Dto.Shop
{
    public class CorporationDto : OrganisationBase
    {
        public CorporationDto(CorporationWallet corporation) : base(corporation)
        {
            CorporationUrl = corporation.CorporationLogoUrl;
        }
        public string CorporationUrl { get; set; }
    }
}
