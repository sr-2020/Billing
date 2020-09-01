using System;
using System.Collections.Generic;
using System.Text;

namespace Billing.Dto.Shop
{
    public class CorporationDto : OrganisationBase
    {

        public string CorporationUrl { get; set; }
        public decimal SalesAmount { get; set; }
    }
}
